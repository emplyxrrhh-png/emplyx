Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessPeriod
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class AccessPeriods
    Inherits PageBase

    Private oPermission As Permission

    <Runtime.Serialization.DataContract()>
    Private Class ObjectCallbackRequest

        <Runtime.Serialization.DataMember(Name:="aTab")>
        Public aTab As Integer

        <Runtime.Serialization.DataMember(Name:="ID")>
        Public ID As Integer

        <Runtime.Serialization.DataMember(Name:="Name")>
        Public Name As String

        <Runtime.Serialization.DataMember(Name:="action")>
        Public Action As String

        <Runtime.Serialization.DataMember(Name:="StampParam")>
        Public StampParam As Double

        <Runtime.Serialization.DataMember(Name:="gridPeriods")>
        Public GridPeriods As String

    End Class

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Access.Groups.Definition")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("frmNewPeriod", "~/Access/Scripts/frmNewPeriod.js")
        Me.InsertExtraJavascript("AccessPeriod", "~/Access/Scripts/AccessPeriod.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\Access") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesAccessPeriods.TreeCaption = Me.Language.Translate("TreeCaptionPeriods", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)
        Me.oPermission = Me.GetFeaturePermission("Access.Periods.Definition")
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
        Else
            hdnModeEdit.Value = "false"
        End If

        Dim dTbl As DataTable = AccessPeriodServiceMethods.GetAccessPeriods(Me.Page)
        If dTbl.Rows.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        If Not Me.IsPostBack Then
            loadFormNewAccPermission()
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean

        Select Case oParameters.Action

            Case "GETACCESSPERIOD"
                bRet = LoadAccessPeriod(oParameters.ID)
            Case "SAVEACCESSPERIOD"
                bRet = SaveAccessPeriod(oParameters)
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadAccessPeriod(ByRef IdAccessPeriod As Integer, Optional ByVal eCurrentAccessPeriod As roAccessPeriod = Nothing) As Boolean
        Dim bRet As Boolean = False

        Dim ErrorInfo As String = String.Empty
        Dim GridsJSON As String = String.Empty
        Dim oCurrentAccessPeriod As roAccessPeriod = Nothing
        Try

            If Me.oPermission > Permission.None Then

                If eCurrentAccessPeriod IsNot Nothing Then
                    oCurrentAccessPeriod = eCurrentAccessPeriod
                Else
                    If IdAccessPeriod = -1 Then
                        oCurrentAccessPeriod = New roAccessPeriod
                        oCurrentAccessPeriod.ID = -1
                    Else
                        oCurrentAccessPeriod = AccessPeriodServiceMethods.GetAccessPeriodByID(Me.Page, IdAccessPeriod, True)
                    End If
                End If

                txtName.Text = oCurrentAccessPeriod.Name

                GridsJSON = CreateGridsJSON(oCurrentAccessPeriod)

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETACCESSPERIOD")
            ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
            ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", ErrorInfo)
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentAccessPeriod.Name)
            ASPxCallbackPanelContenido.JSProperties.Add("cpGridsJSON", GridsJSON)
        End Try

        Return bRet

    End Function

    Private Function GetDateFromStringHour(strHour As String) As DateTime
        Dim dt As DateTime
        Dim dHour As String = strHour.Split(":")(0)
        Dim dMinute As String = strHour.Split(":")(1)
        dt = New Date(1899, 12, 30, dHour, dMinute, 0)
        Return dt
    End Function

    Private Function SaveAccessPeriod(ByRef oParameters As ObjectCallbackRequest) As Boolean
        Dim bRet As Boolean = False

        Dim ErrorInfo As String = String.Empty
        Dim oCurrentAccessPeriod As roAccessPeriod = Nothing
        Try

            If Me.oPermission >= Permission.Write Then

                oCurrentAccessPeriod = AccessPeriodServiceMethods.GetAccessPeriodByID(Me.Page, oParameters.ID, False)
                If Not oCurrentAccessPeriod Is Nothing Then

                    oCurrentAccessPeriod.ID = oParameters.ID
                    oCurrentAccessPeriod.Name = txtName.Text

                    If Not String.IsNullOrEmpty(oParameters.GridPeriods) Then

                        'Carrega de les zones en el ZoneException  ------------------------------------------
                        Dim resultDaily As New Generic.SortedList(Of String, roAccessPeriodDaily)
                        Dim resultHolidays As New Generic.SortedList(Of String, roAccessPeriodHolidays)

                        Dim strAux() As String
                        Dim strKey As String
                        Dim GridPeriods() As String = Server.UrlDecode(oParameters.GridPeriods).Split(";")
                        For Each item As String In GridPeriods

                            strAux = item.Split("#")
                            strKey = strAux(2) & strAux(3) & strAux(4) & strAux(5) & strAux(6)
                            If strAux(2) <> "" Then
                                If Not resultDaily.ContainsKey(strKey) Then
                                    Dim oAccessPeriodDaily As New roAccessPeriodDaily
                                    oAccessPeriodDaily.IDAccessPeriod = oCurrentAccessPeriod.ID
                                    oAccessPeriodDaily.BeginTime = GetDateFromStringHour(strAux(3))
                                    oAccessPeriodDaily.EndTime = GetDateFromStringHour(strAux(4))
                                    oAccessPeriodDaily.DayofWeek = CInt(strAux(2))
                                    resultDaily.Add(strKey, oAccessPeriodDaily)
                                End If
                            Else
                                If Not resultHolidays.ContainsKey(strKey) Then
                                    Dim oAccessPeriodHolidays As New roAccessPeriodHolidays
                                    oAccessPeriodHolidays.IDAccessPeriod = oCurrentAccessPeriod.ID
                                    oAccessPeriodHolidays.Day = CInt(strAux(5)) 'Day
                                    oAccessPeriodHolidays.Month = CInt(strAux(6)) 'Month
                                    oAccessPeriodHolidays.BeginTime = GetDateFromStringHour(strAux(3)) 'Begintime
                                    oAccessPeriodHolidays.EndTime = GetDateFromStringHour(strAux(4)) 'Endtime
                                    resultHolidays.Add(strKey, oAccessPeriodHolidays)
                                End If
                            End If

                        Next

                        Dim lstAccessPeriodDaily As New Generic.List(Of roAccessPeriodDaily)(resultDaily.Values)
                        oCurrentAccessPeriod.AccessPeriodDaily = lstAccessPeriodDaily

                        Dim lstAccessPeriodHolidays As New Generic.List(Of roAccessPeriodHolidays)(resultHolidays.Values)
                        oCurrentAccessPeriod.AccessPeriodHolidays = lstAccessPeriodHolidays
                    Else
                        oCurrentAccessPeriod.AccessPeriodHolidays = Nothing
                        oCurrentAccessPeriod.AccessPeriodDaily = Nothing
                    End If

                    bRet = AccessPeriodServiceMethods.SaveAccessPeriod(Me.Page, oCurrentAccessPeriod, True)
                    If bRet Then
                        Dim treePath As String = "/source/" & oCurrentAccessPeriod.ID
                        HelperWeb.roSelector_SetSelection(oCurrentAccessPeriod.ID.ToString, treePath, "ctl00_contentMainBody_roTreesAccessPeriods")
                    Else
                        ErrorInfo = AccessPeriodServiceMethods.LastErrorText
                    End If
                Else
                    ErrorInfo = AccessPeriodServiceMethods.LastErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        Finally
            If bRet = True Then
                LoadAccessPeriod(oParameters.ID, oCurrentAccessPeriod)
                ASPxCallbackPanelContenido.JSProperties("cpIsNew") = True
            Else
                bRet = LoadAccessPeriod(-1, oCurrentAccessPeriod)
                ASPxCallbackPanelContenido.JSProperties("cpAction") = "SAVEACCESSPERIOD"
                ASPxCallbackPanelContenido.JSProperties("cpResult") = "NOK"
                ASPxCallbackPanelContenido.JSProperties("cpMessage") = ErrorInfo
            End If
        End Try

        Return bRet

    End Function

    Private Function CreateGridsJSON(ByRef oCurrentAccessPeriod As roAccessPeriod) As String
        Try

            Dim oJGDaily As New Generic.List(Of Object)
            Dim oJGHoliday As New Generic.List(Of Object)

            Dim oJFDaily As Generic.List(Of JSONFieldItem)
            Dim oJFHoliday As Generic.List(Of JSONFieldItem)

            Dim strJSONGroups As String = ""

            If oCurrentAccessPeriod.AccessPeriodDaily IsNot Nothing Then
                If oCurrentAccessPeriod.AccessPeriodDaily.Count > 0 Then
                    For Each oAccessPeriodDaily As roAccessPeriodDaily In oCurrentAccessPeriod.AccessPeriodDaily
                        oJFDaily = New Generic.List(Of JSONFieldItem)
                        oJFDaily.Add(New JSONFieldItem("IDAccessPeriod", oAccessPeriodDaily.IDAccessPeriod, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFDaily.Add(New JSONFieldItem("Description", oAccessPeriodDaily.Description, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFDaily.Add(New JSONFieldItem("DayofWeek", oAccessPeriodDaily.DayofWeek, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFDaily.Add(New JSONFieldItem("BeginTime", Format(oAccessPeriodDaily.BeginTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFDaily.Add(New JSONFieldItem("EndTime", Format(oAccessPeriodDaily.EndTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFDaily.Add(New JSONFieldItem("Day", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFDaily.Add(New JSONFieldItem("Month", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJGDaily.Add(oJFDaily)
                    Next
                End If
            End If

            If oCurrentAccessPeriod.AccessPeriodHolidays IsNot Nothing Then
                If oCurrentAccessPeriod.AccessPeriodHolidays.Count > 0 Then
                    For Each oAccessPeriodHoliday As roAccessPeriodHolidays In oCurrentAccessPeriod.AccessPeriodHolidays
                        oJFHoliday = New Generic.List(Of JSONFieldItem)
                        oJFHoliday.Add(New JSONFieldItem("IDAccessPeriod", oAccessPeriodHoliday.IDAccessPeriod, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFHoliday.Add(New JSONFieldItem("Description", oAccessPeriodHoliday.Description, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFHoliday.Add(New JSONFieldItem("DayofWeek", "", Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFHoliday.Add(New JSONFieldItem("BeginTime", Format(oAccessPeriodHoliday.BeginTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFHoliday.Add(New JSONFieldItem("EndTime", Format(oAccessPeriodHoliday.EndTime, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFHoliday.Add(New JSONFieldItem("Day", oAccessPeriodHoliday.Day, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFHoliday.Add(New JSONFieldItem("Month", oAccessPeriodHoliday.Month, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJGHoliday.Add(oJFHoliday)
                    Next
                End If
            End If

            strJSONGroups = "{""grid"":["

            Dim strJSONText As String = ""
            For Each oObj As Object In oJGDaily
                strJSONText = "{""fields"":" & roJSONHelper.Serialize(oObj) & " },"
                strJSONGroups &= strJSONText
            Next

            For Each oObj As Object In oJGHoliday
                strJSONText = "{""fields"":" & roJSONHelper.Serialize(oObj) & " },"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]}"

            Return "[" & strJSONGroups & "]"
        Catch ex As Exception
            Return String.Empty
        End Try

    End Function

    Private Sub loadFormNewAccPermission()
        Try

            Dim obj As Object = Me.frmNewPeriod1.FindControl("opTypePeriodNormal")
            Dim cmbWeekDay As DevExpress.Web.ASPxComboBox = obj.FindControl("cmbWeekDay")
            cmbWeekDay.Items.Clear()
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(" ", "0"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Monday", DefaultScope), "1"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Tuesday", DefaultScope), "2"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Wednesday", DefaultScope), "3"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Thursday", DefaultScope), "4"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Friday", DefaultScope), "5"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Saturday", DefaultScope), "6"))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Sunday", DefaultScope), "7"))
            cmbWeekDay.SelectedIndex = 0

            obj = Me.frmNewPeriod1.FindControl("opTypePeriodEspecific")
            Dim cmbMonths As DevExpress.Web.ASPxComboBox = obj.FindControl("cmbMonths")
            cmbMonths.Items.Clear()
            cmbMonths.Items.Add(New DevExpress.Web.ListEditItem(" ", "0"))
            For n As Integer = 1 To 12
                cmbMonths.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Keyword("month." & n.ToString), n.ToString))
            Next
            cmbMonths.SelectedIndex = 0
        Catch ex As Exception
        End Try
    End Sub

End Class