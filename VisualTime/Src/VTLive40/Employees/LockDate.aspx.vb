Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class LockDate
    Inherits PageBase

#Region "Declarations"

    Dim EmployeeID As String
    Dim FirstDate As Date

#End Region

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        InsertExtraJavascript("Summary", "~/Employees/Scripts/LockDate.js")

        EmployeeID = Request.Params("EmployeeID")
        Dim lockDate As Date = Request.Params("lockDate")
        Dim lockDateType As Boolean = Request.Params("lockDateType")

        If Not Me.IsPostBack Then

            FirstDate = API.ConnectorServiceMethods.GetFirstDate(Me.Page)

            If Year(FirstDate) = 1900 Then
                rbLDG.Text = rbLDG.Text + " (" + Me.Language.Translate("LockDate.GlobalDateNotConfigured", DefaultScope) + ")"
            Else
                rbLDG.Text = rbLDG.Text + " (" + Me.Language.Translate("LockDate.ActualValue", DefaultScope) + " " + FirstDate.ToShortDateString.ToString + ")"
            End If

            ' Obtenemos el código de empleado actual

            If lockDateType = False Then
                rbLDG.Checked = True
                txtLockDateSpecific.Date = Nothing
            Else
                rbLDS.Checked = True
                txtLockDateSpecific.Date = lockDate.Date
            End If

        End If

    End Sub

    Protected Sub PerformActionCallback_Callback(ByVal source As Object, ByVal e As DevExpress.Web.CallbackEventArgs) Handles PerformActionCallback.Callback
        e.Result = String.Empty

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Select Case strParameter.Trim.ToUpperInvariant
            Case "VALIDATE"
                PerformActionCallback.JSProperties.Add("cpAction", "VALIDATE")
                If Me.txtLockDateSpecific.Value Is Nothing AndAlso Me.rbLDS.Checked Then
                    PerformActionCallback.JSProperties.Add("cpResult", False)
                    PerformActionCallback.JSProperties.Add("cpErrorMessageKey", Me.Language.Translate("LockDate.Requiered", DefaultScope))
                Else
                    PerformActionCallback.JSProperties.Add("cpResult", True)
                End If

            Case "PERFORM_ACTION"
                If Me.SaveLockDate() Then
                    PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                    PerformActionCallback.JSProperties.Add("cpActionResult", "OK")
                Else
                    PerformActionCallback.JSProperties.Add("cpAction", "ERROR")
                    PerformActionCallback.JSProperties.Add("cpActionResult", "ERROR")
                End If
            Case "CHECKPROGRESS"
                PerformActionCallback.JSProperties.Add("cpAction", "CHECKPROGRESS")
                PerformActionCallback.JSProperties.Add("cpActionResult", "")
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Function SaveLockDate() As Boolean
        Dim result As Boolean
        If rbLDG.Checked = True Then
            result = API.EmployeeServiceMethods.SaveEmployeeLockDate(Me.Page, EmployeeID, FirstDate, False, True)
        Else
            If Year(txtLockDateSpecific.Date) <> 1 Then
                result = API.EmployeeServiceMethods.SaveEmployeeLockDate(Me.Page, EmployeeID, txtLockDateSpecific.Date, True, True)
            Else
                result = False
            End If

        End If

        Return result
    End Function

    Private Sub SetPermissions()

    End Sub

    Private Sub btnSaveAndEdit_Click(sender As Object, e As EventArgs) Handles btnSaveAndEdit.Click
        Me.MustRefresh = "1"
        Me.CanClose = True
    End Sub

#End Region

End Class