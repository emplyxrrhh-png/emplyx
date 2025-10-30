Namespace Suprema

    Public Enum AvailableOperators
        Equal = 0
        Not_Equal = 1
        Contains = 2
        Between = 3
        [Like] = 4
        Greater = 5
        Less = 6
    End Enum

    Public Class LoginRequest
        Public Property User As Suprema.User
    End Class

    Public Class EventsRequest
        Public Property Query As Suprema.Query
    End Class

    Public Class LoginResponse
        Public Property User As Suprema.User
        Public Property Response As Suprema.Response
        Public Property httpResponseStatus As Integer

    End Class

    Public Class SearchResponse
        Public Property EventCollection As punchCollection
        Public Property Response As Suprema.Response
        Public Property httpResponseStatus As Integer
    End Class

    Public Class punchCollection
        Public Property rows As punch()
    End Class

    Public Class punchUser
        Public Property name As String
        Public Property photo_exists As Boolean
        Public Property user_id As String
    End Class

    Public Class groupUser
        Public Property id As String
        Public Property name As String
    End Class

    Public Class device
        Public Property id As String
        Public Property name As String
    End Class

    Public Class punchEvent
        Public Property code As String
    End Class

    Public Class punchTimezone
        Public Property half As String
        Public Property hour As String
        Public Property negative As String
    End Class

    Public Class punch
        Public Property id As String

        Public Property server_datetime As DateTime

        Public Property datetime As DateTime

        Public Property index As String
        Public Property user_id_name As String
        Public Property user_id As punchUser
        Public Property user_group_id As groupUser
        Public Property device_id As device
        Public Property event_type_id As punchEvent
        Public Property is_dst As String
        Public Property timezone As punchTimezone

        Public Property user_update_by_device As Boolean
        Public Property tna_key As String
        Public Property hint As String
        Public Property temperature As String

    End Class

    Public Class Response
        Public Property code As String
        Public Property link As String
        Public Property message As String

    End Class

    Public Class User
        Public Property login_id As String
        Public Property password As String

    End Class

    Public Class Query
        Public Property limit As Integer
        Public Property conditions As condition()
        Public Property orders As order()
    End Class

    Public Class condition
        Public Property column As String
        Public Property [operator] As Integer
        Public Property values As String()
    End Class

    Public Class order
        Public Property column As String
        Public Property descending As Boolean
    End Class

End Namespace