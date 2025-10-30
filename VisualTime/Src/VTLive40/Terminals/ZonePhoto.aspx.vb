Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Web.Base

Partial Class ZonePhoto
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("ID") = "" Or Not IsNumeric(Request("ID")) Then Exit Sub

        Dim bImage() As Byte = Nothing

        Dim oZonePlane As roZonePlane = API.ZoneServiceMethods.GetZonePlaneByID(Me.Page, Me.Request("ID"), False)

        If oZonePlane IsNot Nothing Then
            If oZonePlane.PlaneImage IsNot Nothing AndAlso oZonePlane.PlaneImage.Length > 0 Then
                bImage = oZonePlane.PlaneImage
            End If

            If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
                Response.Clear()
                Response.ContentType = "image/jpeg"
                Response.BinaryWrite(bImage)
            Else
                Response.Write("Nothing to do")
            End If
        End If

        ''If Request("ID") = "" Or Not IsNumeric(Request("ID")) Then Exit Sub

        ''Dim bImage() As Byte = Nothing
        ''Dim oAccessZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, Me.Request("ID"))

        ''If oAccessZone IsNot Nothing Then
        ''    If oAccessZone.Image IsNot Nothing AndAlso oAccessZone.Image.Length > 0 Then
        ''        bImage = oAccessZone.Image
        ''    Else
        ''        If oAccessZone.ParentZone IsNot Nothing AndAlso oAccessZone.ParentZone.Image.Length > 0 Then
        ''            bImage = oAccessZone.ParentZone.Image
        ''        End If
        ''    End If

        ''    If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
        ''        Response.Clear()
        ''        Response.ContentType = "image/jpeg"
        ''        Response.BinaryWrite(bImage)
        ''    Else
        ''        Response.Write("Nothing to do")
        ''        'Dim oStrm As New System.IO.FileStream(Me.Server.MapPath("..\Images\userStart.png"), IO.FileMode.Open, IO.FileAccess.Read)
        ''        'Dim oReader As New System.IO.BinaryReader(oStrm)
        ''        'ReDim bImage(oReader.BaseStream.Length - 1)
        ''        'oStrm.Read(bImage, 0, bImage.Length)
        ''        'oReader.Read(bImage, 0, oReader.BaseStream.Length)
        ''        'Response.Clear()
        ''        'Response.ContentType = "image/png"
        ''        'Response.BinaryWrite(bImage)
        ''        'oReader.Close()
        ''        'oStrm.Close()
        ''    End If
        ''End If

    End Sub

End Class