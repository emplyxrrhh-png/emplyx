Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Requests_srvRequests
    Inherits PageBase

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        Select Case Request("action")
            Case "getRequestsTab" ' Retorna la capcelera de la plana
                Me.Controls.Clear()
                If Me.CheckPermissionsAny Then
                    LoadRequestsTab(roTypes.Any2Integer(Request("aTab")))
                Else
                    Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                    Response.Write(rError.toJSON)
                End If

            Case "getRequestsList" ' Retorna un Export (Contenidors)
                If Me.CheckPermissionsAny Then
                    Me.divGeneral.Visible = False
                    Me.Controls.Remove(Me.divGeneral)
                    LoadRequestsListData(roTypes.Any2String(Request("Filter")), roTypes.Any2String(Request("Order")), roTypes.Any2String(Request("ListType")),
                                         roTypes.Any2String(Request("FilterEmployees")), roTypes.Any2String(Request("FilterTree")), roTypes.Any2String(Request("FilterTreeUser")),
                                         roTypes.Any2Integer(Request("NumRequestToLoad")), roTypes.Any2String(Request("LevelsBelow")), roTypes.Any2Integer(Request("IdCause")), roTypes.Any2Integer(Request("IdSupervisor")))
                Else
                    Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                    Response.Write(rError.toJSON)
                End If
            Case "getRequest"
                Me.tblRequests.Visible = False
                Me.divGeneral.Style("Display") = ""
                LoadRequestData(roTypes.Any2Integer(Request("IDRequest")), roTypes.Any2String(Request("IdTableRow")))

            Case "approveRequest" ' Acepta una solicitud
                Me.Controls.Clear()
                Me.ApproveRefuseRequest(Request("IDRequest"), True, roTypes.Any2String(Request("Comments")), roTypes.Any2Boolean(Request("CheckLockedDays")), roTypes.Any2Boolean(Request("forceApprove")))

            Case "refuseRequest" ' Denega una solicitud
                Me.Controls.Clear()
                Me.ApproveRefuseRequest(Request("IDRequest"), False, roTypes.Any2String(Request("Comments")))

            Case "getBarButtons"
                Me.Controls.Clear()
                GetBarButtons()

            Case "saveFilter"
                Me.Controls.Clear()
                SaveFilter(roTypes.Any2String(Request("Filter")))

        End Select

    End Sub

    Private Sub SaveFilter(ByVal Filter As String)
        If Filter <> String.Empty Then
            Filter = Server.UrlDecode(Filter)
            API.RequestServiceMethods.SetFilterRequests(Me, WLHelperWeb.CurrentPassport.ID, Filter)
        Else
            API.RequestServiceMethods.SetFilterRequests(Me, WLHelperWeb.CurrentPassport.ID, "")
        End If
    End Sub

    Private Sub LoadRequestsTab(ByVal ActiveTab As Integer)
        Try

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/Requests80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><a href=""javascript: void(0);"" onclick="""" style=""display: block;cursor: text;"" title=""""><span id=""readOnlyNameAccessZones"" class=""NameText"">" & Me.Language.Translate("RequestsList.Title", Me.DefaultScope) & " </span></a></div>" &
                                "<div id=""NameChange"" style=""display: none;""><table><tr><td><input type=""text"" id=""txtName"" style=""width: 350px;"" value="""" class=""inputNameText"" ConvertControl=""TextField"" CCallowBlank=""false"" onblur="""" ></td>" &
                                "</tr></table></div>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(ActiveTab))

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Me.Controls.Add(oMainDiv)
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub LoadRequestsListData(ByVal strFilter As String, ByVal strOrder As String, ByVal strListType As String, ByVal FilterEmployees As String,
                                     ByVal strFilterTree As String, ByVal strFilterTreeUser As String, ByVal NumRequestToLoad As Integer, ByVal LevelsBelow As String, ByVal IdCause As Integer, ByVal IdSupervisor As Integer)

        strFilter = strFilter.Replace("@idPassport", WLHelperWeb.CurrentPassport.ID)

        If FilterEmployees <> String.Empty Then

            Dim lstEmployees As Generic.List(Of Integer) = Nothing
            Dim lstGroups As Generic.List(Of Integer) = Nothing
            Dim strListEmployees As String = ""

            'obtener todos los empleados de los grupos seleccionados en el arbol v3
            roSelectorManager.ExtractIdsFromSelectionString(FilterEmployees, lstEmployees, lstGroups)
            If lstGroups IsNot Nothing Then
                Dim tmp As Generic.List(Of Integer) = API.EmployeeGroupsServiceMethods.GetEmployeeListFromGroupRecursive(Me, lstGroups.ToArray, "Employees", "U",
                                                                                                                                           strFilterTree, strFilterTreeUser)
                lstEmployees.AddRange(tmp)
            End If

            If lstEmployees.Count > 0 Then
                For Each intID As Integer In lstEmployees
                    strListEmployees &= intID.ToString & ","
                Next
                strListEmployees = strListEmployees.Substring(0, strListEmployees.Length - 1)

                strFilter = strFilter + " AND Requests.IDEmployee IN (" & strListEmployees & ") "
            End If

        End If

        Dim bIncludeAutomaticRequests As Boolean = False
        If strListType.Trim = "2" Then bIncludeAutomaticRequests = True

        Dim tbRequests As DataTable = API.RequestServiceMethods.GetRequestsSupervisor(Me, WLHelperWeb.CurrentPassport.ID, strFilter, strOrder, True, NumRequestToLoad, LevelsBelow, IdCause, IdSupervisor, bIncludeAutomaticRequests)
        If tbRequests IsNot Nothing AndAlso tbRequests.Rows.Count > 0 Then

            Dim tbRequestTypes As DataTable = GetRequestTypesFromApplication()

            Dim oRow() As DataRow

            For Each oRequestRow As DataRow In tbRequests.Rows
                oRow = tbRequestTypes.Select("ElementID=" & oRequestRow("RequestType"))
                If oRow.Length > 0 Then
                    createRowLineRequest(oRequestRow, oRow(0), strListType)
                Else
                    createRowLineRequest(oRequestRow, Nothing, strListType)
                End If
            Next

        End If

    End Sub

    Private Sub LoadRequestData(ByVal intIDRequest As Integer, ByVal IdTableRow As String)

        Try
            If intIDRequest > 0 Then
                Dim oRequest As roRequest = API.RequestServiceMethods.GetRequestByID(Me, intIDRequest, True)
                If oRequest IsNot Nothing AndAlso oRequest.IDEmployee > 0 Then

                    Dim oRequestPermission As Permission = Permission.None

                    ' Obtenemos la configuración de permiso en función del tipo de solicitud y el empleado
                    Dim oRequestTypeSecurity As roRequestTypeSecurity = API.RequestServiceMethods.GetRequestTypeSecurity(Me, oRequest.RequestType)
                    If oRequestTypeSecurity IsNot Nothing Then
                        oRequestPermission = Me.GetFeaturePermissionByEmployee(oRequestTypeSecurity.SupervisorFeatureName, oRequest.IDEmployee, "U")

                    End If

                    If oRequestPermission >= Permission.Read Then
                        'Obtenemos permisos sobre documentos


                        Me.trDocumentsWithoutPermissions.Visible = False
                        Me.trDocuments.Visible = False
                        Dim documentsbyRequest As Generic.List(Of Integer) = DocumentsServiceMethods.GetDocumentsbyRequest(oRequest.ID, Me.Page, False)
                        Dim i = 0
                        If documentsbyRequest.Count > 0 Then
                            Dim hasPermissionOverDocuments As Boolean = False
                            For Each document As Integer In documentsbyRequest
                                i = i + 1
                                Select Case i
                                    Case 1
                                        Dim idDocument As Integer = DocumentsServiceMethods.CanAccessRequestDocumentation(document, Me.Page, False)
                                        If idDocument > 0 Then
                                            document1.Style("display") = ""
                                            Me.lbldocument1.Attributes("onclick") = "downloadRequestDoc(" & idDocument & ");"
                                            Dim documentName = DocumentsServiceMethods.GetDocumentById(Me.Page, idDocument, False).DocumentTemplate.Name
                                            Me.lbldocument1.Text = documentName
                                            hasPermissionOverDocuments = True
                                        End If
                                    Case 2
                                        Dim idDocument As Integer = DocumentsServiceMethods.CanAccessRequestDocumentation(document, Me.Page, False)
                                        If idDocument > 0 Then
                                            document2.Style("display") = ""
                                            Me.lbldocument2.Attributes("onclick") = "downloadRequestDoc(" & idDocument & ");"
                                            Dim documentName = DocumentsServiceMethods.GetDocumentById(Me.Page, idDocument, False).DocumentTemplate.Name
                                            Me.lbldocument2.Text = documentName
                                            hasPermissionOverDocuments = True
                                        End If
                                    Case 3
                                        Dim idDocument As Integer = DocumentsServiceMethods.CanAccessRequestDocumentation(document, Me.Page, False)
                                        If idDocument > 0 Then
                                            document3.Style("display") = ""
                                            Me.lbldocument3.Attributes("onclick") = "downloadRequestDoc(" & idDocument & ");"
                                            Dim documentName = DocumentsServiceMethods.GetDocumentById(Me.Page, idDocument, False).DocumentTemplate.Name
                                            Me.lbldocument3.Text = documentName
                                            hasPermissionOverDocuments = True
                                        End If
                                    Case 4
                                        Dim idDocument As Integer = DocumentsServiceMethods.CanAccessRequestDocumentation(document, Me.Page, False)
                                        If idDocument > 0 Then
                                            document4.Style("display") = ""
                                            Me.lbldocument4.Attributes("onclick") = "downloadRequestDoc(" & idDocument & ");"
                                            Dim documentName = DocumentsServiceMethods.GetDocumentById(Me.Page, idDocument, False).DocumentTemplate.Name
                                            Me.lbldocument4.Text = documentName
                                            hasPermissionOverDocuments = True
                                        End If
                                    Case 5
                                        Dim idDocument As Integer = DocumentsServiceMethods.CanAccessRequestDocumentation(document, Me.Page, False)
                                        If idDocument > 0 Then
                                            document5.Style("display") = ""
                                            Me.lbldocument5.Attributes("onclick") = "downloadRequestDoc(" & idDocument & ");"
                                            Dim documentName = DocumentsServiceMethods.GetDocumentById(Me.Page, idDocument, False).DocumentTemplate.Name
                                            Me.lbldocument5.Text = documentName
                                            hasPermissionOverDocuments = True
                                        End If
                                End Select

                            Next

                            If hasPermissionOverDocuments Then
                                Me.trDocuments.Visible = True
                            Else
                                Me.trDocumentsWithoutPermissions.Visible = True
                            End If
                        End If

                        If oRequest.RequestType = eRequestType.Telecommute Then
                            Me.trTelecommuteResume.Visible = True
                            Dim oLng As New roLanguage
                            oLng.SetLanguageReference("LiveRequests", WLHelperWeb.CurrentLanguage)

                            Dim oStats As roEmployeeTelecommuteAgreementStats = EmployeeServiceMethods.GetTelecommuteStatsAtDate(Me.Page, oRequest.IDEmployee, oRequest.Date1)

                            If oStats IsNot Nothing Then
                                oLng.ClearUserTokens()
                                Dim sPeriotType As String = oLng.Translate("Period." & oStats.EmployeeTelecommuteAgreement.Agreement.PeriodType.ToString, "Telecommute")

                                oLng.ClearUserTokens()
                                oLng.AddUserToken(oStats.PeriodStart.ToString("dd/MM/yyyy"))
                                oLng.AddUserToken(oStats.PeriodEnd.ToString("dd/MM/yyyy"))

                                If oStats.EmployeeTelecommuteAgreement.Agreement.MaxType = TelecommutingMaxType._Days Then
                                    oLng.AddUserToken(oStats.EmployeeTelecommuteAgreement.Agreement.MaxDays)
                                    oLng.AddUserToken("d/")
                                Else
                                    oLng.AddUserToken(oStats.EmployeeTelecommuteAgreement.Agreement.MaxPercentage)
                                    oLng.AddUserToken("%/")
                                End If
                                oLng.AddUserToken(sPeriotType)

                                Me.lblResumePeriod.Text = oLng.Translate("Definition.Description", "Telecommute")

                                oLng.ClearUserTokens()
                                oLng.AddUserToken(oStats.TelecommutePlannedDays)
                                oLng.AddUserToken(oStats.TelecommutePlannedHours)
                                oLng.AddUserToken(oStats.TotalWorkingPlannedHours)
                                Me.lblTCResumeType.Text = oLng.Translate("Definition.Apply", "Telecommute")
                            End If
                        Else
                            Me.trTelecommuteResume.Visible = False
                        End If

                        If oRequest.RequestType = eRequestType.DailyRecord Then
                            Me.trDailyRecordResume.Visible = True
                        Else
                            Me.trDailyRecordResume.Visible = False
                        End If

                        ' Obtenemos el nivel de mando del supervisor
                        Dim intLevelOfAuthority As Integer = 0
                        intLevelOfAuthority = 0
                        If WLHelperWeb.CurrentPassport.ID > 0 Then intLevelOfAuthority = SecurityV3ServiceMethods.GetPassportLevelOfAuthority(Me, WLHelperWeb.CurrentPassport.ID, oRequest.RequestType, IIf(oRequest.IDCause.HasValue, oRequest.IDCause, 0), oRequest.ID)

                        ' Foto empleado
                        Me.imgEmployee.Src = Me.ResolveUrl("../Employees/loadimage.aspx?IdEmployee=" & oRequest.IDEmployee & "&NewParam=" & Now.TimeOfDay.Seconds.ToString)
                        Me.imgEmployee.Attributes("onclick") = "showEmployee(null, '" & oRequest.IDEmployee.ToString & "','" & Configuration.RootUrl & "');"

                        ' Nombre empleado
                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, oRequest.IDEmployee, False)
                        If oEmployee IsNot Nothing Then Me.txtEmployeeName.Text = oEmployee.Name
                        Me.lnkEmployeeName.Attributes("onclick") = "showEmployee(null, '" & oRequest.IDEmployee.ToString & "','" & Configuration.RootUrl & "');"

                        ' Nombre grupo
                        'Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Me, oRequest.IDEmployee, False)
                        Dim oMobility As roMobility = API.EmployeeServiceMethods.GetCurrentMobility(Me, oRequest.IDEmployee)
                        If oMobility IsNot Nothing Then Me.txtGroupName.Text = API.EmployeeServiceMethods.GetCurrentFullGroupName(Me, oRequest.IDEmployee) ' oMobility.Name
                        Me.lnkGroupName.Attributes("onclick") = "showEmployeeGroup(null, '" & oMobility.IdGroup.ToString & "','" & Configuration.RootUrl & "');"

                        ' Tipo solicitud
                        Dim oTypeRow As DataRow = Nothing
                        Dim tbRequestTypes As DataTable = GetRequestTypesFromApplication()
                        Dim oRow() As DataRow = tbRequestTypes.Select("ElementName='" & oRequest.RequestType.ToString & "'")
                        If oRow.Length > 0 Then oTypeRow = oRow(0)
                        Dim strImg As String = "../Requests/Images/RequestTypes/32/"
                        ' Determinamos el nombre de la imagen del tipo de solicitud
                        If oTypeRow IsNot Nothing Then
                            strImg &= oTypeRow("ElementName") & ".png"
                        Else
                            strImg &= "UnknownType.png"
                        End If
                        Me.imgRequestType.Src = strImg
                        If oTypeRow IsNot Nothing Then
                            Me.txtRequestType.Text = oTypeRow("ElementDesc")
                        End If

                        Dim oMsgParams As New Generic.List(Of String)

                        ' Fecha y hora
                        Me.txtRequestDate.Text = Format(oRequest.RequestDate, HelperWeb.GetShortDateFormat) & " " & Format(oRequest.RequestDate, HelperWeb.GetShortTimeFormat)
                        Dim intFromDays As Integer = Math.Abs(DateDiff(DateInterval.Day, Now.Date, oRequest.RequestDate.Date))
                        oMsgParams = New Generic.List(Of String)
                        If intFromDays > 1 Then
                            oMsgParams.Add(intFromDays)
                            txtRequestDateDays.Text = Me.Language.Translate("Request.RequestDate.FromDays", Me.DefaultScope, oMsgParams)
                        ElseIf intFromDays = 1 Then
                            txtRequestDateDays.Text = Me.Language.Translate("Request.RequestDate.FromOneDay", Me.DefaultScope, oMsgParams)
                        Else
                            ''txtRequestDateDays.Text = Me.Language.Translate("Request.RequestDate.FromToday", Me.DefaultScope, oMsgParams)
                        End If

                        ' Estado solicitud
                        Dim oStatusRow As DataRow = Nothing
                        Dim tbRequestStates As DataTable = GetRequestStatesFromApplication(True)
                        oRow = tbRequestStates.Select("ElementName='" & oRequest.RequestStatus.ToString & "'")
                        If oRow.Length > 0 Then oStatusRow = oRow(0)
                        oMsgParams = New Generic.List(Of String)
                        oMsgParams.Add(oStatusRow("ElementDesc"))
                        Dim xLastApprovalDate As Date = oRequest.RequestDate
                        If oRequest.RequestApprovals IsNot Nothing AndAlso oRequest.RequestApprovals.Count > 0 Then
                            xLastApprovalDate = oRequest.RequestApprovals(oRequest.RequestApprovals.Count - 1).ApprovalDateTime
                        End If
                        Dim intLastApprovalDays As Integer = Math.Abs(DateDiff(DateInterval.Day, Now.Date, xLastApprovalDate.Date))
                        If intLastApprovalDays > 1 Then
                            oMsgParams.Add(intLastApprovalDays)
                            txtRequestState.Text = Me.Language.Translate("Request.StatusInfo.FromDays", Me.DefaultScope, oMsgParams)
                        ElseIf intLastApprovalDays = 1 Then
                            txtRequestState.Text = Me.Language.Translate("Request.StatusInfo.FromOneDay", Me.DefaultScope, oMsgParams)
                        Else
                            txtRequestState.Text = Me.Language.Translate("Request.StatusInfo.FromToday", Me.DefaultScope, oMsgParams)
                        End If
                        ' Determinamos el nombre de la imagen del estado de la solicitud
                        strImg = "../Requests/Images/RequestStates/16/"
                        If oStatusRow IsNot Nothing Then
                            strImg &= oStatusRow("ElementName") & ".png"
                        Else
                            strImg &= "UnknownState.png"
                        End If
                        Me.imgRequestState.Src = strImg
                        If oRequest.RequestApprovals IsNot Nothing AndAlso oRequest.RequestApprovals.Count > 0 Then
                            oMsgParams = New Generic.List(Of String)
                            Dim oPassport As roPassportTicket = API.SecurityServiceMethods.GetPassportTicket(Me, oRequest.RequestApprovals(oRequest.RequestApprovals.Count - 1).IDPassport)

                            If oRequest.RequestStatus <> eRequestStatus.Canceled Then
                                If Not oPassport Is Nothing Then
                                    oMsgParams.Add(oPassport.Name)
                                Else
                                    oMsgParams.Add(" -Desconocido- ")
                                End If
                            Else
                                If oEmployee IsNot Nothing Then oMsgParams.Add(oEmployee.Name)
                            End If

                            If oRequest.RequestStatus = eRequestStatus.OnGoing Or oRequest.RequestStatus = eRequestStatus.Accepted Then
                                txtLastApproval.Text = Me.Language.Translate("Request.LastActionInfo.Accepted.Passport", Me.DefaultScope, oMsgParams)
                            ElseIf oRequest.RequestStatus = eRequestStatus.Denied Then
                                txtLastApproval.Text = Me.Language.Translate("Request.LastActionInfo.Denied.Passport", Me.DefaultScope, oMsgParams)
                            ElseIf oRequest.RequestStatus = eRequestStatus.Canceled Then
                                txtLastApproval.Text = Me.Language.Translate("Request.LastActionInfo.Canceled.Passport", Me.DefaultScope, oMsgParams)
                            End If
                        End If

                        Me.lnkRequestApprovals.Attributes("onclick") = "javascript: var url = 'Requests/RequestApprovals.aspx?IDRequest=" & oRequest.ID.ToString & "'; " &
                                                                   "parent.ShowExternalForm2(url, 700, 415, '', '', false, false, false);"

                        Me.lnkRequestOrgChart.Attributes("onclick") = ""
                        Me.lnkRequestOrgChart.Style("display") = "none"

                        ' Comentarios empleado
                        If oRequest.Comments <> "" Then
                            Me.txtComments.Text = oRequest.Comments
                        Else
                            Me.lblComments.Visible = False
                        End If

                        ' Botones aprobar/denegar solicitud
                        Dim intLastApprovalIDPassport As Integer = -1
                        If oRequest.RequestApprovals IsNot Nothing AndAlso oRequest.RequestApprovals.Count > 0 Then
                            intLastApprovalIDPassport = oRequest.RequestApprovals(oRequest.RequestApprovals.Count - 1).IDPassport
                        End If
                        Dim bolApproveEnabled As Boolean = False
                        Dim bolRefuseEnabled As Boolean = False
                        If oRequest.RequestStatus = eRequestStatus.Pending AndAlso oRequestPermission > Permission.Read Then
                            If oRequest.StatusLevel <= intLevelOfAuthority Then
                                ' Si esta pendiente y el nivel de mando de la solicitud es igual o inferior, quiere decir que
                                ' se ha creado directamente en este nivel para ser aprobado por un superior,
                                ' entonces si tienes el mismo nivel no puedes gestionarla
                                bolApproveEnabled = False
                                bolRefuseEnabled = False
                            Else
                                bolApproveEnabled = True
                                bolRefuseEnabled = True
                            End If
                        ElseIf oRequest.RequestStatus = eRequestStatus.OnGoing Then

                            If oRequest.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso oRequestPermission > Permission.Read AndAlso oRequest.StatusLevel.HasValue = False Then
                                bolApproveEnabled = True
                                bolRefuseEnabled = True
                            Else
                                If oRequest.StatusLevel > intLevelOfAuthority AndAlso oRequestPermission > Permission.Read Then
                                    bolApproveEnabled = True
                                    bolRefuseEnabled = True
                                Else
                                    'Una vez aprobada o denegada ya no se puede modificar (aunque sea el mismo supervisor)
                                    bolApproveEnabled = False
                                    bolRefuseEnabled = False
                                End If
                            End If
                        Else
                            'La solicitud ya ha sido aprobada o denegada definitivamente. No se puede deshacer.
                            bolApproveEnabled = False
                            bolRefuseEnabled = False
                        End If

                        If oRequest.RequestType = eRequestType.ExchangeShiftBetweenEmployees AndAlso oRequest.RequestStatus = eRequestStatus.Pending Then
                            bolApproveEnabled = False
                            bolRefuseEnabled = False
                        End If

                        'Validación de aprobaciones y rechazos automáticos por el usuario de sistema

                        If oRequest.AutomaticValidation = True Then

                            If oRequest.RequestStatus <> eRequestStatus.Denied Then
                                bolApproveEnabled = False
                                bolRefuseEnabled = False
                            Else
                                If oRequest.RequestApprovals.Count <> 0 Then
                                    Dim oPassport As roPassportTicket = API.SecurityServiceMethods.GetPassportTicket(Me, oRequest.RequestApprovals(oRequest.RequestApprovals.Count - 1).IDPassport)
                                    If oRequest.RequestStatus = eRequestStatus.Denied AndAlso oPassport.ID = roConstants.GetSystemUserId() Then
                                        bolApproveEnabled = True
                                    Else
                                        bolApproveEnabled = False
                                        bolRefuseEnabled = False
                                    End If
                                End If
                            End If

                        End If

                        If bolApproveEnabled Then
                            Me.lnkApprove.Attributes("onclick") = "showComments('" & oRequest.ID.ToString & "', '" & IdTableRow & "', '0'); return false;"
                        Else
                            Me.lnkApprove.Attributes("onclick") = "return false;"
                        End If
                        Me.divApproveLink.Attributes("class") &= IIf(Not bolApproveEnabled, "Disabled", "")
                        Me.lblApproveLink.Enabled = False

                        If bolRefuseEnabled Then
                            Me.lnkRefuse.Attributes("onclick") = "showComments('" & oRequest.ID.ToString & "', '" & IdTableRow & "', '1'); return false;"
                        Else
                            Me.lnkRefuse.Attributes("onclick") = "return false;"
                        End If
                        Me.divRefuseLink.Attributes("class") &= IIf(Not bolRefuseEnabled, "Disabled", "")
                        Me.lblRefuseLink.Enabled = False

                        ' Obtenemos la información del contenido de la solicitud
                        Me.lblRequestInfo.Text = API.RequestServiceMethods.GetRequestInfo(Me, oRequest.ID, True)

                        Select Case oRequest.RequestType
                            Case eRequestType.UserFieldsChange
                                Me.LoadUserFieldChangeRequestData(oRequest)

                            Case eRequestType.ForbiddenPunch,
                             eRequestType.JustifyPunch,
                             eRequestType.ExternalWorkResumePart, eRequestType.ForbiddenTaskPunch
                                Me.LoadUserForbiddenPunchRequestData(oRequest)

                            Case eRequestType.ChangeShift,
                             eRequestType.VacationsOrPermissions,
                             eRequestType.PlannedAbsences,
                             eRequestType.CancelHolidays,
                             eRequestType.PlannedHolidays
                                Me.LoadPlanificationRequestData(oRequest)
                            Case eRequestType.ExternalWorkWeekResume
                                frmDayDetails1.createRequestDaysResume(oRequest.RequestType, oRequest.RequestDays)
                                Me.trHolidaysResume.Style("display") = ""
                                Me.lblRequestDaysResume.Attributes("onclick") = "showPlannedHolidaysResume(" & oRequest.ID & ");"
                            Case eRequestType.ExchangeShiftBetweenEmployees

                            Case eRequestType.DailyRecord
                                Me.LoadDailyRecordRequestData(oRequest)

                        End Select

                        ' Mostrar información del siguiente supervisor que tiene que cursar la solicitud
                        If oRequest.NextLevelPassports <> "" Then
                            divShowSupervisorsPending.Style("display") = "none"
                            divLblSupervisorsPending.Style("display") = ""
                            oMsgParams = New Generic.List(Of String)
                            oMsgParams.Add(oRequest.NextLevelPassports)
                            Me.lblNextLevelPassports.Text = Me.Language.Translate("Request.NextApprovalInfo", Me.DefaultScope, oMsgParams)
                        Else
                            divShowSupervisorsPending.Style("display") = ""
                            divLblSupervisorsPending.Style("display") = "none"
                            aAcept.Attributes("onclick") = "showPendingSupervisors(" & oRequest.ID & ");return false;"
                        End If
                    Else
                        Me.divGeneral.Style("display") = "none"
                    End If
                Else
                    Me.divGeneral.Style("display") = "none"
                End If
            Else
                Me.divGeneral.Style("display") = "none"
            End If
        Catch ex As Exception
            Response.Write(ex.Message.ToString & ex.StackTrace.ToString)
        End Try

    End Sub

    Private Sub ApproveRefuseRequest(ByVal intIDRequest As Integer, ByVal bolApprove As Boolean, ByVal _Comments As String, Optional ByVal _CheckLockedDays As Boolean = False, Optional ByVal _ForceApprove As Boolean = False)
        Try
            Dim rError As roJSON.JSONError

            If API.RequestServiceMethods.ApproveRefuse(Me, intIDRequest, WLHelperWeb.CurrentPassport.ID, bolApprove, _Comments, _CheckLockedDays, _ForceApprove) = False Then
                If roWsUserManagement.SessionObject.States.RequestState.Result = RequestResultEnum.ExistsLockedDaysInPeriod Then
                    rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.RequestState.ErrorText)
                    ' ...
                ElseIf roWsUserManagement.SessionObject.States.RequestState.Result = RequestResultEnum.NotEnoughConceptBalance Then

                    If HelperSession.AdvancedParametersCache("ConfirmHolidayOverFlow").Trim = "1" Then
                        rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.RequestState.ErrorText & "." & Me.Language.Translate("AssignHolidays.Sure", Me.DefaultScope), RequestResultEnum.NotEnoughConceptBalance.ToString())
                    Else
                        rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.RequestState.ErrorText)
                    End If
                ElseIf roWsUserManagement.SessionObject.States.RequestState.Result = RequestResultEnum.NeedConfirmation Then
                    rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.RequestState.ErrorText, RequestResultEnum.NeedConfirmation.ToString())
                ElseIf roWsUserManagement.SessionObject.States.RequestState.Result = RequestResultEnum.RequestRuleError Then
                    rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.RequestState.ErrorText, RequestResultEnum.RequestRuleError.ToString())
                Else
                    rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.RequestState.ErrorText)
                End If
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & ex.StackTrace.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub GetBarButtons()
        Try
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\General\Requests\Requests", WLHelperWeb.CurrentPassportID)

            Dim destDiv As HtmlGenericControl = roTools.BuildCentralBar(guiActions, "-1", Me.Language, Me.DefaultScope, "Requests")

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            destDiv.RenderControl(htw)

            Response.Write(sw.ToString)
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Function CreateTabs(ByVal ActiveTab As Integer) As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing, Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabHistory", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
        Next

        oTabButtons(ActiveTab).Attributes("class") = "bTab-active"

        Return hTableGen ' Retorna el HTMLTable

    End Function

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

    Private Sub createRowLineRequest(ByRef oRequestData As DataRow, ByRef oTypeRow As DataRow, ByVal strListType As String)
        Try

            Dim oRequestPermission As Permission = Permission.None

            ' Obtenemos la configuración de permiso en función del tipo de solicitud y el empleado
            Dim oRequestTypeSecurity As roRequestTypeSecurity = API.RequestServiceMethods.GetRequestTypeSecurity(Me, oRequestData("RequestType"))
            If oRequestTypeSecurity IsNot Nothing Then
                oRequestPermission = Me.GetFeaturePermissionByEmployee(oRequestTypeSecurity.SupervisorFeatureName, oRequestData("IDEmployee"), "U")
            End If

            If oRequestPermission > Permission.None Then

                Dim htRequestRow As New HtmlTableRow
                htRequestRow.ID = "htRequestRow" & strListType & "_" & oRequestData("ID")
                htRequestRow.Attributes("class") = "RequestsListRow"
                htRequestRow.Attributes("onmouseover") = "OverRow(this);"
                htRequestRow.Attributes("onmouseout") = "OutRow(this);"
                htRequestRow.Attributes("style") = "cursor: pointer;"
                htRequestRow.Attributes("onclick") = "SelectRow(this, '" & htRequestRow.ID & "');"

                Dim oRequestResume As New Requests_WebUserControls_frmRequestResume
                oRequestResume = LoadControl("WebUserControls/frmRequestResume.ascx")
                oRequestResume.ID = "frmRequestResume_" & oRequestData("ID")
                oRequestResume.SetRequestData(oRequestData, oTypeRow, oRequestPermission, htRequestRow.ID)

                Dim htCell As New HtmlTableCell
                htCell.VAlign = "top"
                htCell.Controls.Add(oRequestResume)
                htRequestRow.Cells.Add(htCell)

                Me.tblRequests.Rows.Add(htRequestRow)

                htRequestRow = New HtmlTableRow
                htCell = New HtmlTableCell
                htCell.VAlign = "top"
                htCell.Attributes("style") = "width: auto; padding-top: 0px; padding-left: 5px;"
                htCell.Align = "right"

                htRequestRow.Cells.Add(htCell)
                Me.tblRequests.Rows.Add(htRequestRow)

            End If
        Catch ex As Exception
            Response.Write(ex.Message & " " & ex.StackTrace)
        End Try
    End Sub

    Private Function CheckPermissionsAny() As Boolean

        ' Verificar permisos
        Dim bolHasPermission As Boolean = False
        Dim oRequestsTypeSecurity As Generic.List(Of roRequestTypeSecurity) = API.RequestServiceMethods.GetRequestTypeSecurityListAll(Me)
        For Each oRequestTypeSecurity As roRequestTypeSecurity In oRequestsTypeSecurity
            If Me.HasFeaturePermission(oRequestTypeSecurity.SupervisorFeatureName, Permission.Read) Then
                bolHasPermission = True
                Exit For
            End If
        Next

        Return bolHasPermission

    End Function

    Private Function CheckPermissions(ByVal _RequestType As eRequestType) As Boolean
        Return True
    End Function

    Private Sub LoadUserFieldChangeRequestData(ByVal oRequest As roRequest)

        Dim dsUserFields As DataSet = API.EmployeeServiceMethods.GetUserFieldsDataset(Me, oRequest.IDEmployee)

        Dim dTbl As DataTable = dsUserFields.Tables(0)

        Dim dv As New DataView(dTbl)
        dv.RowFilter = "FieldName = '" & oRequest.FieldName & "'"

        Dim oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None} ' Permiso configurado sobre la información de la ficha para los distintos niveles de acceso del empleado actual ('Employees.UserFields.Information.Low', 'Employees.UserFields.Information.Medium', 'Employees.UserFields.Information.High')
        oUserFieldsAccessPermission(0) = Me.GetFeaturePermissionByEmployee("Employees.UserFields.Information.Low", oRequest.IDEmployee)
        oUserFieldsAccessPermission(1) = Me.GetFeaturePermissionByEmployee("Employees.UserFields.Information.Medium", oRequest.IDEmployee)
        oUserFieldsAccessPermission(2) = Me.GetFeaturePermissionByEmployee("Employees.UserFields.Information.High", oRequest.IDEmployee)

        Dim Columns() As String = {"FieldCaption", "Value"}
        Dim htUserFields As HtmlTable = creaGridUserFields(dv.ToTable(), Columns, oUserFieldsAccessPermission, oRequest.IDEmployee, , , , False)
        Me.tdUserFields.Controls.Add(htUserFields)

        Me.lnkEmployeeUserFields.Attributes("onclick") = "showEmployee(null, '" & oRequest.IDEmployee.ToString & "','" & Configuration.RootUrl & "');"

        Me.trUserFieldsChange.Visible = True

    End Sub

    Private Sub LoadUserForbiddenPunchRequestData(ByVal oRequest As roRequest)

        Me.lnkEmployeeMoves.Attributes("onclick") = "javascript: var url = 'Scheduler/MovesNew.aspx?GroupID=-1'; " &
                                                    "url = url + '&EmployeeID=" & oRequest.IDEmployee.ToString & "&Date=" & Format(oRequest.Date1, "dd/MM/yyyy") & "'; " &
                                                    "parent.ShowExternalForm2(url, 1400, 620, '', '', false, false, false);"
        Me.trForbiddenPunch.Visible = True

    End Sub

    Private Sub LoadDailyRecordRequestData(ByVal oRequest As roRequest)

        Me.lnkEmployeeMovesDR.Attributes("onclick") = "javascript: var url = 'Scheduler/MovesNew.aspx?GroupID=-1'; " &
                                                    "url = url + '&EmployeeID=" & oRequest.IDEmployee.ToString & "&Date=" & Format(oRequest.Date1, "dd/MM/yyyy") & "'; " &
                                                    "parent.ShowExternalForm2(url, 1400, 620, '', '', false, false, false);"
        Me.trDailyRecordResume.Visible = True

    End Sub

    Private Sub LoadPlanificationRequestData(ByVal oRequest As roRequest)

        Me.lnkPlanificationAnual.Attributes("onclick") = "showEmpAnnualDetail('" & oRequest.IDEmployee.ToString & "');"

        Dim oMobility As roMobility = API.EmployeeServiceMethods.GetCurrentMobility(Me, oRequest.IDEmployee)
        If oMobility IsNot Nothing Then
            Dim intTab As Integer = 1
            If oRequest.RequestType = eRequestType.PlannedAbsences Then
                intTab = 0
            End If
            Dim xStartDate As Date = oRequest.Date1.Value.Date
            Dim xEndDate As Date = oRequest.Date2.Value.Date
            If Math.Abs(DateDiff(DateInterval.Day, xStartDate, xEndDate)) < 7 Then
                xStartDate = Me.GetMondayDate(xStartDate)
                xEndDate = Me.GetSundayDate(xEndDate)
            End If

            Dim strStartDateJS As String = "new Date(" & xStartDate.Year & "," & (xStartDate.Month - 1) & "," & xStartDate.Day & ")"
            Dim strEndDateJS As String = "new Date(" & xEndDate.Year & "," & (xEndDate.Month - 1) & "," & xEndDate.Day & ")"

            Me.lnkPlanificationGroup.Attributes("onclick") = "showPlanificationGroup('" & oMobility.IdGroup.ToString & "'," & strStartDateJS & "," & strEndDateJS & "," & intTab.ToString & ",'" & Configuration.RootUrl & "');"

            Me.lblPlanificationGroupLink.Text = Me.lblPlanificationGroupLink.Text.Replace("{0}", oMobility.Name)
        Else
            Me.lnkPlanificationGroup.Style("display") = "none"
        End If

        If oRequest.RequestType = eRequestType.VacationsOrPermissions Then

            Me.trVacationsResume.Style("display") = ""
            Me.trHolidaysResume.Style("display") = "none"
            Dim htVacationsResume As HtmlTable = creaGridVacationsResume(oRequest.IDEmployee, oRequest.IDShift, oRequest.Date1)
            Me.tdVacationsResume.Controls.Add(htVacationsResume)
        ElseIf oRequest.RequestType = eRequestType.PlannedHolidays Then
            Me.trVacationsResume.Style("display") = ""
            Dim htVacationsResume As HtmlTable = creaGridPlannedHolidaysResume(oRequest)
            Me.tdVacationsResume.Controls.Add(htVacationsResume)
        End If

        If (oRequest.RequestType = eRequestType.VacationsOrPermissions OrElse
           oRequest.RequestType = eRequestType.CancelHolidays OrElse
           oRequest.RequestType = eRequestType.PlannedHolidays) AndAlso (oRequest.RequestDays IsNot Nothing AndAlso oRequest.RequestDays.Count > 0) Then
            frmDayDetails1.createRequestDaysResume(oRequest.RequestType, oRequest.RequestDays)

            Me.trHolidaysResume.Style("display") = ""
            Me.lblRequestDaysResume.Attributes("onclick") = "showPlannedHolidaysResume(" & oRequest.ID & ");"
        End If

        Me.trPlanification.Visible = True

    End Sub

    Private Function creaGridUserFields(ByVal dTable As DataTable, ByVal ColumnNames() As String, ByVal oUserFieldsAccessPermission() As Permission, ByVal _IDEmployee As Integer, Optional ByVal dColField As String = "", Optional ByVal editMode As Boolean = False, Optional ByVal arrErrors As DataTable = Nothing, Optional ByVal bolShowCategories As Boolean = True) As HtmlTable
        ''Try
        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        hTable.Attributes("class") = "GridStyle GridEmpleados"
        hTable.Attributes("width") = "100%"

        'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.Width = "250"
        hTCell.Attributes("style") = "border-right: 0; width: 250px"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Field", Me.DefaultScope) ' "Campo"
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.InnerHtml = Me.Language.Translate("UserFieldsGrid.Columns.Value", Me.DefaultScope) '"Valor"
        hTCell.ColSpan = 2
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim Categories() As String = API.UserFieldServiceMethods.GetCategories(Me, True)
        If Categories IsNot Nothing Then

            ReDim Preserve Categories(Categories.Length)
            Categories(Categories.Length - 1) = ""

            Dim Rows() As DataRow
            Dim bolEditable As Boolean = False

            Dim intCountHigh As Integer = 0

            For Each strCategory As String In Categories

                ' Obtenemos los campos correspondientes a la categoría
                Rows = dTable.Select("Category = '" & strCategory & "'" & IIf(strCategory = "", " OR Category IS NULL", ""), "FieldCaption")

                If Rows.Length > 0 Then

                    If bolShowCategories Then
                        ' Pinta la fila con el nombre de la categoría actual
                        hTRow = New HtmlTableRow
                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cell3"
                        hTCell.Style("padding") = "3px"
                        hTCell.ColSpan = 3
                        If strCategory <> "" Then
                            hTCell.InnerText = strCategory
                        Else
                            hTCell.InnerText = Me.Language.Translate("UserField.Category.None", Me.DefaultScope)
                        End If
                        hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell3"
                        hTRow.Cells.Add(hTCell)
                        hTable.Rows.Add(hTRow)
                    End If

                    ' Bucle por los campos de la categoría actual
                    For Each oRow As DataRow In Rows

                        hTRow = New HtmlTableRow
                        altRow = IIf(altRow = "1", "2", "1")

                        ' Pinta columna nombre campo
                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cell" & altRow
                        hTCell.Attributes.Add("nowrap", "nowrap")
                        hTCell.Style("padding") = "3px"
                        hTCell.InnerText = oRow("FieldCaption").ToString
                        If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                        hTCell.Attributes("title") = roTypes.Any2String(oRow("Description"))
                        hTRow.Cells.Add(hTCell)

                        ' Pinta columna valor
                        Dim strValue As String = oRow("Value").ToString

                        Dim strValueDate As String = Format(CDate(oRow("Date").ToString), HelperWeb.GetShortDateFormat)
                        If CDate(oRow("Date").ToString) = New Date(1900, 1, 1) Then strValueDate = ""

                        Dim strBegin As String = ""
                        Dim strEnd As String = ""
                        Dim strLanguageKey As String
                        Dim xDate As Date

                        'Select Case oRow("Type")
                        Select Case CType(oRow("Type"), FieldTypes)
                            Case FieldTypes.tDate
                                If oRow("ValueDateTime").ToString <> "" Then
                                    strValue = Format(CDate(oRow("ValueDateTime").ToString), HelperWeb.GetShortDateFormat)
                                End If
                            Case FieldTypes.tTime
                                ''If oRow("ValueDateTime").ToString <> "" Then
                                ''    strValue = Format(CDate(oRow("ValueDateTime").ToString), HelperWeb.GetShortTimeFormat)
                                ''End If
                            Case FieldTypes.tDatePeriod
                                If strValue.Split("*")(0) <> "" AndAlso strValue.Split("*")(0).Length = 10 Then
                                    xDate = New Date(strValue.Split("*")(0).Substring(0, 4), strValue.Split("*")(0).Substring(5, 2), strValue.Split("*")(0).Substring(8, 2))
                                    strBegin = Format(xDate, HelperWeb.GetShortDateFormat)
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1).Length = 10 Then
                                    xDate = New Date(strValue.Split("*")(1).Substring(0, 4), strValue.Split("*")(1).Substring(5, 2), strValue.Split("*")(1).Substring(8, 2))
                                    strEnd = Format(xDate, HelperWeb.GetShortDateFormat)
                                End If
                            Case FieldTypes.tTimePeriod
                                If strValue.Split("*")(0) <> "" AndAlso strValue.Split("*")(0).Length = 5 Then
                                    xDate = New Date(1900, 1, 1, strValue.Split("*")(0).Substring(0, 2), strValue.Split("*")(0).Substring(3, 2), 0)
                                    strBegin = Format(xDate, HelperWeb.GetShortTimeFormat)
                                End If
                                If strValue.Split("*").Length > 1 AndAlso strValue.Split("*")(1).Length = 5 Then
                                    xDate = New Date(1900, 1, 1, strValue.Split("*")(1).Substring(0, 2), strValue.Split("*")(1).Substring(3, 2), 0)
                                    strEnd = Format(xDate, HelperWeb.GetShortTimeFormat)
                                End If
                            Case Else
                        End Select

                        Select Case CType(oRow("AccessLevel"), Integer)
                            Case 0 ' Low
                                bolEditable = (oUserFieldsAccessPermission(0) >= Permission.Write)
                            Case 1 ' Medium
                                bolEditable = (oUserFieldsAccessPermission(1) >= Permission.Write)
                            Case 2 ' High
                                bolEditable = (oUserFieldsAccessPermission(2) >= Permission.Write)
                            Case Else
                                bolEditable = False
                        End Select

                        hTCell = New HtmlTableCell
                        hTCell.Attributes("class") = "GridStyle-cell" & altRow

                        Dim divShowValue As HtmlGenericControl = Nothing

                        If oRow("AccessLevel") = 2 Then
                            Dim tbValue As New HtmlTable
                            Dim rwValue As New HtmlTableRow
                            Dim clValue As New HtmlTableCell

                            Dim aAnchorShowValue As New HtmlAnchor
                            With aAnchorShowValue
                                .ID = "aShowValue_" & intCountHigh.ToString
                                .HRef = "javascript:void(0);"
                                .Attributes("class") = "UserFieldShowHideValueAnchor"
                                .Attributes("onclick") = "ShowUserFieldValue(this, '" & intCountHigh & "');"
                                .InnerHtml = Me.Language.Translate("UserField.HighLevel.ShowCaption", Me.DefaultScope) ' "Mostrar valor ..."
                            End With
                            'hTCell.Controls.Add(aAnchorShowValue)
                            Dim aAnchorHideValue As New HtmlAnchor
                            With aAnchorHideValue
                                .ID = "aHideValue_" & intCountHigh.ToString
                                .HRef = "javascript:void(0);"
                                .Style("display") = "none"
                                .Attributes("class") = "UserFieldShowHideValueAnchor"
                                .Attributes("onclick") = "HideUserFieldValue(this, '" & intCountHigh & "');"
                                .InnerHtml = Me.Language.Translate("UserField.HighLevel.HideCaption", Me.DefaultScope) ' "Ocultar valor ..."
                            End With
                            'hTCell.Controls.Add(aAnchorHideValue)
                            divShowValue = New HtmlGenericControl("div")
                            With divShowValue
                                .ID = "divShowValue_" & intCountHigh.ToString
                                .Attributes("class") = "UserFieldShowHideValueDiv"
                                .Style("text-align") = "left"
                                .Style("width") = "100%"
                                .Style("display") = "none"
                            End With
                            'hTCell.Controls.Add(divShowValue)

                            tbValue.Style("width") = "100%"
                            clValue.Controls.Add(aAnchorShowValue)
                            clValue.Controls.Add(divShowValue)
                            rwValue.Cells.Add(clValue)
                            clValue = New HtmlTableCell
                            clValue.Attributes("align") = "right"
                            clValue.Controls.Add(aAnchorHideValue)
                            rwValue.Cells.Add(clValue)
                            tbValue.Rows.Add(rwValue)

                            hTCell.Controls.Add(tbValue)

                            intCountHigh += 1
                        End If

                        If Not editMode Or Not bolEditable Then ' En modo normal

                            hTCell.Style("padding") = "3px"

                            Select Case CType(oRow("Type"), FieldTypes)
                                Case FieldTypes.tDatePeriod
                                    If strBegin <> "" Or strEnd <> "" Then
                                        Dim oParams As New Generic.List(Of String)
                                        If strBegin <> "" Then oParams.Add(strBegin)
                                        If strEnd <> "" Then oParams.Add(strEnd)
                                        strLanguageKey = "DatePeriod.FieldCaption"
                                        If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                        If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                        strValue = Me.Language.Translate(strLanguageKey, Me.DefaultScope, oParams)
                                    Else
                                        strValue = ""
                                    End If
                                Case FieldTypes.tTimePeriod
                                    If strBegin <> "" Or strEnd <> "" Then
                                        Dim oParams As New Generic.List(Of String)
                                        If strBegin <> "" Then oParams.Add(strBegin)
                                        If strEnd <> "" Then oParams.Add(strEnd)
                                        strLanguageKey = "TimePeriod.FieldCaption"
                                        If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                        If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                        strValue = Me.Language.Translate(strLanguageKey, Me.DefaultScope, oParams)
                                    Else
                                        strValue = ""
                                    End If
                                Case Else
                            End Select

                            If oRow("AccessLevel") <> 2 Then
                                If strValue = "" Then
                                    hTCell.InnerText = " "
                                Else
                                    hTCell.InnerHtml = strValue
                                End If
                                ' Si tiene control de histórico muestra tooltip con la fecha de incio de validez
                                If roTypes.Any2Boolean(oRow("History")) And strValueDate <> "" Then
                                    Dim oParams As New Generic.List(Of String)
                                    oParams.Add(Format(CDate(oRow("Date")), HelperWeb.GetShortDateFormat))
                                    hTCell.Attributes.Add("title", Me.Language.Translate("FieldValueDate.Info", Me.DefaultScope, oParams))
                                End If
                            Else
                                divShowValue.InnerHtml = strValue
                                ' Si tiene control de histórico muestra tooltip con la fecha de incio de validez
                                If roTypes.Any2Boolean(oRow("History")) And strValueDate <> "" Then
                                    Dim oParams As New Generic.List(Of String)
                                    oParams.Add(Format(CDate(oRow("Date")), HelperWeb.GetShortDateFormat))
                                    divShowValue.Attributes.Add("title", Me.Language.Translate("FieldValueDate.Info", Me.DefaultScope, oParams))
                                End If
                            End If

                            hTRow.Cells.Add(hTCell)

                            ' Pinta columna con link a edición histórico valores
                            Dim aAnchorHistory As HtmlAnchor = Nothing
                            If Not editMode And roTypes.Any2Boolean(oRow("History")) Then
                                aAnchorHistory = New HtmlAnchor
                                With aAnchorHistory
                                    .ID = "aHistory_" & intCountHigh.ToString
                                    .HRef = "javascript:void(0);"
                                    .Attributes("class") = "UserFieldShowHideValueAnchor"
                                    .Attributes("onclick") = "ShowUserFieldHistory('" & _IDEmployee.ToString & "', '" & oRow("FieldCaption").ToString & "');"
                                    .InnerText = Me.Language.Translate("UserField.History.ShowCaption", Me.DefaultScope) ' "Histórico"
                                End With
                            End If
                            hTCell = New HtmlTableCell
                            hTCell.Attributes("class") = "GridStyle-cell" & altRow & " GridStyle-cellHistory"
                            hTCell.Attributes.Add("nowrap", "nowrap")
                            hTable.Style("border-left") = "none"
                            hTCell.Style("padding") = "3px"
                            If aAnchorHistory IsNot Nothing Then hTCell.Controls.Add(aAnchorHistory)
                            'If hTCell.InnerText = "" Then hTCell.InnerText = " "
                            If aAnchorHistory IsNot Nothing Then hTCell.Attributes("title") = Me.Language.Translate("UserField.History.ShowTitle", Me.DefaultScope) ' "Mostrar detalle histórico"
                            hTRow.Cells.Add(hTCell)
                        Else ' En modo edición

                            Dim strHtmlError As String = ""
                            Dim strEditCss As String = "textEdit x-form-text x-form-field"
                            hTCell.Style("padding") = "0px"
                            If arrErrors IsNot Nothing Then
                                'Si es comproben els errors
                                Dim dRows() As DataRow = arrErrors.Select("FieldName = '" & oRow(dColField).ToString & "'")
                                If dRows.Length > 0 Then
                                    'hTCell.InnerHtml = "<input type=""text"" style=""width: 100%; color: #666666; font-size: 11px; background-color: #ffffff; border: solid 1px #7D8BA3; padding-left: 2px; background-color: #FEFE9B;"" id=""" & oRow(dColField).ToString & """ value=""" & dRows(0).Item("FieldValue") & """ />"
                                    strValue = roTypes.Any2String(dRows(0).Item("FieldValue"))
                                    strHtmlError = "<br />" &
                                                   "<span style=""padding-left: 5px; color: red"">" & dRows(0).Item("ErrorDescription").ToString & "</span>"
                                    strEditCss &= " textEditError"
                                    If roTypes.Any2String(dRows(0).Item("BeginEnd")) = "BeginPeriod" Then
                                        strBegin = dRows(0).Item("FieldValue")
                                    ElseIf roTypes.Any2String(dRows(0).Item("BeginEnd")) = "EndPeriod" Then
                                        strEnd = dRows(0).Item("FieldValue")
                                    End If
                                End If
                            End If
                            'hTCell.InnerHtml = "<input type=""text"" class=""textEdit"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dTable.Rows(n)("Value").ToString & """  onblur=""this.className='textEdit';"" onfocus=""this.className='textEdit-focus';""  />"
                            Dim oContainer As Object
                            If oRow("AccessLevel") <> 2 Then
                                oContainer = hTCell
                            Else
                                oContainer = divShowValue
                            End If

                            Dim cnListTable As HtmlTable = Nothing

                            Dim strOnChangeScript As String = "UserFieldValueChange('" & oRow("FieldCaption").ToString & "', '" & Format(Now.Date, HelperWeb.GetShortDateFormat) & "');"

                            'Select oRow("Type")
                            Select Case CType(oRow("Type"), FieldTypes)
                                Case FieldTypes.tDate
                                    'hTCell.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & "-focus';"" style=""width:75px;"" ConvertControl=""DatePicker""  />"
                                    oContainer.InnerHtml = "<input type=""text"" id=""" & oRow(dColField).ToString & """ value=""" & strValue & """ style=""width:75px;"" ConvertControl=""DatePicker"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tTime
                                    oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""TextField"" CCregex=""/^([0-9]?[0-9]?[0-9]?[0-9]):([0-5][0-9])$/"" CCmaxLength=""7"" CCtime=""false"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tText
                                    oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""TextField"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tNumeric
                                    oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""NumberField"" CCallowDecimals=""false"" CCmaxValue=""2147483647"" CCminValue=""-2147483648"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tDecimal
                                    oContainer.InnerHtml = "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & """ value=""" & strValue.Replace(HelperWeb.GetDecimalDigitFormat, ".") & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & " x-form-focus';"" ConvertControl=""NumberField"" CCallowDecimals=""true"" CCdecimalPrecision=""3"" CCmaxValueText=""16"" CConchange=""" & strOnChangeScript & """ />"
                                Case FieldTypes.tList
                                    oContainer.InnerHtml = ""

                                    Dim cnList As New Robotics.WebControls.roComboBox
                                    cnList.AutoResizeChildsWidth = True
                                    cnList.ParentWidth = "100%"
                                    cnList.ChildsWidth = "100%"
                                    Dim cnComboValue As New Web.UI.HtmlControls.HtmlInputText
                                    cnComboValue.ID = oRow(dColField).ToString
                                    cnComboValue.Style.Add("display", "none")
                                    ' Cargamos valores lista al combo
                                    Dim oUserFieldInfo As roUserField = API.UserFieldServiceMethods.GetUserField(Me, oRow("FieldCaption").ToString, Types.EmployeeField, False, False)
                                    If oUserFieldInfo IsNot Nothing Then
                                        For Each strItem As String In oUserFieldInfo.ListValues
                                            cnList.AddItem(strItem, strOnChangeScript)
                                        Next
                                    End If
                                    'oContainer.Controls.Add(cnList)
                                    cnList.Value = strValue
                                    cnList.SelectedValue = strValue
                                    cnComboValue.Value = strValue
                                    'oContainer.Controls.Add(cnComboValue)
                                    cnList.HiddenValue = cnComboValue.ClientID

                                    cnListTable = New HtmlTable
                                    cnListTable.Width = "100%"
                                    cnListTable.Rows.Add(New HtmlTableRow)
                                    Dim cnListCell As HtmlTableCell
                                    ' Columna con el combo y el campo oculto con el valor
                                    cnListCell = New HtmlTableCell
                                    cnListCell.Controls.Add(cnList)
                                    cnListCell.Controls.Add(cnComboValue)
                                    cnListTable.Rows(0).Cells.Add(cnListCell)
                                    ' Columna con el texto 'Válido a partir de' (sólo si hay control histórico)
                                    cnListCell = New HtmlTableCell
                                    ''If roTypes.Any2Boolean(oRow("History")) Then
                                    ''    cnListCell.Style.Add("padding-left", "5px")
                                    ''    cnListCell.InnerText = Me.Language.Translate("FieldValueDate.Label", Me.DefaultScope)
                                    ''End If
                                    cnListTable.Rows(0).Cells.Add(cnListCell)
                                    ' Columna con el campo fecha inicio de validez (sólo si hay control histórico)
                                    cnListCell = New HtmlTableCell
                                    Dim cnFieldDate As New HtmlControls.HtmlInputText()
                                    With cnFieldDate
                                        .ID = oRow(dColField).ToString & "_@@Date@@"
                                        .Style.Add("width", "75px")
                                        .Attributes.Add("ConvertControl", "DatePicker")
                                        .Attributes.Add("CCvisible", IIf(roTypes.Any2Boolean(oRow("History")), "true", "false"))
                                        .Value = strValueDate
                                    End With
                                    cnListCell.Style.Add("padding-left", "5px")
                                    cnListCell.Controls.Add(cnFieldDate)
                                    cnListTable.Rows(0).Cells.Add(cnListCell)

                                    oContainer.controls.add(cnListTable)

                                Case FieldTypes.tDatePeriod
                                    oContainer.InnerHtml = "<table><tr>"
                                    oContainer.InnerHtml &= "<td style=""padding-right:3px;"">" & Me.Language.Translate("DatePeriod.FieldEdit.BeginTitle", Me.DefaultScope) & "</td>"
                                    oContainer.InnerHtml &= "<td width=""100px""><input type=""text"" id=""" & oRow(dColField).ToString & "_##BeginPeriod##"" value=""" & strBegin & """ style=""width:75px;"" ConvertControl=""DatePicker"" CConchange=""" & strOnChangeScript & """ CCvtype=""daterange"" CCendDateField=""" & oRow(dColField).ToString & "_##EndPeriod##"" /></td>"
                                    oContainer.InnerHtml &= "<td style=""padding-right:3px;"">" & Me.Language.Translate("DatePeriod.FieldEdit.EndTitle", Me.DefaultScope) & "</td>"
                                    oContainer.InnerHtml &= "<td><input type=""text"" id=""" & oRow(dColField).ToString & "_##EndPeriod##"" value=""" & strEnd & """ style=""width:75px;"" ConvertControl=""DatePicker"" CConchange=""" & strOnChangeScript & """ CCvtype=""daterange"" CCstartDateField=""" & oRow(dColField).ToString & "_##BeginPeriod##"" /></td>"
                                    oContainer.InnerHtml &= "</tr></table>"
                                Case FieldTypes.tTimePeriod
                                    oContainer.InnerHtml = Me.Language.Translate("TimePeriod.FieldEdit.BeginTitle", Me.DefaultScope)
                                    oContainer.InnerHtml &= "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & "_##BeginPeriod##"" value=""" & strBegin & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & "-focus';"" style=""width:40px;"" ConvertControl=""TextField"" CCtime=""true"" CConchange=""" & strOnChangeScript & """ />"
                                    oContainer.InnerHtml &= Me.Language.Translate("TimePeriod.FieldEdit.EndTitle", Me.DefaultScope)
                                    oContainer.InnerHtml &= "<input type=""text"" class=""" & strEditCss & """ id=""" & oRow(dColField).ToString & "_##EndPeriod##"" value=""" & strEnd & """  onblur=""this.className='" & strEditCss & "';"" onfocus=""this.className='" & strEditCss & "-focus';"" style=""width:40px;"" ConvertControl=""TextField"" CCtime=""true"" CConchange=""" & strOnChangeScript & """ />"

                            End Select

                            If oRow("Type") <> FieldTypes.tList Then
                                Dim strInnerHtml As String = "<table style=""width:100%;""><tr>"

                                If roTypes.Any2Boolean(oRow("History")) = False Then
                                    strInnerHtml &= "<td colspan=""3"">" & oContainer.InnerHtml & "</td>"
                                Else
                                    strInnerHtml &= "<td >" & oContainer.InnerHtml & "</td>" &
                                                    "<td align=""right""><table><tr>"
                                    strInnerHtml &= "<td style=""padding-left: 5px;"">" & Me.Language.Translate("FieldValueDate.Label", Me.DefaultScope) & "</td>"
                                    strInnerHtml &= "<td style=""padding-left: 2px;""><input type=""text"" id=""" & oRow(dColField).ToString & "_@@Date@@" & """ value=""" & strValueDate & """ " &
                                                                                            "style=""width:75px;"" ConvertControl=""DatePicker"" CCvisible=""" & IIf(roTypes.Any2Boolean(oRow("History")), "true", "false") & """  /></td>"
                                    strInnerHtml &= "</tr></table></td>"

                                End If
                                strInnerHtml &= "</tr></table>"
                                oContainer.InnerHtml = strInnerHtml
                            End If

                            If strHtmlError <> "" Then
                                If oRow("AccessLevel") <> 2 And oRow("Type") <> FieldTypes.tList Then
                                    oContainer.InnerHtml &= strHtmlError
                                ElseIf oRow("AccessLevel") = 2 Then
                                    Dim tbValue As HtmlTable = hTCell.Controls(0)
                                    Dim rwValue As New HtmlTableRow
                                    Dim clValue As New HtmlTableCell
                                    clValue.InnerHtml = strHtmlError
                                    rwValue.Cells.Add(clValue)
                                    tbValue.Rows.Add(rwValue)
                                Else
                                    Dim tbValue As HtmlTable = hTCell.Controls(1)
                                    Dim rwValue As New HtmlTableRow
                                    Dim clValue As New HtmlTableCell
                                    clValue.InnerHtml = strHtmlError
                                    rwValue.Cells.Add(clValue)
                                    tbValue.Rows.Add(rwValue)
                                End If
                            End If

                        End If
                        ' Cerramos la fila
                        hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
                        hTCell.ColSpan = 1
                        hTRow.Cells.Add(hTCell)

                        hTable.Rows.Add(hTRow)

                    Next

                End If

            Next

        End If

        ' ''Bucle als registres
        ''For n As Integer = 0 To dTable.Rows.Count - 1
        ''    hTRow = New HtmlTableRow
        ''    altRow = IIf(altRow = "1", "2", "1")

        ''For y As Integer = 0 To dTable.Columns.Count - 1
        ''    'Si es la columna del id
        ''    If dTable.Columns(y).ColumnName = dColField Then Continue For
        ''    hTCell = New HtmlTableCell

        ''    'Cambia el alternateRow
        ''    hTCell.Attributes("class") = "GridStyle-cell" & altRow

        ''    If (y = 2 And editMode = True) Then
        ''        hTCell.Style("padding") = "0px"
        ''        If arrErrors IsNot Nothing Then
        ''            'Si es comproben els errors
        ''            Dim dRows() As DataRow = arrErrors.Select("FieldName = '" & dTable.Rows(n)(dColField).ToString & "'")
        ''            If dRows.Length > 0 Then
        ''                hTCell.InnerHtml = "<input type=""text"" style=""width: 100%; color: #666666; font-size: 11px; background-color: #ffffff; border: solid 1px #7D8BA3; padding-left: 2px; background-color: #FEFE9B;"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dRows(0).Item("FieldValue") & """ /><br />" & _
        ''                "<span style=""padding-left: 5px; color: red"">" & dRows(0).Item("ErrorDescription").ToString & "</span>"
        ''            Else
        ''                hTCell.InnerHtml = "<input type=""text"" class=""textEdit"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dTable.Rows(n)(y).ToString & """ onblur=""this.className='textEdit';"" onfocus=""this.className='textEdit-focus';"" />"
        ''            End If
        ''        Else
        ''            hTCell.InnerHtml = "<input type=""text"" class=""textEdit"" id=""" & dTable.Rows(n)(dColField).ToString & """ value=""" & dTable.Rows(n)(y).ToString & """  onblur=""this.className='textEdit';"" onfocus=""this.className='textEdit-focus';""  />"
        ''        End If

        ''    Else
        ''        hTCell.Style("padding") = "3px"
        ''        hTCell.InnerText = dTable.Rows(n)(y).ToString
        ''    End If
        ''    If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
        ''    'Si es la ultima columna (per tancar el row)
        ''    If y = dTable.Columns.Count - 1 Then hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow

        ''    'Carrega la celda al row
        ''    hTRow.Cells.Add(hTCell)
        ''Next
        ''hTable.Rows.Add(hTRow)
        ''Next
        Return hTable
        ''Catch ex As Exception
        ''    Dim hTableError As New HtmlTable
        ''    Dim hTableCellError As New HtmlTableCell
        ''    Dim hTableRowError As New HtmlTableRow
        ''    hTableCellError.InnerHtml = ex.Message.ToString
        ''    hTableRowError.Cells.Add(hTableCellError)
        ''    hTableError.Rows.Add(hTableRowError)
        ''    Return hTableError
        ''End Try
    End Function

    Private Function creaGridPlannedHolidaysResume(ByVal oRequest As roRequest) As HtmlTable

        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        Dim intDone As Double = 0
        Dim intPending As Double = 0
        Dim intLasting As Double = 0
        Dim intDisponible As Double = 0

        'If API.EmployeeServiceMethods.VacationsResumeQuery(Me, oRequest.IDEmployee, 0, Now.Date, oRequest.Date1, intDone, intPending, intLasting, intDisponible) Then

        If API.EmployeeServiceMethods.ProgrammedHolidaysResumeQuery(Me, oRequest.IDEmployee, oRequest.IDCause.Value, Now.Date, oRequest.Date1, intPending, intLasting, intDisponible) Then
            hTable.Attributes("class") = "GridStyle GridEmpleados"

            hTRow = New HtmlTableRow

            'hTCell = New HtmlTableCell
            'hTCell.Attributes("class") = "GridStyle-cellheader"
            'hTCell.Attributes("style") = "border-right: 0px; "
            'hTCell.InnerHtml = Me.Language.Translate("VacationsResumeGrid.Columns.Done", Me.DefaultScope)
            'hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("ProgrammedHolidaysResumeQuery.Columns.Actual", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("ProgrammedHolidaysResumeQuery.Columns.Pending", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("ProgrammedHolidaysResumeQuery.Columns.Lasting", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("ProgrammedHolidaysResumeQuery.Columns.Total", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            hTRow = New HtmlTableRow
            altRow = IIf(altRow = "1", "2", "1")

            ' Pinta días ya disfrutados
            'hTCell = New HtmlTableCell
            'hTCell.Attributes("class") = "GridStyle-cell" & altRow
            'hTCell.Attributes.Add("nowrap", "nowrap")
            'hTCell.Style("padding") = "3px"
            'hTCell.Attributes("style") = "border-right: 0px; "
            'hTCell.InnerText = intDone.ToString
            'If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            'hTRow.Cells.Add(hTCell)

            ' Pinta días disponibles
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerText = roConversions.ConvertHoursToTime(intDisponible.ToString)
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            ' Pinta días solicitados pendientes de procesar
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerText = roConversions.ConvertHoursToTime(intPending)
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            ' Pinta días solicitados y aprobados pendientes de disfrutar
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.InnerText = roConversions.ConvertHoursToTime(intLasting.ToString)
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            ' Pinta días solicitados y aprobados pendientes de disfrutar
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.InnerText = roConversions.ConvertHoursToTime(intDisponible - intPending - intLasting)
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

        End If

        Return hTable

    End Function

    Private Function creaGridVacationsResume(ByVal _IDEmployee As Integer, ByVal _IDShift As Integer, ByVal VacationsDate As DateTime) As HtmlTable

        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        Dim intDone As Double = 0
        Dim intPending As Double = 0
        Dim intLasting As Double = 0
        Dim intDisponible As Double = 0
        Dim intExpiredDays As Double = 0
        Dim intDaysWithoutEnjoyment As Double = 0

        Dim bIsAnualYearWork As Boolean = False

        If API.EmployeeServiceMethods.VacationsResumeQuery(Me, _IDEmployee, _IDShift, Now.Date, VacationsDate, intDone, intPending, intLasting, intDisponible, intExpiredDays, intDaysWithoutEnjoyment) Then

            Dim oShift = API.ShiftServiceMethods.GetShift(Me, _IDShift, False)
            If oShift.ShiftType = Robotics.Base.DTOs.ShiftType.Vacations AndAlso oShift.IDConceptBalance <> 0 Then
                Dim oConcept As Robotics.Base.VTBusiness.Concept.roConcept = API.ConceptsServiceMethods.GetConceptByID(Nothing, CInt(oShift.IDConceptBalance), False)
                If oConcept.DefaultQuery = "L" Then
                    bIsAnualYearWork = True
                End If
            End If

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            hTRow = New HtmlTableRow

            If (Not bIsAnualYearWork) Then
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cellheader"
                hTCell.Attributes("style") = "border-right: 0px; "
                hTCell.InnerHtml = Me.Language.Translate("VacationsResumeGrid.Columns.Done", Me.DefaultScope)
                hTRow.Cells.Add(hTCell)
            End If

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("VacationsResumeGrid.Columns.Actual", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Pending", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Lasting", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            If bIsAnualYearWork Then
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cellheader"
                hTCell.Attributes("style") = "border-right: 0px; "
                hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Expired", Me.DefaultScope)
                hTRow.Cells.Add(hTCell)

                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cellheader"
                hTCell.Attributes("style") = "border-right: 0px; "
                hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.WithoutEnjoyment", Me.DefaultScope)
                hTRow.Cells.Add(hTCell)
            End If

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Total", Me.DefaultScope)
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            hTRow = New HtmlTableRow
            altRow = IIf(altRow = "1", "2", "1")

            If (Not bIsAnualYearWork) Then
                ' Pinta días ya disfrutados
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.Attributes("style") = "border-right: 0px; "
                hTCell.InnerText = intDone.ToString
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)
            End If

            ' Pinta días disponibles
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerText = intDisponible.ToString
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            ' Pinta días solicitados pendientes de procesar
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.Attributes("style") = "border-right: 0px; "
            hTCell.InnerText = intPending.ToString
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            ' Pinta días solicitados y aprobados pendientes de disfrutar
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.InnerText = intLasting.ToString
            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            If bIsAnualYearWork Then
                ' Pinta días que caducarán
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.InnerText = intExpiredDays.ToString
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                ' Pinta días no disponibles
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.InnerText = intDaysWithoutEnjoyment.ToString
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)
            End If

            ' Pinta saldo restante final previsto
            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cell" & altRow
            hTCell.Attributes.Add("nowrap", "nowrap")
            hTCell.Style("padding") = "3px"
            hTCell.InnerText = roTypes.Any2String(intDisponible - intPending - intLasting - intExpiredDays - intDaysWithoutEnjoyment)

            If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

        End If

        Return hTable

    End Function

    Private Function GetMondayDate(ByVal xDate As Date) As Date

        Dim xRet As Date = xDate
        If xDate.DayOfWeek <> DayOfWeek.Monday Then
            If xDate.DayOfWeek = DayOfWeek.Sunday Then
                xRet = xRet.AddDays(-6)
            Else
                xRet = xRet.AddDays(-(xDate.DayOfWeek - 1))
            End If
        End If

        Return xRet

    End Function

    Private Function GetSundayDate(ByVal xDate As Date) As Date

        Dim xRet As Date = xDate
        If xDate.DayOfWeek <> DayOfWeek.Sunday Then
            xRet = xRet.AddDays(7 - xDate.DayOfWeek)
        End If

        Return xRet

    End Function

    Private Function GetRequestTypesFromApplication(Optional ByVal bReload As Boolean = False) As DataTable
        Dim tbRequestTypes As DataTable = Nothing
        Try

            If bReload Or HttpContext.Current.Application(WLHelperWeb.CompanyToken & "_Requests_RequestTypes") Is Nothing Then
                tbRequestTypes = API.RequestServiceMethods.GetRequestTypes(Me)
                HttpContext.Current.Application.Lock()
                HttpContext.Current.Application.Remove(WLHelperWeb.CompanyToken & "_Requests_RequestTypes")
                If tbRequestTypes IsNot Nothing Then
                    HttpContext.Current.Application.Add(WLHelperWeb.CompanyToken & "_Requests_RequestTypes", tbRequestTypes)
                End If
                HttpContext.Current.Application.UnLock()
            Else
                tbRequestTypes = HttpContext.Current.Application(WLHelperWeb.CompanyToken & "_Requests_RequestTypes")
            End If
        Catch
        End Try
        Return tbRequestTypes
    End Function

    Private Function GetRequestStatesFromApplication(Optional ByVal bReload As Boolean = False) As DataTable
        Dim tbRequestStates As DataTable = Nothing
        Try

            If bReload Or HttpContext.Current.Application("Requests_RequestStates") Is Nothing Then
                tbRequestStates = API.RequestServiceMethods.GetRequestStates(Me)
                HttpContext.Current.Application.Lock()
                HttpContext.Current.Application.Remove("Requests_RequestStates")
                If tbRequestStates IsNot Nothing Then
                    HttpContext.Current.Application.Add("Requests_RequestStates", tbRequestStates)
                End If
                HttpContext.Current.Application.UnLock()
            Else
                tbRequestStates = HttpContext.Current.Application("Requests_RequestStates")
            End If
        Catch
        End Try
        Return tbRequestStates
    End Function

End Class