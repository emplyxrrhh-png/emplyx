Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.Web.Base

Partial Class ChangeZoneImage
    Inherits PageBase

    Private Const FeatureAlias As String = "Access.Zones.Definition"

    Private CurrentIDZone As Integer

    Private oPermission As Permission

    Protected Sub btOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOK.Click
        'Si esta marcat el checkbox per borrar la imatge
        If Me.checkDel.Checked = True Then
            DeleteImage()
            Me.MustRefresh = "6"
            Me.CanClose = True
            Exit Sub
        End If

        If Me.fuImageFile.PostedFile IsNot Nothing Then

            Dim bolChanged As Boolean = False

            Try
                Dim oAccessZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me, Me.CurrentIDZone, False)

                Const BUFFER_SIZE As Integer = 255
                Dim intBytesRead As Integer = 0
                Dim Buffer(BUFFER_SIZE) As Byte
                Dim strUploadedContent As StringBuilder = New StringBuilder("")

                Dim oStream As System.IO.Stream = Me.fuImageFile.PostedFile.InputStream

                ReDim oAccessZone.Image(oStream.Length - 1)
                oStream.Read(oAccessZone.Image, 0, oStream.Length)

                bolChanged = True

                API.ZoneServiceMethods.SaveZone(Me, oAccessZone, True)
            Catch ex As Exception

            End Try

            If bolChanged Then

                Me.MustRefresh = "6"
                Me.CanClose = True

            End If

        End If

    End Sub

    Private Sub DeleteImage()
        Try
            Dim oAccessZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me, Me.CurrentIDZone, False)
            oAccessZone.Image = Nothing
            API.ZoneServiceMethods.SaveZone(Me, oAccessZone, True)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Me.CurrentIDZone = Request("ID")

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission < Permission.Write Then
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

End Class