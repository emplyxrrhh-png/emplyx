Namespace BusinessLogicLayer

    Public Class Document

        Private _IDEmployee As Integer
        Private _IDReader As Byte
        Private _Name As String
        Private _Company As String
        Private _BeginDate As DateTime
        Private _EndDate As DateTime
        Private _DenyAccess As Boolean

        Public Property IDEmployee() As Integer
            Get
                Return _IDEmployee
            End Get
            Set(ByVal Value As Integer)
                _IDEmployee = Value
            End Set
        End Property

        Public Property IDReader() As Byte
            Get
                Return _IDReader
            End Get
            Set(ByVal Value As Byte)
                _IDReader = Value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal Value As String)
                _Name = Value
            End Set
        End Property

        Public Property Company() As String
            Get
                Return _Company
            End Get
            Set(ByVal Value As String)
                _Company = Value
            End Set
        End Property

        Public Property BeginDate() As DateTime
            Get
                Return _BeginDate
            End Get
            Set(ByVal Value As DateTime)
                _BeginDate = Value
            End Set
        End Property

        Public Property EndDate() As DateTime
            Get
                Return _EndDate
            End Get
            Set(ByVal Value As DateTime)
                _EndDate = Value
            End Set
        End Property

        Public Property DenyAccess() As Boolean
            Get
                Return _DenyAccess
            End Get
            Set(ByVal Value As Boolean)
                _DenyAccess = Value
            End Set
        End Property

        Public Overrides Function ToString() As String
            Return _IDEmployee.ToString + ";" + _IDReader.ToString + ";" + _Name.ToString + ";" + _Company.ToString + ";" + _BeginDate.ToString + ";" + _EndDate.ToString + ";" + _DenyAccess.ToString
        End Function

    End Class

End Namespace