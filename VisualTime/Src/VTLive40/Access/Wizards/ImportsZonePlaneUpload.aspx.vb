Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Web.Base

Partial Class ImportsZonePlaneUpload
    Inherits PageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Access.Zones", Permission.Write) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

    End Sub

    Protected Sub btnSend_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnSend.Click
        Dim strMsg As String = ""

        If Not fileOrig.HasFile Then
            strMsg = Language.Translate("FileImport.NotFound", DefaultScope)
        End If

        If Me.hdnIDPlane.Value = "" Or hdnIDPlane.Value = "-1" Then
            strMsg = Language.Translate("FileImport.NoSelectedPlaneZone", DefaultScope)
        End If

        If strMsg <> "" Then
            Me.lblMsg.Text = strMsg
            Me.lblMsg.Visible = True
        Else
            'Session.Remove("ImgPlaneFileOrig")
            'Session.Add("ImgPlaneFileOrig", fileOrig.FileBytes)

            Dim oZonePlane As roZonePlane
            If Me.hdnIDPlane.Value <> "" And hdnIDPlane.Value <> "-1" Then
                oZonePlane = API.ZoneServiceMethods.GetZonePlaneByID(Me.Page, hdnIDPlane.Value, False)
                If oZonePlane IsNot Nothing Then
                    oZonePlane.PlaneImage = fileOrig.FileBytes
                    API.ZoneServiceMethods.SaveZonePlane(Me.Page, oZonePlane, True)
                End If
            End If

            'Me.Controls.Clear()
            Me.Response.Write("<script language=""javascript"">parent.showImage(parent.retIDPlane());document.reload();</script>")

        End If

    End Sub

End Class