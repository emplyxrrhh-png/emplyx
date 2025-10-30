Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class AnnualView
    Inherits PageBase

    Private bolCancelDblCkick As Boolean

    Private Property IdEmployeeSelected() As Integer
        Get
            If ViewState("AnnualView_IdEmployeeSelected") Is Nothing Then
                Return 0
            Else
                Return ViewState("AnnualView_IdEmployeeSelected")
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("AnnualView_IdEmployeeSelected") = value
        End Set
    End Property

    Private Property YearSelected() As Integer
        Get
            If ViewState("AnnualView_YearSelected") Is Nothing Then
                Return DateTime.Today.Year
            Else
                Return ViewState("AnnualView_YearSelected")
            End If
        End Get
        Set(ByVal value As Integer)
            ViewState("AnnualView_YearSelected") = value
        End Set
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.InsertCssIncludes()
        Me.InsertExtraCssIncludes("~/Scheduler/Styles/AnnualView.css")

        If Not Me.IsPostBack Then

            Me.IdEmployeeSelected = roTypes.Any2Integer(Request("EmployeeID"))
            Dim intYearView As Integer = roTypes.Any2Integer(Request("Year"))
            Me.YearSelected = IIf(intYearView <= 0, DateTime.Today.Year, intYearView)

            Dim strOrigin As String = roTypes.Any2String(Request("fromPage"))

            If strOrigin = "request" OrElse strOrigin = "employees" Then
                bolCancelDblCkick = True
                Me.lblInfo.Style("display") = "none"
            Else
                bolCancelDblCkick = False
                Me.lblInfo.Style("display") = ""
            End If

            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Me.IdEmployeeSelected, False)
            If Not oEmployee Is Nothing Then
                lblEmployee.Text = roTypes.Any2String(oEmployee.Name)
            End If

            DrawScreen(True)

        End If

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshwebform", "parent.showLoader(false);", True)

    End Sub

    Private Sub DrawScreen(ByVal IsComplete As Boolean)
        If IsComplete Then
            FillContracts()
        End If
        FillShifts()
        tdYearSelected.InnerText = Me.YearSelected
        tdPlaceMonths.InnerHtml = RenderMonths()
    End Sub

    Private Function IdealTextColor(ByVal bg As System.Drawing.Color) As System.Drawing.Color
        Dim nThreshold As Integer = 105
        Dim bgDelta As Integer = Convert.ToInt32((bg.R * 0.299) + (bg.G * 0.587) + (bg.B * 0.114))
        Dim forecolor As System.Drawing.Color
        If (255 - bgDelta < nThreshold) Then
            forecolor = System.Drawing.Color.Black
        Else
            forecolor = System.Drawing.Color.White
        End If
        Return forecolor
    End Function

    Private Function RenderMonths() As String

        Dim dDate As New Date(Me.YearSelected, 1, 1)
        Dim today As Integer = 1
        Dim todaymonth As Integer = 1

        Dim iDaysInMonth As Integer
        Dim iWeekdayMonthStartsOn As Integer
        Dim iDayOfMonthIndex As Integer
        Dim iColumnPosition As Integer

        Dim iCountDays As Integer = 0

        Dim dateID As Date
        Dim oPanelColor As System.Drawing.Color = Drawing.Color.Empty
        Dim intColor As Integer = -1
        Dim iColor As String = ""
        Dim fColor As String = ""
        Dim shiftTitle As String = ""
        Dim r, g, b As Byte
        Dim feastDayStyle = "border: 2px solid red"

        Dim DateInf As DateTime = New Date(Me.YearSelected, 1, 1)
        Dim DateSup As DateTime = New Date(Me.YearSelected, 12, 31)

        'si hay contrato seleccionado obtener fechas del contrato sino coger año completo
        If cmbContractsDev.SelectedItem.Value <> String.Empty Then
            Dim oContract As roContract = API.ContractsServiceMethods.GetContract(Me.Page, cmbContractsDev.SelectedItem.Value, False)
            If Not oContract Is Nothing Then
                DateInf = IIf(oContract.BeginDate < DateInf, DateInf, oContract.BeginDate)
                DateSup = IIf(oContract.EndDate > DateSup, DateSup, oContract.EndDate)
            End If
        End If

        Dim tbPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Me.Page, Me.IdEmployeeSelected, DateInf, DateSup)
        Dim oProgrammedHolidays As Generic.List(Of roProgrammedHoliday) = API.ProgrammedHolidaysServiceMethods.GetProgrammedHolidaysList(Me.IdEmployeeSelected, "Date >=" & roTypes.Any2Time(DateInf).SQLSmallDateTime & " AND Date <=" & roTypes.Any2Time(DateSup).SQLSmallDateTime, Me.Page, False)
        Dim bContinuar As Boolean = False
        If tbPlan IsNot Nothing AndAlso tbPlan.Rows.Count > 0 Then
            bContinuar = True
        End If

        Dim strRender As String = "<table style=""height:512px;""><tr>" & Environment.NewLine

        For n As Integer = 1 To 12

            If n = 5 Or n = 9 Then
                strRender &= "<tr>"
            End If

            strRender &= "<td width=""190px"" valign=""top"">"

            iDaysInMonth = Date.DaysInMonth(Me.YearSelected, n)
            iWeekdayMonthStartsOn = GetWeekdayMonthStartsOn(n, Me.YearSelected)

            strRender &= "<table border=""0"" cellspacing=""0"" cellpadding=""0"" width=""100%"" style=""width: 190px; padding: 2px;"">" & Environment.NewLine
            strRender &= "<tr><td colspan=""7"">"
            strRender &= "<table border=""0"">"
            strRender &= "	<tr>" & Environment.NewLine
            strRender &= "		<td align=""center"" valign=""top"" colspan=""7"" class=""monthlybg-header"">" & MonthName(n).ToString.ToUpper & " " & Me.YearSelected & "</td>" & Environment.NewLine
            strRender &= "	</tr>" & Environment.NewLine
            strRender &= "</table>"
            strRender &= "</td></tr>"
            strRender &= "	<tr>" & Environment.NewLine
            For wdName As Integer = 1 To 7
                strRender &= "		<td class=""monthlybg-header-day"">" & Me.Language.Keyword("weekday." & wdName.ToString).Substring(0, 3) & "</TD>" & Environment.NewLine
            Next
            strRender &= "	</tr>" & Environment.NewLine

            strRender &= "" & Environment.NewLine

            If iWeekdayMonthStartsOn = 0 Then iWeekdayMonthStartsOn = 7

            If iWeekdayMonthStartsOn <> 1 Then
                strRender &= vbTab & "<tr>" & Environment.NewLine
                iColumnPosition = 1
                Do While iColumnPosition < iWeekdayMonthStartsOn
                    strRender &= vbTab & vbTab & "<td>&nbsp;</td>" & Environment.NewLine
                    iColumnPosition = iColumnPosition + 1
                Loop
            End If

            iDayOfMonthIndex = 1
            iColumnPosition = iWeekdayMonthStartsOn

            Do While iDayOfMonthIndex <= iDaysInMonth
                Dim isFeastDay As Boolean = False
                If iColumnPosition = 1 Then
                    strRender &= vbTab & "<tr>" & Environment.NewLine
                End If

                oPanelColor = Drawing.Color.Empty
                intColor = -1
                iColor = String.Empty
                fColor = String.Empty
                shiftTitle = String.Empty

                If bContinuar Then

                    Dim oDetail As DataRow() = tbPlan.Select("Date = '" & Format(New Date(Me.YearSelected, n, iDayOfMonthIndex), "yyyy/MM/dd") & "'")
                    If oDetail.Length > 0 Then
                        Dim oRowPlan As DataRow = oDetail(0)
                        If roTypes.Any2Boolean(oRowPlan("FeastDay")) = True Then
                            isFeastDay = True
                        End If
                        If roTypes.Any2Integer(oRowPlan("Status")) >= 40 Then
                            ' El día ha sido procesado. Pintamos el color del horario utilizado
                            If Not IsDBNull(oRowPlan("ShiftColor1")) AndAlso oRowPlan("ShiftColor1") IsNot Nothing Then
                                intColor = roTypes.Any2Integer(oRowPlan("ShiftColor1"))
                            ElseIf Not IsDBNull(oRowPlan("UsedColor")) AndAlso oRowPlan("UsedColor") IsNot Nothing Then
                                intColor = roTypes.Any2Integer(oRowPlan("UsedColor"))
                            End If
                            shiftTitle = roTypes.Any2String(oRowPlan("Name1"))
                        Else
                            ' El día no ha sido procesado. Pintamos el color del horario principal
                            If Not IsDBNull(oRowPlan("ShiftColor1")) AndAlso oRowPlan("ShiftColor1") IsNot Nothing Then
                                intColor = roTypes.Any2Integer(oRowPlan("ShiftColor1"))
                                shiftTitle = roTypes.Any2String(oRowPlan("Name1"))
                            End If
                        End If

                        If intColor <> -1 Then
                            r = intColor And 255
                            g = (intColor \ 256) And 255
                            b = (intColor \ 65536) And 255
                            oPanelColor = Drawing.Color.FromArgb(r, g, b)
                        End If

                        If oPanelColor <> Drawing.Color.Empty Then
                            iColor = System.Drawing.ColorTranslator.ToHtml(oPanelColor)
                            fColor = System.Drawing.ColorTranslator.ToHtml(Me.IdealTextColor(oPanelColor))
                        End If

                        iCountDays += 1

                    End If

                End If

                dateID = New DateTime(Me.YearSelected, n, iDayOfMonthIndex, System.Globalization.CultureInfo.CurrentCulture.Calendar) '.ToString("dd/MM/yyyy")

                Dim bExistPlannedHoliday As Boolean = (oProgrammedHolidays.FindAll(Function(x) x.ProgrammedDate = dateID).Count > 0)

                If bolCancelDblCkick Then
                    If iColor = "" Then
                        strRender &= vbTab & vbTab & "<td class=""monthlybg-day""><a id=""" & dateID & """ href=""javascript: void(0);"" class=""daybg rndPlanCorner"" style=""" & If(bExistPlannedHoliday, "style=""border: 2px solid #000;", "") & If(isFeastDay, feastDayStyle, "") & ";"">" & iDayOfMonthIndex & "</a></td>" & Environment.NewLine
                    Else
                        strRender &= vbTab & vbTab & "<td Class=""monthlybg-day""><a id=""" & dateID & """ href=""javascript: void(0);"" class=""daybg rndPlanCorner"" style=""" & If(bExistPlannedHoliday, "border: 2px solid #000", "") & If(isFeastDay, feastDayStyle, "") & ";background-color: " & iColor & "; color: " & fColor & ";"" title=""" & shiftTitle & """>" & iDayOfMonthIndex & "</a></td>" & Environment.NewLine
                    End If
                Else
                    If iColor = "" Then
                        strRender &= vbTab & vbTab & "<td class=""monthlybg-day""><a id=""" & dateID & """ ondblclick=""getSelectedDay(this.id)""  href=""javascript: void(0);"" class=""daybg rndPlanCorner"" style=""" & If(bExistPlannedHoliday, "style=""border: 2px solid #000;""", "") & If(isFeastDay, feastDayStyle, "") & ";"">" & iDayOfMonthIndex & "</a></td>" & Environment.NewLine
                    Else
                        strRender &= vbTab & vbTab & "<td class=""monthlybg-day""><a id=""" & dateID & """ ondblclick=""getSelectedDay(this.id)""  href=""javascript: void(0);"" class=""daybg rndPlanCorner"" style=""" & If(bExistPlannedHoliday, "border: 2px solid #000", "") & If(isFeastDay, feastDayStyle, "") & ";background-color: " & iColor & "; color: " & fColor & ";"" title=""" & shiftTitle & """>" & iDayOfMonthIndex & "</a></td>" & Environment.NewLine
                    End If
                End If

                If iColumnPosition = 7 Then
                    strRender &= vbTab & "</tr>" & Environment.NewLine
                    iColumnPosition = 0
                End If

                iDayOfMonthIndex = iDayOfMonthIndex + 1
                iColumnPosition = iColumnPosition + 1
            Loop

            If iColumnPosition <> 1 Then
                Do While iColumnPosition <= 7
                    strRender &= vbTab & vbTab & "<td>&nbsp;</td>" & Environment.NewLine
                    iColumnPosition = iColumnPosition + 1
                Loop
                strRender &= vbTab & "" & Environment.NewLine
            End If

            strRender &= "</table>" & Environment.NewLine

            dDate = dDate.AddMonths(1)

            strRender &= "</td>" & Environment.NewLine

            If n = 4 Or n = 8 Then
                strRender &= "</tr>"
            End If

        Next

        strRender &= "</table>" & Environment.NewLine

        Return strRender

    End Function

    Private Function GetWeekdayMonthStartsOn(ByVal iMonth As Integer, ByVal iYear As Integer) As Integer
        Return New Date(iYear, iMonth, 1).DayOfWeek
    End Function

    Private Sub FillContracts()
        Dim tbContracts As DataTable = API.ContractsServiceMethods.GetContractsByIDEmployee(Me.Page, Me.IdEmployeeSelected, False)
        'cmbContracts.ClearItems()
        cmbContractsDev.Items.Clear()

        'cmbContracts.AddItem("", "", "ShowContract('')")

        cmbContractsDev.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("AllContracts.Caption", Me.DefaultScope), ""))
        Dim DateInf As DateTime = New DateTime(Me.YearSelected, 1, 1)
        Dim DateSup As DateTime = New DateTime(Me.YearSelected, 1, 1).AddYears(1).AddDays(-1)

        If Not tbContracts Is Nothing AndAlso tbContracts.Rows.Count > 0 Then
            For Each rw As DataRow In tbContracts.Rows

                If DateInf >= rw("BeginDate") And DateInf <= rw("EndDate") Or
                   DateSup >= rw("BeginDate") And DateSup <= rw("EndDate") Or
                   DateInf <= rw("BeginDate") And DateSup >= rw("EndDate") Or
                   rw("BeginDate") <= DateInf And rw("BeginDate") >= DateSup Then

                    'cmbContracts.AddItem(rw("IdContract"), rw("IdContract"), "ShowContract('" & rw("IdContract") & "')")
                    cmbContractsDev.Items.Add(New DevExpress.Web.ListEditItem(rw("IdContract"), rw("IdContract")))

                End If
            Next
        End If
        cmbContractsDev.SelectedIndex = 0

    End Sub

    Private Sub FillShifts()
        Dim strRender As String = String.Empty
        Dim strRenderPlanned As String = String.Empty
        Dim totalPlanned As Decimal = 0
        Dim auxColor As System.Drawing.Color
        Dim oHTMLColor As String

        strRender = "<table width=""100%"">" & Environment.NewLine
        Dim dTblShifts As DataTable
        If cmbContractsDev.SelectedItem.Value = "" Then
            dTblShifts = API.ShiftServiceMethods.GetShiftsTotalsByEmployee(Me.Page, Me.YearSelected, -1, Me.IdEmployeeSelected)
        Else
            dTblShifts = API.ShiftServiceMethods.GetShiftsTotalsByEmployeeAndContract(Me.Page, Me.YearSelected, -1, Me.IdEmployeeSelected, cmbContractsDev.SelectedItem.Value)
        End If
        If Not dTblShifts Is Nothing AndAlso dTblShifts.Rows.Count > 0 Then
            For Each dRowShift As DataRow In dTblShifts.Rows
                If roTypes.Any2Integer(dRowShift("Quantity")) > 0 Then
                    totalPlanned = totalPlanned + roTypes.Any2Double(dRowShift("Hours"))
                    auxColor = System.Drawing.ColorTranslator.FromWin32(dRowShift("Color"))
                    oHTMLColor = System.Drawing.ColorTranslator.ToHtml(auxColor)

                    strRender &= "<tr width=""100%"">"
                    strRender &= "<td><a href=""javascript: void(0);"" class=""daybg rndPlanCorner"" style=""background-color: " & oHTMLColor & ";"" title=""" & dRowShift("Name").ToString & """></a></td>"
                    strRender &= "<td><a title=""" & dRowShift("Name").ToString & """>" & dRowShift("ShortName").ToString & "</span></td>" & Environment.NewLine
                    strRender &= "<td align=""right""> - </td>" & Environment.NewLine
                    strRender &= "<td align=""left"" style=""white-space:nowrap;""><a>" & dRowShift("Quantity").ToString & " (" & dRowShift("HoursString").ToString & ")</span></td>" & Environment.NewLine
                    strRender &= "</tr>" & Environment.NewLine

                End If

            Next
        End If

        strRender &= "</table>"

        divShifts.InnerHtml = strRender
        Dim ts As TimeSpan = TimeSpan.FromHours(CDbl(totalPlanned))
        divTotalPlanned.InnerHtml = String.Format("{0}:{1}", Math.Truncate(ts.TotalHours), ts.Minutes)
    End Sub

    Protected Sub PreviousYear_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles PreviousYear.ServerClick
        Me.YearSelected = Me.YearSelected - 1
        DrawScreen(True)
    End Sub

    Protected Sub NextYear_ServerClick(ByVal sender As Object, ByVal e As System.EventArgs) Handles NextYear.ServerClick
        Me.YearSelected = Me.YearSelected + 1
        DrawScreen(True)
    End Sub

    Protected Sub cmbContractsDev_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cmbContractsDev.SelectedIndexChanged
        DrawScreen(False)
    End Sub

End Class