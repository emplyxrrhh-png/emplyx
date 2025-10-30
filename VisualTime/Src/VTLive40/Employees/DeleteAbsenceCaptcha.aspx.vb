Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Employees_DeleteAbsenceCaptcha
    Inherits PageBase

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            txtCaptcha.Focus()

            Me.hdnErrorValue.Value = Me.Language.Translate("Error.CaptchaValidation", DefaultScope)

            ' 0 absencia por días - 1 absencia por horas - 2 vacaciones
            Dim intAbsenceType As Integer = roTypes.Any2Integer(Request.Params("AbsenceType"))

            Dim intIdProgrammedHoliday As Integer = roTypes.Any2Integer(Request.Params("IdProgrammedHoliday"))
            Dim intIdProgrammedOvertime As Integer = roTypes.Any2Integer(Request.Params("IdProgrammedOvertime"))

            Dim beginDate As Date = If(Request.Params("BeginDate") = "1970/01/01", Nothing, roTypes.Any2DateTime(Request.Params("BeginDate")))
            Dim intIdEmployee As Integer = roTypes.Any2Integer(Request.Params("IdEmployee"))
            Dim intIdAbsence As Integer = roTypes.Any2Integer(Request.Params("IdAbsence"))
            Dim intIdCause As Integer = roTypes.Any2Integer(Request.Params("IdCause"))
            If (beginDate = Nothing Or intIdEmployee <= 0 Or intIdAbsence < 0) AndAlso Not ((intIdProgrammedHoliday < 0 AndAlso intIdProgrammedOvertime > 0) OrElse (intIdProgrammedHoliday > 0 AndAlso intIdProgrammedOvertime < 0)) Then
                btnAccept.Visible = False
            Else
                ViewState("BeginDate") = beginDate
                ViewState("IdEmployee") = intIdEmployee
                ViewState("AbsenceType") = intAbsenceType
                ViewState("IdCause") = intIdCause
                ViewState("IdAbsence") = intIdAbsence
                Dim oAbsence As Object = Nothing
                If intAbsenceType = 0 Then
                    Me.Img1.Src = "~/Base/Images/Employees/ProgrammedAbsences.png"
                    Me.lblDescription1.Text = Me.Language.Translate("Delete.ProgrammedAbsenceDescription", Me.DefaultScope)
                    oAbsence = API.ProgrammedAbsencesServiceMethods.GetProgrammedAbsence(Me.Page, intIdEmployee, beginDate)
                ElseIf intAbsenceType = 1 Then
                    Me.Img1.Src = "~/Base/Images/Employees/ProgrammedCauses.png"
                    Me.lblDescription1.Text = Me.Language.Translate("Delete.ProgrammedCauseDescription", Me.DefaultScope)
                    oAbsence = API.ProgrammedCausesServiceMethods.GetProgrammedCause(Me.Page, intIdEmployee, beginDate, intIdAbsence)
                ElseIf intAbsenceType = 2 Then
                    Me.Img1.Src = "~/Base/Images/Employees/ProgrammedHolidays.png"
                    Me.lblDescription1.Text = Me.Language.Translate("Delete.ProgrammedHolidaysDescription", Me.DefaultScope)
                    oAbsence = API.ProgrammedHolidaysServiceMethods.GetProgrammedHolidayById(Me.Page, intIdProgrammedHoliday, False)
                ElseIf intAbsenceType = 3 Then
                    Me.Img1.Src = "~/Base/Images/Employees/ProgrammedOvertimes.png"
                    Me.lblDescription1.Text = Me.Language.Translate("Delete.ProgrammedOvertimessDescription", Me.DefaultScope)
                    oAbsence = API.ProgrammedOvertimesServiceMethods.GetProgrammedOvertimeById(Me.Page, intIdProgrammedOvertime, False)
                End If
                If Not oAbsence Is Nothing Then
                    Dim r As New Random()
                    c1.Text = r.Next(0, 9)
                    c2.Text = r.Next(0, 9)
                    c3.Text = r.Next(0, 9)
                    c4.Text = r.Next(0, 9)

                    If intAbsenceType = 0 Then
                        Me.hdnJsType.Value = "0," & intIdCause & "," & intIdEmployee & "," & beginDate.ToString(HelperWeb.GetShortDateFormat) & ""
                    ElseIf intAbsenceType = 1 Then
                        Me.hdnJsType.Value = "1," & intIdCause & "," & intIdEmployee & "," & beginDate.ToString(HelperWeb.GetShortDateFormat) & "," & intIdAbsence
                    ElseIf intAbsenceType = 2 Then
                        Me.hdnJsType.Value = "2," & intIdProgrammedHoliday
                    ElseIf intAbsenceType = 3 Then
                        Me.hdnJsType.Value = "3," & intIdProgrammedOvertime
                    End If
                Else
                    btnAccept.Visible = False
                End If
            End If
        End If

    End Sub

    'Protected Sub btnAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccept.Click
    '    If txtCaptcha.Value = c1.Text & c2.Text & c3.Text & c4.Text Then
    '        Me.CanClose = True
    '        Me.MustRefresh = "5"
    '        Dim intAbsenceType As Integer = roTypes.Any2Integer(ViewState("AbsenceType"))
    '        Dim beginDate As Date = roTypes.Any2DateTime(ViewState("BeginDate"))
    '        Dim intIdEmployee As Integer = roTypes.Any2Integer(ViewState("IdEmployee"))
    '        Dim intIdAbsence As Integer = roTypes.Any2Integer(ViewState("IdAbsence"))
    '        Dim intIdCause As Integer = roTypes.Any2Integer(ViewState("IdCause"))

    '        If intAbsenceType = 0 Then
    '            Me.hdnParams_PageBase.Value = "DELETE_PROGRAMMED_ABSENCE#" & intIdCause & "," & intIdEmployee & ",'" & beginDate.ToString(HelperWeb.GetShortDateFormat) & "'"
    '        Else
    '            Me.hdnParams_PageBase.Value = "DELETE_PROGRAMMED_CAUSE#" & intIdCause & "," & intIdEmployee & ",'" & beginDate.ToString(HelperWeb.GetShortDateFormat) & "'," & intIdAbsence
    '        End If
    '    End If
    'End Sub

End Class