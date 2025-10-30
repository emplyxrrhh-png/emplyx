Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum SeletorType
        <EnumMember()> TreeState = 0
        <EnumMember()> Universal = 1
    End Enum

    <DataContract>
    Public Enum SelectionSource
        <EnumMember()> Employee = 0
        <EnumMember()> Group = 1
        <EnumMember()> EmployeeOrGroup = 2
        <EnumMember()> Collective = 3
        <EnumMember()> Agreement = 4
    End Enum

    Public Class roUniversalSelector

        <DataMember>
        Public Property Filter As roSelectorFilter
        <DataMember>
        Public Property SelectorType As SeletorType
        <DataMember>
        Public Property TemporalTableName As String
        <DataMember>
        Public Property AllEmployeeSelected As Boolean
        <DataMember>
        Public Property NoEmployeeSelected As Boolean
        <DataMember>
        Public Property Employees As List(Of roSelectedEmployee)

        Public Sub New()
            Filter = New roSelectorFilter
            SelectorType = SeletorType.Universal
            TemporalTableName = String.Empty
            AllEmployeeSelected = False
            NoEmployeeSelected = False
            Employees = Nothing
        End Sub
    End Class

    Public Class roSelectedEmployee
        <DataMember>
        Public Property IdEmployee As Integer
        <DataMember>
        Public Property EmployeeName As String
        <DataMember>
        Public Property Group As String
        <DataMember>
        Public Property SelectionSource As SelectionSource

    End Class

    Public Class roSelectorFilter
        <DataMember>
        Public Property UserFields As String
        <DataMember>
        Public Property ComposeFilter As String
        <DataMember>
        Public Property ComposeMode As String
        <DataMember>
        Public Property Filters As String
        <DataMember>
        Public Property Operation As String

        Public Sub New()
            UserFields = String.Empty
            ComposeFilter = String.Empty
            ComposeMode = String.Empty
            Filters = String.Empty
            Operation = String.Empty
        End Sub
    End Class

    Public Class roSelectorFilterContext
        <DataMember>
        Public Property IdPassport As Integer
        <DataMember>
        Public Property Feature As String
        <DataMember>
        Public Property FeatureType As String
        <DataMember>
        Public Property RequieredPermission As Nullable(Of Permission)
        <DataMember>
        Public Property AddOnlyDirectEmployees As Boolean
        <DataMember>
        Public Property DateInf As Nullable(Of Date)
        <DataMember>
        Public Property DateSup As Nullable(Of Date)

        Public Sub New()
            Feature = String.Empty
            FeatureType = "U"
            RequieredPermission = Nothing
            AddOnlyDirectEmployees = False
            DateInf = Nothing
            DateSup = Nothing
        End Sub
    End Class

End Namespace