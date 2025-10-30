Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class Forms_EmployeeAllowedApplications
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.AllowedApplications"

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

        ' Posicionar el div de progreso
        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intIDEmployee)
        If Me.oPermission > Permission.None Then

            Me.btSave.Visible = (Me.oPermission >= Permission.Write)
            If Me.oPermission = Permission.Read Then
                Me.btCancel.Text = Me.Language.Keyword("Button.Close")
                Me.DisableControls(Me.cnAllowedApplications.Controls)
            End If

            Me.cnAllowedApplications.Type = LoadType.Employee
            Me.cnAllowedApplications.ID = Me.intIDEmployee

            AddHandler Me.MessageFrame1.OptionOnClick, AddressOf MessageClick
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub MediosAccesoLocal_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnAllowedApplications.Load

        If Not Me.IsPostBack Then

            Me.cnAllowedApplications.LoadData(LoadType.Employee, Me.intIDEmployee)

            If Me.cnAllowedApplications.Exceptioncode = SecurityResultEnum.PassportDoesNotExists Then
                Me.MustRefresh = "10"
                Me.CanClose = True
            End If
        End If

    End Sub

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        If Me.oPermission >= Permission.Write Then

            Dim strErrorInfo As String = ""

            'If Me.MediosAccesoLocal.SaveData(Me.intIDEmployee) Then
            If Me.cnAllowedApplications.SaveData(strErrorInfo) Then

                Me.MustRefresh = "7"

                Me.CanClose = True
            Else
                HelperWeb.ShowMessage(Me.Page, "", strErrorInfo)
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