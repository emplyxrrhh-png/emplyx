Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class frmComplementary
    Inherits UserControlBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackCalendarRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="idShift")>
        Public idShift As Integer

    End Class

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        shiftDefinitionCallback.ClientInstanceName = Me.ClientID & "_shiftDefinitionCallbackClient"
        txtShiftFloatingStart.ClientInstanceName = Me.ClientID & "_txtShiftFloatingStartClient"
        cmbStartAtFloating.ClientInstanceName = Me.ClientID & "_cmbStartAtFloatingClient"

        txtShiftStart1.ClientInstanceName = Me.ClientID & "_txtShiftStart1Client"
        cmbStartAt1.ClientInstanceName = Me.ClientID & "_cmbStartAt1Client"
        txtShiftOrdinary1.ClientInstanceName = Me.ClientID & "_txtShiftOrdinary1Client"
        txtShiftComplementary1.ClientInstanceName = Me.ClientID & "_txtShiftComplementary1Client"

        txtShiftStart2.ClientInstanceName = Me.ClientID & "_txtShiftStart2Client"
        cmbStartAt2.ClientInstanceName = Me.ClientID & "_cmbStartAt2Client"
        txtShiftOrdinary2.ClientInstanceName = Me.ClientID & "_txtShiftOrdinary2Client"
        txtShiftComplementary2.ClientInstanceName = Me.ClientID & "_txtShiftComplementary2Client"

        cmbStartAtFloating.Items.Clear()
        cmbStartAtFloating.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
        cmbStartAtFloating.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
        cmbStartAtFloating.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))
        cmbStartAtFloating.SelectedIndex = 0

        cmbStartAt1.Items.Clear()
        cmbStartAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
        cmbStartAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
        cmbStartAt1.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))
        cmbStartAt1.SelectedIndex = 0

        cmbStartAt2.Items.Clear()
        cmbStartAt2.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftDay", DefaultScope), 1))
        cmbStartAt2.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftAfter", DefaultScope), 2))
        cmbStartAt2.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("ManualDetect.ShiftBefore", DefaultScope), 0))
        cmbStartAt2.SelectedIndex = 0
    End Sub

    Public Sub InitCallbacks(ByVal callbackFuncName As String)
        shiftDefinitionCallback.ClientSideEvents.EndCallback = callbackFuncName
    End Sub

    Protected Sub shiftDefinitionCallback_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles shiftDefinitionCallback.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackCalendarRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "SHIFTLAYERDEFINITION"
                Dim dayDescription = GetShiftLayersDefinition(oParameters, responseMessage)
                If responseMessage <> String.Empty Then
                    shiftDefinitionCallback.JSProperties.Add("cpMessage", responseMessage)
                    shiftDefinitionCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    shiftDefinitionCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(dayDescription))
                    shiftDefinitionCallback.JSProperties.Add("cpResult", "OK")
                End If
                shiftDefinitionCallback.JSProperties.Add("cpAction", "SHIFTLAYERDEFINITION")
        End Select
    End Sub

    Private Function GetShiftLayersDefinition(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarShift
        Dim oCalendarShiftDefinition As Robotics.Base.DTOs.roCalendarShift = Nothing

        Try

            oCalendarShiftDefinition = CalendarServiceMethods.GetShiftDefinition(Me.Page, oParameters.idShift)

            If oCalendarShiftDefinition Is Nothing Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.CalendarV2State.ErrorDetail
            End If
        Catch ex As Exception
            oCalendarShiftDefinition = Nothing
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return oCalendarShiftDefinition
    End Function

End Class