Imports System.Data
Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para justificaciones en terminales
    ''' </summary>
    Public Class CausesFile

        Private slCauses As New SortedList

        Public Sub Add(ByVal CauseID As Integer, ByVal CauseName As String, ByVal ReaderInputCode As Integer, ByVal WorkingType As Boolean)
            Dim oCause As New Cause
            Dim sID As String
            sID = CauseID.ToString
            oCause.CauseID = CauseID
            oCause.CauseName = CauseName.Replace("'", "''")
            oCause.ReaderInputCode = ReaderInputCode
            oCause.WorkingType = WorkingType
            slCauses.Add(sID, oCause)
        End Sub

        Public Overrides Function ToString() As String
            Dim NewFile As String = ""
            Try

                Dim ocause As Cause
                For Each ocause In slCauses.Values
                    NewFile += ocause.ToString + BCGlobal.KeyNewLine
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CardFile::ToString: Unexpected error: ", ex)
            End Try
            Return NewFile
        End Function

        Public Function HasChanged(terminalId As String) As String
            Dim result As Boolean = False
            Dim sql As String = $"@SELECT# CauseId
                                        ,Name
                                        ,ReaderInputCode
                                        ,WorkingType
                                    FROM TerminalsSyncCausesData WHERE TerminalId = {terminalId}"

            Try
                Dim tb As DataTable = AccessHelper.CreateDataTable(sql, "")

                If (tb.Rows.Count = 0 AndAlso slCauses.Values.Count = 0) Then
                    result = True
                ElseIf (tb.Rows.Count = slCauses.Values.Count) Then
                    For Each row As DataRow In tb.Rows
                        Dim cause As Cause = slCauses.Item(row("CauseId").ToString())

                        If IsNothing(cause) OrElse Not cause.CauseName = row("Name") OrElse Not cause.ReaderInputCode = row("ReaderInputCode") OrElse Not cause.WorkingType = row("WorkingType") Then
                            result = True
                        End If
                    Next
                ElseIf (tb.Rows.Count = 1 AndAlso IsDBNull(tb.Rows(0)("Name")) AndAlso slCauses.Count = 0) Then
                    'Siguen sin haber cambios
                Else
                    result = True
                End If
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "SirensFile::SaveToDataBase: Unexpected error: ", Ex)
                result = True
            End Try

            Return result
        End Function

        Public Function SaveToDataBase(terminalId As String) As String
            Dim result As Boolean = True

            Dim insertCount = 0
            Dim timeStamp As DateTime = DateTime.Now

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncCausesData (CauseId
                                                               ,Name
                                                               ,ReaderInputCode
                                                               ,WorkingType
                                                               ,TerminalId
                                                               ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try
                For Each oCause As Cause In slCauses.Values
                    sqlValues &= $" ({oCause.CauseID},
                              '{oCause.CauseName.Replace("'", "''")}',
                               {oCause.ReaderInputCode},
                               {IIf(oCause.WorkingType, 1, 0)},
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
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncCausesData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteCausesXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteCausesXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "CausesFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteCausesXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As Boolean
            Dim sql As String = $"@DELETE# FROM TerminalsSyncCausesData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CausesFile::DeleteCausesXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

        Public Function ToXml() As String

            Dim strXml As String = ""

            Try

                Dim dsLocalData As New LocalDataSet
                Dim tbCauses As LocalDataSet.CausesDataTable = dsLocalData.Causes

                Dim oRow As LocalDataSet.CausesRow
                For Each oCause As Cause In slCauses.Values
                    oRow = tbCauses.NewCausesRow
                    oRow.IDCause = oCause.CauseID
                    oRow.Name = oCause.CauseName
                    oRow.ReaderInputCode = oCause.ReaderInputCode
                    oRow.WorkingType = oCause.WorkingType
                    tbCauses.Rows.Add(oRow)
                Next

                Dim oStrm As New MemoryStream
                Dim bXml() As Byte

                'tbCards.WriteXml(oStrm, System.Data.XmlWriteMode.WriteSchema, False)
                tbCauses.WriteXml(oStrm)
                ReDim bXml(oStrm.Length - 1)
                oStrm.Position = 0
                oStrm.Read(bXml, 0, oStrm.Length)
                oStrm.Close()

                strXml = System.Text.Encoding.UTF8.GetString(bXml, 0, bXml.Length)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "CausesFile::ToXml: Unexpected error: ", Ex)
            End Try

            Return strXml

        End Function

    End Class

End Namespace