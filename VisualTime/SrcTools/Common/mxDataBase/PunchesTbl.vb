Imports System.Data.SQLite
Namespace Database


    Public Class clsPunchesTbl

        Private mConn As SQLiteConnection
        Public mPunchPhotoTbl As clsPunchesPhoto

        Private Const COL_PunchTime = 0
        Private Const COL_Reader = 1
        Private Const COL_Data = 2
        Private Const COL_Action = 3
        Private Const COL_Incidence = 4
        Private Const COL_Photo = 5
        Private Const COL_Online = 6
        Private Const COL_Sended = 7
        Private Const COL_Other = 8
        'Private Const COL_DEBUG = 9
        Public COL_ROWID = 9

        Private mLastIDPunch As Long

        Public Sub New(ByRef Conn As SQLiteConnection)
            Try
                mConn = Conn
                mPunchPhotoTbl = New clsPunchesPhoto()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::New:", ex)
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
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS punches (PunchTime DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP ,Reader INTEGER,Data TEXT,Action TEXT,Incidence INTEGER DEFAULT 0 ,Photo BLOB, online BOOLEAN DEFAULT 0, Sended BOOLEAN DEFAULT 0 );"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::CreateTable:punches table created.")

                'Creamos indice de fecha
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS idxpunches ON punches(PunchTime DESC);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::CreateTable:punches index created.")

                'Creamos la tabla de fichajes para control de repetidos
                SQLcommand.CommandText = "CREATE TABLE IF NOT EXISTS punches_temp (PunchTime DATETIME NOT NULL  DEFAULT CURRENT_TIMESTAMP ,Data TEXT);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::CreateTable:punches_temp table created.")

                'Creamos indice
                SQLcommand.CommandText = "CREATE INDEX IF NOT EXISTS idxpunchestemp ON punches_temp(PunchTime DESC, Data ASC);"
                If SQLcommand.ExecuteNonQuery() > 0 Then clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::CreateTable:punches_temp index created.")

                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::CreateTable:", ex)
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
                SQLcommand.CommandText = "DELETE FROM punches;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::ClearTable:punches table empty.")

                mPunchPhotoTbl.ClearTable()

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::ClearTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Reindexa la tabla
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ReindexTable()
            Try
                debug.WriteLine("clsPunchesTbl::ReindexTable::Reindexing table.")

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                'Creamos la tabla de tarjetas
                SQLcommand.CommandText = "REINDEX punches;"
                SQLcommand.ExecuteNonQuery()
                SQLcommand.Dispose()
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::ReindexTable:employees index has updated.")

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::ReindexTable:", ex)
            End Try
        End Sub

        ''' <summary>
        ''' Guarda un marcaje en la base de datos
        ''' </summary>
        ''' <param name="oEmployeePunch"></param>
        ''' <param name="Online"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SavePunch(ByVal oEmployeePunch As Business.clsEmployeePunch, Optional ByVal Online As Boolean = False) As Long
            Dim sSQL As String = ""
            Try
                debug.WriteLine("clsPunchesTbl::SavePunch::Saving punch.")

                sSQL = "INSERT OR REPLACE INTO punches (PunchTime,Reader,Data,Action,Incidence,online)"
                sSQL += "VALUES("
                sSQL += "'" + clsGlobal.date2SQLDate(oEmployeePunch.PunchTime) + "', "
                sSQL += CInt(oEmployeePunch.Reader).ToString + ", "
                If oEmployeePunch.Reader = clsInputs.eTypeInput.Card Then
                    sSQL += "'" + clsCardReader.ConvertCard(oEmployeePunch.Data) + "', "
                Else
                    sSQL += "'" + oEmployeePunch.Data + "', "
                End If
                sSQL += "'" + oEmployeePunch.Action + "', "
                sSQL += oEmployeePunch.Incidence.ToString + ", "
                sSQL += IIf(Online, "1", "0")
                sSQL += ")"

                Dim SQLcommand As SQLiteCommand
                mLastIDPunch = 0
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = sSQL

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::SaveOfflinePunch:Puch saved:" + sSQL)


                    Try
                        'Obtenemos el id de la fila insertada
                        SQLcommand.CommandText = "SELECT last_insert_rowid()"
                        mLastIDPunch = clsGlobal.Any2Long(SQLcommand.ExecuteScalar)

                    Catch ex As Exception
                        clsSystemControl.ControlException(ex)
                        clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::SaveOfflinePunch:Error getting rowid", ex)
                    End Try

                    If oEmployeePunch.HasPhoto Then
                        mPunchPhotoTbl.SavePunchPhoto(mLastIDPunch, oEmployeePunch.Photo)
                    End If
                    'mLastIDPunch = getLastIDPunch()
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing
                Return mLastIDPunch

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::SaveOfflinePunch:line(" + sSQL + ")", ex)
                Return -1
            End Try
        End Function

        ''' <summary>
        ''' Guarda un marcaje en la tabla temporal para detectar fichajes repetidos
        ''' </summary>
        ''' <param name="oEmployeePunch"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function SavePunchForRepeatedCheck(ByVal oEmployeePunch As Business.clsEmployeePunch) As Boolean
            Dim sSQL As String = ""
            Dim bRes As Boolean = False
            Try
                Debug.WriteLine("clsPunchesTbl::SavePunchForRepeatedCheck::Saving punch.")

                sSQL = "INSERT OR REPLACE INTO punches_temp (PunchTime,Data)"
                sSQL += "VALUES("
                sSQL += "'" + clsGlobal.date2SQLDate(oEmployeePunch.PunchTime) + "', "
                sSQL += "'" + oEmployeePunch.Data + "') "

                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = sSQL

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::SavePunchForRepeatedCheck:Puch saved:" + sSQL)
                    bRes = True
                Else
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::SavePunchForRepeatedCheck:Error saving puch for repeated control" + sSQL)
                    bRes = False
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing
                Return bRes

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::SavePunchForRepeatedCheck:line(" + sSQL + ")", ex)
                Return False
            End Try
        End Function

        ''' <summary>
        ''' Verifica si el empleado realizó un fichaje hace menos de un minuto
        ''' </summary>
        ''' <param name="sData"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function CheckIfPunchIsRepeated(ByVal sData As String) As Boolean
            Dim sSQL As String = ""
            Dim bRes As Boolean = False
            Dim PunchTimeLimit As DateTime
            Try
                Debug.WriteLine("clsPunchesTbl::SavePunchForRepeatedCheck::Saving punch.")

                PunchTimeLimit = Now.AddSeconds(-1 * clsSysConfig.PreventRepeatedOfflinePunchesPeriod)

                sSQL = "SELECT COUNT(*) FROM PUNCHES_TEMP WHERE Data = '" + sData + "' and PunchTime > Datetime('" + PunchTimeLimit.ToString("yyyy-MM-dd HH:mm:ss") + "');"

                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = sSQL

                If clsGlobal.Any2Long(SQLcommand.ExecuteScalar) > 0 Then
                    bRes = True
                Else
                    bRes = False
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing

                Return bRes

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::CheckIfPunchIsRepeated:line(" + sSQL + ")", ex)
                ' Ante un error permito fichar
                Return False
            End Try
        End Function


        ''' <summary>
        ''' Obtiene un fichaje offline
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function getOfflinePunch() As Business.clsEmployeePunch
            Try
                debug.WriteLine("clsPunchesTbl::getOfflinePunch::Getting punch.")

                Dim SQLcommand As SQLiteCommand
                Dim SQLReader As SQLiteDataReader

                'Buscamos los datos del empleado
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "SELECT *, rowid FROM punches where online=0 and sended=0 order by PunchTime limit 1"

                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.SingleRow)

                'Si hay datos los carga
                If SQLReader.Read() Then
                    'PunchTime,Reader,Action,Incidence,PhotoName
                    Dim oPunch As Business.clsEmployeePunch
                    oPunch = New Business.clsEmployeePunch()
                    oPunch.PunchTime = clsGlobal.Any2Date(SQLReader.GetDateTime(COL_PunchTime))
                    oPunch.Reader = clsGlobal.Any2Long(SQLReader.GetInt16(COL_Reader))
                    oPunch.Data = clsGlobal.Any2String(SQLReader.GetString(COL_Data))
                    oPunch.Action = clsGlobal.Any2String(SQLReader.GetString(COL_Action))
                    oPunch.Incidence = clsGlobal.Any2Long(SQLReader.GetInt16(COL_Incidence))
                    oPunch.PunchID = clsGlobal.Any2Long(SQLReader.GetValue(COL_ROWID))
                    oPunch.Photo = clsGlobal.Any2Bytes(SQLReader.GetValue(COL_Photo))
                    Debug.WriteLine("clsPunchesTbl::getOfflinePunch::Get punch.")
                    If oPunch.Photo.Length = 0 Then
                        oPunch.Photo = mPunchPhotoTbl.GetPunchPhoto(oPunch.PunchID)
                    End If
                    debug.WriteLine("clsPunchesTbl::getOfflinePunch::return punch.")
                    Return oPunch
                Else
                    Return Nothing
                End If

                SQLReader.Dispose()
                SQLcommand.Dispose()
                SQLReader = Nothing
                SQLcommand = Nothing
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::getOfflinePunch:", ex)
                Return Nothing
            End Try

        End Function

        ''' <summary>
        ''' Marca los fichajes como enviados
        ''' </summary>
        ''' <param name="IDRow"></param>
        ''' <remarks></remarks>
        Public Sub MarkAsSended(ByVal IDRow As Integer)
            Try

                debug.WriteLine("clsPunchesTbl::MarkAsSended::starting...")
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "update punches set sended=1 where ROWID=" + IDRow.ToString

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::MarkAsSended:Mark line " + IDRow.ToString + " as sended.")
                    mPunchPhotoTbl.Mark4Delete(IDRow)
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::MarkAsSended:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra los marcajes enviados
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub DeleteSended()
            Try
                debug.WriteLine("clsPunchesTbl::DeleteSended::starting...")

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "DELETE FROM punches where sended=1"

                If SQLcommand.ExecuteNonQuery() > 0 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::DeleteSended:Delete sended lines")
                    mPunchPhotoTbl.DeleteSended()
                End If

                SQLcommand.Dispose()
                SQLcommand = Nothing
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::DeleteSended:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra un marcaje de la base de datos
        ''' </summary>
        ''' <param name="PunchID"></param>
        ''' <remarks></remarks>
        Public Sub DeletePunch(ByVal PunchID As Long)
            Try
                debug.WriteLine("clsPunchesTbl::DeletePunch::starting...")

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "DELETE FROM punches where ROWID=" + PunchID.ToString

                If SQLcommand.ExecuteNonQuery() > 0 Then
                    debug.WriteLine("clsPunchesTbl::DeletePunch:Deleted.")
                    mPunchPhotoTbl.DelPunchPhoto(PunchID)
                End If
                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::DeletePunch:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Actualiza la indicencia de un marcaje (para offline)
        ''' </summary>
        ''' <param name="PunchID"></param>
        ''' <param name="Incidence"></param>
        ''' <remarks></remarks>
        Public Sub UpdatePunchIncidence(ByVal PunchID As Long, ByVal Incidence As Integer)
            Try

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "update punches set Incidence=" + Incidence.ToString + " where ROWID=" + PunchID.ToString

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::UpdatePunchIncidence:Updated.")
                End If
                SQLcommand.Dispose()
                SQLcommand = Nothing

            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::UpdatePunchIncidence:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Desmarca los marcajes que esten como enviados
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ResetSended()
            Try
                debug.WriteLine("clsPunchesTbl::ResetSended::starting.")

                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "update punches set sended=0 where sended=1"

                If SQLcommand.ExecuteNonQuery() > 0 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::ResetSended:Reset sended lines")
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::ResetSended:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Borra los fichajes que se guardaron para el control de fichajes repetidos
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub ResetPunchesForRepeatedCheck()
            Try
                Dim sSQL As String
                Dim SQLcommand As SQLiteCommand
                Dim PunchTimeLimit As DateTime

                Debug.WriteLine("clsPunchesTbl::ResetPunchesForRepeatedCheck::starting.")

                PunchTimeLimit = Now.AddSeconds(-2 * clsSysConfig.PreventRepeatedOfflinePunchesPeriod)

                sSQL = "DELETE FROM PUNCHES_TEMP WHERE PunchTime < Datetime('" + PunchTimeLimit.ToString("yyyy-MM-dd HH:mm:ss") + "');"

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = sSQL

                If SQLcommand.ExecuteNonQuery() > 0 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::ResetPunchesForRepeatedCheck:Reset temp punches for repeated check")
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::ResetPunchesForRepeatedCheck:", ex)
            End Try

        End Sub



        ''' <summary>
        ''' Desmarca los fichajes que se han enviado online para ser procesados cuando se vuelva a conectar
        ''' </summary>
        ''' <remarks></remarks>
        Public Sub UncheckOnlineSended()
            Try

                debug.WriteLine("clsPunchesTbl::UncheckOnlineSended::starting...")
                Dim SQLcommand As SQLiteCommand

                SQLcommand = mConn.CreateCommand
                SQLcommand.CommandText = "update punches set online=0 where online=1"

                If SQLcommand.ExecuteNonQuery() = 1 Then
                    clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roDebug, "clsPunchesTbl::UncheckOnlineSended:Reset sended lines")
                End If
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::UncheckOnlineSended:", ex)
            End Try

        End Sub

        ''' <summary>
        ''' Obtienen número de fichajes registrados
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function PunchesCount() As Integer
            Try
                Dim SQLcommand As SQLiteCommand
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "SELECT count(*) from punches;"
                Return clsGlobal.Any2Long(SQLcommand.ExecuteScalar())

                SQLcommand.Dispose()
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::PunchesCount:", ex)
                Return -1
            End Try
        End Function

        Public Function PunchesColumnsCount() As Integer
            Dim iColumns As Integer = 0
            Try
                Dim SQLcommand As SQLiteCommand
                Dim SQLReader As SQLiteDataReader


                'Buscamos los datos del empleado
                SQLcommand = mConn.CreateCommand

                SQLcommand.CommandText = "pragma table_info('punches');"

                SQLReader = SQLcommand.ExecuteReader(Data.CommandBehavior.Default)

                'Si hay datos los carga
                While SQLReader.Read()
                    iColumns += 1
                End While

                SQLReader.Dispose()
                SQLcommand.Dispose()
                SQLReader = Nothing
                SQLcommand = Nothing
            Catch ex As Exception
                clsSystemControl.ControlException(ex)
                clsGlobal.oLog.logMessage(Robotics.VTBase.roLog.EventType.roError, "clsPunchesTbl::getOfflinePunch:", ex)
                Return 0
            End Try
            Return iColumns
        End Function
    End Class
End Namespace