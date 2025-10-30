Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Capture
Imports Robotics.Base.VTBusiness.Common

Public Class CaptureMethods

    Public Shared Function GetCaptureByID(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roCapture)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCaptureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roCapture)
        oResult.Value = New roCapture(intID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveCapture(ByVal oCapture As roCapture, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCaptureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oCapture.State = bState
        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oCapture.Save()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCapture.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteCapture(ByVal oCapture As roCapture, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCaptureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        oCapture.State = bState

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oCapture.Delete()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oCapture.State, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteCaptureByID(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roCaptureState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oCapture As New roCapture(intID, bState)
        oResult.Value = oCapture.Delete()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class