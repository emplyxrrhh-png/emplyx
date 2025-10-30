Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Employees_EmployeeDownloadFile
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim strDocument As String = roTypes.Any2String(Request("Document"))
        If strDocument <> String.Empty Then

            Dim arrBytes() As Byte = Nothing
            Dim strErrorInfo As String = String.Empty

            arrBytes = GetDocumentToView(strDocument, strErrorInfo)
            If arrBytes IsNot Nothing Then
                If arrBytes.Length > 0 Then
                    LanzarDoc(arrBytes, strDocument)
                Else
                    Dim enc As New System.Text.UTF8Encoding()
                    Dim arrAux() As Byte = enc.GetBytes("-")
                    LanzarDoc(arrAux, strDocument)
                End If
            Else
                LanzarError(strErrorInfo)
            End If
        End If

    End Sub

    Private Sub LanzarDoc(ByRef arrBytes() As Byte, ByVal strDocument As String)

        Dim strNameFile As String = System.IO.Path.GetFileName(strDocument)
        Dim strExtensionFile As String = System.IO.Path.GetExtension(strDocument)

        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()

        Select Case strExtensionFile.ToUpper()

            Case ".PDF"
                Response.ContentType = "application/pdf"

            Case ".XLS", "XLSX"
                Response.ContentType = "application/vnd.ms-excel" 'no es valido --> application/x-excel

            Case ".DOC", ".DOCX"
                Response.ContentType = "application/msword"

            Case ".TXT"
                Response.ContentType = "text/plain"

            Case ".HTM", ".HTML", ".HTMLS"
                Response.ContentType = "text/html"

            Case ".JPG", ".JFIF", ".JPE", ".JPEG"
                Response.ContentType = "image/jpeg"

            Case ".BMP"
                Response.ContentType = "image/bmp"

            Case ".XML"
                Response.ContentType = "application/xml"

        End Select

        strNameFile = strNameFile.Replace(" ", "_")

        Response.AddHeader("Content-Disposition", "attachment; filename=""" & strNameFile.Replace(" ", "_") & """")
        Response.AddHeader("Content-Type", "application/octet-stream")
        Response.AddHeader("Content-Transfer-Encoding", "binary")
        Response.AddHeader("Content-Length", arrBytes.Length)
        Response.ContentType = "application/octet-stream"

        Response.OutputStream.Write(arrBytes, 0, arrBytes.Length)
        Response.Flush()
        Response.Close()

    End Sub

    Private Sub LanzarError(ByVal strTextoError As String)
        Dim str As String = "<html><body><H2>" & strTextoError & "</H2></body></html>"

        Response.Clear()
        Response.ClearHeaders()
        Response.ClearContent()
        Response.ContentType = "text/html"
        Response.Write(str)
        Response.Flush()
        Response.Close()

    End Sub

    Private Function GetDocumentToView(ByVal strDocument As String, ByRef strErrorInfo As String) As Byte()
        Dim arrByte() As Byte = Nothing

        Try

            arrByte = API.EmployeeServiceMethods.GetDocumentToView(Me, strDocument)
            If arrByte Is Nothing Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        End Try

        Return arrByte

    End Function

End Class