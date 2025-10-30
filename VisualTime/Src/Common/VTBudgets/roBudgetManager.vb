Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.Base.VTCalendar
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTBudgets

    Public Class roBudgetManager
        Private oState As roBudgetState = Nothing
        Private oShiftCache As New Hashtable

        Public ReadOnly Property State As roBudgetState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roBudgetState()
        End Sub

        Public Sub New(ByVal _State As roBudgetState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strNodeFilter As String, ByVal TypeView As BudgetView, ByVal DetailLevel As BudgetDetailLevel, Optional ByVal bAudit As Boolean = False, Optional ByVal _IDProductiveUnit As Integer = -1, Optional ByVal strProductiveUnitFilter As String = "", Optional ByVal bolShowIndictments As Boolean = False) As roBudget

            Dim bolRet As roBudget = Nothing

            Try

                ' Validamos si tiene licencia de HRScheduling
                Dim oLicense As New roServerLicense
                Dim bolLicenseHRScheduling As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                bolRet = New roBudget
                bolRet.FirstDay = xFirstDay
                bolRet.LastDay = xLastDay

                Dim oParam As Object = New roParameters("OPTIONS", True).Parameter(Parameters.FirstDate)
                If oParam IsNot Nothing AndAlso IsDate(oParam) Then
                    bolRet.FreezingDate = roTypes.Any2DateTime(CDate(oParam))
                Else
                    bolRet.FreezingDate = New DateTime(1970, 1, 1, 0, 0, 0)
                End If

                ' Cargamos las filas de la cabecera del presupuesto
                bolRet.BudgetHeader = GenerateHeaderCell(strNodeFilter, DetailLevel, xFirstDay, xLastDay)

                ' Cargamos las filas del presupuesto
                Dim oBudgetRowState As New roBudgetRowState(oState.IDPassport)
                bolRet.BudgetData = roBudgetRowManager.LoadRowsByBudget(xFirstDay, xLastDay, strNodeFilter, oBudgetRowState, TypeView, DetailLevel, bolLicenseHRScheduling, _IDProductiveUnit, strProductiveUnitFilter)

                ' Cargamos los indicadores de planificacion en caso necesario
                If bolShowIndictments Then AddIndictmentsToBudget(bolRet, xFirstDay, xLastDay)

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", bolRet.FirstDay, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tBudget, bolRet.FirstDay, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::Load")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByRef oBudget As roBudget, Optional ByVal bAudit As Boolean = False, Optional ByVal bolCheckPermission As Boolean = False, Optional ByVal bolAssignEmployees As Boolean = False) As roBudgetResult
            Dim bolRet As Boolean = False
            Dim oRet As New roBudgetResult
            Dim oBudgetResultDays As New Generic.List(Of roBudgetDataDayError)
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.oState.Result = ProductiveUnitResultEnum.NoError
                oRet.Status = BudgetStatusEnum.KO

                ' Validamos si tiene licencia de HRScheduling
                Dim oLicense As New roServerLicense
                Dim bolLicenseHRScheduling As Boolean = oLicense.FeatureIsInstalled("Feature\HRScheduling")

                Dim oParameters As roParameters
                oParameters = New roParameters("OPTIONS", True)

                bolRet = Me.Validate(oBudget, oBudgetResultDays, bolLicenseHRScheduling, bolCheckPermission)

                If bolRet Then

                    bolRet = True
                    oRet.Status = BudgetStatusEnum.OK

                    Dim oBudgetRowState As New roBudgetRowPeriodDataState(oState.IDPassport)
                    Dim oBudgetRowPeriodData As New roBudgetRowPeriodDataManager(oBudgetRowState)

                    ' Si los datos son correctos los guardamos
                    For Each oBudgetRow As roBudgetRow In oBudget.BudgetData
                        ' Para cada fila nueva o modificada, guardamos las celdas que se hayan cambiado o agregado
                        If oBudgetRow.RowState = BudgetRowState.NewRow Or oBudgetRow.RowState = BudgetRowState.UpdateRow Then
                            If oBudgetRow.ProductiveUnitData IsNot Nothing AndAlso oBudgetRow.ProductiveUnitData.ProductiveUnit IsNot Nothing AndAlso oBudgetRow.ProductiveUnitData.ProductiveUnit.ID > 0 Then
                                If oBudgetRow.PeriodData IsNot Nothing Then
                                    bolRet = oBudgetRowPeriodData.Save(oBudgetRow.ProductiveUnitData, oBudgetRow.PeriodData, oBudgetResultDays, bolLicenseHRScheduling, True, False, True, oParameters,, bolAssignEmployees)
                                    If Not bolRet Then
                                        oRet.Status = BudgetStatusEnum.KO
                                        Exit For
                                    End If
                                End If
                            End If
                        End If

                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tBudget, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::Save")
            Finally

                If Not bolRet Then
                    oRet.Status = BudgetStatusEnum.KO
                    oRet.BudgetDataResult = oBudgetResultDays.ToArray
                End If

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return oRet

        End Function

        Public Function Validate(ByVal oBudget As roBudget, ByRef oBudgetResultDays As Generic.List(Of roBudgetDataDayError), ByVal bolLicenseHRScheduling As Boolean, Optional ByVal bolCheckPermission As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Try

                Me.oState.Result = BudgetResultEnum.NoError

                'If oBudget.BudgetData IsNot Nothing Then
                '    Dim oParameters As roParameters
                '    oParameters = New roParameters("OPTIONS", True)

                '    Dim oBudgetRowState As New roBudgetRowPeriodDataState(oState.IDPassport)
                '    Dim oBudgetRowPeriodData As New roBudgetRowPeriodDataManager(oBudgetRowState)

                '    Dim strMsg As String = ""
                '    For Each oBudgetRow As roBudgetRow In oBudget.BudgetData
                '        ' Para cada fila, validamos si los datos son correctos
                '        If oBudgetRow.EmployeeData IsNot Nothing AndAlso oBudgetRow.EmployeeData.IDEmployee > 0 Then
                '            If oBudgetRow.PeriodData IsNot Nothing Then
                '                bolRet = oBudgetRowPeriodData.Validate(oBudgetRow.EmployeeData, oBudgetRow.PeriodData, strMsg, oParameters, oBudgetResultDays, bolLicenseHRScheduling, oTrans, bolCheckPermission)
                '                If Not bolRet Then
                '                    Me.oState.Result = roBudgetState.ResultEnum.InValidData
                '                    Me.oState.ErrorDetail &= strMsg
                '                End If
                '            End If
                '        End If
                '    Next

                '    ' Si hay algun valor erroneo, no seguimos
                '    If Me.oState.Result <> roBudgetState.ResultEnum.NoError Then
                '        bolRet = False
                '    End If
                'End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::Validate")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Function GenerateHeaderCell(ByVal IDNode As Integer, ByVal detailLevel As BudgetDetailLevel, ByVal xStartDate As DateTime, ByVal xFinishDate As DateTime) As roBudgetHeader
            Dim oBudgetHeader As New roBudgetHeader
            Dim tmpFirtsDay As DateTime = xStartDate
            Dim tmpEndDay As DateTime = xFinishDate

            oBudgetHeader.ProductiveUnitHeaderData = New roBudgetHeaderCell
            oBudgetHeader.ProductiveUnitHeaderData.Row1Text = Me.oState.Language.Translate("Budget.Header.ProductiveUnits", "") 'Unidades productivas
            oBudgetHeader.ProductiveUnitHeaderData.Row2Text = ""
            oBudgetHeader.ProductiveUnitHeaderData.BackColor = ""

            Dim tmplist As New Generic.List(Of roBudgetHeaderCell)

            ' Obtenemos los dias festivos asignados al nodo en caso necesario
            Dim tbFeastDays As DataTable = Nothing
            tbFeastDays = roBudgetManager.GetFeastDaysNode(IDNode, oState)

            Dim oRows() As DataRow = Nothing

            'Vista multiples dias
            If detailLevel = BudgetDetailLevel.Daily Or detailLevel = BudgetDetailLevel.Mode Then
                While tmpFirtsDay <= tmpEndDay

                    Dim tmpCell As New roBudgetHeaderCell
                    tmpCell.Row1Text = tmpFirtsDay.ToString("dddd", oState.Language.GetLanguageCulture)
                    tmpCell.Row2Text = tmpFirtsDay.ToString("dd/MM/yyyy")

                    oRows = tbFeastDays.Select("ScheduleDate = '" & Format(tmpFirtsDay, "yyyy/MM/dd") & "'")
                    If oRows.Length > 0 Then
                        ' Dia Festivo
                        tmpCell.FeastDay = True
                    End If

                    oState.Language.GetMonthAndDayDateFormat()
                    Dim existsAlert As Boolean = False

                    If Not existsAlert Then
                        If tmpFirtsDay.Date = DateTime.Now.Date Then
                            tmpCell.BackColor = "#44C57E" ' Verd suau
                        Else
                            If tmpFirtsDay.DayOfWeek = DayOfWeek.Sunday Then
                                tmpCell.BackColor = "#C0DEFD" 'Blau suau
                            Else
                                tmpCell.BackColor = "#F2F4F2" 'Blanc
                            End If
                        End If
                    Else
                        tmpCell.BackColor = "#FF9595" 'Vermell suau
                    End If

                    tmplist.Add(tmpCell)
                    tmpFirtsDay = tmpFirtsDay.AddDays(1)
                End While
                oBudgetHeader.PeriodHeaderData = tmplist.ToArray
            Else
                'Vista diaria
                tmpFirtsDay = tmpFirtsDay.AddDays(-1)
                tmpEndDay = tmpEndDay.AddDays(2)

                While tmpFirtsDay < tmpEndDay

                    Dim tmpCell As New roBudgetHeaderCell
                    tmpCell.Row1Text = tmpFirtsDay.ToString("dd/MM/yyyy")
                    tmpCell.Row2Text = tmpFirtsDay.ToString("HH:mm")

                    oRows = tbFeastDays.Select("ScheduleDate = '" & Format(tmpFirtsDay.Date, "yyyy/MM/dd") & "'")
                    If oRows.Length > 0 Then
                        ' Dia Festivo
                        tmpCell.FeastDay = True
                    End If

                    Dim existsAlert As Boolean = False

                    If Not existsAlert Then
                        If DateTime.Now >= tmpFirtsDay AndAlso DateTime.Now < tmpFirtsDay.AddMinutes(30) Then
                            tmpCell.BackColor = "#44C57E" ' Verd suau
                        Else
                            tmpCell.BackColor = "#F2F4F2" 'Blanc
                        End If
                    Else
                        tmpCell.BackColor = "#FF9595" 'Vermell suau
                    End If

                    tmplist.Add(tmpCell)
                    tmpFirtsDay = tmpFirtsDay.AddMinutes(30)
                End While
                oBudgetHeader.PeriodHeaderData = tmplist.ToArray
            End If

            Return oBudgetHeader
        End Function

#End Region

#Region "Helpers"

        Public Shared Function GetGroupsFromNode(ByVal _IDNode As Integer, ByRef oState As roBudgetState) As String

            Dim bolCloseTrans As Boolean = False
            Dim strEmployeefilter As String = String.Empty
            Dim bolret As Boolean = False

            Try
                ' OJO!! No modificar ,se utiliza en IDN APV de Rankings
                ' Obtenemos todos los grupos del nodo seleccionado
                Dim strSQL As String = "@SELECT# sysroSecurityNode_Groups.*  FROM sysroSecurityNode_Groups  " &
                                       "WHERE [IDSecurityNode] = " & _IDNode.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oGroup As DataRow In tb.Rows
                        strEmployeefilter += "A" & oGroup("IDGroup") & ","
                    Next
                End If

                ' Obtenemos todos los grupos de los nodos hijo
                strSQL = "@SELECT# * FROM sysroSecurityNodes " &
                                       "WHERE [IDParent] = " & _IDNode.ToString
                tb = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        strSQL = "@SELECT# sysroSecurityNode_Groups.*  FROM sysroSecurityNode_Groups  " &
                       "WHERE [IDSecurityNode] = " & oRow("ID").ToString
                        Dim tbaux As DataTable = CreateDataTable(strSQL)
                        If tbaux IsNot Nothing AndAlso tbaux.Rows.Count > 0 Then
                            For Each oGroup As DataRow In tbaux.Rows
                                strEmployeefilter += "A" & oGroup("IDGroup") & ","
                            Next
                        End If
                    Next
                End If


                ' Obtenemos todos los grupos del nodo seleccionado
                'Dim oNodeManager As New roSecurityNodeManager()
                'Dim oNode As roSecurityNode = oNodeManager.Load(_IDNode, True, True)
                'If oNode IsNot Nothing Then
                '    If oNode.Groups IsNot Nothing Then
                '        For Each oGroup As roSecurityNodeGroup In oNode.Groups
                '            strEmployeefilter += "A" & oGroup.IDGroup & ","
                '        Next
                '    End If

                '    If oNode.Children IsNot Nothing Then
                '        For Each oNodeChildren As roSecurityNode In oNode.Children
                '            strEmployeefilter += GetGroupsFromNode(oNodeChildren.ID, oState)
                '        Next
                '    End If
                'End If

                bolret = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::GetGroupsFromNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::GetGroupsFromNode")
            End Try

            Return strEmployeefilter

        End Function

        Public Shared Function GetEmployeeListFromNode(ByVal _IDNode As Integer, ByRef oState As roBudgetState, Optional ByVal _Day As DateTime = Nothing,
                                                   Optional ByVal _ApplySecurity As Boolean = True) As String
            Dim strRet As String = String.Empty
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                Dim strEmployeefilter As String = String.Empty
                Dim strEmployees As String = String.Empty

                ' Obtenemos todos los grupos del nodo seleccionado

                strEmployeefilter = GetGroupsFromNode(_IDNode, oState)

                ' Obtenemos la lista de empleados
                If strEmployeefilter.Length > 0 Then strEmployeefilter = strEmployeefilter.Substring(0, strEmployeefilter.Length - 1)
                If strEmployeefilter <> String.Empty Then
                    Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, If(_ApplySecurity, "Calendar.Scheduler", ""), "U", Permission.Read, strEmployeefilter, "11110", "", False, _Day, _Day)
                    strEmployees = String.Join(",", lstEmployees)
                End If

                strRet = strEmployees
                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeListFromNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeListFromNode")
            End Try

            Return strRet

        End Function

        Public Shared Function GetEmployeeListFromNode(ByVal _IDNode As Integer, ByRef oState As roBudgetState, Optional ByVal _DayStart As DateTime = Nothing, Optional ByVal _DayEnd As DateTime = Nothing) As String
            Dim strRet As String = String.Empty
            Dim bolRet As Boolean = False

            Try
                Dim strEmployeefilter As String = String.Empty
                Dim strEmployees As String = String.Empty

                ' Obtenemos todos los grupos del nodo seleccionado

                strEmployeefilter = GetGroupsFromNode(_IDNode, oState)

                ' Obtenemos la lista de empleados
                If strEmployeefilter.Length > 0 Then strEmployeefilter = strEmployeefilter.Substring(0, strEmployeefilter.Length - 1)
                If strEmployeefilter <> String.Empty Then
                    Dim lstEmployees As Generic.List(Of Integer) = roSelector.GetEmployeeList(oState.IDPassport, "Calendar.Scheduler", "U", Permission.Read, strEmployeefilter, "11110", "", False, _DayStart, _DayEnd)
                    strEmployees = String.Join(",", lstEmployees)
                End If

                strRet = strEmployees
                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeListFromNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeListFromNode")
            End Try

            Return strRet

        End Function

        Public Shared Function CurrentStatusEmployeesSummaryOnNode(ByVal _IDNode As Integer, ByVal _Day As DateTime, ByRef oState As roBudgetState,
                                                  Optional ByVal _strEmployeesNode As String = "") As roCurrentStatusEmployeesSummary
            Dim oRet As New roCurrentStatusEmployeesSummary
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try

                ' Obtenemos los empleados del NODO
                Dim strEmployees As String = _strEmployeesNode
                If strEmployees.Length = 0 Then strEmployees = roBudgetManager.GetEmployeeListFromNode(_IDNode, oState, _Day, True)

                If strEmployees.Length > 0 Then
                    Dim strEmployeesAssigned As String = "-1"

                    ' AUSENCIA PROLONGADA
                    Dim oEmployeesOnProgrammedAbsence As New List(Of roCurrentStatusEmployeesSummary_EmployeeDetail)
                    strSQL = "@SELECT# * FROM sysrovwCurrentEmployeeGroups where idemployee in ( @SELECT# DISTINCT IDEMPLOYEE from ProgrammedAbsences WHERE "
                    strSQL &= " ( ( (BeginDate >= " & roTypes.Any2Time(_Day).SQLSmallDateTime & " AND BeginDate <= " & roTypes.Any2Time(_Day).SQLSmallDateTime & ")"
                    strSQL &= " OR "
                    strSQL &= " ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & roTypes.Any2Time(_Day).SQLSmallDateTime & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & roTypes.Any2Time(_Day).SQLSmallDateTime & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & roTypes.Any2Time(_Day).SQLSmallDateTime & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & roTypes.Any2Time(_Day).SQLSmallDateTime & ") )"
                    strSQL &= " AND IDEmployee in(" & strEmployees & ")"
                    strSQL &= " AND IDEmployee in(@SELECT# distinct IDEmployee FROM EmployeeAssignments) )"
                    strSQL &= " Order by EmployeeName"
                    Dim tbX As DataTable = CreateDataTable(strSQL)
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            For Each orow As DataRow In tbX.Rows
                                Dim oCurrentStatusEmployeesSummary_EmployeeDetail As New roCurrentStatusEmployeesSummary_EmployeeDetail
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDEmployee = orow("IDEmployee")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.EmployeeName = orow("EmployeeName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDGroup = orow("IDGroup")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.GroupName = orow("GroupName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.FullGroupName = orow("FullGroupName")

                                oEmployeesOnProgrammedAbsence.Add(oCurrentStatusEmployeesSummary_EmployeeDetail)

                                strEmployeesAssigned += "," & orow("IDEmployee").ToString
                            Next

                            oRet.EmployeesOnProgrammedAbsence = oEmployeesOnProgrammedAbsence.ToArray
                        End If
                    End If

                    ' DE VACACIONES
                    Dim oEmployeesOnHolidays As New List(Of roCurrentStatusEmployeesSummary_EmployeeDetail)
                    strSQL = "@SELECT# * FROM sysrovwCurrentEmployeeGroups where idemployee in ( @SELECT# DISTINCT IDEMPLOYEE from DailySchedule WHERE "
                    strSQL &= " Date=" & roTypes.Any2Time(_Day).SQLSmallDateTime
                    strSQL &= " AND IsHolidays=1 "
                    strSQL &= " AND IDEmployee in(" & strEmployees & ")"
                    strSQL &= " AND IDEmployee NOT in(" & strEmployeesAssigned & ")"
                    strSQL &= " AND IDEmployee in(@SELECT# distinct IDEmployee FROM EmployeeAssignments) )"
                    strSQL &= " Order by EmployeeName"
                    tbX = CreateDataTable(strSQL)
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            For Each orow As DataRow In tbX.Rows
                                Dim oCurrentStatusEmployeesSummary_EmployeeDetail As New roCurrentStatusEmployeesSummary_EmployeeDetail
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDEmployee = orow("IDEmployee")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.EmployeeName = orow("EmployeeName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDGroup = orow("IDGroup")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.GroupName = orow("GroupName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.FullGroupName = orow("FullGroupName")

                                oEmployeesOnHolidays.Add(oCurrentStatusEmployeesSummary_EmployeeDetail)

                                strEmployeesAssigned += "," & orow("IDEmployee").ToString
                            Next

                            oRet.EmployeesOnHolidays = oEmployeesOnHolidays.ToArray
                        End If
                    End If

                    ' EN DESCANSO
                    Dim oEmployeesNoWorkingShift As New List(Of roCurrentStatusEmployeesSummary_EmployeeDetail)
                    strSQL = "@SELECT# * FROM sysrovwCurrentEmployeeGroups where idemployee in ( @SELECT# DISTINCT IDEMPLOYEE from DailySchedule WHERE "
                    strSQL &= " Date=" & roTypes.Any2Time(_Day).SQLSmallDateTime
                    strSQL &= " AND IDShift1 IN(@SELECT# ID FROM Shifts WHERE ExpectedWorkingHours=0 ) "
                    strSQL &= " AND IsHolidays =0"
                    strSQL &= " AND IDEmployee in(" & strEmployees & ")"
                    strSQL &= " AND IDEmployee NOT in(" & strEmployeesAssigned & ")"
                    strSQL &= " AND IDEmployee in(@SELECT# distinct IDEmployee FROM EmployeeAssignments) )"
                    strSQL &= " Order by EmployeeName"
                    tbX = CreateDataTable(strSQL)
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            For Each orow As DataRow In tbX.Rows
                                Dim oCurrentStatusEmployeesSummary_EmployeeDetail As New roCurrentStatusEmployeesSummary_EmployeeDetail
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDEmployee = orow("IDEmployee")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.EmployeeName = orow("EmployeeName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDGroup = orow("IDGroup")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.GroupName = orow("GroupName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.FullGroupName = orow("FullGroupName")

                                oEmployeesNoWorkingShift.Add(oCurrentStatusEmployeesSummary_EmployeeDetail)

                                strEmployeesAssigned += "," & orow("IDEmployee").ToString
                            Next

                            oRet.EmployeesNoWorkingShift = oEmployeesNoWorkingShift.ToArray
                        End If
                    End If

                    ' SIN PUESTO
                    Dim oEmployeesWithoutAssignment As New List(Of roCurrentStatusEmployeesSummary_EmployeeDetail)
                    strSQL = "@SELECT# * FROM sysrovwCurrentEmployeeGroups where idemployee in ( @SELECT# DISTINCT IDEMPLOYEE from DailySchedule WHERE "
                    strSQL &= " Date=" & roTypes.Any2Time(_Day).SQLSmallDateTime
                    strSQL &= " AND isnull(IDAssignment,0) = 0  "
                    strSQL &= " AND IDEmployee in(" & strEmployees & ")"
                    strSQL &= " AND IDEmployee NOT in(" & strEmployeesAssigned & ")"
                    strSQL &= " AND IDEmployee in(@SELECT# distinct IDEmployee FROM EmployeeAssignments) )"
                    strSQL &= " Order by EmployeeName"
                    tbX = CreateDataTable(strSQL)
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            For Each orow As DataRow In tbX.Rows
                                Dim oCurrentStatusEmployeesSummary_EmployeeDetail As New roCurrentStatusEmployeesSummary_EmployeeDetail
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDEmployee = orow("IDEmployee")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.EmployeeName = orow("EmployeeName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.IDGroup = orow("IDGroup")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.GroupName = orow("GroupName")
                                oCurrentStatusEmployeesSummary_EmployeeDetail.FullGroupName = orow("FullGroupName")
                                oEmployeesWithoutAssignment.Add(oCurrentStatusEmployeesSummary_EmployeeDetail)
                                strEmployeesAssigned += "," & orow("IDEmployee").ToString
                            Next

                            oRet.EmployeesWithoutAssignment = oEmployeesWithoutAssignment.ToArray
                        End If
                    End If
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::CurrentStatusEmployeesSummaryOnNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::CurrentStatusEmployeesSummaryOnNode")
            End Try

            Return oRet
        End Function

        Public Function AddBudgetRowData(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _intIDNode As Integer, ByVal _intIDProductiveUnit As Integer) As roBudget
            Dim oRet As roBudgetRow = Nothing
            Dim oBudget As roBudget = Nothing
            Dim bolRet As Boolean = False

            Try
                oBudget = New roBudget
                oBudget.FirstDay = _FirstDay
                oBudget.LastDay = _LastDay

                Dim oParameters As New roParameters("OPTIONS", True)
                Dim oParam As Object = oParameters.Parameter(Parameters.FirstDate)

                If oParam IsNot Nothing AndAlso IsDate(oParam) Then
                    oBudget.FreezingDate = roTypes.Any2DateTime(CDate(oParam))
                Else
                    oBudget.FreezingDate = New DateTime(1970, 1, 1, 0, 0, 0)
                End If

                oBudget.BudgetHeader = Nothing

                Dim oRowList As New Generic.List(Of roBudgetRow)

                ' Creamos la fila del presupuesto con la unidad productiva indicada
                oRet = New roBudgetRow
                oRet.RowState = BudgetRowState.NewRow

                ' Datos generales de la unidad productiva
                oRet.ProductiveUnitData = New roBudgetRowProductiveUnitData
                oRet.ProductiveUnitData.IDNode = _intIDNode
                oRet.ProductiveUnitData.NodeName = ""
                oRet.ProductiveUnitData.ProductiveUnit = New roProductiveUnit

                ' Obtenemos los permisos del Supervisor sobre la planificación
                Dim oPermissionPassport As Permission = Permission.None
                oPermissionPassport = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Scheduler", "U")

                oRet.ProductiveUnitData.Permission = oPermissionPassport

                Dim oProductiveUnitState As New roProductiveUnitState(oState.IDPassport)
                Dim oProductiveUnitManager As New roProductiveUnitManager(oProductiveUnitState)
                oRet.ProductiveUnitData.ProductiveUnit = oProductiveUnitManager.LoadProductiveUnit(_intIDProductiveUnit)

                ' Datos del periodo seleccionado
                Dim oBudgetRowPeriodDataState As New roBudgetRowPeriodDataState(oState.IDPassport)
                oRet.PeriodData = roBudgetRowPeriodDataManager.LoadCellsByBudget(_FirstDay, _LastDay, _intIDProductiveUnit, _intIDNode, oPermissionPassport, oParameters, BudgetView.Definition, BudgetDetailLevel.Daily, oBudgetRowPeriodDataState, True)

                oRowList.Add(oRet)

                ' Asignamos la fila al presupuesto
                oBudget.BudgetData = oRowList.ToArray

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::AddBudgetRowData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::AddBudgetRowData")
            End Try

            Return oBudget

        End Function

        Public Function RemoveBudgetRowData(ByVal _intIDNode As Integer, ByVal _intIDProductiveUnit As Integer) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                oState.Result = ProductiveUnitResultEnum.NoError

                ' Desasignamos los empleados planificados a ese Nodo/Unidad productiva
                Dim strSQL As String = "@UPDATE# DailySchedule WITH (ROWLOCK) SET IDDailyBudgetPosition=Null where IDDailyBudgetPosition in(@SELECT# ID FROM DailyBudget_Positions WHERE IDDailyBudget in(@SELECT# ID from  DailyBudgets WHERE IDNode=" & _intIDNode & " AND IDProductiveUnit=" & _intIDProductiveUnit & "))"
                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                If bolRet Then
                    ' Eliminamos las posiciones del Nodo/Unidad productiva
                    strSQL = "@DELETE# FROM DailyBudget_Positions WHERE IDDailyBudget in(@SELECT# ID from  DailyBudgets WHERE IDNode=" & _intIDNode & " AND IDProductiveUnit=" & _intIDProductiveUnit & ")"
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet Then
                    ' Eliminamos los modos diarias del Nodo/Unidad productiva
                    strSQL = "@DELETE# FROM DailyBudgets WHERE IDNode=" & _intIDNode & " AND IDProductiveUnit=" & _intIDProductiveUnit
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet Then
                    ' Eliminamos todas las notificaciones de cobertura insuficiente del nodo y unidad productiva relacionadas
                    strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (54)) AND Key2Numeric= " & _intIDNode & " AND Key5Numeric=" & _intIDProductiveUnit
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::RemoveBudgetRowData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::RemoveBudgetRowData")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function GetBudgetPeriodHourDefinition(ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime, ByVal _intIDNode As Integer, ByVal _intIDProductiveUnit As Integer,
                                          Optional ByVal bolShowIndictments As Boolean = False) As roBudget
            Dim oRet As roBudgetRow = Nothing
            Dim oBudget As roBudget = Nothing
            Dim bolRet As Boolean = False

            Try

                oBudget = New roBudget
                oBudget.FirstDay = _FirstDay
                oBudget.LastDay = _LastDay

                Dim oParameters As New roParameters("OPTIONS", True)
                Dim oParam As Object = oParameters.Parameter(Parameters.FirstDate)

                If oParam IsNot Nothing AndAlso IsDate(oParam) Then
                    oBudget.FreezingDate = roTypes.Any2DateTime(CDate(oParam))
                Else
                    oBudget.FreezingDate = New DateTime(1970, 1, 1, 0, 0, 0)
                End If

                oBudget.BudgetHeader = Nothing

                Dim oRowList As New Generic.List(Of roBudgetRow)

                ' Creamos la fila del presupuesto con la unidad productiva indicada
                oRet = New roBudgetRow
                oRet.RowState = BudgetRowState.NewRow

                ' Datos generales de la unidad productiva
                oRet.ProductiveUnitData = New roBudgetRowProductiveUnitData
                oRet.ProductiveUnitData.IDNode = _intIDNode
                oRet.ProductiveUnitData.NodeName = ""
                oRet.ProductiveUnitData.ProductiveUnit = New roProductiveUnit

                ' Obtenemos los permisos del Supervisor sobre la planificación
                Dim oPermissionPassport As Permission = Permission.None
                oPermissionPassport = WLHelper.GetPermissionOverFeature(oState.IDPassport, "Calendar.Scheduler", "U")

                oRet.ProductiveUnitData.Permission = oPermissionPassport

                Dim oProductiveUnitState As New roProductiveUnitState(oState.IDPassport)
                Dim oProductiveUnitManager As New roProductiveUnitManager(oProductiveUnitState)
                oRet.ProductiveUnitData.ProductiveUnit = oProductiveUnitManager.LoadProductiveUnit(_intIDProductiveUnit)

                ' Datos del periodo seleccionado
                Dim oBudgetRowPeriodDataState As New roBudgetRowPeriodDataState(oState.IDPassport)
                oRet.PeriodData = roBudgetRowPeriodDataManager.LoadCellsByBudget(_FirstDay, _LastDay, _intIDProductiveUnit, _intIDNode, oPermissionPassport, oParameters, BudgetView.Definition, BudgetDetailLevel.Daily, oBudgetRowPeriodDataState, True, True)

                oRowList.Add(oRet)

                ' Asignamos la fila al presupuesto
                oBudget.BudgetData = oRowList.ToArray

                ' Cargamos los indicadores de planificacion en caso necesario
                If bolShowIndictments Then AddIndictmentsToBudget(oBudget, _FirstDay, _LastDay)

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::GetBudgetPeriodHourDefinition")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::GetBudgetPeriodHourDefinition")
            End Try

            Return oBudget

        End Function

        Public Shared Function GetFeastDaysNode(ByVal IdNode As Integer, ByRef oState As roBudgetState) As DataTable
            '
            ' Obtiene los dias festivos del nodo seleccionado
            '
            Dim tbPlan As DataTable = Nothing

            oState.UpdateStateInfo()

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * FROM sysroScheduleTemplates_Detail " &
                             "WHERE IDTemplate IN(@SELECT# ID from sysroScheduleTemplates where id in(@SELECT# IDScheduleTemplate from sysroSecurityNodes where id=" & IdNode.ToString & ")) " &
                             "ORDER BY ScheduleDate"

                tbPlan = CreateDataTableWithoutTimeouts(strSQL, , "FestDaysPlan")
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::GetFeastDaysNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::GetFeastDaysNode")
            End Try

            Return tbPlan

        End Function

        Public Function GetProductiveUnitsFromNode(ByVal _IDNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime) As Generic.List(Of roProductiveUnit)

            Dim oProductiveUnits As New Generic.List(Of roProductiveUnit)
            Dim oProductiveUnit As roProductiveUnit = Nothing

            Dim bolret As Boolean = False

            Try
                ' Obtenemos todas las unidades productivas asignadas al presupuesto dentro del periodo indicado
                Dim strQuery As String = String.Empty
                strQuery &= " @SELECT# DISTINCT sysroSecurityNodes.Name as NodeName, DailyBudgets.IDNode, DailyBudgets.IDProductiveUnit, ProductiveUnits.Name as ProductiveUnitName, ProductiveUnits.Color as ProductiveUnitColor, ProductiveUnits.ShortName as ProductiveUnitShortName  "
                strQuery &= " FROM DailyBudgets INNER JOIN ProductiveUnits ON DailyBudgets.IDProductiveUnit = ProductiveUnits.ID  INNER JOIN sysroSecurityNodes ON DailyBudgets.IDNode = sysroSecurityNodes.ID "
                strQuery &= " WHERE DailyBudgets.IDNode = " & _IDNode.ToString
                strQuery &= " AND DailyBudgets.Date >= " & roTypes.Any2Time(xFirstDay.Date).SQLSmallDateTime & " And DailyBudgets.Date <= " & roTypes.Any2Time(xLastDay.Date).SQLSmallDateTime
                strQuery &= " ORDER BY ProductiveUnits.Name "

                Dim dTbl As System.Data.DataTable = CreateDataTable(strQuery)

                Dim oProductiveUnitState As New roProductiveUnitState(oState.IDPassport)
                Dim oProductiveUnitManager As New roProductiveUnitManager(oProductiveUnitState)

                If dTbl IsNot Nothing Then
                    For Each oRowUP As DataRow In dTbl.Rows
                        oProductiveUnit = New roProductiveUnit
                        oProductiveUnit = oProductiveUnitManager.LoadProductiveUnit(oRowUP("IDProductiveUnit"))
                        If oProductiveUnit IsNot Nothing Then
                            oProductiveUnits.Add(oProductiveUnit)
                        End If
                    Next
                End If

                bolret = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::GetProductiveUnitsFromNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::GetProductiveUnitsFromNode")
            End Try

            Return oProductiveUnits

        End Function

        Public Function CopyBudget(ByVal oParameters As roCollection, ByRef oState As roBudgetState,
                                 Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            oState.UpdateStateInfo()

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' 0.Obtenemos los parametros necesarios
                Dim intOriginNodeID As Integer = roTypes.Any2Integer(oParameters("lstOriginNode"))
                Dim intOriginProductiveUnitID As Integer = roTypes.Any2Integer(oParameters("lstOriginProductiveUnit"))

                Dim xBeginDateSource As Date = Date.Parse(oParameters("BeginDateSource"))
                Dim xEndDateSource As Date = Date.Parse(oParameters("EndDateSource"))
                Dim xFromDateDestination As Date = Date.Parse(oParameters("FromDateDestination"))

                Dim bolCopyEmployees As Boolean = roTypes.Any2Boolean(oParameters("CopyEmployees"))
                Dim bolKeepHolidays As Boolean = roTypes.Any2Boolean(oParameters("KeepHolidays"))

                ' 1.Obtenemos el presupuesto a copiar del nodo/unidad productiva a copiar
                Dim oSourceBudget As New roBudget
                oSourceBudget = Load(xBeginDateSource, xEndDateSource, intOriginNodeID.ToString, BudgetView.Planification, BudgetDetailLevel.Mode, False, intOriginProductiveUnitID)

                ' 2.Calculamos el periodo  de fechas a tratar en el empleado destino y las fechas en las que se debe aplicar el pegado
                Dim xBeginDateDestination As Date
                Dim xEndDateDestination As Date
                Dim xApplyDays As New roCollection
                SetPeriodDestinationFromParameters(xBeginDateDestination, xEndDateDestination, oParameters, xApplyDays)

                ' 4.Obtenemmos el presupuesto destino de las fechas que tenemos que tratar
                Dim oDestinationBudget As New roBudget
                oDestinationBudget = Load(xBeginDateDestination, xEndDateDestination, intOriginNodeID.ToString, BudgetView.Planification, BudgetDetailLevel.Mode, False, intOriginProductiveUnitID)

                '' 5.Aplicamos la copia en los dias que sean necesarios
                If oDestinationBudget IsNot Nothing AndAlso oDestinationBudget.BudgetData IsNot Nothing Then
                    For Each oCalendarRowBudgetData As roBudgetRow In oDestinationBudget.BudgetData
                        If oCalendarRowBudgetData IsNot Nothing AndAlso oCalendarRowBudgetData.PeriodData IsNot Nothing Then
                            For Each oCalendarRowDayData As roBudgetRowDayData In oCalendarRowBudgetData.PeriodData.DayData
                                ' Para cada fecha del calendario del presupuesto destino comprobamos si hay que aplicar el pegado
                                If xApplyDays.Exists(oCalendarRowDayData.PlanDate) Then

                                    Debug.Print("Pegar en el dia -->" & oCalendarRowDayData.PlanDate.ToString)

                                    ' Obtenemos la celda origen a partir de la posicion indicada
                                    If oSourceBudget IsNot Nothing AndAlso oSourceBudget.BudgetData IsNot Nothing Then
                                        For Each oSourceCalendarRowBudgetData As roBudgetRow In oSourceBudget.BudgetData
                                            If oSourceCalendarRowBudgetData IsNot Nothing AndAlso oSourceCalendarRowBudgetData.PeriodData IsNot Nothing Then
                                                For Each oSourceCalendarRowDayData As roBudgetRowDayData In oSourceCalendarRowBudgetData.PeriodData.DayData
                                                    If oSourceCalendarRowDayData.PlanDate = roTypes.Any2Time(xBeginDateSource).Add(xApplyDays.Item(oCalendarRowDayData.PlanDate) - 1, "d").Value Then
                                                        ' Pegamos los datos del dia en la celda destino
                                                        Debug.Print("....Copiar los datos del dia  -->" & oSourceCalendarRowDayData.PlanDate.ToString)
                                                        Dim bolCanModifyDay As Boolean = True
                                                        ' Si hay que copiar empleados, primero eliminamos las asignaciones actuales en el dia destino
                                                        If Not bolCopyEmployees Then
                                                            ' Si solo se copia planificacion, verificamos que no existan ya empleados asignados
                                                            If oCalendarRowDayData.ProductiveUnitMode IsNot Nothing AndAlso oCalendarRowDayData.ProductiveUnitMode.UnitModePositions IsNot Nothing Then
                                                                For Each oUnitModePosition As roProductiveUnitModePosition In oCalendarRowDayData.ProductiveUnitMode.UnitModePositions
                                                                    If oUnitModePosition.EmployeesData IsNot Nothing AndAlso oUnitModePosition.EmployeesData.Count > 0 Then
                                                                        bolCanModifyDay = False
                                                                    End If
                                                                Next
                                                            End If
                                                        End If

                                                        If bolCanModifyDay Then
                                                            Dim oNewBudgetRowDayData As New roBudgetRowDayData
                                                            oNewBudgetRowDayData.HasChanged = True
                                                            ' Asignamos la informacion del modo
                                                            oNewBudgetRowDayData.PlanDate = oCalendarRowDayData.PlanDate
                                                            oNewBudgetRowDayData.ProductiveUnitStatus = oSourceCalendarRowDayData.ProductiveUnitStatus

                                                            oNewBudgetRowDayData.ProductiveUnitMode = New roProductiveUnitMode
                                                            oNewBudgetRowDayData.ProductiveUnitMode.ID = oSourceCalendarRowDayData.ProductiveUnitMode.ID
                                                            oNewBudgetRowDayData.ProductiveUnitMode.CostValue = oSourceCalendarRowDayData.ProductiveUnitMode.CostValue
                                                            oNewBudgetRowDayData.ProductiveUnitMode.Coverage = oSourceCalendarRowDayData.ProductiveUnitMode.Coverage
                                                            oNewBudgetRowDayData.ProductiveUnitMode.IDProductiveUnit = oSourceCalendarRowDayData.ProductiveUnitMode.IDProductiveUnit

                                                            Dim oUnitModePositions As New Generic.List(Of roProductiveUnitModePosition)
                                                            For Each oUnitModePosition As roProductiveUnitModePosition In oSourceCalendarRowDayData.ProductiveUnitMode.UnitModePositions
                                                                Dim oNewUnitModePosition As New roProductiveUnitModePosition
                                                                oNewUnitModePosition.ID = -1
                                                                oNewUnitModePosition.AssignmentData = New roCalendarAssignmentCellData
                                                                oNewUnitModePosition.AssignmentData.ID = oUnitModePosition.AssignmentData.ID
                                                                oNewUnitModePosition.IDProductiveUnitMode = oUnitModePosition.IDProductiveUnitMode
                                                                oNewUnitModePosition.IsExpandable = oUnitModePosition.IsExpandable
                                                                oNewUnitModePosition.Quantity = oUnitModePosition.Quantity
                                                                oNewUnitModePosition.ShiftData = oUnitModePosition.ShiftData

                                                                Dim oEmployeesData As New Generic.List(Of roProductiveUnitModePositionEmployeeData)
                                                                If bolCopyEmployees Then
                                                                    For Each oEmployeeData As roProductiveUnitModePositionEmployeeData In oUnitModePosition.EmployeesData
                                                                        Dim oNewEmployeeData As New roProductiveUnitModePositionEmployeeData
                                                                        oNewEmployeeData.IDEmployee = oEmployeeData.IDEmployee
                                                                        oNewEmployeeData.ShiftData = oEmployeeData.ShiftData
                                                                        oEmployeesData.Add(oNewEmployeeData)
                                                                    Next
                                                                End If
                                                                oNewUnitModePosition.EmployeesData = oEmployeesData.ToArray

                                                                oUnitModePositions.Add(oNewUnitModePosition)
                                                            Next
                                                            oNewBudgetRowDayData.ProductiveUnitMode.UnitModePositions = oUnitModePositions.ToArray

                                                            ' Añadimos los nuevos datos al presupuesto
                                                            bolRet = AddBudgetRowDayData(oDestinationBudget, oNewBudgetRowDayData, oNewBudgetRowDayData.ProductiveUnitMode.IDProductiveUnit, bolKeepHolidays)
                                                            If Not bolRet Then
                                                                Return bolRet
                                                                Exit Function
                                                            End If
                                                        End If
                                                    End If
                                                Next
                                            End If
                                        Next
                                    End If
                                End If
                            Next
                        End If
                    Next
                End If

                ' 5.Guardamos los datos
                Dim oBudgetResult As roBudgetResult = Save(oDestinationBudget, True, True, True)

                If oBudgetResult.Status = BudgetStatusEnum.OK Then
                    bolRet = True
                Else
                    oState.Result = BudgetResultEnum.InValidData
                    If oBudgetResult.BudgetDataResult IsNot Nothing Then
                        'Mostramos el primer error que tenga el resultado de guardar el presupuesto
                        oState.ErrorText = oBudgetResult.BudgetDataResult(0).ErrorText
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::CopyBudget")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::CopyBudget")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Function SetPeriodDestinationFromParameters(ByRef xBeginDateDestination As Date, ByRef xEndDateDestination As Date, ByVal oParameters As roCollection, ByRef xApplyDays As roCollection) As Boolean
            '
            ' Calculamos el periodo de fechas que necesitaremos cargar del empleado destino
            '

            Dim bolRet As Boolean = True

            Try
                ' Fijamos el inicio del periodo a partir de la fecha desde cuando se quiere pegar en el destino
                xBeginDateDestination = Date.Parse(oParameters("FromDateDestination"))

                ' Obtenemos el periodo de fechas que se quieren copiar del origne
                Dim xBeginDateSource As Date = Date.Parse(oParameters("BeginDateSource"))
                Dim xEndDateSource As Date = Date.Parse(oParameters("EndDateSource"))

                ' Calculamos el final del periodo
                Dim intRepeatMode As Integer = roTypes.Any2Integer(oParameters("RepeatMode"))
                Dim strRepeatModeValue As String = roTypes.Any2String(oParameters("RepeatModeValue"))

                Dim xActualDate As Date
                Dim intPeriodCopyDays As Integer = 0
                Dim intDaytoCopy As Integer = 0
                Dim intTimes As Integer = 0

                ' Nos guardamos el numero total de dias a copiar
                intPeriodCopyDays = DateDiff(DateInterval.Day, xBeginDateSource, xEndDateSource) + 1

                Dim xToDestinationDate As Date = Date.MaxValue

                If intRepeatMode = 0 Then
                    ' Numero de repeticiones
                    intTimes = roTypes.Any2Integer(strRepeatModeValue)
                Else
                    ' hasta una fecha concreta inclusive
                    xToDestinationDate = Date.Parse(strRepeatModeValue)

                    ' Calculamos el numero de dias del periodo a pegar
                    Dim intPeriodPasteDays As Integer = DateDiff(DateInterval.Day, xBeginDateDestination, xToDestinationDate) + 1

                    'Calculamos el numero de repeticiones
                    intTimes = Fix(intPeriodPasteDays / intPeriodCopyDays)
                    If intPeriodPasteDays Mod intPeriodCopyDays > 0 Then intTimes += 1
                End If

                ' Fecha inicial de pegado en el presupuesto destino
                xActualDate = xBeginDateDestination
                intDaytoCopy += 1
                If intDaytoCopy > intPeriodCopyDays Then intDaytoCopy = 1

                xApplyDays.Add(xActualDate, intDaytoCopy)

                For intRepeat As Integer = 1 To intTimes
                    ' Para cada repeticion
                    ' Revisar cuando empieza la fecha de inicio de la actual repeticion,
                    ' siempre que no sea la primera
                    If intRepeat <> 1 Then
                        ' Justo al terminar, vamos al siguiente dia
                        xActualDate = xActualDate.AddDays(1)
                        intDaytoCopy += 1
                        If intDaytoCopy > intPeriodCopyDays Then intDaytoCopy = 1

                        xApplyDays.Add(xActualDate, intDaytoCopy)
                    End If

                    ' Obtenemos todos los dias siguientes hasta el ultimo dia de la actual repeticion, teniendo en cuenta el total de dias a copiar
                    For i As Integer = 1 To intPeriodCopyDays - 1
                        xActualDate = xActualDate.AddDays(1)
                        intDaytoCopy += 1
                        If intDaytoCopy > intPeriodCopyDays Then intDaytoCopy = 1

                        xApplyDays.Add(xActualDate, intDaytoCopy)
                    Next
                Next

                ' Nos guardamos el final del periodo
                xEndDateDestination = xActualDate
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetManager::SetPeriodDestinationFromParameters")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetManager::SetPeriodDestinationFromParameters")
            End Try

            Return bolRet

        End Function

        Public Function AddBudgetRowDayData(ByRef oBudget As roBudget, ByVal oNewBudgetRowDayData As roBudgetRowDayData, ByVal intIDProductiveUnit As Integer, Optional ByVal bolKeepHolidays As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bContinueChecking As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.oState.Result = BudgetResultEnum.InValidData

                If oBudget Is Nothing Or oNewBudgetRowDayData Is Nothing Or intIDProductiveUnit <= 0 Then
                    Return bolRet
                    Exit Function
                End If

                If oBudget.BudgetData Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                Dim oBudgetRowState As New roBudgetRowPeriodDataState(oState.IDPassport)
                Dim oBudgetRowPeriodData As New roBudgetRowPeriodDataManager(oBudgetRowState)

                ' Buscamos el dia a modificar del empleado indicado
                For Each oExistingBudgetRow As roBudgetRow In oBudget.BudgetData
                    If oExistingBudgetRow.ProductiveUnitData IsNot Nothing AndAlso oExistingBudgetRow.ProductiveUnitData.ProductiveUnit IsNot Nothing AndAlso oExistingBudgetRow.ProductiveUnitData.ProductiveUnit.ID = intIDProductiveUnit Then

                        'Me.oState.Result = roCalendarState.ResultEnum.RowDayData_PlannedDayNotExist
                        Me.oState.Result = BudgetResultEnum.InValidData

                        If oExistingBudgetRow.PeriodData IsNot Nothing AndAlso oExistingBudgetRow.PeriodData.DayData IsNot Nothing Then
                            For Each oExistingBudgetRowDayData As roBudgetRowDayData In oExistingBudgetRow.PeriodData.DayData
                                If oExistingBudgetRowDayData.PlanDate = oNewBudgetRowDayData.PlanDate Then
                                    ' Hemos encontrado el dia a modificar
                                    Dim bolCanModifyDay As Boolean = True
                                    Dim bolModified As Boolean = False

                                    ' Miro si por permisos ese día se puede modificar
                                    If oExistingBudgetRow.ProductiveUnitData.Permission > 3 Then
                                        ' Verificamos si podemos modificar un dia bloqueado
                                        If bolCanModifyDay Then
                                            bolModified = True
                                            ' Finalizo
                                            If bolModified Then
                                                oExistingBudgetRowDayData.ProductiveUnitMode = oNewBudgetRowDayData.ProductiveUnitMode
                                                oExistingBudgetRowDayData.ProductiveUnitStatus = oNewBudgetRowDayData.ProductiveUnitStatus
                                                For Each oUnitModePosition As roProductiveUnitModePosition In oExistingBudgetRowDayData.ProductiveUnitMode.UnitModePositions
                                                    oUnitModePosition.ID = -1
                                                    If oUnitModePosition.EmployeesData IsNot Nothing AndAlso oUnitModePosition.EmployeesData.Count > 0 Then
                                                        Dim oEmployeesData As New Generic.List(Of roProductiveUnitModePositionEmployeeData)
                                                        ' Validamos si cada uno de los empleados puede ser asignado a la posicion,
                                                        ' teniendo en cuenta si tiene contrato y si esta en un grupo del nodo de la posicion
                                                        ' y si no esta asignado actualmente a otro presupuesto ese mismo dia

                                                        ' en caso necesario mirar tambien si tiene vacaciones
                                                        For Each oEmployeeData As roProductiveUnitModePositionEmployeeData In oUnitModePosition.EmployeesData
                                                            Dim bolAddEmployee As Boolean = False
                                                            ' Contrato activo
                                                            If VTBusiness.Common.roBusinessSupport.EmployeeWithContract(oEmployeeData.IDEmployee, State, oExistingBudgetRowDayData.PlanDate) Then
                                                                bolAddEmployee = True
                                                            End If
                                                            If bolAddEmployee Then
                                                                bolAddEmployee = False
                                                                'TODO Replace roSecurityNodes For Something
                                                                ' Asignado a un grupo que pertenece al nodo del presupuesto
                                                                'Dim strSQL As String = "@SELECT# IDGroup FROM EmployeeGroups WHERE IDEmployee=" & oEmployeeData.IDEmployee & " AND BeginDate<=" & roTypes.Any2Time(oExistingBudgetRowDayData.PlanDate).SQLSmallDateTime & " AND EndDate>=" & roTypes.Any2Time(oExistingBudgetRowDayData.PlanDate).SQLSmallDateTime
                                                                'Dim intIDGroup As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))
                                                                'Dim intNode As Integer = roSecurityNodeManager.GetSecurityNodeFromGroupOrEmployee(New roSecurityNodeState(-1), intIDGroup, True, oExistingBudgetRowDayData.PlanDate)
                                                                'If oExistingBudgetRow.ProductiveUnitData.IDNode = intNode Then
                                                                '    bolAddEmployee = True
                                                                'ElseIf intNode > 0 Then
                                                                '    ' Buscamos si alguno de los nodos padre es al que esta asignado el presupuesto
                                                                '    Dim oNodemanager As New roSecurityNodeManager
                                                                '    Dim oNode As roSecurityNode = oNodemanager.Load(intNode, False, False)
                                                                '    While oNode IsNot Nothing AndAlso bolAddEmployee = False
                                                                '        If oNode.ID = oExistingBudgetRow.ProductiveUnitData.IDNode Then
                                                                '            bolAddEmployee = True
                                                                '        Else
                                                                '            If oNode.IDParent > 0 Then
                                                                '                oNode = oNodemanager.Load(oNode.IDParent, False, False)
                                                                '            Else
                                                                '                oNode = Nothing
                                                                '            End If
                                                                '        End If
                                                                '    End While
                                                                'End If
                                                            End If

                                                            If bolAddEmployee And bolKeepHolidays Then
                                                                bolAddEmployee = False
                                                                ' verificamos si el empleado tiene asignado vacaciones en el dia de destino, en ese caso lo descartamos
                                                                Dim strSQL As String = "@SELECT# IDEmployee FROM DailySchedule WHERE IDEmployee=" & oEmployeeData.IDEmployee & " AND Date=" & roTypes.Any2Time(oExistingBudgetRowDayData.PlanDate).SQLSmallDateTime & " AND isnull(IsHolidays,0) = 1 "
                                                                If roTypes.Any2Integer(ExecuteScalar(strSQL)) = 0 Then
                                                                    bolAddEmployee = True
                                                                End If
                                                            End If

                                                            If bolAddEmployee Then oEmployeesData.Add(oEmployeeData)
                                                        Next
                                                        oUnitModePosition.EmployeesData = oEmployeesData.ToArray
                                                    End If
                                                Next
                                            End If
                                        End If
                                    Else
                                        bolModified = False
                                        'Me.oState.Result = roCalendarState.ResultEnum.RowDayData_PermissionDeniedOverEmployee
                                        Me.oState.Result = BudgetResultEnum.PermissionDenied
                                        bolRet = False
                                        Exit For
                                    End If
                                    If bolModified Then
                                        oExistingBudgetRowDayData.HasChanged = True
                                        oExistingBudgetRow.RowState = BudgetRowState.UpdateRow
                                    End If
                                    Me.oState.Result = BudgetResultEnum.NoError
                                    bolRet = True
                                    Exit For
                                End If
                            Next
                        End If
                    End If
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::AddBudgetRowDayData")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::AddBudgetRowDayData")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function GetEmployeeAvailableForNode(ByVal _IDNode As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date) As roEmployeeAvailableForNode()
            Dim oRet As New Generic.List(Of roEmployeeAvailableForNode)
            Dim bolRet As Boolean = False
            Dim strEmployees As String = String.Empty
            Dim xPlanDate As Date = xBeginDate
            Dim strSQL As String

            Try
                ' Obtenemos los empleados del nodo que pueden realizar el puesto indicado

                strEmployees = roBudgetManager.GetEmployeeListFromNode(_IDNode, Me.State, xPlanDate, True)

                If strEmployees.Length = 0 Then strEmployees = "-1"
                strEmployees = "(" & strEmployees & ")"

                While xPlanDate <= xEndDate

                    Dim oEmployeeAvailableForNode As New roEmployeeAvailableForNode

                    ' Filtro para empleados ya asignados a una posicion
                    Dim strDailyBudgetPosition As String
                    strDailyBudgetPosition = " AND ISNULL((@SELECT# COUNT(*) FROM DailySchedule WITH (NOLOCK) WHERE DailySchedule.IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND DailySchedule.Date = " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(DailySchedule.IDDailyBudgetPosition, 0) > 0), 0) = 0  "

                    ' Filtro para empleados de ausencia prevista diaria
                    Dim strSQLProgrammedAbsence As String
                    strSQLProgrammedAbsence = " AND isnull((@SELECT# count(*) from ProgrammedAbsences WITH (NOLOCK) WHERE IDEmployee = sysrovwAllEmployeeGroups.IDEmployee AND "
                    strSQLProgrammedAbsence &= " ( ( (BeginDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")"
                    strSQLProgrammedAbsence &= " OR "
                    strSQLProgrammedAbsence &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")"
                    strSQLProgrammedAbsence &= " OR "
                    strSQLProgrammedAbsence &= " (BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")))),0) = 0"

                    strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName " &
                             " , 'MULTI' as 'AssignmentName'" &
                            ", -100 AS 'IDAssignment' " &
                            ", sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName   " &
                            "FROM sysrovwAllEmployeeGroups WITH (NOLOCK) " &
                            "INNER JOIN EmployeeContracts WITH (NOLOCK)  " &
                            "ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                        "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                            "sysrovwAllEmployeeGroups.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                            "(EmployeeContracts.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")  " &
                            "AND (@SELECT# count(*) from EmployeeAssignments WITH (NOLOCK) where EmployeeAssignments.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee) >0 " &
                        strSQLProgrammedAbsence &
                        strDailyBudgetPosition

                    '' Empleados que ya tengan asignado un puesto ese dia por planificacion
                    'strSQL = "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName " &
                    '        " , (@SELECT# Name from Assignments WITH (NOLOCK) where id in (@SELECT# IDAssignment from DailySchedule where  DailySchedule.Date =" & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND DailySchedule.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee)) as 'AssignmentName'" &
                    '        ", (@SELECT# IDAssignment from DailySchedule WITH (NOLOCK) where DailySchedule.Date =" & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND DailySchedule.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee) AS 'IDAssignment' " &
                    '        ", sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName   " &
                    '        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK) " &
                    '        "INNER JOIN EmployeeContracts WITH (NOLOCK)  " &
                    '        "ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                    '    "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                    '        "sysrovwAllEmployeeGroups.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                    '        "(EmployeeContracts.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")  " &
                    '        "AND (@SELECT# count(*) from EmployeeAssignments WITH (NOLOCK) where EmployeeAssignments.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee) >0 " &
                    '        "AND (@SELECT# count(*) from DailySchedule WITH (NOLOCK) where DailySchedule.Date =" & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND DailySchedule.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee AND isnull(DailySchedule.IDAssignment,0) > 0 ) > 0 " &
                    'strDailyBudgetPosition &
                    '    strSQLProgrammedAbsence &
                    '    " UNION "

                    '' Empleados que pueden realizar 1 puesto dentro del nodo y no esten planificados
                    'strSQL += "@SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName " &
                    '        " , (@SELECT# Name from Assignments WITH (NOLOCK) where id in (@SELECT# top 1 IDAssignment from EmployeeAssignments where EmployeeAssignments.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee)) as 'AssignmentName'" &
                    '        ", (@SELECT# top 1 IDAssignment from EmployeeAssignments WITH (NOLOCK) where EmployeeAssignments.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee) AS 'IDAssignment' " &
                    '        ", sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName   " &
                    '        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK) " &
                    '        "INNER JOIN EmployeeContracts WITH (NOLOCK)  " &
                    '        "ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                    '    "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                    '        "sysrovwAllEmployeeGroups.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                    '        "(EmployeeContracts.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")  " &
                    '        "AND (@SELECT# count(*) from EmployeeAssignments WITH (NOLOCK) where EmployeeAssignments.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee) =1 " &
                    '        "AND (@SELECT# count(*) from DailySchedule WITH (NOLOCK) where DailySchedule.Date =" & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND DailySchedule.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee AND isnull(DailySchedule.IDAssignment,0) > 0 ) = 0 " &
                    'strDailyBudgetPosition &
                    '    strSQLProgrammedAbsence &
                    '    " UNION "
                    '' Empleados que pueden realizar mas de un puesto dentro del nodo y no esten planificados
                    'strSQL += " @SELECT# sysrovwAllEmployeeGroups.IDEmployee, sysrovwAllEmployeeGroups.EmployeeName " &
                    '        " , 'MULTI' as 'AssignmentName'" &
                    '        ", -100 AS 'IDAssignment' " &
                    '        ", sysrovwAllEmployeeGroups.IDGroup, sysrovwAllEmployeeGroups.FullGroupName   " &
                    '        "FROM sysrovwAllEmployeeGroups WITH (NOLOCK) " &
                    '        "INNER JOIN EmployeeContracts WITH (NOLOCK)  " &
                    '        "ON sysrovwAllEmployeeGroups.IDEmployee = EmployeeContracts.IDEmployee " &
                    '    "WHERE sysrovwAllEmployeeGroups.IDEmployee IN " & strEmployees & " AND " &
                    '        "sysrovwAllEmployeeGroups.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND sysrovwAllEmployeeGroups.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND " &
                    '        "(EmployeeContracts.EndDate >= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & ")  " &
                    '        "AND (@SELECT# count(*) from EmployeeAssignments WITH (NOLOCK)  where EmployeeAssignments.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee) >1 " &
                    '        "AND (@SELECT# count(*) from DailySchedule WITH (NOLOCK)  where DailySchedule.Date =" & roTypes.Any2Time(xPlanDate).SQLSmallDateTime & " AND DailySchedule.IDEmployee=sysrovwAllEmployeeGroups.IDEmployee AND isnull(DailySchedule.IDAssignment,0) > 0 ) = 0 " &
                    'strDailyBudgetPosition &
                    '    strSQLProgrammedAbsence
                    strSQL &= " ORDER BY EmployeeName"
                    Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)

                    oEmployeeAvailableForNode.BudgetDate = xPlanDate

                    Dim BudgetEmployeeAvailableForNode As New Generic.List(Of roBudgetEmployeeAvailableForPosition)
                    'Cargar los datos de las posiciones
                    If dTbl IsNot Nothing Then
                        For Each dRow As DataRow In dTbl.Rows
                            Dim oBudgetEmployeeAvailableForPosition As New roBudgetEmployeeAvailableForPosition
                            oBudgetEmployeeAvailableForPosition.IDEmployee = roTypes.Any2Integer(dRow("IDEmployee"))
                            oBudgetEmployeeAvailableForPosition.EmployeeName = roTypes.Any2String(dRow("EmployeeName"))
                            oBudgetEmployeeAvailableForPosition.IDAssignment = roTypes.Any2Integer(dRow("IDAssignment"))
                            oBudgetEmployeeAvailableForPosition.AssignmentName = roTypes.Any2String(dRow("AssignmentName"))
                            If oBudgetEmployeeAvailableForPosition.IDAssignment = -100 Then
                                oBudgetEmployeeAvailableForPosition.AssignmentName = Me.oState.Language.Translate("Budget.EmployeeAvailable.MultiAssignments", "") ' Multiples puestos
                            End If
                            oBudgetEmployeeAvailableForPosition.IDShift = 0
                            oBudgetEmployeeAvailableForPosition.ShiftName = ""
                            oBudgetEmployeeAvailableForPosition.IDGroup = roTypes.Any2Integer(dRow("IDGroup"))
                            oBudgetEmployeeAvailableForPosition.FullGroupName = roTypes.Any2String(dRow("FullGroupName"))
                            oBudgetEmployeeAvailableForPosition.Cost = 0

                            BudgetEmployeeAvailableForNode.Add(oBudgetEmployeeAvailableForPosition)
                        Next
                    End If
                    oEmployeeAvailableForNode.BudgetEmployeeAvailableForNode = BudgetEmployeeAvailableForNode.ToArray
                    oRet.Add(oEmployeeAvailableForNode)

                    ' cambiamos de dia
                    xPlanDate = xPlanDate.AddDays(1)
                End While

                bolRet = True
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeAvailableForNode")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roBudgetManager::GetEmployeeAvailableForNode")
            End Try
            Return oRet.ToArray

        End Function

        Public Function AddIndictmentsToBudget(ByRef oBudget As roBudget, ByVal _FirstDay As DateTime, ByVal _LastDay As DateTime) As Boolean
            '
            ' Asignamos los indicadores de planificacion a los empleados del presupuesto
            '

            Dim bolRet As Boolean = False
            Dim bContinueChecking As Boolean = True

            Try
                Me.oState.Result = BudgetResultEnum.InValidData

                If oBudget Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                If oBudget.BudgetData Is Nothing Then
                    Return bolRet
                    Exit Function
                End If

                ' 01. Obtenemos los empleados del presupuesto que tenemos que cargar los indicadores
                Dim strEmployees As String = "B-1"
                If oBudget.BudgetData IsNot Nothing Then
                    For Each oBugetRow As roBudgetRow In oBudget.BudgetData
                        If oBugetRow.PeriodData IsNot Nothing AndAlso oBugetRow.PeriodData.DayData IsNot Nothing Then
                            For Each oPeriodData As roBudgetRowDayData In oBugetRow.PeriodData.DayData
                                If oPeriodData.ProductiveUnitMode IsNot Nothing AndAlso oPeriodData.ProductiveUnitMode.UnitModePositions IsNot Nothing Then
                                    For Each oUnitModePosition As roProductiveUnitModePosition In oPeriodData.ProductiveUnitMode.UnitModePositions
                                        If oUnitModePosition.EmployeesData IsNot Nothing Then
                                            For Each oEmployeeData As roProductiveUnitModePositionEmployeeData In oUnitModePosition.EmployeesData
                                                strEmployees += ",B" & oEmployeeData.IDEmployee
                                            Next
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If

                ' 02. Cargamos el calendario de planificacion de los empleados del presupuesto
                Dim oCalendarState = New roCalendarState(Me.oState.IDPassport)
                Dim oCalendarManager As New roCalendarManager(oCalendarState)

                Dim oCalendar As New roCalendar
                oCalendar = oCalendarManager.Load(_FirstDay, _LastDay, strEmployees, CalendarView.Planification, CalendarDetailLevel.Daily, False)

                ' 03. Cargamos los indicadores
                Dim oIndictments As New List(Of roCalendarScheduleIndictment)
                Try
                    Dim oCalRuleState As New roCalendarScheduleRulesState(oState.IDPassport)
                    Dim oCalRulesManager As New roCalendarScheduleRulesManager(oCalRuleState)
                    oIndictments = oCalRulesManager.CheckScheduleRules(oCalendar)
                Catch ex As Exception
                End Try

                ' 04. Añadimos los indicadores a cada empleado/dia que corresponda del presupuesto
                If oIndictments IsNot Nothing AndAlso oIndictments.Count > 0 Then
                    For Each _Indictment As roCalendarScheduleIndictment In oIndictments
                        If _Indictment.Dates IsNot Nothing AndAlso _Indictment.Dates.Count > 0 Then
                            For Each oBugetRow As roBudgetRow In oBudget.BudgetData
                                If oBugetRow.PeriodData IsNot Nothing AndAlso oBugetRow.PeriodData.DayData IsNot Nothing Then
                                    For Each oPeriodData As roBudgetRowDayData In oBugetRow.PeriodData.DayData
                                        If _Indictment.Dates.Contains(oPeriodData.PlanDate) Then
                                            If oPeriodData.ProductiveUnitMode IsNot Nothing AndAlso oPeriodData.ProductiveUnitMode.UnitModePositions IsNot Nothing Then
                                                For Each oUnitModePosition As roProductiveUnitModePosition In oPeriodData.ProductiveUnitMode.UnitModePositions
                                                    If oUnitModePosition.EmployeesData IsNot Nothing Then
                                                        For Each oEmployeeData As roProductiveUnitModePositionEmployeeData In oUnitModePosition.EmployeesData
                                                            If oEmployeeData.IDEmployee = _Indictment.IDEmployee Then
                                                                ' Añadimos el indicador  al empleado/dia correspondiente
                                                                Dim oEmployeeIndictments As New List(Of roCalendarScheduleIndictment)
                                                                If oEmployeeData.Alerts.Indictments IsNot Nothing Then
                                                                    oEmployeeIndictments = oEmployeeData.Alerts.Indictments.ToList
                                                                End If
                                                                oEmployeeIndictments.Add(_Indictment)
                                                                oEmployeeData.Alerts.Indictments = oEmployeeIndictments.ToArray
                                                            End If
                                                        Next
                                                    End If
                                                Next
                                            End If
                                        End If
                                    Next
                                End If
                            Next
                        End If
                    Next
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::AddIndictmentsToBudget")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roBudgetManager::AddIndictmentsToBudget")
            End Try

            Return bolRet

        End Function

        Public Function GetEmployeesAvailableWithSpecificSchedulerInNode(ByVal _IDNode As Integer, ByVal xDate As Date, ByVal _ProductiveUnitModePosition As roProductiveUnitModePosition) As roEmployeeAvailableForNode
            ' Obtenemos los empleados planificados con mismo horario y puesto que el de una posicion
            ' dentro de un nodo concreto en una fecha
            ' que no tengan definida una prevision de dias de ausencia
            Dim oRet As New roEmployeeAvailableForNode
            Dim bolRet As Boolean = False
            Dim strEmployees As String = String.Empty
            Dim strSQL As String

            Try

                If _ProductiveUnitModePosition Is Nothing OrElse _ProductiveUnitModePosition.ShiftData Is Nothing Then
                    Return oRet
                    Exit Function
                End If

                ' Obtenemos los empleados del nodo
                strEmployees = roBudgetManager.GetEmployeeListFromNode(_IDNode, Me.State, xDate, False)

                If strEmployees.Length = 0 Then strEmployees = "-1"
                strEmployees = "(" & strEmployees & ")"

                ' Filtro para empleados de ausencia prevista diaria
                Dim strSQLProgrammedAbsence As String
                strSQLProgrammedAbsence = " And isnull((@SELECT# count(*) from ProgrammedAbsences With (NOLOCK) WHERE IDEmployee = DailySchedule.IDEmployee And "
                strSQLProgrammedAbsence &= " ( ( (BeginDate >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " And BeginDate <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")"
                strSQLProgrammedAbsence &= " Or "
                strSQLProgrammedAbsence &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " And ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")"
                strSQLProgrammedAbsence &= " Or "
                strSQLProgrammedAbsence &= " (BeginDate <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " And ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")))),0) = 0"

                strSQL = "@SELECT# DailySchedule.IDEmployee " &
                        "FROM DailySchedule WITH (NOLOCK) " &
                        "INNER JOIN EmployeeContracts WITH (NOLOCK)  " &
                        "ON DailySchedule.IDEmployee = EmployeeContracts.IDEmployee " &
                    "WHERE DailySchedule.IDEmployee IN " & strEmployees &
                        " AND (EmployeeContracts.EndDate >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " ) AND (EmployeeContracts.BeginDate <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")  " &
                        " AND DailySchedule.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime & " AND DailySchedule.IDShift1 = " & _ProductiveUnitModePosition.ShiftData.ID.ToString

                If _ProductiveUnitModePosition.AssignmentData IsNot Nothing AndAlso _ProductiveUnitModePosition.AssignmentData.ID > 0 Then
                    strSQL &= " AND DailySchedule.IDAssignment= " & _ProductiveUnitModePosition.AssignmentData.ID.ToString
                End If
                strSQL &= strSQLProgrammedAbsence

                ' Si debemos aplicar como filtro empleados en previsiones de ausencia por horas
                Dim oParam As New AdvancedParameter.roAdvancedParameter("VTLive.EmployeesAvailable.ApplyProgrammedCauses", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport))
                If roTypes.Any2Boolean(oParam.Value) Then

                    ' A partir de un minimo de horas concretas
                    oParam = New AdvancedParameter.roAdvancedParameter("VTLive.EmployeesAvailable.ApplyProgrammedCauses.MinDuration", New AdvancedParameter.roAdvancedParameterState(Me.oState.IDPassport))
                    Dim dblMinDuration As Double = 0.0
                    If roTypes.Any2String(oParam.Value).Length > 0 Then dblMinDuration = roTypes.Any2Double(oParam.Value.Replace(".", roConversions.GetDecimalDigitFormat))

                    ' Filtro para empleados de ausencia prevista por horas a partir de un minimo de duracion
                    Dim strSQLProgrammedCause As String
                    strSQLProgrammedCause = " And isnull((@SELECT# count(*) from ProgrammedCauses With (NOLOCK) WHERE IDEmployee = DailySchedule.IDEmployee And "
                    strSQLProgrammedCause &= " Duration >= " & roTypes.Any2String(dblMinDuration).Replace(",", ".") & "  And "
                    strSQLProgrammedCause &= " ( ( (Date >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " And Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")"
                    strSQLProgrammedCause &= " Or "
                    strSQLProgrammedCause &= " (ISNULL(FinishDate, Date) >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " And ISNULL(FinishDate, Date) <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")"
                    strSQLProgrammedCause &= " Or "
                    strSQLProgrammedCause &= " (Date <= " & roTypes.Any2Time(xDate).SQLSmallDateTime & " And ISNULL(FinishDate, Date) >= " & roTypes.Any2Time(xDate).SQLSmallDateTime & ")))),0) = 0"

                    strSQL &= strSQLProgrammedCause
                End If

                Dim xEmptyDate As New Date(1899, 12, 30, 0, 0, 0, 0)

                If _ProductiveUnitModePosition.ShiftData.StartHour <> xEmptyDate AndAlso _ProductiveUnitModePosition.ShiftData.Type = ShiftTypeEnum.NormalFloating Then
                    strSQL += " AND DailySchedule.StartShift1= " & roTypes.Any2Time(_ProductiveUnitModePosition.ShiftData.StartHour).SQLDateTime
                End If

                Dim dTbl As System.Data.DataTable = CreateDataTableWithoutTimeouts(strSQL)

                Dim oEmployeeState As New Employee.roEmployeeState(-1)
                roBusinessState.CopyTo(Me.State, oEmployeeState)

                oRet.BudgetDate = xDate

                Dim BudgetEmployeeAvailableForNode As New Generic.List(Of roBudgetEmployeeAvailableForPosition)
                'Cargar los datos de las posiciones
                If dTbl IsNot Nothing Then
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oBudgetEmployeeAvailableForPosition As New roBudgetEmployeeAvailableForPosition
                        oBudgetEmployeeAvailableForPosition.IDEmployee = roTypes.Any2Integer(dRow("IDEmployee"))
                        If _ProductiveUnitModePosition.AssignmentData IsNot Nothing AndAlso _ProductiveUnitModePosition.AssignmentData.ID > 0 Then
                            oBudgetEmployeeAvailableForPosition.IDAssignment = _ProductiveUnitModePosition.AssignmentData.ID
                        End If
                        oBudgetEmployeeAvailableForPosition.IDShift = _ProductiveUnitModePosition.ShiftData.ID

                        BudgetEmployeeAvailableForNode.Add(oBudgetEmployeeAvailableForPosition)
                    Next
                End If
                oRet.BudgetEmployeeAvailableForNode = BudgetEmployeeAvailableForNode.ToArray

                bolRet = True
            Catch ex As DbException
                Me.State.UpdateStateInfo(ex, "roBudgetManager::GetEmployeesAvailableWithSpecificSchedulerInNode")
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roBudgetManager::GetEmployeesAvailableWithSpecificSchedulerInNode")
            End Try
            Return oRet
        End Function

        Public Function GetMinimumAmountWithSpecificSchedulerInNode(ByVal _IDNode As Integer, ByVal xDate As Date, ByVal _ProductiveUnitModePosition As roProductiveUnitModePosition) As Double
            '
            ' Obtiene la cantidad minima necesaria de un Horario + Puesto en un presupuesto para una fecha concreta
            '
            Dim dblRet As Double = 0

            oState.UpdateStateInfo()

            Try

                If _ProductiveUnitModePosition Is Nothing OrElse _ProductiveUnitModePosition.ShiftData Is Nothing Then
                    Return dblRet
                    Exit Function
                End If

                Dim strSQL As String
                strSQL = " @SELECT# sum(isnull(DailyBudget_Positions.Quantity, 0))  " &
                        " FROM DailyBudgets " &
                        " INNER JOIN DailyBudget_Positions On dbo.DailyBudgets.ID = dbo.DailyBudget_Positions.IDDailyBudget  " &
                        " INNER JOIN Assignments  On dbo.DailyBudget_Positions.IDAssignment = Assignments.ID  "
                strSQL &= " WHERE DailyBudgets.IDNode = " & _IDNode & " And " &
                              "DailyBudgets.Date = " & roTypes.Any2Time(xDate).SQLSmallDateTime &
                              " AND DailyBudget_Positions.IDShift=" & _ProductiveUnitModePosition.ShiftData.ID.ToString
                If _ProductiveUnitModePosition.AssignmentData IsNot Nothing AndAlso _ProductiveUnitModePosition.AssignmentData.ID > 0 Then
                    strSQL &= " AND DailyBudget_Positions.IDAssignment=" & _ProductiveUnitModePosition.AssignmentData.ID.ToString
                End If

                Dim xEmptyDate As New Date(1899, 12, 30, 0, 0, 0, 0)

                If _ProductiveUnitModePosition.ShiftData.StartHour <> xEmptyDate AndAlso _ProductiveUnitModePosition.ShiftData.Type = ShiftTypeEnum.NormalFloating Then
                    strSQL += " AND DailyBudget_Positions.StartShift= " & roTypes.Any2Time(_ProductiveUnitModePosition.ShiftData.StartHour).SQLDateTime
                End If

                dblRet = roTypes.Any2Double(ExecuteScalar(strSQL))
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetMinimumAmountWithSpecificSchedulerInNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roBudgetRowPeriodDataManager::GetMinimumAmountWithSpecificSchedulerInNode")
            Finally

            End Try

            Return dblRet

        End Function

#End Region

    End Class

End Namespace