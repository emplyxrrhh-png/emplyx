Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Camera
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Cameras
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    Private oPermission As Permission

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Administration.Cameras.Definition")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")

        Me.InsertExtraJavascript("Camera", "~/Cameras/Scripts/Camera.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roTreesCameras.TreeCaption = Me.Language.Translate("TreeCaptionCameras", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
            txtDescription.Enabled = False
            cmbModel.Enabled = False
        End If

        'PPR desactivado temporalmente NO ELIMINAR-->
        'Dim oPermission As Permission = Me.GetFeaturePermission("Administration.Cameras.Definition")
        'If oPermission = Permission.None Or Not String.IsNullOrEmpty(Request.Url.Query) Then
        '    If Not String.IsNullOrEmpty(Request.Url.Query) Then
        '        If Uri.IsWellFormedUriString(Request.Url.AbsoluteUri, UriKind.Absolute) Then
        '            WLHelperWeb.PendingUserSessionQueryString("Cameras") = Request.Url.PathAndQuery
        '            WLHelperWeb.RedirectDefault()
        '        Else
        '            WLHelperWeb.RedirectAccessDenied(False)
        '        End If
        '    Else
        '        WLHelperWeb.RedirectAccessDenied(False)
        '    End If
        '    Exit Sub
        'End If

        If Not API.CameraServiceMethods.ExitsCamera(Me.Page, -1) Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        'PPR desactivado temporalmente NO ELIMINAR-->
        'Dim oQueryString As New roQueryStringState("Cameras")
        'If CameraService.CameraServiceMethods.ExitsCamera(Me.Page, -1) Then
        '    oQueryString.HasReg = "1"
        'Else
        '    oQueryString.HasReg = "0"
        'End If
        'ProcessQueryString(oQueryString, WLHelperWeb.PendingCookieQueryString("Cameras", True))
        'HelperWeb.roPage_SetQueryStringState(oQueryString)

        If Not Me.IsPostBack And Not Me.IsCallback Then
            Me.LoadCamerasData()
        End If

    End Sub

    'PPR desactivado temporalmente NO ELIMINAR-->
    'Private Sub ProcessQueryString(ByRef oQueryString As roQueryStringState, ByVal strQueryString As String)
    '    Try
    '        If strQueryString <> String.Empty Then

    '            Dim dic As Generic.Dictionary(Of QuerystringDefaultParam, String) = GetParamsFromQuerystring(strQueryString)
    '            For Each item As Generic.KeyValuePair(Of QuerystringDefaultParam, String) In dic
    '                Select Case item.Key

    '                    Case QuerystringDefaultParam.ID
    '                        Dim treePath As String = String.Empty
    '                        If CameraService.CameraServiceMethods.ExitsCamera(Me.Page, item.Value) Then
    '                            treePath = "/source/" & item.Value
    '                        End If
    '                        HelperWeb.roSelector_SetSelection(item.Value, treePath, "ctl00_contentMainBody_roTreesCameras")

    '                    Case QuerystringDefaultParam.TAB
    '                        If item.Value <> "" Then
    '                            oQueryString.ActiveTab = item.Value
    '                        End If

    '                End Select
    '            Next
    '        End If
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean

        Select Case oParameters.Action

            Case "GETCAMERA"
                bRet = LoadCamera(oParameters.ID, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETCAMERA")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

            Case "SAVECAMERA"
                bRet = SaveCamera(oParameters.ID, oParameters.Name, responseMessage)

                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVECAMERA")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                Else
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETCAMERA")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", True)
                End If

            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadCamera(ByRef IdCamera As Integer, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim oCurrentCamera As roCamera

                If IdCamera = -1 Then
                    oCurrentCamera = New roCamera()
                    oCurrentCamera.ID = -1
                Else
                    oCurrentCamera = CameraServiceMethods.GetCameraByID(Me.Page, IdCamera, True)
                End If

                txtDescription.Value = oCurrentCamera.Description
                txtURL.Value = oCurrentCamera.Url
                txtName.Text = oCurrentCamera.Name

                Dim it As DevExpress.Web.ListEditItem = cmbModel.Items.FindByValue(oCurrentCamera.Model)
                If it Is Nothing Then
                    cmbModel.SelectedIndex = 0
                Else
                    cmbModel.SelectedItem = it
                End If

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function SaveCamera(ByRef IdCamera As Integer, ByRef CameraName As String, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission >= Permission.Write Then

                Dim oCurrentCamera As roCamera = CameraServiceMethods.GetCameraByID(Me.Page, IdCamera, False)
                If Not oCurrentCamera Is Nothing Then

                    oCurrentCamera.ID = IdCamera
                    oCurrentCamera.Name = txtName.Text
                    oCurrentCamera.Description = txtDescription.Value
                    oCurrentCamera.Url = txtURL.Value

                    If Not cmbModel.SelectedItem Is Nothing Then
                        oCurrentCamera.Model = cmbModel.SelectedItem.Value
                    End If

                    bRet = CameraServiceMethods.SaveCamera(Me.Page, oCurrentCamera, True)
                    If bRet Then
                        Dim treePath As String = "/source/" & oCurrentCamera.ID
                        HelperWeb.roSelector_SetSelection(oCurrentCamera.ID.ToString, treePath, "ctl00_contentMainBody_roTreesCameras")
                    Else
                        ErrorInfo = CameraServiceMethods.LastErrorText
                    End If
                Else
                    ErrorInfo = CameraServiceMethods.LastErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Sub LoadCamerasData()
        cmbModel.Items.Clear()
        cmbModel.Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Model.Standard", DefaultScope), " "))
    End Sub

End Class