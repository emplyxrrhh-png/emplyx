Imports System.IO
Imports System.Web.Hosting
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTToDoLists
Imports Robotics.Security
Imports Robotics.Security.Base

Namespace VTPortal

    Public Class ToDoListHelper

        Public Shared Function GetAllToDoListsByType(ByVal oPassport As roPassportTicket, ByVal type As ToDoListType, ByVal oToDoListState As roToDoListState) As OnBoardingList
            Dim lrret As New OnBoardingList()
            Dim lista As New Generic.List(Of OnBoardingInfo)

            Try
                lrret.Status = ErrorCodes.OK
                Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Resources/userDefault.png")
                Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

                Dim ImageData As Byte()
                ImageData = New Byte(fileStream.Length - 1) {}
                fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
                fileStream.Close()

                Dim oToDoListManager As New roToDoListManager(oToDoListState)
                Dim oToDolists As Generic.List(Of roToDoList) = oToDoListManager.GetAllToDoListsByType(type, True, False)

                For Each list As roToDoList In oToDolists

                    Dim OnBoardingListObject As New OnBoardingInfo
                    OnBoardingListObject.IdEmployee = list.IdEmployee
                    OnBoardingListObject.StartDate = list.StartDate
                    OnBoardingListObject.Status = list.Status
                    OnBoardingListObject.EmployeeName = list.EmployeeName
                    OnBoardingListObject.Comments = list.Comments
                    If list.EmployeeImage IsNot Nothing Then
                        OnBoardingListObject.Image = VTPortal.EmployeesHelper.LoadEmployeeImage(list.EmployeeImage, Nothing)
                    Else
                        OnBoardingListObject.Image = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"
                    End If

                    OnBoardingListObject.IdList = list.Id

                    lista.Add(OnBoardingListObject)
                Next

                lrret.OnBoardings = lista.ToArray
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::OnboardingsHelper::GetSupervisedOnboardings")
            End Try

            Return lrret
        End Function

        Public Shared Function GetOnBoardingTasksById(ByVal oPassport As roPassportTicket, ByVal idOnboarding As Integer, ByVal oToDoListState As roToDoListState) As OnBoardingTasks
            Dim lrret As New OnBoardingTasks()
            Dim lista As New Generic.List(Of TaskInfo)

            Try
                lrret.Status = ErrorCodes.OK

                Dim oToDoListManager As New roToDoListManager(oToDoListState)
                Dim oToDolists As Generic.List(Of roToDoTask) = oToDoListManager.GetToDoListTasks(idOnboarding, False)

                For Each list As roToDoTask In oToDolists

                    Dim OnBoardingListObject As New TaskInfo
                    OnBoardingListObject.TaskName = list.TaskName
                    OnBoardingListObject.IdList = list.IdList
                    OnBoardingListObject.Id = list.Id
                    OnBoardingListObject.Done = list.Done
                    OnBoardingListObject.LastChangeDate = list.LastChangeDate
                    OnBoardingListObject.SupervisorName = list.SupervisorName

                    lista.Add(OnBoardingListObject)
                Next

                lrret.Tasks = lista.ToArray
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::OnboardingsHelper::GetOnBoardingTasksById")
            End Try

            Return lrret
        End Function

        Public Shared Function UpdateTaskStatus(ByVal oPassport As roPassportTicket, ByVal status As Boolean, ByVal idTask As Integer, ByVal oToDoListState As roToDoListState) As Boolean
            Dim lrret As Boolean
            Try
                lrret = True
                Dim oToDoListManager As New roToDoListManager(oToDoListState)
                Dim ListTaskToCreate As New Generic.List(Of roToDoTask)
                Dim Task As New roToDoTask()
                Task = oToDoListManager.GetToDoTask(idTask, Nothing)
                Task.Done = status
                Task.SupervisorName = oPassport.Name
                Task.LastChangeDate = Date.Now

                ListTaskToCreate.Add(Task)

                Dim oToDolists As Generic.List(Of roToDoTask) = oToDoListManager.CreateOrUpdateToDoTasks(ListTaskToCreate)
                If oToDolists.Count = 1 Then
                    lrret = True
                Else
                    lrret = False
                End If
            Catch ex As Exception
                lrret = False

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::OnboardingsHelper::GetOnBoardingTasksById")
            End Try

            Return lrret
        End Function

    End Class

End Namespace