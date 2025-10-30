Imports System.Web.Services.Description
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace DataLink

    Public Class roCalendarImport
        Inherits roDataLinkImport

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)
        End Sub

        Public Sub New(ByVal importType As eImportType, ByVal oImportFile As Byte(), Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(importType, oImportFile, state)
        End Sub

        Public Sub New(ByVal importType As eImportType, ByVal fileNameAsciiOExcelOXML As String, Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(importType, fileNameAsciiOExcelOXML, state)
        End Sub

        Public Sub New(ByVal fileNameAscii As String, ByVal fileNameExcel As String, Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(fileNameAscii, fileNameExcel, state)
        End Sub


#Region "21- IMPORTACIÓN DE PLANIFICACIÓN V2"

        Public Function ImportPlanningV2(ByVal BeginDate As Date, ByVal EndDate As Date, ByVal mEmployees As String, ByRef strMsg As String, ByVal bExcelIsATemplate As Boolean, ByVal bCopyMainShifts As Boolean, ByVal bCopyHolidays As Boolean, ByVal bKeepHolidays As Boolean, ByVal bKeepLockedDays As Boolean, Optional oExcelFile As Byte() = Nothing, Optional oExcelProfileBytes As Byte() = Nothing) As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKExcel Then
                    ' Obtengo array de bytes del fichero
                    If oExcelFile Is Nothing Then
                        oExcelFile = IO.File.ReadAllBytes(Me.strFileNameExcel)
                    End If

                    Dim oResult As New Base.DTOs.roCalendarResult
                    Dim oCalendarState As New Base.VTCalendar.roCalendarState
                    roBusinessState.CopyTo(Me.State, oCalendarState)
                    Dim oCalendarManager As Base.VTCalendar.roCalendarManager = New Base.VTCalendar.roCalendarManager(oCalendarState)
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine
                    Select Case oCalendarManager.ImportFromExcelToDDBB(oExcelFile, mEmployees, BeginDate, EndDate, oResult, bCopyMainShifts, bCopyHolidays,, bKeepHolidays, bKeepLockedDays,,, True, bExcelIsATemplate, oExcelProfileBytes)
                        Case Base.DTOs.CalendarStatusEnum.OK
                            'Se ha guardado, todo o parte
                            Select Case oResult.Status
                                Case Base.DTOs.CalendarStatusEnum.WARNING, Base.DTOs.CalendarStatusEnum.KO
                                    Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                            End Select
                            If Not oResult.CalendarDataResult Is Nothing Then
                                For Each oError As Base.DTOs.roCalendarDataDayError In oResult.CalendarDataResult
                                    strLogEvent &= oError.ErrorText
                                Next
                            End If
                        Case Base.DTOs.CalendarStatusEnum.KO
                            Me.State.Result = DataLinkResultEnum.Exception
                            For Each oError As roCalendarDataDayError In oResult.CalendarDataResult
                                strMsg = strMsg & oError.ErrorText & vbCrLf
                            Next
                            strLogEvent = strLogEvent & strMsg
                            Me.State.ErrorText = strMsg
                    End Select
                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEXCELFile", "") & vbNewLine
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportPlanningV2")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

#End Region

#Region "22- IMPORTAR CALENDAR ABSENCES ASCII"

        Public Function ImportCalendarAbsencesAscii(ByVal strSeparator As String) As Boolean
            Dim bolRet As Boolean = False

            Dim languageTag As String = "ProgrammedAbsences"

            Dim updatedPeriod As Integer = 0
            Dim newPeriod As Integer = 0
            Dim deletedPeriod As Integer = 0
            Dim InvalidLines As Integer = 0
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""
            Dim sAPVDetail As String = String.Empty
            Dim bAvoidNotification As Boolean = False

            Try
                If Me.bolIsFileOKAscii AndAlso bolIsFileOKExcel Then

                    Dim iColumnsTotal As Integer = 0
                    iColumnsTotal = System.Enum.GetValues(GetType(RoboticsExternAccess.AbsencesAsciiColumns)).Length
                    If mAbsencesIgnoreColumnChar.Length > 0 Then
                        ' Si en la plantilla hay que ignorar columnas, no me ciño al número de posibles columnas, y veo cuántas columnas hay realmente en el excel
                        iColumnsTotal = BookSpreadsheetLight.GetWorksheetStatistics().NumberOfColumns
                    End If

                    ' Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(iColumnsTotal - 1) As Integer

                    ' Definimos array con los valores de las columnas
                    Dim ColumnsVal(iColumnsTotal - 1) As String

                    ' Lee todas las incidencias
                    Dim tbCauses As DataTable = CreateDataTable("@SELECT# id, Name, ShortName, Export from causes")

                    ' Inicia la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & vbNewLine

                    If GetSheetsCount() > 0 Then
                        ' Activa la prima hoja del fichero excel
                        SetActiveSheet(0)

                        ' Cuenta el número de líneas
                        Dim intLines As Integer
                        intLines = Me.CountLinesAscii()

                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then
                            Dim InvalidColumn As String = ""

                            ' Cuenta el número de columnas
                            Dim intColumns As Integer = Me.ValidarColumnasAbsences(ColumnsPos, InvalidColumn)
                            If intColumns > 0 Then
                                ReDim ColumnsVal(ColumnsPos.Length - 1)
                                Dim msgError As String = ""

                                ' Recorre el Fichero Ascii para grabar los datos en tablas desconectadas
                                Dim strLine As String = ""
                                Dim bHaveToClose As Boolean = False
                                For intRow As Integer = 0 To intLines - 1
                                    Try
                                        bolRet = False
                                        bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                                        ' Lee una línea del fichero Ascii
                                        strLine = Me.oAsciiReader.ReadLine()
                                        Dim errorInfo As String = String.Empty
                                        ' Procesa la línea
                                        If Me.GetDataAsciiAbsences(strLine, intColumns, ColumnsPos, ColumnsVal, strSeparator, errorInfo) Then

                                            If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration) = "" Then
                                                ' Graba una ausencia programada
                                                Dim oAbsenceStateResult As New Absence.roProgrammedAbsenceState
                                                bolRet = ProcessProgrammedAbsence(tbCauses, ColumnsVal, msgError, intRow + 1, oAbsenceStateResult)

                                                If mCustomizationCode.ToUpper = "VPA" AndAlso oAbsenceStateResult.ErrorText.Contains("#APV*") Then
                                                    sAPVDetail &= oAbsenceStateResult.ErrorText.Replace("#APV*", "") & vbCrLf & vbCrLf & vbCrLf
                                                End If
                                            Else
                                                ' Graba una ausencia por horas
                                                Dim oAbsenceStateResult As New Incidence.roProgrammedCauseState
                                                bolRet = ProcessProgrammedCause(tbCauses, ColumnsVal, msgError, intRow + 1, oAbsenceStateResult)
                                            End If

                                            ' Cuenta el registro
                                            Dim cInsert As Char = mAbsencesOperationCodes.Substring(0, 1)
                                            Dim cModify As Char = mAbsencesOperationCodes.Substring(1, 1)
                                            Dim cDelete As Char = mAbsencesOperationCodes.Substring(2, 1)

                                            If bolRet Then
                                                Select Case ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action).ToString
                                                    Case cInsert
                                                        newPeriod += 1
                                                    Case cModify
                                                        updatedPeriod += 1
                                                    Case cDelete
                                                        deletedPeriod += 1
                                                End Select
                                            Else
                                                Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow + 1 & " " & errorInfo
                                                If msgError <> "" Then msgLog &= ". " & msgError & vbNewLine
                                                msgLog &= vbNewLine
                                            End If
                                        Else
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                            msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow + 1 & " " & errorInfo & vbNewLine
                                        End If
                                    Catch ex As Exception
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportCalendarAbsencesaAsccii")
                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow + 1 & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    Finally
                                        Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                    End Try

                                    If bolRet = False Then
                                        InvalidLines += 1
                                    End If
                                Next

                                'APV: Envío de mail con incidencias en la importación de ausencias resueltas automáticamente
                                If mCustomizationCode.ToUpper = "VPA" AndAlso sAPVDetail.Length > 0 Then
                                    ' Creo tarea de notificación
                                    Dim iNotification As Integer = -1
                                    Dim strSQL As String = "@SELECT# TOP 1 ID from Notifications where IDType = 61 and Name LIKE '%(Adv=IMPAUS)' AND Activated = 1"
                                    iNotification = roTypes.Any2Integer(ExecuteScalar(strSQL))
                                    If iNotification > 0 Then
                                        strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Parameters ) VALUES " &
                                                "(" & iNotification.ToString & ", " & eImportGuide.CalendarAbsencesASCII & ",'" & sAPVDetail & "')"
                                        bolRet = ExecuteSql(strSQL)
                                        If bolRet Then bAvoidNotification = True
                                    End If
                                End If

                                ' Resumen importación
                                strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent." & languageTag & "Finish", "") & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.New" & languageTag, "") & newPeriod.ToString & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.Updated" & languageTag, "") & updatedPeriod.ToString & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.Deleted" & languageTag, "") & deletedPeriod.ToString & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.Invalid" & languageTag, "") & InvalidLines.ToString & vbNewLine
                            Else
                                ' Definición de columnas incorrectas
                                Me.State.Result = DataLinkResultEnum.InvalidColumns
                                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidColumns", "") & " '" & InvalidColumn & "'" & vbNewLine
                            End If
                        Else
                            Me.State.Result = DataLinkResultEnum.NoRegisters
                            strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoRegisters", "") & vbNewLine
                        End If
                    End If

                    ' Auditamos importación ausencias programadas
                    Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                    Me.State.AddAuditParameter(tbParameters, "{ImportProgrammedAbsences}", "Ascii", "", 1)
                    Me.State.Audit(VTBase.Audit.Action.aExecuted, VTBase.Audit.ObjectType.tDataLinkImportProgrammedAbsences, "", tbParameters, -1)

                    ' Crea la tarea de Ausencias programadas
                    If newPeriod + updatedPeriod + deletedPeriod Then
                        Dim oContext As New roCollection
                        oContext.Add("Employee.ID", -1)
                        oContext.Add("Date", Now.Date)
                        Extensions.roConnector.InitTask(TasksType.MOVES, oContext)
                    End If
                Else
                    If bolIsFileOKAscii = False Then
                        Me.State.Result = DataLinkResultEnum.InvalidASCIIFile
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidASCIIFile", "") & vbNewLine
                    ElseIf bolIsFileOKExcel = False Then
                        Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidFormatFile", "") & vbNewLine
                    End If
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportDailyCalendarAbsencesASCII")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport, bAvoidNotification)
            End Try

            Return bolRet

        End Function

        Private Function ValidarColumnasAbsences(ByRef ColumnsPos() As Integer, ByRef InvalidColumn As String) As Integer
            Dim intNumColumnas As Integer = -1
            Dim bolIsValid As Boolean = True
            Dim intCol As Integer
            Dim Columna As String

            Try
                ' Inicializa variable
                InvalidColumn = ""
                For intCol = 0 To ColumnsPos.Length - 1
                    ColumnsPos(intCol) = -1
                Next

                intCol = 0

                ' Lee primera columna
                Columna = GetCellValueWithoutFormat(0, intCol).ToUpper
                Do While Columna <> "" And bolIsValid
                    Select Case Columna
                        Case "OPERACION", "OPERACIÓN", "TIPO DE REGISTRO" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.Action) = intCol
                        Case "NIF", "CIF" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.NIF) = intCol
                        Case "CIF LETRA" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter) = intCol
                        Case "NOMBRE CORTO" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) = intCol
                        Case "FECHA INICIO", "FECHA_INCIO" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) = intCol
                        Case "FECHA FINAL", "FECHA FIN", "FECHA_FINAL" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) = intCol
                        Case "HORA INICIO" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = intCol
                        Case "HORA FINAL", "HORA FIN" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.EndHour) = intCol
                        Case "DURACION" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.Duration) = intCol
                        Case "MAXIMOS DIAS" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) = intCol
                        Case "IMPORT_KEY" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) = intCol
                        Case "ID INCIDENCIA" : ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) = intCol
                        Case "IDENTIFICADOR", "FECHA DE CORTE" : bolIsValid = True

                        Case Else
                            bolIsValid = (Columna = mAbsencesIgnoreColumnChar)
                    End Select

                    If bolIsValid Then
                        intCol += 1
                        Columna = GetCellValueWithoutFormat(0, intCol).ToUpper
                    Else
                        InvalidColumn = Columna
                    End If
                Loop

                ' Comprueba las columnas obligatorias
                If bolIsValid = True Then
                    bolIsValid = False
                    InvalidColumn = "Required Fields"

                    If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.NIF) = -1 AndAlso ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) = -1 Then Exit Try
                    If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) = -1 AndAlso ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) = -1 Then Exit Try
                    If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) = -1 Then Exit Try
                    'If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) = -1 And ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) = -1 Then Exit Try

                    If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.Duration) <> -1 Then
                        ' Ausencia por horas
                        If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = -1 Then Exit Try
                        If ColumnsPos(RoboticsExternAccess.AbsencesAsciiColumns.EndHour) = -1 Then Exit Try
                    End If

                    bolIsValid = True
                    InvalidColumn = ""
                End If

                If bolIsValid Then intNumColumnas = intCol
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ValidarColumnasAbsences")

            End Try

            Return intNumColumnas
        End Function

        Private Function GetDataAsciiAbsences(ByVal strData As String, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String,
                                                  ByVal strSeparator As String, Optional ByRef errorInfo As String = "") As Boolean

            Dim bolRet As Boolean = False

            Try

                'Inicializamos los datos
                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                Dim SplitData() As String = strData.Split(strSeparator)
                Dim intPos As Integer
                Dim strValue As String = ""
                For intColumn As Integer = 0 To ColumnsPos.Length - 1
                    intPos = ColumnsPos(intColumn)
                    If intPos >= 0 Then
                        If SplitData.Length > intPos Then
                            strValue = SplitData(intPos)
                            If strValue IsNot Nothing Then
                                ColumnsVal(intColumn) = strValue.Trim
                            End If
                        End If
                    End If
                Next

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ImportPrimaryKey", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ShortCause", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ExportCause", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.BeginDate", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ProgrammedAbsence", "")
                Else
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ProgrammedCause", "")
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.InsertOperation", "")
                Else
                    ' Si la acción corresponde con alguna de estas , parseamos el valor
                    Select Case ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action).ToUpper
                        'IUD
                        Case "NUEVO" : errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.InsertOperation", "")
                        Case "ACTUAL" : errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.UpdateOperation", "")
                        Case "BORRADO" : errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.DeleteOperation", "")
                    End Select
                End If

                ' Comprueba los datos obligatorios
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF) = "" AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) = "" Then Exit Try
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) = "" AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) = "" Then Exit Try
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) = "" Then Exit Try

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration) <> "" Or ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) <> "" Or ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour) <> "" Then
                    ' Ausencia por horas
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = "" Then Exit Try
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour) = "" Then Exit Try
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration) = "" Then Exit Try

                    ' No puede ser por dias y horas a la vez
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) <> "" And ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) <> ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) Then Exit Try
                Else
                    ' Ausencia por días
                    ' Si no tiene fecha final ni Numero Maximo de dias de duracion asigna 60 días por defecto
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) = "" And ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) = "" Then
                        ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) = 60
                    End If
                End If

                ' Si no hay operación por defecto es un Alta
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "" Then ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "I"

                ' Si hay Cif_Letra une CIF con CIF_Letra
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter) <> "" Then
                    ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF) = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF) & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter)
                    ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter) = ""
                End If

                ' Comprueba la consistencia de la información
                Try
                    Dim aux As String = ""

                    ' Fecha inicio
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate)
                    If aux.Length <> 0 Then
                        If Not IsDate(aux) Then Exit Try
                    End If

                    ' Fecha final
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate)
                    If aux.Length <> 0 Then
                        If Not IsDate(aux) Then Exit Try
                    End If

                    ' Hora inicio
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour)
                    If aux.Length <> 0 Then
                        If aux.Length > 5 Or Not IsDate(aux) Then Exit Try
                    End If

                    ' Hora final
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour)
                    If aux.Length <> 0 Then
                        If aux.Length > 5 Or Not IsDate(aux) Then Exit Try
                    End If

                    ' La fecha de finalización no puede ser inferior a la de inicio
                    Dim BeginDate As DateTime = CDate(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate))
                    Dim EndDate As DateTime

                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) <> "" Then
                        EndDate = CDate(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate))
                        If BeginDate > EndDate Then Exit Try
                    End If

                    bolRet = True
                Catch ex As Exception
                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataAsciiAbsences")

                End Try
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataAsciiAbsences")
            End Try

            Return bolRet

        End Function

        Protected Friend Function ProcessProgrammedAbsence(ByVal tbCauses As DataTable, ByRef ColumnsVal() As String, ByRef msgLog As String, ByVal intRow As Integer, ByRef oAbsenceStateResult As Absence.roProgrammedAbsenceState) As Boolean
            Dim bolRet As Boolean = False

            Try
                msgLog = ""

                Dim action As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action)
                Dim ShortCause As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause)
                Dim ExportCause As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause)

                Dim startDate As Date = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate))
                Dim endDate As Date
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) <> "" Then endDate = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate))

                Dim maxDays As Integer
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) <> "" Then maxDays = Convert.ToInt32(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays))

                ' Busca el empleado
                Dim idEmployee As Integer = isEmployeeNew(ColumnsVal, RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey, RoboticsExternAccess.AbsencesAsciiColumns.NIF, New UserFields.roUserFieldState)
                If idEmployee > 0 Then
                    ' Busca la incidencia
                    Dim rw() As DataRow = Nothing
                    If ShortCause IsNot Nothing AndAlso ShortCause.Length > 0 Then
                        rw = tbCauses.Select("ShortName='" & ShortCause.Replace("'", "''") & "'")
                    Else
                        rw = tbCauses.Select("Export='" & ExportCause.Replace("'", "''") & "'")
                    End If

                    If rw.Length = 0 Then
                        Me.State.Result = DataLinkResultEnum.InvalidCause
                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidCauseOnRegister", "") & " " & intRow & vbNewLine
                    Else
                        Dim idCause As Long = rw(0)("id")

                        Dim cInsert As Char = mAbsencesOperationCodes.Substring(0, 1)
                        Dim cModify As Char = mAbsencesOperationCodes.Substring(1, 1)
                        Dim cDelete As Char = mAbsencesOperationCodes.Substring(2, 1)

                        ' Crea el registro
                        Select Case action
                            Case cInsert
                                oAbsenceStateResult = New Absence.roProgrammedAbsenceState
                                roBusinessState.CopyTo(Me.State, oAbsenceStateResult)
                                Dim oAbsence As New Absence.roProgrammedAbsence(idEmployee, startDate, mCustomizationCode.ToUpper, "DATAIMPORT", oAbsenceStateResult)
                                If oAbsence.Load(False) Then
                                    If endDate = #12:00:00 AM# Then
                                        oAbsence.MaxLastingDays = maxDays
                                        oAbsence.FinishDate = Nothing
                                    Else
                                        oAbsence.FinishDate = endDate
                                        oAbsence.MaxLastingDays = 0
                                    End If

                                    oAbsence.IDCause = idCause
                                    bolRet = oAbsence.Save(, False)
                                    If bolRet Then
                                    Else
                                        msgLog = oAbsence.State.ErrorText
                                        roBusinessState.CopyTo(oAbsence.State, oAbsenceStateResult)
                                    End If
                                End If

                            Case cModify
                                oAbsenceStateResult = New Absence.roProgrammedAbsenceState
                                roBusinessState.CopyTo(Me.State, oAbsenceStateResult)
                                Dim oAbsence As New Absence.roProgrammedAbsence(idEmployee, startDate, mCustomizationCode.ToUpper, "DATAIMPORT", oAbsenceStateResult)
                                If oAbsence.Load(False) Then
                                    If Not IsNothing(oAbsence.IDCause) Then
                                        If endDate = #12:00:00 AM# Then
                                            oAbsence.MaxLastingDays = maxDays
                                            oAbsence.FinishDate = Nothing
                                        Else
                                            oAbsence.FinishDate = endDate
                                            oAbsence.MaxLastingDays = 0
                                        End If

                                        oAbsence.IDCause = idCause
                                        bolRet = oAbsence.Save(, False)
                                        If bolRet = False Then
                                            msgLog = oAbsence.State.ErrorText
                                            roBusinessState.CopyTo(oAbsence.State, oAbsenceStateResult)
                                        End If
                                    Else
                                        Me.State.Result = DataLinkResultEnum.InvalidData
                                        msgLog = Me.State.Language.Translate("ProgrammedAbsence.Error.DoesNotExists", "")
                                    End If
                                End If
                            Case cDelete
                                oAbsenceStateResult = New Absence.roProgrammedAbsenceState
                                roBusinessState.CopyTo(Me.State, oAbsenceStateResult)
                                Dim oAbsence As New Absence.roProgrammedAbsence(idEmployee, startDate, oAbsenceStateResult)
                                If oAbsence.Load(False) Then
                                    If Not IsNothing(oAbsence.IDCause) Then
                                        bolRet = oAbsence.Delete()
                                        If bolRet = False Then
                                            msgLog = oAbsence.State.ErrorText
                                            roBusinessState.CopyTo(oAbsence.State, oAbsenceStateResult)
                                        End If
                                    Else
                                        Me.State.Result = DataLinkResultEnum.InvalidData
                                        msgLog = Me.State.Language.Translate("ProgrammedAbsence.Error.DoesNotExists", "")
                                    End If
                                End If
                        End Select
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEmployee", "") & " " & intRow & vbNewLine
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ProcessProgrammedAbsence")
            End Try

            Return bolRet

        End Function

        Protected Friend Function ProcessProgrammedCause(ByVal tbCauses As DataTable, ByRef ColumnsVal() As String, ByRef msgLog As String, ByVal intRow As Integer, ByRef oAbsenceState As Incidence.roProgrammedCauseState) As Boolean
            Dim bolRet As Boolean = False

            Try
                msgLog = ""

                Dim action As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action)
                Dim ShortCause As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause)
                Dim ExportCause As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause)

                Dim startDate As Date = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate))
                Dim endDate As Date = startDate
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) <> "" Then endDate = roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate))

                Dim startHour As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour)
                Dim endHour As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour)

                'Dim durationSg As Date = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration)
                Dim durationSg As String = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration).Replace(".", roConversions.GetDecimalDigitFormat())

                If Not durationSg.Contains(":") Then
                    durationSg = roConversions.ConvertHoursToTime(durationSg)
                End If

                ' Busca el empleado
                Dim idEmployee As Integer = isEmployeeNew(ColumnsVal, RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey, RoboticsExternAccess.AbsencesAsciiColumns.NIF, New UserFields.roUserFieldState)

                If idEmployee > 0 Then
                    ' Busca la incidencia
                    Dim rw() As DataRow = Nothing
                    If ShortCause.Length > 0 Then
                        rw = tbCauses.Select("ShortName='" & ShortCause.Replace("'", "''") & "'")
                    Else
                        rw = tbCauses.Select("Export='" & ExportCause.Replace("'", "''") & "'")
                    End If

                    If rw.Length = 0 Then
                        Me.State.Result = DataLinkResultEnum.InvalidCause
                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidCauseOnRegister", "") & " " & intRow & vbNewLine
                    Else
                        Dim idCause As Long = rw(0)("id")
                        ' Crea el registro
                        Select Case action
                            Case "I"
                                oAbsenceState = New Incidence.roProgrammedCauseState
                                roBusinessState.CopyTo(Me.State, oAbsenceState)
                                Dim oAbsence As New Incidence.roProgrammedCause(idEmployee, startDate, oAbsenceState)
                                If endDate <> #12:00:00 AM# Then oAbsence.ProgrammedEndDate = endDate
                                oAbsence.MinDuration = 0
                                oAbsence.Duration = roConversions.ConvertTimeToHours(durationSg)
                                oAbsence.IDCause = idCause
                                oAbsence.BeginTime = roTypes.Any2DateTime("1899-12-30 " & startHour)
                                oAbsence.EndTime = roTypes.Any2DateTime("1899-12-30 " & endHour)
                                bolRet = oAbsence.Save(, False)
                                If Not bolRet Then
                                    Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                    msgLog = oAbsence.State.ErrorText
                                    roBusinessState.CopyTo(oAbsence.State, oAbsenceState)
                                End If
                            Case "U"
                                oAbsenceState = New Incidence.roProgrammedCauseState
                                roBusinessState.CopyTo(Me.State, oAbsenceState)
                                Dim oAbsence As New Incidence.roProgrammedCause(idEmployee, startDate, oAbsenceState)
                                oAbsence.BeginTime = roTypes.Any2DateTime("1899-12-30 " & startHour)

                                If oAbsence.LoadByStartDate(False) Then
                                    If endDate <> #12:00:00 AM# Then oAbsence.ProgrammedEndDate = endDate
                                    oAbsence.Duration = roConversions.ConvertTimeToHours(durationSg)
                                    oAbsence.IDCause = idCause
                                    oAbsence.EndTime = roTypes.Any2DateTime("1899-12-30 " & endHour)
                                    bolRet = oAbsence.Save(, False)
                                    If bolRet = False Then msgLog = oAbsence.State.ErrorText
                                Else
                                    Me.State.Result = DataLinkResultEnum.InvalidData
                                    msgLog = Me.State.Language.Translate("ProgrammedCause.Error.DoesNotExists", "")
                                    roBusinessState.CopyTo(oAbsence.State, oAbsenceState)
                                    bolRet = False
                                End If

                            Case "D"
                                oAbsenceState = New Incidence.roProgrammedCauseState
                                roBusinessState.CopyTo(Me.State, oAbsenceState)
                                Dim oAbsence As New Incidence.roProgrammedCause(idEmployee, startDate, oAbsenceState)
                                oAbsence.BeginTime = roTypes.Any2DateTime("1899-12-30 " & startHour)

                                If oAbsence.LoadByStartDate(False) Then
                                    bolRet = oAbsence.Delete(True)
                                    If bolRet = False Then msgLog = oAbsence.State.ErrorText
                                Else
                                    Me.State.Result = DataLinkResultEnum.InvalidData
                                    msgLog = Me.State.Language.Translate("ProgrammedCause.Error.DoesNotExists", "")
                                    roBusinessState.CopyTo(oAbsence.State, oAbsenceState)
                                    bolRet = False
                                End If
                        End Select
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEmployee", "") & " " & intRow & vbNewLine
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ProcessProgrammedCause")

            End Try

            Return bolRet

        End Function

#End Region

#Region "22 - IMPORTAR CALENDAR ABSENCES EXCEL"

        Public Function ImportCalendarAbsencesEXCEL() As Boolean
            Dim bolRet As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""
            Dim updatedPeriod As Integer = 0
            Dim newPeriod As Integer = 0
            Dim deletedPeriod As Integer = 0
            Dim InvalidLines As Integer = 0
            Dim languageTag As String = "ProgrammedAbsences"

            Try
                If Me.bolIsFileOKExcel Then

                    'Obtenemos la fecha de congelación
                    Dim freezeDate As Date = New Date(1900, 1, 1)

                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(AbsencesAsciiColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(AbsencesAsciiColumns)).Length - 1) As String

                    ' Lee todas las justificaciones
                    Dim tbCauses As DataTable = CreateDataTable("@SELECT# id, Name, ShortName, Export from Causes")

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine

                    If GetSheetsCount() > 0 Then

                        SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intBeginLine As Integer
                        Dim intEndLine As Integer
                        Dim intLines As Integer = Me.CountLinesExcel(intBeginLine, intEndLine)
                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then
                            Dim InvalidColumn As String = ""

                            'Contar número de columnas
                            Dim FieldKey As String = ""
                            Dim intColumns As Integer = Me.ValidarColumnasAbsences(ColumnsPos, InvalidColumn)
                            If intColumns > 0 Then

                                bolRet = True
                                Dim msgError As String = ""

                                Dim bHaveToClose As Boolean = False
                                ' Recorremos toda la hoja excel
                                For intRow As Integer = intBeginLine To intEndLine
                                    Try

                                        bolRet = False
                                        bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                                        Dim errorInfo As String = String.Empty
                                        'Obtenemos los datos del puesto a partir del excel
                                        If Me.GetDataExcelAbsences(intRow, intColumns, ColumnsPos, ColumnsVal, errorInfo) Then

                                            If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = "" Then
                                                ' Graba una ausencia programada                                                
                                                Dim oAbsenceStateResult As New Absence.roProgrammedAbsenceState
                                                bolRet = ProcessProgrammedAbsence(tbCauses, ColumnsVal, msgError, intRow, oAbsenceStateResult)
                                            Else
                                                ' Graba una ausencia por horas                                                
                                                Dim oAbsenceStateResult As New Incidence.roProgrammedCauseState
                                                bolRet = ProcessProgrammedCause(tbCauses, ColumnsVal, msgError, intRow, oAbsenceStateResult)
                                            End If

                                            ' Cuenta el registro
                                            Dim cInsert As Char = mAbsencesOperationCodes.Substring(0, 1)
                                            Dim cModify As Char = mAbsencesOperationCodes.Substring(1, 1)
                                            Dim cDelete As Char = mAbsencesOperationCodes.Substring(2, 1)

                                            If bolRet Then
                                                Select Case ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action).ToString
                                                    Case cInsert
                                                        newPeriod += 1
                                                    Case cModify
                                                        updatedPeriod += 1
                                                    Case cDelete
                                                        deletedPeriod += 1
                                                End Select
                                            Else
                                                Me.State.Result = DataLinkResultEnum.SomeRegistersNotImported
                                                msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.RegisterNotImported", "") & " " & intRow & " " & errorInfo & " "
                                                If msgError <> "" Then msgLog &= ". " & msgError & vbNewLine
                                                msgLog &= vbNewLine
                                            End If
                                        Else
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                            msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow & " " & errorInfo & " " & vbNewLine
                                        End If
                                    Catch ex As Exception
                                        Me.State.Result = DataLinkResultEnum.Exception
                                        Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportCalendarAbsencesexcel")
                                        msgLog &= Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownErrorOnRegister", "") & " " & intRow & vbNewLine & ex.Message & vbNewLine
                                        bolRet = False
                                    Finally
                                        Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                                    End Try

                                    If bolRet = False Then
                                        InvalidLines += 1
                                    End If

                                Next

                                ' Resumen importación
                                strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent." & languageTag & "Finish", "") & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.New" & languageTag, "") & newPeriod.ToString & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.Updated" & languageTag, "") & updatedPeriod.ToString & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.Deleted" & languageTag, "") & deletedPeriod.ToString & vbNewLine
                                strLogEvent = strLogEvent & Me.State.Language.Translate("Import.LogEvent.Invalid" & languageTag, "") & InvalidLines.ToString & vbNewLine
                            Else ' Columnas inválidas
                                Me.State.Result = DataLinkResultEnum.InvalidColumns
                                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidColumns", "") & " '" & InvalidColumn & "'" & vbNewLine
                            End If
                        Else ' No has registros
                            Me.State.Result = DataLinkResultEnum.NoRegisters
                            strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoRegisters", "") & vbNewLine
                        End If
                    Else ' No hay ningún libro en el fichero excel
                        Me.State.Result = DataLinkResultEnum.NoSheets
                        strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NoSheetsInExcelFile", "") & vbNewLine
                    End If

                    ' Auditamos importación ausencias programadas
                    Dim tbParameters As DataTable = Me.State.CreateAuditParameters()
                    Me.State.AddAuditParameter(tbParameters, "{ImportProgrammedAbsences}", "Excel", "", 1)
                    Me.State.Audit(VTBase.Audit.Action.aExecuted, VTBase.Audit.ObjectType.tDataLinkImportProgrammedAbsences, "", tbParameters, -1)

                    ' Crea la tarea de Ausencias programadas
                    If newPeriod + updatedPeriod + deletedPeriod Then
                        Dim oContext As New roCollection
                        oContext.Add("Employee.ID", -1)
                        oContext.Add("Date", Now.Date)
                        Extensions.roConnector.InitTask(TasksType.MOVES, oContext)
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEXCELFile", "") & vbNewLine
                End If
            Catch ex As Exception
                bolRet = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::ImportCalendarAbsencesEXCEL")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                Me.SaveImportLog(Me.IDImportGuide, strLogEvent & msgLog, Me.State.IDPassport)
            End Try

            Return bolRet

        End Function

        Private Function GetDataExcelAbsences(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String, Optional ByRef errorInfo As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Count - 1
                    ColumnsVal(intColumn) = GetCellValue(intRow, ColumnsPos(intColumn))
                Next

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ImportPrimaryKey", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ShortCause", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ExportCause", "") & " " & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) IsNot Nothing AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) <> "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.BeginDate", "") & " " & roTypes.Any2DateTime(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate)).ToShortDateString() & " "
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ProgrammedAbsence", "")
                Else
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.ProgrammedCause", "")
                End If

                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "" Then
                    errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.InsertOperation", "")
                Else
                    ' Si la acción corresponde con alguna de estas , parseamos el valor
                    Select Case ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action).ToUpper
                        'IUD
                        Case "NUEVO" : errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.InsertOperation", "")
                        Case "ACTUAL" : errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.UpdateOperation", "")
                        Case "BORRADO" : errorInfo = errorInfo & " " & Me.State.Language.Translate("Import.LogEvent.DeleteOperation", "")
                    End Select
                End If

                ' Comprueba los datos obligatorios
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF) = "" AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ImportPrimaryKey) = "" Then Exit Try
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ShortCause) = "" AndAlso ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.ExportCause) = "" Then Exit Try
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) = "" Then Exit Try


                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) <> "" Or ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour) <> "" Then
                    ' Ausencia por horas
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour) = "" Then Exit Try
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour) = "" Then Exit Try
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Duration) = "" Then Exit Try

                    ' No puede ser por dias y horas a la vez
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) <> "" And ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate) <> ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) Then Exit Try
                Else
                    ' Ausencia por días
                    ' Si no tiene fecha final ni Numero Maximo de dias de duracion asigna 60 días por defecto
                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) = "" And ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) = "" Then
                        ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.MaxDays) = 60
                    End If
                End If

                ' Si no hay operación por defecto es un Alta
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "" Then
                    ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "I"
                Else
                    ' Si la acción corresponde con alguna de estas , parseamos el valor
                    Select Case ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action).ToUpper
                        'IUD
                        Case "NUEVO" : ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "I"
                        Case "ACTUAL" : ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "U"
                        Case "BORRADO" : ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.Action) = "D"
                    End Select
                End If
                ' Si hay Cif_Letra une CIF con CIF_Letra
                If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter) <> "" Then
                    ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF) = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF) & ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter)
                    ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.NIF_Letter) = ""
                End If

                ' Comprueba la consistencia de la información
                Try
                    Dim aux As String = ""

                    ' Fecha inicio
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate)
                    If aux.Length <> 0 Then
                        If Not IsDate(aux) Then Exit Try
                    End If

                    ' Fecha final
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate)
                    If aux.Length <> 0 Then
                        If Not IsDate(aux) Then Exit Try
                    End If

                    ' Hora inicio
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginHour)
                    If aux.Length <> 0 Then
                        If aux.Length > 5 Or Not IsDate(aux) Then Exit Try
                    End If

                    ' Hora final
                    aux = ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndHour)
                    If aux.Length <> 0 Then
                        If aux.Length > 5 Or Not IsDate(aux) Then Exit Try
                    End If

                    ' La fecha de finalización no puede ser inferior a la de inicio
                    Dim BeginDate As DateTime = CDate(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.BeginDate))
                    Dim EndDate As DateTime

                    If ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate) <> "" Then
                        EndDate = CDate(ColumnsVal(RoboticsExternAccess.AbsencesAsciiColumns.EndDate))
                        If BeginDate > EndDate Then Exit Try
                    End If

                    bolRet = True
                Catch ex As Exception
                    Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelAbsences")

                End Try
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelAbsences")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace