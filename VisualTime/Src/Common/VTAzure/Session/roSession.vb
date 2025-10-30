Namespace Session

    Public Enum SessionError
        roNoError
        roMaxConcurrentReached
        roPassportAlreadyConnected
        roPassportRequired
    End Enum

    Public Class roSession


        Private strID As String
        Private strLocation As String
        Private intIDPassport As Integer
        Private bolIsRegistered As Boolean
        Private xAlive As Date

        Public Sub New(ByVal _Location As String, ByVal _IDPassport As Integer, ByVal _IsRegistered As Boolean)
            Me.strID = roSupport.GetSmallID(6)
            Me.strLocation = _Location
            Me.intIDPassport = _IDPassport
            Me.bolIsRegistered = _IsRegistered
            Me.xAlive = Now
        End Sub

        Public ReadOnly Property ID() As String
            Get
                Return Me.strID
            End Get
        End Property
        Public ReadOnly Property Location() As String
            Get
                Return Me.strLocation
            End Get
        End Property
        Public ReadOnly Property IDPassport() As Integer
            Get
                Return Me.intIDPassport
            End Get
        End Property
        Public ReadOnly Property IsRegistered() As Boolean
            Get
                Return Me.bolIsRegistered
            End Get
        End Property

        Public Property Alive() As Date
            Get
                Return Me.xAlive
            End Get
            Set(ByVal value As Date)
                Me.xAlive = value
            End Set
        End Property

    End Class

End Namespace