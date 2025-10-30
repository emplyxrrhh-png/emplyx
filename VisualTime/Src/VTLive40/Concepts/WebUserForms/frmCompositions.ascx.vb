Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Concept
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class WebUserForms_frmCompositions
    Inherits UserControlBase

    Public Sub loadCombosComposition(ByVal oIDConcept As Integer)
        Try

            Dim dTbl As DataTable = CausesServiceMethods.GetCausesShortList(Nothing)
            Dim dTblShifts As DataTable = API.ShiftServiceMethods.GetShifts(Nothing)

            Dim oConcept As New roConcept

            'Dim optChkCondition As WebUserControls_roOptionPanelClient = Me.optChkCondition

            'Dim cmbCause As DevExpress.Web.ASPxComboBox = Me.cmbCause
            'Dim cmbCauseAdd As DevExpress.Web.ASPxComboBox = Me.cmbCauseAdd
            'Dim cmbCauseType As DevExpress.Web.ASPxComboBox = Me.cmbCauseType
            'Dim cmbCompare As DevExpress.Web.ASPxComboBox = Me.cmbCompare
            'Dim cmbTypeValue As DevExpress.Web.ASPxComboBox = Me.cmbTypeValue
            'Dim cmbCauseUField As DevExpress.Web.ASPxComboBox = Me.cmbCauseUField

            cmbCause.Items.Clear()
            cmbValueDailyCause.Items.Clear()
            cmbDayContainsCause.Items.Clear()
            cmbShift.Items.Clear()
            cmbCauseAdd.Items.Clear()
            cmbCauseType.Items.Clear()
            cmbDayCause.Items.Clear()

            For Each dRow As DataRow In dTblShifts.Rows
                cmbShift.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID") & "_" & dRow("ShiftType")))
            Next

            For Each dRow As DataRow In dTbl.Rows
                cmbCauseType.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                cmbCauseAdd.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))

                If Robotics.VTBase.roTypes.Any2Boolean(dRow("DayType")) OrElse Robotics.VTBase.roTypes.Any2Boolean(dRow("CustomType")) Then
                    cmbValueCustomCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                    If Robotics.VTBase.roTypes.Any2Boolean(dRow("DayType")) Then cmbValueDailyCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                Else
                    cmbDayContainsCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                    cmbDayCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                    cmbCause.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))

                End If
            Next

            Dim strEnums() As String = System.Enum.GetNames(GetType(ConditionCompareType))

            cmbCompare.Items.Clear()
            For n As Integer = 0 To strEnums.Length - 1
                cmbCompare.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(strEnums(n), DefaultScope), n.ToString))
            Next

            cmbTypeValue.Items.Clear()
            cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.DirectValue).ToString, DefaultScope), CInt(ValueType.DirectValue)))
            cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.UserField).ToString, DefaultScope), CInt(ValueType.UserField)))
            cmbTypeValue.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(ValueType), ValueType.IDCause).ToString, DefaultScope), CInt(ValueType.IDCause)))

            'Combo camps fitxa
            cmbCauseUField.Items.Clear()
            Dim tbUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(4) AND Used=1", False)
            For Each oRow As DataRow In tbUserFields.Select("", "FieldName")
                cmbCauseUField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
            Next

            cmbFactorUserField.Items.Clear()
            Dim tbFactorUserFields As DataTable = API.UserFieldServiceMethods.GetUserFields(Me.Page, Types.EmployeeField, "FieldType IN(1,3) AND Used=1", False)
            For Each oRow As DataRow In tbFactorUserFields.Select("", "FieldName")
                cmbFactorUserField.Items.Add(New DevExpress.Web.ListEditItem(oRow("FieldName"), oRow("FieldName")))
            Next

            cmbDaysType.Items.Clear()
            cmbDaysType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), TypeDayPlanned.AllDays).ToString, DefaultScope), CInt(TypeDayPlanned.AllDays)))
            cmbDaysType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), TypeDayPlanned.Laboral).ToString, DefaultScope), CInt(TypeDayPlanned.Laboral)))
            cmbDaysType.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), TypeDayPlanned.NonLaboral).ToString, DefaultScope), CInt(TypeDayPlanned.NonLaboral)))

            cmbDaysTypeCause.Items.Clear()
            cmbDaysTypeCause.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), TypeDayPlanned.AllDays).ToString, DefaultScope), CInt(TypeDayPlanned.AllDays)))
            cmbDaysTypeCause.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), TypeDayPlanned.Laboral).ToString, DefaultScope), CInt(TypeDayPlanned.Laboral)))
            cmbDaysTypeCause.Items.Add(New DevExpress.Web.ListEditItem(Me.Language.Translate(System.Enum.GetName(GetType(TypeDayPlanned), TypeDayPlanned.NonLaboral).ToString, DefaultScope), CInt(TypeDayPlanned.NonLaboral)))

            If conceptCompositionData.Contains("IDConceptComposition") Then
                conceptCompositionData("IDConceptComposition") = -1
            Else
                conceptCompositionData.Add("IDConceptComposition", -1)
            End If

            If conceptCompositionData.Contains("IDType") Then
                conceptCompositionData("IDType") = 0
            Else
                conceptCompositionData.Add("IDType", 0)
            End If

            If conceptCompositionData.Contains("IsNew") Then
                conceptCompositionData("IsNew") = False
            Else
                conceptCompositionData.Add("IsNew", False)
            End If
        Catch ex As Exception
            Response.Write(ex.StackTrace)
        End Try
    End Sub

    Protected Sub WebUserForms_frmCompositions_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            ibtNewCauseOK.Src = Me.ResolveUrl("~/Base/Images/Grid/save.png")
            ibtNewCauseCancel.Src = Me.ResolveUrl("~/Base/Images/Grid/cancel.png")
            imgAddListValue.Src = Me.ResolveUrl("~/Base/Images/Grid/add.png")
            imgRemoveListValue.Src = Me.ResolveUrl("~/Base/Images/Grid/remove.png")
        End If
    End Sub

End Class