Imports System.IO
Imports System.Net
Imports System.Net.Mail
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Public Class SendMail

    Private Shared _lockObj As Object = New Object
    Private Shared _instance As SmtpClient = Nothing

    Public Shared Function GetInstance(ByVal oSmtpConfig As roMailConfig, Optional ByVal bReload As Boolean = False) As SmtpClient
        Try

            If _instance Is Nothing OrElse bReload Then
                SyncLock _lockObj
                    Try
                        If _instance IsNot Nothing Then
                            _instance.Dispose()
                        End If
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::Error disposing smtp instance", ex)
                    End Try
                    _instance = GetSmptServer(oSmtpConfig)
                End SyncLock
            End If
        Catch ex As Exception
            _instance = Nothing
        End Try
        Return _instance
    End Function

    Public Sub New()
        MyBase.New()
    End Sub

    Private Shared Function IsValidEmail(ByVal strIn As String) As Boolean
        Try
            Dim a As New System.Net.Mail.MailAddress(strIn)
        Catch
            Return False
        End Try
        Return True
    End Function

    Public Shared Function GetSmptServer(Optional ByVal oSmtpConfig As roMailConfig = Nothing) As SmtpClient
        Dim smtpServer As String = ""
        Dim smtpPort As Integer = 0
        Dim smtpUser As String = ""
        Dim smtpPassword As String = ""
        Dim smtpDomain As String = ""
        Dim strMailFrom As String = ""
        Dim strConnType As String = "ANONYMOUS"
        Dim bolUseSSL As Boolean = False

        Try

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            If oSmtpConfig Is Nothing Then
                Dim oParameters As New roParameters("OPTIONS")

                smtpServer = Any2String(oParameters.Parameter(Parameters.MailServer))
                If smtpServer.Length = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail:: Smtp Server is empty")
                    Return Nothing
                End If

                If Any2Integer(oParameters.Parameter(Parameters.ServerPort)) <> 0 Then
                    smtpPort = Any2Integer(oParameters.Parameter(Parameters.ServerPort))
                Else
                    smtpPort = 25
                End If
                strMailFrom = Any2String(oParameters.Parameter(Parameters.MailAccount))
                smtpUser = Any2String(oParameters.Parameter(Parameters.MailUser))
                smtpPassword = Any2String(oParameters.Parameter(Parameters.MailPWD))
                ' Miramos si está codificado o no (el WIN32 se codificaba Base64 con el literal @@_VisualTime_@@ por delante)
                strConnType = Any2String(oParameters.Parameter(Parameters.MailAuthentication))
                smtpDomain = Any2String(oParameters.Parameter(Parameters.MailDomain))
                bolUseSSL = Any2Boolean(oParameters.Parameter(Parameters.UseSSL))
            Else
                'Recuperamos los datos de la base de datos
                smtpServer = oSmtpConfig.MailServer
                If smtpServer.Length = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail:: Smtp Server is empty")
                    Return Nothing
                End If

                If oSmtpConfig.ServerPort Then
                    smtpPort = oSmtpConfig.ServerPort
                Else
                    smtpPort = 25
                End If
                strMailFrom = oSmtpConfig.MailAccount
                smtpUser = oSmtpConfig.MailUser
                smtpPassword = oSmtpConfig.MailPWD
                ' Miramos si está codificado o no (el WIN32 se codificaba Base64 con el literal @@_VisualTime_@@ por delante)
                strConnType = oSmtpConfig.MailAuthentication
                smtpDomain = oSmtpConfig.MailDomain
                bolUseSSL = Parameters.UseSSL
            End If

            Dim client As New SmtpClient()
            client.Host = smtpServer
            client.Port = smtpPort
            If (bolUseSSL) Then
                client.EnableSsl = True
            Else
                client.EnableSsl = False
            End If
            ' The server requires user's credentials
            ' not the default credentials
            If strConnType <> "Anonima" Then
                client.UseDefaultCredentials = False
                client.Credentials = New System.Net.NetworkCredential(smtpUser, smtpPassword)
            Else
                client.UseDefaultCredentials = True
            End If

            ' Provide your credentials
            client.DeliveryMethod = SmtpDeliveryMethod.Network

            Return client
        Catch ex As Exception
            Dim errorMsg = "ERROR:" & ex.Message.ToString & " " & ex.StackTrace.ToString & "--->" & Environment.NewLine
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail::" & errorMsg & "::Error:", ex)
            Return Nothing
        End Try
    End Function

    Public Shared Function SendMail(ByVal MsgTo As String, ByVal strSubject As String, ByVal strBody As String, ByVal strFileName As String, byteArray As Byte(), ByVal oSmtpServer As SmtpClient, ByVal strFromAddress As String, ByVal sCompanyId As String, Optional ByVal strEncoding As String = Nothing) As String
        Dim strRet As String = "OK"

        If sCompanyId = String.Empty Then
            sCompanyId = "UNKNOWN"
        End If

        '******************************************************************************************
        '              EASendMail
        '******************************************************************************************
        Dim errStr As String = ""
        Dim bEndConnection As Boolean = False
        Dim bDebugEnvironment As Boolean = False
        Try

            Dim strMailFrom = String.Empty
            If oSmtpServer Is Nothing Then
                oSmtpServer = GetSmptServer()
                bEndConnection = True
            End If

            If strFromAddress = String.Empty Then
                Dim oParameters As New roParameters("OPTIONS")
                strMailFrom = Any2String(oParameters.Parameter(Parameters.MailAccount))
            Else
                strMailFrom = strFromAddress
            End If

            Try
                bDebugEnvironment = Robotics.VTBase.roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.Debug"))
            Catch ex As Exception
                bDebugEnvironment = False
            End Try

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

            Dim msg As New MailMessage()
            Using msg
                msg.From = New MailAddress(strMailFrom)

                If MsgTo.Contains(";") Then
                    Dim destinations As String() = MsgTo.Split(";")

                    For Each destination In destinations
                        Dim direction = destination.Trim
                        If IsValidEmail(direction) AndAlso direction <> String.Empty Then

                            If bDebugEnvironment Then
                                msg.To.Add(New MailAddress(direction))
                            Else
                                msg.Bcc.Add(New MailAddress(direction))
                            End If

                        End If
                    Next
                Else
                    If IsValidEmail(MsgTo) Then
                        If bDebugEnvironment Then
                            msg.To.Add(New MailAddress(MsgTo))
                        Else
                            msg.Bcc.Add(New MailAddress(MsgTo))
                        End If
                    End If
                End If

                If bDebugEnvironment Then
                    If msg.To.Count = 0 Then
                        roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, sCompanyId & "::roSendMail::SendMail:No valid destination:" & MsgTo)
                        Return "ERROR#NoValidDestination"
                    End If
                Else
                    If msg.Bcc.Count = 0 Then
                        roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, sCompanyId & "::roSendMail::SendMail:No valid destination:" & MsgTo)
                        Return "ERROR#NoValidDestination"
                    End If
                End If

                msg.Subject = strSubject
                If (strEncoding IsNot Nothing) Then
                    msg.SubjectEncoding = Text.Encoding.GetEncoding(strEncoding)
                End If

                If strBody.IndexOf("<html") >= 0 OrElse strBody.IndexOf("<div") >= 0 OrElse (strBody.IndexOf("</") >= 0 AndAlso strBody.IndexOf("<") >= 0) Then
                    msg.Body = strBody
                    msg.IsBodyHtml = True
                Else
                    msg.Body = strBody
                    msg.IsBodyHtml = False
                End If

                If strFileName.Length > 0 Then
                    If Not IsNothing(byteArray) Then
                        msg.Attachments.Add(New Attachment(New MemoryStream(byteArray), strFileName))
                    Else
                        msg.Attachments.Add(New Attachment(strFileName))
                    End If
                End If
                SyncLock _lockObj
                    oSmtpServer.Send(msg)
                End SyncLock

            End Using

            strRet = "OK"
            roTrace.GetInstance().AddTraceEvent($"{sCompanyId}::roSendMail::SendMail(to:{MsgTo}; subject:{strSubject}):OK.")

            If bEndConnection Then
                oSmtpServer.Dispose()
            End If
        Catch exp As SmtpException
            errStr = String.Format("Exception: Server Respond: {0} {1}", exp.StatusCode.ToString, exp.Message)
        Catch exp As System.Net.Sockets.SocketException
            errStr = String.Format("Exception: Networking Error: {0} {1}", exp.ErrorCode, exp.Message)
        Catch exp As System.ComponentModel.Win32Exception
            errStr = String.Format("Exception: System Error: {0} {1}", exp.ErrorCode, exp.Message)
        Catch exp As System.Exception
            errStr = String.Format("Exception: Common: {0}", exp.Message)
        End Try

        If errStr.Length > 0 Then
            roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, sCompanyId & "::roSendMail::SendMail(to:" & MsgTo & "; subject:" & strSubject & "):Error:" & errStr)
            strRet = "ERROR:" & errStr & Environment.NewLine
            strRet += "MsgTo: " & MsgTo & ",strSubject: " & strSubject & ",strFileName :" & strFileName
        End If
        Return strRet

    End Function

End Class