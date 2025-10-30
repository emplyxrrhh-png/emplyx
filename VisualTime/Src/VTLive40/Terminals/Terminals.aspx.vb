Imports DevExpress.CodeParser
Imports DevExpress.Web
Imports DevExpress.Xpo.Logger.Transport
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Terminals
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class TerminalsCallbackRequest

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

        <Runtime.Serialization.DataMember(Name:="FieldModified")>
        Public FieldModified As String

        <Runtime.Serialization.DataMember(Name:="IdReader")>
        Public IdReader As Integer

        <Runtime.Serialization.DataMember(Name:="sirens")>
        Public sirens As SirensJson()

        <Runtime.Serialization.DataMember(Name:="nfctags")>
        Public tags As TagsJson()

        <Runtime.Serialization.DataMember(Name:="gridTags")>
        Public GridTags As TagsJson()

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class SirensJson

        <Runtime.Serialization.DataMember(Name:="id")>
        Public id As Integer

        <Runtime.Serialization.DataMember(Name:="idterminal")>
        Public idterminal As Integer

        <Runtime.Serialization.DataMember(Name:="weekday")>
        Public weekday As Integer

        <Runtime.Serialization.DataMember(Name:="weekdayname")>
        Public weekdayname As String

        <Runtime.Serialization.DataMember(Name:="duration")>
        Public duration As Integer

        <Runtime.Serialization.DataMember(Name:="hour")>
        Public hour As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class TagsJson

        <Runtime.Serialization.DataMember(Name:="id")>
        Public id As Integer

        <Runtime.Serialization.DataMember(Name:="description")>
        Public description As String

        <Runtime.Serialization.DataMember(Name:="nfc")>
        Public nfc As String

        <Runtime.Serialization.DataMember(Name:="idzone")>
        Public idzone As Integer

        <Runtime.Serialization.DataMember(Name:="zone")>
        Public zone As String

        <Runtime.Serialization.DataMember(Name:="mode")>
        Public mode As String

        <Runtime.Serialization.DataMember(Name:="idmode")>
        Public idmode As Integer

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class ZonesJson

        <Runtime.Serialization.DataMember(Name:="id")>
        Public id As Integer

        <Runtime.Serialization.DataMember(Name:="name")>
        Public name As String

        <Runtime.Serialization.DataMember(Name:="zonemode")>
        Public zonemode As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class ModesJson

        <Runtime.Serialization.DataMember(Name:="id")>
        Public id As Integer

        <Runtime.Serialization.DataMember(Name:="name")>
        Public name As String

    End Class

#Region "properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("Terminals_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("Terminals_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("Terminals_iCurrentTask") = value
        End Set
    End Property

    Private Property ErrorExists As Boolean
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorExists")
            If val IsNot Nothing Then
                Return roTypes.Any2Boolean(val)
            Else
                HttpContext.Current.Session("ErrorExists") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("ErrorExists") = value
        End Set
    End Property

    Private Property ErrorDescription As String
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorDescription")
            If val IsNot Nothing Then
                Return roTypes.Any2String(val)
            Else
                HttpContext.Current.Session("ErrorDescription") = False
                Return False
            End If
        End Get
        Set(value As String)
            HttpContext.Current.Session("ErrorDescription") = value
        End Set
    End Property

    Public ReadOnly Property GetTerminalTypes(ByVal strRetTypes As String) As DataTable
        Get
            SyncLock (GetType(Terminals))
                If HttpContext.Current.Cache("Terminals_TerminalsType" & "_" & strRetTypes.ToUpper()) IsNot Nothing Then
                    Return HttpRuntime.Cache("Terminals_TerminalsType" & "_" & strRetTypes.ToUpper())
                Else
                    Dim dTblTerminalTypes As DataTable = API.TerminalServiceMethods.GetTerminalTypes(Me.Page, strRetTypes.ToUpper())
                    HttpRuntime.Cache.Add("Terminals_TerminalsType" & "_" & strRetTypes.ToUpper(), dTblTerminalTypes, Nothing, Cache.NoAbsoluteExpiration, New TimeSpan(1, 0, 0), CacheItemPriority.Default, Nothing)
                    Return dTblTerminalTypes
                End If
            End SyncLock
        End Get
    End Property

    Public Property TerminalReadersData() As Hashtable
        Get
            Return Session("Terminals_TerminalReaders")
        End Get
        Set(ByVal value As Hashtable)
            Session("Terminals_TerminalReaders") = value
        End Set
    End Property

    Public Property CurrentTerminalID As Integer
        Get
            Return Session("Terminal_CurrentTerminalID")
        End Get
        Set(value As Integer)
            Session("Terminal_CurrentTerminalID") = value
        End Set
    End Property

#End Region

    Private Const FeatureAlias As String = "Terminals.Definition"
    Private Const FeatureAliasStatus As String = "Terminals.StatusInfo"
    Private oPermission As Permission
    Private oPermissionStatus As Permission

    Private bolCheckAccessAuthorizationOnNoAccessReaders As Boolean = False

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("flReaderMap", "~/Terminals/Scripts/flReaderMap.js")

        Me.InsertExtraJavascript("TerminalsSelector", "~/Terminals/Scripts/TerminalsSelector.js")
        Me.InsertExtraJavascript("TerminalsListV2", "~/Terminals/Scripts/TerminalsListV2.js")
        Me.InsertExtraJavascript("TerminalsV2", "~/Terminals/Scripts/TerminalsV2.js")

        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("roUserFieldCriteria", "~/Base/Scripts/roUserFieldCriteria.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("frmAddSiren", "~/Terminals/Scripts/frmAddSiren.js")
        Me.InsertExtraJavascript("frmCfgInteractive", "~/Terminals/Scripts/frmCfgInteractive.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Terminals") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesTerminals.TreeCaption = Me.Language.Translate("TreeCaptionTerminals", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oPermissionStatus = Me.GetFeaturePermission(FeatureAliasStatus)

        Me.bolCheckAccessAuthorizationOnNoAccessReaders = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("CheckAccessAuthorizationOnNoAccessReaders"))

        If Me.oPermission = Permission.None And Me.oPermissionStatus = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
            Me.DisableControls(Me.Controls)
        Else
            hdnModeEdit.Value = "false"
        End If

        If Not IsPostBack Then
            LoadInitialData()
        End If

    End Sub

    Public ReadOnly Property showNoConfig As String
        Get
            Dim ret As Boolean = Not Session("showNoConfig") Is Nothing AndAlso Session("showNoConfig").ToString.Length > 0
            Session("showNoConfig") = ""
            Return ret.ToString.ToLower
        End Get
    End Property

    Public ReadOnly Property showNoUSBFile As String
        Get
            Dim ret As Boolean = Not Session("showNoUSBFile") Is Nothing AndAlso Session("showNoUSBFile").ToString.Length > 0
            Session("showNoUSBFile") = ""
            Return ret.ToString.ToLower
        End Get
    End Property

    Private Sub LoadInitialData()

        ' Cargo campos
        Try
            ' Avisamos si las comunicaciones están deshabilitadas
            ' Miramos el estado de las comunicaciones. Parámetro CommsOffline de sysroParameters
            Dim oParameters = API.ConnectorServiceMethods.GetParameters(Me)
            Dim oParams As New roCollection(oParameters.ParametersXML)
            Dim sOfflineDate As String
            Dim sOffline As String
            Dim bCommsOffline As Boolean

            sOffline = roTypes.Any2String(oParams.Item(oParameters.ParametersNames(Parameters.CommsOffLine)))
            sOfflineDate = roTypes.Any2String(oParams.Item(oParameters.ParametersNames(Parameters.CommsOffLineDate)))
            bCommsOffline = IsDate(sOfflineDate)

            If bCommsOffline Then
                ' Comunicaciones deshabilitadas.
                If sOffline = "1" Then
                    ' Seguirán deshabilitadas en sucesivos reinicios
                    lblCommsState.Text = Me.Language.Translate("TerminalList.CoomsState.Offline", DefaultScope)
                    imgCommsDisabledAlert.Visible = True
                Else
                    ' Se habilitarán en el próximo reinicio de servidor
                    lblCommsState.Text = Me.Language.Translate("TerminalList.CoomsState.Offline", DefaultScope)
                    imgCommsDisabledAlert.Visible = True
                End If
            Else
                ' Comunicaciones habilitadas
                If sOffline <> "1" Then
                    ' Seguirán habilitadas en próximos reinicios
                    lblCommsState.Text = ""
                    imgCommsDisabledAlert.Visible = False
                Else
                    ' Se deshabilitarán en el proximo reinicio
                    lblCommsState.Text = Me.Language.Translate("TerminalList.CoomsState.Offline", DefaultScope)
                    imgCommsDisabledAlert.Visible = True
                End If
            End If

            ReloadSourcesCombos()
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try

    End Sub

    Protected Sub ReloadSourcesCombos()
        Me.cmbSourceData.Items.Clear()
        Me.cmbSourceData.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("Import.DefaultSource", Me.DefaultScope), ""))
        Me.cmbSourceData.SelectedIndex = 0

        Me.cmbIDEmployeeSuprema.Items.Clear()
        Dim dtblUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me, Types.EmployeeField, "Used <> 0", False)

        Me.cmbIDEmployeeSuprema.Items.Add("", "0")
        If dtblUserFields IsNot Nothing AndAlso dtblUserFields.Rows.Count > 0 Then
            Dim dRows() As DataRow = dtblUserFields.Select("", "FieldName")

            For Each dRow As DataRow In dRows
                If dRow("FieldType") = 0 OrElse dRow("FieldType") = 1 Then
                    Me.cmbIDEmployeeSuprema.Items.Add(dRow("FieldName"), dRow("ID"))
                End If
            Next
        End If
        Me.cmbIDEmployeeSuprema.SelectedIndex = 0


    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New TerminalsCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Select Case oParameters.Type
            Case "T"
                ProcessTerminalRequest(oParameters)
                rowTerminalInfo.Style("Display") = ""
                rowTerminalsList.Style("Display") = "none"
            Case "L"
                ProcessTerminalListRequest(oParameters)
                rowTerminalInfo.Style("Display") = "none"
                rowTerminalsList.Style("Display") = ""
        End Select
    End Sub

#Region "Terminals"

    Private Sub ProcessTerminalRequest(ByVal oParameters As TerminalsCallbackRequest)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case oParameters.Action
                Case "GETTERMINALS"
                    LoadTerminalsData(oParameters)
                Case "VALIDATETERMINALREADER"
                    ValidateTerminalReaders(oParameters)
                Case "SAVETERMINAL"
                    SaveTerminal(oParameters)
            End Select

            ProcessTerminalsSelectedTabVisible(oParameters)
        End If
    End Sub

    Private Sub ProcessTerminalsSelectedTabVisible(ByVal oParameters As TerminalsCallbackRequest)
        Me.div01.Style("display") = "none"
        Me.div02.Style("display") = "none"
        Me.div03.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.div01.Style("display") = ""
            Case 1
                Me.div02.Style("display") = ""
            Case 2
                Me.div03.Style("display") = ""
        End Select
    End Sub

    Private Sub LoadTerminalRelatedData(ByRef oParameters As TerminalsCallbackRequest, ByVal oTerminal As roTerminal)
        Dim strRetTypes As String = ""
        If oParameters.ID = -1 Then strRetTypes = "Remote"

        Dim dTblTerminalTypes As DataTable = Me.GetTerminalTypes(strRetTypes)
        cmbModel.Items.Clear()
        For Each dRow As DataRow In dTblTerminalTypes.Rows
            cmbModel.Items.Add(dRow("Type").ToString, dRow("Type").ToString)
        Next

        ' Determinamos si el terminal es tipo web
        Dim bolWeb As Boolean = False
        dTblTerminalTypes = Me.GetTerminalTypes("web")
        If dTblTerminalTypes IsNot Nothing Then
            Dim dRowT As DataRow()
            dRowT = dTblTerminalTypes.Select("Type = '" & oTerminal.Type.Replace("'", "''") & "'")
            If dRowT.Length > 0 Then
                bolWeb = True
            End If
        End If

        Dim bVirtual As Boolean
        bVirtual = (oTerminal.Type.ToString.ToUpper = "VIRTUAL")

        Dim pathImg As String = "Images/TerminalIcos/"
        Dim strImg As String = "BLANK"
        If oTerminal.Type <> "" Then strImg = oTerminal.Type
        If oTerminal.LastStatus.ToUpper <> "OK" Then strImg &= "_DIS"

        'Permisos Info. Estat
        If bolWeb Or bVirtual Or oPermissionStatus = Permission.None Then
            strImg = strImg.Replace("_DIS", "")
            lblConfStatus.Style("display") = "none"
            txtStatus.Style("display") = "none"
        Else
            lblConfStatus.Style("display") = ""
            txtStatus.Style("display") = ""
        End If

        imgTerminal.Src = pathImg & strImg & ".png"

        ' Cargamos la lista de zonas horarias
        Me.loadComboTimeZones(oTerminal.Type)

        Me.tdCheckTimeZone.Style.Item("display") = IIf(oTerminal.Type.ToLower = "liveportal" OrElse oTerminal.Type.ToLower = "mx9" OrElse oTerminal.Type.ToLower = "nfc", "none", "")
        Me.trLocation.Style("display") = IIf(bolWeb, "none", "")

        ' Sólo mostramos la opción de capturar imagen si es un terminal mx6 o mx7
        Me.tdCaptureImage.Style.Item("display") = IIf(oTerminal.Type.ToLower = "mx6" Or oTerminal.Type.ToLower = "mx7" Or oTerminal.Type.ToLower = "mx8" Or oTerminal.Type.ToLower = "mx9", "", "none")
        Me.panUSB.Style.Item("display") = IIf(oTerminal.Type.ToLower = "mx8", "", "none")

        Dim arrRelays() As String = oTerminal.SupportedSirens.Split(",")
        cmbRelay.ValueType = GetType(Integer)
        cmbRelay.Items.Clear()
        cmbRelay.Items.Add(New DevExpress.Web.ListEditItem("", 0))
        For Each strRelay As String In arrRelays
            cmbRelay.Items.Add(New DevExpress.Web.ListEditItem(strRelay, roTypes.Any2Integer(strRelay)))
        Next

        If TerminalReadersData Is Nothing Then
            TerminalReadersData = New Hashtable
        End If

        Dim bLegacyEnabled As Boolean = False
        For n As Integer = 1 To oTerminal.Readers.Count
            Dim hTitle As New HtmlGenericControl("SPAN")
            hTitle.InnerHtml = oTerminal.Readers(n - 1).Description
            Dim hControl As New WebUserControls_frmTerminalReaderV2()
            If Not bLegacyEnabled Then bLegacyEnabled = oTerminal.Readers(n - 1).LegacyRestrictionModeAllowed

            Select Case n
                Case 1
                    tabCtl01.TabVisible(1) = True
                    frmTR1.LoadDefaultValues(oTerminal, oTerminal.Readers(n - 1), oPermission, bolCheckAccessAuthorizationOnNoAccessReaders)
                    tabCtl01.TabTitle1.Controls.Clear()
                    tabCtl01.TabTitle1.Controls.Add(hTitle)
                Case 2
                    tabCtl01.TabVisible(2) = True
                    frmTR2.LoadDefaultValues(oTerminal, oTerminal.Readers(n - 1), oPermission, bolCheckAccessAuthorizationOnNoAccessReaders)
                    tabCtl01.TabTitle2.Controls.Clear()
                    tabCtl01.TabTitle2.Controls.Add(hTitle)
                Case 3
                    tabCtl01.TabVisible(3) = True
                    frmTR3.LoadDefaultValues(oTerminal, oTerminal.Readers(n - 1), oPermission, bolCheckAccessAuthorizationOnNoAccessReaders)
                    tabCtl01.TabTitle3.Controls.Clear()
                    tabCtl01.TabTitle3.Controls.Add(hTitle)
                Case 4
                    tabCtl01.TabVisible(4) = True
                    frmTR4.LoadDefaultValues(oTerminal, oTerminal.Readers(n - 1), oPermission, bolCheckAccessAuthorizationOnNoAccessReaders)
                    tabCtl01.TabTitle4.Controls.Clear()
                    tabCtl01.TabTitle4.Controls.Add(hTitle)
            End Select
        Next

        Me.legacyModeEnabled.Value = bLegacyEnabled.ToString()

        If oParameters.aTab = 2 And oTerminal.SupportedSirens = "" Then
            oParameters.aTab = 0
        End If

        'Configuramos visiblidad de componentes para NFC
        Me.divNFC.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc", "", "none")
        Me.trSerialNumber.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc", "none", "")
        Me.trFirmware.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc", "none", "")
        Me.trLocation.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc" OrElse oTerminal.Type.ToLower = "suprema", "none", "")
        Me.Label1.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc" OrElse oTerminal.Type.ToLower = "suprema", "none", "")
        Me.lblConfStatus.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc", "none", "")
        Me.txtStatus.Style.Item("display") = IIf(oTerminal.Type.ToLower = "nfc", "none", "")
        Me.txtTerminalAddress.ClientEnabled = False

    End Sub

    Private Sub ValidateTerminalReaders(ByVal oParameters As TerminalsCallbackRequest)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oTerminal As roTerminal = Nothing

        Try
            oTerminal = API.TerminalServiceMethods.GetTerminal(Me, oParameters.ID, False)

            If oTerminal Is Nothing Then Exit Sub

            'Check Permissions
            Dim disControls As Boolean = False
            If Me.oPermission < Permission.Write Then
                Me.DisableControls(Me.Controls)
                Me.panTbSirens.Controls.Clear()
            End If

            LoadTerminalRelatedData(oParameters, oTerminal)

            LoadDataFromControlsToTerminal(oParameters, oTerminal)

            Dim hControl As New WebUserControls_frmTerminalReaderV2()
            Dim bLegacyEnabled As Boolean = False
            For n As Integer = 1 To oTerminal.Readers.Count
                bLegacyEnabled = oTerminal.Readers(n - 1).LegacyRestrictionModeAllowed
                Select Case n
                    Case 1
                        hControl = frmTR1
                    Case 2
                        hControl = frmTR2
                    Case 3
                        hControl = frmTR3
                    Case 4
                        hControl = frmTR4
                End Select

                oTerminal.Readers(n - 1) = hControl.LoadReaderValueFromSource(oTerminal, oPermission, oParameters.FieldModified, bLegacyEnabled)
            Next
        Catch ex As Exception
        Finally
            LoadTerminalsData(oParameters, oTerminal)
            ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "VALIDATETERMINALREADER"
            If strError = String.Empty Then
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "OK"
                ASPxCallbackPanelContenido.JSProperties.Add("cpControlValidationRO", oParameters.FieldModified)
                ASPxCallbackPanelContenido.JSProperties.Add("cpReaderValidationIdRO", oParameters.IdReader)
            Else
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = strMessage
            End If
        End Try
    End Sub

    Private Sub LoadTerminalsData(ByRef oParameters As TerminalsCallbackRequest, Optional ByVal eTerminal As roTerminal = Nothing)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oTerminal As roTerminal = Nothing
        Dim GridsJSON As String = ""
        Dim ComboZones As String = ""
        Dim ModesZones As String = ""
        Dim NFCReaders As ArrayList
        Dim zones As DataTable

        Try
            If eTerminal Is Nothing Then
                oTerminal = API.TerminalServiceMethods.GetTerminal(Me, oParameters.ID, False)
                TerminalReadersData = Nothing
            Else
                oTerminal = eTerminal
            End If

            If oTerminal Is Nothing Then Exit Sub

            'Check Permissions
            Dim disControls As Boolean = False
            If Me.oPermission < Permission.Write Then
                Me.DisableControls(Me.Controls)
                Me.panTbSirens.Controls.Clear()
            End If

            LoadTerminalRelatedData(oParameters, oTerminal)

            txtName.Text = oTerminal.Description
            txtID.Value = oTerminal.ID
            CurrentTerminalID = oTerminal.ID

            txtID.Enabled = False
            For Each oItem As DevExpress.Web.ListEditItem In cmbModel.Items
                If roTypes.Any2String(oItem.Value).ToUpper = oTerminal.Type.ToUpper Then
                    cmbModel.SelectedItem = oItem
                    Exit For
                End If
            Next
            cmbModel.Enabled = False

            txtTerminalAddress.Value = oTerminal.Location
            txtFirmware.Value = If(oTerminal.FirmVersion.Trim = String.Empty, Me.Language.Translate("TerminalList.FirmwareUnknown", DefaultScope), oTerminal.FirmVersion)

            txtSerialNumber.Value = If(oTerminal.SerialNumber.Trim = String.Empty, Me.Language.Translate("TerminalList.FirmwareUnknown", DefaultScope), oTerminal.SerialNumber)

            Dim lstStatus As String = ""
            If oTerminal.LastStatus IsNot Nothing Then lstStatus = oTerminal.LastStatus.ToString.ToUpper

            If lstStatus = "OK" Then
                txtStatus.Style("color") = "green"
                txtStatus.Text = Me.Language.Translate("TerminalConnected", DefaultScope)
            Else
                txtStatus.Style("color") = "red"
                txtStatus.Text = Me.Language.Translate("TerminalDisconnected", DefaultScope)
            End If

            txtStatus.Attributes("title") = oTerminal.LastUpdate.ToString.ToUpper

            chkTimeZone.Checked = oTerminal.IsDifferentZoneTime
            chkAutoDaylight.Checked = oTerminal.AutoDaylight
            For Each oItem As DevExpress.Web.ListEditItem In cmbTimeZones.Items
                If roTypes.Any2String(oItem.Value).Split("_")(0) = oTerminal.TimeZoneName Then
                    cmbTimeZones.SelectedItem = oItem
                    Exit For
                End If
            Next

            Dim bolCaptureImage As Boolean = False
            Dim bolUSB As Boolean = False
            If oTerminal.Type.ToLower = "mx6" Or oTerminal.Type.ToLower = "mx7" Or oTerminal.Type.ToLower = "mx8" Or oTerminal.Type.ToLower = "mx9" Then
                If oTerminal.ConfigurationTable IsNot Nothing AndAlso oTerminal.ConfigurationTable.Tables.Count > 0 AndAlso oTerminal.ConfigurationTable.Tables(0).Rows.Count > 0 Then
                    Dim oRows() As DataRow = oTerminal.ConfigurationTable.Tables(0).Select("Name='CaptureImage'")
                    If oRows.Length = 1 Then
                        bolCaptureImage = roTypes.Any2Boolean(oRows(0).Item("Value"))
                    End If
                End If
            End If
            chkCaptureImage.Checked = bolCaptureImage

            chkEnabled.Visible = True
            If oTerminal.Type = "LivePortal" OrElse oTerminal.Type = "NFC" OrElse oTerminal.Type = "Suprema" OrElse oTerminal.Type = "Time Gate" OrElse oTerminal.Type = "Virtual" Then
                oTerminal.Enabled = True
                chkEnabled.Visible = False
            End If

            chkEnabled.Checked = oTerminal.Enabled

            cmbRelay.SelectedItem = cmbRelay.Items.FindByValue(oTerminal.SirensOutput)

            Dim hControl As New WebUserControls_frmTerminalReaderV2()
            For n As Integer = 1 To oTerminal.Readers.Count
                Select Case n
                    Case 1
                        hControl = frmTR1
                        hControl.LoadReaderValue(oTerminal, oTerminal.Readers(n - 1), oPermission)
                    Case 2
                        hControl = frmTR2
                        hControl.LoadReaderValue(oTerminal, oTerminal.Readers(n - 1), oPermission)
                    Case 3
                        hControl = frmTR3
                        hControl.LoadReaderValue(oTerminal, oTerminal.Readers(n - 1), oPermission)
                    Case 4
                        hControl = frmTR4
                        hControl.LoadReaderValue(oTerminal, oTerminal.Readers(n - 1), oPermission)
                End Select
            Next

            If oTerminal.Type = "NFC" Then
                NFCReaders = oTerminal.Readers
                Dim tagsJson = LoadNFCDataFromReaders(oTerminal)
                GridsJSON = roJSONHelper.SerializeNewtonSoft(tagsJson)

                zones = API.ZoneServiceMethods.GetZones(Me.Page)
                Dim ZonesJson = LoadZonesJson(zones)
                ComboZones = roJSONHelper.SerializeNewtonSoft(ZonesJson)
            End If

            Select Case oTerminal.Type
                Case "Time Gate"
                    trFirmware.Visible = False
                    trAPKVersion.Visible = True
                    trInactivityTime.Visible = True
                    txtInactivityTime.Text = oTerminal.CustomDuration
                    txtAPKVersion.Text = oTerminal.FirmVersion
                    tdCheckTimeZone.Visible = False
                    Label1.Text = Me.Language.Translate("terminals.label1sharedportal.text", DefaultScope)
                Case "Suprema"
                    trFirmware.Visible = False
                    trAPKVersion.Visible = False
                    trInactivityTime.Visible = False
                    tdCheckTimeZone.Visible = False
                    Label1.Text = Me.Language.Translate("terminals.label1sharedportal.text", DefaultScope)
                Case Else
                    trFirmware.Visible = True
                    trAPKVersion.Visible = False
                    trInactivityTime.Visible = False
                    tdCheckTimeZone.Visible = True
                    Label1.Text = Me.Language.Translate("terminals.label1.text", DefaultScope)
            End Select

        Catch ex As Exception
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETTERMINAL")
            If strError = String.Empty Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oTerminal.Description)
                ASPxCallbackPanelContenido.JSProperties.Add("cpActiveReadersRO", oTerminal.Readers.Count)
                ASPxCallbackPanelContenido.JSProperties.Add("cpSirensRO", CreateSirensGridsJSON(oTerminal))
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", "")

                If oTerminal.Type = "NFC" Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpNFCReaders", GridsJSON)
                    ASPxCallbackPanelContenido.JSProperties.Add("cpZones", ComboZones)
                    'ASPxCallbackPanelContenido.JSProperties.Add("cpModes", ModesZones)
                End If
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "KO")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessageRO", strMessage)
            End If

        End Try
    End Sub

    Private Sub SaveTerminal(ByRef oParameters As TerminalsCallbackRequest, Optional ByVal eTerminal As roTerminal = Nothing)
        Dim oError As roJSON.JSONError = Nothing
        Dim oTerminal As roTerminal = Nothing

        Try
            If oPermission < Permission.Write Then
                oError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Else
                Dim strSupportedSirens As String = ""
                oTerminal = API.TerminalServiceMethods.GetTerminal(Me, oParameters.ID, False)

                strSupportedSirens = oTerminal.SupportedSirens

                If oTerminal Is Nothing Then Exit Sub

                LoadTerminalRelatedData(oParameters, oTerminal)

                oError = LoadDataFromControlsToTerminal(oParameters, oTerminal)

                Dim hControl As New WebUserControls_frmTerminalReaderV2()
                Dim bLegacyEnabled As Boolean = False
                For n As Integer = 1 To oTerminal.Readers.Count
                    bLegacyEnabled = oTerminal.Readers(n - 1).LegacyRestrictionModeAllowed
                    Select Case n
                        Case 1
                            hControl = frmTR1
                        Case 2
                            hControl = frmTR2
                        Case 3
                            hControl = frmTR3
                        Case 4
                            hControl = frmTR4
                    End Select
                    oTerminal.Readers(n - 1) = hControl.LoadReaderValueFromSource(oTerminal, oPermission, bLegacyEnabled)
                Next

                If Not oError.Error Then
                    If oTerminal.Type = "NFC" Then
                        For Each tag In oParameters.GridTags
                            API.TerminalServiceMethods.SaveNFCReader(Me, oTerminal.ID, tag.id, tag.idzone, tag.nfc, tag.idmode, tag.description, True)
                        Next
                    ElseIf oTerminal.Type.ToUpper = "TIME GATE" Then
                        If txtInactivityTime.Value < 10 OrElse txtInactivityTime.Value > 250 Then
                            oError = New roJSON.JSONError(True, Me.Language.Translate("InvalidInactivityTime.Description", DefaultScope))
                        Else
                            oTerminal.CustomDuration = txtInactivityTime.Value
                        End If

                    End If
                    If Not oError.Error Then
                        If API.TerminalServiceMethods.SaveTerminal(Me, oTerminal, True) Then
                            oParameters.ID = oTerminal.ID
                            oError = New roJSON.JSONError(False, "OK:" & oTerminal.ID)
                        Else
                            oError = New roJSON.JSONError(True, API.TerminalServiceMethods.LastErrorText)
                        End If
                    End If

                End If
                oTerminal.SupportedSirens = strSupportedSirens
            End If
        Catch ex As Exception
        Finally
            If oError IsNot Nothing AndAlso oError.Error = False Then
                LoadTerminalsData(oParameters)
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVETERMINAL"
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "OK"
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameTreeRO", oTerminal.ID & " - " & oTerminal.Description)
            Else
                LoadTerminalsData(oParameters, oTerminal)
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVETERMINAL"
                If oError IsNot Nothing Then
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = oError.toJSON()
                Else
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = New roJSON.JSONError(True, "").toJSON()
                End If
            End If
        End Try

    End Sub

    Private Function LoadDataFromControlsToTerminal(ByVal oParameters As TerminalsCallbackRequest, ByRef oTerminal As roTerminal) As roJSON.JSONError

        Dim oError As New roJSON.JSONError(False, "")

        oTerminal.Location = roTypes.Any2String(txtTerminalAddress.Value)

        oTerminal.Description = txtName.Text

        oTerminal.IsDifferentZoneTime = chkTimeZone.Checked
        oTerminal.AutoDaylight = chkAutoDaylight.Checked
        If cmbTimeZones.SelectedItem IsNot Nothing Then oTerminal.TimeZoneName = roTypes.Any2String(cmbTimeZones.SelectedItem.Value).Split("_")(0)

        oTerminal.ConfigurationTable.Tables(0).Select("Name='CaptureImage'")

        If oTerminal.Type.ToLower = "mx6" Or oTerminal.Type.ToLower = "mx7" Or oTerminal.Type.ToLower = "mx8" Or oTerminal.Type.ToLower = "mx9" Then
            Dim bolCaptureImage As Boolean = chkCaptureImage.Checked
            If oTerminal.ConfigurationTable IsNot Nothing AndAlso oTerminal.ConfigurationTable.Tables.Count > 0 Then
                If oTerminal.ConfigurationTable.Tables(0).Rows.Count > 0 Then
                    Dim oRows() As DataRow = oTerminal.ConfigurationTable.Tables(0).Select("Name='CaptureImage'")
                    'If oRows.Length = 1 Then
                    '    oRows(0).Item("Value") = bolCaptureImage
                    'Else 'no hay fila de ese tipo la creamos

                    Dim ds As DataSet = oTerminal.ConfigurationTable
                    Dim tb As DataTable = ds.Tables(0)
                    Dim rowita As DataRow = tb.Rows.Add()
                    rowita("Name") = "CaptureImage"
                    rowita("Value") = bolCaptureImage
                    rowita("Type") = 11
                    tb.AcceptChanges()
                    oTerminal.ConfigurationTable = ds

                    'End If
                Else 'no hay fila de ese tipo la creamos

                    Dim tbRet As New DataTable("Configuration")
                    tbRet.Columns.Add(New DataColumn("Name", GetType(String)))
                    tbRet.Columns.Add(New DataColumn("Type", GetType(Integer)))
                    tbRet.Columns.Add(New DataColumn("Value", GetType(String)))
                    Dim ds As DataSet = New DataSet()
                    ds.Tables.Add(tbRet)

                    Dim rowita As DataRow = tbRet.Rows.Add()
                    rowita("Name") = "CaptureImage"
                    rowita("Value") = bolCaptureImage
                    rowita("Type") = 11
                    tbRet.AcceptChanges()
                    oTerminal.ConfigurationTable = ds

                End If
            End If
        End If

        Dim resultSirens As New Generic.List(Of roTerminalSiren)
        If oParameters.sirens IsNot Nothing AndAlso cmbRelay.SelectedItem IsNot Nothing AndAlso cmbRelay.SelectedItem.Value > 0 Then
            Dim index As Integer = 1
            For Each oJsonSiren As SirensJson In oParameters.sirens
                Dim oSiren As New roTerminalSiren
                oSiren.IDTerminal = oTerminal.ID
                oSiren.ID = index
                oSiren.Hour = New DateTime(1989, 12, 30, oJsonSiren.hour.Split(":")(0), oJsonSiren.hour.Split(":")(1), 0)
                oSiren.Duration = oJsonSiren.duration
                oSiren.WeekDay = oJsonSiren.weekday
                resultSirens.Add(oSiren)
                index = index + 1
            Next
        End If

        oTerminal.Sirens = resultSirens
        oTerminal.Enabled = chkEnabled.Checked
        If oTerminal.Type = "LivePortal" OrElse oTerminal.Type = "NFC" OrElse oTerminal.Type = "Suprema" OrElse oTerminal.Type = "Time Gate" OrElse oTerminal.Type = "Virtual" Then
            oTerminal.Enabled = True
        End If

        If cmbRelay.SelectedItem IsNot Nothing Then oTerminal.SirensOutput = cmbRelay.SelectedItem.Value

        Return oError
    End Function

    Private Function LoadNFCDataFromReaders(ByRef oTerminal As roTerminal) As List(Of TagsJson)

        Dim tagsNFC As New Generic.List(Of TagsJson)

        Dim index As Integer = 1
        For Each oJsonNFCTag As Object In oTerminal.Readers
            Dim oTag As New TagsJson
            oTag.id = oJsonNFCTag.id
            oTag.idzone = oJsonNFCTag.idzone
            oTag.description = oJsonNFCTag.Description
            oTag.nfc = oJsonNFCTag.NfcTagValue
            oTag.zone = oJsonNFCTag.id

            oTag.idmode = roTypes.Any2Integer(API.ZoneServiceMethods.GetZoneByID(Me.Page, roTypes.Any2Integer(oJsonNFCTag.idzone), False).IsWorkingZone)

            If oTag.idmode = 1 Then
                oTag.mode = Me.Language.Translate("TerminalList.Entrada", DefaultScope)
            Else
                oTag.mode = Me.Language.Translate("TerminalList.Salida", DefaultScope)
            End If

            tagsNFC.Add(oTag)
            index = index + 1
        Next

        Return tagsNFC
    End Function

    Private Function LoadZonesJson(ByRef zones As DataTable) As List(Of ZonesJson)

        Dim zonesjson As New Generic.List(Of ZonesJson)

        For Each row As DataRow In zones.Rows
            Dim oTag As New ZonesJson
            oTag.id = row.Item("Id")
            oTag.name = row.Item("Name")
            If row.Item("IsWorkingZone") = True Then
                oTag.zonemode = Me.Language.Translate("TerminalList.Entrada", DefaultScope)
            Else
                oTag.zonemode = Me.Language.Translate("TerminalList.Salida", DefaultScope)
            End If

            zonesjson.Add(oTag)
        Next row

        Return zonesjson
    End Function

    Private Function LoadModesJson() As List(Of ModesJson)

        Dim modesjson As New Generic.List(Of ModesJson)

        Dim oModeEntrada As New ModesJson
        oModeEntrada.id = 1
        oModeEntrada.name = Me.Language.Translate("TerminalList.Entrada", DefaultScope)
        modesjson.Add(oModeEntrada)

        Dim oModeSalida As New ModesJson
        oModeSalida.id = 0
        oModeSalida.name = Me.Language.Translate("TerminalList.Salida", DefaultScope)
        modesjson.Add(oModeSalida)

        Return modesjson
    End Function

    Private Sub loadComboTimeZones(ByVal _TerminalType As String)
        Try

            Dim oTimeZones As Generic.List(Of roTerminalTimeZone) = API.TerminalServiceMethods.GetTimeZones(Me, _TerminalType)

            Me.cmbTimeZones.Items.Clear()
            For Each oTimeZone As roTerminalTimeZone In oTimeZones
                Dim timezoneValue As String = oTimeZone.Name & "_"
                If oTimeZone.SupportsDaylightSavingTime Then
                    timezoneValue = timezoneValue & "1"
                Else
                    timezoneValue = timezoneValue & "0"
                End If
                Me.cmbTimeZones.Items.Add(oTimeZone.DisplayName, timezoneValue)
            Next
        Catch ex As Exception
        End Try
    End Sub

    Private Function CreateSirensGridsJSON(ByVal oCurrentTerminal As roTerminal) As String
        Try

            Dim oJGSirens As New Generic.List(Of Object)
            Dim oJFSirens As Generic.List(Of JSONFieldItem)

            Dim strJSONGroups As String = ""

            If oCurrentTerminal.Sirens IsNot Nothing Then
                If oCurrentTerminal.Sirens.Count > 0 Then
                    For Each oSiren As roTerminalSiren In oCurrentTerminal.Sirens
                        oJFSirens = New Generic.List(Of JSONFieldItem)

                        oJFSirens.Add(New JSONFieldItem("ID", oSiren.ID, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFSirens.Add(New JSONFieldItem("IDTerminal", oSiren.IDTerminal, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFSirens.Add(New JSONFieldItem("WeekDayName", Me.Language.Keyword("weekday." & oSiren.WeekDay), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFSirens.Add(New JSONFieldItem("WeekDay", oSiren.WeekDay, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFSirens.Add(New JSONFieldItem("Hour", Format(oSiren.Hour, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFSirens.Add(New JSONFieldItem("Duration", oSiren.Duration, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJGSirens.Add(oJFSirens)
                    Next
                End If
            End If

            strJSONGroups = "{ ""sirens"": ["

            For Each oObj As Object In oJGSirens
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"": "
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= " } ,"
                strJSONGroups &= strJSONText
            Next

            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "] }"

            Return strJSONGroups
        Catch ex As Exception
            Return ""
        End Try
    End Function

#End Region

#Region "TerminalsList"

    Private Sub ProcessTerminalListRequest(ByVal oParameters As TerminalsCallbackRequest)
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Me.oPermission > Permission.None Then

            Select Case oParameters.Action
                Case "GETTERMINALSLIST"
                    LoadTerminalsListData(oParameters)
                Case "SAVETERMINALSLIST"
                    SaveTerminalList(oParameters)
            End Select

            ProcessTerminalListSelectedTabVisible(oParameters)
        End If
    End Sub

    Private Sub ProcessTerminalListSelectedTabVisible(ByVal oParameters As TerminalsCallbackRequest)
        Me.divList01.Style("display") = "none"
        Me.divList02.Style("display") = "none"

        Select Case oParameters.aTab
            Case 0
                Me.divList01.Style("display") = ""
            Case 1
                Me.divList02.Style("display") = ""
        End Select
    End Sub

    Private Sub SaveTerminalList(ByVal oParameters As TerminalsCallbackRequest)

        Dim oError As roJSON.JSONError = New roJSON.JSONError(False, "")
        Dim oTerminal As roTerminal = Nothing
        Dim oConnectorParameters As roParameters = Nothing
        Try
            If oPermission < Permission.Write Then
                oError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            End If

            oConnectorParameters = API.ConnectorServiceMethods.GetParameters(Me)

            If ConnectorHasChanges(oConnectorParameters) Then
                Dim oParams As New roCollection(oConnectorParameters.ParametersXML)
                Dim bolSaved As Boolean = False

                oParams.Remove(oConnectorParameters.ParametersNames(Parameters.ConnectorDefaultSource))

                If cmbSourceData.SelectedItem IsNot Nothing Then
                    oParams.Add(oConnectorParameters.ParametersNames(Parameters.ConnectorDefaultSource), cmbSourceData.SelectedItem.Value.ToString)
                Else
                    oParams.Add(oConnectorParameters.ParametersNames(Parameters.ConnectorDefaultSource), "")
                End If

                oParams.Remove(oConnectorParameters.ParametersNames(Parameters.ConnectorSourceName))
                oParams.Add(oConnectorParameters.ParametersNames(Parameters.ConnectorSourceName), txtConnectorOriginFileName.Text)
                oParams.Remove(oConnectorParameters.ParametersNames(Parameters.ConnectorReadingsName))
                oParams.Add(oConnectorParameters.ParametersNames(Parameters.ConnectorReadingsName), txtConnectorDestinyFileName.Text)

                oConnectorParameters.ParametersXML = oParams.XML

                If Not oError.Error Then
                    bolSaved = API.ConnectorServiceMethods.SaveParameters(Me, oConnectorParameters, True)

                    If bolSaved Then
                        Robotics.DataLayer.roCacheManager.GetInstance().UpdateParamCache()
                        oError = New roJSON.JSONError(False, "OK")
                    Else
                        oError = New roJSON.JSONError(True, API.ConnectorServiceMethods.LastErrorText)
                    End If
                End If
            End If

            ' Parametros para enlace con Suprema
            Dim supremaConfiguration As SupremaConfigurationParameters = New SupremaConfigurationParameters()
            supremaConfiguration.URL = txtURLSuprema.Text
            supremaConfiguration.Username = txtUserSuprema.Text
            supremaConfiguration.Password = txtPasswordSuprema.Text.Trim
            Dim employeeUserField As String = String.Empty
            If cmbIDEmployeeSuprema.SelectedItem IsNot Nothing Then
                employeeUserField = cmbIDEmployeeSuprema.SelectedItem.Value.ToString
            End If
            supremaConfiguration.EmployeeUserfieldId = employeeUserField
            Dim strDate As String = ""
            Try
                If txtDateInfSuprema.Value IsNot Nothing Then
                    ' Date to UTC
                    Dim fechaUTC As DateTime = DateTime.SpecifyKind(txtDateInfSuprema.Date, DateTimeKind.Utc).ToUniversalTime()
                    strDate = fechaUTC.ToString("yyyy-MM-ddTHH:mm:ss.fffZ")
                End If
            Catch ex As Exception
                strDate = ""
            End Try
            supremaConfiguration.StartDate = strDate

            oError.Error = Not API.TerminalServiceMethods.SaveSupremaConfiguration(Me, supremaConfiguration)

        Catch ex As Exception
            oError.Error = False
        Finally
            LoadTerminalsListData(oParameters, oConnectorParameters)
            ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVETERMINALSLIST"
            If oError IsNot Nothing AndAlso Not oError.Error Then
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "OK"
            Else
                If oError IsNot Nothing Then
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = oError.toJSON()
                Else
                    ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                    ASPxCallbackPanelContenido.JSProperties("cpMessageRO") = New roJSON.JSONError(True, "").toJSON()
                End If
            End If
        End Try

    End Sub

    Private Function ConnectorHasChanges(oConnectorParameters As roParameters) As Boolean

        If oConnectorParameters.ParametersNames(Parameters.ConnectorSourceName).Trim.ToLower() <> txtConnectorOriginFileName.Text.Trim.ToLower() OrElse
                            oConnectorParameters.ParametersNames(Parameters.ConnectorReadingsName).Trim.ToLower() <> txtConnectorDestinyFileName.Text.Trim.ToLower() Then
            Return True
        End If

        Return False
    End Function

    Private Sub LoadTerminalsListData(ByVal oParameters As TerminalsCallbackRequest, Optional ByVal eParameters As roParameters = Nothing)

        Dim oTerminalParameters = Nothing

        If eParameters Is Nothing Then
            oTerminalParameters = API.ConnectorServiceMethods.GetParameters(Me)
        Else
            oTerminalParameters = eParameters
        End If

        Dim oParams As New roCollection(oTerminalParameters.ParametersXML)

        Dim oDisable As Boolean = True
        If oPermission > Permission.Read Then
            oDisable = False
        End If

        ' Parametro ruta configurada por defecto
        cmbSourceData.SelectedItem = cmbSourceData.Items.FindByValue(roTypes.Any2String(oParams.Item(oTerminalParameters.ParametersNames(Parameters.ConnectorDefaultSource))))

        ' Nombre del fichero en origen
        txtConnectorOriginFileName.Text = roTypes.Any2String(oParams.Item(oTerminalParameters.ParametersNames(Parameters.ConnectorSourceName)))

        ' Nombre del fichero en destino
        txtConnectorDestinyFileName.Text = roTypes.Any2String(oParams.Item(oTerminalParameters.ParametersNames(Parameters.ConnectorReadingsName)))

        ' Parámetros Suprema
        Dim supremaConfigurationParameters As Robotics.Base.DTOs.SupremaConfigurationParameters = API.TerminalServiceMethods.GetSupremaConfiguration(Me)
        txtURLSuprema.Value = supremaConfigurationParameters.URL
        txtUserSuprema.Value = supremaConfigurationParameters.Username
        txtPasswordSuprema.Password = True
        Me.hasSupremaPassword.Value = supremaConfigurationParameters.HasPassword.ToString.ToLower
        Dim dateLastRun As Date = supremaConfigurationParameters.LastRun

        Me.imgSupremaLight.Src = "Images/green_light.png"

        If supremaConfigurationParameters.IsActive Then
            Me.lblSupremaStatusDesc.Text = Me.Language.Translate("cksupremaactive.active", Me.DefaultScope)
            If dateLastRun <> Date.MinValue AndAlso (Now.Subtract(dateLastRun).TotalMinutes < 5 * supremaConfigurationParameters.CheckPeriod) Then
                Me.imgSupremaLight.Src = "Images/green_light.png"
                Dim params As New List(Of String)
                params.Add(dateLastRun.ToString("dd/MM/yyyy hh:mm tt", New System.Globalization.CultureInfo(WLHelperWeb.CurrentCulture)))
                Me.lblSupremaStatusDesc.Text = Me.Language.Translate("cksupremaactive.activewithlastrun", Me.DefaultScope, params)
            ElseIf dateLastRun <> Date.MinValue AndAlso dateLastRun <> roTypes.CreateDateTime(1900, 1, 1) Then
                Me.imgSupremaLight.Src = "Images/amber_light.png"
                Dim params As New List(Of String)
                params.Add(dateLastRun.ToString("dd/MM/yyyy hh:mm tt", New System.Globalization.CultureInfo(WLHelperWeb.CurrentCulture)))
                Me.lblSupremaStatusDesc.Text = Me.Language.Translate("cksupremaactive.activedelayed", Me.DefaultScope, params)
            ElseIf dateLastRun = roTypes.CreateDateTime(1900, 1, 1) Then
                Me.imgSupremaLight.Src = "Images/red_light.png"
                Me.lblSupremaStatusDesc.Text = Me.Language.Translate("cksupremaactive.activenonoperational", Me.DefaultScope)
            End If
        Else
            Me.imgSupremaLight.Src = "Images/grey_light.png"
            Me.lblSupremaStatusDesc.Text = Me.Language.Translate("cksupremaactive.inactive", Me.DefaultScope)
        End If

        'If supremaConfigurationParameters.IsActive Then
        '    Me.ckSupremaActive.Checked = True
        '    Me.ckSupremaActive.Text = Me.Language.Translate("cksupremaactive.active", Me.DefaultScope)
        '    If dateLastRun <> Date.MinValue AndAlso (Now.Subtract(dateLastRun).TotalMinutes < 5 * supremaConfigurationParameters.CheckPeriod) Then
        '        Dim params As New List(Of String)
        '        params.Add(dateLastRun.ToString("dd/MM/yyyy hh:mm tt", New System.Globalization.CultureInfo(WLHelperWeb.CurrentCulture)))
        '        Me.ckSupremaActive.Text = Me.Language.Translate("cksupremaactive.activewithlastrun", Me.DefaultScope, params)
        '    ElseIf dateLastRun <> Date.MinValue Then
        '        Dim params As New List(Of String)
        '        params.Add(dateLastRun.ToString("dd/MM/yyyy hh:mm tt", New System.Globalization.CultureInfo(WLHelperWeb.CurrentCulture)))
        '        Me.ckSupremaActive.Text = Me.Language.Translate("cksupremaactive.activenonoperational", Me.DefaultScope, params)
        '    End If
        'Else
        '    Me.ckSupremaActive.Checked = False
        '    Me.ckSupremaActive.Text = Me.Language.Translate("cksupremaactive.inactive", Me.DefaultScope)
        'End If

        Try
            Dim strFieldID As String = supremaConfigurationParameters.EmployeeUserfieldId.ToString
            For i As Integer = 0 To Me.cmbIDEmployeeSuprema.Items.Count - 1
                If Me.cmbIDEmployeeSuprema.Items(i).Value = strFieldID Then
                    Me.cmbIDEmployeeSuprema.SelectedIndex = i
                    Exit For
                End If
            Next
        Catch ex As Exception
            cmbIDEmployeeSuprema.SelectedIndex = 0
        End Try

        Try
            Dim strDate As String = supremaConfigurationParameters.StartDate
            Dim strformat As String = "yyyy-MM-ddTHH:mm:ss.fffZ"
            Dim SupremaInitialDate As DateTime = DateTime.ParseExact(strDate, strformat, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.RoundtripKind)
            txtDateInfSuprema.Value = SupremaInitialDate
        Catch ex As Exception
            txtDateInfSuprema.Value = Nothing
        End Try

        Dim IDZones As New Generic.List(Of Integer)
        IDZones.Add(oParameters.ID)

        Dim oTerminalList As DataTable = API.TerminalServiceMethods.GetTerminalsLiveStatus(Me.Page, "", True)
        Dim tbTerminalTypes As DataTable = API.TerminalServiceMethods.GetTerminalTypes(Me.Page, "web")

        Dim oTerminalReader2Row As DataRow = Nothing

        For Each oTerminal As DataRow In oTerminalList.Rows

            If roTypes.Any2Integer(oTerminal("IDReader")) = 1 Then
                Dim dtRows As DataRow() = oTerminalList.Select("ID=" & oTerminal("ID") & " And IDReader = 2")
                If dtRows.Count > 0 Then
                    oTerminalReader2Row = dtRows(0)
                Else
                    oTerminalReader2Row = Nothing
                End If

                createRowLineTerminal(oTerminal, oTerminalReader2Row, tbTerminalTypes)
                oTerminalReader2Row = Nothing
            End If
        Next

        ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETTERMINALSLIST")
        ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")

    End Sub

    Private Sub createRowLineTerminal(ByVal oTerminal As DataRow, ByVal oTerminalReader2 As DataRow, ByVal tbTerminalTypes As DataTable)
        Try
            Dim strTerminalType = roTypes.Any2String(oTerminal("Type"))

            ' Determinamos si se trata de un terminal tipo web
            Dim bolWeb As Boolean = False
            If tbTerminalTypes IsNot Nothing Then
                Dim dRowT As DataRow()
                dRowT = tbTerminalTypes.Select("Type = '" & strTerminalType.Replace("'", "''") & "'")
                If dRowT.Length > 0 Then
                    bolWeb = True
                End If
            End If

            Dim bVirtual As Boolean
            bVirtual = (strTerminalType.ToUpper = "VIRTUAL")

            Dim htRow As New HtmlTableRow
            Dim htCell As New HtmlTableCell
            htCell.Attributes("class") = ""
            htCell.Attributes("style") = "background-position: -1; border-bottom: solid 1px silver;"
            htCell.InnerText = " "
            htRow.Cells.Add(htCell)

            htCell = New HtmlTableCell
            htCell.VAlign = "top"
            htCell.Attributes("style") = "height: 100px; border-bottom: solid 1px silver;"

            Dim htTable As New HtmlTable
            Dim hRowT As HtmlTableRow
            Dim hCellT As HtmlTableCell

            htTable.Border = "0"
            htTable.Attributes("style") = If(roTypes.Any2Boolean(oTerminal("Enabled")), "width: 100%;", "width: 100%; background-color: gainsboro;")

            hRowT = New HtmlTableRow
            hCellT = New HtmlTableCell
            hCellT.Attributes("style") = "width: 100px;"

            Dim pathImg As String = "Images/TerminalIcos/"
            Dim strImg As String = "BLANK"

            If strTerminalType <> "" Then strImg = strTerminalType
            If roTypes.Any2String(oTerminal("LastStatus")).ToUpper <> "OK" Then strImg &= "_DIS"

            'Permisos Info. Estat
            If oPermissionStatus = Permission.None Then strImg = strImg.Replace("_DIS", "")

            'Inserim la imatge del terminal
            Dim hImg As New HtmlImage
            hImg.Src = pathImg & strImg & ".png"
            hImg.Attributes("style") = "cursor: pointer;"

            'Permisos edicio Terminal
            If oPermission > Permission.None Then
                hImg.Attributes("onclick") = "ConfigureTerminal('" & oTerminal("ID") & "')"
            End If

            hCellT.Controls.Add(hImg)
            hRowT.Cells.Add(hCellT)

            hCellT = New HtmlTableCell
            hCellT.VAlign = "middle"
            hCellT.Attributes("style") = "padding-top: 0px; padding-left: 5px;"

            'Taula ID
            Dim hTableID As New HtmlTable
            Dim hRowID As New HtmlTableRow
            Dim hCellID As New HtmlTableCell
            hTableID.Width = "350px"
            hTableID.Height = "auto"
            hTableID.Border = "0"

            hCellID.ColSpan = "2"
            Dim spanTerminalName As New HtmlGenericControl("SPAN")
            spanTerminalName.Attributes("style") = "font-size: 16px; font-weight: bold;"
            spanTerminalName.InnerHtml = oTerminal("Description")
            hCellID.Controls.Add(spanTerminalName)

            'Nom del terminal
            hRowID.Cells.Add(hCellID)
            hTableID.Rows.Add(hRowID)

            hRowID = New HtmlTableRow
            hCellID = New HtmlTableCell

            Dim spanID As New HtmlGenericControl("SPAN")
            spanID.Attributes("style") = "font-weight: bold;"
            spanID.InnerHtml = Me.Language.Translate("TerminalList.ID", DefaultScope)
            hCellID.Controls.Add(spanID)
            hRowID.Cells.Add(hCellID)

            hCellID = New HtmlTableCell
            hCellID.InnerHtml = oTerminal("ID")
            hRowID.Cells.Add(hCellID)
            hTableID.Rows.Add(hRowID)

            hRowID = New HtmlTableRow
            hCellID = New HtmlTableCell

            If Not bolWeb Then
                Dim spanIP As New HtmlGenericControl("SPAN")
                spanIP.Attributes("style") = "font-weight: bold;"
                spanIP.InnerHtml = Me.Language.Translate("TerminalList.IP", DefaultScope)
                hCellID.Controls.Add(spanIP)
            End If
            hRowID.Cells.Add(hCellID)

            hCellID = New HtmlTableCell
            If Not bolWeb Then
                hCellID.InnerHtml = oTerminal("Location")
            End If
            hRowID.Cells.Add(hCellID)

            hTableID.Rows.Add(hRowID)

            hRowID = New HtmlTableRow
            hCellID = New HtmlTableCell

            If Not bolWeb AndAlso Not bVirtual Then
                Dim spanIP As New HtmlGenericControl("SPAN")
                spanIP.Attributes("style") = "font-weight: bold;"
                spanIP.InnerHtml = Me.Language.Translate("TerminalList.FirmwareVersion", DefaultScope)
                hCellID.Controls.Add(spanIP)
            End If
            hRowID.Cells.Add(hCellID)

            hCellID = New HtmlTableCell
            If Not bolWeb AndAlso Not bVirtual Then
                hCellID.InnerHtml = If(roTypes.Any2String(oTerminal("FirmVersion")).Trim <> String.Empty, oTerminal("FirmVersion"), Me.Language.Translate("TerminalList.FirmwareUnknown", Me.DefaultScope))
            End If
            hRowID.Cells.Add(hCellID)

            hTableID.Rows.Add(hRowID)

            hRowID = New HtmlTableRow
            hCellID = New HtmlTableCell

            If Not bolWeb AndAlso Not bVirtual Then
                Dim spanIP As New HtmlGenericControl("SPAN")
                spanIP.Attributes("style") = "font-weight: bold;"
                spanIP.InnerHtml = Me.Language.Translate("TerminalList.SerialNumber", DefaultScope)
                hCellID.Controls.Add(spanIP)
            End If
            hRowID.Cells.Add(hCellID)

            hCellID = New HtmlTableCell
            If Not bolWeb AndAlso Not bVirtual Then
                hCellID.InnerHtml = If(roTypes.Any2String(oTerminal("SerialNumber")).Trim <> String.Empty, oTerminal("SerialNumber"), Me.Language.Translate("TerminalList.FirmwareUnknown", Me.DefaultScope))
            End If
            hRowID.Cells.Add(hCellID)

            hTableID.Rows.Add(hRowID)

            hRowID = New HtmlTableRow
            hCellID = New HtmlTableCell

            If Not bolWeb AndAlso Not bVirtual Then
                If oPermissionStatus > Permission.None Then
                    Dim spanStatus As New HtmlGenericControl("SPAN")
                    spanStatus.Attributes("style") = "font-weight: bold;"
                    spanStatus.InnerHtml = Me.Language.Translate("TerminalList.Status", DefaultScope)
                    hCellID.Controls.Add(spanStatus)
                End If
            End If
            hRowID.Cells.Add(hCellID)

            hCellID = New HtmlTableCell
            If Not bolWeb AndAlso Not bVirtual Then
                If oPermissionStatus > Permission.None Then
                    Dim spanState As New HtmlGenericControl("SPAN")
                    If roTypes.Any2String(oTerminal("LastStatus")).ToUpper() = "OK" Then
                        spanState.Attributes("style") = "font-weight: bold; color: green;"
                        spanState.InnerHtml = Me.Language.Translate("TerminalList.Connected", DefaultScope)
                    Else
                        spanState.Attributes("style") = "font-weight: bold; color: red;"
                        spanState.InnerHtml = Me.Language.Translate("TerminalList.NotConnected", DefaultScope)
                    End If
                    If Not IsDBNull(oTerminal("LastUpdate")) Then spanState.Attributes("title") = roTypes.Any2DateTime(oTerminal("LastUpdate")).ToLongDateString

                    'Label de estat
                    hCellID.Controls.Add(spanState)
                End If
            End If
            hRowID.Cells.Add(hCellID)
            hTableID.Rows.Add(hRowID)

            hRowID = New HtmlTableRow
            hCellID = New HtmlTableCell
            hCellID.Align = "right"
            hCellID.Attributes("style") = "height:auto; padding-top: 10px;"
            hCellID.VAlign = "bottom"

            'hCellID.Controls.Add(aTerminalCfg)
            hRowID.Cells.Add(hCellID)
            hTableID.Rows.Add(hRowID)

            'Afegim la taula de ids
            hCellT.Controls.Add(hTableID)
            hRowT.Cells.Add(hCellT)

            'htTable.Rows.Add(hRowT)

            'hRowT = New HtmlTableRow
            hCellT = New HtmlTableCell
            hCellT.VAlign = "top"
            hCellT.Attributes("style") = "width: auto; padding-top: 5px; padding-left: 5px;"
            hCellT.Align = "right"

            If oPermissionStatus > Permission.None Then
                hCellT.InnerHtml = "<div class=""RoundCornerFrame roundCorner"" id=""terminalStatusDiv" & oTerminal("ID") & """style=""width:375px;background-color:lightgray"">" &
                                        "<div class=""btnFlat"">" &
                                            "<a href=""javascript: void(0)"" id=""btnEditType"" runat=""server"" onclick=""refreshTerminalStatus(" & oTerminal("ID") & ")"">" &
                                                "<span ID=""lblEditType"" >" & Me.Language.Translate("TerminalList.RefreshTerminalData", Me.DefaultScope) & "</span>" &
                                            "</a>" &
                                        "</div>" &
                                  "</div>"
            Else
                hCellT.InnerHtml = "<div class=""RoundCornerFrame roundCorner"" style=""width:375px;background-color:lightgray"">" &
                                  "</div>"
            End If

            If oPermission > Permission.None Then
                Dim strCams As String = "<td>&nbsp;</td>"

                If Not IsDBNull(oTerminal("IDCamera")) Then
                    strCams &= "<td width=""10px""><a href=""TerminalviewCam.aspx?ID=" & oTerminal("IDCamera") & """ class=""aTerminalViewCam"" target=""_blank"">" & roTypes.Any2String(oTerminal("IDCamera")) & "</a></td>"
                End If

                If oTerminalReader2 IsNot Nothing AndAlso Not IsDBNull(oTerminalReader2("IDCamera")) Then
                    strCams &= "<td width=""10px""><a href=""TerminalviewCam.aspx?ID=" & oTerminalReader2("IDCamera") & """ class=""aTerminalViewCam"" target=""_blank"">" & roTypes.Any2String(oTerminalReader2("IDCamera")) & "</a></td>"
                End If

                hCellT.InnerHtml &= "<table border=""0"">" &
                                    "<tr>" & strCams & "<td><a href=""javascript: void(0);"" class=""aTerminalCfg"" onclick=""ConfigureTerminal('" & oTerminal("ID") & "')"">" & Me.Language.Translate("TerminalList.Configuration", DefaultScope) & "</a></td></tr>" &
                                    "</table>"
            End If

            hRowT.Cells.Add(hCellT)
            htTable.Rows.Add(hRowT)

            'Afegim la taula amb tot el contingut
            htCell.Controls.Add(htTable)
            htRow.Cells.Add(htCell)

            'Afegim a la taula final
            Me.tblTerminals.Rows.Add(htRow)

            htRow = New HtmlTableRow
            htCell = New HtmlTableCell
            htCell.VAlign = "top"
            htCell.Attributes("style") = "width: auto; padding-top: 5px; padding-left: 5px;"
            htCell.Align = "right"

            Me.tblTerminals.Rows.Add(htRow)
        Catch ex As Exception
            Response.Write(ex.Message & " " & ex.StackTrace)
        End Try

    End Sub

#End Region

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.ToUpper()
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", True)
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.GenerateTerminalUSBFile()
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then

                    Dim oSettings As New Robotics.VTBase.roSettings
                    Dim sTerminalPath As String = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" & CurrentTerminalID.ToString
                    sTerminalPath = IO.Path.Combine(sTerminalPath, "USB")
                    sTerminalPath = IO.Path.Combine(sTerminalPath, "Terminal" + CurrentTerminalID.ToString.PadLeft(3, "0") + ".config")

                    If API.LiveTasksServiceMethods.FileExists(Me.Page, sTerminalPath) Then
                        PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                        PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                        HttpContext.Current.Session("ExportFileName") = sTerminalPath
                    Else
                        PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                        PerformActionCallback.JSProperties.Add("cpActionResult", "")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                End If
        End Select

    End Sub

    Protected Function GenerateTerminalUSBFile() As Integer
        Try
            Dim sSystemPath As String = ""
            Try
                Dim oSettings As New Robotics.VTBase.roSettings
                Dim sTerminalPath As String = oSettings.GetVTSetting(eKeys.Readings) & "\Terminal" & CurrentTerminalID.ToString
                sTerminalPath = IO.Path.Combine(sTerminalPath, "USB")
                sTerminalPath = IO.Path.Combine(sTerminalPath, "Terminal" + CurrentTerminalID.ToString.PadLeft(3, "0") + ".config")

                Dim bolError As Boolean = False

                If API.LiveTasksServiceMethods.FileExists(Me.Page, sTerminalPath) Then
                    If Not API.LiveTasksServiceMethods.RemoveFile(Me.Page, sTerminalPath) Then
                        bolError = True
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = Me.Language.Translate("Error.NotDeletedFile", Me.DefaultScope)
                    End If
                End If

                If Not bolError Then
                    If ConnectorServiceMethods.LaunchBroadcasterForTerminalTask(Me.Page, CurrentTerminalID, "CreateUSB") Then
                        Return 1
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = Me.Language.Translate("Error.NotGeneratingFile", Me.DefaultScope)
                    End If
                End If
            Catch ex As Exception
            End Try
        Catch ex As Exception
            iCurrentTask = -1
        End Try

        Return iCurrentTask
    End Function

    Protected Sub btUSBdownSoft_Click(sender As Object, e As EventArgs) Handles btUSBdownSoft.Click
        Try
            Dim sSystemPath As String = ""
            Try
                'Obtenemos el path del terminal
                ' Miramos si es una máquina de 64 bits para buscar en el registro correctamente
                Dim oSettings As New Robotics.VTBase.roSettings
                Dim _RegistryRoot As String = "HKEY_LOCAL_MACHINE\Software\"
                If Microsoft.Win32.Registry.GetValue("HKEY_LOCAL_MACHINE\Software\Wow6432node\Robotics\VisualTime\Server", "Running", "False") <> Nothing Then
                    _RegistryRoot = "HKEY_LOCAL_MACHINE\Software\Wow6432node\"
                End If
                oSettings = New Robotics.VTBase.roSettings(_RegistryRoot & "Robotics\VisualTime")

                sSystemPath = oSettings.GetVTSetting(eKeys.System)
            Catch ex As Exception
            End Try
            If Not Me.DownloadFile(IO.Path.Combine(sSystemPath, "USBTerminal.zip")) Then
                Session("showNoUSBFile") = "true"
            End If
        Catch ex As Exception

        End Try
    End Sub

    Protected Sub btUSBuploadFile_Click(sender As Object, e As EventArgs) Handles btUSBuploadFile.Click
        If USBFileUpload.HasFile Then
        Else
            ClientScript.RegisterStartupScript(Me.GetType(), "showerro", " showErrorPopup('Error.ValidationTitle', 'error', 'Error.ValidationFieldsFailed', '', 'Error.OK', 'Error.OKDesc', '');")
        End If
    End Sub

End Class