Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common

Public Class AccessGroupMethods

    Public Shared Function GetAccessGroups(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAccessGroup))
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roAccessGroup))
        oResult.Value = roAccessGroup.GetAccessGroupsList(bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAccessGroupsDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roAccessGroup.GetAccessGroupsDataTable(bState)
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

    Public Shared Function GetAccessGroupsByPassport(ByVal oState As roWsState, ByVal intIDPassport As Integer) As roGenericVtResponse(Of Integer())
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer())
        oResult.Value = roAccessGroup.GetAccessGroupsByPassport(bState, intIDPassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAccessGroupsByPassportDataSet(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal validateParent As Boolean) As roGenericVtResponse(Of DataSet)
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As New DataSet
        Dim tb As DataTable = Nothing
        tb = roAccessGroup.GetAccessGroupsByPassportDataTable(bState, intIDPassport, validateParent)
        If tb IsNot Nothing Then ds.Tables.Add(tb)
        oResult.Value = ds

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAccessGroupByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean, ByVal idPassport As Integer) As roGenericVtResponse(Of roAccessGroup)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAccessGroup)
        oResult.Value = New roAccessGroup(intID, bState, bolAudit, idPassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function SaveAccessGroupByPassport(ByVal intIDPassport As Integer, ByVal intAccessGroups() As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roAccessGroup.SaveAccessGroupByPassport(bState, intIDPassport, intAccessGroups, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function SaveAccessGroup(ByVal oAccessGroup As roAccessGroup, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAccessGroup)
        'cambio mi state genérico a un estado especifico
        oAccessGroup.State = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, oAccessGroup.State)
        oAccessGroup.State.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAccessGroup)
        If oAccessGroup.Save(bolAudit) Then
            oResult.Value = oAccessGroup
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroup.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteAccessGroup(ByVal oAccessGroup As roAccessGroup, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oAccessGroup.State = bState
        oResult.Value = oAccessGroup.Delete(bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroup.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function DeleteAccessGroupByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oAccessGroup As New roAccessGroup(intID, bState, False)
        oResult.Value = oAccessGroup.Delete(bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oAccessGroup.State, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetEmployeeAuthorizations(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roAccessGroup.GetEmployeeAuthorizations(IDEmployee, IDGroup, bState)
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

    Public Shared Function CopyAccess(ByVal _IDSourceAccess As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roAccessGroup)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roAccessGroup)
        oResult.Value = roAccessGroup.CopyAccess(_IDSourceAccess, _NewName, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function EmptyAccessGroupEmployees(ByVal _IDSourceAccess As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roAccessGroup.EmptyAccessGroupEmployees(_IDSourceAccess, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function GetAuthorizationsByZone(ByVal IdZone As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAccessGroup))
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roAccessGroup))
        oResult.Value = roAccessGroup.GetAuthorizationsByZone(IdZone, bState, bolAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function UpgradeAccessMode(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roAccessGroup.UpgradeAccessMode(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

End Class