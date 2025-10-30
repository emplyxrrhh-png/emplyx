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

    Public Class roProgrammedHolidayManager
        Private oState As roProgrammedHolidayState = Nothing

        Public ReadOnly Property State As roProgrammedHolidayState
            Get
                Return oState
            End Get
        End Property

#Region "Constructores"

        Public Sub New()
            oState = New roProgrammedHolidayState()
        End Sub

        Public Sub New(ByVal _State As roProgrammedHolidayState)
            oState = _State
        End Sub

#End Region

#Region "Métodos"

        ''' <summary>
        ''' Carga datos prevision de vacaciones por horas
        ''' </summary>
        ''' <param name="idProgrammedHoliday"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function LoadProgrammedHoliday(idProgrammedHoliday As Long, Optional ByVal bAudit As Boolean = False) As roProgrammedHoliday

            Dim bolRet As New roProgrammedHoliday

            oState.Result = HolidayResultEnum.NoError

            bolRet.ID = -1

            If idProgrammedHoliday > 0 Then

                Try

                    Dim strSQL As String = "@SELECT# * FROM ProgrammedHolidays " &
                                           "WHERE ID=" & idProgrammedHoliday.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        bolRet.ID = Any2Long(tb.Rows(0)("ID"))
                        bolRet.IDEmployee = Any2Integer(tb.Rows(0)("IDEmployee"))
                        bolRet.ProgrammedDate = tb.Rows(0)("Date")
                        bolRet.IDCause = Any2Integer(tb.Rows(0)("IDCause"))
                        bolRet.Description = Any2String(tb.Rows(0)("Description"))
                        bolRet.AllDay = Any2Boolean(tb.Rows(0)("AllDay"))

                        If bolRet.AllDay Then
                            bolRet.BeginTime = New DateTime(1899, 12, 30, 0, 0, 0)
                            bolRet.EndTime = New DateTime(1899, 12, 30, 0, 0, 0)
                            bolRet.Duration = 0
                        Else
                            bolRet.BeginTime = tb.Rows(0)("BeginTime")
                            bolRet.EndTime = tb.Rows(0)("EndTime")
                            bolRet.Duration = Any2Time(bolRet.EndTime).NumericValue - Any2Time(bolRet.BeginTime).NumericValue
                        End If
                    End If

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDEmployee}", bolRet.IDEmployee, "", 1)
                        oState.AddAuditParameter(tbParameters, "{IDCause}", bolRet.IDCause, "", 1)
                        oState.AddAuditParameter(tbParameters, "{Date}", bolRet.ProgrammedDate, "", 1)

                        strSQL = "@SELECT# Name FROM Employees WHERE ID = " & bolRet.IDEmployee.ToString
                        Dim strEmployee As String = Any2String(ExecuteScalar(strSQL))

                        Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tProgrammedHoliday, strEmployee, tbParameters, -1)
                    End If
                Catch ex As Data.Common.DbException
                    Me.oState.UpdateStateInfo(ex, "roProgrammedHolidayManager::LoadProgrammedHoliday")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roProgrammedHolidayManager::LoadProgrammedHoliday")
                End Try

            End If

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda una prevision de vacaciones por horas
        ''' </summary>
        ''' <param name="oProgrammedHoliday"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <param name="bAudit"></param>
        ''' <returns></returns>
        Public Function SaveProgrammedHoliday(oProgrammedHoliday As roProgrammedHoliday, Optional ByVal bAudit As Boolean = False, Optional CreateTask As Boolean = True) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(oProgrammedHoliday) Then
                    Me.oState.Result = HolidayResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If ValidateProgrammedHoliday(oProgrammedHoliday, Me.State) Then

                    Dim tb As New DataTable("ProgrammedHolidays")
                    Dim strSQL As String = "@SELECT# * FROM ProgrammedHolidays WHERE ID=" & oProgrammedHoliday.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim _FreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedHoliday.IDEmployee, False, Me.State)

                    Dim bolLaunchUpdate As Boolean = True

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oRow As DataRow

                    Dim _ProgrammedDateOld As Date = Nothing
                    Dim _IDCauseOld As Integer = 0
                    Dim _AllDayOld As Boolean = False
                    Dim _BeginTimeOld As Date = Nothing
                    Dim _EndTimeOld As Date = Nothing
                    Dim _DurationOld As Double = 0
                    Dim bolActionInsert As Boolean = False

                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bolIsNew = True
                        bolActionInsert = True
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        _ProgrammedDateOld = oRow("Date")
                        _IDCauseOld = oRow("IDCause")
                        _AllDayOld = oRow("AllDay")
                        _BeginTimeOld = oRow("BeginTime")
                        _EndTimeOld = oRow("EndTime")
                        _DurationOld = oRow("Duration")

                        'Si todos los datos coinciden, no lanzamos recalculo
                        If oProgrammedHoliday.ProgrammedDate = _ProgrammedDateOld AndAlso oProgrammedHoliday.BeginTime = _BeginTimeOld AndAlso _IDCauseOld = oProgrammedHoliday.IDCause AndAlso
                            _EndTimeOld = oProgrammedHoliday.EndTime AndAlso _AllDayOld = oProgrammedHoliday.AllDay AndAlso _DurationOld = oProgrammedHoliday.Duration Then
                            bolLaunchUpdate = False
                        End If

                    End If

                    If oProgrammedHoliday.ID > 0 Then
                        oRow("ID") = oProgrammedHoliday.ID
                    Else
                        oRow("ID") = GetNextId()
                        oProgrammedHoliday.ID = oRow("ID")
                    End If

                    oRow("IDEmployee") = oProgrammedHoliday.IDEmployee
                    oRow("Date") = oProgrammedHoliday.ProgrammedDate
                    oRow("IDCause") = oProgrammedHoliday.IDCause
                    oRow("Description") = oProgrammedHoliday.Description
                    oRow("AllDay") = oProgrammedHoliday.AllDay

                    If oProgrammedHoliday.AllDay Then
                        oRow("BeginTime") = New DateTime(1899, 12, 30, 0, 0, 0)
                        oRow("EndTime") = New DateTime(1899, 12, 30, 0, 0, 0)
                        oRow("Duration") = 0
                    Else
                        oRow("BeginTime") = oProgrammedHoliday.BeginTime
                        oRow("EndTime") = oProgrammedHoliday.EndTime
                        oRow("Duration") = Any2Time(oProgrammedHoliday.EndTime).NumericValue - Any2Time(oProgrammedHoliday.BeginTime).NumericValue
                    End If

                    If bolLaunchUpdate Then oRow("Timestamp") = Now

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

                        strSQL = "@SELECT# Name FROM Employees WHERE ID = " & oProgrammedHoliday.IDEmployee.ToString
                        Dim strEmployee As String = Any2String(ExecuteScalar(strSQL))

                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Me.oState.Audit(If(bolActionInsert, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tProgrammedHoliday, strEmployee, tbParameters, -1)

                    End If

                    ' Notificamos cambio al servidor
                    If bolRet And bolLaunchUpdate Then

                        'Actualiza la table de DailySchedule del dia anterior/actual y posterior
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status = 55, [GUID] = '' WHERE Status > 55 And IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " Or Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime & ")" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                        'Elimina la justificacion de esos dias
                        strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " Or Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime & ")" &
                                    " And ( IDCause = " & oProgrammedHoliday.IDCause.ToString & ")" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime &
                                    " And ( Manual = 0)"
                        If bolRet Then
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        End If

                        If Not bolIsNew And bolRet Then
                            ' Si es una modificacion debemos actualizar tambien los datos antiguos

                            'Actualiza la table de DailySchedule del dia anterior/actual y posterior
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status = 55, [GUID] = '' WHERE Status > 55 And IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(_ProgrammedDateOld).Add(-1, "d").SQLSmallDateTime & " Or Date = " & roTypes.Any2Time(_ProgrammedDateOld).SQLSmallDateTime & ")" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                            strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " And " &
                                    "(Date = " & roTypes.Any2Time(_ProgrammedDateOld).Add(-1, "d").SQLSmallDateTime & " Or Date = " & roTypes.Any2Time(_ProgrammedDateOld).SQLSmallDateTime & ")" &
                                    " And ( IDCause = " & _IDCauseOld.ToString & ")" &
                                    " And Date > " & roTypes.Any2Time(_FreezeDate).SQLSmallDateTime &
                                    " And ( Manual = 0)"
                            If bolRet Then
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            End If

                        End If

                        ' Crea la tarea de prevision de vacaciones por horas
                        If CreateTask And bolRet Then
                            Dim oContext As New roCollection
                            oContext.Add("Employee.ID", oProgrammedHoliday.IDEmployee)
                            oContext.Add("Date", Now.Date)
                            roConnector.InitTask(TasksType.PROGRAMMEDABSENCES, oContext)
                        End If
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedHolidayManager::SaveProgrammedHoliday")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedHolidayManager::SaveProgrammedHoliday")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        '''' <summary>
        '''' Elimina  una prevision de vacaciones por horas
        '''' </summary>
        '''' <param name="oProgrammedHoliday"></param>
        '''' <param name="oActiveTransaction"></param>
        '''' <param name="bAudit"></param>
        '''' <returns></returns>
        Public Function DeleteProgrammedHoliday(oProgrammedHoliday As roProgrammedHoliday, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim tb As New DataTable("ProgrammedHolidays")
                Dim strSQL As String = "@SELECT# * FROM ProgrammedHolidays WHERE ID=" & oProgrammedHoliday.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb.Rows.Count > 0 Then

                    If Not IsDBNull(tb.Rows(0).Item("Date")) Then oProgrammedHoliday.ProgrammedDate = tb.Rows(0).Item("Date")
                    If Not IsDBNull(tb.Rows(0).Item("IDCause")) Then oProgrammedHoliday.IDCause = CInt(tb.Rows(0).Item("IDCause"))
                    If Not IsDBNull(tb.Rows(0).Item("BeginTime")) Then oProgrammedHoliday.BeginTime = tb.Rows(0).Item("BeginTime")
                    If Not IsDBNull(tb.Rows(0).Item("EndTime")) Then oProgrammedHoliday.BeginTime = tb.Rows(0).Item("EndTime")

                    tb.Rows(0).Delete()

                    da.Update(tb)

                    bolRet = True
                End If

                'Verificamos si se encuentra en periodo de congelacion
                Dim _freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedHoliday.IDEmployee, False, Me.State)

                If bolRet Then
                    If oProgrammedHoliday.ProgrammedDate <= _freezeDate Then
                        Me.State.Result = HolidayResultEnum.InFreezeDate
                        bolRet = False
                    End If
                End If

                If bolRet Then
                    ' En el caso de que tenga el parametro avanzado de no permitir gestionar justificaciones que
                    ' no esten dentro de sus grupos de negocio, lo validamos
                    If roTypes.Any2String(New roAdvancedParameter("BusinessGroup.ApplyNotAllowedModifyCause", New roAdvancedParameterState, Nothing).Value) = "1" Then
                        Dim tbCauses As DataTable = Nothing

                        Dim strQueryCauses = " @SELECT# * FROM Causes "
                        Dim strBusiness As String = GetBusinessGroupList()
                        If strBusiness <> String.Empty Then
                            strQueryCauses &= " WHERE (ISNULL(BusinessGroup,'') = '' OR ISNULL(BusinessGroup,'') IN (" & strBusiness & ")) "
                        End If

                        strQueryCauses &= " ORDER BY Name"

                        tbCauses = CreateDataTable(strQueryCauses)

                        If tbCauses IsNot Nothing Then
                            Dim oRows() As DataRow = tbCauses.Select("ID = " & oProgrammedHoliday.IDCause)
                            If oRows IsNot Nothing AndAlso oRows.Length = 0 Then
                                Me.State.Result = HolidayResultEnum.NotAllowedCause
                                bolRet = False
                            End If
                        End If
                    End If
                End If

                'Actualiza la table de DailySchedule del dia anterior/actual y posterior
                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) Set Status = 55, [GUID] = '' WHERE Status > 55 And IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " And " &
                                 "(Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " Or Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime & ")" &
                                 " And Date > " & roTypes.Any2Time(_freezeDate).SQLSmallDateTime
                If bolRet Then
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                'Elimina la justificacion de esos dias
                strSQL = "@DELETE# DailyCauses WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " And " &
                                 "(Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).Add(-1, "d").SQLSmallDateTime & " Or Date = " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime & ")" &
                                 " And ( IDCause = " & oProgrammedHoliday.IDCause.ToString & ")" &
                                 " And Date > " & roTypes.Any2Time(_freezeDate).SQLSmallDateTime &
                                 " And ( manual = 0)"
                If bolRet Then
                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    strSQL = "@SELECT# Name FROM Employees WHERE ID = " & oProgrammedHoliday.IDEmployee.ToString
                    Dim strEmployee As String = Any2String(ExecuteScalar(strSQL))

                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tProgrammedHoliday, strEmployee, Nothing, -1)
                End If

                If bolRet Then
                    Dim oContext As New roCollection
                    oContext.Add("Employee.ID", oProgrammedHoliday.IDEmployee)
                    oContext.Add("Date", Now.Date)

                    roConnector.InitTask(TasksType.PROGRAMMEDABSENCES, oContext)

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roProgrammedHolidayManager::DeleteProgrammedHoliday")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roProgrammedHolidayManager::DeleteProgrammedHoliday")
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
                oState.UpdateStateInfo(ex, "roProgrammedHoliday::GetBusinessGroupList")
            End Try

            Return strRet

        End Function

        Public Function RegisterDeleteProgrammedHoliday(ByVal IDEmployee As Integer, Optional ByVal IdHoursHoliday As Integer = 0, Optional ByVal PlannedDate As DateTime = Nothing, Optional dNow As DateTime = Nothing) As Boolean
            Dim bolRet As Boolean = False
            Dim strSQL As String = ""

            Try
                If dNow = Nothing Then dNow = Date.Now
                If IdHoursHoliday > 0 Then
                    bolRet = ExecuteSql("@INSERT# INTO DeletedProgrammedHolidays (IDEmployee, IDHoursHoliday, PlannedDate, TimeStamp) VALUES(" & IDEmployee.ToString & "," & IdHoursHoliday.ToString & "," & Any2Time(PlannedDate).SQLDateTime & "," & Any2Time(dNow).SQLDateTime & ")")
                Else
                    bolRet = ExecuteSql("@INSERT# INTO DeletedProgrammedHolidays (IDEmployee, PlannedDate, TimeStamp) VALUES(" & IDEmployee.ToString & "," & Any2Time(PlannedDate).SQLDateTime & "," & Any2Time(dNow).SQLDateTime & ")")
                End If
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roProgrammedHoliday::RegisterDeleteProgrammedHoliday")
            End Try

            Return bolRet

        End Function

        Public Function GetProgrammedHolidays(ByVal _IDEmployee As Integer, ByVal _State As roProgrammedHolidayState, Optional ByVal strWhere As String = "") As List(Of roProgrammedHoliday)

            Dim oRet As New List(Of roProgrammedHoliday)

            Try

                _State.Result = HolidayResultEnum.NoError

                Dim strSQL As String
                strSQL = "@SELECT# ProgrammedHolidays.ID, IDEmployee,IDCause, Date, BeginTime, EndTime, isnull(convert(numeric(8,6), Duration),0) as duration, ProgrammedHolidays.Description, " &
                                "ProgrammedHolidays.AllDay , Causes.Name " &
                         "FROM ProgrammedHolidays " &
                                    "LEFT JOIN Causes On Causes.ID = ProgrammedHolidays.IDCause " &
                         "WHERE idEmployee = " & _IDEmployee

                If strWhere <> "" Then
                    strSQL = strSQL & " AND " & strWhere
                End If

                strSQL = strSQL & " ORDER BY Date DESC"

                Dim tbHolidays As DataTable = CreateDataTable(strSQL)
                If (tbHolidays IsNot Nothing AndAlso tbHolidays.Rows.Count > 0) Then
                    For Each rowholiday As DataRow In tbHolidays.Rows
                        Dim ProgrammedHoliday = New roProgrammedHoliday
                        ProgrammedHoliday = LoadProgrammedHoliday(rowholiday("Id"))
                        oRet.Add(ProgrammedHoliday)
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedHoliday::GetProgrammedHolidays")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedHoliday::GetProgrammedHolidays")
            End Try

            Return oRet

        End Function

#End Region

#Region "Helper Methods"

        Public Shared Function ValidateProgrammedHoliday(ByVal oProgrammedHoliday As roProgrammedHoliday, ByVal _State As roProgrammedHolidayState) As Boolean

            Dim bolRet As Boolean = False

            Try

                _State.Result = HolidayResultEnum.NoError

                Dim queryDate As String = roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime()

                Dim queryStartHour As String = roTypes.Any2Time(oProgrammedHoliday.BeginTime).SQLDateTime()
                Dim queryEndHour As String = roTypes.Any2Time(oProgrammedHoliday.EndTime).SQLDateTime()

                Dim strSQL As String

                ' Si la prevision es de un periodo en concreto
                If Not oProgrammedHoliday.AllDay Then
                    If roTypes.Any2Time(oProgrammedHoliday.EndTime).NumericValue = 0 Then
                        _State.Result = HolidayResultEnum.InvalidDateTimeInterval
                    ElseIf Any2Time(oProgrammedHoliday.BeginTime).NumericValue > Any2Time(oProgrammedHoliday.EndTime).NumericValue Then
                        _State.Result = HolidayResultEnum.InvalidDateTimeInterval
                    ElseIf Any2Time(oProgrammedHoliday.BeginTime).NumericValue = Any2Time(oProgrammedHoliday.EndTime).NumericValue Then
                        _State.Result = HolidayResultEnum.InvalidDateTimeInterval
                    Else

                        ' Verificamos si existe otra prevision de vacaciones por horas en el mismo periodo
                        strSQL = "@SELECT# * from ProgrammedHolidays " &
                                 "WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                        strSQL &= " Date = " & queryDate & " AND "
                        strSQL &= " ((" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime AND AllDay = 0)"
                        strSQL &= " OR "
                        strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime  AND AllDay = 0)"
                        strSQL &= " OR "
                        strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime AND AllDay = 0)"
                        strSQL &= " OR "
                        strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND AllDay = 0)  "
                        ' o una prevision de vacaciones de todo el dia para la misma fecha
                        strSQL &= " Or (AllDay = 1))"
                        strSQL &= " AND ID  <> " & oProgrammedHoliday.ID.ToString

                        Dim tb As DataTable = CreateDataTable(strSQL, )
                        If tb IsNot Nothing Then

                            If tb.Rows.Count > 0 Then
                                _State.Result = HolidayResultEnum.AnotherHolidayExistInDate
                                bolRet = False
                            Else
                                bolRet = True
                            End If
                        End If
                    End If
                Else
                    ' Si es de todo el dia, verificamos que no hay otra prevision de vacaciones el mismo dia
                    ' ya sea de todo el dia o de un único periodo
                    strSQL = "@SELECT# * from ProgrammedHolidays " &
                                 "WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                    strSQL &= " Date = " & queryDate
                    strSQL &= " AND ID  <> " & oProgrammedHoliday.ID.ToString
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        If tb.Rows.Count > 0 Then
                            _State.Result = HolidayResultEnum.AnotherHolidayExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                'Verificamos si la prevision de vacaciones por horas se encuentra en periodo de congelacion
                If bolRet Then
                    Dim freezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oProgrammedHoliday.IDEmployee, False, _State)
                    Dim _IDCauseOld As Integer = oProgrammedHoliday.IDCause
                    Dim _ProgrammedDateOld As Date = Nothing
                    Dim _BeginTimeOld As Date = Nothing
                    Dim _EndTimeOld As Date = Nothing
                    Dim _AllDayOld As Boolean = False

                    'Si es una modificacion de una existente
                    If oProgrammedHoliday.ID > 0 Then
                        'Recuperamos los datos antiguos
                        Dim tb As New DataTable("ProgrammedHolidays")
                        strSQL = "@SELECT# * FROM ProgrammedHolidays WHERE "
                        strSQL &= "ProgrammedHolidays.ID = " & oProgrammedHoliday.ID.ToString

                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        If tb.Rows.Count > 0 Then
                            _ProgrammedDateOld = tb.Rows(0)("Date")
                            _IDCauseOld = tb.Rows(0)("IDCause")
                            _BeginTimeOld = tb.Rows(0)("BeginTime")
                            _EndTimeOld = tb.Rows(0)("EndTime")
                            _AllDayOld = tb.Rows(0)("AllDay")
                        End If

                        ' Si alguna de las fechas esta en periodo de congelacion no dejamos modificar
                        If oProgrammedHoliday.ProgrammedDate <= freezeDate Or _ProgrammedDateOld <= freezeDate Then
                            _State.Result = HolidayResultEnum.InFreezeDate
                            bolRet = False
                        End If
                    Else
                        'Si es una nueva prevision, validamos si se encuentra en periodo de congelacion
                        If oProgrammedHoliday.ProgrammedDate <= freezeDate Then
                            _State.Result = HolidayResultEnum.InFreezeDate
                            bolRet = False
                        End If
                    End If
                End If

                'Verificamos si la prevision de vaca por horas se encuentra dentro del periodo del contrato.
                If bolRet Then
                    strSQL = "@SELECT# * FROM EmployeeContracts WHERE " &
                             "BeginDate <= " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime() & " AND " &
                             "EndDate >= " & roTypes.Any2Time(oProgrammedHoliday.ProgrammedDate).SQLSmallDateTime() & " AND " &
                             "IDEmployee = " & oProgrammedHoliday.IDEmployee
                    Dim dTblC As DataTable = CreateDataTable(strSQL, )
                    If dTblC Is Nothing OrElse dTblC.Rows.Count = 0 Then
                        _State.Result = HolidayResultEnum.DateOutOfContract
                        bolRet = False
                    End If
                End If

                ' Verificamos si existe una prevision de dias de ausencia para el mismo dia
                If bolRet Then
                    strSQL = "@SELECT# * from ProgrammedAbsences WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                    strSQL &= " ( ( (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                    strSQL &= " OR "
                    strSQL &= " (ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) <= " & queryDate & ")"
                    strSQL &= " OR "
                    strSQL &= " (BeginDate <= " & queryDate & " AND ISNULL(FinishDate, DATEADD(DAY,MaxLastingDays - 1,BeginDate )) >= " & queryDate & ") ) )"

                    Dim tbX As DataTable = CreateDataTable(strSQL, )
                    If tbX IsNot Nothing Then
                        If tbX.Rows.Count > 0 Then
                            _State.Result = HolidayResultEnum.AnotherAbsenceExistInDate
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If

                If bolRet Then
                    ' Verificamos si existe una prevision de horas de ausencia o exceso para el mismo dia
                    If Not oProgrammedHoliday.AllDay Then
                        ' Si es de un periodo concreto
                        strSQL = "@SELECT# * from ProgrammedCauses " &
                             "WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                        strSQL &= " ( ( (Date >= " & queryDate & " AND Date <= " & queryDate & ")"
                        strSQL &= " OR "
                        strSQL &= " (IsNULL(FinishDate,Date) >= " & queryDate & " AND IsNULL(FinishDate,Date) <= " & queryDate & ")"
                        strSQL &= " OR "
                        strSQL &= " (Date <= " & queryDate & " AND IsNULL(FinishDate,Date) >= " & queryDate & ") ) AND  ("

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
                                _State.Result = HolidayResultEnum.AnotherAbsenceExistInDate
                                bolRet = False
                            Else
                                bolRet = True
                            End If
                        End If

                        If bolRet Then
                            strSQL = "@SELECT# * from ProgrammedOvertimes " &
                             "WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                            strSQL &= " ( (  (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                            strSQL &= " OR "
                            strSQL &= " (EndDate >= " & queryDate & " AND EndDate <= " & queryDate & ")"
                            strSQL &= " OR "
                            strSQL &= " (BeginDate <= " & queryDate & " AND EndDate >= " & queryDate & ") )  AND  ("

                            strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " >  EndTime)"
                            strSQL &= " OR "
                            strSQL &= " (" & queryStartHour & " < BeginTime  AND " & queryEndHour & " BETWEEN BeginTime AND EndTime AND " & queryEndHour & " <> BeginTime )"
                            strSQL &= " OR "
                            strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime AND  EndTime <> " & queryStartHour & "  AND " & queryEndHour & " > EndTime)"
                            strSQL &= " OR "
                            strSQL &= " (" & queryStartHour & " BETWEEN BeginTime AND EndTime   AND " & queryEndHour & " BETWEEN BeginTime AND EndTime ) ) ) "

                            Dim tbx As DataTable = CreateDataTable(strSQL, )
                            If tbx IsNot Nothing Then
                                If tbx.Rows.Count > 0 Then
                                    _State.Result = HolidayResultEnum.AnotherOvertimeExistInDate
                                    bolRet = False
                                Else
                                    bolRet = True
                                End If
                            End If

                        End If
                    Else
                        ' si es de un dia completo
                        strSQL = "@SELECT# * from ProgrammedCauses " &
                             "WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                        strSQL &= "  ( (Date >= " & queryDate & " AND Date <= " & queryDate & ")"
                        strSQL &= " OR "
                        strSQL &= " (IsNULL(FinishDate,Date) >= " & queryDate & " AND IsNULL(FinishDate,Date) <= " & queryDate & ")"
                        strSQL &= " OR "
                        strSQL &= " (Date <= " & queryDate & " AND IsNULL(FinishDate,Date) >= " & queryDate & ") )"
                        Dim tb As DataTable = CreateDataTable(strSQL, )
                        If tb IsNot Nothing Then
                            If tb.Rows.Count > 0 Then
                                _State.Result = HolidayResultEnum.AnotherAbsenceExistInDate
                                bolRet = False
                            Else
                                bolRet = True
                            End If
                        End If

                        If bolRet Then
                            strSQL = "@SELECT# * from ProgrammedOvertimes " &
                             "WHERE IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                            strSQL &= "  (  (BeginDate >= " & queryDate & " AND BeginDate <= " & queryDate & ")"
                            strSQL &= " OR "
                            strSQL &= " (EndDate >= " & queryDate & " AND EndDate <= " & queryDate & ")"
                            strSQL &= " OR "
                            strSQL &= " (BeginDate <= " & queryDate & " AND EndDate >= " & queryDate & ") )  "
                            Dim tbw As DataTable = CreateDataTable(strSQL, )
                            If tbw IsNot Nothing Then
                                If tbw.Rows.Count > 0 Then
                                    _State.Result = HolidayResultEnum.AnotherOvertimeExistInDate
                                    bolRet = False
                                Else
                                    bolRet = True
                                End If
                            End If

                        End If
                    End If
                End If

                If bolRet Then
                    ' Verificamos que la prevision no esté asignada a un dia con un horario de vacaciones planificado o de no trabajo
                    strSQL = "@SELECT# isnull(IsHolidays,0)  as Holidays from DailySchedule, Shifts " &
                             "WHERE Shifts.ID = DailySchedule.IDShift1  AND IDEmployee = " & oProgrammedHoliday.IDEmployee.ToString & " AND "
                    strSQL &= " Date = " & queryDate & " AND ( isnull(IsHolidays,0) = 1 OR  isnull(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)  = 0 ) "
                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        If tb.Rows.Count > 0 Then
                            _State.Result = HolidayResultEnum.InHolidayPlanification
                            bolRet = False
                        Else
                            bolRet = True
                        End If
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roProgrammedHoliday::ValidateProgrammedHoliday")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roProgrammedHoliday::ValidateProgrammedHoliday")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetNextId() As Long
            Dim bolRet As Boolean = True
            Dim retValue As Long = 0

            Try
                Dim strQry As String = "@SELECT# (ISNULL(MAX(ID), 0) + 1) FROM ProgrammedHolidays "

                retValue = roTypes.Any2Long(ExecuteScalar(strQry))
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roProgrammedHoliday:GetNextId")
            End Try

            Return retValue
        End Function

#End Region

    End Class

End Namespace