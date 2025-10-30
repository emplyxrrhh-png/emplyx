Imports System.Data.SqlTypes
Imports Robotics.Azure
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.roConstants
Imports ServiceApi

Public Class roCacheManager

    Protected Shared _instance As roCacheManager

    Private _oCacheBase As roBaseCache = Nothing

    Private _GlobalCacheRebootDate As DateTime = DateTime.MinValue
    Private _LastGlobalCacheCheckDate As DateTime = DateTime.MinValue


    Private _CompanyCacheRebootDate As New Hashtable
    Private _LastCompanyCacheCheckDate As New Hashtable

    Private _CompanyParametersCacheRebootDate As New Hashtable
    Private _LastCompanyParametersCacheCheckDate As New Hashtable

    Public ReadOnly Property GlobalCacheRebootDate As DateTime
        Get
            Return _GlobalCacheRebootDate
        End Get
    End Property

    Public ReadOnly Property LastGlobalCacheCheckDate As DateTime
        Get
            Return _LastGlobalCacheCheckDate
        End Get
    End Property

    Private _engineCausesDataLastUpdate As DateTime = DateTime.MinValue
    Private _engineConceptsDataLastUpdate As DateTime = DateTime.MinValue
    Private _engineLabAgreesDataLastUpdate As DateTime = DateTime.MinValue
    Private _engineShiftsDataLastUpdate As DateTime = DateTime.MinValue
    Private _terminalsLastUpdate As DateTime = DateTime.MinValue

    Public Shared ReadOnly Property RedisEnabled As Boolean
        Get
            Return False '(VTBase.roConstants.IsDistributedSystemEnabled)
        End Get
    End Property

    Public Shared ReadOnly Property GetInstance() As roCacheManager
        Get
            If _instance Is Nothing Then
                _instance = New roCacheManager
            End If

            If DateTime.Now.Subtract(_instance.LastGlobalCacheCheckDate).TotalSeconds > 120 Then
                _instance.UpdateLastCacheCheck()

                If roRedisManger.GetGlobalDatetimeResetCache() > _instance.GlobalCacheRebootDate Then
                    _instance.UpdateLastCacheDate()
                    _instance.RebootCache()
                    roServiceConfigurationRepository.RebootCache()
                    roConfigRepository.RebootCache()
                    roAzureKeyvault.RebootCache()
                End If
            End If

            Return _instance
        End Get
    End Property

    Public Sub New()
        If _oCacheBase Is Nothing Then
            _oCacheBase = New roLocalCache
            _engineCausesDataLastUpdate = DateTime.Now
            _engineConceptsDataLastUpdate = DateTime.Now
            _engineLabAgreesDataLastUpdate = DateTime.Now
            _engineShiftsDataLastUpdate = DateTime.Now
            _terminalsLastUpdate = DateTime.Now

            _GlobalCacheRebootDate = DateTime.Now
            _LastGlobalCacheCheckDate = DateTime.Now
        End If
    End Sub

#Region "Parameters"

    Public Function UpdateParametersCache(strCompanyName As String, parameters As ParametersData) As Boolean
        Return _oCacheBase.UpdateParametersCache(strCompanyName, parameters)
    End Function

    Public Function GetParametersCache(strCompanyName As String, ByVal key As Robotics.Base.DTOs.Parameters) As Object
        Dim parameterCache As ParametersData = GetParametersCache(strCompanyName)

        If parameterCache IsNot Nothing AndAlso parameterCache.Parameters.ContainsKey(key.ToString) Then
            Return parameterCache.Parameters(key.ToString)
        Else
            Return String.Empty
        End If
    End Function

    Public Function GetParametersCache(strCompanyName As String) As ParametersData

        Dim parameterCache As ParametersData = _oCacheBase.GetParametersCache(strCompanyName)

        Try
            If parameterCache Is Nothing OrElse parameterCache.ValidUntil < DateTime.Now Then

                Try
                    Dim strSQL As String = "@SELECT# Data FROM sysroParameters WHERE [ID] = 'OPTIONS'"
                    Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                        Dim oTmpCollection As New roCollection(roTypes.Any2String(tb.Rows(0).Item("Data")).Trim)

                        parameterCache = New ParametersData()
                        For index As Integer = 1 To oTmpCollection.Count
                            parameterCache.Parameters.Add(oTmpCollection.Key(index), oTmpCollection(oTmpCollection.Key(index)))
                        Next
                        DataLayer.roCacheManager.GetInstance().UpdateParametersCache(strCompanyName, parameterCache)

                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roCacheManager::GetParametersCache::Could not load parameters table", ex)
                End Try

            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roCacheManager::GetParametersCache::Could not retrieve parameters from cache", ex)
        End Try

        Return parameterCache
    End Function

    Public Function PurgueCompanyParametersCache(strCompany As String) As Boolean
        Return _oCacheBase.PurgueCompanyParametersCache(strCompany)
    End Function

    Public Function PurgueParametersCache() As Boolean
        Return _oCacheBase.PurgueParametersCache()
    End Function

#End Region

#Region "Advanced Parameters"

    Public Function UpdateAdvParametersCache(strCompanyName As String, parameters As AdvancedParametersData) As Boolean
        Return _oCacheBase.UpdateAdvParametersCache(strCompanyName, parameters)
    End Function

    Public Function GetAdvParametersCache(strCompanyName As String) As AdvancedParametersData

        Dim parameterCache As AdvancedParametersData = _oCacheBase.GetAdvParametersCache(strCompanyName)

        Try
            If parameterCache Is Nothing OrElse (parameterCache IsNot Nothing AndAlso parameterCache.ValidUntil < DateTime.Now) Then

                Try
                    Dim strSQL As String = "@SELECT# * FROM sysroLiveAdvancedParameters ORDER BY ParameterName ASC"

                    Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                    If tb IsNot Nothing Then
                        Dim oAdvParameter As AdvancedParameterCache = Nothing
                        parameterCache = New AdvancedParametersData()
                        For Each oRow As DataRow In tb.Rows
                            If Not parameterCache.AdvancedParameters.ContainsKey(roTypes.Any2String(oRow("ParameterName")).Trim.ToLower) Then
                                parameterCache.AdvancedParameters.Add(roTypes.Any2String(oRow("ParameterName")).Trim.ToLower, roTypes.Any2String(oRow("Value")).Trim)
                            End If
                        Next
                        DataLayer.roCacheManager.GetInstance().UpdateAdvParametersCache(strCompanyName, parameterCache)
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roCacheManager::GetAdvParametersCache::Could not load advanced parameters table", ex)
                End Try

            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roCacheManager::GetAdvParametersCache::Could not retrieve advanced parameters from cache", ex)
        End Try

        Return parameterCache
    End Function

    Public Function GetAdvParametersCache(strCompanyName As String, key As String) As String
        Dim parameterCache As AdvancedParametersData = GetAdvParametersCache(strCompanyName)

        If parameterCache IsNot Nothing AndAlso parameterCache.AdvancedParameters.ContainsKey(key.ToLower) Then
            Return parameterCache.AdvancedParameters(key.ToLower)
        Else
            Return String.Empty
        End If
    End Function

    Public Function PurgueCompanyAdvParametersCache(strCompany As String) As Boolean
        Return _oCacheBase.PurgueCompanyAdvParametersCache(strCompany)
    End Function

    Public Function PurgueAdvParametersCache() As Boolean
        Return _oCacheBase.PurgueAdvParametersCache()
    End Function

#End Region

#Region "Companies"

    Public Function GetOwinCompanies() As roCompanyConfiguration()
        Dim oCompanies As roCompanyConfiguration()

        Dim oManager As New roCompanyConfigurationRepository()
        oCompanies = oManager.GetCompanies()

        For Each oCompanyConf As roCompanyConfiguration In oCompanies
            Robotics.DataLayer.roCacheManager.GetInstance.UpdateCompanyCache(oCompanyConf)
        Next

        Return oCompanies
    End Function

    Public Function UpdateCompanyCache(ByVal oCompany As roCompanyConfiguration) As Boolean
        Return _oCacheBase.UpdateCompanyCache(oCompany)
    End Function

    Public Function IsCompanyCacheEmpty() As Boolean
        Dim oCompany As roCompanyConfiguration() = _oCacheBase.GetCompanies()

        If oCompany Is Nothing OrElse oCompany.Length = 0 Then
            Return True
        End If

        Return False
    End Function

    Public Function GetCompanies() As roCompanyConfiguration()
        Dim oCompanies As roCompanyConfiguration()

        oCompanies = _oCacheBase.GetCompanies()

        If oCompanies Is Nothing OrElse oCompanies.Length = 0 Then
            Dim oManager As New roCompanyConfigurationRepository()
            oCompanies = oManager.GetCompanies()

            For Each oCompanyConf As roCompanyConfiguration In oCompanies
                Robotics.DataLayer.roCacheManager.GetInstance.UpdateCompanyCache(oCompanyConf)
            Next
        End If

        Return oCompanies
    End Function

    Public Function GetCompany(strCompany As String) As roCompanyConfiguration
        Dim oCompany As roCompanyConfiguration = _oCacheBase.GetCompany(strCompany)

        If oCompany Is Nothing Then
            Dim oCompanyConfiguration As New roCompanyConfigurationRepository()
            oCompany = oCompanyConfiguration.GetCompanyConfiguration(strCompany)

            If oCompany IsNot Nothing AndAlso oCompany.companyname <> String.Empty Then
                UpdateCompanyCache(oCompany)
            End If
        End If

        Return oCompany
    End Function

    Public Function CompanyIsInCache(strCompany As String) As Boolean
        Dim oCompany As roCompanyConfiguration = _oCacheBase.GetCompany(strCompany)

        If oCompany Is Nothing Then
            Return False
        End If

        Return True
    End Function

    Public Function ClearCompaniesCache() As Boolean
        Return _oCacheBase.ClearCompaniesCache()
    End Function

#End Region


#Region "Locales"

    Public Function GetLocales() As roAzureLocale()
        Dim oLocales As roAzureLocale()

        oLocales = _oCacheBase.GetLocales()

        If oLocales Is Nothing OrElse oLocales.Length = 0 Then
            Dim oManager As New roAzureLocaleRepository()
            oLocales = oManager.GetLocales()

            For Each oLocale As roAzureLocale In oLocales
                UpdateLocaleCache(oLocale)
            Next
        End If

        Return oLocales
    End Function

    Public Function GetLocaleByKey(key As String) As roAzureLocale
        Dim oLocale As roAzureLocale = _oCacheBase.GetLocaleByKey(key)

        If oLocale Is Nothing Then
            Dim oManager As New roAzureLocaleRepository()
            oLocale = oManager.GetLocaleByKey(key)

            If oLocale IsNot Nothing AndAlso oLocale.key = key Then
                UpdateLocaleCache(oLocale)
            End If
        End If

        Return oLocale
    End Function

    Public Function GetLocaleById(id As Integer) As roAzureLocale
        Dim oLocale As roAzureLocale = _oCacheBase.GetLocaleById(id)

        If oLocale Is Nothing Then
            Dim oManager As New roAzureLocaleRepository()
            oLocale = oManager.GetLocaleById(id)

            If oLocale IsNot Nothing AndAlso oLocale.id = id Then
                UpdateLocaleCache(oLocale)
            End If
        End If

        Return oLocale
    End Function

    Public Function UpdateLocaleCache(ByVal oLocale As roAzureLocale) As Boolean
        Return _oCacheBase.UpdateLocaleCache(oLocale)
    End Function

    Public Function ClearLocalesCache() As Boolean
        Return _oCacheBase.ClearLocalesCache()
    End Function

#End Region


#Region "VTServiceApi"

    Public Function GetCompanyInfo(sGUID As Object) As roCompanyInfo

        Dim oCompanyInfo As roCompanyInfo = _oCacheBase.GetCompanyInfo(sGUID)

        If oCompanyInfo Is Nothing Then
            Dim oApi As New VTServiceApi.roServiceApiManager()
            oCompanyInfo = oApi.GetCompanyInfo(sGUID)

            If oCompanyInfo IsNot Nothing AndAlso oCompanyInfo.code <> String.Empty Then
                _oCacheBase.UpdateCompanyInfoCache(sGUID, oCompanyInfo)
            End If
        End If

        Return oCompanyInfo
    End Function

    Public Function GetCompanyGUID(sAzureCompanyName As Object) As String
        Dim sGUID As String = _oCacheBase.GetCompanyGUID(sAzureCompanyName)

        If sGUID Is Nothing Then
            Dim oApi As New VTServiceApi.roServiceApiManager()
            sGUID = oApi.GetCompanyToken(sAzureCompanyName)

            If sGUID IsNot Nothing AndAlso sGUID <> String.Empty Then
                _oCacheBase.UpdateCompanyTokenCache(sAzureCompanyName, sGUID)
            End If
        End If

        Return sGUID

    End Function

#End Region

#Region "Terminals"

    Public Function UpdateTerminalCache(ByVal oTerminal As roTerminalRegister) As Boolean
        Return _oCacheBase.UpdateTerminalCache(oTerminal)
    End Function

    Public Function GetTerminals() As roTerminalRegister()
        Dim oTerminals As roTerminalRegister() = _oCacheBase.GetTerminals()

        If oTerminals Is Nothing OrElse oTerminals.Length = 0 Then
            Dim oTerminalRepo As New roAzureTerminalRepository()
            Dim cacheTerminals As roTerminalRegister() = oTerminalRepo.GetTerminals()

            For Each oTerminal As roTerminalRegister In cacheTerminals
                Robotics.DataLayer.roCacheManager.GetInstance.UpdateTerminalCache(oTerminal)
            Next
        End If

        Return oTerminals
    End Function

    Public Function GetTerminal(strSN As String) As roTerminalRegister
        Dim oTerminal As roTerminalRegister = _oCacheBase.GetTerminal(strSN)

        If Now.Subtract(_terminalsLastUpdate).TotalMinutes > 1440 Then
            _terminalsLastUpdate = DateTime.Now
            _oCacheBase.ClearTerminalsCache()
        End If

        If oTerminal Is Nothing Then
            Dim oTerminalRepo As New roAzureTerminalRepository()
            oTerminal = oTerminalRepo.GetTerminalCompanyConfiguration(strSN)

            Robotics.DataLayer.roCacheManager.GetInstance.UpdateTerminalCache(oTerminal)
        End If

        Return oTerminal
    End Function

    Public Function ClearTerminalsCache() As Boolean
        Return _oCacheBase.ClearTerminalsCache()
    End Function

#End Region

#Region "Causes"

    Public Function UpdateCauseCache(strCompanyName As String, oCause As roCauseEngine) As Boolean
        Return _oCacheBase.UpdateCauseCache(strCompanyName, oCause)
    End Function

    Public Function GetCausesCache(strCompanyName As String) As Hashtable
        Return _oCacheBase.GetCausesCache(strCompanyName)
    End Function

    Public Function GetCauseCache(strCompanyName As String, idCause As Integer) As roCauseEngine
        Return _oCacheBase.GetCauseCache(strCompanyName, idCause)
    End Function

    Public Function RemoveCauseFromCache(strCompanyName As String, idCause As Integer) As Boolean
        Return _oCacheBase.RemoveCauseFromCache(strCompanyName, idCause)
    End Function

    Public Function PurgueCompanyCausesCache(strCompany As String) As Boolean
        Return _oCacheBase.PurgueCompanyCausesCache(strCompany)
    End Function

    Public Function PurgueCausesCache() As Boolean
        Return _oCacheBase.PurgueShiftsCache()
    End Function

#End Region

#Region "LabAgrees"

    Public Function UpdateLabAgreeCache(strCompanyName As String, oLabAgree As roLabAgreeEngine) As Boolean
        Return _oCacheBase.UpdateLabAgreeCache(strCompanyName, oLabAgree)
    End Function

    Public Function GetLabAgreesCache(strCompanyName As String) As Hashtable
        Return _oCacheBase.GetLabAgreesCache(strCompanyName)
    End Function

    Public Function GetLabAgreeCache(strCompanyName As String, idLabAgree As Integer) As roLabAgreeEngine
        Return _oCacheBase.GetLabAgreeCache(strCompanyName, idLabAgree)
    End Function

    Public Function RemoveLabAgreeFromCache(strCompanyName As String, idLabAgree As Integer) As Boolean
        Return _oCacheBase.RemoveLabAgreeFromCache(strCompanyName, idLabAgree)
    End Function

    Public Function PurgueCompanyLabAgreesCache(strCompany As String) As Boolean
        Return _oCacheBase.PurgueCompanyLabAgreesCache(strCompany)
    End Function

    Public Function PurgueLabAgreesCache() As Boolean
        Return _oCacheBase.PurgueLabAgreesCache()
    End Function

#End Region

#Region "Shifts"

    Public Function UpdateShiftCache(strCompanyName As String, oShift As roShiftEngine) As Boolean
        Return _oCacheBase.UpdateShiftCache(strCompanyName, oShift)
    End Function

    Public Function GetShiftsCache(strCompanyName As String) As Hashtable
        Return _oCacheBase.GetShiftsCache(strCompanyName)
    End Function

    Public Function GetShiftCache(strCompanyName As String, idShift As Integer) As roShiftEngine
        Return _oCacheBase.GetShiftCache(strCompanyName, idShift)
    End Function

    Public Function RemoveShiftFromCache(strCompanyName As String, idShift As Integer) As Boolean
        Return _oCacheBase.RemoveShiftFromCache(strCompanyName, idShift)
    End Function

    Public Function PurgueCompanyShiftsCache(strCompany As String) As Boolean
        Return _oCacheBase.PurgueCompanyShiftsCache(strCompany)
    End Function

    Public Function PurgueShiftsCache() As Boolean
        Return _oCacheBase.PurgueShiftsCache()
    End Function

#End Region

#Region "Concepts"

    Public Function UpdateConceptCache(strCompanyName As String, oConcept As roConceptEngine) As Boolean
        Return _oCacheBase.UpdateConceptCache(strCompanyName, oConcept)
    End Function

    Public Function GetConceptsCache(strCompanyName As String) As Hashtable
        Return _oCacheBase.GetConceptsCache(strCompanyName)
    End Function

    Public Function GetConceptCache(strCompanyName As String, idConcept As Integer) As roConceptEngine
        Return _oCacheBase.GetConceptCache(strCompanyName, idConcept)
    End Function

    Public Function RemoveConceptFromCache(strCompanyName As String, idConcept As Integer) As Boolean
        Return _oCacheBase.RemoveConceptFromCache(strCompanyName, idConcept)
    End Function

    Public Function PurgueCompanyConceptsCache(strCompany As String) As Boolean
        Return _oCacheBase.PurgueCompanyConceptsCache(strCompany)
    End Function

    Public Function PurgueConceptsCache() As Boolean
        Return _oCacheBase.PurgueConceptsCache()
    End Function

#End Region

#Region "VTLiveApi sessions"

    Public Function GetVTLiveApiSession(strApiSessionKey As String) As roApiSession
        Return _oCacheBase.GetVTLiveApiSession(strApiSessionKey)
    End Function

    Public Function UpdateVTLiveApiSession(oApiSession As roApiSession) As Boolean
        Return _oCacheBase.UpdateVTLiveApiSession(oApiSession)
    End Function

    Public Function PurgueVTLiveApiSessions() As roApiSession()
        Return _oCacheBase.PurgueVTLiveApiSessions()
    End Function

    Public Function DeleteVTLiveApiSession(strApiSessionKey As String) As Boolean
        Return _oCacheBase.DeleteVTLiveApiSession(strApiSessionKey)
    End Function

#End Region

#Region "Reboot cache"

    Public Function UpdateInitCache(Optional oDBConnction As roBaseConnection = Nothing) As Boolean

        Try
            Dim strSQL As String = $"MERGE INTO sysroParameters
                                        USING (VALUES ('CACHEINIT')) AS Source (ID)
                                        ON sysroParameters.ID = Source.ID
                                        WHEN MATCHED THEN
                                            @UPDATE# SET Data = '{roTypes.CreateDateTime().AddMinutes(5).ToString("yyyy/MM/dd HH:mm:ss")}'
                                        WHEN NOT MATCHED THEN
                                            @INSERT# (ID, Data)
                                            VALUES ('CACHEINIT', '{roTypes.CreateDateTime().AddMinutes(5).ToString("yyyy/MM/dd HH:mm:ss")}');"

            AccessHelper.ExecuteSql(strSQL, oDBConnction)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roCacheManager::ExecuteTask::Could not update cacheinit parameter", ex)
        End Try

    End Function

    Public Function UpdateParamCache(Optional minutesDelay As Integer = 1, Optional oDBConnction As roBaseConnection = Nothing) As Boolean

        Try
            If minutesDelay < 1 Then minutesDelay = 1
            If minutesDelay > 5 Then minutesDelay = 5

            Dim strSQL As String = $"MERGE INTO sysroParameters
                                        USING (VALUES ('PARAMINIT')) AS Source (ID)
                                        ON sysroParameters.ID = Source.ID
                                        WHEN MATCHED THEN
                                            @UPDATE# SET Data = '{roTypes.CreateDateTime().AddMinutes(minutesDelay).ToString("yyyy/MM/dd HH:mm:ss")}'
                                        WHEN NOT MATCHED THEN
                                            @INSERT# (ID, Data)
                                            VALUES ('PARAMINIT', '{roTypes.CreateDateTime().AddMinutes(minutesDelay).ToString("yyyy/MM/dd HH:mm:ss")}');"

            AccessHelper.ExecuteSql(strSQL, oDBConnction)
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "roCacheManager::ExecuteTask::Could not update paraminit parameter", ex)
        End Try

    End Function


    Public Function NeedToRefreshCompanyCache(strCompany As String) As Boolean
        Dim checkDate As DateTime = roTypes.CreateDateTime()

        Dim alreadyChecked As Boolean = False

        If _LastCompanyCacheCheckDate.ContainsKey(strCompany) AndAlso _CompanyCacheRebootDate.ContainsKey(strCompany) Then

            If checkDate.Subtract(_LastCompanyCacheCheckDate(strCompany)).TotalSeconds > 120 Then
                _LastCompanyCacheCheckDate(strCompany) = checkDate

                Dim sSQL As String = $"@SELECT# Data FROM sysroParameters where ID='CACHEINIT'"

                Dim cacheParam As Object = DataLayer.AccessHelper.ExecuteScalar(sSQL)
                If cacheParam IsNot Nothing AndAlso Not IsDBNull(cacheParam) Then
                    Dim dbDate As DateTime = roTypes.Any2Time(cacheParam).ValueDateTime
                    If dbDate > _CompanyCacheRebootDate(strCompany) Then
                        _CompanyCacheRebootDate(strCompany) = dbDate
                        RebootCompanyCache(strCompany)
                        'Forzamos a recargar la configuración de la compañia para que no se pierda de las listas globales
                        GetCompany(strCompany)
                        alreadyChecked = True
                    End If
                End If
            End If

        Else
            If Not _LastCompanyCacheCheckDate.ContainsKey(strCompany) Then _LastCompanyCacheCheckDate.Add(strCompany, checkDate)
            If Not _CompanyCacheRebootDate.ContainsKey(strCompany) Then _CompanyCacheRebootDate.Add(strCompany, checkDate)
        End If

        If Not alreadyChecked Then RebootParamCacheIfNeeded(strCompany, checkDate)

        Return True

    End Function

    Private Sub RebootParamCacheIfNeeded(strCompany As String, checkDate As Date)

        If _LastCompanyParametersCacheCheckDate.ContainsKey(strCompany) AndAlso _CompanyParametersCacheRebootDate.ContainsKey(strCompany) Then

            If checkDate.Subtract(_LastCompanyParametersCacheCheckDate(strCompany)).TotalSeconds > 120 Then
                _LastCompanyParametersCacheCheckDate(strCompany) = checkDate

                Dim sSQL As String = $"@SELECT# Data FROM sysroParameters where ID='PARAMINIT'"
                Dim cacheParam As Object = DataLayer.AccessHelper.ExecuteScalar(sSQL)
                If cacheParam IsNot Nothing AndAlso Not IsDBNull(cacheParam) Then
                    Dim dbDate As DateTime = roTypes.Any2Time(cacheParam).ValueDateTime
                    If dbDate > _CompanyParametersCacheRebootDate(strCompany) Then
                        _CompanyParametersCacheRebootDate(strCompany) = dbDate
                        RebootCompanyParametersCache(strCompany)
                    End If
                End If
            End If
        Else
            If Not _LastCompanyParametersCacheCheckDate.ContainsKey(strCompany) Then _LastCompanyParametersCacheCheckDate.Add(strCompany, checkDate)
            If Not _CompanyParametersCacheRebootDate.ContainsKey(strCompany) Then _CompanyParametersCacheRebootDate.Add(strCompany, checkDate)
        End If
    End Sub

    Public Function RebootCompanyCache(strCompany As String) As Boolean
        Return _oCacheBase.RebootCompanyCache(strCompany)
    End Function

    Public Function RebootCompanyParametersCache(strCompany As String) As Boolean
        Return _oCacheBase.RebootCompanyParametersCache(strCompany)
    End Function

    Public Function RebootCache() As Boolean
        Return _oCacheBase.RebootCache()
    End Function

    Public Function UpdateLastCacheCheck() As Boolean
        _LastGlobalCacheCheckDate = DateTime.Now
    End Function

    Public Function UpdateLastCacheDate() As Boolean
        _GlobalCacheRebootDate = DateTime.Now
    End Function

#End Region

#Region "Notifications"

    Public Function UpdateCustomLanguageCache(strCompanyName As String, oNotification As Dictionary(Of String, Byte())) As Boolean
        Return _oCacheBase.UpdateCustomLanguageCache(strCompanyName, oNotification)
    End Function

    Public Function GetCustomLanguage(strCompanyName As String) As Dictionary(Of String, Byte())
        Return _oCacheBase.GetCustomLanguage(strCompanyName)
    End Function

    Public Function PurgueCustomLanguageCache() As Boolean
        Return _oCacheBase.PurgueCustomLanguageCache()
    End Function

    Public Function PurgeCompanyCustomLanguage(strCompanyName As String) As Boolean
        Return _oCacheBase.PurgeCompanyCustomLanguage(strCompanyName)
    End Function

#End Region

#Region "Database connections"

    Public Function UpdateConnection(ByVal oCn As roBaseConnection) As Boolean
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
            If Not String.IsNullOrEmpty(sId) Then sId = $"{Threading.Thread.CurrentThread.ManagedThreadId.ToString()}_{sId}"
        End If

        If sId <> String.Empty Then
            Return _oCacheBase.UpdateConnectionOnId(sId, oCn)
        Else
            Return False
        End If

    End Function

    Public Function GetNewConnection(Optional ByVal bInitTransactionByDefault As Boolean = False, Optional ByVal bFromReadSource As Boolean = False) As roBaseConnection
        Dim oCn As roBaseConnection = roBaseConnection.ForceNewConnection(Nothing, bInitTransactionByDefault, bFromReadSource)

        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
            If Not String.IsNullOrEmpty(sId) Then sId = $"{Threading.Thread.CurrentThread.ManagedThreadId.ToString()}_{sId}"
        End If

        If sId <> String.Empty Then
            _oCacheBase.UpdateConnectionOnId(sId, oCn)
        End If

        Return oCn
    End Function

    Public Function RemoveCurrentConnection() As Boolean

        Dim oCn As roBaseConnection = roCacheManager.GetInstance().GetConnection(False)

        If oCn IsNot Nothing AndAlso oCn.IsOpen() Then
            oCn.ForceClose()
            oCn.Dispose()
            oCn = Nothing
        End If

        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
            If Not String.IsNullOrEmpty(sId) Then sId = $"{Threading.Thread.CurrentThread.ManagedThreadId.ToString()}_{sId}"
        End If

        If sId <> String.Empty Then
            Return _oCacheBase.RemoveConnectionOnId(sId)
        Else
            Return False
        End If
    End Function

    Public Function GetConnection(Optional ByVal bForceCreate As Boolean = True) As roBaseConnection
        Dim sId As String

        If Not Robotics.VTBase.roConstants.IsMultitenantServiceEnabled Then
            sId = Robotics.VTBase.roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.RequestGUID)
        Else
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
            If Not String.IsNullOrEmpty(sId) Then sId = $"{Threading.Thread.CurrentThread.ManagedThreadId.ToString()}_{sId}"
        End If

        'Sólo aplicará si debugamos en local
        If sId Is Nothing AndAlso VTBase.roTypes.Any2Boolean(VTBase.roConstants.GetConfigurationParameter("VTLive.DebugProcessLocally")) Then
            roLog.GetInstance.logMessage(roLog.EventType.roCritic, "roAzureSupport::GetCompanyName:Alarm if you read that in PROD!")
            sId = Threading.Thread.GetDomain().GetData(Threading.Thread.CurrentThread.ManagedThreadId.ToString() & "_RequestGUID")
        End If

        If sId <> String.Empty Then

            Dim oCn As roBaseConnection = _oCacheBase.GetConnectionOnId(sId)

            If oCn Is Nothing AndAlso bForceCreate Then
                oCn = roBaseConnection.ForceNewConnection(Nothing, False, False)

                If oCn.IsInitialized Then
                    _oCacheBase.UpdateConnectionOnId(sId, oCn)
                Else
                    oCn = Nothing
                End If
            End If

            Return oCn
        Else
            Return Nothing
        End If
    End Function

#End Region

    Public Function CheckUpdateCache() As Boolean
        Return _oCacheBase.CheckUpdateCache()
    End Function

    Public Function CheckEngineCacheNeedsUpdate(ByVal companyName As String) As Boolean
        Dim oRet As Boolean = False
        Try
            Dim oSQL As String = "@SELECT# Value from sysroliveAdvancedParameters where ParameterName ='Engine.Causes.LastCacheUpdate'"

            Dim oDBDate As Object = DataLayer.AccessHelper.ExecuteScalar(oSQL)

            If oDBDate IsNot Nothing AndAlso Not IsDBNull(oDBDate) Then
                Dim oCacheDate As DateTime = roTypes.Any2DateTime(oDBDate)
                If oCacheDate > Me._engineCausesDataLastUpdate Then
                    Me.PurgueCompanyCausesCache(companyName)
                    Me._engineCausesDataLastUpdate = oCacheDate
                End If
            End If

            oSQL = "@SELECT# Value from sysroliveAdvancedParameters where ParameterName ='Engine.Concepts.LastCacheUpdate'"

            oDBDate = DataLayer.AccessHelper.ExecuteScalar(oSQL)

            If oDBDate IsNot Nothing AndAlso Not IsDBNull(oDBDate) Then
                Dim oCacheDate As DateTime = roTypes.Any2DateTime(oDBDate)
                If oCacheDate > Me._engineConceptsDataLastUpdate Then
                    Me.PurgueCompanyConceptsCache(companyName)
                    Me._engineConceptsDataLastUpdate = oCacheDate
                End If
            End If

            oSQL = "@SELECT# Value from sysroliveAdvancedParameters where ParameterName ='Engine.LabAgrees.LastCacheUpdate'"

            oDBDate = DataLayer.AccessHelper.ExecuteScalar(oSQL)

            If oDBDate IsNot Nothing AndAlso Not IsDBNull(oDBDate) Then
                Dim oCacheDate As DateTime = roTypes.Any2DateTime(oDBDate)
                If oCacheDate > Me._engineLabAgreesDataLastUpdate Then
                    Me.PurgueCompanyLabAgreesCache(companyName)
                    Me._engineLabAgreesDataLastUpdate = oCacheDate
                End If
            End If

            oSQL = "@SELECT# Value from sysroliveAdvancedParameters where ParameterName ='Engine.Shifts.LastCacheUpdate'"

            oDBDate = DataLayer.AccessHelper.ExecuteScalar(oSQL)

            If oDBDate IsNot Nothing AndAlso Not IsDBNull(oDBDate) Then
                Dim oCacheDate As DateTime = roTypes.Any2DateTime(oDBDate)
                If oCacheDate > Me._engineShiftsDataLastUpdate Then
                    Me.PurgueCompanyShiftsCache(companyName)
                    Me._engineShiftsDataLastUpdate = oCacheDate
                End If
            End If
        Catch ex As Exception
            'do nothing
        End Try

        Return oRet
    End Function

    Public Sub UpdateEngineCausesCacheLastUpdate(ByVal oDate As DateTime)
        Me._engineCausesDataLastUpdate = oDate
    End Sub

    Public Sub UpdateEngineConceptsCacheLastUpdate(ByVal oDate As DateTime)
        Me._engineConceptsDataLastUpdate = oDate
    End Sub

    Public Sub UpdateEngineLabAgreesCacheLastUpdate(ByVal oDate As DateTime)
        Me._engineLabAgreesDataLastUpdate = oDate
    End Sub

    Public Sub UpdateEngineShiftsCacheLastUpdate(ByVal oDate As DateTime)
        Me._engineShiftsDataLastUpdate = oDate
    End Sub

End Class