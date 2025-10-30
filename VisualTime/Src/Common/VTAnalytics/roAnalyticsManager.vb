Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Threading
Imports Robotics.Base.Analytics.Manager
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Base.VTSelectorManager.roSelectorManager
Imports Microsoft.VisualBasic.Logging
Imports Robotics.Base.DTOs.UserFieldsTypes

Namespace VTAnalyticsManager

    <DataContract>
    Public Class roAnalyticsManager

#Region "Properties"

        Private CompanyID As String
        Private dicInvalidTypeDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicPunchDirectionDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicTypeDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicStatusDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicMethodDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicVersionDescription As Generic.Dictionary(Of String, String) = Nothing
        Private dicPassportName As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicKeywords As Generic.Dictionary(Of String, String) = Nothing
        Private dicPassportPermissions As Generic.Dictionary(Of Integer, Base.DTOs.Permission) = Nothing
        Private dicDayOfWeekDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicMonthDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Private dicUnespecifiedZoneDescription As Generic.Dictionary(Of String, String) = Nothing
        Private dicReliableDescription As Generic.Dictionary(Of Integer, String) = Nothing
        Const minRows4Paralelize As Integer = 5000

        Private translationsHash As Generic.List(Of roLayoutDescription) '= New Hashtable()
        Private cubeProcdure As String
        Private executionLanguage As roLanguage
        Private executionLanguageKey As String
#End Region

#Region "Cube Methods"

        Public Function GenerateDBCube(strDBProcedure As String, idView As Integer, iIdPassport As Integer, xBeginDate As Date, xEndDate As Date, strEmployees As String, strConcepts As String, strCostCenters As String, strUserFields As String, strRequestTypes As String, strCauses As String,
                                       bIncludeZeroValues As Boolean, ByVal bolIncludeZeroCauseValues As Boolean, bolIncludeUndefinedCenter As Boolean, strFeature As String, ByVal oLng As roLanguage, ByVal xBeginTime As Date, ByVal xEndTime As Date, ByVal geniusExecution As roGeniusExecution, Optional ByVal BIExecutionName As String = Nothing, Optional ByVal OverwriteResults As Boolean = False) As String
            Dim strFileName As String = String.Empty

            Try

                Dim DateInf As DateTime = xBeginDate.Date
                Dim DateSup As DateTime = xEndDate.Date
                Dim TimeInf As DateTime = xBeginTime
                Dim TimeSup As DateTime = xEndTime

                If DateInf > DateSup Then
                    Dim aux As DateTime = DateSup
                    DateSup = DateInf
                    DateInf = aux
                End If

                If strFeature = String.Empty Then strFeature = "Employees"

                Dim empFilter As String() = strEmployees.Split("#")

                Dim strEmpFilter As String = If(empFilter.Length > 0, empFilter(0), "")
                Dim strStatusFilter As String = If(empFilter.Length > 1, empFilter(1), "11110")
                Dim strUfFilter As String = If(empFilter.Length > 2, empFilter(2), "")


                Dim strEmployeeFilter As String = GetWhereWithoutPermissions(strEmpFilter, strStatusFilter, strUfFilter, False).ToUpper().Replace("@SELECT#", "SELECT")
                Dim strUserFieldsFilter As String = strUserFields

                If roPassportManager.IsRoboticsUserOrConsultant(iIdPassport) Then
                    Me.executionLanguage = Nothing
                    Me.executionLanguageKey = String.Empty
                Else
                    Me.executionLanguage = oLng
                    Me.executionLanguageKey = oLng.LanguageKey
                End If

                If BIExecutionName Is Nothing Then
                    strFileName = "##DBAnalytics##_" & idView & "_" & iIdPassport & "_" & Thread.CurrentThread.ManagedThreadId & "_" & Guid.NewGuid.ToString & "_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".json"
                Else
                    strFileName = BIExecutionName & ".json"
                End If

                roTrace.GetInstance().AddTraceEvent("Initializing file name" & strFileName)

                Dim arrayUserFields = {}
                If strUserFieldsFilter <> String.Empty Then arrayUserFields = strUserFieldsFilter.Split(",")

                Dim types = New List(Of UserFieldProperties)
                Dim oState As New UserFields.roUserFieldState()
                For Each userField In arrayUserFields
                    Dim type As UserFields.roUserField = New UserFields.roUserField(oState, userField, UserFieldsTypes.Types.EmployeeField, False, False)
                    types.Add(New UserFieldProperties With {
                        .Name = type.FieldName,
                        .Type = type.FieldType,
                        .RequieredFeature = IIf(type.AccessLevel = UserFieldsTypes.AccessLevels.aLow, 1530, IIf(type.AccessLevel = UserFieldsTypes.AccessLevels.aMedium, 1540, 1550))
                    })
                Next

                Dim strBusinessCenterFilter As String = strCostCenters
                If (strBusinessCenterFilter Is "") Then
                    If Not bolIncludeUndefinedCenter Then
                        strBusinessCenterFilter = "0"
                    End If
                Else
                    If bolIncludeUndefinedCenter Then
                        strBusinessCenterFilter += ",0"
                    End If
                End If

                Dim arrayConcepts = {}
                If strConcepts <> String.Empty Then arrayConcepts = strConcepts.Split(",")

                Dim oConceptsList As New Generic.List(Of Concept.roConcept)
                For Each sIdConcept In arrayConcepts
                    Dim oTmpConcept As New Concept.roConcept(roTypes.Any2Integer(sIdConcept), New Concept.roConceptState())

                    If oTmpConcept.State.Result = ConceptResultEnum.NoError Then
                        oConceptsList.Add(oTmpConcept)
                    End If
                Next

                Dim oCostCenterFields As roBusinessCenterFieldDefinition() = Nothing

                Me.cubeProcdure = strDBProcedure.Split("(")(0)
                roTrace.GetInstance().AddTraceEvent("Executing SP " & strDBProcedure)

                Using oCommand As DbCommand = DataLayer.AccessHelper.CreateCommand(Me.cubeProcdure)
                    oCommand.CommandType = CommandType.StoredProcedure
                    oCommand.CommandTimeout = 0

                    AccessHelper.AddParameter(oCommand, "@idpassport", DbType.Int32).Value = iIdPassport

                    If strDBProcedure.Contains("@initialDate") Then AccessHelper.AddParameter(oCommand, "@initialDate", DbType.DateTime).Value = DateInf
                    If strDBProcedure.Contains("@endDate") Then AccessHelper.AddParameter(oCommand, "@endDate", DbType.DateTime).Value = DateSup
                    If strDBProcedure.Contains("@initialTime") Then AccessHelper.AddParameter(oCommand, "@initialTime", DbType.DateTime).Value = TimeInf
                    If strDBProcedure.Contains("@endTime") Then AccessHelper.AddParameter(oCommand, "@endTime", DbType.DateTime).Value = TimeSup
                    If strDBProcedure.Contains("@employeeFilter") Then AccessHelper.AddParameter(oCommand, "@employeeFilter", DbType.String, strEmployeeFilter.Length).Value = strEmployeeFilter
                    If strDBProcedure.Contains("@conceptsFilter") Then AccessHelper.AddParameter(oCommand, "@conceptsFilter", DbType.String, strConcepts.Length).Value = strConcepts
                    If strDBProcedure.Contains("@userFieldsFilter") Then AccessHelper.AddParameter(oCommand, "@userFieldsFilter", DbType.String, strUserFieldsFilter.Length).Value = strUserFieldsFilter
                    If strDBProcedure.Contains("@costCenterFilter") Then AccessHelper.AddParameter(oCommand, "@costCenterFilter", DbType.String, strBusinessCenterFilter.Length).Value = strBusinessCenterFilter
                    If strDBProcedure.Contains("@requestTypesFilter") Then AccessHelper.AddParameter(oCommand, "@requestTypesFilter", DbType.String, strRequestTypes.Length).Value = strRequestTypes
                    If strDBProcedure.Contains("@causesFilter") Then AccessHelper.AddParameter(oCommand, "@causesFilter", DbType.String, strCauses.Length).Value = strCauses

                    If Me.cubeProcdure.Contains("Genius_CostCenters_Detail") Then
                        If strDBProcedure.Contains("@includeZeros") Then AccessHelper.AddParameter(oCommand, "@includeZeros", DbType.Boolean).Value = bolIncludeZeroCauseValues
                    Else
                        If strDBProcedure.Contains("@includeZeros") Then AccessHelper.AddParameter(oCommand, "@includeZeros", DbType.Boolean).Value = bIncludeZeroValues
                    End If

                    roTrace.GetInstance().AddTraceEvent("Adding parameters to SP")

                    Using da As DbDataAdapter = AccessHelper.CreateDataAdapter(oCommand)
                        Dim oDBResults As New DataTable
                        da.Fill(oDBResults)

                        roTrace.GetInstance().AddTraceEvent("SP Executed")

                        Me.translationsHash = New Generic.List(Of roLayoutDescription)
                        GenerateAzureFileForDatatable(oDBResults, iIdPassport, strFileName, BIExecutionName, OverwriteResults, arrayUserFields, types, oConceptsList, oCostCenterFields)

                        Try
                            Dim oGeniusManager As New Analytics.Manager.roGeniusViewManager(New Analytics.Manager.roAnalyticState(iIdPassport))

                            Dim layout As String = geniusExecution.CubeLayout
                            Dim jsonObject As Newtonsoft.Json.Linq.JObject = CType(Newtonsoft.Json.JsonConvert.DeserializeObject(layout), Newtonsoft.Json.Linq.JObject)

                            If BIExecutionName Is Nothing Then
                                roTrace.GetInstance().AddTraceEvent("Compressing JSON")
                                For Each oLayoutDescription As roLayoutDescription In Me.translationsHash
                                    compressJsonValues(jsonObject, "reportFilters", "uniqueName", oLayoutDescription.ColumnName, oLayoutDescription.Id)
                                    compressJsonValues(jsonObject, "rows", "uniqueName", oLayoutDescription.ColumnName, oLayoutDescription.Id)
                                    compressJsonValues(jsonObject, "columns", "uniqueName", oLayoutDescription.ColumnName, oLayoutDescription.Id)
                                    compressJsonValues(jsonObject, "measures", "uniqueName", oLayoutDescription.ColumnName, oLayoutDescription.Id)
                                    compressJsonValues(jsonObject, "measures", "formula", oLayoutDescription.ColumnName, oLayoutDescription.Id)
                                Next
                            End If

                            geniusExecution.CubeLayout = Newtonsoft.Json.JsonConvert.SerializeObject(jsonObject)
                            oGeniusManager.UpdateGeniusExecution(geniusExecution)
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roError, "CAnalyticsServerNet::GenerateDBCube:", ex)
                        End Try

                    End Using
                End Using

                GC.WaitForPendingFinalizers()
                GC.Collect()
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CAnalyticsServerNet::GenerateScheduleCube:", ex)
                strFileName = "Error"
            End Try

            'Dim oDecompressedFile As New Generic.List(Of DTOs.Analytics_Schedule_Concept)
            'oDecompressedFile.AddRange(roTypes.File2Any(Of Analytics_Schedule_Concept)(strFileName))

            Return strFileName

        End Function

        Private Sub GenerateAzureFileForDatatable(oDBResults As DataTable, iIdPassport As Integer, strFileName As String, BIExecutionName As String, OverwriteResults As Boolean, arrayUserFields() As Object, types As List(Of UserFieldProperties), oConceptsList As List(Of Concept.roConcept), ByRef oCostCenterFields() As roBusinessCenterFieldDefinition)

            Dim iPropertyIndex As Integer = 1
            Dim iGeniusIndex As Integer = 1

            For Each oProperty As DataColumn In oDBResults.Columns
                Dim columnTranslation As String = String.Empty
                iGeniusIndex = iPropertyIndex
                'If oProperty.ColumnName.Contains("(HH:MM)") Then
                Dim oConceptDef As Concept.roConcept = oConceptsList.Find(Function(x) (x.Name) = oProperty.ColumnName)
                If oConceptDef IsNot Nothing Then
                    iGeniusIndex = 99 + oConceptDef.ID
                End If
                'End If
                Dim translationKey As String = oProperty.ColumnName
                If translationKey.EndsWith("_ToHours") Then translationKey = translationKey.Replace("_ToHours", "")
                If translationKey.EndsWith("_ToDateString") Then translationKey = translationKey.Replace("_ToDateString", "")
                If translationKey.Equals("Date") AndAlso Me.cubeProcdure.Contains("Genius_Requests_Schedule") Then translationKey = "RequestDate"

                If Me.executionLanguage IsNot Nothing Then
                    columnTranslation = Me.executionLanguage.Translate($"dbColumnname.{translationKey}.rotext", "")
                    If columnTranslation.ToLowerInvariant.Contains("notfound") OrElse columnTranslation = String.Empty Then
                        columnTranslation = translationKey
                    End If

                    If oProperty.ColumnName.StartsWith("UserField") Then
                        Dim iUserField As Integer = roTypes.Any2Integer(oProperty.ColumnName.Replace("UserField", ""))
                        If iUserField <= arrayUserFields.Length Then
                            columnTranslation = arrayUserFields(iUserField - 1)
                        Else
                            columnTranslation = oProperty.ColumnName
                        End If
                    End If

                    If oProperty.ColumnName.Equals("Field1") OrElse oProperty.ColumnName.Equals("Field2") OrElse oProperty.ColumnName.Equals("Field3") _
                            OrElse oProperty.ColumnName.Equals("Field4") OrElse oProperty.ColumnName.Equals("Field5") Then
                        If oCostCenterFields Is Nothing Then oCostCenterFields = loadBusinessCentersUserFields()
                    End If

                    If oCostCenterFields IsNot Nothing Then
                        If oProperty.ColumnName.Equals("Field1") Then
                            columnTranslation = oCostCenterFields(0).Name
                        ElseIf oProperty.ColumnName.Equals("Field2") Then
                            columnTranslation = oCostCenterFields(1).Name
                        ElseIf oProperty.ColumnName.Equals("Field3") Then
                            columnTranslation = oCostCenterFields(2).Name
                        ElseIf oProperty.ColumnName.Equals("Field4") Then
                            columnTranslation = oCostCenterFields(3).Name
                        ElseIf oProperty.ColumnName.Equals("Field5") Then
                            columnTranslation = oCostCenterFields(4).Name
                        End If
                    End If
                Else
                    columnTranslation = oProperty.ColumnName
                End If

                Me.translationsHash.Add(New roLayoutDescription() With {
                        .ColumnName = oProperty.ColumnName,
                        .Id = iGeniusIndex,
                        .Caption = IIf(columnTranslation.ToLowerInvariant.Contains("notfound"), oProperty.ColumnName, columnTranslation)
                    })

                iPropertyIndex += 1
            Next

            FillTranslationDictionaries(Me.executionLanguageKey, iIdPassport)
            roTrace.GetInstance().AddTraceEvent("Translations finished")
            Dim xAzureContext As New Robotics.Azure.RoAzureSupport
            Dim queueType As roLiveQueueTypes

            If BIExecutionName Is Nothing Then
                queueType = roLiveQueueTypes.analytics
            Else
                queueType = roLiveQueueTypes.analyticsbi
                If OverwriteResults Then
                    Azure.RoAzureSupport.DeleteFileFromCompanyContainer(strFileName, "", queueType)
                End If
            End If

            If oDBResults IsNot Nothing AndAlso oDBResults.Rows.Count > minRows4Paralelize Then
                xAzureContext.Any2GeniusParalellized(oDBResults, strFileName, Me.translationsHash, types.ToArray, queueType, Azure.RoAzureSupport.GetCompanyName(), dicKeywords, dicInvalidTypeDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportPermissions, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, BIExecutionName)
            Else
                xAzureContext.Any2Genius(oDBResults, strFileName, Me.translationsHash, types.ToArray, queueType, Azure.RoAzureSupport.GetCompanyName(), dicKeywords, dicInvalidTypeDescription, dicMethodDescription, dicPassportName, dicPunchDirectionDescription, dicStatusDescription, dicTypeDescription, dicVersionDescription, dicPassportPermissions, dicDayOfWeekDescription, dicReliableDescription, dicMonthDescription, dicUnespecifiedZoneDescription, BIExecutionName)
            End If
            roTrace.GetInstance().AddTraceEvent("JSON generated")

        End Sub

        Private Function loadBusinessCentersUserFields() As roBusinessCenterFieldDefinition()
            Dim oCostCenterFields As roBusinessCenterFieldDefinition() = {
                    New roBusinessCenterFieldDefinition(New roTaskFieldState(-1), 1, False),
                    New roBusinessCenterFieldDefinition(New roTaskFieldState(-1), 2, False),
                    New roBusinessCenterFieldDefinition(New roTaskFieldState(-1), 3, False),
                    New roBusinessCenterFieldDefinition(New roTaskFieldState(-1), 4, False),
                    New roBusinessCenterFieldDefinition(New roTaskFieldState(-1), 5, False)
                    }

            Return oCostCenterFields
        End Function

        Private Sub compressJsonValues(ByRef oJson As Newtonsoft.Json.Linq.JObject, ByVal sKey As String, ByVal sJsonKey As String, ByVal sOld As String, ByVal sNew As String)
            Dim slice As Newtonsoft.Json.Linq.JToken = oJson("slice")(sKey)
            If slice Is Nothing Then Return
            For Each jsonItem As Newtonsoft.Json.Linq.JToken In slice
                Dim val As Newtonsoft.Json.Linq.JToken = jsonItem(sJsonKey)
                If val IsNot Nothing Then
                    If sJsonKey <> "formula" Then
                        If val.ToString.Split(".")(0) = sOld Then
                            jsonItem.Item(sJsonKey) = jsonItem.Item(sJsonKey).ToString.Replace(sOld, sNew)
                        End If
                    Else
                        jsonItem.Item(sJsonKey) = jsonItem.Item(sJsonKey).ToString.Replace("""" & sOld & """", """" & sNew & """")
                    End If
                End If
            Next
        End Sub

        Private Sub FillTranslationDictionaries(ByVal langkey As String, ByVal idPassport As Integer)
            If langkey <> String.Empty Then
                Dim translation = GetInvalidTypeDescription(0, langkey)
                translation = GetPunchDirectionDescription(0, langkey)
                translation = GetTypeDescription(0, langkey)
                translation = GetStatusDescription(0, langkey)
                translation = GetMethodDescription(0, langkey)
                translation = GetVersionDescription("", langkey)
                translation = GetPassportName(1, langkey)
                translation = GetDayOfWeekDescription(0, langkey)
                translation = GetMonthDescription(1, langkey)
                translation = GetUnespecifiedZoneDescription("Sin especificar", langkey)
                translation = GetReliableDescription(0, langkey)
                GetPassportPermission(idPassport) ' dicPassportPermissions
                LoadKeywords(langkey)
            Else
                GetPassportPermission(idPassport) ' dicPassportPermissions
            End If

        End Sub

        Public Function GenerateAuditCube(ByVal idView As Integer, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime, ByRef oLog As roLog) As String
            Dim strFileName As String = String.Empty

            Try
                Dim DateInf As DateTime = xBeginDate
                Dim DateSup As DateTime = xEndDate

                If DateInf > DateSup Then
                    Dim aux As DateTime = DateSup
                    DateSup = DateInf
                    DateInf = aux
                End If

                strFileName = "##AuditAnalytics##_" & idView & "_" & iIdPassport & "_" & Thread.CurrentThread.ManagedThreadId & "_" & Guid.NewGuid.ToString & "_" & DateTime.Now.ToString("yyyyMMddHHmmss") & ".json"

                Dim oPassportTicket As roPassportTicket = roPassportManager.GetPassportTicket(iIdPassport)
                Dim oAudit As List(Of Extensions.roAudit) = VTBase.Extensions.roAudit.GetAzureAudit(DateInf, DateSup)
                Dim oDBResults As DataTable = GetUserAuditCube(oAudit, oPassportTicket)




                Me.cubeProcdure = String.Empty
                Me.executionLanguage = New roLanguage()
                Me.executionLanguage.SetLanguageReference("LiveGenius", oPassportTicket.Language.Key)
                Me.executionLanguageKey = Me.executionLanguage.LanguageKey
                Me.translationsHash = New Generic.List(Of roLayoutDescription)
                GenerateAzureFileForDatatable(oDBResults, iIdPassport, strFileName, Nothing, False, {}, New List(Of UserFieldProperties)(), New List(Of Concept.roConcept)(), {})

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "CAnalyticsServerNet::GenerateTasksCube:", ex)
                strFileName = String.Empty
            End Try

            Return strFileName

        End Function

#End Region

#Region "Helpers"

        Private Function GetInvalidTypeDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicInvalidTypeDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrInvalidTypeDescription As Array = System.Enum.GetValues(GetType(InvalidTypeEnum))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrInvalidTypeDescription.Length)
                    For Each it As Integer In arrInvalidTypeDescription
                        dicAux.Add(it, oLng.Translate("InvalidTypeEnum_" & it, "analyticsscheduler"))
                    Next
                    dicInvalidTypeDescription = dicAux
                End If

                Try
                    strRet = dicInvalidTypeDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetReliableDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicReliableDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrReliableTypeDescription As Array = System.Enum.GetValues(GetType(TypeReliable))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrReliableTypeDescription.Length)
                    For Each it As Integer In arrReliableTypeDescription
                        dicAux.Add(it, oLng.Translate("ReliableTypeEnum_" & it, "analyticsscheduler"))
                    Next
                    dicReliableDescription = dicAux
                End If

                Try
                    strRet = dicReliableDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetPunchDirectionDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicPunchDirectionDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrInvalidTypeDescription As Array = System.Enum.GetValues(GetType(PunchTypeEnum))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrInvalidTypeDescription.Length)
                    For Each it As Integer In arrInvalidTypeDescription
                        dicAux.Add(it, oLng.Translate("PunchTypeEnum_" & it, "analyticsscheduler"))
                    Next
                    dicPunchDirectionDescription = dicAux
                End If

                Try
                    strRet = dicPunchDirectionDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetTypeDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicTypeDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrTypeDescription As Array = System.Enum.GetValues(GetType(eRequestType))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrTypeDescription.Length)
                    For Each it As Integer In arrTypeDescription
                        dicAux.Add(it, oLng.Translate("RequestTypeEnum_" & it, "analyticsscheduler"))
                    Next
                    dicTypeDescription = dicAux

                End If

                Try
                    strRet = dicTypeDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetStatusDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicStatusDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrTypeDescription As Array = System.Enum.GetValues(GetType(eRequestStatus))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrTypeDescription.Length)
                    For Each it As Integer In arrTypeDescription
                        dicAux.Add(it, oLng.Translate("RequestStatusEnum_" & it, "analyticsscheduler"))
                    Next
                    dicStatusDescription = dicAux
                End If

                Try
                    strRet = dicStatusDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function NoCenterText(ByVal Value As String, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty Then Return "(Not assigned)"
            Try
                If dicKeywords Is Nothing Then
                    LoadKeywords(langKey)
                End If

                If Value = String.Empty Then
                    Try
                        strRet = dicKeywords("NoCenter")
                    Catch ex As Exception
                        strRet = ""
                    End Try
                Else
                    strRet = Value
                End If
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Sub LoadKeywords(langKey As String)
            If dicKeywords Is Nothing Then
                dicKeywords = New Dictionary(Of String, String)

                Dim oLng As New roLanguage()
                oLng.SetLanguageReference("LiveGenius", langKey)

                dicKeywords.Add("NoCenter", oLng.Translate("noCenter", "dbkeywords"))
            End If
        End Sub

        Private Function GetMethodDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicMethodDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrTypeDescription As Array = System.Enum.GetValues(GetType(AuthenticationMethod))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrTypeDescription.Length)
                    For Each it As Integer In arrTypeDescription
                        dicAux.Add(it, oLng.Translate("MethodEnum_" & it, "analyticsscheduler"))
                    Next
                    dicMethodDescription = dicAux
                Else
                    If dicMethodDescription(Index) = String.Empty Then
                        Dim oLng As New roLanguage()
                        oLng.SetLanguageReference("LiveScheduler", langKey)

                        dicMethodDescription(Index) = oLng.Translate("MethodEnum_" & Index, "analyticsscheduler")
                    End If
                End If

                Try
                    strRet = dicMethodDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetDayOfWeekDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicDayOfWeekDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrTypeDescription As Array = System.Enum.GetValues(GetType(DayOfWeek))
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrTypeDescription.Length)
                    For Each it As Integer In arrTypeDescription
                        dicAux.Add(it + 1, it & " - " & oLng.Translate("DayOfWeekTypeEnum_" & it + 1, "analyticsscheduler"))
                    Next
                    dicDayOfWeekDescription = dicAux
                Else
                    If dicDayOfWeekDescription(Index) = String.Empty Then
                        Dim oLng As New roLanguage()
                        oLng.SetLanguageReference("LiveScheduler", langKey)

                        dicDayOfWeekDescription(Index) = oLng.Translate("DayOfWeekTypeEnum_" & Index, "analyticsscheduler")
                    End If
                End If

                Try
                    strRet = dicDayOfWeekDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetMonthDescription(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicMonthDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim arrTypeDescription As Array = {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11}
                    Dim dicAux As New Generic.Dictionary(Of Integer, String)(arrTypeDescription.Length)
                    For Each it As Integer In arrTypeDescription
                        dicAux.Add(it + 1, oLng.Translate("MonthTypeEnum_" & it + 1, "analyticsscheduler"))
                    Next
                    dicMonthDescription = dicAux
                Else
                    If dicMonthDescription(Index) = String.Empty Then
                        Dim oLng As New roLanguage()
                        oLng.SetLanguageReference("LiveScheduler", langKey)

                        dicMonthDescription(Index) = oLng.Translate("MonthTypeEnum_" & Index, "analyticsscheduler")
                    End If
                End If

                Try
                    strRet = dicMonthDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetUnespecifiedZoneDescription(ByVal Index As String, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = "" Then Return Index
            Try
                If dicUnespecifiedZoneDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("ZonesService", langKey)

                    Dim dicAux As New Generic.Dictionary(Of String, String)(1)
                    dicAux.Add("Sin especificar", oLng.Translate("WorldZone", "Zones"))
                    dicUnespecifiedZoneDescription = dicAux
                Else
                    If dicUnespecifiedZoneDescription(Index) = String.Empty Then
                        Dim oLng As New roLanguage()
                        oLng.SetLanguageReference("ZonesService", langKey)

                        dicUnespecifiedZoneDescription(Index) = oLng.Translate("WorldZone", "Zones")
                    End If
                End If

                Try
                    strRet = dicUnespecifiedZoneDescription(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function GetPassportName(ByVal Index As Integer, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty OrElse Index = -1 Then Return Index.ToString
            Try
                If dicPassportName Is Nothing Then

                    Dim strSQL As String = "@SELECT# ID,Name from sysropassports"
                    Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)

                    Dim dicAux As New Generic.Dictionary(Of Integer, String)
                    For Each oRow In tb.Rows
                        dicAux.Add(roTypes.Any2Integer(oRow("ID")), roTypes.Any2String(oRow("Name")))
                    Next
                    dicPassportName = dicAux
                End If

                Try
                    strRet = dicPassportName(Index)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Sub GetPassportPermission(ByVal IdPassport As Integer)
            Try
                If dicPassportPermissions Is Nothing Then
                    Dim dicAux As New Generic.Dictionary(Of Integer, Permission)
                    dicAux.Add(1530, Security.WLHelper.GetPermissionOverFeature(IdPassport, "Employees.UserFields.Information.Low", "U"))
                    dicAux.Add(1540, Security.WLHelper.GetPermissionOverFeature(IdPassport, "Employees.UserFields.Information.Medium", "U"))
                    dicAux.Add(1550, Security.WLHelper.GetPermissionOverFeature(IdPassport, "Employees.UserFields.Information.High", "U"))
                    dicPassportPermissions = dicAux
                End If
            Catch
                'do nothing
            End Try
        End Sub

        Private Function GetVersionDescription(ByVal value As String, langKey As String) As String
            Dim strRet As String = String.Empty
            If langKey = String.Empty Then Return value.ToString
            Try
                If dicVersionDescription Is Nothing Then
                    Dim oLng As New roLanguage()
                    oLng.SetLanguageReference("LiveScheduler", langKey)

                    Dim dicAux As New Generic.Dictionary(Of String, String)
                    dicAux.Add("RXFFNG", oLng.Translate("RXFFNG", "analyticsscheduler"))
                    dicAux.Add("ZKUNIPAL", oLng.Translate("ZKUNIPAL", "analyticsscheduler"))
                    dicAux.Add("ZKUNIFAC", oLng.Translate("ZKUNIFAC", "analyticsscheduler"))
                    dicVersionDescription = dicAux
                End If

                Try
                    strRet = dicVersionDescription(value)
                Catch ex As Exception
                    strRet = ""
                End Try
            Catch
                strRet = ""
            End Try

            Return strRet

        End Function

        Private Function ApplyFormat(field As String, index As Integer, types As List(Of String)) As String
            Try
                If field IsNot Nothing AndAlso field <> String.Empty Then
                    If types(index - 1) = "1" OrElse types(index - 1) = "3" Then
                        Return field.Replace(".", roConversions.GetDecimalDigitFormat)
                    Else
                        Return field
                    End If
                Else
                    Return field
                End If
            Catch ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "CAnalyticsServerNetQC::ApplyFormat:", ex)
                Return field
            End Try

        End Function

#End Region

#Region "AuditLive"

        Private Function GetUserAuditCube(ByVal oAzureAudit As Generic.List(Of VTBase.Extensions.roAudit), oPassportTicket As roPassportTicket) As DataTable
            Dim dtAudit As DataTable = New DataTable

            With dtAudit
                .Columns.Add(New DataColumn("ID", GetType(Integer)))
                .Columns.Add(New DataColumn("ActionId", GetType(Integer)))
                .Columns.Add(New DataColumn("Action", GetType(String)))
                .Columns.Add(New DataColumn("Date_ToDateString", GetType(Date)))
                .Columns.Add(New DataColumn("Time_ToTime", GetType(DateTime)))
                .Columns.Add(New DataColumn("ElementId", GetType(Integer)))
                .Columns.Add(New DataColumn("Element", GetType(String)))
                .Columns.Add(New DataColumn("ElementName", GetType(String)))
                .Columns.Add(New DataColumn("PassportName", GetType(String)))
                .Columns.Add(New DataColumn("Message", GetType(String)))
                .Columns.Add(New DataColumn("SessionID", GetType(String)))
            End With


            Try
                Dim tbObjects As DataTable = roAudit.GetAuditObjectTypes(oPassportTicket.Language.Key)
                Dim tbActions As DataTable = roAudit.GetAuditActions(oPassportTicket.Language.Key)


                Dim oAuditRow As DataRow
                For Each oAudit In oAzureAudit
                    oAuditRow = dtAudit.NewRow
                    oAuditRow("ID") = oAudit.ID
                    oAuditRow("ActionId") = CInt(oAudit.Action)
                    oAuditRow("Action") = tbActions.Select($"ActionID ={CInt(oAudit.Action)}")(0)("ActionDesc")
                    oAuditRow("Date_ToDateString") = oAudit._Date.Date
                    oAuditRow("Time_ToTime") = oAudit._Date.ToString("g")
                    oAuditRow("ElementId") = CInt(oAudit.ObjectType)
                    oAuditRow("Element") = tbObjects.Select($"ElementID ={CInt(oAudit.ObjectType)}")(0)("ElementDesc")
                    oAuditRow("ElementName") = oAudit.ObjectName
                    oAuditRow("PassportName") = oAudit.UserName

                    Try
                        Dim clientLocation As String = oAudit.ClientLocation
                        Dim sessionId As String
                        If clientLocation.Contains(":") AndAlso clientLocation.Contains("#") Then
                            Dim parts As String() = clientLocation.Split(":"c)
                            sessionId = parts(0) & clientLocation.Substring(clientLocation.IndexOf("#"c))
                        Else
                            sessionId = clientLocation
                        End If

                        oAuditRow("SessionID") = sessionId
                    Catch ex As Exception
                    End Try


                    oAuditRow("Message") = oAudit.GetMessage(oPassportTicket.Language.Key)


                    dtAudit.Rows.Add(oAuditRow)
                Next

            Catch ex As Exception
                dtAudit.Rows.Clear()
            End Try

            dtAudit.AcceptChanges()

            Return dtAudit
        End Function



        Private Function ScheduleToAnalytics_AuditAzure(ByVal oAzureAudit As Generic.List(Of VTBase.Extensions.roAudit)) As List(Of Analytics_Audit)
            Dim oAudit As New List(Of Analytics_Audit)

            For Each cAudit As VTBase.Extensions.roAudit In oAzureAudit
                oAudit.Add(New DTOs.Analytics_Audit() With {
                    .ActionID = CInt(cAudit.Action),
                    .ClientLocation = cAudit.ClientLocation,
                    .[Date] = cAudit._Date,
                    .ElementID = CInt(cAudit.ObjectType),
                    .ElementName = cAudit.ObjectName,
                    .ID = cAudit.ID,
                    .MessageParameters = roJSONHelper.SerializeNewtonSoft(cAudit.Parameters),
                    .PassportName = cAudit.UserName,
                    .SessionID = cAudit.SessionID
                })
            Next

            Return oAudit
        End Function

#End Region

        Public Function GenerateScheduledTasks() As BaseTaskResult
            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String = "@SELECT# ID, IdPassport FROM Genius_Views_Scheduler where NextDateTimeExecuted <=" & Robotics.VTBase.roTypes.Any2Time(Now).SQLSmallDateTime & " AND NextDateTimeExecuted is not null"
                Dim tb2 As DataTable = CreateDataTable(strSQL)
                If tb2 IsNot Nothing AndAlso tb2.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb2.Rows

                        Dim oPassport As roPassport = roPassportManager.GetPassport(oRow("IDPassport"), LoadType.Passport, New roSecurityState())
                        If oPassport.IsActivePassport Then

                            Dim bGenTask As Boolean = True
                            If oPassport.IDEmployee.HasValue AndAlso oPassport.IDEmployee > 0 Then
                                Dim sSQL As String = "@SELECT# IDEmployee from sysrosubvwCurrentEmployeePeriod where idemployee=" & oPassport.IDEmployee.Value.ToString
                                bGenTask = Robotics.VTBase.roTypes.Any2Integer(Robotics.DataLayer.AccessHelper.ExecuteScalar(sSQL)) > 0
                            End If

                            If bGenTask Then RunGeniusTaskById(oRow("ID"), True)
                        End If
                    Next
                End If
            Catch Ex As Exception
                'Stop
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CAnalyticsServerNet::CheckSchedulerTasks :", Ex)
                bolRet = False
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = String.Empty}
        End Function

        Public Shared Function RunGeniusTaskById(idScheduledTask As Integer, ByVal bUpdateTask As Boolean) As Boolean
            Dim bRes As Boolean = True

            Try

                Dim oGeniusScheduler As roGeniusScheduler = Nothing
                Dim oGeniusSchedulerManager As roGeniusSchedulerManager = Nothing

                oGeniusSchedulerManager = New roGeniusSchedulerManager(New roAnalyticState(-1))
                oGeniusScheduler = oGeniusSchedulerManager.Load(idScheduledTask, False)
                If oGeniusScheduler IsNot Nothing Then
                    Dim BeginPeriod As Date
                    Dim EndPeriod As Date
                    roGeniusSchedulerManager.GetAnalyticPeriod(New roAnalyticState(-1), oGeniusScheduler, BeginPeriod, EndPeriod)
                    ' Creamos la tarea con los los parametros de la analitica planificada

                    Dim oGeniusManager As New roGeniusViewManager(New roAnalyticState(oGeniusScheduler.IDPassport))
                    Dim view As roGeniusView = oGeniusManager.Load(oGeniusScheduler.IDGeniusView, False)
                    If view IsNot Nothing Then
                        view.DateInf = BeginPeriod
                        view.DateSup = EndPeriod

                        oGeniusManager.Execute(view, True, True, True)
                    End If

                    If bUpdateTask Then
                        ' Actualizamos la fecha de proxima ejecución de la Analítica planificada
                        Dim exError As Exception = Nothing
                        Dim dNextDateTimeExecuted As Nullable(Of Date)
                        dNextDateTimeExecuted = Support.roLiveSupport.GetNextRun(roReportSchedulerScheduleManager.retScheduleString(oGeniusScheduler.Scheduler), oGeniusScheduler.NextDateTimeExecuted, exError)

                        If dNextDateTimeExecuted.HasValue AndAlso dNextDateTimeExecuted.Value <> Nothing Then
                            oGeniusScheduler.NextDateTimeExecuted = dNextDateTimeExecuted
                        Else
                            oGeniusScheduler.NextDateTimeExecuted = Nothing
                        End If

                        oGeniusScheduler.StateReport = StateReportEnum.Executing
                        oGeniusScheduler.LastExecution = System.DateTime.Now
                        oGeniusScheduler.LastDateTimeUpdated = System.DateTime.Now
                        oGeniusSchedulerManager.Save(oGeniusScheduler)
                    End If

                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CAnalyticsServerNet::CheckSchedulerTasks::ReportSchedulerTask" & idScheduledTask.ToString, ex)
                bRes = False
            End Try

            Return bRes
        End Function

    End Class

End Namespace