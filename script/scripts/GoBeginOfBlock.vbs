Option Explicit

Sub GoBeginOfBlock()
    Dim r, c
    Select Case xce.BlockMode
    Case xce.BlockType_None
        Exit Sub
    Case xce.BlockType_Line
        r = xce.BlockStartLine
        c = 0
    Case xce.BlockType_Box, xce.BlockType_Stream
        r = xce.BlockStartLine
        c = xce.BlockStartColumn
    End Select
    xce.CursorLine = r
    xce.CursorColumn = c
End Sub