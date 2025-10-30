Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase

Public Class RoValidateIDSupport
    '
    ' Clase con funciones de propósito general de ValidateID
    '

    Private sSubscriptionName As String = ""
    Private sSubscriptionPass As String = ""
    Private sURLApi As String = "" ' --https://pre-vidsignercloud.validatedid.com/api/
    Private sDeviceName As String = ""

#Region "Declarations - Constructor"

    Public Sub New()

    End Sub

#End Region

    Private Function GetConfiguration() As Boolean
        Dim bolret = False
        Try
            Dim sql As String = ""
            If sSubscriptionName.Length = 0 Then

                sql = "@SELECT# isnull(Data,'') from sysroParameters where ID = 'VID_SName'"
                sSubscriptionName = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            If sSubscriptionPass.Length = 0 Then
                sql = "@SELECT# isnull(Data,'') from sysroParameters where ID = 'VID_SPwd'"
                sSubscriptionPass = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            If sURLApi.Length = 0 Then
                sql = "@SELECT# isnull(Data,'') from sysroParameters where ID = 'VID_URL'"
                sURLApi = roTypes.Any2String(AccessHelper.ExecuteScalar(sql))
            End If

            bolret = True
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::GetConfiguration :", ex)
        End Try

        Return bolret

    End Function

    Public Function PostDocument(ByVal aDocDTO As DocumentDTO) As DocumentVID_PostDocResult
        ' Subimos el documento a la plataforma para poder ser firmado
        '
        ' Obtenemos el GUID del docmumento
        ' Obtenemos el GUID del firmante
        ' Obtenemos la URL del documento para el firmante

        Dim oPostDocResult As New DocumentVID_PostDocResult

        Try
            oPostDocResult.IsValid = False

            GetConfiguration()

            oPostDocResult.IsValid = False
            oPostDocResult.GUID = ""
            oPostDocResult.SignerGUI = ""
            oPostDocResult.SignatureViDRemoteURL = ""

            Dim Client As HttpClient = New HttpClient()
            Client.BaseAddress = New Uri(sURLApi)
            Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sSubscriptionName & ":" + sSubscriptionPass)))
            Client.Timeout = New TimeSpan(0, 0, 100)

            Dim json = JsonConvert.SerializeObject(aDocDTO)
            Dim content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
            ' Subimos el documento
            Dim responseGuid = Client.PostAsync("documents/", content).Result

            If responseGuid.IsSuccessStatusCode Then
                Dim OB As Object
                OB = JsonConvert.DeserializeObject(Of DocGuiDTO)(responseGuid.Content.ReadAsStringAsync().Result)
                ' Obtenemos el GUID del documento
                If OB IsNot Nothing Then
                    oPostDocResult.GUID = OB.DocGUI
                End If

                If oPostDocResult.GUID.Length > 0 Then
                    ' Obtenemos la info del documento
                    Dim oDocumentInfoDTO As DocumentInfoDTO = GetDocumentInfo(oPostDocResult.GUID, Client)
                    If oDocumentInfoDTO IsNot Nothing AndAlso oDocumentInfoDTO.Signers IsNot Nothing AndAlso oDocumentInfoDTO.Signers.Length > 0 Then
                        oPostDocResult.SignerGUI = oDocumentInfoDTO.Signers(0).SignerGUI
                    End If

                    ' Obtenemos la URL del signante
                    If oPostDocResult.SignerGUI.Length > 0 Then
                        Dim oSignatureViDRemoteURLDTO As SignatureViDRemoteURLDTO = GetSignatureVIDRemoteUrl(oPostDocResult.SignerGUI, Client)
                        If OB IsNot Nothing Then
                            oPostDocResult.SignatureViDRemoteURL = oSignatureViDRemoteURLDTO.SignatureViDRemoteURL
                            ' Hay que concatenar la URL de retorno en base 64 aHR0cHM6Ly93d3cuZ29vZ2xlLmVz en base 64
                            ' por ejemplo--https://google.es --> aHR0cHM6Ly93d3cuZ29vZ2xlLmVz
                            ' --https://pre-vidremote.vidsigner.net/loading?emailid=ffb1bf14-cc90-4cfa-9c89-f47db5a923ed&url_return=aHR0cHM6Ly93d3cuZ29vZ2xlLmVz
                            oPostDocResult.IsValid = True
                        End If
                    End If
                End If
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roInfo, "RoValidateIDSupport::PostDocument:: VIDSigner returned error on callback: Reason: " & responseGuid.ReasonPhrase)
            End If
        Catch ex As Exception
            oPostDocResult.IsValid = False
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::PostDocument :", ex)
        End Try

        Return oPostDocResult

    End Function

    Public Function GetDocumentInfo(ByVal GUIDDocument As String, Optional _Client As HttpClient = Nothing) As DocumentInfoDTO
        '
        ' Obtenemos la info del documento en ValidateID
        '
        Dim oDocumentInfoDTO As DocumentInfoDTO = Nothing

        Try

            GetConfiguration()

            Dim Client As HttpClient = _Client
            If IsNothing(Client) Then
                Client = New HttpClient
                Client.BaseAddress = New Uri(sURLApi)
                Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sSubscriptionName & ":" + sSubscriptionPass)))
                Client.Timeout = New TimeSpan(0, 0, 30)
            End If

            ' Obtenemos la info del documento
            Dim responseURL = Client.GetAsync("documentinfo/" & GUIDDocument).Result
            If responseURL.IsSuccessStatusCode Then
                oDocumentInfoDTO = JsonConvert.DeserializeObject(Of DocumentInfoDTO)(responseURL.Content.ReadAsStringAsync().Result)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::GetDocumentInfo :", ex)
        End Try

        Return oDocumentInfoDTO
    End Function

    Public Function GetSignatureVIDRemoteUrl(ByVal SignerGUI As String, Optional _Client As HttpClient = Nothing) As SignatureViDRemoteURLDTO
        '
        ' Obtenemos la URL del signante
        '
        Dim oSignatureViDRemoteURLDTO As SignatureViDRemoteURLDTO = Nothing

        Try
            GetConfiguration()

            Dim Client As HttpClient = _Client
            If IsNothing(Client) Then
                Client = New HttpClient
                Client.BaseAddress = New Uri(sURLApi)
                Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sSubscriptionName & ":" + sSubscriptionPass)))
                Client.Timeout = New TimeSpan(0, 0, 30)
            End If

            ' Obtenemos la url del signante
            Dim responseURL = Client.GetAsync("signature/" & SignerGUI & "/vidremoteurl").Result
            If responseURL.IsSuccessStatusCode Then
                oSignatureViDRemoteURLDTO = JsonConvert.DeserializeObject(Of SignatureViDRemoteURLDTO)(responseURL.Content.ReadAsStringAsync().Result)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::GetSignatureVIDRemoteUrl :", ex)
        End Try

        Return oSignatureViDRemoteURLDTO
    End Function

    Public Function GetSignedDocument(ByVal GUIDDocument As String, Optional _Client As HttpClient = Nothing) As SignedDocumentDTO
        '
        ' Obtenemos el documento signado
        '
        Dim oSignedDocumentDTO As SignedDocumentDTO = Nothing

        Try
            GetConfiguration()
            Dim Client As HttpClient = _Client
            If IsNothing(Client) Then
                Client = New HttpClient
                Client.BaseAddress = New Uri(sURLApi)
                Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sSubscriptionName & ":" + sSubscriptionPass)))
                Client.Timeout = New TimeSpan(0, 0, 30)
            End If

            ' Obtenemos la info del documento
            Dim responseURL = Client.GetAsync("signeddocuments/" & GUIDDocument).Result
            If responseURL.IsSuccessStatusCode Then
                oSignedDocumentDTO = JsonConvert.DeserializeObject(Of SignedDocumentDTO)(responseURL.Content.ReadAsStringAsync().Result)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::GetSignedDocument :", ex)
        End Try

        Return oSignedDocumentDTO
    End Function

    Public Function GetSignedDocumentReport(ByVal GUIDDocument As String, Optional _Client As HttpClient = Nothing) As SignedDocumentReportDTO
        '
        ' Obtenemos el informe con las evidencias del documento
        '
        Dim oSignedDocumentReportDTO As SignedDocumentReportDTO = Nothing

        Try
            GetConfiguration()

            Dim Client As HttpClient = _Client
            If IsNothing(Client) Then
                Client = New HttpClient
                Client.BaseAddress = New Uri(sURLApi)
                Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Basic", Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(sSubscriptionName & ":" + sSubscriptionPass)))
                Client.Timeout = New TimeSpan(0, 0, 30)
            End If

            ' Obtenemos la info del documento
            Dim responseURL = Client.GetAsync("signeddocuments/" & GUIDDocument & "/report").Result
            If responseURL.IsSuccessStatusCode Then
                oSignedDocumentReportDTO = JsonConvert.DeserializeObject(Of SignedDocumentReportDTO)(responseURL.Content.ReadAsStringAsync().Result)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::GetSignedDocumentReport :", ex)

        End Try

        Return oSignedDocumentReportDTO
    End Function

    Public Function CreateDocWithRemoteSigner(ByVal DocData As DocumentVID_BasicData, ByVal LanguageKey As String) As DocumentDTO
        Dim aDocDTO As DocumentDTO = New DocumentDTO()
        Dim aSignersArray As SignerDTO() = New SignerDTO(0) {}

        Try
            Dim aSigner As SignerDTO = createRemoteSigner(DocData, LanguageKey)
            aSignersArray(0) = aSigner
            aDocDTO.IssuerName = DocData.IssuerName ' "Nombre emisor"
            aDocDTO.FileName = DocData.FileName '"Trial_Remote.pdf"
            aDocDTO.OrderedSignatures = True
            aDocDTO.Signers = aSignersArray
            aDocDTO.DocContent = getPDFB64(DocData.FileBytes)
            aDocDTO.ExpirationDate = Format(Now.AddHours(1), "dd/MM/yyyy HH:mm:ss")
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::CreateDocWithRemoteSigner :", ex)
        Finally
        End Try

        Return aDocDTO

    End Function

    Private Function createRemoteSigner(ByVal DocData As DocumentVID_BasicData, ByVal LanguageKey As String) As SignerDTO
        Dim aSigner As SignerDTO = New SignerDTO()
        Try
            aSigner.DeviceName = ""
            aSigner.eMail = DocData.Email
            aSigner.NumberID = DocData.NIF
            aSigner.SignerName = DocData.EmployeeName
            aSigner.TypeOfID = "Nif"
            aSigner.SignatureType = "emailandsms"
            aSigner.Language = GetLanguageCode(LanguageKey)
            aSigner.PhoneNumber = DocData.PhoneNumber
            aSigner.SkipSignatureEmail = True
            Dim aVisible As VisibleDTO = New VisibleDTO()
            aVisible.Page = -1
            aVisible.PosX = 100
            aVisible.PosY = 240
            aVisible.SizeX = 50
            aVisible.SizeY = 20
            aSigner.Visible = aVisible
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "RoValidateIDSupport::CreateDocWithRemoteSigner :", ex)
        Finally
        End Try

        Return aSigner
    End Function

    Private Function getPDFB64(pdfBytes As Byte()) As String
        'Dim pdfBytes As Byte() = File.ReadAllBytes("c:\work\testdoc.pdf")
        Return Convert.ToBase64String(pdfBytes)
    End Function

    Public Function GetLanguageCode(ByVal LanguageKey As String) As String
        ' Vidsigner: SignatureType=emailandsms => Languages allowed: ar, br, ca, de, es, en, eu, fi, fr, gl, hu, it, ko, nl, no, pl, pt, ro, zh
        Dim allowedLanguages As New HashSet(Of String)({"ar", "br", "ca", "de", "es", "en", "eu", "fi", "fr", "gl", "hu", "it", "ko", "nl", "no", "pl", "pt", "ro", "zh"})
        Dim languageResult As String = "es"

        If Not String.IsNullOrEmpty(LanguageKey) Then
            Dim languageCode As String = LanguageKey.Substring(0, 2).ToLower()
            If allowedLanguages.Contains(languageCode) Then
                languageResult = languageCode
            End If
        End If

        Return languageResult
    End Function


End Class