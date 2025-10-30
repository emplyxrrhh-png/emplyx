Imports System.Data
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.CostCenter

Public Class CostCenterProxy
    Implements ICostCenterSvc

    Public Function KeepAlive() As Boolean Implements ICostCenterSvc.KeepAlive
        Return True
    End Function

    Public Function GetCostCenters(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICostCenterSvc.GetCostCenters
        Return CostCenterMethods.GetCostCenters(oState)
    End Function

    Public Function GetAvailableCostCentersDataTable(ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal bolCheckPassportPermission As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICostCenterSvc.GetAvailableCostCentersDataTable
        Return CostCenterMethods.GetAvailableCostCentersDataTable(IDEmployee, xDate, bolCheckPassportPermission, oState)
    End Function

    Public Function GetEmployeeWorkingCostCenter(ByVal IDEmployee As Integer, ByVal IdCostCener As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roBusinessCenter) Implements ICostCenterSvc.GetEmployeeWorkingCostCenter
        Return CostCenterMethods.GetEmployeeWorkingCostCenter(IDEmployee, IdCostCener, oState)
    End Function


End Class
