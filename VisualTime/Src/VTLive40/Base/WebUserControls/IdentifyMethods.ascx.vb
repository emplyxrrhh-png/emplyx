Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_WebUserControls_IdentifyMethods
    Inherits UserControlBase

#Region "Declarations"

    Public Enum WizardMode
        ModeNewSingle
        ModeNewMulti
        ModeNormal
    End Enum

    Private oModoWizardNew As WizardMode = WizardMode.ModeNormal

    Private bolResetPwd As Boolean = False

    Private _exceptionCode As Integer

#End Region

#Region "Properties"

    Public ReadOnly Property Exceptioncode() As Integer
        Get
            Return _exceptionCode
        End Get
    End Property

    Public Property Type() As LoadType
        Get
            Return Me.hdnType.Value
        End Get
        Set(ByVal value As LoadType)
            Me.hdnType.Value = value
        End Set
    End Property

    Public Property _ID() As Integer
        Get
            Return Me.hdnID.Value
        End Get
        Set(ByVal value As Integer)
            Me.hdnID.Value = value
        End Set
    End Property

    Public Property ModoWizardNew() As WizardMode
        Get
            Return Me.oModoWizardNew
        End Get
        Set(ByVal value As WizardMode)
            Me.oModoWizardNew = value
        End Set
    End Property

    Public Property chkUserName_() As Boolean
        Get
            Return Me.chkUsername.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.chkUsername.Checked = value
        End Set
    End Property

    Public WriteOnly Property txtUserName_() As String
        Set(ByVal value As String)
            Me.txtUserName.Value = value
        End Set
    End Property

    Public WriteOnly Property txtPassword_() As String
        Set(ByVal value As String)
            Me.txtPassword.Value = value
        End Set
    End Property

    Public Property chkCard_() As Boolean
        Get
            Return Me.chkCard.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.chkCard.Checked = value
        End Set
    End Property

    Public WriteOnly Property txtCard_() As String
        Set(ByVal value As String)
            Me.txtCardMX.Value = value
        End Set
    End Property

    Public Property chkNFC_() As Boolean
        Get
            Return Me.chkNFC.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.chkNFC.Checked = value
        End Set
    End Property

    Public WriteOnly Property txtNFC_() As String
        Set(ByVal value As String)
            Me.txtNFC.Value = value
        End Set
    End Property

    Public Property chkBiometric_() As Boolean
        Get
            Return Me.chkBiometric.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.chkBiometric.Checked = value
        End Set
    End Property

    Public Property chkPin_() As Boolean
        Get
            Return Me.chkPin.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.chkPin.Checked = value
        End Set
    End Property

    Public Property chkPlate_() As Boolean
        Get
            Return Me.chkPlate.Checked
        End Get
        Set(ByVal value As Boolean)
            Me.chkPlate.Checked = value
        End Set
    End Property

    Public WriteOnly Property txtPin_() As String
        Set(ByVal value As String)
            Me.txtPin.Value = value
        End Set
    End Property

    Public WriteOnly Property txtPlate1_() As String
        Set(ByVal value As String)
            Me.txtPlate1.Value = value
        End Set
    End Property

    Public WriteOnly Property txtPlate2_() As String
        Set(ByVal value As String)
            Me.txtPlate2.Value = value
        End Set
    End Property

    Public WriteOnly Property txtPlate3_() As String
        Set(ByVal value As String)
            Me.txtPlate3.Value = value
        End Set
    End Property

    Public WriteOnly Property txtPlate4_() As String
        Set(ByVal value As String)
            Me.txtPlate4.Value = value
        End Set
    End Property

    Public ReadOnly Property chkUsernameClientID() As String
        Get
            Return Me.chkUsername.ClientID
        End Get
    End Property

    Public ReadOnly Property chkCardClientID() As String
        Get
            Return Me.chkCard.ClientID
        End Get
    End Property

    Public ReadOnly Property chkNFCClientID() As String
        Get
            Return Me.chkNFC.ClientID
        End Get
    End Property

    Public ReadOnly Property chkBiometricClientID() As String
        Get
            Return Me.chkBiometric.ClientID
        End Get
    End Property

    Public ReadOnly Property chkPinClientID() As String
        Get
            Return Me.chkPin.ClientID
        End Get
    End Property

    Public ReadOnly Property chkPlateClientID() As String
        Get
            Return Me.chkPlate.ClientID
        End Get
    End Property

    Public ReadOnly Property isPasswordResetted() As Boolean
        Get
            Return Me.bolResetPwd
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'Me.LoadFunctionalityCombo()

        Me.SetControlsVisible()

        'If Not Me.IsPostBack Then
        'Me.SetFunctionalityCombo()
        'End If

        Me.RegisterScripts()

        Me.chkCard.ClientScript = "SetFunctionality('" & Me.ClientID & "');"
        Me.chkBiometric.ClientScript = "SetFunctionality('" & Me.ClientID & "');"
        Me.chkPin.ClientScript = "SetFunctionality('" & Me.ClientID & "');"
        Me.chkNFC.ClientScript = "SetFunctionality('" & Me.ClientID & "');"

        'Si no esta habilitado el cegidID, deshabilito el control
        Dim ssoType As String = roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "SSOTYPE").Value)
        If ssoType.Trim.ToUpper() <> "CEGIDID" Then Me.ckCegidID.Enabled = False

        'Se requiere licencia para que el metodo de identificacion este activo
        Me.chkPlate.Enabled = HelperSession.GetFeatureIsInstalledFromApplication("Feature\TerminalConnector")
        If Me.chkPlate.Enabled Then
            Me.chkPlate.ClientScript = "SetFunctionality('" & Me.ClientID & "');"
        End If

        Me.chkUsername.ClientScript = "SetFunctionality('" & Me.ClientID & "');"
        Me.txtUserName.Attributes("CConchange") = "IdentifyUserNameOnChange('" & Me.ClientID & "');"

        Me.divApplicationArea.Visible = False

        Dim oParameters As roParameters
        oParameters = API.ConnectorServiceMethods.GetParameters(Nothing)
        Dim oParams As New roCollection(oParameters.ParametersXML)
        If oParameters.ParametersNames.Contains(Parameters.DisableBiometricData.ToString()) AndAlso roTypes.Any2Boolean(oParams.Item(oParameters.ParametersNames(Parameters.DisableBiometricData))) Then
            chkBiometric.Enabled = False
        End If

        'If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
        'Me.divMethodsArea.Style("display") = "none"
        'End If

    End Sub

#End Region

#Region "Methods"

    Public Sub RegisterScripts()
        Dim script As ClientScriptManager = Me.Parent.Page.ClientScript

        Dim cacheManager As New NoCachePageBase
        cacheManager.InsertExtraJavascript("IdentifyMethods", "~/Base/Scripts/IdentifyMethods.js", Me.Parent.Page)

        Dim scriptString As String

        scriptString = "<script language=JavaScript> " & vbCrLf &
                           "function IdentifyMethodsLoad(){ " & vbCrLf &
                               "venableOPC('" & Me.chkUsername.ClientID & "'); " & vbCrLf &
                               "venableOPC('" & Me.chkCard.ClientID & "'); " & vbCrLf &
                               "venableOPC('" & Me.chkNFC.ClientID & "'); " & vbCrLf &
                               "venableOPC('" & Me.chkBiometric.ClientID & "'); " & vbCrLf &
                               "venableOPC('" & Me.chkPin.ClientID & "'); " & vbCrLf &
                               "venableOPC('" & Me.chkPlate.ClientID & "'); " & vbCrLf &
                               "SetFunctionality('" & Me.ClientID & "'); " & vbCrLf &
                               "var pwd; " & vbCrLf &
                               "pwd = document.getElementById('" & Me.txtPassword.ClientID & "'); " & vbCrLf &
                               "if (pwd != null) { pwd.value = RoboticsJsUtils.decryptString(pwd.getAttribute('valuePwd')); } " & vbCrLf &
                               "pwd = document.getElementById('" & Me.txtPin.ClientID & "'); " & vbCrLf &
                               "if (pwd != null) { pwd.value = RoboticsJsUtils.decryptString(pwd.getAttribute('valuePwd')); } " & vbCrLf &
                            "} " & vbCrLf &
                        "</script>"

        If Not script.IsClientScriptBlockRegistered(GetType(String), "IdentifyMethods_Load") Then
            script.RegisterClientScriptBlock(GetType(String), "IdentifyMethods_Load", scriptString)
        End If

        scriptString = "<script language=JavaScript> " & vbCrLf &
                            "IdentifyMethodsLoad();" & vbCrLf &
                       "</script>"

        If (Not script.IsStartupScriptRegistered("IdentifyMethods_Startup")) Then
            script.RegisterStartupScript(GetType(String), "IdentifyMethods_Startup", scriptString)
        End If

    End Sub

    Public Sub SetHiddenEmployeeData()
        Try
            If Me.chkPin.Checked AndAlso Not String.IsNullOrEmpty(Me.txtPin.Attributes("valuePwd")) Then
                Me.txtPin.Value = NoCachePageBase.Decrypt(Me.txtPin.Attributes("valuePwd"))
            End If
        Catch ex As Exception
            Me.txtPin.Value = ""
        End Try

    End Sub

    Public Sub LoadData(ByVal PassportType As LoadType, ByVal intID As Integer, Optional ByRef _Passport As roPassport = Nothing)
        Me.divApplicationArea.Visible = True

        Me.Type = PassportType
        Me._ID = intID

        Me.SetControlsVisible()

        Me.chkUsername.Checked = False
        Me.chkCard.Checked = False
        Me.chkNFC.Checked = False
        Me.chkBiometric.Checked = False
        Me.chkPin.Checked = False
        Me.chkPlate.Checked = False

        Dim oPassport As roPassport = _Passport
        If oPassport Is Nothing Then
            oPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me._ID, Me.Type, True)
            _Passport = oPassport
        End If

        If roWsUserManagement.SessionObject.States.UserAdminSecurityState.Result <> SecurityResultEnum.NoError Then
            _exceptionCode = roWsUserManagement.SessionObject.States.UserAdminSecurityState.Result
        End If

        If oPassport IsNot Nothing Then

            Dim Params As New Generic.List(Of String)
            Params.Add(roTypes.Any2Integer(oPassport.IDEmployee))
            Me.lblInformation.Text = Me.Language.Translate("Information.Message", Me.DefaultScope, Params)
            Me.lblPinDescription2.Text = Me.Language.Translate("Information.PIN.Message", Me.DefaultScope, Params)
            Me.lblInactiveUser.Text = Me.Language.Translate("InactiveUser.Message", Me.DefaultScope, Params)
            Me.lblBlockedByInactivity.Text = Me.Language.Translate("BlockedByInactivity.Message", Me.DefaultScope, Params)

            Dim oMethod As roPassportAuthenticationMethodsRow

            Me.trRecoveryKey.Visible = False

            If Me.chkUsername.Visible Then
                Me.trValidateByAD.Style("display") = "none"
                oMethod = oPassport.AuthenticationMethods.PasswordRow
                If oMethod IsNot Nothing Then
                    Me.chkUsername.Checked = oMethod.Enabled
                    Me.txtUserName.Value = oMethod.Credential.Trim
                    If oMethod.Credential.IndexOf("\") >= 0 Then 'Se autentifica mediante AD -> no se muestra el password
                        Me.divUsernameOptions.Visible = False
                        Me.txtPassword.Value = "-"
                        Me.txtPassword.Attributes.Add("valuePwd", NoCachePageBase.Encrypt("-"))
                        Me.trPassword.Style("display") = "none"
                        Me.trValidateByAD.Style("display") = ""
                    Else
                        Me.txtPassword.Value = oMethod.Password
                        Me.txtPassword.Attributes.Add("valuePwd", NoCachePageBase.Encrypt("xxxxxxxx"))
                        Me.trPassword.Style("display") = "none"
                    End If

                    If API.SecurityServiceMethods.GetPassportBelongsToAdminGroup(Me.Page, oPassport.ID) Then
                        Me.hdnMustActivateApplicationAccess.Value = "0"
                    Else
                        Me.hdnMustActivateApplicationAccess.Value = "1"
                    End If

                    Me.chkRestictApplicationAccess.Checked = oMethod.BloquedAccessApp
                Else
                    Me.divUsernameOptions.Visible = False
                    Me.txtUserName.Value = ""
                    Me.chkUsername.Checked = False
                    Me.txtPassword.Attributes.Add("valuePwd", NoCachePageBase.Encrypt("xxxxxxxx"))
                    Me.trPassword.Style("display") = "none"
                End If
            End If

            If Me.chkCard.Visible Then
                If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Length > 0 Then
                    oMethod = oPassport.AuthenticationMethods.CardRows(0)
                Else
                    oMethod = Nothing
                End If

                If oMethod IsNot Nothing Then
                    Me.chkCard.Checked = oMethod.Enabled
                    Me.txtCardMX.Value = oMethod.Credential
                Else
                    Me.chkCard.Checked = False
                    Me.txtCardMX.Value = String.Empty
                End If
            End If

            If Me.chkNFC.Visible Then
                If oPassport.AuthenticationMethods.NFCRow IsNot Nothing Then
                    oMethod = oPassport.AuthenticationMethods.NFCRow
                Else
                    oMethod = Nothing
                End If

                If oMethod IsNot Nothing Then
                    Me.chkNFC.Checked = oMethod.Enabled
                    Me.txtNFC.Value = oMethod.Credential
                Else
                    Me.chkNFC.Checked = False
                    Me.txtNFC.Value = String.Empty
                End If
            End If

            If Me.chkBiometric.Visible Then

                If oPassport.AuthenticationMethods.BiometricRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.BiometricRows.Length > 0 Then
                    If oPassport.AuthenticationMethods.BiometricRows.ToList.FindAll(Function(oPass) oPass.Enabled).Count > 0 Then
                        Me.chkBiometric.Checked = True
                        If oPassport.IDEmployee.HasValue Then
                            Me.txtIDBiometricSX.Value = oPassport.IDEmployee
                        End If
                    Else
                        Me.chkBiometric.Checked = False
                    End If
                Else
                    Me.chkBiometric.Checked = False
                End If
            End If

            If Me.chkPin.Visible Then
                oMethod = oPassport.AuthenticationMethods.PinRow
                If oMethod IsNot Nothing Then
                    Me.chkPin.Checked = oMethod.Enabled
                    'Me.txtPin.Value = oMethod.Password
                    Me.txtPin.Attributes.Add("valuePwd", NoCachePageBase.Encrypt(oMethod.Password))
                Else
                    Me.chkPin.Checked = False
                    Me.txtPin.Attributes.Add("valuePwd", NoCachePageBase.Encrypt(String.Empty))
                End If
                Dim timegateAdvancedParameter As roAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Nothing, "Timegate.Identification.CustomUserFieldId")
                If timegateAdvancedParameter IsNot Nothing Then
                    Dim customUserFieldObj As TimegateConfiguration = JsonConvert.DeserializeObject(Of TimegateConfiguration)(timegateAdvancedParameter.Value)
                    If customUserFieldObj IsNot Nothing AndAlso customUserFieldObj.CustomUserFieldEnabled AndAlso customUserFieldObj.UserFieldId > 0 AndAlso oPassport.IDEmployee IsNot Nothing Then
                        Dim userFieldValue As String = roEmployeeUserField.GetEmployeeUserFieldValueAtDateById(oPassport.IDEmployee, customUserFieldObj.UserFieldId, Date.Now, New Robotics.Base.VTUserFields.UserFields.roUserFieldState).FieldValue
                        If userFieldValue IsNot Nothing AndAlso userFieldValue <> "" AndAlso userFieldValue <> "0" Then
                            Me.lblPinDescription.Text = Me.Language.Translate("lblpindescription.text", Me.DefaultScope) & " " & userFieldValue
                        Else
                            Me.lblPinDescription.Text = Me.Language.Translate("EmptyPINId.text", Me.DefaultScope)
                        End If
                    Else
                        Me.lblPinDescription.Text = Me.Language.Translate("lblpindescription.text", Me.DefaultScope) & " " & roTypes.Any2Integer(oPassport.IDEmployee)
                    End If
                Else
                    Me.lblPinDescription.Text = Me.Language.Translate("lblpindescription.text", Me.DefaultScope) & " " & roTypes.Any2Integer(oPassport.IDEmployee)
                End If
            End If

            If Me.ckCegidID.Visible Then
                Dim ssoType As String = roTypes.Any2String(API.CommonServiceMethods.GetAdvancedParameter(Nothing, "SSOTYPE").Value)
                Me.ckCegidID.Checked = False

                If ssoType.Trim.ToUpper = "CEGIDID" Then
                    oMethod = oPassport.AuthenticationMethods.IntegratedSecurityRow
                    If oMethod IsNot Nothing Then
                        Me.ckCegidID.Checked = oMethod.Enabled

                        If oMethod.Credential.StartsWith("\temp_") Then
                            Me.imgCegidIdStatus.Src = "~/Base/Images/TasksIdle.png"
                            Me.lblCegidIdStatus.Text = "Inactivo"
                            Me.lblCegidIdRegisterDate.Text = ""
                            Me.btnResetCegidId.Style("display") = "none"
                        Else
                            Me.imgCegidIdStatus.Src = "~/Base/Images/TasksRun.png"
                            Me.lblCegidIdStatus.Text = "Activo"
                            Me.lblCegidIdRegisterDate.Text = $"{oMethod.LastUpdatePassword.Date.ToString(HelperWeb.GetShortDateFormat())}"
                            Me.btnResetCegidId.Style("display") = ""
                        End If


                    End If
                End If
            End If

            Me.txtPlate1.Value = String.Empty
            Me.txtPlate2.Value = String.Empty
            Me.txtPlate3.Value = String.Empty
            Me.txtPlate4.Value = String.Empty

            Me.chkPlate.Checked = False

            If Me.chkPlate.Visible AndAlso Me.chkPlate.Enabled = True Then

                Dim arrMethods As roPassportAuthenticationMethodsRow() = oPassport.AuthenticationMethods.PlateRows
                If arrMethods IsNot Nothing Then
                    'obtener registros
                    For Each rw As roPassportAuthenticationMethodsRow In arrMethods
                        Select Case rw.Version
                            Case 1
                                Me.txtPlate1.Value = rw.Credential
                                If Me.chkPlate.Checked = False Then
                                    Me.chkPlate.Checked = rw.Enabled
                                End If
                            Case 2
                                Me.txtPlate2.Value = rw.Credential
                                If Me.chkPlate.Checked = False Then
                                    Me.chkPlate.Checked = rw.Enabled
                                End If
                            Case 3
                                Me.txtPlate3.Value = rw.Credential
                                If Me.chkPlate.Checked = False Then
                                    Me.chkPlate.Checked = rw.Enabled
                                End If
                            Case 4
                                Me.txtPlate4.Value = rw.Credential
                                If Me.chkPlate.Checked = False Then
                                    Me.chkPlate.Checked = rw.Enabled
                                End If
                        End Select
                    Next
                End If
            End If

            Me.lblInactiveUser.Visible = False

            If oPassport.AuthenticationMethods.PasswordRow IsNot Nothing Then

                If oPassport.AuthenticationMethods.PasswordRow.Credential.Contains("\") Then

                    Dim tmpPassport As roPassport = UserAdminServiceMethods.GetPassport(Me.Page, oPassport.ID, LoadType.Passport)

                    If tmpPassport IsNot Nothing AndAlso tmpPassport.AuthenticationMethods.PasswordRow.LastAppActionDate = DateTime.MinValue Then
                        Me.lblInactiveUser.Visible = True
                    End If
                Else
                    If oPassport.AuthenticationMethods.PasswordRow.LastUpdatePassword = Nothing OrElse oPassport.AuthenticationMethods.PasswordRow.LastUpdatePassword = New Date(1900, 1, 1) Then
                        Me.lblInactiveUser.Visible = True
                    End If
                End If

            End If

            Me.lblBlockedByInactivity.Visible = False
            If oPassport.AuthenticationMethods.PasswordRow IsNot Nothing AndAlso oPassport.AuthenticationMethods.PasswordRow.BlockedAccessByInactivity Then
                Me.lblBlockedByInactivity.Visible = True
            End If

        End If

    End Sub

    ''' <summary>
    ''' Actualiza el passport con la información de los mètodos de autentificación actuales
    ''' </summary>
    ''' <param name="oPassport"></param>
    Public Sub LoadPassport(ByVal oPassport As roPassport)

        Me.divApplicationArea.Visible = True
        Me.lblInformation.Visible = False
        Me.divUsernameOptions.Visible = False
        Me.trRecoveryKey.Visible = False

        Me.trPassword.Visible = False
        Me.lblInformation.Visible = True
        Me.divUsernameOptions.Visible = True

        If oPassport.AuthenticationMethods Is Nothing Then
            oPassport.AuthenticationMethods = New roPassportAuthenticationMethods
        End If

        Dim oMethod As roPassportAuthenticationMethodsRow

        oMethod = oPassport.AuthenticationMethods.PasswordRow
        Dim oldCredential As String = ""
        If Me.chkUsername.Checked Then
            Dim bolAddMethod As Boolean = False

            If oMethod Is Nothing Then
                bolAddMethod = True
                oMethod = New roPassportAuthenticationMethodsRow
                With oMethod
                    .IDPassport = oPassport.ID
                    .Method = AuthenticationMethod.Password
                    .Version = ""
                    .BiometricID = 0
                    .RowState = RowState.UpdateRow
                End With
            Else
                oldCredential = oMethod.Credential
            End If

            Me.bolResetPwd = False
            oMethod.Credential = Me.txtUserName.Value.Trim
            If (bolAddMethod) Then
                Me.divUsernameOptions.Visible = False
                'Si es un usuario nuevo no es de active directory dejo el pwd en blanco para que se genere y envie
                If oMethod.Credential.IndexOf("\") = -1 Then
                    oMethod.Password = ""
                    Me.bolResetPwd = True
                Else 'Si es un usuario nuevo y es de active directory dejo el pwd a un string reservado
                    oMethod.Password = "-"
                End If
            Else
                If (oldCredential.IndexOf("\") = -1) Then
                    'Si el usuario no se autentificaba mediante AD
                    If oMethod.Credential.IndexOf("\") = -1 Then

                        'si el nuevo usuario no es de AD dejamos el pwd antiguo
                        Dim oldPassport As roPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me._ID, Me.Type, True)
                        oMethod.Password = oldPassport.AuthenticationMethods.PasswordRow.Password
                    Else
                        'Si el nuevo usuario se autentifica mediante active directory dejamos el pwd a un string reservado
                        Me.divUsernameOptions.Visible = False
                        oMethod.Password = "-"
                    End If
                Else
                    'Si el usuario antes se autentificaba mediante AD
                    If oMethod.Credential.IndexOf("\") = -1 Then
                        'Si no se autentifica mediante active directory le reseteamos el pwd, para que se le genere uno
                        oMethod.Password = ""
                        Me.bolResetPwd = True
                    Else
                        'Si el nuevo usuario se autentifica mediante active directory dejamos el pwd a un string reservado
                        Me.divUsernameOptions.Visible = False
                        oMethod.Password = "-"
                    End If
                End If
            End If

            oMethod.Enabled = True
            oMethod.RowState = RowState.UpdateRow

            If API.SecurityServiceMethods.GetPassportBelongsToAdminGroup(Me.Page, oPassport.ID) = False Then
                oMethod.BloquedAccessApp = Me.chkRestictApplicationAccess.Checked
            End If

            oPassport.AuthenticationMethods.PasswordRow = oMethod
        Else
            If oMethod IsNot Nothing Then
                If oMethod.RowState <> RowState.NewRow Then
                    oMethod.Enabled = False
                    If oMethod.Credential.IndexOf("\") = -1 Then
                        'si el nuevo usuario no es de AD dejamos el pwd antiguo
                        Dim oldPassport As roPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me._ID, Me.Type, True)
                        If oldPassport IsNot Nothing Then
                            oMethod.Password = oldPassport.AuthenticationMethods.PasswordRow.Password
                        End If
                    Else
                        'Si el nuevo usuario se autentifica mediante active directory dejamos el pwd a un string reservado
                        Me.divUsernameOptions.Visible = False
                        oMethod.Password = "-"
                    End If
                    oMethod.RowState = RowState.UpdateRow
                    oPassport.AuthenticationMethods.PasswordRow = oMethod
                End If
            End If
        End If

        oMethod = oPassport.AuthenticationMethods.IntegratedSecurityRow
        If Me.ckCegidID.Checked Then
            If oMethod Is Nothing Then
                oMethod = New roPassportAuthenticationMethodsRow
                With oMethod
                    .IDPassport = oPassport.ID
                    .Method = AuthenticationMethod.IntegratedSecurity
                    .Version = ""
                    .BiometricID = 0
                    .Password = ""
                    .Credential = $"\temp_{Guid.NewGuid.ToString("N").ToLower}"
                End With
            End If
            oMethod.Enabled = True
            oMethod.RowState = RowState.UpdateRow

            oPassport.AuthenticationMethods.IntegratedSecurityRow = oMethod
        Else
            If oMethod IsNot Nothing Then
                oMethod.Enabled = False
                oMethod.RowState = RowState.UpdateRow

                oPassport.AuthenticationMethods.IntegratedSecurityRow = oMethod
            End If
        End If


        If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Length > 0 Then
            oMethod = oPassport.AuthenticationMethods.CardRows(0)
        Else
            oMethod = Nothing
        End If

        If Me.chkCard.Checked Then
            If oMethod Is Nothing Then
                oMethod = New roPassportAuthenticationMethodsRow
                With oMethod
                    .IDPassport = oPassport.ID
                    .Method = AuthenticationMethod.Card
                    .Version = ""
                    .BiometricID = 0
                    .Password = ""
                End With
            End If
            oMethod.Credential = Me.txtCardMX.Value
            oMethod.Enabled = True
            oMethod.RowState = RowState.UpdateRow

            oPassport.AuthenticationMethods.CardRows = {oMethod}
        Else
            If oMethod IsNot Nothing Then
                oMethod.Enabled = False
                oMethod.Credential = Me.txtCardMX.Value.Trim
                If Me.txtCardMX.Value.Trim <> String.Empty Then
                    oMethod.RowState = RowState.UpdateRow
                Else
                    oMethod.RowState = RowState.DeleteRow
                End If

                oPassport.AuthenticationMethods.CardRows = {oMethod}
            End If
        End If


        If oPassport.AuthenticationMethods.NFCRow IsNot Nothing Then
            oMethod = oPassport.AuthenticationMethods.NFCRow
        Else
            oMethod = Nothing
        End If

        If Me.chkNFC.Checked Then
            If oMethod Is Nothing Then
                oMethod = New roPassportAuthenticationMethodsRow
                With oMethod
                    .IDPassport = oPassport.ID
                    .Method = AuthenticationMethod.NFC
                    .Version = ""
                    .BiometricID = 0
                    .Password = ""
                End With
            End If
            oMethod.Credential = Me.txtNFC.Value
            oMethod.Enabled = True
            oMethod.RowState = RowState.UpdateRow

            oPassport.AuthenticationMethods.NFCRow = oMethod
        Else
            If oMethod IsNot Nothing Then
                oMethod.Enabled = False
                oMethod.Credential = Me.txtNFC.Value.Trim
                If Me.txtNFC.Value.Trim <> String.Empty Then
                    oMethod.RowState = RowState.UpdateRow
                Else
                    oMethod.RowState = RowState.DeleteRow
                End If

                oPassport.AuthenticationMethods.NFCRow = oMethod
            End If
        End If




        Dim oBioMethods As roPassportAuthenticationMethodsRow() = oPassport.AuthenticationMethods.BiometricRows
        If oBioMethods Is Nothing Then
            oBioMethods = {}
        End If
        If Me.chkBiometric.Checked Then
            If oBioMethods.Length = 0 Then
                oMethod = New roPassportAuthenticationMethodsRow
                With oMethod
                    .IDPassport = oPassport.ID
                    .Method = AuthenticationMethod.Biometry
                    .Version = "RXA200"
                    .BiometricID = 0
                    .Credential = ""
                    .Password = ""
                    .BiometricData = {}
                    .RowState = RowState.UpdateRow
                End With

                Dim tmpList As Generic.List(Of roPassportAuthenticationMethodsRow) = oBioMethods.ToList
                tmpList.Add(oMethod)
                oBioMethods = tmpList.ToArray

            End If
            For Each oMethod In oBioMethods
                oMethod.Enabled = True
                oMethod.RowState = RowState.UpdateRow
            Next
        Else
            For Each oRow As roPassportAuthenticationMethodsRow In oBioMethods
                oRow.Enabled = False
                oRow.RowState = RowState.UpdateRow
            Next
        End If

        oPassport.AuthenticationMethods.BiometricRows = oBioMethods

        oMethod = oPassport.AuthenticationMethods.PinRow
        If Me.chkPin.Checked Then
            If oMethod Is Nothing Then
                oMethod = New roPassportAuthenticationMethodsRow
                With oMethod
                    .IDPassport = oPassport.ID
                    .Method = AuthenticationMethod.Pin
                    .Version = ""
                    '.BiometricID = 0
                    .Credential = ""
                End With
            End If
            oMethod.Password = Me.txtPin.Value
            oMethod.Enabled = True
            oMethod.RowState = RowState.UpdateRow

            oPassport.AuthenticationMethods.PinRow = oMethod
        Else
            If oMethod IsNot Nothing Then
                oMethod.Enabled = False ' oMethod.Delete()
                oMethod.RowState = RowState.UpdateRow
                oPassport.AuthenticationMethods.PinRow = oMethod
            End If
        End If

        'MATRICULAS
        If chkPlate.Enabled = True Then
            Dim arrMethods As roPassportAuthenticationMethodsRow() = oPassport.AuthenticationMethods.PlateRows
            GestionaPlateToTable(oPassport.ID, Me.txtPlate1.Value.Trim(), 1, arrMethods)
            GestionaPlateToTable(oPassport.ID, Me.txtPlate2.Value.Trim(), 2, arrMethods)
            GestionaPlateToTable(oPassport.ID, Me.txtPlate3.Value.Trim(), 3, arrMethods)
            GestionaPlateToTable(oPassport.ID, Me.txtPlate4.Value.Trim(), 4, arrMethods)

            oPassport.AuthenticationMethods.PlateRows = arrMethods
        End If

        'FIN MATRICULAS

        'If Me.cmbFunctionality_Value.Value <> "" Then
        '    ReDim oPassport.AuthenticationMerge(0)
        '    oPassport.AuthenticationMerge(0) = Me.cmbFunctionality_Value.Value
        'Else
        '    ReDim oPassport.AuthenticationMerge(-1)
        'End If

    End Sub

    'Private Shared Function Chivato(ByVal item As PassportAuthenticationMethodsDataSet.sysroPassports_AuthenticationMethodsRow) As Boolean
    '    If item.RowState = DataRowState.Deleted Or item.RowState = DataRowState.Detached Then Return False
    '    Dim sVersion As String = "1"
    '    Return item.Version = sVersion 'NumVersion
    'End Function

    Private Sub GestionaPlateToTable(ByVal ID As Integer, ByVal PlateValue As String, ByVal strVersion As String, ByRef arrMethods As roPassportAuthenticationMethodsRow())

        Dim oMethod As roPassportAuthenticationMethodsRow = Nothing
        If Not arrMethods Is Nothing Then
            oMethod = Array.Find(arrMethods, Function(m) m.Version = strVersion)
            'oMethod = Array.Find(arrMethods, AddressOf Chivato)
        End If
        If Not oMethod Is Nothing Then
            'Actualizar registro
            oMethod.Credential = PlateValue
            oMethod.Enabled = (PlateValue <> String.Empty) And Me.chkPlate.Checked
            oMethod.TimeStamp = Now
            oMethod.RowState = RowState.UpdateRow
        Else
            If PlateValue <> String.Empty Then
                'crear registro
                oMethod = New roPassportAuthenticationMethodsRow
                oMethod.IDPassport = ID
                oMethod.Password = ""
                oMethod.Method = AuthenticationMethod.Plate
                oMethod.Version = strVersion
                oMethod.BiometricID = 0
                oMethod.Credential = PlateValue
                oMethod.Enabled = True
                oMethod.TimeStamp = Now
                oMethod.RowState = RowState.UpdateRow

                Dim tmpList As Generic.List(Of roPassportAuthenticationMethodsRow) = New List(Of roPassportAuthenticationMethodsRow)
                If arrMethods IsNot Nothing Then tmpList.AddRange(arrMethods.ToList)
                tmpList.Add(oMethod)
                arrMethods = tmpList.ToArray
            End If
        End If
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="strMsg"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function Validate(ByRef strMsg As String) As Boolean

        Dim bolRet As Boolean = True

        Me.txtCardMX.Value = Me.txtCardMX.Value.Trim

        'If Not Me.bolMultiEmployee AndAlso _
        If Me.chkCard.Checked AndAlso (Me.txtCardMX.Value.Length = 0 Or Not IsNumeric(Me.txtCardMX.Value)) Then
            'Hemos de tener un numero de tarjeta
            strMsg = Me.Language.Translate("Validate.InvalidCardID", Me.DefaultScope)
            bolRet = False
        ElseIf Me.oModoWizardNew = WizardMode.ModeNewSingle AndAlso Me.chkCard.Checked AndAlso
               UserAdminServiceMethods.CredentialExists(Me.Page, Me.txtCardMX.Value, AuthenticationMethod.Card, "", Nothing) Then
            'El numero de tarjeta debe ser unico
            strMsg = Me.Language.Translate("Validate.NotUniqueCardID", Me.DefaultScope)
            bolRet = False
        End If


        Me.txtNFC.Value = Me.txtNFC.Value.Trim

        'If Not Me.bolMultiEmployee AndAlso _
        If Me.chkNFC.Checked AndAlso Me.txtNFC.Value.Length = 0 Then
            'Hemos de tener un numero de tarjeta
            strMsg = Me.Language.Translate("Validate.InvalidNFCID", Me.DefaultScope)
            bolRet = False
        ElseIf Me.oModoWizardNew = WizardMode.ModeNewSingle AndAlso Me.chkNFC.Checked AndAlso
               UserAdminServiceMethods.CredentialExists(Me.Page, Me.txtNFC.Value, AuthenticationMethod.NFC, "", Nothing) Then
            'El numero de tarjeta debe ser unico
            strMsg = Me.Language.Translate("Validate.NotUniqueNFCID", Me.DefaultScope)
            bolRet = False
        End If


        If Me.chkPlate.Enabled = True Then
            'Que no se repita ninguna para el mismo empleado

            txtPlate1.Value = txtPlate1.Value.Trim().Replace("'", "")
            txtPlate2.Value = txtPlate2.Value.Trim().Replace("'", "")
            txtPlate3.Value = txtPlate3.Value.Trim().Replace("'", "")
            txtPlate4.Value = txtPlate4.Value.Trim().Replace("'", "")

            Dim PlatesList As New Generic.List(Of String)()
            If txtPlate1.Value <> String.Empty Then PlatesList.Add(txtPlate1.Value)
            If txtPlate2.Value <> String.Empty Then PlatesList.Add(txtPlate2.Value)
            If txtPlate3.Value <> String.Empty Then PlatesList.Add(txtPlate3.Value)
            If txtPlate4.Value <> String.Empty Then PlatesList.Add(txtPlate4.Value)

            Dim PlatesDuplicate = PlatesList.GroupBy(Function(s) s).SelectMany(Function(grp) grp.Skip(1))
            If PlatesDuplicate.Count > 0 Then
                strMsg = Me.Language.Translate("Validate.DuplicatePlates", Me.DefaultScope)
                bolRet = False
            Else
                'Si tiene matriculas activadas, que tenga alguna informada.
                If Me.chkPlate.Checked Then
                    Dim strPlates = Me.txtPlate1.Value.Trim() & Me.txtPlate2.Value.Trim() & Me.txtPlate3.Value.Trim() & Me.txtPlate4.Value.Trim()
                    If strPlates = String.Empty Then
                        strMsg = Me.Language.Translate("Validate.EmptyPlates", Me.DefaultScope)
                        bolRet = False
                    End If
                End If
            End If
        End If

        ' Validación acceso web
        ' No comprobamos el nombre de usurio, pues se genera la contraseña automaticamente mediante regenerar o automaticamente al crear
        If Me.chkUserName_ = True Then
            If Not Me.oModoWizardNew Then
                Dim oPassport As roPassport = Nothing

                oPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me._ID, Me.Type, True)
                Dim intPassport As Integer = 0
                If Not oPassport Is Nothing Then
                    intPassport = oPassport.ID
                End If

                If UserAdminServiceMethods.CredentialExists(Me.Page, Me.txtUserName.Value.Trim, AuthenticationMethod.Password, "", intPassport) Then
                    strMsg = Me.Language.Translate("Validate.NotUniqueCredential", Me.DefaultScope)
                    bolRet = False
                End If
            End If
        End If

        Return bolRet

    End Function

    Private Sub SetControlsVisible()

        Me.chkUsername.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.chkUsername.Content.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.chkUsername.Description.Visible = Not Me.chkUsername.Content.Visible

        If Me.ModoWizardNew = WizardMode.ModeNewMulti Then
            Me.tableCard.Style("height") = "auto"
        End If
        Me.tdCardDescription.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.tdCardDescription2.Visible = Not Me.tdCardDescription.Visible
        Me.trCardData.Visible = Me.tdCardDescription.Visible

        Me.tdNFCDescription.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.tdNFCDescription2.Visible = Not Me.tdNFCDescription.Visible
        Me.trNFCData.Visible = Me.tdNFCDescription.Visible




        If Me.ModoWizardNew = WizardMode.ModeNewMulti Then
            Me.chkNFC.Visible = False
            Me.tableBiometric.Style("height") = "auto"
        End If

        Me.chkPin.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.chkPin.Content.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.chkPin.Description.Visible = Not Me.chkPin.Content.Visible

        Me.chkPlate.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.chkPlate.Content.Visible = (Me.ModoWizardNew <> WizardMode.ModeNewMulti)
        Me.chkPlate.Description.Visible = Not Me.chkPlate.Content.Visible

    End Sub

    Public Sub SetEnabled(ByVal bolEnabled As Boolean)

        Me.chkUsername.Enabled = bolEnabled
        Me.chkCard.Enabled = bolEnabled
        Me.chkNFC.Enabled = bolEnabled
        Me.chkBiometric.Enabled = bolEnabled
        Me.chkPin.Enabled = bolEnabled
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\TerminalConnector") Then
            Me.chkPlate.Enabled = bolEnabled
        Else
            Me.chkPlate.Enabled = False
        End If
        'Me.cmbFunctionality.Enabled = bolEnabled

    End Sub

    Public Function SaveData(ByRef strErrorInfo As String) As Boolean

        Dim bolRet As Boolean = False

        Dim oPassport As roPassport = UserAdminServiceMethods.GetPassport(Me.Page, Me.ID, Me.Type)
        If oPassport Is Nothing AndAlso Me.Type = LoadType.Employee Then
            oPassport = New roPassport()
            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Me.ID, False)
            With oPassport
                .IDEmployee = oEmployee.ID
                .Name = oEmployee.Name
                .GroupType = "E"
                .Language = WLHelperWeb.CurrentPassport.Language
                .State = 1
            End With
        End If

        If oPassport IsNot Nothing Then

            ' Cargamos información de los métodos de identificación
            Me.LoadPassport(oPassport)
            ' Guardamos el passport
            If Me.Validate(strErrorInfo) Then
                If oPassport.GroupType = "E" Then
                    ' Tenemos que revisar si es supervisor, en ese caso indicar groupType = ""
                    If API.UserAdminServiceMethods.IsSupervisorPassport(Me.Page, oPassport.ID) Then
                        oPassport.GroupType = ""
                    End If
                End If

                bolRet = UserAdminServiceMethods.SavePassport(Me.Page, oPassport, True)
                If Not bolRet Then
                    strErrorInfo = UserAdminServiceMethods.SecurityLastErrorText
                End If

                If bolRet Then

                    If oPassport.ID = WLHelperWeb.CurrentPassport.ID Then
                        ' Obtenemos el contexto actual
                        Dim oContext As WebCContext = WLHelperWeb.Context(HttpContext.Current.Request, WLHelperWeb.CurrentPassport.ID)
                        ' Verificamos si ha cambiado la contraseña
                        Dim oMethod As roPassportAuthenticationMethodsRow = oPassport.AuthenticationMethods.PasswordRow
                        If oMethod IsNot Nothing Then
                            If oMethod.Password <> oContext.Password Then
                                roWsUserManagement.RemoveCurrentsession()
                                oContext.Password = oMethod.Password
                            End If
                        End If
                    End If

                End If

            End If
        End If

        Return bolRet

    End Function

#End Region

End Class