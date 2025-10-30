Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Web.Base
Imports Robotics.Web.Base.HelperWeb

Partial Class MoveEmployee
    Inherits PageBase

    Private intIDEmployee As Integer
    Private intIDGroupSelected As Integer

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("OpenWindow", "~/Base/Scripts/OpenWindow.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.GroupMobility", Permission.Write) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        If ScriptManager.GetCurrent(Me).IsInAsyncPostBack Then Exit Sub

        HelperWeb.roSelector_Initialize("roChildSelectorW_treeGroupMoveEmployee")
        Me.GroupSelectorFrame.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=100&TreesMultiSelect=000&TreesOnlyGroups=100&TreeFunction=parent.GroupSelected&FilterFloat=false&AdvancedFilterVisible=false&" &
                                                                "PrefixTree=treeGroupMoveEmployee&FeatureAlias=Employees&FeatureType=A")
        GroupSelectorFrame.Disabled = False

        Dim bolContractOK As Boolean = True
        Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Me, Me.intIDEmployee, False)
        If oContract Is Nothing Then
            bolContractOK = False
        Else
            bolContractOK = (oContract.BeginDate <= Now.Date And oContract.EndDate >= Now.Date)
        End If
        If Not bolContractOK Then HelperWeb.ShowMessage(Me, Me.Language.Translate("NoActiveContrat.Message", Me.DefaultScope), "NoActiveContractKey", False, True, HelperWeb.MsgBoxIcons.InformationIcon)

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

        If Not Me.IsPostBack Then
            Me.optNow.Checked = True
            Me.optNow.Enabled = False
            Me.optFuture.Enabled = False

            chkCopyPlan.Enabled = False
            Me.hdnIDGroupSelected.Value = ""
        End If

    End Sub

    Protected Sub ASPxCallbackMoveContenido_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackMoveContenido.Callback

        ' Verifico que el empleado tenga contrato en vigor
        Dim bolContractOK As Boolean = True
        Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Me, Me.intIDEmployee, False)
        If oContract Is Nothing Then
            bolContractOK = False
        Else
            bolContractOK = (oContract.BeginDate <= Now.Date And oContract.EndDate >= Now.Date)
        End If
        If Not bolContractOK Then
            HelperWeb.ShowMessage(Me, Me.Language.Translate("NoActiveContrat.Message", Me.DefaultScope), "NoActiveContractKey", False, True, HelperWeb.MsgBoxIcons.InformationIcon)
        End If

        If e.Parameter.StartsWith("NOSAVE") Then
            ASPxCallbackMoveContenido.JSProperties.Add("cpActionRO", "NOSAVE")
            ASPxCallbackMoveContenido.JSProperties.Add("cpResultRO", "NOOK")
            Me.CanClose = False
            Me.MustRefresh = "-1"
            bolContractOK = False
        End If

        If bolContractOK Then

            hdnGroupSelected.Value = "1"
            lnkGroupSelected.Text = Me.Language.Translate("GroupSelected.DefaultText", Me.DefaultScope)

            If Me.hdnIDGroupSelected.Value.Length > 1 Then

                Dim intIDGroup As Integer = CInt(Me.hdnIDGroupSelected.Value.Substring(1))

                Me.hdnGroupSelected.Value = intIDGroup
                Me.lnkGroupSelected.Text = API.EmployeeGroupsServiceMethods.GetGroup(Me, intIDGroup, False).Name

                Dim ds As DataSet = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroup(Me, intIDGroup, API.EmployeeGroupsServiceMethods.eEmployeesFromGroup.Current, "Employees")
                If ds IsNot Nothing Then
                    If Not e.Parameter.StartsWith("SAVE") Then
                        Me.ddlSourceEmployee.SelectedItem = Nothing
                    End If

                    Me.ddlSourceEmployee.DataSource = ds.Tables(0)
                    Me.ddlSourceEmployee.TextField = "EmployeeName"
                    Me.ddlSourceEmployee.ValueField = "IDEmployee"
                    Me.ddlSourceEmployee.DataBind()

                    Me.optFuture.Enabled = True
                    Me.optNow.Enabled = True
                    Me.chkCopyPlan.Enabled = True

                End If

            End If

        End If

        If e.Parameter.StartsWith("SAVE") Then
            Dim SourceEmployeeID As Integer
            Dim bolCopyPlan As Boolean

            If Me.intIDEmployee > 0 Then

                Me.intIDGroupSelected = Val(hdnGroupSelected.Value)

                If Me.intIDGroupSelected <= 0 Then
                    ShowMessage(Me.Page, "", Me.Language.Translate("GroupRequired.Message", Me.DefaultScope))
                Else
                    bolCopyPlan = Me.chkCopyPlan.Checked

                    If Me.ddlSourceEmployee.SelectedIndex > -1 Then
                        SourceEmployeeID = Me.ddlSourceEmployee.SelectedItem.Value
                    Else
                        SourceEmployeeID = -1
                    End If

                    If Me.optNow.Checked Then
                        If API.EmployeeServiceMethods.UpdateEmployeeGroup(Me, Me.intIDEmployee, Me.intIDGroupSelected, DateTime.Now.Date, bolCopyPlan, SourceEmployeeID, LockedDayAction.ReplaceAll, LockedDayAction.ReplaceAll, ShiftPermissionAction.ContinueAll, Nothing, , False) Then
                            Me.CanClose = True
                            Me.MustRefresh = "4"
                        End If
                    Else
                        API.EmployeeServiceMethods.UpdateEmployeeGroup(Me, Me.intIDEmployee, Me.intIDGroupSelected, txtMoveDate.Date, bolCopyPlan, SourceEmployeeID, LockedDayAction.ReplaceAll, LockedDayAction.ReplaceAll, ShiftPermissionAction.ContinueAll, Nothing, , False)
                        Me.CanClose = True
                        Me.MustRefresh = "4"

                    End If
                End If
                'ScriptManager.RegisterStartupScript(Me, Me.GetType(), "endLoading", "showLoadingGrid(false)", True)
                ASPxCallbackMoveContenido.JSProperties.Add("cpActionRO", "SAVE")
                ASPxCallbackMoveContenido.JSProperties.Add("cpResultRO", "OK")
            End If
        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        If strButtonKey = "NoActiveContractKey" Then
            Me.CanClose = True
        End If

    End Sub

End Class