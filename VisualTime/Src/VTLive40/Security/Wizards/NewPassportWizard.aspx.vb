Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.UsersAdmin
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Wizards_NewPassportWizard
    Inherits PageBase

    Private Enum Frame
        frmWelcome
        frmType
        frmGeneralData
        frmExpiration
        frmLocation
        frmPermissions
        frmIdentifyMethods
        frmEmployeeSelector
        frmCredential
    End Enum

#Region "Declarations"

    Private oActiveFrame As Frame

    Private oEmployeeSelected As roEmployee = Nothing

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("NewPassportWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmType)
                oFrames.Add(Frame.frmGeneralData)

                ViewState("NewPassportWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("NewPassportWizard_Frames") = value
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

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Administration.Security", Permission.Admin) Then

                Me.Frames = Nothing
                txtStartDate.Value = Nothing
                txtExpirationDate.Value = Nothing
                Me.SetStepTitles()

                Me.oActiveFrame = Frame.frmWelcome
            Else

                WLHelperWeb.RedirectAccessDenied(True)

            End If
        Else
            Me.oActiveFrame = SelectActiveFrame()
        End If

    End Sub

    Private Function SelectActiveFrame() As Frame
        Dim oDiv As HtmlControl
        For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
            oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
            If oDiv.Style("display") <> "none" Then Return Me.FrameByIndex(n)
        Next

        Return Frame.frmWelcome
    End Function

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
        Dim idPassportDelete As Integer = -1
        Dim newPassportId As Integer = 1

        If Me.CheckFrame(Me.oActiveFrame) Then
            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim oNewPassport As roPassport = Nothing
            Dim oParentPassport As roPassport = Nothing

            Dim strErrorInfo As String = ""

            If Me.optUserPassport.Checked Then
                oNewPassport = CreateNewPassportForUser()
            ElseIf Me.optEmployeePassport.Checked Then
                oNewPassport = SetupEmployeePassportAsSupervisor()
            End If

            If oNewPassport Is Nothing Then
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.NewPassportWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = "Impossible is nothing"
                Me.lblWelcome3.ForeColor = Drawing.Color.Red

                Return
            End If

            If strErrorInfo = "" AndAlso oNewPassport.GroupType <> "U" Then Me.cnIdentifyMethods.Validate(strErrorInfo)

            If strErrorInfo = "" Then
                ' Cargamos información de los métodos de identificación
                Me.cnIdentifyMethods.LoadPassport(oNewPassport)

                ' Guardamos el passport
                If Not API.UserAdminServiceMethods.SavePassport(Me, oNewPassport, True, True) Then
                    strErrorInfo = API.UserAdminServiceMethods.SecurityLastErrorText
                Else
                    newPassportId = oNewPassport.ID
                    Me.LaunchBotActions(oNewPassport)
                End If
            End If

            SetResultControlsVisibility(strErrorInfo, newPassportId)
        End If

    End Sub

    Private Function CreateNewPassportForUser() As roPassport
        Me.cnIdentifyMethods.ModoWizardNew = Base_WebUserControls_IdentifyMethods.WizardMode.ModeNewSingle

        Dim oNewPassport As New roPassport
        oNewPassport.Name = Me.txtPassportName.Text
        oNewPassport.Description = Me.txtPassportDescription.Value
        oNewPassport.GroupType = ""

        If Me.txtStartDate.Value IsNot Nothing Then oNewPassport.StartDate = Me.txtStartDate.Date
        If Me.txtExpirationDate.Value IsNot Nothing Then oNewPassport.ExpirationDate = Me.txtExpirationDate.Date
        If Me.txtMail.Text <> "" Then oNewPassport.Email = Me.txtMail.Text

        Dim passwordRow As New roPassportAuthenticationMethodsRow
        If Me.txtCredential.Text <> "" Then
            oNewPassport.AuthenticationMethods.PasswordRow = passwordRow
            oNewPassport.AuthenticationMethods.PasswordRow.Method = AuthenticationMethod.Password
            oNewPassport.AuthenticationMethods.PasswordRow.Version = ""
            oNewPassport.AuthenticationMethods.PasswordRow.BiometricID = 0
            oNewPassport.AuthenticationMethods.PasswordRow.RowState = RowState.NewRow
            oNewPassport.AuthenticationMethods.PasswordRow.Enabled = True
            oNewPassport.AuthenticationMethods.PasswordRow.Credential = Me.txtCredential.Text
            oNewPassport.AuthenticationMethods.PasswordRow.Password = ""
        End If

        oNewPassport.State = 1
        oNewPassport.IsSupervisor = True
        oNewPassport.Language = WLHelperWeb.CurrentPassport.Language


        Return oNewPassport
    End Function

    Private Function SetupEmployeePassportAsSupervisor() As roPassport
        Me.cnIdentifyMethods.ModoWizardNew = Base_WebUserControls_IdentifyMethods.WizardMode.ModeNormal

        Dim oEmployeePassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me, Me.hdnEmployeeSelected.Value.Substring(1), LoadType.Employee)

        If oEmployeePassport IsNot Nothing Then
            Me.cnIdentifyMethods.LoadData(LoadType.Employee, oEmployeePassport.IDEmployee, oEmployeePassport)
            Me.cnIdentifyMethods.SetHiddenEmployeeData()
            oEmployeePassport.State = 1
            oEmployeePassport.IsSupervisor = True
        End If

        Return oEmployeePassport
    End Function

    Private Sub SetResultControlsVisibility(strErrorInfo As String, newPassportId As Integer)
        Me.lblWelcome1.Text = Me.Language.Translate("End.NewPassportWelcome1.Text", Me.DefaultScope)
        If strErrorInfo = "" Then
            Me.MustRefresh = $"{newPassportId}"

            Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.NewPassportWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = ""
        Else
            Me.lblWelcome2.Text = Me.Language.Translate("End.Error.NewPassportWelcome2.Text", Me.DefaultScope)
            Me.lblWelcome3.Text = strErrorInfo
            Me.lblWelcome3.ForeColor = Drawing.Color.Red
        End If
        Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

    End Sub

    Private Sub LaunchBotActions(oNewPassport As roPassport)
        ' Ejecutamos la regla de bot del tipo copiar permisos y funcionalidades en caso que este activa
        Dim oLicense As New roServerLicense
        Dim bolIsInstalledBots As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")
        If bolIsInstalledBots Then
            Try
                Dim oBotState As New roBotState(-1)
                Dim oBotManager As New roBotManager(oBotState)
                Dim _oParameters As New Dictionary(Of BotRuleParameterEnum, String)
                _oParameters.Add(BotRuleParameterEnum.DestinationSupervisor, oNewPassport.ID.ToString)
                oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopySupervisorPermissions, _oParameters)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "Creating new passport " & oNewPassport.Name & ": Error executing bot", ex)
            End Try
        End If
    End Sub

#End Region

#Region "Methods"

    Private Function FrameIndex(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = CInt(oFrame)
        Return intRet
    End Function

    Private Function FramePos(ByVal oFrame As Frame) As Integer
        For n As Integer = 0 To Me.Frames.Count - 1
            If Me.Frames(n) = oFrame Then
                Return n
            End If
        Next
        Return 0
    End Function

    Private Function FrameByIndex(ByVal intIndex As Integer) As Frame
        Dim oRet As Frame = intIndex
        Return oRet
    End Function

    Private Function CheckFrame(ByVal oFrame As Frame) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case oFrame
            Case Wizards_NewPassportWizard.Frame.frmType
                ValidatePassportType(bolRet, strMsg)
            Case Wizards_NewPassportWizard.Frame.frmGeneralData
                ValidateGeneralData(bolRet, strMsg)
            Case Wizards_NewPassportWizard.Frame.frmExpiration
                ValidateExpirationPeriod(bolRet, strMsg)
            Case Wizards_NewPassportWizard.Frame.frmPermissions
                If strMsg <> "" Then bolRet = False
                Me.lblStep5Error.Text = strMsg
            Case Wizards_NewPassportWizard.Frame.frmIdentifyMethods
                If strMsg <> "" Then bolRet = False
                Me.lblStep6Error.Text = strMsg
            Case Wizards_NewPassportWizard.Frame.frmEmployeeSelector
                ValidateEmployeeSelected(bolRet, strMsg)
            Case Wizards_NewPassportWizard.Frame.frmCredential
                ValidateCredentialExists(bolRet, strMsg)
        End Select

        Return bolRet

    End Function

    Private Sub ValidateCredentialExists(ByRef bolRet As Boolean, ByRef strMsg As String)
        If Me.txtCredential.Text <> "" AndAlso UserAdminServiceMethods.CredentialExists(Me.Page, Me.txtCredential.Text.Trim, AuthenticationMethod.Password, "", -1) Then
            strMsg = Me.Language.Translate("Validate.NotUniqueCredential", Me.DefaultScope)
            bolRet = False
        End If
        If strMsg <> "" Then bolRet = False
        Me.lblStep8Error.Text = strMsg
    End Sub

    Private Sub ValidateEmployeeSelected(ByRef bolRet As Boolean, ByRef strMsg As String)
        ' Hemos de tener algún empleado seleccionado
        If Me.hdnEmployeeSelected.Value = "" OrElse Me.hdnEmployeeSelected.Value.StartsWith("A") Then
            strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeeSelected", Me.DefaultScope)
        Else
            Me.oEmployeeSelected = API.EmployeeServiceMethods.GetEmployee(Me, Me.hdnEmployeeSelected.Value.Substring(1), False)
            If Me.oEmployeeSelected Is Nothing Then
                strMsg = Me.Language.Translate("CheckPage.EmployeeSelectedNotExist", Me.DefaultScope)
            Else
                Dim oEmpPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me, Me.hdnEmployeeSelected.Value.Substring(1), LoadType.Employee)
                If oEmpPassport Is Nothing OrElse oEmpPassport.IsSupervisor Then
                    strMsg = Me.Language.Translate("CheckPage.EmployeeAlreadyASupervisor", Me.DefaultScope)
                End If
            End If
            End If
        If strMsg <> "" Then bolRet = False
        Me.lblStep7Error.Text = strMsg
    End Sub

    Private Sub ValidateExpirationPeriod(ByRef bolRet As Boolean, ByRef strMsg As String)
        If (Me.txtStartDate.Value IsNot Nothing AndAlso Me.txtExpirationDate.Value IsNot Nothing) AndAlso Me.txtStartDate.Date > Me.txtExpirationDate.Date Then
            strMsg = Me.Language.Translate("CheckPage.IncorrectExpirationPeriod", Me.DefaultScope)
        End If
        If strMsg <> "" Then bolRet = False
        Me.lblStep3Error.Text = strMsg
    End Sub

    Private Sub ValidateGeneralData(ByRef bolRet As Boolean, ByRef strMsg As String)
        If Me.txtPassportName.Text.Trim = "" Then
            strMsg = Me.Language.Translate("CheckPage.IncorrectPassportName", Me.DefaultScope)
        End If
        If strMsg <> "" Then bolRet = False
        Me.lblStep2Error.Text = strMsg
    End Sub

    Private Sub ValidatePassportType(ByRef bolRet As Boolean, ByRef strMsg As String)
        If Not Me.optUserPassport.Checked AndAlso Not Me.optEmployeePassport.Checked Then
            strMsg = Me.Language.Translate("CheckPage.InvalidType", Me.DefaultScope)
        End If
        If strMsg <> "" Then bolRet = False
        Me.lblStep1Error.Text = strMsg
    End Sub

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            Case Frame.frmType
                Dim oFrames As Generic.List(Of Frame) = Me.Frames

                oFrames.Clear()
                If Me.optUserPassport.Checked Then
                    oFrames.Add(Frame.frmWelcome)
                    oFrames.Add(Frame.frmType)
                    oFrames.Add(Frame.frmGeneralData)
                    oFrames.Add(Frame.frmExpiration)
                    oFrames.Add(Frame.frmCredential)
                    Me.oActiveFrame = Frame.frmGeneralData
                Else
                    oFrames.Add(Frame.frmWelcome)
                    oFrames.Add(Frame.frmType)
                    oFrames.Add(Frame.frmEmployeeSelector)
                    oFrames.Add(Frame.frmGeneralData)

                    Me.oActiveFrame = Frame.frmEmployeeSelector
                End If

                oActiveFrame = Me.oActiveFrame

                Me.Frames = oFrames

                Me.SetStepTitles()

            Case Frame.frmEmployeeSelector
                Me.SetStepTitles()

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True

                ' Obtenemos el nombre del empleado
                If Me.oEmployeeSelected Is Nothing Then _
                    Me.oEmployeeSelected = API.EmployeeServiceMethods.GetEmployee(Me, Me.hdnEmployeeSelected.Value.Substring(1), False)
                If Me.oEmployeeSelected IsNot Nothing Then
                    Me.txtPassportName.Text = Me.oEmployeeSelected.Name
                    Me.txtPassportDescription.Value = ""
                End If

        End Select

        Select Case oActiveFrame

            Case Frame.frmEmployeeSelector

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=111&TreesMultiSelect=000&TreesOnlyGroups=000&TreeFunction=parent.EmployeeSelected&FilterFloat=false&" &
                                                                                                                               "PrefixTree=treeEmployeesNewPassportWizard")
                Me.ifEmployeeSelector.Disabled = False

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

        If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 AndAlso Me.FramePos(oActiveFrame) = 0 Then
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