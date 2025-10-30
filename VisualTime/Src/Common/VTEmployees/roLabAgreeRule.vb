Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace LabAgree

#Region "roLabAgreeRule"

    <DataContract()>
    <Serializable>
    Public Class roLabAgreeRule

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intID As Integer
        Private strName As String
        Private strDescription As String
        Private oDefinition As roLabAgreeRuleDefinition
        Private oSchedule As roLabAgreeSchedule

        Public Sub New()
            Me.oState = New roLabAgreeState
            Me.intID = -1
        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intID = _ID
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
            End Set
        End Property

        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember()>
        Public Property Name() As String
            Get
                Return Me.strName
            End Get
            Set(ByVal value As String)
                Me.strName = value
            End Set
        End Property

        <DataMember()>
        Public Property Description() As String
            Get
                Return Me.strDescription
            End Get
            Set(ByVal value As String)
                Me.strDescription = value
            End Set
        End Property

        <DataMember()>
        Public Property Definition() As roLabAgreeRuleDefinition
            Get
                Return Me.oDefinition
            End Get
            Set(ByVal value As roLabAgreeRuleDefinition)
                Me.oDefinition = value
            End Set
        End Property

        <DataMember()>
        Public Property Schedule() As roLabAgreeSchedule
            Get
                Return Me.oSchedule
            End Get
            Set(ByVal value As roLabAgreeSchedule)
                Me.oSchedule = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM AccrualsRules " &
                                       "WHERE IdAccrualsRule = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    strName = Any2String(oRow("Name"))
                    strDescription = Any2String(oRow("Description"))

                    Dim strSchedule As String = Any2String(oRow("Schedule"))
                    If strSchedule <> "" Then
                        oSchedule = New roLabAgreeSchedule(strSchedule, oState)
                    End If

                    Dim strDefinition As String = Any2String(oRow("Definition"))
                    If strDefinition <> "" Then
                        oDefinition = New roLabAgreeRuleDefinition(New roCollection(strDefinition), oState)
                    End If

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(VTBase.Audit.Action.aSelect, VTBase.Audit.ObjectType.tLabAgreeRule, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal IDLabAgree As Integer) As Boolean

            Dim bolRet As Boolean = True
            Dim strDefaultQuery As String = ""

            Try
                ' El nombre no puede estar en blanco
                If Me.Name = "" Then
                    oState.Result = LabAgreeResultEnum.LabAgreeRulesNameCannotBeNull
                    bolRet = False
                    Return bolRet
                End If

                If Me.Definition IsNot Nothing AndAlso Me.Definition.MainAccrual > 0 Then
                    strDefaultQuery = Any2String(ExecuteScalar("@SELECT# DefaultQuery FROM Concepts WHERE [ID] = " & Me.Definition.MainAccrual))
                End If

                ' No hay arrastres para saldos de tipo contrato anualizado
                If strDefaultQuery = "L" Then
                    oState.Result = LabAgreeResultEnum.LabAgreeRulesConceptIncompatibleAction
                    bolRet = False
                    Return bolRet
                End If

                If Me.Schedule IsNot Nothing AndAlso Me.Schedule.ScheduleType = LabAgreeScheduleScheduleType.Annual Then
                    ' Si esta seleccionada la opcion anual
                    'En caso de seleccionar un dia y mes concreto
                    Dim bolFechaCorrecta As Boolean = True

                    If Me.Schedule.Month = 2 Then
                        If Me.Schedule.Day > 29 Then
                            bolFechaCorrecta = False
                        End If
                    Else
                        Try
                            Dim dt As New DateTime(DateTime.Today.Year, Me.Schedule.Month, Me.Schedule.Day)
                        Catch ex As Exception
                            bolFechaCorrecta = False
                        End Try
                    End If

                    If Not bolFechaCorrecta Then
                        oState.Result = LabAgreeResultEnum.IncorrectDate
                        bolRet = False
                        Return bolRet
                    End If
                End If

                If Me.Definition IsNot Nothing AndAlso Me.Definition.MainAccrual > 0 AndAlso Me.Definition.Action = LabAgreeRuleDefinitionAction.Move Then
                    ' En el caso que la regla sea de un saldo por contrato, con caducidad, no se puede realizar la accion de mover
                    ' al ser incompatible con el tipo de saldo que es
                    Dim bApplyExpiredHours As Boolean = Any2Boolean(ExecuteScalar("@SELECT# ApplyExpiredHours FROM Concepts WHERE [ID] = " & Me.Definition.MainAccrual))

                    If strDefaultQuery = "C" AndAlso bApplyExpiredHours Then
                        oState.Result = LabAgreeResultEnum.LabAgreeRulesConceptIncompatibleAction
                        bolRet = False
                        Return bolRet
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal IDLabAgree As Integer, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate(IDLabAgree) Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim oOldLabAgreeRule As roLabAgreeRule = Nothing

                    Dim tb As New DataTable("AccrualsRules")
                    Dim strSQL As String = "@SELECT# * FROM AccrualsRules " &
                                           "WHERE IdAccrualsRule = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        Me.ID = GetNextID()
                        oRow("IdAccrualsRule") = Me.ID
                    Else
                        oOldLabAgreeRule = New roLabAgreeRule(Me.ID, Me.oState, False)
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = strName
                    oRow("Description") = strDescription
                    oRow("Definition") = oDefinition.GetXml
                    oRow("Schedule") = oSchedule.toString

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow
                    bolRet = True

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, VTBase.Audit.Action.aInsert, VTBase.Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = VTBase.Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, VTBase.Audit.ObjectType.tLabAgreeRule, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Save")
            End Try

            Return bolRet

        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roLabAgreeState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
        End Sub

        ''' <summary>
        ''' Borra la regla de convenio
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed() Then

                    Dim DelQuerys() As String = {"@DELETE# FROM AccrualsRules WHERE IDAccrualsRule = " & Me.intID.ToString}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = LabAgreeResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = LabAgreeResultEnum.NoError)

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(VTBase.Audit.Action.aDelete, VTBase.Audit.ObjectType.tLabAgreeRule, Me.strName, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Verifica si la regla de convenio se está usando. En oState.Result establece quien lo está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed() As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                'Dim strSQL As String
                'Dim tb As DataTable
                'Dim strUseConcept As String = ""

                ' Reglas de acumulados
                '' Verifica que el acumulado no se esté usando en ninguna regla de acumulados
                'strSQL = "@SELECT# name, Definition FROM AccrualsRules "
                'tb = CreateDataTable(strSQL, )
                'If tb IsNot Nothing Then
                '    strUseConcept = ""
                '    For Each oRow As DataRow In tb.Rows
                '        If Any2String(oRow("Definition")).Trim <> "" Then
                '            ' Compruebo si el IDConcept esta dentro de la definicion de la regla de acumulados
                '            oCollection = New roCollection(oRow("Definition"))
                '            If Any2Integer(oCollection.Item("MainAccrual")) = Me.ID Then
                '                strUseConcept &= ", " & oRow("Name")
                '            End If
                '        End If
                '    Next
                '    If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                '    If strUseConcept <> "" Then
                '        oState.Result = conceptResultEnum.UsedByAccrualRules
                '        bolIsUsed = True
                '    End If
                'End If

                'Dim strUsedLabAgree As String = ""
                'If Not bolIsUsed Then
                '    ' Convenios - Reglas
                '    ' Verifica que la regla no se esté usando en ningún convenio
                '    strSQL = "@SELECT# LabAgree.Name From LabAGree, LabAgreeAccrualsRules Where LabAgree.ID = LabAgreeAccrualsRules.IdLabAgree And LabAgreeAccrualsRules.IdAccrualsRules = " & Me.intID
                '    tb = CreateDataTable(strSQL, )
                '    If tb IsNot Nothing Then
                '        strUsedLabAgree = ""
                '        For Each oRow As DataRow In tb.Rows
                '            ' Guardo el nombre de todos los empleados que lo usan
                '            strUsedLabAgree &= "," & oRow("Name")
                '        Next
                '        If strUsedLabAgree <> "" Then strUsedLabAgree = strUsedLabAgree.Substring(1)
                '        If strUsedLabAgree <> "" Then
                '            oState.Result = LabAgreeResultEnum.LabAgreeAccrualsRulesUsedInLabAgree
                '            bolIsUsed = True
                '        End If
                '    End If
                'End If

                'If Not bolIsUsed Then
                '    ' Grupos de acumulados
                '    ' Verifica que no existan grupos de acumulados que usen el concepto
                '    strSQL = "@SELECT# SysRoReportGroups.Name " & _
                '             "FROM SysRoReportGroups " & _
                '                    "INNER JOIN SysRoReportGroupConcepts ON " & _
                '                              "SysRoReportGroupConcepts.IDReportGroup = SysRoReportGroups.ID AND " & _
                '                              "SysRoReportGroupConcepts.IDConcept = " & Me.ID.ToString
                '    tb = CreateDataTable(strSQL, )

                '    If tb IsNot Nothing Then
                '        strUseConcept = ""
                '        For Each oRow As DataRow In tb.Rows
                '            ' Guardo el nombre de todos los grupos de acumulados que lo usan
                '            strUseConcept &= ", " & oRow("Name")
                '        Next
                '        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)

                '        If strUseConcept <> "" Then
                '            oState.Result = conceptResultEnum.UsedByReportsGroups
                '            bolIsUsed = True
                '        End If
                '    End If
                'End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRule::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRule::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta un nuevo acumulado
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer
            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(IdAccrualsRule) FROM AccrualsRules"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1
        End Function

        ''' <summary>
        ''' Actualiza el estado de la 'DailySchedule' de los empleados relacionados a través del convenio y notifica el proceso de recálculo CONCEPTS al servidor.
        ''' </summary>
        ''' <param name="oOldLabAgreeRule">Configuración anterior del valor inicial. Necesario para determinar que valores han cambiado. Si es Nothing, se considera que se tiene que recalcular.</param>
        ''' <param name="_IDEmployee">Opcional. Código del empleado a recalcular.</param>
        ''' <param name="_ModifDate">Opcional. Fecha en la que se realiza un cambio. Se utiliza cuando se informa empleado, para indicar que se ha modificado un campo de la ficha en una fecha en concreto.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Recalculate(ByVal oOldLabAgreeRule As roLabAgreeRule, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifDate As Date = Nothing, Optional ByVal bolRunTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                ' Miramos si es necesario recalcular
                Dim bolMustRecalc As Boolean = True
                If oOldLabAgreeRule IsNot Nothing Then
                    Dim strOldDefinition As String = ""
                    Dim strDefinition As String = ""
                    If oOldLabAgreeRule.Definition IsNot Nothing Then strOldDefinition = oOldLabAgreeRule.Definition.GetXml()
                    If Me.Definition IsNot Nothing Then strDefinition = Me.Definition.GetXml()
                    bolMustRecalc = (strOldDefinition <> strDefinition)

                    If Not bolMustRecalc Then
                        Dim strOldSchedule As String = ""
                        Dim strSchedule As String = ""
                        If oOldLabAgreeRule.Schedule IsNot Nothing Then strOldSchedule = oOldLabAgreeRule.Schedule.toString()
                        If Me.Schedule IsNot Nothing Then strSchedule = Me.Schedule.toString()
                        bolMustRecalc = (strOldSchedule <> strSchedule)
                    End If

                End If

                If bolMustRecalc Then

                    ' Obtenemos la fecha de congelación
                    Dim xFreezingDate As New Date(1900, 1, 1)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    If oParameters.Parameter(Parameters.FirstDate) IsNot Nothing AndAlso IsDate(oParameters.Parameter(Parameters.FirstDate)) Then
                        xFreezingDate = CDate(oParameters.Parameter(Parameters.FirstDate))
                    End If

                    Dim strSQL As String

                    ' Buscamos la fecha más antigua por cada empleado, de todos los convenios con esta regla asignados a sus contratos.
                    strSQL = "@SELECT# EmployeeContracts.IDEmployee, MIN(LabAgreeAccrualsRules.BeginDate) AS FirstAccrualRuleDate, MIN(EmployeeContracts.BeginDate) AS FirstContractDate " &
                             "FROM EmployeeContracts INNER JOIN LabAgreeAccrualsRules " &
                                    "ON EmployeeContracts.IDLabAgree = LabAgreeAccrualsRules.IDLabAgree " &
                             "WHERE LabAgreeAccrualsRules.IdAccrualsRules = " & Me.ID.ToString & " "
                    If _IDEmployee <> -1 Then
                        strSQL &= "AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                    End If
                    strSQL &= "GROUP BY EmployeeContracts.IDEmployee"
                    Dim tbEmployees As DataTable = CreateDataTable(strSQL, )

                    If tbEmployees.Rows.Count > 0 Then

                        Dim xFirstDate As Date

                        For Each oEmployeeRow As DataRow In tbEmployees.Rows

                            ' Obtenemos la fecha de inicio del periodo de la regla
                            xFirstDate = oEmployeeRow("FirstAccrualRuleDate")

                            ' Si se informa de fecha de modificación, miramos si es posterior al inicio del periodo de la regla. Nos quedamos con la más grande.
                            If _ModifDate <> Nothing Then
                                If xFirstDate < _ModifDate Then xFirstDate = _ModifDate
                            End If

                            ' Comparamos la fecha obtenida con la del inicio del contrato. Nos quedamos con la más grande.
                            xFirstDate = oEmployeeRow("FirstAccrualRuleDate")
                            If xFirstDate < oEmployeeRow("FirstContractDate") Then xFirstDate = oEmployeeRow("FirstContractDate")
                            ' Miramos si la fecha obtenida está dentro de periodo de congelación.
                            If xFirstDate <= xFreezingDate Then xFirstDate = xFreezingDate.AddDays(1)

                            ' Actualizar el Status de la tabla DailySchedule a 65
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                     "WHERE IDEmployee = " & oEmployeeRow("IDEmployee") & " AND " &
                                           "Date = " & Any2Time(xFirstDate.Date).SQLSmallDateTime & " AND " &
                                           "Status >= 65"
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            If bolRet Then
                                strSQL = "@INSERT# INTO DailySchedule (IDEmployee, Date, Status) " &
                                         "@SELECT# Employees.ID, " & Any2Time(xFirstDate).SQLSmallDateTime & ", 65 " &
                                         "FROM Employees " &
                                         "WHERE Employees.ID = " & oEmployeeRow("IDEmployee") & " AND " &
                                               "Employees.ID NOT IN " &
                                               "(@SELECT# DS.IDEmployee " &
                                                "FROM DailySchedule DS " &
                                                "WHERE Date = " & Any2Time(xFirstDate).SQLSmallDateTime & ")"
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            End If
                            If Not bolRet Then Exit For

                        Next
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Recalculate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRule::Recalculate")
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve un Datatable con todos los reglas de convenios
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLabAgreeRules(ByVal _State As roLabAgreeState) As System.Data.DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * from AccrualsRules Order By Name"

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
                _State.UpdateStateInfo(ex, "roLabAgreeRule::GetLabAgreeRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeRule::GetLabAgreeRules")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

#End Region

#Region "roLabAgreeRuleDefinition"

    <DataContract()>
    <Serializable>
    Public Class roLabAgreeRuleDefinition

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intAction As Integer
        Private intComparation As Integer
        Private intDestiAccrual As Integer
        Private intMainAccrual As Integer

        Private oValueType As LabAgreeRuleDefinitionValueType

        Private intDif As Integer

        Private dblValue As Double
        Private oValueUserField As roUserField
        Private intValueIDConcept As Integer

        Private dblUntil As Double
        Private oUntilUserField As roUserField

        Private oDefinition As roCollection

        Public Sub New()
            Me.oState = New roLabAgreeState
        End Sub

        Public Sub New(ByVal _State As roLabAgreeState)
            Me.oState = _State
        End Sub

        Public Sub New(ByVal xDefinition As roCollection, ByVal _State As roLabAgreeState)

            Me.oState = _State

            If xDefinition IsNot Nothing Then
                If xDefinition.Exists("Action") Then intAction = Any2Integer(xDefinition.Item("Action"))
                If xDefinition.Exists("Comparation") Then intComparation = Any2Integer(xDefinition.Item("Comparation"))
                If xDefinition.Exists("DestiAccrual") Then intDestiAccrual = Any2Integer(xDefinition.Item("DestiAccrual"))
                If xDefinition.Exists("MainAccrual") Then intMainAccrual = Any2Integer(xDefinition.Item("MainAccrual"))

                If xDefinition.Exists("ValueType") Then
                    Me.oValueType = Any2Integer(xDefinition.Item("ValueType"))
                Else
                    Dim intSubAccrual As Integer = 0
                    If xDefinition.Exists("SubAccrual") Then intSubAccrual = Any2Integer(xDefinition.Item("SubAccrual"))
                    If intSubAccrual = 0 Then
                        Me.oValueType = LabAgreeRuleDefinitionValueType.DirectValue
                    ElseIf intSubAccrual > 0 Then
                        Me.oValueType = LabAgreeRuleDefinitionValueType.ConceptValue
                        Me.intValueIDConcept = intSubAccrual
                    End If
                End If
                If xDefinition.Exists("Value") Then dblValue = Any2Double(xDefinition.Item("Value"))
                If xDefinition.Exists("ValueUserField") AndAlso xDefinition.Item("ValueUserField") <> String.Empty Then
                    Dim oUserFieldState As New roUserFieldState()
                    roBusinessState.CopyTo(oState, oUserFieldState)
                    Dim oUserField As New roUserField(oUserFieldState, Any2String(xDefinition.Item("ValueUserField")), Types.EmployeeField, False)
                    If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                        Me.oValueUserField = oUserField
                    End If
                End If
                If xDefinition.Exists("ValueIDConcept") Then Me.intValueIDConcept = Any2Integer(xDefinition.Item("ValueIDConcept"))

                If xDefinition.Exists("Dif") Then intDif = Any2Integer(xDefinition.Item("Dif"))
                If xDefinition.Exists("Until") Then dblUntil = Any2Double(xDefinition.Item("Until"))
                If xDefinition.Exists("UntilUserField") AndAlso xDefinition.Item("UntilUserField") <> String.Empty Then
                    Dim oUserFieldState As New roUserFieldState()
                    roBusinessState.CopyTo(oState, oUserFieldState)
                    Dim oUserField As New roUserField(oUserFieldState, Any2String(xDefinition.Item("UntilUserField")), Types.EmployeeField, False)
                    If oUserFieldState.Result = UserFieldResultEnum.NoError Then
                        Me.oUntilUserField = oUserField
                    End If
                End If

            End If

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
            End Set
        End Property

        <DataMember()>
        Public Property Action() As LabAgreeRuleDefinitionAction
            Get
                Return Me.intAction
            End Get
            Set(ByVal value As LabAgreeRuleDefinitionAction)
                Me.intAction = value
            End Set
        End Property

        <DataMember()>
        Public Property Comparation() As LabAgreeRuleDefinitionComparation
            Get
                Return intComparation
            End Get
            Set(ByVal value As LabAgreeRuleDefinitionComparation)
                Me.intComparation = value
            End Set
        End Property

        <DataMember()>
        Public Property DestiAccrual() As Integer
            Get
                Return intDestiAccrual
            End Get
            Set(ByVal value As Integer)
                Me.intDestiAccrual = value
            End Set
        End Property

        <DataMember()>
        Public Property MainAccrual() As Integer
            Get
                Return intMainAccrual
            End Get
            Set(ByVal value As Integer)
                intMainAccrual = value
            End Set
        End Property

        ''<DataMember()> _
        ''Public Property SubAccrual() As Integer
        ''    Get
        ''        Return intSubAccrual
        ''    End Get
        ''    Set(ByVal value As Integer)
        ''        Me.intSubAccrual = value
        ''    End Set
        ''End Property

        <DataMember()>
        Public Property ValueType() As LabAgreeRuleDefinitionValueType
            Get
                Return Me.oValueType
            End Get
            Set(ByVal value As LabAgreeRuleDefinitionValueType)
                Me.oValueType = value
            End Set
        End Property

        <DataMember()>
        Public Property [Value]() As Double
            Get
                Return Me.dblValue
            End Get
            Set(ByVal value As Double)
                Me.dblValue = value
            End Set
        End Property

        <DataMember()>
        Public Property ValueUserField() As roUserField
            Get
                Return Me.oValueUserField
            End Get
            Set(ByVal value As roUserField)
                Me.oValueUserField = value
            End Set
        End Property

        <DataMember()>
        Public Property ValueIDConcept() As Integer
            Get
                Return Me.intValueIDConcept
            End Get
            Set(ByVal value As Integer)
                Me.intValueIDConcept = value
            End Set
        End Property

        <DataMember()>
        Public Property Dif() As LabAgreeRuleDefinitionDif
            Get
                Return intDif
            End Get
            Set(ByVal value As LabAgreeRuleDefinitionDif)
                Me.intDif = value
            End Set
        End Property

        <DataMember()>
        Public Property Until() As Double
            Get
                Return Me.dblUntil
            End Get
            Set(ByVal value As Double)
                Me.dblUntil = value
            End Set
        End Property

        <DataMember()>
        Public Property UntilUserField() As roUserField
            Get
                Return Me.oUntilUserField
            End Get
            Set(ByVal value As roUserField)
                Me.oUntilUserField = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function GetXml() As String

            Dim oDefinition As New roCollection

            oDefinition.Add("Action", Me.intAction)
            oDefinition.Add("Comparation", Me.intComparation)
            oDefinition.Add("DestiAccrual", Me.intDestiAccrual)
            oDefinition.Add("MainAccrual", Me.intMainAccrual)

            oDefinition.Add("ValueType", Me.oValueType)
            oDefinition.Add("Value", Me.dblValue)
            If Me.oValueUserField IsNot Nothing AndAlso Me.oValueUserField.FieldName IsNot Nothing Then
                oDefinition.Add("ValueUserField", Me.oValueUserField.FieldName)
            Else
                oDefinition.Add("ValueUserField", "")
            End If
            oDefinition.Add("ValueIDConcept", Me.intValueIDConcept)

            ' *****************
            If Me.oValueType = LabAgreeRuleDefinitionValueType.DirectValue Then
                oDefinition.Add("SubAccrual", 0) ' Para mantener compatibilidad con proceso de recálculo actual
            ElseIf Me.oValueType = LabAgreeRuleDefinitionValueType.ConceptValue Then
                oDefinition.Add("SubAccrual", Me.intValueIDConcept) ' Para mantener compatibilidad con proceso de recálculo actual
            End If
            ' *****************

            oDefinition.Add("Dif", Me.intDif)
            oDefinition.Add("Until", Me.dblUntil)
            If Me.oUntilUserField IsNot Nothing AndAlso Me.oUntilUserField.FieldName IsNot Nothing Then
                oDefinition.Add("UntilUserField", Me.oUntilUserField.FieldName)
            Else
                oDefinition.Add("UntilUserField", "")
            End If

            Return oDefinition.XML

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeRuleDefinition::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeRuleDefinition::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function IsEqual(ByVal oCompare As roLabAgreeRuleDefinition) As Boolean

            Dim bolRet As Boolean = False

            If oCompare IsNot Nothing Then
                bolRet = (oCompare.GetXml() = Me.GetXml())
            End If

            Return bolRet

        End Function

#End Region

    End Class

#End Region

#Region "roLabAgreeSchedule"

    <DataContract()>
    <Serializable>
    Public Class roLabAgreeSchedule

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intScheduleType As LabAgreeScheduleScheduleType
        Private intMonthlyType As LabAgreeScheduleMonthlyType

        Private intDays As Integer 'Dies a comptar (cada x dies)
        Private intMonths As Integer 'Mesos a comptar (cada x mesos)

        Private intDay As Integer 'Dia del mes
        Private intWeekDay As Integer 'Dia de la setmana
        Private intMonth As Integer 'Mes
        Private intStart As Integer '1er, 2on, etc.

        Private strSchedule As String

        Public Sub New()
            Me.oState = New roLabAgreeState
            strSchedule = ""
        End Sub

        Public Sub New(ByVal strScheduleStr As String)
            Me.oState = New roLabAgreeState
            strSchedule = strScheduleStr
            Load()
        End Sub

        Public Sub New(ByVal strScheduleStr As String, ByVal _State As roLabAgreeState)
            Me.oState = _State
            strSchedule = strScheduleStr
            Load()
        End Sub

#Region "Properties"

        <DataMember()>
        Public Property ScheduleType() As LabAgreeScheduleScheduleType
            Get
                Return intScheduleType
            End Get
            Set(ByVal value As LabAgreeScheduleScheduleType)
                intScheduleType = value
            End Set
        End Property
        <DataMember()>
        Public Property MonthlyType() As LabAgreeScheduleMonthlyType
            Get
                Return intMonthlyType
            End Get
            Set(ByVal value As LabAgreeScheduleMonthlyType)
                intMonthlyType = value
            End Set
        End Property
        <DataMember()>
        Public Property Days() As Integer
            Get
                Return intDays
            End Get
            Set(ByVal value As Integer)
                intDays = value
            End Set
        End Property
        <DataMember()>
        Public Property Months() As Integer
            Get
                Return intMonths
            End Get
            Set(ByVal value As Integer)
                intMonths = value
            End Set
        End Property
        <DataMember()>
        Public Property Day() As Integer
            Get
                Return intDay
            End Get
            Set(ByVal value As Integer)
                intDay = value
            End Set
        End Property
        <DataMember()>
        Public Property Month() As Integer
            Get
                Return intMonth
            End Get
            Set(ByVal value As Integer)
                intMonth = value
            End Set
        End Property
        <DataMember()>
        Public Property Start() As Integer
            Get
                Return intStart
            End Get
            Set(ByVal value As Integer)
                intStart = value
            End Set
        End Property

        <DataMember()>
        Public Property WeekDay() As LabAgreeScheduleWeekDay
            Get
                Return intWeekDay
            End Get
            Set(ByVal value As LabAgreeScheduleWeekDay)
                intWeekDay = value
            End Set
        End Property

        <IgnoreDataMember()>
        Public Shadows ReadOnly Property toString() As String
            Get
                Return retScheduleString()
            End Get
        End Property

        Private Function retScheduleString() As String
            Dim strRet As String = ""

            Select Case intScheduleType
                Case LabAgreeScheduleScheduleType.Daily ' Diario
                    strRet = "D@"
                    strRet &= StrDup(3 - intDays.ToString.Length, "0") & intDays.ToString
                Case LabAgreeScheduleScheduleType.Weekly ' Semanal
                    strRet = "S@"
                    strRet &= intWeekDay.ToString
                Case LabAgreeScheduleScheduleType.Monthly ' Mensual
                    strRet = "M@"
                    If intMonthlyType = LabAgreeScheduleMonthlyType.DayAndMonth Then ' Dia i mes
                        strRet &= "DM@"
                        strRet &= StrDup(2 - intDay.ToString.Length, "0") & intDay.ToString & "@"
                        strRet &= StrDup(2 - intMonths.ToString.Length, "0") & intMonths.ToString
                    ElseIf intMonthlyType = LabAgreeScheduleMonthlyType.DayAndStartup Then 'Inici, Dia i Mes
                        strRet &= "DS@"
                        strRet &= intStart.ToString & "@" & intWeekDay.ToString & "@" & StrDup(3 - intMonths.ToString.Length, "0") & intMonths.ToString
                    End If
                Case LabAgreeScheduleScheduleType.Annual 'Anual
                    strRet = "A@"
                    strRet &= intDay.ToString & "@" & intMonth.ToString
            End Select
            Return strRet
        End Function

        Private Sub Load()
            If strSchedule = "" Then Exit Sub
            Dim arrStr() As String
            arrStr = strSchedule.Split("@")

            intDay = 0
            intDays = 0
            intMonth = 1
            intMonths = 0
            intWeekDay = 1
            intStart = 0

            If arrStr.Length > 0 Then
                Select Case arrStr(0)
                    Case "D" ' Diari (Dies)
                        intScheduleType = LabAgreeScheduleScheduleType.Daily
                        intDays = CInt(arrStr(1))
                    Case "S" ' Semanal (Dia Semana)
                        intScheduleType = LabAgreeScheduleScheduleType.Weekly
                        intWeekDay = CInt(arrStr(1))
                    Case "M" ' Mensual
                        intScheduleType = LabAgreeScheduleScheduleType.Monthly
                        Select Case arrStr(1)
                            Case "DM" 'Dia / Mes
                                intMonthlyType = LabAgreeScheduleMonthlyType.DayAndMonth
                                intDay = CInt(arrStr(2))
                                intMonths = CInt(arrStr(3))
                            Case "DS" 'Inici / DiaSem / Meses
                                intMonthlyType = LabAgreeScheduleMonthlyType.DayAndStartup
                                intStart = CInt(arrStr(2))
                                intWeekDay = CInt(arrStr(3))
                                intMonths = CInt(arrStr(4))
                        End Select
                    Case "A" ' Anual (dia / mes)
                        intScheduleType = LabAgreeScheduleScheduleType.Annual
                        intDay = CInt(arrStr(1))
                        intMonth = CInt(arrStr(2))
                End Select
            End If
        End Sub

        'TODO: Falta Validate del roLabAgreeSchedule
        Public Sub Validate()
        End Sub

        Public Function IsEqual(ByVal oCompareItem As roLabAgreeSchedule) As Boolean

            Dim bolRet As Boolean = False

            If oCompareItem IsNot Nothing Then
                bolRet = (oCompareItem.toString() = Me.toString())
            End If

            Return bolRet

        End Function

#End Region

    End Class

#End Region

End Namespace