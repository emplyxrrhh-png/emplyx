Imports Robotics.Base.DTOs

Public Class DocumentsHelper

    Public Sub New()
    End Sub

    'documents format {"idDoc#idTemplate#idEmployee#idGroup"}
    'templates format {"idTemplate#idpassport1,idpassport2,idpassport3..idpassportn"}
    Public Function SetAvailableDocuments(idPassport As Integer, documents As String(), templates As String())

        Robotics.Web.Base.API.Fakes.ShimDocumentsServiceMethods.GetDocumentByIdPageInt32Boolean =
            Function(reference, idDocument, audit)

                For Each oDocFormat As String In documents
                    Dim docFormat As String() = oDocFormat.Split("#")
                    If docFormat(0) = idDocument.ToString() Then
                        Return New roDocument() With {
                        .Id = idDocument,
                        .DocumentTemplate = New roDocumentTemplate() With {.Id = docFormat(1)},
                        .IdEmployee = CInt(docFormat(2)),
                        .IdCompany = CInt(docFormat(3))
                        }
                    End If
                Next

            End Function

        Robotics.Web.Base.API.Fakes.ShimDocumentsServiceMethods.GetDocumentTemplateByIdPageInt32Boolean =
            Function(reference, idTemplate, audit)

                For Each oTemplateFormat As String In templates
                    Dim docFormat As String() = oTemplateFormat.Split("#")
                    If docFormat(0) = idTemplate.ToString() Then
                        Dim availableSupervisors As String() = docFormat(1).Split(",")

                        For Each aSupervisor In availableSupervisors
                            If CInt(aSupervisor) = idPassport Then
                                Return New roDocumentTemplate() With {.Id = idTemplate}
                            End If
                        Next

                    End If
                Next

                Return Nothing
            End Function

        Robotics.Web.Base.API.Fakes.ShimDocumentsServiceMethods.GetDocumentFilePageInt32Boolean =
            Function(reference, idDocument, audit)
                Return New roDocumentFile() With {.DocumentContent = {42, 64}, .DocumentName = "loaded"}
            End Function

        Robotics.Web.Base.API.Fakes.ShimDocumentsServiceMethods.DeleteDocumentPageInt32Boolean =
           Function(reference, idDocument, audit)
               Return True
           End Function

    End Function

End Class