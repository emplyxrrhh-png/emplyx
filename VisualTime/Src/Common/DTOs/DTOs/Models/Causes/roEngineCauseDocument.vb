Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Class roEngineCauseDocument

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer
        <DataMember>
        Public Property IDCause() As Integer

        <DataMember>
        Public Property IDLabAgree() As Integer

        <DataMember>
        Public Property IDdocument() As Integer

        <DataMember>
        Public Property TypeRequest() As TypeRequestEnum

        <DataMember>
        Public Property NumberItems() As Integer

        <DataMember>
        Public Property NumberItems2() As Integer

        <DataMember>
        Public Property FlexibleWhen() As Integer

        <DataMember>
        Public Property FlexibleWhen2() As Integer

        <DataMember>
        Public Property LabAgreeName() As String

        <DataMember>
        Public Property DocumentName() As String

#End Region

    End Class

End Namespace