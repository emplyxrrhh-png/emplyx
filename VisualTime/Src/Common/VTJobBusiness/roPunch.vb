Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Public Class roPunch

    Public Function SaveOfflinePunch(ByVal _DateTime As DateTime, ByVal _IDCard As Long,
                                     ByVal _IDReader As Integer, ByVal _Type As Char, ByVal _IDCause As Integer,
                                     ByVal _InvalidRead As Byte, ByVal _Rdr As Byte, ByVal _Capture() As Byte,
                                     ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False

        If _IDCause = -1 Then _IDCause = 0

        Dim strPunch As String = "DateTime=" & _DateTime.ToShortDateString & " " & _DateTime.ToShortTimeString & ";" &
                                 "IDCard=" & _IDCard.ToString & ";" &
                                 "IDReader=" & _IDReader & ";" &
                                 "Type=" & _Type & ";" &
                                 "IDCause=" & _IDCause & ";" &
                                 "InvalidRead=" & _InvalidRead & ";" &
                                 "Rdr=" & _Rdr
        If _Capture IsNot Nothing Then
            strPunch &= "Capture.Length=" & _Capture.Length
        End If

        oLog.logMessage(roLog.EventType.roDebug, "roPunch::SaveOfflinePunch::Saving offline punch data (" & strPunch & ")")

        Try

            Dim strSQL As String

            ' Verificamos si hay que guardar el fichaje en la table InvalidMoves (sólo si es inválido y proviene de un mx7 con comportamiento TAACC
            Dim bolSaveInvalidMove As Boolean = False
            If _InvalidRead Then
                strSQL = "@SELECT# TerminalReaders.Mode " &
                         "FROM Terminals INNER JOIN TerminalReaders ON Terminals.ID = TerminalReaders.IDTerminal " &
                         "WHERE Terminals.Type = 'MX7' AND " &
                               "Terminals.ID = " & _IDReader.ToString & " AND " &
                               "TerminalReaders.ID = " & _Rdr.ToString
                Dim tbTerminalMode As DataTable = CreateDataTable(strSQL, )
                If tbTerminalMode IsNot Nothing AndAlso tbTerminalMode.Rows.Count = 1 Then
                    bolSaveInvalidMove = (roTypes.Any2String(tbTerminalMode.Rows(0).Item("Mode")).ToUpper = "TAACC")
                End If
            End If

            If Not bolSaveInvalidMove Then

                strSQL = "@INSERT# INTO Entries([DateTime], IDCard, IDReader, Type, IDCause, InvalidRead, Rdr) " &
                         "VALUES( CONVERT(smalldatetime, '" & Format(_DateTime, "MM/dd/yyyy HH:mm") & "', 120), " &
                                _IDCard.ToString & ", " & _IDReader.ToString & ", '" & _Type & "', " &
                                _IDCause.ToString & ", " & _InvalidRead.ToString & ", " & _Rdr.ToString & ")"
                bolRet = ExecuteSql(strSQL)

                ' Grabar captura imagen
                strSQL = "@SELECT# TOP 1 [ID] FROM Entries ORDER BY [ID] DESC"
                Dim rd As DbDataReader = CreateDataReader(strSQL)
                If rd.Read Then

                    Dim intIDEntry As Integer = rd(0)

                    rd.Close()

                    Dim tbCaptures As New DataTable("EntriesCaptures")
                    strSQL = "@SELECT# * FROM EntriesCaptures WHERE IDEntry = " & intIDEntry.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbCaptures)

                    Dim oRow As DataRow = Nothing
                    Dim bolNewRow As Boolean = False
                    If tbCaptures.Rows.Count = 0 Then
                        oRow = tbCaptures.NewRow
                        bolNewRow = True
                        oRow("IDEntry") = intIDEntry
                    Else
                        oRow = tbCaptures.Rows(0)
                    End If

                    If _Capture IsNot Nothing Then
                        oRow("Capture") = _Capture
                    Else
                        oRow("Capture") = DBNull.Value
                    End If

                    If bolNewRow Then
                        tbCaptures.Rows.Add(oRow)
                    End If

                    da.Update(tbCaptures)
                Else
                    rd.Close()
                End If
            Else

                ' Guardar captura imagen
                Dim lngIDCapture As Long = -1
                If _Capture IsNot Nothing AndAlso _Capture.Length > 0 Then
                    Dim oCaptureObj As New roCapture(lngIDCapture, oLog)
                    oCaptureObj.DateTime = _DateTime
                    oCaptureObj.Capture = roTypes.Bytes2Image(_Capture)
                    If oCaptureObj.Save() Then
                        lngIDCapture = oCaptureObj.ID
                    End If
                End If

                strSQL = "@INSERT# INTO InvalidMoves(ID, [DateTime], IDCard, IDReader, Type, IDCause, Rdr, IDCapture) " &
                         "VALUES(" & Me.GetNextIDInvalidMove(oLog) & ", " &
                                 "CONVERT(smalldatetime, '" & Format(_DateTime, "MM/dd/yyyy HH:mm") & "', 120), " &
                                 _IDCard.ToString & ", " & _IDReader.ToString & ", '" & _Type & "', " &
                                 _IDCause.ToString & ", " & _Rdr.ToString & ", " &
                                 IIf(lngIDCapture > 0, lngIDCapture.ToString, "NULL") & ")"
                bolRet = ExecuteSql(strSQL)

            End If
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roOPunch::SaveOfflinePunch :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roPunch::SaveOfflinePunch :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Private Function GetNextIDInvalidMove(ByVal oLog As roLog) As Integer

        Dim lngRet As Long = 1

        Try

            Dim strSQL As String = "@SELECT# MAX(ID) FROM InvalidMoves"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                If Not IsDBNull(tb.Rows(0).Item(0)) Then
                    lngRet = CLng(tb.Rows(0).Item(0)) + 1
                End If
            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roPunch::GetNextIDInvalidMove :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roPunch::GetNextIDInvalidMove :", ex)
        Finally

        End Try

        Return lngRet
    End Function

End Class