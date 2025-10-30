Imports System.Data.SQLite

Namespace Database

    ''' <summary>
    ''' Tabla de cambios en la tabla biometrica
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsBioChangeTbl
        Private mConn As SQLiteConnection

        Private Const COL_Action = 0
        Private Const COL_IDEmployee = 1
        Private Const COL_IDFinger = 2
        Private Const COL_TimeStamp = 3

        Public Enum ActionType
            add
            del
        End Enum

        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBioChangeTbl::New:", ex)
            End Try
        End Sub


        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateTable()
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                'Creamos la tabla de empleados
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS biochanges (Action TEXT, IDemployee INTEGER, IDFinger INTEGER, TimeStamp DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBioChangeTbl::CreateTable:biochanges table created.")

                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBioChangeTbl::CreateTable:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Elimina los tados de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM biochanges;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBioChangeTbl::ClearTable:biometrics table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBioChangeTbl::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Elimina una registro de la tabla
        ''' </summary>
        ''' <param name="RowID"></param>
        ''' <remarks></remarks>
        Public Sub DeleteBioChange(ByVal RowID As Long)
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "DELETE FROM biochanges where ROWID=" + RowID.ToString

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBioChangeTbl::DeleteBioChange:Deleted.")
                End If
                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBioChangeTbl::DeletePunch:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Añade un registro en la tabla
        ''' </summary>
        ''' <param name="Acction"></param>
        ''' <param name="IDEmployee"></param>
        ''' <param name="IDFinger"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal Acction As ActionType, ByVal IDEmployee As Integer, ByVal IDFinger As Byte) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand
                Dim sSQL As String

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                sSQL = "INSERT OR REPLACE INTO biochanges(Action, IDemployee, IDFinger, TimeStamp) VALUES("
                sSQL += clsGlobal.Any2SQL(Acction.ToString) + ", "
                sSQL += clsGlobal.Any2SQL(IDEmployee) + ", "
                sSQL += clsGlobal.Any2SQL(IDFinger) + ", "
                sSQL += "Datetime('" + Now.ToString("yyyy-MM-dd HH:mm:ss") + "'));"

                SQLcommand.CommandText = sSQL
                If SQLcommand.ExecuteNonQuery() > 0 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsBioChangeTbl::addRow:New row(" + Acction.ToString + "," + IDEmployee.ToString + "," + IDFinger.ToString + ").")
                End If
                SQLcommand.Dispose()
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBioChangeTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Obtien un registro de la tabla
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getBioChange() As clsBioChangeData
            Try

                Dim SQLcommand As SQLiteCommand
                Dim SQLReader As SQLiteDataReader

                'Buscamos los datos del empleado
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "SELECT *, rowid FROM biochanges where TimeStamp=(select min(TimeStamp) from biochanges)"
                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                'Si hay datos los carga
                If SQLReader.Read() Then
                    'PunchTime,Reader,Action,Incidence,PhotoName
                    Dim oBioChange As New clsBioChangeData
                    oBioChange.Action = clsGlobal.Any2String(SQLReader.GetValue(COL_Action))
                    oBioChange.IDEmployee = clsGlobal.Any2Long(SQLReader.GetValue(COL_IDEmployee))
                    oBioChange.IDFinger = clsGlobal.Any2Long(SQLReader.GetValue(COL_IDFinger))
                    oBioChange.TimeStamp = clsGlobal.Any2Date(SQLReader.GetValue(COL_TimeStamp))
                    oBioChange.rowID = clsGlobal.Any2Long(SQLReader.GetValue(4))
                    Return oBioChange
                Else
                    Return Nothing
                End If

                SQLReader.Dispose()
                SQLcommand.Dispose()
                SQLReader = Nothing
                SQLcommand = Nothing
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsBioChangeTbl::getBioChange:", ex)
                Return Nothing
            End Try

        End Function

    End Class

    ''' <summary>
    ''' Clase de datos para la tabla
    ''' </summary>
    ''' <remarks></remarks>
    Public Class clsBioChangeData
        Public Action As String
        Public IDEmployee As Integer
        Public IDFinger As Integer
        Public TimeStamp As DateTime
        Public Data As Byte()
        Public rowID As Long
    End Class
End Namespace
