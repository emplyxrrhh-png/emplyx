Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class TaskHelper
    Public Sub Load(ByVal geniusViewName As String)
        Robotics.VTBase.Extensions.VTLiveTasks.Fakes.ShimroLiveTask.AllInstances.LoadBoolean = Function(task, load)
                                                                                                   task.Action = roLiveTaskTypes.AnalyticsTask.ToString.ToUpper
                                                                                                   Dim parameters As roCollection = New roCollection()
                                                                                                   parameters.Add("APIVersion", "2")
                                                                                                   parameters.Add("AnalyticType", "2")
                                                                                                   parameters.Add("IdView", "1")
                                                                                                   parameters.Add("IDPassport", "1")
                                                                                                   parameters.Add("ScheduleBeginDate", DateTime.Now.AddMonths(-1))
                                                                                                   parameters.Add("ScheduleEndDate", DateTime.Now)
                                                                                                   parameters.Add("IncludeZeroValues", "1")
                                                                                                   parameters.Add("IncludeZeroCauseValues", "1")
                                                                                                   parameters.Add("IncludeEntriesWithoutBusinessCenter", "1")
                                                                                                   parameters.Add("Employees", "1")
                                                                                                   parameters.Add("Concepts", "1")
                                                                                                   parameters.Add("UserFields", "1")
                                                                                                   parameters.Add("BusinessCenters", "1")
                                                                                                   parameters.Add("RequestTypes", "1")
                                                                                                   parameters.Add("Causes", "1")
                                                                                                   parameters.Add("Feature", "1")
                                                                                                   parameters.Add("DSFunction", "1")
                                                                                                   parameters.Add("SendEmail", "1")
                                                                                                   parameters.Add("OverwriteResults", "false")
                                                                                                   parameters.Add("DownloadBI", "true")
                                                                                                   task.Parameters = parameters
                                                                                                   parameters.Add("GeniusViewName", geniusViewName)
                                                                                                   Return True
                                                                                               End Function
    End Sub

    Public Sub Save()
        Robotics.VTBase.Extensions.VTLiveTasks.Fakes.ShimroLiveTask.AllInstances.SaveroBaseConnectionBoolean = Function(task, connection, save)
                                                                                                                   Return True
                                                                                                               End Function
    End Sub

End Class
