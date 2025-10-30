Imports Robotics.Base.DTOs
Imports Robotics.Base.VTSelectorManager
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base

Partial Class Wizards_MultiEmployeeMobilityWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome '0
        frmEmployees '1
        frmGroup '2
        frmDates '3
    End Enum

    Private oActiveFrame As Frame

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("MultiEmployeeMobilityWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmEmployees)
                oFrames.Add(Frame.frmGroup)
                oFrames.Add(Frame.frmDates)

                ViewState("MultiEmployeeMobilityWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("MultiEmployeeMobilityWizard_Frames") = value
        End Set
    End Property

    Private ReadOnly Property FreezeDate() As Nullable(Of Date)
        Get

            Dim oDate As Nullable(Of Date)

            If ViewState("NewMultiEmployeeWizard_FreezeDate") = Nothing Then

                Dim oParameters As roParameters = API.ConnectorServiceMethods.GetParameters(Me)
                If oParameters IsNot Nothing Then
                    Dim oParams As New roCollection(oParameters.ParametersXML)
                    Dim auxDate As Object
                    Try
                        auxDate = oParams.Item(oParameters.ParametersNames(Parameters.FirstDate))
                    Catch ex As Exception
                        auxDate = Nothing
                    End Try
                    If auxDate IsNot Nothing AndAlso IsDate(auxDate) Then
                        ViewState("NewMultiEmployeeWizard_FreezeDate") = CType(auxDate, Date).ToShortDateString()
                    Else
                        ViewState("NewMultiEmployeeWizard_FreezeDate") = String.Empty
                    End If
                End If

                Dim strDate As String = ViewState("NewMultiEmployeeWizard_FreezeDate")
                Dim Fecha As Date
                If Date.TryParse(strDate, Fecha) Then
                    oDate = Fecha
                End If
            Else

                Dim Fecha As Date
                Dim strDate As String = ViewState("NewMultiEmployeeWizard_FreezeDate")
                If Date.TryParse(strDate, Fecha) Then
                    oDate = Fecha
                End If
            End If

            Return oDate

        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Employees", Permission.Admin) Then
                Me.Frames = Nothing

                Me.SetStepTitles()

                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizardGrid")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeMultiEmployeeMobilityGroupWizard")

                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

                Me.ifGroupSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifGroupSelector.Disabled = True

                Me.txtMoveDate.Date = Date.Now.Date
                Me.optNow.Checked = True

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

    End Sub

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

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckFrame(Me.oActiveFrame) Then

            Dim bolRet As Boolean = True

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim strErrorInfo As String = ""

            Dim selectedDate As Date = Date.Now.Date
            If Me.optFuture.Checked Then
                selectedDate = txtMoveDate.Date.Date
            End If

            Dim lstErrorEmps As String = String.Empty
            Dim lstEmployees As Generic.List(Of Integer) = Nothing
            Dim lstGroups As Generic.List(Of Integer) = Nothing

            'obtener todos los empleados de los grupos seleccionados en el arbol v3
            roSelectorManager.ExtractIdsFromSelectionString(Me.hdnEmployeesSelected.Value, lstEmployees, lstGroups)
            If lstGroups IsNot Nothing Then
                Dim strFilter As String = Me.hdnFilter.Value
                Dim strFilterUser As String = Me.hdnFilterUser.Value
                Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me, lstGroups.ToArray, "Employees", "U", strFilter, strFilterUser)
                lstEmployees.AddRange(tmp)
            End If

            Dim lstAuditParameterNames As New List(Of String)
            Dim lstAuditParameterValues As New List(Of String)
            lstAuditParameterNames.Add("{Employees}")
            lstAuditParameterValues.Add(lstEmployees.Count.ToString)
            lstAuditParameterNames.Add("{Group}")
            lstAuditParameterValues.Add(API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, Convert.ToInt32(Me.hdnIDGroupSelected.Value.Substring(1)), False).Name)

            API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aUpdate, Robotics.VTBase.Audit.ObjectType.tMassMobilityEmployees, "Movilidad Masiva", lstAuditParameterNames, lstAuditParameterValues, Me.Page)

            For Each iIDEmployee As Integer In lstEmployees
                If Not API.EmployeeServiceMethods.UpdateEmployeeGroup(Me, iIDEmployee, roTypes.Any2Integer(Me.hdnIDGroupSelected.Value.Substring(1)), selectedDate, False, -1, LockedDayAction.ReplaceAll, LockedDayAction.ReplaceAll, ShiftPermissionAction.ContinueAll, Nothing, , False) Then
                    lstErrorEmps = API.EmployeeServiceMethods.GetEmployeeName(Me.Page, iIDEmployee) & ","
                End If
            Next

            If lstErrorEmps.Count > 0 Then
                Dim lstAuditParameterNamesEnd As New List(Of String)
                Dim lstAuditParameterValuesEnd As New List(Of String)
                lstAuditParameterNamesEnd.Add("{Employees}")
                lstAuditParameterValuesEnd.Add(lstErrorEmps.Count.ToString)
                lstAuditParameterNamesEnd.Add("{Group}")
                lstAuditParameterValuesEnd.Add(API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, Convert.ToInt32(Me.hdnIDGroupSelected.Value.Substring(1)), False).Name)

                API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aNone, Robotics.VTBase.Audit.ObjectType.tMassMobilityEmployees, "Movilidad Masiva", lstAuditParameterNamesEnd, lstAuditParameterValuesEnd, Me.Page)
            End If

            If lstErrorEmps <> String.Empty Then 'Aqui va el save de las movilidades
                strErrorInfo = Me.Language.Translate("End.MultiEmployeeMobilityWizard.ErrorDetail", Me.DefaultScope) & ":" & lstErrorEmps
            End If

            Me.lblWelcome1.Text = Me.Language.Translate("End.MultiEmployeeMobilityWizard.Text", Me.DefaultScope)
            If strErrorInfo = "" Then
                Me.MustRefresh = "1"

                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.MultiEmployeeMobilityWizard.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.MultiEmployeeMobilityWizard.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

        End If

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
            'Case Wizards_NewMultiEmployeeWizard.Frame.frmGroup

            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmEmployees
                If Me.hdnEmployeesSelected.Value.Trim = "" Then
                    strMsg = Me.Language.Translate("CheckPage.NoEmployeeSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmGroup

                If Me.hdnIDGroupSelected.Value.Trim = "" Then
                    strMsg = Me.Language.Translate("CheckPage.NoGroupSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmDates
                Dim selectedDate As Date = Date.Now.Date
                If Me.optFuture.Checked Then
                    selectedDate = txtMoveDate.Date.Date
                End If

                'If selectedDate <= FreezeDate Then
                ' strMsg = Me.Language.Translate("CheckPage.FreezeDate", Me.DefaultScope)
                ' End If

                If selectedDate < DateTime.Now.Date Then
                    strMsg = Me.Language.Translate("CheckPage.MustBeFuture", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            'Case Wizards_NewMultiEmployeeWizard.Frame.frmGroup

            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmEmployees
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True
            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmGroup
                Me.ifGroupSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifGroupSelector.Disabled = True
            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmDates

        End Select

        Select Case oActiveFrame
            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmEmployees
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeMultiEmployeeMobilityEmployeesWizard&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeMultiEmployeeMobilityEmployeesWizard&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)

                Me.ifEmployeesSelector.Disabled = False
            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmGroup
                Me.ifGroupSelector.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=100&TreesMultiSelect=000&TreesOnlyGroups=100&TreeFunction=parent.GroupSelected&FilterFloat=false&" &
                                                                                                                               "PrefixTree=treeMultiEmployeeMobilityGroupWizard&PrefixCookie=objContainerTreeV3_treeMultiEmployeeMobilityGroupWizard&" &
                                                                                                                               "FeatureAlias=Employees")
                Me.ifGroupSelector.Disabled = False
            Case Wizards_MultiEmployeeMobilityWizard.Frame.frmDates

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

        Dim oLabel As Label
        Dim strStep As String = ""
        For n As Integer = 1 To System.Enum.GetValues(GetType(Frame)).Length - 1
            If n > 1 Then
                strStep = Me.hdnStepTitle2.Text.Replace("{0}", Me.FramePos(Me.FrameByIndex(n)))
                strStep = strStep.Replace("{1}", Me.Frames.Count - 1)
            End If
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Title")
            oLabel.Text = Me.hdnStepTitle.Text & strStep
        Next

    End Sub

#End Region

End Class