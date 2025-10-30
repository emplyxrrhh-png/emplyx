Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roJobMachineMove

    Public Enum MovementStatus
        Begin_
        End_
        Indet_
    End Enum

#Region "Declarations - Constructor"

    Private oLog As roLog

    'Variables locales
    Private intIDMachine As Integer
    Private xDateTimeIN As Nullable(Of DateTime)
    Private xDateTimeOUT As Nullable(Of DateTime)
    Private intIDEmployeeIN As Integer
    Private intIDEmployeeOUT As Integer
    Private intIDTerminalIN As Integer
    Private intIDTerminalOUT As Integer
    Private oJob As roJob
    Private intIDIncidence As Integer
    Private strIncidenceDesc As String
    Private lngIDMove As Long
    Private dblPieces1 As Double
    Private dblPieces2 As Double
    Private dblPieces3 As Double

    Public Sub New(ByVal _IDMachine As Integer, ByVal _IDMove As Long, ByVal _Log As roLog)

        Me.oLog = _Log

        Me.intIDMachine = _IDMachine

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

#End Region

#Region "Methods"

    Public Sub LoadNewBEGIN(ByVal _DateTimeIN As DateTime, ByVal _IDEmployeeIN As Integer, ByVal _IDTerminalIN As Integer, ByVal _Job As roJob, Optional ByVal _IDMove As Long = -1, Optional ByVal _IDIncidence As Integer = 0)

        Me.IDMove = _IDMove

        Me.xDateTimeIN = _DateTimeIN
        Me.intIDEmployeeIN = _IDEmployeeIN
        Me.intIDTerminalIN = _IDTerminalIN
        Me.oJob = _Job
        Me.IDIncidence = _IDIncidence

    End Sub

    Public Sub LoadNewEND(ByVal _DateTimeOUT As DateTime, ByVal _IDEmployeeOUT As Integer, ByVal _IDTerminalOUT As Integer, Optional ByVal _IDMove As Long = -1, Optional ByVal _Pieces1 As Double = 0, Optional ByVal _Pieces2 As Double = 0, Optional ByVal _Pieces3 As Double = 0)

        ''Me.IDMove = _IDMove

        Me.xDateTimeOUT = _DateTimeOUT
        Me.intIDEmployeeOUT = _IDEmployeeOUT
        Me.intIDTerminalOUT = _IDTerminalOUT
        Me.dblPieces1 = _Pieces1
        Me.dblPieces2 = _Pieces2
        Me.dblPieces3 = _Pieces3

    End Sub

    Private Sub Load()

        If Me.lngIDMove <= 0 Then

            Me.xDateTimeIN = Nothing
            Me.xDateTimeOUT = Nothing
            Me.intIDEmployeeIN = -1
            Me.intIDEmployeeOUT = -1
            Me.intIDTerminalIN = -1
            Me.intIDTerminalOUT = -1
            Me.oJob = Nothing
            Me.intIDIncidence = 0

            Me.dblPieces1 = 0
            Me.dblPieces2 = 0
            Me.dblPieces3 = 0
        Else

            Dim rd As DbDataReader = Nothing
            Try

                rd = CreateDataReader("@SELECT# * FROM MachineJobMoves WHERE [ID] = " & Me.IDMove.ToString)
                If rd.Read Then

                    Me.intIDMachine = rd("IDMachine")

                    If Not IsDBNull(rd("InDateTime")) Then
                        Me.xDateTimeIN = rd("InDateTime")
                    Else
                        Me.xDateTimeIN = Nothing
                    End If
                    If Not IsDBNull(rd("OutDateTime")) Then
                        Me.xDateTimeOUT = rd("OutDateTime")
                    Else
                        Me.xDateTimeOUT = Nothing
                    End If

                    If Not IsDBNull(rd("InIDEmployee")) Then
                        Me.intIDEmployeeIN = rd("InIDEmployee")
                    Else
                        Me.intIDEmployeeIN = -1
                    End If
                    If Not IsDBNull(rd("OutIDEmployee")) Then
                        Me.intIDEmployeeOUT = rd("OutIDEmployee")
                    Else
                        Me.intIDEmployeeOUT = -1
                    End If

                    If Not IsDBNull(rd("InIDReader")) Then
                        Me.intIDTerminalIN = rd("InIDReader")
                    Else
                        Me.intIDTerminalIN = -1
                    End If
                    If Not IsDBNull(rd("OutIDReader")) Then
                        Me.intIDTerminalOUT = rd("OutIDReader")
                    Else
                        Me.intIDTerminalOUT = -1
                    End If

                    If Not IsDBNull(rd("IDJob")) Then
                        Me.oJob = New roJob
                        Me.oJob.ID = rd("IDJob")
                        Me.oJob.Load(Me.oLog)
                    Else
                        Me.oJob = Nothing
                    End If

                    Me.IDIncidence = Any2Integer(rd("IDIncidence"))

                    Me.dblPieces1 = Any2Double(rd("Pieces1"))
                    Me.dblPieces2 = Any2Double(rd("Pieces2"))
                    Me.dblPieces3 = Any2Double(rd("Pieces3"))

                End If
                rd.Close()
            Catch ex As DbException
            Catch ex As Exception
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

        End If

    End Sub

    Public Function Save(ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False
        oLog.logMessage(roLog.EventType.roDebug, "roJobMachineMove::Save::Saving machine job punch data (" & Me.MoveInfo() & ")")
        Try
            Dim tbMove As New DataTable("MachineJobMoves")
            Dim strSQL As String = "@SELECT# * FROM MachineJobMoves WHERE [ID] = " & Me.lngIDMove.ToString
            Dim cmd As DbCommand = CreateCommand(strSQL)
            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tbMove)

            Dim oRow As DataRow = Nothing
            If Me.lngIDMove = -1 Then
                oRow = tbMove.NewRow
            ElseIf tbMove.Rows.Count = 1 Then
                oRow = tbMove.Rows(0)
            End If

            oRow("IDMachine") = Me.intIDMachine
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

            oRow("InIDEmployee") = IIf(Me.intIDEmployeeIN <> -1, Me.intIDEmployeeIN, DBNull.Value)
            oRow("OutIDEmployee") = IIf(Me.intIDEmployeeOUT <> -1, Me.intIDEmployeeOUT, DBNull.Value)

            oRow("InIDReader") = IIf(Me.intIDTerminalIN <> -1, Me.intIDTerminalIN, DBNull.Value)
            oRow("OutIDReader") = IIf(Me.intIDTerminalOUT <> -1, Me.intIDTerminalOUT, DBNull.Value)

            If Me.oJob IsNot Nothing Then
                oRow("IDJob") = Me.oJob.ID
            End If

            oRow("IDIncidence") = Me.intIDIncidence

            oRow("Processed") = 0

            oRow("Pieces1") = Me.dblPieces1
            oRow("Pieces2") = Me.dblPieces2
            oRow("Pieces3") = Me.dblPieces3

            If Me.lngIDMove = -1 Then
                tbMove.Rows.Add(oRow)
            End If

            da.Update(tbMove)

            If Me.lngIDMove <= 0 Then
                Dim rd As DbDataReader = CreateDataReader("@SELECT# TOP 1 [ID] FROM MachineJobMoves WHERE IDMachine = " & Me.intIDMachine.ToString & " " &
                                                          "ORDER BY [ID] DESC")
                If rd.Read Then
                    Me.lngIDMove = rd("ID")
                End If
                rd.Close()
            End If

            ' Modificar DailySchedule.Status i JobStatus
            Dim xDate As DateTime
            Dim xEndDate As DateTime
            If Not Me.xDateTimeOUT.HasValue Then ' Solo actualiza el día de inicio del movimiento
                xDate = Me.xDateTimeIN.Value
                xEndDate = Me.xDateTimeIN.Value
            Else ' Actualiza el estado de todo el periodo del movimineto (más un día antes y un día después)
                xDate = Me.xDateTimeIN.Value.AddDays(-1)
                xEndDate = Me.xDateTimeOUT.Value.AddDays(1)
            End If
            bolRet = ExecuteSql(strSQL)

            While xDate <= xEndDate
                Dim tbSchedule As New DataTable("DailySchedule")
                strSQL = "@SELECT# * FROM DailyMachineSchedule " &
                         "WHERE IDMachine = " & Me.intIDMachine.ToString & " AND " &
                               "[Date] = " & Any2Time(xDate.Date).SQLSmallDateTime
                cmd = CreateCommand(strSQL)
                da = CreateDataAdapter(cmd, True)
                da.Fill(tbSchedule)
                If tbSchedule.Rows.Count = 0 Then
                    oRow = tbSchedule.NewRow
                    oRow("IDMachine") = Me.intIDMachine
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

            ' Si es la primera vez que se inicia la fase, marcar fecha inicio
            If bolRet AndAlso
               Me.DateTimeIN.HasValue AndAlso IsDBNull(Me.oJob.StartDate) Then
                bolRet = Me.oJob.UpdateStartDate(Me.DateTimeIN.Value.Date, oLog)
            End If

            If bolRet AndAlso Me.intIDMachine <> 0 Then
                bolRet = Me.oJob.UpdateMachine(Me.intIDMachine, oLog)
            End If
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJobMachineMove::Save :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJobMachineMove::Save :", ex)
        End Try

        Return bolRet

    End Function

    Public Sub GetLastMove(ByRef oLastMoveType As MovementStatus, ByRef xLastMoveDateTime As DateTime, ByRef lngLastMoveID As Long)

        Dim rd As DbDataReader = Nothing

        Try

            Dim strSQL As String = "@SELECT# TOP 1 ID, InDateTime FROM MachineJobMoves " &
                                   "WHERE IDMachine = " & Me.intIDMachine.ToString & " AND OutDateTime IS NULL " &
                                   "ORDER BY InDateTime DESC"
            rd = CreateDataReader(strSQL)

            If rd.Read Then
                oLastMoveType = MovementStatus.Begin_
                xLastMoveDateTime = CDate(rd("InDateTime"))
                lngLastMoveID = rd("ID")
            Else
                oLastMoveType = MovementStatus.Indet_
            End If
            rd.Close()

            strSQL = "@SELECT# TOP 1 ID, OutDateTime FROM MachineJobMoves " &
                     "WHERE IDMachine = " & Me.intIDMachine.ToString & " AND OutDateTime IS NOT NULL " &
                     "ORDER BY OutDateTime DESC"
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                If CDate(rd("OutDateTime")) > xLastMoveDateTime Or oLastMoveType = MovementStatus.Indet_ Then
                    oLastMoveType = MovementStatus.End_
                    xLastMoveDateTime = CDate(rd("OutDateTime"))
                    lngLastMoveID = rd("ID")
                End If
            End If
            rd.Close()
        Catch ex As DbException
            Me.oLog.logMessage(roLog.EventType.roError, "roJobMachineMove::GetLastMove :", ex)
        Catch ex As Exception
            Me.oLog.logMessage(roLog.EventType.roError, "roJobMachineMove::GetLastMove :", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

    End Sub

    Private Sub LoadIncidence()

        Dim rd As DbDataReader = Nothing

        Try

            Dim strSQL As String = "@SELECT# Name FROM JobIncidences WHERE [ID] = " & Me.intIDIncidence.ToString
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                Me.strIncidenceDesc = rd("Name")
            End If
        Catch ex As Exception
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

    End Sub

    Private Function MoveInfo() As String

        Dim strPunch As String = "IDMachine=" & Me.intIDMachine.ToString & ";"
        If Me.xDateTimeIN.HasValue Then _
            strPunch &= "InDateTime= " & Me.xDateTimeIN.Value.ToShortDateString & " " & Me.xDateTimeIN.Value.ToShortTimeString & ";"
        If Me.xDateTimeOUT.HasValue Then _
            strPunch &= "OutDateTime= " & Me.xDateTimeOUT.Value.ToShortDateString & " " & Me.xDateTimeOUT.Value.ToShortTimeString & ";"
        If Me.intIDTerminalIN <> -1 Then _
            strPunch &= "InIDReader=" & Me.intIDTerminalIN.ToString & ";"
        If Me.intIDTerminalOUT <> -1 Then _
            strPunch &= "OutIDReader=" & Me.intIDTerminalOUT.ToString & ";"
        If Me.intIDEmployeeIN <> -1 Then _
            strPunch &= "InIDEmployee=" & Me.intIDEmployeeIN.ToString & ";"
        If Me.intIDEmployeeOUT <> -1 Then _
            strPunch &= "OutIDEmployee=" & Me.intIDEmployeeOUT.ToString & ";"
        If Me.oJob IsNot Nothing Then _
            strPunch &= "IDJob=" & Me.oJob.ID & ";"
        strPunch &= "IDIncidence=" & Me.intIDIncidence & ";"
        strPunch &= "Processed=0;"
        strPunch &= "Pieces1=" & Me.dblPieces1 & ";"
        strPunch &= "Pieces2=" & Me.dblPieces2 & ";"
        strPunch &= "Pieces3=" & Me.dblPieces3 & ";"

        Return strPunch

    End Function

#End Region

End Class