Imports System.Data.Common
Imports System.IO
Imports DocumentFormat.OpenXml.Packaging
Imports Newtonsoft.Json
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Mail
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roCustomVSL
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

        Public Function VSL_ImportWorkSheetsExcel() As Boolean
            Dim ret As Boolean = False
            Dim strLogEvent As String = ""
            Dim msgLog As String = ""

            Try
                If Me.bolIsFileOKExcel Then
                    'Definimos array con las posiciones de las columnas
                    Dim ColumnsPos(System.Enum.GetValues(GetType(VSL_WorkSheetsExcelColumns)).Length - 1) As Integer

                    'Definimos array con los valores de las columnas
                    Dim ColumnsVal(System.Enum.GetValues(GetType(VSL_WorkSheetsExcelColumns)).Length - 1) As String

                    Dim n As Integer = 0
                    Dim u As Integer = 0

                    'Inicio de la importación
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Start", "") & Environment.NewLine

                    If Me.GetSheetsCount() > 0 Then

                        Me.SetActiveSheet(0)

                        ' Contamos el número de lineas
                        Dim intBeginLine As Integer
                        Dim intEndLine As Integer
                        Dim intLines As Integer = Me.CountLinesExcel(intBeginLine, intEndLine)
                        strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.TotalRows", "") & " " & intLines.ToString & Environment.NewLine
                        If intLines > 0 Then
                            Dim InvalidColumn As String = ""

                            'Contar número de columnas
                            Dim intColumns As Integer = Me.VSL_ValidarColumnasWorkSheets(ColumnsPos, InvalidColumn, Me.State)
                            If intColumns > 0 Then

                                Try
                                    ret = True

                                    ' Prepara el datatable de partes
                                    Dim daPartes As DbDataAdapter = VSL_CreateDataAdapter_Partes(Me.State)

                                    ' Prepara el datatable para identificar al empleado
                                    Dim daEmployee As DbDataAdapter = VSL_CreateDataAdapter_idEmployee(Me.State)

                                    ' Prepara el datatable de DailySchedule
                                    Dim daDailySchedule As DbDataAdapter = VSL_CreateDataAdapter_DailySchedule(Me.State)

                                    ' Prepara el datatable de DailyCauses
                                    Dim daDailyCauses As DbDataAdapter = VSL_CreateDataAdapter_DailyCauses(Me.State)

                                    ' Prepara el command de Causes
                                    Dim dtCauses = CreateDataTable("@SELECT# id,Name,ShortName from causes", "Causes")

                                    ' Recorremos toda la hoja excel
                                    For intRow As Integer = intBeginLine To intEndLine
                                        If Me.GetDataExcelVSLPartes(intRow, intColumns, ColumnsPos, ColumnsVal, Me.State) Then
                                            ret = False

                                            ' Selecciona el empleado
                                            Dim tbE As New DataTable("Employees")
                                            daEmployee.SelectCommand.Parameters("@idParte").Value = ColumnsVal(VSL_WorkSheetsExcelColumns.EmpleadoID)
                                            daEmployee.Fill(tbE)
                                            If tbE.Rows.Count Then
                                                Dim idEmployee As Integer = tbE.Rows(0)("idEmployee")
                                                Dim Diet() As String
                                                Dim DietasPrimera As Short = 0
                                                Dim DietasMedia As Short = 0
                                                Dim DietasComida As Short = 0
                                                Dim DietasTrans As Short = 0

                                                ' Dietas
                                                Diet = Split(ColumnsVal(VSL_WorkSheetsExcelColumns.Dieta), ";")

                                                For i As Integer = 0 To Diet.Length - 1
                                                    Select Case Diet(i)
                                                        Case "1/1" : DietasPrimera = 1
                                                        Case "1/2" : DietasMedia = 1
                                                        Case "1/3" : DietasComida = 1
                                                        Case "1/4" : DietasTrans = 1
                                                    End Select
                                                Next i

                                                ' Guarda el parte
                                                Dim tb As New DataTable("ESP_PARTESTRABAJO")
                                                daPartes.SelectCommand.Parameters("@Parte").Value = ColumnsVal(VSL_WorkSheetsExcelColumns.Parte)
                                                daPartes.SelectCommand.Parameters("@idEmployee").Value = idEmployee
                                                daPartes.SelectCommand.Parameters("@Fecha").Value = ColumnsVal(VSL_WorkSheetsExcelColumns.Dia)
                                                daPartes.Fill(tb)

                                                Dim oRow As DataRow
                                                If tb.Rows.Count = 0 Then
                                                    oRow = tb.NewRow
                                                    oRow("idEmpleado") = idEmployee
                                                    oRow("Fecha") = ColumnsVal(VSL_WorkSheetsExcelColumns.Dia)
                                                    oRow("Parte") = ColumnsVal(VSL_WorkSheetsExcelColumns.Parte)
                                                    n += 1
                                                Else
                                                    oRow = tb.Rows(0)
                                                    u += 1
                                                End If

                                                oRow("Proyecto") = ColumnsVal(VSL_WorkSheetsExcelColumns.Proyecto)
                                                oRow("HorasNormales") = CDbl(ColumnsVal(VSL_WorkSheetsExcelColumns.HN))
                                                oRow("HorasExtrasNormales") = CDbl(ColumnsVal(VSL_WorkSheetsExcelColumns.HE))
                                                oRow("HorasExtrasFestivo") = CDbl(ColumnsVal(VSL_WorkSheetsExcelColumns.HF))
                                                oRow("HorasExtrasSabados") = CDbl(ColumnsVal(VSL_WorkSheetsExcelColumns.HS))
                                                oRow("KMs") = ColumnsVal(VSL_WorkSheetsExcelColumns.KM)

                                                oRow("DietasPrimera") = DietasPrimera
                                                oRow("DietasMedia") = DietasMedia
                                                oRow("DietasComida") = DietasComida
                                                oRow("DietasTrans") = DietasTrans
                                                oRow("DietasPlusAltura") = IIf(ColumnsVal(VSL_WorkSheetsExcelColumns.PA).ToUpper = "SI", 1, 0)

                                                If tb.Rows.Count <= 0 Then
                                                    tb.Rows.Add(oRow)
                                                End If

                                                daPartes.Update(tb)

                                                ' Actualiza DailyCauses
                                                Call VSL_UpdateDailyCauses(daDailyCauses, dtCauses, idEmployee, oRow("Fecha"), oRow("HorasNormales"), oRow("HorasExtrasNormales"), oRow("HorasExtrasSabados"), oRow("HorasExtrasFestivo"), 0, 0, oRow("DietasPrimera"), oRow("DietasMedia"), oRow("DietasComida"), oRow("DietasPlusAltura"), oRow("DietasTrans"), Me.State)

                                                ' Actualiza DailySchedule
                                                VSL_UpdateDailySchedule(daDailySchedule, idEmployee, oRow("Fecha"), Me.State)
                                            Else
                                                Me.State.Result = DataLinkResultEnum.InvalidEmployee
                                                msgLog &= Me.State.Language.Translate("Import.LogEvent.Import.InvalidEmployee ", "") & " " & intRow & vbNewLine
                                            End If
                                        Else
                                            Me.State.Result = DataLinkResultEnum.SomeRegistersAreInvalidFormat
                                            msgLog &= Me.State.Language.Translate("Import.LogEvent.InvalidFormatOnRegister", "") & " " & intRow & vbNewLine
                                        End If
                                    Next

                                    ' Lanza DailyCauses
                                    roConnector.InitTask(TasksType.DAILYCAUSES)

                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.Finish", "") & Environment.NewLine
                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.NewWorkSheets", "") & ": " & n.ToString & Environment.NewLine
                                    strLogEvent = strLogEvent & Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UpdatedWorkSheets", "") & ": " & u.ToString & Environment.NewLine

                                Catch ex As Exception
                                    Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_ImportWorkSheets")
                                    Me.State.Result = DataLinkResultEnum.Exception
                                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
                                    ret = False
                                End Try

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
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidExcelFile
                    strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.InvalidEXCELFile", "") & vbNewLine
                End If

            Catch ex As Exception
                ret = False
                Me.State.Result = DataLinkResultEnum.Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_ImportWorkSheets")
                strLogEvent = Now.ToString & " --> " & Me.State.Language.Translate("Import.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine

            Finally
                Me.SaveImportLog(roDataLinkImport.eImportGuide.VSL_WorkSheets, strLogEvent & msgLog, , Me.State.IDPassport)
            End Try

            Return ret

        End Function

        Public Function VSL_UpdateDailyCauses(ByVal adpDailyCauses As DbDataAdapter, ByVal dtCauses As DataTable, ByVal mIDEmpleado As String, ByVal Fecha As Date, ByVal HorasNormales As Double, ByVal HorasExtrasNormales As Double, ByVal HorasExtrasSabados As Double, ByVal HorasExtrasFestivos As Double, ByVal BolsaHorasExtrasNormales As Double, ByVal BolsaHorasExtrasFestivos As Double, ByVal DietasPrimera As Double, ByVal DietasMedia As Double, ByVal DietasComida As Double, ByVal DietasPlusAltura As Double, ByVal DietasTrans As Double, ByRef oState As roDataLinkState)
            Dim oCn As DbConnection = Nothing
            Dim bReturn As Boolean = False
            Try

                If adpDailyCauses Is Nothing Then
                    adpDailyCauses = Me.VSL_CreateDataAdapter_DailyCauses(Me.State)
                    dtCauses = CreateDataTable("@SELECT# id,Name,ShortName from causes", "Causes")
                End If

                VSL_UpdateCause(dtCauses, "DIU", adpDailyCauses, mIDEmpleado, Fecha, HorasNormales, Me.State)
                VSL_UpdateCause(dtCauses, "EXA", adpDailyCauses, mIDEmpleado, Fecha, HorasExtrasNormales, Me.State)
                VSL_UpdateCause(dtCauses, "EXB", adpDailyCauses, mIDEmpleado, Fecha, HorasExtrasSabados, Me.State)
                VSL_UpdateCause(dtCauses, "EXC", adpDailyCauses, mIDEmpleado, Fecha, HorasExtrasFestivos, Me.State)
                VSL_UpdateCause(dtCauses, "BE", adpDailyCauses, mIDEmpleado, Fecha, BolsaHorasExtrasNormales, Me.State)
                VSL_UpdateCause(dtCauses, "BEF", adpDailyCauses, mIDEmpleado, Fecha, BolsaHorasExtrasFestivos, Me.State)
                VSL_UpdateCause(dtCauses, "DP", adpDailyCauses, mIDEmpleado, Fecha, DietasPrimera, Me.State)
                VSL_UpdateCause(dtCauses, "DM", adpDailyCauses, mIDEmpleado, Fecha, DietasMedia, Me.State)
                VSL_UpdateCause(dtCauses, "DCT", adpDailyCauses, mIDEmpleado, Fecha, DietasComida, Me.State)
                VSL_UpdateCause(dtCauses, "PA", adpDailyCauses, mIDEmpleado, Fecha, DietasPlusAltura, Me.State)
                VSL_UpdateCause(dtCauses, "TRN", adpDailyCauses, mIDEmpleado, Fecha, DietasTrans, Me.State)

                bReturn = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_UpdateDailyCauses")
                bReturn = False
            Finally
                If oCn IsNot Nothing AndAlso oCn.State = ConnectionState.Open Then oCn.Close()
            End Try

            Return bReturn
        End Function
        Public Function VSL_UpdateDailySchedule(ByVal adpDailySchedule As DbDataAdapter, ByVal mIDEmpleado As Long, ByVal Fecha As Date, ByRef oState As roDataLinkState)
            Dim bReturn As Boolean = False

            Try
                Dim dt As New DataTable
                Dim row As DataRow

                If adpDailySchedule Is Nothing Then
                    adpDailySchedule = Me.VSL_CreateDataAdapter_DailySchedule(Me.State)
                End If


                ' Selecciona el registro en DailySchedule
                adpDailySchedule.SelectCommand.Parameters("@idEmployee").Value = mIDEmpleado
                adpDailySchedule.SelectCommand.Parameters("@Date").Value = Fecha
                adpDailySchedule.Fill(dt)
                'RS.Open("@SELECT# * FROM DailySchedule WHERE IDEmployee=" & EmployeeID & " AND Date=" & Any2Time(moveDate).SQLSmallDateTime, m_Connection, adOpenForwardOnly, adLockOptimistic)

                ' El registro no existe, crea ahora
                If dt.Rows.Count = 0 Then
                    row = dt.NewRow
                    row("IDEmployee") = mIDEmpleado
                    row("Fecha") = Fecha
                Else
                    row = dt.Rows(0)
                End If

                ' Actualiza status
                row("Status") = 65

                ' Guarda cambios
                If dt.Rows.Count = 0 Then dt.Rows.Add(row)
                adpDailySchedule.Update(dt)

                bReturn = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_UpdateDailySchedule")
                bReturn = False
            End Try

            Return bReturn
        End Function
        Private Function VSL_IdCauseGet(ByVal dtCauses As DataTable, ByVal ShortName As String, ByRef oState As roDataLinkState) As Integer
            Dim row() As DataRow
            Dim iRet As Integer = 0

            Try
                row = dtCauses.Select("ShortName='" & ShortName & "'")

                If row.Length > 0 Then iRet = row(0)("id")

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_IdCauseGet")
            End Try

            Return iRet
        End Function
        Private Sub VSL_UpdateCause(ByVal dtCauses As DataTable, ByVal ShortName As String, ByVal adpDailyCauses As DbDataAdapter, ByVal mIDEmpleado As Long, ByVal Fecha As Date, ByVal Value As Double, ByRef oState As roDataLinkState)
            Dim dt As New DataTable
            Dim oCn As DbConnection = Nothing

            Try
                ' Selecciona la justificacione
                Dim idCause As Integer = VSL_IdCauseGet(dtCauses, ShortName, Me.State)
                If idCause = 0 Then Exit Try

                Dim row As DataRow

                If adpDailyCauses Is Nothing Then
                    adpDailyCauses = Me.VSL_CreateDataAdapter_DailyCauses(Me.State)
                End If

                ' Selecciona el registro en DailyCauses
                adpDailyCauses.SelectCommand.Parameters("@idEmployee").Value = mIDEmpleado
                adpDailyCauses.SelectCommand.Parameters("@Date").Value = Fecha
                adpDailyCauses.SelectCommand.Parameters("@idCause").Value = idCause
                adpDailyCauses.Fill(dt)
                'RS.Open("@SELECT# * FROM dailycauses WHERE IDEmployee=" & mIDEmpleado & " AND Date=" & Any2Time(Fecha).SQLSmallDateTime & " and IDCause=" & IDCause & " and Manual =1 and IDRelatedIncidence=0", m_Connection, adOpenDynamic, adLockOptimistic)

                If dt.Rows.Count = 0 Then
                    If Value = 0 Then Exit Try

                    ' Crea el registro
                    row = dt.NewRow
                    row("IDEmployee") = mIDEmpleado
                    row("Date") = Fecha
                    row("IDCause") = idCause
                    row("Value") = Value
                    row("IDRelatedIncidence") = 0
                    row("Manual") = 1
                Else
                    row = dt.Rows(0)
                    If Value = 0 Then
                        row.Delete()
                    Else
                        row("Value") = Value
                    End If
                End If

                ' Actualiza el valor
                If dt.Rows.Count = 0 Then dt.Rows.Add(row)
                adpDailyCauses.Update(dt)


            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_UpdateCause")
            Finally
                If oCn IsNot Nothing AndAlso oCn.State = ConnectionState.Open Then oCn.Close()
            End Try

            dt.Dispose()
        End Sub
        Private Function GetDataExcelVSLPartes(ByVal intRow As Integer, ByVal intColumnas As Integer, ByVal ColumnsPos() As Integer, ByRef ColumnsVal() As String, ByRef oState As roDataLinkState) As Boolean
            Dim bolRet As Boolean = False

            Try

                For n As Integer = 0 To ColumnsVal.Length - 1
                    ColumnsVal(n) = ""
                Next

                For intColumn As Integer = 0 To ColumnsPos.Length - 1
                    ColumnsVal(intColumn) = Me.GetCellValue(intRow, ColumnsPos(intColumn))
                Next

                ' Sustituye el punto por coma
                ColumnsVal(VSL_WorkSheetsExcelColumns.HE) = ColumnsVal(VSL_WorkSheetsExcelColumns.HE).Replace(".", ",")
                ColumnsVal(VSL_WorkSheetsExcelColumns.HN) = ColumnsVal(VSL_WorkSheetsExcelColumns.HN).Replace(".", ",")
                ColumnsVal(VSL_WorkSheetsExcelColumns.HF) = ColumnsVal(VSL_WorkSheetsExcelColumns.HF).Replace(".", ",")
                ColumnsVal(VSL_WorkSheetsExcelColumns.HS) = ColumnsVal(VSL_WorkSheetsExcelColumns.HS).Replace(".", ",")

                If ColumnsVal(VSL_WorkSheetsExcelColumns.EmpleadoNombre).Length > 0 AndAlso ColumnsVal(VSL_WorkSheetsExcelColumns.Dia).Length > 0 Then
                    bolRet = True
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDataExcelVSLPartes")
            End Try

            Return bolRet

        End Function
        Private Function VSL_CreateDataAdapter_Partes(ByRef oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String =
                    "@SELECT# * " &
                    "FROM  ESP_PARTESTRABAJO " &
                    "WHERE Parte=@Parte and IDEmpleado=@idEmployee and Fecha=@Fecha"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@Parte", DbType.Int32)
                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@Fecha", DbType.Date)

                da = CreateDataAdapter(cmd, True)

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_CreateDataAdapter_Partes")
            End Try

            Return da
        End Function

        Private Function VSL_CreateDataAdapter_idEmployee(ByRef oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String =
                    "@SELECT# idEmployee " &
                    "FROM  EmployeeUserFieldValues " &
                    "WHERE Fieldname='idParte' and substring(Value,1,25)=@idParte"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idParte", DbType.String)
                da = CreateDataAdapter(cmd, False)

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_CreateDataAdapter_idEmployee")
            End Try

            Return da
        End Function

        Private Function VSL_CreateDataAdapter_DailyCauses(ByRef oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String =
                    "@SELECT# * " &
                    "FROM DailyCauses " &
                    "WHERE IDEmployee=@IDEmployee AND Date=@Date and IDCause=@IDCause  and Manual=1 and IDRelatedIncidence=0"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@Date", DbType.Date)
                AddParameter(cmd, "@idCause", DbType.Int16)
                da = CreateDataAdapter(cmd, True)

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_CreateDataAdapter_DailyCauses")
            End Try

            Return da
        End Function

        Private Function VSL_CreateDataAdapter_DailySchedule(ByRef oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String =
                    "@SELECT# idEmployee, Date, Status " &
                    "FROM DailySchedule " &
                    "WHERE IDEmployee=@IDEmployee AND Date=@Date"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@Date", DbType.Date)

                da = CreateDataAdapter(cmd, True)

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "VSLSystem::VSL_CreateDataAdapter_DailySchedule")
            End Try

            Return da
        End Function
        Private Function VSL_ValidarColumnasWorkSheets(ByRef ColumnsPos() As Integer, ByRef InvalidColumn As String, ByRef oState As roDataLinkState) As Integer
            Dim intNumColumnas As Integer = -1
            Dim intCol As Integer = 0
            Dim bolIsValid As Boolean = True
            Dim Columna As String

            InvalidColumn = ""

            ' Inicializa columnas
            For intCol = 0 To ColumnsPos.Length - 1
                ColumnsPos(intCol) = -1
            Next

            ' Descompone columnas
            intCol = 0
            Columna = Me.GetCellValueWithoutFormat(0, intCol).ToUpper
            Do While Columna <> "" And bolIsValid
                Select Case Columna
                    Case "OPERARIO" : ColumnsPos(VSL_WorkSheetsExcelColumns.EmpleadoNombre) = intCol
                    Case "DIA" : ColumnsPos(VSL_WorkSheetsExcelColumns.Dia) = intCol
                    Case "PROYECTO" : ColumnsPos(VSL_WorkSheetsExcelColumns.Proyecto) = intCol
                    Case "DIETA" : ColumnsPos(VSL_WorkSheetsExcelColumns.Dieta) = intCol
                    Case "KM" : ColumnsPos(VSL_WorkSheetsExcelColumns.KM) = intCol
                    Case "PA" : ColumnsPos(VSL_WorkSheetsExcelColumns.PA) = intCol
                    Case "HN" : ColumnsPos(VSL_WorkSheetsExcelColumns.HN) = intCol
                    Case "HE" : ColumnsPos(VSL_WorkSheetsExcelColumns.HE) = intCol
                    Case "HF" : ColumnsPos(VSL_WorkSheetsExcelColumns.HF) = intCol
                    Case "HS" : ColumnsPos(VSL_WorkSheetsExcelColumns.HS) = intCol
                    Case "EMPLEADO" : ColumnsPos(VSL_WorkSheetsExcelColumns.EmpleadoID) = intCol
                    Case "PARTE Nº" : ColumnsPos(VSL_WorkSheetsExcelColumns.Parte) = intCol
                    Case Else : bolIsValid = False
                End Select

                If bolIsValid Then
                    intCol += 1
                    Columna = Me.GetCellValueWithoutFormat(0, intCol)
                Else
                    InvalidColumn = Columna
                End If
            Loop

            If bolIsValid Then
                If Not (ColumnsPos(VSL_WorkSheetsExcelColumns.EmpleadoNombre) = -1 Or ColumnsPos(VSL_WorkSheetsExcelColumns.Dia) = -1 Or
                   ColumnsPos(VSL_WorkSheetsExcelColumns.Proyecto) = -1 Or ColumnsPos(VSL_WorkSheetsExcelColumns.Dieta) = -1 Or
                  ColumnsPos(VSL_WorkSheetsExcelColumns.KM) = -1 Or ColumnsPos(VSL_WorkSheetsExcelColumns.PA) = -1 Or
                  ColumnsPos(VSL_WorkSheetsExcelColumns.HN) = -1 Or ColumnsPos(VSL_WorkSheetsExcelColumns.HE) = -1 Or
                  ColumnsPos(VSL_WorkSheetsExcelColumns.HF) = -1 Or ColumnsPos(VSL_WorkSheetsExcelColumns.HS) = -1 Or
                  ColumnsPos(VSL_WorkSheetsExcelColumns.EmpleadoID) = -1 Or ColumnsPos(VSL_WorkSheetsExcelColumns.Parte) = -1) Then
                    intNumColumnas = intCol
                Else
                    InvalidColumn = "Required Fields"
                End If
            End If

            Return intNumColumnas
        End Function

    End Class


End Namespace