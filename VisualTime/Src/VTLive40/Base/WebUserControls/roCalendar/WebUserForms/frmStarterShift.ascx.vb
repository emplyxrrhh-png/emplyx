Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class frmStarterShift
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

        txtShiftStart1.ClientInstanceName = Me.ClientID & "_txtShiftStart1Client"
        txtShiftEnd1.ClientInstanceName = Me.ClientID & "_txtShiftEnd1Client"
        txtShiftOrdinary1.ClientInstanceName = Me.ClientID & "_txtShiftOrdinary1Client"
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