Imports system.Data.SQLite

Namespace Database

    Public Class clsDocumentsTbl

        Private Const COL_idEmployee = 0
        Private Const COL_idReader = 1
        Private Const COL_Document = 2
        Private Const COL_Company = 3
        Private Const COL_BeginDate = 4
        Private Const COL_EndDate = 5
        Private Const COL_DenyAccess = 6

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
                'Creamos la tabla de documentos
                Dim sSQL As String = "CREATE TABLE IF NOT EXISTS Documents ("
                sSQL += "idEmployee INTEGER, idReader INTEGER"
                sSQL += ", Name TEXT, Company TEXT, BeginDate DATETIME"
                sSQL += ", EndDate DATETIME, DenyAccess BOOLEAN DEFAULT 0);"
                SQLcommand.CommandText = sSQL
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsDocumentsTbl::CreateTable:documents table created.")

                SQLcommand.Dispose()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDocumentsTbl::CreateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM documents;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsDocumentsTbl::ClearTable:documents table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDocumentsTbl::ClearTable:", ex)
            End Try
        End Sub

        Public Function addRow(ByVal _IDEmployee As Integer, ByVal _IDReader As Byte, ByVal _Name As String, ByVal _Company As String, ByVal _BeginDate As DateTime, ByVal _EndDate As DateTime, ByVal _DenyAccess As Boolean) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                Dim sSQL As String = "INSERT INTO documents(idEmployee, idReader, Name, Company, BeginDate, EndDate, DenyAccess)"
                sSQL += "VALUES(" + clsGlobal.Any2SQL(_IDEmployee) + ", " + clsGlobal.Any2SQL(_IDReader) + ", " + clsGlobal.Any2SQL(_Name) + ", " + clsGlobal.Any2SQL(_Company) + ", "
                sSQL += clsGlobal.Any2SQL(_BeginDate) + ", " + clsGlobal.Any2SQL(_EndDate) + ", " + clsGlobal.Any2SQL(_DenyAccess) + ")"
                SQLcommand.CommandText = sSQL
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsDocumentsTbl::addRow:New row(" + _IDEmployee.ToString + "," + _IDReader.ToString + "," + _Name.ToString + "," + _Company.ToString + "," + clsGlobal.Any2SQL(_BeginDate) + "," + clsGlobal.Any2SQL(_EndDate) + "," + _DenyAccess.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDocumentsTbl::addRow:", ex)
                Return False
            End Try

        End Function

        ''' <summary>
        ''' Borra registro de la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function delRow(ByVal IDEmployee As Integer) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Borramos todos los documentos del empleado
                SQLcommand.CommandText = "DELETE FROM documents WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee) + ";"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsDocumentsTbl::delRow:delete row(" + IDEmployee.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDocumentsTbl::delRow:", ex)
                Return False
            End Try

        End Function

        Public Function getEmployeeDocuments(ByVal IDEmployee As Integer) As List(Of clsDocumentData)
            Dim lst As New List(Of clsDocumentData)
            Try

                Dim SQLcommand As New SQLiteCommand("Select * from documents WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee) + " order by DenyAccess,EndDate ;", mConn)
                Dim sqReader As SQLiteDataReader = SQLcommand.ExecuteReader

                lst.Clear()
                Dim oDocument As clsDocumentData
                While sqReader.Read
                    oDocument = New clsDocumentData
                    oDocument.IDEmployee = sqReader.GetInt32(COL_idEmployee)
                    oDocument.IDReader = sqReader.GetInt32(COL_idReader)
                    oDocument.Name = sqReader.GetString(COL_Document)
                    oDocument.Company = sqReader.GetString(COL_Company)
                    oDocument.BeginDate = sqReader.GetDateTime(COL_BeginDate)
                    oDocument.EndDate = sqReader.GetDateTime(COL_EndDate)
                    oDocument.DenyAccess = sqReader.GetBoolean(COL_DenyAccess)
                    oDocument.CheckData()
                    lst.Add(oDocument)

                End While


            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDocumentsTbl::getEmployeeDocuments:", ex)
            End Try
            Return lst
        End Function

    End Class

    Public Class clsDocumentData
        Public IDEmployee As Integer
        Public IDReader As Integer = 0
        Public Name As String = ""
        Public Company As String = ""
        Public BeginDate As DateTime = clsGlobal.NULLDATETIME
        Public EndDate As DateTime = clsGlobal.NULLDATETIME
        Public DenyAccess As Boolean = False
        Public Valid As Boolean = False

        Public Sub CheckData()
            Try
                Valid = False
                If Not (BeginDate = clsGlobal.NULLDATETIME Or EndDate = clsGlobal.NULLDATETIME) Then
                    If BeginDate < Now And EndDate.Date.AddDays(1) > Now Then
                        'Es un documento dentro de fechas
                        Valid = True
                    End If
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDocumentData::CheckData:", ex)
            End Try
        End Sub
    End Class
End Namespace
