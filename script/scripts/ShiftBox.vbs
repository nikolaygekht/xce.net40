Option Explicit

Sub ShiftBlockLeft()
    ShiftBlock False
End Sub

Sub ShiftBlockRight()
    ShiftBlock True
End Sub


Sub ShiftBlock(ShiftRight)
    Dim fillChar, OldInsertMode, cl, cc, i, j, sc

    If xce.BlockMode <> xce.BlockType_Box and xce.BlockMode <> xce.BlockType_Line Then
        xce.MessageBox "Box block must be chosen", "Shift Block"
        Exit Sub
    End If

    If xce.BlockMode = xce.BlockType_Box Then
        sc = xce.BlockStartColumn
    End If

    If xce.BlockMode = xce.BlockType_Line Then
        sc = 0
    End If

    If xce.InsertMode Then
        OldInsertMode = True
    Else
        xce.ExecuteCommand "InsertMode"
        OldInsertMode = False
    End If

    cl = xce.CursorLine
    cc = xce.CursorColumn

    For i = xce.BlockStartLine To xce.BlockEndLine
        xce.CursorLine = i
        xce.CursorColumn = sc

        If ShiftRight Then
            xce.Stroke " "
        Else
            If xce.CursorColumn < xce.CurrentLineLength Then
                xce.ExecuteCommand "Delete"
            End If
        End If
    Next

    If Not OldInsertMode Then
        xce.ExecuteCommand "InsertMode"
    End If

    xce.CursorLine = cl
    xce.CursorColumn = cc
End Sub
