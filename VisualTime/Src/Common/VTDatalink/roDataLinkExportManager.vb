Imports System.Data.Common
Imports DocumentFormat.OpenXml.Drawing.Diagrams
Imports DocumentFormat.OpenXml.Wordprocessing
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roDataLinkExportManager
        Private oState As roDataLinkGuideState = Nothing
        Private oShiftCache As New Hashtable

        Public ReadOnly Property State As roDataLinkGuideState
            Get
                Return oState
            End Get
        End Property

        Public Sub New(ByVal _State As roDataLinkGuideState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function LoadGuideById(iID As Integer, Optional ByVal bAudit As Boolean = False) As roDatalinkExportGuide
            Dim eConcept As roDatalinkConcept

            Try

                Dim strSql = "@SELECT# Concept from ExportGuides where Version=2 and ID=" & iID.ToString

                Dim strConcept As String = String.Empty
                strConcept = roTypes.Any2String(ExecuteScalar(strSql))
                eConcept = System.Enum.Parse(eConcept.GetType, strConcept, True)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExportManager::LoadById")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExportManager::LoadById")
            End Try

            Return LoadGuide(eConcept, bAudit)
        End Function

        Public Function LoadGuide(ByVal eConcept As roDatalinkConcept, Optional ByVal bAudit As Boolean = False) As roDatalinkExportGuide

            Dim oGuide As roDatalinkExportGuide = Nothing

            Try
                Dim strSql = "@SELECT# * from ExportGuides "
                strSql = strSql & " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = RequieredFunctionalities AND pof.Permission > 0 "
                strSql = strSql & " WHERE Version = 2 And Concept ='" & eConcept.ToString & "'"
                Dim dtExportGuides As New DataTable
                dtExportGuides = CreateDataTable(strSql)

                Dim oTemplateManager As New roDataLinkExportTemplateManager(Me.oState)
                Dim oRow As DataRow
                If dtExportGuides IsNot Nothing AndAlso dtExportGuides.Rows.Count > 0 Then
                    oRow = dtExportGuides.Rows(0)
                    oGuide = New roDatalinkExportGuide
                    oGuide.Id = roTypes.Any2Integer(oRow("ID"))
                    oGuide.ExecutionMode = roTypes.Any2Integer(oRow("Mode"))
                    oGuide.Version = roTypes.Any2Integer(oRow("Version"))
                    oGuide.IsActive = roTypes.Any2Boolean(oRow("Active"))
                    oGuide.IsEnabled = roTypes.Any2Boolean(oRow("Enabled"))
                    oGuide.Templates = oTemplateManager.GetDataLinkExportGuideTemplates(oGuide).ToArray
                    oGuide.RequiredFunctionalities = roTypes.Any2String(oRow("RequieredFunctionalities"))
                    oGuide.FeatureAliasID = roTypes.Any2String(oRow("FeatureAliasID"))
                    oGuide.ProfileType = roTypes.Any2Integer(oRow("ProfileType"))
                    oGuide.Concept = roTypes.Any2String(oRow("Concept"))
                    oGuide.DefaultSeparator = roTypes.Any2String(oRow("Separator"))
                    oGuide.IsCustom = roTypes.Any2Boolean(oRow("IsCustom"))
                    oGuide.Name = roTypes.Any2String(oRow("Name"))

                    oGuide.Schedules = LoadSchedules(oGuide.Id)
                End If

                ' Auditar lectura
                If bAudit AndAlso oGuide IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            End Try

            Return oGuide
        End Function

        Public Function LoadSchedules(ByVal iProfileType As Integer) As roDatalinkExportSchedule()

            Dim oGuideList As roDatalinkExportSchedule() = {}
            Try

                Dim strSql = "@SELECT# * from ExportGuidesSchedules where IDGuide =" & iProfileType & ""

                Dim dtExportGuides As New DataTable
                dtExportGuides = CreateDataTable(strSql)

                If dtExportGuides IsNot Nothing AndAlso dtExportGuides.Rows.Count > 0 Then
                    Dim oTmpList As New Generic.List(Of roDatalinkExportSchedule)

                    For Each oRow As DataRow In dtExportGuides.Rows
                        Dim oGuide As New roDatalinkExportSchedule
                        oGuide.Id = roTypes.Any2Integer(oRow("Id"))
                        oGuide.Name = roTypes.Any2String(oRow("Name"))
                        oGuide.ExportFileName = roTypes.Any2String(oRow("ExportFileName"))
                        oGuide.EmployeeFilter = roTypes.Any2String(oRow("EmployeeFilter"))
                        oGuide.AutomaticDatePeriod = roTypes.Any2String(oRow("AutomaticDatePeriod"))
                        oGuide.ApplyLockDate = roTypes.Any2Boolean(oRow("ApplyLockDate"))
                        oGuide.NextExecutionDate = roTypes.Any2DateTime(oRow("NextExecution"))
                        oGuide.Scheduler = roReportSchedulerScheduleManager.Load(roTypes.Any2String(oRow("Scheduler")))
                        oGuide.DataSourceFileName = roTypes.Any2String(oRow("DataSourceFile"))
                        oGuide.ExportFileType = roTypes.Any2String(oRow("ExportFileType"))
                        oGuide.Separator = roTypes.Any2String(oRow("Separator"))
                        oGuide.LastExecutionLog = roTypes.Any2String(oRow("LastLog"))
                        oGuide.Location = roTypes.Any2String(oRow("Destination"))
                        oGuide.IdTemplate = roTypes.Any2Integer(oRow("IDTemplate"))
                        oGuide.IdGuide = roTypes.Any2Integer(oRow("IDGuide"))
                        oGuide.ExportFileNameTimeStampFormat = roTypes.Any2String(oRow("ExportFileNameTimeStampFormat"))
                        oGuide.WSParameters = roTypes.Any2String(oRow("WSParameters"))

                        oGuide.Enabled = roTypes.Any2Boolean(oRow("Enabled"))
                        oGuide.Active = roTypes.Any2Boolean(oRow("Active"))

                        oTmpList.Add(oGuide)
                    Next
                    oGuideList = oTmpList.ToArray

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            End Try

            Return oGuideList
        End Function

        Public Function Save(ByRef oGuide As roDatalinkExportGuide, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                Me.oState.Result = DataLinkGuideResultEnum.NoError

                bolRet = Validate(oGuide)

                If bolRet Then
                    Dim strSQL As String = String.Empty
                    Dim tbAux As New DataTable
                    Dim oRow As DataRow
                    Dim cmd As DbCommand
                    Dim da As DbDataAdapter
                    strSQL = "@SELECT# * from ExportGuides where ID = " & oGuide.Id
                    cmd = CreateCommand(strSQL)
                    da = CreateDataAdapter(cmd, True)

                    da.Fill(tbAux)

                    If tbAux IsNot Nothing AndAlso tbAux.Rows.Count = 1 Then
                        oRow = tbAux.Rows(0)
                        oRow("Enabled") = oGuide.IsEnabled
                        oRow("Separator") = oGuide.DefaultSeparator

                        Dim iListIds As New Generic.List(Of Integer)
                        iListIds.Add(-1)
                        For Each oSchedule In oGuide.Schedules
                            iListIds.Add(oSchedule.Id)
                        Next

                        RemoveNotExistingGuides(oGuide.Id, iListIds)

                        For Each oSchedule In oGuide.Schedules
                            bolRet = (bolRet AndAlso SaveSchedules(oGuide.Id, oSchedule))
                        Next

                        If oGuide.IsEnabled Then
                            oRow("Mode") = 2
                            oRow("Active") = 1
                        Else
                            oRow("Mode") = 1
                            oRow("Active") = 0
                        End If
                        da.Update(tbAux)
                        bolRet = True
                    Else
                        Me.oState.Result = DataLinkGuideResultEnum.NoSuchGuide
                    End If
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function RemoveNotExistingGuides(ByVal idExportGuide As Integer, ByVal lstExistingIds As Generic.List(Of Integer)) As Boolean

            Dim oRet As Boolean = True

            Try

                Dim strSql = "@DELETE# from ExportGuidesSchedules where IDguide = " & idExportGuide & " AND Id not in(" & String.Join(",", lstExistingIds.ToArray) & ")"
                oRet = ExecuteSql(strSql)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            End Try

            Return oRet
        End Function

        Public Function SaveSchedules(ByVal idExportGuide As Integer, ByVal oSchedule As roDatalinkExportSchedule) As Boolean

            Dim oRet As Boolean = True

            Try
                Dim strSql = "@SELECT# * from ExportGuidesSchedules where Id =" & oSchedule.Id & ""

                Dim bolIsNew As Boolean = False
                Dim tbAux As New DataTable
                Dim cmd As DbCommand
                Dim da As DbDataAdapter
                cmd = CreateCommand(strSql)
                da = CreateDataAdapter(cmd, True)

                da.Fill(tbAux)

                Dim oRow As DataRow = Nothing
                If oSchedule.Id <= 0 Then
                    oRow = tbAux.NewRow
                    oRow("ID") = GetNextScheduleID()
                    bolIsNew = True
                ElseIf tbAux.Rows.Count = 1 Then
                    oRow = tbAux.Rows(0)
                    bolIsNew = False
                End If

                oRow("Name") = oSchedule.Name
                oRow("ExportFileName") = oSchedule.ExportFileName
                oRow("EmployeeFilter") = oSchedule.EmployeeFilter
                oRow("AutomaticDatePeriod") = oSchedule.AutomaticDatePeriod

                oRow("ApplyLockDate") = oSchedule.ApplyLockDate
                oRow("Scheduler") = roReportSchedulerScheduleManager.retScheduleString(oSchedule.Scheduler)
                oRow("ExportFileType") = oSchedule.ExportFileType
                oRow("Separator") = oSchedule.Separator
                oRow("Destination") = roTypes.Any2String(oSchedule.Location)
                'oRow("LastLog") = oSchedule.LastExecutionLog

                oRow("ExportFileNameTimeStampFormat") = oSchedule.ExportFileNameTimeStampFormat
                oRow("WSParameters") = oSchedule.WSParameters

                oRow("IdGuide") = oSchedule.IdGuide
                oRow("IdTemplate") = oSchedule.IdTemplate

                oRow("Enabled") = oSchedule.Enabled
                oRow("Active") = oSchedule.Active
                oRow("DatasourceFile") = String.Empty

                Dim gNextException As Exception = Nothing
                Dim nextDate As Nullable(Of Date) = Robotics.Base.VTBusiness.Support.roLiveSupport.GetNextRun(roReportSchedulerScheduleManager.retScheduleString(oSchedule.Scheduler), Nothing, gNextException)
                If nextDate IsNot Nothing Then
                    oRow("NextExecution") = nextDate
                Else
                    oRow("NextExecution") = DBNull.Value
                End If

                If gNextException IsNot Nothing Then
                    Me.oState.Result = DataLinkGuideResultEnum.ErrorSettingNextExecutionTime
                    oState.UpdateStateInfo(gNextException, "roExportGuide::GetNextRun")
                    oRow("Active") = 0
                    oRet = False
                End If

                If oRet Then
                    If bolIsNew Then
                        tbAux.Rows.Add(oRow)
                    End If

                    da.Update(tbAux)
                    oRet = True
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::Load")
            End Try

            Return oRet
        End Function

        Private Function GetNextScheduleID() As Integer
            Dim intRet As Integer = 0
            Dim strSQL As String = "@SELECT# MAX(ID) FROM ExportGuidesSchedules "
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = roTypes.Any2Integer(tb.Rows(0).Item(0))
            End If
            Return intRet + 1
        End Function

        Public Function Validate(ByVal oGuide As roDatalinkExportGuide) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                If bolRet AndAlso oGuide.IsEnabled Then
                    For Each oShcedule In oGuide.Schedules
                        bolRet = bolRet AndAlso ValidateSchedule(oShcedule)
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function ValidateSchedule(ByVal oGuide As roDatalinkExportSchedule) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                If bolRet Then
                    If bolRet AndAlso oGuide.IdTemplate <= 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.ExportTemplateRequired
                        bolRet = False
                    End If

                    If bolRet AndAlso oGuide.ExportFileName.Length = 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.ExportFileRequired
                        bolRet = False
                    End If

                    If bolRet AndAlso (oGuide.Scheduler Is Nothing OrElse roReportSchedulerScheduleManager.retScheduleString(oGuide.Scheduler).Length = 0) Then
                        Me.oState.Result = DataLinkGuideResultEnum.ScheduleRequired
                        bolRet = False
                    End If

                    If bolRet AndAlso oGuide.EmployeeFilter.Split("@")(0).Length = 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.EmployeeFilterRequired
                        bolRet = False
                    End If

                    If bolRet AndAlso oGuide.AutomaticDatePeriod.Length = 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.DatePeriodRequired
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function


        Public Function ExecuteExport(ByRef oExportTask As roDatalinkExportTask) As roDataLinkState
            Dim oDataLinkState As roDataLinkState = New roDataLinkState(Me.oState.IDPassport)
            Dim bTemplateRequired As Boolean = True

            If oExportTask IsNot Nothing AndAlso oExportTask.ExportGuide IsNot Nothing AndAlso oExportTask.ExportParameters IsNot Nothing AndAlso oExportTask.ExportParameters.IdPassport > 0 Then

                Try
                    oDataLinkState.Result = DataLinkResultEnum.NoError


                    Dim oResultFile As Byte() = Nothing
                    If IsStandardTemplate(oExportTask.ExportGuide.ProfileType) Then
                        oResultFile = ExecuteStandardExport(oExportTask, oDataLinkState)
                    Else
                        oResultFile = ExecuteCustomExport(oExportTask, oDataLinkState)
                    End If

                    If oDataLinkState.Result = DataLinkResultEnum.NoError AndAlso oResultFile IsNot Nothing Then

                        Dim fileType As Integer = roTypes.Any2Integer(oExportTask.ExportParameters.FileType)
                        ' Gestión de fichero de salida
                        If Not oExportTask.ExportParameters.IsProgrammed Then
                            ' Exportación manual. Subo a Azure para que la pantalla la descargue en navegador del cliente
                            Azure.RoAzureSupport.SaveFileOnAzure(oResultFile, oExportTask.ExportResultFileName, DTOs.roLiveQueueTypes.datalink)
                        Else

                            Dim oPGPEnabled As Boolean = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "PGP.Enabled"))
                            Dim bExport As Boolean = True
                            If oPGPEnabled Then
                                Try
                                    Dim oPGPHelper As New PGPCryptographyHelper

                                    Dim vtPublicKey = Azure.RoAzureSupport.DownloadFileFromCompanyContainer("pgp.pub", roLiveDatalinkFolders.certificates.ToString, roLiveQueueTypes.datalink)
                                    oResultFile = oPGPHelper.EncryptPGP(oResultFile, vtPublicKey)
                                Catch ex As Exception
                                    bExport = False
                                    oDataLinkState.Result = DataLinkResultEnum.PGPEncryptionFailed
                                    roLog.GetInstance().logMessage(roLog.EventType.roError, "roDataLinkExportManager::ExecuteExport:PGPEncryptionFailed ", ex)
                                End Try
                            End If


                            If bExport Then
                                ' Automática. Envío el resultado a la carpeta de Azure de enlace automático
                                Select Case fileType
                                    Case DataLinkResultFileType.Excel
                                        oExportTask.ExportResultFileName = oExportTask.ExportParameters.FileName & "_" & Now.ToString("yyyyMMddHHmmssff") & ".xlsx"
                                    Case DataLinkResultFileType.Text
                                        oExportTask.ExportResultFileName = oExportTask.ExportParameters.FileName & "_" & Now.ToString("yyyyMMddHHmmssff") & ".txt"
                                    Case Else
                                        oExportTask.ExportResultFileName = oExportTask.ExportParameters.FileName & "_" & Now.ToString("yyyyMMddHHmmssff") & ".xlsx"
                                End Select
                                If Not Azure.RoAzureSupport.SaveFileOnCompanyContainer(New System.IO.MemoryStream(oResultFile), oExportTask.ExportResultFileName, DTOs.roLiveDatalinkFolders.export.ToString, DTOs.roLiveQueueTypes.datalink, True) Then
                                    oDataLinkState.Result = DataLinkResultEnum.ErrorSendingExportResultFile
                                End If
                            End If

                        End If

                        ' En el caso de saldos con plantilla o Previsiones de ausencia con plantilla debemos revisar si hay que
                        ' realizar llamada a WS, en el resto de casos no
                        Dim bolExecuteWS As Boolean = False
                        If (oExportTask.ExportGuide IsNot Nothing AndAlso (oExportTask.ExportGuide.ProfileType = 1 OrElse oExportTask.ExportGuide.ProfileType = 2)) AndAlso oExportTask.ExportGuide.Id >= 9004 Then
                            Dim oStateExportGuide = New roDataLinkState(Me.oState.IDPassport)
                            Dim oExport As New VTDataLink.DataLink.roExportGuide(oExportTask.ExportGuide.Id, oStateExportGuide)
                            If oExport.WSxmlConf IsNot Nothing AndAlso oExport.WSxmlConf.Length > 0 Then
                                If oExport.WSConf IsNot Nothing AndAlso Any2Integer(oExport.WSConf("WSType")) <> 0 Then bolExecuteWS = True
                            End If
                        End If

                        If bolExecuteWS Then
                            oDataLinkState.Result = DataLinkResultEnum.PendingWSExport
                        End If
                    Else
                        oDataLinkState.Result = DataLinkResultEnum.UnableToRecoverExcelProfile
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDataLinkExportManager::ExecuteExport:Exception ", ex)
                    oDataLinkState.Result = DataLinkResultEnum.Exception
                    oDataLinkState.ErrorText = ex.Message
                End Try
            Else
                If oExportTask Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportTask
                ElseIf oExportTask.ExportGuide Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportGuide
                ElseIf oExportTask.ExportParameters Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportParameters
                Else
                    oDataLinkState.Result = DataLinkResultEnum.NoPassportSpecified
                End If
            End If

            Return oDataLinkState
        End Function


        Public Function ExecuteStandardExport(ByRef oExportTask As roDatalinkExportTask, ByRef oDataLinkState As roDataLinkState) As Byte()
            Dim oResultFile As Byte() = Nothing
            Dim bTemplateRequired As Boolean = True

            If oExportTask IsNot Nothing AndAlso oExportTask.ExportGuide IsNot Nothing AndAlso oExportTask.ExportParameters IsNot Nothing AndAlso oExportTask.ExportParameters.IdPassport > 0 Then

                Try
                    oDataLinkState.Result = DataLinkResultEnum.NoError
                    ' Recuperamos plantilla. No todas las guías de exportación las usan ...
                    Dim oExportGuideTemplate As roDatalinkExportGuideTemplate = Nothing
                    Dim oExcelProfileBytes As Byte() = Nothing

                    ' Recuperamos empleados a tratar
                    Dim fileType As Integer = roTypes.Any2Integer(oExportTask.ExportParameters.FileType)
                    Dim iIDSchedule As Integer = oExportTask.ExportParameters.IdSchedule

                    Dim tmpEmployeeFilterTable As String = GetEmployeeFilterOnTemporalTableName(oExportTask)

                    If tmpEmployeeFilterTable <> String.Empty Then
                        Dim idTemplate As Integer = oExportTask.ExportParameters.IdTemplate

                        oExportGuideTemplate = oExportTask.ExportGuide.Templates.ToList.Find(Function(x) x.ID = idTemplate)

                        ' Cargo plantilla
                        If oExportGuideTemplate IsNot Nothing AndAlso oExportGuideTemplate.TemplateFile IsNot Nothing AndAlso oExportGuideTemplate.TemplateFile.Length > 0 Then
                            ' Veo si se trata de una plantilla exclusiva del cliente (las de exportación de calendario), o común (el resto)
                            oExcelProfileBytes = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(oExportGuideTemplate.TemplateFile, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink, False)
                            If oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0 Then
                                ' Si no encontré plantillas propias del cliente, busco las comunes
                                oExcelProfileBytes = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure(oExportGuideTemplate.TemplateFile, DTOs.roLiveQueueTypes.datalink, False)
                            End If
                        Else
                            bTemplateRequired = False
                        End If

                        If bTemplateRequired AndAlso (oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0) Then
                            oDataLinkState.Result = DataLinkResultEnum.UnableToRecoverExcelProfile
                        Else
                            Select Case oExportTask.ExportGuide.ProfileType
                                Case DataLinkExportProfile.Accruals
                                    ' Exportacion avanzada de saldos
                                    oExportTask.ExportResultFileName = "AdvExportAccrual"
                                    Dim field5Date As Date = Now.Date
                                    ' Busco en oExportTask.ExportGuide.Schedules el elemecnto con ID = oExportTask.ExportParameters.IdSchedule
                                    Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                    If exportTaskSchedule IsNot Nothing Then
                                        field5Date = exportTaskSchedule.NextExecutionDate.Date
                                    End If
                                    Dim oAccrualsExport As New roAccrualsExport(oDataLinkState)
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            ' Exportación avanzada de saldos
                                            oResultFile = oAccrualsExport.ExportProfileAccrualsEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportParameters.IdConceptGroup, oExportGuideTemplate.TemplateFile, False, 0, ,,,, field5Date,, oExcelProfileBytes, oExportGuideTemplate.ID, oExportTask.ExportParameters.IsProgrammed, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oAccrualsExport.ExportProfileAccrualsASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportParameters.IdConceptGroup, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, ,,,, field5Date,, oExcelProfileBytes, oExportGuideTemplate.ID, oExportTask.ExportParameters.IsProgrammed, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oAccrualsExport.State
                                Case DataLinkExportProfile.Employees

                                    Dim oEmployeesExport As New roEmployeesExport(oDataLinkState)
                                    oExportTask.ExportResultFileName = "AdvExportEmployees"
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            ' Exportación avanzada de empleados
                                            oResultFile = oEmployeesExport.ExportProfileEmployeeEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oEmployeesExport.ExportProfileEmployeeASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oEmployeesExport.State

                                Case DataLinkExportProfile.Punches
                                    Dim oPunchesExport As New roPunchesExport(oDataLinkState)
                                    oExportTask.ExportResultFileName = "AdvExportPunches"
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            ' Exportación avanzada de fichajes
                                            oResultFile = oPunchesExport.ExportProfilePunchesExEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oPunchesExport.ExportProfilePunchesExASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oPunchesExport.State

                                Case DataLinkExportProfile.DailyCauses
                                    oExportTask.ExportResultFileName = "AdvExportDailyCauses"
                                    If roBusinessSupport.GetCustomizationCode().ToUpper = "TAIF" AndAlso oExportTask.ExportParameters.IsProgrammed Then
                                        Dim currentDate As Date = Now.Date
                                        ' Busco en oExportTask.ExportGuide.Schedules el elemecnto con ID = oExportTask.ExportParameters.IdSchedule
                                        Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                        If exportTaskSchedule IsNot Nothing Then
                                            currentDate = exportTaskSchedule.NextExecutionDate.Date
                                        End If
                                        If currentDate.Day <= 16 Then
                                            oExportTask.ExportParameters.BeginDate = DateSerial(currentDate.AddMonths(-1).Year, currentDate.AddMonths(-1).Month, 1)
                                            oExportTask.ExportParameters.EndDate = currentDate.AddDays(-1)
                                        Else
                                            oExportTask.ExportParameters.BeginDate = DateSerial(currentDate.Year, currentDate.Month, 1)
                                            oExportTask.ExportParameters.EndDate = currentDate.AddDays(-1)
                                        End If
                                    End If
                                    Dim oCausesExport As New roCausesExport(oDataLinkState)
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            ' Exportación avanzada de justificaciones Diarias
                                            oResultFile = oCausesExport.ExportProfileDailyCausesEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oCausesExport.ExportProfileDailyCausesASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oCausesExport.State
                                Case DataLinkExportProfile.Planning
                                    If oExportGuideTemplate.IdParentGuide = 20003 AndAlso oExcelProfileBytes Is Nothing Then

                                        Select Case oExportGuideTemplate.IdName
                                            Case "schedule"
                                                Dim employeeConf As String() = oExportTask.ExportParameters.EmployeesSelected.Split("@")
                                                Dim strListEmployees As String = employeeConf(0) & "¬" & employeeConf(2) & "¬" & employeeConf(3)
                                                Dim oScheduleExport As New roFixedExportSchedule(oDataLinkState)
                                                oExportTask.ExportResultFileName = "AdvExportPlanning_Schedule"
                                                oResultFile = oScheduleExport.ExportSchedule(strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, iIDSchedule)
                                                oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                                oDataLinkState = oScheduleExport.State
                                            Case "holidays"
                                                Dim oScheduleExport As New roFixedExportHolidays(oDataLinkState)
                                                oExportTask.ExportResultFileName = "AdvExportPlanningHolidays"
                                                oResultFile = oScheduleExport.ExportHolidays(tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, True, iIDSchedule)
                                                oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                                oDataLinkState = oScheduleExport.State
                                            Case "holidaysonly"
                                                Dim oScheduleExport As New roFixedExportHolidays(oDataLinkState)
                                                oExportTask.ExportResultFileName = "AdvExportPlanningHolidaysOnly"
                                                oResultFile = oScheduleExport.ExportHolidays(tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, False, iIDSchedule)
                                                oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                                oDataLinkState = oScheduleExport.State
                                            Case "presence"
                                                Dim employeeConf As String() = oExportTask.ExportParameters.EmployeesSelected.Split("@")
                                                Dim strListEmployees As String = employeeConf(0) & "¬" & employeeConf(2) & "¬" & employeeConf(3)
                                                Dim oScheduleExport As New roFixesExportShiftsAndAttendance(oDataLinkState)
                                                oExportTask.ExportResultFileName = "AdvExportPlanningAttendance"
                                                oResultFile = oScheduleExport.ExportShiftsWithAttendance(strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, iIDSchedule)
                                                oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                                oDataLinkState = oScheduleExport.State
                                        End Select
                                    Else
                                        Dim employeeConf As String() = oExportTask.ExportParameters.EmployeesSelected.Split("@")
                                        Dim strListEmployees As String = employeeConf(0) & "¬" & employeeConf(2) & "¬" & employeeConf(3)
                                        Dim oScheduleExport As New roCalendarExport(oDataLinkState)
                                        oExportTask.ExportResultFileName = "AdvExportPlanningV2"
                                        oResultFile = oScheduleExport.ExportProfilePlanningV2EXCEL(oExportTask.ExportGuide.Id, strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, oExcelProfileBytes, iIDSchedule)
                                        oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        oDataLinkState = oScheduleExport.State
                                    End If
                                Case DataLinkExportProfile.Requests
                                    Dim oRequestExporter As New roFixedExportRequests(oDataLinkState)
                                    oExportTask.ExportResultFileName = "AdvExportExportRequests"
                                    oResultFile = oRequestExporter.ExportRequests(tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, iIDSchedule)
                                    oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                    oDataLinkState = oRequestExporter.State
                                Case DataLinkExportProfile.Absences
                                    If String.IsNullOrEmpty(oExportGuideTemplate.TemplateFile) Then
                                        Dim oAbsenceExporter As New roFixedExportAbsences(oDataLinkState)
                                        oExportTask.ExportResultFileName = "AdvExportExportAbsences"
                                        oResultFile = oAbsenceExporter.ExportAbsences(tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, iIDSchedule)
                                        oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        oDataLinkState = oAbsenceExporter.State
                                    Else

                                        oExportTask.ExportResultFileName = "AdvExportAbsences"
                                        Dim oAbsenceExporter As New roAbsenceExport(oDataLinkState)
                                        Select Case fileType
                                            Case DataLinkResultFileType.Excel
                                                ' Exportación avanzada de fichajes
                                                oResultFile = oAbsenceExporter.ExportProfileProlonguedAbsencesEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                                oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                            Case DataLinkResultFileType.Text
                                                oResultFile = oAbsenceExporter.ExportProfileProlonguedAbsencesASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                                oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                        End Select
                                        oDataLinkState = oAbsenceExporter.State
                                    End If
                                Case DataLinkExportProfile.Tasks
                                    oExportTask.ExportResultFileName = "AdvExportTasks"
                                    Dim oTaskExporter As New roTaskExport(oDataLinkState)
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            oResultFile = oTaskExporter.ExportProfileTasksEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oTaskExporter.ExportProfileTasksASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oTaskExporter.State
                                Case DataLinkExportProfile.CostCenters
                                    oExportTask.ExportResultFileName = "AdvExportBusinessCenters"
                                    Dim oCostCenterExport As New roCostCenterExport(oDataLinkState)
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            oResultFile = oCostCenterExport.ExportProfileCostCentersEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, String.Empty, String.Empty, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oCostCenterExport.ExportProfileCostCentersASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, String.Empty, String.Empty, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oCostCenterExport.State
                                Case DataLinkExportProfile.DinningRoom
                                    oExportTask.ExportResultFileName = "AdvExportDinners"
                                    Dim oDiningExport As New roDinningExport(oDataLinkState)
                                    Select Case fileType
                                        Case DataLinkResultFileType.Excel
                                            oResultFile = oDiningExport.ExportProfileDinnersEXCEL(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, False, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                        Case DataLinkResultFileType.Text
                                            oResultFile = oDiningExport.ExportProfileDinnersASCII(oExportTask.ExportGuide.Id, tmpEmployeeFilterTable, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportGuideTemplate.TemplateFile, oExportTask.ExportParameters.Separator, 0, ,,,,,, oExcelProfileBytes, iIDSchedule)
                                            oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "txt")
                                    End Select
                                    oDataLinkState = oDiningExport.State
                            End Select

                        End If
                    Else
                        ' Sin empleados, seguramente por permisos
                        oDataLinkState.Result = DataLinkResultEnum.NoEmployeeSuitable
                    End If


                    ' Si se ha ejecutado una exportación de saldos y en caso necesario,
                    ' asignamos la fecha de fin de la exportación como fecha de cierre de los empleados exportados
                    If oDataLinkState.Result = DataLinkResultEnum.NoError AndAlso oResultFile IsNot Nothing AndAlso
                        oExportTask.ExportParameters.LockDataAfterExport AndAlso oExportTask.ExportGuide.ProfileType = 1 Then
                        Dim oEmployeeState As New VTEmployees.Employee.roEmployeeState(Me.oState.IDPassport)
                        Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable($"@SELECT# * FROM #{tmpEmployeeFilterTable}")

                        For Each oRow As DataRow In tb.Rows
                            Dim oEmployee As VTEmployees.Employee.roEmployee = VTEmployees.Employee.roEmployee.GetEmployee(roTypes.Any2Integer(oRow("id")), oEmployeeState, False)
                            If oEmployee IsNot Nothing Then
                                roBusinessSupport.SaveEmployeeLockDate(oEmployee.ID, oExportTask.ExportParameters.EndDate, True, oEmployeeState,, True)
                            End If
                        Next

                        Extensions.roConnector.InitTask(Robotics.Base.DTOs.TasksType.MOVES)
                    End If



                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDataLinkExportManager::ExecuteExport:Exception ", ex)
                    oDataLinkState.Result = DataLinkResultEnum.Exception
                    oDataLinkState.ErrorText = ex.Message
                End Try
            Else
                If oExportTask Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportTask
                ElseIf oExportTask.ExportGuide Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportGuide
                ElseIf oExportTask.ExportParameters Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportParameters
                Else
                    oDataLinkState.Result = DataLinkResultEnum.NoPassportSpecified
                End If
            End If

            Return oResultFile
        End Function

        Public Function ExecuteCustomExport(ByRef oExportTask As roDatalinkExportTask, ByRef oDataLinkState As roDataLinkState) As Byte()
            Dim oResultFile As Byte() = Nothing
            Dim bTemplateRequired As Boolean = True

            If oExportTask IsNot Nothing AndAlso oExportTask.ExportGuide IsNot Nothing AndAlso oExportTask.ExportParameters IsNot Nothing AndAlso oExportTask.ExportParameters.IdPassport > 0 Then

                Try
                    oDataLinkState.Result = DataLinkResultEnum.NoError
                    ' Recuperamos plantilla. No todas las guías de exportación las usan ...
                    Dim oExportGuideTemplate As roDatalinkExportGuideTemplate = Nothing
                    Dim oExcelProfileBytes As Byte() = Nothing

                    ' Recuperamos empleados a tratar
                    Dim strListEmployees As String = Nothing
                    Dim strEmployeefilter As String = Nothing
                    Dim conf As String() = Nothing
                    Dim lstEmployees As List(Of Integer) = Nothing
                    Dim fileType As Integer = roTypes.Any2Integer(oExportTask.ExportParameters.FileType)
                    Dim iIDSchedule As Integer = oExportTask.ExportParameters.IdSchedule
                    LoadEmployeeFilter(oExportTask, strListEmployees, strEmployeefilter, conf, lstEmployees)

                    If strEmployeefilter <> String.Empty Then
                        strListEmployees = strEmployeefilter
                        Dim idTemplate As Integer = oExportTask.ExportParameters.IdTemplate

                        oExportGuideTemplate = oExportTask.ExportGuide.Templates.ToList.Find(Function(x) x.ID = idTemplate)

                        ' Cargo plantilla
                        If oExportGuideTemplate IsNot Nothing AndAlso oExportGuideTemplate.TemplateFile IsNot Nothing AndAlso oExportGuideTemplate.TemplateFile.Length > 0 Then
                            ' Veo si se trata de una plantilla exclusiva del cliente (las de exportación de calendario), o común (el resto)
                            oExcelProfileBytes = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(oExportGuideTemplate.TemplateFile, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink, False)
                            If oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0 Then
                                ' Si no encontré plantillas propias del cliente, busco las comunes
                                oExcelProfileBytes = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure(oExportGuideTemplate.TemplateFile, DTOs.roLiveQueueTypes.datalink, False)
                            End If
                        Else
                            bTemplateRequired = False
                        End If

                        If bTemplateRequired AndAlso (oExcelProfileBytes Is Nothing OrElse oExcelProfileBytes.Length = 0) Then
                            oDataLinkState.Result = DataLinkResultEnum.UnableToRecoverExcelProfile
                        Else
                            Select Case oExportTask.ExportGuide.ProfileType
                                Case DataLinkExportProfile.Argal
                                    oResultFile = roCustomArgal.ExportProfileArgalASCII(strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState)
                                    oExportTask.ExportResultFileName = "ARGALZip.zip"
                                Case DataLinkExportProfile.CustomAttendanceProductivity
                                    strListEmployees = conf(0) & "¬" & conf(2) & "¬" & conf(3)
                                    oExportTask.ExportResultFileName = "EXCELExportAttendance.xlsx"
                                    oResultFile = roCustomTDC.EXCELExportAttendance(strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, oDataLinkState)
                                    oExportTask.ExportResultFileName = roDataLinkManager.GetUniqueFileName(oExportTask.ExportResultFileName, "xlsx")
                                Case DataLinkExportProfile.CustomIberper
                                    oResultFile = roCustomIberper.ASCIIExportIberper(strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportParameters.Separator, oExportTask.ExportGuide.Id, oDataLinkState)
                                    oExportTask.ExportResultFileName = "IbermaticaZip.zip"
                                Case DataLinkExportProfile.IvecoMDOValladolidD
                                    ' FIAT (MDO Valladolid diaria)
                                    If oExportTask.ExportParameters.IsProgrammed AndAlso oExportTask.ExportParameters.EndDate > Now Then
                                        oExportTask.ExportParameters.EndDate = Now.AddDays(-1)
                                    End If
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOValladolid(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, True)
                                    oExportTask.ExportResultFileName = "MFENH002_VA.txt"
                                Case DataLinkExportProfile.IvecoMDOValladolidM
                                    ' FIAT (MDO Valladolid Mensual)
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOValladolid(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, False)
                                    oExportTask.ExportResultFileName = "MFENH003_VA.txt"
                                Case DataLinkExportProfile.IvecoMDOMadridD
                                    If oExportTask.ExportParameters.IsProgrammed AndAlso oExportTask.ExportParameters.EndDate > Now Then
                                        oExportTask.ExportParameters.EndDate = Now.AddDays(-1)
                                    End If
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOMadridv2(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, True, False, False)
                                    oExportTask.ExportResultFileName = "MFENH002_MA.txt"
                                Case DataLinkExportProfile.IvecoWorkanalysisHoursEmployee
                                    ' FIAT WorkAnalysis - Horas trabajadas diarias por empleado
                                    oResultFile = roCustomFiat.EXCELExportFIATWAHT(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "EXCELExportFIATWAHTD.xlsx"
                                Case DataLinkExportProfile.IvecoPlusPresenciaMadrid, DataLinkExportProfile.IvecoPlusPresenciaValladolid
                                    oResultFile = roCustomFiat.EXCELExportFIATPlusPresencia(strListEmployees, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportParameters.Separator, oExcelProfileBytes, oDataLinkState)
                                    oExportTask.ExportResultFileName = "EXCELExportFIATPLUS.xlsx"
                                Case DataLinkExportProfile.IvecoWorkanalysisReportM
                                    ' FIAT WorkAnalysis - Report Mensual
                                    oResultFile = roCustomFiat.EXCELExportFIATWARM(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "EXCELExportFIATWARM.xlsx"
                                Case DataLinkExportProfile.IvecoMDOLogisticMadridD
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOMadridv2(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, True, True, False)
                                    oExportTask.ExportResultFileName = "PMWMA_CFTE.IV51.txt"
                                Case DataLinkExportProfile.IvecoMDOMadridM
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOMadridv2(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, False, False, False)
                                    oExportTask.ExportResultFileName = "MFENH003_MA.txt"
                                Case DataLinkExportProfile.IvecoJobHistory
                                    oResultFile = roCustomFiat.ExcelExportFIATPlacesHistory(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "ExcelExportPlacesHistory.xlsx"
                                Case DataLinkExportProfile.IvecoMDOLogisticMadridM
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOMadridv2(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, False, True, False)
                                    oExportTask.ExportResultFileName = "PMWMA_MFTE.IV51.txt"
                                Case DataLinkExportProfile.IvecoMDOMadridMWorkanalysis
                                    oResultFile = roCustomFiat.ASCIIExportFIATMDOMadridv2(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, False, False, True)
                                    oExportTask.ExportResultFileName = "MFENH004_MA.txt"
                                Case DataLinkExportProfile.IvecoWorkanalysisHoursSection
                                    ' FIAT WorkAnalysis - Horas trabajadas diarias por sección
                                    oResultFile = roCustomFiat.EXCELExportFIATWAHS(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "EXCELExportFIATWAHTS.xlsx"
                                Case DataLinkExportProfile.IvecoConguallo
                                    ' FIAT Conguallo
                                    oResultFile = roCustomFiat.EXCELExportFIATConguallo(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "MDOConguallo.xlsx"
                                Case DataLinkExportProfile.IvecoCategoryChange
                                    oResultFile = roCustomFiat.ExcelExportCategory(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, oExportTask.ExportParameters.Separator, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "ExcelExportCategory.xlsx"
                                Case DataLinkExportProfile.LivenDailyCauses
                                    ' LIVEN. Exportación de justificaciones diarias
                                    If oExportTask.ExportParameters.IsProgrammed Then
                                        Dim oCurrentDate As Date = Now.Date
                                        Dim oEndDate As Date
                                        ' Busco en oExportTask.ExportGuide.Schedules el elemecnto con ID = oExportTask.ExportParameters.IdSchedule
                                        Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                        If exportTaskSchedule IsNot Nothing Then
                                            oCurrentDate = exportTaskSchedule.NextExecutionDate.Date
                                        End If

                                        ' En automático se exportan los n meses completos anteriores a la fecha actual
                                        Dim oBeginDate As Date = New Date(oCurrentDate.Year, oCurrentDate.Month, 1, 0, 0, 0, 0, DateTimeKind.Local)
                                        oEndDate = oBeginDate.AddDays(-1)
                                        Dim iMonths As Integer = roTypes.Any2Integer(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "LIVEN.TotalMonthsInAuto"))
                                        If iMonths = 0 Then iMonths = 1
                                        oBeginDate = oBeginDate.AddMonths(-1 * iMonths)

                                        oExportTask.ExportParameters.BeginDate = oBeginDate
                                        oExportTask.ExportParameters.EndDate = oEndDate
                                    End If

                                    oResultFile = roCustomLiven.EXCELExportDailyCausesLIVEN(oExportTask.ExportGuide.Id, strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, iIDSchedule)
                                    oExportTask.ExportResultFileName = "ExcelExportLivenDailyCauses.xlsx"

                                Case DataLinkExportProfile.RosRocaDynamics
                                    ' ROS ROCA. Exportación a Dynamics
                                    If oExportTask.ExportParameters.IsProgrammed Then
                                        Dim oCurrentDate As Date = Now.Date

                                        ' Busco en oExportTask.ExportGuide.Schedules el elemecnto con ID = oExportTask.ExportParameters.IdSchedule
                                        Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                        If exportTaskSchedule IsNot Nothing Then
                                            oCurrentDate = exportTaskSchedule.NextExecutionDate.Date
                                        End If
                                        ' En automático se exporta el dia anterior a la ejecución
                                        Dim oBeginDate As Date = oCurrentDate
                                        Dim oEndDate As Date

                                        oBeginDate = oBeginDate.AddDays(-1)
                                        oEndDate = oBeginDate

                                        oExportTask.ExportParameters.BeginDate = oBeginDate
                                        oExportTask.ExportParameters.EndDate = oEndDate
                                    End If

                                    oResultFile = roCustomRosRoca.EXCELExportDynamics(oExportTask.ExportGuide.Id, strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oDataLinkState, iIDSchedule)
                                    oExportTask.ExportResultFileName = "ExcelExportRosRocaDynamics.xlsx"

                                Case DataLinkExportProfile.TisvolPrimas
                                    If oExportTask.ExportParameters.IsProgrammed Then
                                        Dim oCurrentDate As Date = Now.Date
                                        Dim oEndDate As Date

                                        Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                        If exportTaskSchedule IsNot Nothing Then
                                            oCurrentDate = exportTaskSchedule.NextExecutionDate.Date
                                        End If

                                        Dim oBeginDate As Date = roTypes.CreateDateTime(oCurrentDate.Year, oCurrentDate.Month, 1, 0, 0, 0, 0)
                                        oEndDate = oBeginDate.AddDays(-1)
                                        oBeginDate = roTypes.CreateDateTime(oEndDate.Year, oEndDate.Month, 1)

                                        oExportTask.ExportParameters.BeginDate = oBeginDate
                                        oExportTask.ExportParameters.EndDate = oEndDate
                                    End If
                                    oResultFile = roCustomTisvol.EXCELExportPrimasEstimate(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, False, oExportTask.ExportGuide.Id, oDataLinkState, oExportGuideTemplate.TemplateFile)
                                    oExportTask.ExportResultFileName = "EXCELExportPrimasEstimate.xlsx"
                                Case DataLinkExportProfile.TisvolPrimasSummary
                                    If oExportTask.ExportParameters.IsProgrammed Then
                                        Dim oCurrentDate As Date = Now.Date
                                        Dim oEndDate As Date

                                        Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                        If exportTaskSchedule IsNot Nothing Then
                                            oCurrentDate = exportTaskSchedule.NextExecutionDate.Date
                                        End If

                                        Dim oBeginDate As Date = roTypes.CreateDateTime(oCurrentDate.Year, oCurrentDate.Month, 1, 0, 0, 0, 0)
                                        oEndDate = oBeginDate.AddDays(-1)
                                        oBeginDate = roTypes.CreateDateTime(oEndDate.Year, oEndDate.Month, 1)

                                        oExportTask.ExportParameters.BeginDate = oBeginDate
                                        oExportTask.ExportParameters.EndDate = oEndDate
                                    End If
                                    oResultFile = roCustomTisvol.EXCELExportPrimasEstimate(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, False, oExportTask.ExportGuide.Id, oDataLinkState, oExportGuideTemplate.TemplateFile, True)
                                    oExportTask.ExportResultFileName = "EXCELExportPrimasEstimateSummary.xlsx"
                                Case DataLinkExportProfile.TisvolPunches
                                    If oExportTask.ExportParameters.IsProgrammed Then
                                        Dim oCurrentDate As Date = Now.Date

                                        Dim exportTaskSchedule As roDatalinkExportSchedule = oExportTask.ExportGuide.Schedules.ToList.Find(Function(x) x.Id = iIDSchedule)
                                        If exportTaskSchedule IsNot Nothing Then
                                            oCurrentDate = exportTaskSchedule.NextExecutionDate.Date
                                        End If

                                        Dim oEndDate As Date = oCurrentDate
                                        Dim oBeginDate As Date = roTypes.CreateDateTime(oEndDate.Year, oEndDate.Month, oEndDate.Day - 1, 0, 0, 0)
                                        Dim oEndDateExp As Date = roTypes.CreateDateTime(oEndDate.Year, oEndDate.Month, oEndDate.Day - 1, 23, 59, 59)

                                        oExportTask.ExportParameters.BeginDate = oBeginDate
                                        oExportTask.ExportParameters.EndDate = oEndDateExp
                                    End If

                                    oResultFile = roCustomTisvol.ASCIIExportPunchesTisvol(strEmployeefilter, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate, oExportTask.ExportGuide.Id, oExportTask.ExportParameters.Separator, oDataLinkState)
                                    oExportTask.ExportResultFileName = "ASCIIExportPunchesTisvol.txt"
                            End Select

                        End If
                    Else
                        ' Sin empleados, seguramente por permisos
                        oDataLinkState.Result = DataLinkResultEnum.NoEmployeeSuitable
                    End If

                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDataLinkExportManager::ExecuteExport:Exception ", ex)
                    oDataLinkState.Result = DataLinkResultEnum.Exception
                    oDataLinkState.ErrorText = ex.Message
                End Try
            Else
                If oExportTask Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportTask
                ElseIf oExportTask.ExportGuide Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportGuide
                ElseIf oExportTask.ExportParameters Is Nothing Then
                    oDataLinkState.Result = DataLinkResultEnum.NoExportParameters
                Else
                    oDataLinkState.Result = DataLinkResultEnum.NoPassportSpecified
                End If
            End If

            Return oResultFile
        End Function



        Public Function GetExportTemplateBytes(ByVal idExportGuide As Integer, ByVal idTemplate As Integer) As Byte()
            Dim bTemplateContent As Byte() = {}
            Try

                Dim oExportGuide As roDatalinkExportGuide = Me.LoadGuideById(idExportGuide)

                If oExportGuide IsNot Nothing Then
                    Dim oexporGuideTemplate As roDatalinkExportGuideTemplate = oExportGuide.Templates.ToList().Find(Function(x) x.ID = idTemplate)

                    If oexporGuideTemplate IsNot Nothing AndAlso oexporGuideTemplate.TemplateFile IsNot Nothing AndAlso oexporGuideTemplate.TemplateFile.Length > 0 Then
                        ' Veo si se trata de una plantilla exclusiva del cliente (las de exportación de calendario), o común (el resto)
                        bTemplateContent = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(oexporGuideTemplate.TemplateFile, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink)
                        If bTemplateContent Is Nothing OrElse bTemplateContent.Length = 0 Then
                            ' Si no encontré plantillas propias del cliente, busco las comunes
                            bTemplateContent = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure(oexporGuideTemplate.TemplateFile, DTOs.roLiveQueueTypes.datalink)
                        End If
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roDatalinkExportManager::GetExportTemplateBytes::", ex)
            End Try

            Return bTemplateContent
        End Function

        Public Function SaveExportTemplateBytes(ByVal bTemplateContent As Byte(), ByVal idExportGuide As Integer, ByVal idTemplate As Integer) As Boolean
            Dim bRet As Boolean = False
            Try

                Dim oExportGuide As roDatalinkExportGuide = Me.LoadGuideById(idExportGuide)

                If oExportGuide IsNot Nothing Then
                    Dim oexporGuideTemplate As roDatalinkExportGuideTemplate = oExportGuide.Templates.ToList().Find(Function(x) x.ID = idTemplate)

                    If oexporGuideTemplate IsNot Nothing AndAlso oexporGuideTemplate.TemplateFile IsNot Nothing AndAlso oexporGuideTemplate.TemplateFile.Length > 0 AndAlso bTemplateContent IsNot Nothing AndAlso bTemplateContent.Length > 0 Then
                        ' Veo si se trata de una plantilla exclusiva del cliente (las de exportación de calendario), o común (el resto)
                        bRet = Azure.RoAzureSupport.SaveFileOnCompanyContainer(New System.IO.MemoryStream(bTemplateContent), oexporGuideTemplate.TemplateFile, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink, True)
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roDatalinkExportManager::SaveExportTemplateBytes::", ex)
            End Try

            Return bRet
        End Function

        Public Function DuplicateExportTemplateBytes(ByVal bTemplateContent As Byte(), ByVal idExportGuide As Integer, ByVal idTemplate As Integer, ByVal newTemplateName As String) As Boolean
            Dim bRet As Boolean = False
            Try

                Dim oExportGuide As roDatalinkExportGuide = Me.LoadGuideById(idExportGuide)

                If oExportGuide IsNot Nothing Then
                    Dim oexporGuideTemplate As roDatalinkExportGuideTemplate = oExportGuide.Templates.ToList().Find(Function(x) x.ID = idTemplate)
                    If oexporGuideTemplate IsNot Nothing Then

                        Dim oldFileName As String() = oexporGuideTemplate.TemplateFile.Split(".")

                        Dim oNewTemplate As New roDatalinkExportGuideTemplate With {
                            .ID = -1,
                            .IdParentGuide = oexporGuideTemplate.IdParentGuide,
                            .IsDefault = oexporGuideTemplate.IsDefault,
                            .Name = newTemplateName & "*",
                            .IdName = newTemplateName & "*",
                            .PostProcessScript = oexporGuideTemplate.PostProcessScript,
                            .TemplateFile = oldFileName(0) & "." & DateTime.Now.ToString("yyyyMMddHHmmss") & "." & oldFileName(oldFileName.Length - 1)
                            }

                        Dim oTemplateManager As New roDataLinkExportTemplateManager

                        bRet = oTemplateManager.Save(oNewTemplate)
                        If bRet AndAlso oNewTemplate.TemplateFile IsNot Nothing AndAlso oNewTemplate.TemplateFile.Length > 0 AndAlso bTemplateContent IsNot Nothing AndAlso bTemplateContent.Length > 0 Then
                            ' Veo si se trata de una plantilla exclusiva del cliente (las de exportación de calendario), o común (el resto)
                            bRet = Azure.RoAzureSupport.SaveFileOnCompanyContainer(New System.IO.MemoryStream(bTemplateContent), oNewTemplate.TemplateFile, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink, True)
                        End If
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roDatalinkExportManager::DuplicateExportTemplateBytes::", ex)
            End Try

            Return bRet
        End Function

#End Region

#Region "Helper"

        Private Shared Function GetEmployeeFilterOnTemporalTableName(oExportTask As roDatalinkExportTask) As String
            Dim conf As String() = oExportTask.ExportParameters.EmployeesSelected.Split("@")

            Dim strSelectorEmployees As String = conf(0)
            Dim strFeature As String = conf(1)
            Dim strFilter As String = conf(2)
            Dim strFilterUser As String = conf(3)

            Dim oParam As New AdvancedParameter.roAdvancedParameter("SeeEmployeeDataAtDate", New AdvancedParameter.roAdvancedParameterState())
            Dim bolEmployeesAtDate As Boolean = If(String.IsNullOrEmpty(oParam.Value), True, Any2Boolean(oParam.Value))

            If oExportTask.ExportParameters.BeginDate = #12:00:00 AM# OrElse oExportTask.ExportParameters.EndDate = #12:00:00 AM# Then
                bolEmployeesAtDate = False
            End If

            Dim vtSelector As New roSelectorFilter() With {
                .ComposeFilter = strSelectorEmployees,
                .Filters = strFilter,
                .UserFields = strFilterUser,
                .Operation = "OR",
                .ComposeMode = "Custom"
            }




            If bolEmployeesAtDate Then
                Dim vtSelectorBehaviour As New roSelectorFilterContext() With {
                                                                                        .IdPassport = oExportTask.ExportParameters.IdPassport,
                                                                                        .Feature = strFeature,
                                                                                        .FeatureType = "U",
                                                                                        .RequieredPermission = Permission.Read,
                                                                                        .AddOnlyDirectEmployees = False,
                                                                                        .DateInf = oExportTask.ExportParameters.BeginDate,
                                                                                        .DateSup = oExportTask.ExportParameters.EndDate
                                                                                    }

                Return roSelector.GetEmployeeListByContractOnTemporalTableName(vtSelector, vtSelectorBehaviour)
            Else
                Dim vtSelectorBehaviour As New roSelectorFilterContext() With {
                                                                                        .IdPassport = oExportTask.ExportParameters.IdPassport,
                                                                                        .Feature = strFeature,
                                                                                        .FeatureType = "U",
                                                                                        .RequieredPermission = Permission.Read,
                                                                                        .AddOnlyDirectEmployees = False,
                                                                                        .DateInf = Nothing,
                                                                                        .DateSup = Nothing
                                                                                    }


                Return roSelector.GetEmployeeListOnTemporalTableName(vtSelector, vtSelectorBehaviour)
            End If


        End Function



        Private Shared Sub LoadEmployeeFilter(oExportTask As roDatalinkExportTask, ByRef strListEmployees As String, ByRef strEmployeefilter As String, ByRef conf() As String, ByRef lstEmployees As List(Of Integer))
            strListEmployees = ""
            strEmployeefilter = String.Empty
            conf = oExportTask.ExportParameters.EmployeesSelected.Split("@")

            Dim strSelectorEmployees As String = conf(0)
            Dim strFeature As String = conf(1)
            Dim strFilter As String = conf(2)
            Dim strFilterUser As String = conf(3)

            Dim oParam As New AdvancedParameter.roAdvancedParameter("SeeEmployeeDataAtDate", New AdvancedParameter.roAdvancedParameterState())
            Dim bolEmployeesAtDate As Boolean = If(String.IsNullOrEmpty(oParam.Value), True, Any2Boolean(oParam.Value))

            If oExportTask.ExportParameters.BeginDate = #12:00:00 AM# OrElse oExportTask.ExportParameters.EndDate = #12:00:00 AM# Then
                bolEmployeesAtDate = False
            End If

            If bolEmployeesAtDate Then
                lstEmployees = roSelector.GetEmployeeListByContract(oExportTask.ExportParameters.IdPassport, strFeature, "U", Permission.Read,
                                    strSelectorEmployees, strFilterUser, False, oExportTask.ExportParameters.BeginDate, oExportTask.ExportParameters.EndDate)
            Else
                lstEmployees = roSelector.GetEmployeeList(oExportTask.ExportParameters.IdPassport, strFeature, "U", Permission.Read,
                                    strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)
            End If

            For Each iID As Integer In lstEmployees
                strEmployeefilter = strEmployeefilter & iID.ToString & ","
            Next
            If strEmployeefilter.Length > 0 Then strEmployeefilter = strEmployeefilter.Substring(0, strEmployeefilter.Length - 1)
        End Sub


        Private Shared Function IsStandardTemplate(ByVal exportType As DataLinkExportProfile) As Boolean

            Dim standardTypes As DataLinkExportProfile() = {DataLinkExportProfile.Accruals,
                                                                DataLinkExportProfile.ProgrammedAbsenceLegacy,
                                                                DataLinkExportProfile.DinningRoomLegacy,
                                                                DataLinkExportProfile.PunchesLegacy,
                                                                DataLinkExportProfile.TasksLegacy,
                                                                DataLinkExportProfile.Employees,
                                                                DataLinkExportProfile.CostCentersLegacy,
                                                                DataLinkExportProfile.Punches,
                                                                DataLinkExportProfile.DailyCauses,
                                                                DataLinkExportProfile.EmployeeTasksLegacy,
                                                                DataLinkExportProfile.PlanningLegacy,
                                                                DataLinkExportProfile.Planning,
                                                                DataLinkExportProfile.Requests,
                                                                DataLinkExportProfile.Absences,
                                                                DataLinkExportProfile.Tasks,
                                                                DataLinkExportProfile.CostCenters,
                                                                DataLinkExportProfile.DinningRoom}

            Return standardTypes.Contains(exportType)
        End Function
#End Region

    End Class

End Namespace