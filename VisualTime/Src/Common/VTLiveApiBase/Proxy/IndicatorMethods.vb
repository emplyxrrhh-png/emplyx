Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Indicator

Public Class IndicatorMethods

    ''' <summary>
    ''' Obtiene la lista de indicadores definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de los indicadores definidos.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetIndicators(ByVal _Type As IndicatorsType, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roIndicator))
        Dim bState = New roIndicatorState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roIndicator))
        oResult.Value = roIndicator.GetIndicators(_Order, bState, _Type, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de indicadores definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Un objeto dataset con una tabla con los indicadores definidos.</returns>
    ''' <remarks></remarks>
    Public Shared Function GetIndicatorsDataTable(ByVal _Type As IndicatorsType, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roIndicatorState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roIndicator.GetIndicatorsDataTable(_Order, bState, _Type, _Audit)
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

    Public Shared Function GetIndicator(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roIndicator)

        Dim bState = New roIndicatorState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roIndicator)
        oResult.Value = New roIndicator(_ID, bState, _Audit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Guarda los datos del indicador. Si és nuevo, se actualiza el ID del indicador pasado.<br/>
    ''' </summary>
    ''' <param name="oIndicator">Puesto a guardar (roIndicaor)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>
    Public Shared Function SaveIndicator(ByVal _Type As IndicatorsType, ByVal oIndicator As roIndicator, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roIndicator)

        oIndicator.State = New roIndicatorState(-1)
        roWsStateManager.CopyTo(oState, oIndicator.State)
        oIndicator.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roIndicator)
        If oIndicator.Save(_Type, bAudit) Then
            oResult.Value = oIndicator
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oIndicator.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Elimina el indicador con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID del indicador a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function DeleteIndicator(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        Dim bState = New roIndicatorState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oIndicator As New roIndicator(ID, bState, False)
        oResult.Value = oIndicator.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oIndicator.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Metodo utilizado para publicar el enum IndicatorCompareType
    ''' </summary>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Shared Function ExportIndicatorComparteType(ByVal enumVal As IndicatorCompareType) As roGenericVtResponse(Of Boolean)
        Dim bState = New roIndicatorState(-1)

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = True

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class