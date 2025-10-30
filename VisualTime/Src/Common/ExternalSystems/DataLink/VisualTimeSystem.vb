Imports System.Data.Common
Imports Robotics.Base.VTBusiness
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.VTBase
Imports Robotics.DataLayer.AccessHelper

Namespace VisualTime
    Public Class VisualTimeSystem
        Private Shared oLog As New roLog("VTExternalSystem")

        Public Shared Function VISUALTIME_PUNCHES_Line(ByRef ExportedFieldsObj As Object, ByVal dtRegistrosAExportar As DataTable, ByVal drEmployee As DataRow, ByVal drAccrual As DataRow, ByVal dtConcepts As DataTable, ByVal BeginDate As Date, ByVal EndDate As Date, Obj As Object) As String
            Dim errorMsg = ""
            Dim bolRet = False

            Try
                ' Crea el adaptador para seleccionar el primer y último fichaje del día
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::VISUALTIME_PUNCHES_Line:Start")

                Dim exportedFields = CType(ExportedFieldsObj, List(Of ProfileExportFields))
                Dim acrrualValue = drAccrual("TotalConcept")
                Dim adpPres As DbDataAdapter
                Dim adpShift As DbDataAdapter
                Dim dtPres As New DataTable
                Dim dtShift As New DataTable
                Dim firstInPunch = String.Empty
                Dim firstOutPunch = String.Empty
                Dim lastInPunch = String.Empty
                Dim lastOutPunch = String.Empty

                adpPres = VISUALTIME_CreateDataAdapterPunches()
                adpShift = VISUALTIME_CreateDataAdapterShift()

                adpShift.SelectCommand.Parameters("@idEmployee").Value = drEmployee("idEmployee")
                adpShift.SelectCommand.Parameters("@ShiftDate").Value = drAccrual("Date")
                adpShift.Fill(dtShift)

                adpPres.SelectCommand.Parameters("@idEmployee").Value = drEmployee("idEmployee")
                adpPres.SelectCommand.Parameters("@ShiftDate").Value = drAccrual("Date")
                adpPres.Fill(dtPres)

                If (dtShift.Rows.Count > 0) Then
                    exportedFields(4).Value = dtShift.Rows(0)("Name")
                End If

                If (dtPres.Rows.Count > 0) Then
                    For Each drPunch As DataRow In dtPres.Rows
                        If (drPunch("ActualType").ToString().Trim().Equals("1")) Then
                            If (String.IsNullOrEmpty(firstInPunch)) Then
                                firstInPunch = drPunch("DateTime")
                            Else
                                lastInPunch = drPunch("DateTime")
                            End If
                        ElseIf (drPunch("ActualType").ToString().Trim().Equals("2")) Then
                            If (String.IsNullOrEmpty(firstOutPunch)) Then
                                firstOutPunch = drPunch("DateTime")
                            Else
                                lastOutPunch = drPunch("DateTime")
                            End If
                        End If
                    Next
                End If
                exportedFields(5).Value = firstInPunch
                exportedFields(6).Value = firstOutPunch
                exportedFields(7).Value = lastInPunch
                exportedFields(8).Value = lastOutPunch

            Catch ex As Exception
                errorMsg = "ERROR::FIAT_InfoTipo_Line: " & ex.Message
                oLog.logMessage(roLog.EventType.roError, "ERROR::FIAT_SIMENU_Line: ", ex)

            Finally
                bolRet = False
                oLog.logMessage(roLog.EventType.roDebug, "VTExternalSystem::FIAT_SIMENU_Line:End")
            End Try

            Return IIf(bolRet, "", "ERROR :" & errorMsg)
        End Function

        Private Shared Function VISUALTIME_CreateDataAdapterPunches() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# DateTime, ActualType from punches where idEmployee=@idEmployee and shiftdate=@ShiftDate order by datetime"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@ShiftDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)

            Catch ex As Exception
                Return Nothing
            End Try

            Return da
        End Function

        Private Shared Function VISUALTIME_CreateDataAdapterShift() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# s.Name  FROM DailySchedule ds " &
                    "inner join Shifts s on ds.IDShiftUsed = s.ID where ds.IDEmployee=@idEmployee and ds.Date=@ShiftDate"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@ShiftDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)

            Catch ex As Exception
                Return Nothing
            End Try

            Return da
        End Function
    End Class
End Namespace


