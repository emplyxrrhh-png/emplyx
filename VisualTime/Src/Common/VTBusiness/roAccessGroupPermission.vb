Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace AccessGroup

    <DataContract()>
    Public Class roAccessGroupPermission

#Region "Declarations - Constructor"

        Private oState As roAccessGroupState

        Private intIDAccessGroup As Integer
        Private intIDZone As Integer
        Private intIDAccessPeriod As Integer

        Public Sub New()
            Me.oState = New roAccessGroupState()
            Me.intIDAccessGroup = -1
        End Sub

        Public Sub New(ByVal _IDAccessGroup As Integer, ByVal _IDZone As Integer, ByVal _IDAccessPeriod As Integer, ByVal _State As roAccessGroupState)
            Me.oState = _State
            Me.intIDAccessGroup = _IDAccessGroup
            Me.intIDZone = _IDZone
            Me.intIDAccessPeriod = _IDAccessPeriod
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roAccessGroupState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessGroupState)
                Me.oState = value
            End Set
        End Property

        <DataMember()>
        Public Property IDAccessGroup() As Integer
            Get
                Return Me.intIDAccessGroup
            End Get
            Set(ByVal value As Integer)
                Me.intIDAccessGroup = value
            End Set
        End Property

        <DataMember()>
        Public Property IDZone() As Integer
            Get
                Return Me.intIDZone
            End Get
            Set(ByVal value As Integer)
                Me.intIDZone = value
            End Set
        End Property

        <DataMember()>
        Public Property IDAccessPeriod() As Integer
            Get
                Return Me.intIDAccessPeriod
            End Get
            Set(ByVal value As Integer)
                Me.intIDAccessPeriod = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Try

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("AccessGroupsPermissions")
                Dim strSQL As String = "@SELECT# * FROM AccessGroupsPermissions WHERE IDAccessGroup = " & Me.intIDAccessGroup.ToString & " And IDZone = " & Me.intIDZone & " And IDAccessPeriod = " & Me.intIDAccessPeriod
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("IDAccessGroup") = Me.IDAccessGroup
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                End If

                oRow("IDZone") = Me.intIDZone
                oRow("IDAccessPeriod") = Me.intIDAccessPeriod

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)
                bolRet = True

                If bolRet And bAudit Then
                    oAuditDataNew = oRow

                    bolRet = False
                    ' Auditamos
                    Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                    Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                    Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                    bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tAccessGroupPermission, "", tbAuditParameters, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessGroupPermission::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessGroupPermission::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim DeleteQuerys() As String = {"@DELETE# FROM AccessGroupsPermissions WHERE IDAccessGroup = " & Me.intIDAccessGroup.ToString & " And IDZone = " & Me.intIDZone & " And IDAccessPeriod = " & Me.intIDAccessPeriod}

                For Each strSQL As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then Exit For
                Next

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tAccessGroupPermission, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessGroupPermission::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessGroupPermission::Delete")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetAccessGroupsPermissionsList(ByVal _ID As Integer, ByVal _State As roAccessGroupState,
                                                              Optional ByVal bAudit As Boolean = False) As Generic.List(Of roAccessGroupPermission)

            Dim oRet As New Generic.List(Of roAccessGroupPermission)

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessGroupsPermissions Where IDAccessGroup = " & _ID & " ORDER BY IDzone"

                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    Dim oAccessGroupPermission As roAccessGroupPermission = Nothing

                    For Each oRow As DataRow In tb.Rows
                        oAccessGroupPermission = New roAccessGroupPermission(oRow("IDAccessGroup"), oRow("IDZone"), oRow("IDAccessPeriod"), _State)
                        oRet.Add(oAccessGroupPermission)
                    Next

                End If
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroupPermission::GetAccessGroupsPermissionsList")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroupPermission::GetAccessGroupsPermissionsList")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetAccessGroupsPermissionsDataTable(ByVal _ID As Integer, ByVal _State As roAccessGroupState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM AccessGroupsPermissions Where IDAccessGroup = " & _ID & " ORDER BY IDZone"

                tbRet = CreateDataTable(strSQL, )
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roAccessGroupPermission::GetAccessGroupsPermissionsDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roAccessGroupPermission::GetAccessGroupsPermissionsDataTable")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

End Namespace