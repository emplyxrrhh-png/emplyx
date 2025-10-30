Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Shift

    <DataContract()>
    Public Class roShiftAssignment

#Region "Declarations - Constructors"

        Private oState As roShiftState

        Private intIDShift As Integer
        Private intIDAssignment As Integer

        Private dblCoverage As Double

        Public Sub New()

            Me.oState = New roShiftState
            Me.intIDShift = -1
            Me.intIDAssignment = -1

        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _IDAssignment As Integer, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDShift = _IDShift
            Me.intIDAssignment = _IDAssignment

            Me.Load()

        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _IDAssignment As Integer, ByVal _Coverage As Double, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDShift = _IDShift
            Me.intIDAssignment = _IDAssignment
            Me.dblCoverage = _Coverage

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roShiftState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roShiftState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property IDShift() As Integer
            Get
                Return Me.intIDShift
            End Get
            Set(ByVal value As Integer)
                Me.intIDShift = value
            End Set
        End Property
        <DataMember()>
        Public Property IDAssignment() As Integer
            Get
                Return Me.intIDAssignment
            End Get
            Set(ByVal value As Integer)
                Me.intIDAssignment = value
            End Set
        End Property
        <DataMember()>
        Public Property Coverage() As Double
            Get
                Return Me.dblCoverage
            End Get
            Set(ByVal value As Double)
                Me.dblCoverage = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM ShiftAssignments " &
                                       "WHERE IDShift = " & Me.intIDShift.ToString & " AND " &
                                             "IDAssignment = " & Me.intIDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oRow As DataRow = tb.Rows(0)

                    Me.dblCoverage = Any2Double(oRow("Coverage"))

                    ' TODO: auditar ShiftAssignment
                    ' Auditar lectura
                    ''If _Audit Then
                    ''    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    ''    bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tDailyCoverageAssignment, Me.AuditObjectName(), tbParameters, -1)
                    ''End If

                    bolRet = True

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftAssignment::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftAssignment::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                If Me.dblCoverage <= 0 Or Me.dblCoverage > 1 Then
                    oState.Result = ShiftResultEnum.ShiftAssignmentInvalidCoverage
                    bolRet = False
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftAssignment::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftAssignment::Validate")
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal _Audit As Boolean = False, Optional ByVal _Notify As Boolean = True) As Boolean

            Dim bolRet As Boolean = False
            Dim bolChangeData As Boolean = False

            Try
                If Me.Validate() Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing
                    Dim oOldShiftAssignment As roShiftAssignment = Nothing

                    Dim tb As New DataTable("ShiftAssignments")
                    Dim strSQL As String = "@SELECT# * FROM ShiftAssignments " &
                                           "WHERE IDShift = " & Me.intIDShift.ToString & " AND " &
                                                 "IDAssignment = " & Me.intIDAssignment.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDShift") = Me.intIDShift
                        oRow("IDAssignment") = Me.intIDAssignment
                        bolChangeData = True
                    Else
                        oOldShiftAssignment = New roShiftAssignment(Me.intIDShift, Me.intIDAssignment, Me.oState)
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                        If oRow("Coverage") <> Me.dblCoverage Then
                            bolChangeData = True
                        End If

                    End If

                    oRow("Coverage") = Me.dblCoverage

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    If _Notify Then
                        ' Lanzamps los procesos de recálculo
                        ''roDailyCoverage.Recalculate(roDailyCoverage.RecalculateTaskType.Update_All, Me.oState, , , , )
                    End If

                    oAuditDataNew = oRow
                    If bolRet And _Audit And bolChangeData Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String

                        Dim strShift As String = Any2String(ExecuteScalar("@SELECT# Name FROM Shifts where id=" & Me.intIDShift.ToString))
                        Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & oAuditDataNew("IDAssignment")))

                        strObjectName = strShift & ": " & strAssignment
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tShiftAssignment, strObjectName, tbAuditParameters, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftAssignment::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftAssignment::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal _Audit As Boolean = False, Optional ByVal _Notify As Boolean = True) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim strShift As String = Any2String(ExecuteScalar("@SELECT# Name FROM Shifts where id=" & Me.intIDShift.ToString))
                Dim strAssignment As String = Any2String(ExecuteScalar("@SELECT# Name FROM Assignments where id=" & Me.intIDAssignment.ToString))

                Dim DelQuerys() As String = {"@DELETE# FROM ShiftAssignments WHERE IDShift = " & Me.intIDShift.ToString & " AND " &
                                                                             "IDAssignment = " & Me.intIDAssignment.ToString}
                For n As Integer = 0 To DelQuerys.Length - 1
                    If Not ExecuteSql(DelQuerys(n)) Then
                        oState.Result = ShiftResultEnum.ConnectionError
                        Exit For
                    End If
                Next

                bolRet = (oState.Result = ShiftResultEnum.NoError)

                If bolRet And _Notify Then
                    ' Notificamos el cambio al servidor
                    ''bolRet = roDailyCoverage.Recalculate(roDailyCoverage.RecalculateTaskType.Update_All, Me.oState, , , , )
                End If

                If bolRet And _Audit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tShiftAssignment, strShift & ": " & strAssignment, Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftAssignment::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftAssignment::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetShiftAssignments(ByVal _IDShift As Integer, ByVal _State As roShiftState, Optional ByVal _Audit As Boolean = True) As Generic.List(Of roShiftAssignment)

            Dim oRet As New Generic.List(Of roShiftAssignment)

            Try

                Dim strSQL As String = "@SELECT# ShiftAssignments.* " &
                                       "FROM ShiftAssignments INNER JOIN Assignments " &
                                                "ON ShiftAssignments.IDAssignment = Assignments.ID " &
                                       "WHERE ShiftAssignments.IDShift = " & _IDShift.ToString & " " &
                                       "ORDER BY Assignments.Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roShiftAssignment(oRow("IDShift"), oRow("IDAssignment"), Any2Double(oRow("Coverage")), _State))
                    Next

                    If _Audit Then
                        ' ...
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftAssignment::GetShiftAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftAssignment::GetShiftAssignments")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function ExistShiftAssignment(ByVal _IDShift As Integer, ByVal _IDAssignment As Integer, ByVal _State As roShiftState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM ShiftAssignments " &
                                       "WHERE IDShift = " & _IDShift.ToString & " AND " &
                                             "IDAssignment = " & _IDAssignment.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                bolRet = (tb IsNot Nothing AndAlso tb.Rows.Count > 0)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftAssignment::ExistShiftAssignment")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftAssignment::ExistShiftAssignment")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function SaveShiftAssignments(ByVal _IDShift As Integer, ByVal _ShiftAssignments As Generic.List(Of roShiftAssignment), ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                If ValidateShiftAssignments(_IDShift, _ShiftAssignments, _State) Then
                    Dim bHaveToClose As Boolean = Robotics.DataLayer.AccessHelper.StartTransaction()

                    ' Obtenemos los puestos asignados actualmente
                    Dim lstOldShiftAssignments As Generic.List(Of roShiftAssignment) = roShiftAssignment.GetShiftAssignments(_IDShift, _State)

                    Dim IDAssignmentsSaved As New Generic.List(Of Integer)
                    If _ShiftAssignments IsNot Nothing AndAlso _ShiftAssignments.Count > 0 Then
                        For Each oShiftAssignment As roShiftAssignment In _ShiftAssignments
                            oShiftAssignment.oState = _State
                            oShiftAssignment.IDShift = _IDShift
                            bolRet = oShiftAssignment.Save(bAudit, False)
                            If Not bolRet Then Exit For
                            IDAssignmentsSaved.Add(oShiftAssignment.IDAssignment)
                        Next
                    Else
                        bolRet = True
                    End If

                    If bolRet Then
                        ' Borramos los puestos asignados de la tabla que no esten en la lista
                        For Each oShiftAssignment As roShiftAssignment In lstOldShiftAssignments
                            If ExistShiftAssignmentInList(_ShiftAssignments, oShiftAssignment) Is Nothing Then
                                bolRet = oShiftAssignment.Delete(bAudit, False)
                                If Not bolRet Then Exit For
                            End If
                        Next
                    End If

                    Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftAssignment::SaveShiftAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftAssignment::SaveShiftAssignments")
            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateShiftAssignments(ByVal _IDShift As Integer, ByVal _ShiftAssignments As Generic.List(Of roShiftAssignment), ByVal _State As roShiftState) As Boolean
            Dim bolRet As Boolean = False

            Try
                Dim oList As New Generic.List(Of roShiftAssignment)

                ' Verificamos que la lista de puestos sea correcta.
                bolRet = True
                For Each oShiftAssignment As roShiftAssignment In _ShiftAssignments
                    If ExistShiftAssignmentInList(oList, oShiftAssignment) IsNot Nothing Then
                        bolRet = False
                        _State.Result = ShiftResultEnum.ShiftAssignmentRepited
                        Exit For
                    Else
                        oList.Add(oShiftAssignment)
                    End If
                Next

                If bolRet Then

                    Dim strSQL As String = ""

                    ' Verificamos que no se haya eliminado ningún puesto que ya esté asignado al calendario para un empleado
                    Dim lstOldShiftAssignments As Generic.List(Of roShiftAssignment) = roShiftAssignment.GetShiftAssignments(_IDShift, _State, False)
                    For Each oShiftAssignment As roShiftAssignment In lstOldShiftAssignments
                        If ExistShiftAssignmentInList(_ShiftAssignments, oShiftAssignment) Is Nothing Then

                            strSQL = "@SELECT# COUNT(*) FROM dailySchedule WHERE IDShift1 = " & oShiftAssignment.IDShift.ToString & " AND IDAssignment = " & oShiftAssignment.IDAssignment.ToString
                            If Any2Integer(ExecuteScalar(strSQL)) > 0 Then
                                bolRet = False
                                _State.Result = ShiftResultEnum.ShiftAssignmentAssigned
                                Exit For
                            End If

                            ' Verificamos que este asignado a alguna posicion de presupuesto
                            strSQL = "@SELECT# COUNT(*) FROM ProductiveUnit_Mode_Positions WHERE IDShift = " & oShiftAssignment.IDShift.ToString & " AND IDAssignment = " & oShiftAssignment.IDAssignment.ToString
                            If Any2Integer(ExecuteScalar(strSQL)) > 0 Then
                                bolRet = False
                                _State.Result = ShiftResultEnum.ShiftAssignmentAssigned
                                Exit For
                            End If

                            strSQL = "@SELECT# COUNT(*) FROM DailyBudget_Positions WHERE IDShift = " & oShiftAssignment.IDShift.ToString & " AND IDAssignment = " & oShiftAssignment.IDAssignment.ToString
                            If Any2Integer(ExecuteScalar(strSQL)) > 0 Then
                                bolRet = False
                                _State.Result = ShiftResultEnum.ShiftAssignmentAssigned
                                Exit For
                            End If

                        End If
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftAssignment::ValidateShiftAssignments")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftAssignment::ValidateShiftAssignments")
            End Try

            Return bolRet

        End Function

        Private Shared Function ExistShiftAssignmentInList(ByVal lstShiftAssignments As Generic.List(Of roShiftAssignment), ByVal oShiftAssignment As roShiftAssignment) As roShiftAssignment

            Dim oRet As roShiftAssignment = Nothing

            If lstShiftAssignments IsNot Nothing Then

                For Each oItem As roShiftAssignment In lstShiftAssignments
                    If oItem.IDShift = oShiftAssignment.IDShift And
                       oItem.IDAssignment = oShiftAssignment.IDAssignment Then
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

End Namespace