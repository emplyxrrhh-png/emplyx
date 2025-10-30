Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class OptionsTaskField
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"

#Region "Declarations"

    Private oPermission As Permission = Permission.None

#End Region

#Region "Properties"

    Private Property TaskFieldData() As roTaskFieldDefinition
        Get
            Dim oData As roTaskFieldDefinition = Session("TaskField_TaskFieldData")
            If oData Is Nothing Then

                Dim intTaskID As String = Request.Params("TaskFieldID")
                If intTaskID Is Nothing Then intTaskID = 1

                oData = New roTaskFieldDefinition
                If intTaskID <> 0 Then
                    Dim oRows() As DataRow = Me.TaskFieldsData.Select("ID = " & intTaskID)
                    If oRows.Length = 1 Then
                        With oData
                            .ID = oRows(0).Item("ID")
                            .Name = oRows(0).Item("Name")
                            .Type = oRows(0).Item("Type")
                            .Action = oRows(0).Item("Action")
                            .ValueType = oRows(0).Item("TypeValue")
                            Dim _ListValues As New Generic.List(Of String)
                            If roTypes.Any2String(oRows(0).Item("ListValues")) <> "" Then
                                For Each value As String In roTypes.Any2String(oRows(0).Item("ListValues")).Split("~")
                                    _ListValues.Add(value)
                                Next
                                _ListValues.Sort()
                                .ListValues = _ListValues
                            End If

                        End With
                    End If
                End If

                Session("TaskField_TaskFieldData") = oData

            End If
            Return oData
        End Get
        Set(ByVal value As roTaskFieldDefinition)
            Session("TaskField_TaskFieldData") = value
        End Set
    End Property

    Private ReadOnly Property TaskFieldDataChanged() As Boolean
        Get
            Dim oData As roTaskFieldDefinition = Session("TaskField_TaskFieldData")
            Return (oData Is Nothing)
        End Get
    End Property

    Private Property TaskFieldsData() As DataTable
        Get
            Dim tb As DataTable = Nothing
            tb = Session("ConfigurationOptions_TaskFieldsData")
            Return tb
        End Get
        Set(ByVal value As DataTable)
            Session("ConfigurationOptions_TaskFieldsData") = value

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
        Me.InsertExtraJavascript("roTaskFieldCriteria", "~/Base/Scripts/roUserFieldCriteria.js")

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

        Me.LoadTypes()
        Me.LoadActions()
        Me.LoadValueTypes()

        If Not Me.IsPostBack Then

            Me.TaskFieldData = Nothing

        End If

        Me.UpdateData()

        If Not Me.IsPostBack Then
            Me.SetPermissions()
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

    End Sub

    Protected Sub btNewListValueOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNewListValueOK.Click

        If Me.txtNewListValue.Text.Trim <> "" Then
            Dim oRet As Boolean = True

            Dim newVal As Double = 0
            If Me.cmbTypes_Value.Value = FieldTypes.tNumeric Then
                oRet = Double.TryParse(Me.txtNewListValue.Text.Trim.Replace(".", HelperWeb.GetDecimalDigitFormat), newVal)

                'If Not IsNumeric(Me.txtNewListValue.Text.Trim) Then
                '    oRet = False
                'End If

            End If

            If oRet Then
                Dim lstValues As New Generic.List(Of String)

                For Each oItem As ListItem In Me.cmbListValues.Items
                    lstValues.Add(oItem.Text)
                Next

                Dim newValStr As String = ""

                If Me.cmbTypes_Value.Value = FieldTypes.tNumeric Then
                    newValStr = newVal.ToString("F2").Replace(",", ".")
                Else
                    newValStr = Me.txtNewListValue.Text
                End If

                If lstValues.IndexOf(newValStr) < 0 Then
                    lstValues.Add(newValStr)
                Else
                    ' Ya existe el valor en la lista
                    ' ...
                End If

                lstValues.Sort()

                Me.cmbListValues.Items.Clear()
                For Each value As String In lstValues
                    Me.cmbListValues.Items.Add(value)
                Next

                Me.txtNewListValue.Text = ""
            Else
                HelperWeb.ShowMessage(Me, "", Me.Language.Translate("NewListValue.Invalid", Me.DefaultScope))
            End If
        Else
            HelperWeb.ShowMessage(Me, "", Me.Language.Translate("NewListValue.Invalid", Me.DefaultScope))
        End If

    End Sub

    Protected Sub btDeleteListValue_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btDeleteListValue.Click

        If Me.cmbListValues.SelectedIndex >= 0 Then

            Me.cmbListValues.Items.RemoveAt(Me.cmbListValues.SelectedIndex)

            'Dim Param As New ArrayList
            'Param.Add(Me.cmbListValues.SelectedValue)

            'HelperWeb.ShowOptionMessage(Me, Me.Language.Translate("DeleteListValue.Title", Me.DefaultScope), _
            '                                Me.Language.Translate("DeleteListValue.Description", Me.DefaultScope, Param), _
            '                                Me.Language.Translate("DeleteListValue.Answer.Yes.Title", Me.DefaultScope), "DeleteListValue.Answer.Yes", Me.Language.Translate("DeleteListValue.Answer.Yes.Description", Me.DefaultScope), _
            '                                Me.Language.Translate("DeleteListValue.Answer.No.Title", Me.DefaultScope), "DeleteListValue.Answer.No", Me.Language.Translate("DeleteListValue.Answer.No.Description", Me.DefaultScope))

        End If

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Me.oPermission >= Permission.Write Then

            If Me.ValidateData() Then
                Dim _ListValues() As String
                Dim strListValues As String = ""

                With Me.TaskFieldData
                    .Name = Me.txtName.Value
                    .Action = Me.cmbActions_Value.Value
                    '.Action = Me.otype
                    .ValueType = Me.cmbValueTypes_Value.Value
                    ReDim _ListValues(Me.cmbListValues.Items.Count - 1)
                    For n As Integer = 0 To Me.cmbListValues.Items.Count - 1
                        _ListValues(n) = Me.cmbListValues.Items(n).Text
                        strListValues &= "~" & Me.cmbListValues.Items(n).Text
                    Next
                    If strListValues.Length > 0 Then strListValues = strListValues.Substring(1)
                    .ListValues = _ListValues.ToList

                End With

                Dim bolSaved As Boolean = False

                Dim tbTaskfields As DataTable = Me.TaskFieldsData
                If tbTaskfields IsNot Nothing Then
                    Dim oRows() As DataRow = tbTaskfields.Select("ID = " & Me.TaskFieldData.ID)
                    Dim oUserFieldRow As DataRow
                    oUserFieldRow = oRows(0)
                    With Me.TaskFieldData
                        oUserFieldRow("ID") = .ID
                        oUserFieldRow("Name") = .Name
                        oUserFieldRow("Action") = .Action
                        oUserFieldRow("Type") = .Type
                        oUserFieldRow("TypeValue") = .ValueType
                        oUserFieldRow("ListValues") = strListValues

                    End With
                    Me.TaskFieldsData = tbTaskfields
                    bolSaved = True
                End If

                Me.CanClose = bolSaved
                Me.MustRefresh = IIf(bolSaved, "1", "0")
                If bolSaved Then
                    ' Para forzar que se recargen los datos en la pantalla opciones de presencia
                    'Session("AttendanceOptions_TaskFieldsData") = Nothing
                End If

            End If

        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Select Case strButtonKey
            Case "DeleteCategory.Answer.Yes"

                'API.ReportServiceMethods.DeleteProfile(Me, Me.cmbProfiles_Value.Value)
                'Me.LoadCategories()

            Case "DeleteListValue.Answer.Yes"

        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub LoadTypes()
        'Me.ddlTypes.Items.Clear()
        'Dim oItem As ListItem
        'For Each oValue As FieldTypes In System.Enum.GetValues(GetType(FieldTypes))
        '    oItem = New ListItem(Me.Language.Translate("FieldType." & System.Enum.GetName(GetType(FieldTypes), oValue), Me.DefaultScope), oValue)
        '    Me.ddlTypes.Items.Add(oItem)
        'Next
        Me.cmbTypes.ClearItems()
        For Each oValue As FieldTypes In System.Enum.GetValues(GetType(FieldTypes))
            Me.cmbTypes.AddItem(Me.Language.Translate("FieldType." & System.Enum.GetName(GetType(FieldTypes), oValue), Me.DefaultScope), oValue, "VisibleAccessValidationControl(false);")
        Next

    End Sub

    Private Sub LoadActions()
        Me.cmbActions.ClearItems()
        For Each oValue As ActionTypes In System.Enum.GetValues(GetType(ActionTypes))
            Me.cmbActions.AddItem(Me.Language.Translate("ActionTypes." & System.Enum.GetName(GetType(ActionTypes), oValue), Me.DefaultScope), oValue, "VisibleAccessValidationControl(false);")
        Next

    End Sub

    Private Sub LoadValueTypes()
        Me.cmbValueTypes.ClearItems()
        For Each oValue As ValueTypes In System.Enum.GetValues(GetType(ValueTypes))
            Me.cmbValueTypes.AddItem(Me.Language.Translate("ValueTypes." & System.Enum.GetName(GetType(ValueTypes), oValue), Me.DefaultScope), oValue, "VisibleListValuesControl(" & IIf(oValue = ValueTypes.aList, "true", "false") & "); VisibleAccessValidationControl(false);")
        Next

    End Sub

    Private Sub UpdateData()

        Dim intTaskFieldID As String = roTypes.Any2String(Request.Params("TaskFieldID"))
        Dim fl As roTaskFieldDefinition = API.UserFieldServiceMethods.GetTaskField(Me, intTaskFieldID, False)
        Me.cmbTypes.Enabled = False
        Me.cmbActions.Enabled = oPermission >= Permission.Write
        Me.cmbValueTypes.Enabled = oPermission >= Permission.Write

        If Me.TaskFieldDataChanged Then
            With Me.TaskFieldData
                Me.txtName.Value = .Name
                Me.cmbTypes.Value = .Type
                Me.cmbTypes.SelectedValue = .Type
                Me.cmbActions.Value = .Action
                Me.cmbActions.SelectedValue = .Action
                Me.cmbValueTypes.Value = .ValueType
                Me.cmbValueTypes.SelectedValue = .ValueType
                If .ListValues IsNot Nothing Then
                    For Each value As String In .ListValues
                        Me.cmbListValues.Items.Add(value)
                    Next
                End If

            End With
        End If

        Me.lblListValues.Style.Add("display", IIf(cmbValueTypes.Value = ValueTypes.aList, "", "none"))
        Me.cmbListValues.Style.Add("display", IIf(cmbValueTypes_Value.Value = ValueTypes.aList, "", "none"))
        Me.divListActions.Style.Add("display", IIf(cmbValueTypes_Value.Value = ValueTypes.aList, "", "none"))

    End Sub

    Private Function ValidateData() As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        If Not Robotics.DataLayer.roSupport.IsXSSSafe(Me.txtName.Value) Then
            strMsg = Me.Language.Translate("Validate.XSSError", Me.DefaultScope)
            Me.txtName.Focus()
        Else
            If Me.txtName.Value.Trim = "" Then
                strMsg = Me.Language.Translate("Validate.InvalidName", Me.DefaultScope)
                Me.txtName.Focus()
            Else
                Dim oRows(-1) As DataRow
                oRows = Me.TaskFieldsData.Select("Name='" & Replace(Me.txtName.Value, "'", "''") & "' AND ID <> " & Me.TaskFieldData.ID)
                If oRows.Length > 0 Then
                    strMsg = Me.Language.Translate("Validate.FieldExist", Me.DefaultScope)
                    Me.txtName.Focus()
                End If
            End If

            If cmbValueTypes_Value.Value = ValueTypes.aList Then
                Dim _ListValues As String()
                Dim strListValues As String = ""

                ReDim _ListValues(Me.cmbListValues.Items.Count - 1)
                For n As Integer = 0 To Me.cmbListValues.Items.Count - 1
                    _ListValues(n) = Me.cmbListValues.Items(n).Text
                    strListValues = $"{strListValues}~{Me.cmbListValues.Items(n).Text}"
                Next
                If strListValues.Length = 0 OrElse Not Robotics.DataLayer.roSupport.IsXSSSafe(strListValues) Then
                    strMsg = Me.Language.Translate("Validate.ListValues", Me.DefaultScope)
                    Me.txtName.Focus()
                End If

            End If
        End If


        lblError.Text = ""
        lblError.Visible = False

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