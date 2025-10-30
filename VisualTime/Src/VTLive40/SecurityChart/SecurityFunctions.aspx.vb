Imports Robotics.Base.DTOs
Imports Robotics.UsersAdmin
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class SecurityFunctions
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class PassportCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As String

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="params")>
        Public Params As String

        <Runtime.Serialization.DataMember(Name:="clientControlsData")>
        Public clientControlsData As ClientControlsData

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class ClientControlsData

        <Runtime.Serialization.DataMember(Name:="selectedBusinessGroups")>
        Public selectedBusinessGroups As String()

        <Runtime.Serialization.DataMember(Name:="businessGroups")>
        Public businessGroups As SelectListItem()

        <Runtime.Serialization.DataMember(Name:="updatedPermissions")>
        Public updatedPermissions As FeaturePermission()

        <Runtime.Serialization.DataMember(Name:="clientEnabled")>
        Public clientEnabled As Boolean

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class SelectListItem

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As String

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class FeaturePermission

        <Runtime.Serialization.DataMember(Name:="IDFeature")>
        Public IDFeature As Integer

        <Runtime.Serialization.DataMember(Name:="IDPermission")>
        Public IDPermission As Integer

        <Runtime.Serialization.DataMember(Name:="Checked")>
        Public Checked As Boolean

    End Class

    Public Property FeatureList(ByVal bReload As Boolean) As Feature()
        Get
            Dim oLst As Feature() = Session("roApplication_FeatureList")
            If oLst Is Nothing OrElse bReload Then
                Session("roApplication_FeatureList") = API.UserAdminServiceMethods.GetFeaturesList(Me, "U")
                oLst = Session("roApplication_FeatureList")
            End If
            Return oLst

        End Get
        Set(value As Feature())
            Session("roApplication_FeatureList") = value
        End Set
    End Property

    Private oPermission As Permission
    Private strActiveTab As String = "TABBUTTON_General"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Administration.Security")

        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("IdentifyMethods", "~/Base/Scripts/IdentifyMethods.js")
        Me.InsertExtraJavascript("SliderTip", "~/Base/Scripts/SliderTip.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roComboBox", "~/Base/Scripts/roComboBox.js")

        Me.InsertExtraJavascript("SecurityFunctions", "~/SecurityChart/Scripts/SecurityFunctions.js")
        Me.InsertExtraJavascript("bcSelector", "~/Base/Scripts/frmBusinessCenterSelector.js")
        Me.InsertExtraJavascript("utils", "~/Base/Scripts/Live/utils.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        roTreesSecurityFunctions.TreeCaption = Me.Language.Translate("TreeCaptionSecurityFunctions", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.hdnValueGridName.Value = Me.Language.Translate("GridIps.NameValue", DefaultScope)

        If Not Me.HasFeaturePermission("Administration", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Not Me.IsPostBack Then
            Dim oLst = FeatureList(True)
        End If

        Dim oUpdatePermissions As New Generic.List(Of FeaturePermission)
        Me.divPermissionsTable.Controls.Add(Me.CreateFeaturesTable(-1, Nothing, oUpdatePermissions))

        Me.ConvertControlsDivID = "divContent"

    End Sub

    Private Function CreateBusinessGroupsBoxJSON() As SelectListItem()
        Dim oLst As SelectListItem() = {}
        Try
            Dim lstItems As New Generic.List(Of SelectListItem)

            Dim lstAddedItems As New Generic.List(Of String)
            Dim strIDField As String = "BusinessGroup"
            Dim strNameField As String = "BusinessGroup"
            Dim strText As String

            Dim tbShiftsBusinessGroup As DataTable = API.ShiftServiceMethods.GetBusinessGroupFromShiftGroups(Me)
            Dim tbCausesBusinessGroup As DataTable = CausesServiceMethods.GetBusinessGroupFromCauseGroups(Me)
            Dim tbConceptsGroupsBusinessGroup As DataTable = API.ConceptsServiceMethods.GetBusinessGroupFromConceptGroups(Me)

            For Each oRow As DataRow In tbShiftsBusinessGroup.Rows
                strText = roTypes.Any2String(oRow(strNameField)).Trim
                If Not lstAddedItems.Contains(strText) Then

                    Dim oTemp As New SelectListItem
                    oTemp.ID = strText
                    oTemp.Name = strText
                    lstItems.Add(oTemp)

                    lstAddedItems.Add(strText)
                End If
            Next

            For Each oRow As DataRow In tbCausesBusinessGroup.Rows
                strText = roTypes.Any2String(oRow(strNameField)).Trim
                If Not lstAddedItems.Contains(strText) Then
                    Dim oTemp As New SelectListItem
                    oTemp.ID = strText
                    oTemp.Name = strText
                    lstItems.Add(oTemp)

                    lstAddedItems.Add(strText)
                End If
            Next

            For Each oRow As DataRow In tbConceptsGroupsBusinessGroup.Rows
                strText = roTypes.Any2String(oRow(strNameField)).Trim
                If Not lstAddedItems.Contains(strText) Then
                    Dim oTemp As New SelectListItem
                    oTemp.ID = strText
                    oTemp.Name = strText
                    lstItems.Add(oTemp)

                    lstAddedItems.Add(strText)
                End If
            Next

            oLst = lstItems.ToArray()
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
        End Try
        Return oLst
    End Function

    Protected Sub PermissionCallback_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles PermissionCallback.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New PassportCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim oClientControls As New ClientControlsData

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "UPDATESECURITYFUNCTIONPERMISSION"
                bRet = UpdatePermission(oParameters, responseMessage)

                If bRet Then
                    oClientControls.updatedPermissions = GetNewPermissions(oParameters)
                Else
                    oClientControls.updatedPermissions = {}
                End If

                PermissionCallback.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls))
                PermissionCallback.JSProperties.Add("cpActionRO", "UPDATESECURITYFUNCTIONPERMISSION")
                PermissionCallback.JSProperties.Add("cpResultRO", IIf(bRet, "OK", "KO"))
                If Not bRet Then
                    PermissionCallback.JSProperties.Add("cpMessage", responseMessage)
                End If
        End Select

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New PassportCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim oClientControls As New ClientControlsData
        oClientControls.businessGroups = CreateBusinessGroupsBoxJSON()
        If oPermission > Permission.Read Then
            oClientControls.clientEnabled = True
        Else
            oClientControls.clientEnabled = False
        End If

        Dim responseMessage = String.Empty
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "UPDATESECURITYFUNCTIONPERMISSION"
                bRet = UpdatePermission(oParameters, responseMessage)

                If bRet Then
                    oClientControls.updatedPermissions = GetNewPermissions(oParameters)
                Else
                    oClientControls.updatedPermissions = {}
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "UPDATESECURITYFUNCTIONPERMISSION")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", IIf(bRet, "OK", "KO"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

                ASPxCallbackPanelContenido.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls))

            Case "GETSECURITYFUNCTION"
                bRet = LoadSecurityFunctionData(oParameters, responseMessage, oClientControls)
                Dim existingExternalIdsTable As DataTable = API.SecurityChartServiceMethods.GetAllExternalIds(Me)

                If existingExternalIdsTable IsNot Nothing Then
                    Me.hdnExistingExternalIds.Value = String.Join(",", existingExternalIdsTable.AsEnumerable().Select(Function(row) row.Field(Of String)("ExternalId")))
                Else
                    Me.hdnExistingExternalIds.Value = String.Empty
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETSECURITYFUNCTION")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", IIf(bRet, "OK", "KO"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
                ASPxCallbackPanelContenido.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls))
                ASPxCallbackPanelContenido.JSProperties("cpExistingExternalIds") = hdnExistingExternalIds.Value

            Case "SAVESECURITYFUNCTION"
                Dim oSavedObject As roGroupFeature = Nothing
                responseMessage = saveSecurityFunction(oParameters, oSavedObject)

                If oSavedObject IsNot Nothing Then
                    bRet = LoadSecurityFunctionData(oParameters, responseMessage, oClientControls, oSavedObject)
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", IIf(responseMessage = "OK", "OK", "KO"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                If responseMessage = "OK" Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVESECURITYFUNCTION")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", True)
                Else
                    ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "SAVESECURITYFUNCTION")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If

                ASPxCallbackPanelContenido.JSProperties.Add("cpClientControlsRO", roJSONHelper.Serialize(oClientControls))
        End Select
    End Sub

#Region "Callback methods"

    Private Function UpdatePermission(ByVal oParameters As PassportCallbackRequest, ByRef ErrorInfo As String, Optional ByVal eCurSecurityFunction As roGroupFeature = Nothing) As Boolean
        Dim bRet As Boolean = False
        Dim oCurSecurityFunction As roGroupFeature

        If Me.oPermission > Permission.Read Then

            If IsNumeric(oParameters.ID) Then

                If eCurSecurityFunction Is Nothing Then
                    oCurSecurityFunction = API.SecurityChartServiceMethods.GetGroupFeatureByID(oParameters.ID, Me.Page)
                Else
                    oCurSecurityFunction = eCurSecurityFunction
                End If

                If oCurSecurityFunction Is Nothing Then Return False

                Me.strActiveTab = oParameters.aTab

                Dim idFeature As Integer = roTypes.Any2Integer(oParameters.Params.Split(",")(0))
                Dim strPermission As String = oParameters.Params.Split(",")(1)

                Dim iPermission As Integer = 0

                Select Case strPermission.ToUpper
                    Case "ADMIN"
                        iPermission = 9
                    Case "WRITE"
                        iPermission = 6
                    Case "READ"
                        iPermission = 3
                End Select

                If API.SecurityChartServiceMethods.SetGroupFeaturePermission(oParameters.ID, idFeature, iPermission, Me.Page) Then
                    bRet = True
                Else
                    bRet = False
                End If
            End If
        Else
            ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            bRet = False
        End If

        Return bRet
    End Function

    ''' <summary>
    ''' Carrega el passport per ID
    ''' </summary>
    ''' <remarks></remarks>
    Private Function LoadSecurityFunctionData(ByVal oParameters As PassportCallbackRequest, ByRef ErrorInfo As String, ByRef clientOptions As ClientControlsData, Optional ByVal eCurSecurityFunction As roGroupFeature = Nothing) As Boolean
        ' Try
        Dim oCurSecurityFunction As roGroupFeature

        If Me.oPermission > Permission.None Then

            If IsNumeric(oParameters.ID) Then

                If eCurSecurityFunction Is Nothing Then
                    If oParameters.ID >= 0 Then
                        oCurSecurityFunction = API.SecurityChartServiceMethods.GetGroupFeatureByID(oParameters.ID, Me.Page)
                    Else
                        oCurSecurityFunction = New roGroupFeature
                    End If
                Else
                    oCurSecurityFunction = eCurSecurityFunction
                End If

                If oCurSecurityFunction Is Nothing Then Return False
                Me.strActiveTab = oParameters.aTab

                Me.txtName.Text = oCurSecurityFunction.Name
                Me.txtDescription.Text = oCurSecurityFunction.Description
                Me.txtExport.Text = oCurSecurityFunction.ExternalId
                clientOptions.selectedBusinessGroups = oCurSecurityFunction.BusinessGroupList

                If oCurSecurityFunction.BusinessGroupList Is Nothing OrElse oCurSecurityFunction.BusinessGroupList.Length = 0 Then
                    optBGListAll.Checked = True
                    optBGListValue.Checked = False
                Else
                    optBGListAll.Checked = False
                    optBGListValue.Checked = True
                End If

                If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
                    divBusinessRoles.Style("display") = "none"
                End If

                frmBusinessCenterSelector.SetSecurityGroup(oParameters.ID, False, True, True, False)
                Me.txtBusinessCenter.Value = ""

                Me.tbBusinessCenter.Visible = (HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") OrElse HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl"))
                If Me.tbBusinessCenter.Visible Then
                    Dim oBusinessCenter As DataTable = API.TasksServiceMethods.GetBusinessCenterBySecurityGroupDataTable(Me, oParameters.ID, False)
                    If oBusinessCenter IsNot Nothing Then
                        For Each row As DataRow In oBusinessCenter.Rows
                            Me.txtBusinessCenter.Value &= row("Name") & ", "
                        Next
                        If Me.txtBusinessCenter.Value.EndsWith(", ") Then
                            Me.txtBusinessCenter.Value = Me.txtBusinessCenter.Value.Substring(0, Me.txtBusinessCenter.Value.Length - 2)
                        End If
                    End If
                End If

                If oParameters.ID > -1 Then
                    Dim oUpdatePermissions As New Generic.List(Of FeaturePermission)

                    Me.CreateFeaturesTable(oParameters.ID, oCurSecurityFunction.Features, oUpdatePermissions)
                    clientOptions.updatedPermissions = oUpdatePermissions.ToArray()
                Else
                    Dim oUpdatePermissions As New Generic.List(Of FeaturePermission)
                    Me.CreateFeaturesTable(-1, Nothing, oUpdatePermissions)
                    clientOptions.updatedPermissions = oUpdatePermissions.ToArray()
                End If

            End If

            If oParameters.ID = 0 Then
                Me.txtDescription.ReadOnly = True
                Me.txtName.ReadOnly = True
            End If

            If Me.oPermission < Permission.Write Then
                Me.btAddBusinessCenter.Visible = False

                'Desactivar edición
                Me.DisableControls()
            End If

            'Mostra el TAB seleccionat
            Select Case Me.strActiveTab
                Case "TABBUTTON_General"
                    Me.divGeneral.Style("display") = ""
                Case "TABBUTTON_Permissions"
                    Me.divPermissions.Style("display") = ""
            End Select
        Else
            ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            Return False
        End If

        Return True
    End Function

    Private Function saveSecurityFunction(ByVal oParameters As PassportCallbackRequest, ByRef oCurSecurityFunction As roGroupFeature) As String
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""
        oCurSecurityFunction = Nothing

        ' Verificamos si el passport actual tiene permisso de escritura
        If Me.oPermission >= Permission.Write Then

            Try
                oCurSecurityFunction = API.SecurityChartServiceMethods.GetGroupFeatureByID(oParameters.ID, Me.Page)

                If oCurSecurityFunction Is Nothing Then
                    oCurSecurityFunction = New roGroupFeature
                    oCurSecurityFunction.ID = -1
                End If

                oCurSecurityFunction.Name = Me.txtName.Text
                oCurSecurityFunction.Description = Me.txtDescription.Text
                oCurSecurityFunction.ExternalId = Me.txtExport.Text

                If Me.optBGListAll.Checked Then
                    oCurSecurityFunction.BusinessGroupList = {}
                Else
                    oCurSecurityFunction.BusinessGroupList = oParameters.clientControlsData.selectedBusinessGroups
                End If

                If Not API.SecurityChartServiceMethods.SaveGroupFeature(oCurSecurityFunction, Me.Page) Then
                    strErrorInfo = roWsUserManagement.SessionObject.States.GroupFeatureState.ErrorText
                Else
                    Dim treePath As String = "/source/" & oCurSecurityFunction.ID
                    HelperWeb.roSelector_SetSelection(oCurSecurityFunction.ID.ToString, treePath, "ctl00_contentMainBody_roTreesSecurityFunctions")
                End If
            Catch ex As Exception
                strErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
            End Try

            If strErrorInfo <> "" Then
                strResponse = strErrorInfo
            End If
        Else
            strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)

            strResponse = strErrorInfo
        End If

        'Response.Write(strResponse)
        Return strResponse
    End Function

    Private Function GetNewPermissions(ByVal oParameters As PassportCallbackRequest) As FeaturePermission()
        Dim oUpdatePermissions As New Generic.List(Of FeaturePermission)
        Dim oCurSecurityFunction As roGroupFeature

        If Me.oPermission > Permission.None Then

            If IsNumeric(oParameters.ID) Then

                oCurSecurityFunction = API.SecurityChartServiceMethods.GetGroupFeatureByID(oParameters.ID, Me.Page)

                Me.CreateFeaturesTable(oParameters.ID, oCurSecurityFunction.Features, oUpdatePermissions)

            End If
        End If

        Return oUpdatePermissions.ToArray()
    End Function

#End Region

#Region "Features methods"

    Private Function CreateFeaturesTable(ByVal idSecurityFunction As Integer, ByVal oSecurityFunctionFeatures As roGroupFeaturePermissionsOverFeature(), ByRef oUpdatedPermissions As Generic.List(Of FeaturePermission)) As HtmlTable

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
        hTCell.InnerHtml = Me.Language.Translate("Functionalties.Columns.Name", Me.DefaultScope) ' "Funcionalidad"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "FeaturesTableStyle-cellheader"
        hTCell.Attributes("style") = "text-align: right;"
        hTCell.InnerHtml = Me.Language.Translate("Functionalties.Columns.Permission", Me.DefaultScope) '"Permiso"
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim oFeaturesAll As Feature() = FeatureList(False)

        Dim oFeatureRows As Generic.List(Of HtmlTableRow) = Me.GetFeatures(idSecurityFunction, oFeaturesAll, oSecurityFunctionFeatures, Nothing, "U", 1, oUpdatedPermissions)
        If oFeatureRows IsNot Nothing Then
            For Each oRow As HtmlTableRow In oFeatureRows
                hTable.Rows.Add(oRow)
            Next
        End If

        Return hTable

    End Function

    Private Function GetFeatures(ByVal idSecurityFunction As Integer, ByVal oFeaturesAll() As Feature, ByVal oSecurityFunctionFeatures As roGroupFeaturePermissionsOverFeature(), ByVal idParentFeature As Nullable(Of Integer), ByVal type As String, ByVal intLevel As Integer, ByRef oUpdatedPermissions As Generic.List(Of FeaturePermission)) As Generic.List(Of HtmlTableRow)

        Dim oFeatureRows As New Generic.List(Of HtmlTableRow)

        If oFeaturesAll IsNot Nothing Then

            ' Obtenemos las funcionalidades del nivel actual (idParentfeature)
            Dim oFeatures As New Generic.List(Of Feature)
            For Each oItem As Feature In oFeaturesAll
                If Not idParentFeature.HasValue Then
                    If Not oItem.IDParent.HasValue Then oFeatures.Add(oItem)
                Else
                    If oItem.IDParent.HasValue AndAlso oItem.IDParent = idParentFeature Then
                        oFeatures.Add(oItem)
                    End If
                End If
            Next

            Dim hTRow As HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim divFeature As HtmlGenericControl
            Dim divAnchorFeature As HtmlGenericControl
            Dim divOpenFeature As HtmlGenericControl
            Dim divInfoFeature As HtmlGenericControl
            Dim aAnchor As HtmlAnchor
            Dim aAnchorInfo As HtmlAnchor
            Dim iHtmlImg As HtmlImage

            ' Obtenemos traducción tooltip botón mostrar información
            Dim strFeatureInformationButton As String = Me.Language.Translate("Feature.Information.Button", Me.DefaultScope)

            For Each oFeature As Feature In oFeatures

                If oFeature.MaxConfigurable > Permission.None Then

                    ' Pinta la fila con el nombre de la categoría actual y sus permisos
                    hTRow = New HtmlTableRow
                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " " & "FeaturesTableStyle-noendcellLevel" & intLevel

                    aAnchor = New HtmlAnchor
                    With aAnchor
                        .HRef = "javascript:void(0);"
                        .Style("width") = "100%"
                        .Attributes("class") = "FeatureAnchor"
                        If oFeature.IsGroup Then
                            .Attributes("onclick") = "ShowHideFeatureChilds('" & oFeature.ID & "');"
                        Else
                            .Style("cursor") = "default"
                        End If
                        .InnerHtml = oFeature.Name ' Me.Language.Translate("Feature." & oFeature.Alias & ".Name", Me.DefaultScope)
                    End With

                    iHtmlImg = New HtmlImage
                    With iHtmlImg
                        .Alt = ""
                        .ID = "aFeatureOpenImg" & oFeature.ID
                        If oFeature.IsGroup Then
                            .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/tree/elbow-minus-nl.gif")
                            .Attributes("onclick") = "ShowHideFeatureChilds('" & oFeature.ID & "');"
                            .Style("cursor") = "pointer"
                        Else
                            .Style("min-width") = "16px"
                            .Src = Me.Page.ResolveUrl("~/Base/ext-3.4.0/resources/images/default/s.gif")
                        End If
                    End With

                    divFeature = New HtmlGenericControl("div")
                    divFeature.Style("width") = "100%"

                    divOpenFeature = New HtmlGenericControl("div")
                    divOpenFeature.Style("float") = "left"
                    divOpenFeature.Style("text-align") = "left"
                    divOpenFeature.Controls.Add(iHtmlImg)

                    divAnchorFeature = New HtmlGenericControl("div")
                    divAnchorFeature.Style("float") = "left"
                    divAnchorFeature.Style("text-align") = "left"
                    divAnchorFeature.Style("margin-top") = "2px"
                    divAnchorFeature.Controls.Add(aAnchor)

                    divInfoFeature = New HtmlGenericControl("div")
                    divInfoFeature.Style("float") = "right"
                    divInfoFeature.Style("text-align") = "right"
                    aAnchorInfo = New HtmlAnchor
                    With aAnchorInfo
                        .ID = "ctl00_contentMainBody_ASPxCallbackPanelContenido_aFeatureInfo" & oFeature.ID
                        .HRef = "javascript:void(0);"
                        .Attributes("class") = "FeatureInfoAnchor"
                        .Attributes("onclick") = "ShowHideFeatureInfo(this, '" & oFeature.ID & "');"
                        .Title = strFeatureInformationButton
                    End With
                    divInfoFeature.Controls.Add(aAnchorInfo)

                    divFeature.Controls.Add(divOpenFeature)
                    divFeature.Controls.Add(divAnchorFeature)
                    divFeature.Controls.Add(divInfoFeature)
                    hTCell.Controls.Add(divFeature)

                    hTRow.Cells.Add(hTCell)
                    hTCell = New HtmlTableCell
                    hTCell.Attributes("class") = "FeaturesTableStyle-cellLevel" & intLevel & " FeaturesTableStyle-cellPermissions"
                    hTCell.Attributes("align") = "right"
                    hTCell.Style("padding") = "3px"
                    hTCell.Controls.Add(Me.CreateFeaturePermissionsTable(idSecurityFunction, oSecurityFunctionFeatures, oFeature, oUpdatedPermissions))
                    hTRow.Cells.Add(hTCell)

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)

                    ' Pinta fila con la información de la funcionalidad
                    hTRow = New HtmlTableRow
                    hTRow.ID = "rowFeatureInfo" & oFeature.ID
                    hTRow.Style("display") = "none"
                    hTCell = New HtmlTableCell
                    hTCell.Style("padding-left") = (5 * 10) & "px"
                    hTCell.Controls.Add(Me.CreateFeatureInfoTable(oFeature))
                    hTRow.Cells.Add(hTCell)
                    hTCell = New HtmlTableCell
                    hTRow.Cells.Add(hTCell)

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)

                End If

                If oFeature.IsGroup Then

                    Dim hChildsTable As New HtmlTable
                    With hChildsTable
                        .ID = "ctl00_contentMainBody_ASPxCallbackPanelContenido_tbFeatureChilds" & oFeature.ID
                        .Border = 0
                        .CellPadding = 0
                        .CellSpacing = 0
                        .Attributes("class") = "FeaturesTableStyle GridFeatureChilds"
                    End With
                    Dim oChildFeatureRows As Generic.List(Of HtmlTableRow) = Me.GetFeatures(idSecurityFunction, oFeaturesAll, oSecurityFunctionFeatures, oFeature.ID, type, intLevel + 1, oUpdatedPermissions)
                    If oChildFeatureRows IsNot Nothing Then
                        For Each oRow As HtmlTableRow In oChildFeatureRows
                            hChildsTable.Rows.Add(oRow)
                        Next
                    End If

                    hTRow = New HtmlTableRow
                    hTRow.ID = "rowFeatureChilds" & oFeature.ID
                    'hTRow.Style("display") = "none"

                    hTCell = New HtmlTableCell
                    hTCell.ColSpan = 2
                    hTCell.Style("padding-left") = "15px" '(intLevel * 10) & "px"
                    hTCell.Controls.Add(hChildsTable)

                    hTRow.Cells.Add(hTCell)

                    ' Añadimos fila la colección
                    oFeatureRows.Add(hTRow)

                End If

            Next

        End If

        Return oFeatureRows

    End Function

    Private Function CreateFeaturePermissionsTable(ByVal idSecurityFunction As Integer, ByVal oSecurityFunctionFeatures As roGroupFeaturePermissionsOverFeature(), ByVal oFeature As Feature, ByRef oUpdatedPermissions As Generic.List(Of FeaturePermission)) As HtmlTable

        Dim oRet As New HtmlTable

        Dim oFeaturePermissions As Permission() = oFeature.Permissions.ToArray
        If oFeaturePermissions IsNot Nothing Then

            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim aPermission As HtmlAnchor
            'Dim aTooltip As HtmlGenericControl

            With oRet
                .Border = 0
                .CellPadding = 0
                .CellSpacing = 0
                .Attributes("class") = "GridStFeaturesTableStyleyle GridPermissions"
                .Align = "right"
            End With

            For Each oPermission As Permission In oFeaturePermissions

                hTCell = New HtmlTableCell

                Dim tooltipText As String = ""
                Select Case oPermission
                    Case Permission.Admin
                        tooltipText = Me.Language.Translate("PermissionList.Tooltip.Admin", Me.DefaultScope)
                    Case Permission.Write
                        tooltipText = Me.Language.Translate("PermissionList.Tooltip.Write", Me.DefaultScope)
                    Case Permission.Read
                        tooltipText = Me.Language.Translate("PermissionList.Tooltip.Read", Me.DefaultScope)
                    Case Permission.None
                        tooltipText = Me.Language.Translate("PermissionList.Tooltip.None", Me.DefaultScope)
                End Select

                Dim oTempPerm As New FeaturePermission()
                oTempPerm.IDFeature = oFeature.ID
                oTempPerm.IDPermission = CInt(oPermission)

                If oSecurityFunctionFeatures IsNot Nothing Then
                    oTempPerm.Checked = PermissionSelected(oFeature, oSecurityFunctionFeatures, oPermission)
                Else
                    oTempPerm.Checked = False
                End If

                oUpdatedPermissions.Add(oTempPerm)

                aPermission = New HtmlAnchor
                aPermission.ID = "aFeaturePermission" & oPermission.ToString & "_" & oFeature.ID
                aPermission.HRef = "javascript:void(0);"
                aPermission.Attributes("class") = "Permission" & oPermission.ToString
                aPermission.Title = tooltipText

                If oTempPerm.Checked Then
                    aPermission.Attributes("class") &= " PermissionPressed"
                Else
                    aPermission.Attributes("class") &= " PermissionUnPressed"
                End If


                If oFeature.Alias = "Employees.Complaints" AndAlso WLHelperWeb.CurrentUserIsConsultantOrCegid Then
                    aPermission.Style.Item("cursor") = "not-allowed"
                    aPermission.Attributes("onclick") = ""
                    aPermission.Title = Me.Language.Translate("PermissionList.Tooltip.FeatureForbidden", Me.DefaultScope)
                Else
                    If Me.oPermission < Permission.Write Then
                        aPermission.Style.Item("cursor") = "default"
                        aPermission.Attributes("onclick") = ""
                    Else
                        If idSecurityFunction >= 0 AndAlso oSecurityFunctionFeatures IsNot Nothing Then
                            If (idSecurityFunction = 0 AndAlso PermissionSelected(oFeature, oSecurityFunctionFeatures, Permission.None) AndAlso oPermission > Permission.Read) OrElse idSecurityFunction > 0 Then
                                aPermission.Attributes("onclick") = "UpdFeaturePermission('" & oFeature.ID & "', '" & oPermission.ToString & "');"
                            Else
                                aPermission.Style.Item("cursor") = "default"
                                aPermission.Attributes("onclick") = ""
                            End If
                        Else
                            aPermission.Attributes("onclick") = "UpdFeaturePermission('" & oFeature.ID & "', '" & oPermission.ToString & "');"
                        End If
                    End If


                End If

                'hTCell.Controls.Add(aTooltip)
                hTCell.Controls.Add(aPermission)
                hTRow.Cells.Add(hTCell)
            Next
            oRet.Rows.Add(hTRow)

        End If

        Return oRet

    End Function

    Private Function PermissionSelected(ByVal oFeature As Feature, ByVal oSecurityFunctionFeatures As roGroupFeaturePermissionsOverFeature(), ByVal oPermission As Permission) As Boolean
        Dim bPressed As Boolean = False

        For Each oSecurityFeature As roGroupFeaturePermissionsOverFeature In oSecurityFunctionFeatures
            If oSecurityFeature.IDFeature = oFeature.ID AndAlso (
                (oSecurityFeature.Permision = 9 AndAlso oPermission = Permission.Admin) _
                OrElse (oSecurityFeature.Permision = 6 AndAlso oPermission = Permission.Write) _
                OrElse (oSecurityFeature.Permision = 3 AndAlso oPermission = Permission.Read) _
                OrElse (oSecurityFeature.Permision = 0 AndAlso oPermission = Permission.None)) Then
                bPressed = True
                Exit For
            End If
        Next

        Return bPressed
    End Function

    Private Function CreateFeatureInfoTable(ByVal oFeature As Feature) As HtmlTable

        Dim hInfo As New HtmlTable

        With hInfo
            .ID = "tbFeatureInfo" & oFeature.ID
            .Border = 0
            .CellPadding = 0
            .CellSpacing = 0
            .Attributes("class") = "FeatureInfoTable"
            '.Style("display") = "none"
        End With
        Dim hTRow As New HtmlTableRow
        Dim hTCell As New HtmlTableCell
        hTCell.InnerHtml = oFeature.Description
        hTRow.Cells.Add(hTCell)

        hInfo.Rows.Add(hTRow)

        Return hInfo

    End Function

#End Region

End Class