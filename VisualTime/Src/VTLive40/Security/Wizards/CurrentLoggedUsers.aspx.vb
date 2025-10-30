Imports System.Reflection
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class Base_CurrentLoggedUsers
    Inherits PageBase

    Private strObjectType As String = ""

    Protected Sub form1_Init(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Init

        Dim currentUsers As UserList = SecurityV2ServiceMethods.GetCurrentLoggedUsers(Me.Page)
        If currentUsers IsNot Nothing Then
            Dim current = GetDataTableFromArray(currentUsers.Users)

            GridUsers.DataSource = current
            GridUsers.DataBind()
        End If

    End Sub

    Protected Sub form1_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles form1.Load

        If Not Me.IsPostBack Then
            Me.GridUsers.Columns("Nombre").Caption = Me.Language.Translate("Name", Me.DefaultScope.ToLowerInvariant())
            Me.GridUsers.Columns("Descripción").Caption = Me.Language.Translate("Descriptiontable", Me.DefaultScope.ToLowerInvariant())
            Me.GridUsers.Columns("Origen").Caption = Me.Language.Translate("Origin", Me.DefaultScope.ToLowerInvariant())
        End If

    End Sub

    Public Shared Function GetDataTableFromArray(Of T)(ByVal list As IList(Of T)) As DataTable

        Dim table As New DataTable()
        Dim fields() As FieldInfo = GetType(T).GetFields()
        For Each field As FieldInfo In fields
            table.Columns.Add(field.Name, field.FieldType)
        Next
        For Each item As T In list
            Dim row As DataRow = table.NewRow()
            For Each field As FieldInfo In fields
                row(field.Name) = field.GetValue(item)
            Next
            table.Rows.Add(row)
        Next
        Return table
    End Function

    Private Sub btnAccept_Click(sender As Object, e As EventArgs) Handles btnAccept.Click
        Dim currentUsers As UserList = SecurityV2ServiceMethods.GetCurrentLoggedUsers(Me.Page)
        If currentUsers IsNot Nothing Then
            Dim current = GetDataTableFromArray(currentUsers.Users)

            GridUsers.DataSource = current
            GridUsers.DataBind()
        End If
    End Sub

End Class