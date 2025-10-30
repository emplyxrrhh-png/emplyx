Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class roCustomIberper
        Inherits roDataLinkExport
        Private Enum IberperFiles
            Accruals = 0
            ProgAbsences = 1
            Scheduler = 2
            Canteen = 3
        End Enum

        Public Shared Function ASCIIExportIberper(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal Separator As String, ByVal nIdExport As Integer, ByRef oState As roDataLinkState) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim strlogevent As String = ""
            Dim msgLog As String = ""
            Dim zip As ZipExport = Nothing
            Dim zipPath As String = String.Empty
            Dim NameFile As String


            Try
                NameFile = "Ibermatica" & "." & Now.ToString("yyyyMMddHHss") & ".zip"
                Dim bolRet As Boolean = True
                zip = New ZipExport(NameFile)

                'Inicio de la exportación
                strlogevent = Now.ToString & " --> " & oState.Language.Translate("ExportIberper.LogEvent.Start", "") & vbNewLine

                If Separator = "" Then Separator = ";"

                ' Comprueba campos de la ficha necesarios
                ' USR_Iberper_Comedor
                Dim queryString As String = "@SELECT# Name FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Groups]')  AND name = 'USR_Iberper_Comedor'"
                If Any2String(Robotics.DataLayer.AccessHelper.ExecuteScalar(queryString)) = "" Then
                    oState.Result = DataLinkResultEnum.FieldIberper_ComedorNotExists
                    strlogevent &= Now.ToString & " --> " & oState.Language.Translate("ResultEnum.FieldIberper_ComedorNotExists", "") & vbNewLine
                    Return {}
                End If

                ' Abre los ficheros iberper
                Dim iberperExports As New Hashtable

                ' Exporta saldos
                bolRet = ASCIIExportIberper_Accruals(mEmployees, dtBeginDate, dtEndDate, Separator, iberperExports, oState)

                ' Exporta ausencias prolongadas
                If bolRet Then bolRet = ASCIIExportIberper_ProgAbsences(mEmployees, dtBeginDate, dtEndDate, Separator, iberperExports, oState)

                ' Exporta horarios
                If bolRet Then bolRet = ASCIIExportIberper_Scheduler(mEmployees, dtBeginDate, dtEndDate, Separator, iberperExports, oState)

                ' Exporta comedor
                If bolRet Then bolRet = ASCIIExportIberper_Canteen(mEmployees, dtBeginDate, dtEndDate, Separator, iberperExports, oState)



                If bolRet Then
                    zip.zipExportFile.AddEntry("Ibermatica.txt", iberperExports(IberperFiles.Accruals))

                    zip.zipExportFile.AddEntry("ProgAbsen.txt", iberperExports(IberperFiles.ProgAbsences))

                    zip.zipExportFile.AddEntry("Horarios.txt", iberperExports(IberperFiles.Scheduler))

                    zip.zipExportFile.AddEntry("Comedor.txt", iberperExports(IberperFiles.Canteen))

                    zip.SaveFile()

                    ' Devuelve array de bytes
                    arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                    Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

                    ' Exportación finalizada
                    strlogevent += Now.ToString & " --> " & oState.Language.Translate("ExportIberper.LogEvent.Finish", "") & vbNewLine & msgLog & vbNewLine
                End If

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportIberper")
                strlogevent += Now.ToString & " --> " & oState.Language.Translate("Export.LogEvent.UnknownError", "") & vbNewLine & ex.Message & vbNewLine
            Finally
                ' Graba log
                SaveExportLog(nIdExport, strlogevent & msgLog & vbNewLine)
            End Try

            Return arrFile

        End Function

        Private Shared Function ASCIIExportIberper_Accruals(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal Separator As String, ByRef iberperExports As Hashtable, ByRef oState As roDataLinkState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim line As String = ""
                Dim queryString As String = "" &
               "@SELECT# DA.IDEmployee, C.Name, C.Export, C.ShortName, SUM(DA.Value) AS total, " &
               "   (@SELECT# VALUE from GetAllEmployeeUserFieldValue('Iberper','" & Format$(dtEndDate, "yyyyMMdd") & "') WHERE idEmployee=DA.IDEmployee  ) as CodIberper " &
               "FROM DailyAccruals DA " &
               "   INNER JOIN Concepts C ON DA.IDConcept = C.ID " &
               "WHERE DA.idEmployee in (" & mEmployees.ToString & ") " &
                "   AND (DA.Date BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") " &
                "   AND C.Export<>'0' and C.Export<>'' " &
               "GROUP BY DA.IDEmployee, C.Name, C.Export, C.ShortName " &
               "Order by DA.IDEmployee"
                Dim dt As DataTable = CreateDataTable(queryString, "Accruals")


                Dim strLines As New List(Of String)
                For Each row As DataRow In dt.Rows
                    line = Month(dtEndDate).ToString.PadLeft(2) & Separator
                    line += Any2String(row("CodIberper")).PadRight(12) & Separator
                    line += "  " & Separator
                    line += row("ShortName").ToString.PadRight(3) & Separator
                    line += Format(row("Total"), "0.00").ToString.PadLeft(10) & Separator

                    strLines.Add(line)
                Next

                iberperExports.Add(IberperFiles.Accruals, GenerateStreamFromString(strLines))

                dt.Dispose()

                bolRet = True

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportIberper_Accruals")
            End Try

            Return bolRet
        End Function

        Private Shared Function ASCIIExportIberper_ProgAbsences(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal Separator As String, ByRef iberperExports As Hashtable, ByRef oState As roDataLinkState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim line As String = ""

                ' Selecciona datos
                Dim queryString As String = "" &
              "@SELECT# PA.BeginDate, PA.FinishDate, PA.IDCause, C.Name , C.Description, C.ShortName, " &
              " (@SELECT# VALUE from GetAllEmployeeUserFieldValue('Iberper','" & Format$(dtEndDate, "yyyyMMdd") & "') WHERE idEmployee=PA.IDEmployee  ) as CodIberper " &
              "FROM ProgrammedAbsences PA " &
              "   INNER JOIN Causes C ON PA.IDCause = C.ID " &
              "WHERE PA.idEmployee in (" & mEmployees.ToString & ") " &
              "   and PA.FinishDate >= " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & " and PA.BeginDate <= " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime &
              "   and C.Description like'%Export=%'" &
              "Order by PA.IDEmployee"


                Dim strLines As New List(Of String)
                Dim dt As DataTable = CreateDataTable(queryString, "Accruals")
                For Each row As DataRow In dt.Rows
                    line = Any2String(row("CodIberper")).PadRight(12) & Separator
                    line += "  " & Separator
                    line += ASCIIExportIberper_Scheduler_NombreCortoDeterminar(row("Description")).ToString.PadRight(3) & Separator
                    line += Format(row("Begindate"), "yyyy/MM/dd") & Separator
                    If Not IsDBNull(row("FinishDate")) Then
                        line += Format(row("FinishDate"), "yyyy/MM/dd") & Separator
                    Else
                        line += Space(10) & Separator
                    End If

                    line += Space(10) & Separator & " " & Separator

                    strLines.Add(line)
                Next

                iberperExports.Add(IberperFiles.ProgAbsences, GenerateStreamFromString(strLines))

                dt.Dispose()

                bolRet = True

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportIberper_ProgAbsences")
            End Try

            Return bolRet
        End Function

        Private Shared Function ASCIIExportIberper_Scheduler(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal Separator As String, ByRef iberperExports As Hashtable, ByRef oState As roDataLinkState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim FecIni As Date
                Dim FecFin As Date
                Dim FecDomingo As Date
                Dim NextDate As Date
                Dim UltHor As String = ""
                Dim ShortName As String = ""
                Dim ExportSemanasNaturales As Boolean
                Dim IndicarSemanaNatural As Boolean
                Dim n As Long = 0
                Dim EmpAnt As Integer = 0
                Dim CodIberper As String = ""

                ' Selecciona datos                
                Dim queryString As String = "" &
                "@SELECT# DS.idEmployee, DS.Date, isnull(DS.IDShiftUsed,DS.IDShift1) , S.Name , S.ShortName, S.Description, " &
                " (@SELECT# VALUE from GetAllEmployeeUserFieldValue('Iberper','" & Format$(dtEndDate, "yyyyMMdd") & "') WHERE idEmployee=DS.IDEmployee  ) as CodIberper " &
                "FROM DailySchedule DS " &
                "   INNER JOIN Shifts S ON isnull(DS.IDShiftUsed,DS.IDShift1) = S.ID " &
                "WHERE DS.idEmployee in (" & mEmployees.ToString & ") " &
                "   AND (DS.Date BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") " &
                "   AND NOT (isnull(DS.IDShiftUsed,DS.IDShift1) IS NULL) AND (S.Description LIKE '%EXPORT=%') " &
                "ORDER BY DS.idEmployee, DS.Date "

                Dim strLines As New List(Of String)
                Dim dt As DataTable = CreateDataTable(queryString, "Accruals")
                For Each row As DataRow In dt.Rows
                    If EmpAnt = 0 Then EmpAnt = row("idEmployee")

                    ' Comprueba si cambia el empleado
                    If EmpAnt <> row("idEmployee") Then
                        ' Graba la linea
                        strLines.Add(ASCIIExportIberper_Scheduler_GrabarLinea(Separator, CodIberper, ShortName, ExportSemanasNaturales, IndicarSemanaNatural, FecIni, FecFin))
                        EmpAnt = row("idEmployee")
                        UltHor = ""
                    End If

                    ' Primer registro
                    If UltHor = "" Or EmpAnt <> row("idEmployee") Then
                        FecIni = row("Date")
                        FecFin = row("Date")
                        NextDate = row("Date")
                        FecDomingo = DateAdd("d", 7 - Weekday(row("Date"), vbMonday), row("Date"))

                        ASCIIExportIberper_Scheduler_GetNombreCortoHorario(row("Description"), ShortName, ExportSemanasNaturales, IndicarSemanaNatural)
                        UltHor = ASCIIExportIberper_Scheduler_NombreCortoDeterminar(row("Description"))
                        CodIberper = Any2String(row("CodIberper"))
                    End If

                    ' Comprueba si se acaba la semana o cambia el horario
                    'If UltHor <> ASCIIExportIberper_Scheduler_NombreCortoDeterminar(row("Description")) Or row("Date") > FecDomingo Or row("Date") > NextDate Or (ExportSemanasNaturales = False And n > 0) Then
                    If UltHor <> ASCIIExportIberper_Scheduler_NombreCortoDeterminar(row("Description")) Or (ExportSemanasNaturales And row("Date") > FecDomingo) Or (row("Date") > NextDate) Then
                        ' Graba la linea
                        strLines.Add(ASCIIExportIberper_Scheduler_GrabarLinea(Separator, CodIberper, ShortName, ExportSemanasNaturales, IndicarSemanaNatural, FecIni, FecFin))

                        ' Asigna datos de control
                        FecIni = row("Date")
                        FecFin = row("Date")
                        NextDate = row("Date")
                        FecDomingo = DateAdd("d", 7 - Weekday(row("Date"), vbMonday), row("Date"))

                        ASCIIExportIberper_Scheduler_GetNombreCortoHorario(row("Description"), ShortName, ExportSemanasNaturales, IndicarSemanaNatural)
                        UltHor = ASCIIExportIberper_Scheduler_NombreCortoDeterminar(row("Description"))
                    Else
                        FecFin = row("Date")
                    End If

                    NextDate = DateAdd("d", 1, NextDate)
                    n = n + 1
                Next

                ' Graba la linea
                If UltHor <> "" Then
                    strLines.Add(ASCIIExportIberper_Scheduler_GrabarLinea(Separator, CodIberper, ShortName, ExportSemanasNaturales, IndicarSemanaNatural, FecIni, FecFin))
                End If


                iberperExports.Add(IberperFiles.Scheduler, GenerateStreamFromString(strLines))

                dt.Dispose()
                bolRet = True

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportIberper_Scheduler")
            End Try

            Return bolRet
        End Function

        Private Shared Function ASCIIExportIberper_Scheduler_GrabarLinea(ByVal Separator As String, ByVal CodIberper As String, ByVal ShortName As String, ByVal ExportSemanasNaturales As Boolean, ByVal IndicarSemanaNatural As Boolean, ByVal FecIni As Date, ByVal FecFin As Date) As String
            Dim Line As String
            Dim FecPrimMes As Date

            ' Datos
            Line = CodIberper.PadRight(12) & Separator
            Line += "  " & Separator

            ' Añade el numero de semana
            If ExportSemanasNaturales Then
                FecPrimMes = CDate("01/" & DatePart("M", FecIni) & "/" & DatePart("yyyy", FecIni))

                If IndicarSemanaNatural Then
                    Line += Left$(ShortName & "   ", 2) & DatePart("ww", FecIni, vbMonday, vbFirstJan1) - DatePart("ww", FecPrimMes, vbMonday, vbFirstJan1) + 1 & Separator
                Else
                    Line += Left$(ShortName & "   ", 3) & Separator
                End If

            Else
                Line += Left$(ShortName & "   ", 3) & Separator
            End If

            Line += Format$(FecIni, "yyyy/MM/dd") & Separator
            Line += Format$(FecFin, "yyyy/MM/dd") & Separator
            Line += Space(10) & Separator & Separator

            ' Graba el registro
            Return Line
        End Function

        Private Shared Sub ASCIIExportIberper_Scheduler_GetNombreCortoHorario(ByVal Str As String, ByRef NombreCorto As String, ByRef ExportSemanasNaturales As Boolean, ByRef IndicarSemanaNatural As Boolean)
            Dim j As Integer
            Dim aux$

            ' Comprueba si tiene que exportar por semanas naturales por # o por @
            j = InStr(1, Str, "#")
            IndicarSemanaNatural = (j > 0)

            ' Si no exporta semanas naturales indicando la semana (#) comprueba si tiene que exportar semanas naturales sin indicar la semana (@)
            If Not IndicarSemanaNatural Then
                j = InStr(1, Str, "@")
                ExportSemanasNaturales = (j > 0)
            Else
                ExportSemanasNaturales = True
            End If

            ' Determina el nombre corto
            aux = ASCIIExportIberper_Scheduler_NombreCortoDeterminar(Str)
            If IndicarSemanaNatural Then aux = Mid$(aux, 1, j - 1)

            NombreCorto = aux
        End Sub

        Private Shared Function ASCIIExportIberper_Scheduler_NombreCortoDeterminar(ByVal Str As String) As String
            Dim i As Integer
            Str = UCase(Str)

            ' Busca en la descripcion
            i = InStr(1, Str, "EXPORT=")

            ' Selecciona la descripción corta
            If i > 0 Then
                Str = Trim(Mid$(Str & "   ", i + 7, 3))
            End If

            Return Str
        End Function

        Private Shared Function ASCIIExportIberper_Canteen(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal Separator As String, ByRef iberperExports As Hashtable, ByRef oState As roDataLinkState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim strDateEnd As String = Format$(dtEndDate, "yyyyMMdd")
                Dim line As String = ""

                ' Selecciona las empresas
                Dim dtBuss As DataTable = CreateDataTable("@SELECT# id, USR_Iberper_Comedor from groups where CHARINDEX('\',path)=0")

                ' Selecciona datos
                Dim queryString As String = "" &
                "@SELECT# P.idEmployee, G.Path, COUNT(IDEmployee) AS Total, " &
                "   (@SELECT# VALUE from GetAllEmployeeUserFieldValue('Iberper','" & strDateEnd & "') WHERE idEmployee=P.IDEmployee  ) as CodIberper " &
                "FROM Punches P " &
                "   inner join Groups G on G.ID=dbo.GetEmployeeGroup (P.IDEmployee, '" & strDateEnd & "') " &
                "WHERE idEmployee in (" & mEmployees.ToString & ") " &
                "   AND (ShiftDate BETWEEN " & roTypes.Any2Time(dtBeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") " &
                "   and Type = 10 AND (InvalidType IS NULL or InvalidType=0 or InvalidType=7) " &
                "GROUP BY P.idEmployee,G.Path"

                Dim strLines As New List(Of String)
                Dim dt As DataTable = CreateDataTable(queryString, "Accruals")
                For Each row As DataRow In dt.Rows
                    line = Format$(dtEndDate, "MM") & Separator
                    line += Any2String(row("CodIberper")).PadRight(12) & Separator
                    line += "  " & Separator
                    line += GetBusinessField(dtBuss, row("path"), "USR_Iberper_Comedor") & Separator
                    line += Any2String(row("total")).PadRight("10") & Separator

                    strLines.Add(line)
                Next
                iberperExports.Add(IberperFiles.Canteen, GenerateStreamFromString(strLines))

                dt.Dispose()

                bolRet = True

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportIberper_Canteen")
            End Try

            Return bolRet
        End Function

        Private Shared Function GetBusinessField(ByVal dtBussiness As DataTable, ByVal Path As String, ByVal FieldName As String) As String
            Dim aux As String = ""

            Dim idGroup As String = Path.Split("\")(0)
            If idGroup <> "" Then
                Dim rows As DataRow() = dtBussiness.Select("id=" & idGroup)
                If rows.Length > 0 Then aux = roTypes.Any2String(rows(0)(FieldName))
            End If

            Return aux

        End Function
    End Class

End Namespace