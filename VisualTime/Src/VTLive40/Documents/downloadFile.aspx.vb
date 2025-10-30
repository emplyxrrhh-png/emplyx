Imports System.IO
Imports System.IO.Compression
Imports DevExpress.XtraSpreadsheet.Model
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class certificate_downloadFile
    Inherits NoCachePageBase

    Private _language As roLanguageWeb = Nothing

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Dim clientId As String = String.Empty
        Dim certificateName As String = String.Empty

        Dim requestPaths As String() = Request.Url.LocalPath.Split("/")
        If requestPaths(1).ToUpper = "BIOCERTIFICATE" AndAlso requestPaths.Length = 4 Then
            clientId = requestPaths(2).ToLower()
            certificateName = requestPaths(3).ToLower()
        End If



        If Not String.IsNullOrEmpty(clientId) AndAlso Not String.IsNullOrEmpty(certificateName) Then

            If WLHelperWeb.CurrentPassport Is Nothing OrElse (WLHelperWeb.CurrentPassport IsNot Nothing AndAlso Robotics.Azure.RoAzureSupport.GetCompanyName() = clientId) Then
                Dim bSetCompanyInfo As Boolean = Robotics.Azure.RoAzureSupport.GetCompanyName().ToLower <> clientId.ToLower
                Try
                    If bSetCompanyInfo Then
                        Web.HttpContext.Current.Session("roMultiCompanyId") = clientId
                        Global_asax.ReloadSharedData()
                    End If

                    Dim oDocument As roDocumentFile = DocumentsServiceMethods.GetBioCertificateBytes(Page, certificateName, True)

                    If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty

                    If oDocument IsNot Nothing Then
                        VisualizeDocument(oDocument)
                    Else





                        HttpContext.Current.Session("CustomErrorMessage") = TranslateLabel("NotFound") ' "No se ha podido obtener el certificado"
                        WLHelperWeb.RedirectToUrl("/Base/Ooops.aspx")
                    End If
                Catch ex As Exception
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "certificate_downloadFile::", ex)
                Finally
                    If bSetCompanyInfo Then Web.HttpContext.Current.Session("roMultiCompanyId") = String.Empty
                End Try


            Else
                HttpContext.Current.Session("CustomErrorMessage") = TranslateLabel("AlreadyLoggedIn") '"Para poder acceder al certificado debe cerrar la sesión de Visualtime"
                WLHelperWeb.RedirectToUrl("/Base/Ooops.aspx")
            End If
        Else
            HttpContext.Current.Session("CustomErrorMessage") = TranslateLabel("NotFound") '"No se ha podido obtener el certificado"
            WLHelperWeb.RedirectToUrl("/Base/Ooops.aspx")
        End If


    End Sub

    Protected Function TranslateLabel(ByVal labelKey As String) As String
        Dim serverLanguage As String = API.CommonServiceMethods.DefaultLanguage()

        If _language Is Nothing Then
            _language = New roLanguageWeb()
            _language.SetLanguageReference("LivePortal", serverLanguage)
        End If

        Return _language.Translate(labelKey, "biocertificate")
    End Function

    Protected Sub VisualizeDocument(ByVal deliveredDocument As roDocumentFile)

        Try

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()

            Response.AddHeader("Content-Disposition", "attachment; filename=""" & deliveredDocument.DocumentName.Replace(" ", "_") & """")
            Response.AddHeader("Content-Type", "application/octet-stream")
            Response.AddHeader("Content-Transfer-Encoding", "binary")
            Response.AddHeader("Content-Length", deliveredDocument.DocumentContent.Length)
            Response.ContentType = "application/octet-stream"
            Response.OutputStream.Write(deliveredDocument.DocumentContent, 0, deliveredDocument.DocumentContent.Length)
            Response.Flush()
            Response.Close()
        Catch ex As Exception
            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()

            Response.ContentType = "image/gif"
            Response.WriteFile(Me.Server.MapPath("..\Reports\Images\PRINTER_REMOVE_256.GIF"))
            Response.Flush()
        End Try

    End Sub

End Class