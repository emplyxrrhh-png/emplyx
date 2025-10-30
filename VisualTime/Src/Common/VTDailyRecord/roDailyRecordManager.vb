Imports System.Data.Common
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTRequests
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace VTDailyRecord

    Public Class roDailyRecordManager

        Private oState As roDailyRecordState = Nothing

        Public ReadOnly Property State As roDailyRecordState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roDailyRecordState()
        End Sub

        Public Sub New(ByVal _State As roDailyRecordState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        Public Function SaveDailyRecord(ByRef oDailyRecord As roDailyRecord, ByVal dRequestDate As Date, ByVal acceptWarning As Boolean, ByVal oSupervisor As roPassportTicket, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim strCRC As String = String.Empty
            Dim bHaveToClose As Boolean = False

            Try
                oState.Result = DailyRecordResultEnum.NoError

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not ValidateDailyRecord(oDailyRecord, strCRC) Then
                    Return False
                End If

                Dim oRequestState As Requests.roRequestState = New Requests.roRequestState(oState.IDPassport)
                Dim oRequestdailyRecord As New Requests.roRequest(-1, oRequestState)
                oRequestdailyRecord.RequestDate = dRequestDate
                oRequestdailyRecord.IDEmployee = oDailyRecord.IdEmployee
                oRequestdailyRecord.strDate1 = oDailyRecord.RecordDate.ToString("yyyy-MM-dd HH:mm")
                oRequestdailyRecord.IDCause = Nothing
                oRequestdailyRecord.Comments = String.Empty
                oRequestdailyRecord.Date2 = Nothing
                oRequestdailyRecord.Field1 = String.Empty
                oRequestdailyRecord.RequestType = eRequestType.DailyRecord
                oRequestdailyRecord.RequestStatus = eRequestStatus.Pending
                oRequestdailyRecord.CRC = strCRC

                ' Guardamos la solicitud SIN generar la notificación para el supervisor
                If oRequestdailyRecord.SaveWithParams(acceptWarning, True,,,, False) Then
                    ' Guardamos fichajes de la declaración
                    Dim oPunchList As New List(Of Punch.roPunch)
                    oDailyRecord.Id = oRequestdailyRecord.ID
                    oPunchList = oDailyRecord.Punches.ToList.ConvertAll(AddressOf XDailyRecordPunchToPunch)

                    Dim oTerminals As List(Of Terminal.roTerminal) = Terminal.roTerminal.GetEmployeeTerminals(oDailyRecord.IdEmployee, "LIVEPORTAL", New Terminal.roTerminalState())
                    Dim oTerminal As Terminal.roTerminal = New Terminal.roTerminal

                    If oTerminals IsNot Nothing AndAlso oTerminals.Count > 0 Then
                        oTerminal = oTerminals(0)
                    End If

                    For Each oPunch As Punch.roPunch In oPunchList
                        oPunch.IDRequest = oDailyRecord.Id
                        oPunch.IDTerminal = oTerminal.ID
                        oPunch.NotReliableCause = DTOs.NotReliableCause.DailyRecord.ToString()
                        bolRet = oPunch.Save(False)
                        If Not bolRet Then
                            oState.Result = DailyRecordResultEnum.ErrorSavingDailyRecordPunch
                            Exit For
                        End If
                    Next
                    Dim oContext As New roCollection
                    oContext.Add("User.ID", oDailyRecord.IdEmployee)
                    roConnector.InitTask(TasksType.MOVES, oContext)
                Else
                    bolRet = False
                    oState.Result = DailyRecordResultEnum.ErrorSavingDailyRecordRequest
                End If

                ' Si la solicitud la está creando un supervisor, lanzo la aprobación de la solicitud
                If bolRet AndAlso oSupervisor IsNot Nothing Then
                    Dim strApproveRefuse As String = oState.Language.Translate("Requests.ApprovedBy", "")
                    If Not oRequestdailyRecord.ApproveRefuse(oSupervisor.ID, True, strApproveRefuse & oSupervisor.Name,, False, False, True) Then
                        oState.Result = DailyRecordResultEnum.ErrorAprovingDailyRecordMadeBySupervisor
                    End If
                End If

                If bolRet AndAlso bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tDailyRecord, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::SaveDailyRecord")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::SaveDailyRecord")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        Public Overloads Function GetCRC(oDailyRecord As roDailyRecord) As String
            Dim strCRC As String = String.Empty
            Try
                Dim oCurrentPunch As roDailyRecordPunch = Nothing
                For i = 0 To oDailyRecord.Punches.Length - 1
                    oCurrentPunch = oDailyRecord.Punches(i)
                    If (i + 1) Mod 2 <> 0 Then
                        ' Entrada
                        If strCRC.Length > 0 Then
                            strCRC = strCRC & "*"
                        End If
                        strCRC = strCRC & oCurrentPunch.DateTime.ToString("yyyyMMddHHmmss")
                    Else
                        ' Salida
                        strCRC = strCRC & "-" & oCurrentPunch.DateTime.ToString("yyyyMMddHHmmss")
                    End If
                Next
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "Error al calcular el CRC de la declaración diaria", ex)
            End Try

            Return CryptographyHelper.EncryptWithMD5(strCRC)
        End Function

        Public Function DeleteDailyRecord(ByVal idDailyRecord As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                oState.Result = DailyRecordResultEnum.NoError

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If idDailyRecord > 0 Then
                    bolRet = True
                    '0.- Borramos todos los fichajes
                    'strSQL = "@DELETE# Punches WHERE IdRequest = " & idDailyRecord.ToString
                    'bolRet = ExecuteSql(strSQL)

                    '1.- Borramos solicitud y sus fichajes
                    Dim oReqState As Requests.roRequestState = New Requests.roRequestState(Me.oState.IDPassport)
                    Dim oRequest As New Requests.roRequest(idDailyRecord, oReqState, False)
                    Dim oPunchState As New Punch.roPunchState
                    If oRequest IsNot Nothing Then
                        ' borramos los fichajes
                        Dim tbPunches As DataTable = Punch.roPunch.GetPunches("IdRequest = " & idDailyRecord.ToString, oPunchState)
                        If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                            For Each orow As DataRow In tbPunches.Rows
                                orow.Delete()
                            Next
                            Dim oPunches As New Punch.roPunchList(oPunchState)
                            bolRet = oPunches.Save(tbPunches, False, False, False)
                            If bolRet Then
                                Dim oContext As New roCollection
                                oContext.Add("User.ID", oRequest.IDEmployee)
                                roConnector.InitTask(TasksType.MOVES, oContext)
                            End If
                        End If

                        If bolRet Then
                            ' Borramos la solicitud
                            bolRet = oRequest.Delete()
                        End If
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet
        End Function

        Private Function ValidateDailyRecord(oDailyRecord As roDailyRecord, ByRef strCRC As String) As Boolean
            Dim oRet As Boolean = False

            Try
                ' No se puede hacer una declaración a futuro, ni hoy
                If oDailyRecord.RecordDate >= Date.Now Then
                    oState.Result = DailyRecordResultEnum.FutureDailyRecordNotAllowed
                    Return oRet
                End If

                ' No se pueden hacer declaraciones para días congelados
                Dim xFreezeDate As Date = Common.roBusinessSupport.GetEmployeeLockDatetoApply(oDailyRecord.IdEmployee, False, Me.oState)
                If oDailyRecord.RecordDate <= xFreezeDate Then
                    oState.Result = DailyRecordResultEnum.InFrozenPeriod
                    Return oRet
                End If

                Select Case Robotics.Base.VTBusiness.Punch.roDailyRecordPunchHelper.CheckDailyRecordPunches(oDailyRecord.Punches, strCRC)
                    Case DailyRecordPunchesResultEnum.Exception
                        oState.Result = DailyRecordResultEnum.Exception
                        Return oRet
                    Case DailyRecordPunchesResultEnum.InvalidSequence
                        oState.Result = DailyRecordResultEnum.UnconsistentDailyRecordPunches
                        Return oRet
                    Case DailyRecordPunchesResultEnum.PunchesNumberShouldBeEven
                        oState.Result = DailyRecordResultEnum.DailyRecordOddNumberOfPunches
                        Return oRet
                    Case DailyRecordPunchesResultEnum.PunchesListCantBeEmpty
                        oState.Result = DailyRecordResultEnum.DailyRecordMustContainPunches
                        Return oRet
                    Case DailyRecordPunchesResultEnum.PunchesOverlaped
                        oState.Result = DailyRecordResultEnum.DailyRecordPunchesOverlaped
                        Return oRet
                    Case DailyRecordPunchesResultEnum.PunchRepeated
                        oState.Result = DailyRecordResultEnum.DailyRecordHasRepeatedPunches
                        Return oRet
                    Case DailyRecordPunchesResultEnum.Exception
                        oState.Result = DailyRecordResultEnum.Exception
                        Return oRet
                End Select

                ' No se pueden hacer declaraciones para un día en el que ya hay una
                Dim strSQL As String = String.Empty
                strSQL = "@SELECT# COUNT(*) FROM Requests WHERE IDEmployee = @idemployee AND RequestType = @idrequesttype AND Status IN (0,1,2) AND Date1 = @date1"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idrequesttype", CommandParameter.ParameterType.tInt, 17))
                parameters.Add(New CommandParameter("@idemployee", CommandParameter.ParameterType.tInt, oDailyRecord.IdEmployee))
                parameters.Add(New CommandParameter("@date1", CommandParameter.ParameterType.tDateTime, oDailyRecord.RecordDate))
                Dim iTotal As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL, parameters))

                If iTotal > 0 Then
                    oState.Result = DailyRecordResultEnum.DailyRecordAlreadyExistsOnDate
                    Return False
                End If

                oRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::ValidateDailyRecord")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::ValidateDailyRecord")
            End Try
            Return oRet
        End Function

        Public Function LoadDailyRecord(ByVal idDailyRecord As Integer, ByRef oDailyRecordRequest As Requests.roRequest, Optional ByVal bAudit As Boolean = False) As roDailyRecord
            Dim oDailyRecord As roDailyRecord = Nothing
            Dim lDailyRecordPunches As New List(Of roDailyRecordPunch)

            Try

                oState.Result = DailyRecordResultEnum.NoError

                Dim oReqState As Requests.roRequestState = New Requests.roRequestState(oState.IDPassport)
                If oDailyRecordRequest Is Nothing Then oDailyRecordRequest = New Requests.roRequest(idDailyRecord, oReqState)

                If oDailyRecordRequest IsNot Nothing Then
                    oDailyRecord = New roDailyRecord
                    oDailyRecord.Id = oDailyRecordRequest.ID
                    oDailyRecord.IdEmployee = oDailyRecordRequest.IDEmployee
                    oDailyRecord.RecordDate = oDailyRecordRequest.Date1
                    ' Cargo tiempos y estado
                    Dim iMargin As Integer = 0
                    Dim oShift As New Shift.roShift()
                    Dim dExpected As Double = oShift.GetExpectedWorkingHoursForEmployeeOnDate(oDailyRecord.IdEmployee, oDailyRecord.RecordDate)
                    Dim dAccrued As Double = Concept.roConcept.GetAccrualValueOnDateForDailyRecordCheck(oDailyRecord.IdEmployee, oDailyRecord.RecordDate, iMargin)
                    oDailyRecord.TimeAccrued = If(dAccrued >= 0, roTypes.Any2Time(dAccrued).Minutes, -1 * roTypes.Any2Time(-1 * dAccrued).Minutes)
                    oDailyRecord.TimeExpected = roTypes.Any2Time(dExpected).Minutes
                    oDailyRecord.Adjusted = (Math.Abs(oDailyRecord.TimeExpected - oDailyRecord.TimeAccrued) <= Math.Abs(iMargin))
                    oDailyRecord.DailyRecordInfo = Me.oState.Language.Translate("DailyRecordInfo.DeclarationFor", "") + " " + oDailyRecord.RecordDate.ToString("dd/MM/yyyy") + " (" + Me.oState.Language.Translate("DailyRecordInfo.Adjusted", "") + ")"
                    If Not oDailyRecord.Adjusted Then
                        oDailyRecord.DailyRecordInfo = Me.oState.Language.Translate("DailyRecordInfo.DeclarationFor", "") + " " + oDailyRecord.RecordDate.ToString("dd/MM/yyyy") + " (" + Me.oState.Language.Translate("DailyRecordInfo.NotAdjusted", "") + ")"
                        If oDailyRecord.TimeAccrued = 0 Then
                            oDailyRecord.DailyRecordInfo = Me.oState.Language.Translate("DailyRecordInfo.DeclarationFor", "") + " " + oDailyRecord.RecordDate.ToString("dd/MM/yyyy") + " (" + Me.oState.Language.Translate("DailyRecordInfo.NotAdjusted.NotAccrued", "") + ")"
                        End If
                    End If

                    Select Case oDailyRecordRequest.RequestStatus
                        Case eRequestStatus.Pending, eRequestStatus.OnGoing
                            oDailyRecord.DailyRecordStatus = eDailyRecordDateStatus.OnGoing
                        Case eRequestStatus.Accepted
                            oDailyRecord.DailyRecordStatus = eDailyRecordDateStatus.Done
                        Case eRequestStatus.Canceled, eRequestStatus.Denied
                            oDailyRecord.DailyRecordStatus = eDailyRecordDateStatus.Pending
                    End Select

                    '1.- Cargo Fichajes
                    Dim tbPunches As DataTable
                    Dim strFilter As String = "IDEmployee = " & oDailyRecord.IdEmployee.ToString &
                                              " AND ActualType IN (1,2) " &
                                              " AND IdRequest = " & oDailyRecord.Id &
                                              " ORDER BY DateTime ASC"

                    Dim oPunchState As New Punch.roPunchState(oState.IDPassport)
                    tbPunches = Punch.roPunch.GetPunches(strFilter, oPunchState)

                    Dim oDailyRecordPunch As roDailyRecordPunch = Nothing
                    If tbPunches IsNot Nothing AndAlso tbPunches.Rows.Count > 0 Then
                        For Each oRow As DataRow In tbPunches.Rows
                            oDailyRecordPunch = New roDailyRecordPunch
                            oDailyRecordPunch.ShiftDate = oRow("ShiftDate")
                            oDailyRecordPunch.IdDailyRecord = If(oRow("IdRequest") Is DBNull.Value, -1, oRow("IdRequest"))
                            oDailyRecordPunch.IdEmployee = oDailyRecord.IdEmployee
                            oDailyRecordPunch.DateTime = oRow("DateTime")
                            oDailyRecordPunch.IDCause = roTypes.Any2Integer(oRow("TypeData"))
                            oDailyRecordPunch.CauseName = Cause.roCause.GetCauseNameByID(oDailyRecordPunch.IDCause)
                            oDailyRecordPunch.IDZone = oRow("IDZone")
                            oDailyRecordPunch.ZoneName = Zone.roZone.GetZoneNameByID(oDailyRecordPunch.IDZone)
                            oDailyRecordPunch.InTelecommute = roTypes.Any2Boolean(oRow("InTelecommute"))
                            oDailyRecordPunch.Type = oRow("ActualType")
                            lDailyRecordPunches.Add(oDailyRecordPunch)
                        Next
                        oDailyRecord.Punches = lDailyRecordPunches.ToArray
                    Else
                        oDailyRecord.Punches = {}
                    End If

                    oDailyRecord.Modified = (oDailyRecordRequest.CRC <> GetCRC(oDailyRecord))
                Else
                    oState.Result = DailyRecordResultEnum.DailyRecordDoesNotExists
                    Return oDailyRecord
                End If

                If bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDailyRecord, "", tbParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::LoadCommunique")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::LoadCommunique")
            End Try
            Return oDailyRecord
        End Function

        Public Function LoadDailyRecordCalendar(idEmployee As Integer, dDateFrom As Date, dDateTo As Date, Optional ByVal bAudit As Boolean = False) As roDailyRecordCalendar
            Dim retDailyRecordCalendar As roDailyRecordCalendar = New roDailyRecordCalendar

            Dim strSQL As String = String.Empty
            Dim lstDailyRecordCalendar As New List(Of roDailyRecordCalendarItem)

            Try
                Dim maxNumberOfDaysPastValue As Integer
                Dim maxNumberOfDaysPast As String = roTypes.Any2String(roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName, "VTPortal.DailyRecord.MaxNumberOfDaysPast"))

                If Not Integer.TryParse(maxNumberOfDaysPast, maxNumberOfDaysPastValue) Then
                    maxNumberOfDaysPastValue = -1
                End If

                oState.Result = CommuniqueResultEnum.NoError

                strSQL = "@SELECT# DailySchedule.IDEmployee, " &
                         "    alldays.dt as Date, " &
                         "    CASE WHEN ISNULL(DailySchedule.IsHolidays,0) = 0 THEN isnull(programmedholidays.allday,0) " &
                         "    ELSE 1 END AS IsHoliday," &
                         "    ISNULL(DailySchedule.FeastDay, 0) IsFeast, " &
                         "    ISNULL(ProgrammedAbsences.AbsenceID,0) AbsenceId,  " &
                         "    ISNULL(sysrovwTelecommutingAgreement.TelecommutingAgreementSource, '') TelecommuteAgreementType,  " &
                         "    ISNULL(Requests.Id, -1) IDDailyReport, " &
                         "    ISNULL(Shifts.ExpectedWorkingHours,0) ExpectedWorkingHours, " &
                         "	  Requests.Status DailyReportStatus, " &
                         "    (@SELECT# EC.BeginDate from EmployeeContracts EC WHERE EC.IDEmployee = " & idEmployee.ToString & " AND DailySchedule.Date between EC.BeginDate and EC.EndDate) as ContractBeginDate, " &
                         "    (@SELECT# count(*) FROM ShiftsPunchesPattern WHERE IDshift =  Dailyschedule.IDShift1) As PatternPunches " &
                         "    FROM Alldays(" & roTypes.Any2Time(dDateFrom).SQLSmallDateTime & "," & roTypes.Any2Time(dDateTo).SQLSmallDateTime & ") alldays " &
                         "    LEFT JOIN DailySchedule ON DailySchedule.Date = alldays.dt AND DailySchedule.IDEmployee = " & idEmployee.ToString &
                         "    LEFT JOIN sysrovwTelecommutingAgreement ON sysrovwTelecommutingAgreement.IDEmployee = " & idEmployee.ToString & " AND alldays.dt BETWEEN sysrovwTelecommutingAgreement.TelecommutingAgreementStart AND sysrovwTelecommutingAgreement.TelecommutingAgreementEnd  " &
                         "    LEFT JOIN ProgrammedAbsences ON  ProgrammedAbsences.IDEmployee = " & idEmployee.ToString & " AND alldays.dt BETWEEN ProgrammedAbsences.BeginDate AND ISNULL(ProgrammedAbsences.FinishDate, dateadd(day,ProgrammedAbsences.MaxLastingDays - 1, ProgrammedAbsences.BeginDate))  " &
                         "    LEFT JOIN Requests ON Requests.IDEmployee = " & idEmployee.ToString & " AND Requests.date1 = alldays.dt AND Requests.RequestType = 17  " &
                         "    LEFT JOIN Shifts ON Shifts.ID = DailySchedule.IDShift1 " &
                         "    LEFT JOIN ProgrammedHolidays ON  ProgrammedHolidays.IDEmployee = " & idEmployee & " AND ProgrammedHolidays.Date = alldays.dt AND programmedholidays.allday = 1" &
                         "ORDER BY alldays.dt ASC, Requests.ID DESC"

                Dim tbCalendar As DataTable = Nothing
                tbCalendar = CreateDataTable(strSQL)

                Dim oNewDailyRecordCalendarItem As roDailyRecordCalendarItem = Nothing
                If tbCalendar IsNot Nothing AndAlso tbCalendar.Rows.Count > 0 Then
                    For Each oRow As DataRow In tbCalendar.Rows
                        If lstDailyRecordCalendar.Find(Function(d) d.Date = roTypes.Any2DateTime(oRow("Date")).Date) Is Nothing Then

                            oNewDailyRecordCalendarItem = New roDailyRecordCalendarItem
                            oNewDailyRecordCalendarItem.Date = roTypes.Any2DateTime(oRow("Date")).Date

                            If oRow("DailyReportStatus") IsNot DBNull.Value Then
                                Select Case oRow("DailyReportStatus")
                                    Case eRequestStatus.Accepted
                                        oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.Done
                                    Case eRequestStatus.Denied, eRequestStatus.Canceled
                                        oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.Pending
                                    Case Else
                                        oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.OnGoing
                                End Select
                            ElseIf oNewDailyRecordCalendarItem.Date > Now.Date OrElse (maxNumberOfDaysPastValue >= 0 AndAlso Now.Date.Subtract(oNewDailyRecordCalendarItem.Date).Days > maxNumberOfDaysPastValue) Then 'Por decisión de producto, se permite declarar en: oRow("IsHoliday") OrElse oRow("AbsenceId") > 0
                                oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.NotAllowed
                            ElseIf oRow("ExpectedWorkingHours") = 0 OrElse roTypes.Any2Integer(oRow("AbsenceID")) > 0 Then
                                oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.NotNeeded
                            ElseIf oRow("DailyReportStatus") Is DBNull.Value Then
                                oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.Pending
                            End If

                            If oRow("ContractBeginDate") Is DBNull.Value Then
                                oNewDailyRecordCalendarItem.DateStatus = eDailyRecordDateStatus.NotAllowed
                            End If

                            oNewDailyRecordCalendarItem.IdEmployee = idEmployee
                            oNewDailyRecordCalendarItem.IdRecord = oRow("IDDailyReport")
                            oNewDailyRecordCalendarItem.IdReport = oNewDailyRecordCalendarItem.IdRecord
                            oNewDailyRecordCalendarItem.CanTelecommute = (oRow("TelecommuteAgreementType") <> String.Empty)
                            oNewDailyRecordCalendarItem.HasPunchesPattern = (roTypes.Any2Integer(oRow("PatternPunches")) > 0)
                            lstDailyRecordCalendar.Add(oNewDailyRecordCalendarItem)
                        End If
                    Next
                    retDailyRecordCalendar.DayData = lstDailyRecordCalendar.ToArray
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::DailyRecordCalendar")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::DailyRecordCalendar")
            End Try
            Return retDailyRecordCalendar
        End Function

        Public Function LoadPunchesPattern(idEmployee As Integer, dDate As Date) As roDailyRecordPunchesPattern
            Dim oDailyRecordPunchesPattern As roDailyRecordPunchesPattern = New roDailyRecordPunchesPattern
            Dim strSQL As String
            Dim lDailyRecordPunches As New List(Of roDailyRecordPunch)

            Try

                oState.Result = DailyRecordResultEnum.NoError

                strSQL = "@SELECT# ShiftsPunchesPattern.DateTime, ShiftsPunchesPattern.PunchType FROM DailySchedule " &
                            "INNER JOIN Shifts On Shifts.ID = DailySchedule.IDShift1 " &
                            "INNER JOIN ShiftsPunchesPattern ON ShiftsPunchesPattern.IDShift = Shifts.ID " &
                            "WHERE DailySchedule.IDEmployee = " & idEmployee.ToString & " AND DailySchedule.Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime &
                            " ORDER BY ShiftsPunchesPattern.DateTime ASC	"

                Dim oPunch As roDailyRecordPunch
                Dim tbPattern As DataTable = Nothing
                Dim dPunchDateTime As DateTime = DateTime.MinValue
                tbPattern = CreateDataTable(strSQL)

                If tbPattern IsNot Nothing AndAlso tbPattern.Rows.Count > 0 Then
                    For Each oPatternRow As DataRow In tbPattern.Rows
                        oPunch = New roDailyRecordPunch
                        oPunch.IdEmployee = idEmployee
                        dPunchDateTime = roTypes.Any2DateTime(oPatternRow("DateTime"))
                        oPunch.DateTime = New Date(dDate.Year, dDate.Month, dDate.Day, dPunchDateTime.Hour, dPunchDateTime.Minute, 0)
                        If dPunchDateTime.Day = 31 Then oPunch.DateTime = oPunch.DateTime.AddDays(1)
                        oPunch.ShiftDate = dDate
                        oPunch.Type = PunchTypeEnum._IN
                        If roTypes.Any2Integer(oPatternRow("PunchType")) = 2 Then oPunch.Type = PunchTypeEnum._OUT
                        lDailyRecordPunches.Add(oPunch)
                    Next
                End If

                oDailyRecordPunchesPattern.Punches = lDailyRecordPunches.ToArray
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::LoadPunchesPattern")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roDailyRecordManager::LoadPunchesPattern")
            End Try
            Return oDailyRecordPunchesPattern
        End Function

        Private Function XDailyRecordPunchToPunch(oDailyRecordPunch As roDailyRecordPunch) As Punch.roPunch
            Dim oRet As Punch.roPunch
            Try
                oRet = New Punch.roPunch

                oRet.Type = oDailyRecordPunch.Type
                oRet.ActualType = oDailyRecordPunch.Type
                oRet.ShiftDate = oDailyRecordPunch.ShiftDate
                oRet.DateTime = oDailyRecordPunch.DateTime
                oRet.TypeData = oDailyRecordPunch.IDCause
                oRet.InTelecommute = oDailyRecordPunch.InTelecommute
                oRet.IDZone = oDailyRecordPunch.IDZone
                oRet.IDEmployee = oDailyRecordPunch.IdEmployee
                If oRet.IDZone <= 0 Then oRet.IDZone = Punch.roPunch.DEFAULT_ZONE
                oRet.IsNotReliable = True
            Catch ex As Exception
                oRet = Nothing
            End Try
            Return oRet
        End Function

        Public Function GetPortalErrorCode(ByVal roErrorCode As Integer) As Integer
            Dim returnValue As Integer = ErrorCodes.GENERAL_ERROR

            Select Case roErrorCode
                Case DailyRecordResultEnum.NoError
                    returnValue = ErrorCodes.OK
                Case DailyRecordResultEnum.ConnectionError
                    returnValue = ErrorCodes.REQUEST_ERROR_ConnectionError
                Case DailyRecordResultEnum.InFrozenPeriod
                    returnValue = ErrorCodes.DAILYRECORD_INCORRECT_DATE_FROZEN
                Case DailyRecordResultEnum.DailyRecordOddNumberOfPunches
                    returnValue = ErrorCodes.DAILYRECORD_ODD_NUMBER_OF_PUNCHES
                Case DailyRecordResultEnum.DailyRecordAlreadyExistsOnDate
                    returnValue = ErrorCodes.DAILYRECORD_INCORRECT_DATE_ALREADY_EXISTS
                Case DailyRecordResultEnum.ErrorSavingDailyRecordPunch
                    returnValue = ErrorCodes.FORBID_ERROR_SAVE_PUNCH
                Case DailyRecordResultEnum.FutureDailyRecordNotAllowed
                    returnValue = ErrorCodes.DAILYRECORD_INCORRECT_DATE_FUTURE
                Case DailyRecordResultEnum.ErrorSavingDailyRecordRequest
                    returnValue = ErrorCodes.DAILYRECORD_INCORRECT_DATE_FUTURE
                Case DailyRecordResultEnum.UnconsistentDailyRecordPunches
                    returnValue = ErrorCodes.DAILYRECORD_WRONG_PUNCHES_SEQUENCE
                Case DailyRecordResultEnum.ErrorLoadingDailyRecord
                    returnValue = ErrorCodes.DAILYRECORD_ERROR_LOADING
                Case DailyRecordResultEnum.ErrorLoadingDailyRecordCalendar
                    returnValue = ErrorCodes.DAILYRECORD_ERROR_LOADING_CALENDAR
                Case DailyRecordResultEnum.ErrorAprovingDailyRecordMadeBySupervisor
                    returnValue = ErrorCodes.DAILYRECORD_ERROR_APPROVING_WHEN_MADE_BY_SUPERVISOR
                Case DailyRecordResultEnum.DailyRecordDoesNotExists
                    returnValue = ErrorCodes.DAILYRECORD_ERROR_DOES_NOT_EXISTS
                Case DailyRecordResultEnum.DailyRecordMustContainPunches
                    returnValue = ErrorCodes.DAILYRECORD_ERROR_NO_PUNCHES
                Case DailyRecordResultEnum.DailyRecordPunchesOverlaped
                    returnValue = ErrorCodes.DAILYRECORD_PUNCHES_OVERLAPED
                Case DailyRecordResultEnum.DailyRecordHasRepeatedPunches
                    returnValue = ErrorCodes.DAILYRECORD_HAS_REPEATED_PUNCHES
                Case DailyRecordResultEnum.Exception
                    returnValue = ErrorCodes.GENERAL_ERROR
            End Select
            Return returnValue
        End Function

#End Region

    End Class

End Namespace