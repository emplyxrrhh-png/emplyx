Public Class Form1
    Private _debug As Boolean = False


    Private Sub Button2_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button2.Click
        Try
            If txtTo.Text.Trim <> String.Empty Then
                Me.Cursor = Cursors.WaitCursor
                Dim oSendMail As New roSendMail.roSendMail.SendMail()
                oSendMail.SMTPDebug = _debug
                'Dim ar As String = "p.ramirez@robotics.es;o.badiola@robotics.es,i.lopez@robotics.es"
                Dim strMsg As String = oSendMail.SendMail(txtTo.Text.Trim, Me.txtSubject.Text, txtBody.Text, Me.txtAttach.Text)

                MsgBox(strMsg)
                Me.Cursor = Cursors.Default
            Else
                MsgBox("debe especificar destinatario/s")
            End If
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MsgBox("Error: " & ex.Message)
        End Try
    End Sub

    Private Sub Form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If My.Application.CommandLineArgs.Count > 0 Then
            If My.Application.CommandLineArgs(0).ToUpper.IndexOf("DEBUG") >= 0 Then
                _debug = True
                lblDebug.Visible = True
            End If
        End If
    End Sub
End Class
