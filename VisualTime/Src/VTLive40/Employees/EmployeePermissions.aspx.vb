Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class Employees_EmployeePermissions
    Inherits PageBase

    Private Const FeatureAlias As String = "" '"Employees.Permissions"

#Region "Declarations"

    Private intIDEmployee As Integer
    Private strPermissionsType As String = "U"

    Private oPermission As Permission

#End Region

#Region "Events"

    Public Event OnMessageClick(ByVal btn As String)

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not Me.HasFeaturePermission("Administration", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        Dim PermissionsType As String = Request.Params("PermissionsType")
        If PermissionsType IsNot Nothing AndAlso PermissionsType.Length > 0 Then
            Me.strPermissionsType = PermissionsType
        End If

        Me.oPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intIDEmployee)
        If Me.oPermission > Permission.None Or FeatureAlias = "" Then

            ''AddHandler Me.MessageFrame1.OptionOnClick, AddressOf MessageClick
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub cnPassportPermissions_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles cnPassportPermissions.Load

        If Not Me.IsPostBack Then

            Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me.Page, Me.intIDEmployee, LoadType.Employee, False)

            If oPassport IsNot Nothing Then

                Me.cnPassportPermissions.LoadData(oPassport.ID, Me.strPermissionsType)

            End If

        End If

    End Sub

    Protected Sub MessageClick(ByVal btn As String)
        RaiseEvent OnMessageClick(btn)
        'Me.MediosAccesoLocal.OnMessageClick(btn)
    End Sub

#End Region

End Class