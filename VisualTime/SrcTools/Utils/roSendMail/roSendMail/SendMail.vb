Imports System
Imports System.Collections.Generic
Imports System.ComponentModel
Imports System.Text
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.VTBase.Extensions
Imports EASendMail
Imports Robotics.Base.DTOs
Imports System.Net

Namespace roSendMail
    Public Class roMailServer
        Public Property SmtpClient As SmtpClient
    End Class


    <ComClass(SendMail.ClassId, SendMail.InterfaceId, SendMail.EventsId)> _
    Public Class SendMail

        Private _debug As Boolean

#Region "GUID de COM"
        ' Estos GUID proporcionan la identidad de COM para esta clase 
        ' y las interfaces de COM. Si las cambia, los clientes 
        ' existentes no podrán obtener acceso a la clase.
        Public Const ClassId As String = "597d6972-9f69-45d0-883a-8a2e2e5f494a"
        Public Const InterfaceId As String = "9455fc69-ec59-477b-a8b0-f3cbaa5b1e89"
        Public Const EventsId As String = "021343f3-8c10-4d91-a3ed-baa5c5af8a61"
#End Region

        'Private oLog As New Robotics.VTBase.roLog("roSendMail")
        'Private bWriteLog As Boolean = False

        Public Sub New()
            MyBase.New()
        End Sub

        Public Property SMTPDebug() As Boolean
            Get
                Return _debug
            End Get
            Set(ByVal value As Boolean)
                _debug = value
            End Set
        End Property


        Private Function IsValidEmail(ByVal strIn As String) As Boolean
            Try
                Dim a As New System.Net.Mail.MailAddress(strIn)
            Catch
                Return False
            End Try
            Return True
        End Function


        ''' <summary>
        ''' Función que envia e-mail/s según los parámetros de conexión del registro y de la BBDD
        ''' Previamente, realiza una validación de los mails de destino. 
        ''' Si alguno no tiene un formato válido, lo desecha y continúa con el siguiente.  
        ''' </summary>
        ''' <param name="MsgTo">Cadena con la relación de e-mails, separados por ';' de los destinatarios que recibirán el e-mail</param>
        ''' <param name="strSubject">Cadena con el mail del remitente</param>
        ''' <param name="strBody">Cuerpo del mensaje</param>
        ''' <param name="strFileName">Ruta del fichero a adjuntar ó "" si no hay fichero </param>
        ''' <remarks></remarks>
        Public Function SendMail(ByVal MsgTo As String, ByVal strSubject As String, ByVal strBody As String, ByVal strFileName As String, byteArray As Byte()) As String
            Dim strRet As String = "OK"
            Dim smtpServer As String = ""
            Dim smtpPort As Integer = 0
            Dim smtpUser As String = ""
            Dim smtpPassword As String = ""
            Dim smtpDomain As String = ""
            Dim strMailFrom As String = ""
            Dim strMailTo As String = MsgTo
            Dim strConnType As String = "ANONYMOUS"
            Dim bolUseSSL As Boolean = False
            'Dim objVTS As VTSInitTask
            'Dim objServer As Object
            'Dim oSec As VTMain.roSecurityBase64

            Try
                'objVTS = New VTSInitTask
                'objServer = objVTS.ConnectToServer()

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                'ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Dim oParameters As New roParameters("OPTIONS")

                'Recuperamos los datos de la base de datos
                smtpServer = Any2String(oParameters.Parameter(Parameters.MailServer))
                If smtpServer.Length = 0 Then
                    strRet = "Smtp Server is empty"
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail:: " + strRet)
                    Return strRet
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
                Try
                    If Encoding.UTF8.GetString(Convert.FromBase64String(smtpPassword)).Contains("@@_Visualtime_@@") Then
                        'Estaba codificada ...
                        smtpPassword = Encoding.UTF8.GetString(Convert.FromBase64String(smtpPassword)).Replace("@@_Visualtime_@@", "")
                    End If
                Catch ex As Exception
                    ' No estaba codificado
                End Try
                strConnType = Any2String(oParameters.Parameter(Parameters.MailAuthentication))
                smtpDomain = Any2String(oParameters.Parameter(Parameters.MailDomain))
                bolUseSSL = Any2Boolean(oParameters.Parameter(Parameters.UseSSL))

                'objServer = Nothing
                'objVTS = Nothing
            Catch ex As Exception
                strRet = "ERROR:" & ex.Message.ToString & " " & ex.StackTrace.ToString & "--->" & Environment.NewLine &
                         "MsgTo: " & MsgTo & ",strSubject: " & strSubject & ",strFileName :" & strFileName

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail::" + strRet + "::Error:", ex)
                Return strRet
            End Try

            '******************************************************************************************
            '              EASendMail
            '******************************************************************************************
            Dim errStr As String = ""
            Try
                Dim oMail As SmtpMail = New SmtpMail("ES-E1582190613-00480-46DBFD974DB8FA45-54E62683E961B15U") 'New SmtpMail("ES-AA1141023508-00728-4F20430C37133BD3AABA3B0D940D0B97")
                Dim oSmtp As SmtpClient = New SmtpClient

                oMail.From = New MailAddress(strMailFrom)
                oMail.Bcc = New AddressCollection(MsgTo)

                oMail.Subject = strSubject

                If strBody.IndexOf("<html") >= 0 Or strBody.IndexOf("<div") >= 0 Or (strBody.IndexOf("</") >= 0 And strBody.IndexOf("<") >= 0) Then
                    oMail.ImportHtml(strBody, System.AppDomain.CurrentDomain.BaseDirectory, ImportHtmlBodyOptions.ImportLocalPictures)
                Else
                    oMail.TextBody = strBody
                End If

                If strFileName.Length > 0 Then
                    If Not IsNothing(byteArray) Then
                        oMail.AddAttachment(strFileName, byteArray)
                    Else
                        oMail.AddAttachment(strFileName)
                    End If
                End If

                Dim oServer As SmtpServer = New SmtpServer(smtpServer, smtpPort)

                If Not strConnType = "Anonima" Then
                    oServer.AuthType = SmtpAuthType.AuthAuto
                    oServer.User = smtpUser
                    oServer.Password = smtpPassword
                End If

                If (bolUseSSL) Then
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto
                End If
                If _debug Then oSmtp.LogFileName = IO.Path.Combine(IO.Directory.GetCurrentDirectory, "SendMail_" + Now.ToString("yyyyMMdd") + ".log")

                oSmtp.Timeout = 60
                oSmtp.SendMail(oServer, oMail)
                strRet = "OK"
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roSendMail::SendMail:email sended.(to:" + MsgTo + "; subject:" + strSubject + ")")

            Catch exp As SmtpTerminatedException
                errStr = exp.Message
            Catch exp As SmtpServerException
                errStr = String.Format("Exception: Server Respond: {0}", exp.ErrorMessage)
            Catch exp As System.Net.Sockets.SocketException
                errStr = String.Format("Exception: Networking Error: {0} {1}", exp.ErrorCode, exp.Message)
            Catch exp As System.ComponentModel.Win32Exception
                errStr = String.Format("Exception: System Error: {0} {1}", exp.ErrorCode, exp.Message)
            Catch exp As System.Exception
                errStr = String.Format("Exception: Common: {0}", exp.Message)
            End Try
            If errStr.Length > 0 Then
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roSendMail::SendMail:Error:" + errStr)
                strRet = "ERROR:" + errStr + Environment.NewLine
                strRet += "MsgTo: " + MsgTo + ",strSubject: " + strSubject + ",strFileName :" + strFileName
            End If
            Return strRet

        End Function

        ''' <summary>
        ''' Función que envia e-mail/s según los parámetros de conexión del registro y de la BBDD
        ''' Previamente, realiza una validación de los mails de destino. 
        ''' Si alguno no tiene un formato válido, lo desecha y continúa con el siguiente.  
        ''' </summary>
        ''' <param name="MsgTo">Cadena con la relación de e-mails, separados por ';' de los destinatarios que recibirán el e-mail</param>
        ''' <param name="strSubject">Cadena con el mail del remitente</param>
        ''' <param name="strBody">Cuerpo del mensaje</param>
        ''' <param name="strFileName">Ruta del fichero a adjuntar ó "" si no hay fichero </param>
        ''' <remarks></remarks>
        Public Function SendMailVB6(ByVal MsgTo As String, ByVal strSubject As String, ByVal strBody As String, ByVal strFileName As String) As String
            Dim strRet As String = "OK"
            Dim smtpServer As String = ""
            Dim smtpPort As Integer = 0
            Dim smtpUser As String = ""
            Dim smtpPassword As String = ""
            Dim smtpDomain As String = ""
            Dim strMailFrom As String = ""
            Dim strMailTo As String = MsgTo
            Dim strConnType As String = "ANONYMOUS"
            Dim bolUseSSL As Boolean = False
            'Dim objVTS As VTSInitTask
            'Dim objServer As Object
            'Dim oSec As VTMain.roSecurityBase64

            Try
                'objVTS = New VTSInitTask
                'objServer = objVTS.ConnectToServer()

                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12
                'ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12

                Dim oParameters As New roParameters("OPTIONS")

                'Recuperamos los datos de la base de datos
                smtpServer = Any2String(oParameters.Parameter(Parameters.MailServer))
                If smtpServer.Length = 0 Then
                    strRet = "Smtp Server is empty"
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail:: " + strRet)
                    Return strRet
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
                Try
                    If Encoding.UTF8.GetString(Convert.FromBase64String(smtpPassword)).Contains("@@_Visualtime_@@") Then
                        'Estaba codificada ...
                        smtpPassword = Encoding.UTF8.GetString(Convert.FromBase64String(smtpPassword)).Replace("@@_Visualtime_@@", "")
                    End If
                Catch ex As Exception
                    ' No estaba codificado
                End Try
                strConnType = Any2String(oParameters.Parameter(Parameters.MailAuthentication))
                smtpDomain = Any2String(oParameters.Parameter(Parameters.MailDomain))
                bolUseSSL = Any2Boolean(oParameters.Parameter(Parameters.UseSSL))

                'objServer = Nothing
                'objVTS = Nothing
            Catch ex As Exception
                strRet = "ERROR:" & ex.Message.ToString & " " & ex.StackTrace.ToString & "--->" & Environment.NewLine &
                         "MsgTo: " & MsgTo & ",strSubject: " & strSubject & ",strFileName :" & strFileName

                roLog.GetInstance().logMessage(roLog.EventType.roError, "roSendMail::SendMail::" + strRet + "::Error:", ex)
                Return strRet
            End Try

            '******************************************************************************************
            '              EASendMail
            '******************************************************************************************
            Dim errStr As String = ""
            Try
                Dim oMail As SmtpMail = New SmtpMail("ES-E1582190613-00480-46DBFD974DB8FA45-54E62683E961B15U") 'New SmtpMail("ES-AA1141023508-00728-4F20430C37133BD3AABA3B0D940D0B97")
                Dim oSmtp As SmtpClient = New SmtpClient

                oMail.Reset()

                oMail.From = New MailAddress(strMailFrom)
                oMail.Bcc = New AddressCollection(MsgTo)

                oMail.Subject = strSubject

                If strBody.IndexOf("<html") >= 0 Or strBody.IndexOf("<div") >= 0 Or (strBody.IndexOf("</") >= 0 And strBody.IndexOf("<") >= 0) Then
                    oMail.ImportHtml(strBody, System.AppDomain.CurrentDomain.BaseDirectory, ImportHtmlBodyOptions.ImportLocalPictures)
                Else
                    oMail.TextBody = strBody
                End If

                If strFileName.Length > 0 Then
                    oMail.AddAttachment(strFileName)
                End If

                Dim oServer As SmtpServer = New SmtpServer(smtpServer, smtpPort)

                If Not strConnType = "Anonima" Then
                    oServer.AuthType = SmtpAuthType.AuthAuto
                    oServer.User = smtpUser
                    oServer.Password = smtpPassword
                End If

                If (bolUseSSL) Then
                    oServer.ConnectType = SmtpConnectType.ConnectSSLAuto
                End If
                If _debug Then oSmtp.LogFileName = IO.Path.Combine(IO.Directory.GetCurrentDirectory, "SendMail_" + Now.ToString("yyyyMMdd") + ".log")

                oSmtp.Timeout = 60
                oSmtp.SendMail(oServer, oMail)
                strRet = "OK"
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roDebug, "roSendMail::SendMail:email sended.(to:" + MsgTo + "; subject:" + strSubject + ")")

            Catch exp As SmtpTerminatedException
                errStr = exp.Message
            Catch exp As SmtpServerException
                errStr = String.Format("Exception: Server Respond: {0}", exp.ErrorMessage)
            Catch exp As System.Net.Sockets.SocketException
                errStr = String.Format("Exception: Networking Error: {0} {1}", exp.ErrorCode, exp.Message)
            Catch exp As System.ComponentModel.Win32Exception
                errStr = String.Format("Exception: System Error: {0} {1}", exp.ErrorCode, exp.Message)
            Catch exp As System.Exception
                errStr = String.Format("Exception: Common: {0}", exp.Message)
            End Try
            If errStr.Length > 0 Then
                roLog.GetInstance().logMessage(Robotics.VTBase.roLog.EventType.roError, "roSendMail::SendMail:Error:" + errStr)
                strRet = "ERROR:" + errStr + Environment.NewLine
                strRet += "MsgTo: " + MsgTo + ",strSubject: " + strSubject + ",strFileName :" + strFileName
            End If
            Return strRet

        End Function

    End Class
End Namespace