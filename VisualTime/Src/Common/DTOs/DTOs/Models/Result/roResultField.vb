Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Class roResultField
        <DataMember>
        Public Property ID As Integer
        <DataMember>
        Public Property Name As String
        <DataMember>
        Public Property Type As UserFieldsTypes.FieldTypes

    End Class

    ''' <summary>
    ''' Representa un listado de variables
    ''' </summary>
    <DataContract>
    Public Class roResultListResponse

        Public Sub New()
            ResultFields = {}
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property ResultFields As roResultField()

        <DataMember>
        Public Property oState As roWsState
    End Class

End Namespace