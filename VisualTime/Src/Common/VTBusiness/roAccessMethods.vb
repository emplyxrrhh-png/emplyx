Public Class roAccessMethods

#Region "Declarations"

    Private intEmployeeID As Integer
    Private intBiometricID As Nullable(Of Integer)
    Private bolAllowBiometric As Boolean
    Private bolAllowCards As Boolean
    Private bolAllowCardPlusBio As Boolean
    Private bolAllowBioPriority As Boolean
    Private bolAllowWeb As Boolean
    Private strWebLogin As String
    Private strWebPassword As String
    Private strWebWindowsSession As String
    Private bolWebIntegratedSecurity As Boolean

#End Region

#Region "Properties"

    Public Property EmployeeID() As Integer
        Get
            Return intEmployeeID
        End Get
        Set(ByVal value As Integer)
            intEmployeeID = value
        End Set
    End Property

    Public Property BiometricID() As Integer
        Get
            Return intBiometricID
        End Get
        Set(ByVal value As Integer)
            intBiometricID = value
        End Set
    End Property

    Public Property AllowBiometric() As Boolean
        Get
            Return bolAllowBiometric
        End Get
        Set(ByVal value As Boolean)
            bolAllowBiometric = value
        End Set
    End Property

    Public Property AllowCards() As Boolean
        Get
            Return bolAllowCards
        End Get
        Set(ByVal value As Boolean)
            bolAllowCards = value
        End Set
    End Property

    Public Property AllowCardPlusBio() As Boolean
        Get
            Return bolAllowCardPlusBio
        End Get
        Set(ByVal value As Boolean)
            bolAllowCardPlusBio = False
        End Set
    End Property

    Public Property AllowBioPriority() As Boolean
        Get
            Return bolAllowBioPriority
        End Get
        Set(ByVal value As Boolean)
            bolAllowBioPriority = value
        End Set
    End Property

    Public Property AllowWeb() As Boolean
        Get
            Return bolAllowWeb
        End Get
        Set(ByVal value As Boolean)
            bolAllowWeb = value
        End Set
    End Property

    Public Property WebLogin() As String
        Get
            Return strWebLogin
        End Get
        Set(ByVal value As String)
            strWebLogin = value
        End Set
    End Property

    Public Property WebPassword() As String
        Get
            Return strWebPassword
        End Get
        Set(ByVal value As String)
            strWebPassword = value
        End Set
    End Property

    Public Property WebWindowsSession() As String
        Get
            Return strWebWindowsSession
        End Get
        Set(ByVal value As String)
            strWebWindowsSession = value
        End Set
    End Property

    Public Property WebIntegratedSecurity() As Boolean
        Get
            Return bolWebIntegratedSecurity
        End Get
        Set(ByVal value As Boolean)
            bolWebIntegratedSecurity = value
        End Set
    End Property

#End Region

End Class