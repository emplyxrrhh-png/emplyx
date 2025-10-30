Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Employee

    <DataContract>
    <Serializable>
    Public Class roTelecommuteAgreement

        Private sMandatoryDays As String
        Private sPresenceMandatoryDays As String
        Private sOptionalDays As String
        Private iMaxDays As Integer
        Private iMaxPercentage As Integer
        Private ePeriodType As TelecommutingPeriodType
        Private dAgreementStart As Date
        Private dAgreementEnd As Date

#Region "Properties"

        <DataMember>
        Public Property AgreementStart As Date
            Get
                Return dAgreementStart
            End Get
            Set(ByVal value As DateTime)
                dAgreementStart = value
            End Set
        End Property

        <DataMember>
        Public Property AgreementEnd As Date
            Get
                Return dAgreementEnd
            End Get
            Set(ByVal value As Date)
                dAgreementEnd = value
            End Set
        End Property

        <DataMember>
        Public Property MandatoryDays As String
            Get
                Return sMandatoryDays
            End Get
            Set(ByVal value As String)
                sMandatoryDays = value
            End Set
        End Property

        <DataMember>
        Public Property PresenceMandatoryDays As String
            Get
                Return sPresenceMandatoryDays
            End Get
            Set(ByVal value As String)
                sPresenceMandatoryDays = value
            End Set
        End Property

        <DataMember>
        Public Property OptionalDays As String
            Get
                Return sOptionalDays
            End Get
            Set(ByVal value As String)
                sOptionalDays = value
            End Set
        End Property

        <DataMember>
        Public Property MaxDays As Integer
            Get
                Return iMaxDays
            End Get
            Set(ByVal value As Integer)
                iMaxDays = value
            End Set
        End Property

        <DataMember>
        Public Property MaxPercentage As Integer
            Get
                Return iMaxPercentage
            End Get
            Set(ByVal value As Integer)
                iMaxPercentage = value
            End Set
        End Property

        <DataMember>
        Public ReadOnly Property MaxType As TelecommutingMaxType
            Get
                If Me.MaxPercentage > 0 Then
                    Return TelecommutingMaxType._Percentage
                Else
                    Return TelecommutingMaxType._Days
                End If
            End Get
        End Property

        <DataMember>
        Public Property PeriodType As TelecommutingPeriodType
            Get
                Return ePeriodType
            End Get
            Set(ByVal value As TelecommutingPeriodType)
                ePeriodType = value
            End Set
        End Property

#End Region

        Public Sub New()
            sMandatoryDays = String.Empty
            sOptionalDays = String.Empty
            iMaxDays = 0
            iMaxPercentage = 0
            dAgreementStart = Date.MinValue
            dAgreementEnd = New Date(2079, 1, 1)
        End Sub

        Public Function DefinitioRaw() As String
            Return sMandatoryDays & sOptionalDays & iMaxDays.ToString & iMaxPercentage.ToString & dAgreementStart.ToShortDateString & dAgreementEnd.ToShortDateString
        End Function

        Public Function PeriodRaw() As String
            Return dAgreementStart.ToShortDateString & dAgreementEnd.ToShortDateString
        End Function

    End Class

    <DataContract>
    <Serializable>
    Public Class roEmployeeTelecommuteAgreementStats

        Private dTelecommutePlannedDays As Double
        Private dTelecommutePlannedHours As Double
        Private dTotalWorkingPlannedHours As Double
        Private dHolidaysHours As Double
        Private dTelecommuteHolidaysHours As Double
        Private dCurrentPercentage As Double
        Private dPeriodStart As Date
        Private dPeriodEnd As Date
        Private _EmployeeTelecommuteAgreement As roEmployeeTelecommuteAgreement

#Region "Properties"

        <DataMember>
        Public Property TelecommutePlannedDays As Double
            Get
                Return dTelecommutePlannedDays
            End Get
            Set(ByVal value As Double)
                dTelecommutePlannedDays = value
            End Set
        End Property

        <DataMember>
        Public Property TelecommutePlannedHours As Double
            Get
                Return dTelecommutePlannedHours
            End Get
            Set(ByVal value As Double)
                dTelecommutePlannedHours = value
            End Set
        End Property

        <DataMember>
        Public Property TotalWorkingPlannedHours As Double
            Get
                Return dTotalWorkingPlannedHours
            End Get
            Set(ByVal value As Double)
                dTotalWorkingPlannedHours = value
            End Set
        End Property

        <DataMember>
        Public Property HolidaysHours As Double
            Get
                Return dHolidaysHours
            End Get
            Set(ByVal value As Double)
                dHolidaysHours = value
            End Set
        End Property

        <DataMember>
        Public Property TelecommuteHolidaysHours As Double
            Get
                Return dTelecommuteHolidaysHours
            End Get
            Set(ByVal value As Double)
                dTelecommuteHolidaysHours = value
            End Set
        End Property

        <DataMember>
        Public Property CurrentPercentage As Double
            Get
                Return dCurrentPercentage
            End Get
            Set(ByVal value As Double)
                dCurrentPercentage = value
            End Set
        End Property

        <DataMember>
        Public Property PeriodStart As Date
            Get
                Return dPeriodStart
            End Get
            Set(ByVal value As Date)
                dPeriodStart = value
            End Set
        End Property

        <DataMember>
        Public Property PeriodEnd As Date
            Get
                Return dPeriodEnd
            End Get
            Set(ByVal value As Date)
                dPeriodEnd = value
            End Set
        End Property

        <DataMember>
        Public Property EmployeeTelecommuteAgreement As roEmployeeTelecommuteAgreement
            Get
                Return _EmployeeTelecommuteAgreement
            End Get
            Set(ByVal value As roEmployeeTelecommuteAgreement)
                _EmployeeTelecommuteAgreement = value
            End Set
        End Property

#End Region

        Public Sub New()
            dTelecommutePlannedHours = 0
            dTelecommutePlannedDays = 0
            dTotalWorkingPlannedHours = 0
            dHolidaysHours = 0
            dCurrentPercentage = 0
            _EmployeeTelecommuteAgreement = Nothing
        End Sub

    End Class

    <DataContract>
    <Serializable>
    Public Class roEmployeeTelecommuteAgreement

        Private sIdContract As String
        Private iIdLabAgree As Integer
        Private eSource As DTOs.TelecommuteAgreementSource
        Private dContractStart As Date
        Private dContractEnd As Date
        Private oAgreement As roTelecommuteAgreement

#Region "Properties"

        <DataMember>
        Public Property IdLabAgree As Integer
            Get
                Return iIdLabAgree
            End Get
            Set(ByVal value As Integer)
                iIdLabAgree = value
            End Set
        End Property

        <DataMember>
        Public Property IdContract As String
            Get
                Return sIdContract
            End Get
            Set(ByVal value As String)
                sIdContract = value
            End Set
        End Property

        <DataMember>
        Public Property ContractStart As Date
            Get
                Return dContractStart
            End Get
            Set(ByVal value As DateTime)
                dContractStart = value
            End Set
        End Property

        <DataMember>
        Public Property ContractEnd As Date
            Get
                Return dContractEnd
            End Get
            Set(ByVal value As Date)
                dContractEnd = value
            End Set
        End Property

        <DataMember>
        Public Property Source As DTOs.TelecommuteAgreementSource
            Get
                Return eSource
            End Get
            Set(ByVal value As DTOs.TelecommuteAgreementSource)
                eSource = value
            End Set
        End Property

        <DataMember>
        Public Property Agreement As roTelecommuteAgreement
            Get
                Return oAgreement
            End Get
            Set(value As roTelecommuteAgreement)
                oAgreement = value
            End Set
        End Property

#End Region

        Public Sub New()
            sIdContract = String.Empty
            iIdLabAgree = -1
            eSource = TelecommuteAgreementSource.LabAgree
            dContractStart = Date.MinValue
            dContractEnd = New Date(2079, 1, 1)
            oAgreement = New roTelecommuteAgreement
        End Sub

        Public Function RecalculateTelecommutingChanges(ByVal type As DTOs.TelecommuteAgreementSource, ByVal idSource As String, ByVal oOldTelecommuteAgreement As Employee.roTelecommuteAgreement, ByVal oNewTelecommuteAgreement As Employee.roTelecommuteAgreement, oState As roEmployeeState) As Boolean
            Dim bolRet As Boolean = True
            Dim bolRecalculate As Boolean = False
            Try

                If Not oOldTelecommuteAgreement Is Nothing OrElse Not oNewTelecommuteAgreement Is Nothing Then

                    Dim dMinDate As Date = Date.MinValue
                    Dim dMaxDate As Date = Date.MinValue
                    Dim dScheduleMinDate As Date
                    Dim dScheduleMaxDate As Date

                    Dim strSQL As String = String.Empty
                    ' Recalcular sólo es necesario si hay horarios planificados con reglas de horario basadas en telertabajo
                    Dim xFreezingDate As New Date(1900, 1, 1)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing AndAlso IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                        xFreezingDate = CDate(oParameters.Parameter(Parameters.FirstDate))
                    End If

                    strSQL = $"@SELECT# COUNT(DISTINCT ds.idshift1)
                                    FROM DailySchedule ds
                                    WHERE ds.Date > {roTypes.Any2Time(xFreezingDate.Date).SQLSmallDateTime}
                                    AND EXISTS (
                                        @SELECT# 1 
                                        FROM sysroShiftsCausesRules scr
                                        WHERE scr.IDShift = ds.IDShift1
                                        AND scr.Definition LIKE '%""DayValidationRule"" type=""2"">10</Item>%'
                                    );"

                    Dim iCheck As Integer = 0
                    iCheck = VTBase.roTypes.Any2Integer(ExecuteScalar(strSQL))

                    If iCheck > 0 Then

                        If Not oOldTelecommuteAgreement Is Nothing AndAlso Not oNewTelecommuteAgreement Is Nothing Then
                            ' Tenía y tengo acuerdo
                            If oOldTelecommuteAgreement.DefinitioRaw <> oNewTelecommuteAgreement.DefinitioRaw Then

                                ' Fecha mínima y máxima
                                dMinDate = New DateTime(Math.Min(oNewTelecommuteAgreement.AgreementStart.Ticks, oOldTelecommuteAgreement.AgreementStart.Ticks)).Date
                                dMinDate = New DateTime(Math.Max(dMinDate.Ticks, xFreezingDate.AddDays(1).Ticks)).Date
                                dMaxDate = New DateTime(Math.Max(oNewTelecommuteAgreement.AgreementEnd.Ticks, oOldTelecommuteAgreement.AgreementEnd.Ticks)).Date
                                dMaxDate = New DateTime(Math.Min(dMaxDate.Ticks, Now.Date.Ticks)).Date

                                Select Case type
                                    Case TelecommuteAgreementSource.LabAgree
                                        strSQL = "@SELECT# * FROM sysrovwTelecommutingAgreement WHERE TelecommutingAgreementSource = 'LabAgree' AND LabAgreeId = " & idSource &
                                                 " AND ContractStart <= " & Any2Time(dMaxDate).SQLSmallDateTime & " AND ContractEnd >= " & Any2Time(dMinDate).SQLSmallDateTime
                                    Case TelecommuteAgreementSource.Contract
                                        strSQL = "@SELECT# * FROM sysrovwTelecommutingAgreement WHERE IdContract = '" & idSource &
                                                 "' AND ContractStart <= " & Any2Time(dMaxDate).SQLSmallDateTime & " AND ContractEnd >= " & Any2Time(dMinDate).SQLSmallDateTime
                                End Select

                                Dim tbAgreements As DataTable
                                tbAgreements = CreateDataTable(strSQL)
                                Dim idEmployee As Integer = 0
                                Dim dContractStart As Date
                                Dim dContractEnd As Date

                                If Not tbAgreements Is Nothing AndAlso tbAgreements.Rows.Count > 0 Then
                                    bolRecalculate = True
                                    For Each oRow As DataRow In tbAgreements.Rows
                                        idEmployee = Any2Integer(oRow("IDEmployee"))
                                        dContractStart = Any2DateTime(oRow("ContractStart"))
                                        dContractEnd = Any2DateTime(oRow("ContractEnd"))

                                        dScheduleMinDate = New DateTime(Math.Max(dContractStart.Ticks, dMinDate.Ticks)).Date
                                        dScheduleMaxDate = New DateTime(Math.Min(dContractEnd.Ticks, dMaxDate.Ticks)).Date

                                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 40, [GUID] = '' WHERE Status > 40 " &
                                                 " AND IdEmployee = " & idEmployee.ToString &
                                                 " AND Date BETWEEN " & Any2Time(dScheduleMinDate).SQLSmallDateTime & " AND " & Any2Time(dScheduleMaxDate).SQLSmallDateTime &
                                                 " AND IDShift1 IN (@SELECT# IdShift FROM sysroShiftsCausesRules WHERE CHARINDEX('""DayValidationRule"" type=""2"">10</Item>',sysroShiftsCausesRules.Definition) > 0)"
                                        ExecuteSql(strSQL)
                                    Next
                                End If
                            End If
                        ElseIf Not oOldTelecommuteAgreement Is Nothing Then
                            ' Tenía acuerdo y ahora no
                            dMinDate = oOldTelecommuteAgreement.AgreementStart
                            dMinDate = New DateTime(Math.Max(dMinDate.Ticks, xFreezingDate.AddDays(1).Ticks)).Date
                            dMaxDate = oOldTelecommuteAgreement.AgreementEnd
                            dMaxDate = New DateTime(Math.Min(dMaxDate.Ticks, Now.Date.Ticks)).Date

                            Select Case type
                                Case TelecommuteAgreementSource.LabAgree
                                    strSQL = "@SELECT# * FROM sysrovwTelecommutingAgreement WHERE TelecommutingAgreementSource = 'LabAgree' AND LabAgreeId = " & idSource &
                                                 " AND ContractStart <= " & Any2Time(dMaxDate).SQLSmallDateTime & " AND ContractEnd >= " & Any2Time(dMinDate).SQLSmallDateTime
                                Case TelecommuteAgreementSource.Contract
                                    strSQL = "@SELECT# * FROM sysrovwTelecommutingAgreement WHERE IdContract = '" & idSource & "'" &
                                                 " AND ContractStart <= " & Any2Time(dMaxDate).SQLSmallDateTime & " AND ContractEnd >= " & Any2Time(dMinDate).SQLSmallDateTime
                            End Select

                            Dim tbAgreements As DataTable
                            tbAgreements = CreateDataTable(strSQL)
                            Dim idEmployee As Integer = 0
                            Dim dContractStart As Date
                            Dim dContractEnd As Date
                            If Not tbAgreements Is Nothing AndAlso tbAgreements.Rows.Count > 0 Then
                                bolRecalculate = True
                                For Each oRow As DataRow In tbAgreements.Rows
                                    idEmployee = Any2Integer(oRow("IDEmployee"))
                                    dContractStart = Any2DateTime(oRow("ContractStart"))
                                    dContractEnd = Any2DateTime(oRow("ContractEnd"))

                                    dScheduleMinDate = New DateTime(Math.Max(dContractStart.Ticks, dMinDate.Ticks)).Date
                                    dScheduleMaxDate = New DateTime(Math.Min(dContractEnd.Ticks, dMaxDate.Ticks)).Date

                                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 40, [GUID] = '' WHERE Status > 40 " &
                                                 " AND IdEmployee = " & idEmployee.ToString &
                                                 " AND Date BETWEEN " & Any2Time(dScheduleMinDate).SQLSmallDateTime & " AND " & Any2Time(dScheduleMaxDate).SQLSmallDateTime &
                                                 " AND IDShift1 IN (@SELECT# IdShift FROM sysroShiftsCausesRules WHERE CHARINDEX('""DayValidationRule"" type=""2"">10</Item>',sysroShiftsCausesRules.Definition) > 0)"
                                    ExecuteSql(strSQL)
                                Next
                            End If
                        Else
                            ' No tenía acuerdo, y ahora si
                            ' 1.- Marco todos los periodos de los empleados con contrato asociado a este convenio, con fecha posterior a la de bloqueo de empleado y general y anterior a hoy, y entre fecha de inicio y fin de acuerdo eliminado
                            dMinDate = oNewTelecommuteAgreement.AgreementStart
                            dMinDate = New DateTime(Math.Max(dMinDate.Ticks, xFreezingDate.AddDays(1).Ticks)).Date
                            dMaxDate = oNewTelecommuteAgreement.AgreementEnd
                            dMaxDate = New DateTime(Math.Min(dMaxDate.Ticks, Now.Date.Ticks)).Date

                            Select Case type
                                Case TelecommuteAgreementSource.LabAgree
                                    strSQL = "@SELECT# * FROM EmployeeContracts WHERE IdLabAgree = " & idSource &
                                             " AND BeginDate <= " & Any2Time(dMaxDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(dMinDate).SQLSmallDateTime
                                Case TelecommuteAgreementSource.Contract
                                    strSQL = "@SELECT# * FROM EmployeeContracts WHERE IdContract = '" & idSource & "'" &
                                                 " AND BeginDate <= " & Any2Time(dMaxDate).SQLSmallDateTime & " AND EndDate >= " & Any2Time(dMinDate).SQLSmallDateTime
                            End Select

                            Dim tbAgreements As DataTable
                            tbAgreements = CreateDataTable(strSQL)
                            Dim idEmployee As Integer = 0
                            Dim dContractStart As Date
                            Dim dContractEnd As Date
                            If Not tbAgreements Is Nothing AndAlso tbAgreements.Rows.Count > 0 Then
                                bolRecalculate = True
                                For Each oRow As DataRow In tbAgreements.Rows
                                    idEmployee = Any2Integer(oRow("IDEmployee"))
                                    dContractStart = Any2DateTime(oRow("BeginDate"))
                                    dContractEnd = Any2DateTime(oRow("EndDate"))

                                    dScheduleMinDate = New DateTime(Math.Max(dContractStart.Ticks, dMinDate.Ticks)).Date
                                    dScheduleMaxDate = New DateTime(Math.Min(dContractEnd.Ticks, dMaxDate.Ticks)).Date

                                    strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 40, [GUID] = '' WHERE Status > 40 " &
                                                 " AND IdEmployee = " & idEmployee.ToString &
                                                 " AND Date BETWEEN " & Any2Time(dScheduleMinDate).SQLSmallDateTime & " AND " & Any2Time(dScheduleMaxDate).SQLSmallDateTime &
                                                 " AND IDShift1 IN (@SELECT# IdShift FROM sysroShiftsCausesRules WHERE CHARINDEX('""DayValidationRule"" type=""2"">10</Item>',sysroShiftsCausesRules.Definition) > 0)"
                                    ExecuteSql(strSQL)
                                Next
                            End If
                        End If
                    End If

                    If bolRecalculate Then
                        Select Case type
                            Case TelecommuteAgreementSource.Contract
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEmployeeTelecommuteAgreement::RecalculateTelecommutingChanges: Telecommute agreement changed. Scope = Contrato. Days between " & dScheduleMinDate.ToShortDateString & " and " & dScheduleMaxDate.ToShortDateString & " will be recalculated")
                            Case TelecommuteAgreementSource.LabAgree
                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roEmployeeTelecommuteAgreement::RecalculateTelecommutingChanges: Telecommute agreement changed. Scope = Convenio. Days between " & dMinDate.ToShortDateString & " and " & dMaxDate.ToShortDateString & " will be recalculated under contrat period for this labagree contracts")
                        End Select
                    End If

                End If
            Catch ex As DbException
                bolRet = False
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::RecalculateTelecommutingChanges")
            Catch ex As Exception
                bolRet = False
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::RecalculateTelecommutingChanges")
            End Try

            Return bolRet

        End Function

        Public Function GetTelecommuteAgreementOnDate(ByVal IDEmployee As String, dDate As Date, oState As roEmployeeState) As roEmployeeTelecommuteAgreement
            Dim oRet As roEmployeeTelecommuteAgreement = Nothing

            Try
                Dim strSQL As String
                strSQL = "@SELECT# TOP 1 * FROM sysrovwTelecommutingAgreement " &
                         " WHERE TelecommutingAgreementStart IS NOT NULL AND IDEmployee = " & IDEmployee.ToString &
                         " AND " & roTypes.Any2Time(dDate.Date).SQLDateTime & " BETWEEN TelecommutingAgreementStart AND TelecommutingAgreementEnd  " &
                         " AND " & roTypes.Any2Time(dDate.Date).SQLDateTime & " BETWEEN ContractStart AND ContractEnd "
                Dim tbRes As DataTable = CreateDataTable(strSQL)

                If Not tbRes Is Nothing AndAlso tbRes.Rows.Count > 0 Then
                    oRet = New roEmployeeTelecommuteAgreement
                    oRet.IdContract = IdContract
                    oRet.IdLabAgree = roTypes.Any2Integer(tbRes.Rows(0)("LabAgreeId"))
                    oRet.Source = DirectCast([Enum].Parse(GetType(DTOs.TelecommuteAgreementSource), roTypes.Any2String(tbRes.Rows(0)("TelecommutingAgreementSource"))), TelecommuteAgreementSource)
                    oRet.Agreement.AgreementStart = Any2DateTime(tbRes.Rows(0)("TelecommutingAgreementStart")).Date
                    oRet.Agreement.AgreementEnd = Any2DateTime(tbRes.Rows(0)("TelecommutingAgreementEnd")).Date
                    oRet.Agreement.MandatoryDays = Any2String(tbRes.Rows(0)("TelecommutingMandatoryDays"))
                    oRet.Agreement.PresenceMandatoryDays = Any2String(tbRes.Rows(0)("PresenceMandatoryDays"))
                    oRet.Agreement.OptionalDays = Any2String(tbRes.Rows(0)("TelecommutingOptionalDays"))
                    oRet.Agreement.MaxDays = Any2Integer(tbRes.Rows(0)("TelecommutingMaxDays"))
                    oRet.Agreement.MaxPercentage = Any2Integer(tbRes.Rows(0)("TelecommutingMaxPercentage"))
                    oRet.Agreement.PeriodType = Any2Integer(tbRes.Rows(0)("PeriodType"))
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetTelecommuteAgreementOnDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetTelecommuteAgreementOnDate")
            End Try

            Return oRet
        End Function

        Public Function GetTelecommuteAgreementStats(ByVal IDEmployee As String, dDate As Date, oState As roEmployeeState) As roEmployeeTelecommuteAgreement
            Dim oRet As roEmployeeTelecommuteAgreement = Nothing

            Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetTelecommuteAgreementStats")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetTelecommuteAgreementStats")
            End Try

            Return oRet
        End Function

        Public Function GetEmployeeContractTelecommuteAgreement(ByVal IDContract As String, oState As roEmployeeState) As roEmployeeTelecommuteAgreement
            Dim oRet As roEmployeeTelecommuteAgreement = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysrovwTelecommutingAgreement WHERE IdContract = '" & IDContract & "'"
                Dim tbRes As DataTable
                tbRes = CreateDataTable(strSQL)
                If Not tbRes Is Nothing AndAlso tbRes.Rows.Count > 0 Then
                    oRet = New roEmployeeTelecommuteAgreement
                    oRet.IdContract = IDContract
                    oRet.IdLabAgree = roTypes.Any2Integer(tbRes.Rows(0)("LabAgreeId"))
                    oRet.Source = DirectCast([Enum].Parse(GetType(DTOs.TelecommuteAgreementSource), roTypes.Any2String(tbRes.Rows(0)("TelecommutingAgreementSource"))), TelecommuteAgreementSource)
                    oRet.Agreement.AgreementStart = Any2DateTime(tbRes.Rows(0)("TelecommutingAgreementStart")).Date
                    oRet.Agreement.AgreementEnd = Any2DateTime(tbRes.Rows(0)("TelecommutingAgreementEnd")).Date
                    oRet.Agreement.MandatoryDays = Any2String(tbRes.Rows(0)("TelecommutingMandatoryDays"))
                    oRet.Agreement.PresenceMandatoryDays = Any2String(tbRes.Rows(0)("PresenceMandatoryDays"))
                    oRet.Agreement.OptionalDays = Any2String(tbRes.Rows(0)("TelecommutingOptionalDays"))
                    oRet.Agreement.MaxDays = Any2Integer(tbRes.Rows(0)("TelecommutingMaxDays"))
                    oRet.Agreement.MaxPercentage = Any2Integer(tbRes.Rows(0)("TelecommutingMaxPercentage"))
                    oRet.Agreement.PeriodType = Any2Integer(tbRes.Rows(0)("PeriodType"))
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetContractTelecommuteAgreement")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetContractTelecommuteAgreement")
            End Try

            Return oRet
        End Function

        Public Function GetLabAgreeTelecommuteAgreement(ByVal IDLabAgree As Integer, oState As roEmployeeState) As roTelecommuteAgreement
            Dim oRet As roTelecommuteAgreement = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM LabAgree WHERE Id = " & IDLabAgree.ToString
                Dim tbRes As DataTable
                tbRes = CreateDataTable(strSQL)
                If Not tbRes Is Nothing AndAlso tbRes.Rows.Count > 0 Then
                    If Any2Boolean(tbRes.Rows(0)("Telecommuting")) Then
                        oRet = New roTelecommuteAgreement
                        oRet.AgreementStart = Any2DateTime(tbRes.Rows(0)("TelecommutingAgreementStart")).Date
                        oRet.AgreementEnd = Any2DateTime(tbRes.Rows(0)("TelecommutingAgreementEnd")).Date
                        oRet.MandatoryDays = Any2String(tbRes.Rows(0)("TelecommutingMandatoryDays"))
                        oRet.PresenceMandatoryDays = Any2String(tbRes.Rows(0)("PresenceMandatoryDays"))
                        oRet.OptionalDays = Any2String(tbRes.Rows(0)("TelecommutingOptionalDays"))
                        oRet.MaxDays = Any2Integer(tbRes.Rows(0)("TelecommutingMaxDays"))
                        oRet.MaxPercentage = Any2Integer(tbRes.Rows(0)("TelecommutingMaxPercentage"))
                        oRet.PeriodType = Any2Integer(tbRes.Rows(0)("PeriodType"))
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetLabAgreeTelecommuteAgreement")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetEmployeeLabAgreeTelecommuteAgreement")
            End Try

            Return oRet
        End Function

        Public Shared Function GetTelecommuteStatsAtDate(idEmployee As Integer, dDate As Date, oETC As Employee.roEmployeeTelecommuteAgreement, oEmployeeState As roEmployeeState) As roEmployeeTelecommuteAgreementStats
            Dim oRet As New roEmployeeTelecommuteAgreementStats

            Try
                If oETC Is Nothing Then
                    Dim oEmployeeTelecommuteAgreement As Employee.roEmployeeTelecommuteAgreement = New Employee.roEmployeeTelecommuteAgreement
                    oETC = oEmployeeTelecommuteAgreement.GetTelecommuteAgreementOnDate(idEmployee, dDate, oEmployeeState)
                End If

                If oETC Is Nothing Then
                    oRet = Nothing
                Else
                    oRet.EmployeeTelecommuteAgreement = oETC

                    Dim dAgreementStart As Date = roTypes.Any2DateTime(oETC.Agreement.AgreementStart)
                    Dim dAgreementEnd As Date = roTypes.Any2DateTime(oETC.Agreement.AgreementEnd)

                    Dim iDayOfWeek As Integer
                    iDayOfWeek = Weekday(dDate, vbMonday)
                    If iDayOfWeek = 0 Then iDayOfWeek = 7

                    Select Case oETC.Agreement.PeriodType
                        Case TelecommutingPeriodType._Week
                            ' Calculamos inicio y fin de semana que incluye la fecha dada
                            oRet.PeriodStart = dDate.AddDays(-1 * (iDayOfWeek - 1))
                            oRet.PeriodEnd = oRet.PeriodStart.AddDays(6)
                        Case TelecommutingPeriodType._Month
                            ' Calculamos inicio y fin de mes que incluye la fecha dada
                            oRet.PeriodStart = New Date(dDate.Year, dDate.Month, 1)
                            oRet.PeriodEnd = oRet.PeriodStart.AddMonths(1).AddDays(-1)
                        Case TelecommutingPeriodType._Trimester
                            ' Calculamos inicio y fin de trimestre que incluye la fecha dada
                            Dim iStartMonth As Integer
                            iStartMonth = ((dDate.Month - 1) \ 3) * 3 + 1
                            oRet.PeriodStart = New Date(dDate.Year, iStartMonth, 1)
                            oRet.PeriodEnd = oRet.PeriodStart.AddMonths(3).AddDays(-1) 'New Date(dDate.Year, dPeriodStart.Month + 2, dPeriodStart.AddMonths(3).AddDays(-1).Day)
                    End Select

                    ' Calculo intersección entre fechas del acuerdo y periodo
                    Dim dStartDate As Date
                    Dim dEndDate As Date
                    dStartDate = IIf(oRet.PeriodStart.Subtract(dAgreementStart).TotalDays < 0, dAgreementStart, oRet.PeriodStart)
                    dEndDate = IIf(oRet.PeriodEnd.Subtract(dAgreementEnd).TotalDays < 0, oRet.PeriodEnd, dAgreementEnd)

                    Dim strSQL As String = String.Empty
                    ' 1.- Consulto total de horas teóricas en el periodo final, y total de teletrabajo planificado
                    strSQL = "@SELECT# " &
                             "	SUM(CASE WHEN ISNULL(TelecommutePlanned, TelecommutingExpected) = 1 THEN 1 ELSE 0 END) AS TelecommutePlannedDays," &
                             "	SUM(ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)) AS ExpectedWorkingHours," &
                             "	SUM(ISNULL(TelecommutePlanned, TelecommutingExpected) * ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours)) AS TelecommuteHours," &
                             "  SUM(CASE WHEN AllDay = 1 THEN ISNULL(DailySchedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) ELSE ISNULL(Duration, 0) END) AS HolidayHours," &
                             "  SUM(ISNULL(TelecommutePlanned, TelecommutingExpected) * CASE WHEN AllDay = 1 THEN ISNULL(Dailyschedule.ExpectedWorkingHours, Shifts.ExpectedWorkingHours) ELSE ISNULL(Duration, 0) END) AS HolidaysOnTelecommute " &
                             " FROM DailySchedule " &
                             " LEFT JOIN ProgrammedHolidays ON DailySchedule.IDEmployee =  ProgrammedHolidays.IDEmployee AND ProgrammedHolidays.Date = DailySchedule.Date " &
                             " INNER JOIN Shifts WITH (NOLOCK) ON Shifts.ID = DailySchedule.IDShift1  " &
                             " INNER JOIN EmployeeZonesBetweenDates(" & roTypes.Any2Time(dStartDate.Date).SQLDateTime & "," & roTypes.Any2Time(dEndDate.Date).SQLDateTime & ",'" & idEmployee.ToString & "') AUX ON AUX.IDEmployee = DailySchedule.IDEmployee AND DailySchedule.Date = AUX.RefDate " &
                             " WHERE AUX.IDEmployee = " & idEmployee.ToString &
                             " AND DailySchedule.DATE BETWEEN " & roTypes.Any2Time(dStartDate.Date).SQLDateTime & " AND " & roTypes.Any2Time(dEndDate.Date).SQLDateTime

                    Dim tTable As DataTable
                    tTable = CreateDataTable(strSQL)

                    If Not tTable Is Nothing AndAlso tTable.Rows.Count > 0 Then
                        oRet.TelecommutePlannedDays = roTypes.Any2Integer(tTable.Rows(0).Item("TelecommutePlannedDays"))
                        oRet.TelecommutePlannedHours = roTypes.Any2Double(tTable.Rows(0).Item("TelecommuteHours"))
                        oRet.TotalWorkingPlannedHours = roTypes.Any2Double(tTable.Rows(0).Item("ExpectedWorkingHours"))
                        oRet.HolidaysHours = roTypes.Any2Double(tTable.Rows(0).Item("HolidayHours"))
                        oRet.TelecommuteHolidaysHours = roTypes.Any2Double(tTable.Rows(0).Item("HolidaysOnTelecommute"))

                        If (oRet.TotalWorkingPlannedHours - oRet.HolidaysHours) = 0 Then
                            oRet.CurrentPercentage = 0
                        Else
                            oRet.CurrentPercentage = (oRet.TelecommutePlannedHours - oRet.TelecommuteHolidaysHours) / (oRet.TotalWorkingPlannedHours - oRet.HolidaysHours) * 100
                        End If

                    End If
                End If
            Catch ex As DbException
                oEmployeeState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetLabAgreeTelecommuteAgreement")
            Catch ex As Exception
                oEmployeeState.UpdateStateInfo(ex, "roEmployeeTelecommuteAgreement::GetEmployeeLabAgreeTelecommuteAgreement")
            End Try

            Return oRet

        End Function

    End Class

End Namespace