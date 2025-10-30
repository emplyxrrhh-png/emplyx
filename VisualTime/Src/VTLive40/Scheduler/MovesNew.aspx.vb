Imports System.Globalization
Imports DevExpress.Web
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Scheduler_MovesNew
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class AbsenceDetailObject

        <Runtime.Serialization.DataMember(Name:="AbsenceID")>
        Public AbsenceID As Integer

        <Runtime.Serialization.DataMember(Name:="IDEmployee")>
        Public IDEmployee As Integer

        <Runtime.Serialization.DataMember(Name:="IDCause")>
        Public IDCause As String

        <Runtime.Serialization.DataMember(Name:="Description")>
        Public Description As String

        <Runtime.Serialization.DataMember(Name:="BeginDate")>
        Public BeginDate As Date

        <Runtime.Serialization.DataMember(Name:="EndDate")>
        Public EndDate As Date

        <Runtime.Serialization.DataMember(Name:="BeginTime")>
        Public BeginTime As Date

        <Runtime.Serialization.DataMember(Name:="EndTime")>
        Public EndTime As Date

        <Runtime.Serialization.DataMember(Name:="Duration")>
        Public Duration As Double

        <Runtime.Serialization.DataMember(Name:="AbsenceType")>
        Public AbsenceType As String

        <Runtime.Serialization.DataMember(Name:="UserDescription")>
        Public UserDescription As String

    End Class

#Region "Propiedades"

    Private bolFeatureAnyWhere As Boolean = False
    Private bolFeatureTasks As Boolean = False
    Private bolFeatureDiningRoom As Boolean = False
    Private intSelectorType As Integer
    Private bolBusinessGroupApplyNotAllowedModifyCause As Boolean = False

    Public Property ActualEditCausePermission() As Boolean
        Get
            Dim bolEditCause As Boolean = False
            bolEditCause = (Me.DirectCausesPermission >= Permission.Write) AndAlso
                             (Me.CurrentDirectCausePermission >= Permission.Write)

            Return bolEditCause

        End Get
        Set(ByVal value As Boolean)
        End Set
    End Property

    Public Property ActualEditCostCenterPermission() As Boolean
        Get
            Dim boleditcenter As Boolean = False
            boleditcenter = (Me.DirectCausesPermission >= Permission.Read) AndAlso
                         (Me.CurrentDirectCausePermission >= Permission.Read)
            If boleditcenter Then
                boleditcenter = Me.CostCenterPermission > Permission.Read
            End If
            Return boleditcenter
        End Get
        Set(ByVal value As Boolean)
        End Set
    End Property

    Public Enum ViewPageTypes
        Moves = 1
        Mobility = 2
        Tasks = 3
        DinningRoom = 4
    End Enum

    Private Enum SaveReturnTypes
        Undefined
        ChangesSaved
        NoChangesSaved
        IsError
    End Enum

    <Runtime.Serialization.DataContract()>
    Private Class MoveParameter

        <Runtime.Serialization.DataMember(Name:="accion")>
        Public Accion As String

        <Runtime.Serialization.DataMember(Name:="valor")>
        Public Valor As String

        <Runtime.Serialization.DataMember(Name:="scriptClient")>
        Public ScriptClient As String

        <Runtime.Serialization.DataMember(Name:="showMessage")>
        Public ShowMessage As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class StatusRequest

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Long

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class MoveDetailsParameter

        <Runtime.Serialization.DataMember(Name:="isnewmove")>
        Public IsNewMove As String

        <Runtime.Serialization.DataMember(Name:="canedit")>
        Public CanEdit As String

        <Runtime.Serialization.DataMember(Name:="stateofrow")>
        Public StateOfRow As String

        <Runtime.Serialization.DataMember(Name:="idmove")>
        Public IdMove As Long

        <Runtime.Serialization.DataMember(Name:="keyrow")>
        Public KeyRow As String

        <Runtime.Serialization.DataMember(Name:="movetype")>
        Public MoveType As Integer

        <Runtime.Serialization.DataMember(Name:="actualtype")>
        Public ActualType As Integer

        <Runtime.Serialization.DataMember(Name:="idterminal")>
        Public IDTerminal As Integer

        <Runtime.Serialization.DataMember(Name:="idaction")>
        Public IDAction As Integer

        <Runtime.Serialization.DataMember(Name:="idcause_idtask_iddiningroom")>
        Public IDCause_IDTask_IDDiningRoom As Integer

        <Runtime.Serialization.DataMember(Name:="reliability")>
        Public Reliability As String

        <Runtime.Serialization.DataMember(Name:="telecommuting")>
        Public Telecommuting As String

        <Runtime.Serialization.DataMember(Name:="idzone")>
        Public IDZone As Integer

        <Runtime.Serialization.DataMember(Name:="actualdate")>
        Public ActualDate As String

        <Runtime.Serialization.DataMember(Name:="shiftdate")>
        Public ShiftDate As String

        <Runtime.Serialization.DataMember(Name:="position")>
        Public Position As String

        <Runtime.Serialization.DataMember(Name:="city")>
        Public City As String

        <Runtime.Serialization.DataMember(Name:="timeZone")>
        Public TimeZone As String

        <Runtime.Serialization.DataMember(Name:="idpassport")>
        Public IDPassport As Integer

        <Runtime.Serialization.DataMember(Name:="field1")>
        Public Field1 As String

        <Runtime.Serialization.DataMember(Name:="field2")>
        Public Field2 As String

        <Runtime.Serialization.DataMember(Name:="field3")>
        Public Field3 As String

        <Runtime.Serialization.DataMember(Name:="field4")>
        Public Field4 As Double

        <Runtime.Serialization.DataMember(Name:="field5")>
        Public Field5 As Double

        <Runtime.Serialization.DataMember(Name:="field6")>
        Public Field6 As Double

        <Runtime.Serialization.DataMember(Name:="invalidType")>
        Public InvalidType As Integer

        <Runtime.Serialization.DataMember(Name:="typeDetails")>
        Public TypeDetails As String

        <Runtime.Serialization.DataMember(Name:="center")>
        Public Center As Boolean

        <Runtime.Serialization.DataMember(Name:="fullAddress")>
        Public FullAddress As String

        <Runtime.Serialization.DataMember(Name:="maskAlert")>
        Public MaskAlert As Integer

        <Runtime.Serialization.DataMember(Name:="temperatureAlert")>
        Public TemperatureAlert As Integer

        <Runtime.Serialization.DataMember(Name:="verifyType")>
        Public VerifyType As Integer

        <Runtime.Serialization.DataMember(Name:="idRequest")>
        Public IDRequest As Integer

    End Class

#End Region

#Region "Permissions"

    Public Property CurrentPermission() As Permission
        Get
            Return Session("MovesPage_CurrentPermission")
        End Get
        Set(ByVal value As Permission)

            Session("MovesPage_CurrentPermission") = value

            If value > Permission.None Then
                Dim curDate As DateTime = Now
                If DatePage() IsNot Nothing Then curDate = DatePage()

                Me.CurrentJustifyIncPermission = Me.GetFeaturePermissionByEmployeeOnDate("Calendar.JustifyIncidences", Me.IDEmployeePage, curDate)
                Me.CurrentDirectCausePermission = Me.GetFeaturePermissionByEmployeeOnDate("Calendar.DirectCauses", Me.IDEmployeePage, curDate)

                Me.CurrentAccrualsSchedulerPermission = Me.GetFeaturePermission("Calendar.Accruals")
                Me.JustifyIncPermission = Me.GetFeaturePermission("Calendar.JustifyIncidences")
                Me.DirectCausesPermission = Me.GetFeaturePermission("Calendar.DirectCauses")

                Me.RemarksPermission = Me.GetFeaturePermission("Calendar.Remarks")
            Else
                Me.CurrentJustifyIncPermission = Permission.None
                Me.CurrentDirectCausePermission = Permission.None

                Me.CurrentAccrualsSchedulerPermission = Permission.None
                Me.JustifyIncPermission = Permission.None
                Me.DirectCausesPermission = Permission.None

                Me.RemarksPermission = Permission.None
            End If
        End Set
    End Property

    Public Property CurrentDirectCausePermission() As Permission
        Get
            Return Session("MovesPage_CurrentDirectCausePermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_CurrentDirectCausePermission") = value
        End Set
    End Property

    Public Property CurrentAccrualsSchedulerPermission() As Permission
        Get
            Return Session("MovesPage_CurrentAccrualsSchedulerPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_CurrentAccrualsSchedulerPermission") = value
        End Set
    End Property

    Public Property TaskPunchesPermission() As Permission
        Get
            Return Session("MovesPage_TaskPunchesPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_TaskPunchesPermission") = value
        End Set
    End Property

    Public Property DiningRoomPunchesPermission() As Permission
        Get
            Return Session("MovesPage_DiningRoomPunchesPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_DiningRoomPunchesPermission") = value
        End Set
    End Property

    Public Property DirectCausesPermission() As Permission
        Get
            Return Session("MovesPage_DirectCausesPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_DirectCausesPermission") = value
        End Set
    End Property

    Public Property CostCenterPermission() As Permission
        Get
            Return Session("MovesPage_CostCenterPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_CostCenterPermission") = value
        End Set
    End Property

    Public Property CurrentJustifyIncPermission() As Permission
        Get
            Return Session("MovesPage_CurrentJustifyIncPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_CurrentJustifyIncPermission") = value
        End Set
    End Property

    Public Property RemarksPermission() As Permission
        Get
            Return Session("MovesPage_RemarksPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_RemarksPermission") = value
        End Set
    End Property

    Public Property JustifyIncPermission() As Permission
        Get
            Return Session("MovesPage_JustifyIncPermission")
        End Get
        Set(ByVal value As Permission)
            Session("MovesPage_JustifyIncPermission") = value
        End Set
    End Property

    Public Property FeatureCostCenter() As Boolean
        Get
            Return Session("MovesPage_FeatureCostCenter")
        End Get
        Set(ByVal value As Boolean)
            Session("MovesPage_FeatureCostCenter") = value
        End Set
    End Property

#End Region

#Region "GLOBAL Properties"

    Public Property LabelYear() As String
        Get
            If Session("MovesPage_LabelYear") IsNot Nothing Then
                Return Session("MovesPage_LabelYear")
            Else
                Session("MovesPage_LabelYear") = Me.Language.Translate("Type.Annual", Me.DefaultScope)
                Return Session("MovesPage_LabelYear")
            End If
        End Get
        Set(ByVal value As String)
            Session("MovesPage_LabelYear") = value
        End Set
    End Property

    Public Property LabelMonth() As String
        Get
            If Session("MovesPage_LabelMonth") IsNot Nothing Then
                Return Session("MovesPage_LabelMonth")
            Else
                Session("MovesPage_LabelMonth") = Me.Language.Translate("Type.Month", Me.DefaultScope)
                Return Session("MovesPage_LabelMonth")
            End If
        End Get
        Set(ByVal value As String)
            Session("MovesPage_LabelMonth") = value
        End Set
    End Property

    Public Property LabelWeek() As String
        Get
            If Session("MovesPage_LabelWeek") IsNot Nothing Then
                Return Session("MovesPage_LabelWeek")
            Else
                Session("MovesPage_LabelWeek") = Me.Language.Translate("Type.Week", Me.DefaultScope)
                Return Session("MovesPage_LabelWeek")
            End If
        End Get
        Set(ByVal value As String)
            Session("MovesPage_LabelWeek") = value
        End Set
    End Property

    Public Property LabelContract() As String
        Get
            If Session("MovesPage_LabelContract") IsNot Nothing Then
                Return Session("MovesPage_LabelContract")
            Else
                Session("MovesPage_LabelContract") = Me.Language.Translate("Type.Contract", Me.DefaultScope)
                Return Session("MovesPage_LabelContract")
            End If
        End Get
        Set(ByVal value As String)
            Session("MovesPage_LabelContract") = value
        End Set
    End Property

    Public Property LabelAnnualWork() As String
        Get
            If Session("MovesPage_LabelAnnualWork") IsNot Nothing Then
                Return Session("MovesPage_LabelAnnualWork")
            Else
                Session("MovesPage_LabelAnnualWork") = Me.Language.Translate("Type.AnnualWork", Me.DefaultScope)
                Return Session("MovesPage_LabelAnnualWork")
            End If
        End Get
        Set(ByVal value As String)
            Session("MovesPage_LabelContract") = value
        End Set
    End Property

    Public Property LabelDay() As String
        Get
            If Session("MovesPage_LabelDay") IsNot Nothing Then
                Return Session("MovesPage_LabelDay")
            Else
                Session("MovesPage_LabelDay") = Me.Language.Translate("Type.Day", Me.DefaultScope)
                Return Session("MovesPage_LabelDay")
            End If
        End Get
        Set(ByVal value As String)
            Session("MovesPage_LabelDay") = value
        End Set
    End Property

    Public Property IdFilteredTask() As Integer
        Get
            If Session("MovesPage_IdFilteredTask") IsNot Nothing Then
                Return Session("MovesPage_IdFilteredTask")
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            Session("MovesPage_IdFilteredTask") = value
        End Set
    End Property

    Public Property IDEmployeePageInitial() As Integer
        Get
            Return Session("MovesPage_IDEmployeePageInitial")
        End Get
        Set(ByVal value As Integer)
            Session("MovesPage_IDEmployeePageInitial") = value
        End Set
    End Property

    Public Property EmployeeFilter() As String
        Get
            Return Session("MovesPage_EmployeeFilter")
        End Get
        Set(ByVal value As String)
            Session("MovesPage_EmployeeFilter") = value
        End Set
    End Property

    Public Property IsNewVersion() As String
        Get
            Return Session("MovesPage_IsNewVersion")
        End Get
        Set(ByVal value As String)
            Session("MovesPage_IsNewVersion") = value
        End Set
    End Property

    Public Property HasAction() As Boolean
        Get
            Return Session("MovesPage_HasAction")
        End Get
        Set(ByVal value As Boolean)
            Session("MovesPage_HasAction") = value
        End Set
    End Property

    Public Property SelectorBeginDate() As String
        Get
            Return Session("MovesPage_SelectorBeginDate")
        End Get
        Set(ByVal value As String)
            Session("MovesPage_SelectorBeginDate") = value
        End Set
    End Property

    Public Property SelectorEndDate() As String
        Get
            Return Session("MovesPage_SelectorEndDate")
        End Get
        Set(ByVal value As String)
            Session("MovesPage_SelectorEndDate") = value
        End Set
    End Property

    Public Property GridSelectorRowSel() As Integer
        Get
            Return roTypes.Any2Integer(Session("MovesPage_GridSelectorRowSel"))
        End Get
        Set(ByVal value As Integer)
            If value < 0 Then value = 0
            Session("MovesPage_GridSelectorRowSel") = value
        End Set
    End Property

    Public Property IDEmployeePage() As Integer
        Get
            Return Session("MovesPage_IDEmployeePage")
        End Get

        Set(ByVal value As Integer)

            Session("MovesPage_IDEmployeePage") = value
            Me.hdnIDEmployeePage.Value = value

            If value > 0 Then
                Dim curDate As DateTime = Now
                If Me.DatePage IsNot Nothing Then curDate = Me.DatePage
                Me.CurrentPermission = Me.GetFeaturePermissionByEmployeeOnDate("Calendar.Punches.Punches", value, curDate)

                Dim bolCurrentJustifyAddNewIncPermissionActive As Boolean = Me.DirectCausesPermission >= Permission.Admin AndAlso
                                                                            Me.CurrentDirectCausePermission >= Permission.Write AndAlso
                                                                            Me.CheckPeriodOfFreezingPage(True)

                Me.btnAddNewIncidence.Visible = bolCurrentJustifyAddNewIncPermissionActive
                Me.btnAddNewMove.Visible = (Me.CurrentPermission > Permission.Read Or Me.CurrentPermission >= Permission.Read And Me.CostCenterPermission >= Permission.Write) AndAlso Me.CheckPeriodOfFreezingPage(True)

                Dim dtPlan As DataTable = API.EmployeeServiceMethods.GetPlan(Me.Page, value, curDate)

                If (dtPlan IsNot Nothing AndAlso dtPlan.Rows.Count > 0) Then
                    Dim oShiftInfo As Robotics.Base.DTOs.roCalendarShift = CalendarServiceMethods.GetShiftDefinition(Me.Page, roTypes.Any2Integer(dtPlan.Rows(0)("IDShift1")))
                    Me.ibtShiftSelector.Visible = (Me.HasFeaturePermissionByEmployeeOnDate("Calendar.Scheduler", Permission.Write, value, curDate) AndAlso Not oShiftInfo.AllowFloating AndAlso Not oShiftInfo.AllowComplementary)

                    Me.spRemoveHolidays.Visible = (Me.ibtShiftSelector.Visible And roTypes.Any2Boolean(dtPlan.Rows(0)("IsHolidays")))
                    Me.ibtRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
                    Me.lbRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
                Else

                    Me.ibtShiftSelector.Visible = Me.HasFeaturePermissionByEmployeeOnDate("Calendar.Scheduler", Permission.Write, value, curDate)

                    Me.spRemoveHolidays.Visible = False
                    Me.ibtRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
                    Me.lbRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
                End If

                'bolEditEnabled = (Me.JustifyIncPermission >= Permission.Write) AndAlso
                '                 (Me.CurrentJustifyIncPermission >= Permission.Write)

                Me.spRemoveHolidays.Visible = Me.ibtShiftSelector.Visible
                Me.txtEmployeeName.Text = GetEmployeeName(value)
                Me.txtRemarks.ReadOnly = Not Me.RemarksPermission > Permission.Read
                Me.btnAply.Visible = (Me.CurrentPermission > Permission.Read) Or (Me.CurrentJustifyIncPermission > Permission.Read) Or (Me.ViewPage = ViewPageTypes.Moves And Me.RemarksPermission > Permission.Read) Or
                                (Me.ViewPage = ViewPageTypes.Moves And Me.CurrentPermission >= Permission.Read And Me.CostCenterPermission > Permission.Read)
                Me.btnUndo.Visible = (Me.CurrentPermission > Permission.Read) Or (Me.ViewPage = ViewPageTypes.Moves And Me.CurrentPermission >= Permission.Read And Me.CostCenterPermission > Permission.Read) Or (Me.ViewPage = ViewPageTypes.Moves And Me.RemarksPermission > Permission.Read)
            Else

                Me.CurrentPermission = Permission.Read

                Me.btnAddNewIncidence.Visible = False
                Me.btnAddNewMove.Visible = False

                Me.ibtShiftSelector.Visible = False
                Me.spRemoveHolidays.Visible = False
                Me.ibtRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
                Me.lbRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
                Me.txtEmployeeName.Text = String.Empty
                Me.txtRemarks.ReadOnly = False
                Me.btnAply.Visible = True
                Me.btnUndo.Visible = True
            End If

        End Set
    End Property

    Public Property DatePage() As Nullable(Of DateTime)
        Get
            Return Session("MovesPage_DatePage")
        End Get

        Set(ByVal value As Nullable(Of DateTime))

            If Not value Is Nothing Then

                Session("MovesPage_DatePage") = value
                Me.hdnDatePage.Value = value

                Dim bolCurrentJustifyAddNewIncPermissionActive As Boolean = Me.DirectCausesPermission >= Permission.Admin AndAlso
                                                                            Me.CurrentDirectCausePermission >= Permission.Write AndAlso
                                                                            Me.CheckPeriodOfFreezingPage(True)

                Me.btnAddNewIncidence.Visible = bolCurrentJustifyAddNewIncPermissionActive
                Me.btnAddNewMove.Visible = (Me.CurrentPermission > Permission.Read) AndAlso Me.CheckPeriodOfFreezingPage(False)

                Me.txtDatePage.Value = value
                Me.txtDate2.Text = value

                If (value.Value > Date.Today) Then
                    Me.lblEditIncidences.Text = Me.Language.Translate("lblEditIncidences.Future", Me.DefaultScope)
                End If
            Else

                ViewState.Remove("MovesPage_DatePage")
                Me.hdnDatePage.Value = ""

                Me.btnAddNewIncidence.Visible = False
                Me.btnAddNewMove.Visible = False

                Me.txtDatePage.Value = Nothing
                Me.txtDate2.Text = String.Empty

            End If

        End Set
    End Property

    Public Property FreezingDatePage(Optional ByVal bolReload As Boolean = False) As Date
        Get
            Dim xFreezeDate As Date = DateTime.MinValue
            If Session("MovesPage_FreezingDate") IsNot Nothing Then
                xFreezeDate = CDate(Session("MovesPage_FreezingDate"))
            End If

            If bolReload OrElse xFreezeDate = DateTime.MinValue Then
                xFreezeDate = API.ConnectorServiceMethods.GetFirstDate(Me.Page)
                Session("MovesPage_FreezingDate") = xFreezeDate
            End If

            Return CDate(Session("MovesPage_FreezingDate"))
        End Get
        Set(ByVal value As Date)
            Session("MovesPage_FreezingDate") = value
        End Set
    End Property

    Public ReadOnly Property CheckPeriodOfFreezingPage(Optional ByVal bolReload As Boolean = False) As Boolean
        Get
            If bolReload Then Session("MovesPage_CheckPeriodOfFreezingPage") = Nothing
            Dim bolFreeze = Session("MovesPage_CheckPeriodOfFreezingPage")

            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_MovesLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = Session("MovesPage_MovesLastDate")

            If WLHelperWeb.CurrentPassportID > 0 AndAlso (bolReload OrElse bolFreeze Is Nothing OrElse
               Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
               Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DatePage) Then
                bolFreeze = CheckPeriodOfFreezing()
                Session("MovesPage_CheckPeriodOfFreezingPage") = bolFreeze
            End If

            Return bolFreeze
        End Get
    End Property

    Public Property ViewPage() As ViewPageTypes
        Get
            Return Session("MovesPage_ViewPage")
        End Get
        Set(ByVal value As ViewPageTypes)
            Session("MovesPage_ViewPage") = value
            Me.hdnViewPage.Value = value
        End Set
    End Property

    Public Property ViewAcumPage() As String
        Get
            Dim ViewAcumPageTemp As String = HelperWeb.GetCookie("SchedulerMovesTabAcumSelected")
            If ViewAcumPageTemp = String.Empty Then
                ViewAcumPageTemp = "ANUAL"
                HelperWeb.CreateCookie("SchedulerMovesTabAcumSelected", ViewAcumPageTemp, False)
            End If
            Return ViewAcumPageTemp
        End Get
        Set(ByVal value As String)
            If value.ToUpper = "ANUAL" Then
                HelperWeb.CreateCookie("SchedulerMovesTabAcumSelected", "ANUAL", False)
            Else
                HelperWeb.CreateCookie("SchedulerMovesTabAcumSelected", "DAILY", False)
            End If
        End Set
    End Property

    Public Property IdGroup() As Integer
        Get
            If Session("MovesPage_IdGroup") IsNot Nothing Then
                Return Session("MovesPage_IdGroup")
            Else
                Return 0
            End If
        End Get
        Set(ByVal value As Integer)
            Session("MovesPage_IdGroup") = value
        End Set
    End Property

#End Region

#Region "Moves Properties functions"

    Private Property PunchesDataTable(Optional ByVal bolReload As Boolean = False, Optional ByVal bInitialLoad As Boolean = False) As DataTable
        Get
            Dim tbPunches As DataTable = Session("MovesPage_PunchesData")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_MovesLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = Session("MovesPage_MovesLastDate")
            Dim eLastViewPage As Nullable(Of ViewPageTypes) = roTypes.Any2Integer(Session("MovesPage_MovesLastViewPage"))

            If Not bInitialLoad Then
                If WLHelperWeb.CurrentPassport IsNot Nothing AndAlso (bolReload OrElse tbPunches Is Nothing OrElse
               Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
               Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DatePage OrElse
               Not eLastViewPage.HasValue OrElse eLastViewPage.Value <> Me.ViewPage) Then

                    Dim startDate As DateTime = New DateTime(Me.DatePage.Value.Year, Me.DatePage.Value.Month, Me.DatePage.Value.Day, 0, 0, 0)
                    Dim endDate As DateTime = New DateTime(Me.DatePage.Value.Year, Me.DatePage.Value.Month, Me.DatePage.Value.Day, 23, 59, 59)

                    Dim strTypes As String
                    Select Case Me.ViewPage
                        Case ViewPageTypes.Tasks
                            strTypes = "1,2,3,4,7"

                        Case ViewPageTypes.DinningRoom
                            strTypes = "10"

                        Case Else
                            'strTypes = "1,2,3,5,7"
                            strTypes = "1,2,3,5,6,7"
                            If CostCenterPermission >= Permission.Read Then
                                strTypes += ",13"
                            End If

                    End Select

                    tbPunches = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, startDate, endDate, Nothing, Nothing, Me.IDEmployeePage, , , , , , strTypes, , , , )

                    'añado columna de la clave
                    tbPunches.Columns.Add(New DataColumn("Clave", GetType(String)))
                    tbPunches.Columns.Add(New DataColumn("IsModified", GetType(Boolean)))

                    For Each r As DataRow In tbPunches.Rows
                        r("Clave") = Guid.NewGuid()
                        r("IsModified") = False
                    Next

                    'si los fichajes son de comedor quitar los marcados como invalidos sea por el motivo que sea
                    'If Me.ViewPage = ViewPageTypes.DinningRoom Then
                    '    If tbPunches.Rows.Count > 0 Then
                    '        Dim PunchesRows As DataRow() = tbPunches.Select("InvalidType IS NULL")
                    '        If PunchesRows.Length > 0 Then
                    '            tbPunches = PunchesRows.CopyToDataTable
                    '        Else
                    '            tbPunches.Rows.Clear()
                    '            tbPunches.AcceptChanges()
                    '        End If
                    '    End If
                    'End If

                    'IMPORTANTE HACERLO AL FINAL PORQUE LA FUNCION PunchesRows.CopyToDataTable ELIMINA LAS PRIMARYKEY DE LA DATATABLE------------
                    'Añade campo clave a la tabla
                    If Not tbPunches Is Nothing Then
                        tbPunches.PrimaryKey = New DataColumn() {tbPunches.Columns("Clave")}
                        tbPunches.AcceptChanges()
                    End If

                    Session("MovesPage_PunchesData") = tbPunches
                    Session("MovesPage_MovesLastIDEmployee") = Me.IDEmployeePage
                    Session("MovesPage_MovesLastDate") = Me.DatePage
                    Session("MovesPage_MovesLastViewPage") = Me.ViewPage

                    GridMoves.FocusedRowIndex = 0
                End If
            End If

            Return tbPunches
        End Get
        Set(ByVal value As DataTable)
            Session("MovesPage_PunchesData") = value
        End Set
    End Property

    Private Property BusinessCentersDataTable(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataTable
        Get
            Dim tbBusinessCenters As DataTable = Session("MovesPage_BusinessCenters")

            If bolReload OrElse tbBusinessCenters Is Nothing Then

                tbBusinessCenters = API.TasksServiceMethods.GetBusinessCenters(Me, bolCheckStatus)
                Dim oNewRow As DataRow = tbBusinessCenters.NewRow
                oNewRow.Item("ID") = 0
                oNewRow.Item("Name") = Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope)
                tbBusinessCenters.Rows.Add(oNewRow)
                tbBusinessCenters.AcceptChanges()

                Session("MovesPage_BusinessCenters") = tbBusinessCenters
            End If

            Return tbBusinessCenters

        End Get
        Set(ByVal value As DataTable)
            Session("MovesPage_BusinessCenters") = value
        End Set
    End Property

    Private Property BusinessCentersDetailDataTable(Optional ByVal bolReload As Boolean = False, Optional ByVal bolCheckStatus As Boolean = False) As DataTable
        Get
            Dim tbBusinessCenters As DataTable = Session("MovesPage_BusinessCentersDetail")

            If bolReload OrElse tbBusinessCenters Is Nothing Then

                tbBusinessCenters = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me, WLHelperWeb.CurrentPassportID, bolCheckStatus)

                Dim oNewRow As DataRow = tbBusinessCenters.NewRow
                oNewRow.Item("ID") = 0
                oNewRow.Item("Name") = Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope)
                tbBusinessCenters.Rows.Add(oNewRow)
                tbBusinessCenters.AcceptChanges()

                Session("MovesPage_BusinessCentersDetail") = tbBusinessCenters
            End If

            Return tbBusinessCenters

        End Get
        Set(ByVal value As DataTable)
            Session("MovesPage_BusinessCentersDetail") = value
        End Set
    End Property

    Private Property GetAvailableCostCentersByEmployee(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbBusinessCenters As DataTable = Session("MovesPage_AvailableCentersByEmployee")

            If bolReload OrElse tbBusinessCenters Is Nothing Then

                tbBusinessCenters = API.CostCenterServiceMethods.GetAvailableCostCentersByEmployee(Me, Me.IDEmployeePage, Me.DatePage, True)
                If Not tbBusinessCenters Is Nothing Then
                    Dim oNewRow As DataRow = tbBusinessCenters.NewRow
                    oNewRow.Item("IDCenter") = 0
                    oNewRow.Item("Name") = Me.Language.Translate("CostCenter.DefaultCostCenter", Me.DefaultScope)
                    tbBusinessCenters.Rows.Add(oNewRow)
                    tbBusinessCenters.AcceptChanges()
                End If

                Session("MovesPage_AvailableCentersByEmployee") = tbBusinessCenters
            End If

            Return tbBusinessCenters

        End Get
        Set(ByVal value As DataTable)
            Session("MovesPage_AvailableCentersByEmployee") = value
        End Set
    End Property

    Public ReadOnly Property PunchesDataView(Optional ByVal bolReload As Boolean = False, Optional ByVal bInitialLoad As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.PunchesDataTable(bolReload, bInitialLoad)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
                dv.Sort = "DateTime, Id"
            End If
            Return dv

            Return dv
        End Get
    End Property

    Public Property ShowPunchOrder() As Boolean
        Get
            Dim _showOrder As Boolean = True
            If Not Session("ShowPunchOrder") Is Nothing Then
                _showOrder = Convert.ToBoolean(Session("ShowPunchOrder"))
            End If
            Return _showOrder
        End Get
        Set(ByVal value As Boolean)
            Session("MovesPage_ShowPunchOrder") = value
        End Set
    End Property

#End Region

#Region "Incidences Properties functions"

    Private Property IncidencesDataTable(Optional ByVal bolReload As Boolean = False, Optional ByVal bInitialLoad As Boolean = False) As DataTable
        Get

            Dim tbIncidences As DataTable = Session("MovesPage_IncidencesDatatable")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_IncidencesLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = Session("MovesPage_IncidencesLastDate")

            If Not bInitialLoad Then
                If bolReload OrElse tbIncidences Is Nothing OrElse
               Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
               Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DatePage Then

                    tbIncidences = API.EmployeeServiceMethods.GetIncidences(Me.Page, Me.IDEmployeePage, Me.DatePage)

                    'añado columna de hora en formato hora desde formato double
                    tbIncidences.Columns.Add(New DataColumn("ValueHora", GetType(String)))
                    tbIncidences.Columns.Add(New DataColumn("ValueHoraEditable", GetType(String)))

                    'añado columna de texto de incidencia
                    tbIncidences.Columns.Add(New DataColumn("TextoIncidencia", GetType(String)))

                    'añado columna de la clave
                    tbIncidences.Columns.Add(New DataColumn("Clave", GetType(String)))

                    'añado columna de la calculado
                    tbIncidences.Columns.Add(New DataColumn("Calculado", GetType(String)))

                    Dim IncidenceText As String

                    For Each r As DataRow In tbIncidences.Rows
                        r("ValueHora") = roConversions.ConvertHoursToTime(CDbl(r("Value")))
                        r("ValueHoraEditable") = roConversions.ConvertHoursToTime(CDbl(r("Value")))
                        'r("TextoIncidencia") = IIf(roTypes.Any2String(r("IDType")) <> String.Empty, Me.Language.Keyword("Incidence." & roTypes.Any2String(r("IDType"))).ToUpper(), "")
                        If roTypes.Any2Double(r("IDRelatedIncidence")) = 0 Then
                            r("TextoIncidencia") = String.Empty
                            r("ValueHora") = String.Empty

                            ' En el caso de una justificación añadida o que venga de alguna regla
                            ' miramos el tipo de justificacion para formatear el valor de una forma u otra
                            If roTypes.Any2Boolean(r("DayType")) Or roTypes.Any2Boolean(r("CustomType")) Then
                                ' Formato numerico
                                r("ValueHoraEditable") = Format(CDbl(r("Value")), "##0.000")
                            End If
                        Else
                            If roTypes.Any2String(r("IDType")) <> String.Empty Then
                                IncidenceText = Me.Language.Keyword("Incidence." & roTypes.Any2String(r("IDType")))
                                If IncidenceText <> String.Empty Then
                                    r("TextoIncidencia") = IncidenceText.Substring(0, 1).ToUpper & IncidenceText.Substring(1)
                                Else
                                    r("TextoIncidencia") = String.Empty
                                End If
                            Else
                                r("TextoIncidencia") = String.Empty
                            End If

                        End If
                        r("Clave") = Guid.NewGuid()
                        If roTypes.Any2Boolean(r("Manual")) = True Then
                            r("Calculado") = Me.Language.Translate("Calculado.Manual", Me.DefaultScope)
                        Else
                            r("Calculado") = Me.Language.Translate("Calculado.Automatico", Me.DefaultScope)
                        End If
                    Next

                    'añade campo clave a la tabla
                    If Not tbIncidences Is Nothing Then
                        tbIncidences.PrimaryKey = New DataColumn() {tbIncidences.Columns("Clave")}

                        'If tbIncidences.Rows.Count = 0 Then 'Añadir fila vacía porque si el grid se carga sin filas, el combobox de incidencias no se abre (error de ASPXGriwView)
                        '    Dim oNewRow As DataRow = tbIncidences.NewRow
                        '    oNewRow.Item("Clave") = Guid.NewGuid()
                        '    oNewRow.Item("IDEmployee") = Me.IDEmployeePage
                        '    oNewRow.Item("Date") = Me.DatePage
                        '    oNewRow.Item("IDCause") = 0
                        '    oNewRow.Item("Manual") = 0
                        '    oNewRow.Item("IDCenter") = 0
                        '    oNewRow.Item("DefaultCenter") = 1
                        '    oNewRow.Item("ManualCenter") = 0

                        '    tbIncidences.Rows.Add(oNewRow)
                        'End If
                        tbIncidences.AcceptChanges()
                    End If
                End If

            End If
            Session("MovesPage_IncidencesDatatable") = tbIncidences
            Session("MovesPage_IncidencesLastIDEmployee") = Me.IDEmployeePage
            Session("MovesPage_IncidencesLastDate") = Me.DatePage

            Return tbIncidences

        End Get
        Set(ByVal value As DataTable)
            Session("MovesPage_IncidencesDatatable") = value
        End Set
    End Property

    Private ReadOnly Property IncidencesDataView(Optional ByVal bolReload As Boolean = False, Optional ByVal bInitialLoad As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.IncidencesDataTable(bolReload, bInitialLoad)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
                dv.Sort = "BeginTimeOrder,DailyRule,AccruedRule,AccrualsRules"
            End If
            Return dv
        End Get
    End Property

    Private Function GetJustCausesDataAll(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session("MovesPage_JustCausesDataAll") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = CausesServiceMethods.GetCauses(Me.Page, "", False)
            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                Session("MovesPage_JustCausesDataAll") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("MovesPage_JustCausesDataAll")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

    Private Function GetJustCausesData(Optional ByVal bolReload As Boolean = False) As DataView

        If bolReload Or Session("MovesPage_JustCausesData") Is Nothing Then
            Dim dv As DataView = Nothing
            Dim returnTb As New DataTable
            returnTb.Columns.Add(New DataColumn("ID", GetType(Short)))
            returnTb.Columns.Add(New DataColumn("Name", GetType(String)))
            returnTb.Columns.Add(New DataColumn("DayType", GetType(Boolean)))
            returnTb.Columns.Add(New DataColumn("CustomType", GetType(Boolean)))
            Dim oNewRow As DataRow = returnTb.NewRow

            Dim tb As DataTable = CausesServiceMethods.GetCauses(Me.Page, "", True)
            If tb IsNot Nothing Then
                For Each oRow As DataRow In tb.Rows
                    oNewRow = returnTb.NewRow
                    With oNewRow
                        .Item("ID") = oRow("ID")
                        .Item("Name") = oRow("Name")
                        .Item("DayType") = roTypes.Any2Boolean(oRow("DayType"))
                        .Item("CustomType") = roTypes.Any2Boolean(oRow("CustomType"))

                    End With
                    returnTb.Rows.Add(oNewRow)
                Next

                Session("MovesPage_JustCausesData") = returnTb

                dv = New DataView(returnTb)
                dv.Sort = "Name ASC"
            End If

            Return dv
        Else
            Dim dtReturn As DataTable = Session("MovesPage_JustCausesData")
            Dim dv As DataView = New DataView(dtReturn)
            dv.Sort = "Name ASC"
            Return dv
        End If
    End Function

#End Region

#Region "Acum Properties functions"

    Private ReadOnly Property AcumAnualDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tbTotalAccruals As DataTable = Session("MovesPage_AcumAnualDatatable")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_AcumAnualLastIDEmployee")
            Dim dLastDate As Nullable(Of Date) = Session("MovesPage_AcumAnualLastDate")
            Dim strLastView As String = Session("MovesPage_AcumAnualLastView")

            If bolReload OrElse tbTotalAccruals Is Nothing OrElse
               Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
               Not dLastDate.HasValue OrElse dLastDate.Value <> Me.DatePage OrElse
               strLastView = String.Empty OrElse strLastView <> Me.ViewPage Then

                tbTotalAccruals = Nothing
                If Me.ViewPage <> ViewPageTypes.Tasks Then
                    If Me.CurrentAccrualsSchedulerPermission >= Permission.Read Then
                        tbTotalAccruals = API.EmployeeServiceMethods.GetAnualAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage, True)
                    End If
                Else
                    tbTotalAccruals = API.EmployeeServiceMethods.GetTaskAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage)
                End If

                If tbTotalAccruals IsNot Nothing Then

                    tbTotalAccruals.Columns.Add(New DataColumn("Type", GetType(String)))
                    tbTotalAccruals.Columns.Add(New DataColumn("TotalFormatted", GetType(String)))
                    tbTotalAccruals.Columns.Add(New DataColumn("Periodicidad", GetType(String)))

                    For Each oRow As DataRow In tbTotalAccruals.Rows
                        oRow("Type") = "A" 'Anual
                        oRow("TotalFormatted") = oRow("TotalFormat")
                        oRow("Periodicidad") = LabelYear() '"Año"
                    Next

                End If

                If Me.ViewPage <> ViewPageTypes.Tasks Then
                    Dim tbMonthAccruals As DataTable = Nothing
                    If Me.CurrentAccrualsSchedulerPermission >= Permission.Read Then
                        tbMonthAccruals = API.EmployeeServiceMethods.GetMonthAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage, True)
                    End If

                    If tbMonthAccruals IsNot Nothing Then

                        tbMonthAccruals.Columns.Add(New DataColumn("Type", GetType(String)))
                        tbMonthAccruals.Columns.Add(New DataColumn("TotalFormatted", GetType(String)))
                        tbMonthAccruals.Columns.Add(New DataColumn("Periodicidad", GetType(String)))

                        For Each oRow As DataRow In tbMonthAccruals.Rows
                            oRow("Type") = "M" 'Mensual
                            oRow("TotalFormatted") = oRow("TotalFormat")
                            oRow("Periodicidad") = LabelMonth()
                            tbTotalAccruals.ImportRow(oRow)
                        Next
                    End If
                End If

                'SALDOS SEMANALES
                If Me.ViewPage <> ViewPageTypes.Tasks Then
                    Dim tbWeekAccruals As DataTable = Nothing
                    If Me.CurrentAccrualsSchedulerPermission >= Permission.Read Then
                        tbWeekAccruals = API.EmployeeServiceMethods.GetWeekAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage, True)
                    End If

                    If tbWeekAccruals IsNot Nothing Then

                        tbWeekAccruals.Columns.Add(New DataColumn("Type", GetType(String)))
                        tbWeekAccruals.Columns.Add(New DataColumn("TotalFormatted", GetType(String)))
                        tbWeekAccruals.Columns.Add(New DataColumn("Periodicidad", GetType(String)))

                        For Each oRow As DataRow In tbWeekAccruals.Rows
                            oRow("Type") = "W" 'Semanal
                            oRow("TotalFormatted") = oRow("TotalFormat")
                            oRow("Periodicidad") = LabelWeek()
                            tbTotalAccruals.ImportRow(oRow)
                        Next
                    End If
                End If

                'SALDOS Contrato
                If Me.ViewPage <> ViewPageTypes.Tasks Then
                    Dim tbContractAccruals As DataTable = Nothing
                    If Me.CurrentAccrualsSchedulerPermission >= Permission.Read Then
                        tbContractAccruals = API.EmployeeServiceMethods.GetContractAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage, True)
                    End If

                    If tbContractAccruals IsNot Nothing Then

                        tbContractAccruals.Columns.Add(New DataColumn("Type", GetType(String)))
                        tbContractAccruals.Columns.Add(New DataColumn("TotalFormatted", GetType(String)))
                        tbContractAccruals.Columns.Add(New DataColumn("Periodicidad", GetType(String)))

                        For Each oRow As DataRow In tbContractAccruals.Rows
                            oRow("Type") = "C" 'Contrato
                            oRow("TotalFormatted") = oRow("TotalFormat")
                            oRow("Periodicidad") = LabelContract()
                            tbTotalAccruals.ImportRow(oRow)
                        Next
                    End If
                End If

                'SALDOS Anual Laboral
                If Me.ViewPage <> ViewPageTypes.Tasks Then
                    Dim tbAnnualWorkAccruals As DataTable = Nothing
                    If Me.CurrentAccrualsSchedulerPermission >= Permission.Read Then
                        tbAnnualWorkAccruals = API.EmployeeServiceMethods.GetAnnualWorkAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage, True)
                    End If

                    If tbAnnualWorkAccruals IsNot Nothing Then

                        tbAnnualWorkAccruals.Columns.Add(New DataColumn("Type", GetType(String)))
                        tbAnnualWorkAccruals.Columns.Add(New DataColumn("TotalFormatted", GetType(String)))
                        tbAnnualWorkAccruals.Columns.Add(New DataColumn("Periodicidad", GetType(String)))

                        For Each oRow As DataRow In tbAnnualWorkAccruals.Rows
                            oRow("Type") = "L" 'Anual Laboral
                            oRow("TotalFormatted") = oRow("TotalFormat")
                            oRow("Periodicidad") = LabelAnnualWork()
                            tbTotalAccruals.ImportRow(oRow)
                        Next
                    End If
                End If

                'añade campo clave a la tabla
                If Not tbTotalAccruals Is Nothing Then
                    If Me.ViewPage = ViewPageTypes.Tasks Then
                        tbTotalAccruals.PrimaryKey = New DataColumn() {tbTotalAccruals.Columns("IDTask")}
                        tbTotalAccruals.AcceptChanges()
                    Else
                        tbTotalAccruals.PrimaryKey = New DataColumn() {tbTotalAccruals.Columns("IDConcept"), tbTotalAccruals.Columns("CarryOver")}
                        tbTotalAccruals.AcceptChanges()
                    End If
                End If

                Session("MovesPage_AcumAnualDatatable") = tbTotalAccruals
                Session("MovesPage_AcumAnualLastIDEmployee") = Me.IDEmployeePage
                Session("MovesPage_AcumAnualLastDate") = Me.DatePage
                Session("MovesPage_AcumAnualLastView") = Me.ViewPage

            End If

            Return tbTotalAccruals

        End Get
    End Property

    Private ReadOnly Property AcumAnualDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.AcumAnualDataTable(bolReload)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
                dv.Sort = "Name"
            End If
            Return dv
        End Get
    End Property

    Private ReadOnly Property AcumDailyDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tbDailyAccruals As DataTable = Session("MovesPage_AcumDailyDatatable")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_AcumDailyLastIDEmployee")
            Dim dLastDate As Nullable(Of Date) = Session("MovesPage_AcumDailyLastDate")
            Dim strLastView As String = Session("MovesPage_AcumDailyLastView")

            If bolReload OrElse tbDailyAccruals Is Nothing OrElse
               Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
               Not dLastDate.HasValue OrElse dLastDate.Value <> Me.DatePage OrElse
               strLastView = String.Empty OrElse strLastView <> Me.ViewPage Then

                tbDailyAccruals = Nothing
                If Me.ViewPage <> ViewPageTypes.Tasks Then
                    If Me.CurrentAccrualsSchedulerPermission >= Permission.Read Then
                        tbDailyAccruals = API.EmployeeServiceMethods.GetDailyAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage, True)
                    End If
                Else
                    tbDailyAccruals = API.EmployeeServiceMethods.GetDailyTaskAccruals(Me.Page, Me.IDEmployeePage, Me.DatePage)
                End If

                'añade campo clave a la tabla
                If Not tbDailyAccruals Is Nothing Then

                    tbDailyAccruals.Columns.Add(New DataColumn("TotalFormatted", GetType(String)))
                    tbDailyAccruals.Columns.Add(New DataColumn("Periodicidad", GetType(String)))

                    For Each oRow As DataRow In tbDailyAccruals.Rows
                        'oRow("Total") = oRow("Value")
                        oRow("TotalFormatted") = oRow("ValueFormat")
                        oRow("Periodicidad") = LabelDay() '"Día"
                    Next

                    If Me.ViewPage = ViewPageTypes.Tasks Then
                        tbDailyAccruals.PrimaryKey = New DataColumn() {tbDailyAccruals.Columns("IDTask")}
                        tbDailyAccruals.AcceptChanges()
                    Else
                        'tbDailyAccruals.PrimaryKey = New DataColumn() {tbDailyAccruals.Columns("IDConcept"), tbDailyAccruals.Columns("CarryOver")}
                        tbDailyAccruals.AcceptChanges()
                    End If

                End If

                Session("MovesPage_AcumDailyDatatable") = tbDailyAccruals
                Session("MovesPage_AcumDailyLastIDEmployee") = Me.IDEmployeePage
                Session("MovesPage_AcumDailyLastDate") = Me.DatePage
                Session("MovesPage_AcumDailyLastView") = Me.ViewPage

            End If

            Return tbDailyAccruals

        End Get
    End Property

    Private ReadOnly Property AcumDailyDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.AcumDailyDataTable(bolReload)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
                dv.Sort = "Name"
            End If
            Return dv
        End Get
    End Property

#End Region

#Region "Selector Properties functions"

    Private Property SelectorDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get

            Dim dv As DataView = Nothing
            Dim tb As DataTable = Session("MovesPage_SelectorData")

            Try
                Dim intLastSelectorType As Nullable(Of Integer) = Session("MovesPage_LastSelectorType")
                Dim strSelectorValue As String = HelperWeb.GetCookie("Moves_SelectorValue")
                If strSelectorValue <> "" Then
                    Me.intSelectorType = Val(strSelectorValue)
                    Session("MovesPage_SelectorValue") = Me.intSelectorType
                ElseIf Session("MovesPage_SelectorValue") IsNot Nothing Then
                    Me.intSelectorType = Val(Session("MovesPage_SelectorValue"))
                End If

                If bolReload OrElse tb Is Nothing OrElse Not intLastSelectorType.HasValue OrElse intLastSelectorType.Value <> Me.intSelectorType Then
                    Dim _IDGroup As Integer = Me.IdGroup
                    Dim strAction As String = roTypes.Any2String(Request.QueryString("action"))
                    If strAction <> String.Empty Then

                        Dim _IDEmployee As String = String.Empty
                        If Me.IDEmployeePageInitial > 0 Then
                            _IDEmployee = Me.IDEmployeePageInitial.ToString
                        End If

                        Select Case strAction
                            Case "notjustifiedDays"
                                Me.lblSubtitle.Text = Me.Language.Translate("Selector.NotJustifiedDays", Me.DefaultScope)
                                dv = LoadIncompleteMovesSelector(0, EmployeeFilter)
                            Case "incompletedDays"
                                Me.lblSubtitle.Text = Me.Language.Translate("Selector.IncompletedDays", Me.DefaultScope)
                                dv = LoadIncompleteIncidencesSelector(0, EmployeeFilter)
                            Case "notreliabledDays"
                                Me.lblSubtitle.Text = Me.Language.Translate("Selector.NotReliabledDays", Me.DefaultScope)
                                dv = Me.LoadSuspiciousMovesSelector(0, EmployeeFilter)
                            Case Else
                                Me.lblSubtitle.Text = ""
                                dv = Me.LoadEmployeesSelector(_IDGroup, EmployeeFilter)
                        End Select
                    Else
                        Me.lblSubtitle.Text = ""
                        dv = Me.LoadEmployeesSelector(_IDGroup, EmployeeFilter)
                    End If

                    Session("MovesPage_SelectorData") = dv.ToTable()
                    Session("MovesPage_LastSelectorType") = Me.intSelectorType
                Else
                    dv = New DataView(tb)

                    Dim strAction As String = roTypes.Any2String(Request.QueryString("action"))
                    If strAction <> String.Empty Then
                        Select Case strAction
                            Case "notjustifiedDays", "incompletedDays", "notreliabledDays"
                                dv.Sort = "Name, Date"
                            Case Else
                                dv.Sort = "Path, Name, Date"
                        End Select
                    Else
                        dv.Sort = "Path, Name, Date"
                    End If


                End If
            Catch ex As Exception
                tb = New DataTable

                tb.Columns.Add(New DataColumn("Clave", GetType(String)))
                tb.Columns.Add(New DataColumn("Name", GetType(String)))
                tb.Columns.Add(New DataColumn("Date", GetType(DateTime)))
                tb.Columns.Add(New DataColumn("IDEmployee", GetType(Integer)))

                'añade campo clave a la tabla
                tb.PrimaryKey = New DataColumn() {tb.Columns("Clave")}
                tb.AcceptChanges()

                dv = New DataView(tb)
                dv.Sort = "Name, Date"

                Me.hdnExceptionOccurred.Value = "1"
            End Try

            Return dv
        End Get

        Set(ByVal value As DataView)
            If value IsNot Nothing Then
                Session("MovesPage_SelectorData") = value.ToTable()
            Else
                Session("MovesPage_SelectorData") = Nothing
            End If
        End Set

    End Property

    Private Property SelectorSelectedRowIndex As Integer
        Get

            Dim oVal = roTypes.Any2Integer(Session("MovesPage_SelectorSelectedRowIndex"))

            Try
                'Si mantengo un indice  mayor que el numero de filas lo seteo a la última fila
                If Me.SelectorDataView IsNot Nothing AndAlso oVal >= Me.SelectorDataView.Table.Rows.Count Then
                    oVal = Me.SelectorDataView.Table.Rows.Count - 1
                End If
            Catch ex As Exception
                'Si no puedo calcular el indice seleccionado seteo la primera fila
                oVal = 0
            End Try

            'Si tengo un indice menor que la primera fila lo seteo a la primera fila
            If oVal < 0 Then oVal = 0

            Return oVal
        End Get
        Set(value As Integer)
            Session("MovesPage_SelectorSelectedRowIndex") = value
        End Set
    End Property

#End Region

#Region "Shift Properties functions"

    Private Property DailyScheduleDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("MovesPage_DailyScheduleData")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_DailyScheduleLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = Session("MovesPage_DailyScheduleLastDate")

            If bolReload OrElse tb Is Nothing OrElse Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
               Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DatePage Then

                tb = API.EmployeeServiceMethods.GetPlan(Me.Page, Me.IDEmployeePage, Me.DatePage)

                Session("MovesPage_DailyScheduleData") = tb
                Session("MovesPage_DailyScheduleLastIDEmployee") = Me.IDEmployeePage
                Session("MovesPage_DailyScheduleLastDate") = Me.DatePage

            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("MovesPage_DailyScheduleData") = value
        End Set

    End Property

#End Region

#Region "Properties Absences functions"

    Private ReadOnly Property AbsenceDataMessage(Optional ByVal bolReload As Boolean = False) As String
        Get

            Dim strAbsence As String = Session("MovesPage_AbsenceDataMessage")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_AbsenceLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = Session("MovesPage_AbsenceLastDate")

            If bolReload OrElse strAbsence Is Nothing OrElse Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
                                Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DatePage Then

                Try
                    Me.btnAddNewAbsence.Visible = True

                    Dim tb As DataTable
                    tb = API.EmployeeServiceMethods.GetEmployeeForecastsInPeriod(Me.Page, Me.IDEmployeePage, Me.DatePage)

                    Dim absencesDS As New Generic.List(Of AbsenceDetailObject)

                    ' Añadir la columna con el literal que define la ausencia
                    If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                        Dim Params As New Generic.List(Of String)
                        Params.Add(tb.Rows.Count)
                        If tb.Rows.Count > 1 Then strAbsence = Me.Language.Translate("ProgrammedAbsence.Counts", Me.DefaultScope, Params)
                        If tb.Rows.Count = 1 Then strAbsence = Me.Language.Translate("ProgrammedAbsence.Count", Me.DefaultScope, Params)

                        For Each oRow In tb.Rows
                            Dim oTmp As New AbsenceDetailObject With {
                                .AbsenceID = roTypes.Any2Integer(oRow("AbsenceID")),
                                .IDEmployee = roTypes.Any2Integer(oRow("IDEmployee")),
                                .AbsenceType = roTypes.Any2String(oRow("AbsenceType")),
                                .BeginDate = roTypes.Any2DateTime(oRow("BeginDate")),
                                .EndDate = roTypes.Any2DateTime(oRow("EndDate")),
                                .BeginTime = roTypes.Any2DateTime(oRow("BeginTime")),
                                .EndTime = roTypes.Any2DateTime(oRow("EndTime")),
                                .Duration = roTypes.Any2Double(oRow("Duration")),
                                .IDCause = roTypes.Any2String(oRow("IDCause")),
                                .Description = roTypes.Any2String(oRow("Description"))
                                }
                            absencesDS.Add(oTmp)
                        Next

                        For Each oAbsence As AbsenceDetailObject In absencesDS

                            Dim desc As String = String.Empty
                            Select Case oAbsence.AbsenceType
                                Case "ProgrammedAbsence"
                                    Params = New Generic.List(Of String)
                                    Params.Add(oAbsence.BeginDate.ToShortDateString)
                                    Params.Add(oAbsence.EndDate.ToShortDateString)
                                    Params.Add(oAbsence.IDCause)
                                    If oAbsence.Description <> "" Then
                                        Params.Add("(" & oAbsence.Description & ")")
                                    Else
                                        Params.Add(oAbsence.Description)
                                    End If

                                    desc = Me.Language.Translate("ProgrammedAbsence.Literal", Me.DefaultScope, Params)
                                Case "ProgrammedCause"
                                    Params = New Generic.List(Of String)
                                    Params.Add(oAbsence.BeginDate.ToShortDateString)
                                    Params.Add(oAbsence.EndDate.ToShortDateString)

                                    Params.Add(CDate(roTypes.Any2Time(oAbsence.Duration).Value).ToShortTimeString)
                                    Params.Add(oAbsence.IDCause)
                                    If oAbsence.Description <> "" Then
                                        Params.Add("(" & oAbsence.Description & ")")
                                    Else
                                        Params.Add(oAbsence.Description)
                                    End If

                                    Params.Add(oAbsence.BeginTime.ToShortTimeString)

                                    Params.Add(oAbsence.EndTime.ToShortTimeString)

                                    desc = Me.Language.Translate("ProgrammedCause.Literal", Me.DefaultScope, Params)
                                Case "ProgrammedOverTime"
                                    Params = New Generic.List(Of String)

                                    Dim bOneDay As Boolean = True
                                    Params.Add(oAbsence.BeginDate.ToShortDateString())

                                    If oAbsence.BeginDate <> oAbsence.EndDate Then
                                        Params.Add(oAbsence.EndDate.ToShortDateString())
                                        bOneDay = False
                                    End If

                                    Params.Add(CDate(roTypes.Any2Time(oAbsence.Duration).Value).ToShortTimeString)
                                    Params.Add(oAbsence.IDCause)
                                    Params.Add(oAbsence.Description)
                                    Params.Add(oAbsence.BeginTime.ToShortTimeString())
                                    Params.Add(oAbsence.EndTime.ToShortTimeString())
                                    If bOneDay Then
                                        desc = Me.Language.Translate("ProgrammedOvertimes.OneDayLiteral", Me.DefaultScope, Params)
                                    Else
                                        desc = Me.Language.Translate("ProgrammedOvertimes.MultipleDaysLiteral", Me.DefaultScope, Params)
                                    End If
                                Case "ProgrammedHoliday"
                                    Params = New Generic.List(Of String)
                                    Params.Add(oAbsence.BeginDate.ToShortDateString())

                                    Params.Add(CDate(roTypes.Any2Time(oAbsence.Duration).Value).ToShortTimeString)
                                    Params.Add(oAbsence.IDCause)
                                    Params.Add(oAbsence.Description)
                                    Params.Add(oAbsence.BeginTime.ToShortTimeString())
                                    Params.Add(oAbsence.EndTime.ToShortTimeString())
                                    If oAbsence.Duration = 0 Then
                                        desc = Me.Language.Translate("ProgrammedHolidays.AllDayLiteral", Me.DefaultScope, Params)
                                    Else
                                        desc = Me.Language.Translate("ProgrammedHolidays.Literal", Me.DefaultScope, Params)
                                    End If

                            End Select

                            oAbsence.UserDescription = desc
                        Next
                    Else
                        Dim Params As New Generic.List(Of String)
                        strAbsence = Me.Language.Translate("ProgrammedAbsence.None", Me.DefaultScope, Params)
                    End If

                    If Me.hdnAbsencesInfo.Contains("cpAbsencesData") Then
                        Me.hdnAbsencesInfo("cpAbsencesData") = roJSONHelper.SerializeNewtonSoft(absencesDS.ToArray())
                    Else
                        Me.hdnAbsencesInfo.Add("cpAbsencesData", roJSONHelper.SerializeNewtonSoft(absencesDS.ToArray()))
                    End If
                Catch ex As Exception
                    strAbsence = ""
                End Try

                Session("MovesPage_AbsenceDataMessage") = strAbsence
                Session("MovesPage_AbsenceLastIDEmployee") = Me.IDEmployeePage
                Session("MovesPage_AbsenceLastDate") = Me.DatePage

            End If

            If Not (Me.CurrentPermission >= Permission.Write) Then
                Me.btnAddNewAbsence.Visible = False
            End If

            Return strAbsence

        End Get
    End Property

#End Region

#Region "Remarks Properties functions"

    Private Property RemarkDataMessage(Optional ByVal bolReload As Boolean = False) As String
        Get

            Dim strRemark As String = Session("MovesPage_RemarkData")
            Dim intLastIDEmployee As Nullable(Of Integer) = Session("MovesPage_RemarkLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = Session("MovesPage_RemarkLastDate")

            If bolReload OrElse strRemark Is Nothing OrElse Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployeePage OrElse
                                Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DatePage Then

                strRemark = API.EmployeeServiceMethods.GetRemarkText(Me.Page, Me.IDEmployeePage, Me.DatePage)

                Session("MovesPage_RemarkData") = strRemark
                Session("MovesPage_RemarkLastIDEmployee") = Me.IDEmployeePage
                Session("MovesPage_RemarkLastDate") = Me.DatePage

            End If

            Return strRemark

        End Get

        Set(ByVal value As String)
            Session("MovesPage_RemarkData") = value
        End Set

    End Property

#End Region

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmMovesNew.Load

        Me.InsertExtraJavascript("MovesDevExpress", "~/Scheduler/Scripts/MovesDevExpress.js")
        Me.InsertExtraJavascript("MovesExtended", "~/Scheduler/Scripts/MovesExtended.js")
        Me.InsertExtraJavascript("MovesDetail", "~/Scheduler/Scripts/MovesDetail.js")

        'AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

        If Not IsPostBack AndAlso Not IsCallback Then
            Me.EmptySession()
        End If

        bolFeatureAnyWhere = True
        bolFeatureTasks = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv")
        bolFeatureDiningRoom = HelperSession.GetFeatureIsInstalledFromApplication("Forms\DiningRoom")
        FeatureCostCenter = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

        Me.TaskPunchesPermission = Me.GetFeaturePermission("Tasks.Punches")
        Me.DiningRoomPunchesPermission = Me.GetFeaturePermission("DiningRoom.Punches")
        Me.CostCenterPermission = Me.GetFeaturePermission("BusinessCenters.Punches")
        If Not FeatureCostCenter Then
            Me.CostCenterPermission = Permission.None
        End If

        bolBusinessGroupApplyNotAllowedModifyCause = False
        If roTypes.Any2String(HelperSession.AdvancedParametersCache("BusinessGroup.ApplyNotAllowedModifyCause")) = "1" Then
            ' No dejamos modificar asignaciones de justificaciones que el supervisor no pueda gestionar por grupos de negocio
            bolBusinessGroupApplyNotAllowedModifyCause = True
        End If

        If Not IsPostBack AndAlso Not IsCallback Then
            If Not Me.HasFeaturePermission("Calendar.Punches.Punches", Permission.Read) Then
                WLHelperWeb.RedirectAccessDenied(True)
                Exit Sub
            End If

            'Combo de Vistas
            Me.LoadViewsCombo()
            Dim tmpDate As DateTime = Me.FreezingDatePage(True)

            Me.HasAction = (Request.QueryString("action") IsNot Nothing)

            CreateColumnsMoves()
            CreateColumnsIncidences()
            CreateColumnsAcum()
            CreateColumnsSelector()

            'Inicializa variables
            Me.IDEmployeePage = -1
            Me.DatePage = Nothing
            Me.IdGroup = -1
            Me.IdFilteredTask = -1
            Me.BusinessCentersDataTable = Nothing
            Me.BusinessCentersDetailDataTable = Nothing

            'Id de Empleeado recibido
            Dim tmpIdTask As Integer = roTypes.Any2Integer(Request.Params("TaskFilterID"))
            If tmpIdTask > 0 Then
                Me.IdFilteredTask = tmpIdTask
            End If

            'Id de Grupo recibido
            Dim tmpIdGroup As Integer = roTypes.Any2Integer(Request.Params("GroupID"))
            If tmpIdGroup > 0 Then
                Me.IdGroup = tmpIdGroup
            End If

            'Id de Empleeado recibido
            Me.IDEmployeePageInitial = roTypes.Any2Integer(Request.Params("EmployeeID"))

            If (Me.IdFilteredTask > 0) Then
                Me.ViewPage = ViewPageTypes.Tasks
            Else
                Me.ViewPage = ViewPageTypes.Moves
            End If

            If Me.ViewPage = ViewPageTypes.Tasks Then
                Me.GridMoves.Columns("LocationZone").Visible = True
                Me.GridMoves.Columns("IDTerminalUnbound").Visible = False
            Else
                Me.GridMoves.Columns("LocationZone").Visible = False
                Me.GridMoves.Columns("IDTerminalUnbound").Visible = True
            End If

            If Not Me.HasFeaturePermission("Employees.UserFields.Information.High", Permission.Read) Then
                Me.divCaptureRights.Style("display") = "none"
            Else
                Me.divCaptureRights.Style("display") = ""
            End If

            GridMoves.SettingsLoadingPanel.Text = Me.Language.Translate("SettingsLoadingPanel.Text", Me.DefaultScope)
            GridIncidences.SettingsLoadingPanel.Text = Me.Language.Translate("SettingsLoadingPanel.Text", Me.DefaultScope)
            GridAcum.SettingsLoadingPanel.Text = Me.Language.Translate("SettingsLoadingPanel.Text", Me.DefaultScope)
            CallbackPanelShift.SettingsLoadingPanel.Text = Me.Language.Translate("SettingsLoadingPanel.Text", Me.DefaultScope)
            CallbackPanelRemarks.SettingsLoadingPanel.Text = Me.Language.Translate("SettingsLoadingPanel.Text", Me.DefaultScope)
            CallbackPanelAbsence.SettingsLoadingPanel.Text = Me.Language.Translate("SettingsLoadingPanel.Text", Me.DefaultScope)
            LoadingPanelDetails.Text = Me.Language.Translate("Details.SettingsLoadingPanel.Text", Me.DefaultScope)
        End If

        'If GridIncidences.IsCallback Then
        BindGridIncidences(False, True)
        'End If

        'If GridMoves.IsCallback Then
        BindGridMoves(False, True)
        'End If

        'If Not IsCallback Then
        '    If Me.ViewPage = ViewPageTypes.Mobility Then
        '        Me.tbHtmlLocalization.Style("display") = ""
        '        Me.tbHtmlIncidences.Style("display") = "none"
        '        Me.trHtmlRemarks.Style("display") = "none"
        '    Else
        '        Me.tbHtmlLocalization.Style("display") = "none"
        '        Me.tbHtmlIncidences.Style("display") = ""
        '        Me.trHtmlRemarks.Style("display") = ""
        '    End If
        'End If

    End Sub

    Protected Sub ASPxMovesNewPanel_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxMovesNewPanel.Callback
        Dim strParameter As String = Server.UrlDecode(e.Parameter())

        If strParameter = "INITIALLOAD" Then
            SetcontrolsVisibility()
            initialMovesNewLoad()
        ElseIf (strParameter = "RELOADDATA") Then
            SetcontrolsVisibility()
            LoadData(True, False)
        ElseIf (strParameter = "NAVIGATETOPREVIOUSDATE") Then
            SetcontrolsVisibility()
            navigateToPreviousDate()
        ElseIf (strParameter = "NAVIGATETONEXTDATE") Then
            SetcontrolsVisibility()
            navigateToNextDate()
        ElseIf (strParameter = "NAVIGATETOPREVIOUSEMPLOYEE") Then
            SetcontrolsVisibility()
            navigateToPreviousEmployee()
        ElseIf (strParameter = "NAVIGATETONEXTEMPLOYEE") Then
            SetcontrolsVisibility()
            navigateToNextEmployee()
        ElseIf (strParameter = "NAVIGATETOPREVIOUSSELECTOR") Then
            SetcontrolsVisibility()
            navigateToPreviousSelector()
        ElseIf (strParameter = "NAVIGATETONEXTSELECTOR") Then
            SetcontrolsVisibility()
            navigateToNextSelector()
        ElseIf (strParameter = "NAVIGATETOSELECTORCHOICE") Then
            SetcontrolsVisibility()
            navigateToSelectorChoice()
        ElseIf (strParameter = "CHANGEDATE") Then
            SetcontrolsVisibility()
            navigateToSelectedDate()
        End If

        ASPxMovesNewPanel.JSProperties.Add("cpActionRO", "strParameter")
    End Sub

    Protected Sub navigateToSelectorChoice()
        ChangeSelector(False, False)
    End Sub

    Protected Sub navigateToPreviousEmployee()
        If Not Me.EditingData() Then
            Me.PreviousEmployee(True)
        End If
    End Sub

    Protected Sub navigateToNextEmployee()
        If Not Me.EditingData() Then
            Me.NextEmployee(True)
        End If
    End Sub

    Protected Sub navigateToNextSelector()
        If Not Me.EditingData() Then
            Dim oldRow As Integer = Me.GridSelectorRowSel
            SelectorSelectedRowIndex = oldRow

            Dim intRow = GridSelectorGetNextRowIndex()
            GridSelector.FocusedRowIndex = intRow

            If intRow > -1 AndAlso oldRow <> intRow Then
                Me.ChangeSelector(True, True)
            Else
                LoadData(False, False)
            End If
        End If
    End Sub

    Protected Sub navigateToPreviousSelector()
        If Not Me.EditingData() Then
            Dim oldRow As Integer = Me.GridSelectorRowSel
            SelectorSelectedRowIndex = oldRow

            Dim intRow As Integer = GridSelectorGetPreviousRowIndex()
            GridSelector.FocusedRowIndex = intRow

            If intRow > -1 AndAlso oldRow <> intRow Then
                Me.ChangeSelector(True, True)
            Else
                LoadData(False, False)
            End If
        End If
    End Sub

    Protected Sub navigateToPreviousDate()
        Me.PreviousDate(True)
    End Sub

    Protected Sub navigateToNextDate()
        Me.NextDate(True)
    End Sub

    Protected Sub navigateToSelectedDate()
        Me.ChangeDate(True)
    End Sub

    Private Sub SetcontrolsVisibility()

        If Me.ViewPage = ViewPageTypes.Mobility Then
            Me.tbHtmlLocalization.Style("display") = ""
            Me.tbHtmlIncidences.Style("display") = "none"
            Me.trHtmlRemarks.Style("display") = "none"
        Else
            Me.tbHtmlLocalization.Style("display") = "none"
            Me.tbHtmlIncidences.Style("display") = ""
            Me.trHtmlRemarks.Style("display") = ""
        End If

        ' Botones de navegacion
        Me.ibtNextDate.Visible = Not Me.HasAction
        Me.ibtPreviousDate.Visible = Not Me.HasAction

        Me.txtDatePage.Visible = Not Me.HasAction

        Me.txtDate2.Visible = Me.HasAction
        Me.pnlSelector.Visible = Me.HasAction
        Me.cmbView.Visible = Not Me.HasAction

        Me.lblSubtitle.Visible = Me.HasAction

        Me.cmbView.Enabled = (Me.IdFilteredTask = -1)
        Me.txtDatePage.Enabled = (Me.IdFilteredTask = -1)
        Me.ibtNextEmployee.Visible = (Not Me.HasAction AndAlso Me.IdFilteredTask = -1)
        Me.ibtPreviousEmployee.Visible = (Not Me.HasAction AndAlso Me.IdFilteredTask = -1)

        If Me.GridSelector.VisibleRowCount = 1 Then
            Me.ibtNextEmployee.Visible = False
            Me.ibtPreviousEmployee.Visible = False
        Else
            Me.ibtNextEmployee.Visible = True
            Me.ibtPreviousEmployee.Visible = True
        End If

        If Me.HasAction Then
            Dim strAction As String = roTypes.Any2String(Request.QueryString("action"))
            Select Case strAction
                Case "notjustifiedDays"
                    Me.lblSubtitle.Text = Me.Language.Translate("Selector.NotJustifiedDays", Me.DefaultScope)
                Case "incompletedDays"
                    Me.lblSubtitle.Text = Me.Language.Translate("Selector.IncompletedDays", Me.DefaultScope)
                Case "notreliabledDays"
                    Me.lblSubtitle.Text = Me.Language.Translate("Selector.NotReliabledDays", Me.DefaultScope)
                Case Else
                    Me.lblSubtitle.Text = ""
            End Select

        End If
    End Sub

    Private Sub initialMovesNewLoad()

        If Not Me.HasAction Then
            Dim selDate As Object = Nothing
            Dim strDate As String = Request.Params("Date") ' En formato 'dd/MM/yyyy'
            If strDate <> String.Empty Then
                selDate = New Date(strDate.Split("/")(2), strDate.Split("/")(1), strDate.Split("/")(0))
            Else
                selDate = Date.Now
                If Me.IDEmployeePageInitial > 0 AndAlso Me.IdFilteredTask > 0 Then
                    Dim tmpDate As DateTime = API.TasksServiceMethods.GetNextTaskPunchDate(Me.Page, Me.IdFilteredTask, Me.IDEmployeePageInitial, New DateTime(1970, 1, 1, 0, 0, 0))
                    selDate = New Date(tmpDate.Year, tmpDate.Month, tmpDate.Day)
                End If

            End If

            Session("MovesPage_DatePage") = selDate
        End If

        If Me.IDEmployeePageInitial > 0 Then
            Me.IDEmployeePage = Me.IDEmployeePageInitial
        End If

        If Not Me.HasAction Then
            Dim strDate As String = Request.Params("Date") ' En formato 'dd/MM/yyyy'
            If strDate <> String.Empty Then
                Me.DatePage = New Date(strDate.Split("/")(2), strDate.Split("/")(1), strDate.Split("/")(0))
            Else
                Me.DatePage = Date.Now
                If Me.IDEmployeePageInitial > 0 AndAlso Me.IdFilteredTask > 0 Then
                    Dim tmpDate As DateTime = API.TasksServiceMethods.GetNextTaskPunchDate(Me.Page, Me.IdFilteredTask, Me.IDEmployeePageInitial, New DateTime(1970, 1, 1, 0, 0, 0))
                    Me.DatePage = New Date(tmpDate.Year, tmpDate.Month, tmpDate.Day)
                End If

            End If
        End If

        If Me.HasAction Then
            Me.CalculateDaysPeriods()
        Else
            Me.GetDatesPeriod()
        End If

        If Request.QueryString("CalendarV2") IsNot Nothing AndAlso Request.QueryString("CalendarV2").ToString().Equals("1") Then
            EmployeeFilter = Request.QueryString("EmpFilter").ToString()
        Else
            EmployeeFilter = String.Empty
        End If

        Me.BindGridSelector(True)
        GridSelector.DataBind()

        If Me.GridSelector.VisibleRowCount = 1 Then
            Me.ibtNextEmployee.Visible = False
            Me.ibtPreviousEmployee.Visible = False
        Else
            Me.ibtNextEmployee.Visible = True
            Me.ibtPreviousEmployee.Visible = True
        End If

        Me.GridSelectorRowSel = SituarSelector()
        Me.SelectorSelectedRowIndex = Me.GridSelectorRowSel

        If Me.HasAction Then
            If Me.GridSelectorRowIDEmployee(0, False) > 0 Then
                Me.GridSelectorRowSel = 0
                Me.SelectorSelectedRowIndex = 0
                Me.DatePage = Me.GridSelectorRowDate(0, False)
                Me.IDEmployeePage = Me.GridSelectorRowIDEmployee(0, False)
            Else
                Me.IDEmployeePage = -1
                Me.DatePage = DateTime.Today
            End If
        End If

        'Comprobar si el selector tiene registros
        If Me.SelectorDataView.Count = 0 Then
            HideContent()
        Else
            GridSelector.FocusedRowIndex = 0
            Me.LoadData(True, False)
        End If
    End Sub

    Private Sub HideContent()
        btnAply.Visible = False
        btnUndo.Visible = False
        divContenido.Visible = False
        divError.Visible = True
        lblError.Text = Me.Language.Translate("NoRecordsFound.Message", Me.DefaultScope)
    End Sub

    Private Sub LoadData(Optional ByVal bolReload As Boolean = False, Optional ByVal bolReloadSelector As Boolean = True)
        Try
            'Añadimos esta comprobación para validar que la sesión no ha caducado y provocar que cuando modifican algo explicitamente en la pantalla esta se actualice
            If WLHelperWeb.CurrentPassport IsNot Nothing Then
                Me.IDEmployeePage = Me.IDEmployeePage
                Me.DatePage = Me.DatePage

                Me.BindGridMoves(bolReload)

                Me.BindGridAcum(bolReload)
                GridAcum.DataBind()

                Me.BindGridSelector((bolReload AndAlso bolReloadSelector))
                GridSelector.DataBind()

                Me.LoadShiftUsed(bolReload)

                Me.LoadAbsence(bolReload)

                If Me.ViewPage = ViewPageTypes.Mobility Then

                    Me.LoadLocalizationData()
                Else
                    Me.BindGridIncidences(bolReload)
                    Me.LoadRemarks(bolReload)
                End If

                'Auditoría
                Audit()
            End If
        Catch ex As Exception
            Me.hdnExceptionOccurred.Value = "1"
        End Try

    End Sub

#Region "BIND GRIDS"

    Private Sub BindGridMoves(ByVal bolReload As Boolean, Optional ByVal bInitialLoad As Boolean = False)
        Me.GridMoves.DataSource = Me.PunchesDataView(bolReload, bInitialLoad)
        Me.GridMoves.DataBind()
    End Sub

    Private Sub BindGridIncidences(ByVal bolReload As Boolean, Optional ByVal bInitialLoad As Boolean = False)
        Me.GridIncidences.DataSource = Me.IncidencesDataView(bolReload, bInitialLoad)
        Me.GridIncidences.DataBind()
    End Sub

    Private Sub BindGridAcum(ByVal bolReload As Boolean)
        If bolReload Then
            Dim dv As DataView
            dv = Me.AcumAnualDataView(True)
            dv = Me.AcumDailyDataView(True)
        End If

        If Me.ViewAcumPage = "ANUAL" Then
            Me.GridAcum.DataSource = Me.AcumAnualDataView(False)
        Else
            Me.GridAcum.DataSource = Me.AcumDailyDataView(False)
        End If
        'NO HACER DATABIND AQUI!! --> Me.GridAcum.DataBind()
    End Sub

    Private Sub BindGridSelector(ByVal bolReload As Boolean)
        Me.GridSelector.DataSource = Me.SelectorDataView(bolReload)
        'NO HACER DATABIND AQUI!! --> Me.GridSelector.DataBind()
    End Sub

#End Region

#Region "------REGION COLUMNAS CUATRO GRIDS---------"

    Private Sub CreateColumnsSelector()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridSelector.Columns.Clear()
        Me.GridSelector.KeyFieldName = "Clave"
        Me.GridSelector.SettingsText.EmptyDataRow = " "
        Me.GridSelector.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
        Me.GridSelector.Settings.VerticalScrollableHeight = 200
        Me.GridSelector.Settings.VerticalScrollBarMode = ScrollBarMode.Visible

        'Me.GridSelector.KeyboardSupport = True
        'Me.GridSelector.Settings.ShowFilterRow = False
        'Me.GridSelector.Settings.ShowFooter = False
        'Me.GridSelector.Settings.UseFixedTableLayout = True
        'Me.GridSelector.Settings.ShowVerticalScrollBar = True
        'Me.GridSelector.SettingsBehavior.AllowSelectSingleRowOnly = True
        'Me.GridSelector.SettingsBehavior.AllowFocusedRow = True
        'Me.GridSelector.SettingsBehavior.AllowSort = False
        'Me.GridSelector.SettingsPager.PageSize = 6
        'Me.GridSelector.SettingsPager.ShowEmptyDataRows = True
        'Me.GridSelector.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
        'Me.GridSelector.Styles.Cell.Wrap = DevExpress.Utils.DefaultBoolean.False

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Clave"
        GridColumn.FieldName = "Clave"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSelector.Columns.Add(GridColumn)

        'Name Employee
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridSelector.Column.Name", DefaultScope) '"Nombre"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 110
        Me.GridSelector.Columns.Add(GridColumn)

        'Date
        'FechaFin y hora fin
        GridColumnDate = New GridViewDataDateColumn
        GridColumnDate.Caption = Me.Language.Translate("GridSelector.Column.Date", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "Date"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnDate.Width = 45
        Me.GridSelector.Columns.Add(GridColumnDate)

        'IDEmployee
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDEmployee"
        GridColumn.FieldName = "IDEmployee"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridSelector.Columns.Add(GridColumn)

    End Sub

    Private Sub CreateColumnsAcum()

        Dim GridColumn As GridViewDataTextColumn

        Dim VisibleIndex As Integer = 0

        Me.GridAcum.Columns.Clear()
        If Me.ViewPage = ViewPageTypes.Tasks Then
            Me.GridAcum.KeyFieldName = "IDTask"
        Else
            Me.GridAcum.KeyFieldName = "IDConcept"
        End If

        Me.GridAcum.SettingsText.EmptyDataRow = " "

        'Me.GridAcum.KeyboardSupport = True
        'Me.GridAcum.Settings.ShowFilterRow = False
        'Me.GridAcum.Settings.ShowFooter = False
        'Me.GridAcum.Settings.UseFixedTableLayout = True
        'Me.GridAcum.Settings.ShowVerticalScrollBar = True
        'Me.GridAcum.Settings.VerticalScrollableHeight = 200
        'Me.GridAcum.SettingsBehavior.AllowSelectSingleRowOnly = True
        'Me.GridAcum.SettingsBehavior.AllowFocusedRow = True
        'Me.GridAcum.SettingsBehavior.AllowSort = False
        'Me.GridAcum.SettingsPager.PageSize = 6
        'Me.GridAcum.SettingsPager.ShowEmptyDataRows = True
        'Me.GridAcum.SettingsPager.Mode = GridViewPagerMode.ShowAllRecords
        'Me.GridAcum.Styles.Cell.Wrap = DevExpress.Utils.DefaultBoolean.False

        'IDConcept
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDConcept"
        GridColumn.FieldName = "IDConcept"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridAcum.Columns.Add(GridColumn)

        'Name
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridAcum.Column.Name", DefaultScope) '"Name"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 59
        Me.GridAcum.Columns.Add(GridColumn)

        'Type Concept (anual o mensual)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridAcum.Column.Periodicidad", DefaultScope) '"Period"
        GridColumn.FieldName = "Periodicidad"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 16
        Me.GridAcum.Columns.Add(GridColumn)

        'Total
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridAcum.Column.Total", DefaultScope) '"Total"
        GridColumn.FieldName = "TotalFormatted" '"Total"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 20
        Me.GridAcum.Columns.Add(GridColumn)

    End Sub

    Private Sub CreateColumnsIncidences()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridComboCommand As GridViewDataComboBoxColumn
        Dim GridColumnTime As GridViewDataTimeEditColumn

        Dim VisibleIndex As Integer = 0

        Me.GridIncidences.Columns.Clear()
        Me.GridIncidences.KeyFieldName = "Clave"
        Me.GridIncidences.SettingsText.EmptyDataRow = " "

        Me.GridIncidences.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        Me.GridIncidences.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        Me.GridIncidences.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        Me.GridIncidences.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        Me.GridIncidences.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        Me.GridIncidences.SettingsEditing.Mode = GridViewEditingMode.Inline

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 15
        VisibleIndex = VisibleIndex + 1

        Me.GridIncidences.Columns.Add(GridColumnCommand)

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Clave"
        GridColumn.FieldName = "Clave"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.GridIncidences.Columns.Add(GridColumn)

        'Incidencia
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Incidence", DefaultScope) '"Incidencia"
        GridColumn.FieldName = "TextoIncidencia"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 38
        Me.GridIncidences.Columns.Add(GridColumn)

        'Zona Horaria
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.TimeZoneName", DefaultScope) '"Zona Horaria"
        GridColumn.FieldName = "TimeZoneName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 32
        Me.GridIncidences.Columns.Add(GridColumn)

        'Inicio
        GridColumnTime = New GridViewDataTimeEditColumn
        GridColumnTime.Caption = Me.Language.Translate("GridIncidences.Column.BeginTime", DefaultScope) '"Inicio"
        GridColumnTime.FieldName = "BeginTime"
        GridColumnTime.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnTime.ReadOnly = True
        GridColumnTime.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.Width = 12
        Me.GridIncidences.Columns.Add(GridColumnTime)

        'Final
        GridColumnTime = New GridViewDataTimeEditColumn
        GridColumnTime.Caption = Me.Language.Translate("GridIncidences.Column.EndTime", DefaultScope) '"Final"
        GridColumnTime.FieldName = "EndTime"
        GridColumnTime.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnTime.ReadOnly = True
        GridColumnTime.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnTime.Width = 12
        Me.GridIncidences.Columns.Add(GridColumnTime)

        'Valor Hora NO Editable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.ValueHora", DefaultScope) '"Valor"
        GridColumn.FieldName = "ValueHora"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 12
        Me.GridIncidences.Columns.Add(GridColumn)

        'Justificación
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridIncidences.Column.Justification", DefaultScope) '"Justificación"
        GridComboCommand.FieldName = "IDCause"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "ID"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Short)
        GridComboCommand.PropertiesComboBox.DataSource = Me.GetJustCausesDataAll()
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.Width = 42
        GridComboCommand.ReadOnly = False
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        Me.GridIncidences.Columns.Add(GridComboCommand)

        'Valor Hora Editable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.ValueHoraEditable", DefaultScope) '"Valor"
        GridColumn.FieldName = "ValueHoraEditable"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 12
        'GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-9999..9999>:<00..59>" 'Quitamos la mascara porque la añadimos más adelante dependiendo del tipo de justificación (horas, días o personalizado)
        GridColumn.PropertiesTextEdit.Width = 60
        Me.GridIncidences.Columns.Add(GridColumn)

        'Centro de Coste
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridIncidences.Column.CostCenter", DefaultScope) '"Centro de coste"
        GridComboCommand.FieldName = "IDCenter"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "ID"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Short)
        GridComboCommand.PropertiesComboBox.DataSource = Me.BusinessCentersDataTable()
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.Width = 38
        GridComboCommand.ReadOnly = Me.CostCenterPermission <= Permission.Read
        GridComboCommand.Visible = Me.CostCenterPermission >= Permission.Read AndAlso FeatureCostCenter
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        Me.GridIncidences.Columns.Add(GridComboCommand)

        'Calculado
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridIncidences.Column.Calculado", DefaultScope)
        GridColumn.FieldName = "Calculado"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 24
        Me.GridIncidences.Columns.Add(GridColumn)

        'IDRelatedIncidence
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDRelatedIncidence"
        GridColumn.FieldName = "IDRelatedIncidence"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridIncidences.Columns.Add(GridColumn)

    End Sub

    Private Sub CreateColumnsMoves()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnImage As GridViewDataImageColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim CustomButton As GridViewCommandColumnCustomButton

        Dim VisibleIndex As Integer = 0

        Me.GridMoves.Columns.Clear()
        Me.GridMoves.KeyFieldName = "Clave"
        Me.GridMoves.SettingsText.EmptyDataRow = " "
        Me.GridMoves.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        Me.GridMoves.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        Me.GridMoves.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        Me.GridMoves.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        Me.GridMoves.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        'Me.GridMoves.ClientSideEvents.CustomButtonClick = "GridMoves_ButtonClick"
        Me.GridMoves.ClientSideEvents.RowDblClick = "GridMoves_RowDblClick"

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 10
        VisibleIndex = VisibleIndex + 1

        Me.GridMoves.Columns.Add(GridColumnCommand)

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Clave"
        GridColumn.FieldName = "Clave"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridMoves.Columns.Add(GridColumn)

        'Order
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = " "
        GridColumn.FieldName = "Order"
        'GridColumn.VisibleIndex = VisibleIndex
        'VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 6
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        Me.GridMoves.Columns.Add(GridColumn)

        'Photo
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "MaskPhoto"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'Photo
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "TemperaturePhoto"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'Photo
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "SourcePhoto"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'Photo
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "ImgPhoto"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'TODO: cambiar la key de "declaracionjornada"
        'declaracionjornada
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "ImgDeclaracionJornada"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 10
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'Hora (Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridMoves.Column.DateTime", DefaultScope) '"Hora"
        GridColumn.FieldName = "DateTimeUnbound"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 20
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.GridMoves.Columns.Add(GridColumn)

        'Tipo
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("GridMoves.Column.ImgType", DefaultScope) '"Tipo"
        GridColumnImage.FieldName = "ImgType"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 12
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
            'Telecommuting
            GridColumnImage = New GridViewDataImageColumn
            GridColumnImage.Caption = Me.Language.Translate("GridMoves.Column.Telecommuting", DefaultScope) '"Tipo"
            GridColumnImage.FieldName = "ImgTelecommuting"
            GridColumnImage.VisibleIndex = VisibleIndex
            VisibleIndex = VisibleIndex + 1
            GridColumnImage.ReadOnly = True
            GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnImage.Width = 12
            GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
            GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
            GridColumnImage.PropertiesImage.ImageHeight = 16
            GridColumnImage.PropertiesImage.ImageWidth = 16
            GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
            Me.GridMoves.Columns.Add(GridColumnImage)
        End If

        'TypeData
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridMoves.Column.Detalle", DefaultScope) '"Detalle"
        GridColumn.FieldName = "TypeDataUnbound"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 50
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.GridMoves.Columns.Add(GridColumn)

        'IDTerminal (Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridMoves.Column.Terminal", DefaultScope) '"Terminal"
        GridColumn.FieldName = "IDTerminalUnbound"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 45
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.GridMoves.Columns.Add(GridColumn)

        'LocationZone
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridMoves.Column.Location", DefaultScope) '"Location"
        GridColumn.FieldName = "LocationZone"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 40
        Me.GridMoves.Columns.Add(GridColumn)

        'IsNotReliable (Unbound )
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("GridMoves.Column.Fiability", DefaultScope) '"Fiab."
        GridColumnImage.FieldName = "IsNotReliableUnbound"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 12
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'IDPassport (Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridMoves.Column.User", DefaultScope) '"Usuario"
        GridColumn.FieldName = "IDPassportUnbound"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = 35
        Me.GridMoves.Columns.Add(GridColumn)

        'Action (Unbound)
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = Me.Language.Translate("GridMoves.Column.Action", DefaultScope) '"Act."
        GridColumnImage.FieldName = "ActionUnbound"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 12
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "Images/{0}"
        Me.GridMoves.Columns.Add(GridColumnImage)

        'Type
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Type"
        GridColumn.FieldName = "Type"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridMoves.Columns.Add(GridColumn)

        'Link to details (Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "LinkDetails"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.GridMoves.Columns.Add(GridColumn)

        'Columna modificada
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "IsModified"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.Boolean
        Me.GridMoves.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 18
        VisibleIndex = VisibleIndex + 1

        'CustomButton = New GridViewCommandColumnCustomButton()
        'CustomButton.ID = "DeleteEntrieRow"
        'CustomButton.Image.Url = "~/Base/Images/Grid/remove.png"
        'CustomButton.Image.ToolTip = Me.Language.Translate("GridDocumentsTracking.Column.DeleteRow", DefaultScope)
        'CustomButton.Image.Height = New Unit(16)
        'CustomButton.Image.Width = New Unit(16)
        'GridColumnCommand.CustomButtons.Add(CustomButton)

        CustomButton = New GridViewCommandColumnCustomButton()
        CustomButton.ID = "ValidateMove"
        CustomButton.Image.Url = "~/Employees/Images/tick.png"
        CustomButton.Image.ToolTip = Me.Language.Translate("GridMoves.Column.ValidateMove", DefaultScope)
        CustomButton.Image.Height = New Unit(16)
        CustomButton.Image.Width = New Unit(16)
        GridColumnCommand.CustomButtons.Add(CustomButton)

        'Columna oculta con observaciones del fichajes
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "Remarks"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridMoves.Columns.Add(GridColumn)

        'Columna oculta con motivo de no fiabilidad en caso de ser un fichaje no fiable
        GridColumn = New GridViewDataTextColumn()
        GridColumn.FieldName = "NotReliableCause"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.GridMoves.Columns.Add(GridColumn)

        Me.GridMoves.Columns.Add(GridColumnCommand)

    End Sub

#End Region

#Region "GRID MOVES"

    Protected Sub btnAddNewMove_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("btnAddNewMoveText", Me.DefaultScope)
        txtLabel.ToolTip = Me.Language.Translate("btnAddNewMoveTooltip", Me.DefaultScope)

    End Sub

    Protected Sub lblPunchesCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblPunchesCaption", Me.DefaultScope)

        If (Me.IdFilteredTask > 0) Then
            Dim oTask As roTask = API.TasksServiceMethods.GetTaskByID(Me.Page, Me.IdFilteredTask, False)
            If oTask IsNot Nothing Then
                txtLabel.Text = txtLabel.Text & ": " & oTask.Project & " " & oTask.Name
            End If

        End If

    End Sub

    Private Sub GridMoves_CustomButtonInitialize(sender As Object, e As ASPxGridViewCustomButtonEventArgs) Handles GridMoves.CustomButtonInitialize

        Dim row = GridMoves.GetRow(e.VisibleIndex)
        If e.ButtonID = "ValidateMove" AndAlso row IsNot Nothing Then
            Dim rowView = DirectCast(row, System.Data.DataRowView)
            If (rowView.Row("IsNotReliable") = True AndAlso roTypes.Any2Integer(rowView.Row("IDRequest")) = 0) Then
                e.Visible = DevExpress.Utils.DefaultBoolean.True
            Else
                e.Visible = DevExpress.Utils.DefaultBoolean.False
            End If
        End If

    End Sub

    Private Sub GridMoves_HtmlDataCellPrepared(ByVal sender As Object, ByVal e As ASPxGridViewTableDataCellEventArgs) Handles GridMoves.HtmlDataCellPrepared
        If e.DataColumn.FieldName = "IsNotReliableUnbound" Then
            If Not (e.CellValue Is Nothing Or IsDBNull(e.CellValue)) Then
                If roTypes.Any2Boolean(e.GetValue("IsNotReliable")) = True Then
                    If roTypes.Any2String(e.GetValue("NotReliableCause")) IsNot Nothing Then
                        e.Cell.ToolTip = Me.Language.Translate(roTypes.Any2String(e.GetValue("NotReliableCause")), Me.DefaultScope) & ". " & roTypes.Any2String(e.GetValue("Remarks"))
                    Else
                        e.Cell.ToolTip = roTypes.Any2String(e.GetValue("Remarks"))
                    End If
                Else
                    e.Cell.ToolTip = roTypes.Any2String(e.GetValue("Remarks"))
                End If
            End If
        End If
    End Sub

    Private Sub GridMoves_CustomButtonCallback(sender As Object, e As ASPxGridViewCustomButtonCallbackEventArgs) Handles GridMoves.CustomButtonCallback
        Dim row = GridMoves.GetRow(e.VisibleIndex)
        If e.ButtonID = "ValidateMove" AndAlso row IsNot Nothing Then
            If DirectCast(row, System.Data.DataRowView).Row("IsNotReliable") = True Then
                row("IsNotReliable") = False
            Else
                row("IsNotReliable") = True

            End If
            GridMoves.DataBind()
        End If
    End Sub

    Protected Sub GridMoves_CustomUnboundColumnData(ByVal sender As Object, ByVal e As ASPxGridViewColumnDataEventArgs) Handles GridMoves.CustomUnboundColumnData

        If Not e.IsGetData Then Return

        Select Case e.Column.FieldName
            Case "ImgType"
                If Not e.GetListSourceFieldValue("Type") Is Nothing Then
                    Dim imageSource As String = ""
                    Dim accessType As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("Type"))
                    Select Case accessType
                        Case 1, 2
                            imageSource = "Employee"
                        Case 3
                            imageSource = "Automatic"
                        Case 5
                            imageSource = "ValidAccess"
                        Case 6
                            imageSource = "InvalidAccess"

                            If roTypes.Any2Integer(e.GetListSourceFieldValue("InvalidType")) = 20 Then
                                imageSource = "AccessNotConfirmed"
                            End If

                        Case 7
                            imageSource = "Integrated"

                            If roTypes.Any2Integer(e.GetListSourceFieldValue("ActualType")) = 5 Then
                                accessType = 5
                                imageSource = "ValidAccess"
                            End If
                        Case 4
                            imageSource = "Task"
                        Case 10

                            If e.GetListSourceFieldValue("InvalidType") Is DBNull.Value Then
                                imageSource = "DiningRoom"
                            Else
                                imageSource = "InvalidDiningRoom"
                            End If
                        Case 13
                            imageSource = "BusinessCenter"

                        Case Else
                            imageSource = "Alert16"
                    End Select

                    If accessType <> 5 And accessType <> 6 And accessType <> 4 And accessType <> 10 And imageSource <> "Alert16" And accessType <> 13 Then
                        If roTypes.Any2Integer(e.GetListSourceFieldValue("ActualType")) = 1 Then
                            imageSource += "In"
                        Else
                            imageSource += "Out"
                        End If
                    End If

                    e.Value = imageSource & ".png"
                Else
                    e.Value = "Pendiente.png"
                End If

            Case "Order"
                If Me.ViewPage = ViewPageTypes.Mobility AndAlso Me.ShowPunchOrder Then
                    e.Value = e.ListSourceRowIndex + 1
                Else
                    e.Value = ""
                End If
            Case "ImgTelecommuting"
                Try
                    If roTypes.Any2Boolean(e.GetListSourceFieldValue("InTelecommute")) = True Then
                        e.Value = "home16.png"
                    End If
                Catch
                End Try
            Case "ImgDeclaracionJornada"
                Try
                    If Not IsDBNull(e.GetListSourceFieldValue("IDRequest")) AndAlso roTypes.Any2Integer(e.GetListSourceFieldValue("IDRequest")) > 0 Then
                        e.Value = "DailyRecord.GIF"
                    Else
                        e.Value = "empty.png"
                    End If
                Catch
                End Try
            Case "ImgPhoto"
                Try
                    If roTypes.Any2Integer(e.GetListSourceFieldValue("ID")) > 0 Then
                        If roTypes.Any2Boolean(e.GetListSourceFieldValue("HasPhoto")) Then
                            e.Value = "Camera.png"
                        End If
                    End If
                Catch
                End Try

            Case "MaskPhoto"
                Try
                    If Not IsDBNull(e.GetListSourceFieldValue("MaskAlert")) Then
                        If roTypes.Any2Boolean(e.GetListSourceFieldValue("MaskAlert")) Then
                            e.Value = "maskAlert.png"
                        Else
                            e.Value = "maskOK.png"
                        End If
                    Else
                        e.Value = "empty.png"
                    End If
                Catch
                End Try

            Case "TemperaturePhoto"
                Try
                    If Not IsDBNull(e.GetListSourceFieldValue("TemperatureAlert")) Then
                        If roTypes.Any2Boolean(e.GetListSourceFieldValue("TemperatureAlert")) Then
                            e.Value = "TemperatureAlert.png"
                        Else
                            e.Value = "TemperatureOK.png"
                        End If
                    Else
                        e.Value = "empty.png"
                    End If
                Catch
                End Try
            Case "SourcePhoto"
                Try
                    If roTypes.Any2Integer(e.GetListSourceFieldValue("VerificationType")) > 0 Then
                        e.Value = "VerificationType." & roTypes.Any2Integer(e.GetListSourceFieldValue("VerificationType")) & ".png"
                    End If
                Catch
                End Try

            Case "TypeDataUnbound"
                Dim accessType As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("Type"))
                If e.GetListSourceFieldValue("TypeData") IsNot Nothing Then
                    Dim id As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("TypeData"))
                    If id > 0 Or accessType = 4 Or accessType = 10 Or accessType = 6 Then
                        Try
                            Select Case accessType
                                Case 1, 2, 3, 7
                                    e.Value = CausesServiceMethods.GetCauseByID(Me.Page, id, False).Name
                                Case 4
                                    e.Value = API.TasksServiceMethods.GetNameTask(Me.Page, id)
                                Case 13
                                    e.Value = ""
                                    Dim oCenter As New roBusinessCenter
                                    oCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me.Page, id, False)
                                    If Not oCenter Is Nothing Then
                                        e.Value = oCenter.Name
                                    End If

                                Case 6
                                    Dim accessStatus As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("InvalidType"))
                                    Select Case accessStatus
                                        Case InvalidTypeEnum.NOHP_
                                            ' Acceso denegado por documentación incorrecta
                                            e.Value = Me.Language.Translate("Access.Documentation", Me.DefaultScope)
                                        Case InvalidTypeEnum.NTIME_
                                            ' Acceso denegado por fuera de hora
                                            e.Value = Me.Language.Translate("Access.OutOfTime", Me.DefaultScope)
                                        Case InvalidTypeEnum.NRDR_
                                            ' Acceso denegado por lector invàlido
                                            e.Value = Me.Language.Translate("Access.InvalidReader", Me.DefaultScope)
                                        Case InvalidTypeEnum.NCON_
                                            ' Acceso denegado: sin contrato
                                            e.Value = Me.Language.Translate("Access.NoContract", Me.DefaultScope)
                                        Case InvalidTypeEnum.NFLD_
                                            ' Acceso denegado: campos de la ficha del empleado
                                            e.Value = Me.Language.Translate("Access.EmployeeField", Me.DefaultScope)
                                        Case InvalidTypeEnum.NAPB
                                            ' Acceso denegado: campos de la ficha del empleado
                                            e.Value = Me.Language.Translate("Access.Antipassback", Me.DefaultScope)
                                        Case InvalidTypeEnum.NSRV
                                            ' Acceso denegado por el servidor (validaciolnes servidor)
                                            e.Value = Me.Language.Translate("Access.ServerValidation", Me.DefaultScope)
                                        Case InvalidTypeEnum.NNC
                                            e.Value = Me.Language.Translate("Access.AccessNotCompleted", Me.DefaultScope)
                                    End Select
                                Case 10
                                    Dim dinningStatus As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("InvalidType"))

                                    Select Case dinningStatus
                                        Case InvalidTypeEnum.NTIME_
                                            'Sin turno
                                            e.Value = Me.Language.Translate("DinningRoom.NoTurn", Me.DefaultScope)
                                        Case InvalidTypeEnum.NDEF_
                                            'Fichaje offline
                                            e.Value = Me.Language.Translate("DinningRoom.Offline", Me.DefaultScope)
                                        Case InvalidTypeEnum.NRPT_
                                            'Fichaje repetido
                                            e.Value = Me.Language.Translate("DinningRoom.Repeated", Me.DefaultScope)
                                        Case Else
                                            Dim dinningTurn As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("TypeData"))
                                            e.Value = API.DiningRoomServiceMethods.GetDiningRoomTurnByID(Me.Page, dinningTurn, False).Name
                                    End Select

                                Case Else
                                    e.Value = Nothing
                            End Select
                        Catch ex As Exception
                            e.Value = String.Empty
                        End Try
                    Else
                        If Not e.GetListSourceFieldValue("NFC") Is Nothing Then
                            e.Value = e.GetListSourceFieldValue("NFC")
                        Else
                            e.Value = String.Empty
                        End If
                    End If
                Else
                    If accessType = 4 Then
                        e.Value = API.TasksServiceMethods.GetNameTask(Me.Page, 0)
                    Else
                        e.Value = String.Empty
                    End If
                End If

            Case "IDTerminalUnbound"
                If Not (e.GetListSourceFieldValue("IDTerminal") Is Nothing Or IsDBNull(e.GetListSourceFieldValue("IDTerminal"))) AndAlso e.GetListSourceFieldValue("IDTerminal") > 0 Then
                    Try
                        e.Value = API.TerminalServiceMethods.GetTerminalName(Me.Page, roTypes.Any2Integer(e.GetListSourceFieldValue("IDTerminal")))
                        If e.Value = String.Empty Then
                            e.Value = Me.Language.Translate("UnknownTerminal", Me.DefaultScope)
                        End If
                    Catch ex As Exception
                        e.Value = Me.Language.Translate("UnknownTerminal", Me.DefaultScope)
                    End Try
                Else
                    e.Value = Me.Language.Translate("UnknownTerminal", Me.DefaultScope)
                End If

            Case "IsNotReliableUnbound"
                If Not (e.GetListSourceFieldValue("IsNotReliable") Is Nothing Or IsDBNull(e.GetListSourceFieldValue("IsNotReliable"))) Then
                    If roTypes.Any2Boolean(e.GetListSourceFieldValue("IsNotReliable")) = True Then
                        e.Value = "notreliable.png"
                    Else
                        e.Value = "reliable.png"
                    End If
                Else
                    e.Value = String.Empty
                End If


            Case "IDPassportUnbound"
                If Not (e.GetListSourceFieldValue("IDPassport") Is Nothing Or IsDBNull(e.GetListSourceFieldValue("IDPassport"))) Then
                    Try
                        Dim passport As roPassport = Nothing

                        If (roTypes.Any2Integer(e.GetListSourceFieldValue("IDPassport")) <> 0) Then
                            passport = API.UserAdminServiceMethods.GetPassport(Me.Page, roTypes.Any2Integer(e.GetListSourceFieldValue("IDPassport")), LoadType.Passport)
                        End If

                        If Not passport Is Nothing Then
                            e.Value = passport.Name
                        Else
                            e.Value = String.Empty
                        End If
                    Catch ex As Exception
                        e.Value = String.Empty
                    End Try
                Else
                    e.Value = String.Empty
                End If

            Case "ActionUnbound"
                If Not e.GetListSourceFieldValue("IDPassport") Is Nothing And Not e.GetListSourceFieldValue("Action") Is Nothing Then
                    Select Case roTypes.Any2Integer(e.GetListSourceFieldValue("Action"))
                        Case 1
                            e.Value = "actionAdd.png"
                        Case 2
                            e.Value = "actionModify.png"
                        Case 3
                            e.Value = "actionChangeWay.png"
                        Case Else
                            e.Value = "blank.gif" 'String.Empty
                    End Select
                Else
                    e.Value = String.Empty
                End If

            Case "LinkDetails"

                Dim PunchType As Integer = roTypes.Any2Integer(e.GetListSourceFieldValue("Type"))
                Dim bolCanEdit = ((PunchType <= 4 Or PunchType = 10 Or PunchType = 13) And Me.CheckPeriodOfFreezingPage)
                Dim bolEditEnabled As Boolean = (Me.CurrentPermission >= Permission.Write)
                If PunchType = 13 Then
                    bolEditEnabled = bolCanEdit And Me.CurrentPermission >= Permission.Read And Me.CostCenterPermission >= Permission.Write
                End If

                Dim mdate As DateTime = Convert.ToDateTime(e.GetListSourceFieldValue("DateTime"))

                Dim urlJSON As String = ""

                urlJSON = """idmove"":""{0}"",""canedit"":""{1}"",""actualdate"":""{2}"",""shiftdate"":""{3}"",""idterminal"":""{4}"",""idaction"":""{5}"",""idcause_idtask_iddiningroom"":""{6}"",""reliability"":""{7}"",""idpassport"":""{8}"",""movetype"":""{9}"",""actualtype"":""{10}"",""city"":""{11}"",""idzone"":""{12}"",""position"":""{13}"",""field1"":""{14}"",""field2"":""{15}"",""field3"":""{16}"",""timeZone"":""{17}"",""fullAddress"":{18}"

                urlJSON = String.Format(urlJSON, e.GetListSourceFieldValue("ID"), (bolCanEdit And bolEditEnabled).ToString.ToLower, Format(mdate, "dd/MM/yyyy_HH:mm:ss"),
                    Format(CDate(e.GetListSourceFieldValue("ShiftDate")), "dd/MM/yyyy"), e.GetListSourceFieldValue("IDTerminal"), e.GetListSourceFieldValue("Action"),
                    e.GetListSourceFieldValue("TypeData"), e.GetListSourceFieldValue("IsNotReliable"), e.GetListSourceFieldValue("IDPassport"), e.GetListSourceFieldValue("Type"),
                    e.GetListSourceFieldValue("ActualType"), e.GetListSourceFieldValue("LocationZone"), e.GetListSourceFieldValue("IDZone"),
                    HttpUtility.HtmlEncode(e.GetListSourceFieldValue("Location").ToString()),
                    HttpUtility.HtmlEncode(e.GetListSourceFieldValue("Field1").ToString()),
                    HttpUtility.HtmlEncode(e.GetListSourceFieldValue("Field2").ToString()),
                    HttpUtility.HtmlEncode(e.GetListSourceFieldValue("Field3").ToString()),
                    e.GetListSourceFieldValue("TimeZone"), JsonConvert.ToString(e.GetListSourceFieldValue("FullAddress")), e.GetListSourceFieldValue("MaskAlert"), e.GetListSourceFieldValue("TemperatureAlert"), e.GetListSourceFieldValue("VerificationType"))

                If IsDBNull(e.GetListSourceFieldValue("Field4")) Then
                    urlJSON += String.Format(",""field4"":""{0}""", "0")
                Else
                    urlJSON += String.Format(",""field4"":""{0}""", Replace(e.GetListSourceFieldValue("Field4"), ",", "."))
                End If

                If IsDBNull(e.GetListSourceFieldValue("Field5")) Then
                    urlJSON += String.Format(",""field5"":""{0}""", "0")
                Else
                    urlJSON += String.Format(",""field5"":""{0}""", Replace(e.GetListSourceFieldValue("Field5"), ",", "."))
                End If

                If IsDBNull(e.GetListSourceFieldValue("Field6")) Then
                    urlJSON += String.Format(",""field6"":""{0}""", "0")
                Else
                    urlJSON += String.Format(",""field6"":""{0}""", Replace(e.GetListSourceFieldValue("Field6"), ",", "."))
                End If

                Dim bolRowModified As Boolean = e.GetListSourceFieldValue("IsModified")
                If bolRowModified Then
                    urlJSON += String.Format(",""stateofrow"":""{0}"",""keyrow"":""{1}""", DataRowState.Modified.ToString(), e.GetListSourceFieldValue("Clave"))
                Else
                    urlJSON += String.Format(",""stateofrow"":""{0}"",""keyrow"":""{1}""", DataRowState.Unchanged.ToString(), e.GetListSourceFieldValue("Clave"))
                End If

                If IsDBNull(e.GetListSourceFieldValue("InvalidType")) Then
                    urlJSON += String.Format(",""invalidtype"":""{0}""", "-1")
                Else
                    urlJSON += String.Format(",""invalidtype"":""{0}""", roTypes.Any2String(e.GetListSourceFieldValue("InvalidType")))
                End If

                If IsDBNull(e.GetListSourceFieldValue("TypeDetails")) Then
                    urlJSON += String.Format(",""typedetails"":""{0}""", "")
                Else
                    urlJSON += String.Format(",""typedetails"":""{0}""", e.GetListSourceFieldValue("TypeDetails"))
                End If

                e.Value = "{" & urlJSON & "}"

            Case "DateTimeUnbound"

                If Not e.GetListSourceFieldValue("DateTime") Is Nothing Then

                    Dim dt1 As Date = CType(e.GetListSourceFieldValue("DateTime"), DateTime)
                    Dim modifier As String = ""
                    If Not e.GetListSourceFieldValue("ShiftDate") Is Nothing Then
                        Dim dt2 As Date = CType(e.GetListSourceFieldValue("ShiftDate"), DateTime)
                        Dim intOrder As Integer = DateTime.Compare(dt1.Date, dt2.Date)
                        If intOrder > 0 Then
                            modifier = "+"
                        ElseIf intOrder < 0 Then
                            modifier = "-"
                        End If
                    End If

                    e.Value = dt1.ToString("HH:mm:ss") + modifier

                End If

        End Select
    End Sub

    Protected Sub GridMoves_CommandButtonInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCommandButtonEventArgs) Handles GridMoves.CommandButtonInitialize
        If e.ButtonType = ColumnCommandButtonType.Delete Then
            Dim accessType As Integer = roTypes.Any2Integer(GridMoves.GetRowValues(e.VisibleIndex, "Type"))
            Dim accessActualType As Integer = roTypes.Any2Integer(GridMoves.GetRowValues(e.VisibleIndex, "ActualType"))
            Dim bolCanRemove = (accessType <= 4 Or (accessType = 7 AndAlso (accessActualType = 1 Or accessActualType = 2)) Or accessType = 10 Or accessType = 13) And Me.CheckPeriodOfFreezingPage
            Dim bolDeleteEnabled As Boolean = bolCanRemove And (Me.CurrentPermission >= Permission.Write)
            If accessType = 13 Then
                bolDeleteEnabled = bolCanRemove And Me.CurrentPermission >= Permission.Read And Me.CostCenterPermission >= Permission.Write
                '    'If bolDeleteEnabled Then
                '    '    ' Solo dejamos eliminar el fichaje de centro de coste si el supervisor puede gestionarlo
                '    '    Dim CostCenter As Integer = roTypes.Any2Integer(GridMoves.GetRowValues(e.VisibleIndex, "TypeData"))
                '    '    If BusinessCentersDataTable.Select("ID=" & CostCenter.ToString).Length = 0 Then
                '    '        bolDeleteEnabled = False
                '    '    End If
                '    'End If
            End If

            'Comprobamos si el fichaje viene de declaración de la jornada (si es decl. jornada, no permitimos borrado)
            Dim row = GridMoves.GetRow(e.VisibleIndex)
            If (row IsNot Nothing) Then
                Try
                    Dim rowView = DirectCast(row, System.Data.DataRowView)
                    Dim idRequest = roTypes.Any2Integer(rowView.Row("IDRequest"))
                    If (idRequest > 0) Then
                        bolDeleteEnabled = bolDeleteEnabled AndAlso IsRequestApproved(idRequest)
                    End If
                Catch ex As Exception
                    Throw New Exception("MovesNew::GridMoves::GridMoves_CommandButtonInitialize:: " + ex.Message)
                End Try
            End If
            e.Visible = bolDeleteEnabled

        ElseIf e.ButtonType = ColumnCommandButtonType.Edit Then
            Dim bolEditVisible As Boolean = True

            Dim accessType As Integer = roTypes.Any2Integer(GridMoves.GetRowValues(e.VisibleIndex, "Type"))

            If accessType = 6 Then bolEditVisible = False
            'If accessType = 13 Then
            ' Solo dejamos editar el fichaje de centro de coste si el supervisor puede gestionarlo
            'Dim CostCenter As Integer = roTypes.Any2Integer(GridMoves.GetRowValues(e.VisibleIndex, "TypeData"))
            'If BusinessCentersDataTable.Select("ID=" & CostCenter.ToString).Length = 0 Then
            '    bolEditVisible = True
            'End If
            'End If

            e.Visible = bolEditVisible
        End If
    End Sub

    Protected Sub GridMoves_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridMoves.RowDeleting
        Dim tb As DataTable = Me.PunchesDataTable()
        Dim i As Integer = GridMoves.FindVisibleIndexByKeyValue(e.Keys(GridMoves.KeyFieldName))
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridMoves.KeyFieldName))

        Dim accessType As Integer = roTypes.Any2Integer(dr("Type"))

        ' Si es un fichaje de accesos con presencia integrada se permite el borrado pero solo se quita la parte de presencia
        If accessType = 7 Then
            dr("Type") = 5
            dr("ActualType") = 5
            dr("Action") = 2
            dr("IDPassport") = WLHelperWeb.CurrentPassport.ID
        Else
            dr.Delete()
        End If

        Me.PunchesDataTable = tb
        e.Cancel = True
    End Sub

    Protected Sub GridMoves_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridMoves.CustomCallback
        If e.Parameters = "RELOAD" Then
            BindGridMoves(True)

        ElseIf e.Parameters = "SAVE_RELOAD" Then
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)
            Dim enumRet As SaveReturnTypes = MovesSaveDataCallback()
            If enumRet = SaveReturnTypes.ChangesSaved Then
                BindGridMoves(True)
                grid.JSProperties("cpReturnValue") = "OK"
            Else
                grid.JSProperties("cpReturnValue") = ""
            End If

        ElseIf e.Parameters = "ShowLocation" Then
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)
            grid.Columns("LocationZone").Visible = True
            grid.Columns("IDTerminalUnbound").Visible = False

        ElseIf e.Parameters = "ShowTerminal" Then
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)
            grid.Columns("LocationZone").Visible = False
            grid.Columns("IDTerminalUnbound").Visible = True
        End If

    End Sub

#End Region

#Region "GRID INCIDENCIAS"

    Private Sub GridIncidences_DataBinding(sender As Object, e As EventArgs) Handles GridIncidences.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = GridIncidences.Columns("IDCause")
        oCombo.PropertiesComboBox.DataSource = Me.GetJustCausesDataAll()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)

        oCombo = GridIncidences.Columns("IDCenter")
        oCombo.PropertiesComboBox.DataSource = Me.BusinessCentersDataTable()
        oCombo.PropertiesComboBox.TextField = "Name"
        oCombo.PropertiesComboBox.ValueField = "ID"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)
    End Sub

    Protected Sub lblIncidencesCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblIncidencesGridCaption", Me.DefaultScope)
    End Sub

    Protected Sub btnAddNewIncidence_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("btnAddNewIncidenceText", Me.DefaultScope)
        txtLabel.ToolTip = Me.Language.Translate("btnAddNewIncidenceTooltip", Me.DefaultScope)

    End Sub

    Protected Sub GridIncidences_CellEditorInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewEditorEventArgs) Handles GridIncidences.CellEditorInitialize
        If e.Column.FieldName = "IDCause" Then

            Dim tb As DataTable = Me.IncidencesDataTable()
            If e.VisibleIndex >= 0 Then
                'Dim dRow As DataRow = tb.Rows(e.VisibleIndex)
                Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)

                Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                cmb.Font.Size = 8
                cmb.Focus()

                If Not dRow Is Nothing Then
                    Dim dtVisibleCauses As DataTable = GetJustCausesData().Table

                    Dim removeItems As New Generic.List(Of ListEditItem)
                    For Each oItem As ListEditItem In cmb.Items

                        Dim oRows() As DataRow = dtVisibleCauses.Select("ID = " & oItem.Value)

                        If oRows.Length = 0 Then
                            If roTypes.Any2Integer(dRow("IDCause")) <> oItem.Value Then
                                removeItems.Add(oItem)
                            End If
                        Else
                            ' Si el registro tiene una incidencia relacionada
                            ' no puede contener justificaciones de dia ni personalizadas
                            If roTypes.Any2Double(dRow("IDRelatedIncidence")) > 0 Then
                                Dim oRowsaux() As DataRow = dtVisibleCauses.Select("ID = " & oItem.Value & "AND isnull(DayType,0) =0 AND isnull(CustomType,0) = 0 ")
                                If oRowsaux.Length = 0 Then
                                    If roTypes.Any2Integer(dRow("IDCause")) <> oItem.Value Then
                                        removeItems.Add(oItem)
                                    End If
                                End If
                            End If
                        End If
                    Next

                    For Each oDeleteItem As ListEditItem In removeItems
                        cmb.Items.Remove(oDeleteItem)
                    Next
                End If
            Else
                Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                cmb.Font.Size = 8
                cmb.Focus()

                Dim dtVisibleCauses As DataTable = GetJustCausesData().Table

                Dim removeItems As New Generic.List(Of ListEditItem)
                For Each oItem As ListEditItem In cmb.Items

                    Dim oRows As DataRow() = Nothing
                    If dtVisibleCauses IsNot Nothing Then oRows = dtVisibleCauses.Select("ID = " & oItem.Value)

                    If dtVisibleCauses Is Nothing OrElse oRows.Length = 0 Then
                        removeItems.Add(oItem)
                    End If
                Next

                For Each oDeleteItem As ListEditItem In removeItems
                    cmb.Items.Remove(oDeleteItem)
                Next
            End If

        ElseIf e.Column.FieldName = "IDCenter" Then

            Dim bolOnlyAvailableCostCentersByEmployee As Boolean = False
            Dim strIDCenter As String = "ID"
            If roTypes.Any2String(HelperSession.AdvancedParametersCache("OnlyAvailableCostCentersByEmployee")) = "1" Then
                ' Solo mostramos los centros de coste que tiene asignado el empleado para realizar cesiones
                bolOnlyAvailableCostCentersByEmployee = True
                strIDCenter = "IDCenter"
            End If

            Dim tb As DataTable = Me.IncidencesDataTable()
            If e.VisibleIndex >= 0 Then
                'Dim dRow As DataRow = tb.Rows(e.VisibleIndex)
                Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)

                Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                cmb.Font.Size = 8
                'cmb.Focus()

                If Not dRow Is Nothing Then
                    Dim dtVisibleCenters As DataTable = Nothing
                    If Not bolOnlyAvailableCostCentersByEmployee Then
                        dtVisibleCenters = Me.BusinessCentersDetailDataTable(, True)
                    Else
                        dtVisibleCenters = Me.GetAvailableCostCentersByEmployee(True)
                    End If

                    Dim removeItems As New Generic.List(Of ListEditItem)
                    For Each oItem As ListEditItem In cmb.Items

                        Dim oRows() As DataRow = Nothing

                        If Not dtVisibleCenters Is Nothing Then oRows = dtVisibleCenters.Select(strIDCenter & " = " & oItem.Value)

                        If dtVisibleCenters Is Nothing OrElse oRows.Length = 0 Then
                            If roTypes.Any2Integer(dRow("IDCenter")) <> oItem.Value Then
                                removeItems.Add(oItem)
                            End If
                        End If
                    Next

                    For Each oDeleteItem As ListEditItem In removeItems
                        cmb.Items.Remove(oDeleteItem)
                    Next
                End If
            Else
                Try
                    Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
                    cmb.Font.Size = 8
                    'cmb.Focus()

                    Dim dtVisibleCenters As DataTable = Nothing
                    If Not bolOnlyAvailableCostCentersByEmployee Then
                        dtVisibleCenters = Me.BusinessCentersDetailDataTable(, True)
                    Else
                        dtVisibleCenters = Me.GetAvailableCostCentersByEmployee(True)
                    End If

                    Dim removeItems As New Generic.List(Of ListEditItem)
                    For Each oItem As ListEditItem In cmb.Items
                        Dim oRows() As DataRow = Nothing

                        If Not dtVisibleCenters Is Nothing Then oRows = dtVisibleCenters.Select(strIDCenter & " = " & oItem.Value)

                        If dtVisibleCenters Is Nothing OrElse oRows.Length = 0 Then
                            removeItems.Add(oItem)
                        End If
                    Next

                    For Each oDeleteItem As ListEditItem In removeItems
                        cmb.Items.Remove(oDeleteItem)
                    Next
                Catch ex As Exception

                End Try
            End If

        ElseIf e.Column.FieldName = "ValueHoraEditable" Then

            Dim txt As ASPxTextBox
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)

            txt = CType(e.Editor, ASPxTextBox)
            txt.Width = 36
            txt.Font.Size = 8

            If grid.IsNewRowEditing() Then
                txt.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                Dim tb As DataTable = Me.IncidencesDataTable()
                If e.VisibleIndex >= 0 Then
                    Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)
                    If dRow IsNot Nothing Then
                        If roTypes.Any2Boolean(dRow("DayType")) Or roTypes.Any2Boolean(dRow("CustomType")) Then
                            txt.MaskSettings.Mask = "<-9999..9999>.<000..999>"
                            txt.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
                        End If
                    End If
                End If
            Else
                If Not IsDirectCause(e.VisibleIndex) Then
                    txt.MaskSettings.Mask = "HH:mm"
                Else
                    txt.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                    'si el numero de horas es cero hacer una chapucilla porque el control no lo pinta bien
                    Try
                        If txt.Text.Split(":")(0) = "00" Then
                            txt.Text = txt.Text.Substring(1)
                        End If
                    Catch ex As Exception
                    End Try

                    Dim tb As DataTable = Me.IncidencesDataTable()
                    If e.VisibleIndex >= 0 Then
                        Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)
                        If dRow IsNot Nothing Then
                            If roTypes.Any2Boolean(dRow("DayType")) Or roTypes.Any2Boolean(dRow("CustomType")) Then
                                txt.MaskSettings.Mask = "<-9999..9999>.<000..999>"
                                txt.MaskSettings.IncludeLiterals = DevExpress.Web.MaskIncludeLiteralsMode.DecimalSymbol
                            End If
                        End If
                    End If

                End If
            End If
        End If

        Dim tbInc As DataTable = Me.IncidencesDataTable()
        If e.VisibleIndex >= 0 Then
            'Dim dRow As DataRow = tb.Rows(e.VisibleIndex)
            Dim dRow As DataRow = tbInc.Rows.Find(e.KeyValue)
            If Not dRow Is Nothing Then

                If e.Column.FieldName.StartsWith("IDCause") OrElse e.Column.FieldName.StartsWith("ValueHoraEditable") Then
                    If (roTypes.Any2Integer(dRow("DailyRule")) = 1) OrElse (roTypes.Any2Integer(dRow("AccrualsRules")) = 1) OrElse (roTypes.Any2Integer(dRow("AccruedRule")) = 1) Then
                        e.Editor.Enabled = False
                        'For Each oControl As ASPxEditBase In e.Editor.Controls
                        '    oControl.Enabled = False
                        'Next

                    End If
                End If
            End If
        End If

    End Sub

    Protected Sub GridIncidences_CommandButtonInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCommandButtonEventArgs) Handles GridIncidences.CommandButtonInitialize
        If e.ButtonType = ColumnCommandButtonType.Delete Then

            Dim bolCauseType As Boolean = IsDirectCause(e.VisibleIndex)

            Dim bolDeleteEnabled As Boolean = (bolCauseType AndAlso
                                               Me.DirectCausesPermission >= Permission.Admin AndAlso
                                               Me.CurrentDirectCausePermission >= Permission.Write)

            e.Visible = bolDeleteEnabled And Me.CheckPeriodOfFreezingPage

            Dim dailyRule As Integer = roTypes.Any2Integer(GridIncidences.GetRowValues(e.VisibleIndex, "DailyRule"))
            Dim AccruedRule As Integer = roTypes.Any2Integer(GridIncidences.GetRowValues(e.VisibleIndex, "AccruedRule"))
            Dim accrualRule As Integer = roTypes.Any2Integer(GridIncidences.GetRowValues(e.VisibleIndex, "AccrualsRules"))

            If (dailyRule = 1) OrElse (accrualRule = 1) OrElse (AccruedRule = 1) Then
                e.Visible = False
            End If

        ElseIf e.ButtonType = ColumnCommandButtonType.Edit Then

            Dim bolEditEnabled As Boolean

            If IsDirectCause(e.VisibleIndex) Then
                bolEditEnabled = (Me.DirectCausesPermission >= Permission.Write) AndAlso
                                 (Me.CurrentDirectCausePermission >= Permission.Write)

                ' Miramos si tiene permisos sobre la feature de centro de costes
                If Not bolEditEnabled And FeatureCostCenter Then
                    bolEditEnabled = (Me.DirectCausesPermission >= Permission.Read) AndAlso
                                 (Me.CurrentDirectCausePermission >= Permission.Read)
                    If bolEditEnabled Then
                        bolEditEnabled = Me.CostCenterPermission > Permission.Read
                    End If
                End If
            Else
                bolEditEnabled = (Me.JustifyIncPermission >= Permission.Write) AndAlso
                                 (Me.CurrentJustifyIncPermission >= Permission.Write)

                ' Miramos si tiene permisos sobre la feature de centro de costes
                If Not bolEditEnabled And FeatureCostCenter Then
                    bolEditEnabled = (Me.JustifyIncPermission >= Permission.Read) AndAlso
                                 (Me.CurrentJustifyIncPermission >= Permission.Read)
                    If bolEditEnabled Then
                        bolEditEnabled = Me.CostCenterPermission > Permission.Read
                    End If
                End If
            End If

            e.Visible = bolEditEnabled And Me.CheckPeriodOfFreezingPage

        End If
    End Sub

    Protected Sub GridIncidences_InitNewRow(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInitNewRowEventArgs) Handles GridIncidences.InitNewRow

        Dim bolEditCause As Boolean = False
        Dim bolEditCenter As Boolean = False
        Dim bolEditComboCause As Boolean = False

        bolEditCause = (Me.DirectCausesPermission >= Permission.Write) AndAlso
                             (Me.CurrentDirectCausePermission >= Permission.Write)

        bolEditComboCause = bolEditCause

        bolEditCenter = (Me.DirectCausesPermission >= Permission.Read) AndAlso
                         (Me.CurrentDirectCausePermission >= Permission.Read)
        If bolEditCenter Then
            bolEditCenter = Me.CostCenterPermission > Permission.Read
        End If

        AssignTemplatesInReadOnlyColumns(bolEditCause, bolEditCenter, bolEditComboCause)
    End Sub

    Private Function checkLabAgreeLimits(ByVal idCause As Integer, ByVal valueEddited As Double, ByVal clave As String) As Boolean
        Dim bCorrect As Boolean = False

        Dim idCauses As New Generic.List(Of Integer)
        idCauses.Add(idCause)

        Dim tb As DataTable = Me.IncidencesDataTable()
        Dim acumValue As Double = valueEddited

        For Each oRow As DataRow In tb.Rows
            If oRow.RowState <> DataRowState.Deleted Then
                If oRow("Clave") <> clave AndAlso oRow("IDCause") = idCause Then
                    acumValue = acumValue + roTypes.Any2Double(oRow("Value"))
                End If
            End If
        Next

        Dim valuesEddited As New Generic.List(Of Double)
        valuesEddited.Add(acumValue)

        bCorrect = API.LabAgreeServiceMethods.ValidateLabAgreeDailyCausesOnDate(Me.Page, Me.IDEmployeePage, Me.DatePage, idCauses.ToArray(), valuesEddited.ToArray())

        Return bCorrect
    End Function

    Protected Sub GridIncidences_CustomErrorText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomErrorTextEventArgs) Handles GridIncidences.CustomErrorText
        e.ErrorText = e.Exception.Message
    End Sub

    Protected Sub GridIncidences_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles GridIncidences.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim intFormatType As Integer = 0
            Dim cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

            Dim IDCause As Integer = roTypes.Any2Integer(e.NewValues("IDCause"))

            Dim dtVisibleCauses As DataTable = GetJustCausesData().Table

            Dim bolContinue As Boolean = True

            If dtVisibleCauses IsNot Nothing Then
                Dim oRows() As DataRow = dtVisibleCauses.Select("ID = " & IDCause.ToString)
                If oRows IsNot Nothing Then
                    If oRows.Length > 0 Then
                        If roTypes.Any2Boolean(oRows(0).Item("DayType")) Or roTypes.Any2Boolean(oRows(0).Item("CustomType")) Then
                            intFormatType = 1
                        End If
                    Else
                        ' No tiene permisos para añadir dicha justificacion,
                        ' no pertenece a ningun grupo de negocio que gestione
                        bolContinue = False
                        Throw New Exception(Me.Language.Translate("Incidence.NotAllowedCause.Error", Me.DefaultScope))
                    End If
                End If
            End If

            If bolContinue Then
                Dim ValueHoraEditable As Double = roConversions.ConvertTimeToHours(e.NewValues("ValueHoraEditable"))

                If Not checkLabAgreeLimits(IDCause, ValueHoraEditable, -1) Then
                    Throw New Exception(Me.Language.Translate("Incidence.LabAgreeExceeded.Error", Me.DefaultScope))
                Else
                    Dim tb As DataTable = Me.IncidencesDataTable()
                    Dim oNewRow As DataRow = tb.NewRow
                    With oNewRow
                        .Item("IDEmployee") = Me.IDEmployeePage
                        .Item("Date") = Me.DatePage
                        .Item("IDRelatedIncidence") = 0
                        .Item("IDCause") = IDCause
                        .Item("AccrualsRules") = 0
                        .Item("DailyRule") = 0
                        .Item("AccruedRule") = 0
                        If intFormatType = 1 Then
                            .Item("DayType") = 1
                            .Item("CustomType") = 1
                        End If
                        .Item("Manual") = 1
                        .Item("Calculado") = Me.Language.Translate("Calculado.Manual", Me.DefaultScope)
                        Try
                            If WLHelperWeb.CurrentPassport.IDUser.HasValue Then .Item("CauseUser") = WLHelperWeb.CurrentPassport.IDUser
                        Catch ex As Exception
                            Throw New Exception(Me.Language.Translate("AccessDenied.Message", Me.DefaultScope))
                        End Try

                        .Item("CauseUserType") = 0
                        .Item("IsNotReliable") = 0
                        .Item("Value") = ValueHoraEditable
                        .Item("BeginTimeOrder") = New Date(2079, 1, 1)
                        .Item("IncidenceValue") = ValueHoraEditable

                        Dim IDCenter As Integer = roTypes.Any2Integer(e.NewValues("IDCenter"))
                        .Item("IDCenter") = IDCenter

                        Dim intIDCenterDefault = API.TasksServiceMethods.GetEmployeeDefaultBusinessCenter(Me, Me.IDEmployeePage, Me.DatePage)

                        .Item("ManualCenter") = 0
                        If roTypes.Any2Integer(.Item("IDCenter")) = 0 Then
                            .Item("IDCenter") = intIDCenterDefault
                        Else
                            .Item("ManualCenter") = 1
                        End If
                        If roTypes.Any2Integer(.Item("IDCenter")) = intIDCenterDefault Then
                            .Item("DefaultCenter") = 1
                        End If

                        'añadidos
                        If intFormatType = 0 Then
                            .Item("ValueHora") = roConversions.ConvertHoursToTime(ValueHoraEditable)
                        Else
                            .Item("ValueHora") = Format(ValueHoraEditable, "##0.000")
                        End If
                        .Item("ValueHoraEditable") = .Item("ValueHora")
                        .Item("TextoIncidencia") = IIf(roTypes.Any2String(.Item("IDType")) <> String.Empty, Me.Language.Keyword("Incidence." & roTypes.Any2String(.Item("IDType"))), "")
                        .Item("Clave") = Guid.NewGuid()

                    End With

                    tb.Rows.Add(oNewRow)

                    Me.IncidencesDataTable = tb
                End If

            End If

            e.Cancel = True
            grid.CancelEdit()

        End If

    End Sub

    Protected Sub GridIncidences_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridIncidences.RowUpdating

        Dim tb As DataTable = Me.IncidencesDataTable()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridIncidences.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim RowTime As Double = 0
        Dim GridTime As Double = 0
        Dim GridCenter As Integer = 0
        Dim RowCenter As Integer = 0
        Dim bolModifytime As Boolean = False
        Dim bolModifyCenter As Boolean = False

        Dim intFormatType As Integer = 0

        Dim cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

        'obtener la hora guardada
        If Not dr("ValueHoraEditable") Is System.DBNull.Value Then

            If roTypes.Any2Boolean(dr("DayType")) Or roTypes.Any2Boolean(dr("CustomType")) Then
                intFormatType = 1
            End If

            If intFormatType = 1 Then
                RowTime = roTypes.Any2Double(dr("ValueHoraEditable").Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
            Else
                RowTime = roConversions.ConvertTimeToHours(dr("ValueHoraEditable"))
            End If

        End If

        RowCenter = roTypes.Any2Integer(dr.Item("IDCenter"))

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            If enumerator.Key.ToString() = "ValueHoraEditable" Then
                If intFormatType = 0 Then
                    GridTime = roConversions.ConvertTimeToHours(enumerator.Value)
                Else
                    GridTime = roTypes.Any2Double(enumerator.Value.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                End If

                bolModifytime = True
            ElseIf enumerator.Key.ToString() = "IDCenter" Then
                GridCenter = enumerator.Value
                bolModifyCenter = True
            End If
        End While

        Dim checkIDCause As Integer = -1
        If e.NewValues("IDCause") IsNot Nothing Then
            checkIDCause = e.NewValues("IDCause")
        Else
            checkIDCause = dr.Item("IDCause")
        End If

        Dim dblNewValue_ValueHoraEditable As Double = 0

        If bolModifytime Then
            If intFormatType = 0 Then
                dblNewValue_ValueHoraEditable = roConversions.ConvertTimeToHours(e.NewValues("ValueHoraEditable"))
            Else
                dblNewValue_ValueHoraEditable = roTypes.Any2Double(e.NewValues("ValueHoraEditable").Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
            End If
        End If

        'Si me han modificado el tiempo y este causa un error de validación de convenios no debo permitir modificar la linia
        If bolModifytime AndAlso Not checkLabAgreeLimits(checkIDCause, dblNewValue_ValueHoraEditable, dr.Item("Clave")) Then
            Throw New Exception(Me.Language.Translate("Incidence.LabAgreeExceeded.Error", Me.DefaultScope))
        Else
            enumerator = e.NewValues.GetEnumerator()
            enumerator.Reset()
            While enumerator.MoveNext()
                dr(enumerator.Key.ToString()) = enumerator.Value
            End While

            'Comprobar si es direct cause
            Dim IsDirectCause As Boolean = (roTypes.Any2Integer(dr("IDRelatedIncidence")) <> 0)

            ' revismos si ha cambiado el centro
            If GridCenter <> RowCenter AndAlso bolModifyCenter Then
                dr.Item("ManualCenter") = 1
                dr.Item("DefaultCenter") = 1
                ' Revisamos si es el centro por defecto
                Dim intIDCenterDefault = API.TasksServiceMethods.GetEmployeeDefaultBusinessCenter(Me, dr("IDEmployee"), Me.DatePage)
                If dr.Item("IDCenter") <> intIDCenterDefault Then
                    dr.Item("DefaultCenter") = 0
                End If
            End If

            If bolModifytime Then
                If RowTime <> 0 AndAlso RowTime > GridTime AndAlso roTypes.Any2Double(dr("IDRelatedIncidence")) <> 0 Then
                    'la hora anterior es mas grande que la nueva
                    Dim oNewRow As DataRow = tb.NewRow

                    oNewRow.Item("IDEmployee") = dr("IDEmployee")
                    oNewRow.Item("Date") = dr("Date")
                    oNewRow.Item("IDRelatedIncidence") = dr("IDRelatedIncidence")
                    If Me.ActualEditCausePermission Then
                        oNewRow.Item("IDCause") = 0
                    Else
                        oNewRow.Item("IDCause") = dr("IDCause")
                    End If

                    oNewRow.Item("AccrualsRules") = 0
                    oNewRow.Item("DailyRule") = 0
                    oNewRow.Item("AccruedRule") = 0
                    oNewRow.Item("Manual") = 1
                    oNewRow.Item("Calculado") = Me.Language.Translate("Calculado.Manual", Me.DefaultScope)
                    Try
                        If WLHelperWeb.CurrentPassport.IDUser.HasValue Then oNewRow.Item("CauseUser") = WLHelperWeb.CurrentPassport.IDUser
                    Catch ex As Exception
                        Throw New Exception(Me.Language.Translate("AccessDenied.Message", Me.DefaultScope))
                    End Try

                    oNewRow.Item("CauseUserType") = 0
                    oNewRow.Item("IsNotReliable") = 0
                    oNewRow.Item("Value") = RowTime - GridTime
                    oNewRow.Item("TimeZoneName") = dr("TimeZoneName")
                    oNewRow.Item("BeginTime") = dr("BeginTime")
                    oNewRow.Item("BeginTimeOrder") = dr("BeginTimeOrder")
                    oNewRow.Item("EndTime") = dr("EndTime")
                    oNewRow.Item("IncidenceValue") = dr("IncidenceValue")
                    oNewRow.Item("IDType") = dr("IDType")

                    If GridCenter <> RowCenter AndAlso bolModifyCenter Then
                        oNewRow.Item("IDCenter") = GridCenter
                    Else
                        oNewRow.Item("IDCenter") = dr("IDCenter")
                    End If

                    oNewRow.Item("DefaultCenter") = dr("DefaultCenter")
                    oNewRow.Item("ManualCenter") = dr("ManualCenter")

                    'añadidos
                    oNewRow.Item("ValueHora") = roConversions.ConvertHoursToTime(RowTime - GridTime)

                    oNewRow.Item("ValueHoraEditable") = oNewRow.Item("ValueHora")
                    oNewRow.Item("TextoIncidencia") = IIf(roTypes.Any2String(dr("IDType")) <> String.Empty, Me.Language.Keyword("Incidence." & roTypes.Any2String(dr("IDType"))), "")
                    oNewRow.Item("Clave") = Guid.NewGuid()

                    'corregir la fila modificada
                    dr.Item("Value") = GridTime
                    dr.Item("Manual") = 1
                    dr.Item("Calculado") = Me.Language.Translate("Calculado.Manual", Me.DefaultScope)
                    dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
                    dr.Item("ValueHoraEditable") = dr.Item("ValueHora")

                    tb.Rows.Add(oNewRow)
                Else
                    If RowTime < GridTime And Not Me.ActualEditCausePermission Then
                        ' Si no tiene permisos para modificar el tiempo y supera el valor inicial
                        ' no dejamos modificarlo
                        dr.Item("Value") = RowTime
                        If intFormatType = 0 Then
                            dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
                        Else
                            dr.Item("ValueHora") = Format(dr.Item("Value"), "##0.000")
                        End If

                        dr.Item("ValueHoraEditable") = dr.Item("ValueHora")

                        Throw New Exception(Me.Language.Translate("ModifyTimeDenied.Message", Me.DefaultScope))
                    Else
                        'corregir la fila modificada
                        dr.Item("Value") = GridTime
                        dr.Item("Manual") = 1
                        dr.Item("Calculado") = Me.Language.Translate("Calculado.Manual", Me.DefaultScope)
                        If intFormatType = 0 Then
                            dr.Item("ValueHora") = roConversions.ConvertHoursToTime(dr.Item("Value"))
                        Else
                            dr.Item("ValueHora") = Format(dr.Item("Value"), "##0.000")
                        End If

                        dr.Item("ValueHoraEditable") = dr.Item("ValueHora")
                    End If
                End If
            End If

            Me.IncidencesDataTable = tb
        End If

        'tb.AcceptChanges()

        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridIncidences_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridIncidences.RowDeleting
        Dim tb As DataTable = Me.IncidencesDataTable()

        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridIncidences.KeyFieldName))
        If bolBusinessGroupApplyNotAllowedModifyCause AndAlso Not AllowModifyCause(roTypes.Any2Integer(dr.Item("IDCause"))) Then
            Throw New Exception(Me.Language.Translate("Incidence.NotAllowedCause.Error", Me.DefaultScope))
        Else
            dr.Delete()
            Me.IncidencesDataTable = tb
        End If

        e.Cancel = True
    End Sub

    Protected Sub GridIncidences_StartRowEditing(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxStartRowEditingEventArgs) Handles GridIncidences.StartRowEditing
        Dim bolEditCause As Boolean = False
        Dim bolEditCenter As Boolean = False
        Dim bolEditComboCause As Boolean = False

        If roTypes.Any2Integer(GridIncidences.GetRowValuesByKeyValue(e.EditingKeyValue, "IDRelatedIncidence")) = 0 Then
            bolEditCause = (Me.DirectCausesPermission >= Permission.Write) AndAlso
                             (Me.CurrentDirectCausePermission >= Permission.Write)

            bolEditCenter = (Me.DirectCausesPermission >= Permission.Read) AndAlso
                             (Me.CurrentDirectCausePermission >= Permission.Read)
            If bolEditCenter Then
                bolEditCenter = Me.CostCenterPermission > Permission.Read
            End If

            ' No dejamos modificar la justificacion seleccionada cuando se ha añadido manualmente
            bolEditComboCause = False
        Else
            bolEditCause = (Me.JustifyIncPermission >= Permission.Write) AndAlso
                             (Me.CurrentJustifyIncPermission >= Permission.Write)

            bolEditComboCause = bolEditCause

            bolEditCenter = (Me.JustifyIncPermission >= Permission.Read) AndAlso
                             (Me.CurrentJustifyIncPermission >= Permission.Read)
            If bolEditCenter Then
                bolEditCenter = Me.CostCenterPermission > Permission.Read
            End If
        End If

        ' En el caso que este habilitado el parametro de
        ' No permitir modificar justificaciones que
        ' el supervisor no puede gestionar por grupos de negocio
        If bolBusinessGroupApplyNotAllowedModifyCause Then
            ' Si no tiene permisos para gestionar la justificacion asignada,
            ' deshabilitamos la edición
            If Not AllowModifyCause(roTypes.Any2Integer(GridIncidences.GetRowValuesByKeyValue(e.EditingKeyValue, "IDCause"))) Then
                bolEditCause = False
                bolEditCenter = False
                bolEditComboCause = False
            End If
        End If

        AssignTemplatesInReadOnlyColumns(bolEditCause, bolEditCenter, bolEditComboCause)
    End Sub

    Protected Sub GridIncidences_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridIncidences.CustomCallback
        If e.Parameters = "RELOAD" Then
            BindGridIncidences(True)
        ElseIf e.Parameters = "SAVE_RELOAD" Then
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)
            Dim enumRet As SaveReturnTypes = IncidencesSaveDataCallback()
            If enumRet = SaveReturnTypes.ChangesSaved Then
                BindGridIncidences(True)
                grid.JSProperties("cpReturnValue") = "OK"
            Else
                grid.JSProperties("cpReturnValue") = ""
            End If
        End If

    End Sub

    Protected Sub GridIncidences_HtmlDataCellPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewTableDataCellEventArgs) Handles GridIncidences.HtmlDataCellPrepared
        Dim tb As DataTable = Me.IncidencesDataTable()
        If tb IsNot Nothing AndAlso e IsNot Nothing Then

            If e.VisibleIndex >= 0 Then
                'Dim dRow As DataRow = tb.Rows(e.VisibleIndex)
                Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)

                If dRow IsNot Nothing Then
                    If e.DataColumn.FieldName.StartsWith("ValueHoraEditable") OrElse e.DataColumn.FieldName.StartsWith("IDCause") OrElse e.DataColumn.FieldName.StartsWith("IDCenter") Then

                        If (roTypes.Any2Integer(dRow("DailyRule")) = 1) OrElse (roTypes.Any2Integer(dRow("AccrualsRules")) = 1) OrElse (roTypes.Any2Integer(dRow("AccruedRule")) = 1) Then
                            e.Cell.Enabled = False
                            e.Cell.Style("background-color") = "#acacac" ' blue
                        End If
                    End If
                End If
            End If
        End If

    End Sub

#End Region

#Region "GRID ACUMULADOS"

    Protected Sub lblAcumCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblAcumCaption", Me.DefaultScope)
    End Sub

    Protected Sub btnShowTotalAcum_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("btnShowTotalAcumText", Me.DefaultScope)

    End Sub

    Protected Sub btnShowDailyAcum_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("btnShowDailyAcumText", Me.DefaultScope)
    End Sub

    Protected Sub GridAcum_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridAcum.DataBinding
        If Not IsPostBack AndAlso Not IsCallback Then
            Me.BindGridAcum(True)
        Else
            Me.BindGridAcum(False)
        End If
    End Sub

    Protected Sub GridAcum_CustomColumnDisplayText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs) Handles GridAcum.CustomColumnDisplayText
        'If e.Column.FieldName = "Total" Then
        'If e.Value IsNot System.DBNull.Value And e.Value IsNot Nothing Then
        'e.DisplayText = roConversions.ConvertHoursToTime(CDbl(e.Value))
        'End If
        'End If
    End Sub

    Protected Sub GridAcum_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridAcum.CustomCallback
        If e.Parameters = "RELOAD" Then
            Me.BindGridAcum(True)
            GridAcum.DataBind()
        End If
    End Sub

#End Region

#Region "GRID SELECTOR"

    Protected Sub GridSelector_DataBinding(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridSelector.DataBinding
        If Not IsPostBack AndAlso Not IsCallback Then
            Me.BindGridSelector(True)
        Else
            Me.BindGridSelector(False)
        End If
    End Sub

#End Region

#Region "REGION CALLBACKS"

    ''' <summary>
    ''' CALLBACK UTIIZABLE PARA MODIFICAR VARIABLES DE SESSION DEL SERVIDOR.
    ''' ¡¡¡NO MODIFICAR NADA MAS PORQUE NO FUNCIONARÁ!!!!!
    ''' </summary>
    Protected Sub CallbackSession_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles CallbackSession.Callback

        Dim strParameter As String = Server.UrlDecode(e.Parameter())

        Dim oMoveParameter As New MoveParameter()
        oMoveParameter = roJSONHelper.Deserialize(strParameter, oMoveParameter.GetType())

        Select Case oMoveParameter.Accion.ToUpperInvariant
            Case "SHOWDAILYACUM"
                Me.ViewAcumPage = "DAILY"

            Case "SHOWTOTALACUM"
                Me.ViewAcumPage = "ANUAL"

            Case "VISTA"
                ' Han cambiado la vista
                Select Case oMoveParameter.Valor.ToUpperInvariant
                    Case ViewPageTypes.Moves
                        Me.ViewPage = ViewPageTypes.Moves
                    Case ViewPageTypes.Mobility
                        Me.ViewPage = ViewPageTypes.Mobility
                    Case ViewPageTypes.Tasks
                        Me.ViewPage = ViewPageTypes.Tasks
                    Case ViewPageTypes.DinningRoom
                        Me.ViewPage = ViewPageTypes.DinningRoom
                End Select
            Case "ISCHANGEALLOWED"
                e.Result = Me.IsChangeAllowed().ToString.ToUpperInvariant

            Case "SAVEMOVE"
                e.Result = UpdateMoveDetail(oMoveParameter.Valor).ToString.ToUpperInvariant

        End Select

    End Sub

    Private Sub Audit()
        Try
            ' Auditoría de consulta de datos
            If Me.IDEmployeePage <> -1 Then
                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)
                lstAuditParameterNames.Add("{EmployeeName}")
                lstAuditParameterValues.Add(Me.GetEmployeeName(Me.IDEmployeePage))
                lstAuditParameterNames.Add("{Date}")
                lstAuditParameterValues.Add(Me.DatePage.Value.ToShortDateString)
                lstAuditParameterNames.Add("{Info}")
                Dim sInfo As String = ""
                Select Case Me.ViewPage
                    Case ViewPageTypes.DinningRoom
                        sInfo = Me.Language.Translate("cbViewDiningRoom", Me.DefaultScope)
                    Case ViewPageTypes.Mobility
                        sInfo = Me.Language.Translate("cbViewMobility", Me.DefaultScope)
                    Case ViewPageTypes.Moves
                        sInfo = Me.Language.Translate("cbViewAttendanceandAccess", Me.DefaultScope)
                    Case ViewPageTypes.Tasks
                        sInfo = Me.Language.Translate("cbViewTasks", Me.DefaultScope)
                End Select
                lstAuditParameterValues.Add(sInfo)

                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aSelect, Robotics.VTBase.Audit.ObjectType.tDailyPunchesDetail, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
            End If
        Catch ex As Exception
        End Try

    End Sub

    Private Function IsChangeAllowed() As Boolean
        Dim bolAllowed As Boolean = True
        bolAllowed = Not Me.EditingData
        If bolAllowed Then
            bolAllowed = Not Me.HasChanges()
        End If
        Return bolAllowed
    End Function

    Protected Sub CallbackPanelShift_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPanelShift.Callback
        Dim cbp As ASPxCallbackPanel = CType(sender, ASPxCallbackPanel)

        If e.Parameter = String.Empty Then
            Me.LoadShiftUsed(True)
            cbp.JSProperties("cpReturnValue") = ""
        Else
            Try
                Dim strParameter As String = Server.UrlDecode(e.Parameter)
                Dim oMoveParameter As New MoveParameter()
                oMoveParameter = roJSONHelper.Deserialize(strParameter, oMoveParameter.GetType())

                If oMoveParameter.Accion.ToUpperInvariant = "SHIFTCHANGE" Then

                    If Me.CheckPeriodOfFreezingWithoutFuture() Then

                        ' Mirar si està pendiente modificar el horario asignado
                        If Me.hdnIDShiftChange.Value <> "" AndAlso Val(Me.hdnIDShiftChange.Value) > 0 Then
                            'TODO: Aqui no falta mirar si el dia es troba bloquejat???
                            Dim xStartShift As DateTime = Nothing
                            Dim strStart As String = Me.hdnStartShiftChange.Value
                            If strStart.Length >= 12 Then xStartShift = New DateTime(CInt(strStart.Substring(0, 4)), CInt(strStart.Substring(4, 2)), CInt(strStart.Substring(6, 2)), CInt(strStart.Substring(8, 2)), CInt(strStart.Substring(10, 2)), 0)
                            Dim intIDAssignment As Integer = -1
                            If Me.hdnIDAssignmentChange.Value <> "" AndAlso IsNumeric(Me.hdnIDAssignmentChange.Value) Then intIDAssignment = Val(Me.hdnIDAssignmentChange.Value)
                            If API.EmployeeServiceMethods.AssignShift(Me.Page, Me.IDEmployeePage, Me.DatePage, CInt(Me.hdnIDShiftChange.Value), 0, 0, 0, xStartShift, Nothing, Nothing, Nothing, intIDAssignment, Robotics.Base.DTOs.LockedDayAction.None, Robotics.Base.DTOs.LockedDayAction.None, True) Then
                                Me.LoadShiftUsed(True)
                            End If
                            Me.hdnIDShiftChange.Value = ""

                            cbp.JSProperties("cpReturnValue") = "OK"

                        End If

                    End If
                ElseIf oMoveParameter.Accion.ToUpperInvariant = "SHIFTREMOVEHOLIDAYS" Then
                    If API.EmployeeServiceMethods.AssignShift(Me.Page, Me.IDEmployeePage, Me.DatePage, 0, 0, 0, 0, Nothing, Nothing, Nothing, Nothing, 0, Robotics.Base.DTOs.LockedDayAction.None, Robotics.Base.DTOs.LockedDayAction.None, True) Then
                        Me.LoadShiftUsed(True)
                    End If
                End If
            Catch
            End Try
        End If

    End Sub

    Protected Sub CallbackPanelAbsence_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPanelAbsence.Callback
        Me.LoadAbsence(True)
    End Sub

    Protected Sub CallbackPanelRemarks_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPanelRemarks.Callback
        If e.Parameter = "" Then
            Me.LoadRemarks(True)

        ElseIf e.Parameter = "SAVE_RELOAD" Then
            Dim cbp As ASPxCallbackPanel = CType(sender, ASPxCallbackPanel)
            Dim enumRet As SaveReturnTypes = RemarksSaveDataCallback()
            If enumRet = SaveReturnTypes.ChangesSaved Then
                Me.LoadRemarks(True)
                cbp.JSProperties("cpReturnValue") = "OK"
            Else
                cbp.JSProperties("cpReturnValue") = ""
            End If
        End If

    End Sub

    Protected Sub CallbackPanelLocalization_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPanelLocalization.Callback
        Me.LoadLocalizationData()
    End Sub

#End Region

#Region "HORARIO"

    Private Sub LoadShiftUsed(Optional ByVal bolReload As Boolean = False)
        Dim tbDailySchedule As DataTable = Me.DailyScheduleDataTable(bolReload)
        If tbDailySchedule IsNot Nothing AndAlso tbDailySchedule.Rows.Count > 0 Then

            Dim oShiftInfo As Robotics.Base.DTOs.roCalendarShift = CalendarServiceMethods.GetShiftDefinition(Me.Page, roTypes.Any2Integer(tbDailySchedule.Rows(0)("IDShift1")))
            Me.ibtShiftSelector.Visible = (Not oShiftInfo.AllowFloating AndAlso Not oShiftInfo.AllowComplementary)

            Me.spRemoveHolidays.Visible = (Me.ibtShiftSelector.Visible And roTypes.Any2Boolean(tbDailySchedule.Rows(0)("IsHolidays")))
            Me.ibtRemoveHolidays.Visible = Me.spRemoveHolidays.Visible
            Me.lbRemoveHolidays.Visible = Me.spRemoveHolidays.Visible

            'Añadimos la hora de inicio
            Me.txtShiftUsed.Text = String.Empty
            Dim expectedWH As Double = roTypes.Any2Double(tbDailySchedule.Rows(0).Item("ExpectedWorkingHours1"))
            Dim isFloating As Double = roTypes.Any2Double(tbDailySchedule.Rows(0).Item("IsFloating1"))
            Dim idShift As Double = roTypes.Any2Double(tbDailySchedule.Rows(0).Item("IDShift1"))
            Dim startShiftKey As String = "StartShift1"
            If roTypes.Any2Integer(tbDailySchedule.Rows(0).Item("Status")) >= 40 And Not IsDBNull(tbDailySchedule.Rows(0).Item("IDShiftUsed")) And IsDBNull(tbDailySchedule.Rows(0).Item(startShiftKey)) Then
                expectedWH = roTypes.Any2Double(tbDailySchedule.Rows(0).Item("ExpectedWorkingHoursUsedShift"))
                isFloating = roTypes.Any2Double(tbDailySchedule.Rows(0).Item("IsFloatingUsedShift"))
                idShift = roTypes.Any2Double(tbDailySchedule.Rows(0).Item("IDShiftUsed"))
            End If
            Dim strBegin As String = Robotics.Base.VTBusiness.Shift.roShift.GetBeginHourByShiftDX(tbDailySchedule.Rows(0).Item("IDEmployee"), idShift, tbDailySchedule.Rows(0).Item("Date"), 1)
            Dim arrBegin = strBegin.Split(":") 'Formato: E:22:00:00 ...
            If (strBegin.Length > 0 And strBegin.Length > 5 And strBegin.StartsWith("E:") And arrBegin.Length > 2) Then
                strBegin = arrBegin(1) + ":" + arrBegin(2)
                If (expectedWH > 0) Then
                    Me.txtShiftUsed.Text = "[" + roTypes.Any2String(strBegin) + "] "
                End If
            End If

            If roTypes.Any2Integer(tbDailySchedule.Rows(0).Item("Status")) >= 40 And Not IsDBNull(tbDailySchedule.Rows(0).Item("IDShiftUsed")) Then
                Me.hdnIDShiftPage.Value = roTypes.Any2Integer(tbDailySchedule.Rows(0).Item("IDShiftUsed"))

                Me.txtShiftUsed.Text += roTypes.Any2String(tbDailySchedule.Rows(0).Item("UsedName"))
                Try
                    Dim auxColor As System.Drawing.Color = Nothing
                    If Not IsDBNull(tbDailySchedule.Rows(0).Item("ShiftColor1")) Then
                        auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2String(tbDailySchedule.Rows(0).Item("ShiftColor1")))
                    Else
                        auxColor = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2String(tbDailySchedule.Rows(0).Item("UsedColor")))
                    End If
                    Me.divShiftShape.Style("background-color") = System.Drawing.ColorTranslator.ToHtml(auxColor)
                Catch
                    Me.divShiftShape.Style("background-color") = "white"
                End Try
            Else
                Me.hdnIDShiftPage.Value = roTypes.Any2Integer(tbDailySchedule.Rows(0).Item("IDShift1"))
                Me.txtShiftUsed.Text += roTypes.Any2String(tbDailySchedule.Rows(0).Item("Name1"))
                Try
                    Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(roTypes.Any2String(tbDailySchedule.Rows(0).Item("ShiftColor1")))
                    Me.divShiftShape.Style("background-color") = System.Drawing.ColorTranslator.ToHtml(auxColor)
                Catch
                    Me.divShiftShape.Style("background-color") = "white"
                End Try
            End If
        Else
            Me.divShiftShape.Style("background-color") = "white"
            Me.txtShiftUsed.Text = ""
            Me.hdnIDShiftPage.Value = 0
        End If
    End Sub

#End Region

#Region "AUSENCIAS"

    Private Sub LoadAbsence(Optional ByVal bolReload As Boolean = False)
        Me.lblAbsenceDetailsInfo.Text = Me.AbsenceDataMessage(bolReload)
    End Sub

#End Region

#Region "REMARKS"

    Private Sub LoadRemarks(Optional ByVal bolReload As Boolean = False)
        Me.txtRemarks.Text = Me.RemarkDataMessage(bolReload)
        Me.txtRemarks.Enabled = (Me.FreezingDatePage < Me.DatePage)
    End Sub

#End Region

#Region "LOCALIZATION"

    Public Sub LoadLocalizationData()
        'Solo se carga si la vista seleccionada es la de Mobilidad
        If Me.ViewPage = ViewPageTypes.Mobility Then
            LocalizationMapControl1.Data = getCoordsData()
            LocalizationMapControl1.LoadMap()
        End If
    End Sub

    Private Function getCoordsData() As String
        Dim sb As StringBuilder = New StringBuilder()
        Dim dv As DataView = Me.PunchesDataView()
        Dim isFirstRow As Boolean = True
        For Each dr As DataRowView In dv
            Dim point As String = dr("Location").ToString().Replace(" ", "")
            point = point.Replace(",", ";")
            If isFirstRow Then
                isFirstRow = False
            Else
                sb.Append("|")
            End If
            If Not String.IsNullOrEmpty(point) AndAlso point <> "0;0" Then
                sb.Append(point)
            End If
        Next
        Return sb.ToString()
    End Function

#End Region

#Region "PRIVADAS"

    Private Sub AssignTemplatesInReadOnlyColumns(ByVal EditCause As Boolean, ByVal EditCostCenter As Boolean, ByVal bolEditComboCause As Boolean)
        Dim template As Scheduler_ReadOnlyTemplate = New Scheduler_ReadOnlyTemplate()
        CType(GridIncidences.Columns("TextoIncidencia"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("TimeZoneName"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("BeginTime"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("EndTime"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("ValueHora"), GridViewDataColumn).EditItemTemplate = template
        CType(GridIncidences.Columns("Calculado"), GridViewDataColumn).EditItemTemplate = template

        If Not EditCause Then
            CType(GridIncidences.Columns("IDCause"), GridViewDataColumn).EditItemTemplate = template
        End If

        If Not bolEditComboCause Then
            CType(GridIncidences.Columns("IDCause"), GridViewDataColumn).EditItemTemplate = template
        End If

        If Not EditCostCenter Then CType(GridIncidences.Columns("IDCenter"), GridViewDataColumn).EditItemTemplate = template

        If Not EditCause And Not EditCostCenter Then CType(GridIncidences.Columns("ValueHoraEditable"), GridViewDataColumn).EditItemTemplate = template

    End Sub

    Private Function IsDirectCause(ByVal VisibleIndex As Integer) As Boolean
        Dim num As Integer = roTypes.Any2Integer(GridIncidences.GetRowValues(VisibleIndex, "IDRelatedIncidence"))
        Return (num = 0)
    End Function

    Private Function AllowModifyCause(ByVal IDCause As Integer) As Boolean
        ' verifica si el supervisor tiene permisos para modificar la justificacion

        Dim bolRet As Boolean = True

        ' Justificaciones permitidas
        Dim dtVisibleCauses As DataTable = GetJustCausesData().Table
        Dim oRows() As DataRow = dtVisibleCauses.Select("ID = " & IDCause)

        If oRows IsNot Nothing AndAlso oRows.Length = 0 Then bolRet = False

        Return bolRet
    End Function

    Private Function CheckPeriodOfFreezing() As Boolean
        Dim bolRet As Boolean = True

        If Me.DatePage Is Nothing Then
            bolRet = False
        Else
            Dim bType As Boolean
            Me.FreezingDatePage = API.EmployeeServiceMethods.GetEmployeeLockDatetoApply(Me.Page, Me.IDEmployeePage(), bType, False)

            If (Me.FreezingDatePage >= Me.DatePage) Then
                bolRet = False
            End If

            If bolRet Then
                bolRet = False
                ' Verificamos que el empleado tenga contrato para la fecha activa
                Dim tbContracts As DataTable = API.ContractsServiceMethods.GetContractsByIDEmployee(Me.Page, Me.IDEmployeePage, False)
                If tbContracts IsNot Nothing Then
                    bolRet = (tbContracts.Select("BeginDate <= '" & Format(Me.DatePage, "yyyy/MM/dd") & "' AND EndDate >= '" & Format(Me.DatePage, "yyyy/MM/dd") & "'").Length > 0)
                End If
            End If

            If bolRet Then
                ' Verificamos que no estemos intentando modificar un día futuro.
                bolRet = (Me.DatePage <= Now.Date)
            End If

        End If

        Return bolRet
    End Function

    Private Function CheckPeriodOfFreezingWithoutFuture() As Boolean
        Dim bolRet As Boolean = True

        If Me.DatePage Is Nothing Then
            bolRet = False
        Else
            Dim bType As Boolean
            Me.FreezingDatePage = API.EmployeeServiceMethods.GetEmployeeLockDatetoApply(Me.Page, Me.IDEmployeePage(), bType, False)

            If (Me.FreezingDatePage >= Me.DatePage) Then
                bolRet = False
            End If

            If bolRet Then
                bolRet = False
                ' Verificamos que el empleado tenga contrato para la fecha activa
                Dim tbContracts As DataTable = API.ContractsServiceMethods.GetContractsByIDEmployee(Me.Page, Me.IDEmployeePage, False)
                If tbContracts IsNot Nothing Then
                    bolRet = (tbContracts.Select("BeginDate <= '" & Format(Me.DatePage, "yyyy/MM/dd") & "' AND EndDate >= '" & Format(Me.DatePage, "yyyy/MM/dd") & "'").Length > 0)
                End If
            End If
        End If

        Return bolRet
    End Function

#End Region

#Region "DETECCION CAMBIOS PENDIENTES"

    Private Function IncidencesHasChanges(Optional ByVal tbIncidences As DataTable = Nothing) As Boolean
        Dim bolRet As Boolean = False
        If tbIncidences Is Nothing Then
            tbIncidences = Me.IncidencesDataTable
        End If
        Dim tbChanges As DataTable = tbIncidences.GetChanges()
        If Not tbChanges Is Nothing AndAlso tbChanges.Rows.Count > 0 Then
            bolRet = True
        End If
        Return bolRet
    End Function

    Private Function MovesHasChanges(Optional ByVal tbMoves As DataTable = Nothing) As Boolean
        Dim bolRet As Boolean = False
        If tbMoves Is Nothing Then
            tbMoves = Me.PunchesDataTable
        End If
        Dim tbChanges As DataTable = tbMoves.GetChanges()
        If Not tbChanges Is Nothing AndAlso tbChanges.Rows.Count > 0 Then
            bolRet = True
        End If
        Return bolRet
    End Function

    Private ReadOnly Property RemarksHasChanges() As Boolean
        Get
            Dim strRemarks As String = Me.RemarkDataMessage
            If strRemarks Is Nothing Then strRemarks = ""
            Return (strRemarks <> Me.txtRemarks.Text.Trim)
        End Get
    End Property

#End Region

#Region "GUARDAR CAMBIOS PENDIENTES"

    Private Function IncidencesSaveDataCallback() As SaveReturnTypes
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined
        Try
            Dim tbIncidences As DataTable = Me.IncidencesDataTable
            If IncidencesHasChanges(tbIncidences) Then
                hdnClientStatus.Value = ""
                Dim bolRet As Boolean = API.EmployeeServiceMethods.SaveDailyCauses(Me.Page, Me.IDEmployeePage, Me.DatePage, tbIncidences, True, True)
                If bolRet Then
                    enumRet = SaveReturnTypes.ChangesSaved
                End If
            Else
                enumRet = SaveReturnTypes.NoChangesSaved
            End If
        Catch ex As Exception
            enumRet = SaveReturnTypes.IsError
        End Try
        Return enumRet
    End Function

    Private Function MovesSaveDataCallback() As SaveReturnTypes
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined
        Try
            Dim tbMoves As DataTable = Me.PunchesDataTable
            If MovesHasChanges(tbMoves) Then
                hdnClientStatus.Value = ""
                Dim bolRet As Boolean = API.PunchServiceMethods.SavePunches(Me.Page, tbMoves, True)
                If bolRet Then
                    enumRet = SaveReturnTypes.ChangesSaved
                End If
            Else
                enumRet = SaveReturnTypes.NoChangesSaved
            End If
        Catch ex As Exception
            enumRet = SaveReturnTypes.IsError
        End Try
        Return enumRet
    End Function

    Private Function RemarksSaveDataCallback() As SaveReturnTypes
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined
        Try
            If RemarksHasChanges() Then
                Dim bolRet As Boolean = API.EmployeeServiceMethods.SaveRemarkText(Me.Page, Me.IDEmployeePage, Me.DatePage, Me.txtRemarks.Text.Trim)
                If bolRet Then
                    enumRet = SaveReturnTypes.ChangesSaved
                End If
            Else
                enumRet = SaveReturnTypes.NoChangesSaved
            End If
        Catch ex As Exception
            enumRet = SaveReturnTypes.IsError
        End Try
        Return enumRet
    End Function

    '===============================================================================
    Private Function PageSaveData() As Boolean
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined

        enumRet = MovesSaveData()
        If enumRet = SaveReturnTypes.ChangesSaved Then
            Me.MustRefresh = "1"
        End If
        If enumRet <> SaveReturnTypes.IsError Then
            enumRet = IncidencesSaveData()
            If enumRet <> SaveReturnTypes.IsError Then
                enumRet = RemarksSaveData()
            End If
        End If
        hdnClientStatus.Value = ""
        Return (enumRet <> SaveReturnTypes.IsError)
    End Function

    Private Function IncidencesSaveData() As SaveReturnTypes
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined
        Try
            Dim tbIncidences As DataTable = Me.IncidencesDataTable
            If IncidencesHasChanges(tbIncidences) Then
                hdnClientStatus.Value = ""
                Dim bolRet As Boolean = API.EmployeeServiceMethods.SaveDailyCauses(Me.Page, Me.IDEmployeePage, Me.DatePage, tbIncidences, True, True)
                If bolRet Then
                    Me.BindGridIncidences(True)
                    enumRet = SaveReturnTypes.ChangesSaved
                End If
            Else
                enumRet = SaveReturnTypes.NoChangesSaved
            End If
        Catch ex As Exception
            enumRet = SaveReturnTypes.IsError
        End Try
        Return enumRet
    End Function

    Private Function MovesSaveData() As SaveReturnTypes
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined
        Try
            Dim tbMoves As DataTable = Me.PunchesDataTable
            If MovesHasChanges(tbMoves) Then
                hdnClientStatus.Value = ""
                Dim bolRet As Boolean = API.PunchServiceMethods.SavePunches(Me.Page, tbMoves, True)
                If bolRet Then
                    Me.BindGridMoves(True)
                    enumRet = SaveReturnTypes.ChangesSaved
                End If
            Else
                enumRet = SaveReturnTypes.NoChangesSaved
            End If
        Catch ex As Exception
            enumRet = SaveReturnTypes.IsError
        End Try
        Return enumRet
    End Function

    Private Function RemarksSaveData() As SaveReturnTypes
        Dim enumRet As SaveReturnTypes = SaveReturnTypes.Undefined
        Try
            If RemarksHasChanges() Then
                Dim bolRet As Boolean = API.EmployeeServiceMethods.SaveRemarkText(Me.Page, Me.IDEmployeePage, Me.DatePage, Me.txtRemarks.Text.Trim)
                If bolRet Then
                    Me.RemarkDataMessage = Me.txtRemarks.Text.Trim
                    enumRet = SaveReturnTypes.ChangesSaved
                End If
            Else
                enumRet = SaveReturnTypes.NoChangesSaved
            End If
        Catch ex As Exception
            enumRet = SaveReturnTypes.IsError
        End Try
        Return enumRet
    End Function

#End Region

#Region "Bookmark de GridSelector"

    Private Function SituarSelector() As Integer
        Dim intNumFila As Integer = -1

        Try
            For i As Integer = 0 To GridSelector.VisibleRowCount - 1
                If Me.IDEmployeePage = roTypes.Any2Integer(GridSelector.GetRowValues(i, "IDEmployee")) Then
                    GridSelector.FocusedRowIndex = i
                    intNumFila = i
                    Exit For
                End If
            Next
        Catch ex As Exception
            intNumFila = -1
            Me.hdnExceptionOccurred.Value = "1"
        End Try

        Return intNumFila
    End Function

    Private ReadOnly Property GridSelectorRowIDEmployee(ByVal intRow As Integer, ByVal bolSituar As Boolean) As Integer
        Get
            Dim intIDEmployee As Integer = 0
            Try
                If intRow >= 0 And Me.GridSelector.VisibleRowCount > intRow Then
                    intIDEmployee = roTypes.Any2Integer(GridSelector.GetRowValues(intRow, "IDEmployee"))
                    If bolSituar Then
                        GridSelector.FocusedRowIndex = intRow
                    End If
                End If
            Catch ex As Exception
                intIDEmployee = -1
                Me.hdnExceptionOccurred.Value = "1"
            End Try

            Return intIDEmployee
        End Get
    End Property

    Private ReadOnly Property GridSelectorRowDate(ByVal intRow As Integer, ByVal bolSituar As Boolean) As Nullable(Of Date)
        Get
            Dim xDateMove As Nullable(Of Date)
            Try
                If intRow >= 0 And Me.GridSelector.VisibleRowCount > intRow Then
                    If Not IsDBNull(Me.GridSelector.GetRowValues(intRow, "Date")) Then
                        xDateMove = Me.GridSelector.GetRowValues(intRow, "Date")
                        If bolSituar Then
                            GridSelector.FocusedRowIndex = intRow
                        End If
                    End If
                End If
            Catch ex As Exception
                xDateMove = Nothing
                Me.hdnExceptionOccurred.Value = "1"
            End Try

            Return xDateMove
        End Get
    End Property

    Private Function GridSelectorGetNextRowIndex() As Integer
        Dim intMaxIndex As Integer = Me.GridSelector.VisibleRowCount - 1
        If intMaxIndex < 0 Then Return -1

        Dim intIndex = Me.GridSelectorRowSel
        If intIndex < intMaxIndex Then
            intIndex = intIndex + 1
            Me.GridSelectorRowSel = intIndex
        End If
        Return intIndex
    End Function

    Private Function GridSelectorGetPreviousRowIndex() As Integer
        Dim intMaxIndex As Integer = Me.GridSelector.VisibleRowCount - 1
        If intMaxIndex < 0 Then Return -1

        Dim intIndex = Me.GridSelectorRowSel
        If intIndex > 0 Then
            intIndex = intIndex - 1
            Me.GridSelectorRowSel = intIndex
        End If
        Return intIndex
    End Function

#End Region

    Private Sub PreviousEmployee(ByVal bolAskIfNeeded As Boolean)
        If Me.GridSelectorRowSel > 0 Then
            Dim intRowIndex As Integer = Me.GridSelectorRowSel

            Dim intIDEmployeeSelected As Integer = Me.GridSelectorRowIDEmployee(intRowIndex, True)
            If intIDEmployeeSelected > 0 Then
                If Me.IDEmployeePage <> intIDEmployeeSelected Then
                    Me.IDEmployeePage = intIDEmployeeSelected
                Else
                    Me.GridSelectorRowSel -= 1
                    Me.IDEmployeePage = Me.GridSelectorRowIDEmployee(Me.GridSelectorRowSel, True)
                End If
            Else
                Me.GridSelectorRowSel -= 1
                Me.IDEmployeePage = Me.GridSelectorRowIDEmployee(Me.GridSelectorRowSel, True)
            End If

            Me.LoadData(True, False)
        Else
            Me.LoadData(False, False)

        End If
    End Sub

    Private Sub NextEmployee(ByVal bolAskIfNeeded As Boolean)
        If Me.GridSelectorRowSel < Me.GridSelector.VisibleRowCount - 1 Then
            Dim intRowIndex As Integer = Me.GridSelectorRowSel

            Dim intIDEmployeeSelected As Integer = Me.GridSelectorRowIDEmployee(intRowIndex, True)
            If intIDEmployeeSelected > 0 Then
                If Me.IDEmployeePage <> intIDEmployeeSelected Then
                    Me.IDEmployeePage = intIDEmployeeSelected
                Else
                    Me.GridSelectorRowSel += 1
                    Me.IDEmployeePage = Me.GridSelectorRowIDEmployee(Me.GridSelectorRowSel, True)
                End If
            Else
                Me.GridSelectorRowSel += 1
                Me.IDEmployeePage = Me.GridSelectorRowIDEmployee(Me.GridSelectorRowSel, True)
            End If

            Me.LoadData(True, False)
        Else
            Me.LoadData(False, False)
        End If
    End Sub

    Private Sub PreviousDate(ByVal bolAskIfNeeded As Boolean)
        Try
            Dim dateChanged As Boolean = True
            If Me.IDEmployeePage > 0 AndAlso Me.IdFilteredTask > 0 Then
                Dim tmpDate As DateTime = API.TasksServiceMethods.GetAntTaskPunchDate(Me.Page, Me.IdFilteredTask, Me.IDEmployeePage, Me.DatePage)
                If (tmpDate.Year = Me.DatePage.Value.Year AndAlso tmpDate.Month = Me.DatePage.Value.Month AndAlso tmpDate.Day = Me.DatePage.Value.Day) Then
                    dateChanged = False
                End If
                Me.DatePage = New Date(tmpDate.Year, tmpDate.Month, tmpDate.Day)
            Else
                Me.DatePage = Me.DatePage.Value.AddDays(-1)
            End If
            If (dateChanged) Then
                Dim intRowIndex As Integer = Me.GridSelectorRowSel
                BindGridSelector(True)
                GridSelector.DataBind()
                Dim intIDEmployeeSelected As Integer = Me.GridSelectorRowIDEmployee(intRowIndex, True)
                If intIDEmployeeSelected > 0 AndAlso SituarSelector() = -1 Then
                    If Me.IDEmployeePage <> intIDEmployeeSelected Then
                        Me.IDEmployeePage = intIDEmployeeSelected
                        Me.hdnMustShowUserAlert.Value = 1
                    End If
                End If

                Me.LoadData(True, False)
                Me.ibtNextDate.Visible = True
            Else
                Me.LoadData(False, False)
                Me.ibtPreviousDate.Visible = False
            End If
        Catch ex As Exception
            Me.hdnExceptionOccurred.Value = "1"
        End Try
    End Sub

    Private Sub NextDate(ByVal bolAskIfNeeded As Boolean)
        Try
            Dim dateChanged As Boolean = True
            If Me.IDEmployeePage > 0 AndAlso Me.IdFilteredTask > 0 Then
                Dim tmpDate As DateTime = API.TasksServiceMethods.GetNextTaskPunchDate(Me.Page, Me.IdFilteredTask, Me.IDEmployeePage, Me.DatePage)
                If (tmpDate.Year = Me.DatePage.Value.Year AndAlso tmpDate.Month = Me.DatePage.Value.Month AndAlso tmpDate.Day = Me.DatePage.Value.Day) Then
                    dateChanged = False
                End If
                Me.DatePage = New Date(tmpDate.Year, tmpDate.Month, tmpDate.Day)
            Else
                Me.DatePage = Me.DatePage.Value.AddDays(1)
            End If
            If (dateChanged) Then
                Dim intRowIndex As Integer = Me.GridSelectorRowSel
                BindGridSelector(True)
                GridSelector.DataBind()
                Dim intIDEmployeeSelected As Integer = Me.GridSelectorRowIDEmployee(intRowIndex, True)
                If intIDEmployeeSelected > 0 AndAlso SituarSelector() = -1 Then
                    If Me.IDEmployeePage <> intIDEmployeeSelected Then
                        Me.IDEmployeePage = intIDEmployeeSelected
                        Me.hdnMustShowUserAlert.Value = 1
                    End If
                End If
                SituarSelector()

                Me.LoadData(True, False)
                Me.ibtPreviousDate.Visible = True
            Else
                Me.LoadData(False, False)
                Me.ibtNextDate.Visible = False
            End If
        Catch ex As Exception
            Me.hdnExceptionOccurred.Value = "1"
        End Try
    End Sub

    Private Sub ChangeSelector(ByVal bolAskIfNeeded As Boolean, ByVal bolSituar As Boolean)
        Try
            Dim intRowIndex As Integer = SelectorSelectedRowIndex
            Dim intIDEmployeeSelected As Integer = Me.GridSelectorRowIDEmployee(intRowIndex, False)
            Dim oldSelectorDate As DateTime = Me.GridSelectorRowDate(intRowIndex, False).Value

            If selectorDataChanged(intIDEmployeeSelected, oldSelectorDate) Then

                Dim curDv As DataView = SelectorDataView
                curDv.Table.Rows(intRowIndex).Delete()
                SelectorDataView = curDv

                BindGridSelector(False)
                GridSelector.DataBind()

                If GridSelector.FocusedRowIndex >= Me.GridSelectorRowSel Then
                    Me.GridSelectorRowSel = GridSelector.FocusedRowIndex - 1
                    Me.GridSelector.FocusedRowIndex = Me.GridSelectorRowSel
                End If
            End If

            Me.GridSelectorRowSel = GridSelector.FocusedRowIndex
            intRowIndex = Me.GridSelectorRowSel
            Dim newIDEmployeeSelected As Integer = Me.GridSelectorRowIDEmployee(intRowIndex, bolSituar)
            Dim newSelectorDate As Nullable(Of Date) = Me.GridSelectorRowDate(intRowIndex, bolSituar)

            If newIDEmployeeSelected > 0 Then
                SelectorSelectedRowIndex = intRowIndex

                If Me.IDEmployeePage <> newIDEmployeeSelected Then
                    Me.IDEmployeePage = newIDEmployeeSelected
                End If

                Dim xDateMoves As DateTime = Me.GridSelectorRowDate(intRowIndex, bolSituar).Value
                If xDateMoves <> Me.DatePage Then
                    Me.DatePage = xDateMoves
                End If

                Dim myCookie As HttpCookie = Response.Cookies("Moves_SelectorVisible")
                If myCookie IsNot Nothing Then
                    Response.Cookies("Moves_SelectorVisible").Value = False
                End If

                Me.LoadData(True, False)
            Else

                Me.LoadData(False, False)
                SelectorSelectedRowIndex = 0
            End If
        Catch ex As Exception
            Me.hdnExceptionOccurred.Value = "1"
        End Try
    End Sub

    Private Function selectorDataChanged(ByVal idEmployee As Integer, ByVal selectedDate As Date) As Boolean
        Dim bolRet As Boolean = True

        Dim strAction As String = roTypes.Any2String(Request.QueryString("action"))
        If strAction <> String.Empty Then
            Select Case strAction
                Case "notjustifiedDays"
                    Dim tb As DataTable = API.EmployeeServiceMethods.GetIncompleteIncidences(Me.Page, selectedDate, selectedDate, -1, idEmployee)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then bolRet = False
                Case "incompletedDays"
                    Dim tb As DataTable = API.EmployeeServiceMethods.GetIncompleteDays(Me.Page, selectedDate, selectedDate, -1, idEmployee)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then bolRet = False
                Case "notreliabledDays"
                    Dim tb As DataTable = API.EmployeeServiceMethods.GetSuspiciousPunches(Me.Page, selectedDate, selectedDate, -1, idEmployee)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then bolRet = False
                Case Else
            End Select
        End If

        Return bolRet
    End Function

    Private Sub ChangeDate(ByVal bolAskIfNeeded As Boolean)
        Me.DatePage = txtDatePage.Value
        Me.LoadData(True, False)
    End Sub

    Private Function EditingData() As Boolean
        Try
            If Me.GridIncidences.IsEditing Then
                HelperWeb.ShowMessage(Me.Page, "", Me.Language.Translate("DataEditing.Message", Me.DefaultScope))
                Return True
            Else
                Return False
            End If
        Catch
        End Try
        Return False
    End Function

    Private Function HasChanges() As Boolean
        Dim bolHasChanges As Boolean = False

        bolHasChanges = IncidencesHasChanges()
        If Not bolHasChanges Then
            bolHasChanges = MovesHasChanges()
            If Not bolHasChanges Then
                bolHasChanges = RemarksHasChanges()
            End If
        End If
        Return bolHasChanges
    End Function

    Private Function AskSaveDataIfNeeded(ByVal strActionKey As String) As Boolean

        Dim bolAsked As Boolean = False

        If Not Me.EditingData Then
            'Me.GridIncidences.IsEditing Then
            If Me.HasChanges() = True Then
                bolAsked = True
                HelperWeb.ShowOptionMessage(Me.Page, Me.Language.Translate("SaveConfirmation.Title", Me.DefaultScope), "",
                                                  Me.Language.Translate("SaveConfirmation.Answer.Yes.Title", Me.DefaultScope), "SaveDataKey_" & strActionKey, Me.Language.Translate("SaveConfirmation.Answer.Yes.Description", Me.DefaultScope), "", False, True,
                                                  Me.Language.Translate("SaveConfirmation.Answer.No.Title", Me.DefaultScope), "NoSaveDataKey_" & strActionKey, Me.Language.Translate("SaveConfirmation.Answer.No.Description", Me.DefaultScope), "", False, True,
                                                  Me.Language.Translate("SaveConfirmation.Answer.Cancel.Title", Me.DefaultScope), "CancelKey", Me.Language.Translate("SaveConfirmation.Answer.Cancel.Description", Me.DefaultScope), "", False, True,
                                                  "", "", "", "", False, True, "", HelperWeb.MsgBoxIcons.QuestionIcon)

            End If
        Else
            bolAsked = True
        End If

        Return bolAsked

    End Function

    Private Sub EmptySession()
        Session("MovesPage_CurrentPermission") = Nothing
        Session("MovesPage_CurrentDirectCausePermission") = Nothing
        Session("MovesPage_CurrentAccrualsSchedulerPermission") = Nothing
        Session("MovesPage_TaskPunchesPermission") = Nothing
        Session("MovesPage_DiningRoomPunchesPermission") = Nothing
        Session("MovesPage_DirectCausesPermission") = Nothing
        Session("MovesPage_CostCenterPermission") = Nothing
        Session("MovesPage_CurrentJustifyIncPermission") = Nothing
        Session("MovesPage_RemarksPermission") = Nothing
        Session("MovesPage_JustifyIncPermission") = Nothing
        Session("MovesPage_FeatureCostCenter") = Nothing
        Session("MovesPage_IdFilteredTask") = Nothing
        Session("MovesPage_IDEmployeePageInitial") = Nothing
        Session("MovesPage_EmployeeFilter") = Nothing
        Session("MovesPage_IsNewVersion") = Nothing
        Session("MovesPage_HasAction") = Nothing
        Session("MovesPage_SelectorBeginDate") = Nothing
        Session("MovesPage_SelectorEndDate") = Nothing
        Session("MovesPage_GridSelectorRowSel") = Nothing
        Session("MovesPage_IDEmployeePage") = Nothing
        Session("MovesPage_DatePage") = Nothing
        Session("MovesPage_CheckPeriodOfFreezingPage") = Nothing
        Session("MovesPage_ViewPage") = Nothing
        Session("MovesPage_IdGroup") = Nothing

        Session("MovesPage_PunchesData") = Nothing
        Session("MovesPage_MovesLastIDEmployee") = Nothing
        Session("MovesPage_MovesLastDate") = Nothing
        Session("MovesPage_MovesLastViewPage") = Nothing

        Session("MovesPage_AvailableCentersByEmployee") = Nothing

        Session("MovesPage_IncidencesDatatable") = Nothing
        Session("MovesPage_IncidencesLastIDEmployee") = Nothing
        Session("MovesPage_IncidencesLastDate") = Nothing

        Session("MovesPage_AcumAnualDatatable") = Nothing
        Session("MovesPage_AcumAnualLastIDEmployee") = Nothing
        Session("MovesPage_AcumAnualLastDate") = Nothing
        Session("MovesPage_AcumAnualLastView") = Nothing

        Session("MovesPage_AcumDailyDatatable") = Nothing
        Session("MovesPage_AcumDailyLastIDEmployee") = Nothing
        Session("MovesPage_AcumDailyLastDate") = Nothing
        Session("MovesPage_AcumDailyLastView") = Nothing

        Session("MovesPage_SelectorData") = Nothing
        Session("MovesPage_LastSelectorType") = Nothing

        Session("LabelDay") = Nothing
        Session("LabelContract") = Nothing
        Session("LabelWeek") = Nothing
        Session("LabelMonth") = Nothing
        Session("LabelYear") = Nothing

    End Sub

    Private Sub LoadViewsCombo()

        Me.cmbView.Items.Clear()

        Me.cmbView.Items.Add(New ListEditItem(Me.Language.Translate("cbViewAttendanceandAccess", Me.DefaultScope), Integer.Parse(ViewPageTypes.Moves)))
        Me.cmbView.SelectedIndex = 0

        If bolFeatureTasks And Me.TaskPunchesPermission >= Permission.Read Then
            Me.cmbView.Items.Add(New ListEditItem(Me.Language.Translate("cbViewTasks", Me.DefaultScope), Integer.Parse(ViewPageTypes.Tasks)))
            If Me.IdFilteredTask > 0 Then
                Me.cmbView.SelectedIndex = 1
            End If
        End If

        If bolFeatureAnyWhere Then
            Me.cmbView.Items.Add(New ListEditItem(Me.Language.Translate("cbViewMobility", Me.DefaultScope), Integer.Parse(ViewPageTypes.Mobility)))
        End If

        If bolFeatureDiningRoom And Me.DiningRoomPunchesPermission >= Permission.Read Then
            Me.cmbView.Items.Add(New ListEditItem(Me.Language.Translate("cbViewDiningRoom", Me.DefaultScope), Integer.Parse(ViewPageTypes.DinningRoom)))
        End If

    End Sub

#Region "Funciones privadas complementarias"

    Private Function GetEmployeeName(ByVal id As Integer) As String
        Dim name As String = ""
        If id > 0 Then
            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, id, False)
            If oEmployee IsNot Nothing Then
                name = oEmployee.Name
            End If
        End If
        Return name
    End Function

    Private Function LoadEmployeesSelector(ByVal _IDGroup As Integer, Optional employeesSelected As String = "") As DataView
        ''MyLog.WriteLog(Me, "Init LoadEmployeesSelector")
        Dim dv As DataView = Nothing

        ' Obtener la definición de los empleados
        Dim tb As DataTable = Nothing
        tb = API.SchedulerServiceMethods.GetScheduledEmployeesFromList(Me.Page, employeesSelected)

        If tb IsNot Nothing Then

            'añado columna de la clave
            tb.Columns.Add(New DataColumn("Clave", GetType(String)))
            For Each r As DataRow In tb.Rows
                r("Clave") = Guid.NewGuid()
            Next
            'añade campo clave a la tabla
            tb.PrimaryKey = New DataColumn() {tb.Columns("Clave")}
            tb.AcceptChanges()

            tb.Columns.Add(New DataColumn("Date", GetType(Date)))

            If tb.Rows.Count = 0 Then
                Dim x As DataRow = tb.NewRow()
                x("Clave") = Guid.NewGuid()
                tb.Rows.Add(x)
            End If

            dv = New DataView(tb)
            'dv.RowFilter = "CurrentEmployee > 0"
            dv.Sort = "Path, Name ASC"

            If _IDGroup > -1 Then
                For Each oRow As DataRow In tb.Rows
                    oRow("Date") = Me.DatePage
                Next
            End If
        End If

        Return dv

    End Function

    Private Function LoadIncompleteMovesSelector(ByVal _IDGroup As Integer, ByVal _IDEmployee As String) As DataView
        Dim dv As DataView = Nothing
        Dim tb As DataTable = API.EmployeeServiceMethods.GetIncompleteIncidences(Me.Page, CDate(Me.SelectorBeginDate), CDate(SelectorEndDate), _IDGroup, _IDEmployee)
        If Not tb Is Nothing Then
            'añado columna de la clave
            tb.Columns.Add(New DataColumn("Clave", GetType(String)))
            For Each r As DataRow In tb.Rows
                r("Clave") = Guid.NewGuid()
            Next
            'añade campo clave a la tabla
            tb.PrimaryKey = New DataColumn() {tb.Columns("Clave")}
            tb.AcceptChanges()

            dv = New DataView(tb)
            dv.Sort = "Name, Date"
        End If
        Return dv
    End Function

    Private Function LoadIncompleteIncidencesSelector(ByVal _IDGroup As Integer, ByVal _IDEmployee As String) As DataView
        Dim dv As DataView = Nothing
        Dim tb As DataTable = API.EmployeeServiceMethods.GetIncompleteDays(Me.Page, CDate(Me.SelectorBeginDate), CDate(Me.SelectorEndDate), _IDGroup, _IDEmployee)
        If Not tb Is Nothing Then
            'añado columna de la clave
            tb.Columns.Add(New DataColumn("Clave", GetType(String)))
            For Each r As DataRow In tb.Rows
                r("Clave") = Guid.NewGuid()
            Next
            'añade campo clave a la tabla
            tb.PrimaryKey = New DataColumn() {tb.Columns("Clave")}
            tb.AcceptChanges()

            dv = New DataView(tb)
            dv.Sort = "Name, Date"
        End If
        Return dv
    End Function

    Private Function LoadSuspiciousMovesSelector(ByVal _IDGroup As Integer, ByVal _IDEmployee As String) As DataView
        Dim dv As DataView = Nothing
        Dim tb As DataTable = API.EmployeeServiceMethods.GetSuspiciousPunches(Me.Page, CDate(Me.SelectorBeginDate), CDate(Me.SelectorEndDate), _IDGroup, _IDEmployee)
        If Not tb Is Nothing Then
            'añado columna de la clave
            tb.Columns.Add(New DataColumn("Clave", GetType(String)))
            For Each r As DataRow In tb.Rows
                r("Clave") = Guid.NewGuid()
            Next
            'añade campo clave a la tabla
            tb.PrimaryKey = New DataColumn() {tb.Columns("Clave")}
            tb.AcceptChanges()

            dv = New DataView(tb)
            dv.Sort = "Name, Date"
        End If
        Return dv
    End Function

    Private Sub GetDatesPeriod()
        Dim strPeriodType As String = HelperWeb.GetCookie("_Selector_Period_TypeDate")
        Dim strDateFormat As String = HelperWeb.GetShortDateFormat
        Select Case strPeriodType
            Case "Today"
                Me.SelectorBeginDate = Format(Now, strDateFormat)
                Me.SelectorEndDate = Format(Now, strDateFormat)
            Case "Yesterday"
                Me.SelectorBeginDate = Format(Now.AddDays(-1), strDateFormat)
                Me.SelectorEndDate = Format(Now.AddDays(-1), strDateFormat)
            Case "CurrentWeek"
                'Case "CurrentWeek"
                Me.SelectorBeginDate = Format(Now.AddDays(1 - Weekday(Now, vbMonday)), strDateFormat)
                Me.SelectorEndDate = Format(Now.AddDays(7 - Weekday(Now, vbMonday)), strDateFormat)
            Case "LastWeek"
                Me.SelectorBeginDate = Format(Now.AddDays(-6 - Weekday(Now, vbMonday)), strDateFormat)
                Me.SelectorEndDate = Format(Now.AddDays(-Weekday(Now, vbMonday)), strDateFormat)
            Case "CurrentMonth"
                Me.SelectorBeginDate = Format(New Date(Now.Year, Now.Month, 1), strDateFormat)
                Me.SelectorEndDate = Format(New Date(Now.Year, Now.Month, (New Date(Now.Year, Now.Month, 1)).AddMonths(1).AddDays(-1).Day), strDateFormat)
            Case "LastMonth"
                Dim dtMonth As Date
                dtMonth = Now.AddMonths(-1)
                Me.SelectorBeginDate = Format(New Date(dtMonth.Year, dtMonth.Month, 1), strDateFormat)
                Me.SelectorEndDate = Format(New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day), strDateFormat)
            Case "CurrentYear"
                Me.SelectorBeginDate = Format(New Date(Now.Year, 1, 1), strDateFormat)
                Me.SelectorEndDate = Format(Now, strDateFormat)
            Case "Other"
                Dim strBeginDate As String = HelperWeb.GetCookie("_Selector_Period_startDate")
                Dim xBeginDate As Date
                If strBeginDate = "" Then
                    xBeginDate = New Date(Now.Year, Now.Month, 1)
                Else
                    xBeginDate = New Date(strBeginDate.Substring(6, 4), strBeginDate.Substring(3, 2), strBeginDate.Substring(0, 2))
                End If

                Dim strEndDate As String = HelperWeb.GetCookie("_Selector_Period_endDate")
                Dim xEndDate As Date
                If strEndDate = "" Then
                    xEndDate = New Date(Now.Year, Now.Month, Now.Day)
                Else
                    xEndDate = New Date(strEndDate.Substring(6, 4), strEndDate.Substring(3, 2), strEndDate.Substring(0, 2))
                End If

                Dim strDaysFrom As String = HelperWeb.GetCookie("_Selector_Period_toDays")
                Dim intDaysFrom As Integer = 1
                If strDaysFrom <> "" AndAlso IsNumeric(strDaysFrom) Then intDaysFrom = Val(strDaysFrom)

                Dim strOtherType As String = HelperWeb.GetCookie("_Selector_Period_OtherType")
                If strOtherType = "0" Then
                    'Entre dates
                    Me.SelectorBeginDate = Format(xBeginDate, strDateFormat)
                    Me.SelectorEndDate = Format(xEndDate, strDateFormat)
                Else
                    'Per dies
                    Me.SelectorBeginDate = Format(xBeginDate, strDateFormat)
                    Me.SelectorEndDate = Format(CDate(Me.SelectorBeginDate).AddDays(intDaysFrom), strDateFormat)
                End If
            Case ""
                Dim intervalDays As String = HelperWeb.GetCookie("SchedulerIntervalDates")
                If intervalDays <> "" Then
                    Dim days = intervalDays.Split(",")
                    Dim startDate = days(0).Split("#")
                    Dim endDate = days(1).Split("#")
                    Me.SelectorBeginDate = Format(New Date(roTypes.Any2Integer(startDate(0)), roTypes.Any2Integer(startDate(1)), roTypes.Any2Integer(startDate(2))), strDateFormat)
                    Me.SelectorEndDate = Format(New Date(roTypes.Any2Integer(endDate(0)), roTypes.Any2Integer(endDate(1)), roTypes.Any2Integer(endDate(2))), strDateFormat)
                Else
                    Me.SelectorBeginDate = Format(Now.AddDays(1 - Weekday(Now, vbMonday)), strDateFormat)
                    Me.SelectorEndDate = Format(Now.AddDays(7 - Weekday(Now, vbMonday)), strDateFormat)
                End If
        End Select
    End Sub

    Private Sub CalculateDaysPeriods()
        'Dim SelectorType As String = Request("SelectorType")
        Dim SelectorType As String = "FREEACTION"

        'Dim SelectorInit As String = Request("SelectorInit")
        Dim SelectorInit As String = "ACTUAL"

        'Dim SelectorDatePos As String = Request("SelectorDatePos")
        Dim SelectorDatePos As String = ""

        Dim OtherType As Integer

        Dim dAct As Date
        Dim dEnd As Date

        Dim strDateFormat As String = HelperWeb.GetShortDateFormat

        Select Case SelectorType.ToString.ToUpper
            Case "DAILY"
                Select Case SelectorInit.ToString.ToUpper
                    Case "TODAY"
                        dAct = Date.Now
                        dEnd = Date.Now
                    Case "TOMORROW"
                        dAct = Date.Now.AddDays(1)
                        dEnd = Date.Now.AddDays(1)
                    Case "YESTERDAY"
                        dAct = Date.Now.AddDays(-1)
                        dEnd = Date.Now.AddDays(-1)
                    Case "FREE"
                        dAct = CDate(Request("SelectorDatePos"))
                        dEnd = CDate(Request("SelectorDatePos"))
                End Select
            Case "WEEKLY"
                If Now.DayOfWeek = 0 Then
                    'Si es diumenge, tira enrera 7 dies
                    dAct = Now.AddDays(-7)
                Else
                    dAct = Now.AddDays((Now.DayOfWeek * -1) + 1)
                End If
                Select Case SelectorInit.ToString.ToUpper
                    Case "PREVIOUS"
                        dAct = dAct.AddDays(-7)
                        dEnd = dAct.AddDays(6)
                    Case "ACTUAL"
                        dEnd = dAct.AddDays(6)
                    Case "NEXT"
                        dAct = dAct.AddDays(7)
                        dEnd = dAct.AddDays(6)
                    Case "FREE"
                        dAct = CDate(Request("SelectorDatePos"))
                        If dAct.DayOfWeek = 0 Then
                            'Si es diumenge, tira enrera 7 dies
                            dAct = dAct.AddDays(-7)
                        Else
                            dAct = dAct.AddDays((dAct.DayOfWeek * -1) + 1)
                        End If
                        dEnd = dAct.AddDays(6)
                End Select
            Case "MONTHLY"
                Dim iMonth As Integer = Now.Month
                Dim iYear As Integer = Now.Year
                Select Case SelectorInit.ToString.ToUpper
                    Case "PREVIOUS"
                        If iMonth = 1 Then
                            iMonth = 12
                            iYear -= 1
                        Else
                            iMonth -= 1
                        End If
                    Case "ACTUAL"
                    Case "NEXT"
                        If iMonth = 12 Then
                            iMonth = 1
                            iYear += 1
                        Else
                            iMonth += 1
                        End If
                    Case "FREE"
                        iYear = CDate(Request("SelectorDatePos")).Year
                        iMonth = CDate(Request("SelectorDatePos")).Month
                End Select
                dAct = New Date(iYear, iMonth, 1)
                dEnd = dAct.AddMonths(1)
                dEnd = dEnd.AddDays(-1)
            Case "ANNUAL"
                Dim iFirstDay As Integer = 1
                Dim iLastDay As Integer = 31
                Dim iFirstMonth As Integer = 1
                Dim iLastMonth As Integer = 12
                Dim iYear As Integer = Now.Year
                Select Case SelectorInit.ToString.ToUpper
                    Case "PREVIOUS"
                        iYear -= 1
                    Case "ACTUAL"
                    Case "NEXT"
                        iYear += 1
                    Case "FREE"
                        iYear = CDate(Request("SelectorDatePos")).Year
                End Select
                dAct = New Date(iYear, iFirstMonth, iFirstDay)
                dEnd = New Date(iYear, iLastMonth, iLastDay)
            Case "FREE"
                OtherType = CInt(Request("OtherType"))
                If OtherType = 0 Then
                    'Entre dates
                    dAct = Date.ParseExact(Request("DateStart"), "dd/MM/yyyy", CultureInfo.CurrentCulture)
                    dEnd = Date.ParseExact(Request("DateEnd"), "dd/MM/yyyy", CultureInfo.CurrentCulture)
                Else
                    'Per dies
                    dAct = Date.ParseExact(Request("DateStart"), "dd/MM/yyyy", CultureInfo.CurrentCulture)
                    dEnd = dAct.AddDays(CInt(Request("toDays")) - 1)
                End If

            Case "FREEACTION"
                dAct = Date.ParseExact(Request("DateStart"), "dd/MM/yyyy", CultureInfo.CurrentCulture)
                dEnd = Date.ParseExact(Request("DateEnd"), "dd/MM/yyyy", CultureInfo.CurrentCulture)
            Case Else
        End Select

        Me.SelectorBeginDate = Format(dAct, strDateFormat)
        Me.SelectorEndDate = Format(dEnd, strDateFormat)
    End Sub

#End Region

    'Protected Sub OnMessageClick(ByVal strButtonKey As String)
    '    If strButtonKey <> "CancelKey" Then

    '        If strButtonKey.Split("_").Length > 1 Then

    '            Dim strActionKey As String = strButtonKey.Split("_")(1)
    '            Dim bolProceed As Boolean = False

    '            Select Case strButtonKey.Split("_")(0)
    '                Case "SaveDataKey"
    '                    bolProceed = PageSaveData()

    '                Case "NoSaveDataKey"
    '                    bolProceed = True
    '            End Select

    '            If bolProceed Then
    '                Select Case strActionKey
    '                    Case "PreviousEmployee"
    '                        Me.PreviousEmployee(False)

    '                    Case "NextEmployee"
    '                        Me.NextEmployee(False)

    '                    Case "PreviousDate"
    '                        Me.PreviousDate(False)

    '                    Case "NextDate"
    '                        Me.NextDate(False)

    '                    Case "ChangeDate"
    '                        Me.ChangeDate(False)

    '                    Case "ChangeSelector"
    '                        Me.ChangeSelector(False, True)

    '                    Case "Close"
    '                        Me.CanClose = True

    '                    Case "MixMoves"
    '                        '??? Me.ShowReorderMoves()
    '                End Select
    '            Else
    '                If strActionKey = "ChangeDate" Then

    '                End If
    '            End If
    '        End If
    '    End If
    'End Sub

#Region "BOTONES"

    Protected Sub StatusCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles StatusCallback.Callback

        Dim strParameter As String = Server.UrlDecode(e.Parameter())

        Dim oStatusParameter As New StatusRequest()
        oStatusParameter = roJSONHelper.Deserialize(strParameter, oStatusParameter.GetType())

        Select Case oStatusParameter.Action.ToUpperInvariant
            Case "CANCLOSE"
                If Me.StatusCallback.JSProperties.ContainsKey("cpAction") Then
                    Me.StatusCallback.JSProperties("cpAction") = "CANCLOSE"
                Else
                    Me.StatusCallback.JSProperties.Add("cpAction", "CANCLOSE")
                End If

                Dim bolCanClose = True
                If Not Me.GridIncidences.IsEditing Then
                    bolCanClose = Not Me.AskSaveDataIfNeeded("Close")
                End If

                If Me.StatusCallback.JSProperties.ContainsKey("cpResult") Then
                    Me.StatusCallback.JSProperties("cpResult") = bolCanClose
                Else
                    Me.StatusCallback.JSProperties.Add("cpResult", bolCanClose)
                End If

        End Select

    End Sub

#End Region

    Protected Function UpdateMoveDetail(ByVal KeyRowIndex As String) As Boolean

        If hdnGridChanges.Value = "0" Then Return True

        Dim tbPunches As DataTable = Me.PunchesDataTable

        hdnDetailsMoveDate.Value = DateTime.ParseExact(hdnDetailsShiftDate.Value, "yyyy/MM/dd", Nothing).AddDays(Convert.ToInt16(hdnDaysToAdd.Value)).ToString("yyyy/MM/dd")

        If hdnGridChanges.Value = "1" Then 'Fichaje modificado

            Dim oMoveDataRow As DataRow = tbPunches.Rows.Find(KeyRowIndex)

            Try
                If oMoveDataRow.Item("IDReader") Is Nothing Then oMoveDataRow.Item("IDReader") = 0
                oMoveDataRow.Item("IsModified") = True
            Catch
                Return True
            End Try

            GetDetailProperties(oMoveDataRow)
            oMoveDataRow.Item("IsNotReliable") = IIf(Me.hdnDetailsReliabilityPop.Value = "true", 0, 1)
            oMoveDataRow.Item("InTelecommute") = IIf(Me.hdnDetailsTelecommutingPop.Value = "true", 1, 0)

            Return True

        ElseIf hdnGridChanges.Value = "2" Then 'Fichaje nuevo
            Try

                Dim oNewRow As DataRow = tbPunches.NewRow
                oNewRow.Item("Clave") = Guid.NewGuid()
                oNewRow.Item("ID") = -1
                oNewRow.Item("IsNotReliable") = IIf(Me.hdnDetailsReliabilityPop.Value = "true", 0, 1)
                oNewRow.Item("InTelecommute") = IIf(Me.hdnDetailsTelecommutingPop.Value = "true", 1, 0)
                oNewRow.Item("IDEmployee") = Me.IDEmployeePage
                oNewRow.Item("IDReader") = 0
                oNewRow.Item("IsModified") = True

                GetDetailProperties(oNewRow)

                tbPunches.Rows.Add(oNewRow)

                Return True
            Catch e As Exception
                Return True
            End Try
        End If
        Return True
    End Function

    Private Sub GetDetailProperties(ByRef row As DataRow)
        Dim cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

        row.Item("Type") = Convert.ToInt16(Me.hdnDetailsType.Value)
        row.Item("ActualType") = Convert.ToInt16(Me.hdnDetailsActualType.Value)
        row.Item("ShiftDate") = CDate(Me.hdnDetailsShiftDate.Value)
        Dim time As String() = Me.hdnDetailsMoveTime.Value.Split(":")
        row.Item("DateTime") = DateTime.ParseExact(Me.hdnDetailsMoveDate.Value, "yyyy/MM/dd", Nothing).Date.AddHours(Convert.ToInt16(time(0))).AddMinutes(Convert.ToInt16(time(1))).AddSeconds(Convert.ToInt16(time(2)))
        row.Item("IDTerminal") = Convert.ToInt16(Me.hdnDetailsIdTerminal.Value)
        If row.Item("ActualType") <> 4 And row.Item("ActualType") <> 13 Then
            row.Item("TypeData") = Convert.ToInt16(Me.hdnDetailsIdCause.Value)
        Else
            If row.Item("ActualType") = 4 Then
                row.Item("TypeData") = Convert.ToInt32(Me.hdnDetailsIDTask.Value)
            ElseIf row.Item("ActualType") = 13 Then
                row.Item("TypeData") = Convert.ToInt32(Me.hdnDetailsIdCenter.Value)
            End If
        End If


        row.Item("Action") = Convert.ToInt16(Me.hdnDetailsIdAction.Value)
        row.Item("IDPassport") = Convert.ToInt32(Me.hdnDetailsIdPassport.Value)
        row.Item("Location") = Me.hdnDetailsPosition.Value
        row.Item("LocationZone") = Me.hdnDetailsCity.Value
        row.Item("IDZone") = Convert.ToInt16(Me.hdnDetailsIdZone.Value)
        row.Item("Field1") = Me.hdnDetailsField1Pop.Value
        row.Item("Field2") = Me.hdnDetailsField2Pop.Value
        row.Item("Field3") = Me.hdnDetailsField3Pop.Value
        If Me.hdnDetailsField4Pop.Value = "" OrElse Not IsNumeric(Me.hdnDetailsField4Pop.Value) Then
            Me.hdnDetailsField4Pop.Value = "0"
        End If
        If Me.hdnDetailsField5Pop.Value = "" OrElse Not IsNumeric(Me.hdnDetailsField5Pop.Value) Then
            Me.hdnDetailsField5Pop.Value = "0"
        End If
        If Me.hdnDetailsField6Pop.Value = "" OrElse Not IsNumeric(Me.hdnDetailsField6Pop.Value) Then
            Me.hdnDetailsField6Pop.Value = "0"
        End If

        row.Item("Field4") = roTypes.Any2Double(Me.hdnDetailsField4Pop.Value.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
        row.Item("Field5") = roTypes.Any2Double(Me.hdnDetailsField5Pop.Value.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
        row.Item("Field6") = roTypes.Any2Double(Me.hdnDetailsField6Pop.Value.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)

        If Me.hdnDetailsInvalidtype.Value = "-1" Or Not IsNumeric(Me.hdnDetailsInvalidtype.Value) Then
            row.Item("InvalidType") = DBNull.Value
        Else
            row.Item("InvalidType") = roTypes.Any2Integer(Me.hdnDetailsInvalidtype.Value)
        End If

        If Me.hdnDetailsTypeDetails.Value = "" Then
            row.Item("TypeDetails") = DBNull.Value
        Else
            row.Item("TypeDetails") = Me.hdnDetailsTypeDetails.Value
        End If

    End Sub

    '=== DETALLES DEL FICHAJE ======================================================

    Protected Sub CallbackPanelDetails_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPanelDetails.Callback
        Dim strParameter As String = Server.UrlDecode(e.Parameter())

        Dim oMoveDetailsParameter As New MoveDetailsParameter()
        oMoveDetailsParameter = roJSONHelper.Deserialize(strParameter, oMoveDetailsParameter.GetType())

        If oMoveDetailsParameter.IsNewMove.ToUpper = "TRUE" Then
            oMoveDetailsParameter.CanEdit = "true"
        End If

        oMoveDetailsParameter.ActualDate = oMoveDetailsParameter.ActualDate.Replace("_", " ")

        LoadDataDetailMove(oMoveDetailsParameter)
    End Sub

    Private Sub LoadDataDetailMove(ByVal oMoveDetailsParameter As MoveDetailsParameter)

        Dim strPunchId As String = String.Empty

        bolFeatureAnyWhere = True
        bolFeatureTasks = HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv")
        bolFeatureDiningRoom = HelperSession.GetFeatureIsInstalledFromApplication("Forms\DiningRoom")
        FeatureCostCenter = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")

        hdnDetailsIdPassportModifiedPop.Value = WLHelperWeb.CurrentPassport.ID

        If Boolean.Parse(oMoveDetailsParameter.IsNewMove) Then
            Me.hdnDetailsKeyRowPop.Value = String.Empty
            hdnDetailsIdMovePop.Value = "0"
        Else
            Me.hdnDetailsKeyRowPop.Value = oMoveDetailsParameter.KeyRow
            hdnDetailsIdMovePop.Value = oMoveDetailsParameter.IdMove.ToString()
        End If

        LoadDetailsComboType(oMoveDetailsParameter.IsNewMove, oMoveDetailsParameter.MoveType, oMoveDetailsParameter.ActualType)

        LoadDetailsComboDateSelection()

        LoadDetailsComboCauses()

        LoadDetailsComboCenters()

        LoadDetailsTaskFields()

        LoadDetailsComboTerminals(oMoveDetailsParameter.IsNewMove, oMoveDetailsParameter.IDTerminal)

        LoadDetailsComboZones()

        LoadDetailsComboDiningRoomTurn()

        If oMoveDetailsParameter.StateOfRow = "" Then
            LoadDetailsEmpty()
        Else
            Dim oPunch As roPunch = API.PunchServiceMethods.GetPunch(Me, oMoveDetailsParameter.IdMove, False)
            If Not oPunch.ActualType.HasValue Then oPunch.ActualType = PunchTypeEnum._NOTDEFINDED

            If Not oPunch.HasPhoto Then
                strPunchId = String.Empty
            Else
                strPunchId = oPunch.ID.ToString()
            End If

            Dim strNameTask As String = String.Empty

            Dim CanTelecommute = True
            Dim iIDRequest = roTypes.Any2Integer(oPunch.IDRequest)
            If (iIDRequest > 0) Then
                oMoveDetailsParameter.IDRequest = oPunch.IDRequest
                If (IsRequestApproved(iIDRequest) = False) Then
                    oMoveDetailsParameter.CanEdit = "false"
                End If
            End If

            If oMoveDetailsParameter.StateOfRow = DataRowState.Unchanged.ToString() Then

                If oPunch.ActualType = PunchTypeEnum._TASK Then
                    strNameTask = API.TasksServiceMethods.GetNameTask(Me, oPunch.TypeData)
                End If

                hdnDetailsTypePop.Value = oPunch.Type

                Dim intReliability As Integer = 1 - Convert.ToInt16(oPunch.IsNotReliable)
                Dim intTelecommuting As Integer = Convert.ToInt16(oPunch.InTelecommute)

                Dim invalidType As Integer = -1
                If oPunch.InvalidType.HasValue Then invalidType = CInt(oPunch.InvalidType.Value)

                LoadDetailsData(oPunch.DateTime, oPunch.ShiftDate, oPunch.IDTerminal, oPunch.Action, oPunch.TypeData, intReliability, oPunch.LocationZone,
                                oPunch.IDZone, oPunch.Location, oPunch.TimeZone, strNameTask, oPunch.ActualType, oPunch.Field1, oPunch.Field2, oPunch.Field3, oPunch.Field4, oPunch.Field5, oPunch.Field6, invalidType, oPunch.TypeDetails, oPunch.FullAddress, If(oPunch.MaskAlert.HasValue, If(oPunch.MaskAlert.Value, 1, 0), -1), If(oPunch.TemperatureAlert.HasValue, If(oPunch.TemperatureAlert.Value, 1, 0), -1), roTypes.Any2Integer(oPunch.VerificationType), intTelecommuting, CanTelecommute, oPunch.IDRequest)
            Else

                If oMoveDetailsParameter.ActualType = PunchTypeEnum._TASK Then
                    strNameTask = API.TasksServiceMethods.GetNameTask(Me, oMoveDetailsParameter.IDCause_IDTask_IDDiningRoom)
                End If

                hdnDetailsTypePop.Value = oMoveDetailsParameter.MoveType

                Dim intReliability As Integer = 1 - Convert.ToInt16(Boolean.Parse(oMoveDetailsParameter.Reliability))
                Dim intTelecommuting As Integer = 1 - Convert.ToInt16(Boolean.Parse(oMoveDetailsParameter.Telecommuting))

                LoadDetailsData(DateTime.ParseExact(oMoveDetailsParameter.ActualDate, "dd/MM/yyyy HH:mm:ss", System.Globalization.CultureInfo.CurrentCulture), Date.ParseExact(oMoveDetailsParameter.ShiftDate, "dd/MM/yyyy", System.Globalization.CultureInfo.CurrentCulture), oMoveDetailsParameter.IDTerminal, oMoveDetailsParameter.IDAction,
                                oMoveDetailsParameter.IDCause_IDTask_IDDiningRoom, intReliability, oMoveDetailsParameter.City, oMoveDetailsParameter.IDZone,
                                oMoveDetailsParameter.Position, oMoveDetailsParameter.TimeZone, strNameTask, oMoveDetailsParameter.ActualType,
                                oMoveDetailsParameter.Field1, oMoveDetailsParameter.Field2, oMoveDetailsParameter.Field3, oMoveDetailsParameter.Field4, oMoveDetailsParameter.Field5, oMoveDetailsParameter.Field6, oMoveDetailsParameter.InvalidType, oMoveDetailsParameter.TypeDetails, oMoveDetailsParameter.FullAddress, oMoveDetailsParameter.MaskAlert, oMoveDetailsParameter.TemperatureAlert, oMoveDetailsParameter.VerifyType, intTelecommuting, CanTelecommute, oMoveDetailsParameter.IDRequest)

            End If
            If Not oPunch.IsNotReliable Then
                lblDetailsNotReliable.Visible = False
            Else
                lblDetailsNotReliable.Visible = True
                lblDetailsNotReliable.Text = Me.Language.Translate(oPunch.NotReliableCause, Me.DefaultScope) & ". " & oPunch.Remarks
            End If

        End If

        LoadDetailsUser(oMoveDetailsParameter.IDPassport, oMoveDetailsParameter.IdMove, Boolean.Parse(oMoveDetailsParameter.CanEdit))

        LoadDetailsPhoto(strPunchId)

        CheckEditableControls(Boolean.Parse(oMoveDetailsParameter.CanEdit))

        txtDetailsTime.Focus()

        If Not HelperSession.GetFeatureIsInstalledFromApplication("Feature\Telecommuting") Then
            Me.divTelecommutingBlock.Visible = False
        Else
            Me.divTelecommutingBlock.Visible = True
        End If

    End Sub

    Private Sub LoadDetailsEmpty()

        cmbDetailsType.SelectedIndex = 0

        txtDetailsTime.Value = Nothing
        txtDetailsTime.ReadOnly = False

        cmbDetailsCause.SelectedIndex = 0
        cmbDetailsCenter.SelectedIndex = 0
        cmbTerminal.Value = "0"
        cmbZone.SelectedIndex = 0

        hdnDetailsIdTerminalPop.Value = "0"
        hdnDetailsIdActionPop.Value = "1"
        'hdnDetailsReliabilityPop.Value = 100
        'txtDetailsReliability.Text = 100
        hdnDetailsReliabilityPop.Value = "true"
        hdnDetailsTelecommutingPop.Value = "false"

        Dim bEmployeeHasTelecommuteAgreementOnDate As Boolean = False
        Dim sEmployeeTelecommuteMandatoryDays As String = String.Empty
        Dim sEmployeeTelecommuteOptionalDays As String = String.Empty
        Dim iEmployeeTelecommuteMaxDays As Integer = 0
        Dim iEmployeeTelecommuteMaxPercentage As Integer = 0
        Dim iEmployeeTelecommutePeriodType As Integer = 0

        Employee.roEmployee.GetEmployeeTelecommutingDataOnDate(Me.DatePage, IDEmployeePage, New Employee.roEmployeeState(-1), bEmployeeHasTelecommuteAgreementOnDate, sEmployeeTelecommuteMandatoryDays, sEmployeeTelecommuteOptionalDays, iEmployeeTelecommuteMaxDays, iEmployeeTelecommuteMaxPercentage, iEmployeeTelecommutePeriodType)

        hdnDetailsCanTelecommutePop.Value = bEmployeeHasTelecommuteAgreementOnDate
        cmbDiningRoomTurn.SelectedIndex = 0

        hdnDetailsMoveDatePop.Value = Me.DatePage.Value.ToString("yyyy/MM/dd")
        hdnDetailsTypePop.Value = cmbDetailsType.SelectedItem.Value
        hdnDetailsShiftDatePop.Value = hdnDetailsMoveDatePop.Value

        cmbMaskControl.SelectedItem = cmbMaskControl.Items.FindByValue(-1)
        cmbTemperatureControl.SelectedItem = cmbTemperatureControl.Items.FindByValue(-1)
        cmbVerifyType.SelectedItem = cmbVerifyType.Items.FindByValue(-1)

        txtFieldTask1.Text = String.Empty
        txtFieldTask2.Text = String.Empty
        txtFieldTask3.Text = String.Empty

    End Sub

    Private Sub LoadDetailsData(ByVal moveDate As DateTime, ByVal shiftDate As DateTime, ByVal idTerminal As Integer, ByVal idAction As Integer, ByVal idCause_idTask_idDiningRoom As Integer, ByVal Reliability As Integer,
                                ByVal city As String, ByVal idZone As Integer, ByVal position As String, ByVal timeZone As String, ByVal TaskName As String, ByVal ActualType As Integer,
                                ByVal field1 As String, ByVal field2 As String, ByVal field3 As String, ByVal field4 As Double, ByVal field5 As Double, ByVal field6 As Double,
                                ByVal invalidType As Integer, ByVal typeDetails As String, ByVal address As String, ByVal iMaskAlert As Integer, ByVal iTemperatureAlert As Integer, ByVal iVerifyType As Integer, ByVal Telecommuting As Integer, ByVal CanTelecommute As Boolean, ByVal iIDRequest As Integer)

        txtDetailsTime.Value = Format(moveDate, "HH:mm")
        hdnDetailsMoveTimePop.Value = Format(moveDate, "HH:mm:ss")
        hdnDetailsMoveDatePop.Value = moveDate.ToString("yyyy/MM/dd")

        txtDetailsTime.ReadOnly = txtDetailsTime.ReadOnly Or Convert.ToInt16(hdnDetailsTypePop.Value) > 3

        Dim tmpDate As Date = New DateTime(moveDate.Year, moveDate.Month, moveDate.Day)
        If tmpDate = shiftDate Then
            cmbDetailsDateSelection.Value = "0"
        ElseIf tmpDate < shiftDate Then
            cmbDetailsDateSelection.Value = "-1"
        ElseIf tmpDate > shiftDate Then
            cmbDetailsDateSelection.Value = "1"
        End If
        hdnDetailsShiftDatePop.Value = shiftDate.ToString("yyyy/MM/dd")

        hdnDetailsIdTerminalPop.Value = idTerminal.ToString()

        txtCity.Text = city
        hdnDetailsCityPop.Value = city

        txtFullAddress.Text = address
        hdnDetailsAddress.Value = address
        If timeZone <> String.Empty Then
            txtTimeZone.Text = TimeZoneInfo.FindSystemTimeZoneById(timeZone).DisplayName
            hdnDetailsTimeZone.Value = timeZone
        Else
            txtTimeZone.Text = ""
            hdnDetailsTimeZone.Value = ""
        End If

        txtFieldTask1.Text = field1
        txtFieldTask2.Text = field2
        txtFieldTask3.Text = field3
        txtFieldTask4.Value = field4
        txtFieldTask5.Value = field5
        txtFieldTask6.Value = field6

        hdnDetailsField1Pop.Value = field1
        hdnDetailsField2Pop.Value = field2
        hdnDetailsField3Pop.Value = field3
        hdnDetailsField4Pop.Value = field4
        hdnDetailsField5Pop.Value = field5
        hdnDetailsField6Pop.Value = field6

        If ActualType = 4 Then
            txtTask.Text = TaskName
            txtTask.ToolTip = TaskName
            hdnDetailsIDTaskPop.Value = idCause_idTask_idDiningRoom.ToString
        Else
            txtTask.Text = ""
            txtTask.ToolTip = ""
            hdnDetailsIDTaskPop.Value = "0"
        End If

        hdnDetailsInvalidTypePop.Value = invalidType
        hdnDetailsTypeDetailsPop.Value = typeDetails

        hdnDetailsPositionPop.Value = HttpUtility.HtmlDecode(position)

        Dim it As ListEditItem
        it = cmbTerminal.Items.FindByValue(idTerminal.ToString)
        If it Is Nothing Then
            cmbTerminal.Value = "0"
        Else
            cmbTerminal.SelectedItem = it
        End If

        cmbZone.SelectedIndex = 0
        If ActualType <> 10 Then 'Comedor
            it = cmbZone.Items.FindByValue(idZone.ToString)
            If it Is Nothing Then
                cmbZone.SelectedIndex = 0
            Else
                cmbZone.SelectedItem = it
            End If
        End If
        hdnDetailsIdActionPop.Value = idAction.ToString()

        cmbDetailsCause.SelectedIndex = 0
        cmbDiningRoomTurn.SelectedIndex = 0
        cmbDetailsCenter.SelectedIndex = 0
        If ActualType <> 4 And ActualType <> 10 And ActualType <> 13 Then 'tareas y comedor
            cmbDetailsCause.Value = idCause_idTask_idDiningRoom.ToString
        Else
            If ActualType = 10 Then
                cmbDiningRoomTurn.SelectedItem = cmbDiningRoomTurn.Items.FindByValue(CType(idCause_idTask_idDiningRoom, Short))
            End If
            If ActualType = 13 Then
                cmbDetailsCenter.SelectedItem = cmbDetailsCenter.Items.FindByValue(CType(idCause_idTask_idDiningRoom, Short))
                If cmbDetailsCenter.SelectedItem Is Nothing Then
                    ' Si no esta en la lista hay que añadirlo porque el centro actualmente esta inactivo
                    Dim oCenter As roBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, idCause_idTask_idDiningRoom, False)

                    If oCenter IsNot Nothing Then
                        Dim newItem As New DevExpress.Web.ListEditItem(oCenter.Name & ("*"), oCenter.ID)
                        Me.cmbDetailsCenter.Items.Add(newItem)
                        Me.cmbDetailsCenter.SelectedItem = newItem
                    Else
                        cmbDetailsCenter.SelectedItem = cmbDetailsCenter.Items.FindByValue(CType(0, Short))
                    End If
                End If
            End If
        End If

        hdnDetailsReliabilityPop.Value = IIf(Reliability = 1, True, False)
        hdnDetailsTelecommutingPop.Value = IIf(Telecommuting = 1, True, False)
        hdnDetailsCanTelecommutePop.Value = CanTelecommute
        'txtDetailsReliability.Text = hdnDetailsReliabilityPop.Value

        Try
            Dim oTerminal As roTerminal = API.TerminalServiceMethods.GetTerminal(Me, idTerminal, False)
            If oTerminal.Type <> String.Empty Then
                cmbTerminal.ReadOnly = True
            End If
            If idZone <> 0 Then
                cmbZone.ReadOnly = True
            End If
        Catch ex As Exception

        End Try

        divCenterCaption.Style("display") = "none"
        divCenterCmb.Style("display") = "none"
        divCauseCaption.Style("display") = "none"
        divCauseCmb.Style("display") = "none"

        If ActualType = 13 Then
            divCenterCaption.Style("display") = ""
            divCenterCmb.Style("display") = ""
        Else
            divCauseCaption.Style("display") = ""
            divCauseCmb.Style("display") = ""
        End If

        If ActualType = 10 And invalidType >= 0 Then
            trDinningRoomValidate.Style("display") = ""
            chkValidateDinningRoom.Checked = True

            Select Case invalidType
                Case InvalidTypeEnum.NTIME_
                    lblDinningRoomValidateDescription.Text = Me.Language.Translate("InvalidPunch.WithReason", Me.DefaultScope) & Me.Language.Translate("DinningRoom.NoTurn", Me.DefaultScope) & Me.Language.Translate("InvalidPunch.UncheckToValidate", Me.DefaultScope)
                Case InvalidTypeEnum.NDEF_
                    lblDinningRoomValidateDescription.Text = Me.Language.Translate("InvalidPunch.WithReason", Me.DefaultScope) & Me.Language.Translate("DinningRoom.Offline", Me.DefaultScope) & Me.Language.Translate("InvalidPunch.UncheckToValidate", Me.DefaultScope)
                Case InvalidTypeEnum.NRPT_
                    lblDinningRoomValidateDescription.Text = Me.Language.Translate("InvalidPunch.WithReason", Me.DefaultScope) & Me.Language.Translate("DinningRoom.Repeated", Me.DefaultScope) & Me.Language.Translate("InvalidPunch.UncheckToValidate", Me.DefaultScope)
                Case Else
                    lblDinningRoomValidateDescription.Text = Me.Language.Translate("InvalidPunch.WithOutReason", Me.DefaultScope)
            End Select

        End If

        hdnMaskAlert.Value = iMaskAlert
        hdnTemperatureAlert.Value = iTemperatureAlert
        hdnVerificationType.Value = iVerifyType
        hdnIDRequest.Value = iIDRequest
        Dim isRequestAndApproved = IsRequestApproved(iIDRequest)
        hdnIsRequestApproved.Value = isRequestAndApproved
        hdnCanEdit.Value = Not isRequestAndApproved 'Si es fichaje declarativo y no está aprobado, no se puede editar

        cmbMaskControl.SelectedItem = cmbMaskControl.Items.FindByValue(iMaskAlert)
        cmbTemperatureControl.SelectedItem = cmbTemperatureControl.Items.FindByValue(iTemperatureAlert)
        cmbVerifyType.SelectedItem = cmbVerifyType.Items.FindByValue(iVerifyType)

    End Sub

    Private Sub LoadDetailsComboType(ByVal IsNew As Boolean, Optional ByVal PunchType As PunchTypeEnum = PunchTypeEnum._IN, Optional ByVal ActualType As Integer = 0)

        cmbDetailsType.Items.Clear()
        cmbDetailsType.Enabled = True
        cmbDetailsType.ReadOnly = False

        Select Case Me.ViewPage

            Case ViewPageTypes.Moves, ViewPageTypes.Mobility
                Dim addItemCost As Boolean = False
                Dim addNewItemCost As Boolean = False

                If FeatureCostCenter And Me.CostCenterPermission >= Permission.Read Then 'licencia y permiso para fichajes de dcentros de coste
                    If Me.CostCenterPermission >= Permission.Read Then addItemCost = True
                    If Me.CostCenterPermission >= Permission.Write Then addNewItemCost = True
                End If

                If IsNew Then
                    If Me.CurrentPermission >= Permission.Write Then
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
                    End If

                    If addNewItemCost Then
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.CostCenter", Me.DefaultScope), Integer.Parse(PunchTypeEnum._CENTER)))
                    End If
                    cmbDetailsType.SelectedIndex = 0
                Else
                    If PunchType = PunchTypeEnum._AV Then '5 Acceso válido
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Access", Me.DefaultScope), Integer.Parse(PunchTypeEnum._AV)))
                        cmbDetailsType.Enabled = False
                        cmbDetailsType.SelectedIndex = 0

                    ElseIf PunchType = PunchTypeEnum._L Then '7 Acceso integrado con presencia
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
                        cmbDetailsType.Enabled = False
                        cmbDetailsType.SelectedIndex = IIf(ActualType = PunchStatus.In_, 0, 1)
                    Else '1, 2, 3
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
                        If addItemCost And ActualType = 13 Then
                            cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.CostCenter", Me.DefaultScope), Integer.Parse(PunchTypeEnum._CENTER)))
                            cmbDetailsType.SelectedIndex = 2
                            cmbDetailsType.ReadOnly = True
                        Else
                            cmbDetailsType.SelectedIndex = IIf(ActualType = PunchStatus.In_, 0, 1)
                        End If
                    End If
                End If

            Case ViewPageTypes.Tasks

                'Comprobar Empleado
                Dim addItemTask As Boolean = False
                If bolFeatureTasks And Me.TaskPunchesPermission >= Permission.Read Then 'licencia y permiso para fichajes de tareas
                    If Me.HasFeaturePermissionByEmployee("Tasks.Punches", Permission.Read, Me.IDEmployeePage) Then 'permiso de acceso al empleado
                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.IDEmployeePage, False)
                        If oEmployee.Type = "J" Then
                            addItemTask = True
                        End If
                    End If
                End If

                If IsNew Then
                    cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
                    cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
                    If addItemTask Then
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Task", Me.DefaultScope), Integer.Parse(PunchTypeEnum._TASK)))
                    End If
                    cmbDetailsType.SelectedIndex = 0
                Else
                    If PunchType = PunchTypeEnum._TASK Then '4 Tarea
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Task", Me.DefaultScope), Integer.Parse(PunchTypeEnum._TASK)))
                        cmbDetailsType.ReadOnly = False
                        cmbDetailsType.SelectedIndex = 0

                    ElseIf PunchType = PunchTypeEnum._L Then '7 Acceso integrado con presencia
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
                        cmbDetailsType.ReadOnly = True
                        cmbDetailsType.SelectedIndex = IIf(ActualType = PunchStatus.In_, 0, 1)
                    Else '1, 2, 3
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.In", Me.DefaultScope), Integer.Parse(PunchStatus.In_)))
                        cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.Out", Me.DefaultScope), Integer.Parse(PunchStatus.Out_)))
                        cmbDetailsType.SelectedIndex = IIf(ActualType = PunchStatus.In_, 0, 1)
                    End If
                End If

            Case ViewPageTypes.DinningRoom
                cmbDetailsType.Items.Add(New ListEditItem(Me.Language.Translate("TypeCaption.DiningRoom", Me.DefaultScope), Integer.Parse(PunchTypeEnum._DR)))
                cmbDetailsType.ReadOnly = True
                cmbDetailsType.SelectedIndex = 0

        End Select
    End Sub

    Private Sub LoadDetailsComboCenters()
        divCenterCaption.Style("display") = "none"
        divCenterCmb.Style("display") = "none"

        cmbDetailsCenter.Items.Clear()
        cmbDetailsCenter.ValueType = GetType(Short)

        cmbMaskControl.Items.Clear()
        cmbTemperatureControl.Items.Clear()
        cmbVerifyType.Items.Clear()
        cmbMaskControl.ValueType = GetType(Integer)
        cmbTemperatureControl.ValueType = GetType(Integer)
        cmbVerifyType.ValueType = GetType(Integer)

        cmbMaskControl.Items.Add(Me.Language.Translate("MaskControl.Unknown", Me.DefaultScope), -1)
        cmbTemperatureControl.Items.Add(Me.Language.Translate("TemperatureControl.Unknown", Me.DefaultScope), -1)

        cmbMaskControl.Items.Add(Me.Language.Translate("MaskControl.OK", Me.DefaultScope), 0)
        cmbTemperatureControl.Items.Add(Me.Language.Translate("TemperatureControl.OK", Me.DefaultScope), 0)

        cmbMaskControl.Items.Add(Me.Language.Translate("MaskControl.Alert", Me.DefaultScope), 1)
        cmbTemperatureControl.Items.Add(Me.Language.Translate("TemperatureControl.Alert", Me.DefaultScope), 1)

        For Each oVal As Integer In System.Enum.GetValues(GetType(VerificationType))
            Me.cmbVerifyType.Items.Add(Me.Language.Translate("VerificationType.Name." & System.Enum.GetName(GetType(VerificationType), oVal), Me.DefaultScope), oVal)
        Next

        If Me.ViewPage <> ViewPageTypes.Moves AndAlso Me.ViewPage <> ViewPageTypes.Mobility Then
            cmbDetailsCenter.Items.Add(New ListEditItem("", 0))
            cmbDetailsCenter.SelectedIndex = 0
        Else

            divCenterCaption.Style("display") = ""
            divCenterCmb.Style("display") = ""

            Dim tbCenters As DataTable = Me.GetAvailableCostCentersByEmployee(True)

            cmbDetailsCenter.DataSource = tbCenters
            cmbDetailsCenter.TextField = "Name"
            cmbDetailsCenter.ValueField = "IDCenter"
            Try
                cmbDetailsCenter.DataBind()
                cmbDetailsCenter.SelectedIndex = 0
            Catch ex As Exception
            End Try
        End If

    End Sub

    Private Sub LoadDetailsComboCauses()

        divCauseCaption.Style("display") = "none"
        divCauseCmb.Style("display") = "none"

        cmbDetailsCause.Items.Clear()

        If Me.ViewPage = ViewPageTypes.DinningRoom Then
            cmbDetailsCause.Items.Add(New ListEditItem("", "0"))
            cmbDetailsCause.SelectedIndex = 0
        Else

            divCauseCaption.Style("display") = ""
            divCauseCmb.Style("display") = ""

            Dim tbCauses As DataTable = CausesServiceMethods.GetCausesByEmployeeInputPermissions(Me, Me.IDEmployeePage, True)
            Dim oRow As DataRow = tbCauses.NewRow
            oRow("ID") = 0
            oRow("Name") = ""
            oRow("AllowInputFromReader") = True
            tbCauses.Rows.InsertAt(oRow, 0)

            cmbDetailsCause.DataSource = tbCauses
            cmbDetailsCause.TextField = "Name"
            cmbDetailsCause.ValueField = "ID"
            Try
                cmbDetailsCause.DataBind()
                cmbDetailsCause.SelectedIndex = 0
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub LoadDetailsTaskFields()
        If Me.ViewPage = ViewPageTypes.Tasks Then
            divTaskFields.Style("display") = ""
            divTaskFields2.Style("display") = ""
            Dim tbField As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me, Types.TaskField)
            If Not tbField Is Nothing AndAlso tbField.Rows.Count > 0 Then
                For n As Integer = 0 To tbField.Rows.Count - 1
                    Select Case n
                        Case 0 : lblFieldTask1.Text = roTypes.Any2String(tbField.Rows(n)("Name")) & ":"
                        Case 1 : lblFieldTask2.Text = roTypes.Any2String(tbField.Rows(n)("Name")) & ":"
                        Case 2 : lblFieldTask3.Text = roTypes.Any2String(tbField.Rows(n)("Name")) & ":"
                        Case 3 : lblFieldTask4.Text = roTypes.Any2String(tbField.Rows(n)("Name")) & ":"
                        Case 4 : lblFieldTask5.Text = roTypes.Any2String(tbField.Rows(n)("Name")) & ":"
                        Case 5 : lblFieldTask6.Text = roTypes.Any2String(tbField.Rows(n)("Name")) & ":"
                    End Select
                Next
            End If
        Else
            divTaskFields.Style("display") = "none"
            divTaskFields2.Style("display") = "none"
        End If
    End Sub
    Private Sub LoadDetailsComboTerminals(ByVal isNew As Boolean, ByVal idTerminal As Integer)
        cmbTerminal.Items.Clear()

        Dim terminalsList As DataTable
        If isNew Then
            terminalsList = API.TerminalServiceMethods.GetTerminalsDataSet(Me, "(Deleted = 0 OR Deleted IS NULL)")
        Else
            terminalsList = API.TerminalServiceMethods.GetTerminalsDataSet(Me, $"(Deleted = 0 OR Deleted IS NULL) OR ID = {idTerminal}")
        End If

        cmbTerminal.Items.Add(New ListEditItem("", "0"))
        For Each terminal As DataRow In terminalsList.Rows
            cmbTerminal.Items.Add(New ListEditItem(terminal("Description"), terminal("ID")))
        Next

        cmbTerminal.SelectedIndex = 0
    End Sub

    Private Sub LoadDetailsComboZones()

        cmbZone.Items.Clear()

        If Me.ViewPage = ViewPageTypes.DinningRoom Then
            cmbZone.Visible = False
            lblLocationZone.Visible = False
            cmbZone.Items.Add(New ListEditItem("", "0"))
            cmbZone.SelectedIndex = 0
        Else
            cmbZone.Visible = True
            lblLocationZone.Visible = True
            Dim tbZones As DataTable = API.ZoneServiceMethods.GetZones(Me)
            Dim voidRow As DataRow = tbZones.NewRow()
            voidRow("ID") = 0
            voidRow("Name") = ""
            voidRow("IsWorkingZone") = 0
            tbZones.Rows.InsertAt(voidRow, 0)

            cmbZone.DataSource = tbZones
            cmbZone.TextField = "Name"
            cmbZone.ValueField = "ID"
            Try
                cmbZone.DataBind()
                cmbZone.SelectedIndex = 0
            Catch ex As Exception
            End Try
        End If
    End Sub

    Private Sub LoadDetailsComboDiningRoomTurn()

        cmbDiningRoomTurn.Items.Clear()
        cmbDiningRoomTurn.ValueType = GetType(Short)
        If Me.ViewPage = ViewPageTypes.DinningRoom Then
            cmbDiningRoomTurn.Visible = True
            lblDiningRoomTurn.Visible = True
            Dim tbDiningRoomTurns As DataTable = API.DiningRoomServiceMethods.GetDiningRoomTurns(Me)
            Dim voidRow As DataRow = tbDiningRoomTurns.NewRow()
            voidRow("ID") = 0
            voidRow("Name") = ""
            tbDiningRoomTurns.Rows.InsertAt(voidRow, 0)

            cmbDiningRoomTurn.DataSource = tbDiningRoomTurns
            cmbDiningRoomTurn.TextField = "Name"
            cmbDiningRoomTurn.ValueField = "ID"
            Try
                cmbDiningRoomTurn.DataBind()
                cmbDiningRoomTurn.SelectedIndex = 0
            Catch ex As Exception
            End Try
        Else
            cmbDiningRoomTurn.Visible = False
            lblDiningRoomTurn.Visible = False
            cmbDiningRoomTurn.Items.Add(New ListEditItem("", 0))
            cmbDiningRoomTurn.SelectedIndex = 0
        End If
    End Sub

    Private Sub LoadDetailsComboDateSelection()
        cmbDetailsDateSelection.Items.Clear()
        cmbDetailsDateSelection.Items.Add(New ListEditItem(Me.Language.Translate("MoveDetails.DayBefore", Me.DefaultScope), "-1"))
        cmbDetailsDateSelection.Items.Add(New ListEditItem(Me.Language.Translate("MoveDetails.DayPresent", Me.DefaultScope), "0"))
        cmbDetailsDateSelection.Items.Add(New ListEditItem(Me.Language.Translate("MoveDetails.DayAfter", Me.DefaultScope), "1"))
        Me.cmbDetailsDateSelection.SelectedIndex = 1
    End Sub

    Private Sub LoadDetailsUser(ByRef IDPassport As Integer, ByVal IDPunch As Long, ByVal CanEdit As Boolean)
        Dim currentAction As String = ""
        Dim userName As String = ""

        If IDPunch > 0 Then
            If CanEdit Then currentAction = "2"
            If IDPassport > 0 Then
                Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me, IDPassport, LoadType.Passport)
                If Not oPassport Is Nothing Then
                    userName = oPassport.Name
                End If
            Else
                userName = WLHelperWeb.CurrentPassport.Name
                currentAction = "0"
            End If
        Else
            If CanEdit Then
                hdnDetailsIdActionPop.Value = "1"
                currentAction = ""
            Else
                hdnDetailsIdActionPop.Value = "0"
                currentAction = ""
            End If
            Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport
            IDPassport = oPassport.ID
            userName = oPassport.Name
        End If

        hdnDetailsIdPassportPop.Value = IDPassport
        If currentAction = "" Then currentAction = hdnDetailsIdActionPop.Value
        hdnDetailsIdCurrentActionPop.Value = currentAction

        GetDetailsAction(userName, Val(hdnDetailsIdActionPop.Value))

    End Sub

    Private Sub GetDetailsAction(ByVal userName As String, ByVal action As Integer)
        If action > 0 Then
            Dim sAction As String = ""
            Select Case action
                Case 1
                    sAction = Me.Language.Translate("ActionVerb.Add", Me.DefaultScope)
                Case 2
                    sAction = Me.Language.Translate("ActionVerb.Modify", Me.DefaultScope)
                Case 3
                    sAction = Me.Language.Translate("ActionVerb.ChangeWay", Me.DefaultScope)
                Case Else
                    sAction = Me.Language.Translate("ActionVerb.Unknown", Me.DefaultScope)
            End Select
            lblAction.Text = String.Format(Me.Language.Translate("lblAction", Me.DefaultScope), userName, sAction)
        Else
            lblAction.Text = Me.Language.Translate("lblNoAction", Me.DefaultScope)
        End If
    End Sub

    Private Sub LoadDetailsPhoto(ByVal strIdPunch As String)
        Me.imgEmployeeCapture.Attributes.Add("attr-urlPunch", Me.ResolveUrl("LoadImageCapture.aspx?IdPunch=" & strIdPunch & "&NewParam=" & Now.TimeOfDay.Seconds.ToString))
    End Sub

    Private Sub CheckEditableControls(ByVal bCanEdit As Boolean)

        Dim selValueDetailsType As String = cmbDetailsType.SelectedItem.Value

        cmbDetailsType.Enabled = bCanEdit

        txtDetailsTime.ReadOnly = Not bCanEdit

        cmbDetailsDateSelection.Enabled = (selValueDetailsType = "1" Or selValueDetailsType = "2" Or selValueDetailsType = "4" Or selValueDetailsType = "13") And bCanEdit

        cmbDetailsCause.Enabled = bCanEdit
        cmbDetailsCenter.Enabled = bCanEdit

        divCenterCaption.Style("display") = "none"
        divCenterCmb.Style("display") = "none"
        divCauseCaption.Style("display") = "none"
        divCauseCmb.Style("display") = "none"

        If selValueDetailsType = "4" Or selValueDetailsType = "10" Or selValueDetailsType = "13" Then 'tareas y comedor y centros de coste
            If selValueDetailsType = "13" Then
                divCenterCaption.Style("display") = ""
                divCenterCmb.Style("display") = ""
            End If
            cmbDetailsType.ReadOnly = True
        Else
            divCauseCaption.Style("display") = ""
            divCauseCmb.Style("display") = ""
        End If

        cmbTerminal.Enabled = (selValueDetailsType = "1" Or selValueDetailsType = "2" Or selValueDetailsType = "4" Or selValueDetailsType = "13" Or selValueDetailsType = "10") And bCanEdit

        cmbZone.Enabled = (selValueDetailsType = "1" Or selValueDetailsType = "2" Or selValueDetailsType = "4" Or selValueDetailsType = "13") And bCanEdit
        tdlblLocationZone.Visible = (selValueDetailsType <> "10")
        tdcmbZone.Visible = (selValueDetailsType <> "10")

        cmbDiningRoomTurn.Enabled = selValueDetailsType = "10" And bCanEdit
        tdlblDiningRoomTurn.Visible = (selValueDetailsType = "10")
        tdcmbDiningRoomTurn.Visible = (selValueDetailsType = "10")

        btnAcceptPop.Visible = bCanEdit

        If selValueDetailsType = 4 Then
            divTaskFields.Style("display") = ""
            divTaskFields2.Style("display") = ""
            txtTask.Style("display") = ""
            lblTask.Style("display") = ""
            ImgTask.Style("display") = ""
        Else
            divTaskFields.Style("display") = "none"
            divTaskFields2.Style("display") = "none"
            txtTask.Style("display") = "none"
            lblTask.Style("display") = "none"
            ImgTask.Style("display") = "none"
        End If
        txtFieldTask1.Enabled = bCanEdit
        txtFieldTask2.Enabled = bCanEdit
        txtFieldTask3.Enabled = bCanEdit
        txtFieldTask4.Enabled = bCanEdit
        txtFieldTask5.Enabled = bCanEdit
        txtFieldTask6.Enabled = bCanEdit

        ImgField1.Visible = bCanEdit
        ImgField2.Visible = bCanEdit
        ImgField3.Visible = bCanEdit
        ImgField4.Visible = bCanEdit
        ImgField5.Visible = bCanEdit
        ImgField6.Visible = bCanEdit

        Dim oFieldTask As New roTaskFieldDefinition
        Dim oData As New Generic.List(Of String)

        Dim strOutput As String = ""

        If bCanEdit Then
            For i As Integer = 1 To 6
                oFieldTask = API.UserFieldServiceMethods.GetTaskField(Me.Page, i, False)
                If Not oFieldTask Is Nothing Then
                    If oFieldTask.ValueType = ValueTypes.aValue Then
                        Select Case i
                            Case 1 : ImgField1.Visible = False
                            Case 2 : ImgField2.Visible = False
                            Case 3 : ImgField3.Visible = False
                            Case 4 : ImgField4.Visible = False
                            Case 5 : ImgField5.Visible = False
                            Case 6 : ImgField6.Visible = False
                        End Select
                    End If
                End If
            Next
        End If

        If selValueDetailsType = "10" Then
            imgCity.Visible = False
            trLocation.Style("display") = "none"
            trLocation2.Style("display") = "none"
        Else
            imgCity.Visible = (bCanEdit And bolFeatureAnyWhere)
            trLocation.Style("display") = ""
            trLocation2.Style("display") = ""
        End If

        'Si es fichaje declarativo y no está aprobado, no se puede editar
        Dim iIDRequest = roTypes.Any2Integer(hdnIDRequest.Value)
        If iIDRequest > 0 Then
            bCanEdit = bCanEdit AndAlso IsRequestApproved(iIDRequest)
        End If
        hdnCanEdit.Value = bCanEdit

    End Sub

    Private Function IsRequestApproved(ByVal IDRequest As Integer) As Boolean
        Dim bRet = False
        If IDRequest > 0 Then
            Dim request = API.RequestServiceMethods.GetRequestByID(Nothing, IDRequest, False)
            bRet = request.RequestType.Equals(eRequestType.DailyRecord) AndAlso request.RequestStatus.Equals(eRequestStatus.Accepted)
        End If
        Return bRet
    End Function

    '=== FIN   DETALLES DEL FICHAJE ======================================================

End Class

#Region "Class Helper"

Public Class Scheduler_ReadOnlyTemplate
    Implements ITemplate

    Public Sub InstantiateIn1(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
        Dim Auxcontainer As GridViewEditItemTemplateContainer = CType(container, GridViewEditItemTemplateContainer)
        Dim lbl As New ASPxLabel()
        lbl.ID = "lbl"
        Auxcontainer.Controls.Add(lbl)
        lbl.Text = IIf(Auxcontainer.Text = "&nbsp;", "", HttpContext.Current.Server.HtmlDecode(Auxcontainer.Text))
        lbl.Width = 10
        lbl.Font.Size = 8
        lbl.Wrap = DevExpress.Utils.DefaultBoolean.False
    End Sub

End Class

#End Region