Imports Robotics.Base.DTOs
Imports ServiceApi

Public Interface iRoboticsCache

#Region "Update cache"

    Function CheckUpdateCache() As Boolean

#End Region

#Region "Locales"

    Function UpdateLocaleCache(oLocale As roAzureLocale) As Boolean

    Function GetLocales() As roAzureLocale()

    Function GetLocaleById(id As Integer) As roAzureLocale

    Function GetLocaleByKey(key As String) As roAzureLocale

    Function ClearLocalesCache() As Boolean
#End Region

#Region "Parameters"

    Function UpdateParametersCache(strCompanyName As String, parameters As ParametersData) As Boolean

    Function GetParametersCache(strCompanyName As String) As ParametersData

    Function PurgueCompanyParametersCache(strCompany As String) As Boolean

    Function PurgueParametersCache() As Boolean

#End Region

#Region "VTLiveAdvancedParameters"

    Function UpdateAdvParametersCache(ByVal strCompanyName As String, ByVal parameters As AdvancedParametersData) As Boolean

    Function GetAdvParametersCache(ByVal strCompanyName As String) As AdvancedParametersData

    Function PurgueCompanyAdvParametersCache(ByVal strCompany As String) As Boolean

    Function PurgueAdvParametersCache() As Boolean

#End Region

#Region "LabAgree"

    Function UpdateLabAgreeCache(strCompanyName As String, oLabAgree As roLabAgreeEngine) As Boolean

    Function GetLabAgreesCache(strCompanyName As String) As Hashtable

    Function GetLabAgreeCache(strCompanyName As String, idLabAgree As Integer) As roLabAgreeEngine

    Function RemoveLabAgreeFromCache(strCompanyName As String, idLabAgree As Integer) As Boolean

    Function PurgueCompanyLabAgreesCache(strCompany As String) As Boolean

    Function PurgueLabAgreesCache() As Boolean

#End Region

#Region "Causes"

    Function UpdateCauseCache(strCompanyName As String, oCause As roCauseEngine) As Boolean

    Function GetCausesCache(strCompanyName As String) As Hashtable

    Function GetCauseCache(strCompanyName As String, idCause As Integer) As roCauseEngine

    Function RemoveCauseFromCache(strCompanyName As String, idCause As Integer) As Boolean

    Function PurgueCompanyCausesCache(strCompany As String) As Boolean

    Function PurgueCausesCache() As Boolean

#End Region

#Region "Shifts"

    Function UpdateShiftCache(strCompanyName As String, oShift As roShiftEngine) As Boolean

    Function GetShiftsCache(strCompanyName As String) As Hashtable

    Function GetShiftCache(strCompanyName As String, idShift As Integer) As roShiftEngine

    Function RemoveShiftFromCache(strCompanyName As String, idShift As Integer) As Boolean

    Function PurgueCompanyShiftsCache(strCompany As String) As Boolean

    Function PurgueShiftsCache() As Boolean

#End Region

#Region "Concepts"

    Function UpdateConceptCache(strCompanyName As String, oConcept As roConceptEngine) As Boolean

    Function GetConceptsCache(strCompanyName As String) As Hashtable

    Function GetConceptCache(strCompanyName As String, idConcept As Integer) As roConceptEngine

    Function RemoveConceptFromCache(strCompanyName As String, idConcept As Integer) As Boolean

    Function PurgueCompanyConceptsCache(strCompany As String) As Boolean

    Function PurgueConceptsCache() As Boolean

#End Region

#Region "Companies"

    Function UpdateCompanyCache(ByVal oCompany As roCompanyConfiguration) As Boolean

    Function GetCompanies() As roCompanyConfiguration()

    Function GetCompany(ByVal strCompany As String) As roCompanyConfiguration

    Function ClearCompaniesCache() As Boolean

#End Region

#Region "VTServiceApi"

    Function GetCompanyInfo(sGUID As String) As roCompanyInfo

    Function UpdateCompanyInfoCache(sGUID As String, oCompanyInfo As roCompanyInfo) As Boolean

    Function GetCompanyGUID(sAzureCompanyName As String) As String

    Function UpdateCompanyTokenCache(sAzureCompanyName As String, sToken As String) As Boolean

    Function ClearServiceApiCache() As Boolean

#End Region

#Region "Terminals"

    Function UpdateTerminalCache(ByVal oTerminal As roTerminalRegister) As Boolean

    Function GetTerminals() As roTerminalRegister()

    Function GetTerminal(ByVal strSN As String) As roTerminalRegister

    Function ClearTerminalsCache() As Boolean

#End Region

#Region "VTLiveApi sessions"

    Function GetVTLiveApiSession(ByVal strApiSessionKey As String) As roApiSession

    Function DeleteVTLiveApiSession(ByVal strApiSessionKey As String) As Boolean

    Function UpdateVTLiveApiSession(ByVal oApiSession As roApiSession) As Boolean

    Function PurgueVTLiveApiSessions() As roApiSession()

#End Region

#Region "Notifications"

    Function UpdateCustomLanguageCache(strCompanyName As String, ByVal oNotification As Dictionary(Of String, Byte())) As Boolean

    Function GetCustomLanguage(strCompanyName As String) As Dictionary(Of String, Byte())

    Function PurgueCustomLanguageCache() As Boolean

    Function PurgeCompanyCustomLanguage(strCompanyName As String) As Boolean

#End Region

#Region "RebootCache"

    Function RebootCompanyCache(ByVal strCompany As String) As Boolean

    Function RebootCompanyParametersCache(ByVal strCompany As String) As Boolean

    Function RebootCache() As Boolean

#End Region

#Region "Database connections"

    Function UpdateConnectionOnId(ByVal iThreadId As String, ByVal oConnection As Object) As Boolean

    Function GetConnectionOnId(ByVal iThreadId As String) As Object

    Function RemoveConnectionOnId(iThreadId As String) As Boolean

#End Region

End Interface