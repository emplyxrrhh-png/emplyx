Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.DocumentAbsence

Public Class DocumentAbsenceMethods

    Public Shared Function GetDocumentsDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roDocumentAbsence.GetDocumentAbsencesDataTable("", bState)
        If tb IsNot Nothing Then
            If tb.DataSet IsNot Nothing Then
                ds = tb.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tb)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDocumentByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDocumentAbsence)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDocumentAbsence)
        oResult.Value = New roDocumentAbsence(intID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveDocument(ByVal oDocument As roDocumentAbsence, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDocumentAbsence)

        'cambio mi state genérico a un estado especifico
        oDocument.State = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, oDocument.State)
        oDocument.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDocumentAbsence)
        If oDocument.Save(bAudit) Then
            oResult.Value = oDocument
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocument.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteDocument(ByVal oDocument As roDocumentAbsence, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oDocument.State = bState
        oResult.Value = oDocument.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocument.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteDocumentByID(ByVal IDDocumentAbsence As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oDocument As New roDocumentAbsence(IDDocumentAbsence, bState, False)
        oResult.Value = oDocument.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDocument.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Retorna true si existe el id especificado. Si se pasa un -1 retorna true si hay registros en la tabla
    ''' </summary>
    Public Shared Function ExitsDocument(ByVal IDDocumentAbsence As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roDocumentAbsence.ExitsDocumentAbsence(IDDocumentAbsence, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetDocumentAbsenceAdviceByID(ByVal IDAdvice As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roDocumentAbsenceAdvice)

        Dim bState = New roDocumentAbsenceState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDocumentAbsenceAdvice)
        oResult.Value = New roDocumentAbsenceAdvice(IDAdvice, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

End Class