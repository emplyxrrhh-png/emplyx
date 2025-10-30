Imports System.Data.Common
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roDataLinkImportManager
        Private oState As roDataLinkGuideState = Nothing

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

        Public Function LoadById(iID As Integer, Optional ByVal bAudit As Boolean = False) As roDatalinkImportGuide
            Dim eConcept As roDatalinkConcept

            Try
                Dim strSql = "@SELECT# Concept from ImportGuides where Version=2 and ID=" & iID.ToString

                Dim strConcept As String = String.Empty
                strConcept = roTypes.Any2String(ExecuteScalar(strSql))
                eConcept = System.Enum.Parse(eConcept.GetType, strConcept, True)
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkImportManager::LoadById")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkImportManager::LoadById")
            End Try

            Return LoadGuide(eConcept, bAudit)
        End Function

        Public Function LoadGuide(ByVal eConcept As roDatalinkConcept, Optional ByVal bAudit As Boolean = False) As roDatalinkImportGuide

            Dim oGuide As roDatalinkImportGuide = Nothing

            Try
                Dim strSql = "@SELECT# ImportGuides.* from ImportGuides "
                strSql = strSql & " INNER JOIN sysrovwSecurity_PermissionOverFeatures pof WITH (NOLOCK) on pof.IDPassport = " & oState.IDPassport.ToString & " And pof.FeatureType = 'U' AND pof.FeatureAlias = RequieredFunctionalities AND pof.Permission > 0 "
                strSql = strSql & " WHERE Version=2 and Concept='" & eConcept.ToString & "'"
                Dim dtImportGuides As New DataTable
                dtImportGuides = CreateDataTable(strSql)
                Dim oTemplateManager As New roDataLinkImportTemplateManager(Me.oState)
                Dim oRow As DataRow
                If dtImportGuides IsNot Nothing AndAlso dtImportGuides.Rows.Count > 0 Then
                    oRow = dtImportGuides.Rows(0)
                    oGuide = New roDatalinkImportGuide
                    oGuide.Id = roTypes.Any2Integer(oRow("ID"))
                    oGuide.ExecutionMode = roTypes.Any2Integer(oRow("Mode"))
                    oGuide.Version = roTypes.Any2Integer(oRow("Version"))
                    oGuide.IsActive = roTypes.Any2Boolean(oRow("Active"))
                    oGuide.IsEnabled = roTypes.Any2Boolean(oRow("Enabled"))
                    oGuide.LastExecutionLog = roTypes.Any2String(oRow("LastLog"))
                    oGuide.CopySource = roTypes.Any2Boolean(oRow("CopySource"))
                    oGuide.IdDefaultTemplate = roTypes.Any2Integer(oRow("IDDefaultTemplate"))
                    oGuide.Templates = oTemplateManager.GetDataLinkImportGuideTemplates(oGuide).ToArray
                    oGuide.RequiredFunctionalities = roTypes.Any2String(oRow("RequieredFunctionalities"))
                    oGuide.FeatureAliasID = roTypes.Any2String(oRow("FeatureAliasID"))
                    oGuide.Location = roTypes.Any2String(oRow("Destination"))
                    oGuide.SourceFilePath = roTypes.Any2String(oRow("SourceFilePath"))
                    oGuide.FormatFilePath = roTypes.Any2String(oRow("FormatFilePath"))
                    oGuide.Separator = roTypes.Any2String(oRow("Separator"))
                    oGuide.Concept = roTypes.Any2String(oRow("Concept"))
                    oGuide.FileType = roTypes.Any2String(oRow("Type"))
                    oGuide.IsCustom = roTypes.Any2Boolean(oRow("IsCustom"))
                    oGuide.Name = roTypes.Any2String(oRow("Name"))
                End If

                ' Auditar lectura
                If bAudit AndAlso oGuide IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDataLink, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDataLinkImportManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkImportManager::Load")
            End Try

            Return oGuide
        End Function

        Public Function Save(ByRef oGuide As roDatalinkImportGuide, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                bolRet = Validate(oGuide)

                If bolRet Then
                    Dim strSQL As String = String.Empty
                    Dim tbAux As New DataTable
                    Dim oRow As DataRow
                    Dim cmd As DbCommand
                    Dim da As DbDataAdapter
                    strSQL = "@SELECT# * from importguides where ID = " & oGuide.Id
                    cmd = CreateCommand(strSQL)
                    da = CreateDataAdapter(cmd, True)

                    da.Fill(tbAux)

                    If Not tbAux Is Nothing AndAlso tbAux.Rows.Count = 1 Then
                        oRow = tbAux.Rows(0)
                        oRow("IDDefaultTemplate") = oGuide.IdDefaultTemplate
                        oRow("Enabled") = oGuide.IsEnabled
                        oRow("Destination") = roTypes.Any2Integer(oGuide.Location)
                        oRow("SourceFilePath") = oGuide.SourceFilePath
                        oRow("FormatFilePath") = oGuide.FormatFilePath
                        oRow("Type") = oGuide.FileType
                        oRow("Separator") = oGuide.Separator
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
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportManager::Save")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oGuide As roDatalinkImportGuide) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = DataLinkGuideResultEnum.NoError

                If oGuide.IsEnabled Then
                    If oGuide.IdDefaultTemplate <= 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.ImportTemplateRequired
                        bolRet = False
                    End If

                    If bolRet AndAlso oGuide.SourceFilePath.Length = 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.SourceFileRequired
                        bolRet = False
                    End If

                    If System.Enum.Parse(GetType(Robotics.Base.DTOs.roDatalinkConcept), oGuide.Concept) = roDatalinkConcept.Absences AndAlso bolRet AndAlso oGuide.FileType = 2 AndAlso oGuide.FormatFilePath.Length = 0 Then
                        Me.oState.Result = DataLinkGuideResultEnum.ImportTemplateRequired
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportManager::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkImportManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function ExecuteImport(ByRef oImportTask As roDatalinkImportTask) As roDataLinkState
            Dim oImportFile As Byte() = Nothing
            Dim oImportSchemaFile As Byte() = Nothing
            Dim oDataLinkState As roDataLinkState = New roDataLinkState(Me.oState.IDPassport)

            If oImportTask IsNot Nothing AndAlso oImportTask.ImportParameters IsNot Nothing AndAlso oImportTask.ImportParameters.IdPassport > 0 Then

                Try
                    oDataLinkState.Result = DataLinkResultEnum.NoError
                    oDataLinkState = New roDataLinkState(Me.oState.IDPassport)

                    ' 1.- Recuperamos empleados a tratar
                    Dim strListEmployees As String = Nothing
                    Dim strEmployeefilter As String = Nothing
                    Dim conf As String() = Nothing
                    Dim lstEmployees As List(Of Integer) = Nothing

                    If oImportTask.ImportParameters.EmployeesSelected IsNot Nothing AndAlso oImportTask.ImportParameters.EmployeesSelected.Length > 0 Then
                        LoadEmployeeFilter(oImportTask, strListEmployees, strEmployeefilter, conf, lstEmployees)
                        strListEmployees = strEmployeefilter
                    End If
                    ' 2.- Plantilla (si la hay)                     

                    Dim idTemplate As Integer = oImportTask.ImportGuide.IdDefaultTemplate
                    If oImportTask.ImportParameters.IdTemplate <> -1 AndAlso oImportTask.ImportParameters.IdTemplate <> 0 Then
                        idTemplate = oImportTask.ImportParameters.IdTemplate
                    End If
                    Dim oImportGuideTemplate = oImportTask.ImportGuide.Templates.ToList.Find(Function(x) x.ID = idTemplate)

                    ' 3.- Fichero de datos
                    ' 3.1.- Tipo: Texto/Excel
                    Dim dataFileType As Integer = 0
                    If oImportTask.ImportParameters.FileType <> 0 Then
                        dataFileType = oImportTask.ImportParameters.FileType
                    Else
                        dataFileType = oImportTask.ImportGuide.FileType
                    End If

                    ' 3.2.- Separador (en caso de ficheros de datos de tipo texto)
                    Dim separator = String.Empty
                    If Not String.IsNullOrEmpty(oImportTask.ImportParameters.Separator) Then
                        separator = oImportTask.ImportParameters.Separator
                    Else
                        separator = oImportTask.ImportGuide.Separator
                    End If

                    ' 3.3.- Datos
                    ' En función de si es programada o no, cojo el fichero del container genérico, o del propio del cliente
                    If oImportTask.ImportParameters.IsProgrammed Then
                        ' Del propio del cliente
                        oImportFile = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(oImportTask.ImportParameters.OriginalFileName, roLiveDatalinkFolders.import.ToString, roLiveQueueTypes.datalink)


                        Dim oPGPEnabled As Boolean = roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "PGP.Enabled"))
                        If oPGPEnabled AndAlso oImportFile IsNot Nothing AndAlso oImportFile.Length > 0 Then
                            Dim bImport As Boolean = False
                            Try
                                Dim oPGPHelper As New PGPCryptographyHelper

                                Dim vtPrivateKey As Byte() = Azure.RoAzureSupport.DownloadFile("certificates/vt-pgpprivate.asc", roLiveQueueTypes.datalink)
                                oImportFile = oPGPHelper.DecryptPGP(oImportFile, vtPrivateKey, RoAzureSupport.GetVisualtimePGPPassphrase())
                                If oImportFile.Length = 0 Then oDataLinkState.Result = DataLinkResultEnum.PGPEncryptionFailed

                            Catch ex As Exception
                                bImport = False
                                oImportFile = Nothing
                                oDataLinkState.Result = DataLinkResultEnum.PGPEncryptionFailed
                                roLog.GetInstance().logMessage(roLog.EventType.roError, "roDataLinkExportManager::ExecuteImport:PGPEncryptionFailed ", ex)
                            End Try
                        End If


                        oImportSchemaFile = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(oImportTask.ImportParameters.SchemaFileName, roLiveDatalinkFolders.import.ToString, roLiveQueueTypes.datalink)
                        ' Renombro conforme está en proceso
                        Azure.RoAzureSupport.RenameFileInCompanyContainer(oImportTask.ImportParameters.OriginalFileName, oImportTask.ImportParameters.OriginalFileName & ".pro", roLiveDatalinkFolders.import.ToString, roLiveQueueTypes.datalink)
                    Else
                        ' Del general, donde se han creado las copias temporales de los ficheros que ha subido el supervisor que está haciendo la importación
                        oImportFile = Azure.RoAzureSupport.GetFileBytesFromAzure(oImportTask.ImportParameters.OriginalFileName, roLiveQueueTypes.datalink)
                        oImportSchemaFile = Azure.RoAzureSupport.GetFileBytesFromAzure(oImportTask.ImportParameters.SchemaFileName, roLiveQueueTypes.datalink)
                    End If

                    If oImportTask.ImportGuide.Id = 21 AndAlso oImportGuideTemplate.TemplateFile.Trim.Length > 0 Then
                        ' Busco si hay plantilla propia de cliente
                        oImportSchemaFile = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(oImportGuideTemplate.TemplateFile, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink)
                        If oImportSchemaFile Is Nothing OrElse oImportSchemaFile.Length = 0 Then
                            ' Si no encontré plantillas propias del cliente, busco las comunes
                            oImportSchemaFile = Azure.RoAzureSupport.GetCommonTemplateBytesFromAzure(oImportGuideTemplate.TemplateFile, DTOs.roLiveQueueTypes.datalink)
                        End If
                    End If

                    If oImportFile IsNot Nothing AndAlso oDataLinkState.Result = DataLinkResultEnum.NoError Then
                        ' Persistimos en ficheros temporales por comodidad
                        Dim oTempFileName As String = String.Empty
                        Dim oTempTemplateName As String = String.Empty
                        oTempFileName = System.IO.Path.GetTempFileName()
                        oTempTemplateName = System.IO.Path.GetTempFileName()
                        System.IO.File.WriteAllBytes(oTempFileName, oImportFile)
                        If oImportSchemaFile IsNot Nothing Then System.IO.File.WriteAllBytes(oTempTemplateName, oImportSchemaFile)

                        Select Case oImportTask.ImportGuide.Id
                            Case DataLinkImportProfile.Employees
                                Select Case oImportGuideTemplate.ID
                                    Case 1
                                        ' Si no utilizamos ninguna plantilla
                                        Select Case dataFileType
                                            Case DataLinkResultFileType.Text
                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting ASCII employees import")
                                                Dim oDataLinkImport As New roEmployeeImport(oTempFileName, oTempTemplateName, oDataLinkState)
                                                oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                                oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                                oDataLinkImport.IsAutomaticProcess = oImportTask.ImportParameters.IsProgrammed
                                                oDataLinkImport.ImportEmployeesAscii(separator)
                                            Case DataLinkResultFileType.Excel
                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting Excel employees import")
                                                Dim oDataLinkImport As New roEmployeeImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                                oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                                oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                                oDataLinkImport.IsAutomaticProcess = oImportTask.ImportParameters.IsProgrammed
                                                oDataLinkImport.ImportEmployeesExcel()
                                        End Select
                                    Case 8
                                        'SAGE -MURANO
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTDataImport::ExecuteImport::Employees SAGE MURANO")
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting ASCII employees import")
                                        Dim oDataLinkImport As New roEmployeeImport(oTempFileName, oTempTemplateName, oDataLinkState)
                                        oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                        oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                        oDataLinkImport.IsAutomaticProcess = oImportTask.ImportParameters.IsProgrammed
                                        oDataLinkImport.ImportEmployeeSageMuranoAscii()
                                End Select
                            Case DataLinkImportProfile.Planning
                                ' Importación de planificación V2 desde Excel
                                If oImportSchemaFile IsNot Nothing Then
                                    roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting planning v2 import")
                                    Dim strErrorInfo As String = String.Empty
                                    Dim oDataLinkImport As New roCalendarImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                    oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                    oDataLinkImport.ImportPlanningV2(oImportTask.ImportParameters.BeginDate, oImportTask.ImportParameters.EndDate, conf(0) & "¬" & conf(2) & "¬" & conf(3), strErrorInfo, oImportTask.ImportParameters.CalendarExcelIsTemplate, oImportTask.ImportParameters.CalendarCopyMainShifts, oImportTask.ImportParameters.CalendarCopyHolidays, oImportTask.ImportParameters.CalendarKeepHolidays, oImportTask.ImportParameters.CalendarKeepLockedDays, oImportFile, oImportSchemaFile)
                                Else
                                    oDataLinkState.Result = DataLinkResultEnum.UnableToRecoverExcelProfile
                                End If
                            Case DataLinkImportProfile.Absences
                                Select Case dataFileType
                                    Case DataLinkResultFileType.Text
                                        ' Importación de Ausencias ASCII
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting ASCII ProgrammedAbsences import")
                                        Dim oDataLinkImport As New roCalendarImport(oTempFileName, oTempTemplateName, oDataLinkState)
                                        oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                        oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                        oDataLinkImport.IsAutomaticProcess = oImportTask.ImportParameters.IsProgrammed
                                        oDataLinkImport.ImportCalendarAbsencesAscii(separator)
                                    Case DataLinkResultFileType.Excel
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting Excel ProgrammedAbsences import")
                                        Dim oDataLinkImport As New roCalendarImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                        oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                        oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                        oDataLinkImport.IsAutomaticProcess = oImportTask.ImportParameters.IsProgrammed
                                        oDataLinkImport.ImportCalendarAbsencesEXCEL()
                                End Select
                            Case DataLinkImportProfile.DailyCauses
                                ' Importación de Justificaciones diarias
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting DailyCauses import")
                                Dim oDataLinkImport As New roCausesImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                oDataLinkImport.ImportDailyCausesExcel()
                            Case DataLinkImportProfile.Tasks
                                ' Importación de tareas desde Excel
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting tasks import")
                                Dim oDataLinkImport As New roTasksImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                oDataLinkImport.ImportTasksExcel()
                            Case DataLinkImportProfile.CostCenters
                                ' Importación de centros de coste desde Excel
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting business centers import")
                                Dim oDataLinkImport As New roCostCentersImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                oDataLinkImport.ImportBusinessCenter(oImportTask.ImportGuide.Id)
                            Case DataLinkImportProfile.Supervisors
                                Select Case dataFileType
                                    Case DataLinkResultFileType.Excel
                                        roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting Excel supervisors import")
                                        Dim oDataLinkImport As New roSupervisorsImport(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                        oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                        oDataLinkImport.ImportFileCreationDateTime = oImportTask.ImportParameters.BeginDate
                                        oDataLinkImport.IsAutomaticProcess = oImportTask.ImportParameters.IsProgrammed
                                        oDataLinkImport.ImportSupervisorsExcel()
                                End Select
                            Case DataLinkImportProfile.VSLWorkSheets
                                ' Importación de partes de trabajo (ESPECIAL VSL)
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roDatalinkImportManager::ExecuteImport:Starting tasks import")
                                Dim oDataLinkImport As New roCustomVSL(eImportType.IsExcelType, oImportFile, oDataLinkState)
                                oDataLinkImport.IDImportGuide = oImportTask.ImportGuide.Id
                                oDataLinkImport.VSL_ImportWorkSheetsExcel()
                        End Select

                        ' Borro ficheros temporales
                        Try
                            If oTempFileName <> String.Empty Then
                                If IO.File.Exists(oTempFileName) Then IO.File.Delete(oTempFileName)
                            End If
                            If oTempTemplateName <> String.Empty Then
                                If IO.File.Exists(oTempTemplateName) Then IO.File.Delete(oTempTemplateName)
                            End If
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "VTDataLink::roDataLinkImportManager::ExecuteImport::Error deleting temporary files: ", ex)
                        End Try
                    End If

                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "VTDataLink::roDataLinkImportManager::ExecuteImport::Unexpected error: ", ex)
                    oDataLinkState.Result = DataLinkResultEnum.Exception
                    oDataLinkState.ErrorText = ex.Message
                End Try
            Else
                oDataLinkState.Result = DataLinkResultEnum.NoPassportSpecified
            End If

            Return oDataLinkState
        End Function

#End Region

#Region "Helper"

        Private Shared Sub LoadEmployeeFilter(oImportTask As roDatalinkImportTask, ByRef strListEmployees As String, ByRef strEmployeefilter As String, ByRef conf() As String, ByRef lstEmployees As List(Of Integer))
            Try
                strListEmployees = ""
                strEmployeefilter = String.Empty
                conf = oImportTask.ImportParameters.EmployeesSelected.Split("@")

                Dim strSelectorEmployees As String = conf(0)
                Dim strFeature As String = conf(1)
                Dim strFilter As String = conf(2)
                Dim strFilterUser As String = conf(3)

                Dim oParam As New AdvancedParameter.roAdvancedParameter("SeeEmployeeDataAtDate", New AdvancedParameter.roAdvancedParameterState())
                Dim bolEmployeesAtDate As Boolean = If(String.IsNullOrEmpty(oParam.Value), True, Any2Boolean(oParam.Value))

                If oImportTask.ImportParameters.BeginDate = #12:00:00 AM# OrElse oImportTask.ImportParameters.EndDate = #12:00:00 AM# Then
                    bolEmployeesAtDate = False
                End If


                If bolEmployeesAtDate Then
                    lstEmployees = Security.roSelector.GetEmployeeListByContract(oImportTask.ImportParameters.IdPassport, strFeature, "U", Permission.Read,
                                        strSelectorEmployees, strFilterUser, False, oImportTask.ImportParameters.BeginDate, oImportTask.ImportParameters.EndDate)
                Else
                    lstEmployees = Security.roSelector.GetEmployeeList(oImportTask.ImportParameters.IdPassport, strFeature, "U", Permission.Read,
                                        strSelectorEmployees, strFilter, strFilterUser, False, Nothing, Nothing)
                End If


                For Each iID As Integer In lstEmployees
                    strEmployeefilter = strEmployeefilter & iID.ToString & ","
                Next
                If strEmployeefilter.Length > 0 Then strEmployeefilter = strEmployeefilter.Substring(0, strEmployeefilter.Length - 1)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTDataLink::roDataLinkImportManager::LoadEmployeeFilter::Unexpected error: ", ex)
            End Try
        End Sub

#End Region

#Region "Events"

#End Region

    End Class

End Namespace