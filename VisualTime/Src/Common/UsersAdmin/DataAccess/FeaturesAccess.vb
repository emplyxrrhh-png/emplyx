Imports Robotics.DataLayer

Namespace DataAccess

    Public NotInheritable Class FeaturesAccess

        Public Shared Sub GetList(ByVal table As DataTable, ByVal featureType As String, ByVal sEdition As String)

            Try
                Dim Command As DbCommand = AccessHelper.CreateCommand("UsersAdmin_Features_List")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@featureType", DbType.AnsiString, 1).Value = featureType
                AccessHelper.AddParameter(Command, "@edition", DbType.AnsiString, 20).Value = sEdition
                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(table)
            Finally
            End Try
        End Sub

    End Class

End Namespace