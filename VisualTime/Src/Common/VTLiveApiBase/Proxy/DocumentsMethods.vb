Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTDocuments

Public Class DocumentsMethods

#Region "Documents V2"

    Public Shared Function GetTemplateDocuments(filterScope As Boolean, docScope As DocumentScope, ByVal oState As roWsState) As roDocumentTemplateListResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate As Generic.List(Of roDocumentTemplate) = oDocumentManager.GetTemplateDocuments(filterScope, docScope)

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.DocumentTemplates = If(oDocumentTemplate IsNot Nothing, oDocumentTemplate.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetTemplateDocumentsAvailableByAbsence(ByVal idAbsence As Integer, ByVal idEmployee As Integer, ByVal isStarting As Boolean, ByVal eforecast As ForecastType, ByVal oState As roWsState) As roDocumentTemplateListResponse

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate As Generic.List(Of roDocumentTemplate) = oDocumentManager.GetTemplateDocumentsAvailableByAbsence(idAbsence, idEmployee, eforecast, isStarting)
        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.DocumentTemplates = If(oDocumentTemplate IsNot Nothing, oDocumentTemplate.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentTemplateById(ByVal idDocumentTemplate As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate = oDocumentManager.LoadDocumentTemplate(idDocumentTemplate)

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateResponse

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.DocumentTemplate = oDocumentTemplate
        genericResponse.oState = newGState
        Return genericResponse

    End Function

    Public Shared Function SaveDocumentTemplate(ByVal oDocumentTemplate As roDocumentTemplate, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateResponse

        Dim bState = New roDocumentState(-1)

        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        If Not oDocumentManager.SaveDocumentTemplate(oDocumentTemplate, bAudit) Then oDocumentTemplate.Id = -1

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.DocumentTemplate = oDocumentTemplate
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function DeleteDocumentTemplate(ByVal oDocumentTemplate As roDocumentTemplate, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oRet = oDocumentManager.DeleteDocumentTemplate(oDocumentTemplate, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetAvailableDocumentTemplateByType(ByVal docType As DocumentType, ByVal idRelatedObject As Integer, ByVal minPermission As Permission, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetAvailableDocumentTemplateByType(docType, idRelatedObject, minPermission, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)

        genericResponse.DocumentTemplates = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetTemplateDocumentsByType(ByVal docType As DocumentType, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetTemplateDocumentsByType(docType, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)

        genericResponse.DocumentTemplates = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetAccessAuthorizationsTemplates(ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetAccessAuthorizationsTemplates(bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)

        genericResponse.DocumentTemplates = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function CanEditDocument(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentPermission

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)

        Dim bRet As Boolean = oDocumentManager.CanEditDocument(idDocument)

        'crear el response genérico
        Dim genericResponse As New roDocumentPermission
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.CanEditDocument = bRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function RegisterA3Payroll(ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)

        Dim bRet As Boolean = oDocumentManager.RegisterA3Payrroll()

        'crear el response genérico
        Dim genericResponse As New roDocumentStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Result = bRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function UnRegisterA3Payroll(ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)

        Dim bRet As Boolean = oDocumentManager.UnRegisterA3Payrroll()

        'crear el response genérico
        Dim genericResponse As New roDocumentStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Result = bRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentsbyRequest(ByVal idRequest As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As Generic.List(Of Integer)

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)

        Dim bRet As Generic.List(Of Integer) = oDocumentManager.GetDocumentsbyRequest(idRequest)

        Return bRet
    End Function

    Public Shared Function CanAccessRequestDocumentation(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roRequestDocument

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)

        Dim bRet As Integer = oDocumentManager.CanAccessRequestDocumentation(idDocument)

        'crear el response genérico
        Dim genericResponse As New roRequestDocument
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.DocumentId = bRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function SaveDocument(ByVal oDocument As roDocument, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)

        If Not oDocumentManager.SaveDocument(oDocument, bAudit) Then oDocument.Id = -1

        'crear el response genérico
        Dim genericResponse As New roDocumentResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Document = oDocument
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetSystemDocumentList(ByVal scope As DocumentScope, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetSystemDocumentList(scope, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentsByTemplateId(ByVal templateId As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState, Optional searchTerm As String = "", Optional showAll As Boolean = False) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetDocumentsByTemplateId(templateId, bAudit, searchTerm, showAll)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentsByFilterName(ByVal filterExpression As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetDocumentsByFilterName(filterExpression, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentsByType(ByVal oDocType As DocumentType, ByVal idRelatedObject As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState, Optional bCalcWeight As Boolean = True) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetDocumentsByType(idRelatedObject, oDocType, bAudit, bCalcWeight)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentEmployeesByGroup(ByVal idGroup As Integer, ByVal filter As String, ByVal orderBy As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetDocumentEmployeesByGroup(idGroup, filter, orderBy, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetAllDocumentEmployeesBySelection(ByVal employees As String, ByVal filter As String, ByVal filterUser As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentList = oDocumentManager.GetAllDocumentEmployeesBySelection(employees, filter, filterUser, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = oDocumentList.ToArray
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function UpdateDocumentStatus(ByVal idDocument As Integer, ByVal oNewDocStatus As DocumentStatus, ByVal strSupervisorRemark As String, ByVal updateStatusDate As DateTime, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oRet = oDocumentManager.ChangeDocumentState(idDocument, oNewDocStatus, strSupervisorRemark, updateStatusDate, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentById(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocument = oDocumentManager.LoadDocument(idDocument, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Document = oDocument
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetBioCertificateBytes(ByVal certificateName As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentFileRespone

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate = oDocumentManager.GetBioCertificateBytes(certificateName, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentFileRespone
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.DocumentFile = oDocumentTemplate
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentFileById(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentFileRespone

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate = oDocumentManager.GetDocumentBytesById(idDocument, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentFileRespone
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.DocumentFile = oDocumentTemplate
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetSignReportDocumentBytesById(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentFileRespone

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate = oDocumentManager.GetSignReportDocumentBytesById(idDocument, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentFileRespone
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.DocumentFile = oDocumentTemplate
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function SignStatusDocumentInProgress(ByVal idDocument As Integer, ByVal GUIDDoc As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oRet = oDocumentManager.SignStatusDocumentInProgress(GUIDDoc, idDocument, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function DeleteDocument(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oRet = oDocumentManager.DeleteDocument(idDocument, bAudit)

        'crear el response genérico
        Dim genericResponse As New roDocumentStandarResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Result = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function CheckIfEmployeeDocumentExists(ByVal document As roDocument, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oRet As roDocument = oDocumentManager.CheckIfEmployeeDocumentExists(document)

        'crear el response genérico
        Dim genericResponse As New roDocumentResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Document = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetDocumentationFaultAlerts(ByVal idRelatedObject As Integer, ByVal type As DocumentType, ByVal oFilterForecast As ForecastType, ByVal idForecastObject As Integer, ByVal oState As roWsState, Optional checkStatusLevel As Boolean = False) As DocumentAlertsRespone

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oRet = oDocumentManager.GetDocumentationFaultAlerts(idRelatedObject, type, idForecastObject,,, oFilterForecast,, checkStatusLevel)

        'crear el response genérico
        Dim genericResponse As New DocumentAlertsRespone
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.DocumentAlerts = oRet
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function GetCauseAvailableDocumentTemplateByEmployee(ByVal idEmployee As Integer, ByVal idCause As Integer, ByVal bIsStarting As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim oDocumentTemplate As Generic.List(Of roDocumentTemplate) = oDocumentManager.GetCauseAvailableDocumentTemplateByEmployee(idCause, idEmployee, bIsStarting)

        'crear el response genérico
        Dim genericResponse As New roDocumentTemplateListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.DocumentTemplates = If(oDocumentTemplate IsNot Nothing, oDocumentTemplate.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

    Public Shared Function SplitA3PayrollDocument(ByVal document As roDocument, ByVal audit As Boolean, ByVal oState As roWsState) As roDocumentListResponse

        Dim bState = New roDocumentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oDocumentManager As New roDocumentManager(bState)
        Dim splittedDocuments As Generic.List(Of roDocument) = oDocumentManager.SplitA3PayrollDocument(document, audit)

        'crear el response genérico
        Dim genericResponse As New roDocumentListResponse
        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocumentManager.State, newGState)
        genericResponse.Documents = If(splittedDocuments IsNot Nothing, splittedDocuments.ToArray, {})
        genericResponse.oState = newGState
        Return genericResponse
    End Function

#End Region

End Class