Imports System.Data.Common
Imports Robotics.Base
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace DataLink

    Public Class ProfileExportDinners

#Region "Declarations - Constructor"

        Enum RegisterType
            OneByEmployee
            OneByDay
            OneByRegister
        End Enum

        Private mEmployeeFilterTable As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#
        Private mBody As ProfileExportBody
        Private mIncludeSchedule As Boolean = False
        Private mIncludeAccrual As Boolean = False
        Private mRegisterType As RegisterType = RegisterType.OneByEmployee
        Private mdaDinners As DbDataAdapter = Nothing
        Private mIncludeOfflines As Boolean = False
        Private mdaDailyAccruals As DbDataAdapter = Nothing

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private oState As roDataLinkState

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

        Public Property IncludeSchedule() As Boolean
            Get
                Return Me.mIncludeSchedule
            End Get

            Set(ByVal value As Boolean)
                Me.mIncludeSchedule = value
            End Set
        End Property

        Public Property IncludeAccrual() As Boolean
            Get
                Return Me.mIncludeAccrual
            End Get

            Set(ByVal value As Boolean)
                Me.mIncludeAccrual = value
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

        Public Property IncludeOfflines As Boolean
            Get
                Return mIncludeOfflines
            End Get
            Set(value As Boolean)
                mIncludeOfflines = value
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

                ''If customization
                'Dim sSQL As String =
                '    "@SELECT# sysrovwCurrentEmployeeGroups.IDEmployee, EmployeeName, GroupName, FullGroupName, dbo.EmployeeContracts.IDContract, " &
                '    "dbo.EmployeeContracts.BeginDate, dbo.EmployeeContracts.EndDate " &
                '    "FROM dbo.sysrovwCurrentEmployeeGroups INNER JOIN " &
                '    "dbo.EmployeeContracts ON dbo.sysrovwCurrentEmployeeGroups.IDEmployee = dbo.EmployeeContracts.IDEmployee " &
                '    "WHERE NOT (dbo.EmployeeContracts.EndDate <" & roTypes.Any2Time(Me.mBeginDate).SQLDateTime &
                '            " OR dbo.EmployeeContracts.BeginDate > " & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ") "
                'If Me.mEmployeesFilter <> "" Then sSQL += " and sysrovwCurrentEmployeeGroups.IDEmployee in (" & Me.mEmployeesFilter & ")"
                'sSQL += " ORDER BY EmployeeName, EmployeeContracts.begindate"

                ''else customization (para msd)

                Dim sSQL As String =
                    "@SELECT# sysroEmployeeGroups.IDEmployee, employees.Name as EmployeeName,  GroupName, FullGroupName, EmployeeContracts.IDContract, " &
                    " dbo.sysroEmployeeGroups.BeginDate, dbo.sysroEmployeeGroups.EndDate " &
                    "FROM dbo.sysroEmployeeGroups " &
                    " inner join EmployeeContracts on employeecontracts.IDEmployee = dbo.sysroEmployeeGroups.IDEmployee " &
                    " inner join employees on sysroEmployeeGroups.IDEmployee = employees.id "
                If Me.mEmployeeFilterTable <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeFilterTable & " ON " & Me.mEmployeeFilterTable & ".Id = sysroEmployeeGroups.IDEmployee "
                sSQL &= "WHERE (dbo.EmployeeContracts.EndDate >=" & roTypes.Any2Time(Me.mBeginDate).SQLDateTime &
                            " AND dbo.EmployeeContracts.BeginDate <= " & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ") "
                sSQL += " ORDER BY EmployeeName, EmployeeContracts.begindate"

                Dim dtEmployees As DataTable = CreateDataTable(sSQL, "Employees")
                Dim dtDinners As DataTable
                Dim dtSchedule As DataTable = Nothing
                Dim dtDailyAccruals As DataTable = Nothing
                Dim oEmployeesState As Employee.roEmployeeState
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

                    oEmployeesState = New Employee.roEmployeeState

                    ' Carga Horarios, Justificaciones y Saldos de cada empleado en caso de ser necesarios

                    If Me.IncludeSchedule Then dtSchedule = VTBusiness.Scheduler.roScheduler.GetPlan(Row("idEmployee"), mBeginDate, mEndDate, oEmployeesState,, True)
                    If Me.IncludeAccrual Then
                        dtDailyAccruals = LoadAccrualsForEmployee(Row("idEmployee"), mBeginDate, mEndDate)
                    End If

                    ' Carga información de registros de comedor del empleado para el periodo
                    dtDinners = LoadRegisters(Row)
                    If IsNothing(dtDinners) Then Exit For

                    ' Para cada registro de comedor
                    For Each cRow As DataRow In dtDinners.Rows
                        If cRow("Veces") > 0 Then
                            CreateLine(cRow, dtDailyAccruals, dtSchedule)
                        End If
                    Next
                Next

                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()
                bolRet = True

                'Se deja un mensaje informativo dentro del archivo ascii
                If Profile.MemoryStreamWriter IsNot Nothing AndAlso Profile.MemoryStreamWriter.Length = 0 Then
                    Dim textErrror As Byte() = System.Text.Encoding.Unicode.GetBytes(Me.oState.Language.Translate("roDataLinkExport.ExportProfile.NoInfo", ""))
                    Profile.MemoryStreamWriter.Write(textErrror, 0, textErrror.Length)
                End If

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDinners:ExportProfile")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Private Function CreateLine(ByVal cRow As DataRow, ByVal dtAccruals As DataTable, ByVal dtSchedule As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                For i As Integer = 0 To Me.Profile.Fields.Count - 1
                    Dim current = Profile.Fields(i)
                    Select Case current.Source.ToUpper
                        Case "CANTIDAD" : current.Value = cRow("Veces")
                        Case "FECHA" : current.Value = cRow("ShiftDate")
                        Case "HORA" : current.Value = cRow("DateTime")
                        Case "TURNO" : current.Value = "" '"falta"
                        Case Else
                            Select Case current.Source.ToUpper.Split("_")(0)
                                Case "SALDO"
                                    If ((current.Source.ToUpper.Split("_").Length > 2) AndAlso (current.Source.ToUpper.Split("_")(1) = "V")) Then
                                        If (cRow.Table.Columns.Contains("ShiftDate")) Then
                                            current.Value = GetConcept_TotalValue(current.Source.ToString.Split("_")(2), dtAccruals, oState, cRow("ShiftDate"))
                                        Else
                                            current.Value = GetConcept_TotalValue(current.Source.ToString.Split("_")(2), dtAccruals, oState)
                                        End If
                                    End If
                                Case "HORARIO"
                                    If (current.Source.ToUpper.Split("_").Length > 1) Then
                                        If (cRow.Table.Columns.Contains("ShiftDate")) Then
                                            current.Value = GetScheduleByDay(dtSchedule, current.Source.ToUpper.Split("_")(1), oState, cRow("ShiftDate"))
                                        Else
                                            current.Value = GetScheduleByDay(dtSchedule, current.Source.ToUpper.Split("_")(1), oState)
                                        End If
                                    End If
                            End Select
                    End Select
                Next i

                ' Graba la línea
                Me.Profile.CreateLine()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDinners:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim bolGroup1 As Boolean = False
                Dim i As Integer = 0
                Dim n As Integer = 0

                Dim dt As New DataTable

                ' Determina el empleado
                Dim idEmpleado As Long = row("idEmployee")
                mRegisterType = RegisterType.OneByEmployee

                ' Para cada columna
                For i = 0 To Me.Profile.Fields.Count - 1
                    Profile.Fields(i).Value = ""

                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Profile.Fields(i).Value = mBeginDate

                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Profile.Fields(i).Value = mEndDate

                        Case "GRUPO"
                            Profile.Fields(i).Value = row("GroupName")

                        Case "GRUPO_COMPLETO"
                            Profile.Fields(i).Value = row("FullGroupName")

                        Case "NOMBRE"
                            Profile.Fields(i).Value = row("EmployeeName")

                        Case "CONTRATO"
                            Profile.Fields(i).Value = row("idContract")

                        Case "CONTRATO_FECHA_INICIO"
                            Profile.Fields(i).Value = row("BeginDate")

                        Case "CONTRATO_FECHA_FINAL"
                            Profile.Fields(i).Value = row("EndDate")

                        Case "FECHA"
                            mRegisterType = RegisterType.OneByDay

                        Case "HORA", "TURNO"
                            mRegisterType = RegisterType.OneByRegister

                        Case Else
                            ' Determina el tipo de campo
                            If InStr(1, Profile.Fields(i).Source.ToUpper, "USR_") Then
                                ' Lee el dato del campo de la ficha
                                If dtEmpUsrFields.Rows.Count > 0 Then
                                    Dim FieldName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Dim r() As DataRow = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                                    If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                End If
                            Else
                                ' Literal
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                    Profile.Fields(i).Value = Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8)
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
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDinners:LoadInfo")
            End Try

            Return bolRet
        End Function

        Private Function LoadRegisters(ByVal Row As DataRow) As DataTable
            'Dim bolRet As Boolean = False
            Dim bd As Date = IIf(Me.mBeginDate > Row("BeginDate"), Me.mBeginDate, Row("BeginDate"))
            Dim ed As Date = IIf(Me.mEndDate < Row("EndDate"), Me.mEndDate, Row("EndDate"))
            Dim dt As DataTable = Nothing

            Try
                ' Crea el adaptador si no está definido
                If IsNothing(mdaDinners) Then mdaDinners = CreateDataAdapter_Dinners()

                ' Lee todos los saldos del empleado entre fechas
                dt = New DataTable
                With mdaDinners.SelectCommand
                    .Parameters("@idEmployee").Value = Row("idEmployee")
                    .Parameters("@BeginDate").Value = bd
                    .Parameters("@EndDate").Value = ed
                End With

                dt = New DataTable
                mdaDinners.Fill(dt)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDinners:LoadDinners")
            End Try

            Return dt
        End Function

        Private Function CreateDataAdapter_Dinners() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = ""

                ' Filtro si debo mostrar offlines
                Dim sFilterOfflines As String = String.Empty
                If mIncludeOfflines Then
                    sFilterOfflines = "and (InvalidType is NULL or InvalidType=7)"
                Else
                    sFilterOfflines = "and InvalidType is NULL"
                End If

                Select Case mRegisterType
                    Case RegisterType.OneByEmployee
                        strSQL = "@SELECT# COUNT(*) AS Veces FROM dbo.Punches " &
                                 "WHERE  idEmployee=@idEmployee and (ShiftDate between @BeginDate and @EndDate) and Type=10 " & sFilterOfflines & " "

                    Case RegisterType.OneByDay
                        strSQL = "@SELECT# COUNT(*) AS Veces, ShiftDate FROM dbo.Punches " &
                                 "WHERE  idEmployee=@idEmployee and (ShiftDate between @BeginDate and @EndDate) and Type=10 " & sFilterOfflines & " " &
                                 "GROUP BY ShiftDate ORDER BY ShiftDate"

                    Case RegisterType.OneByRegister
                        strSQL = "@SELECT# 1 AS Veces, ShiftDate, DateTime FROM dbo.Punches " &
                                 "WHERE  idEmployee=@idEmployee and (ShiftDate between @BeginDate and @EndDate) and Type=10 " & sFilterOfflines & " " &
                                 "ORDER BY DateTime"

                End Select

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportDinners:CreateDataAdapter_Dinners")
            End Try

            Return da
        End Function

#End Region

        Private Function LoadAccrualsForEmployee(ByVal idEmployee As Integer, ByVal BeginDate As Date, ByVal EndDate As Date) As DataTable
            'Dim bolRet As Boolean = False
            Dim bd As Date = IIf(Me.mBeginDate > BeginDate, Me.mBeginDate, BeginDate)
            Dim ed As Date = IIf(Me.mEndDate < EndDate, Me.mEndDate, EndDate)
            Dim dt As DataTable = Nothing

            Try
                ' Cargo saldos. Si debo mostrar ceros, o bien si debo incluir todas las fechas porque se quiere detalle de fichajes o planificación, lo indico.
                If IsNothing(mdaDailyAccruals) Then mdaDailyAccruals = CreateDataAdapter_ConceptsEx(True, True, True, Profile, oState)

                ' Lee todos los saldos del empleado entre fechas
                With mdaDailyAccruals.SelectCommand
                    .Parameters("@idEmployee").Value = idEmployee
                    .Parameters("@BeginDate").Value = bd
                    .Parameters("@EndDate").Value = ed
                End With

                dt = New DataTable
                mdaDailyAccruals.Fill(dt)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadConcepts")
            End Try

            Return dt
        End Function


        Private Function GetConcept_TotalValue(ByVal ShortName As String, ByVal dt As DataTable, oState As roDataLinkState, Optional DateAccrual As Date = #12:00:00 AM#) As Double
            Dim t As Double = 0

            Try
                Dim i As Integer = 0
                Dim rows() As DataRow

                If DateAccrual = #12:00:00 AM# Then
                    rows = dt.Select("ShortName='" & ShortName & "'")
                Else
                    rows = dt.Select("ShortName='" & ShortName & "' and Date=#" & DateAccrual.Month & "/" & DateAccrual.Day & "/" & DateAccrual.Year & "#")
                End If

                ' Suma el total
                For i = 0 To rows.Length - 1
                    t += rows(i)("TotalConcept")
                Next
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConcept_TotalValue")
            End Try

            Return t
        End Function

        Private Function GetScheduleByDay(ByVal dt As DataTable, Info As String, oState As roDataLinkState, Optional DateSchedule As Date = #12:00:00 AM#) As String

            Dim ret As String = String.Empty
            Dim oScheduleDataView As System.Data.DataView = Nothing
            Try

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    oScheduleDataView = New System.Data.DataView(dt)
                    oScheduleDataView.Sort = "Date ASC"
                End If
                If oScheduleDataView IsNot Nothing AndAlso oScheduleDataView.Table IsNot Nothing AndAlso oScheduleDataView.ToTable.Rows.Count > 0 Then
                    Dim oScheduleRow As DataRow
                    oScheduleDataView.RowFilter = "Date = '" & Format(DateSchedule, "yyyy/MM/dd") & "'"
                    If oScheduleDataView.ToTable.Rows.Count > 0 Then
                        oScheduleRow = oScheduleDataView.ToTable.Rows(0)
                        ' Si el día está calculado, me quedo con ShiftUsed. Si no, con el Shift1
                        If Not IsDBNull(oScheduleRow("IDShiftUsed")) AndAlso Not IsDBNull(oScheduleRow("IDShift1")) Then
                            If Not IsDBNull(oScheduleRow("IDShiftUsed")) Then
                                Select Case Info.ToUpper
                                    Case "NOMBRE"
                                        ret = oScheduleRow("UsedName")
                                    Case "NC"
                                        ret = oScheduleRow("UsedShortName")
                                    Case "TEO"
                                        ret = oScheduleRow("ExpectedWorkingHoursUsedShift")
                                End Select
                            Else
                                Select Case Info.ToUpper
                                    Case "NOMBRE"
                                        ret = oScheduleRow("Name1")
                                    Case "NC"
                                        ret = oScheduleRow("ShortName1")
                                    Case "TEO"
                                        ret = oScheduleRow("ExpectedWorkingHours1")
                                End Select
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::GetScheduleByDay")
            End Try

            Return ret
        End Function

        Private Function CreateDataAdapter_ConceptsEx(ByVal includeZeros As Boolean, mExportConceptsWithDate As Boolean, mExportMoreThatOneConceptByLine As Boolean, Profile As ProfileExportBody, oState As roDataLinkState) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = ""

                If mExportConceptsWithDate Then
                    ' Exportación de saldos con fecha
                    strSQL = "WITH alldays AS ( " &
                                "@SELECT# @BeginDate AS dt " &
                                    "UNION ALL " &
                                "@SELECT# DATEADD(dd, 1, dt) " &
                                    "FROM alldays s " &
                                    "WHERE DATEADD(dd, 1, dt) <= @EndDate) " &
                            "@SELECT# ShortName AS ShortName, dateConcept.Date AS Date, isnull(dbo.DailyAccruals.Value,0) AS TotalConcept  " &
                            "FROM (@SELECT# alldays.dt AS Date, Concepts.ID AS IDConcept, Concepts.ShortName AS ShortName, @idEmployee AS IDEmployee from alldays,Concepts WHERE Concepts.Export<>'0' "

                    If mExportMoreThatOneConceptByLine Then
                        ' Exportación de saldos con fecha con varios saldos por linea
                        strSQL &= " and (" & GetConceptsByLineDelimited(Profile) & ") "
                    End If

                    strSQL &= ") dateConcept "

                    If includeZeros Then
                        strSQL &= " LEFT OUTER JOIN "
                    Else
                        strSQL &= " INNER JOIN "
                    End If

                    strSQL &= "DailyAccruals ON dateConcept.Date = DailyAccruals.Date AND DailyAccruals.IDConcept = dateConcept.IDConcept AND dateConcept.IDEmployee = DailyAccruals.IDEmployee "

                    strSQL &= " ORDER BY dateConcept.Date, ShortName "
                    strSQL &= " OPTION (maxrecursion 0) "
                Else
                    ' Exportación de saldos totalizados
                    strSQL = "WITH alldays AS ( " &
                                "@SELECT# @BeginDate AS dt " &
                                    "UNION ALL " &
                                "@SELECT# DATEADD(dd, 1, dt) " &
                                    "FROM alldays s " &
                                    "WHERE DATEADD(dd, 1, dt) <= @EndDate) " &
                            "@SELECT# SUM(isnull(dbo.DailyAccruals.Value,0)) AS TotalConcept, ShortName AS ShortName " &
                            "FROM (@SELECT# alldays.dt AS Date, Concepts.ID AS IDConcept, Concepts.ShortName AS ShortName, @idEmployee AS IDEmployee from alldays,Concepts "

                    If mExportMoreThatOneConceptByLine Then
                        ' Exportación de saldos con fecha con varios saldos por linea
                        strSQL &= " WHERE Concepts.Export<>'0' and (" & GetConceptsByLineDelimited(Profile) & ") "
                    End If

                    strSQL &= ") dateConcept "

                    If includeZeros Then
                        strSQL &= " LEFT OUTER JOIN "
                    Else
                        strSQL &= " INNER JOIN "
                    End If

                    strSQL &= "DailyAccruals ON dateConcept.Date = DailyAccruals.Date AND DailyAccruals.IDConcept = dateConcept.IDConcept AND dateConcept.IDEmployee = DailyAccruals.IDEmployee "

                    ' Group by
                    strSQL &= "GROUP BY ShortName"
                    strSQL &= " OPTION (maxrecursion 0) "
                End If

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDataLinkExport::CreateDataAdapter_Saldos")
            End Try

            Return da
        End Function

        Private Function GetConceptsByLineDelimited(Profile As ProfileExportBody) As String
            ' Determina si hay conceptos individuales
            Dim aux As String = ""
            Dim ConceptsByLine As New List(Of String)
            Dim i As Integer = 0

            ConceptsByLine = GetConceptsByLine(Profile)
            For i = 0 To ConceptsByLine.Count - 1
                If i > 0 Then aux += " or "
                aux += "dbo.Concepts.ShortName='" & ConceptsByLine(i) & "'"
            Next

            Return aux
        End Function

        Private Function GetConceptsByLine(Profile As ProfileExportBody) As List(Of String)
            Dim IndivConcepts As New List(Of String)
            Dim i As Integer = 0

            ' Lee los saldos individuales
            For i = 0 To Profile.Fields.Count - 1
                If Profile.Fields(i).Source.ToUpper.StartsWith("SALDO_V_") Then
                    Dim j As Integer
                    ' Comprueba que el saldo no exista
                    For j = 0 To IndivConcepts.Count - 1
                        If IndivConcepts(j) = Profile.Fields(i).Source.Substring(8) Then Exit For
                    Next

                    ' Nuevo saldo
                    If j = IndivConcepts.Count Then IndivConcepts.Add(Profile.Fields(i).Source.Substring(8))
                End If
            Next

            Return IndivConcepts
        End Function

    End Class

End Namespace