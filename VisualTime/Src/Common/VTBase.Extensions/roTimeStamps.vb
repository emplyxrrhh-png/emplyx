Imports Robotics.DataLayer.AccessHelper

Public Class roTimeStamps

    Public Shared Sub UpdateEmployeeTimestamp(Optional idEmployee As Integer = 0, Optional idPassport As Integer = 0)
        Dim sSQL As String = String.Empty
        Try
            If idEmployee > 0 Then
                sSQL = "@UPDATE# Employees SET Timestamp = GETDATE() WHERE Id = " & idEmployee.ToString
                ExecuteSql(sSQL)
            Else
                If idPassport > 0 Then
                    sSQL = "@UPDATE# Employee SET Timestamp = GETDATE() WHERE Id = (SELECT IdEmployee FROM sysroPassports WHERE ID = " & idEmployee.ToString & ")"
                    ExecuteSql(sSQL)
                End If
            End If
        Catch ex As Exception
        End Try
    End Sub

    Public Shared Function CheckIfScheduleHasChanged(row As DataRow) As Boolean
        Dim bRet As Boolean = False
        Dim columsExcluded As New List(Of String) From {"Remarks", "Status", "JobStatus", "LockedDay", "IDAssignment", "OldIDAssignment", "OldIDShift", "IDEmployeeCovered", "TaskStatus", "IDAssignmentBase", "IsHolidays", "IDDailyBudgetPosition", "WorkCenter", "GUID", "Timestamp"}

        Try
            Dim columnList As List(Of DataColumn) = row.Table.Columns.Cast(Of DataColumn)().ToList()
            ' Recorrer las columnas de la fila
            For Each column As DataColumn In columnList.Where(Function(c) Not columsExcluded.Contains(c.ColumnName)).ToList()
                ' Comparar los valores antiguos y nuevos de la columna
                Dim oldValue As Object = row(column, DataRowVersion.Original)
                Dim newValue As Object = row(column, DataRowVersion.Current)

                ' Verificar si la columna ha cambiado
                If Not Object.Equals(oldValue, newValue) Then
                    bRet = True
                    Exit For
                End If
            Next
        Catch ex As Exception
            ' Por ejemplo si la fila es nueva (no tiene versión anterior)
            bRet = True
        End Try

        Return bRet

    End Function

    Public Shared Function CheckIfScheduleHasChanged(dt As DataTable) As Boolean
        Dim bRet As Boolean = False
        Dim columsExcluded As New List(Of String) From {"Remarks", "Status", "JobStatus", "LockedDay", "IDAssignment", "OldIDAssignment", "OldIDShift", "IDEmployeeCovered", "TaskStatus", "IDAssignmentBase", "IsHolidays", "IDDailyBudgetPosition", "WorkCenter", "GUID", "Timestamp"}

        Try
            If dt.GetChanges() IsNot Nothing Then
                ' Obtener las filas modificadas
                Dim modifiedRows As DataRow() = dt.Select(Nothing, Nothing, DataViewRowState.ModifiedCurrent)

                Dim columnList As List(Of DataColumn) = dt.Columns.Cast(Of DataColumn)().ToList()

                ' Recorrer las filas modificadas
                For Each row As DataRow In modifiedRows
                    ' Recorrer las columnas de la fila
                    For Each column As DataColumn In columnList.Where(Function(c) Not columsExcluded.Contains(c.ColumnName)).ToList()
                        '  .ToList.Where(Function(column) Not lista.Contains(column.ColumnName)).ToList()
                        ' Comparar los valores antiguos y nuevos de la columna
                        Dim oldValue As Object = row(column, DataRowVersion.Original)
                        Dim newValue As Object = row(column, DataRowVersion.Current)

                        ' Verificar si la columna ha cambiado
                        If Not Object.Equals(oldValue, newValue) Then
                            bRet = True
                            Exit For
                        End If
                    Next
                Next
            End If
        Catch ex As Exception
            bRet = True
        End Try

        Return bRet

    End Function

End Class