Imports System.ServiceModel.Activation
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessPeriod
Imports Robotics.Base.VTBusiness.Common

<AspNetCompatibilityRequirements(RequirementsMode:=AspNetCompatibilityRequirementsMode.Required)>
Public Class AccessPeriodProxy
    Implements IAccessPeriodSvc

    Public Function KeepAlive() As Boolean Implements IAccessPeriodSvc.KeepAlive
        Return True
    End Function

    Public Function GetAccessPeriods(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAccessPeriod)) Implements IAccessPeriodSvc.GetAccessPeriods
        Return AccessPeriodMethods.GetAccessPeriods(oState, bolAudit)
    End Function

    Public Function GetAccessPeriodsDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAccessPeriodSvc.GetAccessPeriodsDataSet
        Return AccessPeriodMethods.GetAccessPeriodsDataSet(oState)
    End Function

    Public Function GetAccessPeriodByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAccessPeriod) Implements IAccessPeriodSvc.GetAccessPeriodByID
        Return AccessPeriodMethods.GetAccessPeriodByID(intID, oState, bolAudit)
    End Function


    Public Function SaveAccessPeriod(ByVal oAccessPeriod As roAccessPeriod, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAccessPeriod) Implements IAccessPeriodSvc.SaveAccessPeriod
        Return AccessPeriodMethods.SaveAccessPeriod(oAccessPeriod, oState, bolAudit)
    End Function

    Public Function DeleteAccessPeriod(ByVal oAccessPeriod As roAccessPeriod, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessPeriodSvc.DeleteAccessPeriod
        Return AccessPeriodMethods.DeleteAccessPeriod(oAccessPeriod, oState, bolAudit)
    End Function

    Public Function DeleteAccessPeriodByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessPeriodSvc.DeleteAccessPeriodByID
        Return AccessPeriodMethods.DeleteAccessPeriodByID(intID, oState, bolAudit)
    End Function

    Public Function getAccessPeriodDailyDescription(ByVal oAccessPeriodDaily As roAccessPeriodDaily, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IAccessPeriodSvc.getAccessPeriodDailyDescription
        Return AccessPeriodMethods.getAccessPeriodDailyDescription(oAccessPeriodDaily, oState)
    End Function

    Public Function getAccessPeriodHolidaysDescription(ByVal oAccessPeriodHolidays As roAccessPeriodHolidays, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IAccessPeriodSvc.getAccessPeriodHolidaysDescription
        Return AccessPeriodMethods.getAccessPeriodHolidaysDescription(oAccessPeriodHolidays, oState)
    End Function

End Class
