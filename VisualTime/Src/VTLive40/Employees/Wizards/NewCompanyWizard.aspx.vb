Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Web.Base

Partial Class Wizards_NewCompanyWizard
    Inherits PageBase

    Private Enum Frame
        frmWelcome
        frmName
        frmEnd
    End Enum

#Region "Declarations"

    Private oActiveFrame As Frame

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("NewCompanyWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmName)
                oFrames.Add(Frame.frmEnd)

                ViewState("NewCompanyWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("NewCompanyWizard_Frames") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.Groups", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Not Me.IsPostBack Then

            If API.LicenseServiceMethods.FeatureIsInstalled("Feature\MultiCompany") Then

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

            Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, -1, False)
            With oGroup
                .ID = -1
                .Name = Me.txtCompanyName.Value
                .Path = ""
                .SecurityFlags = ""
            End With

            Dim strErrorInfo As String = ""

            Me.lblWelcome1.Text = Me.Language.Translate("End.NewCompanyWelcome1.Text", Me.DefaultScope)
            Dim intNewGroupID As Integer = API.EmployeeGroupsServiceMethods.SaveGroup(Me, oGroup, True)

            If intNewGroupID = -1 Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeGroupState.ErrorText
            End If

            If strErrorInfo = "" Then

                Me.MustRefresh = "1"

                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.NewCompanyWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.NewCompanyWelcome2.Text", Me.DefaultScope)
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
            Case Wizards_NewCompanyWizard.Frame.frmName
                If Me.txtCompanyName.Text.Length = 0 Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.InvalidCompanyName", Me.DefaultScope)
                ElseIf Me.CompanyNameExists(Me.txtCompanyName.Value) Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.ExistCompanyName", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame

        End Select

        Select Case oActiveFrame
            Case Frame.frmName
                Me.txtCompanyName.Focus()

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

    Private Function CompanyNameExists(ByVal strName As String) As Boolean

        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetCompanyByName(Me, strName)
        Return (oGroup IsNot Nothing)

    End Function

#End Region

End Class