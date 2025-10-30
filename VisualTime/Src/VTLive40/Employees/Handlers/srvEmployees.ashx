<%@ WebHandler Language="VB" Class="srvEmployees" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Data
Imports System.Net
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTBusiness.Absence
Imports Robotics.Base.VTBusiness.Incidence
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase.Extensions

Public Class srvEmployees
    Inherits handlerBase

    <Runtime.Serialization.DataContract()>
    Public Class EmployeeSummaryData
        <Runtime.Serialization.DataMember(Name:="taskNames")>
        Public TasksNames As String()

        <Runtime.Serialization.DataMember(Name:="taskValues")>
        Public TasksValues As Double()

        <Runtime.Serialization.DataMember(Name:="taskSummaryName")>
        Public TasksSummaryName As String

        <Runtime.Serialization.DataMember(Name:="centersNames")>
        Public CentersNames As String()

        <Runtime.Serialization.DataMember(Name:="centersValues")>
        Public CentersValues As Double()

        <Runtime.Serialization.DataMember(Name:="centersSummaryName")>
        Public CentersSummaryName As String

        <Runtime.Serialization.DataMember(Name:="workingNames")>
        Public WorkingNames As String()

        <Runtime.Serialization.DataMember(Name:="workingValues")>
        Public WorkingValues As Double()

        <Runtime.Serialization.DataMember(Name:="workingSummaryName")>
        Public WorkingSummaryName As String

        <Runtime.Serialization.DataMember(Name:="absenceNames")>
        Public AbsenceNames As String()

        <Runtime.Serialization.DataMember(Name:="absenceValues")>
        Public AbsenceValues As Double()

        <Runtime.Serialization.DataMember(Name:="absenceSummaryName")>
        Public AbsenceSummaryName As String

        <Runtime.Serialization.DataMember(Name:="accrual")>
        Public AccrualValue As String

        <Runtime.Serialization.DataMember(Name:="punchLocation")>
        Public PunchLocation As String
    End Class

    Private Const FeatureAlias As String = "Employees"
    Private Const FeatureAliasPlanification As String = "Calendar.Scheduler"
    Private Const FeatureDocuments As String = "Documents"
    Private Const FeatureConfigOptions As String = "Administration.Options.General"

    Private CurrentIDEmployee As Integer

    '-> Private strInfo As String = ""
    Private lblIdentifyMethodsInfo As String = ""
    Private lblContractsDescription As String = ""
    Private lblContractsInfo As String = ""
    Private actualTab As Integer = 0 'TAB actual

    Private oPermission As Permission               ' Permiso configurado sobre la funcionalidad 'Employees'
    Private oPermissionShc As Permission            ' Permiso configurado sobre la funcionalidad 'Calendario'
    Private oCurrentPermission As Permission        ' Permiso configurado sobre el empleado actual
    Private oUserFieldsPermission As Permission     ' Permiso configurado sobre la información de la ficha del empleado actual ('Employees.UserFields.Information')
    Private oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None} ' Permiso configurado sobre la información de la ficha para los distintos niveles de acceso del empleado actual ('Employees.UserFields.Information.Low', 'Employees.UserFields.Information.Medium', 'Employees.UserFields.Information.High')
    Private oPermissionDoc As Permission            ' Permiso configurado sobre la funcionalidad 'Doucmentos'        
    Private oPermissionConfigOptions As Permission  ' Permiso configurado sobre la funcionalidad 'Opciones de configuración'    

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.CurrentIDEmployee = roTypes.Any2Integer(Request("ID"))

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        oPermissionShc = Me.GetFeaturePermission(FeatureAliasPlanification)
        oPermissionDoc = GetFeaturePermission(FeatureDocuments)
        oPermissionConfigOptions = Me.GetFeaturePermission(FeatureConfigOptions)

        If Me.CurrentIDEmployee > 0 Then
            Me.oCurrentPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias, Me.CurrentIDEmployee)
        Else
            Me.oCurrentPermission = Permission.Admin
        End If

        If Me.oPermission > Permission.None And Me.oCurrentPermission > Permission.None Then

            Me.oUserFieldsPermission = Me.GetFeaturePermissionByEmployee(FeatureAlias & ".UserFields.Information", Me.CurrentIDEmployee)
            Me.oUserFieldsAccessPermission(0) = Me.GetFeaturePermissionByEmployee(FeatureAlias & ".UserFields.Information.Low", Me.CurrentIDEmployee)
            Me.oUserFieldsAccessPermission(1) = Me.GetFeaturePermissionByEmployee(FeatureAlias & ".UserFields.Information.Medium", Me.CurrentIDEmployee)
            Me.oUserFieldsAccessPermission(2) = Me.GetFeaturePermissionByEmployee(FeatureAlias & ".UserFields.Information.High", Me.CurrentIDEmployee)

            Select Case context.Request("action")
                Case "getEmployeeTab" ' Retorna un Usuari (Tab Superior)
                    LoadEmployeeDataTab()
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "updateHighlightColor"
                    updateHighlightColor(Request("ID"))
                Case "chgName" 'Cambia el Nom de l'usuari seleccionat
                    changeNameEmployee(Request("ID"))
                Case "chgForgottenRight"
                    changeForgottenRightEmployee(Request("ID"))
                Case "deleteProgAus" 'Eliminar una linia del Grid de Ausencias
                    DeleteProgAus()
                Case "deleteProgInc" 'Eliminar una linia del Grid de Ausencias
                    DeleteProgInc()
                Case "deleteProgrammedAbsence" 'Eliminar una ausencia programada
                    DeleteProgrammedAbsence()
                Case "deleteProgHolidays" 'Eliminar una ausencia programada
                    DeleteProgrammedHoliday()
                Case "deleteProgOvertime" 'Eliminar una prevision horas exceso
                    DeleteProgrammedOvertime()
                Case "AuditUserFieldQuery"
                    AuditUserFieldQuery()
                Case "deleteEmp" 'Elimina un usuari
                    deleteEmployee()
                Case "DeleteBiometricDataByEmployee"
                    DeleteBiometricDataByEmployee()
                Case "DrawEmployeeSummary"
                    CreateSummaryData(roTypes.Any2Integer(Request("ID")), Request("Type"), Request("Range"))
                Case "employeeHasData"
                    checkIfEmployeeHasData()
                Case "DeleteBiometricsAllEmployees"
                    DeleteBiometricDataOfAllEmployees()
            End Select

        Else
            ' Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Dim strResponse As String = "MESSAGE" &
                          "TitleKey=CheckPermission.Denied.Title&" +
                          "DescriptionKey=CheckPermission.Denied.Description&" +
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" +
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Response.Write(strResponse)
        End If
    End Sub

    ''' <summary>
    ''' Carrega l'usuari per ID (sols el Tab superior)
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub LoadEmployeeDataTab()
        Try
            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, Request("ID"), False)
            If oEmployee Is Nothing Then Exit Sub

            Me.CurrentIDEmployee = Request("ID")
            actualTab = Request("aTab")

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = System.Web.VirtualPathUtility.ToAbsolute("~/Employees/loadimage.aspx?IdEmployee=" & oEmployee.ID & "&NewParam=" & Now.TimeOfDay.Seconds.ToString)
            img.Attributes("style") = "cursor:pointer;"
            img.Attributes.Add("OnClick", "ShowChangeEmployeeImage('" & Me.CurrentIDEmployee & "');")
            img.Width = 80
            oImageDiv.Controls.Add(img)

            'Recupera les descripcions
            Dim strFullGroupName As String = String.Empty
            StateDescriptions(oEmployee.ID, strFullGroupName)

            Dim strChangeNameJS As String = "onclick=""EditName('true');"""
            ' Miramos si el pasport acutal tiene permisos para modificar el nombre
            If Me.GetFeaturePermissionByEmployee("Employees.NameFoto", Me.CurrentIDEmployee) < Permission.Write Then
                strChangeNameJS = ""
            End If

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameEmp"" class=""NameText"">  " & oEmployee.Name & "</span></div>" &
                                SetEmployeeStateInfo() & "<br>" &
                                "<a href=""javascript: void(0)"" onclick=""changeEmployeeTabs(2);"" class=""barDescription"">" & lblContractsInfo & "</a><br>" &
                                "<a href=""javascript: void(0)"" onclick=""ShowMoveCurrentEmployee('" & oEmployee.ID & "');"" class=""barDescription"">" & strFullGroupName & "</a><br>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateEmployeeTabs)

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            oMainDiv.RenderControl(htw)

            Response.Write(sw.ToString)

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Genera els botons de Usuaris de la dreta (General, Contratos, Fichajes, Permisos...)
    ''' </summary>
    ''' <returns>Retorna un HTML Table amb els botons en format columna</returns>
    ''' <remarks></remarks>
    Private Function CreateEmployeeTabs() As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell
        Dim intindex As Integer
        Dim intAbsence As Integer
        Dim intAssignment As Integer
        Dim intSupervisor As Integer
        Dim intPunches As Integer

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_GeneralEmployees", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(1)
        oTabButtons(1) = Nothing

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_UserField", Me.Language.Translate("tabUserField", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        ReDim Preserve oTabButtons(2)
        oTabButtons(2) = Nothing

        oTabButtons(2) = CreateNewHtmlAnchor("TABBUTTON_Contract", Me.Language.Translate("tabContract", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(2))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        intindex = 2

        '==========================================
        'Aqui partim en 2 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        hCellGen = New HtmlTableCell
        hCellGen.Attributes("valign") = "top"

        'Regenerem la taula
        hTableButtons = New HtmlTable
        hTableRowButtons = New HtmlTableRow
        hTableCellButtons = New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0
        '==========================================

        intindex = intindex + 1
        intPunches = intindex

        ReDim Preserve oTabButtons(intindex)
        oTabButtons(intindex) = Nothing

        oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_Punches", Me.Language.Translate("tabPunches", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(intindex))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        Dim oLicSupport As New roLicenseSupport()
        Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

        If Not oLicInfo.Edition = roServerLicense.roVisualTimeEdition.Starter Then
            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") And
               Me.HasFeaturePermissionByEmployee("Employees.Assignments", Permission.Read, Me.CurrentIDEmployee) Then

                intindex = intindex + 1
                intAssignment = intindex

                ReDim Preserve oTabButtons(intindex)
                oTabButtons(intindex) = Nothing

                oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_Assignment", Me.Language.Translate("tabAssignment", Me.DefaultScope), "bTab")
                hTableCellButtons.Controls.Add(oTabButtons(intindex))
                hTableRowButtons.Cells.Add(hTableCellButtons)
                hTableButtons.Rows.Add(hTableRowButtons)
                hTableCellButtons = New HtmlTableCell
                hTableRowButtons = New HtmlTableRow
            End If
        End If

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ProgrammedAbsences") AndAlso oPermissionShc > 0 Then
            intindex = intindex + 1
            intAbsence = intindex

            ReDim Preserve oTabButtons(intindex)
            oTabButtons(intindex) = Nothing

            oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_Absence", Me.Language.Translate("tabAbsence", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(intindex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
        End If

        '==========================================
        'Aqui partim en 2 columnes els TABS...
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)

        hCellGen = New HtmlTableCell
        hCellGen.Attributes("valign") = "top"

        'Regenerem la taula
        hTableButtons = New HtmlTable
        hTableRowButtons = New HtmlTableRow
        hTableCellButtons = New HtmlTableCell

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0
        '==========================================

        intindex = intindex + 1
        intSupervisor = intindex

        ReDim Preserve oTabButtons(intindex)
        oTabButtons(intindex) = Nothing

        oTabButtons(intindex) = CreateNewHtmlAnchor("TABBUTTON_Supervisors", Me.Language.Translate("tabSupervisors", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(intindex))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        oTabButtons(0).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(0);")
        oTabButtons(1).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(1);")
        oTabButtons(2).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(2);")
        oTabButtons(intPunches).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(3);")
        oTabButtons(intSupervisor).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(6);")

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ProgrammedAbsences") AndAlso oPermissionShc > 0 Then
            '-> If Me.ProgrammedAbsencesLicense Then
            oTabButtons(intAbsence).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(4);")
        Else
            If actualTab = intAbsence Then actualTab = 0
        End If

        '-> If Me.AssignmentsLicense And (Me.HasFeaturePermissionByEmployee("Employees.Assignments", Permission.Read, Me.CurrentIDEmployee)) Then
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") And
           Me.HasFeaturePermissionByEmployee("Employees.Assignments", Permission.Read, Me.CurrentIDEmployee) Then
            oTabButtons(intAssignment).Attributes.Add("OnClick", "javascript: changeEmployeeTabs(5);")
        Else
            If actualTab = intAssignment Then actualTab = 0
        End If

        If actualTab > intSupervisor Then
            actualTab = intSupervisor
        End If

        oTabButtons(actualTab).Attributes("class") = "bTab-active"
        Return hTableGen ' Retorna el HTMLTable

    End Function

    Private Function SetEmployeeStateInfo() As String
        ' Acutliza la información resumen estado empleado 'Presente/Ausente'
        Dim strInfo As String = ""
        Dim strOnClick As String = ""

        Dim tbAbsences As DataTable = Me.ProgrammedAbsencesData()

        If tbAbsences IsNot Nothing Then
            Dim oAbsence() As DataRow = tbAbsences.Select("BeginDate <= '" & Format(Now.Date, "yyyy/MM/dd") & "' AND " &
                                         "RealFinishDate >= '" & Format(Now.Date, "yyyy/MM/dd") & "'",
                                         "BeginDate DESC")
            If oAbsence.Length > 0 Then
                Dim oParams As New Generic.List(Of String)
                oParams.Add(oAbsence(0)("Name"))
                oParams.Add(CDate(oAbsence(0)("BeginDate")))
                oParams.Add(CDate(oAbsence(0)("RealFinishDate")))
                strInfo = Me.Language.Translate("StateInfo.ScheduledAbsence", Me.DefaultScope, oParams)
                If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ProgrammedAbsences") Then
                    '-> If Me.ProgrammedAbsencesLicense Then
                    strOnClick = "javascript: var url = 'Employees/EditProgrammedAbsence.aspx?EmployeeID=" & Me.CurrentIDEmployee & "&NewRecord=0'; " &
                                             "url = url + '&BeginDate=" & Format(oAbsence(0)("BeginDate"), HelperWeb.GetShortDateFormat) & "'; " &
                                             "parent.ShowExternalForm2(url, 830, 485, '', '',false,false); "
                End If
            End If
        End If

        If strInfo = "" Then
            ' Buscamos el último marcaje
            'Dim oLastMove As EmployeeService.roMove = API.EmployeeServiceMethods.GetLastMove(Me, Me.CurrentIDEmployee)
            Dim oLastMove As roPunch = API.EmployeeServiceMethods.GetLastPunch(Nothing, Me.CurrentIDEmployee)
            Dim oParams As New Generic.List(Of String)
            Dim oParams2 As New Generic.List(Of String)
            If oLastMove IsNot Nothing Then
                If oLastMove.ActualType = PunchTypeEnum._OUT Then
                    If roTypes.Any2Double(oLastMove.TypeData) > 0 Then
                        Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Nothing, oLastMove.TypeData, False)
                        If oCause IsNot Nothing Then
                            oParams2.Add(oCause.Name)
                            oParams.Add(Me.Language.Translate("StateInfo.Out.Cause", Me.DefaultScope, oParams2))
                        Else
                            oParams.Add("")
                        End If
                    Else
                        oParams.Add("")
                    End If
                    oParams.Add(oLastMove.DateTime.Value.Date)
                    oParams.Add(Format(oLastMove.DateTime.Value, "HH:mm"))
                    strInfo = Me.Language.Translate("StateInfo.Out", Me.DefaultScope, oParams)
                    If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Calendar") Then
                        strOnClick = "javascript: var url = 'Scheduler/MovesNew.aspx?GroupID=-1'; " &
                         "url = url + '&EmployeeID=" & Me.CurrentIDEmployee & "&Date=" & Format(oLastMove.DateTime.Value.Date, "dd/MM/yyyy") & "'; " &
                         "parent.ShowExternalForm2(url, 1400, 620, '', '', false, false, false);"

                    End If
                ElseIf oLastMove.ActualType = PunchTypeEnum._IN Then
                    If roTypes.Any2Double(oLastMove.TypeData) > 0 Then
                        Dim oCause As roCause = CausesServiceMethods.GetCauseByID(Nothing, oLastMove.TypeData, False)
                        If oCause IsNot Nothing Then
                            oParams2.Add(oCause.Name)
                            oParams.Add(Me.Language.Translate("StateInfo.In.Cause", Me.DefaultScope, oParams2))
                        Else
                            oParams.Add("")
                        End If
                    Else
                        oParams.Add("")
                    End If
                    oParams.Add(oLastMove.DateTime.Value.Date)
                    oParams.Add(Format(oLastMove.DateTime.Value, "HH:mm"))
                    strInfo = Me.Language.Translate("StateInfo.In", Me.DefaultScope, oParams)

                    If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Calendar") Then

                        strOnClick = "javascript: var url = 'Scheduler/MovesNew.aspx?GroupID=-1'; " &
                         "url = url + '&EmployeeID=" & Me.CurrentIDEmployee & "&Date=" & Format(oLastMove.DateTime.Value.Date, "dd/MM/yyyy") & "'; " &
                         "parent.ShowExternalForm2(url, 1400, 620, '', '', false, false, false);"

                    End If
                End If
            End If

            Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, Me.CurrentIDEmployee, False)

            If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Productiv") And oEmployee.Type = "J" Then
                Dim xDate As Date

                Dim intTaskPunch As Integer = API.PunchServiceMethods.GetLastPunchTaskInfoByEmployee(Nothing, Me.CurrentIDEmployee, PunchTypeEnum._TASK, xDate)
                If intTaskPunch > 0 Then
                    Dim oLastTaskPunch As roPunch = API.PunchServiceMethods.GetPunch(Nothing, intTaskPunch, False)
                    If Not oLastTaskPunch Is Nothing Then
                        Dim strTask As String = API.TasksServiceMethods.GetNameTask(Nothing, oLastTaskPunch.TypeData)
                        Dim oParams3 As New Generic.List(Of String)
                        oParams3.Add(strTask)
                        oParams3.Add(oLastTaskPunch.DateTime.Value.Date)
                        oParams3.Add(Format(oLastTaskPunch.DateTime.Value, "HH:mm"))
                        strInfo = strInfo & Me.Language.Translate("StateInfo.Task", Me.DefaultScope, oParams3)
                    End If
                End If
            End If

        End If

        Return "<a href=""javascript: void(0)"" onclick=""" & strOnClick & """  class=""barDescription"" style=""" & IIf(strOnClick = "", "cursor: default;", "") & """>" & strInfo & "</a>"

    End Function

    ''' <summary>
    ''' Recupera les descripcions
    ''' </summary>
    ''' <param name="IdEmployee"></param>
    ''' <param name="strFullGroupName"></param>
    ''' <remarks></remarks>
    Private Sub StateDescriptions(ByVal IdEmployee As Integer, ByRef strFullGroupName As String)
        Try
            ' Mostrar información resumen tipo identificación empleado
            lblIdentifyMethodsInfo = ""
            If Me.HasFeaturePermissionByEmployee("Employees.IdentifyMethods", Permission.Read, Me.CurrentIDEmployee) Then
                Dim strMsgId As String = ""
                Dim oEmployeePassport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, IdEmployee, LoadType.Employee)
                If oEmployeePassport IsNot Nothing Then
                    Dim tbMethods As roPassportAuthenticationMethods = oEmployeePassport.AuthenticationMethods
                    Dim oCardMethods() As roPassportAuthenticationMethodsRow = tbMethods.CardRows
                    Dim oBioMethods() As roPassportAuthenticationMethodsRow = tbMethods.BiometricRows
                    Dim oPinMethods As roPassportAuthenticationMethodsRow = tbMethods.PinRow
                    If oCardMethods IsNot Nothing AndAlso oCardMethods.Length > 0 Then strMsgId &= "OrCard"
                    If oBioMethods IsNot Nothing AndAlso oBioMethods.Length > 0 Then strMsgId &= "OrBio"
                    If oPinMethods IsNot Nothing Then strMsgId &= "OrPin"
                    If strMsgId <> "" Then
                        strMsgId = strMsgId.Substring(2)
                    Else
                        strMsgId = "NoPunches"
                    End If
                Else
                    strMsgId = "NoPunches"
                End If
                lblIdentifyMethodsInfo = Me.Language.Translate("MeansAccess.PType." & strMsgId, Me.DefaultScope)
            End If

            ' Cargo el literal de contratos
            lblContractsDescription = ""
            lblContractsInfo = ""
            If Me.HasFeaturePermissionByEmployee("Employees.Contract", Permission.Read, Me.CurrentIDEmployee) Then
                Dim oContract As roContract = API.ContractsServiceMethods.GetActiveContract(Nothing, Me.CurrentIDEmployee, False)
                If oContract IsNot Nothing Then
                    If API.ContractsServiceMethods.LastError.Result = ContractsResultEnum.ContractNotFound Then
                        lblContractsDescription = Me.Language.Translate("Employee_ContractsDescription.NoContract", Me.DefaultScope)
                    Else
                        If oContract.EndDate.Date = New Date(2079, 1, 1) Then
                            Dim Params As New Generic.List(Of String)
                            Params.Add(oContract.IDContract)
                            Params.Add(oContract.BeginDate.ToShortDateString)
                            lblContractsDescription = Me.Language.Translate("Employee_ContractsDescription", Me.DefaultScope, Params)
                        Else
                            Dim Params As New Generic.List(Of String)
                            Params.Add(oContract.IDContract)
                            Params.Add(oContract.BeginDate.ToShortDateString)
                            Params.Add(Format(oContract.EndDate, HelperWeb.GetShortDateFormat))
                            lblContractsDescription = Me.Language.Translate("Employee_ContractsDescription.EndDate", Me.DefaultScope, Params)
                        End If
                    End If
                    lblContractsInfo = lblContractsDescription
                End If
            End If

            ' Cargo el literal de mobilidad
            If Me.HasFeaturePermissionByEmployee("Employees.GroupMobility", Permission.Read, Me.CurrentIDEmployee) Then
                'Dim oMobility As EmployeeService.roMobility = API.EmployeeServiceMethods.GetCurrentMobility(Me, Me.CurrentIDEmployee)
                'If oMobility IsNot Nothing Then
                '    Dim oArrayList As New Generic.List(Of String)
                '    oArrayList.Add(oMobility.Name)
                '    oArrayList.Add(oMobility.BeginDate.ToShortDateString)
                '    lblMobilityInfo = Me.Language.Translate("Employee_Group", Me.DefaultScope) & oMobility.Name
                'End If

                Dim AuxFullGroupName = API.EmployeeServiceMethods.GetCurrentFullGroupName(Nothing, Me.CurrentIDEmployee)
                strFullGroupName = Me.Language.Translate("Employee_Group", Me.DefaultScope) & " " & AuxFullGroupName

            End If

        Catch ex As Exception

        End Try
    End Sub

    ''' <summary>
    ''' Crea la barra d'eines que va al TAB de la capcelera
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub GetBarButtons(ByVal sID As String)
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Employees\Employees", WLHelperWeb.CurrentPassportID)
            Dim guiActionsGroups As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Employees\Groups", WLHelperWeb.CurrentPassportID)

            For Each action As roGuiAction In guiActionsGroups
                If action.IDPath <> "MaxMinimize" Then
                    If action.IDPath <> "DeleteGroup" Then
                        guiActions.Add(action)
                    End If
                End If
            Next

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Employees")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Genera automaticament HtmlAnchors
    ''' </summary>
    ''' <param name="Name">Nom del boton (ID)</param>
    ''' <param name="Text">Texte (InnerText)</param>
    ''' <param name="CssClassPrefix">No es fa servir...</param>
    ''' <returns>un HTMLButton</returns>
    ''' <remarks></remarks>
    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

    ''' <summary>
    ''' Retorna un Datable amb les dades de Ausencies
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function ProgrammedAbsencesData() As DataTable
        Try
            Dim tb As DataTable
            tb = API.ProgrammedAbsencesServiceMethods.GetProgrammedAbsences(Nothing, Me.CurrentIDEmployee)

            ' Añadir la columna con el literal que define la ausencia
            tb.Columns.Add(New DataColumn("Literal", GetType(String)))
            Dim Params As Generic.List(Of String)
            For Each oRow As DataRow In tb.Rows
                If Not IsDBNull(oRow("IDCause")) Then
                    Params = New Generic.List(Of String)
                    Params.Add(CDate(oRow("BeginDate")).ToShortDateString)
                    Params.Add(CDate(oRow("RealFinishDate")).ToShortDateString)
                    Params.Add(oRow("Name"))
                    If Not IsDBNull(oRow("Description")) AndAlso oRow("Description") <> "" Then
                        Params.Add("(" & oRow("Description") & ")")
                    Else
                        Params.Add("")
                    End If
                    oRow("Literal") = Me.Language.Translate("ProgrammedAbsence.Literal", Me.DefaultScope, Params)
                End If
            Next

            Return tb
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Sub updateHighlightColor(ByVal IDEmployee As Integer)
        Dim strResponse As String = "OK"
        Dim strErrorInfo As String = ""

        Dim oState As New roWsState
        Dim bolSaveData As Boolean

        'Carrega Grid Usuaris per comprobar el tipus de columna
        Dim dsUsuari As roEmployee
        dsUsuari = API.EmployeeServiceMethods.GetEmployee(Nothing, IDEmployee, False)

        ' Guardo los UserFields del employee actual
        bolSaveData = False
        Dim recievedColor As String = ""
        For Each cVars As String In Request.Params
            If cVars.ToString = "ID" Then Continue For
            If cVars.ToString = "action" Then Continue For
            If cVars.ToString = "StampParam" Then Continue For

            If cVars.ToString = "highlightColor" Then
                recievedColor = Request("highlightColor")
                Dim auxColor As System.Drawing.Color = Drawing.ColorTranslator.FromHtml(recievedColor)
                Dim intColor As Integer = Drawing.ColorTranslator.ToWin32(auxColor)
                dsUsuari.HighlightColor = intColor
                bolSaveData = True
            End If
        Next

        If bolSaveData = False Then
            strErrorInfo = Me.Language.Translate("InvalidRecievedColor", Me.DefaultScope) '"No es un color válido"
        Else
            API.EmployeeServiceMethods.SaveEmployee(Nothing, dsUsuari)
        End If

        If strErrorInfo <> "" Then
            strResponse = "MESSAGE" &
                          "TitleKey=UpdateColor.Error.Title&" +
                          "DescriptionText=" + strErrorInfo + "&" +
                          "Option1TextKey=UpdateColor.Error.Option1Text&" +
                          "Option1DescriptionKey=UpdateColor.Error.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
        End If

        Response.Write(strResponse)
    End Sub

    ''' <summary>
    ''' Cambia el nom de l'usuari
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub changeNameEmployee(ByVal IDEmployee As Integer)
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            If Me.GetFeaturePermissionByEmployee("Employees.NameFoto", Me.CurrentIDEmployee) >= Permission.Write Then

                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IDEmployee, False)

                If oEmployee Is Nothing Then Exit Sub
                Me.CurrentIDEmployee = IDEmployee

                Dim NouName As String = Request("NewName")
                Dim idLang As Integer = -1
                Dim isProductiv As String = Request("IsProductiv")

                If NouName = "" Then
                    strErrorInfo = Me.Language.Translate("NameInvalid", Me.DefaultScope)
                Else
                    oEmployee.Name = NouName
                    If isProductiv.ToUpper = "TRUE" Then
                        oEmployee.Type = "J"
                    Else
                        oEmployee.Type = "A"
                    End If

                    bolRet = API.EmployeeServiceMethods.SaveEmployee(Nothing, oEmployee)
                    If Not bolRet Then
                        strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText

                    Else
                        Dim oPassport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, Me.CurrentIDEmployee, LoadType.Employee)

                        If Request("NewLang") IsNot Nothing AndAlso IsNumeric(Request("NewLang")) Then
                            idLang = roTypes.Any2Integer(Request("NewLang"))
                        End If

                        Dim bUpdated = API.UserAdminServiceMethods.UpdatePassportNameAndLanguage(Nothing, oPassport.ID, NouName, idLang)

                        'If idLang IsNot Nothing AndAlso IsNumeric(idLang) Then
                        '    oEmpPassport.IDLanguage = roTypes.Any2Integer(idLang)
                        'End If

                        'bolRet = API.UserAdminServiceMethods.SavePassport(Nothing, oEmpPassport)
                        strErrorInfo = API.UserAdminServiceMethods.LastErrorText
                    End If
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally
            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=ChangeNameEmployee.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=ChangeNameEmployee.Error.Option1Text&" +
                              "Option1DescriptionKey=ChangeNameEmployee.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)

        End Try
    End Sub

    ''' <summary>
    ''' Cambia la configuración del derecho al olvido de los usuarios
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub changeForgottenRightEmployee(ByVal IDEmployee As Integer)
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            If Me.GetFeaturePermissionByEmployee("Employees", Me.CurrentIDEmployee) >= Permission.Write Then

                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, IDEmployee, False)

                If oEmployee Is Nothing Then Exit Sub
                Me.CurrentIDEmployee = IDEmployee

                Dim hasForgottenRight As String = Request("HasForgottenRight")

                If hasForgottenRight.ToUpper = "TRUE" Then
                    oEmployee.HasForgottenRight = True
                Else
                    oEmployee.HasForgottenRight = False
                End If

                bolRet = API.EmployeeServiceMethods.SaveEmployee(Nothing, oEmployee)
                If Not bolRet Then
                    strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
                End If
            Else
                strErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally
            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=ChangeForgottenRightEmployee.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=ChangeForgottenRightEmployee.Error.Option1Text&" +
                              "Option1DescriptionKey=ChangeForgottenRightEmployee.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)

        End Try
    End Sub

    ''' <summary>
    ''' Elimina la linea del Grid de Ausencias
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProgAus()
        Try
            Me.CurrentIDEmployee = Request("ID")
            Dim oProgrammedAbsence As New roProgrammedAbsence
            ' Obtenemos los campos de la grid
            oProgrammedAbsence.IDCause = Request("IDCause")
            oProgrammedAbsence.IDEmployee = Request("ID")
            Dim strDate As String = Request("BeginDate")
            oProgrammedAbsence.BeginDate = CDate(strDate).Date
            'oProgrammedAbsence.BeginDate = New Date(strDate.Substring(6, 4), strDate.Substring(3, 2), strDate.Substring(0, 2))

            If Not API.ProgrammedAbsencesServiceMethods.DeleteProgrammedAbsence(Nothing, oProgrammedAbsence, True) Then
                'ERROR
                Response.Write("MESSAGE" & roWsUserManagement.SessionObject.States.ProgrammedAbsenceState.ErrorText)
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina la linea del Grid de Ausencias
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProgrammedOvertime()
        Try
            Dim idProgrammedOvertime As Integer = Request("IDProgOvertime")

            Dim oProgOvertime As roProgrammedOvertime = ProgrammedOvertimesServiceMethods.GetProgrammedOvertimeById(Nothing, idProgrammedOvertime, False)

            If Not ProgrammedOvertimesServiceMethods.DeleteProgrammedOvertime(oProgOvertime, Nothing, True) Then
                Response.Write("MESSAGE" & roWsUserManagement.SessionObject.States.ProgrammedOvertimeState.ErrorText)
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina la linea del Grid de Ausencias
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProgrammedHoliday()
        Try
            Dim idProgrammedHoliday As Integer = Request("IDProgHoliday")

            Dim oProgHoliday As roProgrammedHoliday = ProgrammedHolidaysServiceMethods.GetProgrammedHolidayById(Nothing, idProgrammedHoliday, False)

            If Not ProgrammedHolidaysServiceMethods.DeleteProgrammedHoliday(oProgHoliday, Nothing, True) Then
                Response.Write("MESSAGE" & roWsUserManagement.SessionObject.States.ProgrammedHolidayState.ErrorText)
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina la linea del Grid de Incidencias
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProgInc()
        Try
            Me.CurrentIDEmployee = Request("ID")
            Dim oProgrammedcause As New roProgrammedCause
            ' Obtenemos los campos de la grid
            oProgrammedcause.IDCause = Request("IDCause")
            oProgrammedcause.IDEmployee = Request("ID")
            oProgrammedcause.ID = Request("IDAbsence")
            Dim strDate As String = Request("BeginDate")
            oProgrammedcause.ProgrammedDate = CDate(strDate).Date

            If Not API.ProgrammedCausesServiceMethods.DeleteProgrammedCause(Nothing, oProgrammedcause, True) Then
                'ERROR
                Response.Write("MESSAGE" & roWsUserManagement.SessionObject.States.ProgrammedCauseState.ErrorText)
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString)
        End Try
    End Sub

    ''' <summary>
    ''' Elimina la ausencia programada simple
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteProgrammedAbsence()
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim oProgrammedAbsence As New roProgrammedAbsence
            oProgrammedAbsence.IDCause = roTypes.Any2Integer(Request("IDCause"))
            oProgrammedAbsence.IDEmployee = roTypes.Any2Integer(Request("ID"))
            Dim strDate As String = Request("BeginDate")
            oProgrammedAbsence.BeginDate = DateTime.Parse(strDate)

            If API.ProgrammedAbsencesServiceMethods.DeleteProgrammedAbsence(Nothing, oProgrammedAbsence, True) Then
                rError = New roJSON.JSONError(False, "OK")
            Else
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.ProgrammedAbsenceState.ErrorText)
            End If

            Response.Write(rError.toJSON)

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub AuditUserFieldQuery()
        Dim strResponse As String = ""
        Try
            Dim strFieldName As String = roTypes.Any2String(Request.Params("FieldName"))
            If strFieldName <> "" Then
                Dim oEmployeeUserField As roEmployeeUserField = API.EmployeeServiceMethods.GetEmployeeUserFieldValueAtDate(Nothing, Me.CurrentIDEmployee, strFieldName, Now.Date)
                strResponse = "OK"
            End If
        Catch ex As Exception
            Response.Write("MESSAGE" & ex.Message.ToString)
        End Try
        Response.Write(strResponse)
    End Sub

    Private Sub checkIfEmployeeHasData()
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            bolRet = Robotics.Web.Base.API.EmployeeServiceMethods.CheckIfEmployeeHasData(Nothing, Me.CurrentIDEmployee)
            If Not bolRet Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally

            Dim strResponse As String
            If Not bolRet Then
                strResponse = "NODATA"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=DeleteEmployee.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=DeleteEmployee.Error.Option1Text&" +
                              "Option1DescriptionKey=DeleteEmployee.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)

        End Try
    End Sub

    ''' <summary>
    ''' Borra el usuari seleccionat
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub deleteEmployee()
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            bolRet = Robotics.Web.Base.API.EmployeeServiceMethods.DeleteEmployee(Nothing, Me.CurrentIDEmployee)
            If Not bolRet Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally

            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=DeleteEmployee.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=DeleteEmployee.Error.Option1Text&" +
                              "Option1DescriptionKey=DeleteEmployee.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)

        End Try
    End Sub

    ''' <summary>
    ''' Borra datos biometricos del empleado seleccionado
    ''' </summary>
    ''' <remarks></remarks>
    Private Sub DeleteBiometricDataByEmployee()
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            bolRet = Robotics.Web.Base.API.EmployeeServiceMethods.DeleteBiometricData(Nothing, Me.CurrentIDEmployee)
            If Not bolRet Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally

            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=DeleteBiometricsEmployee.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=DeleteBiometricsEmployee.Error.Option1Text&" +
                              "Option1DescriptionKey=DeleteBiometricsEmployee.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)
        End Try
    End Sub

    '' <summary>
    '' Borra datos biometricos de todos los empleados
    '' </summary>
    '' <remarks></remarks>
    Private Sub DeleteBiometricDataOfAllEmployees()
        Dim bolRet As Boolean = False
        Dim strErrorInfo As String = ""
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If oPermissionConfigOptions < Permission.Write Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If
            bolRet = Robotics.Web.Base.API.EmployeeServiceMethods.DeleteBiometricDataForAllEmployees(Nothing)
            If Not bolRet Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorDetail
            End If
        Catch ex As Exception
            strErrorInfo = ex.Message
        Finally

            Dim strResponse As String
            If bolRet Then
                strResponse = "OK"
            Else
                strResponse = "MESSAGE" &
                              "TitleKey=DeleteBiometricForAllEmployees.Error.Title&" +
                              "DescriptionText=" + strErrorInfo + "&" +
                              "Option1TextKey=DeleteBiometricForAllEmployees.Error.Option1Text&" +
                              "Option1DescriptionKey=DeleteBiometricForAllEmployees.Error.Option1Description&" +
                              "Option1OnClickScript=HideMsgBoxForm(); return false;&" +
                              "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon)
            End If
            Response.Write(strResponse)
        End Try
    End Sub

    Public Sub CreateSummaryData(idEmployee As Integer, type As String, range As String)
        Dim summaryType As New SummaryType
        Select Case range
            Case SummaryType.Anual.ToString
                summaryType = SummaryType.Anual
            Case SummaryType.Mensual.ToString
                summaryType = SummaryType.Mensual
            Case SummaryType.Semanal.ToString
                summaryType = SummaryType.Semanal
            Case SummaryType.Daily.ToString
                summaryType = SummaryType.Daily
            Case SummaryType.Contrato.ToString
                summaryType = SummaryType.Contrato
            Case SummaryType.LastYear.ToString
                summaryType = SummaryType.LastYear
            Case SummaryType.LastMonth.ToString
                summaryType = SummaryType.LastMonth
            Case SummaryType.ContractAnnualized.ToString
                summaryType = SummaryType.ContractAnnualized
        End Select

        WLHelperWeb.Context(Request).SummaryType = summaryType

        Select Case type
            Case "accrual"
                LoadAccrualData(idEmployee, range, summaryType)
            Case "cause"
                LoadCauseData(idEmployee, range, summaryType)
            Case "tasks"
                LoadTaskData(idEmployee, range, summaryType)
            Case "centers"
                LoadCentersData(idEmployee, range, summaryType)
        End Select
    End Sub

    Private Sub LoadAccrualData(idEmployee As Integer, range As String, summaryType As SummaryType)

        Dim summary = API.EmployeeServiceMethods.GetEmployeesSummaryById(Nothing, idEmployee, Date.Now, summaryType, SummaryType.Anual, SummaryType.Anual, SummaryType.Anual, SummaryRequestType.Accruals)

        If summary IsNot Nothing AndAlso (summary.employeeAccruals IsNot Nothing AndAlso summary.employeeAccruals.Count > 0) Then
            Dim divAccualDraw As New HtmlGenericControl("div")
            For Each accrual As roAccrualsSummary In summary.employeeAccruals.OrderBy(Function(ac) ac.Name).ToList()
                Dim divAccrual As New HtmlGenericControl("div")
                divAccrual.Attributes("class") = "accrual"

                Dim divAccrualName As New HtmlGenericControl("div")
                divAccrualName.InnerHtml = WebUtility.HtmlEncode(If(accrual.Name.Length > 32, accrual.Name.Substring(0, 32).Replace("\", "_").PadRight(33, " ") & "...", accrual.Name.Replace("\", "_").PadRight(33, " ")))
                divAccrualName.Attributes("title") = accrual.Name
                divAccrual.Controls.Add(divAccrualName)

                Dim divAccrualImage As New HtmlGenericControl("div")
                divAccrualImage.Attributes("class") = "accrualSummaryBox"

                Dim divColorOne As New HtmlGenericControl("div")
                divColorOne.Attributes("class") = "color1"
                divColorOne.Attributes("id") = "divColor1"
                Dim spanTotal As New HtmlGenericControl("span")

                If (Not accrual.MaxValue.Equals(0)) Then

                    Dim divColorTwo As New HtmlGenericControl("div")
                    divColorTwo.Attributes("class") = "accrualWarningColor"

                    If (accrual.MaxValue < 0) Then
                        divColorTwo.Style("float") = "left"
                    End If

                    Dim spanTotal2 As New HtmlGenericControl("span")
                    spanTotal2.Style("color") = "black"

                    If accrual.Total <= 0 Then
                        divColorTwo.Style("width") = "100%"
                        If accrual.Type = "O" Then
                            spanTotal2.InnerHtml = accrual.TotalFormat '& " de " & Format(accrual.MaxValue, "##0.000")
                            spanTotal.InnerHtml = accrual.TotalFormat '& " de " & Format(accrual.MaxValue, "##0.000")
                        Else
                            spanTotal.InnerHtml = roConversions.ConvertHoursToTime(accrual.Total) '& " de " & roConversions.ConvertHoursToTime(accrual.MaxValue)
                            spanTotal2.InnerHtml = roConversions.ConvertHoursToTime(accrual.Total) '& " de " & roConversions.ConvertHoursToTime(accrual.MaxValue)
                        End If
                    Else
                        spanTotal.InnerHtml = accrual.TotalFormat
                        divColorOne.Controls.Add(spanTotal)
                        Dim colorDivWidht = (100 - CType(((accrual.Total * 100) / accrual.MaxValue), Integer))
                        divColorTwo.Style("width") = If(colorDivWidht >= 100, "75%", colorDivWidht.ToString & "%")
                        divColorTwo.Style("max-width") = "145px"
                        If accrual.Type = "O" Then
                            spanTotal2.InnerHtml = (accrual.MaxValue - accrual.Total)
                        Else
                            spanTotal2.InnerHtml = roConversions.ConvertHoursToTime(accrual.MaxValue - accrual.Total)
                        End If
                    End If

                    divColorTwo.Controls.Add(spanTotal2)
                    divColorOne.Controls.Add(divColorTwo)
                Else
                    spanTotal.InnerHtml = accrual.TotalFormat
                    divColorOne.Controls.Add(spanTotal)
                End If

                divAccrualImage.Controls.Add(divColorOne)
                divAccrual.Controls.Add(divAccrualImage)
                divAccualDraw.Controls.Add(divAccrual)
            Next

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            divAccualDraw.RenderControl(htw)

            Response.Write(sw.ToString)
        End If
    End Sub

    Private Sub LoadCauseData(idEmployee As Integer, range As String, summaryType As SummaryType)

        Dim summary = API.EmployeeServiceMethods.GetEmployeesSummaryById(Nothing, idEmployee, Date.Now, SummaryType.Anual, summaryType, SummaryType.Anual, SummaryType.Anual, SummaryRequestType.Causes)
        If (summary.employeeCauses IsNot Nothing AndAlso summary.employeeCauses.Count > 0) Then
            Dim divCausesDraw As New HtmlGenericControl("div")
            For Each cause As roCausesSummary In summary.employeeCauses.OrderBy(Function(ca) ca.Name).ToList()
                Dim divCause As New HtmlGenericControl("div")
                divCause.Attributes("class") = "accrual"

                Dim divCauseName As New HtmlGenericControl("div")
                divCauseName.InnerHtml = WebUtility.HtmlEncode(If(cause.Name.Length > 32, cause.Name.Substring(0, 32).Replace("\", "_").PadRight(33, " ") & "...", cause.Name.Replace("\", "_").PadRight(33, " ")))
                divCauseName.Attributes("title") = cause.Name
                divCause.Controls.Add(divCauseName)

                Dim divCauseImage As New HtmlGenericControl("div")
                divCauseImage.Attributes("class") = "accrualSummaryBox"

                Dim divColorOne As New HtmlGenericControl("div")
                divColorOne.Attributes("class") = "color1"
                Dim spanTotal As New HtmlGenericControl("span")
                spanTotal.InnerHtml = cause.TotalFormat
                divColorOne.Controls.Add(spanTotal)

                If (Not cause.Limit.Equals(0)) Then

                    Dim divColorTwo As New HtmlGenericControl("div")
                    divColorTwo.Attributes("class") = "accrualWarningColor"

                    If (cause.Limit < 0) Then
                        divColorTwo.Style("float") = "left"
                    End If

                    Dim spanTotal2 As New HtmlGenericControl("span")
                    spanTotal2.Style("color") = "black"

                    If cause.Limit <= 0 Then
                        divColorTwo.Style("width") = "100%"
                        spanTotal.InnerHtml = roConversions.ConvertHoursToTime(cause.Total) '& " de " & roConversions.ConvertHoursToTime(accrual.MaxValue)
                        spanTotal2.InnerHtml = roConversions.ConvertHoursToTime(cause.Total) '& " de " & roConversions.ConvertHoursToTime(accrual.MaxValue)
                    Else
                        spanTotal.InnerHtml = roConversions.ConvertHoursToTime(cause.Limit - cause.Total)
                        divColorOne.Controls.Add(spanTotal)
                        Dim colorDivWidht = (100 - CType((((cause.Limit - cause.Total) * 100) / cause.Limit), Integer))
                        divColorTwo.Style("width") = If(colorDivWidht >= 100, "75%", colorDivWidht.ToString & "%")
                        divColorTwo.Style("max-width") = "145px"

                        spanTotal2.InnerHtml = cause.TotalFormat
                    End If

                    divColorTwo.Controls.Add(spanTotal2)
                    divColorOne.Controls.Add(divColorTwo)
                Else
                    spanTotal.InnerHtml = cause.TotalFormat
                    divColorOne.Controls.Add(spanTotal)
                End If

                'If (Not cause.Limit.Equals(0)) Then
                '    Dim divColorTwo As New HtmlGenericControl("div")
                '    divColorTwo.Attributes("class") = "causeLimitColor"
                '    If (cause.Limit < 0) Then
                '        divColorTwo.Style("float") = "left"
                '    End If
                '    Dim colorDivWidht = (100 - CType(((cause.Total * 100) / cause.Limit), Integer))
                '    divColorTwo.Style("width") = If(colorDivWidht >= 100, "75%", colorDivWidht.ToString & "%")
                '    Dim spanTotal2 As New HtmlGenericControl("span")
                '    spanTotal2.Style("color") = "black"
                '    spanTotal2.InnerHtml = roConversions.ConvertHoursToTime(cause.Limit - cause.Total)
                '    divColorTwo.Controls.Add(spanTotal2)
                '    divColorOne.Controls.Add(divColorTwo)
                'End If

                divCauseImage.Controls.Add(divColorOne)
                divCause.Controls.Add(divCauseImage)
                divCausesDraw.Controls.Add(divCause)
            Next
            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            divCausesDraw.RenderControl(htw)

            Response.Write(sw.ToString)
        End If
    End Sub

    Private Sub LoadTaskData(idEmployee As Integer, range As String, summaryType As SummaryType)
        Dim summaryJson As New EmployeeSummaryData
        Dim summary = API.EmployeeServiceMethods.GetEmployeesSummaryById(Nothing, idEmployee, Date.Now, SummaryType.Anual, SummaryType.Anual, summaryType, SummaryType.Anual, SummaryRequestType.Tasks)

        If (summary.employeeTasks IsNot Nothing AndAlso summary.employeeTasks.Count > 0) Then
            Dim arrayValue = If(summary.employeeTasks.Count > 8, 8, summary.employeeTasks.Count - 1)
            Dim taskNames(arrayValue) As String
            Dim taskValues(arrayValue) As Double
            For value As Integer = 0 To arrayValue
                taskNames(value) = summary.employeeTasks(value).TaskName.Replace("\", "\\")
                taskValues(value) = summary.employeeTasks(value).TaskValue
            Next
            summaryJson.TasksNames = taskNames
            summaryJson.TasksValues = taskValues
            summaryJson.TasksSummaryName = Language.Translate("Summary.TaskSummaryName", DefaultScope) & " " & roConversions.ConvertHoursToTime(taskValues.Sum())
        End If
        Response.Write(roJSONHelper.SerializeNewtonSoft(summaryJson))

    End Sub

    Private Sub LoadCentersData(idEmployee As Integer, range As String, summaryType As SummaryType)
        Dim summaryJson As New EmployeeSummaryData
        Dim summary = API.EmployeeServiceMethods.GetEmployeesSummaryById(Nothing, idEmployee, Date.Now, SummaryType.Anual, SummaryType.Anual, SummaryType.Anual, summaryType, SummaryRequestType.CostCenter)

        If (summary.employeeBussinessCenters IsNot Nothing AndAlso summary.employeeBussinessCenters.Count > 0) Then
            'claculo los totales de horas para mostrar las leyendas
            Dim totalHoursCenters = summary.employeeBussinessCenters.Sum(Function(f) (f.EmployeeCost * f.CauseCostFactor))
            Dim totalHoursWorking = summary.employeeBussinessCenters.Where(Function(w) w.CauseType).Sum(Function(f) f.EmployeeCost * f.CauseCostFactor)
            Dim totalHoursAbsence = summary.employeeBussinessCenters.Where(Function(a) Not a.CauseType).Sum(Function(f) f.EmployeeCost * f.CauseCostFactor)
            Dim percentageWorking As Double = If(totalHoursCenters > 0, ((totalHoursWorking * 100) / totalHoursCenters), 0)
            Dim percentageAbsence As Double = If(totalHoursCenters > 0, ((totalHoursAbsence * 100) / totalHoursCenters), 0)

            Dim workingPor = (Math.Truncate(percentageWorking * 100) / 100).ToString
            Dim absPor = (Math.Truncate(percentageAbsence * 100) / 100).ToString

            ' Quesito de todos los centros de coste
            Dim centersValues = summary.employeeBussinessCenters.
                           GroupBy(Function(g) New With {Key g.CenterName}).
                           Select(Function(group) New With {
                              .CenterName = group.Key.CenterName,
                              .TotalAmount = group.Sum(Function(a) a.EmployeeCost * a.CauseCostFactor)}).ToList()

            If Not (centersValues.Count.Equals(1) AndAlso String.IsNullOrEmpty(centersValues(0).CenterName)) Then
                Dim arrayValue = If(centersValues.Count > 8, 8, centersValues.Count - 1)
                Dim bCentersNames(arrayValue) As String
                Dim bCentersValues(arrayValue) As Double
                For value As Integer = 0 To arrayValue

                    Dim centerName As String = Me.Language.Translate("NoCenter", Me.DefaultScope)

                    If centersValues(value).CenterName.Trim <> String.Empty Then
                        centerName = WebUtility.HtmlEncode(If(centersValues(value).CenterName.Length > 35, centersValues(value).CenterName.Substring(0, 31).Replace("\", "_").PadRight(35, " ") & "...", centersValues(value).CenterName.Replace("\", "_").PadRight(35, " ")))
                    End If

                    bCentersNames(value) = centerName
                    bCentersValues(value) = centersValues(value).TotalAmount
                Next
                summaryJson.CentersNames = bCentersNames
                summaryJson.CentersValues = bCentersValues
                summaryJson.CentersSummaryName = Language.Translate("Summary.SummaryCentersName", DefaultScope) & " " & (Math.Truncate(bCentersValues.Sum * 100) / 100).ToString & " €"

                'Quesito de las justificaciones de presencia y ausencia

                Dim causesValues = summary.employeeBussinessCenters.
                           GroupBy(Function(bc) New With {Key .Type = bc.CauseType, Key .Name = bc.CauseName}).
                           Select(Function(group) New With {
                              .CauseName = group.Key.Name,
                              .Type = group.Key.Type,
                              .TotalAmount = group.Sum(Function(a) a.EmployeeCost * a.CauseCostFactor)}).ToList()

                causesValues = causesValues.OrderByDescending(Function(f) f.TotalAmount).ToList

                If Not (causesValues.Count.Equals(1) AndAlso String.IsNullOrEmpty(causesValues(0).CauseName)) Then
                    Dim workingCauses = causesValues.Where(Function(cw) cw.Type).ToList()
                    Dim absencesCauses = causesValues.Where(Function(cw) Not cw.Type).ToList()

                    Dim arrayValueAb = If(absencesCauses.Count > 8, 8, absencesCauses.Count - 1)
                    Dim absencesNames(arrayValueAb) As String
                    Dim absencesValues(arrayValueAb) As Double

                    For value As Integer = 0 To arrayValueAb
                        absencesNames(value) = WebUtility.HtmlEncode(If(absencesCauses(value).CauseName.Length > 35, absencesCauses(value).CauseName.Substring(0, 31).Replace("\", "_").PadRight(35, " ") & "...", absencesCauses(value).CauseName.Replace("\", "_").PadRight(35, " ")))
                        absencesValues(value) = absencesCauses(value).TotalAmount
                    Next

                    Dim arrayValuePre = If(workingCauses.Count > 8, 8, workingCauses.Count - 1)
                    Dim presenceNames(arrayValuePre) As String
                    Dim presenceValues(arrayValuePre) As Double

                    For value As Integer = 0 To arrayValuePre
                        presenceNames(value) = WebUtility.HtmlEncode(If(workingCauses(value).CauseName.Length > 35, workingCauses(value).CauseName.Substring(0, 31).Replace("\", "_").PadRight(35, " ") & "...", workingCauses(value).CauseName.Replace("\", "_").PadRight(35, " ")))
                        presenceValues(value) = workingCauses(value).TotalAmount
                    Next

                    summaryJson.AbsenceNames = absencesNames
                    summaryJson.AbsenceValues = absencesValues
                    summaryJson.AbsenceSummaryName = Language.Translate("Summary.SummaryAbscence", DefaultScope) & " " & absPor & "%"

                    summaryJson.WorkingNames = presenceNames
                    summaryJson.WorkingValues = presenceValues
                    summaryJson.WorkingSummaryName = Language.Translate("Summary.SummaryWorking", DefaultScope) & " " & workingPor & "%"

                End If
            End If
        End If
        Response.Write(roJSONHelper.SerializeNewtonSoft(summaryJson))
    End Sub

End Class