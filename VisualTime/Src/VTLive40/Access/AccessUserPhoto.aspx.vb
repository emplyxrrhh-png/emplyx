Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Web.Base

Partial Class AccessUserPhoto
    Inherits PageBase

    Private Const FeatureAliasEmp As String = "Employees.NameFoto"
    Private Const FeatureAliasAccess As String = "Access.Zones.Definition"

    Private oPermissionEmp As Permission
    Private oPermissionAccess As Permission

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Request("ID") = "" Or Not IsNumeric(Request("ID")) Then Exit Sub

        Me.oPermissionEmp = Me.GetFeaturePermissionByEmployee(FeatureAliasEmp, Request("ID"))
        Me.oPermissionAccess = Me.GetFeaturePermissionByEmployee(FeatureAliasAccess, Request("ID"))

        If oPermissionEmp = Permission.None Then
            ReturnImageDefault()
            Exit Sub
        End If

        If oPermissionAccess = Permission.None Then
            ReturnImageDefault()
            Exit Sub
        End If

        Dim bImage() As Byte = Nothing
        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Me.Request("ID"), False)

        If oEmployee IsNot Nothing Then
            If oEmployee.Image IsNot Nothing AndAlso oEmployee.Image.Length > 0 Then
                bImage = oEmployee.Image
            Else
                ReturnImageDefault()
                Exit Sub
            End If

            If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
                Dim ms As MemoryStream = New MemoryStream()

                ms.Write(bImage, 0, bImage.Length - 1)
                Dim oImg As Image = Image.FromStream(ms)
                Dim gr As Graphics = Graphics.FromImage(oImg)

                Dim bytImage(-1) As Byte

                If oImg.Width > 45 Or oImg.Height > 45 Then
                    Dim oResImage As System.Drawing.Image = HelperWeb.ResizeImage(oImg, 45, 45)

                    Dim msResult As MemoryStream = New MemoryStream()
                    oResImage.Save(msResult, Imaging.ImageFormat.Jpeg)
                    ReDim bytImage(msResult.Length)
                    bytImage = msResult.ToArray()
                    msResult.Close()
                Else
                    Dim msResult As MemoryStream = New MemoryStream()
                    oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
                    ReDim bytImage(msResult.Length)
                    bytImage = msResult.ToArray()
                    msResult.Close()
                End If

                Response.Clear()
                Response.ContentType = "image/png"
                Response.BinaryWrite(bytImage)
            Else
                Response.Write("Nothing to do")
            End If
        End If

    End Sub

    Private Sub ReturnImageDefault()
        Dim bImage() As Byte = Nothing

        Dim oStrm As New System.IO.FileStream(Me.Server.MapPath("..\Base\Images\userStart.png"), IO.FileMode.Open, IO.FileAccess.Read)
        Dim oReader As New System.IO.BinaryReader(oStrm)
        ReDim bImage(oReader.BaseStream.Length - 1)
        oStrm.Read(bImage, 0, bImage.Length)
        oReader.Read(bImage, 0, oReader.BaseStream.Length)
        oReader.Close()
        oStrm.Close()
        If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
            Dim ms As MemoryStream = New MemoryStream()

            ms.Write(bImage, 0, bImage.Length - 1)
            Dim oImg As Image = Image.FromStream(ms)
            Dim gr As Graphics = Graphics.FromImage(oImg)

            Dim bytImage(-1) As Byte

            If oImg.Width > 45 Or oImg.Height > 45 Then
                Dim oResImage As System.Drawing.Image = HelperWeb.ResizeImage(oImg, 45, 45)

                Dim msResult As MemoryStream = New MemoryStream()
                oResImage.Save(msResult, Imaging.ImageFormat.Jpeg)
                ReDim bytImage(msResult.Length)
                bytImage = msResult.ToArray()
                msResult.Close()
            Else
                Dim msResult As MemoryStream = New MemoryStream()
                oImg.Save(msResult, Imaging.ImageFormat.Jpeg)
                ReDim bytImage(msResult.Length)
                bytImage = msResult.ToArray()
                msResult.Close()
            End If

            Response.Clear()
            Response.ContentType = "image/png"
            Response.BinaryWrite(bytImage)
        Else
            Response.Write("Nothing to do")
        End If
    End Sub

End Class