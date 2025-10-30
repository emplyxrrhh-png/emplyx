Imports System.Data.Common
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTHolidays
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Base.VTBusiness.DiningRoom

Namespace DataLink
    Public Class ProfileExportConcepts

#Region "Declarations- Constructor"

        Private mEmployeeTempTableName As String = ""
        Private mBeginDay As Integer = 0
        Private mBeginDate As Date = #12:00:00 AM#
        Private mEndDate As Date = #12:00:00 AM#

        Private mBody As ProfileExportBody

        Private mdaDailyAccruals As DbDataAdapter = Nothing
        Private mdaDiningAccruals As DbDataAdapter = Nothing

        Private taAccruals As DataAdapter = Nothing

        Private mExcelFileName As String
        Private mExportConceptsWithDate As Boolean = False
        Private mExportMoreThatOneConceptByLine As Boolean = False
        Private mOutputFileType As ProfileExportBody.FileTypeExport = Nothing
        Private mAccrualsFilteredBy As Integer
        Private mBreakingBy As String
        Private mBreakingByMobility As Boolean
        Private mBreakingByContract As Boolean

        Private mExportZeroValues As Boolean = False
        Private mOnlyDepartamentsOnLevel As Integer = -1
        Private mIncludePunches As Boolean = False
        Private mIncludeDining As Boolean = False
        Private mIncludeEmployeeAccrualPeriods As Boolean = False
        Private mIncludeSchedule As Boolean = False
        Private mEspGuideName As String = ""

        Private mPeriodPattern As String = String.Empty

        Private dblField1 As Nullable(Of Double) = Nothing
        Private dblField2 As Nullable(Of Double) = Nothing
        Private strField3 As String = Nothing
        Private strField4 As String = Nothing
        Private xField5 As Nullable(Of Date) = Nothing
        Private xField6 As Nullable(Of Date) = Nothing

        Private ilastIDEmployee As Integer = -1
        Private iAccrualValueOrder As Integer = 0
        Private iTotalLines As Integer = 0
        Private mHeader As String = ""
        Private mHeaderA3 As String = ""
        Private mSortColumnIndex As Integer = 0
        Private mSortAscending As Boolean = False
        Private mSortRowsOffset As Integer = 0
        Private mFutureConceptsHipotels As Boolean = False
        Private mContractsTerminations As Boolean = False
        Private mContractTerminationCheckDaysInAdvance As Integer = 1
        Private mOnlyActiveContractsData As Boolean = False

        Private oState As roDataLinkState
        Private oLog As New roLog("ProfileExportConcepts")

        Public Sub New(ByVal employeeTempTableName As String, ByVal strExcelFileName As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDay As Integer, ByVal _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeTempTableName = employeeTempTableName
            mBeginDay = BeginDay
            mOutputFileType = OutputFileType
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
            mExcelFileName = strExcelFileName
        End Sub

        ' Lanzamiento manual
        Public Sub New(ByVal employeeTempTableName As String, ByVal strExcelFileName As String, OutputFileName As String, OutputFileType As ProfileExportBody.FileTypeExport, ByVal DelimitedChar As String, ByVal BeginDate As Date, ByVal EndDate As Date, _State As roDataLinkState,
                   Optional ByVal Field1 As Nullable(Of Double) = Nothing, Optional ByVal Field2 As Nullable(Of Double) = Nothing, Optional ByVal Field3 As String = Nothing, Optional ByVal Field4 As String = Nothing, Optional ByVal Field5 As Nullable(Of Date) = Nothing, Optional ByVal Field6 As Nullable(Of Date) = Nothing)

            Me.dblField1 = Field1
            Me.dblField2 = Field2
            Me.strField3 = Field3
            Me.strField4 = Field4
            Me.xField5 = Field5
            Me.xField6 = Field6

            Me.oState = IIf(_State Is Nothing, New roDataLinkState(), _State)
            mEmployeeTempTableName = employeeTempTableName
            mBeginDate = BeginDate
            mEndDate = EndDate
            mOutputFileType = OutputFileType
            mBody = New ProfileExportBody(OutputFileName, OutputFileType, DelimitedChar, _State)
            mExcelFileName = strExcelFileName
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

        Public Property OutputFileType() As Boolean
            Get
                Return Me.mOutputFileType
            End Get
            Set(ByVal NewValue As Boolean)
                Me.mOutputFileType = NewValue
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

        Public Property AccrualsFilteredBy() As Integer
            Get
                Return Me.mAccrualsFilteredBy
            End Get
            Set(ByVal value As Integer)
                Me.mAccrualsFilteredBy = value
            End Set
        End Property

        Public Property BreakingBy() As String
            Get
                Return Me.mBreakingBy
            End Get

            Set(ByVal value As String)
                Me.mBreakingBy = value
            End Set
        End Property

        Public Property BreakingByMobility() As Boolean
            Get
                Return mBreakingByMobility
            End Get

            Set(ByVal value As Boolean)
                mBreakingByMobility = value
            End Set
        End Property

        Public Property BreakingByContract() As Boolean
            Get
                Return mBreakingByContract
            End Get

            Set(ByVal value As Boolean)
                mBreakingByContract = value
            End Set
        End Property

        Public Property ExportZeroValues() As Boolean
            Get
                Return Me.mExportZeroValues
            End Get

            Set(ByVal value As Boolean)
                Me.mExportZeroValues = value
            End Set
        End Property

        Public Property OnlyDepartamentsOnLevel() As Integer
            Get
                Return Me.mOnlyDepartamentsOnLevel
            End Get

            Set(ByVal value As Integer)
                Me.mOnlyDepartamentsOnLevel = value
            End Set
        End Property

        Public Property IncludePunches() As Boolean
            Get
                Return Me.mIncludePunches
            End Get

            Set(ByVal value As Boolean)
                Me.mIncludePunches = value
            End Set
        End Property

        Public Property IncludeEmployeeAccrualPeriods As Boolean
            Get
                Return Me.mIncludeEmployeeAccrualPeriods
            End Get
            Set(value As Boolean)
                Me.mIncludeEmployeeAccrualPeriods = value
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

        Public Property PeriodPattern() As String
            Get
                Return Me.mPeriodPattern
            End Get

            Set(ByVal value As String)
                Me.mPeriodPattern = value
            End Set
        End Property

        Public Property EspGuideName() As String
            Get
                Return Me.mEspGuideName
            End Get

            Set(ByVal value As String)
                Me.mEspGuideName = value
            End Set
        End Property

        Public Property HeaderLine() As String
            Get
                Return Me.mHeader
            End Get

            Set(ByVal value As String)
                Me.mHeader = value
            End Set
        End Property

        Public Property HeaderLineA3() As String
            Get
                Return Me.mHeaderA3
            End Get

            Set(ByVal value As String)
                Me.mHeaderA3 = value
            End Set
        End Property

        Public Property SortColumnIndex As Integer
            Get
                Return Me.mSortColumnIndex
            End Get
            Set(value As Integer)
                Me.mSortColumnIndex = value
            End Set
        End Property

        Public Property SortAscending As Boolean
            Get
                Return Me.mSortAscending
            End Get
            Set(value As Boolean)
                Me.mSortAscending = value
            End Set
        End Property

        Public Property SortRowsOffset As Integer
            Get
                Return Me.mSortRowsOffset
            End Get
            Set(value As Integer)
                Me.mSortRowsOffset = value
            End Set
        End Property

        Public Property FutureConceptsHipotels As Boolean
            Get
                Return Me.mFutureConceptsHipotels
            End Get
            Set(value As Boolean)
                Me.mFutureConceptsHipotels = value
            End Set
        End Property

        Public Property ContractTerminations As Boolean
            Get
                Return Me.mContractsTerminations
            End Get
            Set(value As Boolean)
                Me.mContractsTerminations = value
            End Set
        End Property


        Public Property OnlyActiveContractsData As Boolean
            Get
                Return Me.mOnlyActiveContractsData
            End Get
            Set(value As Boolean)
                Me.mOnlyActiveContractsData = value
            End Set
        End Property

        Public Property ContractTerminationCheckDaysInAdvance As Integer
            Get
                Return Me.mContractTerminationCheckDaysInAdvance
            End Get
            Set(value As Integer)
                Me.mContractTerminationCheckDaysInAdvance = value
            End Set
        End Property
        Public Property IncludeDining() As Boolean
            Get
                Return Me.mIncludeDining
            End Get

            Set(ByVal value As Boolean)
                Me.mIncludeDining = value
            End Set
        End Property


#End Region

#Region "Methods"

        Public Function ExportProfileIE(ByVal intIDExport As Integer, ByVal strTemplateFileName As String, Optional ByVal bIsAuto As Boolean = False) As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try

                ' IVECO - Determina la fecha inicial y final para exportación automática (no tiene en cuenta lo definido en la tarea planificada
                If bIsAuto AndAlso VTBusiness.Common.roBusinessSupport.GetCustomizationCode().ToUpper() = "TAIF" Then
                    If intIDExport = 20002 AndAlso (Me.EspGuideName.Contains("SAPF3") OrElse Me.EspGuideName.Contains("SAP IF3")) AndAlso Me.xField5.HasValue Then
                        ' Exportación para SAP IF3 de saldos
                        If Me.xField5.Value.Day <= 17 Then
                            Me.mBeginDate = DateSerial(Me.xField5.Value.AddMonths(-1).Year, Me.xField5.Value.AddMonths(-1).Month, 1)
                            Me.mEndDate = Me.xField5.Value.AddDays(-1)
                        Else
                            Me.mBeginDate = DateSerial(Me.xField5.Value.Year, Me.xField5.Value.Month, 1)
                            Me.mEndDate = Me.xField5.Value.AddDays(-1)
                        End If
                    End If
                End If

                ' ICA Group - Determina la fecha inicial y final para exportación automática (no tiene en cuenta lo definido en la tarea planificada
                If bIsAuto Then
                    If intIDExport = 20002 AndAlso Me.EspGuideName.Contains("SAP ADP ICA Group") Then
                        ' Exportación especial para cliente IAC
                        If Now.Date.Day >= 1 AndAlso Now.Date.Day <= 15 Then
                            ' Exporto segunda quincena del mes anterior
                            mBeginDate = DateSerial(Now.AddMonths(-1).Year, Now.AddMonths(-1).Month, 16)
                            mEndDate = DateSerial(Now.Year, Now.Month, 1).AddDays(-1)
                        Else
                            ' Exporto primera quincena del mes en curso
                            mBeginDate = DateSerial(Now.Year, Now.Month, 1)
                            mEndDate = DateSerial(Now.Year, Now.Month, 15)
                        End If
                    End If
                End If


                If Me.mBeginDate = #12:00:00 AM# Then
                    ' TODO: Este código nunca se ejecuta en HA. Las fechas en automático son las definidas en la tarea planificada. Pero se pierde la posibilidad de definir patrón de periodo en la plantilla (lo tenía algún cliente ...). No se activa por prudencia ...
                    If Me.PeriodPattern.Trim.Length > 0 AndAlso (Me.PeriodPattern.IndexOf(",") > 0 OrElse Me.PeriodPattern.IndexOf(".") > 0) Then
                        ' Si me pasaron una definición de periodo de datos, la interpreto ahora ....
                        roDataLinkExport.GetExportPeriodFromPattern(Me.PeriodPattern.Replace(".", ","), Now, mBeginDate, mEndDate)
                    Else
                        ' Exportación automática
                        Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)
                        ' La exportación siempre es del mes anterior
                        Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                        ' Si el dia de inicio de exportación es posterior al dia en que se lanza se debe restar un mes adicional para obtener el valor del mes entero.
                        If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                        Me.mEndDate = Me.mBeginDate.AddMonths(1)
                        Me.mEndDate = Me.mEndDate.AddDays(-1)
                    End If
                End If

                If Me.EspGuideName.Contains("SAP ADP ICA Group") AndAlso Me.Profile.DelimitedChar.Trim.Length = 1 Then Me.Profile.DelimitedChar = "|"""

                ' Campos de la ficha
                Dim dtEmpUsrFields As DataTable = Nothing

                ' Niveles del empleado
                Dim dtEmpGroups As DataTable = Nothing

                ' Obtiene los saldos
                Dim dtAllConcepts As DataTable = CreateDataTable("@SELECT# id, Name, Description, idType, ShortName, Export, DefaultQuery from concepts")

                If Not Profile.FileOpen(, mHeaderA3) Then Exit Try

                ' Crea el fichero de salida
                bolCloseFile = True

                ' Tabla para valores de la ficha por empleado
                Dim dtEmployeeUserFieldValuesHistory As New DataTable
                Dim dtMobilities As New DataTable

                Dim dicVacationsShifts As New Dictionary(Of String, Integer)
                Dim dicVacationsShiftsType As New Dictionary(Of String, String)
                Dim dicVacationsCauses As New Dictionary(Of String, Integer)

                ' Miro si debo ordenar por nombre de empleado o por nombre de departamento (si se incluye)
                ' Aprovecho para recopilar campos de la ficha usados, por si debo informar datatable temporal de valores de la ficha para empleados ...
                Dim bOrderByGroup As Boolean = False
                Dim lUsrFld As New List(Of String)
                mExportConceptsWithDate = False
                mExportMoreThatOneConceptByLine = False
                For i = 0 To Profile.Fields.Count - 1
                    If Profile.Fields(i).Source.ToUpper.StartsWith("GRUPO") Then
                        bOrderByGroup = True
                    End If
                    If Profile.Fields(i).Source.ToUpper.StartsWith("NOMBRE") Then
                        ' Ordeno por empleado
                        bOrderByGroup = False
                    End If
                    If Me.Profile.Fields(i).Source.StartsWith("USR_") Then
                        lUsrFld.Add(Me.Profile.Fields(i).Source.Substring(4, Me.Profile.Fields(i).Source.Length - 4))
                    End If
                    If InStr(Profile.Fields(i).Source.ToUpper, "SALDO_FECHA") > 0 Then mExportConceptsWithDate = True
                    If Profile.Fields(i).Source.Length >= 4 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 4) = "VAC_" AndAlso Profile.Fields(i).Source.Split("_").Count > 2 Then
                        ' Detalle de saldos de vacaciones
                        If Not dicVacationsShifts.ContainsKey(Profile.Fields(i).Source.Split("_")(1)) Then
                            Dim iVacShiftId As Integer = 0
                            Dim strSQL = "@SELECT# id from shifts where IsObsolete = 0 and ShortName = '" & Profile.Fields(i).Source.ToUpper.Split("_")(1) & "'"
                            iVacShiftId = Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                            If iVacShiftId > 0 Then dicVacationsShifts.Add(Profile.Fields(i).Source.Split("_")(1), iVacShiftId)
                        End If
                        If Not dicVacationsShiftsType.ContainsKey(Profile.Fields(i).Source.Split("_")(1)) Then
                            Dim iVacConceptType As String = ""
                            Dim strSQL = "@SELECT# DefaultQuery from Concepts INNER JOIN Shifts ON Shifts.idConceptBalance = Concepts.Id where IsObsolete = 0 and Shifts.ShortName = '" & Profile.Fields(i).Source.ToUpper.Split("_")(1) & "'"
                            iVacConceptType = Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                            If iVacConceptType <> "" Then dicVacationsShiftsType.Add(Profile.Fields(i).Source.Split("_")(1), iVacConceptType)
                        End If
                        mExportMoreThatOneConceptByLine = True
                    End If

                    If Profile.Fields(i).Source.Length >= 5 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 5) = "VACH_" AndAlso Profile.Fields(i).Source.Split("_").Count > 2 Then
                        ' Detalle de saldos de vacaciones por horas
                        If Not dicVacationsCauses.ContainsKey(Profile.Fields(i).Source.Split("_")(1)) Then
                            Dim iVacCauseId As Integer = 0
                            Dim strSQL = "@SELECT# id from causes where ShortName = '" & Profile.Fields(i).Source.ToUpper.Split("_")(1) & "'"
                            iVacCauseId = Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                            If iVacCauseId > 0 Then dicVacationsCauses.Add(Profile.Fields(i).Source.Split("_")(1), iVacCauseId)
                        End If
                        mExportMoreThatOneConceptByLine = True
                    End If
                Next
                Dim sUsrFilter As String = String.Empty
                For Each sFieldName As String In lUsrFld
                    If sUsrFilter <> String.Empty Then sUsrFilter = $"{sUsrFilter},"
                    sUsrFilter = $"{sUsrFilter}'{sFieldName}'"
                Next

                ' Selecciona los empleados con contrato activo
                Dim sSort As String = "EmployeeName, Begindate"
                Dim sSQL = String.Empty

                ' Para el caso de finiquitos ...
                Dim dTerminateDate As Date = Now.Date.AddDays(-1 * Me.ContractTerminationCheckDaysInAdvance)
                If Me.ContractTerminations Then
                    If Not bIsAuto Then
                        'PBI 1370373, si la exportación con finiquito es manual, miramos las bajas de la fecha seleccionada
                        dTerminateDate = mEndDate
                    Else
                        ' ... si se lanzó automáticamente, siempre trato las bajas de ayer, que ya he calculado más arriba
                    End If
                    ' Sea como sea, siempre acumulo hasta la fecha de fin resultante
                    mEndDate = dTerminateDate
                End If

                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "ProfileExportConcepts::ExportProfileIE:ExportTemplateID = " & strTemplateFileName)

                If strTemplateFileName.Contains("Daily") Then
                    ' Saldos diarios. Considero todo empleado que tenga un día de contrato en el periodo de la exportación
                    sSQL = "@SELECT# seg.IDEmployee, EmployeeName, GroupName, FullGroupName, dbo.EmployeeContracts.IDContract, " &
                            "dbo.EmployeeContracts.BeginDate, EmployeeContracts.Enddate, EmployeeContracts.EndContractReason  " &
                            "FROM dbo.sysrovwCurrentEmployeeGroups seg " &
                            "INNER JOIN dbo.EmployeeContracts On seg.IDEmployee = dbo.EmployeeContracts.IDEmployee "

                    If Me.mEmployeeTempTableName <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeTempTableName & " ON " & Me.mEmployeeTempTableName & ".Id = seg.IDEmployee "
                    sSQL &= "WHERE Not (dbo.EmployeeContracts.EndDate <" & Any2Time(Me.mBeginDate).SQLDateTime &
                            " Or dbo.EmployeeContracts.BeginDate > " & Any2Time(Me.mEndDate).SQLDateTime & ") "

                    If Me.ContractTerminations Then
                        ' Finiquito PNET
                        If bIsAuto Then
                            sSQL += " AND (EmployeeContracts.EndDate = " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime & " OR (EmployeeContracts.EndDate > " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime & " AND DATEPART(YEAR,EmployeeContracts.EndDate) <> 2079 AND EmployeeContracts.BeginDate <= " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime & "))"
                        Else
                            sSQL += " AND EmployeeContracts.EndDate = " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime
                        End If
                    ElseIf Me.OnlyActiveContractsData Then
                        ' Nómina PNET. Sólo se envía info de contratos activos a fecha de exportación
                        sSQL += " AND CONVERT(Date, GETDATE()) BETWEEN dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate "
                    End If

                    'If Me.mEmployeesFilter <> "" Then sSQL += " And seg.IDEmployee In (" & mEmployeesFilter & ")"
                Else
                    ' Saldos a fecha. Considero todo empleado que tenga contrato el día al que extraigo los saldos
                    sSQL = "@SELECT# seg.IDEmployee, Employees.Name EmployeeName, Groups.Name GroupName, Groups.FullGroupName FullGroupName, dbo.EmployeeContracts.IDContract, " &
                            "dbo.EmployeeContracts.BeginDate, EmployeeContracts.Enddate, EmployeeContracts.EndContractReason " &
                            "FROM dbo.sysroEmployeeGroups seg  "

                    If Me.mEmployeeTempTableName <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeTempTableName & " ON " & Me.mEmployeeTempTableName & ".Id = seg.IDEmployee "

                    sSQL &= "INNER JOIN Employees ON Employees.ID = seg.IDEmployee " &
                            "INNER JOIN Groups ON Groups.ID = seg.IDGroup " &
                            "INNER JOIN dbo.EmployeeContracts On seg.IDEmployee = dbo.EmployeeContracts.IDEmployee " &
                            "WHERE "

                    If Me.ContractTerminations Then
                        If bIsAuto Then
                            sSQL += " (EmployeeContracts.EndDate = " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime & " OR (EmployeeContracts.EndDate > " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime & " AND DATEPART(YEAR,EmployeeContracts.EndDate) <> 2079 AND EmployeeContracts.BeginDate <= " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime & ")) AND " & Any2Time(Me.mEndDate).SQLDateTime & " BETWEEN seg.BeginDate AND seg.enddate "
                        Else
                            sSQL += " EmployeeContracts.EndDate = " & roTypes.Any2Time(dTerminateDate).SQLSmallDateTime
                        End If
                    Else
                        sSQL += Any2Time(Me.mEndDate).SQLDateTime & " BETWEEN dbo.EmployeeContracts.BeginDate AND dbo.EmployeeContracts.EndDate " &
                            "AND " & Any2Time(Me.mEndDate).SQLDateTime & " BETWEEN seg.BeginDate AND seg.enddate "
                    End If

                    'If Me.mEmployeesFilter <> "" Then sSQL += " And seg.IDEmployee In (" & mEmployeesFilter & ")"
                End If

                If bOrderByGroup Then
                    sSQL += " ORDER BY FullGroupName, EmployeeName, EmployeeContracts.begindate"
                    sSort = "FullGroupName, EmployeeName, Begindate"
                Else
                    sSQL += " ORDER BY EmployeeName, EmployeeContracts.begindate"
                    sSort = "EmployeeName, Begindate"
                End If

                Dim dtEmployees As DataTable = CreateDataTable(sSQL, "Employees")
                Dim dtDailyAccruals As DataTable
                Dim dtPunches As New DataTable
                Dim dtSchedule As New DataTable

                Dim oEmployeesState As New Employee.roEmployeeState(Me.State.IDPassport)
                Dim oPunchState As New Punch.roPunchState(Me.State.IDPassport)

                ' ROTURA CAMPOS DE LA FICHA
                If (mBreakingBy <> "") Then bolRet = CreateUserFieldsBreaks(dtEmployees)

                Dim rowS As DataRow() = Nothing
                If dtEmployees IsNot Nothing Then
                    rowS = dtEmployees.Select("", sSort)
                End If

                Dim n As Long = 0
                Dim EmpAnt As Integer = 0
                Dim bDatAnt As Date = Nothing
                Dim bDate As Date = Nothing

                ' Para ICA Group imprimo línea de cabecera
                If Me.EspGuideName.Contains("SAP ADP ICA Group") Then
                    Dim strFecha As String
                    strFecha = mEndDate.AddDays(1).ToString("yyyyMMdd")
                    Dim strHeader As String = mHeader.Replace("@@FECHA@@", strFecha)
                    Dim b() As Byte = System.Text.Encoding.Default.GetBytes(strHeader & vbNewLine)
                    Me.Profile.MemoryStreamWriter.Write(b, 0, b.Length)
                End If


                ' Para cada empleado, contrato y rotura
                Dim iTotEmployees As Integer = 0
                If rowS IsNot Nothing Then
                    iTotEmployees = rowS.Count
                End If

                ' Recupero los saldos que se deben mostrar
                Dim strConceptsFiltered As String = GetConceptsFilteredBy()
                Dim strDiningTurnsFiltered As String = ""
                ' Si es una exportación de finiquito obtengo los horarios asociados a cada saldo de vacaciones
                Dim tbHolidayShifts As DataTable = Nothing
                If Me.ContractTerminations Then
                    Dim strSQL As String = "@select# Shifts.ID as IdShift, IDConceptBalance as IDConcept from Shifts inner join concepts on concepts.ID = IDConceptBalance where ShiftType = 2 "
                    tbHolidayShifts = CreateDataTable(strSQL, )
                End If
                Dim oParams As New roParameters("OPTIONS", True)

                If rowS IsNot Nothing Then
                    ' En el caso que la exportación sea diaria y se deban incluir datos de comedor
                    ' obtenemos los turnos que se tienen que extraer
                    If Not strTemplateFileName.Contains("AtDate") AndAlso Me.IncludeDining Then
                        strDiningTurnsFiltered = GetTurnsFilteredBy()
                    End If

                    For Each Row As DataRow In rowS
                        n += 1
                        oLog.logMessage(roLog.EventType.roDebug, "Exporting concepts for employee number " & n.ToString & " out of " & iTotEmployees.ToString)
                        ' Campos de la ficha
                        bDate = IIf(Row("EndDate") > mEndDate, mEndDate, Row("EndDate"))
                        If EmpAnt <> Row("idEmployee") Or bDatAnt <> bDate Then
                            dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(Row("idEmployee"), bDate, Me.oState)
                            dtEmpGroups = DataLinkDynamicCode.CreateDataTable_Groups(Row("idEmployee"), Me.oState)
                            ' Si es necesario, carga valores de fichajes
                            If EmpAnt <> Row("idEmployee") AndAlso (Me.IncludePunches OrElse Me.IncludeSchedule OrElse Me.mExportConceptsWithDate) Then
                                If Me.IncludePunches Then dtPunches = VTBusiness.Punch.roPunch.GetPunchesPres(Row("idEmployee"), mBeginDate, mEndDate, False, oPunchState)
                                If Me.IncludeSchedule Then dtSchedule = VTBusiness.Scheduler.roScheduler.GetPlan(Row("idEmployee"), mBeginDate, mEndDate, oEmployeesState,, True)
                                If Me.mExportConceptsWithDate Then
                                    dtEmployeeUserFieldValuesHistory = DataLinkDynamicCode.CreateDataTable_EmployeeUserFieldValuesHistory(Row("idEmployee"), sUsrFilter, oState)
                                    dtMobilities = DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(Row("idEmployee"), Me.oState)
                                End If
                            End If
                            EmpAnt = Row("idEmployee")
                            bDatAnt = bDate
                        End If

                        ' Carga datos del registro de empleado
                        If Not LoadInfo(dtEmployees, Row, dtEmpUsrFields, dtEmpGroups, True, Row("EndDate")) Then Exit For

                        ' Carga información de los saldos del empleado para el periodo
                        If strTemplateFileName.Contains("AtDate") Then
                            ' Saldos a fecha
                            dtDailyAccruals = LoadAccrualsForEmployeeAtDate(Row("idEmployee"), strConceptsFiltered, oParams, Me.mEndDate)
                            If Me.ContractTerminations AndAlso dtDailyAccruals IsNot Nothing AndAlso dtDailyAccruals.Columns.Contains("IDConcept") AndAlso tbHolidayShifts IsNot Nothing AndAlso tbHolidayShifts.Rows.Count > 0 Then
                                ' si se están exportando saldos para un finiquito se tienen en cuenta las vacaciones planificadas hasta la fecha de baja
                                For Each cRow As DataRow In dtDailyAccruals.Rows
                                    If tbHolidayShifts.Select("IdConcept = " & cRow("IDConcept")).Count > 0 AndAlso cRow("EndPeriod") IsNot Nothing AndAlso cRow("EndPeriod").ToString() <> "" Then
                                        Dim idShift As Integer = tbHolidayShifts.Select("IdConcept = " & cRow("IDConcept")).FirstOrDefault()("IdShift")

                                        Dim intLasting = roBusinessSupport.GetApprovedButNotTakenHolidays(Row("IDEmployee"), idShift, cRow("Date"), cRow("EndPeriod"), State)
                                        cRow("TotalConcept") = cRow("TotalConcept") - intLasting
                                    End If
                                Next
                            End If

                        Else
                            ' Saldos Diarios
                            dtDailyAccruals = LoadAccrualsForEmployee(Row("idEmployee"), Row("BeginDate"), Row("EndDate"))

                            ' En el caso que exportemos saldos diarios con totalizadores del periodo
                            ' que incluyan un solo saldo por linea
                            ' y se tenga que incluir la info de comedor, la obtenemos ahora
                            If Me.IncludeDining AndAlso Not mExportConceptsWithDate AndAlso Not mExportMoreThatOneConceptByLine AndAlso strDiningTurnsFiltered.Length > 0 Then LoadDiningAccrualsForEmployee(Row("idEmployee"), mBeginDate, mEndDate, dtDailyAccruals, strDiningTurnsFiltered)

                        End If

                        If dtDailyAccruals Is Nothing Then Exit For

                        ' Asigna valores
                        Dim PreviousDate As Date = #12:00:00 AM#

                        If Not mExportMoreThatOneConceptByLine Then
                            ' Indicamos si existen datos de turno de comedor
                            Dim ExistDiningData As Boolean = False
                            If dtDailyAccruals.Columns.Contains("IsDining") Then
                                ExistDiningData = True
                            End If
                            ' Para cada registro de saldos
                            For Each cRow As DataRow In dtDailyAccruals.Rows
                                ' Exporta un saldo por línea
                                ExportOneConceptByLine(dtEmployees, Row, cRow, dtAllConcepts, Row("idEmployee"), ExistDiningData)
                            Next
                        Else
                            ' Exporta varios saldos por línea
                            If mExportConceptsWithDate Then
                                ' con fecha
                                Dim lastRow As DataRow = Nothing
                                Dim previousRow As DataRow = Nothing
                                For Each cRow As DataRow In dtDailyAccruals.Rows
                                    lastRow = cRow
                                    If PreviousDate = #12:00:00 AM# Then PreviousDate = cRow("Date")

                                    ' Comprueba si cambia la fecha
                                    If PreviousDate <> cRow("Date") Then
                                        ' Exporta los saldos de la fecha
                                        ExportConceptsByLine(dtDailyAccruals, dtAllConcepts, Row, previousRow, PreviousDate, Row("idEmployee"), dtPunches, dtSchedule, dtEmployeeUserFieldValuesHistory, dtMobilities)
                                        PreviousDate = cRow("Date")
                                    End If
                                    previousRow = cRow
                                Next
                                ' Exporta la ultima línea
                                If PreviousDate <> #12:00:00 AM# Then
                                    ExportConceptsByLine(dtDailyAccruals, dtAllConcepts, Row, lastRow, PreviousDate, Row("idEmployee"), dtPunches, dtSchedule, dtEmployeeUserFieldValuesHistory, dtMobilities)
                                End If
                            Else
                                Dim cRow As DataRow = Nothing
                                ' sin fecha
                                ExportConceptsByLine(dtDailyAccruals, dtAllConcepts, Row, cRow, , Row("idEmployee"),,,,, dicVacationsShifts, dicVacationsCauses, dicVacationsShiftsType)
                            End If
                        End If
                    Next
                Else
                    oLog.logMessage(roLog.EventType.roDebug, "No se encontraron empleados disponibles en roDataLinkExport:: ProfileExportConcepts:ExportProfileIE")
                End If

                ' Si hace falta ordeno
                If Me.SortColumnIndex > 0 Then Profile.SortByColumnIndex(Me.SortColumnIndex, Me.SortAscending, Me.SortRowsOffset)

                ' Para ICA Group imprimo línea de cabecera
                If Me.EspGuideName.Contains("SAP ADP ICA Group") Then
                    Dim b() As Byte = System.Text.Encoding.Default.GetBytes("TRAIL|""" & (iTotalLines + 2).ToString)
                    Me.Profile.MemoryStreamWriter.Write(b, 0, b.Length)
                End If


                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()

                bolRet = True

                'Se deja un mensaje informativo dentro del archivo ascii
                If Profile.MemoryStreamWriter IsNot Nothing AndAlso Profile.MemoryStreamWriter.Length = 0 Then
                    Dim textErrror As Byte() = System.Text.Encoding.Unicode.GetBytes(Me.oState.Language.Translate("roDataLinkExport.ExportProfile.NoInfo", ""))
                    Profile.MemoryStreamWriter.Write(textErrror, 0, textErrror.Length)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport:: ProfileExportConcepts:ExportProfileIE")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()

            End Try

            Return bolRet

        End Function

        Public Function ExportProfileOnlyDepartments() As Boolean
            Dim bolCloseFile As Boolean = False
            Dim bolRet As Boolean = False

            Try

                ' Determina la fecha inicial y final para importación automática
                If Me.mBeginDate = #12:00:00 AM# Then

                    If Me.strField3 IsNot Nothing AndAlso Me.strField3.ToUpper = "TAIF" Then
                        Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/1")
                        Me.mEndDate = CDate(Now.Year & "/" & Now.Month & "/" & Now.Day)
                    Else
                        ' Importación automática
                        Me.mBeginDate = CDate(Now.Year & "/" & Now.Month & "/" & Me.mBeginDay)
                        ' La exportación siempre es del mes anterior
                        Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                        ' Si el dia de inicio de exportación es posterior al dia en que se lanza se debe restar un mes adicional para obtener el valor del mes entero.
                        If Now.Day < Me.mBeginDay Then Me.mBeginDate = Me.mBeginDate.AddMonths(-1)
                        Me.mEndDate = Me.mBeginDate.AddMonths(1)
                        Me.mEndDate = Me.mEndDate.AddDays(-1)
                    End If
                End If

                ' Campos de la ficha
                Dim dtEmpUsrFields As DataTable = Nothing
                ' Niveles del empleado
                Dim dtEmpGroups As DataTable = Nothing

                ' Obtiene los saldos
                Dim dtAllConcepts As DataTable = CreateDataTable("@SELECT# id, Name, Description, idType, ShortName, Export from concepts")

                ' Crea el fichero de salida
                If Profile.FileOpen() = False Then Exit Try
                bolCloseFile = True

                ' Selecciona los empleados con contrato activo

                Dim sSQL As String
                ' Se crea casuistica para Fiat donde no se retorna el FullGroupName para que solo retorne una línea por grupo en su caso
                Dim customization As String = roBusinessSupport.GetCustomizationCode().ToUpper()
                If roTypes.Any2String(customization) = "TAIF" Then
                    sSQL = "@SELECT# distinct dbo.UFN_SEPARATES_COLUMNS(FullGroupName," & (mOnlyDepartamentsOnLevel + 1).ToString & ",'\') GroupNameOnLevel " &
                       " FROM dbo.sysrovwCurrentEmployeeGroups "

                    If Me.mEmployeeTempTableName <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeTempTableName & " ON " & Me.mEmployeeTempTableName & ".Id = sysrovwCurrentEmployeeGroups.IDEmployee "

                    sSQL &= " INNER JOIN dbo.EmployeeContracts ON dbo.sysrovwCurrentEmployeeGroups.IDEmployee = dbo.EmployeeContracts.IDEmployee " &
                        " WHERE NOT (dbo.EmployeeContracts.EndDate <" & roTypes.Any2Time(Me.mBeginDate).SQLDateTime &
                        " OR dbo.EmployeeContracts.BeginDate > " & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ") "
                Else
                    sSQL = "@SELECT# distinct dbo.UFN_SEPARATES_COLUMNS(FullGroupName," & (mOnlyDepartamentsOnLevel + 1).ToString & ",'\') GroupNameOnLevel, FullGroupName " &
                       ", dbo.GetNumberEmployeesFromGroupLevel(FullGroupName," & (mOnlyDepartamentsOnLevel + 1).ToString & "," & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & "," & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ") as NumberEmployeesFromGroupLevel " &
                       " FROM dbo.sysrovwCurrentEmployeeGroups "

                    If Me.mEmployeeTempTableName <> "" Then sSQL &= " INNER JOIN " & Me.mEmployeeTempTableName & " ON " & Me.mEmployeeTempTableName & ".Id = sysrovwCurrentEmployeeGroups.IDEmployee "

                    sSQL &= " INNER JOIN dbo.EmployeeContracts ON dbo.sysrovwCurrentEmployeeGroups.IDEmployee = dbo.EmployeeContracts.IDEmployee " &
                        " WHERE NOT (dbo.EmployeeContracts.EndDate <" & roTypes.Any2Time(Me.mBeginDate).SQLDateTime &
                        " OR dbo.EmployeeContracts.BeginDate > " & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ") "
                End If

                'If Me.mEmployeesFilter <> "" Then sSQL += " and sysrovwCurrentEmployeeGroups.IDEmployee in (" & Me.mEmployeesFilter & ")"
                sSQL += " ORDER BY GroupNameOnLevel asc"

                Dim dtEmployees As DataTable = CreateDataTable(sSQL, "Groups")
                Dim dtDailyAccruals As DataTable

                Dim rowS() = dtEmployees.Select("", "GroupNameOnLevel")
                Dim n As Long = 0
                Dim EmpAnt As Integer = 0
                Dim bDatAnt As Date = Nothing
                Dim bDate As Date = Nothing

                ' Para cada empleado, contrato y rotura
                For Each Row As DataRow In rowS
                    n += 1

                    ' Carga datos del registro de empleado
                    If Not LoadInfoOnlyDepartments(Row, dtEmpUsrFields, dtEmpGroups) Then Exit For

                    ' Si sólo quiero Departamentos ...
                    dtDailyAccruals = LoadAccrualsForDepartmentLevel(Me.mEmployeeTempTableName, Row("GroupNameOnLevel"))
                    If IsNothing(dtDailyAccruals) Then Exit For

                    ' Asigna valores
                    Dim strAccrualKey As String = ""
                    Dim PreviousDate As Date = #12:00:00 AM#

                    ' Exporta varios saldos por línea
                    Dim cRow As DataRow = Nothing
                    ' sin fecha
                    ExportConceptsByLine(dtDailyAccruals, dtAllConcepts, Row, cRow, )
                Next

                If Not IsNothing(dtEmpUsrFields) Then dtEmpUsrFields.Dispose()

                bolRet = True

                'Se deja un mensaje informativo dentro del archivo ascii
                If Profile.MemoryStreamWriter IsNot Nothing AndAlso Profile.MemoryStreamWriter.Length = 0 Then
                    Dim textErrror As Byte() = System.Text.Encoding.Unicode.GetBytes(Me.oState.Language.Translate("roDataLinkExport.ExportProfile.NoInfo", ""))
                    Profile.MemoryStreamWriter.Write(textErrror, 0, textErrror.Length)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:ExportProfileOnlyDepartments")
            Finally
                ' Cierra el fichero
                If bolCloseFile Then Me.Profile.FileClose()
            End Try

            Return bolRet

        End Function

        Private Function CreateUserFieldsBreaks(ByRef dtEmployees As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Crea el dataadapter de la ficha para roturar
                Dim bd As Date
                Dim ed As Date
                Dim daBreaking As DbDataAdapter = CreateDataAdapter_EmployeeUserBreaking()
                Dim dt As DataTable = Nothing
                Dim newRow() As DataRow = Nothing
                Dim n As Integer = 0

                ' Para cada registro
                For Each row As DataRow In dtEmployees.Rows
                    ' Determina la fecha inicial y final
                    bd = IIf(Me.mBeginDate > row("BeginDate"), Me.mBeginDate, row("BeginDate"))
                    ed = IIf(Me.mEndDate < row("EndDate"), Me.mEndDate, row("EndDate"))

                    ' Lee todos los campos de la ficha que han cambiado entre fechas
                    With daBreaking.SelectCommand
                        .Parameters("@idEmployee").Value = row("idEmployee")
                        .Parameters("@BeginDate").Value = bd
                        .Parameters("@EndDate").Value = ed
                    End With

                    dt = New DataTable
                    daBreaking.Fill(dt)

                    ' Procesa los registros
                    If dt.Rows.Count > 0 Then
                        If dt.Rows(0)("Date") <> row("BeginDate") Then
                            If IsNothing(newRow) Then
                                ReDim Preserve newRow(0)
                            Else
                                ReDim Preserve newRow(newRow.Length)
                            End If

                            ' Primer registro
                            row("EndDate") = CDate(dt.Rows(0)("Date")).AddDays(-1)
                            n = newRow.Length - 1

                            ' registros intermedios
                            For i As Integer = 0 To dt.Rows.Count - 2
                                If dt.Rows(i)("Date") <> dt.Rows(i + 1)("Date") Then
                                    newRow(n) = dtEmployees.NewRow
                                    newRow(n).ItemArray = row.ItemArray
                                    newRow(n)("idEmployee") = row("idEmployee")
                                    newRow(n)("idContract") = row("idContract")
                                    newRow(n)("BeginDate") = dt.Rows(i)("Date")
                                    newRow(n)("EndDate") = CDate(dt.Rows(i + 1)("Date")).AddDays(-1)

                                    ReDim Preserve newRow(newRow.Length)
                                    n = newRow.Length - 1
                                End If
                            Next

                            ' Último registro
                            If DateDiff(DateInterval.Day, dt(dt.Rows.Count - 1)("Date"), ed) <> DateDiff(DateInterval.Day, Me.mBeginDate, Me.mEndDate) Then
                                newRow(n) = dtEmployees.NewRow
                                newRow(n).ItemArray = row.ItemArray
                                newRow(n)("idEmployee") = row("idEmployee")
                                newRow(n)("idContract") = row("idContract")
                                newRow(n)("BeginDate") = dt(dt.Rows.Count - 1)("Date")
                                newRow(n)("EndDate") = ed
                            End If
                        End If
                    End If
                Next

                ' Añade todos los registros nuevos a la tabla
                If Not IsNothing(newRow) Then
                    For n = 0 To newRow.Length - 1
                        dtEmployees.Rows.Add(newRow(n))
                    Next
                End If

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::CreateBreaks:CreateBreaks")
            End Try

            Return bolRet

        End Function

        Private Function CreateBreaksContract(ByRef dtEmployees As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                ' Crea el dataadapter de la ficha para roturar
                Dim daBreaking As DbDataAdapter = CreateDataAdapter_EmployeeContractBreaking()
                Dim dtContracts As DataTable = Nothing
                Dim newRow() As DataRow = Nothing
                Dim n As Integer = 0

                ' Para cada registro
                For Each rowEmployee As DataRow In dtEmployees.Rows
                    'Consulto los contratos del empleado en el periodo de selección
                    With daBreaking.SelectCommand
                        .Parameters("@idEmployee").Value = rowEmployee("IDEmployee")
                    End With
                    dtContracts = New DataTable
                    daBreaking.Fill(dtContracts)
                Next

                ' Añade todos los registros nuevos a la tabla
                If Not IsNothing(newRow) Then
                    For n = 0 To newRow.Length - 1
                        dtEmployees.Rows.Add(newRow(n))
                    Next
                End If

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::CreateBreaks:CreateBreaks")
            End Try

            Return bolRet

        End Function

        Private Function ExportOneConceptByLine(ByVal dtRowsToExport As DataTable, ByVal eRow As DataRow, ByVal cRow As DataRow, ByVal dtAllConcepts As DataTable, Optional idEmployee As String = "", Optional ByVal ExistDiningData As Boolean = False)
            Dim bolRet As Boolean = False

            Try
                Dim strAccrualKey As String = ""
                Dim i As Integer = 0

                If ilastIDEmployee <> idEmployee Then
                    iAccrualValueOrder = 1
                    ilastIDEmployee = idEmployee
                Else
                    iAccrualValueOrder = iAccrualValueOrder + 1
                End If
                iTotalLines = iTotalLines + 1

                ' Exporta un saldo por línea
                For i = 0 To Me.Profile.Fields.Count - 1
                    If (Profile.Fields(i).Source.ToUpper.Length < 6) Then Continue For
                    If Profile.Fields(i).Source.ToUpper.Substring(0, 6) = "SALDO_" Then
                        strAccrualKey = Profile.Fields(i).Source.Split("_")(1)
                        Select Case strAccrualKey.ToUpper
                            Case "ORDEN" : Profile.Fields(i).Value = iAccrualValueOrder
                            Case "V"
                                If Me.EspGuideName.Contains("SAP ADP ICA Group") AndAlso GetConceptField(cRow("ShortName"), "idType", dtAllConcepts) = "H" Then
                                    ' Grupo ICA exporta saldos de tiempo en minutos
                                    Profile.Fields(i).Value = cRow("TotalConcept") * 60
                                Else
                                    Profile.Fields(i).Value = cRow("TotalConcept")
                                End If
                            Case "NC" : Profile.Fields(i).Value = cRow("ShortName")
                            Case "PERIODO" : Profile.Fields(i).Value = oState.Language.TranslateWithDefault("roDataLinkExport.ProfileExportConcepts.Period." & cRow("DefaultQuery"), "", cRow("DefaultQuery"))
                            Case "INICIOPERIODO" : Profile.Fields(i).Value = cRow("BeginPeriod")
                            Case "FINPERIODO" : Profile.Fields(i).Value = cRow("EndPeriod")
                            Case "NOMBRE" : Profile.Fields(i).Value = cRow("ConceptName")
                            Case "INICIOCOMPUTO"
                                ' Sólo tienen sentido en exportaciones de FINIQUITO
                                Try
                                    Profile.Fields(i).Value = cRow("BeginComputedPeriod")
                                Catch ex As Exception
                                    Profile.Fields(i).Value = String.Empty
                                End Try
                            Case "FINALCOMPUTO"
                                ' Sólo tienen sentido en exportaciones de FINIQUITO
                                Try
                                    Profile.Fields(i).Value = cRow("EndComputedPeriod")
                                Catch ex As Exception
                                    Profile.Fields(i).Value = String.Empty
                                End Try
                            Case "EXPORTARCOMO"
                                If ExistDiningData AndAlso Not IsDBNull(cRow("IsDining")) AndAlso cRow("IsDining") Then
                                    ' Saldo de comedor
                                    Profile.Fields(i).Value = cRow("ShortName")
                                Else
                                    ' Saldo normal
                                    Profile.Fields(i).Value = GetConceptField(cRow("ShortName"), "Export", dtAllConcepts)
                                End If

                            Case "FECHA"
                                Profile.Fields(i).Value = cRow("Date")
                            Case "ICAADPDATE"
                                If Me.EspGuideName.Contains("SAP ADP ICA Group") Then
                                    Dim sConceptName As String = GetConceptField(cRow("ShortName"), "Name", dtAllConcepts)
                                    If sConceptName.Contains("C") Then
                                        ' Concepto de contabilidad. Último día del periodo actual
                                        Profile.Fields(i).Value = mEndDate
                                    ElseIf sConceptName.Contains("P") Then
                                        ' Concepto de nómina. Primer día del siguiente periodo.
                                        Profile.Fields(i).Value = mEndDate.AddDays(1)
                                    Else
                                        Profile.Fields(i).Value = Me.mBeginDate
                                    End If
                                End If
                            Case "TIPO"
                                Profile.Fields(i).Value = GetConceptField(cRow("ShortName"), "idType", dtAllConcepts)
                                Profile.Fields(i).GetValueFromList = True
                                If Me.EspGuideName.Contains("SAP ADP ICA Group") Then
                                    If Profile.Fields(i).Value = "H" Then
                                        Profile.Fields(i).Value = "002"
                                    Else
                                        Profile.Fields(i).Value = "010"
                                    End If
                                End If
                            Case "DISPONIBILIDAD"
                                Profile.Fields(i).Value = cRow("StartEnjoymentDate")
                            Case "CADUCIDAD"
                                Profile.Fields(i).Value = cRow("ExpiredDate")
                            Case Else
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "SALDO_RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(10, Profile.Fields(i).Source.Length - 10)
                                    'Profile.Fields(i).Value = "RBS not supported"
                                    'Profile.Fields(i).Value = DataLinkDynamicCode.GetAccrualCellValueFromExternalCodeByLine(scriptFileName, Profile.Fields, dtRowsToExport, eRow, cRow, dtAllConcepts, Me.mBeginDate, Me.mEndDate, oCn)
                                    Profile.Fields(i).Value = AccrualCellValueByLine.GetValue(scriptFileName, Profile.Fields, dtRowsToExport, eRow, cRow, dtAllConcepts, Me.mBeginDate, Me.mEndDate)
                                End If
                        End Select
                    End If
                    If InStr(1, Profile.Fields(i).Source.ToUpper, "RBSB_") Then ' Robotics script base
                        Dim scriptFileName As String = Profile.Fields(i).Source.Substring(5, Profile.Fields(i).Source.Length - 5)
                        Profile.Fields(i).Value = "RBS not supported"
                    End If
                Next i

                ' Graba la línea
                Me.Profile.CreateLine()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:ExportOneConceptByLine")
            End Try

            Return bolRet
        End Function

        Private Function ExportConceptsByLine(ByVal dtDailyAccruals As DataTable, ByVal dtAllConcepts As DataTable, ByVal eRow As DataRow, ByVal cRow As DataRow, Optional DateAccrual As Date = #12:00:00 AM#, Optional idEmployee As String = "", Optional dtEmpPunches As DataTable = Nothing, Optional dtEmpSchedule As DataTable = Nothing, Optional dtEmpUserFldValuesHistory As DataTable = Nothing, Optional dtMobilities As DataTable = Nothing, Optional dicVacationShifts As Dictionary(Of String, Integer) = Nothing, Optional dicVacationCauses As Dictionary(Of String, Integer) = Nothing, Optional dicVacationShiftsType As Dictionary(Of String, String) = Nothing)
            Dim bolRet As Boolean = False

            Try
                Dim strAccrualKey As String = ""
                Dim i As Integer = 0
                Dim iPunchPositionOnDay As Integer = 0
                Dim oScheduleDataView As System.Data.DataView = Nothing

                If dtEmpSchedule IsNot Nothing AndAlso dtEmpSchedule.Rows.Count > 0 Then
                    oScheduleDataView = New System.Data.DataView(dtEmpSchedule)
                    oScheduleDataView.Sort = "Date ASC"
                End If

                For i = 0 To Me.Profile.Fields.Count - 1
                    If Profile.Fields(i).Source.Length >= 6 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 6) = "SALDO_" Then
                        strAccrualKey = Profile.Fields(i).Source.Split("_")(1)
                        Select Case strAccrualKey.ToUpper
                            Case "V" : Profile.Fields(i).Value = GetConcept_TotalValue(Profile.Fields(i).ShortName, dtDailyAccruals, DateAccrual)
                            Case "INICIOPERIODO" : Profile.Fields(i).Value = GetConcept_PeriodDates(Profile.Fields(i).ShortName, dtDailyAccruals, DateAccrual, "begin")
                            Case "FINPERIODO" : Profile.Fields(i).Value = GetConcept_PeriodDates(Profile.Fields(i).ShortName, dtDailyAccruals, DateAccrual, "end")
                            Case "FECHA" : Profile.Fields(i).Value = DateAccrual
                            Case "NC" : Profile.Fields(i).Value = Profile.Fields(i).ShortName
                            Case "EXPORTARCOMO" : Profile.Fields(i).Value = GetConceptField(Profile.Fields(i).ShortName, "Export", dtAllConcepts)
                            Case "TIPO"
                                Profile.Fields(i).Value = GetConceptField(Profile.Fields(i).ShortName, "idType", dtAllConcepts)
                                Profile.Fields(i).GetValueFromList = True
                            Case "DISPONIBILIDAD" : Profile.Fields(i).Value = GetConcept_StartEnjoymentDate(Profile.Fields(i).ShortName, dtDailyAccruals, DateAccrual)
                            Case "CADUCIDAD" : Profile.Fields(i).Value = GetConcept_ExpiredDate(Profile.Fields(i).ShortName, dtDailyAccruals, DateAccrual)
                            Case Else
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "SALDO_RBS_") Then ' Robotics script
                                    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(10, Profile.Fields(i).Source.Length - 10)
                                    'Profile.Fields(i).Value = "RBS not supported"
                                    'Profile.Fields(i).Value = DataLinkDynamicCode.GetAccrualCellValueFromExternalCodeByLine(scriptFileName, Profile.Fields, dtRowsToExport, eRow, cRow, dtAllConcepts, Me.mBeginDate, Me.mEndDate, oCn)
                                    Profile.Fields(i).Value = AccrualCellValueByLine.GetValue(scriptFileName, Profile.Fields, dtDailyAccruals, eRow, cRow, dtAllConcepts, Me.mBeginDate, Me.mEndDate)
                                End If

                        End Select
                    ElseIf Profile.Fields(i).Source.Length >= 10 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 10) = "USR_FECHA_" Then
                        If (String.IsNullOrEmpty(idEmployee)) Then Continue For
                        If DateAccrual = #12:00:00 AM# Then Continue For
                        'If IsNothing(cRow("Date")) Then Continue For
                        Dim dtEmpUsrFields = DataLinkDynamicCode.CreateDataTable_EmployeeUserFields(idEmployee, DateAccrual, Me.oState)
                        If dtEmpUsrFields.Rows.Count > 0 Then
                            Dim FieldName As String = Profile.Fields(i).Source.Substring(10, Profile.Fields(i).Source.Length - 10)
                            Dim r As DataRow() = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                            If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then
                                Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                If (Profile.Fields(i + 1).Source.ToString().ToUpper().Contains("LITERAL_FECHA")) Then
                                    Profile.Fields(i + 1).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Date"), r(0)("FieldType"))
                                End If

                            End If
                            If (Profile.Fields(i + 1).Value.ToString().ToUpper().Equals("FECHA")) Then Profile.Fields(i + 1).Value = String.Empty
                        End If
                    ElseIf Profile.Fields(i).Source.Length >= 9 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 8) = "FICHAJE_" Then
                        Dim dPunch As DateTime = Nothing
                        Dim intIDTerminal As Integer = 0
                        Dim strTerminalDescription As String = ""

                        Profile.Fields(i).Value = ""
                        If dtEmpPunches IsNot Nothing AndAlso dtEmpPunches.Rows.Count > 0 Then
                            If Profile.Fields(i).Source.ToUpper.Substring(8, 2).ToUpper = "IN" Then
                                iPunchPositionOnDay = Profile.Fields(i).Source.Substring(10, Profile.Fields(i).Source.Length - 10)
                                dPunch = GetNthPunch(iPunchPositionOnDay, dtEmpPunches, "IN", DateAccrual, intIDTerminal)
                                If dPunch <> Nothing Then Profile.Fields(i).Value = dPunch
                            End If
                            If Profile.Fields(i).Source.ToUpper.Substring(8, 3).ToUpper = "OUT" Then
                                iPunchPositionOnDay = Profile.Fields(i).Source.Substring(11, Profile.Fields(i).Source.Length - 11)
                                dPunch = GetNthPunch(iPunchPositionOnDay, dtEmpPunches, "OUT", DateAccrual, intIDTerminal)
                                If dPunch <> Nothing Then Profile.Fields(i).Value = dPunch
                            End If

                            If Profile.Fields(i).Source.Length > 22 Then
                                If Profile.Fields(i).Source.ToUpper.Substring(8, 11).ToUpper = "TERMINAL_ID" Then
                                    If Profile.Fields(i).Source.ToUpper.Substring(20, 2).ToUpper = "IN" Then
                                        iPunchPositionOnDay = Profile.Fields(i).Source.Substring(22, Profile.Fields(i).Source.Length - 22)
                                        dPunch = GetNthPunch(iPunchPositionOnDay, dtEmpPunches, "IN", DateAccrual, intIDTerminal)
                                        If dPunch <> Nothing Then Profile.Fields(i).Value = intIDTerminal
                                    End If
                                    If Profile.Fields(i).Source.ToUpper.Substring(20, 3).ToUpper = "OUT" Then
                                        iPunchPositionOnDay = Profile.Fields(i).Source.Substring(23, Profile.Fields(i).Source.Length - 23)
                                        dPunch = GetNthPunch(iPunchPositionOnDay, dtEmpPunches, "OUT", DateAccrual, intIDTerminal)
                                        If dPunch <> Nothing Then Profile.Fields(i).Value = intIDTerminal
                                    End If
                                End If
                            End If

                            If Profile.Fields(i).Source.Length > 31 Then
                                If Profile.Fields(i).Source.ToUpper.Substring(8, 20).ToUpper = "TERMINAL_DESCRIPCION" Or Profile.Fields(i).Source.ToUpper.Substring(8, 20).ToUpper = "TERMINAL_DESCRIPCIÓN" Then
                                    If Profile.Fields(i).Source.ToUpper.Substring(29, 2).ToUpper = "IN" Then
                                        iPunchPositionOnDay = Profile.Fields(i).Source.Substring(31, Profile.Fields(i).Source.Length - 31)
                                        dPunch = GetNthPunch(iPunchPositionOnDay, dtEmpPunches, "IN", DateAccrual, intIDTerminal)
                                        If dPunch <> Nothing Then
                                            strTerminalDescription = roTypes.Any2String(ExecuteScalar("@SELECT# isnull(Description,'') FROM Terminals WHERE ID=" & intIDTerminal.ToString))
                                            Profile.Fields(i).Value = strTerminalDescription
                                        End If
                                    End If
                                    If Profile.Fields(i).Source.ToUpper.Substring(29, 3).ToUpper = "OUT" Then
                                        iPunchPositionOnDay = Profile.Fields(i).Source.Substring(32, Profile.Fields(i).Source.Length - 32)
                                        dPunch = GetNthPunch(iPunchPositionOnDay, dtEmpPunches, "OUT", DateAccrual, intIDTerminal)
                                        If dPunch <> Nothing Then
                                            strTerminalDescription = roTypes.Any2String(ExecuteScalar("@SELECT# isnull(Description,'') FROM Terminals WHERE ID=" & intIDTerminal.ToString))
                                            Profile.Fields(i).Value = strTerminalDescription
                                        End If
                                    End If
                                End If
                            End If

                        End If
                    ElseIf Profile.Fields(i).Source.Length >= 9 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 8) = "HORARIO_" Then
                        Profile.Fields(i).Value = ""
                        If oScheduleDataView IsNot Nothing AndAlso oScheduleDataView.Table IsNot Nothing AndAlso oScheduleDataView.ToTable.Rows.Count > 0 Then
                            Dim oScheduleRow As DataRow
                            oScheduleDataView.RowFilter = "Date = '" & Format(DateAccrual, "yyyy/MM/dd") & "'"
                            If oScheduleDataView.ToTable.Rows.Count > 0 Then
                                oScheduleRow = oScheduleDataView.ToTable.Rows(0)
                                ' Si el día está calculado, me quedo con ShiftUsed. Si no, con el Shift1
                                If Not IsDBNull(oScheduleRow("IDShiftUsed")) OrElse Not IsDBNull(oScheduleRow("IDShift1")) Then
                                    If Not IsDBNull(oScheduleRow("IDShiftUsed")) Then
                                        Select Case Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8).ToUpper
                                            Case "NOMBRE"
                                                Profile.Fields(i).Value = oScheduleRow("UsedName")
                                            Case "NC"
                                                Profile.Fields(i).Value = oScheduleRow("UsedShortName")
                                            Case "TEO"
                                                Profile.Fields(i).Value = oScheduleRow("ExpectedWorkingHoursUsedShift")
                                            Case "EQUIV"
                                                Profile.Fields(i).Value = oScheduleRow("ExportUsed")
                                        End Select
                                    Else
                                        Select Case Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8).ToUpper
                                            Case "NOMBRE"
                                                Profile.Fields(i).Value = oScheduleRow("Name1")
                                            Case "NC"
                                                Profile.Fields(i).Value = oScheduleRow("ShortName1")
                                            Case "TEO"
                                                Profile.Fields(i).Value = oScheduleRow("ExpectedWorkingHours1")
                                            Case "EQUIV"
                                                Profile.Fields(i).Value = oScheduleRow("Export1")
                                        End Select
                                    End If
                                End If
                                ' EuropeAssistance
                                If Profile.Fields(i).Source.Length >= 11 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 11) = "HORARIO_EA_" Then
                                    If Not IsDBNull(oScheduleRow("AdvancedParametersUsed")) OrElse Not IsDBNull(oScheduleRow("AdvancedParameters1")) Then
                                        Dim sPart As String = String.Empty
                                        sPart = Profile.Fields(i).Source.Substring(11, Profile.Fields(i).Source.Length - 11)
                                        If Not IsDBNull(oScheduleRow("AdvancedParametersUsed")) AndAlso oScheduleRow("AdvancedParametersUsed") <> "" Then
                                            Profile.Fields(i).Value = EuropeAssistance_Calendar(roTypes.Any2String(oScheduleRow("AdvancedParametersUsed")), sPart)
                                        Else
                                            Profile.Fields(i).Value = EuropeAssistance_Calendar(roTypes.Any2String(oScheduleRow("AdvancedParameters1")), sPart)
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    ElseIf Profile.Fields(i).Source.Length >= 4 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 4) = "USR_" AndAlso mExportConceptsWithDate AndAlso (dtEmpUserFldValuesHistory IsNot Nothing AndAlso dtEmpUserFldValuesHistory.Rows.Count) Then
                        'Se incluye campo de la ficha y la exportación de saldos incluye fecha. El valor del campo de la ficha debe ser con la fecha detalle, por lo que sobreescribo el calculado inicialmente.
                        Dim sUsrFldName As String = String.Empty
                        sUsrFldName = Me.Profile.Fields(i).Source.Substring(4, Me.Profile.Fields(i).Source.Length - 4)
                        Dim oUserFieldValueOnDateRow As DataRow
                        Dim FieldType As FieldTypes = FieldTypes.tText
                        oUserFieldValueOnDateRow = GetUserFieldValueOnDate(cRow("Date"), cRow("Date"), sUsrFldName, dtEmpUserFldValuesHistory, FieldType)
                        If Not oUserFieldValueOnDateRow Is Nothing AndAlso Not IsDBNull(oUserFieldValueOnDateRow("Value")) Then
                            Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(oUserFieldValueOnDateRow("Value"), oUserFieldValueOnDateRow("FieldType"))
                        Else
                            Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue("", FieldType)
                        End If
                    ElseIf Profile.Fields(i).Source.ToUpper = "GRUPO" AndAlso mExportConceptsWithDate AndAlso (dtMobilities IsNot Nothing AndAlso dtMobilities.Rows.Count) Then
                        'Se incluye nombre de grupo y la exportación de saldos incluye fecha. El valor del grupo debe ser con la fecha detalle, por lo que sobreescribo el calculado inicialmente.
                        Dim oMobilityRow As DataRow
                        oMobilityRow = GetMobilityOnDate(cRow("Date"), cRow("Date"), dtMobilities)
                        Profile.Fields(i).Value = oMobilityRow("Name")
                    ElseIf Profile.Fields(i).Source.Length >= 4 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 4) = "VAC_" Then
                        ' Saldos de vacaciones de un horario dado
                        If (String.IsNullOrEmpty(idEmployee)) Then Continue For
                        Dim iVacShift As Integer = 0
                        Dim sVacShiftType As String = ""

                        ' Obtenemos información resumen días vacaciones
                        Dim iDisponible As Double = 0
                        Dim _EmpState As Employee.roEmployeeState = New Employee.roEmployeeState
                        Dim VacationsResumeValue() As Double = {0, 0, 0, 0, 0, 0}
                        Dim dRefDate As Date = Now
                        If idEmployee > 0 Then
                            If Not dicVacationShifts Is Nothing Then
                                iVacShift = dicVacationShifts(Profile.Fields(i).Source.ToUpper.Split("_")(1))

                                ' Miro si calculo a fecha de hoy o a fecha fin de exportación
                                If Profile.Fields(i).Source.ToUpper.Split("_").Count > 3 AndAlso Profile.Fields(i).Source.ToUpper.Split("_")(3).ToUpper = "FF" Then
                                    dRefDate = Me.mEndDate
                                End If

                                'Miro si el saldo asociado al horario es de tipo contrato anualizado
                                If dicVacationShiftsType IsNot Nothing Then
                                    sVacShiftType = dicVacationShiftsType(Profile.Fields(i).Source.ToUpper.Split("_")(1))
                                    If sVacShiftType <> "L" Then
                                        roBusinessSupport.VacationsResumeQuery(idEmployee, iVacShift, dRefDate, Nothing, Nothing, dRefDate, VacationsResumeValue(0), VacationsResumeValue(1), VacationsResumeValue(2), iDisponible, _EmpState, VacationsResumeValue(4), VacationsResumeValue(5))
                                        VacationsResumeValue(3) = iDisponible - VacationsResumeValue(2) - VacationsResumeValue(1)
                                    Else
                                        VacationsResumeValue(0) = 0
                                        VacationsResumeValue(1) = 0
                                        VacationsResumeValue(2) = 0
                                        VacationsResumeValue(3) = 0
                                    End If
                                End If

                                If Profile.Fields(i).Source.ToUpper.Split("_").Count > 2 Then
                                    Select Case Profile.Fields(i).Source.ToUpper.Split("_")(2)
                                        Case "YR" 'Ya realizadas
                                            Profile.Fields(i).Value = VacationsResumeValue(0)
                                        Case "SP" 'Pendientes de aprobar
                                            Profile.Fields(i).Value = VacationsResumeValue(1)
                                        Case "AP" 'Aprobadas pendients de disfrutar
                                            Profile.Fields(i).Value = VacationsResumeValue(2)
                                        Case "DS" 'Total por realizar: Total - ya realizadas
                                            Profile.Fields(i).Value = iDisponible
                                        Case "PR" 'Total por solicitar - Solicitadas
                                            Profile.Fields(i).Value = VacationsResumeValue(3)
                                    End Select
                                End If
                            End If
                        End If
                    ElseIf Profile.Fields(i).Source.Length >= 5 AndAlso Profile.Fields(i).Source.ToUpper.Substring(0, 5) = "VACH_" Then
                        ' Saldos de vacaciones por horas de una justificación dada
                        If (String.IsNullOrEmpty(idEmployee)) Then Continue For
                        Dim iVacCause As Integer = 0

                        ' Obtenemos información resumen horas vacaciones
                        Dim iDisponible As Double = 0
                        Dim _EmpState As Employee.roEmployeeState = New Employee.roEmployeeState
                        Dim VacationsResumeValue() As Double = {0, 0, 0, 0}
                        Dim dRefDate As Date = Now
                        If idEmployee > 0 Then
                            If Not dicVacationShifts Is Nothing Then
                                iVacCause = dicVacationCauses(Profile.Fields(i).Source.ToUpper.Split("_")(1))

                                ' Miro si calculo a fecha de hoy o a fecha fin de exportación
                                If Profile.Fields(i).Source.ToUpper.Split("_").Count > 3 AndAlso Profile.Fields(i).Source.ToUpper.Split("_")(3).ToUpper = "FF" Then
                                    dRefDate = Me.mEndDate
                                End If

                                roBusinessSupport.ProgrammedHolidaysResumeQuery(idEmployee, iVacCause, dRefDate, Nothing, Nothing, dRefDate, VacationsResumeValue(1), VacationsResumeValue(2), iDisponible, _EmpState)
                                VacationsResumeValue(0) = 0 'En vacaciones por horas, las ya realizadas valen 0 (por alguna razón ...)
                                VacationsResumeValue(3) = iDisponible - VacationsResumeValue(2) - VacationsResumeValue(1)

                                If Profile.Fields(i).Source.ToUpper.Split("_").Count > 2 Then
                                    Select Case Profile.Fields(i).Source.ToUpper.Split("_")(2)
                                        Case "YR" 'Ya realizadas
                                            Profile.Fields(i).Value = VacationsResumeValue(0)
                                        Case "SP" 'Pendientes de aprobar
                                            Profile.Fields(i).Value = VacationsResumeValue(1)
                                        Case "AP" 'Aprobadas pendients de disfrutar
                                            Profile.Fields(i).Value = VacationsResumeValue(2)
                                        Case "DS" 'Total por realizar: Total - ya realizadas
                                            Profile.Fields(i).Value = iDisponible
                                        Case "PR" 'Total por solicitar - Solicitadas
                                            Profile.Fields(i).Value = VacationsResumeValue(3)
                                    End Select
                                End If
                            End If
                        End If
                    ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBSB_") Then ' Robotics script base
                        Dim scriptFileName As String = Profile.Fields(i).Source.Substring(5, Profile.Fields(i).Source.Length - 5)
                        Profile.Fields(i).Value = "RBS not supported"
                    End If
                Next i

                ' Graba la línea
                Me.Profile.CreateLine()

                bolRet = True
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:ExportConceptsByLine")
            End Try

            Return bolRet
        End Function

        Private Function GetNthPunch(n As Integer, dtPunches As DataTable, str As String, dShiftDate As Date, ByRef intIDTerminal As Integer) As DateTime
            Dim oRet As DateTime = Nothing
            Dim oPunchRow As DataRow
            Dim iIN As Integer = 0
            Dim iOUT As Integer = 0

            Dim oDataView As System.Data.DataView = New System.Data.DataView(dtPunches)
            oDataView.RowFilter = "ShiftDate = '" & Format(dShiftDate, "yyyy/MM/dd") & "'"
            oDataView.Sort = "ShiftDate ASC, DateTime ASC"

            intIDTerminal = 0

            Try
                For Each oPunchRow In oDataView.ToTable.Rows
                    If oPunchRow("ActualType") = 1 Then
                        iIN = iIN + 1
                        If iIN = n AndAlso str = "IN" AndAlso dShiftDate = roTypes.Any2DateTime(oPunchRow("ShiftDate")) Then
                            intIDTerminal = roTypes.Any2Integer(oPunchRow("IDTerminal"))
                            Return oPunchRow("DateTime")
                        End If
                    End If
                    If oPunchRow("ActualType") = 2 Then
                        iOUT = iOUT + 1
                        If iOUT = n AndAlso str = "OUT" AndAlso dShiftDate = roTypes.Any2DateTime(oPunchRow("ShiftDate")) Then
                            intIDTerminal = roTypes.Any2Integer(oPunchRow("IDTerminal"))
                            Return oPunchRow("DateTime")
                        End If
                    End If
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConcept_TotalValue")
            End Try
            Return oRet
        End Function

        Private Function GetMobilityOnDate(dStartDate As DateTime, dEndDate As DateTime, dtGroups As DataTable) As DataRow
            Dim oRet As DataRow = Nothing
            Try
                For Each oMobilityRow As DataRow In dtGroups.Rows
                    If oMobilityRow("BeginDate") <= dStartDate AndAlso oMobilityRow("EndDate") >= dEndDate Then
                        oRet = oMobilityRow
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetMobilityOnDate")
            End Try
            Return oRet
        End Function

        Private Function GetConcept_TotalValue(ByVal ShortName As String, ByVal dt As DataTable, Optional DateAccrual As Date = #12:00:00 AM#) As Double
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
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConcept_TotalValue")
            End Try

            Return t
        End Function

        Private Function GetConcept_ExpiredDate(ByVal ShortName As String, ByVal dt As DataTable, Optional DateAccrual As Date = #12:00:00 AM#) As Date

            Try
                Dim rows() As DataRow

                If DateAccrual = #12:00:00 AM# Then
                    Return Nothing
                Else
                    rows = dt.Select("ShortName='" & ShortName & "' and Date=#" & DateAccrual.Month & "/" & DateAccrual.Day & "/" & DateAccrual.Year & "#")
                    If rows IsNot Nothing AndAlso rows.Length > 0 Then
                        Return roTypes.Any2DateTime(rows(0)("ExpiredDate"))
                    Else
                        Return Nothing
                    End If
                End If

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConcept_ExpiredDate")
                Return Nothing
            End Try

        End Function

        Private Function GetConcept_StartEnjoymentDate(ByVal ShortName As String, ByVal dt As DataTable, Optional DateAccrual As Date = #12:00:00 AM#) As Date

            Try
                Dim rows() As DataRow

                If DateAccrual = #12:00:00 AM# Then
                    Return Nothing
                Else
                    rows = dt.Select("ShortName='" & ShortName & "' and Date=#" & DateAccrual.Month & "/" & DateAccrual.Day & "/" & DateAccrual.Year & "#")
                    If rows IsNot Nothing AndAlso rows.Length > 0 Then
                        Return roTypes.Any2DateTime(rows(0)("StartEnjoymentDate"))
                    Else
                        Return Nothing
                    End If
                End If

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConcept_StartEnjoymentDate")
                Return Nothing
            End Try

        End Function

        Private Function GetConceptField(ByVal ShortName As String, ByVal Field As String, ByVal dt As DataTable) As String
            Dim ConceptField As String = ""

            Try
                Dim rows As DataRow() = dt.Select("ShortName='" & ShortName & "'")
                If rows.Length > 0 Then ConceptField = roTypes.Any2String(rows(0)(Field))
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConceptField")
            End Try

            Return ConceptField
        End Function

        Private Function GetConcept_PeriodDates(ByVal ShortName As String, ByVal dt As DataTable, dDate As Date, sPos As String) As DateTime
            Dim dRet As DateTime = Nothing
            Try
                Dim rows As DataRow()

                If dDate = #12:00:00 AM# Then
                    rows = dt.Select("ShortName='" & ShortName & "'")
                Else
                    rows = dt.Select("ShortName='" & ShortName & "' and Date=#" & dDate.Month & "/" & dDate.Day & "/" & dDate.Year & "#")
                End If

                If rows.Length > 0 Then
                    Select Case sPos.ToLower
                        Case "begin"
                            dRet = roTypes.Any2DateTime(rows(0)("BeginPeriod"))
                        Case "end"
                            dRet = roTypes.Any2DateTime(rows(0)("EndPeriod"))
                    End Select
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:GetConcept_BeginPeriod")
            End Try

            Return dRet
        End Function

        Private Function CreateDataAdapter_EmployeeUserBreaking() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strBrk() As String = Me.mBreakingBy.Split(",")
                Dim str As String = ""

                For i As Integer = 0 To strBrk.Length - 1
                    If i > 0 Then str += ","

                    str += "'" & strBrk(i) & "'"
                Next

                Dim strSQL As String = "@SELECT# idEmployee, FieldName, Date, Value from EmployeeUserFieldValues " &
                "Where idEmployee=@idEmployee and (Date > @BeginDate and date<=@EndDate) and FieldName in (" & str & ") " &
                "Order by Date "
                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:CreateDataAdapter_EmployeeUserBreaking")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_EmployeeContractBreaking() As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = "@DECLARE# @beginDate datetime, @endDate datetime " &
                                    " set @beginDate = " & Any2Time(Me.mBeginDate).SQLDateTime & " " &
                                    " set @endDate = " & Any2Time(Me.mEndDate).SQLDateTime & " "

                strSQL += " ;WITH alldays AS (
										   SELECT @beginDate AS contractDate
										   UNION ALL
										   SELECT DATEADD(dd, 1, contractDate)
												FROM alldays s
											WHERE DATEADD(dd, 1, contractDate) <= @endDate)
							select distinct  ec.IDEmployee, ec.IDContract, ec.BeginDate, ec.EndDate from EmployeeContracts ec
							inner join alldays on alldays.contractDate between BeginDate and EndDate
							where IDEmployee = @idEmployee order by BeginDate
							OPTION (maxrecursion 0)"

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:CreateDataAdapter_EmployeeContractBreaking")
            End Try

            Return da
        End Function

        Private Function LoadInfo(ByVal dtRowsToExport As DataTable, ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable, ByVal dtGroups As DataTable, Optional bHide2079 As Boolean = False, Optional EndContract As Nullable(Of Date) = Nothing) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim bExportOneConceptByLine As Boolean = False
                Dim strAccrualKey As String = ""
                Dim i As Integer = 0

                Dim dt As New DataTable

                Dim dtGroupsOnDate As New DataTable
                ' Recupero información de departamento en la fecha del periodo
                dtGroupsOnDate = DataLinkDynamicCode.CreateDataTable_GroupsOnDate(row("idEmployee"), IIf(Me.mEndDate < row("EndDate"), Me.mEndDate, row("EndDate")), oState)

                For i = 0 To Me.Profile.Fields.Count - 1
                    Profile.Fields(i).Value = ""

                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Profile.Fields(i).Value = mBeginDate
                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Profile.Fields(i).Value = mEndDate
                        Case "FECHA_INICIO_COMPUTO", "FECHA_INICIO_CÓMPUTO"
                            Profile.Fields(i).Value = IIf(mBeginDate >= row("BeginDate"), mBeginDate, row("BeginDate"))
                        Case "FECHA_FINAL_COMPUTO", "FECHA_FINAL_CÓMPUTO"
                            Profile.Fields(i).Value = IIf(mEndDate <= row("EndDate"), mEndDate, row("EndDate"))
                        Case "GRUPO"
                            Profile.Fields(i).Value = row("GroupName")
                        Case "GRUPO_COMPLETO"
                            Profile.Fields(i).Value = row("FullGroupName")
                        Case "NOMBRE"
                            Profile.Fields(i).Value = row("EmployeeName")
                        Case "CONTRATO"
                            Profile.Fields(i).Value = row("idContract")
                        Case "CONTRATO_MOTIVO_FIN"
                            Profile.Fields(i).Value = roTypes.Any2String(row("EndContractReason"))
                        Case "CONTRATO_SINPERIODO"
                            Profile.Fields(i).Value = row("idContract")
                            Try
                                Profile.Fields(i).Value = row("idContract").ToString.Split(".")(0)
                            Catch ex As Exception
                            End Try
                        Case "CONTRATO_SOLOPERIODO"
                            Profile.Fields(i).Value = ""
                            Try
                                Profile.Fields(i).Value = row("idContract").ToString.Split(".")(1)
                            Catch ex As Exception
                            End Try
                        Case "CONTRATO_FECHA_INICIO"
                            Profile.Fields(i).Value = row("BeginDate")
                        Case "CONTRATO_FECHA_FINAL"
                            If bHide2079 AndAlso Any2DateTime(row("EndDate")).Date = New Date(2079, 1, 1) Then
                                Profile.Fields(i).Value = String.Empty
                            Else
                                Profile.Fields(i).Value = row("EndDate")
                            End If
                        Case "ROTURA_FECHA_INICIO"
                            ' Determina la fecha inicial
                            Profile.Fields(i).Value = IIf(Me.mBeginDate > row("BeginDate"), Me.mBeginDate, row("BeginDate"))
                        Case "ROTURA_FECHA_FINAL"
                            ' Determina la fecha final
                            Profile.Fields(i).Value = IIf(Me.mEndDate < row("EndDate"), Me.mEndDate, row("EndDate"))
                        Case Else
                            ' Determina el tipo de campo
                            If (InStr(1, Profile.Fields(i).Source.ToUpper, "USR_FECHA_")) Then
                                Exit Select
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "USR_") Then
                                ' Lee el dato del campo de la ficha
                                If dtEmpUsrFields.Rows.Count > 0 Then
                                    Dim FieldName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                    Dim r As DataRow() = dtEmpUsrFields.Select("FieldName='" & FieldName & "'")
                                    If r.Length > 0 AndAlso Not IsDBNull(r(0)("Value")) Then Me.Profile.Fields(i).Value = DataLinkDynamicCode.GetEmployeeUserFieldValue(r(0)("Value"), r(0)("FieldType"))
                                End If
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                Profile.Fields(i).Value = Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8)
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "FECHA_REFERENCIA_") OrElse InStr(1, Profile.Fields(i).Source.ToUpper, "FECHA_IMPUTACION_") Then
                                'Fecha de referencia o de imputacion, formada por el día indicado en el TAG, el mes y el año de la fecha de fin de exportación. Si es una exporación de finiquitos, se toma la fecha de fin de contrato
                                Dim iDay As Integer = roTypes.Any2Integer(Profile.Fields(i).Source.Substring(17, 2))
                                Try
                                    Dim dRefDate As DateTime
                                    If Me.ContractTerminations AndAlso EndContract.HasValue Then
                                        dRefDate = New DateTime(EndContract.Value.Year, EndContract.Value.Month, iDay, 0, 0, 0, DateTimeKind.Local)
                                    Else
                                        dRefDate = New DateTime(mEndDate.Year, mEndDate.Month, iDay, 0, 0, 0, DateTimeKind.Local)
                                    End If
                                    Profile.Fields(i).Value = dRefDate
                                Catch ex As Exception
                                    Profile.Fields(i).Value = String.Empty
                                End Try
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "FECHA_PAGO_") Then
                                'Fecha de pago, formada por el día indicado en el TAG, el mes y el año de la fecha de ejecución
                                Dim iDay As Integer = roTypes.Any2Integer(Profile.Fields(i).Source.Substring(11, 2))
                                Try
                                    Dim dRefDate As DateTime
                                    If Me.ContractTerminations AndAlso EndContract.HasValue Then
                                        dRefDate = New DateTime(EndContract.Value.Year, EndContract.Value.Month, iDay, 0, 0, 0, DateTimeKind.Local)
                                    Else
                                        dRefDate = New DateTime(Now.Date.Year, Now.Date.Month, iDay, 0, 0, 0, DateTimeKind.Local)
                                    End If
                                    Profile.Fields(i).Value = dRefDate
                                Catch ex As Exception
                                    Profile.Fields(i).Value = String.Empty
                                End Try
                            ElseIf Me.Profile.Fields(i).Source.ToUpper.StartsWith("NIVEL") Then
                                Dim iLevel As Integer = roTypes.Any2Integer(Me.Profile.Fields(i).Source.Substring(5, Me.Profile.Fields(i).Source.Length - 5))
                                If dtGroups.Rows.Count >= (iLevel + 1) Then
                                    Me.Profile.Fields(i).Value = dtGroups.Rows(iLevel)("Name")
                                Else
                                    Me.Profile.Fields(i).Value = "" 'Cuidado
                                End If
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "SALDO_") Then ' Saldos
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "SALDO_RBS") = 0 Then
                                    If UBound(Profile.Fields(i).Source.Split("_")) = 1 Then bExportOneConceptByLine = True
                                    If UBound(Profile.Fields(i).Source.Split("_")) > 1 Then
                                        strAccrualKey = Profile.Fields(i).Source.Split("_")(2)
                                        Profile.Fields(i).ShortName = strAccrualKey.ToUpper
                                        mExportMoreThatOneConceptByLine = True
                                    End If

                                    ' Comprueba si hay que exportar la fecha del saldo
                                    If InStr(Profile.Fields(i).Source.ToUpper, "SALDO_FECHA") > 0 Then mExportConceptsWithDate = True
                                End If
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                'Profile.Fields(i).Value = "RBS not supported"
                                'Profile.Fields(i).Value = DataLinkDynamicCode.GetAccrualCellValueFromExternalCodeByHead(scriptFileName, Profile.Fields, dtRowsToExport, DataLinkDynamicCode.CreateDataTable_Employees(row("idEmployee"), oCn, Me.oState),
                                '                                                                           dtEmpUsrFields,
                                '                                                                           DataLinkDynamicCode.CreateDataTable_Contracts(row("idEmployee"), oCn, Me.oState),
                                '                                                                           DataLinkDynamicCode.CreateDataTable_Groups(row("idEmployee"), oCn, Me.oState),
                                '                                                                           DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(row("idEmployee"), oCn, Me.oState),
                                '                                                                               DataLinkDynamicCode.CreateDataTable_Authorizations(row("idEmployee"), oCn, Me.oState), row("BeginDate"), row("EndDate"), Me.mBeginDate, Me.mEndDate, Me.mExcelFileName, oCn)
                                Dim accrualCellValueByHead = New DataLink.AccrualCellValueByHead
                                Profile.Fields(i).Value = accrualCellValueByHead.GetValue(scriptFileName, Profile.Fields, dtRowsToExport, DataLinkDynamicCode.CreateDataTable_Employees(row("idEmployee"), Me.oState),
                                                                                                       dtEmpUsrFields,
                                                                                                       DataLinkDynamicCode.CreateDataTable_Contracts(row("idEmployee"), Me.oState),
                                                                                                       DataLinkDynamicCode.CreateDataTable_Groups(row("idEmployee"), Me.oState),
                                                                                                       DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(row("idEmployee"), Me.oState),
                                                                                                       DataLinkDynamicCode.CreateDataTable_Authorizations(row("idEmployee"), Me.oState), row("BeginDate"), row("EndDate"), Me.mBeginDate, Me.mEndDate, Me.mExcelFileName)
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "GRUPO_NIVEL_") Then
                                ' Buscamos el Nivel de departamento indicado
                                Dim strLevel As String = Profile.Fields(i).Source.ToUpper.Replace("GRUPO_NIVEL_", "").Trim
                                Dim iLevel As Integer
                                Try
                                    iLevel = Integer.Parse(strLevel)
                                    If dtGroupsOnDate.Rows.Count >= (iLevel + 1) Then
                                        Me.Profile.Fields(i).Value = dtGroupsOnDate.Rows(iLevel)("Name")
                                    Else
                                        Me.Profile.Fields(i).Value = ""
                                    End If
                                Catch ex As Exception
                                    'No es un número
                                    Profile.Fields(i).Value = ""
                                End Try
                            End If
                    End Select
                Next

                ' Si hay saldos de los dos tipos no es correcto porque la plantilla no es correcta
                bolRet = Not (bExportOneConceptByLine AndAlso mExportMoreThatOneConceptByLine)

                dt.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadInfo")
            End Try

            Return bolRet
        End Function

        Private Function LoadInfoOnlyDepartments(ByVal row As DataRow, ByVal dtEmpUsrFields As DataTable, ByVal dtGroups As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim bExportOneConceptByLine As Boolean = False
                Dim strAccrualKey As String = ""
                Dim i As Integer = 0

                Dim dt As New DataTable

                ' Para cada columna
                mExportMoreThatOneConceptByLine = False

                For i = 0 To Me.Profile.Fields.Count - 1
                    Profile.Fields(i).Value = ""

                    Select Case Profile.Fields(i).Source.ToUpper
                        Case "FECHA_INICIO_EXPORTACION", "FECHA_INICIO_EXPORTACIÓN"
                            Profile.Fields(i).Value = mBeginDate

                        Case "FECHA_FINAL_EXPORTACION", "FECHA_FINAL_EXPORTACIÓN"
                            Profile.Fields(i).Value = mEndDate

                        Case "GRUPO"
                            Profile.Fields(i).Value = row("GroupNameOnLevel")
                        Case "GRUPO_COMPLETO"
                            Profile.Fields(i).Value = row("FullGroupName")
                        Case Else
                            ' Determina el tipo de campo
                            If InStr(1, Profile.Fields(i).Source.ToUpper, "LITERAL_") Then
                                Profile.Fields(i).Value = Profile.Fields(i).Source.Substring(8, Profile.Fields(i).Source.Length - 8)
                            ElseIf Me.Profile.Fields(i).Source.ToUpper.StartsWith("NIVEL") Then
                                Dim iLevel As Integer = roTypes.Any2Integer(Me.Profile.Fields(i).Source.Substring(5, Me.Profile.Fields(i).Source.Length - 5))
                                If dtGroups.Rows.Count >= (iLevel + 1) Then
                                    Me.Profile.Fields(i).Value = dtGroups.Rows(iLevel)("Name")
                                Else
                                    Me.Profile.Fields(i).Value = ""
                                End If
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "SALDO_") Then ' Saldos
                                If InStr(1, Profile.Fields(i).Source.ToUpper, "SALDO_RBS") = 0 Then
                                    If UBound(Profile.Fields(i).Source.Split("_")) = 1 Then bExportOneConceptByLine = True
                                    If UBound(Profile.Fields(i).Source.Split("_")) > 1 Then
                                        strAccrualKey = Profile.Fields(i).Source.Split("_")(2)
                                        Profile.Fields(i).ShortName = strAccrualKey.ToUpper
                                        mExportMoreThatOneConceptByLine = True
                                    End If

                                    ' Comprueba si hay que exportar la fecha del saldo
                                    If InStr(Profile.Fields(i).Source.ToUpper, "SALDO_FECHA") > 0 Then mExportConceptsWithDate = True
                                End If
                                'ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "RBS_") Then ' Robotics script
                                '    Dim scriptFileName As String = Profile.Fields(i).Source.Substring(4, Profile.Fields(i).Source.Length - 4)
                                '    Profile.Fields(i).Value = DataLinkDynamicCode.GetAccrualCellValueFromExternalCodeByHead(scriptFileName, Profile.Fields, dtRowsToExport, DataLinkDynamicCode.CreateDataTable_Employees(row("idEmployee"), oCn, Me.oState), _
                                '                                                                               dtEmpUsrFields, _
                                '                                                                               DataLinkDynamicCode.CreateDataTable_Contracts(row("idEmployee"), oCn, Me.oState), _
                                '                                                                               DataLinkDynamicCode.CreateDataTable_Groups(row("idEmployee"), oCn, Me.oState), _
                                '                                                                               DataLinkDynamicCode.CreateDataTable_EmployeeMobilities(row("idEmployee"), oCn, Me.oState), _
                                '                                                                                   DataLinkDynamicCode.CreateDataTable_Authorizations(row("idEmployee"), oCn, Me.oState), row("BeginDate"), row("EndDate"), Me.mBeginDate, Me.mEndDate, Me.mExcelFileName)
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "GRUPO_NIVEL_") Then
                                Profile.Fields(i).Value = row("GroupNameOnLevel")
                            ElseIf InStr(1, Profile.Fields(i).Source.ToUpper, "GRUPO_TOTAL_EMPLEADOS") Then
                                Profile.Fields(i).Value = row("NumberEmployeesFromGroupLevel")
                            End If
                    End Select
                Next

                ' Si hay saldos de los dos tipos no es correcto porque la plantilla no es correcta
                bolRet = Not (bExportOneConceptByLine AndAlso mExportMoreThatOneConceptByLine)

                dt.Dispose()
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadInfoOnlyDepartments")
            End Try

            Return bolRet
        End Function

        Private Function LoadAccrualsForEmployee(ByVal idEmployee As Integer, ByVal BeginDate As Date, ByVal EndDate As Date) As DataTable
            Dim bd As Date = IIf(Me.mBeginDate > BeginDate, Me.mBeginDate, BeginDate)
            Dim ed As Date = IIf(Me.mEndDate < EndDate, Me.mEndDate, EndDate)

            If Me.ContractTerminations Then
                ' Si estoy pagando finiquitos:
                ' 0.- El periodo de exportación es referente a la fecha de baja, y no respecto a la fecha de ejecución
                If Now.Month <> EndDate.Month AndAlso Now.Date > EndDate Then
                    'Si la fecha de baja es anterior a la de hoy, el periodo debe ser el anterior al que he recibido
                    bd = bd.AddMonths(-1)
                End If

                ' 1.- Voy hasta el final del contrato, aunque esté más allá del periodo de nómina que estoy generando
                ed = EndDate
            End If

            Dim dt As DataTable = Nothing

            Try
                ' Cargo saldos. Si debo mostrar ceros, o bien si debo incluir todas las fechas porque se quiere detalle de fichajes o planificación, lo indico.
                If IsNothing(mdaDailyAccruals) Then mdaDailyAccruals = CreateDataAdapter_ConceptsEx(Me.ExportZeroValues OrElse Me.IncludePunches OrElse Me.IncludeSchedule, Me.IncludeEmployeeAccrualPeriods)

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

        Private Sub LoadDiningAccrualsForEmployee(ByVal idEmployee As Integer, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef dtDailyAccruals As DataTable, ByVal strDiningConcepts As String)
            'Dim bolRet As Boolean = False
            Dim bd As Date = IIf(Me.mBeginDate > BeginDate, Me.mBeginDate, BeginDate)
            Dim ed As Date = IIf(Me.mEndDate < EndDate, Me.mEndDate, EndDate)
            Dim dt As DataTable = Nothing
            Dim strSQL As String = ""
            Try
                ' Cargo saldos. Si debo mostrar ceros, o bien si debo incluir todas las fechas porque se quiere detalle de fichajes o planificación, lo indico.
                If IsNothing(mdaDiningAccruals) Then mdaDiningAccruals = CreateDataAdapter_DiningConcepts(Me.ExportZeroValues OrElse Me.IncludePunches OrElse Me.IncludeSchedule, strDiningConcepts)

                ' Lee todos los saldos del empleado entre fechas
                With mdaDiningAccruals.SelectCommand
                    .Parameters("@idEmployee").Value = idEmployee
                    .Parameters("@BeginDate").Value = bd
                    .Parameters("@EndDate").Value = ed
                End With

                dt = New DataTable
                mdaDiningAccruals.Fill(dt)

                Dim oRowAccruals() As DataRow = Nothing
                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    ' Crear una nueva columna de tipo booleana llamada "IsDining"
                    Dim colIsDining As New DataColumn("IsDining", GetType(Boolean))

                    ' Añadir la columna al DataTable
                    dtDailyAccruals.Columns.Add(colIsDining)

                    For Each orow As DataRow In dt.Rows
                        ' Añadimos el nuevo registro
                        Try
                            Dim aRow As DataRow = dtDailyAccruals.NewRow()
                            aRow("ShortName") = orow("ShortName")
                            aRow("TotalConcept") = orow("TotalConcept")
                            aRow("ConceptName") = orow("ConceptName")
                            aRow("IsDining") = True
                            dtDailyAccruals.Rows.Add(aRow)
                            dtDailyAccruals.AcceptChanges()
                        Catch ex As Exception
                        End Try
                    Next
                End If

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadDiningAccrualsForEmployee")
            End Try

        End Sub


        Private Function LoadFutureAccruals_Hipotels(ByRef strFutureConcepts As String) As DataTable
            Dim oConcepts As DataTable = Nothing

            Try
                strFutureConcepts = "'W','L','F','HS','HR+','18H','HPC','V','-V','12'"
                oConcepts = CreateDataTable("@SELECT# distinct ID, Name, ShortName from Concepts where (" & GetConceptsByLineDelimited() & ") AND ShortName in(" & strFutureConcepts & ")")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadFutureAccruals_Hipotels")
            End Try

            Return oConcepts
        End Function

        Private Sub LoadFutureAccrualsForEmployee_Hipotels(ByVal idEmployee As Integer, ByVal BeginDate As Date, ByVal EndDate As Date, ByVal oConcepts As DataTable, ByRef dtDailyAccruals As DataTable, ByVal strFutureConcepts As String)
            Dim bd As Date = IIf(Me.mBeginDate > BeginDate, Me.mBeginDate, BeginDate)
            Dim ed As Date = IIf(Me.mEndDate < EndDate, Me.mEndDate, EndDate)
            Dim strSQL As String = ""
            Dim oRowAccruals() As DataRow = Nothing

            Try
                ' Obtenemos los valores futuros, solo en caso de estar exportando datos del mes actual y que no sea el ultimo dia del mes
                If ed > Now.Date AndAlso Now.Date.Month = Now.Date.AddDays(1).Month Then
                    Dim FirstDate As Date = Now.Date.AddDays(1)
                    Dim LastDate As Date = New Date(FirstDate.Year, FirstDate.Month, 1)
                    LastDate = LastDate.AddMonths(1).AddDays(-1)
                    LastDate = IIf(LastDate < EndDate, LastDate, EndDate)
                    Dim oProgrammedAbsState = New Absence.roProgrammedAbsenceState(oState.IDPassport, oState.Context, oState.ClientAddress)

                    ' Filtramos por los saldos seleccionados que tengan tratamiento especial a futuro
                    If oConcepts IsNot Nothing AndAlso oConcepts.Rows.Count > 0 Then
                        For Each oConcept As DataRow In oConcepts.Rows
                            Select Case Any2String(oConcept("ShortName")).ToUpper
                                Case "W" 'W Turno partido
                                    ' Obtenemos cada uno de los dias planificados que contenga en la descripcion del horario 'Partido'
                                    strSQL = "@SELECT# Currentdate as Date, 1 as ExpectedWorkingHours FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.Description like '%Partido%' "
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 1)
                                    End If

                                Case "L" ' Dia Libre
                                    ' Obtenemos cada uno de los dias planificados con el horario que se llame 'Libranza'
                                    strSQL = "@SELECT# Currentdate as Date, 1 as ExpectedWorkingHours  FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.Name like 'Libranza' "
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 1)
                                    End If

                                Case "F" ' Festivo disfrutado
                                    ' Obtenemos cada uno de los dias planificados con el horario 'FESTIVO'
                                    strSQL = "@SELECT# Currentdate as Date, 1 as ExpectedWorkingHours FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.Name like 'FESTIVO' "
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 1)
                                    End If

                                Case "HS" ' Horas sindicales
                                    ' Previsiones de ausencia por horas/dias de la justificacion 'Horas sindicales'
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '9'"))
                                    Dim intFactor As Double = 0.125
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "Hs", intIDCause, intFactor, oProgrammedAbsState)

                                Case "HR+" ' Horas R+
                                    ' Previsiones de ausencia por horas/dias y excesos de la justificacion '10i Dia libre a compensar R+ '
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '10'"))
                                    Dim intFactor As Double = 0.125
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "HR+", intIDCause, intFactor, oProgrammedAbsState)

                                    ' Obtenemos cada uno de los dias planificados con el horario 'R+' y sus horas teoricas
                                    strSQL = "@SELECT# Currentdate as Date, 1 as ExpectedWorkingHours FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.Name like '%+R%' "
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 1)
                                    End If

                                Case "18H" ' Horas R-
                                    ' Previsiones de ausencia por horas/dias de la justificacion '18i compensación día libre trabajado R-'
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '18'"))
                                    Dim intFactor As Double = 0.125
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "18H", intIDCause, intFactor, oProgrammedAbsState)

                                    ' Obtenemos cada uno de los dias planificados con el horario 'Horario R+' y sus horas teoricas
                                    strSQL = "@SELECT# Currentdate as Date, 1 as ExpectedWorkingHours FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.Name like '%-R%' "
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 1)
                                    End If

                                Case "HPC" ' Horas K permiso retribuido
                                    ' Previsiones de ausencia por horas/dias de la justificacion '11 Permiso de convenio Retribuido '
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '11'"))
                                    Dim intFactor As Double = 0.125
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "HPC", intIDCause, intFactor, oProgrammedAbsState)

                                    ' Obtenemos cada uno de los dias planificados con el horario 'Horario R+' y sus horas teoricas
                                    strSQL = "@SELECT# Currentdate as Date, 1 as ExpectedWorkingHours FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.Name like 'K-Permiso Retribuido' "
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 1)
                                    End If

                                Case "V" ' Vacaciones
                                    ' Previsiones de vacaciones por horas de la justificación 'Vacaciones'
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE 'V'"))
                                    Dim intFactor As Double = 0.175
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "V", intIDCause, intFactor, oProgrammedAbsState)

                                    ' Obtenemos cada uno de los dias planificados con un horario de tipo Vacaciones que se justifique con la justificación "Vacaciones",
                                    ' y obtenemos las horas teoricas del horario base, en caso que sean 0 inidcamos un 8
                                    strSQL = "@SELECT# Currentdate as Date,  (case when isnull(ExpectedWorkingHoursAbs,0) = 0 then 8 else isnull(ExpectedWorkingHoursAbs,0)  end)  as ExpectedWorkingHours  FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.ShiftType = 2 and IDCauseHolidays= " & intIDCause.ToString
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 0.175)
                                    End If

                                Case "-V" ' 15 Añadir a vacaciones
                                    ' Previsiones de exceso con la justificación 15 añadir vacaciones
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '-V'"))
                                    Dim intFactor As Double = 0.175
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "-V", intIDCause, intFactor, oProgrammedAbsState)

                                    ' Obtenemos cada uno de los dias planificados con un horario de tipo Vacaciones que se justifique con la justificación "con la justificación 15 Añadir Vacaciones ",
                                    ' y obtenemos las horas teoricas del horario base, en caso que sean 0 inidcamos un 8
                                    strSQL = "@SELECT# Currentdate as Date,  (case when isnull(ExpectedWorkingHoursAbs,0) = 0 then 8 else isnull(ExpectedWorkingHoursAbs,0)  end)  as ExpectedWorkingHours  FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.ShiftType = 2 and IDCauseHolidays= " & intIDCause.ToString
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 0.175)
                                    End If
                                Case "12" ' 15 Añadir a vacaciones tiempo trabajado
                                    ' Previsiones de exceso con la justificación 15 añadir vacaciones
                                    Dim intIDCause = Any2Integer(ExecuteScalar("@SELECT# ID FROM CAUSES WHERE SHORTNAME LIKE '12'"))
                                    Dim intFactor As Double = 0.175
                                    AddFutureAcrualsForEmployee_PA_Hipotels(idEmployee, FirstDate, LastDate, dtDailyAccruals, "12", intIDCause, intFactor, oProgrammedAbsState)

                                    ' Obtenemos cada uno de los dias planificados con un horario de tipo Vacaciones que se justifique con la justificación "con la justificación 15 Añadir Vacaciones ",
                                    ' y obtenemos las horas teoricas del horario base, en caso que sean 0 inidcamos un 8
                                    strSQL = "@SELECT# Currentdate as Date,  (case when isnull(ExpectedWorkingHoursAbs,0) = 0 then 8 else isnull(ExpectedWorkingHoursAbs,0)  end)  as ExpectedWorkingHours  FROM sysroEmployeesShifts INNER JOIN Shifts ON sysroEmployeesShifts.IDSHift = Shifts.ID WHERE IDEmployee = " & idEmployee.ToString & " AND Currentdate >=" & Any2Time(FirstDate).SQLSmallDateTime & " AND Currentdate <=" & Any2Time(LastDate).SQLSmallDateTime & " AND Shifts.ShiftType = 2 and IDCauseHolidays= " & intIDCause.ToString
                                    Dim oDays As DataTable = CreateDataTable(strSQL)
                                    If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                                        SetFutureAcrualValue_Hipotels(oDays, oConcept("ShortName"), dtDailyAccruals, 0.175)
                                    End If

                            End Select
                        Next
                    End If
                End If

                ' Reordenamos el datatable por fecha
                If dtDailyAccruals IsNot Nothing AndAlso mExportConceptsWithDate Then
                    Dim oScheduleDataView As New DataView
                    oScheduleDataView = New System.Data.DataView(dtDailyAccruals)
                    oScheduleDataView.Sort = "Date ASC"

                    dtDailyAccruals = oScheduleDataView.ToTable
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadFutureAccrualsForEmployee_Hipotels")
            End Try

        End Sub

        Private Sub AddFutureAcrualsForEmployee_PA_Hipotels(ByVal idEmployee As Integer, ByVal BeginDate As Date, ByVal EndDate As Date, ByRef dtAccruals As DataTable, ByVal ShortNameConcept As String, ByVal IDCause As Integer, ByVal intFactor As Double, Optional ByVal oProgrammedAbsState As Absence.roProgrammedAbsenceState = Nothing)
            Dim tbProgrammedAbsences As DataTable = Nothing
            Dim tbProgrammedCauses As DataTable = Nothing
            Dim BDate As Date
            Dim EDate As Date
            Dim strSQL As String = ""
            Dim oDays As DataTable = Nothing

            Try
                If oProgrammedAbsState Is Nothing Then oProgrammedAbsState = New Absence.roProgrammedAbsenceState(oState.IDPassport, oState.Context, oState.ClientAddress)

                Dim oRows() As DataRow = Nothing
                tbProgrammedAbsences = Absence.roProgrammedAbsence.GetProgrammedAbsences(idEmployee, "", oProgrammedAbsState)
                tbProgrammedCauses = Absence.roProgrammedAbsence.GetProgrammedCauses(idEmployee, BeginDate, EndDate, oProgrammedAbsState)

                '01. PREVISIONES DE AUSENCIA POR DIAS
                oRows = tbProgrammedAbsences.Select("(BeginDate <= '" & Format(EndDate, "yyyy/MM/dd") & "' AND " &
                                                        "RealFinishDate >= '" & Format(BeginDate, "yyyy/MM/dd") & "') AND IDCause =" & IDCause.ToString)
                If oRows IsNot Nothing AndAlso oRows.Length > 0 Then
                    For Each orow As DataRow In oRows
                        ' Para cada previsiones de ausencia, obtenemos el periodo a tener en cuenta
                        Dim _Begin As Double = Math.Max(roTypes.Any2Time(orow("BeginDate")).NumericValue, roTypes.Any2Time(BeginDate).NumericValue)
                        Dim _Finish As Double = Math.Min(roTypes.Any2Time(orow("RealFinishDate")).NumericValue, roTypes.Any2Time(EndDate).NumericValue)
                        BDate = roTypes.Any2Time(_Begin).ValueDateTime
                        EDate = roTypes.Any2Time(_Finish).ValueDateTime

                        ' Obtenemos cada día que haya que generar horas de ausencia a futuro
                        strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end) ExpectedWorkingHours, D.Date FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & idEmployee.ToString
                        strSQL += " AND D.Date >= " & Any2Time(BDate).SQLSmallDateTime
                        strSQL += " AND D.Date <= " & Any2Time(EDate).SQLSmallDateTime
                        strSQL += " AND S.ID = D.IDShift1 "
                        strSQL += " AND (case when isnull(D.IsHolidays,0) = 1 then 0 else isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours)  end) > 0"

                        oDays = CreateDataTable(strSQL)
                        If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                            SetFutureAcrualValue_Hipotels(oDays, ShortNameConcept, dtAccruals, intFactor)
                        End If
                    Next
                End If

                '02. PREVISIONES DE AUSENCIA POR HORAS
                oRows = tbProgrammedCauses.Select("(Date <= '" & Format(EndDate, "yyyy/MM/dd") & "' and isnull(FinishDate, date) >= '" & Format(BeginDate, "yyyy/MM/dd") & "') AND IDCause =" & IDCause.ToString)
                If oRows IsNot Nothing AndAlso oRows.Length > 0 Then
                    Dim dblDuration As Double = 0
                    For Each orow As DataRow In oRows
                        ' Para cada previsiones de ausencia por horas, obtenemos el periodo a tener en cuenta
                        Dim _Begin As Double = Math.Max(roTypes.Any2Time(orow("Date")).NumericValue, roTypes.Any2Time(BeginDate).NumericValue)
                        Dim _Finish As Double = Math.Min(roTypes.Any2Time(orow("FinishDate")).NumericValue, roTypes.Any2Time(EndDate).NumericValue)
                        BDate = roTypes.Any2Time(_Begin).ValueDateTime
                        EDate = roTypes.Any2Time(_Finish).ValueDateTime

                        dblDuration = Any2Double(orow("Duration"))

                        ' Obtenemos cada día que haya que generar horas de ausencia a futuro
                        strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end) ExpectedWorkingHours, D.Date FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & idEmployee.ToString
                        strSQL += " AND D.Date >= " & Any2Time(BDate).SQLSmallDateTime
                        strSQL += " AND D.Date <= " & Any2Time(EDate).SQLSmallDateTime
                        strSQL += " AND S.ID = D.IDShift1 "
                        strSQL += " AND (case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end) > 0"

                        oDays = CreateDataTable(strSQL)
                        If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                            SetFutureAcrualValue_Hipotels(oDays, ShortNameConcept, dtAccruals, intFactor)
                        End If
                    Next
                End If

                ' PREVISIONES DE EXCESO
                Dim oProgrammedOvertimeyManager As New VTHolidays.roProgrammedOvertimeManager()
                Dim oProgrammedOvertimesState As New VTHolidays.roProgrammedOvertimeState(oState.IDPassport, oState.Context, oState.ClientAddress)
                Dim lstProgrammedOvertimes As Generic.List(Of roProgrammedOvertime) = oProgrammedOvertimeyManager.GetProgrammedOvertimes(idEmployee, oProgrammedOvertimesState).FindAll(Function(x) (x.ProgrammedBeginDate <= EndDate AndAlso x.ProgrammedEndDate >= BeginDate And x.IDCause = IDCause))
                If lstProgrammedOvertimes IsNot Nothing AndAlso lstProgrammedOvertimes.Count > 0 Then
                    Dim dblDuration As Double = 0
                    For Each oProgrammedOvertime As roProgrammedOvertime In lstProgrammedOvertimes
                        ' Para cada previsiones de ausencia por horas, obtenemos el periodo a tener en cuenta
                        Dim _Begin As Double = Math.Max(roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).NumericValue, roTypes.Any2Time(BeginDate).NumericValue)
                        Dim _Finish As Double = Math.Min(roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).NumericValue, roTypes.Any2Time(EndDate).NumericValue)
                        BDate = roTypes.Any2Time(_Begin).ValueDateTime
                        EDate = roTypes.Any2Time(_Finish).ValueDateTime

                        dblDuration = oProgrammedOvertime.Duration

                        ' Obtenemos cada día que haya que generar horas de exceso a futuro
                        strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end) ExpectedWorkingHours, D.Date FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & idEmployee.ToString
                        strSQL += " AND D.Date >= " & Any2Time(BDate).SQLSmallDateTime
                        strSQL += " AND D.Date <= " & Any2Time(EDate).SQLSmallDateTime
                        strSQL += " AND S.ID = D.IDShift1 "
                        strSQL += " AND (case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end) > 0"

                        oDays = CreateDataTable(strSQL)
                        If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                            SetFutureAcrualValue_Hipotels(oDays, ShortNameConcept, dtAccruals, intFactor)
                        End If
                    Next
                End If

                ' VACACIONES POR HORAS
                Dim oProgrammedHolidayManager As New roProgrammedHolidayManager()
                Dim lstProgrammedHolidays As New Generic.List(Of roProgrammedHoliday)
                lstProgrammedHolidays = oProgrammedHolidayManager.GetProgrammedHolidays(idEmployee, New roProgrammedHolidayState(oState.IDPassport, oState.Context, oState.ClientAddress), "Date >=" & roTypes.Any2Time(BeginDate).SQLSmallDateTime & " AND Date <=" & roTypes.Any2Time(EndDate).SQLSmallDateTime)
                If lstProgrammedHolidays IsNot Nothing AndAlso lstProgrammedHolidays.Count > 0 Then
                    Dim dblDuration As Double = 0
                    For Each oProgrammedHolidays As roProgrammedHoliday In lstProgrammedHolidays
                        dblDuration = oProgrammedHolidays.Duration

                        If oProgrammedHolidays.AllDay Then
                            ' Obtenemos el día que haya que generar horas de vacaciones a futuro teniendo en cuenta las horas teoricas del horario planficado
                            strSQL = "@SELECT# isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) as ExpectedWorkingHours, D.Date FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & idEmployee.ToString
                            strSQL += " AND D.Date = " & Any2Time(oProgrammedHolidays.ProgrammedDate).SQLSmallDateTime
                            strSQL += " AND S.ID = D.IDShift1 "
                            strSQL += " AND isnull(isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours),0)  > 0"
                        Else
                            ' Obtenemos el día que haya que generar horas de vacaciones a futuro teniendo en cuenta las horas indicadas en la prevision
                            strSQL = "@SELECT# (case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end) ExpectedWorkingHours, D.Date FROM DailySchedule D, Shifts S WHERE D.IDEmployee=" & idEmployee.ToString
                            strSQL += " AND D.Date = " & Any2Time(oProgrammedHolidays.ProgrammedDate).SQLSmallDateTime
                            strSQL += " AND S.ID = D.IDShift1 "
                            strSQL += " AND (case when isnull(D.IsHolidays,0) = 1 then 0 else case when isnull(D.ExpectedWorkingHours, S.ExpectedWorkingHours) = 0 then 0 else " & dblDuration.ToString.Replace(",", ".") & " end end) > 0"
                        End If
                        oDays = CreateDataTable(strSQL)
                        If oDays IsNot Nothing AndAlso oDays.Rows.Count > 0 Then
                            SetFutureAcrualValue_Hipotels(oDays, ShortNameConcept, dtAccruals, intFactor)
                        End If
                    Next

                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport:: ProfileExportConcepts:AddFutureAcrualsForEmployee_PA_Hipotels")
            End Try

        End Sub

        Private Sub SetFutureAcrualValue_Hipotels(ByVal oDays As DataTable, ByVal ShortNameConcept As String, ByRef dtAccruals As DataTable, ByVal intFactor As Double)
            Try

                ' Obtenemos los datos del saldo relacionado
                Dim oRowAccruals() As DataRow = Nothing
                For Each oDay As DataRow In oDays.Rows
                    ' De cada día , debemos crear o añadir un registro en el datatable de dtAccruals
                    If mExportConceptsWithDate Then
                        oRowAccruals = dtAccruals.Select("Date = '" & Format(oDay("Date"), "yyyy/MM/dd") & "' AND ShortName ='" & ShortNameConcept & "'")
                    Else
                        oRowAccruals = dtAccruals.Select("ShortName ='" & ShortNameConcept & "'")
                    End If

                    If oRowAccruals IsNot Nothing AndAlso oRowAccruals.Length > 0 Then
                        ' Actualizamos el registro existente
                        oRowAccruals(0)("TotalConcept") = oRowAccruals(0)("TotalConcept") + (oDay("ExpectedWorkingHours") * intFactor)
                    Else
                        ' Añadimos el nuevo registro
                        Dim aRow As DataRow = dtAccruals.NewRow()
                        If mExportConceptsWithDate Then aRow("Date") = oDay("Date")
                        aRow("ShortName") = ShortNameConcept
                        aRow("Date") = oDay("Date")
                        aRow("TotalConcept") = (oDay("ExpectedWorkingHours") * intFactor)
                        aRow("ConceptName") = ShortNameConcept
                        dtAccruals.Rows.Add(aRow)
                    End If
                    dtAccruals.AcceptChanges()
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport:: ProfileExportConcepts:SetFutureAcrualValue_Hipotels")
            End Try

        End Sub



        Private Function LoadAccrualsForEmployeeAtDate(ByVal idEmployee As Integer, ByVal strConceptsFilterByGroup As String, ByVal oParams As roParameters, ByVal EndDate As Date) As DataTable
            Dim dt As New DataTable

            Try
                Dim sSQL As String = "@SELECT# ID as IDConcept, Name as ConceptName, ShortName, 0.0 As TotalConcept, GETDATE() As Date, IDType as Type, DefaultQuery, GETDATE() as BeginPeriod, GETDATE() as EndPeriod, NULL as ExpiredDate, NULL as StartEnjoymentDate FROM Concepts WHERE Export <> '0'"
                If strConceptsFilterByGroup.Length > 0 Then sSQL = sSQL & " AND ShortName in (" & strConceptsFilterByGroup & ") "

                dt = CreateDataTable(sSQL, "AccrualsAtDate")

                Dim value As Double
                Dim oConcept As Concept.roConcept
                Dim oConceptState As New Concept.roConceptState(-1)
                For Each oRow In dt.Rows
                    oConcept = New Concept.roConcept(oRow("IDConcept"), oConceptState)
                    Dim lstDates As Generic.List(Of DateTime) = Nothing
                    value = Concept.roConcept.GetAccrualValueOnDate(idEmployee, oParams, EndDate, False, oConcept, Nothing, lstDates)
                    If lstDates.Count = 2 Then
                        oRow("BeginPeriod") = lstDates(0)
                        oRow("EndPeriod") = lstDates(1)
                    Else
                        oRow("BeginPeriod") = DBNull.Value
                        oRow("EndPeriod") = DBNull.Value
                    End If
                    oRow("TotalConcept") = value
                    oRow("Date") = EndDate
                    If (Me.ExportZeroValues = False OrElse oConcept.DefaultQuery = "L") AndAlso value = 0 Then
                        oRow.Delete()
                    End If
                Next

                If dt IsNot Nothing Then dt.AcceptChanges()

            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadAccrualsForEmployeeAtDate")
            End Try

            Return dt
        End Function

        Private Function LoadAccrualsForDepartmentLevel(ByVal strEmployees As String, ByVal strDepartmentName As String) As DataTable
            Dim dt As DataTable = Nothing

            Try
                'If IsNothing(mdaDailyAccruals) Then mdaDailyAccruals = CreateDataAdapter_Concepts_ByDepartamentLevel(cn)
                Dim strSQL As String = ""

                ' Exportación de saldos totalizados
                strSQL = "@SELECT# SUM(isnull(DailyAccruals.Value,0)) AS TotalConcept, Concepts.ShortName AS ShortName "
                strSQL = strSQL & " FROM DailyAccruals, Concepts, sysrovwCurrentEmployeeGroups, EmployeeContracts"

                If strEmployees <> "" Then strSQL &= " INNER JOIN " & strEmployees & " ON " & strEmployees & ".Id = sysrovwCurrentEmployeeGroups.IDEmployee "

                strSQL = strSQL & " where DailyAccruals.IDConcept =Concepts.ID"
                strSQL = strSQL & " and DailyAccruals.IDEmployee=sysrovwCurrentEmployeeGroups.IDEmployee"
                'strSQL = strSQL & " and sysrovwCurrentEmployeeGroups.IDEmployee in (" & strEmployees & ")  "
                strSQL = strSQL & " and Concepts.Export<>'0' and (" & GetConceptsByLineDelimited() & ") "
                'strSQL = strSQL & " and  rtrim(ltrim(dbo.UFN_SEPARATES_COLUMNS(sysrovwCurrentEmployeeGroups.FullGroupName,2,'\'))) = '" & strDepartmentName.Trim & "'"
                strSQL = strSQL & " and  rtrim(ltrim(dbo.UFN_SEPARATES_COLUMNS(sysrovwCurrentEmployeeGroups.FullGroupName," & (mOnlyDepartamentsOnLevel + 1).ToString & ",'\'))) = '" & strDepartmentName.Trim & "'"
                strSQL = strSQL & " and DailyAccruals.Date between " & roTypes.Any2Time(Me.mBeginDate).SQLDateTime & " and " & roTypes.Any2Time(Me.mEndDate).SQLDateTime & ""
                strSQL = strSQL & " and EmployeeContracts.IDEmployee = sysrovwCurrentEmployeeGroups.IDEmployee "
                strSQL = strSQL & " and DailyAccruals.Date between EmployeeContracts.BeginDate and EmployeeContracts.EndDate "
                strSQL = strSQL & " group by Concepts.shortname"

                dt = CreateDataTable(strSQL, "Accruals")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:LoadConcepts")
            End Try

            Return dt
        End Function

        Private Function CreateDataAdapter_ConceptsEx(ByVal includeZeros As Boolean, Optional ByVal includeConceptPeriods As Boolean = False) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim AccFilteredBy As String
                Dim strSQL As String = ""

                If mExportConceptsWithDate Then
                    ' Exportación de saldos con fecha
                    strSQL = "WITH alldays AS ( " &
                            "@SELECT# @BeginDate AS dt " &
                                "UNION ALL " &
                            "@SELECT# DATEADD(dd, 1, dt) " &
                                "FROM alldays s " &
                                "WHERE DATEADD(dd, 1, dt) <= @EndDate) "

                    If Not includeConceptPeriods Then
                        strSQL &= "@SELECT# ShortName AS ShortName, ConceptName, dateConcept.Date AS Date, isnull(dbo.DailyAccruals.Value,0) AS TotalConcept, DailyAccruals.BeginPeriod, DailyAccruals.EndPeriod, @BeginDate AS BeginComputedPeriod, @EndDate AS EndComputedPeriod, StartEnjoymentDate, ExpiredDate "
                    Else
                        strSQL &= "@SELECT# ShortName AS ShortName, ConceptName, dateConcept.Date AS Date, isnull(dbo.DailyAccruals.Value,0) AS TotalConcept, DailyAccruals.BeginPeriod, DailyAccruals.EndPeriod, @BeginDate AS BeginComputedPeriod, @EndDate AS EndComputedPeriod, StartEnjoymentDate, ExpiredDate "
                    End If
                    strSQL &= "FROM (@SELECT# alldays.dt AS Date, Concepts.ID AS IDConcept, Concepts.Name AS ConceptName, Concepts.DefaultQuery, Concepts.ShortName AS ShortName, @idEmployee AS IDEmployee from alldays,Concepts WHERE Concepts.Export<>'0' "

                    If mExportMoreThatOneConceptByLine Then
                        ' Exportación de saldos con fecha con varios saldos por linea
                        strSQL &= " and (" & GetConceptsByLineDelimited() & ") "
                    End If

                    strSQL &= ") dateConcept "

                    If includeZeros Then
                        strSQL &= " LEFT OUTER JOIN "
                    Else
                        strSQL &= " INNER JOIN "
                    End If

                    strSQL &= "DailyAccruals ON dateConcept.Date = DailyAccruals.Date AND DailyAccruals.IDConcept = dateConcept.IDConcept AND dateConcept.IDEmployee = DailyAccruals.IDEmployee "

                    strSQL &= " WHERE (DefaultQuery <> 'L' OR value <> 0) "

                    If Not mExportMoreThatOneConceptByLine Then
                        AccFilteredBy = GetConceptsFilteredBy()
                        If AccFilteredBy <> "" Then strSQL &= " AND ShortName in (" & AccFilteredBy & ") "
                    End If

                    strSQL &= " ORDER BY dateConcept.Date, ShortName, ConceptName "
                    strSQL &= " OPTION (maxrecursion 0) "
                Else
                    ' Exportación de saldos totalizados
                    strSQL = "WITH alldays AS ( " &
                            "@SELECT# @BeginDate AS dt " &
                                "UNION ALL " &
                            "@SELECT# DATEADD(dd, 1, dt) " &
                                "FROM alldays s " &
                                "WHERE DATEADD(dd, 1, dt) <= @EndDate) "
                    If Not includeConceptPeriods Then
                        strSQL &= "@SELECT# SUM(isnull(dbo.DailyAccruals.Value,0)) AS TotalConcept, ShortName AS ShortName, ConceptName, MAX(dbo.DailyAccruals.BeginPeriod) As BeginPeriod, MAX(dbo.DailyAccruals.EndPeriod) As EndPeriod, @BeginDate AS BeginComputedPeriod, @EndDate AS EndComputedPeriod, StartEnjoymentDate, ExpiredDate "
                    Else
                        strSQL &= "@SELECT# SUM(isnull(dbo.DailyAccruals.Value,0)) As TotalConcept, ShortName As ShortName, ConceptName, dbo.DailyAccruals.BeginPeriod As BeginPeriod, DailyAccruals.EndPeriod As EndPeriod, @BeginDate AS BeginComputedPeriod, @EndDate AS EndComputedPeriod, StartEnjoymentDate, ExpiredDate "
                    End If
                    strSQL &= "FROM (@SELECT# alldays.dt As Date, Concepts.ID As IDConcept, Concepts.Name As ConceptName, Concepts.DefaultQuery, Concepts.ShortName As ShortName, @idEmployee As IDEmployee from alldays,Concepts "

                    If mExportMoreThatOneConceptByLine Then
                        ' Exportación de saldos con fecha con varios saldos por linea
                        strSQL &= " WHERE Concepts.Export<>'0' and (" & GetConceptsByLineDelimited() & ") "
                    End If

                    strSQL &= ") dateConcept "

                    If includeZeros Then
                        strSQL &= " LEFT OUTER JOIN "
                    Else
                        strSQL &= " INNER JOIN "
                    End If

                    strSQL &= "DailyAccruals ON dateConcept.Date = DailyAccruals.Date AND DailyAccruals.IDConcept = dateConcept.IDConcept AND dateConcept.IDEmployee = DailyAccruals.IDEmployee "
                    If Not mExportMoreThatOneConceptByLine Then
                        AccFilteredBy = GetConceptsFilteredBy()
                        If AccFilteredBy <> "" Then strSQL &= " WHERE ShortName in (" & AccFilteredBy & ")"
                    End If

                    ' Group by
                    If Not includeConceptPeriods Then
                        strSQL &= "GROUP BY ShortName, ConceptName, DailyAccruals.StartEnjoymentDate, DailyAccruals.ExpiredDate"
                    Else
                        strSQL &= "GROUP BY ShortName, ConceptName, DailyAccruals.BeginPeriod, DailyAccruals.EndPeriod, DailyAccruals.StartEnjoymentDate, DailyAccruals.ExpiredDate"
                    End If

                    strSQL &= " OPTION (maxrecursion 0) "
                End If

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:CreateDataAdapter_Saldos")
            End Try

            Return da
        End Function

        Private Function CreateDataAdapter_DiningConcepts(ByVal includeZeros As Boolean, ByVal strDiningConcepts As String) As DbDataAdapter
            Dim da As DbDataAdapter = Nothing

            Try
                Dim strSQL As String = ""


                ' Exportación de saldos totalizados
                strSQL = "WITH alldays AS ( " &
                            "@SELECT# @BeginDate AS dt " &
                                "UNION ALL " &
                            "@SELECT# DATEADD(dd, 1, dt) " &
                                "FROM alldays s " &
                                "WHERE DATEADD(dd, 1, dt) <= @EndDate) "
                strSQL &= "@SELECT# COUNT(punches.ID) AS TotalConcept, ShortName AS ShortName, ConceptName, NULL As BeginPeriod, NULL As EndPeriod "
                strSQL &= "FROM (@SELECT# alldays.dt AS Date, DiningRoomTurns.ID AS IDConcept, DiningRoomTurns.Name AS ConceptName, 'Y' as DefaultQuery, DiningRoomTurns.Export AS ShortName, @idEmployee AS IDEmployee from alldays,DiningRoomTurns "
                strSQL &= ") dateConcept "

                If includeZeros Then
                    strSQL &= " LEFT OUTER JOIN "
                Else
                    strSQL &= " INNER JOIN "
                End If

                strSQL &= "Punches with (nolock) ON dateConcept.Date = Punches.ShiftDate AND Punches.TypeData = dateConcept.IDConcept AND dateConcept.IDEmployee = Punches.IDEmployee  AND Punches.Type=10 and Punches.InvalidType is null "

                If strDiningConcepts <> "" Then strSQL &= " WHERE IDConcept in (" & strDiningConcepts & ") "

                ' Group by
                strSQL &= "GROUP BY ShortName, ConceptName"
                strSQL &= " OPTION (maxrecursion 0) "

                Dim cmd As DbCommand = CreateCommand(strSQL)

                AddParameter(cmd, "@idEmployee", DbType.Int32)
                AddParameter(cmd, "@BeginDate", DbType.Date)
                AddParameter(cmd, "@EndDate", DbType.Date)
                da = CreateDataAdapter(cmd, False)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportConcepts:CreateDataAdapter_DiningConcepts")
            End Try

            Return da
        End Function




        Private Function GetConceptsByLine() As List(Of String)
            Dim IndivConcepts As New List(Of String)
            Dim i As Integer = 0

            ' Lee los saldos individuales
            For i = 0 To Me.Profile.Fields.Count - 1
                If Me.Profile.Fields(i).ShortName <> "" Then
                    Dim j As Integer
                    ' Comprueba que el saldo no exista
                    For j = 0 To IndivConcepts.Count - 1
                        If IndivConcepts(j) = Me.Profile.Fields(i).ShortName Then Exit For
                    Next

                    ' Nuevo saldo
                    If j = IndivConcepts.Count Then IndivConcepts.Add(Me.Profile.Fields(i).ShortName)
                End If
            Next

            Return IndivConcepts
        End Function

        Private Function GetConceptsByLineDelimited() As String
            ' Determina si hay conceptos individuales
            Dim aux As String = ""
            Dim ConceptsByLine As New List(Of String)
            Dim i As Integer = 0

            ConceptsByLine = GetConceptsByLine()
            For i = 0 To ConceptsByLine.Count - 1
                If i > 0 Then aux += " or "
                aux += "dbo.Concepts.ShortName='" & ConceptsByLine(i) & "'"
            Next

            Return aux
        End Function

        Private Function GetConceptsFilteredBy() As String
            Dim str As String = ""

            Try
                Dim Aux As String = ""
                ' Filtra por los conceptos indicados
                If mAccrualsFilteredBy <> 0 Then
                    'Dim AccFilteredBy() As String = mAccrualsFilteredBy.Split(";")

                    ' Carga los conceptos del grupo
                    Dim Cpts As New Concept.roConceptGroup
                    Dim _state As New Concept.roConceptState

                    Cpts.ID = mAccrualsFilteredBy
                    Dim myList As List(Of Concept.roConcept) = Concept.roConceptGroup.GetConceptsforGroup(Cpts.ID, _state)

                    For i As Integer = 0 To myList.Count - 1
                        If i > 0 Then Aux += ","

                        Aux += "'" & myList(i).ShortName & "'"
                    Next
                End If

                str = Aux
            Catch ex As Exception

            End Try

            Return str
        End Function

        Private Function GetTurnsFilteredBy() As String
            Dim str As String = ""

            Try
                Dim Aux As String = ""

                ' Carga los turnos que se tienen que exportar
                Dim oState = New roDiningRoomState(-1)
                Dim tb As DataTable = Nothing
                Dim oDiningRoomTurn As New roDiningRoomTurn(-1, oState)
                tb = oDiningRoomTurn.GetDiningRoomTurns(-1)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    ' Filtramos por los turnos que tengan codigo de exportación
                    Dim oRows As DataRow() = tb.Select("Export is not null and Export <> '0'", "ID ASC")
                    If oRows Is Nothing OrElse oRows.Length = 0 Then
                    Else
                        For Each orow In oRows
                            If Aux.Length > 0 Then Aux += ","
                            Aux += orow("ID").ToString
                        Next
                    End If
                End If

                str = Aux
            Catch ex As Exception

            End Try

            Return str
        End Function



        Private Function GetUserFieldValueOnDate(dStartDate As DateTime, dEndDate As DateTime, sFieldName As String, dtEmpUsrFieldHistory As DataTable, ByRef FieldType As FieldTypes) As DataRow
            Dim oRet As DataRow = Nothing
            Try
                For Each oEmpUsrFieldHistoryRow As DataRow In dtEmpUsrFieldHistory.Rows
                    If oEmpUsrFieldHistoryRow("EndDate") >= dEndDate AndAlso oEmpUsrFieldHistoryRow("BeginDate") <= dEndDate AndAlso roTypes.Any2String(oEmpUsrFieldHistoryRow("FieldName")).Trim.ToUpper = sFieldName.Trim.ToUpper Then
                        oRet = oEmpUsrFieldHistoryRow
                        FieldType = oRet("FieldType")
                        Exit For
                    End If
                Next
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDataLinkExport::ProfileExportEmployees:GetUserFieldValueOnDate")
            End Try
            Return oRet
        End Function

        Private Function EuropeAssistance_Calendar(ByVal sShiftAdvancedParameter As String, ByVal sPart As String) As String
            Dim oRet As String = String.Empty
            Const cError As String = "##DEF_ERROR##"
            Try
                ' Especificación de horario: HoraOFranjaDeEntrada@HoraFranjaDeSalida@FranjaDeComida@TiempoComida
                ' Ejemplo (con flexibilidad en entrada y salida): 8:00-9:00@16:00-20:00@13:00-14:00@00:45

                ' 0.- Validamos
                Dim sKey As String = "DEFINICIÓN"
                If sShiftAdvancedParameter.Trim.Length = 0 Then
                    Return oRet
                ElseIf Not (sShiftAdvancedParameter.ToUpper.Contains("DEFINICIÓN") OrElse sShiftAdvancedParameter.ToUpper.Contains("DEFINICION")) Then
                    Return oRet
                Else
                    If sShiftAdvancedParameter.ToUpper.Contains("DEFINICION") Then
                        sKey = "DEFINICION"
                    End If
                    Dim sTemp As String = String.Empty
                    sTemp = sShiftAdvancedParameter.Substring(sShiftAdvancedParameter.ToUpper.IndexOf(sKey))
                    Dim iFirstDelimiter As Integer = -1
                    Dim iLastDelimiter As Integer = -1
                    iFirstDelimiter = sTemp.IndexOf("[")
                    iLastDelimiter = sTemp.IndexOf("]")
                    If iFirstDelimiter = -1 OrElse iLastDelimiter = -1 Then
                        Return cError
                    End If
                    sShiftAdvancedParameter = sTemp.Substring(iFirstDelimiter + 1, iLastDelimiter - iFirstDelimiter - 1)
                End If

                If sShiftAdvancedParameter.Split("@").Length <> 4 Then
                    Return cError
                End If

                ' 1.- Identificamos
                Dim sEntrada As String = String.Empty
                Dim sFlexEntrada As String = String.Empty
                Dim sSalida As String = String.Empty
                Dim sFlexSalida As String = String.Empty
                Dim sIniComida As String = String.Empty
                Dim sFinComida As String = String.Empty
                Dim sTiempoComida As String = String.Empty

                ' 1.1.- Entrada
                If sShiftAdvancedParameter.Split("@")(0).Split("-").Length = 2 Then
                    sEntrada = sShiftAdvancedParameter.Split("@")(0).Split("-")(0).Trim
                    sFlexEntrada = sShiftAdvancedParameter.Split("@")(0).Split("-")(1).Trim
                ElseIf sShiftAdvancedParameter.Split("@")(0).Split("-").Length = 1 Then
                    sEntrada = sShiftAdvancedParameter.Split("@")(0).Trim()
                    sFlexEntrada = sEntrada
                Else
                    sEntrada = cError
                    sFlexEntrada = sEntrada
                End If

                ' 1.2.- Salida
                If sShiftAdvancedParameter.Split("@")(1).Split("-").Length = 2 Then
                    sSalida = sShiftAdvancedParameter.Split("@")(1).Split("-")(0).Trim
                    sFlexSalida = sShiftAdvancedParameter.Split("@")(1).Split("-")(1).Trim
                ElseIf sShiftAdvancedParameter.Split("@")(1).Split("-").Length = 1 Then
                    sSalida = sShiftAdvancedParameter.Split("@")(1).Trim()
                    sFlexSalida = sSalida
                Else
                    sSalida = cError
                    sFlexSalida = sSalida
                End If

                ' 1.3.- Comida
                If sShiftAdvancedParameter.Split("@")(2).Split("-").Length = 2 Then
                    sIniComida = sShiftAdvancedParameter.Split("@")(2).Split("-")(0).Trim
                    sFinComida = sShiftAdvancedParameter.Split("@")(2).Split("-")(1).Trim
                    sTiempoComida = sShiftAdvancedParameter.Split("@")(3).Trim()
                ElseIf sShiftAdvancedParameter.Split("@")(2).Trim <> "" Then
                    sIniComida = cError
                    sFinComida = sIniComida
                End If

                ' 1.4.- Comida
                sTiempoComida = sShiftAdvancedParameter.Split("@")(3).Trim()

                Select Case sPart
                    Case "ENTRADA"
                        oRet = sEntrada
                    Case "FLEX_ENTRADA"
                        oRet = sFlexEntrada
                    Case "SALIDA"
                        oRet = sSalida
                    Case "FLEX_SALIDA"
                        oRet = sFlexSalida
                    Case "INI_COMIDA"
                        oRet = sIniComida
                    Case "FIN_COMIDA"
                        oRet = sFinComida
                    Case "TIEMPO_COMIDA"
                        oRet = sTiempoComida
                End Select
            Catch ex As Exception
            End Try
            Return oRet
        End Function

#End Region

    End Class

End Namespace