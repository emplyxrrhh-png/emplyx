Imports System.Runtime.Serialization

Namespace CardsTree.Model

    <DataContract>
    Public Class Card

        <DataMember>
        Public Property Id As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Description As String

        <DataMember>
        Public Property CreatedOn As DateTime

        <DataMember>
        Public Property Icon As String

        <DataMember>
        Public Property IsSystem As Boolean

        <DataMember>
        Public Property Type As String

        <DataMember>
        Public Property Filterfield As String

        <DataMember>
        Public Property Filterfield2 As String

        <DataMember>
        Public Property Badge As Integer

        <DataMember>
        Public Property StatusText As String

        <DataMember>
        Public Property Status As Integer

        <DataMember>
        Public Property AllowHtml As Boolean

    End Class

End Namespace