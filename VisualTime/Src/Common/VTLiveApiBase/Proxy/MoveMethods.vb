Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Move

Public Class MoveMethods

    Public Shared Function GetProductionMoves(ByVal selectedDate As Date, ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roMoveState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roJobMove.GetProductionMoves(selectedDate, IDEmployee, bState)
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

    Public Shared Function GetProductionMovesInPeriod(ByVal selectedDate As Date, ByVal endSelectedDate As Date, ByVal IDEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roMoveState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roJobMove.GetProductionMovesInPeriod(selectedDate, endSelectedDate, IDEmployee, bState)
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

End Class