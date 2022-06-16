#include <windows.h>
#include <stdio.h>
#include <stdlib.h>
#include <string.h>

/** Create installation script for one folder.

    @param szPath       Folder name with slash terminator
    @param pOut         File to be written
  */
void process(const char *szPath, FILE *pOut)
{
    char szMask[MAX_PATH];
    char szFile[MAX_PATH];

    strcpy(szMask, szPath);
    if (strlen(szMask))
        strcat(szMask, "\\");
    strcat(szMask, "*.*");

    HANDLE hFind;
    WIN32_FIND_DATA wfd;

    hFind = FindFirstFile(szMask, &wfd);
    if (hFind == INVALID_HANDLE_VALUE)
        return ;

    fprintf(pOut, "   SetOutPath $INSTDIR");
    if (strlen(szPath))
        fprintf(pOut, "\\%s", szPath);
    fprintf(pOut, "\n");

    do
    {
        //process only files on this step
        if ((wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != FILE_ATTRIBUTE_DIRECTORY)
        {
            if (!strlen(szPath) &&
                (!stricmp(wfd.cFileName, "xceinst.nsi") ||
                 !stricmp(wfd.cFileName, "xcefiles.nsi") ||
                 !stricmp(wfd.cFileName, "xcefiles1.nsi") ||
                 !stricmp(wfd.cFileName, "xceinst.bat") ||
                 !stricmp(wfd.cFileName, "xceinst.exe")))
                    continue;

            strcpy(szFile, szPath);
            if (strlen(szFile))
                strcat(szFile, "\\");
            strcat(szFile, wfd.cFileName);
            fprintf(pOut, "    File %s\n", szFile);
        }
    }while (FindNextFile(hFind, &wfd));
    FindClose(hFind);

    hFind = FindFirstFile(szMask, &wfd);
    if (hFind == INVALID_HANDLE_VALUE)
        return ;
    do
    {
        //process only files on this step
        if ((wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) == FILE_ATTRIBUTE_DIRECTORY)
        {
            if (wfd.cFileName[0] == '.')
                continue;
            strcpy(szFile, szPath);
            if (strlen(szFile))
                strcat(szFile, "\\");
            strcat(szFile, wfd.cFileName);
            process(szFile, pOut);
        }
    }while (FindNextFile(hFind, &wfd));
    FindClose(hFind);
}

/** Create installation script for one folder.

    @param szPath       Folder name with slash terminator
    @param pOut         File to be written
  */
void process1(const char *szPath, FILE *pOut)
{
    char szMask[MAX_PATH];
    char szFile[MAX_PATH];

    strcpy(szMask, szPath);
    if (strlen(szMask))
        strcat(szMask, "\\");
    strcat(szMask, "*.*");

    HANDLE hFind;
    WIN32_FIND_DATA wfd;

    hFind = FindFirstFile(szMask, &wfd);
    if (hFind == INVALID_HANDLE_VALUE)
        return ;
    do
    {
        //process only files on this step
        if ((wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) == FILE_ATTRIBUTE_DIRECTORY)
        {
            if (wfd.cFileName[0] == '.')
                continue;
            strcpy(szFile, szPath);
            if (strlen(szFile))
                strcat(szFile, "\\");
            strcat(szFile, wfd.cFileName);
            process1(szFile, pOut);
        }
    }while (FindNextFile(hFind, &wfd));
    FindClose(hFind);


    hFind = FindFirstFile(szMask, &wfd);


    do
    {
        //process only files on this step
        if ((wfd.dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY) != FILE_ATTRIBUTE_DIRECTORY)
        {
            if (!strlen(szPath) &&
                (!stricmp(wfd.cFileName, "xceinst.nsi") ||
                 !stricmp(wfd.cFileName, "xcefiles.nsi") ||
                 !stricmp(wfd.cFileName, "xcefiles1.nsi") ||
                 !stricmp(wfd.cFileName, "xceinst.bat") ||
                 !stricmp(wfd.cFileName, "xceinst.exe")))
                    continue;

            strcpy(szFile, szPath);
            if (strlen(szFile))
                strcat(szFile, "\\");
            strcat(szFile, wfd.cFileName);
            fprintf(pOut, "    Delete $INSTDIR\\%s\n", szFile);
        }
    }while (FindNextFile(hFind, &wfd));
    FindClose(hFind);
    fprintf(pOut, "   RmDir $INSTDIR");
    if (strlen(szPath))
        fprintf(pOut, "\\%s", szPath);
    fprintf(pOut, "\n");

}


/** Application entry point. */
int main(int argc, char *argv[])
{

    FILE *pOut = fopen("xcefiles.nsi", "w");

    if (!pOut)
    {
        printf("Can't open %s\n", "xcefiles.nsi");
        return -2;
    }

    process("", pOut);

    fclose(pOut);

    pOut = fopen("xcefiles1.nsi", "w");

    if (!pOut)
    {
        printf("Can't open %s\n", "xcefiles1.nsi");
        return -2;
    }

    process1("", pOut);

    fclose(pOut);
    return 0;
}
