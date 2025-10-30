Imports System.Collections.Generic
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTBusiness.Terminal
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTPortal

    Public Class PunchHelper
        Public Shared Function SavePunch(ByVal oPassport As roPassportTicket, ByVal terminal As roTerminal, ByVal idEmployee As Integer, ByVal timeZone As TimeZoneInfo, ByVal causeId As Integer, ByVal direction As String, ByVal latitude As Double, ByVal longitude As Double, ByVal identifier As String,
                                                          ByVal locationZone As String, ByVal fullAddress As String, ByVal punchImage As System.Drawing.Image, ByVal reliable As Boolean, ByVal nfcTag As String, Optional punchDate As Nullable(Of DateTime) = Nothing, Optional ByVal bIsForgottenPunch As Boolean = False, Optional bTelecommuting As Boolean = False, Optional avoidPermissions As Boolean = False, Optional ByVal reliableZone As Boolean = True, Optional ByVal selectedZone As Integer = -1, Optional strIP As String = "", Optional isApp As Boolean = False, Optional comments As String = "", Optional notReliableCause As String = Nothing) As StdResponse
            Dim bSaved As New StdResponse
            Try

                Dim oReqState As New Requests.roRequestState(-1)
                If oPassport IsNot Nothing Then oReqState = New Requests.roRequestState(oPassport.ID)

                Dim oPermList As New PermissionList

                If oPassport IsNot Nothing Then oPermList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If (oPassport Is Nothing OrElse oPermList.Punch.SchedulePunch) OrElse avoidPermissions Then
                    bSaved.Result = True
                    bSaved.Status = ErrorCodes.OK

                    Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(idEmployee, New Employee.roEmployeeState)

                    If terminal Is Nothing Then
                        Dim oTerminals As Generic.List(Of Terminal.roTerminal) = roTerminal.GetEmployeeTerminals(idEmployee, "LIVEPORTAL", New Terminal.roTerminalState())
                        Dim dServerDateTime As Date = DateTime.Now
                        If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                            dServerDateTime = oTerminals(0).GetCurrentDateTime()
                            terminal = oTerminals(0)
                        End If
                    End If

                    'Recuperamos los valores de estado del empleado desde el servidor (hora, etc.)
                    Dim serverDatetime As DateTime = VTPortal.StatusHelper.GetCurrentTerminalDatetime(terminal, timeZone)

                    If punchDate.HasValue Then
                        ' Verificamos que la fecha/hora proporcionada no sea futura
                        If punchDate > serverDatetime Then
                            bSaved.Result = False
                            bSaved.Status = ErrorCodes.FORBID_INCORRECT_DATE_FUTURE
                            Return bSaved
                        Else
                            Dim roContractState = New Contract.roContractState()
                            roBusinessState.CopyTo(oReqState, roContractState)
                            Dim oContractDates = Common.roBusinessSupport.GetDatesOfContractInDate(idEmployee, punchDate, roContractState)
                            'Si no hay contrato en la fecha seleccionada, no se puede realizar el fichaje
                            If oContractDates.Count = 0 Then
                                bSaved.Result = False
                                bSaved.Status = ErrorCodes.REQUEST_ERROR_OutOfContract
                                Return bSaved
                            End If
                            serverDatetime = punchDate
                        End If
                    End If

                    Dim oPunch As Punch.roPunch = Nothing
                    Dim oPunchStatus As PunchSeqStatus = PunchSeqStatus.OK
                    fullAddress = fullAddress.Replace("""", "")

                    bSaved.Result = Punch.roPunch.DoPortalPunch(oEmployee.ID, serverDatetime, direction, terminal.ID, 1, causeId, punchImage, reliable, oPunch, New Punch.roPunchState(), oPunchStatus,
                        latitude, longitude, locationZone, fullAddress, timeZone.Id, bIsForgottenPunch, nfcTag, bTelecommuting, reliableZone, selectedZone, strIP, isApp, comments, notReliableCause)

                    If (oPunchStatus <> PunchSeqStatus.OK) Then

                        If oPunchStatus = PunchSeqStatus.Repited_IN Then
                            bSaved.Status = ErrorCodes.PUNCH_ERROR_REPEAT_IN
                        ElseIf oPunchStatus = PunchSeqStatus.Repited_OUT Then
                            bSaved.Status = ErrorCodes.PUNCH_ERROR_REPEAT_OUT
                        ElseIf oPunchStatus = PunchSeqStatus.NFC_TAG_NOT_FOUND Then
                            bSaved.Status = ErrorCodes.PUNCH_NFC_TAG_NOT_FOUND
                        Else
                            bSaved.Status = ErrorCodes.GENERAL_ERROR
                        End If
                    End If
                Else
                    bSaved.Result = False
                    bSaved.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                bSaved.Result = False
                bSaved.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::PunchHelper::SavePunch")
            End Try

            Return bSaved
        End Function

        Public Shared Function GetEmployeePunches(ByVal oPassport As roPassportTicket, ByVal idEmployee As Integer, ByVal eDate As DateTime, ByRef oPunchState As Punch.roPunchState) As EmployeePunches
            Dim lrret As New EmployeePunches

            Try
                Dim oReqState As New Requests.roRequestState(oPassport.ID)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassport, Nothing, oReqState)

                If oPermList.Punch.ScheduleQuery OrElse oPermList.Punch.ProductiVQuery Then
                    Dim oServerLicense As New roServerLicense()
                    Dim bHRScheduling As Boolean = oServerLicense.FeatureIsInstalled("Feature\HRScheduling")

                    Dim strPunchesQuery As String = "-1"

                    If oPermList.Punch.ScheduleQuery Then strPunchesQuery &= ",1,2,3,7,13"
                    If oPermList.Punch.ProductiVQuery Then strPunchesQuery &= ",4"

                    Dim tbPunches As DataTable = Punch.roPunch.GetPortalPunchesDataTable(oPunchState, eDate, eDate, Nothing, Nothing, idEmployee, strPunchesQuery)
                    Dim oRetPunches As New Generic.List(Of EmployeePunch)
                    If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then

                        strPunchesQuery = strPunchesQuery.Replace(",7", "")
                        Dim oPunchesRows As DataRow() = tbPunches.Select("ActualType IN(" & strPunchesQuery & ")", "DateTime ASC")

                        For Each oRow As DataRow In oPunchesRows
                            Dim otmpPunch As New EmployeePunch
                            otmpPunch.ID = roTypes.Any2Integer(oRow("ID"))
                            otmpPunch.Type = roTypes.Any2Integer(oRow("Type"))
                            otmpPunch.ActualType = roTypes.Any2Integer(oRow("ActualType"))
                            otmpPunch.TypeData = roTypes.Any2Integer(oRow("TypeData"))
                            otmpPunch.DateTime = roTypes.Any2DateTime(oRow("DateTime"))
                            otmpPunch.LocationZone = roTypes.Any2String(oRow("LocationZone"))
                            otmpPunch.FullAddress = roTypes.Any2String(oRow("FullAddress"))
                            otmpPunch.EmployeeName = roTypes.Any2String(oRow("EmployeeName"))
                            otmpPunch.ZoneName = roTypes.Any2String(oRow("ZoneName"))
                            otmpPunch.TerminalName = roTypes.Any2String(oRow("TerminalName"))
                            otmpPunch.RelatedInfo = roTypes.Any2String(oRow("RelatedInfo"))
                            otmpPunch.Field1 = roTypes.Any2String(oRow("Field1"))
                            otmpPunch.Field2 = roTypes.Any2String(oRow("Field2"))
                            otmpPunch.Field3 = roTypes.Any2String(oRow("Field3"))
                            otmpPunch.Field4 = roTypes.Any2Double(oRow("Field4"))
                            otmpPunch.Field5 = roTypes.Any2Double(oRow("Field5"))
                            otmpPunch.Field6 = roTypes.Any2Double(oRow("Field6"))
                            otmpPunch.InTelecommute = roTypes.Any2String(oRow("InTelecommute"))
                            otmpPunch.RequestedTypeData = roTypes.Any2String(oRow("RequestedTypeData"))

                            If otmpPunch.Type = 4 Then
                                Dim bState = New roTaskState(-1)

                                Dim tarea = New roTask(roTypes.Any2Integer(oRow("TypeData")), bState)
                                If tarea IsNot Nothing Then
                                    otmpPunch.RelatedInfo = tarea.Project + "; " + tarea.Name + "; " + tarea.Description

                                    'Elimina el último salto de línea y punto y coma
                                    otmpPunch.RelatedInfo = otmpPunch.RelatedInfo.TrimEnd(";"c, " "c)

                                    ' Si el fichaje de tarea tiene informado alguno de los seis parámetros, los añado a Descripción con un saldo de línea.
                                    If Not IsDBNull(oRow("Field1")) OrElse Not IsDBNull(oRow("Field2")) OrElse Not IsDBNull(oRow("Field3")) OrElse Not IsDBNull(oRow("Field4")) OrElse Not IsDBNull(oRow("Field5")) OrElse Not IsDBNull(oRow("Field6")) Then
                                        Dim attributeName As String = String.Empty
                                        Dim attributeValue As String = String.Empty

                                        Dim requiredAttributesForTask As Dictionary(Of Integer, String) = Nothing
                                        Dim sql As String = "@SELECT# IDField, sysroFieldsTask.Name FROM FieldsTask " &
                                                            "INNER JOIN sysroFieldsTask ON sysroFieldsTask.id = FieldsTask.IDField AND FieldsTask.IDTask = " & roTypes.Any2Integer(tarea.ID)

                                        Dim tbAux As DataTable = DataLayer.AccessHelper.CreateDataTable(sql)
                                        If tbAux IsNot Nothing AndAlso tbAux.Rows.Count > 0 Then
                                            requiredAttributesForTask = New Dictionary(Of Integer, String)
                                            For Each oRowAux As DataRow In tbAux.Rows
                                                requiredAttributesForTask.Add(roTypes.Any2Integer(oRowAux("IDField")), roTypes.Any2String(oRowAux("Name")))
                                            Next
                                        End If

                                        'Agrega los parámetros a RelatedInfo con salto de línea si están presentes
                                        For Each field As Integer In {4, 5, 6, 1, 2, 3}
                                            If Not IsDBNull(oRow($"Field{field}")) Then
                                                If requiredAttributesForTask IsNot Nothing AndAlso requiredAttributesForTask.ContainsKey(field) Then
                                                    attributeName = requiredAttributesForTask(field)
                                                    attributeValue = otmpPunch.GetType().GetProperty($"Field{field}").GetValue(otmpPunch)
                                                    Dim lineMaxLength As Integer = 40
                                                    If attributeName.Length + attributeValue.Length > lineMaxLength Then
                                                        'Acorto cadenas para ajustar a una línea
                                                        If attributeName.Length < (lineMaxLength \ 2) Then
                                                            attributeValue = roSupport.TruncateScreenText(attributeValue, lineMaxLength - attributeName.Length)
                                                        ElseIf attributeValue.Length < (lineMaxLength \ 2) Then
                                                            attributeName = roSupport.TruncateScreenText(attributeName, lineMaxLength - attributeValue.Length)
                                                        Else
                                                            attributeName = roSupport.TruncateScreenText(attributeName, (lineMaxLength \ 2))
                                                            attributeValue = roSupport.TruncateScreenText(attributeValue, (lineMaxLength \ 2))
                                                        End If
                                                    End If
                                                    otmpPunch.RelatedInfo += vbCrLf & attributeName & ": " & attributeValue
                                                End If
                                            End If
                                        Next

                                    End If
                                End If
                            End If

                            oRetPunches.Add(otmpPunch)
                        Next
                    End If
                    lrret.Punches = oRetPunches.ToArray
                    lrret.Status = ErrorCodes.OK
                Else
                    lrret.Punches = {}
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Punches = {}
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::PunchHelper::GetEmployeePunches")
            End Try

            Return lrret
        End Function

        Public Shared Function CorrectPunchDateTime(ByVal oPunch As Punch.roPunch) As DateTime
            Try
                Return CorrectPunchDateTime(oPunch.DateTime, oPunch.TimeZone, oPunch.IDTerminal, oPunch.IDZone, oPunch.IDEmployee)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::PunchHelper::CorrectPunchDateTime-roPunch")

                Return oPunch.DateTime
            End Try
        End Function

        Public Shared Function CorrectPunchDateTime(ByVal oPunch As DataRow) As DateTime

            Try
                Dim idTerminal As Integer = 0
                Dim idZone As Integer = 0
                Dim timeZone As String = ""

                If oPunch("TimeZone") IsNot DBNull.Value Then timeZone = roTypes.Any2String(oPunch("TimeZone"))
                If oPunch("IDTerminal") IsNot DBNull.Value Then idTerminal = roTypes.Any2Integer(oPunch("IDTerminal"))
                If oPunch("IDZone") IsNot DBNull.Value Then idZone = roTypes.Any2Integer(oPunch("IDZone"))

                Return CorrectPunchDateTime(oPunch("DateTime"), timeZone, idTerminal, idZone, oPunch("IDEmployee"))
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::PunchHelper::CorrectPunchDateTime-DataRow")

                Return roTypes.Any2DateTime(oPunch("DateTime"))
            End Try

        End Function

        Private Shared Function CorrectPunchDateTime(ByVal oDateTime As DateTime, ByVal punchTimeZone As String, ByVal idTerminal As Integer, ByVal idZone As Integer, ByVal idEmployee As Integer) As DateTime

            Try
                Dim bContinueSearching As Boolean = True
                'Si no hubiera ninguna zona horaria definida aplicamos la zona del servidor.
                Dim tZoneinfo As TimeZoneInfo = TimeZoneInfo.Local
                Try
                    'Si el fichaje tiene zona horaria asignada aplicaremos esta
                    If punchTimeZone <> String.Empty Then
                        tZoneinfo = TimeZoneInfo.FindSystemTimeZoneById(punchTimeZone)
                        bContinueSearching = False
                    End If

                    'Si no tiene aplicaremos la del termianl
                    If bContinueSearching AndAlso idTerminal > 0 Then
                        Dim oTerminal As New Terminal.roTerminal(idTerminal, New Terminal.roTerminalState)
                        If oTerminal.TimeZoneName <> String.Empty Then
                            tZoneinfo = TimeZoneInfo.FindSystemTimeZoneById(oTerminal.TimeZoneName)
                            bContinueSearching = False
                        End If
                    End If

                    'Si no tiene terminal, pero tiene zona, aplicaremos la de la zona
                    If bContinueSearching AndAlso idZone > 0 Then
                        Dim oZone As New Zone.roZone(idZone, New Zone.roZoneState())
                        If oZone.DefaultTimezone <> String.Empty Then
                            tZoneinfo = TimeZoneInfo.FindSystemTimeZoneById(oZone.DefaultTimezone)
                            bContinueSearching = False
                        End If
                    End If

                    'Aplicaremos la zona horaria que tiene por defecto el empleado
                    If bContinueSearching Then
                        Dim workingZoneID As Integer = -1
                        Dim nonWorkingZoneID As Integer = -1

                        Dim oMobility As Employee.roMobility = Employee.roMobility.GetCurrentMobility(idEmployee, New Employee.roEmployeeState)

                        If oMobility IsNot Nothing Then
                            Group.roGroup.GetGroupZones(oMobility.IdGroup, workingZoneID, nonWorkingZoneID, New Group.roGroupState)
                            If workingZoneID > -1 Then
                                Dim oZone As New Zone.roZone(workingZoneID, New Zone.roZoneState())
                                If oZone.DefaultTimezone <> String.Empty Then
                                    tZoneinfo = TimeZoneInfo.FindSystemTimeZoneById(oZone.DefaultTimezone)
                                    bContinueSearching = False
                                End If
                            End If
                        End If

                    End If
                Catch ex As Exception
                    tZoneinfo = TimeZoneInfo.Local
                End Try

                Return TimeZoneInfo.ConvertTime(oDateTime, tZoneinfo, TimeZoneInfo.Local)
            Catch ex As Exception
                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::PunchHelper::CorrectPunchDateTime-params")

                Return oDateTime
            End Try
        End Function

        Public Shared Function CheckCapacityOnPunch(ByVal direction As String, ByVal latitude As String, ByVal longitude As String, ByVal idEmployee As Integer, ByRef oZone As Zone.roZone) As Boolean
            Dim bRet As Boolean = True

            Try
                If direction = "E" AndAlso latitude.Trim.Length > 0 AndAlso longitude.Trim.Length > 0 Then
                    Dim sWorkCenter As String = String.Empty
                    ' Recupero zona y dentro de trabajo al que estoy accediendo
                    Dim oZoneState As Zone.roZoneState = New Zone.roZoneState(-1)
                    oZone = Zone.roZone.GetZoneFromCoordinates(latitude & "," & longitude, oZoneState)
                    If Not oZone Is Nothing Then
                        If oZone.Capacity.HasValue AndAlso oZone.Capacity.Value > 0 Then
                            Dim tZoneExpectedOccupation As Tuple(Of Integer, Boolean)
                            tZoneExpectedOccupation = Zone.roZone.GetZoneExpectedOccupation(oZone, Now.Date, oZoneState, idEmployee)

                            Dim iOccupation As Integer = tZoneExpectedOccupation.Item1
                            If Not tZoneExpectedOccupation.Item2 Then
                                iOccupation += 1
                            End If

                            ' Si se espera a más emleados de los que caben, y el que ficha no está entr los esperados, aviso y de dejo fichar ....
                            If iOccupation > oZone.Capacity.Value AndAlso Not tZoneExpectedOccupation.Item2 Then
                                bRet = False
                            End If
                        End If
                    End If
                End If
            Catch ex As Exception
                bRet = True
            End Try

            Return bRet
        End Function

    End Class

End Namespace