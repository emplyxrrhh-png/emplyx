Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace Activity

    <DataContract()>
    Public Class roActivity

#Region "Declarations - Constructor"

        Private oState As roActivityState

        Private intID As Integer
        Private strName As String = ""
        Private strDescription As String = ""

        Private lstCompanies As Generic.List(Of roActivityCompany)

        Public Sub New()

            Me.oState = New roActivityState(-1)

            Me.intID = -1

        End Sub

        Public Sub New(ByVal _State As roActivityState)

            Me.oState = _State

            Me.intID = -1

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _State As roActivityState, Optional ByVal bAudit As Boolean = False)

            Me.oState = _State

            Me.intID = _ID

            Me.Load(bAudit)

        End Sub

        Public Sub New(ByVal _ID As Integer, ByVal _Name As String, ByVal _Description As String, ByVal _ActivityCompanies As Generic.List(Of roActivityCompany), ByVal _State As roActivityState)

            Me.oState = _State

            Me.intID = _ID
            Me.strName = _Name
            Me.strDescription = _Description

            Me.lstCompanies = _ActivityCompanies

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
                If Me.lstCompanies IsNot Nothing Then
                    For Each oCompany As roActivityCompany In Me.lstCompanies
                        oCompany.State = value
                    Next
                End If
            End Set
        End Property
        <DataMember()>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
                If Me.lstCompanies IsNot Nothing Then
                    For Each oChild As roActivityCompany In Me.lstCompanies
                        oChild.IDActivity = Me.intID
                    Next
                End If
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
        Public Property Companies() As Generic.List(Of roActivityCompany)
            Get
                Return Me.lstCompanies
            End Get
            Set(ByVal value As Generic.List(Of roActivityCompany))
                Me.lstCompanies = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM Activities " &
                                       "WHERE ID = " & Me.intID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = oRow("Name")
                    Me.strDescription = Any2String(oRow("Description"))

                    Me.lstCompanies = roActivityCompany.GetActivityCompanies(Me.intID, Me.oState)

                End If

                ' Auditar lectura
                If bAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tActivity, Me.strName, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roActivity::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivity::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True
            Try

                If Me.strName = "" Then
                    bolRet = False
                    Me.oState.Result = DTOs.ActivityResultEnum.InvalidName
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivity::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivity::Validate")
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

                    Dim tb As New DataTable("TimeZones")
                    Dim strSQL As String = "@SELECT# * FROM Activities " &
                                           "WHERE ID = " & Me.intID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        Me.intID = Me.GetNextID()
                        oRow("ID") = Me.intID
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("Name") = Me.strName
                    oRow("Description") = Me.strDescription

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    oAuditDataNew = oRow

                    bolRet = roActivityCompany.SaveActivityCompanies(Me.intID, Me.lstCompanies, Me.oState)

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tActivity, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet Then
                        ' Notificamos al servidor que tiene que lanzar el broadcaster
                        ' ** Queda pendiente informar los IDs de los terminales a regenerar. Actualmente regenera los ficheros para todos los terminales tipo mx6
                        roConnector.InitTask(TasksType.BROADCASTER)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivity::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivity::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                If Not Me.IsUsed() Then

                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                    bolRet = roActivityCompany.DeleteActivityCompanies(Me.intID, Me.oState)

                    If bolRet Then

                        Dim strSQL As String = "@DELETE# FROM Activities WHERE ID = " & Me.intID.ToString
                        bolRet = ExecuteSql(strSQL)

                    End If

                    If bolRet And bAudit Then
                        ' Auditamos
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tActivity, Me.strName, Nothing, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivity::Delete")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivity::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function IsUsed() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM TerminalReaders WHERE IDActivity = " & Me.intID.ToString
                If Any2Integer(ExecuteScalar(strSQL)) > 0 Then

                    bolRet = True
                    Me.oState.Result = DTOs.ActivityResultEnum.ActivityAssignedInTerminalReader

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivity::GetNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivity::GetNextID")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetNextID() As Integer
            Dim intRet As Integer = -1

            Try

                Dim strSQL As String = "@SELECT# MAX(ID) as MaxID FROM Activities"
                Dim oRet As Object = ExecuteScalar(strSQL)

                If IsDBNull(oRet) Then
                    intRet = 1
                Else
                    intRet = CInt(oRet) + 1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roActivity::GetNextID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roActivity::GetNextID")
            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetActivities(ByVal _State As roActivityState, Optional ByVal bAudit As Boolean = False) As Generic.List(Of roActivity)

            Dim oRet As New Generic.List(Of roActivity)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM Activities "
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        oRet.Add(New roActivity(oRow("ID"), _State))

                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roActivity::GetActivities")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roActivity::GetActivities")
            Finally

            End Try

            Return oRet

        End Function

#End Region

#End Region

    End Class

End Namespace