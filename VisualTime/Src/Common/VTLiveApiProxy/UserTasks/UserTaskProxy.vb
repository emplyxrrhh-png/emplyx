Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.UserTask

Public Class UserTaskProxy
    Implements IUserTaskSvc

    Public Function KeepAlive() As Boolean Implements IUserTaskSvc.KeepAlive
        Return True
    End Function

    Public Function GetUserTasks(ByVal _TaskType As TaskType, ByVal _TaskCompletedState As TaskCompletedState, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserTaskSvc.GetUserTasks
        Return UserTaskMethods.GetUserTasks(_TaskType, _TaskCompletedState, oState)
    End Function

    Public Function GetUserTask(ByVal _ID As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roUserTask) Implements IUserTaskSvc.GetUserTask

        Return UserTaskMethods.GetUserTask(_ID, oState, bAudit)

    End Function

    Public Function SaveUserTask(ByVal oUserTask As roUserTask, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserTaskSvc.SaveUserTask

        Return UserTaskMethods.SaveUserTask(oUserTask, oState, bAudit)

    End Function

    Public Function DeleteUserTask(ByVal oUserTask As roUserTask, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserTaskSvc.DeleteUserTask

        Return UserTaskMethods.DeleteUserTask(oUserTask, oState, bAudit)

    End Function

    Public Function DeleteUserTaskById(ByVal _ID As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IUserTaskSvc.DeleteUserTaskById

        Return UserTaskMethods.DeleteUserTaskById(_ID, oState, bAudit)

    End Function

End Class
