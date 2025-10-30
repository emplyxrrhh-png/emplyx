Imports Robotics.Web.Base

Partial Class Employees_DeleteEmployeeContractCaptcha
    Inherits PageBase

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            txtCaptcha.Focus()

            Dim r As New Random()
            c1.Text = r.Next(0, 9)
            c2.Text = r.Next(0, 9)
            c3.Text = r.Next(0, 9)
            c4.Text = r.Next(0, 9)

            Me.hdnErrorValue.Value = Me.Language.Translate("CaptchaTrackingValidationError", DefaultScope)
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "refreshInitGrid", "window.parent.parent.showLoader(false);", True)
        End If

    End Sub

    'Protected Sub btnAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccept.Click
    '    If txtCaptcha.Value = c1.Text & c2.Text & c3.Text & c4.Text Then

    '    End If
    'End Sub

End Class