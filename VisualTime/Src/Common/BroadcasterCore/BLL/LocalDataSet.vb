Partial Public Class LocalDataSet

    Partial Class EmployeeTimeZonesZKPush2DataTable

        Private Sub EmployeeTimeZonesZKPush2DataTable_ColumnChanging(sender As Object, e As Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.IDEmployeeColumn.ColumnName) Then
                'Agregar código de usuario aquí
            End If

        End Sub

    End Class

    Partial Class EmployeeAccesLevelMxaDataTable

        Private Sub EmployeeAccesLevelMxaDataTable_ColumnChanging(sender As Object, e As Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.Door1Column.ColumnName) Then
                'Agregar código de usuario aquí
            End If

        End Sub

    End Class

    Partial Class TimeZonesMxaDataTable

        Private Sub TimeZonesMxaDataTable_ColumnChanging(sender As Object, e As Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.BeginTime1Column.ColumnName) Then

                'Agregar código de usuario aquí
            End If

        End Sub

    End Class

    Partial Class PlatesDataTable

        Private Sub PlatesDataTable_ColumnChanging(ByVal sender As System.Object, ByVal e As System.Data.DataColumnChangeEventArgs) Handles Me.ColumnChanging
            If (e.Column.ColumnName = Me.PlateNumberColumn.ColumnName) Then
                'Agregar código de usuario aquí
            End If

        End Sub

    End Class

End Class