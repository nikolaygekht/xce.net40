Option Explicit

Sub AlignToCursor()
    Dim h, i, cc, OldInsertMode

    If xce.CursorLine >= xce.LinesCount Then
        Exit Sub
    End If

    If xce.CurrentLineLength < 1 Then
        Exit Sub
    End If

    cc = xce.CursorColumn
    xce.CursorColumn = 0
    h = FindHome(xce.CursorLine)

    If cc < h Then
        For i = 1 To h - cc
            xce.ExecuteCommand "Delete"
        Next
    Else
        If cc > h Then
            xce.CursorColumn = 0
            OldInsertMode = xce.InsertMode
            If Not xce.InsertMode Then
                xce.ExecuteCommand "InsertMode"
            End If

            For i = 1 To cc - h
                xce.Stroke(" ")
            Next

            If Not OldInsertMode Then
                xce.ExecuteCommand "InsertMode"
            End If
        End If
    End If

    xce.CursorColumn = cc
End Sub



Sub AlignOutline()
    Dim cl, cc, s, h

    cl = xce.CursorLine
    cc = xce.CursorColumn

    xce.ExecuteCommand("CheckPair")

    If cl <> CursorLine Then
        xce.CursorColumn = FindHome(CursorLine)
        xce.CursorLine = cl
        xce.ExecuteCommandWithParam "Script", "AlignToCursor"
    Else
        xce.CursorLine = cl
        xce.CursorColumn = cc
    End If
End Sub

Function FindHome(lineIndex)
    Dim s, h
    s = xce.Line(lineIndex)
    h = 1
    While h <= Len(s) and Mid(s, h, 1) = " "
        h = h + 1
    Wend
    FindHome = h - 1
End Function
