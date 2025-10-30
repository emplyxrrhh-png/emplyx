Imports Robotics.Web.Base

Partial Class frmAddSiren
    Inherits UserControlBase

    Protected Sub frmAddSiren_Load(sender As Object, e As EventArgs) Handles Me.Load
        If Not IsPostBack Then
            LoadInitialValues()
        End If
    End Sub

    Private Sub LoadInitialValues()
        cmbSirenWeekDay.Items.Clear()
        cmbSirenWeekDay.ValueType = GetType(Integer)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.1"), 1)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.2"), 2)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.3"), 3)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.4"), 4)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.5"), 5)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.6"), 6)
        cmbSirenWeekDay.Items.Add(Me.Language.Keyword("weekday.7"), 7)
    End Sub

End Class