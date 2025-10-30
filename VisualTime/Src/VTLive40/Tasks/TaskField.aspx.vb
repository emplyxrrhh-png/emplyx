Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class TaskField
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"

#Region "Declarations"

    Private oPermission As Permission = Permission.None
    Private TypeValue As ValueTypes = ValueTypes.aValue

#End Region

#Region "Properties"

    Private Property TaskFieldsDefinitionData() As DataTable
        Get
            Dim oData As DataTable = Session("TaskField_TaskFieldTaskData")
            If oData Is Nothing Then

                oData = API.UserFieldServiceMethods.GetTaskFields(Me.Page, Types.TaskField, "")

                Session("TaskField_TaskFieldTaskData") = oData

            End If
            Return oData
        End Get
        Set(ByVal value As DataTable)
            Session("TaskField_TaskFieldTaskData") = value
        End Set
    End Property

    Private Property TaskFieldsListValues() As Generic.List(Of String)
        Get
            Dim oFieldTask As New roTaskFieldDefinition
            Dim oData As New Generic.List(Of String)

            oFieldTask = API.UserFieldServiceMethods.GetTaskField(Me.Page, roTypes.Any2Integer(Request("TaskFieldID")), False)
            If Not oFieldTask Is Nothing Then
                Me.TypeValue = oFieldTask.ValueType
                If Not oFieldTask.ListValues Is Nothing Then
                    For Each strValue As String In oFieldTask.ListValues
                        oData.Add(strValue)
                    Next
                End If
            End If

            Session("TaskField_TaskFieldsListValues") = oData

            Return oData
        End Get
        Set(ByVal value As Generic.List(Of String))
            Session("TaskField_TaskFieldsListValues") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("OpenWindow", "~/Base/Scripts/OpenWindow.js", , True)
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js", , True)

        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("roTaskAlertCriteria", "~/Base/Scripts/roUserFieldCriteria.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes()
        Me.InsertExtraCssIncludes("~/Base/ext-3.4.0/resources/css/ext-all.css")

        ' Si el passport actual no tiene permisso de lectura, redirigimos a página de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Me.LoadTaskFields(Request("SelectedNodes"), Request("TaskFieldID"))
        Me.LoadActions()
        Me.LoadTaskListValues(Request("SelectedNodes"), Request("TaskFieldID"))

        If Not Me.IsPostBack Then

            Me.TaskFieldsDefinitionData = Nothing
            If (Request("TaskFieldID") <> -1) Then
                Me.cmbAvailableFields.SelectedValue = Request("TaskFieldID")
                Me.cmbAvailableFields.Enabled = False
                Me.cmbActions.SelectedText = Request("action")
                Me.txtValue.Value = Request("value")
                Me.cmbListValues.SelectedValue = Request("value")
                If (Request("action") = Me.Language.Translate("ActionTypes." & System.Enum.GetName(GetType(ActionTypes), ActionTypes.aCreate), Me.DefaultScope)) Or (Request("action") = Me.Language.Translate("ActionTypes." & System.Enum.GetName(GetType(ActionTypes), ActionTypes.aBegin), Me.DefaultScope)) Or (Request("action") = Me.Language.Translate("ActionTypes." & System.Enum.GetName(GetType(ActionTypes), ActionTypes.tComplete), Me.DefaultScope)) Then
                    Me.txtValue.Disabled = IIf(TypeValue = ValueTypes.aValue, False, True)
                    Me.cmbListValues.Enabled = IIf(TypeValue = ValueTypes.aValue, False, True)
                    Me.cmbListValues.Visible = IIf(TypeValue = ValueTypes.aValue, False, True)
                    Me.lblList.Visible = IIf(TypeValue = ValueTypes.aValue, False, True)
                Else
                    Me.txtValue.Disabled = True
                    Me.cmbListValues.Enabled = False
                    Me.cmbListValues.Visible = False
                    Me.lblList.Visible = False
                End If

            End If

        End If

        Me.UpdateData()

        If Not Me.IsPostBack Then
            Me.SetPermissions()
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Me.oPermission >= Permission.Write Then
            Dim bolSaved = False

            If Me.ValidateData() Then

                Dim oTaskFields As New Generic.List(Of Object)

                Dim oEmpField As TaskFieldTemplateStructField
                Dim oEmp As New Generic.List(Of TaskFieldTemplateStructField)

                oEmpField = New TaskFieldTemplateStructField
                oEmpField.attname = "idfield"
                oEmpField.value = Me.cmbAvailableFields_Value.Value
                oEmp.Add(oEmpField)

                oEmpField = New TaskFieldTemplateStructField
                oEmpField.attname = "fieldname"
                oEmpField.value = Me.cmbAvailableFields_Text.Value
                oEmp.Add(oEmpField)

                oEmpField = New TaskFieldTemplateStructField
                oEmpField.attname = "value"
                oEmpField.value = Me.txtValue.Value
                oEmp.Add(oEmpField)

                oEmpField = New TaskFieldTemplateStructField
                oEmpField.attname = "action"
                oEmpField.value = Me.cmbActions_Text.Value
                oEmp.Add(oEmpField)

                oTaskFields.Add(oEmp)

                Dim selectedFields As Integer = 0
                Dim strJSON As String = "{rows : [ "
                For Each oObj As Object In oTaskFields
                    selectedFields = selectedFields + 1
                    strJSON &= " {fields:"
                    Dim oEmpFld As Generic.List(Of TaskFieldTemplateStructField)
                    oEmpFld = CType(oObj, Generic.List(Of TaskFieldTemplateStructField))
                    strJSON &= roJSONHelper.Serialize(oEmpFld) & "} ,"
                Next
                strJSON = strJSON.Substring(0, Len(strJSON) - 2) & "]}"

                If selectedFields > 0 Then
                    hdnParams_PageBase.Value = strJSON
                Else
                    hdnParams_PageBase.Value = ""
                End If

                Me.CanClose = True
                Me.MustRefresh = "1"
                ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeScript", "Close()", True)
            End If

        End If

    End Sub

    Protected Sub btCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btCancel.Click
        Me.CanClose = True
        Me.MustRefresh = "0"
        Me.hdnParams_PageBase.Value = ""
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeScript", "Close()", True)
    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Select Case strButtonKey
            Case "DeleteCategory.Answer.Yes"
            Case "DeleteListValue.Answer.Yes"
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadTaskListValues(ByVal selectedNodes As String, ByVal selectedField As String)
        Me.cmbListValues.ClearItems()

        For Each oValue As String In TaskFieldsListValues
            'If (roTypes.Any2Integer(selectedField) = roTypes.Any2Integer(oValue("ID")) Or selectedNodes.IndexOf(roTypes.Any2String(oValue("ID"))) = -1) Then
            Me.cmbListValues.AddItem(oValue.Replace("'", ""), oValue, "onChangeListValue('" & oValue.Replace("'", "") & "')")
            'End If
        Next

    End Sub

    Private Sub LoadTaskFields(ByVal selectedNodes As String, ByVal selectedField As String)
        Me.cmbAvailableFields.ClearItems()

        For Each oValue As DataRow In TaskFieldsDefinitionData.Rows()
            If (roTypes.Any2Integer(selectedField) = roTypes.Any2Integer(oValue("ID")) Or selectedNodes.IndexOf(roTypes.Any2String(oValue("ID"))) = -1) Then
                Me.cmbAvailableFields.AddItem(oValue("Name"), oValue("ID"), "onChangeField('" & oValue("Action") & "','" & oValue("TypeValue") & "','" & oValue("ID") & "')")
            End If
        Next

    End Sub

    Private Sub LoadActions()
        Me.cmbActions.ClearItems()
        For Each oValue As ActionTypes In System.Enum.GetValues(GetType(ActionTypes))
            Me.cmbActions.AddItem(Me.Language.Translate("ActionTypes." & System.Enum.GetName(GetType(ActionTypes), oValue), Me.DefaultScope), oValue, "")
        Next
        Me.cmbActions.Enabled = False
    End Sub

    Private Sub UpdateData()

    End Sub

    Private Function ValidateData() As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""
        Dim bollist As Boolean = False

        If (Me.cmbAvailableFields_Value.Value = "") Then
            strMsg = Me.Language.Translate("SelectNewFields.NoFieldsSelected", DefaultScope)
        End If

        If bolRet Then
            Dim oDefinition As DataTable = TaskFieldsDefinitionData()
            For Each cRow As DataRow In oDefinition.Rows
                If (roTypes.Any2Integer(cRow("ID")) = roTypes.Any2Integer(Me.cmbAvailableFields_Value.Value)) Then

                    If (roTypes.Any2Integer(cRow("Type")) = 1 AndAlso roTypes.Any2Integer(cRow("Action")) = ActionTypes.aCreate) Then
                        Dim result As Integer
                        If (Integer.TryParse(txtValue.Value, result) = False) Then
                            strMsg = Me.Language.Translate("SelectNewFields.FieldMustBeANumber", DefaultScope)
                        End If
                    End If

                    If roTypes.Any2Integer(cRow("TypeValue")) = ValueTypes.aList And txtValue.Value.Length = 0 And roTypes.Any2Integer(cRow("Action")) = ActionTypes.aCreate Then
                        strMsg = Me.Language.Translate("SelectNewFields.NoFieldsSelected", DefaultScope)
                    End If

                End If
            Next
        End If

        If strMsg <> "" Then
            lblError.Text = strMsg
            lblError.Visible = True
            Me.updError.Update()
            bolRet = False
        End If

        Return bolRet

    End Function

    Private Sub SetPermissions()

        If Me.oPermission < Permission.Write Then

            Me.DisableControls()

            Me.btAccept.Style("display") = "none"
            Me.btCancel.Text = Me.Language.Keyword("Button.Close")

        End If

    End Sub

#End Region

End Class