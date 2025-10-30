Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Employee

    <DataContract>
    <Serializable>
    Public Class roEmployeeAssignment

#Region "Declarations - Constructors"

        <NonSerialized()>
        Private oState As roEmployeeState

        Private intIDEmployee As Integer
        Private intIDAssignment As Integer

        Private intSuitability As Integer

        Public Sub New()

            Me.oState = New roEmployeeState
            Me.intIDEmployee = -1
            Me.intIDAssignment = -1

        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _IDAssignment As Integer, ByVal _State As roEmployeeState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee
            Me.intIDAssignment = _IDAssignment

            Me.Load()

        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _IDAssignment As Integer, ByVal _Suitability As Integer, ByVal _State As roEmployeeState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee
            Me.intIDAssignment = _IDAssignment
            Me.intSuitability = _Suitability

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roEmployeeState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roEmployeeState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Property IDAssignment() As Integer
            Get
                Return Me.intIDAssignment
            End Get
            Set(ByVal value As Integer)
                Me.intIDAssignment = value
            End Set
        End Property

        <DataMember>
        Public Property Suitability() As Integer
            Get
                Return Me.intSuitability
            End Get
            Set(ByVal value As Integer)
                Me.intSuitability = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM EmployeeAssignments " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                             "IDAssignment = " & Me.intIDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intSuitability = Any2Integer(oRow("Suitability"))

                    ' TODO: auditar EmployeeAssignment
                    ' Auditar lectura
                    ''If _Audit Then
                    ''    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    ''    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDailyCoverageAssignment, Me.AuditObjectName(), tbParameters, -1)
                    ''End If

                    bolRet = True

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function SaveEmployeeAssignments(ByVal _IDEmployee As Integer, ByVal _EmployeeAssignments As Generic.List(Of roEmployeeAssignment), ByVal _State As roEmployeeState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                If ValidateEmployeeAssignments(_IDEmployee, _EmployeeAssignments, _State) Then

                    ' Obtenemos los puestos asignados actualmente
                    Dim lstOldDailyAssignments As Generic.List(Of roEmployeeAssignment) = roEmployeeAssignment.GetEmployeeAssignments(_IDEmployee, _State)

                    ' De los puestos que hay que borrar , primero verificamos que no esten en uso
                    For Each oEmployeeAssignment As roEmployeeAssignment In lstOldDailyAssignments
                        If ExistEmployeeAssignmentInList(_EmployeeAssignments, oEmployeeAssignment) Is Nothing Then
                            If oEmployeeAssignment.IsUsed(_State) Then
                                bolRet = False
                                Exit For
                            End If
                        End If
                    Next

                    If bolRet Then
                        Dim IDAssignmentsSaved As New Generic.List(Of Integer)
                        If _EmployeeAssignments IsNot Nothing AndAlso _EmployeeAssignments.Count > 0 Then
                            For Each oEmployeeAssignment As roEmployeeAssignment In _EmployeeAssignments
                                oEmployeeAssignment.oState = _State
                                oEmployeeAssignment.IDEmployee = _IDEmployee
                                bolRet = oEmployeeAssignment.SaveEmployeeAssignment(bAudit, False)
                                If Not bolRet Then Exit For
                                IDAssignmentsSaved.Add(oEmployeeAssignment.IDAssignment)
                            Next
                        Else
                            bolRet = True
                        End If

                        If bolRet Then
                            ' Borramos los puestos asignados de la tabla que no esten en la lista
                            For Each oEmployeeAssignment As roEmployeeAssignment In lstOldDailyAssignments
                                If ExistEmployeeAssignmentInList(_EmployeeAssignments, oEmployeeAssignment) Is Nothing Then
                                    bolRet = oEmployeeAssignment.Delete(bAudit)
                                    If Not bolRet Then Exit For
                                End If
                            Next
                        End If
                    End If
                Else
                    bolRet = False
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::SaveTeoricDailyCoverageAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roDailyCoverageAssignment::SaveTeoricDailyCoverageAssignments")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function SaveEmployeeAssignment(Optional ByVal _Audit As Boolean = False, Optional ByVal _ExecuteTask As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Dim bolChangeData As Boolean = False

            Try

                If Me.Validate() Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim oOldEmployeeAssignment As roEmployeeAssignment = Nothing

                    Dim tb As New DataTable("EmployeeAssignments")
                    Dim strSQL As String = "@SELECT# * FROM EmployeeAssignments " &
                                           "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                 "IDAssignment = " & Me.intIDAssignment.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDEmployee") = Me.intIDEmployee
                        oRow("IDAssignment") = Me.intIDAssignment
                        bolChangeData = True
                    Else
                        oOldEmployeeAssignment = New roEmployeeAssignment(Me.intIDEmployee, Me.intIDAssignment, Me.oState)
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        If oRow("Suitability") <> Me.intSuitability Then
                            bolChangeData = True
                        End If
                    End If

                    oRow("Suitability") = Me.intSuitability

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    oAuditDataNew = oRow

                    If bolRet And _Audit And bolChangeData Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String

                        Dim strEmployee As String = Any2String(ExecuteScalar("@SELECT# Name FROM Employees where id=" & Me.intIDEmployee.ToString))
                        Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & oAuditDataNew("IDAssignment")))

                        strObjectName = strEmployee & ": " & strAssignment
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tEmployeeAssignment, strObjectName, tbAuditParameters, -1)

                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::SaveEmployeeAssignment")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::SaveEmployeeAssignment")
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                If Me.intSuitability <= 0 Or Me.intSuitability > 100 Then
                    oState.Result = EmployeeResultEnum.AssignmentInvalidSuitability
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::Validate")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateEmployeeAssignments(ByVal _IDEmployee As Integer, ByVal _EmployeeAssignments As Generic.List(Of roEmployeeAssignment), ByVal _State As roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oList As New Generic.List(Of roEmployeeAssignment)

                ' Verificamos que la lista de puestos sea correcta.
                bolRet = True
                For Each oEmployeeAssignment As roEmployeeAssignment In _EmployeeAssignments
                    If ExistEmployeeAssignmentInList(oList, oEmployeeAssignment) IsNot Nothing Then
                        bolRet = False
                        _State.Result = EmployeeResultEnum.AssignmentRepited
                        Exit For
                    Else
                        oList.Add(oEmployeeAssignment)
                    End If
                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeAssignments::ValidateDailyCoverageAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeAssignments::ValidateDailyCoverageAssignments")
            End Try

            Return bolRet

        End Function

        Private Shared Function ExistEmployeeAssignmentInList(ByVal lstEmployeeAssignments As Generic.List(Of roEmployeeAssignment), ByVal oEmployeeAssignment As roEmployeeAssignment) As roEmployeeAssignment

            Dim oRet As roEmployeeAssignment = Nothing

            If lstEmployeeAssignments IsNot Nothing Then

                For Each oItem As roEmployeeAssignment In lstEmployeeAssignments
                    If oItem.IDEmployee = oEmployeeAssignment.IDEmployee And
                       oItem.IDAssignment = oEmployeeAssignment.IDAssignment Then
                        oRet = oItem
                        Exit For
                    End If
                Next

            End If

            Return oRet

        End Function

        ''' <summary>
        ''' Verifica si el puesto se está usando en algun ambito del empleado. En oState.Result establece si se está usando.
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Function IsUsed(ByVal _State As roEmployeeState) As Boolean

            Dim bolIsUsed As Boolean = False

            Try

                Dim strSQL As String
                Dim tb As DataTable
                Dim strUsed As String = ""

                If Not bolIsUsed Then
                    ' Verifica que el puesto no este asignado en la planificacion
                    strSQL = "@SELECT# Employees.Name, DailySchedule.Date From Employees, DailySchedule Where DailySchedule.IDEmployee = Employees.ID And (DailySchedule.IDAssignment = " & Me.intIDAssignment & " OR DailySchedule.OldIDAssignment = " & Me.intIDAssignment & ") AND DailySchedule.IDEmployee=" & Me.intIDEmployee & " Order by  Date"
                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing Then
                        strUsed = ""
                        For Each oRow As DataRow In tb.Rows
                            ' Guardo el nombre y la fecha  de todos los empleados que lo tienen planificado
                            strUsed &= "," & Format(CDate(oRow("Date")), "dd/MM/yyyy")
                        Next
                        If strUsed <> "" Then strUsed = strUsed.Substring(1)
                        If strUsed <> "" Then
                            _State.Language.ClearUserTokens()
                            Dim strNameAssignment = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments WHERE ID=" & Me.intIDAssignment))
                            _State.Language.AddUserToken(strUsed)
                            _State.Language.AddUserToken(strNameAssignment)
                            _State.Result = EmployeeResultEnum.AssignmentUsedOnScheduler
                            _State.Language.ClearUserTokens()

                            bolIsUsed = True
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::IsUsed")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::IsUsed")
            Finally

            End Try

            Return bolIsUsed

        End Function

        Public Function Delete(Optional ByVal _Audit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strEmployee As String = Any2String(ExecuteScalar("@SELECT# Name FROM Employees where id=" & Me.intIDEmployee.ToString))
                Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & Me.intIDAssignment.ToString))

                Dim DelQuerys() As String = {"@DELETE# FROM EmployeeAssignments WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                                             "IDAssignment = " & Me.intIDAssignment.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = EmployeeResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = EmployeeResultEnum.NoError)

                If bolRet And _Audit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployeeAssignment, strEmployee & ": " & strAssignment, Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployeeAssignment::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetEmployeeAssignmentsDataTable(ByVal _IDEmployee As Integer, ByVal _State As roEmployeeState, Optional ByVal _Audit As Boolean = True) As System.Data.DataTable
            ' Recupera la lista de Puestos del empleado en un datatable
            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Assignments.Name, EmployeeAssignments.Suitability, EmployeeAssignments.IDAssignment, EmployeeAssignments.IDEmployee, Assignments.Color, Assignments.ShortName  " &
                                       "FROM EmployeeAssignments INNER JOIN Assignments " &
                                                "ON EmployeeAssignments.IDAssignment = Assignments.ID " &
                                       "WHERE EmployeeAssignments.IDEmployee = " & _IDEmployee.ToString & " " &
                                       "ORDER BY Assignments.Name"

                oRet = CreateDataTable(strSQL, )

                If oRet IsNot Nothing AndAlso oRet.Rows.Count > 0 Then
                    If _Audit Then
                        ' Auditamos
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeAssignment::GetEmployeeAssignmentsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeAssignment::GetEmployeeAssignmentsDataTable")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeAssignments(ByVal _IDEmployee As Integer, ByVal _State As roEmployeeState, Optional ByVal _Audit As Boolean = True) As Generic.List(Of roEmployeeAssignment)

            Dim oRet As New Generic.List(Of roEmployeeAssignment)

            Try

                Dim strSQL As String = "@SELECT# EmployeeAssignments.* " &
                                       "FROM EmployeeAssignments INNER JOIN Assignments " &
                                                "ON EmployeeAssignments.IDAssignment = Assignments.ID " &
                                       "WHERE EmployeeAssignments.IDEmployee = " & _IDEmployee.ToString & " " &
                                       "ORDER BY Assignments.Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roEmployeeAssignment(oRow("IDEmployee"), oRow("IDAssignment"), Any2Integer(oRow("Suitability")), _State))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployeeAssignment::GetEmployeeAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployeeAssignment::GetEmployeeAssignments")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

End Namespace