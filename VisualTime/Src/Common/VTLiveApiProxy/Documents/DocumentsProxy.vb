Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.DTOs

Public Class DocumentsProxy
    Implements IDocumentsSvc

    Public Function KeepAlive() As Boolean Implements IDocumentsSvc.KeepAlive
        Return True
    End Function

#Region "Documents V2"
    Public Function GetTemplateDocuments(filterScope As Boolean, docScope As DocumentScope, ByVal oState As roWsState) As roDocumentTemplateListResponse Implements IDocumentsSvc.GetTemplateDocuments
        Return DocumentsMethods.GetTemplateDocuments(filterScope, docScope, oState)
    End Function

    Public Function GetTemplateDocumentsAvailableByAbsence(ByVal idAbsence As Integer, ByVal idEmployee As Integer, ByVal isStarting As Boolean, ByVal eforecast As ForecastType, ByVal oState As roWsState) As roDocumentTemplateListResponse Implements IDocumentsSvc.GetTemplateDocumentsAvailableByAbsence
        Return DocumentsMethods.GetTemplateDocumentsAvailableByAbsence(idAbsence, idEmployee, isStarting, eforecast, oState)
    End Function

    Function GetDocumentTemplateById(ByVal idDocumentTemplate As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateResponse Implements IDocumentsSvc.GetDocumentTemplateById
        Return DocumentsMethods.GetDocumentTemplateById(idDocumentTemplate, bAudit, oState)
    End Function

    Public Function SaveDocumentTemplate(ByVal oDocumentTemplate As roDocumentTemplate, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateResponse Implements IDocumentsSvc.SaveDocumentTemplate
        Return DocumentsMethods.SaveDocumentTemplate(oDocumentTemplate, bAudit, oState)
    End Function

    Function DeleteDocumentTemplate(ByVal oDocumentTemplate As roDocumentTemplate, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse Implements IDocumentsSvc.DeleteDocumentTemplate
        Return DocumentsMethods.DeleteDocumentTemplate(oDocumentTemplate, bAudit, oState)
    End Function


    Public Function GetAvailableDocumentTemplateByType(ByVal docType As DocumentType, ByVal idRelatedObject As Integer, ByVal minPermission As Permission, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse Implements IDocumentsSvc.GetAvailableDocumentTemplateByType
        Return DocumentsMethods.GetAvailableDocumentTemplateByType(docType, idRelatedObject, minPermission, bAudit, oState)
    End Function

    Public Function GetTemplateDocumentsByType(ByVal docType As DocumentType, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse Implements IDocumentsSvc.GetTemplateDocumentsByType
        Return DocumentsMethods.GetTemplateDocumentsByType(docType, bAudit, oState)
    End Function

    Public Function GetAccessAuthorizationsTemplates(ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse Implements IDocumentsSvc.GetAccessAuthorizationsTemplates
        Return DocumentsMethods.GetAccessAuthorizationsTemplates(bAudit, oState)
    End Function


    Function CanEditDocument(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentPermission Implements IDocumentsSvc.CanEditDocument
        Return DocumentsMethods.CanEditDocument(idDocument, bAudit, oState)
    End Function

    Function GetDocumentsbyRequest(ByVal idRequest As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As Generic.List(Of Integer) Implements IDocumentsSvc.GetDocumentsbyRequest
        Return DocumentsMethods.GetDocumentsbyRequest(idRequest, bAudit, oState)
    End Function


    Function CanAccessRequestDocumentation(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roRequestDocument Implements IDocumentsSvc.CanAccessRequestDocumentation
        Return DocumentsMethods.CanAccessRequestDocumentation(idDocument, bAudit, oState)
    End Function


    Function SaveDocument(ByVal oDocument As roDocument, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentResponse Implements IDocumentsSvc.SaveDocument
        Return DocumentsMethods.SaveDocument(oDocument, bAudit, oState)
    End Function

    Public Function GetDocumentsByType(ByVal oDocType As DocumentType, ByVal idRelatedObject As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentListResponse Implements IDocumentsSvc.GetDocumentsByType
        Return DocumentsMethods.GetDocumentsByType(oDocType, idRelatedObject, bAudit, oState)
    End Function


    Public Function GetDocumentEmployeesByGroup(ByVal idGroup As Integer, ByVal filter As String, ByVal orderBy As String, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentListResponse Implements IDocumentsSvc.GetDocumentEmployeesByGroup
        Return DocumentsMethods.GetDocumentEmployeesByGroup(idGroup, filter, orderBy, bAudit, oState)
    End Function


    Function UpdateDocumentStatus(ByVal idDocument As Integer, ByVal oNewDocStatus As DocumentStatus, ByVal strSupervisorRemark As String, ByVal updateStatusDate As DateTime, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse Implements IDocumentsSvc.UpdateDocumentStatus
        Return DocumentsMethods.UpdateDocumentStatus(idDocument, oNewDocStatus, strSupervisorRemark, updateStatusDate, bAudit, oState)
    End Function


    Public Function GetDocumentById(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentResponse Implements IDocumentsSvc.GetDocumentById
        Return DocumentsMethods.GetDocumentById(idDocument, bAudit, oState)
    End Function

    Public Function GetDocumentFileById(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentFileRespone Implements IDocumentsSvc.GetDocumentFileById
        Return DocumentsMethods.GetDocumentFileById(idDocument, bAudit, oState)
    End Function

    Function DeleteDocument(ByVal idDocument As Integer, ByVal bAudit As Boolean, ByVal oState As roWsState) As roDocumentStandarResponse Implements IDocumentsSvc.DeleteDocument
        Return DocumentsMethods.DeleteDocument(idDocument, bAudit, oState)
    End Function

    Function GetDocumentationFaultAlerts(ByVal idRelatedObject As Integer, ByVal type As DocumentType, ByVal oFilterForecast As ForecastType, ByVal idForecastObject As Integer, ByVal oState As roWsState) As DocumentAlertsRespone Implements IDocumentsSvc.GetDocumentationFaultAlerts
        Return DocumentsMethods.GetDocumentationFaultAlerts(idRelatedObject, type, oFilterForecast, idForecastObject, oState)
    End Function

    Function GetCauseAvailableDocumentTemplateByEmployee(ByVal idEmployee As Integer, ByVal idCause As Integer, ByVal bIsStarting As Boolean, ByVal oState As roWsState) As roDocumentTemplateListResponse Implements IDocumentsSvc.GetCauseAvailableDocumentTemplateByEmployee
        Return DocumentsMethods.GetCauseAvailableDocumentTemplateByEmployee(idEmployee, idCause, bIsStarting, oState)
    End Function



#End Region

End Class
