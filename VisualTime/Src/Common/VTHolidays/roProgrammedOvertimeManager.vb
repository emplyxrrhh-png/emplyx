Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Common.AdvancedParameter
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace VTHolidays

    Public Class roProgrammedOvertimeManager
        Private oState As roProgrammedOvertimeState = Nothing

        Public ReadOnly Property State As roProgrammedOvertimeState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roProgrammedOvertimeState()
        End Sub

        Public Sub New(ByVal _State As roProgrammedOvertimeState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        ''' <summary>
        ''' Carga datos prevision de horas de exceso
        ''' </summary>
        ''' <param name="idProgrammedOvertime"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadProgrammedOvertime(idProgrammedOvertime As Long, Optional ByVal bAudit As Boolean = False) As roProgrammedOvertime

            Dim bolRet As New roProgrammedOvertime

            oState.Result = HolidayResultEnum.NoError

            bolRet.ID = -1

            If idProgrammedOvertime > 0 Then

                Try

                    Dim strSQL As String = "@SELECT# ProgrammedOvertimes.*, " &
                                            "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from CausesDocumentsTracking cdt where cdt.IDCause = ProgrammedOvertimes.IDCause) > 0 Then 1 ELSE 0 END)  AS HasDocuments, " &
                                            "(@SELECT# CASE WHEN (@SELECT# COUNT(*) from sysrovwForecastDocumentsFaults adf where adf.forecastID = ProgrammedOvertimes.ID  AND adf.forecasttype='overtime') > 0 Then 0 ELSE 1 END)  AS DocumentsDelivered " &
                                            " FROM ProgrammedOvertimes " &
                                            "WHERE ID=" & idProgrammedOvertime.ToString

                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        bolRet.ID = Any2Long(tb.Rows(0)("ID"))
                        bolRet.IDEmployee = Any2Integer(tb.Rows(0)("IDEmployee"))
                        bolRet.ProgrammedBeginDate = tb.Rows(0)("BeginDate")
                        bolRet.ProgrammedEndDate = tb.Rows(0)("EndDate")
                        bolRet.IDCause = Any2Integer(tb.Rows(0)("IDCause"))
                        bolRet.RequestId = Any2Integer(tb.Rows(0)("RequestId"))
                        bolRet.Description = Any2String(tb.Rows(0)("Description"))
                        bolRet.Duration = Any2Double(tb.Rows(0)("Duration"))
                        bolRet.BeginTime = tb.Rows(0)("BeginTime")
                        bolRet.EndTime = tb.Rows(0)("EndTime")
                        bolRet.MinDuration = Any2Double(tb.Rows(0)("MinDuration"))
                        bolRet.HasDocuments = Any2Boolean(tb.Rows(0)("HasDocuments"))
                        bolRet.DocumentsDelivered = Any2Boolean(tb.Rows(0)("DocumentsDelivered"))
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDEmployee}", bolRet.IDEmployee, "", 1)
                        oState.AddAuditParameter(tbParameters, "{IDCause}", bolRet.IDCause, "", 1)
                        oState.AddAuditParameter(tbParameters, "{BeginDate}", bolRet.ProgrammedBeginDate, "", 1)
                        oState.AddAuditParameter(tbParameters, "{EndDate}", bolRet.ProgrammedEndDate, "", 1)

                        strSQL = "@SELECT# Name FROM Employees WHERE ID = " & bolRet.IDEmployee.ToString
                        Dim strEmployee As String = Any2String(ExecuteScalar(strSQL))

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedOvertime, strEmployee, tbParameters, -1)
                    End If
                Catch ex As Data.Common.DbException
                    Me.oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::LoadProgrammedOvertime")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::LoadProgrammedOvertime")
                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda una prevision de horas de exceso
        ''' </summary>
        ''' <param name="oProgrammedOvertime"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveProgrammedOvertime(oProgrammedOvertime As roProgrammedOvertime, Optional ByVal bAudit As Boolean = False, Optional CreateTask As Boolean = True) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(oProgrammedOvertime) Then
                    Me.oState.Result = OvertimeResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateProgrammedOvertime(oProgrammedOvertime, Me.State) Then

                    Dim tb As New DataTable("ProgrammedOvertimes")
                    Dim strSQL As String = "@SELECT# * FROM ProgrammedOvertimes WHERE ID=" & oProgrammedOvertime.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim _FreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedOvertime.IDEmployee, False, Me.State)

                    Dim bolLaunchUpdate As Boolean = True

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oRow As DataRow

                    Dim _ProgrammedBeginDateOld As Date = Nothing
                    Dim _ProgrammedEndDateOld As Date = Nothing
                    Dim _IDCauseOld As Integer = 0
                    Dim _BeginTimeOld As Date = Nothing
                    Dim _EndTimeOld As Date = Nothing
                    Dim _DurationOld As Double = 0
                    Dim _MinDurationOld As Double = 0

                    Dim bolActionInsert As Boolean = False

                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bolIsNew = True
                        bolActionInsert = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        _ProgrammedBeginDateOld = oRow("BeginDate")
                        _ProgrammedEndDateOld = oRow("EndDate")
                        _IDCauseOld = oRow("IDCause")
                        _BeginTimeOld = oRow("BeginTime")
                        _EndTimeOld = oRow("EndTime")
                        _DurationOld = Any2Double(oRow("Duration"))
                        _MinDurationOld = Any2Double(oRow("MinDuration"))

                        'Si todos los datos coinciden, no lanzamos recalculo
                        If oProgrammedOvertime.ProgrammedEndDate = _ProgrammedEndDateOld And oProgrammedOvertime.ProgrammedBeginDate = _ProgrammedBeginDateOld And oProgrammedOvertime.BeginTime = _BeginTimeOld And _IDCauseOld = oProgrammedOvertime.IDCause And
                            _EndTimeOld = oProgrammedOvertime.EndTime And Any2Time(_DurationOld).VBNumericValue = Any2Time(oProgrammedOvertime.Duration).VBNumericValue And Any2Time(_MinDurationOld).VBNumericValue = Any2Time(oProgrammedOvertime.MinDuration).VBNumericValue Then
                            bolLaunchUpdate = False
                        End If

                    End If

                    If oProgrammedOvertime.ID > 0 Then
                        oRow("ID") = oProgrammedOvertime.ID
                    Else
                        oRow("ID") = GetNextId()
                        oProgrammedOvertime.ID = oRow("ID")
                    End If

                    oRow("IDEmployee") = oProgrammedOvertime.IDEmployee
                    oRow("BeginDate") = oProgrammedOvertime.ProgrammedBeginDate
                    If oProgrammedOvertime.RequestId.HasValue Then
                        oRow("RequestId") = oProgrammedOvertime.RequestId.Value
                    Else
                        oRow("RequestId") = DBNull.Value
                    End If

                    oRow("EndDate") = oProgrammedOvertime.ProgrammedEndDate
                    oRow("IDCause") = oProgrammedOvertime.IDCause
                    oRow("Description") = oProgrammedOvertime.Description
                    oRow("Duration") = Any2Time(oProgrammedOvertime.EndTime).NumericValue - Any2Time(oProgrammedOvertime.BeginTime).NumericValue

                    oRow("MinDuration") = oProgrammedOvertime.MinDuration
                    oRow("BeginTime") = oProgrammedOvertime.BeginTime
                    oRow("EndTime") = oProgrammedOvertime.EndTime

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    Dim oLicense As New roServerLicense

                    ' Auditamos
                    If bolRet And bAudit Then
                        oAuditDataNew = oRow
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)

                        strSQL = "@SELECT# Name FROM Employees WHERE ID = " & oProgrammedOvertime.IDEmployee.ToString
                        Dim strEmployee As String = Any2String(ExecuteScalar(strSQL))

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tProgrammedOvertime, strEmployee, tbParameters, -1)

                    End If

                    ' Notificamos cambio al servidor
                    If bolRet And bolLaunchUpdate Then

                        'Actualiza la table de DailySchedule del dia anterior/actual y posterior
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status = 55, [GUID] = '' WHERE Status > 55 And IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).Add(-1, "d").SQLSmallDateTime & " Or (Date >= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime & "))" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                        'Elimina la justificacion de esos dias
                        strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).Add(-1, "d").SQLSmallDateTime & " Or (Date >= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime & "))" &
                                    " And ( IDCause = " & oProgrammedOvertime.IDCause.ToString & ")" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime &
                                    " And ( Manual = 0)"
                        If bolRet Then
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        End If

                        If Not bolIsNew And bolRet Then
                            ' Si es una modificacion debemos actualizar tambien los datos antiguos

                            'Actualiza la table de DailySchedule del dia anterior/actual y posterior
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status = 55, [GUID] = '' WHERE Status > 55 And IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(_ProgrammedBeginDateOld).Add(-1, "d").SQLSmallDateTime & " Or (Date >= " & roTypes.Any2Time(_ProgrammedBeginDateOld).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(_ProgrammedEndDateOld).SQLSmallDateTime & "))" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                            strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(_ProgrammedBeginDateOld).Add(-1, "d").SQLSmallDateTime & " Or (Date >= " & roTypes.Any2Time(_ProgrammedBeginDateOld).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(_ProgrammedEndDateOld).SQLSmallDateTime & "))" &
                                    " And ( IDCause = " & _IDCauseOld.ToString & ")" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime &
                                    " And ( Manual = 0)"
                            If bolRet Then
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            End If
                        End If

                        ' Crea la tarea de prevision de horas de exceso
                        If CreateTask And bolRet Then
                            Dim oContext As New roCollection
                            oContext.Add("Employee.ID", oProgrammedOvertime.IDEmployee)
                            oContext.Add("Date", Now.Date)
                            roConnector.InitTask(TasksType.PROGRAMMEDABSENCES, oContext)
                        End If
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::SaveProgrammedOvertime")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::SaveProgrammedOvertime")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        '''' <summary>
        '''' Elimina  una prevision de horas de exceso
        '''' </summary>
        '''' <param name="oProgrammedOvertime"></param>
        '''' <param name="oActiveTransaction"></param>
        '''' <param name="bAudit"></param>
        '''' <returns></returns>
        Public Function DeleteProgrammedOvertime(oProgrammedOvertime As roProgrammedOvertime, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim tb As New DataTable("ProgrammedOvertimes")
                Dim strSQL As String = "@SELECT# * FROM ProgrammedOvertimes WHERE ID=" & oProgrammedOvertime.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb.Rows.Count > 0 Then

                    If Not IsDBNull(tb.Rows(0).Item("BeginDate")) Then oProgrammedOvertime.ProgrammedBeginDate = tb.Rows(0).Item("BeginDate")
                    If Not IsDBNull(tb.Rows(0).Item("EndDate")) Then oProgrammedOvertime.ProgrammedEndDate = tb.Rows(0).Item("EndDate")
                    If Not IsDBNull(tb.Rows(0).Item("IDCause")) Then oProgrammedOvertime.IDCause = CInt(tb.Rows(0).Item("IDCause"))
                    If Not IsDBNull(tb.Rows(0).Item("BeginTime")) Then oProgrammedOvertime.BeginTime = tb.Rows(0).Item("BeginTime")
                    If Not IsDBNull(tb.Rows(0).Item("EndTime")) Then oProgrammedOvertime.BeginTime = tb.Rows(0).Item("EndTime")

                    tb.Rows(0).Delete()

                    da.Update(tb)

                    bolRet = True
                End If

                'Verificamos si se encuentra en periodo de congelacion
                Dim _freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedOvertime.IDEmployee, False, Me.oState)

                If bolRet Then
                    If oProgrammedOvertime.ProgrammedBeginDate <= _freezeDate Or oProgrammedOvertime.ProgrammedEndDate <= _freezeDate Then
                        Me.State.Result = OvertimeResultEnum.InFreezeDate
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    ' En el caso de que tenga el parametro avanzado de no permitir gestionar justificaciones que
                    ' no esten dentro de sus grupos de negocio, lo validamos
                    If roTypes.Any2String(New roAdvancedParameter("BusinessGroup.ApplyNotAllowedModifyCause", New roAdvancedParameterState).Value) = "1" Then
                        Dim tbCauses As DataTable = Nothing

                        Dim strQueryCauses = " @SELECT# * FROM Causes "
                        Dim strBusiness As String = GetBusinessGroupList()
                        If strBusiness <> String.Empty Then
                            strQueryCauses &= " WHERE (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                        End If

                        strQueryCauses &= " ORDER BY Name"

                        tbCauses = CreateDataTable(strQueryCauses)

                        If tbCauses IsNot Nothing Then
                            Dim oRows() As DataRow = tbCauses.Select("ID = " & oProgrammedOvertime.IDCause)
                            If oRows IsNot Nothing AndAlso oRows.Length = 0 Then
                                Me.State.Result = OvertimeResultEnum.NotAllowedCause
                                bolRet = False
                            End If
                        End If
                    End If
                End If

                'Actualiza la table de DailySchedule del dia anterior/actual
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status = 55, [GUID] = '' WHERE Status > 55 And IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " And " &
                                 "(Date = " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).Add(-1, "d").SQLSmallDateTime & " Or (Date >= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime & "))" &
                                 " And Date > " & roTypes.Any2Time(_freezeDate).SQLSmallDateTime
                If bolRet Then
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                'Elimina la justificacion de esos dias
                strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " And " &
                                 "(Date = " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).Add(-1, "d").SQLSmallDateTime & " Or (Date >= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime & " and Date <= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime & "))" &
                                 " And ( IDCause = " & oProgrammedOvertime.IDCause.ToString & ")" &
                                 " And Date > " & roTypes.Any2Time(_freezeDate).SQLSmallDateTime &
                                 " And ( manual = 0)"
                If bolRet Then
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet Then
                    Try
                        strSQL = "@DELETE# sysroNotificationTasks WHERE Key1Numeric = " & oProgrammedOvertime.IDEmployee & " AND Key5Numeric = " & oProgrammedOvertime.ID & " AND IDNotification = 701 AND Parameters LIKE 'OVERTIME%'"
                        bolRet = ExecuteSql(strSQL)
                    Catch ex As Exception
                    End Try
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    strSQL = "@SELECT# Name FROM Employees WHERE ID = " & oProgrammedOvertime.IDEmployee.ToString
                    Dim strEmployee As String = Any2String(ExecuteScalar(strSQL))

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProgrammedOvertime, strEmployee, Nothing, -1)
                End If

                If bolRet Then
                    Dim oContext As New roCollection
                    oContext.Add("Employee.ID", oProgrammedOvertime.IDEmployee)
                    oContext.Add("Date", Now.Date)

                    roConnector.InitTask(TasksType.PROGRAMMEDABSENCES, oContext)

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::DeleteProgrammedOvertime")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::DeleteProgrammedOvertime")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function GetBusinessGroupList() As String
            Dim strRet As String = String.Empty
            Try

                Dim strQuery As String
                strQuery = "@SELECT# BusinessGroupList FROM sysroGroupFeatures WHERE ID IN(@SELECT# isnull(IDGroupFeature,-1) from sysroPassports WHERE id = " & oState.IDPassport & " ) "
                Dim dtBusinessGroups As System.Data.DataTable = CreateDataTable(strQuery)
                If dtBusinessGroups IsNot Nothing And dtBusinessGroups.Rows.Count > 0 Then

                    Dim arrAux() As String = roTypes.Any2String(dtBusinessGroups.Rows(0)("BusinessGroupList")).Split(";")
                        For Each item As String In arrAux
                            If item.Trim() <> String.Empty Then
                                strRet &= "'" & item.Trim().Replace("'", "''") & "',"
                            End If
                        Next
                        If strRet.Length > 0 Then strRet = strRet.Substring(0, strRet.Length() - 1)

                    End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedOvertimeManager::GetBusinessGroupList")
            End Try

            Return strRet

        End Function

        Public Function GetProgrammedOvertimes(ByVal _IDEmployee As Integer, ByVal _State As roProgrammedOvertimeState, Optional ByVal strWhere As String = "") As List(Of roProgrammedOvertime)

            Dim oRet As New List(Of roProgrammedOvertime)

            Try

                _State.Result = HolidayResultEnum.NoError

                Dim strSQL As String
                strSQL = "@SELECT# ProgrammedOvertimes.ID, IDEmployee,IDCause, BeginDate,EndDate, BeginTime, EndTime, isnull(convert(numeric(8,6), Duration),0) as duration, ProgrammedOvertimes.Description, isnull(convert(numeric(8,6), MinDuration),0) as Minduration,  " &
                                " Causes.Name " &
                         "FROM ProgrammedOvertimes " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedOvertimes.IDCause " &
                         "WHERE idEmployee = " & _IDEmployee

                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If

                strSQL = strSQL & " ORDER BY BeginDate DESC"

                Dim tbHolidays As DataTable = CreateDataTable(strSQL)
                If (tbHolidays IsNot Nothing AndAlso tbHolidays.Rows.Count > 0) Then
                    For Each rowholiday As DataRow In tbHolidays.Rows
                        Dim ProgrammedOvertime = New roProgrammedOvertime
                        ProgrammedOvertime = LoadProgrammedOvertime(rowholiday("Id"))
                        oRet.Add(ProgrammedOvertime)
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedOvertime::GetProgrammedOvertimes")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedOvertime::GetProgrammedOvertimes")
            End Try

            Return oRet

        End Function

#End Region

#Region "Helper Methods"

        Public Shared Function ValidateProgrammedOvertime(ByVal oProgrammedOvertime As roProgrammedOvertime, ByVal _State As roProgrammedOvertimeState) As Boolean

            Dim bolRet As Boolean = False

            Try

                _State.Result = OvertimeResultEnum.NoError

                Dim queryDateStart As String = roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime()
                Dim queryDateEnd As String = Nothing

                Dim tmpEndDate As Date

                tmpEndDate = oProgrammedOvertime.ProgrammedEndDate
                queryDateEnd = roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime()

                Dim queryStartHour As String = roTypes.Any2Time(oProgrammedOvertime.BeginTime).SQLDateTime()
                Dim queryEndHour As String = roTypes.Any2Time(oProgrammedOvertime.EndTime).SQLDateTime()

                Dim strSQL As String

                If oProgrammedOvertime.ProgrammedBeginDate > tmpEndDate Then
                    _State.Result = OvertimeResultEnum.InvalidDate
                ElseIf roTypes.Any2Time(oProgrammedOvertime.EndTime).NumericValue = 0 Then
                    _State.Result = OvertimeResultEnum.InvalidDateTimeInterval
                ElseIf oProgrammedOvertime.BeginTime > oProgrammedOvertime.EndTime Then
                    _State.Result = OvertimeResultEnum.InvalidDateTimeInterval
                ElseIf oProgrammedOvertime.Duration = 0 Or roTypes.Any2Time(oProgrammedOvertime.Duration).NumericValue > roTypes.Any2Time("23:59").NumericValue Then
                    _State.Result = OvertimeResultEnum.InvalidDuration
                ElseIf roTypes.Any2Time(oProgrammedOvertime.Duration).NumericValue > roTypes.Any2Time(roTypes.Any2Time(oProgrammedOvertime.EndTime).NumericValue - roTypes.Any2Time(oProgrammedOvertime.BeginTime).NumericValue).NumericValue Then
                    _State.Result = OvertimeResultEnum.InvalidDuration
                ElseIf roTypes.Any2Time(oProgrammedOvertime.Duration).NumericValue < roTypes.Any2Time(oProgrammedOvertime.MinDuration).NumericValue Then
                    _State.Result = OvertimeResultEnum.InvalidDuration
                Else

                    ' Verificamos si existe otra prevision de exceso en el mismo periodo
                    strSQL = "@SELECT# * from ProgrammedOvertimes " &
                             "WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " AND "
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
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) ) "

                    strSQL &= " AND ID  <> " & oProgrammedOvertime.ID.ToString

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then

                        If tb.Rows.Count > 0 Then
                            _State.Result = OvertimeResultEnum.AnotherExistInDay
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                If bolRet Then
                    ' revisamos si existe otra prevision de ausencias por horas en el mismo periodo

                    strSQL = "@SELECT# * from ProgrammedCauses " &
                                 "WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " AND "

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

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        If tb.Rows.Count > 0 Then
                            _State.Result = OvertimeResultEnum.AnotherAbsenceExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                'Verificamos si la prevision de exceso se encuentra en periodo de congelacion
                If bolRet Then
                    Dim freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedOvertime.IDEmployee, False, _State)
                    Dim _IDCauseOld As Integer = oProgrammedOvertime.IDCause
                    Dim _ProgrammedBeginDateOld As Date = Nothing
                    Dim _ProgrammedEndDateOld As Date = Nothing

                    Dim _BeginTimeOld As Date = Nothing
                    Dim _EndTimeOld As Date = Nothing

                    'Si es una modificacion de una existente
                    If oProgrammedOvertime.ID > 0 Then
                        'Recuperamos los datos antiguos
                        Dim tb As New DataTable("ProgrammedOvertimes")
                        strSQL = "@SELECT# * FROM ProgrammedOvertimes WHERE "
                        strSQL &= "ProgrammedOvertimes.ID = " & oProgrammedOvertime.ID.ToString

                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        If tb.Rows.Count > 0 Then
                            _ProgrammedBeginDateOld = tb.Rows(0)("BeginDate")
                            _ProgrammedEndDateOld = tb.Rows(0)("EndDate")
                            _IDCauseOld = tb.Rows(0)("IDCause")
                            _BeginTimeOld = tb.Rows(0)("BeginTime")
                            _EndTimeOld = tb.Rows(0)("EndTime")

                        End If

                        ' Si alguna de las fechas esta en periodo de congelacion no dejamos modificar
                        If oProgrammedOvertime.ProgrammedBeginDate <= freezeDate Or _ProgrammedBeginDateOld <= freezeDate Then
                            _State.Result = OvertimeResultEnum.InFreezeDate
                            bolRet = False
                        End If

                        If oProgrammedOvertime.ProgrammedEndDate <= freezeDate Or _ProgrammedEndDateOld <= freezeDate Then
                            _State.Result = OvertimeResultEnum.InFreezeDate
                            bolRet = False
                        End If
                    Else
                        'Si es una nueva prevision, validamos si se encuentra en periodo de congelacion
                        If oProgrammedOvertime.ProgrammedBeginDate <= freezeDate Then
                            _State.Result = OvertimeResultEnum.InFreezeDate
                            bolRet = False
                        End If

                        If oProgrammedOvertime.ProgrammedEndDate <= freezeDate Then
                            _State.Result = OvertimeResultEnum.InFreezeDate
                            bolRet = False
                        End If

                    End If
                End If

                'Verificamos si la prevision de exceso se encuentra dentro del periodo del contrato.
                If bolRet Then
                    strSQL = "@SELECT# * FROM EmployeeContracts WHERE " &
                             "BeginDate <= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedBeginDate).SQLSmallDateTime() & " AND " &
                             "BeginDate <= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(oProgrammedOvertime.ProgrammedEndDate).SQLSmallDateTime() & " AND " &
                             "IDEmployee = " & oProgrammedOvertime.IDEmployee
                    Dim dTblC As DataTable = CreateDataTable(strSQL, )
                    If dTblC Is Nothing OrElse dTblC.Rows.Count = 0 Then
                        _State.Result = OvertimeResultEnum.DateOutOfContract
                        bolRet = False
                    End If
                End If

                ' Ausencia prolongada para el mismo dia
                If bolRet Then
                    strSQL = "@SELECT# * from ProgrammedAbsences WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee & " AND "

                    strSQL &= " ( ( (BeginDate >= " & queryDateStart & " AND BeginDate <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDateEnd & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & queryDateStart & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDateEnd & ") )"

                    Dim tbX As DataTable = CreateDataTable(strSQL, )
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            _State.Result = OvertimeResultEnum.AnotherAbsenceExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                If bolRet Then
                    ' Verificamos si existe otra prevision de vacaciones por horas en el mismo periodo
                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                 "WHERE IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " AND "
                    strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND "
                    strSQL &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                    strSQL &= " OR "
                    strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                    ' o una prevision de vacaciones de todo el dia para la misma fecha
                    strSQL &= " Or (AllDay = 1))"

                    Dim tbr As DataTable = CreateDataTable(strSQL, )
                    If tbr IsNot Nothing Then

                        If tbr.Rows.Count > 0 Then
                            _State.Result = OvertimeResultEnum.AnotherHolidayExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                If bolRet Then
                    Dim oAdvParam As New AdvancedParameter.roAdvancedParameter("VTLive.Absences.ValidateHolidayOnDate", New AdvancedParameter.roAdvancedParameterState())
                    If roTypes.Any2Boolean(oAdvParam.Value) Then
                        strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                                                    "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & oProgrammedOvertime.IDEmployee.ToString & " AND "
                        strSQL &= " Date >= " & queryDateStart & " AND Date <= " & queryDateEnd & " AND ( isnull(IsHolidays,0) = 1 ) "
                        Dim tb As DataTable = CreateDataTable(strSQL, )
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
                _State.UpdateStateInfo(ex, "roProgrammedOvertime::ValidateProgrammedOvertime")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedOvertime::ValidateProgrammedOvertime")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetNextId() As Long
            Dim bolRet As Boolean = True
            Dim retValue As Long = 0

            Try

                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), 0) + 1) FROM ProgrammedOvertimes "

                retValue = roTypes.Any2Long(ExecuteScalar(strQry))
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roProgrammedOvertime:GetNextId")
            End Try

            Return retValue
        End Function

#End Region

    End Class

End Namespace