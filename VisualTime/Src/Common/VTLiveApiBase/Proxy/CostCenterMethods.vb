Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.CostCenter

Public Class CostCenterMethods

    Public Shared Function GetCostCenters(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roCostCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oCostCenters As New roCostCenterList(bState)
        Dim ds As New DataSet
        ds.Tables.Add(oCostCenters.GetCostCenters())

        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAvailableCostCentersDataTable(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal bolCheckPassportPermission As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roBusinessCenter.GetAvailableBusinessCentersDataTable(bState, IDEmployee, xDate, bolCheckPassportPermission)
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

    Public Shared Function GetEmployeeWorkingCostCenter(ByVal IDEmployee As Integer, ByVal IdCostCener As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roBusinessCenter)

        Dim bState = New roBusinessCenterState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roBusinessCenter)
        Dim _BusinessCenter As New roBusinessCenter
        Dim BusinessName As String = roBusinessCenter.GetEmployeeWorkingCostCenter(bState, IDEmployee, IdCostCener, _BusinessCenter)
        If BusinessName.Length > 0 Then
            oResult.Value = _BusinessCenter
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class