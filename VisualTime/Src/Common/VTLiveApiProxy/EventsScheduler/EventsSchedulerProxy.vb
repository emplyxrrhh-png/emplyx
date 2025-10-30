Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.EventScheduler

Public Class EventsSchedulerProxy
    Implements IEventsSchedulerSvc

    Public Function KeepAlive() As Boolean Implements IEventsSchedulerSvc.KeepAlive
        Return True
    End Function

    Public Function GetEventsScheduler(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roEventScheduler)) Implements IEventsSchedulerSvc.GetEventsScheduler
        Return EventsSchedulerMethods.GetEventsScheduler(oState, bAudit)
    End Function

    Public Function GetEventScheduler(ByVal IDEventScheduler As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEventScheduler) Implements IEventsSchedulerSvc.GetEventScheduler
        Return EventsSchedulerMethods.GetEventScheduler(IDEventScheduler, oState, bAudit)
    End Function

    Public Function GetEventSchedulerByYear(ByVal Year As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet) Implements IEventsSchedulerSvc.GetEventSchedulerByYear
        Return EventsSchedulerMethods.GetEventSchedulerByYear(Year, oState, bAudit)
    End Function

    Public Function GetEventSchedulerByName(ByVal Name As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet) Implements IEventsSchedulerSvc.GetEventSchedulerByName
        Return EventsSchedulerMethods.GetEventSchedulerByName(Name, oState, bAudit)
    End Function

    Public Function SaveEventScheduler(ByVal oEventScheduler As roEventScheduler, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEventScheduler) Implements IEventsSchedulerSvc.SaveEventScheduler
        Return EventsSchedulerMethods.SaveEventScheduler(oEventScheduler, oState, bAudit)
    End Function

    Public Function DeleteEventScheduler(ByVal oEventScheduler As roEventScheduler, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEventsSchedulerSvc.DeleteEventScheduler
        Return EventsSchedulerMethods.DeleteEventScheduler(oEventScheduler, oState, bAudit)
    End Function

    Public Function DeleteEventSchedulerByID(ByVal IDEventScheduler As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IEventsSchedulerSvc.DeleteEventSchedulerByID
        Return EventsSchedulerMethods.DeleteEventSchedulerByID(IDEventScheduler, oState, bAudit)
    End Function

    Public Function GetEventAuthorizations(ByVal IDEvent As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IEventsSchedulerSvc.GetEventAuthorizations
        Return EventsSchedulerMethods.GetEventAuthorizations(IDEvent, oState)
    End Function

    Public Function CopyEvent(ByVal _IDSourceEvent As Integer, ByVal _NewName As String, ByVal _NewDate As Date, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEventScheduler) Implements IEventsSchedulerSvc.CopyEvent
        Return EventsSchedulerMethods.CopyEvent(_IDSourceEvent, _NewName, _NewDate, oState, bAudit)
    End Function


End Class
