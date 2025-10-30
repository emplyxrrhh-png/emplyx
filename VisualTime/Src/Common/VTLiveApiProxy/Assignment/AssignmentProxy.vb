Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.Common

Public Class AssignmentProxy
    Implements IAssignmentSvc

    Public Function KeepAlive() As Boolean Implements IAssignmentSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene la lista de puestos definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de los puestos definidos.</returns>
    ''' <remarks></remarks>
    Public Function GetAssignments(ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roAssignment)) Implements IAssignmentSvc.GetAssignments
        Return AssignmentMethods.GetAssignments(_Order, _Audit, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de puestos definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Un objeto dataset con una tabla con los puestos definidos.</returns>
    ''' <remarks></remarks>
    Public Function GetAssignmentsDataTable(ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAssignmentSvc.GetAssignmentsDataTable
        Return AssignmentMethods.GetAssignmentsDataTable(_Order, _Audit, oState)
    End Function

    Public Function GetAssignment(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roAssignment) Implements IAssignmentSvc.GetAssignment
        Return AssignmentMethods.GetAssignment(_ID, _Audit, oState)
    End Function


    ''' <summary>
    ''' Guarda los datos del puesto. Si és nuevo, se actualiza el ID del puesto pasado.<br/>
    ''' </summary>
    ''' <param name="oAssignment">Puesto a guardar (roAssignment)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>
    Public Function SaveAssignment(ByVal oAssignment As roAssignment, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roAssignment) Implements IAssignmentSvc.SaveAssignment
        Return AssignmentMethods.SaveAssignment(oAssignment, oState, bAudit)
    End Function


    ''' <summary>
    ''' Elimina lel puesto con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID del puestoa eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function DeleteAssignment(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAssignmentSvc.DeleteAssignment
        Return AssignmentMethods.DeleteAssignment(ID, oState, bAudit)

    End Function


End Class
