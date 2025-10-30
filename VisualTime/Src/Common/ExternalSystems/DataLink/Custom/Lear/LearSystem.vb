Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.VTBase

Namespace Lear
    Public Class LearSystem
        Private Shared oLog As New roLog("VTExternalSystem")

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
                If Value <> 0 Or SaveWithZeroValue = True Then
                    sSQL = "@INSERT# INTO dailyAccruals (IDEmployee, Date, IDConcept, Value) values (" & idEmployee & "," & roTypes.Any2Time(BeginDate).SQLDateTime &
                           ", " & id & "," & Replace(Value, ",", ".") & ")"
                    ExecuteScalar(sSQL)
                End If

                bolRet = True

            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "ERROR:Lear::InsertAccrual: ", ex)
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Shared Function LEAR_Pagos(ByVal drEmployee As DataRow, ByVal RegistrosAExportar As DataTable, ByVal BeginDateBreak As Date, ByVal EndDateBreak As Date, ByVal BeginDatePer As Date, ByVal EndDatePer As Date, ByVal strExcelFileName As String) As String
            Dim bolRet As Boolean = False
            Dim ErrMsg As String = ""

            Try
                Dim bBeginDate As Date
                Dim bEndDate As Date
                Dim cBeginDate As Date
                Dim cEndDate As Date

                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::LEAR_Pagos:Start")

                '' Borra todos los saldos del empleado sin composicion
                Dim sSQL As String = "@DELETE# FROM dailyaccruals where IDConcept not in (@SELECT# IDConcept from ConceptCauses) and idEmployee=" & drEmployee("ID") &
                    " and date between " & roTypes.Any2Time(BeginDateBreak).SQLDateTime & " and " & roTypes.Any2Time(EndDateBreak).SQLDateTime
                ExecuteScalar(sSQL)

                ' Determina las fechas iniciales y finales en función del la fecha de rotura o del periodo de nomina
                bBeginDate = IIf(BeginDatePer < BeginDateBreak, BeginDateBreak, BeginDatePer)
                bEndDate = IIf(EndDatePer < EndDateBreak, EndDatePer, EndDateBreak)


                ' Determina el contrato
                sSQL = "@SELECT# idContract, BeginDate, EndDate from EmployeeContracts " &
                       "where idEmployee=" & drEmployee("ID") & " and " & roTypes.Any2Time(BeginDateBreak).SQLDateTime & " Between BeginDate and EndDate"
                Dim dtContract As DataTable = CreateDataTable(sSQL, "Contracts")
                Dim idContract As String = roTypes.Any2String(ExecuteScalar(sSQL))

                ' Determina las fechas iniciales y finales en función del la fecha de contrato o del periodo de nomina
                cBeginDate = IIf(BeginDatePer < dtContract.Rows(0)("BeginDate"), dtContract.Rows(0)("BeginDate"), BeginDatePer)
                cEndDate = IIf(EndDatePer < dtContract.Rows(0)("EndDate"), EndDatePer, dtContract.Rows(0)("EndDate"))

                ' AJUSTE A PAGOS
                bolRet = LEAR_CalculoPagos(strExcelFileName, RegistrosAExportar, drEmployee("ID"), cBeginDate, cEndDate, EndDatePer < dtContract.Rows(0)("EndDate"), ErrMsg)

            Catch ex As Exception
                ErrMsg = "ERROR::LEAR_Pagos: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::LEAR_Pagos: ", ex)
            End Try

            Return IIf(bolRet, "", "ERROR:" & ErrMsg)
        End Function

        Private Shared Function LEAR_CalculoPagos(ByVal templateName As String, ByRef ExportedFieldsObj As Object, ByVal idEmployee As Integer, ByVal StartDate As Date, ByVal EndDate As Date, ByVal fullPeriod As Boolean, ByRef ErrMsg As String) As Boolean
            Dim bolRet As Boolean = True
            Dim Concept51 As Double = 0
            Dim Concept55 As Double = 0
            Dim Concept1301 As Double = 0
            Dim Concept1306 As Double = 0


            Try
                Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "% Rto", EndDate, New UserFields.roUserFieldState)
                If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then
                    ' 51 Coeficiente de rendimiento
                    Concept51 = roTypes.Any2Double(oUserField.FieldValue.ToString.Replace(".", roConversions.GetDecimalDigitFormat()))
                End If

                oUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(idEmployee, "% Estimulo", EndDate, New UserFields.roUserFieldState)
                If oUserField IsNot Nothing AndAlso oUserField.FieldValue IsNot Nothing Then
                    ' 55 Estimulo
                    Concept55 = roTypes.Any2Double(oUserField.FieldValue.ToString.Replace(".", roConversions.GetDecimalDigitFormat()))
                End If

                ' 1301 N horas incentivo
                Dim sSQL As String = "@SELECT# DATES.Date, EC.idEmployee, EC.idContract, ISNULL(DS.IDShiftUsed, DS.IDShift1) AS IDShift, " &
                        " (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = " & idEmployee.ToString & " AND " &
                        " EmployeeUserFieldValues.FieldName = 'Horas paro' AND EmployeeUserFieldValues.Date <= DATES.Date " &
                        " ORDER BY EmployeeUserFieldValues.Date DESC) HorasParo, " &
                        " (@SELECT# TOP 1 Value FROM EmployeeUserFieldValues WHERE EmployeeUserFieldValues.IDEmployee = " & idEmployee.ToString & " AND " &
                        " EmployeeUserFieldValues.FieldName = '% Rto' AND EmployeeUserFieldValues.Date <= DATES.Date " &
                        " ORDER BY EmployeeUserFieldValues.Date DESC) Rendimiento  " &
                       "from dbo.ExplodeDates('" & Format(StartDate, "yyyyMMdd") & "','" & Format(EndDate, "yyyyMMdd") & "') as DATES " &
                       "    Left JOIN EmployeeContracts EC on EC.idEmployee=" & idEmployee.ToString & " and DATES.Date between BeginDate and EndDate " &
                       "    Left JOIN DailySchedule DS on DS.IDEmployee=EC.idEmployee and Dates.Date=DS.date " &
                       "where not EC.idContract is null " &
                       "order by ec.idContract,dates.date"

                Dim tb As DataTable = CreateDataTable(sSQL, "Pagos")
                Dim TotalHorasCien As Double = 0
                Dim TotalPre As Double = 0
                Dim TotalParo As Double = 0
                Dim Estimulo As Double = 0


                For Each aRow As DataRow In tb.Rows
                    ' Obtenemos el saldo de presencia PRE
                    sSQL = "@SELECT# isnull(SUM(value), 0) AS TotalValue from DailyAccruals, Concepts WHERE IDEmployee = " & roTypes.Any2String(idEmployee) & " AND " &
                           "DailyAccruals.IDConcept = Concepts.ID AND Shortname in('PRE') AND " &
                            "Date =" & roTypes.Any2Time(aRow("Date")).SQLSmallDateTime & " GROUP BY ShortName"

                    Dim PreConcept As Double = roTypes.Any2Double(ExecuteScalar(sSQL))
                    TotalPre += PreConcept

                    ' Campo paro
                    Dim Paro As Double = roTypes.Any2Double(aRow("HorasParo").ToString.Replace(".", roConversions.GetDecimalDigitFormat()))
                    If PreConcept < 8 Then
                        Paro = 0
                    End If

                    TotalParo += Paro

                    ' Rendimiento
                    Dim Rendimiento As Double = roTypes.Any2Double(aRow("Rendimiento").ToString.Replace(".", roConversions.GetDecimalDigitFormat()))
                    If Rendimiento > 0 Then
                        TotalHorasCien += Math.Round(((PreConcept - Paro) * (Rendimiento / 100)), 4)
                    End If
                Next
                Concept1301 = TotalHorasCien - TotalPre + TotalParo
                ' en el caso que sea negativo lo dejamos a 0
                If Concept1301 < 0 Then
                    Concept1301 = 0
                End If

                ' 1306 N horas estimulo
                Concept1306 = 0
                If Concept55 > 0 Then
                    Concept1306 = TotalHorasCien * (Concept55 / 100)
                End If

                ' Insertamos los valores de los saldos virtuales
                InsertAccrual(idEmployee, "51", StartDate, EndDate, Concept51, False)
                InsertAccrual(idEmployee, "55", StartDate, EndDate, Concept55, False)
                InsertAccrual(idEmployee, "1301", StartDate, EndDate, Concept1301, False)
                InsertAccrual(idEmployee, "1306", StartDate, EndDate, Concept1306, False)


            Catch ex As Exception
                ErrMsg = "ERROR::LEAR_CalculoPagos: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::LEAR_CalculoPagos: ", ex)
                bolRet = False
            End Try

            Return bolRet
        End Function
    End Class
End Namespace
