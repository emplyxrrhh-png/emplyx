Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTBusiness.Common

Public Class CausesMethods

    ''' <summary>
    ''' Recupera justificación por ID
    ''' </summary>
    ''' <param name="IDCause">ID de justificación a recuperar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve una justificación (roCause)</returns>
    ''' <remarks></remarks>
    Public Shared Function GetCauseByID(ByVal IDCause As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCause)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roCause)
        oResult.Value = New roCause(IDCause, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve una lista de justificaciones
    ''' </summary>
    ''' <param name="strWhere">Extension Where SQL</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (Causes) ordenado por Name</returns>
    ''' <remarks></remarks>
    Public Shared Function GetCauses(ByVal strWhere As String, ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oCause As New roCause(-1, bState)
        Dim tbCauses As DataTable = oCause.Causes(strWhere, bolFilterBusinessGroups)
        If tbCauses IsNot Nothing Then
            If tbCauses.DataSet IsNot Nothing Then
                ds = tbCauses.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tbCauses)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve la lista de justificaciones a las que tiene acceso de fichaje (pestaña fichajes de la pantalla de definición de justificaciones) el empleado indicado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCausesByEmployeeInputPermissions(ByVal intIDEmployee As Integer, ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oCause As New roCause(-1, bState)
        Dim tbCauses As DataTable = roCause.GetCausesByEmployeeInputPermissions(intIDEmployee, bState, bolFilterBusinessGroups)
        If tbCauses IsNot Nothing Then
            If tbCauses.DataSet IsNot Nothing Then
                ds = tbCauses.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tbCauses)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve la lista de justificaciones a las que tiene acceso de justificar (pestaña visibilidad de la pantalla de definición de justificaciones) el empleado indicado.
    ''' </summary>
    ''' <param name="intIDEmployee">Código del empleado</param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetCausesByEmployeeVisibilityPermissions(ByVal intIDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oCause As New roCause(-1, bState)
        Dim tbCauses As DataTable = roCause.GetCausesByEmployeeVisibilityPermissions(intIDEmployee, bState)
        If tbCauses IsNot Nothing Then
            If tbCauses.DataSet IsNot Nothing Then
                ds = tbCauses.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tbCauses)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Devuelve una lista de justificaciones (formato corto)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (Causes:ID, Causes:Name) ordenado por Name</returns>
    ''' <remarks></remarks>
    Public Shared Function GetCausesShortList(ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oCause As New roCause(-1, bState)
        Dim tbCauses As DataTable = oCause.CausesShortList(bolFilterBusinessGroups)
        If tbCauses IsNot Nothing Then
            If tbCauses.DataSet IsNot Nothing Then
                ds = tbCauses.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tbCauses)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve una lista de justificaciones (formato corto)
    ''' </summary>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>DataSet (Causes:ID, Causes:Name) ordenado por Name</returns>
    ''' <remarks></remarks>
    Public Shared Function CausesShortListByRequestType(ByVal eRequestType As eCauseRequest, ByVal bolFilterBusinessGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim oCause As New roCause(-1, bState)
        Dim tbCauses As DataTable = oCause.CausesShortListByRequestType(eRequestType, bolFilterBusinessGroups)
        If tbCauses IsNot Nothing Then
            If tbCauses.DataSet IsNot Nothing Then
                ds = tbCauses.DataSet
            Else
                ds = New DataSet
                ds.Tables.Add(tbCauses)
            End If
        End If

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
    ''' </summary>
    Public Shared Function GetBusinessGroupFromCauseGroups(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oCause As New roCause(-1, bState)
        Dim tbCauses As DataSet = roCause.GetBusinessGroupFromCauseGroups(bState)

        If tbCauses Is Nothing Then
            Dim ds As New DataSet
            ds.Tables.Add(tbCauses.Tables.Item(0))
            oResult.Value = ds
        Else
            oResult.Value = tbCauses

        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda la justificación <br/>
    ''' Ejecuta ValidateCause() y Task.CAUSES
    ''' </summary>
    ''' <param name="oCause">Justificación a guardar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha realizado la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function SaveCause(ByVal oCause As roCause, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCause)
        'cambio mi state genérico a un estado especifico
        oCause.State = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, oCause.State)
        oCause.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roCause)

        If oCause.Save(bAudit) Then
            oResult.Value = oCause
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCause.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Valida la justificación<br/>
    ''' Comprueba que:<br/>
    ''' - El nombre no puede estar en blanco<br/>
    ''' - El nombre corto no puede estar en blanco<br/>
    ''' - El nombre no puede existir en la bdd para otra justificación<br/>
    ''' - El nombre corto no puede existir en la bdd para otra justificación<br/>
    ''' - Valida campos (...)<br/>
    ''' - ReaderInputCode no se este utilizando en otra justificación
    ''' </summary>
    ''' <param name="oCause"></param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha realizado la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function ValidateCause(ByVal oCause As roCause, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oCause.ValidateCause()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Elimina la justificación por ID<br/>
    ''' Borra: <br/>
    ''' - DailyCauses<br/>
    ''' - ConceptCauses<br/>
    ''' - Causes
    ''' </summary>
    ''' <param name="IDCause">ID de justificación a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha realizado la acción</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function DeleteCause(ByVal IDCause As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oCause As New roCause(IDCause, bState, False)
        oResult.Value = oCause.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    ''' <summary>
    ''' Comprueba si la justificación esta en uso<br/>
    ''' - En algún horario (CauseUsedInShift)<br/>
    ''' - En algún acumulado (CauseUsedinAccrual)<br/>
    ''' - En alguna ausencia prolongada (CauseUsedInProgrammedAbsence)<br/>
    ''' - En alguna incidencia prevista (CauseUsedInProgrammedCause)<br/>
    ''' - En algún fichaje con incidencia por terminal (CauseUsedInPunchWithIncidence)
    ''' </summary>
    ''' <param name="IDCause"></param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se encuentra en uso</returns>
    ''' <remarks>Si devuelve FALSE, en oState.ResultEnum devuelve el motivo</remarks>
    Public Shared Function CauseIsUsed(ByVal IDCause As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCauseState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oCause As New roCause(IDCause, bState, False)
        oResult.Value = (oCause.IsUsed Or oCause.CauseUsedWithEquivalenceCause <> String.Empty)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

End Class