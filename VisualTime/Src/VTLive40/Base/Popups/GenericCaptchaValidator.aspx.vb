Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Base_GenericCaptchaValidator
    Inherits PageBase

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            txtCaptcha.Focus()

            Me.hdnCaptchaAction.Value = roTypes.Any2String(Me.Request("Action"))
            Me.hdnPopupName.Value = roTypes.Any2String(Me.Request("PopupName"))
            Me.hdnCallbackName.Value = roTypes.Any2String(Me.Request("CallbackName"))

            Dim r As New Random()
            c1.Text = r.Next(0, 9)
            c2.Text = r.Next(0, 9)
            c3.Text = r.Next(0, 9)
            c4.Text = r.Next(0, 9)

            spanCaptchaDesc2.InnerText = ""

            If Me.hdnCaptchaAction.Value.ToUpper = "SAVELASECURITYCHART" Then
                spanCaptchaDesc2.InnerText = Me.Language.Translate("SecurityChart.Warning.MessageBig", Me.DefaultScope)
            ElseIf Me.hdnCaptchaAction.Value.ToUpper = "LEGACYRESTRICTIONWARNING" Then
                spanCaptchaDesc2.InnerText = Me.Language.Translate("SecurityChart.TerminalInLegacyMode.Message", Me.DefaultScope)
            ElseIf Me.hdnCaptchaAction.Value.ToUpper = "COPYSPECIAL" Then
                spanCaptchaDesc2.InnerText = Me.Language.Translate("Calendar.Warning.Message", Me.DefaultScope)
            ElseIf Me.hdnCaptchaAction.Value.ToUpper = "CLOSEDATE" Then
                Dim oParamList As New Generic.List(Of String)
                Dim FreezingDatePage As Date = API.ConnectorServiceMethods.GetFirstDate(Me.Page)
                oParamList.Add(Format(FreezingDatePage, HelperWeb.GetShortDateFormat))

                lblCaptchaDesc1.Text = Me.Language.Translate("CloseDate.Warning.Message1", Me.DefaultScope, oParamList)
                spanCaptchaDesc2.InnerText = Me.Language.Translate("CloseDate.Warning.Message2", Me.DefaultScope)
            ElseIf Me.hdnCaptchaAction.Value.ToUpper = "SAVEWSPARAMS" Then
                lblCaptchaDesc1.Text = Me.Language.Translate("WSConfig.Warning.Message1", Me.DefaultScope)
                spanCaptchaDesc2.InnerText = Me.Language.Translate("WSConfig.Warning.Message2", Me.DefaultScope)
            Else
                If Me.Request("ShowFreezingDate") IsNot Nothing AndAlso Me.Request("ShowFreezingDate") = "1" Then

                    Dim FreezingDatePage As Date = API.ConnectorServiceMethods.GetFirstDate(Me.Page)

                    Dim strtxt As String = ""
                    If Me.hdnCaptchaAction.Value.ToUpper = "SAVELABAGREE" Then
                        strtxt = Me.Language.Translate("ShowLabAgreeFreezingDate.Message", Me.DefaultScope) & " " & Format(FreezingDatePage, HelperWeb.GetShortDateFormat)
                    Else
                        strtxt = Me.Language.Translate("ShowFreezingDate.Message", Me.DefaultScope) & " " & Format(FreezingDatePage, HelperWeb.GetShortDateFormat)
                    End If

                    spanCaptchaDesc2.InnerText = strtxt
                End If
            End If

        End If

    End Sub

    Protected Sub AuditCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles AuditCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "AUDIT"
                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)
                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tCaptcha, "", lstAuditParameterNames, lstAuditParameterValues, Me)
        End Select

    End Sub

End Class