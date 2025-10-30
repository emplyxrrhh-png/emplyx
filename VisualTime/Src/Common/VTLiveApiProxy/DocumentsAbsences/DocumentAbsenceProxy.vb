Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.DocumentAbsence

Public Class DocumentAbsenceProxy
    Implements IDocumentAbsenceSvc

    Public Function KeepAlive() As Boolean Implements IDocumentAbsenceSvc.KeepAlive
        Return True
    End Function

    Public Function GetDocumentsDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IDocumentAbsenceSvc.GetDocumentsDataSet
        Return DocumentAbsenceMethods.GetDocumentsDataSet(oState)
    End Function

    Public Function GetDocumentByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDocumentAbsence) Implements IDocumentAbsenceSvc.GetDocumentByID
        Return DocumentAbsenceMethods.GetDocumentByID(intID, oState, bAudit)
    End Function

    Public Function SaveDocument(ByVal oDocument As roDocumentAbsence, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDocumentAbsence) Implements IDocumentAbsenceSvc.SaveDocument
        Return DocumentAbsenceMethods.SaveDocument(oDocument, oState, bAudit)
    End Function

    Public Function DeleteDocument(ByVal oDocument As roDocumentAbsence, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IDocumentAbsenceSvc.DeleteDocument
        Return DocumentAbsenceMethods.DeleteDocument(oDocument, oState, bAudit)
    End Function


    Public Function DeleteDocumentByID(ByVal IDDocumentAbsence As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IDocumentAbsenceSvc.DeleteDocumentByID
        Return DocumentAbsenceMethods.DeleteDocumentByID(IDDocumentAbsence, oState, bAudit)
    End Function

    ''' <summary>
    ''' Retorna true si existe el id especificado. Si se pasa un -1 retorna true si hay registros en la tabla
    ''' </summary>
    Public Function ExitsDocument(ByVal IDDocumentAbsence As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDocumentAbsenceSvc.ExitsDocument
        Return DocumentAbsenceMethods.ExitsDocument(IDDocumentAbsence, oState)
    End Function

    Public Function GetDocumentAbsenceAdviceByID(ByVal IDAdvice As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roDocumentAbsenceAdvice) Implements IDocumentAbsenceSvc.GetDocumentAbsenceAdviceByID
        Return DocumentAbsenceMethods.GetDocumentAbsenceAdviceByID(IDAdvice, oState)
    End Function


End Class
