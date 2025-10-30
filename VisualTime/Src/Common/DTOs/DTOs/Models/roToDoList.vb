Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    <Serializable>
    Public Enum ToDoListType
        <EnumMember> OnBoarding
        <EnumMember> Generic  'Para fase II
    End Enum

    <DataContract>
    <Serializable>
    Public Class roToDoList
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property Type As ToDoListType
        <DataMember>
        Public Property IdEmployee As Integer
        <DataMember>
        Public Property EmloyeeName As String
        <DataMember>
        Public Property EmployeeImage As Byte()
        <DataMember>
        Public Property StartDate As Date
        <DataMember>
        Public Property CreatedOn As Date
        <DataMember>
        Public Property Status As String
        <DataMember>
        Public Property EmployeeName As String
        <DataMember>
        Public Property Image As String
        <DataMember>
        Public Property Comments As String
        <DataMember>
        Public Property LastModifiedBy As String
        <DataMember>
        Public Property Tasks As roToDoTask()

        Public Sub New()
            LastModifiedBy = String.Empty
            Comments = String.Empty
            StartDate = Now.Date
            CreatedOn = Now
            Status = String.Empty
            Type = ToDoListType.OnBoarding
            Tasks = {}
            EmployeeImage = {}
        End Sub

    End Class

    Public Class roToDoTask
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property IdList As Integer
        <DataMember>
        Public Property TaskName As String
        <DataMember>
        Public Property Done As Boolean
        <DataMember>
        Public Property SupervisorName As String
        <DataMember>
        Public Property LastChangeDate As Date

        Public Sub New()
            TaskName = String.Empty
            SupervisorName = String.Empty
            LastChangeDate = Now
            Done = False
        End Sub

    End Class

    Public Class NewTaskData
        <DataMember>
        Public Property List As String
        <DataMember>
        Public Property Task As String
    End Class

    Public Class NewOnBoardingData
        <DataMember>
        Public Property Employee As String
        <DataMember>
        Public Property StartDate As String
        <DataMember>
        Public Property CopyEmp As String

    End Class

    <DataContract()>
    Public Class OnBoardingTasks

        Public Sub New()
            Status = 0
            Tasks = {}
        End Sub

        <DataMember()>
        Public Tasks As TaskInfo()
        <DataMember()>
        Public Status As Long
    End Class

    <DataContract()>
    Public Class OnBoardingList

        Public Sub New()
            Status = 0
            OnBoardings = {}
        End Sub

        <DataMember()>
        Public OnBoardings As OnBoardingInfo()
        <DataMember()>
        Public Status As Long
    End Class

    <DataContract()>
    Public Class OnBoardingInfo
        <DataMember()>
        Public IdEmployee As Integer
        <DataMember()>
        Public IdList As Integer
        <DataMember()>
        Public EmployeeName As String
        <DataMember()>
        Public Group As String
        <DataMember()>
        Public Status As String
        <DataMember()>
        Public Comments As String
        <DataMember()>
        Public Image As String
        <DataMember()>
        Public StartDate As Date
        <DataMember()>
        Public BeginContractDate As String
    End Class

    <DataContract()>
    Public Class CopyInfo
        <DataMember()>
        Public id As Integer
        <DataMember()>
        Public text As String
    End Class

    Public Class TaskInfo
        <DataMember>
        Public Property Id As Integer
        <DataMember>
        Public Property IdList As Integer
        <DataMember>
        Public Property TaskName As String
        <DataMember>
        Public Property Done As Boolean
        <DataMember>
        Public Property SupervisorName As String
        <DataMember>
        Public Property LastChangeDate As String
    End Class

End Namespace