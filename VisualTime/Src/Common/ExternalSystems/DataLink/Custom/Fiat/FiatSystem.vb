Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.VTBase

Namespace Fiat
    Public Class FiatSystem
        Private Shared oLog As New roLog("VTExternalSystem")

#Region "Common"
        Private Shared Function InsertAccrual(ByVal idEmployee As Integer, ByVal ConceptExport As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Value As Double, ByVal SaveWithZeroValue As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim id As Integer

                ' Busca el id del saldo
                'Dim sSQL As String = "@SELECT# id from Concepts where name like '" & idConcept & "%'"
                Dim sSQL As String = "@SELECT# id from Concepts where Export ='" & ConceptExport & "'"
                id = ExecuteScalar(sSQL)
                If id = 0 Then Exit Try

                ' Borra el saldo entre las fechas
                sSQL = "@DELETE# FROM dailyAccruals where idEmployee=" & idEmployee & " and idConcept=" & id & " and date between " & roTypes.Any2Time(BeginDate).SQLDateTime & " and " & roTypes.Any2Time(EndDate).SQLDateTime
                ExecuteScalar(sSQL)

                ' Inserta el saldo
                If Value Or SaveWithZeroValue = True Then
                    sSQL = "@INSERT# INTO dailyAccruals (IDEmployee, Date, IDConcept, Value) values (" & idEmployee & "," & roTypes.Any2Time(BeginDate).SQLDateTime &
                           ", " & id & "," & Replace(Value, ",", ".") & ")"
                    ExecuteScalar(sSQL)
                End If

                bolRet = True

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "ERROR::InsertAccrual: ", ex)
                bolRet = False
            End Try

            Return bolRet
        End Function

        Private Shared Function GetIDField(ByVal FieldName As String, ByVal afield As Generic.List(Of ProfileExportFields)) As Integer
            Dim iRet As Integer = -1

            For i As Integer = 0 To afield.Count - 1
                If afield(i).Source.ToUpper = FieldName.ToUpper Then
                    iRet = i
                    Exit For
                End If
            Next

            Return iRet
        End Function
#End Region

#Region "Public Methods"
        Public Shared Function FIAT_InfoTipo(ByVal drEmployee As DataRow, ByVal RegistrosAExportar As DataTable, ByVal BeginDateBreak As Date, ByVal EndDateBreak As Date, ByVal BeginDatePer As Date, ByVal EndDatePer As Date, ByVal strExcelFileName As String) As String
            Dim bolRet As Boolean = False
            Dim ErrMsg As String = ""

            Try
                Dim bBeginDate As Date
                Dim bEndDate As Date
                Dim cBeginDate As Date
                Dim cEndDate As Date

                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo:Start")

                ' Comprueba que es FIAT
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState(), Nothing)
                If oAdvParam.Value.ToUpper <> "TAIF" Then
                    ErrMsg = "Script not authorized"
                    Exit Try
                End If

                ' Borra todos los saldos del empleado sin composicion
                Dim sSQL As String = "@DELETE# FROM dailyaccruals where IDConcept not in (@SELECT# IDConcept from ConceptCauses) and idEmployee=" & drEmployee("ID") &
                    " and date between " & roTypes.Any2Time(BeginDateBreak).SQLDateTime & " and " & roTypes.Any2Time(EndDateBreak).SQLDateTime
                ExecuteScalar(sSQL)

                ' Determina las fechas iniciales y finales en función del la fecha de rotura o del periodo de nomina
                bBeginDate = IIf(BeginDatePer < BeginDateBreak, BeginDateBreak, BeginDatePer)
                bEndDate = IIf(EndDatePer < EndDateBreak, EndDatePer, EndDateBreak)

                ' Lee la division a la que pertenece el empleado 
                Dim GrupoProfesional As String = ""
                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(drEmployee("ID"), "125GRUPO PROFESIONAL", bEndDate, New UserFields.roUserFieldState, False)
                If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then GrupoProfesional = oUserField.FieldValue.ToString

                ' Determina el contrato
                sSQL = "@SELECT# idContract, BeginDate, EndDate from EmployeeContracts " &
                       "where idEmployee=" & drEmployee("ID") & " and " & roTypes.Any2Time(BeginDateBreak).SQLDateTime & " Between BeginDate and EndDate"
                Dim dtContract As DataTable = CreateDataTable(sSQL, "Contracts")
                Dim idContract As String = roTypes.Any2String(ExecuteScalar(sSQL))

                ' Determina las fechas iniciales y finales en función del la fecha de contrato o del periodo de nomina
                cBeginDate = IIf(BeginDatePer < dtContract.Rows(0)("BeginDate"), dtContract.Rows(0)("BeginDate"), BeginDatePer)
                cEndDate = IIf(EndDatePer < dtContract.Rows(0)("EndDate"), EndDatePer, dtContract.Rows(0)("EndDate"))

                ' Determina los días con contrato del periodo seleccionado
                sSQL = "@SELECT# count(idEmployee) as total from sysroDailyScheduleByContract " &
                       "where idEmployee=" & drEmployee("ID") & " and NumContrato='" & idContract & "' and date between " & roTypes.Any2Time(BeginDatePer).SQLDateTime & " and " & roTypes.Any2Time(EndDatePer).SQLDateTime
                Dim DContr As Integer = roTypes.Any2Integer(ExecuteScalar(sSQL))

                ' Determina si es la última rotura del empleado
                Dim bolFirstBreak As Boolean = FIAT_IsFirstEmployeePeriode(drEmployee("ID"), idContract, BeginDateBreak, RegistrosAExportar)
                Dim bolLastBreak As Boolean = FIAT_IsLastEmployeePeriode(drEmployee("ID"), idContract, BeginDateBreak, RegistrosAExportar)

                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo:Idemployee=" & drEmployee("ID") & ";" & drEmployee("Name") & "; Grupo Profesional=" & GrupoProfesional)

                ' Lo primero calcula los dias de ERE e Inactividad para restarlos de las horas de salario
                If bolFirstBreak Then
                    ' ERE
                    bolRet = FIAT_ERE_INACTIVITY_Process(drEmployee("ID"), cBeginDate, cEndDate, Core.DTOs.ConceptsCodes.SN_ERE, ErrMsg)

                    ' INACTIVITY
                    If bolRet Then bolRet = FIAT_ERE_INACTIVITY_Process(drEmployee("ID"), cBeginDate, cEndDate, Core.DTOs.ConceptsCodes.SN_INACTIVIDAD, ErrMsg)
                Else
                    bolRet = True
                End If

                ' HORAS DE SALARIO
                If bolRet Then bolRet = FIAT_HORAS_SALARIO_Process(drEmployee("ID"), GrupoProfesional, bolLastBreak, bBeginDate, bEndDate, cBeginDate, cEndDate, ErrMsg)

                ' PREMIO
                If bolRet Then bolRet = FIAT_Premio(drEmployee("ID"), GrupoProfesional, bBeginDate, bEndDate, BeginDatePer, EndDatePer, ErrMsg)

                ' Estas calculos se aplican al periodo de nomina (del 1 al 31) y no al periodo de la rotura 
                ' (podría haber una rotura por cambio de categoría del 1 al 10 y del 11 al 31)
                If bolLastBreak Then
                    Dim dtLastProlAbs As New DataTable
                    Dim dtLastAus As New DataTable

                    ' Si es un empleado, el mes es de 31 días y existen ausencias para restar hay que restar la última ausencia
                    If FIAT_GrupoProfesionalIsEmployee(GrupoProfesional) AndAlso DContr = 31 Then 'And DAU Then
                        ' Determina cual es la última ausencia prolongada de Enfermedad, ERE o Inactividad
                        dtLastProlAbs = FIAT_GetLastProlonguedAbsence(drEmployee("ID"), cBeginDate, cEndDate, ErrMsg)

                        ' Determina cual es la última ausencia de EmpAusHoras o EmpAusDias
                        dtLastAus = FIAT_GetLastAbsences(drEmployee("ID"), cBeginDate, cEndDate, ErrMsg)

                        ' Determina cual es la última
                        If dtLastProlAbs.Rows.Count And dtLastAus.Rows.Count Then
                            Dim sd As Date = IIf(dtLastProlAbs.Rows(0)("BeginDate") < BeginDatePer, bBeginDate, dtLastProlAbs.Rows(0)("BeginDate"))
                            Dim ed As Date = IIf(dtLastProlAbs.Rows(0)("FinishDate") > EndDatePer, EndDatePer, dtLastProlAbs.Rows(0)("FinishDate"))
                            If ed < dtLastAus.Rows(0)("Date") Then dtLastProlAbs.Rows.Clear()
                        End If
                    End If

                    ' ENFERMEDAD
                    If bolRet Then bolRet = FIAT_IT_Process(drEmployee("ID"), GrupoProfesional, cBeginDate, cEndDate, dtLastProlAbs, ErrMsg)

                    ' AJUSTE A PRIMA
                    If bolRet Then bolRet = FIAT_Ajuste_Prima(strExcelFileName, RegistrosAExportar, drEmployee("ID"), cBeginDate, cEndDate, EndDatePer < dtContract.Rows(0)("EndDate"), ErrMsg)
                End If

            Catch ex As Exception
                ErrMsg = "ERROR::FIAT_InfoTipo: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_InfoTipo: ", ex)

            Finally
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo:End")
            End Try

            Return IIf(bolRet, "", "ERROR:" & ErrMsg)
        End Function

        Public Shared Function FIAT_InfoTipo_Line(ByRef ExportedFieldsObj As Object, ByVal dtRegistrosAExportar As DataTable, ByVal drEmployee As DataRow, ByVal drAccrual As DataRow, ByVal dtConcepts As DataTable, ByVal BeginDate As Date, ByVal EndDate As Date) As String
            Dim ErrorMsg As String = ""
            Dim bolRet As Boolean = False

            Try
                Dim sSQL As String = ""
                Dim id As Integer = -1
                Dim ExportedFields As Generic.List(Of ProfileExportFields) = CType(ExportedFieldsObj, Generic.List(Of ProfileExportFields))

                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo_Line:Start")

                ' Comprueba que es FIAT
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                If oAdvParam.Value.ToUpper <> "TAIF" Then
                    ErrorMsg = "Script not authorized"
                    Exit Try
                End If

                ' Lee el saldo a procesar
                Dim row As DataRow() = dtConcepts.Select("ShortName='" & drAccrual("ShortName") & "'")
                If row.Length = 0 Then Exit Try

                Dim cBeginDate As Date = IIf(drEmployee("BeginDate") < BeginDate, BeginDate, drEmployee("BeginDate"))
                Dim cEndDate As Date = IIf(drEmployee("EndDate") > EndDate, EndDate, drEmployee("EndDate"))

                ' Tratamiento especial para claves
                Select Case row(0)("Export")
                    Case Core.DTOs.ConceptsCodes.SN_ACCIDENTE_2109
                        ' Para cada baja por accidente (composicion del saldo 2109) que empiece en el periodo de nómina y cuyo primer día sea laborable se resta un dia del saldo 2109
                        sSQL = "@SELECT# BeginDate from ProgrammedAbsences inner join causes on idcause=id where idEmployee=" & drEmployee("IDEmployee") &
                           " and (BeginDate between " & roTypes.Any2Time(cBeginDate).SQLDateTime & " and " & roTypes.Any2Time(cEndDate).SQLDateTime &
                           ")  And RelapsedDate is Null and idCause in (@SELECT# idCause from ConceptCauses inner join Concepts on ConceptCauses.IDConcept=Concepts.ID and Concepts.Export  ='" & Core.DTOs.ConceptsCodes.SN_ACCIDENTE_2109 & "')"
                        Dim tb As DataTable = CreateDataTable(sSQL, "Abs2109")
                        For Each aRow As DataRow In tb.Rows
                            If FIAT_GetDiasLaborables(drEmployee("IDEmployee"), aRow("BeginDate"), 1) > 0 Then drAccrual(0) -= 1
                        Next

                        If tb.Rows.Count > 0 Then
                            id = GetIDField("Saldo_V", ExportedFields)
                            If id > 0 Then ExportedFields.Item(id).Value = drAccrual(0)
                        End If

                    Case Core.DTOs.ConceptsCodes.SN_ACCIDENTE_2110
                        ' Para cada baja por accidente (composicion del saldo 2109) que empiece en el periodo de nómina se resta un dia del saldo 2110
                        sSQL = "@SELECT# Count(*) as Total from ProgrammedAbsences inner join causes on idcause=id where idEmployee=" & drEmployee("IDEmployee") &
                           " and (BeginDate between " & roTypes.Any2Time(cBeginDate).SQLDateTime & " and " & roTypes.Any2Time(cEndDate).SQLDateTime &
                           ")  And RelapsedDate is Null and idCause in (@SELECT# idCause from ConceptCauses inner join Concepts on ConceptCauses.IDConcept=Concepts.ID and Concepts.Export  ='" & Core.DTOs.ConceptsCodes.SN_ACCIDENTE_2109 & "')"
                        Dim Saldo2110 As Double = roTypes.Any2Double(ExecuteScalar(sSQL))
                        If Saldo2110 > 0 Then
                            drAccrual(0) -= Saldo2110
                            id = GetIDField("Saldo_V", ExportedFields)
                            If id > 0 Then ExportedFields.Item(id).Value = drAccrual(0)
                        End If
                End Select

                ' Si es un mes de 31 días, un empleado y el saldo es tipo #EMPAUSHORAS# o #EMPAUSDIAS= tiene que quitar la última ausencia que tuviera
                ' Si no es ERE, INACTIVIDAD O IT porque ya han sido tratadas en la cabecera
                Dim bolProcesar As Boolean = (InStr(roTypes.Any2String(row(0)("Description")), "#EMPAUSHORAS#") > 0) OrElse (InStr(roTypes.Any2String(row(0)("Description")), "#EMPAUSDIAS=") > 0)

                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo_Line:Idemployee=" & drEmployee("IDEmployee") & ";" & drEmployee("EmployeeName") & "; Saldo=" & drAccrual("ShortName"))

                If bolProcesar AndAlso System.DateTime.DaysInMonth(EndDate.Year, EndDate.Month) = 31 AndAlso drAccrual("ShortName") <> Core.DTOs.ConceptsCodes.SN_ENFERMEDAD And drAccrual("ShortName") <> Core.DTOs.ConceptsCodes.SN_ERE And drAccrual("ShortName") <> Core.DTOs.ConceptsCodes.SN_INACTIVIDAD Then
                    ' Lee el Grupo Profesional al que pertenece el empleado 
                    Dim GrupoProfesional As String = ""
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(drEmployee("IDEmployee"), "125GRUPO PROFESIONAL", cEndDate, New UserFields.roUserFieldState, False)
                    If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then GrupoProfesional = oUserField.FieldValue.ToString

                    oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo_Line:Idemployee=" & drEmployee("IDEmployee") & ";" & drEmployee("EmployeeName") & "; Grupo Profesional=" & GrupoProfesional)

                    ' Si es un empleado
                    If FIAT_GrupoProfesionalIsEmployee(GrupoProfesional) Then
                        ' Determina los días con contrato del periodo seleccionado
                        sSQL = "@SELECT# count(idEmployee) as total from sysroDailyScheduleByContract " &
                               "where idEmployee=" & drEmployee("IDEmployee") & " and NumContrato='" & drEmployee("idContract") & "' and date between " & roTypes.Any2Time(cBeginDate).SQLDateTime & " and " & roTypes.Any2Time(cEndDate).SQLDateTime
                        Dim DContr As Integer = roTypes.Any2Integer(ExecuteScalar(sSQL))

                        ' Si tiene 31 dias de contrato
                        If DContr = 31 Then
                            Dim dtLastAus As New DataTable
                            Dim dtLastProlAbs As New DataTable

                            ' Determina cual es la última ausencia prolongada de Enfermedad, ERE o Inactividad
                            dtLastProlAbs = FIAT_GetLastProlonguedAbsence(drEmployee("IDEmployee"), cBeginDate, cEndDate, ErrorMsg)

                            ' Determina cual es la última ausencia de EmpAusHoras o EmpAusDias
                            dtLastAus = FIAT_GetLastAbsences(drEmployee("IDEmployee"), cBeginDate, cEndDate, ErrorMsg)

                            ' Determina cual es la última
                            If dtLastProlAbs.Rows.Count > 0 And dtLastAus.Rows.Count > 0 Then
                                Dim sd As Date = IIf(dtLastProlAbs.Rows(0)("BeginDate") < BeginDate, BeginDate, dtLastProlAbs.Rows(0)("BeginDate"))
                                Dim ed As Date = IIf(dtLastProlAbs.Rows(0)("FinishDate") > EndDate, EndDate, dtLastProlAbs.Rows(0)("FinishDate"))
                                If ed > dtLastAus.Rows(0)("Date") Then dtLastAus.Rows.Clear()
                            End If

                            ' Si existe una ausencia determina si es la del registro y le resta 1
                            If dtLastAus.Rows.Count Then
                                If drAccrual("ShortName") = dtLastAus.Rows(0)("ShortName") Then
                                    If InStr(roTypes.Any2String(row(0)("Description")), "#EMPAUSHORAS#") > 0 Then
                                        ' #EMPAUSHORAS#                                      
                                        drAccrual(0) -= 1
                                    Else
                                        ' #EMPAUSDIAS
                                        Dim ShortName As String = ""
                                        Dim i As Integer = InStr(roTypes.Any2String(row(0)("Description")), "#EMPAUSDIAS=")
                                        Dim j As Integer = InStr(i + 12, roTypes.Any2String(row(0)("Description")), "#")
                                        If i <> 0 And j <> 0 Then ShortName = row(0)("Description").ToString.Substring(i + 11, j - i - 12)

                                        ' Selecciona el saldo de horas indicado para restarle horas
                                        If ShortName <> "" Then
                                            sSQL = "@SELECT# Sum(Value) as Total from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                                                   "where idEmployee=" & drEmployee("IDEmployee") & " and date between " & roTypes.Any2Time(cBeginDate).SQLDateTime & " and " & roTypes.Any2Time(cEndDate).SQLDateTime &
                                                   " and shortname='" & ShortName & "' and carryover=0 and startupvalue=0 "
                                            Dim Value As Double = roTypes.Any2Double(ExecuteScalar(sSQL))
                                            Dim Pje As Double = 0

                                            ' Lee el pje de dedicación al que pertenece el empleado                
                                            Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                                            oUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(drEmployee("IDEmployee"), "117P DEDICACIÓN", cEndDate, New UserFields.roUserFieldState, False)
                                            If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then Pje = roTypes.Any2Double(oUserField.FieldValue.ToString.Replace(".", oInfo.CurrencyDecimalSeparator)) / 100

                                            ' Al valor del saldo le resta 8 horas multiplicadas por el pje de dedicación
                                            Value -= 8 * Pje
                                            If Value <= 0 Then Value = 0

                                            ' Asigna el valor
                                            id = GetIDField("Saldo_V", ExportedFields)
                                            If id > 0 Then ExportedFields.Item(id).Value = Value

                                            id = GetIDField("Saldo_Tipo", ExportedFields)
                                            If id > 0 Then ExportedFields.Item(id).Value = "H"
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If

                ' Los saldos que se multiplican por 8 son los que incluyen en la descripcion el texto #SALDOX8#
                If InStr(roTypes.Any2String(row(0)("Description")), "#SALDOX8#") Then
                    id = GetIDField("Saldo_V", ExportedFields)
                    If id > 0 Then ExportedFields.Item(id).Value = drAccrual(0) * 8

                    id = GetIDField("Saldo_Tipo", ExportedFields)
                    If id > 0 Then ExportedFields.Item(id).Value = "H"
                End If

                bolRet = True

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_InfoTipo_Line: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_InfoTipo_Line: ", ex)

            Finally
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_InfoTipo_Line:End")
            End Try

            Return IIf(bolRet, "", "ERROR:" & ErrorMsg)
        End Function

        Public Shared Function FIAT_CalculateISSEAccrualValue(ByVal drEmployee As DataRow, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal Nivel0 As String) As String
            Dim bolRet As Boolean = True
            Dim ErrMsg As String = ""
            Dim Result As String = ""
            Dim strAccrualValue As String = "0000000"
            Try
                ' Comprueba que es FIAT
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                If oAdvParam.Value.ToUpper <> "TAIF" Then
                    ErrMsg = "Script not authorized"
                    Exit Try
                End If

                Dim strSQL As String = ""

                If Nivel0.ToUpper.Contains("MADRID") Then
                    strSQL = "@SELECT# sum(Value) from DailyAccruals where IDEmployee = " & drEmployee("ID") & " and IDConcept = (@SELECT# ID From Concepts where ShortName = '15M') AND Date between '" & BeginDate.ToString("yyyyMMdd") & "' and '" & EndDate.ToString("yyyyMMdd") & "'"
                Else
                    strSQL = "@SELECT# sum(Value) from DailyAccruals where IDEmployee = " & drEmployee("ID") & " and IDConcept = (@SELECT# ID From Concepts where ShortName = '15V') AND Date between '" & BeginDate.ToString("yyyyMMdd") & "' and '" & EndDate.ToString("yyyyMMdd") & "'"
                End If

                Dim obj As Object = ExecuteScalar(strSQL)

                If Not IsDBNull(obj) Then
                    Dim accValue As Double = Val(obj)
                    strAccrualValue = accValue.ToString("F2")
                    strAccrualValue = strAccrualValue.Replace(",", ".")
                End If

                strAccrualValue = "0000000" & strAccrualValue
                strAccrualValue = strAccrualValue.Substring(strAccrualValue.Length - 7)
            Catch ex As Exception
                Result = "ERROR::FIAT_CalculateISSEAccrualValue: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_CalculateISSEAccrualValue: ", ex)
                bolRet = False
            End Try

            Result = IIf(bolRet, strAccrualValue, "ERROR:" & ErrMsg)
            Return Result

        End Function

        Public Shared Function FIAT_PlantCode(ByVal Nivel0 As String) As String
            Dim bolRet As Boolean = True
            Dim ErrMsg As String = ""
            Dim Result As String = ""
            Dim strPlantCode As String = "0000000"
            Try
                ' Comprueba que es FIAT
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                If oAdvParam.Value.ToUpper <> "TAIF" Then
                    ErrMsg = "Script not authorized"
                    Exit Try
                End If

                Dim strSQL As String = ""

                If Nivel0.ToUpper.Contains("MADRID") Then
                    strPlantCode = "0165"
                Else
                    strPlantCode = "3165"
                End If
            Catch ex As Exception
                Result = "ERROR::FIAT_PlantCode: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_PlantCode: ", ex)
                bolRet = False
            End Try

            Result = IIf(bolRet, strPlantCode, "ERROR:" & ErrMsg)
            Return Result

        End Function

        Public Shared Function FIAT_SIMENU_Line(ByRef ExportedFieldsObj As Object, ByVal dtRegistrosAExportar As DataTable, ByVal drEmployee As DataRow, ByVal drAccrual As DataRow, ByVal dtConcepts As DataTable, ByVal BeginDate As Date, ByVal EndDate As Date) As String
            Dim errorMsg = ""
            Dim bolRet = False

            Try
                ' Crea el adaptador para seleccionar el primer y último fichaje del día
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_SIMENU_Line:Start")

                ' Comprueba que es FIAT
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                If oAdvParam.Value.ToUpper <> "TAIF" Then
                    errorMsg = "Script not authorized"
                    Exit Try
                End If

                Dim exportedFields = CType(ExportedFieldsObj, List(Of ProfileExportFields))
                Dim acrrualValue = drAccrual("TotalConcept")
                Dim adpPres As DbDataAdapter
                Dim dtPres As New DataTable
                adpPres = FIAT_CreateDataAdapter_Punches()
                adpPres.SelectCommand.Parameters("@idEmployee").Value = drEmployee("idEmployee")
                adpPres.SelectCommand.Parameters("@ShiftDate").Value = drAccrual("Date")
                adpPres.Fill(dtPres)
                exportedFields(8).Value = Format(roTypes.Any2Double(acrrualValue) * 60, "00000")
                If (dtPres.Rows.Count > 0) Then
                    exportedFields(4).Value = dtPres.Rows(0)("FirstPunch")
                    exportedFields(6).Value = dtPres.Rows(0)("LastPunch")
                End If

            Catch ex As Exception
                errorMsg = "ERROR::FIAT_InfoTipo_Line: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_SIMENU_Line: ", ex)

            Finally
                bolRet = False
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_SIMENU_Line:End")
            End Try

            Return IIf(bolRet, "", "ERROR :" & errorMsg)
        End Function

        Public Shared Function FIAT_ABSENTISMO_Line(ByRef ExportedFieldsObj As Object, ByVal dtRegistrosAExportar As DataTable, ByVal drEmployee As DataRow, ByVal drAccrual As DataRow, ByVal dtConcepts As DataTable, ByVal BeginDate As Date, ByVal EndDate As Date) As String
            Dim errorMsg = ""
            Dim bolRet = True
            Dim dSum As Double = 0

            Try
                ' Comprueba que es FIAT
                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("Customization", New AdvancedParameter.roAdvancedParameterState())
                If oAdvParam.Value.ToUpper <> "TAIF" Then
                    errorMsg = "Script not authorized"
                    Exit Try
                End If

                Dim exportedFields = CType(ExportedFieldsObj, List(Of ProfileExportFields))
                Dim dTeo As Double = exportedFields.Item(2).Value

                For i = 4 To exportedFields.Count - 3
                    If dTeo = 0 Then
                        exportedFields.Item(i).Value = "DIV0"
                    Else
                        exportedFields.Item(i).Value = exportedFields.Item(i).Value * 100 / dTeo
                        dSum = dSum + exportedFields.Item(i).Value
                    End If
                Next

                exportedFields.Item(exportedFields.Count - 2).Value = dSum


            Catch ex As Exception
                bolRet = False
                errorMsg = "ERROR::FIAT_ABSENTISMO_Line: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_ABSENTISMO_Line: ", ex)
            Finally
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_ABSENTISMO_Line:End")
            End Try

            Return IIf(bolRet, "", "ERROR :" & errorMsg)
        End Function

#End Region

#Region "Helper methods"
        Private Shared Function FIAT_GrupoProfesionalIsEmployee(ByVal GrupoProfesional As String) As Boolean
            'Los empleados son todas aquellas personas que tengan en el campo de la ficha “125GRUPO PROFESIONAL”
            'DIR	DIRECTOR	EMPLEADOS
            'ECO	EMPLEADO	
            'MAS	MANDO SUPERIOR	
            'MIL	MANDO INTERMEDIO LIBRE DESIGNACION	
            'MIC	MANDO INTERMEDIO CONVENIO	
            'OPD	OPERARIO DIRECTO	OPERARIOS
            'OPI	OPERARIO INDIRECTO	

            Dim bolRet As Boolean = False

            Try
                bolRet = (GrupoProfesional = "DIR" OrElse GrupoProfesional = "ECO" OrElse GrupoProfesional = "MAS" OrElse GrupoProfesional = "MIL" OrElse GrupoProfesional = "MIC")

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_GrupoProfesionalIsEmployee: ", ex)
            End Try

            Return bolRet
        End Function

        Private Shared Function FIAT_IsLastEmployeePeriode(ByVal idEmployee As Integer, ByVal idContract As String, ByVal breakBeginDate As Date, ByVal RegistrosAExportar As DataTable) As Boolean
            Dim bolLastPeriode As Boolean = True
            Dim Rows As DataRow() = Nothing

            Try
                ' Lee todos los periodos del empleado y contrato
                Rows = RegistrosAExportar.Select("idEmployee=" & idEmployee & " and idContract='" & idContract & "'", "BeginDate")

                If Rows.Length > 1 Then
                    For i As Integer = 0 To Rows.Length - 1
                        If Rows(i)("BeginDate") = breakBeginDate Then
                            bolLastPeriode = (i = Rows.Length - 1)
                            Exit For
                        End If
                    Next i
                End If

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_IsLastEmployeePeriode: ", ex)
            End Try

            Return bolLastPeriode
        End Function

        Private Shared Function FIAT_IsFirstEmployeePeriode(ByVal idEmployee As Integer, ByVal idContract As String, ByVal breakBeginDate As Date, ByVal RegistrosAExportar As DataTable) As Boolean
            Dim bolFirstPeriode As Boolean = True
            Dim Rows As DataRow() = Nothing

            Try
                ' Lee todos los periodos del empleado y contrato
                Rows = RegistrosAExportar.Select("idEmployee=" & idEmployee & " and idContract='" & idContract & "'", "BeginDate")

                If Rows.Length > 1 Then
                    For i As Integer = 0 To Rows.Length - 1
                        If Rows(i)("BeginDate") = breakBeginDate Then
                            bolFirstPeriode = (i = 0)
                            Exit For
                        End If
                    Next i
                End If

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_IsFirstEmployeePeriode: ", ex)
            End Try

            Return bolFirstPeriode
        End Function

        Private Shared Function FIAT_HORAS_SALARIO_Process(ByVal idEmployee As Long, ByVal GrupoProfesional As String, ByVal bolLastBreak As Boolean, ByVal StartDateBreak As Date, ByVal EndDateBreak As Date, ByVal StartDatePer As Date, ByVal EndDatePer As Date, ByRef ErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                ErrorMsg = ""
                Dim sSQL As String = ""
                Dim HorasSalario As Double = 0
                Dim Pje As Double = 0
                Dim Saldo As String = "2001"

                ' Determina el saldo de días de ausencias
                sSQL = "@SELECT# count(distinct(Date)) as Total from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                       "where idEmployee=" & idEmployee & " and date between " & roTypes.Any2Time(StartDateBreak).SQLDateTime & " and " & roTypes.Any2Time(EndDateBreak).SQLDateTime &
                       " and shortname='DAU' and carryover=0 and startupvalue=0 "
                Dim DAU As Double = roTypes.Any2Double(ExecuteScalar(sSQL))

                ' Determina el saldo de horas de ausencia
                sSQL = "@SELECT# Sum(Value) as Total from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                       "where idEmployee=" & idEmployee & " and date between " & roTypes.Any2Time(StartDateBreak).SQLDateTime & " and " & roTypes.Any2Time(EndDateBreak).SQLDateTime &
                       " and Description like '%#RESTAHORAS#%' and carryover=0 and startupvalue=0 "
                Dim RHO As Double = roTypes.Any2Double(ExecuteScalar(sSQL))

                ' Lee el pje de dedicación al que pertenece el empleado                
                Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "117P DEDICACIÓN", EndDateBreak, New UserFields.roUserFieldState, False)
                If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then Pje = roTypes.Any2Double(oUserField.FieldValue.ToString.Replace(".", oInfo.CurrencyDecimalSeparator)) / 100

                ' Calcula el valor
                Dim DiasIntervalo As Integer = DateDiff("d", StartDateBreak, EndDateBreak) + 1

                ' Resta a los dias de salario los dias de ERE o Inactividad 
                sSQL = "@SELECT# Sum (Value) as Total from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                       "where idEmployee=" & idEmployee & " and date between " & roTypes.Any2Time(StartDateBreak).SQLDateTime & " and " & roTypes.Any2Time(EndDateBreak).SQLDateTime &
                       " and Export in ('2152','2165') and carryover=0 and startupvalue=0 "
                Dim ERE As Double = roTypes.Any2Double(ExecuteScalar(sSQL))

                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_HORAS_SALARIO_Process:Idemployee=" & idEmployee & "; Grupo Profesional=" & GrupoProfesional & "; DiasIntervalo;=" & DiasIntervalo & "; ERE=" & ERE & "; Pje=" & Pje.ToString & "; DAU=" & DAU & "; Horas Salario=" & HorasSalario)

                DiasIntervalo -= ERE
                If DiasIntervalo < 0 Then DiasIntervalo = 0

                ' Para cada baja por accidente (composicion del saldo 2109) que empiece en el periodo de nómina se suma un día a las horas de salario
                sSQL = "@SELECT# Count(*) as Total from ProgrammedAbsences inner join causes on idcause=id where idEmployee=" & idEmployee &
                       " and (BeginDate between " & roTypes.Any2Time(StartDateBreak).SQLDateTime & " and " & roTypes.Any2Time(EndDateBreak).SQLDateTime &
                       ") And RelapsedDate is Null and  idCause in (@SELECT# idCause from ConceptCauses inner join Concepts on ConceptCauses.IDConcept=Concepts.ID and Concepts.Export  ='" & Core.DTOs.ConceptsCodes.SN_ACCIDENTE_2109 & "')"
                Dim ACC As Double = roTypes.Any2Double(ExecuteScalar(sSQL))
                DiasIntervalo += ACC

                ' Si es un empleado 
                If bolLastBreak AndAlso FIAT_GrupoProfesionalIsEmployee(GrupoProfesional) Then
                    ' Si el periodo tiene 31 dias resta uno 
                    If DateDiff("d", StartDatePer, EndDatePer) + 1 = 31 Then ' then  DAU <= 1 Then
                        DiasIntervalo -= 1 ' se quita un día 
                        If DAU > 0 Then DAU -= 1 ' si tiene DAU se quita un día
                        If DiasIntervalo < 0 Then DiasIntervalo = 0
                    End If

                    ' Si el periodo tiene todos los dias de febrero suma hasta completar los 30 días
                    If EndDatePer.Month = 2 And DateDiff("d", StartDatePer, EndDatePer) + 1 = DateTime.DaysInMonth(EndDatePer.Year, 2) Then
                        DiasIntervalo += 30 - (DateDiff("d", StartDatePer, EndDatePer) + 1)
                    End If
                End If

                HorasSalario = (8 * Pje * (DiasIntervalo - DAU)) - RHO
                If HorasSalario < 0 Then HorasSalario = 0

                ' Crea el registro                                     
                InsertAccrual(idEmployee, Saldo, StartDateBreak, EndDateBreak, HorasSalario, False)

                bolRet = True

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_HORAS_SALARIO_Process: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_HORAS_SALARIO_Process: ", ex)

            End Try

            Return bolRet
        End Function

        Private Shared Function FIAT_ERE_INACTIVITY_Process(ByVal idEmployee As Long, ByVal StartDate As Date, ByVal EndDate As Date, ByVal ShortNameAccrual As String, ByRef ErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                ErrorMsg = ""

                Dim sSQL As String = ""
                Dim dp As Double = 0
                Dim tds As Double = 0
                Dim tdp As Double = 0
                Dim d As Integer = 0
                Dim StartDay As Date = Nothing
                Dim EndDay As Date = Nothing

                ' Selecciona el saldo correspondiente del empleado entre fechas
                sSQL = "@SELECT# Date, Value from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                       "where idEmployee=" & idEmployee & " and date between " & roTypes.Any2Time(StartDate).SQLDateTime & " and " & roTypes.Any2Time(EndDate).SQLDateTime &
                       " and shortname='" & ShortNameAccrual & "' and carryover=0 and startupvalue=0 " &
                       "order by date"
                Dim dt As DataTable = CreateDataTable(sSQL, "Saldos")
                Dim n As Integer = 0

                For Each Row As DataRow In dt.Rows
                    If n > 0 Then
                        d = DateDiff("d", EndDay, Row("Date"))

                        ' Determina los días seguidos
                        If d = 1 Then
                            EndDay = Row("Date")
                        Else
                            ' Calcula los dias de prestación
                            dp = FIAT_ERE_INACTIVITY_CalculateDays(idEmployee, StartDay, EndDay, ErrorMsg)
                            If dp Then
                                tdp += dp
                                tds += DateDiff("d", StartDay, EndDay) + 1
                            End If

                            n = 0
                        End If
                    End If

                    If n = 0 Then
                        StartDay = Row("Date")
                        EndDay = Row("Date")
                    End If

                    n = n + 1
                Next

                ' Procesa el último intervalo
                If n Then
                    ' Calcula los dias de prestación
                    dp = FIAT_ERE_INACTIVITY_CalculateDays(idEmployee, StartDay, EndDay, ErrorMsg)
                    If dp Then
                        tdp += dp
                        tds += DateDiff("d", StartDay, EndDay) + 1
                    End If

                    ' Redondea por aproximación
                    tdp = Int(tdp + 0.5)

                    ' Los dias transformados no pueden superar 30
                    If tdp > 30 Then tdp = 30

                    ' Crea el registro                 
                    Select Case ShortNameAccrual
                        Case Core.DTOs.ConceptsCodes.SN_ERE
                            InsertAccrual(idEmployee, "2135", StartDate, EndDate, tds, False)
                            InsertAccrual(idEmployee, "2152", StartDate, EndDate, tdp, False)
                            InsertAccrual(idEmployee, "2125", StartDate, EndDate, tdp, False) ' la multiplicacion * 8 se hace en el line

                        Case Core.DTOs.ConceptsCodes.SN_INACTIVIDAD
                            InsertAccrual(idEmployee, "2182", StartDate, EndDate, tdp, False)
                            InsertAccrual(idEmployee, "2175", StartDate, EndDate, tds, False) ' la multiplicacion * 8 se hace en el line
                            InsertAccrual(idEmployee, "2165", StartDate, EndDate, tdp, False) ' la multiplicacion * 8 se hace en el line
                    End Select
                End If

                bolRet = True

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_ERE_INACTIVITY_Calculate: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_ERE_INACTIVITY_Calculate: ", ex)

            End Try

            Return bolRet
        End Function

        Private Shared Function FIAT_ERE_INACTIVITY_GetAccrual(ByRef ShortName As Integer, ByRef ErrorMsg As String) As Integer
            Dim id As Integer = 0

            Try
                ' Determina los saldos 
                Dim sSQL As String = "@SELECT# id from Concepts where ShortName ='" & ShortName & "%'"
                id = ExecuteScalar(sSQL)

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_ERE_INACTIVITY_Calculate: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_ERE_INACTIVITY_Calculate: ", ex)
            End Try

            Return id
        End Function

        Private Shared Function FIAT_ERE_INACTIVITY_CalculateDays(ByVal idEmployee As Long, ByVal StartDay As Date, ByVal EndDay As Date, ByRef ErrorMsg As String) As Double
            Dim dp As Double = 0

            Try
                ErrorMsg = ""

                ' Si el empleado ha empezado antes una IT no calcula el saldo
                Dim sSQL As String = "@SELECT# idEmployee from ProgrammedAbsences inner join causes on idcause=id where idEmployee=" & idEmployee & " and (" & roTypes.Any2Time(StartDay).SQLDateTime &
                                   " between BeginDate and FinishDate) and ShortName IN ('" & Core.DTOs.ConceptsCodes.SN_ENFERMEDAD & "','" & Core.DTOs.ConceptsCodes.SN_ACCIDENTE_NO_LABORAL & "')"

                If (ExecuteScalar(sSQL) <> 0) Then Return 0

                ' Determina días del periodo
                Dim d = DateDiff("d", StartDay, EndDay) + 1

                ' Calcula días de prestacion
                If d = 5 Then
                    dp = 7
                ElseIf d = 7 Then
                    If StartDay.DayOfWeek = DayOfWeek.Monday AndAlso EndDay.DayOfWeek = DayOfWeek.Sunday Then dp = 7
                ElseIf d = 10 Then
                    dp = 13
                ElseIf d = 31 Then
                    dp = 30
                Else
                    dp = d * 1.25
                End If

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_ERE_INACTIVITY_CalculateDays: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_ERE_INACTIVITY_CalculateDays: ", ex)
            End Try

            Return dp
        End Function

        Private Shared Function FIAT_Ajuste_Prima(ByVal templateName As String, ByRef ExportedFieldsObj As Object, ByVal idEmployee As Integer, ByVal StartDate As Date, ByVal EndDate As Date, ByVal fullPeriod As Boolean, ByRef ErrMsg As String) As Boolean
            Dim bolRet As Boolean = True

            Try
                'Obtenemos el fichero de la plantilla
                Dim oSettings As New roSettings
                'Dim strDatalinkPath As String = oSettings.GetVTSetting(eKeys.DataLink)
                'Dim inputFile As String = IO.Path.Combine(strDatalinkPath, templateName)
                Dim inputFile As String = templateName

                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "CNHI AJUSTE A PRIMA", Date.Now, New UserFields.roUserFieldState, False)

                If oUserField IsNot Nothing AndAlso roTypes.Any2Integer(oUserField.FieldValue) = 1 Then
                    Dim ajp As Double = 0

                    Dim isFireman As Boolean = False
                    Dim oUserFieldFireman As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "CNHI Bomberos", Date.Now, New UserFields.roUserFieldState, False)

                    If oUserFieldFireman IsNot Nothing AndAlso roTypes.Any2Integer(oUserFieldFireman.FieldValue) = 1 Then
                        isFireman = True
                    End If


                    If Not isFireman Then
                        Try
                            Dim strSQLNivel0 As String = "@SELECT# top 1 Groups.Name from Groups inner join dbo.GetEmployeeGroupTree('" & idEmployee & "',null,'" & EndDate.ToString("yyyyMMdd") & "') eg on Groups.ID = eg.ID order by Path ASC"
                            Dim level0 As String = roTypes.Any2String(ExecuteScalar(strSQLNivel0))
                            If level0.ToUpper.Contains("MADRID") Then
                                ajp = GetValueFromWorkSheet_Ajuste_Prima(inputFile, 1, idEmployee, EndDate, fullPeriod)
                            Else
                                ajp = GetValueFromWorkSheet_Ajuste_Prima(inputFile, 2, idEmployee, EndDate, fullPeriod)
                            End If
                        Catch ex As Exception
                            ajp = 0
                        End Try
                    Else
                        ajp = FIAT_Calculate_Fireman_Prima(idEmployee, StartDate, EndDate, fullPeriod)
                    End If

                    InsertAccrual(idEmployee, "2009", StartDate, EndDate, ajp, False)
                End If

            Catch ex As Exception
                ErrMsg = "ERROR::FIAT_Ajuste_Prima: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_Ajuste_Prima: ", ex)
                bolRet = False

            End Try

            Return bolRet
        End Function

        Private Shared Function FIAT_Calculate_Fireman_Prima(ByVal idEmployee As Integer, ByVal StartDate As Date, ByVal EndDate As Date, ByVal fullPeriod As Boolean) As Double
            Dim ajpValue As Double = 0

            If Not fullPeriod Then
                ' Si no tiene contrato el valor de ajuste prima es el acumulado de todos los meses * factor -1
                Dim initialStartDate As New Date(EndDate.Year, 1, 1, 0, 0, 0, 0)
                For iRow As Integer = 1 To EndDate.Month
                    Dim tmpEndDate As Date = initialStartDate.AddMonths(1).AddDays(-1)

                    Dim tmpSqlValue As String = "@SELECT# SUM(Value) FROM DailyAccruals WHERE IDConcept = (@SELECT# TOP 1 ID from Concepts where Description like '%#AjustePrimaBomberos%') " &
                    "AND IDEmployee=" & idEmployee & " AND Date BETWEEN '" & initialStartDate.ToString("yyyyMMdd") & "' AND '" & tmpEndDate.ToString("yyyyMMdd") & "'"

                    ajpValue += (156 - roTypes.Any2Double(ExecuteScalar(tmpSqlValue)))

                    initialStartDate = initialStartDate.AddMonths(1)
                Next

                ajpValue = (ajpValue * -1)
            Else
                Dim strAJPValue As String = "@SELECT# SUM(Value) FROM DailyAccruals WHERE IDConcept = (@SELECT# TOP 1 ID from Concepts where Description like '%#AjustePrimaBomberos%') " &
                    "AND IDEmployee=" & idEmployee & " AND Date BETWEEN '" & StartDate.ToString("yyyyMMdd") & "' AND '" & EndDate.ToString("yyyyMMdd") & "'"

                ajpValue = 156 - roTypes.Any2Double(ExecuteScalar(strAJPValue))
            End If

            Return ajpValue
        End Function

        Private Shared Function GetValueFromWorkSheet_Ajuste_Prima(ByVal templateName As String, ByVal intIDWorkSheet As Integer, ByVal idEmployee As Integer, ByVal EndDate As Date, ByVal fullPeriod As Boolean) As Double
            Dim ajpValue As Double = 0
            Dim oExcelProfileBytes As Byte() = Nothing
            oExcelProfileBytes = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(templateName, roLiveDatalinkFolders.templates.ToString, roLiveQueueTypes.datalink, False)
            Dim oExcel As New ExcelExport(oExcelProfileBytes)

            If oExcel.FileIsOK Then
                If Not fullPeriod Then
                    ' Si no tiene contrato el valor de ajuste prima es el acumulado de todos los meses * factor -1
                    For iRow As Integer = 2 To EndDate.Month

                        Dim strValue As String = roTypes.Any2String(oExcel.GetCellValue(iRow, 6, intIDWorkSheet))

                        ajpValue += roTypes.Any2Double(strValue.Replace(",", roConversions.GetDecimalDigitFormat()))
                    Next
                    ajpValue = (ajpValue * -1)
                Else
                    Dim strValue As String = roTypes.Any2String(oExcel.GetCellValue((EndDate.Month + 1), 6, intIDWorkSheet))
                    ' Si tiene contrato el valor de ajuste prima es el de la celda
                    ajpValue = roTypes.Any2Double(strValue.Replace(",", roConversions.GetDecimalDigitFormat()))
                End If
            End If

            Return ajpValue
        End Function

        Private Shared Function FIAT_IT_Process(ByVal idEmployee As Integer, ByVal GrupoProfesional As String, ByVal StartDate As Date, ByVal EndDate As Date, ByVal dtLastProlAbs As DataTable, ByRef ErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                ErrorMsg = ""

                Dim sSQL As String = ""
                Dim StartDay As Date = Nothing
                Dim EndDay As Date = Nothing
                Dim DContr As Integer = 0

                Dim DNatPer As Integer = 0
                Dim DNatIT As Integer = 0
                Dim DNatR As Integer = 0

                Dim TDL_T(3) As Integer
                Dim TDN_T(3) As Integer
                Dim TDIT As Integer = 0

                ' Selecciona las ausencias prolongadas del empleado
                sSQL = "@SELECT# idCause, BeginDate, FinishDate, MaxLastingDays, RelapsedDate, ShortName from ProgrammedAbsences inner join Causes on  ProgrammedAbsences.idCause = Causes.id " &
                       "where idEmployee=" & idEmployee & " and (FinishDate is Null OR not (BeginDate >" & roTypes.Any2Time(EndDate).SQLDateTime & " or FinishDate<" & roTypes.Any2Time(StartDate).SQLDateTime & ")) " &
                       "and ShortName IN ('" & Core.DTOs.ConceptsCodes.SN_ENFERMEDAD & "','" & Core.DTOs.ConceptsCodes.SN_ACCIDENTE_NO_LABORAL & "','" & Core.DTOs.ConceptsCodes.SN_IT_PROLONGADA_ENFERMEDAD & "') " &
                       "order by BeginDate"
                Dim dt As DataTable = CreateDataTable(sSQL, "IT")
                Dim n As Integer = 0
                Dim bolRestadoDia = False

                For Each Row As DataRow In dt.Rows
                    ' Determina la fecha de inicio
                    If Row("BeginDate") < StartDate Then
                        StartDay = StartDate
                    Else
                        StartDay = Row("BeginDate")
                    End If

                    ' Determinal fecha final
                    If IsDBNull(Row("FinishDate")) Then
                        EndDay = EndDate
                    ElseIf Row("FinishDate") > EndDate Then
                        EndDay = EndDate
                    Else
                        EndDay = Row("FinishDate")
                    End If

                    ' Determina los días naturales desde el inicio de la IT hasta el inicio del periodo
                    DNatIT = DateDiff("d", Row("BeginDate"), StartDay.AddDays(-1)) + 1

                    ' Si la ultima ausencia prolongada es de enfermedad no tiene en cuenta el último día
                    If dtLastProlAbs.Rows.Count > 0 AndAlso Not bolRestadoDia Then
                        If dtLastProlAbs.Rows(0)("ShortName") = Core.DTOs.ConceptsCodes.SN_ENFERMEDAD Then
                            EndDay = EndDay.AddDays(-1)
                            bolRestadoDia = True
                        End If
                    End If

                    ' Determina los días naturales de la IT en el periodo seleccionado
                    DNatPer = DateDiff("d", StartDay, EndDay) + 1

                    ' Total días IT del periodo seleccionado
                    TDIT += DNatPer

                    ' Si es un empleado los meses son de 30 días
                    If FIAT_GrupoProfesionalIsEmployee(GrupoProfesional) Then
                        ' Si es la última IT regula los días para que siempre sean 30 días como máximo (febrero y meses con 31 días)
                        If n + 1 = dt.Rows.Count Then
                            If Month(StartDay) = 2 Then
                                ' Febrero
                                If EndDay.Day = 28 Then DNatPer += 2 ' Si está de baja el día 28 añade los dos días hasta completar 30 días naturales. 
                                If EndDay.Day = 29 Then DNatPer += 1 ' Si está de baja el día 29 añade el día hasta completar 30 días naturales.                             
                            End If
                        End If
                    End If


                    DNatR = 0

                    ' Si la ausencia prolongada tiene fecha de recaida determina el tramo en el que se encuentra
                    If Not IsDBNull(Row("RelapsedDate")) AndAlso Row("BeginDate") <> Row("RelapsedDate") Then
                        ' Lee la ausencia prolongada de recaida
                        sSQL = "@SELECT# idCause, BeginDate, FinishDate, MaxLastingDays, RelapsedDate from ProgrammedAbsences " &
                              "where idEmployee=" & idEmployee & " and BeginDate =" & roTypes.Any2Time(Row("RelapsedDate")).SQLDateTime
                        Dim dtR As DataTable = CreateDataTable(sSQL, "ITR")

                        If dtR.Rows.Count > 0 Then
                            If Not IsDBNull(dtR.Rows(0)("FinishDate")) Then
                                ' Días naturales de recaida
                                DNatR = DateDiff("d", dtR.Rows(0)("BeginDate"), dtR.Rows(0)("FinishDate")) + 1
                            End If
                        End If
                    End If

                    ' Descompone los días naturales
                    Dim INI As Integer = 0
                    Dim TDN(3) As Integer

                    INI = DNatIT + DNatR

                    ' Si la justificacion es diferente de IT Enfermedad Prolongada
                    ' descompone en base a los tramos de la ausencia
                    If roTypes.Any2String(Row("ShortName")) <> Core.DTOs.ConceptsCodes.SN_IT_PROLONGADA_ENFERMEDAD Then
                        FIAT_IT_Descompose(INI + 1, INI + DNatPer, TDN(0), TDN(1), TDN(2), TDN(3))
                    Else
                        ' En el caso de IT Enfermedad Prolongada, siempre se acumula en el 4 tramo
                        FIAT_IT_Descompose_IT(INI + 1, INI + DNatPer, TDN(3))
                    End If

                    ' Descompone los dias laborables                    
                    Dim tBeginDate As Date = StartDay

                    ' Tramos
                    For i As Integer = 0 To 3
                        If TDN(i) > 0 Then
                            TDN_T(i) += TDN(i)
                            TDL_T(i) += FIAT_GetDiasLaborables(idEmployee, tBeginDate, TDN(i))
                            tBeginDate = tBeginDate.AddDays(TDN(i))
                        End If
                    Next i

                    n += 1
                Next

                ' Borra el saldo anterior e Inserta los nuevos saldos
                InsertAccrual(idEmployee, "2111", StartDate, EndDate, TDL_T(0), False) ' Días laborables del tramo 1 ' la multiplicacion * 8 se hace en el line
                InsertAccrual(idEmployee, "2113", StartDate, EndDate, TDL_T(1), False) ' Días laborables del tramo 2 ' la multiplicacion * 8 se hace en el line
                InsertAccrual(idEmployee, "2115", StartDate, EndDate, TDL_T(2), False) ' Días laborables del tramo 3 ' la multiplicacion * 8 se hace en el line
                InsertAccrual(idEmployee, "2117", StartDate, EndDate, TDL_T(3), False) ' Días laborables del tramo 4 ' la multiplicacion * 8 se hace en el line

                InsertAccrual(idEmployee, "2112", StartDate, EndDate, TDN_T(0), False) ' Días naturales del tramo 1 ' la multiplicacion * 8 se hace en el line
                InsertAccrual(idEmployee, "2114", StartDate, EndDate, TDN_T(1), False) ' Días naturales del tramo 2 ' la multiplicacion * 8 se hace en el line
                InsertAccrual(idEmployee, "2116", StartDate, EndDate, TDN_T(2), False) ' Días naturales del tramo 3 ' la multiplicacion * 8 se hace en el line
                InsertAccrual(idEmployee, "2118", StartDate, EndDate, TDN_T(3), False) ' Días naturales del tramo 4 ' la multiplicacion * 8 se hace en el line

                bolRet = True

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_IT_Process: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_IT_Process: ", ex)
            End Try

            Return bolRet

        End Function

        Private Shared Function FIAT_GetDiasLaborables(ByVal idEmployee As Long, ByVal BeginDate As Date, ByVal MaxDays As Integer) As Integer
            Dim sSQL As String = ""
            Dim Value As Integer = 0

            Try
                ' Determina los días laborables desde el inicio de la IT hasta el inicio del periodo
                sSQL = "@SELECT# Count(*) as Total from DailySchedule inner join shifts on IDShiftUsed=shifts.ID  " &
                       "where idEmployee=" & idEmployee & " and date between " & roTypes.Any2Time(BeginDate).SQLDateTime & " and " & roTypes.Any2Time(BeginDate.AddDays(MaxDays - 1)).SQLDateTime & " " &
                       "and shifts.ExpectedWorkingHours <>0 "
                Value = roTypes.Any2Integer(ExecuteScalar(sSQL))

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_GetDiasLaborables: ", ex)

            End Try

            Return Value
        End Function

        Private Shared Function FIAT_GetLastProlonguedAbsence(ByVal idEmployee As Long, ByVal StartDate As Date, ByVal EndDate As Date, ByRef ErrorMsg As String) As DataTable
            Dim dt As DataTable = Nothing

            Try
                Dim sSQl As String = ""
                ErrorMsg = ""

                ' Selecciona la última ausencia prolongada del empleado
                sSQl = "@SELECT# shortName, BeginDate, ISNULL(FinishDate, DATEADD(D,maxlastingdays,BeginDate)) AS FinishDate from ProgrammedAbsences inner join Causes on  ProgrammedAbsences.idCause = Causes.id " &
                       "where idEmployee=" & idEmployee & " and (FinishDate is Null OR not (BeginDate >" & roTypes.Any2Time(EndDate).SQLDateTime & " or FinishDate<" & roTypes.Any2Time(StartDate).SQLDateTime & ")) " &
                       "and ShortName in ('" & Core.DTOs.ConceptsCodes.SN_ENFERMEDAD & "','" & Core.DTOs.ConceptsCodes.SN_ERE & "','" & Core.DTOs.ConceptsCodes.SN_INACTIVIDAD & "','" & Core.DTOs.ConceptsCodes.SN_IT_PROLONGADA_ENFERMEDAD & "') " &
                       "order by BeginDate desc"
                dt = CreateDataTable(sSQl, "LastProAbs")

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_GetLastProlonguedAbsence: " & ex.Message

            End Try

            Return dt
        End Function

        Private Shared Function FIAT_GetLastAbsences(ByVal idEmployee As Long, ByVal StartDate As Date, ByVal EndDate As Date, ByRef ErrorMsg As String) As DataTable
            Dim dt As DataTable = Nothing

            Try
                Dim sSQl As String = ""
                ErrorMsg = ""

                ' Selecciona la última ausencia del empleado tipo 'EmpAusHoras' o 'EmpAusDias'
                sSQl = "@SELECT# date,idconcept,value,shortname from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                    "where idEmployee=" & idEmployee & " and (Date between " & roTypes.Any2Time(StartDate).SQLDateTime & " and " & roTypes.Any2Time(EndDate).SQLDateTime & ") " &
                    "and IDConcept in (@SELECT# id from Concepts  where description like '%#EmpAusHoras#%' or description like '%#EmpAusDias%') " &
                    "order by date desc"
                dt = CreateDataTable(sSQl, "LastAbs")

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_GetLastAbsences: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_GetLastAbsences: ", ex)

            End Try

            Return dt
        End Function

        Private Shared Sub FIAT_IT_Descompose(ByVal Ini As Integer, ByVal Fin As Integer, ByRef T1 As Integer, ByRef T2 As Integer, ByRef T3 As Integer, ByRef T4 As Integer)
            For i As Integer = Ini To Fin
                Select Case i
                    Case 1 To 3 : T1 += 1
                    Case 4 To 15 : T2 += 1
                    Case 16 To 20 : T3 += 1
                    Case Else : T4 += 1
                End Select
            Next i
        End Sub

        Private Shared Sub FIAT_IT_Descompose_IT(ByVal Ini As Integer, ByVal Fin As Integer, ByRef T4 As Integer)
            For i As Integer = Ini To Fin
                T4 += 1
            Next i
        End Sub


        Private Shared Function FIAT_Premio(ByVal idEmployee As Long, ByVal GrupoProfesional As String, ByVal StartDate As Date, ByVal EndDate As Date, ByVal StartDatePer As Date, ByVal EndDatePer As Date, ByRef ErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False

            Try
                ErrorMsg = ""

                Dim sSQL As String = ""
                Dim DContr As Integer = 0
                Dim Value As Double = 0
                Dim HorasPremio As Double = 0
                Dim SaldoPremio As Double = 0

                ' Si es un empleado o mando superior cálcula el premio
                If FIAT_GrupoProfesionalIsEmployee(GrupoProfesional) Then
                    ' Lee las horas premio del campo de la ficha                    
                    Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "CNHI HORAS PREMIO", EndDate, New UserFields.roUserFieldState, False)
                    If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then HorasPremio = roTypes.Any2Double(oUserField.FieldValue.ToString.Replace(".", oInfo.CurrencyDecimalSeparator))

                    ' Lee el saldo Premio 
                    sSQL = "@SELECT# sum (Value) as Premio from DailyAccruals inner join concepts on concepts.id=dailyaccruals.idConcept " &
                           "where idEmployee=" & idEmployee & " and date between " & roTypes.Any2Time(StartDate).SQLDateTime & " and " & roTypes.Any2Time(EndDate).SQLDateTime &
                           " and shortname='" & "1PR" & "' and carryover=0 and startupvalue=0 "
                    SaldoPremio = roTypes.Any2Double(ExecuteScalar(sSQL))

                    ' Calcula el valor
                    If StartDate = StartDatePer AndAlso EndDate = EndDatePer Then
                        ' Todo el periodo trabajado
                        Value = SaldoPremio + HorasPremio
                    Else
                        ' Alta o baja
                        Value = SaldoPremio + (HorasPremio / 30) * (DateDiff("d", StartDate, EndDate) + 1)
                    End If

                    ' Los mandos superiores no tienen premio negativo
                    If GrupoProfesional.ToUpper = "MAS" AndAlso Value < 0 Then Value = 0
                End If

                ' Borra el saldo anterior e Inserta el nuevo saldo
                InsertAccrual(idEmployee, "2079", StartDate, EndDate, Value, False)

                bolRet = True

            Catch ex As Exception
                ErrorMsg = "ERROR::FIAT_Premio: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_Premio: ", ex)


            End Try

            Return bolRet

        End Function

        Private Shared Function FIAT_CreateDataAdapter_Punches() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# (@SELECT# min(datetime) from punches where idEmployee=@idEmployee and shiftdate=@ShiftDate and actualtype IN (1,2)) as FirstPunch, " &
                                            "(@SELECT# max(datetime) from punches where idEmployee=@idEmployee and shiftdate=@ShiftDate and actualtype IN (1,2)) as LastPunch "

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@ShiftDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)

            Catch ex As Exception
                Return Nothing
            End Try

            Return da
        End Function
#End Region


    End Class
End Namespace
