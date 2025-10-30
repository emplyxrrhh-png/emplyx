Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para periodos de acceso en terminales ZK (excepto centralitas)
    ''' </summary>
    Public Class TimeZoneZKPush2File
        Private slTimeZones As New SortedList
        Public HasChange As Boolean = False

        Public Sub Add(ByVal TimeZoneID As Integer, ByVal WeekDay As Byte, ByVal BeginTime As String, ByVal EndTime As String)
            Dim oTimeZone As New TimeZoneZKPush2
            Dim bFound As Boolean = False
            Try
                For Each oTimeZone In slTimeZones.Values
                    'Mira si ya añadí el periodo de acceso
                    If oTimeZone.ID = TimeZoneID Then
                        'Si no hay horario para el dia definido lo crea
                        If Not oTimeZone.Exist(WeekDay, Date.Parse(BeginTime), Date.Parse(EndTime)) Then
                            oTimeZone.add(WeekDay, Date.Parse(BeginTime), Date.Parse(EndTime))
                        End If
                        bFound = True
                        Exit For
                    End If
                Next

                'Si no existe el periodo de acceso, lo añado ahora
                If Not bFound Then
                    Dim oNewTimeZone As New TimeZoneZKPush2
                    oNewTimeZone.ID = TimeZoneID
                    oNewTimeZone.add(WeekDay, Date.Parse(BeginTime), Date.Parse(EndTime))
                    slTimeZones.Add(TimeZoneID, oNewTimeZone)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZoneZKPush2File::Add: Unexpected error: ", ex)
            End Try
        End Sub

        Public Sub Remove(ByVal IDAccessPeriod As Integer, ByVal WeekDay As Byte)
            Dim oTz As TimeZoneZKPush2
            For Each oTz In slTimeZones.Values
                If oTz.ID = IDAccessPeriod Then
                    oTz.Remove(WeekDay)
                End If
            Next
        End Sub

        Public Overrides Function ToString() As String
            Dim iCount As Integer
            Dim NewFile As String = ""
            For iCount = 0 To slTimeZones.Count - 1
                NewFile += slTimeZones(iCount).ToString + BCGlobal.KeyNewLine
            Next
            Return NewFile
        End Function

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncPushTimeZonesData
                                           (IDTimeZone
                                           ,IDDayOrHol
                                           ,BeginTime
                                           ,EndTime
                                           ,TerminalId
                                           ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oTimeZone As TimeZoneZKPush2 In slTimeZones.Values
                    For i As Integer = 1 To 10
                        sqlValues &= $" ({oTimeZone.ID},
                                  {i},
                                  CONVERT(SMALLDATETIME, '{oTimeZone.BeginTime(i).ToString("yyyy-MM-dd HH:mm:ss")}', 120),
                                  CONVERT(SMALLDATETIME, '{oTimeZone.EndTime(i).ToString("yyyy-MM-dd HH:mm:ss")}', 120),
                                  {terminalId},
                                  CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)),"

                        insertCount += 1

                        If insertCount > 300 Then
                            sqlValuesList.Add(New String(sqlValues))
                            sqlValues = String.Empty
                            insertCount = 0
                        End If
                    Next
                Next

                If Not sqlValues.Equals(String.Empty) Then sqlValuesList.Add(New String(sqlValues))

                If sqlValuesList.Count > 0 Then
                    For Each values As String In sqlValuesList
                        values = values.Substring(0, values.Length - 1)
                        result = result AndAlso AccessHelper.ExecuteSql(String.Concat(sqlInsert, values))
                    Next
                Else
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncPushTimeZonesData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteTimeZonesZKPush2XmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteTimeZonesZKPush2XmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZoneZKPush2File::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteTimeZonesZKPush2XmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As Boolean
            Dim sql As String = $"@DELETE# FROM TerminalsSyncPushTimeZonesData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZoneZKPush2File::DeleteEmployeesXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Function ToXml(idTimeZone As Integer, Optional idDayOrHol As Integer = 0) As String

            Dim strXml As String = ""

            Try

                Dim dsLocalData As New LocalDataSet
                Dim tbTimeZones As LocalDataSet.TimeZonesZKPush2DataTable = dsLocalData.TimeZonesZKPush2

                Dim oRow As LocalDataSet.TimeZonesZKPush2Row
                For Each oTimeZone As TimeZoneZKPush2 In slTimeZones.Values
                    If oTimeZone.ID = idTimeZone Then
                        For i As Integer = 1 To 10
                            oRow = tbTimeZones.NewTimeZonesZKPush2Row
                            oRow.IDTimeZone = oTimeZone.ID
                            oRow.IdDayOrHol = i
                            oRow.BeginTime = oTimeZone.BeginTime(i)
                            oRow.EndTime = oTimeZone.EndTime(i)
                            tbTimeZones.Rows.Add(oRow)
                        Next
                        Exit For
                    End If
                Next

                Dim oStrm As New MemoryStream
                Dim bXml() As Byte

                tbTimeZones.WriteXml(oStrm)
                ReDim bXml(oStrm.Length - 1)
                oStrm.Position = 0
                oStrm.Read(bXml, 0, oStrm.Length)
                oStrm.Close()

                strXml = System.Text.Encoding.UTF8.GetString(bXml, 0, bXml.Length)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZoneZKPush2File::ToXml: Unexpected error: ", Ex)
            End Try
            Return strXml

        End Function

        Public Function CompareXmlFromDatabase(ByVal terminalId As Integer, ByRef oBroadcasterManager As BroadcasterManager)
            'Dim HasChange As Boolean = False

            Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# IDTimeZone
                                          ,IDDayOrHol as IdDayOrHol
                                          ,BeginTime
                                          ,EndTime
                                      FROM TerminalsSyncPushTimeZonesData WITH (NOLOCK) WHERE TerminalId = {terminalId} FOR XML PATH('TimeZonesZKPush2'), ROOT('LocalDataSet'))
                                    @select# @xmldata as returnXml"

            Try
                Dim dsLocalData As New LocalDataSet
                Dim oTblXML As LocalDataSet.TimeZonesZKPush2DataTable = dsLocalData.TimeZonesZKPush2
                Dim oRowXML As LocalDataSet.TimeZonesZKPush2Row
                Dim oTimeZoneDB As TimeZoneZKPush2
                Dim tbRet As Object

                tbRet = AccessHelper.ExecuteScalar(sql)

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><TimeZonesZKPush2 /></LocalDataSet>") Then
                        tbRet = tbRet.Replace("<LocalDataSet>", "<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd"">")

                        Dim memoryStream = New MemoryStream()
                        Dim streamWriter = New StreamWriter(memoryStream, System.Text.Encoding.UTF8)
                        streamWriter.Write(tbRet)
                        streamWriter.Flush()
                        memoryStream.Position = 0

                        oTblXML.ReadXml(memoryStream)
                    End If

                    For Each oRowXML In oTblXML.Rows
                        If slTimeZones.ContainsKey(oRowXML.IDTimeZone) Then
                            oTimeZoneDB = CType(slTimeZones.Item(oRowXML.IDTimeZone), TimeZoneZKPush2)
                            If oTimeZoneDB.ID = oRowXML.IDTimeZone Then
                                ' Estoy en el día que toca. Comparo las tres franjas
                                If oTimeZoneDB.BeginTime(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.BeginTime.TimeOfDay OrElse oTimeZoneDB.EndTime(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.EndTime.TimeOfDay Then
                                    ' Creo tarea para actualizar toda la definición del timezone
                                    oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addtimezone, 0, , oRowXML.IDTimeZone, , Me.ToXml(oRowXML.IDTimeZone))
                                    HasChange = True
                                End If
                            Else
                                'Sigo...
                            End If
                        Else
                            'Si no existe actualmente, elimino el periodo de acceso
                            oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.deltimezone, 0, , oRowXML.IDTimeZone)
                            HasChange = True
                        End If
                    Next
                Else
                    ' Si no existe no genero tarea de borrado de timezones del terminal, porque de todos modos volveré a definir las 50 disponibles.
                End If

                'Buscamos todas las timezones que no existían antes
                Dim bFound As Boolean = False
                For Each oTimeZoneDB In slTimeZones.Values
                    For Each oRow As LocalDataSet.TimeZonesZKPush2Row In oTblXML
                        If oRow.IDTimeZone = oTimeZoneDB.ID Then
                            bFound = True
                        End If
                    Next
                    If Not bFound Then
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addtimezone, 0, , oTimeZoneDB.ID, 0, Me.ToXml(oTimeZoneDB.ID))
                        HasChange = True
                    End If
                    bFound = False
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"TimeZoneZKPush2File::CompareXml: Unexpected error generating sync tasks for terminal {terminalId}: ", ex)
            End Try
            Return HasChange
        End Function

    End Class

End Namespace