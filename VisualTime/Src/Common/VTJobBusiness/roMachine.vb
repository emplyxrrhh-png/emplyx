Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roMachine

#Region "Methods"

    Public Shared Function getMachineByTerminal(ByVal IDTerminal As Integer, ByVal oLog As roLog) As Integer
        Dim iMachine As Integer = 0

        Try
            Dim sSql As String = "@SELECT# [ID] FROM Machines WHERE IDTerminal=" + IDTerminal.ToString

            iMachine = roTypes.Any2Integer(ExecuteScalar(sSql))
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roMachine::getMachineByTerminal:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roMachine::getMachineByTerminal:", ex)
        End Try

        Return iMachine
    End Function

    Public Function ListMachines(ByVal oLog As roLog) As DataTable

        Dim oMachines As DataTable = Nothing
        Try

            Dim sSql As String = "@SELECT# [ID], ReaderInputCode, Name FROM Machines " &
                                 "WHERE [ID] > 0 " &
                                 "ORDER BY Name"
            oMachines = CreateDataTable(sSql, )
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roMachine::ListMachines:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roMachine::ListMachines:", ex)
        End Try

        Return oMachines

    End Function

    Public Function GetLastIDJob(ByVal intIDMachine As Integer) As Long
        '
        'Retorna el último trabajo donde se ha estado sin incidencia
        '

        Dim sSQL As String
        Dim ads As DbDataReader

        sSQL = "@SELECT# top 1 * FROM MachineJobMoves WHERE IDMachine = " & intIDMachine.ToString
        sSQL = sSQL & " AND InDateTime is Not Null AND OutDateTime is Not Null"
        '    sSQL = sSQL & " AND IDIncidence = 0"
        sSQL = sSQL & " Order by OutDateTime DESC,ID DESC "

        GetLastIDJob = 0

        ads = CreateDataReader(sSQL)
        If ads.Read Then
            GetLastIDJob = Any2Double(ads.Item("IDJob"))
            ads.Close()
            ads = Nothing
            Exit Function
        End If
        ads.Close()
        ads = Nothing

    End Function

#End Region

End Class