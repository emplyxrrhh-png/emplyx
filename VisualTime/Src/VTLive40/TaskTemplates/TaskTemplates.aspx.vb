Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class TaskTemplates
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class TaskProjectsCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="oType")>
        Public Type As String

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="taskFields")>
        Public taskFields As TaskProjectsTemplateField()

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class TaskProjectsTemplateField

        <Runtime.Serialization.DataMember(Name:="idtasktemplatefield")>
        Public idtasktemplatefield As Integer

        <Runtime.Serialization.DataMember(Name:="name")>
        Public name As String

    End Class

    Private Const FeatureAlias As String = "Tasks.TemplatesDefinition"
    Private Const FeatureEmployees As String = ""
    Private oPermission As Permission

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")

        Me.InsertExtraJavascript("TaskProjects", "~/TaskTemplates/Scripts/TaskProjects.js")
        Me.InsertExtraJavascript("ProjectsV2", "~/TaskTemplates/Scripts/ProjectsV2.js")
        Me.InsertExtraJavascript("TaskTemplatesV2", "~/TaskTemplates/Scripts/TaskTemplatesV2.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesTaskTemplates.TreeCaption = Me.Language.Translate("TreeCaptionTaskTemplates", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        If Not IsPostBack Then
            LoadCombos()
        End If

        'Comprobem si no hi han registres, en aquest cas, carreguem un registre nou
        Dim dTbl As DataTable = API.TaskTemplatesServiceMethods.GetProjectTemplatesDataTable(Me.Page, "", False)
        If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New TaskProjectsCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Type
            Case "T"
                ProcessTaskTemplateRequest(oParameters)
                trTaskTemplate.Style("Display") = ""
                trProjects.Style("Display") = "none"
            Case "P"
                ProcessProjectsRequest(oParameters)
                trTaskTemplate.Style("Display") = "none"
                trProjects.Style("Display") = ""
        End Select
    End Sub

    Private Sub LoadCombos()
        cmbGroup.Items.Clear()
        cmbGroup.ValueType = GetType(Integer)
        Dim IdPassport As Integer = WLHelperWeb.CurrentPassport().ID

        Dim dTblGroups As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me, IdPassport, True)
        If dTblGroups.Rows.Count > 0 Then
            For Each dRow As DataRow In dTblGroups.Rows
                If dRow("ID").ToString <> "0" Then
                    cmbGroup.Items.Add(dRow("Name"), dRow("ID"))
                End If
            Next
        End If
    End Sub

#Region "TaskTemplates"

    Private Sub ProcessTaskTemplateRequest(ByVal oParameters As TaskProjectsCallbackRequest)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case oParameters.Action
                Case "GETTASKTEMPLATE"
                    LoadTaskTemplate(oParameters)
                Case "SAVETASKTEMPLATE"
                    SaveTaskTemplate(oParameters)
            End Select

            ProcessTaskTemplateSelectedTabVisible(oParameters)
        End If
    End Sub

    Private Sub ProcessTaskTemplateSelectedTabVisible(ByVal oParameters As TaskProjectsCallbackRequest)
        Me.div20.Style("display") = "none"
        Me.div21.Style("display") = "none"
        Me.div22.Style("display") = "none"
        Me.div23.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div20.Style("display") = ""
            Case 1
                Me.div21.Style("display") = ""
            Case 2
                Me.div22.Style("display") = ""
            Case 3
                Me.div23.Style("display") = ""
        End Select
    End Sub

    Private Sub LoadTaskTemplate(ByVal oParameters As TaskProjectsCallbackRequest, Optional ByVal eTaskTemplate As roTaskTemplate = Nothing)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oTaskTemplate As roTaskTemplate = Nothing

        Try
            If eTaskTemplate Is Nothing Then
                oTaskTemplate = API.TaskTemplatesServiceMethods.GetTaskTemplate(Me, oParameters.ID, False)
            Else
                oTaskTemplate = eTaskTemplate
            End If

            If oTaskTemplate Is Nothing Then Exit Sub

            'Check Permissions
            Dim disControls As Boolean = False
            If Me.oPermission < Permission.Write Then
                Me.DisableControls(Me.Controls)
            Else
                Me.lblEmpSelect.Attributes.Add("OnClick", "javascript: TaskDetailShowSelector();")
            End If

            If oTaskTemplate.IDProject > 0 Then
                If ProjectTaskConfig.Contains("IDProjectTemplate") Then
                    ProjectTaskConfig("IDProjectTemplate") = oTaskTemplate.IDProject
                Else
                    ProjectTaskConfig.Add("IDProjectTemplate", oTaskTemplate.IDProject)
                End If
            Else
                ProjectTaskConfig("IDProjectTemplate") = ProjectTaskConfig("IDProjectTemplate")
            End If

            txtTaskTemplateName.Text = oTaskTemplate.Name
            txtShortName.Text = oTaskTemplate.ShortName
            txtDescription.Text = oTaskTemplate.Description
            colorTaskTemplate.Color = System.Drawing.ColorTranslator.FromWin32(oTaskTemplate.Color)
            txtTag.Value = oTaskTemplate.Tag
            TrackBarPriority.Value = oTaskTemplate.Priority
            txtInitialTime.Text = roConversions.ConvertHoursToTime(oTaskTemplate.InitialTime)

            Try
                If txtInitialTime.Text.Split(":")(0) = "00" Then
                    txtInitialTime.Text = txtInitialTime.Text.Substring(1)
                End If
            Catch ex As Exception
            End Try

            optActivAllways.Checked = False
            optActivByDate.Checked = False
            optActivByEndTask.Checked = False
            optActivByIniTask.Checked = False

            Select Case oTaskTemplate.TypeActivation
                Case TaskTypeActivationEnum._ALWAYS
                    optActivAllways.Checked = True
                Case TaskTypeActivationEnum._ATDATE
                    optActivByDate.Checked = True
                Case TaskTypeActivationEnum._ATFINISHTASK
                    optActivByEndTask.Checked = True
                Case TaskTypeActivationEnum._ATRUNTASK
                    optActivByIniTask.Checked = True
            End Select

            optClosingAllways.Checked = False
            optClosingByDate.Checked = False
            Select Case oTaskTemplate.TypeClosing
                Case TaskTypeClosingEnum._ATDATE
                    optClosingByDate.Checked = True
                Case TaskTypeClosingEnum._UNDEFINED
                    optClosingAllways.Checked = True
            End Select

            Dim activationTask As Integer = -1
            If oTaskTemplate.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK Or oTaskTemplate.TypeActivation = TaskTypeActivationEnum._ATRUNTASK Then
                activationTask = oTaskTemplate.ActivationTask
            End If
            If ProjectTaskConfig.Contains("ActivationTask") Then
                ProjectTaskConfig("ActivationTask") = activationTask
            Else
                ProjectTaskConfig.Add("ActivationTask", activationTask)
            End If

            Dim strAux As String = String.Empty
            Select Case oTaskTemplate.TypeActivation
                Case TaskTypeActivationEnum._ATFINISHTASK
                    strAux = API.TaskTemplatesServiceMethods.GetNameTask(Me, roTypes.Any2Integer(oTaskTemplate.ActivationTask))
                    Me.txtEndTask.Value = strAux
                    Me.txtIniTask.Value = String.Empty
                    If ProjectTaskConfig.Contains("IdTaskActivationType") Then
                        ProjectTaskConfig("IdTaskActivationType") = oTaskTemplate.ActivationTask
                    Else
                        ProjectTaskConfig.Add("IdTaskActivationType", oTaskTemplate.ActivationTask)
                    End If
                Case TaskTypeActivationEnum._ATRUNTASK
                    strAux = API.TaskTemplatesServiceMethods.GetNameTask(Me, roTypes.Any2Integer(oTaskTemplate.ActivationTask))
                    Me.txtEndTask.Value = String.Empty
                    Me.txtIniTask.Value = strAux
                    If ProjectTaskConfig.Contains("IdTaskActivationType") Then
                        ProjectTaskConfig("IdTaskActivationType") = oTaskTemplate.ActivationTask
                    Else
                        ProjectTaskConfig.Add("IdTaskActivationType", oTaskTemplate.ActivationTask)
                    End If
                Case Else
                    Me.txtEndTask.Value = String.Empty
                    Me.txtIniTask.Value = String.Empty
                    If ProjectTaskConfig.Contains("IdTaskActivationType") Then
                        ProjectTaskConfig("IdTaskActivationType") = -1
                    Else
                        ProjectTaskConfig.Add("IdTaskActivationType", -1)
                    End If
            End Select

            optTypeCollabAny.Checked = False
            optTypeCollabFirst.Checked = False
            Select Case oTaskTemplate.TypeCollaboration
                Case TaskTypeCollaborationEnum._ANY
                    optTypeCollabAny.Checked = True
                Case TaskTypeCollaborationEnum._THEFIRST
                    optTypeCollabFirst.Checked = True
            End Select

            optColabAllEmp.Checked = False
            optColabOnlyOneEmp.Checked = False
            Select Case oTaskTemplate.ModeCollaboration
                Case TaskModeCollaborationEnum._ALLTHESAMETIME
                    optColabAllEmp.Checked = True
                Case TaskModeCollaborationEnum._ONEPERSONATTIME
                    optColabOnlyOneEmp.Checked = True
            End Select

            optAutEmpAll.Checked = False
            optAutEmpSelect.Checked = False
            Select Case oTaskTemplate.TypeAuthorization
                Case TaskTypeAuthorizationEnum._ANYEMPLOYEE
                    optAutEmpAll.Checked = True
                Case TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES
                    optAutEmpSelect.Checked = True
            End Select

            'obtener empleados de la tarea
            Dim strEmployeesTask As String = String.Empty
            If oTaskTemplate.Employees IsNot Nothing Then
                For Each item In oTaskTemplate.Employees
                    strEmployeesTask &= item.ID & ","
                Next
                If strEmployeesTask.EndsWith(",") Then strEmployeesTask = strEmployeesTask.Substring(0, strEmployeesTask.Length - 1)
            End If

            'obtener grupos de la tarea
            Dim strGroupsTask As String = String.Empty
            If oTaskTemplate.Groups IsNot Nothing Then
                For Each item In oTaskTemplate.Groups
                    strGroupsTask &= item.ID & ","
                Next
                If strGroupsTask.EndsWith(",") Then strGroupsTask = strGroupsTask.Substring(0, strGroupsTask.Length - 1)
            End If

            LoadEmployeeGroupsToSelector(strEmployeesTask, strGroupsTask)

            Dim nEmployees As Double = API.TaskTemplatesServiceMethods.GetEmployeesFromTask(Me, oParameters.ID, strEmployeesTask, strGroupsTask)

            Dim strEmpNumber As String = Me.Language.Translate("TaskDetail.Seleccionar", Me.DefaultScope)
            If (nEmployees > 0) Then strEmpNumber = nEmployees & " " & Me.Language.Translate("TaskDetail.Seleccionados", Me.DefaultScope)

            Me.lblEmpSelect.Text = strEmpNumber
        Catch ex As Exception
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETTASKTEMPLATE")
            If strError = String.Empty Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oTaskTemplate.Name)
                ASPxCallbackPanelContenido.JSProperties.Add("cpFieldsGridRO", CreateFieldsGridsJSON(oTaskTemplate))
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            End If

        End Try
    End Sub

    Private Sub SaveTaskTemplate(ByVal oParameters As TaskProjectsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentTaskTemplate As roTaskTemplate = Nothing

        Try
            Dim IsNegative As Boolean = False
            Dim bolIsNew As Boolean = False

            If oParameters.ID = 0 Or oParameters.ID = -1 Then
                oCurrentTaskTemplate = New roTaskTemplate()
                oCurrentTaskTemplate.ID = -1
                oCurrentTaskTemplate.Passport = WLHelperWeb.CurrentPassport.ID
            Else
                oCurrentTaskTemplate = API.TaskTemplatesServiceMethods.GetTaskTemplate(Me, oParameters.ID, False)
            End If

            If oCurrentTaskTemplate Is Nothing Then Exit Sub

            oCurrentTaskTemplate.Name = txtTaskTemplateName.Text
            oCurrentTaskTemplate.IDProject = ProjectTaskConfig.Get("IDProjectTemplate")
            oCurrentTaskTemplate.ShortName = txtShortName.Text
            oCurrentTaskTemplate.Description = txtDescription.Text
            oCurrentTaskTemplate.Color = Drawing.ColorTranslator.ToWin32(colorTaskTemplate.Color)
            oCurrentTaskTemplate.Tag = txtTag.Value
            oCurrentTaskTemplate.Priority = roTypes.Any2Integer(TrackBarPriority.Value)
            oCurrentTaskTemplate.InitialTime = roTypes.Any2Time(txtInitialTime.Text).NumericValue

            If optTypeCollabAny.Checked Then
                oCurrentTaskTemplate.TypeCollaboration = TaskTypeCollaborationEnum._ANY
            Else
                oCurrentTaskTemplate.TypeCollaboration = TaskTypeCollaborationEnum._THEFIRST
            End If

            If optActivAllways.Checked Then
                oCurrentTaskTemplate.TypeActivation = TaskTypeActivationEnum._ALWAYS
            ElseIf optActivByDate.Checked Then
                oCurrentTaskTemplate.TypeActivation = TaskTypeActivationEnum._ATDATE
            ElseIf optActivByEndTask.Checked Then
                oCurrentTaskTemplate.TypeActivation = TaskTypeActivationEnum._ATFINISHTASK
                oCurrentTaskTemplate.ActivationTask = roTypes.Any2Integer(ProjectTaskConfig.Get("IdTaskActivationType"))
            ElseIf optActivByIniTask.Checked Then
                oCurrentTaskTemplate.TypeActivation = TaskTypeActivationEnum._ATRUNTASK
                oCurrentTaskTemplate.ActivationTask = roTypes.Any2Integer(ProjectTaskConfig.Get("IdTaskActivationType"))
            End If

            If optClosingAllways.Checked Then
                oCurrentTaskTemplate.TypeClosing = TaskTypeClosingEnum._UNDEFINED
            Else
                oCurrentTaskTemplate.TypeClosing = TaskTypeClosingEnum._ATDATE
            End If

            If optColabAllEmp.Checked Then
                oCurrentTaskTemplate.ModeCollaboration = TaskModeCollaborationEnum._ALLTHESAMETIME
            Else
                oCurrentTaskTemplate.ModeCollaboration = TaskModeCollaborationEnum._ONEPERSONATTIME
            End If

            If optAutEmpAll.Checked Then
                oCurrentTaskTemplate.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
            Else
                oCurrentTaskTemplate.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES
            End If

            'Obtener empleados y grupos seleccionados en la tarea
            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpDetailTaskGrid")
            If oTreeState.Selected1 <> String.Empty Then

                Dim oArrEmployees As New Generic.List(Of roEmployeeTaskTemplateDescription)
                Dim oArrGroups As New Generic.List(Of roGroupTaskTemplateDescription)

                Dim arr() As String = oTreeState.Selected1.Split(",")
                For Each item As String In arr
                    If item.IndexOf("A") = 0 Then
                        oArrGroups.Add(New roGroupTaskTemplateDescription() With {.ID = item.Substring(1)})
                    ElseIf item.IndexOf("B") = 0 Then
                        oArrEmployees.Add(New roEmployeeTaskTemplateDescription() With {.ID = item.Substring(1)})
                    End If
                Next

                oCurrentTaskTemplate.Employees = oArrEmployees
                oCurrentTaskTemplate.Groups = oArrGroups
            Else
                oCurrentTaskTemplate.Employees = Nothing
                oCurrentTaskTemplate.Groups = Nothing

                If oCurrentTaskTemplate.TypeAuthorization = TaskTypeAuthorizationEnum._ASSIGNMENTEMPLOYEES Or oCurrentTaskTemplate.TypeAuthorization = TaskTypeAuthorizationEnum._SELECTEDEMPLOYEES Then
                    oCurrentTaskTemplate.TypeAuthorization = TaskTypeAuthorizationEnum._ANYEMPLOYEE
                End If
            End If

            If API.TaskTemplatesServiceMethods.SaveTaskTemplate(Me, oCurrentTaskTemplate, True) = True Then
                If oParameters.taskFields IsNot Nothing AndAlso oParameters.taskFields.Length > 0 Then
                    Dim fieldsToSave As New Generic.List(Of roTaskFieldTaskTemplate)
                    For Each oSelectedField As TaskProjectsTemplateField In oParameters.taskFields
                        Dim curField As roTaskFieldTaskTemplate = New roTaskFieldTaskTemplate()
                        curField.IDField = oSelectedField.idtasktemplatefield
                        curField.IDTaskTemplate = oCurrentTaskTemplate.ID
                        fieldsToSave.Add(curField)
                    Next

                    API.UserFieldServiceMethods.SaveTaskTemplateFields(Me.Page, oCurrentTaskTemplate.ID, fieldsToSave)
                Else
                    API.UserFieldServiceMethods.SaveTaskTemplateFields(Me.Page, oCurrentTaskTemplate.ID, New Generic.List(Of roTaskFieldTaskTemplate))
                End If

                Dim rOK As New roJSON.JSONError(False, "OK:" & oCurrentTaskTemplate.ID)
                strMessage = rOK.toJSON
            Else
                Dim rError As New roJSON.JSONError(True, API.TaskTemplatesServiceMethods.LastErrorText)
                strMessage = rError.toJSON
                strError = "KO"
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, API.TaskTemplatesServiceMethods.LastErrorText)
            strMessage = rError.toJSON
            strError = "KO"
        Finally

            If strError = String.Empty Then
                oParameters.ID = oCurrentTaskTemplate.ID
                Dim treePath As String = "/source/A" & oCurrentTaskTemplate.IDProject & "/B" & oCurrentTaskTemplate.ID
                HelperWeb.roSelector_SetSelection("B" & oCurrentTaskTemplate.ID.ToString, treePath, "ctl00_contentMainBody_roTreesTaskTemplates")
                LoadTaskTemplate(oParameters)

                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVETASKTEMPLATE"
            Else
                LoadTaskTemplate(oParameters, oCurrentTaskTemplate)
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVETASKTEMPLATE"
                ASPxCallbackPanelContenido.JSProperties("cpFieldsGridRO") = CreateFieldsGridsJSON(oCurrentTaskTemplate, oParameters.taskFields)
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)

            End If

        End Try
    End Sub

    Private Sub LoadEmployeeGroupsToSelector(ByVal strEmployees As String, ByVal strGroups As String)

        Try

            Dim strItems As String = String.Empty

            If strEmployees <> String.Empty Or strGroups <> String.Empty Then

                Dim bAgregarItem As Boolean = False

                Dim tmpLista As New Generic.List(Of String)

                If strGroups <> "" And strGroups <> "-1" Then 'construir cadena con los grupos
                    For Each strGroup As String In strGroups.Split(",")
                        'ppr --> para nuevo arbol v3 en la lista de grupos obtenida del perfil hay que eliminar los grupos a los que no tenga permiso el usuario
                        If FeatureEmployees <> "" Then
                            If Me.HasFeaturePermissionByGroup(FeatureEmployees, Permission.Read, strGroup, "U") Then
                                strItems &= "A" & strGroup & ","
                            End If
                        Else
                            strItems &= "A" & strGroup & ","
                        End If
                    Next
                End If
                If strEmployees <> "" And strEmployees <> "-1" Then 'construir cadena con los empleados
                    For Each strEmployee As String In strEmployees.Split(",")
                        'ppr --> para nuevo arbol v3 en la lista de empleados obtenida del perfil hay que eliminar los empleados a los que no tenga permiso el usuario
                        If FeatureEmployees <> "" Then
                            If Me.HasFeaturePermissionByEmployee(FeatureEmployees, Permission.Read, strEmployee, "U") Then
                                strItems &= "B" & strEmployee & ","
                            End If
                        Else
                            strItems &= "B" & strEmployee & ","
                        End If
                    Next
                End If

                If strItems.EndsWith(",") Then strItems = strItems.Substring(0, strItems.Length - 1)

            End If

            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpDetailTaskGrid")
            oTreeState.Selected1 = strItems
            HelperWeb.roSelector_SetTreeState(oTreeState)
        Catch
        End Try
    End Sub

    Private Function CreateFieldsGridsJSON(ByVal oCurrentTaskTemplate As roTaskTemplate, Optional ByVal fields() As TaskProjectsTemplateField = Nothing) As String
        Try

            Dim oJTaskFieldsTask As New Generic.List(Of Object)
            Dim oJFTaskFieldsTask As Generic.List(Of JSONFieldItem)
            Dim strJSONGroups As String = ""

            If fields Is Nothing Then
                'If Request("ID") = "-1" Then Exit Sub
                Dim dt As DataTable = API.UserFieldServiceMethods.GetTaskTemplateFieldsDataSet(Me.Page, oCurrentTaskTemplate.ID)

                If (dt IsNot Nothing) Then
                    For Each cRow As DataRow In dt.Rows
                        oJFTaskFieldsTask = New Generic.List(Of JSONFieldItem)
                        oJFTaskFieldsTask.Add(New JSONFieldItem("IDTaskTemplateField", roTypes.Any2String(cRow("IDField")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFTaskFieldsTask.Add(New JSONFieldItem("Name", roTypes.Any2String(cRow("FieldName")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJTaskFieldsTask.Add(oJFTaskFieldsTask)
                    Next
                End If
            Else
                For Each oField As TaskProjectsTemplateField In fields
                    oJFTaskFieldsTask = New Generic.List(Of JSONFieldItem)
                    oJFTaskFieldsTask.Add(New JSONFieldItem("IDTaskTemplateField", oField.idtasktemplatefield, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskFieldsTask.Add(New JSONFieldItem("Name", oField.name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJTaskFieldsTask.Add(oJFTaskFieldsTask)
                Next
            End If

            For Each oObj As Object In oJTaskFieldsTask
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)

            Return strJSONGroups
        Catch ex As Exception
            Return ""
        End Try
    End Function

#End Region

#Region "Projects"

    Private Sub ProcessProjectsRequest(ByVal oParameters As TaskProjectsCallbackRequest)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case oParameters.Action
                Case "GETPROJECT"
                    LoadProject(oParameters)
                Case "SAVEPROJECT"
                    SaveProject(oParameters)
            End Select

            ProcessProjectSelectedTabVisible(oParameters)
        End If
    End Sub

    Private Sub ProcessProjectSelectedTabVisible(ByVal oParameters As TaskProjectsCallbackRequest)
        Me.div00.Style("display") = "none"
        Me.div01.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
            Case 1
                Me.div01.Style("display") = ""
        End Select
    End Sub

    Private Sub LoadProject(ByVal oParameters As TaskProjectsCallbackRequest, Optional ByVal eProject As roProjectTemplates = Nothing)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentProject As roProjectTemplates = Nothing

        Try
            If eProject Is Nothing Then
                oCurrentProject = API.TaskTemplatesServiceMethods.GetProjectTemplate(Me, oParameters.ID, False)
            Else
                oCurrentProject = eProject
            End If

            If oCurrentProject Is Nothing Then Exit Sub

            If ProjectTaskConfig.Contains("IDProjectTemplate") Then
                ProjectTaskConfig("IDProjectTemplate") = oCurrentProject.ID
            Else
                ProjectTaskConfig.Add("IDProjectTemplate", oCurrentProject.ID)
            End If

            'Check Permissions
            Dim disControls As Boolean = False
            If Me.oPermission < Permission.Write Then
                Me.DisableControls(Me.Controls)
            End If

            Dim bolMoveIcon As Boolean = False

            Dim nRowControl As DataRow
            Dim strCols() As String = {Me.Language.Translate("ColumnProjectName", Me.DefaultScope)}
            Dim sizeCols() As String = {"600px"}
            Dim cssCols() As String = {"GridStyle-cellheader"}

            'Carrega Horaris del grup
            Dim dtblCurrent As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(oCurrentProject.ID, "", Me, "", False)
            If (dtblCurrent Is Nothing) Then dtblCurrent = New DataTable()

            For y As Integer = dtblCurrent.Columns.Count - 1 To 2 Step -1
                dtblCurrent.Columns.Remove(dtblCurrent.Columns(y).ColumnName)
            Next

            Dim tblControlCurrent As DataTable

            If dtblCurrent.Rows.Count > 0 Then
                tblControlCurrent = New DataTable
                tblControlCurrent.Columns.Add("NomCamp") 'Nom del Camp
                tblControlCurrent.Columns.Add("NomParam") 'Parametre que es pasara
                tblControlCurrent.Columns.Add("Visible") 'Si es te que carregar en les columnes o no...

                nRowControl = tblControlCurrent.NewRow
                nRowControl("NomCamp") = "ID"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = False
                tblControlCurrent.Rows.Add(nRowControl)

                nRowControl = tblControlCurrent.NewRow
                nRowControl("NomCamp") = "Project"
                nRowControl("NomParam") = ""
                nRowControl("Visible") = True
                tblControlCurrent.Rows.Add(nRowControl)

                Dim htmlHGridCurrent As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
                Me.divHeaderProject.Controls.Add(htmlHGridCurrent)

                Dim htmlTGridCurrent As HtmlTable = creaGridLists(dtblCurrent, strCols, sizeCols, tblControlCurrent, 1)
                Me.divGridProject.Controls.Add(htmlTGridCurrent)
            Else
                Me.divHeaderProject.InnerHtml = Me.Language.Translate("NoTaskTemplates", Me.DefaultScope) '"No hay empleados actualmente"
            End If

            cmbGroup.SelectedItem = cmbGroup.Items.FindByValue(oCurrentProject.IDCenter)
            If cmbGroup.SelectedItem Is Nothing Then
                ' Si no esta en la lista hay que añadirlo porque el centro actualmente esta inactivo
                Dim oCenter As roBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, oCurrentProject.IDCenter, False)

                If oCenter IsNot Nothing Then
                    Dim newItem As New DevExpress.Web.ListEditItem(oCenter.Name & ("*"), oCenter.ID)
                    Me.cmbGroup.Items.Add(newItem)
                    Me.cmbGroup.SelectedItem = newItem
                Else
                    cmbGroup.SelectedItem = cmbGroup.Items.FindByValue(CType(0, Short))
                End If
            End If

            txtProjectName.Text = oCurrentProject.Project
        Catch ex As Exception
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETPROJECT")
            If strError = String.Empty Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentProject.Project)
                ASPxCallbackPanelContenido.JSProperties.Add("cpFieldsGridRO", CreateFieldsGridsJSON(oCurrentProject))
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            End If

        End Try
    End Sub

    Private Sub SaveProject(ByVal oParameters As TaskProjectsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentProject As roProjectTemplates = Nothing

        Try

            oCurrentProject = API.TaskTemplatesServiceMethods.GetProjectTemplate(Me, oParameters.ID, False)

            If oCurrentProject Is Nothing Then Exit Sub

            If cmbGroup.SelectedItem IsNot Nothing Then oCurrentProject.IDCenter = cmbGroup.SelectedItem.Value
            oCurrentProject.Project = txtProjectName.Text
            If (oCurrentProject.Passport = -1) Then
                oCurrentProject.Passport = WLHelperWeb.CurrentPassport.ID
            End If

            If API.TaskTemplatesServiceMethods.SaveProjectTemplate(Me, oCurrentProject, True) = True Then
                If oParameters.taskFields IsNot Nothing AndAlso oParameters.taskFields.Length > 0 Then
                    Dim fieldsToSave As New Generic.List(Of roTaskFieldProjectTemplate)
                    For Each oSelectedField As TaskProjectsTemplateField In oParameters.taskFields
                        Dim curField As roTaskFieldProjectTemplate = New roTaskFieldProjectTemplate()
                        curField.IDField = oSelectedField.idtasktemplatefield
                        curField.IDProjectTemplate = oCurrentProject.ID
                        fieldsToSave.Add(curField)
                    Next

                    API.UserFieldServiceMethods.SaveProjectTemplateFields(Me.Page, oCurrentProject.ID, fieldsToSave)
                Else
                    API.UserFieldServiceMethods.SaveProjectTemplateFields(Me.Page, oCurrentProject.ID, New Generic.List(Of roTaskFieldProjectTemplate))
                End If

                Dim rOK As New roJSON.JSONError(False, "OK:" & oCurrentProject.ID)
                strMessage = rOK.toJSON
            Else
                Dim rError As New roJSON.JSONError(True, API.TaskTemplatesServiceMethods.LastErrorText)
                strMessage = rError.toJSON
                strError = "KO"
            End If
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, API.TaskTemplatesServiceMethods.LastErrorText)
            strMessage = rError.toJSON
            strError = "KO"
        Finally

            If strError = String.Empty Then
                oParameters.ID = oCurrentProject.ID
                Dim treePath As String = "/source/A" & oCurrentProject.ID
                HelperWeb.roSelector_SetSelection("A" & oCurrentProject.ID.ToString, treePath, "ctl00_contentMainBody_roTreesTaskTemplates")
                LoadProject(oParameters)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
            Else
                LoadProject(oParameters, oCurrentProject)
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEPROJECT"
                ASPxCallbackPanelContenido.JSProperties("cpFieldsGridRO") = CreateFieldsGridsJSON(oCurrentProject, oParameters.taskFields)
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)

            End If

        End Try
    End Sub

    Private Function CreateFieldsGridsJSON(ByVal oCurrentTaskTemplate As roProjectTemplates, Optional ByVal fields() As TaskProjectsTemplateField = Nothing) As String
        Try

            Dim oJTaskFieldsTask As New Generic.List(Of Object)
            Dim oJFTaskFieldsTask As Generic.List(Of JSONFieldItem)
            Dim strJSONGroups As String = ""

            If fields Is Nothing Then
                'If Request("ID") = "-1" Then Exit Sub
                Dim dt As DataTable = API.UserFieldServiceMethods.GetProjectTemplateFieldsDataSet(Me.Page, oCurrentTaskTemplate.ID)

                If (dt IsNot Nothing) Then
                    For Each cRow As DataRow In dt.Rows
                        oJFTaskFieldsTask = New Generic.List(Of JSONFieldItem)
                        oJFTaskFieldsTask.Add(New JSONFieldItem("IDTaskTemplateField", roTypes.Any2String(cRow("IDField")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFTaskFieldsTask.Add(New JSONFieldItem("Name", roTypes.Any2String(cRow("FieldName")), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJTaskFieldsTask.Add(oJFTaskFieldsTask)
                    Next
                End If
            Else
                For Each oField As TaskProjectsTemplateField In fields
                    oJFTaskFieldsTask = New Generic.List(Of JSONFieldItem)
                    oJFTaskFieldsTask.Add(New JSONFieldItem("IDTaskTemplateField", oField.idtasktemplatefield, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJFTaskFieldsTask.Add(New JSONFieldItem("Name", oField.name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    oJTaskFieldsTask.Add(oJFTaskFieldsTask)
                Next
            End If

            For Each oObj As Object In oJTaskFieldsTask
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)

            Return strJSONGroups
        Catch ex As Exception
            Return ""
        End Try
    End Function

    Private Function creaHeaderLists(ByVal nomCols() As String, ByVal sizeCols() As String, ByVal cssCols() As String) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            For n As Integer = 0 To nomCols.Length - 1
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = cssCols(n) '"GridStyle-cellheader"
                hTCell.InnerHtml = nomCols(n)
                If nomCols(n) = "" Then hTCell.InnerText = " "
                hTCell.Width = sizeCols(n)
                hTRow.Cells.Add(hTCell)
            Next

            'Celda d'espai per edicio
            hTCell = New HtmlTableCell
            hTCell.InnerText = " "
            hTCell.Width = "40px"
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function creaGridLists(ByVal dTable As DataTable, Optional ByVal nomCols() As String = Nothing, Optional ByVal sizeCols() As String = Nothing, Optional ByVal dTblControls As DataTable = Nothing, Optional ByVal colSpanCol As Integer = 1) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            Dim strClickShow As String = ""         'Href onclick Mode edicio
            Dim strClickMove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            'hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            'Bucle als registres

            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim colsizeint As Integer = -1
                For y As Integer = 0 To dTable.Columns.Count - 1
                    'Comproba si es una columna que no es te de visualitzar
                    If dTblControls IsNot Nothing Then
                        Dim dRowSel() As DataRow = dTblControls.Select("NomCamp = '" & dTable.Columns(y).ColumnName & "'")
                        If dRowSel.Length > 0 Then
                            If dRowSel(0).Item("Visible") = False Then Continue For
                        End If
                    End If

                    colsizeint += 1
                    hTCell = New HtmlTableCell
                    hTCell.Width = sizeCols(colsizeint)

                    'Cambia el alternateRow
                    hTCell.Attributes("class") = "GridStyle-cell" & altRow
                    hTCell.Style("display") = "table-cell"

                    'hTCell.InnerText = dTable.Columns(y).ColumnName & ":" & dTable.Rows(n)(y).ToString
                    hTCell.InnerText = dTable.Rows(n)(y).ToString
                    If IsDate(dTable.Rows(n)(y)) Then
                        hTCell.InnerText = Format(dTable.Rows(n)(y), HelperWeb.GetShortDateFormat)
                        If hTCell.InnerText = "01/01/2079" Then hTCell.InnerText = ""
                        'hTCell.Style("text-align") = "center"
                    End If
                    If hTCell.InnerText = "" Then hTCell.InnerText = " "
                    'Si es la ultima columna (per tancar el row)
                    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

                    'Carrega la celda al row
                    hTRow.Cells.Add(hTCell)
                Next
                hTable.Rows.Add(hTRow)
            Next
            Return hTable
        Catch ex As Exception
            Dim htmlTableErr As New HtmlTable
            Dim htmlTableErrCell As New HtmlTableCell
            Dim htmlTableErrRow As New HtmlTableRow
            htmlTableErrCell.InnerHtml = ex.Message.ToString & " " & ex.StackTrace.ToString
            htmlTableErrRow.Cells.Add(htmlTableErrCell)
            htmlTableErr.Rows.Add(htmlTableErrRow)
            Return htmlTableErr
        End Try
    End Function

#End Region

End Class