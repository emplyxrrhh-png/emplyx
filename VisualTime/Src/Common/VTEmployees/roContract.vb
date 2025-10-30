Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Runtime.Serialization
Imports System.Security
Imports System.Text
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Contract

    Public Class roContractDates

        Public BeginDate As DateTime
        Public EndDate As DateTime

    End Class

    Public Class roContract

#Region "Declarations - Constructor"

        Private oState As New roContractState

        Private strIDContract As String
        Private intIDEmployee As Integer
        Private datBeginDate As DateTime
        Private datEndDate As DateTime
        Private intIDCard As Nullable(Of Long)
        Private strEnterprise As String
        Private oLabAgree As LabAgree.roLabAgree
        Private boolTelecommuting As Nullable(Of Boolean)
        Private strTelecommutingMandatoryDays As String
        Private strPresenceMandatoryDays As String
        Private strTelecommutingOptionalDays As String
        Private intTelecommutingMaxDays As Nullable(Of Integer)
        Private intTelecommutingMaxPercentage As Nullable(Of Integer)
        Private intTelecommutingPeriodType As Nullable(Of Integer)
        Private dtTelecommutingAgreementStart As Nullable(Of DateTime)
        Private dtTelecommutingAgreementEnd As Nullable(Of DateTime)
        Private strEndContractReason As String
        Private strOriginalIDContract As String

        Public Sub New()
            Me.State = New roContractState
            Me.strIDContract = ""
            Me.strOriginalIDContract = "#######"
        End Sub

        Public Sub New(ByVal _State As roContractState, Optional ByVal _IDContract As String = "")
            Me.State = _State
            Me.strIDContract = _IDContract
            Me.strOriginalIDContract = "#######"
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roContractState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roContractState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property IDContract() As String
            Get
                Return strIDContract
            End Get
            Set(ByVal value As String)
                strIDContract = value
            End Set
        End Property

        <DataMember()>
        Public Property IDEmployee() As Integer
            Get
                Return intIDEmployee
            End Get
            Set(ByVal value As Integer)
                intIDEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property BeginDate() As DateTime
            Get
                Return datBeginDate
            End Get
            Set(ByVal value As DateTime)
                datBeginDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EndDate() As Date
            Get
                Return datEndDate
            End Get
            Set(ByVal value As Date)
                datEndDate = value
            End Set
        End Property

        <DataMember()>
        Public Property IDCard() As Nullable(Of Long)
            Get
                Return intIDCard
            End Get
            Set(ByVal value As Nullable(Of Long))
                intIDCard = value
            End Set
        End Property

        <DataMember()>
        Public Property Enterprise() As String
            Get
                Return strEnterprise
            End Get
            Set(ByVal value As String)
                strEnterprise = value
            End Set
        End Property

        <DataMember()>
        Public Property Telecommuting() As Nullable(Of Boolean)
            Get
                Return boolTelecommuting
            End Get
            Set(ByVal value As Nullable(Of Boolean))
                boolTelecommuting = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingMandatoryDays() As String
            Get
                Return strTelecommutingMandatoryDays
            End Get
            Set(ByVal value As String)
                strTelecommutingMandatoryDays = value
            End Set
        End Property

        <DataMember()>
        Public Property PresenceMandatoryDays() As String
            Get
                Return strPresenceMandatoryDays
            End Get
            Set(ByVal value As String)
                strPresenceMandatoryDays = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingOptionalDays() As String
            Get
                Return strTelecommutingOptionalDays
            End Get
            Set(ByVal value As String)
                strTelecommutingOptionalDays = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingMaxDays() As Nullable(Of Integer)
            Get
                Return intTelecommutingMaxDays
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intTelecommutingMaxDays = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingMaxPercentage() As Nullable(Of Integer)
            Get
                Return intTelecommutingMaxPercentage
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intTelecommutingMaxPercentage = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingPeriodType() As Nullable(Of Integer)
            Get
                Return intTelecommutingPeriodType
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intTelecommutingPeriodType = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingAgreementStart() As Nullable(Of DateTime)
            Get
                Return Me.dtTelecommutingAgreementStart
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                dtTelecommutingAgreementStart = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingAgreementEnd() As Nullable(Of DateTime)
            Get
                Return Me.dtTelecommutingAgreementEnd
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.dtTelecommutingAgreementEnd = value
            End Set
        End Property

        <DataMember()>
        Public Property LabAgree() As LabAgree.roLabAgree
            Get
                Return oLabAgree
            End Get
            Set(ByVal value As LabAgree.roLabAgree)
                oLabAgree = value
            End Set
        End Property

        <DataMember()>
        Public Property OriginalIDContract() As String
            Get
                Return Me.strOriginalIDContract
            End Get
            Set(ByVal value As String)
                Me.strOriginalIDContract = value
            End Set
        End Property

        <DataMember()>
        Public Property EndContractReason() As String
            Get
                Return strEndContractReason
            End Get
            Set(ByVal value As String)
                strEndContractReason = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bolAudit As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Try

                Me.strOriginalIDContract = Me.strIDContract

                Dim strSQL As String = "@SELECT# * FROM EmployeeContracts " &
                                       "WHERE IDContract = '" & Me.strOriginalIDContract.Replace("'", "''") & "' "

                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    bolRet = GetContractData(tb.Rows(0), bolAudit)

                    ' Auditar lectura
                    If bolAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{IDContract}", Me.IDContract, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tEmployeeContract, Me.IDContract, tbParameters, -1)
                    End If
                Else
                    oState.Result = ContractsResultEnum.ContractNotFound
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roContract::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roContract::Load")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False, Optional ByVal bCallBroadcaster As Boolean = True) As Boolean
            ' Guarda los datos de un contrato

            Dim bolRet As Boolean = False

            Dim oContractOld As DataRow = Nothing
            Dim oContractNew As DataRow = Nothing
            Dim bHaveToClose As Boolean = False

            Dim bIsNewContract As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = ContractsResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.ValidateContract() Then

                    Dim oOldContract As roContract = Nothing

                    Dim tb As New DataTable("EmployeeContracts")
                    Dim strSQL As String = "@SELECT# * FROM EmployeeContracts " &
                                           "WHERE IDEmployee = @EmployeeID AND " &
                                                 "IDContract = @ContractID "
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    AddParameter(cmd, "@EmployeeID", DbType.Int32, 0)
                    AddParameter(cmd, "@ContractID", DbType.String, 50)
                    cmd.Parameters("@EmployeeID").Value = Me.intIDEmployee.ToString
                    cmd.Parameters("@ContractID").Value = Me.strOriginalIDContract

                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        bIsNewContract = True
                    Else
                        oOldContract = New roContract(Me.oState, Me.strOriginalIDContract)
                        oOldContract.Load(False)
                        oRow = tb.Rows(0)
                        oContractOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("IDEmployee") = Me.intIDEmployee
                    oRow("IDContract") = Me.strIDContract

                    oRow("BeginDate") = Me.BeginDate
                    oRow("EndDate") = Me.EndDate
                    If Me.IDCard.HasValue Then
                        oRow("IDCard") = Me.IDCard.Value
                    Else
                        oRow("IDCard") = DBNull.Value
                    End If
                    oRow("Enterprise") = Me.Enterprise

                    If Me.EndContractReason IsNot Nothing AndAlso Me.EndContractReason.Length > 0 Then
                        oRow("EndContractReason") = Me.EndContractReason
                    Else
                        oRow("EndContractReason") = DBNull.Value
                    End If

                    If Me.Telecommuting IsNot Nothing Then
                        oRow("Telecommuting") = Me.Telecommuting
                    Else
                        oRow("Telecommuting") = 0
                    End If

                    If Me.TelecommutingMandatoryDays IsNot Nothing Then
                        oRow("TelecommutingMandatoryDays") = Me.TelecommutingMandatoryDays
                    Else
                        oRow("TelecommutingMandatoryDays") = String.Empty
                    End If

                    If Me.PresenceMandatoryDays IsNot Nothing Then
                        oRow("PresenceMandatoryDays") = Me.PresenceMandatoryDays
                    Else
                        oRow("PresenceMandatoryDays") = String.Empty
                    End If

                    If Me.TelecommutingOptionalDays IsNot Nothing Then
                        oRow("TelecommutingOptionalDays") = Me.TelecommutingOptionalDays
                    Else
                        oRow("TelecommutingOptionalDays") = String.Empty
                    End If

                    If Me.TelecommutingMaxDays.HasValue AndAlso Me.TelecommutingMaxDays IsNot Nothing Then
                        oRow("TelecommutingMaxDays") = Me.TelecommutingMaxDays
                    Else
                        oRow("TelecommutingMaxDays") = 0
                    End If

                    If Me.TelecommutingMaxPercentage.HasValue AndAlso Me.TelecommutingMaxPercentage IsNot Nothing Then
                        oRow("TelecommutingMaxPercentage") = Me.TelecommutingMaxPercentage
                    Else
                        oRow("TelecommutingMaxPercentage") = 0
                    End If
                    If Me.TelecommutingPeriodType.HasValue AndAlso Me.TelecommutingPeriodType IsNot Nothing Then
                        oRow("PeriodType") = Me.TelecommutingPeriodType
                    Else
                        oRow("PeriodType") = 0
                    End If

                    If Me.TelecommutingAgreementStart.HasValue AndAlso Me.TelecommutingAgreementStart IsNot Nothing Then
                        oRow("TelecommutingAgreementStart") = Me.TelecommutingAgreementStart
                    Else
                        oRow("TelecommutingAgreementStart") = DBNull.Value
                    End If

                    If Me.TelecommutingAgreementEnd.HasValue AndAlso Me.TelecommutingAgreementEnd IsNot Nothing Then
                        oRow("TelecommutingAgreementEnd") = Me.TelecommutingAgreementEnd
                    Else
                        oRow("TelecommutingAgreementEnd") = DBNull.Value
                    End If

                    If Me.LabAgree IsNot Nothing Then
                        oRow("IDLabAgree") = Me.LabAgree.ID
                    Else
                        oRow("IDLabAgree") = DBNull.Value
                    End If

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If

                    ' TimeStamp de empleados
                    If bIsNewContract OrElse (oOldContract.ToString <> Me.ToString) Then
                        VTBase.Extensions.roTimeStamps.UpdateEmployeeTimestamp(Me.IDEmployee)
                    End If

                    ' Validamos si hay cambios de teletrabajo para el contrato
                    Try
                        Dim oNewTelecommuteEmployeeAgreement As Employee.roTelecommuteAgreement = Nothing
                        Dim oOldTelecommuteEmployeeAgreement As Employee.roEmployeeTelecommuteAgreement = Nothing
                        Dim oTelecommuteAgreementManager As New Employee.roEmployeeTelecommuteAgreement
                        If oOldContract Is Nothing Then
                            ' 2.- Si es nuevo contrato, comparo Sin Acuerdo con el Acuerdo del nuevo (venga de Convenio o de Contrato)
                            oNewTelecommuteEmployeeAgreement = New Employee.roTelecommuteAgreement
                            If Me.Telecommuting Then
                                ' Teletrabajo sobreescrito
                                oNewTelecommuteEmployeeAgreement.AgreementStart = Me.TelecommutingAgreementStart
                                oNewTelecommuteEmployeeAgreement.AgreementEnd = Me.TelecommutingAgreementEnd
                                oNewTelecommuteEmployeeAgreement.MandatoryDays = Me.TelecommutingMandatoryDays
                                oNewTelecommuteEmployeeAgreement.PresenceMandatoryDays = Me.PresenceMandatoryDays
                                oNewTelecommuteEmployeeAgreement.OptionalDays = Me.TelecommutingOptionalDays
                                oNewTelecommuteEmployeeAgreement.MaxDays = Me.TelecommutingMaxDays
                                oNewTelecommuteEmployeeAgreement.MaxPercentage = Me.TelecommutingMaxPercentage
                                oNewTelecommuteEmployeeAgreement.PeriodType = Me.TelecommutingPeriodType
                            ElseIf Me.LabAgree IsNot Nothing AndAlso Me.LabAgree.Telecommuting Then
                                ' Teletrabajo según convenio
                                oNewTelecommuteEmployeeAgreement = oTelecommuteAgreementManager.GetLabAgreeTelecommuteAgreement(Me.LabAgree.ID, New Employee.roEmployeeState(Me.State.IDPassport))
                            End If
                        Else
                            ' 3.- Si modifico contrato existente, comparo el acuerdo antiguo (sysrovTelecommutingAgreement) con el actual (venga de Convenio o se esté sobreescribiendo)
                            ' 3.1.- Antiguo
                            oOldTelecommuteEmployeeAgreement = oTelecommuteAgreementManager.GetEmployeeContractTelecommuteAgreement(oOldContract.IDContract, New Employee.roEmployeeState(Me.State.IDPassport))

                            ' 3.2.- Nuevo. O viene definido directamente, o viene del convenio que le he asignado
                            If Me.TelecommutingAgreementStart IsNot Nothing Then
                                ' Teletrabajo sobreescrito
                                oNewTelecommuteEmployeeAgreement = New Employee.roTelecommuteAgreement
                                oNewTelecommuteEmployeeAgreement.AgreementStart = Me.TelecommutingAgreementStart
                                oNewTelecommuteEmployeeAgreement.AgreementEnd = Me.TelecommutingAgreementEnd
                                oNewTelecommuteEmployeeAgreement.MandatoryDays = Me.TelecommutingMandatoryDays
                                oNewTelecommuteEmployeeAgreement.PresenceMandatoryDays = Me.PresenceMandatoryDays
                                oNewTelecommuteEmployeeAgreement.OptionalDays = Me.TelecommutingOptionalDays
                                oNewTelecommuteEmployeeAgreement.MaxDays = Me.TelecommutingMaxDays
                                oNewTelecommuteEmployeeAgreement.MaxPercentage = Me.TelecommutingMaxPercentage
                                oNewTelecommuteEmployeeAgreement.PeriodType = Me.TelecommutingPeriodType
                            Else
                                ' Miro si hay teletrabajo según convenio del nuevo contrato
                                If Not oLabAgree Is Nothing Then oNewTelecommuteEmployeeAgreement = oTelecommuteAgreementManager.GetLabAgreeTelecommuteAgreement(Me.oLabAgree.ID, New Employee.roEmployeeState(Me.State.IDPassport))
                            End If
                        End If

                        Dim oTelecommuteEmployeeAgreement As New Employee.roEmployeeTelecommuteAgreement
                        bolRet = oTelecommuteEmployeeAgreement.RecalculateTelecommutingChanges(TelecommuteAgreementSource.Contract, Me.IDContract, If(oOldTelecommuteEmployeeAgreement Is Nothing OrElse oOldTelecommuteEmployeeAgreement.Agreement Is Nothing, Nothing, oOldTelecommuteEmployeeAgreement.Agreement), oNewTelecommuteEmployeeAgreement, New Employee.roEmployeeState(Me.State.IDPassport))
                    Catch ex As Exception
                        roLog.GetInstance().logMessage(roLog.EventType.roError, "roContract::Save: Error seting days to recalculate telecommute after contract " & Me.IDContract & " change. Some days can't be uncalculated", ex)
                    End Try

                    da.Update(tb)

                    oContractNew = oRow

                    strSQL = "@SELECT# * FROM EmployeeContracts " &
                             "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                             "BeginDate < " & Any2Time(Me.BeginDate).SQLSmallDateTime
                    Dim tbOlds As DataTable = CreateDataTable(strSQL)
                    If tbOlds.Rows.Count = 0 Then ' Es el primer contrato, modificamos la fecha de inicio del primer movimiento
                        ' Borramos todas las movilidades anteriores a la fecha de inicio del primer contrato
                        strSQL = "@DELETE# FROM EmployeeGroups " &
                                 "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                 "EndDate < " & Any2Time(Me.BeginDate).SQLSmallDateTime
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        ' Actualizamos la fecha de inicio de la primera mobilidad
                        strSQL = "@UPDATE# EmployeeGroups Set BeginDate=" & Any2Time(Me.BeginDate).SQLDateTime & " " &
                                 "WHERE IDEmployee=" & Me.IDEmployee & " AND " &
                                 "BeginDate= (@SELECT# MIN(EG.BeginDate) FROM EmployeeGroups EG " &
                                 "WHERE EG.IDEmployee=" & Me.IDEmployee & " )"
                        bolRet = (bolRet And ExecuteSqlWithoutTimeOut(strSQL))
                    Else
                        bolRet = True
                    End If

                    strSQL = "@SELECT# * FROM EmployeeContracts " &
                             "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                             "BeginDate < " & Any2Time(DateTime.Now.Date).SQLSmallDateTime & " AND EndDate > " & Any2Time(DateTime.Now.Date).SQLSmallDateTime
                    Dim tbCurrentContract As DataTable = CreateDataTable(strSQL)
                    If tbOlds.Rows.Count = 0 Then ' Si el empleado está sin contracto actualmente borramos sus alertas generadas

                        ' Reseteamos el estado del empleado para que lo muestre como fuera de la oficina
                        strSQL = "@UPDATE#  EmployeeStatus SET BeginMandatory = null WHERE IDEmployee = " & Me.IDEmployee
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                        ' Borramos sus notificaciones de empleado ausente
                        strSQL = "@DELETE# FROM sysroNotificationTasks WHERE IDNotification IN (@SELECT# ID FROM Notifications WHERE idtype IN (15,13)) AND Key1Numeric = " & Me.IDEmployee
                        bolRet = (bolRet And ExecuteSqlWithoutTimeOut(strSQL))
                    End If

                    ' Borramos datos fuera de contrato
                    If bolRet Then
                        bolRet = RemoveDaysWithoutContract(Me.IDEmployee, Me.State)
                    End If

                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)

                    If bolRet Then
                        If bAudit Then
                            bolRet = False
                            ' Auditamos
                            Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                            Extensions.roAudit.AddFieldsValues(tbAuditParameters, oContractNew, oContractOld)
                            Dim oAuditAction As Audit.Action = IIf(oContractOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                            Dim strObjectName As String
                            If oAuditAction = Audit.Action.aInsert Then
                                strObjectName = oContractNew("IDContract")
                            ElseIf oContractOld("IDContract") <> oContractNew("IDContract") Then
                                strObjectName = oContractOld("IDContract") & " -> " & oContractNew("IDContract")
                            Else
                                strObjectName = oContractNew("IDContract")
                            End If
                            Dim strEmployeeName As String = GetEmployeeName(IDEmployee, oState)
                            oState.AddAuditParameter(tbAuditParameters, "{EmployeeName}", strEmployeeName, "", 1)
                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tEmployeeContract, strObjectName & " (" & strEmployeeName & ")", tbAuditParameters, -1)
                        End If

                        bolRet = Me.Recalculate(oOldContract)

                        'Borramos notificaciones que se generaron cuando la persona no tenia contrato (13 15 19 y 35 son las de ausencia y fichajes impares)
                        strSQL = "@DELETE# sysroNotificationTasks " &
                                    " WHERE Key1Numeric = " & Me.IDEmployee &
                                    " AND IDNotification In (@SELECT# ID FROM Notifications WHERE idtype IN (13, 15, 19, 21, 35)) " &
                                    " AND id Not IN (" &
                                    "    @SELECT# id from sysroNotificationTasks nt" &
                                    "    INNER JOIN EmployeeContracts ON IDEmployee=nt.Key1Numeric " &
                                    "    WHERE Key3DateTime between BeginDate AND EndDate " &
                                    "	 AND Key1Numeric=" & Me.IDEmployee & ")"
                        ExecuteSqlWithoutTimeOut(strSQL)

                        If bCallBroadcaster Then
                            Robotics.VTBase.Extensions.roConnector.InitTask(TasksType.BROADCASTER)
                        End If
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roContract::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roContract::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean
            ' Borra el contrato
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                Dim strSQL As String

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Cargamos los datos del contrato a borrar
                If Me.Load(False) Then

                    bolRet = True
                    Dim dtContracts As DataTable = roContract.GetContractsByIDEmployee(Me.IDEmployee, Me.oState)
                    If dtContracts IsNot Nothing AndAlso dtContracts.Rows.Count = 1 Then
                        bolRet = False
                        Me.oState.Result = ContractsResultEnum.LastContractDeleteError
                    End If

                    If bolRet Then
                        ' Obtenemos el nombre del empleado relacionado para auditar
                        Dim strEmployeeName As String = GetEmployeeNameByContract(strIDContract, oState)

                        Dim strWhere As String = "IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                 "Date >= " & Any2Time(Me.BeginDate.Date).SQLSmallDateTime & " AND " &
                                                 "Date <= " & Any2Time(Me.EndDate.Date).SQLSmallDateTime
                        Dim DeleteSQLs As New Generic.List(Of String)

                        'Borramos justificaciones diarias
                        DeleteSQLs.Add("@DELETE# DailyCauses WHERE " & strWhere)

                        'Borramos planificación
                        DeleteSQLs.Add("@DELETE# DailySchedule WHERE " & strWhere)

                        'Borramos incidencias
                        DeleteSQLs.Add("@DELETE# DailyIncidences WHERE " & strWhere)

                        'Borramos los acumulados
                        DeleteSQLs.Add("@DELETE# DailyAccruals WHERE " & strWhere)

                        For Each strSQLDelete As String In DeleteSQLs
                            bolRet = ExecuteSqlWithoutTimeOut(strSQLDelete)
                            If Not bolRet Then Exit For
                        Next

                        ' Timestamp
                        Extensions.roTimeStamps.UpdateEmployeeTimestamp(Me.IDEmployee)

                        ' Validamos si hay cambios de teletrabajo para el contrato
                        Try
                            Dim oTelecommuteAgreement As New Employee.roEmployeeTelecommuteAgreement
                            Dim oTelecommuteEmployeeAgreement As Employee.roEmployeeTelecommuteAgreement = Nothing
                            ' 1.- Si tenía acuerdo (directamente o a través de convenio)
                            oTelecommuteEmployeeAgreement = oTelecommuteAgreement.GetEmployeeContractTelecommuteAgreement(Me.IDContract, New Employee.roEmployeeState(Me.State.IDPassport))
                            If Not oTelecommuteEmployeeAgreement Is Nothing Then
                                bolRet = oTelecommuteAgreement.RecalculateTelecommutingChanges(TelecommuteAgreementSource.Contract, Me.IDContract, oTelecommuteEmployeeAgreement.Agreement, Nothing, New Employee.roEmployeeState(Me.State.IDPassport))
                            End If
                        Catch ex As Exception
                        End Try

                        If bolRet Then

                            strSQL = "@DELETE# FROM EmployeeContracts " &
                                     "WHERE IDContract = @ContractID "

                            Dim cmd As DbCommand = CreateCommand(strSQL)
                            AddParameter(cmd, "@ContractID", DbType.String, 50)
                            cmd.Parameters("@ContractID").Value = strIDContract
                            cmd.ExecuteNonQuery()

                            If bolRet And bAudit Then
                                ' Auditamos
                                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                                oState.AddAuditParameter(tbParameters, "{IDContract}", strIDContract, "", 1)
                                oState.AddAuditParameter(tbParameters, "{EmployeeName}", strEmployeeName, "", 1)
                                bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployeeContract, strIDContract & " (" & strEmployeeName & ")", tbParameters, -1)
                            End If

                        End If

                        If bolRet Then
                            bolRet = Me.Recalculate(Nothing)
                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roContract::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roContract::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function ValidateContract() As Boolean
            Dim bolRet As Boolean = False
            Dim oDatatable As Data.DataTable
            Dim oDataRow As Data.DataRow
            Dim datBegindate As Date
            Dim datEndDate As Date

            Me.oState.UpdateStateInfo()

            Try

                ' Compruebo que el numero de contrato no exista
                Dim oContractTmp As New roContract(New roContractState(Me.oState.IDPassport, Me.oState.Context, Me.oState.ClientAddress), Me.strIDContract)
                If oContractTmp.Load(False) Then
                    If oContractTmp.IDEmployee <> Me.IDEmployee Then
                        Me.oState.Result = ContractsResultEnum.InvalidIDContract
                    Else
                        If Me.OriginalIDContract <> Me.IDContract Then
                            Me.oState.Result = ContractsResultEnum.InvalidIDContract
                        End If
                    End If
                End If

                If Me.IDContract.Trim() = "" Then
                    Me.oState.Result = ContractsResultEnum.InvalidIDContract
                End If

                'Si hay licencia de convenios, obligamos a insertar un convenio
                'If oState.Result = ContractsResultEnum.NoError Then
                '    If bolIsLabAgree Then
                '        If Me.LabAgree Is Nothing Then
                '            Me.oState.Result = ContractsResultEnum.LabAgreeEmpty
                '        End If
                '    End If
                'End If

                ' Recuperamos la fecha de congelación
                Dim dtFreezeDate As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, Me.oState)

                If oState.Result = ContractsResultEnum.NoError Then

                    Dim oOrigContract As roContract = Nothing

                    Dim bolValidIDCard As Boolean = True
                    ''If Me.IDCard.HasValue AndAlso Me.IDCard.Value >= 0 Then
                    ''    bolValidIDCard = ValidateCardID(Me.IDCard.Value, Me.intIDEmployee, Me.datBeginDate, Me.datEndDate, oState)
                    ''End If

                    'If bolValidIDCard Then

                    ' Verifico que la fecha inicio sea inferior o igual a la fecha fin
                    If Me.BeginDate <= Me.EndDate Then

                        Dim bolContractEdit As Boolean = False
                        If Me.OriginalIDContract <> "#######" Then
                            bolContractEdit = True
                        End If

                        If bolContractEdit Then
                            If oOrigContract Is Nothing Then
                                oOrigContract = New roContract(oState, Me.OriginalIDContract)
                                oOrigContract.Load(False)
                            End If
                            If Not oOrigContract Is Nothing Then
                                'Si se ha modificado la fecha inicio y se encontraba dentro del periodo de congelación
                                If oOrigContract.BeginDate <> Me.BeginDate And dtFreezeDate >= oOrigContract.BeginDate Or
                                    oOrigContract.BeginDate <> Me.BeginDate And dtFreezeDate >= Me.BeginDate Then
                                    oState.Result = ContractsResultEnum.ContractInFreezeDate
                                End If

                                'Si se ha modificado la fecha fin y se encontraba dentro del periodo de congelación
                                If oOrigContract.EndDate <> Me.EndDate And dtFreezeDate >= oOrigContract.EndDate Or
                                    oOrigContract.EndDate <> Me.EndDate And dtFreezeDate >= Me.EndDate Then
                                    oState.Result = ContractsResultEnum.ContractInFreezeDate
                                End If

                            End If
                        Else
                            ' Si es un contrato nuevo
                            ' Compruebo que las fechas no esten dentro del periodo de congelación
                            If dtFreezeDate >= Me.BeginDate Then
                                oState.Result = ContractsResultEnum.ContractInFreezeDate
                            End If

                            If dtFreezeDate >= Me.EndDate Then
                                oState.Result = ContractsResultEnum.ContractInFreezeDate
                            End If
                        End If

                        If oState.Result = ContractsResultEnum.NoError Then
                            ' Compruebo que las fechas del contrato no se solapan con las de otro de este empleado
                            oDatatable = GetContractsByIDEmployee(Me.IDEmployee, Me.oState)
                            If oDatatable IsNot Nothing Then

                                For Each oDataRow In oDatatable.Rows
                                    If oDataRow("IDContract") <> Me.OriginalIDContract Then
                                        datBegindate = oDataRow("BeginDate")
                                        datEndDate = oDataRow("EndDate")

                                        If Me.BeginDate >= datBegindate And Me.BeginDate <= datEndDate Then
                                            oState.Result = ContractsResultEnum.InvalidDateInterval
                                            Exit For
                                        End If

                                        If Me.EndDate >= datBegindate And Me.EndDate <= datEndDate Then
                                            oState.Result = ContractsResultEnum.InvalidDateInterval
                                            Exit For
                                        End If

                                        If Me.BeginDate <= datBegindate And Me.EndDate >= datBegindate Then
                                            oState.Result = ContractsResultEnum.InvalidDateInterval
                                            Exit For
                                        End If

                                        If Me.BeginDate <= datEndDate And Me.EndDate >= datEndDate Then
                                            oState.Result = ContractsResultEnum.InvalidDateInterval
                                            Exit For
                                        End If
                                    Else
                                        'Si es un contrato existente, comprobar si se modifican fechas en periodo de congelación
                                        If oOrigContract Is Nothing Then
                                            oOrigContract = New roContract(oState, Me.OriginalIDContract)
                                            oOrigContract.Load(False)
                                        End If
                                        If Not oOrigContract Is Nothing Then
                                            ' Compruebo que las fechas no esten dentro del periodo de congelación
                                            If dtFreezeDate >= oOrigContract.BeginDate And dtFreezeDate >= oOrigContract.EndDate Then
                                                oState.Result = ContractsResultEnum.ContractInFreezeDate
                                            End If

                                            'If dtFreezeDate >= oContract.EndDate Then
                                            '    oState.Result = ContractsResultEnum.ContractInFreezeDate
                                            'End If
                                        End If
                                    End If
                                Next
                            End If
                        End If

                        'Si tiene ausencias programadas / previstas, y quedan fuera de los contratos, avisar
                        '' Segons Xavi Iglesias (aixo no es important)
                        ''If oState.Result = ContractsResultEnum.NoError Then
                        ''    If oDatatable IsNot Nothing Then
                        ''        Dim oPAbsence As New Absence.roProgrammedAbsence()
                        ''        Dim oStatusPA As New Absence.roProgrammedAbsenceState(oState.IDPassport)
                        ''        Dim dTblAbsences As DataTable = Absence.roProgrammedAbsence.GetProgrammedAbsences(Me.IDEmployee, oStatusPA)

                        ''        If Not dTblAbsences Is Nothing Then
                        ''            'Recorremos todas las ausencias en busca de periodos incorrectos
                        ''            For Each dRowPA As DataRow In dTblAbsences.Rows
                        ''                Dim oIsPAOut As Boolean = True
                        ''                Dim dPABeginDate As Date = dRowPA("BeginDate")
                        ''                Dim dPAFinishDate As Date = Nothing

                        ''                If dRowPA("FinishDate") Is DBNull.Value Then
                        ''                    dPAFinishDate = dPABeginDate.AddDays(dRowPA("MaxLastingDays"))
                        ''                Else
                        ''                    dPAFinishDate = dRowPA("FinishDate")
                        ''                End If

                        ''                'Recorremos los contratos para saber si las ausencias se encuentran dentro de los periodos
                        ''                For Each oDataRow In oDatatable.Rows
                        ''                    If oDataRow("IDContract") <> Me.OriginalIDContract Then
                        ''                        If dPABeginDate >= oDataRow("BeginDate") And dPABeginDate <= oDataRow("EndDate") Then
                        ''                            oIsPAOut = False
                        ''                            Exit For
                        ''                        End If

                        ''                        If dPAFinishDate >= oDataRow("Begindate") And dPAFinishDate <= oDataRow("EndDate") Then
                        ''                            oIsPAOut = False
                        ''                            Exit For
                        ''                        End If
                        ''                    Else
                        ''                        'Si es el contrato que estamos modificando, comprobamos con los valores actuales
                        ''                        If dPABeginDate >= Me.BeginDate And dPABeginDate <= Me.EndDate Then
                        ''                            oIsPAOut = False
                        ''                            Exit For
                        ''                        End If

                        ''                        If dPAFinishDate >= Me.BeginDate And dPAFinishDate <= Me.EndDate Then
                        ''                            oIsPAOut = False
                        ''                            Exit For
                        ''                        End If
                        ''                    End If

                        ''                Next

                        ''                If oIsPAOut = True Then
                        ''                    oState.Result = ContractsResultEnum.ProgrammedAbsencesOutOfContracts
                        ''                    Exit For
                        ''                End If
                        ''            Next
                        ''        End If
                        ''    End If
                        ''End If

                        If oState.Result = ContractsResultEnum.NoError Then
                            ' Si se esta modificando un contrato que no esta finalizado se deben comprobar los medios de identificación.
                            If Me.EndDate > Now.Date Then
                                ' Verifico que con la nueva definición del contrato los métodos de autentificación del pasaporte relacionado sean correctos
                                Dim oPassportManager As New roPassportManager(Me.State.IDPassport)
                                Dim oPassport As roPassport = oPassportManager.LoadPassport(Me.IDEmployee, LoadType.Employee)
                                If oPassport IsNot Nothing AndAlso Not oPassportManager.IsValidAuthenticationMethods(oPassport, True) Then
                                    oState.Result = ContractsResultEnum.InvalidAuthenticationMethods
                                    oState.ErrorText &= vbCrLf & oPassportManager.State.ErrorText
                                End If
                            End If
                        End If
                    Else
                        oState.Result = ContractsResultEnum.InvalidDates
                    End If

                End If

                bolRet = True
            Catch ex As Exception
                bolRet = False
                oState.UpdateStateInfo(ex, "roContract::ValidateContract")
            End Try

            Return (oState.Result = ContractsResultEnum.NoError)
        End Function

        Private Function GetContractData(ByVal oRow As DataRow, Optional ByVal bolAudit As Boolean = True) As Boolean
            ' Pasado un datareader se vuelcan los datos del registro actual en una
            'clase(wscContract)
            Dim bolRet As Boolean = False

            Try
                Me.IDContract = oRow("IDContract")
                Me.OriginalIDContract = Me.IDContract
                Me.IDEmployee = oRow("IDEmployee")
                Me.BeginDate = oRow("BeginDate")
                Me.EndDate = oRow("EndDate")
                If Not IsDBNull(oRow("IDCard")) Then
                    Me.IDCard = CLng(oRow("IDCard"))
                Else
                    Me.IDCard = 0
                End If

                If Not IsDBNull(oRow("Enterprise")) Then
                    Me.Enterprise = oRow("Enterprise")
                Else
                    Me.Enterprise = ""
                End If

                If Not IsDBNull(oRow("Telecommuting")) Then
                    Me.Telecommuting = Any2Boolean(oRow("Telecommuting"))
                Else
                    Me.Telecommuting = Nothing
                End If
                Me.TelecommutingMandatoryDays = Any2String(oRow("TelecommutingMandatoryDays"))
                Me.PresenceMandatoryDays = Any2String(oRow("PresenceMandatoryDays"))
                Me.TelecommutingOptionalDays = Any2String(oRow("TelecommutingOptionalDays"))
                Me.TelecommutingMaxDays = Any2Integer(oRow("TelecommutingMaxDays"))
                Me.TelecommutingMaxPercentage = Any2Integer(oRow("TelecommutingMaxPercentage"))
                Me.TelecommutingPeriodType = Any2Integer(oRow("PeriodType"))

                If Not IsDBNull(oRow("TelecommutingAgreementStart")) Then
                    Me.TelecommutingAgreementStart = Any2DateTime(oRow("TelecommutingAgreementStart"))
                Else
                    Me.TelecommutingAgreementStart = Nothing
                End If
                If Not IsDBNull(oRow("TelecommutingAgreementEnd")) Then
                    Me.TelecommutingAgreementEnd = Any2DateTime(oRow("TelecommutingAgreementEnd"))
                Else
                    Me.TelecommutingAgreementEnd = Nothing
                End If

                If Not IsDBNull(oRow("EndContractReason")) Then
                    Me.EndContractReason = oRow("EndContractReason")
                Else
                    Me.EndContractReason = ""
                End If

                oLabAgree = Nothing
                If Not IsDBNull(oRow("IDLabAgree")) Then
                    Dim oStateLA As New LabAgree.roLabAgreeState
                    roBusinessState.CopyTo(oState, oStateLA)
                    oLabAgree = New LabAgree.roLabAgree(oRow("IDLabAgree"), oStateLA)
                End If

                If bolAudit Then
                    ' Auditar lectura
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{IDContract}", oRow("IDContract"), "", 1)
                    Dim strEmployeeName As String = GetEmployeeName(IDEmployee, oState)
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", strEmployeeName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tEmployeeContract, oRow("IDContract") & " (" & strEmployeeName & ")", tbParameters, -1)
                End If

                bolRet = True
            Catch ex As Exception
                bolRet = False
                oState.UpdateStateInfo(ex, "roContract::GetContractData")
            End Try

            Return bolRet
        End Function

        Public Function Recalculate(ByVal oOldContract As roContract) As Boolean

            Dim bolRet As Boolean = True

            Try
                Dim bolMustRecalc As Boolean = True
                Dim bolLabAgreeChanged As Boolean = True
                If oOldContract IsNot Nothing Then
                    ' Estamos modificando un contrato existente
                    bolMustRecalc = (oOldContract.BeginDate <> Me.BeginDate OrElse oOldContract.EndDate <> Me.EndDate)
                    Dim intIDOldLabAgree As Integer = 0
                    Dim intIDLabAgree As Integer = 0
                    If Not bolMustRecalc Then
                        If oOldContract.LabAgree IsNot Nothing Then intIDOldLabAgree = oOldContract.LabAgree.ID
                        If Me.LabAgree IsNot Nothing Then intIDLabAgree = Me.LabAgree.ID
                        bolMustRecalc = (intIDOldLabAgree <> intIDLabAgree)
                        bolLabAgreeChanged = (intIDOldLabAgree <> intIDLabAgree)
                    End If
                Else
                    ' Estamos creando un contrato nuevo, o bien borrando un contrato
                End If

                If bolMustRecalc Then
                    If bolLabAgreeChanged Then
                        If oOldContract IsNot Nothing AndAlso oOldContract.LabAgree IsNot Nothing Then
                            bolRet = oOldContract.LabAgree.Recalculate(oOldContract.LabAgree, Me.IDEmployee, Me.BeginDate)
                        End If
                    End If

                    If bolRet AndAlso Me.LabAgree IsNot Nothing Then
                        ' Volvemos a cargar la definición del convenio para obtener todos los datos
                        Me.LabAgree = New LabAgree.roLabAgree(Me.LabAgree.ID, New VTEmployees.LabAgree.roLabAgreeState(Me.State.IDPassport))
                        bolRet = Me.LabAgree.Recalculate(Nothing, Me.IDEmployee, Me.BeginDate)
                    End If

                    If bolRet Then
                        If (Me.LabAgree IsNot Nothing AndAlso Me.LabAgree.StartupValues IsNot Nothing AndAlso Me.LabAgree.StartupValues.Count = 0) OrElse (Me.LabAgree Is Nothing) Then
                            ' en el caso que el convenio no tenga valores iniciales o que el contrato no tenga convenio
                            ' forzamos el recalculo del primer dia de contrato

                            ' Obtenemos la fecha de congelación
                            Dim xFreezingDate = roParameters.GetFirstDate()
                            xFreezingDate = xFreezingDate.AddDays(1)

                            Dim xFirstDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(Me.IDEmployee, False, Me.oState)
                            xFirstDateEmployee = xFirstDateEmployee.AddDays(1)

                            ' Obtenemos la fecha para actualizar el status=65 a DailySchedule
                            Dim xStatusDate As Date = xFreezingDate
                            If xStatusDate < xFirstDateEmployee Then xStatusDate = xFirstDateEmployee

                            'Hay que marcar para recalcular el primer dia de contrato
                            Dim strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                                    "WHERE Status >= 65 AND IDEmployee = " & Me.IDEmployee.ToString & " AND " &
                                                        "Date = " & Any2Time(Me.BeginDate).SQLSmallDateTime & " AND " &
                                                        "Date >= " & Any2Time(xStatusDate).SQLSmallDateTime

                            bolRet = ExecuteSql(strSQL)


                        End If
                    End If
                End If

                If bolRet And bolMustRecalc Then
                    Extensions.roConnector.InitTask(TasksType.DAILYSCHEDULE)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roContract::Recalculate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roContract::Recalculate")
                bolRet = False
            End Try

            Return bolRet

        End Function



        Public Overrides Function ToString() As String
            Dim sb As New StringBuilder()

            sb.AppendLine("IDContract:  " & IDContract)
            sb.AppendLine("IDEmployee: " & IDEmployee)
            sb.AppendLine("BeginDate: " & BeginDate.ToString())
            sb.AppendLine("EndDate: " & EndDate.ToString())
            sb.AppendLine("IDCard: " & If(IDCard.HasValue, IDCard.ToString(), "Null"))
            sb.AppendLine("Enterprise: " & Enterprise)
            sb.AppendLine("Telecommuting: " & If(Telecommuting.HasValue, Telecommuting.ToString(), "Null"))
            sb.AppendLine("TelecommutingMandatoryDays: " & TelecommutingMandatoryDays)
            sb.AppendLine("PresenceMandatoryDays: " & PresenceMandatoryDays)
            sb.AppendLine("TelecommutingOptionalDays: " & TelecommutingOptionalDays)
            sb.AppendLine("TelecommutingMaxDays: " & If(TelecommutingMaxDays.HasValue, TelecommutingMaxDays.ToString(), "Null"))
            sb.AppendLine("TelecommutingMaxPercentage: " & If(TelecommutingMaxPercentage.HasValue, TelecommutingMaxPercentage.ToString(), "Null"))
            sb.AppendLine("TelecommutingPeriodType: " & If(TelecommutingPeriodType.HasValue, TelecommutingPeriodType.ToString(), "Null"))
            sb.AppendLine("TelecommutingAgreementStart: " & If(TelecommutingAgreementStart.HasValue, TelecommutingAgreementStart.ToString(), "Null"))
            sb.AppendLine("TelecommutingAgreementEnd: " & If(TelecommutingAgreementEnd.HasValue, TelecommutingAgreementEnd.ToString(), "Null"))
            sb.AppendLine("LabAgree: " & If(LabAgree IsNot Nothing, LabAgree.ID.ToString(), "Null"))
            sb.AppendLine("EndContractReason: " & EndContractReason)

            Return sb.ToString()
        End Function

#End Region

#Region "Helper methods"

        Public Shared Function GetContractInDate(ByVal IDEmployee As Integer, ByVal InDate As DateTime, ByRef oState As roContractState, Optional ByVal bolAudit As Boolean = True) As roContract
            Dim oContract As roContract = Nothing

            Dim bolRet As Boolean = False
            Try
                Dim strSQL As String = " @SELECT# IDContract From EmployeeContracts " &
                                       " Where BeginDate <= " & Any2Time(InDate.Date).SQLSmallDateTime & " " &
                                       " And EndDate >= " & Any2Time(InDate.Date).SQLSmallDateTime & " " &
                                       " And IDEmployee = '" & IDEmployee & "' "

                Dim idContract As String = roTypes.Any2String(ExecuteScalar(strSQL))

                If idContract <> String.Empty Then
                    oContract = New roContract(oState, idContract)
                    If Not oContract.Load(bolAudit) Then
                        oState.Result = ContractsResultEnum.ContractNotFound
                    End If
                Else
                    oState.Result = ContractsResultEnum.ContractNotFound
                End If

                bolRet = True
            Catch ex As Exception
                oContract = Nothing
                oState.UpdateStateInfo(ex, "roContract::GetContractData")
            End Try

            Return oContract
        End Function

        Public Shared Function GetContractInDateLite(ByVal IDEmployee As Integer, ByVal InDate As DateTime, ByRef oState As roContractState) As roContract
            Dim oContract As roContract = Nothing

            Try

                Dim strSQL As String = " @SELECT# * From EmployeeContracts with (nolock) " &
                                       " Where BeginDate <= " & Any2Time(InDate.Date).SQLSmallDateTime & " " &
                                       " And EndDate >= " & Any2Time(InDate.Date).SQLSmallDateTime & " " &
                                       " And IDEmployee = '" & IDEmployee & "' "

                Dim tb As DataTable
                tb = CreateDataTable(strSQL)
                If Not tb Is Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)
                    oContract = New roContract
                    oContract.IDContract = Any2String(oRow("IDContract"))
                    Dim oLabAgree As LabAgree.roLabAgree = Nothing
                    If Any2Integer(oRow("IDLabAgree")) > 0 Then
                        oLabAgree = New LabAgree.roLabAgree
                        oLabAgree.ID = Any2Integer(oRow("IDLabAgree"))
                    End If
                    oContract.LabAgree = oLabAgree
                    oContract.BeginDate = oRow("BeginDate")
                    oContract.EndDate = oRow("EndDate")
                Else
                    oState.Result = ContractsResultEnum.ContractNotFound
                End If
            Catch ex As Exception
                oContract = Nothing
                oState.UpdateStateInfo(ex, "roContract::GetContractInDateLite")
            Finally

            End Try

            Return oContract
        End Function

        Public Shared Function GetActiveContract(ByVal IDEmployee As Integer, ByRef oState As roContractState, Optional ByVal bolAudit As Boolean = True) As roContract
            Dim oContract As roContract = Nothing
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = " @SELECT# top 1 IDContract From EmployeeContracts " &
                                       " Where BeginDate <= " & Any2Time(Now.Date).SQLSmallDateTime & " " &
                                       " And EndDate >= " & Any2Time(Now.Date).SQLSmallDateTime & " " &
                                       " And IDEmployee = '" & IDEmployee & "' "

                Dim idContract As String = roTypes.Any2String(ExecuteScalar(strSQL))

                If idContract <> String.Empty Then
                    oContract = New roContract(oState, idContract)
                    If Not oContract.Load(bolAudit) Then
                        oState.Result = ContractsResultEnum.ContractNotFound
                    End If
                Else
                    strSQL = " @SELECT# top 1 IDContract From EmployeeContracts " &
                             " Where BeginDate >= " & Any2Time(Now.Date).SQLSmallDateTime & " " &
                             " And IDEmployee = '" & IDEmployee & "' ORDER BY BeginDate ASC"

                    idContract = roTypes.Any2String(ExecuteScalar(strSQL))

                    If idContract <> String.Empty Then
                        oContract = New roContract(oState, idContract)
                        If Not oContract.Load(bolAudit) Then
                            oState.Result = ContractsResultEnum.ContractNotFound
                        End If
                    Else
                        oState.Result = ContractsResultEnum.ContractNotFound
                    End If

                End If
                bolRet = True
            Catch ex As Exception
                oContract = Nothing
                oState.UpdateStateInfo(ex, "roContract::GetActiveContract")
            End Try

            Return oContract

        End Function

        Public Shared Function GetLastContract(ByVal IDEmployee As Integer, ByRef oState As roContractState, Optional ByVal bolAudit As Boolean = True) As roContract
            Dim oContract As roContract = Nothing
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = " @SELECT# top 1 IDContract From EmployeeContracts " &
                                       " Where IDEmployee = " & IDEmployee.ToString & " ORDER BY EndDate DESC"

                Dim idContract As String = roTypes.Any2String(ExecuteScalar(strSQL))

                If idContract <> String.Empty Then
                    oContract = New roContract(oState, idContract)
                    If Not oContract.Load(bolAudit) Then
                        oState.Result = ContractsResultEnum.ContractNotFound
                    End If
                Else
                    oState.Result = ContractsResultEnum.ContractNotFound
                End If

                bolRet = True
            Catch ex As Exception
                oContract = Nothing
                oState.UpdateStateInfo(ex, "roContract::GetLastContract")
            End Try

            Return oContract

        End Function

        Public Shared Function GetContractsByIDEmployee(ByVal IDEmployee As Integer, ByVal _State As roContractState,
                                                        Optional ByVal bAudit As Boolean = False) As System.Data.DataTable
            ' Recupera la lista de contratos para el empleado pasado por parámetro
            Dim oRet As DataTable = Nothing

            Try

                'TODO: Fa falta controlar la llicencia aqui (tema columna bbdd antigues, etc.)
                Dim strSQL As String = " @SELECT# EmployeeContracts.*, LabAgree.Name as LabAgreeName From EmployeeContracts LEFT JOIN LabAgree " &
                                        " ON (EmployeeContracts.IDLabAgree = LabAgree.ID) Where EmployeeContracts.IDEmployee = " & IDEmployee &
                                        " Order by EmployeeContracts.BeginDate"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    If bAudit Then
                        ' Auditamos consulta masiva
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = ""
                        For Each oRow As DataRow In oRet.Rows
                            strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("IDContract")
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{IDContracts}", strAuditName, "", 1)
                        Dim strEmployeeName As String = GetEmployeeName(IDEmployee, _State)
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{EmployeeName}", strEmployeeName, "", 1)
                        _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tEmployeeContract, strAuditName & " (" & strEmployeeName & ")", tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roContract::GetContractsByIDEmployee")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::GetContractsByIDEmployee")
            End Try

            Return oRet

        End Function

        Public Shared Function UpdateContractID(ByVal OldIDContract As String, ByVal NewIDContract As String, ByVal _State As roContractState) As Boolean
            ' Modifica el número de contrato de un empleado
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = " @UPDATE# EmployeeContracts Set IDContract = '" & NewIDContract & "' Where IDContract = '" & OldIDContract & "' "

                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roContract::UpdateContractID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::UpdateContractID")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExistsCardID(ByVal strCardID As String, ByVal xDay As DateTime,
                                            ByVal _State As roContractState) As Boolean
            '
            'Devuelve TRUE si el ID de tarjeta existe en la base de datos.
            'Si el ID no existe devuelve FALSE
            '
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String

                'Mira las targetas de empleado
                strSQL = "@SELECT# IDCard FROM EmployeeContracts WHERE IDCard = " & strCardID
                strSQL &= " AND " & Any2Time(xDay).SQLDateTime & " >= BeginDate And EndDate >= " & Any2Time(xDay).SQLDateTime

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    If tb.Rows.Count = 0 Then

                        'Mira las targetas de equipos, por si tuviera producción
                        strSQL = "@SELECT# IDCard From Teams Where IDCard =" & strCardID
                        tb = CreateDataTable(strSQL, )
                        If tb.Rows.Count = 0 Then

                            'Mira las targetas de incidencias de produccion
                            strSQL = "@SELECT# ReaderInputCode From JobIncidences Where ReaderInputCode =" & strCardID
                            tb = CreateDataTable(strSQL, )
                            If tb.Rows.Count = 0 Then
                                bolRet = False
                            Else
                                bolRet = True
                            End If
                        Else
                            bolRet = True
                        End If
                    Else
                        bolRet = True
                    End If

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roContract::ExistsCardID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::ExistsCardID")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function HasPunchesAfterEndContractDate(ByVal idEmployee As String, idContract As String, finalDate As Date,
                                               ByVal _State As roContractState) As Boolean
            '
            'Devuelve TRUE si existe algun punch posterior a la fecha fin del contrato
            'Si no existe ningún punch devuelve FALSE
            '
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String

                'Mira en los contratos de los empleados
                Dim tb As New DataTable("PunchesContract")
                strSQL = "@SELECT# ec.IDEmployee " &
                         "FROM punches p " &
                         "INNER JOIN EmployeeContracts ec ON p.IDEmployee = ec.IDEmployee " &
                         "WHERE ec.IDEmployee={0} AND ec.IDContract='{1}' AND p.ShiftDate > {2} " &
                         "GROUP BY ec.IDEmployee"

                strSQL = String.Format(strSQL, idEmployee, idContract, Any2Time(finalDate).SQLSmallDateTime)

                Dim dt As DataTable = CreateDataTable(strSQL)

                If dt IsNot Nothing Then
                    bolRet = dt.Rows.Count > 0
                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roContract::HasPunchesAfterEndContractDate")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::HasPunchesAfterEndContractDate")
            End Try

            Return bolRet

        End Function

        Public Shared Function HasPunchesOutOfContratToBeModified(ByVal idEmployee As String, dBeginOldContractDate As Date, dEndOldContractDate As Date, dBeginNewContractDate As Date, dEndNewContractDate As Date, ByVal _State As roContractState) As Boolean
            '
            ' Devuelve TRUE si hay algún fichaje que tras la modificación de fechas de contrato queda fuera de contrato
            '
            Dim somePunchesWillBeLost As Boolean = False

            Try
                Dim periodsToCheck As List(Of roDateTimePeriod) = roBusinessSupport.GetPeriodsOutsideModifiedPeriod(dBeginOldContractDate, dEndOldContractDate, dBeginNewContractDate, dEndNewContractDate)

                Dim sql As String
                Dim continueChecking As Boolean = periodsToCheck.Any
                Dim currentPeriodIndex As Integer = 0
                Dim period As roDateTimePeriod
                Dim totalProtectedPunches As Integer

                While continueChecking
                    period = periodsToCheck(currentPeriodIndex)

                    sql = $"@SELECT# COUNT(*) FROM Punches WITH (NOLOCK) WHERE IdEmployee = @idemployee AND ShiftDate BETWEEN @startPeriodToCheck AND @endPeriodToCheck"
                    Dim parameters As New List(Of CommandParameter)
                    ' Estamos cortando días del inicio o del final o del imicio y del final del contrato original (dBeginNewContractDate > dBeginOldContractDate OrElse dEndNewContractDate < dEndOldContractDate)
                    Dim startPeriodToCheck As Date = period.BeginDateTimePeriod.Date
                    Dim endPeriodToCheck As Date = period.EndDateTimePeriod.Date

                    parameters.Add(New CommandParameter("@idEmployee", CommandParameter.ParameterType.tInt, idEmployee))
                    parameters.Add(New CommandParameter("startPeriodToCheck", CommandParameter.ParameterType.tDateTime, startPeriodToCheck))
                    parameters.Add(New CommandParameter("endPeriodToCheck", CommandParameter.ParameterType.tDateTime, endPeriodToCheck))

                    totalProtectedPunches = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sql, parameters))

                    somePunchesWillBeLost = (totalProtectedPunches > 0)

                    currentPeriodIndex += 1

                    continueChecking = Not somePunchesWillBeLost AndAlso currentPeriodIndex < periodsToCheck.Count
                End While


            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roContract::HasPunchesOutOfContratToBeModified")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::HasPunchesOutOfContratToBeModified")
            End Try

            Return somePunchesWillBeLost

        End Function

        Public Shared Function ExistsContractID(ByVal strContractID As String,
                                                ByVal _State As roContractState) As Boolean
            '
            'Devuelve TRUE si el ID de contratp existe en la base de datos.
            'Si el ID no existe devuelve FALSE
            '
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String

                'Mira en los contratos de los empleados
                Dim tb As New DataTable("EmployeeContracts")
                strSQL = "@SELECT# * FROM EmployeeContracts WHERE IDContract = @ContractID"

                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@ContractID", DbType.String, 16)
                cmd.Parameters("@ContractID").Value = strContractID.Trim

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb IsNot Nothing Then

                    bolRet = (tb.Rows.Count > 0)

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roContract::ExistsContractID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::ExistsContractID")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateCardID(ByVal strCardID As String, ByVal intIDEmployee As Integer, ByVal xBeginContract As Date, ByVal xEndContract As Date,
                                            ByVal _State As roContractState) As Boolean
            '
            'Devuelve TRUE si el ID de tarjeta no se está utilizando.
            '
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String

                'Mira las targetas de empleado
                strSQL = "@SELECT# IDCard FROM EmployeeContracts " &
                         "WHERE IDCard = " & strCardID & " AND " &
                                "EndDate >= " & Any2Time(xBeginContract).SQLDateTime & " And BeginDate <= " & Any2Time(xEndContract).SQLDateTime & " AND " &
                                "IDEmployee <> " & intIDEmployee.ToString

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    If tb.Rows.Count = 0 Then

                        'Mira las targetas de equipos, por si tuviera producción
                        strSQL = "@SELECT# IDCard From Teams Where IDCard =" & strCardID
                        tb = CreateDataTable(strSQL, )
                        If tb.Rows.Count = 0 Then

                            'Miramos que la tarjeta no la tenga asigana una justificacion
                            strSQL = "@SELECT# ReaderInputCode FROM Causes WHERE ReaderInputCode = " & strCardID & " AND AllowInputFromReader=1"
                            tb = CreateDataTable(strSQL, )
                            If tb.Rows.Count = 0 Then

                                'Mira las targetas de incidencias de produccion
                                strSQL = "@SELECT# ReaderInputCode From JobIncidences Where ReaderInputCode =" & strCardID
                                tb = CreateDataTable(strSQL, )
                                If tb.Rows.Count = 0 Then
                                    bolRet = True
                                End If

                            End If

                        End If

                    End If

                    If Not bolRet Then _State.Result = ContractsResultEnum.InvalidCardID
                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roContract::ValidateCardID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::ValidateCardID")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function GetMaxIDCard(ByVal _State As roContractState) As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# MAX(IDCard) FROM EmployeeContracts"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then intRet = tb.Rows(0).Item(0)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roContract::GetMaxIDCard")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::GetMaxIDCard")
            Finally

            End Try

            Return intRet

        End Function

        Private Shared Function GetEmployeeName(ByVal _IDEmployee As Integer, ByVal _State As roContractState) As String
            Return roBusinessSupport.GetEmployeeName(_IDEmployee, _State)
        End Function

        Private Shared Function GetEmployeeNameByContract(ByVal _IDContract As String, ByVal _State As roContractState) As String

            Dim strRet As String = ""

            ' Obtenemos nombre del empleado actual
            Dim oEmployeeState As New Employee.roEmployeeState(_State.IDPassport, _State.Context)
            Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployeeByContract(_IDContract, oEmployeeState)
            If oEmployee IsNot Nothing Then
                strRet = oEmployee.Name
            End If

            Return strRet
        End Function

        Public Shared Function RemoveDaysWithoutContract(ByVal IDEmployee As Integer, ByVal _State As roContractState) As Boolean
            '
            ' Borra todos los registros que hay en DailySchedule, que no esten dentro
            '  de ningún contrato del empleado
            '
            Dim bolRet As Boolean = False

            Try

                If roBusinessSupport.GetCustomizationCode() = "taif" Then
                    bolRet = True
                    Return True
                End If

                Dim strSQL As String

                ' Obtenemos los contratos del empleado ordenados por fecha de inicio
                Dim tbContracts As DataTable = GetContractsByIDEmployee(IDEmployee, _State)
                If tbContracts.Rows.Count > 0 Then

                    Dim TablesDelete As String() = {"DailyAccruals", "DailyCauses", "DailyIncidences", "DailySchedule"}
                    Dim xPreviousDate As Date
                    Dim xEndDate As New Date(2079, 1, 1)

                    xPreviousDate = tbContracts.Rows(0).Item("BeginDate")

                    For Each oContract As DataRow In tbContracts.Rows

                        ' Elimina las fechas fuera del intervalo
                        For Each strTable As String In TablesDelete
                            strSQL = "@DELETE# FROM " & strTable & " WHERE IDEmployee=" & IDEmployee &
                                     " AND Date>" & Any2Time(xPreviousDate).SQLSmallDateTime &
                                     " AND Date<" & Any2Time(oContract("BeginDate")).SQLSmallDateTime
                            ExecuteSqlWithoutTimeOut(strSQL)

                            'Ya no existe el módulo de hrscheduling v1, quitamos el recalculate de coberturas
                            'RecalculateDailyCoverages(IDEmployee, Any2Time(xPreviousDate).Add(1, "d").Value, Any2Time(oContract("BeginDate")).Add(-1, "d").Value, _State)
                        Next

                        xPreviousDate = oContract("EndDate")

                    Next

                    If xPreviousDate >= Now.Date Then
                        ' En el caso que el ultimo contrato tenga fecha de fin futura, no eliminamos los datos futuros
                        xEndDate = Now.Date.AddDays(-1)
                    End If

                    'Eliminamos los datos a partir del último registro (Elimina las fechas fuera de contrato)
                    For Each strTable As String In TablesDelete
                        strSQL = "@DELETE# FROM " & strTable & " WHERE IDEmployee=" & IDEmployee &
                                 " AND Date>" & Any2Time(xPreviousDate).SQLSmallDateTime &
                                 " AND Date<" & Any2Time(xEndDate).SQLSmallDateTime
                        ExecuteSqlWithoutTimeOut(strSQL)

                        'Ya no existe el módulo de hrscheduling v1, quitamos el recalculate de coberturas
                        'RecalculateDailyCoverages(IDEmployee, Any2Time(xPreviousDate).Add(1, "d").Value, xEndDate, _State)
                    Next

                    'PPR 28/11/2012
                    '====================================================
                    'ELIMINAR DATOS DESDE EL INICIO DE LOS TIEMPOS HASTA UN DIA ANTES DEL INICIO DEL PRIMER CONTRATO
                    xPreviousDate = New DateTime(1990, 1, 1)
                    xEndDate = tbContracts.Rows(0).Item("BeginDate")
                    For Each strTable As String In TablesDelete
                        strSQL = "@DELETE# FROM " & strTable & " WHERE IDEmployee=" & IDEmployee & " AND Date < " & Any2Time(xEndDate).SQLSmallDateTime
                        ExecuteSqlWithoutTimeOut(strSQL)
                    Next

                    'ELIMINAR DATOS DESDE UN DIA DESPUES DEL ULTIMO CONTRATO HASTA EL FIN DE LOS TIEMPOS
                    If tbContracts.Rows(tbContracts.Rows.Count - 1).Item("EndDate") IsNot Nothing Then
                        xPreviousDate = tbContracts.Rows(tbContracts.Rows.Count - 1).Item("EndDate")
                        If xPreviousDate >= Now.Date Then
                            ' En el caso que el ultimo contrato tenga fecha de fin futura, no eliminamos los datos futuros
                            xPreviousDate = New Date(2079, 1, 1)
                        End If

                        For Each strTable As String In TablesDelete
                            strSQL = "@DELETE# FROM " & strTable & " WHERE IDEmployee=" & IDEmployee & " AND Date > " & Any2Time(xPreviousDate).SQLSmallDateTime
                            ExecuteSqlWithoutTimeOut(strSQL)
                        Next
                    End If

                End If

                bolRet = True
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roContract::RemoveDaysWithoutContract")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::RemoveDaysWithoutContract")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetMaxIDContract(ByVal _State As roContractState) As ULong
            Dim lngRet As ULong = 0
            Try
                Dim strSQL As String = "@SELECT# MAX(CONVERT(decimal(25,0),  REPLACE(IDContract,',','.'))) FROM EmployeeContracts WHERE not IDContract like '%[^0-9]%' and  ISNUMERIC(IDContract)=1"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then lngRet = tb.Rows(0).Item(0)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roContract::GetMaxIDContract")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roContract::GetMaxIDContract")
            Finally

            End Try

            Return lngRet

        End Function

        Public Shared Function GetContractDatesByEmployee(ByVal IDEmployee As Integer, ByVal StartDate As DateTime, ByVal FinishDate As DateTime, ByVal oState As roContractState) As Generic.List(Of roContractDates)

            Dim lstContractDates As New Generic.List(Of roContractDates)

            Try
                Dim strSQL As String = "@SELECT# BeginDate, EndDate FROM EmployeeContracts WHERE (IdEmployee = " & IDEmployee & ") AND " &
                                       "((BeginDate >= " & Any2Time(StartDate).SQLDateTime & " AND BeginDate  <= " & Any2Time(FinishDate).SQLDateTime & ") OR " &
                                       "(EndDate >= " & Any2Time(StartDate).SQLDateTime & " AND EndDate <= " & Any2Time(FinishDate).SQLDateTime & ") OR " &
                                       "(BeginDate  <= " & Any2Time(StartDate).SQLDateTime & " AND EndDate >= " & Any2Time(FinishDate).SQLDateTime & ")) ORDER BY BeginDate ASC, EndDate ASC"

                Dim oSqlCommand As DbCommand = CreateCommand(strSQL)
                Dim rd As DbDataReader = oSqlCommand.ExecuteReader()
                While rd.Read()
                    lstContractDates.Add(New roContractDates() With {.BeginDate = rd("BeginDate"), .EndDate = rd("EndDate")})
                End While

                rd.Close()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roContract::GetContractDatesByEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roContract::GetContractDatesByEmployee")
            Finally

            End Try

            Return lstContractDates

        End Function

#End Region

        Protected Overrides Sub Finalize()
            MyBase.Finalize()
        End Sub

    End Class

End Namespace