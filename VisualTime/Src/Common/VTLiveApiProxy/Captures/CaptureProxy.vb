Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Capture
Imports Robotics.Base.VTBusiness.Common

Public Class CaptureProxy
    Implements ICaptureSvc

    Public Function KeepAlive() As Boolean Implements ICaptureSvc.KeepAlive
        Return True
    End Function

    Public Function GetCaptureByID(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roCapture) Implements ICaptureSvc.GetCaptureByID
        Return CaptureMethods.GetCaptureByID(intID, oState)
    End Function

    Public Function SaveCapture(ByVal oCapture As roCapture, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ICaptureSvc.SaveCapture
        Return CaptureMethods.SaveCapture(oCapture, oState)
    End Function

    Public Function DeleteCapture(ByVal oCapture As roCapture, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ICaptureSvc.DeleteCapture
        Return CaptureMethods.DeleteCapture(oCapture, oState)
    End Function

    Public Function DeleteCaptureByID(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ICaptureSvc.DeleteCaptureByID
        Return CaptureMethods.DeleteCaptureByID(intID, oState)
    End Function

End Class
