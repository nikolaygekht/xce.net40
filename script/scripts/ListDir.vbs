Option Explicit

Sub ListDir()
    Dim path, s, i, f, c

    If system.FileNameFromPath(xce.FileName) = "DirectoryList" Then
        path = xce.Line(0)
        If xce.CursorLine > 0 And xce.CursorLine < xce.LinesCount Then
            path = system.CombinePaths(path, xce.Line(CursorLine))
        End If
        If Not system.DirectoryExists(path) Then
            path = "."
        End If
    Else
        path = "."
    End If

    path = xce.Prompt("Directory", path, 40, "List Dir")
    If DirectoryExists(path) Then
        xce.ExecuteCommandWithParam "OpenFile", "DirectoryList"
        xce.SaveRequired = False
        xce.CursorLine = 0

        While xce.LinesCount > 1
            xce.ExecuteCommand "DeleteLine"
        Wend
        xce.CursorColumn = 0
        xce.ExecuteCommand "DeleteToEndOfLine"

        path = system.GetFullPath(path)
        xce.CursorColumn = 0
        xce.Stroke(path)

        c = 1
        s = system.Directories(path)
        For i = LBound(s) to UBound(s)
            xce.CursorLine = c
            c = c + 1
            xce.CursorColumn = 0
            xce.Stroke s(i)
        Next

        s = system.Files(path, "*.*")
        For i = LBound(s) to UBound(s)
            xce.CursorLine = c
            c = c + 1
            xce.CursorColumn = 0
            xce.Stroke s(i)
        Next
        xce.CursorLine = 0
        xce.CursorColumn = 0
    Else
        xce.MessageBox "Directory does not exists", "List Dir"
    End If
End Sub