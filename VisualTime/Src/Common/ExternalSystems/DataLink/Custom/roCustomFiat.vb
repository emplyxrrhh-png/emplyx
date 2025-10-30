Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.Azure

Namespace DataLink


    Public Class roCustomFiat
        Inherits roDataLinkExport

#Region "10005/10007/10014/10015 -Exportación FIAT - MDO Madrid"
        Public Shared Function ASCIIExportFIATMDOMadridv2(ByVal employees As String, ByVal beginDate As Date, ByVal endDate As Date, ByRef state As roDataLinkState, ByVal delimiterChar As String, ByVal isDaily As Boolean, ByVal onlyCopyFile As Boolean, ByVal isMirror As Boolean) As Byte()
            '
            ' MDO Madrid Mensual/Diaria
            '

            Dim arrFile As Byte() = Nothing
            Dim cn As DbConnection = Nothing
            Dim ret As Boolean = True

            Try

                Dim nameFile As String = ""
                Dim strLine As String = ""
                Dim strLines As List(Of String) = New List(Of String)
                Dim exportStream As MemoryStream = New MemoryStream()
                Dim sql As String = ""
                Dim isApplyConguallo As Boolean = False
                Dim existHTM As Boolean = False


                Dim settings As New roSettings
                Dim fileCopyName As String = ""

                ' Obtenemos el nombre del fichero de copia
                If isDaily Then
                    fileCopyName = "tmp/MDODaily_" & Format(beginDate, "yyyyMMdd") & Format(endDate, "yyyyMMdd") & ".txt"
                Else
                    If Not isMirror Then
                        fileCopyName = "tmp/MDOMonthly_" & Format(beginDate, "yyyyMMdd") & Format(endDate, "yyyyMMdd") & ".txt"
                    Else
                        fileCopyName = "tmp/MDOMonthlyMirror_" & Format(beginDate, "yyyyMMdd") & Format(endDate, "yyyyMMdd") & ".txt"
                    End If

                End If


                If onlyCopyFile Then
                    ' EN ESTE CASO SOLO TENEMOS QUE OBTENER EL FICHERO YA GENERADO Y HACER UNA COPIA
                    Try
                        ' Devuelve array de bytes
                        arrFile = Azure.RoAzureSupport.DownloadFileFromCompanyContainer(fileCopyName, "", DTOs.roLiveQueueTypes.datalink)
                        Azure.RoAzureSupport.DeleteFileFromAzure(fileCopyName & ".bak", DTOs.roLiveQueueTypes.datalink)
                        Azure.RoAzureSupport.RenameFileInCompanyContainer(fileCopyName, fileCopyName & ".bak", "", DTOs.roLiveQueueTypes.datalink)
                        Return arrFile
                    Catch ex As Exception
                        state.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportFIATMDOMadridv2")
                        Return arrFile
                    End Try
                End If

                ' Crea el fichero y cabecera
                If isDaily Then
                    strLine = "***PREMDOGG"
                    nameFile = "DataLinkExportFIATMDOLGTMadridDiaria.TXT"
                Else
                    nameFile = "DataLinkExportFIATMDOLGTMadridMensual.TXT"
                    If Not isMirror Then
                        strLine = "***PREMDOMM"
                    Else
                        strLine = "***PREMDOM3"
                    End If
                End If

                strLine += Format(Now.Date, "yyyyMMdd")
                strLine += Format(beginDate, "yyyyMMdd")
                strLine += Format(endDate, "yyyyMMdd")
                strLine += "00000"
                strLine += "000000000000" & Space(8)
                strLines.Add(strLine)

                ' El periodo siempre tiene que ser del mismo mes y empezar el dia 1
                If beginDate.Month <> endDate.Month Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "Invalid Period, not same month")
                    state.Result = DataLinkResultEnum.IVECOInvalidPeriod
                    ret = False
                End If

                If beginDate.Day <> 1 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "Invalid Begin Period")
                    state.Result = DataLinkResultEnum.IVECOInvalidBeginPeriod
                    ret = False
                End If

                ' Si es MENSUAL , revisamos que el periodo sea el de todo un mes
                If Not isDaily Then
                    Dim iPastMonth As Integer = 0
                    If beginDate.Month = 1 Then
                        iPastMonth = 12
                    Else
                        iPastMonth = beginDate.Month - 1
                    End If
                    ' Si el mes del dia anterior al del inicio no es igual al mes anterior al dia del inicio del periodo
                    '' If dtBeginDate.AddDays(-1).Month <> dtBeginDate.Month - 1 Then
                    If beginDate.AddDays(-1).Month <> iPastMonth Then
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "Invalid Period")
                        state.Result = DataLinkResultEnum.IVECOInvalidPeriod
                        ret = False
                    End If

                    Dim iNextMonth As Integer = 0
                    If endDate.Month = 12 Then
                        iNextMonth = 1
                    Else
                        iNextMonth = endDate.Month + 1
                    End If

                    If endDate.AddDays(1).Month <> iNextMonth Then
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "Invalid Period")
                        state.Result = DataLinkResultEnum.IVECOInvalidPeriod
                        ret = False
                    End If
                End If


                If isMirror Then
                    ' En el caso que estemos exportando datos de la tabla espejo, debemos generar los datos con los mismos empleados
                    ' exportados anteriormente a traves de la exportacion de MDO Madrid Mensual
                    employees = Any2String(ExecuteScalar("@SELECT# isnull(Field_4, '') FROM ExportGuides WHERE ID=20016"))
                    If employees IsNot Nothing AndAlso employees.Length > 0 AndAlso employees.EndsWith(",") Then
                        employees = employees.TrimEnd(","c)
                    End If

                End If

                If employees.Length = 0 Then
                    roLog.GetInstance().logMessage(roLog.EventType.roError, "Empty Employees")
                    state.Result = DataLinkResultEnum.IVECOEmptyEmployees
                    ret = False
                End If


                If ret Then

                    ' COMPROBAMOS SI HAY QUE APLICAR EL CONGUALLO
                    If beginDate.Day <= 15 And endDate.Day >= 15 Then
                        isApplyConguallo = True
                    End If

                    If Not isDaily And Not isMirror Then
                        ' ******* MENSUAL: HAY QUE GUARDAR EL TOTAL DE HORAS TRABAJADAS POR EMPLEADO Y CENTRO
                        ' ******* EL ULTIMO DIA DEL MES

                        ' FECHA DEL ULTIMO DIA DEL MES
                        Dim xDate As Date = endDate
                        xDate = DateSerial(xDate.Year, xDate.Month, 1)
                        xDate = xDate.AddMonths(1).AddDays(-1)

                        If delimiterChar = "[" Then
                            ' ELIMINAMOS LAS HORAS TRABAJADAS DEL MES POR EMPLEADO Y CENTRO GUARDADOS, EN CASO QUE EL SEPARADOR SEA [
                            sql = "@DELETE# FROM MDOMonthlyValues WHERE Date=" & Any2Time(xDate).SQLSmallDateTime
                            sql = sql & " AND MDOMonthlyValues.Type = 1"
                            ExecuteSqlWithoutTimeOut(sql)
                            roLog.GetInstance().logMessage(roLog.EventType.roInfo, "DELETED HTM VALUE")


                            ' ADEMAS DE ELIMINAR DE LAS TABLAS ESPEJO EL PERIODO SELECCIONADO
                            sql = "@DELETE# FROM DailyCauses_MIRROR WHERE Date >=" & Any2Time(beginDate).SQLSmallDateTime
                            sql = sql & " AND Date <=" & Any2Time(endDate).SQLSmallDateTime
                            ExecuteSqlWithoutTimeOut(sql)

                            sql = "@DELETE# FROM DailyIncidences_MIRROR WHERE Date >=" & Any2Time(beginDate).SQLSmallDateTime
                            sql = sql & " AND Date <=" & Any2Time(endDate).SQLSmallDateTime
                            ExecuteSqlWithoutTimeOut(sql)

                            roLog.GetInstance().logMessage(roLog.EventType.roInfo, "DELETED MIRROR TABLES on period: " & beginDate & " " & endDate)

                        End If


                        sql = "@SELECT# isnull(COUNT(*), 0) FROM MDOMonthlyValues WHERE Date=" & Any2Time(xDate).SQLSmallDateTime
                        sql = sql & " AND MDOMonthlyValues.Type = 1"

                        existHTM = IIf(Any2Double(ExecuteScalar(sql)) > 0, True, False)
                        If Not existHTM Then
                            ' SI NO EXISTE NINGUN VALOR DE HORAS TRABAJADAS MES, 
                            ' GUARDAMOS LAS HORAS TRABAJADAS DEL MES POR EMPLEADO Y CENTRO
                            sql = "@INSERT# INTO MDOMonthlyValues (IDEmployee, Date, Type, IDCenter, Value, Matricula)  "
                            sql = sql & " @SELECT# DailyCauses.IDEmployee," & Any2Time(xDate).SQLSmallDateTime & ",1, DailyCauses.IDCenter , SUM(CONVERT(NUMERIC(18,2), DailyCauses.Value)) AS Total "
                            sql = sql & " , (@SELECT# top(1) Value from EmployeeUserFieldValues where FieldName='111MATRÍCULA' and EmployeeUserFieldValues.idEmployee=DailyCauses.IDEmployee and EmployeeUserFieldValues.Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") as Matricula   "
                            sql = sql & " FROM dbo.DailyCauses"
                            sql = sql & " INNER JOIN Employees ON Employees.ID = DailyCauses.IDEmployee"
                            sql = sql & " WHERE Employees.ID IN (" & employees.ToString & ")"
                            sql = sql & " AND  DailyCauses.Date >= " & Any2Time(beginDate).SQLSmallDateTime
                            sql = sql & " AND  DailyCauses.Date <= " & Any2Time(endDate).SQLSmallDateTime
                            sql = sql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                            sql = sql & " AND DailyCauses.IDCenter > 0"
                            sql = sql & " AND DailyCauses.AccrualsRules = 0"
                            sql = sql & " GROUP BY DailyCauses.IDEmployee, DailyCauses.IDCenter"
                            ExecuteSqlWithoutTimeOut(sql)

                            ' ADEMAS GENERAMOS LA FOTO CON LOS DATOS DE LAS TABLA DAILYCAUSES Y DAILYINCIDENCES DEL PERIODO INDICADO
                            ' Y DE LOS EMPLEADOS Y JUSTIFICACIONES DEL SALDO 15M 
                            ' PARA QUE POSTERIORMENTE WORKANALYSIS UTILICE ESOS DATOS
                            sql = "@INSERT# INTO DailyCauses_MIRROR (IDEmployee, Date, IDRelatedIncidence, IDCause, Value, AccrualsRules, Manual , IDCenter, DefaultCenter, ManualCenter, DailyRule, AccruedRule )  "
                            sql = sql & " @SELECT# IDEmployee, Date, IDRelatedIncidence, IDCause, Value, AccrualsRules, Manual , IDCenter, DefaultCenter, ManualCenter, DailyRule, AccruedRule FROM dbo.DailyCauses"
                            sql = sql & " INNER JOIN Employees ON Employees.ID = DailyCauses.IDEmployee"
                            sql = sql & " WHERE Employees.ID IN (" & employees.ToString & ")"
                            sql = sql & " AND  DailyCauses.Date >= " & Any2Time(beginDate).SQLSmallDateTime
                            sql = sql & " AND  DailyCauses.Date <= " & Any2Time(endDate).SQLSmallDateTime
                            sql = sql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                            sql = sql & " AND DailyCauses.IDCenter > 0"
                            sql = sql & " AND DailyCauses.AccrualsRules = 0"
                            ExecuteSqlWithoutTimeOut(sql)

                            sql = "@INSERT# INTO DailyIncidences_MIRROR (ID, IDEmployee, Date, IDType, IDZone, Value, BeginTime , EndTime, IDCenter, DefaultCenter )  "
                            sql = sql & " @SELECT# DISTINCT DailyIncidences.ID, DailyIncidences.IDEmployee, DailyIncidences.Date, DailyIncidences.IDType, DailyIncidences.IDZone, DailyIncidences.Value, DailyIncidences.BeginTime , DailyIncidences.EndTime, DailyIncidences.IDCenter, DailyIncidences.DefaultCenter FROM dbo.DailyIncidences"
                            sql = sql & " INNER JOIN Employees ON Employees.ID = DailyIncidences.IDEmployee"
                            sql = sql & " INNER JOIN DailyCauses_MIRROR ON DailyCauses_MIRROR.IDRelatedIncidence = DailyIncidences.ID"
                            sql = sql & " AND DailyCauses_MIRROR.IDEmployee = DailyIncidences.IDEmployee"
                            sql = sql & " AND DailyCauses_MIRROR.Date = DailyIncidences.Date"
                            sql = sql & " WHERE Employees.ID IN (" & employees.ToString & ")"
                            sql = sql & " AND  DailyCauses_MIRROR.Date >= " & Any2Time(beginDate).SQLSmallDateTime
                            sql = sql & " AND  DailyCauses_MIRROR.Date <= " & Any2Time(endDate).SQLSmallDateTime
                            ExecuteSqlWithoutTimeOut(sql)

                            ' Nos guardamos los empleados exportados para posteriormente utilizarlos para enviar los mismos empleados a WorkAnalysis
                            sql = "@UPDATE# ExportGuides Set Field_4='" & employees.ToString & "' WHERE ID=20016"
                            ExecuteSqlWithoutTimeOut(sql)


                        Else
                            roLog.GetInstance().logMessage(roLog.EventType.roInfo, "EXISTS HTM VALUE, NOT SAVED HTM DATA, NOT SAVE MIRROR TABLES")
                        End If
                    End If

                    If isDaily Then
                        ' *** DIARIA
                        ' EN EL CASO QUE DENTRO DEL PERIODO SE EXPORTE EL DIA 15
                        ' HAY QUE GENERAR EL CONGUALLO Y GUARDAR EL VALOR, SOLO EN EL CASO QUE NO EXSITA YA
                        If isApplyConguallo Then

                            ' FECHA EN LA QUE SE DEBE GUARDAR EL CONGUALLO DEL MES ANTERIOR,
                            ' ULTIMO DIA DEL MES ANTERIOR
                            Dim xDate As Date = beginDate
                            xDate = DateSerial(xDate.Year, xDate.Month, 1)
                            xDate = xDate.AddDays(-1)

                            ' FECHA INICIO DEL MES ANTERIOR
                            Dim tBeginDate As Date = xDate
                            tBeginDate = DateSerial(tBeginDate.Year, tBeginDate.Month, 1)


                            Dim BolExistHCM As Boolean = False
                            sql = "@SELECT# isnull(COUNT(*), 0) FROM MDOMonthlyValues WHERE Date=" & Any2Time(xDate).SQLSmallDateTime
                            sql = sql & " AND MDOMonthlyValues.Type = 2"
                            BolExistHCM = IIf(Any2Double(ExecuteScalar(sql)) > 0, True, False)
                            If Not BolExistHCM Then
                                ' EN CASO QUE NO EXISTA NINGUN VALOR 

                                ' CALCULAMOS EL CONGUALLO Y LO GUARDAMOS EL ULTIMO DIA DEL MES ANTERIOR

                                ' PRIMERO DE LOS EMPLEADOS QUE GENERARON HTM EN EL MOMENTO DEL CIERRE
                                sql = "@SELECT# IDEmployee, IDCenter,  SUM(CONVERT(NUMERIC(18,2),Value)) AS Total FROM dbo.MDOMonthlyValues "
                                sql = sql & " WHERE "
                                sql = sql & " Date = " & Any2Time(xDate).SQLSmallDateTime
                                sql = sql & " AND Type = 1"
                                sql = sql & " GROUP BY IDEmployee, IDCenter"
                                sql = sql & " ORDER BY IDEmployee, IDCenter"

                                Dim dt As DataTable = CreateDataTableWithoutTimeouts(sql, )

                                For Each row As System.Data.DataRow In dt.Rows
                                    ' Para cada Empleado y centro , obtenemos los VALORS ACTUALES del mes anterior
                                    sql = "@SELECT# SUM(CONVERT(NUMERIC(18,2),DailyCauses.Value)) AS Total FROM dbo.DailyCauses"
                                    sql = sql & " WHERE DailyCauses.IDEmployee = " & row("IDEmployee").ToString
                                    sql = sql & " AND DailyCauses.IDCenter=" & row("IDCenter").ToString
                                    sql = sql & " AND DailyCauses.Date >= " & Any2Time(tBeginDate).SQLSmallDateTime
                                    sql = sql & " AND  DailyCauses.Date <= " & Any2Time(xDate).SQLSmallDateTime
                                    sql = sql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                                    sql = sql & " AND DailyCauses.AccrualsRules = 0"
                                    Dim dblTotalM As Double = Any2Double(ExecuteScalar(sql))

                                    ' CALCULAMOS EL CONGUALLO PARA EMPLEADO/CENTRO
                                    Dim dblConguallo As Double = dblTotalM - Any2Double(row("Total"))

                                    'GUARDAMOS EL CONGUALLO EL ULTIMO DIA DEL MES ANTERIOR
                                    sql = "@INSERT# INTO dbo.MDOMonthlyValues (IDEmployee, Date, Type, IDCenter, Value, Matricula)  VALUES ("
                                    sql = sql & row("IDEmployee") & "," & Any2Time(xDate).SQLSmallDateTime & ",2," & row("IDCenter").ToString & "," & Any2String(dblConguallo).Replace(",", ".") & ",(@SELECT# top(1) Value from EmployeeUserFieldValues where FieldName='111MATRÍCULA' and EmployeeUserFieldValues.idEmployee=" & row("IDEmployee").ToString & " and EmployeeUserFieldValues.Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") )"
                                    ExecuteSqlWithoutTimeOut(sql)

                                Next

                                ' DESPUES LOS EMPLEADOS QUE NO GENERARON HTM EN EL MOMENTO DEL CIERRE Y AHORA SI
                                sql = "@SELECT# DailyCauses.IDEmployee, DailyCauses.IDCenter,  SUM(CONVERT(NUMERIC(18,2),DailyCauses.Value)) AS Total FROM dbo.DailyCauses"
                                sql = sql & " WHERE DailyCauses.IDEmployee IN (" & employees.ToString & ")"
                                sql = sql & " AND NOT EXISTS (@SELECT# 1 as Tot FROM MDOMonthlyValues x where x.Date=" & Any2Time(xDate).SQLSmallDateTime & " AND x.type = 1  and x.IDCenter = dailycauses.IDCenter and x.IDEmployee = DailyCauses.IDEmployee)"
                                sql = sql & " AND DailyCauses.Date >= " & Any2Time(tBeginDate).SQLSmallDateTime
                                sql = sql & " AND  DailyCauses.Date <= " & Any2Time(xDate).SQLSmallDateTime
                                sql = sql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                                sql = sql & " AND DailyCauses.IDCenter > 0"
                                sql = sql & " AND DailyCauses.AccrualsRules = 0"
                                sql = sql & " GROUP BY DailyCauses.IDEmployee, DailyCauses.IDCenter"
                                sql = sql & " ORDER BY DailyCauses.IDEmployee, DailyCauses.IDCenter"

                                dt = CreateDataTableWithoutTimeouts(sql, )
                                For Each row As System.Data.DataRow In dt.Rows
                                    ' Para cada Empleado y centro , generamos el valor del conguallo
                                    ' unicamente con el valor actual del mes
                                    Dim dblConguallo As Double = Any2Double(row("Total"))

                                    'GUARDAMOS EL CONGUALLO EL ULTIMO DIA DEL MES ANTERIOR
                                    Try
                                        sql = "@INSERT# INTO dbo.MDOMonthlyValues (IDEmployee, Date, Type, IDCenter, Value, Matricula)  VALUES ("
                                        sql = sql & row("IDEmployee") & "," & Any2Time(xDate).SQLSmallDateTime & ",2," & row("IDCenter").ToString & "," & Any2String(dblConguallo).Replace(",", ".") & ",(@SELECT# top(1) Value from EmployeeUserFieldValues where FieldName='111MATRÍCULA' and EmployeeUserFieldValues.idEmployee=" & row("IDEmployee").ToString & " and EmployeeUserFieldValues.Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") )"
                                        ExecuteSqlWithoutTimeOut(sql)
                                    Catch ex As Exception
                                    End Try
                                Next

                                ' Libera memoria
                                dt.Dispose()
                            End If
                        End If
                    End If


                    '''''  ***************************************************************************************
                    ' Eliminamos los datos de la tabla temporal
                    ExecuteSql("@DELETE# FROM TMPMDO")


                    If Not isDaily And Not isMirror Then
                        ' MENSUAL 
                        If Not existHTM Then
                            ' Si es la primera vez que se cargan los datos de HTM, obtenemos los datos de la tabla espejo ya que los datos guardados en esa tabla son la foto del mes
                            ' y podemos obtener los datos de forma diaria
                            sql = "@INSERT# INTO TMPMDO (IDCenter, Date, Value, Center) "
                            sql = sql & "@SELECT# DailyCauses.IDCenter, DailyCauses.Date, SUM(CONVERT(NUMERIC(18,2),DailyCauses.Value)) AS Total, (@SELECT# Name FROM BusinessCenters WHERE ID = DailyCauses.IDCenter)  as Center FROM dbo.DailyCauses"
                            sql = sql & " INNER JOIN Employees ON Employees.ID = DailyCauses.IDEmployee"
                            sql = sql & " WHERE Employees.ID IN (" & employees.ToString & ")"
                            sql = sql & " AND  DailyCauses.Date >= " & Any2Time(beginDate).SQLSmallDateTime
                            sql = sql & " AND  DailyCauses.Date <= " & Any2Time(endDate).SQLSmallDateTime
                            sql = sql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                            sql = sql & " AND DailyCauses.IDCenter > 0"
                            sql = sql & " AND DailyCauses.AccrualsRules = 0"
                            sql = sql & " GROUP BY DailyCauses.IDCenter, Date"
                            sql = sql & " ORDER BY DailyCauses.IDCenter, Date"

                            sql = sql.Replace("DailyCauses", "DailyCauses_MIRROR")

                            ExecuteSqlWithoutTimeOut(sql)

                        Else
                            ' OBTENEMOS LOS DATOS DE HTM DEL ULTIMO DIA DEL MES, ya que los datos originales ya se cargaron en su momento 
                            ' FECHA DEL ULTIMO DIA DEL MES
                            Dim xDate As Date = endDate
                            xDate = DateSerial(xDate.Year, xDate.Month, 1)
                            xDate = xDate.AddMonths(1).AddDays(-1)

                            ' INSERTAMOS LOS VALORES EN LA TABLA TEMPORAL
                            sql = "@INSERT# INTO TMPMDO (IDCenter, Date, Value, Center) "
                            sql = sql & " @SELECT# MDOMonthlyValues.IDCenter, MDOMonthlyValues.Date, SUM(CONVERT(NUMERIC(18,2),MDOMonthlyValues.Value)) AS Total, (@SELECT# Name FROM BusinessCenters WHERE ID = MDOMonthlyValues.IDCenter)  as Center FROM dbo.MDOMonthlyValues"
                            sql = sql & " WHERE "
                            sql = sql & " MDOMonthlyValues.Date = " & Any2Time(xDate).SQLSmallDateTime
                            sql = sql & " AND MDOMonthlyValues.Type = 1"
                            sql = sql & " GROUP BY MDOMonthlyValues.IDCenter, Date"
                            sql = sql & " ORDER BY MDOMonthlyValues.IDCenter, Date"
                            ExecuteSqlWithoutTimeOut(sql)

                        End If

                    Else
                        '' DIARIO , COGEREMOS EL VALOR ACTUAL DEL PERIODO TENIENDO EN CUENTA EL SALDO 15M      **************

                        ' INSERTAMOS LOS VALORES EN LA TABLA TEMPORAL

                        sql = "@INSERT# INTO TMPMDO (IDCenter, Date, Value, Center) "
                        sql = sql & "@SELECT# DailyCauses.IDCenter, DailyCauses.Date, SUM(CONVERT(NUMERIC(18,2),DailyCauses.Value)) AS Total, (@SELECT# Name FROM BusinessCenters WHERE ID = DailyCauses.IDCenter)  as Center FROM dbo.DailyCauses"
                        sql = sql & " INNER JOIN Employees ON Employees.ID = DailyCauses.IDEmployee"
                        sql = sql & " WHERE Employees.ID IN (" & employees.ToString & ")"
                        sql = sql & " AND  DailyCauses.Date >= " & Any2Time(beginDate).SQLSmallDateTime
                        sql = sql & " AND  DailyCauses.Date <= " & Any2Time(endDate).SQLSmallDateTime
                        sql = sql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                        sql = sql & " AND DailyCauses.IDCenter > 0"
                        sql = sql & " AND DailyCauses.AccrualsRules = 0"
                        sql = sql & " GROUP BY DailyCauses.IDCenter, Date"
                        sql = sql & " ORDER BY DailyCauses.IDCenter, Date"
                        If isMirror Then
                            sql = sql.Replace("DailyCauses", "DailyCauses_MIRROR")
                        End If

                        ExecuteSqlWithoutTimeOut(sql)

                    End If


                    If isApplyConguallo Then
                        ' OBTENEMOS LOS VALORES DEL CONGUALLO DEL MES ANTERIOR
                        Dim xDate As Date = beginDate
                        xDate = DateSerial(xDate.Year, xDate.Month, 1)
                        xDate = xDate.AddDays(-1)

                        sql = "@SELECT# MDOMonthlyValues.IDCenter,  SUM(CONVERT(NUMERIC(18,2),MDOMonthlyValues.Value)) AS Total, (@SELECT# Name FROM BusinessCenters WHERE ID = MDOMonthlyValues.IDCenter)  as Center FROM dbo.MDOMonthlyValues "
                        sql = sql & " WHERE "
                        sql = sql & " MDOMonthlyValues.Date = " & Any2Time(xDate).SQLSmallDateTime
                        sql = sql & " AND MDOMonthlyValues.Type = 2 "
                        sql = sql & " GROUP BY MDOMonthlyValues.IDCenter"
                        sql = sql & " ORDER BY MDOMonthlyValues.IDCenter"

                        Dim dt As DataTable = CreateDataTableWithoutTimeouts(sql, )

                        For Each row As System.Data.DataRow In dt.Rows
                            ' Añadimos/creamos para cada centro en el día 15 el valor de la diferencia del mes anterior
                            sql = "@SELECT# isnull(ID, 0) FROM TMPMDO WHERE IDCenter=" & row("IDCenter").ToString & " AND Date=" & Any2Time(DateSerial(endDate.Year, endDate.Month, 15)).SQLSmallDateTime
                            Dim dblID As Double = Any2Double(ExecuteScalar(sql))
                            If dblID > 0 Then
                                sql = "@UPDATE# TMPMDO SET VALUE=VALUE + " & Any2String(Any2Double(row("Total"))).Replace(",", ".") & "  WHERE ID=" & dblID.ToString
                                ExecuteSqlWithoutTimeOut(sql)
                            Else
                                sql = "@INSERT# INTO TMPMDO (IDCenter, Date, Value, Center) VALUES (" & row("IDCenter") & "," & Any2Time(DateSerial(endDate.Year, endDate.Month, 15)).SQLSmallDateTime & "," & Any2String(Any2Double(row("Total"))).Replace(",", ".") & ",'" & row("Center") & "')"
                                ExecuteSqlWithoutTimeOut(sql)
                            End If
                        Next
                        ' Libera memoria
                        dt.Dispose()
                    End If

                    ' GENERAMOS EL FICHERO A PARTIR DE LOS DATOS DE LA TABLA TEMPORAL
                    ' ORDENAMOS POR SECCION ASC CUENTA ASC FECHA ASC
                    sql = "@SELECT# * FROM dbo.TMPMDO WHERE charindex('/', center) > 0 "
                    sql += " ORDER BY left(center,CHARINDEX('/',center) -1), right(center,len(center) - CHARINDEX('/',center))  , Date "

                    Dim dtx As DataTable = CreateDataTableWithoutTimeouts(sql, )

                    For Each row As System.Data.DataRow In dtx.Rows
                        ' Para cada fecha del periodo
                        strLine = "51000"

                        ' SECCION
                        strLine += Right("00000" & Any2String(row("Center")).Split("/")(0), 5)

                        ' CUENTA 
                        strLine += Right("00000" & Any2String(row("Center")).Split("/")(1), 5)

                        ' CUENTA 
                        strLine += Right("00000" & Any2String(row("Center")).Split("/")(1), 5)

                        strLine += "000000000"

                        ' FECHA
                        strLine += Format(row("Date"), "yyyyMMdd")

                        Dim dblTotal As Double = Any2Double(row("Value"))

                        ' SIGNO
                        strLine += IIf(dblTotal < 0, "-", "+")

                        ' VALOR
                        Dim strHTR As String = Format(Math.Abs(dblTotal), "00000.00").Replace(roConversions.GetDecimalDigitFormat(), "")
                        strLine += strHTR

                        strLine += Space(15)

                        If strHTR <> "0000000" Then
                            ' Graba la linea si el saldo tiene valor                            
                            strLines.Add(strLine)
                        End If

                    Next
                    exportStream = GenerateStreamFromString(strLines)
                    ' Libera memoria
                    If dtx IsNot Nothing Then
                        dtx.Dispose()
                    End If
                End If

                If ret Then
                    ' Devuelve array de bytes                    
                    arrFile = exportStream.ToArray()
                    Azure.RoAzureSupport.UploadStream2BlobInCompanyContainer(exportStream, fileCopyName, roLiveQueueTypes.datalink, RoAzureSupport.GetCompanyName())

                    exportStream.Dispose()
                    ' Hacemos una copia del fichero resultante para 
                    ' que posteriormente la exportación de Logistic lo utilice.                                        
                End If


            Catch ex As Exception
                state.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportFIATMDOMadridv2")

            Finally
                ' Cierra la base de datos
                If Not IsNothing(cn) Then
                    If cn.State = ConnectionState.Open Then cn.Close()
                    cn.Dispose()
                End If
            End Try

            Return arrFile

        End Function

#End Region

#Region "10016 -Exportación FIAT - Plus Presencia"
        Public Shared Function EXCELExportFIATPlusPresencia(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByVal DelimiterChar As String, ByVal excelProfileBytes As Byte(), ByRef oState As roDataLinkState) As Byte()

            Dim arrFile As Byte() = Nothing

            Try

                ' Determina el nombre del fichero excel
                Dim NameFile As String = "IvecoPlusPresencia#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim ExcelProfile As New ExcelExport(excelProfileBytes)

                ' Crea la cabecera del fichero excel
                ExcFile.SetCellValue(1, 1, "ZHRKEY-CLIID", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                ExcFile.SetCellValue(1, 2, "ZHRKEY-BEGDA", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                ExcFile.SetCellValue(1, 3, "P0015-LGART", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                ExcFile.SetCellValue(1, 4, "P0015-BETRG", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                ExcFile.SetCellValue(1, 5, "P0015-WAERS", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                ' Obtenemos los empleados seleccionados
                Dim Ssql As String = ""
                Ssql = "@SELECT# * FROM Employees "
                Ssql = Ssql & " WHERE Employees.ID IN (" & mEmployees.ToString & ")"
                Ssql = Ssql & " ORDER BY ID"

                Dim wdt As DataTable = CreateDataTableWithoutTimeouts(Ssql)
                Dim xRow As Integer = 2

                Dim oEmployeeState As New UserFields.roUserFieldState(oState.IDPassport)

                For Each row As DataRow In wdt.Rows
                    ' Para cada empleado obtenemos los datos concretos
                    Dim IDEmployeeField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(row("ID"), "112N.SAP NOMINA", dtEndDate, oEmployeeState)

                    ' ID Empleado
                    Dim strID As String = ""
                    If Not IDEmployeeField Is Nothing Then
                        strID = IDEmployeeField.FieldValue
                    End If
                    ExcFile.SetCellValue(xRow, 1, strID, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Fecha fin Periodo Extraido
                    ExcFile.SetCellValue(xRow, 2, dtEndDate, "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Código de Concepto
                    ExcFile.SetCellValue(xRow, 3, "3400", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Lee el pje de dedicación al que pertenece el empleado  
                    Dim Pje As Double = 0
                    Dim oInfo As System.Globalization.NumberFormatInfo = System.Globalization.CultureInfo.CurrentCulture.NumberFormat
                    Dim oUserField As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(row("ID"), "117P DEDICACIÓN", dtEndDate, oEmployeeState)
                    If Not IsNothing(oUserField.FieldValue) Then Pje = roTypes.Any2Double(oUserField.FieldValue.ToString.Replace(".", oInfo.CurrencyDecimalSeparator)) / 100

                    ' Obtenemos Grupo profesional
                    Dim strImport As String = "0"
                    Dim intTypeEmployee As Integer = 300
                    Dim Field125GrupoProfesional As UserFields.roEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(row("ID"), "125GRUPO PROFESIONAL", dtEndDate, oEmployeeState)
                    If Not Field125GrupoProfesional Is Nothing Then
                        'Tipo de empleado
                        Select Case Trim(Any2String(Field125GrupoProfesional.FieldValue))
                            Case "OPD", "OPI" ' Operario
                                intTypeEmployee = 2
                            Case "ECO" ' Empleado
                                intTypeEmployee = 3
                            Case "MIL", "MIC"  ' Mando Intermedio
                                intTypeEmployee = 4
                            Case "MAS", "DIR" ' Mando Superior
                                intTypeEmployee = 5
                            Case Else
                        End Select

                        ' En el caso que el empleado sea de un tipo definido buscamos el tramo a tener en cuenta
                        If intTypeEmployee < 300 Then

                            ' Calculamos el porcentaje de presencia
                            Dim dblPorPresencia As Double = 0.0

                            Dim dblHorasTeo As Double = 0.0
                            Dim valueTeo As Double = 0.0

                            ' Para obtener las horas teoricas, miramos si existen en la plantilla,
                            ' en caso contrario las obtenemos del saldo de teoricas del empleado
                            If ExcelProfile.FileIsOK Then
                                valueTeo = Any2Double(Any2String(ExcelProfile.GetCellValue(2, 9, 1)).Replace(".", roConversions.GetDecimalDigitFormat()))
                            End If
                            If valueTeo > 0 Then
                                dblHorasTeo = valueTeo
                            Else
                                dblHorasTeo = Any2Double(DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(isnull(VALUE, 0)) FROM DAILYACCRUALS WHERE IDEMPLOYEE=" & row("id").ToString & " AND Date >=" & Any2Time(dtBeginDate).SQLSmallDateTime & " AND Date <=" & Any2Time(dtEndDate).SQLSmallDateTime & " AND IDConcept IN(@SELECT# id from concepts where ShortName like 'Teo')"))
                            End If

                            Dim dblHorasAbs As Double = 0.0
                            dblHorasAbs = Any2Double(DataLayer.AccessHelper.ExecuteScalar("@SELECT# SUM(isnull(VALUE, 0)) FROM DAILYACCRUALS WHERE IDEMPLOYEE=" & row("id").ToString & " AND Date >=" & Any2Time(dtBeginDate).SQLSmallDateTime & " AND Date <=" & Any2Time(dtEndDate).SQLSmallDateTime & " AND IDConcept IN(@SELECT# id from concepts where ShortName like 'APP')"))

                            If dblHorasTeo > 0 Then
                                Try
                                    dblPorPresencia = Any2Double(Format(((dblHorasTeo - dblHorasAbs) / dblHorasTeo) * 100, "00.00"))
                                Catch ex As Exception
                                    dblPorPresencia = 0.0
                                End Try
                            End If

                            ' Obtenemos documento plantilla
                            If ExcelProfile.FileIsOK Then
                                ' Obtenemos las condiciones de los tramos
                                Dim intTramo As Integer = -1
                                For i As Integer = 2 To 5
                                    Dim strTramo As String = Any2String(ExcelProfile.GetCellValue(i, intTypeEmployee, 1))
                                    Dim BeginP As Double = 0.0
                                    Dim EndP As Double = 0.0
                                    Try

                                        BeginP = Any2Double(strTramo.Split("#")(0).Replace(",", roConversions.GetDecimalDigitFormat()))
                                        EndP = Any2Double(strTramo.Split("#")(1).Replace(",", roConversions.GetDecimalDigitFormat()))
                                        If dblPorPresencia >= BeginP And dblPorPresencia <= EndP Then
                                            intTramo = i
                                            Exit For
                                        End If

                                    Catch ex As Exception
                                    End Try
                                Next

                                If intTramo > 0 Then
                                    Dim value As Double = Any2Double(Any2String(ExcelProfile.GetCellValue(intTramo, 8, 1)).Replace(".", roConversions.GetDecimalDigitFormat())) * Pje
                                    strImport = Any2String(value)

                                    ' Los directivos no tienen plus presencia
                                    If Trim(Any2String(Field125GrupoProfesional.FieldValue)) = "DIR" Then strImport = "0"
                                End If
                            End If
                        End If
                    End If

                    ExcFile.SetCellValue(xRow, 4, strImport, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Literal
                    ExcFile.SetCellValue(xRow, 5, "EUR", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    xRow += 1
                Next

                ' Graba el archivo
                ExcFile.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportFIATPlusPresencia")
            End Try

            Return arrFile

        End Function
#End Region

#Region "10004/10006/10023 -Exportación FIAT - MDO Valladolid"
        Public Shared Function ASCIIExportFIATMDOValladolid(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal bolDaily As Boolean) As Byte()
            Dim arrFile As Byte() = Nothing
            Dim objStream As MemoryStream = New MemoryStream()

            Try
                ' Crea el fichero
                Dim NameFile As String = ""
                If bolDaily Then
                    NameFile = "DataLinkExportFIATMDOValladolidDiaria.txt"
                Else
                    NameFile = "DataLinkExportFIATMDOValladolidMensual.txt"
                End If

                Dim strLine As String = ""
                Dim strLines As List(Of String) = New List(Of String)

                ' Cabecera
                If bolDaily Then
                    strLine = "***PRESENGG"
                Else
                    strLine = "***PRESENMM"
                End If


                strLine += Format(Now.Date, "yyyyMMdd")
                strLine += Format(dtBeginDate, "yyyyMMdd")
                strLine += Format(dtEndDate, "yyyyMMdd")
                strLine += "00000C"
                strLine += Space(31)
                strLines.Add(strLine)

                ' Eliminamos los registros de la tabla temporal
                ExecuteSql("@DELETE# FROM TMPMDOValladolid")

                ' Seleccionar los empleados
                Dim queryString As String = "" &
                    "@SELECT# dbo.Employees.id  " &
                    "FROM dbo.Employees " &
                    " WHERE " &
                    " dbo.Employees.ID in (" & mEmployees.ToString & ")" &
                    " ORDER BY Employees.id"
                Dim dt As DataTable = CreateDataTable(queryString)

                For Each row As System.Data.DataRow In dt.Rows
                    ' Para cada empleado
                    Dim strSQL As String = "@SELECT# top(1) value from EmployeeUserFieldValues where idEmployee=" & row("ID") & " AND Date<= " & Any2Time(Now.Date).SQLSmallDateTime & " AND FieldName= '111MATRÍCULA' order by Date Desc"
                    Dim strMatricula As String = Any2String(ExecuteScalar(strSQL))

                    Dim oActualDate As Date
                    oActualDate = dtBeginDate
                    Dim oEndDate As Date
                    oEndDate = dtEndDate

                    While oActualDate <= oEndDate
                        strSQL = "@SELECT# Value from DailyAccruals where idEmployee=" & row("ID") & " AND Date= " & Any2Time(oActualDate).SQLSmallDateTime & " AND IDConcept IN(@SELECT# id from concepts where shortname like '15V') AND CarryOver = 0 and StartupValue = 0 "
                        Dim strHTR As String = Format(Math.Abs(Any2Double(ExecuteScalar(strSQL))), "00.00").Replace(roConversions.GetDecimalDigitFormat(), "")

                        strSQL = "@SELECT# Value from DailyAccruals where idEmployee=" & row("ID") & " AND Date= " & Any2Time(oActualDate).SQLSmallDateTime & " AND IDConcept IN(@SELECT# id from concepts where shortname like '15I') AND CarryOver = 0 and StartupValue = 0 "
                        Dim strHINC As String = Format(Math.Abs(Any2Double(ExecuteScalar(strSQL))), "00.00").Replace(roConversions.GetDecimalDigitFormat(), "")

                        If strHINC <> "0000" Or strHTR <> "0000" Then
                            ' Graba la linea en la tabla temporal
                            ExecuteSql("@INSERT# INTO TMPMDOValladolid (Matricula, Fecha, Valor1, Valor2) VALUES ('" & strMatricula.Replace("'", "''") & "'," & Any2Time(oActualDate).SQLSmallDateTime & ",'" & strHTR & "','" & strHINC & "')")
                        End If

                        oActualDate = oActualDate.AddDays(1)
                    End While
                Next


                ' *****************************************
                ' Obtenemos todos los registros de la tabla temporal y los guardamos en el fichero
                queryString = "" &
                    "@SELECT# *    " &
                    "FROM dbo.TMPMDOValladolid " &
                    "ORDER BY Fecha desc, Matricula asc"
                dt = CreateDataTable(queryString)

                For Each row As System.Data.DataRow In dt.Rows
                    ' Para cada registro empleado
                    Dim strMatricula As String = Right("00000000" & Any2String(row("Matricula")), 8)

                    strLine = ""
                    strLine += strMatricula
                    strLine += "07"
                    strLine += "54"
                    strLine += Format(row("Fecha"), "yyyyMMdd")
                    strLine += "1"

                    Dim strHTR As String = Any2String(row("Valor1"))
                    strLine += strHTR
                    strLine += "0000"
                    Dim strHINC As String = Any2String(row("Valor2"))
                    strLine += strHINC
                    strLine += Space(39)

                    ' Graba la linea si uno de los dos saldos tiene valor
                    strLines.Add(strLine)

                Next

                objStream = GenerateStreamFromString(strLines)

                ' Libera memoria
                dt.Dispose()

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ASCIIExportFIATMDOValladolid")
            Finally
                arrFile = objStream.ToArray()
                objStream.Dispose()
            End Try

            Return arrFile

        End Function
#End Region

#Region "10024 -Exportación FIAT - Conguallo"

        Public Shared Function EXCELExportFIATConguallo(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal DelimiterChar As String, ByVal oExcelFileName As String) As Byte()

            Dim arrFile As Byte() = Nothing

            Try
                ' Determina el nombre del fichero excel
                Dim NameFile As String = "EXCELExportFIATEXCELExportFIATConguallo#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & oExcelFileName
                Dim ExcelProfile As New ExcelExport(strExcelFileName)


                ' Crea la cabecera del fichero excel 
                ExcFile.SetCellValue(1, 1, "SAPHR     ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 2, "Sección       ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 3, "Cuenta        ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 4, "Fecha         ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 5, "Valor         ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                ' Obtenemos los empleados seleccionados
                Dim Ssql As String = ""
                Ssql = "@SELECT# MDOMonthlyValues.*, BusinessCenters.Name as BC, "
                Ssql = Ssql & " (@SELECT# top(1) convert(nvarchar(100), isnull(Value, '')) from EmployeeUserFieldValues where FieldName='113N.SAP HR LIGHT' and EmployeeUserFieldValues.idEmployee=MDOMonthlyValues.IdEmployee and Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") as SAPHR  "
                Ssql = Ssql & " FROM MDOMonthlyValues, BusinessCenters "
                Ssql = Ssql & " WHERE BusinessCenters.ID = MDOMonthlyValues.IDCenter AND [Type] = 2 "
                Ssql = Ssql & " AND Date = " & Any2Time(dtEndDate).SQLSmallDateTime
                Ssql = Ssql & " ORDER BY SAPHR, BusinessCenters.Name, Date"

                Dim wdt As DataTable = CreateDataTableWithoutTimeouts(Ssql, )
                Dim xRow As Integer = 2

                Dim oEmployeeState As New Employee.roEmployeeState

                For Each row As DataRow In wdt.Rows
                    ' Para cada empleado, centro y fecha obtenemos los valores

                    ' Matricula
                    ExcFile.SetCellValue(xRow, 1, Any2String(row("SAPHR")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                    ' Seccion 
                    Dim strSeccion As String = ""
                    Try
                        strSeccion = row("BC")
                        strSeccion = Right("00000" & strSeccion.Split("/")(0), 5)
                    Catch ex As Exception
                    End Try
                    ExcFile.SetCellValue(xRow, 2, strSeccion, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                    ' Cuenta
                    Dim strCuenta As String = ""
                    Try
                        strCuenta = row("BC")
                        strCuenta = Right("00000" & strCuenta.Split("/")(1), 5)
                    Catch ex As Exception
                    End Try
                    ExcFile.SetCellValue(xRow, 3, strCuenta, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                    ' Fecha
                    ExcFile.SetCellValue(xRow, 4, row("Date"), "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                    ' Valor
                    ExcFile.SetCellValue(xRow, 5, Any2Double(row("value")), "#0.00", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    xRow += 1
                Next

                ' Graba el fichero 
                ExcFile.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportFIATConguallo")
            End Try

            Return arrFile

        End Function

#End Region

#Region "10020/10021/10022 -Exportación FIAT - WorkAnalysis"
        Public Shared Function EXCELExportFIATWAHT(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal DelimiterChar As String, ByVal oExcelFileName As String) As Byte()
            ' Horas trabajadas por empleado
            Dim arrFile As Byte() = Nothing

            Try
                ' Determina el nombre del fichero excel
                Dim NameFile As String = "IvecoWAnalysisHTD#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & oExcelFileName
                Dim ExcelProfile As New ExcelExport(strExcelFileName)

                ' Crea la cabecera del fichero excel
                ExcFile.SetCellValue(1, 1, "Empresa                    ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 2, "SAPHR        ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 3, "Línea        ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 4, "Cuenta       ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)


                Dim ActualDate As Date
                ActualDate = dtBeginDate
                Dim i As Integer = 5
                While ActualDate <= dtEndDate
                    ExcFile.SetCellValue(1, i, ActualDate, "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                    ActualDate = ActualDate.AddDays(1)
                    i += 1
                End While
                ExcFile.AutoFitColumn(1, i)


                ' Obtenemos los empleados seleccionados
                Dim Ssql As String = ""
                Dim xRow As Integer = 2

                Dim oEmployeeState As New Employee.roEmployeeState

                Dim strSQL As String = ""

                ' Para cada empleado obtenemos las movilidades 
                strSQL = "@SELECT#  Employees.ID as IDE , IDGroup, GroupName, BeginDate, EndDate, sysroEmployeeGroups.FullGroupName, sysroEmployeeGroups.Path, Groups.DescriptionGroup,  " &
                  " (@SELECT# top(1) Value from EmployeeUserFieldValues where FieldName='113N.SAP HR LIGHT' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") as SAPHR  " &
                  "FROM Employees " &
                           "INNER JOIN sysroEmployeeGroups " &
                           "ON Employees.ID = sysroEmployeeGroups.IDEmployee " &
                           "INNER JOIN Groups " &
                           "ON sysroEmployeeGroups.IDGroup = Groups.ID " &
                           "WHERE Employees.ID IN (" & mEmployees.ToString & ")" &
                           " And ((" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate" &
                           " And " & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.EndDate)" &
                           " Or (" & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate" &
                           " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.EndDate)" &
                           " Or (" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.BeginDate" &
                           " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate))" &
                           " Order by Employees.ID "

                Dim wdz As DataTable = CreateDataTableWithoutTimeouts(strSQL, )

                For Each zrow As DataRow In wdz.Rows
                    ' para cada movilidad creamos un registro

                    ' Empresa
                    Dim strFullGroupName As String = Any2String(zrow("FullGroupName"))
                    Dim strEmpresa As String = ""
                    Try
                        strEmpresa = Trim(String2Item(strFullGroupName, 0, "\"))
                    Catch ex As Exception
                    End Try
                    ExcFile.SetCellValue(xRow, 1, strEmpresa, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Matrícula
                    Dim strMatricula As String = Any2String(zrow("SAPHR"))
                    ExcFile.SetCellValue(xRow, 2, strMatricula, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Linea
                    Dim strLinea As String = ""
                    Try
                        strLinea = Trim(String2Item(strFullGroupName, 6, "\"))
                    Catch ex As Exception
                    End Try
                    ExcFile.SetCellValue(xRow, 3, strLinea, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Cuenta
                    Dim strCuenta As String = Any2String(zrow("DescriptionGroup"))
                    Try
                        strCuenta = String2Item(Trim(String2Item(strCuenta, 0, ";")), 1, "#")
                    Catch ex As Exception
                    End Try

                    ExcFile.SetCellValue(xRow, 4, strCuenta, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ' Obtenemos todos los saldos del periodo seleccionado
                    strSQL = "@SELECT# isnull(VALUE,0) as Total, Date FROM DAILYACCRUALS WHERE IDEMPLOYEE=" & zrow("IDE") & " AND Date >=" & Any2Time(dtBeginDate).SQLSmallDateTime & " and date <= " & Any2Time(dtEndDate).SQLSmallDateTime & " AND IDConcept IN(@SELECT# id from concepts where shortname like '15M')"
                    Dim wdzaux As DataTable = CreateDataTableWithoutTimeouts(strSQL, )


                    ' Valor del saldo 15M para cada dia seleccionado
                    ActualDate = dtBeginDate
                    i = 5
                    Dim dblValue As Double = 0
                    While ActualDate <= dtEndDate
                        dblValue = 0

                        ' Si esta dentro de la movilidad
                        If CDate(zrow("BeginDate")) <= ActualDate And ActualDate <= CDate(zrow("EndDate")) Then
                            If wdzaux IsNot Nothing AndAlso wdzaux.Rows.Count > 0 Then
                                Dim dRowsP() As DataRow = wdzaux.Select("Date ='" & Format(ActualDate, "yyyy/MM/dd") & "'")
                                If dRowsP.Length > 0 Then
                                    dblValue = dRowsP(0)("Total")
                                End If
                            End If

                            'Dim dblValue As Double = Any2Double(ExecuteScalar("@SELECT# isnull(SUM(VALUE),0) FROM DAILYACCRUALS WHERE IDEMPLOYEE=" & zrow("IDE") & " AND Date=" & Any2Time(ActualDate).SQLSmallDateTime & " AND IDConcept IN(@SELECT# id from concepts where shortname like '15M')", cn))
                            ExcFile.SetCellValue(xRow, i, dblValue, "#0.00", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                        End If

                        ActualDate = ActualDate.AddDays(1)
                        i += 1
                    End While

                    xRow += 1
                Next

                ' Graba el fichero 
                ExcFile.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportFIATWAHT")
            End Try

            Return arrFile

        End Function

        Public Shared Function EXCELExportFIATWAHS(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal DelimiterChar As String, ByVal oExcelFileName As String) As Byte()
            ' Horas trabajadas por seccion
            Dim arrFile As Byte() = Nothing

            Try
                ' Determina el nombre del fichero excel
                Dim NameFile As String = "EXCELExportFIATWAnalysisHS#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & oExcelFileName
                Dim ExcelProfile As New ExcelExport(strExcelFileName)

                ' Crea la cabecera del fichero excel
                ExcFile.SetCellValue(1, 1, "Unidad Operativa               ", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                Dim ActualDate As Date
                ActualDate = dtBeginDate
                Dim i As Integer = 2
                While ActualDate <= dtEndDate
                    ExcFile.SetCellValue(1, i, ActualDate, "dd/mm/yyyy", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                    ActualDate = ActualDate.AddDays(1)
                    i += 1
                End While
                ExcFile.AutoFitColumn(1, i)


                ExecuteSql("@DELETE# FROM TMPWorkAnalysis")

                '' Obtenemos los empleados seleccionados
                Dim Ssql As String = ""
                Dim xRow As Integer = 2


                Dim strSQL As String = ""

                ' ***********************************************************
                strSQL = " @INSERT# INTO TMPWorkAnalysis (IDEmployee, Date, Field1, Value ) "
                strSQL = strSQL & " @SELECT# DailyAccruals.IDEmployee, date, dbo.GetUOMax(fullgroupname),  isnull(Value, 0) as Value FROM sysroEmployeeGroups"
                strSQL = strSQL & " INNER JOIN Employees ON Employees.ID = sysroEmployeeGroups.IDEmployee"
                strSQL = strSQL & " INNER JOIN DAILYACCRUALS ON DAILYACCRUALS.IDEmployee = sysroEmployeeGroups.IDEmployee"
                strSQL = strSQL & " WHERE DailyAccruals.IDEmployee IN  (" & mEmployees.ToString & ")"
                strSQL = strSQL & " And ((" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate"
                strSQL = strSQL & " And " & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.EndDate)"
                strSQL = strSQL & " Or (" & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate"
                strSQL = strSQL & " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.EndDate)"
                strSQL = strSQL & " Or (" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime & " <= sysroEmployeeGroups.BeginDate"
                strSQL = strSQL & " And " & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime & " >= sysroEmployeeGroups.BeginDate))"
                strSQL = strSQL & " And DailyAccruals.Date >=" & Any2Time(Any2Time(dtBeginDate).DateOnly).SQLSmallDateTime
                strSQL = strSQL & " And DailyAccruals.Date <=" & Any2Time(Any2Time(dtEndDate).DateOnly).SQLSmallDateTime
                strSQL = strSQL & " AND DailyAccruals.Date >= sysroEmployeeGroups.BeginDate "
                strSQL = strSQL & " AND DailyAccruals.Date <= sysroEmployeeGroups.EndDate"
                strSQL = strSQL & " AND IDConcept IN(@SELECT# id from concepts where shortname like '15M')"

                ExecuteSqlWithoutTimeOut(strSQL)

                ' Obtenemos las distintas UO
                Ssql = "@SELECT# DISTINCT Field1 FROM TMPWorkAnalysis ORDER BY Field1"
                Dim wdtaux As DataTable = CreateDataTableWithoutTimeouts(Ssql, )

                For Each row As DataRow In wdtaux.Rows

                    'U.O.
                    ExcFile.SetCellValue(xRow, 1, row("Field1"), "", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                    ActualDate = dtBeginDate
                    i = 2
                    While ActualDate <= dtEndDate

                        Dim dblValue As Double = Any2Double(ExecuteScalar("@SELECT# isnull(SUM(VALUE),0) FROM TMPWorkAnalysis WHERE Field1='" & row("Field1").ToString.Replace("'", "''") & "' AND Date=" & Any2Time(ActualDate).SQLSmallDateTime))
                        ExcFile.SetCellValue(xRow, i, dblValue, "#0.00", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                        ActualDate = ActualDate.AddDays(1)
                        i += 1
                    End While
                    xRow += 1
                Next

                ' Graba el fichero 
                ExcFile.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportFIATWAHS")
            End Try

            Return arrFile
        End Function

        Public Shared Function EXCELExportFIATWARM(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal DelimiterChar As String, ByVal oExcelFileName As String) As Byte()
            ' Report Mensual
            Dim arrFile As Byte() = Nothing

            Try
                ' Determina el nombre del fichero excel
                Dim NameFile As String = "EXCELExportFIATWAnalysisRM#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim oSettings As New roSettings()
                Dim strExcelFileName As String = oSettings.GetVTSetting(eKeys.DataLink) & "\" & oExcelFileName
                Dim ExcelProfile As New ExcelExport(strExcelFileName)

                ' Crea la cabecera del fichero excel
                ExcFile.SetCellValue(1, 1, "SAPHR", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 2, "Clave presencia", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 3, "Presencia", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 4, "Línea", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 5, "Código cuenta", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 6, "Cuenta", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                ExcFile.SetCellValue(1, 7, "Horas", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)
                ExcFile.SetCellValue(1, 8, "Categoría", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                ExcFile.AutoFitColumn(1, 8)

                ' Obtenemos los empleados seleccionados
                Dim Ssql As String = ""
                Ssql = "@SELECT# ID, (@SELECT# top(1) Value from EmployeeUserFieldValues where FieldName='113N.SAP HR LIGHT' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") as SAPHR, (@SELECT# top(1) Value from EmployeeUserFieldValues where FieldName='125GRUPO PROFESIONAL' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ") as GProfesional   "
                Ssql = Ssql & " , (@SELECT#  Top 1 isnull(DescriptionGroup, '') " &
                            "FROM groups where id in(@SELECT# idgroup from Employees x " &
                            "INNER JOIN sysroEmployeeGroups " &
                            "ON x.ID = sysroEmployeeGroups.IDEmployee " &
                            "WHERE x.ID = employees.id " &
                            " And sysroEmployeeGroups.BeginDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                            " And sysroEmployeeGroups.EndDate >= " & Any2Time(dtEndDate).SQLSmallDateTime & ")) as Cuenta"


                Ssql = Ssql & " , (@SELECT#  Top 1 FullGroupName " &
                    "FROM Employees z " &
                           "INNER JOIN sysroEmployeeGroups " &
                           "ON Employees.ID = sysroEmployeeGroups.IDEmployee " &
                           "WHERE z.ID = employees.id " &
                           " And sysroEmployeeGroups.BeginDate <= " & Any2Time(dtEndDate).SQLSmallDateTime &
                           " And sysroEmployeeGroups.EndDate >= " & Any2Time(dtEndDate).SQLSmallDateTime & ") as FullGroupName"

                Ssql = Ssql & " FROM Employees "
                Ssql = Ssql & " WHERE Employees.ID IN (" & mEmployees.ToString & ")"
                Ssql = Ssql & " ORDER BY ID"


                Dim wdt As DataTable = CreateDataTableWithoutTimeouts(Ssql, )
                Dim xRow As Integer = 2

                Dim oEmployeeState As New Employee.roEmployeeState

                Dim strSQL As String = ""


                For Each row As DataRow In wdt.Rows

                    ' SAPHR
                    Dim strMatricula As String = Any2String(row("SAPHR"))


                    'Código cuenta
                    Dim strCuenta As String = ""
                    Try
                        strCuenta = String2Item(Trim(String2Item(Any2String(row("Cuenta")), 0, ";")), 1, "#")
                    Catch ex As Exception
                    End Try

                    ' Codigo Linea y descripcion
                    Dim strFullGroupName As String = Any2String(row("FullGroupName"))
                    Dim strLinea As String = ""
                    Try
                        strLinea = Trim(String2Item(strFullGroupName, 6, "\"))
                    Catch ex As Exception
                    End Try

                    'Cuenta
                    strSQL = "@SELECT# Name  from TMPCuentas where ID =" & Any2Double(strCuenta).ToString
                    Dim strCuentaDsc As String = Any2String(ExecuteScalar(strSQL))


                    Ssql = "@SELECT# SUM(Value) as Total , IDCause, Causes.Name as CauseName, Causes.ShortName FROM DailYCauses, Causes "
                    Ssql = Ssql & " WHERE DailYCauses.IDEmployee = " & row("ID")
                    Ssql = Ssql & "  AND Date >=" & Any2Time(dtBeginDate).SQLSmallDateTime & " AND Date <=" & Any2Time(dtEndDate).SQLSmallDateTime
                    Ssql = Ssql & "  AND DailyCauses.IDCause = Causes.ID "
                    Ssql = Ssql & " AND DailyCauses.IDCause IN(@SELECT# DISTINCT IDCause FROM ConceptCauses WHERE IDConcept IN(@SELECT# ID FROM Concepts WHERE ShortName like '15M'))"
                    Ssql = Ssql & "  GROUP BY IDCause, Causes.Name, Causes.ShortName "
                    Ssql = Ssql & "  ORDER BY IDCause "

                    Dim zdt As DataTable = CreateDataTableWithoutTimeouts(Ssql, )
                    ' Obtenemos las justificaciones diarias de cada empleado
                    For Each zrow As DataRow In zdt.Rows
                        'Matrícula
                        ExcFile.SetCellValue(xRow, 1, strMatricula, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        'Clave presencia
                        ExcFile.SetCellValue(xRow, 2, Any2String(zrow("ShortName")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        'Presencia
                        ExcFile.SetCellValue(xRow, 3, Any2String(zrow("CauseName")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        ExcFile.SetCellValue(xRow, 4, strLinea, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        ExcFile.SetCellValue(xRow, 5, strCuenta, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        ExcFile.SetCellValue(xRow, 6, strCuentaDsc, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        'Horas
                        ExcFile.SetCellValue(xRow, 7, Any2Double(zrow("Total")), "#0.00", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Right, False)

                        'Categoría
                        Dim strCategoria As String = Any2String(row("GProfesional"))
                        ExcFile.SetCellValue(xRow, 8, strCategoria, "", DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                        xRow += 1
                    Next
                Next


                ' Graba el fichero 
                ExcFile.SaveFile()

                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::EXCELExportFIATWARM")
            End Try

            Return arrFile

        End Function
#End Region

#Region "Exportación FIAT - PlacesHistory"
        Public Shared Function ExcelExportFIATPlacesHistory(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal DelimiterChar As String, ByVal oExcelFileName As String) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                Dim NameFile As String = "IvecotFIATPlacesHistory#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                Dim queryString As String = "" &
                    "@SELECT# dbo.sysroEmployeeGroups.GroupName, dbo.sysroEmployeeGroups.Path, dbo.Employees.ID as idEmployee, " &
                    "dbo.sysroEmployeeGroups.IDGroup, dbo.Employees.Name as EmployeeName, dbo.sysroEmployeeGroups.BeginDate, " &
                    "dbo.sysroEmployeeGroups.EndDate, dbo.Groups.DescriptionGroup, dbo.sysroEmployeeGroups.FullGroupName, " &
                    "dbo.EmployeeContracts.IDContract, dbo.EmployeeContracts.BeginDate AS ECBeginDate, dbo.EmployeeContracts.EndDate AS ECEndDate, " &
                    "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='111MATRÍCULA' and EmployeeUserFieldValues.idEmployee=sysroEmployeeGroups.IDEmployee and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as Matricula, " &
                    "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='113N.SAP HR LIGHT' and EmployeeUserFieldValues.idEmployee=sysroEmployeeGroups.IDEmployee and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as MatriculaSAP, " &
                    "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='113CAT. INF. 8' and EmployeeUserFieldValues.idEmployee=sysroEmployeeGroups.IDEmployee and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as Categoria " &
                    "FROM Employees inner JOin sysroEmployeeGroups  on idEmployee=id INNER JOIN dbo.Groups ON dbo.sysroEmployeeGroups.IDGroup = dbo.Groups.ID INNER JOIN " &
                    "dbo.EmployeeContracts ON dbo.sysroEmployeeGroups.IDEmployee = dbo.EmployeeContracts.IDEmployee " &
                    "WHERE " &
                    " dbo.sysroEmployeeGroups.IDEmployee in (" & mEmployees.ToString & ")" &
                    " AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & " between EmployeeContracts.BeginDate and EmployeeContracts.EndDate "

                Dim dtCab As DataTable = CreateDataTableWithoutTimeouts(queryString, )

                ' Procesa los puestos
                Dim Descrip As String = ""
                Dim Puesto As String = ""
                Dim Categoria As String = ""
                Dim i As Integer = 0

                dtCab.Columns.Add("Puesto", Type.GetType("System.String"))
                dtCab.Columns.Add("PuestoDescripcion", Type.GetType("System.String"))
                dtCab.Columns.Add("PuestoCategoria", Type.GetType("System.String"))

                For Each Row As DataRow In dtCab.Rows
                    Dim aux1() As String
                    Descrip = ""
                    Categoria = ""
                    aux1 = Any2String(Row("FullGroupName")).Split("\")

                    If aux1.Length >= 8 Then
                        Dim aux2() As String
                        aux2 = Trim(aux1(7)).Split(" ")
                        Puesto = aux2(0)

                        For i = 1 To aux2.Length - 1
                            If i > 1 Then Descrip += " "
                            Descrip += aux2(i)
                        Next
                    End If

                    Dim Aux As String = Any2String(Row("DescriptionGroup")).ToUpper
                    i = InStr(Aux, "GRUPO#")
                    If i > 0 Then Categoria = Aux.Substring(i + 5, Aux.Length - 5 - i)

                    Row("Puesto") = Puesto
                    Row("PuestoDescripcion") = Descrip
                    Row("PuestoCategoria") = Categoria
                Next


                Dim Rows() As DataRow = dtCab.Select("", "EmployeeName, EndDate Desc")

                ' Crea la cabecera del excel
                If dtCab.Rows.Count Then
                    ExcFile.SetCellValue(1, 1, "Matrícula", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 2, "Nombre", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                    ExcFile.SetCellValue(1, 3, "Cód. Puesto", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 4, "Denominación", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                    ExcFile.SetCellValue(1, 5, "Cat. Puesto", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 6, "Seccion Puesto", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 7, "F.E. Puesto", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 8, "F.S. Puesto", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 9, "Estado", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)

                    For j As Integer = 1 To 9
                        ExcFile.SetPattern(1, i, Color.Gray)
                    Next j
                End If

                ' Crea las líneas
                Dim xRow As Integer = 2
                Dim n As Integer = 0
                Dim xRowIniEmp As Integer = xRow
                Dim xRowFinEmp As Integer = xRow
                Dim Section As String = ""
                Dim Name As String = ""
                Dim EmpAnt As Integer = 0
                Dim Fecha As Date = Nothing

                For Each rowCab As System.Data.DataRow In Rows
                    Try
                        xRowFinEmp = xRow
                        If EmpAnt = 0 Then EmpAnt = rowCab("idEmployee")
                        If EmpAnt <> rowCab("idEmployee") Then
                            ' Si el empleado tiene mas de un registro combina registros del empleado
                            If xRowFinEmp - xRowIniEmp > 1 Then
                                ExcFile.MergeCells(xRowIniEmp, 1, xRowFinEmp - 1, 1)
                                ExcFile.MergeCells(xRowIniEmp, 2, xRowFinEmp - 1, 2)
                            End If

                            ' Crea bordes del puesto
                            ExcFile.CreateBox(xRowIniEmp, 1, xRowFinEmp - 1, 9, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin)

                            EmpAnt = rowCab("idEmployee")
                            xRowIniEmp = xRow
                            n = 0
                        End If

                        ' Crea los registros
                        If n = 0 Then
                            ExcFile.SetCellValue(xRow, 1, Any2String(rowCab("MatriculaSAP")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)

                            Name = rowCab("EmployeeName")
                            Dim iName As Integer = 0
                            If IsNumeric(Name.Substring(1, 1)) Then
                                iName = InStr(Name, " ")
                                If iName > 0 Then Name = Name.Substring(iName, Name.Length - iName)
                            End If
                            ExcFile.SetCellValue(xRow, 2, Name, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        End If

                        ExcFile.SetCellValue(xRow, 3, Any2String(rowCab("Puesto")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        ExcFile.SetCellValue(xRow, 4, Any2String(rowCab("PuestoDescripcion")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        ExcFile.SetCellValue(xRow, 5, Any2String(rowCab("PuestoCategoria")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        Section = ""
                        If Any2String(rowCab("FullGroupName")).Split("\").Length >= 7 Then
                            ' Split the 7th element by space and process it
                            Dim segment As String = Any2String(rowCab("FullGroupName")).Split("\")(6).Split(" ")(0)

                            ' If the first part is empty, use the second part if it exists
                            If segment <> "" Then
                                Section = Right(segment.PadLeft(5, "0"), 5)
                            ElseIf Any2String(rowCab("FullGroupName")).Split("\")(6).Split(" ").Length > 1 Then
                                Section = Right(Any2String(rowCab("FullGroupName")).Split("\")(6).Split(" ")(1).PadLeft(5, "0"), 5)
                            End If
                        End If
                        ExcFile.SetCellValue(xRow, 6, Section, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        ExcFile.SetCellValue(xRow, 7, Any2String(rowCab("BeginDate")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)

                        If rowCab("EndDate") = CDate("2079/01/01") Then
                            ExcFile.SetCellValue(xRow, 9, "ACTIVO", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        Else
                            ExcFile.SetCellValue(xRow, 8, Any2String(rowCab("EndDate")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                            ExcFile.SetCellValue(xRow, 9, "ANTIGUO", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                        End If

                        xRow += 1
                        n += 1
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roDataLinkExport::ExcelExportPlacesHistory " & ex.Message)
                    End Try
                Next

                ' Si el empleado tiene mas de un registro combina registros del empleado
                If xRowFinEmp - xRowIniEmp > 1 Then
                    ExcFile.MergeCells(xRowIniEmp, 1, xRowFinEmp, 1)
                    ExcFile.MergeCells(xRowIniEmp, 2, xRowFinEmp, 2)
                End If

                If n Then
                    ExcFile.CreateBox(xRowIniEmp, 1, xRowFinEmp, 9, DocumentFormat.OpenXml.Spreadsheet.BorderStyleValues.Thin)
                End If

                ' Autoajusta columnas              
                For iColumn As Integer = 1 To 9
                    ExcFile.AutoFitColumn(iColumn, 50)
                Next

                ExcFile.ColumnSize(2, 50, 0)

                ' Graba el fichero 
                ExcFile.SaveFile()

                ' Devuelve array de bytes
                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ExcelExportPlacesHistory")
            End Try

            Return arrFile
        End Function
#End Region


#Region "Exportación FIAT - HISTORIAL CAMBIO DE CATEGORIA (ID 8997)"

        Public Shared Function ExcelExportCategory(ByVal mEmployees As String, ByVal dtBeginDate As Date, ByVal dtEndDate As Date, ByRef oState As roDataLinkState, ByVal DelimiterChar As String, ByVal oExcelFileName As String) As Byte()
            Dim arrFile As Byte() = Nothing

            Try
                ' Se conecta a la base de datos

                ' Determina el nombre del fichero excel
                Dim NameFile As String = "EXCELExportFIATCategories#" & oState.IDPassport & "#" & Now.ToString("yyyyMMddHHss") & ".xlsx"
                Dim ExcFile As New ExcelExport(ExcelExport.ExcelVersion.exc_2007, NameFile)

                ' Crea el adaptador para seleccionar las categorias del empleado

                ' Selecciona los empleados
                Dim queryString As String = "" &
                    "@SELECT# dbo.Employees.Name,  dbo.Employees.id,  EmployeeContracts.BeginDate, " &
                        "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='111MATRÍCULA' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as Matricula, " &
                        "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='113N.SAP HR LIGHT' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as MatriculaSAP, " &
                        "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='111FCH.ALTA GRUPO' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as FechaAntig, " &
                        "(@SELECT# top(1) Value from EmployeeUserFieldValues " &
                        "where FieldName='113CAT. INF. 8' and EmployeeUserFieldValues.idEmployee=Employees.Id and Date<=" & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & ") as Categoria, " &
                    "FullGroupName, Path " &
                    "FROM dbo.Employees INNER JOIN " &
                    "dbo.EmployeeContracts ON dbo.Employees.ID = dbo.EmployeeContracts.IDEmployee  INNER JOIN " &
                    "sysroEmployeeGroups ON dbo.sysroEmployeeGroups.idEmployee=dbo.Employees.id " &
                    "WHERE " &
                    " dbo.Employees.ID in (" & mEmployees.ToString & ")" &
                    " AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & " between EmployeeContracts.BeginDate and EmployeeContracts.EndDate " &
                    " AND " & roTypes.Any2Time(dtEndDate).SQLSmallDateTime & " between sysroEmployeeGroups.BeginDate and sysroEmployeeGroups.EndDate " &
                    "ORDER BY Employees.Name "
                Dim dtCab As DataTable = CreateDataTable(queryString)

                ' Crea la cabecera del excel
                If dtCab.Rows.Count Then
                    ExcFile.SetCellValue(1, 1, "Matrícula", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 2, "Matrícula SAP", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 3, "Nombre", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                    ExcFile.SetCellValue(1, 4, "Categoría", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)
                    ExcFile.SetCellValue(1, 5, "Fecha Inicio", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 6, "Fecha Fin", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 7, "Sección Actual", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                    ExcFile.SetCellValue(1, 8, "Fecha de Antigüedad", , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)

                    For i As Integer = 1 To 8
                        ExcFile.SetPattern(1, i, Color.Gray)
                    Next i
                End If

                ' Crea las líneas
                Dim xRow As Integer = 2
                Dim n As Integer = 0
                Dim xRowIniEmp As Integer = 0
                Dim xRowFinEmp As Integer = 0

                Dim Seccion As String = ""
                Dim Name As String = ""
                Dim DateAnt As Date = Nothing
                Dim Fecha As Date = Nothing

                For Each rowCab As System.Data.DataRow In dtCab.Rows
                    If Any2String(rowCab("Categoria")) <> "" Then
                        ' Para cada empleado selecciona las categorias
                        Dim strSQL As String = "@SELECT# Date,Value from EmployeeUserFieldValues where idEmployee= " & rowCab("ID").ToString & " and Fieldname = '113CAT. INF. 8' order by Date Desc "
                        Dim dtLin As DataTable = CreateDataTable(strSQL)
                        n = 0

                        xRowIniEmp = xRow

                        ' Crea las líneas del excel
                        For Each rowLin As DataRow In dtLin.Rows
                            If n = 0 Then
                                ExcFile.SetCellValue(xRow, 1, Any2String(rowCab("Matricula")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)
                                ExcFile.SetCellValue(xRow, 2, Any2String(rowCab("MatriculaSAP")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)

                                Name = Any2String(rowCab("Name"))
                                Try
                                    Dim i As Integer = 0
                                    If IsNumeric(Name.Substring(1, 1)) Then
                                        i = InStr(Name, " ")
                                        If i > 0 Then
                                            Name = Name.Substring(i, Name.Length - i)
                                        End If
                                    End If

                                Catch ex As Exception
                                    oState.UpdateStateInfo(ex, "roDataLinkExport::sf_GetName")
                                End Try
                                ExcFile.SetCellValue(xRow, 3, Name, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False, , , , , , DocumentFormat.OpenXml.Spreadsheet.VerticalAlignmentValues.Center)

                                Try
                                    Dim fullPath As String = Any2String(rowCab("FullGroupName"))
                                    Dim aux1() As String = fullPath.Split("\"c)

                                    If aux1.Length >= 7 Then
                                        Dim aux2() As String = aux1(6).Split(" "c)
                                        If aux2(0) <> "" Then
                                            Seccion = Right(aux2(0).ToString.PadLeft(5, "0"c), 5)
                                        ElseIf aux2.Length > 1 Then
                                            Seccion = Right(aux2(1).ToString.PadLeft(5, "0"c), 5)
                                        End If
                                    End If
                                Catch ex As Exception
                                    oState.UpdateStateInfo(ex, "roDataLinkExport::Seccion_Assignment")
                                End Try

                                ExcFile.SetCellValue(xRow, 7, Seccion, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                                ExcFile.SetCellValue(xRow, 8, Any2String(rowCab("FechaAntig")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                            End If

                            ExcFile.SetCellValue(xRow, 4, Any2String(rowLin("Value")), , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Left, False)

                            Fecha = rowLin("Date")
                            If Fecha = CDate("1900/01/01") Then Fecha = rowCab("BeginDate")

                            ExcFile.SetCellValue(xRow, 5, Fecha.ToShortDateString, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                            If n > 0 Then ExcFile.SetCellValue(xRow, 6, DateAnt.AddDays(-1).ToShortDateString, , DocumentFormat.OpenXml.Spreadsheet.HorizontalAlignmentValues.Center, False)
                            DateAnt = rowLin("Date")
                            xRow += 1
                            n += 1
                        Next

                        xRowFinEmp = xRow - 1

                        ' Si el empleado tiene mas de un registro combina registros del empleado
                        If xRowFinEmp - xRowIniEmp > 0 Then
                            ExcFile.MergeCells(xRowIniEmp, 1, xRowFinEmp, 1)
                            ExcFile.MergeCells(xRowIniEmp, 2, xRowFinEmp, 2)
                            ExcFile.MergeCells(xRowIniEmp, 3, xRowFinEmp, 3)
                        End If
                    End If
                Next

                ' Autoajusta columnas              
                For i As Integer = 1 To 8
                    ExcFile.AutoFitColumn(i, 50)
                Next

                ' Graba el fichero 
                ExcFile.SaveFile()

                ' Devuelve array de bytes
                arrFile = Azure.RoAzureSupport.GetFileBytesFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)
                Azure.RoAzureSupport.DeleteFileFromAzure(NameFile, DTOs.roLiveQueueTypes.datalink)

            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ExcelExportCategory")
            End Try

            Return arrFile
        End Function
#End Region

    End Class

End Namespace