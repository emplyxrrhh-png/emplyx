<%@ WebHandler Language="VB" Class="srvAccessGroups" %>

Imports System
Imports System.Web
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API
Imports Robotics.VTBase
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup

Public Class srvAccessGroups
    Inherits handlerBase

    Private oPermission As Permission
    Private oPermissionAss As Permission

    Public Property InitialEmployees() As Generic.List(Of roEmployeeDescription)
        Get
            Return Me.Session("AccessGroups_InitialEditValues")
        End Get
        Set(ByVal value As Generic.List(Of roEmployeeDescription))
            If value Is Nothing Then
                Me.Session("AccessGroups_InitialEditValues") = Nothing
            Else
                Me.Session("AccessGroups_InitialEditValues") = value
            End If

        End Set
    End Property

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)

        Me.oPermission = Me.GetFeaturePermission("Access.Groups.Definition")
        Me.oPermissionAss = Me.GetFeaturePermission("Access.Groups.Assign")

        If oPermission > Permission.None Then
            Select Request("action")
                Case "getAccessGroupTab"
                    LoadAccessGroupDataTab()
                Case "deleteAccessGroup"
                    DeleteAccessGroup(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "canSaveAccessGroups"
                    CanSaveAccessGroups()
                Case "copyXAccessGroup"
                    CopyAccessDataX(Request("ID"))
                Case "emptyAccessGroupEmp"
                    EmptyAccessGroupEmployees(Request("ID"))
            End Select

        Else

            Dim strResponse As String = "MESSAGE" & _
                          "TitleKey=CheckPermission.Denied.Title&" + _
                          "DescriptionKey=CheckPermission.Denied.Description&" + _
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" + _
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)

        End If

    End Sub

    Private Sub EmptyAccessGroupEmployees(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermissionAss < Permission.Write Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            Dim oNewAccess As Boolean = AccessGroupServiceMethods.EmptyAccessGroupEmployees(Nothing, oID)

            If oNewAccess Then
                rError = New roJSON.JSONError(False, "OK:" & oID)
            Else
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.AccessGroupState.ErrorText)
            End If

            If rError.Error = False Then
                HelperWeb.roSelector_SetSelection(oID.ToString, "/source/" & oID.ToString, "ctl00_contentMainBody_roTreesAccessGroups")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub CopyAccessDataX(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            Dim oNewAccess As roAccessGroup = AccessGroupServiceMethods.CopyAccess(Nothing, oID)

            If oNewAccess Is Nothing Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.AccessGroupState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK:" & oNewAccess.ID)
            End If

            If rError.Error = False Then
                HelperWeb.roSelector_SetSelection(oNewAccess.ID.ToString, "/source/" & oNewAccess.ID.ToString, "ctl00_contentMainBody_roTreesAccessGroups")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub


    Private Sub LoadAccessGroupDataTab()
        Try

            Dim oCurrentGroup As roAccessGroup

            If Request("ID") = "-1" Then
                oCurrentGroup = New roAccessGroup
            Else
                oCurrentGroup = AccessGroupServiceMethods.GetAccessGroupByID(Nothing, Request("ID"), True, If(HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1"), WLHelperWeb.CurrentPassportID, 0))
            End If

            Dim actualEmployeesInGroup As Integer = 0

            If oCurrentGroup Is Nothing Then Exit Sub

            If oCurrentGroup.Employees IsNot Nothing Then
                Dim iEmployees As New Generic.List(Of roEmployeeDescription)
                iEmployees.AddRange(oCurrentGroup.Employees)
                InitialEmployees = iEmployees
                actualEmployeesInGroup = iEmployees.Count
            Else
                InitialEmployees = Nothing
            End If

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))
            If intActiveTab > 1 Then intActiveTab = 0

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/AccessGroups80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameAccessGroup"" class=""NameText"">" & oCurrentGroup.Name & " </span></div>" &
                                    Me.Language.Translate("EmployeesInGroup", DefaultScope) & " " & actualEmployeesInGroup

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(intActiveTab))

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            oMainDiv.RenderControl(htw)

            Response.Write(sw.ToString)

        Catch ex As Exception
            Response.Write("MESSAGE" & _
                           "TitleKey=SaveName.Error.Text&" & _
                           "DescriptionText=" & ex.ToString & "&" & _
                           "Option1TextKey=SaveName.Error.Option1Text&" & _
                           "Option1DescriptionKey=SaveName.Error.Option1Description&" & _
                           "Option1OnClickScript=HideMsgBoxForm(); return false;&" & _
                           "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon))
        End Try
    End Sub

    Private Function CreateTabs(ByRef intActiveTab As Integer) As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGroups", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(1)
        oTabButtons(1) = Nothing

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabEmployees", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Documents") OrElse HelperSession.GetFeatureIsInstalledFromApplication("Feature\OHP") Then
            ReDim Preserve oTabButtons(2)
            oTabButtons(2) = Nothing

            oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_02", Me.Language.Translate("tabDocuments", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(2))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
        End If

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
        Next

        oTabButtons(intActiveTab).Attributes("class") = "bTab-active"

        Return hTableGen

    End Function

    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerText = Text
        Return obutton
    End Function

    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\AccessManagement\AccessGroups\management", WLHelperWeb.CurrentPassportID)


            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "AccessGroups")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub DeleteAccessGroup(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            If AccessGroupServiceMethods.DeleteAccessGroup(Nothing, oID, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.AccessGroupState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub CanSaveAccessGroups()
        Try
            Dim oCurrentGroup As roAccessGroup

            'Check Permissions
            If Me.oPermissionAss < Permission.Write Then
                Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            oCurrentGroup = AccessGroupServiceMethods.GetAccessGroupByID(Nothing, Request("ID"), False, If(HelperSession.AdvancedParametersCache("AccessGroupsMode").Equals("1"), WLHelperWeb.CurrentPassportID, 0))

            If oCurrentGroup Is Nothing Then
                Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.DoesNotExists.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim allExists As Boolean = True
            If oCurrentGroup.Employees IsNot Nothing AndAlso InitialEmployees IsNot Nothing Then
                Dim cEmployees As New Generic.List(Of roEmployeeDescription)
                cEmployees.AddRange(oCurrentGroup.Employees)

                Dim iEmployees As Generic.List(Of roEmployeeDescription) = InitialEmployees

                For Each cEmp As roEmployeeDescription In cEmployees

                    Dim bFound As Boolean = False

                    For Each iEmp As roEmployeeDescription In iEmployees
                        If cEmp.ID = iEmp.ID Then
                            bFound = True
                            Exit For
                        End If
                    Next

                    If Not bFound Then
                        allExists = False
                        Exit For
                    End If
                Next
            End If


            If allExists Then
                Dim rOK As New roJSON.JSONError(False, "OK:" & oCurrentGroup.ID)
                Response.Write(rOK.toJSON)
            Else
                Dim rError As New roJSON.JSONError(True, Me.Language.Translate("AccessGroup.GroupAlreadyModified.Description", DefaultScope))
                Response.Write(rError.toJSON)
            End If

        Catch ex As Exception

        End Try
    End Sub

End Class