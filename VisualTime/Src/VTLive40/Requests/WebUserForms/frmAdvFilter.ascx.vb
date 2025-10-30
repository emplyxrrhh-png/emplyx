Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class frmAdvFilter
    Inherits UserControlBase

    Private _mode As Integer = 0
    Private _prefix As String = String.Empty

    Public Property Mode As Integer
        Get
            Return _mode
        End Get
        Set(value As Integer)
            _mode = value
        End Set
    End Property

    Public Property Prefix As String
        Get
            Return _prefix
        End Get
        Set(value As String)
            _prefix = value
        End Set
    End Property

    Public Overloads ReadOnly Property Language As roLanguageWeb
        Get
            Return CType(Me.Page, PageBase).Language
        End Get
    End Property

    Public Overloads ReadOnly Property DefaultScope As String
        Get
            Return CType(Me.Page, PageBase).DefaultScope
        End Get
    End Property

    Private Sub frmAdvFilter_Load(sender As Object, e As EventArgs) Handles Me.Load

        Select Case Mode
            Case 0
                'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS TreeV3 DEL FILTRO AVANZADO DE PENDIENTES
                HelperWeb.roSelector_Initialize("objContainerTreeV3_RequestsFilterTreeEmpPend")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_RequestsFilterTreeEmpPendGrid")
            Case 1
                'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS TreeV3 DEL FILTRO AVANZADO DE HISTORICO
                HelperWeb.roSelector_Initialize("objContainerTreeV3_RequestsFilterTreeEmpOther")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_RequestsFilterTreeEmpOtherGrid")
            Case 2
                'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS TreeV3 DEL FILTRO AVANZADO DE PENDIENTES
                HelperWeb.roSelector_Initialize("objContainerTreeV3_RequestsFilterTreeEmpHist")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_RequestsFilterTreeEmpHistGrid")
        End Select

        '=== Obtener filtro de solcititudes guardado en BDD
        Dim oParams As New roRequestListParams
        Try
            Dim strFilter As String = API.RequestServiceMethods.GetFilterRequests(Nothing, WLHelperWeb.CurrentPassport.ID)
            If strFilter <> String.Empty AndAlso strFilter <> "{}" Then

                Me.chkSaveFilter.Checked = True
                Robotics.Web.Base.HelperWeb.EraseCookie("RequestListParams")

                oParams = roRequestListParams.roRequestListParams_Get(strFilter)
            Else
                oParams = roRequestListParams.roRequestListParams_Get()
            End If
        Catch
            HelperWeb.EraseCookie("RequestListParams")
            oParams = roRequestListParams.roRequestListParams_Get()
        End Try

        Dim dTbl As DataTable = CausesServiceMethods.GetCausesShortList(Nothing)
        Me.cmbCause.ClientSideEvents.SelectedIndexChanged = "function(s,e){ setFilterIdCause(" & Me.Mode & ", s.GetSelectedItem().value); }"

        cmbCause.Items.Add("", "0") ', "setFilterIdCause(" & Me.Mode & ", '0');")
        For Each dRow As DataRow In dTbl.Rows
            cmbCause.Items.Add(dRow("Name"), dRow("ID")) ', "setFilterIdCause(" & Me.Mode & ", '" & dRow("ID") & "');")
        Next

        Me.cmbSupervisor.ClientSideEvents.SelectedIndexChanged = "function(s,e){ setFilterIdSupervisor(" & Me.Mode & ", s.GetSelectedItem().value); }"
        Dim oPassports As roPassport() = API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Me.Page, True)

        If oPassports IsNot Nothing AndAlso oPassports.Count > 0 Then
            For Each oPassport As roPassport In oPassports
                cmbSupervisor.Items.Add(oPassport.Name, oPassport.ID)
            Next

            cmbSupervisor.Items.Add(Me.Language.Translate("Supervisor.All", Me.DefaultScope), 0)
            cmbSupervisor.SelectedIndex = 0
        End If

        '=== Numero de Solicitudes =======================================
        Me.cmbNumRequests.ClientSideEvents.SelectedIndexChanged = "function(s,e){ loadRequests(" & Me.Mode & ", false, false); }"
        Me.cmbNumRequests.ClientInstanceName = Me.cmbNumRequests.ID & Me.Prefix

        Me.cmbNumRequests.Items.Add(25, 25)
        Me.cmbNumRequests.Items.Add(50, 50)
        Me.cmbNumRequests.Items.Add(100, 100)
        Me.cmbNumRequests.Items.Add(200, 200)
        'For i As Byte = 10 To 100 Step 10
        '    Me.cmbNumRequests.Items.Add(i.ToString, i.ToString)
        'Next
        Me.cmbNumRequests.Value = "25"
        '===================================================================

        Me.txtRequestDateBegin.ClientInstanceName = "txtRequestDateBegin" & Me.Prefix & "_Client"
        Me.txtRequestDateEnd.ClientInstanceName = "txtRequestDateEnd" & Me.Prefix & "_Client"

        Me.txtRequestedDateBegin.ClientInstanceName = "txtRequestedDateBegin" & Me.Prefix & "_Client"
        Me.txtRequestedDateEnd.ClientInstanceName = "txtRequestedDateEnd" & Me.Prefix & "_Client"

        '========= Orden de carga de solicitudes ========================================
        Dim arrTranslate() As String = {"OrderField.RequestDate", "OrderField.LastRequestApprovalDate", "OrderField.RequestType", "OrderField.EmployeeName", "OrderField.GroupName", "OrderField.StatusLevel"}
        Me.cmbOrder.ClientInstanceName = Me.cmbOrder.ID & Me.Prefix
        Me.cmbOrder.ClientSideEvents.SelectedIndexChanged = "function(s,e){ setOrderField(" & Me.Mode & ", s.GetSelectedItem().value); loadRequests(" & Me.Mode & ", false, false); }"
        For Each Item As String In arrTranslate
            Me.cmbOrder.Items.Add(Me.Language.Translate(Item, Me.DefaultScope), Item.Substring(11))
        Next
        '===================================================================

        Dim strTypeFilter As String = String.Empty
        Dim strDate As String = String.Empty
        Dim strDate2 As String = String.Empty
        Dim strOrderDirection As String = String.Empty
        Select Case Mode
            Case 0
                Me.cmbOrder.Value = oParams.OrderFieldPend
                strOrderDirection = oParams.OrderDirectionPend
                strDate = oParams.FilterRequestDatePend
                strDate2 = oParams.FilterRequestedDatePend
                strTypeFilter = oParams.FilterRequestTypePend
                Me.cmbCause.Value = oParams.FilterIdCausePend
                Me.cmbSupervisor.Value = oParams.FilterIdSupervisorPend
            Case 1
                Me.cmbOrder.Value = oParams.OrderFieldOther
                strOrderDirection = oParams.OrderDirectionOther
                strDate = oParams.FilterRequestDateOther
                strDate2 = oParams.FilterRequestedDateOther
                strTypeFilter = oParams.FilterRequestTypeOther
                Me.cmbCause.Value = oParams.FilterIdCauseOther
                Me.cmbSupervisor.Value = oParams.FilterIdSupervisorOther
            Case 2
                Me.cmbOrder.Value = oParams.OrderFieldHist
                strOrderDirection = oParams.OrderDirectionHist
                strDate = oParams.FilterRequestDateHist
                strDate2 = oParams.FilterRequestedDateHist
                strTypeFilter = oParams.FilterRequestTypeHist
                Me.cmbCause.Value = oParams.FilterIdCauseHist
                Me.cmbSupervisor.Value = oParams.FilterIdSupervisorHist
        End Select

        '========= tipo de Orden ascendente - descendente =======================================
        Me.icoAscending.Attributes("class") = Me.icoAscending.Attributes("class").Split(" ")(0) & " " & IIf(strOrderDirection = "ASC", "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoDescending.Attributes("class") = Me.icoDescending.Attributes("class").Split(" ")(0) & " " & IIf(strOrderDirection = "DESC", "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoAscending.Title = Me.Language.Translate("OrderDirection.Ascending", Me.DefaultScope)
        Me.icoDescending.Title = Me.Language.Translate("OrderDirection.Descending", Me.DefaultScope)
        Me.icoAscending.Attributes("onclick") = "SortDirection(" & Me.Mode & ", 'ASC'); loadRequests(" & Me.Mode & ", false, false);"
        Me.icoDescending.Attributes("onclick") = "SortDirection(" & Me.Mode & ", 'DESC'); loadRequests(" & Me.Mode & ", false, false);"
        '===================================================================

        Me.advFilterButtonShow.Title = Me.Language.Translate("AdvancedFilter", Me.DefaultScope)
        Me.advFilterButtonShow.Attributes("onclick") = "filterVisible(" & Me.Mode & ");"

        '========= Niveles y Dias de Others =======================================
        If Me.Mode = 1 Then
            Me.mode2Inf.Style("display") = ""

            Me.divSecurityV3Hidden.Style("display") = "none"
            Me.lblSecurityV3Hidden.Style("display") = "none"

            Me.OtherRequests_txtDaysFrom.Attributes.Add("onchange", "setDaysFrom(" & Me.Mode & ", '" & Me.OtherRequests_txtDaysFrom.ClientID & "'); loadRequests(" & Me.Mode & ", false, false);")
            Me.OtherRequests_txtDaysFrom.Value = oParams.FilterDaysFromOther
        End If

        '===================================================================

        '==== Lista histórico solicitudes ==============================
        If Me.Mode = 2 Then
            Me.mode3Inf.Style("display") = ""
            Dim strStateFilter As String = If(oParams.FilterRequestStateHist <> String.Empty, oParams.FilterRequestStateHist, "11000")
            Me.icoStatePending.Attributes("class") = Me.icoStatePending.Attributes("class").Split(" ")(0) & " " & IIf(strStateFilter.Substring(0, 1) = "1", "RequestListIcoPressed", "RequestListIcoUnPressed")
            Me.icoStateOnGoing.Attributes("class") = Me.icoStateOnGoing.Attributes("class").Split(" ")(0) & " " & IIf(strStateFilter.Substring(1, 1) = "1", "RequestListIcoPressed", "RequestListIcoUnPressed")
            Me.icoStateAccepted.Attributes("class") = Me.icoStateAccepted.Attributes("class").Split(" ")(0) & " " & IIf(strStateFilter.Substring(2, 1) = "1", "RequestListIcoPressed", "RequestListIcoUnPressed")
            Me.icoStateDenied.Attributes("class") = Me.icoStateDenied.Attributes("class").Split(" ")(0) & " " & IIf(strStateFilter.Substring(3, 1) = "1", "RequestListIcoPressed", "RequestListIcoUnPressed")
            Me.icoStateCanceled.Attributes("class") = Me.icoStateCanceled.Attributes("class").Split(" ")(0) & " " & IIf(strStateFilter.Substring(4, 1) = "1", "RequestListIcoPressed", "RequestListIcoUnPressed")
            Me.icoStatePending.Title = Me.Language.Translate("FilterRequestState.Pending", Me.DefaultScope)
            Me.icoStateOnGoing.Title = Me.Language.Translate("FilterRequestState.OnGoing", Me.DefaultScope)
            Me.icoStateAccepted.Title = Me.Language.Translate("FilterRequestState.Accepted", Me.DefaultScope)
            Me.icoStateDenied.Title = Me.Language.Translate("FilterRequestState.Denied", Me.DefaultScope)
            Me.icoStateCanceled.Title = Me.Language.Translate("FilterRequestState.Canceled", Me.DefaultScope)

            Me.icoStatePending.Attributes("onclick") = "StateFilter(" & Me.Mode & ", this); loadRequests(" & Me.Mode & ", false, false);"
            Me.icoStateOnGoing.Attributes("onclick") = "StateFilter(" & Me.Mode & ", this); loadRequests(" & Me.Mode & ", false, false);"
            Me.icoStateAccepted.Attributes("onclick") = "StateFilter(" & Me.Mode & ", this); loadRequests(" & Me.Mode & ", false, false);"
            Me.icoStateDenied.Attributes("onclick") = "StateFilter(" & Me.Mode & ", this); loadRequests(" & Me.Mode & ", false, false);"
            Me.icoStateCanceled.Attributes("onclick") = "StateFilter(" & Me.Mode & ", this); loadRequests(" & Me.Mode & ", false, false);"
        End If
        '===================================================================

        '==== Establecemos filtro avanzado Solicitudes Pendientes =========
        If strDate IsNot Nothing Then
            If strDate.Split("*").Length = 2 Then
                Dim xDate As Date
                Dim firstDate As String = String.Empty
                firstDate = strDate.Split("*")(0)
                If firstDate <> "" Then
                    Try
                        xDate = New Date(firstDate.Split("/")(0), firstDate.Split("/")(1), firstDate.Split("/")(2))
                        Me.txtRequestDateBegin.Date = xDate '.Value = Format(xDate, HelperWeb.GetShortDateFormat)
                    Catch
                    End Try
                End If
                strDate = strDate.Split("*")(1)
                If strDate <> "" Then
                    Try
                        xDate = New Date(strDate.Split("/")(0), strDate.Split("/")(1), strDate.Split("/")(2))
                        Me.txtRequestDateEnd.Date = xDate '.Value = Format(xDate, HelperWeb.GetShortDateFormat)
                    Catch
                    End Try
                End If
            End If
        End If
        '==== Establecemos filtro avanzado Solicitudes Fecha Efecto =========
        If strDate2 IsNot Nothing Then
            If strDate2.Split("*").Length = 2 Then
                Dim xDate As Date
                Dim firstDate As String = String.Empty
                firstDate = strDate2.Split("*")(0)
                If firstDate <> "" Then
                    Try
                        xDate = New Date(firstDate.Split("/")(0), firstDate.Split("/")(1), firstDate.Split("/")(2))
                        Me.txtRequestedDateBegin.Date = xDate '.Value = Format(xDate, HelperWeb.GetShortDateFormat)
                    Catch
                    End Try
                End If
                strDate2 = strDate2.Split("*")(1)
                If strDate2 <> "" Then
                    Try
                        xDate = New Date(strDate2.Split("/")(0), strDate2.Split("/")(1), strDate2.Split("/")(2))
                        Me.txtRequestedDateEnd.Date = xDate '.Value = Format(xDate, HelperWeb.GetShortDateFormat)
                    Catch
                    End Try
                End If
            End If
        End If
        strTypeFilter = "," & strTypeFilter & ","
        Me.icoTypeUserFieldsChange.Attributes("class") = Me.icoTypeUserFieldsChange.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",1,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeForbiddenPunch.Attributes("class") = Me.icoTypeForbiddenPunch.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",2,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeJustifyPunch.Attributes("class") = Me.icoTypeJustifyPunch.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",3,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeExternalWorkResumePart.Attributes("class") = Me.icoTypeExternalWorkResumePart.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",4,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeExternalWorkWeekResume.Attributes("class") = Me.icoTypeExternalWorkWeekResume.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",4,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeChangeShift.Attributes("class") = Me.icoTypeChangeShift.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",5,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeVacationsOrPermissions.Attributes("class") = Me.icoTypeVacationsOrPermissions.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",6,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeCancelHolidays.Attributes("class") = Me.icoTypeCancelHolidays.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",6,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypePlannedAbsences.Attributes("class") = Me.icoTypePlannedAbsences.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",7,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypePlannedCauses.Attributes("class") = Me.icoTypePlannedCauses.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",9,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeExchangeShiftBetweenEmployees.Attributes("class") = Me.icoTypeExchangeShiftBetweenEmployees.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",8,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeForbiddenTaskPunch.Attributes("class") = Me.icoTypeForbiddenTaskPunch.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",10,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeForgottenCostCenterPunch.Attributes("class") = Me.icoTypeForgottenCostCenterPunch.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",12,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypePlannedHolidays.Attributes("class") = Me.icoTypePlannedHolidays.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",13,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypePlannedOvertimes.Attributes("class") = Me.icoTypePlannedOvertimes.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",14,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeTelecommute.Attributes("class") = Me.icoTypeTelecommute.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",16,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeDailyRecord.Attributes("class") = Me.icoTypeDailyRecord.Attributes("class").Split(" ")(0) & " " & IIf(strTypeFilter.Contains(",17,"), "RequestListIcoPressed", "RequestListIcoUnPressed")
        Me.icoTypeUserFieldsChange.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeForbiddenPunch.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeJustifyPunch.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeExternalWorkResumePart.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeExternalWorkWeekResume.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeChangeShift.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeVacationsOrPermissions.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeCancelHolidays.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypePlannedAbsences.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypePlannedCauses.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeExchangeShiftBetweenEmployees.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeForbiddenTaskPunch.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeForgottenCostCenterPunch.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypePlannedHolidays.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypePlannedOvertimes.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeTelecommute.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"
        Me.icoTypeDailyRecord.Attributes("onclick") = "TypeFilter(" & Me.Mode & ",this);"

        Me.icoTypeUserFieldsChange.Title = Me.Language.Translate("RequestType.UserFieldsChange", Me.DefaultScope)
        Me.icoTypeForbiddenPunch.Title = Me.Language.Translate("RequestType.ForbiddenPunch", Me.DefaultScope)
        Me.icoTypeJustifyPunch.Title = Me.Language.Translate("RequestType.JustifyPunch", Me.DefaultScope)
        Me.icoTypeExternalWorkResumePart.Title = Me.Language.Translate("RequestType.ExternalWorkResumePart", Me.DefaultScope)
        Me.icoTypeExternalWorkWeekResume.Title = Me.Language.Translate("RequestType.ExternalWorkWeekResume", Me.DefaultScope)
        Me.icoTypeChangeShift.Title = Me.Language.Translate("RequestType.ChangeShift", Me.DefaultScope)
        Me.icoTypeVacationsOrPermissions.Title = Me.Language.Translate("RequestType.VacationsOrPermissions", Me.DefaultScope)
        Me.icoTypeCancelHolidays.Title = Me.Language.Translate("RequestType.CancelHolidays", Me.DefaultScope)
        Me.icoTypePlannedAbsences.Title = Me.Language.Translate("RequestType.PlannedAbsences", Me.DefaultScope)
        Me.icoTypePlannedCauses.Title = Me.Language.Translate("RequestType.PlannedCauses", Me.DefaultScope)
        Me.icoTypeExchangeShiftBetweenEmployees.Title = Me.Language.Translate("RequestType.ExchangeShiftBetweenEmployees", Me.DefaultScope)
        Me.icoTypeForbiddenTaskPunch.Title = Me.Language.Translate("RequestType.ForbiddenTaskPunch", Me.DefaultScope)
        Me.icoTypeForgottenCostCenterPunch.Title = Me.Language.Translate("RequestType.ForgottenCostCenterPunch", Me.DefaultScope)
        Me.icoTypePlannedHolidays.Title = Me.Language.Translate("RequestType.PlannedHolidays", Me.DefaultScope)
        Me.icoTypePlannedOvertimes.Title = Me.Language.Translate("RequestType.PlannedOvertimes", Me.DefaultScope)
        Me.icoTypeTelecommute.Title = Me.Language.Translate("RequestType.Telecommute", Me.DefaultScope)
        Me.icoTypeDailyRecord.Title = Me.Language.Translate("RequestType.DailyRecord", Me.DefaultScope)

        Me.aFEmployees.Attributes("onmouseover") = "DDown_Over(" & Me.Mode & ");"
        Me.aFEmployees.Attributes("onmouseout") = "DDown_Out(" & Me.Mode & ");"

        Me.divFloatMenuE.Attributes("onmouseover") = "document.getElementById('" & Me.divFloatMenuE.ClientID & "').style.display='';"
        Me.divFloatMenuE.Attributes("onmouseout") = "document.getElementById('" & Me.divFloatMenuE.ClientID & "').style.display='none';"

        Me.aEmpAll.Attributes("onclick") = "ShowSelector(" & Me.Mode & ",1);DDown_Out(" & Me.Mode & ");"
        Me.aEmpSelect.Attributes("onclick") = "ShowSelector(" & Me.Mode & ",2);DDown_Out(" & Me.Mode & ");"

        Me.btnApply.Attributes("onclick") = "if (AdvancedFilter(" & Me.Mode & ") == true) { loadRequestsFromFilter(" & Me.Mode & ", false); filterVisible(" & Me.Mode & "); }"
        Me.btnApply.Title = Me.Language.Keyword("Button.Apply")
        Me.btnApply.InnerHtml = Me.Language.Keyword("Button.Apply")

        Me.btnCancel.Attributes("onclick") = "filterVisible(" & Me.Mode & ");"
        Me.btnCancel.Title = Me.Language.Translate("Button.Cancel", Me.DefaultScope)
        Me.btnCancel.InnerHtml = Me.Language.Translate("Button.Cancel", Me.DefaultScope)

        Me.btnRefresh.Attributes("onclick") = "SelectedRowID=''; loadRequests(" & Me.Mode & ", true, false);"
        Me.btnRefresh.Title = Me.Language.Translate("Button.Refresh", Me.DefaultScope)

        If oParams.FilterEmployeePend <> String.Empty Then
            Dim tmpCookie As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_RequestsFilterTreeEmpPend")
            tmpCookie.ActiveTree = "1"
            tmpCookie.UserField = oParams.FilterTreePend
            tmpCookie.UserFieldFilter = oParams.FilterTreeUserPend
            HelperWeb.roSelector_SetTreeState(tmpCookie)

            tmpCookie = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_RequestsFilterTreeEmpPendGrid")
            tmpCookie.ActiveTree = "1"
            tmpCookie.Selected1 = oParams.FilterEmployeePend
            HelperWeb.roSelector_SetTreeState(tmpCookie)
        End If

        If oParams.FilterEmployeeOther <> String.Empty Then
            Dim tmpCookie As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_RequestsFilterTreeEmpOther")
            tmpCookie.ActiveTree = "1"
            tmpCookie.UserField = oParams.FilterTreeOther
            tmpCookie.UserFieldFilter = oParams.FilterTreeUserOther
            HelperWeb.roSelector_SetTreeState(tmpCookie)

            tmpCookie = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_RequestsFilterTreeEmpOtherGrid")
            tmpCookie.ActiveTree = "1"
            tmpCookie.Selected1 = oParams.FilterEmployeeOther
            HelperWeb.roSelector_SetTreeState(tmpCookie)
        End If

        If oParams.FilterEmployeeHist <> String.Empty Then
            Dim tmpCookie As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_RequestsFilterTreeEmpHist")
            tmpCookie.ActiveTree = "1"
            tmpCookie.UserField = oParams.FilterTreeHist
            tmpCookie.UserFieldFilter = oParams.FilterTreeUserHist
            HelperWeb.roSelector_SetTreeState(tmpCookie)

            tmpCookie = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_RequestsFilterTreeEmpHistGrid")
            tmpCookie.ActiveTree = "1"
            tmpCookie.Selected1 = oParams.FilterEmployeeHist
            HelperWeb.roSelector_SetTreeState(tmpCookie)
        End If

    End Sub

End Class