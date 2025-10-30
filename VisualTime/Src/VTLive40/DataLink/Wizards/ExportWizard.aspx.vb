Imports Robotics.Base.DTOs
Imports Robotics.Base.VTDataLink.DataLink
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Wizards_ExportWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome
        frmEmployeesSelector
        frmSchedule
        frmFinish
    End Enum

    Private oActiveFrame As Frame
    Private strExportName As String = ""
    Private wSepChar As String = Chr(94) & Chr(124) & Chr(94)

    Public Property FeatureID() As String
        Get
            Dim strFeature As String = String.Empty

            If Session("ExportWizard_FeatureData") IsNot Nothing Then
                strFeature = roTypes.Any2String(Session("ExportWizard_FeatureData"))
            End If

            If strFeature = String.Empty Then strFeature = "Employees"

            Return strFeature
        End Get
        Set(ByVal value As String)
            Session("ExportWizard_FeatureData") = value
        End Set
    End Property

#End Region

#Region "Properties"

    Private ReadOnly Property CanExport() As Boolean
        Get
            Return True 'API.DataLinkServiceMethods.CanExecuteExcelDataTemplate(Me.Page)
        End Get
    End Property

    Private Property Frames() As Generic.List(Of Frame)
        Get

            Dim oFrames As New Generic.List(Of Frame)
            If Me.hdnFrames.Value = "" Then

                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmEmployeesSelector)
                oFrames.Add(Frame.frmSchedule)
                oFrames.Add(Frame.frmFinish)
            Else

                For Each strItem As String In Me.hdnFrames.Value.Split("*")
                    oFrames.Add(strItem)
                Next

            End If

            Return oFrames

        End Get
        Set(ByVal value As Generic.List(Of Frame))
            Me.hdnFrames.Value = ""
            Me.hdnFramesOnlyClient.Value = ""
            If value IsNot Nothing Then
                For Each oItem As Frame In value
                    Me.hdnFrames.Value &= "*" & oItem
                    Select Case oItem
                        Case Frame.frmWelcome
                            Me.hdnFramesOnlyClient.Value &= "*1"
                        Case Else
                            Me.hdnFramesOnlyClient.Value &= "*0"
                    End Select
                Next
                If Me.hdnFrames.Value <> "" Then Me.hdnFrames.Value = Me.hdnFrames.Value.Substring(1)
                If Me.hdnFramesOnlyClient.Value <> "" Then Me.hdnFramesOnlyClient.Value = Me.hdnFramesOnlyClient.Value.Substring(1)
            End If
        End Set
    End Property

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("DatalinkEW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("DatalinkEW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("DatalinkEW_iCurrentTask") = value
        End Set
    End Property

    Private Property ErrorExists As Boolean
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorExists")
            If val IsNot Nothing Then
                Return roTypes.Any2Boolean(val)
            Else
                HttpContext.Current.Session("ErrorExists") = False
                Return False
            End If
        End Get
        Set(value As Boolean)
            HttpContext.Current.Session("ErrorExists") = value
        End Set
    End Property

    Private Property ErrorDescription As String
        Get
            Dim val As Object = HttpContext.Current.Session("ErrorDescription")
            If val IsNot Nothing Then
                Return roTypes.Any2String(val)
            Else
                HttpContext.Current.Session("ErrorDescription") = False
                Return False
            End If
        End Get
        Set(value As String)
            HttpContext.Current.Session("ErrorDescription") = value
        End Set
    End Property

#End Region

#Region "Events"
    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roTreeState", "~/Base/Scripts/rocontrols/roTrees/roTreeState.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Employees.DataLink", Permission.Read) AndAlso
            Not Me.HasFeaturePermission("Calendar.DataLink", Permission.Read) AndAlso
            Not Me.HasFeaturePermission("Tasks.DataLink", Permission.Read) AndAlso
            Not Me.HasFeaturePermission("BusinessCenters.DataLink", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Return
        End If

        Server.ScriptTimeout = 10000

        If roTypes.Any2String(Request("action")) <> "ExecuteExport" Then
            Me.hdnConcept.Value = roTypes.Any2String(Request("Concept")).Trim
            Me.FeatureID = "Employees"

            If Not Me.IsPostBack Then
                If Request("isBusiness") IsNot Nothing AndAlso Request("isBusiness") <> "" Then
                    hdnIsBusiness.Value = roTypes.Any2String(Request("isBusiness"))
                End If
                Dim oConfParameters As String() = Nothing

                If Me.hdnConcept.Value <> String.Empty Then
                    Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), Me.hdnConcept.Value), roDatalinkConcept)
                    Dim guide As roDatalinkGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me.Page, eConcept, False)

                    Me.hdnApplyCloseDate.Value = If((guide.Concept = roDatalinkConcept.Accruals), "1", "0")
                    Me.hdnIDExport.Value = guide.Export.Id
                    Me.hdnProfileType.Value = guide.Export.ProfileType
                    Me.strExportName = guide.Name
                    Me.hdnOnlyProfile.Value = False
                    Me.hdnExcelTemplateFile.Value = guide.Concept.ToString
                    Me.hdnConceptProfile.Value = CInt(eConcept)

                    If eConcept <> roDatalinkConcept.Schedule AndAlso eConcept <> roDatalinkConcept.Requests AndAlso eConcept <> roDatalinkConcept.CustomArgal AndAlso eConcept <> roDatalinkConcept.ZCustom AndAlso eConcept <> roDatalinkConcept.Iberper Then
                        cmbFormatExport.Items.Clear()
                        Dim exportFormats = System.Enum.GetValues(GetType(roDatalinkFormat))
                        For Each ef In exportFormats
                            cmbFormatExport.Items.Add(System.Enum.GetName(GetType(roDatalinkFormat), ef), roTypes.Any2Integer(ef) + 1) ' le sumamos 1 porqué 1: EXCEL 2:ASCII
                        Next

                        If eConcept <> 6 Then
                            formatExportTr.Style("display") = ""
                            'formatExportTr.Visible = True
                        Else
                            formatExportTr.Style("display") = "none"
                            'formatExportTr.Visible = False
                        End If
                    Else
                        formatExportTr.Style("display") = "none"
                        'formatExportTr.Visible = False
                    End If

                    cmbProfilesExport.Items.Clear()
                    For Each oTemplate In guide.Export.Templates
                        cmbProfilesExport.Items.Add(oTemplate.Name, oTemplate.TemplateFile & "@" & oTemplate.ID & "*" & IIf(eConcept = roDatalinkConcept.Accruals, "1", "0"))
                    Next

                    Me.hdnFileType.Value = "1" 'Excel
                    Me.hdnExcelInstalled.Value = "1" ' No nos hace falta el excel instalado

                    Me.txtExportSeparator.Text = guide.Export.DefaultSeparator

                    Me.chk2007Version.Checked = True
                    Me.chk2007Version.Visible = False
                    Me.lblVersionExcel.Visible = False

                    If guide.Export.FeatureAliasID <> String.Empty Then Me.FeatureID = guide.Export.FeatureAliasID
                Else
                    Me.hdnIDExport.Value = roTypes.Any2String(Request("IDExport"))
                    formatExportTr.Style("display") = "none"
                    'formatExportTr.Visible = False
                    Dim oExport As roExportGuide = Nothing
                    If Me.hdnIDExport.Value >= 8100 Then
                        oExport = API.DataLinkServiceMethods.GetExportGuide(Me.Page, Me.hdnIDExport.Value, False)
                        If oExport IsNot Nothing AndAlso oExport.FeatureAliasID <> String.Empty Then Me.FeatureID = oExport.FeatureAliasID
                    End If

                    If oExport IsNot Nothing AndAlso Me.hdnIDExport.Value >= 9004 AndAlso oExport.EmployeeFilter <> String.Empty Then
                        oConfParameters = oExport.EmployeeFilter.Split("@")
                    End If

                    If Me.hdnIDExport.Value >= 9004 Then
                        ' Lee las plantillas disponibles
                        Dim Profiles As DataTable = API.DataLinkServiceMethods.GetTemplatesByProfileMask(Me, roTypes.Any2String(Request("ProfileMask")))

                        cmbProfilesExport.Items.Clear()
                        If Profiles IsNot Nothing Then
                            For Each row In Profiles.Rows
                                cmbProfilesExport.Items.Add(row("Description"), row("Name") & "*" & IIf(roTypes.Any2Boolean(row("NeedTemplate")) = True, "1", "0"))
                            Next
                        End If
                    End If

                    If oExport IsNot Nothing Then
                        Me.hdnApplyCloseDate.Value = If(oExport.ProfileType = "1", "1", "0")
                    End If

                    Me.hdnFileType.Value = roTypes.Any2String(Request("FileType"))
                    Me.hdnExcelTemplateFile.Value = roTypes.Any2String(Request("ExcelTemplateFile")) 'si ExcelTemplateFile <> "" --> se utiliza plantilla excel
                    Me.hdnExcelInstalled.Value = roTypes.Any2String(Request("ExcelInstalled"))
                    Me.hdnProfileType.Value = roTypes.Any2String(Request("ProfileType"))
                    Me.strExportName = roTypes.Any2String(Request("ExportName"))
                    Me.hdnOnlyProfile.Value = roTypes.Any2Boolean(Request("OnlyProfile"))
                End If

                If Me.HasFeaturePermission(Me.FeatureID, Permission.Read) Then
                    Me.Frames = Nothing
                    Me.Frames = Me.Frames

                    Me.SetStepTitles()

                    'Inicializamos el selector de empleados
                    HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpExportWizard")
                    HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpExportWizardGrid")

                    Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                    Me.ifEmployeesSelector.Disabled = True

                    Me.txtScheduleBeginDate.Date = Now.Date
                    Me.txtScheduleEndDate.Date = Now.Date

                    Me.oActiveFrame = Frame.frmWelcome

                    ' Solicita el tipo de excel
                    'If Integer.Parse(Me.hdnIDExport.Value) >= 9000 And Integer.Parse(Me.hdnIDExport.Value) <= 9003 Then
                    If hdnFileType.Value = "1" Then
                        tbExcelVersion.Visible = True
                    Else
                        tbExcelVersion.Visible = False
                    End If

                    If Me.hdnApplyCloseDate.Value = "1" Then
                        Me.trApplyLockDate.Style("display") = ""
                    Else
                        Me.trApplyLockDate.Style("display") = "none"
                        Me.ckApplyLockDate.Checked = False
                    End If

                    trSeparator.Visible = hdnIsBusiness.Value = "1"
                    trSeparator.Style("display") = "none"

                    ' Solicita el grupo de saldos
                    ' If Integer.Parse(Me.hdnProfileType.Value) = 1 Then
                    If Me.hdnProfileType.Value = "1" Then
                        Me.lblConceptsGroups.Visible = True
                        Me.cmbConceptsGroups.Visible = True

                        ' Lee log grupos de saldos
                        Dim dTbl As DataTable = API.ConceptsServiceMethods.GetConceptsGroups(Me.Page, True)
                        Me.cmbConceptsGroups.Items.Clear()
                        For Each row As DataRow In dTbl.Rows
                            Me.cmbConceptsGroups.Items.Add(row("Name"), row("id"))
                        Next
                    Else
                        Me.lblConceptsGroups.Visible = False
                        Me.cmbConceptsGroups.Visible = False
                    End If

                    ' Solicita Plantilla y caracter separador
                    If Me.hdnIDExport.Value >= 9004 Then
                        If oConfParameters IsNot Nothing AndAlso oConfParameters.Length >= 4 Then
                            Me.hdnEmployeesSelected.Value = HttpUtility.UrlEncode(oConfParameters(0))
                            Me.hdnFilter.Value = HttpUtility.UrlEncode(oConfParameters(3))
                            Me.hdnFilterUser.Value = HttpUtility.UrlEncode(oConfParameters(4))

                            HelperWeb.roSelector_SetSelection(oConfParameters(0), "", "objContainerTreeV3_treeEmpExportWizard", oConfParameters(3), oConfParameters(4))
                            HelperWeb.roSelector_SetSelection(oConfParameters(0), "", "objContainerTreeV3_treeEmpExportWizardGrid", oConfParameters(3), oConfParameters(4))
                        End If
                    Else
                        Me.lblProfileName.Visible = False
                        Me.cmbProfilesExport.Visible = False
                    End If
                Else

                    WLHelperWeb.RedirectAccessDenied(True)

                End If
            Else

                Me.oActiveFrame = Me.FrameByIndex(Me.hdnActiveFrame.Value)
                Dim oDiv As HtmlControl
                For n As Integer = 0 To System.Enum.GetValues(GetType(Frame)).Length - 1
                    oDiv = HelperWeb.GetControl(Me.Controls, "divStep" & n.ToString)
                    If n = Me.FrameIndex(Me.oActiveFrame) Then
                        oDiv.Style("display") = "block"
                    Else
                        oDiv.Style("display") = "none"
                    End If
                Next

            End If

        End If

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

    Protected Sub btResume_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btResume.Click
        CloseWizard()
    End Sub

    Private Sub CloseWizard()
        Try

            If Not ErrorExists Then
                Me.MustRefresh = "1"
                Me.lblWelcome1.Text = Me.Language.Translate("End.EmployeeCopyWelcome1.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = String.Empty

                'Obtener ruta de la plantilla de excel
                Dim strTemplateFileName As String = roTypes.Any2String(HttpContext.Current.Session("ExportFileName"))
                If strTemplateFileName.Length > 0 Then
                    Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeCopyWelcome2.Text", Me.DefaultScope)

                    'COMPROBAR FISICAMENTE SI SE HA CREADO EL FICHERO
                    If System.IO.File.Exists(strTemplateFileName) Then
                        Me.lblWelcome3.Text = Me.Language.Translate("End.Ok.EmployeeCopyWelcome3.Text", Me.DefaultScope) & " " & strTemplateFileName
                    Else
                        Me.lblWelcome3.Text = String.Empty
                    End If

                    Me.btnDownload.Visible = True
                Else
                    If Me.hdnProfileType.Value <> "999" Then
                        Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeCopyWelcome2Empty.Text", Me.DefaultScope)
                    Else
                        Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeCopyWelcome2.Text", Me.DefaultScope)
                    End If
                End If
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.EmployeeCopyWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Visible = False
                ' Mostramos los errores en pantalla
                Me.txtErrors.Visible = True
                Me.txtErrors.InnerHtml = ErrorDescription
            End If

            ErrorDescription = Nothing
            ErrorExists = Nothing
            iCurrentTask = Nothing

            Me.btClose.Text = Me.Language.Keyword("Button.Close")
            Me.FrameChange(Me.oActiveFrame, Frame.frmWelcome)
        Catch ex As Exception
            Response.Write(ex.Message.ToString & " " & ex.StackTrace & " ")
        End Try
    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.ToUpper()
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckFrame(Me.oActiveFrame))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.ExecuteExport()
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case roLiveTaskStatus.All, roLiveTaskStatus.Stopped
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                            Case roLiveTaskStatus.Running
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                                HttpContext.Current.Session("ExportFileName") = oTask.ErrorCode
                            Case roLiveTaskStatus.Finished
                                PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "KO")
                                iCurrentTask = -1
                                ErrorExists = True
                                ErrorDescription = oTask.ErrorCode
                        End Select
                    Else
                        iCurrentTask = -1
                        ErrorExists = True
                        ErrorDescription = roWsUserManagement.SessionObject.States.LiveTaskState.ErrorText
                        PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    End If
                Else
                    iCurrentTask = -1
                    ErrorExists = True
                    ErrorDescription = Me.Language.Translate("Error.CouldNotRetrieveTask.Text", Me.DefaultScope)
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                End If
            Case "SAVE_PROFILE"
                PerformActionCallback.JSProperties.Add("cpAction", "SAVE_PROFILE")
                Dim oExport As roExportGuide = API.DataLinkServiceMethods.GetExportGuide(Me.Page, roTypes.Any2Integer(Me.hdnIDExport.Value), False)
                If oExport IsNot Nothing Then
                    If Me.hdnEmployeesSelected.Value = String.Empty Then
                        PerformActionCallback.JSProperties.Add("cpActionResult", Me.Language.Translate("", Me.DefaultScope))
                    Else
                        Dim filterEmp As String = HttpUtility.UrlDecode(Me.hdnEmployeesSelected.Value) & "@" & WLHelperWeb.CurrentPassportID & "@" & "Employees" & "@" & HttpUtility.UrlDecode(Me.hdnFilter.Value) & "@" & HttpUtility.UrlDecode(Me.hdnFilterUser.Value)
                        oExport.EmployeeFilter = filterEmp
                        API.DataLinkServiceMethods.SaveExportGuide(Me.Page, oExport, False)
                        PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                    End If

                End If

        End Select

    End Sub

    Private Function ExecuteExport() As Integer
        Dim iTask As Integer = -1

        Try
            Try

                Dim oExport As roExportGuide = API.DataLinkServiceMethods.GetExportGuide(Me.Page, roTypes.Any2Integer(Me.hdnIDExport.Value), False)
                Dim employeesSelected As String = HttpUtility.UrlDecode(Me.hdnEmployeesSelected.Value) & "@" & "Employees" & "@" & HttpUtility.UrlDecode(Me.hdnFilter.Value) & "@" & HttpUtility.UrlDecode(Me.hdnFilterUser.Value)

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)
                Dim ProfileTemplate As String = ""
                Dim ConceptGroup As String = ""
                Dim Separator As String = ""
                Dim idProfileTemplpate As Integer = 0

                lstAuditParameterNames.Add("{ExportName}")
                lstAuditParameterValues.Add(strExportName)

                lstAuditParameterNames.Add("{originDate}")
                lstAuditParameterValues.Add(txtScheduleBeginDate.Date.ToShortDateString)

                lstAuditParameterNames.Add("{destinationDate}")
                lstAuditParameterValues.Add(txtScheduleBeginDate.Date.ToShortDateString)

                If Not IsNothing(cmbConceptsGroups.SelectedItem) Then
                    ConceptGroup = cmbConceptsGroups.SelectedItem.Value
                    lstAuditParameterNames.Add("{ConceptsGroup}")
                    lstAuditParameterValues.Add(cmbConceptsGroups.SelectedItem.Text)
                End If

                ProfileTemplate = Me.hdnExcelTemplateFile.Value
                If Not IsNothing(cmbProfilesExport.SelectedItem) Then ProfileTemplate = roTypes.Any2String(cmbProfilesExport.SelectedItem.Value).Split("*")(0)
                If Not IsNothing(cmbFormatExport.SelectedItem) Then
                    Me.hdnFileType.Value = roTypes.Any2String(cmbFormatExport.SelectedItem.Value)
                End If

                If ProfileTemplate.Split("@").Count = 2 Then
                    idProfileTemplpate = roTypes.Any2Integer(ProfileTemplate.Split("@")(1))
                    ProfileTemplate = ProfileTemplate.Split("@")(0)
                End If

                If hdnIsBusiness.Value = "1" Then
                    If Not IsNothing(txtExportSeparator.Value) Then Separator = txtExportSeparator.Value
                Else
                    Separator = oExport.Separator
                End If

                iTask = API.LiveTasksServiceMethods.ExecuteExportInBackground(Me.Page, roTypes.Any2Integer(Me.hdnIDExport.Value), idProfileTemplpate, Me.hdnFileType.Value, employeesSelected, IIf(Me.hdn2007Version.Value = "0", False, True) _
                                                                                           , txtScheduleBeginDate.Date, txtScheduleEndDate.Date, IIf(Me.hdnExcelInstalled.Value = "0", False, True), ProfileTemplate, ConceptGroup, Separator _
                                                                                           , roTypes.Any2Integer(Me.hdnProfileType.Value), Me.ckApplyLockDate.Checked)

                If iTask >= 0 Then
                    Try
                        lstAuditParameterNames.Add("{iTask}")
                        lstAuditParameterValues.Add(iTask)

                        API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tDataLink, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
                    Catch ex As Exception
                    End Try
                End If
            Catch ex As Exception
            End Try
        Catch ex As Exception
        End Try

        Return iTask
    End Function

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

        Dim strMsg As String = ""
        Dim bolRet As Boolean = True

        Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), Me.hdnConcept.Value), roDatalinkConcept)
        Dim formatNeeded As Boolean = False
        If eConcept <> roDatalinkConcept.Schedule AndAlso eConcept <> roDatalinkConcept.Requests AndAlso eConcept <> roDatalinkConcept.CustomArgal AndAlso eConcept <> roDatalinkConcept.ZCustom AndAlso eConcept <> roDatalinkConcept.Iberper Then

            If eConcept <> roDatalinkConcept.Absences Then
                formatNeeded = True
            Else

                If Me.cmbProfilesExport.IsVisible = True And cmbProfilesExport.SelectedItem IsNot Nothing _
                    AndAlso roTypes.Any2String(cmbProfilesExport.SelectedItem.Value).Split("@")(0) <> String.Empty Then
                    formatNeeded = True
                End If

            End If

        End If

        Select Case Frame
            Case Wizards_ExportWizard.Frame.frmEmployeesSelector

                'Comprobar si hay algun empleado seleccionado
                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                    bolRet = False
                End If
                Me.lblStep1Error.Text = strMsg

            Case Wizards_ExportWizard.Frame.frmSchedule
                If Me.txtScheduleBeginDate.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.BeginCopyScheduleIncorrect", Me.DefaultScope)
                ElseIf Me.txtScheduleEndDate.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.EndCopyScheduleIncorrect", Me.DefaultScope)
                ElseIf Me.txtScheduleBeginDate.Date > Me.txtScheduleEndDate.Date Then
                    If cmbProfilesExport.SelectedItem.Value.ToString.Contains("AtDate") Then
                        Me.txtScheduleBeginDate.Date = Me.txtScheduleEndDate.Date
                    Else
                        strMsg = Me.Language.Translate("CheckPage.CopySchedulePeriodIncorrect", Me.DefaultScope)
                    End If
                ElseIf Me.cmbProfilesExport.IsVisible = True And IsNothing(cmbProfilesExport.SelectedItem) Then
                    strMsg = Me.Language.Translate("CheckPage.ProfileIncorrect", Me.DefaultScope)
                ElseIf formatNeeded AndAlso cmbProfilesExport.SelectedItem Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.FormatIncorrect", Me.DefaultScope)
                ElseIf Me.cmbConceptsGroups.IsVisible = True And IsNothing(cmbConceptsGroups.SelectedItem) Then

                    If roTypes.Any2String(cmbProfilesExport.SelectedItem.Value).Split("*")(1) = "1" Then
                        strMsg = Me.Language.Translate("CheckPage.ConceptGroupIncorrect", Me.DefaultScope)
                    End If
                Else
                    If HelperSession.AdvancedParametersCache("CheckExportPeriod").Equals("1") AndAlso roTypes.Any2Integer(Me.hdnIDExport.Value.Trim) = 10032 AndAlso roTypes.Any2Integer(Me.hdnOverwritePeriod.Value) = 0 Then
                        'strMsg = "Verificamos si ya se ha exportado el periodo indicado"
                        Dim bolExistExportPeriod As Boolean = API.DataLinkServiceMethods.ExistsExportPeriod(Me, txtScheduleBeginDate.Date, txtScheduleEndDate.Date)

                        If bolExistExportPeriod Then
                            Me.hdnAskOverwritePeriodConfirmation.Value = "1"
                            bolRet = False
                        End If
                    Else
                        Me.hdnAskOverwritePeriodConfirmation.Value = "0"
                        Me.hdnOverwritePeriod.Value = "0"
                    End If
                End If
                ' Si la importación es ASCII formateada no lleva caracter separador
                'If Me.txtExportSeparator.IsVisible = True And Me.txtExportSeparator.Value = "" Then
                ' strMsg = Me.Language.Translate("CheckPage.SeparatorIncorrect", Me.DefaultScope)
                ' End If

                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            Case Frame.frmEmployeesSelector
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

            Case Frame.frmSchedule
                ' Guardamos las fechas en los campos ocultos con formato yyyy/MM/dd para no tener problemas con el formato de fecha actual.
                Me.hdn2007Version.Value = IIf(chk2007Version.Checked, "1", "0")

        End Select

        Select Case oActiveFrame
            Case Frame.frmEmployeesSelector
                If CanExport Then
                    Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                           "PrefixTree=treeEmpExportWizard&FeatureAlias=" & Me.FeatureID & "&PrefixCookie=objContainerTreeV3_treeEmpExportWizardGrid&" &
                                           "AfterSelectFuncion=parent.GetSelectedTreeV3"
                    Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)
                    Me.ifEmployeesSelector.Disabled = False

                    excelErrorTable.Visible = False
                    newEmployeeSelectorPanel.Visible = True
                Else
                    excelErrorTable.Visible = True
                    newEmployeeSelectorPanel.Visible = False
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

        If roTypes.Any2Boolean(hdnOnlyProfile.Value) Then
            If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 And Me.FramePos(oActiveFrame) = 0 Then
                Me.btPrev.Visible = False '.Style("display") = "none"
                Me.btNext.Visible = False '.Style("display") = "none"
                Me.btEnd.Visible = False '.Style("display") = "none"
                Me.btSaveProfile.Visible = False
            Else
                Me.btNext.Visible = False '.Style("display") = "none"
                Me.btSaveProfile.Visible = True
                Me.btClose.Visible = True
            End If
        Else
            If Me.FramePos(oOldFrame) = Me.Frames.Count - 1 And Me.FramePos(oActiveFrame) = 0 Then
                Me.btPrev.Visible = False '.Style("display") = "none"
                Me.btNext.Visible = False '.Style("display") = "none"
                Me.btEnd.Visible = False '.Style("display") = "none"
                Me.btSaveProfile.Visible = False
            Else
                If CanExport Then
                    Me.btPrev.Visible = IIf(Me.FramePos(oActiveFrame) > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
                    Me.btNext.Visible = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
                Else
                    Me.btPrev.Visible = False '.Style("display") = "none"
                    Me.btNext.Visible = False '.Style("display") = "none"
                End If
                Me.btEnd.Visible = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
                Me.btSaveProfile.Visible = False
            End If
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
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Info")
            If oLabel IsNot Nothing Then oLabel.Text = Me.hdnSetpInfo.Text
        Next

    End Sub

    Private Sub LoadUserFields()
        'Dim oUserFields As Generic.List(Of UserFieldService.roUserField) = API.UserFieldServiceMethods.GetUserFieldList(Me, True)
        'If oUserFields IsNot Nothing Then
        '    Dim oNode As TreeNode
        '    For Each oUserField As UserFieldService.roUserField In oUserFields
        '        oNode = New TreeNode(oUserField.FieldName, oUserField.FieldName, "~/Images/UserField 16.png")
        '        Me.treeUserFields.Nodes.Add(oNode)
        '    Next
        'End If
    End Sub

#End Region

End Class