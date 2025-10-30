Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class Wizards_SupervisorCopy
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome '0
        frmSelector '1
        frmOptions '2
    End Enum

    Private oActiveFrame As Frame
    Private iOriginID As Integer = 0

#End Region

#Region "Properties"

    Private Property SupervisorsData(Optional ByVal bolReload As Boolean = False) As roPassport()
        Get
            Dim lst As roPassport() = Session("SecurityCopy_SupervisorsData")

            If bolReload OrElse lst Is Nothing Then
                lst = API.SecurityV3ServiceMethods.GetAllAvailableSupervisorsList(Nothing)

                Session("SecurityCopy_SupervisorsData") = lst
            End If
            Return lst
        End Get
        Set(value As roPassport())
            Session("SecurityCopy_SupervisorsData") = value
        End Set
    End Property

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("EmployeesGroupWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmSelector)
                oFrames.Add(Frame.frmOptions)

                ViewState("EmployeesGroupWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("EmployeesGroupWizard_Frames") = value
        End Set
    End Property

#End Region

    Protected Sub frmSecurityActions_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSecurityActions.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub frmSecurityActions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles frmSecurityActions.Load
        Me.InsertCssIncludes(Me.Page)

        CreateColumnsSupervisors()

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Administration.Security", Permission.Write) Then

                BindSupervisorsTreeList(True)

                Me.Frames = Nothing

                Me.SetStepTitles()

                Me.oActiveFrame = Frame.frmWelcome
            Else
                WLHelperWeb.RedirectAccessDenied(True)
            End If
        Else

            Dim oDiv As HtmlControl
            For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                If oDiv.Style("display") <> "none" Then
                    Me.oActiveFrame = Me.FrameByIndex(n)
                    Exit For
                End If
            Next

        End If

        Me.iOriginID = Request.Params("IDOrigin")
    End Sub

#Region "Events"

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click
        If Me.CheckFrame(Me.oActiveFrame) Then
            Dim oOldFrame As Frame = Me.oActiveFrame
            If Me.Frames.Count > Me.FramePos(Me.oActiveFrame) + 1 Then
                Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) + 1)
            End If
            Me.FrameChange(oOldFrame, Me.oActiveFrame)
        End If
    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click
        Dim oOldFrame As Frame = Me.oActiveFrame
        Me.oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) - 1)
        Me.FrameChange(oOldFrame, Me.oActiveFrame)
    End Sub

#End Region

#Region "Methods"

    Private Function FrameIndex(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = CInt(oFrame)
        Return intRet
    End Function

    Private Function FramePos(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = 0
        For n As Integer = 0 To Me.Frames.Count - 1
            If Me.Frames(n) = oFrame Then
                intRet = n
                Exit For
            End If
        Next
        Return intRet
    End Function

    Private Function FrameByIndex(ByVal intIndex As Integer) As Frame
        Dim oRet As Frame = intIndex
        Return oRet
    End Function

    Private Function CheckFrame(ByVal Frame As Frame) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case Frame

            Case Frame.frmSelector

                Dim oDestinationIDs As New Generic.List(Of Integer)

                For Each oID As Integer In grdSupervisors.GetSelectedFieldValues("ID")
                    If oID <> iOriginID Then
                        oDestinationIDs.Add(oID)
                    End If
                Next

                If oDestinationIDs.Count = 0 Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Frame.frmOptions

                'Comprobar acciones de seguridad marcadas
                If 1 = 2 Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectOptionsSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame

            Case Frame.frmSelector
                BindSupervisorsTreeList(False)
            Case Frame.frmOptions
                BindSupervisorsTreeList(False)
        End Select

        Select Case oActiveFrame

            Case Frame.frmSelector
                BindSupervisorsTreeList(False)
            Case Frame.frmOptions
                BindSupervisorsTreeList(False)
        End Select

        Me.hdnActiveFrame.Value = Me.FrameIndex(oActiveFrame)

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & Me.FrameIndex(oOldFrame))
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If

        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & Me.FrameIndex(oActiveFrame))
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 And Me.FramePos(oActiveFrame) = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(Me.FramePos(oActiveFrame) > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub SetStepTitles()

        'Dim oLabel As Label
        'Dim strStep As String = ""
        'For n As Integer = 1 To System.Enum.GetValues(GetType(Frame)).Length - 1
        '    If n > 1 Then
        '        strStep = Me.hdnStepTitle2.Text.Replace("{0}", Me.FramePos(Me.FrameByIndex(n)))
        '        strStep = strStep.Replace("{1}", Me.Frames.Count - 1)
        '    End If
        '    oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Title")
        '    oLabel.Text = Me.hdnStepTitle.Text & strStep
        'Next

    End Sub

#End Region

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckFrame(Me.oActiveFrame) Then

            Me.hiddenShowFile.Value = String.Empty

            Dim bolRet As Boolean = True

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim strErrorInfo As String = ""

            Dim oDestinationIDs As New Generic.List(Of Integer)

            For Each oID As Integer In grdSupervisors.GetSelectedFieldValues("ID")
                If oID <> iOriginID Then
                    oDestinationIDs.Add(oID)
                End If
            Next

            If grdSupervisors.GetSelectedFieldValues("ID").Count > 0 Then

                Dim bCopyRestrictions As Boolean = False
                Dim bCopyCostCenters As Boolean = False
                Dim bCopyRoles As Boolean = False

                bCopyRestrictions = ckCopyCategories.Checked
                bCopyCostCenters = ckCopyGroups.Checked
                bCopyRoles = ckCopyRoles.Checked

                If Not API.SecurityV3ServiceMethods.CopySupervisorProperties(Me.Page, iOriginID, oDestinationIDs.ToArray, ckCopyRestrictions.Checked, False, False, bCopyRestrictions, bCopyCostCenters, bCopyRoles) Then
                    strErrorInfo = roWsUserManagement.SessionObject().States.SecurityV3State.ErrorText
                End If
            End If

            Me.lblWelcomeEmployees.Text = Me.Language.Translate("End.SupervisorCopyWelcome1.Text", Me.DefaultScope)
            If strErrorInfo = "" Then
                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.SupervisorCopyWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.SupervisorCopyWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

        End If

    End Sub

#Region "Groups TreeList"

    Private Sub CreateColumnsSupervisors()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn

        Dim VisibleIndex As Integer = 0

        Me.grdSupervisors.Columns.Clear()
        Me.grdSupervisors.KeyFieldName = "ID"
        Me.grdSupervisors.SettingsText.EmptyDataRow = " "

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "ID"
        GridColumn.FieldName = "ID"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.Visible = False
        GridColumn.Width = 40
        Me.grdSupervisors.Columns.Add(GridColumn)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ShowApplyFilterButton = False
        GridColumnCommand.ShowClearFilterButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.SelectAllCheckboxMode = GridViewSelectAllCheckBoxMode.None ' GridViewSelectAllCheckBoxMode.AllPages
        GridColumnCommand.ShowSelectCheckbox = True

        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = Unit.Percentage(5)
        VisibleIndex = VisibleIndex + 1
        Me.grdSupervisors.Columns.Add(GridColumnCommand)

        'Nombre
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridCauses.Column.Name", DefaultScope)
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = Unit.Percentage(95)
        Me.grdSupervisors.Columns.Add(GridColumn)

    End Sub

    Private Sub BindSupervisorsTreeList(ByVal bolReload As Boolean)
        Me.grdSupervisors.DataSource = Me.SupervisorsData(bolReload)
        Me.grdSupervisors.DataBind()
    End Sub

#End Region

End Class