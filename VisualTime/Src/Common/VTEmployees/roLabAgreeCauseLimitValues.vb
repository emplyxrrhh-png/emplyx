Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace LabAgree

#Region "roLabAgreeAccrualRule"

    <DataContract()>
    <Serializable>
    Public Class roLabAgreeCauseLimitValues

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intIDLabAgree As Integer
        Private intIDCauseLimitValue As Integer
        Private dtBeginDate As DateTime
        Private dtEndDate As DateTime
        Private oCauseLimitValue As roCauseLimitValue

        Public Sub New()
            Me.oState = New roLabAgreeState
        End Sub

        Public Sub New(ByVal _IDLabAgree As Integer, ByVal _IDCauseLimitValue As Integer, ByVal _State As roLabAgreeState,
                        Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intIDLabAgree = _IDLabAgree
            Me.intIDCauseLimitValue = _IDCauseLimitValue
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
        Public Property IDLabAgree() As Integer
            Get
                Return Me.intIDLabAgree
            End Get
            Set(ByVal value As Integer)
                Me.intIDLabAgree = value
            End Set
        End Property

        <DataMember()>
        Public Property IDCauseLimitValue() As Integer
            Get
                Return Me.intIDCauseLimitValue
            End Get
            Set(ByVal value As Integer)
                Me.intIDCauseLimitValue = value
                'If value > 0 Then oCauseLimitValue = New roCauseLimitValue(value, oState)
            End Set
        End Property

        <DataMember()>
        Public Property BeginDate() As DateTime
            Get
                Return Me.dtBeginDate
            End Get
            Set(ByVal value As DateTime)
                dtBeginDate = value
            End Set
        End Property

        <DataMember()>
        Public Property EndDate() As DateTime
            Get
                Return Me.dtEndDate
            End Get
            Set(ByVal value As DateTime)
                Me.dtEndDate = value
            End Set
        End Property

        <DataMember()>
        Public Property CauseLimitValue() As roCauseLimitValue
            Get
                Return Me.oCauseLimitValue
            End Get
            Set(ByVal value As roCauseLimitValue)
                Me.oCauseLimitValue = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeCauseLimitValues " &
                                       "WHERE IDLabAgree = " & Me.intIDLabAgree.ToString & " And IDCauseLimitValue = " & Me.intIDCauseLimitValue.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    dtBeginDate = oRow("BeginDate")
                    dtEndDate = oRow("EndDate")

                    oCauseLimitValue = New roCauseLimitValue(Me.IDCauseLimitValue, oState, False)

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLabAgreeCauseLimitValue, oCauseLimitValue.Name, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                If IsNothing(Me.oCauseLimitValue) Then
                    oState.Result = LabAgreeResultEnum.CauseLimitValueIDCauseEmpty
                    bolRet = False
                End If

                ' El nombre no puede estar en blanco
                If oCauseLimitValue.Name = "" Then
                    oState.Result = LabAgreeResultEnum.CauseLimitValueNameInvalid
                    bolRet = False
                End If

                If bolRet And oCauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.UserField Then
                    If oCauseLimitValue.MaximumAnnualField Is Nothing OrElse oCauseLimitValue.MaximumAnnualField.FieldName = "" Then
                        oState.Result = LabAgreeResultEnum.CauseLimitValueMaximumAnnualUserFieldEmpty
                        bolRet = False
                    End If
                End If

                If bolRet And oCauseLimitValue.MaximumMonthlyType = LabAgreeValueType.UserField Then
                    If oCauseLimitValue.MaximumMonthlyField Is Nothing OrElse oCauseLimitValue.MaximumMonthlyField.FieldName = "" Then
                        oState.Result = LabAgreeResultEnum.CauseLimitValueMaximumMonthlyUserFieldEmpty
                        bolRet = False
                    End If
                End If

                If bolRet And oCauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.None And oCauseLimitValue.MaximumMonthlyType = LabAgreeValueType.None Then
                    oState.Result = LabAgreeResultEnum.CauseLimitValueNoUserFieldsSelected
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Validamos la definicion del limite y el periodo indicado
                If Me.Validate() Then
                    Dim oOldCauseLimitValue As roCauseLimitValue = Nothing

                    ' Si no es nuevo, nos guardamos la definicion antigua del limite
                    If Me.oCauseLimitValue.IDCauseLimitValue > 0 Then
                        oOldCauseLimitValue = New roCauseLimitValue(Me.oCauseLimitValue.IDCauseLimitValue, Me.oState, False)
                    End If

                    ' Guardamos la definicio del limite por justificacion
                    bolRet = Me.oCauseLimitValue.Save(Me.intIDLabAgree, True)
                    If Not bolRet Then
                        Return bolRet
                        Exit Function
                    End If

                    Me.intIDCauseLimitValue = oCauseLimitValue.IDCauseLimitValue

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim oOldLabAgreeCauseLimitValue As roLabAgreeCauseLimitValues = Nothing

                    Dim tb As New DataTable("LabAgreeCauseLimitValues")
                    Dim strSQL As String = "@SELECT# * FROM LabAgreeCauseLimitValues " &
                                           "WHERE IDLabAgree = " & Me.intIDLabAgree.ToString & " AND IDCauseLimitValue = " & Me.intIDCauseLimitValue.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDLabAgree") = Me.intIDLabAgree
                        oRow("IDCauseLimitValue") = Me.intIDCauseLimitValue
                    Else
                        oOldLabAgreeCauseLimitValue = New roLabAgreeCauseLimitValues(Me.intIDLabAgree, Me.intIDCauseLimitValue, Me.oState)
                        If Not oOldCauseLimitValue Is Nothing Then
                            oOldLabAgreeCauseLimitValue.CauseLimitValue = oOldCauseLimitValue
                        End If
                        oRow = tb.Rows(0)
                    End If

                    oRow("BeginDate") = dtBeginDate
                    oRow("EndDate") = dtEndDate

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = Me.Recalculate(oOldLabAgreeCauseLimitValue, , , False)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Save")
            Finally

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
        ''' Borra el limite de justificacion
        ''' </summary>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If Not Me.IsUsed() Then

                    bolRet = False

                    Dim DelQuerys() As String = {"@DELETE# FROM LabAgreeCauseLimitValues WHERE IDLabAgree = " & Me.intIDLabAgree.ToString & " AND IDCauseLimitValue  = " & Me.intIDCauseLimitValue.ToString,
                                                 "@DELETE# FROM CauseLimitValues WHERE IDCauseLimitValue = " & Me.intIDCauseLimitValue.ToString}
                    For n As Integer = 0 To DelQuerys.Length - 1
                        If Not ExecuteSql(DelQuerys(n)) Then
                            oState.Result = LabAgreeResultEnum.ConnectionError
                            Exit For
                        End If
                    Next

                    bolRet = (oState.Result = LabAgreeResultEnum.NoError)

                    If bolRet Then
                        ' Notificamos el cambio al servidor
                        bolRet = Me.Recalculate(Nothing, , , False)
                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tLabAgreeCauseLimitValue, "", Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Delete")
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
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

        ''' <summary>
        ''' Actualiza el estado de la 'DailySchedule' de los empleados relacionados a través del convenio y notifica el proceso de recálculo CONCEPTS al servidor.
        ''' </summary>
        ''' <param name="oOldLabAgreeCauseLimitValue">Configuración anterior del valor inicial. Necesario para determinar que valores han cambiado. Si es Nothing, se considera que se tiene que recalcular.</param>
        ''' <param name="_IDEmployee">Opcional. Si se indica, solo actualiza el estado del empleado indicado y notifica el proceso de recálculo DAILYCAUSES al servidor.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Recalculate(ByVal oOldLabAgreeCauseLimitValue As roLabAgreeCauseLimitValues, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifyDate As Date = Nothing, Optional ByVal bolRunTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim bolMustRecalcLimit As Boolean = (oOldLabAgreeCauseLimitValue Is Nothing)

                If Not bolMustRecalcLimit Then
                    bolMustRecalcLimit = (oOldLabAgreeCauseLimitValue.BeginDate <> Me.BeginDate) Or (oOldLabAgreeCauseLimitValue.EndDate <> Me.EndDate)
                End If

                If Not bolMustRecalcLimit Then
                    bolMustRecalcLimit = (oOldLabAgreeCauseLimitValue.oCauseLimitValue.IDCause <> Me.oCauseLimitValue.IDCause)
                End If

                If Not bolMustRecalcLimit Then
                    bolMustRecalcLimit = (oOldLabAgreeCauseLimitValue.oCauseLimitValue.IDExcessCause <> Me.oCauseLimitValue.IDExcessCause)
                End If

                Dim strOldValue As String = ""
                Dim strValue As String = ""

                If Not bolMustRecalcLimit Then
                    strOldValue = ""
                    strValue = ""
                    If oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.UserField AndAlso
                       oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumAnnualField IsNot Nothing Then strOldValue = oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumAnnualField.FieldName
                    If Me.oCauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.UserField AndAlso
                       Me.oCauseLimitValue.MaximumAnnualField IsNot Nothing Then strValue = Me.oCauseLimitValue.MaximumAnnualField.FieldName
                    bolMustRecalcLimit = (strOldValue <> strValue)
                End If

                If Not bolMustRecalcLimit Then
                    strOldValue = ""
                    strValue = ""
                    If oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.DirectValue Then strOldValue = oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumAnnualValue
                    If Me.oCauseLimitValue.MaximumAnnualValueType = LabAgreeValueType.DirectValue Then strValue = Me.oCauseLimitValue.MaximumAnnualValue
                    bolMustRecalcLimit = (strOldValue <> strValue)
                End If

                If Not bolMustRecalcLimit Then
                    strOldValue = ""
                    strValue = ""
                    If oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumMonthlyType = LabAgreeValueType.UserField AndAlso
                       oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumMonthlyField IsNot Nothing Then strOldValue = oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumMonthlyField.FieldName
                    If Me.oCauseLimitValue.MaximumMonthlyType = LabAgreeValueType.UserField AndAlso
                       Me.oCauseLimitValue.MaximumMonthlyField IsNot Nothing Then strValue = Me.oCauseLimitValue.MaximumMonthlyField.FieldName
                    bolMustRecalcLimit = (strOldValue <> strValue)
                End If

                If Not bolMustRecalcLimit Then
                    strOldValue = ""
                    strValue = ""
                    If oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumMonthlyType = LabAgreeValueType.DirectValue Then strOldValue = oOldLabAgreeCauseLimitValue.oCauseLimitValue.MaximumMonthlyValue
                    If Me.oCauseLimitValue.MaximumMonthlyType = LabAgreeValueType.DirectValue Then strValue = Me.oCauseLimitValue.MaximumMonthlyValue
                    bolMustRecalcLimit = (strOldValue <> strValue)
                End If

                If Not bolMustRecalcLimit Then
                    Return bolRet
                    Exit Function
                End If

                ' Obtenemos la fecha de congelación
                Dim xFreezingDate As Date = roParameters.GetFirstDate()

                ' Obtenemos la primera fecha de recálculo
                Dim xFirstUpdateDate As Date = Nothing
                Dim xLastUpdateDate As Date = Nothing

                If oOldLabAgreeCauseLimitValue Is Nothing Then
                    xFirstUpdateDate = Me.BeginDate
                    xLastUpdateDate = Me.EndDate
                    If xFirstUpdateDate <= xFreezingDate Then xFirstUpdateDate = xFreezingDate.AddDays(1)
                    If xLastUpdateDate <= xFreezingDate Then xLastUpdateDate = xFreezingDate.AddDays(1)
                Else
                    xFirstUpdateDate = IIf(oOldLabAgreeCauseLimitValue.BeginDate < Me.BeginDate, oOldLabAgreeCauseLimitValue.BeginDate, Me.BeginDate)
                    If xFirstUpdateDate <= xFreezingDate Then xFirstUpdateDate = xFreezingDate.AddDays(1)

                    xLastUpdateDate = IIf(oOldLabAgreeCauseLimitValue.EndDate > Me.EndDate, oOldLabAgreeCauseLimitValue.EndDate, Me.EndDate)
                    If xLastUpdateDate <= xFreezingDate Then xFirstUpdateDate = xLastUpdateDate.AddDays(1)

                End If

                If xFirstUpdateDate <> Nothing And xLastUpdateDate <> Nothing Then

                    Dim strSQL As String

                    Dim strSQLEmployees As String = "@SELECT# DISTINCT EmployeeContracts.IDEmployee, EmployeeContracts.BeginDate, EmployeeContracts.EndDate " &
                                                    "FROM EmployeeContracts INNER JOIN LabAgree " &
                                                            "ON EmployeeContracts.IDLabAgree = LabAgree.ID " &
                                                    "WHERE LabAgree.ID = " & Me.intIDLabAgree.ToString
                    If _IDEmployee <> -1 Then
                        strSQLEmployees &= " AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                    End If

                    Dim tbEmployees As DataTable = CreateDataTableWithoutTimeouts(strSQLEmployees)
                    If tbEmployees IsNot Nothing Then

                        Dim xStatusDate As Date
                        Dim xEndStatusDate As Date

                        If tbEmployees.Rows.Count > 0 Then

                            ' Recorremos los empleados implicados para actualizar el Status de la tabla DailySchedule a 55 para el periodo del limite

                            For Each oRow As DataRow In tbEmployees.Rows
                                ' Obtenemos la fecha para actualizar el status=55 a DailySchedule
                                xStatusDate = xFirstUpdateDate
                                If xStatusDate < oRow("BeginDate") Then
                                    xStatusDate = oRow("BeginDate")
                                End If

                                xEndStatusDate = xLastUpdateDate
                                If xEndStatusDate > oRow("EndDate") Then
                                    xEndStatusDate = oRow("EndDate")
                                End If

                                ' Actualizamos la tabla DailySchedule
                                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 55, [GUID] = '' " &
                                         "WHERE Status >= 55 AND " &
                                               "IDEmployee = " & oRow("IDEmployee") & " AND Date between " & Any2Time(xStatusDate).SQLSmallDateTime & " AND " & Any2Time(xEndStatusDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "

                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                                If Not bolRet Then Exit For
                            Next

                        ElseIf _ModifyDate <> Nothing And _IDEmployee <> -1 Then

                            ' Obtenemos la fecha para actualizar el status=55 a DailySchedule
                            xStatusDate = xFirstUpdateDate
                            If xStatusDate < _ModifyDate Then
                                xStatusDate = _ModifyDate
                            End If

                            xEndStatusDate = xLastUpdateDate
                            If xEndStatusDate > _ModifyDate Then
                                xEndStatusDate = _ModifyDate
                            End If

                            ' Actualizamos la tabla DailySchedule
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 55, [GUID] = '' " &
                                     "WHERE Status >= 55  AND IDEmployee = " & _IDEmployee & " AND Date between " & Any2Time(xStatusDate).SQLSmallDateTime & " AND " & Any2Time(xEndStatusDate).SQLSmallDateTime & " AND Date > (@SELECT# LockDate from sysrovwEmployeeLockDate where sysrovwEmployeeLockDate.IDEmployee = DailySchedule.IDEmployee)  "

                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)

                        End If

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Recalculate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::Recalculate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function ExecuteToday(ByVal p_rtTaskDate As Date) As Boolean
            '
            ' Devuelve si ese dia se tenia que ejecutar el limite por justificacion
            '

            Dim bRet As Boolean = True

            Try
                ' Primero validamos el período de validez del límite de justificación
                bRet = (p_rtTaskDate <= EndDate AndAlso p_rtTaskDate >= BeginDate)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::ExecuteToday")
                bRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::ExecuteToday")
                bRet = False
            End Try

            Return bRet

        End Function

#Region "Helper methods"

        Public Shared Function GetLabAgreeCauseLimitValues(ByVal _IDLabAgree As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roLabAgreeCauseLimitValues)

            Dim oRet As New Generic.List(Of roLabAgreeCauseLimitValues)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeCauseLimitValues " &
                                       "WHERE IDLabAgree = " & _IDLabAgree.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                For Each oLabAgreeCauseLimitRow As DataRow In tb.Rows
                    oRet.Add(New roLabAgreeCauseLimitValues(oLabAgreeCauseLimitRow("IDLabAgree"), oLabAgreeCauseLimitRow("IDCauseLimitValue"), _State, bAudit))
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeCauseLimitValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeCauseLimitValues")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveLabAgreeCauseLimitValues(ByVal _IDLabAgree As Integer, ByVal oLabAgreeCauseLimitValues As Generic.List(Of roLabAgreeCauseLimitValues), ByVal _State As roLabAgreeState,
                                                        Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos los limites por justificacion actuales
                Dim OldLabAgreeCauseLimitValues As Generic.List(Of roLabAgreeCauseLimitValues) = GetLabAgreeCauseLimitValues(_IDLabAgree, _State, False)

                Dim oOldLabAgreeCauseLimitValue As roLabAgreeCauseLimitValues = Nothing

                Dim IDAccrualRulesSaved As New Generic.List(Of Integer)
                If oLabAgreeCauseLimitValues IsNot Nothing Then
                    bolRet = True
                    For Each oLabAgreeCauseLimit As roLabAgreeCauseLimitValues In oLabAgreeCauseLimitValues
                        oLabAgreeCauseLimit.IDLabAgree = _IDLabAgree
                        roBusinessState.CopyTo(_State, oLabAgreeCauseLimit.oCauseLimitValue.State)
                        bolRet = oLabAgreeCauseLimit.Save(bAudit)
                        If Not bolRet Then Exit For
                        IDAccrualRulesSaved.Add(oLabAgreeCauseLimit.intIDCauseLimitValue)
                    Next
                Else
                    bolRet = True
                End If

                If bolRet Then
                    ' Borramos las composiciones de la tabla que no esten en la lista y las borramos
                    For Each oLabAgreeCauseLimit As roLabAgreeCauseLimitValues In OldLabAgreeCauseLimitValues
                        If ExistCauseLimitValueInList(oLabAgreeCauseLimitValues, oLabAgreeCauseLimit) Is Nothing Then
                            bolRet = oLabAgreeCauseLimit.Delete(True)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::SaveLabAgreeCauseLimitValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::SaveLabAgreeCauseLimitValues")
            Finally

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteLabAgreeCauseLimitValues(ByVal _IDLabAgree As Integer, ByVal _State As roLabAgreeState,
                                                          Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos las reglas del convenio actuales
                Dim OldLabAgreeCaueLimitValues As Generic.List(Of roLabAgreeCauseLimitValues) = GetLabAgreeCauseLimitValues(_IDLabAgree, _State, False)

                bolRet = True
                For Each oCauseLimitValue As roLabAgreeCauseLimitValues In OldLabAgreeCaueLimitValues
                    bolRet = oCauseLimitValue.Delete(bAudit)
                    If Not bolRet Then Exit For
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::DeleteLabAgreeCauseLimitValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::DeleteLabAgreeCauseLimitValues")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateLabAgreeCauseLimitValues(ByVal oLabAgreeCauseLimitValues As Generic.List(Of roLabAgreeCauseLimitValues), ByVal _State As roLabAgreeState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Verificar que
                If oLabAgreeCauseLimitValues.Count > 0 Then
                    For Each oLabAgreeCauseLimitValue As roLabAgreeCauseLimitValues In oLabAgreeCauseLimitValues
                        bolRet = oLabAgreeCauseLimitValue.Validate()
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::ValidateLabAgreeCauseLimitValues")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeCauseLimitValues::ValidateLabAgreeCauseLimitValues")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Retorna un grup de Regles de conveni segons la regla
        ''' </summary>
        ''' <param name="_IdCauseLimitValues">ID de la regla (AccrualsRules)</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLabAgreeCauseLimitValuesUseRule(ByVal _IdCauseLimitValues As Integer, ByVal _State As roLabAgreeState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeCauseLimitValues " &
                                       "WHERE IDCauseLimitValue = " & _IdCauseLimitValues.ToString
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeAccrualRulesUseRule")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeAccrualRulesUseRule")
            Finally

            End Try

            Return oRet

        End Function

        Private Shared Function ExistCauseLimitValueInList(ByVal lstCauseLimitValues As Generic.List(Of roLabAgreeCauseLimitValues), ByVal oCauseLimitVal As roLabAgreeCauseLimitValues) As roLabAgreeCauseLimitValues

            Dim oRet As roLabAgreeCauseLimitValues = Nothing

            If lstCauseLimitValues IsNot Nothing Then

                For Each oItem As roLabAgreeCauseLimitValues In lstCauseLimitValues
                    If oItem.IDCauseLimitValue = oCauseLimitVal.IDCauseLimitValue Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

#End Region

#End Region

    End Class

#End Region

End Namespace