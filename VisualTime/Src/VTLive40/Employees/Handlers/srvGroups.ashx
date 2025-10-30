<%@ WebHandler Language="VB" Class="srvGroups" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTBusiness.Group

Public Class srvGroups
    Inherits handlerBase

    Private Const FeatureAlias As String = "Employees"
    Private Const FeatureGroupAlias As String = "Employees.Groups"
    Private Const FeatureMobilityAlias As String = "Employees.GroupMobility"
    Private Const FeatureKpiAlias As String = "KPI.Definition"
    Private Const FeatureCostCenterAlias As String = "Employees.BusinessCenters"
    Private Const FeatureAssignActivities As String = "Documents.Permision"


    Private oContext As WebCContext = Nothing
    Private CurrentIDGroup As Integer

    'Textes de Descripcio
    Private strInfo As String = ""
    Private actualTab As Integer = 0 'TAB actual 

    Private oPermission As Permission
    Private oCurrentPermission As Permission        ' Permiso configurado sobre el grupo actual
    Private oPermissionMobility As Permission
    Private oPermissionGroup As Permission
    Private oPermissionKpi As Permission
    Private oPermissionCostCenter As Permission
    Private oPermissionActivities As Permission

    Private oUserFieldsPermission As Permission     ' Permiso configurado sobre la información de la ficha del empleado actual ('Employees.UserFields.Information')
    Private oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None} ' Permiso configurado sobre la información de la ficha para los distintos niveles de acceso del empleado actual ('Employees.UserFields.Information.Low', 'Employees.UserFields.Information.Medium', 'Employees.UserFields.Information.High')    

    Private bolMultiCompanyLicense As Boolean = False
    Private bolKPIsLicense As Boolean = False
    Private bolCostCentersLicense As Boolean = False
    Private bolDocumentsLicense As Boolean = False
    Private bolAccesslicense As Boolean = False
    Private iAccessGroupMode As Integer = 0


    Private bolIsCompany As Boolean = False

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "Employees"
        CurrentIDGroup = Request("ID")

        Me.oPermission = Me.GetFeaturePermissionByGroup(FeatureAlias, Me.CurrentIDGroup)
        Me.oPermissionGroup = Me.GetFeaturePermission(FeatureGroupAlias)
        Me.oCurrentPermission = Me.GetFeaturePermissionByGroup(FeatureAlias, CurrentIDGroup)
        Me.oPermissionKpi = Me.GetFeaturePermission(FeatureKpiAlias)
        Me.oPermissionCostCenter = Me.GetFeaturePermission(FeatureCostCenterAlias)
        Me.oPermissionActivities = GetFeaturePermission(FeatureAssignActivities)

        Me.oUserFieldsPermission = Me.GetFeaturePermissionByGroup(FeatureAlias & ".UserFields.Information", CurrentIDGroup)
        Me.oUserFieldsAccessPermission(0) = Me.GetFeaturePermissionByGroup(FeatureAlias & ".UserFields.Information.Low", CurrentIDGroup)
        Me.oUserFieldsAccessPermission(1) = Me.GetFeaturePermissionByGroup(FeatureAlias & ".UserFields.Information.Medium", CurrentIDGroup)
        Me.oUserFieldsAccessPermission(2) = Me.GetFeaturePermissionByGroup(FeatureAlias & ".UserFields.Information.High", CurrentIDGroup)

        If Me.oPermission > Permission.None And Me.oCurrentPermission > Permission.None Then

            '-> Me.bolMultiCompanyLicense = LicenseService.LicenseServiceMethods.FeatureIsInstalled("Feature\MultiCompany")
            Me.bolMultiCompanyLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\MultiCompany")
            Me.bolKPIsLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\KPIs")
            Me.bolCostCentersLicense = HelperSession.GetFeatureIsInstalledFromApplication("Feature\CostControl")
            Me.bolDocumentsLicense = (HelperSession.GetFeatureIsInstalledFromApplication("Feature\Absences") OrElse HelperSession.GetFeatureIsInstalledFromApplication("Feature\Documents"))
            Me.bolAccesslicense = HelperSession.GetFeatureIsInstalledFromApplication("Forms\Access")

            iAccessGroupMode = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("AdvancedAccessMode"))

            Select Case context.Request("action")
                Case "getGroupsTab" ' Retorna un Grup (Tab Superior)
                    LoadGroupDataTab(False)
                Case "getGroupsTabLite"
                    LoadGroupDataTab(True)
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "getBarButtonsLite"
                    GetBarButtonsLite(Request("ID"))
                Case "chgNameGroup" 'Cambia el Nom del grup seleccionat
                    changeNameGroup()
                Case "deleteGroup" 'Borra el grup seleccionat
                    DeleteGroup(Request("ID"))
            End Select

        Else
            ' Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Dim strResponse As String = "MESSAGE" &
                          "TitleKey=CheckPermission.Denied.Title&" +
                          "DescriptionKey=CheckPermission.Denied.Description&" +
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" +
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Response.Write(strResponse)
        End If
    End Sub

    ''' <summary>
    ''' Borra el grup seleccionat
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteGroup(ByVal sID As String)
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            bolRet = Robotics.Web.Base.API.EmployeeGroupsServiceMethods.DeleteGroup(Nothing, sID, True)
            If Not bolRet Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeGroupState.ErrorText
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally
            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=DeleteGroup.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=DeleteGroup.Error.Option1Text&" +
                              "Option1DescriptionKey=DeleteGroup.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)
        End Try
    End Sub

    ''' <summary>
    ''' Carrega el grup per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadGroupDataTab(ByVal bLiteCharge As Boolean)
        Try
            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, Request("ID"), False)
            If oGroup Is Nothing Then Exit Sub
            bolIsCompany = oGroup.ID.ToString = oGroup.Path
            Me.CurrentIDGroup = Request("ID")
            actualTab = Request("aTab")

            If oGroup.ID.ToString <> oGroup.Path And actualTab > 0 Then
                actualTab = actualTab - 1
            End If

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = System.Web.VirtualPathUtility.ToAbsolute("~/Base/Images/groupDefault80.png")
            ' Si estamos cargando un grupo de primer nivel y hay la licencia de multi empresa activada, mostramos el icono de empresa
            If oGroup.Path = oGroup.ID.ToString AndAlso Me.bolMultiCompanyLicense Then
                img.Src = "Images/Company80.png"
            End If
            oImageDiv.Controls.Add(img)

            Dim strChangeNameJS As String = "onclick=""EditNameGroup('true');"""
            ' Miramos si el pasport acutal tiene permisos para modificar el nombre
            If Me.oPermissionGroup < Permission.Write Then
                strChangeNameJS = ""
            End If

            Dim dtblCurrent As DataTable
            dtblCurrent = Me.CurrentEmployeesData
            Dim intEmployeeCount As Integer = dtblCurrent.Rows.Count

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameGroup"" class=""NameText"">  " & oGroup.Name & "</span></div>" & vbNewLine &
                                    oGroup.FullGroupName & "<br />" &
                                    Me.Language.Translate("EmployeesInGroup", DefaultScope) & " " & intEmployeeCount & "/" & CurrentEmployeesRecursiveData.Count

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateGroupTabs(bLiteCharge))

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            oMainDiv.RenderControl(htw)

            Response.Write(sw.ToString)
        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Genera els botons de Usuaris de la dreta (General, Contratos, Fichajes, Ausencias Previstas...)
    ''' </summary>
    ''' <returns>Retorna un HTML Table amb els botons en format columna</returns>
    ''' <remarks></remarks>
    Private Function CreateGroupTabs(ByVal bLiteCharge As Boolean) As HtmlTable
        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell


        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim intindex As Integer

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0



        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}

        If bLiteCharge Then

            oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_UserFields", Me.Language.Translate("tabGeneralEmployees", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(0))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableRowButtons.VAlign = "top"
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
            intindex = 0
            oTabButtons(0).Attributes.Add("OnClick", "javascript: changeCompanyTabs(0);")

            If actualTab > intindex Then actualTab = 0

            oTabButtons(actualTab).Attributes("class") = "bTab-active"
            hCellGen.Controls.Add(hTableButtons)

            hRowGen.Cells.Add(hCellGen)

            'Documentos de grupo y empleados
            intindex = intindex + 1
            ReDim Preserve oTabButtons(intindex)
            oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_GroupDocuments", Me.Language.Translate("tabGroupDocuments", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(intindex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(1);")

            hCellGen.Controls.Add(hTableButtons)
            hRowGen.Cells.Add(hCellGen)
        Else
            'General
            oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_GeneralGroups", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(0))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableRowButtons.VAlign = "top"
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
            intindex = 0
            oTabButtons(0).Attributes.Add("OnClick", "javascript: changeCompanyTabs(0);")

            ' Centros de coste
            If (bolCostCentersLicense AndAlso oPermissionCostCenter > Permission.None) Then
                intindex = intindex + 1
                ReDim Preserve oTabButtons(intindex)
                oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_CostCenters", Me.Language.Translate("tabCostCenters", Me.DefaultScope), "bTab")
                hTableCellButtons.Controls.Add(oTabButtons(intindex))
                hTableRowButtons.Cells.Add(hTableCellButtons)
                hTableButtons.Rows.Add(hTableRowButtons)
                oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(3);")
            End If

            intindex = intindex + 1
            ReDim Preserve oTabButtons(intindex)
            oTabButtons(intindex) = Nothing
            oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_UserFields", Me.Language.Translate("tabEmployees", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(intindex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(1);")


            hCellGen.Controls.Add(hTableButtons)

            hRowGen.Cells.Add(hCellGen)


            hCellGen = New HtmlTableCell
            hCellGen.Attributes("valign") = "top"

            'Regenerem la taula
            hTableButtons = New HtmlTable
            hTableRowButtons = New HtmlTableRow
            hTableCellButtons = New HtmlTableCell

            hTableButtons.Border = 0
            hTableButtons.CellSpacing = 0
            hTableButtons.CellPadding = 0


            If Me.bolKPIsLicense AndAlso oPermissionKpi > Permission.None Then
                intindex = intindex + 1
                ReDim Preserve oTabButtons(intindex)
                oTabButtons(intindex) = Nothing

                oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_GroupIndicators", Me.Language.Translate("tabGroupIndicators", Me.DefaultScope), "bTab")
                hTableCellButtons.Controls.Add(oTabButtons(intindex))
                hTableRowButtons.Cells.Add(hTableCellButtons)
                hTableButtons.Rows.Add(hTableRowButtons)
                oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(2);")
                hCellGen.Controls.Add(hTableButtons)
                hRowGen.Cells.Add(hCellGen)
            End If

            ' Documentos
            If (bolDocumentsLicense AndAlso oPermissionActivities > Permission.None) Then
                If (bolIsCompany = True) Then
                    intindex = intindex + 1
                    ReDim Preserve oTabButtons(intindex)
                    oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_CompanyDocuments", Me.Language.Translate("tabCompanyDocuments", Me.DefaultScope), "bTab")
                    hTableCellButtons.Controls.Add(oTabButtons(intindex))
                    hTableRowButtons.Cells.Add(hTableCellButtons)
                    hTableButtons.Rows.Add(hTableRowButtons)
                    oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(4);")

                    hCellGen.Controls.Add(hTableButtons)
                    hRowGen.Cells.Add(hCellGen)
                Else
                    'intindex = intindex + 1
                    'ReDim Preserve oTabButtons(intindex)
                    'oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_GroupDocuments", Me.Language.Translate("tabGroupDocuments", Me.DefaultScope), "bTab")
                    'hTableCellButtons.Controls.Add(oTabButtons(intindex))
                    'hTableRowButtons.Cells.Add(hTableCellButtons)
                    'hTableButtons.Rows.Add(hTableRowButtons)
                    'oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(6);")

                    'hCellGen.Controls.Add(hTableButtons)
                    'hRowGen.Cells.Add(hCellGen)
                End If
            Else
                If actualTab = intindex + 1 Then actualTab = 0
            End If

            If bolAccesslicense AndAlso iAccessGroupMode = 1 Then
                intindex = intindex + 1
                ReDim Preserve oTabButtons(intindex)
                oTabButtons(intindex) = Nothing

                oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_CompanyAccessAuthorizations", Me.Language.Translate("tabCompanyAccessAuthorizations", Me.DefaultScope), "bTab")
                hTableCellButtons.Controls.Add(oTabButtons(intindex))
                hTableRowButtons.Cells.Add(hTableCellButtons)
                hTableButtons.Rows.Add(hTableRowButtons)
                oTabButtons(intindex).Attributes.Add("OnClick", "javascript: changeCompanyTabs(5);")
                hCellGen.Controls.Add(hTableButtons)
                hRowGen.Cells.Add(hCellGen)
            End If



            If actualTab > intindex Then actualTab = 0

            oTabButtons(actualTab).Attributes("class") = "bTab-active"
        End If

        hTableGen.Rows.Add(hRowGen)


        Return hTableGen ' Retorna el HTMLTable

    End Function

    ''' <summary>
    ''' Genera automaticament HtmlAnchors
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="CssClassPrefix">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\Company\Groups", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Employees")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub GetBarButtonsLite(ByVal sID As String)
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Employees\Employees", WLHelperWeb.CurrentPassportID)
            Dim guiActionsGroups As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Employees\GroupsLite", WLHelperWeb.CurrentPassportID)

            For Each action As roGuiAction In guiActions
                If action.IDPath <> "MaxMinimize" Then
                    If action.IDPath <> "EmployeeDelWzd" Then
                        guiActionsGroups.Add(action)
                    End If
                End If
            Next

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActionsGroups, sID, Me.Language, Me.DefaultScope, "Employees")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Retorna un Datatable amb els Usuaris del grup actuals
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CurrentEmployeesData() As DataTable
        Try
            Dim tb As DataTable = Nothing
            Dim ds As DataSet = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroup(Nothing, Me.CurrentIDGroup, API.EmployeeGroupsServiceMethods.eEmployeesFromGroup.Current, FeatureAlias)
            If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                tb = ds.Tables(0)
            End If

            Return tb
        Catch ex As Exception
            'TODO: Retornar missatge error
            Return Nothing
        End Try
    End Function

    ''' <summary>
    ''' Retorna un Datatable amb els Usuaris del grup actuals
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CurrentEmployeesRecursiveData() As List(Of Integer)
        Try
            Dim lst As List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Nothing, {Me.CurrentIDGroup}, FeatureAlias, "U", "", "")

            Return lst
        Catch ex As Exception
            'TODO: Retornar missatge error
            Return New List(Of Integer)
        End Try
    End Function

    ''' <summary>
    ''' Cambia el nom del grup
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub changeNameGroup()
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            If Me.oCurrentPermission >= Permission.Write Then
                Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, Request("ID"), False)

                If oGroup Is Nothing Then Exit Sub
                CurrentIDGroup = Request("ID")

                Dim NouName As String = Request("NewName")
                If NouName = "" Then
                    strErrorInfo = Me.Language.Translate("NameInvalid", "srvEmployees") 'Nombre no valido
                Else
                    oGroup.Name = NouName
                    bolRet = API.EmployeeGroupsServiceMethods.SaveGroup(Nothing, oGroup, True)
                    If Not bolRet Then
                        strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeGroupState.ErrorText
                    End If
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        Finally
            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=ChangeNameGroup.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=ChangeNameGroup.Error.Option1Text&" +
                              "Option1DescriptionKey=ChangeNameGroup.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)

        End Try
    End Sub

End Class