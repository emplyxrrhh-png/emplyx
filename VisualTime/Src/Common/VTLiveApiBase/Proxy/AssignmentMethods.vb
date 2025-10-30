Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.Common

Public Class AssignmentMethods

    ''' <summary>
    ''' Obtiene la lista de puestos definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de los puestos definidos.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetAssignments(ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roAssignment))
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAssignmentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roAssignment))
        oResult.Value = roAssignment.GetAssignments(_Order, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de puestos definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Un objeto dataset con una tabla con los puestos definidos.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetAssignmentsDataTable(ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAssignmentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roAssignment.GetAssignmentsDataTable(_Order, bState, _Audit)
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

    Public Shared Function GetAssignment(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roAssignment)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAssignmentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAssignment)
        oResult.Value = New roAssignment(_ID, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos del puesto. Si és nuevo, se actualiza el ID del puesto pasado.<br/>
    ''' </summary>
    ''' <param name="oAssignment">Puesto a guardar (roAssignment)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>
    Public Shared Function SaveAssignment(ByVal oAssignment As roAssignment, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roAssignment)
        'cambio mi state genérico a un estado especifico
        oAssignment.State = New roAssignmentState(-1)
        roWsStateManager.CopyTo(oState, oAssignment.State)
        oAssignment.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAssignment)
        If oAssignment.Save(bAudit) Then
            oResult.Value = oAssignment
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAssignment.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina lel puesto con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID del puestoa eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function DeleteAssignment(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAssignmentState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oAssignment As roAssignment = New roAssignment(ID, bState, bAudit)
        oAssignment.State = bState
        oResult.Value = oAssignment.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAssignment.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class