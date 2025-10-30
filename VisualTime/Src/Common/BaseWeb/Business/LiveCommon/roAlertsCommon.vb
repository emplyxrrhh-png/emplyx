Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class roAlertsCommon

    Public Enum eDocumentAlertType
        DocumentsValidation = 0
        LeaveDocuments = 1
        MandatoryDocuments = 2
        GpaAlerts = 3
        WorkForecastDocuments = 4
        LeaveDocumentsValidationPending = 5
        LeaveDocumentsUndelivered = 6
        WorkForecastDocumentsValidationPending = 7
        WorkForecastDocumentsUndelivered = 8
        AccessAuthorizationDocuments = 9
        AccessAuthorizationDocumentsValidationsPending = 10
        AccessAuthorizationDocumentsUndelivered = 11
    End Enum

    Public Shared Function BuildAlertDiv(ByVal dRow As DataRow, ByVal bIsUserTask As Boolean, ByVal oRequest As System.Web.HttpRequest) As String
        Dim alertDiv As String = String.Empty

        If bIsUserTask Then
            alertDiv &= "<div class='Alert_BoxLine'>" & getLink(dRow, oRequest)
            If roTypes.Any2Boolean(dRow("isurgent")) Then
                alertDiv &= "<div style='float:left;min-width:35px'><img class='Alerts_Warning' src='Images/Warning.png' /></div>"
            Else
                alertDiv &= "<div style='float:left;min-width:35px'>&nbsp;</div>"
            End If

            alertDiv &= "<div style='float:left;'><span>" + roTypes.Any2String(dRow("desktopdescription")) + "</span></div>"

            If roTypes.Any2Boolean(dRow("IsCritic")) Then
                alertDiv &= "<div style='float:left;min-width:35px'><img class='Alerts_Warning' src='Images/DoubleWarning.png' /></div>"
            Else
                alertDiv &= "<div style='float:left;min-width:35px'>&nbsp;</div>"
            End If

            alertDiv &= "</a></div>"
        Else
            Dim bHasLink As Boolean = False
            Dim sAdviceLink As String = getAdviceLink(dRow, bHasLink, oRequest)

            'If sAdviceLink.Length > 0 Then
            alertDiv &= "<div class='Alert_BoxLine'>" & sAdviceLink
            If bHasLink Then
                alertDiv &= "<div style='float:left;min-width:35px'><img class='Alerts_Warning' src='Images/Warning.png' /></div>"
            Else
                alertDiv &= "<div style='float:left;min-width:35px'>&nbsp;</div>"
            End If
            alertDiv &= "<div style='float:left;width:90%'><span>" & roTypes.Any2String(dRow("MessageEx")) & "</span></div>"

            alertDiv &= "</a></div>"
            'End If
        End If
        Return alertDiv
    End Function

    Public Shared Function BuildDocumentAlertsDiv(ByVal tsAlerts As DocumentAlerts, ByVal docType As DocumentType, ByVal idRelatedObject As Integer, ByVal oLanguageWeb As roLanguageWeb, ByVal langScope As String, ByVal oRequest As System.Web.HttpRequest, Optional ByVal eForecastType As ForecastType = ForecastType.Any) As String
        Dim alertDiv As String = String.Empty

        Dim bLiteCharge As Boolean = True
        If (eForecastType = ForecastType.Any OrElse eForecastType = ForecastType.AnyAbsence) Then bLiteCharge = False

        If tsAlerts.DocumentsValidation.Count > 0 Then
            Dim params As New Generic.List(Of String)
            params.Add(tsAlerts.DocumentsValidation.Count)

            alertDiv &= roAlertsCommon.BuildAlertDiv(tsAlerts.DocumentsValidation.Count, tsAlerts.DocumentsValidation.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.DocumentsValidation.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate("Alert.Documents." & docType.ToString & ".AlertValidation", langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.DocumentsValidation, docType, idRelatedObject, oRequest, bLiteCharge)
        End If

        If tsAlerts.AbsenteeismDocuments.Count > 0 AndAlso (eForecastType = ForecastType.Any OrElse eForecastType = ForecastType.AnyAbsence OrElse eForecastType = ForecastType.AbsenceDays OrElse eForecastType = ForecastType.AbsenceHours OrElse eForecastType = ForecastType.Leave) Then
            Dim params As New Generic.List(Of String)
            Dim sDescKey As String
            Dim sDescDocsNames As String = String.Empty
            Dim iTot As Integer
            If Not bLiteCharge Then
                sDescKey = "Alert.Documents." & docType.ToString & ".AlertAbsenteeismUndelivered"

                iTot = tsAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count
                params.Add(iTot)
                If iTot > 0 Then
                    alertDiv &= roAlertsCommon.BuildAlertDiv(iTot, tsAlerts.AbsenteeismDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.AbsenteeismDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.LeaveDocumentsUndelivered, docType, idRelatedObject, oRequest, bLiteCharge)
                End If

                sDescKey = "Alert.Documents." & docType.ToString & ".AlertAbsenteeismValidationPending"
                iTot = tsAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count
                params.Clear()
                params.Add(iTot)
                If iTot > 0 Then
                    alertDiv &= roAlertsCommon.BuildAlertDiv(iTot, tsAlerts.AbsenteeismDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.AbsenteeismDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.LeaveDocumentsValidationPending, docType, idRelatedObject, oRequest, bLiteCharge)
                End If
            Else
                For Each oAlert As DocumentAlert In tsAlerts.AbsenteeismDocuments
                    If sDescDocsNames.Trim.Length = 0 Then
                        sDescDocsNames = oAlert.DocumentTemplateName
                    Else
                        sDescDocsNames = sDescDocsNames & " " & oLanguageWeb.Translate("Alert.Documents.And", langScope, params) & " " & oAlert.DocumentTemplateName
                    End If
                Next
                params.Add(sDescDocsNames)
                If tsAlerts.AbsenteeismDocuments.Count > 1 Then
                    sDescKey = "Alert.Documents." & docType.ToString & ".AlertAbsenteeism.Lite"
                Else
                    If tsAlerts.AbsenteeismDocuments(0).IDDocument > 0 Then
                        sDescKey = "Alert.Documents." & docType.ToString & ".AlertAbsenteeism.ValidationPending.One.Lite"
                    Else
                        sDescKey = "Alert.Documents." & docType.ToString & ".AlertAbsenteeism.One.Lite"
                    End If

                End If

                alertDiv &= roAlertsCommon.BuildAlertDiv(tsAlerts.AbsenteeismDocuments.Count, tsAlerts.AbsenteeismDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.AbsenteeismDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.LeaveDocuments, docType, idRelatedObject, oRequest, bLiteCharge)
            End If

        End If

        If tsAlerts.MandatoryDocuments.Count > 0 Then
            Dim params As New Generic.List(Of String)
            params.Add(tsAlerts.MandatoryDocuments.Count)

            alertDiv &= roAlertsCommon.BuildAlertDiv(tsAlerts.MandatoryDocuments.Count, tsAlerts.MandatoryDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.MandatoryDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate("Alert.Documents." & docType.ToString & ".AlertMandatory", langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.MandatoryDocuments, docType, idRelatedObject, oRequest, bLiteCharge)
        End If

        If tsAlerts.WorkForecastDocuments.Count > 0 AndAlso (eForecastType = ForecastType.Any OrElse eForecastType = ForecastType.OverWork) Then
            Dim params As New Generic.List(Of String)
            Dim sDescKey As String
            Dim iTot As Integer

            sDescKey = "Alert.Documents." & docType.ToString & ".AlertForecastDocumentsUndelivered"
            iTot = tsAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count
            params.Add(iTot)
            If iTot > 0 Then
                alertDiv &= roAlertsCommon.BuildAlertDiv(iTot, tsAlerts.WorkForecastDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.WorkForecastDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.WorkForecastDocumentsUndelivered, docType, idRelatedObject, oRequest, bLiteCharge)
            End If

            sDescKey = "Alert.Documents." & docType.ToString & ".AlertForecastDocumentsValidationPending"
            iTot = tsAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count
            params.Clear()
            params.Add(iTot)
            If iTot > 0 Then
                alertDiv &= roAlertsCommon.BuildAlertDiv(iTot, tsAlerts.WorkForecastDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.WorkForecastDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.WorkForecastDocumentsValidationPending, docType, idRelatedObject, oRequest, bLiteCharge)
            End If

        End If

        If tsAlerts.AccessAuthorizationDocuments.Count > 0 Then
            Dim params As New Generic.List(Of String)
            Dim sDescKey As String
            Dim iTot As Integer

            sDescKey = "Alert.Documents." & docType.ToString & ".AuthorizationAccessDocumentsUndelivered"
            iTot = tsAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count
            params.Add(iTot)
            If iTot > 0 Then
                alertDiv &= roAlertsCommon.BuildAlertDiv(iTot, tsAlerts.AccessAuthorizationDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.AccessAuthorizationDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.AccessAuthorizationDocumentsUndelivered, docType, idRelatedObject, oRequest, bLiteCharge)
            End If

            sDescKey = "Alert.Documents." & docType.ToString & ".AuthorizationAccessDocumentsValidationPending"
            iTot = tsAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count
            params.Clear()
            params.Add(iTot)
            If iTot > 0 Then
                alertDiv &= roAlertsCommon.BuildAlertDiv(iTot, tsAlerts.AccessAuthorizationDocuments.ToList().FindAll(Function(x) x.IsUrgent = True).Count > 0,
                                                     tsAlerts.AccessAuthorizationDocuments.ToList().FindAll(Function(x) x.IsCritic = True).Count > 0, oLanguageWeb.Translate(sDescKey, langScope, params),
                                                     roAlertsCommon.eDocumentAlertType.AccessAuthorizationDocumentsValidationsPending, docType, idRelatedObject, oRequest, bLiteCharge)
            End If

        End If

        Return alertDiv
    End Function

    Public Shared Function BuildAlertDiv(ByVal iNumObjects As Integer, ByVal isUrgent As Boolean, ByVal isCritic As Boolean, ByVal lngTag As String, ByVal eType As eDocumentAlertType, ByVal docType As DocumentType, ByVal idRelatedObject As Integer, ByVal oRequest As System.Web.HttpRequest, Optional ByVal bLiteCharge As Boolean = False) As String
        Dim alertDiv As String = String.Empty
        If iNumObjects > 0 Then
            Dim cssStyle As String = String.Empty
            If bLiteCharge Then cssStyle = "font-size:15px !important;word-wrap:initial !important;white-space: nowrap;text-overflow: ellipsis;"

            alertDiv &= "<div class='Alert_BoxLine'" & If(bLiteCharge, "style='max-width:750px'", "") & ">" & GetDocumentLink(eType, idRelatedObject, docType, oRequest)
            If isUrgent Then
                alertDiv &= "<div style='float:left;min-width:35px'><img class='Alerts_Warning' src='../Alerts/Images/Warning.png' /></div>"
            Else
                alertDiv &= "<div style='float:left;min-width:35px'>&nbsp;</div>"
            End If

            alertDiv &= "<div style='float:left;" & If(bLiteCharge, "max-width:680px;overflow: hidden;", "") & "'><span style='" & cssStyle & "'>" & lngTag & "</span></div>"

            If isCritic Then
                alertDiv &= "<div style='float:left;min-width:35px'><img class='Alerts_Warning' src='../Alerts/Images/DoubleWarning.png' /></div>"
            Else
                alertDiv &= "<div style='float:left;min-width:35px'>&nbsp;</div>"
            End If

            alertDiv &= "</a></div>"
        End If

        Return alertDiv
    End Function

    Private Shared Function GetDocumentLink(ByVal eType As eDocumentAlertType, ByVal idRelatedObject As Integer, ByVal docType As DocumentType, ByVal oRequest As System.Web.HttpRequest) As String
        Dim link As String = ""
        Try

            Select Case eType
                Case eDocumentAlertType.DocumentsValidation, eDocumentAlertType.WorkForecastDocuments, eDocumentAlertType.WorkForecastDocumentsUndelivered, eDocumentAlertType.WorkForecastDocumentsValidationPending
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=-1&DocumentAlertType=" & roTypes.Any2Integer(eType).ToString & "&DocumentType=" & docType.ToString.ToUpper & "&IdRelatedObject=" & idRelatedObject.ToString
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case eDocumentAlertType.LeaveDocuments, eDocumentAlertType.LeaveDocumentsUndelivered, eDocumentAlertType.LeaveDocumentsValidationPending
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=-1&DocumentAlertType=" & roTypes.Any2Integer(eType).ToString & "&DocumentType=" & docType.ToString.ToUpper & "&IdRelatedObject=" & idRelatedObject.ToString
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case eDocumentAlertType.MandatoryDocuments
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=-1&DocumentAlertType=" & roTypes.Any2Integer(eType).ToString & "&DocumentType=" & docType.ToString.ToUpper & "&IdRelatedObject=" & idRelatedObject.ToString
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case eDocumentAlertType.AccessAuthorizationDocuments, eDocumentAlertType.AccessAuthorizationDocumentsUndelivered, eDocumentAlertType.AccessAuthorizationDocumentsValidationsPending
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=-1&DocumentAlertType=" & roTypes.Any2Integer(eType).ToString & "&DocumentType=" & docType.ToString.ToUpper & "&IdRelatedObject=" & idRelatedObject.ToString
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case eDocumentAlertType.GpaAlerts
                    'Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=-1&DocumentAlertType=" & roTypes.Any2Integer(eType).ToString & "&DocumentType=" & docType.ToString.ToUpper & "&IdRelatedObject=" & idRelatedObject.ToString
                    'link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                    link = "<a href='" + oRequest.ApplicationPath + "#" + IIf(Configuration.RootUrl(0) = "/", Configuration.RootUrl, "/" + Configuration.RootUrl) + "/Absences/AbsencesStatus?IDEmployee=" & idRelatedObject & "&Status=all&DocumentsDelivered=0'  target='_blank'>"
                Case Else

            End Select
        Catch ex As Exception
        End Try
        Return link
    End Function

    Private Shared Function getAdviceLink(ByVal dRow As DataRow, ByRef bolHasLink As Boolean, ByVal oRequest As System.Web.HttpRequest) As String
        Dim strLink As String = ""

        Try
            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            Dim resolver As String = dRow("ResolverURL")
            Dim bolAction As Boolean = False
            Dim bolLink As Boolean = False
            Dim strDestination As String = ""

            strClickEdit = "ShowResolveUserTask("
            strClickRemove = "ShowRemoveUserTask("

            Dim strParameter1 As String = String.Empty
            Dim strParameter2 As String = String.Empty
            Dim strParameter3 As String = String.Empty

            Select Case resolver
                Case "FN:\\Resolver_MovesInvalidCardID"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Employees.IdentifyMethods", "U", Permission.Write)
                    bolLink = False
                Case "FN:\\Resolver_Over_MaxJobEmployees_Soon", "FN:\\Resolver_Over_MaxEmployees_Soon"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Administration.Security", "U", Permission.Write)
                    bolLink = False
                Case "FN:\\Resolver_Terminal_Unrecognized"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Terminals.Definition", "U", Permission.Write)
                    bolLink = False
                Case "FN:\\Resolver_Coverage", "FN:\\Resolver_Absence"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Calendar", "U", Permission.Write)
                    bolLink = False
                    strParameter1 = dRow.Item(7)
                    strParameter2 = dRow.Item(9)
                Case "FN:\\Resolver_CloseDateAlert"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Administration.Security", "U", Permission.Write)
                    bolLink = True
                    strDestination = oRequest.ApplicationPath + "#" + IIf(Configuration.RootUrl(0) = "/", Configuration.RootUrl, "/" + Configuration.RootUrl) + "/Security/LockDB"
                Case "FN:\\Resolver_TerminalDisconnected"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Terminals.Definition", "U", Permission.Read) AndAlso API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Terminals.StatusInfo", "U", Permission.Read)
                    bolLink = True
                    strDestination = oRequest.ApplicationPath + "#" + IIf(Configuration.RootUrl(0) = "/", Configuration.RootUrl, "/" + Configuration.RootUrl) + "/Terminals/Terminals"
                Case "FN:\\mxC_NotRegistered"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Terminals.Definition", "U", Permission.Write)
                    bolLink = False
                    strParameter1 = roTypes.Any2String(dRow("ID")).Replace("USERTASK:\\MXC_NOTREGISTERED", "")
                Case "FN:\\TERMINAL_NotRegistered"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Terminals.Definition", "U", Permission.Write)
                    bolLink = False
                    strParameter1 = roTypes.Any2String(dRow("ID")).Replace("USERTASK:\\TERMINAL_NOTREGISTERED", "")
                    strParameter2 = roTypes.Any2String(dRow("ResolverValue2"))
                    strParameter3 = roTypes.Any2String(dRow("ResolverValue3"))
                Case "FN:\\Resolver_ParserInvalidEntries"
                    bolAction = API.SecurityServiceMethods.HasPermissionOverFeature(Nothing, "Calendar.Punches", "U", Permission.Write)
                    bolLink = False
                Case Else
                    bolAction = False
                    bolLink = False
            End Select

            strClickEdit &= """" & resolver.ToString & """,""" & strParameter1 & """,""" & strParameter2 & """,""" & strParameter3 & """)"
            strClickRemove &= """" & resolver.ToString & """,""" & strParameter1 & """,""" & strParameter2 & """,""" & strParameter3 & """)"

            If bolAction Then
                bolHasLink = True

                If bolLink Then
                    strLink = "<a href='" & strDestination & "' Target='_blank'>"
                Else
                    strLink = "<a href='#' onclick='" & strClickEdit & "'>"
                    'strLink &= "<a href='#' onclick='" & strClickRemove & "'>"
                End If
            Else
                bolHasLink = False
            End If
        Catch ex As Exception
        End Try
        Return strLink

    End Function

    Private Shared Function getLink(ByVal row As DataRow, ByVal oRequest As System.Web.HttpRequest) As String
        Dim link As String = ""
        Try

            Select Case roTypes.Any2Integer(row("idalerttype"))
                Case DTOs.eNotificationType.Day_With_Unmatched_Time_Record '19
                    link = "<a href='#' onclick='ShowIncompletedDays(""" + roTypes.Any2DateTime(row("datefrom")).ToString("dd/MM/yyyy") + """,""" + roTypes.Any2DateTime(row("dateto")).ToString("dd/MM/yyyy") + """);'>"
                Case DTOs.eNotificationType.Day_with_Unreliable_Time_Record '20
                    link = "<a href='#' onclick='ShowNotReliabledDays(""" + roTypes.Any2DateTime(row("datefrom")).ToString("dd/MM/yyyy") + """,""" + roTypes.Any2DateTime(row("dateto")).ToString("dd/MM/yyyy") + """);'>"
                Case DTOs.eNotificationType.Non_Justified_Incident '21
                    link = "<a href='#' onclick='ShowNotJustifiedDays(""" + roTypes.Any2DateTime(row("datefrom")).ToString("dd/MM/yyyy") + """,""" + roTypes.Any2DateTime(row("dateto")).ToString("dd/MM/yyyy") + """);'>"
                Case DTOs.eNotificationType.Request_Pending '40
                    link = "<a href='" + oRequest.ApplicationPath + "#" + IIf(Configuration.RootUrl(0) = "/", Configuration.RootUrl, "/" + Configuration.RootUrl) + "/Requests/Requests' target='_blank'>"
                Case DTOs.eNotificationType.Employee_Not_Arrived_or_Late '15
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Employee_Present_With_Expired_Documents '41
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Document_Not_Delivered '34
                    link = "<a href='" + oRequest.ApplicationPath + "#" + IIf(Configuration.RootUrl(0) = "/", Configuration.RootUrl, "/" + Configuration.RootUrl) + "/Absences/AbsencesStatus?Status=all&DocumentsDelivered=0'  target='_blank'>"
                Case DTOs.eNotificationType.LabAgree_Max_Exceeded '43
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.LabAgree_Min_Reached '44
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Task_Close_to_Start '24
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Task_Close_to_Finish '23
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Task_exceeding_Started_Date '29
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Task_Exceeding_Planned_Time '25
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Task_Exceeding_Finished_Date '26
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Task_With_ALerts '30
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Productive_Unit_Under_Coverage '54
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.InvalidPortalConsents '57
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.InvalidDesktopConsents '58
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.InvalidTerminalConsents '59
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.InvalidVisitsConsents '60
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Tasks_Request_complete '64
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.Punches_In_LockDate '68
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                Case DTOs.eNotificationType.NonSupervisedDepartments '87
                    Dim url As String = "../Alerts/AlertsDetail.aspx?NotificationType=" + roTypes.Any2Integer(row("idalerttype")).ToString & "&DocumentAlertType=-1&DocumentType=EMPLOYEE&IdRelatedObject=-1"
                    link = " <a href='#' onclick='showAlertDetailPopUp(""" + url + """);'>"
                    'link = "<a href='" + oRequest.ApplicationPath + "#" + IIf(Configuration.RootUrl(0) = "/", Configuration.RootUrl, "/" + Configuration.RootUrl) + "/Security/Supervisors' target='_blank'>"
                Case Else

            End Select
        Catch ex As Exception
        End Try
        Return link
    End Function

End Class