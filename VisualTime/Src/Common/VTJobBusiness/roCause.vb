Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Public Class roCause

#Region "Methods"

    Public Shared Function GetName(ByVal IDIncidence As Integer, ByVal oLog As roLog) As String
        Dim sName As String = ""
        Try
            sName = roTypes.Any2String(ExecuteScalar("@SELECT# Name FROM Causes where [ID]=" + IDIncidence.ToString))
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roCause::ListCauses:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roCause::ListCauses:", ex)
        End Try

        Return sName
    End Function

    Public Shared Sub GetInfo(ByVal IDIncidence As Integer, ByRef sName As String, ByRef iReaderInput As Integer, ByVal oLog As roLog)
        sName = ""
        Try
            Dim dt As DataTable = CreateDataTable("@SELECT# Name, ReaderInputCode FROM Causes where [ID]=" + IDIncidence.ToString, )

            If dt.Rows.Count = 1 Then
                sName = roTypes.Any2String(dt.Rows(0).Item("Name"))
                iReaderInput = roTypes.Any2Integer(dt.Rows(0).Item("ReaderInputCode"))
            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roCause::ListCauses:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roCause::ListCauses:", ex)
        End Try
    End Sub

    Public Function ListCauses(ByVal oLog As roLog) As DataTable

        Dim oCauses As DataTable = Nothing
        Try

            Dim sSql As String = "@SELECT# [ID], ReaderInputCode, Name FROM Causes " &
                                 "WHERE [ID] > 0 AND " &
                                       "AllowInputFromReader = 1 " &
                                 "ORDER BY Name"
            oCauses = CreateDataTable(sSql, )
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roCause::ListCauses:", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roCause::ListCauses:", ex)
        End Try

        Return oCauses

    End Function

#End Region

End Class