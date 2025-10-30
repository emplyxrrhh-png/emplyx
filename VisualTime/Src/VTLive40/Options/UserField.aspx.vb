Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class UserField
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.UserFields.Definition"

#Region "Declarations"

    Private oPermission As Permission = Permission.None

    Private oType As Types = Types.EmployeeField

    Private bolOHPLicense As Boolean = False
    Private bolIsLivePortal As Boolean = False

#End Region

#Region "Properties"

    Private ReadOnly Property IsNew() As Boolean
        Get
            Return roTypes.Any2String(Request.Params("UserFieldName")) = ""
        End Get

    End Property

    Private Property UserFieldData() As roUserField
        Get
            Dim oData As roUserField = Session("UserField_UserFieldData")
            If oData Is Nothing Then

                Dim strUserFieldName As String = Request.Params("UserFieldName")
                If strUserFieldName Is Nothing Then strUserFieldName = ""

                oData = New roUserField
                oData.Type = Me.oType
                If strUserFieldName <> "" Then
                    Dim oRows() As DataRow = Me.UserFieldsData.Select("FieldName = '" & strUserFieldName & "' AND Type = " & oType)
                    If oRows.Length = 1 Then
                        With oData
                            .FieldName = oRows(0).Item("FieldName")
                            If Not IsDBNull(oRows(0).Item("OriginalFieldName")) Then
                                .OriginalFieldName = oRows(0).Item("OriginalFieldName")
                            End If
                            .FieldType = oRows(0).Item("FieldType")
                            .Used = oRows(0).Item("Used")
                            .Unique = oRows(0).Item("Unique")
                            .Id = oRows(0).Item("Id")
                            .AccessLevel = IIf(Not IsDBNull(oRows(0).Item("AccessLevel")), oRows(0).Item("AccessLevel"), AccessLevels.aMedium)
                            .Category = roTypes.Any2String(oRows(0).Item("Category"))
                            Dim _ListValues As New Generic.List(Of String)
                            If roTypes.Any2String(oRows(0).Item("ListValues")) <> "" Then
                                For Each value As String In roTypes.Any2String(oRows(0).Item("ListValues")).Split("~")
                                    _ListValues.Add(value)
                                Next
                                _ListValues.Sort()
                                .ListValues = _ListValues
                            End If
                            .Description = roTypes.Any2String(oRows(0).Item("Description"))
                            .AccessValidation = roTypes.Any2Integer(oRows(0).Item("AccessValidation"))
                            .History = roTypes.Any2Boolean(oRows(0).Item("History"))
                            If oRows(0).Table.Columns.Contains("UsedInProcess") Then
                                .InProcess = roTypes.Any2Boolean(oRows(0).Item("UsedInProcess"))
                            End If
                            If oRows(0).Table.Columns.Contains("UsedInTimeGate") Then
                                .InTimeGate = roTypes.Any2Boolean(oRows(0).Item("UsedInTimeGate"))
                            End If
                            .RequestPermissions = roTypes.Any2Integer(oRows(0).Item("RequestPermissions"))
                            .RequestConditions = API.UserFieldServiceMethods.LoadUserFieldConditionsFromXml(Me, roTypes.Any2String(oRows(0).Item("RequestCriteria"))).ToList
                            .AliasFieldName = roTypes.Any2String(oRows(0).Item("Alias"))
                            .isSystem = roTypes.Any2Boolean(oRows(0).Item("isSystem"))
                        End With
                    End If
                End If

                Session("UserField_UserFieldData") = oData

            End If
            Return oData
        End Get
        Set(ByVal value As roUserField)
            Session("UserField_UserFieldData") = value
        End Set
    End Property

    Private ReadOnly Property UserFieldDataChanged() As Boolean
        Get
            Dim oData As roUserField = Session("UserField_UserFieldData")
            Return (oData Is Nothing)
        End Get
    End Property

    Private Property UserFieldsData() As DataTable
        Get
            Dim tb As DataTable = Nothing
            Select Case Me.oType
                Case Types.EmployeeField
                    tb = Session("ConfigurationOptions_EmployeeUserFieldsData")
                Case Types.GroupField
                    tb = Session("ConfigurationOptions_GroupUserFieldsData")
            End Select
            Return tb
        End Get
        Set(ByVal value As DataTable)
            Select Case Me.oType
                Case Types.EmployeeField
                    Session("ConfigurationOptions_EmployeeUserFieldsData") = value
                Case Types.GroupField
                    Session("ConfigurationOptions_GroupUserFieldsData") = value
            End Select
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

        ' Si el passport actual no tiene permisso de lectura, redirigimos a p·gina de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        ' Obtenemos la licencia para prevensiÛn de riesgos laborales
        Me.bolOHPLicense = API.LicenseServiceMethods.FeatureIsInstalled("Feature\OHP")

        Me.bolIsLivePortal = API.LicenseServiceMethods.FeatureIsInstalled("Feature\LivePortal")

        If Request.Params("Type") IsNot Nothing Then
            Select Case Request.Params("Type")
                Case "0" : Me.oType = Types.EmployeeField
                Case "1" : Me.oType = Types.GroupField
            End Select
        End If

        Me.LoadTypes()
        Me.LoadAccessLevels()

        Me.LoadCategories()
        Me.LoadAccessValidations()

        If Not Me.IsPostBack Then

            Me.UserFieldData = Nothing

        End If

        Me.UpdateData()

        If Not Me.IsPostBack Then
            Me.SetPermissions()
        End If

        Me.LoadSystemConvert(Me.UserFieldData.FieldType)

        Me.chkHistory.Disabled = (Me.oType = Types.GroupField)

        If Not Me.IsNew AndAlso Me.UserFieldData.FieldType = FieldTypes.tDocument Then
            Me.chkHistory.Disabled = True
        End If

        Dim tgUserField As Integer = -1
        Dim parameter As roAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Me.Page, "Timegate.Identification.CustomUserFieldId")
        If parameter IsNot Nothing AndAlso parameter.Value.Trim.Length > 0 Then
            Dim timegateConfiguration As TimegateConfiguration
            timegateConfiguration = roJSONHelper.DeserializeNewtonSoft(parameter.Value, GetType(TimegateConfiguration))
            If timegateConfiguration IsNot Nothing AndAlso timegateConfiguration.CustomUserFieldEnabled Then
                tgUserField = timegateConfiguration.UserFieldId
            End If
        End If

        If Not Me.IsNew AndAlso (Me.UserFieldData.AliasFieldName = "sysroVisualtimeID" OrElse Me.UserFieldData.Id = tgUserField) Then
            Me.chkUnique.Disabled = True
        End If

        Me.lblHistory.Enabled = (Me.oType <> Types.GroupField)

        Me.tabCtl01.TabVisible(2) = (Me.oType = Types.EmployeeField) AndAlso Me.bolIsLivePortal AndAlso Me.UserFieldData.FieldType <> FieldTypes.tDocument

        If Me.UserFieldData.isSystem Then
            Me.categoriesTR.Style("display") = ""
            Me.descriptionTR.Style("display") = "none"
            Me.historyTR.Style("display") = "none"
            Me.txtName.Attributes.Add("readonly", "readonly")
        Else
            Me.categoriesTR.Style("display") = ""
            Me.descriptionTR.Style("display") = ""
            Me.historyTR.Style("display") = ""
        End If
        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Me.oPermission >= Permission.Write Then

            Me.txtName.Value = Me.txtName.Value.Replace("'", "¥")

            If Me.ValidateData() Then

                Dim _ListValues() As String
                Dim strListValues As String = ""

                Dim newAlias = ""
                Dim accessLevel = Nothing
                Dim category = ""

                Select Case Me.cbConvert_Value.Value
                    Case 0
                        newAlias = "sysroMobile"
                        accessLevel = AccessLevels.aLow
                    Case 1
                        newAlias = "sysroGender"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 2
                        newAlias = "sysroProfessionalCategory"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 3
                        newAlias = "sysroQuoteGroup"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 4
                        newAlias = "sysroPosition"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aMedium
                    Case 5
                        newAlias = "sysroTotalSalary"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 6
                        newAlias = "sysroBaseSalary"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 7
                        newAlias = "sysroSalarySupp"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 8
                        newAlias = "sysroExtraSalary"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 9
                        newAlias = "sysroEarningsOverTime"
                        category = "Igualdad"
                        accessLevel = AccessLevels.aHigh
                    Case 10
                        newAlias = "sysroBirthdate"
                        category = "Personal"
                        accessLevel = AccessLevels.aLow
                    Case Else
                        newAlias = ""
                        accessLevel = Nothing
                End Select

                With Me.UserFieldData
                    .FieldName = Me.txtName.Value
                    .Type = Me.oType
                    If .OriginalFieldName = "" Then .OriginalFieldName = .FieldName
                    '.FieldType = Me.ddlTypes.SelectedValue
                    '.AccessLevel = Me.ddlAccessLevels.SelectedValue
                    .FieldType = Me.cmbTypes_Value.Value
                    If accessLevel IsNot Nothing Then
                        .AccessLevel = accessLevel
                    Else
                        .AccessLevel = Me.cmbAccessLevels_Value.Value
                    End If
                    .Category = Me.cmbCategories_Value.Value
                    ReDim _ListValues(Me.cmbListValues.Items.Count - 1)
                    For n As Integer = 0 To Me.cmbListValues.Items.Count - 1
                        _ListValues(n) = Me.cmbListValues.Items(n).Text
                        strListValues &= "~" & Me.cmbListValues.Items(n).Text
                    Next
                    If strListValues.Length > 0 Then strListValues = strListValues.Substring(1)
                    .ListValues = _ListValues.ToList
                    .Description = Me.txtDescription.Value
                    .AccessValidation = Me.cmbAccessValidation_Value.Value
                    .History = Me.chkHistory.Checked
                    .Unique = Me.chkUnique.Checked
                    If Me.cbConvert_Value.Value <> "" Then
                        .ConvertTo = Me.cbConvert_Value.Value
                    End If
                    If Me.optRequestAll.Checked Then
                        .RequestPermissions = 0
                    ElseIf Me.optRequestNobody.Checked Then
                        .RequestPermissions = 1
                        'ElseIf Me.optRequestCriteria.Checked Then
                        '    .RequestPermissions = 2
                    End If
                End With

                Dim bolSaved As Boolean = False

                Dim tbUserfields As DataTable = Me.UserFieldsData

                If tbUserfields IsNot Nothing Then

                    If newAlias <> "" Then
                        Dim oRowsToUpdate() As DataRow = tbUserfields.Select("Alias = '" & newAlias & "'")
                        Dim oUserFieldRowToUpdate = oRowsToUpdate(0)

                        With Me.UserFieldData
                            oUserFieldRowToUpdate("Alias") = ""
                            oUserFieldRowToUpdate("IsSystem") = 0
                            oUserFieldRowToUpdate("Used") = False
                        End With

                        If oRowsToUpdate.Length = 0 Then tbUserfields.Rows.Add(oUserFieldRowToUpdate)
                        Me.UserFieldsData = tbUserfields

                    End If

                    Dim oRows() As DataRow = tbUserfields.Select("OriginalFieldName = '" & Me.UserFieldData.OriginalFieldName & "'")
                    Dim oUserFieldRow As DataRow
                    If oRows.Length = 0 Then
                        oUserFieldRow = tbUserfields.NewRow
                    Else
                        oUserFieldRow = oRows(0)
                    End If
                    With Me.UserFieldData
                        oUserFieldRow("FieldName") = .FieldName
                        oUserFieldRow("Type") = .Type
                        If oRows.Length = 0 Then oUserFieldRow("OriginalFieldName") = .FieldName
                        oUserFieldRow("FieldType") = .FieldType
                        oUserFieldRow("Used") = .Used
                        oUserFieldRow("AccessLevel") = .AccessLevel
                        oUserFieldRow("Category") = .Category
                        oUserFieldRow("ListValues") = strListValues
                        oUserFieldRow("Description") = .Description
                        oUserFieldRow("AccessValidation") = .AccessValidation
                        oUserFieldRow("History") = .History
                        oUserFieldRow("Unique") = .Unique
                        oUserFieldRow("RequestPermissions") = .RequestPermissions

                        If IsDBNull(oUserFieldRow("isSystem")) Then oUserFieldRow("isSystem") = False

                        If newAlias <> "" Then
                            oUserFieldRow("Alias") = newAlias
                            oUserFieldRow("IsSystem") = 1
                            oUserFieldRow("Used") = True
                            oUserFieldRow("AccessLevel") = accessLevel
                        End If

                        If .RequestConditions IsNot Nothing Then
                            oUserFieldRow("RequestCriteria") = API.UserFieldServiceMethods.GetUserFieldConditionsXml(Me, .RequestConditions.ToList)
                        Else
                            oUserFieldRow("RequestCriteria") = ""
                        End If
                    End With
                    If oRows.Length = 0 Then tbUserfields.Rows.Add(oUserFieldRow)
                    Me.UserFieldsData = tbUserfields
                    bolSaved = True
                Else
                    bolSaved = API.UserFieldServiceMethods.SaveUserField(Me, Me.UserFieldData, True)
                End If

                Me.CanClose = bolSaved
                Me.MustRefresh = IIf(bolSaved, "1", "0")
                If bolSaved Then
                    ' Para forzar que se recargen los datos en la pantalla opciones de presencia
                    'Session("AttendanceOptions_UserFieldsData") = Nothing
                End If

            End If

        End If

    End Sub

    Protected Sub btNewCategoryOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNewCategoryOK.Click
        If Me.txtNewCategory.Text.Trim <> "" Then

            Me.cmbCategories.AddItem(Me.txtNewCategory.Text.Trim, Me.txtNewCategory.Text.Trim, "")
            Me.cmbCategories.SelectedValue = Me.txtNewCategory.Text.Trim

            Me.txtNewCategory.Text = ""
        Else
            HelperWeb.ShowMessage(Me, "", Me.Language.Translate("NewCategory.InvalidName", Me.DefaultScope))
        End If

    End Sub

    Protected Sub btDeleteCategory_Click(ByVal sender As Object, ByVal e As System.EventArgs)

        If Me.cmbCategories.SelectedIndex >= 0 Then

            Dim Param As New Generic.List(Of String)
            Param.Add(Me.cmbCategories_Text.Value)

            HelperWeb.ShowOptionMessage(Me, Me.Language.Translate("DeleteCategory.Title", Me.DefaultScope), Me.Language.Translate("DeleteCategory.Description", Me.DefaultScope, Param),
                                            Me.Language.Translate("DeleteCategory.Answer.Yes.Title", Me.DefaultScope), "DeleteCategory.Answer.Yes", Me.Language.Translate("DeleteCategory.Answer.Yes.Description", Me.DefaultScope), "", True, True,
                                            Me.Language.Translate("DeleteCategory.Answer.No.Title", Me.DefaultScope), "DeleteCategory.Answer.No", Me.Language.Translate("DeleteCategory.Answer.No.Description", Me.DefaultScope), "", True, True,
                                            "", "", "", "", True, True, "", "", "", "", True, True, "")

        End If

    End Sub

    Protected Sub btNewListValueOK_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNewListValueOK.Click

        If Me.txtNewListValue.Text.Trim <> "" Then

            Dim lstValues As New Generic.List(Of String)

            For Each oItem As ListItem In Me.cmbListValues.Items
                lstValues.Add(oItem.Text)
            Next

            If lstValues.IndexOf(Me.txtNewListValue.Text) < 0 Then
                lstValues.Add(Me.txtNewListValue.Text)
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

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Select Case strButtonKey
            Case "DeleteCategory.Answer.Yes"

                'API.ReportServiceMethods.DeleteProfile(Me, Me.cmbProfiles_Value.Value)
                Me.LoadCategories()

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
            If Not Me.IsNew OrElse (Me.IsNew AndAlso oValue <> FieldTypes.tDocument) Then
                Me.cmbTypes.AddItem(Me.Language.Translate("FieldType." & System.Enum.GetName(GetType(FieldTypes), oValue), Me.DefaultScope), oValue, "VisibleListValuesControl(" & IIf(oValue = FieldTypes.tList, "true", "false") & "); VisibleAccessValidationControl(" & IIf(Me.bolOHPLicense And oValue = FieldTypes.tDatePeriod, "true", "false") & ");")
            End If
        Next

    End Sub

    Private Sub LoadAccessLevels()
        'Me.ddlAccessLevels.Items.Clear()
        'Dim oItem As ListItem
        'For Each oValue As AccessLevels In System.Enum.GetValues(GetType(AccessLevels))
        '    oItem = New ListItem(Me.Language.Translate("AccessLevel." & System.Enum.GetName(GetType(AccessLevels), oValue), Me.DefaultScope), oValue)
        '    Me.ddlAccessLevels.Items.Add(oItem)
        'Next

        Me.cmbAccessLevels.ClearItems()
        For Each oValue As AccessLevels In System.Enum.GetValues(GetType(AccessLevels))
            Me.cmbAccessLevels.AddItem(Me.Language.Translate("AccessLevel." & System.Enum.GetName(GetType(AccessLevels), oValue), Me.DefaultScope), oValue, "")
        Next
    End Sub

    Private Sub LoadSystemConvert(fieldType)
        Me.cbConvertType.ClearItems()

        Dim fieldValue As FieldTypes = fieldType

        If Me.UserFieldData.Used AndAlso Not Me.UserFieldData.isSystem AndAlso Me.UserFieldData.FieldType <> FieldTypes.tDocument AndAlso Me.oType = Types.EmployeeField Then
            Me.tabCtl01.TabVisible(3) = True
        End If

        Select Case fieldValue
            Case FieldTypes.tDecimal
                Me.cbConvertType.AddItem("Salario total anual", 5, "")
                Me.cbConvertType.AddItem("Salario base anual", 6, "")
                Me.cbConvertType.AddItem("Complementos salariales anuales", 7, "")
                Me.cbConvertType.AddItem("Percepciones extrasalariales", 8, "")
                Me.cbConvertType.AddItem("Percepciones por extrordinarias y complementarias", 9, "")
            Case FieldTypes.tText
                Me.cbConvertType.AddItem("MÛvil", 0, "")
                Me.cbConvertType.AddItem("Puesto", 4, "")
            Case FieldTypes.tList
                Me.cbConvertType.AddItem("GÈnero", 1, "")
                Me.cbConvertType.AddItem("CategorÌa profesional", 2, "")
                Me.cbConvertType.AddItem("Grupo cotizaciÛn", 3, "")
            Case FieldTypes.tDate
                Me.cbConvertType.AddItem("Fecha de nacimiento", 10, "")
            Case Else
                Me.tabCtl01.TabVisible(3) = False

        End Select

    End Sub

    Private Sub LoadCategories()

        Me.cmbCategories.ClearItems()
        Dim Rows() As DataRow = Me.UserFieldsData.Select("Category IS NOT NULL", "Category")
        Dim strLastCategory As String = ""
        For Each Row As DataRow In Rows
            If roTypes.Any2String(Row("Category")) <> "" Then
                If roTypes.Any2String(Row("Category")) <> strLastCategory Then
                    strLastCategory = roTypes.Any2String(Row("Category"))
                    Me.cmbCategories.AddItem(roTypes.Any2String(Row("Category")), roTypes.Any2String(Row("Category")), "")
                End If
            End If
        Next

    End Sub

    Private Sub LoadAccessValidations()

        Me.cmbAccessValidation.ClearItems()
        For Each oValue As AccessValidation In System.Enum.GetValues(GetType(AccessValidation))
            Me.cmbAccessValidation.AddItem(Me.Language.Translate("AccessValidation." & System.Enum.GetName(oValue.GetType, oValue), Me.DefaultScope), oValue, "")
        Next
    End Sub

    Private Sub UpdateData()

        Dim strUserFieldName As String = roTypes.Any2String(Request.Params("UserFieldName"))
        Dim fl As roUserField = API.UserFieldServiceMethods.GetUserField(Me, strUserFieldName, Types.EmployeeField, True, False)
        Me.cmbTypes.Enabled = (Not fl.Used AndAlso oPermission >= Permission.Write)

        If Me.UserFieldDataChanged Then
            With Me.UserFieldData
                Me.txtName.Value = .FieldName
                If .InProcess OrElse .InTimeGate Then
                    Me.txtName.Attributes.Add("readonly", "readonly")
                End If
                Me.imgUsed.Visible = (.InProcess OrElse .InTimeGate)
                If .InProcess Then Me.imgUsed.Attributes("Title") = Me.Language.Translate("UserfieldUsedInProcess", Me.DefaultScope)
                If .InTimeGate Then Me.imgUsed.Attributes("Title") = Me.Language.Translate("UserfieldUsedInTimeGate", Me.DefaultScope)
                Me.cmbTypes.Value = .FieldType
                Me.cmbTypes.SelectedValue = .FieldType
                Me.cmbAccessLevels.Value = .AccessLevel
                Me.cmbAccessLevels.SelectedValue = .AccessLevel
                Me.lblType.Enabled = Not .Used

                Me.cmbCategories.Value = .Category
                Me.cmbCategories.SelectedValue = .Category
                If .ListValues IsNot Nothing Then
                    For Each value As String In .ListValues
                        Me.cmbListValues.Items.Add(value)
                    Next
                End If
                Me.txtDescription.Value = .Description
                Me.cmbAccessValidation.Value = .AccessValidation
                Me.cmbAccessValidation.SelectedValue = .AccessValidation
                Me.chkHistory.Checked = .History
                Me.chkUnique.Checked = .Unique
                Me.optRequestAll.Checked = (.RequestPermissions = 0)
                Me.optRequestNobody.Checked = (.RequestPermissions = 1)
                'Me.optRequestCriteria.Checked = (.RequestPermissions = 2)
                If .RequestPermissions = 2 Then
                    If .RequestConditions IsNot Nothing AndAlso .RequestConditions.Count > 0 Then
                        Me.usrRequestCriteria.loadValuesCriteria(.RequestConditions(0))
                    End If
                End If
            End With
        End If

        Me.lblListValues.Style.Add("display", IIf(cmbTypes_Value.Value = FieldTypes.tList, "", "none"))
        Me.cmbListValues.Style.Add("display", IIf(cmbTypes_Value.Value = FieldTypes.tList, "", "none"))
        Me.divListActions.Style.Add("display", IIf(cmbTypes_Value.Value = FieldTypes.tList, "", "none"))

        Me.lblAccessValidation.Style.Add("display", IIf(Me.bolOHPLicense AndAlso cmbTypes_Value.Value = FieldTypes.tDatePeriod, "", "none"))
        Me.cmbAccessValidation.Style.Add("display", IIf(Me.bolOHPLicense AndAlso cmbTypes_Value.Value = FieldTypes.tDatePeriod, "", "none"))

    End Sub

    Private Function ValidateData() As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        If Not Robotics.DataLayer.roSupport.IsXSSSafe(Me.txtName.Value) OrElse Not Robotics.DataLayer.roSupport.IsXSSSafe(Me.txtDescription.Value) Then
            strMsg = Me.Language.Translate("Validate.XSSError", Me.DefaultScope)
            Me.txtName.Focus()
        Else
            ' Comprueba que no haya caracteres invalidos
            Dim _reg As Regex = New Regex("^[A-Za-z0-9 \-_'.,#*/+%&$ø?°!()\[\]{}=;:Á«Ò—·ÈÌÛ˙‡ËÏÚ˘¸Ô]+$", RegexOptions.IgnoreCase)
            Dim m As Match = _reg.Match(Me.txtName.Value.ToLower.Trim)
            bolRet = m.Success

            If Not bolRet Then
                strMsg = Me.Language.Translate("Validate.InvalidName", Me.DefaultScope)
                Me.txtName.Focus()
            Else

                If Me.txtName.Value.Trim = "" Then
                    strMsg = Me.Language.Translate("Validate.InvalidName", Me.DefaultScope)
                    Me.txtName.Focus()
                Else

                    If (Not Me.chkUnique.Checked) OrElse (Me.chkUnique.Checked AndAlso (roTypes.Any2Integer(Me.cmbTypes_Value.Value) = CInt(FieldTypes.tText) OrElse roTypes.Any2Integer(Me.cmbTypes_Value.Value) = CInt(FieldTypes.tNumeric))) Then
                        If Me.UserFieldData.Id < 0 OrElse (Not Me.chkUnique.Checked) OrElse (Me.chkUnique.Checked AndAlso API.UserFieldServiceMethods.CanUserFieldApplyUniqueConstraint(Me.Page, Me.UserFieldData.FieldName, Me.UserFieldData.Id, False)) Then
                            Dim oRows(-1) As DataRow
                            If Me.UserFieldData.FieldName = "" Then
                                'oRows = Me.UserFieldsData.Select("FieldName='" & Me.txtName.Value & "' AND OriginalFieldName <> '" & Me.UserFieldData.OriginalFieldName & "'")
                                oRows = Me.UserFieldsData.Select(String.Format("FieldName='{0}'", Me.txtName.Value.Replace("'", "¥")) & " AND OriginalFieldName <> '" & Me.UserFieldData.OriginalFieldName & "'")
                            Else
                                'oRows = Me.UserFieldsData.Select("FieldName='" & Me.txtName.Value & "' AND OriginalFieldName <> '" & Me.UserFieldData.FieldName & "'")
                                oRows = Me.UserFieldsData.Select(String.Format("FieldName='{0}'", Me.txtName.Value.Replace("'", "¥")) & " AND OriginalFieldName <> '" & Me.UserFieldData.FieldName & "'")
                            End If
                            If oRows.Length > 0 Then
                                strMsg = Me.Language.Translate("Validate.FieldExist", Me.DefaultScope)
                                Me.txtName.Focus()
                            End If
                        Else
                            strMsg = Me.Language.Translate("Validate.UserFieldUniqueConstraint", Me.DefaultScope)
                        End If
                    Else
                        strMsg = Me.Language.Translate("Validate.UserFieldUniqueTypeConstraint", Me.DefaultScope)
                    End If





                End If

                If strMsg = "" AndAlso Me.bolOHPLicense AndAlso Me.cmbTypes_Value.Value = FieldTypes.tDatePeriod AndAlso Me.cmbAccessValidation_Value.Value = "" Then
                    strMsg = Me.Language.Translate("Validate.AccessValidationRequired", Me.DefaultScope)
                    Me.cmbAccessValidation.Focus()
                End If
            End If
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

            Me.divAddCategory.Visible = False
            Me.divListActions.Visible = False

            Me.btAccept.Style("display") = "none"
            Me.btCancel.Text = Me.Language.Keyword("Button.Close")

        End If

    End Sub

#End Region

End Class