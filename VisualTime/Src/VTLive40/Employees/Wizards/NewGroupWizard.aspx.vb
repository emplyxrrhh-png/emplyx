Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Wizards_NewGroupWizard
    Inherits PageBase

    Private intActivePage As Integer

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.Groups", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        If Not Me.IsPostBack Then

            Dim intIdGroup As Integer = roTypes.Any2Integer(Request.Params("GroupID"))
            If intIdGroup <= 0 Then
                btNext.Visible = False
                btPrev.Visible = False
                lblNewGroupWelcome3.Text = "GRUPO DEPENDIENTE INCORRECTO. NO ES POSIBLE CONTINUAR"
            Else
                hdnIdGroup.Value = intIdGroup

                Me.btClose.Visible = Not Me.IsPopup

                Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text

                Me.intActivePage = 0

                Dim GroupName As String = roTypes.Any2String(Request.Params("GroupName"))
                Me.lblStep1Info2b.Text = GroupName.ToUpper()
            End If
        Else

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1

        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim intOldPage As Integer = Me.intActivePage
            Me.intActivePage += 1

            Me.PageChange(intOldPage, Me.intActivePage)

        End If

    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim intOldPage As Integer = Me.intActivePage
        Me.intActivePage -= 1

        Me.PageChange(intOldPage, Me.intActivePage)

    End Sub

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckPage(Me.intActivePage) Then

            Me.PageChange(1, 2)

            Dim oParentGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, hdnIdGroup.Value, False)

            Dim oGroup As New roGroup()
            With oGroup
                .ID = -1
                .Name = Me.txtGroupName.Text
                .Path = oParentGroup.Path
                .SecurityFlags = oParentGroup.SecurityFlags
                .DescriptionGroup = Me.txtDescription.Text
            End With

            Dim strErrorInfo As String = ""
            Me.lblNewGroupWelcome1.Text = Me.Language.Translate("End.NewGroupWelcome1.Text", Me.DefaultScope)
            Dim intNewGroupID As Integer = API.EmployeeGroupsServiceMethods.SaveGroup(Me, oGroup, True)
            If intNewGroupID <> -1 Then

                oGroup.ID = intNewGroupID

                Me.MustRefresh = "8"

                ' Añadimos grupo a la selección del contexto
                Dim oContext As WebCContext = WLHelperWeb.Context(Me.Request)
                Dim lstGroups As ArrayList = oContext.Groups
                Dim lstGroupNames As ArrayList = oContext.GroupNames
                lstGroups.Add(oGroup.ID)
                lstGroupNames.Add(oGroup.Name)
                oContext.Groups = lstGroups
                oContext.GroupNames = lstGroupNames

                Me.lblNewGroupWelcome2.Text = Me.Language.Translate("End.Ok.NewGroupWelcome2.Text", Me.DefaultScope)
                Me.lblNewGroupWelcome3.Text = ""
            Else
                Me.lblNewGroupWelcome2.Text = Me.Language.Translate("End.Error.NewGroupWelcome2.Text", Me.DefaultScope)
                Me.lblNewGroupWelcome3.Text = strErrorInfo
                Me.lblNewGroupWelcome3.ForeColor = Drawing.Color.Red
            End If
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(2, 0)

        End If

    End Sub

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                If Me.txtGroupName.Text.Length = 0 Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.InvalidGroupName", Me.DefaultScope)
                End If

                If strMsg = "" Then
                    If API.EmployeeGroupsServiceMethods.GetGroup(Me, Me.hdnIdGroup.Value, False) Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.GroupSelectedNotExist", Me.DefaultScope)
                    End If
                End If

                If strMsg = "" Then
                    If Me.GroupNameExists(Me.txtGroupName.Value, Me.hdnIdGroup.Value) Then
                        strMsg = Me.Language.Translate("CheckPage.Page1.ExistGroupName", Me.DefaultScope)
                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intActivePage
            Case 1
                Me.txtGroupName.Focus()
        End Select

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intOldPage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If

        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intActivePage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = IIf(intOldPage = 2, False, True) '.Style("display") = IIf(intOldPage = 2, "none", "block")
            Me.btEnd.Visible = False '.Style("display") = "none"
        ElseIf intActivePage = 1 Then
            Me.btPrev.Visible = True '.Style("display") = "block"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = True '.Style("display") = "block"
        ElseIf intActivePage = 2 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
            Me.btClose.Visible = True '.Style("display") = "block"
        End If

    End Sub

    Private Function GroupNameExists(ByVal strName As String, Optional ByVal intIDParent As Integer = -1) As Boolean

        Dim oGroup As roGroup = Nothing
        If intIDParent = -1 Then
            oGroup = API.EmployeeGroupsServiceMethods.GetGroupByNameInLevel(Me, strName, "")
        Else
            Dim oGroupParent As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, intIDParent, False)
            If oGroupParent IsNot Nothing Then
                oGroup = API.EmployeeGroupsServiceMethods.GetGroupByNameInLevel(Me, strName, oGroupParent.Path)
            End If
        End If

        Return (oGroup IsNot Nothing)

    End Function

End Class