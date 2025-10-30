Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTPortal

    Public Class AccrualsHelper

        Public Shared Function GetEmployeeAccrualsSummary(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal sDate As DateTime, ByVal oEmpState As Employee.roEmployeeState) As AccrualsSummary
            Dim lrret As New AccrualsSummary

            Try

                lrret.Status = ErrorCodes.OK

                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                roBusinessState.CopyTo(oEmpState, oReqState)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.Schedule.ScheduleAccruals Then
                    lrret.ScheduleSummary = roEmployeeSummaryManager.GetEmployeeAccrualSummary(idEmployee, sDate, oEmpState)
                Else
                    lrret.ScheduleSummary = New roEmployeeAccrualsSummary
                End If

                If oPermList.Schedule.ProductivAccruals Then
                    lrret.ProductiVSummary = roEmployeeSummaryManager.GetEmployeeAccrualProductivSummary(idEmployee, sDate, oEmpState)
                Else
                    lrret.ProductiVSummary = New roEmployeeAccrualsProductivSummary
                End If

                If oPermList.Schedule.QuerySchedule Then
                    lrret.HolidaysSummary = roEmployeeSummaryManager.GetEmployeeHolidaysSummary(idEmployee, oEmpState)
                Else
                    lrret.HolidaysSummary = New roEmployeeHolidaysSummary
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.ScheduleSummary = New roEmployeeAccrualsSummary

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::AccrualsHelper::GetEmployeeAccrualsSummary")
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeAccruals(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal sDate As DateTime, ByVal bOnlyDiary As Boolean, ByVal oEmpState As Employee.roEmployeeState) As EmployeeAccruals
            Dim lrret As New EmployeeAccruals

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)
                If oPermList.Schedule.ProductivAccruals OrElse oPermList.Schedule.ScheduleAccruals Then
                    Dim oServerLicense As New roServerLicense()
                    Dim bHRScheduling As Boolean = oServerLicense.FeatureIsInstalled("Feature\HRScheduling")

                    Dim tmpList As New Generic.List(Of EmployeeAccrualGroup)
                    Dim tmpAccList As New Generic.List(Of EmployeeAccrualValue)

                    If oPermList.Schedule.ScheduleAccruals Then
                        Dim dtDailyAccruals As DataTable = Concept.roConcept.GetDailyAccruals(idEmployee, sDate, oEmpState, False)
                        Dim dtWeeklyAccruals As DataTable = Concept.roConcept.GetWeekAccruals(idEmployee, sDate, oEmpState, True)
                        Dim dtMonthAccruals As DataTable = Concept.roConcept.GetMonthAccruals(idEmployee, sDate, oEmpState, True)
                        Dim dtAnnualAccruals As DataTable = Concept.roConcept.GetAnualAccruals(idEmployee, sDate, oEmpState, True)
                        Dim dtContractAccruals As DataTable = Concept.roConcept.GetContractAccruals(idEmployee, sDate, oEmpState, True)

                        If (dtDailyAccruals IsNot Nothing AndAlso dtDailyAccruals.Rows.Count > 0) Then
                            tmpList.Clear()
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "daily"

                            For Each oRow As DataRow In dtDailyAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDConcept"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("ValueFormat"))

                                tmpAccList.Add(oElem)
                            Next
                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)

                        End If

                        If (Not bOnlyDiary AndAlso dtWeeklyAccruals IsNot Nothing AndAlso dtWeeklyAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "week"

                            For Each oRow As DataRow In dtWeeklyAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDConcept"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next
                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        If (Not bOnlyDiary AndAlso dtMonthAccruals IsNot Nothing AndAlso dtMonthAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "month"

                            For Each oRow As DataRow In dtMonthAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDConcept"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next

                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        If (Not bOnlyDiary AndAlso dtAnnualAccruals IsNot Nothing AndAlso dtAnnualAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "year"

                            For Each oRow As DataRow In dtAnnualAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDConcept"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next

                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        If (Not bOnlyDiary AndAlso dtContractAccruals IsNot Nothing AndAlso dtContractAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "contract"

                            For Each oRow As DataRow In dtContractAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDConcept"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next

                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        lrret.ScheduleAccruals = tmpList.ToArray
                    Else
                        lrret.ScheduleAccruals = {}
                    End If

                    If oPermList.Schedule.ProductivAccruals Then
                        Dim dtDailyTaskAccruals As DataTable = Concept.roConcept.GetDailyTaskAccruals(idEmployee, sDate, oEmpState)
                        Dim dtWeeklyTaskAccruals As DataTable = Concept.roConcept.GetWeekTaskAccruals(idEmployee, sDate, oEmpState, True)
                        Dim dtMonthTaskAccruals As DataTable = Concept.roConcept.GetMonthTaskAccruals(idEmployee, sDate, oEmpState, True)
                        Dim dtAnnualTaskAccruals As DataTable = Concept.roConcept.GetTaskAccruals(idEmployee, sDate, oEmpState, True)
                        Dim dtContractTaskAccruals As DataTable = Concept.roConcept.GetContractTaskAccruals(idEmployee, sDate, oEmpState, True)

                        tmpList.Clear()

                        If (dtDailyTaskAccruals IsNot Nothing AndAlso dtDailyTaskAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "daily"

                            For Each oRow As DataRow In dtDailyTaskAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDTask"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("ValueFormat"))

                                tmpAccList.Add(oElem)
                            Next

                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)

                        End If

                        If (bOnlyDiary AndAlso dtWeeklyTaskAccruals IsNot Nothing AndAlso dtWeeklyTaskAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "week"

                            For Each oRow As DataRow In dtWeeklyTaskAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDTask"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next
                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        If (bOnlyDiary AndAlso dtMonthTaskAccruals IsNot Nothing AndAlso dtMonthTaskAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "month"

                            For Each oRow As DataRow In dtMonthTaskAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDTask"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next
                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        If (bOnlyDiary AndAlso dtAnnualTaskAccruals IsNot Nothing AndAlso dtAnnualTaskAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "year"

                            For Each oRow As DataRow In dtAnnualTaskAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDTask"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next

                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        If (Not bOnlyDiary AndAlso dtContractTaskAccruals IsNot Nothing AndAlso dtContractTaskAccruals.Rows.Count > 0) Then
                            tmpAccList.Clear()

                            Dim oGroup As New EmployeeAccrualGroup
                            oGroup.key = "contract"

                            For Each oRow As DataRow In dtContractTaskAccruals.Rows
                                Dim oElem As New EmployeeAccrualValue
                                oElem.IdAccrual = roTypes.Any2Integer(oRow("IDTask"))
                                oElem.Name = roTypes.Any2String(oRow("Name"))
                                oElem.Value = roTypes.Any2String(oRow("TotalFormat"))
                                tmpAccList.Add(oElem)
                            Next

                            oGroup.items = tmpAccList.ToArray
                            tmpList.Add(oGroup)
                        End If

                        lrret.TaskAccruals = tmpList.ToArray
                    Else
                        lrret.TaskAccruals = {}
                    End If

                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                    lrret.ScheduleAccruals = {}
                    lrret.TaskAccruals = {}
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                lrret.ScheduleAccruals = {}
                lrret.TaskAccruals = {}

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::AccrualsHelper::GetEmployeeAccruals")
            End Try

            Return lrret
        End Function

    End Class

End Namespace