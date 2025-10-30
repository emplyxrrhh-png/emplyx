Imports system.Data.SQLite

Namespace Database
    Public Class clsEmployeesTbl

        Private Const COL_idEmployee = 0
        Private Const COL_Name = 1
        Private Const COL_PIN = 2
        Private Const COL_Language = 3
        Private Const COL_AllowCard = 4
        Private Const COL_AllowBio = 5
        Private Const COL_AllowPIN = 6
        Private Const COL_AllowCardBio = 7
        Private Const COL_StrPIN = 8
        Private Const COL_AllowedCauses = 9

        Private mIDEmployee As Integer = 0
        Private mName As String = ""
        Private mLastEmployeeData As clsEmployeeData = New clsEmployeeData

        Private mConn As SQLiteConnection

        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeesTbl::New:", ex)
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

                'Creamos la tabla de empleados
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS employees (idEmployee INTEGER PRIMARY KEY, name TEXT, pin INTEGER, language TEXT,AllowCard BOOLEAN DEFAULT 1,AllowBio BOOLEAN DEFAULT 1,AllowPIN BOOLEAN DEFAULT 1,AllowCardBio BOOLEAN DEFAULT 0,StrPIN TEXT,AllowedCauses TEXT);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:employees table created.")

                'Creamos indice de empleados
                SQLcommand.CommandText = "CREATE UNIQUE INDEX IF NOT EXISTS idxemployees ON employees(idEmployee);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:employees index created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::CreateTable:", ex)
            End Try

        End Sub


        Public Sub UpdateTable(ByVal Version As Byte)
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                Select Case Version
                    Case 1
                        'Actualizamos la tabla de empleados
                        SQLcommand.CommandText = "ALTER TABLE employees ADD COLUMN AllowCard BOOLEAN DEFAULT 1;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:employees table created.")

                        SQLcommand.CommandText = "ALTER TABLE employees ADD COLUMN AllowBio BOOLEAN DEFAULT 1;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:employees table created.")

                        SQLcommand.CommandText = "ALTER TABLE employees ADD COLUMN AllowPIN BOOLEAN DEFAULT 1;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:employees table created.")

                        SQLcommand.CommandText = "ALTER TABLE employees ADD COLUMN AllowCardBio BOOLEAN DEFAULT 0;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:employees table created.")
                    Case 2
                        SQLcommand.CommandText = "ALTER TABLE employees ADD COLUMN StrPIN TEXT;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::UpdateTable:StrPIN column created.")

                        SQLcommand.CommandText = "update employees set StrPIN = pin;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::UpdateTable:StrPIN informed.")
                    Case 3
                        SQLcommand.CommandText = "ALTER TABLE employees ADD COLUMN AllowedCauses TEXT;"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::UpdateTable:AllowedCauses column created.")

                        SQLcommand.CommandText = "update employees set AllowedCauses = '*';"
                        If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::UpdateTable:AllowedCauses informed.")
                End Select
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::UpdateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM employees;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mIDEmployee = 0
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeeTbl::ClearTable:employees table empty.")
                mLastEmployeeData = New clsEmployeeData

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::ClearTable:", ex)
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
                SQLcommand.CommandText = "REINDEX employees;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mIDEmployee = 0
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeeTbl::ReindexTable:employees index has updated.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::ReindexTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade un registro a la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="Name"></param>
        ''' <param name="PIN"></param>
        ''' <param name="Language"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDEmployee As Integer, ByVal Name As String, ByVal PIN As String, ByVal Language As String) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO employees(idEmployee,name,StrPIN,language) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + "," + clsGlobal.Any2SQL(Name) + "," + clsGlobal.Any2SQL(PIN) + "," + clsGlobal.Any2SQL(Language) + ");"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mIDEmployee = 0
                mLastEmployeeData = New clsEmployeeData
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeeTbl::addRow:New row(" + IDEmployee.ToString + "," + Name + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Borra un registro de la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function delRow(ByVal IDEmployee As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM employees WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee) + ";"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mIDEmployee = 0
                mLastEmployeeData = New clsEmployeeData
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsEmployeeTbl::delRow:delete row(" + IDEmployee.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeeTbl::delRow:", ex)
                Return False
            End Try

        End Function

#End Region

#Region "Consultas"

        Public Function getData(ByVal IDEmployee As Integer) As clsEmployeeData
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try
                'Si el usuario ya esta en memoria no lo volvemos a buscar
                If IDEmployee <> mLastEmployeeData.ID Or mLastEmployeeData.ID = 0 Then

                    'Buscamos los datos del empleado
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from employees where idEmployee=" + IDEmployee.ToString
                    SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                    'Si hay datos los carga
                    If SQLReader.Read() Then
                        mLastEmployeeData.ID = IDEmployee
                        mLastEmployeeData.Name = SQLReader.GetString(COL_Name)
                        mLastEmployeeData.PIN = clsGlobal.Any2String(SQLReader.GetValue(COL_StrPIN))
                        Try
                            mLastEmployeeData.AllowCard = clsGlobal.Any2Boolean(SQLReader.GetValue(COL_AllowCard))
                            mLastEmployeeData.AllowBio = clsGlobal.Any2Boolean(SQLReader.GetValue(COL_AllowBio))
                            mLastEmployeeData.AllowPIN = clsGlobal.Any2Boolean(SQLReader.GetValue(COL_AllowPIN))
                            mLastEmployeeData.AllowCardBio = clsGlobal.Any2Boolean(SQLReader.GetValue(COL_AllowCardBio))
                        Catch ex As Exception
                        End Try
                    Else
                        mLastEmployeeData = New clsEmployeeData
                    End If
                    SQLReader.Dispose()
                    SQLcommand.Dispose()
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeesTbl::getData:", ex)
            End Try

            Return mLastEmployeeData

        End Function


        ''' <summary>
        ''' Obtiene el nombre del empleado dado el ID
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getName(ByVal IDEmployee As Integer) As String

            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try
                'Si el usuario ya esta en memoria no lo volvemos a buscar
                If mIDEmployee <> IDEmployee Then

                    'Buscamos los datos del empleado
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from employees where idEmployee=" + IDEmployee.ToString
                    SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                    'Si hay datos los carga
                    If SQLReader.Read() Then
                        mIDEmployee = IDEmployee
                        mName = SQLReader.GetString(COL_Name)
                    Else
                        mIDEmployee = 0
                        mName = ""
                    End If
                    SQLReader.Dispose()
                    SQLcommand.Dispose()
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeesTbl::getName:", ex)
                mName = ""
            End Try

            Return mName

        End Function

        ''' <summary>
        ''' Obtienen número de empleados registrados
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function EmployeesCount() As Integer
            Try
                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "SELECT count(*) from employees;"
                Return clsGlobal.Any2Long(SQLcommand.ExecuteScalar())

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsEmployeesTbl::EmployeesCount:", ex)
                Return -1
            End Try
        End Function

#End Region

    End Class
End Namespace