Imports System.Data.Common
Imports System.Data.SqlTypes
Imports System.Runtime.Serialization
Imports Microsoft.VisualBasic.Devices
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees.Contract
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace LabAgree

    <DataContract()>
    <Serializable>
    Public Class roLabAgree

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intID As Integer
        Private strName As String
        Private strDescription As String
        Private boolTelecommuting As Boolean
        Private strTelecommutingMandatoryDays As String
        Private strPresenceMandatoryDays As String
        Private strTelecommutingOptionalDays As String
        Private intTelecommutingMaxDays As Integer
        Private intTelecommutingMaxPercentage As Integer
        Private intTelecommutingPeriodType As Integer
        Private dtTelecommutingAgreementStart As Nullable(Of DateTime)
        Private dtTelecommutingAgreementEnd As Nullable(Of DateTime)
        Private intExtraHoursConfiguration As Integer
        Private strExtraHoursIDCauseSimples As String
        Private intExtraHoursIDCauseDoubles As Integer
        Private intExtraHoursIDCauseTriples As Integer
        'Private dtBeginDate As DateTime
        'Private dtFinishDate As DateTime

        Private oLabAgreeAccrualRules As New Generic.List(Of roLabAgreeAccrualRule)
        Private oStartupValues As New Generic.List(Of roStartupValue)
        Private oLabAgreeCauseLimitValues As New Generic.List(Of roLabAgreeCauseLimitValues)
        Private oRequestRules As New Generic.List(Of roRequestRule)

        Public Sub New()
            Me.oState = New roLabAgreeState
            Me.ID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.ID = _ID
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roLabAgreeState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roLabAgreeState)
                Me.oState = value
                If Me.oLabAgreeAccrualRules IsNot Nothing Then
                    For Each oLabAgreeAccRule As roLabAgreeAccrualRule In Me.oLabAgreeAccrualRules
                        oLabAgreeAccRule.State = Me.oState
                    Next
                End If
                If Me.oStartupValues IsNot Nothing Then
                    For Each oStartupValue As roStartupValue In Me.oStartupValues
                        oStartupValue.State = Me.oState
                    Next
                End If
                If Me.oRequestRules IsNot Nothing Then
                    For Each oRequestRule As roRequestRule In Me.oRequestRules
                        oRequestRule.State = Me.oState
                    Next
                End If

            End Set
        End Property

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
                If oLabAgreeAccrualRules IsNot Nothing Then
                    For Each oLabAgreeAR As roLabAgreeAccrualRule In oLabAgreeAccrualRules
                        oLabAgreeAR.IDLabAgree = intID
                    Next
                End If

                If oRequestRules IsNot Nothing Then
                    For Each oRequestRule As roRequestRule In oRequestRules
                        oRequestRule.IDLabAgree = intID
                    Next
                End If

            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property Telecommuting() As Boolean
            Get
                Return boolTelecommuting
            End Get
            Set(ByVal value As Boolean)
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
        Public Property TelecommutingPeriodType() As Integer
            Get
                Return intTelecommutingPeriodType
            End Get
            Set(ByVal value As Integer)
                intTelecommutingPeriodType = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingMaxDays() As Integer
            Get
                Return intTelecommutingMaxDays
            End Get
            Set(ByVal value As Integer)
                intTelecommutingMaxDays = value
            End Set
        End Property

        <DataMember()>
        Public Property TelecommutingMaxPercentage() As Integer
            Get
                Return intTelecommutingMaxPercentage
            End Get
            Set(ByVal value As Integer)
                intTelecommutingMaxPercentage = value
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
        Public Property LabAgreeAccrualRules() As Generic.List(Of roLabAgreeAccrualRule)
            Get
                Return Me.oLabAgreeAccrualRules
            End Get
            Set(ByVal value As Generic.List(Of roLabAgreeAccrualRule))
                Me.oLabAgreeAccrualRules = value
            End Set
        End Property

        <DataMember()>
        Public Property LabAgreeRequestRules() As Generic.List(Of roRequestRule)
            Get
                Return Me.oRequestRules
            End Get
            Set(ByVal value As Generic.List(Of roRequestRule))
                Me.oRequestRules = value
            End Set
        End Property

        <DataMember()>
        Public Property StartupValues() As Generic.List(Of roStartupValue)
            Get
                Return Me.oStartupValues
            End Get
            Set(ByVal value As Generic.List(Of roStartupValue))
                Me.oStartupValues = value
            End Set
        End Property

        <DataMember()>
        Public Property LabAgreeCauseLimitValues() As Generic.List(Of roLabAgreeCauseLimitValues)
            Get
                Return Me.oLabAgreeCauseLimitValues
            End Get
            Set(ByVal value As Generic.List(Of roLabAgreeCauseLimitValues))
                Me.oLabAgreeCauseLimitValues = value
            End Set
        End Property

        <DataMember()>
        Public Property ExtraHoursConfiguration() As Integer
            Get
                Return intExtraHoursConfiguration
            End Get
            Set(ByVal value As Integer)
                intExtraHoursConfiguration = value
            End Set
        End Property

        <DataMember()>
        Public Property ExtraHoursIDCauseSimples() As String
            Get
                Return strExtraHoursIDCauseSimples
            End Get
            Set(ByVal value As String)
                strExtraHoursIDCauseSimples = value
            End Set
        End Property
        <DataMember()>
        Public Property ExtraHoursIDCauseDoubles() As Integer
            Get
                Return intExtraHoursIDCauseDoubles
            End Get
            Set(ByVal value As Integer)
                intExtraHoursIDCauseDoubles = value
            End Set
        End Property
        <DataMember()>
        Public Property ExtraHoursIDCauseTriples() As Integer
            Get
                Return intExtraHoursIDCauseTriples
            End Get
            Set(ByVal value As Integer)
                intExtraHoursIDCauseTriples = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM LabAgree WHERE ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL)

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = Any2String(oRow("Name"))
                    Me.strDescription = Any2String(oRow("Description"))
                    Me.boolTelecommuting = Any2Boolean(oRow("Telecommuting"))
                    Me.strTelecommutingMandatoryDays = Any2String(oRow("TelecommutingMandatoryDays"))
                    Me.strPresenceMandatoryDays = Any2String(oRow("PresenceMandatoryDays"))
                    Me.strTelecommutingOptionalDays = Any2String(oRow("TelecommutingOptionalDays"))
                    Me.intTelecommutingMaxDays = Any2Integer(oRow("TelecommutingMaxDays"))
                    Me.intTelecommutingMaxPercentage = Any2Integer(oRow("TelecommutingMaxPercentage"))
                    Me.intTelecommutingPeriodType = Any2Integer(oRow("PeriodType"))
                    If Not IsDBNull(oRow("TelecommutingAgreementStart")) Then
                        Me.dtTelecommutingAgreementStart = Any2DateTime(oRow("TelecommutingAgreementStart"))
                    Else
                        Me.dtTelecommutingAgreementStart = Nothing
                    End If
                    If Not IsDBNull(oRow("TelecommutingAgreementEnd")) Then
                        Me.dtTelecommutingAgreementEnd = Any2DateTime(oRow("TelecommutingAgreementEnd"))
                    Else
                        Me.dtTelecommutingAgreementEnd = Nothing
                    End If
                    'Me.dtBeginDate = oRow("BeginDate")
                    'Me.dtFinishDate = oRow("FinishDate")

                    Me.intExtraHoursConfiguration = Any2Integer(oRow("ExtraHoursConfiguration"))
                    Me.strExtraHoursIDCauseSimples = Any2String(oRow("ExtraHoursIDCauseSimples"))
                    Me.intExtraHoursIDCauseDoubles = Any2Integer(oRow("ExtraHoursIDCauseDoubles"))
                    Me.intExtraHoursIDCauseTriples = Any2Integer(oRow("ExtraHoursIDCauseTriples"))

                    Me.oLabAgreeAccrualRules = roLabAgreeAccrualRule.GetLabAgreeAccrualRules(Me.ID, Me.oState, False)
                    Me.oStartupValues = Me.GetLabAgreeStartupValues(False)
                    Me.oLabAgreeCauseLimitValues = roLabAgreeCauseLimitValues.GetLabAgreeCauseLimitValues(Me.ID, Me.oState, False)
                    Me.oRequestRules = roRequestRule.GetRequestsRules("IDLabAgree=" & Me.ID.ToString, Me.oState, False)

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLabAgree, Me.strName, tbParameters, -1)
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLabAgree::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::Load")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nuevo convenio
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer
            Dim intRet As Integer = 0
            Dim strSQL As String = "@SELECT# MAX(ID) FROM LabAgree"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If
            Return intRet + 1
        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String
                Dim tb As DataTable
                Dim cmd As DbCommand
                Dim da As DbDataAdapter

                ' El nombre no puede estar en blanco
                If Me.Name = "" Then
                    oState.Result = LabAgreeResultEnum.NameCannotBeNull
                    bolRet = False
                    Return False
                End If

                If bolRet AndAlso bolCheckNames Then

                    ' Compuebo que el nombre no exista
                    tb = New DataTable()
                    strSQL = "@SELECT# * FROM LabAgree " &
                             "WHERE Name = @LabAgreeName AND " &
                                   "ID <> " & Me.ID.ToString
                    cmd = CreateCommand(strSQL)
                    AddParameter(cmd, "@LabAgreeName", DbType.String, 64)
                    cmd.Parameters("@LabAgreeName").Value = Me.Name
                    da = CreateDataAdapter(cmd, True)
                    tb.Rows.Clear()
                    da.Fill(tb)

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        oState.Result = LabAgreeResultEnum.NameAlreadyExist
                        bolRet = False
                    End If

                End If

                'Comprovació de linies duplicades a les regles de convenis
                If bolRet Then
                    If Me.oLabAgreeAccrualRules IsNot Nothing Then
                        For Each oLabAR As roLabAgreeAccrualRule In Me.oLabAgreeAccrualRules
                            Dim IDAccRule As Integer = oLabAR.IDAccrualRule
                            Dim intCount As Integer = 0
                            For Each oLabAR2 As roLabAgreeAccrualRule In Me.oLabAgreeAccrualRules
                                If IDAccRule = oLabAR2.IDAccrualRule Then intCount += 1
                            Next
                            If intCount > 1 Then
                                oState.Result = LabAgreeResultEnum.LabAgreeRulesDuplicatedInEdition
                                bolRet = False
                                Exit For
                            End If
                        Next
                    End If
                End If

                'Comprovació de linies duplicades als valors maxims de justificacions
                If bolRet Then
                    If Me.oLabAgreeAccrualRules IsNot Nothing Then
                        For Each oLabCL As roLabAgreeCauseLimitValues In Me.oLabAgreeCauseLimitValues
                            Dim IDCauseLimit As Integer = oLabCL.IDCauseLimitValue
                            Dim intCount As Integer = 0
                            For Each oLabCL2 As roLabAgreeCauseLimitValues In Me.oLabAgreeCauseLimitValues
                                If IDCauseLimit = oLabCL2.IDCauseLimitValue Then intCount += 1
                            Next
                            If intCount > 1 Then
                                oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitDuplicatedInEdition
                                bolRet = False
                                Exit For
                            End If
                        Next
                    End If
                End If

                'Comprovació de dates en les regles de convenis
                If bolRet Then bolRet = AccrualsRulesCheckDates()

                'Comprovació de dates en els valors maxims de justificacions
                If bolRet Then bolRet = CausesLimitsCheckDates()
                Try
                    Dim oServerLicense As New roServerLicense

                    Dim lotsOfLabagrees As Boolean = oServerLicense.FeatureIsInstalled("Feature\ConcertRules")
                    Dim legacy_TBR_onlyoneLabagree As Boolean = oServerLicense.FeatureIsInstalled("Feature\ConcertRules") AndAlso oServerLicense.FeatureIsInstalled("Feature\ConcertRulesSingle")

                    If Not lotsOfLabagrees OrElse (legacy_TBR_onlyoneLabagree) Then

                        strSQL = "@SELECT# Count(*) FROM LabAgree WHERE ID <> " & Me.ID
                        Dim TotalLabAgree As Long = ExecuteScalar(strSQL)
                        If TotalLabAgree >= 1 Then
                            oState.Result = LabAgreeResultEnum.OnlyOneLabAgreeAllowed
                            bolRet = False
                        End If
                    End If
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roLabAgree::Validate")
                End Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bIsNew As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                If Not DataLayer.roSupport.IsXSSSafe(Me) Then
                    Me.oState.Result = LabAgreeResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oOldLabAgree As roLabAgree = Nothing

                    Dim tb As New DataTable("LabAgree")
                    Dim strSQL As String = "@SELECT# * FROM LabAgree " &
                                           "WHERE ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        bIsNew = True
                        oRow = tb.NewRow
                        oRow("ID") = Me.GetNextID()
                        Me.ID = oRow("ID")
                    Else
                        oOldLabAgree = New roLabAgree(Me.ID, Me.State, False) ' Creamos una copia del convenio actual para poder recalcular después de la grabación
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription
                    oRow("Telecommuting") = Me.boolTelecommuting
                    oRow("TelecommutingMandatoryDays") = Me.strTelecommutingMandatoryDays
                    oRow("PresenceMandatoryDays") = Me.strPresenceMandatoryDays
                    oRow("TelecommutingOptionalDays") = Me.strTelecommutingOptionalDays
                    oRow("TelecommutingMaxDays") = Me.intTelecommutingMaxDays
                    oRow("TelecommutingMaxPercentage") = Me.intTelecommutingMaxPercentage
                    oRow("PeriodType") = Me.intTelecommutingPeriodType

                    If Me.dtTelecommutingAgreementStart Is Nothing OrElse Year(Me.dtTelecommutingAgreementStart) = 1 Then
                        oRow("TelecommutingAgreementStart") = DBNull.Value
                    Else
                        oRow("TelecommutingAgreementStart") = Me.dtTelecommutingAgreementStart
                    End If

                    If Me.dtTelecommutingAgreementEnd Is Nothing OrElse Year(Me.dtTelecommutingAgreementEnd) = 1 Then
                        oRow("TelecommutingAgreementEnd") = DBNull.Value
                    Else
                        oRow("TelecommutingAgreementEnd") = Me.dtTelecommutingAgreementEnd
                    End If

                    oRow("ExtraHoursConfiguration") = Me.intExtraHoursConfiguration
                    oRow("ExtraHoursIDCauseSimples") = Me.strExtraHoursIDCauseSimples
                    oRow("ExtraHoursIDCauseDoubles") = Me.intExtraHoursIDCauseDoubles
                    oRow("ExtraHoursIDCauseTriples") = Me.intExtraHoursIDCauseTriples

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If

                    ' Validamos si hay cambios de teletrabajo
                    Try
                        If Not bIsNew Then
                            Dim oOldTelecommuteAgreement As Employee.roTelecommuteAgreement = Nothing
                            Dim oNewTelecommuteAgreement As Employee.roTelecommuteAgreement = Nothing

                            If Me.boolTelecommuting Then
                                oNewTelecommuteAgreement = New Employee.roTelecommuteAgreement
                                oNewTelecommuteAgreement.AgreementStart = Me.TelecommutingAgreementStart
                                oNewTelecommuteAgreement.AgreementEnd = Me.TelecommutingAgreementEnd
                                oNewTelecommuteAgreement.MandatoryDays = Me.TelecommutingMandatoryDays
                                oNewTelecommuteAgreement.PresenceMandatoryDays = Me.PresenceMandatoryDays
                                oNewTelecommuteAgreement.OptionalDays = Me.TelecommutingOptionalDays
                                oNewTelecommuteAgreement.MaxDays = Me.TelecommutingMaxDays
                                oNewTelecommuteAgreement.MaxPercentage = Me.TelecommutingMaxPercentage
                                oNewTelecommuteAgreement.PeriodType = Me.TelecommutingPeriodType
                            End If
                            If oOldLabAgree.Telecommuting Then
                                oOldTelecommuteAgreement = New Employee.roTelecommuteAgreement
                                oOldTelecommuteAgreement.AgreementStart = oOldLabAgree.TelecommutingAgreementStart.Value
                                oOldTelecommuteAgreement.AgreementEnd = oOldLabAgree.TelecommutingAgreementEnd.Value
                                oOldTelecommuteAgreement.MandatoryDays = oOldLabAgree.TelecommutingMandatoryDays
                                oOldTelecommuteAgreement.PresenceMandatoryDays = oOldLabAgree.PresenceMandatoryDays
                                oOldTelecommuteAgreement.OptionalDays = oOldLabAgree.TelecommutingOptionalDays
                                oOldTelecommuteAgreement.MaxDays = oOldLabAgree.TelecommutingMaxDays
                                oOldTelecommuteAgreement.MaxPercentage = oOldLabAgree.TelecommutingMaxPercentage
                                oOldTelecommuteAgreement.PeriodType = oOldLabAgree.TelecommutingPeriodType
                            End If

                            Dim oTelecommuteEmployeeAgreement As New Employee.roEmployeeTelecommuteAgreement
                            oTelecommuteEmployeeAgreement.RecalculateTelecommutingChanges(TelecommuteAgreementSource.LabAgree, roTypes.Any2String(Me.ID), oOldTelecommuteAgreement, oNewTelecommuteAgreement, New Employee.roEmployeeState(Me.State.IDPassport))
                        End If
                    Catch ex As Exception
                    End Try

                    ' Validamos si hay cambios en la gestion de horas extras de latam
                    ' Gestion de horas extras en latam
                    Try
                        If roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Latam.OvertimeManagement")) Then
                            If Not bIsNew Then
                                If Me.DefinitioLatamOvertimeRaw() <> oOldLabAgree.DefinitioLatamOvertimeRaw() Then
                                    Me.RecalculateLatamOvertime()
                                End If
                            End If
                        End If
                    Catch ex As Exception
                    End Try

                    da.Update(tb)

                    ' Actualizamos la reglas de convenio

                    If Me.oLabAgreeAccrualRules IsNot Nothing AndAlso Me.oLabAgreeAccrualRules.Count > 0 Then
                        bolRet = roLabAgreeAccrualRule.SaveLabAgreeAccrualRules(Me.ID, Me.oLabAgreeAccrualRules, Me.oState, False)
                    Else
                        bolRet = roLabAgreeAccrualRule.DeleteLabAgreeAccrualRules(Me.ID, Me.State, False)
                    End If

                    ' Actualizamos los valores iniciales
                    If bolRet Then
                        If Me.oStartupValues IsNot Nothing AndAlso Me.oStartupValues.Count > 0 Then
                            bolRet = SaveLabAgreeStartupValues(Me.ID, False)
                        Else
                            bolRet = DeleteLabAgreeStartupValues(Me.ID, False)
                        End If
                    End If

                    ' Actualizamos los limites por justificacion
                    If bolRet Then
                        If Me.oLabAgreeCauseLimitValues IsNot Nothing AndAlso Me.oLabAgreeCauseLimitValues.Count > 0 Then
                            bolRet = roLabAgreeCauseLimitValues.SaveLabAgreeCauseLimitValues(Me.ID, Me.oLabAgreeCauseLimitValues, Me.oState, False)
                        Else
                            bolRet = roLabAgreeCauseLimitValues.DeleteLabAgreeCauseLimitValues(Me.ID, Me.State, False)
                        End If
                    End If

                    ' Actualizamos reglas de solicitud
                    If bolRet Then
                        If Me.oRequestRules IsNot Nothing AndAlso Me.oRequestRules.Count > 0 Then
                            bolRet = roRequestRule.SaveLabAgreeRequestRules(Me.ID, Me.oRequestRules, Me.oState, False)
                        Else
                            bolRet = roRequestRule.DeleteLabAgreeRequestRules(Me.ID, Me.State, False)
                        End If
                    End If

                    oAuditDataNew = oRow

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tLabAgree, strObjectName, tbAuditParameters, -1)
                    End If
                End If

                Try
                    'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                    If bolRet Then
                        Dim oParamsAux As New roCollection
                        oParamsAux.Add("ObjectID", Me.ID)
                        oParamsAux.Add("Action", CacheAction.InsertOrUpdate.ToString)

                        roConnector.InitTask(TasksType.LABAGREES, oParamsAux)
                    End If
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roLabAgree::Save::Could not send cache update")
                End Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function DefinitioLatamOvertimeRaw() As String
            Return ExtraHoursConfiguration.ToString & ExtraHoursIDCauseSimples & ExtraHoursIDCauseDoubles.ToString & ExtraHoursIDCauseTriples.ToString
        End Function


        ''' <summary>
        ''' Borra el acumulado siempre y cuando no se use.
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed() Then

                    'Borramos las reglas de convenio
                    bolRet = roLabAgreeAccrualRule.DeleteLabAgreeAccrualRules(Me.ID, Me.oState)

                    'Borramos los valores iniciales
                    If bolRet Then bolRet = DeleteLabAgreeStartupValues(Me.ID)

                    'Borramos los limites por justificacion
                    If bolRet Then bolRet = roLabAgreeCauseLimitValues.DeleteLabAgreeCauseLimitValues(Me.ID, Me.oState)

                    ' borramos las reglas de solicitud
                    If bolRet Then bolRet = roRequestRule.DeleteLabAgreeRequestRules(Me.ID, Me.oState)

                    If bolRet Then

                        bolRet = False

                        'Borramos el concepto
                        Dim DelQuerys() As String = {"@DELETE# FROM LabAgree WHERE ID = " & Me.ID.ToString}
                        For n As Integer = 0 To DelQuerys.Length - 1
                            If Not ExecuteSql(DelQuerys(n)) Then
                                oState.Result = LabAgreeResultEnum.ConnectionError
                                Exit For
                            End If
                        Next

                        bolRet = (oState.Result = LabAgreeResultEnum.NoError)

                        If bolRet And bAudit Then
                            ' Auditamos
                            bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tLabAgree, Me.strName, Nothing, -1)
                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Debemos hacer el cambio una vez se ha validado la transacción para que el resto de funciones puedan acceder al elemento creado.
                If bolRet Then
                    Dim oParamsAux As New roCollection
                    oParamsAux.Add("ObjectID", Me.ID)
                    oParamsAux.Add("Action", CacheAction.Delete.ToString)

                    roConnector.InitTask(TasksType.LABAGREES, oParamsAux)
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLabAgree::Save::Could not send cache update")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica si el convenio se está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                Dim strSQL As String
                Dim tb As DataTable
                Dim strUsedEmployees As String = ""

                ' Verifica si se usa en contratos de empleados
                strSQL = "@SELECT# Employees.Name From Employees, EmployeeContracts Where Employees.ID = EmployeeContracts.IDEmployee And EmployeeContracts.IDLabAgree = " & Me.intID
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then
                    strUsedEmployees = ""
                    For Each oRow As DataRow In tb.Rows
                        ' Guardo el nombre de todos los empleados que lo usan
                        strUsedEmployees &= "," & oRow("Name")
                    Next
                    If strUsedEmployees <> "" Then strUsedEmployees = strUsedEmployees.Substring(1)
                    If strUsedEmployees <> "" Then
                        oState.Result = LabAgreeResultEnum.UsedInEmployeeContracts
                        bolIsUsed = True
                    End If
                End If

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::IsUsed")
            End Try

            Return bolIsUsed

        End Function

        ''' <summary>
        ''' Comprovem que no es modifiqui cap regla de conveni que no es pot modificar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function CausesLimitsCheckDates() As Boolean
            Dim bolRet As Boolean = True

            Try

                ' Obtenemos la fecha de congelación
                Dim xFirstDate As Date = roParameters.GetFirstDate()

                'Si es una modificació del registre
                If Me.intID > 0 Then
                    If Me.oLabAgreeCauseLimitValues IsNot Nothing Then
                        For Each oLabCL As roLabAgreeCauseLimitValues In Me.oLabAgreeCauseLimitValues
                            Dim strSQL As String = "@SELECT# * From LabAgreeCauseLimitValues Where IDLabAgree = " & Me.intID & " And IDCauseLimitValue = " & oLabCL.IDCauseLimitValue
                            Dim dTblAR As DataTable = CreateDataTable(strSQL, )
                            If dTblAR.Rows.Count > 0 Then
                                'Si la data fi esta dins el periode congelació i han tocat alguna data, error
                                If dTblAR.Rows(0)("EndDate") <> oLabCL.EndDate And oLabCL.EndDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabCL.CauseLimitValue.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitEndDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If

                                'Si la data inici esta dins de la data congelació i la nova data inici es diferent, error
                                If dTblAR.Rows(0)("BeginDate") <> oLabCL.BeginDate And oLabCL.BeginDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabCL.CauseLimitValue.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitBeginDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If
                            Else 'Si es una nova linia
                                'Si la data fi es troba dins de la data de congelació
                                If oLabCL.EndDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabCL.CauseLimitValue.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitEndDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If

                                'Si la data inici es troba dins de la data de congelació
                                If oLabCL.BeginDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabCL.CauseLimitValue.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitBeginDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                Else 'Si es un nou registre (ID <= 0)
                    If Me.oLabAgreeCauseLimitValues IsNot Nothing Then
                        For Each oLabCL As roLabAgreeCauseLimitValues In Me.oLabAgreeCauseLimitValues
                            'Si la data fi es troba dins de la data de congelació
                            If oLabCL.EndDate <= xFirstDate Then
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(oLabCL.CauseLimitValue.Name)
                                oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitEndDateInFreezePeriod
                                bolRet = False
                                Exit For
                            End If

                            'Si la data inici es troba dins de la data de congelació
                            If oLabCL.BeginDate <= xFirstDate Then
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(oLabCL.CauseLimitValue.Name)
                                oState.Result = LabAgreeResultEnum.LabAgreeCauseLimitBeginDateInFreezePeriod
                                bolRet = False
                                Exit For
                            End If
                        Next
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::CausesLimitsCheckDates")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::CausesLimitsCheckDates")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Comprovem que no es modifiqui cap regla de conveni que no es pot modificar
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function AccrualsRulesCheckDates() As Boolean
            Dim bolRet As Boolean = True

            Try

                ' Obtenemos la fecha de congelación
                Dim xFirstDate As Date = roParameters.GetFirstDate()

                'Si es una modificació del registre
                If Me.intID > 0 Then
                    If Me.oLabAgreeAccrualRules IsNot Nothing Then
                        For Each oLabAR As roLabAgreeAccrualRule In Me.oLabAgreeAccrualRules
                            Dim strSQL As String = "@SELECT# * From LabAgreeAccrualsRules Where IDLabAgree = " & Me.intID & " And IdAccrualsRules = " & oLabAR.IDAccrualRule
                            Dim dTblAR As DataTable = CreateDataTable(strSQL, )
                            If dTblAR.Rows.Count > 0 Then
                                'Si la data fi esta dins el periode congelació i han tocat alguna data, error
                                If dTblAR.Rows(0)("EndDate") <> oLabAR.EndDate And oLabAR.EndDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabAR.LabAgreeRule.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeRulesEndDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If

                                'Si la data inici esta dins de la data congelació i la nova data inici es diferent, error
                                If dTblAR.Rows(0)("BeginDate") <> oLabAR.BeginDate And oLabAR.BeginDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabAR.LabAgreeRule.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeRulesBeginDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If
                            Else 'Si es una nova linia
                                'Si la data fi es troba dins de la data de congelació
                                If oLabAR.EndDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabAR.LabAgreeRule.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeRulesEndDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If

                                'Si la data inici es troba dins de la data de congelació
                                If oLabAR.BeginDate <= xFirstDate Then
                                    oState.Language.ClearUserTokens()
                                    oState.Language.AddUserToken(oLabAR.LabAgreeRule.Name)
                                    oState.Result = LabAgreeResultEnum.LabAgreeRulesBeginDateInFreezePeriod
                                    bolRet = False
                                    Exit For
                                End If
                            End If
                        Next
                    End If
                Else 'Si es un nou registre (ID <= 0)
                    If Me.oLabAgreeAccrualRules IsNot Nothing Then
                        For Each oLabAR As roLabAgreeAccrualRule In Me.oLabAgreeAccrualRules
                            'Si la data fi es troba dins de la data de congelació
                            If oLabAR.EndDate <= xFirstDate Then
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(oLabAR.LabAgreeRule.Name)
                                oState.Result = LabAgreeResultEnum.LabAgreeRulesEndDateInFreezePeriod
                                bolRet = False
                                Exit For
                            End If

                            'Si la data inici es troba dins de la data de congelació
                            If oLabAR.BeginDate <= xFirstDate Then
                                oState.Language.ClearUserTokens()
                                oState.Language.AddUserToken(oLabAR.LabAgreeRule.Name)
                                oState.Result = LabAgreeResultEnum.LabAgreeRulesBeginDateInFreezePeriod
                                bolRet = False
                                Exit For
                            End If
                        Next
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::AccrualsRulesCheckDates")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::AccrualsRulesCheckDates")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Lanza el recálculo para las reglas y valores iniciales del convenio para un empleado.
        ''' </summary>
        ''' <param name="IDEmployee">Código del empleado a recalcular.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Recalculate(ByVal oOldLabAgree As roLabAgree, ByVal IDEmployee As Integer, ByVal _ModifyDate As Date) As Boolean

            Dim bolRet As Boolean = True

            Try

                If oOldLabAgree Is Nothing Then
                    For Each oRule As roLabAgreeAccrualRule In Me.LabAgreeAccrualRules
                        bolRet = oRule.Recalculate(Nothing, IDEmployee, Nothing)
                        If Not bolRet Then Exit For
                    Next

                    For Each oCL As roLabAgreeCauseLimitValues In Me.LabAgreeCauseLimitValues
                        bolRet = oCL.Recalculate(Nothing, IDEmployee, Nothing)
                        If Not bolRet Then Exit For
                    Next
                Else
                    For Each oRule As roLabAgreeAccrualRule In oOldLabAgree.LabAgreeAccrualRules
                        bolRet = oRule.Recalculate(Nothing, IDEmployee, _ModifyDate)
                        If Not bolRet Then Exit For
                    Next

                    For Each oCL As roLabAgreeCauseLimitValues In oOldLabAgree.LabAgreeCauseLimitValues
                        bolRet = oCL.Recalculate(Nothing, IDEmployee, Nothing)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    bolRet = Me.RecalculateStartupValues(Nothing, IDEmployee, Nothing)
                End If

                If bolRet Then
                    If roTypes.Any2Boolean(roCacheManager.GetInstance().GetAdvParametersCache(Azure.RoAzureSupport.GetCompanyName(), "Latam.OvertimeManagement")) Then
                        bolRet = Me.RecalculateLatamOvertime(IDEmployee, _ModifyDate)
                    End If
                End If


            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::Recalculate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::Recalculate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Function ExistStartupValueInList(ByVal lstStartupValues As Generic.List(Of roStartupValue), ByVal oStartupValue As roStartupValue) As roStartupValue

            Dim oRet As roStartupValue = Nothing

            If lstStartupValues IsNot Nothing Then

                For Each oItem As roStartupValue In lstStartupValues
                    If oItem.ID = oStartupValue.ID Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

        ''' <summary>
        ''' Lanza el proceso de recálculo de la gestión de horas extras de latam
        ''' </summary>
        ''' <param name="OldLabAgree">Parametros antiguos .</param>
        ''' <param name="NewLabAgree">Nuevos valores.</param>
        ''' <param name="_IDEmployee">Opcional. Si se indica, solo se lanza el recálculo para el empleado indicado.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RecalculateLatamOvertime(Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifyDate As Date = Nothing) As Boolean

            Dim bolRet As Boolean = True

            Try
                ' Obtenemos la fecha de congelación
                Dim xFreezingDate = roParameters.GetFirstDate()

                xFreezingDate = xFreezingDate.AddDays(1)

                Dim strSQL As String

                Dim strSQLEmployees As String = "@SELECT# DISTINCT EmployeeContracts.IDEmployee, EmployeeContracts.BeginDate, EmployeeContracts.EndDate " &
                                                    "FROM EmployeeContracts WITH(NOLOCK)   INNER JOIN LabAgree WITH(NOLOCK) " &
                                                            "ON EmployeeContracts.IDLabAgree = LabAgree.ID " &
                                                    "WHERE LabAgree.ID = " & Me.ID.ToString
                If _IDEmployee <> -1 Then
                    strSQLEmployees &= " AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                End If

                Dim tbEmployees As DataTable = CreateDataTableWithoutTimeouts(strSQLEmployees)
                If tbEmployees IsNot Nothing Then

                    Dim xStatusDate As Date
                    Dim xStatusEndDate As Date

                    If tbEmployees.Rows.Count > 0 Then

                        ' Recorremos los empleados implicados para actualizar el Status de la tabla DailySchedule a 65 para cada contrato

                        For Each oRow As DataRow In tbEmployees.Rows
                            Dim xFirstDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oRow("IDEmployee"), False, Me.oState)
                            xFirstDateEmployee = xFirstDateEmployee.AddDays(1)

                            ' Obtenemos la fecha para actualizar el status=65 a DailySchedule
                            xStatusDate = xFreezingDate
                            If xStatusDate < xFirstDateEmployee Then xStatusDate = xFirstDateEmployee

                            If xStatusDate < oRow("BeginDate") Then
                                xStatusDate = oRow("BeginDate")
                            End If

                            xStatusEndDate = New DateTime(Math.Min(Any2DateTime(oRow("EndDate")).Ticks, Now.Date.Ticks))

                            ' Actualizamos la tabla DailySchedule
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                         "WHERE Status >= 65 And " &
                                               "IDEmployee = " & oRow("IDEmployee") & " And Date >= " & Any2Time(xStatusDate).SQLSmallDateTime & " And Date <= " & Any2Time(xStatusEndDate).SQLSmallDateTime

                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        Next

                    ElseIf _ModifyDate <> Nothing And _IDEmployee <> -1 Then

                        ' Obtenemos la fecha para actualizar el status=65 a DailySchedule
                        Dim xFirstDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(_IDEmployee, False, Me.oState)
                        xFirstDateEmployee = xFirstDateEmployee.AddDays(1)

                        xStatusDate = xFreezingDate
                        If xStatusDate < xFirstDateEmployee Then xStatusDate = xFirstDateEmployee

                        If xStatusDate < _ModifyDate Then
                            xStatusDate = _ModifyDate
                        End If

                        Dim oCurrentContract As roContract = Contract.roContract.GetContractInDateLite(_IDEmployee, _ModifyDate, New Contract.roContractState(Me.oState.IDPassport))
                        If Not oCurrentContract Is Nothing Then
                            xStatusEndDate = New DateTime(Math.Min(Any2DateTime(oCurrentContract.EndDate).Ticks, Now.Date.Ticks))
                        Else
                            xStatusEndDate = Now.Date
                        End If

                        ' Actualizamos la tabla DailySchedule
                        strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                     "WHERE Status >= 65 And " &
                                           "IDEmployee = " & _IDEmployee & " And Date >= " & Any2Time(xStatusDate).SQLSmallDateTime & " And Date <= " & Any2Time(xStatusEndDate).SQLSmallDateTime
                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                    End If

                End If

                If Not bolRet AndAlso oState.Result = LabAgreeResultEnum.NoError Then
                    oState.Result = LabAgreeResultEnum.SqlError
                End If


            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::RecalculateLatamOvertime")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::RecalculateLatamOvertime")
                bolRet = False
            End Try

            Return bolRet

        End Function

#Region "StartupValues"

        ''' <summary>
        ''' Devuelve valores iniciales asociados al convenio
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetLabAgreeStartupValues(Optional ByVal bAudit As Boolean = False) As Generic.List(Of roStartupValue)

            Dim oRet As New Generic.List(Of roStartupValue)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeStartupValues " &
                                       "WHERE IDLabAgree = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                For Each oLabAgreeRuleRow As DataRow In tb.Rows
                    If (roTypes.Any2String(oLabAgreeRuleRow("IDStartupValue")).Length > 0) Then
                        oRet.Add(New roStartupValue(oLabAgreeRuleRow("IDStartupValue"), Me.oState, bAudit))
                    End If
                Next
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLabAgree::GetLabAgreeStartupValues")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLabAgree::GetLabAgreeStartupValues")
            Finally

            End Try

            Return oRet

        End Function



        Public Function SaveLabAgreeStartupValues(ByVal idLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos los valores iniciales anteriores
                Dim OldStartupValues As Generic.List(Of roStartupValue) = Me.GetLabAgreeStartupValues(False)

                Dim oOldStartupValue As roStartupValue = Nothing

                Dim strIDs As String = ""
                If oStartupValues IsNot Nothing Then
                    bolRet = True
                    For Each oStartupValue As roStartupValue In oStartupValues

                        ' Guardamos la definicio del valor inicial
                        bolRet = oStartupValue.Save(idLabAgree, True)
                        If Not bolRet Then
                            Return bolRet
                            Exit Function
                        End If

                        Dim tb As New DataTable("LabAgreeStartupValues")
                        Dim strSQL As String = "@SELECT# * FROM LabAgreeStartupValues " &
                                               "WHERE IDLabAgree = " & idLabAgree.ToString & " AND IDStartupValue = " & oStartupValue.ID.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            oRow("IDLabAgree") = Me.intID
                            oRow("IDConcept") = oStartupValue.IDConcept
                            oRow("IDStartupValue") = oStartupValue.ID
                        Else
                            oRow = tb.Rows(0)
                        End If

                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)

                        oOldStartupValue = ExistStartupValueInList(OldStartupValues, oStartupValue)

                        bolRet = oStartupValue.Recalculate(oOldStartupValue, , , False)
                        If Not bolRet Then Exit For

                        strIDs &= "," & oStartupValue.ID.ToString
                    Next
                    If strIDs <> "" Then strIDs = strIDs.Substring(1)
                Else
                    bolRet = True
                End If

                If bolRet Then
                    If bolRet Then
                        ' Borramos los valores iniciales que ya no existan en el convenio
                        For Each oStartupValue As roStartupValue In OldStartupValues
                            If ExistStartupValueInList(oStartupValues, oStartupValue) Is Nothing Then
                                bolRet = oStartupValue.Delete(idLabAgree, True)
                                If Not bolRet Then Exit For
                            End If
                        Next
                    End If
                    ' Borramos las composiciones de la tabla que no esten en la lista y sus definiciones
                    Dim strSQL As String = "@DELETE# FROM LabAgreeStartupValues " &
                                               "WHERE IDLabAgree = " & idLabAgree.ToString & " AND NOT IDStartupValue IN (" & strIDs & ")"
                    bolRet = ExecuteSql(strSQL)

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::SaveLabAgreeStartupValues")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::SaveLabAgreeStartupValues")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function DeleteLabAgreeStartupValues(ByVal idLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos los valores iniciales anteriores
                Dim OldStartupValues As Generic.List(Of roStartupValue) = Me.GetLabAgreeStartupValues()

                Dim strSQL As String

                bolRet = True
                For Each oStartupValue As roStartupValue In OldStartupValues
                    bolRet = oStartupValue.Delete(idLabAgree, bAudit)
                    If Not bolRet Then Exit For
                Next

                If bolRet Then
                    strSQL = "@DELETE# FROM LabAgreeStartupValues WHERE IDLabAgree = " & idLabAgree
                    bolRet = ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::DeleteLabAgreeStartupValues")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::DeleteLabAgreeStartupValues")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function ValidateLabAgreeStartupValues(ByVal idLabAgree As Integer) As Boolean
            Dim bolRet As Boolean = False

            Try

                ' Verificar que
                If Me.oStartupValues.Count > 0 Then
                    For Each oStartupValues As roStartupValue In Me.oStartupValues
                        bolRet = oStartupValues.Validate(idLabAgree)
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roLabAgree::ValidateLabAgreeStartupValues")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roLabAgree::ValidateLabAgreeStartupValue")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Lanza el proceso de recálculo de los valores iniciales modificados del convenio.
        ''' </summary>
        ''' <param name="OldStartupValues">Lista anterior de valores iniciales. Necesaria para determinar que valores han cambiado. Si es Nothing, se recalcularán todos los valores iniciales del convenio.</param>
        ''' <param name="_IDEmployee">Opcional. Si se indica, solo se lanza el recálculo para el empleado indicado.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Private Function RecalculateStartupValues(ByVal OldStartupValues As Generic.List(Of roStartupValue), Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifyDate As Date = Nothing) As Boolean

            Dim bolRet As Boolean = True

            Try

                ' Obtenemos los valores iniciales que han cambiado
                Dim lstStartupValuesChanged As New Generic.List(Of roStartupValue)
                If OldStartupValues IsNot Nothing Then
                    For Each oItem As roStartupValue In OldStartupValues
                        If Me.ExistStartupValueInList(Me.StartupValues, oItem) Is Nothing Then
                            If Me.ExistStartupValueInList(lstStartupValuesChanged, oItem) Is Nothing Then lstStartupValuesChanged.Add(oItem)
                        End If
                    Next
                End If
                If Me.StartupValues IsNot Nothing Then
                    For Each oItem As roStartupValue In Me.StartupValues
                        If Me.ExistStartupValueInList(OldStartupValues, oItem) Is Nothing Then
                            If Me.ExistStartupValueInList(lstStartupValuesChanged, oItem) Is Nothing Then lstStartupValuesChanged.Add(oItem)
                        End If
                    Next
                End If

                If lstStartupValuesChanged.Count > 0 Then

                    Dim strSQL As String

                    ' Obtenemos los empleados que tengan asignado este convenio
                    strSQL = "@SELECT# DISTINCT EmployeeContracts.IDEmployee " &
                             "FROM EmployeeContracts " &
                             "WHERE "
                    If _IDEmployee = -1 Then
                        strSQL &= "IDLabAgree = " & Me.ID.ToString
                    Else
                        strSQL &= "IDEmployee = " & _IDEmployee.ToString
                    End If
                    Dim tbEmployees As DataTable = CreateDataTable(strSQL)
                    If tbEmployees IsNot Nothing AndAlso tbEmployees.Rows.Count > 0 Then

                        ' Recorremos los valores iniciales que han cambiado
                        For Each oStartupValue As roStartupValue In lstStartupValuesChanged

                            ' Recorremos todos los empleados con el convenio asignado y lanzamos el recálculo del valor inicial
                            For Each oEmployeeRow As DataRow In tbEmployees.Rows
                                bolRet = oStartupValue.Recalculate(Nothing, oEmployeeRow("IDEmployee"), _ModifyDate)
                                If Not bolRet Then Exit For
                            Next

                            If Not bolRet Then Exit For

                        Next

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::RecalculateStartupValues")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::RecalculateStartupValues")
                bolRet = False
            End Try

            Return bolRet

        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roLabAgreeState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

#End Region

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un Datatable con todos los convenios
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLabAgrees(ByVal _State As roLabAgreeState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * from LabAgree Order By Name"

                oRet = CreateDataTable(strSQL, )

                'If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                '    ' Auditamos consulta masiva
                '    Dim tbAuditParameters As DataTable = Audit.roAudit.CreateParametersTable()
                '    Dim strAuditName As String = ""
                '    For Each oRow As DataRow In oRet.Rows
                '        strAuditName &= IIf(strAuditName <> "", ",", "") & oRow("IDContract")
                '    Next
                '    Audit.roAudit.AddParameter(tbAuditParameters, "{IDContracts}", strAuditName, "", 1)
                '    Dim strEmployeeName As String = GetEmployeeName(IDEmployee, _State)
                '    Audit.roAudit.AddParameter(tbAuditParameters, "{EmployeeName}", strEmployeeName, "", 1)
                '    _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tEmployeeContract, strAuditName & " (" & strEmployeeName & ")", tbAuditParameters, -1)
                'End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgree::GetLabAgrees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgree::GetLabAgrees")
            Finally

            End Try

            Return oRet

        End Function
        Public Shared Function ValidateLabAgreeDailyCausesOnDate(ByVal idEmployee As Integer, ByVal valDate As DateTime, ByVal valIdCause() As Integer, ByVal valValue() As Double, ByVal oState As roLabAgreeState) As Boolean
            Dim bolRet As Boolean = True

            Try

                Dim oContractState As New Contract.roContractState(oState.IDPassport)
                Dim oEmployeeState As New Employee.roEmployeeState(oState.IDPassport)

                Dim bExecuteSQL As String = "@SELECT# COUNT(*) FROM CauseLimitValues"

                If roTypes.Any2Integer(ExecuteScalar(bExecuteSQL)) > 0 Then
                    'Cargamos su contracto activo en la fecha

                    Dim oContract As Contract.roContract = Contract.roContract.GetContractInDate(idEmployee, valDate, oContractState, False)
                    If oState.Result = LabAgreeResultEnum.NoError Then
                        Dim oLabAgree As roLabAgree = oContract.LabAgree

                        'Si el empleado tiene convenio asignado y este tiene maximos de justificaciones en esa fecha seguimos calculando
                        If oLabAgree IsNot Nothing AndAlso oLabAgree.oLabAgreeCauseLimitValues.Count > 0 Then

                            Dim curIndex As Integer = 0
                            For Each oIDCause As Integer In valIdCause
                                For Each oLimit As roLabAgreeCauseLimitValues In oLabAgree.oLabAgreeCauseLimitValues
                                    'Si encontramos un límite y aplica por su periodo de validez y justificación pasamos a mirar el valor
                                    If oLimit.BeginDate <= valDate AndAlso oLimit.EndDate >= valDate AndAlso oLimit.CauseLimitValue.IDCause = oIDCause Then

                                        'Tenemos que comprobar limites anuales
                                        If oLimit.CauseLimitValue.MaximumAnnualValueType <> LabAgreeValueType.None Then
                                            Dim oParams As New roParameters("OPTIONS", True)
                                            Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                                            Dim intYearIniMonth As Integer = oParams.Parameter(Parameters.YearPeriod)
                                            If intMonthIniDay = 0 Then intMonthIniDay = 1
                                            If intYearIniMonth = 0 Then intYearIniMonth = 1

                                            Dim queryBeginLimit As DateTime
                                            If valDate.Month > intYearIniMonth Then
                                                queryBeginLimit = New DateTime(valDate.Year, intYearIniMonth, intMonthIniDay)
                                            ElseIf valDate.Month = intYearIniMonth And valDate.Day >= intMonthIniDay Then
                                                queryBeginLimit = New DateTime(valDate.Year, intYearIniMonth, intMonthIniDay)
                                            Else
                                                queryBeginLimit = New DateTime(valDate.Year - 1, intYearIniMonth, intMonthIniDay)
                                            End If

                                            Dim queryEndLimit As DateTime = queryBeginLimit.AddYears(1).AddDays(-1)
                                            If oContract.BeginDate > queryBeginLimit Then queryBeginLimit = oContract.BeginDate
                                            If oContract.EndDate < queryEndLimit Then queryEndLimit = oContract.EndDate

                                            'Calculamos el valor del saldo para todo el periodo que estamos buscando salvo el día a validar
                                            Dim strSQL As String = "@SELECT# ISNULL(SUM(Value),0) FROM DailyCauses dc " &
                                                                       " INNER JOIN DailySchedule ds on ds.Date = dc.Date and ds.IDEmployee = dc.IDEmployee" &
                                                                   " WHERE ds.IDEmployee = " & idEmployee &
                                                                       " AND dc.IDCause = " & oIDCause &
                                                                       " AND dc.Date BETWEEN " & roTypes.Any2Time(queryBeginLimit).SQLSmallDateTime & " AND " & roTypes.Any2Time(queryEndLimit).SQLSmallDateTime &
                                                                       " AND dc.DATE <> " & roTypes.Any2Time(valDate).SQLSmallDateTime & " AND ds.Status >= 70"

                                            Dim causeValue As Decimal = roTypes.Any2Double(ExecuteScalar(strSQL))

                                            If oLimit.CauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.DirectValue Then
                                                'Si el valor esta indicado directamente en el máximo anual tenemos que comprobar el valor del periodo + nuevo valor del día con el campo directo
                                                If (causeValue + valValue(curIndex)) > oLimit.CauseLimitValue.MaximumAnnualValue Then
                                                    bolRet = False
                                                End If
                                            Else
                                                Dim oUserFieldState As New roUserFieldState(oState.IDPassport)
                                                'Si el valor esta indicado en un campo de la ficha en el máximo anual tenemos que comprobar el valor del periodo + nuevo valor del día con el campo de la ficha a fecha final de contrato
                                                Dim rFieldValue As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(idEmployee),
                                                                                            oLimit.CauseLimitValue.MaximumAnnualField.FieldName, oContract.EndDate, oUserFieldState, False)
                                                If rFieldValue.FieldValue IsNot Nothing Then
                                                    Dim maxValue As Double = 0
                                                    If rFieldValue.Definition.FieldType = FieldTypes.tDecimal Or rFieldValue.Definition.FieldType = FieldTypes.tNumeric Then
                                                        maxValue = roTypes.Any2Double(rFieldValue.FieldValue)
                                                    ElseIf rFieldValue.Definition.FieldType = FieldTypes.tTime Then
                                                        maxValue = roConversions.ConvertTimeToHours(roTypes.Any2String(rFieldValue.FieldValue))
                                                    End If

                                                    If (causeValue + valValue(curIndex)) > maxValue Then
                                                        bolRet = False
                                                    End If
                                                End If

                                            End If

                                        End If

                                        'Tenemos que comprobar limites mensuales
                                        If bolRet AndAlso oLimit.CauseLimitValue.MaximumMonthlyType <> LabAgreeValueType.None Then
                                            Dim oParams As New roParameters("OPTIONS", True)
                                            Dim intMonthIniDay As Integer = oParams.Parameter(Parameters.MonthPeriod)
                                            If intMonthIniDay = 0 Then intMonthIniDay = 1

                                            Dim queryBeginLimit As DateTime
                                            If valDate.Day > intMonthIniDay Then
                                                'Si el dia es posterior al inicio del periodo (mismo mes)
                                                queryBeginLimit = New Date(valDate.Year, valDate.Month, intMonthIniDay)
                                            ElseIf valDate.Day < intMonthIniDay Then
                                                'Si el dia es anterior al inicio del periodo (mes anterior)
                                                queryBeginLimit = New Date(valDate.AddMonths(-1).Year, valDate.AddMonths(-1).Month, intMonthIniDay)
                                            Else
                                                'Si es el mismo dia
                                                queryBeginLimit = valDate
                                            End If

                                            Dim queryEndLimit As DateTime = queryBeginLimit.AddMonths(1).AddDays(-1)
                                            If oContract.BeginDate > queryBeginLimit Then queryBeginLimit = oContract.BeginDate
                                            If oContract.EndDate < queryEndLimit Then queryEndLimit = oContract.EndDate

                                            'Calculamos el valor del saldo para todo el periodo que estamos buscando salvo el día a validar
                                            Dim strSQL As String = "@SELECT# ISNULL(SUM(Value),0) FROM DailyCauses dc " &
                                                                        " INNER JOIN DailySchedule ds on ds.Date = dc.Date and ds.IDEmployee = dc.IDEmployee" &
                                                                    " WHERE ds.IDEmployee = " & idEmployee &
                                                                        " AND dc.IDCause = " & oIDCause &
                                                                        " AND dc.Date BETWEEN " & roTypes.Any2Time(queryBeginLimit).SQLSmallDateTime & " AND " & roTypes.Any2Time(queryEndLimit).SQLSmallDateTime &
                                                                        " AND dc.DATE <> " & roTypes.Any2Time(valDate).SQLSmallDateTime & " AND ds.Status >= 70"

                                            Dim causeValue As Decimal = roTypes.Any2Double(ExecuteScalar(strSQL))

                                            If oLimit.CauseLimitValue.MaximumMonthlyType = LabAgreeValueType.DirectValue Then
                                                'Si el valor esta indicado directamente en el máximo anual tenemos que comprobar el valor del periodo + nuevo valor del día con el campo directo
                                                If (causeValue + valValue(curIndex)) > oLimit.CauseLimitValue.MaximumMonthlyValue Then
                                                    bolRet = False
                                                End If
                                            Else
                                                Dim oUserFieldState As New roUserFieldState(oState.IDPassport)
                                                'Si el valor esta indicado en un campo de la ficha en el máximo anual tenemos que comprobar el valor del periodo + nuevo valor del día con el campo de la ficha a fecha final de contrato
                                                Dim rFieldValue As roEmployeeUserField = roEmployeeUserField.GetEmployeeUserFieldValueAtDate(roTypes.Any2String(idEmployee),
                                                                                            oLimit.CauseLimitValue.MaximumMonthlyField.FieldName, oContract.EndDate, oUserFieldState, False)
                                                If rFieldValue.FieldValue IsNot Nothing Then
                                                    Dim maxValue As Double = 0
                                                    If rFieldValue.Definition.FieldType = FieldTypes.tDecimal Or rFieldValue.Definition.FieldType = FieldTypes.tNumeric Then
                                                        maxValue = roTypes.Any2Double(rFieldValue.FieldValue)
                                                    ElseIf rFieldValue.Definition.FieldType = FieldTypes.tTime Then
                                                        maxValue = roConversions.ConvertTimeToHours(roTypes.Any2String(rFieldValue.FieldValue))
                                                    End If

                                                    If (causeValue + valValue(curIndex)) > maxValue Then
                                                        bolRet = False
                                                    End If
                                                End If
                                            End If
                                        End If

                                        'Si hemos encontrado el límite no hace falta que sigamos mirando el resto de limites
                                        Exit For
                                    End If

                                Next
                                curIndex = curIndex + 1

                                'Si ha habido algun error no seguimos validando se retorna el mensaje de error directamente
                                If Not bolRet Then
                                    Exit For
                                End If
                            Next
                        Else
                            bolRet = True
                        End If
                    Else
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgree::ValidateLabAgreeDailyCausesOnDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgree::ValidateLabAgreeDailyCausesOnDate")
            End Try

            Return bolRet
        End Function

#End Region

    End Class

End Namespace