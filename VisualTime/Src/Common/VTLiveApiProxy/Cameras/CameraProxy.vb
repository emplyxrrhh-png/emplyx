Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Camera

Public Class CameraProxy
    Implements ICameraSvc

    Public Function KeepAlive() As Boolean Implements ICameraSvc.KeepAlive
        Return True
    End Function


    Public Function GetCameras(ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roCamera)) Implements ICameraSvc.GetCameras
        Return CameraMethods.GetCameras(oState, bAudit)
    End Function

    Public Function GetCamerasDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements ICameraSvc.GetCamerasDataSet
        Return CameraMethods.GetCamerasDataSet(oState)
    End Function

    Public Function GetCameraByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCamera) Implements ICameraSvc.GetCameraByID
        Return CameraMethods.GetCameraByID(intID, oState, bAudit)
    End Function

    Public Function SaveCamera(ByVal oCamera As roCamera, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roCamera) Implements ICameraSvc.SaveCamera
        Return CameraMethods.SaveCamera(oCamera, oState, bAudit)
    End Function

    Public Function DeleteCamera(ByVal oCamera As roCamera, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ICameraSvc.DeleteCamera
        Return CameraMethods.DeleteCamera(oCamera, oState, bAudit)
    End Function

    Public Function DeleteCameraByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements ICameraSvc.DeleteCameraByID
        Return CameraMethods.DeleteCameraByID(intID, oState, bAudit)
    End Function

    ''' <summary>
    ''' Retorna true si existe el id especificado. Si se pasa un -1 retorna true si hay registros en la tabla
    ''' </summary>
    Public Function ExitsCamera(ByVal IDCamera As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ICameraSvc.ExitsCamera
        Return CameraMethods.ExitsCamera(IDCamera, oState)
    End Function


End Class
