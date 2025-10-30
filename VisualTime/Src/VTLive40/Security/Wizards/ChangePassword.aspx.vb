Imports Robotics.Base.DTOs
Imports Robotics.Security.Base
Imports Robotics.UsersAdmin.Business
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Security_ChangePassword
    Inherits PageBase

    Private strObjectType As String = ""

    Public Sub New()
        Me.OverrrideLanguageFile = "LivePortal"
        Me.OverrrideDefaultScope = "roMainMenu"
    End Sub

    Protected Sub form1_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Init

    End Sub

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        Dim bShowCancel As Integer = roTypes.Any2Integer(Request.Params("cCancel"))

        If bShowCancel = 1 Then
            Me.btChangePwdCancel.Visible = True
        Else
            Me.btChangePwdCancel.Visible = False
        End If

        'Dim strMsg = Me.Language.Translate("Validate.NoPasswordHighRequierements", Me.DefaultScope)

        'Me.lblChangePwdMessage.Text = strMsg
        'Me.lblChangePwdMessage.Style("display") = ""
        'Me.lblChangePwdMessage.Style("visibility") = "visible"

        If Not Me.IsPostBack Then

        End If

    End Sub

    Private Sub btChangePwdOK_Click(sender As Object, e As EventArgs) Handles btChangePwdOK.Click
        Dim strMsg As String = ""

        'Dim oControl As Control = Me.Page.FindControl("hdnLOPD")
        'If oControl IsNot Nothing Then CType(oControl, HiddenField).Value = "0"

        Dim oPassportTicket As roPassportTicket = WLHelperWeb.CurrentPassport
        If oPassportTicket IsNot Nothing Then

            Dim oContext As WebCContext = WLHelperWeb.Context(HttpContext.Current.Request, oPassportTicket.ID)
            If oContext IsNot Nothing Then

                If oContext.Password.Trim = CryptographyHelper.EncryptMD5(Me.txtOldPwd.Text) Then

                    If Me.txtNewPwd.Text = Me.txtNewPwdConfirm.Text Then
                        Dim resultCode As PasswordLevelError = API.SecurityServiceMethods.IsValidPwd(Nothing, oPassportTicket, oPassportTicket.ID, Me.txtNewPwd.Text, True, Me.txtOldPwd.Text)
                        Select Case resultCode
                            Case PasswordLevelError.No_Error
                                Dim oUser As wscUserAdmin = API.UserAdminServiceMethods.GetUserAdmin(Me.Page, oPassportTicket.ID)
                                oUser.Password = CryptographyHelper.EncryptMD5(Me.txtNewPwd.Text)
                                If API.UserAdminServiceMethods.UpdateUserAdmin(Me.Page, oUser) Then
                                    AuthHelper.SetPassportKeyValidated(oPassportTicket.ID, resultCode = PasswordLevelError.No_Error, WLHelperWeb.SecurityToken, True)
                                    roWsUserManagement.RemoveCurrentsession()
                                    oContext.Password = oUser.Password

                                    Dim oPassport As roPassportTicket = WLHelperWeb.CurrentPassport()

                                    HttpContext.Current.Session("LOPD") = False
                                    HttpContext.Current.Session("PASSWORDEXPIRED") = False
                                    HttpContext.Current.Session("ShowLegalText") = False

                                    'WLHelperWeb.RedirectToUrl(Me.Request.Url.AbsolutePath)
                                End If
                            Case PasswordLevelError.No_Passport_Error
                                strMsg = Me.Language.Translate("Validate.NoPassportError", Me.DefaultScope)
                            Case PasswordLevelError.Low_Error
                                strMsg = Me.Language.Translate("Validate.NoPasswordLowRequierements", Me.DefaultScope)
                            Case PasswordLevelError.Medium_Error
                                strMsg = Me.Language.Translate("Validate.NoPasswordMediumRequierements", Me.DefaultScope)
                            Case PasswordLevelError.High_Error
                                strMsg = Me.Language.Translate("Validate.NoPasswordHighRequierements", Me.DefaultScope)
                        End Select
                    Else ' Contraseñas distintas
                        strMsg = Me.Language.Translate("ChangePwd.InvalidNewPassword.Message", Me.DefaultScope)
                    End If
                Else ' Contraseña actual incorrecta
                    strMsg = Me.Language.Translate("ChangePwd.InvalidOldPassword.Message", Me.DefaultScope)
                End If

            End If

        End If

        If strMsg <> "" Then
            Me.lblChangePwdMessage.Text = strMsg
            Me.lblChangePwdMessage.Style("display") = ""
            Me.lblChangePwdMessage.Style("visibility") = "visible"
        Else
            Me.hdnMustClose.Value = "1"
        End If
    End Sub

End Class