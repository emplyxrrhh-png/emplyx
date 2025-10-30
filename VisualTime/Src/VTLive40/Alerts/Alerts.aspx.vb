Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Alerts
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private oPermission As Permission
    Private NotifiDS As DataSet
    Private NotifDat As Date

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Employees")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("Alerts", "~/Alerts/Scripts/Alerts.js")
        'Me.InsertExtraCssIncludes("~/Alerts/css/alerts.css")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
        End If
        lastUpdateText.Text = WLHelperWeb.LastAlertsLoadDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ")
        Me.LoadData(False)

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        Select Case oParameters.Action

            Case "REFRESH"
                bRet = LoadData(True)

                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "REFRESH")
                ASPxCallbackPanelContenido.JSProperties.Add("cpLastUpdate", WLHelperWeb.LastAlertsLoadDate.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadData(Optional ByVal bolReload As Boolean = False) As Boolean
        Dim bRet As Boolean = False

        Dim tsAlerts As DataTable = WLHelperWeb.UserAlerts(bolReload)
        Dim tbAdvices As DataTable = WLHelperWeb.SystemAlerts(bolReload)
        Dim tbEmployeeDocuments As DocumentAlerts = WLHelperWeb.EmployeeDocumentationAlerts(bolReload, True)
        Dim tbCompanyDocuments As DocumentAlerts = WLHelperWeb.CompanyDocumentationAlerts(bolReload, True)

        If (Not tsAlerts Is Nothing AndAlso tsAlerts.Rows.Count > 0) OrElse
            (Not tbAdvices Is Nothing AndAlso tbAdvices.Rows.Count > 0 OrElse
            (tbEmployeeDocuments.DocumentsValidation.Count + tbEmployeeDocuments.AbsenteeismDocuments.Count + tbEmployeeDocuments.MandatoryDocuments.Count + tbEmployeeDocuments.WorkForecastDocuments.Count + tbEmployeeDocuments.AccessAuthorizationDocuments.Count) > 0 OrElse
            (tbCompanyDocuments.DocumentsValidation.Count + tbCompanyDocuments.AbsenteeismDocuments.Count + tbCompanyDocuments.MandatoryDocuments.Count + tbCompanyDocuments.WorkForecastDocuments.Count + tbCompanyDocuments.AccessAuthorizationDocuments.Count) > 0) Then

            Me.NoAlerts.Style("display") = "none"
            Me.Alerts.Style("display") = ""

            Me.userAlerts.Style("display") = "none"
            Me.systemAlerts.Style("display") = "none"

            If (tsAlerts IsNot Nothing AndAlso tsAlerts.Rows.Count > 0) OrElse
                (tbEmployeeDocuments.DocumentsValidation.Count + tbEmployeeDocuments.AbsenteeismDocuments.Count + tbEmployeeDocuments.MandatoryDocuments.Count + tbEmployeeDocuments.WorkForecastDocuments.Count + tbEmployeeDocuments.AccessAuthorizationDocuments.Count) > 0 OrElse
                (tbCompanyDocuments.DocumentsValidation.Count + tbCompanyDocuments.WorkForecastDocuments.Count + tbCompanyDocuments.MandatoryDocuments.Count + tbCompanyDocuments.GpaAlerts.Count + tbCompanyDocuments.AccessAuthorizationDocuments.Count) > 0 Then
                Me.userAlerts.Style("display") = ""
                Me.userAlertsCount.InnerText = tsAlerts.Rows.Count +
                    If(tbEmployeeDocuments.MandatoryDocuments.Count > 0, 1, 0) +
                    If(tbEmployeeDocuments.DocumentsValidation.Count > 0, 1, 0) +
                    If(tbEmployeeDocuments.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tbEmployeeDocuments.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(tbEmployeeDocuments.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tbEmployeeDocuments.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(tbEmployeeDocuments.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tbEmployeeDocuments.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(tbCompanyDocuments.MandatoryDocuments.Count > 0, 1, 0) +
                    If(tbCompanyDocuments.DocumentsValidation.Count > 0, 1, 0) +
                    If(tbCompanyDocuments.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tbCompanyDocuments.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0)

                Dim alertDiv As String = String.Empty
                For Each row As DataRow In tsAlerts.Rows
                    alertDiv &= roAlertsCommon.BuildAlertDiv(row, True, Request)
                Next

                alertDiv &= roAlertsCommon.BuildDocumentAlertsDiv(tbEmployeeDocuments, DocumentType.Employee, -1, Me.Language, Me.DefaultScope, Me.Request)

                alertDiv &= roAlertsCommon.BuildDocumentAlertsDiv(tbCompanyDocuments, DocumentType.Company, -1, Me.Language, Me.DefaultScope, Me.Request)

                Me.userAlertsContent.InnerHtml = alertDiv
            End If

            If Not tbAdvices Is Nothing AndAlso tbAdvices.Rows.Count > 0 Then
                Me.systemAlerts.Style("display") = ""
                Me.systemAlertsCount.InnerText = tbAdvices.Rows.Count

                Dim bHasLink As Boolean = False
                Dim systemDiv As String = String.Empty
                For Each row As DataRow In tbAdvices.Rows
                    systemDiv &= roAlertsCommon.BuildAlertDiv(row, False, Request)
                Next
                Me.systemAlertsContent.InnerHtml = systemDiv
            End If
        Else
            Me.NoAlerts.Style("display") = ""
            Me.Alerts.Style("display") = "none"

            Dim noAlertMsg As String = "<div class='mainBlock' style='text-align:initial' ><div class='mainCentered'><div  class='Alert_BoxLine'>&nbsp;</div><div  class='Alert_BoxLine'><span>" & Me.Language.Translate("EverythingOK", Me.DefaultScope) & "</span></div><div  class='Alert_BoxLine'>&nbsp;</div></div></div>"
            Me.noAlertMsg.InnerHtml = noAlertMsg
        End If

        Return bRet

    End Function

End Class