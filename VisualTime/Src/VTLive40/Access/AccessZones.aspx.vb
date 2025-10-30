Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Zone
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class AccessZones
    Inherits PageBase

    Private oPermission As Permission

    Private arrWeekDayNames() As String = {"", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"}

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

        <Runtime.Serialization.DataMember(Name:="gridExceptions")>
        Public GridExceptions As String

        <Runtime.Serialization.DataMember(Name:="gridPeriods")>
        Public GridPeriods As String

    End Class

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Access.Zones.Definition")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("flLocationMap", "~/Access/Scripts/flLocationMap.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("frmAddException", "~/Access/Scripts/frmAddException.js")
        Me.InsertExtraJavascript("frmAddPeriod", "~/Access/Scripts/frmAddPeriod.js")
        Me.InsertExtraJavascript("AccessZone", "~/Access/Scripts/AccessZone.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        roTreesAccessZones.TreeCaption = Me.Language.Translate("TreeCaptionAccessZones", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
            cmbZonePlaneMain.Enabled = False
            dxColorPicker.Enabled = False
            cmbZonePlane.Enabled = False
            cmbCamera.Enabled = False
            txtDescription.ReadOnly = True
            optList.ReadOnly = True
        End If

        If Not Me.IsPostBack And Not Me.IsCallback Then
            If Me.oPermission < Permission.Write Then
                hdnModeEdit.Value = "true"
            Else
                hdnModeEdit.Value = "false"
            End If

            Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Me.Page)
            If dTbl.Rows.Count = 0 Then
                Me.noRegs.Value = "1"
            Else
                Me.noRegs.Value = ""
            End If

            Me.dateConfig.Clear()
            Me.dateConfig.Add("Format", HelperWeb.GetShortDateFormat())
            Me.dateConfig.Add("Separator", HelperWeb.GetShortDateSeparator())

            HelperSession.DeleteEmployeeGroupsFromApplication()

            Me.LoadAccessZonesData()

            Me.LoadZonePlane()

            Me.LoadZonePlaneMain()

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

            Case "GETACCESSZONEMAIN"

                Me.LoadZonePlaneMain()

                Dim PositionParam As String = String.Empty
                Dim ImageIdParam As String = String.Empty
                bRet = LoadAccessZoneMain(oParameters.ID, responseMessage, PositionParam, ImageIdParam)

                Me.hdnManagePlane.Set("PositionMain", PositionParam)
                Me.hdnManagePlane.Set("ImageIDMain", ImageIdParam)

                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETACCESSZONEMAIN")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If

            Case "GETACCESSZONE"
                LoadAccessZoneCallback(oParameters)
            Case "SAVEACCESSZONE"
                Me.LoadZonePlane()
                bRet = SaveAccessZone(oParameters, responseMessage)

                If bRet Then
                    LoadAccessZoneCallback(oParameters)
                    ASPxCallbackPanelContenido.JSProperties("cpIsNew") = True
                Else
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVEACCESSZONE")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                    If Not bRet Then
                        ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                    End If
                End If
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Sub LoadAccessZoneCallback(ByVal oParameters As ObjectCallbackRequest)
        Me.LoadZonePlane()

        Dim responseMessage = String.Empty
        Dim GridsJSON As String = String.Empty
        Dim bRet As Boolean = LoadAccessZone(oParameters.ID, GridsJSON, responseMessage)

        ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETACCESSZONE")
        ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
        If Not bRet Then
            ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
        End If
        ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
        ASPxCallbackPanelContenido.JSProperties.Add("cpGridsJSON", GridsJSON)
        ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
    End Sub

    Private Function LoadAccessZoneMain(ByRef IdAccessPlane As Integer, ByRef ErrorInfo As String, ByRef PositionParam As String, ByRef ImageIdParam As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim strPosition As String = ""

                Dim oZones As Generic.List(Of roZone) = API.ZoneServiceMethods.GetZonesFromPlane(Me.Page, IdAccessPlane, False)
                For Each oZone As roZone In oZones
                    If oZone.X1 <> -1 Then
                        Dim auxColor As System.Drawing.Color = System.Drawing.ColorTranslator.FromWin32(oZone.Color)
                        Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(auxColor)
                        '3,Zona 1,40,158,52,167,CCCC00,0,0,
                        strPosition &= oZone.ID & "," & oZone.Name & "," & oZone.X1 & "," & oZone.Y1 & "," & oZone.X2 & "," & oZone.Y2 & "," & oHTMLColor.Replace("#", "") & ",0,0,"
                    End If
                Next

                If strPosition <> "" Then strPosition = strPosition.Substring(0, Len(strPosition) - 1)

                PositionParam = strPosition
                ImageIdParam = IdAccessPlane

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function LoadAccessZone(ByRef IdAccessZone As Integer, ByRef GridsJSON As String, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim oCurrentAccessZone As roZone

                If IdAccessZone = -1 Then
                    oCurrentAccessZone = New roZone
                    oCurrentAccessZone.ID = -1
                Else
                    oCurrentAccessZone = ZoneServiceMethods.GetZoneByID(Me.Page, IdAccessZone, True)
                End If

                Me.txtName.Text = oCurrentAccessZone.Name
                Me.dxColorPicker.Color = System.Drawing.ColorTranslator.FromWin32(oCurrentAccessZone.Color)
                Me.txtDescription.Value = oCurrentAccessZone.Description

                If oCurrentAccessZone.IDCamera.HasValue Then
                    Dim it As DevExpress.Web.ListEditItem = Me.cmbCamera.Items.FindByValue(oCurrentAccessZone.IDCamera)
                    If it Is Nothing Then
                        Me.cmbCamera.SelectedIndex = 0
                    Else
                        Me.cmbCamera.SelectedItem = it
                    End If
                Else
                    Me.cmbCamera.SelectedIndex = 0
                End If

                If oCurrentAccessZone.IDPlane.HasValue Then
                    Dim it As DevExpress.Web.ListEditItem = Me.cmbZonePlane.Items.FindByValue(oCurrentAccessZone.IDPlane)
                    If it Is Nothing Then
                        Me.cmbZonePlane.SelectedIndex = 0
                    Else
                        Me.cmbZonePlane.SelectedItem = it
                    End If
                Else
                    Me.cmbZonePlane.SelectedIndex = 0
                End If

                If oCurrentAccessZone.DefaultTimezone <> String.Empty Then
                    Me.cmbDefaultTimeZone.SelectedItem = cmbDefaultTimeZone.Items.FindByValue(oCurrentAccessZone.DefaultTimezone)
                Else
                    Me.cmbDefaultTimeZone.SelectedItem = cmbDefaultTimeZone.Items.FindByValue("")
                End If

                Dim strPosition As String = ""
                If oCurrentAccessZone.X1 <> -1 Then
                    Dim oHTMLColor As String = System.Drawing.ColorTranslator.ToHtml(Me.dxColorPicker.Color)
                    strPosition = oCurrentAccessZone.X1 & "," & oCurrentAccessZone.Y1 & "," & oCurrentAccessZone.X2 & "," & oCurrentAccessZone.Y2 & "," & "false," & oHTMLColor.Replace("#", "")
                End If
                Me.hdnManagePlane.Set("Position", strPosition)

                Dim imgID As String = ""
                If oCurrentAccessZone.IDPlane.HasValue Then
                    imgID = oCurrentAccessZone.IDPlane.Value
                End If
                Me.hdnManagePlane.Set("ImageID", imgID)

                If oCurrentAccessZone.IsWorkingZone Then
                    optList.SelectedIndex = 1
                Else
                    optList.SelectedIndex = 0
                End If

                GridsJSON = CreateGridsJSON(oCurrentAccessZone)

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function SaveAccessZone(ByRef oParameters As ObjectCallbackRequest, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission >= Permission.Write Then

                Dim oCurrentAccessZone As roZone

                If oParameters.ID = -1 Then
                    oCurrentAccessZone = New roZone()
                    oCurrentAccessZone.ID = -1
                Else
                    oCurrentAccessZone = ZoneServiceMethods.GetZoneByID(Me.Page, oParameters.ID, False)
                End If

                If Not oCurrentAccessZone Is Nothing Then

                    oCurrentAccessZone.ID = oParameters.ID
                    oCurrentAccessZone.Name = txtName.Text

                    oCurrentAccessZone.Color = Drawing.ColorTranslator.ToWin32(Me.dxColorPicker.Color)
                    oCurrentAccessZone.Description = Me.txtDescription.Value

                    If cmbCamera.SelectedItem.Value <> 0 Then
                        oCurrentAccessZone.IDCamera = cmbCamera.SelectedItem.Value
                    End If

                    If cmbZonePlane.SelectedItem.Value <> 0 Then
                        oCurrentAccessZone.IDPlane = cmbZonePlane.SelectedItem.Value
                    Else
                        oCurrentAccessZone.IDPlane = Nothing
                    End If

                    If Me.optList.SelectedIndex = 1 Then
                        oCurrentAccessZone.IsWorkingZone = True
                    Else
                        oCurrentAccessZone.IsWorkingZone = False
                    End If

                    oCurrentAccessZone.DefaultTimezone = cmbDefaultTimeZone.SelectedItem.Value

                    If roTypes.Any2String(hdnManagePlane.Get("Position")) <> String.Empty Then
                        Dim arrPos() As String = roTypes.Any2String(hdnManagePlane.Get("Position")).Split(",")
                        oCurrentAccessZone.X1 = arrPos(0)
                        oCurrentAccessZone.Y1 = arrPos(1)
                        oCurrentAccessZone.X2 = arrPos(2)
                        oCurrentAccessZone.Y2 = arrPos(3)
                    End If

                    'TODO: Aixo sols actualment, en un futur jerarquic cambiar...
                    oCurrentAccessZone.ParentZone = API.ZoneServiceMethods.GetZoneByID(Me.Page, retParentZoneID, False)
                    oCurrentAccessZone.Proportion = 1

                    Dim strAux() As String
                    Dim strKey As String
                    Dim GridRows() As String

                    'Grids Exceptions
                    If Not String.IsNullOrEmpty(oParameters.GridExceptions) Then
                        Dim resultExceptions As New Generic.SortedList(Of String, roZoneException)
                        GridRows = Server.UrlDecode(oParameters.GridExceptions).Split(";")
                        For Each item As String In GridRows
                            strAux = item.Split("#")
                            strKey = strAux(1)
                            If Not resultExceptions.ContainsKey(strKey) Then
                                resultExceptions.Add(strKey, New roZoneException() With {.IDZone = oCurrentAccessZone.ID, .ExceptionDate = strAux(1)})
                            End If
                        Next
                        Dim lstExceptions As New Generic.List(Of roZoneException)(resultExceptions.Values)
                        oCurrentAccessZone.ZonesException = lstExceptions
                    Else
                        oCurrentAccessZone.ZonesException = New Generic.List(Of roZoneException)
                    End If

                    'Grids Periods
                    If Not String.IsNullOrEmpty(oParameters.GridPeriods) Then
                        Dim resultPeriods As New Generic.SortedList(Of String, roZoneInactivity)
                        GridRows = Server.UrlDecode(oParameters.GridPeriods).Split(";")
                        For Each item As String In GridRows
                            strAux = item.Split("#")
                            strKey = strAux(0) & strAux(1) & strAux(2) & strAux(3) & strAux(4)
                            If Not resultPeriods.ContainsKey(strKey) Then

                                Dim dBegin As DateTime
                                Dim dEnd As DateTime

                                Dim dHour As String = strAux(3).Split(":")(0)
                                Dim dMinute As String = strAux(3).Split(":")(1)
                                dBegin = New Date(1950, 12, 31, dHour, dMinute, 0)

                                dHour = strAux(4).Split(":")(0)
                                dMinute = strAux(4).Split(":")(1)
                                dEnd = New Date(1950, 12, 31, dHour, dMinute, 0)

                                resultPeriods.Add(strKey, New roZoneInactivity() With {.IDZone = oCurrentAccessZone.ID, .WeekDay = strAux(2), .Begin = dBegin, .End = dEnd})
                            End If
                        Next
                        Dim lstPeriods As New Generic.List(Of roZoneInactivity)(resultPeriods.Values)
                        oCurrentAccessZone.ZonesInactivity = lstPeriods
                    Else
                        oCurrentAccessZone.ZonesInactivity = New Generic.List(Of roZoneInactivity)
                    End If

                    bRet = ZoneServiceMethods.SaveZone(Me.Page, oCurrentAccessZone, True)
                    If bRet Then
                        Dim treePath As String = "/source/"
                        If oCurrentAccessZone.ParentZone.ID > 0 Then treePath &= "A" & oCurrentAccessZone.ParentZone.ID & "/"
                        treePath &= "B" & oCurrentAccessZone.ID
                        HelperWeb.roSelector_SetSelection("B" & oCurrentAccessZone.ID.ToString, treePath, "ctl00_contentMainBody_roTreesAccessZones")
                        oParameters.ID = oCurrentAccessZone.ID
                    Else
                        ErrorInfo = ZoneServiceMethods.LastErrorText
                    End If
                Else
                    ErrorInfo = ZoneServiceMethods.LastErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function retParentZoneID() As Integer
        Dim intReturn As Integer = -1

        Try
            Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Me.Page)
            Dim dRows() As DataRow = dTbl.Select("IDParent IS NULL")
            If dRows.Length > 0 Then
                intReturn = dRows(0)("ID")
            End If
        Catch ex As Exception
        End Try

        Return intReturn
    End Function

    Private Function CreateGridsJSON(ByRef oCurrentAccessZone As roZone) As String
        Try

            Dim oJGException As New Generic.List(Of Object)
            Dim oJGPeriods As New Generic.List(Of Object)

            Dim oJFExceptions As Generic.List(Of JSONFieldItem)
            Dim oJFPeriods As Generic.List(Of JSONFieldItem)

            If oCurrentAccessZone.ZonesException IsNot Nothing Then
                If oCurrentAccessZone.ZonesException.Count > 0 Then
                    For Each oZoneException As roZoneException In oCurrentAccessZone.ZonesException
                        oJFExceptions = New Generic.List(Of JSONFieldItem)
                        oJFExceptions.Add(New JSONFieldItem("IDZone", oZoneException.IDZone, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFExceptions.Add(New JSONFieldItem("ExceptionDate", oZoneException.ExceptionDate, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJGException.Add(oJFExceptions)
                    Next
                End If
            End If

            If oCurrentAccessZone.ZonesInactivity IsNot Nothing Then
                If oCurrentAccessZone.ZonesInactivity.Count > 0 Then
                    For Each oZoneInactivity As roZoneInactivity In oCurrentAccessZone.ZonesInactivity
                        oJFPeriods = New Generic.List(Of JSONFieldItem)

                        oJFPeriods.Add(New JSONFieldItem("IDZone", oZoneInactivity.IDZone, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFPeriods.Add(New JSONFieldItem("WeekDayName", Me.Language.Translate("WeekDayNames." & arrWeekDayNames(oZoneInactivity.WeekDay), DefaultScope), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFPeriods.Add(New JSONFieldItem("WeekDay", oZoneInactivity.WeekDay, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFPeriods.Add(New JSONFieldItem("Begin", Format(oZoneInactivity.Begin, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFPeriods.Add(New JSONFieldItem("End", Format(oZoneInactivity.End, "HH:mm"), Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        oJGPeriods.Add(oJFPeriods)
                    Next
                End If
            End If

            Dim strJSONGroups As String = "{""exceptions"":["

            For Each oObj As Object In oJGException
                Dim strJSONText As String = ""
                strJSONText &= "{""fields"":"
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= "},"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]},"

            strJSONGroups &= "{""periods"":["
            For Each oObj As Object In oJGPeriods
                Dim strJSONText As String = ""
                strJSONText &= "{ ""fields"":"
                strJSONText &= roJSONHelper.Serialize(oObj)
                strJSONText &= "},"
                strJSONGroups &= strJSONText
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "]}"

            Return "[" & strJSONGroups & "]"
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Private Sub LoadZonePlane(Optional nValue As Integer = -1)
        Me.cmbZonePlane.Items.Clear()
        Me.cmbZonePlane.ValueType = GetType(Integer)
        Me.cmbZonePlane.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("NoPlane", DefaultScope), 0))
        Dim dTblZP As DataTable = API.ZoneServiceMethods.GetZonePlanes(Me.Page)
        For Each dRow As DataRow In dTblZP.Rows
            Me.cmbZonePlane.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
        Next

    End Sub

    Private Sub LoadZonePlaneMain(Optional nValue As Integer = -1)
        Me.cmbZonePlaneMain.Items.Clear()
        Me.cmbZonePlaneMain.ValueType = GetType(Integer)
        Me.cmbZonePlaneMain.Items.Add(New DevExpress.Web.ListEditItem("", 0))
        Dim dTblZP As DataTable = API.ZoneServiceMethods.GetZonePlanes(Me.Page)
        For Each dRow As DataRow In dTblZP.Rows
            Me.cmbZonePlaneMain.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
        Next
        Me.cmbZonePlaneMain.SelectedIndex = 0

        If nValue > 0 Then
            Dim it As DevExpress.Web.ListEditItem = Me.cmbZonePlaneMain.Items.FindByValue(nValue)
            If it Is Nothing Then
                Me.cmbZonePlane.SelectedIndex = 0
            Else
                Me.cmbZonePlane.SelectedItem = it
            End If
        End If

    End Sub

    Private Sub LoadAccessZonesData()

        Me.loadFormAddPeriod()

        Me.cmbCamera.Items.Clear()
        Me.cmbCamera.ValueType = GetType(Integer)
        Me.cmbCamera.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("NoCamera", DefaultScope), 0))
        Dim dTblCam As DataTable = API.CameraServiceMethods.GetCameras(Me.Page)
        For Each dRow As DataRow In dTblCam.Rows
            Me.cmbCamera.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
        Next

        Me.optList.Items.Clear()
        Me.optList.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("optList.Absence", DefaultScope), "0"))
        Me.optList.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("optList.Presence", DefaultScope), "1"))

        Me.cmbDefaultTimeZone.Items.Clear()

        Dim oTimeZones As ObjectModel.ReadOnlyCollection(Of TimeZoneInfo) = TimeZoneInfo.GetSystemTimeZones()

        Me.cmbDefaultTimeZone.Items.Clear()
        Me.cmbDefaultTimeZone.Items.Add("", "")
        For Each oTimeZone As TimeZoneInfo In oTimeZones
            Me.cmbDefaultTimeZone.Items.Add(oTimeZone.DisplayName, oTimeZone.Id)
        Next

        If Me.oPermission <> Permission.Admin Then
            Me.AddZoneExceptionBtn.Visible = False
            Me.AddZoneInactivityBtn.Visible = False
        End If

    End Sub

    Private Sub loadFormAddPeriod()
        Try

            Dim cmbWeekDay As DevExpress.Web.ASPxComboBox = Me.frmAddPeriod1.FindControl("cmbPeriodWeekDay")
            cmbWeekDay.Items.Clear()
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(" ", 0))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Monday", DefaultScope), 1))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Tuesday", DefaultScope), 2))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Wednesday", DefaultScope), 3))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Thursday", DefaultScope), 4))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Friday", DefaultScope), 5))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Saturday", DefaultScope), 6))
            cmbWeekDay.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("WeekDayNames.Sunday", DefaultScope), 7))
            cmbWeekDay.SelectedIndex = 0
        Catch ex As Exception
        End Try
    End Sub

    Protected Sub cmbZonePlane_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles cmbZonePlane.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)
        Dim nValue As Integer = roTypes.Any2Integer(strParameter.Substring(strParameter.IndexOf("=") + 1))
        Me.LoadZonePlane(nValue)

        Me.cmbZonePlane.SelectedIndex = 0

        If nValue > 0 Then
            Dim it As DevExpress.Web.ListEditItem = Me.cmbZonePlane.Items.FindByValue(nValue)
            If it Is Nothing Then
                Me.cmbZonePlane.SelectedIndex = 0
            Else
                Me.cmbZonePlane.SelectedItem = it
            End If
        End If

        cmbZonePlane.JSProperties.Add("cpValue", cmbZonePlane.SelectedItem.Value)
    End Sub

    Protected Sub cmbZonePlaneMain_Callback(sender As Object, e As DevExpress.Web.CallbackEventArgsBase) Handles cmbZonePlaneMain.Callback
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)
        If strParameter.IndexOf("SELECTEDVALUE") >= 0 Then
            Dim nValue As Integer = roTypes.Any2Integer(strParameter.Substring(strParameter.IndexOf("=") + 1))
            Me.LoadZonePlaneMain(nValue)
            cmbZonePlaneMain.JSProperties.Add("cpAction", "SELECTEDVALUE")
            If cmbZonePlane.SelectedItem IsNot Nothing Then
                cmbZonePlaneMain.JSProperties.Add("cpValue", cmbZonePlane.SelectedItem.Value)
            Else
                cmbZonePlaneMain.JSProperties.Add("cpValue", "")
            End If
        End If
    End Sub

    Protected Sub CallbackHelper_Callback(source As Object, e As DevExpress.Web.CallbackEventArgs) Handles CallbackHelper.Callback
        e.Result = String.Empty
        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        If strParameter.IndexOf("SELECTEDPLANE") >= 0 Then
            Dim IdPlane As Integer = roTypes.Any2Integer(strParameter.Substring(strParameter.IndexOf("=") + 1))
            Dim ErrorInfo As String = String.Empty
            Dim PositionParam As String = String.Empty
            Dim ImageIdParam As String = String.Empty
            LoadAccessZoneMain(IdPlane, ErrorInfo, PositionParam, ImageIdParam)
            e.Result = PositionParam & "@" & ImageIdParam
        End If
    End Sub

End Class