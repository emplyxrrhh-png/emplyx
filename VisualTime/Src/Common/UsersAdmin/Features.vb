Namespace Business

    ''' <summary>
    ''' The list of valid features in application. Use these contants in code instead of
    ''' hard-coding aliases to facilitate maintenante and avoid mistyping.
    ''' Do not put features groups here since permissions checks should
    ''' always be made on features directly. Add an entry for application alias.
    ''' </summary>
    Public NotInheritable Class Features

        Private Sub New()
        End Sub

        Public Const UsersAdmin As String = "Administration.Security"
    End Class

End Namespace