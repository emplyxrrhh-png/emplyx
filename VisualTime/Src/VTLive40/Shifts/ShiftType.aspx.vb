Imports Robotics.Base.DTOs
Imports Robotics.Web.Base

Partial Class ShiftType
    Inherits PageBase

#Region "Events"

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        If Not Me.HasFeaturePermission("Shifts.Definition", Permission.Admin) Then
            WLHelperWeb.RedirectAccessDenied(True)
            Exit Sub
        End If

        With Me.optGroup
            .addOPanel(Me.optNewShift_Delay)
            .addOPanel(Me.optNewShift_Flex)
            .addOPanel(Me.optNewShift_PartRigid)
            .addOPanel(Me.optNewShift_PartSemiFlex)
            .addOPanel(Me.optNewShift_Rigid)
            .addOPanel(Me.optNewShift_SemiFlex)
        End With

    End Sub

    Protected Sub btOKNewShift_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btOKNewShift.Click

        If optNewShift_Delay.Checked Then
            Me.MustRefresh = "ShiftType;Vacations"
        ElseIf optNewShift_Flex.Checked Then
            Me.MustRefresh = "ShiftType;Flex"
        ElseIf optNewShift_PartRigid.Checked Then
            Me.MustRefresh = "ShiftType;PartRigid"
        ElseIf optNewShift_PartSemiFlex.Checked Then
            Me.MustRefresh = "ShiftType;PartSemiFlex"
        ElseIf optNewShift_Rigid.Checked Then
            Me.MustRefresh = "ShiftType;Rigid"
        ElseIf optNewShift_SemiFlex.Checked Then
            Me.MustRefresh = "ShiftType;SemiFlex"
        End If
        Me.CanClose = True

    End Sub

    Protected Sub btCancelNewShift_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btCancelNewShift.Click

        Me.MustRefresh = "ShiftType;"
        Me.CanClose = True

    End Sub

#End Region

End Class