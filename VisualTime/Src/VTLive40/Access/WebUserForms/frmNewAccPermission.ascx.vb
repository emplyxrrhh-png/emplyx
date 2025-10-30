Imports Robotics.Web.Base

Partial Class frmNewAccPermission
    Inherits UserControlBase

    Protected Sub frmNewAccPermission_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            loadFormNewAccPermission()
        End If
    End Sub

    Private Sub loadFormNewAccPermission()
        Try
            cmbZone.Items.Clear()
            cmbZone.ValueType = GetType(Integer)
            cmbZone.Items.Add(New DevExpress.Web.ListEditItem("", -1))
            Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Nothing)
            Dim intParentZone As Integer = retParentZoneID()
            If dTbl IsNot Nothing Then
                For Each dRow As DataRow In dTbl.Rows
                    If dRow("ID") <> intParentZone Then
                        cmbZone.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                    End If
                Next
            End If

            cmbPeriod.Items.Clear()
            cmbPeriod.ValueType = GetType(Integer)
            cmbPeriod.Items.Add(New DevExpress.Web.ListEditItem("", -1))
            dTbl = API.AccessPeriodServiceMethods.GetAccessPeriods(Nothing)

            If dTbl IsNot Nothing Then
                For Each dRow As DataRow In dTbl.Rows
                    cmbPeriod.Items.Add(New DevExpress.Web.ListEditItem(dRow("Name"), dRow("ID")))
                Next
            End If
        Catch ex As Exception
        End Try
    End Sub

    Private Function retParentZoneID() As Integer
        Dim intReturn As Integer = -1

        Try
            Dim dTbl As DataTable = API.ZoneServiceMethods.GetZones(Me.Page)
            Dim dRows() As DataRow = dTbl.Select("IDParent IS NULL")
            If dRows.Length > 0 Then
                intReturn = dRows(0)("ID")
            End If
        Catch ex As Exception
        End Try

        Return intReturn
    End Function

End Class