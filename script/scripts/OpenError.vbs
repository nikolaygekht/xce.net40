Option Explicit

Sub OpenError()
    Dim f, s, l, file, line, message, rc

    file = ""
    line = -1
    message = ""

    rc = xce.FirstSyntaxRegion(CursorLine)
    s = xce.CurrentLine

    While rc
        If xce.SyntaxRegionName = "errfile:name" Then
            f = xce.SyntaxRegionStart + 1
            l = xce.SyntaxRegionLength

            file = Mid(s, f, l)
        End If

        If xce.SyntaxRegionName = "errfile:lineno" Then
            f = xce.SyntaxRegionStart + 1
            l = xce.SyntaxRegionLength

            line = CLng(Mid(s, f, l))
        End If
        If xce.SyntaxRegionName = "errfile:errcode" Then
            f = xce.SyntaxRegionStart + 1
            l = xce.SyntaxRegionLength

            message = Mid(s, f, l)
        End If
        rc = xce.NextSyntaxRegion
    Wend

    If Len(file) > 0 And line > 0 Then
        If Not(system.FileExists(file)) Then
            xce.MessageBox "File does not exist", "Open Error"
            Exit Sub
        End If
        xce.ExecuteCommandWithParam "OpenFile", file
        xce.CursorLine = line - 1
        If Len(message) > 0 Then
            xce.MessageBox message, "Error"
        End If
    End If
End Sub

Sub FindNextError()
    Dim ext, rc, f, r, c, sr
    ext = UCase(system.FileExtensionFromPath(xce.FileName))
    Select Case ext
    Case ".ERR"
        r = xce.CursorLine
        r = r + 1
        While r < xce.LinesCount
            rc = xce.FirstSyntaxRegion(r)
            f = False

            While rc And Not f
                If xce.SyntaxRegionName = "errfile:name" Then
                    f = True
                End If
                rc = xce.NextSyntaxRegion
            Wend
            If f Then
                xce.CursorLine = r
                xce.CursorColumn = 0
                Exit Sub
            End If
            r = r  + 1
        Wend
    Case ".DS"
        r = xce.CursorLine
        sr = r
        c = xce.CursorColumn
        While r < xce.LinesCount
            rc = xce.FirstSyntaxRegion(r)
            f = False
            While rc And Not f
                If xce.SyntaxRegionLength > 0 And xce.SyntaxRegionName = "ds:error" Then
                    If (r > sr) Or (r = sr And xce.SyntaxRegionStart > c) Then
                        xce.CursorLine = r
                        xce.CursorColumn = xce.SyntaxRegionStart
                        f = True
                    End If
                End If
                rc = xce.NextSyntaxRegion
            Wend
            If  f Then
                Exit Sub
            End If
            r = r + 1
        Wend
    End Select
    xce.MessageBox "No more errors found", "Error"
End Sub

Sub FindPreviousError()
    Dim rc, f, r
    r = xce.CursorLine
    r = r - 1
    While r >= 0
        rc = xce.FirstSyntaxRegion(r)
        f = False

        While rc And Not f
            If xce.SyntaxRegionName = "errfile:name" Then
                f = True
            End If
            rc = xce.NextSyntaxRegion
        Wend

        If f Then
            xce.CursorLine = r
            xce.CursorColumn = 0
            Exit Sub
        End If
        r = r - 1
    Wend

    xce.MessageBox "No more errors found", "Error"
End Sub

