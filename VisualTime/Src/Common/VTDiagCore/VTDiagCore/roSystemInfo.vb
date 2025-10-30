Imports Robotics.Mail

Namespace Robotics.Diagnostics

    Public Class roSystemInfo

        Public Shared Function canSendEmail(ByVal destination As String, ByVal header As String, ByVal body As String, ByVal strFileName As String, ByRef oState As VTDiagCore.wscDiagnosticsState) As String
            Try
                If destination <> String.Empty Then
                    Return SendMail.SendMail(destination, header, body, strFileName, Nothing, Nothing, "", "")
                Else
                    Return "Error"
                End If
            Catch ex As Exception
                oState.Result = VTDiagCore.wscDiagnosticsState.ResultEnum.FileAccessError
                Return ex.Message
            End Try
        End Function

    End Class

End Namespace