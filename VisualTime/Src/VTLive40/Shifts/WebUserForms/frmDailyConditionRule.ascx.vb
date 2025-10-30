Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmDailyConditionRule
    Inherits UserControlBase

    Public Property Instance() As String
        Get
            If ViewState("roDailyRuleConditionInstance") Is Nothing Then
                Return ""
            Else
                Return ViewState("roDailyRuleConditionInstance")
            End If
        End Get
        Set(ByVal value As String)
            ViewState("roDailyRuleConditionInstance") = value
        End Set
    End Property

    Private Sub WebUserForms_frmDailyConditionRule_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ibtNewCauseOK.Src = Me.ResolveUrl("~/Base/Images/Grid/save.png")
            ibtNewCauseOK.Attributes("onclick") = "AddCauseOK(" & Me.Instance & ",0,'divNewCauseConditions_" & Me.Instance & "'); return false;"
            ibtNewCauseCancel.Src = Me.ResolveUrl("~/Base/Images/Grid/cancel.png")
            ibtNewCauseCancel.Attributes("onclick") = "ShowWindow('divNewCauseConditions_" & Me.Instance & "',false);"
            imgAddListValue.Src = Me.ResolveUrl("~/Base/Images/Grid/add.png")
            imgAddListValue.Attributes("onclick") = "ShowWindow('divNewCauseConditions_" & Me.Instance & "',true);"
            imgRemoveListValue.Src = Me.ResolveUrl("~/Base/Images/Grid/remove.png")
            imgRemoveListValue.Attributes("onclick") = "RemoveListValue(" & Me.Instance & ", 0, 'divNewCauseConditions_" & Me.Instance & "',true);"

            cmbCauseAdd.ClientInstanceName = "cmbCauseAddClient_" & Me.Instance

            ibtNewTimezoneOK.Src = Me.ResolveUrl("~/Base/Images/Grid/save.png")
            ibtNewTimezoneOK.Attributes("onclick") = "AddTimeZoneOK(" & Me.Instance & ",0,'divNewTimeZone_" & Me.Instance & "'); return false;"
            ibtNewTimezoneCancel.Src = Me.ResolveUrl("~/Base/Images/Grid/cancel.png")
            ibtNewTimezoneCancel.Attributes("onclick") = "ShowWindow('divNewTimeZone_" & Me.Instance & "',false);"
            imgAddTimeZoneValue.Src = Me.ResolveUrl("~/Base/Images/Grid/add.png")
            imgAddTimeZoneValue.Attributes("onclick") = "ShowWindow('divNewTimeZone_" & Me.Instance & "',true);"
            imgRemoveTimeZoneValue.Src = Me.ResolveUrl("~/Base/Images/Grid/remove.png")
            imgRemoveTimeZoneValue.Attributes("onclick") = "RemoveTimeZoneValue(" & Me.Instance & ", 0, 'divNewTimeZone_" & Me.Instance & "',true);"

            cmbTimeZoneAdd.ClientInstanceName = "cmbTimeZoneAddClient_" & Me.Instance

            cmbCompare.ClientInstanceName = "cmbCompareClient_" & Me.Instance
            cmbCompare.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showCompareValue(" & Me.Instance & ",s);}"

            cmbTypeValue.ClientInstanceName = "cmbTypeValueClient_" & Me.Instance
            cmbTypeValue.ClientSideEvents.SelectedIndexChanged = "function(s,e){ showTypeValue(" & Me.Instance & ",s);}"

            txtValueType.ClientInstanceName = "txtValueTypeClient_" & Me.Instance
            txtValueTypeTo.ClientInstanceName = "txtValueTypeToClient_" & Me.Instance
            cmbCauseUField.ClientInstanceName = "cmbCauseUFieldClient_" & Me.Instance

            ibtValueCauseOK.Src = Me.ResolveUrl("~/Base/Images/Grid/save.png")
            ibtValueCauseOK.Attributes("onclick") = "AddCauseOK(" & Me.Instance & ",1,'divValueCauseConditions_" & Me.Instance & "'); return false;"
            ibtValueCauseCancel.Src = Me.ResolveUrl("~/Base/Images/Grid/cancel.png")
            ibtValueCauseCancel.Attributes("onclick") = "ShowWindow('divValueCauseConditions_" & Me.Instance & "',false);"
            imgAddValueCauseValue.Src = Me.ResolveUrl("~/Base/Images/Grid/add.png")
            imgAddValueCauseValue.Attributes("onclick") = "ShowWindow('divValueCauseConditions_" & Me.Instance & "',true);"
            imgRemoveValueCauseValue.Src = Me.ResolveUrl("~/Base/Images/Grid/remove.png")
            imgRemoveValueCauseValue.Attributes("onclick") = "RemoveListValue(" & Me.Instance & ", 1, 'divValueCauseConditions_" & Me.Instance & "',true);"

            cmbAddValueCause.ClientInstanceName = "cmbAddValueCauseClient_" & Me.Instance

            ibtNewTimezoneValueOK.Src = Me.ResolveUrl("~/Base/Images/Grid/save.png")
            ibtNewTimezoneValueOK.Attributes("onclick") = "AddTimeZoneOK(" & Me.Instance & ",1,'divNewTimeZoneValue_" & Me.Instance & "'); return false;"
            ibtNewTimezoneValueCancel.Src = Me.ResolveUrl("~/Base/Images/Grid/cancel.png")
            ibtNewTimezoneValueCancel.Attributes("onclick") = "ShowWindow('divNewTimeZoneValue_" & Me.Instance & "',false);"
            imgAddTimeZoneValueValue.Src = Me.ResolveUrl("~/Base/Images/Grid/add.png")
            imgAddTimeZoneValueValue.Attributes("onclick") = "ShowWindow('divNewTimeZoneValue_" & Me.Instance & "',true);"
            imgRemoveTimeZoneValueValue.Src = Me.ResolveUrl("~/Base/Images/Grid/remove.png")
            imgRemoveTimeZoneValueValue.Attributes("onclick") = "RemoveTimeZoneValue(" & Me.Instance & ", 1, 'divNewTimeZoneValue_" & Me.Instance & "',true);"

            cmbTimeZoneValueAdd.ClientInstanceName = "cmbTimeZoneValueAddClient_" & Me.Instance

            loadCombosComposition()
        End If
    End Sub

    Public Sub loadCombosComposition()
        Try

            Dim dTbl As DataTable = CausesServiceMethods.GetCausesShortList(Me.Page)
            Dim dTblTimeZones As DataTable = API.ShiftServiceMethods.GetTimeZones(Me.Page)

            cmbCauseAdd.Items.Clear()

            For Each dRow As DataRow In dTbl.Rows
                cmbCauseAdd.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                cmbAddValueCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
            Next
            cmbCauseAdd.SelectedIndex = 0
            cmbAddValueCause.SelectedIndex = 0

            cmbTimeZoneAdd.Items.Clear()

            cmbTimeZoneAdd.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TimeZones.Any", Me.DefaultScope), -1))
            cmbTimeZoneValueAdd.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate("TimeZones.Any", Me.DefaultScope), -1))
            For Each dRow As DataRow In dTblTimeZones.Rows
                cmbTimeZoneAdd.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                cmbTimeZoneValueAdd.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
            Next
            cmbTimeZoneAdd.SelectedIndex = 0
            cmbTimeZoneValueAdd.SelectedIndex = 0

            Dim strEnums() As String = System.Enum.GetNames(GetType(DailyConditionCompareType))

            cmbCompare.Items.Clear()
            For n As Integer = 0 To strEnums.Length - 1
                cmbCompare.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(strEnums(n), DefaultScope), n.ToString))
            Next

            cmbTypeValue.Items.Clear()
            cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(Robotics.Base.DTOs.ValueType), Robotics.Base.DTOs.ValueType.DirectValue).ToString, DefaultScope), CInt(Robotics.Base.DTOs.ValueType.DirectValue)))
            cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(Robotics.Base.DTOs.ValueType), Robotics.Base.DTOs.ValueType.UserField).ToString, DefaultScope), CInt(Robotics.Base.DTOs.ValueType.UserField)))
            cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(Robotics.Base.DTOs.ValueType), Robotics.Base.DTOs.ValueType.IDCause).ToString, DefaultScope), CInt(Robotics.Base.DTOs.ValueType.IDCause)))

            cmbCauseUField.Items.Clear()
            Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(4) AND Used=1", False)
            For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
                cmbCauseUField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
            Next
            cmbCauseUField.SelectedIndex = 0
        Catch ex As Exception
            Response.Write(ex.StackTrace)
        End Try
    End Sub

End Class