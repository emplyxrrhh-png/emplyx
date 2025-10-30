Imports system.Data.SQLite

Namespace Database


    Public Class clsSirensTbl

        Private Const COL_DayOf = 0
        Private Const COL_BeginTime = 1
        Private Const COL_idRelay = 2
        Private Const COL_Duration = 3

        Private mConn As SQLiteConnection
        Private mList As List(Of clsSirenData)

        Public Sub New(ByRef Conn As SQLiteConnection)
            mConn = Conn
            mList = New List(Of clsSirenData)
        End Sub

        ''' <summary>
        ''' Devuelve la lista de sirenas que hay para hoy
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property SirensToday() As List(Of clsSirenData)
            Get
                Return mList
            End Get
        End Property

        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateTable()
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                'Creamos la tabla de horarios
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS sirens (DayOf INTEGER, BeginTime DATETIME, idRelay INTEGER, duration INTEGER);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:sirens table created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsSirensTbl::CreateTable:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra los datos de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM sirens;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsSirensTbl::ClearTable:sirens table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsSirensTbl::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade un registro en la tabla
        ''' </summary>
        ''' <param name="DayOf"></param>
        ''' <param name="StartTime"></param>
        ''' <param name="Relay"></param>
        ''' <param name="Seconds"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal DayOf As Integer, ByVal StartTime As DateTime, ByVal Relay As Byte, ByVal Seconds As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand
                Dim sSQL As String

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                sSQL = "INSERT OR REPLACE INTO sirens(DayOf, BeginTime, idRelay, duration) VALUES("
                sSQL += clsGlobal.Any2SQL(DayOf) + ", "
                sSQL += clsGlobal.Any2SQL(StartTime) + ", "
                sSQL += clsGlobal.Any2SQL(Relay) + ", "
                sSQL += clsGlobal.Any2SQL(Seconds) + ");"

                SQLcommand.CommandText = sSQL
                If SQLcommand.ExecuteNonQuery() > 0 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsSirensTbl::addRow:New row(" + DayOf.ToString + "," + StartTime.ToShortTimeString + "," + Relay.ToString + "," + Seconds.ToString + ").")
                End If
                SQLcommand.Dispose()
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsSirensTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Carga las sirenas de hoy en la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Load()
            Try
                Dim sSQL As String = "Select * from sirens where"
                If Now.DayOfWeek = DayOfWeek.Sunday Then
                    sSQL += " DayOf=0 or DayOf=7"
                Else
                    sSQL += " DayOf=" + clsGlobal.Any2Long(Now.DayOfWeek).ToString
                End If
                Dim SQLcommand As New SQLiteCommand(sSQL, mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader

                mList.Clear()
                Dim oSiren As clsSirenData
                While sqReader.Read
                    oSiren = New clsSirenData
                    oSiren.StartDate = Now.Date.AddHours(sqReader.GetDateTime(COL_BeginTime).Hour).AddMinutes(sqReader.GetDateTime(COL_BeginTime).Minute)
                    oSiren.Duration = sqReader.GetInt32(COL_Duration)
                    oSiren.EndDate = oSiren.StartDate.AddSeconds(oSiren.Duration)
                    mList.Add(oSiren)
                End While
                'clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsSirensTbl::Load:Loaded " + mList.Count.ToString + " sirens.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsSirensTbl::Load:", ex)
            End Try
        End Sub

    End Class

    ''' <summary>
    ''' Estructura de datos de rirenas
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsSirenData
        Public StartDate As DateTime
        Public Duration As Integer
        Public EndDate As DateTime
    End Class
End Namespace
