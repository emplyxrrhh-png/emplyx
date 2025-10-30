Imports System.Runtime.Serialization

Namespace DTOs

    <DataContract()>
    Public Class TaskUserField
        <DataMember()>
        Public UserFieldId As Integer
        <DataMember()>
        Public Name As String
        <DataMember()>
        Public Used As Boolean
        <DataMember()>
        Public ValueType As Integer
        <DataMember()>
        Public ValuesList As String()
    End Class

    <DataContract()>
    Public Class TaskUserFields
        <DataMember()>
        Public Fields As TaskUserField()
        <DataMember()>
        Public Status As Long
    End Class

End Namespace