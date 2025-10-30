Imports system.Data.SQLite

Namespace Database

    Public Class clsGroupsTbl

        Private Const COL_idEmployee = 0
        Private Const COL_idGroup = 1

        Private mIDEmployee As Integer = 0
        Private mIDGroup As Integer = 0

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
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS groups (idEmployee INTEGER, idGroup INTEGER, PRIMARY KEY (idEmployee,idGroup));"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::CreateTable:groups table created.")

                'Creamos indice de grupos
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS groupsidx ON groups(idEmployee ASC);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::CreateTable:groups index created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::CreateTable:", ex)
            End Try

        End Sub

        Public Sub UpdateTable(ByVal Version As Byte)
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                Select Case Version
                    Case 3
                        'Se elimina la restricción de un grupo de accesos por empleado
                        SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS groupsAux (idEmployee INTEGER, idGroup INTEGER, PRIMARY KEY (idEmployee,idGroup));"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::UpdateTable:Aux groups table created.")

                        SQLcommand.CommandText = "INSERT INTO groupsAux SELECT * FROM groups;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::UpdateTable:Aux groups table populated.")

                        SQLcommand.CommandText = "DROP TABLE groups;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::UpdateTable:groups table droped.")

                        SQLcommand.CommandText = "ALTER TABLE groupsAux RENAME TO groups;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::UpdateTable:Aux groups table renamed.")

                        SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS groupsidx ON groups(idEmployee ASC);"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::CreateTable:groups index created.")

                        SQLcommand.Dispose()
                End Select
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::UpdateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM groups;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mIDEmployee = 0
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::ClearTable:groups table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade registro a la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="IDGroup"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDEmployee As Integer, ByVal IDGroup As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO groups(idEmployee,idGroup) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + "," + clsGlobal.Any2SQL(IDGroup) + ");"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::addRow:New row(" + IDEmployee.ToString + "," + IDGroup.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Elimina la información de autorizaciones de acceso para el empleado
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function delRow(ByVal IDEmployee As Integer, ByVal IDGroup As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "DELETE FROM groups WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee)
                If IDGroup <> 0 Then
                    SQLcommand.CommandText &= " AND idGroup=" & clsGlobal.Any2SQL(IDGroup)
                End If
                SQLcommand.CommandText &= ";"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsGroupsDB::delRow:Deleted row for employee " + IDEmployee.ToString + ")")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Obtien el grupo dado el empleado
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getGroup(ByVal IDEmployee As Integer) As Integer
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try
                'Si el usuario ya esta en memoria no lo volvemos a buscar
                If mIDEmployee <> IDEmployee Then

                    'Buscamos los datos del empleado
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from groups where idEmployee=" + IDEmployee.ToString
                    SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                    'Si hay datos los carga
                    If SQLReader.Read() Then
                        mIDEmployee = IDEmployee
                        mIDGroup = SQLReader.GetInt32(COL_idGroup)
                    Else
                        mIDEmployee = 0
                        mIDGroup = 0
                    End If
                    SQLReader.Dispose()
                    SQLcommand.Dispose()
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::getGroup:", ex)
                mIDGroup = 0
            End Try

            Return mIDGroup

        End Function


        ''' <summary>
        ''' Obtien la lista de grupos de acceso dado el empleado
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getGroups(ByVal IDEmployee As Integer) As List(Of String)
            Dim aResponse As New List(Of String)
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try
                'Si el usuario ya esta en memoria no lo volvemos a buscar
                If mIDEmployee <> IDEmployee Then

                    'Buscamos los datos del empleado
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from groups where idEmployee=" + IDEmployee.ToString
                    SQLReader = SQLcommand.ExecuteReader

                    Dim i As Integer = 0
                    While SQLReader.Read
                        aResponse.Add(SQLReader.GetInt32(COL_idGroup).ToString)
                        i += 1
                    End While

                    SQLReader.Dispose()
                    SQLcommand.Dispose()
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsGroupsDB::getGroup:", ex)
                mIDGroup = 0
            End Try

            Return aResponse

        End Function

    End Class

End Namespace