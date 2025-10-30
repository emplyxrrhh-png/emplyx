Imports Robotics.Base.DTOs
Imports Robotics.Base.VTDataLink.DataLink
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Wizards_ImportWizard
    Inherits PageBase

#Region "Declarations"

    Private Enum Frame
        frmWelcome
        frmSelection
        frmEmployeesSelector
        frmSchedule
        frmFinish
    End Enum

    Public Property FeatureID() As String
        Get
            Dim strFeature As String = String.Empty

            If Session("ImportWizard_FeatureData") IsNot Nothing Then
                strFeature = roTypes.Any2String(Session("ImportWizard_FeatureData"))
            End If

            If strFeature = String.Empty Then strFeature = "Employees"

            Return strFeature
        End Get
        Set(ByVal value As String)
            Session("ImportWizard_FeatureData") = value
        End Set
    End Property

    Private intIDImport As Integer = 0
    Private oActiveFrame As Frame
    Private oCurrentImport As roImportGuide

#End Region

#Region "Properties"

    Private Property Frames() As Generic.List(Of Frame)
        Get

            Dim oFrames As New Generic.List(Of Frame)
            If Me.hdnFrames.Value = "" Then

                oFrames.Add(Frame.frmWelcome)
                oFrames.Add(Frame.frmSelection)
                If Me.intIDImport = 2 OrElse Me.intIDImport = 14 OrElse Me.intIDImport = 21 Then ' Si es la importación de planificación mostramos pantalles de filtro
                    oFrames.Add(Frame.frmEmployeesSelector)
                    oFrames.Add(Frame.frmSchedule)
                End If
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
            Dim val As Object = HttpContext.Current.Session("DatalinkIW_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("DatalinkIW_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("DatalinkIW_iCurrentTask") = value
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

        If Me.HasFeaturePermission("Employees.DataLink", Permission.Write) Or
            Me.HasFeaturePermission("Calendar.DataLink", Permission.Write) Or
            Me.HasFeaturePermission("Tasks.DataLink", Permission.Write) Or
            Me.HasFeaturePermission("BusinessCenters.DataLink", Permission.Write) Then
        Else
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        Server.ScriptTimeout = 10000

        Me.hdnConcept.Value = roTypes.Any2String(Request("Concept")).Trim

        If Request("action") <> "ExecuteImport" Then

            If Me.hdnConcept.Value <> String.Empty Then
                Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), Me.hdnConcept.Value), roDatalinkConcept)
                Dim guide As roDatalinkGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me.Page, eConcept, False)
                Me.oCurrentImport = API.DataLinkServiceMethods.GetImportGuide(Me, guide.Import.Id, False)
                Me.intIDImport = Me.oCurrentImport.ID
            Else
                Me.intIDImport = roTypes.Any2Integer(Request("IDImport"))
                Me.oCurrentImport = API.DataLinkServiceMethods.GetImportGuide(Me, intIDImport, False)
            End If

            Me.hdnIDImport.Value = Me.intIDImport
            If Me.intIDImport = 14 OrElse Me.intIDImport = 21 Then
                Me.imgNewMultiEmployeeWizard.ImageUrl = "~/Base/Images/Wizards/wzmenslong.gif"
            End If

            If Not Me.IsPostBack Then
                If Request("isBusiness") IsNot Nothing AndAlso Request("isBusiness") <> "" Then
                    hdnIsBusiness.Value = roTypes.Any2String(Request("isBusiness"))
                End If
                Me.hdnIDImport.Value = Me.intIDImport
                If Me.intIDImport = 14 OrElse Me.intIDImport = 21 Then
                    Me.imgNewMultiEmployeeWizard.ImageUrl = "~/Base/Images/Wizards/wzmenslong.gif"
                End If

                If Me.intIDImport = 20 Then
                    Me.ifUploads.Style.Add("height", "300px")
                End If

                Me.FeatureID = "Employees"
                    If oCurrentImport IsNot Nothing AndAlso oCurrentImport.FeatureAliasID <> String.Empty Then Me.FeatureID = oCurrentImport.FeatureAliasID

                    Me.SetStepTitles()

                    'Inicializamos el selector de empleados
                    HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmployeesImportWizard")
                    HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmployeesImportWizardGrid")

                    Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                    Me.ifEmployeesSelector.Disabled = True

                    Me.txtScheduleBeginDate.Value = Now.Date
                    Me.txtScheduleEndDate.Value = Now.Date

                    Me.oActiveFrame = Frame.frmWelcome
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
            Me.lblWelcome1.Text = Me.Language.Translate("End.EmployeeImportWelcome1.Text", Me.DefaultScope)

            If Not ErrorExists Then
                Me.MustRefresh = "1"
                Me.lblWelcome2.Text = Me.Language.Translate("End.Ok.EmployeeImportWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Text = Me.Language.Translate("End.Ok.EmployeeImportWelcome3.Text", Me.DefaultScope)
                Me.lblWelcome3.Visible = True
                Me.txtErrors.Visible = True
                Me.txtErrors.Style("color") = "black"

                Dim tbImports As DataTable = API.DataLinkServiceMethods.GetImports(Me)
                Dim oRows() As DataRow = tbImports.Select("ID = " & Me.intIDImport)

                If oRows.Length = 1 Then
                    Dim oImportRow As DataRow = oRows(0)
                    Me.txtErrors.Value = oImportRow("LastLog")
                Else
                    Me.txtErrors.Value = ""
                End If
            Else
                Me.lblWelcome2.Text = Me.Language.Translate("End.Error.EmployeeImportWelcome2.Text", Me.DefaultScope)
                Me.lblWelcome3.Visible = True
                Me.lblWelcome3.Text = ErrorDescription
                Me.txtErrors.Visible = True
                Me.txtErrors.Style("color") = "red"

                Dim tbImports As DataTable = API.DataLinkServiceMethods.GetImports(Me)
                Dim oRows() As DataRow = tbImports.Select("ID = " & Me.intIDImport)

                If oRows.Length = 1 Then
                    Dim oImportRow As DataRow = oRows(0)
                    Me.txtErrors.Value = oImportRow("LastLog")
                Else
                    Me.txtErrors.Value = ""
                End If
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
                iCurrentTask = Me.ExecuteImport()
            Case "CHECKPROGRESS"
                If iCurrentTask >= 0 Then
                    Dim oTask As roLiveTask = API.LiveTasksServiceMethods.GetLiveTaskStatus(Me.Page, iCurrentTask)
                    If oTask IsNot Nothing Then
                        Select Case oTask.Status
                            Case 0, 1
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "")
                            Case 2
                                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                                PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                            Case 3
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
        End Select

    End Sub

    Private Function ExecuteImport() As Integer

        Dim iTask As Integer = -1

        Try

            Me.oCurrentImport = API.DataLinkServiceMethods.GetImportGuide(Me, intIDImport, False)

            If Me.oCurrentImport Is Nothing Then Return False

            Dim oFileOrig As Byte() = Session("ImportFileOrig")
            Dim oFileSchema As Byte() = Session("ImportFileSchema")
            Dim iIDImportTemplate As Integer = -1
            If Session("IDImportTemplate") IsNot Nothing AndAlso Session("IDImportTemplate") <> -1 Then
                iIDImportTemplate = Session("IDImportTemplate")
            End If

            Dim separator As String = String.Empty
            Dim fileType As Integer = -1

            Dim employeesSelected As String = Me.hdnEmployeesSelected.Value & "@" & Me.FeatureID & "@" & Me.hdnFilter.Value & "@" & Me.hdnFilterUser.Value

            Dim startDate As Date = Me.txtScheduleBeginDate.Date
            Dim endDate As Date = Me.txtScheduleEndDate.Date

            Dim ExcelIsTemplate = IIf(Me.rbExcelType2.Checked = True, 1, 0)
            Dim CopyMainShifts = Me.chkCopyMainShifts.Checked
            Dim CopyHolidays = Me.chkCopyHolidays.Checked
            Dim KeepHolidays = Me.chkKeepHolidays.Checked
            Dim KeepLockedDays = Me.chkKeepLockedDays.Checked

            If hdnIsBusiness.Value = "1" Then
                separator = Session("ImportSeparator")
                fileType = Session("FileType")
            Else
                separator = Me.oCurrentImport.Separator
                fileType = Me.oCurrentImport.Type
            End If

            Try
                Dim strFileName As String
                If intIDImport = 1 Then
                    ' Miro si hay timestamp en el nombre del fichero
                    strFileName = Session("ImportFileOrigName")
                    If strFileName.Contains("@") Then
                        Dim sYear, sMonth, sDay As String
                        sYear = strFileName.Substring(strFileName.IndexOf("@") + 1, 8).Substring(0, 4)
                        sMonth = strFileName.Substring(strFileName.IndexOf("@") + 1, 8).Substring(4, 2)
                        sDay = strFileName.Substring(strFileName.IndexOf("@") + 1, 8).Substring(6, 2)
                        If IsDate(sYear & "/" & sMonth & "/" & sDay) Then
                            startDate = DateSerial(sYear, sMonth, sDay)
                        End If
                    End If
                End If

                If intIDImport = 14 Then

                End If
            Catch ex As Exception
            End Try

            iTask = API.LiveTasksServiceMethods.ExecuteImportInBackground(Me.Page, intIDImport, oFileOrig, oFileSchema, employeesSelected, startDate, endDate,
                                                                                       ExcelIsTemplate, CopyMainShifts, CopyHolidays, KeepHolidays, KeepLockedDays, separator, fileType, iIDImportTemplate)

            If iTask >= 0 Then
                ' Audit()
                HelperSession.DeleteEmployeeGroupsFromApplication()
                Try
                    Dim lstAuditParameterNames As New List(Of String)
                    Dim lstAuditParameterValues As New List(Of String)

                    lstAuditParameterNames.Add("{iTask}")
                    lstAuditParameterValues.Add(iTask)

                    lstAuditParameterNames.Add("{ImportName}")
                    lstAuditParameterValues.Add(oCurrentImport.Name)

                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tDataLinkImportTask, "", lstAuditParameterNames, lstAuditParameterValues, Me.Page)
                Catch ex As Exception
                End Try
            End If
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

        Select Case Frame
            Case Wizards_ImportWizard.Frame.frmSelection

                If strMsg <> "" Then bolRet = False
                Me.lblStep1Error.Text = strMsg

            Case Wizards_ImportWizard.Frame.frmEmployeesSelector

                'Comprobar si hay algun empleado seleccionado
                If Me.hdnEmployeesSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.IncorrectEmployeesSelected", Me.DefaultScope)
                    bolRet = False
                End If

                Me.lblStep1Error.Text = strMsg

            Case Wizards_ImportWizard.Frame.frmSchedule
                If Me.txtScheduleBeginDate.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.BeginCopyScheduleIncorrect", Me.DefaultScope)
                ElseIf Me.txtScheduleEndDate.Value Is Nothing Then
                    strMsg = Me.Language.Translate("CheckPage.EndCopyScheduleIncorrect", Me.DefaultScope)
                ElseIf Me.txtScheduleBeginDate.Date > Me.txtScheduleEndDate.Date Then
                    strMsg = Me.Language.Translate("CheckPage.CopySchedulePeriodIncorrect", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub FrameChange(ByVal oOldFrame As Frame, ByVal oActiveFrame As Frame)

        Select Case oOldFrame
            Case Frame.frmSelection

            Case Frame.frmEmployeesSelector
                ' Desactivar el iframe del selector de grupos
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifEmployeesSelector.Disabled = True

            Case Frame.frmSchedule

        End Select

        Select Case oActiveFrame
            Case Frame.frmSelection
                Dim strFileTypes As String = "111"
                'Dim oImport As DataLinkService.roImportGuide

                'oImport = API.DataLinkServiceMethods.GetImportGuide(Me, Me.intIDImport, False)

                If Not Me.oCurrentImport Is Nothing Then
                    If Me.intIDImport = 2 Or Me.intIDImport = 5 Or Me.intIDImport = 11 Or Me.intIDImport = 14 Or Me.intIDImport = 21 Then ' Si estamos en importación de planificación o tareas sólo mostrar el origen de datos excel
                        strFileTypes = "100"
                    ElseIf Me.intIDImport = 20 OrElse Me.intIDImport = 22 Then
                        strFileTypes = "110" 'Para usuarios importación en Excel y ASCII
                    Else
                        Select Case Me.oCurrentImport.Type
                            Case 1 'EXCEL
                                strFileTypes = "100"
                            Case 2 'ASCII
                                strFileTypes = "010"
                            Case 3 'XML
                                strFileTypes = "001"
                        End Select
                    End If
                End If

                If Not Me.oCurrentImport Is Nothing Then
                    If Me.intIDImport = 14 OrElse Me.intIDImport = 21 Then
                        Me.advancedImportOptions.Visible = True
                    Else
                        Me.advancedImportOptions.Visible = False
                    End If
                End If
                Me.ifUploads.Attributes("src") = Me.ResolveUrl("ImportsUpload.aspx?FileTypes=" & strFileTypes & "&IdImport=" & Me.intIDImport & "&Separator=" & Me.oCurrentImport.Separator & "&isBusiness=" & Me.hdnIsBusiness.Value)

            Case Frame.frmEmployeesSelector
                Dim strAux As String = "~/Base/WebUserControls/roWizardSelectorContainerMultiSelectV3.aspx?" &
                                       "PrefixTree=treeEmployeesImportWizard&FeatureAlias=" & Me.FeatureID & "=Write&PrefixCookie=objContainerTreeV3_treeEmployeesImportWizardGrid&" &
                                       "AfterSelectFuncion=parent.GetSelectedTreeV3"
                Me.ifEmployeesSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifEmployeesSelector.Disabled = False

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
            Me.btPrev.Visible = False
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
            oLabel = HelperWeb.GetControl(Me.Controls, "lblStep" & n.ToString & "Info")
            If oLabel IsNot Nothing Then oLabel.Text = Me.hdnSetpInfo.Text
        Next

    End Sub

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

                Dim tbGroups As DataTable = API.EmployeeGroupsServiceMethods.GetGroups(Me, "Employees")
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

        Dim tb As DataTable = API.EmployeeGroupsServiceMethods.GetEmployeesFromGroupWithType(Me, _IDGroup, "Employees")
        If tb IsNot Nothing Then
            For Each oRow As DataRow In tb.Rows
                oRet.Add(oRow("IDEmployee"))
            Next
        End If

        Return oRet

    End Function

#End Region

End Class