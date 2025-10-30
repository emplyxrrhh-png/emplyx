Imports system.Data.SQLite

Namespace Database


    Public Class clsTimeZonesTbl

        Private Const COL_idGroup = 0
        Private Const COL_idReader = 1
        Private Const COL_weekDay = 2
        Private Const COL_BeginTime = 3
        Private Const COL_EndTime = 4

        Private mConn As SQLiteConnection

        Private mIDGroup As Integer
        Private mIDReader As Byte
        Private mWeekDay As Byte
        Private mList As List(Of TimezoneData)

        Public Sub New(ByRef Conn As SQLiteConnection)
            mConn = Conn
        End Sub

        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateTable()
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                'Creamos la tabla de horarios
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS timezones (idGroup INTEGER, idReader INTEGER, weekDay INTEGER, BeginTime DATETIME, EndTime DATETIME);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsTimeZonesTbl:timezones table created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTimeZonesTbl::CreateTable:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra datos de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM timezones;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mIDGroup = 0
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsTimeZonesTbl::ClearTable:timezones table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTimeZonesTbl::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade registro en la tabla
        ''' </summary>
        ''' <param name="idGroup"></param>
        ''' <param name="idReader"></param>
        ''' <param name="weekDay"></param>
        ''' <param name="BeginTime"></param>
        ''' <param name="EndTime"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal idGroup As Integer, ByVal idReader As Integer, ByVal weekDay As Byte, ByVal BeginTime As DateTime, ByVal EndTime As DateTime) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand
                Dim sSQL As String

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                sSQL = "INSERT OR REPLACE INTO timezones(idGroup, idReader, weekDay, BeginTime, EndTime) VALUES ("
                sSQL += clsGlobal.Any2SQL(idGroup) + ", "
                sSQL += clsGlobal.Any2SQL(idReader) + ", "
                sSQL += clsGlobal.Any2SQL(weekDay) + ", "
                sSQL += clsGlobal.Any2SQL(BeginTime) + ", "
                sSQL += clsGlobal.Any2SQL(EndTime) + ")"
                SQLcommand.CommandText = sSQL
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsTimeZonesTbl::addRow:New row.(" + idGroup.ToString + "," + idReader.ToString + "," + weekDay.ToString + "," + BeginTime.ToString("HH:mm:ss") + "," + EndTime.ToString("HH:mm:ss") + ")")
                mIDGroup = 0
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsTimeZonesTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Devuelve las zonas que afectan a un grupo, lector y dia
        ''' </summary>
        ''' <param name="IDGroup"></param>
        ''' <param name="IDReader"></param>
        ''' <param name="WeekDay"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getTimezones(ByVal IDGroup As Integer, ByVal IDReader As Byte, ByVal WeekDay As Byte) As List(Of TimezoneData)
            Dim ls As List(Of TimezoneData) = New List(Of TimezoneData)
            Try
                If mIDGroup = IDGroup And mIDReader = IDReader And mWeekDay = WeekDay Then
                    Return mList
                Else
                    Dim sql As String = "Select * from timezones where idGroup=" + IDGroup.ToString + " and idReader=" + IDReader.ToString
                    If WeekDay > 6 Then
                        sql += " and (weekDay=7 or weekDay=0)"
                    Else
                        sql += " and weekDay=" + WeekDay.ToString
                    End If
                    Dim cmd As New SQLiteCommand(sql, mConn)
                    Dim sqReader As SQLiteDataReader = cmd.ExecuteReader

                    Dim otz As TimezoneData
                    While sqReader.Read
                        otz = New TimezoneData
                        otz.BeginTime = Now.Date.AddHours(sqReader.GetDateTime(COL_BeginTime).Hour).AddMinutes(sqReader.GetDateTime(COL_BeginTime).Minute)
                        otz.EndTime = Now.Date.AddHours(sqReader.GetDateTime(COL_EndTime).Hour).AddMinutes(sqReader.GetDateTime(COL_EndTime).Minute)
                        ls.Add(otz)
                    End While
                    mIDGroup = IDGroup
                    mIDReader = IDReader
                    mWeekDay = WeekDay
                    mList = ls
                    sqReader.Dispose()
                    cmd.Dispose()
                    Return mList
                End If

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                Return New List(Of TimezoneData)
            End Try
        End Function
    End Class

    ''' <summary>
    ''' Estructura de datos de periodos
    ''' </summary>
    ''' <remarks></remarks>
    Public Class TimezoneData
        Public BeginTime As DateTime
        Public EndTime As DateTime
    End Class
End Namespace