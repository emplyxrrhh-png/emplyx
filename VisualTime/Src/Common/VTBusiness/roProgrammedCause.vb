Imports System.Data.Common
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Incidence

    <Serializable>
    <DataContract()>
    Public Class roProgrammedCause
        Inherits Forecast.roForecast

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roProgrammedCauseState

        Private intIDCause As Nullable(Of Integer)
        Private intIDEmployee As Nullable(Of Integer)
        Private xProgrammedDate As Nullable(Of DateTime)
        Private xBeginTime As Nullable(Of DateTime)
        Private xBeginTimeOriginal As Nullable(Of DateTime)
        Private xEndTime As Nullable(Of DateTime)
        Private xEndTimeOriginal As Nullable(Of DateTime)
        Private dblDuration As Double
        Private dblDurationOriginal As Double
        Private strDescription As String
        Private bIsWin32 As Boolean

        Private intIDEmployeeOriginal As Nullable(Of Integer)
        Private xProgrammedDateOriginal As Nullable(Of DateTime)

        Private intID As Nullable(Of Integer)
        Private intIDOriginal As Nullable(Of Integer)
        Private xProgrammedEndDate As Nullable(Of DateTime)
        Private xProgrammedEndDateOriginal As Nullable(Of DateTime)
        Private dblMinDuration As Double
        Private dblMinDurationOriginal As Double

        Private intRequestId As Nullable(Of Integer)

        Private intAbsenceID As Integer

        Public Sub New()
            Me.oState = New roProgrammedCauseState
            Me.IsWin32 = False
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _State As roProgrammedCauseState)
            Me.oState = _State
            Me.IDEmployee = _IDEmployee
            Me.ProgrammedDate = _BeginDate
            Me.intID = GetNextId(_IDEmployee, _BeginDate)
            Me.intAbsenceID = -1
            Me.IsWin32 = False
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _ID As Integer, ByVal _State As roProgrammedCauseState)
            Me.oState = _State
            Me.intID = _ID
            Me.IDEmployee = _IDEmployee
            Me.ProgrammedDate = _BeginDate
            Me.IsWin32 = False
            Me.intAbsenceID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roProgrammedCauseState)
            Me.intAbsenceID = _ID
            Me.IsWin32 = False
            Me.Load(_ID)

        End Sub

#End Region

#Region "Properties"

        <DataMember()>
        Public Property AbsenceID() As Integer
            Get
                Return intAbsenceID
            End Get
            Set(ByVal value As Integer)
                intAbsenceID = value
            End Set
        End Property

        <DataMember()>
        Public Property RequestId() As Nullable(Of Integer)
            Get
                Return intRequestId
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intRequestId = value
            End Set
        End Property

        <DataMember()>
        Public Overrides Property IDCause() As Nullable(Of Integer)
            Get
                Return intIDCause
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDCause = value
            End Set
        End Property

        <DataMember()>
        Public Property IDEmployee() As Nullable(Of Integer)
            Get
                Return intIDEmployee
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property ProgrammedDate() As Nullable(Of DateTime)
            Get
                Return xProgrammedDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xProgrammedDate = value
            End Set
        End Property

        <DataMember()>
        Public Property BeginTime() As Nullable(Of DateTime)
            Get
                Return xBeginTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xBeginTime = value
            End Set
        End Property

        <DataMember()>
        Public Property BeginTimeOriginal() As Nullable(Of DateTime)
            Get
                Return xBeginTimeOriginal
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xBeginTimeOriginal = value
            End Set
        End Property

        <DataMember()>
        Public Property EndTime() As Nullable(Of DateTime)
            Get
                Return xEndTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xEndTime = value
            End Set
        End Property

        <DataMember()>
        Public Property EndTimeOriginal() As Nullable(Of DateTime)
            Get
                Return xEndTimeOriginal
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xEndTimeOriginal = value
            End Set
        End Property

        <DataMember()>
        Public Property Duration() As Double
            Get
                Return dblDuration
            End Get
            Set(ByVal value As Double)
                dblDuration = value
            End Set
        End Property

        <DataMember()>
        Public Property DurationOriginal() As Double
            Get
                Return dblDurationOriginal
            End Get
            Set(ByVal value As Double)
                dblDurationOriginal = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        <IgnoreDataMember()>
        <XmlIgnore()>
        Public Property State() As roProgrammedCauseState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roProgrammedCauseState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDEmployeeOriginal() As Nullable(Of Integer)
            Get
                Return intIDEmployeeOriginal
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDEmployeeOriginal = value
            End Set
        End Property

        <DataMember()>
        Public Property ProgrammedDateOriginal() As Nullable(Of DateTime)
            Get
                Return xProgrammedDateOriginal
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xProgrammedDateOriginal = value
            End Set
        End Property
        <DataMember()>
        Public Property IsWin32() As Boolean
            Get
                Return bIsWin32
            End Get
            Set(ByVal value As Boolean)
                bIsWin32 = value
            End Set
        End Property
        <DataMember()>
        Public Property ID() As Nullable(Of Integer)
            Get
                Return intID
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intID = value
            End Set
        End Property
        <DataMember()>
        Public Property IDOriginal() As Nullable(Of Integer)
            Get
                Return intIDOriginal
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDOriginal = value
            End Set
        End Property
        <DataMember()>
        Public Property ProgrammedEndDateOriginal() As Nullable(Of DateTime)
            Get
                Return xProgrammedEndDateOriginal
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xProgrammedEndDateOriginal = value
            End Set
        End Property
        <DataMember()>
        Public Property ProgrammedEndDate() As Nullable(Of DateTime)
            Get
                Return xProgrammedEndDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xProgrammedEndDate = value
            End Set
        End Property
        <DataMember()>
        Public Property MinDuration() As Double
            Get
                Return dblMinDuration
            End Get
            Set(ByVal value As Double)
                dblMinDuration = value
            End Set
        End Property
        <DataMember()>
        Public Property MinDurationOriginal() As Double
            Get
                Return dblMinDurationOriginal
            End Get
            Set(ByVal value As Double)
                dblMinDurationOriginal = value
            End Set
        End Property

        ''' Propiedades para poder derivar de la clase de previsiones genéricas roForecast
        <DataMember()>
        Public Overrides Property BeginDate() As Nullable(Of DateTime)
            Get
                Return xProgrammedDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xProgrammedDate = value
            End Set
        End Property

        <DataMember()>
        Public Overrides Property FinishDate() As Nullable(Of DateTime)
            Get
                Return xProgrammedEndDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xProgrammedEndDate = value
            End Set
        End Property

        <DataMember()>
        Public Overrides Property MaxLastingDays() As Integer
            Get
                Return 0
            End Get
            Set(ByVal value As Integer)

            End Set
        End Property

        <DataMember()>
        Public Overrides Property RealFinishDate() As DateTime
            Get
                If Me.FinishDate.HasValue Then
                    Return Me.FinishDate.Value
                Else
                    Return Me.BeginDate.Value.AddDays(Me.MaxLastingDays - 1)
                End If
            End Get
            Set(value As DateTime)

            End Set

        End Property

#End Region

#Region "Methods"

        Private Function SQLWhere() As String

            Dim strRet As String = ""
            strRet = "ProgrammedCauses.IDEmployee = "
            If Me.IDEmployeeOriginal.HasValue Then
                strRet &= Me.IDEmployeeOriginal.Value & " AND "
            Else
                strRet &= Me.IDEmployee.Value & " AND "
            End If
            strRet &= "ProgrammedCauses.Date = "
            If Me.ProgrammedDateOriginal.HasValue Then
                strRet &= roTypes.Any2Time(Me.ProgrammedDateOriginal.Value).SQLSmallDateTime & " AND "
            Else
                strRet &= roTypes.Any2Time(Me.ProgrammedDate.Value).SQLSmallDateTime & " AND "
            End If

            strRet &= "ProgrammedCauses.ID = "
            If Me.IDOriginal.HasValue Then
                strRet &= Me.IDOriginal.Value
            Else
                If Me.ID.HasValue Then
                    strRet &= Me.ID.Value
                Else
                    strRet &= -1
                End If
            End If

            Return strRet

        End Function

        Private Function SQLDateWhere() As String

            Dim strRet As String = ""
            strRet = "ProgrammedCauses.IDEmployee = "
            If Me.IDEmployeeOriginal.HasValue Then
                strRet &= Me.IDEmployeeOriginal.Value & " AND "
            Else
                strRet &= Me.IDEmployee.Value & " AND "
            End If
            strRet &= "ProgrammedCauses.Date = "
            If Me.ProgrammedDateOriginal.HasValue Then
                strRet &= roTypes.Any2Time(Me.ProgrammedDateOriginal.Value).SQLSmallDateTime & " AND "
            Else
                strRet &= roTypes.Any2Time(Me.ProgrammedDate.Value).SQLSmallDateTime & " AND "
            End If

            strRet &= "ProgrammedCauses.BeginTime = "
            If Me.BeginTimeOriginal.HasValue Then
                strRet &= roTypes.Any2Time(Me.BeginTimeOriginal.Value).SQLDateTime
            Else
                strRet &= roTypes.Any2Time(Me.BeginTime.Value).SQLDateTime
            End If

            Return strRet

        End Function

        Public Overrides Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Me.IDEmployeeOriginal = Me.IDEmployee
            Me.ProgrammedDateOriginal = Me.ProgrammedDate

            Me.IDCause = Nothing
            Me.BeginTime = Nothing
            Me.EndTime = Nothing
            Me.Duration = 0
            Me.Description = ""
            Me.AbsenceID = -1

            If Me.IDEmployee.HasValue And Me.ProgrammedDate.HasValue And Me.ID.HasValue Then
                Try
                    Dim strSQL As String = "@SELECT# * FROM ProgrammedCauses " &
                                           "WHERE " & Me.SQLWhere()
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        If Not IsDBNull(tb.Rows(0)("IDCause")) Then Me.IDCause = CInt(tb.Rows(0)("IDCause"))

                        If Not IsDBNull(tb.Rows(0)("AbsenceID")) Then Me.AbsenceID = CInt(tb.Rows(0)("AbsenceID"))

                        If Not IsDBNull(tb.Rows(0)("BeginTime")) Then
                            Me.BeginTime = tb.Rows(0)("BeginTime")
                            Me.BeginTimeOriginal = tb.Rows(0)("BeginTime")
                        End If

                        If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                            If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                                Me.ProgrammedEndDate = tb.Rows(0)("FinishDate")
                                Me.ProgrammedEndDateOriginal = tb.Rows(0)("FinishDate")
                            End If
                        End If

                        If Not IsDBNull(tb.Rows(0)("EndTime")) Then
                            Me.EndTime = tb.Rows(0)("EndTime")
                            Me.EndTimeOriginal = tb.Rows(0)("EndTime")
                        End If

                        If Not IsDBNull(tb.Rows(0)("Duration")) Then
                            Me.Duration = tb.Rows(0)("Duration")
                            Me.DurationOriginal = tb.Rows(0)("Duration")
                        End If

                        If Not IsDBNull(tb.Rows(0)("MinDuration")) Then
                            Me.MinDuration = tb.Rows(0)("MinDuration")
                            Me.MinDurationOriginal = tb.Rows(0)("MinDuration")
                        End If

                        If Not IsDBNull(tb.Rows(0)("Description")) Then Me.Description = tb.Rows(0)("Description")

                        If Not IsDBNull(tb.Rows(0)("Win32")) Then Me.IsWin32 = tb.Rows(0)("Win32")
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDEmployee}", Me.IDEmployee, "", 1)
                        oState.AddAuditParameter(tbParameters, "{IDCause}", Me.IDCause, "", 1)
                        oState.AddAuditParameter(tbParameters, "{Date}", Me.ProgrammedDate, "", 1)

                        Dim oEmpState As New Employee.roEmployeeState()
                        roBusinessState.CopyTo(Me.oState, oEmpState)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedCause, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbParameters, -1)
                    End If

                    bolRet = True
                Catch ex As Data.Common.DbException
                    oState.UpdateStateInfo(ex, "roProgrammedCause::Load")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roProgrammedCause::Load")
                End Try

            End If

            Return bolRet

        End Function

        Public Overloads Function Load(_ID As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Me.IDEmployeeOriginal = Me.IDEmployee
            Me.ProgrammedDateOriginal = Me.ProgrammedDate

            Me.IDCause = Nothing
            Me.BeginTime = Nothing
            Me.EndTime = Nothing
            Me.Duration = 0
            Me.Description = ""
            Me.AbsenceID = _ID

            Try

                Dim strSQL As String = "@SELECT# * FROM ProgrammedCauses " &
                                           "WHERE AbsenceID =  " & Me.AbsenceID
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0)("IDCause")) Then Me.IDCause = CInt(tb.Rows(0)("IDCause"))
                    If Not IsDBNull(tb.Rows(0)("IDEmployee")) Then Me.IDEmployee = CInt(tb.Rows(0)("IDEmployee"))
                    If Not IsDBNull(tb.Rows(0)("ID")) Then Me.ID = CInt(tb.Rows(0)("ID"))
                    If Not IsDBNull(tb.Rows(0)("AbsenceID")) Then Me.AbsenceID = CInt(tb.Rows(0)("AbsenceID"))
                    If Not IsDBNull(tb.Rows(0)("RequestId")) Then Me.RequestId = CInt(tb.Rows(0)("RequestId"))
                    If Not IsDBNull(tb.Rows(0)("Date")) Then Me.ProgrammedDate = tb.Rows(0)("Date")
                    If Not IsDBNull(tb.Rows(0)("BeginTime")) Then
                        Me.BeginTime = tb.Rows(0)("BeginTime")
                        Me.BeginTimeOriginal = tb.Rows(0)("BeginTime")
                    End If

                    If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                        If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                            Me.ProgrammedEndDate = tb.Rows(0)("FinishDate")
                            Me.ProgrammedEndDateOriginal = tb.Rows(0)("FinishDate")
                        End If
                    End If

                    If Not IsDBNull(tb.Rows(0)("EndTime")) Then
                        Me.EndTime = tb.Rows(0)("EndTime")
                        Me.EndTimeOriginal = tb.Rows(0)("EndTime")
                    End If

                    If Not IsDBNull(tb.Rows(0)("Duration")) Then
                        Me.Duration = tb.Rows(0)("Duration")
                        Me.DurationOriginal = tb.Rows(0)("Duration")
                    End If

                    If Not IsDBNull(tb.Rows(0)("MinDuration")) Then
                        Me.MinDuration = tb.Rows(0)("MinDuration")
                        Me.MinDurationOriginal = tb.Rows(0)("MinDuration")
                    End If

                    If Not IsDBNull(tb.Rows(0)("Description")) Then Me.Description = tb.Rows(0)("Description")

                    If Not IsDBNull(tb.Rows(0)("Win32")) Then Me.IsWin32 = tb.Rows(0)("Win32")
                End If

                If bAudit Then
                    ' Auditar lectura
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{IDEmployee}", Me.IDEmployee, "", 1)
                    oState.AddAuditParameter(tbParameters, "{IDCause}", Me.IDCause, "", 1)
                    oState.AddAuditParameter(tbParameters, "{Date}", Me.ProgrammedDate, "", 1)

                    Dim oEmpState As New Employee.roEmployeeState()
                    roBusinessState.CopyTo(Me.oState, oEmpState)

                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedCause, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roProgrammedCause::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedCause::Load")
            End Try

            Return bolRet

        End Function

        Public Function LoadByStartDate(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Me.IDEmployeeOriginal = Me.IDEmployee
            Me.ProgrammedDateOriginal = Me.ProgrammedDate

            Me.IDCause = Nothing
            'Me.BeginTime = Nothing
            Me.EndTime = Nothing
            Me.Duration = 0
            Me.Description = ""
            Me.ID = -1

            If Me.IDEmployee.HasValue And Me.ProgrammedDate.HasValue And Me.BeginTime.HasValue Then

                Try

                    Dim strSQL As String = "@SELECT# * FROM ProgrammedCauses " &
                                           "WHERE " & Me.SQLDateWhere()

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        If Not IsDBNull(tb.Rows(0)("IDCause")) Then Me.IDCause = CInt(tb.Rows(0)("IDCause"))

                        If Not IsDBNull(tb.Rows(0)("AbsenceID")) Then Me.AbsenceID = CInt(tb.Rows(0)("AbsenceID"))

                        If Not IsDBNull(tb.Rows(0)("BeginTime")) Then
                            Me.BeginTime = tb.Rows(0)("BeginTime")
                            Me.BeginTimeOriginal = tb.Rows(0)("BeginTime")
                        End If

                        If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                            If Not IsDBNull(tb.Rows(0)("FinishDate")) Then
                                Me.ProgrammedEndDate = tb.Rows(0)("FinishDate")
                                Me.ProgrammedEndDateOriginal = tb.Rows(0)("FinishDate")
                            End If
                        End If

                        If Not IsDBNull(tb.Rows(0)("EndTime")) Then
                            Me.EndTime = tb.Rows(0)("EndTime")
                            Me.EndTimeOriginal = tb.Rows(0)("EndTime")
                        End If

                        If Not IsDBNull(tb.Rows(0)("Duration")) Then
                            Me.Duration = tb.Rows(0)("Duration")
                            Me.DurationOriginal = tb.Rows(0)("Duration")
                        End If

                        If Not IsDBNull(tb.Rows(0)("MinDuration")) Then
                            Me.MinDuration = tb.Rows(0)("MinDuration")
                            Me.MinDurationOriginal = tb.Rows(0)("MinDuration")
                        End If

                        If Not IsDBNull(tb.Rows(0)("Description")) Then Me.Description = tb.Rows(0)("Description")

                        If Not IsDBNull(tb.Rows(0)("Win32")) Then Me.IsWin32 = tb.Rows(0)("Win32")

                        If Not IsDBNull(tb.Rows(0)("ID")) Then Me.ID = tb.Rows(0)("ID")
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDEmployee}", Me.IDEmployee, "", 1)
                        oState.AddAuditParameter(tbParameters, "{IDCause}", Me.IDCause, "", 1)
                        oState.AddAuditParameter(tbParameters, "{Date}", Me.ProgrammedDate, "", 1)

                        Dim oEmpState As New Employee.roEmployeeState()
                        roBusinessState.CopyTo(Me.oState, oEmpState)

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedCause, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbParameters, -1)
                    End If

                    bolRet = True
                Catch ex As Data.Common.DbException
                    oState.UpdateStateInfo(ex, "roProgrammedCause::Load")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roProgrammedCause::Load")
                Finally

                End Try

            End If

            Return bolRet

        End Function

        Public Overrides Function Save(Optional ByVal bAudit As Boolean = False, Optional CreateTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = ProgrammedCausesResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateProgrammedCause(Me, Me.State) Then

                    Dim tb As New DataTable("ProgrammedCauses")
                    Dim strSQL As String = "@SELECT# * FROM ProgrammedCauses WHERE " & Me.SQLWhere()
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim _ProgrammedDate As Date = Me.ProgrammedDate.Value
                    Dim _ProgrammedEndDate As Date
                    If Me.ProgrammedEndDate.HasValue Then
                        _ProgrammedEndDate = Me.ProgrammedEndDate
                    Else
                        _ProgrammedEndDate = Me.ProgrammedDate
                    End If

                    Dim _BeginTime As Date = Me.BeginTime.Value
                    Dim _EndTime As Date = Me.EndTime.Value

                    Dim _IDCauseOld As Integer = Me.IDCause.Value

                    Dim _Duration As Double = Me.Duration
                    Dim _MinDuration As Double = Me.MinDuration

                    Dim _FreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee.Value, False, Me.oState)

                    Dim bolLaunchUpdate As Boolean = True

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oRow As DataRow
                    Dim _ProgrammedEndDateOld As Date = Nothing

                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bolIsNew = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        Dim _ProgrammedDateOld As Date = oRow("Date")

                        If IsDBNull(oRow("FinishDate")) Then
                            _ProgrammedEndDateOld = _ProgrammedDateOld
                        Else
                            _ProgrammedEndDateOld = oRow("FinishDate")
                        End If

                        Dim _BeginTimeOld As Date = oRow("BeginTime")
                        Dim _EndTimeOld As Date = oRow("EndTime")
                        Dim _DurationOld As Double = oRow("Duration")
                        Dim _MinDurationOld As Double = 0
                        If IsDBNull(oRow("MinDuration")) Then
                            _MinDurationOld = 0
                        Else
                            _MinDurationOld = oRow("MinDuration")
                        End If

                        _IDCauseOld = oRow("IDCause")

                        'Si todos los datos coinciden, no lanzamos recalculo
                        If _ProgrammedDate = _ProgrammedDateOld And _BeginTime = _BeginTimeOld And _IDCauseOld = Me.IDCause.Value And
                            _EndTimeOld = _EndTime And _DurationOld = _Duration And _ProgrammedEndDate = _ProgrammedEndDateOld And _MinDuration = _MinDurationOld Then
                            bolLaunchUpdate = False
                        End If

                        _ProgrammedDate = IIf(_ProgrammedDateOld < _ProgrammedDate, _ProgrammedDateOld, _ProgrammedDate)
                        _ProgrammedEndDate = IIf(_ProgrammedEndDateOld > _ProgrammedEndDate, _ProgrammedEndDateOld, _ProgrammedEndDate)

                        'Fecha de congelación
                        _ProgrammedDate = IIf(_ProgrammedDate <= _FreezeDate, _FreezeDate, _ProgrammedDate)
                        _ProgrammedDateOld = IIf(_ProgrammedDateOld <= _FreezeDate, _FreezeDate, _ProgrammedDateOld)
                    End If

                    oRow("IDCause") = Me.IDCause.Value
                    oRow("IDEmployee") = Me.IDEmployee.Value
                    If Me.RequestId.HasValue Then
                        oRow("RequestId") = Me.RequestId.Value
                    Else
                        oRow("RequestId") = DBNull.Value
                    End If

                    oRow("Date") = Me.ProgrammedDate.Value

                    Dim _ActualFinishDate As Date = Me.ProgrammedDate
                    If Me.ProgrammedEndDate.HasValue Then
                        _ActualFinishDate = Me.ProgrammedEndDate

                        oRow("FinishDate") = Me.ProgrammedEndDate.Value
                    Else
                        oRow("FinishDate") = DBNull.Value
                    End If

                    If Me.BeginTime.HasValue Then
                        oRow("BeginTime") = Me.BeginTime.Value
                    Else
                        oRow("BeginTime") = DBNull.Value
                    End If
                    If Me.EndTime.HasValue Then
                        oRow("EndTime") = Me.EndTime.Value
                    Else
                        oRow("EndTime") = DBNull.Value
                    End If

                    oRow("Duration") = Me.Duration
                    oRow("MinDuration") = Me.MinDuration

                    oRow("Description") = Me.Description
                    oRow("Win32") = Me.IsWin32

                    If Me.intID.HasValue AndAlso Me.intID.Value >= 0 Then
                        oRow("ID") = Me.intID
                    Else
                        oRow("ID") = GetNextId(Me.IDEmployee, Me.ProgrammedDate)
                        Me.intID = oRow("ID")
                    End If

                    oRow("Timestamp") = Now

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    If bolIsNew Then
                        Dim tmpId As DataTable = CreateDataTable("@SELECT# TOP 1 [AbsenceID] FROM ProgrammedCauses WHERE IDEmployee=" & Me.intIDEmployee.ToString & " AND IDCause= " & Me.intIDCause.ToString & " ORDER BY [AbsenceID] DESC")

                        If tmpId IsNot Nothing AndAlso tmpId.Rows.Count = 1 Then
                            Me.intAbsenceID = tmpId.Rows(0)("AbsenceID")
                            oRow("AbsenceID") = Me.intAbsenceID
                        End If
                    End If

                    ' Auditamos
                    If bolRet And bAudit Then
                        oAuditDataNew = oRow
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                        Dim oEmpState As New Employee.roEmployeeState()
                        roBusinessState.CopyTo(Me.oState, oEmpState)

                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tProgrammedCause, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), tbAuditParameters, -1)
                    End If

                    ' Notificamos cambio al servidor
                    If bolRet And bolLaunchUpdate Then

                        Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("VerifyProgrammedCauseOnMandatory", New AdvancedParameter.roAdvancedParameterState())

                        If oAdvParam.Value.ToUpper = "1" Then
                            'Actualiza la table de DailySchedule del dia anterior
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = '' WHERE Status > 45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date = " & roTypes.Any2Time(_ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(ProgrammedDate.Value).Add(-1, "d").SQLSmallDateTime & ")"
                            ExecuteSqlWithoutTimeOut(strSQL)

                            'Actualiza la table de DailySchedule del dia posterior
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = '' WHERE Status > 45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date = " & roTypes.Any2Time(_ProgrammedDate).Add(1, "d").SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(ProgrammedDate.Value).Add(1, "d").SQLSmallDateTime & ")"
                            ExecuteSqlWithoutTimeOut(strSQL)

                            'Actualiza la table de DailySchedule
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 45, [GUID] = '' " &
                                     "WHERE Status>45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date >= " & roTypes.Any2Time(_ProgrammedDate).SQLSmallDateTime & " OR Date >= " & roTypes.Any2Time(ProgrammedDate.Value).SQLSmallDateTime & ") AND " &
                                     "(Date <= " & roTypes.Any2Time(_ProgrammedEndDate).SQLSmallDateTime & " OR Date <= " & roTypes.Any2Time(ProgrammedEndDate.Value).SQLSmallDateTime & ")"
                            ExecuteSqlWithoutTimeOut(strSQL)
                        Else
                            'Actualiza la table de DailySchedule del dia anterior
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 55, [GUID] = '' WHERE Status > 55 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date = " & roTypes.Any2Time(_ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(ProgrammedDate.Value).Add(-1, "d").SQLSmallDateTime & ")"
                            ExecuteSqlWithoutTimeOut(strSQL)

                            'Actualiza la table de DailySchedule del dia posterior
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 55, [GUID] = '' WHERE Status > 55 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date = " & roTypes.Any2Time(_ProgrammedDate).Add(1, "d").SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(ProgrammedDate.Value).Add(1, "d").SQLSmallDateTime & ")"
                            ExecuteSqlWithoutTimeOut(strSQL)

                            'Actualiza la table de DailySchedule
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 55, [GUID] = '' " &
                                     "WHERE Status>55 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                     "(Date >= " & roTypes.Any2Time(_ProgrammedDate).SQLSmallDateTime & " OR Date >= " & roTypes.Any2Time(ProgrammedDate.Value).SQLSmallDateTime & ") AND " &
                                     "(Date <= " & roTypes.Any2Time(_ProgrammedEndDate).SQLSmallDateTime & " OR Date <= " & roTypes.Any2Time(ProgrammedEndDate.Value).SQLSmallDateTime & ")"
                            ExecuteSqlWithoutTimeOut(strSQL)

                        End If

                        'Elimina las Causas
                        strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                 "(Date >= " & roTypes.Any2Time(_ProgrammedDate).SQLSmallDateTime & " OR Date >= " & roTypes.Any2Time(ProgrammedDate.Value).SQLSmallDateTime & ")" & " AND " &
                                 "(Date <= " & roTypes.Any2Time(_ProgrammedEndDate).SQLSmallDateTime & " OR Date <= " & roTypes.Any2Time(ProgrammedEndDate.Value).SQLSmallDateTime & ") AND " &
                                 "( IDCause = " & _IDCauseOld & " OR IDCause = " & Me.IDCause.Value & ")"
                        ExecuteSqlWithoutTimeOut(strSQL)

                        'Elimina las Causas
                        strSQL = "@DELETE# DailyCauses " &
                                 "WHERE IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                       "(Date = " & roTypes.Any2Time(_ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(ProgrammedDate.Value).Add(-1, "d").SQLSmallDateTime & ")" & " AND " &
                                       "( IDCause = " & _IDCauseOld & " OR IDCause = " & Me.IDCause.Value & ")"
                        ExecuteSqlWithoutTimeOut(strSQL)

                        strSQL = "@DELETE# DailyCauses " &
                                 "WHERE IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                       "(Date = " & roTypes.Any2Time(_ProgrammedDate).Add(1, "d").SQLSmallDateTime & " OR Date = " & roTypes.Any2Time(ProgrammedDate.Value).Add(1, "d").SQLSmallDateTime & ")" & " AND " &
                                       "( IDCause = " & _IDCauseOld & " OR IDCause = " & Me.IDCause.Value & ")"
                        ExecuteSqlWithoutTimeOut(strSQL)

                        ' Borramos notificaciones de empleado ausente dentro del periodo de la ausencia prevista
                        strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (15,13)) AND Key1Numeric = " & Me.IDEmployee.Value & " AND Key3DateTime >= " & roTypes.Any2Time(Me.ProgrammedDate.Value).SQLDateTime & " AND Key3DateTime <= " & roTypes.Any2Time(Me.ProgrammedEndDate.Value.AddDays(1).AddMinutes(-1)).SQLDateTime
                        ExecuteSqlWithoutTimeOut(strSQL)

                        ' Crea la tarea de Ausencias programadas
                        If CreateTask Then
                            Dim oContext As New roCollection
                            oContext.Add("User.ID", Me.IDEmployee.Value)
                            oContext.Add("Date", Now.Date)
                            If oAdvParam.Value.ToUpper = "1" Then
                                roConnector.InitTask(TasksType.MOVES, oContext)
                            Else
                                roConnector.InitTask(TasksType.PROGRAMMEDABSENCES, oContext)
                            End If
                        End If

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedCause:Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedCause:Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Overrides Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim tb As New DataTable("ProgrammedCauses")
                Dim strSQL As String = "@SELECT# * FROM ProgrammedCauses WHERE " & Me.SQLWhere()
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb.Rows.Count > 0 Then

                    If Not IsDBNull(tb.Rows(0).Item("IDCause")) Then Me.IDCause = CInt(tb.Rows(0).Item("IDCause"))
                    If Not IsDBNull(tb.Rows(0).Item("BeginTime")) Then Me.BeginTime = tb.Rows(0).Item("BeginTime")
                    If Not IsDBNull(tb.Rows(0).Item("EndTime")) Then Me.BeginTime = tb.Rows(0).Item("EndTime")
                    If Not IsDBNull(tb.Rows(0).Item("Duration")) Then Me.Duration = tb.Rows(0).Item("Duration")
                    If Not IsDBNull(tb.Rows(0).Item("MinDuration")) Then Me.Duration = tb.Rows(0).Item("MinDuration")
                    If Not IsDBNull(tb.Rows(0).Item("FinishDate")) Then Me.ProgrammedEndDate = tb.Rows(0).Item("FinishDate")
                    If Not IsDBNull(tb.Rows(0).Item("AbsenceId")) Then Me.AbsenceID = tb.Rows(0).Item("AbsenceId")

                    tb.Rows(0).Delete()

                    da.Update(tb)

                    bolRet = True
                End If

                'Comprovem si es troba en periode de congelació
                If bolRet Then
                    Dim freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee.Value, False, Me.oState)
                    If Me.ProgrammedDate.Value <= freezeDate Then
                        Me.State.Result = ProgrammedCausesResultEnum.InFreezeDate
                        bolRet = False
                    End If
                End If

                Dim _ProgrammedDate As Date = Me.ProgrammedDate.Value
                Dim _ProgrammedEndDate As Date
                If Me.ProgrammedEndDate.HasValue Then
                    _ProgrammedEndDate = Me.ProgrammedEndDate
                Else
                    _ProgrammedEndDate = Me.ProgrammedDate
                End If

                If bolRet Then
                    ' En el caso de que tenga el parametro avanzado de no permitir gestionar justificaciones que
                    ' no esten dentro de sus grupos de negocio, lo validamos
                    If roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("BusinessGroup.ApplyNotAllowedModifyCause", New AdvancedParameter.roAdvancedParameterState).Value) = "1" Then
                        Dim oCause As New Cause.roCause(-1, New Cause.roCauseState(Me.State.IDPassport))
                        Dim tbCauses As DataTable = oCause.Causes("", True)

                        If tbCauses IsNot Nothing Then
                            Dim oRows() As DataRow = tbCauses.Select("ID = " & Me.IDCause)
                            If oRows IsNot Nothing AndAlso oRows.Length = 0 Then
                                Me.State.Result = ProgrammedCausesResultEnum.NotAllowedCause
                                bolRet = False
                            End If
                        End If
                    End If
                End If

                If bolRet Then
                    ' Borramos la causa de la ausencia dentro de los limites de la ausencia
                    strSQL = "@DELETE# DailyCauses " &
                             "WHERE IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                   "IDCause = " & Me.IDCause.Value & " AND " &
                                   "Date >= " & roTypes.Any2Time(_ProgrammedDate).SQLSmallDateTime() & " AND Date <= " & roTypes.Any2Time(_ProgrammedEndDate).SQLSmallDateTime()
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                End If

                'If bolRet AndAlso Me.AbsenceID > 0 Then
                '    ' Borramos los documentos de la ausencia
                '    strSQL = "@DELETE# Documents WHERE IdEmployee =  " & Me.IDEmployee.Value & " and IdHoursAbsence = " & Me.AbsenceID
                '    bolRet = ExecuteSql(strSQL)
                'End If

                Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("VerifyProgrammedCauseOnMandatory", New AdvancedParameter.roAdvancedParameterState())

                If bolRet Then
                    If oAdvParam.Value.ToUpper = "1" Then
                        ' Notificamos el borrado de la ausencia
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                 "SET Status = 40 " &
                                 "WHERE Status > 45 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                       "( Date >= " & roTypes.Any2Time(_ProgrammedDate).Add(-1, "d").SQLSmallDateTime() & " AND Date <= " & roTypes.Any2Time(_ProgrammedEndDate).Add(1, "d").SQLSmallDateTime & " AND Date <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ")"

                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    Else
                        ' Notificamos el borrado de la ausencia
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) " &
                                 "SET Status = 50 " &
                                 "WHERE Status > 55 AND IDEmployee = " & Me.IDEmployee.Value & " AND " &
                                       "( Date >= " & roTypes.Any2Time(_ProgrammedDate).Add(-1, "d").SQLSmallDateTime() & " AND Date <= " & roTypes.Any2Time(_ProgrammedEndDate).Add(1, "d").SQLSmallDateTime & " AND Date <= " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ")"

                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                    End If
                End If

                If bolRet Then
                    Try
                        strSQL = "@DELETE# sysroNotificationTasks WHERE Key1Numeric = " & Me.IDEmployee.Value & " AND Key5Numeric = " & Me.AbsenceID & " AND IDNotification = 701 AND Parameters LIKE 'HOURS%'"
                        bolRet = ExecuteSql(strSQL)
                    Catch ex As Exception
                    End Try
                End If

                If bolRet Then
                    Try
                        strSQL = "@INSERT# INTO DeletedProgrammedCauses VALUES (" & Me.IDEmployee.Value & "," & Me.AbsenceID & "," & roTypes.Any2Time(Now).SQLSmallDateTime & ")"
                        bolRet = ExecuteSql(strSQL)
                    Catch ex As Exception
                    End Try
                End If

                If bolRet And bAudit Then
                    ' Auditamos

                    Dim oEmpState As New Employee.roEmployeeState()
                    roBusinessState.CopyTo(Me.oState, oEmpState)

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProgrammedCause, roBusinessSupport.GetEmployeeName(Me.IDEmployee, oEmpState), Nothing, -1)
                End If

                Dim oContext As New roCollection
                oContext.Add("User.ID", Me.IDEmployee.Value)
                oContext.Add("Date", Now.Date)

                If oAdvParam.Value.ToUpper = "1" Then
                    roConnector.InitTask(TasksType.MOVES, oContext)
                Else
                    roConnector.InitTask(TasksType.PROGRAMMEDABSENCES, oContext)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedCause::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedCause::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function GetNextId(ByVal employeeID As Integer, ByVal xDate As DateTime) As Integer
            Dim bolRet As Boolean = True

            Dim retValue As Integer = 0

            Try

                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), -1) + 1) FROM ProgrammedCauses WHERE "
                strQry &= "ProgrammedCauses.IDEmployee = " & employeeID & " AND "
                strQry &= "ProgrammedCauses.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime

                retValue = roTypes.Any2Integer(ExecuteScalar(strQry))
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roProgrammedCause:GetNextId")
            End Try

            Return retValue
        End Function

        Public Overrides Function ToString() As String
            Dim oRet As String = String.Empty
            Try
                oRet = "Ausencia por horas del empleado " & Employee.roEmployee.GetEmployee(Me.IDEmployee, Nothing).Name & " desde el día " & Me.BeginDate.Value.ToShortDateString & " hasta el día " & Me.RealFinishDate.ToShortDateString & " por " & Cause.roCause.GetCauseNameByID(Me.IDCause)
            Catch ex As Exception
                oRet = "Error recuperando descripción de ausencia por horas"
            End Try
            Return oRet
        End Function

        Public Overrides Function SurfaceCopy() As Object
            Return Me.MemberwiseClone
        End Function

#Region "Helper methods"

        Public Shared Function GetProgrammedCauses(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _State As roProgrammedCauseState, Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String

                Dim strUniqueidentifierField As String = String.Empty
                If _IDEmployee = -1 Then
                    strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                End If

                strSQL = "@SELECT# IDCause, ProgrammedCauses.IDEmployee, ProgrammedCauses.ID, ProgrammedCauses.Date, FinishDate, BeginTime, EndTime, convert(numeric(8,6), Duration) as duration,convert(numeric(8,6), MinDuration) as minDuration, ProgrammedCauses.Description, " &
                                "ProgrammedCauses.Win32 , Causes.Name, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedCauses.AbsenceID AND adf.forecasttype='hours') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, AbsenceID, " &
                                "Timestamp, " &
                                "'Hour' AS Type,  " &
                                "'CRU' AS Action,  "
                If _IDEmployee > 0 Then
                    strSQL += " '' AS NIF, " &
                                " '' AS IdImport "
                Else
                    strSQL += " NifTable.Value AS NIF, " &
                              " IdTable.Value AS IdImport "
                End If
                strSQL += "FROM ProgrammedCauses " &
                                "LEFT JOIN Causes On Causes.ID = ProgrammedCauses.IDCause " &
                            " LEFT JOIN (@SELECT# row_number() over (partition by EmployeeUserFieldValues.IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = ProgrammedCauses.IDEmployee AND NifTable.Date < GETDATE() " &
                            " Left JOIN (@SELECT# row_number() over (partition by EmployeeUserFieldValues.IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = ProgrammedCauses.IDEmployee AND IdTable.Date < GETDATE() "

                If _IDEmployee > 0 Then
                    strSQL += " WHERE ProgrammedCauses.idEmployee = " & _IDEmployee & " "
                Else
                    strSQL += " WHERE 1=1 "
                End If
                strSQL += " AND " &
                               "((ProgrammedCauses.Date BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ") OR " &
                                "(FinishDate BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ") OR " &
                                "(ProgrammedCauses.Date < " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND FinishDate > " & roTypes.Any2Time(_EndDate).SQLSmallDateTime & ")) AND " &
                                "(NifTable.RowNumber1 = 1 or NifTable.RowNumber1 is null) and (IdTable.RowNumber1 = 1 or IdTable.RowNumber1 is null)"

                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If

                strSQL = strSQL & " ORDER BY Date DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedAbsences")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedCausesByTimestamp(ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _State As roProgrammedCauseState, Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String

                Dim strUniqueidentifierField As String = String.Empty
                If _IDEmployee = -1 Then
                    strUniqueidentifierField = roTypes.Any2String(New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState, ).Value)
                End If

                strSQL = "@SELECT# IDCause, ProgrammedCauses.IDEmployee, ProgrammedCauses.ID, ProgrammedCauses.Date, FinishDate, BeginTime, EndTime, convert(numeric(8,6), Duration) as duration,convert(numeric(8,6), MinDuration) as minDuration, ProgrammedCauses.Description, " &
                                "ProgrammedCauses.Win32 , Causes.Name, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedCauses.AbsenceID AND adf.forecasttype='hours') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered, AbsenceID, " &
                                "Timestamp, " &
                                "'Hour' AS Type,  " &
                                "'CRU' AS Action,  "
                If _IDEmployee > 0 Then
                    strSQL += " '' AS NIF, " &
                                " '' AS IdImport "
                Else
                    strSQL += " NifTable.Value AS NIF, " &
                              " IdTable.Value AS IdImport "
                End If
                strSQL += " FROM ProgrammedCauses " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedCauses.IDCause " &
                                " LEFT JOIN (@SELECT# row_number() over (partition by EmployeeUserFieldValues.IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = 'NIF') NifTable ON NifTable.IDEmployee = ProgrammedCauses.IDEmployee AND NifTable.Date < GETDATE() " &
                                " Left JOIN (@SELECT# row_number() over (partition by EmployeeUserFieldValues.IdEmployee, FieldName order by Date desc) As 'RowNumber1', * from EmployeeUserFieldValues WHERE FieldName = '" & strUniqueidentifierField & "') IdTable ON IdTable.IDEmployee = ProgrammedCauses.IDEmployee AND IdTable.Date < GETDATE() "

                If _IDEmployee > 0 Then
                    strSQL += " WHERE ProgrammedCauses.idEmployee = " & _IDEmployee & " "
                Else
                    strSQL += " WHERE 1=1 "
                End If
                strSQL += " AND " &
                         " Timestamp BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime &
                         " UNION ALL " &
                             "@SELECT# NULL as idcause, " &
                             "IDEmployee,  " &
                             "NULL As ID,  " &
                             "NULL AS Date, " &
                             "Timestamp, " &
                             "NULL As FinishDate, " &
                             "NULL AS BeginTime, " &
                             "NULL AS EndTime, " &
                             "NULL AS minDuration, " &
                             "NULL AS Description, " &
                             "NULL AS Win32, " &
                             "NULL AS Name, " &
                             "NULL AS HasDocuments, " &
                             "NULL As DocumentsDelivered, " &
                             "AbsenceID, " &
                             "Timestamp, " &
                             "'Hour' AS Type,  " &
                             "'D' AS Action,  " &
                             " '' AS NIF, " &
                             " '' AS IdImport " &
                             " FROM DeletedProgrammedCauses " &
                             " WHERE DeletedProgrammedCauses.idEmployee = " & _IDEmployee & " AND Timestamp BETWEEN " & roTypes.Any2Time(_BeginDate).SQLSmallDateTime & " AND " & roTypes.Any2Time(_EndDate).SQLSmallDateTime

                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If

                strSQL = strSQL & " ORDER BY Date DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedAbsences")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedCauses(ByVal _IDEmployee As Integer, ByVal _State As roProgrammedCauseState, Optional ByVal strWhere As String = "") As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDCause, IDEmployee, ProgrammedCauses.ID, Date, FinishDate, BeginTime, EndTime, convert(numeric(8,6), Duration) as duration,convert(numeric(8,6), MinDuration) as minDuration, ProgrammedCauses.Description, " &
                                "ProgrammedCauses.Win32 , Causes.Name, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = Causes.ID) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedCauses.AbsenceID AND adf.forecasttype='hours') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered " &
                         "FROM ProgrammedCauses " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedCauses.IDCause " &
                         "WHERE idEmployee = " & _IDEmployee

                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If

                strSQL = strSQL & " ORDER BY Date DESC"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedAbsences")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedAbsences")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetProgrammedCauses(ByVal _IDEmployee As Integer, ByVal strWhere As String, ByVal _State As roProgrammedCauseState) As DataTable
            Return GetProgrammedCauses(_IDEmployee, _State, strWhere)
        End Function

        Public Shared Function GetProgrammedCause(ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal _ID As Integer, ByVal _State As roProgrammedCauseState, Optional ByVal bolAudit As Boolean = True) As roProgrammedCause

            Dim oRet As roProgrammedCause = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# IDCause, IDEmployee, ID, Date, BeginTime, EndTime, Duration, Description, Win32 " &
                         "FROM ProgrammedCauses " &
                         "WHERE IDEmployee = " & _IDEmployee & " AND " &
                               "Date = " & roTypes.Any2Time(_Date).SQLSmallDateTime() & " AND " &
                               "ID = " & _ID

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    oRet = New roProgrammedCause(tb.Rows(0).Item("IDEmployee"), tb.Rows(0).Item("Date"), tb.Rows(0).Item("ID"), _State)
                    oRet.Load(bolAudit)

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedCause")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedCause::GetProgrammedCause")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function ValidateProgrammedCause(ByVal oProgrammedCause As roProgrammedCause, ByVal _State As roProgrammedCauseState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim queryDateStart As String = roTypes.Any2Time(oProgrammedCause.ProgrammedDate).SQLSmallDateTime()
                Dim queryDateEnd As String = Nothing

                Dim tmpEndDate As Date

                If oProgrammedCause.ProgrammedEndDate.HasValue Then
                    tmpEndDate = oProgrammedCause.ProgrammedEndDate.Value
                    queryDateEnd = roTypes.Any2Time(oProgrammedCause.ProgrammedEndDate.Value).SQLSmallDateTime()
                Else
                    tmpEndDate = oProgrammedCause.ProgrammedDate
                    queryDateEnd = roTypes.Any2Time(oProgrammedCause.ProgrammedDate).SQLSmallDateTime()
                End If

                oProgrammedCause.BeginTime = New Date(1899, 12, 30, oProgrammedCause.BeginTime.Value.Hour, oProgrammedCause.BeginTime.Value.Minute, oProgrammedCause.BeginTime.Value.Second)
                oProgrammedCause.EndTime = New Date(1899, 12, 30, oProgrammedCause.EndTime.Value.Hour, oProgrammedCause.EndTime.Value.Minute, oProgrammedCause.EndTime.Value.Second)

                If oProgrammedCause.BeginTime > oProgrammedCause.EndTime Then
                    oProgrammedCause.EndTime = oProgrammedCause.EndTime.Value.AddDays(1)
                End If

                Dim queryStartHour As String = roTypes.Any2Time(oProgrammedCause.BeginTime).SQLDateTime()
                Dim queryEndHour As String = roTypes.Any2Time(oProgrammedCause.EndTime).SQLDateTime()

                Dim strSQL As String

                If Not oProgrammedCause.ProgrammedDate.HasValue Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDate
                ElseIf oProgrammedCause.ProgrammedDate > tmpEndDate Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDate
                ElseIf Not oProgrammedCause.BeginTime.HasValue Or Not oProgrammedCause.EndTime.HasValue Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDateTimeInterval
                ElseIf roTypes.Any2Time(oProgrammedCause.EndTime.Value).NumericValue = 0 Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDateTimeInterval
                ElseIf oProgrammedCause.BeginTime.Value > oProgrammedCause.EndTime.Value Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDateTimeInterval
                ElseIf oProgrammedCause.Duration = 0 Or roTypes.Any2Time(oProgrammedCause.Duration).NumericValue > roTypes.Any2Time("23:59").NumericValue Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDuration
                ElseIf roTypes.Any2Time(oProgrammedCause.Duration).NumericValue > roTypes.Any2Time(roTypes.Any2Time(oProgrammedCause.EndTime.Value).NumericValue - roTypes.Any2Time(oProgrammedCause.BeginTime.Value).NumericValue).NumericValue Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDuration
                ElseIf roTypes.Any2Time(oProgrammedCause.Duration).NumericValue < roTypes.Any2Time(oProgrammedCause.MinDuration).NumericValue Then
                    _State.Result = ProgrammedCausesResultEnum.InvalidDuration
                Else

                    strSQL = "@SELECT# * from ProgrammedCauses " &
                             "WHERE IDEmployee = " & oProgrammedCause.IDEmployee.Value & " AND "

                    strSQL &= " ( ( (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (IsNULL(FinishDate,Date) >= " & queryDateStart & " AND IsNULL(FinishDate,Date) <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (Date <= " & queryDateStart & " AND IsNULL(FinishDate,Date) >= " & queryDateEnd & ") ) AND  ("

                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"

                    If oProgrammedCause.IDEmployeeOriginal.HasValue Then
                        strSQL &= " AND CONVERT(varchar, IDEmployee) + '*' + CONVERT(varchar, Date, 103) + '*' + CONVERT(varchar,ID) <> '" & oProgrammedCause.IDEmployeeOriginal.Value & "*" & Format(oProgrammedCause.ProgrammedDateOriginal.Value, "dd/MM/yyyy") & "*" & oProgrammedCause.ID.Value & "'"
                    End If
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedCausesResultEnum.AnotherExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                Dim _ProgrammedDate As Nullable(Of Date) = oProgrammedCause.ProgrammedDate
                Dim _ProgrammedDateOriginal As Nullable(Of Date) = oProgrammedCause.ProgrammedDateOriginal
                Dim _ProgrammedEndDate As Nullable(Of Date)
                Dim _ProgrammedEndDateOriginal As Nullable(Of Date)

                If oProgrammedCause.ProgrammedEndDate.HasValue Then
                    _ProgrammedEndDate = oProgrammedCause.ProgrammedEndDate
                    _ProgrammedEndDateOriginal = oProgrammedCause.ProgrammedEndDateOriginal
                Else
                    _ProgrammedEndDate = oProgrammedCause.ProgrammedDate
                    _ProgrammedEndDateOriginal = oProgrammedCause.ProgrammedEndDateOriginal
                End If

                'Comprovem si la ausencia es troba en periode de congelació
                If bolRet Then
                    Dim freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedCause.IDEmployee.Value, False, _State)
                    Dim _IDCauseOld As Integer = oProgrammedCause.IDCause

                    'Si existeix la ausencia i es una modificació
                    If _ProgrammedDateOriginal.HasValue Then
                        'Recuperem la justificacio antiga per fer comprovacions
                        Dim tb As New DataTable("ProgrammedCauses")
                        strSQL = "@SELECT# * FROM ProgrammedCauses WHERE "
                        strSQL &= "ProgrammedCauses.IDEmployee = "
                        If oProgrammedCause.IDEmployeeOriginal.HasValue Then
                            strSQL &= oProgrammedCause.IDEmployeeOriginal.Value & " AND "
                        Else
                            strSQL &= oProgrammedCause.IDEmployee.Value & " AND "
                        End If
                        strSQL &= "ProgrammedCauses.Date >= "
                        If _ProgrammedDateOriginal.HasValue Then
                            strSQL &= roTypes.Any2Time(_ProgrammedDateOriginal.Value).SQLSmallDateTime & " AND "
                        Else
                            strSQL &= roTypes.Any2Time(_ProgrammedDate.Value).SQLSmallDateTime & " AND "
                        End If

                        strSQL &= "ProgrammedCauses.Date <= "
                        If _ProgrammedEndDateOriginal.HasValue Then
                            strSQL &= roTypes.Any2Time(_ProgrammedEndDateOriginal.Value).SQLSmallDateTime
                        Else
                            strSQL &= roTypes.Any2Time(_ProgrammedEndDate.Value).SQLSmallDateTime
                        End If

                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        If tb.Rows.Count > 0 Then
                            _IDCauseOld = tb.Rows(0)("IDCause")
                        End If

                        If oProgrammedCause.ProgrammedDateOriginal = oProgrammedCause.ProgrammedDate Then
                            'Si les dates coincideixen, i es troba en periode de congelacio, comprovem que no
                            's'hagi modificat la justificació
                            If oProgrammedCause.ProgrammedDate.Value <= freezeDate Then
                                If oProgrammedCause.IDCause <> _IDCauseOld Then
                                    _State.Result = ProgrammedCausesResultEnum.InFreezeDate
                                    bolRet = False
                                End If

                            End If
                        Else
                            If oProgrammedCause.ProgrammedDateOriginal <> oProgrammedCause.ProgrammedDate And
                                oProgrammedCause.ProgrammedDate.Value <= freezeDate Then
                                _State.Result = ProgrammedCausesResultEnum.InFreezeDate
                                bolRet = False
                            ElseIf oProgrammedCause.ProgrammedDate.Value <= freezeDate And oProgrammedCause.IDCause <> _IDCauseOld Then
                                _State.Result = ProgrammedCausesResultEnum.InFreezeDate
                                bolRet = False
                            End If
                        End If

                        If bolRet Then
                            ' Comprobamos datos de seguimiento en el caso que sea una modificación
                            Dim oLicense As New roServerLicense
                            If oLicense.FeatureIsInstalled("Feature\Absences") Then
                                Dim intCountTrackDays As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM AbsenceTracking WHERE TypeAbsence = 1 AND IDEmployee=" & oProgrammedCause.IDEmployeeOriginal & " AND IDCause=" & _IDCauseOld & " AND Date=" & roTypes.Any2Time(oProgrammedCause.ProgrammedDateOriginal.Value).SQLSmallDateTime & " AND IDAbsence=" & oProgrammedCause.ID))
                                If intCountTrackDays > 0 Then
                                    If oProgrammedCause.IDCause <> _IDCauseOld Then
                                        ' Si se cambia la justificacion y tiene registros de seguimiento no se deja cambiar
                                        _State.Result = ProgrammedCausesResultEnum.ExistTrackingDays
                                        bolRet = False
                                    End If

                                    If bolRet Then
                                        If oProgrammedCause.ProgrammedDateOriginal <> oProgrammedCause.ProgrammedDate Then
                                            ' Si se cambia la fecha de inicio y tiene registros de seguimiento no se deja cambiar
                                            _State.Result = ProgrammedCausesResultEnum.ExistTrackingDays
                                            bolRet = False
                                        End If
                                    End If

                                    If bolRet Then
                                        If oProgrammedCause.ProgrammedEndDateOriginal <> oProgrammedCause.ProgrammedEndDate Then
                                            ' Si se cambia la fecha de finalización
                                            ' y Si existen registros de seguimiento ya entregados >= a la nueva fecha de finalización no se deja cambiar
                                            Dim intCountTrackDaysWithDeliveredDays As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# COUNT(*) FROM AbsenceTracking WHERE TypeAbsence = 1 AND IDEmployee=" & oProgrammedCause.IDEmployeeOriginal & " AND IDCause=" & _IDCauseOld & " AND Date=" & roTypes.Any2Time(oProgrammedCause.ProgrammedDateOriginal).SQLSmallDateTime & " AND IDAbsence=" & oProgrammedCause.ID & " AND DeliveryDate IS not null AND TrackDay >=" & roTypes.Any2Time(oProgrammedCause.ProgrammedEndDate).SQLSmallDateTime))
                                            If intCountTrackDaysWithDeliveredDays > 0 Then
                                                _State.Result = ProgrammedCausesResultEnum.ExistTrackingDays
                                                bolRet = False
                                            End If
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    Else
                        'Si es una nova ausencia
                        If oProgrammedCause.ProgrammedDate.Value <= freezeDate Then
                            _State.Result = ProgrammedCausesResultEnum.InFreezeDate
                            bolRet = False
                        End If

                    End If
                End If

                'Comprovem si la ausencia es troba dins el periode de contracte.
                If bolRet Then
                    strSQL = "@SELECT# * FROM EmployeeContracts WHERE " &
                             "BeginDate <= " & roTypes.Any2Time(_ProgrammedDate.Value).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(_ProgrammedDate.Value).SQLSmallDateTime() & " AND " &
                             "BeginDate <= " & roTypes.Any2Time(_ProgrammedEndDate.Value).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(_ProgrammedEndDate.Value).SQLSmallDateTime() & " AND " &
                             "IDEmployee = " & oProgrammedCause.IDEmployee
                    Dim dTblC As DataTable = CreateDataTable(strSQL)
                    If dTblC Is Nothing OrElse dTblC.Rows.Count = 0 Then
                        _State.Result = ProgrammedCausesResultEnum.DateOutOfContract
                        bolRet = False
                    End If
                End If

                ' Comprovemo si existeix una ausencia prolongada pel mateix dia
                If bolRet Then
                    strSQL = "@SELECT# * from ProgrammedAbsences WHERE IDEmployee = " & oProgrammedCause.IDEmployee.Value & " AND "

                    strSQL &= " ( ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") )"

                    Dim tbX As DataTable = CreateDataTable(strSQL)
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            _State.Result = ProgrammedCausesResultEnum.AnotherAbsenceExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                ' Verificamos si existe una prevision de vacaciones por horas en el mismo periodo
                If bolRet Then
                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                 "WHERE IDEmployee = " & oProgrammedCause.IDEmployee.Value & " AND "
                    strSQL &= "  (Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & ") AND  "
                    strSQL &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                    ' o una prevision de vacaciones de todo el dia para la misma fecha
                    strSQL &= " Or (AllDay = 1))"

                    Dim tbX As DataTable = CreateDataTable(strSQL)
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            _State.Result = ProgrammedCausesResultEnum.AnotherHolidayExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                ' verificamos si existe otra prevision de exceso para el mismo periodo
                If bolRet Then
                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                             "WHERE IDEmployee = " & oProgrammedCause.IDEmployee.Value & " AND "
                    strSQL &= " ( (  (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (EndDate >= " & queryDateStart & " AND EndDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & queryDateStart & " AND EndDate >= " & queryDateEnd & ") )  AND  ("

                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) )"

                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = ProgrammedCausesResultEnum.AnotherOvertimeinDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                If bolRet Then
                    Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("VTLive.Absences.ValidateHolidayOnDate", New AdvancedParameter.roAdvancedParameterState())
                    If roTypes.Any2Boolean(oAdvParam.Value) Then
                        ' Verificamos que la previsión no se solape con un horario de vacaciones planificado
                        strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                    "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & oProgrammedCause.IDEmployee.Value & " AND "
                        strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND ( isnull(IsHolidays,0) = 1 ) "
                        Dim tb As DataTable = CreateDataTable(strSQL)
                        If tb IsNot Nothing Then
                            If tb.Rows.Count > 0 Then
                                _State.Result = ProgrammedCausesResultEnum.AnotherHolidayExistInDate
                                bolRet = False
                            Else
                                bolRet = True
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedCause::ValidateProgrammedCause")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedCause::ValidateProgrammedCause")
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace

Namespace Forecast

    <Serializable()>
    <DataContract()>
    Public MustInherit Class roForecast
        <DataMember>
        Public MustOverride Property BeginDate As Nullable(Of DateTime)
        <DataMember>
        Public MustOverride Property FinishDate As Nullable(Of DateTime)
        <DataMember>
        Public MustOverride Property MaxLastingDays() As Integer
        <DataMember>
        Public MustOverride Property RealFinishDate() As DateTime
        <DataMember>
        Public MustOverride Property IDCause() As Nullable(Of Integer)

        Public MustOverride Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

        Public MustOverride Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

        Public MustOverride Function Save(Optional ByVal bAudit As Boolean = False, Optional CreateTask As Boolean = True) As Boolean

        Public MustOverride Overloads Function ToString() As String

        Public MustOverride Function SurfaceCopy() As Object

    End Class

    <DataContract()>
    Public Class roProgrammedOvertimeProxy
        Inherits Forecast.roForecast

        Private _iID As Long
        Private _iIDEmployee As Integer
        Private _dBeginDate As Nullable(Of DateTime)
        Private _dFinishDate As Nullable(Of DateTime)
        Private _iIDCause As Nullable(Of Integer)
        Private oManager As VTHolidays.roProgrammedOvertimeManager
        Private oProgrammedOvertine As DTOs.roProgrammedOvertime

        <DataMember>
        Public Property ID As Long
            Get
                Return _iID
            End Get
            Set(value As Long)
                _iID = value
                If Not oProgrammedOvertine Is Nothing Then oProgrammedOvertine.ID = value
            End Set
        End Property

        <DataMember>
        Public Property IDEmployee As Integer
            Get
                Return _iIDEmployee
            End Get
            Set(value As Integer)
                _iIDEmployee = value
                If Not oProgrammedOvertine Is Nothing Then oProgrammedOvertine.IDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Overrides Property BeginDate As Date?
            Get
                Return _dBeginDate
            End Get
            Set(value As Date?)
                _dBeginDate = value
                If Not oProgrammedOvertine Is Nothing Then oProgrammedOvertine.ProgrammedBeginDate = value
            End Set
        End Property

        <DataMember>
        Public Overrides Property FinishDate As Date?
            Get
                Return _dFinishDate
            End Get
            Set(value As Date?)
                _dFinishDate = value
                If Not oProgrammedOvertine Is Nothing Then oProgrammedOvertine.ProgrammedEndDate = value
            End Set
        End Property

        <DataMember>
        Public Overrides Property MaxLastingDays As Integer
            Get
                Return 0
            End Get
            Set(value As Integer)

            End Set
        End Property

        <DataMember>
        Public Overrides Property RealFinishDate As Date
            Get
                Return _dFinishDate
            End Get
            Set(value As Date)
                _dFinishDate = value
                If Not oProgrammedOvertine Is Nothing Then oProgrammedOvertine.ProgrammedEndDate = value
            End Set
        End Property

        <DataMember>
        Public Overrides Property IdCause As Integer?
            Get
                Return _iIDCause
            End Get
            Set(value As Integer?)
                _iIDCause = value
                If Not oProgrammedOvertine Is Nothing Then oProgrammedOvertine.IDCause = value
            End Set
        End Property

        Public Sub New()
            Me.ID = -1
        End Sub

        Public Sub New(ByVal ID As Long, ByVal _IDEmployee As Integer, ByVal _BeginDate As Date)
            Me.ID = ID
            Me.IDEmployee = _IDEmployee
            Me.BeginDate = _BeginDate
            oManager = New VTHolidays.roProgrammedOvertimeManager
        End Sub

        Public Overrides Function Load(Optional bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            oProgrammedOvertine = oManager.LoadProgrammedOvertime(Me.ID)
            If oManager.State.Result = OvertimeResultEnum.NoError Then
                Me.BeginDate = oProgrammedOvertine.ProgrammedBeginDate
                Me.FinishDate = oProgrammedOvertine.ProgrammedEndDate
                Me.IdCause = oProgrammedOvertine.IDCause
                Me.IDEmployee = oProgrammedOvertine.IDEmployee
                oRet = True
            End If
            Return oRet
        End Function

        Public Overrides Function Delete(Optional bAudit As Boolean = False) As Boolean
            Return oManager.DeleteProgrammedOvertime(oProgrammedOvertine)
        End Function

        Public Overrides Function Save(Optional bAudit As Boolean = False, Optional CreateTask As Boolean = True) As Boolean
            Return oManager.SaveProgrammedOvertime(oProgrammedOvertine)
        End Function

        Public Overrides Function ToString() As String
            Dim oRet As String = String.Empty
            Try
                oRet = "Horas de exceso del empleado " & Employee.roEmployee.GetEmployee(Me.IDEmployee, Nothing).Name & " desde el día " & Me.BeginDate.Value.ToShortDateString & " hasta el día " & Me.RealFinishDate.ToShortDateString & " por " & Cause.roCause.GetCauseNameByID(Me.IdCause)
            Catch ex As Exception
                oRet = "Error recuperando descripción de ausencia por horas"
            End Try
            Return oRet
        End Function

        Public Overrides Function SurfaceCopy() As Object
            Return Me.MemberwiseClone
        End Function

    End Class

End Namespace