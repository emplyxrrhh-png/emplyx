Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class DocumentVisualize
    Inherits NoCachePageBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As EventArgs) Handles Me.Load
        'If Not IsPostBack Then
        Dim idDeliveredDocument = Split(Request.Params("DeliveredDocument"), ",")
        Dim idDeliveredSign = Split(Request.Params("DeliveredSign"), ",")
        Dim pgpKeyParam = Request.Params("VTPublicPGPKey")
        Dim bError As Boolean = True

        If idDeliveredDocument.Length > 0 Then
            If idDeliveredDocument.Length = 1 Then
                If idDeliveredDocument(0) <> "" Then
                    Dim oDoc As roDocument = API.DocumentsServiceMethods.GetDocumentById(Nothing, idDeliveredDocument(0), False)

                    If oDoc IsNot Nothing Then
                        Dim oTemplate As roDocumentTemplate = API.DocumentsServiceMethods.GetDocumentTemplateById(Nothing, oDoc.DocumentTemplate.Id, False)

                        If oTemplate IsNot Nothing Then
                            If oDoc.IdEmployee > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, oDoc.IdEmployee, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                                bError = False
                                VisualizeDocument(DocumentsServiceMethods.GetDocumentFile(Page, idDeliveredDocument(0), True))
                            End If

                            If oDoc.IdCompany > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Nothing, oDoc.IdCompany, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                                bError = False
                                VisualizeDocument(DocumentsServiceMethods.GetDocumentFile(Page, idDeliveredDocument(0), True))
                            End If
                        End If

                    End If

                    If bError Then
                        Response.Clear()
                        Response.ClearHeaders()
                        Response.ClearContent()

                        Response.ContentType = "image/gif"
                        Response.WriteFile(Me.Server.MapPath("..\Reports\Images\PRINTER_REMOVE_256.GIF"))
                        Response.Flush()
                    End If

                End If
            Else
                'Función no soportada en HA
            End If
        End If

        If idDeliveredSign.Length > 0 Then
            If idDeliveredSign.Length = 1 Then
                If idDeliveredSign(0) <> "" Then
                    Dim oDoc As roDocument = API.DocumentsServiceMethods.GetDocumentById(Nothing, idDeliveredSign(0), False)

                    If oDoc IsNot Nothing Then
                        Dim oTemplate As roDocumentTemplate = API.DocumentsServiceMethods.GetDocumentTemplateById(Nothing, oDoc.DocumentTemplate.Id, False)

                        If oTemplate IsNot Nothing Then
                            If oDoc.IdEmployee > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, oDoc.IdEmployee, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                                bError = False
                                VisualizeDocument(DocumentsServiceMethods.GetSignReportDocumentBytesById(Page, idDeliveredSign(0), True))
                            End If

                            If oDoc.IdCompany > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Nothing, oDoc.IdCompany, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                                bError = False
                                VisualizeDocument(DocumentsServiceMethods.GetSignReportDocumentBytesById(Page, idDeliveredSign(0), True))
                            End If
                        End If

                    End If

                    If bError Then
                        Response.Clear()
                        Response.ClearHeaders()
                        Response.ClearContent()

                        Response.ContentType = "image/gif"
                        Response.WriteFile(Me.Server.MapPath("..\Reports\Images\PRINTER_REMOVE_256.GIF"))
                        Response.Flush()
                    End If

                End If
            Else

            End If
        End If

        If Not String.IsNullOrEmpty(pgpKeyParam) Then
            Dim vtPublicKey As Byte() = Azure.RoAzureSupport.DownloadFile("certificates/vt-pgppublic.asc", roLiveQueueTypes.datalink)
            If vtPublicKey IsNot Nothing AndAlso vtPublicKey.Length > 0 Then
                Response.Clear()
                Response.ClearHeaders()
                Response.ClearContent()

                Response.ContentType = "application/pgp-keys"
                Response.AddHeader("Content-Disposition", "attachment; filename=vt-pgppublic.asc")
                Response.BinaryWrite(vtPublicKey)
                Response.End()
            End If
        End If
        'End If
    End Sub

    Protected Sub VisualizeDocument(ByVal deliveredDocument As roDocumentFile)

        Try

            Response.Clear()
            Response.ClearHeaders()
            Response.ClearContent()

            Dim strNameFile = "_"

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