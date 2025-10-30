Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.EventScheduler
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Events
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

        <Runtime.Serialization.DataMember(Name:="gridAuthorizations")>
        Public gridAuthorizations As String

    End Class

    Private Const FeatureAlias As String = "Events.Definition"
    Private oPermission As Permission

#Region "Properties"

    Private Property AuthorizationsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Events_EmployeesAuthorizations")

            If bolReload OrElse tb Is Nothing Then
                'tb = AccessGroupService.AccessGroupServiceMethods.GetEmployeeAuthorizations(Me.Page, intIDEmployee, True)
                tb = EventSchedulerMethods.GetEventAuthorizations(Me.Page, Session("Events_CurrentIDEvent"), True)

                If tb IsNot Nothing Then
                    tb.PrimaryKey = New DataColumn() {tb.Columns("IDAuthorization")}
                    tb.AcceptChanges()
                End If

                Session("Events_EmployeesAuthorizations") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Events_EmployeesAuthorizations") = value
        End Set
    End Property

    Private Property AccessGroupsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("Events_EmployeesAccessGroups")

            If bolReload OrElse tb Is Nothing Then
                tb = AccessGroupServiceMethods.GetAccessGroups(Me.Page)

                Session("Events_EmployeesAccessGroups") = tb
            End If
            Return tb
        End Get
        Set(value As DataTable)
            Session("Events_EmployeesAccessGroups") = value
        End Set
    End Property

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("Events", "~/Access/Scripts/EventsV2.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("frmAddAuthorization", "~/Access/Scripts/frmAddAuthorization.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\Events") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesEvents.TreeCaption = Me.Language.Translate("TreeCaptionEvents", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oPermission < Permission.Write Then
            hdnModeEdit.Value = "true"
            Me.DisableControls()
        Else
            hdnModeEdit.Value = "false"
        End If

        Me.btnAddNewAccessGroup.Visible = (Me.oPermission >= Permission.Admin)

        Dim oEvents As Generic.List(Of roEventScheduler) = EventSchedulerMethods.GetEventsScheduler(Me, False)
        If oEvents Is Nothing OrElse oEvents.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        If Not Me.IsPostBack Then

            Dim oAccessgroups As DataTable = Me.AccessGroupsData(True)

        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim bRet As Boolean = False

        Select Case oParameters.Action

            Case "GETEventScheduler"
                LoadEvent(oParameters)
            Case "SAVEEventScheduler"
                SaveEvent(oParameters)
            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Sub LoadEvent(ByVal oParameters As ObjectCallbackRequest, Optional ByVal eEventScheduler As roEventScheduler = Nothing)
        Dim oCurrentEvent As roEventScheduler = Nothing
        Dim result As String = "OK"
        Try

            If eEventScheduler IsNot Nothing Then
                oCurrentEvent = eEventScheduler
            Else
                If oParameters.ID = -1 Then
                    oCurrentEvent = New roEventScheduler
                Else
                    oCurrentEvent = EventSchedulerMethods.GetEventScheduler(Me, oParameters.ID, True)
                End If
            End If

            Session("Events_CurrentIDEvent") = oParameters.ID

            If oCurrentEvent Is Nothing Then Exit Sub

            txtName.Text = oCurrentEvent.Name
            txtDescription.Text = oCurrentEvent.Description
            txtDate.Date = oCurrentEvent.EventDate
            txtEndDate.Date = oCurrentEvent.EventDateEnd
            txtShortName.Text = oCurrentEvent.ShortName
            txtMainDate.Date = oCurrentEvent.EventMainDate

            Dim GridsJSON As String = String.Empty
            GridsJSON = CreateGridsJSON(oCurrentEvent)

            ASPxCallbackPanelContenido.JSProperties.Add("cpGridsJSON", GridsJSON)

            'Mostra el TAB seleccionat
            Select Case oParameters.aTab
                Case 0
                    Me.div00.Style("display") = ""
            End Select
        Catch ex As Exception
            result = "KO"
        Finally
            ASPxCallbackPanelContenido.JSProperties.Add("cpActionRO", "GETEventScheduler")
            ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", oCurrentEvent.Name)
            ASPxCallbackPanelContenido.JSProperties.Add("cpResultRO", result)
            ASPxCallbackPanelContenido.JSProperties.Add("cpIsNewRO", False)
        End Try

    End Sub

    Private Function CreateGridsJSON(ByRef oCurrentEvent As roEventScheduler) As String
        Try

            Dim oJGAuthorization As New Generic.List(Of Object)
            Dim oJFAuthorization As Generic.List(Of JSONFieldItem)
            Dim strJSONText As String = ""

            Dim strJSONGroups As String = ""
            If Request("ID") = "-1" Then Return ""

            If oCurrentEvent Is Nothing Then Return ""

            If oCurrentEvent.Authorizations IsNot Nothing Then
                If oCurrentEvent.Authorizations.Count > 0 Then

                    Dim Authorizations = From row In oCurrentEvent.Authorizations.AsEnumerable() Select row.IDAuthorization Distinct

                    For Each auth In Authorizations
                        Dim curAuths = oCurrentEvent.Authorizations.ToList.FindAll(Function(x) x.IDAuthorization = auth)
                        Dim idEvent = -1
                        Dim strSelected = String.Empty
                        For Each oDayAuth In curAuths
                            idEvent = oDayAuth.IDEvent
                            strSelected &= oDayAuth.AuthorizationDate.ToString("dd/MM/yyyy") & ","
                        Next

                        If strSelected <> String.Empty Then
                            strSelected = strSelected.Substring(0, strSelected.Length - 1)
                        End If

                        Dim AccessGroup = AccessGroupServiceMethods.GetAccessGroupByID(Me.Page, auth)
                        oJFAuthorization = New Generic.List(Of JSONFieldItem)
                        oJFAuthorization.Add(New JSONFieldItem("idauthorization", auth.ToString.ToLower, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFAuthorization.Add(New JSONFieldItem("Type", idEvent.ToString.ToLower, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFAuthorization.Add(New JSONFieldItem("Authorization", strSelected.ToString, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                        oJFAuthorization.Add(New JSONFieldItem("Display", AccessGroup.Name.ToString & " (" & strSelected.ToString & ")", Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                        oJGAuthorization.Add(oJFAuthorization)

                    Next

                    'For Each auth In Authorizations
                    '    Dim x As Integer = 0
                    '    For Each oAuthorization As EventSchedulerService.roEventAccessAuthorization In oCurrentEvent.Authorizations
                    '        If oAuthorization.IDAuthorization = auth Then
                    '            Dim AccessGroup = AccessGroupService.AccessGroupServiceMethods.GetAccessGroupByID(Me.Page, oAuthorization.IDAuthorization)
                    '            oJFAuthorization = New Generic.List(Of JSONFieldItem)
                    '            oJFAuthorization.Add(New JSONFieldItem("idauthorization", oAuthorization.IDAuthorization.ToString.ToLower, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '            oJFAuthorization.Add(New JSONFieldItem("Type", oAuthorization.IDEvent.ToString.ToLower, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '            oJFAuthorization.Add(New JSONFieldItem("Authorization", oAuthorization.AuthorizationDate.ToString, Nothing, roJSON.JSONType.None_JSON, Nothing, False))
                    '            oJFAuthorization.Add(New JSONFieldItem("Display", AccessGroup.Name.ToString, Nothing, roJSON.JSONType.None_JSON, Nothing, False))

                    '            oJGAuthorization.Add(oJFAuthorization)
                    '            x = x + 1
                    '        End If
                    '    Next
                    'Next
                End If
            End If

            strJSONGroups &= "{ ""authorizations"": [ "

            For Each oObj As Object In oJGAuthorization
                Dim strJSONTextEx As String = ""
                strJSONTextEx &= "{ ""fields"": "
                strJSONTextEx &= roJSONHelper.Serialize(oObj)
                strJSONTextEx &= " } ,"
                strJSONGroups &= strJSONTextEx
            Next
            If strJSONGroups.EndsWith(",") Then strJSONGroups = strJSONGroups.Substring(0, Len(strJSONGroups) - 1)
            strJSONGroups &= "] }"

            Return "[" & strJSONGroups & "]"
        Catch ex As Exception
            Return String.Empty
        End Try
    End Function

    Private Sub SaveEvent(ByVal oParameters As ObjectCallbackRequest)
        Dim rError As New roJSON.JSONError(False, "")
        Dim oCurrentEvent As roEventScheduler = Nothing

        Try
            Dim bolIsNew As Boolean = False
            If oParameters.ID = "-1" Then bolIsNew = True

            If bolIsNew Then
                oCurrentEvent = New roEventScheduler
                oCurrentEvent.ID = -1
            Else
                oCurrentEvent = EventSchedulerMethods.GetEventScheduler(Me, oParameters.ID, False)
            End If

            If oCurrentEvent Is Nothing Then Exit Sub

            oCurrentEvent.Name = txtName.Text
            oCurrentEvent.Description = txtDescription.Text
            oCurrentEvent.EventDate = txtDate.Date
            oCurrentEvent.ShortName = txtShortName.Text
            oCurrentEvent.EventDateEnd = txtEndDate.Date
            oCurrentEvent.EventMainDate = txtMainDate.Date

            If Not String.IsNullOrEmpty(oParameters.gridAuthorizations) Then

                Dim resultGroups As New Generic.List(Of roEventAccessAuthorization)
                Dim oAccessGroupPermission As roEventAccessAuthorization

                Dim oArrElements = oParameters.gridAuthorizations.Split(".")
                For Each element As String In oArrElements
                    Dim auth = element.Split(";")
                    Dim oArrDays = auth(1).Split(",")
                    For Each day As String In oArrDays
                        If IsDate(day) Then
                            oAccessGroupPermission = New roEventAccessAuthorization
                            oAccessGroupPermission.IDAuthorization = auth(0)
                            oAccessGroupPermission.IDEvent = oCurrentEvent.ID
                            If day >= oCurrentEvent.EventDate AndAlso day <= oCurrentEvent.EventDateEnd Then
                                oAccessGroupPermission.AuthorizationDate = day
                            Else
                                oAccessGroupPermission.AuthorizationDate = oCurrentEvent.EventDate
                            End If
                            If Not resultGroups.Contains(oAccessGroupPermission) Then
                                resultGroups.Add(oAccessGroupPermission)
                            End If
                        End If
                    Next
                Next
                oCurrentEvent.Authorizations = resultGroups
            Else
                oCurrentEvent.Authorizations = Nothing
            End If

            If Me.oPermission = Permission.Write And bolIsNew Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            End If

            If rError.Error = False Then
                If EventSchedulerMethods.SaveEventScheduler(Me, oCurrentEvent, True) = True Then
                    Session("Events_CurrentIDEvent") = oCurrentEvent.ID
                    HelperWeb.roSelector_SetSelection(oCurrentEvent.ID, "/source/" & oCurrentEvent.ID, "ctl00_contentMainBody_roTreesEvents")
                    rError = New roJSON.JSONError(False, "OK:" & oCurrentEvent.ID)
                Else
                    rError = New roJSON.JSONError(True, EventSchedulerMethods.LastErrorText)
                End If
            End If
        Catch ex As Exception
            rError = New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
        Finally
            If rError.Error = False Then
                LoadEvent(oParameters, oCurrentEvent)
                ASPxCallbackPanelContenido.JSProperties("cpIsNewRO") = True
                ASPxCallbackPanelContenido.JSProperties("cpNameRO") = oCurrentEvent.Name
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "OK"
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "GETEventScheduler"
            Else
                LoadEvent(oParameters, oCurrentEvent)
                ASPxCallbackPanelContenido.JSProperties("cpActionRO") = "SAVEEventScheduler"
                ASPxCallbackPanelContenido.JSProperties("cpResultRO") = "KO"
                ASPxCallbackPanelContenido.JSProperties.Add("cpErrorRO", rError)
            End If
        End Try
    End Sub

End Class