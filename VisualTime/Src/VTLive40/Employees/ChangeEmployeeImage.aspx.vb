Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Web.Base

Partial Class ChangeEmployeeImage
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.NameFoto"

    Private CurrentIDEmployee As Integer

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
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.CurrentIDEmployee, False)

                Const BUFFER_SIZE As Integer = 255
                Dim intBytesRead As Integer = 0
                Dim Buffer(BUFFER_SIZE) As Byte
                Dim strUploadedContent As StringBuilder = New StringBuilder("")

                Dim oStream As System.IO.Stream = Me.fuImageFile.PostedFile.InputStream

                '-----------------------------------------------------------------------
                Dim oImg As Image = Image.FromStream(oStream)
                Dim gr As Graphics = Graphics.FromImage(oImg)

                Dim bytImage(-1) As Byte

                Dim msResult As MemoryStream = New MemoryStream()
                If oImg.Width > 200 Or oImg.Height > 200 Then
                    Dim oResImage As System.Drawing.Image = HelperWeb.ResizeImage(oImg, 200, 200)
                    oResImage.Save(msResult, Imaging.ImageFormat.Jpeg)
                    ReDim bytImage(msResult.Length)
                    bytImage = msResult.ToArray()
                Else
                    oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
                    ReDim bytImage(msResult.Length)
                    bytImage = msResult.ToArray()
                End If
                msResult.Close()

                ReDim oEmployee.Image(bytImage.Length - 1)
                oEmployee.Image = bytImage

                bolChanged = True

                API.EmployeeServiceMethods.SaveEmployee(Me, oEmployee)
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
            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.CurrentIDEmployee, False)
            oEmployee.Image = Nothing
            API.EmployeeServiceMethods.SaveEmployee(Me, oEmployee)
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Me.CurrentIDEmployee = Request("ID")

        Me.oPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.CurrentIDEmployee)
        If Me.oPermission < Permission.Write Then
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

End Class