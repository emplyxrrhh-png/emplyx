Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.DiningRoom

Public Class DiningRoomProxy
    Implements IDiningRoomSvc

    Public Function KeepAlive() As Boolean Implements IDiningRoomSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Recupera un Turno
    ''' </summary>
    ''' <param name="ID">ID del turno a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Turno (roDiningRoomTurn)</returns>
    Public Function GetDiningRoomTurn(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDiningRoomTurn) Implements IDiningRoomSvc.GetDiningRoomTurn
        Return DiningRoomMethods.GetDiningRoomTurn(ID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Recupera un Turno
    ''' </summary>
    ''' <param name="ID">ID del turno a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Turno (roDiningRoomTurn)</returns>
    Public Function GetDiningRoomTurnsByDiningRoom(ByVal intIDDiningRoom As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IDiningRoomSvc.GetDiningRoomTurnsByDiningRoom
        Return DiningRoomMethods.GetDiningRoomTurnsByDiningRoom(intIDDiningRoom, oState)
    End Function


    ''' <summary>
    ''' Recupera un Turno
    ''' </summary>
    ''' <param name="ID">ID del turno a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Turno (roDiningRoomTurn)</returns>
    Public Function SaveDiningRoomTurn(ByVal oDiningRoomTurn As roDiningRoomTurn, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDiningRoomTurn) Implements IDiningRoomSvc.SaveDiningRoomTurn
        Return DiningRoomMethods.SaveDiningRoomTurn(oDiningRoomTurn, oState, bAudit)
    End Function

    ''' <summary>
    ''' Elimina Turno por ID
    ''' </summary>
    ''' <param name="IDTurn">ID del Turno a eliminar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function DeleteDiningRoomTurn(ByVal IDTurn As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IDiningRoomSvc.DeleteDiningRoomTurn
        Return DiningRoomMethods.DeleteDiningRoomTurn(IDTurn, oState, bAudit)
    End Function

    ''' <summary>
    ''' Validación del Turno
    ''' </summary>
    ''' <param name="oDiningRoomTurn">Turno a validar (roShift)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Function ValidateDiningRoomTurn(ByVal oDiningRoomTurn As roDiningRoomTurn, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDiningRoomSvc.ValidateDiningRoomTurn
        Return DiningRoomMethods.ValidateDiningRoomTurn(oDiningRoomTurn, oState)
    End Function

    ''' <summary>
    ''' Crea una copia de un Turno existente.
    ''' </summary>
    ''' <param name="_IDSourceTurn">Código del Turno del que se quiere realizar la copia</param>
    ''' <param name="_NewName">Nombre del nuevo Turno creado. Si no se informa, se utiliza el tag de idioma 'DiningRoomTurns.DiningRoomTurnsSave.Copy' para generar el nuevo nombre (copia de ...).</param>
    ''' <param name="oState"></param>
    ''' <returns>Nuevo horario creado</returns>
    ''' <remarks></remarks>
    Public Function CopyDiningRoomTurn(ByVal _IDSourceTurn As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDiningRoomTurn) Implements IDiningRoomSvc.CopyDiningRoomTurn
        Return DiningRoomMethods.CopyDiningRoomTurn(_IDSourceTurn, _NewName, oState, bAudit)
    End Function

    ''' <summary>
    ''' Retorna true si existe el id especificado. Si se pasa un -1 retorna true si hay registros en la tabla
    ''' </summary>
    Public Function ExitsDiningRoomTurn(ByVal IdDiningRoomTurn As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IDiningRoomSvc.ExitsDiningRoomTurn
        Return DiningRoomMethods.ExitsDiningRoomTurn(IdDiningRoomTurn, oState)
    End Function


End Class
