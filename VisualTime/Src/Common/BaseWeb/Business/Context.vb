Imports Robotics.Base.DTOs
Imports Robotics.Security

<Serializable()>
Public Class WebCContext

#Region "Declarations - Constructor"

    Private oContext As CContext

    Private bolExcludeState As Boolean
    Private intIDPassport As Integer
    Private strAppContext As String
    'Private oHttpRequest As System.Web.HttpRequest

    Private strConfXml As String

    Public Sub New()

        Me.intIDPassport = -1
        Me.strAppContext = String.Empty
        Me.bolExcludeState = False

        Me.oContext = New CContext
        Me.oContext.Employees = New ArrayList({-1}.ToArray)
        Me.oContext.EmployeeNames = New ArrayList({-1}.ToArray)
        Me.oContext.Groups = New ArrayList({-1}.ToArray)
        Me.oContext.GroupNames = New ArrayList({-1}.ToArray)

        ' Semana actual
        Dim intDayWeek As Integer = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek) - 1
        Me.oContext.BeginDate = Now.AddDays(-intDayWeek)
        Me.oContext.EndDate = Me.oContext.BeginDate.AddDays(6)

        Me.strConfXml = String.Empty

    End Sub

    Public Sub New(ByVal _IDPassport As Integer, ByVal _AppContext As String, Optional ByVal excludeState As Boolean = False)

        Me.intIDPassport = _IDPassport
        Me.strAppContext = _AppContext
        Me.bolExcludeState = excludeState
        Me.Load()

    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property IDPassport() As Integer
        Get
            Return Me.intIDPassport
        End Get
    End Property

    Public ReadOnly Property AppContext() As String
        Get
            Return Me.strAppContext
        End Get
    End Property

    Public Property Employees() As ArrayList
        Get
            Return Me.oContext.Employees
        End Get
        Set(ByVal value As ArrayList)
            Me.oContext.Employees = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property Groups() As ArrayList
        Get
            Return Me.oContext.Groups
        End Get
        Set(ByVal value As ArrayList)
            Me.oContext.Groups = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property EmployeeNames() As ArrayList
        Get
            Return Me.oContext.EmployeeNames
        End Get
        Set(ByVal value As ArrayList)
            Me.oContext.EmployeeNames = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property GroupNames() As ArrayList
        Get
            Return Me.oContext.GroupNames
        End Get
        Set(ByVal value As ArrayList)
            Me.oContext.GroupNames = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property BeginDate() As DateTime
        Get
            Return Me.oContext.BeginDate
        End Get
        Set(ByVal value As DateTime)
            Me.oContext.BeginDate = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property EndDate() As DateTime
        Get
            Return Me.oContext.EndDate
        End Get
        Set(ByVal value As DateTime)
            Me.oContext.EndDate = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property MenuGroup() As String
        Get
            Return Me.oContext.MenuGroup
        End Get
        Set(ByVal value As String)
            Me.oContext.MenuGroup = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property MenuItem() As String
        Get
            Return Me.oContext.MenuItem
        End Get
        Set(ByVal value As String)
            Me.oContext.MenuItem = value
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property Password() As String
        Get
            Dim strPwd As String = String.Empty
            If Me.oContext.Password IsNot Nothing Then
                'strPwd = CryptographyHelper.Decrypt(Me.oContext.Password)
                strPwd = Me.oContext.Password
            End If
            If strPwd = String.Empty Then strPwd = " "
            Return strPwd
        End Get
        Set(ByVal value As String)
            Dim strPwdEnc As String = value
            If Me.oContext.Password Is Nothing OrElse strPwdEnc <> Me.oContext.Password Then
                Me.oContext.Password = strPwdEnc
                API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
            End If
        End Set
    End Property

    Public Property PeriodType() As ContextPeriodTypes
        Get
            Return Me.oContext.PeriodType
        End Get
        Set(ByVal value As ContextPeriodTypes)
            Me.oContext.PeriodType = value
            Me.CalculateDatesPeriod()
            API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

    Public Property ConfXml() As String
        Get
            Return Me.strConfXml
        End Get
        Set(ByVal value As String)
            Me.strConfXml = value
        End Set
    End Property

    Public Property SummaryType() As String
        Get
            Return Me.oContext.SummaryType
        End Get
        Set(ByVal value As String)
            Dim oldValue As String = Me.oContext.SummaryType
            Me.oContext.SummaryType = value
            If oldValue <> value Then API.SecurityServiceMethods.SetContext(Nothing, Me.intIDPassport, Me.oContext)
        End Set
    End Property

#End Region

#Region "Methods"

    Public Sub Load()
        Me.oContext = API.SecurityServiceMethods.GetContext(Nothing, Me.intIDPassport, Me.bolExcludeState)
    End Sub

    Public Sub CalculateDatesPeriod()

        Select Case Me.PeriodType
            Case ContextPeriodTypes.Today
                Me.BeginDate = Now
                Me.EndDate = Now
            Case ContextPeriodTypes.Yesterday
                Me.BeginDate = Now.AddDays(-1)
                Me.EndDate = Now.AddDays(-1)
            Case ContextPeriodTypes.CurrentWeek
                Me.BeginDate = Now.AddDays(1 - Weekday(Now, vbMonday))
                Me.EndDate = Now.AddDays(7 - Weekday(Now, vbMonday))
            Case ContextPeriodTypes.LastWeek
                Me.BeginDate = Now.AddDays(-6 - Weekday(Now, vbMonday))
                Me.EndDate = Now.AddDays(-Weekday(Now, vbMonday))
            Case ContextPeriodTypes.CurrentMonth
                Me.BeginDate = New Date(Now.Year, Now.Month, 1)
                Me.EndDate = New Date(Now.Year, Now.Month, (New Date(Now.Year, Now.Month, 1)).AddMonths(1).AddDays(-1).Day)
            Case ContextPeriodTypes.LastMonth
                Dim dtMonth As Date
                dtMonth = Now.AddMonths(-1)
                Me.BeginDate = New Date(dtMonth.Year, dtMonth.Month, 1)
                Me.EndDate = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
            Case ContextPeriodTypes.CurrentYear
                Me.BeginDate = New Date(Now.Year, 1, 1)
                Me.EndDate = Now
            Case ContextPeriodTypes.Other

        End Select

    End Sub

#End Region

End Class