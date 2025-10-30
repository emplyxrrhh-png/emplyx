Imports System.Data.Common
Imports System.Xml.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase.roTypes

Namespace Shift

    Public Class roDailySchedule

#Region "Declarations - Constructors"

        Private oState As roShiftState

        Private intIDEmployee As Integer
        Private xDate As Date

        Private intIDShift1 As Nullable(Of Integer)
        Private intIDShift2 As Nullable(Of Integer)
        Private intIDShift3 As Nullable(Of Integer)
        Private intIDShift4 As Nullable(Of Integer)
        Private intIdShiftBase As Nullable(Of Integer)

        Private xStartShift1 As DateTime
        Private xStartShift2 As DateTime
        Private xStartShift3 As DateTime
        Private xStartShift4 As DateTime
        Private xStartShiftBase As DateTime

        Private strNameShift1 As String
        Private strNameShift2 As String
        Private strNameShift3 As String
        Private strNameShift4 As String
        Private strNameShiftBase As String

        Private strShortNameShift1 As String
        Private strShortNameShift2 As String
        Private strShortNameShift3 As String
        Private strShortNameShift4 As String
        Private strShortNameShiftBase As String

        Private intShiftColor1 As Nullable(Of Integer)
        Private intShiftColor2 As Nullable(Of Integer)
        Private intShiftColor3 As Nullable(Of Integer)
        Private intShiftColor4 As Nullable(Of Integer)

        Private intIDShiftUsed As Nullable(Of Integer)
        Private intIDShiftUsedColor As Nullable(Of Integer)
        Private strShiftUsedName As String

        Private xStartShiftUsed As DateTime

        Private oRemark As roRemark ' Observaciones (sysroRemarks)

        Private oSchedulerRemarks As Generic.List(Of Scheduler.roCalendarRemark) ' Resaltes

        Private bolLocked As Boolean

        Private bolProgrammedAbsence As Boolean
        Private bolExpectedIncidence As Boolean

        Private strProgrammedAbsencePosition As String
        Private strExpectedIncidencePosition As String

        Private intIDCause As Integer
        Private strCauseName As String
        Private xRealFinishDate As Date

        Private strPresenceDetail As String

        Private intStatus As Integer
        Private intJobStatus As Integer

        Private intIDAssignment As Nullable(Of Integer)
        Private bolIsCovered As Nullable(Of Boolean)
        Private intOldIDAssignment As Nullable(Of Integer)
        Private intCoverageIDEmployee As Nullable(Of Integer)

        Private intAssignmentColor As Nullable(Of Integer)
        Private strAssignmentName As String
        Private strAssignmentShortName As String

        Private intOldAssignmentColor As Nullable(Of Integer)
        Private strOldAssignmentName As String
        Private strOldAssignmentShortName As String
        Private strCoverageEmployeeName As String

        Private intIdAssignmentBase As Nullable(Of Integer)
        Private bolIsHolidays As Nullable(Of Boolean)

        Public Sub New()
            Me.oState = New roShiftState
            Me.intIDEmployee = -1
        End Sub

        Public Sub New(ByVal _State As roShiftState)
            Me.oState = _State
            Me.intIDEmployee = -1
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal _State As roShiftState)
            Me.oState = _State
            Me.intIDEmployee = _IDEmployee
            Me.xDate = _Date
            Me.Load()
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' ID de Empleado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property

        ''' <summary>
        ''' Fecha de Calendario
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Date_() As Date
            Get
                Return Me.xDate
            End Get
            Set(ByVal value As Date)
                Me.xDate = value
            End Set
        End Property

        ''' <summary>
        ''' Id de calendario #1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDShift1() As Nullable(Of Integer)
            Get
                Return Me.intIDShift1
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDShift1 = value
            End Set
        End Property

        ''' <summary>
        ''' Id de calendario #2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDShift2() As Nullable(Of Integer)
            Get
                Return Me.intIDShift2
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDShift2 = value
            End Set
        End Property

        ''' <summary>
        ''' Id de calendario #3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDShift3() As Nullable(Of Integer)
            Get
                Return Me.intIDShift3
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDShift3 = value
            End Set
        End Property

        ''' <summary>
        ''' Id de calendario #4
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDShift4() As Nullable(Of Integer)
            Get
                Return Me.intIDShift4
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDShift4 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del Calendario 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftName1() As String
            Get
                Return Me.strNameShift1
            End Get
            Set(ByVal value As String)
                Me.strNameShift1 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del Calendario 2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftName2() As String
            Get
                Return Me.strNameShift2
            End Get
            Set(ByVal value As String)
                Me.strNameShift2 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del Calendario 3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftName3() As String
            Get
                Return Me.strNameShift3
            End Get
            Set(ByVal value As String)
                Me.strNameShift3 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del Calendario 4
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftName4() As String
            Get
                Return Me.strNameShift4
            End Get
            Set(ByVal value As String)
                Me.strNameShift4 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre corto del Calendario 1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftShortName1() As String
            Get
                Return Me.strShortNameShift1
            End Get
            Set(ByVal value As String)
                Me.strShortNameShift1 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre corto del Calendario 2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftShortName2() As String
            Get
                Return Me.strShortNameShift2
            End Get
            Set(ByVal value As String)
                Me.strShortNameShift2 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre corto del Calendario 3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftShortName3() As String
            Get
                Return Me.strShortNameShift3
            End Get
            Set(ByVal value As String)
                Me.strShortNameShift3 = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre corto del Calendario 4
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftShortName4() As String
            Get
                Return Me.strShortNameShift4
            End Get
            Set(ByVal value As String)
                Me.strShortNameShift4 = value
            End Set
        End Property

        ''' <summary>
        ''' Color del calendario #1
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftColor1() As Nullable(Of Integer)
            Get
                Return Me.intShiftColor1
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intShiftColor1 = value
            End Set
        End Property

        ''' <summary>
        ''' Color del calendario #2
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftColor2() As Nullable(Of Integer)
            Get
                Return Me.intShiftColor2
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intShiftColor2 = value
            End Set
        End Property

        ''' <summary>
        ''' Color del calendario #3
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftColor3() As Nullable(Of Integer)
            Get
                Return Me.intShiftColor3
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intShiftColor3 = value
            End Set
        End Property

        ''' <summary>
        ''' Color del calendario #4
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftColor4() As Nullable(Of Integer)
            Get
                Return Me.intShiftColor4
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intShiftColor4 = value
            End Set
        End Property

        ''' <summary>
        ''' Calendario usado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDShiftUsed() As Nullable(Of Integer)
            Get
                Return Me.intIDShiftUsed
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDShiftUsed = value
            End Set
        End Property

        ''' <summary>
        ''' Color del calendario usado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftUsedColor() As Nullable(Of Integer)
            Get
                Return Me.intIDShiftUsedColor
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDShiftUsedColor = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del calendario usado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property ShiftUsedName() As String
            Get
                Return Me.strShiftUsedName
            End Get
            Set(ByVal value As String)
                Me.strShiftUsedName = value
            End Set
        End Property

        ''' <summary>
        ''' Observaciones
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Remarks() As roRemark
            Get
                Return Me.oRemark
            End Get
            Set(ByVal value As roRemark)
                Me.oRemark = value
            End Set
        End Property

        ''' <summary>
        ''' Estado (procesos)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Status() As Integer
            Get
                Return Me.intStatus
            End Get
            Set(ByVal value As Integer)
                Me.intStatus = value
            End Set
        End Property

        Public Property JobStatus() As Integer
            Get
                Return Me.intJobStatus
            End Get
            Set(ByVal value As Integer)
                Me.intJobStatus = value
            End Set
        End Property

        ''' <summary>
        ''' Resaltes
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property SchedulerRemarks() As Generic.List(Of Scheduler.roCalendarRemark)
            Get
                Return Me.oSchedulerRemarks
            End Get
            Set(ByVal value As Generic.List(Of Scheduler.roCalendarRemark))
                Me.oSchedulerRemarks = value
            End Set
        End Property

        ''' <summary>
        ''' Día calendario bloqueado (Por plantillas, etc.)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property Locked() As Boolean
            Get
                Return Me.bolLocked
            End Get
            Set(ByVal value As Boolean)
                Me.bolLocked = value
            End Set
        End Property

        ''' <summary>
        ''' Día con Ausencia prolongada?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InProgrammedAbsence() As Boolean
            Get
                Return Me.bolProgrammedAbsence
            End Get
            Set(ByVal value As Boolean)
                Me.bolProgrammedAbsence = value
            End Set
        End Property

        ''' <summary>
        ''' Posiciones sobre ausencia prolongada  (Dia inicial, entre, final...)
        ''' </summary>
        ''' <value></value>
        ''' <returns>Devuelve tlrb (top-left-right-bottom) 1-activo, 0-inactivo</returns>
        ''' <remarks></remarks>
        Public Property ProgrammedAbsencePosition() As String
            Get
                Return Me.strProgrammedAbsencePosition
            End Get
            Set(ByVal value As String)
                Me.strProgrammedAbsencePosition = value
            End Set
        End Property

        ''' <summary>
        ''' El día se encuentra en Incidencia?
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property InExpectedIncidence() As Boolean
            Get
                Return Me.bolExpectedIncidence
            End Get
            Set(ByVal value As Boolean)
                Me.bolExpectedIncidence = value
            End Set
        End Property

        ''' <summary>
        ''' Posicion en Incidencia (Dia inicial, entre, final...)
        ''' </summary>
        ''' <value></value>
        ''' <returns>Devuelve tlrb (top-left-right-bottom) 1-activo, 0-inactivo</returns>
        ''' <remarks></remarks>
        Public Property ExpectedIncidencePosition() As String
            Get
                Return Me.strExpectedIncidencePosition
            End Get
            Set(ByVal value As String)
                Me.strExpectedIncidencePosition = value
            End Set
        End Property

        ''' <summary>
        ''' Detalles de presencia
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property PresenceDetail() As String
            Get
                Return Me.strPresenceDetail
            End Get
            Set(ByVal value As String)
                Me.strPresenceDetail = value
            End Set
        End Property

        Public Property IDCause() As Integer
            Get
                Return Me.intIDCause
            End Get
            Set(ByVal value As Integer)
                Me.intIDCause = value
            End Set
        End Property

        Public Property CauseName() As String
            Get
                Return Me.strCauseName
            End Get
            Set(ByVal value As String)
                Me.strCauseName = value
            End Set
        End Property

        Public Property RealFinishDate() As Date
            Get
                Return Me.xRealFinishDate
            End Get
            Set(ByVal value As Date)
                Me.xRealFinishDate = value
            End Set
        End Property

        Public Property StartShift1() As DateTime
            Get
                Return Me.xStartShift1
            End Get
            Set(ByVal value As DateTime)
                Me.xStartShift1 = value
            End Set
        End Property
        Public Property StartShift2() As DateTime
            Get
                Return Me.xStartShift2
            End Get
            Set(ByVal value As DateTime)
                Me.xStartShift2 = value
            End Set
        End Property
        Public Property StartShift3() As DateTime
            Get
                Return Me.xStartShift3
            End Get
            Set(ByVal value As DateTime)
                Me.xStartShift3 = value
            End Set
        End Property
        Public Property StartShift4() As DateTime
            Get
                Return Me.xStartShift4
            End Get
            Set(ByVal value As DateTime)
                Me.xStartShift4 = value
            End Set
        End Property
        Public Property StartShiftUsed() As DateTime
            Get
                Return Me.xStartShiftUsed
            End Get
            Set(ByVal value As DateTime)
                Me.xStartShiftUsed = value
            End Set
        End Property

        ''' <summary>
        ''' Código del puesto asignado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDAssignment() As Nullable(Of Integer)
            Get
                Return Me.intIDAssignment
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDAssignment = value
            End Set
        End Property

        ''' <summary>
        ''' Indica si el puesto está o ha sido cubierto.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IsCovered() As Nullable(Of Boolean)
            Get
                Return Me.bolIsCovered
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolIsCovered = value
            End Set
        End Property

        ''' <summary>
        ''' Código del puesto asignado antes de hacer la cobertura
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property OldIDAssignment() As Nullable(Of Integer)
            Get
                Return Me.intOldIDAssignment
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intOldIDAssignment = value
            End Set
        End Property

        ''' <summary>
        ''' Código del empleado que ha realizado la cobertura o del empleado al que se le está haciendo la cobertura. <br/>
        ''' Si IsCovered es verdadero, contiene el codigo del empleado que está realizando la cobertura.<br/>
        ''' Si IsCovered es falso y OldIDAssignment no es nulo, contiener el código del empleado al que se le está realizando la cobertura
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CovereageIDEmployee() As Nullable(Of Integer)
            Get
                Return Me.intCoverageIDEmployee
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intCoverageIDEmployee = value
            End Set
        End Property

        Public Property AssignmentColor() As Nullable(Of Integer)
            Get
                Return Me.intAssignmentColor
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intAssignmentColor = value
            End Set
        End Property

        Public Property AssignmentName() As String
            Get
                Return Me.strAssignmentName
            End Get
            Set(ByVal value As String)
                Me.strAssignmentName = value
            End Set
        End Property

        Public Property AssignmentShortName() As String
            Get
                Return Me.strAssignmentShortName
            End Get
            Set(ByVal value As String)
                Me.strAssignmentShortName = value
            End Set
        End Property

        Public Property OldAssignmentColor() As Nullable(Of Integer)
            Get
                Return Me.intOldAssignmentColor
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intOldAssignmentColor = value
            End Set
        End Property

        Public Property OldAssignmentName() As String
            Get
                Return Me.strOldAssignmentName
            End Get
            Set(ByVal value As String)
                Me.strOldAssignmentName = value
            End Set
        End Property

        Public Property OldAssignmentShortName() As String
            Get
                Return Me.strOldAssignmentShortName
            End Get
            Set(ByVal value As String)
                Me.strOldAssignmentShortName = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre del empleado que ha realizado la cobertura o del empleado al que se le está haciendo la cobertura. <br/>
        ''' Si IsCovered es verdadero, contiene el nombre del empleado que está realizando la cobertura.<br/>
        ''' Si IsCovered es falso y OldIDAssignment no es nulo, contiener el nombre del empleado al que se le está realizando la cobertura.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property CoverageEmployeeName() As String
            Get
                Return Me.strCoverageEmployeeName
            End Get
            Set(ByVal value As String)
                Me.strCoverageEmployeeName = value
            End Set
        End Property

        ''' <summary>
        ''' Código del puesto Base asignado
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDShiftBase() As Nullable(Of Integer)
            Get
                Return Me.intIdShiftBase
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIdShiftBase = value
            End Set
        End Property

        Public Property StartShiftBase() As DateTime
            Get
                Return Me.xStartShiftBase
            End Get
            Set(ByVal value As DateTime)
                Me.xStartShiftBase = value
            End Set
        End Property

        Public Property ShiftShortNameBase() As String
            Get
                Return Me.strShortNameShiftBase
            End Get
            Set(ByVal value As String)
                Me.strShortNameShiftBase = value
            End Set
        End Property
        Public Property ShiftNameBase() As String
            Get
                Return Me.strNameShiftBase
            End Get
            Set(ByVal value As String)
                Me.strNameShiftBase = value
            End Set
        End Property

        ''' <summary>
        ''' Id de calendario Base
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Property IDAssignmentBase() As Nullable(Of Integer)
            Get
                Return Me.intIdAssignmentBase
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIdAssignmentBase = value
            End Set
        End Property

        Public Property IsHolidays() As Nullable(Of Boolean)
            Get
                Return Me.bolIsHolidays
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolIsHolidays = value
            End Set
        End Property

#End Region

#Region "Methods"

        Private Function SQLWhere() As String
            Return "DailySchedule.IDEmployee = " & Me.IDEmployee.ToString & " And " &
                   "DailySchedule.Date = " & Any2Time(Me.Date_).SQLSmallDateTime
        End Function

        Private Function Load() As Boolean

            Dim bolRet As Boolean = False

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# *, " &
                                                "CASE ISNULL(IsCovered,0) WHEN 0 " &
                                                    "THEN DailySchedule.IDEmployeeCovered " &
                                                    "ELSE (@SELECT# DS.IDEmployee FROM DailySchedule DS WHERE DS.Date = DailySchedule.Date AND DS.IDEmployeeCovered = DailySchedule.IDEmployee ) END  AS 'CoverageIDEmployee', " &
                                                "CASE ISNULL(IsCovered,0) WHEN 0 " &
                                                    "THEN (@SELECT# Employees.Name FROM Employees WHERE Employees.ID = DailySchedule.IDEmployeeCovered) " &
                                                    "ELSE (@SELECT# Employees.Name FROM DailySchedule DS INNER JOIN Employees ON DS.IDEmployee = Employees.ID WHERE DS.Date = DailySchedule.Date AND DS.IDEmployeeCovered = DailySchedule.IDEmployee ) END  AS 'CoverageEmployeeName' " &
                                       "FROM DailySchedule " &
                                       "WHERE " & Me.SQLWhere()
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("IDShift1")) Then Me.intIDShift1 = CInt(tb.Rows(0)("IDShift1"))
                    If Not IsDBNull(tb.Rows(0)("IDShift2")) Then Me.intIDShift2 = CInt(tb.Rows(0)("IDShift2"))
                    If Not IsDBNull(tb.Rows(0)("IDShift3")) Then Me.intIDShift3 = CInt(tb.Rows(0)("IDShift3"))
                    If Not IsDBNull(tb.Rows(0)("IDShift4")) Then Me.intIDShift4 = CInt(tb.Rows(0)("IDShift4"))
                    If Not IsDBNull(tb.Rows(0)("IDShiftUsed")) Then Me.intIDShiftUsed = CInt(tb.Rows(0)("IDShiftUsed"))
                    If Not IsDBNull(tb.Rows(0)("StartShift1")) Then Me.xStartShift1 = tb.Rows(0)("StartShift1")
                    If Not IsDBNull(tb.Rows(0)("StartShift2")) Then Me.xStartShift2 = tb.Rows(0)("StartShift2")
                    If Not IsDBNull(tb.Rows(0)("StartShift3")) Then Me.xStartShift3 = tb.Rows(0)("StartShift3")
                    If Not IsDBNull(tb.Rows(0)("StartShift4")) Then Me.xStartShift4 = tb.Rows(0)("StartShift4")
                    If Not IsDBNull(tb.Rows(0)("StartShiftUsed")) Then Me.xStartShiftUsed = tb.Rows(0)("StartShiftUsed")
                    If Not IsDBNull(tb.Rows(0)("Remarks")) Then
                        Me.oRemark = New roRemark(tb.Rows(0)("Remarks"), Me.oState)
                    Else
                        Me.oRemark = New roRemark(Me.oState)
                    End If

                    Me.intStatus = tb.Rows(0)("Status")
                    Me.intJobStatus = tb.Rows(0)("JobStatus")

                    If Not IsDBNull(tb.Rows(0)("IDAssignment")) Then Me.intIDAssignment = Any2Integer(tb.Rows(0)("IDAssignment"))
                    If Not IsDBNull(tb.Rows(0)("IsCovered")) Then Me.bolIsCovered = Any2Integer(tb.Rows(0)("IsCovered"))
                    If Not IsDBNull(tb.Rows(0)("OldIDAssignment")) Then Me.intOldIDAssignment = Any2Integer(tb.Rows(0)("OldIDAssignment"))

                    If Not IsDBNull(tb.Rows(0)("IDEmployeeCovered")) Then Me.intCoverageIDEmployee = Any2Integer(tb.Rows(0)("CoverageIDEmployee"))
                    Me.strCoverageEmployeeName = Any2String(tb.Rows(0)("CoverageEmployeeName"))

                    If Not IsDBNull(tb.Rows(0)("IDShiftBase")) Then Me.intIdShiftBase = CInt(tb.Rows(0)("IDShiftBase"))
                    If Not IsDBNull(tb.Rows(0)("StartShiftBase")) Then Me.xStartShiftBase = tb.Rows(0)("StartShiftBase")
                    If Not IsDBNull(tb.Rows(0)("IDAssignmentBase")) Then Me.intIdAssignmentBase = Any2Integer(tb.Rows(0)("IDAssignmentBase"))
                    If Not IsDBNull(tb.Rows(0)("IsHolidays")) Then Me.bolIsHolidays = CInt(tb.Rows(0)("IsHolidays"))

                End If

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailySchedule:Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailySchedule:Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save() As Boolean

            Dim bolRet As Boolean = False

            Dim rd As DbDataReader = Nothing

            Try

                Dim tb As New DataTable("DailySchedule")
                Dim strSQL As String = "@SELECT# * FROM DailySchedule WHERE " & Me.SQLWhere()
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("IDEmployee") = Me.intIDEmployee
                oRow("Date") = Me.xDate
                If Me.intIDShift1.HasValue Then oRow("IDShift1") = Me.intIDShift1.Value
                If Me.intIDShift2.HasValue Then oRow("IDShift2") = Me.intIDShift2.Value
                If Me.intIDShift3.HasValue Then oRow("IDShift3") = Me.intIDShift3.Value
                If Me.intIDShift4.HasValue Then oRow("IDShift4") = Me.intIDShift4.Value
                If Me.intIDShiftUsed.HasValue Then oRow("IDShiftUsed") = Me.intIDShiftUsed.Value
                If Me.xStartShift1 <> Nothing Then
                    oRow("StartShift1") = Me.xStartShift1
                Else
                    oRow("StartShift1") = DBNull.Value
                End If
                If Me.xStartShift2 <> Nothing Then
                    oRow("StartShift2") = Me.xStartShift1
                Else
                    oRow("StartShift2") = DBNull.Value
                End If
                If Me.xStartShift3 <> Nothing Then
                    oRow("StartShift3") = Me.xStartShift1
                Else
                    oRow("StartShift3") = DBNull.Value
                End If
                If Me.xStartShift4 <> Nothing Then
                    oRow("StartShift4") = Me.xStartShift1
                Else
                    oRow("StartShift4") = DBNull.Value
                End If
                If Me.xStartShiftUsed <> Nothing Then oRow("StartShiftUsed") = Me.xStartShiftUsed
                If Me.oRemark IsNot Nothing Then
                    Dim oldRemark = IIf(IsDBNull(oRow("Remarks")), "", oRow("Remarks").ToString)
                    Dim dateRemark = IIf(IsDBNull(oRow("Date")), "", oRow("Date").ToString)
                    Dim employeeRemark = IIf(IsDBNull(oRow("IDEmployee")), "", oRow("IDEmployee").ToString)
                    If Me.oRemark.Save(True, oldRemark, dateRemark, employeeRemark) Then
                        oRow("Remarks") = Me.oRemark.ID
                    End If
                End If
                oRow("Status") = Me.intStatus
                oRow("JobStatus") = Me.intJobStatus

                If Me.intIdShiftBase.HasValue Then oRow("IDShiftBase") = Me.intIdShiftBase.Value
                If Me.xStartShiftBase <> Nothing Then
                    oRow("StartShiftBase") = Me.xStartShiftBase
                Else
                    oRow("StartShiftBase") = DBNull.Value
                End If
                If Me.intIdAssignmentBase.HasValue Then oRow("IDAssignmentBase") = Me.intIdAssignmentBase.Value
                If Me.bolIsHolidays.HasValue Then oRow("IsHolidays") = Me.bolIsHolidays.Value

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailySchedule:Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailySchedule:Save")
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim sSql As String = "@DELETE# FROM DailySchedule WHERE " & Me.SQLWhere()
                bolRet = ExecuteSql(sSql)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailySchedule::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailySchedule::Delete")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

    Public Class roDailyScheduleList

#Region "Declarations - Constructors"

        Private oState As roShiftState

        Private Items As ArrayList

        Public Sub New()

            Me.oState = New roShiftState
            Me.Items = New ArrayList

        End Sub

        Public Sub New(ByVal _State As roShiftState)

            Me.oState = _State
            Me.Items = New ArrayList

        End Sub

#End Region

#Region "Properties"

        <XmlArray("DailySchedule"), XmlArrayItem("roDailySchedule", GetType(roDailySchedule))>
        Public Property DailySchedule() As ArrayList
            Get
                Return Me.Items
            End Get
            Set(ByVal value As ArrayList)
                Me.Items = value
            End Set
        End Property

        Public ReadOnly Property State() As roShiftState
            Get
                Return Me.oState
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function Save() As Boolean

            Dim bolRet As Boolean = False
            Try
                For Each oSchedule As roDailySchedule In Me.DailySchedule
                    bolRet = oSchedule.Save
                    If Not bolRet Then
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roDailyScheduleList::Save")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal tbDailySchedule As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oDailySchedule As roDailySchedule

                For Each oRow As DataRow In tbDailySchedule.Rows

                    Select Case oRow.RowState
                        Case DataRowState.Added, DataRowState.Modified
                            oDailySchedule = New roDailySchedule(oRow("IDEmployee"), oRow("Date"), Me.oState)
                            With oDailySchedule
                                If Not IsDBNull(oRow("IDShift1")) Then .IDShift1 = oRow("IDShift1")
                                If Not IsDBNull(oRow("IDShift2")) Then .IDShift2 = oRow("IDShift2")
                                If Not IsDBNull(oRow("IDShift3")) Then .IDShift3 = oRow("IDShift3")
                                If Not IsDBNull(oRow("IDShift4")) Then .IDShift4 = oRow("IDShift4")
                                If Not IsDBNull(oRow("IDShiftUsed")) Then .IDShiftUsed = oRow("IDShiftUsed")
                                If Not IsDBNull(oRow("IDShiftBase")) Then .IDShiftBase = oRow("IDShiftBase")
                                .Status = oRow("Status")
                                .Status = oRow("JobStatus")
                            End With
                            bolRet = oDailySchedule.Save

                        Case DataRowState.Deleted
                            oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                            oDailySchedule = New roDailySchedule(oRow("IDEmployee"), oRow("Date"), Me.oState)
                            bolRet = oDailySchedule.Delete

                        Case Else
                            bolRet = True

                    End Select

                    If Not bolRet Then

                        Exit For
                    End If
                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roDailyScheduleList::Save")
            End Try

            Return bolRet

        End Function

        Public Function LoadData(ByVal ds As DataSet, ByRef oState As roShiftState) As Boolean

            Dim bolRet As Boolean = False

            Try

                If ds.Tables.Contains("DailySchedule") Then

                    Dim tb As DataTable = ds.Tables("DailySchedule")
                    Dim oDailySchedule As roDailySchedule

                    For Each oRow As DataRow In tb.Rows
                        oDailySchedule = New roDailySchedule(Any2Integer(oRow("IDEmployee")),
                                                             oRow("Date"), oState)
                        Me.DailySchedule.Add(oDailySchedule)
                    Next

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDailyScheduleList:LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyScheduleList:LoadData")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace