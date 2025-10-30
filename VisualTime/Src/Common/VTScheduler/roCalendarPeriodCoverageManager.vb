Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTCalendar

    Public Class roCalendarPeriodCoverageManager
        Private oState As roCalendarRowState = Nothing

        Public Sub New()
            Me.oState = New roCalendarRowState()
        End Sub

        Public Sub New(ByVal _State As roCalendarRowState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal xDate As DateTime, ByVal intIDEmployee As Integer, Optional ByVal bAudit As Boolean = False) As roCalendarRow

            Dim bolRet As roCalendarRow = Nothing

            Try

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", "", "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tCalendarRow, "", tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(ByRef oCalendarRow As roCalendarRow, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                If bolRet AndAlso bAudit Then

                    bolRet = True
                    ' Auditamos
                    Dim tbAuditParameters As DataTable = roAudit.CreateParametersTable()
                    roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    Dim strObjectName As String
                    If oAuditAction = Audit.Action.aInsert Then
                        strObjectName = oAuditDataNew("Name")
                    Else
                        strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                    End If
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tCalendarRow, strObjectName, tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal oCalendarRow As roCalendarRow, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Try

                If bolRet Then
                    Dim DeleteQuerys As String() = {""}

                    For Each strSQL As String In DeleteQuerys
                        If strSQL <> String.Empty Then bolRet = ExecuteSql(strSQL)

                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tCalendarRow, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Delete")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oCalendarRow As roCalendarRow, ByVal bolIsNew As Boolean) As Boolean

            Dim bolRet As Boolean = True

            Try
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helpers"

        Public Shared Function LoadCoverageByCalendar(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _strEmployeeFilter As String, ByRef oState As roCalendarPeriodCoverageState, Optional ByVal lstAssignments As String = "") As roCalendarCoverageDay()
            ' Llenamos las coberturas del calendario
            Dim oRet As New Generic.List(Of roCalendarCoverageDay)
            Dim bolRet As Boolean = False
            Dim intIDGroup As Double
            Dim strGroups As String = "-1"

            Try

                ' Si no tiene permisos de lectura como minimo no mostramos nada
                Dim oPermissionPassport As Permission = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Scheduler", "U")

                If oPermissionPassport >= Permission.Read Then
                    ' 00. Obtenemos los grupos sobre los que tiene permisos el Supervisor y las excepciones sobre empleados
                    Dim oStateGroup As New Group.roGroupState(oState.IDPassport)
                    Dim tbGroups As DataTable = Group.roGroup.GetGroups("Calendar", "U", oStateGroup).Tables(0)
                    Dim oGroups As DataRow() = Nothing

                    ' 01. Obtenemos el grupo seleccionado
                    intIDGroup = roTypes.Any2Double(_strEmployeeFilter.Split("¬")(0).Substring(1))

                    oGroups = tbGroups.Select("[ID] = " & intIDGroup.ToString)
                    If oGroups.Length > 0 Then
                        strGroups = intIDGroup.ToString
                    End If
                End If

                ' Obtenemos todas las fechas del periodo seleccionado y marcamos obtenemos los datos de las coberturas
                Dim strQuery As String = "With alldays As ( "
                strQuery &= " @SELECT# " & roTypes.Any2Time(_FirstDay).SQLSmallDateTime & " AS dt "
                strQuery &= " UNION ALL  "
                strQuery &= " @SELECT# DateAdd(dd, 1, dt) "
                strQuery &= " From alldays s "
                strQuery &= " Where DateAdd(dd, 1, dt) <= " & roTypes.Any2Time(_LastDay).SQLSmallDateTime & ") "
                strQuery &= " @SELECT# distinct alldays.dt, groups.ID as IDGroup, (@SELECT# count(*) From DailyCoverage Where DailyCoverage.Date = alldays.dt AND DailyCoverage.IDGroup = Groups.ID "
                If lstAssignments IsNot Nothing AndAlso lstAssignments.Length > 0 Then
                    strQuery &= " AND IDAssignment IN(" & lstAssignments & ") "
                End If

                strQuery &= " ) As assignments FROM alldays, Groups WHERE  Groups.ID In ( " & strGroups & ") ORDER BY alldays.dt"
                strQuery &= " OPTION (maxrecursion 0) "

                Dim dTbl As System.Data.DataTable = CreateDataTable(strQuery)
                If dTbl IsNot Nothing Then
                    For Each oRowCov As DataRow In dTbl.Rows
                        ' Para cada dia con cobertura
                        Dim oCalendarCoverageDay As New roCalendarCoverageDay
                        oCalendarCoverageDay.IDGroup = intIDGroup
                        oCalendarCoverageDay.CoverageDate = oRowCov("dt")
                        oCalendarCoverageDay.ActualStatus = CalendarCoverageDayStatus.WITHOUTCOVERAGE
                        oCalendarCoverageDay.PlannedStatus = CalendarCoverageDayStatus.WITHOUTCOVERAGE

                        oCalendarCoverageDay.ActualProcessed = True
                        oCalendarCoverageDay.PlannedProcessed = True

                        If roTypes.Any2Double(oRowCov("assignments")) > 0 Then
                            oCalendarCoverageDay.ActualStatus = CalendarCoverageDayStatus.OK
                            oCalendarCoverageDay.PlannedStatus = CalendarCoverageDayStatus.OK

                            strQuery = "@SELECT# Name, ShortName, IDGroup, Date, IDAssignment, ExpectedCoverage, PlannedCoverage, ActualCoverage,PlannedStatus,ActualStatus FROM DailyCoverage , Assignments WHERE Assignments.id = DailyCoverage.IDAssignment"
                            strQuery &= " And "
                            strQuery &= " IDGroup = " & roTypes.Any2Double(oRowCov("IDGroup")).ToString
                            strQuery &= " And Date = " & roTypes.Any2Time(oRowCov("dt")).SQLSmallDateTime
                            If Not lstAssignments Is Nothing AndAlso lstAssignments.Length > 0 Then
                                strQuery &= " And IDAssignment IN(" & lstAssignments & ") "
                            End If

                            strQuery &= " ORDER BY IDAssignment"
                            Dim dTblRow As System.Data.DataTable = CreateDataTable(strQuery)
                            Dim oCalendarAssignmentCoverageDayList As New Generic.List(Of roCalendarAssignmentCoverageDay)

                            'Cargar las dotaciones del dia
                            If dTblRow IsNot Nothing Then
                                For Each oRowAss As DataRow In dTblRow.Rows
                                    Dim oCalendarAssignmentCoverageDay As New roCalendarAssignmentCoverageDay
                                    oCalendarAssignmentCoverageDay.ID = oRowAss("IDAssignment")
                                    oCalendarAssignmentCoverageDay.Expected = roTypes.Any2Double(oRowAss("ExpectedCoverage"))
                                    oCalendarAssignmentCoverageDay.Planned = roTypes.Any2Double(oRowAss("PlannedCoverage"))
                                    oCalendarAssignmentCoverageDay.Actual = roTypes.Any2Double(oRowAss("ActualCoverage"))
                                    oCalendarAssignmentCoverageDay.Name = roTypes.Any2String(oRowAss("Name"))
                                    oCalendarAssignmentCoverageDay.ShortName = roTypes.Any2String(oRowAss("ShortName"))

                                    oCalendarAssignmentCoverageDayList.Add(oCalendarAssignmentCoverageDay)

                                    ' Guardamos el estado actual de la cobertura
                                    If oCalendarAssignmentCoverageDay.Expected > oCalendarAssignmentCoverageDay.Planned Then oCalendarCoverageDay.PlannedStatus = CalendarCoverageDayStatus.KO
                                    If oCalendarAssignmentCoverageDay.Expected < oCalendarAssignmentCoverageDay.Planned AndAlso oCalendarCoverageDay.PlannedStatus = CalendarCoverageDayStatus.OK Then oCalendarCoverageDay.PlannedStatus = CalendarCoverageDayStatus.OVERLOAD

                                    If oCalendarAssignmentCoverageDay.Expected > oCalendarAssignmentCoverageDay.Actual Then oCalendarCoverageDay.ActualStatus = CalendarCoverageDayStatus.KO
                                    If oCalendarAssignmentCoverageDay.Expected < oCalendarAssignmentCoverageDay.Actual AndAlso oCalendarCoverageDay.ActualStatus = CalendarCoverageDayStatus.OK Then oCalendarCoverageDay.ActualStatus = CalendarCoverageDayStatus.OVERLOAD

                                    If roTypes.Any2Double(oRowAss("ActualStatus")) <> 100 Then oCalendarCoverageDay.ActualProcessed = False
                                    If roTypes.Any2Double(oRowAss("PlannedStatus")) <> 100 Then oCalendarCoverageDay.PlannedProcessed = False
                                Next
                            End If
                            oCalendarCoverageDay.AssignmentData = oCalendarAssignmentCoverageDayList.ToArray
                        End If
                        oRet.Add(oCalendarCoverageDay)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::LoadCoverageByCalendar")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCalendarPeriodCoverageManager::LoadCoverageByCalendar")
            End Try

            Return oRet.ToArray

        End Function

#End Region

    End Class

End Namespace