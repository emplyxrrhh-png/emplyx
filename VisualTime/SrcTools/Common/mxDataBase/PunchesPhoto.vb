Imports System.Data.SQLite
Namespace Database
    Public Class clsPunchesPhoto

        Public Shared mConn As SQLiteConnection

        Private Const COL_ID = 0
        Private Const COL_Photo = 1
        Private Const COL_Sended = 2
        Private Punches As Integer = 0

        Public Sub New()
            Try
                Open()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::New:", ex)
            End Try
        End Sub


        ''' <summary>
        ''' Abre la conexión con la base de datos
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub Open()
            Try
                If mConn Is Nothing Then
                    If Not IO.File.Exists(clsSystemHelper.PathDatabaseFilePhoto) Then
                        clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "DataBases::CreateDB:Creating file...")
                    End If
                    mConn = New SQLiteConnection("Data Source=" + clsSystemHelper.PathDatabaseFilePhoto + ";")
                End If
                If mConn.State <> ConnectionState.Open Then
                    mConn.Open()
                    CreateTable()
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsDataBases::Open:", ex)
            End Try
        End Sub

        Private Sub DeleteDB()
            Try
                If Not mConn Is Nothing Then
                    mConn.Close()
                    mConn = Nothing
                    If IO.File.Exists(clsSystemHelper.PathDatabaseFilePhoto) Then
                        IO.File.Delete(clsSystemHelper.PathDatabaseFilePhoto)
                        clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesPhoto::DeleteDB:punchesphotodb deleted.")
                    End If
                    Open()
                End If
            Catch ex As Exception

            End Try
        End Sub

        ''' <summary>
        ''' Crea la tabla en la BBDD
        ''' </summary>
        ''' <remarks></remarks>
        Private Sub CreateTable()
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                'Creamos la tabla de fotos de marcajes
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS punchesphoto (ID INTEGER NOT NULL,Photo BLOB);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesPhoto::CreateTable:punchesphoto table created.")

                'Creamos la tabla de fotos enviadas
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS punchesphotosended (ID INTEGER NOT NULL);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesPhoto::CreateTable:punchesphotoSended table created.")

                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::CreateTable:", ex)
            End Try

        End Sub


        ''' <summary>
        ''' Borra los datos de la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ClearTable()
            Try

                Dim fileInf As IO.FileInfo = New IO.FileInfo(clsSystemHelper.PathDatabaseFilePhoto)
                If fileInf.Length > 1000000 Then
                    DeleteDB()
                Else
                    Open()
                    Dim SQLcommand As SQLiteCommand

                    SQLcommand = mConn.CreateCommand
                    'Creamos la tabla de photos
                    SQLcommand.CommandText = "DELETE FROM punchesphoto;"
                    SQLcommand.ExecuteNonQuery()
                    'Creamos la tabla de photos enviadas
                    SQLcommand.CommandText = "DELETE FROM punchesphotosended;"
                    SQLcommand.ExecuteNonQuery()

                    SQLcommand.Dispose()

                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesPhoto::ClearTable:punchesphoto table empty.")
                End If

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::ClearTable:", ex)
            End Try
        End Sub

        Public Function SavePunchPhoto(ByVal ID As Integer, ByVal Photo As Byte()) As Boolean
            Dim sSQL As String = ""
            Dim bRet As Boolean = False
            Try
                Debug.WriteLine("clsPunchesPhoto::SavePunchPhoto::Saving punch photo.")
                Open()
                sSQL = "INSERT OR REPLACE INTO punchesphoto (ID,Photo)"
                sSQL += "VALUES("
                sSQL += "" + ID.ToString + ", "
                sSQL += "@photo"
                sSQL += ")"

                Dim SQLcommand As SQLiteCommand
                Dim SQLparm As New SQLiteParameter("@photo", DbType.Binary)

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = sSQL
                SQLparm.Value = Photo
                SQLcommand.Parameters.Add(SQLparm)

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    bRet = True
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesPhoto::SavePunchPhoto:Puch saved:" + sSQL)
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::SaveOfflinePunch:line(" + sSQL + ")", ex)
                Return -1
            End Try
            Return bRet
        End Function

        Public Function GetPunchPhoto(ByVal ID As Integer) As Byte()
            Dim SQLcommand As SQLiteCommand
            Dim SQLReader As SQLiteDataReader
            Dim bin As Byte() = Array.CreateInstance(GetType(Byte), 0)
            Try
                Debug.WriteLine("clsPunchesPhoto::GetPunchPhoto::Getting punch photo.")
                Open()
                'Buscamos los datos del empleado
                SQLcommand = mConn.CreateCommand
                ' SQLcommand.CommandText = "SELECT *, rowid FROM punches where PunchTime=(select min(PunchTime) from punches where online=0 and sended=0) and sended=0 and online=0"
                SQLcommand.CommandText = "SELECT * FROM punchesphoto where id=" + ID.ToString + " limit 1"

                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                'Si hay datos los carga
                If SQLReader.Read() Then
                    bin = clsGlobal.Any2Bytes(SQLReader.GetValue(COL_Photo))
                    Debug.WriteLine("clsPunchesPhoto::getPunchPhoto::Get photo.")
                    SQLReader.Dispose()
                    SQLcommand.Dispose()
                End If

                SQLReader.Dispose()
                SQLcommand.Dispose()
                SQLReader = Nothing
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::GetPunchPhoto:", ex)
                Return Array.CreateInstance(GetType(Byte), 0)
            End Try
            Return bin
        End Function


        ''' <summary>
        ''' Marca los fichajes como enviados
        ''' </summary>
        ''' <param name="ID"></param>
        ''' <remarks></remarks>
        Public Sub Mark4Delete(ByVal ID As Integer)
            Try
                Debug.WriteLine("clsPunchesPhoto::Mark4Delete::Marking as delete.")

                Open()
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "INSERT OR REPLACE INTO punchesphotosended (ID) values (" + ID.ToString + ")"

                If SQLcommand.ExecuteNonQuery() > 0 Then
                    Debug.WriteLine("clsPunchesPhoto::Mark4Delete:Mark line " + ID.ToString + " as sended.")
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::Mark4Delete:", ex)
            End Try

        End Sub


        ''' <summary>
        ''' Borra los marcajes enviados
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DeleteSended(Optional ByVal force As Boolean = False)
            Try
                Debug.WriteLine("clsPunchesPhoto::DeleteSended::Deletting sended.")
                Open()
                Dim sends As Integer = 0

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand

                If Not force Then
                    SQLcommand.CommandText = "SELECT count(*) from punchesphotosended"
                    sends = clsGlobal.Any2Long(SQLcommand.ExecuteScalar())
                    Debug.WriteLine("clsPunchesPhoto::DeleteSended::Punchs markets:" + sends.ToString)
                End If

                If sends > 50 Or force Then
                    Try
                        clsDisplay.Statustext(SystemText.getText("statustext.deletephotopunch", "Procesando fichajes"))
                    Catch ex As Exception
                    End Try
                    SQLcommand.CommandText = "DELETE FROM punchesphoto where id in (select id from punchesphotosended)"
                    oa3000API.ActiveteAPIWatchDog(False)
                    If SQLcommand.ExecuteNonQuery() > 0 Then
                        clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesPhoto::DeleteSended:Delete sended lines")
                        SQLcommand.CommandText = "DELETE FROM punchesphotosended"
                        SQLcommand.ExecuteNonQuery()
                    End If
                    oa3000API.ActiveteAPIWatchDog(oa3000API.HasAPIWatchDogActive)
                    Try
                        clsDisplay.StatusText("")
                    Catch ex As Exception
                    End Try
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing

                Punches = 0
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::DeleteSended:", ex)
            End Try

        End Sub
        Public Function DelPunchPhoto(ByVal ID As Integer) As Boolean
            Dim bRet As Boolean = False
            Dim sSQL As String = ""
            Try

                Mark4Delete(ID)
                If Punches > 10 Then
                    DeleteSended()
                Else
                    Punches += 1
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesPhoto::DelPunchPhoto:(" + sSQL + ")", ex)
            End Try

        End Function
    End Class
End Namespace