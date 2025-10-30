Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.AccessPeriod
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class AccessGroups
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="gridGroups")>
        Public GridGroups As String

        <Runtime.Serialization.DataMember(Name:="authorized")>
        Public authorized As EmployeeStructField()()

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class AuhtorizedObject

        <Runtime.Serialization.DataMember(Name:="fields")>
        Public fields As EmployeeStructField()

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class EmployeeStructField

        <Runtime.Serialization.DataMember(Name:="field")>
        Public attname As String

        <Runtime.Serialization.DataMember(Name:="value")>
        Public value As String

    End Class

    Private oPermissionDef As Permission
    Private oPermissionAss As Permission
    Private oPermissionDoc As Permission

    Private Property OHPDocumentTemplatesData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roDocumentTemplate)
        Get

            Dim tbDocuments As Generic.List(Of roDocumentTemplate) = Session("AccessGroups_OHPDocuments")

            If bolReload Or tbDocuments Is Nothing Then
                Dim oList As New Generic.List(Of roDocumentTemplate)
                oList = DocumentsServiceMethods.GetAccessAuthorizationsTemplates(Me.Page, False)

                tbDocuments = oList
                Session("AccessGroups_OHPDocuments") = oList

            End If
            Return tbDocuments

        End Get
        Set(ByVal value As Generic.List(Of roDocumentTemplate))
            If value IsNot Nothing Then
                Session("AccessGroups_OHPDocuments") = value
            Else
                Session("AccessGroups_OHPDocuments") = Nothing
            End If
        End Set
    End Property

    Private Property AuthorizationDocumentsData() As Generic.List(Of roAuthorizationDocument)
        Get

            Dim tbDocuments As Generic.List(Of roAuthorizationDocument) = Session("AccessGroups_AssignedDocuments")

            Return tbDocuments

        End Get
        Set(ByVal value As Generic.List(Of roAuthorizationDocument))
            If value IsNot Nothing Then
                Session("AccessGroups_AssignedDocuments") = value
            Else
                Session("AccessGroups_AssignedDocuments") = Nothing
            End If
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermissionDef = Me.GetFeaturePermission("Access.Groups.Definition")
        Me.oPermissionAss = Me.GetFeaturePermission("Access.Groups.Assign")
        Me.oPermissionDoc = Me.GetFeaturePermission("Documents")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("frmNewAccPermission", "~/Access/Scripts/frmNewAccPermission.js")
        Me.InsertExtraJavascript("AccessGroup", "~/Access/Scripts/AccessGroup.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Access") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesAccessGroups.TreeCaption = Me.Language.Translate("TreeCaptionAccessGroups", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermissionDef = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermissionDef = Permission.Read AndAlso oPermissionAss.Equals(Permission.Read) Then
            DisableControls()
        End If

        If Me.oPermissionDef < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        If Me.oPermissionAss < Permission.Write Then
            hdnModeEditEmployees.Value = "true"
        Else
            hdnModeEditEmployees.Value = "false"
        End If

        If Not Me.IsPostBack Then
            Dim oList = OHPDocumentTemplatesData(True)
            CreateAccessAuthorityColumns()
        End If

        BindGridDocumentsAuthorized()

        Dim dTbl As DataTable = AccessGroupServiceMethods.GetAccessGroups(Me.Page)
        If dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean

        Dim GridsJSON As String = String.Empty
        Select Case oParameters.Action

            Case "GETACCESSGROUP"
                bRet = LoadAccessGroup(oParameters.ID)
            Case "SAVEACCESSGROUP"
                bRet = SaveAccessGroup(oParameters)
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadAccessGroup(ByRef IdAccessGroup As Integer, Optional ByVal CAccessGroup As roAccessGroup = Nothing) As Boolean
        Dim bRet As Boolean = False

        Dim ErrorInfo As String = String.Empty
        Dim GridsJSON As String = String.Empty
        Try

            If Me.oPermissionDef > Permission.None Then

                Dim oCurrentGroup As roAccessGroup

                If CAccessGroup Is Nothing Then
                    If IdAccessGroup = -1 Then
                        oCurrentGroup = New roAccessGroup
                    Else
                        oCurrentGroup = AccessGroupServiceMethods.GetAccessGroupByID(Me.Page, CInt(IdAccessGroup), True, If(HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1"), WLHelperWeb.CurrentPassportID, 0))
                    End If
                Else
                    oCurrentGroup = CAccessGroup
                End If

                txtName.Text = oCurrentGroup.Name
                txtShortName.Text = oCurrentGroup.ShortName

                GridsJSON = CreateGridsJSON(oCurrentGroup)
                If oCurrentGroup.AuthorizationDocuments IsNot Nothing Then AuthorizationDocumentsData = oCurrentGroup.AuthorizationDocuments.ToList
                BindGridDocumentsAuthorized()

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETACCESSGROUP")
            ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
            ASPxCallbackPanelContenido.JSProperties.Add("cpGridsJSON", GridsJSON)
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
            ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", ErrorInfo)
        End Try

        Return bRet

    End Function

    Private Function SaveAccessGroup(ByRef oParameters As ObjectCallbackRequest) As Boolean
        Dim bRet As Boolean = False

        Dim ErrorInfo As String = String.Empty
        Dim oCurrentGroup As roAccessGroup = Nothing
        Try

            If Me.oPermissionDef >= Permission.Write OrElse oPermissionAss >= Permission.Write Then

                'Dim oCurrentGroup As roAccessGroup = AccessGroupServiceMethods.GetAccessGroupByID(Me.Page, oParameters.ID, False)
                Dim bolIsNew As Boolean = False
                If oParameters.ID = 0 Or oParameters.ID = -1 Then bolIsNew = True

                If bolIsNew Then
                    oCurrentGroup = New roAccessGroup
                    oCurrentGroup.ID = -1
                Else
                    oCurrentGroup = AccessGroupServiceMethods.GetAccessGroupByID(Me.Page, oParameters.ID, False, If(HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1"), WLHelperWeb.CurrentPassportID, 0))
                End If

                If Not oCurrentGroup Is Nothing Then

                    oCurrentGroup.ID = oParameters.ID
                    oCurrentGroup.Name = txtName.Text
                    oCurrentGroup.ShortName = txtShortName.Text

                    Dim oArrElements() As String
                    Dim ItemInfo() As String

                    If Not String.IsNullOrEmpty(oParameters.GridGroups) Then
                        Dim resultGroups As New Generic.List(Of roAccessGroupPermission)
                        Dim oAccessGroupPermission As roAccessGroupPermission
                        oArrElements = oParameters.GridGroups.Split(";")
                        For Each item As String In oArrElements
                            ItemInfo = item.Split("#")
                            oAccessGroupPermission = New roAccessGroupPermission() With {.IDAccessGroup = ItemInfo(0), .IDZone = ItemInfo(1), .IDAccessPeriod = ItemInfo(2)}
                            If Not resultGroups.Contains(oAccessGroupPermission) Then
                                resultGroups.Add(oAccessGroupPermission)
                            End If
                        Next
                        oCurrentGroup.AccessGroupPermissions = resultGroups
                    Else
                        oCurrentGroup.AccessGroupPermissions = Nothing
                    End If

                    Dim oNewEmployeeAuthorizedList As New Generic.List(Of roEmployeeDescription)
                    Dim oNewGroupAuthorizedList As New Generic.List(Of roGroupDescription)

                    If Not oParameters.authorized Is Nothing Then
                        For Each oItem As EmployeeStructField() In oParameters.authorized

                            If oItem(4).value.ToUpper = "G" Then
                                Dim tmpGroup As New roGroupDescription
                                tmpGroup.ID = roTypes.Any2Integer(oItem(2).value.Split("_")(1))
                                tmpGroup.Name = roTypes.Any2String(oItem(3).value)

                                oNewGroupAuthorizedList.Add(tmpGroup)
                            ElseIf oItem(4).value.ToUpper = "E" Then

                                Dim tmpEmp As New roEmployeeDescription
                                tmpEmp.ID = roTypes.Any2Integer(oItem(2).value.Split("_")(1))
                                tmpEmp.Name = roTypes.Any2String(oItem(3).value)

                                oNewEmployeeAuthorizedList.Add(tmpEmp)
                            End If
                        Next
                    End If
                    oCurrentGroup.Employees = oNewEmployeeAuthorizedList
                    oCurrentGroup.Groups = oNewGroupAuthorizedList
                    If Not Me.AuthorizationDocumentsData Is Nothing Then
                        oCurrentGroup.AuthorizationDocuments = Me.AuthorizationDocumentsData
                    Else
                        oCurrentGroup.AuthorizationDocuments = New List(Of roAuthorizationDocument)
                    End If

                    bRet = AccessGroupServiceMethods.SaveAccessGroup(Me.Page, oCurrentGroup, True)
                    If bRet Then
                        Dim treePath As String = "/source/" & oCurrentGroup.ID
                        HelperWeb.roSelector_SetSelection(oCurrentGroup.ID.ToString, treePath, "ctl00_contentMainBody_roTreesAccessGroups")
                    Else
                        ErrorInfo = AccessGroupServiceMethods.LastErrorText
                    End If
                Else
                    ErrorInfo = AccessGroupServiceMethods.LastErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        Finally
            If Not bRet Then
                bRet = LoadAccessGroup(-1, oCurrentGroup)
                ASPxCallbackPanelContenido.JSProperties("cpAction") = "SAVEACCESSGROUP"
                ASPxCallbackPanelContenido.JSProperties("cpResult") = "NOK"
                ASPxCallbackPanelContenido.JSProperties("cpMessage") = ErrorInfo
            Else
                LoadAccessGroup(oCurrentGroup.ID, oCurrentGroup)
                ASPxCallbackPanelContenido.JSProperties("cpIsNew") = True
            End If
        End Try

        Return bRet

    End Function

    Private Function CreateGridsJSON(ByRef oCurrentGroup As roAccessGroup) As String

        Dim strJSONGroups As String = ""

        Try

            Dim oJGAccPerm As New Generic.List(Of Object)
            Dim oJGEmployees As New Generic.List(Of Object)
            Dim oJGGroups As New Generic.List(Of Object)

            Dim oJFAccPerm As Generic.List(Of JSONFieldItem)
            Dim oJFEmployees As Generic.List(Of JSONFieldItem)
            Dim oJFGroups As Generic.List(Of JSONFieldItem)

            If oCurrentGroup.AccessGroupPermissions IsNot Nothing Then
                If oCurrentGroup.AccessGroupPermissions.Count > 0 Then
                    For Each oAccGroupPerm As roAccessGroupPermission In oCurrentGroup.AccessGroupPermissions
                        oJFAccPerm = New Generic.List(Of JSONFieldItem)

                        oJFAccPerm.Add(New JSONFieldItem("IDAccessGroup", oAccGroupPerm.IDAccessGroup, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFAccPerm.Add(New JSONFieldItem("IDZone", oAccGroupPerm.IDZone, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Dim oZone As roZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, oAccGroupPerm.IDZone, False)
                        Dim ZoneName As String = ""
                        If oZone IsNot Nothing Then
                            ZoneName = oZone.Name
                        Else
                            ZoneName = "<not found>"
                        End If
                        oJFAccPerm.Add(New JSONFieldItem("ZoneName", ZoneName, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFAccPerm.Add(New JSONFieldItem("IDAccessPeriod", oAccGroupPerm.IDAccessPeriod, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Dim oPeriod As roAccessPeriod = AccessPeriodServiceMethods.GetAccessPeriodByID(Me.Page, oAccGroupPerm.IDAccessPeriod, False)
                        Dim PeriodName As String = ""
                        If oPeriod IsNot Nothing Then
                            PeriodName = oPeriod.Name
                        Else
                            PeriodName = "<not found>"
                        End If
                        oJFAccPerm.Add(New JSONFieldItem("PeriodName", PeriodName, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        oJGAccPerm.Add(oJFAccPerm)
                    Next
                End If
            End If

            If oCurrentGroup.Employees IsNot Nothing Then
                If oCurrentGroup.Employees.Count > 0 Then
                    For Each oEmployee As roEmployeeDescription In oCurrentGroup.Employees
                        oJFEmployees = New Generic.List(Of JSONFieldItem)
                        oJFEmployees.Add(New JSONFieldItem("icon", "<img src=""" & Me.ResolveUrl("~/Base/Images/EmployeeSelector/Empleado-16x16.gif") & " "" />", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        oJFEmployees.Add(New JSONFieldItem("idaccessgroup", oCurrentGroup.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        oJFEmployees.Add(New JSONFieldItem("idobject", "1_" & oEmployee.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        If oEmployee.Name IsNot Nothing Then
                            oJFEmployees.Add(New JSONFieldItem("name", oEmployee.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        Else
                            oJFEmployees.Add(New JSONFieldItem("name", API.EmployeeServiceMethods.GetEmployeeName(Me.Page, oEmployee.ID), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        End If

                        oJFEmployees.Add(New JSONFieldItem("type", "e", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        oJGEmployees.Add(oJFEmployees)
                    Next
                End If
            End If

            If oCurrentGroup.Groups IsNot Nothing AndAlso oCurrentGroup.Groups.Count > 0 Then
                For Each oGroups As roGroupDescription In oCurrentGroup.Groups
                    oJFGroups = New Generic.List(Of JSONFieldItem)

                    oJFGroups.Add(New JSONFieldItem("icon", "<img src=""" & Me.ResolveUrl("~/Base/Images/EmployeeSelector/Grupos-16x16.Gif") & " "" />", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJFGroups.Add(New JSONFieldItem("idaccessgroup", oCurrentGroup.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJFGroups.Add(New JSONFieldItem("idobject", "0_" & oGroups.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    If oGroups.Name IsNot Nothing Then
                        oJFGroups.Add(New JSONFieldItem("name", oGroups.Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    Else
                        oJFGroups.Add(New JSONFieldItem("name", API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, oGroups.ID, False).Name, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    End If

                    oJFGroups.Add(New JSONFieldItem("type", "g", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    oJGGroups.Add(oJFGroups)
                Next
            End If

            strJSONGroups = "{""groupperms"":["

            Dim strJSONText As String = ""
            For Each oObj As Object In oJGAccPerm
                strJSONText = "{""fields"":" & roJSONHelper.Serialize(oObj) & " },"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]},"

            strJSONGroups &= "{""employees"":["
            For Each oObj As Object In oJGEmployees
                strJSONText = "{""fields"":" & roJSONHelper.Serialize(oObj) & " },"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]},"

            strJSONGroups &= "{""groups"":["
            For Each oObj As Object In oJGGroups
                strJSONText = "{""fields"":" & roJSONHelper.Serialize(oObj) & " },"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]}"

            Return "[" & strJSONGroups & "]"
        Catch ex As Exception
            Return String.Empty
        End Try

    End Function

#Region "Grid AccessAuthority"

    Private Sub GridDocumentsAuthorized_DataBinding(sender As Object, e As EventArgs) Handles gridDocumentsAuthorized.DataBinding

        Dim oCombo As GridViewDataComboBoxColumn = Nothing

        If Me.oPermissionDoc >= Permission.Write Then
            oCombo = gridDocumentsAuthorized.Columns()(2)
        Else
            oCombo = gridDocumentsAuthorized.Columns()(1)
        End If

        oCombo.PropertiesComboBox.DataSource = Me.OHPDocumentTemplatesData
        oCombo.PropertiesComboBox.ValueField = "Id"
        oCombo.PropertiesComboBox.ValueType = GetType(Integer)
        oCombo.PropertiesComboBox.TextField = "Name"

    End Sub

    Private Sub CreateAccessAuthorityColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridComboCommand As GridViewDataComboBoxColumn

        Dim VisibleIndex As Integer = 0

        Me.gridDocumentsAuthorized.Columns.Clear()
        Me.gridDocumentsAuthorized.KeyFieldName = "ID"
        Me.gridDocumentsAuthorized.SettingsText.EmptyDataRow = " "
        Me.gridDocumentsAuthorized.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermissionDoc > Permission.Read Then
            Me.gridDocumentsAuthorized.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.gridDocumentsAuthorized.FindTitleTemplateControl("btnAddDocument")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.gridDocumentsAuthorized.Columns.Add(GridColumn)

        If Me.oPermissionDoc >= Permission.Write Then

            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

            GridColumnCommand.ShowDeleteButton = False
            GridColumnCommand.ShowEditButton = True
            GridColumnCommand.ShowCancelButton = True
            GridColumnCommand.ShowUpdateButton = False

            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 24
            VisibleIndex = VisibleIndex + 1

            Me.gridDocumentsAuthorized.Columns.Add(GridColumnCommand)

        End If

        'Combo documentos
        GridComboCommand = New GridViewDataComboBoxColumn()
        GridComboCommand.Caption = Me.Language.Translate("GridDocumentsAuthorized.Column.DocumentName", DefaultScope)
        GridComboCommand.FieldName = "ID"
        VisibleIndex = VisibleIndex + 1
        GridComboCommand.PropertiesComboBox.TextField = "Name"
        GridComboCommand.PropertiesComboBox.ValueField = "Id"
        GridComboCommand.PropertiesComboBox.ValueType = GetType(Integer)
        GridComboCommand.PropertiesComboBox.DataSource = Me.OHPDocumentTemplatesData()
        GridComboCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridComboCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridComboCommand.ReadOnly = False
        GridComboCommand.PropertiesComboBox.IncrementalFilteringMode = IncrementalFilteringMode.Contains
        Me.gridDocumentsAuthorized.Columns.Add(GridComboCommand)

        'Hora
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridDocumentsAuthorized.Column.Scope", DefaultScope)
        GridColumn.FieldName = "DocumentScope"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        'GridColumn.SortIndex = 1
        'GridColumn.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        GridColumn.Width = 200
        Me.gridDocumentsAuthorized.Columns.Add(GridColumn)

        If Me.oPermissionDoc >= Permission.Write Then
            'Command buttons
            GridColumnCommand = New GridViewCommandColumn()
            GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
            GridColumnCommand.ShowDeleteButton = True
            GridColumnCommand.ShowEditButton = False
            GridColumnCommand.ShowCancelButton = False
            GridColumnCommand.ShowUpdateButton = True
            GridColumnCommand.Caption = " "
            GridColumnCommand.VisibleIndex = VisibleIndex
            GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
            GridColumnCommand.Width = 24
            VisibleIndex = VisibleIndex + 1

            Me.gridDocumentsAuthorized.Columns.Add(GridColumnCommand)
        End If

    End Sub

    Private Sub BindGridDocumentsAuthorized()
        Me.gridDocumentsAuthorized.DataSource = Me.AuthorizationDocumentsData()
        Me.gridDocumentsAuthorized.DataBind()
    End Sub

    Protected Sub GridDocumentsAuthorized_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles gridDocumentsAuthorized.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "DocumentScope"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("ID") IsNot System.DBNull.Value Then
                        Dim tb As Generic.List(Of roDocumentTemplate) = Me.OHPDocumentTemplatesData()
                        If (tb.Count > 0) Then
                            For Each oDoc In tb
                                If oDoc.Id = roTypes.Any2Integer(e.GetListSourceFieldValue("ID")) Then
                                    e.Value = Language.Translate("GridDocumentsAuthorized.Scope." & oDoc.Scope, DefaultScope)
                                End If
                            Next

                        End If
                    End If
                End If
            Case Else

        End Select
    End Sub

    Protected Sub GridDocumentsAuthorized_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles gridDocumentsAuthorized.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridDocumentsAuthorized()
            gridDocumentsAuthorized.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridDocumentsAuthorized()
            gridDocumentsAuthorized.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridDocumentsAuthorized_RowInserting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataInsertingEventArgs) Handles gridDocumentsAuthorized.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim bFound As Boolean = False
            Dim tb As Generic.List(Of roAuthorizationDocument) = Me.AuthorizationDocumentsData()
            If (tb.Count > 0) Then
                For Each oDoc In tb
                    If oDoc.ID = roTypes.Any2Integer(e.NewValues("ID")) Then
                        bFound = True
                    End If
                Next

            End If
            If Not bFound Then
                If e.NewValues("ID") Is Nothing Then
                    Throw New Exception(Me.Language.Translate("GridDocumentsAuthorized.Error.EmptyData", Me.DefaultScope))
                End If

                Dim oNewRow As New roAuthorizationDocument
                With oNewRow
                    .ID = e.NewValues("ID")
                    For Each oTemplate In OHPDocumentTemplatesData
                        If .ID = oTemplate.Id Then .Name = oTemplate.Name
                    Next
                End With
                tb.Add(oNewRow)

                Me.AuthorizationDocumentsData = tb
                e.Cancel = True
                grid.CancelEdit()
            Else
                Throw New Exception(Me.Language.Translate("GridDocumentsAuthorized.Error.AlreadyExists", Me.DefaultScope))
            End If
        End If
    End Sub

    Protected Sub GridDocumentsAuthorized_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs) Handles gridDocumentsAuthorized.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridDocumentsAuthorized_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles gridDocumentsAuthorized.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roAuthorizationDocument) = Me.AuthorizationDocumentsData()

        If e.Values("ID") > 0 Then
            Dim selObject As roAuthorizationDocument = Nothing
            For Each oObject As roAuthorizationDocument In tb
                If oObject.ID = e.Values("ID") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Me.AuthorizationDocumentsData = tb
            e.Cancel = True

        End If

        BindGridDocumentsAuthorized()
        gridDocumentsAuthorized.JSProperties("cpAction") = "ROWDELETE"

    End Sub

#End Region

End Class