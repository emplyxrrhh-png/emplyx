Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Public Class roJobIncidence

#Region "Methods"

    Public Function ListJobIncidences(ByVal oLog As roLog) As DataTable

        Dim oIncidences As DataTable = Nothing
        Try

            Dim sSql As String = "@SELECT# [ID], ReaderInputCode, Name FROM JobIncidences " &
                                 "WHERE [ID] > 0 " &
                                 "ORDER BY Name"
            oIncidences = CreateDataTable(sSql, )
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJobIncidence::ListJobIncidences:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJobIncidence::ListJobIncidences:", ex)
        End Try

        Return oIncidences

    End Function

    Public Function GetInfo(ByVal intID As Integer, ByRef strName As String, ByRef intReaderInputCode As Integer, ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False
        Dim rd As DbDataReader = Nothing

        Try

            strName = ""
            intReaderInputCode = 0

            Dim sSql As String = "@SELECT# ReaderInputCode, Name FROM JobIncidences " &
                                 "WHERE [ID] = " & intID.ToString
            rd = CreateDataReader(sSql)
            If rd.Read Then
                strName = rd("Name")
                If Not IsDBNull(rd("ReaderInputCode")) Then intReaderInputCode = rd("ReaderInputCode")
            End If
            rd.Close()

            bolRet = True
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJobIncidence::GetInfo:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJobIncidence::GetInfo:", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()
        End Try

        Return bolRet

    End Function

#End Region

End Class