Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Module roJobMoves
    '
    ' Funciones generales para producción
    '

    'Estado del empleado o del equipo
    Public Const S_BEGIN = "BEGIN"
    Public Const S_BEGINDISTRIBUTED = "BEGINDIS"
    Public Const S_BEGININCIDENCE = "BEGININCI"
    Public Const S_FINISH = "FINISH"
    Public Const S_IN = "IN"
    Public Const S_OUT = "OUT"

    Public Function ResetJobStatusForThisEmployeeMove(ByVal EmployeeID As Long, ByVal InDateTime As Object, ByVal OutDateTime As Object, ByRef LogHandle As roLog) As Boolean
        '
        ' Marca como no procesado el dia de DailySchedule al que afecta este
        '  movimiento de producción.
        '
        Dim myBeginDate As New roTime
        Dim myFinishDate As New roTime
        ResetJobStatusForThisEmployeeMove = True

        ' Si el movimiento no esta completo, no hace nada
        If Not IsDate(InDateTime) And Not IsDate(OutDateTime) Then Exit Function

        If Not IsDate(InDateTime) Then InDateTime = OutDateTime
        If Not IsDate(OutDateTime) Then OutDateTime = InDateTime

        ' Como versión preliminar, marcamos como no calculados desde el dia antes hasta el dia
        '  despues, asi nos curamos en salud.
        myBeginDate = Any2Time(InDateTime).Substract(1, "d")
        myFinishDate = Any2Time(OutDateTime).Add(1, "d")

        Try
            If Not ExecuteSql("@UPDATE# DailySchedule WITH (ROWLOCK) SET JobStatus=0 WHERE IDEmployee=" & EmployeeID &
                " AND Date BETWEEN " & myBeginDate.SQLSmallDateTime & " AND " &
                myFinishDate.SQLDateTime) Then
                ' Ha habido algun error obteniendo movimientos
                LogHandle.logMessage(roLog.EventType.roDebug, "ResetJobStatusForThisEmployeeMove: Error updating DailySchedule (Error: ).")
                ResetJobStatusForThisEmployeeMove = False
                Exit Function
            End If
        Catch Ex As DbException
            LogHandle.logMessage(roLog.EventType.roError, "ResetJobStatusForThisEmployeeMove: Error updating DailySchedule (Error: " & Ex.Message & ").", Ex)
            ResetJobStatusForThisEmployeeMove = False
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "ResetJobStatusForThisEmployeeMove: Error updating DailySchedule (Error: " & ex.Message & ").", ex)
            ResetJobStatusForThisEmployeeMove = False
        End Try

        ' Actualizamos para volver a exportar
        'SQLExecute "@UPDATE# DailySchedule SET IsExported=0 WHERE IDEmployee=" & EmployeeID & " AND Date =" & Any2Time(Any2Time(InDateTime).DateOnly).SQLSmallDateTime, mConnection

    End Function

End Module