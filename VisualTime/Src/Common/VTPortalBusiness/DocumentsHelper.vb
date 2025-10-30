Imports System.Collections.Generic
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Security.Base

Namespace VTPortal

    Public Class DocumentsHelper

        Public Shared Function GetMyDocuments(ByVal idEmployee As Integer, ByVal iDate As DateTime, ByVal eDate As DateTime, ByVal filter As String, ByVal orderBy As String, ByRef oDocState As VTDocuments.roDocumentState) As roEmployeeDocumentsResponse
            Dim bSaved As New roEmployeeDocumentsResponse

            Try
                Dim oManager As New VTDocuments.roDocumentManager(oDocState)

                Dim oDocuments As Generic.List(Of roDocument) = oManager.GetDocumentsByEmployee(idEmployee, iDate, eDate, filter, orderBy)

                bSaved.Documents = oDocuments.ToArray
                If oManager.State.Result = DocumentResultEnum.NoError Then
                    bSaved.Status = ErrorCodes.OK
                Else
                    bSaved.Documents = {}
                    bSaved.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                bSaved.Documents = {}
                bSaved.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::GetMyDocuments")
            End Try

            Return bSaved
        End Function

        Public Shared Function GetDocumentBytes(idDocument As Integer, idEmployee As Integer, ByVal oState As VTDocuments.roDocumentState) As roGenericResponse(Of roDocumentFile)
            Dim result As roGenericResponse(Of roDocumentFile) = New roGenericResponse(Of roDocumentFile)
            Try
                Dim oManager As New VTDocuments.roDocumentManager(oState)
                result.Value = oManager.GetDocumentBytesById(idDocument, True)
                result.Status = ErrorCodes.OK

                If idEmployee <> -1 Then
                    Dim setRead = oManager.SetDocumentRead(idDocument, idEmployee, True)
                End If
            Catch ex As Exception
                result.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::GetDocumentBytes")
            End Try
            Return result
        End Function

        Public Shared Function GetAvailableDocumentTemplateType(ByVal idEmployee As Integer, ByRef oDocState As VTDocuments.roDocumentState) As GenericList
            Dim lrret As New GenericList

            Try
                lrret.Status = ErrorCodes.OK

                Dim oManager As New VTDocuments.roDocumentManager(oDocState)
                Dim oTempLates As List(Of roDocumentTemplate) = oManager.GetAvailableDocumentTemplateByTypeForEmployee(DocumentType.Employee, idEmployee, 6, False)

                Dim items As New Generic.List(Of SelectField)

                For Each oDoc As roDocumentTemplate In oTempLates
                    Dim oTmpItem As New SelectField With {
                        .FieldName = oDoc.Name,
                        .FieldValue = oDoc.Id
                    }

                    If oDoc.LeaveDocumentType = LeaveDocumentType.ReturnReport Then
                        oTmpItem.RelatedInfo = 1
                    Else
                        oTmpItem.RelatedInfo = 0
                    End If

                    items.Add(oTmpItem)
                Next

                lrret.SelectFields = items.ToArray
            Catch ex As Exception
                lrret.SelectFields = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::GetAvailableDocumentTemplateType")
            End Try

            Return lrret
        End Function

        Public Shared Function GetLeaveTemplates(ByVal idEmployee As Integer, ByVal idCause As Integer, ByVal isStarting As Boolean, ByRef oDocState As VTDocuments.roDocumentState) As GenericList
            Dim lrret As New GenericList

            Try
                lrret.Status = ErrorCodes.OK

                Dim oManager As New VTDocuments.roDocumentManager(oDocState)
                Dim oTempLates As List(Of roDocumentTemplate) = oManager.GetCauseAvailableDocumentTemplateByEmployee(idCause, idEmployee, isStarting)

                Dim items As New Generic.List(Of SelectField)

                For Each oDoc As roDocumentTemplate In oTempLates
                    Dim oTmpItem As New SelectField With {
                        .FieldName = oDoc.Name,
                        .FieldValue = oDoc.Id
                    }

                    If oDoc.LeaveDocumentType = LeaveDocumentType.ReturnReport Then
                        oTmpItem.RelatedInfo = 1
                    Else
                        oTmpItem.RelatedInfo = 0
                    End If

                    items.Add(oTmpItem)
                Next

                lrret.SelectFields = items.ToArray
            Catch ex As Exception
                lrret.SelectFields = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::GetMyDocuments")
            End Try

            Return lrret
        End Function

        Public Shared Function GetCauseTemplates(ByVal idEmployee As Integer, ByVal idCause As Integer, ByRef oDocState As VTDocuments.roDocumentState) As GenericList
            Dim lrret As New GenericList

            Try
                lrret.Status = ErrorCodes.OK

                Dim oManager As New VTDocuments.roDocumentManager(oDocState)
                Dim oTempLates As List(Of roDocumentTemplate) = oManager.GetCauseDocumentTemplate(idCause, idEmployee)

                Dim items As New Generic.List(Of SelectField)

                For Each oDoc As roDocumentTemplate In oTempLates
                    Dim oTmpItem As New SelectField With {
                        .FieldName = oDoc.Name,
                        .FieldValue = oDoc.Id
                    }

                    items.Add(oTmpItem)
                Next

                lrret.SelectFields = items.ToArray
            Catch ex As Exception
                lrret.SelectFields = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::GetCauseTemplates")
            End Try

            Return lrret
        End Function

        Public Shared Function UploadDocument(ByVal oPassport As roPassportTicket, ByVal fUploadedFile As Web.HttpPostedFile, ByVal remarks As String,
                                              ByVal idDocumentTemplate As Integer, ByVal idRelatedObject As Integer,
                                              ByVal docRelatedInfo As String, ByVal forecastType As ForecastType,
                                              ByRef oDocState As VTDocuments.roDocumentState, Optional idRequest As Integer = 0) As StdResponse
            Dim oResponse As New StdResponse

            If idDocumentTemplate = -1 Then
                oResponse.Result = False
                oResponse.Status = ErrorCodes.NO_DOCUMENT_TEMPLATE_SELECTED
                Return oResponse
            ElseIf fUploadedFile Is Nothing Then
                oResponse.Result = False
                oResponse.Status = ErrorCodes.NO_DOCUMENT_ATTACHED
                Return oResponse
            End If

            Try
                oResponse.Status = ErrorCodes.OK

                Dim dFinishDate As Date = New Date(2079, 1, 1)
                Dim oManager As New VTDocuments.roDocumentManager(oDocState)

                Dim file As IO.Stream = fUploadedFile.InputStream()
                Dim fileData As Byte() = New Byte(file.Length - 1) {}
                file.Read(fileData, 0, file.Length)

                Dim oDeliverdDocument As New roDocument
                With oDeliverdDocument
                    .Id = -1
                    If fUploadedFile.FileName.IndexOf(".") > 0 Then
                        .Title = fUploadedFile.FileName.Substring(0, fUploadedFile.FileName.LastIndexOf("."))
                    Else
                        .Title = fUploadedFile.FileName
                    End If

                    .IdEmployee = oPassport.IDEmployee
                    .IdCompany = -1
                    .DocumentTemplate = oManager.LoadDocumentTemplate(idDocumentTemplate)

                    If fUploadedFile.FileName.IndexOf(".") > 0 Then
                        .DocumentType = fUploadedFile.FileName.Substring(fUploadedFile.FileName.LastIndexOf("."))
                    Else
                        .DocumentType = ".txt"
                    End If

                    If idRelatedObject > 0 Then
                        Select Case .DocumentTemplate.Scope
                            Case DocumentScope.LeaveOrPermission, DocumentScope.CauseNote
                                Select Case forecastType
                                    Case ForecastType.AbsenceDays, ForecastType.Leave
                                        .IdDaysAbsence = idRelatedObject
                                    Case ForecastType.AbsenceHours
                                        .IdHoursAbsence = idRelatedObject
                                    Case ForecastType.OverWork
                                        .IdOvertimeForecast = idRelatedObject
                                End Select

                                'Si es un documento de alta, recojo la fecha de alta para cerrar la ausencia
                                If .DocumentTemplate.LeaveDocumentType = LeaveDocumentType.ReturnReport Then
                                    dFinishDate = DateTime.ParseExact(docRelatedInfo, "yyyyMMdd", Nothing)
                                End If
                            Case DocumentScope.EmployeeContract
                                .IdContract = idRelatedObject
                        End Select
                    End If

                    .Document = fileData
                    .DeliveredDate = Date.Now
                    .DeliveryChannel = "VisualTimePortal"
                    .DeliveredBy = oPassport.Name
                    .Status = DocumentStatus.Pending
                    .LastStatusChange = Date.Now
                    .BeginDate = New Date(1900, 1, 1)
                    .EndDate = dFinishDate
                    .IdRequest = IIf(idRequest = 0, Nothing, idRequest)
                    .Remarks = remarks
                End With

                If (oManager.SaveDocument(oDeliverdDocument)) Then
                    oResponse.Result = True
                Else
                    oResponse.Result = False
                    oResponse.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                oResponse.Result = False
                oResponse.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::UploadDocument")
            End Try

            Return oResponse
        End Function

        Public Shared Function UploadDocumenttoSign(ByVal idDocument As Integer, idEmployee As Integer, ByVal oState As VTDocuments.roDocumentState) As roGenericResponse(Of DocumentVID_PostDocResult)
            Dim result As roGenericResponse(Of DocumentVID_PostDocResult) = New roGenericResponse(Of DocumentVID_PostDocResult)
            Try
                Dim oManager As New VTDocuments.roDocumentManager(oState)
                result.Value = oManager.UploadDocumenttoSign(idDocument)

                If result.Value IsNot Nothing Then
                    result.Status = ErrorCodes.OK
                Else
                    Select Case oState.Result
                        Case DocumentResultEnum.EmptyFile
                            result.Status = ErrorCodes.EMPTY_FILE
                        Case DocumentResultEnum.InvalidMobileNumber
                            result.Status = ErrorCodes.INVALID_MOBILE_NUMBER
                        Case DocumentResultEnum.NumberOfSignedDocumentsExceeded
                            result.Status = ErrorCodes.NUMBER_SIGNED_DOC_EXCEEDED
                        Case DocumentResultEnum.ErrorUploadingDocumentToSign
                            result.Status = ErrorCodes.ERROR_UPLOADING_DOCUMENT_TO_SIGN
                        Case Else
                            result.Status = ErrorCodes.NO_DOCUMENT_UPLOADED_TO_SIGN
                    End Select
                End If

                If oManager.State.Result = DocumentResultEnum.NoError AndAlso idEmployee <> -1 Then
                    oManager.SetDocumentRead(idDocument, idEmployee, True)
                End If
            Catch ex As Exception
                result.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::UploadDocumenttoSign")
            End Try

            Return result

        End Function

        Public Shared Function DocumentSignatureStatus(ByVal guidDoc As String, ByVal oState As VTDocuments.roDocumentState) As roGenericResponse(Of DocumentInfoDTO)
            Dim result As roGenericResponse(Of DocumentInfoDTO) = New roGenericResponse(Of DocumentInfoDTO)
            Try
                Dim oManager As New VTDocuments.roDocumentManager(oState)
                result.Value = oManager.DocumentSignatureStatus(guidDoc)

                If result.Value IsNot Nothing Then
                    result.Status = ErrorCodes.OK
                Else
                    result.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                result.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::DocumentSignatureStatus")
            End Try

            Return result

        End Function

        Public Shared Function DownloadSignedDocument(ByVal guidDoc As String, ByVal documentid As Integer, ByVal oState As VTDocuments.roDocumentState) As roGenericResponse(Of Boolean)
            Dim result As roGenericResponse(Of Boolean) = New roGenericResponse(Of Boolean)
            Try
                Dim oManager As New VTDocuments.roDocumentManager(oState)
                result.Value = oManager.DownloadSignedDocument(guidDoc, documentid)

                If result.Value Then
                    result.Status = ErrorCodes.OK
                Else
                    result.Status = ErrorCodes.GENERAL_ERROR
                End If
            Catch ex As Exception
                result.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::DocumentsHelper::DownloadSignedDocument")
            End Try

            Return result

        End Function

    End Class

End Namespace