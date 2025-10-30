Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class ChangeCommsStateWizard
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

        ''AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

        If Not Me.IsPostBack Then

            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text
            Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text

            Me.lblCurrentUser.Text = WLHelperWeb.CurrentPassport.Name

            Me.intActivePage = 0
        Else

            ' Miramos el estado de las comunicaciones. Parámetro CommsOffline de sysroParameters
            Dim oParameters = API.ConnectorServiceMethods.GetParameters(Me)
            Dim oParams As New roCollection(oParameters.ParametersXML)

            sOffline = roTypes.Any2String(oParams.Item(oParameters.ParametersNames(Parameters.CommsOffLine)))
            bCommsOffline = (sOffline = "1")

            If bCommsOffline Then
                ' Comunicaciones deshabilitadas. Si continuas, se activarán
                Dim Params As New Generic.List(Of String)
                Params = New Generic.List(Of String)
                Params.Add(sOffline)
                lblCommsConnectionActualStatus.Text = Me.Language.Translate("ChangeCommsStateWizard.CommsDisabled", Me.DefaultScope, Params)
                lblChangeConnectionStateDesc.Text = Me.Language.Translate("ChangeCommsStateWizard.CommsDisabled.Desc", Me.DefaultScope)
            Else
                ' Comunicaciones activas. Si continuas, se deshabilitarán
                lblCommsConnectionActualStatus.Text = Me.Language.Translate("ChangeCommsStateWizard.CommsEnabled", Me.DefaultScope)
                lblChangeConnectionStateDesc.Text = Me.Language.Translate("ChangeCommsStateWizard.CommsEnabled.Desc", Me.DefaultScope)
            End If

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
            If Me.divStep2.Style("display") <> "none" Then Me.intActivePage = 2
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

            Try
                Dim oParameters = API.ConnectorServiceMethods.GetParameters(Me)
                Dim oParams As New roCollection(oParameters.ParametersXML)

                oParams.Remove(oParameters.ParametersNames(Parameters.CommsOffLine))
                If bCommsOffline Then
                    ' Activamos (borramos fecha de paso a offline)
                    oParams.Add(oParameters.ParametersNames(Parameters.CommsOffLine), "0")
                Else
                    ' Desactivo comunicaciones. El cambio será efectivo en el próximo arranque de servidor
                    oParams.Add(oParameters.ParametersNames(Parameters.CommsOffLine), "1")
                End If

                oParameters.ParametersXML = oParams.XML

                bolSaved = API.ConnectorServiceMethods.SaveParameters(Me, oParameters, True)
            Catch ex As Exception
                bolSaved = False
            End Try

            Me.lblWelcome1.Text = Me.Language.Translate("End.ChangeCommsStateWizard.Text", Me.DefaultScope)
            If bolSaved Then

                Me.MustRefresh = "9"

                If bCommsOffline Then
                    Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.ChangeCommsStateWizard.Enabled.Text", Me.DefaultScope)
                Else
                    Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.ChangeCommsStateWizard.Disabled.Text", Me.DefaultScope)
                End If
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.ChangeCommsStateWizard.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(2, 0)

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1 ' Pantalla de explicación
                ' No requiere validación
            Case 2 ' Pantalla de solicitud de password
                If txtPassword.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.PasswordRequired", Me.DefaultScope)
                Else

                    Dim credential As String = API.UserAdminServiceMethods.GetUserAdmin(Me.Page, WLHelperWeb.CurrentPassport.ID).Login
                    Dim strDefaultDomain As String = roTypes.Any2String(HelperSession.AdvancedParametersCache("VTLive.AD.DefaultDomain"))

                    If strDefaultDomain.Trim <> String.Empty AndAlso credential.IndexOf("\") = -1 Then
                        credential = ".\" & credential
                    End If

                    ' Validamos el password proporcionado
                    Dim oPass As roPassportTicket = Nothing
                    oPass = API.SecurityServiceMethods.Authenticate(Me.Page, Nothing, AuthenticationMethod.Password, credential, txtPassword.Value, True, , , , , , , , False)
                    If oPass Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.Page1.WrongPassword", Me.DefaultScope)
                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg
        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 2
        End Select

        Select Case intActivePage
            Case 1

            Case 2
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

        If intOldPage = 2 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 2, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 2, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")

        End If

    End Sub

#End Region

End Class