Imports System.Threading
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDataLink.DataLink
Imports Robotics.ExternalSystems
Imports Robotics.ExternalSystems.Suprema
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes

Namespace DatalinkServer

    Public Class DatalinkTaskExecution

#Region "Declarations - Constructor"

        Public Sub New()
        End Sub

#End Region

#Region "Properties"

        Private oLog As New roLog("DatalinkQueueClient")

#End Region

#Region "Methods"

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            '
            ' Procesa guía
            '
            Dim bolRet As Boolean = False
            Dim taskDescription As String = String.Empty
            Try

                Dim sType As String = oTask.Action.ToUpper

                Dim oDatalinkState As New roDataLinkGuideState(oTask.IDPassport)
                Select Case sType
                    Case roLiveTaskTypes.GenerateDatalinkTasks.ToString.ToUpper
                        Dim oDatalinkManager As New VTDataLink.DataLink.roDataLinkManager(New VTDataLink.DataLink.roDataLinkGuideState(-1))
                        bolRet = oDatalinkManager.ExecuteScheduledTasks()


                    Case roLiveTaskTypes.Import.ToString.ToUpper
                        Dim oImportManager As New roDataLinkImportManager(oDatalinkState)

                        Dim oImportGuide As roDatalinkImportGuide = Nothing
                        Dim oImportTaskResultState As New roDataLinkState
                        Dim oImportTask As New roDatalinkImportTask
                        Dim oImportParameters As New roDatalinkImportParameters

                        oImportGuide = oImportManager.LoadById(Any2String(oTask.Parameters("IDImport")))
                        oImportParameters.BeginDate = DateTime.Parse(oTask.Parameters("ScheduleBeginDate"))
                        oImportParameters.EndDate = DateTime.Parse(oTask.Parameters("ScheduleEndDate"))
                        oImportParameters.EmployeesSelected = roTypes.Any2String(oTask.Parameters("EmployeesSelected"))
                        oImportParameters.CalendarCopyMainShifts = roTypes.Any2Boolean(oTask.Parameters("CopyMainShifts"))
                        oImportParameters.CalendarExcelIsTemplate = roTypes.Any2Boolean(oTask.Parameters("ExcelIsTemplate"))
                        oImportParameters.CalendarCopyHolidays = roTypes.Any2Boolean(oTask.Parameters("CopyHolidays"))
                        oImportParameters.CalendarKeepHolidays = roTypes.Any2Boolean(oTask.Parameters("KeepHolidays"))
                        oImportParameters.CalendarKeepLockedDays = roTypes.Any2Boolean(oTask.Parameters("KeepLockedDays"))
                        oImportParameters.Separator = roTypes.Any2String(oTask.Parameters("Separator"))
                        oImportParameters.FileType = roTypes.Any2Integer(oTask.Parameters("FileType"))
                        oImportParameters.IdPassport = oTask.IDPassport
                        oImportParameters.OriginalFileName = oTask.Parameters("oFileOrig")
                        oImportParameters.IsProgrammed = Any2Boolean(oTask.Parameters("IsBackground"))
                        oImportParameters.IdTemplate = oTask.Parameters("IDImportTemplate")

                        ' Dependiendo de si es automática o manual, recojo el fichero de schema (si existe)
                        If oImportParameters.IsProgrammed AndAlso oImportGuide IsNot Nothing AndAlso oImportGuide.FormatFilePath IsNot Nothing Then
                            oImportParameters.SchemaFileName = oImportGuide.FormatFilePath
                        Else
                            oImportParameters.SchemaFileName = oTask.Parameters("oFileSchema")
                        End If

                        oImportTask.ImportGuide = oImportGuide
                        oImportTask.ImportParameters = oImportParameters

                        oImportTaskResultState = oImportManager.ExecuteImport(oImportTask)

                        If oImportParameters.IsProgrammed Then
                            Azure.RoAzureSupport.RenameFileInCompanyContainer(oImportTask.ImportParameters.OriginalFileName & ".pro", oImportTask.ImportParameters.OriginalFileName.Replace("WIP_", "").Replace("_WIP", "") & "." & Now.ToString("yyyyMMddHHmmssffff") & ".bck", roLiveDatalinkFolders.import.ToString, roLiveQueueTypes.datalink)
                        End If

                        Select Case oImportTaskResultState.Result
                            Case DataLinkResultEnum.NoError
                                bolRet = True
                            Case Else
                                'Si acabo con una excepción, el fichero ya fue renombrado, y por tanto no se volverá a importar ---
                                taskDescription = oImportTaskResultState.ErrorText
                        End Select

                        ' Borro fichero de importación
                        Azure.RoAzureSupport.DeleteFileFromAzure(oImportParameters.OriginalFileName, DTOs.roLiveQueueTypes.datalink)

                    Case roLiveTaskTypes.Export.ToString.ToUpper
                        Dim oExportManager As New roDataLinkExportManager(oDatalinkState)

                        Dim oExportTaskResultState As New roDataLinkState
                        Dim oExportTask As New roDatalinkExportTask
                        Dim oExportGuide As New roDatalinkExportGuide
                        Dim oExportParameters As New roDatalinkExportParameters

                        oExportGuide = oExportManager.LoadGuideById(Any2String(oTask.Parameters("IDExport")))
                        oExportParameters.BeginDate = DateTime.Parse(oTask.Parameters("ScheduleBeginDate"))
                        oExportParameters.EndDate = DateTime.Parse(oTask.Parameters("ScheduleEndDate"))
                        oExportParameters.EmployeesSelected = roTypes.Any2String(oTask.Parameters("EmployeesSelected"))
                        oExportParameters.IdConceptGroup = roTypes.Any2Integer(oTask.Parameters("ConceptGroup"))
                        oExportParameters.LockDataAfterExport = Any2Boolean(oTask.Parameters("ApplyLockDate"))
                        oExportParameters.IdPassport = oTask.IDPassport
                        oExportParameters.IsProgrammed = Any2Boolean(oTask.Parameters("IsBackground"))
                        oExportParameters.Destination = Any2String(oTask.Parameters("Destination"))
                        oExportParameters.FileName = Any2String(oTask.Parameters("ExportFileName"))
                        oExportParameters.IdTemplate = Any2Integer(oTask.Parameters("IDExportTemplate"))
                        oExportParameters.FileType = Any2Integer(oTask.Parameters("FileType"))
                        If oTask.Parameters.Exists("IDSchedule") Then
                            oExportParameters.IdSchedule = Any2Integer(oTask.Parameters("IDSchedule"))
                        Else
                            oExportParameters.IdSchedule = -1
                        End If
                        oExportParameters.Separator = Any2String(oTask.Parameters("Separator"))

                        oExportTask.ExportGuide = oExportGuide
                        oExportTask.ExportParameters = oExportParameters

                        ' Guardamos para que se calcule la próxima fecha de ejecución y así no se ejecute de nuevo
                        bolRet = oExportManager.Save(oExportGuide)
                        ' Ejecutamos la exportación
                        oExportTaskResultState = oExportManager.ExecuteExport(oExportTask)

                        Select Case oExportTaskResultState.Result
                            Case DataLinkResultEnum.NoError
                                bolRet = True
                                taskDescription = Azure.RoAzureSupport.GetCompanyName & "/" & oExportTask.ExportResultFileName
                            Case Else
                                bolRet = False
                                taskDescription = oExportTaskResultState.ErrorText
                        End Select
                    Case roLiveTaskTypes.CTAIMA.ToString.ToUpper
                        Dim ctaimaSystem As CTAIMA.CTAIMASystem = New CTAIMA.CTAIMASystem()
                        Dim oCTAIMATaskResultState As New roDataLinkState
                        Dim apiVersion = New AdvancedParameter.roAdvancedParameter("CTaimaApiVersion", New AdvancedParameter.roAdvancedParameterState).Value
                        Dim isSyncPunchesEnabled = roTypes.Any2Boolean(New AdvancedParameter.roAdvancedParameter("CTaimaSyncPunchesEnabled", New AdvancedParameter.roAdvancedParameterState).Value)
                        ''''''''''''''''''OLD API V1''''''''''''''''''
                        If apiVersion.Trim.Equals(String.Empty) Then
                            oCTAIMATaskResultState = ctaimaSystem.SyncData()
                        End If
                        ''''''''''''''''''''''''''''''''''''''''''''''

                        ''''''''''''''''''NEW API V2''''''''''''''''''
                        If apiVersion.Trim.Equals("1.0") Then
                            oCTAIMATaskResultState = ctaimaSystem.SyncDataRestful()

                            If isSyncPunchesEnabled Then
                                oCTAIMATaskResultState = ctaimaSystem.SendDataRestful()
                            End If
                        End If
                        ''''''''''''''''''''''''''''''''''''''''''''''
                        Select Case oCTAIMATaskResultState.Result
                            Case DataLinkResultEnum.NoError
                                bolRet = True
                        End Select
                    Case roLiveTaskTypes.Suprema.ToString.ToUpper
                        Dim oSupremaTaskResultState As New roDataLinkState
                        Dim oSupremaSystem As SupremaSystem = New SupremaSystem
                        If oSupremaSystem.IsEnabled() Then
                            oSupremaTaskResultState = oSupremaSystem.SyncData
                        End If

                        Select Case oSupremaTaskResultState.Result
                            Case DataLinkResultEnum.NoError
                                bolRet = True
                        End Select


                End Select
            Catch Ex As Exception
                roLog.GetInstance.logMessage(roLog.EventType.roError, "ServerNet::ExecuteTask::", Ex)
            End Try

            Return New BaseTaskResult With {.Result = bolRet, .Description = taskDescription}
        End Function

#End Region


    End Class

End Namespace