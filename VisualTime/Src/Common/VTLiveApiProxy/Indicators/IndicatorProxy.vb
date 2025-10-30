Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Indicator

Public Class IndicatorProxy
    Implements IIndicatorSvc

    Public Function KeepAlive() As Boolean Implements IIndicatorSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' Obtiene la lista de indicadores definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Una lista de objetos con la definición de los indicadores definidos.</returns>
    ''' <remarks></remarks>
    Public Function GetIndicators(ByVal _Type As IndicatorsType, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roIndicator)) Implements IIndicatorSvc.GetIndicators
        Return IndicatorMethods.GetIndicators(_Type, _Order, _Audit, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de indicadores definidos.
    ''' </summary>
    ''' <param name="_Order"></param>
    ''' <param name="oState"></param>
    ''' <param name="_Audit"></param>
    ''' <returns>Un objeto dataset con una tabla con los indicadores definidos.</returns>
    ''' <remarks></remarks>
    Public Function GetIndicatorsDataTable(ByVal _Type As IndicatorsType, ByVal _Order As String, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IIndicatorSvc.GetIndicatorsDataTable
        Return IndicatorMethods.GetIndicatorsDataTable(_Type, _Order, _Audit, oState)
    End Function

    Public Function GetIndicator(ByVal _ID As Integer, ByVal _Audit As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roIndicator) Implements IIndicatorSvc.GetIndicator
        Return IndicatorMethods.GetIndicator(_ID, _Audit, oState)
    End Function


    ''' <summary>
    ''' Guarda los datos del indicador. Si és nuevo, se actualiza el ID del indicador pasado.<br/>
    ''' </summary>
    ''' <param name="oIndicator">Puesto a guardar (roIndicaor)</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha podido guardar el puesto.</returns>
    ''' <remarks></remarks>
    Public Function SaveIndicator(ByVal _Type As IndicatorsType, ByVal oIndicator As roIndicator, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roIndicator) Implements IIndicatorSvc.SaveIndicator
        Return IndicatorMethods.SaveIndicator(_Type, oIndicator, oState, bAudit)
    End Function


    ''' <summary>
    ''' Elimina el indicador con el ID indicado
    ''' </summary>
    ''' <param name="ID">ID del indicador a eliminar</param>
    ''' <param name="oState">Información adicional de estado</param>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function DeleteIndicator(ByVal ID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IIndicatorSvc.DeleteIndicator
        Return IndicatorMethods.DeleteIndicator(ID, oState, bAudit)
    End Function


    ''' <summary>
    ''' Metodo utilizado para publicar el enum IndicatorCompareType
    ''' </summary>
    ''' <returns>Devuelve TRUE si se ha eliminado correctamente</returns>
    ''' <remarks>Si es FALSE, en oState.ResultEnum devuelve el motivo.</remarks>
    Public Function ExportIndicatorComparteType(ByVal enumVal As IndicatorCompareType) As roGenericVtResponse(Of Boolean) Implements IIndicatorSvc.ExportIndicatorComparteType
        Return IndicatorMethods.ExportIndicatorComparteType(enumVal)
    End Function



End Class
