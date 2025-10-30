Imports system.Data.SQLite

Namespace Database
    Public Class clsCausesTbl

        Private Const COL_idIncidence = 0
        Private Const COL_Name = 1

        Private mCauses As Dictionary(Of String, String)

        Private mConn As SQLiteConnection

        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::New:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Obtiene el listado de incidencias
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public ReadOnly Property Causes() As Dictionary(Of String, String)
            Get
                'Si aun no estan cargadas las carga.
                If mCauses Is Nothing Then
                    Load()
                End If
                Return mCauses
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

                'Creamos la tabla de empleados
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS causes (idCause INTEGER PRIMARY KEY, name TEXT);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "DataBases::CreateDB:causes table created.")

                'Creamos indice de empleados
                SQLcommand.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS idxcauses ON causes(idCause);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "DataBases::CreateDB:causes index created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::CreateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM causes;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCausesTbl::ClearTable:causes table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::ClearTable:", ex)
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
                SQLcommand.CommandText = "REINDEX causes;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCausesTbl::ReindexTable:causes index has updated.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::ReindexTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade registro en la tabla
        ''' </summary>
        ''' <param name="IDCause"></param>
        ''' <param name="Name"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDCause As Integer, ByVal Name As String) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO causes(idCause,name) VALUES(" + clsGlobal.Any2SQL(IDCause) + "," + clsGlobal.Any2SQL(Name) + ");"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCausesTbl::addRow:New row(" + IDCause.ToString + "," + Name + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' genera el cache de incidencias
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub Load()
            Try

                mCauses = New Dictionary(Of String, String)
                Dim cmd As New SQLiteCommand("Select * from causes order by name", mConn)
                Dim sqReader As SQLiteDataReader = cmd.ExecuteReader

                Dim i As Integer = 0
                While sqReader.Read
                    mCauses.Add(clsGlobal.Any2String(sqReader.GetInt16(COL_idIncidence)), sqReader.GetString(COL_Name))
                    i += 1
                End While
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCausesTbl::Load:Loaded " + i.ToString + " causes.")
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::Load:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Obtiene el nombre de la incidencia dado el id
        ''' </summary>
        ''' <param name="IDCause"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getName(ByVal IDCause As Integer) As String

            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try

                'Si los valores estan en memoria primero lo buscamos de aqui
                If mCauses.ContainsKey(IDCause) Then
                    Return mCauses.Item(IDCause)
                End If

                'Buscamos los datos del empleado
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "select * from causes where idCause=" + IDCause.ToString
                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                'Si hay datos los carga
                If SQLReader.Read() Then
                    Return SQLReader.GetString(COL_Name)
                Else
                    Return ""
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCausesTbl::getName:", ex)
                Return ""
            End Try

        End Function
    End Class
End Namespace

