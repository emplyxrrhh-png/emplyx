Imports system.Data.SQLite


Namespace Database
    Public Class clsCardsTbl

        Private Const COL_idEmployee = 0
        Private Const COL_Card = 1

        Private mIDEmployee As Integer
        Private mCard As String

        Private mConn As SQLiteConnection

        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::New:", ex)
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
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS cards (idEmployee INTEGER PRIMARY KEY, card TEXT);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCardsTbl::CreateTable:cards table created.")

                'Creamos indice de tarjetas
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS idxcards ON cards(card);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCardsTbl::CreateTable:cards index created.")

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::CreateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM cards;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mCard = ""
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCardsTbl::ClearTable:cards table empty.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::ClearTable:", ex)
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
                SQLcommand.CommandText = "REINDEX cards;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mCard = ""
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCardsTbl::ReindexTable:cards index has updated.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::ReindexTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Añade registro a la tabla
        ''' </summary>
        ''' <param name="IDEmployee"></param>
        ''' <param name="Card"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function addRow(ByVal IDEmployee As Integer, ByVal Card As String) As Boolean
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "INSERT OR REPLACE INTO cards(idEmployee,card) VALUES(" + clsGlobal.Any2SQL(IDEmployee) + "," + clsGlobal.Any2SQL(Card) + ");"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mCard = ""
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCardsTbl::addRow:New row(" + IDEmployee.ToString + "," + Card + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::addRow:", ex)
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
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "DELETE FROM cards WHERE idEmployee=" + clsGlobal.Any2SQL(IDEmployee) + ";"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                mCard = ""
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsCardsTbl::delRow:delete row(" + IDEmployee.ToString + ").")
                Return True
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::delRow:", ex)
                Return False
            End Try

        End Function

#End Region

#Region "Consultas"

        ''' <summary>
        ''' Obtien el ID de empleado dada una tarjeta
        ''' </summary>
        ''' <param name="Card"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getEmployee(ByVal Card As String) As Integer
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Try
                'Si llega una tarjeta vacia no la buscamos
                If Card = "" Or Card = "0" Then
                    mIDEmployee = 0
                    mCard = ""
                    Return mIDEmployee
                End If

                'Si el usuario ya esta en memoria no lo volvemos a buscar
                If mCard <> Card Then

                    'Buscamos los datos del empleado
                    SQLcommand = mConn.CreateCommand
                    SQLcommand.CommandText = "select * from cards where card LIKE '%" + Card.TrimStart("0") + "'"
                    SQLReader = SQLcommand.ExecuteReader()

                    'Si hay datos los carga
                    If SQLReader.Read() Then
                        mIDEmployee = SQLReader.GetInt16(COL_idEmployee)
                        mCard = Card
                        If SQLReader.Read() Then
                            clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roWarning, "clsCardsTbl::getEmployee::It has been found repeated card.(" + Card + ")")
                            mIDEmployee = -1
                        End If
                        SQLReader.Dispose()
                        SQLcommand.Dispose()
                        Return mIDEmployee
                    Else
                        mIDEmployee = 0
                        mCard = ""
                        SQLReader.Dispose()
                        SQLcommand.Dispose()
                        Return mIDEmployee
                    End If
                Else
                    Return mIDEmployee
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::getEmployee:", ex)
            End Try

        End Function
        ''' <summary>
        ''' Obtienen número de tarjetas registrado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CardsCount() As Integer
            Try
                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "SELECT count(*) from cards;"
                Return clsGlobal.Any2Long(SQLcommand.ExecuteScalar())

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsCardsTbl::CardsCount:", ex)
                Return -1
            End Try
        End Function

#End Region

    End Class

End Namespace