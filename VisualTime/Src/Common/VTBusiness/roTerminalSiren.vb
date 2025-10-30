Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Namespace Terminal

    <DataContract>
    Public Class roTerminalSiren

#Region "Declarations"

        Private oState As roTerminalState

        Private intIDTerminal As Integer
        Private intID As Integer
        Private intWeekDay As Integer
        Private xHour As DateTime
        Private intDuration As Integer

        Public Sub New()
            Me.oState = New roTerminalState()
        End Sub

        Public Sub New(ByVal _IDTerminal As Integer, ByVal _ID As Integer, ByVal _State As roTerminalState,
                       Optional ByVal bAudit As Boolean = False)
            Me.oState = _State
            Me.intIDTerminal = _IDTerminal
            Me.intID = _ID
            Me.Load()
        End Sub

        Public Sub New(ByVal _IDTerminal As Integer, ByVal _ID As Integer, ByVal _WeekDay As Integer, ByVal _Hour As DateTime, ByVal _Duration As Integer, ByVal _State As roTerminalState)
            Me.oState = _State
            Me.intIDTerminal = _IDTerminal
            Me.intID = _ID
            Me.intWeekDay = _WeekDay
            Me.xHour = _Hour
            Me.intDuration = _Duration
        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember>
        Public Property State() As roTerminalState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roTerminalState)
                Me.oState = value
            End Set
        End Property

        <DataMember>
        Public Property IDTerminal() As Integer
            Get
                Return Me.intIDTerminal
            End Get
            Set(ByVal value As Integer)
                Me.intIDTerminal = value
            End Set
        End Property

        <DataMember>
        Public Property ID() As Integer
            Get
                Return Me.intID
            End Get
            Set(ByVal value As Integer)
                Me.intID = value
            End Set
        End Property

        <DataMember>
        Public Property WeekDay() As Integer
            Get
                Return Me.intWeekDay
            End Get
            Set(ByVal value As Integer)
                Me.intWeekDay = value
            End Set
        End Property

        <DataMember>
        Public Property Hour() As DateTime
            Get
                Return Me.xHour
            End Get
            Set(ByVal value As DateTime)
                Me.xHour = value
            End Set
        End Property

        <DataMember>
        Public Property Duration() As Integer
            Get
                Return Me.intDuration
            End Get
            Set(ByVal value As Integer)
                Me.intDuration = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Function Load(Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Try

                Dim strSQL As String = "@SELECT# * FROM TerminalSirens " &
                                       "WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND " &
                                             "ID = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    Dim oRow As DataRow = tb.Rows(0)

                    Me.intWeekDay = Any2Integer(oRow("WeekDay"))
                    Me.xHour = CDate(oRow("Hour"))
                    Me.intDuration = Any2Integer(oRow("Duration"))

                    bolRet = True

                    If bAudit Then
                        ' Auditar lectura
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        bolRet = Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tTerminalSiren, "", tbParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminalSiren::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminalSiren::Load")
            Finally
            End Try

            Return bolRet

        End Function

        Public Function Validate() As Boolean

            Dim bolRet As Boolean = True

            Try

                ' Verificamos que el terminal exista
                Dim oTerminal As New roTerminal(Me.oState)
                oTerminal.ID = Me.intIDTerminal
                If Not oTerminal.Load(False) Then
                    Me.oState.Result = TerminalBaseResultEnum.Siren_TerminalNotExist
                    bolRet = False
                End If

                If bolRet Then

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTerminalSiren::Validate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTerminalSiren::Validate")
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

                    Dim tb As New DataTable("TerminalSirens")
                    Dim strSQL As String = "@SELECT# * FROM TerminalSirens " &
                                           "WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND " &
                                                 "ID = " & Me.ID.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tb)

                    Dim oRow As DataRow
                    If tb.Rows.Count = 0 Then
                        oRow = tb.NewRow
                        oRow("IDTerminal") = Me.IDTerminal
                        oRow("ID") = Me.ID
                    Else
                        oRow = tb.Rows(0)
                        oAuditDataOld = Extensions.roAudit.CloneRow(oRow)
                    End If

                    oRow("WeekDay") = Me.WeekDay
                    oRow("Hour") = Me.Hour
                    oRow("Duration") = Me.Duration

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
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tTerminalSiren, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminalSiren::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminalSiren::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@DELETE# FROM TerminalSirens " &
                                       "WHERE IDTerminal = " & Me.IDTerminal.ToString & " AND " &
                                                 "ID = " & Me.ID.ToString
                ExecuteSql(strSQL)

                bolRet = True

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tTerminalSiren, "", Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roTerminalSiren::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roTerminalSiren::Delete")
            Finally
            End Try

            Return bolRet

        End Function

#Region "Helper methods"

        Public Shared Function GetTerminalSirens(ByVal _IDTerminal As Integer, ByVal _State As roTerminalState) As Generic.List(Of roTerminalSiren)

            Dim oRet As New Generic.List(Of roTerminalSiren)

            Try

                Dim strSQL As String = "@SELECT# * FROM TerminalSirens " &
                                       "WHERE IDTerminal = " & _IDTerminal.ToString & " " &
                                       "ORDER BY WeekDay"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing Then

                    For Each oRow As DataRow In tb.Rows
                        oRet.Add(New roTerminalSiren(_IDTerminal, oRow("ID"), oRow("WeekDay"), oRow("Hour"), oRow("Duration"), _State))
                    Next

                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminalSiren::GetTerminalSirens")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminalSiren::GetTerminalSirens")
            Finally
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la configuración de sirenas de un terminal.
        ''' </summary>
        ''' <param name="_IDTerminal">Código de terminal</param>
        ''' <param name="_Sirens">Lista de sirenas a guardar. Si es nothing se borran todas las sirenas.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTerminalSirens(ByVal _IDTerminal As Integer, ByVal _Sirens As Generic.List(Of roTerminalSiren), ByVal _State As roTerminalState,
                                                  Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ExcludeList As New Generic.List(Of Integer)

                bolRet = True

                If checkReleIsUsedInReaders(_IDTerminal, _State) Then
                    _State.Result = TerminalBaseResultEnum.ReleAlreadyUsed
                    bolRet = False
                End If

                If bolRet AndAlso _Sirens IsNot Nothing AndAlso _Sirens.Count > 0 Then
                    For Each oSiren As roTerminalSiren In _Sirens
                        bolRet = oSiren.Save(bAudit)
                        ExcludeList.Add(oSiren.ID)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    Dim strSQL As String = "@DELETE# FROM TerminalSirens WHERE IDTerminal = " & _IDTerminal.ToString
                    If ExcludeList.Count > 0 Then
                        For Each intID As Integer In ExcludeList
                            strSQL &= " AND ID <> " & intID.ToString
                        Next
                    End If
                    bolRet = ExecuteSql(strSQL)
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminalSiren::SaveTerminalSirens")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminalSiren::SaveTerminalSirens")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function checkReleIsUsedInReaders(ByVal _IDTerminal As Integer, ByVal _State As roTerminalState,
                                                  Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Try

                Dim ExcludeList As New Generic.List(Of Integer)

                Dim strSQL As String = "@SELECT# count(*) FROM Terminals, TerminalReaders WHERE (TerminalReaders.Output = Terminals.SirensOutput OR TerminalReaders.InvalidOutput = Terminals.SirensOutput) AND Terminals.SirensOutput <> 0 AND TerminalReaders.IDTerminal=" & _IDTerminal &
                    " AND TerminalReaders.IDTerminal = Terminals.ID"

                Dim counter As Integer = roTypes.Any2Integer(ExecuteScalar(strSQL))

                If counter > 0 Then
                    bolRet = True
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roTerminalSiren::checkReleIsUsedInReaders")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roTerminalSiren::checkReleIsUsedInReaders")
            End Try

            Return bolRet
        End Function

#End Region

#End Region

    End Class

End Namespace