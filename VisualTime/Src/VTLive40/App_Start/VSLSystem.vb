Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.Extensions
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common

Namespace VSL
    Public Class VSLSystem

        Private Function VSL_CreateDataAdapter_DailyCauses(ByRef oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String =
                    "@SELECT# * " &
                    "FROM DailyCauses " &
                    "WHERE IDEmployee=@IDEmployee AND Date=@Date and IDCause=@IDCause  and Manual=1 and IDRelatedIncidence=0"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@Date", DbType.Date)
                AddParameter(cmd, "@idCause", DbType.Int16)
                da = CreateDataAdapter(cmd, True)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "VSLSystem::VSL_CreateDataAdapter_DailyCauses")
            End Try

            Return da
        End Function

        Private Function VSL_CreateDataAdapter_DailySchedule(ByRef oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String =
                    "@SELECT# idEmployee, Date, Status " &
                    "FROM DailySchedule " &
                    "WHERE IDEmployee=@IDEmployee AND Date=@Date"
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@Date", DbType.Date)

                da = CreateDataAdapter(cmd, True)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "VSLSystem::VSL_CreateDataAdapter_DailySchedule")
            End Try

            Return da
        End Function

        Private Sub VSL_UpdateCause(ByVal dtCauses As DataTable, ByVal ShortName As String, ByVal adpDailyCauses As DbDataAdapter, ByVal mIDEmpleado As Long, ByVal Fecha As Date, ByVal Value As Double, ByRef oState As roDataLinkState)
            Dim dt As New DataTable

            Try
                ' Selecciona la justificacione
                Dim idCause As Integer = VSL_IdCauseGet(dtCauses, ShortName, oState)
                If idCause = 0 Then Exit Try

                Dim row As DataRow

                If adpDailyCauses Is Nothing Then
                    adpDailyCauses = Me.VSL_CreateDataAdapter_DailyCauses(oState)
                End If

                ' Selecciona el registro en DailyCauses
                adpDailyCauses.SelectCommand.Parameters("@idEmployee").Value = mIDEmpleado
                adpDailyCauses.SelectCommand.Parameters("@Date").Value = Fecha
                adpDailyCauses.SelectCommand.Parameters("@idCause").Value = idCause
                adpDailyCauses.Fill(dt)
                'RS.Open("@SELECT# * FROM dailycauses WHERE IDEmployee=" & mIDEmpleado & " AND Date=" & Any2Time(Fecha).SQLSmallDateTime & " and IDCause=" & IDCause & " and Manual =1 and IDRelatedIncidence=0", m_Connection, adOpenDynamic, adLockOptimistic)

                If dt.Rows.Count = 0 Then
                    If Value = 0 Then Exit Try

                    ' Crea el registro
                    row = dt.NewRow
                    row("IDEmployee") = mIDEmpleado
                    row("Date") = Fecha
                    row("IDCause") = idCause
                    row("Value") = Value
                    row("IDRelatedIncidence") = 0
                    row("Manual") = 1
                Else
                    row = dt.Rows(0)
                    If Value = 0 Then
                        row.Delete()
                    Else
                        row("Value") = Value
                    End If
                End If

                ' Actualiza el valor
                If dt.Rows.Count = 0 Then dt.Rows.Add(row)
                adpDailyCauses.Update(dt)


            Catch ex As Exception
                oState.UpdateStateInfo(ex, "VSLSystem::VSL_UpdateCause")
            End Try

            dt.Dispose()
        End Sub

        Private Function VSL_IdCauseGet(ByVal dtCauses As DataTable, ByVal ShortName As String, ByRef oState As roDataLinkState) As Integer
            Dim row() As DataRow
            Dim iRet As Integer = 0

            Try
                row = dtCauses.Select("ShortName='" & ShortName & "'")

                If row.Length > 0 Then iRet = row(0)("id")

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "VSLSystem::VSL_IdCauseGet")
            End Try

            Return iRet
        End Function

#Region "Helper methods"
        Public Function VSL_UpdateDailyCauses(ByVal adpDailyCauses As DbDataAdapter, ByVal dtCauses As DataTable, ByVal mIDEmpleado As String, ByVal Fecha As Date, ByVal HorasNormales As Double, ByVal HorasExtrasNormales As Double, ByVal HorasExtrasSabados As Double, ByVal HorasExtrasFestivos As Double, ByVal BolsaHorasExtrasNormales As Double, ByVal BolsaHorasExtrasFestivos As Double, ByVal DietasPrimera As Double, ByVal DietasMedia As Double, ByVal DietasComida As Double, ByVal DietasPlusAltura As Double, ByVal DietasTrans As Double, ByRef oState As roDataLinkState)
            Dim bReturn As Boolean = False
            Try

                If adpDailyCauses Is Nothing Then
                    adpDailyCauses = Me.VSL_CreateDataAdapter_DailyCauses(oState)
                    dtCauses = CreateDataTable("@SELECT# id,Name,ShortName from causes", "Causes")
                End If

                VSL_UpdateCause(dtCauses, "DIU", adpDailyCauses, mIDEmpleado, Fecha, HorasNormales, oState)
                VSL_UpdateCause(dtCauses, "EXA", adpDailyCauses, mIDEmpleado, Fecha, HorasExtrasNormales, oState)
                VSL_UpdateCause(dtCauses, "EXB", adpDailyCauses, mIDEmpleado, Fecha, HorasExtrasSabados, oState)
                VSL_UpdateCause(dtCauses, "EXC", adpDailyCauses, mIDEmpleado, Fecha, HorasExtrasFestivos, oState)
                VSL_UpdateCause(dtCauses, "BE", adpDailyCauses, mIDEmpleado, Fecha, BolsaHorasExtrasNormales, oState)
                VSL_UpdateCause(dtCauses, "BEF", adpDailyCauses, mIDEmpleado, Fecha, BolsaHorasExtrasFestivos, oState)
                VSL_UpdateCause(dtCauses, "DP", adpDailyCauses, mIDEmpleado, Fecha, DietasPrimera, oState)
                VSL_UpdateCause(dtCauses, "DM", adpDailyCauses, mIDEmpleado, Fecha, DietasMedia, oState)
                VSL_UpdateCause(dtCauses, "DCT", adpDailyCauses, mIDEmpleado, Fecha, DietasComida, oState)
                VSL_UpdateCause(dtCauses, "PA", adpDailyCauses, mIDEmpleado, Fecha, DietasPlusAltura, oState)
                VSL_UpdateCause(dtCauses, "TRN", adpDailyCauses, mIDEmpleado, Fecha, DietasTrans, oState)

                bReturn = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "VSLSystem::VSL_UpdateDailyCauses")
                bReturn = False
            End Try

            Return bReturn
        End Function

        Public Function VSL_UpdateDailySchedule(ByVal adpDailySchedule As DbDataAdapter, ByVal mIDEmpleado As Long, ByVal Fecha As Date, ByRef oState As roDataLinkState)
            Dim bReturn As Boolean = False

            Try
                Dim dt As New DataTable
                Dim row As DataRow

                If adpDailySchedule Is Nothing Then
                    adpDailySchedule = Me.VSL_CreateDataAdapter_DailySchedule(oState)
                End If


                ' Selecciona el registro en DailySchedule
                adpDailySchedule.SelectCommand.Parameters("@idEmployee").Value = mIDEmpleado
                adpDailySchedule.SelectCommand.Parameters("@Date").Value = Fecha
                adpDailySchedule.Fill(dt)
                'RS.Open("@SELECT# * FROM DailySchedule WHERE IDEmployee=" & EmployeeID & " AND Date=" & Any2Time(moveDate).SQLSmallDateTime, m_Connection, adOpenForwardOnly, adLockOptimistic)

                ' El registro no existe, crea ahora
                If dt.Rows.Count = 0 Then
                    row = dt.NewRow
                    row("IDEmployee") = mIDEmpleado
                    row("Fecha") = Fecha
                Else
                    row = dt.Rows(0)
                End If

                ' Actualiza status
                row("Status") = 65

                ' Guarda cambios
                If dt.Rows.Count = 0 Then dt.Rows.Add(row)
                adpDailySchedule.Update(dt)

                bReturn = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "VSLSystem::VSL_UpdateDailySchedule")
                bReturn = False
            End Try

            Return bReturn
        End Function

        Public Sub VSL_InitCalcProcess()
            roConnector.InitTask(TasksType.DAILYCAUSES)
        End Sub

#End Region

    End Class
End Namespace