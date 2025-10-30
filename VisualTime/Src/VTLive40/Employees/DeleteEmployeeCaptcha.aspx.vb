Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Employees_DeleteEmployeeCaptcha
    Inherits PageBase

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            txtCaptcha.Focus()
            Dim intIdEmployee As Integer = roTypes.Any2Integer(Request.Params("IdEmployee"))
            Me.hdnErrorValue.Value = Me.Language.Translate("DeleteEmployee.IncorrectCaptcha", Me.DefaultScope)
            If intIdEmployee <= 0 Then
                btnAccept.Visible = False
            Else
                ViewState("idEmployee") = intIdEmployee
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, intIdEmployee, False)
                If Not oEmployee Is Nothing Then
                    lblNameEmployee.Text = oEmployee.Name.ToUpper()
                    Dim r As New Random()
                    c1.Text = r.Next(0, 9)
                    c2.Text = r.Next(0, 9)
                    c3.Text = r.Next(0, 9)
                    c4.Text = r.Next(0, 9)
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
    '        Dim intIdEmployee As Integer = roTypes.Any2Integer(ViewState("idEmployee"))
    '        Me.hdnParams_PageBase.Value = "DELETE_EMPLOYEE#" & intIdEmployee
    '    End If
    'End Sub

End Class