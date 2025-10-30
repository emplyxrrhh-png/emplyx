Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Public Class frmProductiveUnitSelector
    Inherits UserControlBase

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        cmbAvailablePUnits.ClientInstanceName = Me.ClientID & "_cmbAvailablePUnits"

        If Not IsPostBack Then
            cmbAvailablePUnits.Items.Clear()
            cmbAvailablePUnits.SelectedIndex = 0
            cmbAvailablePUnits.DropDownRows = 4
            AddPunitItems()
        End If

    End Sub

    Private Sub AddPunitItems()
        Dim dTbl As DataTable = API.ShiftServiceMethods.GetShiftsPlanification(Me.Page, -1)

        Dim pLst = AISchedulingServiceMethods.GetProductiveUnits(Me.Page)

        For Each oPunit As roProductiveUnit In pLst
            cmbAvailablePUnits.Items.Add(New DevExpress.Web.ListEditItem(oPunit.Name, oPunit.ID))
        Next
    End Sub

End Class