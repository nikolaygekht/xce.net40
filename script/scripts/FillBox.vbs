Option Explicit

Sub FillBox()
    Dim fillChar, OldInsertMode, cl, cc, i, j

    If xce.BlockMode <> xce.BlockType_Box Then
        xce.MessageBox "Box block must be chosen", "Fill Box"
        Exit Sub
    End If

   fillChar = xce.Prompt("Fill Char:", "", 3, "Fill Box")

    If (Len(fillChar) > 0) Then

        If xce.InsertMode Then
            OldInsertMode = True
            xce.ExecuteCommand "InsertMode"
        Else
            OldInsertMode = False
        End If

        cl = xce.CursorLine
        cc = xce.CursorColumn

        For i = xce.BlockStartLine To xce.BlockEndLine
            xce.CursorLine = i

            For j = xce.BlockStartColumn to xce.BlockEndColumn
                xce.CursorColumn = j
                xce.Stroke fillChar
            Next
        Next

        If OldInsertMode Then
            xce.ExecuteCommand "InsertMode"
        End If

        xce.CursorLine = cl
        xce.CursorColumn = cc
    End if
End Sub
