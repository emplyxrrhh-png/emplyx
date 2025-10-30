Imports System.Data.SqlTypes
Imports Robotics.Base.DTOs
Imports ServiceApi

Public Class roLocalCache
    Inherits roBaseCache


#Region "Companies"

    Public Overrides Function UpdateLocaleCache(ByVal oLocale As roAzureLocale) As Boolean
        Dim hashSessions As Hashtable = Locales

        If hashSessions.ContainsKey(oLocale.id) Then
            hashSessions(oLocale.id) = oLocale
        Else
            hashSessions.Add(oLocale.id, oLocale)
        End If

        Locales = hashSessions

        Return True
    End Function

    Public Overrides Function GetLocales() As roAzureLocale()
        Dim hashSessions As Hashtable = Locales

        Dim olst As New List(Of roAzureLocale)
        For Each localeKey As Integer In hashSessions.Keys
            olst.Add(hashSessions(localeKey))
        Next

        Return olst.ToArray

    End Function

    Public Overrides Function GetLocaleById(id As Integer) As roAzureLocale
        If Locales.ContainsKey(id) Then
            Return Locales(id)
        Else
            Return Nothing
        End If

    End Function

    Public Overrides Function GetLocaleByKey(key As String) As roAzureLocale
        Dim hashSessions As Hashtable = Locales

        For Each localeKey As Integer In hashSessions.Keys
            If CType(hashSessions(localeKey), roAzureLocale).key = key Then
                Return CType(hashSessions(localeKey), roAzureLocale)
            End If
        Next

        Return Nothing

    End Function

    Public Overrides Function ClearLocalesCache() As Boolean
        Locales = New Hashtable

        Return True
    End Function

#End Region

#Region "Parameters"

    Public Overrides Function UpdateParametersCache(strCompanyName As String, params As ParametersData) As Boolean
        Dim hashSessions As Hashtable = Parameters

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = params
        Else
            hashSessions.Add(strCompanyName, params)
        End If

        Parameters = hashSessions

        Return True
    End Function

    Public Overrides Function GetParametersCache(strCompanyName As String) As ParametersData

        If Parameters.ContainsKey(strCompanyName) Then
            Return Parameters(strCompanyName)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function PurgueCompanyParametersCache(strCompany As String) As Boolean
        If Parameters.ContainsKey(strCompany) Then
            Parameters.Remove(strCompany)
        End If

        Return True
    End Function

    Public Overrides Function PurgueParametersCache() As Boolean
        Return PurgueCompanyParametersCache("local")
    End Function

#End Region

#Region "Advanced Parameters"

    Public Overrides Function UpdateAdvParametersCache(strCompanyName As String, parameters As AdvancedParametersData) As Boolean
        Dim hashSessions As Hashtable = AdvancedParameters

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = parameters
        Else
            hashSessions.Add(strCompanyName, parameters)
        End If

        AdvancedParameters = hashSessions

        Return True

    End Function

    Public Overrides Function GetAdvParametersCache(strCompanyName As String) As AdvancedParametersData

        If AdvancedParameters.ContainsKey(strCompanyName) Then
            Return AdvancedParameters(strCompanyName)
        Else
            Return Nothing
        End If

    End Function

    Public Overrides Function PurgueCompanyAdvParametersCache(strCompany As String) As Boolean
        If AdvancedParameters.ContainsKey(strCompany) Then
            AdvancedParameters.Remove(strCompany)
        End If

        Return True
    End Function

    Public Overrides Function PurgueAdvParametersCache() As Boolean
        Return PurgueCompanyAdvParametersCache("local")
    End Function

#End Region

#Region "Companies"

    Public Overrides Function UpdateCompanyCache(ByVal oCompany As roCompanyConfiguration) As Boolean
        Dim hashSessions As Hashtable = Companies

        If hashSessions.ContainsKey(oCompany.companyname) Then
            hashSessions(oCompany.companyname) = oCompany
        Else
            hashSessions.Add(oCompany.companyname, oCompany)
        End If

        Companies = hashSessions

        Return True
    End Function

    Public Overrides Function GetCompanies() As roCompanyConfiguration()
        Dim hashSessions As Hashtable = Companies

        Dim olst As New List(Of roCompanyConfiguration)
        For Each strCompany As String In hashSessions.Keys
            olst.Add(hashSessions(strCompany))
        Next

        Return olst.ToArray

    End Function

    Public Overrides Function GetCompany(strCompany As String) As roCompanyConfiguration
        If Companies.ContainsKey(strCompany) Then
            Return Companies(strCompany)
        Else
            Return Nothing
        End If

    End Function

    Public Overrides Function ClearCompaniesCache() As Boolean
        Companies = New Hashtable

        Return True
    End Function

#End Region

#Region "VTServiceApi"

    Public Overrides Function GetCompanyInfo(sGUID As String) As roCompanyInfo

        If VTServiceApi.ContainsKey(sGUID) Then
            Return VTServiceApi(sGUID)
        Else
            Return Nothing
        End If

    End Function

    Public Overrides Function UpdateCompanyInfoCache(sGUID As String, oCompanyInfo As roCompanyInfo) As Boolean
        Dim hashSessions As Hashtable = VTServiceApi

        If hashSessions.ContainsKey(sGUID) Then
            hashSessions(sGUID) = oCompanyInfo
        Else
            hashSessions.Add(sGUID, oCompanyInfo)
        End If

        VTServiceApi = hashSessions

        Return True
    End Function

    Public Overrides Function GetCompanyGUID(sAzureCompanyName As String) As String
        If VTCompanyGUID.ContainsKey(sAzureCompanyName) Then
            Return VTCompanyGUID(sAzureCompanyName)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function UpdateCompanyTokenCache(sAzureCompanyName As String, sToken As String) As Boolean
        Dim hashSessions As Hashtable = VTCompanyGUID

        If hashSessions.ContainsKey(sAzureCompanyName) Then
            hashSessions(sAzureCompanyName) = sToken
        Else
            hashSessions.Add(sAzureCompanyName, sToken)
        End If

        VTCompanyGUID = hashSessions

        Return True
    End Function

    Public Overrides Function ClearServiceApiCache() As Boolean
        VTServiceApi = New Hashtable
        VTCompanyGUID = New Hashtable

        Return True
    End Function

#End Region

#Region "Terminals"

    Public Overrides Function UpdateTerminalCache(ByVal oTerminal As roTerminalRegister) As Boolean
        Dim hashSessions As Hashtable = Terminals

        If hashSessions.ContainsKey(oTerminal.Id) Then
            hashSessions(oTerminal.Id) = oTerminal
        Else
            hashSessions.Add(oTerminal.Id, oTerminal)
        End If

        Terminals = hashSessions

        Return True
    End Function

    Public Overrides Function GetTerminals() As roTerminalRegister()
        Dim hashSessions As Hashtable = Terminals

        Dim olst As New List(Of roTerminalRegister)
        For Each strCompany As String In hashSessions.Keys
            olst.Add(hashSessions(strCompany))
        Next

        Return olst.ToArray
    End Function

    Public Overrides Function GetTerminal(strSN As String) As roTerminalRegister
        If Terminals.ContainsKey(strSN) Then
            Return Terminals(strSN)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function ClearTerminalsCache() As Boolean
        Terminals = New Hashtable

        Return True
    End Function

#End Region

#Region "Causes"

    Public Overrides Function UpdateCauseCache(strCompanyName As String, oCause As roCauseEngine) As Boolean
        Dim hashSessions As Hashtable = Causes

        Dim causeCache As Hashtable = Nothing

        If hashSessions.ContainsKey(strCompanyName) Then
            causeCache = hashSessions(strCompanyName)
        Else
            causeCache = New Hashtable
        End If

        If causeCache.ContainsKey(oCause.ID) Then
            causeCache(oCause.ID) = oCause
        Else
            causeCache.Add(oCause.ID, oCause)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = causeCache
        Else
            hashSessions.Add(strCompanyName, causeCache)
        End If

        Causes = hashSessions

        Return True
    End Function

    Public Overrides Function GetCausesCache(strCompanyName As String) As Hashtable
        Dim causeCache As Hashtable = Nothing

        Dim hashSessions As Hashtable = Causes

        If hashSessions.ContainsKey(strCompanyName) Then
            causeCache = hashSessions(strCompanyName)
        Else
            causeCache = New Hashtable
        End If

        Return causeCache
    End Function

    Public Overrides Function GetCauseCache(strCompanyName As String, idCause As Integer) As roCauseEngine
        Dim causeCache As Hashtable
        Dim hashSessions As Hashtable = Causes

        If hashSessions.ContainsKey(strCompanyName) Then
            causeCache = hashSessions(strCompanyName)
        Else
            causeCache = New Hashtable
        End If

        If causeCache.ContainsKey(idCause) Then
            Return causeCache(idCause)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function RemoveCauseFromCache(strCompanyName As String, idCause As Integer) As Boolean
        Dim causeCache As Hashtable
        Dim hashSessions As Hashtable = Causes

        If hashSessions.ContainsKey(strCompanyName) Then
            causeCache = hashSessions(strCompanyName)
        Else
            causeCache = New Hashtable
        End If

        If causeCache.ContainsKey(idCause) Then
            causeCache.Remove(idCause)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = causeCache
        End If
        Causes = hashSessions

        Return True
    End Function

    Public Overrides Function PurgueCompanyCausesCache(strCompany As String) As Boolean
        Dim hashSessions As Hashtable = Causes

        If hashSessions.ContainsKey(strCompany) Then
            hashSessions.Remove(strCompany)
        End If

        Causes = hashSessions
        Return True
    End Function

    Public Overrides Function PurgueCausesCache() As Boolean
        Causes = New Hashtable
        Return True
    End Function

#End Region

#Region "Shifts"

    Public Overrides Function UpdateShiftCache(strCompanyName As String, oShift As roShiftEngine) As Boolean
        Dim hashSessions As Hashtable = Shifts

        Dim shiftCache As Hashtable = Nothing

        If hashSessions.ContainsKey(strCompanyName) Then
            shiftCache = hashSessions(strCompanyName)
        Else
            shiftCache = New Hashtable
        End If

        If shiftCache.ContainsKey(oShift.ID) Then
            shiftCache(oShift.ID) = oShift
        Else
            shiftCache.Add(oShift.ID, oShift)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = shiftCache
        Else
            hashSessions.Add(strCompanyName, shiftCache)
        End If

        Shifts = hashSessions

        Return True
    End Function

    Public Overrides Function GetShiftsCache(strCompanyName As String) As Hashtable
        Dim shiftCache As Hashtable = Nothing

        Dim hashSessions As Hashtable = Shifts


        If hashSessions.ContainsKey(strCompanyName) Then
            shiftCache = hashSessions(strCompanyName)
        Else
            shiftCache = New Hashtable
        End If

        Return shiftCache
    End Function

    Public Overrides Function GetShiftCache(strCompanyName As String, idShift As Integer) As roShiftEngine
        Dim shiftCache As Hashtable
        Dim hashSessions As Hashtable = Shifts

        If hashSessions.ContainsKey(strCompanyName) Then
            shiftCache = hashSessions(strCompanyName)
        Else
            shiftCache = New Hashtable
        End If

        If shiftCache.ContainsKey(idShift) Then
            Return shiftCache(idShift)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function RemoveShiftFromCache(strCompanyName As String, idShift As Integer) As Boolean
        Dim shiftCache As Hashtable
        Dim hashSessions As Hashtable = Shifts

        If hashSessions.ContainsKey(strCompanyName) Then
            shiftCache = hashSessions(strCompanyName)
        Else
            shiftCache = New Hashtable
        End If

        If shiftCache.ContainsKey(idShift) Then
            shiftCache.Remove(idShift)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = shiftCache
        End If
        Shifts = hashSessions

        Return True
    End Function

    Public Overrides Function PurgueCompanyShiftsCache(strCompany As String) As Boolean
        Dim hashSessions As Hashtable = Shifts

        If hashSessions.ContainsKey(strCompany) Then
            hashSessions.Remove(strCompany)
        End If

        Shifts = hashSessions
        Return True
    End Function

    Public Overrides Function PurgueShiftsCache() As Boolean
        Shifts = New Hashtable
        Return True
    End Function

#End Region

#Region "LabAgrees"

    Public Overrides Function UpdateLabAgreeCache(strCompanyName As String, oLabAgree As roLabAgreeEngine) As Boolean
        Dim hashSessions As Hashtable = LabAgrees

        Dim labAgreeCache As Hashtable = Nothing

        If hashSessions.ContainsKey(strCompanyName) Then
            labAgreeCache = hashSessions(strCompanyName)
        Else
            labAgreeCache = New Hashtable
        End If

        If labAgreeCache.ContainsKey(oLabAgree.ID) Then
            labAgreeCache(oLabAgree.ID) = oLabAgree
        Else
            labAgreeCache.Add(oLabAgree.ID, oLabAgree)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = labAgreeCache
        Else
            hashSessions.Add(strCompanyName, labAgreeCache)
        End If

        LabAgrees = hashSessions

        Return True
    End Function

    Public Overrides Function GetLabAgreesCache(strCompanyName As String) As Hashtable
        Dim labAgreeCache As Hashtable = Nothing

        Dim hashSessions As Hashtable = LabAgrees


        If hashSessions.ContainsKey(strCompanyName) Then
            labAgreeCache = hashSessions(strCompanyName)
        Else
            labAgreeCache = New Hashtable
        End If

        Return labAgreeCache
    End Function

    Public Overrides Function GetLabAgreeCache(strCompanyName As String, idLabAgree As Integer) As roLabAgreeEngine
        Dim labAgreeCache As Hashtable
        Dim hashSessions As Hashtable = LabAgrees

        If hashSessions.ContainsKey(strCompanyName) Then
            labAgreeCache = hashSessions(strCompanyName)
        Else
            labAgreeCache = New Hashtable
        End If

        If labAgreeCache.ContainsKey(idLabAgree) Then
            Return labAgreeCache(idLabAgree)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function RemoveLabAgreeFromCache(strCompanyName As String, idLabAgree As Integer) As Boolean
        Dim labAgreeCache As Hashtable
        Dim hashSessions As Hashtable = LabAgrees

        If hashSessions.ContainsKey(strCompanyName) Then
            labAgreeCache = hashSessions(strCompanyName)
        Else
            labAgreeCache = New Hashtable
        End If

        If labAgreeCache.ContainsKey(idLabAgree) Then
            labAgreeCache.Remove(idLabAgree)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = labAgreeCache
        End If
        LabAgrees = hashSessions

        Return True
    End Function

    Public Overrides Function PurgueCompanyLabAgreesCache(strCompany As String) As Boolean
        Dim hashSessions As Hashtable = LabAgrees

        If hashSessions.ContainsKey(strCompany) Then
            hashSessions.Remove(strCompany)
        End If

        LabAgrees = hashSessions
        Return True
    End Function

    Public Overrides Function PurgueLabAgreesCache() As Boolean
        LabAgrees = New Hashtable
        Return True
    End Function

#End Region

#Region "Concepts"

    Public Overrides Function UpdateConceptCache(strCompanyName As String, oConcept As roConceptEngine) As Boolean
        Dim hashSessions As Hashtable = Concepts

        Dim conceptCache As Hashtable = Nothing

        If hashSessions.ContainsKey(strCompanyName) Then
            conceptCache = hashSessions(strCompanyName)
        Else
            conceptCache = New Hashtable
        End If

        If conceptCache.ContainsKey(oConcept.ID) Then
            conceptCache(oConcept.ID) = oConcept
        Else
            conceptCache.Add(oConcept.ID, oConcept)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = conceptCache
        Else
            hashSessions.Add(strCompanyName, conceptCache)
        End If

        Concepts = hashSessions

        Return True
    End Function

    Public Overrides Function GetConceptsCache(strCompanyName As String) As Hashtable
        Dim conceptCache As Hashtable

        Dim hashSessions As Hashtable = Concepts

        If hashSessions.ContainsKey(strCompanyName) Then
            conceptCache = hashSessions(strCompanyName)
        Else
            conceptCache = New Hashtable
        End If

        Return conceptCache
    End Function

    Public Overrides Function GetConceptCache(strCompanyName As String, idConcept As Integer) As roConceptEngine
        Dim conceptCache As Hashtable
        Dim hashSessions As Hashtable = Concepts

        If hashSessions.ContainsKey(strCompanyName) Then
            conceptCache = hashSessions(strCompanyName)
        Else
            conceptCache = New Hashtable
        End If

        If conceptCache.ContainsKey(idConcept) Then
            Return conceptCache(idConcept)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function RemoveConceptFromCache(strCompanyName As String, idConcept As Integer) As Boolean
        Dim conceptCache As Hashtable
        Dim hashSessions As Hashtable = Concepts

        If hashSessions.ContainsKey(strCompanyName) Then
            conceptCache = hashSessions(strCompanyName)
        Else
            conceptCache = New Hashtable
        End If

        If conceptCache.ContainsKey(idConcept) Then
            conceptCache.Remove(idConcept)
        End If

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = conceptCache
        End If
        Concepts = hashSessions

        Return True
    End Function

    Public Overrides Function PurgueCompanyConceptsCache(strCompany As String) As Boolean
        Dim hashSessions As Hashtable = Concepts

        If hashSessions.ContainsKey(strCompany) Then
            hashSessions.Remove(strCompany)
        End If

        Concepts = hashSessions
        Return True
    End Function

    Public Overrides Function PurgueConceptsCache() As Boolean
        Concepts = New Hashtable
        Return True
    End Function

#End Region

#Region "VTLiveApi sessions"

    Public Overrides Function GetVTLiveApiSession(strApiSessionKey As String) As roApiSession
        If LiveApiSessions.ContainsKey(strApiSessionKey) Then
            Return LiveApiSessions(strApiSessionKey)
        Else
            Return Nothing
        End If
    End Function

    Public Overrides Function UpdateVTLiveApiSession(oApiSession As roApiSession) As Boolean
        Dim hashSessions As Hashtable = LiveApiSessions

        If hashSessions.ContainsKey(oApiSession.SessionKey) Then
            hashSessions(oApiSession.SessionKey) = oApiSession
        Else
            hashSessions.Add(oApiSession.SessionKey, oApiSession)
        End If

        LiveApiSessions = hashSessions

        Return True
    End Function

    Public Overrides Function PurgueVTLiveApiSessions() As roApiSession()
        Dim bRet As New Generic.List(Of roApiSession)

        For Each oSessionKey As String In LiveApiSessions.Keys
            Dim oSession As roApiSession = LiveApiSessions(oSessionKey)

            Dim timeoutTime As Integer = 30
            If oSession.ApplicationSource = roAppSource.mx9 Then
                timeoutTime = 5
            End If

            If Now.Subtract(oSession.LastRequest).TotalMinutes > (timeoutTime + 5) Then
                If (oSession.ApplicationSource = roAppSource.mx9) AndAlso oSession.IdTerminal > 0 Then
                    bRet.Add(oSession)
                End If
            End If
        Next

        For Each oKeyToRemove As roApiSession In bRet
            DeleteVTLiveApiSession(oKeyToRemove.SessionKey)
        Next

        Return bRet.ToArray
    End Function

    Public Overrides Function DeleteVTLiveApiSession(strApiSessionKey As String) As Boolean
        If LiveApiSessions.ContainsKey(strApiSessionKey) Then
            Dim hashSessions As Hashtable = LiveApiSessions
            hashSessions.Remove(strApiSessionKey)
            LiveApiSessions = hashSessions

            Return True
        Else
            Return False
        End If

    End Function

#End Region

#Region "Reboot cache"

    Public Overrides Function RebootCompanyCache(strCompany As String) As Boolean

        Companies.Remove(strCompany)
        Parameters.Remove(strCompany)
        AdvancedParameters.Remove(strCompany)

        Causes.Remove(strCompany)
        LabAgrees.Remove(strCompany)
        Concepts.Remove(strCompany)
        Shifts.Remove(strCompany)

        Terminals.Remove(strCompany)
        LiveApiSessions.Remove(strCompany)
        CustomLanguageConf.Remove(strCompany)

        Return True
    End Function

    Public Overrides Function RebootCompanyParametersCache(strCompany As String) As Boolean

        Parameters.Remove(strCompany)
        AdvancedParameters.Remove(strCompany)

        Return True
    End Function

    Public Overrides Function RebootCache() As Boolean

        Parameters = New Hashtable
        AdvancedParameters = New Hashtable
        Companies = New Hashtable

        Causes = New Hashtable
        LabAgrees = New Hashtable
        Concepts = New Hashtable
        Shifts = New Hashtable

        Terminals = New Hashtable
        LiveApiSessions = New Hashtable
        CustomLanguageConf = New Hashtable

        VTServiceApi = New Hashtable
        VTCompanyGUID = New Hashtable
        Return True
    End Function

#End Region

#Region "Notifications"

    Public Overrides Function UpdateCustomLanguageCache(strCompanyName As String, oNotification As Dictionary(Of String, Byte())) As Boolean
        Dim hashSessions As Hashtable = CustomLanguageConf

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions(strCompanyName) = oNotification
        Else
            hashSessions.Add(strCompanyName, oNotification)
        End If

        CustomLanguageConf = hashSessions

        Return True
    End Function

    Public Overrides Function GetCustomLanguage(strCompanyName As String) As Dictionary(Of String, Byte())
        Dim hashSessions As Hashtable = CustomLanguageConf

        If hashSessions.ContainsKey(strCompanyName) Then
            Return CustomLanguageConf(strCompanyName)
        Else
            Return Nothing
        End If

    End Function

    Public Overrides Function PurgueCustomLanguageCache() As Boolean
        CustomLanguageConf = New Hashtable
        Return True
    End Function

    Public Overrides Function PurgeCompanyCustomLanguage(strCompanyName As String) As Boolean
        Dim hashSessions As Hashtable = CustomLanguageConf

        If hashSessions.ContainsKey(strCompanyName) Then
            hashSessions.Remove(strCompanyName)
        End If

        CustomLanguageConf = hashSessions

        Return True
    End Function

#End Region

    Public Overrides Function CheckUpdateCache() As Boolean
        Return False
    End Function

End Class