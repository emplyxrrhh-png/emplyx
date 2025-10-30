Option Strict On

Imports Robotics.DataLayer

Namespace VTDiagCore.QueryService

    Public Class Service

        Private Class SqlQueries
            Public Const GetQueries As String = "@SELECT# Id, Name, Description, Value, Parameters FROM sysroQueries"
        End Class

        Public Function GetQueries() As List(Of Query)
            Dim queriesTable = AccessHelper.CreateDataTable(SqlQueries.GetQueries)

            Dim iQueries As New Generic.List(Of Query)

            For Each oRow As DataRow In queriesTable.Rows
                iQueries.Add(Query.FromDataRow(oRow))
            Next

            Return iQueries 'queriesTable.Rows.Cast(Of DataRow).Select(Function(o) Query.FromDataRow(o))
        End Function

        Public Function RunQuery(id As Integer, Optional parameters As List(Of ParameterValue) = Nothing) As DataTable
            Dim queries = GetQueries()
            Dim currentQuery = queries.Find(Function(o) o.Id = id)

            If currentQuery.Parameters.Count > 0 Then
                If parameters Is Nothing Then Throw New ArgumentException("The selected query is parameterized but no parameters have been attached", NameOf(parameters))
                If currentQuery.Parameters.Count <> parameters.Count Then Throw New ArgumentException("The amount of parameters don't match the amount of parameters defined by the query", NameOf(parameters))
            End If

            Using command = AccessHelper.CreateDiagnosticsCommand(currentQuery.Value)
                command.CommandText = currentQuery.Value
                command.CommandTimeout = 0
                For Each parameter In currentQuery.Parameters
                    Dim currentParameter = AccessHelper.AddParameter(command, parameter.Name, parameter.CalculateDBType())
                    currentParameter.Value = CalculateValueFromType(parameter.Type, parameters.Single(Function(o) o.Name = parameter.Name).Value)
                Next

                Dim returnValue = New DataTable("Results")
                Dim dataAdapter = AccessHelper.CreateDataAdapter(command).Fill(returnValue)
                Return returnValue
            End Using
        End Function

        Public Function CalculateValueFromType(type As ParameterType, value As String) As Object
            Select Case type
                Case ParameterType.Decimal
                    Return Convert.ToDecimal(value, Globalization.CultureInfo.InvariantCulture)
                Case ParameterType.Integer
                    Return Convert.ToInt32(value, Globalization.CultureInfo.InvariantCulture)
                Case ParameterType.Date
                    Return DateTime.ParseExact(value, "dd/MM/yyyy", Globalization.CultureInfo.InvariantCulture)
                Case ParameterType.Time
                    Return DateTime.ParseExact(value, "HH:mm", Globalization.CultureInfo.InvariantCulture)
                Case ParameterType.Text
                    Return value
                Case Else
                    Throw New ArgumentOutOfRangeException(NameOf(type), $"El parámetro {NameOf(type)} no contiene un valor válido.")
            End Select
        End Function

    End Class

End Namespace