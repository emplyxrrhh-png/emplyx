Imports System.Runtime.Serialization

Namespace VTVisits

    <DataContract>
    Public Class roJSON_response
        Private sResult As String = "NoError"
        <DataMember()>
        Public Property result As String
            Get
                Return sResult
            End Get
            Set(value As String)
                sResult = value
            End Set
        End Property

    End Class

    <DataContract>
    Public Class roJSON_Integer
        Inherits roJSON_response
        Private ivalue As Integer = -1

        <DataMember()>
        Public Property value As Integer
            Get
                Return ivalue
            End Get
            Set(value As Integer)
                ivalue = value
            End Set
        End Property
    End Class

End Namespace