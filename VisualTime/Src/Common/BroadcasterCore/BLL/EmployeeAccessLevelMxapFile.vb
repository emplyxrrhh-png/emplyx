Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para niveles de autorización de accesos en centralitas mxC y mxS
    ''' </summary>
    Public Class EmployeeAccessLevelMxapFile

        Private slAccessLevel As New SortedList
        Public HasChange As Boolean = False

        Public Sub Add(ByVal EmployeeID As Integer, ByVal TimeZoneID As Integer, ByVal Door1 As Boolean, ByVal Door2 As Boolean, ByVal Door3 As Boolean, ByVal Door4 As Boolean)
            Dim oEmployeeAccessLevel As New EmployeeAccessLevelMxap
            Try
                If Not slAccessLevel.ContainsKey(EmployeeID.ToString + "@" + TimeZoneID.ToString) Then
                    oEmployeeAccessLevel.IDEmployee = EmployeeID
                    oEmployeeAccessLevel.IDTimeZone = TimeZoneID
                    oEmployeeAccessLevel.Door1 = Door1
                    oEmployeeAccessLevel.Door2 = Door2
                    oEmployeeAccessLevel.Door3 = Door3
                    oEmployeeAccessLevel.Door4 = Door4
                    slAccessLevel.Add(EmployeeID.ToString + "@" + TimeZoneID.ToString, oEmployeeAccessLevel)
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeAccessLevelMxapFile::Add: Unexpected error: ", ex)
            End Try
        End Sub

        Public Function ToXml(idEmployee As Integer, idTimeZone As Integer) As String

            Dim strXml As String = ""

            Try

                Dim dsLocalData As New LocalDataSet
                Dim tbEmployeeAccessLevel As LocalDataSet.EmployeeAccesLevelMxaDataTable = dsLocalData.EmployeeAccesLevelMxa

                Dim oRow As LocalDataSet.EmployeeAccesLevelMxaRow
                Dim oAccessLevel As EmployeeAccessLevelMxap

                If slAccessLevel.ContainsKey(idEmployee.ToString + "@" + idTimeZone.ToString) Then
                    oAccessLevel = CType(slAccessLevel.Item(idEmployee.ToString + "@" + idTimeZone.ToString), EmployeeAccessLevelMxap)
                    oRow = tbEmployeeAccessLevel.NewEmployeeAccesLevelMxaRow
                    oRow.IDEmployee = oAccessLevel.IDEmployee
                    oRow.IdTimezone = oAccessLevel.IDTimeZone
                    oRow.Door1 = oAccessLevel.Door1
                    oRow.Door2 = oAccessLevel.Door2
                    oRow.Door3 = oAccessLevel.Door3
                    oRow.Door4 = oAccessLevel.Door4
                    tbEmployeeAccessLevel.Rows.Add(oRow)
                    Dim oStrm As New MemoryStream
                    Dim bXml() As Byte

                    tbEmployeeAccessLevel.WriteXml(oStrm)
                    ReDim bXml(oStrm.Length - 1)
                    oStrm.Position = 0
                    oStrm.Read(bXml, 0, oStrm.Length)
                    oStrm.Close()

                    strXml = System.Text.Encoding.UTF8.GetString(bXml, 0, bXml.Length)
                End If
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeAccessLevelMxapFile::ToXml: Unexpected error: ", Ex)
            End Try

            Return strXml

        End Function

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncEmployeeAccessLevelData (
                                                                 IDEmployee
                                                                ,IDTimeZone
                                                                ,Door1
                                                                ,Door2
                                                                ,Door3
                                                                ,Door4
                                                                ,TerminalId
                                                                ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oAccessLevel As EmployeeAccessLevelMxap In slAccessLevel.Values
                    sqlValues &= $" ({oAccessLevel.IDEmployee},
                               '{oAccessLevel.IDTimeZone}',
                               '{oAccessLevel.Door1}',
                               '{oAccessLevel.Door2}',
                               '{oAccessLevel.Door3}',
                               '{oAccessLevel.Door4}',
                                {terminalId},
                               CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)),"

                    insertCount += 1

                    If insertCount > 300 Then
                        sqlValuesList.Add(New String(sqlValues))
                        sqlValues = String.Empty
                        insertCount = 0
                    End If
                Next

                If Not sqlValues.Equals(String.Empty) Then sqlValuesList.Add(New String(sqlValues))

                If sqlValuesList.Count > 0 Then
                    For Each values As String In sqlValuesList
                        values = values.Substring(0, values.Length - 1)
                        result = result AndAlso AccessHelper.ExecuteSql(String.Concat(sqlInsert, values))
                    Next
                Else
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncEmployeeAccessLevelData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteEmployeeAccessLevelXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteEmployeeAccessLevelXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeAccessLevelMxapFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteEmployeeAccessLevelXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As String
            Dim sql As String = $"@DELETE# FROM TerminalsSyncEmployeeAccessLevelData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeAccessLevelMxapFile::TerminalsSyncEmployeeAccessLevelData: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Sub CompareXmlFromDatabase(ByVal terminalId As Integer, ByRef oBroadcasterManager As BroadcasterManager)
            'Dim HasChange As Boolean = False
            Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# IDEmployee, IdTimezone, Door1, Door2, Door3, Door4
                                      FROM TerminalsSyncEmployeeAccessLevelData WITH (NOLOCK) WHERE TerminalId = {terminalId} FOR XML PATH('EmployeeAccesLevelMxa'), ROOT('LocalDataSet'))
                                    @select# @xmldata as returnXml"

            Try
                Dim ds As New LocalDataSet
                Dim oTblXML As LocalDataSet.EmployeeAccesLevelMxaDataTable = ds.EmployeeAccesLevelMxa
                Dim oRowXML As LocalDataSet.EmployeeAccesLevelMxaRow
                Dim oEmployeeAccessLevel As EmployeeAccessLevelMxap
                Dim tbRet As Object

                tbRet = AccessHelper.ExecuteScalar(sql)

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><EmployeeAccesLevelMxa /></LocalDataSet>") Then
                        tbRet = tbRet.Replace("<LocalDataSet>", "<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd"">")

                        Dim memoryStream = New MemoryStream()
                        Dim streamWriter = New StreamWriter(memoryStream, System.Text.Encoding.UTF8)
                        streamWriter.Write(tbRet)
                        streamWriter.Flush()
                        memoryStream.Position = 0

                        oTblXML.ReadXml(memoryStream)
                    End If

                    For Each oRowXML In oTblXML.Rows
                        If slAccessLevel.ContainsKey(oRowXML.IDEmployee.ToString + "@" + oRowXML.IdTimezone.ToString) Then
                            oEmployeeAccessLevel = CType(slAccessLevel.Item(oRowXML.IDEmployee.ToString + "@" + oRowXML.IdTimezone.ToString), EmployeeAccessLevelMxap)
                            If oEmployeeAccessLevel.Door1 <> oRowXML.Door1 OrElse oEmployeeAccessLevel.Door2 <> oRowXML.Door2 OrElse oEmployeeAccessLevel.Door3 <> oRowXML.Door3 OrElse oEmployeeAccessLevel.Door4 <> oRowXML.Door4 Then
                                ' Ha cambiado el nivel de acceso
                                ' 1.- Elimino el que pudiera tener
                                oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel, oRowXML.IDEmployee, , oRowXML.IdTimezone)
                                ' 2.- Creo el nuevo
                                oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel, oRowXML.IDEmployee, , oRowXML.IdTimezone, , Me.ToXml(oEmployeeAccessLevel.IDEmployee, oEmployeeAccessLevel.IDTimeZone))
                                Me.HasChange = True
                            End If
                        Else
                            'Si no existe actualmente, elimino el nivel de acceso del empleado
                            oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delemployeeaccesslevel, oRowXML.IDEmployee, , oRowXML.IdTimezone)
                            Me.HasChange = True
                        End If
                    Next
                Else
                    'Si no exitian datos borra todos los niveles de acceso
                    If oBroadcasterManager.TerminalData.Type = TerminalData.eTerminalType.rxcp OrElse oBroadcasterManager.TerminalData.Type = TerminalData.eTerminalType.rxcep OrElse oBroadcasterManager.TerminalData.Type = TerminalData.eTerminalType.rx1 OrElse oBroadcasterManager.TerminalData.Type = TerminalData.eTerminalType.rxfp Then
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallemployeetimezones, 0, , 0)
                    Else
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallemployeeaccesslevel, 0, , 0)
                    End If
                    Me.HasChange = True
                End If

                'Buscamos todos los permisos de acceso que no existían antes
                Dim bFound As Boolean = False
                For Each oEmployeeAccessLevel In slAccessLevel.Values
                    For Each oRow As LocalDataSet.EmployeeAccesLevelMxaRow In oTblXML
                        If oRow.IDEmployee = oEmployeeAccessLevel.IDEmployee AndAlso oRow.IdTimezone = oEmployeeAccessLevel.IDTimeZone Then
                            bFound = True
                        End If
                    Next
                    If Not bFound Then
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployeeaccesslevel, oEmployeeAccessLevel.IDEmployee, , oEmployeeAccessLevel.IDTimeZone, , Me.ToXml(oEmployeeAccessLevel.IDEmployee, oEmployeeAccessLevel.IDTimeZone))
                        HasChange = True
                    End If
                    bFound = False
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeAccessLevelMxapFile::CompareXmlFromDatabase: Unexpected error: ", ex)
            End Try
        End Sub

    End Class

End Namespace