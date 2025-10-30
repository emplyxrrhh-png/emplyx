Namespace BusinessLogicLayer

    Public Class GroupEmployees

        Private _EmployeeID As Integer

        Private _GroupID As Byte
        Private _NewEmployee As Boolean

        Property EmployeeID() As Integer
            Get
                Return _EmployeeID
            End Get
            Set(ByVal value As Integer)
                _EmployeeID = value
            End Set
        End Property

        Property GroupID() As Byte
            Get
                Return _GroupID
            End Get
            Set(ByVal value As Byte)
                _GroupID = value
            End Set
        End Property

        Public Property NewEmployee() As String
            Get
                Return Me._NewEmployee
            End Get
            Set(ByVal value As String)
                Me._NewEmployee = value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return _EmployeeID.ToString.PadLeft(5, "0") + BCGlobal.KeyDBField + _GroupID.ToString.PadLeft(5, "0")
        End Function

    End Class

End Namespace