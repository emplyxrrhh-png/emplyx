Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para grupos de empleados en terminales mx8
    ''' </summary>
    Public Class GroupEmployeesFile
        Private slGroupEmployee As New SortedList
        Public HasChange As Boolean = False

        Public Sub Add(ByVal EmployeeID As Integer, ByVal GroupID As Byte)
            Dim oGroupCard As New GroupEmployees
            oGroupCard.EmployeeID = EmployeeID
            oGroupCard.GroupID = GroupID
            oGroupCard.NewEmployee = True
            'If Not slGroupEmployee.ContainsKey(EmployeeID) Then slGroupEmployee.Add(EmployeeID, oGroupCard)
            If Not slGroupEmployee.ContainsKey(EmployeeID * 1000 + GroupID) Then slGroupEmployee.Add(EmployeeID * 1000 + GroupID, oGroupCard)
        End Sub

        Public Overrides Function ToString() As String
            Dim NewFile As String = ""
            Dim oGruop As GroupEmployees
            For Each oGruop In slGroupEmployee.Values
                NewFile += oGruop.ToString + BCGlobal.KeyNewLine
            Next
            Return NewFile
        End Function

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncGroupsData (EmployeeId
                                                               ,GroupId
                                                               ,TerminalID
                                                               ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oGroup As GroupEmployees In slGroupEmployee.Values
                    sqlValues &= $" ({oGroup.EmployeeID},
                               {oGroup.GroupID},
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
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncGroupsData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteGroupsXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteGroupsXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "GroupEmployeesFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteGroupsXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As String
            Dim sql As String = $"@DELETE# FROM TerminalsSyncGroupsData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "GroupEmployeesFile::DeleteGroupsXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Function CompareXmlFromDatabase(ByVal terminalId As Integer, ByRef oBroadcasterManager As BroadcasterManager) As Boolean
            'Dim HasChange As Boolean = False

            Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# EmployeeId AS IDEmployee
                                          ,GroupId AS IDGroup
                                      FROM TerminalsSyncGroupsData WITH (NOLOCK) WHERE TerminalId = {terminalId} FOR XML PATH('Groups'), ROOT('LocalDataSet'))
                                    @select# @xmldata as returnXml"

            Try
                Dim ds As New LocalDataSet
                Dim oTblXML As LocalDataSet.GroupsDataTable = ds.Groups
                Dim oRow As LocalDataSet.GroupsRow
                Dim oGroup As GroupEmployees
                Dim tbRet As Object

                tbRet = AccessHelper.ExecuteScalar(sql)

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><Groups /></LocalDataSet>") Then
                        tbRet = tbRet.Replace("<LocalDataSet>", "<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd"">")

                        Dim memoryStream = New MemoryStream()
                        Dim streamWriter = New StreamWriter(memoryStream, System.Text.Encoding.UTF8)
                        streamWriter.Write(tbRet)
                        streamWriter.Flush()
                        memoryStream.Position = 0

                        oTblXML.ReadXml(memoryStream)
                    End If

                    If slGroupEmployee.Count > 0 Then
                        For Each oRow In oTblXML.Rows
                            If slGroupEmployee.ContainsKey(oRow.IDEmployee * 1000 + oRow.IDGroup) Then
                                oGroup = CType(slGroupEmployee.Item(oRow.IDEmployee * 1000 + oRow.IDGroup), GroupEmployees)
                                If oGroup.GroupID <> oRow.IDGroup Then
                                    'Si han cambiado el nombre crea tarea
                                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployeegroup, oRow.IDEmployee, oGroup.GroupID)
                                    HasChange = True
                                End If
                                oGroup.NewEmployee = False
                            Else
                                'Si no existe actualmente lo borra
                                oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delemployeegroup, oRow.IDEmployee, oRow.IDGroup)
                                HasChange = True
                            End If
                        Next
                    Else
                        If oTblXML.Rows.Count > 0 Then
                            'Antes havian y ahora no, borramos todos
                            'Si el fichero no existia borramos todo
                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallemployeegroup, 0)
                            HasChange = True
                        Else
                            'Si ya esta vacio es que ya lo haviamos borrado antes, no hacemos nada.
                        End If
                    End If
                Else
                    'Si el fichero no existia borramos todo
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delallemployeegroup, 0)
                    HasChange = True
                End If
                'Buscamos los empleados nuevos que no existian anteriormente
                For Each oGroup In slGroupEmployee.Values
                    If oGroup.NewEmployee Then
                        'Si no existe actualmente lo añade
                        oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.addemployeegroup, oGroup.EmployeeID, oGroup.GroupID)
                        HasChange = True
                    End If
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"GroupEmployeesFile::CompareXmlFromDatabase: Unexpected error generating sync tasks for terminal {terminalId}: ", ex)
            End Try

            Return HasChange
        End Function

    End Class

End Namespace