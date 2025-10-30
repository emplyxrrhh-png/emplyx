Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentPendingManagment
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Public Property ForecastType() As ForecastType
        Get
            Return Session(Me.ClientID & "DocumentPendingManagment_ForecastType")
        End Get

        Set(ByVal value As ForecastType)
            Session(Me.ClientID & "DocumentPendingManagment_ForecastType") = value
        End Set
    End Property

    Public Property IdRelatedObject() As Integer
        Get
            Return Session(Me.ClientID & "DocumentPendingManagment_IdEmployee")
        End Get

        Set(ByVal value As Integer)
            Session(Me.ClientID & "DocumentPendingManagment_IdEmployee") = value
        End Set
    End Property

    Public Property IdForecast() As Integer
        Get
            Return Session(Me.ClientID & "DocumentPendingManagment_IdForecast")
        End Get

        Set(ByVal value As Integer)
            Session(Me.ClientID & "DocumentPendingManagment_IdForecast") = value
        End Set
    End Property

    Public Property Type() As DocumentType
        Get
            Return Session(Me.ClientID & "DocumentPendingManagment_Type")
        End Get

        Set(ByVal value As DocumentType)
            Session(Me.ClientID & "DocumentPendingManagment_Type") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Init
        Me.CallbackSessionDocPen.ClientInstanceName = ClientID & "_CallbackSessionDocPenClient"
        Me.ASPxCallbackPanelContenido.ClientInstanceName = ClientID & "_ASPxCallbackPanelContenidoClient"
    End Sub

    Public Sub New()

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        If Not IsPostBack Then

        End If
    End Sub

    Public Sub LoadAlerts(idRelatedObject As Integer, typeCaller As DocumentType, ByVal IdForecast As Integer, ByVal eForecastType As ForecastType)
        ClearSessionVariables()
        Me.IdRelatedObject = idRelatedObject
        Me.IdForecast = IdForecast
        Me.Type = typeCaller
        Me.ForecastType = eForecastType

        If (hdnScopePendingInfo.Contains("IdRelatedObject")) Then
            hdnScopePendingInfo("IdRelatedObject") = idRelatedObject
        Else
            hdnScopePendingInfo.Add("IdRelatedObject", idRelatedObject)
        End If

        If (hdnScopePendingInfo.Contains("ForecastType")) Then
            hdnScopePendingInfo("ForecastType") = CInt(eForecastType)
        Else
            hdnScopePendingInfo.Add("ForecastType", CInt(eForecastType))
        End If

        If (hdnScopePendingInfo.Contains("Type")) Then
            hdnScopePendingInfo("Type") = Type
        Else
            hdnScopePendingInfo.Add("Type", Type)
        End If

        LoadData(True)
    End Sub

    Private Sub ClearSessionVariables()
        Session("DocumentPendingManagment_IdEmployee") = Nothing
        Session("DocumentPendingManagment_Type") = Nothing
        Session("DocumentPendingManagment_Documents") = Nothing
        Session("DocumentPendingManagment_ForecastType") = Nothing
        Session("DocumentPendingManagment_IdForecast") = Nothing

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim bRet As Boolean = False

        Select Case e.Parameter

            Case "REFRESH"
                bRet = LoadData(True)

                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "REFRESH")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadData(Optional ByVal bolReload As Boolean = False) As Boolean
        Dim bRet As Boolean = False

        Dim tsAlerts As DocumentAlerts = Nothing
        If (Me.IdForecast = -2) Then
            tsAlerts = DocumentsServiceMethods.GetDocumentationFaults(Me.Page, Me.Type, Me.IdRelatedObject, If(Me.IdForecast < 0, 0, Me.IdForecast), ForecastType.Any)
        Else
            tsAlerts = DocumentsServiceMethods.GetDocumentationFaults(Me.Page, Me.Type, Me.IdRelatedObject, If(Me.IdForecast < 0, 0, Me.IdForecast), Me.ForecastType)
        End If

        If Not tsAlerts Is Nothing AndAlso ((tsAlerts.DocumentsValidation.Count > 0) OrElse (tsAlerts.AbsenteeismDocuments.Count > 0) _
            OrElse (tsAlerts.MandatoryDocuments.Count > 0) OrElse (tsAlerts.WorkForecastDocuments.Count > 0) OrElse (tsAlerts.AccessAuthorizationDocuments.Count > 0)) AndAlso Me.IdForecast <> -1 Then
            Me.NoAlerts.Style("display") = "none"
            Me.Alerts.Style("display") = ""

            Me.userAlerts.Style("display") = "none"

            If Not (Me.ForecastType = ForecastType.Any OrElse Me.ForecastType = ForecastType.AnyAbsence) Then
                Me.alertsTitle.Style("display") = "none"
                Me.alertCount.Style("display") = "none"
                Me.userAlerts.Style("display") = ""
                Me.userAlertsCount.InnerText = "0"
                Me.alertSpacing.Style("display") = "none"
                Me.alertsBlock.Attributes("style") = ";float:left;height:initial;padding-left:50px;padding-top:10px;padding-bottom: 20px;max-height:50px"
            Else
                Me.alertsTitle.Style("display") = "none"
                Me.userAlerts.Style("display") = ""
                Me.alertSpacing.Style("display") = ""
                Me.userAlertsCount.InnerText = (
                    If(tsAlerts.DocumentsValidation.Count > 0, 1, 0) +
                    If(tsAlerts.MandatoryDocuments.Count > 0, 1, 0) +
                    If(tsAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tsAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(tsAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tsAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(tsAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) + If(tsAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0)
                    )
            End If

            Me.userAlertsContent.InnerHtml = roAlertsCommon.BuildDocumentAlertsDiv(tsAlerts, Me.Type, Me.IdRelatedObject, Me.Language, Me.DefaultScope, Me.Request, Me.ForecastType)
        Else

            Me.NoAlerts.Style("display") = ""
            Me.Alerts.Style("display") = "none"

            Dim noAlertMsg As String = String.Empty

            If Me.ForecastType <> ForecastType.Any Then
                Me.noAlertCount.Style("display") = "none"
                Me.noAlertCount.Style("style") = "float:left;height:initial;padding-left:50px;"
                Me.noAlertMsg.Style("style") = "float:left;height:initial;padding-left:50px;"
                Me.alertSpacing.Style("display") = "none"
                noAlertMsg = "<div class='mainBlock' style='height:initial;text-align:initial;max-height:50px' ><div class='mainCentered'><div  class='Alert_BoxLine'><span>" & Me.Language.Translate("EverythingOK", Me.DefaultScope) & "</span></div><div  class='Alert_BoxLine'>&nbsp;</div></div></div>"
            Else
                Me.noAlertCount.Style("display") = ""
                Me.noAlertMsg.Style("style") = ";float:right; width:69%; height:200px !important;"
                Me.NoAlerts.Style("style") = ";height:100%;padding-top:20px;"
                noAlertMsg = "<div class='mainBlock' style='height:200px !important;text-align:initial' ><div class='mainCentered'><div  class='Alert_BoxLine'>&nbsp;</div><div  class='Alert_BoxLine'><span>" & Me.Language.Translate("EverythingOK", Me.DefaultScope) & "</span></div><div  class='Alert_BoxLine'>&nbsp;</div></div></div>"
            End If

            Me.noAlertMsg.InnerHtml = noAlertMsg
        End If

        Return bRet

    End Function

End Class