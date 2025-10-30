Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Tasks_BusinessCenters
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class CallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="oType")>
        Public Type As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="resultClientAction")>
        Public resultClientAction As String

    End Class

    Private Const FeatureAlias As String = "BusinessCenters.Definition"
    Private oPermission As Permission

    Private Property ZonesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("BusinessCenters_Zones_Data")
            Dim tbValues As Generic.List(Of roBusinessCenterZone) = Session("BusinessCenters_ZonesValues")

            If bolReload OrElse tb Is Nothing Then

                Dim oList As New Generic.List(Of roBusinessCenterZone)

                tb = API.TasksServiceMethods.GetBusinessCenterZones(Me.Page, Me.IDCenter)

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("IDZone")}
                    tb.AcceptChanges()

                    For Each oRow As DataRow In tb.Rows
                        Dim oCurrentZone As New roBusinessCenterZone
                        oCurrentZone.ID = oRow("IDZone")
                        oList.Add(oCurrentZone)
                    Next
                End If

                tbValues = oList
                Session("BusinessCenters_ZonesValues") = tbValues

                Session("BusinessCenters_Zones_Data") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("BusinessCenters_Zones_Data") = value
            Dim tbValues As Generic.List(Of roBusinessCenterZone) = Nothing

            Dim oList As New Generic.List(Of roBusinessCenterZone)
            If value IsNot Nothing Then

                For Each oRow As DataRow In value.Rows
                    Dim oCurrentZone As New roBusinessCenterZone
                    oCurrentZone.ID = oRow("IDZone")
                    oList.Add(oCurrentZone)
                Next
            End If

            tbValues = oList
            Session("BusinessCenters_ZonesValues") = tbValues

        End Set
    End Property

    Private Property IDCenter() As Integer
        Get
            Return ViewState("BusinessCenters_IDCenter")
        End Get
        Set(ByVal value As Integer)
            ViewState("BusinessCenters_IDCenter") = value
        End Set
    End Property

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("BusinessCentersV2", "~/Tasks/Scripts/BusinessCentersV2.js")
        InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        InsertExtraJavascript("frmFilterBusinessCenters", "~/Tasks/Scripts/frmFilterBusinessCenters.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        'If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") = False And HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl") = False Then
        '    WLHelperWeb.RedirectAccessDenied(False)
        '    Exit Sub
        'End If

        roTreesBusinessCenters.TreeCaption = Me.Language.Translate("TreeCaptionBusinessCenters", Me.DefaultScope)

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

        Dim dTbl As DataTable = API.TasksServiceMethods.GetBusinessCenters(Me, False)
        If dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        CreateColumnsZones()

    End Sub

    Protected Sub btnAddNewZone_Init(ByVal sender As Object, ByVal e As EventArgs)
        Dim txtLabel As ASPxButton = CType(sender, ASPxButton)
        txtLabel.Text = Me.Language.Translate("Button.Title.NewZone", DefaultScope)
        txtLabel.ToolTip = ""
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New CallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then
            If oParameters.Action = "GETBUSINESSCENTER" Then
                LoadBusinessCenter(oParameters, False)
            ElseIf oParameters.Action = "SAVEBUSINESSCENTER" Then
                saveBusinessCenter(oParameters)

            ElseIf oParameters.Action = "EDITGRID" Then
                LoadBusinessCenter(oParameters, True)
            ElseIf oParameters.Action = "SAVEBUSINESSCENTERFIELDS" Then
                Dim strError = saveBusinessCenterFieldsGrid(oParameters)
                If strError <> "" Then
                Else
                    LoadBusinessCenter(oParameters, False)
                End If

            End If
        End If

        Me.div00.Style("display") = "none"
        Me.div01.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div00.Style("display") = ""
            Case 1
                Me.div01.Style("display") = ""
        End Select

    End Sub

    Private Sub LoadBusinessCenter(ByVal oParameters As CallbackRequest, ByVal isEdit As Boolean, Optional ByVal eBusinessCenter As roBusinessCenter = Nothing)
        If Me.oPermission < Permission.Write Then
            Me.DisableControls()
            Me.opActive.Enabled = False
            Me.opNoActive.Enabled = False
            Me.opAllZones.Enabled = False
            Me.opSelectedZones.Enabled = False
            Me.GridZones.Enabled = False
        End If

        Dim result As String = "OK"
        Dim oCurrentBusinessCenter As roBusinessCenter = Nothing
        Try
            If eBusinessCenter IsNot Nothing Then
                oCurrentBusinessCenter = eBusinessCenter
            Else
                If oParameters.ID = -1 Then
                    oCurrentBusinessCenter = New roBusinessCenter
                Else
                    oCurrentBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, oParameters.ID, True)
                End If
            End If

            If oCurrentBusinessCenter Is Nothing Then Exit Sub

            Dim oJSONFields As New Generic.List(Of JSONFieldItem)

            If oCurrentBusinessCenter Is Nothing Then Exit Sub

            Me.IDCenter = oCurrentBusinessCenter.ID

            Me.txtName.Text = oCurrentBusinessCenter.Name
            Me.txtDescription.Value = oCurrentBusinessCenter.Description

            Me.opActive.Checked = False
            Me.opNoActive.Checked = False

            If oCurrentBusinessCenter.Status = 1 Then
                Me.opActive.Checked = True
            Else
                Me.opNoActive.Checked = True
            End If

            Me.opAllZones.Checked = False
            Me.opSelectedZones.Checked = False

            If oCurrentBusinessCenter.AuthorizationMode = 0 Then
                Me.opAllZones.Checked = True
            Else
                Me.opSelectedZones.Checked = True
            End If

            BindGridZones(True)

            If isEdit = True Then
                LoadBusinessCenterGridEdit(oCurrentBusinessCenter)
            Else

                Dim Columns() As String = {"FieldCaption", "Value"}
                Dim htmlTGrid As HtmlTable = creaFieldsGrid(oCurrentBusinessCenter, Columns)
                Me.divBusinessCenterFields.Controls.Add(htmlTGrid)

                'Carrega el anchor de editar el grid
                Me.editFieldsGrid.Attributes("onclick") = "editBusinessCenterFieldsGrid('" & oParameters.ID & "')"
                Me.saveFieldsGrid.Attributes("onclick") = "saveBusinessCenterFieldsGrid('" & oParameters.ID & "')"
                Me.cancelFieldsGrid.Attributes("onclick") = "cancelBusinessCenterFieldsGrid('" & oParameters.ID & "')"

                Me.btn1FieldsBusinessCenter.Visible = True
                Me.btnAddNewZone.Visible = True
                If oParameters.ID = -1 Then
                    Me.btn1FieldsBusinessCenter.Visible = False
                    Me.opActive.Checked = True
                    Me.opNoActive.Checked = False
                    Me.btnAddNewZone.Visible = False
                End If

                Me.btn2FieldsBusinessCenter.Visible = False
                Me.btn3FieldsBusinessCenter.Visible = False

            End If

            If Me.oPermission < Permission.Write Then
                Me.btn1FieldsBusinessCenter.Visible = False
                Me.btnAddNewZone.Visible = False
            End If
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETBUSINESSCENTER")
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentBusinessCenter.Name)
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
        End Try

    End Sub

    Private Sub LoadBusinessCenterGridEdit(ByVal oBusinessCenter As roBusinessCenter)

        Try
            If oBusinessCenter Is Nothing Then Exit Sub

            'Carrega Grid Usuaris

            Dim Columns() As String = {"FieldCaption", "Value"}
            Dim htmlTGrid As HtmlTable = creaFieldsGrid(oBusinessCenter, Columns, True)
            Me.divBusinessCenterFields.Controls.Add(htmlTGrid)

            'Carrega el anchor de editar el grid
            Me.btn1FieldsBusinessCenter.Visible = False
            Me.btn2FieldsBusinessCenter.Visible = True
            Me.btn3FieldsBusinessCenter.Visible = True
            Me.editFieldsGrid.Attributes("onclick") = "editBusinessCenterFieldsGrid('" & oBusinessCenter.ID & "')"
            Me.saveFieldsGrid.Attributes("onclick") = "saveBusinessCenterFieldsGrid('" & oBusinessCenter.ID & "')"
            Me.cancelFieldsGrid.Attributes("onclick") = "cancelBusinessCenterFieldsGrid('" & oBusinessCenter.ID & "')"
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try

    End Sub

    Private Sub saveBusinessCenter(ByVal oParameters As CallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentBusinessCenter As roBusinessCenter = Nothing

        Try

            'Check Permissions
            If Me.oPermission < Permission.Write Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            End If

            Dim bolIsNew As Boolean = False
            If oParameters.ID < 1 Then bolIsNew = True

            If Me.oPermission = Permission.Write And bolIsNew Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            End If

            If rError.Error = False Then

                If bolIsNew Then
                    oCurrentBusinessCenter = New roBusinessCenter
                    oCurrentBusinessCenter.ID = -1
                Else
                    oCurrentBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, oParameters.ID, False)
                End If

                If oCurrentBusinessCenter Is Nothing Then Exit Sub

                oCurrentBusinessCenter.Name = txtName.Text
                oCurrentBusinessCenter.Description = txtDescription.Text
                oCurrentBusinessCenter.Status = IIf(opActive.Checked = True, 1, 0)
                oCurrentBusinessCenter.AuthorizationMode = IIf(opAllZones.Checked = True, 0, 1)
                oCurrentBusinessCenter.Zones = Nothing

                If oCurrentBusinessCenter.AuthorizationMode = 1 Then
                    Dim tbZones As Generic.List(Of roBusinessCenterZone) = Session("BusinessCenters_ZonesValues")
                    oCurrentBusinessCenter.Zones = tbZones
                End If

                If API.TasksServiceMethods.SaveBusinessCenter(Me, oCurrentBusinessCenter, True) = True Then
                    Dim treePath As String = "/source/" & oCurrentBusinessCenter.ID

                    If bolIsNew Then
                        Dim tbBusinessCenter As DataTable = TasksServiceMethods.GetBusinessCenters(Me, False)
                        Dim intRet() As Integer = {0}
                        If tbBusinessCenter IsNot Nothing AndAlso tbBusinessCenter.Rows.Count > 0 Then
                            ReDim intRet(tbBusinessCenter.Rows.Count - 1)
                            Dim i As Integer = 0
                            For Each oRow As DataRow In tbBusinessCenter.Rows
                                intRet(i) = oRow("ID")
                                i += 1
                            Next

                            Dim oGroupFeatures As roGroupFeature() = API.SecurityChartServiceMethods.GetRoboticsGroupFeaturesList(Me.Page)
                            If oGroupFeatures IsNot Nothing AndAlso oGroupFeatures.Length > 0 Then
                                For Each oGroupFeature As roGroupFeature In oGroupFeatures
                                    TasksServiceMethods.SaveBusinessCenterBySecurityGroup(Me, oGroupFeature.ID, intRet.ToArray(), False)
                                Next
                            End If
                        End If
                    End If
                    Dim oLicense As New roServerLicense
                    Dim bolIsInstalledBots As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")
                    If bolIsInstalledBots Then
                        Try
                            Dim oBotState As New roBotState(-1)
                            Dim oBotManager As New roBotManager(oBotState)
                            Dim _oParameters As New Dictionary(Of BotRuleParameterEnum, String) From {
                                {BotRuleParameterEnum.DestinationCostCenter, oCurrentBusinessCenter.ID.ToString}
                            }
                            oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopyCenterCostRole, _oParameters)
                        Catch ex As Exception
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "CopyCenterCostrules::Creating CostCenter " & oCurrentBusinessCenter.Name & ": Error executing bot", ex)
                        End Try
                    End If
                    HelperWeb.roSelector_SetSelection(oCurrentBusinessCenter.ID.ToString, treePath, "ctl00_contentMainBody_roTreesBusinessCenters")
                    rError = New roJSON.JSONError(False, "OK:" & oCurrentBusinessCenter.ID)
                Else
                    rError = New roJSON.JSONError(True, API.TasksServiceMethods.LastErrorTextBusinessCenter)
                End If
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message)
        Finally
            If rError.Error = False Then
                LoadBusinessCenter(oParameters, False, oCurrentBusinessCenter)
                ASPxCallbackPanelContenido.JSProperties("cpIsNew") = True
            Else
                LoadBusinessCenter(oParameters, False, oCurrentBusinessCenter)

                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEBUSINESSCENTER"
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try

    End Sub

    ''' <summary>
    ''' Crea grid de campos
    ''' </summary>
    ''' <param name="arrErrors">Datatable amb els error que pintara</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function creaFieldsGrid(ByVal oBusinessCenter As roBusinessCenter, ByVal ColumnNames() As String, Optional ByVal editMode As Boolean = False, Optional ByVal arrErrors As DataTable = Nothing) As HtmlTable
        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        hTable.Attributes("class") = "GridStyle GridEmpleados"

        'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.Width = "250"
        hTCell.Attributes("style") = "border-right: 0; width: 250px"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Field", Me.DefaultScope) ' "Campo"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Value", Me.DefaultScope) '"Valor"

        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim BusinessCenterFields As DataTable = API.UserFieldServiceMethods.GetBusinessCenterFields(Me, Types.TaskField)

        Dim bolEditable As Boolean = False

        Dim intCountHigh As Integer = 0

        hTable.Rows.Add(hTRow)

        ' Bucle por los campos de la categoría actual
        For i As Integer = 1 To 5

            hTRow = New HtmlTableRow
            altRow = IIf(altRow = "1", "2", "1")

            ' Pinta columna nombre campo
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"

            Dim strDescriptionText As String = ""
            If Not BusinessCenterFields Is Nothing AndAlso BusinessCenterFields.Rows.Count > 0 Then
                strDescriptionText = roTypes.Any2String(BusinessCenterFields.Rows(i - 1)("Name"))
            End If

            hTCell.InnerText = strDescriptionText

            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "

            'hTCell.Attributes("title") = ""
            hTRow.Cells.Add(hTCell)

            ' Pinta columna valor
            Dim strValue As String = ""
            Select Case i
                Case 1 : strValue = oBusinessCenter.Field1
                Case 2 : strValue = oBusinessCenter.Field2
                Case 3 : strValue = oBusinessCenter.Field3
                Case 4 : strValue = oBusinessCenter.Field4
                Case 5 : strValue = oBusinessCenter.Field5
            End Select

            bolEditable = True

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow

            Dim divShowValue As HtmlGenericControl = Nothing

            If Not editMode Or Not bolEditable Then ' En modo normal

                If strValue = "" Then
                    hTCell.InnerText = " "
                Else

                    hTCell.InnerHtml = strValue
                End If

                hTRow.Cells.Add(hTCell)
            Else ' En modo edición

                Dim strHtmlError As String = ""
                Dim strEditCss As String = "textEdit x-form-text x-form-field"
                hTCell.Style("padding") = "0px"

                Dim oContainer As Object
                oContainer = hTCell

                Dim htmlFieldObject As Object = Nothing

                Dim textDev As New DevExpress.Web.ASPxTextBox
                textDev.ID = roTypes.Any2String(i)
                textDev.Text = strValue
                htmlFieldObject = textDev

                Dim endHtmlTable As New HtmlTable
                endHtmlTable.Style.Add("width", "100%")

                Dim endHtmlRow As New HtmlTableRow

                'Dim strInnerHtml As String = "<table style=""width:100%;""><tr>"
                If htmlFieldObject Is Nothing Then
                    htmlFieldObject = New HtmlGenericControl
                End If

                Dim endHtmlTableCell As New HtmlTableCell
                endHtmlTableCell.ColSpan = 3
                endHtmlTableCell.Controls.Add(htmlFieldObject)
                endHtmlRow.Controls.Add(endHtmlTableCell)

                endHtmlTable.Controls.Add(endHtmlRow)
                oContainer.controls.add(endHtmlTable)
                If strHtmlError <> "" Then
                    Dim tbValue As HtmlTable = hTCell.Controls(0)
                    Dim rwValue As New HtmlTableRow
                    Dim clValue As New HtmlTableCell
                    clValue.InnerHtml = strHtmlError
                    rwValue.Cells.Add(clValue)
                    tbValue.Rows.Add(rwValue)
                End If
            End If

            ' Cerramos la fila
            hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

        Next

        Return hTable

    End Function

    Private Function saveBusinessCenterFieldsGrid(ByVal oParameters As CallbackRequest) As String
        Try
            Dim oBusinessCenter As roBusinessCenter = Nothing
            Dim intID As Integer
            Dim oState As New Robotics.Base.VTBusiness.Group.roGroupState
            Dim bolSaveData As Boolean

            'Taula per acumular els errors
            Dim errTable As DataTable = New DataTable
            errTable.Columns.Add("FieldName")
            errTable.Columns.Add("FieldValue")
            errTable.Columns.Add("ErrorDescription")

            intID = oParameters.ID
            oBusinessCenter = API.TasksServiceMethods.GetBusinessCenterByID(Me, intID, False)

            If oBusinessCenter Is Nothing Then
                Return ""
                Exit Function
            End If

            ' Guardo los UserFields del centro de coste actual
            bolSaveData = True
            Dim strFieldName As String

            Dim oFieldParameters() As String = oParameters.resultClientAction.Split("&")

            For Each cVars As String In oFieldParameters
                If cVars = String.Empty Then Continue For

                Dim fieldData() As String = cVars.Split("=")
                strFieldName = fieldData(0).Substring(4)

                If strFieldName.ToString.EndsWith("_@@Date@@_I") Then Continue For
                If strFieldName.ToString.EndsWith("_I") Then
                    strFieldName = strFieldName.Substring(0, strFieldName.Length - 2)
                End If
                If strFieldName.ToString.EndsWith("_VI") Then Continue For
                If strFieldName.ToString.EndsWith("_DDDWS") Then Continue For
                If strFieldName.ToString.EndsWith("_LDeletedItems") Then Continue For
                If strFieldName.ToString.EndsWith("_LInsertedItems") Then Continue For
                If strFieldName.ToString.EndsWith("_LCustomCallback") Then Continue For
                If strFieldName.ToString.EndsWith("_Raw") Then Continue For
                If strFieldName.ToString.EndsWith("_DDD_C_STATE") Then Continue For
                If strFieldName.ToString.EndsWith("_DDD_C_FNPWS") Then Continue For

                If cVars.ToString.StartsWith("USR_") Then

                    Select Case roTypes.Any2Double(strFieldName)
                        Case 1 : oBusinessCenter.Field1 = fieldData(1)
                        Case 2 : oBusinessCenter.Field2 = fieldData(1)
                        Case 3 : oBusinessCenter.Field3 = fieldData(1)
                        Case 4 : oBusinessCenter.Field4 = fieldData(1)
                        Case 5 : oBusinessCenter.Field5 = fieldData(1)
                    End Select

                End If

            Next

            If API.TasksServiceMethods.SaveBusinessCenter(Me, oBusinessCenter, True) Then
                Return ""
            Else
                Return "ERROR"
            End If
        Catch ex As Exception
            Dim strResponse As String

            strResponse = "MESSAGE" &
                          "TitleKey=SaveGroupUserFields.Error.Title&" +
                          "DescriptionText=" + ex.Message + "&" +
                          "Option1TextKey=SaveGroupUserFields.Error.Option1Text&" +
                          "Option1DescriptionKey=SaveGroupUserFields.Error.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)

            Response.Write(strResponse)

        End Try

        Return ""
    End Function

#Region "Zones grid"

    Private Sub BindGridZones(ByVal bolReload As Boolean)
        Me.GridZones.DataSource = Me.ZonesData(bolReload)
        Me.GridZones.DataBind()
    End Sub

    Private Sub CreateColumnsZones()
        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn

        Dim VisibleIndex As Integer = 0

        Me.GridZones.Columns.Clear()
        Me.GridZones.KeyFieldName = "IDZone"
        Me.GridZones.SettingsText.EmptyDataRow = " "
        Me.GridZones.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        If Me.oPermission = Permission.Admin Or Me.oPermission >= Permission.Write Then
            Me.GridZones.SettingsEditing.Mode = GridViewEditingMode.Inline
        Else
            Dim uiControl As Control = Me.GridZones.FindTitleTemplateControl("btnAddNewZone")
            If uiControl IsNot Nothing Then
                uiControl.Visible = False
            End If
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "IDZone"
        GridColumn.FieldName = "IDZone"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridZones.Columns.Add(GridColumn)

        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("GridZones.Column.Zone", DefaultScope)
        GridColumnCombo.FieldName = "IDZone"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = ListZonesData(True)
        GridColumnCombo.PropertiesComboBox.TextField = "Name"
        GridColumnCombo.PropertiesComboBox.ValueField = "ID"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.Width = 60
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = False
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridZones.Columns.Add(GridColumnCombo)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowUpdateButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = Unit.Pixel(26)
        VisibleIndex = VisibleIndex + 1

        Me.GridZones.Columns.Add(GridColumnCommand)

    End Sub

    Protected Sub GridZones_CellEditorInitialize(sender As Object, e As ASPxGridViewEditorEventArgs) Handles GridZones.CellEditorInitialize
        If e.Column.FieldName = "IDZone" Then

            Dim tb As DataTable = Me.ZonesData()

            Dim dRow As DataRow = tb.Rows.Find(e.KeyValue)

            Dim cmb As ASPxComboBox = CType(e.Editor, ASPxComboBox)
            cmb.Font.Size = 8
            cmb.Focus()

        End If

    End Sub

    Protected Sub GridZones_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles GridZones.CustomCallback
        If e.Parameters = "REFRESH" Then
            BindGridZones(False)
            GridZones.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            BindGridZones(True)
            GridZones.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridZones_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridZones.RowDeleting
        Dim tb As DataTable = Me.ZonesData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridZones.KeyFieldName))
        If dr IsNot Nothing Then
            dr.Delete()
            tb.AcceptChanges()
            Me.ZonesData = tb
            e.Cancel = True
        End If

        BindGridZones(False)
        GridZones.JSProperties("cpAction") = "ROWDELETE"
    End Sub

    Private Property ListZonesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("BusinessCenters_Zones")

            If bolReload OrElse tb Is Nothing Then
                tb = API.ZoneServiceMethods.GetZones(Me)

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If

                Session("BusinessCenters_Zones") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("BusinessCenters_Zones") = value
        End Set
    End Property

#End Region

    Protected Sub GridZones_RowInserting(sender As Object, e As Data.ASPxDataInsertingEventArgs) Handles GridZones.RowInserting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        If Not e.NewValues Is Nothing Then
            Dim bFound As Boolean = False
            Dim tb As DataTable = Me.ZonesData()
            If (tb.Rows.Count > 0) Then
                For Each oRow As DataRow In tb.Rows
                    If roTypes.Any2Integer(oRow("IDZone")) = roTypes.Any2Integer(e.NewValues("IDZone")) Then
                        bFound = True
                    End If
                Next

            End If
            If Not bFound Then
                If e.NewValues("IDZone") Is Nothing Then
                    Throw New Exception(Me.Language.Translate("BusinessCenters.Error.EmptyData", Me.DefaultScope))
                End If

                Dim oNewRow As DataRow = tb.NewRow
                With oNewRow
                    .Item("IDZone") = e.NewValues("IDZone")
                End With

                tb.Rows.Add(oNewRow)
                tb.AcceptChanges()
                Me.ZonesData = tb
                e.Cancel = True
                grid.CancelEdit()
            Else
                Throw New Exception("Error en la zona")
            End If
        End If

        BindGridZones(False)
        GridZones.JSProperties("cpAction") = "ROWINSERTING"

    End Sub

    Protected Sub GridZones_CustomErrorText(ByVal sender As Object, ByVal e As ASPxGridViewCustomErrorTextEventArgs) Handles GridZones.CustomErrorText
        If e.Exception IsNot Nothing Then
            e.ErrorText = e.Exception.Message
        End If
    End Sub

    Protected Sub GridZones_RowUpdating(sender As Object, e As Data.ASPxDataUpdatingEventArgs) Handles GridZones.RowUpdating
        Dim tb As DataTable = Me.ZonesData()
        Dim dr As DataRow = tb.Rows.Find(e.Keys(GridZones.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "IDZone"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDZone") = enumerator.Value
                    Else
                        dr.Item("IDZone") = String.Empty
                    End If
            End Select

        End While

        If e.NewValues("IDZone") Is Nothing Then
            Throw New Exception(Me.Language.Translate("BusinessCenters.Error.EmptyData", Me.DefaultScope))
        End If

        e.Cancel = True
        grid.CancelEdit()
        BindGridZones(False)

        GridZones.JSProperties("cpAction") = "ROWUPDATING"

    End Sub

End Class