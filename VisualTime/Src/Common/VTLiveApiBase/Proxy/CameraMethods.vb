Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Camera
Imports Robotics.Base.VTBusiness.Common

Public Class CameraMethods

    Public Shared Function GetCameras(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roCamera))
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roCamera))
        oResult.Value = roCamera.GetCamerasList("", bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetCamerasDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim ds As DataSet = Nothing
        Dim tb As DataTable = roCamera.GetCamerasDataTable("", bState)
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

    Public Shared Function GetCameraByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCamera)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roCamera)
        oResult.Value = New roCamera(intID, bState, bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveCamera(ByVal oCamera As roCamera, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCamera)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roCamera)
        oCamera.State = bState
        If oCamera.Save(bAudit) Then
            oResult.Value = oCamera
        Else
            oResult.Value = Nothing
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCamera.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteCamera(ByVal oCamera As roCamera, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oCamera.State = bState
        oResult.Value = oCamera.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCamera.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteCameraByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oCamera As New roCamera(intID, bState, False)

        oCamera.State = bState
        oResult.Value = oCamera.Delete(bAudit)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCamera.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    ''' <summary>
    ''' Retorna true si existe el id especificado. Si se pasa un -1 retorna true si hay registros en la tabla
    ''' </summary>
    Public Shared Function ExitsCamera(ByVal IDCamera As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCameraState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roCamera.ExitsCamera(IDCamera, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class