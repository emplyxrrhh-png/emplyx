Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Microsoft.Win32
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.Extensions.VTLiveTasks

<DataContract()>
Public Class roConnector

    Public Shared Function InitTask(ByVal Task As TasksType, Optional ByVal oParamsAux As roCollection = Nothing) As String

        Select Case Task
            Case TasksType.BROADCASTER, TasksType.BROADCASTER_ONLINE
                Return ProcessBroadcasterTask(Task, oParamsAux)

            Case TasksType.DAILYINCIDENCES, TasksType.DAILYCAUSES, TasksType.DAILYSCHEDULE,
                 TasksType.MOVES, TasksType.ENTRIES, TasksType.PROGRAMMEDABSENCES, TasksType.TASKS, TasksType.INCIDENCES
                Return ProcessEngineTask(Task, roLiveTaskTypes.RunEngine, oParamsAux)

            Case TasksType.SHIFTS, TasksType.CONCEPTS, TasksType.CAUSES, TasksType.LABAGREES
                Return ProcessEngineTask(Task, roLiveTaskTypes.UpdateEngineCache, oParamsAux)
        End Select

        Return String.Empty
    End Function

    Private Shared Function ProcessEngineTask(Task As TasksType, ByVal taskType As roLiveTaskTypes, ByRef oParamsAux As roCollection) As String
        If oParamsAux Is Nothing OrElse oParamsAux.Count = 0 Then
            oParamsAux = New roCollection
            oParamsAux.Add("TaskType", Task.ToString)
        Else
            If oParamsAux.Exists("TaskType") Then
                oParamsAux("TaskType") = Task.ToString
            Else
                oParamsAux.Add("TaskType", Task.ToString)
            End If
        End If

        Return RunLiveTask(taskType, Task, oParamsAux)
    End Function

    Private Shared Function ProcessBroadcasterTask(Task As TasksType, ByRef oParamsAux As roCollection) As String
        ' Miro si debo ejecutar únicamente BC a media noche o los forzados desde pantalla de terminales
        Dim bSkip As Boolean = False
        'Dim strSQLAUX As String = "@SELECT# value from sysroLiveAdvancedParameters where ParameterName = 'VisualTimeServer.EnabledTriggers'"
        Try
            Dim sEnabled As String = String.Empty
            sEnabled = roLiveTask.getAdvancedParameterValue("VisualTimeServer.EnabledTriggers")
            If sEnabled = "0" Then
                bSkip = Not (oParamsAux IsNot Nothing AndAlso ((oParamsAux.Index("Command") > 0 AndAlso oParamsAux("Command") = "RESET_TERMINAL") OrElse (oParamsAux.Index("ON_CHANGE_MANUALLY") > 0 AndAlso oParamsAux("ON_CHANGE_MANUALLY") = "True")))
            End If
        Catch ex As Exception
        End Try

        If Not bSkip Then
            ' Si no informaron parámetros, creo colección vacía porque sino no se guarda la LiveTask !!!
            If oParamsAux Is Nothing OrElse oParamsAux.Count = 0 Then
                oParamsAux = New roCollection
                oParamsAux.Add("AllTerminals", "true")
            End If

            Return RunLiveTask(roLiveTaskTypes.BroadcasterTask, Task, oParamsAux)
        Else
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "ProcessBroadcasterTask::Skipping task")
            Return String.Empty
        End If
    End Function

    Public Shared Function RunLiveTask(ByVal taskType As roLiveTaskTypes, ByVal Task As TasksType, Optional ByVal oParamsAux As roCollection = Nothing) As String
        Dim taskId As Integer = roLiveTask.CreateLiveTask(taskType, oParamsAux, New roLiveTaskState())

        Return taskId
    End Function

    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

End Class

Public Class roBroadcasterDriver
    Private Shared _Instance As roBroadcasterDriver = Nothing
    Private Shared ReadOnly _Sync As New Object
    Private mThread As Threading.Thread
    Private Shared iSleepSeconds As Integer = 30

    Private Sub New()
    End Sub

    Public Shared ReadOnly Property Instance(iSeconds As Integer) As roBroadcasterDriver
        Get
            If _Instance Is Nothing Then
                SyncLock _Sync
                    If _Instance Is Nothing Then
                        _Instance = New roBroadcasterDriver()
                    End If
                End SyncLock
            End If
            If iSeconds > 5 Then iSleepSeconds = iSeconds
            Return _Instance
        End Get
    End Property

    Public Sub Start()
        If Not mThread Is Nothing Then
            mThread.Abort()
        End If
        mThread = New Threading.Thread(AddressOf CallBroadcaster)
        mThread.Start()
    End Sub

    Public Sub CallBroadcaster()
        Try
            Threading.Thread.Sleep(iSleepSeconds * 1000)
            roConnector.InitTask(TasksType.BROADCASTER_ONLINE)
        Catch ex As Exception
        End Try
    End Sub

End Class