Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class EmployeeRemoveData
    Inherits PageBase

    Private Const FeatureAlias As String = "Employees.Contract"

#Region "Declarations"

    Private intIDEmployee As Integer

    Private oPermission As Permission
    Private bCanEdit As Boolean = True

#End Region

#Region "Properties"

    Private Property iCurrentTask As Integer
        Get
            Dim val As Object = HttpContext.Current.Session("ERD_iCurrentTask")
            If val IsNot Nothing Then
                Return roTypes.Any2Integer(val)
            Else
                HttpContext.Current.Session("ERD_iCurrentTask") = -1
                Return -1
            End If
        End Get
        Set(value As Integer)
            HttpContext.Current.Session("ERD_iCurrentTask") = value
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

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Dim EmployeeID As String = Request.Params("EmployeeID")
        If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
            Me.intIDEmployee = CInt(EmployeeID)
        End If

        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        Me.bCanEdit = oPermission > Permission.Read
        If Me.oPermission > Permission.None Then

            Me.LoadingPanel.Style("left") = "0px"
            Me.LoadingPanel.Style("top") = "0px"

            If Me.oPermission < Permission.Admin Then
                Me.btEnd.Visible = False
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(True)
        End If

    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                PerformActionCallback.JSProperties.Add("cpResult", True)
                PerformActionCallback.JSProperties.Add("cpErrorMessageKey", ErrorDescription)
            Case "PERFORM_ACTION"

                If Me.RemoveEmployeeData() Then
                    PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                    PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                Else
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    PerformActionCallback.JSProperties.Add("cpActionResult", "ERROR")
                    PerformActionCallback.JSProperties.Add("cpErrorMessageKey", ErrorDescription)
                End If
            Case "CHECKPROGRESS"
                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                PerformActionCallback.JSProperties.Add("cpActionResult", "")
        End Select

    End Sub

#End Region

    Private Function RemoveEmployeeData() As Boolean
        Dim bRes As Boolean = True

        bRes = API.EmployeeServiceMethods.RemoveEmployeeData(Me.Page, Me.intIDEmployee, Me.optRemoveEmployeePhoto.Checked, Me.optRemoveBiometricData.Checked, Me.optRemovePunchPhoto.Checked)

        ErrorExists = bRes
        ErrorDescription = API.EmployeeServiceMethods.LastErrorText

        Return bRes
    End Function

End Class