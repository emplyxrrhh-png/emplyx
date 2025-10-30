Public Class roWsUserObject
    Private _backend As roBackend
    Private _webServicesStates As roWebServicesStates
    Private _lastAccess As DateTime
    Private _strPassportGUID As String

    Public Property LastAccess As DateTime
        Get
            Return _lastAccess
        End Get
        Set(value As DateTime)
            _lastAccess = value
        End Set
    End Property

    Public Property PassportGUID As String
        Get
            Return _strPassportGUID
        End Get
        Set(value As String)
            _strPassportGUID = value
        End Set
    End Property

    Public Property AccessApi As roBackend
        Get
            Return _backend
        End Get
        Set(value As roBackend)
            _backend = value
        End Set
    End Property

    Public Property States As roWebServicesStates
        Get
            Return _webServicesStates
        End Get
        Set(value As roWebServicesStates)
            _webServicesStates = value
        End Set
    End Property

    Public Sub New(ByVal LastAccess As DateTime, ByVal strPassportGUID As String)
        _lastAccess = LastAccess
        _strPassportGUID = strPassportGUID
        _backend = New roBackend(strPassportGUID)
        _webServicesStates = New roWebServicesStates()
    End Sub

End Class