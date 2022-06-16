'Script use xce-project.ini file to find additional folder locations
'The file may be located at the source folder or any parent folder above
'The file may contain the following keys in the [project] section
'include                    - additional dirs to search include files (semicolon separated list of paths)
'source                     - dirs to search source files (semicolon separated list of paths)
'default-include-location   - default location of the include files (one path, probably relative)
'default-source-location    - default location of the source files (one path, probably relative)

Option Explicit

'
'Finds the value of the xce-project.ini for the file specified.
'
Function FindIncludePath(FileName, TypeSearch)
    Dim Path, t

    FindIncludePath = ""
    Path = system.DirectoryFromPath(system.GetFullPath(FileName))
    While True
        t = system.CombinePaths(Path, "xce-project.ini")
        If system.FileExists(t) Then
            FindIncludePath = system.GetProfileString(t, "project", TypeSearch)
            FindIncludePath = system.GetFullPath(system.CombinePaths(Path, FindIncludePath))
            Exit Function
        Else
            Path = system.ParentDirectory(Path)
            If Path = "" Then
                Exit Function
            End If
        End If
    Wend
End Function

'
'Find the file by it's name and type (type is either include or source)
'
Function FindIncludeFile(FileToFind, TypeSearch)
    Dim Path1, Path2, Path
    Path1 = FindIncludePath(FileName, TypeSearch)
    If TypeSearch = "include" Then
        Path2 = system.Environment("include")
    Else
        Path = ""
    End If
    Path = "."
    If Path1 <> "" Then
        Path = Path + ";" + Path1
    End If
    If Path2 <> "" Then
        Path = Path + ";" + Path2
    End If
    FindIncludeFile = FindInPaths(Path, FileToFind, system.DirectoryFromPath(system.GetFullPath(FileName)))
End Function

'
'Find source or header file for the currently open one
'
Sub ToggleSourceInclude()
    Dim ext, base, baseAndPath, name

    name = ""

    ext = UCase(system.FileExtensionFromPath(FileName))
    base = Mid(system.FileNameFromPath(FileName), 1, Len(system.FileNameFromPath(FileName)) - Len(ext))
    baseAndPath = Mid(FileName, 1, Len(FileName) - Len(ext))
    Select Case Ext
    Case ".H", ".HPP"
        name = FindIncludeFile(base + ".c", "source")
        If name = "" Then
            name = FindIncludeFile(base + ".cpp", "source")
        End If
        If name = "" Then
            name = FindIncludeFile(base + ".cxx", "source")
        End If
        If name = "" Then
            name = FindIncludePath(FileName, "default-source-location")
            If name = "" Then
                name = "."
            End If
            name = system.GetFullPath(system.CombinePaths(name, base + ".cpp"))
        End If
    Case ".C", ".CPP", ".CXX"
        name = FindIncludeFile(base + ".h", "include")
        If name = "" Then
            name = FindIncludeFile(base + ".hpp", "include")
        End If
        If name = "" Then
            name = FindIncludeFile(base + ".hxx", "include")
        End If
        If name = "" Then
            name = FindIncludePath(FileName, "default-include-location")
            If name = "" Then
                name = "."
            End If
            name = system.GetFullPath(system.CombinePaths(name, base + ".h"))
        End If
    End Select
    If Len(name) > 0 Then
        If Not system.FileExists(name) Then
            If Not xce.MessageBoxYesNo("File" + vbCrLf + name + vbCrLf + "does not exists. Open it anyway?", "Toggle File") Then
                Exit Sub
            End If
        End If
        xce.ExecuteCommandWithParam "OpenFile", name
    End IF
End Sub

'
'Find include file at current line
'
Sub OpenIncludeAtCurrentLine()
    Dim text, match, name, iname
    If xce.CursorLine >= 0 And xce.CursorLine < xce.LinesCount Then
        text = xce.Line(xce.CursorLine)
        match = system.RegexMatch("/#include\s*[\x22<]([^\x22>]+)[\x22>]/", text)
        If UBound(match) <= LBound(match) Then
            xce.MessageBox "Is not an #include line", ""
            Exit Sub
        End If
        If Len(match(1)) < 1 Then
            xce.MessageBox "#include line has no file", "Open Include"
            Exit Sub
        End If
        name = match(1)
        iname = FindIncludeFile(name, "include")
        If iname = "" Then
            xce.MessageBox "File" + vbCrLf + name + vbCrLf + "is not found", "Open Include"
            Exit Sub
        End If
        xce.ExecuteCommandWithParam "OpenFile", iname
    End If
End Sub