Imports System.Data.Common
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Job
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Move

    Public Class roJobMove

        Public Enum MovementStatus
            Begin_
            End_
            Indet_
        End Enum

#Region "Declarations - Constructor"

        Private oState As roMoveState

        'Variables locales
        Private intIDEmployee As Integer
        Private xDateTimeIN As Nullable(Of DateTime)
        Private xDateTimeOUT As Nullable(Of DateTime)
        Private intIDTerminalIN As Integer
        Private intIDTerminalOUT As Integer
        Private intIDEmployeeIN As Integer
        Private intIDEmployeeOUT As Integer
        Private oJob As roJob
        Private intIDMachine As Integer
        Private intIDIncidence As Integer
        Private strIncidenceDesc As String
        Private lngIDMove As Long
        Private dblPieces1 As Double
        Private dblPieces2 As Double
        Private dblPieces3 As Double
        Private bolMulti As Boolean
        Private oQualityFields As roJobQualityFields
        Private lngQualityFields() As Long

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _IDMove As Long, ByVal _State As roMoveState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee

            ' Obtenemos los campos para el control de calidad y los inicializamos a zero
            Dim oJobState As New roJobState : roBusinessState.CopyTo(Me.oState, oJobState)
            Me.oQualityFields = New roJobQualityFields(oJobState)
            ReDim Me.lngQualityFields(Me.oQualityFields.Fields.Length - 1)
            For n As Integer = 0 To Me.lngQualityFields.Length - 1
                Me.lngQualityFields(n) = 0
            Next

            Me.IDMove = _IDMove

        End Sub

#End Region

#Region "Properties"

        Public Property Pieces3() As Double
            Get
                Return dblPieces3
            End Get
            Set(ByVal value As Double)
                dblPieces3 = value
            End Set
        End Property
        Public Property Pieces2() As Double
            Get
                Return dblPieces2
            End Get
            Set(ByVal value As Double)
                dblPieces2 = value
            End Set
        End Property
        Public Property Pieces1() As Double
            Get
                Return dblPieces1
            End Get
            Set(ByVal value As Double)
                dblPieces1 = value
            End Set
        End Property

        Public ReadOnly Property Pieces() As Double()
            Get
                Return New Double(2) {dblPieces1, dblPieces2, dblPieces3}
            End Get
        End Property

        Public Property IDMove() As Long
            Get
                Return lngIDMove
            End Get
            Set(ByVal value As Long)
                lngIDMove = value
                Me.Load()
            End Set
        End Property

        Public Property IDIncidence() As Integer
            Get
                Return intIDIncidence
            End Get
            Set(ByVal value As Integer)
                intIDIncidence = value
                Me.LoadIncidence()
            End Set
        End Property

        Public ReadOnly Property IncidenceDesc() As String
            Get
                Return Me.strIncidenceDesc
            End Get
        End Property

        Public Property IDMachine() As Integer
            Get
                Return intIDMachine
            End Get
            Set(ByVal value As Integer)
                intIDMachine = value
            End Set
        End Property

        Public Property Job() As roJob
            Get
                Return oJob
            End Get
            Set(ByVal value As roJob)
                oJob = value
            End Set
        End Property

        Public Property IDTerminalOUT() As Integer
            Get
                Return intIDTerminalOUT
            End Get
            Set(ByVal value As Integer)
                intIDTerminalOUT = value
            End Set
        End Property

        Public Property IDTerminalIN() As Integer
            Get
                Return intIDTerminalIN
            End Get
            Set(ByVal value As Integer)
                intIDTerminalIN = value
            End Set
        End Property

        Public Property DateTimeOUT() As Nullable(Of DateTime)
            Get
                Return xDateTimeOUT
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xDateTimeOUT = value
            End Set
        End Property

        Public Property DateTimeIN() As Nullable(Of DateTime)
            Get
                Return xDateTimeIN
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                xDateTimeIN = value
            End Set
        End Property

        Public Property IDEmployeeOUT() As Integer
            Get
                Return intIDEmployeeOUT
            End Get
            Set(ByVal value As Integer)
                intIDEmployeeOUT = value
            End Set
        End Property
        Public Property IDEmployeeIN() As Integer
            Get
                Return intIDEmployeeIN
            End Get
            Set(ByVal value As Integer)
                intIDEmployeeIN = value
            End Set
        End Property

        Public Property IDEmployeeTeam() As Integer
            Get
                Return intIDEmployee
            End Get
            Set(ByVal value As Integer)
                intIDEmployee = value
            End Set
        End Property

        Public Property QualityFields() As Long()
            Get
                Return Me.lngQualityFields
            End Get
            Set(ByVal value As Long())
                Me.lngQualityFields = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub LoadNewBEGIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, ByVal _Job As roJob, Optional ByVal _IDMove As Long = -1, Optional ByVal _IDMachine As Integer = 0, Optional ByVal _IDIncidence As Integer = 0)

            Me.IDMove = _IDMove

            Me.xDateTimeIN = _DateTimeIN
            Me.intIDTerminalIN = _IDTerminalIN
            Me.oJob = _Job
            Me.intIDMachine = _IDMachine
            Me.IDIncidence = _IDIncidence

        End Sub

        Public Sub LoadNewEND(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _IDMove As Long = -1, Optional ByVal _Pieces1 As Double = 0, Optional ByVal _Pieces2 As Double = 0, Optional ByVal _Pieces3 As Double = 0, Optional ByVal _Multi As Boolean = False, Optional ByVal _QualityFields As Long() = Nothing)

            ''Me.IDMove = _IDMove

            Me.xDateTimeOUT = _DateTimeOUT
            Me.intIDTerminalOUT = _IDTerminalOUT
            Me.dblPieces1 = _Pieces1
            Me.dblPieces2 = _Pieces2
            Me.dblPieces3 = _Pieces3
            Me.bolMulti = _Multi

            If _QualityFields IsNot Nothing Then
                ReDim Me.lngQualityFields(_QualityFields.Length - 1)
                For n As Integer = 0 To _QualityFields.Length - 1
                    Me.lngQualityFields(n) = _QualityFields(n)
                Next
            End If

        End Sub

        Public Sub LoadBEGIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, ByVal _Job As roJob, Optional ByVal _IDMachine As Integer = 0, Optional ByVal _IDIncidence As Integer = 0)

            Me.xDateTimeIN = _DateTimeIN
            Me.intIDTerminalIN = _IDTerminalIN
            Me.oJob = _Job
            Me.intIDMachine = _IDMachine
            Me.IDIncidence = _IDIncidence

        End Sub

        Public Sub LoadEND(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _Pieces1 As Double = 0, Optional ByVal _Pieces2 As Double = 0, Optional ByVal _Pieces3 As Double = 0, Optional ByVal _Multi As Boolean = False, Optional ByVal _QualityFields As Long() = Nothing)

            Me.xDateTimeOUT = _DateTimeOUT
            Me.intIDTerminalOUT = _IDTerminalOUT
            Me.dblPieces1 = _Pieces1
            Me.dblPieces2 = _Pieces2
            Me.dblPieces3 = _Pieces3
            Me.bolMulti = _Multi

            If _QualityFields IsNot Nothing Then
                ReDim Me.lngQualityFields(_QualityFields.Length - 1)
                For n As Integer = 0 To _QualityFields.Length - 1
                    Me.lngQualityFields(n) = _QualityFields(n)
                Next
            End If

        End Sub

        Private Sub Load()

            If Me.lngIDMove <= 0 Then

                Me.xDateTimeIN = Nothing
                Me.xDateTimeOUT = Nothing
                Me.intIDTerminalIN = -1
                Me.intIDTerminalOUT = -1
                Me.intIDEmployeeIN = -1
                Me.intIDEmployeeOUT = -1
                Me.oJob = Nothing
                Me.intIDMachine = 0
                Me.intIDIncidence = 0

                Me.dblPieces1 = 0
                Me.dblPieces2 = 0
                Me.dblPieces3 = 0

                Me.bolMulti = False

                For n As Integer = 0 To Me.lngQualityFields.Length - 1
                    Me.lngQualityFields(n) = 0
                Next
            Else

                Dim tb As DataTable = Nothing
                Try

                    tb = CreateDataTable("@SELECT# * FROM EmployeeJobMoves WHERE [ID] = " & Me.IDMove.ToString, )

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Me.intIDEmployee = tb.Rows(0)("IDEmployee")

                        If Not IsDBNull(tb.Rows(0)("InDateTime")) Then
                            Me.xDateTimeIN = tb.Rows(0)("InDateTime")
                        Else
                            Me.xDateTimeIN = Nothing
                        End If
                        If Not IsDBNull(tb.Rows(0)("OutDateTime")) Then
                            Me.xDateTimeOUT = tb.Rows(0)("OutDateTime")
                        Else
                            Me.xDateTimeOUT = Nothing
                        End If

                        If Not IsDBNull(tb.Rows(0)("InIDReader")) Then
                            Me.intIDTerminalIN = tb.Rows(0)("InIDReader")
                        Else
                            Me.intIDTerminalIN = -1
                        End If
                        If Not IsDBNull(tb.Rows(0)("OutIDReader")) Then
                            Me.intIDTerminalOUT = tb.Rows(0)("OutIDReader")
                        Else
                            Me.intIDTerminalOUT = -1
                        End If

                        If Not IsDBNull(tb.Rows(0)("IDJob")) Then
                            Dim oJobState As New roJobState : roBusinessState.CopyTo(Me.oState, oJobState)
                            Me.oJob = New roJob(oJobState)
                            Me.oJob.ID = tb.Rows(0)("IDJob")
                            Me.oJob.Load()
                        Else
                            Me.oJob = Nothing
                        End If

                        Me.intIDMachine = Any2Integer(tb.Rows(0)("IDMachine"))
                        Me.IDIncidence = Any2Integer(tb.Rows(0)("IDIncidence"))

                        Me.dblPieces1 = Any2Double(tb.Rows(0)("Pieces1"))
                        Me.dblPieces2 = Any2Double(tb.Rows(0)("Pieces2"))
                        Me.dblPieces3 = Any2Double(tb.Rows(0)("Pieces3"))

                        ' Obtenemos los valores de los campos para el control de calidad (si existen), ordenados alfabéticamente
                        For n As Integer = 0 To Me.oQualityFields.Fields.Length - 1
                            Me.lngQualityFields(n) = Any2Long(tb.Rows(0)("QLTY_" & oQualityFields.Fields(n)))
                        Next
                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roJobMove::Load :")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roJobMove::Load :")
                Finally

                End Try

            End If

        End Sub

        Public Function Save() As Boolean
            Dim bolRet As Boolean = False
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roJobMove::Save::Saving job punch data (" & Me.MoveInfo() & ")")

            Try
                Dim bolHasMoves As Boolean = True
                If Me.oJob IsNot Nothing Then bolHasMoves = Me.oJob.HasMoves()

                If Me.IDIncidence = 0 AndAlso Me.oJob IsNot Nothing Then Me.IDIncidence = Me.oJob.AutomaticPreparationIncidence()

                Dim tbMove As New DataTable("EmployeeJobMoves")
                Dim strSQL As String = "@SELECT# * FROM EmployeeJobMoves WHERE [ID] = " & Me.lngIDMove.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbMove)

                Dim oRow As DataRow = Nothing
                If Me.lngIDMove = -1 Then
                    oRow = tbMove.NewRow
                ElseIf tbMove.Rows.Count = 1 Then
                    oRow = tbMove.Rows(0)
                End If

                oRow("IDEmployee") = Me.intIDEmployee
                If Me.xDateTimeIN.HasValue Then
                    oRow("InDateTime") = Me.xDateTimeIN.Value
                Else
                    oRow("InDateTime") = DBNull.Value
                End If
                If Me.xDateTimeOUT.HasValue Then
                    oRow("OutDateTime") = Me.xDateTimeOUT.Value
                Else
                    oRow("OutDateTime") = DBNull.Value
                End If

                oRow("InIDReader") = IIf(Me.intIDTerminalIN <> -1, Me.intIDTerminalIN, DBNull.Value)
                oRow("OutIDReader") = IIf(Me.intIDTerminalOUT <> -1, Me.intIDTerminalOUT, DBNull.Value)

                If Me.oJob IsNot Nothing Then
                    oRow("IDJob") = Me.oJob.ID
                End If

                If Me.bolMulti Then
                    oRow("IsDistributed") = 1
                End If

                oRow("IDMachine") = Me.intIDMachine
                oRow("IDIncidence") = Me.intIDIncidence

                oRow("Processed") = 0

                oRow("Pieces1") = Me.dblPieces1
                oRow("Pieces2") = Me.dblPieces2
                oRow("Pieces3") = Me.dblPieces3

                For n As Integer = 0 To Me.oQualityFields.Fields.Length - 1
                    If Me.lngQualityFields.Length > n Then
                        oRow("QLTY_" & oQualityFields.Fields(n)) = Me.lngQualityFields(n)
                    Else
                        oRow("QLTY_" & oQualityFields.Fields(n)) = 0
                    End If
                Next

                If Me.lngIDMove = -1 Then
                    tbMove.Rows.Add(oRow)
                End If

                da.Update(tbMove)

                If Me.lngIDMove <= 0 Then
                    Dim tb As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM EmployeeJobMoves WHERE IDEmployee = " & Me.intIDEmployee.ToString & " " &
                                                              "ORDER BY [ID] DESC", )

                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        Me.lngIDMove = tb.Rows(0)("ID")
                    End If
                End If

                ' Modificar DailySchedule.JobStatus
                Dim xDate As DateTime
                Dim xEndDate As DateTime
                If Not Me.xDateTimeOUT.HasValue Then ' Solo actualiza el día de inicio del movimiento
                    xDate = Me.xDateTimeIN.Value
                    xEndDate = Me.xDateTimeIN.Value
                Else ' Actualiza el estado de todo el periodo del movimineto (más un día antes y un día después)
                    xDate = Me.xDateTimeIN.Value.AddDays(-1)
                    xEndDate = Me.xDateTimeOUT.Value.AddDays(1)
                End If

                While xDate <= xEndDate
                    Dim tbSchedule As New DataTable("DailySchedule")
                    strSQL = "@SELECT# * FROM DailySchedule " &
                             "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                   "[Date] = " & Any2Time(xDate.Date).SQLSmallDateTime
                    cmd = CreateCommand(strSQL)
                    da = CreateDataAdapter(cmd, True)
                    da.Fill(tbSchedule)
                    If tbSchedule.Rows.Count = 0 Then
                        oRow = tbSchedule.NewRow
                        oRow("IDEmployee") = Me.intIDEmployee
                        oRow("Date") = xDate.Date
                    Else
                        oRow = tbSchedule.Rows(0)
                    End If
                    oRow("JobStatus") = 0
                    If tbSchedule.Rows.Count = 0 Then tbSchedule.Rows.Add(oRow)
                    da.Update(tbSchedule)
                    xDate = xDate.AddDays(1)
                End While

                bolRet = True

                ' Si es la primera vez que se inicia la fase
                If bolRet AndAlso
                   Me.DateTimeIN.HasValue AndAlso Me.oJob IsNot Nothing AndAlso Not bolHasMoves Then ' IsDBNull(Me.oJob.StartDate) Then
                    ' Marcar fecha inicio
                    bolRet = Me.oJob.UpdateStartDate(Me.DateTimeIN.Value.Date)
                End If

                If bolRet AndAlso Me.intIDMachine <> 0 Then
                    bolRet = Me.oJob.UpdateMachine(Me.intIDMachine)
                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roJobMove::Save :")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roJobMove::Save :")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim sSql As String = "@DELETE# FROM EmployeeJobMoves WHERE [ID] = " & Me.IDMove.ToString
                bolRet = ExecuteSql(sSql)
                If bolRet Then
                    Me.IDMove = 0
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roJobMove::Delete :")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roJobMove::Delete :")
            Finally

            End Try

            Return bolRet

        End Function

        Public Sub GetLastMove(ByRef oLastMoveType As MovementStatus, ByRef xLastMoveDateTime As DateTime, ByRef lngLastMoveID As Long)

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, InDateTime FROM EmployeeJobMoves " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND OutDateTime IS NULL " &
                                       "ORDER BY InDateTime DESC"
                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oLastMoveType = MovementStatus.Begin_
                    xLastMoveDateTime = CDate(tb.Rows(0)("InDateTime"))
                    lngLastMoveID = tb.Rows(0)("ID")
                Else
                    oLastMoveType = MovementStatus.Indet_
                    lngLastMoveID = -1
                End If

                strSQL = "@SELECT# TOP 1 ID, OutDateTime FROM EmployeeJobMoves " &
                         "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND OutDateTime IS NOT NULL " &
                         "ORDER BY OutDateTime DESC"

                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If CDate(tb.Rows(0)("OutDateTime")) > xLastMoveDateTime Or oLastMoveType = MovementStatus.Indet_ Then
                        oLastMoveType = MovementStatus.End_
                        xLastMoveDateTime = CDate(tb.Rows(0)("OutDateTime"))
                        lngLastMoveID = tb.Rows(0)("ID")
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roJobMove::GetLastMove :")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roJobMove::GetLastMove :")
            Finally

            End Try

        End Sub

        Public Shared Function GetProductionMovesInPeriod(ByVal selectedDate As Date, ByVal endSelectedDate As Date, ByVal IdEmployee As Integer, ByRef oState As roMoveState) As DataTable

            Dim dt As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# EmployeeJobMoves.ID, InDateTime, OutDateTime, Jobs.Name as JobName , JobIncidences.ShortName as IncidenceName, " &
                                        "srp1.Name as PiecesName1, srp1.IsUsed as PiecesUsed1, Pieces1, " &
                                        "srp2.Name as PiecesName2, srp2.IsUsed as PiecesUsed2, Pieces2, " &
                                        "srp3.Name as PiecesName3, srp3.IsUsed as PiecesUsed3, Pieces3 " &
                                        "FROM EmployeeJobMoves " &
                                        "inner join Jobs on EmployeeJobMoves.IDJob = Jobs.ID " &
                                        "inner join JobIncidences on EmployeeJobMoves.IDIncidence = JobIncidences.ID " &
                                        "inner join (@SELECT# ID, Name, IsUsed from sysroPieceTypes) srp1 on srp1.ID = 1" &
                                        "inner join (@SELECT# ID, Name, IsUsed from sysroPieceTypes) srp2 on srp2.ID = 2 " &
                                        "inner join (@SELECT# ID, Name, IsUsed from sysroPieceTypes) srp3 on srp3.ID = 3 " &
                                       "WHERE IDEmployee = " & IdEmployee.ToString & " AND " &
                                       "InDateTime >= " & Any2Time(selectedDate).SQLSmallDateTime & " AND " &
                                       "InDateTime < " & Any2Time(endSelectedDate.AddDays(1)).SQLSmallDateTime & " " &
                                       "ORDER BY InDateTime ASC"

                dt = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roJobMove::GetProductionMoves :")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roJobMove::GetProductionMoves :")
            Finally

            End Try

            Return dt
        End Function

        Public Shared Function GetProductionMoves(ByVal selectedDate As Date, ByVal IdEmployee As Integer, ByRef oState As roMoveState) As DataTable

            Dim dt As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# EmployeeJobMoves.ID, InDateTime, OutDateTime, Jobs.Name as JobName , JobIncidences.ShortName as IncidenceName, " &
                                        "srp1.Name as PiecesName1, srp1.IsUsed as PiecesUsed1, Pieces1, " &
                                        "srp2.Name as PiecesName2, srp2.IsUsed as PiecesUsed2, Pieces2, " &
                                        "srp3.Name as PiecesName3, srp3.IsUsed as PiecesUsed3, Pieces3 " &
                                        "FROM EmployeeJobMoves " &
                                        "inner join Jobs on EmployeeJobMoves.IDJob = Jobs.ID " &
                                        "inner join JobIncidences on EmployeeJobMoves.IDIncidence = JobIncidences.ID " &
                                        "inner join (@SELECT# ID, Name, IsUsed from sysroPieceTypes) srp1 on srp1.ID = 1" &
                                        "inner join (@SELECT# ID, Name, IsUsed from sysroPieceTypes) srp2 on srp2.ID = 2 " &
                                        "inner join (@SELECT# ID, Name, IsUsed from sysroPieceTypes) srp3 on srp3.ID = 3 " &
                                       "WHERE IDEmployee = " & IdEmployee.ToString & " AND " &
                                       "InDateTime >= " & Any2Time(selectedDate).SQLSmallDateTime & " AND " &
                                       "InDateTime < " & Any2Time(selectedDate.AddDays(1)).SQLSmallDateTime & " " &
                                       "ORDER BY InDateTime ASC"

                dt = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roJobMove::GetProductionMoves :")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roJobMove::GetProductionMoves :")
            Finally

            End Try

            Return dt
        End Function

        Private Sub LoadIncidence()

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# Name FROM JobIncidences WHERE [ID] = " & Me.intIDIncidence.ToString

                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Me.strIncidenceDesc = tb.Rows(0)("Name")
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roJobMove::LoadIncidence :")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roJobMove::LoadIncidence :")
            Finally

            End Try

        End Sub

        Private Function MoveInfo() As String

            Dim strPunch As String = "IDEmployee=" & Me.intIDEmployee.ToString & ";"
            If Me.xDateTimeIN.HasValue Then _
                strPunch &= "InDateTime= " & Me.xDateTimeIN.Value.ToString(oState.Language.GetShortDateFormat) & " " & Me.xDateTimeIN.Value.ToShortTimeString & ";"
            If Me.xDateTimeOUT.HasValue Then _
                strPunch &= "OutDateTime= " & Me.xDateTimeOUT.Value.ToString(oState.Language.GetShortDateFormat) & " " & Me.xDateTimeOUT.Value.ToShortTimeString & ";"
            If Me.intIDTerminalIN <> -1 Then _
                strPunch &= "InIDReader=" & Me.intIDTerminalIN.ToString & ";"
            If Me.intIDTerminalOUT <> -1 Then _
                strPunch &= "OutIDReader=" & Me.intIDTerminalOUT.ToString & ";"
            If Me.oJob IsNot Nothing Then _
                strPunch &= "IDJob=" & Me.oJob.ID & ";"
            If Me.bolMulti Then
                strPunch &= "IsDistributed= 1;"
            End If
            strPunch &= "IDMachine=" & Me.intIDMachine & ";"
            strPunch &= "IDIncidence=" & Me.intIDIncidence & ";"
            strPunch &= "Processed=0;"
            strPunch &= "Pieces1=" & Me.dblPieces1 & ";"
            strPunch &= "Pieces2=" & Me.dblPieces2 & ";"
            strPunch &= "Pieces3=" & Me.dblPieces3 & ";"

            For n As Integer = 0 To Me.oQualityFields.Fields.Length - 1
                If Me.lngQualityFields.Length > n Then
                    strPunch &= "QLTY_" & oQualityFields.Fields(n) & "=" & Me.lngQualityFields(n)
                Else
                    strPunch &= "QLTY_" & oQualityFields.Fields(n) & "=0"
                End If
                strPunch &= ";"
            Next

            Return strPunch

        End Function

#End Region

    End Class

End Namespace