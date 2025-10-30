Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Shift

    <DataContract()>
    Public Class roShiftTimeZone

#Region "Declarations - Constructor"

        Private oState As roShiftState

        Private intIDShift As Integer
        Private intIDZone As Integer
        Private xBeginTime As DateTime
        Private xEndTime As DateTime
        Private bolIsBlocked As Boolean

        Private strName As String

        Public Sub New()

            Me.oState = New roShiftState(-1)

            Me.intIDShift = -1
            Me.intIDZone = -1

        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDShift = _IDShift
            Me.intIDZone = -1

        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _IDZone As Integer, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDShift = _IDShift
            Me.intIDZone = _IDZone

            Me.Load()

        End Sub

        Public Sub New(ByVal _IDShift As Integer, ByVal _IDZone As Integer, ByVal _BeginTime As DateTime, ByVal _EndTime As DateTime, ByVal _IsBlocked As Boolean, ByVal _State As roShiftState)

            Me.oState = _State

            Me.intIDShift = _IDShift
            Me.intIDZone = _IDZone
            Me.xBeginTime = _BeginTime
            Me.xEndTime = _EndTime
            Me.bolIsBlocked = _IsBlocked

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
        Public Property IDZone() As Integer
            Get
                Return Me.intIDZone
            End Get
            Set(ByVal value As Integer)
                Me.intIDZone = value
            End Set
        End Property
        <DataMember()>
        Public Property BeginTime() As DateTime
            Get
                Return Me.xBeginTime
            End Get
            Set(ByVal value As DateTime)
                Me.xBeginTime = value
            End Set
        End Property
        <DataMember()>
        Public Property EndTime() As DateTime
            Get
                Return Me.xEndTime
            End Get
            Set(ByVal value As DateTime)
                Me.xEndTime = value
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
        Public Property IsBlocked() As Boolean
            Get
                Return Me.bolIsBlocked
            End Get
            Set(ByVal value As Boolean)
                Me.bolIsBlocked = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# sysroShiftTimeZones.*, TimeZones.Name " &
                                       "FROM sysroShiftTimeZones INNER JOIN TimeZones ON sysroShiftTimeZones.IDZone = TimeZones.ID " &
                                       "WHERE sysroShiftTimeZones.IDShift = " & Me.intIDShift.ToString & " AND " &
                                             "sysroShiftTimeZones.IDZone = " & Me.intIDZone.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.xBeginTime = oRow("BeginTime")
                    Me.xEndTime = oRow("EndTime")
                    Me.strName = Any2String(oRow("Name"))
                    Me.bolIsBlocked = Any2Boolean(oRow("IsBlocked"))

                    ' Auditar lectura
                    If bAudit Then
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        oState.AddAuditParameter(tbParameters, "{Name}", Me.strName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tShiftTimeZone, Me.strName, tbParameters, -1)
                    End If

                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roShiftTimeZone::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftTimeZone::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String
                Dim tb As DataTable

                ' Verificamos que la zona horaria exista
                If Me.intIDShift <> -1 Then
                    strSQL = "@SELECT# ID FROM TimeZones WHERE ID = " & Me.intIDZone.ToString
                    tb = CreateDataTable(strSQL, )
                    If Not (tb IsNot Nothing AndAlso tb.Rows.Count > 0) Then
                        bolRet = False
                        Me.oState.Result = ShiftResultEnum.ShiftTimeZone_TimeZoneNotExist
                    End If
                End If

                If bolRet Then
                    If Me.xBeginTime > Me.xEndTime Then
                        bolRet = False
                        Me.oState.Result = ShiftResultEnum.ShiftTimeZone_TimePeriodIncorrect
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftTimeZone::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftTimeZone::Validate")
                bolRet = False
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

                    Dim tb As New DataTable("sysroShiftTimeZones")
                    Dim strSQL As String = "@SELECT# * FROM sysroShiftTimeZones " &
                                           "WHERE IDShift = " & Me.intIDShift.ToString & " AND IDZone = " & Me.intIDZone.ToString & " AND BeginTime = " & roTypes.Any2Time(Me.xBeginTime).SQLDateTime
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDShift") = Me.intIDShift
                        oRow("IDZone") = Me.intIDZone
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("BeginTime") = Me.xBeginTime
                    oRow("EndTime") = Me.xEndTime
                    oRow("IsBlocked") = Me.bolIsBlocked

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
                        Dim strObjectName As String = ""
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tShiftTimeZone, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roShiftTimeZone::Save")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roShiftTimeZone::Save")
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetShiftTimeZones(ByVal _IDShift As Integer, ByVal _State As roShiftState) As Generic.List(Of roShiftTimeZone)

            Dim oRet As New Generic.List(Of roShiftTimeZone)

            Try

                Dim strSQL As String = "@SELECT# * " &
                                       "FROM sysroShiftTimeZones " &
                                       "WHERE IDShift = " & _IDShift.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )

                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows

                        oRet.Add(New roShiftTimeZone(oRow("IDShift"), oRow("IDZone"), CDate(oRow("BeginTime")), CDate(oRow("EndTime")), Any2Boolean(oRow("IsBlocked")), _State))

                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftTimeZone::GetShiftTimeZones")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftTimeZone::GetShiftTimeZones")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function SaveShiftTimeZones(ByVal oTimeZones As Generic.List(Of roShiftTimeZone), ByVal _State As roShiftState,
                                                  Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strIDShifts As String = ""
                Dim strIDZones As String = ""
                If oTimeZones IsNot Nothing Then
                    bolRet = True
                    For Each oTimeZone As roShiftTimeZone In oTimeZones
                        bolRet = oTimeZone.Save(bAudit)
                        If Not bolRet Then Exit For
                        strIDShifts &= "," & oTimeZone.IDShift.ToString
                        strIDZones &= "," & oTimeZone.IDZone.ToString
                    Next
                    If strIDShifts <> "" Then strIDShifts = strIDShifts.Substring(1)
                    If strIDZones <> "" Then strIDZones = strIDZones.Substring(1)
                Else
                    bolRet = True
                End If

                'If bolRet Then
                '    ' Borramos las zonas horarias de la tabla que no esten en la lista
                '    If strIDShifts <> "" Then
                '        Dim strSQL As String = "@DELETE# FROM sysroShiftTimeZones " & _
                '                               "WHERE IDShift IN (" & strIDShifts & ") AND IDZone NOT IN (" & strIDZones & ")"
                '        bolRet = ExecuteSql(strSQL, oTrans.Connection)
                '    End If
                'End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftTimeZone::SaveShiftTimeZones")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftTimeZone::SaveShiftTimeZones")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteShiftTimeZones(ByVal _IDShift As Integer, ByVal _State As roShiftState) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM sysroShiftTimeZones WHERE IDShift = " & _IDShift.ToString
                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftTimeZone::DeleteShiftTimeZones")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftTimeZone::DeleteShiftTimeZones")
            Finally

            End Try

            Return bolRet

        End Function

        Public Shared Function ValidateShiftTimeZones(ByVal oTimeZones As Generic.List(Of roShiftTimeZone), ByVal _State As roShiftState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Verificar que
                If oTimeZones IsNot Nothing Then
                    If oTimeZones.Count > 0 Then
                        For Each oTimeZone As roShiftTimeZone In oTimeZones
                            bolRet = oTimeZone.Validate()
                            If Not bolRet Then Exit For
                        Next
                    Else
                        bolRet = True
                    End If
                Else
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roShiftTimeZone::ValidateShiftTimeZones")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roShiftTimeZone::ValidateShiftTimeZones")
            Finally

            End Try

            Return bolRet

        End Function

#End Region

#End Region

    End Class

End Namespace