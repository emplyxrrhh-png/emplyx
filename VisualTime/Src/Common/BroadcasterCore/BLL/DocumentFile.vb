Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para documentos en terminales compatibles
    ''' </summary>
    Public Class DocumentFile

        Private lsDocuments As ArrayList = New ArrayList
        Public HasChange As Boolean = False

        Public Sub Add(ByVal _IDEmployee As Integer, ByVal _IDReader As Byte, ByVal _Name As String, ByVal _Company As String, ByVal _BeginDate As DateTime, ByVal _EndDate As DateTime, ByVal _DenyAccess As Boolean)

            Try
                Dim oDocument As Document = New Document

                oDocument.IDEmployee = _IDEmployee
                oDocument.IDReader = _IDReader
                oDocument.Name = _Name
                oDocument.Company = _Company
                oDocument.BeginDate = _BeginDate
                oDocument.EndDate = _EndDate
                oDocument.DenyAccess = _DenyAccess

                lsDocuments.Add(oDocument)
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DocumentFile::Add: Unexpected error: ", ex)
            End Try

        End Sub

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncDocumentsData (EmployeeId
                                                                ,ReaderId
                                                                ,Name
                                                                ,BeginDate
                                                                ,EndDate
                                                                ,DenyAccess
                                                                ,Company
                                                                ,TerminalId
                                                                ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oDocument As Document In lsDocuments
                    sqlValues &= $" ({oDocument.IDEmployee},
                               {oDocument.IDReader},
                              '{oDocument.Name.Replace("'", "''")}',
                               CONVERT(SMALLDATETIME, '{oDocument.BeginDate.ToString("yyyy-MM-dd HH:mm:ss")}', 120),
                               CONVERT(SMALLDATETIME, '{oDocument.EndDate.ToString("yyyy-MM-dd HH:mm:ss")}', 120),
                               {If(oDocument.DenyAccess, 1, 0)},
                              '{oDocument.Company.Replace("'", "''")}',
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
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncDocumentsData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteDocumentsXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteDocumentsXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "DocumentFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteDocumentsXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As String
            Dim sql As String = $"@DELETE# FROM TerminalsSyncDocumentsData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "DocumentFile::DeleteDocumentsXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Function CompareXmlFromDatabase(ByVal terminalID As Integer, ByRef oBroadcasterManager As BroadcasterManager) As Boolean
            'Dim HasChange As Boolean = False

            Dim sql As String = $"@Declare# @xmldata XML;
                                    @SELECT# @xmldata = (@SELECT# EmployeeId AS IDEmployee
                                          ,ReaderId AS IDReader
                                          ,Name
                                          ,BeginDate
                                          ,EndDate
                                          ,DenyAccess
                                          ,Company
                                      FROM TerminalsSyncDocumentsData WITH (NOLOCK) WHERE TerminalId = {terminalID} FOR XML PATH('Documents'), ROOT('LocalDataSet'))
                                    @select# @xmldata as returnXml"

            Try

                Dim ds As New LocalDataSet
                Dim oTblXML As LocalDataSet.DocumentsDataTable = ds.Documents
                Dim oRowXML As LocalDataSet.DocumentsRow

                Dim lstXML As List(Of String) = New List(Of String)
                Dim lstTbl As List(Of String) = New List(Of String)

                Dim lstEmpl As List(Of Integer) = New List(Of Integer)

                Dim lstEmplDocs As List(Of Integer) = New List(Of Integer)
                Dim iEmp As Integer
                Dim tbRet As Object

                tbRet = AccessHelper.ExecuteScalar(sql)

                'Generamos la lista de empleados que tienen documentos
                For Each oDoc As Document In lsDocuments
                    If Not lstEmpl.Contains(oDoc.IDEmployee) Then
                        lstEmpl.Add(oDoc.IDEmployee)
                    End If
                Next

                'Generamos la lista de documentos
                For Each oDoc As Document In lsDocuments
                    lstTbl.Add(oDoc.ToString)
                Next

                If tbRet IsNot Nothing AndAlso Not tbRet.Equals(String.Empty) AndAlso Not IsDBNull(tbRet) Then
                    If Not tbRet.Equals("<LocalDataSet><Documents /></LocalDataSet>") Then
                        tbRet = tbRet.Replace("<LocalDataSet>", "<LocalDataSet xmlns=""http://tempuri.org/LocalDataSet.xsd"">")

                        Dim memoryStream = New MemoryStream()
                        Dim streamWriter = New StreamWriter(memoryStream, System.Text.Encoding.UTF8)
                        streamWriter.Write(tbRet)
                        streamWriter.Flush()
                        memoryStream.Position = 0

                        oTblXML.ReadXml(memoryStream)
                    End If

                    For Each oRowXML In oTblXML.Rows
                        lstXML.Add(roTypes.Any2String(oRowXML.IDEmployee) + ";" + roTypes.Any2String(oRowXML.IDReader) + ";" + roTypes.Any2String(oRowXML.Name) + ";" + roTypes.Any2String(oRowXML.Company) + ";" + roTypes.Any2String(oRowXML.BeginDate) + ";" + roTypes.Any2String(oRowXML.EndDate) + ";" + roTypes.Any2String(oRowXML.DenyAccess))
                    Next

                    'Miramos que datos en la base de datos hay en el xml
                    Dim lstDel As List(Of String) = New List(Of String)
                    For Each sLst As String In lstTbl
                        If lstXML.Contains(sLst) Then
                            lstDel.Add(sLst)
                        End If
                    Next

                    'Borramos los datos repetidos
                    For Each sLst As String In lstDel
                        lstTbl.Remove(sLst)
                        lstXML.Remove(sLst)
                    Next

                    'Añadimos empleados con documentos eliminados
                    For Each sLst As String In lstXML
                        iEmp = roTypes.Any2Integer(sLst.Split(";")(0))
                        'Miramos si el empleado aun ha de tener documentos
                        If lstEmpl.Contains(iEmp) Then
                            'Añadimos empleados si no estan ya en la lista
                            If Not lstEmplDocs.Contains(iEmp) Then
                                lstEmplDocs.Add(iEmp)
                            End If
                        Else
                            'Sino eliminamos los documentos del terminal
                            'Borramos todos los documentos del empleado
                            oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.deldocument, iEmp)
                        End If
                    Next
                Else
                    'Si el fichero no existia borramos todo
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.delalldocuments, 0)
                    HasChange = True
                End If

                'Añadimos empleados con nuevos documentos
                For Each sLst As String In lstTbl
                    iEmp = roTypes.Any2Integer(sLst.Split(";")(0))
                    'Añadimos empleados si no estan ya en la lista
                    If Not lstEmplDocs.Contains(iEmp) Then
                        lstEmplDocs.Add(iEmp)
                    End If
                Next

                For Each iEmp In lstEmplDocs
                    'Borramos todos los documentos del empleado
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.deldocument, iEmp)
                    'Subimos todos los documentos del empleado
                    oBroadcasterManager.AddTask(Robotics.Comms.Base.roTerminalsSyncTasks.SyncActions.adddocument, iEmp)
                    HasChange = True
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"DocumentFile::CompareXml: Unexpected error generating sync tasks for terminal {terminalID}: ", ex)
            End Try

            Return HasChange
        End Function

    End Class

End Namespace