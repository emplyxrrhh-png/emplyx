Imports System.IO
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roCustomArgal
        Inherits roDataLinkExport

        Public Shared Function ExportProfileArgalASCII(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim bContinue As Boolean = False
            Dim zip As ZipExport = Nothing
            Dim zipPath As String = String.Empty
            Dim oMemStream As MemoryStream

            Try
                Dim NameFile As String

                NameFile = "DataLinkExportArgal#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".zip"
                zip = New ZipExport(NameFile)

                ' Crearemos cuatro ficheros, y los empaquetaremos en un ZIP para poder descargarlo
                '0.- Registros validados
                arrFile = ASCIIExportARGAL_Fichajes(mEmployees, dtBeginDate, dtEndDate, False, oState, bContinue)
                oMemStream = New MemoryStream(arrFile)
                zip.zipExportFile.AddEntry("FichajesValidados", oMemStream)

                '1.- Turnos validados
                arrFile = ASCIIExportARGAL_Turnos(mEmployees, dtBeginDate, dtEndDate, False, oState, bContinue)
                oMemStream = New MemoryStream(arrFile)
                zip.zipExportFile.AddEntry("TurnosValidados", oMemStream)

                '2.- Registros Modificados
                arrFile = ASCIIExportARGAL_Fichajes(mEmployees, dtBeginDate, dtEndDate, True, oState, bContinue)
                oMemStream = New MemoryStream(arrFile)
                zip.zipExportFile.AddEntry("FichajesModificados", oMemStream)

                '3.- Turnos Modificados
                arrFile = ASCIIExportARGAL_Turnos(mEmployees, dtBeginDate, dtEndDate, True, oState, bContinue)
                oMemStream = New MemoryStream(arrFile)
                zip.zipExportFile.AddEntry("TurnosModificados", oMemStream)

                '4.- Si todo fue correctamente, quito marca de  modificados
                If bContinue Then
                    zip.SaveFile()

                    ExecuteSql("@UPDATE# dailyschedule WITH (ROWLOCK) SET ISExported=1, IsModified=0 WHERE IsExported=0 and IsModified=1")
                End If

                ' Devuelve array de bytes

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportARGAL")
            Finally
                ' Borro ficheros
                If IO.File.Exists(zipPath) Then IO.File.Delete(zipPath)
                If Not zip Is Nothing Then zip.zipExportFile.Dispose()
            End Try

            Return arrFile

        End Function

        Private Shared Function ASCIIExportARGAL_Fichajes(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal bOnlyModified As Boolean, ByRef oState As roDataLinkState, ByRef bResult As Boolean) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim NameFile As String = String.Empty
            Dim IDTipo As String = String.Empty
            Dim sIDTerminal As String = String.Empty
            Dim sFuncion As String = String.Empty
            Dim sFabrica As String = String.Empty
            Dim sIDContract As String = String.Empty
            Dim dDateTime As DateTime = DateTime.MinValue
            Dim dShiftDate As Date = DateTime.MinValue
            Dim punchesStream As MemoryStream = New MemoryStream()
            Dim dt As DataTable = Nothing

            Try
                bResult = False

                If Not bOnlyModified Then
                    NameFile = "ExportARGAL_FichajesValidados" & ".txt"
                Else
                    NameFile = "ExportARGAL_FichajesModificados" & ".txt"
                End If

                Dim strLine As String = ""
                Dim strLines As List(Of String) = New List(Of String)
                Dim queryString As String = String.Empty

                queryString = "@SELECT# P.IDEmployee, "
                queryString = queryString & " ISNULL(EUF.Value,'') AS CodigoUnix, "
                queryString = queryString & " ISNULL(EUF1.Value,'') AS Fabrica,  "
                queryString = queryString & " ISNULL(EUF2.Value,'') AS Tipo,  "
                queryString = queryString & " CASE WHEN P.ActualType = 1 THEN ' IN' WHEN  P.ActualType = 2 THEN 'OUT' END AS Funcion, "
                queryString = queryString & " P.IDTerminal, "
                queryString = queryString & " P.DateTime, "
                queryString = queryString & " P.ShiftDate, "
                queryString = queryString & " DS.IsModified, "
                queryString = queryString & " EC.IDContract "
                queryString = queryString & " FROM Punches P "
                queryString = queryString & " INNER JOIN EmployeeContracts EC ON EC.IDEmployee = P.IDEmployee AND P.ShiftDate BETWEEN EC.BeginDate AND EC.EndDate "
                queryString = queryString & " LEFT OUTER JOIN DailySchedule DS ON DS.IDEmployee = P.IDEmployee AND DS.Date = P.ShiftDate "
                queryString = queryString & " LEFT OUTER JOIN EmployeeUserFieldValues EUF ON P.IDEmployee = EUF.IDEmployee AND EUF.FieldName = 'CodigoUnix' AND EUF.Date = convert(smalldatetime,'1900-01-01',120) "
                queryString = queryString & " LEFT OUTER JOIN EmployeeUserFieldValues EUF1 ON P.IDEmployee = EUF1.IDEmployee AND EUF1.FieldName = 'Fabrica' AND EUF1.Date = convert(smalldatetime,'1900-01-01',120) "
                queryString = queryString & " LEFT OUTER JOIN EmployeeUserFieldValues EUF2 ON P.IDEmployee = EUF2.IDEmployee AND EUF2.FieldName = 'Tipo' AND EUF2.Date = convert(smalldatetime,'1900-01-01',120) "
                queryString = queryString & " WHERE  "
                queryString = queryString & " ShiftDate "
                If bOnlyModified Then
                    ' Modificados fuera del periodo
                    queryString = queryString & " NOT "
                End If
                queryString = queryString & " BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & "  AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime
                queryString = queryString & " AND P.ActualType IN (1,2) "
                queryString = queryString & " AND P.IdEmployee IN (" & mEmployees & ") "

                If bOnlyModified Then
                    queryString = queryString & " AND DS.IsModified = 1 "
                End If

                queryString = queryString & " ORDER BY P.IDEmployee, ShiftDate, DateTime "

                dt = CreateDataTable(queryString)

                For Each row As System.Data.DataRow In dt.Rows
                    If Any2String(row("Tipo")) = "P" Then

                        sIDTerminal = Any2String(row("IDTerminal"))
                        sFuncion = Any2String(row("Funcion"))
                        sFabrica = Any2String(row("Fabrica"))
                        sIDContract = Any2String(row("CodigoUnix"))
                        If sIDContract.Length = 0 Then sIDContract = Any2String(row("IDContract"))
                        dDateTime = Any2DateTime(row("DateTime"))
                        dShiftDate = Any2DateTime(row("ShiftDate"))

                        strLine = Right$("00" & sFabrica, 2)
                        strLine = strLine & "|" & Format$(dDateTime.ToString("dd/MM/yy"))
                        strLine = strLine & "|" & Format$(dDateTime.ToString("HH:mm:ss"))
                        strLine = strLine & "|" & sFuncion
                        strLine = strLine & "|" & Right$("00000" & sIDContract, 5)
                        strLine = strLine & "|" & "     "
                        strLine = strLine & "|" & "000"
                        strLine = strLine & "|" & "0000000000"
                        strLine = strLine & "|" & "00000"
                        strLine = strLine & "|" & "00000"
                        strLine = strLine & "|" & Right$("000" & sIDTerminal, 3)
                        strLine = strLine & "|" & Format$(dShiftDate.ToString("dd/MM/yyyy"))

                        ' Graba la linea si uno de los dos saldos tiene valor
                        strLines.Add(strLine)

                    End If
                Next

                punchesStream = GenerateStreamFromString(strLines)

                bResult = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportARGAL_Fichajes")
            Finally
                arrFile = punchesStream.ToArray()

                punchesStream.Dispose()

                ' Libera memoria
                If Not dt Is Nothing Then dt.Dispose()
            End Try

            Return arrFile

        End Function

        Private Shared Function ASCIIExportARGAL_Turnos(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal bOnlyModified As Boolean, ByRef oState As roDataLinkState, ByRef bResult As Boolean) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim NameFile As String = String.Empty
            Dim punchesStream As MemoryStream = New MemoryStream()
            Dim dt As DataTable = Nothing

            Try
                bResult = False
                If Not bOnlyModified Then
                    NameFile = "ExportARGAL_TurnosValidados" & ".txt"
                Else
                    NameFile = "ExportARGAL_TurnosModificados" & ".txt"
                End If

                Dim strLine As String = ""
                Dim strLines As List(Of String) = New List(Of String)
                Dim queryString As String = String.Empty

                queryString = "@SELECT# ROW_NUMBER() OVER (PARTITION BY IdEmployee, Date ORDER BY IdEmployee ASC, Date ASC, BeginTime ASC) As Numero, "
                queryString = queryString & " COUNT(IDEmployee) OVER (PARTITION BY IdEmployee, Date) As Total, "
                queryString = queryString & " * FROM ( "
                queryString = queryString & " @SELECT# DS.IDEmployee, "
                queryString = queryString & " ISNULL(EUF.Value,'') AS CodigoUnix, "
                queryString = queryString & " ISNULL(EUF1.Value,'') AS Fabrica,  "
                queryString = queryString & " ISNULL(EUF2.Value,'') AS Tipo, "
                queryString = queryString & " DS.Date, "
                queryString = queryString & " DI.IDType, "
                queryString = queryString & " DC.IDCause, "
                queryString = queryString & " CA.ShortName, "
                queryString = queryString & " CA.RoundingType, "
                queryString = queryString & " CA.RoundingBy, "
                queryString = queryString & " DI.BeginTime, "
                queryString = queryString & " DI.EndTime, "
                queryString = queryString & " DS.IsModified, "
                queryString = queryString & " EC.IdContract, "
                queryString = queryString & " DATEDIFF(minute,BeginTime,EndTime) Diferencia, "
                'queryString = queryString & " CASE WHEN CA.Description LIKE '%Tipo=ORD%' THEN 'ORD' WHEN CA.Description LIKE '%Tipo=EXT%' THEN 'EXT' ELSE 'Otro' END AS TipoJustificacion "
                queryString = queryString & " CASE CHARINDEX('Tipo=', REPLACE(CONVERT(NVARCHAR(4000),Description),' ','')) WHEN 0 THEN '' ELSE SUBSTRING(REPLACE(CONVERT(NVARCHAR(4000),Description),' ',''), CHARINDEX('Tipo=', REPLACE(CONVERT(NVARCHAR(4000),Description),' ','')) + 5, 3) END AS TipoJustificacion"
                queryString = queryString & " FROM DailySchedule DS "
                queryString = queryString & " INNER JOIN EmployeeContracts EC ON EC.IDEmployee = DS.IDEmployee AND DS.Date BETWEEN EC.BeginDate AND EC.EndDate "
                '--------------------- MOdificación 20200203 Solicitado por D.Acuña sobre especificación inicial ----------------------------
                'queryString = queryString & " RIGHT OUTER JOIN DailyIncidences DI ON DI.IDEmployee = DS.IDEmployee AND DS.Date = DI.Date AND DI.IDType IN (1001, 1010, 1030) "
                queryString = queryString & " RIGHT OUTER JOIN DailyIncidences DI ON DI.IDEmployee = DS.IDEmployee AND DS.Date = DI.Date "
                queryString = queryString & " RIGHT OUTER JOIN DailyCauses DC ON DC.IDEmployee = DI.IDEmployee AND DC.IDRelatedIncidence = DI.ID AND DI.Date = DC.Date "
                queryString = queryString & " LEFT OUTER JOIN Causes CA ON CA.ID = DC.IDCause "
                queryString = queryString & " LEFT OUTER JOIN EmployeeUserFieldValues EUF ON DS.IDEmployee = EUF.IDEmployee AND EUF.FieldName = 'CodigoUnix' AND EUF.Date = convert(smalldatetime,'1900-01-01',120) "
                queryString = queryString & " LEFT OUTER JOIN EmployeeUserFieldValues EUF1 ON DS.IDEmployee = EUF1.IDEmployee AND EUF1.FieldName = 'Fabrica' AND EUF1.Date = convert(smalldatetime,'1900-01-01',120) "
                queryString = queryString & " LEFT OUTER JOIN EmployeeUserFieldValues EUF2 ON DS.IDEmployee = EUF2.IDEmployee AND EUF2.FieldName = 'Tipo' AND EUF2.Date = convert(smalldatetime,'1900-01-01',120) "
                queryString = queryString & " WHERE  "
                queryString = queryString & " DS.Date "
                If bOnlyModified Then
                    ' Modificados fuera del periodo
                    queryString = queryString & " NOT "
                End If
                queryString = queryString & " BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & "  AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime
                'queryString = queryString & " AND CASE WHEN CA.Description LIKE '%Tipo=ORD%' THEN 'ORD' WHEN CA.Description LIKE '%Tipo=EXT%' THEN 'EXT' ELSE 'Otro' END IN ('ORD','EXT') "
                queryString = queryString & " AND CASE CHARINDEX('Tipo=', REPLACE(CONVERT(NVARCHAR(4000),Description),' ','')) WHEN 0 THEN '' ELSE SUBSTRING(REPLACE(CONVERT(NVARCHAR(4000),Description),' ',''), CHARINDEX('Tipo=', REPLACE(CONVERT(NVARCHAR(4000),Description),' ','')) + 5, 3) END <> ''"
                queryString = queryString & " AND DS.IdEmployee IN (" & mEmployees & ") "
                If bOnlyModified Then
                    queryString = queryString & " AND DS.IsModified = 1 "
                End If
                queryString = queryString & " ) AS AUXTB "
                queryString = queryString & " ORDER BY AUXTB.IDEmployee, AUXTB.Date, AUXTB.BeginTime "

                dt = CreateDataTable(queryString)

                Dim sTipoHorario As String = String.Empty
                Dim sIDType As String = String.Empty
                Dim sTipoJustificación As String = String.Empty
                Dim bCorrect As Boolean = False
                Dim sFabrica As String = String.Empty
                Dim sIDContract As String = String.Empty
                Dim dDate As Date = Date.MinValue
                Dim dHoraInicial As DateTime = DateTime.MinValue
                Dim dHoraFinal As DateTime = DateTime.MinValue
                Dim sShortName As String = String.Empty
                Dim sRoundingBy As String = String.Empty
                Dim sRoundingType As String = String.Empty
                Dim iDiferencia As Integer = 0
                Dim iDiferenciaRedondeada As Integer = 0
                Dim oTime As roTime = New roTime
                Dim iNumeroEnDia As Integer = 0
                Dim iTotalEnDia As Integer = 0

                For Each row As System.Data.DataRow In dt.Rows
                    ' Para cada registro
                    If Any2String(row("Tipo")) = "P" Then

                        sIDType = Any2String(row("IDType"))
                        sTipoJustificación = Any2String(row("TipoJustificacion"))

                        '--------------------- MOdificación 20200117 Solicitado por D.Acuña sobre especificación inicial ----------------------------
                        '----------- ORIGINAL ERA ASÍ------------
                        'bCorrect = False
                        'If sIDType = "1001" Then
                        '    If sTipoJustificación = "ORD" Then
                        '        sTipoHorario = "ORD"
                        '        bCorrect = True
                        '    End If
                        'Else
                        '    If sTipoJustificación = "EXT" Then
                        '        sTipoHorario = "EXT"
                        '        bCorrect = True
                        '    End If
                        'End If

                        '----------- NUEVO ES ASÍ------------
                        bCorrect = True
                        sTipoHorario = sTipoJustificación

                        If bCorrect Then
                            sFabrica = Any2String(row("Fabrica"))
                            sIDContract = Any2String(row("CodigoUnix"))
                            If sIDContract.Length = 0 Then sIDContract = Any2String(row("IDContract"))
                            dHoraInicial = Any2DateTime(row("BeginTime"))
                            dHoraFinal = Any2DateTime(row("EndTime"))
                            dDate = Any2DateTime(row("Date"))

                            If sTipoHorario = "EXT" Then
                                sShortName = Any2String(row("ShortName"))
                                If sShortName = "Ext" Then
                                    sRoundingBy = Any2String(row("RoundingBy"))
                                    sRoundingType = Any2String(row("RoundingType"))
                                    iDiferencia = Any2Integer(row("Diferencia"))

                                    iDiferenciaRedondeada = oTime.RoundTime(iDiferencia, sRoundingType, sRoundingBy)

                                    iNumeroEnDia = Any2Integer(row("Numero"))
                                    iTotalEnDia = Any2Integer(row("Total"))

                                    If iTotalEnDia = 1 Then
                                        'Si el total de linias es 1 redondeamos al final
                                        dHoraFinal = dHoraInicial.AddMinutes(iDiferenciaRedondeada)
                                    ElseIf iTotalEnDia = 2 Then
                                        'Si hay dos linias puede estar primero o último
                                        If iNumeroEnDia = 1 Then
                                            'Si es la primera linia redondeamos al inicio
                                            dHoraInicial = dHoraFinal.AddMinutes(-1 * iDiferenciaRedondeada)
                                        ElseIf iNumeroEnDia = 2 Then
                                            'Si es la segunda linia redondeamos al final
                                            dHoraFinal = dHoraInicial.AddMinutes(iDiferenciaRedondeada)
                                        End If
                                    ElseIf iTotalEnDia >= 3 Then
                                        If iNumeroEnDia = iTotalEnDia Then
                                            'Si es la ultima linia redondeamos al final
                                            dHoraFinal = dHoraInicial.AddMinutes(iDiferenciaRedondeada)
                                        Else
                                            'Sino siempre redondeamos al inicio
                                            dHoraInicial = dHoraFinal.AddMinutes(-1 * iDiferenciaRedondeada)
                                        End If
                                    End If

                                End If
                            End If

                            strLine = Right$("00" & sFabrica, 2)
                            strLine = strLine & "|" & Format$(dDate.ToString("dd/MM/yyyy"))
                            strLine = strLine & "|" & Right$("00000" & sIDContract, 5)
                            strLine = strLine & "|" & Format$(dHoraInicial.ToString("HH:mm:ss"))
                            strLine = strLine & "|" & Format$(dHoraFinal.ToString("HH:mm:ss"))
                            strLine = strLine & "|" & sTipoHorario

                            ' Graba la linea si uno de los dos saldos tiene valor
                            strLines.Add(strLine)
                        End If
                    End If
                Next
                punchesStream = GenerateStreamFromString(strLines)

                bResult = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportARGAL_Fichajes")
            Finally
                ' Cierre de fichero
                arrFile = punchesStream.ToArray()

                punchesStream.Dispose()

                ' Libera memoria
                If Not dt Is Nothing Then dt.Dispose()
            End Try

            Return arrFile
        End Function


    End Class

End Namespace