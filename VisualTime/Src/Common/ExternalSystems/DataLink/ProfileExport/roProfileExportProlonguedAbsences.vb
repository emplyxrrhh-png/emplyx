Imports System.Data.Common
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace DataLink

    Public Class ProfileExportProlonguedAbsences

#Region "Declarations - Constructor"

        Private mEmployeeFilterTable As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody
        Private mdaDinners As DbDataAdapter = Nothing
        Private mdaAbsences As DbDataAdapter = Nothing
        Private mdaHolidays As DbDataAdapter = Nothing

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState
        Private mExportOnlyChangedAbsences As Boolean = False
        Private mExportAlsoProgrammedCauses As Boolean = False

        Public Sub New(ByVal tmpEmployeeFilterTable As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal _State As roDataLinkState,
                       Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeFilterTable = tmpEmployeeFilterTable
            mBeginDay = BeginDay
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)

            Me.mExportOnlyChangedAbsences = False
            Me.mExportAlsoProgrammedCauses = False

        End Sub

        ' Lanzamiento manual
        Public Sub New(ByVal tmpEmployeeFilterTable As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal _State As roDataLinkState,
                       Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeFilterTable = tmpEmployeeFilterTable
            mBeginDate = BeginDate
            mEndDate = EndDate
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
        End Sub

#End Region

#Region "Properties"

        Public Property State() As roDataLinkState
            Get
                Return Me.oState
            End Get
            Set(ByVal NewValue As roDataLinkState)

            End Set
        End Property

        Public Property Profile() As ProfileExportBody
            Get
                Return Me.mBody
            End Get
            Set(ByVal value As ProfileExportBody)
                Me.mBody = value
            End Set
        End Property

        Public Property ExportOnlyChangedAbsences As Boolean
            Get
                Return mExportOnlyChangedAbsences
            End Get
            Set(value As Boolean)
                mExportOnlyChangedAbsences = value
            End Set
        End Property

        Public Property ExportAlsoProgrammedCauses As Boolean
            Get
                Return mExportAlsoProgrammedCauses
            End Get
            Set(value As Boolean)
                mExportAlsoProgrammedCauses = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function ExportProfile() As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try
                ' Determina la fecha inicial y final para importación automática
                If Me.mBeginDate = #12:00:00 AM# Then
                    ' Importación automática
                    Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)
                    If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                    Me.mEndDate = Me.mBeginDate.AddMonths(1)
                    Me.mEndDate = Me.mEndDate.AddDays(-1)
                End If

                ' Crea el fichero de salida
                If Not Me.Profile.FileOpen() Then Exit Try
                bolCloseFile = True

                ' Selecciona los empleados con contrato activo
                Dim sSQL As String =
                    "@SELECT# sysrovwCurrentEmployeeGroups.IDEmployee, EmployeeName, GroupName, FullGroupName, dbo.EmployeeContracts.IDContract, " &
                    "dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate " &
                    "FROM dbo.sysrovwCurrentEmployeeGroups " &
                    "INNER JOIN dbo.EmployeeContracts ON dbo.sysrovwCurrentEmployeeGroups.IDEmployee = dbo.EmployeeContracts.IDEmployee "
                If Me.mEmployeeFilterTable <> "" Then sSQL &= "INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = dbo.sysrovwCurrentEmployeeGroups.IDEmployee "

                If Not mExportOnlyChangedAbsences Then
                    sSQL += " WHERE NOT (dbo.EmployeeContracts.EndDate <" & roTypes.Any2Time(Me.mBeginDate).SQLDateTime &
                            " OR dbo.EmployeeContracts.BeginDate > " & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ") "
                Else
                    sSQL += " WHERE 1 = 1 "
                End If
                sSQL += " ORDER BY EmployeeName, EmployeeContracts.begindate"

                Dim dtEmployees As DataTable = CreateDataTable(sSQL, "Employees")
                Dim dtProlAbs As DataTable
                Dim dtHolidays As DataTable
                Dim EmpAnt As Integer = 0
                Dim dtEmpUsrFields As DataTable = Nothing

                ' Para cada empleado
                For Each Row As DataRow In dtEmployees.Rows
                    ' Campos de la ficha
                    If EmpAnt <> Row("idEmployee") Then
                        dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("idEmployee"), Me.mEndDate, Me.oState)
                        EmpAnt = Row("idEmployee")
                    End If

                    ' Carga datos del registro de empleado
                    If Not LoadInfo(dtEmployees, Row, dtEmpUsrFields) Then Exit For

                    ' Carga información de las ausencias prolongadas del empleado para el periodo
                    dtProlAbs = LoadRegisters(Row)

                    ' En caso necesario, cargamos datos de las vacaciones
                    Dim bolExportHolidays = Profile.Fields.Any(Function(f) f.Source.ToUpper().Equals("HOLIDAYS"))
                    dtHolidays = Nothing
                    If bolExportHolidays And Not mExportOnlyChangedAbsences Then dtHolidays = LoadHolidaysRegisters(Row)

                    ' Para cada registro de ausencia prolongada
                    If dtProlAbs IsNot Nothing AndAlso dtProlAbs.Rows.Count > 0 Then
                        For Each cRow As DataRow In dtProlAbs.Rows
                            CreateLine(cRow)
                            If mExportOnlyChangedAbsences Then
                                ' Marcamos la ausencia del empleado como exportada
                                ExecuteSqlWithoutTimeOut("@UPDATE# ProgrammedAbsences SET IsExported=1 WHERE IDEmployee=" & cRow("idEmployee") & " AND IDCause=" & cRow("IDCause") & " AND BeginDate=" & Any2Time(cRow("BeginDate")).SQLSmallDateTime)
                            End If
                        Next
                    End If

                    ' Para cada registro de vacaciones
                    If dtHolidays IsNot Nothing AndAlso dtHolidays.Rows.Count > 0 Then
                        For Each cRow As DataRow In dtHolidays.Rows
                            CreateLineHolidays(cRow)
                        Next
                    End If

                Next

                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()
            End Try

            Return bolRet

        End Function

        Private Function CreateLineHolidays(ByVal cRow As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHAINICIO", "FECHAFINAL" : Profile.Fields(i).Value = cRow("Date")
                        Case "NC" : Profile.Fields(i).Value = cRow("ShortName")
                        Case "EXPORTARCOMO" : Profile.Fields(i).Value = cRow("Export")
                        Case "MAXDIAS", "MAXDÍAS" : Profile.Fields(i).Value = 1
                        Case "DIAS", "DíAS" : Profile.Fields(i).Value = 1
                        Case "OBSERVACIONES" : Profile.Fields(i).Value = ""
                        Case "FECHARECAIDA", "FECHARECAÍDA" : Profile.Fields(i).Value = ""
                    End Select
                Next i

                ' Graba la línea
                bolRet = Me.Profile.CreateLine

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:CreateLineHolidays")
            End Try

            Return bolRet
        End Function

        Private Function CreateLine(ByVal cRow As DataRow) As Boolean
            Dim bolRet As Boolean = False

            Try
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHAINICIO" : Profile.Fields(i).Value = cRow("BeginDate")
                        Case "FECHAFINAL"
                            If Not IsDBNull(cRow("FinishDate")) Then
                                Profile.Fields(i).Value = cRow("FinishDate")
                            Else

                                Dim customization As String = VTBusiness.Common.roBusinessSupport.GetCustomizationCode().ToUpper()
                                If roTypes.Any2String(customization).ToUpper = "TAIF" AndAlso cRow("MaxLastingDays") IsNot DBNull.Value AndAlso roTypes.Any2Integer(cRow("MaxLastingDays")) = 750 Then
                                    Profile.Fields(i).Value = New Date(9999, 12, 31)
                                Else
                                    Profile.Fields(i).Value = Nothing
                                End If
                            End If

                        Case "NC" : Profile.Fields(i).Value = cRow("ShortName")
                        Case "EXPORTARCOMO" : Profile.Fields(i).Value = cRow("Export")
                        Case "MAXDIAS", "MAXDÍAS" : Profile.Fields(i).Value = cRow("MaxLastingDays")
                        Case "FECHARECAIDA", "FECHARECAÍDA"
                            Profile.Fields(i).Value = cRow("RelapsedDate")
                        Case "OBSERVACIONES"
                            Profile.Fields(i).Value = cRow("ProgAbsDescription")
                        Case "HORAINICIO"
                            Profile.Fields(i).Value = cRow("HourBegin")
                        Case "HORAFINAL"
                            Profile.Fields(i).Value = cRow("HourEnd")
                        Case "DURACIONMAX"
                            Profile.Fields(i).Value = cRow("MaxDuration")
                        Case "DURACIONMIN"
                            Profile.Fields(i).Value = cRow("MinDuration")
                        Case "HORASREALESAUSENCIA"
                            Profile.Fields(i).Value = cRow("AbsenceDuration")
                        Case "DIAS", "DíAS" : Profile.Fields(i).Value = cRow("MaxLastingDays")
                            Dim Dias As Integer = 0
                            If Not IsDBNull(cRow("FinishDate")) Then
                                Dias = DateDiff("d", cRow("BeginDate"), cRow("FinishDate")) + 1
                            Else
                                Dias = cRow("MaxLastingDays")
                            End If
                            Profile.Fields(i).Value = Dias
                    End Select
                Next i

                ' Graba la línea
                bolRet = Me.Profile.CreateLine

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim dt As New DataTable

                ' Determina el empleado
                Dim idEmpleado As Long = row("idEmployee")

                ' Para cada columna
                Dim i As Integer = 0
                For i = 0 To Me.Profile.Fields.Count - 1
                    Me.Profile.Fields(i).Value = ""

                    Select Case Me.Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Me.Profile.Fields(i).Value = mBeginDate

                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Me.Profile.Fields(i).Value = mEndDate

                        Case "GRUPO"
                            Me.Profile.Fields(i).Value = row("GroupName")

                        Case "GRUPO_COMPLETO"
                            Me.Profile.Fields(i).Value = row("FullGroupName")

                        Case "NOMBRE"
                            Me.Profile.Fields(i).Value = row("EmployeeName")

                        Case "CONTRATO"
                            Me.Profile.Fields(i).Value = row("idContract")

                        Case "CONTRATO_FECHA_INICIO"
                            Me.Profile.Fields(i).Value = row("BeginDate")

                        Case "CONTRATO_FECHA_FINAL"
                            Me.Profile.Fields(i).Value = row("EndDate")

                        Case Else
                            ' Determina el tipo de campo
                            If InStr(1, Me.Profile.Fields(i).Source.ToUpper, "USR_") Then
                                ' Lee el dato del campo de la ficha
                                If dtEmpUsrFields.Rows.Count > 0 Then
                                    Dim FieldName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Dim r() As DataRow = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                                    If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                End If
                            Else
                                ' Literal
                                If InStr(1, Me.Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                    Me.Profile.Fields(i).Value = Me.Profile.Fields(i).Source.Substring(8, Me.Profile.Fields(i).Source.Length - 8)
                                ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Profile.Fields(i).Value = "RBS not supported"
                                Else
                                End If
                            End If
                    End Select
                Next

                ' Si hay saldos de los dos tipos no es correcto porque la plantilla no es correcta
                bolRet = True

                dt.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:LoadInfo")
            End Try

            Return bolRet
        End Function

        Private Function LoadHolidaysRegisters(ByVal Row As DataRow) As DataTable
            'Dim bolRet As Boolean = False
            Dim bd As Date = IIf(Me.mBeginDate > Row("BeginDate"), Me.mBeginDate, Row("BeginDate"))
            Dim ed As Date = IIf(Me.mEndDate < Row("EndDate"), Me.mEndDate, Row("EndDate"))
            Dim dt As DataTable = Nothing

            Try
                ' Crea el adaptador si no está definido
                If IsNothing(mdaHolidays) Then
                    mdaHolidays = CreateDataAdapter_Holidays()
                End If

                ' Lee todos los saldos del empleado entre fechas
                dt = New DataTable
                With mdaHolidays.SelectCommand
                    .Parameters("@idEmployee").Value = Row("idEmployee")
                    If Not mExportOnlyChangedAbsences Then
                        .Parameters("@BeginDate").Value = bd
                        .Parameters("@EndDate").Value = ed
                    End If
                End With

                dt = New DataTable
                mdaHolidays.Fill(dt)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:LoadHolidaysRegisters")
            End Try

            Return dt
        End Function

        Private Function LoadRegisters(ByVal Row As DataRow) As DataTable
            'Dim bolRet As Boolean = False
            Dim bd As Date = IIf(Me.mBeginDate > Row("BeginDate"), Me.mBeginDate, Row("BeginDate"))
            Dim ed As Date = IIf(Me.mEndDate < Row("EndDate"), Me.mEndDate, Row("EndDate"))
            Dim dt As DataTable = Nothing

            Try
                ' Crea el adaptador si no está definido
                If IsNothing(mdaAbsences) Then
                    If Not mExportOnlyChangedAbsences Then
                        If mExportAlsoProgrammedCauses Then
                            mdaAbsences = CreateDataAdapter_ProlAbsAndCauses()
                        Else
                            mdaAbsences = CreateDataAdapter_ProlAbs()
                        End If
                    Else
                        mdaAbsences = CreateDataAdapter_ProlAbsChanged()
                    End If
                End If

                ' Lee todos los saldos del empleado entre fechas
                dt = New DataTable
                With mdaAbsences.SelectCommand
                    .Parameters("@idEmployee").Value = Row("idEmployee")
                    If Not mExportOnlyChangedAbsences Then
                        .Parameters("@BeginDate").Value = bd
                        .Parameters("@EndDate").Value = ed
                    End If
                End With

                dt = New DataTable
                mdaAbsences.Fill(dt)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:LoadRegisters")
            End Try

            Return dt
        End Function

        Private Function CreateDataAdapter_Holidays() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = ""

                strSQL = "@SELECT# ISNULL(DailySchedule.IDShiftUsed,DailySchedule.IDShift1) AS ShiftID, DailySchedule.IDEmployee, DailySchedule.Date, Shifts.Export, Shifts.Name as ShiftName, Shifts.ShortName   " &
                        "FROM dbo.DailySchedule INNER JOIN dbo.Shifts ON ISNULL(DailySchedule.IDShiftUsed,DailySchedule.IDShift1) = dbo.Shifts.ID " &
                        " WHERE dbo.DailySchedule.idEmployee=@idEmployee " &
                    " AND dbo.DailySchedule.Date >=@BeginDate " &
                    " AND dbo.DailySchedule.Date <=@EndDate " &
                    " AND dbo.DailySchedule.IsHolidays=1 and isnull(Shifts.Export,'0') <>'0' "

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:CreateDataAdapter_Holidays")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_ProlAbs() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = ""

                strSQL = "@SELECT# dbo.ProgrammedAbsences.IDEmployee, dbo.ProgrammedAbsences.BeginDate, dbo.ProgrammedAbsences.FinishDate, dbo.ProgrammedAbsences.MaxLastingDays, " &
                         "dbo.ProgrammedAbsences.Description as ProgAbsDescription, dbo.Causes.Name, dbo.Causes.ShortName, dbo.Causes.Description AS CauseDescription, dbo.Causes.Export, " &
                         "dbo.ProgrammedAbsences.MaxLastingDays, dbo.ProgrammedAbsences.RelapsedDate " &
                         "FROM dbo.ProgrammedAbsences INNER JOIN dbo.Causes ON dbo.ProgrammedAbsences.IDCause = dbo.Causes.ID " &
                         "WHERE  idEmployee=@idEmployee and BeginDate<=@EndDate and (ISNULL(FinishDate,DATEADD(day, MaxLastingDays-1, BeginDate))>=@BeginDate) and Export<>'0'"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:CreateDataAdapter_Dinners")
            End Try

            Return da
        End Function

        ''' <summary>
        ''' Ausencias por días y por horas
        ''' </summary>
        ''' <returns></returns>
        Private Function CreateDataAdapter_ProlAbsAndCauses() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@SELECT# dbo.ProgrammedAbsences.IDEmployee, " &
                      "dbo.ProgrammedAbsences.BeginDate, " &
                      "Isnull(dbo.ProgrammedAbsences.FinishDate, Dateadd(day, dbo.ProgrammedAbsences.MaxLastingDays - 1, programmedabsences.begindate)) as FinishDate, " &
                      "dbo.ProgrammedAbsences.MaxLastingDays, " &
                      "DATEDIFF(day, programmedabsences.begindate, Isnull(finishdate, Dateadd(day, maxlastingdays - 1, programmedabsences.begindate))) + 1 TotalDays, " &
                      "NULL HourBegin, " &
                      "NULL HourEnd, " &
                      "NULL MaxDuration, " &
                      "NULL MinDuration, " &
                      "NULL AbsenceDuration, " &
                      "CONVERT(VARCHAR, dbo.ProgrammedAbsences.Description) AS ProgAbsDescription, " &
                      "dbo.Causes.Name, " &
                      "dbo.Causes.ShortName, " &
                      "CONVERT(VARCHAR, dbo.Causes.Description) AS CauseDescription, " &
                      "dbo.Causes.Export, " &
                      "dbo.ProgrammedAbsences.RelapsedDate " &
                      "FROM dbo.ProgrammedAbsences " &
                      "INNER JOIN dbo.Causes ON dbo.ProgrammedAbsences.IDCause = dbo.Causes.ID " &
                      "WHERE idEmployee=@idEmployee and BeginDate<=@EndDate and (ISNULL(FinishDate, DATEADD(day, MaxLastingDays-1, BeginDate))>=@BeginDate) and Export<>'0' " &
                      "UNION " &
                      "@SELECT# dbo.ProgrammedCauses.IDEmployee, " &
                      "dbo.ProgrammedCauses.Date AS BeginDate, " &
                      "dbo.ProgrammedCauses.FinishDate, " &
                      "0 AS MaxLastingDays, " &
                      "NULL TotalDays, " &
                      "ProgrammedCauses.Begintime AS HourBegin, " &
                      "ProgrammedCauses.EndTime AS HourEnd, " &
                      "CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, ProgrammedCauses.Duration * 3600, 0),24) As TIME(0)))), 0),108) MaxDuration, " &
                      "CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, ProgrammedCauses.MinDuration * 3600, 0),24) As TIME(0)))), 0),108) MinDuration, " &
                      "CONVERT(VARCHAR,DATEADD(mi, DATEDIFF(mi, 0, DATEADD(s, 30, CAST(CONVERT(VARCHAR,DATEADD(SECOND, (@SELECT# sum(isnull(dbo.DailyCauses.Value,0)) FROM dbo.DailyCauses WHERE dbo.ProgrammedCauses.IDCause = dbo.DailyCauses.IDCause and dbo.ProgrammedCauses.IDEmployee = dbo.DailyCauses.IDEmployee AND dbo.ProgrammedCauses.Date = dbo.DailyCauses.Date and dbo.DailyCauses.IDEmployee = @idEmployee) * 3600, 0),24) As TIME(0)))), 0),108) AbsenceDuration, " &
                      "CONVERT(VARCHAR, dbo.ProgrammedCauses.Description) AS ProgAbsDescription, " &
                      "dbo.Causes.Name, " &
                      "dbo.Causes.ShortName, " &
                      "CONVERT(VARCHAR, dbo.Causes.Description) AS CauseDescription, " &
                      "dbo.Causes.Export, " &
                      "NULL AS RelapsedDate " &
                      "FROM dbo.ProgrammedCauses " &
                      "INNER JOIN dbo.Causes ON dbo.ProgrammedCauses.IDCause = dbo.Causes.ID " &
                      "WHERE dbo.ProgrammedCauses.idEmployee=@idEmployee and dbo.ProgrammedCauses.Date<=@EndDate AND dbo.ProgrammedCauses.FinishDate >=@BeginDate AND Export<>'0'"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False) 'S
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:CreateDataAdapter_Dinners")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_ProlAbsChanged() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = ""

                strSQL = "@SELECT# dbo.ProgrammedAbsences.IDEmployee, dbo.ProgrammedAbsences.BeginDate, dbo.ProgrammedAbsences.FinishDate, dbo.ProgrammedAbsences.MaxLastingDays, " &
                         "dbo.ProgrammedAbsences.Description as ProgAbsDescription, dbo.Causes.Name, dbo.Causes.ShortName, dbo.Causes.Description AS CauseDescription, dbo.Causes.Export, dbo.Causes.ID as IDCause, " &
                         "dbo.ProgrammedAbsences.MaxLastingDays, dbo.ProgrammedAbsences.RelapsedDate " &
                         "FROM dbo.ProgrammedAbsences INNER JOIN dbo.Causes ON dbo.ProgrammedAbsences.IDCause = dbo.Causes.ID " &
                         "WHERE  idEmployee=@idEmployee and isnull(IsExported,0) = 0  and Export<>'0'"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportProlonguedAbsences:CreateDataAdapter_ProlAbsChanged")
            End Try

            Return da
        End Function

#End Region

    End Class
End Namespace

