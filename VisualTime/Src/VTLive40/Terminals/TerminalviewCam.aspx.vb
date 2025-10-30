Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Camera
Imports Robotics.Web.Base

Partial Class TerminalviewCam
    Inherits PageBase

    Private Const FeatureAliasCamera As String = "Administration.Cameras.Visualization"
    Private oPermissionCamera As Permission

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Me.Request("ID") = "" Then Exit Sub

        Me.oPermissionCamera = Me.GetFeaturePermission(FeatureAliasCamera)
        If oPermissionCamera = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Dim oCamera As roCamera = API.CameraServiceMethods.GetCameraByID(Me.Page, Request("ID"), True)
        If oCamera IsNot Nothing Then
            Dim strUrl As String = ""
            If oCamera.Url <> "" Then
                strUrl = oCamera.Url
                If Not strUrl.Contains("://") Then
                    strUrl = "http://" & strUrl
                End If
            End If
            Me.Page.Response.Redirect(strUrl, True)
        End If

    End Sub

End Class