Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class PrintAbsences
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="data")>
        Public Data As Integer

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

    End Class

#Region "Declarations"

    Dim EmployeeID As String

#End Region

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Me.InsertExtraJavascript("PrintAbsences", "~/Employees/Scripts/PrintAbsences.js")

        EmployeeID = Request.Params("EmployeeID")

        If Not Me.IsPostBack Then
            Dim scriptKey As String = "UniqueKeyForThisScript"
            Dim javaScript As String = "<script type='text/javascript'>LoadAbsences();</script>"
            ClientScript.RegisterStartupScript(Me.GetType(), scriptKey, javaScript)
        End If

    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Action.Trim.ToUpper
            Case "GETABSENCES"
                Dim absences = Me.ExtractAbsences()
                PerformActionCallback.JSProperties.Add("cpAction", "PRINTABSENCES")
                PerformActionCallback.JSProperties("cpAbsences") = roJSONHelper.SerializeNewtonSoft(absences)
            Case "EXPORT"
                PerformActionCallback.JSProperties.Add("cpAction", "EXPORT")

                Dim tb As DataTable = HttpContext.Current.Session("PRINT_ABSENCES_DATA")

                'Shell Path & " " & EmployeeName & "@" & CodigoUnix & "@" & IniAbsence & "@" & FinAbsence & "@" & WorkDays & "@"
                '& Format(PendingDays, "##0.0000") & "@" & NextDayWork & "@" & CauseShortName
                For Each row As DataRow In tb.Rows
                    If row("RowID") = oParameters.Data Then
                        Dim tbDiasLaborables As DataTable = Nothing
                        Dim tbnextDiasLaborables As DataTable = Nothing
                        Dim nextDiaLaborable As Date = Nothing
                        Dim employeeName = API.EmployeeServiceMethods.GetEmployeeName(Me.Page, EmployeeID)
                        Dim codigoUnix = API.EmployeeServiceMethods.GetEmployeeUserFieldValueAtDate(Me.Page, EmployeeID, "CodigoUnix", Now.Date)
                        Dim workingDays = API.EmployeeServiceMethods.GetPlan(Me, EmployeeID, row("BeginDate"), row("FinishDate"))
                        Dim nextWorkingDays = API.EmployeeServiceMethods.GetPlan(Me, EmployeeID, Date.Parse(row("FinishDate")).AddDays(1), Date.Parse(row("FinishDate")).AddMonths(1))
                        Dim query As IEnumerable(Of DataRow) = From day In workingDays.AsEnumerable() Where day.Field(Of Decimal?)("ExpectedWorkingHoursBase") > 0 Select day
                        Dim queryNext As IEnumerable(Of DataRow) = From day In nextWorkingDays.AsEnumerable() Where day.Field(Of Decimal?)("ExpectedWorkingHoursBase") > 0 Select day

                        If query.Count > 0 Then
                            tbDiasLaborables = query.CopyToDataTable()
                        End If
                        If queryNext.Count > 0 Then
                            tbnextDiasLaborables = queryNext.CopyToDataTable()
                        End If

                        If tbnextDiasLaborables IsNot Nothing Then
                            If tbnextDiasLaborables.Rows.Count > 0 Then
                                nextDiaLaborable = tbnextDiasLaborables.Rows(0)("Date")
                            End If
                        End If

                        'CodigoUnix
                        Dim cadena = "@" + employeeName + "@" + codigoUnix.FieldValue + "@" + row("BeginDate") + "@" + row("FinishDate") + "@" + tbDiasLaborables.Rows.Count.ToString + "@" + "DiasPendientes" + "@" + nextDiaLaborable

                        Dim path As String = "c:\temp\test.txt"
                        Dim createText As String = "blablabla"
                        File.WriteAllText(path, createText, Encoding.UTF8)
                    End If
                Next

        End Select

    End Sub

#End Region

#Region "Methods"

    Private Function ProgrammedAbsencesData(ByVal bReload As Boolean, ByVal IDEmployee As Integer) As DataTable

        Try
            Dim contadorRows = 0
            Dim tb As DataTable = HttpContext.Current.Session("PRINT_ABSENCES_DATA")

            If bReload OrElse tb Is Nothing Then
                Dim tbCauses As DataTable = Nothing
                Dim tbHolidays As DataTable = Nothing

                tb = API.ProgrammedAbsencesServiceMethods.GetProgrammedAbsences(Me, IDEmployee)
                tbCauses = API.ProgrammedCausesServiceMethods.GetProgrammedCauses(Me, IDEmployee)
                Dim oLstHolidays As Generic.List(Of roProgrammedHoliday) = ProgrammedHolidaysServiceMethods.GetProgrammedHolidaysList(IDEmployee, "", Me.Page, True)
                Dim oLstOvertime As Generic.List(Of roProgrammedOvertime) = ProgrammedOvertimesServiceMethods.GetProgrammedOvertimesList(IDEmployee, "", Me.Page, True)

                'Calculo dias de vacaciones por horario
                Dim holidays = API.EmployeeServiceMethods.GetPlan(Me, IDEmployee, Date.Now.AddYears(-1), Date.Now)
                Dim query As IEnumerable(Of DataRow) = From day In holidays.AsEnumerable() Where day.Field(Of Int16?)("ShiftType1") = 2 Select day

                If query.Count > 0 Then
                    tbHolidays = query.CopyToDataTable()
                End If

                Dim iCurrentRow As Integer = 1
                Dim iRowID = New DataColumn("RowID", GetType(String))
                Dim dcTipo = New DataColumn("Tipo", GetType(String))
                Dim dcTipoCauses = New DataColumn("Tipo", GetType(String))
                Dim dcBeginDate = New DataColumn("BeginDate", GetType(DateTime))
                tb.Columns.Add(iRowID)
                tb.Columns.Add(dcTipo)
                tbCauses.Columns.Add(dcTipoCauses)
                tbCauses.Columns.Add(dcBeginDate)

                For Each oRow As DataRow In tb.Rows
                    oRow("Tipo") = "Ausencias por días"
                Next

                For Each oRow As DataRow In tbCauses.Rows
                    oRow("Tipo") = "Ausencias por horas"
                    oRow("BeginDate") = oRow("Date")
                Next

                tb.Merge(tbCauses)

                'Inicio agrupacion Vacaciones por dia

                Dim beginDate As Date = New Date()
                Dim endDate As Date = New Date()
                Dim contador As Int16 = 0
                Dim printed As Boolean = False
                Dim IdShift As Int16 = 0
                Dim NameUsedShift As String = ""

                If tbHolidays IsNot Nothing Then
                    For Each oRow As DataRow In tbHolidays.Rows

                        IdShift = oRow("IDShift1")
                        NameUsedShift = oRow("NameUsedShift")

                        If contador = 0 Then
                            beginDate = oRow("Date")
                            endDate = beginDate
                        Else
                            If CDate(oRow("Date")).Subtract(endDate).Days > 1 Then
                                Dim R As DataRow = tb.NewRow
                                R("ID") = oRow("IDShift1")
                                R("IDCause") = oRow("IDShift1")
                                R("Tipo") = "Vacaciones"
                                R("Name") = oRow("NameUsedShift")
                                R("BeginDate") = beginDate
                                R("FinishDate") = endDate
                                tb.Rows.Add(R)
                                beginDate = oRow("Date")
                                endDate = oRow("Date")
                                printed = True
                            Else
                                endDate = oRow("Date")
                                printed = False
                            End If
                        End If
                        contador = contador + 1
                    Next
                End If

                If printed = False Then
                    Dim R As DataRow = tb.NewRow
                    R("ID") = IdShift
                    R("IDCause") = IdShift
                    R("Tipo") = "Vacaciones"
                    R("Name") = NameUsedShift
                    R("BeginDate") = beginDate
                    R("FinishDate") = endDate
                    tb.Rows.Add(R)
                End If

                'Fin agrupacion por dias

                For Each item In oLstOvertime
                    Dim R As DataRow = tb.NewRow
                    R("ID") = item.ID
                    R("IDCause") = item.IDCause
                    R("Tipo") = "Horas de exceso"
                    R("Name") = CausesServiceMethods.GetCauseByID(Me.Page, item.IDCause, False).Name
                    R("BeginDate") = item.ProgrammedBeginDate
                    R("FinishDate") = item.ProgrammedEndDate
                    tb.Rows.Add(R)
                Next

                For Each item In oLstHolidays
                    Dim R As DataRow = tb.NewRow
                    R("ID") = item.ID
                    R("IDCause") = item.IDCause
                    R("Tipo") = "Vacaciones/Permisos por Horas"
                    R("Name") = CausesServiceMethods.GetCauseByID(Me.Page, item.IDCause, False).Name
                    R("BeginDate") = item.ProgrammedDate
                    R("FinishDate") = item.ProgrammedDate
                    tb.Rows.Add(R)
                Next

                Dim queryExpected As IEnumerable(Of DataRow) = Nothing
                Dim queryNext As IEnumerable(Of DataRow) = Nothing
                Dim CodigoUnix = New DataColumn("CodigoUnix", GetType(String))
                Dim DiasLaborables = New DataColumn("DiasLaborables", GetType(String))
                'Dim DiasPendientes = New DataColumn("DiasPendientes", GetType(String))
                Dim DiaDeIncorporacion = New DataColumn("DiaDeIncorporacion", GetType(Date))
                tb.Columns.Add(CodigoUnix)
                tb.Columns.Add(DiasLaborables)
                'tb.Columns.Add(DiasPendientes)
                tb.Columns.Add(DiaDeIncorporacion)

                For Each row As DataRow In tb.Rows

                    Dim tbDiasLaborables As DataTable = Nothing
                    Dim tbnextDiasLaborables As DataTable = Nothing
                    Dim nextDiaLaborable As Date = Nothing
                    Dim employeeName = API.EmployeeServiceMethods.GetEmployeeName(Me.Page, EmployeeID)
                    Dim codigo = API.EmployeeServiceMethods.GetEmployeeUserFieldValueAtDate(Me.Page, EmployeeID, "CodigoUnix", Now.Date)
                    Dim workingDays = API.EmployeeServiceMethods.GetPlan(Me, EmployeeID, row("BeginDate"), row("FinishDate"))
                    Dim nextWorkingDays = API.EmployeeServiceMethods.GetPlan(Me, EmployeeID, Date.Parse(row("FinishDate")).AddDays(1), Date.Parse(row("FinishDate")).AddMonths(1))

                    row("CodigoUnix") = codigo.FieldValue.ToString

                    If workingDays IsNot Nothing AndAlso nextWorkingDays IsNot Nothing Then
                        If workingDays.Rows.Count > 0 AndAlso nextWorkingDays.Rows.Count > 0 Then
                            If (workingDays(0)("ShiftType1") = 1) Then
                                queryExpected = From day In workingDays.AsEnumerable() Where day.Field(Of Decimal?)("ExpectedWorkingHours1") > 0 Select day
                                queryNext = From day In nextWorkingDays.AsEnumerable() Where day.Field(Of Decimal?)("ExpectedWorkingHours1") > 0 Select day
                            Else
                                queryExpected = From day In workingDays.AsEnumerable() Where day.Field(Of Decimal?)("ExpectedWorkingHoursBase") > 0 Select day
                                queryNext = From day In nextWorkingDays.AsEnumerable() Where day.Field(Of Decimal?)("ExpectedWorkingHours1") > 0 Select day
                            End If

                            If queryExpected.Count > 0 Then
                                row("DiasLaborables") = queryExpected.Count.ToString
                            Else
                                row("DiasLaborables") = "0"

                            End If

                            If queryNext.Count > 0 Then
                                tbnextDiasLaborables = queryNext.CopyToDataTable()
                            End If

                            If tbnextDiasLaborables IsNot Nothing Then
                                If tbnextDiasLaborables.Rows.Count > 0 Then
                                    row("DiaDeIncorporacion") = tbnextDiasLaborables.Rows(0)("Date")

                                End If
                            End If
                        Else
                            row("DiasLaborables") = "Falta planificación"
                        End If
                    Else
                        row("DiasLaborables") = "Falta planificación"
                    End If

                Next

                For Each row In tb.Rows
                    row("RowID") = contadorRows
                    contadorRows = contadorRows + 1
                Next

                HttpContext.Current.Session("PRINT_ABSENCES_DATA") = tb

            End If

            Return tb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function ExtractAbsences() As DataTable
        Return ProgrammedAbsencesData(True, EmployeeID)
    End Function

    Private Sub SetPermissions()

    End Sub

#End Region

End Class