Imports System.Net.Http
Imports System.Net.Http.Headers
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports ServiceApi

Public Class roServiceApiManager
    Private oState As roServiceApiManagerState = Nothing
    Private sConnectionString As String = String.Empty

    Public ReadOnly Property State As roServiceApiManagerState
        Get
            Return oState
        End Get
    End Property

    Private ReadOnly Property ServiceApiURL As String
        Get
            If sConnectionString.Split("@").Length = 3 Then
                Return sConnectionString.Split("@")(0)
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private ReadOnly Property ServiceApiUser As String
        Get
            If sConnectionString.Split("@").Length = 3 Then
                Return sConnectionString.Split("@")(1)
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Private ReadOnly Property ServiceApiToken As String
        Get
            If sConnectionString.Split("@").Length = 3 Then
                Return sConnectionString.Split("@")(2)
            Else
                Return String.Empty
            End If
        End Get
    End Property

    Public Sub New()
        Me.oState = New roServiceApiManagerState()
        sConnectionString = Robotics.VTBase.roTypes.Any2String(Robotics.VTBase.roConstants.GetConfigurationParameter("ApiService.ConnectionString"))
    End Sub

    Public Sub New(ByVal _State As roServiceApiManagerState)
        Me.oState = _State
        sConnectionString = Robotics.VTBase.roTypes.Any2String(Robotics.VTBase.roConstants.GetConfigurationParameter("ApiService.ConnectionString"))
    End Sub

    Public Function GetCompanyTokens() As roCompanyToken
        Dim tokens As roCompanyToken = Nothing
        Dim sApiAuthToken As String = Me.Authenticate()

        If Me.oState.Result <> ServiceApiResultEnum.NoError Then Return Nothing

        Try
            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.BaseAddress = New Uri(ServiceApiURL)
            Client.Timeout = New TimeSpan(0, 0, 100)
            Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", sApiAuthToken)
            Client.DefaultRequestHeaders.Add("companyId", Azure.RoAzureSupport.GetCompanyName())
            Dim responseURL = Client.GetAsync("/api/sc/GetClientToken").Result
            If responseURL.IsSuccessStatusCode Then
                tokens = JsonConvert.DeserializeObject(Of roCompanyToken)(responseURL.Content.ReadAsStringAsync().Result)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GetCompanyTokens:: Bad Request: Response: " & responseURL.ReasonPhrase)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GetCompanyTokens:: " & ex.Message)
        End Try

        Return tokens
    End Function

    Public Function GenerateToken(position As Integer) As roCompanyToken
        Dim tokens As roCompanyToken = Nothing
        Dim sApiAuthToken As String = Me.Authenticate()

        If Me.oState.Result <> ServiceApiResultEnum.NoError Then Return Nothing

        Try
            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.BaseAddress = New Uri(ServiceApiURL)
            Client.Timeout = New TimeSpan(0, 0, 100)
            Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", sApiAuthToken)
            Client.DefaultRequestHeaders.Add("companyId", Azure.RoAzureSupport.GetCompanyName())
            Dim responseURL = Client.PostAsync("/api/sc/ResetClientToken/" & position.ToString(), Nothing).Result
            If responseURL.IsSuccessStatusCode Then
                tokens = JsonConvert.DeserializeObject(Of roCompanyToken)(responseURL.Content.ReadAsStringAsync().Result)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GenerateToken:: Bad Request: Response: " & responseURL.ReasonPhrase)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GenerateToken:: " & ex.Message)
        End Try

        Return tokens
    End Function

    Public Function GetCompanyInfo(sDescriptor As String) As roCompanyInfo
        Dim companyInfo As roCompanyInfo = Nothing
        Dim sApiAuthToken As String = Me.Authenticate()

        If Me.oState.Result <> ServiceApiResultEnum.NoError Then Return Nothing

        Try
            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.BaseAddress = New Uri(ServiceApiURL)
            Client.Timeout = New TimeSpan(0, 0, 100)
            Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", sApiAuthToken)
            Client.DefaultRequestHeaders.Add("companyId", "")
            Dim responseURL = Client.GetAsync("/api/DEX/GetCompany/" & sDescriptor).Result
            If responseURL.IsSuccessStatusCode Then
                companyInfo = JsonConvert.DeserializeObject(Of roCompanyInfo)(responseURL.Content.ReadAsStringAsync().Result)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GetCompanyInfo:: Bad Request: Response: " & responseURL.ReasonPhrase)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GetCompanyInfo:: " & ex.Message)
        End Try

        Return companyInfo
    End Function

    Public Function GetCompanyToken(sCompanyName As String) As String
        Dim companyInfo As String = Nothing
        Dim sApiAuthToken As String = Me.Authenticate()

        If Me.oState.Result <> ServiceApiResultEnum.NoError Then Return Nothing

        Try
            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.BaseAddress = New Uri(ServiceApiURL)
            Client.Timeout = New TimeSpan(0, 0, 100)
            Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", sApiAuthToken)
            Client.DefaultRequestHeaders.Add("companyId", sCompanyName)
            Dim responseURL = Client.GetAsync("/api/DEX/GetURL").Result
            If responseURL.IsSuccessStatusCode Then
                companyInfo = roTypes.Any2String(responseURL.Content.ReadAsStringAsync().Result)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GetDEXurl:: Bad Request: Response: " & responseURL.ReasonPhrase)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::GetDEXurl:: " & ex.Message)
        End Try

        Return companyInfo
    End Function

    Public Function SendSms(sMsgContent As String, sDestinationEmail As String, sCompanyId As String) As Boolean
        Dim bMsgSent As Boolean = False
        Dim sApiAuthToken As String = Me.Authenticate()

        If Me.oState.Result <> ServiceApiResultEnum.NoError Then Return Nothing

        Try
            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.BaseAddress = New Uri(ServiceApiURL)
            Client.Timeout = New TimeSpan(0, 0, 100)
            Client.DefaultRequestHeaders.Authorization = New AuthenticationHeaderValue("Bearer", sApiAuthToken)
            Client.DefaultRequestHeaders.Add("companyId", sCompanyId)

            Dim json = JsonConvert.SerializeObject(New With {.content = sMsgContent, .destination = sDestinationEmail})
            Dim sSms As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")

            Dim responseURL = Client.PostAsync("/api/SMS/Send", sSms).Result
            If responseURL.IsSuccessStatusCode Then
                bMsgSent = roTypes.Any2Boolean(responseURL.Content.ReadAsStringAsync().Result)
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::SendSms:: Bad Request: Response: " & responseURL.ReasonPhrase)
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roServiceApiManager::SendSms:: " & ex.Message)
        End Try

        Return bMsgSent
    End Function

    Public Function GetDEXurl() As String
        Dim url As String = Nothing
        Dim companyToken As String = Me.GetCompanyToken(Azure.RoAzureSupport.GetCompanyName())
        If companyToken IsNot Nothing AndAlso companyToken.Length > 0 Then
            url = "/DEX/" & companyToken
        End If
        Return url
    End Function

    Private Function Authenticate() As String
        Dim AuthTokenSC As String = String.Empty
        Try
            If ServiceApiURL = String.Empty OrElse ServiceApiToken = String.Empty OrElse ServiceApiUser = String.Empty Then
                Me.oState.Result = ServiceApiResultEnum.NoCredentials
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "WSAdministration::GetAuthTokenSC::Login:: Bad Request: ServiceApiURL: " & ServiceApiURL & " ServiceApiToken: " & ServiceApiToken & " ServiceApiUser: " & ServiceApiUser)
                Return String.Empty
            End If

            Dim Client As HttpClient = New HttpClient()
            Client.DefaultRequestHeaders.Add("User-Agent", "Robotics Service")
            Client.BaseAddress = New Uri(ServiceApiURL)
            Client.Timeout = New TimeSpan(0, 0, 100)

            Dim json = JsonConvert.SerializeObject(New With {.username = ServiceApiUser, .password = ServiceApiToken})
            Dim content As New Net.Http.StringContent(json, System.Text.Encoding.UTF8, "application/json")
            Dim response = Client.PostAsync("/api/authenticate/login", content).Result
            If response.IsSuccessStatusCode Then
                Dim oResult As String = response.Content.ReadAsStringAsync().Result
                Dim OB As roAuthenticationResponse = JsonConvert.DeserializeObject(Of roAuthenticationResponse)(oResult)
                AuthTokenSC = OB.token
            Else
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "WSAdministration::GetAuthTokenSC::Login:: Bad Request: Response: " & response.ReasonPhrase)
            End If
        Catch ex As Exception
            Me.oState.Result = ServiceApiResultEnum.ConnectionError
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roServiceApiManager::Authenticate::Could not authenticate", ex)
        End Try

        Return AuthTokenSC
    End Function

End Class