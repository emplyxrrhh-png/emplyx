Imports system.Data.SQLite


Namespace Database
    Public Class clsVersionsTbl
        Private Const COL_tableName = 0
        Private Const COL_Version = 1
        Private Const COL_LastUpdate = 2

        Private _TableName As String
        Private _Version As DateTime
        Private _lastUpdate As DateTime

        Private mConn As SQLiteConnection


        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsVersionTbl::New:", ex)
            End Try
        End Sub

#Region "Gestion de tabla"


        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub CreateTable()
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS version (tablename TEXT PRIMARY KEY  NOT NULL  UNIQUE , version DATETIME, lastupdate DATETIME);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsVersionTbl::CreateTable:version table created.")

                'Creamos indice de tarjetas
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS idxversion ON version(tablename);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsVersionTbl::CreateTable:version index created.")

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsVersionTbl::CreateTable:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borrar datos de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM version;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsVersionTbl::ClearTable:version table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsVersionTbl::ClearTable:", ex)
            End Try
        End Sub

#End Region

#Region "Consultas"

        ''' <summary>
        ''' Obtiene la version de una tabla
        ''' </summary>
        ''' <param name="TableName"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getVersion(ByVal TableName As String) As DateTime
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try
                'Si el usuario ya esta en memoria no lo volvemos a buscar
                If _TableName <> TableName Then

                    'Buscamos la version de la tabla
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from version where tablename='" + TableName + "'"
                    SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                    'Si hay datos los carga
                    If SQLReader.Read() Then
                        _TableName = SQLReader.GetString(COL_tableName)
                        _Version = SQLReader.GetDateTime(COL_Version)
                        _lastUpdate = SQLReader.GetDateTime(COL_LastUpdate)
                        SQLReader.Dispose()
                        SQLcommand.Dispose()
                        Return _Version
                    Else
                        _TableName = TableName
                        _Version = clsGlobal.NULLDATETIME
                        _lastUpdate = clsGlobal.NULLDATETIME
                        SQLReader.Dispose()
                        SQLcommand.CommandText = "INSERT OR REPLACE INTO version VALUES (" + clsGlobal.Any2SQL(TableName) + "," + clsGlobal.Any2SQL(clsGlobal.NULLDATETIME) + "," + clsGlobal.Any2SQL(clsGlobal.NULLDATETIME) + ");REINDEX version;"
                        SQLcommand.ExecuteNonQuery()
                        'indexamos tabla
                        SQLcommand.CommandText = "REINDEX version;"
                        SQLcommand.ExecuteNonQuery()

                        SQLcommand.Dispose()
                        Return _Version
                    End If
                Else
                    Return _Version
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsVersionTbl::getVersion:", ex)
            End Try

        End Function

        ''' <summary>
        ''' Asigna la version en una tabla
        ''' </summary>
        ''' <param name="TableName"></param>
        ''' <param name="Version"></param>
        ''' <remarks></remarks>
        Public Sub setVersion(ByVal TableName As String, ByVal Version As DateTime)
            Dim SQLcommand As SQLiteCommand
            Try
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "update version set version=" + clsGlobal.Any2SQL(Version) + " where tablename=" + clsGlobal.Any2SQL(TableName)
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsVersionTbl::setVersion:", ex)
            End Try
        End Sub
#End Region

    End Class
End Namespace