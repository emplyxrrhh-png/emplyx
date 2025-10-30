Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Tasks_Wizards_CreateTaskWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Task.TaskCreate"

    Private intActivePage As Integer

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

    Private Enum eIdoneidadComparation
        Equal
        EqualLess
        EqualMore
        Less
        More
    End Enum

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        'Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.oPermission = Permission.Admin
        If Me.oPermission < Permission.Write Then
            WLHelperWeb.RedirectAccessDenied(True)
        End If

        Me.LoadList()

        If Not Me.IsPostBack Then

            Me.btClose.Visible = Not Me.IsPopup

            Me.lblStep1Title.Text = Me.hdnStepTitle.Text & Me.lblStep1Title.Text
            Me.lblStep2Title.Text = Me.hdnStepTitle.Text & Me.lblStep2Title.Text
            Me.lblStep3Title.Text = Me.hdnStepTitle.Text & Me.lblStep3Title.Text
            Me.lblStep4Title.Text = Me.hdnStepTitle.Text & Me.lblStep4Title.Text
            Me.lblStep5Title.Text = Me.hdnStepTitle.Text & Me.lblStep5Title.Text

            Me.intActivePage = 0

            'Inicializamos el selector de empleados
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpCreateTaskWizard")
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpCreateTaskWizardGrid")
        Else

            If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
            If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
            If Me.divStep2.Style("display") <> "none" Then Me.intActivePage = 2
            If Me.divStep3.Style("display") <> "none" Then Me.intActivePage = 3
            If Me.divStep4.Style("display") <> "none" Then Me.intActivePage = 4
            If Me.divStep5.Style("display") <> "none" Then Me.intActivePage = 5

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

        If Me.CheckPage(Me.intActivePage) Then

            '-------> CREAR TAREA <------
            CreateTask()
            CloseWizard()

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                'If Me.optCondition.Checked Then
                '    If Me.cmbUserFields_Value.Value = "" Or Me.txtValue.Text = "" Then
                '        strMsg = Me.Language.Translate("CheckPage.Page1", Me.DefaultScope)
                '    End If
                'End If
                'If strMsg <> "" Then bolRet = False
                'Me.lblStep1Error.Text = strMsg

            Case 2

                'If Me.hdnEmployeesSelected.Value = "" Then
                '    strMsg = Me.Language.Translate("CheckPage.Page2.NoEmployeeSelected", Me.DefaultScope)
                'Else
                '    If Me.hdnEmployeesSelected.Value.Split(",").Length = 1 Then
                '        If Me.hdnEmployeesSelected.Value.Substring(1) = Me.intIDEmployee Then
                '            strMsg = Me.Language.Translate("CheckPage.Page2.IncorrectSelection", Me.DefaultScope)
                '        End If
                '    End If
                'End If
                'If strMsg <> "" Then bolRet = False
                'Me.lblStep2Error.Text = strMsg

            Case 3

                'If txtBeginDate.Value = "" OrElse Not IsDate(txtBeginDate.Value) OrElse _
                '   txtEndDate.Value = "" OrElse Not IsDate(Me.txtEndDate.Value) Then
                '    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectDates", Me.DefaultScope)
                'ElseIf CDate(Me.txtBeginDate.Value) > CDate(Me.txtEndDate.Value) Then
                '    strMsg = Me.Language.Translate("CheckPage.Page3.IncorrectPeriod", Me.DefaultScope)
                'End If
                'If strMsg <> "" Then bolRet = False
                'Me.lblStep3Error.Text = strMsg

            Case 4

                'If Me.cmbTypes_Value.Value = "" Then
                '    strMsg = Me.Language.Translate("CheckPage.Page4", Me.DefaultScope)
                'End If
                'If strMsg <> "" Then bolRet = False
                'Me.lblStep4Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 1
                'Dim strFilter As String = ""
                'If Me.optCondition.Checked Then
                '    strFilter = "Employees.[" & Me.cmbUserFields_Value.Value & "] LIKE '" & Me.txtValue.Text.Replace("*", "%") & "'"
                'End If
                'HelperWeb.EmployeeSelector_SetUserFieldsFilter(strFilter)

            Case 2

                '' Desactivar el iframe del selector de grupos
                'Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                'Me.ifEmployeeSelector.Disabled = True

        End Select

        Select Case intActivePage
            Case 2

                ''Inicializar selección del selector de empleados
                'Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" & _
                '                       "PrefixTree=treeEmpCreateTaskWizard&FeatureAlias=Employees&PrefixCookie=objContainerTreeV3_treeEmpCreateTaskWizardGrid&" & _
                '                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                'Me.ifEmployeeSelector.Attributes("src") = Me.ResolveUrl(strAux)
                'Me.ifEmployeeSelector.Disabled = False

        End Select

        Me.hdnActiveFrame.Value = intActivePage

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

        If intOldPage = 5 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 5, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 5, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub LoadList()

        Dim tbAux As DataTable = Nothing

        'Cargar plantillas
        Me.cmbTemplates.ClearItems()
        tbAux = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me.Page, "Name", False)
        For Each dRowAR As DataRow In tbAux.Rows
            Me.cmbTemplates.AddItem(dRowAR("Name"), dRowAR("Id"), "")
        Next

        'CARGAR TABLA COLORES
        CreateColorPickerTable()

        colorShift.Style("background-color") = Me.hdnColor.Value

        'CARGAR PUESTOS
        Me.cmbAssignment.ClearItems()
        tbAux = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me.Page, "Name", False)
        For Each dRowAR As DataRow In tbAux.Rows
            cmbAssignment.AddItem(dRowAR("Name"), dRowAR("Id"), "")
        Next

        'CARGAR VALORES COMPARATIVOS
        Dim strComparations() As String = System.Enum.GetNames(GetType(eIdoneidadComparation))
        Dim strValueComp() As Integer = System.Enum.GetValues(GetType(eIdoneidadComparation))
        For n As Integer = LBound(strComparations) To UBound(strComparations)
            Me.cmbComparation.AddItem(Me.Language.Keyword("Comparation." & strComparations(n)), strValueComp(n), "")
        Next

        'cargar(alfinalizartarea)
        Me.cmbFinTarea.ClearItems()
        tbAux = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me.Page, "Name", False)
        For Each dRowAR As DataRow In tbAux.Rows
            Me.cmbFinTarea.AddItem(dRowAR("Name"), dRowAR("Id"), "")
        Next

        'cargar(aliniciartarea)
        Me.cmbIniTarea.ClearItems()
        tbAux = API.AssignmentServiceMethods.GetAssignmentsDataTable(Me.Page, "Name", False)
        For Each dRowAR As DataRow In tbAux.Rows
            Me.cmbIniTarea.AddItem(dRowAR("Name"), dRowAR("Id"), "")
        Next

    End Sub

    ''' <summary>
    ''' Retorna de la cadena de selecció sols els codis d'usuari marcats
    ''' </summary>
    ''' <param name="strSelection"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function LoadEmployeesSelected(ByVal strSelection As String) As ArrayList
        Dim lstEmployeeSelection As ArrayList = New ArrayList
        If strSelection <> "" Then
            Dim Selection() As String = strSelection.Trim.Split(",")
            If Selection.Length > 0 Then
                For Each Sel As String In Selection
                    Sel = Sel.Trim
                    If Sel <> "" Then
                        Select Case Sel.Substring(0, 1)
                            Case "A" ' Grupo
                                'lstGroupSelection.Add(CInt(Sel.Substring(1)))
                            Case "B" ' Empleado
                                lstEmployeeSelection.Add(CInt(Sel.Substring(1)))
                        End Select
                    End If
                Next
            End If
        End If
        Return lstEmployeeSelection
    End Function

    Private Function GetEmployees(ByVal strIDs As String) As Generic.List(Of Integer)

        Dim EmployeeIDs As New Generic.List(Of Integer)

        Dim intIDEmployee As Integer

        For Each strID As String In strIDs.Split(",")

            If strID.StartsWith("B") Then

                intIDEmployee = strID.Substring(1)
                If Not EmployeeIDs.Contains(intIDEmployee) Then
                    EmployeeIDs.Add(intIDEmployee)
                End If

            ElseIf strID.StartsWith("A") Then

                For Each intIDEmployee In Me.GetEmployeesFromGroup(strID.Substring(1))
                    If Not EmployeeIDs.Contains(intIDEmployee) Then
                        EmployeeIDs.Add(intIDEmployee)
                    End If
                Next

                Dim tbGroups As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Calendar")
                Dim oGroups() As DataRow = tbGroups.Select("(Path LIKE '%\" & strID.Substring(1) & "\%' OR Path LIKE '" & strID.Substring(1) & "\%') AND " &
                                                           "[ID] <> " & strID.Substring(1))
                For Each oRow As DataRow In oGroups
                    For Each intIDEmployee In Me.GetEmployeesFromGroup(oRow("ID"))
                        If Not EmployeeIDs.Contains(intIDEmployee) Then
                            EmployeeIDs.Add(intIDEmployee)
                        End If
                    Next
                Next

            End If
        Next

        Return EmployeeIDs

    End Function

    Private Function GetEmployeesFromGroup(ByVal _IDGroup As Integer) As Generic.List(Of Integer)

        Dim oRet As New Generic.List(Of Integer)

        Dim tb As DataTable = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroupWithType(Me, _IDGroup, "Calendar")
        If tb IsNot Nothing Then
            For Each oRow As DataRow In tb.Rows
                oRet.Add(oRow("IDEmployee"))
            Next
        End If

        Return oRet

    End Function

    Private Sub CloseWizard()
        Me.MustRefresh = "1"
        'Me.lblCopyScheduleWelcome2.Text = Me.Language.Translate("End.Ok.CopyScheduleWelcome2.Text", Me.DefaultScope)
        'Me.lblCopyScheduleWelcome3.Text = ""
        Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Me.PageChange(5, 0)
    End Sub

    Private Sub CreateColorPickerTable()
        Dim oRow As System.Web.UI.HtmlControls.HtmlTableRow
        Dim oCell As System.Web.UI.HtmlControls.HtmlTableCell
        Dim I1 As Byte, I2 As Byte, I3 As Byte
        Dim Color As String

        For I1 = 0 To 15 Step 3
            oRow = New System.Web.UI.HtmlControls.HtmlTableRow
            For I2 = 0 To 15 Step 3
                For I3 = 0 To 15 Step 3
                    Color = Hex(I1) & Hex(I1) & Hex(I2) & Hex(I2) & Hex(I3) & Hex(I3)
                    oCell = New System.Web.UI.HtmlControls.HtmlTableCell
                    oCell.Style("background-color") = "#" & Color
                    oCell.Style("width") = "15px"
                    oCell.Style("height") = "15px"
                    oCell.Style("cursor") = "pointer"
                    If oPermission > Permission.Read Then
                        oCell.Attributes("onclick") = "ChangeColor('#" & Color & "'); showDatePicker(false);"
                    End If
                    oRow.Cells.Add(oCell)
                Next I3
            Next I2
            Me.TableColorPicker.Rows.Add(oRow)
        Next I1
    End Sub

#End Region

    Private Sub CreateTask()

        Dim DateAux As Date
        Dim DateAux2 As Date

        Dim oTask As New roTask()

        oTask.Name = Me.txtName.Value
        oTask.ShortName = Me.txtShortName.Value
        oTask.Description = Me.txtDescription.Value
        oTask.Status = TaskStatusEnum._ON

        Dim oParseColor As String = Me.hdnColor.Value.Replace("#", "")
        Dim auxColor As System.Drawing.Color = Drawing.Color.FromArgb(oParseColor)
        Dim intColor As Integer = Drawing.ColorTranslator.ToWin32(auxColor)
        oTask.Color = intColor

        oTask.Project = Me.txtProject.Value
        oTask.Tag = Me.txtTag.Value
        oTask.Priority = roTypes.Any2Integer(Me.txtPriority.Value)

        '->espera oTask.IDPassport = WLHelperWeb.CurrentPassport().ID

        If txtExpectedStartDate.Value <> String.Empty Then
            DateAux = txtExpectedStartDate.Value
            DateAux2 = txtExpectedStartTime.Value
            oTask.ExpectedStartDate = New Date(DateAux.Year, DateAux.Month, DateAux.Day, DateAux2.Hour, DateAux2.Minute, 0)
        End If

        If txtExpectedEndDate.Value <> String.Empty Then
            DateAux = txtExpectedEndDate.Value
            DateAux2 = txtExpectedEndTime.Value
            oTask.ExpectedEndDate = New Date(DateAux.Year, DateAux.Month, DateAux.Day, DateAux2.Hour, DateAux2.Minute, 0)
        End If

        '->espera
        'oTask.StartDate()
        'oTask.EndDate()
        'oTask.InitialTime()
        'oTask.TimeChangedRequirements()
        'oTask.ForecastErrorTime()
        'oTask.NonProductiveTimeIncidence()
        'oTask.EmployeeTime()
        'oTask.TeamTime()
        'oTask.MaterialTime()
        'oTask.OtherTime()
        'oTask.TypeCollaboration()
        'oTask.ModeCollaboration()
        'oTask.TypeActivation()
        'oTask.ActivationTask()
        'oTask.ActivationDate()
        'oTask.TypeClosing()
        'oTask.ClosingDate()
        'oTask.TypeAuthorization()

        'Dim bRet As Boolean = TaskService.TasksServiceMethods.SaveTask(Me, oTask, True)

    End Sub

End Class