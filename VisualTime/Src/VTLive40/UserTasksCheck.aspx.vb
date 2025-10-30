Imports System.Threading
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.UserTask
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class UserTasksCheck
    Inherits PageBase
    ''Inherits PageBase
    ''Implements ICallbackEventHandler

#Region "Declarations"

    Private bolAsyncCall As Boolean ' Indica si la página se ha llamado a través de 'GetCallbackEventReference' desde el cliente.
    Private oPermission As Permission

#End Region

#Region "PropertieS"

    Private Property CheckUserTasksThread() As Thread
        Get
            Return Session("UserTasksCheck_Thread")
        End Get
        Set(ByVal value As Thread)
            Session("UserTasksCheck_Thread") = value
        End Set
    End Property

    Private Property CheckUserTasksIAsyncResult() As IAsyncResult
        Get
            Return Session("UserTasksCheck_IAsyncResult")
        End Get
        Set(ByVal value As IAsyncResult)
            Session("UserTasksCheck_IAsyncResult") = value
        End Set
    End Property

    Private Property CheckUserTasksResult() As String
        Get
            Return Session("UserTasksCheck_Result")
        End Get
        Set(ByVal value As String)
            Session("UserTasksCheck_Result") = value
        End Set
    End Property

    Private Property UserTasksCount() As Integer
        Get
            Dim intRet As Integer = 0
            If Session("UserTasksCheck_UserTasksCount") IsNot Nothing Then
                intRet = Session("UserTasksCheck_UserTasksCount")
            End If
            Return intRet
        End Get
        Set(ByVal value As Integer)
            Session("UserTasksCheck_UserTasksCount") = value
            'Me.hdnServerStatus.Value = value
        End Set
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        oPermission = GetFeaturePermission("Administration.Alerts")
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If WLHelperWeb.CurrentPassport(True) Is Nothing OrElse (HttpContext.Current.Session("roMultiCompanyId") = String.Empty) Then
            Me.Controls.Clear()
            Response.Clear()
            Response.ContentType = "text/plain"
            Response.Write("KO")
            Return
        End If

        Select Case Request("action")
            Case "reloadAlerts" ' Devuleve el número de tareas de usuario
                Me.Controls.Clear()
                Response.Clear()
                Response.ContentType = "text/plain"

                If WLHelperWeb.CurrentPassport IsNot Nothing Then
                    Dim oEmployeeDocAlerts = WLHelperWeb.EmployeeDocumentationAlerts(True)
                    Dim oCompanyDocAlerts = WLHelperWeb.CompanyDocumentationAlerts(True)

                    Dim oUserAlerts = WLHelperWeb.UserAlerts(True)
                    Dim oSystemAlerts = WLHelperWeb.SystemAlerts(True)

                    Response.Write("OK#" & WLHelperWeb.LastAlertsLoadDate.ToString("HH:mm:ss"))
                Else
                    Response.Write("OK#" & roTypes.CreateDateTime().ToString("HH:mm:ss"))
                End If



            Case "getUserTasksCheck" ' Devuleve el número de tareas de usuario

                Me.Controls.Clear()

                Response.Clear()
                Response.ContentType = "text/plain"

                Dim msgReturn As String = "UserTasks=0"

                Try
                    Dim oEmployeeDocAlerts = WLHelperWeb.EmployeeDocumentationAlerts(, True)
                    Dim oCompanyDocAlerts = WLHelperWeb.CompanyDocumentationAlerts(, True)

                    msgReturn = "UserTasks=" & (WLHelperWeb.UserAlerts.Rows.Count + WLHelperWeb.SystemAlerts.Rows.Count +
                    If(oEmployeeDocAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.WorkForecastDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.MandatoryDocuments.Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.AbsenteeismDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.DocumentsValidation.Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(oEmployeeDocAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0) +
                    If(oCompanyDocAlerts.DocumentsValidation.Count > 0, 1, 0) +
                    If(oCompanyDocAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument > 0).Count > 0, 1, 0) +
                    If(oCompanyDocAlerts.AccessAuthorizationDocuments.ToList.FindAll(Function(x) x.IDDocument = 0).Count > 0, 1, 0)) & "*" & oPermission

                    msgReturn &= "#" & WLHelperWeb.LastAlertsLoadDate.ToString("HH:mm:ss")
                Catch ex As Exception
                    msgReturn = "UserTasks=0*0#00:00:00"
                End Try

                Response.Write(msgReturn)

            Case "checkEmployeesLicense"

                Me.Controls.Clear()

                Response.Clear()
                Response.ContentType = "text/plain"

            Case "checkSession"

                Me.Controls.Clear()

                Response.Clear()
                Response.ContentType = "text/plain"

                Response.Write(Me.CheckSession())
            Case "removeCookies"
                Me.Controls.Clear()
                Response.Write(Me.removeControlCookies())
            Case Else

        End Select

    End Sub

#End Region

#Region "Methods"

    ''' <summary>
    ''' Verifica que la conexión actual sea correcta y que no se hayan sobrepasado el máximo permitido.
    ''' </summary>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Private Function CheckSession() As String

        Dim strRet As String = "TRUE"

        If Robotics.VTBase.roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("CheckLicenseLimits")) <> Robotics.VTBase.roTypes.Any2Boolean(API.PortalServiceMethods.CheckLicenseLimits(Nothing, DateTime.Now, WLHelperWeb.ServerLicense)) Then
            Dim newVal As String = HelperSession.AdvancedParametersCache("RESETCACHE")
            WLHelperWeb.MainMenu = Nothing
        End If
        ' Not AuthHelper.GetPassportKeyValidated(WLHelperWeb.CurrentPassportID, WLHelperWeb.SecurityToken) OrElse
        If (WLHelperWeb.CurrentPassport() Is Nothing _
            AndAlso roWsUserManagement.SessionObject().States.SecurityState.Result <> SecurityResultEnum.NoError) Then

            Dim initErrorText As String = roWsUserManagement.SessionObject().States.SecurityState.ErrorText

            'If WLHelperWeb.CurrentPassport(True) IsNot Nothing Then API.SecurityServiceMethods.SignOut(Me, WLHelperWeb.CurrentPassport(True).ID)

            HelperWeb.EraseCookie("SessionExpired")
            HelperWeb.EraseCookie("UpdateSessionError")
            HelperWeb.EraseCookie("AlreadyLoggedinInOtherLocation")
            HelperWeb.EraseCookie("SessionInvalidatedByPermissions")

            strRet = "FALSE" &
                     "TitleKey=CheckSession.Error.Title&" +
                     "DescriptionText=" & initErrorText & "&" +
                     "Option1TextKey=CheckSession.Error.Option1Text&" +
                     "Option1DescriptionKey=CheckSession.Error.Option1Description&" +
                     "Option1OnClickScript=HideMsgBoxForm(); ButtonClick(document.getElementById('btSignOut')); return false;&" +
                     "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
        Else
            WLHelperWeb.RemoveUserSessionData()
        End If

        Return strRet

    End Function

    Private Function removeControlCookies() As String

        Dim strRet As String = "TRUE"

        HelperWeb.EraseCookie("SessionExpired")

        HelperWeb.EraseCookie("UpdateSessionError")

        HelperWeb.EraseCookie("AlreadyLoggedinInOtherLocation")
        HelperWeb.EraseCookie("SessionInvalidatedByPermissions")

        Return strRet

    End Function

#Region "Async methods"

    Private Sub CheckUserTasksAsyncThread()

        Dim intUserTasks As Integer = 0

        If API.SecurityServiceMethods.GetPassportTicketBySessionID(Me, WLHelperWeb.CurrentPassportID, WLHelperWeb.CurrentPassportAuthMethod) IsNot Nothing _
            AndAlso roWsUserManagement.SessionObject().States.SecurityState.Result = SecurityResultEnum.NoError Then ' Sólo obtiene el número de tareas de usuario si hay un passport activo

            Dim tbUserTasks As DataTable = API.UserTaskServiceMethods.GetUserTasks(Me, TaskType.UserTaskRepair, TaskCompletedState.All)
            If tbUserTasks IsNot Nothing Then
                intUserTasks = tbUserTasks.Rows.Count
            End If

            Me.UserTasksCount = intUserTasks '(Me.UserTasksCount + 10) Mod 30
        Else
            Me.UserTasksCount = -1
        End If

        Me.CheckUserTasksResult = Me.UserTasksCount.ToString

    End Sub

#End Region

#End Region

End Class