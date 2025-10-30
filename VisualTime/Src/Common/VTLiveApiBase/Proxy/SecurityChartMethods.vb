Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Security.Base

Public Class SecurityChartMethods

    Public Shared Function GetGroupFeatures(ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature())

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroupFeature())

        oResult.Value = roGroupFeatureManager.GetGroupFeaturesList(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetConsultantGroupFeature(ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature)

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroupFeature)

        oResult.Value = roGroupFeatureManager.GetConsultantGroupFeature(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetRoboticsGroupFeaturesList(ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature())

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroupFeature())

        oResult.Value = roGroupFeatureManager.GetRoboticsGroupFeaturesList(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function




    Public Shared Function GetGroupFeaturesById(ByVal iIdGroupFeature As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature)
        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroupFeature)

        Dim oManager As New roGroupFeatureManager(bState)
        oResult.Value = oManager.Load(iIdGroupFeature, True, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function DeleteGroupFeature(ByVal oGroupFeature As roGroupFeature, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oManager As New roGroupFeatureManager(bState)
        oResult.Value = oManager.Delete(oGroupFeature, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function SaveGroupFeatures(ByVal oGroupFeature As roGroupFeature, ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature)

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roGroupFeature)

        Dim oManager As New roGroupFeatureManager(bState)
        If oManager.Save(oGroupFeature, True, True) Then
            oResult.Value = oGroupFeature
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function SetGroupFeaturePermission(ByVal iGroupFeatureID As Integer, ByVal iFeatureID As Integer, ByVal iPermission As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roGroupFeatureManager.SetGroupFeaturePermission(iGroupFeatureID, iFeatureID, iPermission, bState, True)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function CopyGroupFeature(ByVal iGroupFeatureID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roGroupFeatureManager.CopyGroupFeature(iGroupFeatureID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetAllExternalIds(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        Dim bState = New roGroupFeatureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roGroupFeatureManager.GetAllExternalIds(bState)
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