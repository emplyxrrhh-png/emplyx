Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Newtonsoft.Json
Imports Robotics.DataLayer
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports SwiftExcel
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Security.Base
Imports Robotics.Base.VTBusiness.DiningRoom
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Azure
Imports System.Activities.Expressions
Imports Robotics.Base.VTSelectorManager
Imports System.Text

Namespace DataLink

    Public Class roCustomTisvol
        Inherits roDataLinkExport

#Region "Exportación Cálculo de Primas - TISVOL"
        ''' <summary>
        ''' Exporta a un excel consolidado las tareas y tiempos calculados de los empleados y rango de fechas seleccionados.
        ''' </summary>
        ''' <param name="mEmployees"></param>
        ''' <param name="dtBeginDate"></param>
        ''' <param name="dtEndDate"></param>
        ''' <param name="IsExc2007"></param>
        ''' <param name="nIdExport"></param>
        ''' <param name="oState"></param>
        ''' <param name="summary"></param>
        ''' <returns>Documento generado</returns>
        ''' <remarks>JP 16/09/2016</remarks>
        Public Shared Function EXCELExportPrimasEstimate(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal IsExc2007 As Boolean, ByVal nIdExport As Integer, ByRef oState As roDataLinkState, ByVal oExcelFileName As String, Optional summary As Boolean = False) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                Dim i As Integer = 0
                Dim SSQL = ""
                Dim beginDate = Any2Time(dtBeginDate).SQLSmallDateTime
                Dim endDate = Any2Time(dtEndDate).SQLSmallDateTime
                Dim NameFile As String
                If summary Then
                    NameFile = "ExportPrimasEstimateSummary#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Else
                    NameFile = "ExportCalculoPrima#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                End If

                Dim excel As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & oState.Language.Translate("EXCELExportPrimasEstimate.LogEvent.Start", "") & vbNewLine
                'Busca el campo de la ficha de la tarea Compartida
                Dim idFieldTask = Any2Double(ExecuteScalar("@SELECT# id from sysroFieldsTask where Name = 'Compartida'"))
                ' Recupero el id del saldo a exportar
                Dim idConcept As Double = Any2Double(ExecuteScalar("@SELECT# id from Concepts where Description like '%#PRIMA%'"))

                If (idFieldTask.Equals(0)) Then
                    Exit Try
                End If

                SSQL = "@SELECT# IDEmployee, e.Name EmployeeName, T.ID, t.InitialTime, t.Name as TaskName, t.ShortName, t.Project, SUM(Value) TotalTime  from DailyTaskAccruals dt
						inner join (@SELECT# ID, NAME, EndDate, (InitialTime + TimeChangedRequirements + EmployeeTime + MaterialTime + ForecastErrorTime + TeamTime + OtherTime + NonProductiveTimeIncidence) InitialTime, Project, ShortName from Tasks
									where Status in ( " & TaskStatusEnum._PENDING & "," & TaskStatusEnum._COMPLETED & ") and convert(date,UpdateStatusDate,120) between " & beginDate & " and " & endDate & " AND Field" & idFieldTask.ToString() & " = 1) t on t.ID = dt.IDTask
						inner join employees e on e.ID = dt.IDEmployee 
						where dt.IDEmployee in (" & mEmployees & ")
						group by IDEmployee,  e.Name, t.id, t.InitialTime, t.Name, t.ShortName, t.Project
						ORDER BY IDEmployee,  e.Name, t.id, t.InitialTime, t.Name, t.ShortName, t.Project"


                'Creo una lista de los registros de la bbdd
                Dim dtEmployeeTasks As DataTable = CreateDataTable(SSQL)
                Dim lstEmployeeTasks = New PrimaEstimatedList()
                Dim eTotalRow = 4
                lstEmployeeTasks.Load(dtEmployeeTasks)
                Dim eRow = 2
                'agrupo por empleado para comenzar la exportación
                For Each item In lstEmployeeTasks.GroupBy(Function(k) New With {Key .IdEmployee = k.IdEmployee, Key .EmployeeName = k.EmployeeName})

                    'campo de la fucha valor punto para cada empleado -->> TRAER EL VALOR A LA FECHA FINAL
                    Dim valorPunto As String = Any2String(ExecuteScalar("@DECLARE# @dateUser date
																		set @dateUser = " & endDate & "
																		select VALUE from GetAllEmployeeUserFieldValue('Valor punto', @dateUser) 
																		WHERE idEmployee=" & item.Key.IdEmployee))

                    Dim valorPuntoDbl = Any2Double(valorPunto.Replace(".", roConversions.GetDecimalDigitFormat))
                    PrimaEstimated.CreateEmployeeHeaderSummary(excel, oState, dtBeginDate, dtEndDate, summary)

                    Dim employeeTotals = New PrimaEstimated.Totals
                    Dim bolIsFirstRecord = True
                    If (Not summary) Then
                        Dim accrualEmployeeTotal As Double = Any2Double(ExecuteScalar("@SELECT# isnull(sum(value),0) from DailyAccruals
						where IDEmployee = " & item.Key.IdEmployee & " and IDConcept = " & idConcept & " and Date between " & beginDate & " and " & endDate))
                        employeeTotals.Accrual = accrualEmployeeTotal
                    End If
                    For Each employeeData In item
                        'TotalTimeJob --> totalTaskTime = total de horas del empleado en esta tarea
                        Dim totalTaskTime As Double = ExecuteScalar("@SELECT# isnull(sum(Value),0) from DailyTaskAccruals where IDTask =" & employeeData.IdTask)
                        'TotalPieces --> totalEmployeeInTask = porcentaje del trabajo de ese empleado en la tarea.
                        Dim totalEmployeeInTask = If(totalTaskTime.Equals(0), 0, employeeData.WorkedHours / totalTaskTime)
                        'TeoricTime --> Tiempo asignado
                        Dim TeoricTime = employeeData.ExpectedHours
                        employeeData.Assigned = TeoricTime * totalEmployeeInTask
                        employeeData.Performance = If(employeeData.WorkedHours.Equals(0), 0, (employeeData.Assigned / employeeData.WorkedHours) * 100)
                        employeeData.HorasPase = employeeData.Assigned - employeeData.WorkedHours
                        If (employeeData.Performance < 133) Then
                            employeeData.Prima = employeeData.HorasPase * valorPuntoDbl
                        Else
                            If (Not employeeData.Performance.Equals(0)) Then
                                employeeData.Prima = valorPuntoDbl * 0.33 * employeeData.WorkedHours
                            End If
                        End If
                        If (employeeData.Prima <= 0.1 And employeeData.Prima >= -0.1) Then employeeData.Prima = 0
                        employeeData.WriteLine(excel, eRow, dtBeginDate, dtEndDate, employeeTotals.Accrual, bolIsFirstRecord)
                        bolIsFirstRecord = False
                        eRow += 1

                    Next
                Next

                ' Graba el archivo
                excel.SaveFile()

                ' Devuelve array de bytes
                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportPrimasEstimate")
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("EXCELExportPrimasEstimate.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("EXCELExportPrimasEstimate.LogEvent.Finish", "") & vbNewLine
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine)
            End Try

            Return arrFile

        End Function

#End Region
#Region "Exportación de fichajes Tisvol"
        Public Shared Function ASCIIExportPunchesTisvol(ByVal strEmployeefilter As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal nIdExport As Integer, ByVal separator As String, ByRef oState As roDataLinkState) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim cn As DbConnection = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""

            Try
                ' Crea el fichero
                Dim NameFile As String = "ExportTISVOLPunches#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".txt"

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & oState.Language.Translate("ASCIIExportPunchesTisvol.LogEvent.Start", "") & vbNewLine

                Dim beginDate = Any2Time(dtBeginDate).SQLSmallDateTime
                Dim endDate = Any2Time(dtEndDate).SQLSmallDateTime

                ' Data adapter para seleccionar contratos
                Dim daPunch As DbDataAdapter
                daPunch = CreateDataAdapter_Punches(oState, strEmployeefilter)

                ' Recupero el id del saldo a exportar
                Dim idConcept As Double = Any2Double(ExecuteScalar("@SELECT# id from Concepts where Description like '%#PUNCH%'"))

                Dim dt As New DataTable
                daPunch.SelectCommand.Parameters("@accrual").Value = idConcept
                daPunch.SelectCommand.Parameters("@BeginDate").Value = dtBeginDate
                daPunch.SelectCommand.Parameters("@EndDate").Value = dtEndDate
                daPunch.Fill(dt)

                'paso el dt a una lista
                Dim employeePunchesList = New EmployeePunchList(dt, oState)
                'creo todas las línes a exportar
                Dim exportLines = employeePunchesList.ToText(oState, separator)

                'vuelca las lineas al fichero
                File.WriteAllLines(NameFile, exportLines)

                ' Devuelve array de bytes
                arrFile = File.ReadAllBytes(NameFile)
                File.Delete(NameFile)

                ' Exportación finalizada
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("ASCIIExportPunchesTisvol.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportPunchesTisvol")

            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine)
            End Try

            Return arrFile
        End Function

        Private Shared Function CreateDataAdapter_Punches(ByRef oState As roDataLinkState, mEmployees As String) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# e.Name, p.ActualType, p.ShiftDate, p.DateTime, s.ShortName, isnull(da.Value,0) Acrrual
										from Punches p
										inner join employees e on p.IDEmployee = e.id 
										inner join DailySchedule d on p.IDEmployee = d.IDEmployee and d.Date = p.ShiftDate
										inner join Shifts s on s.ID = isnull(IDShiftUsed,IDShift1)
										left join DailyAccruals da on p.IDEmployee = da.IDEmployee and da.Date = p.ShiftDate and da.IDConcept = @accrual
										where p.IDEmployee in (" & mEmployees & ")
										and ShiftDate between @BeginDate and @EndDate
										and ActualType in (1,2)
										order by p.idemployee, DateTime"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@accrual", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::CreateDataAdapter_Punches")
            End Try

            Return da
        End Function
#End Region

#Region "Clase auxiliar exportación calculo prima TISVOL"
        Public Class PrimaEstimated
#Region "Propiedades"
            Public Property IdEmployee() As Integer
            Public Property EmployeeName() As String
            Public Property IdTask() As Integer
            Public Property TaskName() As String
            Public Property ExpectedHours() As Double
            Public Property WorkedHours() As Double
            Public Property Assigned() As Double
            Public Property HorasPase() As Double
            Public Property Performance() As Double
            Public Property Prima() As Double
            Public Property EmployeeTotal() As Totals
            Public Property TaskProject As String
            Public Property TaskShortName As String
#End Region
#Region "Métodos"

            Public Shared Sub CreateEmployeeHeaderSummary(ByRef excel As ExcelExport, ByRef oState As roDataLinkState, beginDate As Date, endDate As Date, Optional isSummary As Boolean = False)
                excel.SetCellValue(1, 1, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Employee.Name", ""),,,,,,, True)
                excel.SetCellValue(1, 2, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Project", ""),,,,,,, True)
                excel.SetCellValue(1, 3, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.Name", ""),,,,,,, True)
                excel.SetCellValue(1, 4, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.ShortName", ""),,,,,,, True)
                excel.SetCellValue(1, 5, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.Assigned", ""),,,,,,, True)
                excel.SetCellValue(1, 6, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.Worked", ""),,,,,,, True)
                excel.SetCellValue(1, 7, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.Performance", ""),,,,,,, True)
                excel.SetCellValue(1, 8, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.PaseHour", ""),,,,,,, True)
                excel.SetCellValue(1, 9, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.Prima", ""),,,,,,, True)
                excel.SetCellValue(1, 10, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.TotalPeriod", ""),,,,,,, True)
                excel.SetCellValue(1, 11, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.BeginDate", ""),,,,,,, True)
                excel.SetCellValue(1, 12, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.EndDate", ""),,,,,,, True)
            End Sub

            Public Shared Sub WriteTotals(ByRef excel As ExcelExport, row As Integer, ByRef oState As roDataLinkState, employeeTotals As Totals, Optional isSummary As Boolean = False)
                If (isSummary) Then
                    excel.SetCellValue(row, 1, employeeTotals.EmployeeName)
                Else
                    excel.SetCellValue(row, 3, oState.Language.Translate("roDataLinkExport.ExcelExportAccrualsDaily.Task.Totals", ""),,,,,,, True)
                    excel.SetCellValue(row, 9, Math.Round(employeeTotals.Accrual, 2))
                End If
                excel.SetCellValue(row, 4, Math.Round(employeeTotals.Assigned, 2))
                excel.SetCellValue(row, 5, Math.Round(employeeTotals.Worked, 2))
                excel.SetCellValue(row, 6, Math.Round(employeeTotals.Performance, 2))
                excel.SetCellValue(row, 7, Math.Round(employeeTotals.HorasPase, 2))
                excel.SetCellValue(row, 8, Math.Round(employeeTotals.Prima, 2))

            End Sub

            Public Sub WriteLine(ByRef excel As ExcelExport, row As Integer, beginDate As Date, endDate As Date, accrualValue As Double, bolIsFirst As Boolean)
                excel.SetCellValue(row, 1, EmployeeName)
                excel.SetCellValue(row, 2, TaskProject)
                excel.SetCellValue(row, 3, TaskName)
                excel.SetCellValue(row, 4, TaskShortName)
                excel.SetCellValue(row, 5, Math.Round(Assigned, 4))
                excel.SetCellValue(row, 6, Math.Round(WorkedHours, 4))
                excel.SetCellValue(row, 7, Math.Round(Performance, 4))
                excel.SetCellValue(row, 8, Math.Round(HorasPase, 4))
                excel.SetCellValue(row, 9, Math.Round(Prima, 4))
                excel.SetCellValue(row, 10, If(bolIsFirst, Math.Round(accrualValue, 4), ""))
                excel.SetCellValue(row, 11, If(bolIsFirst, beginDate, ""), "DD/MM/YYYY")
                excel.SetCellValue(row, 12, If(bolIsFirst, endDate, ""), "DD/MM/YYYY")
            End Sub
#End Region

            Public Class Totals
                Public Property Assigned() As Double
                Public Property Worked() As Double
                Public Property Performance() As Double
                Public Property HorasPase() As Double
                Public Property Prima() As Double
                Public Property EmployeeName() As String
                Public Property Accrual() As Double

                Public Sub New()
                    Assigned = 0
                    Worked = 0
                    Performance = 0
                    HorasPase = 0
                    Prima = 0
                    Accrual = 0
                End Sub
            End Class
        End Class

        Public Class PrimaEstimatedList
            Inherits List(Of PrimaEstimated)
            Public Sub Load(dt As DataTable)
                Try
                    For Each row In dt.Rows
                        Dim employeData = New PrimaEstimated With
                            {
                            .IdEmployee = Any2Integer(row("IDEmployee")),
                            .IdTask = Any2Integer(row("ID")),
                            .ExpectedHours = Any2Double(row("InitialTime")),
                            .WorkedHours = Any2Double(row("TotalTime")),
                            .EmployeeName = Any2String(row("EmployeeName")),
                            .TaskName = Any2String(row("TaskName")),
                            .TaskProject = Any2String(row("Project")),
                            .TaskShortName = Any2String(row("ShortName"))
                            }
                        Me.Add(employeData)
                    Next
                Catch ex As Exception

                End Try


            End Sub

        End Class
#End Region
#Region "Clases Auxialiares Exportación fichajes Tisvol"
        Public Class EmployeePunch
            Public Property Name() As String
            Public Property PunchType() As Integer
            Public Property PunchDate() As DateTime
            Public Property Shift() As String
            Public Property Acrrual() As Double
            Public Property Punch() As DateTime
        End Class

        Public Class EmployeePunchList
            Inherits List(Of EmployeePunch)

            Public Sub New(dt As DataTable, ByRef oState As roDataLinkState)
                Try
                    For Each row As DataRow In dt.Rows
                        Dim employeePunch As New EmployeePunch With
                            {
                                .Name = Any2String(row("Name")),
                                .PunchType = Any2Integer(row("ActualType")),
                                .PunchDate = Any2DateTime(row("ShiftDate")),
                                .Shift = Any2String(row("ShortName")),
                                .Acrrual = Any2Double(row("Acrrual")),
                                .Punch = Any2Time(row("DateTime")).TimeOnly
                            }
                        Add(employeePunch)
                    Next
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roDataLinkExport::EmployeePunchList::New")
                End Try

            End Sub

            Public Function ToText(ByRef oState As roDataLinkState, strSeparator As String) As List(Of String)
                Dim fileLines As New List(Of String)
                Dim punchesFormat = "{0};{1};{2};{3};{4};{5}"

                Try
                    For Each employeeData In Me.GroupBy(Function(k) New With {Key .Name = k.Name, Key .PunchDate = k.PunchDate, Key .Shift = k.Shift, Key .Acrrual = k.Acrrual})
                        Dim strPunch1 = "", strPunch2 = "", strPunch3 = "", strPunch4 = "", strPunch5 = "", strPunch6 = ""
                        Dim strLine = New StringBuilder
                        Dim countPunches = 0
                        strLine.Append(employeeData.Key.Name & strSeparator)
                        strLine.Append(employeeData.Key.PunchDate & strSeparator)
                        strLine.Append(employeeData.Key.Shift & strSeparator)
                        For Each punchData In employeeData
                            If (countPunches < 6) Then
                                countPunches += 1
                                Select Case countPunches
                                    Case 1
                                        strPunch1 = punchData.Punch.ToString("HHmm").PadRight(6, "")
                                    Case 2
                                        strPunch2 = punchData.Punch.ToString("HHmm").PadRight(6, "")
                                    Case 3
                                        strPunch3 = punchData.Punch.ToString("HHmm").PadRight(6, "")
                                    Case 4
                                        strPunch4 = punchData.Punch.ToString("HHmm").PadRight(6, "")
                                    Case 5
                                        strPunch5 = punchData.Punch.ToString("HHmm").PadRight(6, "")
                                    Case 6
                                        strPunch6 = punchData.Punch.ToString("HHmm").PadRight(6, "")
                                End Select
                            Else
                                Exit For
                            End If
                        Next
                        strLine.Append(String.Format(punchesFormat, strPunch1, strPunch2, strPunch3, strPunch4, strPunch5, strPunch6) & strSeparator)
                        strLine.Append(String.Empty.PadLeft(24, "") & strSeparator)
                        strLine.Append(employeeData.Key.Acrrual)
                        fileLines.Add(strLine.ToString())
                    Next

                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roDataLinkExport::EmployeePunchList::ToText")
                End Try

                Return fileLines
            End Function

        End Class

#End Region

    End Class

End Namespace