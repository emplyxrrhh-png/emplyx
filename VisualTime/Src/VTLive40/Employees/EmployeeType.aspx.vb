Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API.EmployeeServiceMethods

Partial Class EmployeeType
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.Type"

    Private Enum enumEmployeesTask
        SinProblemas
        SinContrato
        SinLicencia
    End Enum

#Region "Declarations"

    Private intIDEmployee As Integer
    Private oEmployee As roEmployee

    Private oPermission As Permission = Permission.None

#End Region

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        ' Obtenemos el código de empleado actual
        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
            Me.oEmployee = API.EmployeeServiceMethods.GetEmployee(Me, Me.intIDEmployee, True)
            Me.hdnJobEnabled.Value = roTypes.Any2String(oEmployee.Type)
        End If

        ' Si el passport actual no tiene permisso de lectura, rediriguimos a pàgina de acceso denegado
        Me.oPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.intIDEmployee)
        If Me.oPermission <= Permission.Read Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        If Not Me.IsPostBack Then

            If Me.oEmployee IsNot Nothing Then

                Me.cnEmployeeType.LoadData(Me.oEmployee)

                Me.SetPermissions()
            Else
                WLHelperWeb.RedirectAccessDenied(True)
            End If

        End If

    End Sub

    Private Function ChekMaxEmployeesTask() As enumEmployeesTask

        Dim eRet As enumEmployeesTask = enumEmployeesTask.SinProblemas

        Dim FechaInicioContrato As Date = Date.Today

        Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Me, oEmployee.ID, False)
        If oContract IsNot Nothing AndAlso API.ContractsServiceMethods.LastError.Result = ContractsResultEnum.NoError Then
            FechaInicioContrato = oContract.BeginDate
        Else
            eRet = enumEmployeesTask.SinContrato
        End If

        If eRet = enumEmployeesTask.SinProblemas Then
            Dim EmpleadosMaximo As Integer = roTypes.Any2Integer(API.LicenseServiceMethods.FeatureData("VisualTime Server", "MaxJobEmployees"))
            Dim EmpleadosCreados As Integer = API.EmployeeServiceMethods.GetActiveEmployeesTaskCount(Me, FechaInicioContrato)

            Dim Total As Integer = EmpleadosMaximo - (EmpleadosCreados + 1)
            If Total < 0 Then
                eRet = enumEmployeesTask.SinLicencia
            End If
        End If

        Return eRet

    End Function

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btnAccept.Click

        If Me.oPermission >= Permission.Write Then

            If Me.cnEmployeeType.RetrieveData(Me.oEmployee) Then

                Dim bolSaveEmployee As Boolean = True

                If Me.oEmployee.Type = "J" And Me.hdnJobEnabled.Value <> "J" Then

                    Dim eRet As enumEmployeesTask = ChekMaxEmployeesTask()

                    If eRet <> enumEmployeesTask.SinProblemas Then bolSaveEmployee = False

                    If eRet = enumEmployeesTask.SinLicencia Then
                        HelperWeb.ShowMessage(Me, "", Me.Language.Translate("MaximumEmployeeTaskReached.Message", Me.DefaultScope),
                                               , , , HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon))

                    ElseIf eRet = enumEmployeesTask.SinContrato Then
                        HelperWeb.ShowMessage(Me, "", Me.Language.Translate("EmployeeWithoutContract.Message", Me.DefaultScope),
                                              , , , HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon))
                    End If
                End If

                If bolSaveEmployee Then
                    If SaveEmployee(Me, Me.oEmployee) Then
                        Me.MustRefresh = "1"
                        Me.CanClose = True
                    End If
                End If

            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

#End Region

#Region "Methods"

    Private Sub SetPermissions()

        If Me.oPermission < Permission.Write Then

            Me.DisableControls()

            Me.btnAccept.Style("display") = "none"
            Me.btnCancel.Text = Me.Language.Keyword("Button.Close")

        End If

    End Sub

#End Region

End Class