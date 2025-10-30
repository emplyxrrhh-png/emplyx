Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum roConnectorParameterType
        <EnumMember> Unknown = 0
        <EnumMember> Field = 1
        <EnumMember> DBField = 2
        <EnumMember> Dictionary = 3
    End Enum

    <DataContract>
    Public Class roConnectorParameter

        Public Sub New()
            Type = roConnectorParameterType.Unknown
        End Sub

        <DataMember>
        Public Property Type As roConnectorParameterType
        <DataMember>
        Public Property SplitBy As String
        <DataMember>
        Public Property SplitPosition As Integer
        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property Value As String
        <DataMember>
        Public Property Validate As String
        <DataMember>
        Public Property Condition As String
        <DataMember>
        Public Property CalculatedValue As String
        <DataMember>
        Public Property BeginPos As String
        <DataMember>
        Public Property Len As String

    End Class

End Namespace