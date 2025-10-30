Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Web.Base

Partial Class ChangeCommsPassword
    Inherits PageBase

#Region "Declarations"

    Private intActivePage As Integer
    Private bCommsOffline As Boolean
    Private sOffline As String

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
        Dim bRet As Boolean = False
        If Me.CheckPage(Me.intActivePage) Then

            Dim bolSaved As Boolean = False
            Dim strErrorInfo As String = ""

            Try

                Dim oAdvancedParameter As roAdvancedParameter = Nothing
                oAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "PushTerminalsAdminPWD")
                If (oAdvancedParameter IsNot Nothing) Then
                    oAdvancedParameter.Value = Me.txtPassword.Value
                    bolSaved = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameter, True)
                Else
                    bolSaved = False
                End If

                Dim oAdvancedParameterZK As roAdvancedParameter = Nothing
                oAdvancedParameterZK = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "CommsZKPull.PasswordAdmin ")
                If (oAdvancedParameterZK IsNot Nothing) Then
                    oAdvancedParameterZK.Value = Me.txtPassword.Value
                    bolSaved = API.CommonServiceMethods.SaveAdvancedParameter(Me.Page, oAdvancedParameterZK, True)
                Else
                    bolSaved = False
                End If
            Catch ex As Exception
                bolSaved = False
            End Try

            Me.lblWelcome1.Text = Me.Language.Translate("End.ChangeCommsPassword.Text", Me.DefaultScope)
            If bolSaved Then

                Me.MustRefresh = "9"

                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.ChangeCommsPassword.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
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
                If txtPassword.Value.Length <> 4 Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.WrongPassword", Me.DefaultScope)
                Else
                    If Not Regex.IsMatch(txtPassword.Value, "^[0-9]+$") Then
                        strMsg = Me.Language.Translate("CheckPage.Page1.PasswordRequired", Me.DefaultScope)
                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 2
        End Select

        Select Case intActivePage
            Case 1
                txtPassword.Focus()
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

End Class