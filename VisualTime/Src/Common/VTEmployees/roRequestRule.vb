Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTNotifications
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace LabAgree

    <DataContract()>
    <Serializable>
    Public Class roRequestRule

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intIDRule As Integer
        Private intIDLabAgree As Integer
        Private intIDRequestType As eRequestType = eRequestType.UserFieldsChange
        Private intIDRuleType As eRequestRuleType = eRequestRuleType.NegativeAccrual
        Private strName As String
        Private strDescription As String
        Private bolActivated As Boolean
        Private oDefinition As roRequestRuleDefinition
        Private lstRuleCondition As Generic.List(Of roUserFieldCondition)

        Public Sub New()
            Me.oState = New roLabAgreeState
            Me.IDRule = -1
            Me.IDLabAgree = -1
            Me.Activated = True
            Me.lstRuleCondition = New Generic.List(Of roUserFieldCondition)

        End Sub

        Public Sub New(ByVal _IDRule As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.IDRule = _IDRule
            Me.lstRuleCondition = New Generic.List(Of roUserFieldCondition)
            Me.Load(bAudit)
        End Sub

#End Region

#Region "Properties"

        ''' <summary>
        ''' Estado de la regla de solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <IgnoreDataMember()>
        Public Property State() As roLabAgreeState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roLabAgreeState)
                Me.oState = value
            End Set
        End Property

        ''' <summary>
        ''' ID de la regla de solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDRule() As Integer
            Get
                Return intIDRule
            End Get
            Set(ByVal value As Integer)
                intIDRule = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo del convenio
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDLabAgree() As Integer
            Get
                Return intIDLabAgree
            End Get
            Set(ByVal value As Integer)
                intIDLabAgree = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de  solicitud
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDRequestType() As eRequestType
            Get
                Return intIDRequestType
            End Get
            Set(ByVal value As eRequestType)
                intIDRequestType = value
            End Set
        End Property

        ''' <summary>
        ''' Tipo de  regla
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property IDRuleType() As eRequestRuleType
            Get
                Return intIDRuleType
            End Get
            Set(ByVal value As eRequestRuleType)
                intIDRuleType = value
            End Set
        End Property

        ''' <summary>
        ''' Nombre descriptivo
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        ''' <summary>
        ''' Descripcion
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Description() As String
            Get
                Return strDescription
            End Get
            Set(ByVal value As String)
                strDescription = value
            End Set
        End Property

        ''' <summary>
        ''' Estado de la regla de solicitud (Activada?)
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Activated() As Boolean
            Get
                Return bolActivated
            End Get
            Set(ByVal value As Boolean)
                bolActivated = value
            End Set
        End Property
        ''' <summary>
        ''' Definicion
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember()>
        Public Property Definition() As roRequestRuleDefinition
            Get
                Return Me.oDefinition
            End Get
            Set(ByVal value As roRequestRuleDefinition)
                Me.oDefinition = value
            End Set
        End Property

        <DataMember()>
        Public Property RuleConditions() As Generic.List(Of roUserFieldCondition)
            Get
                Return Me.lstRuleCondition
            End Get
            Set(ByVal value As Generic.List(Of roUserFieldCondition))
                If Me.lstRuleCondition Is Nothing OrElse value Is Nothing Then
                    Me.lstRuleCondition = New Generic.List(Of roUserFieldCondition)
                Else
                    If Not value Is Nothing Then
                        Me.lstRuleCondition = value
                    End If
                End If
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM RequestsRules WHERE IDRule = " & Me.IDRule.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    intIDLabAgree = Any2Integer(oRow("IDLabAgree"))
                    intIDRequestType = Any2Integer(oRow("IDRequestType"))
                    intIDRuleType = Any2Integer(oRow("IDRuleType"))

                    strName = Any2String(oRow("Name"))
                    strDescription = Any2String(oRow("Description"))

                    If Not IsDBNull(oRow("Definition")) AndAlso Any2String(oRow("Definition")).Length > 0 Then
                        oDefinition = New roRequestRuleDefinition(oState, oRow("Definition"), intIDRequestType, intIDRuleType)
                    End If

                    bolActivated = Any2Boolean(oRow("Activated"))

                    Dim oUserFieldState As New roUserFieldState
                    roBusinessState.CopyTo(Me.oState, oUserFieldState)
                    Me.lstRuleCondition = roUserFieldCondition.LoadFromXml(Any2String(oRow("RuleCriteria")), oUserFieldState, False)

                    bolRet = True

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tRequestRule, Me.strName, tbParameters, -1)
                    End If
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roRequestRule::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestRule::Load")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene el siguiente ID disponible para dar de alta una nueva regla de solicitud
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function GetNextID() As Integer
            Dim intRet As Integer = 0

            Dim strSQL As String = "@SELECT# MAX(IDRule) FROM RequestsRules"
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                intRet = Any2Integer(tb.Rows(0).Item(0))
            End If

            Return intRet + 1

        End Function

        Public Function Validate(Optional ByVal bolCheckNames As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                'Comprobar que el nombre de la regla de solicitud tenga un nombre
                If bolRet AndAlso strName.Trim = String.Empty Then
                    Me.oState.Result = LabAgreeResultEnum.RequestRuleWithoutName
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestRule:: Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestRule::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                Dim oRequestRuleOld As DataRow = Nothing
                Dim oRequestRuleNew As DataRow = Nothing

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Me.Validate() Then

                    Dim oOldRequestRule As roRequestRule = Nothing

                    Dim tb As New DataTable("RequestsRules")
                    Dim strSQL As String = "@SELECT# * FROM RequestsRules WHERE IDRule = " & Me.IDRule
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDRule") = Me.GetNextID()
                        Me.IDRule = oRow("IDRule")
                    Else
                        oOldRequestRule = New roRequestRule(Me.IDRule, Me.State)
                        oRow = tb.Rows(0)
                        oRequestRuleOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("IDLabAgree") = intIDLabAgree
                    oRow("IDRequestType") = intIDRequestType
                    oRow("IDRuleType") = intIDRuleType
                    oRow("Name") = strName
                    oRow("Description") = strDescription

                    If oDefinition IsNot Nothing Then
                        oRow("Definition") = oDefinition.GetXml
                    Else
                        Dim _Definition As New roCollection
                        oRow("Definition") = _Definition.XML
                    End If

                    oRow("Activated") = bolActivated

                    oRow("RuleCriteria") = roUserFieldCondition.GetXml(Me.lstRuleCondition)

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oRequestRuleNew = oRow

                    bolRet = True

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oRequestRuleNew, oRequestRuleOld)
                        Dim oAuditAction As Audit.Action = IIf(oRequestRuleOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oRequestRuleNew("Name")
                        ElseIf oRequestRuleOld("Name") <> oRequestRuleNew("Name") Then
                            strObjectName = oRequestRuleOld("Name") & " -> " & oRequestRuleNew("Name")
                        Else
                            strObjectName = oRequestRuleNew("Name")
                        End If

                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tRequestRule, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestRule::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestRule::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roLabAgreeState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
            If Me.lstRuleCondition Is Nothing Then
                Me.lstRuleCondition = New Generic.List(Of roUserFieldCondition)
            End If

        End Sub

        ''' <summary>
        ''' Borra la regla de solicitud
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' En el caso que la regla sea de tipo Validación automatica,
                ' debemos cambiar el modo de validación de las solicitudes que
                ' esten pendientes y que se habian creado en modo de validación automática
                ' y que apliquen sobre justificaciones concretas de previsiones de ausencia
                If Me.IDRuleType = eRequestRuleType.AutomaticValidation Then
                    Me.CancelAutomaticValidationModeInRequests(Me)
                End If

                ' En el caso que la regla sea de tipo rechazo automatico,
                ' debemos eliminar el valor de la fecha de rechazo   de las solicitudes que
                ' esten pendientes o en curso
                ' y que apliquen sobre justificaciones concretas de previsiones de ausencia
                If Me.IDRuleType = eRequestRuleType.AutomaticRejection Then
                    Me.CancelAutomaticRejectedInRequests(Me)
                End If

                'Borramos la regla de solicitud
                Dim DelQuerys() As String = {"@DELETE# FROM RequestsRules Where IDRule = " & Me.IDRule.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = LabAgreeResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = LabAgreeResultEnum.NoError)

                If bolRet And bAudit Then
                    ' Auditamos
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{RequestRuleName}", Me.strName, "", 1)
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tRequestRule, strName, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestRule::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestRule::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function CancelAutomaticRejectedInRequests(ByVal _RequestRule As roRequestRule) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If _RequestRule Is Nothing OrElse Not _RequestRule.IDRuleType = eRequestRuleType.AutomaticRejection OrElse _RequestRule.Definition.IDReasons Is Nothing OrElse _RequestRule.Definition.IDReasons.Count = 0 Then
                    Return bolRet
                    Exit Function
                End If

                ' Obtenemos todas las solicitudes
                ' que tengan fecha de rechazo, que esten pendientes de validar o en curso
                ' relacionadas con el convenio de la regla
                Dim strSQL As String = "@SELECT# Requests.ID "
                strSQL += " FROM Requests, EmployeeContracts "
                strSQL += " WHERE EmployeeContracts.IDLabAgree =  " & _RequestRule.IDLabAgree.ToString
                strSQL += " AND Requests.IDEmployee = EmployeeContracts.IDEmployee"
                strSQL += " AND Requests.Date1 between EmployeeContracts.BeginDate and EmployeeContracts.EndDate "
                strSQL += " AND RejectedDate is not null "
                strSQL += " AND isnull(Status, 0) in(0,1) "
                strSQL += " AND IDCause in (" & String.Join(",", _RequestRule.Definition.IDReasons.ToArray()) & ")"

                Dim oRequests As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                If oRequests IsNot Nothing AndAlso oRequests.Rows.Count > 0 Then
                    For Each oRow As DataRow In oRequests.Rows
                        ' Desmarcamos la fecha de rechazo automatico
                        ExecuteSqlWithoutTimeOut("@UPDATE# Requests Set RejectedDate=null WHERE ID= " & oRow("ID").ToString)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRequestRule::CancelAutomaticRejectedInRequests")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRequestRule::CancelAutomaticRejectedInRequests")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function CancelAutomaticValidationModeInRequests(ByVal _RequestRule As roRequestRule) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If _RequestRule Is Nothing OrElse Not _RequestRule.IDRuleType = eRequestRuleType.AutomaticValidation OrElse _RequestRule.Definition.IDReasons Is Nothing OrElse _RequestRule.Definition.IDReasons.Count = 0 Then
                    Return bolRet
                    Exit Function
                End If

                ' Obtenemos todas las solicitudes
                ' marcadas como automaticas , que esten pendientes de validar
                ' relacionadas con el convenio de la regla
                Dim strSQL As String = "@SELECT# Requests.ID "
                strSQL += " FROM Requests, EmployeeContracts "
                strSQL += " WHERE EmployeeContracts.IDLabAgree =  " & _RequestRule.IDLabAgree.ToString
                strSQL += " AND Requests.IDEmployee = EmployeeContracts.IDEmployee"
                strSQL += " AND Requests.Date1 between EmployeeContracts.BeginDate and EmployeeContracts.EndDate "
                strSQL += " AND AutomaticValidation = 1 "
                strSQL += " AND isnull(Status, 0) = 0 "
                strSQL += " AND IDCause in (" & String.Join(",", _RequestRule.Definition.IDReasons.ToArray()) & ")"

                Dim oRequests As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                Dim oNotificationState As New Notifications.roNotificationState(Me.State.IDPassport)

                If oRequests IsNot Nothing AndAlso oRequests.Rows.Count > 0 Then
                    For Each oRow As DataRow In oRequests.Rows
                        ' Desmarcamos la validacion automatica
                        ExecuteSqlWithoutTimeOut("@UPDATE# Requests Set AutomaticValidation=0, ValidationDate = Null WHERE ID= " & oRow("ID").ToString)

                        Notifications.roNotification.GenerateNotificationsForRequest(oRow("ID"), True, oNotificationState, False, True)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roRequestRule::CancelAutomaticValidationModeonRequests")
                bolRet = False
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roRequestRule::CancelAutomaticValidationModeonRequests")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        ''' <summary>
        ''' Devuelve una lista  con todos las reglas de solicitud
        ''' </summary>
        ''' <param name="_SQLFilter">Filtro SQL para el Where </param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetRequestsRules(ByVal _SQLFilter As String, ByVal _State As roLabAgreeState,
                                                Optional ByVal bAudit As Boolean = False) As Generic.List(Of roRequestRule)

            Dim oRet As New Generic.List(Of roRequestRule)

            Try

                Dim strSQL As String = "@SELECT# * from RequestsRules "
                If _SQLFilter <> "" Then strSQL &= " Where " & _SQLFilter

                Dim dTbl As DataTable = CreateDataTable(strSQL, )

                If dTbl IsNot Nothing Then
                    For Each dRow As DataRow In dTbl.Rows
                        Dim oRequestsRule As New roRequestRule(dRow("IDRule"), _State)
                        oRet.Add(oRequestsRule)
                    Next
                End If

                If oRet IsNot Nothing AndAlso oRet.Count > 0 Then
                    ' Auditamos consulta masiva
                    If bAudit Then
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Dim strAuditName As String = ""
                        For Each oRequestsRule As roRequestRule In oRet
                            strAuditName &= IIf(strAuditName <> "", ",", "") & oRequestsRule.Name
                        Next
                        Extensions.roAudit.AddParameter(tbAuditParameters, "{RequestRuleNames}", strAuditName, "", 1)
                        _State.Audit(Audit.Action.aMultiSelect, Audit.ObjectType.tRequestRule, strAuditName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetRequestsRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetRequestsRules")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve una lista  de los tipos de regla en funcion de un tipo de solicitud
        ''' </summary>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetAvailableRequestType(ByVal _State As roLabAgreeState) As DataTable

            ' Recupera los tipos de regla en funcion del tipo de solicitud
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# *, '' AS reqType from sysroRequestType where  exists (@SELECT# * from sysroRequestRuleTypes where IDRequestType = sysroRequestType.IdType)"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    For Each oRow As DataRow In oRet.Rows
                        oRow("reqType") = _State.Language.Translate("RequestType." & oRow("Type").ToString, "RequestType")
                    Next
                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetAvailableRequestType")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetAvailableRequestType")
            Finally

            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve una lista  de los tipos de regla en funcion de un tipo de solicitud
        ''' </summary>
        ''' <param name="_IDRequestType">ID tipo de solicitud</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetRuleTypesByRequestType(ByVal _IDRequestType As Integer, ByVal _State As roLabAgreeState) As DataTable

            ' Recupera los tipos de regla en funcion del tipo de solicitud
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = " @SELECT# *, '' as RuleTypeTranslate FROM sysroRuleType WHERE ID IN (@SELECT# DISTINCT IDRuleType FROM  sysroRequestRuleTypes WHERE IDRequestType= " & _IDRequestType.ToString & ")"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    For Each oRow As DataRow In oRet.Rows
                        oRow("RuleTypeTranslate") = _State.Language.Translate("RuleTypes." & oRow("Type").ToString, "RuleTypes") 'TODO
                    Next
                    oRet.AcceptChanges()
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::GetRuleTypesByRequestType")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::GetRuleTypesByRequestType")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveLabAgreeRequestRules(ByVal _IDLabAgree As Integer, ByVal oLabAgreeRequestRules As Generic.List(Of roRequestRule), ByVal _State As roLabAgreeState,
                                                        Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos las reglas del convenio actuales
                Dim OldLabAgreeRequestRules As Generic.List(Of roRequestRule) = GetRequestsRules("IDLabAgree=" & _IDLabAgree.ToString, _State, False)

                Dim IDRequestRulesSaved As New Generic.List(Of Integer)
                If oLabAgreeRequestRules IsNot Nothing Then
                    bolRet = True
                    For Each oLabAgreeReqRule As roRequestRule In oLabAgreeRequestRules
                        ' Guardamos la definicion de la regla de solicitud
                        oLabAgreeReqRule.IDLabAgree = _IDLabAgree
                        bolRet = oLabAgreeReqRule.Save(True)
                        If Not bolRet Then
                            Return bolRet
                            Exit Function
                        End If
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If

                If bolRet Then
                    ' Borramos las reglas de la tabla que no esten en la lista
                    For Each oRequestRule As roRequestRule In OldLabAgreeRequestRules
                        If ExistRequestRuleInList(oLabAgreeRequestRules, oRequestRule) Is Nothing Then
                            bolRet = oRequestRule.Delete(False)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If

                ' Comprobar que no existe duplicados del mismo tipo y motivo
                If bolRet Then
                    ' Obtenemos las reglas del mismo convenio
                    Dim LabAgreeRequestRules As Generic.List(Of roRequestRule) = GetRequestsRules("IDLabAgree=" & _IDLabAgree, _State, False)
                    If LabAgreeRequestRules IsNot Nothing AndAlso LabAgreeRequestRules.Count > 0 Then
                        For Each oReqRule As roRequestRule In LabAgreeRequestRules
                            For Each _reqrule As roRequestRule In oLabAgreeRequestRules
                                If _reqrule.IDRuleType = oReqRule.IDRuleType And _reqrule.IDRequestType = oReqRule.IDRequestType And _reqrule.IDRule <> oReqRule.IDRule Then
                                    If _reqrule.Definition IsNot Nothing AndAlso _reqrule.Definition.IDReasons.Count > 0 AndAlso oReqRule.Definition IsNot Nothing AndAlso oReqRule.Definition.IDReasons.Count > 0 Then
                                        For Each id As Integer In _reqrule.Definition.IDReasons
                                            If oReqRule.Definition.IDReasons.FindAll(Function(o) oReqRule.Definition.IDReasons.Contains(id)).Count > 0 Then
                                                _State.Result = LabAgreeResultEnum.RequestRuleReasonDuplicated
                                                bolRet = False
                                                Exit For
                                            End If
                                        Next
                                        'If _reqrule.Definition.IDReason = oReqRule.Definition.IDReason Then
                                        '    _State.Result = roLabAgreeState.ResultEnum.RequestRuleReasonDuplicated
                                        '    bolRet = False
                                        '    Exit For
                                        'End If
                                    End If
                                End If
                            Next
                        Next
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::SaveLabAgreeRequestRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::SaveLabAgreeRequestRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteLabAgreeRequestRules(ByVal _IDLabAgree As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos las reglas del convenio actuales
                Dim OldLabAgreeRequestRules As Generic.List(Of roRequestRule) = GetRequestsRules("IDLabAgree=" & _IDLabAgree.ToString, _State, False)

                bolRet = True
                For Each oRequestRule As roRequestRule In OldLabAgreeRequestRules
                    bolRet = oRequestRule.Delete(bAudit)
                    If Not bolRet Then Exit For
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roRequestRule::DeleteLabAgreeRequestRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roRequestRule::DeleteLabAgreeRequestRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Private Shared Function ExistRequestRuleInList(ByVal lstRequestRules As Generic.List(Of roRequestRule), ByVal oRequestRule As roRequestRule) As roRequestRule

            Dim oRet As roRequestRule = Nothing

            If lstRequestRules IsNot Nothing Then

                For Each oItem As roRequestRule In lstRequestRules
                    If oItem.IDRule = oRequestRule.IDRule Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

#End Region

    End Class

    <DataContract()>
    <Serializable>
    Public Class roRequestRuleDefinition

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intIDRequestType As eRequestType = eRequestType.UserFieldsChange
        Private intIDRuleType As eRequestRuleType = eRequestRuleType.NegativeAccrual
        Private bolEmployeeValidation As Boolean
        Private intAction As eActionType
        'Private intIDReason As Nullable(Of Integer)
        Private intTypePlannedDay As Nullable(Of eTypePlannedDay)
        Private intMaxDays As Nullable(Of Integer)
        Private intMinDays As Nullable(Of Integer)
        Private xBeginPeriod As Nullable(Of Date)
        Private xEndPeriod As Nullable(Of Date)
        Private lstIDReasons As New Generic.List(Of Integer)
        Private intTypeCriteriaEmployee As Nullable(Of eTypeEmployeeCriteria)
        Private lstIDPlanificationRules As New Generic.List(Of Integer)

        Public Sub New()
            Me.oState = New roLabAgreeState
        End Sub

        Public Sub New(ByVal _State As roLabAgreeState, ByVal strXml As String, ByVal _IDRequestType As eRequestType, ByVal _IDRuleType As eRequestRuleType)
            Me.oState = _State
            intIDRequestType = _IDRequestType
            intIDRuleType = _IDRuleType
            bolEmployeeValidation = False
            intAction = eActionType.Denied_Action
            Me.LoadFromXml(strXml)
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
        Public Property EmployeeValidation() As Boolean
            Get
                Return Me.bolEmployeeValidation
            End Get
            Set(ByVal value As Boolean)
                Me.bolEmployeeValidation = value
            End Set
        End Property

        'Public Property IDReason() As Nullable(Of Integer)
        '    Get
        '        Return Me.intIDReason
        '    End Get
        '    Set(ByVal value As Nullable(Of Integer))
        '        Me.intIDReason = value
        '    End Set
        'End Property

        <DataMember()>
        Public Property TypePlannedDay() As Nullable(Of eTypePlannedDay)
            Get
                Return Me.intTypePlannedDay
            End Get
            Set(ByVal value As Nullable(Of eTypePlannedDay))
                Me.intTypePlannedDay = value
            End Set
        End Property

        <DataMember()>
        Public Property TypeCriteriaEmployee() As Nullable(Of eTypeEmployeeCriteria)
            Get
                Return Me.intTypeCriteriaEmployee
            End Get
            Set(ByVal value As Nullable(Of eTypeEmployeeCriteria))
                Me.intTypeCriteriaEmployee = value
            End Set
        End Property

        <DataMember()>
        Public Property MaxDays() As Nullable(Of Integer)
            Get
                Return Me.intMaxDays
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intMaxDays = value
            End Set
        End Property

        <DataMember()>
        Public Property MinDays() As Nullable(Of Integer)
            Get
                Return Me.intMinDays
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intMinDays = value
            End Set
        End Property

        <DataMember()>
        Public Property BeginPeriod() As Nullable(Of Date)
            Get
                Return Me.xBeginPeriod
            End Get
            Set(ByVal value As Nullable(Of Date))
                Me.xBeginPeriod = value
            End Set
        End Property

        <DataMember()>
        Public Property EndPeriod() As Nullable(Of Date)
            Get
                Return Me.xEndPeriod
            End Get
            Set(ByVal value As Nullable(Of Date))
                Me.xEndPeriod = value
            End Set
        End Property

        <DataMember()>
        Public Property Action() As eActionType
            Get
                Return Me.intAction
            End Get
            Set(ByVal value As eActionType)
                Me.intAction = value
            End Set
        End Property

        <DataMember()>
        Public Property IDReasons() As Generic.List(Of Integer)
            Get
                Return Me.lstIDReasons
            End Get
            Set(ByVal value As Generic.List(Of Integer))
                Me.lstIDReasons = value
            End Set
        End Property


        <DataMember()>
        Public Property IDPlanificationRules() As Generic.List(Of Integer)
            Get
                Return Me.lstIDPlanificationRules
            End Get
            Set(ByVal value As Generic.List(Of Integer))
                Me.lstIDPlanificationRules = value
            End Set
        End Property

#End Region

#Region "Methods"

        <OnDeserializing>
        Private Sub OnDeserialize(pp As StreamingContext)
            If Me.oState Is Nothing Then
                Me.oState = New roLabAgreeState(roTypes.Any2Integer(roConstants.GetGlobalEnvironmentParameter(GlobalAsaxParameter.CurrentIdPassport)))
            End If
            If Me.lstIDReasons Is Nothing Then
                Me.lstIDReasons = New Generic.List(Of Integer)
            End If
            If Me.lstIDPlanificationRules Is Nothing Then
                Me.lstIDPlanificationRules = New Generic.List(Of Integer)
            End If

        End Sub

        Public Function GetXml() As String

            Dim oDefinition As New roCollection

            oDefinition.Add("EmployeeValidation", bolEmployeeValidation)
            'If intIDReason.HasValue Then
            '    oDefinition.Add("IDReason", intIDReason.Value)
            '    If Not lstIDReasons.Contains(intIDReason.Value) Then
            '        lstIDReasons.Add(intIDReason.Value)
            '    End If
            'End If
            If intTypePlannedDay.HasValue Then oDefinition.Add("TypePlannedDay", intTypePlannedDay.Value)

            If intTypeCriteriaEmployee.HasValue Then oDefinition.Add("TypeCriteriaEmployee", intTypeCriteriaEmployee.Value)

            If intMaxDays.HasValue Then oDefinition.Add("MaxDays", intMaxDays.Value)
            If intMinDays.HasValue Then oDefinition.Add("MinDays", intMinDays.Value)
            If xBeginPeriod.HasValue Then oDefinition.Add("BeginPeriod", xBeginPeriod.Value)
            If xEndPeriod.HasValue Then oDefinition.Add("EndPeriod", xEndPeriod.Value)

            If lstIDReasons.Count > 0 Then oDefinition.Add("IDReasons", String.Join(",", lstIDReasons.ToArray))
            If lstIDPlanificationRules.Count > 0 Then oDefinition.Add("IDPlanificationRules", String.Join(",", lstIDPlanificationRules.ToArray))
            oDefinition.Add("Action", intAction)

            Return oDefinition.XML

        End Function

        Private Sub LoadFromXml(ByVal strXml As String)

            If strXml = "" Then
                Exit Sub
            End If

            ' Añadimos la composicion de la definicion
            Dim oDefinition As New roCollection(strXml)
            If oDefinition.Exists("EmployeeValidation") Then bolEmployeeValidation = oDefinition.Item("EmployeeValidation")
            If oDefinition.Exists("IDReason") Then
                'intIDReason = oDefinition.Item("IDReason")
                lstIDReasons.Add(Any2Integer(oDefinition.Item("IDReason")))
            End If
            If oDefinition.Exists("Action") Then intAction = oDefinition.Item("Action")

            If oDefinition.Exists("TypePlannedDay") Then intTypePlannedDay = roTypes.Any2Integer(oDefinition.Item("TypePlannedDay"))

            If oDefinition.Exists("TypeCriteriaEmployee") Then intTypeCriteriaEmployee = roTypes.Any2Integer(oDefinition.Item("TypeCriteriaEmployee"))

            If oDefinition.Exists("MaxDays") Then intMaxDays = roTypes.Any2Integer(oDefinition.Item("MaxDays"))

            If oDefinition.Exists("MinDays") Then intMinDays = roTypes.Any2Integer(oDefinition.Item("MinDays"))

            If oDefinition.Exists("BeginPeriod") Then xBeginPeriod = oDefinition.Item("BeginPeriod")
            If oDefinition.Exists("EndPeriod") Then xEndPeriod = oDefinition.Item("EndPeriod")

            If oDefinition.Exists("IDReasons") Then
                Dim Selection() As String = oDefinition.Item("IDReasons").split(",")
                For i As Integer = 0 To Selection.Count - 1
                    If Not lstIDReasons.Contains(Any2Integer(Selection(i))) Then
                        lstIDReasons.Add(Any2Integer(Selection(i)))
                    End If
                Next
            End If

            If oDefinition.Exists("IDPlanificationRules") Then
                Dim Selection() As String = oDefinition.Item("IDPlanificationRules").split(",")
                For i As Integer = 0 To Selection.Count - 1
                    If Not lstIDPlanificationRules.Contains(Any2Integer(Selection(i))) Then
                        lstIDPlanificationRules.Add(Any2Integer(Selection(i)))
                    End If
                Next
            End If

        End Sub

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                'TODO: Falta validaciones
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roRequestRuleDefinition::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roRequestRuleDefinition::Validate")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace