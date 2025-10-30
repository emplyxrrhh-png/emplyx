Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Wizards_LaunchTaskTemplateWizard
    Inherits PageBase

#Region "Declarations"

    Private intActivePage As Integer

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("LaunchTaskTemplatesDev", "~/TaskTemplates/Scripts/LaunchTaskTemplatesDev.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Tasks.Definition", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

        If Not Me.IsPostBack Then
            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text
            Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
            Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text
            Me.lblStep4Title.Text = Me.hdnStepTitle.Text & Me.lblStep4Title.Text
            Me.lblStep5Title.Text = Me.hdnStepTitle.Text & Me.lblStep5Title.Text
            Me.intActivePage = 0
        Else

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
            If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
            If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
            If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4
            If Me.divStep5.Style("display") <> "none" Then Me.intActivePage = 5
            BindGridTaskTemplates(False)
            BindGridProjectFields(False)
            BindGridTasksFields(False)
        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        Dim intOldPage As Integer
        If Me.CheckPage(Me.intActivePage) Then
            intOldPage = Me.intActivePage

            If (Me.intActivePage = 1) Then
                If (CheckProjectFieldsAvailable(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value))) Then
                    Me.intActivePage += 1
                Else
                    Me.intActivePage += 2
                End If
            ElseIf (Me.intActivePage = 4) Then
                If (CheckTaskTemplateFieldsAvailable()) Then
                    Me.intActivePage += 1
                Else
                    Me.intActivePage += 2
                End If
            Else
                Me.intActivePage += 1
            End If

            If (Me.intActivePage = 3) Then
                Me.treeTaskTemplates_Value.Value = "Empty"
            End If

            Me.PageChange(intOldPage, Me.intActivePage, True)
        Else
            ProcessNewPage(Me.intActivePage, False)
        End If

        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "lndgGrid", "ShowLoadingGrid(false);", True)
    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim intOldPage As Integer = Me.intActivePage

        If (Me.intActivePage = 3) Then
            If (CheckProjectFieldsAvailable(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value))) Then
                Me.intActivePage -= 1
            Else
                Me.intActivePage -= 2
            End If
        Else
            Me.intActivePage -= 1
        End If

        Me.PageChange(intOldPage, Me.intActivePage, False)
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "lndgGrid", "ShowLoadingGrid(false);", True)
    End Sub

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim bolSaved As Boolean = False
            Dim strErrorInfo As String = ""
            Dim n As Integer = 0

            Dim dt As DataTable = Me.TasksDataTable(False)
            dt.Columns.Add(New DataColumn("ProjectName", GetType(String)))
            dt.Columns.Add(New DataColumn("IDBusinessCenter", GetType(String)))
            dt.Columns.Add(New DataColumn("BarCode", GetType(String)))

            For Each cRow As DataRow In dt.Rows
                cRow("ProjectName") = txtProjectName.Value
                cRow("IDBusinessCenter") = cRow("IDBusiness")
                If chkAutomaticBarCodes.Checked = True Then cRow("BarCode") = Now.ToString("yyyyMMddhhmmssffftt") & n
                n += 1
            Next

            If API.TaskTemplatesServiceMethods.SaveTasksFromTemplates(WLHelperWeb.CurrentPassport.IDParentPassport, Me.Page, dt) Then
                bolSaved = True

                If (CheckProjectFieldsAvailable(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value)) = False) Then
                    CreateColumnsProjectFields()
                    BindGridProjectFields(True)
                End If
                If (CheckTaskTemplateFieldsAvailable() = False) Then
                    CreateColumnsTaskFields()
                    BindGridTasksFields(True)
                End If

                'obtenemos las tareas del proyecto que acabamos de crear para poder crear los campos de la ficha
                Dim taskDt As DataTable = API.TasksServiceMethods.GetTasksByProjectName(Me.Page, Me.txtProjectName.Value)
                Dim fieldsDT As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField, "")
                If taskDt IsNot Nothing Then
                    For Each tRow As DataRow In taskDt.Rows
                        Dim IDTask As Integer = roTypes.Any2Integer(tRow("ID"))
                        Dim tFields As New Generic.List(Of roTaskField)

                        Dim existingTasks As DataRow() = Me.TaskFieldsTemplateDataTable(False).Select("ShortName = '" & tRow("ShortName") & "'")

                        If (existingTasks.Length > 0) Then
                            Dim taskRow As DataRow = existingTasks(0)
                            Dim projectFields As DataTable = Me.ProjectFieldsTemplateDataTable(False)
                            Dim tField As New roTaskField
                            'Cogemos los campos por defecto del proyecto
                            Dim alreadyInserted As New Generic.List(Of Integer)
                            For Each pfRow As DataRow In projectFields.Rows
                                alreadyInserted.Add(roTypes.Any2Integer(pfRow("IDField")))
                                Dim currentField As DataRow = fieldsDT.Select("ID = " & pfRow("IDField"))(0)
                                tField = New roTaskField
                                tField.IDTask = IDTask
                                tField.IDField = currentField("ID")
                                tField.Type = currentField("Type")
                                tField.FieldName = currentField("Name")
                                tField.Action = currentField("Action")
                                If (roTypes.Any2Boolean(pfRow("FieldEnabled")) = True) Then
                                    tField.FieldValue = pfRow("FieldValue")
                                End If

                                tFields.Add(tField)

                            Next

                            For Each tfDef As DataRow In fieldsDT.Rows
                                If (alreadyInserted.Contains(roTypes.Any2Integer(tfDef("ID"))) = False) Then
                                    If (roTypes.Any2Boolean(taskRow("FieldAssigned_" & tfDef("ID"))) = True) Then
                                        tField = New roTaskField
                                        tField.IDTask = IDTask
                                        tField.IDField = tfDef("ID")
                                        tField.Type = tfDef("Type")
                                        tField.FieldName = tfDef("Name")
                                        tField.Action = tfDef("Action")
                                        If (roTypes.Any2Integer(tfDef("Action")) = ActionTypes.aCreate And taskRow("FieldEditable_" & tfDef("ID")) = True) Then
                                            tField.FieldValue = taskRow("FieldValue_" & tfDef("ID"))
                                        End If
                                        tFields.Add(tField)
                                    End If
                                End If
                            Next

                            If (API.TasksServiceMethods.SaveTaskFields(Me.Page, IDTask, tFields) = False) Then
                                bolSaved = False
                                Exit For
                            End If
                        End If
                    Next
                    If Not bolSaved Then
                        For Each tRow As DataRow In taskDt.Rows
                            Dim IDTask As Integer = roTypes.Any2Integer(tRow("ID"))
                            Dim existingTasks As DataRow() = Me.TaskFieldsTemplateDataTable(False).Select("ShortName = '" & tRow("ShortName") & "'")
                            If (existingTasks.Length > 0) Then
                                API.TasksServiceMethods.DeleteTaskByID(Me.Page, IDTask, False)
                            End If
                        Next
                    End If
                End If

            End If

            Me.lblWelcome1.Text = Me.Language.Translate("End.TaskTemplateLaunchWelcome1.Text", Me.DefaultScope)
            If bolSaved Then

                Me.MustRefresh = "9"

                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.TaskTemplateLaunchWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.TaskTemplateLaunchWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")

            Dim lastPageIndex As Integer = 4

            If (CheckTaskTemplateFieldsAvailable()) Then lastPageIndex = 5
            Me.PageChange(lastPageIndex, 0, True)

        End If
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "lndgGrid", "ShowLoadingGrid(false);", True)
    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        If strButtonKey = "MaxEmployeesAcceptKey" Then

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1 ' Pantalla introduccion nombre, centro de coste y plantilla

                If (Me.cmbProjectTemplates_Value.Value = String.Empty) Then
                    strMsg = Me.Language.Translate("ProjectTemplate.SelectProject", DefaultScope) '"Debe seleccionar un proyecto"
                ElseIf (Me.cmbBusinessCenter_Value.Value = String.Empty) Then
                    strMsg = Me.Language.Translate("ProjectTemplate.SelectBusinessCenter", DefaultScope) '"Debe seleccionar un centro de negocios"
                ElseIf Me.txtProjectName.Value.Trim = String.Empty Then
                    strMsg = Me.Language.Translate("ProjectTemplate.ProjectNameBlank", DefaultScope) '"El nombre del proyecto no puede estar en blanco"
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case 2 ' Pantalla introduccion campos de usuario de proyecto
                Dim dt As DataTable = Me.ProjectFieldsTemplateDataTable(False)
                Dim incompleteFields As Boolean = False
                For Each cRow As DataRow In dt.Rows

                    If incompleteFields = False AndAlso roTypes.Any2Integer(cRow("IDField")) > 3 AndAlso roTypes.Any2Boolean(cRow("FieldEnabled")) = True Then
                        Dim test As Double = 0
                        If (Double.TryParse(roTypes.Any2String(cRow("FieldValue")), test) = False) Then
                            Dim params As New Generic.List(Of String)
                            incompleteFields = True
                            params.Add(cRow("FieldName"))
                            strMsg = Me.Language.Translate("ProjectFieldTemplate.FieldTypeError", DefaultScope, params) '"No ha introducido valor para todos los campos"
                        End If

                    End If

                    If (incompleteFields = False AndAlso (cRow("FieldValue") Is System.DBNull.Value Or roTypes.Any2String(cRow("FieldValue")) = "" _
                        AndAlso roTypes.Any2Boolean(cRow("FieldEnabled")) = True)) Then
                        incompleteFields = True
                        strMsg = Me.Language.Translate("ProjectFieldTemplate.NoFieldValue", DefaultScope) '"No ha introducido valor para todos los campos"
                    End If

                    If (incompleteFields) Then
                        Exit For
                    End If
                Next
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case 3 ' Pantalla de seleccion de tareas a lanzar
                bolRet = False
                Dim selectedTreeNodes As String = String.Empty
                For Each curNode As TreeNode In Me.treeTaskTemplates.Nodes(0).ChildNodes
                    If (curNode.Checked) Then
                        selectedTreeNodes &= curNode.Text & "_" & curNode.Value & "#"
                        bolRet = True
                    End If
                Next

                If (selectedTreeNodes = String.Empty) Then
                    Me.treeTaskTemplates_Value.Value = "Empty"
                Else
                    Me.treeTaskTemplates_Value.Value = selectedTreeNodes
                End If

                If (bolRet = False) Then
                    strMsg = Me.Language.Translate("ProjectTemplate.NoTemplateSelected", DefaultScope) '"No hay ninguna tarea seleccionada."
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case 4 ' Configuración de las tareas

                Dim dt As DataTable = Me.TasksDataTable(False)
                Dim incompleteFields As Boolean = False
                For Each cRow As DataRow In dt.Rows
                    'If (incompleteFields = False AndAlso roConversions.ConvertTimeToHours(roTypes.Any2String(cRow("InitialDuration"))) = 0) Then
                    '    incompleteFields = True
                    '    strMsg = Me.Language.Translate("ProjectTemplate.NoTaskDuration", DefaultScope) '"No ha introducido la duración de la tarea"
                    'End If
                    'If (incompleteFields = False AndAlso cRow("InitialDate") Is System.DBNull.Value) Then
                    '    incompleteFields = True
                    '    strMsg = Me.Language.Translate("ProjectTemplate.NoInitialDate", DefaultScope) '"No ha introducido fecha inicio de la tarea"
                    'End If
                    'If (incompleteFields = False AndAlso cRow("EndDate") Is System.DBNull.Value) Then
                    '    incompleteFields = True
                    '    strMsg = Me.Language.Translate("ProjectTemplate.NoEndDate", DefaultScope) '"No ha introducido fecha fin de la tarea"
                    'End If
                    If (incompleteFields = False AndAlso cRow("ActivateByDateDate") Is System.DBNull.Value AndAlso
                        roTypes.Any2Integer(cRow("TypeActivation")) = TaskTypeActivationEnum._ATDATE) Then
                        incompleteFields = True
                        strMsg = Me.Language.Translate("ProjectTemplate.NoActivationDate", DefaultScope) '"No ha introducido fecha activación de la tarea"
                    End If
                    If (incompleteFields = False AndAlso cRow("CloseByDateDate") Is System.DBNull.Value AndAlso
                        roTypes.Any2Integer(cRow("TypeClosing")) = TaskTypeClosingEnum._ATDATE) Then
                        incompleteFields = True
                        strMsg = Me.Language.Translate("ProjectTemplate.NoClosingDate", DefaultScope) '"No ha introducido fecha de cierre de la tarea"
                    End If
                    If (incompleteFields = False AndAlso roTypes.Any2Integer(cRow("IDSelectedTask")) = 0 _
                        AndAlso (roTypes.Any2Integer(cRow("TypeActivation")) = TaskTypeActivationEnum._ATFINISHTASK Or roTypes.Any2Integer(cRow("TypeActivation")) = TaskTypeActivationEnum._ATRUNTASK)) Then
                        incompleteFields = True
                        strMsg = Me.Language.Translate("ProjectTemplate.NoTaskSelected", DefaultScope) '"No ha seleccionado tarea de activación"
                    End If

                    If (incompleteFields) Then
                        Exit For
                    End If
                Next

                If strMsg <> "" Then bolRet = False
                Me.lblStep4Error.Text = strMsg
            Case 5 ' Configuración de los campos de usuario de tarea
                Dim dtFields As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField)
                Dim dt As DataTable = Me.TaskFieldsTemplateDataTable(False)
                Dim incompleteFields As Boolean = False
                For Each cRow As DataRow In dt.Rows

                    For Each oDefRow As DataRow In dtFields.Rows

                        If incompleteFields = False AndAlso roTypes.Any2Integer(oDefRow("ID")) > 3 AndAlso roTypes.Any2Boolean(cRow("FieldEditable_" & oDefRow("ID"))) = True Then
                            Dim test As Double = 0
                            If (Double.TryParse(roTypes.Any2String(cRow("FieldValue_" & oDefRow("ID"))), test) = False) Then
                                Dim params As New Generic.List(Of String)
                                incompleteFields = True
                                params.Add(cRow("FieldValue_" & oDefRow("ID")))
                                strMsg = Me.Language.Translate("ProjectFieldTemplate.TaskFieldTypeError", DefaultScope, params) '"No ha introducido valor para todos los campos"
                            End If
                        End If

                        If (incompleteFields = False AndAlso roTypes.Any2Boolean(cRow("FieldEditable_" & oDefRow("ID"))) = True AndAlso
                                                roTypes.Any2String(cRow("FieldValue_" & oDefRow("ID"))) = "") Then
                            incompleteFields = True
                            strMsg = Me.Language.Translate("ProjectFieldTemplate.NoFieldValue", DefaultScope) '"No ha introducido fecha de cierre de la tarea"
                        End If

                        If (incompleteFields = True) Then
                            Exit For
                        End If
                    Next

                    If (incompleteFields) Then
                        Exit For
                    End If
                Next

                If strMsg <> "" Then bolRet = False
                Me.lblStep5Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer, ByVal reloadGrid As Boolean)

        ProcessOldPage(intOldPage)
        ProcessNewPage(intActivePage, reloadGrid)

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intOldPage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If
        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intActivePage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        Dim lastPageIndex As Integer = 4

        If (CheckTaskTemplateFieldsAvailable()) Then lastPageIndex = 5

        If intOldPage = lastPageIndex And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < lastPageIndex, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = lastPageIndex, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Function CheckProjectFieldsAvailable(ByVal idProjectTemplate As Integer) As Boolean
        Dim fieldsExist As Boolean = False

        'Checkeamos si existen campos de ficha para proyecto
        Dim dt As DataTable = API.UserFieldServiceMethods.GetProjectTemplateFieldsDataSet(Me.Page, idProjectTemplate)
        If dt IsNot Nothing Then
            For Each dRow As DataRow In dt.Rows

                Dim tField As roTaskFieldDefinition = API.UserFieldServiceMethods.GetTaskField(Me.Page, roTypes.Any2Integer(dRow("IDField")), False)

                If tField.Action = ActionTypes.aCreate Then
                    fieldsExist = True
                End If

                If fieldsExist = True Then
                    Exit For
                End If
            Next
        End If

        Return fieldsExist
    End Function

    Private Function CheckTaskTemplateFieldsAvailable() As Boolean
        Dim fieldsExist As Boolean = False

        If Me.treeTaskTemplates.Nodes.Count = 0 Then
            Return False
        End If

        For Each curNode As TreeNode In Me.treeTaskTemplates.Nodes(0).ChildNodes
            If (curNode.Checked) Then
                'Checkeamos si existen campos de ficha para proyecto
                Dim dt As DataTable = API.UserFieldServiceMethods.GetTaskTemplateFieldsDataSet(Me.Page, roTypes.Any2Integer(curNode.Value))
                If dt IsNot Nothing Then
                    For Each dRow As DataRow In dt.Rows

                        Dim tField As roTaskFieldDefinition = API.UserFieldServiceMethods.GetTaskField(Me.Page, roTypes.Any2Integer(dRow("IDField")), False)

                        If tField.Action = ActionTypes.aCreate Then
                            fieldsExist = True
                        End If

                        If fieldsExist = True Then
                            Exit For
                        End If
                    Next
                End If
            End If

            If fieldsExist = True Then
                Exit For
            End If

        Next

        Return fieldsExist
    End Function

    Private Sub ProcessOldPage(ByVal intOldPage As Integer)
        Select Case intOldPage
            Case 2
            Case 3
            Case 4
            Case 5
            Case 6
        End Select
    End Sub

    Private Sub ProcessNewPage(ByVal intActivePage As Integer, ByVal reloadGrid As Boolean)
        Select Case intActivePage
            Case 1
                LoadCombos()
                Me.treeTaskTemplates.Nodes.Clear()
            Case 2
                CreateColumnsProjectFields()
                BindGridProjectFields(reloadGrid)
            Case 3
                Dim projectID As Integer = roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value)
                Dim oProjectTemplate As roProjectTemplates = API.TaskTemplatesServiceMethods.GetProjectTemplate(Me.Page, projectID, False)
                Dim rootNode As New TreeNode(oProjectTemplate.Project)
                rootNode.ImageUrl = "~/TaskTemplates/Images/TaskTemplateSelector/ProjectIco.png"
                rootNode.Expanded = True
                rootNode.ShowCheckBox = False

                If Me.treeTaskTemplates.Nodes.Count = 0 Then
                    Dim dTbl As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(projectID, "", Me.Page, "", False)
                    For Each oRow As DataRow In dTbl.Rows
                        Dim oNode As TreeNode = New TreeNode(roTypes.Any2String(oRow("Name")), roTypes.Any2String(oRow("ID")))
                        oNode.ImageUrl = "~/TaskTemplates/Images/TaskTemplateSelector/TaskTemplateIco.png"

                        If (Me.treeTaskTemplates_Value.Value.Contains(roTypes.Any2String(oRow("Name")) & "_" & roTypes.Any2String(oRow("ID")))) Then
                            oNode.Checked = True
                        Else
                            oNode.Checked = False
                        End If

                        rootNode.ChildNodes.Add(oNode)
                    Next
                    Me.treeTaskTemplates.Nodes.Add(rootNode)
                End If
            Case 4
                CreateColumnsTasks()
                BindGridTaskTemplates(reloadGrid)
            Case 5
                If (CheckTaskTemplateFieldsAvailable()) Then
                    CreateColumnsTaskFields()
                    BindGridTasksFields(reloadGrid)
                Else
                    'lanzamos la copia de plantilla
                End If
            Case 6
                'lanzamos la copia de plantilla
        End Select
    End Sub

    Private Sub LoadCombos()
        Dim dTblGroup As DataTable = API.TaskTemplatesServiceMethods.GetProjectTemplatesDataTable(Me.Page, "", False)

        Dim taskTemplatesTreeState = HelperWeb.roSelector_GetTreeState("ctl00_contentMainBody_roTreesTaskTemplates")

        If dTblGroup.Rows.Count > 0 Then
            Me.cmbProjectTemplates.ClearItems()
            For Each dRGroup As DataRow In dTblGroup.Rows
                Me.cmbProjectTemplates.AddItem(dRGroup("Project"), dRGroup("ID"), "")
            Next
        End If

        If taskTemplatesTreeState IsNot Nothing AndAlso taskTemplatesTreeState.SelectedPath1 <> String.Empty Then
            Dim strPath As String = taskTemplatesTreeState.SelectedPath1
            While (strPath.IndexOf("/") <> -1 AndAlso strPath.StartsWith("A") = False)
                strPath = strPath.Remove(0, strPath.IndexOf("/") + 1)
            End While

            strPath = strPath.Substring(1, strPath.Length - 1)
            If (strPath.IndexOf("/") <> -1) Then
                strPath = strPath.Substring(0, strPath.IndexOf("/"))
            End If

            Me.cmbProjectTemplates.SelectedValue = strPath

        End If

        Dim dTblGroups As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me, WLHelperWeb.CurrentPassport().ID, True)
        If dTblGroups.Rows.Count > 0 Then
            Me.cmbBusinessCenter.ClearItems()
            For Each dRow As DataRow In dTblGroups.Rows
                Me.cmbBusinessCenter.AddItem(dRow("Name"), dRow("ID"), "")
            Next
        End If
    End Sub

#Region "Grid Tareas"

    Protected Sub lblTasksCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblTasksCaptionText", Me.DefaultScope)
    End Sub

    Private Sub CreateColumnsTasks()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridComboCommand As GridViewDataComboBoxColumn
        Dim GridViewDataDateColumn As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridTasks.Columns.Clear()
        Me.GridTasks.KeyFieldName = "ViewName"
        Me.GridTasks.SettingsText.EmptyDataRow = " "
        Me.GridTasks.SettingsEditing.Mode = GridViewEditingMode.Inline
        Me.GridTasks.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        Me.GridTasks.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        Me.GridTasks.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        Me.GridTasks.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        Me.GridTasks.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        'Name
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.TaskName", DefaultScope)
        GridColumn.FieldName = "ViewName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Width = 85
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        Me.GridTasks.Columns.Add(GridColumn)

        'Centro de coste
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridTasks.Column.BusinessCenter", DefaultScope) '"SelectedTask"
        GridComboCommand.FieldName = "IDBusiness"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "ID"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Integer)
        GridComboCommand.PropertiesComboBox.DataSource = Me.GetAvailableBusinessCenters()
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.Width = 40
        GridComboCommand.ReadOnly = False
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        GridComboCommand.Visible = True
        Me.GridTasks.Columns.Add(GridComboCommand)

        'Duracion(Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.InitialTime", DefaultScope) '"Valor"
        GridColumn.FieldName = "InitialDuration"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<-9999..9999>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 40
        GridColumn.Visible = True
        Me.GridTasks.Columns.Add(GridColumn)

        'Fecha inicio(Unbound)
        GridViewDataDateColumn = New GridViewDataDateColumn()
        GridViewDataDateColumn.Caption = Me.Language.Translate("GridTasks.Column.DateStart", DefaultScope) '"Valor"
        GridViewDataDateColumn.FieldName = "InitialDate"
        GridViewDataDateColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridViewDataDateColumn.ReadOnly = False
        GridViewDataDateColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.Width = 50
        GridViewDataDateColumn.Visible = True
        Me.GridTasks.Columns.Add(GridViewDataDateColumn)

        'Hora final(Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.DateTimeStart", DefaultScope) '"Valor"
        GridColumn.FieldName = "InitialDateTime"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<00..23>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 40
        GridColumn.Visible = True
        Me.GridTasks.Columns.Add(GridColumn)

        'Fecha final(Unbound)
        GridViewDataDateColumn = New GridViewDataDateColumn()
        GridViewDataDateColumn.Caption = Me.Language.Translate("GridTasks.Column.DateEnd", DefaultScope) '"Valor"
        GridViewDataDateColumn.FieldName = "EndDate"
        GridViewDataDateColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridViewDataDateColumn.ReadOnly = False
        GridViewDataDateColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.Width = 50
        GridViewDataDateColumn.Visible = True
        Me.GridTasks.Columns.Add(GridViewDataDateColumn)

        'Hora final(Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.DateTimeEnd", DefaultScope) '"Valor"
        GridColumn.FieldName = "EndDateTime"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<00..23>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 40
        GridColumn.Visible = True
        Me.GridTasks.Columns.Add(GridColumn)

        'TypeData
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridTasks.Column.SelectedTask", DefaultScope) '"SelectedTask"
        GridComboCommand.FieldName = "IDSelectedTask"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "ID"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Integer)
        GridComboCommand.PropertiesComboBox.DataSource = Me.GetAvailableTasksData(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value))
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.Width = 85
        GridComboCommand.ReadOnly = False
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        GridComboCommand.Visible = True
        Me.GridTasks.Columns.Add(GridComboCommand)

        'Activar por fecha(Unbound)
        GridViewDataDateColumn = New GridViewDataDateColumn()
        GridViewDataDateColumn.Caption = Me.Language.Translate("GridTasks.Column.ActivateByDateDate", DefaultScope) '"Valor"
        GridViewDataDateColumn.FieldName = "ActivateByDateDate"
        GridViewDataDateColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridViewDataDateColumn.ReadOnly = False
        GridViewDataDateColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.Width = 50
        GridViewDataDateColumn.Visible = True
        Me.GridTasks.Columns.Add(GridViewDataDateColumn)

        'Activar por Hora (Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.ActivateByDateTime", DefaultScope) '"Valor"
        GridColumn.FieldName = "ActivateByDateTime"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<00..23>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 40
        GridColumn.Visible = True
        Me.GridTasks.Columns.Add(GridColumn)

        'Close por fecha(Unbound)
        GridViewDataDateColumn = New GridViewDataDateColumn()
        GridViewDataDateColumn.Caption = Me.Language.Translate("GridTasks.Column.CloseByDateDate", DefaultScope) '"Valor"
        GridViewDataDateColumn.FieldName = "CloseByDateDate"
        GridViewDataDateColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridViewDataDateColumn.ReadOnly = False
        GridViewDataDateColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridViewDataDateColumn.Width = 50
        GridViewDataDateColumn.Visible = True
        Me.GridTasks.Columns.Add(GridViewDataDateColumn)

        'Close por Hora (Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.CloseByDateTime", DefaultScope) '"Valor"
        GridColumn.FieldName = "CloseByDateTime"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.PropertiesTextEdit.MaskSettings.Mask = "<00..23>:<00..59>"
        GridColumn.PropertiesTextEdit.Width = 40
        GridColumn.Visible = True
        Me.GridTasks.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 15
        VisibleIndex = VisibleIndex + 1
        Me.GridTasks.Columns.Add(GridColumnCommand)
    End Sub

    Private Sub BindGridTaskTemplates(Optional ByVal bolReload As Boolean = False)
        Me.GridTasks.DataSource = Me.TaskTemplatesDataView(bolReload)
        Me.GridTasks.DataBind()
    End Sub

    Private ReadOnly Property TaskTemplatesDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.TasksDataTable(bolReload)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
            End If
            Return dv

        End Get
    End Property

    Private Function GetAvailableTasksData(ByVal IDProject As Integer, Optional ByVal excludeID As Integer = -1) As DataView
        Dim dv As DataView = Nothing
        Dim tb As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(IDProject, "", Me.Page, "", False)
        Dim returnTb As New DataTable
        returnTb.Columns.Add(New DataColumn("ID", GetType(Integer)))
        returnTb.Columns.Add(New DataColumn("Name", GetType(String)))

        Dim oNewRow As DataRow = returnTb.NewRow
        With oNewRow
            .Item("ID") = 0
            .Item("Name") = ""
        End With
        returnTb.Rows.Add(oNewRow)

        If (tb IsNot Nothing) Then
            For Each oRow As DataRow In tb.Rows
                If Me.treeTaskTemplates_Value.Value.Contains(roTypes.Any2String(oRow("Name")) & "_" & roTypes.Any2String(oRow("ID"))) Then
                    If (excludeID = -1 Or excludeID <> roTypes.Any2Integer(oRow("ID"))) Then
                        oNewRow = returnTb.NewRow
                        With oNewRow
                            .Item("ID") = oRow("ID")
                            .Item("Name") = oRow("Name")
                        End With
                        returnTb.Rows.Add(oNewRow)
                    End If
                End If
            Next

        End If

        dv = New DataView(returnTb)
        Return dv
    End Function

    Private Function GetAvailableBusinessCenters() As DataView
        Dim dv As DataView = Nothing
        Dim dTblGroups As DataTable = API.TasksServiceMethods.GetBusinessCenterByPassportDataTable(Me, WLHelperWeb.CurrentPassport().ID, True)
        If dTblGroups.Rows.Count > 0 Then
            Me.cmbBusinessCenter.ClearItems()
            For Each dRow As DataRow In dTblGroups.Rows
                Me.cmbBusinessCenter.AddItem(dRow("Name"), dRow("ID"), "")
            Next
        End If

        dv = New DataView(dTblGroups)
        Return dv
    End Function

    Private Property TasksDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbTaskTemplates As DataTable = Session("LaunchTaskTemplate_TasksDataTable")

            If bolReload OrElse tbTaskTemplates Is Nothing Then
                Dim filterList As String = String.Empty
                If (treeTaskTemplates.Nodes.Count > 0) Then
                    For Each curNode As TreeNode In Me.treeTaskTemplates.Nodes(0).ChildNodes
                        If (curNode.Checked) Then
                            If (filterList <> String.Empty) Then filterList &= ","
                            filterList &= curNode.Value
                        End If
                    Next
                End If
                Dim projectID As Integer = roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value)
                Dim tbFullTaskTemplateTasks As DataTable = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(projectID, "", Me.Page, "", False)

                tbTaskTemplates = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(projectID, filterList, Me.Page, "", False)
                tbTaskTemplates.Columns.Add(New DataColumn("IDSelectedTask", GetType(String)))

                'columnas de horas
                tbTaskTemplates.Columns.Add(New DataColumn("InitialDuration", GetType(String)))
                tbTaskTemplates.Columns.Add(New DataColumn("InitialDate", GetType(Date)))
                tbTaskTemplates.Columns.Add(New DataColumn("InitialDateTime", GetType(String)))
                tbTaskTemplates.Columns.Add(New DataColumn("EndDate", GetType(Date)))
                tbTaskTemplates.Columns.Add(New DataColumn("EndDateTime", GetType(String)))
                tbTaskTemplates.Columns.Add(New DataColumn("ActivateByDateDate", GetType(Date)))
                tbTaskTemplates.Columns.Add(New DataColumn("ActivateByDateTime", GetType(String)))
                tbTaskTemplates.Columns.Add(New DataColumn("CloseByDateDate", GetType(Date)))
                tbTaskTemplates.Columns.Add(New DataColumn("CloseByDateTime", GetType(String)))
                tbTaskTemplates.Columns.Add(New DataColumn("IDBusiness", GetType(Integer)))

                'añado columna de la clave
                tbTaskTemplates.Columns.Add(New DataColumn("ViewName", GetType(String)))

                For Each r As DataRow In tbTaskTemplates.Rows
                    r("InitialDuration") = roConversions.ConvertHoursToTime(roTypes.Any2Double(r("InitialTime")))
                    r("InitialDate") = r("ExpectedStartDate")
                    r("InitialDateTime") = roTypes.Any2Time(r("ExpectedStartDate")).Hours & ":" & roTypes.Any2Time(r("ExpectedStartDate")).Minutes
                    r("EndDate") = r("ExpectedEndDate")
                    r("EndDateTime") = roTypes.Any2Time(r("ExpectedEndDate")).Hours & ":" & roTypes.Any2Time(r("ExpectedEndDate")).Minutes
                    r("ActivateByDateDate") = r("ActivationDate")
                    r("ActivateByDateTime") = roTypes.Any2Time(r("ActivationDate")).Hours & ":" & roTypes.Any2Time(r("ActivationDate")).Minutes
                    r("CloseByDateDate") = r("ClosingDate")
                    r("CloseByDateTime") = roTypes.Any2Time(r("ClosingDate")).Hours & ":" & roTypes.Any2Time(r("ClosingDate")).Minutes
                    r("IDBusiness") = Me.cmbBusinessCenter_Value.Value

                    'Hacemos una busqueda inteligente en el listado completo de tareas para seleccionar una tarea por defecto
                    If (roTypes.Any2Integer(r("TypeActivation")) = TaskTypeActivationEnum._ATFINISHTASK) Then
                        r("IDSelectedTask") = RecursivelyFindActivationTask(roTypes.Any2Integer(r("ActivationTask")), tbTaskTemplates, tbFullTaskTemplateTasks, TaskTypeActivationEnum._ATFINISHTASK)
                    ElseIf (roTypes.Any2Integer(r("TypeActivation")) = TaskTypeActivationEnum._ATRUNTASK) Then
                        r("IDSelectedTask") = RecursivelyFindActivationTask(roTypes.Any2Integer(r("ActivationTask")), tbTaskTemplates, tbFullTaskTemplateTasks, TaskTypeActivationEnum._ATRUNTASK)
                    End If

                    r("ViewName") = r("Name")
                Next

                'añade campo clave a la tabla
                If Not tbTaskTemplates Is Nothing Then
                    tbTaskTemplates.PrimaryKey = New DataColumn() {tbTaskTemplates.Columns("ViewName")}
                    tbTaskTemplates.AcceptChanges()
                End If

                Session("LaunchTaskTemplate_TasksDataTable") = tbTaskTemplates
            End If

            Return tbTaskTemplates
        End Get
        Set(ByVal value As DataTable)
            Session("LaunchTaskTemplate_TasksDataTable") = value
        End Set
    End Property

    Protected Sub GridTasks_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridTasks.RowUpdating

        Dim tb As DataTable = Me.TasksDataTable()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridTasks.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "IDSelectedTask"
                    dr.Item("IDSelectedTask") = enumerator.Value
                Case "InitialDuration"
                    dr.Item("InitialDuration") = enumerator.Value
                Case "InitialDate"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("InitialDate") = enumerator.Value
                    Else
                        dr.Item("InitialDate") = System.DBNull.Value
                    End If
                Case "InitialDateTime"
                    dr.Item("InitialDateTime") = enumerator.Value
                Case "EndDate"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("EndDate") = enumerator.Value
                    Else
                        dr.Item("EndDate") = System.DBNull.Value
                    End If
                Case "EndDateTime"
                    dr.Item("EndDateTime") = enumerator.Value
                Case "ActivateByDateDate"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("ActivateByDateDate") = enumerator.Value
                    Else
                        dr.Item("ActivateByDateDate") = System.DBNull.Value
                    End If
                Case "ActivateByDateTime"
                    dr.Item("ActivateByDateTime") = enumerator.Value
                Case "CloseByDateDate"
                    If (enumerator.Value IsNot Nothing) Then
                        dr.Item("CloseByDateDate") = enumerator.Value
                    Else
                        dr.Item("CloseByDateDate") = System.DBNull.Value
                    End If
                Case "CloseByDateTime"
                    dr.Item("CloseByDateTime") = enumerator.Value
                Case "IDBusiness"
                    dr.Item("IDBusiness") = enumerator.Value

            End Select

        End While

        Me.TasksDataTable = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridTasks_CustomColumnDisplayText(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewColumnDisplayTextEventArgs) Handles GridTasks.CustomColumnDisplayText
        If e.Column.FieldName = "InitialDate" Or e.Column.FieldName = "EndDate" Or e.Column.FieldName = "ActivateByDateDate" Or e.Column.FieldName = "CloseByDateDate" Then
            If e.Value IsNot System.DBNull.Value AndAlso e.Value IsNot Nothing Then
                Dim AuxDate As Date = e.Value
                e.DisplayText = Globalization.CultureInfo.CurrentUICulture.TextInfo.ToTitleCase(AuxDate.ToShortDateString)
            End If

        End If

    End Sub

    Protected Sub GridTasks_HtmlDataCellPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewTableDataCellEventArgs) Handles GridTasks.HtmlDataCellPrepared
        Dim tb As DataTable = Me.TasksDataTable()
        Dim dRow As DataRow = tb.Rows(e.VisibleIndex)

        If e.DataColumn.FieldName = "IDSelectedTask" Then
            'Dim properties As ComboBoxProperties = (TryCast(e.DataColumn, GridViewDataComboBoxColumn)).PropertiesComboBox
            'properties.DataSource = Me.GetAvailableTasksData(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value), roTypes.Any2Integer(dRow("ID")))

            If (roTypes.Any2Integer(dRow("TypeActivation")) = TaskTypeActivationEnum._ALWAYS Or roTypes.Any2Integer(dRow("TypeActivation")) = TaskTypeActivationEnum._ATDATE) Then
                e.Cell.Enabled = False
                e.Cell.Style("background-color") = "lightgrey"
                e.Cell.Controls.Clear()
            End If
        ElseIf e.DataColumn.FieldName = "ActivateByDateDate" Or e.DataColumn.FieldName = "ActivateByDateTime" Then
            If (roTypes.Any2Integer(dRow("TypeActivation")) <> TaskTypeActivationEnum._ATDATE) Then
                e.Cell.Enabled = False
                e.Cell.Style("background-color") = "lightgrey"
                e.Cell.Controls.Clear()
            End If
        ElseIf e.DataColumn.FieldName = "CloseByDateDate" Or e.DataColumn.FieldName = "CloseByDateTime" Then
            If (roTypes.Any2Integer(dRow("TypeClosing")) <> TaskTypeClosingEnum._ATDATE) Then
                e.Cell.Enabled = False
                e.Cell.Style("background-color") = "lightgrey"
                e.Cell.Controls.Clear()
            End If
        End If
    End Sub

    Protected Sub GridTasks_CellEditorInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewEditorEventArgs) Handles GridTasks.CellEditorInitialize
        Dim tb As DataTable = Me.TasksDataTable()
        Dim dRow As DataRow = tb.Rows(e.VisibleIndex)

        If e.Column.FieldName = "IDSelectedTask" Then
            Dim cmbBox As ASPxComboBox = CType(e.Editor, ASPxComboBox)
            Dim foundItem As ListEditItem = Nothing
            For Each oItem As ListEditItem In cmbBox.Items
                If (roTypes.Any2String(oItem.Value) = roTypes.Any2String(dRow("ID"))) Then
                    foundItem = oItem
                    Exit For
                End If
            Next

            If foundItem IsNot Nothing Then
                cmbBox.Items.Remove(foundItem)
            End If
            If (roTypes.Any2Integer(dRow("TypeActivation")) = TaskTypeActivationEnum._ALWAYS Or roTypes.Any2Integer(dRow("TypeActivation")) = TaskTypeActivationEnum._ATDATE) Then
                e.Editor.Parent.Controls.Clear()
            End If
        ElseIf e.Column.FieldName = "ActivateByDateDate" Or e.Column.FieldName = "ActivateByDateTime" Then
            If (roTypes.Any2Integer(dRow("TypeActivation")) <> TaskTypeActivationEnum._ATDATE) Then
                e.Editor.Parent.Controls.Clear()
            End If
        ElseIf e.Column.FieldName = "CloseByDateDate" Or e.Column.FieldName = "CloseByDateTime" Then
            If (roTypes.Any2Integer(dRow("TypeClosing")) <> TaskTypeClosingEnum._ATDATE) Then
                e.Editor.Parent.Controls.Clear()
            End If
        ElseIf e.Column.FieldName = "InitialDuration" Then
            Dim txt As ASPxTextBox
            Dim grid As ASPxGridView = CType(sender, ASPxGridView)

            txt = CType(e.Editor, ASPxTextBox)
            txt.Width = 36
            txt.Font.Size = 8

            If grid.IsNewRowEditing() Then
                txt.MaskSettings.Mask = "<-9999..9999>:<00..59>"
            Else
                txt.MaskSettings.Mask = "<-9999..9999>:<00..59>"
                'si el numero de horas es cero hacer una chapucilla porque el control no lo pinta bien
                Try
                    If txt.Text.Split(":")(0) = "00" Then
                        txt.Text = txt.Text.Substring(1)
                    End If
                Catch ex As Exception
                End Try

            End If
        End If
    End Sub

    Private Function RecursivelyFindActivationTask(ByVal actualInitTask As Integer, ByVal tbTaskTemplates As DataTable, ByVal tbFullTaskTemplateTasks As DataTable, ByVal runMode As TaskTypeActivationEnum) As String
        Dim realActivationTask As String = "0"

        Dim existingRows As DataRow() = tbTaskTemplates.Select("ID = " & actualInitTask)
        If (existingRows.Length > 0) Then
            realActivationTask = actualInitTask.ToString()
        Else
            'Buscamos la tarea de la plantilla en el listado completo
            Dim fullExistingRows As DataRow() = tbFullTaskTemplateTasks.Select("ID = " & actualInitTask)

            If fullExistingRows.Length > 0 Then
                'Si existe, buscamos en el listado a exportar
                Dim newActivationTask As Integer = roTypes.Any2Integer(fullExistingRows(0)("ActivationTask"))
                existingRows = tbTaskTemplates.Select("ID = " & newActivationTask)
                If existingRows.Length > 0 Then
                    realActivationTask = newActivationTask.ToString()
                Else
                    realActivationTask = RecursivelyFindActivationTask(newActivationTask, tbTaskTemplates, tbFullTaskTemplateTasks, runMode)
                End If
            Else
                realActivationTask = "0"
            End If
        End If

        Return realActivationTask
    End Function

#End Region

#Region "Project fields template"

    Protected Sub lblProjectFieldsCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblProjectFieldsCaptionText", Me.DefaultScope)
    End Sub

    Private Sub CreateColumnsProjectFields()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim currentGrid As ASPxGridView = Nothing

        currentGrid = Me.GridProjectFields

        Dim VisibleIndex As Integer = 0

        currentGrid.Columns.Clear()
        currentGrid.KeyFieldName = "IDField"
        currentGrid.SettingsText.EmptyDataRow = " "
        currentGrid.SettingsEditing.Mode = GridViewEditingMode.Inline
        currentGrid.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        currentGrid.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        currentGrid.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        currentGrid.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        currentGrid.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        'Id
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasksFields.Column.FieldID", DefaultScope)
        GridColumn.FieldName = "IDField"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Width = 15
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        currentGrid.Columns.Add(GridColumn)

        'Name
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasksFields.Column.FieldName", DefaultScope)
        GridColumn.FieldName = "FieldName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Width = 85
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        currentGrid.Columns.Add(GridColumn)

        'Valor del campo(Unbound)
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasksFields.Column.ValueField", DefaultScope) '"Valor"
        GridColumn.FieldName = "FieldValue"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = 40
        GridColumn.Visible = True
        currentGrid.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 15
        VisibleIndex = VisibleIndex + 1
        currentGrid.Columns.Add(GridColumnCommand)

    End Sub

    Private Sub BindGridProjectFields(Optional ByVal bolReload As Boolean = False)
        Me.GridProjectFields.DataSource = Me.ProjectFieldsTemplatesDataView(bolReload)
        Me.GridProjectFields.DataBind()
    End Sub

    Private ReadOnly Property ProjectFieldsTemplatesDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.ProjectFieldsTemplateDataTable(bolReload)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
            End If
            Return dv

        End Get
    End Property

    Private Property ProjectFieldsTemplateDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbTaskFields As DataTable = Nothing
            tbTaskFields = Session("LaunchTaskTemplate_TasksFieldsProjectDataTable")

            If bolReload OrElse tbTaskFields Is Nothing Then
                tbTaskFields = API.UserFieldServiceMethods.GetProjectTemplateFieldsDataSet(Me.Page, roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value))

                tbTaskFields.Columns.Add(New DataColumn("FieldValue", GetType(String)))
                tbTaskFields.Columns.Add(New DataColumn("FieldEnabled", GetType(Boolean)))
                tbTaskFields.Columns.Add(New DataColumn("FieldType", GetType(Integer)))
                tbTaskFields.Columns.Add(New DataColumn("FieldValuesList", GetType(String())))

                Dim dtFields As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField)

                For Each r As DataRow In tbTaskFields.Rows
                    r("FieldValue") = ""
                    r("FieldEnabled") = False
                    For Each oDefinition As DataRow In dtFields.Rows
                        If roTypes.Any2Integer(oDefinition("Action")) = ActionTypes.aCreate AndAlso
                            roTypes.Any2Integer(r("IDField")) = roTypes.Any2Integer(oDefinition("ID")) Then

                            Dim oDef As roTaskFieldDefinition = API.UserFieldServiceMethods.GetTaskField(Me.Page, roTypes.Any2Integer(oDefinition("ID")), False)

                            If (oDef IsNot Nothing) Then
                                r("FieldType") = oDef.ValueType
                                r("FieldValuesList") = oDef.ListValues.ToArray
                            Else
                                r("FieldType") = ValueTypes.aValue
                                r("FieldValuesList") = Nothing
                            End If

                            r("FieldEnabled") = True
                            Exit For
                        End If
                    Next
                Next

                Session("LaunchTaskTemplate_TasksFieldsProjectDataTable") = tbTaskFields
            End If

            Return tbTaskFields
        End Get
        Set(ByVal value As DataTable)
            Session("LaunchTaskTemplate_TasksFieldsProjectDataTable") = value
        End Set
    End Property

    Protected Sub GridProjectFields_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridProjectFields.RowUpdating

        Dim tb As DataTable = Me.ProjectFieldsTemplateDataTable()
        Dim dr As DataRow = Nothing
        Dim selectRow As DataRow() = tb.Select(GridProjectFields.KeyFieldName & "=" & e.Keys(GridProjectFields.KeyFieldName))
        If selectRow.Length > 0 Then
            dr = selectRow(0)
        End If

        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        'Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        'enumerator.Reset()
        'While enumerator.MoveNext()
        '    Select Case enumerator.Key
        '        Case "FieldValue"
        '            If (dr IsNot Nothing) Then
        '                dr.Item("FieldValue") = enumerator.Value
        '            End If
        '    End Select
        'End While

        Dim c As GridViewDataColumn = grid.Columns("FieldValue")
        Dim lt As ASPxTextBox = grid.FindEditRowCellTemplateControl(c, "fieldValueText")
        Dim lc As ASPxComboBox = grid.FindEditRowCellTemplateControl(c, "fieldValueCmb")

        If lt IsNot Nothing Then dr.Item("FieldValue") = lt.Text
        If lc IsNot Nothing Then dr.Item("FieldValue") = lc.SelectedItem.Value

        Me.ProjectFieldsTemplateDataTable = tb
        e.Cancel = True
        grid.CancelEdit()

    End Sub

    Protected Sub GridProjectFields_HtmlDataCellPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewTableDataCellEventArgs) Handles GridProjectFields.HtmlDataCellPrepared
        Dim tb As DataTable = Me.ProjectFieldsTemplateDataTable()
        Dim dRow As DataRow = tb.Rows(e.VisibleIndex)

        If e.DataColumn.FieldName = "FieldValue" Then
            If roTypes.Any2Boolean(dRow("FieldEnabled")) = False Then
                e.Cell.Enabled = False
                e.Cell.Style("background-color") = "lightgrey"
                e.Cell.Controls.Clear()
            End If
        End If
    End Sub

    Protected Sub GridProjectFields_CellEditorInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewEditorEventArgs) Handles GridProjectFields.CellEditorInitialize
        Dim tb As DataTable = Me.ProjectFieldsTemplateDataTable()
        Dim dRow As DataRow = tb.Rows(e.VisibleIndex)

        If e.Column.FieldName = "FieldValue" Then
            If roTypes.Any2Boolean(dRow("FieldEnabled")) = False Then
                e.Editor.Parent.Controls.Clear()
            End If
        End If
    End Sub

    Protected Sub GridProjectFields_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles GridProjectFields.Load
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim col As GridViewDataTextColumn = grid.Columns("FieldValue")
        If col IsNot Nothing Then
            col.EditItemTemplate = New MultiControlEditFormTemplate(Me)
        End If
    End Sub

#End Region

#Region "Tasks fields template"

    Protected Sub lblTaskFieldsCaption_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxLabel = CType(sender, ASPxLabel)
        txtLabel.CssClass = ""
        txtLabel.Text = Me.Language.Translate("lblTaskFieldsCaptionText", Me.DefaultScope)
    End Sub

    Private Sub CreateColumnsTaskFields()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridComboCommand As GridViewDataComboBoxColumn

        Dim currentGrid As ASPxGridView = Nothing

        currentGrid = Me.GridTaskFieldsTask

        Dim VisibleIndex As Integer = 0

        currentGrid.Columns.Clear()
        currentGrid.KeyFieldName = "ViewName"
        currentGrid.SettingsText.EmptyDataRow = " "
        currentGrid.SettingsEditing.Mode = GridViewEditingMode.Inline
        currentGrid.SettingsCommandButton.DeleteButton.Image.Url = "~/Base/Images/Grid/remove.png"
        currentGrid.SettingsCommandButton.EditButton.Image.Url = "~/Base/Images/Grid/edit.png"
        currentGrid.SettingsCommandButton.UpdateButton.Image.Url = "~/Base/Images/Grid/save.png"
        currentGrid.SettingsCommandButton.CancelButton.Image.Url = "~/Base/Images/Grid/cancel.png"
        currentGrid.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        'Name
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridTasks.Column.TaskName", DefaultScope)
        GridColumn.FieldName = "ViewName"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Width = 50
        GridColumn.ReadOnly = True
        GridColumn.Visible = True
        currentGrid.Columns.Add(GridColumn)

        Dim dtFields As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField)

        For Each dRow As DataRow In dtFields.Rows

            Dim fDef As roTaskFieldDefinition = API.UserFieldServiceMethods.GetTaskField(Me.Page, dRow("ID"), False)

            If (fDef.ValueType = ValueTypes.aValue) Then
                'Hora final(Unbound)
                GridColumn = New GridViewDataTextColumn()
                GridColumn.Caption = dRow("Name")
                GridColumn.FieldName = "FieldValue_" & dRow("ID")
                GridColumn.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridColumn.ReadOnly = False
                GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Center
                GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                GridColumn.Width = 20
                GridColumn.Visible = True
                currentGrid.Columns.Add(GridColumn)
            Else
                GridComboCommand = New GridViewDataComboBoxColumn()
                GridComboCommand.Caption = dRow("Name")
                GridComboCommand.FieldName = "FieldValue_" & dRow("ID")
                GridComboCommand.VisibleIndex = VisibleIndex
                VisibleIndex = VisibleIndex + 1
                GridComboCommand.ReadOnly = False
                GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
                GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
                GridComboCommand.Visible = True
                GridComboCommand.PropertiesComboBox.DataSource = fDef.ListValues
                GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
                GridComboCommand.Width = 20

                currentGrid.Columns.Add(GridComboCommand)
            End If
        Next

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 15
        VisibleIndex = VisibleIndex + 1
        currentGrid.Columns.Add(GridColumnCommand)
    End Sub

    Private Sub BindGridTasksFields(Optional ByVal bolReload As Boolean = False)
        Me.GridTaskFieldsTask.DataSource = Me.TaskFieldsTemplatesDataView(bolReload)
        Me.GridTaskFieldsTask.DataBind()
    End Sub

    Private ReadOnly Property TaskFieldsTemplatesDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            Dim tb As DataTable = Me.TaskFieldsTemplateDataTable(bolReload)
            Dim dv As DataView = Nothing
            If tb IsNot Nothing Then
                dv = New DataView(tb)
            End If
            Return dv

        End Get
    End Property

    Private Property TaskFieldsTemplateDataTable(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tbTaskFields As DataTable = Nothing
            tbTaskFields = Session("LaunchTaskTemplate_TasksFieldsTaskDataTable")

            If bolReload OrElse tbTaskFields Is Nothing Then
                If (CheckProjectFieldsAvailable(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value)) = False) Then
                    CreateColumnsProjectFields()
                    BindGridProjectFields(True)
                End If

                Dim filterList As String = String.Empty
                If (treeTaskTemplates.Nodes.Count > 0) Then
                    For Each curNode As TreeNode In Me.treeTaskTemplates.Nodes(0).ChildNodes
                        If (curNode.Checked) Then
                            If (filterList <> String.Empty) Then filterList &= ","
                            filterList &= curNode.Value
                        End If
                    Next
                End If

                tbTaskFields = API.TaskTemplatesServiceMethods.GetTaskTemplatesDataTable(roTypes.Any2Integer(Me.cmbProjectTemplates_Value.Value), filterList, Me.Page, "", False)

                'añado columna de la clave
                tbTaskFields.Columns.Add(New DataColumn("ViewName", GetType(String)))

                Dim dtFields As DataTable = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField)

                For Each dRow As DataRow In dtFields.Rows
                    tbTaskFields.Columns.Add(New DataColumn("FieldValue_" & dRow("ID"), GetType(String)))
                    tbTaskFields.Columns.Add(New DataColumn("FieldEditable_" & dRow("ID"), GetType(Boolean)))
                    tbTaskFields.Columns.Add(New DataColumn("FieldAssigned_" & dRow("ID"), GetType(Boolean)))
                Next

                For Each r As DataRow In tbTaskFields.Rows
                    r("ViewName") = r("Name")
                    Dim tasksFieldsTemplateTasks As DataTable = API.UserFieldServiceMethods.GetTaskTemplateFieldsDataSet(Me.Page, roTypes.Any2Integer(r("ID")))
                    For Each oDefinition As DataRow In dtFields.Rows
                        r("FieldValue_" & oDefinition("ID")) = ""
                        Dim fieldAssigned As Boolean = False
                        For Each tfRow As DataRow In tasksFieldsTemplateTasks.Rows
                            If (roTypes.Any2Integer(tfRow("IDField")) = oDefinition("ID")) Then
                                fieldAssigned = True
                            End If

                            If fieldAssigned = True Then
                                Exit For
                            End If
                        Next

                        r("FieldAssigned_" & oDefinition("ID")) = fieldAssigned

                        If fieldAssigned AndAlso roTypes.Any2Integer(oDefinition("Action")) = ActionTypes.aCreate Then
                            r("FieldEditable_" & oDefinition("ID")) = True
                        Else
                            r("FieldEditable_" & oDefinition("ID")) = False
                        End If

                    Next
                Next

                Session("LaunchTaskTemplate_TasksFieldsTaskDataTable") = tbTaskFields
            End If

            Return tbTaskFields
        End Get
        Set(ByVal value As DataTable)
            Session("LaunchTaskTemplate_TasksFieldsTaskDataTable") = value
        End Set
    End Property

    Protected Sub GridTaskFieldsTask_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles GridTaskFieldsTask.RowUpdating

        Dim tb As DataTable = Me.TaskFieldsTemplateDataTable()
        Dim dr As DataRow = Nothing
        Dim selectRow As DataRow() = tb.Select(GridTaskFieldsTask.KeyFieldName & "='" & e.Keys(GridTaskFieldsTask.KeyFieldName) & "'")
        If selectRow.Length > 0 Then
            dr = selectRow(0)
        End If

        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            If (dr IsNot Nothing) Then
                If enumerator.Key.ToString().StartsWith("FieldValue") Then
                    dr.Item(enumerator.Key) = enumerator.Value
                End If

            End If
        End While

        Me.TaskFieldsTemplateDataTable = tb
        e.Cancel = True
        grid.CancelEdit()
    End Sub

    Protected Sub GridTaskFieldsTask_HtmlDataCellPrepared(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewTableDataCellEventArgs) Handles GridTaskFieldsTask.HtmlDataCellPrepared
        Dim tb As DataTable = Me.TaskFieldsTemplateDataTable()

        Dim projectFields As DataTable = Me.ProjectFieldsTemplateDataTable()

        Dim dRow As DataRow = tb.Rows(e.VisibleIndex)

        If e.DataColumn.FieldName.StartsWith("FieldValue_") Then
            Dim fieldId As Integer = roTypes.Any2Integer(e.DataColumn.FieldName.Split("_")(1))

            Dim alreadyDefined As Boolean = False
            For Each pRow As DataRow In projectFields.Rows
                If roTypes.Any2Integer(pRow("IDField")) = fieldId Then
                    alreadyDefined = True
                End If

                If alreadyDefined Then
                    Exit For
                End If
            Next
            If alreadyDefined Or roTypes.Any2Boolean(dRow("FieldEditable_" & fieldId)) = False Then
                e.Cell.Enabled = False
                e.Cell.Style("background-color") = "lightgrey"
                e.Cell.Controls.Clear()
            End If
        End If
    End Sub

    Protected Sub GridTaskFieldsTask_CellEditorInitialize(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewEditorEventArgs) Handles GridTaskFieldsTask.CellEditorInitialize
        Dim tb As DataTable = Me.TaskFieldsTemplateDataTable()
        Dim dRow As DataRow = tb.Rows(e.VisibleIndex)
        Dim projectFields As DataTable = Me.ProjectFieldsTemplateDataTable()

        If e.Column.FieldName.StartsWith("FieldValue_") Then
            Dim fieldId As Integer = roTypes.Any2Integer(e.Column.FieldName.Split("_")(1))

            Dim alreadyDefined As Boolean = False
            For Each pRow As DataRow In projectFields.Rows
                If roTypes.Any2Integer(pRow("IDField")) = fieldId Then
                    alreadyDefined = True
                End If

                If alreadyDefined Then
                    Exit For
                End If
            Next
            If alreadyDefined Or roTypes.Any2Boolean(dRow("FieldEditable_" & fieldId)) = False Then
                e.Editor.Parent.Controls.Clear()
            End If
        End If
    End Sub

#End Region

#End Region

    Public Class MultiControlEditFormTemplate
        Implements ITemplate

        Dim parentPage As Wizards_LaunchTaskTemplateWizard
        Dim visibleIndex As Integer = -1

        Public Sub New(ByVal parent As Wizards_LaunchTaskTemplateWizard)
            Me.parentPage = parent
        End Sub

        Public Sub InstantiateIn(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
            Dim gridContainer As GridViewEditItemTemplateContainer = TryCast(container, GridViewEditItemTemplateContainer)

            Dim tb As DataTable = Me.parentPage.ProjectFieldsTemplateDataTable()
            Dim dRow As DataRow = tb.Rows(gridContainer.VisibleIndex)

            Me.visibleIndex = gridContainer.VisibleIndex
            If roTypes.Any2Boolean(dRow("FieldEnabled")) = True Then
                Dim panel As New ASPxPanel

                If (roTypes.Any2Integer(dRow("FieldType")) = ValueTypes.aValue) Then
                    Dim txt As New ASPxTextBox
                    txt.ID = "fieldValueText"
                    txt.Width = New Unit("100%")
                    txt.Text = dRow("FieldValue")
                    AddHandler txt.TextChanged, AddressOf onValueChanged
                    panel.Controls.Add(txt)
                Else
                    Dim cmb As New ASPxComboBox

                    Dim listVal As String() = TryCast(dRow("FieldValuesList"), String())
                    For Each cVal As String In listVal
                        cmb.Items.Add(New ListEditItem(cVal, cVal))
                    Next

                    cmb.ID = "fieldValueCmb"
                    cmb.Width = New Unit("100%")
                    cmb.Text = dRow("FieldValue")
                    AddHandler cmb.TextChanged, AddressOf onValueChanged
                    panel.Controls.Add(cmb)
                End If

                container.Controls.Add(panel)
            End If
        End Sub

        Private Sub onValueChanged(ByVal sender As Object, ByVal e As EventArgs)
            Dim newVal As String = ""
            If TypeOf sender Is ASPxComboBox Then
                newVal = CType(sender, ASPxComboBox).SelectedItem.Value
            ElseIf TypeOf sender Is ASPxTextBox Then
                newVal = CType(sender, ASPxTextBox).Text
            End If

            Dim tb As DataTable = Me.parentPage.ProjectFieldsTemplateDataTable()
            Dim dRow As DataRow = tb.Rows(Me.visibleIndex)
            dRow.Item("FieldValue") = newVal
        End Sub

    End Class

End Class