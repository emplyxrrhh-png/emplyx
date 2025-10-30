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
    Public Class roLabAgreeAccrualRule

#Region "Declarations - Constructor"

        <NonSerialized()>
        Private oState As roLabAgreeState

        Private intIDLabAgree As Integer
        Private intIDAccrualRule As Integer
        Private dtBeginDate As DateTime
        Private dtEndDate As DateTime
        Private oLabAgreeRule As roLabAgreeRule

        Public Sub New()
            Me.oState = New roLabAgreeState
        End Sub

        Public Sub New(ByVal _IDLabAgree As Integer, ByVal _IDAccrualRule As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intIDLabAgree = _IDLabAgree
            Me.intIDAccrualRule = _IDAccrualRule
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
        Public Property IDAccrualRule() As Integer
            Get
                Return Me.intIDAccrualRule
            End Get
            Set(ByVal value As Integer)
                Me.intIDAccrualRule = value
                oLabAgreeRule = New roLabAgreeRule(value, oState)
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
        Public Property LabAgreeRule() As roLabAgreeRule
            Get
                Return Me.oLabAgreeRule
            End Get
            Set(ByVal value As roLabAgreeRule)
                Me.oLabAgreeRule = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeAccrualsRules " &
                                       "WHERE IDLabAgree = " & Me.intIDLabAgree.ToString & " And IdAccrualsRules = " & Me.intIDAccrualRule.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    dtBeginDate = oRow("BeginDate")
                    dtEndDate = oRow("EndDate")

                    oLabAgreeRule = New roLabAgreeRule(Me.IDAccrualRule, oState, False)

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tLabAgreeAccrualRule, oLabAgreeRule.Name, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Load")
            Finally

            End Try

            Return bolRet

        End Function

        'TODO: Falta Validate roLabAgreeAccrualRule
        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                'TODO: Comprobació de dates, etc.
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim oOldLabAgreeAccrualRule As roLabAgreeAccrualRule = Nothing

                    Dim tb As New DataTable("LabAgreeAccrualsRules")
                    Dim strSQL As String = "@SELECT# * FROM LabAgreeAccrualsRules " &
                                           "WHERE IDLabAgree = " & Me.intIDLabAgree.ToString & " AND IdAccrualsRules = " & Me.intIDAccrualRule.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDLabAgree") = Me.intIDLabAgree
                        oRow("IdAccrualsRules") = Me.intIDAccrualRule
                    Else
                        oOldLabAgreeAccrualRule = New roLabAgreeAccrualRule(Me.intIDLabAgree, Me.intIDAccrualRule, Me.oState)
                        oRow = tb.Rows(0)
                    End If

                    oRow("BeginDate") = dtBeginDate
                    oRow("EndDate") = dtEndDate

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Save")
            End Try

            Return bolRet

        End Function

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

                    bolRet = False

                    Dim DelQuerys() As String = {"@DELETE# FROM LabAgreeAccrualsRules WHERE IDLabAgree = " & Me.intIDLabAgree.ToString & " AND IDAccrualsRules  = " & Me.intIDAccrualRule.ToString}
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
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tLabAgreeAccrualRule, "", Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Delete")
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
                'Dim oCollection As roCollection
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

                'If Not bolIsUsed Then
                '    ' Límites anuales
                '    ' Verifica que el acumulado no se esté usando en los límites anuales
                '    strSQL = "@SELECT# idConcept, Employees.Name " & _
                '             "FROM EmployeeConceptAnnualLimits ECAL, Employees " & _
                '             "WHERE ECAL.IDEmployee = Employees.ID AND " & _
                '                   "ECAL.IDConcept = " & Me.ID.ToString & " " & _
                '             "ORDER BY Employees.Name "
                '    tb = CreateDataTable(strSQL, )
                '    If tb IsNot Nothing Then
                '        strUseConcept = ""
                '        For Each oRow As DataRow In tb.Rows
                '            ' Guardo el nombre de todos los empleados que lo usan
                '            strUseConcept &= "," & oRow("Name")
                '        Next
                '        If strUseConcept <> "" Then strUseConcept = strUseConcept.Substring(1)
                '        If strUseConcept <> "" Then
                '            oState.Result = conceptResultEnum.UsedByEmployeeConceptAnnualLimits
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
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

        ''' <summary>
        ''' Actualiza el estado de la 'DailySchedule' de los empleados relacionados a través del convenio y notifica el proceso de recálculo CONCEPTS al servidor.
        ''' </summary>
        ''' <param name="oOldLabAgreeAccrualRule">Configuración anterior del valor inicial. Necesario para determinar que valores han cambiado. Si es Nothing, se considera que se tiene que recalcular.</param>
        ''' <param name="_IDEmployee">Opcional. Si se indica, solo actualiza el estado del empleado indicado y notifica el proceso de recálculo DAILYCAUSES al servidor.</param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function Recalculate(ByVal oOldLabAgreeAccrualRule As roLabAgreeAccrualRule, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _ModifyDate As Date = Nothing, Optional ByVal bolRunTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try

                ' Miramos si es necesario recalcular
                Dim bolMustRecalc As Boolean = True

                If oOldLabAgreeAccrualRule IsNot Nothing Then
                    Dim strOldDefinition As String = ""
                    Dim strDefinition As String = ""
                    If oOldLabAgreeAccrualRule.LabAgreeRule.Definition IsNot Nothing Then strOldDefinition = oOldLabAgreeAccrualRule.LabAgreeRule.Definition.GetXml()
                    If Me.LabAgreeRule.Definition IsNot Nothing Then strDefinition = Me.LabAgreeRule.Definition.GetXml()
                    bolMustRecalc = (strOldDefinition <> strDefinition)

                    If Not bolMustRecalc Then
                        Dim strOldSchedule As String = ""
                        Dim strSchedule As String = ""
                        If oOldLabAgreeAccrualRule.LabAgreeRule.Schedule IsNot Nothing Then strOldSchedule = oOldLabAgreeAccrualRule.LabAgreeRule.Schedule.toString()
                        If Me.LabAgreeRule.Schedule IsNot Nothing Then strSchedule = Me.LabAgreeRule.Schedule.toString()
                        bolMustRecalc = (strOldSchedule <> strSchedule)
                    End If
                    If Not bolMustRecalc Then
                        bolMustRecalc = (oOldLabAgreeAccrualRule.BeginDate <> Me.BeginDate) Or (oOldLabAgreeAccrualRule.EndDate <> Me.EndDate)
                    End If
                End If

                If Not bolMustRecalc Then
                    Return bolRet
                End If

                ' Obtenemos la fecha de congelación
                Dim xFreezingDate = roParameters.GetFirstDate()

                ' Obtenemos la primera fecha de recálculo
                Dim xFirstUpdateDate As Date = Nothing
                If oOldLabAgreeAccrualRule Is Nothing Then
                    xFirstUpdateDate = Me.BeginDate
                    If xFirstUpdateDate <= xFreezingDate Then xFirstUpdateDate = xFreezingDate.AddDays(1)
                Else
                    xFirstUpdateDate = oOldLabAgreeAccrualRule.BeginDate

                    If oOldLabAgreeAccrualRule.BeginDate <> Me.BeginDate Then
                        xFirstUpdateDate = IIf(oOldLabAgreeAccrualRule.BeginDate < Me.BeginDate, oOldLabAgreeAccrualRule.BeginDate, Me.BeginDate)
                    ElseIf oOldLabAgreeAccrualRule.EndDate <> Me.EndDate Then
                        xFirstUpdateDate = IIf(oOldLabAgreeAccrualRule.EndDate < Me.EndDate, oOldLabAgreeAccrualRule.EndDate, Me.EndDate)
                    End If

                    If xFirstUpdateDate <= xFreezingDate Then xFirstUpdateDate = xFreezingDate.AddDays(1)

                End If

                If xFirstUpdateDate <> Nothing Then
                    Dim strSQL As String

                    Dim strSQLEmployees As String = "@SELECT# DISTINCT EmployeeContracts.IDEmployee, EmployeeContracts.BeginDate " &
                                                    "FROM EmployeeContracts WITH(NOLOCK)   INNER JOIN LabAgree WITH(NOLOCK) " &
                                                            "ON EmployeeContracts.IDLabAgree = LabAgree.ID " &
                                                    "WHERE LabAgree.ID = " & Me.intIDLabAgree.ToString
                    If _IDEmployee <> -1 Then
                        strSQLEmployees &= " AND EmployeeContracts.IDEmployee = " & _IDEmployee.ToString
                    End If

                    Dim tbEmployees As DataTable = CreateDataTableWithoutTimeouts(strSQLEmployees)
                    If tbEmployees IsNot Nothing Then

                        Dim xStatusDate As Date

                        If tbEmployees.Rows.Count > 0 Then

                            ' Recorremos los empleados implicados para actualizar el Status de la tabla DailySchedule a 65 para la primera fecha de recálculo

                            For Each oRow As DataRow In tbEmployees.Rows
                                Dim xFirstDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(oRow("IDEmployee"), False, Me.oState)
                                xFirstDateEmployee = xFirstDateEmployee.AddDays(1)

                                ' Obtenemos la fecha para actualizar el status=65 a DailySchedule
                                xStatusDate = xFirstUpdateDate
                                If xStatusDate < xFirstDateEmployee Then xStatusDate = xFirstDateEmployee

                                If xStatusDate < oRow("BeginDate") Then
                                    xStatusDate = oRow("BeginDate")
                                End If

                                ' Actualizamos la tabla DailySchedule
                                strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                         "WHERE Status >= 65 And " &
                                               "IDEmployee = " & oRow("IDEmployee") & " And Date = " & Any2Time(xStatusDate).SQLSmallDateTime

                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                                If bolRet Then
                                    strSQL = "@INSERT# INTO DailySchedule (IDEmployee, Date, Status) " &
                                             "@SELECT# Employees.ID, " & Any2Time(xStatusDate).SQLSmallDateTime & ", 65 " &
                                             "FROM Employees " &
                                             "WHERE Employees.ID = " & oRow("IDEmployee") & " And " &
                                                   "Employees.ID Not IN " &
                                                   "(@SELECT# DS.IDEmployee " &
                                                    "FROM DailySchedule DS " &
                                                    "WHERE Date = " & Any2Time(xStatusDate).SQLSmallDateTime & ")"
                                    bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                                End If

                                If Not bolRet Then Exit For
                            Next

                        ElseIf _ModifyDate <> Nothing AndAlso _IDEmployee <> -1 Then

                            ' Obtenemos la fecha para actualizar el status=65 a DailySchedule
                            Dim xFirstDateEmployee As Date = roBusinessSupport.GetEmployeeLockDatetoApply(_IDEmployee, False, Me.oState)
                            xFirstDateEmployee = xFirstDateEmployee.AddDays(1)

                            xStatusDate = xFirstUpdateDate
                            If xStatusDate < xFirstDateEmployee Then xStatusDate = xFirstDateEmployee

                            If xStatusDate < _ModifyDate Then
                                xStatusDate = _ModifyDate
                            End If

                            ' Actualizamos la tabla DailySchedule
                            strSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET Status = 65, [GUID] = '' " &
                                     "WHERE Status >= 65 And " &
                                           "IDEmployee = " & _IDEmployee & " AND Date = " & Any2Time(xStatusDate).SQLSmallDateTime
                            bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            If bolRet Then
                                strSQL = "@INSERT# INTO DailySchedule (IDEmployee, Date, Status) " &
                                         "@SELECT# Employees.ID, " & Any2Time(xStatusDate).SQLSmallDateTime & ", 65 " &
                                         "FROM Employees " &
                                         "WHERE Employees.ID = " & _IDEmployee & " And " &
                                               "Employees.ID Not IN " &
                                               "(@SELECT# DS.IDEmployee " &
                                                "FROM DailySchedule DS " &
                                                "WHERE Date = " & Any2Time(xStatusDate).SQLSmallDateTime & ")"
                                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                            End If

                        End If

                    End If
                End If

                If Not bolRet AndAlso oState.Result = LabAgreeResultEnum.NoError Then
                    oState.Result = LabAgreeResultEnum.SqlError
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Recalculate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roLabAgreeAccrualRule::Recalculate")
                bolRet = False
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetLabAgreeAccrualRules(ByVal _IDLabAgree As Integer, ByVal _State As roLabAgreeState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roLabAgreeAccrualRule)

            Dim oRet As New Generic.List(Of roLabAgreeAccrualRule)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeAccrualsRules " &
                                       "WHERE IDLabAgree = " & _IDLabAgree.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                For Each oLabAgreeRuleRow As DataRow In tb.Rows
                    oRet.Add(New roLabAgreeAccrualRule(oLabAgreeRuleRow("IDLabAgree"), oLabAgreeRuleRow("IdAccrualsRules"), _State, bAudit))
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeAccrualRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeAccrualRules")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveLabAgreeAccrualRules(ByVal _IDLabAgree As Integer, ByVal oLabAgreeAccrualRules As Generic.List(Of roLabAgreeAccrualRule), ByVal _State As roLabAgreeState,
                                                        Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos las reglas del convenio actuales
                Dim OldLabAgreeAccrualRules As Generic.List(Of roLabAgreeAccrualRule) = GetLabAgreeAccrualRules(_IDLabAgree, _State, False)

                Dim IDAccrualRulesSaved As New Generic.List(Of Integer)
                If oLabAgreeAccrualRules IsNot Nothing Then
                    bolRet = True
                    For Each oLabAgreeAccRule As roLabAgreeAccrualRule In oLabAgreeAccrualRules
                        Dim oOldLabAgreeAccrualRule As roLabAgreeRule = Nothing
                        Dim oOldLabAgreeAccRule As roLabAgreeAccrualRule = Nothing

                        ' Si no es nuevo, nos guardamos la relacion de la regla con el convenio
                        If oLabAgreeAccRule.IDAccrualRule > 0 Then
                            oOldLabAgreeAccRule = New roLabAgreeAccrualRule(_IDLabAgree, oLabAgreeAccRule.IDAccrualRule, _State)
                        End If

                        ' Guardamos la definicion de la regla de convenio
                        roBusinessState.CopyTo(_State, oLabAgreeAccRule.LabAgreeRule.State)
                        bolRet = oLabAgreeAccRule.LabAgreeRule.Save(_IDLabAgree, True)
                        If Not bolRet Then
                            roBusinessState.CopyTo(oLabAgreeAccRule.LabAgreeRule.State, _State)
                            Return bolRet
                            Exit Function
                        End If

                        Dim intIDAccrualRule As Integer = oLabAgreeAccRule.LabAgreeRule.ID

                        oLabAgreeAccRule.IDLabAgree = _IDLabAgree
                        If oLabAgreeAccRule.intIDAccrualRule <= 0 Then
                            oLabAgreeAccRule.intIDAccrualRule = intIDAccrualRule
                        End If
                        ' Guardamos la relacion de la regla con el convenio
                        bolRet = oLabAgreeAccRule.Save(bAudit)
                        If Not bolRet Then Exit For

                        ' Recalculamos
                        bolRet = oLabAgreeAccRule.Recalculate(oOldLabAgreeAccRule, , , False)
                        If Not bolRet Then Exit For

                    Next
                Else
                    bolRet = True
                End If

                If bolRet Then
                    ' Borramos las composiciones de la tabla que no esten en la lista y las borramos
                    For Each oAccrualRule As roLabAgreeAccrualRule In OldLabAgreeAccrualRules
                        If ExistAccrualRuleInList(oLabAgreeAccrualRules, oAccrualRule) Is Nothing Then
                            bolRet = oAccrualRule.Delete(False)
                            If Not bolRet Then Exit For
                            bolRet = oAccrualRule.LabAgreeRule.Delete(True)
                            If Not bolRet Then Exit For
                        End If
                    Next
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::SaveLabAgreeAccrualRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::SaveLabAgreeAccrualRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteLabAgreeAccrualRules(ByVal _IDLabAgree As Integer, ByVal _State As roLabAgreeState,
                                                          Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Obtenemos las reglas del convenio actuales
                Dim OldLabAgreeAccrualRules As Generic.List(Of roLabAgreeAccrualRule) = GetLabAgreeAccrualRules(_IDLabAgree, _State, False)

                bolRet = True
                For Each oAccrualRule As roLabAgreeAccrualRule In OldLabAgreeAccrualRules
                    bolRet = oAccrualRule.Delete(bAudit)
                    If Not bolRet Then Exit For

                    bolRet = oAccrualRule.oLabAgreeRule.Delete(bAudit)
                    If Not bolRet Then Exit For
                Next

                If bolRet Then
                    Dim strSQL As String = "@DELETE# FROM LabAgreeAccrualsRules WHERE IDLabAgree = " & _IDLabAgree
                    bolRet = ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::DeleteLabAgreeAccrualRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::DeleteLabAgreeAccrualRules")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateLabAgreeAccrualRules(ByVal oLabAgreeAccrualRules As Generic.List(Of roLabAgreeAccrualRule), ByVal _State As roLabAgreeState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Verificar que
                If oLabAgreeAccrualRules.Count > 0 Then
                    For Each oLabAgreeAccRule As roLabAgreeAccrualRule In oLabAgreeAccrualRules
                        bolRet = oLabAgreeAccRule.Validate()
                        If Not bolRet Then Exit For
                    Next
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::ValidateLabAgreeAccrualRules")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::ValidateLabAgreeAccrualRules")
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
        ''' Retorna un grup de Regles de conveni segons la regla
        ''' </summary>
        ''' <param name="_IdAccrualsRules">ID de la regla (AccrualsRules)</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetLabAgreeAccrualRulesUseRule(ByVal _IdAccrualsRules As Integer, ByVal _State As roLabAgreeState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM LabAgreeAccrualsRules " &
                                       "WHERE IDAccrualsRules = " & _IdAccrualsRules.ToString
                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeAccrualRulesUseRule")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roLabAgreeAccrualRule::GetLabAgreeAccrualRulesUseRule")
            Finally

            End Try

            Return oRet

        End Function

        Private Shared Function ExistAccrualRuleInList(ByVal lstAccrualRules As Generic.List(Of roLabAgreeAccrualRule), ByVal oAccrualRule As roLabAgreeAccrualRule) As roLabAgreeAccrualRule

            Dim oRet As roLabAgreeAccrualRule = Nothing

            If lstAccrualRules IsNot Nothing Then

                For Each oItem As roLabAgreeAccrualRule In lstAccrualRules
                    If oItem.IDAccrualRule = oAccrualRule.IDAccrualRule Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

        ''' <summary>
        ''' Función usada en motores de cálculo para saber si una determinada regla de arrastre aplica en una fecha
        ''' </summary>
        ''' <param name="dDate"></param>
        ''' <returns></returns>
        Public Function ApplyOnDate(dDate As Date) As Boolean
            Dim bRet As Boolean = False
            Dim iDayOfMonth As Integer
            Dim iMonthDiff As Integer
            Dim dFirstExecDate As Date

            Try

                ' Primero validamos el período de validez de la regla de acumulado
                If Not (dDate <= Me.EndDate AndAlso dDate >= Me.BeginDate) Then
                    Return False
                End If

                If Me.LabAgreeRule.Schedule.toString = "" Then Return False

                Dim oSchedule As New roLabAgreeSchedule
                oSchedule = Me.LabAgreeRule.Schedule

                Select Case oSchedule.ScheduleType
                    Case LabAgreeScheduleScheduleType.Daily
                        If DateDiff("d", Me.BeginDate, dDate) Mod Any2Long(oSchedule.Days) = 0 Then
                            bRet = True
                        End If
                    Case LabAgreeScheduleScheduleType.Weekly
                        If dDate.DayOfWeek = Any2Long(oSchedule.WeekDay) Then
                            bRet = True
                        End If
                    Case LabAgreeScheduleScheduleType.Monthly
                        If oSchedule.MonthlyType = LabAgreeScheduleMonthlyType.DayAndMonth Then
                            ' Primero validamos si es el día en cuestión
                            If Any2Long(oSchedule.Day) <> 32 Then
                                iDayOfMonth = Any2Long(oSchedule.Day)
                            Else
                                iDayOfMonth = getLastDayOfMonth(dDate.Date).Day
                            End If
                            If dDate.Day = iDayOfMonth Then
                                ' Luego validamos que han pasado los mese indicados

                                ' Primero miramos cual fue el primer mes que se ejecutó,
                                ' en función de la fecha de inicio de validez de la regla
                                If Me.BeginDate.Day >= dDate.Day Then
                                    ' 1. Si el día de aplicación de la regla es anterior al de inicio de vigencia,
                                    ' el primer mes ejecución será el siguiente al de inicio de validez de la regla
                                    dFirstExecDate = Me.BeginDate.AddMonths(1)
                                Else
                                    ' 2. Si el día de aplicación de la regla es posterior al de inicio de vigencia,
                                    ' el primer mes de ejecución de la regla es el mismo de la fecha de inicio de vigencia
                                    dFirstExecDate = Me.BeginDate
                                End If

                                'Calculo la diferencia en meses entre la primera ejecución y la fecha de la tarea
                                iMonthDiff = (dDate.Year - dFirstExecDate.Year) * 12 + dDate.Month - dFirstExecDate.Month

                                If iMonthDiff Mod Any2Long(oSchedule.Months) = 0 Then
                                    ' Toca calcular la regla
                                    bRet = True
                                End If
                            End If
                        Else
                            ' Cada n día de la semana de cada m meses (tercer jueves de cada 3 meses)
                            If oSchedule.MonthlyType = LabAgreeScheduleMonthlyType.DayAndStartup Then

                                ' Calculamos el día del més al que corresponden los datos
                                iDayOfMonth = getNthWeekdayOfMonth(dDate.Date, Any2Long(oSchedule.Start), Any2Long(oSchedule.WeekDay)).Day

                                'Calculo la diferencia en meses entre la primera ejecución y la fecha de la tarea
                                iMonthDiff = (dDate.Year - dFirstExecDate.Year) * 12 + dDate.Month - dFirstExecDate.Month

                                'Finalmente comprobamos si es el día y el mes concreto para lanzar el cálculo de la regla
                                If (dDate.Day = iDayOfMonth) AndAlso iMonthDiff Mod Any2Long(oSchedule.Months) = 0 Then
                                    bRet = True
                                End If
                            End If
                        End If
                    Case LabAgreeScheduleScheduleType.Annual
                        If dDate.Day = Any2Long(oSchedule.Day) AndAlso dDate.Month = Any2Long(oSchedule.Month) Then
                            bRet = True
                        End If
                    Case Else
                        bRet = False
                End Select
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roLabAgreeSchedule::ApplyOnDate", ex)
            End Try

            Return bRet
        End Function

#Region "Helper Fechas"

        Public Function getLastDayOfMonth(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el ultimo dia del mes.
            '
            getLastDayOfMonth = DateAdd("d", -1, getFirstDayOfMonth(DateAdd("m", 1, pdate), 0))

        End Function

        Public Function getFirstDayOfMonth(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '
            getFirstDayOfMonth = "1/" & DatePart("m", pdate) - pDif & "/" & DatePart("yyyy", pdate)

        End Function

        Public Function getFirstDayOfWeek(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '

            getFirstDayOfWeek = DateAdd("d", (-1 * Microsoft.VisualBasic.Weekday(pdate)) + 2 + (pDif * 7), pdate)

        End Function

        Public Function getLastDayOfWeek(ByVal pdate As Date, Optional ByVal pDif As Integer = 0) As Date
            '
            '   Calcula el primer dia del mes
            '
            getLastDayOfWeek = DateAdd("d", 6, getFirstDayOfWeek(pdate, pDif))

        End Function

        Public Function getNthWeekdayOfMonth(ByVal pdate As Date, ByVal iNthWeekDay As Integer, ByVal iWeekDay As Integer) As Date
            '
            '   Calcula el enésimo día de la semana de un mes dado (ejemplo: 3er martes del mes --> iNthWeekDay=3, iWeekDay)
            '

            Dim iWeekdayFirstDayOfMonth As Integer
            Dim tmpDate As Date
            Dim bolBeforeDay As Boolean

            If iNthWeekDay < 5 Then
                'El 1,2,3,4 dia de la semana

                ' ¿Qué día de la semana es el primer día del mes?
                iWeekdayFirstDayOfMonth = Microsoft.VisualBasic.Weekday(getFirstDayOfMonth(pdate), vbMonday)

                ' Calculamos qué día del mes que corresponde
                If iWeekdayFirstDayOfMonth > iWeekDay Then
                    getNthWeekdayOfMonth = DateAdd("d", iWeekDay - iWeekdayFirstDayOfMonth + 7 * iNthWeekDay, getFirstDayOfMonth(pdate))
                Else
                    getNthWeekdayOfMonth = DateAdd("d", iWeekDay - iWeekdayFirstDayOfMonth + 7 * (iNthWeekDay - 1), getFirstDayOfMonth(pdate))
                End If
            Else
                ' El ultimo dia de la semana de un mes

                ' Obtenemos el ultimo dia del mes de la fecha indicada
                tmpDate = getLastDayOfMonth(pdate)

                ' Vamos hacia atrás hasta encontrar el dia de la semana que corresponda al indicado
                bolBeforeDay = True
                While bolBeforeDay
                    If Microsoft.VisualBasic.Weekday(tmpDate, vbMonday) = iWeekDay Then
                        bolBeforeDay = False
                    Else
                        tmpDate = DateAdd("d", -1, tmpDate)
                    End If
                End While
                getNthWeekdayOfMonth = tmpDate
            End If
        End Function

#End Region

#End Region

#End Region

    End Class

#End Region

End Namespace