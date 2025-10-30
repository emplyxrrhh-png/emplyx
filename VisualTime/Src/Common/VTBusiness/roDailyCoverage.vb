Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Scheduler

    <DataContract>
    Public Class roDailyCoverage

        Public Enum RecalculateTaskType
            <EnumMember> Update_All
            <EnumMember> Update_Planned
            <EnumMember> Update_Actual
        End Enum

#Region "Declarations - Constructors"

        Private oState As roSchedulerState

        Private intIDGroup As Integer
        Private xCoverageDate As Date

        Private lstCoverageAssignments As Generic.List(Of roDailyCoverageAssignment)

        Public Sub New()

            Me.oState = New roSchedulerState
            Me.intIDGroup = -1

        End Sub

        Public Sub New(ByVal _IDGroup As Integer, ByVal _CoverageDate As Date, ByVal _State As roSchedulerState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.intIDGroup = _IDGroup
            Me.xCoverageDate = _CoverageDate

            Me.Load(_Audit)

        End Sub

#End Region

#Region "Properties"

        Public Property State() As roSchedulerState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roSchedulerState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDGroup() As Integer
            Get
                Return Me.intIDGroup
            End Get
            Set(ByVal value As Integer)
                Me.intIDGroup = value
            End Set
        End Property
        <DataMember()>
        Public Property CoverageDate() As Date
            Get
                Return Me.xCoverageDate
            End Get
            Set(ByVal value As Date)
                Me.xCoverageDate = value
            End Set
        End Property
        <DataMember()>
        Public Property CoverageAssignments() As Generic.List(Of roDailyCoverageAssignment)
            Get
                Return Me.lstCoverageAssignments
            End Get
            Set(ByVal value As Generic.List(Of roDailyCoverageAssignment))
                Me.lstCoverageAssignments = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Obtenemos la lista de puestos definidos para la dotación
                Me.lstCoverageAssignments = roDailyCoverageAssignment.GetDailyCoverageAssignments(Me.intIDGroup, Me.xCoverageDate, Me.oState, _Audit)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAssignment::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAssignment::Load")
            End Try

            Return bolRet

        End Function

        Public Function SaveTeoric() As Boolean

            Dim bolRet As Boolean = False
            Try

                bolRet = roDailyCoverageAssignment.SaveTeoricDailyCoverageAssignments(Me.intIDGroup, Me.xCoverageDate, Me.lstCoverageAssignments, Me.oState, True)
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAssignment::SaveTeoric")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAssignment::SaveTeoric")
            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean
            Dim bolRet As Boolean

            Try

                ' Obtengo la fecha de congelación
                Dim xFreezingDate As Date = Nothing
                Dim bolCanSave As Boolean = False
                Dim oParameters As New roParameters("OPTIONS", True)
                If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing AndAlso IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                    xFreezingDate = CDate(oParameters.Parameter(Parameters.FirstDate))
                End If

                bolRet = True

                If xFreezingDate <> Nothing Then ' Si hay definida una fecha de congelación
                    ' Miro si la fecha del valor del campo está dentro de periodo de congelación
                    bolCanSave = (Me.CoverageDate > xFreezingDate)
                    If Not bolCanSave Then
                        bolRet = False
                        Me.oState.Result = SchedulerResultEnum.DailyScheduleLockedDay
                    End If
                End If

                If bolRet Then

                    Dim strGroup As String = Any2String(ExecuteScalar("@SELECT# Name FROM Groups where id=" & Me.intIDGroup.ToString))

                    Dim strSQL As String = "@DELETE# FROM DailyCoverage WHERE IDGroup = " & Me.intIDGroup.ToString & " AND Date = " & Any2Time(Me.xCoverageDate).SQLSmallDateTime
                    bolRet = ExecuteSql(strSQL)

                    If bolRet Then
                        Dim strSQL2 As String = "@DELETE# FROM sysroNotificationTasks WHERE Key1Numeric = " & Me.intIDGroup.ToString & " AND Key3DateTime = " & Any2Time(Me.xCoverageDate).SQLDateTime & " AND IDNotification IN(@SELECT# id from Notifications WHERE IDType=14)"
                        bolRet = ExecuteSql(strSQL2)
                    End If

                    If bolRet Then
                        bolRet = roDailyCoverage.Recalculate(roDailyCoverage.RecalculateTaskType.Update_All, Me.oState)
                    End If

                    If bolRet Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDailyCoverage, strGroup & ": " & Me.xCoverageDate.Date, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAssignment::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAssignment::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetDailyCoverages(ByVal _IDGroup As Integer, ByVal _BeginCoverageDate As Date, ByVal _EndCoverageDate As Date, ByVal _State As roSchedulerState, Optional ByVal _Audit As Boolean = False) As Generic.List(Of roDailyCoverage)

            Dim oRet As New Generic.List(Of roDailyCoverage)

            Try
                Dim xDate As Date = _BeginCoverageDate.Date
                While xDate <= _EndCoverageDate.Date

                    oRet.Add(New roDailyCoverage(_IDGroup, xDate, _State, _Audit))

                    xDate = xDate.AddDays(1)

                End While
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAssignment::GetDailyCoverages")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::GetDailyCoverages")
            End Try

            Return oRet

        End Function

        Public Shared Function RecalculateEx(ByVal _Command As RecalculateTaskType, ByVal _State As roSchedulerState, ByVal bolHRSchedulingInstalled As Boolean, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _IDShift As Integer = -1, Optional ByVal _ShiftDate As Date = Nothing, Optional ByVal _Notify As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try

                If bolHRSchedulingInstalled Then

                    Dim strSQL As String
                    Dim strStatusSQL As String = ""
                    Select Case _Command
                        Case RecalculateTaskType.Update_All
                            strStatusSQL = "PlannedStatus = 0, ActualStatus = 0 "
                        Case RecalculateTaskType.Update_Planned
                            strStatusSQL = "PlannedStatus = 0 "
                        Case RecalculateTaskType.Update_Actual
                            strStatusSQL = "ActualStatus = 0 "
                    End Select
                    If _IDEmployee > 0 AndAlso _ShiftDate <> Nothing Then

                        strSQL = "@DECLARE# @Path nvarchar(4000) " &
                                 "@SELECT# @Path = Path FROM sysrovwAllEmployeeGroups WHERE IDEmployee = " & _IDEmployee.ToString & " AND BeginDate <= " & Any2Time(_ShiftDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(_ShiftDate).SQLSmallDateTime & " " &
                                 "@UPDATE# DailyCoverage SET " & strStatusSQL &
                                 "WHERE IDGroup IN (@SELECT# * FROM SplitInt(@Path,'\')) AND Date = " & Any2Time(_ShiftDate).SQLSmallDateTime
                        bolRet = ExecuteSql(strSQL)

                    ElseIf _IDShift > 0 AndAlso _ShiftDate <> Nothing Then

                        strSQL = "@UPDATE# DailyCoverage SET " & strStatusSQL &
                                 "WHERE IDAssignment IN (@SELECT# ShiftAssignments.IDAssignment FROM ShiftAssignments WHERE ShiftAssignments.IDShift = " & _IDShift.ToString & ") AND " &
                                       "Date >= " & Any2Time(_ShiftDate).SQLSmallDateTime
                        If _Command = RecalculateTaskType.Update_Actual Or _Command = RecalculateTaskType.Update_All Then
                            strSQL &= " AND Date <= " & Any2Time(Now.Date).SQLSmallDateTime
                        End If
                        bolRet = ExecuteSql(strSQL)

                    End If

                    If _Notify Then

                        ' Lanza el proceso de recálculo
                        Dim Command(-1) As String
                        Select Case _Command
                            Case RecalculateTaskType.Update_All
                                ReDim Command(1)
                                Command(0) = "UPDATE_PLANNED"
                                Command(1) = "UPDATE_ACTUAL"
                            Case RecalculateTaskType.Update_Planned
                                ReDim Command(0)
                                Command(0) = "UPDATE_PLANNED"
                            Case RecalculateTaskType.Update_Actual
                                ReDim Command(0)
                                Command(0) = "UPDATE_ACTUAL"
                        End Select

                        Dim oParamsAux As roCollection = Nothing
                        For Each strCommand As String In Command
                            oParamsAux = New roCollection
                            oParamsAux.Add("Command", strCommand)
                            roConnector.InitTask(TasksType.HRSCHEDULER, oParamsAux)
                        Next

                    End If

                End If

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverage::RecalculateEx")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::RecalculateEx")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function Recalculate(ByVal _Command As RecalculateTaskType, ByVal _State As roSchedulerState, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _IDShift As Integer = -1, Optional ByVal _ShiftDate As Date = Nothing, Optional ByVal _Notify As Boolean = True) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim oLicense As New roServerLicense

                If oLicense.FeatureIsInstalled("Feature\HRScheduling") Then

                    Dim strSQL As String
                    Dim strStatusSQL As String = ""
                    Select Case _Command
                        Case RecalculateTaskType.Update_All
                            strStatusSQL = "PlannedStatus = 0, ActualStatus = 0 "
                        Case RecalculateTaskType.Update_Planned
                            strStatusSQL = "PlannedStatus = 0 "
                        Case RecalculateTaskType.Update_Actual
                            strStatusSQL = "ActualStatus = 0 "
                    End Select
                    If _IDEmployee > 0 AndAlso _ShiftDate <> Nothing Then

                        strSQL = "@DECLARE# @Path nvarchar(4000) " &
                                 "@SELECT# @Path = Path FROM sysrovwAllEmployeeGroups WHERE IDEmployee = " & _IDEmployee.ToString & " AND BeginDate <= " & Any2Time(_ShiftDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(_ShiftDate).SQLSmallDateTime & " " &
                                 "@UPDATE# DailyCoverage SET " & strStatusSQL &
                                 "WHERE IDGroup IN (@SELECT# * FROM SplitInt(@Path,'\')) AND Date = " & Any2Time(_ShiftDate).SQLSmallDateTime
                        bolRet = ExecuteSql(strSQL)

                    ElseIf _IDShift > 0 AndAlso _ShiftDate <> Nothing Then

                        strSQL = "@UPDATE# DailyCoverage SET " & strStatusSQL &
                                 "WHERE IDAssignment IN (@SELECT# ShiftAssignments.IDAssignment FROM ShiftAssignments WHERE ShiftAssignments.IDShift = " & _IDShift.ToString & ") AND " &
                                       "Date >= " & Any2Time(_ShiftDate).SQLSmallDateTime
                        If _Command = RecalculateTaskType.Update_Actual Or _Command = RecalculateTaskType.Update_All Then
                            strSQL &= " AND Date <= " & Any2Time(Now.Date).SQLSmallDateTime
                        End If
                        bolRet = ExecuteSql(strSQL)

                    End If

                    If _Notify Then

                        ' Lanza el proceso de recálculo
                        Dim Command(-1) As String
                        Select Case _Command
                            Case RecalculateTaskType.Update_All
                                ReDim Command(1)
                                Command(0) = "UPDATE_PLANNED"
                                Command(1) = "UPDATE_ACTUAL"
                            Case RecalculateTaskType.Update_Planned
                                ReDim Command(0)
                                Command(0) = "UPDATE_PLANNED"
                            Case RecalculateTaskType.Update_Actual
                                ReDim Command(0)
                                Command(0) = "UPDATE_ACTUAL"
                        End Select

                        Dim oParamsAux As roCollection = Nothing
                        For Each strCommand As String In Command
                            oParamsAux = New roCollection
                            oParamsAux.Add("Command", strCommand)
                            roConnector.InitTask(TasksType.HRSCHEDULER, oParamsAux)
                        Next

                    End If

                End If

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAssignment::ExecuteTask")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::ExecuteTask")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyTeoricDailyCoverage(ByVal _IDGroup As Integer, ByVal _SourceBeginDate As Date, ByVal _SourceEndDate As Date, ByVal _DestinationBeginDate As Date, ByVal _DestinationEndDate As Date, ByRef _State As roSchedulerState) As Boolean
            Dim bolRet As Boolean = False
            Dim oDailyCoverages As New Generic.List(Of roDailyCoverage)

            Try
                ' Si no tiene permisos de escritura no hacemos nada
                Dim oPermissionPassport As Permission = Robotics.Security.WLHelper.GetPermissionOverFeature(_State.IDPassport, "Calendar.Scheduler", "U")

                If oPermissionPassport < Permission.Write Then
                    _State.Result = SchedulerResultEnum.DailyScheduleLockedDay
                    Return bolRet
                    Exit Function
                End If

                Dim xDate As Date = _DestinationBeginDate.Date
                Dim xDateSource As Date = _SourceBeginDate.Date
                While xDate <= _DestinationEndDate.Date

                    ' Obtenemos las coberturas del dia origen
                    Dim oSourceDailyCoverage As New roDailyCoverage(_IDGroup, xDateSource, _State)

                    ' Borramos las coberturas del dia destino
                    Dim oDestinationDailyCoverage As New roDailyCoverage(_IDGroup, xDate, _State)
                    oDestinationDailyCoverage.Delete()

                    'Guardamos la cobertura origen en la fecha destino
                    oSourceDailyCoverage.CoverageDate = xDate
                    oSourceDailyCoverage.SaveTeoric()

                    ' Pasamos al dia siguiente
                    xDate = xDate.AddDays(1)
                    xDateSource = xDateSource.AddDays(1)

                    If xDateSource > _SourceEndDate Then
                        xDateSource = _SourceBeginDate
                    End If
                End While

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roAssignment::CopyTeoricDailyCoverage")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAssignment::CopyTeoricDailyCoverage")
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

    <DataContract>
    Public Class roDailyCoverageAssignment

#Region "Declarations - Constructors"

        Private oState As roSchedulerState

        Private intIDGroup As Integer
        Private xCoverageDate As Date

        Private intIDAssignment As Integer
        Private dblExpectedCoverage As Double
        Private dblPlannedCoverage As Double
        Private dblActualCoverage As Double

        Private bolForcedCoveraged As Nullable(Of Boolean)
        Private bolReal As Nullable(Of Boolean)
        Private bolVerified As Nullable(Of Boolean)

        Private intPlannedStatus As Integer
        Private intActualStatus As Integer

        Public Sub New()

            Me.oState = New roSchedulerState
            Me.intIDGroup = -1
            Me.intIDAssignment = -1

        End Sub

        Public Sub New(ByVal _IDGroup As Integer, ByVal _CoverageDate As Date, ByVal _IDAssignment As Integer, ByVal _State As roSchedulerState, Optional ByVal _Audit As Boolean = False)

            Me.oState = _State

            Me.intIDGroup = _IDGroup
            Me.xCoverageDate = _CoverageDate
            Me.intIDAssignment = _IDAssignment

            Me.Load(_Audit)

        End Sub

        Public Sub New(ByVal oRow As DataRow, ByVal _State As roSchedulerState)

            Me.oState = _State

            Me.intIDGroup = oRow("IDGroup")
            Me.xCoverageDate = oRow("Date")
            Me.intIDAssignment = oRow("IDAssignment")

            Me.ExpectedCoverage = Any2Double(oRow("ExpectedCoverage"))
            Me.PlannedCoverage = Any2Double(oRow("PlannedCoverage"))
            Me.ActualCoverage = Any2Double(oRow("ActualCoverage"))

            If Not IsDBNull(oRow("ForcedCoveraged")) Then
                Me.bolForcedCoveraged = CBool(oRow("ForcedCoveraged"))
            Else
                Me.bolForcedCoveraged = Nothing
            End If
            If Not IsDBNull(oRow("Real")) Then
                Me.bolReal = CBool(oRow("Real"))
            Else
                Me.bolReal = Nothing
            End If
            Me.bolVerified = True
            ''If Not IsDBNull(oRow("Verified")) Then
            ''    Me.bolVerified = CBool(oRow("Verified"))
            ''Else
            ''    Me.bolVerified = Nothing
            ''End If

            Me.intPlannedStatus = Any2Integer(oRow("PlannedStatus"))
            Me.intActualStatus = Any2Integer(oRow("ActualStatus"))

        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property IDGroup() As Integer
            Get
                Return Me.intIDGroup
            End Get
            Set(ByVal value As Integer)
                Me.intIDGroup = value
            End Set
        End Property
        <DataMember>
        Public Property CoverageDate() As Date
            Get
                Return Me.xCoverageDate
            End Get
            Set(ByVal value As Date)
                Me.xCoverageDate = value
            End Set
        End Property
        <DataMember>
        Public Property IDAssignment() As Integer
            Get
                Return Me.intIDAssignment
            End Get
            Set(ByVal value As Integer)
                Me.intIDAssignment = value
            End Set
        End Property
        <DataMember>
        Public Property ExpectedCoverage() As Double
            Get
                Return Me.dblExpectedCoverage
            End Get
            Set(ByVal value As Double)
                Me.dblExpectedCoverage = value
            End Set
        End Property
        <DataMember>
        Public Property PlannedCoverage() As Double
            Get
                Return Me.dblPlannedCoverage
            End Get
            Set(ByVal value As Double)
                Me.dblPlannedCoverage = value
            End Set
        End Property
        <DataMember>
        Public Property ActualCoverage() As Double
            Get
                Return Me.dblActualCoverage
            End Get
            Set(ByVal value As Double)
                Me.dblActualCoverage = value
            End Set
        End Property
        <DataMember>
        Public Property ForcedCoveraged() As Nullable(Of Boolean)
            Get
                Return Me.bolForcedCoveraged
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolForcedCoveraged = value
            End Set
        End Property
        <DataMember>
        Public Property Real() As Nullable(Of Boolean)
            Get
                Return Me.bolReal
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolReal = value
            End Set
        End Property
        <DataMember>
        Public Property Verified() As Nullable(Of Boolean)
            Get
                Return Me.bolVerified
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                Me.bolVerified = value
            End Set
        End Property
        <DataMember>
        Public Property PlannedStatus() As Integer
            Get
                Return Me.intPlannedStatus
            End Get
            Set(ByVal value As Integer)
                Me.intPlannedStatus = value
            End Set
        End Property
        <DataMember>
        Public Property ActualStatus() As Integer
            Get
                Return Me.intActualStatus
            End Get
            Set(ByVal value As Integer)
                Me.intActualStatus = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM DailyCoverage " &
                                       "WHERE IDGroup = " & Me.intIDGroup.ToString & " AND " &
                                             "Date = " & Any2Time(Me.xCoverageDate).SQLSmallDateTime & " AND " &
                                             "IDAssignment = " & Me.intIDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    Me.ExpectedCoverage = Any2Double(oRow("ExpectedCoverage"))
                    Me.PlannedCoverage = Any2Double(oRow("PlannedCoverage"))
                    Me.ActualCoverage = Any2Double(oRow("ActualCoverage"))

                    If Not IsDBNull(oRow("ForcedCoveraged")) Then
                        Me.bolForcedCoveraged = CBool(oRow("ForcedCoveraged"))
                    Else
                        Me.bolForcedCoveraged = Nothing
                    End If
                    If Not IsDBNull(oRow("Real")) Then
                        Me.bolReal = CBool(oRow("Real"))
                    Else
                        Me.bolReal = Nothing
                    End If
                    Me.bolVerified = True
                    ''If Not IsDBNull(oRow("Verified")) Then
                    ''    Me.bolVerified = CBool(oRow("Verified"))
                    ''Else
                    ''    Me.bolVerified = Nothing
                    ''End If

                    Me.intPlannedStatus = Any2Integer(oRow("PlannedStatus"))
                    Me.intActualStatus = Any2Integer(oRow("ActualStatus"))

                    ' TODO: auditar DailyCoverageAssignment
                    ' Auditar lectura
                    ''If _Audit Then
                    ''    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    ''    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDailyCoverageAssignment, Me.AuditObjectName(), tbParameters, -1)
                    ''End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean
            Dim bolRet As Boolean = True

            Try

                If Me.dblExpectedCoverage <= 0 Then
                    oState.Result = SchedulerResultEnum.DailyCoverageAssignmentInvalidExpectedCoverage
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::Validate")
            End Try

            Return bolRet

        End Function

        Public Function SaveTeoric(Optional ByVal _Audit As Boolean = False, Optional ByVal _ExecuteTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bolChangeData As Boolean = False

            Try
                If Me.Validate() Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim oOldDailyCoverageAssignment As roDailyCoverageAssignment = Nothing

                    Dim tb As New DataTable("DailyCoverage")
                    Dim strSQL As String = "@SELECT# * FROM DailyCoverage " &
                                           "WHERE IDGroup = " & Me.intIDGroup.ToString & " AND " &
                                                 "Date = " & Any2Time(Me.xCoverageDate).SQLSmallDateTime & " AND " &
                                                 "IDAssignment = " & Me.intIDAssignment.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDGroup") = Me.intIDGroup
                        oRow("Date") = Me.xCoverageDate
                        oRow("IDAssignment") = Me.intIDAssignment
                        bolChangeData = True
                    Else
                        oOldDailyCoverageAssignment = New roDailyCoverageAssignment(Me.intIDGroup, Me.xCoverageDate, Me.intIDAssignment, Me.oState)
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        If oRow("IDAssignment") <> Me.intIDAssignment Then
                            bolChangeData = True
                        End If
                        If oRow("ExpectedCoverage") <> Me.dblExpectedCoverage Then
                            bolChangeData = True
                        End If

                    End If

                    If Any2Double(oRow("ExpectedCoverage")) <> Me.dblExpectedCoverage Then
                        oRow("ForcedCoveraged") = False
                        oRow("PlannedStatus") = 0
                        oRow("ActualStatus") = 0
                    End If

                    oRow("ExpectedCoverage") = Me.dblExpectedCoverage

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    If _ExecuteTask Then
                        ' Lanzamps los procesos de recálculo
                        roDailyCoverage.Recalculate(roDailyCoverage.RecalculateTaskType.Update_All, Me.oState)
                    End If

                    oAuditDataNew = oRow

                    If bolRet And _Audit And bolChangeData Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String

                        Dim strGroup As String = Any2String(ExecuteScalar("@SELECT# Name FROM Groups where id=" & Me.intIDGroup.ToString))
                        Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & oAuditDataNew("IDAssignment")))

                        strObjectName = strGroup & ": " & Me.xCoverageDate.Date & ":" & strAssignment
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tDailyCoverage, strObjectName, tbAuditParameters, -1)

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::SaveTeoric")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::SaveTeoric")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strGroup As String = Any2String(ExecuteScalar("@SELECT# Name FROM Groups where id=" & Me.intIDGroup.ToString))
                Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & Me.intIDAssignment.ToString))

                Dim DelQuerys() As String = {"@DELETE# FROM DailyCoverage WHERE IDGroup = " & Me.intIDGroup.ToString & " AND " &
                                                                             "Date = " & Any2Time(Me.xCoverageDate).SQLSmallDateTime & " AND " &
                                                                             "IDAssignment = " & Me.intIDAssignment.ToString,
                                             "@DELETE# FROM sysroNotificationTasks WHERE Key1Numeric = " & Me.intIDGroup.ToString & " AND " &
                                                                             "Key3DateTime = " & Any2Time(Me.xCoverageDate).SQLDateTime & " AND " &
                                                                             "Key2Numeric = " & Me.intIDAssignment.ToString & " AND IDNotification IN(@SELECT# id from Notifications WHERE IDType=14)"}

                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = SchedulerResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = SchedulerResultEnum.NoError)

                If bolRet Then
                    ' Notificamos el cambio al servidor
                    bolRet = roDailyCoverage.Recalculate(roDailyCoverage.RecalculateTaskType.Update_All, Me.oState)
                End If

                If bolRet And _Audit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tDailyCoverage, strGroup & ": " & Me.xCoverageDate.Date & ": " & strAssignment, Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function AuditObjectName() As String

            Dim strRet As String = ""

            ' Obtenemos el nombre del grupo
            Dim oGroupState As New Group.roGroupState
            roBusinessState.CopyTo(Me.oState, oGroupState)
            Dim oGroup As New Group.roGroup(Me.intIDGroup, oGroupState)
            ' Obtenemos el nombre del puesto asignado
            Dim oAssignmentState As New Assignment.roAssignmentState
            roBusinessState.CopyTo(Me.oState, oAssignmentState)
            Dim oAssignment As New Assignment.roAssignment(Me.intIDAssignment, oAssignmentState)
            roBusinessState.CopyTo(oAssignmentState, Me.oState)

            strRet = oGroup.Name & "-" & Me.xCoverageDate.ToString(oState.Language.GetShortDateFormat) & "-" & oAssignment.Name

            Return strRet

        End Function

        ''' <summary>
        ''' Obtiene el detalle de empleados planificados para esta covertura (grupo, fecha y puesto)
        ''' </summary>
        ''' <param name="_Order"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetPlannedDetail(Optional ByVal _Order As String = "EmployeeName ASC") As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String
                strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName, DailySchedule.IDAssignment, Assignments.Name AS 'AssignmentName', DailySchedule.IDShift1, Shifts.Name AS 'ShiftName', sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.GroupName, EmployeeAssignments.Suitability, ISNULL(ShiftAssignments.Coverage,0)*100 AS 'Coverage', 0.000 AS 'Cost', EmployeeAssignments.Suitability * ISNULL(ShiftAssignments.Coverage,0)*100 AS 'Points', '' as ConceptValue " &
                         "FROM sysrovwAllEmployeeGroups INNER JOIN DailySchedule " &
                                "ON sysrovwAllEmployeeGroups.IDEmployee = DailySchedule.IDEmployee " &
                                "INNER JOIN DailyCoverage ON DailySchedule.Date = DailyCoverage.Date AND DailySchedule.IDAssignment = DailyCoverage.IDAssignment " &
                                "INNER JOIN Assignments ON DailyCoverage.IDAssignment = Assignments.ID " &
                                "LEFT JOIN Shifts ON DailySchedule.IDShift1 = Shifts.ID " &
                                "INNER JOIN EmployeeAssignments ON DailySchedule.IDEmployee = EmployeeAssignments.IDEmployee AND DailySchedule.IDAssignment = EmployeeAssignments.IDAssignment " &
                                "LEFT JOIN ShiftAssignments ON DailySchedule.IDShift1 = ShiftAssignments.IDShift AND DailySchedule.IDAssignment = ShiftAssignments.IDAssignment " &
                         "WHERE (sysrovwAllEmployeeGroups.Path = (@SELECT# [path] FROM Groups WHERE ID = " & Me.IDGroup.ToString & ") OR sysrovwAllEmployeeGroups.Path LIKE (@SELECT# [path] FROM Groups WHERE ID = " & Me.IDGroup.ToString & ") + '\%') AND " &
                               "BeginDate <= " & Any2Time(Me.CoverageDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(Me.CoverageDate).SQLSmallDateTime & " AND " &
                               "DailySchedule.Date = " & Any2Time(Me.CoverageDate).SQLSmallDateTime & " AND " &
                               "DailyCoverage.IDGroup = " & Me.IDGroup.ToString & " AND DailyCoverage.IDAssignment = " & Me.IDAssignment.ToString & " "
                If _Order <> "" Then
                    strSQL &= " ORDER BY " & _Order
                End If

                tbRet = CreateDataTable(strSQL, )

                ' Obtenemos el campo a utilizar para el coste del empleado
                Dim oParameters As New roParameters("OPTIONS", True)

                Dim strCostField As String = roTypes.Any2String(oParameters.Parameter(Parameters.EmployeeFieldCost))
                strCostField = strCostField.Replace("USR_", "")

                Dim intIDConcept As Integer = 0
                strSQL = "@SELECT# isnull(ID, 0) FROM Concepts WHERE Description like '%#HRS%' "
                intIDConcept = roTypes.Any2Integer(ExecuteScalar(strSQL))

                ' Obtenemos el coste de cada empleado y el saldo del empleado
                For Each dRow As DataRow In tbRet.Rows

                    If strCostField.Length > 0 Then
                        Dim EmployeeUserField As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(dRow("IDEmployee")), strCostField, Me.CoverageDate.Date, New roUserFieldState(oState.IDPassport))

                        If Not IsDBNull(EmployeeUserField.FieldValue) And Not EmployeeUserField.FieldValue Is Nothing Then
                            dRow("Cost") = Any2Double(CStr(EmployeeUserField.FieldValue).Replace(".", roConversions.GetDecimalDigitFormat))
                            dRow.AcceptChanges()
                        End If
                    End If

                    If intIDConcept > 0 Then
                        'ConceptValue
                        Dim oEmployeeState As New Employee.roEmployeeState(oState.IDPassport)
                        Dim oEmployeesConcepttb As DataTable = Concept.roConcept.GetAnualAccruals(roTypes.Any2Integer(dRow("IDEmployee")), Me.CoverageDate.Date, oEmployeeState, , intIDConcept)
                        If oEmployeesConcepttb IsNot Nothing Then
                            For Each oRow As DataRow In oEmployeesConcepttb.Rows
                                dRow("ConceptValue") = oRow("TotalFormat")
                                dRow.AcceptChanges()
                            Next
                        End If
                    End If
                Next
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetPlannedDetail")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetPlannedDetail")
            Finally

            End Try

            Return tbRet

        End Function

#Region "Helper methods"

        Public Shared Function GetDailyCoverageAssignments(ByVal _IDGroup As Integer, ByVal _CoverageDate As Date, ByVal _State As roSchedulerState, Optional ByVal _Audit As Boolean = False) As Generic.List(Of roDailyCoverageAssignment)

            Dim oRet As New Generic.List(Of roDailyCoverageAssignment)

            Try

                Dim strSQL As String = "@SELECT# DailyCoverage.* " &
                                       "FROM DailyCoverage INNER JOIN Assignments " &
                                                "ON DailyCoverage.IDAssignment = Assignments.ID " &
                                       "WHERE DailyCoverage.IDGroup = " & _IDGroup.ToString & " AND " &
                                             "DailyCoverage.Date = " & Any2Time(_CoverageDate).SQLSmallDateTime & " " &
                                       "ORDER BY Assignments.Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roDailyCoverageAssignment(oRow, _State))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetDailyCoverageAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetDailyCoverageAssignments")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetDailyCoverageAssignmentsDataTable(ByVal _IDGroup As Integer, ByVal _CoverageDate As Date, ByVal _State As roSchedulerState, Optional ByVal _Audit As Boolean = False) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# DailyCoverage.* " &
                                       "FROM DailyCoverage INNER JOIN Assignments " &
                                                "ON DailyCoverage.IDAssignment = Assignments.ID " &
                                       "WHERE DailyCoverage.IDGroup = " & _IDGroup.ToString & " AND " &
                                             "DailyCoverage.Date = " & Any2Time(_CoverageDate).SQLSmallDateTime & " " &
                                       "ORDER BY Assignments.Name"
                tbRet = CreateDataTable(strSQL, )
                If tbRet IsNot Nothing Then

                    If _Audit Then
                        ' ***

                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetDailyCoverageAssignmentsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetDailyCoverageAssignmentsDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function SaveTeoricDailyCoverageAssignments(ByVal _IDGroup As Integer, ByVal _CoverageDate As Date, ByVal _DailyAssignments As Generic.List(Of roDailyCoverageAssignment), ByVal _State As roSchedulerState, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                If ValidateDailyCoverageAssignments(_IDGroup, _CoverageDate, _DailyAssignments, _State) Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    ' Obtenemos los puestos asignados actualmente
                    Dim lstOldDailyAssignments As Generic.List(Of roDailyCoverageAssignment) = roDailyCoverageAssignment.GetDailyCoverageAssignments(_IDGroup, _CoverageDate, _State)

                    Dim IDAssignmentsSaved As New Generic.List(Of Integer)
                    If _DailyAssignments IsNot Nothing AndAlso _DailyAssignments.Count > 0 Then
                        For Each oDailyAssignment As roDailyCoverageAssignment In _DailyAssignments
                            oDailyAssignment.oState = _State
                            oDailyAssignment.IDGroup = _IDGroup
                            oDailyAssignment.CoverageDate = _CoverageDate
                            bolRet = oDailyAssignment.SaveTeoric(bAudit, False)
                            If Not bolRet Then Exit For
                            IDAssignmentsSaved.Add(oDailyAssignment.IDAssignment)
                        Next
                    Else
                        bolRet = True
                    End If

                    If bolRet Then
                        ' Borramos los puestos asignados de la tabla que no esten en la lista
                        For Each oDailyAssignment As roDailyCoverageAssignment In lstOldDailyAssignments
                            If ExistDailyAssignmentInList(_DailyAssignments, oDailyAssignment) Is Nothing Then
                                bolRet = oDailyAssignment.Delete(bAudit)
                                If Not bolRet Then Exit For
                            End If
                        Next
                    End If

                    If bolRet Then
                        roDailyCoverage.Recalculate(roDailyCoverage.RecalculateTaskType.Update_All, _State, , , _CoverageDate)
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::SaveTeoricDailyCoverageAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::SaveTeoricDailyCoverageAssignments")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateDailyCoverageAssignments(ByVal _IDGroup As Integer, ByVal _CoverageDate As Date, ByVal _DailyAssignments As Generic.List(Of roDailyCoverageAssignment), ByVal _State As roSchedulerState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Obtengo la fecha de congelación
                Dim xFreezingDate As Date = Nothing
                Dim bolCanSave As Boolean = False
                Dim oParameters As New roParameters("OPTIONS", True)
                If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing AndAlso IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                    xFreezingDate = CDate(oParameters.Parameter(Parameters.FirstDate))
                End If

                bolRet = True

                If xFreezingDate <> Nothing Then ' Si hay definida una fecha de congelación
                    ' Miro si la fecha del valor del campo está dentro de periodo de congelación
                    bolCanSave = (_CoverageDate > xFreezingDate)
                    If Not bolCanSave Then
                        bolRet = False
                        _State.Result = SchedulerResultEnum.DailyScheduleLockedDay
                    End If
                End If

                Dim oList As New Generic.List(Of roDailyCoverageAssignment)

                ' Verificamos que la lista de puestos sea correcta.
                If bolRet Then
                    For Each oDailyAssignment As roDailyCoverageAssignment In _DailyAssignments
                        If ExistDailyAssignmentInList(oList, oDailyAssignment) IsNot Nothing Then
                            bolRet = False
                            _State.Result = SchedulerResultEnum.DailyCoverageAssignmentRepited
                            Exit For
                        Else
                            oList.Add(oDailyAssignment)
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::ValidateDailyCoverageAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::ValidateDailyCoverageAssignments")
            End Try

            Return bolRet

        End Function

        Private Shared Function ExistDailyAssignmentInList(ByVal lstDailyAssignments As Generic.List(Of roDailyCoverageAssignment), ByVal oDailyAssignment As roDailyCoverageAssignment) As roDailyCoverageAssignment

            Dim oRet As roDailyCoverageAssignment = Nothing

            If lstDailyAssignments IsNot Nothing Then

                For Each oItem As roDailyCoverageAssignment In lstDailyAssignments
                    If oItem.IDGroup = oDailyAssignment.IDGroup And
                       oItem.CoverageDate = oDailyAssignment.CoverageDate And
                       oItem.IDAssignment = oDailyAssignment.IDAssignment Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

        Public Shared Function GetDailyCoverageAssignmentFromEmployeeDate(ByVal _IDEmployee As String, ByVal _CoverageDate As Date, ByVal _State As roSchedulerState) As roDailyCoverageAssignment

            Dim oRet As roDailyCoverageAssignment = Nothing

            Try

                Dim strSQL As String
                strSQL = "@DECLARE# @Path nvarchar(4000) " &
                         "@SELECT# @Path = sysrovwAllEmployeeGroups.Path " &
                         "FROM sysrovwAllEmployeeGroups " &
                         "WHERE sysrovwAllEmployeeGroups.IDEmployee = " & _IDEmployee.ToString & " AND " &
                               "sysrovwAllEmployeeGroups.BeginDate <= " & Any2Time(_CoverageDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & Any2Time(_CoverageDate).SQLSmallDateTime & " " &
                         "@SELECT# DailyCoverage.IDGroup, DailyCoverage.IDAssignment " &
                         "FROM DailyCoverage INNER JOIN DailySchedule " &
                                    "ON DailyCoverage.IDAssignment = DailySchedule.IDAssignment AND DailyCoverage.Date = DailySchedule.Date " &
                                    "INNER JOIN Groups ON Groups.ID = DailyCoverage.IDGroup	" &
                         "WHERE DailySchedule.IDEmployee = " & _IDEmployee.ToString & " AND DailySchedule.Date = " & Any2Time(_CoverageDate).SQLSmallDateTime & " AND " &
                               "DailyCoverage.IDGroup IN (@SELECT# * FROM SplitInt(@Path, '\')) " &
                         "ORDER BY Groups.Path DESC"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    oRet = New roDailyCoverageAssignment(tb.Rows(0).Item("IDGroup"), _CoverageDate, tb.Rows(0).Item("IDAssignment"), _State, False)

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetDailyCoverageAssignmentFromEmployeeDate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::GetDailyCoverageAssignmentFromEmployeeDate")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

End Namespace