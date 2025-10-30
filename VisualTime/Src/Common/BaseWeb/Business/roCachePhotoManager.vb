Imports System.Drawing
Imports System.Web
Imports System.Web.Caching

Public Class roCachePhotoManager
    Private Shared _MapPath As String = String.Empty
    Private Shared KeyCache As String = "AccessImgEmployee_"

    Shared Sub New()
    End Sub

    Private Sub New()
    End Sub

    Public Shared Function GetPhoto(ByVal oPage As System.Web.UI.Page, ByVal IdEmployee As Integer, ByVal Prefix As String, ByVal Measures As Byte) As String
        SyncLock (GetType(roCachePhotoManager))
            If HttpContext.Current.Cache(Prefix & KeyCache & IdEmployee.ToString) IsNot Nothing Then
                Return CStr(HttpRuntime.Cache(Prefix & KeyCache & IdEmployee.ToString))
            Else
                CachePhoto(oPage, IdEmployee, Prefix, Measures)
                Return "" & HttpRuntime.Cache(Prefix & KeyCache & IdEmployee.ToString)
            End If
        End SyncLock
    End Function

    Private Shared Sub CachePhoto(ByRef oPage As System.Web.UI.Page, ByVal IdEmployee As Integer, ByVal Prefix As String, ByVal Measures As Byte)
        SyncLock (GetType(roCachePhotoManager))
            ProcessImageEmployee(oPage, IdEmployee, Prefix, Measures)
        End SyncLock
    End Sub

    Public Shared Sub PhotoRemovedCallback(ByVal key As String, ByVal value As Object, ByVal removedReason As CacheItemRemovedReason)
        Try
        Catch ex As Exception
        End Try
    End Sub

    Private Shared Sub ProcessImageEmployee(ByRef oPage As System.Web.UI.Page, ByVal IdEmployee As Integer, ByVal Prefix As String, ByVal Measures As Byte)

        Try

            Dim oEmployee As Robotics.Base.VTEmployees.Employee.roEmployee = API.EmployeeServiceMethods.GetEmployee(oPage, IdEmployee, False)
            If oEmployee IsNot Nothing Then
                If oEmployee.Image IsNot Nothing AndAlso oEmployee.Image.Length > 0 Then

                    Dim bImage() As Byte = oEmployee.Image

                    If bImage IsNot Nothing AndAlso bImage.Length > 0 Then
                        Dim ms As IO.MemoryStream = New IO.MemoryStream()
                        ms.Write(bImage, 0, bImage.Length - 1)
                        Dim oImg As System.Drawing.Image = System.Drawing.Image.FromStream(ms)

                        If oImg.Width > Measures Or oImg.Height > Measures Then
                            oImg = HelperWeb.ResizeImage(oImg, Measures, Measures)

                            Dim converter As New ImageConverter
                            bImage = converter.ConvertTo(oImg, GetType(Byte()))
                        End If

                        Dim base64String As String = "data:image/png;base64," & Convert.ToBase64String(bImage, 0, bImage.Length)

                        HttpRuntime.Cache.Add(Prefix & KeyCache & IdEmployee.ToString, base64String, Nothing, Cache.NoAbsoluteExpiration, New TimeSpan(1, 0, 0), CacheItemPriority.Default, New CacheItemRemovedCallback(AddressOf PhotoRemovedCallback))

                    End If
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

End Class