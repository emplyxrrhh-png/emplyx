Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class FieldsManager
    Inherits PageBase

#Region "Declarations"

    Private _Action_Edit As Integer = 0
    Private _Action_Remove As Integer = 1
    Private _Action_Accept As Integer = 2
    Private _Action_Cancel As Integer = 3

    Private _EmployeeUFNotUsed_EditClickIndex As Integer = 0
    Private _EmployeeUFNotUsed_RemoveClickIndex As Integer = 1
    Private _EmployeeUFNotUsed_selectCellIndex As Integer = 2
    Private _EmployeeUFNotUsed_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}

    Private _EmployeeUFUsed_EditClickIndex As Integer = 0
    Private _EmployeeUFUsed_RemoveClickIndex As Integer = 1
    Private _EmployeeUFUsed_selectCellIndex As Integer = 2
    Private _EmployeeUFUsed_ActionButtons() As String = {"imgEdit2", "imgRemove2", "imgEditAccept2", "imgEditCancel2"}

    Private _SystemEmployeeUFUsed_EditClickIndex As Integer = 0
    Private _SystemEmployeeUFUsed_RemoveClickIndex As Integer = 1
    Private _SystemEmployeeUFUsed_selectCellIndex As Integer = 2
    Private _SystemEmployeeUFUsed_ActionButtons() As String = {"imgEdit2", "imgRemove2", "imgEditAccept2", "imgEditCancel2"}

    Private _GroupUFNotUsed_EditClickIndex As Integer = 0
    Private _GroupUFNotUsed_RemoveClickIndex As Integer = 1
    Private _GroupUFNotUsed_selectCellIndex As Integer = 2
    Private _GroupUFNotUsed_ActionButtons() As String = {"imgEdit3", "imgRemove3", "imgEditAccept3", "imgEditCancel3"}

    Private _GroupUFUsed_EditClickIndex As Integer = 0
    Private _GroupUFUsed_RemoveClickIndex As Integer = 1

    Private _GroupUFUsed_selectCellIndex As Integer = 2
    Private _GroupUFUsed_ActionButtons() As String = {"imgEdit4", "imgRemove4", "imgEditAccept4", "imgEditCancel4"}

    Private _TaskUFNotUsed_EditClickIndex As Integer = 0
    Private _TaskUFNotUsed_selectCellIndex As Integer = 2
    Private _TaskUFNotUsed_ActionButtons() As String = {"imgEdit5", "imgRemove5", "imgEditAccept5", "imgEditCancel5"}

    Private _BusinessCenterUFNotUsed_EditClickIndex As Integer = 0
    Private _BusinessCenterUFNotUsed_selectCellIndex As Integer = 2
    Private _BusinessCenterUFNotUsed_ActionButtons() As String = {"imgEdit5", "imgRemove5", "imgEditAccept5", "imgEditCancel5"}

    Private oUserFieldsPermission As Permission = Permission.None
    Private oTaskFieldsPermission As Permission = Permission.None
    Private oBusinessCenterFieldsPermission As Permission = Permission.None

    Private _Remarks_EditClickIndex As Integer = 0
    Private _Remarks_RemoveClickIndex As Integer = 1
    Private _Remarks_selectCellIndex As Integer = 3
    Private _Remarks_ActionButtons() As String = {"imgEdit6", "imgRemove6", "imgEditAccept6", "imgEditCancel6"}

#End Region

#Region "Properties"

    Private Property EmployeeUserFieldsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("ConfigurationOptions_EmployeeUserFieldsData")
            If bolReload OrElse tb Is Nothing Then
                tb = API.UserFieldServiceMethods.GetUserFields(Me, Types.EmployeeField, "", True)
                Session("ConfigurationOptions_EmployeeUserFieldsData") = tb
            End If
            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("ConfigurationOptions_EmployeeUserFieldsData") = value
        End Set
    End Property

    Private ReadOnly Property EmployeeUserFieldsDataChanged() As Boolean
        Get
            Dim tb As DataTable = Session("ConfigurationOptions_EmployeeUserFieldsData")
            Return (tb Is Nothing)
        End Get
    End Property

    Private ReadOnly Property EmployeeFieldNameSelectedNotUsed() As String
        Get
            Dim strRet As String = ""
            Dim intRow As Integer = Val(Me.hdnEmployeeUserFieldsNotUsedSelectedRowIndex.Value) - 1
            If intRow >= 0 And intRow < Me.grdEmployeeUserFieldsNotUsed.Rows.Count Then
                strRet = CType(Me.grdEmployeeUserFieldsNotUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
            End If
            Return strRet
        End Get
    End Property

    Private ReadOnly Property EmployeeFieldNameSelectedUsed() As String
        Get
            Dim strRet As String = ""
            Dim intRow As Integer = Val(Me.hdnEmployeeUserFieldsUsedSelectedRowIndex.Value) - 1
            If intRow >= 0 And intRow < Me.grdEmployeeUserFieldsUsed.Rows.Count Then
                strRet = CType(Me.grdEmployeeUserFieldsUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
            End If
            Return strRet
        End Get
    End Property

    Private Property GroupUserFieldsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            Dim tb As DataTable = Session("ConfigurationOptions_GroupUserFieldsData")
            If bolReload OrElse tb Is Nothing Then
                tb = API.UserFieldServiceMethods.GetUserFields(Me, Types.GroupField, "", True)
                Session("ConfigurationOptions_GroupUserFieldsData") = tb
            End If
            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("ConfigurationOptions_GroupUserFieldsData") = value
        End Set
    End Property

    Private Property TaskFieldsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("ConfigurationOptions_TaskFieldsData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.UserFieldServiceMethods.GetTaskFields(Me, Types.TaskField)

                Session("ConfigurationOptions_TaskFieldsData") = tb

                ' Reestablecer ínidices selección 'grd '
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("ConfigurationOptions_TaskFieldsData") = value
        End Set
    End Property

    Private Property BusinessCenterFieldsData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get

            Dim tb As DataTable = Session("ConfigurationOptions_BusinessCenterFieldsData")

            If bolReload OrElse tb Is Nothing Then

                tb = API.UserFieldServiceMethods.GetBusinessCenterFields(Me, Types.TaskField)

                Session("ConfigurationOptions_BusinessCenterFieldsData") = tb

                ' Reestablecer ínidices selección 'grd '
                ' ...
            End If

            Return tb

        End Get
        Set(ByVal value As DataTable)
            Session("ConfigurationOptions_BusinessCenterFieldsData") = value
        End Set
    End Property

    Private ReadOnly Property GroupUserFieldsDataChanged() As Boolean
        Get
            Dim tb As DataTable = Session("ConfigurationOptions_GroupUserFieldsData")
            Return (tb Is Nothing)
        End Get
    End Property

    Private ReadOnly Property GroupFieldNameSelectedNotUsed() As String
        Get
            Dim strRet As String = ""
            Dim intRow As Integer = Val(Me.hdnGroupUserFieldsNotUsedSelectedRowIndex.Value) - 1
            If intRow >= 0 And intRow < Me.grdGroupUserFieldsNotUsed.Rows.Count Then
                strRet = CType(Me.grdGroupUserFieldsNotUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
            End If
            Return strRet
        End Get
    End Property

    Private ReadOnly Property TaskFieldNameSelectedNotUsed() As String
        Get
            Dim strRet As String = ""
            Dim intRow As Integer = Val(Me.hdnTaskFieldsNotUsedSelectedRowIndex.Value) - 1
            If intRow >= 0 And intRow < Me.grdTaskFieldsNotUsed.Rows.Count Then
                strRet = CType(Me.grdTaskFieldsNotUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
            End If
            Return strRet
        End Get
    End Property

    Private ReadOnly Property BusinessCenterFieldNameSelectedNotUsed() As String
        Get
            Dim strRet As String = ""
            Dim intRow As Integer = Val(Me.hdnBusinessCenterFieldsNotUsedSelectedRowIndex.Value) - 1
            If intRow >= 0 And intRow < Me.grdBusinessCenterFieldsNotUsed.Rows.Count Then
                strRet = CType(Me.grdBusinessCenterFieldsNotUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
            End If
            Return strRet
        End Get
    End Property

    Private ReadOnly Property GroupFieldNameSelectedUsed() As String
        Get
            Dim strRet As String = ""
            Dim intRow As Integer = Val(Me.hdnGroupUserFieldsUsedSelectedRowIndex.Value) - 1
            If intRow >= 0 And intRow < Me.grdGroupUserFieldsUsed.Rows.Count Then
                strRet = CType(Me.grdGroupUserFieldsUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
            End If
            Return strRet
        End Get
    End Property

#End Region

#Region "Events"

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Options", "~/Options/Scripts/Options.js")
        Me.InsertExtraJavascript("frmAddRoute", "~/Options/Scripts/frmAddRoute.js")
        Me.InsertExtraJavascript("webUserForms", "~/Base/Scripts/webUserForms.js")
        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.oTaskFieldsPermission = Me.GetFeaturePermission("Tasks.FieldsDefinition")
        Me.oUserFieldsPermission = Me.GetFeaturePermission("Employees.UserFields.Definition")
        Me.oBusinessCenterFieldsPermission = Me.GetFeaturePermission("BusinessCenters.Definition")

        If Me.oUserFieldsPermission = Permission.None Then
            WLHelperWeb.RedirectAccessDenied(False)
            Exit Sub
        End If

        If Me.oUserFieldsPermission <= Permission.Read Then
            Me.hdnModeEdit.Value = "0"
        Else
            Me.hdnModeEdit.Value = "1"
        End If

        Me.lblSaveChanges.InnerText = Me.Language.Translate("msgSaveChangesbar", DefaultScope)
        Me.lblSaveChangesBottom.InnerText = Me.lblSaveChanges.InnerText
        Me.lblUndoChanges.InnerText = Me.Language.Translate("msgUndoChangesbar", DefaultScope)
        Me.lblUndoChangesBottom.InnerText = Me.lblUndoChanges.InnerText

        Me.msgHasChanges.Value = Me.Language.Translate("msgHasChanges", DefaultScope)
        Me.msgHasErrors.Value = Me.Language.Translate("msgHasErrors", DefaultScope)

        'If HelperSession.GetFeatureIsInstalledFromApplication("Feature\ONE") Then
        'Me.rowTabButtons2.Style("display") = "none"
        'Me.tbAddEmployeeUserFieldNotUsed.Style("display") = "none !important"
        'Me.tbAddGroupUserFieldNotUsed.Style("display") = "none !important"
        'End If

        If Not Me.IsPostBack Then

            Me.EmployeeUserFieldsData = Nothing
            Me.GroupUserFieldsData = Nothing

            Me.UpdateData(, True)

            Me.SetPermissions()
        Else
            Me.UpdateData()
        End If

        Try

            If Request.Form("__EVENTTARGET") IsNot Nothing Then

                If Request.Form("__EVENTTARGET").EndsWith("btSave") Then
                    Me.btSave_Click(Me.btSave, Nothing)

                ElseIf Request.Form("__EVENTTARGET").EndsWith("btCancel") Then
                    Me.btCancel_Click(Me.btCancel, Nothing)

                ElseIf Request.Form("__EVENTTARGET").Contains("btRemoveEmployeeUserField") Then
                    Dim strFileName As String = ""
                    Dim intRow As Integer = Request.Form("__EVENTTARGET").Split("##")(2)
                    If intRow >= 0 And intRow < Me.grdEmployeeUserFieldsNotUsed.Rows.Count Then
                        strFileName = CType(Me.grdEmployeeUserFieldsNotUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
                        Me.DeleteUserField(Types.EmployeeField, False, strFileName)
                    End If

                ElseIf Request.Form("__EVENTTARGET").Contains("btRemoveGroupUserField") Then
                    Dim strFileName As String = ""
                    Dim intRow As Integer = Request.Form("__EVENTTARGET").Split("##")(2)
                    If intRow >= 0 And intRow < Me.grdGroupUserFieldsNotUsed.Rows.Count Then
                        strFileName = CType(Me.grdGroupUserFieldsNotUsed.Rows(intRow).Cells(3).Controls(3), Label).Text
                        Me.DeleteUserField(Types.GroupField, False, strFileName)
                    End If
                End If
            End If
        Catch ex As Exception
        End Try

    End Sub

    Protected Sub btSave_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btSave.Click

        Dim strKeyMsg As String = ""
        If Me.oUserFieldsPermission >= Permission.Write Then

            Dim bolSaved As Boolean = False

            If Me.oUserFieldsPermission >= Permission.Write Then
                bolSaved = API.UserFieldServiceMethods.SaveUserFields(Me, Me.EmployeeUserFieldsData, True)
            End If

            bolSaved = API.UserFieldServiceMethods.SaveUserFields(Me, Me.GroupUserFieldsData, True)

            If Me.oTaskFieldsPermission >= Permission.Write Then
                bolSaved = API.UserFieldServiceMethods.SaveTaskFields(Me, Me.TaskFieldsData, True)
            End If

            If Me.oBusinessCenterFieldsPermission >= Permission.Write Then
                bolSaved = API.UserFieldServiceMethods.SaveBusinessCenterFields(Me, Me.BusinessCenterFieldsData, True)
            End If

            If bolSaved Then
                Me.hdnChanged.Value = "0"
                Me.UpdateData(-1)
            Else
                Me.hdnChanged.Value = "1"
            End If
        Else
            WLHelperWeb.RedirectAccessDenied(False)
        End If

    End Sub

    Protected Sub btCancel_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btCancel.Click

        Me.hdnChanged.Value = "0"
        Me.UpdateData(-1, True)
    End Sub

    Protected Sub btRefresh_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btRefresh.Click

        Me.UpdateData()
        Me.hdnChanged.Value = "1"
    End Sub

    Protected Sub ibtUserFieldAsign_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtEmployeeUserFieldAsign.Click, ibtGroupUserFieldAsign.Click
        If sender Is ibtEmployeeUserFieldAsign Then
            Me.AsignUserFieldNotUsed(Types.EmployeeField)
        ElseIf sender Is ibtGroupUserFieldAsign Then
            Me.AsignUserFieldNotUsed(Types.GroupField)
        End If
    End Sub

    Protected Sub ibtUserFieldRemove_Click(ByVal sender As Object, ByVal e As System.Web.UI.ImageClickEventArgs) Handles ibtEmployeeUserFieldRemove.Click, ibtGroupUserFieldRemove.Click
        If sender Is ibtEmployeeUserFieldRemove Then
            Me.RemoveUserFieldUsed(Types.EmployeeField)
        ElseIf sender Is ibtGroupUserFieldRemove Then
            Me.RemoveUserFieldUsed(Types.GroupField)
        End If
    End Sub

    Protected Sub DeleteUserField(ByVal Type As Types, ByVal bolUsed As Boolean, Optional ByVal _FieldName As String = "")
        If _FieldName = "" Then
            Select Case Type
                Case Types.EmployeeField
                    If Not bolUsed Then
                        _FieldName = Me.EmployeeFieldNameSelectedNotUsed()
                    Else
                        _FieldName = Me.EmployeeFieldNameSelectedUsed()
                    End If
                Case Types.GroupField
                    If Not bolUsed Then
                        _FieldName = Me.GroupFieldNameSelectedNotUsed()
                    Else
                        _FieldName = Me.GroupFieldNameSelectedUsed()
                    End If

            End Select
        End If
        If _FieldName <> "" Then
            Dim oRows() As DataRow = Nothing
            Select Case Type
                Case Types.EmployeeField
                    oRows = Me.EmployeeUserFieldsData.Select("FieldName = '" & _FieldName & "'")
                Case Types.GroupField
                    oRows = Me.GroupUserFieldsData.Select("FieldName = '" & _FieldName & "'")
            End Select
            If oRows.Length = 1 Then
                oRows(0).Delete()
                Me.hdnChanged.Value = "1"
                Me.UpdateData()
            End If
        End If
    End Sub

    Protected Sub AsignUserFieldNotUsed(ByVal Type As Types)
        Dim strFieldName As String = ""
        Select Case Type
            Case Types.EmployeeField
                strFieldName = Me.EmployeeFieldNameSelectedNotUsed()
            Case Types.GroupField
                strFieldName = Me.GroupFieldNameSelectedNotUsed()
        End Select
        If strFieldName <> "" Then
            Dim oRows(-1) As DataRow
            Select Case Type
                Case Types.EmployeeField
                    oRows = Me.EmployeeUserFieldsData.Select("FieldName = '" & strFieldName & "'")
                Case Types.GroupField
                    oRows = Me.GroupUserFieldsData.Select("FieldName = '" & strFieldName & "'")
            End Select
            If oRows.Length = 1 Then

                If IsDBNull(oRows(0).Item("Category")) OrElse oRows(0).Item("Category") <> "Documentos" Then
                    oRows(0).Item("Used") = True
                    Me.UpdateData()
                    Me.hdnChanged.Value = "1"
                Else
                    Dim oMessageFrame As Object = HelperWeb.ShowMessage(Me, "No se puede agregar el campo", "Desde la versión 5.0, no se pueden utilizar Campos de la ficha de tipo Documento", , , , HelperWeb.MsgBoxIcons.AlertIcon)
                    If oMessageFrame IsNot Nothing Then
                        With oMessageFrame
                            .Title = "No se puede agregar el campo"
                        End With
                    End If
                End If
            End If
        End If
    End Sub

    Protected Sub RemoveUserFieldUsed(ByVal Type As Types)

        Dim tgUserField As Integer = -1
        Dim parameter As roAdvancedParameter = API.CommonServiceMethods.GetAdvancedParameter(Me.Page, "Timegate.Identification.CustomUserFieldId")
        If parameter IsNot Nothing AndAlso parameter.Value.Trim.Length > 0 Then
            Dim timegateConfiguration As TimegateConfiguration
            timegateConfiguration = roJSONHelper.DeserializeNewtonSoft(parameter.Value, GetType(TimegateConfiguration))
            If timegateConfiguration IsNot Nothing AndAlso timegateConfiguration.CustomUserFieldEnabled Then
                tgUserField = timegateConfiguration.UserFieldId
            End If
        End If


        Dim strFieldName As String = ""
        Select Case Type
            Case Types.EmployeeField
                strFieldName = Me.EmployeeFieldNameSelectedUsed()
            Case Types.GroupField
                strFieldName = Me.GroupFieldNameSelectedUsed()
        End Select
        If strFieldName <> "" Then
            Dim oRows(-1) As DataRow
            Select Case Type
                Case Types.EmployeeField
                    oRows = Me.EmployeeUserFieldsData.Select("FieldName = '" & strFieldName & "'")
                Case Types.GroupField
                    oRows = Me.GroupUserFieldsData.Select("FieldName = '" & strFieldName & "'")
            End Select
            If oRows.Length = 1 Then
                Dim bolRemove As Boolean = True
                If Type = Types.EmployeeField Then

                    If oRows(0).Item("Id") = tgUserField AndAlso tgUserField <> -1 Then
                        bolRemove = False
                        Dim oMessageFrame As Object = HelperWeb.ShowMessage(Me, Me.Language.Translate("RemoveUserFieldUsed.UsedInTimegate.Description", Me.DefaultScope), Me.Language.Translate("RemoveUserFieldUsed.UsedInProcess.Title", Me.DefaultScope), , , , HelperWeb.MsgBoxIcons.AlertIcon)
                        If oMessageFrame IsNot Nothing Then
                            With oMessageFrame
                                .Title = Me.Language.Translate("RemoveUserFieldUsed.UsedInProcess.Title", Me.DefaultScope)
                            End With
                        End If
                    ElseIf oRows(0).Item("UsedInProcess") Then
                        bolRemove = False
                        Dim oMessageFrame As Object = HelperWeb.ShowMessage(Me, Me.Language.Translate("RemoveUserFieldUsed.UsedInProcess.Description", Me.DefaultScope), Me.Language.Translate("RemoveUserFieldUsed.UsedInProcess.Title", Me.DefaultScope), , , , HelperWeb.MsgBoxIcons.AlertIcon)
                        If oMessageFrame IsNot Nothing Then
                            With oMessageFrame
                                .Title = Me.Language.Translate("RemoveUserFieldUsed.UsedInProcess.Title", Me.DefaultScope)
                            End With
                        End If
                    End If
                End If
                If bolRemove Then
                    oRows(0).Item("Used") = False
                    Me.UpdateData()
                    Me.hdnChanged.Value = "1"
                End If
            End If
        End If
    End Sub

#Region "grdEmployeeUserFieldsNotUsed"

    Protected Sub grdEmployeeUserFieldsNotUsed_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdEmployeeUserFieldsNotUsed.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                'Dim _EditClickButton As LinkButton = e.Row.Cells(Me._UFNotUsed_EditClickIndex).Controls(0)
                '_jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                _jsEdit = "EditUserFieldNotUsed('" & Types.EmployeeField & "', " & e.Row.RowIndex + 1 & ");"

                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._EmployeeUFNotUsed_RemoveClickIndex).Controls(0)
                '_jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")
                _jsRemove = "RemoveEmployeeUserField(" & e.Row.RowIndex & ");"

                If Me.oUserFieldsPermission < Permission.Admin Then
                    _jsRemove = ""
                End If

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    js = _jsEdit
                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._EmployeeUFNotUsed_selectCellIndex).FindControl(Me._EmployeeUFNotUsed_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._EmployeeUFNotUsed_selectCellIndex).FindControl(Me._EmployeeUFNotUsed_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Attributes("style") += "cursor:pointer;"
                _RemoveButton.Style("display") = IIf(Me.oUserFieldsPermission = Permission.Admin, "", "none")

                If Not IsDBNull(DirectCast(e.Row.DataItem, System.Data.DataRowView).Row("FieldType")) AndAlso DirectCast(e.Row.DataItem, System.Data.DataRowView).Row("FieldType") = FieldTypes.tDocument Then
                    _RemoveButton.Attributes("style") += "display:none"
                End If
            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdEmployeeUserFieldsNotUsed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdEmployeeUserFieldsNotUsed.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))

            Case "RemoveClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                Me.hdnEmployeeUserFieldsNotUsedSelectedRowIndex.Value = _rowIndex + 1

                Me.DeleteUserField(Types.EmployeeField, False)

        End Select

    End Sub

#End Region

#Region "grdEmployeeUserFieldsUsed"

    Protected Sub grdEmployeeUserFieldsUsed_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdEmployeeUserFieldsUsed.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                _jsEdit = "EditUserFieldUsed('" & Types.EmployeeField & "', " & e.Row.RowIndex + 1 & ");"

                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._EmployeeUFUsed_RemoveClickIndex).Controls(0)
                _jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")

                If Me.oUserFieldsPermission < Permission.Admin Then
                    _jsRemove = ""
                End If

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    js = _jsEdit
                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._EmployeeUFUsed_selectCellIndex).FindControl(Me._EmployeeUFUsed_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._EmployeeUFUsed_selectCellIndex).FindControl(Me._EmployeeUFUsed_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                ''_RemoveButton.Attributes("style") += "cursor:pointer;"
                _RemoveButton.Style("display") = "none" ''IIf(Me.oUserFieldsPermission = Permission.Admin, "", "none")

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdEmployeeUserFieldsUsed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdEmployeeUserFieldsUsed.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))

            Case "RemoveClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                Me.hdnEmployeeUserFieldsUsedSelectedRowIndex.Value = _rowIndex + 1

                DeleteUserField(Types.EmployeeField, True)

        End Select

    End Sub

#End Region

#Region "grdSystemEmployeeUserFields"

    Protected Sub grdSystemEmployeeUserFields_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdSystemEmployeeUserFields.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                _jsEdit = "EditSystemUserField('" & Types.EmployeeField & "', " & e.Row.RowIndex + 1 & ");"

                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._SystemEmployeeUFUsed_RemoveClickIndex).Controls(0)

                _jsRemove = ""

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    js = _jsEdit
                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._SystemEmployeeUFUsed_selectCellIndex).FindControl(Me._SystemEmployeeUFUsed_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._SystemEmployeeUFUsed_selectCellIndex).FindControl(Me._SystemEmployeeUFUsed_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Style("display") = "none"

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdSystemEmployeeUserFields_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdSystemEmployeeUserFields.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"
                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))
            Case "RemoveClick"
        End Select

    End Sub

#End Region

#Region "grdGroupUserFieldsNotUsed"

    Protected Sub grdGroupUserFieldsNotUsed_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdGroupUserFieldsNotUsed.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                'Dim _EditClickButton As LinkButton = e.Row.Cells(Me._UFNotUsed_EditClickIndex).Controls(0)
                '_jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                _jsEdit = "EditUserFieldNotUsed('" & Types.GroupField & "', " & e.Row.RowIndex + 1 & ");"

                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._GroupUFNotUsed_RemoveClickIndex).Controls(0)
                '_jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")
                _jsRemove = "RemoveGroupUserField(" & e.Row.RowIndex & ");"

                If Me.oUserFieldsPermission < Permission.Admin Then
                    _jsRemove = ""
                End If

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    js = _jsEdit
                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._GroupUFNotUsed_selectCellIndex).FindControl(Me._GroupUFNotUsed_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._GroupUFNotUsed_selectCellIndex).FindControl(Me._GroupUFNotUsed_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Attributes("style") += "cursor:pointer;"
                '_RemoveButton.Style("display") = IIf(Me.oUserFieldsPermission = Permission.Admin, "", "none")

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdGroupUserFieldsNotUsed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdGroupUserFieldsNotUsed.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))

            Case "RemoveClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                Me.hdnGroupUserFieldsNotUsedSelectedRowIndex.Value = _rowIndex + 1

                Me.DeleteUserField(Types.GroupField, False)

        End Select

    End Sub

#End Region

#Region "grdGroupUserFieldsUsed"

    Protected Sub grdGroupUserFieldsUsed_RowDataBound(ByVal sender As Object, ByVal e As GridViewRowEventArgs) Handles grdGroupUserFieldsUsed.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim _jsRemove As String
                Dim js As String

                _jsEdit = "EditUserFieldUsed('" & Types.GroupField & "', " & e.Row.RowIndex + 1 & ");"

                Dim _RemoveClickButton As LinkButton = e.Row.Cells(Me._GroupUFUsed_RemoveClickIndex).Controls(0)
                _jsRemove = ClientScript.GetPostBackClientHyperlink(_RemoveClickButton, "")

                If Me.oUserFieldsPermission < Permission.Admin Then
                    _jsRemove = ""
                End If

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    js = _jsEdit
                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._GroupUFUsed_selectCellIndex).FindControl(Me._GroupUFUsed_ActionButtons(Me._Action_Edit))
                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._GroupUFUsed_selectCellIndex).FindControl(Me._GroupUFUsed_ActionButtons(Me._Action_Remove))
                _RemoveButton.Attributes("onclick") = _jsRemove
                _RemoveButton.Attributes("style") += "cursor:pointer;"
                '_RemoveButton.Style("display") = IIf(Me.oUserFieldsPermission = Permission.Admin, "", "none")
                _RemoveButton.Style("display") = "none"

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdGroupUserFieldsUsed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdGroupUserFieldsUsed.RowCommand

        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))

            Case "RemoveClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())

                Me.hdnGroupUserFieldsUsedSelectedRowIndex.Value = _rowIndex + 1

                DeleteUserField(Types.GroupField, True)

        End Select

    End Sub

#End Region

#Region "grdTaskFields"

    Protected Sub grdTaskFieldsNotUsed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdTaskFieldsNotUsed.RowCommand
        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))

        End Select

    End Sub

    Protected Sub grdTaskFieldsNotUsed_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdTaskFieldsNotUsed.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim js As String

                'Dim _EditClickButton As LinkButton = e.Row.Cells(Me._UFNotUsed_EditClickIndex).Controls(0)
                '_jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                _jsEdit = "EditTaskFieldNotUsed('" & Types.TaskField & "', " & e.Row.RowIndex + 1 & ");"

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    If Me.oTaskFieldsPermission < Permission.Write Then
                        _jsEdit = ""
                    End If

                    js = _jsEdit

                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._TaskUFNotUsed_selectCellIndex).FindControl(Me._TaskUFNotUsed_ActionButtons(Me._Action_Edit))

                If Me.oTaskFieldsPermission < Permission.Write Then
                    _jsEdit = ""
                End If

                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

                'Dim _RemoveButton As HtmlImage = e.Row.Cells(Me._TaskUFNotUsed_selectCellIndex).FindControl(Me._TaskUFNotUsed_ActionButtons(Me._Action_Remove))
                '_RemoveButton.Attributes("onclick") = _jsRemove
                '_RemoveButton.Attributes("style") += "cursor:pointer;"
                '_RemoveButton.Style("display") = IIf(Me.oUserFieldsPermission = Permission.Admin, "", "none")

            Case DataControlRowType.Footer

        End Select

    End Sub

#End Region

#Region "grdBusinessCenterFields"

    Protected Sub grdBusinessCenterFieldsNotUsed_RowCommand(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewCommandEventArgs) Handles grdBusinessCenterFieldsNotUsed.RowCommand
        Dim _gridView As GridView = sender

        Select Case e.CommandName
            Case "EditClick"

                ' Get the row index
                Dim _rowIndex As Integer = Integer.Parse(e.CommandArgument.ToString())
                ' Parse the event argument (added in RowDataBound) to get the selected column index
                Dim _columnIndex As Integer = -1 ''Integer.Parse(Request.Form("__EVENTARGUMENT"))

        End Select

    End Sub

    Protected Sub grdBusinessCenterFieldsNotUsed_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdBusinessCenterFieldsNotUsed.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                ' Asignar eventos

                Dim _jsEdit As String
                Dim js As String

                'Dim _EditClickButton As LinkButton = e.Row.Cells(Me._UFNotUsed_EditClickIndex).Controls(0)
                '_jsEdit = ClientScript.GetPostBackClientHyperlink(_EditClickButton, "")
                _jsEdit = "EditBusinessCenterFieldNotUsed('" & Types.TaskField & "', " & e.Row.RowIndex + 1 & ");"

                For columnIndex As Integer = 0 To e.Row.Cells.Count - 1
                    ''e.Row.Cells(columnIndex).Attributes.Add("oncontextmenu", strContextMenuScript)
                    'js = _jsCommand.Insert(_jsCommand.Length - 2, columnIndex.ToString)
                    If Me.oBusinessCenterFieldsPermission < Permission.Write Then
                        _jsEdit = ""
                    End If

                    js = _jsEdit

                    e.Row.Cells(columnIndex).Attributes("ondblclick") = js
                    e.Row.Cells(columnIndex).Attributes("style") += "cursor:pointer;"
                Next

                Dim _EditButton As HtmlImage = e.Row.Cells(Me._BusinessCenterUFNotUsed_selectCellIndex).FindControl(Me._BusinessCenterUFNotUsed_ActionButtons(Me._Action_Edit))

                If Me.oBusinessCenterFieldsPermission < Permission.Write Then
                    _jsEdit = ""
                End If

                _EditButton.Attributes("onclick") = _jsEdit
                _EditButton.Attributes("style") += "cursor:pointer;"

            Case DataControlRowType.Footer

        End Select

    End Sub

#End Region

#End Region

#Region "Methods"

    Private Sub UpdateData(Optional ByVal intContextDataType As Integer = 0, Optional ByVal bReload As Boolean = False)
        Dim tbEmployeeUserFields As DataTable = Me.EmployeeUserFieldsData(intContextDataType = -1)

        Dim bolRowAdded As Boolean = False

        Dim dvEmployeeUserFieldsNotUsed As New DataView(tbEmployeeUserFields)
        With dvEmployeeUserFieldsNotUsed
            .RowFilter = "Used = 0 AND isSystem=0"
            .Sort = "FieldName ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With

        With Me.grdEmployeeUserFieldsNotUsed
            .DataSourceID = ""
            .DataSource = dvEmployeeUserFieldsNotUsed
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdEmployeeUserFieldsNotUsed)

        bolRowAdded = False

        Dim dvSystemEmployeeUserFields As New DataView(tbEmployeeUserFields)
        With dvSystemEmployeeUserFields
            .RowFilter = "isSystem = 1"
            .Sort = "FieldName ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With

        With Me.grdSystemEmployeeUserFields
            .DataSourceID = ""
            .DataSource = dvSystemEmployeeUserFields
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdSystemEmployeeUserFields)

        bolRowAdded = False

        Dim dvEmployeeUserFieldsUsed As New DataView(tbEmployeeUserFields)
        With dvEmployeeUserFieldsUsed
            .RowFilter = "Used = 1 AND isSystem=0"
            .Sort = "FieldName ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With
        With Me.grdEmployeeUserFieldsUsed
            .DataSourceID = ""
            .DataSource = dvEmployeeUserFieldsUsed
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdEmployeeUserFieldsUsed)

        Dim tbGroupUserFields As DataTable = Me.GroupUserFieldsData(intContextDataType = -1)

        bolRowAdded = False

        Dim dvGroupUserFieldsNotUsed As New DataView(tbGroupUserFields)
        With dvGroupUserFieldsNotUsed
            .RowFilter = "Used = 0"
            .Sort = "FieldName ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With
        With Me.grdGroupUserFieldsNotUsed
            .DataSourceID = ""
            .DataSource = dvGroupUserFieldsNotUsed
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdGroupUserFieldsNotUsed)

        bolRowAdded = False

        Dim dvGroupUserFieldsUsed As New DataView(tbGroupUserFields)
        With dvGroupUserFieldsUsed
            .RowFilter = "Used = 1 AND NOT FieldName LIKE '_translate_%'"
            .Sort = "FieldName ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With
        With Me.grdGroupUserFieldsUsed
            .DataSourceID = ""
            .DataSource = dvGroupUserFieldsUsed
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdGroupUserFieldsUsed)

        'TAREAS
        Dim tbTaskFields As DataTable = Me.TaskFieldsData(bReload)

        bolRowAdded = False

        Dim dvTaskFieldsNotUsed As New DataView(tbTaskFields)
        With dvTaskFieldsNotUsed
            .Sort = "ID ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With
        With Me.grdTaskFieldsNotUsed
            .DataSourceID = ""
            .DataSource = dvTaskFieldsNotUsed
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdTaskFieldsNotUsed)

        'Centros de coste
        Dim tbBusinessCenterFields As DataTable = Me.BusinessCenterFieldsData(bReload)

        bolRowAdded = False

        Dim dvBusinessCenterFieldsNotUsed As New DataView(tbBusinessCenterFields)
        With dvBusinessCenterFieldsNotUsed
            .Sort = "ID ASC"
            If .Count = 0 Then
                .AddNew()
                bolRowAdded = True
            End If
        End With
        With Me.grdBusinessCenterFieldsNotUsed
            .DataSourceID = ""
            .DataSource = dvBusinessCenterFieldsNotUsed
            .DataBind()
            If bolRowAdded Then .Rows(0).Visible = False
        End With
        HelperWeb.EmptyGridFix(Me.grdBusinessCenterFieldsNotUsed)

    End Sub

    Public Function FieldTypeName(ByVal _Type As Object) As String
        If IsDBNull(_Type) Then Return ""
        Return Me.Language.Translate("FieldType." & System.Enum.GetName(GetType(FieldTypes), _Type), "UserField")
    End Function

    Public Function FieldActionName(ByVal _Type As Object) As String
        If IsDBNull(_Type) Then Return ""
        Return Me.Language.Translate("ActionTypes." & System.Enum.GetName(GetType(ActionTypes), _Type), "TaskField")
    End Function

    Public Function FieldUnique(ByVal isUnique As Object) As Boolean
        Return roTypes.Any2Boolean(isUnique)
    End Function

    Public Function AccessLevelName(ByVal _Level As Object) As String

        If Not IsDBNull(_Level) Then
            Return Me.Language.Translate("AccessLevel." & System.Enum.GetName(GetType(AccessLevels), _Level), "UserField")
        Else
            Return Me.Language.Translate("AccessLevel." & System.Enum.GetName(GetType(AccessLevels), AccessLevels.aMedium), "UserField")
        End If
    End Function

    Private Sub SetPermissions()
        If Me.oUserFieldsPermission > Permission.None Then
            Me.ibtEmployeeUserFieldAsign.Enabled = (Me.oUserFieldsPermission = Permission.Admin)
            Me.ibtEmployeeUserFieldRemove.Enabled = (Me.oUserFieldsPermission = Permission.Admin)
            If Not Me.ibtEmployeeUserFieldAsign.Enabled Then
                Me.ibtEmployeeUserFieldAsign.ImageUrl = "~/Options/Images/UserFields Asign 32 Disabled.gif"
                Me.ibtEmployeeUserFieldAsign.Style("cursor") = "default"
            End If
            If Not Me.ibtEmployeeUserFieldRemove.Enabled Then
                Me.ibtEmployeeUserFieldRemove.ImageUrl = "~/Options/Images/UserFields Remove 32 Disabled.gif"
                Me.ibtEmployeeUserFieldRemove.Style("cursor") = "default"
            End If

            Me.tbAddEmployeeUserFieldNotUsed.Visible = (Me.oUserFieldsPermission = Permission.Admin)
        Else
            Me.TABBUTTON_EmployeeUserFieldsOptions.Style("display") = "none"
            Me.tbEmployeeUserFieldsOptions.Visible = False
        End If

        If Me.oTaskFieldsPermission >= Permission.Read Then
            Me.TABBUTTON_TaskFieldsOptions.Style("display") = ""
            Me.tbTaskFieldsOptions.Visible = True
        Else
            Me.TABBUTTON_TaskFieldsOptions.Style("display") = "none"
            Me.tbTaskFieldsOptions.Visible = False
        End If

        If Me.oBusinessCenterFieldsPermission >= Permission.Read Then
            Me.TABBUTTON_BusinessCenterFieldsOptions.Style("display") = ""
            Me.tbBusinessCenterFieldsOptions.Visible = True
        Else
            Me.TABBUTTON_BusinessCenterFieldsOptions.Style("display") = "none"
            Me.tbBusinessCenterFieldsOptions.Visible = False
        End If

        'If Me.bolMultiCompanyLicense And Me.oPermission > Permission.None Then
        If Me.oUserFieldsPermission > Permission.None Then
            Me.ibtGroupUserFieldAsign.Enabled = (Me.oUserFieldsPermission = Permission.Admin)
            Me.ibtGroupUserFieldRemove.Enabled = (Me.oUserFieldsPermission = Permission.Admin)
            If Not Me.ibtGroupUserFieldAsign.Enabled Then
                Me.ibtGroupUserFieldAsign.ImageUrl = "~/Options/Images/UserFields Asign 32 Disabled.gif"
                Me.ibtGroupUserFieldAsign.Style("cursor") = "default"
            End If
            If Not Me.ibtGroupUserFieldRemove.Enabled Then
                Me.ibtGroupUserFieldRemove.ImageUrl = "~/Options/Images/UserFields Remove 32 Disabled.gif"
                Me.ibtGroupUserFieldRemove.Style("cursor") = "default"
            End If

            Me.tbAddGroupUserFieldNotUsed.Visible = True
        Else
            Me.TABBUTTON_GroupUserFieldsOptions.Style("display") = "none"
            Me.tbGroupUserFieldsOptions.Visible = False
        End If

        ' Desactivar los botons de grabación sólo si no tiene acceso a modificar la configuración de presencia y no tiene permisos de administrar la definición de la ficha
        If Me.oUserFieldsPermission < Permission.Write Then
            Me.btSave.Visible = False '.Style("display") = "none"
            Me.btCancel.Visible = False '.Style("display") = "none"
        End If

    End Sub

#End Region

End Class