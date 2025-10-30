Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class roCalendar
    Inherits UserControlBase

#Region "Class helpers"

    <Runtime.Serialization.DataContract()>
    Private Class CallbackAdvCopyRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="initialBeginDate")>
        Public initialBeginDate As Date

        <Runtime.Serialization.DataMember(Name:="initialEndDate")>
        Public initialEndDate As Date

        <Runtime.Serialization.DataMember(Name:="pasteStartDate")>
        Public pasteStartDate As Date

        <Runtime.Serialization.DataMember(Name:="idEmployee")>
        Public idEmployee As Integer

        <Runtime.Serialization.DataMember(Name:="destEmployee")>
        Public destEmployee As Integer

        <Runtime.Serialization.DataMember(Name:="copyWorkingShifts")>
        Public copyWorkingShifts As Integer

        <Runtime.Serialization.DataMember(Name:="copyHolidaysShifts")>
        Public copyHolidaysShifts As Integer

        <Runtime.Serialization.DataMember(Name:="copyAssignmentsShifts")>
        Public copyAssignmentsShifts As Integer

        <Runtime.Serialization.DataMember(Name:="idSecurityNode")>
        Public idSecurityNode As Integer

        <Runtime.Serialization.DataMember(Name:="idProductiveUnit")>
        Public idProductiveUnit As Integer

        <Runtime.Serialization.DataMember(Name:="copyEmployees")>
        Public copyEmployees As Boolean

        <Runtime.Serialization.DataMember(Name:="filters")>
        Public filters As AdvCopyFilters

        <Runtime.Serialization.DataMember(Name:="idOrgChartNode")>
        Public idOrgChartNode As Integer

        <Runtime.Serialization.DataMember(Name:="pUnitFilter")>
        Public pUnitFilter As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AdvCopyFilters

        <Runtime.Serialization.DataMember(Name:="RepeatMode")>
        Public RepeatMode As Integer

        <Runtime.Serialization.DataMember(Name:="RepeatModeValue")>
        Public RepeatModeValue As String

        <Runtime.Serialization.DataMember(Name:="RepeatStartMode")>
        Public RepeatStartMode As Integer

        <Runtime.Serialization.DataMember(Name:="RepeatStartModeValue")>
        Public RepeatStartModeValue As String

        <Runtime.Serialization.DataMember(Name:="RepeatSkipMode")>
        Public RepeatSkipMode As Integer

        <Runtime.Serialization.DataMember(Name:="RepeatSkipTimes")>
        Public RepeatSkipTimes As Integer

        <Runtime.Serialization.DataMember(Name:="RepeatSkipModeValue")>
        Public RepeatSkipModeValue As String

        <Runtime.Serialization.DataMember(Name:="LockDestDays")>
        Public LockDestDays As Integer

        <Runtime.Serialization.DataMember(Name:="HolidaysMode")>
        Public HolidaysMode As Integer

        <Runtime.Serialization.DataMember(Name:="BloquedMode")>
        Public BloquedMode As Integer

        <Runtime.Serialization.DataMember(Name:="TelecommuteCopy")>
        Public TelecommuteCopy As Integer

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class CallbackCalendarRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="firstDate")>
        Public firstDate As Date

        <Runtime.Serialization.DataMember(Name:="endDate")>
        Public endDate As Date

        <Runtime.Serialization.DataMember(Name:="employeeFilter")>
        Public employeeFilter As String

        <Runtime.Serialization.DataMember(Name:="loadRecursive")>
        Public loadRecursive As Boolean

        <Runtime.Serialization.DataMember(Name:="loadIndictments")>
        Public loadIndictments As Boolean

        <Runtime.Serialization.DataMember(Name:="loadCapacities")>
        Public loadCapacities As Boolean

        <Runtime.Serialization.DataMember(Name:="loadPunches")>
        Public loadPunches As Boolean

        <Runtime.Serialization.DataMember(Name:="typeView")>
        Public typeView As Integer

        <Runtime.Serialization.DataMember(Name:="idEmployee")>
        Public idEmployee As Integer

        <Runtime.Serialization.DataMember(Name:="idGroup")>
        Public idGroup As Integer

        <Runtime.Serialization.DataMember(Name:="idShift")>
        Public idShift As Integer

        <Runtime.Serialization.DataMember(Name:="calendar")>
        Public oCalendar As Robotics.Base.DTOs.roCalendar

        <Runtime.Serialization.DataMember(Name:="shiftData")>
        Public oShiftData As Robotics.Base.DTOs.roCalendarRowShiftData

        <Runtime.Serialization.DataMember(Name:="copyMainShifts")>
        Public bCopyMainShifts As Boolean

        <Runtime.Serialization.DataMember(Name:="copyHolidays")>
        Public bCopyHolidays As Boolean

        <Runtime.Serialization.DataMember(Name:="excelType")>
        Public bExcelType As Boolean

        <Runtime.Serialization.DataMember(Name:="keepHolidays")>
        Public bKeepHolidays As Boolean

        <Runtime.Serialization.DataMember(Name:="keepBloqued")>
        Public bKeepBloqued As Boolean

        <Runtime.Serialization.DataMember(Name:="assignmentsFilter")>
        Public assignmentsFilter As String

        <Runtime.Serialization.DataMember(Name:="copyBeginDate")>
        Public copyBeginDate As Date

        <Runtime.Serialization.DataMember(Name:="copyEndDate")>
        Public copyEndDate As Date

        <Runtime.Serialization.DataMember(Name:="pasteBegindDate")>
        Public pasteBegindDate As Date

        <Runtime.Serialization.DataMember(Name:="pasteEndDate")>
        Public pasteEndDate As Date

        <Runtime.Serialization.DataMember(Name:="remarksConfig")>
        Public remarksConfig As String()

        <Runtime.Serialization.DataMember(Name:="dailyPeriod")>
        Public dailyPeriod As Integer

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class CallbackCoverageCopyRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="IDGroup")>
        Public IDGroup As Integer

        <Runtime.Serialization.DataMember(Name:="copyBeginDate")>
        Public copyBeginDate As Date

        <Runtime.Serialization.DataMember(Name:="copyEndDate")>
        Public copyEndDate As Date

        <Runtime.Serialization.DataMember(Name:="pasteBegindDate")>
        Public pasteBegindDate As Date

        <Runtime.Serialization.DataMember(Name:="pasteEndDate")>
        Public pasteEndDate As Date

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class CallbackBudgetRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="firstDate")>
        Public firstDate As Date

        <Runtime.Serialization.DataMember(Name:="endDate")>
        Public endDate As Date

        <Runtime.Serialization.DataMember(Name:="orgChartFilter")>
        Public orgChartFilter As String

        <Runtime.Serialization.DataMember(Name:="pUnitFilter")>
        Public pUnitFilter As String

        <Runtime.Serialization.DataMember(Name:="typeView")>
        Public typeView As Integer

        <Runtime.Serialization.DataMember(Name:="loadIndictments")>
        Public loadIndictments As Boolean

        <Runtime.Serialization.DataMember(Name:="loadCapacities")>
        Public loadCapacities As Boolean

        <Runtime.Serialization.DataMember(Name:="budget")>
        Public budget As roBudget

        <Runtime.Serialization.DataMember(Name:="pUnitModePosition")>
        Public pUnitModePosition As roProductiveUnitModePosition

        <Runtime.Serialization.DataMember(Name:="employeeData")>
        Public employeeData As roProductiveUnitModePositionEmployeeData()

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class CallbackDayRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="sDate")>
        Public sDate As Date

        <Runtime.Serialization.DataMember(Name:="idOrgChartNode")>
        Public idOrgChartNode As Integer

        <Runtime.Serialization.DataMember(Name:="pUnitModePosition")>
        Public pUnitModePosition As roProductiveUnitModePosition

        <Runtime.Serialization.DataMember(Name:="employeeData")>
        Public employeeData As roProductiveUnitModePositionEmployeeData()

        <Runtime.Serialization.DataMember(Name:="employees")>
        Public employees As roBudgetEmployeeAvailableForPosition()

        <Runtime.Serialization.DataMember(Name:="status")>
        Public status As roCurrentStatusEmployeesSummary

        <Runtime.Serialization.DataMember(Name:="quantity")>
        Public quantity As Integer

        <Runtime.Serialization.DataMember(Name:="iPUnit")>
        Public iPUnit As Integer

        <Runtime.Serialization.DataMember(Name:="iPUnitMode")>
        Public iPUnitMode As Integer

    End Class

    Public Enum roCalendarWorkMode
        roCalendar = 0
        roProductiveUnit = 1
        roBudget = 2
        roDayDetail = 3
    End Enum

#End Region

#Region "Properties"

    Public Property WorkMode As roCalendarWorkMode
        Get
            If ViewState("roCalendarWorkMode") Is Nothing Then
                Return False
            Else
                Return ViewState("roCalendarWorkMode")
            End If
        End Get
        Set(value As roCalendarWorkMode)
            ViewState("roCalendarWorkMode") = value
        End Set
    End Property

    Public Property ClientInstanceName As String

        Get
            If ViewState("roClientInstanceName") Is Nothing Then
                Return False
            Else
                Return ViewState("roClientInstanceName")
            End If
        End Get
        Set(value As String)
            ViewState("roClientInstanceName") = value
        End Set
    End Property

    Public Property complementary_EndCallback As String
        Get
            If ViewState("complementary_EndCallback") Is Nothing Then
                Return False
            Else
                Return ViewState("complementary_EndCallback")
            End If
        End Get
        Set(value As String)
            ViewState("complementary_EndCallback") = value
        End Set
    End Property

    Public Property assignments_EndCallback As String
        Get
            If ViewState("assignments_EndCallback") Is Nothing Then
                Return False
            Else
                Return ViewState("assignments_EndCallback")
            End If
        End Get
        Set(value As String)
            ViewState("assignments_EndCallback") = value
        End Set
    End Property

    Public Property performAction_EndCallback As String
        Get
            If ViewState("performAction_EndCallback") Is Nothing Then
                Return False
            Else
                Return ViewState("performAction_EndCallback")
            End If
        End Get
        Set(value As String)
            ViewState("performAction_EndCallback") = value
        End Set
    End Property

    Public Property roCalendar_EndCallback As String
        Get
            If ViewState("roCalendar_EndCallback") Is Nothing Then
                Return False
            Else
                Return ViewState("roCalendar_EndCallback")
            End If
        End Get
        Set(value As String)
            ViewState("roCalendar_EndCallback") = value
        End Set
    End Property

    Public Property Feature As String
        Get
            If ViewState("Feature") Is Nothing Then
                Return "Calendar"
            Else
                Return ViewState("Feature")
            End If
        End Get
        Set(value As String)
            ViewState("Feature") = value
        End Set
    End Property

    Public Property Height As String
        Get
            If ViewState("Height") Is Nothing Then
                Return "100%"
            Else
                Return ViewState("Height")
            End If
        End Get
        Set(value As String)
            ViewState("Height") = value
        End Set
    End Property

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("Calendar_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("Calendar_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("Calendar_iCurrentTask") = value
        End Set
    End Property

    Private Property ErrorExists As Boolean
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorExists")
            If val IsNot Nothing Then
                Return roTypes.Any2Boolean(val)
            Else
                HttpContext.Current.Session("ErrorExists") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("ErrorExists") = value
        End Set
    End Property

    Private Property ErrorDescription As String
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorDescription")
            If val IsNot Nothing Then
                Return roTypes.Any2String(val)
            Else
                HttpContext.Current.Session("ErrorDescription") = False
                Return False
            End If
        End Get
        Set(value As String)
            HttpContext.Current.Session("ErrorDescription") = value
        End Set
    End Property

    Dim oPermission As Permission = Permission.None ' Me.GetFeaturePermission("Calendar")
    Dim oPermissionPlan As Permission = Permission.None ' Me.GetFeaturePermission("Calendar.Scheduler")
    Dim oPermissionHigh As Permission = Permission.None ' Me.GetFeaturePermission("Calendar.Highlight")
    Dim oCostCenterPermission As Permission = Permission.None

#End Region

#Region "Initialize"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Try
            Me.oPermission = Me.GetFeaturePermission(Me.Feature)

            Me.oPermissionPlan = Me.GetFeaturePermission("Calendar.Scheduler")
            Me.oPermissionHigh = Me.GetFeaturePermission("Calendar.Highlight")

            If API.LicenseServiceMethods.FeatureIsInstalled("Feature\CostControl") Then
                oCostCenterPermission = Me.GetFeaturePermission("BusinessCenters.Punches")
            End If

            Me.ASPxRoCalendarCallback.ClientInstanceName = Me.ClientID & ("_ASPxRoCalendarCallbackClient")
            Me.ASPxRoCalendarCallback.ClientSideEvents.CallbackComplete = Me.roCalendar_EndCallback

            Me.PerformActionCallback.ClientInstanceName = Me.ClientID & ("_PerformActionCallbackClient")
            Me.PerformActionCallback.ClientSideEvents.CallbackComplete = Me.performAction_EndCallback

            Me.hdnCalendarConfig.ClientInstanceName = Me.ClientID & ("_hdnCalendarConfigClient")

            Me.dlgComplementary.InitCallbacks(Me.complementary_EndCallback)
            Me.dlgAssignments.InitCallbacks(Me.assignments_EndCallback)

            If Not Me.IsPostBack Then
                LoadCalendarConfig()
                Me.hdnCalendarWorkMode.Value = CInt(Me.WorkMode).ToString()
                Me.hdnClientInstanceName.Value = Me.ClientInstanceName.ToString()

                Me.dlgAdvCopy.InitLanguages(Me.WorkMode)
                Me.dlgFilterCalendar.InitControl(Me.WorkMode)
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Sub LoadCalendarConfig()
        Dim bolMultipleShiftsLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultipleShifts")
        Dim bolHRSchedulingLicense As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling")
        Dim bolSaasPremium As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")
        Dim bolTelecommuting As Boolean = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting")
        Dim sVTEdition As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VTLive.Edition"))
        Dim sMinimumCalendarCellData As String = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("MinimumCalendarDataWarning"))

        If Not hdnCalendarConfig.Contains("TelecommuteEnabled") Then hdnCalendarConfig.Add("TelecommuteEnabled", bolTelecommuting) Else hdnCalendarConfig("TelecommuteEnabled") = bolTelecommuting
        If Not hdnCalendarConfig.Contains("Language") Then hdnCalendarConfig.Add("Language", WLHelperWeb.CurrentPassport.Language.Key) Else hdnCalendarConfig("Language") = WLHelperWeb.CurrentPassport.Language.Key
        If Not hdnCalendarConfig.Contains("HRScheduling") Then hdnCalendarConfig.Add("HRScheduling", bolHRSchedulingLicense) Else hdnCalendarConfig("HRScheduling") = bolHRSchedulingLicense
        If Not hdnCalendarConfig.Contains("SaasPremium") Then hdnCalendarConfig.Add("SaasPremium", bolSaasPremium) Else hdnCalendarConfig("SaasPremium") = bolSaasPremium
        If Not hdnCalendarConfig.Contains("VTLiveEdition") Then hdnCalendarConfig.Add("VTLiveEdition", sVTEdition) Else hdnCalendarConfig("VTLiveEdition") = sVTEdition
        If Not hdnCalendarConfig.Contains("MinimumCalendarDataWarning") Then hdnCalendarConfig.Add("MinimumCalendarDataWarning", sMinimumCalendarCellData) Else hdnCalendarConfig("MinimumCalendarDataWarning") = sMinimumCalendarCellData

        If Not hdnCalendarConfig.Contains("Language.Empty") Then hdnCalendarConfig.Add("Language.Empty", Me.Language.Translate("Calendar.Empty", Me.DefaultScope)) Else hdnCalendarConfig("Language.Empty") = Me.Language.Translate("Calendar.Empty", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Accept") Then hdnCalendarConfig.Add("Language.Accept", Me.Language.Translate("Calendar.Accept", Me.DefaultScope)) Else hdnCalendarConfig("Language.Accept") = Me.Language.Translate("Calendar.Accept", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Cancel") Then hdnCalendarConfig.Add("Language.Cancel", Me.Language.Translate("Calendar.Cancel", Me.DefaultScope)) Else hdnCalendarConfig("Language.Cancel") = Me.Language.Translate("Calendar.Cancel", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.NoSolection") Then hdnCalendarConfig.Add("Language.NoSolection", Me.Language.Translate("Calendar.NoSolection", Me.DefaultScope)) Else hdnCalendarConfig("Language.NoSolection") = Me.Language.Translate("Calendar.NoSolection", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Employee") Then hdnCalendarConfig.Add("Language.Employee", Me.Language.Translate("Calendar.Employee", Me.DefaultScope)) Else hdnCalendarConfig("Language.Employee") = Me.Language.Translate("Calendar.Employee", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Employees") Then hdnCalendarConfig.Add("Language.Employees", Me.Language.Translate("Calendar.Employees", Me.DefaultScope)) Else hdnCalendarConfig("Language.Employees") = Me.Language.Translate("Calendar.Employees", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Department") Then hdnCalendarConfig.Add("Language.Department", Me.Language.Translate("Calendar.Department", Me.DefaultScope)) Else hdnCalendarConfig("Language.Department") = Me.Language.Translate("Calendar.Department", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Departments") Then hdnCalendarConfig.Add("Language.Departments", Me.Language.Translate("Calendar.Departments", Me.DefaultScope)) Else hdnCalendarConfig("Language.Departments") = Me.Language.Translate("Calendar.Departments", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Connector") Then hdnCalendarConfig.Add("Language.Connector", Me.Language.Translate("Calendar.Connector", Me.DefaultScope)) Else hdnCalendarConfig("Language.Connector") = Me.Language.Translate("Calendar.Connector", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.PlannedHours") Then hdnCalendarConfig.Add("Language.PlannedHours", Me.Language.Translate("Calendar.PlannedHours", Me.DefaultScope)) Else hdnCalendarConfig("Language.PlannedHours") = Me.Language.Translate("Calendar.PlannedHours", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Holidays") Then hdnCalendarConfig.Add("Language.Holidays", Me.Language.Translate("Calendar.Holidays", Me.DefaultScope)) Else hdnCalendarConfig("Language.Holidays") = Me.Language.Translate("Calendar.Holidays", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.Mandatory") Then hdnCalendarConfig.Add("Language.Mandatory", Me.Language.Translate("Calendar.Mandatory", Me.DefaultScope)) Else hdnCalendarConfig("Language.Mandatory") = Me.Language.Translate("Calendar.Mandatory", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Flexible") Then hdnCalendarConfig.Add("Language.Flexible", Me.Language.Translate("Calendar.Flexible", Me.DefaultScope)) Else hdnCalendarConfig("Language.Flexible") = Me.Language.Translate("Calendar.Flexible", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Complementary") Then hdnCalendarConfig.Add("Language.Complementary", Me.Language.Translate("Calendar.Complementary", Me.DefaultScope)) Else hdnCalendarConfig("Language.Complementary") = Me.Language.Translate("Calendar.Complementary", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.ContextMenu_Details") Then hdnCalendarConfig.Add("Language.ContextMenu_Details", Me.Language.Translate("Calendar.ContextMenu_Details", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_Details") = Me.Language.Translate("Calendar.ContextMenu_Details", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_RemoveHolidays") Then hdnCalendarConfig.Add("Language.ContextMenu_RemoveHolidays", Me.Language.Translate("Calendar.ContextMenu_RemoveHolidays", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_RemoveHolidays") = Me.Language.Translate("Calendar.ContextMenu_RemoveHolidays", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_Copy") Then hdnCalendarConfig.Add("Language.ContextMenu_Copy", Me.Language.Translate("Calendar.ContextMenu_Copy", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_Copy") = Me.Language.Translate("Calendar.ContextMenu_Copy", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_Paste") Then hdnCalendarConfig.Add("Language.ContextMenu_Paste", Me.Language.Translate("Calendar.ContextMenu_Paste", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_Paste") = Me.Language.Translate("Calendar.ContextMenu_Paste", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_Block") Then hdnCalendarConfig.Add("Language.ContextMenu_Block", Me.Language.Translate("Calendar.ContextMenu_Block", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_Block") = Me.Language.Translate("Calendar.ContextMenu_Block", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_UnBlock") Then hdnCalendarConfig.Add("Language.ContextMenu_UnBlock", Me.Language.Translate("Calendar.ContextMenu_UnBlock", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_UnBlock") = Me.Language.Translate("Calendar.ContextMenu_UnBlock", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_CancelSelection") Then hdnCalendarConfig.Add("Language.ContextMenu_CancelSelection", Me.Language.Translate("Calendar.ContextMenu_CancelSelection", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_CancelSelection") = Me.Language.Translate("Calendar.ContextMenu_CancelSelection", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_EditComplementary") Then hdnCalendarConfig.Add("Language.ContextMenu_EditComplementary", Me.Language.Translate("Calendar.ContextMenu_EditComplementary", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_EditComplementary") = Me.Language.Translate("Calendar.ContextMenu_EditComplementary", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_AdvPaste") Then hdnCalendarConfig.Add("Language.ContextMenu_AdvPaste", Me.Language.Translate("Calendar.ContextMenu_AdvPaste", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_AdvPaste") = Me.Language.Translate("Calendar.ContextMenu_AdvPaste", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_CopyHolidays") Then hdnCalendarConfig.Add("Language.ContextMenu_CopyHolidays", Me.Language.Translate("Calendar.ContextMenu_CopyHolidays", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_CopyHolidays") = Me.Language.Translate("Calendar.ContextMenu_CopyHolidays", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_CopyWorking") Then hdnCalendarConfig.Add("Language.ContextMenu_CopyWorking", Me.Language.Translate("Calendar.ContextMenu_CopyWorking", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_CopyWorking") = Me.Language.Translate("Calendar.ContextMenu_CopyWorking", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_CopyAssignments") Then hdnCalendarConfig.Add("Language.ContextMenu_CopyAssignments", Me.Language.Translate("Calendar.ContextMenu_CopyAssignments", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_CopyAssignments") = Me.Language.Translate("Calendar.ContextMenu_CopyAssignments", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_EditAssignments") Then hdnCalendarConfig.Add("Language.ContextMenu_EditAssignments", Me.Language.Translate("Calendar.ContextMenu_EditAssignments", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_EditAssignments") = Me.Language.Translate("Calendar.ContextMenu_EditAssignments", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_SetCoverage") Then hdnCalendarConfig.Add("Language.ContextMenu_SetCoverage", Me.Language.Translate("Calendar.ContextMenu_SetCoverage", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_SetCoverage") = Me.Language.Translate("Calendar.ContextMenu_SetCoverage", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_PlanifyCoverage") Then hdnCalendarConfig.Add("Language.ContextMenu_PlanifyCoverage", Me.Language.Translate("Calendar.ContextMenu_PlanifyCoverage", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_PlanifyCoverage") = Me.Language.Translate("Calendar.ContextMenu_PlanifyCoverage", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_Sort") Then hdnCalendarConfig.Add("Language.ContextMenu_Sort", Me.Language.Translate("Calendar.ContextMenu_Sort", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_Sort") = Me.Language.Translate("Calendar.ContextMenu_Sort", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_SetFeast") Then hdnCalendarConfig.Add("Language.ContextMenu_SetFeast", Me.Language.Translate("Calendar.ContextMenu_SetFeast", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_SetFeast") = Me.Language.Translate("Calendar.ContextMenu_SetFeast", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.ContextMenu_RemoveFeast") Then hdnCalendarConfig.Add("Language.ContextMenu_RemoveFeast", Me.Language.Translate("Calendar.ContextMenu_RemoveFeast", Me.DefaultScope)) Else hdnCalendarConfig("Language.ContextMenu_RemoveFeast") = Me.Language.Translate("Calendar.ContextMenu_RemoveFeast", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.Download") Then hdnCalendarConfig.Add("Language.Download", Me.Language.Translate("Calendar.Download", Me.DefaultScope)) Else hdnCalendarConfig("Language.Download") = Me.Language.Translate("Calendar.Download", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.Tooltip_plannedHoliday") Then hdnCalendarConfig.Add("Language.Tooltip_plannedHoliday", Me.Language.Translate("Calendar.Tooltip_plannedHoliday", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_plannedHoliday") = Me.Language.Translate("Calendar.Tooltip_plannedHoliday", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_absent") Then hdnCalendarConfig.Add("Language.Tooltip_absent", Me.Language.Translate("Calendar.Tooltip_absent", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_absent") = Me.Language.Translate("Calendar.Tooltip_absent", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_holidays") Then hdnCalendarConfig.Add("Language.Tooltip_holidays", Me.Language.Translate("Calendar.Tooltip_holidays", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_holidays") = Me.Language.Translate("Calendar.Tooltip_holidays", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_absence") Then hdnCalendarConfig.Add("Language.Tooltip_absence", Me.Language.Translate("Calendar.Tooltip_absence", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_absence") = Me.Language.Translate("Calendar.Tooltip_absence", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_incidence") Then hdnCalendarConfig.Add("Language.Tooltip_incidence", Me.Language.Translate("Calendar.Tooltip_incidence", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_incidence") = Me.Language.Translate("Calendar.Tooltip_incidence", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_notes") Then hdnCalendarConfig.Add("Language.Tooltip_notes", Me.Language.Translate("Calendar.Tooltip_notes", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_notes") = Me.Language.Translate("Calendar.Tooltip_notes", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_overtime") Then hdnCalendarConfig.Add("Language.Tooltip_overtime", Me.Language.Translate("Calendar.Tooltip_overtime", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_overtime") = Me.Language.Translate("Calendar.Tooltip_overtime", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Tooltip_feast") Then hdnCalendarConfig.Add("Language.Tooltip_feast", Me.Language.Translate("Calendar.Tooltip_feast", Me.DefaultScope)) Else hdnCalendarConfig("Language.Tooltip_feast") = Me.Language.Translate("Calendar.Tooltip_feast", Me.DefaultScope)

        Dim oRemarks As Robotics.Base.VTBusiness.Scheduler.roSchedulerRemarks = API.SchedulerServiceMethods.GetSchedulerRemarksConfig(Me.Page, WLHelperWeb.CurrentPassportID)

        Dim remark1Text As String = String.Empty
        Dim remark2Text As String = String.Empty
        Dim remark3Text As String = String.Empty

        If oRemarks.Remarks.Count >= 1 Then
            remark1Text = CausesServiceMethods.GetCauseByID(Me.Page, oRemarks.Remarks(0).IDCause, False).Name
        End If

        If oRemarks.Remarks.Count >= 2 Then
            remark2Text = CausesServiceMethods.GetCauseByID(Me.Page, oRemarks.Remarks(1).IDCause, False).Name
        End If

        If oRemarks.Remarks.Count >= 3 Then
            remark3Text = CausesServiceMethods.GetCauseByID(Me.Page, oRemarks.Remarks(2).IDCause, False).Name
        End If

        If Not hdnCalendarConfig.Contains("Language.Tooltip_remark1") Then hdnCalendarConfig.Add("Language.Tooltip_remark1", remark1Text) Else hdnCalendarConfig("Language.Tooltip_remark1") = remark1Text
        If Not hdnCalendarConfig.Contains("Language.Tooltip_remark2") Then hdnCalendarConfig.Add("Language.Tooltip_remark2", remark2Text) Else hdnCalendarConfig("Language.Tooltip_remark2") = remark2Text
        If Not hdnCalendarConfig.Contains("Language.Tooltip_remark3") Then hdnCalendarConfig.Add("Language.Tooltip_remark3", remark3Text) Else hdnCalendarConfig("Language.Tooltip_remark3") = remark3Text

        If Not hdnCalendarConfig.Contains("Language.Scheduler_Planified") Then hdnCalendarConfig.Add("Language.Scheduler_Planified", Me.Language.Translate("Calendar.Scheduler_Planified", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Planified") = Me.Language.Translate("Calendar.Scheduler_Planified", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Scheduler_Real") Then hdnCalendarConfig.Add("Language.Scheduler_Real", Me.Language.Translate("Calendar.Scheduler_Real", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Real") = Me.Language.Translate("Calendar.Scheduler_Real", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Scheduler_Recursive") Then hdnCalendarConfig.Add("Language.Scheduler_Recursive", Me.Language.Translate("Calendar.Scheduler_Recursive", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Recursive") = Me.Language.Translate("Calendar.Scheduler_Recursive", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Scheduler_NonRecursive") Then hdnCalendarConfig.Add("Language.Scheduler_NonRecursive", Me.Language.Translate("Calendar.Scheduler_NonRecursive", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_NonRecursive") = Me.Language.Translate("Calendar.Scheduler_NonRecursive", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Scheduler_Filter") Then hdnCalendarConfig.Add("Language.Scheduler_Filter", Me.Language.Translate("Calendar.Scheduler_Filter", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Filter") = Me.Language.Translate("Calendar.Scheduler_Filter", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.Empty_Assignment") Then hdnCalendarConfig.Add("Language.Empty_Assignment", Me.Language.Translate("Calendar.Empty_Assignment", Me.DefaultScope)) Else hdnCalendarConfig("Language.Empty_Assignment") = Me.Language.Translate("Calendar.Empty_Assignment", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Empty_Assignment_SN") Then hdnCalendarConfig.Add("Language.Empty_Assignment_SN", Me.Language.Translate("Calendar.Empty_Assignment_SN", Me.DefaultScope)) Else hdnCalendarConfig("Language.Empty_Assignment_SN") = Me.Language.Translate("Calendar.Empty_Assignment_SN", Me.DefaultScope)

        If Not hdnCalendarConfig.Contains("Language.Scheduler_Concepts") Then hdnCalendarConfig.Add("Language.Scheduler_Concepts", Me.Language.Translate("Calendar.Scheduler_Concepts", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Concepts") = Me.Language.Translate("Calendar.Scheduler_Concepts", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Scheduler_Shifts") Then hdnCalendarConfig.Add("Language.Scheduler_Shifts", Me.Language.Translate("Calendar.Scheduler_Shifts", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Shifts") = Me.Language.Translate("Calendar.Scheduler_Shifts", Me.DefaultScope)
        If Not hdnCalendarConfig.Contains("Language.Scheduler_Assignments") Then hdnCalendarConfig.Add("Language.Scheduler_Assignments", Me.Language.Translate("Calendar.Scheduler_Assignments", Me.DefaultScope)) Else hdnCalendarConfig("Language.Scheduler_Assignments") = Me.Language.Translate("Calendar.Scheduler_Assignments", Me.DefaultScope)
    End Sub

    Private Sub roCalendar_Init(sender As Object, e As EventArgs) Handles Me.Init
        IsScriptManagerInParent()
    End Sub

    Public Function IsScriptManagerInParent() As Boolean
        Dim lRet As Boolean = False
        If Me.Parent.Page.ClientScript Is Nothing Then Return False

        Dim cacheManager As New Robotics.Web.Base.NoCachePageBase

        cacheManager.InsertJavascriptIncludes(Me.Parent.Page)

        cacheManager.InsertExtraJavascript("contextMenu", "~/Base/Scripts/jquery.contextMenu.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("jquery.fixedheadertable", "~/Base/jquerylayout/jquery.fixedheadertable.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("selectorUtils", "~/Base/Scripts/Live/utils.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roCalendar", "~/Base/WebuserControls/roCalendar/Scripts/roCalendar.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("moment", "~/Base/Scripts/moment.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("momenttz", "~/Base/Scripts/moment-tz.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("roDate", "~/Base/Scripts/Live/roDateManager.min.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("jquery.daterangepicker", "~/Base/Scripts/jquery.daterangepicker.js", Me.Parent.Page)

        'cacheManager.InsertExtraJavascript("CalendarV2", "~/Scheduler/Scripts/CalendarV2.js", Me.Parent.Page)
        'cacheManager.InsertExtraJavascript("DateSelector", "~/Scheduler/Scripts/DateSelector.js", Me.Parent.Page)

        cacheManager.InsertCssIncludes(Me.Parent.Page)

        cacheManager.InsertExtraCssIncludes("~/Base/WebuserControls/roCalendar/Styles/roCalendar.css", Me.Parent.Page)
        cacheManager.InsertExtraCssIncludes("~/Base/jquerylayout/layout-default-latest.css", Me.Parent.Page)
        cacheManager.InsertExtraCssIncludes("~/Base/jquerylayout/jquery.fixedheadertable.css", Me.Parent.Page)
        cacheManager.InsertExtraCssIncludes("~/Base/WebuserControls/roCalendar/Scripts/roContextMenu/jquery.contextMenu.min.css", Me.Parent.Page)
        Return True
    End Function

#End Region

    Protected Sub ASPxRoCalendarCallback_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxRoCalendarCallback.Callback
        Select Case Me.WorkMode
            Case roCalendarWorkMode.roBudget
                ParseBudgetRequest(sender, e)
            Case roCalendarWorkMode.roCalendar
                ParseCalendarRequest(sender, e)
            Case roCalendarWorkMode.roProductiveUnit
                ParseCalendarRequest(sender, e)
            Case roCalendarWorkMode.roDayDetail
                ParseDayDetailRequest(sender, e)
        End Select
    End Sub

    Protected Sub ParseDayDetailRequest(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase)
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As CallbackDayRequest = Nothing

        oParameters = New CallbackDayRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "GETAVAILABLEPOSITIONEMPLOYEES", "GETAVAILABLEPOSITIONEMPLOYEESFORFULFILL"
                bRet = LoadAvailableEmployees(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.employees))
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResultParams", roJSONHelper.SerializeNewtonSoft(oParameters.status))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "ADDEMPLOYEEPLANONPOSITION"
                bRet = AddEmployeePlanOnPosition(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(bRet))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "REMOVEEMPLOYEEFROMPOSITION"
                bRet = RemoveEmployeeFromPosition(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(bRet))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "UPDATEPRODUCTIVEUNITQUANTITY"
                bRet = UpdateProductiveUnitQuantity(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", bRet)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
        End Select

    End Sub

    Protected Sub ParseBudgetRequest(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase)
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As CallbackBudgetRequest = Nothing

        oParameters = New CallbackBudgetRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "GETAVAILABLEPOSITIONEMPLOYEES", "ADDEMPLOYEEPLANONPOSITION", "REMOVEEMPLOYEEFROMPOSITION"
                Dim oFuncParameters As New CallbackDayRequest()
                oFuncParameters.idOrgChartNode = oParameters.orgChartFilter
                oFuncParameters.sDate = oParameters.firstDate
                oFuncParameters.pUnitModePosition = oParameters.pUnitModePosition
                oFuncParameters.employeeData = oParameters.employeeData

                Select Case oParameters.Action
                    Case "GETAVAILABLEPOSITIONEMPLOYEES"
                        bRet = LoadAvailableEmployees(oFuncParameters, responseMessage)
                        If Not bRet Then
                            ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                            ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                        Else
                            ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oFuncParameters.employees))
                            ASPxRoCalendarCallback.JSProperties.Add("cpObjResultParams", roJSONHelper.SerializeNewtonSoft(oFuncParameters.status))
                            ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                        End If
                    Case "ADDEMPLOYEEPLANONPOSITION"
                        bRet = AddEmployeePlanOnPosition(oFuncParameters, responseMessage)
                        If Not bRet Then
                            ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                            ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                        Else
                            ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(bRet))
                            ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                        End If
                    Case "REMOVEEMPLOYEEFROMPOSITION"
                        bRet = RemoveEmployeeFromPosition(oFuncParameters, responseMessage)
                        If Not bRet Then
                            ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                            ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                        Else
                            ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(bRet))
                            ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                        End If
                End Select

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "LOADAVAILABLEEMPLOYEESDETAIL"
                Dim availableEmployees = LoadEmployeeAvailableForNode(oParameters, responseMessage)
                If availableEmployees.Length = 0 Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(availableEmployees))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "LOADBUDGET", "DISCARDBUDGETANDCONTINUE"
                Dim lstPUnits As New Generic.List(Of roProductiveUnit)

                bRet = LoadBudget(oParameters, lstPUnits, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.budget))
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResultParams", roJSONHelper.SerializeNewtonSoft(lstPUnits.ToArray))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "SAVEBUDGETCHANGES", "SAVEBUDGETCHANGESANDCONTINUE"
                Dim saveResult As roBudgetResult = SaveBudget(oParameters, responseMessage)
                If saveResult.Status = BudgetStatusEnum.KO Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(saveResult))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.budget))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "DELETEBUDGETROW"
                Dim bDelete As Boolean = DeleteBudgetRow(oParameters, responseMessage)
                If Not bDelete Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", True)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "GETNEWBUDGETROW"
                bRet = LoadBudgetNewRow(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.budget))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "GETBUDGETHOURPERIODDEFINITION"
                bRet = GetBudgetHourPeriodDefinition(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.budget))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
                'Case "RUNAIPLANNER"
                '    Dim saveResult As roBudgetResult = RunAIPlanner(oParameters, responseMessage)
                '    If saveResult.Status = BudgetStatusEnum.KO Then
                '        ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                '        ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(saveResult))
                '        ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                '    Else
                '        ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.budget))
                '        ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                '    End If
                '    ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
                'Case "CLEANAIPLANNER"
                '    Dim saveResult As roBudgetResult = CleanAIPlanner(oParameters, responseMessage)
                '    If saveResult.Status = BudgetStatusEnum.KO Then
                '        ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                '        ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(saveResult))
                '        ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                '    Else
                '        ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.budget))
                '        ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                '    End If
                '    ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
        End Select

    End Sub

    Protected Sub ParseCalendarRequest(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase)
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As CallbackCalendarRequest
        oParameters = New CallbackCalendarRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Dim configHolidayShiftType As String = String.Empty

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "COPYCOVERAGES"
                bRet = CopyCoverages(oParameters, oPermissionPlan, oPermissionHigh, oCostCenterPermission, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    LoadCoverages(oParameters, oPermissionPlan, oPermissionHigh, oCostCenterPermission, responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.oCalendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "LOADCOVERAGES"
                bRet = LoadCoverages(oParameters, oPermissionPlan, oPermissionHigh, oCostCenterPermission, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.oCalendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "LOADCALENDAR", "DISCARDANDCONTINUE"
                bRet = LoadCalendar(oParameters, oPermissionPlan, oPermissionHigh, oCostCenterPermission, responseMessage, configHolidayShiftType)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.oCalendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResultParams", roJSONHelper.SerializeNewtonSoft(oParameters.remarksConfig))
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResultConfigShiftType", configHolidayShiftType)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "CHECKCALENDARINDITMENTS"
                Dim checkResult As Robotics.Base.DTOs.roCalendarIndictmentResult = CheckCalendarInditments(oParameters, responseMessage)
                If checkResult.Status = CalendarStatusEnum.KO Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(checkResult.Calendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "SAVECALENDAR", "SAVEANDCONTINUE"
                Dim saveResult As Robotics.Base.DTOs.roCalendarResult = SaveCalendar(oParameters, responseMessage)
                If saveResult.Status = CalendarStatusEnum.KO Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(saveResult))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.oCalendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If

                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "LOADHOURDATA"
                Dim dayHourDescription = GetDayHourDescription(oParameters, responseMessage)
                If responseMessage <> String.Empty Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(dayHourDescription))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "LOADDAYDEFINITION"
                Dim dayDescription = GetDayDescription(oParameters, responseMessage)
                If responseMessage <> String.Empty Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(dayDescription))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "SHIFTLAYERDEFINITION", "SHIFTLAYERDEFINITIONEDIT"
                Dim dayDescription = GetShiftLayersDefinition(oParameters, responseMessage)
                If responseMessage <> String.Empty Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(dayDescription))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "POSITIONMODEDAYDATA", "UPDATECURRENTDAYDATA"
                Dim dayDescription = GetTheoricShiftLayersDefinition(oParameters, responseMessage)
                If responseMessage <> String.Empty Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(dayDescription))
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)
            Case "EXPORTTOEXCEL"
                bRet = ExportSelectionToExcel(oParameters, responseMessage)
                If Not bRet Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "NOK")
                Else
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                End If
                ASPxRoCalendarCallback.JSProperties.Add("cpAction", oParameters.Action)

            Case "IMPORTFROMEXCEL"
                Dim oCalResult As Robotics.Base.DTOs.roCalendarResult = ImportFromExcel(oParameters, responseMessage)
                If oCalResult.Status = CalendarStatusEnum.OK Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.oCalendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpAction", "IMPORTFROMEXCEL")
                ElseIf oCalResult.Status = CalendarStatusEnum.KO Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oCalResult))
                    ASPxRoCalendarCallback.JSProperties.Add("cpAction", "IMPORTFROMEXCELKO")
                ElseIf oCalResult.Status = CalendarStatusEnum.WARNING Then
                    ASPxRoCalendarCallback.JSProperties.Add("cpMessage", responseMessage)
                    ASPxRoCalendarCallback.JSProperties.Add("cpResult", "OK")
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResult", roJSONHelper.SerializeNewtonSoft(oParameters.oCalendar))
                    ASPxRoCalendarCallback.JSProperties.Add("cpObjResultParams", roJSONHelper.SerializeNewtonSoft(oCalResult))
                    ASPxRoCalendarCallback.JSProperties.Add("cpAction", "IMPORTFROMEXCELWARNING")
                End If

        End Select
    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackAdvCopyRequest()
        oParameters = roJSONHelper.DeserializeNewtonSoft(strParameter, oParameters.GetType())

        Select Case oParameters.Action
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", True)
            Case "PERFORM_ACTION"
                Select Case WorkMode
                    Case roCalendarWorkMode.roBudget
                        BudgetAdvancedPaste(oParameters)
                    Case roCalendarWorkMode.roCalendar
                        CalendarAdvancedPaste(oParameters)
                End Select
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
            Case "RUNAIPLANNER"
                RunAIPlannerForBudget(True, oParameters)
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
            Case "CLEARAIPLANNER"
                RunAIPlannerForBudget(False, oParameters)
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case 0, 1
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                                PerformActionCallback.JSProperties.Add("cpActionMsg", "")
                            Case 2
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")

                                If oTask.Action.ToUpper = roLiveTaskTypes.AIPlannerTask.ToString.ToUpper Then
                                    PerformActionCallback.JSProperties.Add("cpActionResult", "OKAIPLANNER")
                                    PerformActionCallback.JSProperties.Add("cpActionMsg", "OKAIPLANNER")
                                    PerformActionCallback.JSProperties.Add("cpFileMsg", oTask.ErrorCode)
                                Else
                                    PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                                    PerformActionCallback.JSProperties.Add("cpActionMsg", "OK")
                                End If

                            Case 3
                                PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "KO")
                                PerformActionCallback.JSProperties.Add("cpActionMsg", "KO")
                                PerformActionCallback.JSProperties.Add("cpErrorMsg", oTask.ErrorCode)
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                        PerformActionCallback.JSProperties.Add("cpActionMsg", "KO")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    PerformActionCallback.JSProperties.Add("cpActionMsg", "KO")
                End If
        End Select

    End Sub

#Region "Calendar"

    Private Sub CalendarAdvancedPaste(oParameters As CallbackAdvCopyRequest)
        Dim employees As New Generic.List(Of Integer)
        employees.Add(oParameters.destEmployee)

        iCurrentTask = API.LiveTasksServiceMethods.ExecuteCalendarSpecialPaste(Me.Page, oParameters.initialBeginDate, oParameters.initialEndDate, oParameters.pasteStartDate, oParameters.idEmployee, employees,
                                                                                            oParameters.filters.RepeatMode, oParameters.filters.RepeatModeValue,
                                                                                            oParameters.filters.RepeatStartMode, oParameters.filters.RepeatStartModeValue,
                                                                                            oParameters.filters.RepeatSkipMode, oParameters.filters.RepeatSkipTimes, oParameters.filters.RepeatSkipModeValue,
                                                                                            oParameters.filters.BloquedMode, oParameters.filters.HolidaysMode, oParameters.copyWorkingShifts, oParameters.copyHolidaysShifts, oParameters.filters.LockDestDays, oParameters.copyAssignmentsShifts, oParameters.filters.TelecommuteCopy)
    End Sub

    Private Function LoadCalendar(ByRef oParameters As CallbackCalendarRequest, ByVal oPermissionPlan As Permission, ByVal oPermissionHigh As Permission, ByVal oCostCenterPermission As Permission, ByRef strMessage As String, ByRef configHolidayShiftType As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim oCalendarDetail As CalendarDetailLevel = CalendarDetailLevel.Detail_30

            Select Case oParameters.dailyPeriod
                Case 15
                    oCalendarDetail = CalendarDetailLevel.Detail_15
                Case 30
                    oCalendarDetail = CalendarDetailLevel.Detail_30
                Case 60
                    oCalendarDetail = CalendarDetailLevel.Detail_60
                Case Else
                    oCalendarDetail = CalendarDetailLevel.Detail_30
            End Select

            Dim oCalendarView As CalendarView = CalendarView.Review
            Dim bLoadPunches As Boolean = oParameters.loadPunches
            Dim bLoadSeatingCapacity As Boolean = True

            If oParameters.endDate > oParameters.firstDate Then oCalendarDetail = CalendarDetailLevel.Daily
            If oParameters.typeView = 1 Then
                oCalendarView = CalendarView.Planification
                bLoadPunches = False
            End If

            Dim oTmpCalendar As Robotics.Base.DTOs.roCalendar = CalendarServiceMethods.GetCalendar(Me.Page, oParameters.firstDate, oParameters.endDate, oParameters.employeeFilter, oParameters.typeView, oCalendarDetail, oParameters.loadRecursive, oParameters.assignmentsFilter, IIf(oCalendarView = CalendarView.Planification, True, False), oParameters.loadIndictments, bLoadPunches, oParameters.loadCapacities)

            If oTmpCalendar IsNot Nothing Then
                oParameters.oCalendar = oTmpCalendar

                Dim oCalConfig = CalendarServiceMethods.GetCalendarConfiguration(Me.Page)
                oParameters.remarksConfig = {Drawing.ColorTranslator.ToHtml(Drawing.Color.FromArgb(Drawing.ColorTranslator.FromWin32(oCalConfig.CalendarRemarks(0).Color).ToArgb())),
                    Drawing.ColorTranslator.ToHtml(Drawing.Color.FromArgb(Drawing.ColorTranslator.FromWin32(oCalConfig.CalendarRemarks(1).Color).ToArgb())),
                    Drawing.ColorTranslator.ToHtml(Drawing.Color.FromArgb(Drawing.ColorTranslator.FromWin32(oCalConfig.CalendarRemarks(2).Color).ToArgb()))
                    }
                If oCalConfig IsNot Nothing AndAlso oCalConfig.CalendarHolidays IsNot Nothing AndAlso oCalConfig.CalendarHolidays <> "" Then
                    Dim oConcept = ConceptsServiceMethods.GetConceptByShortName(Me.Page, oCalConfig.CalendarHolidays, False)
                    If oConcept IsNot Nothing Then
                        configHolidayShiftType = oConcept.DefaultQuery
                    End If
                End If

            Else
                    bRet = False
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.CalendarV2State.ErrorDetail
            End If
        Catch ex As Exception
            bRet = False
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

    Private Function LoadCoverages(ByRef oParameters As CallbackCalendarRequest, ByVal oPermissionPlan As Permission, ByVal oPermissionHigh As Permission, ByVal oCostCenterPermission As Permission, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim oCoverages As Robotics.Base.DTOs.roCalendarCoverageDay() = CalendarServiceMethods.GetCalendarCoverage(Me.Page, oParameters.firstDate, oParameters.endDate, oParameters.employeeFilter, oParameters.assignmentsFilter)

            oParameters.oCalendar = New Robotics.Base.DTOs.roCalendar
            oParameters.oCalendar.CalendarHeader = New Robotics.Base.DTOs.roCalendarHeader
            oParameters.oCalendar.CalendarHeader.PeriodCoverageData = oCoverages
        Catch ex As Exception
            bRet = False
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

    Private Function CopyCoverages(ByRef oParameters As CallbackCalendarRequest, ByVal oPermissionPlan As Permission, ByVal oPermissionHigh As Permission, ByVal oCostCenterPermission As Permission, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try
            bRet = API.SchedulerServiceMethods.CopyTeoricDailyCoverage(Me.Page, oParameters.idGroup, oParameters.copyBeginDate, oParameters.copyEndDate, oParameters.pasteBegindDate, oParameters.pasteEndDate)

            If Not bRet Then
                strMessage = API.SchedulerServiceMethods.LastErrorText
            End If
        Catch ex As Exception
            bRet = False
            strMessage = API.SchedulerServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

    Private Function CheckCalendarInditments(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarIndictmentResult
        Dim bRet As Robotics.Base.DTOs.roCalendarIndictmentResult = Nothing

        Try
            bRet = CalendarServiceMethods.AddIndictmentsToCalendar(Me.Page, oParameters.oCalendar)
            If bRet.Status = CalendarStatusEnum.KO Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText
            End If
        Catch ex As Exception
            bRet = New Robotics.Base.DTOs.roCalendarIndictmentResult
            bRet.Calendar = New Robotics.Base.DTOs.roCalendar
            bRet.Status = CalendarStatusEnum.KO
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

    Private Function SaveCalendar(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarResult
        Dim bRet As Robotics.Base.DTOs.roCalendarResult = Nothing

        Try
            bRet = CalendarServiceMethods.SaveCalendar(Me.Page, oParameters.oCalendar)
            If bRet.Status = CalendarStatusEnum.KO Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText
            End If
        Catch ex As Exception
            bRet = New Robotics.Base.DTOs.roCalendarResult
            bRet.CalendarDataResult = {}
            bRet.Status = CalendarStatusEnum.KO
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

    Private Function ExportSelectionToExcel(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim bExcel As Byte() = CalendarServiceMethods.ExportCalendarToExcel(Me.Page, oParameters.oCalendar)
            HttpContext.Current.Session("CALENDAR_EXPORT") = bExcel

            If Not (bExcel IsNot Nothing AndAlso bExcel.Length > 0) Then
                bRet = False
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText
            End If
        Catch ex As Exception
            bRet = False
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

    Private Function GetTheoricShiftLayersDefinition(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarRowHourData()
        Dim oCalendarDay() As Robotics.Base.DTOs.roCalendarRowHourData = {}

        Try
            Dim oCalendarDetail As CalendarDetailLevel = CalendarDetailLevel.Detail_30

            Select Case oParameters.dailyPeriod
                Case 15
                    oCalendarDetail = CalendarDetailLevel.Detail_15
                Case 30
                    oCalendarDetail = CalendarDetailLevel.Detail_30
                Case 60
                    oCalendarDetail = CalendarDetailLevel.Detail_60
                Case Else
                    oCalendarDetail = CalendarDetailLevel.Detail_30
            End Select

            oCalendarDay = CalendarServiceMethods.GetCalendarDayHourData(Me.Page, -1, -1, oParameters.idShift, oParameters.endDate, DateTime.Now.Date, oCalendarDetail, oParameters.oShiftData)

            If oCalendarDay.Length = 0 Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.CalendarV2State.ErrorDetail
            End If
        Catch ex As Exception
            oCalendarDay = {}
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return oCalendarDay
    End Function

    Private Function GetDayHourDescription(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarRowHourData()
        Dim oCalendarDay() As Robotics.Base.DTOs.roCalendarRowHourData = {}

        Try
            Dim oCalendarDetail As CalendarDetailLevel = CalendarDetailLevel.Detail_30

            Select Case oParameters.dailyPeriod
                Case 15
                    oCalendarDetail = CalendarDetailLevel.Detail_15
                Case 30
                    oCalendarDetail = CalendarDetailLevel.Detail_30
                Case 60
                    oCalendarDetail = CalendarDetailLevel.Detail_60
                Case Else
                    oCalendarDetail = CalendarDetailLevel.Detail_30
            End Select
            oCalendarDay = CalendarServiceMethods.GetCalendarDayHourData(Me.Page, oParameters.idEmployee, oParameters.idGroup, oParameters.idShift, oParameters.endDate, oParameters.firstDate, oCalendarDetail, oParameters.oShiftData)

            If oCalendarDay.Length = 0 Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.CalendarV2State.ErrorDetail
            End If
        Catch ex As Exception
            oCalendarDay = {}
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return oCalendarDay
    End Function

    Private Function GetShiftLayersDefinition(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarShift
        Dim oCalendarShiftDefinition As Robotics.Base.DTOs.roCalendarShift = Nothing

        Try

            oCalendarShiftDefinition = CalendarServiceMethods.GetShiftDefinition(Me.Page, oParameters.idShift)

            If oCalendarShiftDefinition Is Nothing Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.CalendarV2State.ErrorDetail
            End If
        Catch ex As Exception
            oCalendarShiftDefinition = Nothing
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return oCalendarShiftDefinition
    End Function

    Private Function GetDayDescription(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarRowDayData
        Dim oCalendarDay As Robotics.Base.DTOs.roCalendarRowDayData = Nothing

        Try
            Dim bLoadPunches As Boolean = oParameters.loadPunches
            Dim bLoadSeatingCapacity As Boolean = True

            If oParameters.typeView = 1 Then
                bLoadPunches = False
            End If

            Dim oCalendarDetail As CalendarDetailLevel = CalendarDetailLevel.Detail_30
            Select Case oParameters.dailyPeriod
                Case 15
                    oCalendarDetail = CalendarDetailLevel.Detail_15
                Case 30
                    oCalendarDetail = CalendarDetailLevel.Detail_30
                Case 60
                    oCalendarDetail = CalendarDetailLevel.Detail_60
                Case Else
                    oCalendarDetail = CalendarDetailLevel.Detail_30
            End Select

            oCalendarDay = CalendarServiceMethods.GetCalendarDayData(Me.Page, oParameters.idEmployee, oParameters.idGroup, oParameters.firstDate, oParameters.typeView, oCalendarDetail, bLoadPunches)

            If oCalendarDay Is Nothing Then
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.CalendarV2State.ErrorDetail
            End If
        Catch ex As Exception
            oCalendarDay = Nothing
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return oCalendarDay
    End Function

    Private Function ImportFromExcel(ByRef oParameters As CallbackCalendarRequest, ByRef strMessage As String) As Robotics.Base.DTOs.roCalendarResult
        Dim bRet As New Robotics.Base.DTOs.roCalendarResult

        bRet.Status = CalendarStatusEnum.OK
        Try
            Dim oFilebytes As Byte() = HttpContext.Current.Session("CALENDAR_IMPORT")

            If oFilebytes IsNot Nothing AndAlso oFilebytes.Length > 0 Then
                Dim strErrorFileName As String = String.Empty
                oParameters.oCalendar = CalendarServiceMethods.GetCalendarFromExcel(Me.Page, oFilebytes, oParameters.employeeFilter, oParameters.firstDate, oParameters.endDate, oParameters.bCopyMainShifts, oParameters.bCopyHolidays, False, oParameters.bKeepHolidays, oParameters.bKeepBloqued, strErrorFileName, bRet, oParameters.assignmentsFilter, oParameters.loadRecursive, oParameters.bExcelType)

                If bRet.Status <> CalendarStatusEnum.OK Then

                    oFilebytes = API.LiveTasksServiceMethods.DownloadFileAzure(Me.Page, strErrorFileName, roLiveQueueTypes.datalink)
                    HttpContext.Current.Session("CALENDAR_EXPORT") = oFilebytes
                End If
            Else
                bRet.Status = CalendarStatusEnum.KO
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText
            End If

            HttpContext.Current.Session("CALENDAR_IMPORT") = Nothing
        Catch ex As Exception
            bRet.Status = CalendarStatusEnum.KO
            strMessage = CalendarServiceMethods.LastErrorText
        End Try

        Return bRet
    End Function

#End Region

#Region "Budget"

    Private Sub RunAIPlannerForBudget(ByVal bUpdateSchedule As Boolean, ByVal oParameters As CallbackAdvCopyRequest)
        Dim employees As New Generic.List(Of Integer)
        employees.Add(oParameters.destEmployee)

        iCurrentTask = API.LiveTasksServiceMethods.ExecuteAIPlanner(Me.Page, bUpdateSchedule, WLHelperWeb.CurrentPassportID, oParameters.initialBeginDate, oParameters.initialEndDate, oParameters.idOrgChartNode, oParameters.pUnitFilter, True)
    End Sub

    Private Sub BudgetAdvancedPaste(oParameters As CallbackAdvCopyRequest)
        Dim employees As New Generic.List(Of Integer)
        employees.Add(oParameters.destEmployee)

        iCurrentTask = API.LiveTasksServiceMethods.ExecuteBudgetSpecialPaste(Me.Page, oParameters.initialBeginDate, oParameters.initialEndDate, oParameters.pasteStartDate, oParameters.idSecurityNode, oParameters.idProductiveUnit,
                                                                                            oParameters.filters.RepeatMode, oParameters.filters.HolidaysMode, oParameters.filters.RepeatModeValue, oParameters.copyEmployees)
    End Sub

    Private Function LoadEmployeeAvailableForNode(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As roEmployeeAvailableForNode()
        Dim bRet As roEmployeeAvailableForNode() = {}

        Try

            bRet = AISchedulingServiceMethods.GetEmployeeAvailableForNode(Me.Page, oParameters.orgChartFilter, oParameters.firstDate, oParameters.endDate)

            If roWsUserManagement.SessionObject().States.BudgetState.Result <> BudgetResultEnum.NoError Then
                strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.BudgetState.ErrorDetail
            End If
        Catch ex As Exception
            bRet = {}
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function LoadBudget(ByRef oParameters As CallbackBudgetRequest, ByRef lstPUnits As Generic.List(Of roProductiveUnit), ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim oBudgetDetail As BudgetDetailLevel = BudgetDetailLevel.Hour
            Dim oBudgetView As BudgetView = BudgetView.Definition

            Dim idPUnit As Integer = -1
            Dim pUnitFilter As String = oParameters.pUnitFilter

            If oParameters.endDate > oParameters.firstDate Then oBudgetDetail = BudgetDetailLevel.Daily
            If oParameters.typeView = 1 Then
                oBudgetView = BudgetView.Planification
                oBudgetDetail = BudgetDetailLevel.Mode
            ElseIf oParameters.typeView = 2 Then
                pUnitFilter = String.Empty
                oBudgetView = BudgetView.Planification
                oBudgetDetail = BudgetDetailLevel.Hour
                If oParameters.firstDate <> oParameters.endDate Then
                    idPUnit = roTypes.Any2Integer(oParameters.pUnitFilter)

                    lstPUnits = AISchedulingServiceMethods.GetProductiveUnitsFromNode(Me.Page, oParameters.orgChartFilter, oParameters.firstDate, oParameters.endDate)

                    If idPUnit = -1 AndAlso lstPUnits.Count > 0 Then
                        idPUnit = lstPUnits(0).ID
                    End If
                End If
            End If

            Dim oTmpBudget As roBudget = AISchedulingServiceMethods.GetBudget(Me.Page, oParameters.firstDate, oParameters.endDate, oParameters.orgChartFilter, oBudgetView, oBudgetDetail, True, idPUnit, pUnitFilter, oParameters.loadIndictments)

            If oTmpBudget IsNot Nothing Then
                oParameters.budget = oTmpBudget
            Else
                bRet = False
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.BudgetState.ErrorDetail
            End If
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function DeleteBudgetRow(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean

        Try
            bRet = AISchedulingServiceMethods.RemoveNewBudgetRow(Me.Page, roTypes.Any2Integer(oParameters.pUnitFilter), roTypes.Any2Integer(oParameters.orgChartFilter), True)
            If roWsUserManagement.SessionObject().States.BudgetState.Result <> BudgetResultEnum.NoError Then
                strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
            End If
        Catch ex As Exception
            bRet = False
            strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
        End Try

        Return bRet
    End Function

    Private Function GetBudgetHourPeriodDefinition(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim oTmpBudget As roBudget = AISchedulingServiceMethods.GetBudgetHourPeriodDefinition(Me.Page, oParameters.firstDate, oParameters.endDate, roTypes.Any2Integer(oParameters.pUnitFilter), roTypes.Any2Integer(oParameters.orgChartFilter), False, oParameters.loadIndictments)

            If oTmpBudget IsNot Nothing Then
                oParameters.budget = oTmpBudget
            Else
                bRet = False
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.BudgetState.ErrorDetail
            End If
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function LoadBudgetNewRow(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try
            Dim oTmpBudget As roBudget = AISchedulingServiceMethods.GetNewBudgetRow(Me.Page, oParameters.firstDate, oParameters.endDate, roTypes.Any2Integer(oParameters.pUnitFilter), roTypes.Any2Integer(oParameters.orgChartFilter), False)

            If oTmpBudget IsNot Nothing Then
                oParameters.budget = oTmpBudget
            Else
                bRet = False
                strMessage = roWsUserManagement.SessionObject().States.CalendarV2State.ErrorText & vbNewLine & roWsUserManagement.SessionObject().States.BudgetState.ErrorDetail
            End If
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function SaveBudget(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As roBudgetResult
        Dim bRet As roBudgetResult

        Try
            bRet = AISchedulingServiceMethods.SaveBudget(Me.Page, oParameters.budget, True)
            If bRet.Status = BudgetStatusEnum.KO Then
                strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
            End If
        Catch ex As Exception
            bRet = New roBudgetResult
            bRet.BudgetDataResult = {}
            bRet.Status = BudgetStatusEnum.KO
            strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
        End Try

        Return bRet
    End Function

    'Private Function RunAIPlanner(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As roBudgetResult
    '    Dim bRet As New roBudgetResult

    '    Try

    '        Dim oCalendarResponse As roCalendarResponse = AISchedulingServiceMethods.RunAISchedulerForBudget(Me.Page, oParameters.budget, True)
    '        If roWsUserManagement.SessionObject().States.BudgetState.Result <> BudgetResultEnum.NoError Then
    '            strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
    '            bRet.Status = BudgetStatusEnum.KO
    '            bRet.BudgetDataResult = {}
    '        Else
    '            bRet.Status = BudgetStatusEnum.OK
    '            bRet.BudgetDataResult = {}
    '            'oCalendarResponse = CalendarService.CalendarServiceMethods.SaveCalendar(Me.Page, oCalendarResponse.Calendar)
    '        End If
    '    Catch ex As Exception
    '        bRet = New roBudgetResult
    '        bRet.BudgetDataResult = {}
    '        bRet.Status = BudgetStatusEnum.KO
    '        strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
    '    End Try

    '    Return bRet
    'End Function

    'Private Function CleanAIPlanner(ByRef oParameters As CallbackBudgetRequest, ByRef strMessage As String) As roBudgetResult
    '    Dim bRet As New roBudgetResult

    '    Try
    '        Dim oCalendarResponse As roCalendarResponse = AISchedulingServiceMethods.RemoveAIScheduledForBudget(Me.Page, oParameters.budget, True)
    '        If roWsUserManagement.SessionObject().States.BudgetState.Result <> BudgetResultEnum.NoError Then
    '            strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
    '            bRet.Status = BudgetStatusEnum.KO
    '            bRet.BudgetDataResult = {}
    '        Else
    '            bRet.Status = BudgetStatusEnum.OK
    '            bRet.BudgetDataResult = {}
    '            'oCalendarResponse = CalendarService.CalendarServiceMethods.SaveCalendar(Me.Page, oCalendarResponse.Calendar)
    '        End If
    '    Catch ex As Exception
    '        bRet = New roBudgetResult
    '        bRet.BudgetDataResult = {}
    '        bRet.Status = BudgetStatusEnum.KO
    '        strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
    '    End Try

    '    Return bRet
    'End Function

#End Region

#Region "DayDetail"

    Private Function AddEmployeePlanOnPosition(ByRef oParameters As CallbackDayRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try

            bRet = AISchedulingServiceMethods.AddEmployeesPlanOnPosition(Me.Page, oParameters.pUnitModePosition, oParameters.sDate, oParameters.employeeData)

            If roWsUserManagement.SessionObject().States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                strMessage = roWsUserManagement.SessionObject().States.ProductiveUnitState.ErrorText
                bRet = False
            End If
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function UpdateProductiveUnitQuantity(ByRef oParameters As CallbackDayRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try

            Dim oBudget As roBudget = AISchedulingServiceMethods.GetBudget(Me.Page, oParameters.sDate, oParameters.sDate, oParameters.idOrgChartNode, BudgetView.Definition, BudgetDetailLevel.Daily, False, -1, "", False)
            Dim bFound As Boolean = False
            If oBudget IsNot Nothing Then

                For Each oUnit In oBudget.BudgetData
                    If oUnit.ProductiveUnitData.ProductiveUnit.ID = oParameters.iPUnit Then
                        oUnit.PeriodData.DayData(0).HasChanged = True
                        For Each oUnitModePosition In oUnit.PeriodData.DayData(0).ProductiveUnitMode.UnitModePositions
                            If oUnitModePosition.ID = oParameters.pUnitModePosition.ID Then
                                oUnit.RowState = BudgetRowState.UpdateRow
                                oUnitModePosition.Quantity = oParameters.quantity
                                bFound = True
                                Exit For
                            End If
                        Next

                        If bFound Then Exit For
                    End If
                Next

                Dim oBudgetResult As roBudgetResult = AISchedulingServiceMethods.SaveBudget(Me.Page, oBudget, True)
                bRet = (oBudgetResult.Status = BudgetStatusEnum.OK)
            End If

            If roWsUserManagement.SessionObject().States.BudgetState.Result <> BudgetResultEnum.NoError Then
                strMessage = roWsUserManagement.SessionObject().States.ProductiveUnitState.ErrorText
                bRet = False
            End If
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function RemoveEmployeeFromPosition(ByRef oParameters As CallbackDayRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try

            Dim oEmployees = AISchedulingServiceMethods.RemoveEmployeeFromPosition(Me.Page, oParameters.pUnitModePosition, oParameters.sDate, oParameters.employeeData(0))

            If roWsUserManagement.SessionObject().States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                strMessage = roWsUserManagement.SessionObject().States.ProductiveUnitState.ErrorText
                bRet = False
            End If
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

    Private Function LoadAvailableEmployees(ByRef oParameters As CallbackDayRequest, ByRef strMessage As String) As Boolean
        Dim bRet As Boolean = True

        Try

            Dim oEmployees = AISchedulingServiceMethods.GetBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse(Me.Page, oParameters.idOrgChartNode, oParameters.sDate, oParameters.pUnitModePosition)

            If roWsUserManagement.SessionObject().States.BudgetState.Result <> BudgetResultEnum.NoError Then
                strMessage = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
                Return False
            End If

            oParameters.employees = oEmployees.Employees
            oParameters.status = oEmployees.NodeStatus
        Catch ex As Exception
            bRet = False
            strMessage = AISchedulingServiceMethods.LastBudgetStateErrorText
        End Try

        Return bRet
    End Function

#End Region

End Class