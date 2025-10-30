Imports System.Net
Imports System.Web
Imports System.Xml
Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.VTBase

Public Class XmlCacheSessionData

    Public Sub New()
        Xml = Nothing
        ValidUntil = DateTime.Now.AddDays(1)
    End Sub

    Public Property Xml As XmlDocument
    Public Property ValidUntil As DateTime

End Class

Public NotInheritable Class HelperSession

    Public Shared Function GetFeatureIsInstalledFromSession(ByVal strFeature As String) As Boolean
        Dim bRet As Boolean = False

        Try
            If WLHelperWeb.CurrentPassport(True) Is Nothing Then
                bRet = API.LicenseServiceMethods.FeatureIsInstalled(strFeature)
            Else
                Dim strKey As String = WLHelperWeb.CompanyToken & "_" & strFeature

                If HttpContext.Current.Session(strKey) Is Nothing Then
                    bRet = API.LicenseServiceMethods.FeatureIsInstalled(strFeature)
                    HttpContext.Current.Session.Add(strKey, bRet)
                Else
                    bRet = HttpContext.Current.Session(strKey)
                End If
            End If
        Catch
        End Try

        Return bRet

    End Function

    Public Shared Function GetFeatureIsInstalledFromApplication(ByVal strFeature As String, Optional ByVal _passportTicket As roPassportTicket = Nothing, Optional ByVal excludeState As Boolean = False) As Boolean
        Dim bRet As Boolean = False

        Try
            If WLHelperWeb.CurrentPassport(True) Is Nothing Then
                bRet = API.LicenseServiceMethods.FeatureIsInstalled(strFeature)
            Else
                bRet = GetFeatureIsInstalledFromSession(strFeature)
            End If
        Catch
        End Try

        Return bRet

    End Function

    Public Shared Sub DeleteKeyFromSession(ByVal strKey As String)
        Try
            strKey = WLHelperWeb.CompanyToken & "_" & strKey

            If HttpContext.Current.Session(strKey) IsNot Nothing Then
                HttpContext.Current.Session.Remove(strKey)
            End If
        Catch
        End Try
    End Sub

    Public Shared Sub DeleteKeyFromApplication(ByVal strKey As String)
        Try
            strKey = WLHelperWeb.CompanyToken & "_" & strKey

            If HttpContext.Current.Application(strKey) IsNot Nothing Then
                HttpContext.Current.Application.Lock()
                HttpContext.Current.Application.Remove(strKey)
                HttpContext.Current.Application.UnLock()
            End If
        Catch
        End Try
    End Sub

    Public Shared Function GetEmployeeGroupsFromSession(ByVal strFeatureAlias As String, ByVal strFeatureType As String, Optional ByVal bolReload As Boolean = False) As DataTable
        Dim tbEmployeeGroups As System.Data.DataTable = Nothing
        Try
            Dim strNameSession As String = "tbEmployeeGroupsLive_" & strFeatureAlias & "_" & strFeatureType
            If HttpContext.Current.Session(strNameSession) Is Nothing OrElse bolReload Then
                tbEmployeeGroups = API.EmployeeGroupsServiceMethods.GetGroups(Nothing, strFeatureAlias, strFeatureType)
                If tbEmployeeGroups IsNot Nothing Then
                    HttpContext.Current.Session.Remove(strNameSession)
                    HttpContext.Current.Session.Add(strNameSession, tbEmployeeGroups)
                End If
            Else
                tbEmployeeGroups = HttpContext.Current.Session(strNameSession)
            End If
        Catch
        End Try
        Return tbEmployeeGroups
    End Function

    Public Shared Function GetEmployeeGroupsFromApplication(ByVal strFeatureAlias As String, ByVal strFeatureType As String, ByVal ResetGroups As Boolean) As DataTable
        Dim tbEmployeeGroups As System.Data.DataTable = Nothing
        Try

            tbEmployeeGroups = GetEmployeeGroupsFromSession(strFeatureAlias, strFeatureType, ResetGroups)

            'Dim strNameSession As String = WLHelperWeb.CompanyToken & "_tbEmployeeGroupsLive_" & strFeatureAlias & "_" & strFeatureType
            'If HttpContext.Current.Application(strNameSession) Is Nothing Or ResetGroups Then
            '    tbEmployeeGroups = API.EmployeeGroupsServiceMethods.GetGroups(Nothing, strFeatureAlias, strFeatureType)
            '    HttpContext.Current.Application.Lock()
            '    HttpContext.Current.Application.Remove(strNameSession)
            '    If tbEmployeeGroups IsNot Nothing Then
            '        HttpContext.Current.Application.Add(strNameSession, tbEmployeeGroups)
            '    End If
            '    HttpContext.Current.Application.UnLock()
            'Else
            '    tbEmployeeGroups = HttpContext.Current.Application(strNameSession)
            'End If
        Catch
        End Try
        Return tbEmployeeGroups
    End Function

    Public Shared Sub DeleteEmployeeGroupsFromSession()
        Try
            If HttpContext.Current.Session.Count > 0 Then
                Dim strKey As String
                Dim lstKeys As New List(Of String)
                For n As Integer = 0 To HttpContext.Current.Session.Count - 1
                    strKey = HttpContext.Current.Session.Keys(n)
                    If strKey.StartsWith("tbEmployeeGroupsLive_") Then
                        lstKeys.Add(strKey)
                    End If
                Next
                For Each item As String In lstKeys
                    HttpContext.Current.Session.Remove(item)
                Next
            End If
        Catch
        End Try
    End Sub

    Public Shared Sub DeleteEmployeeGroupsFromApplication()
        Try
            DeleteEmployeeGroupsFromSession()

            'If HttpContext.Current.Application.Count > 0 Then
            '    Dim strKey As String
            '    Dim lstKeys As New List(Of String)
            '    For n As Integer = 0 To HttpContext.Current.Application.Count - 1
            '        strKey = HttpContext.Current.Application.Keys(n)
            '        If strKey.StartsWith("tbEmployeeGroupsLive_") Then
            '            lstKeys.Add(strKey)
            '        End If
            '    Next
            '    For Each item As String In lstKeys
            '        HttpContext.Current.Application.Remove(item)
            '    Next
            'End If
        Catch
        End Try
    End Sub

    Public Shared Function GetXmlFeedData() As XmlDocument

        Try

            If HttpContext.Current.Application("XmlFeedCacheData") Is Nothing OrElse CType(HttpContext.Current.Application("XmlFeedCacheData"), XmlCacheSessionData).ValidUntil < DateTime.Now Then

                Dim oWebrequest As New WebClient()
                oWebrequest.Headers.Add("User-Agent: Other")
                Dim oRequestURI As New Uri(HelperSession.AdvancedParametersCache("RoboticsFeedUrl"))
                Dim xmlContent As Byte() = oWebrequest.DownloadData(oRequestURI)

                'Dim MyRssRequest As WebRequest = WebRequest.Create(HelperSession.AdvancedParametersCache("RoboticsFeedUrl"))
                'Dim MyRssResponse As WebResponse = MyRssRequest.GetResponse()
                'Dim MyRssStream As Stream = MyRssResponse.GetResponseStream()

                Dim oCache As New XmlCacheSessionData
                Try
                    oCache.Xml = New XmlDocument()
                    'oCache.Xml.Load(MyRssStream)
                    oCache.Xml.LoadXml(System.Text.Encoding.UTF8.GetString(xmlContent))
                Catch ex As Exception
                    oCache.Xml = Nothing
                End Try

                HttpContext.Current.Application.Lock()
                HttpContext.Current.Application("XmlFeedCacheData") = oCache
                HttpContext.Current.Application.UnLock()

                Return oCache.Xml
            Else
                Dim oCache As XmlCacheSessionData = CType(HttpContext.Current.Application("XmlFeedCacheData"), XmlCacheSessionData)
                Return oCache.Xml
            End If
        Catch ex As Exception
            Dim oCache As New XmlCacheSessionData
            HttpContext.Current.Application("XmlFeedCacheData") = oCache

            Return oCache.Xml
        End Try

    End Function

    Public Shared ReadOnly Property AdvancedParametersCache(ByVal key As String, Optional ByVal bolReload As Boolean = False) As String
        Get
            Dim strResult As String = String.Empty

            If WLHelperWeb.CompanyToken = String.Empty Then
                Return strResult
            End If

            Try

                If bolReload OrElse key.ToUpper = "RESETCACHE" Then
                    DataLayer.roCacheManager.GetInstance().PurgueCompanyAdvParametersCache(WLHelperWeb.CompanyToken)
                End If

                Dim tmpHash As AdvancedParametersData = DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(WLHelperWeb.CompanyToken)

                Dim cacheKey As String = key.Split("#")(0).ToLower
                Dim bAddKey As Boolean = False
                Select Case cacheKey
                    Case "resetcache"
                        strResult = "OK"
                    Case "checklicenselimits"
                        strResult = roTypes.Any2String(API.PortalServiceMethods.CheckLicenseLimits(Nothing, DateTime.Now, WLHelperWeb.ServerLicense))
                    Case "activejobemployeescount"
                        strResult = API.PortalServiceMethods.GetActiveJobEmployeesCount(Nothing, DateTime.ParseExact(key.Split("#")(1), "yyyyMMdd", Nothing))
                    Case "activeemployeescount"
                        strResult = API.PortalServiceMethods.GetActiveEmployeesCount(Nothing, DateTime.ParseExact(key.Split("#")(1), "yyyyMMdd", Nothing))
                    Case "vtlive.defaultreportsversions"
                        strResult = "2"
                    Case "broadcastermanually"
                        If tmpHash.AdvancedParameters.ContainsKey(cacheKey) Then
                            strResult = tmpHash.AdvancedParameters(cacheKey)
                        Else
                            bAddKey = True
                            strResult = WLHelperWeb.GetAdvancedParameterValue("Process\Broadcaster", "BroadcasterManually")
                        End If

                    Case "showaccessmonitor"
                        If tmpHash.AdvancedParameters.ContainsKey(cacheKey) Then
                            strResult = tmpHash.AdvancedParameters(cacheKey)
                        Else
                            bAddKey = True
                            strResult = WLHelperWeb.GetAdvancedParameterValue("Browser\Access", "ShowAccessMonitor")
                        End If
                    Case "vtlive.edition"
                        If tmpHash.AdvancedParameters.ContainsKey(cacheKey) Then
                            strResult = tmpHash.AdvancedParameters(cacheKey)
                        Else
                            Dim oLicSupport As New Extensions.roLicenseSupport()
                            Dim oLicInfo As Extensions.roVTLicense = oLicSupport.GetVTLicenseInfo()
                            If oLicInfo.Edition <> Extensions.roServerLicense.roVisualTimeEdition.NotSet Then
                                strResult = oLicInfo.Edition.ToString.ToLower
                            End If

                            bAddKey = True
                        End If

                    Case Else
                        If tmpHash.AdvancedParameters.ContainsKey(cacheKey) Then
                            strResult = tmpHash.AdvancedParameters(cacheKey)
                        Else
                            strResult = API.CommonServiceMethods.GetAdvancedParameter(Nothing, key, False).Value
                            bAddKey = True
                        End If
                End Select

                If bAddKey Then
                    tmpHash.AdvancedParameters.Add(cacheKey, strResult)
                    DataLayer.roCacheManager.GetInstance().UpdateAdvParametersCache(WLHelperWeb.CompanyToken, tmpHash)
                End If
            Catch
                strResult = String.Empty
            End Try

            Return strResult
        End Get
    End Property

End Class