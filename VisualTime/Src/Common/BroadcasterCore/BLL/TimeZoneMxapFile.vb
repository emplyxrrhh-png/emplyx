Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para periodos de acceso en centralitas
    ''' </summary>
    Public Class TimeZoneMxapFile

        Private slTimeZones As New SortedList
        Public HasChange As Boolean = False

        Public ReadOnly Property TimeZones As SortedList
            Get
                Return slTimeZones
            End Get
        End Property

        Public Sub Add(ByVal TimeZoneID As Integer, ByVal WeekDay As Byte, ByVal BeginTime As String, ByVal EndTime As String)
            Dim oTimeZone As New TimeZoneMxap
            Dim bFound As Boolean = False
            Try
                For Each oTimeZone In slTimeZones.Values
                    'Mira si ya añadí el periodo de acceso
                    If oTimeZone.ID = TimeZoneID Then
                        'Si no hay horario para el dia definido lo crea
                        If oTimeZone.SlotsInDay(WeekDay) < 3 Then
                            If Not oTimeZone.Exist(WeekDay, Date.Parse(BeginTime), Date.Parse(EndTime)) Then
                                oTimeZone.add(WeekDay, Date.Parse(BeginTime), Date.Parse(EndTime))
                            End If
                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "TimeZonesMxapFile::ToXml: Not enough time slots: " + oTimeZone.ToString)
                        End If
                        bFound = True
                        Exit For
                    End If
                Next

                'Si no existe el periodo de acceso, lo añado ahora
                If Not bFound Then
                    Dim oNewTimeZone As New TimeZoneMxap
                    oNewTimeZone.ID = TimeZoneID
                    oNewTimeZone.add(WeekDay, Date.Parse(BeginTime), Date.Parse(EndTime))
                    slTimeZones.Add(TimeZoneID, oNewTimeZone)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZoneMxapFile::Add: Unexpected error: ", ex)
            End Try
        End Sub

        Public Sub Remove(ByVal IDAccessPeriod As Integer, ByVal WeekDay As Byte)
            ' Borro todos los slots de un día
            Dim oTz As TimeZoneMxap
            For Each oTz In slTimeZones.Values
                If oTz.ID = IDAccessPeriod Then
                    oTz.Remove(IDAccessPeriod, WeekDay)
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

        Public Function ToXml(idTimeZone As Integer, Optional idDayOrHol As Integer = 0) As String

            Dim strXml As String = ""

            Try

                Dim dsLocalData As New LocalDataSet
                Dim tbTimeZones As LocalDataSet.TimeZonesMxaDataTable = dsLocalData.TimeZonesMxa

                Dim oRow As LocalDataSet.TimeZonesMxaRow
                For Each oTimeZone As TimeZoneMxap In slTimeZones.Values
                    If oTimeZone.ID = idTimeZone Then
                        If idDayOrHol = 0 Then
                            For i As Integer = 1 To 10
                                oRow = tbTimeZones.NewTimeZonesMxaRow
                                oRow.IDTimeZone = oTimeZone.ID
                                oRow.IdDayOrHol = i
                                oRow.BeginTime1 = oTimeZone.BeginTime1(i)
                                oRow.EndTime1 = oTimeZone.EndTime1(i)
                                oRow.BeginTime2 = oTimeZone.BeginTime2(i)
                                oRow.EndTime2 = oTimeZone.EndTime2(i)
                                oRow.BeginTime3 = oTimeZone.BeginTime3(i)
                                oRow.EndTime3 = oTimeZone.EndTime3(i)
                                tbTimeZones.Rows.Add(oRow)
                            Next
                        Else
                            oRow = tbTimeZones.NewTimeZonesMxaRow
                            oRow.IDTimeZone = oTimeZone.ID
                            oRow.IdDayOrHol = idDayOrHol
                            oRow.BeginTime1 = oTimeZone.BeginTime1(idDayOrHol)
                            oRow.EndTime1 = oTimeZone.EndTime1(idDayOrHol)
                            oRow.BeginTime2 = oTimeZone.BeginTime2(idDayOrHol)
                            oRow.EndTime2 = oTimeZone.EndTime2(idDayOrHol)
                            oRow.BeginTime3 = oTimeZone.BeginTime3(idDayOrHol)
                            oRow.EndTime3 = oTimeZone.EndTime3(idDayOrHol)
                            tbTimeZones.Rows.Add(oRow)
                        End If
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
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZonesMxapFile::ToXml: Unexpected error: ", Ex)
            End Try

            Return strXml

        End Function

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncTimeZonesInbioData (
                                                                IDTimeZone
                                                                ,IdDayOrHol
                                                                ,BeginTime1
                                                                ,EndTime1
                                                                ,BeginTime2
                                                                ,EndTime2
                                                                ,BeginTime3
                                                                ,EndTime3
                                                                ,TerminalId
                                                                ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oTimeZone As TimeZoneMxap In slTimeZones.Values
                    For i As Integer = 1 To 10
                        sqlValues &= $" ({oTimeZone.ID},
                                   '{i}',
                                   CONVERT(SMALLDATETIME, '{oTimeZone.BeginTime1(i).ToString("yyyy-MM-dd HH:mm")}', 120),
                                   CONVERT(SMALLDATETIME, '{oTimeZone.EndTime1(i).ToString("yyyy-MM-dd HH:mm")}', 120),
                                   CONVERT(SMALLDATETIME, '{oTimeZone.BeginTime2(i).ToString("yyyy-MM-dd HH:mm")}', 120),
                                   CONVERT(SMALLDATETIME, '{oTimeZone.EndTime2(i).ToString("yyyy-MM-dd HH:mm")}', 120),
                                   CONVERT(SMALLDATETIME, '{oTimeZone.BeginTime3(i).ToString("yyyy-MM-dd HH:mm")}', 120),
                                   CONVERT(SMALLDATETIME, '{oTimeZone.EndTime3(i).ToString("yyyy-MM-dd HH:mm")}', 120),
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
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncTimeZonesInbioData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteTimeZoneXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteTimeZoneXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZonesMxapFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteTimeZoneXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As String
            Dim sql As String = $"@DELETE# FROM TerminalsSyncTimeZonesInbioData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZonesMxapFile::TerminalsSyncTimeZonesInbioData: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Sub CompareXmlFromDatabase(ByVal terminalId As Integer, ByRef oBroadcasterManager As BroadcasterManager)

            Try
                Dim dsLocalData As New LocalDataSet
                Dim oTblXML As LocalDataSet.TimeZonesMxaDataTable = dsLocalData.TimeZonesMxa
                Dim oRowXML As LocalDataSet.TimeZonesMxaRow
                Dim oTimeZoneDB As TimeZoneMxap
                Dim tbRet As Object

                'Dim HasChange As Boolean = False
                Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# IDTimeZone, IdDayOrHol, BeginTime1, EndTime1, BeginTime2, EndTime2, BeginTime3, EndTime3
                                    FROM TerminalsSyncTimeZonesInbioData WITH (NOLOCK) WHERE TerminalId = {terminalId} FOR XML PATH('TimeZonesMxa'), ROOT('LocalDataSet'))
                                    @SELECT# @xmldata as returnXml"

                tbRet = AccessHelper.ExecuteScalar(sql)

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><TimeZonesMxa /></LocalDataSet>") Then
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
                            oTimeZoneDB = CType(slTimeZones.Item(oRowXML.IDTimeZone), TimeZoneMxap)
                            If oTimeZoneDB.ID = oRowXML.IDTimeZone Then
                                ' Estoy en el día que toda. Comparo las tres franjas
                                If oTimeZoneDB.BeginTime1(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.BeginTime1.TimeOfDay OrElse oTimeZoneDB.EndTime1(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.EndTime1.TimeOfDay _
                                    OrElse
                                    oTimeZoneDB.BeginTime2(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.BeginTime2.TimeOfDay OrElse oTimeZoneDB.EndTime2(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.EndTime2.TimeOfDay _
                                    OrElse
                                    oTimeZoneDB.BeginTime3(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.BeginTime3.TimeOfDay OrElse oTimeZoneDB.EndTime3(oRowXML.IdDayOrHol).TimeOfDay <> oRowXML.EndTime3.TimeOfDay Then
                                    ' Creo tarea para actualizar toda la definición del timezone
                                    oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addtimezone, 0, , oRowXML.IDTimeZone, , Me.ToXml(oRowXML.IDTimeZone))
                                    Me.HasChange = True
                                End If
                            Else
                                'Sigo...
                            End If
                        Else
                            'Si no existe actualmente, elimino el periodo de acceso
                            oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.deltimezone, 0, , oRowXML.IDTimeZone)
                            Me.HasChange = True
                        End If
                    Next
                Else
                    'Si no exitia el anterior fichero borra todos los empleados
                    oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delalltimezones, 0, , 0)
                    Me.HasChange = True
                End If

                'Buscamos todas las timezones que no existían antes
                Dim bFound As Boolean = False
                For Each oTimeZoneDB In slTimeZones.Values
                    For Each oRow As LocalDataSet.TimeZonesMxaRow In oTblXML
                        If oRow.IDTimeZone = oTimeZoneDB.ID Then
                            bFound = True
                        End If
                    Next
                    If Not bFound Then
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addtimezone, 0, , oTimeZoneDB.ID, 0, Me.ToXml(oTimeZoneDB.ID))
                        Me.HasChange = True
                    End If
                    bFound = False
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TimeZonesMxapFile::CompareXmlFromDatabase: Unexpected error: ", ex)
            End Try
        End Sub

    End Class

End Namespace