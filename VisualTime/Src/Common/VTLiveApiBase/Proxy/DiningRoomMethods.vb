Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.DiningRoom

Public Class DiningRoomMethods

    ''' <summary>
    ''' Recupera un Turno
    ''' </summary>
    ''' <param name="ID">ID del turno a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Turno (roDiningRoomTurn)</returns>
    Public Shared Function GetDiningRoomTurn(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDiningRoomTurn)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDiningRoomTurn)
        oResult.Value = New roDiningRoomTurn(ID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult.Value.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera un Turno
    ''' </summary>
    ''' <param name="ID">ID del turno a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Turno (roDiningRoomTurn)</returns>
    Public Shared Function GetDiningRoomTurnsByDiningRoom(ByVal intIDDiningRoom As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)

        Dim ds As New DataSet
        Dim tb As DataTable = Nothing
        Dim oDiningRoomTurn As New roDiningRoomTurn(-1, bState)
        tb = oDiningRoomTurn.GetDiningRoomTurns(intIDDiningRoom)
        If tb IsNot Nothing Then ds.Tables.Add(tb)

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDiningRoomTurn.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Recupera un Turno
    ''' </summary>
    ''' <param name="ID">ID del turno a recuperar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Turno (roDiningRoomTurn)</returns>
    Public Shared Function SaveDiningRoomTurn(ByVal oDiningRoomTurn As roDiningRoomTurn, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDiningRoomTurn)
        'cambio mi state genérico a un estado especifico
        oDiningRoomTurn.State = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, oDiningRoomTurn.State)
        oDiningRoomTurn.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDiningRoomTurn)

        If (oDiningRoomTurn.Save(bAudit)) Then
            oResult.Value = oDiningRoomTurn
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDiningRoomTurn.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina Turno por ID
    ''' </summary>
    ''' <param name="IDTurn">ID del Turno a eliminar</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function DeleteDiningRoomTurn(ByVal IDTurn As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oDiningRoomTurn As New roDiningRoomTurn()
        oDiningRoomTurn.State = bState
        oDiningRoomTurn.ID = IDTurn
        oDiningRoomTurn.Load(False)

        oResult.Value = oDiningRoomTurn.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDiningRoomTurn.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Validación del Turno
    ''' </summary>
    ''' <param name="oDiningRoomTurn">Turno a validar (roShift)</param>
    ''' <param name="oState">Información adicional de estado.</param>
    ''' <returns>Devuelve TRUE si se ha podido realizar la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function ValidateDiningRoomTurn(ByVal oDiningRoomTurn As roDiningRoomTurn, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oDiningRoomTurn.State = bState
        oResult.Value = oDiningRoomTurn.Validate()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oDiningRoomTurn.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Crea una copia de un Turno existente.
    ''' </summary>
    ''' <param name="_IDSourceTurn">Código del Turno del que se quiere realizar la copia</param>
    ''' <param name="_NewName">Nombre del nuevo Turno creado. Si no se informa, se utiliza el tag de idioma 'DiningRoomTurns.DiningRoomTurnsSave.Copy' para generar el nuevo nombre (copia de ...).</param>
    ''' <param name="oState"></param>
    ''' <returns>Nuevo horario creado</returns>
    ''' <remarks></remarks>
    Public Shared Function CopyDiningRoomTurn(ByVal _IDSourceTurn As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roDiningRoomTurn)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roDiningRoomTurn)
        oResult.Value = roDiningRoomTurn.CopyDiningRoomTurn(_IDSourceTurn, _NewName, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult.Value.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Retorna true si existe el id especificado. Si se pasa un -1 retorna true si hay registros en la tabla
    ''' </summary>
    Public Shared Function ExitsDiningRoomTurn(ByVal IdDiningRoomTurn As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roDiningRoomState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roDiningRoomTurn.ExitsDiningRoomTurn(IdDiningRoomTurn, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class