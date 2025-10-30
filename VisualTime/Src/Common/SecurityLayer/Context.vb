Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs

<DataContract>
Public Class CContext

#Region "Declarations - Constructor"

    Private lstEmployees As ArrayList
    Private lstGroups As ArrayList
    Private lstEmployeeNames As ArrayList
    Private lstGroupNames As ArrayList

    Private strDatalinkMode As String

    Private oPeriodType As ContextPeriodTypes
    Private oSummaryType As SummaryType
    Private xBeginDate As DateTime
    Private xEndDate As DateTime

    Private strMenuGroup As String
    Private strMenuItem As String

    Private strPassword As String

    Private strConfXml As String

    Public Sub New()

        Me.lstEmployees = New ArrayList
        Me.lstGroups = New ArrayList

        Me.strDatalinkMode = String.Empty

        ' Semana actual
        Me.PeriodType = ContextPeriodTypes.CurrentWeek
        Me.SummaryType = SummaryType.Contrato
        'Dim intDayWeek As Integer = IIf(Now.DayOfWeek = DayOfWeek.Sunday, 7, Now.DayOfWeek) - 1
        'Me.xBeginDate = Now.AddDays(-intDayWeek)
        'Me.xEndDate = Me.xBeginDate.AddDays(6)

        Me.strMenuGroup = ""
        Me.strMenuItem = ""

    End Sub

#End Region

#Region "Properties"

    <DataMember>
    Public Property Employees() As ArrayList
        Get
            Return Me.lstEmployees
        End Get
        Set(ByVal value As ArrayList)
            Me.lstEmployees = value
        End Set
    End Property

    <DataMember>
    Public Property Groups() As ArrayList
        Get
            Return Me.lstGroups
        End Get
        Set(ByVal value As ArrayList)
            Me.lstGroups = value
        End Set
    End Property

    <DataMember>
    Public Property EmployeeNames() As ArrayList
        Get
            Return Me.lstEmployeeNames
        End Get
        Set(ByVal value As ArrayList)
            Me.lstEmployeeNames = value
        End Set
    End Property

    <DataMember>
    Public Property GroupNames() As ArrayList
        Get
            Return Me.lstGroupNames
        End Get
        Set(ByVal value As ArrayList)
            Me.lstGroupNames = value
        End Set
    End Property

    <DataMember>
    Public Property BeginDate() As DateTime
        Get
            Return Me.xBeginDate
        End Get
        Set(ByVal value As DateTime)
            Me.xBeginDate = value
        End Set
    End Property

    <DataMember>
    Public Property EndDate() As DateTime
        Get
            Return Me.xEndDate
        End Get
        Set(ByVal value As DateTime)
            Me.xEndDate = value
        End Set
    End Property

    <DataMember>
    Public Property DatalinkMode() As String
        Get
            Return Me.strDatalinkMode
        End Get
        Set(ByVal value As String)
            Me.strDatalinkMode = value
        End Set
    End Property

    <DataMember>
    Public Property MenuGroup() As String
        Get
            Return Me.strMenuGroup
        End Get
        Set(ByVal value As String)
            Me.strMenuGroup = value
        End Set
    End Property

    <DataMember>
    Public Property MenuItem() As String
        Get
            Return Me.strMenuItem
        End Get
        Set(ByVal value As String)
            Me.strMenuItem = value
        End Set
    End Property

    <DataMember>
    Public Property Password() As String
        Get
            Return Me.strPassword
        End Get
        Set(ByVal value As String)
            Me.strPassword = value
        End Set
    End Property

    <DataMember>
    Public Property PeriodType() As ContextPeriodTypes
        Get
            Return Me.oPeriodType
        End Get
        Set(ByVal value As ContextPeriodTypes)
            Me.oPeriodType = value
            Me.CalculateDatesPeriod()
        End Set
    End Property

    <DataMember>
    Public Property ConfXml() As String
        Get
            Return Me.strConfXml
        End Get
        Set(ByVal value As String)
            Me.strConfXml = value
        End Set
    End Property

    <DataMember>
    Public Property SummaryType() As SummaryType
        Get
            Return Me.oSummaryType
        End Get
        Set(ByVal value As SummaryType)
            Me.oSummaryType = value
        End Set
    End Property

#End Region

#Region "Methods"

    Public Sub CalculateDatesPeriod()

        Select Case Me.oPeriodType
            Case ContextPeriodTypes.Today
                Me.xBeginDate = Now
                Me.xEndDate = Now
            Case ContextPeriodTypes.Yesterday
                Me.xBeginDate = Now.AddDays(-1)
                Me.xEndDate = Now.AddDays(-1)
            Case ContextPeriodTypes.CurrentWeek
                Me.xBeginDate = Now.AddDays(1 - Weekday(Now, vbMonday))
                Me.xEndDate = Now.AddDays(7 - Weekday(Now, vbMonday))
            Case ContextPeriodTypes.LastWeek
                Me.xBeginDate = Now.AddDays(-6 - Weekday(Now, vbMonday))
                Me.xEndDate = Now.AddDays(-Weekday(Now, vbMonday))
            Case ContextPeriodTypes.CurrentMonth
                Me.xBeginDate = New Date(Now.Year, Now.Month, 1)
                Me.xEndDate = New Date(Now.Year, Now.Month, (New Date(Now.Year, Now.Month, 1)).AddMonths(1).AddDays(-1).Day)
            Case ContextPeriodTypes.LastMonth
                Dim dtMonth As Date
                dtMonth = Now.AddMonths(-1)
                Me.xBeginDate = New Date(dtMonth.Year, dtMonth.Month, 1)
                Me.xEndDate = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
            Case ContextPeriodTypes.CurrentYear
                Me.xBeginDate = New Date(Now.Year, 1, 1)
                Me.xEndDate = Now
            Case ContextPeriodTypes.Other

        End Select

    End Sub

#End Region

End Class