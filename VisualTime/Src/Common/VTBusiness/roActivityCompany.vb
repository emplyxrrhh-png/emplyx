Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Activity

    <DataContract()>
    Public Class roActivityCompany

#Region "Declarations - Constructor"

        Private oState As roActivityState

        Private intIDActivity As Integer
        Private intIDGroup As Integer
        Private intIDParent As Nullable(Of Integer)
        Private strName As String

        Private lstChilds As Generic.List(Of roActivityCompany)

        Public Sub New()

            Me.oState = New roActivityState(-1)

            Me.intIDActivity = -1
            Me.intIDGroup = -1
            Me.intIDParent = Nothing

            Me.lstChilds = New Generic.List(Of roActivityCompany)

        End Sub

        Public Sub New(ByVal _State As roActivityState)

            Me.oState = _State

            Me.intIDActivity = -1
            Me.intIDGroup = -1
            Me.intIDParent = Nothing

            Me.lstChilds = New Generic.List(Of roActivityCompany)

        End Sub

        Public Sub New(ByVal _IDActivity As Integer, ByVal _IDGroup As Integer, ByVal _Name As String, ByVal _Parent As roActivityCompany, ByVal _State As roActivityState)

            Me.oState = _State

            Me.intIDActivity = _IDActivity
            Me.intIDGroup = _IDGroup
            Me.strName = _Name
            If _Parent IsNot Nothing Then
                Me.intIDParent = _Parent.IDGroup
            Else
                Me.intIDParent = Nothing
            End If

            Me.LoadChilds()

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roActivityState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roActivityState)
                Me.oState = value
                If Me.lstChilds IsNot Nothing Then
                    For Each oChild As roActivityCompany In Me.lstChilds
                        oChild.State = value
                    Next
                End If
            End Set
        End Property
        <DataMember()>
        Public Property IDActivity() As Integer
            Get
                Return Me.intIDActivity
            End Get
            Set(ByVal value As Integer)
                Me.intIDActivity = value
                If Me.lstChilds IsNot Nothing Then
                    For Each oChild As roActivityCompany In Me.lstChilds
                        oChild.IDActivity = Me.intIDActivity
                    Next
                End If
            End Set
        End Property
        <DataMember()>
        Public Property IDGroup() As Integer
            Get
                Return Me.intIDGroup
            End Get
            Set(ByVal value As Integer)
                Me.intIDGroup = value
            End Set
        End Property

        <Xml.Serialization.XmlIgnore()>
        <DataMember()>
        Public Property Parent() As roActivityCompany
            Get
                If Me.intIDParent > 0 Then
                    Dim oParent As New roActivityCompany(Me.oState)
                    oParent.IDActivity = Me.intIDActivity
                    oParent.IDGroup = Me.intIDGroup
                    oParent.LoadChilds()
                    Return oParent
                Else
                    Return Nothing
                End If
            End Get
            Set(ByVal value As roActivityCompany)
                If value IsNot Nothing Then
                    Me.intIDParent = value.IDGroup
                Else
                    Me.intIDParent = -1
                End If
            End Set
        End Property
        <DataMember()>
        Public Property IDParent() As Nullable(Of Integer)
            Get
                Return Me.intIDParent
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDParent = value
            End Set
        End Property
        <DataMember()>
        Public Property Childs() As Generic.List(Of roActivityCompany)
            Get
                Return Me.lstChilds
            End Get
            Set(ByVal value As Generic.List(Of roActivityCompany))
                Me.lstChilds = value
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

#End Region

#Region "Methods"

        Public Function LoadChilds() As Boolean

            Dim bolRet As Boolean = False

            Try

                Me.lstChilds = New Generic.List(Of roActivityCompany)

                Dim strSQL As String = "@SELECT# ActivityCompanies.*, Groups.Name " &
                                       "FROM ActivityCompanies INNER JOIN Groups " &
                                                "ON ActivityCompanies.IDGroup = Groups.ID " &
                                       "WHERE ActivityCompanies.IDActivity = " & Me.intIDActivity.ToString & " AND " &
                                             "ActivityCompanies.IDParent = " & Me.intIDGroup.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    For Each oRow As DataRow In tb.Rows
                        Me.lstChilds.Add(New roActivityCompany(oRow("IDActivity"), oRow("IDGroup"), oRow("Name"), Me, Me.oState))
                    Next

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roActivityCompany::LoadChilds")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityCompany::LoadChilds")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                If Me.intIDActivity <= 0 Then
                    bolRet = False
                    Me.oState.Result = DTOs.ActivityResultEnum.InvalidActivityID
                ElseIf Me.intIDGroup <= 0 Then
                    bolRet = False
                    Me.oState.Result = DTOs.ActivityResultEnum.InvalidGroupID
                Else

                    Dim strSQL As String = "@SELECT# COUNT(*) FROM ActivityCompanies " &
                                           "WHERE IDActivity = " & Me.intIDActivity.ToString & " AND " &
                                                 "IDGroup = " & Me.intIDGroup.ToString
                    If Any2Integer(ExecuteScalar(strSQL)) > 0 Then
                        bolRet = False
                        Me.oState.Result = DTOs.ActivityResultEnum.GroupIDRepited
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityCompany::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityCompany::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                If Me.Validate() Then

                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("ActivityCompanies")
                    Dim strSQL As String = "@SELECT# * FROM ActivityCompanies " &
                                           "WHERE IDActivity = " & Me.intIDActivity.ToString & " AND " &
                                                 "IDGroup = " & Me.intIDGroup.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDActivity") = Me.intIDActivity
                        oRow("IDGroup") = Me.intIDGroup
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    If Me.intIDParent.HasValue Then
                        oRow("IDParent") = Me.intIDParent.Value
                    Else
                        oRow("IDParent") = DBNull.Value
                    End If

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

                    If Me.lstChilds IsNot Nothing Then
                        bolRet = True
                        For Each oChild As roActivityCompany In Me.lstChilds
                            bolRet = oChild.Save()
                            If Not bolRet Then
                                Exit For
                            End If
                        Next
                    Else
                        bolRet = True
                    End If

                    If bolRet And bAudit Then
                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = Extensions.roAudit.CreateParametersTable()
                        Extensions.roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String = ""
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tActivityCompany, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityCompany::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityCompany::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function IsCompanyInActivity(ByVal IdCompany As Integer, ByVal oState As roActivityState) As Boolean

            Dim bolRet As Boolean = False

            Dim Reader As DbDataReader = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 IDActivity FROM ActivityCompanies WHERE IDGroup = @IdCompany"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                AddParameter(cmd, "@IdCompany", System.Data.SqlDbType.Int)
                cmd.Parameters("@IdCompany").Value = IdCompany

                Reader = cmd.ExecuteReader()
                If Reader.HasRows Then
                    bolRet = True
                End If
                Reader.Close()
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivityCompany::IsCompanyInActivity")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivityCompany::IsCompanyInActivity")
            Finally
                If Not Reader.IsClosed Then
                    Reader.Close()
                End If

            End Try

            Return bolRet

        End Function

        Public Shared Function GetActivityCompanies(ByVal _IDActivity As Integer, ByVal _State As roActivityState,
                                                    Optional ByVal bAudit As Boolean = False) As Generic.List(Of roActivityCompany)

            Dim oRet As New Generic.List(Of roActivityCompany)

            Try

                Dim strSQL As String = "@SELECT# ActivityCompanies.*, Groups.Name " &
                                       "FROM ActivityCompanies INNER JOIN Groups " &
                                                "ON ActivityCompanies.IDGroup = Groups.ID " &
                                       "WHERE ActivityCompanies.IDActivity = " & _IDActivity.ToString & " AND " &
                                             "ActivityCompanies.IDParent IS NULL " &
                                       "ORDER BY Groups.Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        oRet.Add(New roActivityCompany(oRow("IDActivity"), oRow("IDGroup"), oRow("Name"), Nothing, _State))

                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roActivityCompany::GetActivityCompanies")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roActivityCompany::GetActivityCompanies")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveActivityCompanies(ByVal _IDActivity As Integer, ByVal oActivityCompanies As Generic.List(Of roActivityCompany), ByVal _State As roActivityState) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String = "@DELETE# FROM ActivityCompanies " &
                                       "WHERE IDActivity = " & _IDActivity.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet Then

                    If oActivityCompanies IsNot Nothing Then
                        bolRet = True
                        For Each oCompany As roActivityCompany In oActivityCompanies
                            bolRet = oCompany.Save()
                            If Not bolRet Then Exit For
                        Next
                    Else
                        bolRet = True
                    End If

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roActivityCompany::SaveActivityCompanies")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roActivityCompany::SaveActivityCompanies")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteActivityCompanies(ByVal _IDActivity As Integer, ByVal _State As roActivityState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM ActivityCompanies WHERE IDActivity = " & _IDActivity.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = _State.Audit(Audit.Action.aDelete, Audit.ObjectType.tActivityCompany, "", Nothing, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roActivityCompany::DeleteActivityCompanies")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roActivityCompany::DeleteActivityCompanies")
            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace