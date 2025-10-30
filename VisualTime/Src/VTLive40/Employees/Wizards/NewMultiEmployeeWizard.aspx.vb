Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base

Partial Class Wizards_NewMultiEmployeeWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome '0

        'frmGroup
        frmNumEmployees '1

        frmContractDate '2
        frmContractNumber '3
        frmIdentifyMethods '4
        frmCardNumbers '5
        frmEditData '6
        frmType '7
        frmLocation '8
    End Enum

    Private _Action_Edit As Byte = 0
    Private _Action_Remove As Byte = 1
    Private _Action_Accept As Byte = 2
    Private _Action_Cancel As Byte = 3

    Private _Employees_EditClickIndex As Byte = 0
    Private _Employees_RemoveClickIndex As Byte = 1
    Private _Employees_selectCellIndex As Byte = 5
    Private _Employees_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}
    Private _Employees_EditCellsIndex() As Byte = {2, 3, 4}
    Private _Employees_EditControls() As String = {"IDCard_TextBox", "IDContract_TextBox", "EmployeeName_TextBox"}
    Private _Employees_CaptionControls() As String = {"IDCard_Label", "IDContract_Label", "EmployeeName_Label"}
    Private _Employees_EditFields() As String = {"IDCard", "IDContract", "EmployeeName"}

    Private oActiveFrame As Frame
    Private oPermission As Permission

#End Region

#Region "Properties"

    Private ReadOnly Property FreezeDate() As Nullable(Of Date)
        Get

            Dim oDate As Nullable(Of Date)

            If ViewState("NewMultiEmployeeWizard_FreezeDate") = Nothing Then

                Dim oParameters As roParameters = API.ConnectorServiceMethods.GetParameters(Me)
                If oParameters IsNot Nothing Then
                    Dim oParams As New roCollection(oParameters.ParametersXML)
                    Dim auxDate As Object
                    Try
                        auxDate = oParams.Item(oParameters.ParametersNames(Parameters.FirstDate))
                    Catch ex As Exception
                        auxDate = Nothing
                    End Try
                    If auxDate IsNot Nothing AndAlso IsDate(auxDate) Then
                        ViewState("NewMultiEmployeeWizard_FreezeDate") = CType(auxDate, Date).ToShortDateString()
                    Else
                        ViewState("NewMultiEmployeeWizard_FreezeDate") = String.Empty
                    End If
                End If

                Dim strDate As String = ViewState("NewMultiEmployeeWizard_FreezeDate")
                Dim Fecha As Date
                If Date.TryParse(strDate, Fecha) Then
                    oDate = Fecha
                End If
            Else

                Dim Fecha As Date
                Dim strDate As String = ViewState("NewMultiEmployeeWizard_FreezeDate")
                If Date.TryParse(strDate, Fecha) Then
                    oDate = Fecha
                End If
            End If

            Return oDate

        End Get
    End Property

    Private Property oEmployeeDefault() As roEmployee
        Get
            Dim oEmployee As roEmployee = ViewState("NewMultiEmployeeWizard_EmployeeDefault")

            If oEmployee Is Nothing Then

                oEmployee = New roEmployee
                oEmployee.Type = "A"

                ViewState("NewMultiEmployeeWizard_EmployeeDefault") = oEmployee

            End If

            Return oEmployee

        End Get
        Set(ByVal value As roEmployee)
            ViewState("NewMultiEmployeeWizard_EmployeeDefault") = value
        End Set
    End Property

    Private ReadOnly Property IDPassportWizard() As Integer
        Get
            Dim IDPassportWizardTMP As Integer = roTypes.Any2Integer(ViewState("NewMultiEmployeeWizard_IDPassportWizardTMP"))
            If IDPassportWizardTMP = 0 Then
                IDPassportWizardTMP = WLHelperWeb.CurrentPassport.ID
                ViewState("NewMultiEmployeeWizard_IDPassportWizardTMP") = IDPassportWizardTMP
            End If
            Return IDPassportWizardTMP
        End Get
    End Property

    Private Property Frames() As Generic.List(Of Frame)
        Get
            Dim oFrames As Generic.List(Of Frame) = ViewState("NewMultiEmployeeWizard_Frames")

            If oFrames Is Nothing Then

                oFrames = New Generic.List(Of Frame)
                oFrames.Add(Frame.frmWelcome)
                'oFrames.Add(Frame.frmGroup)
                oFrames.Add(Frame.frmNumEmployees)
                oFrames.Add(Frame.frmContractDate)
                oFrames.Add(Frame.frmContractNumber)

                If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower <> roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
                    oFrames.Add(Frame.frmIdentifyMethods)
                    oFrames.Add(Frame.frmCardNumbers)
                End If

                oFrames.Add(Frame.frmEditData)

                If (oPermission >= Permission.Write) Then oFrames.Add(Frame.frmType)
                oFrames.Add(Frame.frmLocation)

                ViewState("NewMultiEmployeeWizard_Frames") = oFrames

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            ViewState("NewMultiEmployeeWizard_Frames") = value
        End Set
    End Property

    Private Property EmployeesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("NewMultiEmployeeWizard_EmployeesData")

            If bolReload OrElse tb Is Nothing Then

                tb = New DataTable

                tb.Columns.Add("ID", GetType(Long))
                tb.Columns.Add("IDCard", GetType(Long))
                tb.Columns.Add("IDContract", GetType(String))
                tb.Columns.Add("EmployeeName", GetType(String))
                tb.Columns.Add("EmployeeType", GetType(String))
                tb.Columns.Add("AccControlled", GetType(Boolean))
                tb.Columns.Add("RiskControlled", GetType(Boolean))
                tb.Columns.Add("BeginDate", GetType(DateTime))
                tb.Columns.Add("IDGroup", GetType(Integer))
                tb.Columns.Add("CardMethod", GetType(Boolean))
                tb.Columns.Add("BiometricMethod", GetType(Boolean))
                tb.Columns.Add("MergeMethod", GetType(Integer))
                tb.Columns.Add("IDLabAgree", GetType(Integer))
                tb.Columns.Add("PassportGroup", GetType(Integer))
                tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}

                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    Dim oRow As DataRow = tb.NewRow
                    oRow("ID") = 0
                    oRow("IDCard") = 0
                    oRow("IDContract") = ""
                    oRow("EmployeeName") = ""
                    tb.Rows.Add(oRow)
                End If

                tb.AcceptChanges()

                Session("NewMultiEmployeeWizard_EmployeesData") = tb

            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("NewMultiEmployeeWizard_EmployeesData") = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        oPermission = GetFeaturePermission("Employees.Type")
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        If Not Me.IsPostBack Then

            If Me.HasFeaturePermission("Employees", Permission.Admin) Then

                Dim intIdGroup As Integer = roTypes.Any2Integer(Request.Params("GroupID"))
                If intIdGroup <= 0 Then
                    btNext.Visible = False
                    btPrev.Visible = False
                    lblWelcome3.Text = "PARA INICIAR EL ASISTENTE DEBE SELECCIONAR UN GRUPO DE EMPLEADOS. NO ES POSIBLE CONTINUAR"
                Else
                    Me.Frames = Nothing

                    Me.SetStepTitles()

                    HelperWeb.roSelector_Initialize("roChildSelectorW_treeGroupNewMultiEmployeeWizard")
                    Me.txtBeginContract.Date = Now.Date

                    Me.optNumberContractLast.Checked = True
                    Me.optCardsFromLast.Checked = True

                    Me.oActiveFrame = Frame.frmWelcome

                    LabAgreeVisible()

                    hdnIdGroup.Value = intIdGroup
                    Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, hdnIdGroup.Value, False)
                    If Not oGroup Is Nothing Then
                        Me.lblStep1Info2b.Text = roTypes.Any2String(oGroup.Name).ToUpper()
                    Else
                        btNext.Visible = False
                        btPrev.Visible = False
                        lblWelcome3.Text = "PARA INICIAR EL ASISTENTE DEBE SELECCIONAR UN GRUPO DE EMPLEADOS. NO ES POSIBLE CONTINUAR"
                    End If
                End If

                Me.hdnPassportSelected.Value = ""
                Me.chkParentGroupNull.Checked = True
                HelperWeb.roSelector_SetSelection("", "", "roChildSelectorW_treePassportsNewMultiEmployeeWizard")
            Else

                WLHelperWeb.RedirectAccessDenied(True)

            End If
        Else

            'Me.grdEmployees.DataSource = Me.EmployeesData
            'Me.grdEmployees.DataBind()
            'HelperWeb.EmptyGridFix(Me.grdEmployees)

            Dim oDiv As HtmlControl
            For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                If oDiv.Style("display") <> "none" Then
                    Me.oActiveFrame = Me.FrameByIndex(n)
                    Exit For
                End If
            Next

        End If

        If cmbLabAgree.Visible Then
            Dim dTblLabAgree As DataTable = API.LabAgreeServiceMethods.GetLabAgrees(Me.Page)
            cmbLabAgree.DataSource = dTblLabAgree
            cmbLabAgree.TextField = "Name"
            cmbLabAgree.ValueField = "ID"
            cmbLabAgree.ValueType = GetType(Integer)
            cmbLabAgree.DataBind()
        End If

        BindgridEmployeesWizard(False)

        Dim oParameters As roParameters = API.ConnectorServiceMethods.GetParameters(Nothing)
        If roTypes.Any2Boolean(oParameters.Parameter(Parameters.DisableBiometricData)) Then
            Me.hdnDisableBiometric.Value = "true"
        End If

        'If Not Me.IsPostBack Then
        '    CreateColumnsEmployeesWizard(True)
        '    BindgridEmployeesWizard(True)
        'Else
        '    BindgridEmployeesWizard(False)
        'End If
    End Sub

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

    Private Function ChekMaxEmployees() As Boolean
        Dim bolRet As Boolean = True

        Dim EmpleadosMaximo As Integer = roTypes.Any2Integer(API.LicenseServiceMethods.FeatureData("VisualTime Server", "MaxEmployees"))
        Dim FechaInicioContrato As Date = Me.txtBeginContract.Date

        Dim EmpleadosACrear As Integer = roTypes.Any2Integer(Me.txtNumberEmployees.Text)
        Dim EmpleadosCreados As Integer = API.EmployeeServiceMethods.GetActiveEmployeesCount(Me, FechaInicioContrato)

        If FechaInicioContrato.DayOfYear < Date.Now.DayOfYear Then
            Dim EmpleadosCreadosHoy As Integer = API.EmployeeServiceMethods.GetActiveEmployeesCount(Me, Date.Now)
            Dim TotalHoy As Integer = EmpleadosMaximo - (EmpleadosACrear + EmpleadosCreadosHoy)
            If bolRet And TotalHoy < 0 Then
                bolRet = False
            End If
        End If

        Dim Total As Integer = EmpleadosMaximo - (EmpleadosACrear + EmpleadosCreados)
        If bolRet AndAlso Total < 0 Then
            bolRet = False
        End If

        Return bolRet

    End Function

    Private Function ChekMaxEmployeesTask() As Boolean
        Dim bolRet As Boolean = True

        Dim EmpleadosMaximo As Integer = API.LicenseServiceMethods.FeatureData("VisualTime Server", "MaxJobEmployees")
        Dim FechaInicioContrato As Date = Me.txtBeginContract.Date

        Dim EmpleadosACrear As Integer = roTypes.Any2Integer(Me.txtNumberEmployees.Text)
        Dim EmpleadosCreados As Integer = API.EmployeeServiceMethods.GetActiveEmployeesTaskCount(Me, FechaInicioContrato)

        If FechaInicioContrato.DayOfYear < Date.Now.DayOfYear Then
            Dim EmpleadosCreadosHoy As Integer = API.EmployeeServiceMethods.GetActiveEmployeesTaskCount(Me, Date.Now)
            Dim TotalHoy As Integer = EmpleadosMaximo - (EmpleadosACrear + EmpleadosCreadosHoy)
            If bolRet And TotalHoy < 0 Then
                bolRet = False
            End If
        End If

        Dim Total As Integer = EmpleadosMaximo - (EmpleadosACrear + EmpleadosCreados)
        If bolRet AndAlso Total < 0 Then
            bolRet = False
        End If

        Return bolRet

    End Function

    Protected Sub btEnd_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btEnd.Click

        If Me.CheckFrame(Me.oActiveFrame) Then

            Me.FrameChange(Me.oActiveFrame, Me.Frames(Me.Frames.Count - 1))

            Dim tb As DataTable = Me.EmployeesData

            Me.cnEmployeeType.RetrieveData(oEmployeeDefault)

            Dim intPassportGroup As Integer = 0
            If Not Me.chkParentGroupNull.Checked AndAlso Me.hdnPassportSelected.Value <> String.Empty Then
                intPassportGroup = Me.hdnPassportSelected.Value
            End If

            'Creo un array con los contratos que se van a crear - JP
            Dim lstEmployeeContracts As New List(Of String)
            ' Añadimos datos necesarios para la grabación en la tabla
            For Each oRow As DataRow In tb.Rows
                oRow("EmployeeType") = oEmployeeDefault.Type '' IIf(Me.optTypeAttendance.Checked, "A", "J")
                oRow("AccControlled") = oEmployeeDefault.AccControlled
                oRow("RiskControlled") = False 'oEmployeeDefault.RiskControlled
                oRow("BeginDate") = Me.txtBeginContract.Date
                oRow("IDGroup") = hdnIdGroup.Value 'Me.hdnIDGroupSelected.Value.Substring(1)
                oRow("CardMethod") = Me.cnIdentifyMethods.chkCard_
                oRow("BiometricMethod") = Me.cnIdentifyMethods.chkBiometric_
                oRow("MergeMethod") = DBNull.Value
                If cmbLabAgree.Value IsNot Nothing AndAlso roTypes.Any2String(cmbLabAgree.Value) <> "" Then
                    oRow("IDLabAgree") = cmbLabAgree.Value
                End If
                oRow("PassportGroup") = intPassportGroup
                lstEmployeeContracts.Add(oRow("IdContract"))
            Next

            Dim strErrorInfo As String = ""
            Dim lstEmployeeNameError As New List(Of String)

            If Not API.EmployeeServiceMethods.CreateMultiEmployees(Me, tb, lstEmployeeNameError) Then
                strErrorInfo = roWsUserManagement.SessionObject.States.EmployeeState.ErrorText
            End If

            'grabamos las actividades de los empleados

            Me.lblWelcome1.Text = Me.Language.Translate("End.NewMultiEmployeeWelcome1.Text", Me.DefaultScope)
            If strErrorInfo = "" Then
                Me.MustRefresh = "1"
                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.NewMultiEmployeeWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = ""
            Else
                Dim strEmployeesError As String = ""
                For Each strEmployeeNameError As String In lstEmployeeNameError
                    strEmployeesError &= "," & strEmployeeNameError
                Next
                If strEmployeesError <> "" Then strEmployeesError = strEmployeesError.Substring(1)
                Dim MsgParams As New Generic.List(Of String)
                MsgParams.Add(strEmployeesError)
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.NewMultiEmployeeWelcome2.Text", Me.DefaultScope, MsgParams)
                Me.lblWelcome3.Text = strErrorInfo
                Me.lblWelcome3.ForeColor = Drawing.Color.Red
            End If

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)

        End If

    End Sub

#End Region

#Region "Methods"

    Private Function FrameIndex(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = CInt(oFrame)
        Return intRet
    End Function

    Private Function FramePos(ByVal oFrame As Frame) As Integer
        Dim intRet As Integer = 0
        For n As Integer = 0 To Me.Frames.Count - 1
            If Me.Frames(n) = oFrame Then
                intRet = n
                Exit For
            End If
        Next
        Return intRet
    End Function

    Private Function FrameByIndex(ByVal intIndex As Integer) As Frame
        Dim oRet As Frame = intIndex
        Return oRet
    End Function

    Private Function CheckFrame(ByVal Frame As Frame) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case Frame
            'Case Wizards_NewMultiEmployeeWizard.Frame.frmGroup

            Case Wizards_NewMultiEmployeeWizard.Frame.frmNumEmployees
                If Me.txtNumberEmployees.Text = "" OrElse Not IsNumeric(Me.txtNumberEmployees.Text) OrElse CInt(Me.txtNumberEmployees.Text) <= 0 Then
                    strMsg = Me.Language.Translate("CheckPage.NumberEmployeesIncorrect", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Wizards_NewMultiEmployeeWizard.Frame.frmContractDate

                If Me.txtBeginContract.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.BeginContractIncorrect", Me.DefaultScope)
                Else
                    Dim bContinuar As Boolean = True

                    'Comprobar fecha de congelacion
                    Dim oDate As Nullable(Of Date) = FreezeDate()
                    If oDate.HasValue Then
                        Dim x As TimeSpan = oDate.Value.Subtract(Me.txtBeginContract.Date)
                        If x.TotalDays >= 0 Then
                            Dim Params As New Generic.List(Of String)
                            Params.Add(oDate.Value.ToShortDateString)
                            strMsg = Me.Language.Translate("ContractInFreezeDate.Message", Me.DefaultScope, Params)
                            bContinuar = False
                        End If
                    End If

                    'comprobar licencias de empleados
                    If bContinuar Then
                        If Not ChekMaxEmployees() Then
                            Dim Params As New Generic.List(Of String)
                            Params.Add(FormatDateTime(Me.txtBeginContract.Date, DateFormat.ShortDate))
                            strMsg = Me.Language.Translate("MaximumEmployeeReached.Message", Me.DefaultScope, Params)
                        End If
                    End If
                End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

            Case Wizards_NewMultiEmployeeWizard.Frame.frmContractNumber
                If Me.optNumberContractNew.Checked Then
                    If Me.txtNumberContract.Text = "" OrElse Not IsNumeric(Me.txtNumberContract.Text) OrElse CLng(Me.txtNumberContract.Text) < 0 Then
                        strMsg = Me.Language.Translate("CheckPage.NumberContractIncorrect", Me.DefaultScope)
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep3Error.Text = strMsg

            Case Wizards_NewMultiEmployeeWizard.Frame.frmIdentifyMethods

            Case Wizards_NewMultiEmployeeWizard.Frame.frmCardNumbers
                If Me.optCardsFromNumber.Checked Then
                    If Me.txtCardsFromNumber.Text = "" OrElse Not IsNumeric(Me.txtCardsFromNumber.Text) OrElse CLng(Me.txtCardsFromNumber.Text) <= 0 Then
                        strMsg = Me.Language.Translate("CheckPage.BeginNumberCardIncorrect", Me.DefaultScope)
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep5Error.Text = strMsg

            Case Wizards_NewMultiEmployeeWizard.Frame.frmEditData
                If gridEmployeesWizard.IsEditing = False Then
                    ' Verificamos los datos de la grid
                    Dim tb As DataTable = Me.EmployeesData
                    Dim MsgParams As New Generic.List(Of String)
                    For Each oRow As DataRow In tb.Rows

                        If Me.cnIdentifyMethods.chkCard_ AndAlso (IsDBNull(oRow("IDCard")) OrElse Val(oRow("IDCard")) <= 0) Then
                            strMsg = Me.Language.Translate("CheckPage.InvalidCard", Me.DefaultScope)
                        ElseIf Me.cnIdentifyMethods.chkCard_ AndAlso API.UserAdminServiceMethods.CredentialExists(Me, oRow("IDCard"), AuthenticationMethod.Card, "", Nothing) Then
                            MsgParams.Add(oRow("IDCard").ToString)
                            strMsg = Me.Language.Translate("CheckPage.ExistCard", Me.DefaultScope, MsgParams)
                        ElseIf tb.Select("IDCard = " & oRow("IDCard").ToString).Length > 1 Then
                            MsgParams.Add(oRow("IDCard").ToString)
                            strMsg = Me.Language.Translate("CheckPage.RepitedCard", Me.DefaultScope, MsgParams)
                        ElseIf IsDBNull(oRow("IDContract")) OrElse oRow("IDContract") = "" Then
                            strMsg = Me.Language.Translate("CheckPage.InvalidContract", Me.DefaultScope)
                        ElseIf API.ContractsServiceMethods.ExistsContractID(Me, oRow("IDContract")) Then
                            MsgParams.Add(oRow("IDContract").ToString)
                            strMsg = Me.Language.Translate("CheckPage.ExistContract", Me.DefaultScope, MsgParams)
                        ElseIf tb.Select("IDContract = '" & oRow("IDContract").ToString.Replace("'", "''") & "'").Length > 1 Then
                            MsgParams.Add(oRow("IDContract").ToString)
                            strMsg = Me.Language.Translate("CheckPage.RepitedContract", Me.DefaultScope, MsgParams)
                        ElseIf IsDBNull(oRow("EmployeeName")) OrElse oRow("EmployeeName").ToString.Trim = "" Then
                            strMsg = Me.Language.Translate("CheckPage.RequiredEmployeeName", Me.DefaultScope)

                            'desactivado:evitar comprobacion de empleado con nombre duplicado
                            'ElseIf API.EmployeeServiceMethods.GetIdEmployeeByName(Me, oRow("EmployeeName").ToString.Trim) > 0 Then
                            'MsgParams.Add(oRow("EmployeeName"))
                            'strMsg = Me.Language.Translate("CheckPage.ExistEmployeeName", Me.DefaultScope, MsgParams)

                        ElseIf tb.Select("EmployeeName = '" & oRow("EmployeeName").ToString.Replace("'", "''") & "'").Length > 1 Then
                            MsgParams.Add(oRow("EmployeeName").ToString)
                            strMsg = Me.Language.Translate("CheckPage.RepitedEmployeeName", Me.DefaultScope, MsgParams)
                        End If

                        If strMsg <> "" Then bolRet = False
                        If Not bolRet Then Exit For

                    Next
                    Me.lblStep6Error.Text = strMsg
                Else
                    strMsg = Me.Language.Translate("CheckPage.AcceptChanges", Me.DefaultScope)
                    Me.lblStep6Error.Text = strMsg
                    bolRet = False
                End If

            Case Wizards_NewMultiEmployeeWizard.Frame.frmType
                Me.cnEmployeeType.RetrieveData(oEmployeeDefault)
                If oEmployeeDefault.Type = "J" Then
                    If Not ChekMaxEmployeesTask() Then
                        Dim Params As New Generic.List(Of String)
                        Params.Add(FormatDateTime(Me.txtBeginContract.Date, DateFormat.ShortDate))
                        strMsg = Me.Language.Translate("MaximumEmployeeTaskReached.Message", Me.DefaultScope, Params)
                    End If
                    If strMsg <> "" Then bolRet = False
                    Me.lblStep7Error.Text = strMsg
                End If

            Case Wizards_NewMultiEmployeeWizard.Frame.frmLocation

                'Hemos de tener algún passport seleccionado
                If Not Me.chkParentGroupNull.Checked Then
                    If Me.hdnPassportSelected.Value = "" OrElse
                                                      API.UserAdminServiceMethods.GetPassport(Me, Me.hdnPassportSelected.Value, LoadType.Passport) Is Nothing Then
                        strMsg = Me.Language.Translate("CheckPage.IncorrectPassportGroupSelected", Me.DefaultScope)
                    End If
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep8Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)
        Dim oPermission = GetFeaturePermission("Employees.Type")
        Select Case oOldFrame
            'Case Wizards_NewMultiEmployeeWizard.Frame.frmGroup

            Case Wizards_NewMultiEmployeeWizard.Frame.frmNumEmployees

            Case Wizards_NewMultiEmployeeWizard.Frame.frmContractDate

            Case Wizards_NewMultiEmployeeWizard.Frame.frmContractNumber
                If HelperSession.AdvancedParametersCache("VTLive.Edition").ToString.ToLower = roServerLicense.roVisualTimeEdition.Starter.ToString.ToLower Then
                    CreateColumnsEmployeesWizard()
                    Me.LoadEmployeesGrid()
                End If
            Case Wizards_NewMultiEmployeeWizard.Frame.frmIdentifyMethods
                Dim oFrames As Generic.List(Of Frame) = Me.Frames

                oFrames.Clear()
                If Me.cnIdentifyMethods.chkCard_ Then
                    oFrames.Add(Frame.frmWelcome)
                    'oFrames.Add(Frame.frmGroup)
                    oFrames.Add(Frame.frmNumEmployees)
                    oFrames.Add(Frame.frmContractDate)
                    oFrames.Add(Frame.frmContractNumber)
                    oFrames.Add(Frame.frmIdentifyMethods)
                    oFrames.Add(Frame.frmCardNumbers)
                    oFrames.Add(Frame.frmEditData)
                    If (oPermission >= Permission.Write) Then oFrames.Add(Frame.frmType)
                    oFrames.Add(Frame.frmLocation)
                    If oActiveFrame > oOldFrame Then
                        Me.oActiveFrame = Frame.frmCardNumbers
                    Else
                        Me.oActiveFrame = Frame.frmContractNumber
                    End If
                Else
                    oFrames.Add(Frame.frmWelcome)
                    'oFrames.Add(Frame.frmGroup)
                    oFrames.Add(Frame.frmNumEmployees)
                    oFrames.Add(Frame.frmContractDate)
                    oFrames.Add(Frame.frmContractNumber)
                    oFrames.Add(Frame.frmIdentifyMethods)
                    oFrames.Add(Frame.frmEditData)
                    If (oPermission >= Permission.Write) Then oFrames.Add(Frame.frmType)
                    oFrames.Add(Frame.frmLocation)
                    If oActiveFrame > oOldFrame Then
                        Me.oActiveFrame = Frame.frmEditData
                    Else
                        Me.oActiveFrame = Frame.frmContractNumber
                    End If
                    CreateColumnsEmployeesWizard()
                    Me.LoadEmployeesGrid()
                End If

                oActiveFrame = Me.oActiveFrame

                Me.Frames = oFrames

            Case Wizards_NewMultiEmployeeWizard.Frame.frmCardNumbers
                CreateColumnsEmployeesWizard()
                Me.LoadEmployeesGrid()

            Case Wizards_NewMultiEmployeeWizard.Frame.frmEditData

            Case Wizards_NewMultiEmployeeWizard.Frame.frmType
                Me.cnEmployeeType.RetrieveData(oEmployeeDefault)

            Case Wizards_NewMultiEmployeeWizard.Frame.frmLocation
                ' Desactivar el iframe del selector de grupos
                Me.ifPassportSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifPassportSelector.Disabled = True

        End Select

        Select Case oActiveFrame
            'Case Wizards_NewMultiEmployeeWizard.Frame.frmGroup

            Case Wizards_NewMultiEmployeeWizard.Frame.frmNumEmployees
                txtNumberEmployees.Focus()
            Case Wizards_NewMultiEmployeeWizard.Frame.frmContractDate
                txtBeginContract.Focus()
            Case Wizards_NewMultiEmployeeWizard.Frame.frmContractNumber

            Case Wizards_NewMultiEmployeeWizard.Frame.frmIdentifyMethods

            Case Wizards_NewMultiEmployeeWizard.Frame.frmCardNumbers

            Case Wizards_NewMultiEmployeeWizard.Frame.frmEditData
                ' Generar la grid de edición

                BindgridEmployeesWizard(False)

            Case Wizards_NewMultiEmployeeWizard.Frame.frmType
                If oOldFrame <> Frame.frmType Then
                    Me.cnEmployeeType.LoadData(oEmployeeDefault)
                End If

            Case Frame.frmLocation

                'HelperWeb.roSelector_SetSelection(Me.IDPassportWizard(), "/source/" & Me.IDPassportWizard(), "roChildSelectorW_treePassportsNewMultiEmployeeWizard")
                Dim strRelative As String = "~/Base/WebUserControls/roWizardSelectorContainer.aspx?TreesEnabled=100&TreesMultiSelect=000&TreesOnlyGroups=100&" &
                                            "TreeFunction=parent.PassportEmployeeSelected&FilterFloat=false&" &
                                            "PrefixTree=treePassportsNewMultiEmployeeWizard&FiltersVisible=0000&AdvancedFilterVisible=false&" &
                                            "Filter1Class=icoFilterPassports1&Filter1LanguageKey=ttFilterPassports1&" &
                                            "TreeImagePath=images/PassportSelector&TreeSelectorPage=../../Security/PassportSelectorData.aspx"
                Me.ifPassportSelector.Attributes("src") = Me.ResolveUrl(strRelative)
                Me.ifPassportSelector.Disabled = False
                If chkParentGroupNull.Checked = True Then
                    Me.ifPassportSelector.Style("display") = "none"
                Else
                    Me.ifPassportSelector.Style("display") = ""
                End If
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

        If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 And Me.FramePos(oActiveFrame) = 0 Then
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

    Private Sub LoadEmployeesGrid()

        Dim tb As DataTable = Me.EmployeesData

        tb.Rows.Clear()

        ' Obtenemos el número de contrato inicial
        Dim lngContract As ULong
        If Me.optNumberContractLast.Checked Then
            lngContract = API.ContractsServiceMethods.GetMaxIDContract(Me) + 1
        Else
            lngContract = CLng(Me.txtNumberContract.Text)
        End If

        ' Obtenemos el número de targeta inicial
        Dim lngCard As Long
        If Me.optCardsFromLast.Checked Then
            lngCard = API.UserAdminServiceMethods.MaxCredentialvalue(Me, AuthenticationMethod.Card, Nothing) + 1
        ElseIf Me.optCardsContract.Checked Then
            lngCard = lngContract
        Else
            lngCard = CLng(Me.txtCardsFromNumber.Text)
        End If

        Dim oRow As DataRow
        For n As Integer = 0 To CInt(Me.txtNumberEmployees.Text) - 1

            oRow = tb.NewRow
            oRow("ID") = n + 1
            oRow("EmployeeName") = Me.Language.Keyword("Employee") & " " & lngContract.ToString
            oRow("IDContract") = lngContract
            oRow("IDCard") = lngCard

            lngContract += 1
            lngCard += 1

            tb.Rows.Add(oRow)

        Next

        Me.EmployeesData = tb
        BindgridEmployeesWizard(False)

        ' Cargamos control de tipo de empleado
        Me.cnEmployeeType.LoadData(oEmployeeDefault)

        If Not Me.cnEmployeeType.chkAttendanceEnabled And Not Me.cnEmployeeType.chkAccessEnabled And Not Me.cnEmployeeType.chkJobEnabled And Not Me.cnEmployeeType.chkExternsEnabled And Not Me.cnEmployeeType.chkPreventionEnabled Then

            If Me.Frames.Contains(Frame.frmType) Then
                Me.Frames.Remove(Frame.frmType)
            End If
            ''oActiveFrame = Me.Frames(Me.FramePos(Me.oActiveFrame) + 1)

        End If

        ' En el caso que sea seguridad modo v2 no visualziamos el frame de asignar a grupo de usuario
        If Me.Frames.Contains(Frame.frmLocation) Then
            Me.Frames.Remove(Frame.frmLocation)
        End If

    End Sub

    Public Sub LabAgreeVisible()
        gBox1.Visible = True
        cmbLabAgree.Visible = True
        lblLabAgree.Visible = True
    End Sub

#End Region

#Region "GridEmployees"

    Private Sub CreateColumnsEmployeesWizard()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim VisibleIndex As Integer = 0

        'general grid settings
        Me.gridEmployeesWizard.Columns.Clear()
        Me.gridEmployeesWizard.KeyFieldName = "ID"
        Me.gridEmployeesWizard.SettingsText.EmptyDataRow = " "
        Me.gridEmployeesWizard.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        'Edit button

        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.Name = "Edit"
        GridColumnCommand.ShowEditButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 30
        Me.gridEmployeesWizard.Columns.Add(GridColumnCommand)

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("ID.Text", Me.DefaultScope)
        GridColumn.FieldName = "ID"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False 'False
        GridColumn.Width = 40
        Me.gridEmployeesWizard.Columns.Add(GridColumn)

        'Tarjeta
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("Tarjeta.Text", Me.DefaultScope)
        GridColumn.FieldName = "IDCard"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.Visible = Me.cnIdentifyMethods.chkCard_
        GridColumn.ReadOnly = False
        Me.gridEmployeesWizard.Columns.Add(GridColumn)

        'Contrato
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("Contrato.Text", Me.DefaultScope)
        GridColumn.FieldName = "IDContract"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        Me.gridEmployeesWizard.Columns.Add(GridColumn)

        'Nombre
        GridColumn = New GridViewDataTextColumn
        GridColumn.Caption = Me.Language.Translate("Nombre.Text", Me.DefaultScope)
        GridColumn.FieldName = "EmployeeName"
        GridColumn.PropertiesTextEdit.MaxLength = 500
        GridColumn.PropertiesTextEdit.ValidationSettings.RequiredField.IsRequired = True
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = False
        Me.gridEmployeesWizard.Columns.Add(GridColumn)

        'Cancel button

        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowCancelButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.Name = "Cancel"
        GridColumnCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = 30
        Me.gridEmployeesWizard.Columns.Add(GridColumnCommand)

        Me.gridEmployeesWizard.KeyFieldName = "ID"

    End Sub

    Private Sub BindgridEmployeesWizard(ByVal bolReload As Boolean)
        Me.gridEmployeesWizard.DataSource = Me.EmployeesData(bolReload)
        Me.gridEmployeesWizard.KeyFieldName = "ID"
        Me.gridEmployeesWizard.DataBind()

    End Sub

    Protected Sub gridEmployeesWizard_StartRowEditing(sender As Object, e As Data.ASPxStartRowEditingEventArgs) Handles gridEmployeesWizard.StartRowEditing

    End Sub

    Protected Sub gridEmployeesWizard_RowUpdating(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataUpdatingEventArgs) Handles gridEmployeesWizard.RowUpdating

        Dim tb As DataTable = Session("NewMultiEmployeeWizard_EmployeesData")
        Dim dr As DataRow = tb.Rows.Find(e.Keys(gridEmployeesWizard.KeyFieldName))
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)

        Dim enumerator As IDictionaryEnumerator = e.NewValues.GetEnumerator()
        enumerator.Reset()
        While enumerator.MoveNext()
            Dim currentkey As String = ""
            If enumerator.Key IsNot Nothing Then currentkey = enumerator.Key.ToString()
            Select Case currentkey
                Case "ID"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("ID") = roTypes.Any2String(enumerator.Value).Trim
                    Else
                        dr.Item("ID") = String.Empty
                    End If

                Case "IDCard"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDCard") = enumerator.Value
                    Else
                        dr.Item("IDCard") = String.Empty
                    End If

                Case "IDContract"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("IDContract") = enumerator.Value
                    Else
                        dr.Item("IDContract") = String.Empty
                    End If
                Case "EmployeeName"
                    If enumerator.Value IsNot Nothing Then
                        dr.Item("EmployeeName") = enumerator.Value
                    Else
                        dr.Item("EmployeeName") = String.Empty
                    End If

            End Select

        End While

        Me.EmployeesData = tb
        'ViewState("NewMultiEmployeeWizard_EmployeesData") = tb
        e.Cancel = True
        grid.CancelEdit()
        'grid.UpdateEdit()

    End Sub

#End Region

End Class