Imports System.IO
Imports System.Net.Http
Imports System.Net.Http.Headers
Imports System.Security
Imports DevExpress.Web
Imports Newtonsoft.Json
Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTServiceApi
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base
Imports ServiceApi

Partial Class WebUserControls_WSAdministration
    Inherits UserControlBase

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("WEAdmin_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("WEAdmin_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("WEAdmin_iCurrentTask") = value
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

    Private ReadOnly Property UrlApiSC As String
        Get
            Dim ASSCConnectionString = Robotics.VTBase.roTypes.Any2String(Robotics.VTBase.roConstants.GetConfigurationParameter("ApiService.ConnectionString"))
            If ASSCConnectionString.Split("@").Length = 3 Then
                Return ASSCConnectionString.Split("@")(0) + "/"
            Else
                Return Nothing
            End If
        End Get
    End Property

#End Region

#Region "OBTENCION DE DATOS DE LINQ"

    Private Sub EmptyDataAudit()
        Session("ImportAudit_AnaliticsData") = Nothing
    End Sub

    Private Function GetAuditData(Optional ByVal bolReload As Boolean = False) As Object
        Return Session("ImportAudit_AnaliticsData")
    End Function

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Dim cacheManager As New NoCachePageBase
        cacheManager.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js", Me.Parent.Page)
        cacheManager.InsertExtraJavascript("securityIPeditor", "~/Security/Scripts/SecurityIPEditor.js", Me.Parent.Page)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.IsPostBack Then
            CreateColumnsAudit()
            LoadActionsCombo(True)
            EmptyDataAudit()
        Else
            LoadActionsCombo(False)
        End If

        grdAudit.DataSource = GetAuditData()
        grdAudit.FilterExpression = String.Empty
        grdAudit.DataBind()

        Dim myByte As Byte() = System.Text.Encoding.UTF8.GetBytes(HelperWeb.GetCookie("Login_CompanyName"))
        'Dim myByte As Byte() = System.Text.Encoding.UTF8.GetBytes("brico3869")
        Dim myBase64 As String = Convert.ToBase64String(myByte)

        Me.hdnCompanyName.Value = myBase64

        Me.btnAddCertificate.Attributes("onclick") = ClientID & "_AddNewCertificate();"
        Me.NewCertificatePopup.ClientInstanceName = ClientID & "_NewCertificatePopup"
        Me.btnDownloadCertificate.Attributes("onclick") = ClientID & "_DownloadCertificate();"
        Dim oLicense As New roServerLicense
        If oLicense.FeatureIsInstalled("Feature\BIIntegration") Then
            Me.divBIInformation.Visible = True
        Else
            Me.divBIInformation.Visible = False
        End If
    End Sub

#End Region

#Region "WS audit"

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.ToUpper()
            Case "PERFORM_ACTION"
                EmptyDataAudit()
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.ExecuteBackgroundAnalytic()
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case roLiveTaskStatus.All, roLiveTaskStatus.Stopped
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                            Case roLiveTaskStatus.Running
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                                HttpContext.Current.Session("ExportFileName") = oTask.ErrorCode

                                If Session("ImportAudit_AnaliticsData") Is Nothing Then
                                    Dim oAnalyticFile As Byte()

                                    oAnalyticFile = API.LiveTasksServiceMethods.DownloadFileAzure(Me.Page, oTask.ErrorCode, roLiveQueueTypes.analytics)

                                    API.LiveTasksServiceMethods.RemoveCompletedTask(Me.Page, iCurrentTask)

                                    Dim objAnalytic As Generic.List(Of Analytics_Audit) = roTypes.File2Any(Of Robotics.Base.DTOs.Analytics_Audit)(oAnalyticFile)

                                    Session("ImportAudit_AnaliticsData") = objAnalytic.FindAll(Function(x) x.ElementID = Robotics.VTBase.Audit.ObjectType.tDatalinkWS)
                                End If
                            Case roLiveTaskStatus.Finished
                                PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "KO")
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                End If
        End Select

    End Sub

    Private Function ExecuteBackgroundAnalytic() As Integer
        Dim iTask As Integer = -1
        Dim bolFileExport As Boolean = False

        Try
            Try
                Dim DateInf As DateTime = New DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.AddDays(-7).Day, 0, 0, 0)
                Dim DateSup As DateTime = New DateTime(DateTime.Now.Date.Year, DateTime.Now.Date.Month, DateTime.Now.Day, 23, 59, 59)

                If DateInf > DateSup Then
                    Dim aux As DateTime = DateSup
                    DateSup = DateInf
                    DateInf = aux
                End If

                iTask = API.LiveTasksServiceMethods.CreateAnalyticTask(Me.Page, roLiveAnalyticType.Audit, 1, 1, WLHelperWeb.CurrentPassportID, DateInf, DateSup, String.Empty, String.Empty, String.Empty, String.Empty, False, False, True, String.Empty, String.Empty)
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try

        Return iTask
    End Function

    Private Sub CreateColumnsAudit()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCombo As GridViewDataComboBoxColumn
        Dim GridColumnDate As GridViewDataDateColumn
        Dim GridColumnImage As GridViewDataImageColumn

        Dim VisibleIndex As Integer = 0

        Me.grdAudit.Settings.ShowTitlePanel = True
        Me.grdAudit.Columns.Clear()
        Me.grdAudit.KeyFieldName = "ID"
        Me.grdAudit.SettingsText.EmptyDataRow = " "
        Me.grdAudit.Settings.VerticalScrollBarMode = ScrollBarMode.Auto
        Me.grdAudit.SettingsPager.PageSize = 20

        Me.grdAudit.SettingsEditing.Mode = GridViewEditingMode.PopupEditForm

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        Me.grdAudit.Columns.Add(GridColumn)

        'Imagen
        GridColumnImage = New GridViewDataImageColumn
        GridColumnImage.Caption = " "
        GridColumnImage.FieldName = "ActionImage"
        GridColumnImage.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnImage.ReadOnly = True
        GridColumnImage.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnImage.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnImage.Width = 16
        GridColumnImage.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumnImage.PropertiesImage.ImageAlign = ImageAlign.Middle
        GridColumnImage.PropertiesImage.ImageHeight = 16
        GridColumnImage.PropertiesImage.ImageWidth = 16
        GridColumnImage.PropertiesImage.ImageUrlFormatString = "{0}"
        Me.grdAudit.Columns.Add(GridColumnImage)

        'Action
        GridColumnCombo = New GridViewDataComboBoxColumn()
        GridColumnCombo.Caption = Me.Language.Translate("Audit.Column.Action", DefaultScope)
        GridColumnCombo.FieldName = "ActionID"
        GridColumnCombo.VisibleIndex = VisibleIndex
        GridColumnCombo.PropertiesComboBox.DataSource = LoadActionsCombo()
        GridColumnCombo.PropertiesComboBox.TextField = "ActionDesc"
        GridColumnCombo.PropertiesComboBox.ValueField = "ActionId"
        GridColumnCombo.PropertiesComboBox.ValueType = GetType(Integer)
        GridColumnCombo.PropertiesComboBox.ClearButton.DisplayMode = ClearButtonDisplayMode.Always
        VisibleIndex = VisibleIndex + 1
        GridColumnCombo.ReadOnly = True
        GridColumnCombo.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnCombo.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnCombo.Width = 35

        Me.grdAudit.Columns.Add(GridColumnCombo)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("Audit.Column.Date", DefaultScope)
        GridColumnDate.FieldName = "Date"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = True
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.Width = 25
        GridColumnDate.PropertiesDateEdit.DisplayFormatString = "dd/MM/yyyy"
        GridColumn.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False
        'GridColumnDate.SortIndex = 0
        'GridColumnDate.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        Me.grdAudit.Columns.Add(GridColumnDate)

        'Hora
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("Audit.Column.Time", DefaultScope)
        GridColumn.FieldName = "Hour"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False
        'GridColumn.SortIndex = 1
        'GridColumn.SortOrder = DevExpress.Data.ColumnSortOrder.Ascending
        GridColumn.Width = 30
        Me.grdAudit.Columns.Add(GridColumn)

        'Description
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.Description", DefaultScope)
        GridColumn.FieldName = "Message"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Settings.FilterMode = ColumnFilterMode.DisplayText
        GridColumn.Width = 150
        GridColumn.Settings.AllowSort = DevExpress.Utils.DefaultBoolean.False
        GridColumn.Settings.AllowAutoFilter = DevExpress.Utils.DefaultBoolean.False
        Me.grdAudit.Columns.Add(GridColumn)

        'ClientLocation
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("GridContracts.Column.ClientLocation", DefaultScope)
        GridColumn.FieldName = "ClientLocation"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.CellStyle.VerticalAlign = VerticalAlign.Top
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Settings.AutoFilterCondition = AutoFilterCondition.Contains
        GridColumn.Width = 30
        Me.grdAudit.Columns.Add(GridColumn)

    End Sub

    Private Sub grdAudit_DataBinding(sender As Object, e As EventArgs) Handles grdAudit.DataBinding
        Dim oCombo As GridViewDataComboBoxColumn = grdAudit.Columns("ActionID")
        oCombo.PropertiesComboBox.DataSource = Me.LoadActionsCombo()
        oCombo.PropertiesComboBox.TextField = "ActionDesc"
        oCombo.PropertiesComboBox.ValueField = "ActionId"
        oCombo.PropertiesComboBox.ValueType = GetType(Short)
    End Sub

    Private Function LoadActionsCombo(Optional ByVal bolReload As Boolean = False) As DataView

        Dim tb As DataTable = Session("WSAudit_AuditDataActions")
        Dim dv As DataView = Nothing

        If bolReload OrElse tb Is Nothing Then
            tb = API.AuditServiceMethods.GetAuditActions(Me.Page)
            If tb IsNot Nothing Then
                If tb.Rows.Count = 0 Then
                    Dim oNew As DataRow = tb.NewRow
                    oNew("ActionId") = -1
                    oNew("ActionDesc") = ""
                    tb.Rows.Add(oNew)
                End If
                Session("WSAudit_AuditDataActions") = tb
                dv = New DataView(tb)
                dv.Sort = "ActionDesc ASC"
            End If
            Return dv
        Else
            dv = New DataView(tb)
            dv.Sort = "ActionDesc ASC"
            Return dv
        End If
    End Function

    Public Function ActionImage(ByVal oAction As Object) As String
        Dim strRet As String = Me.Page.ResolveUrl("~/Base/Images/Transparencia.gif")
        If Not IsDBNull(oAction) Then
            strRet = "Images/" & System.Enum.GetName(GetType(Robotics.VTBase.Audit.Action), oAction) & ".gif"
            Select Case CType(oAction, Robotics.VTBase.Audit.Action)
                Case Robotics.VTBase.Audit.Action.aConnect
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aConnect.gif")
                Case Robotics.VTBase.Audit.Action.aDisconnect
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aDisconnect.gif")
                Case Robotics.VTBase.Audit.Action.aSelect
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aSelect.gif")
                Case Robotics.VTBase.Audit.Action.aMultiSelect
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aMultiSelect.gif")
                Case Robotics.VTBase.Audit.Action.aInsert
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aInsert.gif")
                Case Robotics.VTBase.Audit.Action.aUpdate
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aUpdate.gif")
                Case Robotics.VTBase.Audit.Action.aDelete
                    strRet = Me.Page.ResolveUrl("~/Audit/Images/aDelete.gif")
            End Select
        End If
        Return strRet
    End Function

    Public Function ObjectTypeImage(ByVal oObjectType As Object) As String
        Dim strRet As String = Me.Page.ResolveUrl("~/Base/Images/Transparencia.gif")
        If Not IsDBNull(oObjectType) Then
            If Not System.Enum.GetName(GetType(Robotics.VTBase.Audit.ObjectType), oObjectType) Is Nothing Then
                strRet = Me.Page.ResolveUrl("~/Audit/Images/" & System.Enum.GetName(GetType(Robotics.VTBase.Audit.ObjectType), oObjectType) & ".gif")
            End If
        End If
        Return strRet
    End Function

    Protected Sub grdAudit_CustomCallback(sender As Object, e As ASPxGridViewCustomCallbackEventArgs) Handles grdAudit.CustomCallback
        If e.Parameters = "REFRESH" Then
            CreateColumnsAudit()
            grdAudit.DataSource = GetAuditData(True)
            grdAudit.FilterExpression = String.Empty
            grdAudit.DataBind()
        End If
    End Sub

    Protected Sub grdAudit_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles grdAudit.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "ActionImage"
                If e.IsGetData Then
                    If e.GetListSourceFieldValue("ActionID") IsNot System.DBNull.Value Then
                        e.Value = Me.Page.ResolveUrl("~/Base/Images/Transparencia.gif")
                        e.Value = Me.Page.ResolveUrl("~/Audit/Images/" & System.Enum.GetName(GetType(Robotics.VTBase.Audit.Action), e.GetListSourceFieldValue("ActionID")) & ".gif")
                        Select Case CType(e.GetListSourceFieldValue("ActionID"), Robotics.VTBase.Audit.Action)
                            Case Robotics.VTBase.Audit.Action.aConnect
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aConnect.gif")
                            Case Robotics.VTBase.Audit.Action.aDisconnect
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aDisconnect.gif")
                            Case Robotics.VTBase.Audit.Action.aSelect
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aSelect.gif")
                            Case Robotics.VTBase.Audit.Action.aMultiSelect
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aMultiSelect.gif")
                            Case Robotics.VTBase.Audit.Action.aInsert
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aInsert.gif")
                            Case Robotics.VTBase.Audit.Action.aUpdate
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aUpdate.gif")
                            Case Robotics.VTBase.Audit.Action.aDelete
                                e.Value = Me.Page.ResolveUrl("~/Audit/Images/aDelete.gif")
                        End Select
                    End If
                End If
            Case "Hour"
                If Not IsDBNull(e.GetListSourceFieldValue("Date")) Then
                    e.Value = CType(e.GetListSourceFieldValue("Date"), Date).ToString("HH:mm:ss")
                End If
            Case "Message"
                If e.IsGetData Then
                    Dim tb As DataTable = Nothing
                    If Not IsDBNull(e.GetListSourceFieldValue("MessageParameters")) AndAlso e.GetListSourceFieldValue("MessageParameters") <> "" Then
                        tb = roConversions.XmlDeserialize(e.GetListSourceFieldValue("MessageParameters"), GetType(DataTable))
                    End If

                    e.Value = GetMessage(e, tb)
                End If
        End Select

    End Sub

    Protected Function GetMessage(ByVal e As ASPxGridViewColumnDataEventArgs, ByVal tbParameters As DataTable) As String

        Dim MessageKey As String = "Message." & System.Enum.GetName(GetType(Robotics.VTBase.Audit.ObjectType), CType(e.GetListSourceFieldValue("ElementID"), Robotics.VTBase.Audit.ObjectType)) & "." &
                                     System.Enum.GetName(GetType(Robotics.VTBase.Audit.Action), CType(e.GetListSourceFieldValue("ActionID"), Robotics.VTBase.Audit.Action))

        Dim strMessage As String = Me.Language.Translate(MessageKey, "Audit")

        Const TOKEN_PATTERN = "\$@[a-zA-Z.]+@"

        ' Reemplazo tokens por valores pasados por parámetro
        strMessage = Regex.Replace(strMessage, TOKEN_PATTERN, New MatchEvaluator(Function(match As Match) CustomMatchEvaulator(match, tbParameters)))

        Return strMessage

    End Function

    Private Function CustomMatchEvaulator(ByVal m As Match, ByVal dtParameters As DataTable) As String
        Return Me.ResolveParameter("{" & m.Value.Substring(2, m.Value.Length - 3) & "}", "", dtParameters)
    End Function

    Private Function ResolveParameter(ByVal strName As String, ByVal strParentName As String, ByVal dtParameters As DataTable) As String

        Dim strRet As String = ""

        If dtParameters IsNot Nothing AndAlso dtParameters.Rows.Count > 0 Then

            Dim strParamName As String
            Dim strChilds As String = ""
            Dim oParams() As DataRow = dtParameters.Select("ParamName = '" & strName & "' AND ParamParent = '" & strParentName & "'", "Priority")
            Dim oChildParams() As DataRow
            For Each oParam As DataRow In oParams

                strParamName = oParam("ParamName")
                If Not strParamName.StartsWith("{") Then
                    If strParamName.StartsWith("?") Then
                        strRet &= Me.Language.Translate("Parameters." & strParamName.Substring(1), "Audit") & ":"
                    Else
                        strRet &= strParamName & ":"
                    End If
                End If
                strRet &= oParam("ParamValue")

                oChildParams = dtParameters.Select("ParamParent = '" & strParamName & "'", "Priority")
                If oChildParams.Length > 0 Then
                    If strRet <> "" Then strRet = vbCrLf & strRet & vbCrLf
                    strChilds = ""
                    Dim oChild As DataRow
                    Dim strValue As String = ""
                    For nChild As Integer = 0 To oChildParams.Length - 1
                        oChild = oChildParams(nChild)
                        strValue = Me.ResolveParameter(oChild("ParamName"), oChild("ParamParent"), dtParameters)
                        If nChild > 0 Then
                            If Not strValue.StartsWith(vbCrLf) Then
                                strChilds &= " -> "
                            End If
                        End If
                        strChilds &= strValue
                    Next

                    While strChilds.EndsWith(" -> ")
                        strChilds = strChilds.Substring(0, strChilds.Length - 4)
                    End While

                    If strChilds <> "" Then
                        strRet &= strChilds
                    End If
                End If

            Next

            If strParentName = "" And strRet.StartsWith(vbCrLf) Then
                strRet = strRet.Substring(2)
            End If

        End If

        Return strRet

    End Function

    Public Sub InitComponents()
        Me.txtWSUserName.Text = HelperSession.AdvancedParametersCache(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessUserName.ToString(), True)
        Me.txtWSToken1.Text = HelperSession.AdvancedParametersCache(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken1.ToString(), True)
        Me.txtWSToken2.Text = HelperSession.AdvancedParametersCache(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessToken2.ToString(), True)
        Me.txtAllowedIPs.Value = API.CommonServiceMethods.GetAdvancedParameter(Nothing, Robotics.Base.DTOs.AdvancedParameterType.ExternAccessIPs.ToString()).Value

        Me.txtWSSCClientName.Text = Robotics.Azure.RoAzureSupport.GetCompanyName()
        Me.txtDISaSToken.Text = HelperSession.AdvancedParametersCache(Robotics.Base.DTOs.AdvancedParameterType.DISaSToken.ToString(), True)
        LoadTokensSC()

        Me.txtURLWS.HRef = "https://vtliveapi.visualtime.net/Datalink/ExternalApi.svc"

        Dim oSM As New Robotics.Base.VTServiceApi.roServiceApiManager
        Dim DEXurl As String = oSM.GetDEXurl()
        Me.lnkExternComplaintChannel.HRef = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.AbsolutePath, "") & DEXurl
        Me.lnkExternComplaintChannel.InnerText = HttpContext.Current.Request.Url.OriginalString.Replace(HttpContext.Current.Request.Url.AbsolutePath, "") & DEXurl
        Me.lnkExternComplaintChannel.Target = "_blank"
        Me.txtURLWS.InnerText = Me.txtURLWS.HRef
        Dim isPgpEnabled As Boolean = roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("PGP.Enabled", True))
        Me.chkPGPEnabled.Checked = isPgpEnabled
        Me.hdnFileUploaded.Visible = isPgpEnabled

        Dim saSLink As String = HelperSession.AdvancedParametersCache(Robotics.Base.DTOs.AdvancedParameterType.BISaSLink.ToString(), True)
        Me.txtBISaSLink.Text = saSLink
        If saSLink IsNot Nothing AndAlso saSLink.Length > 0 Then
            Me.txtContainerURL.Text = saSLink.Split("?")(0)
            Me.txtSaSToken.Text = saSLink.Split("?")(1)
        End If
    End Sub

    Private Sub LoadTokensSC()

        Dim oSM As New Robotics.Base.VTServiceApi.roServiceApiManager
        Dim tokens As roCompanyToken = oSM.GetCompanyTokens()

        If oSM.State.Result = ServiceApiResultEnum.NoError AndAlso tokens IsNot Nothing Then
            Me.txtWSSCToken1.Text = tokens.token1.ToString()
            Me.txtWSSCToken2.Text = tokens.token2.ToString()
            Me.lblErrorSC.Visible = False
            Me.btnGenerateTokenSC1.Disabled = False
            Me.btnGenerateTokenSC2.Disabled = False
        Else
            Me.txtWSSCToken1.Text = ""
            Me.txtWSSCToken2.Text = ""
            Me.lblErrorSC.Visible = True
            Me.btnGenerateTokenSC1.Disabled = True
            Me.btnGenerateTokenSC2.Disabled = True
        End If

    End Sub

    Protected Sub generateTokenSC(sender As Object, e As EventArgs)
        Dim btn As HtmlAnchor = DirectCast(sender, HtmlAnchor)
        Dim idToken As Integer
        If (btn Is btnGenerateTokenSC1) Then
            idToken = 1
        Else
            idToken = 2
        End If

        Dim oSM As New Robotics.Base.VTServiceApi.roServiceApiManager
        Dim tokens As roCompanyToken = oSM.GenerateToken(idToken)

        If oSM.State.Result = ServiceApiResultEnum.NoError Then
            Me.txtWSSCToken1.Text = tokens.token1.ToString()
            Me.txtWSSCToken2.Text = tokens.token2.ToString()
            Me.lblErrorSC.Visible = False
            Me.btnGenerateTokenSC1.Disabled = False
            Me.btnGenerateTokenSC2.Disabled = False
        Else
            Me.txtWSSCToken1.Text = ""
            Me.txtWSSCToken2.Text = ""
            Me.lblErrorSC.Visible = True
            Me.btnGenerateTokenSC1.Disabled = True
            Me.btnGenerateTokenSC2.Disabled = True
        End If
        ScriptManager.RegisterStartupScript(Page, GetType(Page), "CallbackScript", "callbackGenerateToken();", True)

    End Sub

    Protected Sub generateSaSLink_ServerClick(sender As Object, e As EventArgs)
        Dim SaSLink As String = Azure.RoAzureSupport.GenerateContainerSaSTokenWithURI(roLiveQueueTypes.dinner, Azure.RoAzureSupport.GetCompanyName(), "rcdl")
        If SaSLink IsNot Nothing AndAlso SaSLink.Length > 0 Then
            Me.txtDISaSToken.Text = SecurityElement.Escape(SaSLink.Split("?")(1)) 'Escape characters like & to &amp;
        End If
        SaveSaSLinkConfig(Me.txtDISaSToken.Text)
    End Sub

    Protected Sub generateSaSLinkBI_ServerClick(sender As Object, e As EventArgs)
        Dim SaSLink As String = Azure.RoAzureSupport.GenerateContainerSaSTokenWithURI(roLiveQueueTypes.analyticsbi, Azure.RoAzureSupport.GetCompanyName(), "rl")
        Me.txtBISaSLink.Text = SaSLink
        If SaSLink IsNot Nothing AndAlso SaSLink.Length > 0 Then
            Me.txtContainerURL.Text = SaSLink.Split("?")(0)
            Me.txtSaSToken.Text = SaSLink.Split("?")(1)
        End If
        SaveSaSLinkConfigBI(SaSLink)
    End Sub

    Public Function SaveSaSLinkConfigBI(BISaSLink As String) As (Boolean, String)
        Dim bRet As (Boolean, String) = (False, "")
        Dim tmpbRet As Boolean = False

        If Me.txtBISaSLink.Text <> String.Empty Then
            Dim oAdvancedParameter As roAdvancedParameter = Nothing

            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "BISaSLink")
            If (oAdvancedParameter IsNot Nothing) Then
                oAdvancedParameter.Value = BISaSLink
                tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
            Else
                tmpbRet = False
            End If

            bRet = (tmpbRet, "")
        Else
            bRet = (False, "")
        End If

        Return bRet
    End Function

    Public Function SaveTokenConfig() As (Boolean, String)
        Dim bRet As (Boolean, String) = (False, "")
        Dim tmpbRet As Boolean = False

        If Me.txtWSToken1.Text <> String.Empty OrElse Me.txtWSToken2.Text <> String.Empty Then
            Dim oAdvancedParameter As roAdvancedParameter = Nothing

            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "ExternAccessToken1")
            If (oAdvancedParameter IsNot Nothing) Then
                oAdvancedParameter.Value = Me.txtWSToken1.Text
                tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
            Else
                tmpbRet = False
            End If

            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "ExternAccessToken2")
            If (oAdvancedParameter IsNot Nothing) Then
                oAdvancedParameter.Value = Me.txtWSToken2.Text
                tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
            Else
                tmpbRet = False
            End If

            bRet = (tmpbRet, "")
        Else
            bRet = (False, "")
        End If

        Return bRet
    End Function

    Public Function SaveWsConfig() As (Boolean, String)
        Dim bRet As (Boolean, String) = (False, "")
        Dim tmpbRet As Boolean = False

        If txtWSOldPassword.Text <> String.Empty OrElse Me.txtWSPassword.Text.Trim <> String.Empty OrElse Me.txtWSPasswordRepeat.Text <> String.Empty Then
            If HelperSession.AdvancedParametersCache(Robotics.Base.DTOs.AdvancedParameterType.ExternAccessPassword.ToString()) = txtWSOldPassword.Text Then
                If Me.txtWSPassword.Text.Trim <> String.Empty AndAlso Me.txtWSPasswordRepeat.Text.Trim = Me.txtWSPassword.Text.Trim Then
                    Dim oAdvancedParameter As roAdvancedParameter = Nothing
                    oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, Robotics.Base.DTOs.AdvancedParameterType.ExternAccessUserName.ToString())
                    If (oAdvancedParameter IsNot Nothing) Then
                        oAdvancedParameter.Value = Me.txtWSUserName.Text
                        tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
                    Else
                        tmpbRet = False
                    End If

                    oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, Robotics.Base.DTOs.AdvancedParameterType.ExternAccessIPs.ToString())
                    If (oAdvancedParameter IsNot Nothing) Then
                        oAdvancedParameter.Value = Me.txtAllowedIPs.Value
                        tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
                    Else
                        tmpbRet = False
                    End If

                    oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, Robotics.Base.DTOs.AdvancedParameterType.ExternAccessPassword.ToString())
                    If (oAdvancedParameter IsNot Nothing) Then
                        oAdvancedParameter.Value = Me.txtWSPassword.Text
                        tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
                    Else
                        tmpbRet = False
                    End If

                    If tmpbRet Then
                        Dim lstAuditParameterNames As New List(Of String)
                        Dim lstAuditParameterValues As New List(Of String)

                        API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tDatalinkWS, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
                        bRet = (True, "")
                    Else
                        bRet = (tmpbRet, "")
                    End If
                Else
                    bRet = (False, "WSPasswordCheck.NotMatch")
                End If
            Else
                bRet = (False, "WSPasswordCheck.PreviousPWNotMatch")
            End If
        Else
            bRet = (True, "")
        End If

        Return bRet
    End Function

    Public Function SaveSaSLinkConfig(DISaSToken As String) As (Boolean, String)
        Dim bRet As (Boolean, String) = (False, "")
        Dim tmpbRet As Boolean = False

        If Me.txtDISaSToken.Text <> String.Empty Then
            Dim oAdvancedParameter As roAdvancedParameter = Nothing

            oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, Robotics.Base.DTOs.AdvancedParameterType.DISaSToken.ToString())
            If (oAdvancedParameter IsNot Nothing) Then
                oAdvancedParameter.Value = DISaSToken
                tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
            Else
                tmpbRet = False
            End If

            bRet = (tmpbRet, "")
        Else
            bRet = (False, "")
        End If

        Return bRet
    End Function

    Public Function SavePGPConfig() As (Boolean, String)
        Dim bRet As (Boolean, String) = (False, "")
        Dim tmpbRet As Boolean = False
        Dim message As String = ""
        Dim chkPgpEnabled = Me.chkPGPEnabled.Checked
        Dim oAdvancedParameter As roAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "PGP.Enabled")

        If oAdvancedParameter IsNot Nothing Then
            If Not chkPgpEnabled Then
                oAdvancedParameter.Value = "0"
                tmpbRet = True
            Else
                If Not Azure.RoAzureSupport.CheckIfFileExists("pgp.pub", roLiveDatalinkFolders.certificates.ToString, roLiveQueueTypes.datalink) Then
                    tmpbRet = False
                    message = "PGPCheck.NoFileUploaded"
                Else
                    oAdvancedParameter.Value = "1"
                    tmpbRet = True
                End If
            End If

            If tmpbRet Then
                tmpbRet = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
            End If
        Else
            tmpbRet = False
        End If

        bRet = (tmpbRet, message)
        Return bRet
    End Function
#End Region

End Class