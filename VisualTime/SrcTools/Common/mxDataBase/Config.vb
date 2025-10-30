Imports System.Data.SQLite



Namespace Database
    Public Class clsConfigTbl
        Private mConn As SQLiteConnection

        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::New:", ex)
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
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS sysconfig (name TEXT PRIMARY KEY, value TEXT);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsConfigTbl::CreateTable:sysconfig table created.")

                'Creamos indice de tarjetas
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS idxsysconfig ON sysconfig(name);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsConfigTbl::CreateTable:sysconfig index created.")
                SQLcommand.Dispose()

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::CreateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM sysconfig;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsConfigTbl::ClearTable:sysconfig table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::ClearTable:", ex)
            End Try
        End Sub

        Public Sub setConfig(ByVal Name As String, ByVal Value As String, Optional ByRef Conn As SQLiteConnection = Nothing)
            Try
                Dim SQLcommand As SQLiteCommand

                If Conn Is Nothing Then
                    SQLcommand = mConn.CreateCommand
                Else
                    SQLcommand = Conn.CreateCommand
                End If

                SQLcommand.CommandText = "update sysconfig set value='" + Value + "' where name='" + Name + "'"
                If SQLcommand.ExecuteNonQuery() = 0 Then
                    Try
                        SQLcommand.CommandText = "insert into sysconfig(name,value) values('" + Name + "'," + clsGlobal.Any2SQL(Value) + ")"
                        SQLcommand.ExecuteNonQuery()
                        Debug.WriteLine("clsConfigTbl::setConfig::" + Name + "=" + clsGlobal.Any2String(Value))
                    Catch ex As Exception
                        clsSystemControl.ControlException(ex)
                    End Try
                Else
                    Debug.WriteLine("clsConfigTbl::setConfig::" + Name + "=" + clsGlobal.Any2String(Value))
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::setConfig:", ex)
            End Try

        End Sub

        Public Function getConfig(ByVal Name As String, Optional ByRef Conn As SQLiteConnection = Nothing) As String

            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Dim sValue As String = ""
            Try
                'Buscamos los datos del empleado
                If Conn Is Nothing Then
                    SQLcommand = mConn.CreateCommand
                Else
                    SQLcommand = Conn.CreateCommand
                End If

                SQLcommand.CommandText = "select value from sysconfig where name='" + Name + "'"
                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                'Si hay datos los carga
                If SQLReader.Read() Then
                    sValue = SQLReader.GetString(0)
                End If
                SQLReader.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeesTbl::getName:", ex)
            End Try

            Return sValue

        End Function

        ''' <summary>
        ''' Obtiene los datos de la configuración
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getConfig() As Dictionary(Of String, String)
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Dim dic As Dictionary(Of String, String) = New Dictionary(Of String, String)
            Try
                'Buscamos la version de la tabla
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "select * from sysconfig"
                SQLReader = SQLcommand.ExecuteReader()

                'Si hay datos los carga
                While SQLReader.Read()
                    dic.Add(SQLReader.GetString(0), SQLReader.GetString(1))
                    Debug.WriteLine("clsConfigTbl::getConfig:" + SQLReader.GetString(0) + "=" + SQLReader.GetString(1))
                End While
                SQLReader.Dispose()
                SQLcommand.Dispose()
                Return dic
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::getConfig:", ex)
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Guarda la configuración actual
        ''' </summary>
        ''' <param name="Value"></param>
        ''' <remarks></remarks>
        Public Sub setConfig(ByVal Value As Dictionary(Of String, String))
            Try
                If Not Value Is Nothing Then
                    Dim SQLcommand As SQLiteCommand

                    'Buscamos la version de la tabla
                    SQLcommand = mConn.CreateCommand
                    For Each item As KeyValuePair(Of String, String) In Value
                        'SQLcommand.CommandText = "insert or replace into sysconfig(name,value) values('" + item.Key + "'," + clsGlobal.Any2SQL(item.Value) + ")"
                        'SQLcommand.ExecuteNonQuery()

                        SQLcommand.CommandText = "update sysconfig set value='" + item.Value + "' where name='" + item.Key + "'"
                        If SQLcommand.ExecuteNonQuery() = 0 Then
                            Try
                                SQLcommand.CommandText = "insert into sysconfig(name,value) values('" + item.Key + "'," + clsGlobal.Any2SQL(item.Value) + ")"
                                SQLcommand.ExecuteNonQuery()
                                Debug.WriteLine("clsConfigTbl::setConfig::" + item.Key + "=" + clsGlobal.Any2String(item.Value))
                                System.Windows.Forms.Application.DoEvents()
                            Catch ex As Exception
                                clsSystemControl.ControlException(ex)
                            End Try
                        Else
                            Debug.WriteLine("clsConfigTbl::setConfig::" + item.Key + "=" + clsGlobal.Any2String(item.Value))
                        End If
                    Next
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::setConfig:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Reindexa la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReindexTable()
            Try
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "REINDEX sysconfig;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsConfigTbl::ReindexTable:sysconfig index has updated.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsConfigTbl::ReindexTable:", ex)
            End Try
        End Sub
    End Class
End Namespace
