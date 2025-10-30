Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.DiningRoom
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class DiningRoom
    Inherits PageBase

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

    End Class

    Private oPermission As Permission
    Private Const EMPTY_WEEK As String = "0000000"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("DiningRoom.Turns")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("DiningRooms", "~/DiningRoom/Scripts/DiningRooms.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Forms\DiningRoom") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesDiningRoom.TreeCaption = Me.Language.Translate("TreeCaptionDiningRoom", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText
        Me.lblExport.Text = Me.Language.Translate("lblExport", DefaultScope)
        Me.lblExportName.Text = Me.Language.Translate("lblExportName", DefaultScope)

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
            cmbUserFields.Enabled = False
            txtBeginTime.ReadOnly = True
            txtEndTime.ReadOnly = True
        End If

        'PPR desactivado temporalmente NO ELIMINAR-->
        'Dim oPermission As Permission = Me.GetFeaturePermission("DiningRoom.Turns")
        'If oPermission = Permission.None Or Not String.IsNullOrEmpty(Request.Url.Query) Then
        '    If Not String.IsNullOrEmpty(Request.Url.Query) Then
        '        If Uri.IsWellFormedUriString(Request.Url.AbsoluteUri, UriKind.Absolute) Then
        '            WLHelperWeb.PendingUserSessionQueryString("DiningRoom") = Request.Url.PathAndQuery
        '            WLHelperWeb.RedirectDefault()
        '        Else
        '            WLHelperWeb.RedirectAccessDenied(False)
        '        End If
        '    Else
        '        WLHelperWeb.RedirectAccessDenied(False)
        '    End If
        '    Exit Sub
        'End If

        If Not DiningRoomServiceMethods.ExitsDiningRoomTurn(Me.Page, -1) Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        'PPR desactivado temporalmente NO ELIMINAR-->
        'Dim oQueryString As New roQueryStringState("DiningRoom")
        'If DiningRoomServiceMethods.ExitsDiningRoomTurn(Me.Page, -1) Then
        '    oQueryString.HasReg = "1"
        'Else
        '    oQueryString.HasReg = "0"
        'End If
        'ProcessQueryString(oQueryString, WLHelperWeb.PendingCookieQueryString("DiningRoom", True))
        'HelperWeb.roPage_SetQueryStringState(oQueryString)

        If Not Me.IsPostBack And Not Me.IsCallback Then
            Me.LoadDiningRoomData()
        End If

    End Sub

    'PPR desactivado temporalmente NO ELIMINAR-->
    'Private Sub ProcessQueryString(ByRef oQueryString As roQueryStringState, ByVal strQueryString As String)
    '    Try
    '        If strQueryString <> String.Empty Then

    '            Dim dic As Generic.Dictionary(Of QuerystringDefaultParam, String) = GetParamsFromQuerystring(strQueryString)
    '            For Each item As Generic.KeyValuePair(Of QuerystringDefaultParam, String) In dic
    '                Select Case item.Key

    '                    Case QuerystringDefaultParam.ID
    '                        Dim treePath As String = String.Empty
    '                        If DiningRoomServiceMethods.ExitsDiningRoomTurn(Me.Page, item.Value) Then
    '                            treePath = "/source/" & item.Value
    '                        End If
    '                        HelperWeb.roSelector_SetSelection(item.Value, treePath, "ctl00_contentMainBody_roTreesDiningRoom")

    '                    Case QuerystringDefaultParam.TAB
    '                        If item.Value <> "" Then
    '                            oQueryString.ActiveTab = item.Value
    '                        End If

    '                End Select
    '            Next
    '        End If
    '    Catch ex As Exception
    '    End Try
    'End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean

        Select Case oParameters.Action

            Case "GETDININGROOM"
                bRet = LoadDiningRoom(oParameters.ID, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETDININGROOM")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)

            Case "SAVEDININGROOM"
                bRet = SaveDiningRoom(oParameters.ID, oParameters.Name, responseMessage)

                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVEDININGROOM")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                Else
                    ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETDININGROOM")
                    ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", True)
                End If

            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadDiningRoom(ByRef IdDiningRoom As Integer, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim oCurrentDiningRoomTurn As roDiningRoomTurn
                If IdDiningRoom = -1 Then
                    oCurrentDiningRoomTurn = New roDiningRoomTurn()
                    oCurrentDiningRoomTurn.ID = -1
                Else
                    oCurrentDiningRoomTurn = DiningRoomServiceMethods.GetDiningRoomTurnByID(Me.Page, IdDiningRoom, True)
                End If

                txtName.Text = oCurrentDiningRoomTurn.Name
                If oCurrentDiningRoomTurn.Export <> "" Then
                    Me.txtExport.Text = oCurrentDiningRoomTurn.Export '.ToString.Replace(HelperWeb.GetDecimalDigitFormat, ".")
                Else
                    Me.txtExport.Text = "0"
                End If
                txtBeginTime.Value = oCurrentDiningRoomTurn.BeginTime
                txtEndTime.Value = oCurrentDiningRoomTurn.EndTime

                Dim txtWeekDays As String = ValidateCorrectWeek(oCurrentDiningRoomTurn.DaysOfWeek)
                chkWeekDay1.Checked = (txtWeekDays.Substring(0, 1) = "1")
                chkWeekDay2.Checked = (txtWeekDays.Substring(1, 1) = "1")
                chkWeekDay3.Checked = (txtWeekDays.Substring(2, 1) = "1")
                chkWeekDay4.Checked = (txtWeekDays.Substring(3, 1) = "1")
                chkWeekDay5.Checked = (txtWeekDays.Substring(4, 1) = "1")
                chkWeekDay6.Checked = (txtWeekDays.Substring(5, 1) = "1")
                chkWeekDay7.Checked = (txtWeekDays.Substring(6, 1) = "1")

                'EmployeeSelection
                Me.optAll.Checked = True
                Me.optSelection.Checked = False
                cmbUserFields.SelectedIndex = 0
                txtUserFields.Value = String.Empty
                Dim oEmployeeSelection As New roCollection(oCurrentDiningRoomTurn.DefinitionXML)
                If oEmployeeSelection.Count > 0 Then
                    Dim strValue As String = oEmployeeSelection.Item("prmUserFields", roCollection.roSearchMode.roByKey)
                    If strValue <> String.Empty AndAlso strValue.IndexOf("~") > 0 Then
                        Dim strFieldName As String = strValue.Substring(0, strValue.IndexOf("~"))
                        Dim strFieldValue As String = strValue.Substring(strValue.IndexOf("~") + 1, strValue.Length() - strValue.IndexOf("~") - 1)

                        Dim it As DevExpress.Web.ListEditItem = cmbUserFields.Items.FindByValue(strFieldName)
                        If it Is Nothing Then
                            cmbUserFields.SelectedIndex = 0
                        Else
                            cmbUserFields.SelectedItem = it
                        End If
                        txtUserFields.Value = strFieldValue

                        Me.optAll.Checked = False
                        Me.optSelection.Checked = True

                    End If
                End If

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function SaveDiningRoom(ByRef IdDiningRoom As Integer, ByRef DiningRoomName As String, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission >= Permission.Write Then

                Dim oCurrentDiningRoomTurn As roDiningRoomTurn = DiningRoomServiceMethods.GetDiningRoomTurnByID(Me.Page, IdDiningRoom, False)
                If Not oCurrentDiningRoomTurn Is Nothing Then

                    oCurrentDiningRoomTurn.ID = IdDiningRoom
                    oCurrentDiningRoomTurn.Name = txtName.Text
                    If oCurrentDiningRoomTurn.Export IsNot Nothing Then
                        oCurrentDiningRoomTurn.Export = txtExport.Text
                    Else
                        oCurrentDiningRoomTurn.Export = "0"
                    End If
                    oCurrentDiningRoomTurn.BeginTime = New DateTime(1900, 1, 1, txtBeginTime.DateTime.Hour, txtBeginTime.DateTime.Minute, 0)
                        oCurrentDiningRoomTurn.EndTime = New DateTime(1900, 1, 1, txtEndTime.DateTime.Hour, txtEndTime.DateTime.Minute, 0)

                        Dim strWeek As String = IIf(chkWeekDay1.Checked, "1", "0") & IIf(chkWeekDay2.Checked, "1", "0") & IIf(chkWeekDay3.Checked, "1", "0") & IIf(chkWeekDay4.Checked, "1", "0") &
                                            IIf(chkWeekDay5.Checked, "1", "0") & IIf(chkWeekDay6.Checked, "1", "0") & IIf(chkWeekDay7.Checked, "1", "0")

                        Dim oEmployeeSelection As New roCollection()
                        If optSelection.Checked Then
                            If Not cmbUserFields.SelectedItem Is Nothing Then
                                oEmployeeSelection.Add("prmUserFields", cmbUserFields.SelectedItem.Value & "~" & Me.txtUserFields.Value)
                            End If
                        End If
                        oCurrentDiningRoomTurn.DefinitionXML = oEmployeeSelection.XML()

                        oCurrentDiningRoomTurn.IDDiningRoom = 1 'De momento solo se trabaja con un comedor fijo

                        oCurrentDiningRoomTurn.DaysOfWeek = strWeek

                        bRet = DiningRoomServiceMethods.SaveDiningRoomTurn(Me.Page, oCurrentDiningRoomTurn, True)
                        If bRet Then
                            Dim treePath As String = "/source/" & oCurrentDiningRoomTurn.ID
                            HelperWeb.roSelector_SetSelection(oCurrentDiningRoomTurn.ID.ToString, treePath, "ctl00_contentMainBody_roTreesDiningRoom")
                        Else
                            ErrorInfo = DiningRoomServiceMethods.LastErrorText
                        End If
                    Else
                        ErrorInfo = DiningRoomServiceMethods.LastErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Sub LoadDiningRoomData()
        Me.cmbUserFields.Items.Clear()
        Dim dtblUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me, Types.EmployeeField, "Used <> 0", False)
        If Not dtblUserFields Is Nothing Then
            Dim dRows() As DataRow = dtblUserFields.Select("", "FieldName")
            Me.cmbUserFields.Items.Add(New DevExpress.Web.ListEditItem("", ""))
            For Each dRow As DataRow In dRows
                If dRow("FieldType") = 0 Then
                    Me.cmbUserFields.Items.Add(New DevExpress.Web.ListEditItem(dRow("FieldName"), dRow("FieldName")))
                End If
            Next
        End If
    End Sub

    Private Function ValidateCorrectWeek(ByVal strWeek As String) As String
        Dim strFin As String = String.Empty
        If Not strWeek Is Nothing AndAlso strWeek.Length > 0 Then
            For n As Integer = 0 To strWeek.Length - 1
                If strWeek(n) = "0" Or strWeek(n) = "1" Then
                    strFin &= strWeek(n)
                Else
                    strFin &= "0"
                End If
            Next
            strFin = (strFin & EMPTY_WEEK).Substring(0, EMPTY_WEEK.Length)
            Return strFin
        Else
            Return EMPTY_WEEK
        End If
    End Function

End Class