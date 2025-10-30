Imports System.Data.Common
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Base.VTBusiness
Imports Robotics.VTBase.roTypes
Imports Robotics.Base.DTOs

Public Class RecalculatePunchesDirection
    Private Sub cmdOK_Click(sender As Object, e As EventArgs) Handles cmdOK.Click
        Dim dFirstDateTime As DateTime = DateTime.MinValue
        Dim dLastDateTime As DateTime = DateTime.MinValue
        Dim sEmployeeIds As String = String.Empty
        dFirstDateTime = dtFirsDate.Value
        dLastDateTime = dtLastDate.Value
        sEmployeeIds = txtEmployeeIDs.Text
        Me.Cursor = Cursors.WaitCursor
        RecalculatePunchDirection(dFirstDateTime, dLastDateTime, sEmployeeIds)
        Me.Cursor = Cursors.Default
    End Sub
    ''' <summary>
    ''' Este procedimiento está implementado como tarea de EOG
    ''' </summary>
    ''' <param name="dFirstDateTime"></param>
    ''' <param name="dLastDateTime"></param>
    ''' <param name="sEmployeeIDs"></param>
    ''' <returns></returns>
    Public Shared Function RecalculatePunchDirection(ByVal dFirstDateTime As DateTime, ByVal dLastDateTime As DateTime, sEmployeeIDs As String) As Boolean

        Dim bolRet As Boolean = True

        Dim sWhere As String = String.Empty
        Dim sOriginalDirection As String = String.Empty
        Dim sFinalDirection As String = String.Empty
        Dim lEmployeesToRecalculate As New List(Of Integer)
        Dim oParameters As New roParameters("OPTIONS")

        Try
            Dim oPunchState As New Punch.roPunchState
            Dim dtPunches As New DataTable
            Dim oPunch As Punch.roPunch
            Dim strSQL As String

            Dim dFreezeDate As Object = Nothing
            Try
                dFreezeDate = oParameters.Parameter(Parameters.FirstDate)
            Catch ex As Exception
                dFreezeDate = DateTime.MinValue
            End Try

            If dFirstDateTime <> DateTime.MinValue AndAlso dLastDateTime <> DateTime.MinValue AndAlso dFirstDateTime <= dLastDateTime AndAlso sEmployeeIDs.Trim.Length > 0 AndAlso dFirstDateTime.Date >= dFreezeDate Then
                If sEmployeeIDs.Trim <> "*" Then
                    sWhere = "IDEmployee in (" & sEmployeeIDs & ") AND "
                End If

                sWhere += " DateTime between " & Any2Time(dFirstDateTime).SQLDateTime & " AND " & Any2Time(dLastDateTime).SQLDateTime
                sWhere += " AND Type in (3,7) ORDER BY IDEmployee ASC, Datetime ASC"

                dtPunches = Punch.roPunch.GetPunches(sWhere, oPunchState)

                If dtPunches.Rows.Count > 0 Then
                    For Each punchRow In dtPunches.Rows
                        oPunch = New Punch.roPunch(0, punchRow("ID"), oPunchState)
                        sOriginalDirection = oPunch.ActualType.ToString
                        oPunch.Save(,,, True,,, False)
                        sFinalDirection = oPunch.ActualType.ToString
                        ' sFinalDirection = oPunch.CalculateTypeAtt.ToString -> Requiere librarías 4.8 o superior
                        If sOriginalDirection <> sFinalDirection Then
                            If Not lEmployeesToRecalculate.Contains(oPunch.IDEmployee) Then lEmployeesToRecalculate.Add(oPunch.IDEmployee)
                            Try
                                strSQL = "@UPDATE# Punches set DebugInfo = '" & sOriginalDirection & "->" & sFinalDirection & "' where ID = " & oPunch.ID
                                ExecuteSql(strSQL)
                            Catch ex As Exception
                                'Por si la columna DebugInfo no existe. No debiera pasar
                                MsgBox("Algo fue mal ... Puede que no exista el campo DebugInfo en la tabla punches?", MsgBoxStyle.Critical, "Oopsss!")
                                Return False
                            End Try
                        End If
                    Next
                End If

                ' Marco recálculo si se modificó algo
                If lEmployeesToRecalculate.Count > 0 Then
                    strSQL = "@UPDATE# DailySchedule set Status = 0 where IDEmployee in (" & String.Join(",", lEmployeesToRecalculate.ToArray) & ") AND Date BETWEEN " & Any2Time(dFirstDateTime).SQLSmallDateTime & " AND " & Any2Time(dLastDateTime).SQLSmallDateTime & " AND DATE <= GETDATE()"
                    ExecuteSql(strSQL)
                End If

                If oPunchState.Result = PunchResultEnum.NoError Then
                    MsgBox("Finalizado correctamente", MsgBoxStyle.Critical, "Thumbs up!")
                Else
                    MsgBox("Algo fue mal ...", MsgBoxStyle.Critical, "Oopsss!")
                End If
            Else
                MsgBox("Invalid parameters or some date in freeze period ...", MsgBoxStyle.Critical, "Oopsss!")
            End If

        Catch ex As Exception
            MsgBox("Algo fue requetemal ..." & ex.Message, MsgBoxStyle.Critical, "Oopsss!")
        End Try

        Return bolRet
    End Function

End Class
