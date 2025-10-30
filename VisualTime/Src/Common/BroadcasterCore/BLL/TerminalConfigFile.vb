Imports System.Data
Imports System.IO
Imports Robotics.DataLayer
Imports Robotics.VTBase

Namespace BusinessLogicLayer

    ''' <summary>
    ''' Modelo de datos para parámetros de configuración general de los diferentes terminales
    ''' </summary>
    Public Class TerminalConfigFile

        Private mDictionary As New Dictionary(Of String, String)

        Public Enum eParameterNames
            RDR1OpenTime
            RDR1Relay
            RDR2OpenTime
            RDR2Relay
            RDR3OpenTime
            RDR3Relay
            RDR4OpenTime
            RDR4Relay
            Door1ValidTZ
            Door2ValidTZ
            Door3ValidTZ
            Door4ValidTZ
            LockOn
            SaveFailedLog
            COMKey
            VTVersion
            Mode
            InteractionAction
            Zone
            TakePhoto
            ValdationMode
            TimezoneName
            IsDifferentZoneTime
            Door1Intertime
            Door2Intertime
            Door3Intertime
            Door4Intertime
            AutoDaylight
        End Enum

        Public Sub Add(ByVal Name As eParameterNames, ByVal Value As String)
            Try
                If mDictionary.ContainsKey(Name.ToString) Then
                    mDictionary(Name.ToString) = Value
                Else
                    mDictionary.Add(Name.ToString, Value)
                End If
            Catch ex As Exception

            End Try
        End Sub

        Public Overrides Function ToString() As String
            Dim NewFile As String = ""
            For Each item As KeyValuePair(Of String, String) In mDictionary
                NewFile += item.Key + "=" + item.Value + BCGlobal.KeyNewLine
            Next
            Return NewFile
        End Function

        Public Function ToXml() As String

            Dim strXml As String = ""

            Try

                Dim dsLocalData As New LocalDataSet
                Dim tbTerminalConfig As LocalDataSet.TerminalConfigDataTable = dsLocalData.TerminalConfig

                Dim oRow As LocalDataSet.TerminalConfigRow
                For Each item As KeyValuePair(Of String, String) In mDictionary
                    oRow = tbTerminalConfig.NewTerminalConfigRow
                    oRow.Name = item.Key
                    oRow.Value = item.Value
                    tbTerminalConfig.Rows.Add(oRow)
                Next

                Dim oStrm As New MemoryStream
                Dim bXml() As Byte

                tbTerminalConfig.WriteXml(oStrm)
                ReDim bXml(oStrm.Length - 1)
                oStrm.Position = 0
                oStrm.Read(bXml, 0, oStrm.Length)
                oStrm.Close()

                strXml = System.Text.Encoding.UTF8.GetString(bXml, 0, bXml.Length)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalConfigFile::ToXml: Unexpected error: ", Ex)
            End Try

            Return strXml

        End Function

        Public Function HasChanged(terminalId As String) As String
            Dim result As Boolean = False
            Dim sql As String = $"@SELECT# Name
                                            ,Value
                                    FROM TerminalsSyncConfigData WHERE TerminalId = {terminalId}"

            Try
                Dim tb As DataTable = AccessHelper.CreateDataTable(sql, "")

                If (tb.Rows.Count = 0 AndAlso mDictionary.Count = 0) Then
                    'En este caso quiere decir que el terminal no tiene configuración pero la BD no tiene registro de ello, por tanto se debe insertar una linea en blanco, y para ello debo informar
                    'de que hay cambios.
                    result = True
                ElseIf (tb.Rows.Count = mDictionary.Count) Then
                    For Each row As DataRow In tb.Rows
                        Dim configParamenter As String = mDictionary.Item(row("Name").ToString())

                        If IsNothing(configParamenter) OrElse Not configParamenter = row("Value") Then
                            result = True
                        End If
                    Next
                ElseIf (tb.Rows.Count = 1 AndAlso IsDBNull(tb.Rows(0)("Name")) AndAlso mDictionary.Count = 0) Then
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

            Dim sqlInsert As String = "@INSERT# INTO TerminalsSyncConfigData (Name
                                                            ,Value
                                                            ,TerminalId
                                                            ,TimeStamp) VALUES"
            Dim sqlValuesList As List(Of String) = New List(Of String)
            Dim sqlValues As String = String.Empty

            Try

                For Each item As KeyValuePair(Of String, String) In mDictionary
                    sqlValues &= $" ('{item.Key}',
                               '{item.Value}',
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
                    result = result AndAlso AccessHelper.ExecuteSql($"@INSERT# INTO TerminalsSyncConfigData (TerminalId, TimeStamp) VALUES ({terminalId}, CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121))")
                End If

                If result Then
                    'ESBORRO LES LINIES ANTIGUES
                    result = result AndAlso DeleteTerminalsConfigXmlFromDatabase(terminalId, timeStamp, False)
                End If

                If Not result Then
                    'ALGUNA COSA HA FALLAT EN SQL, ESBORRO LES NOVES LINIES.
                    Throw New Exception("SQL Execution result = False")
                End If
            Catch Ex As Exception
                DeleteTerminalsConfigXmlFromDatabase(terminalId, timeStamp, True)

                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalConfigFile::SaveToDataBase: Unexpected error: ", Ex)
                result = False
            End Try

            Return result
        End Function

        Public Function DeleteTerminalsConfigXmlFromDatabase(terminalId As String, timeStamp As DateTime, deleteNewerLines As Boolean) As Boolean
            Dim sql As String = $"@DELETE# FROM TerminalsSyncConfigData WHERE TerminalId = {terminalId} AND TimeStamp {If(deleteNewerLines, ">=", "<")} CONVERT(DATETIME, '{timeStamp.ToString("yyyy-MM-dd HH:mm:ss.fff")}', 121)"

            Try
                Return AccessHelper.ExecuteSql(sql)
            Catch Ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "TerminalConfigFile::DeleteCausesXmlFromDatabase: Unexpected error: ", Ex)
            End Try

            Return False
        End Function

    End Class

End Namespace