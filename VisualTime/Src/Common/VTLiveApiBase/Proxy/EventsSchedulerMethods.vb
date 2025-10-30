Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.EventScheduler

Public Class EventsSchedulerMethods

    Public Shared Function GetEventsScheduler(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roEventScheduler))

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roEventScheduler))
        oResult.Value = roEventScheduler.GetEventsScheduler(bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEventScheduler(ByVal IDEventScheduler As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEventScheduler)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEventScheduler)
        oResult.Value = New roEventScheduler(IDEventScheduler, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetEventSchedulerByYear(ByVal Year As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roEventScheduler.GetEventsSchedulerByYear(Year, bState)
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

    Public Shared Function GetEventSchedulerByName(ByVal Name As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roEventScheduler.GetEventsSchedulerByName(Name, bState)
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

    Public Shared Function SaveEventScheduler(ByVal oEventScheduler As roEventScheduler, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEventScheduler)

        oEventScheduler.State = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, oEventScheduler.State)
        oEventScheduler.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEventScheduler)
        If oEventScheduler.Save(bAudit) Then
            oResult.Value = oEventScheduler
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oEventScheduler.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteEventScheduler(ByVal oEventScheduler As roEventScheduler, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oEventScheduler.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oEventScheduler.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteEventSchedulerByID(ByVal IDEventScheduler As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oEventScheduler As New roEventScheduler(IDEventScheduler, bState, False)
        oEventScheduler.State = bState
        oResult.Value = oEventScheduler.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oEventScheduler.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetEventAuthorizations(ByVal IDEvent As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roEventAccessAuthorization.GetAuthorizationsDataTable(IDEvent, bState)
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

    Public Shared Function CopyEvent(ByVal _IDSourceEvent As Integer, ByVal _NewName As String, ByVal _NewDate As Date, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roEventScheduler)

        Dim bState = New roEventSchedulerState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roEventScheduler)
        oResult.Value = roEventScheduler.CopyEvent(_IDSourceEvent, _NewName, _NewDate, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class