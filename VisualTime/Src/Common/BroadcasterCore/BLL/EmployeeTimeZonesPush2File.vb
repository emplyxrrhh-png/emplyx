Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para periodos de acceso de empleados en terminales ZK (exceptuando centralitas)
    ''' </summary>
    Public Class EmployeeTimeZonesPush2File

        Private slEmployeeTimeZones As New SortedList
        Public HasChange As Boolean = False

        Public Sub Add(ByVal EmployeeID As Integer, ByVal TZs As String)
            Dim oEmpTZs As New EmployeeTimeZonesPush2
            Dim sID As String
            sID = EmployeeID.ToString
            oEmpTZs.EmployeeID = EmployeeID
            oEmpTZs.TZs = TZs
            If Not slEmployeeTimeZones.ContainsKey(sID) Then slEmployeeTimeZones.Add(sID, oEmpTZs)
        End Sub

        Public Overrides Function ToString() As String
            Dim NewFile As String = ""
            Try
                Dim oEmpTZs As EmployeeTimeZonesPush2
                For Each oEmpTZs In slEmployeeTimeZones.Values
                    NewFile += oEmpTZs.ToString + BCGlobal.KeyNewLine
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeTimeZonesPush2File::ToString: Unexpected error: ", ex)
            End Try
            Return NewFile
        End Function

        Public Function ToXml() As String

            Dim strXml As String = ""

            Try

                Dim dsLocalData As New LocalDataSet
                Dim tbEmployeeTZs As LocalDataSet.EmployeeTimeZonesZKPush2DataTable = dsLocalData.EmployeeTimeZonesZKPush2

                Dim oRow As LocalDataSet.EmployeeTimeZonesZKPush2Row
                For Each oEmpTZs As EmployeeTimeZonesPush2 In slEmployeeTimeZones.Values
                    oRow = tbEmployeeTZs.NewEmployeeTimeZonesZKPush2Row
                    oRow.IDEmployee = oEmpTZs.EmployeeID
                    oRow.TZs = oEmpTZs.TZs
                    tbEmployeeTZs.Rows.Add(oRow)
                Next

                Dim oStrm As New MemoryStream
                Dim bXml() As Byte

                tbEmployeeTZs.WriteXml(oStrm)
                ReDim bXml(oStrm.Length - 1)
                oStrm.Position = 0
                oStrm.Read(bXml, 0, oStrm.Length)
                oStrm.Close()

                strXml = System.Text.Encoding.UTF8.GetString(bXml, 0, bXml.Length)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeTimeZonesPush2File::ToXml: Unexpected error: ", Ex)
            End Try

            Return strXml

        End Function

        Public Function CompareXmlFromDatabase(ByVal terminalId As Integer, ByRef oBroadcasterManager As BroadcasterManager)
            Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# IDEmployee
                                          ,TZs
                                      FROM TerminalsSyncPushEmployeeTimeZonesData WITH (NOLOCK) WHERE TerminalId = {terminalId} FOR XML PATH('EmployeeTimeZonesZKPush2'), ROOT('LocalDataSet'))
                                    @select# @xmldata as returnXml"
            Try
                Dim ds As New LocalDataSet
                Dim oTblXML As LocalDataSet.EmployeeTimeZonesZKPush2DataTable = ds.EmployeeTimeZonesZKPush2
                Dim oRowXML As LocalDataSet.EmployeeTimeZonesZKPush2Row
                Dim oEmpTZ As EmployeeTimeZonesPush2
                Dim tbRet As Object

                tbRet = AccessHelper.ExecuteScalar(sql)

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><EmployeeTimeZonesZKPush2 /></LocalDataSet>") Then
                        tbRet = tbRet.Replace("<LocalDataSet>", "<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd"">")

                        Dim memoryStream = New MemoryStream()
                        Dim streamWriter = New StreamWriter(memoryStream, System.Text.Encoding.UTF8)
                        streamWriter.Write(tbRet)
                        streamWriter.Flush()
                        memoryStream.Position = 0

                        oTblXML.ReadXml(memoryStream)
                    End If

                    For Each oRowXML In oTblXML.Rows
                        If slEmployeeTimeZones.ContainsKey(oRowXML.IDEmployee.ToString) Then
                            oEmpTZ = CType(slEmployeeTimeZones.Item(oRowXML.IDEmployee.ToString), EmployeeTimeZonesPush2)
                            If oEmpTZ.TZs <> oRowXML.TZs Then
                                'Si han cambiado la tarjeta crea tarea
                                oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployeetimezones, oRowXML.IDEmployee, , , , oEmpTZ.TZs)
                                HasChange = True
                            End If
                        Else
                            'Si ahora no existe borramos todos los permisos de acceso del empleado
                            oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delemployeetimezones, oRowXML.IDEmployee)
                            HasChange = True
                        End If
                    Next
                Else
                    'Si el fichero no existia borramos todo
                    'TODO: Esta tarea en realidad es redundante. El driver forzará una reprogramación
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallemployeetimezones, 0)
                    'Borramos fichero de empleados para que suban todos de nuevo
                    'oBroadcasterManager.ForceEmployeeDatabaseInfoReset()
                    HasChange = True
                End If
                'Buscamos nuevas tarjeta
                For Each oEmpTZ In slEmployeeTimeZones.Values
                    Dim bFound As Boolean = False
                    For Each oRowXML In oTblXML.Rows
                        If oRowXML.IDEmployee = oEmpTZ.EmployeeID Then
                            bFound = True
                        End If
                    Next
                    If Not bFound Then
                        oBroadcasterManager.AddTaskEx(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployeetimezones, oEmpTZ.EmployeeID, , , , oEmpTZ.TZs)
                        HasChange = True
                    End If
                    bFound = False

                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"EmployeeTimeZonesPush2File::CompareXmlFromDatabase: Unexpected error generating sync tasks for terminal {terminalId}: ", ex)
            End Try
            Return HasChange
        End Function

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncPushEmployeeTimeZonesData
                                       (IDEmployee
                                       ,TZs
                                       ,TerminalId
                                       ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oEmployeeTimeZone As EmployeeTimeZonesPush2 In slEmployeeTimeZones.Values
                    sqlValues &= $" ({oEmployeeTimeZone.EmployeeID},
                              '{oEmployeeTimeZone.TZs}',
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
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncPushEmployeeTimeZonesData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteEmployeeTimeZonesZKPush2XmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteEmployeeTimeZonesZKPush2XmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeTimeZonesZKPush2File::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteEmployeeTimeZonesZKPush2XmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As Boolean
            Dim sql As String = $"@DELETE# FROM TerminalsSyncPushEmployeeTimeZonesData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "EmployeeTimeZoneZKPush2File::DeleteEmployeesXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

    End Class

End Namespace