Imports System.Runtime.Serialization

Namespace CardsTree.Model

    <DataContract>
    Public Class CardsTreeFilter

        <DataMember>
        Public Property ID As String

        <DataMember>
        Public Property Description As String

        <DataMember>
        Public Property Parent As String

        <DataMember>
        Public Property Filter As String

    End Class

End Namespace