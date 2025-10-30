Imports Robotics.Web.Base

Partial Class Base_WebUserControls_UserPhoto
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Dim bImage As Byte() = WLHelperWeb.GetCurrentPassportPhoto()
        If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
            Response.Clear()
            Response.ContentType = "image/gif"
            Response.BinaryWrite(bImage)
        Else
            Dim oStrm As New System.IO.FileStream(Me.Server.MapPath("..\Images\userStart.png"), IO.FileMode.Open, IO.FileAccess.Read)
            Dim oReader As New System.IO.BinaryReader(oStrm)
            ReDim bImage(oReader.BaseStream.Length - 1)
            oStrm.Read(bImage, 0, bImage.Length)
            oReader.Read(bImage, 0, oReader.BaseStream.Length)
            Response.Clear()
            Response.ContentType = "image/png"
            Response.BinaryWrite(bImage)
            oReader.Close()
            oStrm.Close()
        End If

    End Sub

End Class