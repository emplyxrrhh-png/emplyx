Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roApiDocuments
        Inherits roDataLinkApi

        Protected ReadOnly Property ImportEngine As roEmployeeImport
            Get
                Return CType(Me.oDataImport, roEmployeeImport)
            End Get
        End Property

        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roEmployeeImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub



        Public Function GetDocuments(ByRef documents As Generic.List(Of RoboticsExternAccess.roDocument), ByVal documentsCriteria As RoboticsExternAccess.IDatalinkDocumentCriteria, ByRef strErrorMsg As String, ByRef iReturnCode As RoboticsExternAccess.Core.DTOs.ReturnCode) As Boolean
            Dim bolRet As Boolean = False

            Try
                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError

                documents = New Generic.List(Of RoboticsExternAccess.roDocument)

                Dim ColumnsVal As String() = {}
                Dim ColumnsPos As Integer() = {}
                Dim bForAllEmployees As Boolean = False
                Dim bForOneEmployee As Boolean = False

                bolRet = documentsCriteria.GetEmployeeColumnsDefinitionCriteria(ColumnsVal, ColumnsPos)

                bForAllEmployees = (roTypes.Any2String(ColumnsVal(DocumentsCriteriaAsciiColumns.NIF)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(DocumentsCriteriaAsciiColumns.NIF_Letter)).Trim = String.Empty AndAlso roTypes.Any2String(ColumnsVal(DocumentsCriteriaAsciiColumns.ImportPrimaryKey)).Trim = String.Empty)

                Dim strUniqueidentifierField As String = String.Empty

                If bolRet Then
                    Dim idEmployee As Integer = 0
                    Dim lstEmployees As String = String.Empty
                    If Not bForAllEmployees AndAlso ColumnsVal(DocumentsCriteriaAsciiColumns.Type) = "employee" Then
                        ' En el caso que el identificador de usuario tenga el caracter ";" debemos obtener cada uno de los identificadores por separado
                        ' ya que nos viene una lista de identificadores de usuarios
                        If ColumnsVal(DocumentsCriteriaAsciiColumns.ImportPrimaryKey).Contains(";") Then
                            Dim tmplst As String() = ColumnsVal(DocumentsCriteriaAsciiColumns.ImportPrimaryKey).Split(";")
                            For Each employee As String In tmplst
                                If employee.Length > 0 Then
                                    ColumnsVal(DocumentsCriteriaAsciiColumns.ImportPrimaryKey) = employee
                                    idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, DocumentsCriteriaAsciiColumns.ImportPrimaryKey, DocumentsCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                                    If idEmployee > 0 Then
                                        lstEmployees += "," & idEmployee.ToString
                                    Else
                                        strErrorMsg += "," & ColumnsVal(DocumentsCriteriaAsciiColumns.ImportPrimaryKey)
                                    End If
                                End If
                            Next
                            If lstEmployees.Length > 0 Then lstEmployees = lstEmployees.Substring(1)

                        Else

                            idEmployee = Me.ImportEngine.isEmployeeNew(ColumnsVal, DocumentsCriteriaAsciiColumns.ImportPrimaryKey, DocumentsCriteriaAsciiColumns.NIF, New UserFields.roUserFieldState)
                            If idEmployee > 0 Then
                                lstEmployees = idEmployee.ToString
                                bForOneEmployee = True
                            End If
                        End If
                    End If

                    Dim manager As roDocumentManager = New roDocumentManager()
                    Dim template As String = ColumnsVal(DocumentsCriteriaAsciiColumns.Template)
                    Dim type As String = ColumnsVal(DocumentsCriteriaAsciiColumns.Type)
                    Dim title As String = ColumnsVal(DocumentsCriteriaAsciiColumns.Title)
                    Dim company As String = ColumnsVal(DocumentsCriteriaAsciiColumns.Company)
                    Dim timestamp As Date? = roTypes.Any2DateTime(ColumnsVal(DocumentsCriteriaAsciiColumns.Timestamp))
                    Dim importPrimaryKey As String = ColumnsVal(DocumentsCriteriaAsciiColumns.ImportPrimaryKey)
                    Dim extension As String = ColumnsVal(DocumentsCriteriaAsciiColumns.Extension)
                    Dim updateType As String = ColumnsVal(DocumentsCriteriaAsciiColumns.UpdateType)
                    Dim documentsList As List(Of DTOs.roDocument) = manager.GetDocuments(lstEmployees, True, bForAllEmployees, bForOneEmployee, type, title, company, timestamp, updateType, extension, template, importPrimaryKey)

                    If documentsList IsNot Nothing AndAlso documentsList.Count > 0 Then
                        For Each document As DTOs.roDocument In documentsList
                            Dim documentResponse As RoboticsExternAccess.roDocument = New RoboticsExternAccess.roDocument()
                            documentResponse.DocumentTitle = document.Title
                            documentResponse.DocumentType = document.DocumentTemplate.Name
                            documentResponse.DocumentExtension = document.DocumentType
                            If ColumnsVal(DocumentsCriteriaAsciiColumns.Type).ToLower() = "employee" Then
                                documentResponse.CompanyID = ""
                            Else
                                documentResponse.CompanyID = document.IdCompany
                            End If
                            If document.IdDaysAbsence <> 0 Then
                                Dim absenceInfo As roDocumentAbsenceInfo = New roDocumentAbsenceInfo()
                                absenceInfo.Id = document.IdDaysAbsence
                                absenceInfo.AbsenceType = AbsenceType_Enum.Days.ToString()
                                documentResponse.AbsenceInfo = absenceInfo
                            End If
                            If document.IdHoursAbsence <> 0 Then
                                Dim absenceInfo As roDocumentAbsenceInfo = New roDocumentAbsenceInfo()
                                absenceInfo.Id = document.IdHoursAbsence
                                absenceInfo.AbsenceType = AbsenceType_Enum.Hours.ToString()
                                documentResponse.AbsenceInfo = absenceInfo
                            End If
                            Try
                                documentResponse.DocumentData = Convert.ToBase64String(document.Document)
                            Catch ex As Exception
                            End Try
                            documentResponse.DocumentExtension = document.DocumentType
                            documentResponse.DeliveryDate = New roWCFDate(CDate(document.DeliveredDate))
                            documentResponse.DeliveryChannel = document.DeliveryChannel
                            documentResponse.DeliveredBy = document.DeliveredBy

                            If ColumnsVal(DocumentsCriteriaAsciiColumns.UpdateType).Equals("") OrElse ColumnsVal(DocumentsCriteriaAsciiColumns.UpdateType).ToLower().Equals("statuschanged") Then
                                documentResponse.Timestamp = New roWCFDate(CDate(document.LastStatusChange))
                            Else
                                If ColumnsVal(DocumentsCriteriaAsciiColumns.UpdateType).ToLower().Equals("signed") Then
                                    If document.SignDate IsNot Nothing Then
                                        documentResponse.Timestamp = New roWCFDate(CDate(document.SignDate))
                                    End If
                                Else
                                    If ColumnsVal(DocumentsCriteriaAsciiColumns.UpdateType).ToLower().Equals("delivered") Then
                                        documentResponse.Timestamp = New roWCFDate(CDate(document.DeliveredDate))
                                    End If
                                End If
                            End If
                            Dim documentStatus = New roDocumentStatusInfo()
                            documentStatus.Type = DirectCast(document.Status, DocumentStatusType_Enum).ToString()
                            If roTypes.Any2Integer(document.IdLastStatusSupervisor) <> 0 Then
                                Dim supervisorPassport As roPassport = roPassportManager.GetPassport(document.IdLastStatusSupervisor)
                                If supervisorPassport IsNot Nothing Then
                                    documentStatus.SupervisorName = supervisorPassport.Name
                                End If
                            End If
                            documentStatus.DocumentBeginDate = New roWCFDate(CDate(document.BeginDate))
                            documentStatus.DocumentEndDate = New roWCFDate(CDate(document.EndDate))
                            If document.SignDate IsNot Nothing Then
                                documentStatus.DocumentSignedDate = New roWCFDate(CDate(document.SignDate))
                            End If
                            documentStatus.LastStatusChangeDate = New roWCFDate(CDate(document.LastStatusChange))
                            documentResponse.DocumentStatusInfo = documentStatus
                            If ColumnsVal(DocumentsCriteriaAsciiColumns.Type) = "employee" Then
                                If Not bForOneEmployee Then
                                    documentResponse.EmployeeID = roTypes.Any2String(document.IdEmployee)
                                Else
                                    documentResponse.EmployeeID = roTypes.Any2String(ColumnsVal(RoboticsExternAccess.DocumentsCriteriaAsciiColumns.ImportPrimaryKey))
                                End If
                            Else
                                documentResponse.EmployeeID = ""
                            End If
                            documentResponse.DocumentExternalID = document.DocumentExternalID
                            documents.Add(documentResponse)
                        Next
                    End If

                    iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                    bolRet = True
                Else
                    If ColumnsVal(DocumentsCriteriaAsciiColumns.Type) = "" Then
                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._DocumentTypeMustBeSetted
                    Else
                        iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._InvalidDocumentType
                    End If
                    bolRet = False
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::GetDocuments")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function CreateOrUpdateDocument(ByVal oIncomingDocument As RoboticsExternAccess.roDatalinkStandardDocument, ByVal UserName As String, ByRef strErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.State.Result = DataLinkResultEnum.Exception

                ' Creo el documento según el estandar de VisualTime
                Dim oDocument As New DTOs.roDocument
                Dim oDocState As New VTDocuments.roDocumentState()
                Dim oDocumenManager As New VTDocuments.roDocumentManager(oDocState)

                Dim oDocTemplate As New roDocumentTemplate

                ' El empleado destino debe existir
                Dim oUserFieldState As New UserFields.roUserFieldState()
                Dim intIDEmployee As Integer = Me.ImportEngine.isEmployeeNew(oIncomingDocument.UniqueEmployeeID, oIncomingDocument.NifEmpleado, oUserFieldState)

                If intIDEmployee > 0 Then
                    ' El documneto debe contener datos
                    If oIncomingDocument.DocumentData.Length > 0 Then
                        ' La plantilla del documento debe existir
                        oDocTemplate = oDocumenManager.LoadDocumentTemplateByName(oIncomingDocument.DocumentType)
                        If oDocTemplate IsNot Nothing Then
                            ' La plantilla del documento debe ser de tipo "Documentación general de empleado"
                            If oDocTemplate.Scope = DocumentScope.EmployeeContract Then
                                ' Los documentos los deben poder presentar electrónicamente los empleados
                                If oDocTemplate.EmployeeDeliverAllowed OrElse oDocTemplate.SupervisorDeliverAllowed Then
                                    Dim bValidFile As Boolean = True
                                    Try
                                        oDocument.Document = Convert.FromBase64String(oIncomingDocument.DocumentData)
                                    Catch ex As Exception
                                        bValidFile = False
                                    End Try
                                    If bValidFile Then
                                        ' Tamaño máximo
                                        Dim maxFileSize As Integer = roTypes.Any2Integer(New AdvancedParameter.roAdvancedParameter("VTLive.MaxAllowedFileSize", New AdvancedParameter.roAdvancedParameterState).Value)
                                        If maxFileSize = 0 Then maxFileSize = 256
                                        If oDocument.Document.Length <= (maxFileSize * 1024) Then
                                            ' Verifico que la cadena es válida
                                            oDocument.Id = -1
                                            oDocument.IdEmployee = intIDEmployee
                                            oDocument.DocumentTemplate = oDocTemplate

                                            oDocument.DeliveredDate = Now.Date
                                            oDocument.Title = oIncomingDocument.DocumentTitle
                                            If bolIsNew AndAlso oDocument.DocumentTemplate.Scope = DocumentScope.EmployeeContract Then
                                                Dim oContractState As New Contract.roContractState(Me.State.IDPassport)
                                                Dim oContract As Contract.roContract = Contract.roContract.GetActiveContract(oDocument.IdEmployee, oContractState, False)
                                                oDocument.IdContract = oContract.IDContract
                                            End If
                                            oDocument.IdCompany = -1
                                            oDocument.IdPunch = 0
                                            oDocument.IdDaysAbsence = 0
                                            oDocument.IdHoursAbsence = 0
                                            oDocument.IdOvertimeForecast = 0
                                            oDocument.DocumentType = "." & oIncomingDocument.DocumentExtension
                                            oDocument.DeliveryChannel = "Datalink Web Service"
                                            oDocument.DeliveredBy = UserName
                                            oDocument.Status = If(oDocTemplate.ApprovalLevelRequired = 0, DocumentStatus.Validated, DocumentStatus.Pending)
                                            oDocument.StatusLevel = 1
                                            oDocument.IdLastStatusSupervisor = 1
                                            oDocument.LastStatusChange = Now.Date
                                            oDocument.BeginDate = New Date(1900, 1, 1)
                                            oDocument.EndDate = New Date(2079, 1, 1)
                                            oDocument.Remarks = oIncomingDocument.DocumentRemarks
                                            oDocument.DocumentExternalID = oIncomingDocument.DocumentExternalId
                                            Dim oExistingDocument As DTOs.roDocument
                                            ' Antes de guardar, verifico que no existe otro documento con el mismo contenido y título
                                            ' Debo hacerlo solo de los que no tienen DocumentExternalId informado (Estos se crearan de nuevo en el SaveDocument si es necesario)
                                            oExistingDocument = oDocumenManager.LoadEmployeeDocumentByContent(oIncomingDocument.DocumentData, intIDEmployee)
                                            If oExistingDocument Is Nothing OrElse oExistingDocument.Title <> oIncomingDocument.DocumentTitle OrElse oExistingDocument.DocumentTemplate.Scope <> DocumentScope.EmployeeContract Then
                                                bolRet = oDocumenManager.SaveDocument(oDocument, True)
                                                If bolRet Then
                                                    Me.State.Result = DataLinkResultEnum.NoError
                                                Else
                                                    Me.State.Result = DataLinkResultEnum.ErrorSavingDocument
                                                    strErrorMsg = "Unable to save document"
                                                    If oDocState.Result = DocumentResultEnum.ErrorDeletingDocument AndAlso Not String.IsNullOrEmpty(oIncomingDocument.DocumentExternalId) Then
                                                        Me.State.Result = DataLinkResultEnum.ErrorDeletingDocument
                                                        strErrorMsg = "Unable to update document. It was not possible to delete existing document with externalid"
                                                    ElseIf oDocState.Result = DocumentResultEnum.ExternalIdDuplicated AndAlso Not String.IsNullOrEmpty(oIncomingDocument.DocumentExternalId) Then
                                                        Me.State.Result = DataLinkResultEnum.ExternalIdDuplicated
                                                        strErrorMsg = "ExternalId already exists for another document"
                                                    End If
                                                End If
                                            Else
                                                Me.State.Result = DataLinkResultEnum.EmployeeDocumentAlreadyExists
                                                strErrorMsg = "Document already exists for employee"
                                            End If
                                        Else
                                            Me.State.Result = DataLinkResultEnum.DocumentTooBig
                                            strErrorMsg = "Document too big. Max is " & maxFileSize & " kb"
                                        End If
                                    Else
                                        Me.State.Result = DataLinkResultEnum.InvalidDocumentData
                                        strErrorMsg = "Document format not supported"
                                    End If
                                Else
                                    Me.State.Result = DataLinkResultEnum.DocumentNotDeliverable
                                    strErrorMsg = "Document type '" & oIncomingDocument.DocumentType & "' is could not be delivered by Visualtime Datalink"
                                End If
                            Else
                                Me.State.Result = DataLinkResultEnum.InvalidDocumentType
                                strErrorMsg = "Document type '" & oIncomingDocument.DocumentType & "' is a non supported type for Visualtime Datalink"
                            End If
                        Else
                            Me.State.Result = DataLinkResultEnum.UnexistingDocumentTemplate
                            strErrorMsg = "Document type '" & oIncomingDocument.DocumentType & "' must exist in Visualtime"
                        End If
                    Else
                        Me.State.Result = DataLinkResultEnum.InvalidDocumentData
                        strErrorMsg = "Empty document"
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    strErrorMsg = "No such employee"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateDocument")
                Me.State.Result = DataLinkResultEnum.Exception
                strErrorMsg = "Exception: " + ex.Message
            Finally

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function DeleteDocument(ByVal externalDocumentId As String, ByRef strErrorMsg As String, ByRef documentToBeDeleted As DTOs.roDocument) As Boolean
            Dim bolRet As Boolean = False

            Try
                Me.State.Result = DataLinkResultEnum.Exception

                Dim oDocState As New VTDocuments.roDocumentState()
                Dim oDocumenManager As New VTDocuments.roDocumentManager(oDocState)

                ' Recuperamos el documento por ExternalId
                'documentToBeDeleted = oDocumenManager.LoadDocumentByExternalId(externalDocumentId)
                Dim dummyDocument As New DTOs.roDocument With {.Id = -1, .DocumentExternalID = externalDocumentId}
                Dim existingDocuments As List(Of DTOs.roDocument) = oDocumenManager.GetDocumentsByExternalId(dummyDocument)

                If existingDocuments.Any Then
                    bolRet = oDocumenManager.DeleteDocument(existingDocuments.First.Id, True)
                    If bolRet Then
                        If existingDocuments.Count > 1 Then
                            bolRet = False
                            Me.State.Result = DataLinkResultEnum.DocumentDeletedButExternalIdStillExists
                            strErrorMsg = "Unable to delete all documents with same externalid. Please repeat delete action"
                        Else
                            Me.State.Result = DataLinkResultEnum.NoError
                        End If
                    Else
                        Me.State.Result = DataLinkResultEnum.ErrorDeletingDocument
                        strErrorMsg = "Unable to delete document"
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.UnexistentDocument
                    strErrorMsg = "No such document"
                End If

            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::DeleteDocument")
                Me.State.Result = DataLinkResultEnum.Exception
                strErrorMsg = "Exception: " + ex.Message
            End Try

            Return bolRet
        End Function


    End Class

End Namespace