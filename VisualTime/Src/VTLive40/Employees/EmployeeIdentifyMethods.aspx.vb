Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class Forms_EmployeeIdentifyMethods
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.IdentifyMethods"

#Region "Declarations"

    Private intIDEmployee As Integer

    Private oPermission As Permission

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
            Me.hdnIDEmployee.Value = Me.intIDEmployee
        End If

        Me.oPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intIDEmployee)
        If Me.oPermission > Permission.None Then

            Me.btSave.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
                Me.DisableControls(Me.cnIdentifyMethods.Controls)
            End If

            Me.cnIdentifyMethods.Type = LoadType.Employee
            Me.cnIdentifyMethods.ID = Me.intIDEmployee

            AddHandler Me.MessageFrame1.OptionOnClick, AddressOf MessageClick
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub MediosAccesoLocal_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnIdentifyMethods.Load

        If Not Me.IsPostBack Then

            Me.cnIdentifyMethods.LoadData(LoadType.Employee, Me.intIDEmployee)
            If Me.cnIdentifyMethods.Exceptioncode = SecurityResultEnum.PassportDoesNotExists Then
                Me.MustRefresh = "10"
                Me.CanClose = True
            End If

        End If

    End Sub

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        If Me.oPermission >= Permission.Write Then

            Dim strErrorInfo As String = ""

            'If Me.MediosAccesoLocal.SaveData(Me.intIDEmployee) Then
            If Me.cnIdentifyMethods.SaveData(strErrorInfo) Then

                If Me.cnIdentifyMethods.isPasswordResetted Then

                    Dim oPwd As roPassportAuthenticationMethodsRow = API.UserAdminServiceMethods.GetPassport(Me, Me.cnIdentifyMethods._ID, LoadType.Employee).AuthenticationMethods.PasswordRow

                    'Dim newPwd = CryptographyHelper.Decrypt(oPwd.Password)
                    Dim newPwd = oPwd.Password
                    Dim oParams As New Generic.List(Of String)
                    oParams.Add(newPwd)
                    Me.MustRefresh = "9#" & Me.Language.Translate("SavePassport.InfoPwd.ResetDescription", Me.DefaultScope, oParams)
                Else
                    Me.MustRefresh = "7"
                End If

                Me.CanClose = True
            Else
                HelperWeb.ShowMessage(Me.Page, "", strErrorInfo, , , , HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Public Event OnMessageClick(ByVal btn As String)

    Protected Sub MessageClick(ByVal btn As String)
        RaiseEvent OnMessageClick(btn)
        'Me.MediosAccesoLocal.OnMessageClick(btn)
    End Sub

#End Region

End Class