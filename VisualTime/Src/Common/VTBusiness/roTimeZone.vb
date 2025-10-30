Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase

Namespace Shift

    <DataContract()>
    Public Class roTimeZone

#Region "Declarations - Constructor"

        Private oState As roShiftState

        Private intIDTimeZone As Integer
        Private strName As String = ""
        Private strDescription As String = ""

        Public Sub New()

            Me.oState = New roShiftState(-1)

            Me.intIDTimeZone = -1

        End Sub

        Public Sub New(ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDTimeZone = -1

        End Sub

        Public Sub New(ByVal IDTimeZone As Integer, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDTimeZone = IDTimeZone

            Me.Load()

        End Sub

        Public Sub New(ByVal _IDTimeZone As Integer, ByVal _Name As String, ByVal _Description As String, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDTimeZone = _IDTimeZone
            Me.strName = _Name
            Me.strDescription = _Description

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
        Public Property ID() As Integer
            Get
                Return Me.intIDTimeZone
            End Get
            Set(ByVal value As Integer)
                Me.intIDTimeZone = value
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

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM TimeZones " &
                                       "WHERE ID = " & Me.intIDTimeZone.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.strName = oRow("Name")
                    Me.strDescription = oRow("Description")

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTimeZone, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTimeZone::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTimeZone::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                If Me.strName = "" Then
                    bolRet = False
                    Me.oState.Result = ShiftResultEnum.TimeZone_ErrorNameIncorrect  'Debe insertar un nombre
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTimeZone::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTimeZone::Validate")
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bolTimeZoneAlreadyExists = False

            Try

                If Me.Validate() Then
                    Dim oAuditDataOld As DataRow = Nothing
                    Dim oAuditDataNew As DataRow = Nothing

                    Dim tb As New DataTable("TimeZones")

                    If Me.intIDTimeZone = -1 Then
                        Dim strNameSQL As String = "@SELECT# * FROM TimeZones " &
                                           "WHERE Name = '" & Me.strName & "'"
                        Dim cmdName As DbCommand = CreateCommand(strNameSQL)
                        Dim daName As DbDataAdapter = CreateDataAdapter(cmdName, True)
                        daName.Fill(tb)

                        bolTimeZoneAlreadyExists = IIf(tb.Rows.Count <> 0, True, False)

                        tb.Clear()
                    End If

                    If Not bolTimeZoneAlreadyExists Then
                        Dim strSQL As String = "@SELECT# * FROM TimeZones " &
                                               "WHERE ID = " & Me.intIDTimeZone.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                            oRow("ID") = Me.GetMaxIDTimeZone()
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
                        bolRet = True

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
                            bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTimeZone, strObjectName, tbAuditParameters, -1)
                        End If
                    Else
                        Me.oState.Result = ShiftResultEnum.ExportNameAlreadyExist
                        bolRet = False
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTimeZone::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTimeZone::Save")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function GetMaxIDTimeZone() As Integer
            Dim intRet As Integer = -1

            Try

                Dim strSQL As String = "@SELECT# MAX(ID) as MaxID FROM TimeZones"
                intRet = ExecuteScalar(strSQL)

                If IsDBNull(intRet) Then
                    intRet = 1
                Else
                    intRet += 1
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTimeZone::GetMaxIDTimeZone")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTimeZone::GetMaxIDTimeZone")
            Finally

            End Try

            Return intRet

        End Function

#Region "Helper methods"

        Public Shared Function GetTimeZones(ByVal _State As roShiftState) As Generic.List(Of roTimeZone)

            Dim oRet As New Generic.List(Of roTimeZone)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM TimeZones "
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        oRet.Add(New roTimeZone(oRow("ID"), oRow("Name"), oRow("Description"), _State))

                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftTimeZone::GetTimeZones")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftTimeZone::GetTimeZones")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function DeleteTimeZone(ByVal _ID As Integer, ByVal _State As roShiftState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                'Comprobem si algu te assignat el TimeZone a eliminar
                Dim sSQL As String = "@SELECT# Count(*) From sysroShiftTimeZones Where IDZone = " & _ID.ToString
                Dim intResult As Integer = ExecuteScalar(sSQL)

                If intResult > 0 Then
                    bolRet = False
                    _State.Result = ShiftResultEnum.TimeZone_AssignInZones
                Else
                    Dim strSQL As String = "@DELETE# FROM TimeZones WHERE ID = " & _ID.ToString
                    bolRet = ExecuteSql(strSQL)
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = _State.Audit(Audit.Action.aDelete, Audit.ObjectType.tTimeZone, "", Nothing, -1)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTimeZone::DeleteTimeZone")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTimeZone::DeleteTimeZone")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace