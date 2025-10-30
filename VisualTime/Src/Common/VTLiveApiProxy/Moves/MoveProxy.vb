Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Move

Public Class MoveProxy
    Implements IMoveSvc

    Public Function KeepAlive() As Boolean Implements IMoveSvc.KeepAlive
        Return True
    End Function
    Public Function GetProductionMoves(ByVal selectedDate As Date, ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IMoveSvc.GetProductionMoves
        Return MoveMethods.GetProductionMoves(selectedDate, IDEmployee, oState)
    End Function

    Public Function GetProductionMovesInPeriod(ByVal selectedDate As Date, ByVal endSelectedDate As Date, ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IMoveSvc.GetProductionMovesInPeriod
        Return MoveMethods.GetProductionMovesInPeriod(selectedDate, endSelectedDate, IDEmployee, oState)
    End Function



End Class
