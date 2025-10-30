Imports System.Text.RegularExpressions
Imports Robotics.Base.VTServiceApi
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace roSendMessage

    Public Class SendMessage

        Public Shared Function SendMessage(ByVal strMessage As String) As String
            Dim strRet As String = "OK"

            Try
                If Not sendSMS(strMessage) Then strRet = "OK"
            Catch e As Exception
                strRet = "KO"

                If strRet <> "OK" Then
                    roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roSendSMS::SendMessage:Error: " & e.StackTrace)
                End If
            End Try
            Return strRet
        End Function

        Private Shared Function sendSMS(strMessage As String) As Boolean
            Dim bRet As Boolean
            Try
                Dim sMessageParse As String() = strMessage.Split(";") 'userLoginName & ";" & "" & ";" & strSMSDestination & ";" & databaseIdentifier & ";" & strSMSCode & ";"
                Dim sCegidEmail As String = sMessageParse(2)


                Dim emailRegex As String = "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$"

                If Regex.IsMatch(sCegidEmail, emailRegex) Then
                    Dim oApiSvc As New roServiceApiManager
                    bRet = oApiSvc.SendSms(sMessageParse(4).ToLower(), sCegidEmail.ToLower(), sMessageParse(3).ToLower())
                    If bRet Then
                        roTrace.GetInstance().AddTraceEvent($"SMS::OK:{strMessage}")
                    Else
                        roTrace.GetInstance().AddTraceEvent($"SMS::KO:{strMessage}")
                    End If
                Else
                    bRet = False
                    roTrace.GetInstance().AddTraceEvent($"SMS::Not an email:{strMessage}")
                End If


            Catch ex As Exception
                bRet = False
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roSendSMS::SendMessage:Error: " & ex.StackTrace)
            End Try

            Return bRet
        End Function

    End Class

End Namespace