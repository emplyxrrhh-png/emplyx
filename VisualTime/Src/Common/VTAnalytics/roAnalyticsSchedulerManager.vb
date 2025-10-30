Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Support
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Analytics.Manager

#Region "roAnalyticsSchedulerManager"

    <DataContract()>
    Public Class roAnalyticsSchedulerManager

#Region "Declarations - Constructor"

        Private oState As roAnalyticState

        Public Sub New()
            Me.oState = New roAnalyticState
        End Sub

        Public Sub New(ByVal _State As roAnalyticState)
            Me.oState = _State
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Estado de la solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <IgnoreDataMember>
        Public Property State() As roAnalyticState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAnalyticState)
                Me.oState = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(ByVal _ID As Integer, Optional ByVal bolAudit As Boolean = True) As roAnalyticSchedule

            Dim bolRet As New roAnalyticSchedule With {
                .ID = _ID
            }

            Try

                Dim strSQL As String = "@SELECT# * FROM ReportsScheduler WHERE ID = " & bolRet.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim pAdvAccessMode As New AdvancedParameter.roAdvancedParameter("AdvancedAccessMode", Nothing)
                    Dim oRow As DataRow = tb.Rows(0)

                    bolRet.Name = oRow("Name")
                    bolRet.Report = oRow("Report")
                    bolRet.Profile = oRow("Profile")
                    bolRet.ReportScheduleType = roTypes.Any2Integer(oRow("ReportType"))

                    bolRet.ProfileParameters = String.Empty
                    bolRet.ReportFormat = oRow("Format")
                    bolRet.Scheduler = roReportSchedulerScheduleManager.Load(oRow("Scheduler"))
                    If Not IsDBNull(oRow("LastDateTimeExecuted")) Then bolRet.LastDateTimeExecuted = oRow("LastDateTimeExecuted")
                    If Not IsDBNull(oRow("LastDateTimeUpdated")) Then bolRet.LastDateTimeUpdated = oRow("LastDateTimeUpdated")
                    If Not IsDBNull(oRow("NextDateTimeExecuted")) Then bolRet.NextDateTimeExecuted = oRow("NextDateTimeExecuted")
                    If Not IsDBNull(oRow("State")) Then bolRet.StateReport = oRow("State")
                    If Not IsDBNull(oRow("LastExecution")) Then bolRet.LastExecution = oRow("LastExecution")

                    If Not IsDBNull(oRow("ExecuteTask")) Then
                        bolRet.ExecuteTask = oRow("ExecuteTask")
                    Else
                        If bolRet.ExecuteTask.HasValue = False Then
                            bolRet.ExecuteTask = False
                        End If
                    End If

                    If Not IsDBNull(oRow("IDPassport")) Then bolRet.IDPassport = oRow("IDPassport")

                    If Not IsDBNull(oRow("Parameters")) Then
                        Dim oTask As New VTLiveTasks.roLiveTask
                        'bolRet.Collection = oTask.BuildFromXML(oRow("Parameters"))
                        bolRet.XmlParameters = oRow("Parameters")
                    End If

                End If

                If bolRet IsNot Nothing AndAlso bolAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ReportsSchedulerName}", bolRet.Name, String.Empty, 1)
                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tReportScheduler, bolRet.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta una nueva programación
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function GetNextID() As Integer

            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(ID) FROM ReportsScheduler "
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = roTypes.Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        Public Function Validate(ByVal oSchedule As roAnalyticSchedule, Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(ByRef oSchedule As roAnalyticSchedule, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oReportSchedulerOld As DataRow = Nothing
                Dim oReportSchedulerNew As DataRow = Nothing

                If Me.Validate(oSchedule) Then

                    Dim oOldReportScheduler As roAnalyticSchedule = Nothing

                    Dim tb As New DataTable("ReportScheduler")
                    Dim strSQL As String = "@SELECT# * FROM ReportsScheduler " &
                                           "WHERE ID = " & oSchedule.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("ID") = Me.GetNextID()
                        oSchedule.ID = oRow("ID")
                        oSchedule.ReportScheduleType = eReportScheduleType.Analytics
                    Else
                        oOldReportScheduler = Me.Load(oSchedule.ID, False)
                        oRow = tb.Rows(0)
                        oReportSchedulerOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = oSchedule.Name
                    oRow("ReportType") = roTypes.Any2Integer(oSchedule.ReportScheduleType)

                    If oSchedule.ReportScheduleType = eReportScheduleType.Analytics Then
                        oRow("Report") = String.Empty
                        oRow("Profile") = -1
                        oRow("Format") = 0
                        oRow("IDLanguage") = -1
                        oRow("Destination") = DBNull.Value
                        oRow("Parameters") = DBNull.Value
                        oRow("EmergencyReport") = False
                        oRow("EmergencyReportPriority") = False
                        If oSchedule.XmlParameters IsNot Nothing AndAlso oSchedule.XmlParameters <> String.Empty Then
                            oRow("Parameters") = oSchedule.XmlParameters
                        End If
                    Else
                        oRow("Report") = oSchedule.Report
                        oRow("Profile") = oSchedule.Profile
                        oRow("Format") = oSchedule.ReportFormat
                    End If

                    oRow("Scheduler") = roReportSchedulerScheduleManager.retScheduleString(oSchedule.Scheduler)

                    If oSchedule.ExecuteTask.HasValue Then
                        oRow("ExecuteTask") = oSchedule.ExecuteTask.Value
                    Else
                        If IsDBNull(oRow("ExecuteTask")) Then
                            oRow("ExecuteTask") = False
                        End If
                    End If

                    oRow("State") = oSchedule.StateReport
                    If Not IsNothing(oSchedule.LastExecution) Then oRow("LastExecution") = oSchedule.LastExecution

                    If oSchedule.IDUser.HasValue Then oRow("IDUser") = oSchedule.IDUser
                    If oSchedule.IDPassport.HasValue Then oRow("IDPassport") = oSchedule.IDPassport

                    'Si se ha canviado la planificación, actualizamos las fechas
                    Dim exError As Exception = Nothing
                    If oOldReportScheduler IsNot Nothing AndAlso roReportSchedulerScheduleManager.retScheduleString(oOldReportScheduler.Scheduler) <> roReportSchedulerScheduleManager.retScheduleString(oSchedule.Scheduler) Then
                        oSchedule.LastDateTimeExecuted = Nothing
                        oSchedule.NextDateTimeExecuted = roLiveSupport.GetNextRun(roReportSchedulerScheduleManager.retScheduleString(oSchedule.Scheduler), oSchedule.LastDateTimeExecuted, exError)
                        oSchedule.LastDateTimeUpdated = System.DateTime.Now
                    ElseIf oOldReportScheduler Is Nothing Then
                        oSchedule.LastDateTimeExecuted = Nothing
                        oSchedule.NextDateTimeExecuted = roLiveSupport.GetNextRun(roReportSchedulerScheduleManager.retScheduleString(oSchedule.Scheduler), oSchedule.LastDateTimeExecuted, exError)
                        oSchedule.LastDateTimeUpdated = System.DateTime.Now
                    End If

                    If exError IsNot Nothing Then
                        oState.UpdateStateInfo(exError, "roAnalyticsSchedulerManager::GetNextRun")
                        bolRet = False
                    Else
                        If oSchedule.LastDateTimeExecuted.HasValue Then
                            oRow("LastDateTimeExecuted") = oSchedule.LastDateTimeExecuted
                        Else
                            oRow("LastDateTimeExecuted") = DBNull.Value
                        End If

                        If oSchedule.LastDateTimeUpdated.HasValue Then
                            oRow("LastDateTimeUpdated") = oSchedule.LastDateTimeUpdated
                        Else
                            oRow("LastDateTimeUpdated") = DBNull.Value
                        End If

                        If oSchedule.NextDateTimeExecuted.HasValue AndAlso oSchedule.NextDateTimeExecuted.Value <> Nothing Then
                            oRow("NextDateTimeExecuted") = oSchedule.NextDateTimeExecuted
                        Else
                            oRow("NextDateTimeExecuted") = DBNull.Value
                        End If

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        oReportSchedulerNew = oRow

                        bolRet = True

                        If bolRet And bAudit Then
                            bolRet = False
                            ' Auditamos
                            Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                            Extensions.roAudit.AddFieldsValues(tbAuditParameters, oReportSchedulerNew, oReportSchedulerOld)
                            Dim oAuditAction As Audit.Action = IIf(oReportSchedulerOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                            Dim strObjectName As String
                            If oAuditAction = Audit.Action.aInsert Then
                                strObjectName = oReportSchedulerNew("Name")
                            ElseIf oReportSchedulerOld("Name") <> oReportSchedulerNew("Name") Then
                                strObjectName = oReportSchedulerOld("Name") & " -> " & oReportSchedulerNew("Name")
                            Else
                                strObjectName = oReportSchedulerNew("Name")
                            End If

                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tReportScheduler, strObjectName, tbAuditParameters, -1)
                        End If
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Save")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Borra la notificación
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(ByVal oSchedule As roAnalyticSchedule, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                'Borramos la solicitud
                Dim DelQuerys() As String = {"@DELETE# FROM ReportsScheduler WHERE ID = " & oSchedule.ID.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = AnalyticsResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = AnalyticsResultEnum.NoError)

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{ReportSchedulerName}", oSchedule.Name, String.Empty, 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tReportScheduler, oSchedule.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roAnalyticsSchedulerManager::Delete")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un Datatable con todos las programaciones
        ''' </summary>
        ''' <param name="_SQLFilter">Filtro SQL para el Where (ejemplo: 'NotificationType = 1 And Reque...')</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetReportSchedulers(ByVal _SQLFilter As String, ByRef _State As roAnalyticState,
                                Optional ByVal bAudit As Boolean = False) As Generic.List(Of roAnalyticSchedule)

            Dim oRet As New Generic.List(Of roAnalyticSchedule)

            Try

                Dim strSQL As String = "@SELECT# * from ReportsScheduler"
                If _SQLFilter <> String.Empty Then strSQL &= " Where " & _SQLFilter

                strSQL &= " Order By Name"

                Dim dTbl As DataTable = CreateDataTable(strSQL, )

                If dTbl IsNot Nothing Then
                    Dim oManager As New roAnalyticsSchedulerManager(_State)
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oReportScheduler As roAnalyticSchedule = oManager.Load(dRow("ID"), False)
                        oRet.Add(oReportScheduler)
                    Next
                End If

                If oRet IsNot Nothing AndAlso oRet.Count > 0 Then
                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = String.Empty
                        For Each oReportScheduler As roAnalyticSchedule In oRet
                            strAuditName &= IIf(strAuditName <> String.Empty, ",", String.Empty) & oReportScheduler.Name
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{ReportSchedulerNames}", strAuditName, String.Empty, 1)
                        _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tReportScheduler, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roReportScheduler::GetReportSchedulers")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roReportScheduler::GetReportSchedulers")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Lanza un informe
        ''' </summary>
        ''' <param name="ID">ID del ReportScheduler a lanzar</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ExecuteReportScheduler(ByVal ID As Integer, ByRef _State As roAnalyticState,
                                                      Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oManager As New roAnalyticsSchedulerManager(_State)
                Dim oReportScheduler As roAnalyticSchedule = oManager.Load(ID, False)
                If oReportScheduler IsNot Nothing Then
                    If oReportScheduler.ReportScheduleType <> eReportScheduleType.Analytics Then
                        ' Informe planificado
                        oReportScheduler.ExecuteTask = 1
                        bolRet = oManager.Save(oReportScheduler)
                        roConnector.InitTask(TasksType.MANUALREPORTS)
                    Else

                    End If
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = _State.CreateAuditParameters()
                    _State.AddAuditParameter(tbParameters, "{ReportSchedulerID}", ID, String.Empty, 1)
                    bolRet = _State.Audit(Audit.Action.aExecuted, Audit.ObjectType.tReportScheduler, ID, tbParameters, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roReportScheduler::ExecuteReportScheduler")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roReportScheduler::ExecuteReportScheduler")
            End Try

            Return bolRet
        End Function

        Public Shared Sub GetAnalyticPeriod(ByRef _State As roAnalyticState, ByVal oParameters As roCollection, ByRef BeginPeriod As Date, ByRef EndPeriod As Date)
            Dim dtMonth As Date

            Dim ActualDate As Date = Now.Date

            Try

                BeginPeriod = Now.Date
                EndPeriod = Now.Date

                If oParameters("TypePeriod") = TypePeriodEnum.PeriodOther Then
                    ' Libre
                    BeginPeriod = DateTime.Parse(oParameters("ScheduleBeginDate"))
                    EndPeriod = DateTime.Parse(oParameters("ScheduleEndDate"))
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodTomorrow Then
                    'Mañana
                    BeginPeriod = ActualDate.AddDays(1)
                    EndPeriod = ActualDate.AddDays(1)
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodToday Then
                    'Hoy
                    BeginPeriod = ActualDate
                    EndPeriod = ActualDate
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodYesterday Then
                    'Ayer
                    BeginPeriod = ActualDate.AddDays(-1)
                    EndPeriod = ActualDate.AddDays(-1)
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodCurrentWeek Then
                    'Semana actual
                    BeginPeriod = ActualDate.AddDays(1 - Weekday(ActualDate, vbMonday))
                    EndPeriod = ActualDate.AddDays(7 - Weekday(ActualDate, vbMonday))
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodLastWeek Then
                    'Semana pasada
                    BeginPeriod = ActualDate.AddDays(-6 - Weekday(ActualDate, vbMonday))
                    EndPeriod = ActualDate.AddDays(-Weekday(ActualDate, vbMonday))
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodCurrentMonth Then
                    'Mes actual
                    BeginPeriod = New Date(ActualDate.Year, ActualDate.Month, 1)
                    EndPeriod = New Date(ActualDate.Year, ActualDate.Month, (New Date(ActualDate.Year, ActualDate.Month, 1)).AddMonths(1).AddDays(-1).Day)
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodLastMonth Then
                    'Mes anterior
                    dtMonth = ActualDate.AddMonths(-1).Date
                    BeginPeriod = New Date(dtMonth.Year, dtMonth.Month, 1)
                    EndPeriod = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodCurrentYear Then
                    'Año actual
                    BeginPeriod = New Date(ActualDate.Year, 1, 1)
                    EndPeriod = ActualDate
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodNextWeek Then
                    ' Semana siguiente
                    BeginPeriod = ActualDate.AddDays(1 - Weekday(ActualDate, vbMonday)).AddDays(7)
                    EndPeriod = ActualDate.AddDays(1 - Weekday(ActualDate, vbMonday)).AddDays(13)
                ElseIf oParameters("TypePeriod") = TypePeriodEnum.PeriodNextMonth Then
                    ' Mes siguiente
                    dtMonth = ActualDate.AddMonths(1).Date
                    BeginPeriod = New Date(dtMonth.Year, dtMonth.Month, 1)
                    EndPeriod = New Date(dtMonth.Year, dtMonth.Month, (New Date(dtMonth.Year, dtMonth.Month, 1)).AddMonths(1).AddDays(-1).Day)
                End If
            Catch Ex As Exception
                'Stop
                _State.UpdateStateInfo(Ex, "roReportScheduler::GetAnalyticPeriod")
            Finally

            End Try

        End Sub

#End Region

    End Class

#End Region

End Namespace