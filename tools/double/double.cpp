#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

PROCESS_INFORMATION piProcInfo;
HANDLE hSaveStdout = INVALID_HANDLE_VALUE;
HANDLE hSaveStderr = INVALID_HANDLE_VALUE;
HANDLE hChildStdoutRd = INVALID_HANDLE_VALUE, hChildStdoutRdDummy = INVALID_HANDLE_VALUE, hChildStdoutWr = INVALID_HANDLE_VALUE;
HANDLE hFile = INVALID_HANDLE_VALUE;

BOOL CtrlHandler(DWORD fdwCtrlType)
{
    if (piProcInfo.hProcess != 0)
        TerminateProcess(piProcInfo.hProcess, -1);


    if (hSaveStdout != INVALID_HANDLE_VALUE)
        SetStdHandle(STD_OUTPUT_HANDLE, hSaveStdout);
    if (hSaveStderr != INVALID_HANDLE_VALUE)
        SetStdHandle(STD_ERROR_HANDLE, hSaveStderr);

    if (hChildStdoutRd != INVALID_HANDLE_VALUE)
        CloseHandle(hChildStdoutRd);
    if (hChildStdoutWr != INVALID_HANDLE_VALUE)
        CloseHandle(hChildStdoutWr);
    if (hFile != INVALID_HANDLE_VALUE)
        CloseHandle(hFile);

    printf("The process has been broken\n");

    return FALSE;
}


/** Read process output.

    @param szCmdLine        Command line to start the process
    @param szFileName       Name of the file to write process output
    @param bDuplicate       Duplicate output to STDOUT
  */
int duplicateOutput(const char *szCmdLine, const char *szFileName, bool bDuplicate)
{
    SECURITY_ATTRIBUTES saAttr;

    saAttr.nLength = sizeof(SECURITY_ATTRIBUTES);
    saAttr.bInheritHandle = TRUE;
    saAttr.lpSecurityDescriptor = NULL;

    BOOL fSuccess;

    hSaveStdout = GetStdHandle(STD_OUTPUT_HANDLE);
    hSaveStderr = GetStdHandle(STD_ERROR_HANDLE);


    if (!CreatePipe(&hChildStdoutRdDummy, &hChildStdoutWr, &saAttr, 0))
       return 1;

    if (!SetStdHandle(STD_OUTPUT_HANDLE, hChildStdoutWr))
       return 1;

    if (!SetStdHandle(STD_ERROR_HANDLE, hChildStdoutWr))
       return 1;

    fSuccess = DuplicateHandle(GetCurrentProcess(), hChildStdoutRdDummy,
                               GetCurrentProcess(), &hChildStdoutRd, 0,
                               FALSE, DUPLICATE_SAME_ACCESS);
    if(!fSuccess)
        return 1;

    CloseHandle(hChildStdoutRdDummy);



    hFile = CreateFile(szFileName, GENERIC_WRITE, 0, 0,
                       FILE_SHARE_WRITE,
                       FILE_ATTRIBUTE_ARCHIVE | FILE_FLAG_WRITE_THROUGH, 0);

    if (hFile == INVALID_HANDLE_VALUE)
        return 1;

    //execute children application
    STARTUPINFO siStartInfo;


    ZeroMemory(&piProcInfo, sizeof(PROCESS_INFORMATION));

    //Set up members of the STARTUPINFO structure.
    ZeroMemory( &siStartInfo, sizeof(STARTUPINFO) );
    siStartInfo.cb = sizeof(STARTUPINFO);

    // Create the child process.
    fSuccess = CreateProcess(NULL,
                             const_cast<char *>(szCmdLine),
                             NULL,          // process security attributes
                             NULL,          // primary thread security attributes
                             TRUE,          // handles are inherited
                             0,             // creation flags
                             NULL,          // use parent's environment
                             NULL,          // use parent's current directory
                             &siStartInfo,  // STARTUPINFO pointer
                             &piProcInfo);  // receives PROCESS_INFORMATION

    if (!fSuccess)
        return 1;

    while (WaitForSingleObject(piProcInfo.hProcess, 0) == WAIT_TIMEOUT)
    {
        while(true)
        {
            DWORD dwBytesAvail = 0;
            DWORD dwRead = 0, dwWritten;
            BYTE bBuff[4096];
            DWORD i;

            PeekNamedPipe(hChildStdoutRd, 0, 0, 0, &dwBytesAvail, 0);

            if (dwBytesAvail == 0)
                break;

            if (dwBytesAvail > 4096)
                dwBytesAvail = 4096;

            if (!ReadFile(hChildStdoutRd, bBuff, dwBytesAvail, &dwRead, NULL))
                break;

            WriteFile(hSaveStdout, bBuff, dwRead, &dwWritten, NULL);
            WriteFile(hFile, bBuff, dwRead, &dwWritten, NULL);
        };
    }

    while(true)
    {
        DWORD dwBytesAvail;
        PeekNamedPipe(hChildStdoutRd, 0, 0, 0, &dwBytesAvail, 0);

        BYTE b;
        DWORD dwRead, dwWritten;

        if (dwBytesAvail == 0)
            break;

        if (!ReadFile(hChildStdoutRd, &b, 1, &dwRead, NULL))
            break;

        if (dwRead == 0)
            break;

        WriteFile(hSaveStdout, &b, 1, &dwWritten, NULL);
        WriteFile(hFile, &b, 1, &dwWritten, NULL);
    };

    DWORD rc = 0;
    GetExitCodeProcess(piProcInfo.hProcess, &rc);

    CloseHandle(piProcInfo.hProcess);
    CloseHandle(piProcInfo.hThread);

    SetStdHandle(STD_OUTPUT_HANDLE, hSaveStdout);
    SetStdHandle(STD_ERROR_HANDLE, hSaveStderr);

    CloseHandle(hChildStdoutRd);
    CloseHandle(hChildStdoutWr);
    CloseHandle(hFile);

    return rc;
};

/** Add second string to first with quoting if string has spaces.

    @param szStr1       [in/out]    first string
    @param szStr2       [in]        second string
  */
void strscat(char *szStr1, const char *szStr2)
{
    bool bHasSpace = false;
    int i;

    for (i = 0; i < strlen(szStr2) && !bHasSpace; i++)
        if (szStr2[i] == ' ')
            bHasSpace = true;

    if (bHasSpace)
        strcat(szStr1, "\"");
    strcat(szStr1, szStr2);
    if (bHasSpace)
        strcat(szStr1, "\"");
}

int main (int argc, char *argv[])
{
    if (argc < 2)
    {
        printf("This utility executes specified command line\n"
               "and duplicates all output to stdout and stderr to\n"
               "specified file\n"
               "usage: double output_file_name command_line...\n"
               "example: double makefile.err nmake /f myproject.mak\n");
        return 1;
    }

    SetConsoleCtrlHandler((PHANDLER_ROUTINE)CtrlHandler, TRUE);

    char szCmdLine[1024] = "";
    for (int i = 2; i < argc; i++)
    {
        if (i != 2)
            strcat(szCmdLine, " ");
        strscat(szCmdLine, argv[i]);
    }

    int rc = duplicateOutput(szCmdLine, argv[1], true);
    return rc;
};
