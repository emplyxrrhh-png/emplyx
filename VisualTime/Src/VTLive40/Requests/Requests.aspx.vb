Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Requests
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("moment", "~/Base/Scripts/moment.min.js")
        Me.InsertExtraJavascript("momenttz", "~/Base/Scripts/moment-tz.min.js",, True)
        Me.InsertExtraJavascript("roDate", "~/Base/Scripts/Live/roDateManager.min.js",, True)
        Me.InsertExtraJavascript("Requests", "~/Requests/Scripts/Requests.js")
        Me.InsertExtraJavascript("roRequestListParams", "~/Requests/Scripts/roRequestListParams.js")
        Me.InsertExtraJavascript("SchedulerDates", "~/Scheduler/Scripts/SchedulerDates.js")
        Me.InsertExtraJavascript("SchedulerDatesV2", "~/Scheduler/Scripts/DateSelector.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        '' Comprueba la licencia
        'If HelperSession.GetFeatureIsInstalledFromApplication("Feature\LivePortal") = False Then
        '    WLHelperWeb.RedirectAccessDenied(False)
        '    Exit Sub
        'End If

        If Not Me.IsPostBack Then

            bCalendarMode.Value = "2"

            Dim bolHasPermission As Boolean = False
            ' Verificar permisos
            If WLHelperWeb.CurrentPassport IsNot Nothing Then
                Dim oRequestsTypeSecurity As Generic.List(Of roRequestTypeSecurity) = API.RequestServiceMethods.GetRequestTypeSecurityListAll(Me)
                For Each oRequestTypeSecurity As roRequestTypeSecurity In oRequestsTypeSecurity
                    If Me.HasFeaturePermission(oRequestTypeSecurity.SupervisorFeatureName, Permission.Read) Then
                        bolHasPermission = True
                        Exit For
                    End If
                Next
            End If

            If Not bolHasPermission Then
                WLHelperWeb.RedirectAccessDenied(False)
                Exit Sub
            End If

            'OBA: Obtiene por parametros la pestaña inicial a visualizar
            If Request.QueryString("tab") = "all" Then Me.actualListValue.Value = 1 Else Me.actualListValue.Value = 0

            Dim strDateAux As String = HelperWeb.GetShortDateFormat.ToLower
            dtFormatText.Value = strDateAux
            strDateAux = Replace(strDateAux, "ddd", "d")
            strDateAux = Replace(strDateAux, "dd", "d")
            strDateAux = Replace(strDateAux, "mm", "m")
            strDateAux = Replace(strDateAux, "yyyy", "y")
            strDateAux = Replace(strDateAux, "yy", "y")
            strDateAux = Replace(strDateAux, "/", "")
            strDateAux = Replace(strDateAux, "-", "")
            strDateAux = Replace(strDateAux, "d", "0")
            strDateAux = Replace(strDateAux, "m", "1")
            strDateAux = Replace(strDateAux, "y", "2")
            dtFormat.Value = strDateAux

        End If

    End Sub

End Class

<DataContract()>
Public Class roRequestListParams

    'PEND
    Private strPendList_OrderField As String = "RequestDate"

    Private strPendList_OrderDirection As String = "ASC"
    Private strPendList_FilterRequestType As String = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16"
    Private strPendList_FilterRequestDate As String = "" 'BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    Private strPendList_FilterRequestedDate As String = "" 'BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    Private strPendList_FilterEmployee As String = ""
    Private strPendList_FilterTree As String = ""
    Private strPendList_FilterTreeUser As String = ""
    Private strPendList_IdCause As String = "0"
    Private strPendList_IdSupervisor As String = "0"

    'OTHER
    Private strOtherList_OrderField As String = "RequestDate"

    Private strOtherList_OrderDirection As String = "ASC"
    Private strOtherList_FilterRequestType As String = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16"
    Private strOtherList_FilterRequestDate As String = "" 'BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    Private strOtherList_FilterRequestedDate As String = "" 'BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    Private strOtherList_FilterEmployee As String = ""
    Private strOtherList_FilterTree As String = ""
    Private strOtherList_FilterTreeUser As String = ""
    Private strOtherList_IdCause As String = "0"
    Private strOtherList_IdSupervisor As String = "0"
    Private strOtherList_FilterLevels As String = ""
    Private strOtherList_FilterDaysFrom As String = ""

    'HIST
    Private strHistList_OrderField As String = "RequestDate"

    Private strHistList_OrderDirection As String = "ASC"
    Private strHistList_FilterRequestType As String = "1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16"
    Private strHistList_FilterRequestDate As String = "" 'BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    Private strHistList_FilterRequestedDate As String = "" 'BeginDate*EndDate yyyy/MM/dd*yyyy/MM/dd
    Private strHistList_FilterEmployee As String = ""
    Private strHistList_FilterTree As String = ""
    Private strHistList_FilterTreeUser As String = ""
    Private strHistList_IdCause As String = "0"
    Private strHistList_IdSupervisor As String = "0"
    Private strHistoryList_FilterRequestState As String = "11000"

    Public Sub New()
    End Sub

    <DataMember(Name:="OrderFieldPend")>
    Public Property OrderFieldPend() As String
        Get
            Return strPendList_OrderField
        End Get
        Set(ByVal value As String)
            strPendList_OrderField = value
        End Set
    End Property

    <DataMember(Name:="OrderFieldOther")>
    Public Property OrderFieldOther() As String
        Get
            Return strOtherList_OrderField
        End Get
        Set(ByVal value As String)
            strOtherList_OrderField = value
        End Set
    End Property

    <DataMember(Name:="OrderFieldHist")>
    Public Property OrderFieldHist() As String
        Get
            Return strHistList_OrderField
        End Get
        Set(ByVal value As String)
            strHistList_OrderField = value
        End Set
    End Property

    <DataMember(Name:="OrderDirectionPend")>
    Public Property OrderDirectionPend() As String
        Get
            Return strPendList_OrderDirection
        End Get
        Set(ByVal value As String)
            strPendList_OrderDirection = value
        End Set
    End Property

    <DataMember(Name:="OrderDirectionOther")>
    Public Property OrderDirectionOther() As String
        Get
            Return strOtherList_OrderDirection
        End Get
        Set(ByVal value As String)
            strOtherList_OrderDirection = value
        End Set
    End Property

    <DataMember(Name:="OrderDirectionHist")>
    Public Property OrderDirectionHist() As String
        Get
            Return strHistList_OrderDirection
        End Get
        Set(ByVal value As String)
            strHistList_OrderDirection = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestTypePend")>
    Public Property FilterRequestTypePend() As String
        Get
            Return strPendList_FilterRequestType
        End Get
        Set(ByVal value As String)
            strPendList_FilterRequestType = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestTypeOther")>
    Public Property FilterRequestTypeOther() As String
        Get
            Return strOtherList_FilterRequestType
        End Get
        Set(ByVal value As String)
            strOtherList_FilterRequestType = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestTypeHist")>
    Public Property FilterRequestTypeHist() As String
        Get
            Return strHistList_FilterRequestType
        End Get
        Set(ByVal value As String)
            strHistList_FilterRequestType = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestDatePend")>
    Public Property FilterRequestDatePend() As String
        Get
            Return strPendList_FilterRequestDate
        End Get
        Set(ByVal value As String)
            strPendList_FilterRequestDate = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestDateOther")>
    Public Property FilterRequestDateOther() As String
        Get
            Return strOtherList_FilterRequestDate
        End Get
        Set(ByVal value As String)
            strOtherList_FilterRequestDate = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestDateHist")>
    Public Property FilterRequestDateHist() As String
        Get
            Return strHistList_FilterRequestDate
        End Get
        Set(ByVal value As String)
            strHistList_FilterRequestDate = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestedDatePend")>
    Public Property FilterRequestedDatePend() As String
        Get
            Return strPendList_FilterRequestedDate
        End Get
        Set(ByVal value As String)
            strPendList_FilterRequestedDate = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestedDateOther")>
    Public Property FilterRequestedDateOther() As String
        Get
            Return strOtherList_FilterRequestedDate
        End Get
        Set(ByVal value As String)
            strOtherList_FilterRequestedDate = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestedDateHist")>
    Public Property FilterRequestedDateHist() As String
        Get
            Return strHistList_FilterRequestedDate
        End Get
        Set(ByVal value As String)
            strHistList_FilterRequestedDate = value
        End Set
    End Property

    <DataMember(Name:="FilterEmployeePend")>
    Public Property FilterEmployeePend() As String
        Get
            Return strPendList_FilterEmployee
        End Get
        Set(ByVal value As String)
            strPendList_FilterEmployee = value
        End Set
    End Property

    <DataMember(Name:="FilterEmployeeOther")>
    Public Property FilterEmployeeOther() As String
        Get
            Return strOtherList_FilterEmployee
        End Get
        Set(ByVal value As String)
            strOtherList_FilterEmployee = value
        End Set
    End Property

    <DataMember(Name:="FilterEmployeeHist")>
    Public Property FilterEmployeeHist() As String
        Get
            Return strHistList_FilterEmployee
        End Get
        Set(ByVal value As String)
            strHistList_FilterEmployee = value
        End Set
    End Property

    <DataMember(Name:="FilterTreePend")>
    Public Property FilterTreePend() As String
        Get
            Return strPendList_FilterTree
        End Get
        Set(ByVal value As String)
            strPendList_FilterTree = value
        End Set
    End Property

    <DataMember(Name:="FilterTreeOther")>
    Public Property FilterTreeOther() As String
        Get
            Return strOtherList_FilterTree
        End Get
        Set(ByVal value As String)
            strOtherList_FilterTree = value
        End Set
    End Property

    <DataMember(Name:="FilterTreeHist")>
    Public Property FilterTreeHist() As String
        Get
            Return strHistList_FilterTree
        End Get
        Set(ByVal value As String)
            strHistList_FilterTree = value
        End Set
    End Property

    <DataMember(Name:="FilterTreeUserPend")>
    Public Property FilterTreeUserPend() As String
        Get
            Return strPendList_FilterTreeUser
        End Get
        Set(ByVal value As String)
            strPendList_FilterTreeUser = value
        End Set
    End Property

    <DataMember(Name:="FilterTreeUserOther")>
    Public Property FilterTreeUserOther() As String
        Get
            Return strOtherList_FilterTreeUser
        End Get
        Set(ByVal value As String)
            strOtherList_FilterTreeUser = value
        End Set
    End Property

    <DataMember(Name:="FilterTreeUserHist")>
    Public Property FilterTreeUserHist() As String
        Get
            Return strHistList_FilterTreeUser
        End Get
        Set(ByVal value As String)
            strHistList_FilterTreeUser = value
        End Set
    End Property

    <DataMember(Name:="FilterIdCausePend")>
    Public Property FilterIdCausePend() As String
        Get
            Return strPendList_IdCause
        End Get
        Set(ByVal value As String)
            strPendList_IdCause = value
        End Set
    End Property

    <DataMember(Name:="FilterIdCauseOther")>
    Public Property FilterIdCauseOther() As String
        Get
            Return strOtherList_IdCause
        End Get
        Set(ByVal value As String)
            strOtherList_IdCause = value
        End Set
    End Property

    <DataMember(Name:="FilterIdCauseHist")>
    Public Property FilterIdCauseHist() As String
        Get
            Return strHistList_IdCause
        End Get
        Set(ByVal value As String)
            strHistList_IdCause = value
        End Set
    End Property

    <DataMember(Name:="FilterIdSupervisorPend")>
    Public Property FilterIdSupervisorPend() As String
        Get
            Return strPendList_IdSupervisor
        End Get
        Set(ByVal value As String)
            strPendList_IdSupervisor = value
        End Set
    End Property

    <DataMember(Name:="FilterIdSupervisorOther")>
    Public Property FilterIdSupervisorOther() As String
        Get
            Return strOtherList_IdSupervisor
        End Get
        Set(ByVal value As String)
            strOtherList_IdSupervisor = value
        End Set
    End Property

    <DataMember(Name:="FilterIdSupervisorHist")>
    Public Property FilterIdSupervisorHist() As String
        Get
            Return strHistList_IdSupervisor
        End Get
        Set(ByVal value As String)
            strHistList_IdSupervisor = value
        End Set
    End Property

    <DataMember(Name:="FilterLevelsOther")>
    Public Property FilterLevelsOther() As String
        Get
            Return strOtherList_FilterLevels
        End Get
        Set(ByVal value As String)
            strOtherList_FilterLevels = value
        End Set
    End Property

    <DataMember(Name:="FilterDaysFromOther")>
    Public Property FilterDaysFromOther() As String
        Get
            Return strOtherList_FilterDaysFrom
        End Get
        Set(ByVal value As String)
            strOtherList_FilterDaysFrom = value
        End Set
    End Property

    <DataMember(Name:="FilterRequestStateHist")>
    Public Property FilterRequestStateHist() As String
        Get
            Return strHistoryList_FilterRequestState
        End Get
        Set(ByVal value As String)
            strHistoryList_FilterRequestState = value
        End Set
    End Property

    Public Shared Function roRequestListParams_Get(Optional ByVal strFilter As String = "") As roRequestListParams

        Dim oRet As New roRequestListParams

        Try
            Dim strJSON As String
            If strFilter = String.Empty Then
                strJSON = API.SecurityV3ServiceMethods.GetTreeState(Nothing, WLHelperWeb.CurrentPassportID, "TreeState_RequestListParams")
            Else
                strJSON = strFilter
                API.SecurityV3ServiceMethods.SaveTreeState(Nothing, WLHelperWeb.CurrentPassportID, "TreeState_RequestListParams", strJSON)
            End If

            If strJSON <> "" AndAlso strJSON <> "{}" Then
                oRet = roJSONHelper.Deserialize(strJSON, oRet.GetType)
            End If

            If oRet.FilterRequestStateHist.Length = 4 Then oRet.FilterRequestStateHist &= "0"
        Catch ex As Exception

        End Try

        Return oRet

    End Function

    Public Shared Function roRequestListParams_Set(ByVal oParams As roRequestListParams) As Boolean

        Dim bolRet As Boolean = False

        Try
            Dim strJSON As String = roJSONHelper.Serialize(oParams)

            If strJSON <> "" Then

                Robotics.Web.Base.HelperWeb.EraseCookie("RequestListParams")
                Robotics.Web.Base.HelperWeb.CreateCookie("RequestListParams", strJSON, False)

                bolRet = True

            End If
        Catch ex As Exception

        End Try

        Return bolRet

    End Function

End Class