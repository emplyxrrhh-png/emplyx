Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class CommonHelper

        Public Shared RandomGenerator As System.Random = New System.Random()

        Public Shared Function ParseDatetime(dateStart As String) As Date
            Dim sDate As Date
            Dim strDate As String
            Dim strTime As String = "00:00"
            Dim strArrDate As String() = dateStart.Split(" ")
            strDate = strArrDate(0)
            If strArrDate.Length = 2 Then strTime = dateStart.Split(" ")(1)
            sDate = roTypes.CreateDateTime(strDate.Split("-")(0), strDate.Split("-")(1), strDate.Split("-")(2), strTime.Split(":")(0), strTime.Split(":")(1), 0)
            Return sDate
        End Function

        Public Shared Function getSeason(ByVal curDate As DateTime) As Integer
            Try
                Dim value As Double = CDbl(curDate.Month) + (curDate.Day / 100)

                If value < 3.21 OrElse value >= 12.22 Then
                    Return 1 ' Winter
                ElseIf value < 6.21 Then
                    Return 2 ' Spring
                ElseIf value < 9.23 Then
                    Return 3 ' Summer
                Else
                    Return 4 ' Autumn
                End If
            Catch ex As Exception
                'do nothing
            End Try

            Return 2
        End Function

        Public Shared Function GetAvailableEmployeesForDate(ByVal oPassportTicket As roPassportTicket, ByVal sDate As String, ByVal eDate As String, ByVal idShift As Integer, ByVal oLng As roLanguage, ByVal idShiftApplicant As Integer, ByRef ostate As Employee.roEmployeeState) As GenericList
            Dim lrret As New GenericList

            Try

                lrret = VTRequests.Requests.roRequest.GetAvailableEmployeesForDateOnEmployeeShiftExchange(oPassportTicket, sDate, eDate, idShift, oLng, idShiftApplicant, ostate)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommonHelper::GetAvailableEmployeesForDate")
            End Try

            Return lrret
        End Function

        Public Shared Function GetDaysToCompensate(ByVal oPassportTicket As roPassportTicket, ByVal sDate As String, ByVal eDate As String, ByVal iExchangeEmployee As Integer, ByVal oLng As roLanguage, ByRef ostate As Employee.roEmployeeState) As DaysCount
            Dim lrret As New DaysCount

            Try
                Dim oCompensationActive As New Common.AdvancedParameter.roAdvancedParameter("VTLive.ExchangeShiftBetweenEmployees.CompensationRequiered", New Common.AdvancedParameter.roAdvancedParameterState())

                'TODO ISM --> XIS funció per obtenir el número de dies a compensar.
                If roTypes.Any2Boolean(oCompensationActive.Value) Then
                    lrret.Days = 3
                Else
                    lrret.Days = 0
                End If
                'TODO ISM --> XIS funció per obtenir el número de dies a compensar.

                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommonHelper::GetAvailableEmployeesForDate")
            End Try

            Return lrret
        End Function

        Public Shared Function GetTasksByName(ByVal oPassportTicket As roPassportTicket, ByVal taskName As String, ByVal oLng As roLanguage, ByRef oEmpState As Employee.roEmployeeState) As GenericList
            Dim lrret As New GenericList

            Try
                Dim oTable As DataTable = Nothing

                Dim oState As New VTUserFields.UserFields.roUserFieldState(oEmpState.IDPassport)
                roBusinessState.CopyTo(oEmpState, oState)

                Dim oPunchState As New Punch.roPunchState
                roBusinessState.CopyTo(oState, oPunchState)

                oTable = New DataTable
                oTable.TableName = "StartShifts"
                oTable.Columns.Add("ID", GetType(String))
                oTable.Columns.Add("Name", GetType(String))
                oTable.Columns.Add("TaskDescription", GetType(String))

                Dim oNewRow As DataRow = oTable.NewRow
                oNewRow("ID") = "0"
                oNewRow("Name") = oLng.Translate("VTPortal.Tasks.WithoutTask", "VTPortal")
                oNewRow("TaskDescription") = ""
                oTable.Rows.Add(oNewRow)

                Try
                    Dim iLimit As Integer = 200
                    Dim availableTasks = Punch.roPunch.GetAvailableTasksByEmployee(oPassportTicket.IDEmployee, oPunchState, iLimit, taskName)
                    If availableTasks = 0 Then
                        lrret.Status = -10
                    ElseIf availableTasks > iLimit Then
                        lrret.Status = -20
                    Else
                        Dim taskList As DataTable = Punch.roPunch.GetAllowTasksByEmployeeOnPunchV2(oPassportTicket.IDEmployee, oPunchState, False, -1, False, taskName)

                        For Each curTask In taskList.Rows
                            oNewRow = oTable.NewRow
                            oNewRow("ID") = curTask("ID")
                            oNewRow("Name") = curTask("FullTaskName")
                            oNewRow("TaskDescription") = curTask("TaskDescription") + "; " + curTask("Project")
                            oTable.Rows.Add(oNewRow)
                        Next
                    End If
                Catch ex As Exception
                End Try

                Dim items As New Generic.List(Of SelectField)
                If oTable IsNot Nothing Then

                    For Each dRow As DataRow In oTable.Rows
                        Dim selectF As New SelectField
                        selectF.FieldName = dRow(1)
                        selectF.FieldValue = dRow("ID")
                        selectF.RelatedInfo = roTypes.Any2String(dRow("TaskDescription"))
                        items.Add(selectF)
                    Next
                End If
                If (lrret.Status <> -10 AndAlso lrret.Status <> -20) Then lrret.Status = ErrorCodes.OK
                lrret.SelectFields = items.ToArray
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommonHelper::GetTasksByName")
            End Try

            Return lrret
        End Function

        Public Shared Function GetGenericList(ByVal oPassportTicket As roPassportTicket, ByVal requestType As String, ByVal oLng As roLanguage, ByRef oEmpState As Employee.roEmployeeState) As GenericList
            Dim lrret As New GenericList

            Try
                Dim oTable As DataTable = Nothing
                Dim bAddEmptyCause As Boolean = False

                Dim oState As New VTUserFields.UserFields.roUserFieldState(oEmpState.IDPassport)
                roBusinessState.CopyTo(oEmpState, oState)
                If requestType.ToLower = "userfields" Then
                    oTable = VTUserFields.UserFields.roEmployeeUserField.GetUserFieldsDataTable(oPassportTicket.IDEmployee, DateTime.Now.Date, oState)
                ElseIf requestType.ToLower.StartsWith("userfields.") Then
                    Dim strUserFieldName As String = requestType.Split(".")(1)

                    oTable = New DataTable
                    oTable.TableName = "UserFields"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))

                    Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                    roBusinessState.CopyTo(oState, oUserFieldState)

                    Dim oUserFieldInfo As New VTUserFields.UserFields.roUserField(oUserFieldState, strUserFieldName, Types.EmployeeField, False)
                    If oUserFieldInfo IsNot Nothing Then
                        For Each strItem As String In oUserFieldInfo.ListValues
                            Dim oNewRow As DataRow = oTable.NewRow
                            oNewRow("ID") = strItem
                            oNewRow("Name") = strItem
                            oTable.Rows.Add(oNewRow)
                        Next
                    End If

                ElseIf requestType.ToLower.StartsWith("shifts.permissiontype") Then
                    Dim oShiftState As New Shift.roShiftState
                    roBusinessState.CopyTo(oState, oShiftState)

                    oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(oPassportTicket.IDEmployee, oShiftState,,,, True, True)
                    Dim dv As New DataView(oTable)
                    dv.RowFilter = "ShiftType = 2"
                    dv.Sort = "Name"
                    oTable = dv.ToTable
                ElseIf requestType.ToLower.StartsWith("shifts.workingtype") Then

                    Dim oShiftState As New Shift.roShiftState
                    roBusinessState.CopyTo(oState, oShiftState)

                    oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(oPassportTicket.IDEmployee, oShiftState,,,, True)
                    Dim dv As New DataView(oTable)
                    dv.RowFilter = "ShiftType IS NULL OR ShiftType = 0 OR ShiftType = 1"
                    dv.Sort = "Name"
                    oTable = dv.ToTable
                ElseIf requestType.ToLower.StartsWith("shifts.exchangeshifts") Then

                    Dim oShiftState As New Shift.roShiftState
                    roBusinessState.CopyTo(oState, oShiftState)

                    oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(oPassportTicket.IDEmployee, oShiftState,,,, True)
                    Dim dv As New DataView(oTable)
                    dv.RowFilter = "(ShiftType IS NULL OR ShiftType = 0 OR ShiftType = 1) AND (IsFloating = 0 AND AllowComplementary = 0 AND AllowFloatingData = 0)"
                    dv.Sort = "Name"
                    oTable = dv.ToTable
                ElseIf requestType.ToLower.StartsWith("shifts.fulllist.withexcluded") Then

                    Dim oShiftState As New Shift.roShiftState
                    roBusinessState.CopyTo(oState, oShiftState)

                    oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(-1, oShiftState, IncludeObsoletes:=True, isPortal:=True)
                ElseIf requestType.ToLower.StartsWith("shifts.fulllist") Then

                    Dim oShiftState As New Shift.roShiftState
                    roBusinessState.CopyTo(oState, oShiftState)

                    oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(-1, oShiftState,,,, True)
                ElseIf requestType.ToLower.StartsWith("shifts") Then

                    Dim oShiftState As New Shift.roShiftState
                    roBusinessState.CopyTo(oState, oShiftState)

                    oTable = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(oPassportTicket.IDEmployee, oShiftState,,,, True)
                ElseIf requestType.ToLower.StartsWith("employees.fulllist") Then

                    Dim tmpTable = VTBusiness.Common.roBusinessSupport.GetEmployees("", "", "", oEmpState)

                    oTable = New DataTable
                    oTable.TableName = "EmployeeDocuments"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))

                    If tmpTable IsNot Nothing Then
                        For Each oRow As DataRow In tmpTable.Rows
                            Dim oNewRow As DataRow = oTable.NewRow
                            oNewRow("ID") = oRow("IDEmployee")
                            oNewRow("Name") = oRow("EmployeeName")
                            oTable.Rows.Add(oNewRow)
                        Next
                    End If

                ElseIf requestType.ToLower.StartsWith("causes.availableleave") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.Leaves, oCauseState)
                    bAddEmptyCause = True
                ElseIf requestType.ToLower.StartsWith("causes.programmedabsence") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.ProgrammedAbsence, oCauseState)
                    bAddEmptyCause = True
                ElseIf requestType.ToLower.StartsWith("causes.programmedcause") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.ProgrammedCause, oCauseState)
                    bAddEmptyCause = True
                ElseIf requestType.ToLower.StartsWith("causes.externalwork") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.ExternalWork, oCauseState)
                    bAddEmptyCause = False
                ElseIf requestType.ToLower.StartsWith("causes.plannedholiday") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.PlannedHolidays, oCauseState)
                    bAddEmptyCause = True
                ElseIf requestType.ToLower.StartsWith("causes.visibilitypermissions") Then ' Esta opción se mantiene por compatibilidad con versiones anteriores
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)

                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.All, oCauseState)
                    For Each oRow As DataRow In oTable.Rows
                        If Not (roTypes.Any2Boolean(oRow("IsHoliday")) OrElse roTypes.Any2Boolean(oRow("WorkingType")) = False) Then
                            oRow.Delete()
                        End If
                    Next
                    oTable.AcceptChanges()
                    bAddEmptyCause = True
                ElseIf requestType.ToLower.StartsWith("causes.readerinputcode") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesByEmployeeInputPermissions(oPassportTicket.IDEmployee, oCauseState)
                    bAddEmptyCause = True
                ElseIf requestType.ToLower.StartsWith("plannedovertime") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    oTable = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.ProgrammedOvertime, oCauseState)
                ElseIf requestType.ToLower.StartsWith("plannedholiday") Then
                    oTable = New DataTable
                    oTable.TableName = "EmployeeDocuments"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))
                    oTable.Columns.Add("RelatedInfo", GetType(String))

                    Dim oReqState As New Requests.roRequestState
                    roBusinessState.CopyTo(oState, oReqState)

                    If VTPortal.SecurityHelper.GetFeaturePermission(oPassportTicket.ID, New Requests.roRequestTypeSecurity(eRequestType.PlannedHolidays, oReqState).EmployeeFeatureName, "E") >= Permission.Write Then
                        Dim oCauseState As New Cause.roCauseState
                        roBusinessState.CopyTo(oState, oCauseState)
                        Dim tmpCauses = Cause.roCause.GetCausesForRequestByEmployee(oPassportTicket.IDEmployee, eCauseRequest.PlannedHolidays, oCauseState)
                        For Each oRow As DataRow In tmpCauses.Rows
                            'If roTypes.Any2Boolean(oRow("IsHoliday")) Then
                            Dim oNewRow As DataRow = oTable.NewRow
                            oNewRow("ID") = oRow("ID")
                            oNewRow("Name") = oRow("Name")
                            oNewRow("RelatedInfo") = "C"
                            oTable.Rows.Add(oNewRow)
                            'End If
                        Next
                    End If

                    If VTPortal.SecurityHelper.GetFeaturePermission(oPassportTicket.ID, New Requests.roRequestTypeSecurity(eRequestType.VacationsOrPermissions, oReqState).EmployeeFeatureName, "E") >= Permission.Write Then
                        Dim oShiftState As New Shift.roShiftState
                        roBusinessState.CopyTo(oState, oShiftState)

                        Dim tmpShifts = Shift.roShift.GetShiftsByEmployeeVisibilityPermissions(oPassportTicket.IDEmployee, oShiftState,,,, True)
                        Dim dv As New DataView(tmpShifts)
                        dv.RowFilter = "ShiftType = 2"
                        dv.Sort = "Name"
                        tmpShifts = dv.ToTable

                        For Each oRow As DataRow In tmpShifts.Rows
                            Dim oNewRow As DataRow = oTable.NewRow
                            oNewRow("ID") = oRow("ID")
                            oNewRow("Name") = oRow("Name")
                            oNewRow("RelatedInfo") = "S"
                            oTable.Rows.Add(oNewRow)
                        Next
                    End If

                    Dim dvSort As New DataView(oTable)
                    dvSort.Sort = "Name"
                    oTable = dvSort.ToTable

                ElseIf requestType.ToLower.StartsWith("documents") Then
                    Dim oDocState As New VTDocuments.roDocumentState
                    roBusinessState.CopyTo(oState, oDocState)
                    Dim oDocManager As New VTDocuments.roDocumentManager(oDocState)

                    oTable = New DataTable
                    oTable.TableName = "EmployeeDocuments"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))

                    For Each oDoc As roDocumentTemplate In oDocManager.GetAvailableEmployeeTemplateDocuments()
                        Dim oNewRow As DataRow = oTable.NewRow
                        oNewRow("ID") = oDoc.Id
                        oNewRow("Name") = oDoc.Name
                        oTable.Rows.Add(oNewRow)
                    Next

                ElseIf requestType.ToLower.StartsWith("causes") Then
                    Dim oCauseState As New Cause.roCauseState
                    roBusinessState.CopyTo(oState, oCauseState)
                    Dim oCause As New Cause.roCause(-1, oCauseState)
                    oTable = oCause.Causes("", True)
                    bAddEmptyCause = True
                ElseIf requestType.ToLower = "startshifts" Then

                    oTable = New DataTable
                    oTable.TableName = "StartShifts"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))

                    Dim oNewRow As DataRow = oTable.NewRow
                    oNewRow("ID") = "1"
                    oNewRow("Name") = oLng.Translate("StartFloating.ShiftDay", "Shift")
                    oTable.Rows.Add(oNewRow)

                    oNewRow = oTable.NewRow
                    oNewRow("ID") = "2"
                    oNewRow("Name") = oLng.Translate("StartFloating.ShiftAfter", "Shift")
                    oTable.Rows.Add(oNewRow)

                    oNewRow = oTable.NewRow
                    oNewRow("ID") = "0"
                    oNewRow("Name") = oLng.Translate("StartFloating.ShiftBefore", "Shift")
                    oTable.Rows.Add(oNewRow)

                ElseIf requestType.ToLower = "costcenters" Then
                    oTable = New DataTable
                    oTable.TableName = "CostCenters"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))

                    Try
                        Dim oBusinessState As New BusinessCenter.roBusinessCenterState
                        roBusinessState.CopyTo(oState, oBusinessState)

                        Dim dtblCauses As DataTable = BusinessCenter.roBusinessCenter.GetAvailableBusinessCentersDataTable(oBusinessState, oPassportTicket.IDEmployee, Date.Now.Date, False)
                        If dtblCauses IsNot Nothing AndAlso dtblCauses.Rows.Count > 0 Then
                            For Each dRow As DataRow In dtblCauses.Rows
                                Dim oNewRow As DataRow = oTable.NewRow
                                oNewRow("Name") = dRow("Name")
                                oNewRow("ID") = dRow("IDCenter")
                                oTable.Rows.Add(oNewRow)
                            Next
                        End If
                    Catch ex As Exception
                    End Try
                ElseIf requestType.ToLower = "costcentersfull" Then
                    oTable = BusinessCenter.roBusinessCenter.GetBusinessCenters(New BusinessCenter.roBusinessCenterState(oPassportTicket.ID))

                ElseIf requestType.ToLower = "tasks.availabletasks" Then
                    Dim oPunchState As New Punch.roPunchState
                    roBusinessState.CopyTo(oState, oPunchState)

                    oTable = New DataTable
                    oTable.TableName = "StartShifts"
                    oTable.Columns.Add("ID", GetType(String))
                    oTable.Columns.Add("Name", GetType(String))
                    oTable.Columns.Add("TaskDescription", GetType(String))

                    Dim oNewRow As DataRow = oTable.NewRow
                    oNewRow("ID") = "0"
                    oNewRow("Name") = oLng.Translate("VTPortal.Tasks.WithoutTask", "VTPortal")
                    oNewRow("TaskDescription") = ""
                    oTable.Rows.Add(oNewRow)

                    Try
                        Dim taskList As DataTable = Punch.roPunch.GetAllowTasksByEmployeeOnPunchV2(oPassportTicket.IDEmployee, oPunchState, False, -1, False)

                        For Each curTask In taskList.Rows
                            oNewRow = oTable.NewRow
                            oNewRow("ID") = curTask("ID")
                            oNewRow("Name") = curTask("FullTaskName")
                            oNewRow("TaskDescription") = curTask("TaskDescription") + "; " + curTask("Project")
                            oTable.Rows.Add(oNewRow)
                        Next
                    Catch ex As Exception

                    End Try
                Else
                    oTable = Nothing
                End If

                Dim items As New Generic.List(Of SelectField)
                If oTable IsNot Nothing Then
                    If bAddEmptyCause Then 'requestType.ToLower.StartsWith("causes") AndAlso Not requestType.ToLower.Contains("leave")
                        Dim selectF As New SelectField

                        selectF.FieldValue = "-1"
                        selectF.FieldName = "     "

                        items.Add(selectF)
                    End If

                    Dim oAddHours As Common.AdvancedParameter.roAdvancedParameter = Nothing

                    For Each dRow As DataRow In oTable.Rows
                        Dim selectF As New SelectField
                        If requestType.ToLower = "userfields" Then

                            If (roTypes.Any2Integer(dRow("Type")) = 9) Then
                                Dim idTemplate As Integer = 0

                                Dim obj As Object = DataLayer.AccessHelper.ExecuteScalar("@SELECT# Id from DocumentTemplates where Name ='" & dRow(0) & "'")
                                If obj IsNot Nothing AndAlso Not IsDBNull(obj) Then
                                    idTemplate = roTypes.Any2Integer(obj)
                                End If

                                selectF.FieldValue = dRow(0) & "__" & dRow("Type") & "__" & roTypes.Any2Boolean(dRow("History")) & "__" & idTemplate
                            Else
                                selectF.FieldValue = dRow(0) & "__" & dRow("Type") & "__" & roTypes.Any2Boolean(dRow("History")) & "__0"
                            End If
                        Else
                            selectF.FieldValue = dRow(0)
                        End If

                        selectF.FieldName = dRow(1)

                        If dRow.Table.Columns.Contains("RelatedInfo") Then
                            selectF.RelatedInfo = dRow("RelatedInfo")
                        ElseIf dRow.Table.Columns.Contains("AbsenceMandatoryDays") Then
                            selectF.RelatedInfo = roTypes.Any2String(dRow("AbsenceMandatoryDays"))
                        ElseIf dRow.Table.Columns.Contains("TaskDescription") Then
                            selectF.RelatedInfo = roTypes.Any2String(dRow("TaskDescription"))
                        ElseIf dRow.Table.Columns.Contains("ExpectedWorkingHours") Then
                            If oAddHours Is Nothing Then oAddHours = New Common.AdvancedParameter.roAdvancedParameter("VTLive.ExchangeShiftBetweenEmployees.CompensationRequiered", New Common.AdvancedParameter.roAdvancedParameterState())

                            If roTypes.Any2Boolean(oAddHours.Value) Then
                                selectF.RelatedInfo = roTypes.Any2String(dRow("ExpectedWorkingHours")).Replace(",", ".")
                            Else
                                selectF.RelatedInfo = "-1"
                            End If
                        Else
                            selectF.RelatedInfo = String.Empty
                        End If

                        items.Add(selectF)
                    Next
                End If

                lrret.Status = ErrorCodes.OK
                lrret.SelectFields = items.ToArray
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::CommonHelper::GetGenericList")
            End Try

            Return lrret
        End Function

        Public Shared Function LogMessage(ByVal strLogMessage As String) As Boolean
            Dim lrret As Boolean = False

            Try
                roLog.GetInstance().logMessage(roLog.EventType.roDebug, strLogMessage)
            Catch ex As Exception
                lrret = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "VTPortal::CommonHelper::LogMessage", ex)
            End Try

            Return lrret
        End Function

    End Class

End Namespace