Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class TaskAlert
    Inherits PageBase

    Private Const FeatureAlias As String = "Tasks.Definition"

#Region "Declarations"

    Private oPermission As Permission = Permission.None
    Private intAlertID As Integer
    Private intidemployee As Integer

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)
        Me.InsertExtraJavascript("OpenWindow", "~/Base/Scripts/OpenWindow.js", , True)
        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js", , True)

        Me.InsertExtraJavascript("roOptionPanelClient", "~/Base/Scripts/roOptionPanelClient.js")
        Me.InsertExtraJavascript("roOptPanelClientGroup", "~/Base/Scripts/roOptPanelClientGroup.js")
        Me.InsertExtraJavascript("roTabContainerClient", "~/Base/Scripts/roTabContainerClient.js")
        Me.InsertExtraJavascript("roTaskAlertCriteria", "~/Base/Scripts/roUserFieldCriteria.js")

    End Sub

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes()
        Me.InsertExtraCssIncludes("~/Base/ext-3.4.0/resources/css/ext-all.css")

        ' Si el passport actual no tiene permisso de lectura, redirigimos a página de acceso denegado
        Me.oPermission = Me.GetFeaturePermission(FeatureAlias)
        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        If Not Me.IsPostBack Then
            If (Request("alertid") <> -1) Then
                intAlertID = roTypes.Any2Integer(Request("alertid"))
                Me.hdnIDAlert.Value = intAlertID
                Me.hdnIDEmployee.Value = roTypes.Any2Integer(Request("idemployee"))

                Me.txtComment.Value = roTypes.Any2String(Request("comment"))
                Me.txtEmployeeName.Value = roTypes.Any2String(Request("employeename"))
                Me.txtDate.Value = roTypes.Any2String(Request("datetime"))
                Me.chkReaded.Checked = roTypes.Any2Boolean(Request("readed"))

            End If

        End If

        Me.UpdateData()

        If Not Me.IsPostBack Then
            Me.txtComment.Attributes("disabled") = "disabled"
            Me.txtEmployeeName.Attributes("disabled") = "disabled"
            Me.txtDate.Attributes("disabled") = "disabled"
            Me.SetPermissions()
        End If

        AddHandler Me.MessageFrame1.OptionOnClick, AddressOf OnMessageClick

    End Sub

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click

        If Me.oPermission >= Permission.Write Then
            Dim bolSaved = False

            Dim oTaskFields As New Generic.List(Of Object)

            Dim oEmpField As TaskAlertStructField
            Dim oEmp As New Generic.List(Of TaskAlertStructField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "ID"
            oEmpField.value = Me.hdnIDAlert.Value
            oEmp.Add(oEmpField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "EmployeeName"
            oEmpField.value = Me.txtEmployeeName.Value
            oEmp.Add(oEmpField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "IDEmployee"
            oEmpField.value = Me.hdnIDEmployee.Value
            oEmp.Add(oEmpField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "DateTime"
            oEmpField.value = Me.txtDate.Value
            oEmp.Add(oEmpField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "Comment"
            oEmpField.value = Me.txtComment.Value
            oEmp.Add(oEmpField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "Readed"
            oEmpField.value = roTypes.Any2Boolean(Me.chkReaded.Checked)
            oEmp.Add(oEmpField)

            oEmpField = New TaskAlertStructField
            oEmpField.attname = "IsReaded"
            Dim strReaded As String = ""
            Select Case roTypes.Any2Boolean(Me.chkReaded.Checked)
                Case True
                    strReaded = Me.Language.Translate("aReaded", Me.DefaultScope)
                Case False
                    strReaded = Me.Language.Translate("aNotReaded", Me.DefaultScope)
            End Select
            oEmpField.value = strReaded
            oEmp.Add(oEmpField)

            oTaskFields.Add(oEmp)

            Dim selectedFields As Integer = 0
            Dim strJSON As String = "{rows : [ "
            For Each oObj As Object In oTaskFields
                selectedFields = selectedFields + 1
                strJSON &= " {fields:"
                Dim oEmpFld As Generic.List(Of TaskAlertStructField)
                oEmpFld = CType(oObj, Generic.List(Of TaskAlertStructField))
                strJSON &= roJSONHelper.Serialize(oEmpFld) & "} ,"
            Next
            strJSON = strJSON.Substring(0, Len(strJSON) - 2) & "]}"

            If selectedFields > 0 Then
                hdnParams_PageBase.Value = strJSON
            Else
                hdnParams_PageBase.Value = ""
            End If

            Me.CanClose = True
            Me.MustRefresh = "2"
            ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeScript", "Close()", True)
        End If

    End Sub

    Protected Sub btCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btCancel.Click
        Me.CanClose = True
        Me.MustRefresh = "0"
        Me.hdnParams_PageBase.Value = ""
        ScriptManager.RegisterStartupScript(Me, Me.GetType(), "closeScript", "Close()", True)
    End Sub

    Protected Sub OnMessageClick(ByVal strButtonKey As String)

        Select Case strButtonKey
            Case "DeleteCategory.Answer.Yes"
            Case "DeleteListValue.Answer.Yes"
        End Select

    End Sub

#End Region

#Region "Methods"

    Private Sub UpdateData()

    End Sub

    Private Sub SetPermissions()

        If Me.oPermission < Permission.Write Then

            Me.DisableControls()

            Me.btAccept.Style("display") = "none"
            Me.btCancel.Text = Me.Language.Keyword("Button.Close")

        End If

    End Sub

#End Region

End Class

Public Class TaskAlertStructField
    Public attname As String
    Public value As String

End Class