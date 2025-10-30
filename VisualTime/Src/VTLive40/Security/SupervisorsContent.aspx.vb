Imports DevExpress.Utils.Extensions
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class SupervisorsContent
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class PassportCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As String

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="gridCategories")>
        Public GridCategories As roPassportCategoryRow()

        <Runtime.Serialization.DataMember(Name:="gridUsers")>
        Public GridUsers As roPassportExceptionRow()

        <Runtime.Serialization.DataMember(Name:="groups")>
        Public groups As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class EmployeeStructField

        <Runtime.Serialization.DataMember(Name:="field")>
        Public attname As String

        <Runtime.Serialization.DataMember(Name:="value")>
        Public value As String

    End Class

    Private oPermission As Permission
    Private strActiveTab As String = "TABBUTTON_General"

    Protected Sub New()
        Me.OverrrideDefaultScope = "Supervisors"
        Me.OverrrideLanguageFile = "LiveSecurity"
    End Sub

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Administration.Security")

        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("Supervisors", "~/Security/Scripts/SupervisorsFrame.js")
        Me.InsertExtraJavascript("IdentifyMethods", "~/Base/Scripts/IdentifyMethods.js")
        Me.InsertExtraJavascript("SliderTip", "~/Base/Scripts/SliderTip.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roComboBox", "~/Base/Scripts/roComboBox.js")
        Me.InsertExtraJavascript("securityIPeditor", "~/Security/Scripts/SecurityIPEditor.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.hdnStrEmpName.Value = Me.Language.Translate("gridHeaderEmp.EmployeeName", Me.DefaultScope)
        Me.hdnStrCategoryName.Value = Me.Language.Translate("gridHeaderEmp.CategoryName", Me.DefaultScope)
        Me.hdnStrLevelName.Value = Me.Language.Translate("gridHeaderEmp.LevelName", Me.DefaultScope)
        Me.hdnStrNextLevelName.Value = Me.Language.Translate("gridHeaderEmp.NextLevelName", Me.DefaultScope)
        Me.hdnValueGridName.Value = Me.Language.Translate("GridIps.NameValue", DefaultScope)
        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Not Me.HasFeaturePermission("Administration", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        Dim oEmployeeTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("ASPxCallbackPanelContenido_objContainerTreeV3_roTrees1GroupTree")

        HelperWeb.roSelector_SetTreeState(oEmployeeTreeState)

        Me.ConvertControlsDivID = "divContent"
        If Not Me.IsPostBack And Not Me.IsCallback Then
            If Request.QueryString.Count Then
                ProcessQueryString()
            End If
        End If

    End Sub

    Protected Sub ProcessQueryString()

        Dim execJs As String = String.Empty

        If Not Request.QueryString("idpassport") Is Nothing Then
            Dim idemployee As String = Request.QueryString("idpassport")

            HelperWeb.roSelector_SetSelection(idemployee, "/source/" & idemployee, "ctl00_contentMainBody_roTreesSupervisors", "ignore", "ignore")

            execJs = "changeTabs('TABBUTTON_CONSENTS');"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "ChangeInitialLoadTab", execJs, True)
        End If
    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback
        Me.LoadLanguageCombo()

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New PassportCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        If oParameters.Action = "GETPASSPORT" Then
            Dim canModifyAddress As Boolean = False
            LoadPassportData(oParameters, canModifyAddress)
            Me.cnIdentifyMethods.LoadData(LoadType.Passport, oParameters.ID)
            Me.cnAllowedApplications.LoadData(LoadType.Passport, oParameters.ID)
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETPASSPORT")
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            ASPxCallbackPanelContenido.JSProperties.Add("cpCanModifyAddress", canModifyAddress)
        End If

        If oParameters.Action = "SAVEPASSPORT" Then
            Dim responseMessage = ""
            responseMessage = savePassport(oParameters)
            If responseMessage = "OK" Then
                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVEPASSPORT")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", "OK")
            Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVEPASSPORT")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", responseMessage)
            End If
        End If

    End Sub

#Region "Callback methods"

    ''' <summary>
    ''' Carrega el passport per ID
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadPassportData(ByVal oParameters As PassportCallbackRequest, ByRef CanModifyAddress As Boolean, Optional ByVal oCurPassport As roPassport = Nothing)
        ' Try

        Dim oCurrentPassport As roPassport

        If Me.oPermission > Permission.None Then

            If IsNumeric(oParameters.ID) Then

                If oCurPassport Is Nothing Then
                    oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Me, oParameters.ID, LoadType.Passport, True)
                Else
                    oCurrentPassport = oCurPassport
                End If

                If oCurrentPassport Is Nothing Then Return
                Me.strActiveTab = oParameters.aTab
                If oCurrentPassport.GroupType = "U" AndAlso Me.strActiveTab = "TABBUTTON_IdentifyMethods" Then
                    Me.strActiveTab = "TABBUTTON_General"
                ElseIf oCurrentPassport.GroupType = "U" AndAlso Me.strActiveTab = "TABBUTTON_AllowedApplications" Then
                    Me.strActiveTab = "TABBUTTON_General"
                ElseIf oCurrentPassport.GroupType = "E" AndAlso Me.strActiveTab = "TABBUTTON_Permissions" Then 'And Me.oCurrentPassport.GroupType <> "E" Then
                    Me.strActiveTab = "TABBUTTON_General"
                ElseIf oCurrentPassport.GroupType = "" AndAlso Me.strActiveTab = "TABBUTTON_EmployeePermissions" Then
                    Me.strActiveTab = "TABBUTTON_General"
                ElseIf oCurrentPassport.GroupType = "E" AndAlso Me.strActiveTab = "TABBUTTON_EmployeePermissions" Then
                    Me.strActiveTab = "TABBUTTON_General"
                ElseIf oCurrentPassport.GroupType = "" AndAlso Me.strActiveTab = "TABBUTTON_Permissions" Then
                    Me.strActiveTab = "TABBUTTON_General"
                End If

                Me.txtName.Text = oCurrentPassport.Name
                Me.txtDescription.Value = oCurrentPassport.Description

                Me.txtEmailAddress.Value = ""

                If oCurrentPassport.GroupType = "E" And oCurrentPassport.IDEmployee.HasValue Then
                    Me.txtEmailAddress.Enabled = False

                    Dim oConfParameters As roParameters = API.ConnectorServiceMethods.GetParameters(Me)

                    Dim emailUserField As String = String.Empty
                    If oConfParameters IsNot Nothing Then
                        Dim oParams As New roCollection(oConfParameters.ParametersXML)
                        emailUserField = roTypes.Any2String(oParams.Item(oConfParameters.ParametersNames(Parameters.EmailUsrField)))
                    End If

                    If emailUserField <> String.Empty Then
                        Dim uField As roEmployeeUserField = API.EmployeeServiceMethods.GetEmployeeUserFieldValueAtDate(Me.Page, oCurrentPassport.IDEmployee, emailUserField, DateTime.Now)
                        If uField IsNot Nothing Then
                            Me.txtEmailAddress.Value = uField.FieldValue
                        End If
                    Else
                        Me.txtEmailAddress.Value = ""
                    End If
                Else
                    Me.txtEmailAddress.Enabled = True
                    Me.txtEmailAddress.Value = oCurrentPassport.Email
                End If

                If oCurrentPassport.StartDate.HasValue Then
                    Me.txtStartDate.Date = oCurrentPassport.StartDate.Value
                Else
                    Me.txtStartDate.Value = Nothing
                End If
                If oCurrentPassport.ExpirationDate.HasValue Then
                    Me.txtFinishDate.Date = oCurrentPassport.ExpirationDate.Value
                Else
                    Me.txtFinishDate.Value = Nothing
                End If
                If oCurrentPassport.State.HasValue Then
                    Me.chkState.Checked = oCurrentPassport.State.Value
                Else
                    Me.chkState.Checked = True
                End If

                Me.cmbLanguage.SelectedItem = Me.cmbLanguage.Items.FindByValue(oCurrentPassport.Language.ID.ToString)

                Me.divActiveNotifications.Controls.Add(Me.CreateActiveNotificationaTable(oCurrentPassport.ID))

                LoadSecurityOptionsSection(oCurrentPassport, CanModifyAddress)

                Me.cmbSecurityFunctions.SelectedItem = Me.cmbSecurityFunctions.Items.FindByValue(oCurrentPassport.IDGroupFeature)

                If Not oCurrentPassport.IDEmployee.HasValue Then
                    'Me.ckIsSelfSupervised.ReadOnly = True
                    'Me.ckIsSelfSupervised.Visible = False
                    'Me.ckIsSelfSupervised.Checked = False
                End If

                If API.SecurityServiceMethods.GetPassportBelongsToAdminGroup(Me, oCurrentPassport.ID) Then
                    Me.chkState.Disabled = True
                    Me.txtStartDate.Enabled = False
                    Me.txtFinishDate.Enabled = False
                End If

                Dim requestCategories As roPassportCategories = oCurrentPassport.Categories

                Dim GridsJSON As String = roJSONHelper.SerializeNewtonSoft(requestCategories.CategoryRows)
                ASPxCallbackPanelContenido.JSProperties.Add("cpGridsJSON", GridsJSON)


                Dim employeeExceptions As roPassportExceptions = oCurrentPassport.Exceptions

                Dim UsersJSON As String = roJSONHelper.SerializeNewtonSoft(employeeExceptions.Exceptions.Where(Function(x) Not x.Available).ToArray())
                ASPxCallbackPanelContenido.JSProperties.Add("cpUsersJSON", UsersJSON)

                hdnTreeGroups.Value = ""

                If oCurrentPassport.Groups.GroupRows IsNot Nothing Then
                    For Each group In oCurrentPassport.Groups.GroupRows
                        hdnTreeGroups.Value += "A" & group.IDGroup & ","
                    Next
                End If

                If oCurrentPassport.Exceptions.Exceptions IsNot Nothing Then
                    For Each exception In oCurrentPassport.Exceptions.Exceptions
                        If exception.Available Then
                            hdnTreeGroups.Value += "B" & exception.IDEmployee & ","
                        End If
                    Next
                End If

                If hdnTreeGroups.Value.Length > 0 Then
                    hdnTreeGroups.Value = hdnTreeGroups.Value.Remove(hdnTreeGroups.Value.Length - 1)
                End If
            End If

            If Me.oPermission < Permission.Write Then
                'Desactivar edición
                Me.DisableControls()
                Me.cmbLanguage.Enabled = False
                Me.txtDescription.Enabled = False
                Me.cnIdentifyMethods.SetEnabled(False)
                Me.cnAllowedApplications.SetEnabled(False)
            End If

            'Mostra el TAB seleccionat
            Select Case Me.strActiveTab
                Case "TABBUTTON_General"
                    Me.divGeneral.Style("display") = ""
                Case "TABBUTTON_IdentifyMethods"
                    Me.divIdentifyMethods.Style("display") = ""
                Case "TABBUTTON_AllowedApplications"
                    Me.divAllowedApplications.Style("diaplay") = ""
            End Select

        End If

        'Catch ex As Exception
        '    Response.Write(ex.Message.ToString)
        'End Try
    End Sub

    Private Function savePassport(ByVal oParameters As PassportCallbackRequest) As String
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        Dim oCurrentPassport As roPassport = Nothing

        Try
            ' Verificamos si el passport actual tiene permisso de escritura
            If Me.oPermission >= Permission.Write Then

                oCurrentPassport = API.UserAdminServiceMethods.GetPassport(Me, oParameters.ID, LoadType.Passport, True)
                If oCurrentPassport IsNot Nothing Then

                    oCurrentPassport.Name = txtName.Text
                    oCurrentPassport.Description = Me.txtDescription.Value
                    If Me.txtStartDate.Value IsNot Nothing Then
                        If Me.txtStartDate.Value <> Me.txtStartDate.MinDate Then
                            oCurrentPassport.StartDate = Me.txtStartDate.Date
                        Else
                            oCurrentPassport.StartDate = Nothing
                        End If
                    Else
                        oCurrentPassport.StartDate = Nothing
                    End If

                    If Me.txtFinishDate.Value IsNot Nothing Then
                        If Me.txtFinishDate.Value <> Me.txtFinishDate.MinDate Then
                            oCurrentPassport.ExpirationDate = Me.txtFinishDate.Date
                            If oCurrentPassport.ExpirationDate < oCurrentPassport.StartDate Then
                                strErrorInfo = Me.Language.Translate("ActivePeriod.InvalidPeriod", Me.DefaultScope)
                            End If
                        Else
                            oCurrentPassport.ExpirationDate = Nothing
                        End If
                    Else
                        oCurrentPassport.ExpirationDate = Nothing
                    End If

                    oCurrentPassport.State = IIf(Me.chkState.Checked, 1, 0)

                    If Me.cmbLanguage.SelectedItem IsNot Nothing AndAlso IsNumeric(Me.cmbLanguage.SelectedItem.Value) Then
                        Dim oLanguageManager As New roLanguageManager()
                        oCurrentPassport.Language = oLanguageManager.LoadById(Me.cmbLanguage.SelectedItem.Value)
                    End If

                    If Me.cmbSecurityFunctions.SelectedItem IsNot Nothing AndAlso IsNumeric(Me.cmbSecurityFunctions.SelectedItem.Value) Then
                        oCurrentPassport.IDGroupFeature = Me.cmbSecurityFunctions.SelectedItem.Value
                    End If

                    oCurrentPassport.Email = Me.txtEmailAddress.Text

                    ' Verificamos configuración mètodos de autentificación
                    If strErrorInfo = "" AndAlso oCurrentPassport.GroupType <> "U" Then
                        Me.cnIdentifyMethods.Validate(strErrorInfo)
                    End If

                    ' Cargamos información de los métodos de identificación
                    Me.cnIdentifyMethods.LoadPassport(oCurrentPassport)
                    Me.cnAllowedApplications.LoadPassport(oCurrentPassport)

                    For Each row In oParameters.GridCategories
                        row.IDPassport = oCurrentPassport.ID
                        row.RowState = RowState.NewRow
                    Next
                    oCurrentPassport.Categories.CategoryRows = oParameters.GridCategories
                    oCurrentPassport.Categories.idPassport = oCurrentPassport.ID

                    Dim xPassportExceptions As New Generic.List(Of roPassportExceptionRow)
                    If oParameters.groups <> "" Then
                        Dim groups As New Generic.List(Of Integer)
                        Dim employees As New Generic.List(Of Integer)

                        roSelectorManager.ExtractIdsFromSelectionString(oParameters.groups, employees, groups)

                        Dim xPassportGroups As New Generic.List(Of roPassportGroupRow)
                        For Each group In groups
                            Dim xGroup As New roPassportGroupRow
                            xGroup.IDGroup = group
                            xGroup.IDPassport = oCurrentPassport.ID
                            xGroup.RowState = RowState.NewRow
                            xPassportGroups.Add(xGroup)
                        Next

                        For Each employee In employees
                            Dim xException As New roPassportExceptionRow
                            xException.IDPassport = oCurrentPassport.ID
                            xException.IDEmployee = employee
                            xException.Name = API.EmployeeServiceMethods.GetEmployeeName(Nothing, employee)
                            xException.Available = True
                            xPassportExceptions.Add(xException)
                        Next
                        oCurrentPassport.Groups.GroupRows = xPassportGroups.ToArray
                    Else
                        'strErrorInfo = Me.Language.Translate("SavePassport.InfoGroups.NotEmpty", Me.DefaultScope)
                        oCurrentPassport.Groups.GroupRows = {}
                    End If

                    oCurrentPassport.CanApproveOwnRequests = True
                    If oParameters.GridUsers IsNot Nothing Then
                        For Each row In oParameters.GridUsers
                            row.IDPassport = oCurrentPassport.ID

                            If oCurrentPassport.IDEmployee.HasValue AndAlso row.IDEmployee = oCurrentPassport.IDEmployee Then
                                oCurrentPassport.CanApproveOwnRequests = False
                            End If

                            xPassportExceptions.Add(New roPassportExceptionRow() With {
                                .IDPassport = oCurrentPassport.ID,
                                .IDEmployee = row.IDEmployee,
                                .Available = False})
                        Next
                    End If
                    oCurrentPassport.Exceptions = New roPassportExceptions() With {
                           .idPassport = oCurrentPassport.ID,
                           .Exceptions = xPassportExceptions.ToArray
                           }

                    If strErrorInfo = "" Then

                        'Lo ponemos a vacío para que la función que graba decida si es empleado supervisor o supervisor
                        oCurrentPassport.GroupType = ""

                        ' Guardamos el passport
                        If Not API.UserAdminServiceMethods.SavePassport(Me, oCurrentPassport, True, True) Then
                            strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                        End If

                        If strErrorInfo = "" AndAlso Me.cnIdentifyMethods.isPasswordResetted Then
                            Dim oPwd As roPassportAuthenticationMethodsRow = API.UserAdminServiceMethods.GetPassport(Me, oCurrentPassport.ID, LoadType.Passport).AuthenticationMethods.PasswordRow

                            'Dim newPwd = CryptographyHelper.Decrypt(oPwd.Password)
                            Dim newPwd = oPwd.Password
                            Dim oParams As New Generic.List(Of String)
                            oParams.Add(newPwd)
                            strErrorInfo = strResponse & "#" & Me.Language.Translate("SavePassport.InfoPwd.ResetDescription", Me.DefaultScope, oParams)

                        End If

                        'comprobar cambio de contraseña
                        If strErrorInfo = "" Then
                            If oCurrentPassport.ID = WLHelperWeb.CurrentPassport.ID Then
                                Dim oContext As WebCContext = WLHelperWeb.Context(HttpContext.Current.Request, oCurrentPassport.ID)
                                Dim oMethod As roPassportAuthenticationMethodsRow = oCurrentPassport.AuthenticationMethods.PasswordRow

                                If oMethod IsNot Nothing AndAlso oMethod.Password <> oContext.Password Then
                                    roWsUserManagement.RemoveCurrentsession()
                                    oContext.Password = oMethod.Password
                                End If
                            End If


                            'If oCurrentPassport.GroupType = "U" Then
                            strErrorInfo = SaveSecurityOptions(oCurrentPassport)
                            'End If
                        End If

                    End If
                Else
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            strErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        If strErrorInfo <> "" Then
            If strErrorInfo.StartsWith("OK#") Then
                strResponse = "INFOMSG" &
                          "TitleKey=SavePassport.InfoPwd.Title&" +
                          "DescriptionText=" + strErrorInfo.Substring(3, strErrorInfo.Length - 3) + "&" +
                          "Option1TextKey=SavePassport.InfoPwd.Option1Text&" +
                          "Option1DescriptionKey=SavePassport.InfoPwd.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.InformationIcon)
            Else
                strResponse = "MESSAGE" &
                          "TitleKey=SavePassport.Error.Title&" +
                          "DescriptionText=" + strErrorInfo + "&" +
                          "Option1TextKey=SavePassport.Error.Option1Text&" +
                          "Option1DescriptionKey=SavePassport.Error.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)

                Dim canModifyAddress As Boolean = False
                LoadPassportData(oParameters, canModifyAddress, oCurrentPassport)
                ASPxCallbackPanelContenido.JSProperties.Add("cpCanModifyAddress", canModifyAddress)

            End If

        End If

        'Response.Write(strResponse)
        Return strResponse
    End Function

#End Region

#Region "Helper methods"

    Public Sub Add(Of T)(ByRef arr As T(), item As T)
        Array.Resize(arr, arr.Length + 1)
        arr(arr.Length - 1) = item
    End Sub



    Private Sub LoadLanguageCombo()
        Dim oLanguages As roPassportLanguage() = API.UserAdminServiceMethods.GetLanguages(Me)
        Dim bolInstalled As Boolean = False
        With Me.cmbLanguage
            .Items.Clear()
            For Each oLanguage As roPassportLanguage In oLanguages
                bolInstalled = IIf(oLanguage.Key = "ESP", True, False)
                If oLanguage.Installed Or bolInstalled Then
                    .Items.Add(New DevExpress.Web.ListEditItem(Language.Translate("Language." & oLanguage.Key, Me.DefaultScope), oLanguage.ID))
                End If
            Next
        End With

        Dim oGroupFeatures As roGroupFeature() = API.SecurityChartServiceMethods.GetGroupFeatures(Me.Page)
        With Me.cmbSecurityFunctions
            .Items.Clear()
            .ValueType = GetType(Integer)
            For Each oGroupFeature In oGroupFeatures
                .Items.Add(oGroupFeature.Name, oGroupFeature.ID)
            Next
        End With

        Dim oConsultatntGroupFeature As roGroupFeature = API.SecurityChartServiceMethods.GetConsultantGroupFeature(Me.Page)
        If oConsultatntGroupFeature IsNot Nothing Then
            Me.cmbSecurityFunctions.Items.Add(oConsultatntGroupFeature.Name, oConsultatntGroupFeature.ID)
        End If

    End Sub

#End Region

#Region "Features methods"

    Private Function CreateActiveNotificationaTable(ByVal idPassport As Integer) As HtmlTable
        Dim hTable As New HtmlTable
        Dim hTRow As HtmlTableRow
        Dim hTCell As HtmlTableCell

        With hTable
            .Border = 0
            .CellPadding = 0
            .CellSpacing = 0
            .Attributes("class") = "FeaturesTableStyle GridFeatures"
        End With

        ' Añadimos fila nombres columnas
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader FeaturesTableStyle-cellheader-noend"
        'hTCell.Attributes("style") = "border-right: 0;"
        hTCell.InnerHtml = Me.Language.Translate("Notifications.Columns.Name", Me.DefaultScope) ' "Funcionalidad"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader"
        hTCell.Attributes("style") = "text-align: center;"
        hTCell.InnerHtml = Me.Language.Translate("Notifications.Columns.Allowed", Me.DefaultScope) '"Permiso"
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim notificationsTable As DataTable = API.NotificationServiceMethods.GetPermissionOverNotifications(Me.Page, idPassport)

        Dim oNotificationsRows As Generic.List(Of HtmlTableRow) = Me.GetNotifications(notificationsTable)
        If oNotificationsRows IsNot Nothing Then
            For Each oRow As HtmlTableRow In oNotificationsRows
                hTable.Rows.Add(oRow)
            Next
        End If

        Return hTable
    End Function

    Private Function GetNotifications(ByVal notificationsDT As DataTable) As Generic.List(Of HtmlTableRow)

        Dim oFeatureRows As New Generic.List(Of HtmlTableRow)

        If notificationsDT IsNot Nothing AndAlso notificationsDT.Rows.Count > 0 Then

            Dim hTRow As HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim divFeature As HtmlGenericControl
            Dim divAnchorFeature As HtmlGenericControl
            Dim aAnchor As HtmlAnchor
            Dim aAnchorInfo As HtmlAnchor

            ' Obtenemos traducción tooltip botón mostrar información
            Dim strFeatureInformationButton As String = Me.Language.Translate("Feature.Information.Button", Me.DefaultScope)

            Dim intLevel As Integer = 1

            For Each oFeatureRow As DataRow In notificationsDT.Rows
                ' Pinta la fila con el nombre de la categoría actual y sus permisos
                hTRow = New HtmlTableRow
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " " & "FeaturesTableStyle-noendcellLevel" & intLevel

                aAnchor = New HtmlAnchor
                With aAnchor
                    .HRef = "javascript:void(0);"
                    .Style("width") = "100%"
                    .Attributes("class") = "FeatureAnchor"
                    .Style("cursor") = "default"
                    .InnerHtml = Me.Language.Translate("NotifyType." & oFeatureRow("Name"), Me.DefaultScope)
                End With

                divFeature = New HtmlGenericControl("div")
                divFeature.Style("width") = "100%"

                divAnchorFeature = New HtmlGenericControl("div")
                divAnchorFeature.Style("float") = "left"
                divAnchorFeature.Style("text-align") = "left"
                divAnchorFeature.Controls.Add(aAnchor)

                divFeature.Controls.Add(divAnchorFeature)
                hTCell.Controls.Add(divFeature)

                hTRow.Cells.Add(hTCell)

                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " FeaturesTableStyle-cellPermissions"
                hTCell.Attributes("align") = "center"
                hTCell.Style("padding") = "3px"

                aAnchorInfo = New HtmlAnchor
                With aAnchorInfo
                    .Style("width") = "100%"
                    .Style("cursor") = "default"
                    .Attributes("class") = "FeatureAnchor"

                    If roTypes.Any2String(oFeatureRow("Available")) = "1" Then
                        .InnerHtml = "<img src=""../Base/Images/OptionPanel/iconsi.png"" width=""16px"" />"
                    Else
                        .InnerHtml = "<img src=""../Base/Images/OptionPanel/iconno.png"" width=""16px"" />"
                    End If

                End With

                divFeature = New HtmlGenericControl("div")
                divFeature.Style("width") = "100%"

                divAnchorFeature = New HtmlGenericControl("div")
                divAnchorFeature.Attributes("align") = "center"
                divAnchorFeature.Controls.Add(aAnchorInfo)

                divFeature.Controls.Add(divAnchorFeature)
                hTCell.Controls.Add(divFeature)

                hTRow.Cells.Add(hTCell)

                ' Añadimos fila la colección
                oFeatureRows.Add(hTRow)
            Next

        End If

        Return oFeatureRows

    End Function

#End Region

#Region "Security Options"

    Private Function SaveSecurityOptions(ByVal oCurrentPassport As roPassport) As String
        Dim strErrorInfo As String = ""
        Dim inheritedSecurityOptions As roSecurityOptions = API.SecurityOptionsServiceMethods.GetInheritedSecurityOptions(Me.Page, oCurrentPassport.IDParentPassport, False)
        Dim passportSecurityOptions As roSecurityOptions = API.SecurityOptionsServiceMethods.GetSecurityOptions(Me.Page, oCurrentPassport.IDParentPassport, False)

        If Me.ChkRestrictedIP.Checked Then
            passportSecurityOptions.OnlyAllowedAdress = Me.txtAllowedIPs.Value
        Else
            passportSecurityOptions.OnlyAllowedAdress = ""
        End If

        If inheritedSecurityOptions.OnlySameVersionApp = False Then
            passportSecurityOptions.OnlySameVersionApp = Me.chkVersion.Checked
        End If

        passportSecurityOptions.RequestValidationCode = Me.ckRequiereKey.Checked
        passportSecurityOptions.SaveAuthorizedPointDays = roTypes.Any2Integer(Me.txtSaveKeyTime.Value)
        passportSecurityOptions.DifferentAccessPointExceeded = roTypes.Any2Integer(Me.txtAccessDiferentIps.Value)

        If inheritedSecurityOptions.CalendarLock = True Then
            passportSecurityOptions.CalendarLock = Me.ckCalendarLock.Checked
        End If

        If API.SecurityOptionsServiceMethods.SaveSecurityOptions(Me.Page, passportSecurityOptions, True) = False Then
            strErrorInfo = roWsUserManagement.SessionObject.States.SecurityOptionState.ErrorText
        End If

        Return strErrorInfo
    End Function

    Private Function LoadSecurityOptionsSection(ByVal oCurrentPassport As roPassport, ByRef canModifyAddress As Boolean) As Boolean

        Try
            Dim inheritedSecurityOptions As roSecurityOptions = API.SecurityOptionsServiceMethods.GetInheritedSecurityOptions(Me.Page, oCurrentPassport.IDParentPassport, False)
            Dim passportSecurityOptions As roSecurityOptions = API.SecurityOptionsServiceMethods.GetSecurityOptions(Me.Page, oCurrentPassport.IDParentPassport, False)

            Dim allowedAddress As String = inheritedSecurityOptions.OnlyAllowedAdress & "" & passportSecurityOptions.OnlyAllowedAdress

            Me.ChkRestrictedIP.Checked = IIf(passportSecurityOptions.OnlyAllowedAdress <> String.Empty, True, False)
            Me.txtAllowedIPs.Value = passportSecurityOptions.OnlyAllowedAdress
            canModifyAddress = True

            Me.ckRequiereKey.Checked = passportSecurityOptions.RequestValidationCode
            Me.txtSaveKeyTime.Value = passportSecurityOptions.SaveAuthorizedPointDays
            Me.txtAccessDiferentIps.Value = passportSecurityOptions.DifferentAccessPointExceeded

            If inheritedSecurityOptions.OnlySameVersionApp = True Then
                Me.chkVersion.Checked = True
                Me.DisableControls(Me.chkVersion.Controls)
            Else
                Me.chkVersion.Checked = passportSecurityOptions.OnlySameVersionApp
                'If Me.chkVersion.Checked = False Then Me.DisableControls(Me.chkVersion.Controls)
            End If

            If inheritedSecurityOptions.CalendarLock = False Then
                Me.ckCalendarLock.Checked = False
                Me.ckCalendarLock.Disabled = True
            Else
                Me.ckCalendarLock.Checked = passportSecurityOptions.CalendarLock
            End If

            Return True
        Catch ex As Exception
            Return False
        End Try

    End Function

    Private Function CreateGridsJSON(ByRef oCurrentGroupDS As DataSet) As String

        Dim strJSONGroups As String = ""
        'Dim oCurrentGroup As DataTable = oCurrentGroupDS.Tables(0)

        Try
            Dim oJGEmployees As New Generic.List(Of Object)

            Dim strJSONText As String = ""
            strJSONGroups = "{""employees"":["
            For Each oObj As Object In oJGEmployees
                strJSONText = "{""fields"":" & roJSONHelper.Serialize(oObj) & " },"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]}"

            Return strJSONGroups
        Catch ex As Exception
            Return String.Empty
        End Try

    End Function

    Private Sub ASPxCallbackPanelContenido_Load(sender As Object, e As EventArgs) Handles ASPxCallbackPanelContenido.Load

    End Sub

#End Region

End Class