Imports System.Data.Common
Imports Robotics.DataLayer

Public NotInheritable Class GuiAccess

    Public Shared Sub GetGui(ByVal table As DataTable, ByVal applicationAlias As String)
        Try
            Dim Command As DbCommand = AccessHelper.CreateCommand("WebPortal_Gui_SelectByApplication")
            Command.CommandType = CommandType.StoredProcedure
            AccessHelper.AddParameter(Command, "@applicationAlias", DbType.String, 200).Value = applicationAlias
            Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
            Adapter.Fill(table)
        Catch ex As Exception
        End Try
    End Sub

End Class