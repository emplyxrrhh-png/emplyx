Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class TaskHelper

        Public Shared Function SaveNewTaskAlert(ByVal employeeId As Integer, ByVal taskId As Integer, ByVal strMessage As String, ByVal TimeZone As TimeZoneInfo, ByVal oTaskState As Task.roTaskState) As StdResponse
            Dim lrret As New StdResponse

            Try
                Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(employeeId, "LIVEPORTAL", New Terminal.roTerminalState())
                Dim dServerDateTime As Date = DateTime.Now
                Dim oTerminal As Terminal.roTerminal = Nothing
                If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                    dServerDateTime = oTerminals(0).GetCurrentDateTime()
                    oTerminal = oTerminals(0)
                End If

                dServerDateTime = StatusHelper.GetCurrentTerminalDatetime(oTerminal, TimeZone)

                lrret.Result = Task.roTaskAlert.SaveTaskAlertsFromPunch(taskId, employeeId, dServerDateTime, strMessage, oTaskState)

                If lrret.Result Then
                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Status = ErrorCodes.TASK_ALERT_SAVE_ERROR
                End If

                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TaskHelper::SaveNewTaskAlert")
            End Try

            Return lrret
        End Function

        Public Shared Function GetTaskUserFieldsAction(ByVal action As Integer, ByVal taskId As Integer, ByVal oTaskState As Task.roTaskState) As TaskUserFields
            Dim lrret As New TaskUserFields

            Try
                Dim fieldsList = New Generic.List(Of TaskUserField)

                Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState
                roBusinessState.CopyTo(oTaskState, oUserFieldState)
                'Obtenemos la definicion de campos de la tarea
                Dim tFieldsDT As DataTable = VTUserFields.UserFields.roUserField.GetTaskFields(Types.TaskField, oUserFieldState)

                Dim oTaskFieldState As New VTUserFields.UserFields.roTaskFieldState
                roBusinessState.CopyTo(oTaskState, oTaskFieldState)
                'Obtenemos los campos asignados a la tarea
                Dim taskFieldsList As Generic.List(Of VTUserFields.UserFields.roTaskField) = VTUserFields.UserFields.roTaskField.GetTaskFieldsList(taskId, oTaskFieldState)

                Dim selectedTask As New Task.roTask(taskId, oTaskState)

                Dim excludeFields As Boolean = False
                If (action = ActionTypes.aBegin) Then
                    If (selectedTask.ID > 0 AndAlso selectedTask.StartDate.HasValue) Then
                        excludeFields = True
                    End If
                End If

                Dim fieldDefinition As VTUserFields.UserFields.roTaskFieldDefinition

                For Each fieldDefinitionRow As DataRow In tFieldsDT.Rows
                    Dim newField As New TaskUserField

                    newField.UserFieldId = fieldDefinitionRow("ID")

                    Dim usedField As Boolean = False
                    If (excludeFields = False) Then
                        For Each aTaskField As VTUserFields.UserFields.roTaskField In taskFieldsList
                            If (aTaskField.IDField = newField.UserFieldId) Then
                                Select Case action
                                    Case ActionTypes.aBegin
                                        If aTaskField.Action = ActionTypes.aBegin Then
                                            usedField = True
                                        End If
                                    Case ActionTypes.tChange
                                        If aTaskField.Action = ActionTypes.tChange Then
                                            usedField = True
                                        End If
                                    Case ActionTypes.tComplete
                                        If aTaskField.Action = ActionTypes.tChange Or aTaskField.Action = ActionTypes.tComplete Then
                                            usedField = True
                                        End If
                                    Case Else
                                        usedField = False
                                End Select
                            End If

                        Next
                    End If

                    fieldDefinition = New VTUserFields.UserFields.roTaskFieldDefinition(oTaskFieldState, fieldDefinitionRow("ID"), False)
                    If (fieldDefinition IsNot Nothing) Then
                        newField.ValueType = fieldDefinition.ValueType
                        newField.ValuesList = fieldDefinition.ListValues.ToArray()
                    Else
                        newField.ValueType = ValueTypes.aValue
                    End If
                    newField.Used = usedField
                    newField.Name = fieldDefinitionRow("Name")

                    fieldsList.Add(newField)

                Next

                lrret.Fields = fieldsList.ToArray()
                lrret.Status = ErrorCodes.OK
            Catch ex As Exception
                lrret.Fields = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TaskHelper::GetTaskUserFieldsAction")
            End Try

            Return lrret
        End Function

        Public Shared Function SavePunchTaskValues(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo, ByVal taskId As Integer,
                                                   ByVal newTaskId As Integer, ByVal latitude As Double, ByVal longitude As Double, ByVal identifier As String, ByVal locationZone As String,
                                                   ByVal fullAddress As String, ByVal oldValue0 As String, ByVal oldValue1 As String, ByVal oldValue2 As String, ByVal oldValue3 As String,
                                                   ByVal oldValue4 As String, ByVal oldValue5 As String, ByVal newValue0 As String, ByVal newValue1 As String, ByVal newValue2 As String,
                                                   ByVal newValue3 As String, ByVal newValue4 As String, ByVal newValue5 As String, ByVal completeTask As Boolean,
                                                   ByVal punchImage As System.Drawing.Image, ByVal reliable As Boolean, Optional isApp As Boolean = False, Optional notReliableCause As String = Nothing) As StdResponse

            Dim bSaved As New StdResponse
            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.Punch.ProductiVPunch Then
                    bSaved.Result = True
                    bSaved.Status = ErrorCodes.OK

                    Dim bContinue As Boolean = False

                    Dim cCulture As Globalization.CultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture

                    Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(idEmployee, New Employee.roEmployeeState)

                    Dim oTerminals As Generic.List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", New Terminal.roTerminalState())
                    Dim oTerminal As Terminal.roTerminal = Nothing

                    If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                        oTerminal = oTerminals(0)
                    End If

                    'Recuperamos los valores de estado del empleado desde el servidor (hora, etc.)
                    Dim serverDatetime As DateTime = StatusHelper.GetCurrentTerminalDatetime(oTerminal, timeZone)

                    'Recumeramos la defincicion de los campos para saber los que tenemos que asignar a cada sitio
                    Dim definitionFields As DataTable = VTUserFields.UserFields.roUserField.GetTaskFields(Types.TaskField, New VTUserFields.UserFields.roUserFieldState(), "")

                    If completeTask Then
                        Dim curTask As Task.roTask = Task.roTask.GetLastTaskByEmployee(oEmployee.ID, New Task.roTaskState())

                        Dim oldTaskField1 As String = ""
                        Dim oldTaskField2 As String = ""
                        Dim oldTaskField3 As String = ""
                        Dim oldTaskField4 As Double = -1
                        Dim oldTaskField5 As Double = -1
                        Dim oldTaskField6 As Double = -1
                        'Cogemos los valores de los campos a asignar al completar una tarea
                        For Each cRow As DataRow In definitionFields.Rows
                            Dim action As Integer = roTypes.Any2Integer(cRow("Action"))
                            If (action = ActionTypes.tComplete) Then
                                Select Case roTypes.Any2Integer(cRow("ID"))
                                    Case 1
                                        If (oldValue0 <> String.Empty) Then oldTaskField1 = oldValue0
                                    Case 2
                                        If (oldValue1 <> String.Empty) Then oldTaskField2 = oldValue1
                                    Case 3
                                        If (oldValue2 <> String.Empty) Then oldTaskField3 = oldValue2
                                    Case 4
                                        If (oldValue3 <> String.Empty) Then oldTaskField4 = roTypes.Any2Double(oldValue3.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                    Case 5
                                        If (oldValue4 <> String.Empty) Then oldTaskField5 = roTypes.Any2Double(oldValue4.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                    Case 6
                                        If (oldValue5 <> String.Empty) Then oldTaskField6 = roTypes.Any2Double(oldValue5.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                End Select
                            End If
                        Next

                        curTask.EndDate = serverDatetime
                        curTask.Status = TaskStatusEnum._PENDING
                        curTask.IDEmployeeUpdateStatus = oEmployee.ID
                        curTask.UpdateStatusDate = serverDatetime

                        If (curTask.Save() AndAlso
                            Task.roTask.SaveTaskFieldsFromPunch(curTask.ID, oldTaskField1, oldTaskField2, oldTaskField3, oldTaskField4, oldTaskField5, oldTaskField6, New Task.roTaskState())) Then
                            bContinue = True
                        End If
                    Else
                        bContinue = True
                    End If

                    If bContinue Then
                        Dim oPunch As Punch.roPunch = Nothing
                        Dim oPunchStatus As PunchStatus

                        Dim oldTaskField1 As String = ""
                        Dim oldTaskField2 As String = ""
                        Dim oldTaskField3 As String = ""
                        Dim oldTaskField4 As Double = -1
                        Dim oldTaskField5 As Double = -1
                        Dim oldTaskField6 As Double = -1

                        Dim newTaskField1 As String = ""
                        Dim newTaskField2 As String = ""
                        Dim newTaskField3 As String = ""
                        Dim newTaskField4 As Double = -1
                        Dim newTaskField5 As Double = -1
                        Dim newTaskField6 As Double = -1

                        For Each cRow As DataRow In definitionFields.Rows
                            Dim action As Integer = roTypes.Any2Integer(cRow("Action"))
                            If (action = ActionTypes.tChange) Then
                                Select Case roTypes.Any2Integer(cRow("ID"))
                                    Case 1
                                        If (oldValue0 <> String.Empty) Then oldTaskField1 = oldValue0
                                        If (newValue0 <> String.Empty) Then newTaskField1 = newValue0
                                    Case 2
                                        If (oldValue1 <> String.Empty) Then oldTaskField2 = oldValue1
                                        If (newValue1 <> String.Empty) Then newTaskField2 = newValue1
                                    Case 3
                                        If (oldValue2 <> String.Empty) Then oldTaskField3 = oldValue2
                                        If (newValue2 <> String.Empty) Then newTaskField3 = newValue2
                                    Case 4
                                        If (oldValue3 <> String.Empty) Then oldTaskField4 = roTypes.Any2Double(oldValue3.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                        If (newValue3 <> String.Empty) Then newTaskField4 = roTypes.Any2Double(newValue3.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                    Case 5
                                        If (oldValue4 <> String.Empty) Then oldTaskField5 = roTypes.Any2Double(oldValue4.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                        If (newValue4 <> String.Empty) Then newTaskField5 = roTypes.Any2Double(newValue4.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                    Case 6
                                        If (oldValue5 <> String.Empty) Then oldTaskField6 = roTypes.Any2Double(oldValue5.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                        If (newValue5 <> String.Empty) Then newTaskField6 = roTypes.Any2Double(newValue5.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)

                                End Select
                            End If
                        Next

                        If (oldTaskField1 <> "" OrElse oldTaskField2 <> "" OrElse oldTaskField3 <> "" OrElse oldTaskField4 <> -1 OrElse oldTaskField5 <> -1 OrElse oldTaskField6 <> -1) Then
                            If Not Punch.roPunch.DoTaskPunch(oEmployee.ID, serverDatetime, oTerminal.ID, taskId, punchImage, oPunch, oPunchStatus, reliable, New Punch.roPunchState(),
                                                             latitude, longitude, locationZone, fullAddress, timeZone.Id, oldTaskField1, oldTaskField2, oldTaskField3, oldTaskField4,
                                                             oldTaskField5, oldTaskField6, isApp, notReliableCause) Then
                                bSaved.Result = False
                            End If
                        End If

                        If bSaved.Result Then
                            If Not Punch.roPunch.DoTaskPunch(oEmployee.ID, serverDatetime, oTerminal.ID, newTaskId, punchImage, oPunch, oPunchStatus, reliable, New Punch.roPunchState(),
                                                             latitude, longitude, locationZone, fullAddress, timeZone.Id, "", "", "", -1, -1, -1, isApp, notReliableCause) Then
                                bSaved.Result = False
                            Else
                                Dim newTask As Task.roTask = Task.roTask.GetLastTaskByEmployee(oEmployee.ID, New Task.roTaskState())
                                newTaskField1 = ""
                                newTaskField2 = ""
                                newTaskField3 = ""
                                newTaskField4 = -1
                                newTaskField5 = -1
                                newTaskField6 = -1
                                For Each cRow As DataRow In definitionFields.Rows
                                    Dim action As Integer = roTypes.Any2Integer(cRow("Action"))
                                    If (action = ActionTypes.aBegin) Then
                                        Select Case roTypes.Any2Integer(cRow("ID"))
                                            Case 1
                                                If (newValue0 <> String.Empty) Then newTaskField1 = newValue0
                                            Case 2
                                                If (newValue1 <> String.Empty) Then newTaskField2 = newValue1
                                            Case 3
                                                If (newValue2 <> String.Empty) Then newTaskField3 = newValue2
                                            Case 4
                                                If (newValue3 <> String.Empty) Then newTaskField4 = roTypes.Any2Double(newValue3.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                            Case 5
                                                If (newValue4 <> String.Empty) Then newTaskField5 = roTypes.Any2Double(newValue4.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                            Case 6
                                                If (newValue5 <> String.Empty) Then newTaskField6 = roTypes.Any2Double(newValue5.Replace(".", cCulture.NumberFormat.NumberDecimalSeparator), cCulture)
                                        End Select
                                    End If
                                Next
                                If (newTaskField1 <> "" OrElse newTaskField2 <> "" OrElse newTaskField3 <> "" OrElse newTaskField4 <> -1 OrElse newTaskField5 <> -1 OrElse newTaskField6 <> -1) Then
                                    If Not Task.roTask.SaveTaskFieldsFromPunch(newTask.ID, newTaskField1, newTaskField2, newTaskField3, newTaskField4, newTaskField5, newTaskField6, New Task.roTaskState()) Then
                                        bSaved.Result = False
                                    End If
                                End If
                            End If
                        End If
                    Else
                        bSaved.Result = False
                    End If
                Else
                    bSaved.Result = False
                    bSaved.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                bSaved.Result = False
                bSaved.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::TaskHelper::SavePunchTaskValues")
            End Try

            Return bSaved

        End Function

    End Class

End Namespace