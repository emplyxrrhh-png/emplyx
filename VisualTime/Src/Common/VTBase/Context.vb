Public Class Context
    ' Clase que incluye las variables de conexto del aplicativo

    Private intIDPassport As Integer
    Private strPassportName As String
    Private intIDEmployee As Integer
    Private intIDGroup As Integer
    Private datBeginDate As DateTime
    Private datEndDate As DateTime

    Public Property IDPassport() As Integer
        Get
            Return intIDPassport
        End Get
        Set(ByVal value As Integer)
            intIDPassport = value
        End Set
    End Property

    Public Property PassportName() As String
        Get
            Return strPassportName
        End Get
        Set(ByVal value As String)
            strPassportName = value
        End Set
    End Property

    Public Property IDEmployee() As Integer
        Get
            Return intIDEmployee
        End Get
        Set(ByVal value As Integer)
            intIDEmployee = value
        End Set
    End Property

    Public Property IDGroup() As Integer
        Get
            Return intIDGroup
        End Get
        Set(ByVal value As Integer)
            intIDGroup = value
        End Set
    End Property

    Public Property BeginDate() As DateTime
        Get
            Return datBeginDate
        End Get
        Set(ByVal value As DateTime)
            datBeginDate = value
        End Set
    End Property

    Public Property EndDate() As DateTime
        Get
            Return datEndDate
        End Get
        Set(ByVal value As DateTime)
            datEndDate = value
        End Set
    End Property

End Class