Imports system.Data.SQLite

Namespace Database

    Public Class clsAccessTbl

        Private Const COL_idGroup = 0
        Private Const COL_idReader = 1

        Private mConn As SQLiteConnection

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
                'Creamos la tabla de grupos
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS access (idGroup INTEGER, idReader INTEGER);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsAccessTbl::CreateTable:access table created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsAccessTbl::CreateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM access;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsAccessTbl::ClearTable:access table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsAccessTbl::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade una linea en la tabla
        ''' </summary>
        ''' <param name="IDGroup"></param>
        ''' <param name="IDReader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDGroup As Integer, ByVal IDReader As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO access(idGroup, idReader) VALUES(" + clsGlobal.Any2SQL(IDGroup) + "," + clsGlobal.Any2SQL(IDReader) + ");"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsAccessTbl::addRow:New row(" + IDGroup.ToString + "," + IDReader.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsAccessTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Obtiene si tiene acceso al lector
        ''' </summary>
        ''' <param name="IDGroup"></param>
        ''' <param name="IDReader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getReader(ByVal IDGroup As Integer, ByVal IDReader As Byte) As Boolean
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Dim bFind As Boolean = False
            Try
                'Buscamos los datos del empleado
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "select * from access where idGroup=" + IDGroup.ToString + " and idReader=" + IDReader.ToString
                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                'Si hay datos los carga
                If SQLReader.Read() Then
                    bFind = True
                Else
                    bFind = False
                End If
                SQLReader.Dispose()
                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::getGroup:", ex)
                bFind = False
            End Try

            Return bFind

        End Function

        ''' <summary>
        ''' Obtiene si tiene alguno de los grupos indicados tiene acceso al lector indicado
        ''' </summary>
        ''' <param name="Groups"></param>
        ''' <param name="IDReader"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getReader(ByVal Groups As List(Of String), ByVal IDReader As Byte) As Boolean
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Dim bFind As Boolean = False
            Try
                If Groups.Count > 0 Then
                    'Buscamos los datos del empleado
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from access where idGroup in (" + String.Join(",", Groups.ToArray) + ") and idReader=" + IDReader.ToString
                    SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                    'Si hay datos los carga
                    If SQLReader.Read() Then
                        bFind = True
                    Else
                        bFind = False
                    End If
                    SQLReader.Dispose()
                    SQLcommand.Dispose()
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::getGroup:", ex)
                bFind = False
            End Try

            Return bFind

        End Function


    End Class
End Namespace