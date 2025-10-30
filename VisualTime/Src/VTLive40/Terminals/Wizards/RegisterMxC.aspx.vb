Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Forms_RegisterMxC
    Inherits PageBase

#Region "Declarations"

    Private intActivePage As Integer
    Private intIDTerminal As Integer

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Terminals.Definition", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

        Me.intIDTerminal = -1

        If Request("IDTerminal") IsNot Nothing Then Me.intIDTerminal = roTypes.Any2Integer(Request("IDTerminal"))

        If Not Me.IsPostBack Then

            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text

            Me.intActivePage = 0
        Else

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1

        End If
    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        Dim intOldPage As Integer
        If Me.CheckPage(Me.intActivePage) Then

            intOldPage = Me.intActivePage
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

            Dim bolSaved As Boolean = False
            Dim strErrorInfo As String = ""

            bolSaved = TerminalServiceMethods.RegisterMxCTerminal(Me.Page, Me.intIDTerminal, txtRegister.Text)

            Me.lblWelcome1.Text = Me.Language.Translate("End.NewTerminalWelcome1.Text", Me.DefaultScope)
            If bolSaved Then
                Me.MustRefresh = "9"
                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.NewTerminalWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
                Me.btClose.Text = Me.Language.Keyword("Button.Close")
                Me.PageChange(1, 0, True)
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.NewTerminalWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = roWsUserManagement.SessionObject.States.TerminalState.ErrorText
                Me.lblWelcome3.ForeColor = Drawing.Color.Red

                Me.btClose.Text = Me.Language.Keyword("Button.Close")
                Me.PageChange(1, 0)
            End If

        End If

    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        If strButtonKey = "MaxEmployeesAcceptKey" Then

            Me.lblWelcome1.Text = Me.Language.Translate("End.NewEmployeeWelcome1.Text", Me.DefaultScope)
            Me.lblWelcome2.Text = Me.Language.Translate("MaximumEmployeeReached.Message", Me.DefaultScope)
            Me.lblWelcome2.ForeColor = Drawing.Color.Red
            Me.lblWelcome3.Text = ""
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(4, 0)

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1 ' Pantalla Num. de Serie

                Dim oTerminal As roTerminal = TerminalServiceMethods.GetTerminal(Me.Page, Me.intIDTerminal, False)

                Dim isSerialCorrect As Boolean = False

                If oTerminal IsNot Nothing Then
                    isSerialCorrect = TerminalServiceMethods.CheckTerminalSerialNum(Me.Page, txtRegister.Text, oTerminal.SerialNumber)
                End If

                If isSerialCorrect = False Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.InvalidSerialNum", Me.DefaultScope)
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer, Optional ByVal bFinish As Boolean = False)

        Select Case intOldPage
            Case 2
        End Select

        Select Case intActivePage
            Case 1
                txtRegister.Focus()
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
            Me.btEnd.Visible = False '.Style("display") = "none"
            If Not bFinish Then
                Me.btNext.Visible = True '.Style("display") = "none"
            Else
                Me.btNext.Visible = False '.Style("display") = "none"
            End If
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

#End Region

End Class