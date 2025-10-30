Imports DevExpress.Web
Imports Robotics
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Audit
    Inherits PageBase

#Region "Declarations"

    Private oPermission As Permission
    Private Const FeatureAlias As String = "Administration.Audit"

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("Audit_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("Audit_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("Audit_iCurrentTask") = value
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

#Region "OBTENCION DE DATOS DE LINQ"

    Private Sub EmptyDataAudit()
        Session("AnalyticsAudit_AnaliticsData") = Nothing
    End Sub

    Private Function GetAuditData() As Object
        Return Session("AnalyticsAudit_AnaliticsData")
    End Function

#End Region

#Region "Events"
    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("audit", "~/Audit/Scripts/audit.js")
        Me.InsertExtraJavascript("flexmonster", "~/Base/flexmonster/flexmonster.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertExtraCssIncludes("~/Base/flexmonster/flexmonster.min.css", Me.Page)

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)

        If Not Me.HasFeaturePermission("Administration.Audit", Permission.Read) Then
            WLHelperWeb.RedirectAccessDenied(False)
            Return
        End If

        If Not Me.IsPostBack Then

            Dim xDate As Date = Now.AddHours(-2)
            Me.txtBeginDate.Date = xDate.Date
            Me.txtEndDate.Date = Now.Date

            LoadUsersCombo(True)
            LoadActionsCombo(True)
            LoadObjectTypeCombo(True)
            EmptyDataAudit()

            PerformActionCallback.JSProperties.Add("cpFlexBaseUrl", ResolveUrl("~/Base/Flexmonster/"))
#If DEBUG Then
            PerformActionCallback.JSProperties.Add("cpLCode", "Z7LE-XI104O-2L0I6K-1W1U56-2S6L5M-2N4Y07-2K0L37-160T5O-1O4U08-1M1X0S-2T1G45-1M3O0M-3417")
#Else
            PerformActionCallback.JSProperties.Add("cpLCode", API.CommonServiceMethods.GetRuntimeId())                    
#End If
        End If
        PerformActionCallback.JSProperties.Add("cpScriptVersion", VTLive40.Helpers.Constants.ScriptVersion)
    End Sub

#End Region

#Region "Methods"

    Private Function LoadActionsCombo(Optional ByVal bolReload As Boolean = False) As DataView

        Dim tb As DataTable = Session("Audit_AuditDataActions")
        Dim dv As DataView = Nothing

        If bolReload OrElse tb Is Nothing Then
            tb = API.AuditServiceMethods.GetAuditActions(Me)
            If tb IsNot Nothing Then
                If tb.Rows.Count = 0 Then
                    Dim oNew As DataRow = tb.NewRow
                    oNew("ActionId") = -1
                    oNew("ActionDesc") = ""
                    tb.Rows.Add(oNew)
                End If
                Session("Audit_AuditDataActions") = tb
                dv = New DataView(tb)
                dv.Sort = "ActionDesc ASC"
            End If
            Return dv
        Else
            dv = New DataView(tb)
            dv.Sort = "ActionDesc ASC"
            Return dv
        End If
    End Function

    Private Function LoadObjectTypeCombo(Optional ByVal bolReload As Boolean = False) As DataView
        Dim tb As DataTable = Session("Audit_AuditDataType")
        Dim dv As DataView = Nothing

        If bolReload OrElse tb Is Nothing Then
            tb = API.AuditServiceMethods.GetAuditObjectTypes(Me)
            If tb IsNot Nothing Then
                If tb.Rows.Count = 0 Then
                    Dim oNew As DataRow = tb.NewRow
                    oNew("ElementId") = -1
                    oNew("ElementDesc") = ""
                    tb.Rows.Add(oNew)
                End If
                Session("Audit_AuditDataType") = tb
                dv = New DataView(tb)
                dv.Sort = "ElementDesc ASC"
            End If
            Return dv
        Else
            dv = New DataView(tb)
            dv.Sort = "ElementDesc ASC"
            Return dv
        End If
    End Function

    Private Function LoadUsersCombo(Optional ByVal bolReload As Boolean = False) As DataView

        Dim tb As DataTable = Session("Audit_AuditUserNames")
        Dim dv As DataView = Nothing

        If bolReload OrElse tb Is Nothing Then
            tb = API.UserAdminServiceMethods.GetAuditPassports(Me.Page)
            If tb IsNot Nothing Then
                Session("Audit_AuditUserNames") = tb
                dv = New DataView(tb)
                dv.Sort = "UserName ASC"
            End If
            Return dv
        Else
            dv = New DataView(tb)
            dv.Sort = "UserName ASC"
            Return dv
        End If
    End Function

    Public Function ActionImage(ByVal oAction As Object) As String
        Dim strRet As String = Me.Page.ResolveUrl("~/Base/Images/Transparencia.gif")
        If Not IsDBNull(oAction) Then
            strRet = "Images/" & System.Enum.GetName(GetType(Robotics.VTBase.Audit.Action), oAction) & ".gif"
            Select Case CType(oAction, Robotics.VTBase.Audit.Action)
                Case Robotics.VTBase.Audit.Action.aConnect
                    strRet = "Images/aConnect.gif"
                Case Robotics.VTBase.Audit.Action.aDisconnect
                    strRet = "Images/aDisconnect.gif"
                Case Robotics.VTBase.Audit.Action.aSelect
                    strRet = "Images/aSelect.gif"
                Case Robotics.VTBase.Audit.Action.aMultiSelect
                    strRet = "Images/aMultiSelect.gif"
                Case Robotics.VTBase.Audit.Action.aInsert
                    strRet = "Images/aInsert.gif"
                Case Robotics.VTBase.Audit.Action.aUpdate
                    strRet = "Images/aUpdate.gif"
                Case Robotics.VTBase.Audit.Action.aDelete
                    strRet = "Images/aDelete.gif"
            End Select
        End If
        Return strRet
    End Function

    Public Function ObjectTypeImage(ByVal oObjectType As Object) As String
        Dim strRet As String = Me.Page.ResolveUrl("~/Base/Images/Transparencia.gif")
        If Not IsDBNull(oObjectType) AndAlso System.Enum.GetName(GetType(Robotics.VTBase.Audit.ObjectType), oObjectType) IsNot Nothing Then
            strRet = "Images/" & System.Enum.GetName(GetType(Robotics.VTBase.Audit.ObjectType), oObjectType) & ".gif"
        End If
        Return strRet
    End Function

#End Region


    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.ToUpper()
            Case "PERFORM_ACTION"
                EmptyDataAudit()
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.ExecuteBackgroundAnalytic()
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

                                If Session("AnalyticsAudit_AnaliticsData") Is Nothing Then

                                    PerformActionCallback.JSProperties.Add("cpAuditResult", Azure.RoAzureSupport.GetFileSaSTokenWithURI(oTask.ErrorCode, roLiveQueueTypes.analytics, False, ""))

                                End If

                                PerformActionCallback.JSProperties.Add("cpResultFile", oTask.ErrorCode)

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
        End Select

    End Sub

    Private Function ExecuteBackgroundAnalytic() As Integer
        Dim iTask As Integer = -1

        Try
            Dim DateInf As DateTime = roTypes.CreateDateTime(txtBeginDate.Date.Year, txtBeginDate.Date.Month, txtBeginDate.Date.Day, 0, 0, 0)
            Dim DateSup As DateTime = roTypes.CreateDateTime(txtEndDate.Date.Year, txtEndDate.Date.Month, txtEndDate.Date.Day, 23, 59, 59)

            If DateInf > DateSup Then
                Dim aux As DateTime = DateSup
                DateSup = DateInf
                DateInf = aux
            End If


            iTask = API.LiveTasksServiceMethods.CreateAnalyticTask(Me.Page, roLiveAnalyticType.Audit, 1, 1, WLHelperWeb.CurrentPassportID, DateInf, DateSup, String.Empty, String.Empty, String.Empty, String.Empty, False, False, True, String.Empty, String.Empty)


            Dim lstAuditParameterNames As New List(Of String)
            Dim lstAuditParameterValues As New List(Of String)

            lstAuditParameterNames.Add("{LiveTaskParameters}")
            lstAuditParameterValues.Add(iTask.ToString)

            lstAuditParameterNames.Add("{LiveTaskConfiguration}")
            lstAuditParameterValues.Add($"{DateInf.ToShortDateString()} a {DateSup.ToShortDateString()}")

            Dim oAuditState As New AuditState.wscAuditState(WLHelperWeb.CurrentPassportID)
            WebServiceHelper.SetState(oAuditState)
            roLiveSupport.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tLiveTask, "Audit", lstAuditParameterNames, lstAuditParameterValues, oAuditState)


        Catch ex As Exception
            'do nothing
        End Try
        Return iTask
    End Function

#Region "Audit message translator"

    Protected Function GetMessage(ByVal e As ASPxGridViewColumnDataEventArgs, ByVal tbParameters As DataTable) As String

        Dim MessageKey As String = "Message." & System.Enum.GetName(GetType(Robotics.VTBase.Audit.ObjectType), CType(e.GetListSourceFieldValue("ElementID"), Robotics.VTBase.Audit.ObjectType)) & "." &
                                     System.Enum.GetName(GetType(Robotics.VTBase.Audit.Action), CType(e.GetListSourceFieldValue("ActionID"), Robotics.VTBase.Audit.Action))

        Dim strMessage As String = Me.oLanguage.Translate(MessageKey, "Audit")

        Const TOKEN_PATTERN = "\$@[a-zA-Z.]+@"

        ' Reemplazo tokens por valores pasados por parámetro
        strMessage = Regex.Replace(strMessage, TOKEN_PATTERN, New MatchEvaluator(Function(match As Match) CustomMatchEvaulator(match, tbParameters)))

        Return strMessage

    End Function

    Private Function CustomMatchEvaulator(ByVal m As Match, ByVal dtParameters As DataTable) As String
        Return Me.ResolveParameter("{" & m.Value.Substring(2, m.Value.Length - 3) & "}", "", dtParameters)
    End Function

    Private Function ResolveParameter(ByVal strName As String, ByVal strParentName As String, ByVal dtParameters As DataTable) As String

        Dim strRet As New StringBuilder

        If dtParameters IsNot Nothing AndAlso dtParameters.Rows.Count > 0 Then

            Dim strParamName As String
            'Dim strChildParamName As String
            Dim strChilds As New StringBuilder
            Dim oParams As DataRow() = dtParameters.Select("ParamName = '" & strName & "' AND ParamParent = '" & strParentName & "'", "Priority")
            Dim oChildParams As DataRow()
            For Each oParam As DataRow In oParams

                'If strRet <> "" Then strRet &= vbCrLf

                strParamName = oParam("ParamName")
                If Not strParamName.StartsWith("{") Then
                    If strParamName.StartsWith("?") Then
                        strRet.Append(oLanguage.Translate("Parameters." & strParamName.Substring(1), "Audit"))
                    Else
                        strRet.Append(strParamName)
                    End If
                    strRet.Append(":")
                End If
                strRet.Append(roTypes.Any2String(oParam("ParamValue")))

                oChildParams = dtParameters.Select("ParamParent = '" & strParamName & "'", "Priority")
                If oChildParams.Length > 0 Then
                    If strRet.ToString() <> "" Then strRet.Append($"{vbCrLf}{strRet.ToString()}{vbCrLf}")
                    strChilds.Clear()
                    Dim oChild As DataRow
                    Dim strValue As String = ""
                    For nChild As Integer = 0 To oChildParams.Length - 1
                        oChild = oChildParams(nChild)
                        ''If nChild > 0 Then strChilds &= " -> "
                        strValue = Me.ResolveParameter(oChild("ParamName"), oChild("ParamParent"), dtParameters)
                        If nChild > 0 AndAlso Not strValue.StartsWith(vbCrLf) Then
                            strChilds.Append(" -> ")
                        End If
                        strChilds.Append(strValue)
                    Next
                    While strChilds.ToString().EndsWith(" -> ")
                        strChilds = New StringBuilder(strChilds.ToString().Substring(0, strChilds.Length - 4))
                    End While
                    If strChilds.ToString() <> "" Then
                        'strRet &= vbCrLf & strChilds
                        strRet.Append(strChilds)
                    End If
                End If

            Next

            If strParentName = "" AndAlso strRet.ToString().StartsWith(vbCrLf) Then
                strRet = New StringBuilder(strRet.ToString().Substring(2))
            End If

        End If

        Return strRet.ToString()

    End Function

    Private Sub PerformActionCallback_Load(sender As Object, e As EventArgs) Handles PerformActionCallback.Load

    End Sub

#End Region

End Class