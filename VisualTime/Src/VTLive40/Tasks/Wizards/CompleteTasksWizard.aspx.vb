Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.Web.Base

Partial Class Forms_CompleteTasksWizard
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"

#Region "Declarations"

    Private intActivePage As Integer

    Private oPermission As Permission
    Private oCurrentPermission As Permission

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("CompleteT_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("CompleteT_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("CompleteT_iCurrentTask") = value
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
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Server.ScriptTimeout = 1000

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission >= Permission.Write Then

            If Not Me.IsPostBack Then
                Me.intActivePage = 0
                'Inicializamos el selector de empleados
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeCompleteTasksWizard")
                HelperWeb.roSelector_Initialize("objContainerTreeV3_treeCompleteTasksWizardGrid")

                Me.ifTasksSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifTasksSelector.Disabled = True
            Else
                If Me.divStep0.Style("display") <> "none" Then Me.intActivePage = 0
                If Me.divStep1.Style("display") <> "none" Then Me.intActivePage = 1
                If Me.DivStep2.Style("display") <> "none" Then Me.intActivePage = 2
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub btNext_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btNext.Click

        If Me.CheckPage(Me.intActivePage) Then

            Dim intOldPage As Integer = Me.intActivePage
            Me.intActivePage += 1

            'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
            If Me.intActivePage = 1 Then Me.intActivePage += 1

            Me.PageChange(intOldPage, Me.intActivePage)

        End If

    End Sub

    Protected Sub btPrev_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btPrev.Click

        Dim intOldPage As Integer = Me.intActivePage
        Me.intActivePage -= 1

        'Ens saltem la primera pantalla del filtre del arbre, ja que el porta incorporat
        If Me.intActivePage = 1 Then Me.intActivePage -= 1

        Me.PageChange(intOldPage, Me.intActivePage)

    End Sub

    Protected Sub btResume_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btResume.Click
        CloseWizard()
    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", Me.CheckPage(Me.intActivePage))
            Case "PERFORM_ACTION"
                PerformActionCallback.JSProperties.Add("cpAction", "PERFORM_ACTION")
                iCurrentTask = Me.CompleteTasks()
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
                                If oTask.ErrorCode <> String.Empty Then
                                    ErrorDescription = oTask.ErrorCode
                                Else
                                    ErrorDescription = String.Empty
                                End If
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

#End Region

#Region "Methods"

    Private Function CheckPage(ByVal intPage As Integer) As Boolean

        Dim bolRet As Boolean = True
        Dim strMsg As String = ""

        Select Case intPage
            Case 1

                If strMsg <> "" Then bolRet = False

            Case 2

                ' Hemos de tener algún empleado seleccionado
                If Me.hdnTasksSelected.Value = "" Then
                    strMsg = Me.Language.Translate("CheckPage.Page2.NoTasksSelected", Me.DefaultScope)
                End If
                If strMsg <> "" Then bolRet = False
                Me.lblStep2Error.Text = strMsg

        End Select

        Return bolRet

    End Function

    Private Sub PageChange(ByVal intOldPage As Integer, ByVal intActivePage As Integer)

        Select Case intOldPage
            Case 1

            Case 2
                ' Desactivar el iframe del selector de grupos
                Me.ifTasksSelector.Attributes("src") = Me.ResolveUrl("~/Base/BlankPage.aspx")
                Me.ifTasksSelector.Disabled = True
        End Select

        Select Case intActivePage
            Case 2
                ' Inicializar selección del selector de empleados
                Dim strAux As String = "~/Base/WebUserControls/roTreeTaskContainer.aspx?" &
                                       "PrefixCookie=objContainerTreeV3_treeCompleteTasksWizard&AfterSelectFuncion=parent.GetSelectedTreeTask"
                Me.ifTasksSelector.Attributes("src") = Me.ResolveUrl(strAux)
                Me.ifTasksSelector.Disabled = False

        End Select

        Me.hdnActiveFrame.Value = intActivePage

        ' Hacer invisible página anterior
        Dim oPage As HtmlGenericControl = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intOldPage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "none"
        End If
        ' Hacer visible página actual
        oPage = HelperWeb.GetControl(Me.Page.Controls, "divStep" & intActivePage)
        If oPage IsNot Nothing Then
            oPage.Style("display") = "block"
        End If

        If intOldPage = 2 And intActivePage = 0 Then
            Me.btPrev.Visible = False '.Style("display") = "none"
            Me.btNext.Visible = False '.Style("display") = "none"
            Me.btEnd.Visible = False '.Style("display") = "none"
        Else
            Me.btPrev.Visible = IIf(intActivePage > 0, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) > 0, "block", "none")
            Me.btNext.Visible = IIf(intActivePage < 2, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) < Me.Frames.Count - 1, "block", "none")
            Me.btEnd.Visible = IIf(intActivePage = 2, True, False) '.Style("display") = IIf(Me.FramePos(oActiveFrame) = Me.Frames.Count - 1, "block", "none")
        End If

    End Sub

    Private Sub CloseWizard()
        If Not ErrorExists Then
            Me.MustRefresh = "1"
            Me.lblCompleteTaskWelcome1.Text = Me.Language.Translate("End.Ok.CompleteTaskWelcome1.Text", Me.DefaultScope)
            If ErrorDescription = String.Empty Then
                Me.lblCompleteTaskWelcome2.Text = Me.Language.Translate("End.Ok.CompleteTaskWelcome2.Text", Me.DefaultScope)
                Me.lblCompleteTaskWelcome3.Text = ErrorDescription
            Else
                Me.lblCompleteTaskWelcome2.Text = Me.Language.Translate("End.OkWarning.CompleteTaskWelcome2.Text", Me.DefaultScope)
                Me.lblCompleteTaskWelcome3.Text = ErrorDescription
            End If

            Me.lblCompleteTaskWelcome3.ForeColor = Drawing.Color.Orange
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        Else
            Me.lblCompleteTaskWelcome1.Text = Me.Language.Translate("End.Error.CompleteTaskWelcome1.Text", Me.DefaultScope)
            Me.lblCompleteTaskWelcome2.Text = Me.Language.Translate("End.Error.CompleteTaskWelcome2.Text", Me.DefaultScope)
            Me.lblCompleteTaskWelcome3.Text = ErrorDescription
            Me.lblCompleteTaskWelcome3.ForeColor = Drawing.Color.Red
            Me.btClose.Text = Me.Language.Keyword("Button.Close")
        End If

        ErrorDescription = Nothing
        ErrorExists = Nothing
        iCurrentTask = Nothing

        Me.PageChange(2, 0)
    End Sub

    Private Function CompleteTasks() As Integer

        Dim iTask As Integer = -1

        Dim strTasks As String = String.Empty
        Dim strProjects As String = String.Empty
        GetTasks(strTasks, strProjects, True)

        Me.lblCompleteTaskWelcome1.Text = Me.Language.Translate("End.CompleteTaskWelcome1.Text", Me.DefaultScope)

        iTask = API.LiveTasksServiceMethods.CompleteTasksAndProjectsBackground(Me, strTasks, strProjects, True)

        Return iTask
    End Function

    Private Sub GetTasks(ByRef strTasks As String, ByRef strProjects As String, ByVal InStringFormat As Boolean)
        Try
            Dim oTreeState As roTreeState = HelperWeb.roSelector_GetTreeState("objContainerTreeV3_treeCompleteTasksWizard")
            strTasks = roTypes.Any2String(oTreeState.Selected1)
            If InStringFormat Then
                If HttpUtility.UrlDecode(roTypes.Any2String(oTreeState.Selected2)) <> String.Empty Then
                    strProjects = "'" & HttpUtility.UrlDecode(roTypes.Any2String(oTreeState.Selected2)).Replace(",", "','") & "'"
                Else
                    strProjects = ""
                End If
            Else

                strProjects = HttpUtility.UrlDecode(roTypes.Any2String(oTreeState.Selected2))
            End If
        Catch ex As Exception
        End Try
    End Sub

#End Region

End Class