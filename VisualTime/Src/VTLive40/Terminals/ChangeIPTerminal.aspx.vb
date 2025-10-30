Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Web.Base

Partial Class ChangeIPTerminal
    Inherits PageBase

    Private Const FeatureAlias As String = "Terminals.Definition"
    Private oPermission As Permission
    Private Const FeatureAliasStatus As String = "Terminals.StatusInfo"
    Private oPermissionStatus As Permission

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oPermissionStatus = Me.GetFeaturePermission(FeatureAliasStatus)

        Me.lblDescError.Text = ""
        Me.lblDescError.Visible = False
        If Request("ID") Is Nothing Then WLHelperWeb.RedirectAccessDenied()
        If Request("ID") = "" Then WLHelperWeb.RedirectAccessDenied()
        If Not IsNumeric(Request("ID")) Then WLHelperWeb.RedirectAccessDenied()

    End Sub

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click

        Dim strMessage As String = ""

        If Me.txtIP1.Value = "" Or Me.txtIP2.Value = "" Or Me.txtIP3.Value = "" Or Me.txtIP4.Value = "" Then
            strMessage = Me.Language.Translate("ChangeIPTerminal.ErrorIP", DefaultScope)
        End If

        If strMessage = "" Then
            If Not IsNumeric(Me.txtIP1.Value) Or Not IsNumeric(Me.txtIP2.Value) Or Not IsNumeric(Me.txtIP3.Value) Or Not IsNumeric(Me.txtIP4.Value) Then
                strMessage = Me.Language.Translate("ChangeIPTerminal.ErrorIP", DefaultScope)
            End If
        End If

        If strMessage = "" Then
            If (Me.txtIP1.Value < 0 Or Me.txtIP1.Value > 255) Or
                (Me.txtIP2.Value < 0 Or Me.txtIP2.Value > 255) Or
                (Me.txtIP3.Value < 0 Or Me.txtIP3.Value > 255) Or
                (Me.txtIP4.Value < 0 Or Me.txtIP4.Value > 255) Then
                strMessage = Me.Language.Translate("ChangeIPTerminal.ErrorIP", DefaultScope)
            End If
        End If

        If strMessage = "" Then
            'Grabamos el terminal
            Dim oTerminal As roTerminal = API.TerminalServiceMethods.GetTerminal(Me.Page, Request("ID"), False)

            If oTerminal IsNot Nothing Then
                oTerminal.Location = Me.txtIP1.Value.Trim & "." & Me.txtIP2.Value.Trim & "." & Me.txtIP3.Value.Trim & "." & Me.txtIP4.Value.Trim
                If Not API.TerminalServiceMethods.SaveTerminal(Me.Page, oTerminal, True) Then
                    strMessage = roWsUserManagement.SessionObject.States.TerminalState.ErrorText
                End If
            End If
        End If

        If strMessage = "" Then
            Me.MustRefresh = "6"
            Me.CanClose = True
        Else
            Me.lblDescError.Text = strMessage
            Me.lblDescError.Visible = True
        End If

    End Sub

End Class