Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Shift
Imports Robotics.Web.Base

Partial Class Wizards_NewShiftGroupWizard
    Inherits PageBase

#Region "Declarations"

    Private intActivePage As Integer

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Shifts.Definition", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        If Not Me.IsPostBack Then

            Me.btClose.Visible = Not Me.IsPopup

            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text

            HelperWeb.roSelector_Initialize("roChildSelectorW_treeShiftGroupNewShiftGroupWizard")

            Me.intActivePage = 0
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

            Me.PageChange(1, 0)

            Dim oShiftGroup As New roShiftGroup
            With oShiftGroup
                .ID = -1
                .Name = Me.txtShiftGroupName.Value
                .BusinessGroup = Me.txtBusinessGroup.Value
            End With

            Dim strErrorInfo As String = ""
            Me.lblNewShiftGroupWelcome1.Text = Me.Language.Translate("End.NewShiftGroupWelcome1.Text", Me.DefaultScope)
            If API.ShiftServiceMethods.SaveShiftGroup(Me, oShiftGroup, True) Then
                Me.MustRefresh = "8"
                Me.lblNewShiftGroupWelcome2.Text = Me.Language.Translate("End.Ok.NewShiftGroupWelcome2.Text", Me.DefaultScope)
                Me.lblNewShiftGroupWelcome3.Text = ""

                Dim treePath As String = "/source/A" & oShiftGroup.ID
                HelperWeb.roSelector_SetSelection("A" & oShiftGroup.ID.ToString, treePath, "ctl00_contentMainBody_roTreesShifts")
            Else
                Me.lblNewShiftGroupWelcome2.Text = Me.Language.Translate("End.Error.NewShiftGroupWelcome2.Text", Me.DefaultScope)
                Me.lblNewShiftGroupWelcome3.Text = strErrorInfo
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(1, 0)

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                If Me.txtShiftGroupName.Value.Length = 0 Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.InvalidShiftGroupName", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
        End Select

        Select Case intActivePage
            Case 1
                Me.txtShiftGroupName.Focus()
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

        If intOldPage = 1 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

#End Region

    Protected Sub btClose_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btClose.Click
        Me.CanClose = True
        Me.MustRefresh = "8"
    End Sub

End Class