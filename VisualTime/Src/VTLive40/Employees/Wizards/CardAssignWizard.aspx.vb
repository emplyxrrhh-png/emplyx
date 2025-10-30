Imports Robotics.Base.DTOs
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Wizards_CardAssignWizard
    Inherits PageBase

#Region "Declarations"

    Private intActivePage As Integer

    Private lngIDCard As Long

#End Region

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.IdentifyMethods", Permission.Write) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Dim CardID As String = Request.Params("CardID")
        If CardID IsNot Nothing AndAlso CardID.Length > 0 Then
            Me.lngIDCard = CLng(CardID)
        End If

        If Not Me.IsPostBack Then

            Me.hdnEmpToCopy.Value = ""

            Me.lblWelcome2.Text = Me.lblWelcome2.Text.Replace("{0}", Me.lngIDCard)

            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text

            Me.intActivePage = 0

            HelperWeb.roSelector_Initialize("roChildSelectorW_treeEmployeesCardAssignWizard")
        Else

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.DivStep1.Style("display") <> "none" Then Me.intActivePage = 1

        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim intOldPage As Integer = Me.intActivePage
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

        Me.PageChange(1, -1)

        If Me.CheckPage(Me.intActivePage) Then

            Dim bolSaved As Boolean = False
            Dim strErrorInfo As String = ""

            ' Obtener el empleado seleccionado
            If Me.hdnEmpToCopy.Value <> "" AndAlso Me.hdnEmpToCopy.Value.StartsWith("B") Then

                Dim intIDEmployee As Integer = Me.hdnEmpToCopy.Value.Substring(1)

                Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Me, intIDEmployee, LoadType.Employee)
                If oPassport IsNot Nothing Then

                    Dim bolAddMethod As Boolean
                    Dim oCardMethod As roPassportAuthenticationMethodsRow() = oPassport.AuthenticationMethods.CardRows

                    Dim oMethod As roPassportAuthenticationMethodsRow = Nothing
                    If oCardMethod IsNot Nothing AndAlso oCardMethod.Length > 0 Then
                        oMethod = oCardMethod(0)
                    End If

                    If oMethod Is Nothing Then
                        bolAddMethod = True
                        oMethod = New roPassportAuthenticationMethodsRow
                        With oMethod
                            .IDPassport = oPassport.ID
                            .Method = AuthenticationMethod.Card
                            .Version = ""
                            .BiometricID = 0
                            .Password = ""
                        End With
                    End If
                    oMethod.Credential = Me.lngIDCard
                    oMethod.Enabled = True
                    oMethod.RowState = RowState.UpdateRow

                    If bolAddMethod Then
                        oCardMethod = {oMethod}
                    End If
                    oPassport.AuthenticationMethods.CardRows = oCardMethod

                    If Not API.UserAdminServiceMethods.SavePassport(Me.Page, oPassport, False) Then
                        strErrorInfo = roWsUserManagement.SessionObject.States.SecurityState.ErrorText
                    Else
                        ' Asignamos el código de empleado a todos los fichajes sin asignar
                        Dim tbPunches As DataTable = API.PunchServiceMethods.GetInvalidPunchesByIDCard(Me, Me.lngIDCard)
                        If tbPunches IsNot Nothing Then
                            If tbPunches.Rows.Count > 0 Then
                                For Each oPunchesRow As DataRow In tbPunches.Rows
                                    If roTypes.Any2Double(oPunchesRow("IDEmployee")) <= 0 Then
                                        oPunchesRow("IDEmployee") = intIDEmployee
                                    End If
                                Next
                            End If
                        End If

                        bolSaved = API.PunchServiceMethods.SaveInvalidPunches(Me, tbPunches)
                        If Not bolSaved Then
                            strErrorInfo = roWsUserManagement.SessionObject.States.PunchState.ErrorText
                        End If
                    End If
                End If

                '' Cargar el contrato activo
                'Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Me, intIDEmployee)
                'If oContract IsNot Nothing Then

                '    oContract.IDCard = Me.lngIDCard

                '    bolSaved = API.ContractsServiceMethods.SaveContract(Me, oContract)
                '    If Not bolSaved Then
                '        strErrorInfo = API.ContractsServiceMethods.oState.ErrorText
                '    End If

                'End If
            Else ' No existe el empleado
                strErrorInfo = Me.Language.Translate("InvalidEmployee.Message", Me.DefaultScope)
            End If

            Me.lblWelcome1.Text = Me.Language.Translate("End.CardAssignWelcome1.Text", Me.DefaultScope)
            If bolSaved Then

                Me.MustRefresh = Me.lngIDCard '"1"

                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.CardAssignWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.CardAssignWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.PageChange(1, 0)
        Else
            Me.PageChange(-1, 1)
        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                ' Hemos de tener un empleado seleccionado
                If Me.hdnEmpToCopy.Value = "" OrElse Me.hdnEmpToCopy.Value.StartsWith("A") Then
                    strMsg = Me.Language.Translate("CheckPage.Page1.NoEmployeeSelected", Me.DefaultScope)
                    bolRet = False
                Else
                    Dim _IDEmployee As Integer = Me.hdnEmpToCopy.Value.Substring(1)
                    ' Verificar que el empleado puede fichar con tarjeta
                    Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, _IDEmployee, False)
                    If oEmployee Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.Page1.InvalidEmployee", Me.DefaultScope)
                        bolRet = False
                    End If

                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 1
                ' Desactivar el iframe del selector de empleados
                Me.ifEmployeeSelector.Attributes("src") = Me.Page.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeeSelector.Disabled = True

                ' Guardar la selección de grupos
                Dim lstEmployeeSelection As New ArrayList
                Dim lstEmployeeNamesSelection As New ArrayList
                lstEmployeeSelection = HelperWeb.LastEmployeeSelection(lstEmployeeNamesSelection)

        End Select

        Select Case intActivePage
            Case 1

                ''HelperWeb.EmployeeSelector_SetSelection(New ArrayList, New ArrayList)
                ' Activar iframe del selector de empleados
                ''Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("../Base/WebUserControls/EmployeeSelectorPage.aspx?ShowOnlyGroups=0&MultiSelect=0" & _
                ''                                                                                "&Height=" & Me.ifEmployeeSelector.Attributes("height") & _
                ''                                                                                "&Width=" & Me.ifEmployeeSelector.Attributes("width"))

                Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=111&TreesMultiSelect=000&TreesOnlyGroups=000&TreeFunction=parent.cargaEmp&FilterFloat=false&" &
                                                                                                                               "PrefixTree=treeEmployeesCardAssignWizard&" &
                                                                                                                               "FeatureAlias=Employees")

                Me.ifEmployeeSelector.Disabled = False

        End Select

        If intActivePage >= 0 Then

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

        End If

    End Sub

#End Region

End Class