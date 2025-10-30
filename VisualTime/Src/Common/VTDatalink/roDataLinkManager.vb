Imports System.Data.Common
Imports System.Threading
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace DataLink

    Public Class roDataLinkManager
        Private oState As roDataLinkGuideState = Nothing
        Private oShiftCache As New Hashtable

        Public ReadOnly Property State As roDataLinkGuideState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roDataLinkGuideState()
        End Sub

        Public Sub New(ByVal _State As roDataLinkGuideState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal eConcept As roDatalinkConcept, Optional ByVal bAudit As Boolean = False) As roDatalinkGuide

            Dim oGuide As roDatalinkGuide = Nothing

            Try
                oGuide = New roDatalinkGuide
                oGuide.Concept = eConcept

                Dim oExportManager As New roDataLinkExportManager(Me.oState)
                Dim oImportManager As New roDataLinkImportManager(Me.oState)

                oGuide.Import = oImportManager.LoadGuide(eConcept)
                oGuide.Export = oExportManager.LoadGuide(eConcept)
                oGuide.IsCustom = ((oGuide.Import IsNot Nothing AndAlso oGuide.Import.IsCustom) OrElse (oGuide.Export IsNot Nothing AndAlso oGuide.Export.IsCustom))

                If oGuide.IsCustom AndAlso oGuide.Export IsNot Nothing AndAlso oGuide.Export.Name.EndsWith("*") Then
                    oGuide.Name = oGuide.Export.Name.Substring(0, oGuide.Export.Name.Length - 1)
                Else
                    oGuide.Name = Me.oState.Language.Translate("DatalinkConcept.Name." & eConcept.ToString, "")
                End If

                If oGuide.Import IsNot Nothing Then
                    oGuide.ImportDescription = Me.oState.Language.Translate("DatalinkConcept.Name." & eConcept.ToString & ".ImportDesc", "")
                End If
                If oGuide.Export IsNot Nothing Then
                    oGuide.ExportDescription = Me.oState.Language.Translate("DatalinkConcept.Name." & eConcept.ToString & ".ExportDesc", "")
                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkManager::Load")
            End Try

            Return oGuide

        End Function

        Public Function Save(ByRef oGuide As roDatalinkGuide, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                Dim oExportManager As roDataLinkExportManager
                Dim oImportManager As roDataLinkImportManager

                If oGuide.Import Is Nothing AndAlso oGuide.Export Is Nothing Then
                    Me.oState.Result = DataLinkGuideResultEnum.ConnectionError
                    bolRet = False
                End If

                If bolRet AndAlso oGuide.Export IsNot Nothing Then
                    oExportManager = New roDataLinkExportManager(Me.oState)
                    bolRet = oExportManager.Save(oGuide.Export)
                End If

                If bolRet AndAlso oGuide.Import IsNot Nothing Then
                    oImportManager = New roDataLinkImportManager(Me.oState)
                    bolRet = oImportManager.Save(oGuide.Import)
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oDatalinkGuide As roDatalinkGuide) As Boolean
            Dim bolRet As Boolean = True

            Try
                Me.oState.Result = DataLinkGuideResultEnum.NoError
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function GetDataLinkGuides(Optional ByVal bAudit As Boolean = False) As List(Of roDatalinkGuide)
            Dim bolRet As New List(Of roDatalinkGuide)

            Try
                Dim oLicSupport As New roLicenseSupport()
                Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

                Dim sVTEdiction As String = String.Empty
                If oLicInfo.Edition <> roServerLicense.roVisualTimeEdition.NotSet Then
                    sVTEdiction = oLicInfo.Edition.ToString
                End If

                Dim strQuery = "@SELECT# Distinct Concept FROM ImportGuides " &
                                " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = RequieredFunctionalities AND pof.Permission > 0 " &
                                " WHERE Version >= 2 " & IIf(sVTEdiction.Length > 0, " AND CHARINDEX('" & sVTEdiction & "',Edition)> 0", "") &
                    " UNION " &
                                "@SELECT# distinct Concept From ExportGuides " &
                                " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = RequieredFunctionalities AND pof.Permission > 0 " &
                                " where Version >= 2 " & IIf(sVTEdiction.Length > 0, " AND CHARINDEX('" & sVTEdiction & "',Edition)> 0", "")

                Dim tbDatalinkGuides As DataTable = CreateDataTable(strQuery)
                If (tbDatalinkGuides IsNot Nothing AndAlso tbDatalinkGuides.Rows.Count > 0) Then
                    For Each rowDatalinkGuide As DataRow In tbDatalinkGuides.Rows
                        Dim oDatalinkGuide = New roDatalinkGuide
                        oDatalinkGuide = Me.Load(DirectCast([Enum].Parse(GetType(roDatalinkConcept), rowDatalinkGuide("Concept")), roDatalinkConcept))
                        bolRet.Add(oDatalinkGuide)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::GetDataLinkGuides")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::GetDataLinkGuides")
            End Try
            Return bolRet
        End Function

        Public Function ExecuteScheduledTasks() As Boolean

            Dim bolRet As Boolean = True
            Try

                Dim strSQL As String = String.Empty

                ' ------------------------- EXPORTACIONES -------------------------------------
                ' En el caso que se sobrepase del maximo de empleados activos no realizamos ninguna exportacion automatica
                Dim oLicSupport As New Extensions.roLicenseSupport()
                Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()
                If oLicSupport.CheckLicenseLimits(Now.Date, oLicInfo) Then
                    ' Verificamos si existen alguna exportacion automatica a procesar

                    'strSQL = "@SELECT# * FROM ExportGuides WHERE ID >=@id and Mode=@mode and NextExecution<=GetDate() and version = 2 and Enabled = 1 Order by NextExecution"
                    strSQL = "@SELECT# ExportGuidesSchedules.ID as IDSchedule, ExportGuidesSchedules.Name,'' AS DatasourceFile,ExportGuidesSchedules.ExportFileName, " &
                             "ExportGuidesSchedules.Destination,0 AS IntervalMinutes,1 AS StartCalculDay,ExportGuidesSchedules.Separator, " &
                             "ExportGuidesSchedules.NextExecution,ExportGuidesSchedules.ExportFileType,ExportGuides.ProfileType, " &
                             " ExportGuidesSchedules.AutomaticDatePeriod,ExportGuidesSchedules.IDTemplate As IDDefaultTemplate," &
                             "ExportGuides.ID,ExportGuidesSchedules.EmployeeFilter,ExportGuidesSchedules.ApplyLockDate," &
                             "'' AS PostProcessScript, '' AS ProfileName, NULL as Field_1, NULL as Field_2," &
                             "NULL as Field_3, NULL as Field_4, NULL as Field_5, NULL as Field_6," &
                             "ExportGuidesSchedules.ExportFileNameTimeStampFormat " &
                             "FROM ExportGuidesSchedules " &
                             "INNER JOIN ExportGuides On ExportGuidesSchedules.IDGuide = ExportGuides.ID " &
                             "INNER JOIN ExportGuidesTemplates ON ExportGuidesSchedules.IDTemplate = ExportGuidesTemplates.ID " &
                             "WHERE ExportGuidesSchedules.NextExecution <= GetDate() AND ExportGuides.Version = 2 " &
                             "AND ExportGuidesSchedules.Enabled = 1 " &
                             "ORDER BY NextExecution"

                    Dim tbImp As DataTable = CreateDataTable(strSQL)

                    If tbImp IsNot Nothing AndAlso tbImp.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbImp.Rows
                            ' Procesamos el fichero de exportacion en caso necesario
                            If oRow("NextExecution") < Now Then
                                ' 0.- Creo tarea (que lanzará mensaje a la cola)
                                Dim IDExport As Integer = oRow("ID")
                                Dim IDTemplate As Integer = oRow("IDDefaultTemplate") ' Cargar desde el idschedule
                                Dim IDSchedule As Integer = roTypes.Any2Integer(oRow("IDSchedule")) ' Cargar desde la planificacion
                                Dim EmployeesSelected As String = roTypes.Any2String(oRow("EmployeeFilter"))
                                Dim ScheduleBeginDate As Date
                                Dim ScheduleEndDate As Date
                                Dim ProfileType As Integer = oRow("ProfileType")
                                Dim bApplyLockDate As Boolean = roTypes.Any2Boolean("ApplyLockDate")
                                Dim Destination As String = roTypes.Any2String(oRow("Destination"))
                                Dim ExportFileName As String = roTypes.Any2String(oRow("ExportFileName"))
                                Dim ExportFileType As String = roTypes.Any2String(oRow("ExportFileType"))
                                Dim Separator As String = roTypes.Any2String(oRow("Separator"))
                                If Not IsDBNull(oRow("AutomaticDatePeriod")) Then
                                    Dim oPeriodConf As DTOs.roDateTimePeriod = Robotics.VTBase.roSupport.String2DateTimePeriod(roTypes.Any2String(oRow("AutomaticDatePeriod")))
                                    ScheduleBeginDate = oPeriodConf.BeginDateTimePeriod.Date
                                    ScheduleEndDate = oPeriodConf.EndDateTimePeriod.Date
                                End If

                                Dim oTaskState As New roLiveTaskState(oState.IDPassport)

                                bolRet = CreateExportBackground(IDExport, IDTemplate, IDSchedule, EmployeesSelected, ScheduleBeginDate, ScheduleEndDate, ProfileType, bApplyLockDate, Destination, ExportFileName, Separator, ExportFileType, oTaskState)
                            End If
                        Next
                    End If
                Else
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkManager::ExecuteScheduledTasks:Maximum active employees limit has been exceeded. Automatic exports disabled!!")
                End If

                ' ------------------------- IMPORTACIONES -------------------------------------
                strSQL = "@SELECT# * FROM ImportGuides WHERE Mode=@mode and ID in (20,21,22,23,25,26,27,12) and version = 2 and Enabled = 1"
                Dim parametersImp = New List(Of CommandParameter) From
                {
                    New CommandParameter("@mode", CommandParameter.ParameterType.tInt, 2)
                }

                Dim tbExp As DataTable = CreateDataTable(strSQL, parametersImp)

                If tbExp IsNot Nothing AndAlso tbExp.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbExp.Rows
                        Dim strName As String = String.Empty
                        Dim strExtension As String
                        Dim strCompany As String = Azure.RoAzureSupport.GetCompanyName
                        Dim strOriginalFileName As String = roTypes.Any2String(oRow("SourceFilePath"))
                        Dim arrFilenameAux As String() = strOriginalFileName.Split(".")

                        If arrFilenameAux.Length = 1 Then
                            roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roDataLinkManager::ExecuteScheduledTasks::Import for item " & strOriginalFileName & " aborted. Expected extension.")
                            Continue For
                        End If

                        strExtension = arrFilenameAux.Last
                        strName = strOriginalFileName.Substring(0, strOriginalFileName.Length - strExtension.Length - 1)

                        Dim lFilesToImport As New List(Of String)
                        lFilesToImport = Azure.RoAzureSupport.ListFiles(strName, strExtension, roLiveQueueTypes.datalink, roLiveDatalinkFolders.import.ToString, True, strCompany)

                        Dim IDImport As Integer = oRow("ID")
                        Dim IDTemplate As Integer = oRow("IDDefaultTemplate") ' Cargar desde el idschedule
                        Dim oTaskState As New roLiveTaskState(oState.IDPassport)

                        For Each sFileName As String In lFilesToImport
                            If ExistsSameTaskRunning(roLiveTaskTypes.Import, IDImport) Then
                                ' Si se esta ejecutando otra importación del mismo tipo, debemos esperar a que finalice la anterior
                                roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roDataLinkManager::ExecuteScheduledTasks::Another import task is being processed, wait for it. Type: " & IDImport.ToString & ".")
                                Exit For
                            End If

                            Dim oTaskStateImport As New roLiveTaskState(oState.IDPassport)
                            Dim sRenamedName As String = "WIP_" & sFileName.Replace("." & strExtension, "_WIP" & "." & strExtension)
                            Azure.RoAzureSupport.RenameFileInCompanyContainer(sFileName, sRenamedName, roLiveDatalinkFolders.import.ToString, roLiveQueueTypes.datalink)
                            bolRet = CreateImportBackground(IDImport, sRenamedName, oTaskStateImport, IDTemplate)
                        Next
                    Next
                End If

                ' Sincronización de CTAIMA
                Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                If ctaimaSystem.IsEnabled() Then
                    Dim oNextRun As DateTime = ctaimaSystem.GetNextScheduleTime()
                    If oNextRun <= DateTime.Now Then
                        If ExistsSameTaskRunning(roLiveTaskTypes.CTAIMA, Nothing) Then
                            ' Si se esta ejecutando otra tarea del mismo tipo, debemos esperar a que finalice la anterior
                            roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roDataLinkManager::ExecuteScheduledTasks::Another ctaima task is being processed, wait for it.")
                        Else
                            bolRet = CreateCTAIMABackgroundTask()
                        End If

                    End If
                End If

                ' Sincronización con terminales Suprema
                Dim supremaSystem As Suprema.SupremaSystem = New Suprema.SupremaSystem
                If supremaSystem.IsEnabled() Then
                    Dim oNextRun As DateTime = supremaSystem.GetNextScheduleTime()
                    If oNextRun <= DateTime.Now Then
                        If ExistsSameTaskRunning(roLiveTaskTypes.Suprema, Nothing) Then
                            ' Si se esta ejecutando otra tarea del mismo tipo, debemos esperar a que finalice la anterior
                            roLog.GetInstance.logMessage(roLog.EventType.roDebug, "roDataLinkManager::ExecuteScheduledTasks::Another Suprema task is being processed, wait for it.")
                        Else
                            bolRet = CreateSupremaBackgroundTask()
                        End If
                    End If
                End If
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::ExecuteScheduledTasks")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper"

        Public Shared Function GetUniqueFileName(sPrefix As String, sExtension As String) As String
            Return sPrefix & "_" & Now.Ticks.ToString & "." & sExtension
        End Function

        Public Function CreateImportBackground(ByVal IDImport As Integer, ByVal ImportFileName As String, ByVal oState As roLiveTaskState, ByVal IDImportTemplate As Integer) As Boolean

            Dim oResult As Boolean = False

            Try
                Dim strQuery As String = "@UPDATE# ImportGuides set LastLog='' where id=" & IDImport

                Dim oSqlCommand As DbCommand = CreateCommand(strQuery)
                oSqlCommand.ExecuteNonQuery()

                ' Crea la tarea de importación
                Dim oParameters As New roCollection

                oParameters.Add("IDImport", IDImport)
                oParameters.Add("oFileOrig", ImportFileName)

                ' Estos se pueden necesitar a futuro
                oParameters.Add("ScheduleBeginDate", Now)
                oParameters.Add("ScheduleEndDate", Now)
                oParameters.Add("EmployeesSelected", "")
                oParameters.Add("CopyMainShifts", True)
                oParameters.Add("ExcelIsTemplate", True)
                oParameters.Add("CopyHolidays", True)
                oParameters.Add("KeepHolidays", True)
                oParameters.Add("KeepLockedDays", True)
                oParameters.Add("IsBackground", "True")
                oParameters.Add("Separator", "")
                oParameters.Add("FileType", 0)

                ' Scripts de pre y post proceso
                Dim oDataLinkState As New roDataLinkState(Me.oState.IDPassport)
                Dim oImportGuide As New DataLink.roImportGuide(IDImport, oDataLinkState)

                If oImportGuide IsNot Nothing Then
                    Try
                        Dim oImportGuideTemplate As New DataLink.roImportGuideTemplate
                        If oImportGuideTemplate.LoadByParentId(oImportGuide.ID, IDImportTemplate) Then
                            oParameters.Add("PreProcessScript", oImportGuideTemplate.PreProcessScript)
                            oParameters.Add("PostProcessScript", oImportGuideTemplate.PostProcessScript)
                            If oImportGuide.Version = 2 Then
                                oParameters.Add("ExcelTemplateFile", oImportGuideTemplate.Profile)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                Dim oLiveTaskState As New roLiveTaskState(Me.oState.IDPassport)
                roLiveTask.CreateLiveTask(roLiveTaskTypes.Import, oParameters, oLiveTaskState)

                oResult = (oState.Result = LiveTasksResultEnum.NoError)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "VTDataLink::roDatalinkManager:CreateImportBackground")
            End Try

            Return oResult

        End Function

        Public Function CreateExportBackground(ByVal IDExport As Integer, ByVal IDExportTemplate As Integer, ByVal IDSchedule As Integer, ByVal EmployeesSelected As String, ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date,
                                       ByVal ProfileType As Integer, ByVal bApplyLockDate As Boolean, ByVal Destination As String, ByVal ExportFileName As String, ByVal Separator As String,
                                       ByVal ExportFileType As String, ByVal oState As roLiveTaskState) As Boolean

            Dim oResult As Boolean = False

            Dim oParameters As New roCollection

            Dim intLiveTaskType As Integer = roLiveTaskTypes.Export

            Try
                Dim idPassport As Integer = -1
                Dim conf() As String = roTypes.Any2String(EmployeesSelected).Split("@")
                idPassport = roTypes.Any2Integer(conf(1))

                Dim tmpEmployeeSelected As String = String.Empty
                Dim arrEmployeesSelected() As String = EmployeesSelected.Split("@")
                For i = 0 To arrEmployeesSelected.ToList.Count - 1
                    If i <> 1 Then
                        tmpEmployeeSelected = tmpEmployeeSelected & arrEmployeesSelected.ToList(i) & "@"
                    End If
                Next
                EmployeesSelected = tmpEmployeeSelected

                oParameters.Add("IDExport", IDExport)
                oParameters.Add("IsBackground", "True")
                oParameters.Add("FileType", ExportFileType)
                oParameters.Add("EmployeesSelected", EmployeesSelected)
                oParameters.Add("inExcel2007Format", "True")
                oParameters.Add("isExcelInstalled", "True")
                oParameters.Add("ScheduleBeginDate", ScheduleBeginDate.ToString("yyyy/MM/dd HH:mm"))
                oParameters.Add("ScheduleEndDate", ScheduleEndDate.ToString("yyyy/MM/dd HH:mm"))
                oParameters.Add("ConceptGroup", "")
                oParameters.Add("Separator", Separator)
                oParameters.Add("ProfileType", ProfileType)
                oParameters.Add("ApplyLockDate", bApplyLockDate)
                oParameters.Add("Destination", Destination)
                oParameters.Add("ExportFileName", ExportFileName)
                oParameters.Add("IDExportTemplate", IDExportTemplate)
                oParameters.Add("IDSchedule", IDSchedule)

                Dim oDataLinkState As New roDataLinkState(idPassport)
                Dim oExportGuide As New DataLink.roExportGuide(IDExport, oDataLinkState)

                If Not oExportGuide Is Nothing Then
                    ' Si es una exportación de tipo 2, busco la plantilla y posibles scripts de postproceso
                    ' Para cualquier tipo de plantilla, busco si hay scripts de postproceso (es una estandarización de los ya existentes, que se indicaban en la propia plantilla)
                    Try
                        Dim oExportGuideTemplate As New DataLink.roExportGuideTemplate
                        If oExportGuideTemplate.LoadByParentId(oExportGuide.ID, IDExportTemplate) Then
                            oParameters.Add("PreProcessScript", oExportGuideTemplate.PreProcessScript)
                            oParameters.Add("PostProcessScript", oExportGuideTemplate.PostProcessScript)
                            If oExportGuide.Version = 2 Then
                                oParameters.Add("ExcelTemplateFile", oExportGuideTemplate.Profile)
                            End If
                        End If
                    Catch ex As Exception
                    End Try
                End If

                If ProfileType = 999 Then
                    ' Si es una exportación personalizada a un WS externo, indicamos la action CustomExport y el EndPointAdress correspondiente
                    intLiveTaskType = roLiveTaskTypes.CustomExport
                    Dim EndpointAddress As String = ""
                    If Not oExportGuide Is Nothing Then EndpointAddress = oExportGuide.ExportFileName
                    oParameters.Add("EndpointAddress", EndpointAddress)
                End If

                Dim oLiveTaskState As New roLiveTaskState(idPassport)
                roLiveTask.CreateLiveTask(intLiveTaskType, oParameters, oLiveTaskState)

                oResult = (oState.Result = LiveTasksResultEnum.NoError)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkManager::CreateExportBackground")
            End Try

            Return oResult
        End Function

        Public Function CreateCTAIMABackgroundTask() As Boolean

            Dim oResult As Boolean = False

            Try
                Dim oParameters As New roCollection

                oParameters.Add("ScheduleBeginDate", Now)
                oParameters.Add("ScheduleEndDate", Now)

                Dim oLiveTaskState As New roLiveTaskState(Me.oState.IDPassport)
                roLiveTask.CreateLiveTask(roLiveTaskTypes.CTAIMA, oParameters, oLiveTaskState)

                oResult = (oState.Result = LiveTasksResultEnum.NoError)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "VTDataLink::roDatalinkManager:CreateCTAIMASoapTask")
            End Try

            Return oResult

        End Function

        Public Function CreateSupremaBackgroundTask() As Boolean

            Dim oResult As Boolean = False

            Try
                Dim oParameters As New roCollection

                oParameters.Add("ScheduleBeginDate", Now)
                oParameters.Add("ScheduleEndDate", Now)

                Dim oLiveTaskState As New roLiveTaskState(Me.oState.IDPassport)
                roLiveTask.CreateLiveTask(roLiveTaskTypes.Suprema, oParameters, oLiveTaskState)

                oResult = (oState.Result = LiveTasksResultEnum.NoError)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "VTDataLink::roDatalinkManager:CreateSupremaTask")
            End Try

            Return oResult

        End Function


        Public Function ExistsSameTaskRunning(ByVal action As roLiveTaskTypes, ByVal IDImport As Integer) As Boolean
            '
            ' Buscamos otra tarea del mismo tipo que se este ejecutandose o esperando a ser ejecutada
            '

            Dim bolRet As Boolean = False

            Try
                Dim tb As New DataTable("sysroLiveTasks")
                Dim strSQL As String = "@SELECT# * FROM sysroLiveTasks WITH (nolock) WHERE Action LIKE '" & action.ToString().ToUpper() & "' AND Status IN(0,1) "
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb.Rows.Count > 0 Then
                    Dim oState As roLiveTaskState = New roLiveTaskState(-1)

                    For Each orow As DataRow In tb.Rows
                        Dim oTask As roLiveTask = New roLiveTask(roTypes.Any2Integer(orow("ID")), oState)
                        If oTask.ID > 0 Then
                            Select Case action
                                Case roLiveTaskTypes.Suprema, roLiveTaskTypes.CTAIMA
                                    If Now.Subtract(oTask.TimeStamp).TotalMinutes > 15 Then
                                        oTask.Delete()
                                    Else
                                        bolRet = True
                                        Exit For
                                    End If
                                Case Else
                                    If roTypes.Any2Integer(oTask.Parameters("IDImport")) = IDImport Then
                                        bolRet = True
                                        Exit For
                                    End If
                            End Select
                        End If
                    Next
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "VTDataLink::roDatalinkManager:ExistsSameTaskRunning")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace