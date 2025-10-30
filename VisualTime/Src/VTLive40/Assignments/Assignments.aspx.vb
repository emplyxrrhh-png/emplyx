Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Assignments_Assignments
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

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init

        Me.oPermission = Me.GetFeaturePermission("Assignments.Definition")

        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
        Me.InsertExtraJavascript("GenericData", "~/Base/Scripts/GenericData.js")
        Me.InsertExtraJavascript("Assignments", "~/Assignments/Scripts/Assignments.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        ' Comprueba la licencia
        If HelperSession.GetFeatureIsInstalledFromApplication("Feature\HRScheduling") = False Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        roTreesAssignments.TreeCaption = Me.Language.Translate("TreeCaptionAssignments", Me.DefaultScope)

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        If Me.oPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        ElseIf Me.oPermission = Permission.Read Then
            DisableControls()
            txtDescription.Enabled = False
            cmbCostField.Enabled = False
            Me.dxColorPicker.ReadOnly = True
        End If

        Dim dTbl As Generic.List(Of roAssignment) = AssignmentServiceMethods.GetAssignments(Me.Page, "Name", False)
        If dTbl.Count = 0 Then
            Me.noRegs.Value = "1"
        Else
            Me.noRegs.Value = ""
        End If

        If Not Me.IsPostBack And Not Me.IsCallback Then
            Me.LoadAssignmentsData()
        End If

    End Sub

    Protected Sub ASPxCallbackPanelContenido_Callback(ByVal sender As Object, ByVal e As DevExpress.Web.CallbackEventArgsBase) Handles ASPxCallbackPanelContenido.Callback

        Dim strParameter As String = roTypes.Any2String(e.Parameter)
        strParameter = Server.UrlDecode(strParameter)

        Dim oParameters As New ObjectCallbackRequest()
        oParameters = roJSONHelper.Deserialize(strParameter, oParameters.GetType())

        Dim responseMessage = String.Empty
        Dim bRet As Boolean

        Select Case oParameters.Action

            Case "GETASSIGNMENT"
                bRet = LoadAssignment(oParameters.ID, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "GETASSIGNMENT")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                End If
                ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", False)
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)

            Case "SAVEASSIGNMENT"
                bRet = SaveAssignment(oParameters.ID, oParameters.Name, responseMessage)
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "SAVEASSIGNMENT")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", IIf(bRet, "OK", "NOK"))
                ASPxCallbackPanelContenido.JSProperties.Add("cpNameRO", txtName.Text)
                If Not bRet Then
                    ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", responseMessage)
                Else
                    ASPxCallbackPanelContenido.JSProperties("cpAction") = "GETASSIGNMENT"
                    ASPxCallbackPanelContenido.JSProperties.Add("cpIsNew", True)
                End If

            Case Else
                ASPxCallbackPanelContenido.JSProperties.Add("cpAction", "UNKNOW")
                ASPxCallbackPanelContenido.JSProperties.Add("cpResult", "NOK")
                ASPxCallbackPanelContenido.JSProperties.Add("cpMessage", "Unknow call")
        End Select

    End Sub

    Private Function LoadAssignment(ByRef IdAssignment As Integer, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission > Permission.None Then

                Dim oCurrentAssignment As roAssignment

                If IdAssignment = -1 Then
                    oCurrentAssignment = New roAssignment()
                    oCurrentAssignment.ID = -1
                Else
                    oCurrentAssignment = AssignmentServiceMethods.GetAssignment(Me.Page, IdAssignment, True)
                End If

                txtName.Text = oCurrentAssignment.Name
                txtDescription.Value = oCurrentAssignment.Description
                txtShortName.Value = oCurrentAssignment.ShortName
                txtExport.Value = oCurrentAssignment.Export

                Dim it As DevExpress.Web.ListEditItem = cmbCostField.Items.FindByValue(oCurrentAssignment.CostField)
                If it Is Nothing Then
                    cmbCostField.SelectedIndex = 0
                Else
                    cmbCostField.SelectedItem = it
                End If

                Me.dxColorPicker.Color = System.Drawing.ColorTranslator.FromWin32(oCurrentAssignment.Color)

                bRet = True
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Function SaveAssignment(ByRef IdAssignment As Integer, ByRef AssignmentName As String, ByRef ErrorInfo As String) As Boolean
        Dim bRet As Boolean = False

        ErrorInfo = String.Empty

        Try

            If Me.oPermission >= Permission.Write Then

                Dim oCurrentAssignment As roAssignment = AssignmentServiceMethods.GetAssignment(Me.Page, IdAssignment, False)
                If Not oCurrentAssignment Is Nothing Then

                    oCurrentAssignment.ID = IdAssignment
                    oCurrentAssignment.Name = txtName.Text
                    oCurrentAssignment.Description = txtDescription.Value
                    oCurrentAssignment.ShortName = txtShortName.Value
                    oCurrentAssignment.Export = txtExport.Value

                    If Not cmbCostField.SelectedItem Is Nothing Then
                        oCurrentAssignment.CostField = cmbCostField.SelectedItem.Value
                    End If
                    oCurrentAssignment.Color = Drawing.ColorTranslator.ToWin32(Me.dxColorPicker.Color)

                    bRet = AssignmentServiceMethods.SaveAssignment(Me.Page, oCurrentAssignment, True)
                    If bRet Then
                        Dim treePath As String = "/source/" & oCurrentAssignment.ID
                        HelperWeb.roSelector_SetSelection(oCurrentAssignment.ID.ToString, treePath, "ctl00_contentMainBody_roTreesAssignments")
                    Else
                        ErrorInfo = AssignmentServiceMethods.LastErrorText
                    End If
                Else
                    ErrorInfo = AssignmentServiceMethods.LastErrorText
                End If
            Else
                ErrorInfo = Me.Language.Translate("CheckPermission.Denied.Message", Me.DefaultScope)
            End If
        Catch ex As Exception
            ErrorInfo = Me.Language.Translate("Message.UnknowError", Me.DefaultScope)
        End Try

        Return bRet

    End Function

    Private Sub LoadAssignmentsData()
        Me.cmbCostField.Items.Clear()
        Me.cmbCostField.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("AnyField", Me.DefaultScope), ""))
        Dim dTblUF As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "Used=1", False)
        For Each dRow As DataRow In dTblUF.Rows
            If Not dRow("FieldType") Is DBNull.Value Then
                'Si es tipo numerico o decimal, mostrar
                If dRow("FieldType") = 1 Or dRow("FieldType") = 3 Then
                    Me.cmbCostField.Items.Add(New DevExpress.Web.ListEditItem(dRow("FieldName"), "USR_" & dRow("FieldName")))
                End If
            End If
        Next
    End Sub

End Class