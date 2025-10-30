Imports DevExpress.Web
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class DataLinkBusiness
    Inherits PageBase

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As String

        <Runtime.Serialization.DataMember(Name:="Concept")>
        Public Concept As String

        <Runtime.Serialization.DataMember(Name:="Route")>
        Public Route As RouteDefinition()

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="newTemplateName")>
        Public NewTemplateName As String

    End Class

    <Runtime.Serialization.DataContract()>
    Private Class RouteDefinition

        <Runtime.Serialization.DataMember(Name:="attname")>
        Public attname As String

        <Runtime.Serialization.DataMember(Name:="value")>
        Public value As String

    End Class

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("DataLinks", "~/DataLink/Scripts/DataLinksV2.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Not HasFeaturePermission("Employees.DataLink", Permission.Read) AndAlso Not HasFeaturePermission("Tasks.DataLink", Permission.Read) AndAlso
            Not HasFeaturePermission("Calendar.DataLink", Permission.Read) AndAlso Not HasFeaturePermission("BusinessCenters.DataLink", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        End If

        roTreesDatalink.TreeCaption = Me.Language.Translate("TreeCaptionDataLink", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)
        Me.fImportUpload.NullText = Me.Language.Translate("SelectFile", DefaultScope)

        If Not Me.IsPostBack AndAlso Not Me.IsCallback Then

            'INICIALIZAR COOKIES DE USO PARA EL SELECTOR DE EMPLEADOS
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpDatalinkBusinessExport")
            HelperWeb.roSelector_Initialize("objContainerTreeV3_treeEmpDatalinkBusinessExportGrid")

            Me.optSchedulePeriod.Initialize()
        End If

        If Not Me.IsPostBack AndAlso Not Me.IsCallback Then
            advTemplate.CreateDefaultRibbonTabs(True)
            HideribbonTab(advTemplate, 0)
            HideribbonTab(advTemplate, 1)
            advTemplate.Document.CreateNewDocument()
        End If


        If Not Me.IsPostBack Then
            CreateExportScheduleColumns()
        End If

        GetTemplates(False)
        BindGridExportSchedule(False)
    End Sub

    Private Sub LoadCombos(ByVal oGuide As roDatalinkGuide, Optional ByVal bOnlyExport As Boolean = False)
        If oGuide IsNot Nothing AndAlso oGuide.Export IsNot Nothing Then
            Me.cmbExportType.Items.Clear()
            Me.cmbExportType.ValueType = GetType(Integer)
            Dim templateName As String = String.Empty
            For Each oTemplate In oGuide.Export.Templates
                Me.cmbExportType.Items.Add(oTemplate.Name, oTemplate.ID)
            Next
            Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oGuide.Export.Concept), roDatalinkConcept)
            If eConcept <> roDatalinkConcept.Schedule AndAlso eConcept <> roDatalinkConcept.Requests AndAlso eConcept <> roDatalinkConcept.CustomArgal AndAlso eConcept <> roDatalinkConcept.ZCustom Then
                Me.cmbFormatExport.Items.Clear()
                Me.cmbFormatExport.ValueType = GetType(Integer)
                Dim exportFormats = System.Enum.GetValues(GetType(roDatalinkFormat))
                For Each ef In exportFormats
                    cmbFormatExport.Items.Add(System.Enum.GetName(GetType(roDatalinkFormat), ef), roTypes.Any2Integer(ef) + 1) ' le sumamos 1 porque 1: EXCEL 2:ASCII
                Next
            Else
                Me.cmbFormatExport.Items.Clear()
                Me.cmbFormatExport.ValueType = GetType(Integer)
                Dim exportFormats = System.Enum.GetValues(GetType(roDatalinkFormat))
                cmbFormatExport.Items.Add(System.Enum.GetName(GetType(roDatalinkFormat), roDatalinkFormat.Excel), roTypes.Any2Integer(roDatalinkFormat.Excel) + 1) ' le sumamos 1 porque 1: EXCEL 2:ASCII
            End If
        End If

        If Not bOnlyExport AndAlso oGuide IsNot Nothing AndAlso oGuide.Import IsNot Nothing Then
            Me.cmbImportType.Items.Clear()
            Me.cmbImportType.ValueType = GetType(Integer)
            For Each oTemplate In oGuide.Import.Templates
                Me.cmbImportType.Items.Add(oTemplate.Name, oTemplate.ID)
            Next

            Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oGuide.Import.Concept), roDatalinkConcept)
            If eConcept = roDatalinkConcept.Employees OrElse eConcept = roDatalinkConcept.Absences Then
                Me.cmbFormatImport.Items.Clear()
                Me.cmbFormatImport.ValueType = GetType(Integer)
                Dim importFormats = System.Enum.GetValues(GetType(roDatalinkFormat))
                For Each impF In importFormats
                    cmbFormatImport.Items.Add(System.Enum.GetName(GetType(roDatalinkFormat), impF), roTypes.Any2Integer(impF) + 1) ' le sumamos 1 porque 1: EXCEL 2:ASCII
                Next
            End If
        End If

    End Sub

    Private Sub LoadImportScreenControls(ByRef oGuide As roDatalinkGuide)

        If oGuide IsNot Nothing AndAlso oGuide.Import IsNot Nothing Then
            oGuide.Import.IdDefaultTemplate = Me.cmbImportType.SelectedItem?.Value
            oGuide.Import.FormatFilePath = Me.txtfileTmp.Text
            oGuide.Import.SourceFilePath = Me.txtfileOrig.Text
            oGuide.Import.CopySource = True
            oGuide.Import.Location = ""

            oGuide.Import.IsEnabled = Me.ckImportEnable.Checked

            If cmbFormatImport.SelectedItem IsNot Nothing Then
                oGuide.Import.FileType = roTypes.Any2Integer(Me.cmbFormatImport.SelectedItem.Value)
            End If

            oGuide.Import.Separator = Me.txtImportSeparator.Text
        End If

    End Sub

    Private Function LoadExportScreenControls(ByRef oGuide As roDatalinkGuide) As Boolean
        Dim bHasAllData As Boolean = True
        If oGuide IsNot Nothing AndAlso oGuide.Export IsNot Nothing Then
            Dim oSchedule As roDatalinkExportSchedule = Nothing
            Dim idSchedule As Integer = roTypes.Any2Integer(Me.hdnScheduleId.Get("Id"))
            oSchedule = oGuide.Export.Schedules.ToList.Find(Function(x) x.Id = idSchedule)

            Dim bIsNew As Boolean = False
            If oSchedule Is Nothing Then
                bIsNew = True
                oSchedule = New roDatalinkExportSchedule
            End If

            oSchedule.Name = txtScheduleName.Text.Trim()
            oSchedule.IdGuide = oGuide.Export.Id

            Dim filterEmp As String = HttpUtility.UrlDecode(Me.hdnEmployees.Value) & "@" & WLHelperWeb.CurrentPassportID & "@" & "Employees" & "@" & HttpUtility.UrlDecode(Me.hdnFilter.Value) & "@" & HttpUtility.UrlDecode(Me.hdnFilterUser.Value)
            oSchedule.EmployeeFilter = filterEmp

            Dim etype As TypePeriodEnum
            Dim xStartDate As DateTime
            Dim xEndDate As DateTime
            Dim iShiftedMonthStartDay As Integer = 1
            Dim iShiftedMonthMonthsInAdvance As Integer = 1
            Me.optSchedulePeriod.GetValues(etype, xStartDate, xEndDate, iShiftedMonthStartDay, iShiftedMonthMonthsInAdvance)
            If etype = TypePeriodEnum.PeriodNMonthsAgoFromDay Then
                xStartDate = New DateTime(DateTime.Now.Year, iShiftedMonthMonthsInAdvance, iShiftedMonthStartDay)
                xEndDate = xStartDate.AddMonths(1).AddDays(-1)
            End If
            oSchedule.AutomaticDatePeriod = Robotics.VTBase.roSupport.DateTimePeriod2String(etype, xStartDate, xEndDate)

            If oGuide.Concept = roDatalinkConcept.Accruals Then
                oSchedule.ApplyLockDate = ckApplyLockDate.Checked
            Else
                oSchedule.ApplyLockDate = False
            End If

            Dim oTmpScheduler As New roReportSchedulerSchedule
            Me.optSchedule1.GetValues(oTmpScheduler)

            Dim oNewScheduler As New roReportSchedulerSchedule With {
                .DateSchedule = oTmpScheduler.DateSchedule,
                .Day = oTmpScheduler.Day,
                .Days = oTmpScheduler.Days,
                .Hour = oTmpScheduler.Hour,
                .MonthlyType = oTmpScheduler.MonthlyType,
                .ScheduleType = oTmpScheduler.ScheduleType,
                .Start = oTmpScheduler.Start,
                .WeekDay = CInt(oTmpScheduler.WeekDay),
                .WeekDays = oTmpScheduler.WeekDays,
                .Weeks = oTmpScheduler.Weeks
                }

            oSchedule.Scheduler = oNewScheduler

            If Me.cmbExportType.SelectedItem IsNot Nothing Then oSchedule.IdTemplate = roTypes.Any2Integer(Me.cmbExportType.SelectedItem.Value)

            If oGuide.Concept <> roDatalinkConcept.Schedule AndAlso oGuide.Concept <> roDatalinkConcept.Requests AndAlso oGuide.Concept <> roDatalinkConcept.CustomArgal AndAlso oGuide.Concept <> roDatalinkConcept.ZCustom Then
                If oGuide.Concept = roDatalinkConcept.Absences Then
                    'Las ausencias se obliga a dejar el formato como ASCII
                    If oSchedule.IdTemplate < 10 Then
                        oSchedule.ExportFileType = 1
                    Else
                        If cmbFormatExport.SelectedItem IsNot Nothing Then
                            oSchedule.ExportFileType = roTypes.Any2Integer(Me.cmbFormatExport.SelectedItem.Value)
                        Else
                            oSchedule.ExportFileType = 1
                        End If
                    End If
                Else
                    If cmbFormatExport.SelectedItem IsNot Nothing Then
                        oSchedule.ExportFileType = roTypes.Any2Integer(Me.cmbFormatExport.SelectedItem.Value)
                    Else
                        oSchedule.ExportFileType = 1
                    End If
                End If
            Else
                oSchedule.ExportFileType = 1

            End If

            oSchedule.Location = ""
            oSchedule.Separator = Me.txtExportSeparator.Text

            oSchedule.ExportFileName = Me.txtExportfileOrig.Text

            oSchedule.Enabled = ckScheduleActive.Checked

            If oSchedule.Name = String.Empty OrElse oSchedule.ExportFileName = String.Empty OrElse oSchedule.IdTemplate = 0 Then
                bHasAllData = False
            End If

            If bIsNew Then
                Dim lst As Generic.List(Of roDatalinkExportSchedule) = oGuide.Export.Schedules.ToList()
                lst.Add(oSchedule)
                oGuide.Export.Schedules = lst.ToArray
            End If
        End If

        Return bHasAllData
    End Function

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage As New roJSON.JSONError(False, "")
        Dim bRet As Boolean = False

        Select Case oParameters.Action

            Case "GETDATALINK"
                LoadDataLinkGuide(oParameters)
                LoadExportTemplates(oParameters)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETDATALINK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", "")
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
            Case "SAVEIMPORTGUIDE"
                SaveImportGuide(oParameters, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVEIMPORTGUIDE")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", "")
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
            Case "LoadExportTemplates"
                bRet = True
                LoadDataLinkGuide(oParameters)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "LoadExportTemplates")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", "")
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

            Case "LoadExportTemplatesAvailable"
                bRet = True
                LoadDataLinkGuide(oParameters)
                LoadExportTemplatesAvailable(roTypes.Any2String(oParameters.NewTemplateName), oParameters)

                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "LoadExportTemplatesAvailable")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", "")
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

            Case "LoadFileTemplateContent"
                bRet = True
                LoadDataLinkGuide(oParameters)
                LoadFileTemplateContent(oParameters)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "LoadFileTemplateContent")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", "")
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)


            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Public Sub CCallbackPopupOperations_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackPopupOperations.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage As New roJSON.JSONError(False, "")
        Dim bRet As Boolean = False

        Select Case oParameters.Action
            Case "GETINFOSELECTED"
                Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
                Dim tmpCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)
                LoadCombos(tmpCurrentGuide, True)

                LoadExportScreenControls(tmpCurrentGuide)
                SetControlsVisibility(oParameters, tmpCurrentGuide)

                Dim sInfo As String = GetInfoSelected(oParameters)
                CallbackPopupOperations.JSProperties.Add("cpAction", "GETINFOSELECTED")
                CallbackPopupOperations.JSProperties.Add("cpResult", "OK")
                CallbackPopupOperations.JSProperties.Add("cpMessage", sInfo)
                CallbackPopupOperations.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                CallbackPopupOperations.JSProperties.Add("cpNameRO", "")
                CallbackPopupOperations.JSProperties.Add("cpIsNew", False)
            Case "GETDATALINK"
                Dim oSchedule = LoadDataLinkExportSchedule(oParameters)
                CallbackPopupOperations.JSProperties.Add("cpAction", "GETDATALINK")
                CallbackPopupOperations.JSProperties.Add("cpResult", IIf(oSchedule IsNot Nothing, "OK", "NOK"))
                CallbackPopupOperations.JSProperties.Add("cpObject", roJSONHelper.SerializeNewtonSoft(oSchedule))
                CallbackPopupOperations.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                CallbackPopupOperations.JSProperties.Add("cpNameRO", "")
                CallbackPopupOperations.JSProperties.Add("cpIsNew", False)
            Case "SAVEEXPORTGUIDE"
                bRet = SaveExportGuide(oParameters, responseMessage)
                CallbackPopupOperations.JSProperties.Add("cpAction", "SAVEEXPORTGUIDE")
                CallbackPopupOperations.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                CallbackPopupOperations.JSProperties.Add("cpError", roJSONHelper.SerializeNewtonSoft(responseMessage))
                CallbackPopupOperations.JSProperties.Add("cpNameRO", "")
                CallbackPopupOperations.JSProperties.Add("cpIsNew", False)
        End Select

    End Sub

    Private Function GetInfoSelected(ByVal oParameters As ObjectCallbackRequest) As String
        Dim strRet As String = "0; "

        Try
            If Not String.IsNullOrEmpty(Me.hdnEmployees.Value) Then
                strRet = LoadSelectedEmployeesDescription()

                Me.txtSelectedEmployees.Text = strRet
            End If
        Catch
            strRet = "Error loading employees"
        End Try

        Return strRet

    End Function

    Private Function LoadSelectedEmployeesDescription() As String
        Dim sInfo As String = String.Empty

        Dim DateInf As DateTime = DateTime.Now
        Dim DateSup As DateTime = DateTime.Now

        If DateInf > DateSup Then
            Dim aux As DateTime = DateSup
            DateSup = DateInf
            DateInf = aux
        End If

        Dim Selection() As String = Me.hdnEmployees.Value.Trim.Split(",")
        If Selection.Length = 1 Then

            If Selection(0).Substring(0, 1) = "A" Then 'Grupo
                'obtener el nombre del grupo
                Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me.Page, Selection(0).Substring(1), False)
                If Not oGroup Is Nothing Then
                    'Dim NumEmployees As Integer = GetNumOfEmployees()
                    Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Employees", DateInf, DateSup)
                    sInfo = oGroup.Name
                End If

            ElseIf Selection(0).Substring(0, 1) = "B" Then 'Empleado
                'obtener el nombre del empleado
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me.Page, Selection(0).Substring(1), False)
                If Not oEmployee Is Nothing Then
                    sInfo = oEmployee.Name
                End If
            End If
        Else
            'Dim NumEmployees As Integer = GetNumOfEmployees()
            Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(Me.hdnEmployees.Value, Me.hdnFilter.Value, Me.hdnFilterUser.Value, "Employees", DateInf, DateSup)
            sInfo = NumEmployees.ToString & " " & Me.Language.Translate("GridAbsences.EmployeesSelected", DefaultScope)
        End If

        Return sInfo
    End Function

    Private Function SaveExportGuide(ByVal oParameters As ObjectCallbackRequest, ByRef roError As roJSON.JSONError) As Boolean
        Dim bSaved As Boolean = False
        Dim bHasAlldata As Boolean = False
        Dim oCurrentGuide As roDatalinkGuide = Nothing
        Dim strcombosIndex As String = ""

        Try
            Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
            oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

            If oCurrentGuide IsNot Nothing AndAlso oCurrentGuide.Export IsNot Nothing Then
                LoadCombos(oCurrentGuide)
                bHasAlldata = LoadExportScreenControls(oCurrentGuide)

                If bHasAlldata Then
                    oCurrentGuide.Import = Nothing
                    bSaved = API.DataLinkGuideServiceMethods.SaveDatalinkGuide(Me, oCurrentGuide, True)

                    If Not bSaved Then
                        roError.Error = True
                        roError.ErrorText = API.DataLinkGuideServiceMethods.LastErrorText
                        SetControlsVisibility(oParameters, oCurrentGuide)
                    Else
                        LoadCombos(oCurrentGuide, True)
                        LoadDataLinkGuide(oParameters)
                    End If
                Else
                    bSaved = False
                End If

            End If
        Catch ex As Exception
        Finally
        End Try

        Return bSaved
    End Function

    Private Function SaveImportGuide(ByVal oParameters As ObjectCallbackRequest, ByRef roError As roJSON.JSONError) As Boolean
        Dim bSaved As Boolean = False
        Dim strError As String = ""
        Dim oCurrentGuide As roDatalinkGuide = Nothing
        Dim strcombosIndex As String = ""

        Try
            Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
            oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

            If oCurrentGuide IsNot Nothing AndAlso oCurrentGuide.Import IsNot Nothing Then
                LoadCombos(oCurrentGuide)
                LoadImportScreenControls(oCurrentGuide)

                oCurrentGuide.Export = Nothing
                bSaved = API.DataLinkGuideServiceMethods.SaveDatalinkGuide(Me, oCurrentGuide, True)

                If Not bSaved Then
                    roError.Error = True
                    roError.ErrorText = API.DataLinkGuideServiceMethods.LastErrorText
                    SetControlsVisibility(oParameters, oCurrentGuide)
                Else
                    LoadDataLinkGuide(oParameters)
                End If
            End If
        Catch ex As Exception
        Finally
        End Try

        Return bSaved
    End Function

    Private Function LoadDataLinkExportSchedule(ByVal oParameters As ObjectCallbackRequest, Optional ByVal oGuide As roDatalinkGuide = Nothing) As roDatalinkExportSchedule
        Dim oSchedule As roDatalinkExportSchedule = Nothing
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentGuide As roDatalinkGuide = Nothing

        Try
            If oGuide Is Nothing Then
                Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
                oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)
            Else
                oCurrentGuide = oGuide
            End If

            LoadCombos(oCurrentGuide, True)

            If oCurrentGuide IsNot Nothing AndAlso oCurrentGuide.Export IsNot Nothing Then

                Dim idSchedule As Integer = roTypes.Any2Integer(Me.hdnScheduleId.Get("Id"))
                oSchedule = oCurrentGuide.Export.Schedules.ToList.Find(Function(x) x.Id = idSchedule)
            End If

            If oSchedule IsNot Nothing Then
                If oCurrentGuide.Concept = roDatalinkConcept.Accruals Then
                    'Me.ckApplyLockDate.Checked = oCurrentGuide.Export.ApplyLockDate
                    Me.divApplyLockDate.Style("display") = ""
                Else
                    Me.ckApplyLockDate.Checked = False
                    Me.divApplyLockDate.Style("display") = "none"
                End If

                Me.txtScheduleName.Text = oSchedule.Name

                Me.cmbExportType.SelectedItem = Me.cmbExportType.Items.FindByValue(oSchedule.IdTemplate)
                Me.cmbFormatExport.SelectedItem = Me.cmbFormatExport.Items.FindByValue(oSchedule.ExportFileType)

                If cmbFormatExport.SelectedItem IsNot Nothing AndAlso cmbFormatExport.SelectedItem.Value = "2" Then
                    divSeparator.Style("display") = ""
                Else
                    divSeparator.Style("display") = "none"
                End If

                Me.txtExportSeparator.Text = oSchedule.Separator
                Me.txtExportfileOrig.Text = oSchedule.ExportFileName
                Me.txtLogExport.Text = oSchedule.LastExecutionLog

                If oSchedule.Enabled Then
                    Me.ckScheduleActive.Checked = True
                Else
                    Me.ckScheduleActive.Checked = False
                End If


                If oSchedule.AutomaticDatePeriod <> String.Empty Then
                    Dim iShiftedMonthStartDay As Integer = 1
                    Dim iShiftedMonthMonthsInAdvance As Integer = 1
                    Dim oConf As Robotics.Base.DTOs.roDateTimePeriod = Robotics.VTBase.roSupport.String2DateTimePeriod(oSchedule.AutomaticDatePeriod)

                    If oConf.TypePeriod = TypePeriodEnum.PeriodNMonthsAgoFromDay Then
                        Dim dRefDate As Date = Date.ParseExact(oSchedule.AutomaticDatePeriod.Split(",")(1), "yyyy-MM-dd HH:mm:ss", Nothing)
                        iShiftedMonthStartDay = dRefDate.Day
                        iShiftedMonthMonthsInAdvance = dRefDate.Month
                    End If

                    Me.optSchedulePeriod.SetValues(oConf.TypePeriod, oConf.BeginDateTimePeriod, oConf.EndDateTimePeriod, False, iShiftedMonthStartDay, iShiftedMonthMonthsInAdvance)
                Else
                    Me.optSchedulePeriod.SetValues(TypePeriodEnum.PeriodToday, DateTime.Now.Date, DateTime.Now.Date, False)
                End If

                If oSchedule.Scheduler IsNot Nothing Then
                    Me.optSchedule1.LoadValues(oSchedule.Scheduler, False)
                Else
                    Me.optSchedule1.LoadValues(New roReportSchedulerSchedule(), False)
                End If

                Dim arrEmployees As Array = roTypes.Any2String(oSchedule.EmployeeFilter).Split("@")

                If arrEmployees.Length <> 5 Then
                    hdnEmployees.Value = String.Empty
                    hdnFilter.Value = String.Empty
                    hdnFilterUser.Value = String.Empty
                    Me.txtSelectedEmployees.Text = String.Empty

                    HelperWeb.roSelector_SetSelection(String.Empty, "", "objContainerTreeV3_treeEmpDatalinkBusinessExport")
                    HelperWeb.roSelector_SetSelection(String.Empty, "", "objContainerTreeV3_treeEmpDatalinkBusinessExportGrid")
                Else
                    Me.hdnEmployees.Value = arrEmployees(0)
                    Me.hdnFilter.Value = arrEmployees(3)
                    Me.hdnFilterUser.Value = arrEmployees(4)

                    If Not String.IsNullOrEmpty(Me.hdnEmployees.Value) Then
                        Dim sText As String = LoadSelectedEmployeesDescription()
                        Me.txtSelectedEmployees.Text = sText
                    End If

                    HelperWeb.roSelector_SetSelection(arrEmployees(0), "", "objContainerTreeV3_treeEmpDatalinkBusinessExport", arrEmployees(3), arrEmployees(4))
                    HelperWeb.roSelector_SetSelection(arrEmployees(0), "", "objContainerTreeV3_treeEmpDatalinkBusinessExportGrid", arrEmployees(3), arrEmployees(4))
                End If

                Dim CookieSchedulerGrid As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeEmpDatalinkBusinessExportGrid")
                CookieSchedulerGrid.ActiveTree = "1"
                CookieSchedulerGrid.Selected1 = hdnEmployees.Value
                HelperWeb.roSelector_SetTreeState(CookieSchedulerGrid)
            Else
                oSchedule = New roDatalinkExportSchedule()

                Me.ckScheduleActive.Checked = False
                Me.ckApplyLockDate.Checked = False
                Me.txtScheduleName.Text = String.Empty
                Me.cmbExportType.SelectedItem = Nothing
                Me.cmbFormatExport.SelectedItem = Nothing

                Me.divFormatExport.Style("display") = "none"
                Me.divSeparator.Style("display") = "none"
                If oCurrentGuide IsNot Nothing AndAlso oCurrentGuide.Concept = roDatalinkConcept.Accruals Then
                    Me.divApplyLockDate.Style("display") = ""
                Else
                    Me.divApplyLockDate.Style("display") = "none"
                End If

                Me.optSchedulePeriod.SetValues(TypePeriodEnum.PeriodToday, DateTime.Now.Date, DateTime.Now.Date, False)

                Me.optSchedule1.LoadValues(New roReportSchedulerSchedule(), False)

                Me.txtLogExport.Text = String.Empty
                Me.txtExportfileOrig.Text = String.Empty
                Me.txtExportSeparator.Text = String.Empty

                hdnEmployees.Value = String.Empty
                hdnFilter.Value = String.Empty
                hdnFilterUser.Value = String.Empty
                Me.txtSelectedEmployees.Text = String.Empty

                HelperWeb.roSelector_SetSelection(String.Empty, "", "objContainerTreeV3_treeEmpDatalinkBusinessExport")
                HelperWeb.roSelector_SetSelection(String.Empty, "", "objContainerTreeV3_treeEmpDatalinkBusinessExportGrid")
            End If
        Catch ex As Exception
            oSchedule = Nothing
        End Try

        Return oSchedule
    End Function

    Private Sub LoadDataLinkGuide(ByVal oParameters As ObjectCallbackRequest, Optional ByVal oGuide As roDatalinkGuide = Nothing)
        Dim strError As String = ""
        Dim strMessage As String = ""
        Dim oCurrentGuide As roDatalinkGuide = Nothing
        Dim strcombosIndex As String = ""
        Dim isASCII As Boolean = False

        Try
            Dim reloadGrids = False
            If oGuide Is Nothing Then
                Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
                oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)
                reloadGrids = True
            Else
                oCurrentGuide = oGuide
            End If
            Session("DataLinkBusiness_SelectedConcept") = oCurrentGuide.Concept
            GetTemplates(True)
            BindGridExportSchedule(True)

            LoadCombos(oCurrentGuide)

            Me.lblDatalinkBusinessInnerDescription.Text = oCurrentGuide.ImportDescription
            Me.lblDatalinkBusinessExportDescription.Text = oCurrentGuide.ExportDescription
            If oCurrentGuide IsNot Nothing AndAlso oCurrentGuide.Import IsNot Nothing Then

                Me.cmbImportType.SelectedItem = Me.cmbImportType.Items.FindByValue(oCurrentGuide.Import.IdDefaultTemplate)
                Me.hdnSampleFileName.Value = oCurrentGuide.Import.Templates(0).TemplateFile

                Me.txtfileOrig.Text = oCurrentGuide.Import.SourceFilePath
                Me.txtfileTmp.Text = oCurrentGuide.Import.FormatFilePath
                Me.txtLogImport.Text = oCurrentGuide.Import.LastExecutionLog

                Me.ckImportEnable.Checked = oCurrentGuide.Import.IsActive

                Me.txtImportSeparator.Text = oCurrentGuide.Import.Separator

                If cmbFormatImport.Items IsNot Nothing AndAlso cmbFormatImport.Items.Count > 0 Then
                    Me.cmbFormatImport.SelectedItem = Me.cmbFormatImport.Items.FindByValue(oCurrentGuide.Import.FileType)
                    isASCII = cmbFormatImport.SelectedItem.Value = "2"
                Else
                    isASCII = oCurrentGuide.Import.FileType = 2
                End If

                If isASCII Then
                    divImportSeparator.Style("display") = ""
                Else
                    divImportSeparator.Style("display") = "none"
                End If

                If oCurrentGuide.Import.IsActive Then
                    Me.ckImportActive.Checked = True
                    Me.ckImportActive.Text = Me.Language.Translate("Import.ActiveAndEnabled", Me.DefaultScope)
                Else
                    Me.ckImportActive.Checked = False
                    Me.ckImportActive.Text = Me.Language.Translate("Import.ActiveAndDisabled", Me.DefaultScope)
                End If

                Me.ckImportEnable.Checked = oCurrentGuide.Import.IsEnabled
            Else
                Me.cmbImportType.SelectedItem = Nothing
                Me.txtfileOrig.Text = String.Empty
                Me.txtLogImport.Text = String.Empty
                Me.cmbFormatImport.SelectedItem = Nothing
                Me.txtImportSeparator.Text = String.Empty
                Me.ckImportActive.Checked = False
                Me.ckImportActive.Text = Me.Language.Translate("Import.ActiveAndDisabled", Me.DefaultScope)
            End If

            LoadExportTemplates(oParameters)

        Catch ex As Exception
        Finally
            SetControlsVisibility(oParameters, oCurrentGuide)
        End Try
    End Sub

    Private Sub SetControlsVisibility(oParameters As ObjectCallbackRequest, oCurrentGuide As roDatalinkGuide)

        Dim oLicSupport As New Robotics.VTBase.Extensions.roLicenseSupport()
        Dim oLicInfo As Robotics.VTBase.Extensions.roVTLicense = oLicSupport.GetVTLicenseInfo()

        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") OrElse oLicInfo.Edition = Robotics.VTBase.Extensions.roServerLicense.roVisualTimeEdition.Starter OrElse
            oCurrentGuide.Concept = roDatalinkConcept.CustomArgal OrElse oCurrentGuide.Concept = roDatalinkConcept.ZCustom OrElse
            oCurrentGuide.Concept = roDatalinkConcept.Iberper Then
            Me.rowAutomaticExport.Visible = False
            Me.rowAutomaticImport.Visible = False
        Else
            Me.rowAutomaticExport.Visible = True
            Me.rowAutomaticImport.Visible = True
        End If

        If oCurrentGuide.Concept = roDatalinkConcept.Schedule Then
            Me.rowAutomaticImport.Visible = False
        End If

        If oCurrentGuide.Import Is Nothing Then
            Me.div00.Style("display") = "none"
        Else
            Me.div00.Style("display") = ""

            If Me.HasFeaturePermission("Employees.DataLink", Permission.Read) Or Me.HasFeaturePermission("Calendar.DataLink", Permission.Read) Or
                Me.HasFeaturePermission("Tasks.DataLink", Permission.Read) Or Me.HasFeaturePermission("BusinessCenters.DataLink", Permission.Read) Then
                Me.btnImportDiv.Style("display") = ""
                Me.rowAutomaticImport.Style("display") = ""
            End If
            Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oCurrentGuide.Import.Concept), roDatalinkConcept)
            If eConcept = roDatalinkConcept.Employees OrElse eConcept = roDatalinkConcept.Absences Then
                divFormatImport.Style("display") = ""
            Else
                divFormatImport.Style("display") = "none"
            End If

            If ((eConcept = roDatalinkConcept.Employees OrElse eConcept = roDatalinkConcept.Absences) AndAlso cmbFormatImport.SelectedItem IsNot Nothing AndAlso cmbFormatImport.SelectedItem.Value = "2") Then
                divImportType.Style("display") = "none"
            Else
                divImportType.Style("display") = ""
            End If

        End If

        If oCurrentGuide.Export Is Nothing Then
            Me.div01.Style("display") = "none"
        Else
            Me.div01.Style("display") = ""

            If Me.HasFeaturePermission("Employees.DataLink", Permission.Read) OrElse Me.HasFeaturePermission("Calendar.DataLink", Permission.Read) OrElse
                Me.HasFeaturePermission("Tasks.DataLink", Permission.Read) OrElse Me.HasFeaturePermission("BusinessCenters.DataLink", Permission.Read) Then
                Me.btnExportDiv.Style("display") = ""
                Me.rowAutomaticExport.Style("display") = ""
            End If

            Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oCurrentGuide.Export.Concept), roDatalinkConcept)
            If eConcept <> roDatalinkConcept.Schedule AndAlso eConcept <> roDatalinkConcept.Requests AndAlso eConcept <> roDatalinkConcept.CustomArgal AndAlso eConcept <> roDatalinkConcept.ZCustom Then
                divFormatExport.Style("display") = ""

                If eConcept = roDatalinkConcept.Absences AndAlso (Me.cmbExportType.SelectedItem Is Nothing OrElse Me.cmbExportType.SelectedItem.Value < 10) Then
                    divFormatExport.Style("display") = "none"
                End If
            Else
                divFormatExport.Style("display") = "none"
            End If

            If eConcept = roDatalinkConcept.Accruals Then
                'Me.ckApplyLockDate.Checked = oCurrentGuide.Export.ApplyLockDate
                Me.divApplyLockDate.Style("display") = ""
            Else
                Me.ckApplyLockDate.Checked = False
                Me.divApplyLockDate.Style("display") = "none"
            End If

        End If

        If ((oParameters.Concept.ToLower = roDatalinkConcept.Employees.ToString.ToLower OrElse oParameters.Concept.ToUpper = "ABSENCES") AndAlso cmbFormatImport.SelectedItem IsNot Nothing AndAlso cmbFormatImport.SelectedItem.Value = 2) Then
            Me.divFileTemplate.Style("display") = ""
        Else
            Me.divFileTemplate.Style("display") = "none"
        End If

        If oCurrentGuide.Export Is Nothing Then
            Me.div02.Style("display") = "none"
        Else
            Me.div02.Style("display") = ""
        End If

        Me.div00.Style("display") = "none"
        Me.div01.Style("display") = "none"
        Me.div02.Style("display") = "none"

        Select Case oParameters.aTab
            Case "00"
                Me.div00.Style("display") = ""
            Case "01"
                Me.div01.Style("display") = ""
            Case "02"
                Me.div02.Style("display") = ""
        End Select

        Exit Sub




        If oParameters.aTab = 0 Then
            If oCurrentGuide.Import IsNot Nothing Then
                Me.div00.Style("display") = ""
                Me.div01.Style("display") = "none"
            ElseIf oCurrentGuide.Export IsNot Nothing Then
                Me.div00.Style("display") = "none"
                Me.div01.Style("display") = ""
            End If

            Me.div02.Style("display") = "none"

        ElseIf oParameters.aTab = 1 Then
            If oCurrentGuide.Export IsNot Nothing AndAlso oCurrentGuide.Import IsNot Nothing Then
                Me.div00.Style("display") = "none"
                Me.div01.Style("display") = ""
                Me.div02.Style("display") = "none"
            ElseIf oCurrentGuide.Import IsNot Nothing Then
                Me.div00.Style("display") = ""
                Me.div01.Style("display") = "none"
                Me.div02.Style("display") = "none"
            Else
                Me.div00.Style("display") = "none"
                Me.div01.Style("display") = "none"
                Me.div02.Style("display") = ""

            End If
        Else
            If oCurrentGuide.Export IsNot Nothing AndAlso oCurrentGuide.Import IsNot Nothing Then
                Me.div00.Style("display") = "none"
                Me.div01.Style("display") = "none"
                Me.div02.Style("display") = ""
            ElseIf oCurrentGuide.Import IsNot Nothing Then
                Me.div00.Style("display") = ""
                Me.div01.Style("display") = "none"
                Me.div02.Style("display") = "none"
            Else
                Me.div00.Style("display") = "none"
                Me.div01.Style("display") = "none"
                Me.div02.Style("display") = ""

            End If


        End If

    End Sub

#Region "Grid LagAgreeRules"

    Private Property ExportScheduleData(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roDatalinkExportSchedule)
        Get

            Dim tbCauses As Generic.List(Of roDatalinkExportSchedule) = Session("DataLinkBusiness_SchedulesData")

            If bolReload Or tbCauses Is Nothing Then
                Dim oList As New Generic.List(Of roDatalinkExportSchedule)
                If Session("DataLinkBusiness_SelectedConcept") IsNot Nothing Then
                    Dim eConcept As roDatalinkConcept = CType(Session("DataLinkBusiness_SelectedConcept"), roDatalinkConcept)
                    Dim oCurrentGuide As roDatalinkGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

                    If oCurrentGuide.Export IsNot Nothing AndAlso oCurrentGuide.Export.Schedules IsNot Nothing Then
                        For Each oLabObject As roDatalinkExportSchedule In oCurrentGuide.Export.Schedules
                            oList.Add(oLabObject)
                        Next
                    End If

                End If
                tbCauses = oList
                Session("DataLinkBusiness_SchedulesData") = oList

            End If
            Return tbCauses

        End Get
        Set(ByVal value As Generic.List(Of roDatalinkExportSchedule))
            If value IsNot Nothing Then
                Session("DataLinkBusiness_SchedulesData") = value
            Else
                Session("DataLinkBusiness_SchedulesData") = Nothing
            End If
        End Set
    End Property

    Private Function GetTemplates(Optional ByVal bolReload As Boolean = False) As Generic.List(Of roDatalinkExportGuideTemplate)
        Dim tbTemplates As Generic.List(Of roDatalinkExportGuideTemplate) = Session("DataLinkBusiness_Templates")

        If bolReload Or tbTemplates Is Nothing Then
            Dim oList As New Generic.List(Of roDatalinkExportGuideTemplate)
            If Session("DataLinkBusiness_SelectedConcept") IsNot Nothing Then
                Dim eConcept As roDatalinkConcept = CType(Session("DataLinkBusiness_SelectedConcept"), roDatalinkConcept)
                Dim oCurrentGuide As roDatalinkGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

                If oCurrentGuide.Export IsNot Nothing AndAlso oCurrentGuide.Export.Templates IsNot Nothing Then
                    For Each oLabObject As roDatalinkExportGuideTemplate In oCurrentGuide.Export.Templates
                        oList.Add(oLabObject)
                    Next
                End If

            End If
            tbTemplates = oList
            Session("DataLinkBusiness_Templates") = oList

        End If

        Return tbTemplates
    End Function

    Private Sub CreateExportScheduleColumns()

        Dim GridColumn As GridViewDataTextColumn
        Dim GridColumnCommand As GridViewCommandColumn
        Dim GridCheckCommand As GridViewDataCheckColumn
        Dim GridColumnDate As GridViewDataDateColumn

        Dim VisibleIndex As Integer = 0

        Me.GridExports.Columns.Clear()
        Me.GridExports.KeyFieldName = "Id"
        Me.GridExports.SettingsText.EmptyDataRow = " "
        Me.GridExports.Settings.VerticalScrollBarMode = ScrollBarMode.Auto

        Dim uiControl As Control = Me.GridExports.FindTitleTemplateControl("btnAddNewSchedule")
        If uiControl IsNot Nothing Then
            uiControl.Visible = False
        End If

        'Clave
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = "Id"
        GridColumn.FieldName = "Id"
        GridColumn.ReadOnly = False
        GridColumn.Visible = False
        Me.GridExports.Columns.Add(GridColumn)

        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image

        Dim editButton As GridViewCommandColumnCustomButton = New GridViewCommandColumnCustomButton()
        editButton.ID = "ShowDetailButton"
        editButton.Image.Url = "~/Base/Images/Grid/edit.png"
        editButton.Image.Height = New Unit(16, UnitType.Pixel)
        editButton.Image.Width = New Unit(16, UnitType.Pixel)
        editButton.Text = Me.Language.Translate("GridExportSchedule.Column.Edit", DefaultScope) 'Mostrar detalles"

        GridColumnCommand.CustomButtons.Add(editButton)

        Dim logButton = New GridViewCommandColumnCustomButton()
        logButton.ID = "ViewLogDetailButton"
        logButton.Image.Url = "~/Base/Images/Grid/Attachment.png"
        logButton.Image.ToolTip = Me.Language.Translate("GridExportSchedule.Column.LogDetail", DefaultScope)
        logButton.Image.Height = New Unit(16, UnitType.Pixel)
        logButton.Image.Width = New Unit(16, UnitType.Pixel)
        GridColumnCommand.CustomButtons.Add(logButton)

        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowDeleteButton = False
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = New Unit(60, UnitType.Pixel)
        VisibleIndex = VisibleIndex + 1

        Me.GridExports.Columns.Add(GridColumnCommand)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridExportSchedule.Column.Id", DefaultScope) '"Fecha"
        GridColumn.FieldName = "Id"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridExports.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridExportSchedule.Column.LastLog", DefaultScope) '"Fecha"
        GridColumn.FieldName = "LastExecutionLog"
        GridColumn.Visible = False
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridExports.Columns.Add(GridColumn)

        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridExportSchedule.Column.Name", DefaultScope) '"Fecha"
        GridColumn.FieldName = "Name"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.Width = New Unit(45, UnitType.Percentage)
        Me.GridExports.Columns.Add(GridColumn)

        'Combo documentos
        GridColumn = New GridViewDataTextColumn()
        GridColumn.Caption = Me.Language.Translate("GridViews.Column.View", DefaultScope) 'Vista
        GridColumn.FieldName = "IdViewUnbound"
        GridColumn.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumn.ReadOnly = True
        GridColumn.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumn.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumn.Width = New Unit(170, UnitType.Pixel)
        GridColumn.UnboundType = DevExpress.Data.UnboundColumnType.String
        Me.GridExports.Columns.Add(GridColumn)

        'Date
        GridColumnDate = New GridViewDataDateColumn()
        GridColumnDate.Caption = Me.Language.Translate("GridExportSchedule.Column.BeginDate", DefaultScope) '"Fecha"
        GridColumnDate.FieldName = "NextExecutionDate"
        GridColumnDate.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridColumnDate.ReadOnly = False
        GridColumnDate.Width = New Unit(150, UnitType.Pixel)
        GridColumnDate.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridColumnDate.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        Me.GridExports.Columns.Add(GridColumnDate)

        GridCheckCommand = New GridViewDataCheckColumn()
        GridCheckCommand.Caption = Me.Language.Translate("GridExportSchedule.Column.Active", DefaultScope) '"Fecha"
        GridCheckCommand.FieldName = "Enabled"
        GridCheckCommand.VisibleIndex = VisibleIndex
        VisibleIndex = VisibleIndex + 1
        GridCheckCommand.ReadOnly = True
        GridCheckCommand.UnboundType = DevExpress.Data.UnboundColumnType.String
        GridCheckCommand.CellStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Left
        GridCheckCommand.Width = New Unit(80, UnitType.Pixel)
        Me.GridExports.Columns.Add(GridCheckCommand)

        'Command buttons
        GridColumnCommand = New GridViewCommandColumn()
        GridColumnCommand.ButtonRenderMode = GridCommandButtonRenderMode.Image
        GridColumnCommand.ShowDeleteButton = True
        GridColumnCommand.ShowEditButton = False
        GridColumnCommand.ShowCancelButton = False
        GridColumnCommand.ShowUpdateButton = True
        GridColumnCommand.Caption = " "
        GridColumnCommand.VisibleIndex = VisibleIndex
        GridColumnCommand.CellStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.HeaderStyle.HorizontalAlign = HorizontalAlign.Center
        GridColumnCommand.Width = New Unit(40, UnitType.Pixel)
        VisibleIndex = VisibleIndex + 1

        Me.GridExports.Columns.Add(GridColumnCommand)

    End Sub

    Private Sub BindGridExportSchedule(ByVal bolReload As Boolean)
        Me.GridExports.DataSource = Me.ExportScheduleData(bolReload)
        Me.GridExports.DataBind()
    End Sub

    Protected Sub GridExportSchedule_CustomUnboundColumnData(sender As Object, e As ASPxGridViewColumnDataEventArgs) Handles GridExports.CustomUnboundColumnData
        Select Case e.Column.FieldName
            Case "IdViewUnbound"
                If e.IsGetData Then
                    If Not e.GetListSourceFieldValue("IdTemplate") Is Nothing Then
                        Dim idView As Integer = CType(e.GetListSourceFieldValue("IdTemplate"), Integer)

                        Dim cTemplate As roDatalinkExportGuideTemplate = GetTemplates().Find(Function(x) x.ID = idView)
                        If cTemplate IsNot Nothing Then
                            e.Value = cTemplate.Name
                        Else
                            e.Value = ""
                        End If

                    End If
                End If
        End Select
    End Sub

    Protected Sub GridExportSchedule_CustomCallback(ByVal sender As Object, ByVal e As DevExpress.Web.ASPxGridViewCustomCallbackEventArgs) Handles GridExports.CustomCallback
        If e.Parameters = "REFRESH" Then
            GetTemplates(False)
            BindGridExportSchedule(False)
            GridExports.JSProperties("cpAction") = "REFRESH"
        ElseIf e.Parameters = "RELOAD" Then
            GetTemplates(True)
            BindGridExportSchedule(True)
            GridExports.JSProperties("cpAction") = "RELOAD"
        End If
    End Sub

    Protected Sub GridExportSchedule_RowDeleting(ByVal sender As Object, ByVal e As DevExpress.Web.Data.ASPxDataDeletingEventArgs) Handles GridExports.RowDeleting
        Dim grid As ASPxGridView = CType(sender, ASPxGridView)
        Dim tb As Generic.List(Of roDatalinkExportSchedule) = Me.ExportScheduleData()

        If e.Values("Id") > 0 OrElse e.Values("Id") < -1 Then
            Dim selObject As roDatalinkExportSchedule = Nothing
            For Each oObject As roDatalinkExportSchedule In tb
                If oObject.Id = e.Values("Id") Then
                    selObject = oObject
                    Exit For
                End If
            Next

            If selObject IsNot Nothing Then
                tb.Remove(selObject)
            End If

            Dim eConcept As roDatalinkConcept = CType(Session("DataLinkBusiness_SelectedConcept"), roDatalinkConcept)
            Dim oCurrentGuide As roDatalinkGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

            oCurrentGuide.Import = Nothing
            oCurrentGuide.Export.Schedules = tb.ToArray
            API.DataLinkGuideServiceMethods.SaveDatalinkGuide(Me, oCurrentGuide, True)

            Me.ExportScheduleData = tb
            e.Cancel = True

        End If
        GetTemplates(False)
        BindGridExportSchedule(False)
        GridExports.JSProperties("cpAction") = "ROWDELETE"

    End Sub
#End Region

#Region "Templates"

    Public Shared Sub HideribbonTab(ByVal spreadsheet As ASPxSpreadsheet.ASPxSpreadsheet, ByVal tabIndex As Integer)
        Dim fileTab As RibbonTab = spreadsheet.RibbonTabs(tabIndex)
        spreadsheet.RibbonTabs.Remove(fileTab)
    End Sub

    Public Shared Sub HideRibbonItem(ByVal spreadsheet As ASPxSpreadsheet.ASPxSpreadsheet, ByVal tabIndex As Integer, ByVal groupIndex As Integer, ByVal itemIndex As Integer)
        Dim group As RibbonGroup = spreadsheet.RibbonTabs(tabIndex).Groups(groupIndex)
        Dim ribbonItem As RibbonItemBase = group.Items(itemIndex)
        group.Items.Remove(ribbonItem)
    End Sub

    Private Function LoadExportTemplates(ByVal oParameters As ObjectCallbackRequest) As String
        Dim bResult As String = "ACTION"
        Dim oCurrentGuide As roDatalinkGuide = Nothing

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
        oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

        cmbAdvTemplateType.ClientEnabled = False
        cmbAdvTemplateType.Items.Clear()
        cmbAdvTemplateName.Items.Clear()

        For Each oElement In oList
            If eConcept = oElement.Concept Then
                If oElement.Export IsNot Nothing AndAlso oElement.Export.Templates IsNot Nothing Then
                    Dim bInclude As Boolean = False
                    For Each oTemplate In oElement.Export.Templates
                        If oTemplate.TemplateFile <> String.Empty Then bInclude = True
                    Next

                    If bInclude Then
                        If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                            cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                        End If
                    End If
                End If
            End If
        Next

        If cmbAdvTemplateType.Items.Count > 0 Then
            cmbAdvTemplateType.SelectedItem = cmbAdvTemplateType.Items(0)
            cmbAdvTemplateName.Items.Clear()

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export IsNot Nothing AndAlso x.Export.Id = cmbAdvTemplateType.Items(0).Value)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If

            If cmbAdvTemplateName.Items.Count > 0 AndAlso cmbAdvTemplateName.SelectedItem Is Nothing Then
                cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items(0)
            End If
        Else
            cmbAdvTemplateType.Items.Add(New ListEditItem("", -1))
            cmbAdvTemplateType.SelectedItem = cmbAdvTemplateType.Items(0)
            cmbAdvTemplateName.Items.Add("", -1)
            cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items(0)
        End If

        LoadTemplateIntoExcel()


        SetControlsVisibility(oParameters, oCurrentGuide)

        Return bResult
    End Function

    Private Function LoadExportTemplatesAvailable(ByVal overrideTemplateSelected As String, ByVal oParameters As ObjectCallbackRequest) As String
        Dim bResult As String = "ACTION"
        Dim oCurrentGuide As roDatalinkGuide = Nothing
        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
        oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

        For Each oElement In oList
            If oElement.Export IsNot Nothing AndAlso oElement.Export.Templates IsNot Nothing Then
                Dim bInclude As Boolean = False
                For Each oTemplate In oElement.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then bInclude = True
                Next

                If bInclude Then
                    If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                        cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                    End If
                End If
            End If
        Next

        Dim selTemplateType As String = String.Empty
        If cmbAdvTemplateType.SelectedItem IsNot Nothing Then
            selTemplateType = cmbAdvTemplateType.SelectedItem.Value
        End If

        If cmbAdvTemplateType.Items.Count > 0 AndAlso selTemplateType <> String.Empty Then
            cmbAdvTemplateName.Items.Clear()

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export IsNot Nothing AndAlso x.Export.Id = selTemplateType)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If

            If overrideTemplateSelected <> String.Empty Then
                cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items.FindByText(overrideTemplateSelected)
            Else
                If cmbAdvTemplateName.Items.Count > 0 AndAlso cmbAdvTemplateName.SelectedItem Is Nothing Then
                    cmbAdvTemplateName.SelectedItem = cmbAdvTemplateName.Items(0)
                End If
            End If
        End If

        LoadTemplateIntoExcel()

        SetControlsVisibility(oParameters, oCurrentGuide)

        Return bResult
    End Function

    Private Function LoadFileTemplateContent(ByVal oParameters As ObjectCallbackRequest) As String
        Dim bResult As String = "ACTION"

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)
        Dim oCurrentGuide As roDatalinkGuide = Nothing
        Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), oParameters.Concept), roDatalinkConcept)
        oCurrentGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Me, eConcept, True)

        'For Each oElement In oList

        '    Dim bInclude As Boolean = False
        '    For Each oTemplate In oElement.Export.Templates
        '        If oTemplate.TemplateFile <> String.Empty Then bInclude = True
        '    Next

        '    If bInclude Then
        '        If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
        '            cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
        '        End If
        '    End If
        'Next

        'Dim selTemplateType As String = String.Empty
        'If cmbAdvTemplateType.SelectedItem IsNot Nothing Then
        '    selTemplateType = cmbAdvTemplateType.SelectedItem.Value
        'End If

        'If cmbAdvTemplateType.Items.Count > 0 AndAlso selTemplateType <> String.Empty Then

        '    Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export.Id = selTemplateType)
        '    If oGuide IsNot Nothing Then
        '        For Each oTemplate In oGuide.Export.Templates
        '            If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
        '        Next
        '    End If
        'End If

        LoadTemplateIntoExcel()

        SetControlsVisibility(oParameters, oCurrentGuide)

        Return bResult
    End Function

    Public Sub LoadTemplateIntoExcel()
        If cmbAdvTemplateType.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateName.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateType.SelectedItem.Value >= 0 Then
            Dim bContent As Byte() = {}
            bContent = API.DataLinkGuideServiceMethods.GetExportTemplateBytes(Nothing, roTypes.Any2Integer(cmbAdvTemplateType.SelectedItem.Value), roTypes.Any2Integer(cmbAdvTemplateName.SelectedItem.Value))

            If bContent IsNot Nothing AndAlso bContent.Length > 0 Then
                Dim strTempFileName = System.IO.Path.GetTempFileName()

                System.IO.File.WriteAllBytes(strTempFileName, bContent)
                DevExpress.Web.Office.DocumentManager.CloseAllDocuments()
                advTemplate.Open(strTempFileName)
            End If

        Else
            advTemplate.CreateDefaultRibbonTabs(True)
            HideribbonTab(advTemplate, 0)
            HideribbonTab(advTemplate, 1)
            advTemplate.Document.CreateNewDocument()

        End If

    End Sub

    Public Sub CallbackExcel_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles CallbackExcel.Callback
        Dim bRet As Boolean = False
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim oList As Generic.List(Of roDatalinkGuide) = API.DataLinkGuideServiceMethods.GetDatalinkGuides(Nothing, False)

        For Each oElement In oList

            If oElement.Export IsNot Nothing AndAlso oElement.Export.Templates IsNot Nothing Then
                Dim bInclude As Boolean = False
                For Each oTemplate In oElement.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then bInclude = True
                Next

                If bInclude Then
                    If oElement.Export IsNot Nothing AndAlso oElement.Export.Id > 0 Then
                        cmbAdvTemplateType.Items.Add(New ListEditItem(oElement.Name, oElement.Export.Id))
                    End If
                End If
            End If

        Next

        Dim selTemplateType As String = String.Empty
        If cmbAdvTemplateType.SelectedItem IsNot Nothing Then
            selTemplateType = cmbAdvTemplateType.SelectedItem.Value
        End If

        If cmbAdvTemplateType.Items.Count > 0 AndAlso selTemplateType <> String.Empty Then

            Dim oGuide As roDatalinkGuide = oList.Find(Function(x) x.Export IsNot Nothing AndAlso x.Export.Id = selTemplateType)
            If oGuide IsNot Nothing Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then cmbAdvTemplateName.Items.Add(oTemplate.Name, oTemplate.ID)
                Next
            End If
        End If

        If oParameters.Action = "SAVEEDITOR" Then
            If cmbAdvTemplateType.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateName.SelectedItem IsNot Nothing Then

                advTemplate.Document.SaveDocument(advTemplate.Document.Path)
                Dim bTemplateContent As Byte() = System.IO.File.ReadAllBytes(advTemplate.Document.Path)
                bRet = API.DataLinkGuideServiceMethods.SaveExportTemplateBytes(Nothing, bTemplateContent, roTypes.Any2Integer(cmbAdvTemplateType.SelectedItem.Value), roTypes.Any2Integer(cmbAdvTemplateName.SelectedItem.Value))
            End If
        ElseIf oParameters.Action = "DUPLICATE" Then
            If cmbAdvTemplateType.SelectedItem IsNot Nothing AndAlso cmbAdvTemplateName.SelectedItem IsNot Nothing Then
                advTemplate.Document.SaveDocument(advTemplate.Document.Path)
                Dim bTemplateContent As Byte() = System.IO.File.ReadAllBytes(advTemplate.Document.Path)
                bRet = API.DataLinkGuideServiceMethods.DuplicateExportTemplateBytes(Nothing, bTemplateContent, roTypes.Any2Integer(cmbAdvTemplateType.SelectedItem.Value), roTypes.Any2Integer(cmbAdvTemplateName.SelectedItem.Value), oParameters.NewTemplateName)
            End If
        End If

        CallbackExcel.JSProperties.Add("cpActionRO", oParameters.Action)
        CallbackExcel.JSProperties.Add("cpResultRO", If(bRet, "OK", "ERROR"))

    End Sub

#End Region



End Class