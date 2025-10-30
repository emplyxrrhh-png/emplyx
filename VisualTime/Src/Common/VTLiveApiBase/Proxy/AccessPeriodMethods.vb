Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessPeriod
Imports Robotics.Base.VTBusiness.Common

Public Class AccessPeriodMethods

    Public Shared Function GetAccessPeriods(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAccessPeriod))

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roAccessPeriod))
        oResult.Value = roAccessPeriod.GetAccessPeriodList(bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetAccessPeriodsDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roAccessPeriod.GetAccessPeriodDataTable(bState)
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

    Public Shared Function GetAccessPeriodByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAccessPeriod)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAccessPeriod)
        oResult.Value = New roAccessPeriod(intID, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveAccessPeriod(ByVal oAccessPeriod As roAccessPeriod, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAccessPeriod)
        'cambio mi state genérico a un estado especifico
        'Dim bState = New roAccessPeriodState(-1)
        oAccessPeriod.State = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, oAccessPeriod.State)
        'roWsStateManager.CopyTo(oState, bState)
        'bState.UpdateStateInfo()
        oAccessPeriod.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAccessPeriod)
        If (oAccessPeriod.Save(bolAudit)) Then
            oResult.Value = oAccessPeriod
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessPeriod.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteAccessPeriod(ByVal oAccessPeriod As roAccessPeriod, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        'Dim bState = New roAccessPeriodState(-1)
        oAccessPeriod.State = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, oAccessPeriod.State)
        'roWsStateManager.CopyTo(oState, bState)
        'bState.UpdateStateInfo()
        oAccessPeriod.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oAccessPeriod.Delete(bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessPeriod.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteAccessPeriodByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oAccessPeriod As New roAccessPeriod(intID, bState, False)
        oResult.Value = oAccessPeriod.Delete(bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function getAccessPeriodDailyDescription(ByVal oAccessPeriodDaily As roAccessPeriodDaily, ByVal oState As roWsState) As roGenericVtResponse(Of String)
        'cambio mi state genérico a un estado especifico
        'Dim bState = New roAccessPeriodState(-1)
        oAccessPeriodDaily.State = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, oAccessPeriodDaily.State)
        'roWsStateManager.CopyTo(oState, bState)
        'bState.UpdateStateInfo()
        oAccessPeriodDaily.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = oAccessPeriodDaily.Description

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessPeriodDaily.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function getAccessPeriodHolidaysDescription(ByVal oAccessPeriodHolidays As roAccessPeriodHolidays, ByVal oState As roWsState) As roGenericVtResponse(Of String)
        'cambio mi state genérico a un estado especifico
        'Dim bState = New roAccessPeriodState(-1)
        oAccessPeriodHolidays.State = New roAccessPeriodState(-1)
        roWsStateManager.CopyTo(oState, oAccessPeriodHolidays.State)
        'roWsStateManager.CopyTo(oState, bState)
        'bState.UpdateStateInfo()
        oAccessPeriodHolidays.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        oResult.Value = oAccessPeriodHolidays.Description

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessPeriodHolidays.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class