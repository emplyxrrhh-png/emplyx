Namespace Business


    Public Class clsEmployeePunch


        Public Enum eTypeReader
            Nul = 0
            Card = 1
            Bio = 2
            Keyboard = 3
        End Enum

        Private _Data As String
        Private _Reader As eTypeReader
        Private _IDEmployee As Integer = 0
        Private _EmployeePhoto As String
        Private _PunchTime As DateTime
        Private _Incidence As Integer
        Private _IncidenceName As Integer
        Private _Name As String
        Private _Action As String
        Private _PunchID As Long
        Private _Photo As Byte()
        Private _AccessGroup As Integer
        Private _Language As String = ""

        Public Property PunchID() As Long
            Get
                Return _PunchID
            End Get
            Set(ByVal value As Long)
                _PunchID = value
            End Set
        End Property

        Public Property Name() As String
            Get
                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property


        Public Property EmployeePhoto() As String
            Get
                Return _EmployeePhoto
            End Get
            Set(ByVal value As String)
                _EmployeePhoto = value
            End Set
        End Property
        Public Property Data() As String
            Get
                Return _Data
            End Get
            Set(ByVal value As String)
                _Data = value
            End Set
        End Property

        Public Property Reader() As eTypeReader
            Get
                Return _Reader
            End Get
            Set(ByVal value As eTypeReader)
                _Reader = value
            End Set
        End Property

        Public Property IDEmployee() As Integer
            Get
                Return _IDEmployee
            End Get
            Set(ByVal value As Integer)
                _IDEmployee = value
            End Set
        End Property

        Public Property PunchTime() As DateTime
            Get
                Return _PunchTime
            End Get
            Set(ByVal value As DateTime)
                _PunchTime = value
            End Set
        End Property

        Public Property Action() As String
            Get
                Return _Action
            End Get
            Set(ByVal value As String)
                _Action = value
            End Set
        End Property

        Public Property Incidence() As Integer
            Get
                Return _Incidence
            End Get
            Set(ByVal value As Integer)
                _Incidence = value
            End Set
        End Property

        Public Property AccessGroup() As Integer
            Get
                Return _AccessGroup
            End Get
            Set(ByVal value As Integer)
                _AccessGroup = value
            End Set
        End Property

        Public Property Language() As String
            Get
                Return _Language
            End Get
            Set(ByVal value As String)
                _Language = value
            End Set
        End Property

        Public Property Photo() As Byte()
            Get
                If _Photo Is Nothing Then
                    Return Array.CreateInstance(GetType(Byte), 0)
                Else
                    Return _Photo
                End If
            End Get
            Set(ByVal value As Byte())
                _Photo = value
            End Set
        End Property

        Public Sub New()
            _Data = ""
            _Reader = 0
            _PunchTime = Now
            _Action = "X"

        End Sub

        Public Sub New(ByVal Data As String, ByVal Reader As eTypeReader)

            _Data = Data
            _Reader = Reader
            _PunchTime = Now
            _Action = "X"

        End Sub

        Public ReadOnly Property HasPhoto() As Boolean
            Get
                Return _Photo.Length > 2
            End Get
        End Property

    End Class

End Namespace