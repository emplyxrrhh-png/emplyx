Imports System.Runtime.Caching
Imports Robotics.Base.DTOs
Imports ServiceApi

Public MustInherit Class roBaseCache
    Implements iRoboticsCache

    Private Shared memoryCache As MemoryCache = MemoryCache.Default

    Protected _instance As roBaseCache = Nothing

    Protected _identifier As Guid

    'Private _advParametersCache As AdvancedParametersData = Nothing

    Public Sub New()
        _identifier = Guid.NewGuid()
    End Sub


#Region "Locales"

    Protected Shared Property Locales As Hashtable
        Get
            Dim cacheObject = memoryCache("oLocalesConfiguration")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oLocalesConfiguration", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateLocaleCache(oLocale As roAzureLocale) As Boolean Implements iRoboticsCache.UpdateLocaleCache

    Public MustOverride Function GetLocales() As roAzureLocale() Implements iRoboticsCache.GetLocales

    Public MustOverride Function GetLocaleById(id As Integer) As roAzureLocale Implements iRoboticsCache.GetLocaleById

    Public MustOverride Function GetLocaleByKey(key As String) As roAzureLocale Implements iRoboticsCache.GetLocaleByKey

    Public MustOverride Function ClearLocalesCache() As Boolean Implements iRoboticsCache.ClearLocalesCache
#End Region

#Region "LabAgrees"

    Protected Shared Property LabAgrees As Hashtable
        Get
            Dim cacheObject = memoryCache("oLabAgreeCache")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oLabAgreeCache", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateLabAgreeCache(strCompanyName As String, oLabAgree As roLabAgreeEngine) As Boolean Implements iRoboticsCache.UpdateLabAgreeCache

    Public MustOverride Function GetLabAgreesCache(strCompanyName As String) As Hashtable Implements iRoboticsCache.GetLabAgreesCache

    Public MustOverride Function GetLabAgreeCache(strCompanyName As String, idLabAgree As Integer) As roLabAgreeEngine Implements iRoboticsCache.GetLabAgreeCache

    Public MustOverride Function RemoveLabAgreeFromCache(strCompanyName As String, idLabAgree As Integer) As Boolean Implements iRoboticsCache.RemoveLabAgreeFromCache

    Public MustOverride Function PurgueCompanyLabAgreesCache(strCompany As String) As Boolean Implements iRoboticsCache.PurgueCompanyLabAgreesCache

    Public MustOverride Function PurgueLabAgreesCache() As Boolean Implements iRoboticsCache.PurgueLabAgreesCache

#End Region

#Region "Causes"

    Protected Shared Property Causes As Hashtable
        Get
            Dim cacheObject = memoryCache("oCauseCache")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oCauseCache", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateCauseCache(strCompanyName As String, oCause As roCauseEngine) As Boolean Implements iRoboticsCache.UpdateCauseCache

    Public MustOverride Function GetCausesCache(strCompanyName As String) As Hashtable Implements iRoboticsCache.GetCausesCache

    Public MustOverride Function GetCauseCache(strCompanyName As String, idCause As Integer) As roCauseEngine Implements iRoboticsCache.GetCauseCache

    Public MustOverride Function RemoveCauseFromCache(strCompanyName As String, idCause As Integer) As Boolean Implements iRoboticsCache.RemoveCauseFromCache

    Public MustOverride Function PurgueCompanyCausesCache(strCompany As String) As Boolean Implements iRoboticsCache.PurgueCompanyCausesCache

    Public MustOverride Function PurgueCausesCache() As Boolean Implements iRoboticsCache.PurgueCausesCache

#End Region

#Region "Shifts"

    Protected Shared Property Shifts As Hashtable
        Get
            Dim cacheObject = memoryCache("oShiftCache")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oShiftCache", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateShiftCache(strCompanyName As String, oShift As roShiftEngine) As Boolean Implements iRoboticsCache.UpdateShiftCache

    Public MustOverride Function GetShiftsCache(strCompanyName As String) As Hashtable Implements iRoboticsCache.GetShiftsCache

    Public MustOverride Function GetShiftCache(strCompanyName As String, idShift As Integer) As roShiftEngine Implements iRoboticsCache.GetShiftCache

    Public MustOverride Function RemoveShiftFromCache(strCompanyName As String, idShift As Integer) As Boolean Implements iRoboticsCache.RemoveShiftFromCache

    Public MustOverride Function PurgueCompanyShiftsCache(strCompany As String) As Boolean Implements iRoboticsCache.PurgueCompanyShiftsCache

    Public MustOverride Function PurgueShiftsCache() As Boolean Implements iRoboticsCache.PurgueShiftsCache

#End Region

#Region "Parameters"

    Protected Shared Property Parameters As Hashtable
        Get
            Dim cacheObject = memoryCache("oParameters")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oParameters", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateParametersCache(strCompanyName As String, parameters As ParametersData) As Boolean Implements iRoboticsCache.UpdateParametersCache

    Public MustOverride Function GetParametersCache(strCompanyName As String) As ParametersData Implements iRoboticsCache.GetParametersCache

    Public MustOverride Function PurgueCompanyParametersCache(strCompany As String) As Boolean Implements iRoboticsCache.PurgueCompanyParametersCache

    Public MustOverride Function PurgueParametersCache() As Boolean Implements iRoboticsCache.PurgueParametersCache

#End Region

#Region "Advanced Parameters"

    Protected Shared Property AdvancedParameters As Hashtable
        Get
            Dim cacheObject = memoryCache("advancedParametersData")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If

            Return cacheObject
        End Get
        Set(value As Hashtable)
            memoryCache.Set("advancedParametersData", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateAdvParametersCache(strCompanyName As String, parameters As AdvancedParametersData) As Boolean Implements iRoboticsCache.UpdateAdvParametersCache

    Public MustOverride Function GetAdvParametersCache(strCompanyName As String) As AdvancedParametersData Implements iRoboticsCache.GetAdvParametersCache

    Public MustOverride Function PurgueCompanyAdvParametersCache(strCompany As String) As Boolean Implements iRoboticsCache.PurgueCompanyAdvParametersCache

    Public MustOverride Function PurgueAdvParametersCache() As Boolean Implements iRoboticsCache.PurgueAdvParametersCache

#End Region

#Region "Companies"

    Protected Shared Property Companies As Hashtable
        Get
            Dim cacheObject = memoryCache("oCompanyConfiguration")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oCompanyConfiguration", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateCompanyCache(oCompany As roCompanyConfiguration) As Boolean Implements iRoboticsCache.UpdateCompanyCache

    Public MustOverride Function GetCompanies() As roCompanyConfiguration() Implements iRoboticsCache.GetCompanies

    Public MustOverride Function GetCompany(strCompany As String) As roCompanyConfiguration Implements iRoboticsCache.GetCompany

    Public MustOverride Function ClearCompaniesCache() As Boolean Implements iRoboticsCache.ClearCompaniesCache

#End Region

#Region "VTServiceApi"

    Protected Shared Property VTServiceApi As Hashtable
        Get
            Dim cacheObject = memoryCache("oVTServiceApi")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oVTServiceApi", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Protected Shared Property VTCompanyGUID As Hashtable
        Get
            Dim cacheObject = memoryCache("oVTServiceApiGUID")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oVTServiceApiGUID", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function GetCompanyInfo(sGUID As String) As roCompanyInfo Implements iRoboticsCache.GetCompanyInfo

    Public MustOverride Function UpdateCompanyInfoCache(sGUID As String, oCompanyInfo As roCompanyInfo) As Boolean Implements iRoboticsCache.UpdateCompanyInfoCache

    Public MustOverride Function GetCompanyGUID(sAzureCompanyName As String) As String Implements iRoboticsCache.GetCompanyGUID

    Public MustOverride Function UpdateCompanyTokenCache(sAzureCompanyName As String, sToken As String) As Boolean Implements iRoboticsCache.UpdateCompanyTokenCache

    Public MustOverride Function ClearServiceApiCache() As Boolean Implements iRoboticsCache.ClearServiceApiCache

#End Region

#Region "VTLiveApi"

    Protected Shared Property LiveApiSessions As Hashtable
        Get
            Dim cacheObject = memoryCache("oLiveApiSessions")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oLiveApiSessions", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function GetVTLiveApiSession(strApiSessionKey As String) As roApiSession Implements iRoboticsCache.GetVTLiveApiSession

    Public MustOverride Function DeleteVTLiveApiSession(strApiSessionKey As String) As Boolean Implements iRoboticsCache.DeleteVTLiveApiSession

    Public MustOverride Function UpdateVTLiveApiSession(oApiSession As roApiSession) As Boolean Implements iRoboticsCache.UpdateVTLiveApiSession

    Public MustOverride Function PurgueVTLiveApiSessions() As roApiSession() Implements iRoboticsCache.PurgueVTLiveApiSessions

#End Region

#Region "Notifications Definition"

    Protected Shared Property CustomLanguageConf As Hashtable
        Get
            Dim cacheObject = memoryCache("oNotificationSendItem")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oNotificationSendItem", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateCustomLanguageCache(strCompanyName As String, oNotification As Dictionary(Of String, Byte())) As Boolean Implements iRoboticsCache.UpdateCustomLanguageCache

    Public MustOverride Function GetCustomLanguage(strCompanyName As String) As Dictionary(Of String, Byte()) Implements iRoboticsCache.GetCustomLanguage

    Public MustOverride Function PurgueCustomLanguageCache() As Boolean Implements iRoboticsCache.PurgueCustomLanguageCache

    Public MustOverride Function PurgeCompanyCustomLanguage(strCompanyName As String) As Boolean Implements iRoboticsCache.PurgeCompanyCustomLanguage

#End Region

#Region "Terminals"

    Protected Shared Property Terminals As Hashtable
        Get
            Dim cacheObject = memoryCache("oTerminalIdentifiers")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oTerminalIdentifiers", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateTerminalCache(ByVal oTerminal As roTerminalRegister) As Boolean Implements iRoboticsCache.UpdateTerminalCache

    Public MustOverride Function GetTerminals() As roTerminalRegister() Implements iRoboticsCache.GetTerminals

    Public MustOverride Function GetTerminal(strSN As String) As roTerminalRegister Implements iRoboticsCache.GetTerminal

    Public MustOverride Function ClearTerminalsCache() As Boolean Implements iRoboticsCache.ClearTerminalsCache

#End Region

#Region "Concepts"

    Protected Shared Property Concepts As Hashtable
        Get
            Dim cacheObject = memoryCache("oConceptCache")

            If (cacheObject Is Nothing) Then
                Return New Hashtable()
            Else
                Return cacheObject
            End If
        End Get
        Set(value As Hashtable)
            memoryCache.Set("oConceptCache", value, DateTimeOffset.Now.AddHours(25))
        End Set
    End Property

    Public MustOverride Function UpdateConceptCache(strCompanyName As String, oConcept As roConceptEngine) As Boolean Implements iRoboticsCache.UpdateConceptCache

    Public MustOverride Function GetConceptsCache(strCompanyName As String) As Hashtable Implements iRoboticsCache.GetConceptsCache

    Public MustOverride Function GetConceptCache(strCompanyName As String, idConcept As Integer) As roConceptEngine Implements iRoboticsCache.GetConceptCache

    Public MustOverride Function RemoveConceptFromCache(strCompanyName As String, idConcept As Integer) As Boolean Implements iRoboticsCache.RemoveConceptFromCache

    Public MustOverride Function PurgueCompanyConceptsCache(strCompany As String) As Boolean Implements iRoboticsCache.PurgueCompanyConceptsCache

    Public MustOverride Function PurgueConceptsCache() As Boolean Implements iRoboticsCache.PurgueConceptsCache

#End Region

    Public MustOverride Function RebootCompanyCache(strCompany As String) As Boolean Implements iRoboticsCache.RebootCompanyCache

    Public MustOverride Function RebootCompanyParametersCache(strCompany As String) As Boolean Implements iRoboticsCache.RebootCompanyParametersCache

    Public MustOverride Function RebootCache() As Boolean Implements iRoboticsCache.RebootCache

    Public MustOverride Function CheckUpdateCache() As Boolean Implements iRoboticsCache.CheckUpdateCache

#Region "Database connections"

    Public Function UpdateConnectionOnId(iThreadId As String, oConnection As Object) As Boolean Implements iRoboticsCache.UpdateConnectionOnId

        memoryCache.Set("oDBConnections_" & iThreadId, oConnection, DateTimeOffset.Now.AddDays(1))

        Return True
    End Function

    Public Function GetConnectionOnId(iThreadId As String) As Object Implements iRoboticsCache.GetConnectionOnId
        If memoryCache.Contains("oDBConnections_" & iThreadId) Then
            Return memoryCache.Get("oDBConnections_" & iThreadId)
        Else
            Return Nothing
        End If
    End Function

    Public Function RemoveConnectionOnId(iThreadId As String) As Boolean Implements iRoboticsCache.RemoveConnectionOnId
        If memoryCache.Contains("oDBConnections_" & iThreadId) Then
            Try
                memoryCache.Remove("oDBConnections_" & iThreadId)
                Return True
            Catch ex As Exception
                Return False
            End Try
        End If

        Return True
    End Function

#End Region

End Class