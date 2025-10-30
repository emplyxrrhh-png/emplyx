Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roJobTeamMove

    Public Enum MovementStatus
        Begin_
        End_
        Indet_
    End Enum

#Region "Declarations - Constructor"

    Private oLog As roLog

    'Variables locales
    Private intIDTeam As Integer
    Private xDateTimeIN As Nullable(Of DateTime)
    Private xDateTimeOUT As Nullable(Of DateTime)
    Private intIDTerminalIN As Integer
    Private intIDTerminalOUT As Integer
    Private oJob As roJob
    Private intIDMachine As Integer
    Private intIDIncidence As Integer
    Private strIncidenceDesc As String
    Private lngIDMove As Long
    Private dblPieces1 As Double
    Private dblPieces2 As Double
    Private dblPieces3 As Double
    Private bolMulti As Boolean

    Public Sub New(ByVal _IDTeam As Integer, ByVal _IDMove As Long, ByVal _Log As roLog)

        Me.oLog = _Log

        Me.intIDTeam = _IDTeam

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

    Public Property IDTeam() As Integer
        Get
            Return intIDTeam
        End Get
        Set(ByVal value As Integer)
            intIDTeam = value
        End Set
    End Property

#End Region

#Region "Methods"

    Public Sub LoadNewBEGIN(ByVal _DateTimeIN As DateTime, ByVal _IDTerminalIN As Integer, ByVal _Job As roJob, Optional ByVal _IDMove As Long = -1, Optional ByVal _IDMachine As Integer = 0, Optional ByVal _IDIncidence As Integer = 0)

        Me.IDMove = _IDMove

        Me.xDateTimeIN = _DateTimeIN
        Me.intIDTerminalIN = _IDTerminalIN
        Me.oJob = _Job
        Me.intIDMachine = _IDMachine
        Me.IDIncidence = _IDIncidence

    End Sub

    Public Sub LoadNewEND(ByVal _DateTimeOUT As DateTime, ByVal _IDTerminalOUT As Integer, Optional ByVal _IDMove As Long = -1, Optional ByVal _Pieces1 As Double = 0, Optional ByVal _Pieces2 As Double = 0, Optional ByVal _Pieces3 As Double = 0, Optional ByVal _Multi As Boolean = False)

        ''Me.IDMove = _IDMove

        Me.xDateTimeOUT = _DateTimeOUT
        Me.intIDTerminalOUT = _IDTerminalOUT
        Me.dblPieces1 = _Pieces1
        Me.dblPieces2 = _Pieces2
        Me.dblPieces3 = _Pieces3
        Me.bolMulti = _Multi

    End Sub

    Private Sub Load()

        If Me.lngIDMove <= 0 Then

            Me.xDateTimeIN = Nothing
            Me.xDateTimeOUT = Nothing
            Me.intIDTerminalIN = -1
            Me.intIDTerminalOUT = -1
            Me.oJob = Nothing
            Me.intIDMachine = 0
            Me.intIDIncidence = 0

            Me.dblPieces1 = 0
            Me.dblPieces2 = 0
            Me.dblPieces3 = 0

            Me.bolMulti = False
        Else

            Dim rd As DbDataReader = Nothing
            Try

                rd = CreateDataReader("@SELECT# * FROM TeamJobMoves WHERE [ID] = " & Me.IDMove.ToString)
                If rd.Read Then

                    Me.intIDTeam = rd("IDTeam")

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

                    Me.intIDMachine = Any2Integer(rd("IDMachine"))
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
        oLog.logMessage(roLog.EventType.roDebug, "roJobTeamMove::Save::Saving team job punch data (" & Me.MoveInfo() & ")")

        Try
            Dim tbMove As New DataTable("TeamJobMoves")
            Dim strSQL As String = "@SELECT# * FROM TeamJobMoves WHERE [ID] = " & Me.lngIDMove.ToString
            Dim cmd As DbCommand = CreateCommand(strSQL)
            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tbMove)

            Dim oRow As DataRow = Nothing
            If Me.lngIDMove = -1 Then
                oRow = tbMove.NewRow
            ElseIf tbMove.Rows.Count = 1 Then
                oRow = tbMove.Rows(0)
            End If

            oRow("IDTeam") = Me.intIDTeam
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

            If Me.lngIDMove = -1 Then
                tbMove.Rows.Add(oRow)
            End If

            da.Update(tbMove)

            If Me.lngIDMove <= 0 Then
                Dim rd As DbDataReader = CreateDataReader("@SELECT# TOP 1 [ID] FROM TeamJobMoves WHERE IDTeam = " & Me.intIDTeam.ToString & " " &
                                                          "ORDER BY [ID] DESC")
                If rd.Read Then
                    Me.lngIDMove = rd("ID")
                End If
                rd.Close()
            End If

            bolRet = Me.ResetJobStatusForThisTeamMove(oLog)

            ' Si es la primera vez que se inicia la fase, marcar fecha inicio
            If bolRet AndAlso
               Me.DateTimeIN.HasValue AndAlso IsDBNull(Me.oJob.StartDate) Then
                bolRet = Me.oJob.UpdateStartDate(Me.DateTimeIN.Value.Date, oLog)
            End If

            If bolRet AndAlso Me.intIDMachine <> 0 Then
                bolRet = Me.oJob.UpdateMachine(Me.intIDMachine, oLog)
            End If
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJobTeamMove::Save :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJobTeamMove::Save :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Public Sub GetLastMove(ByRef oLastMoveType As MovementStatus, ByRef xLastMoveDateTime As DateTime, ByRef lngLastMoveID As Long)
        Dim rd As DbDataReader = Nothing

        Try
            Dim strSQL As String = "@SELECT# TOP 1 ID, InDateTime FROM TeamJobMoves " &
                                   "WHERE IDTeam = " & Me.intIDTeam.ToString & " AND OutDateTime IS NULL " &
                                   "ORDER BY InDateTime DESC"
            rd = CreateDataReader(strSQL)

            If rd.Read Then
                oLastMoveType = MovementStatus.Begin_
                xLastMoveDateTime = CDate(rd("InDateTime"))
                lngLastMoveID = rd("ID")
            Else
                oLastMoveType = MovementStatus.Indet_
                lngLastMoveID = -1
            End If
            rd.Close()

            strSQL = "@SELECT# TOP 1 ID, OutDateTime FROM TeamJobMoves " &
                     "WHERE IDTeam = " & Me.intIDTeam.ToString & " AND OutDateTime IS NOT NULL " &
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
            Me.oLog.logMessage(roLog.EventType.roError, "roJobTeamMove::GetLastMove :", ex)
        Catch ex As Exception
            Me.oLog.logMessage(roLog.EventType.roError, "roJobTeamMove::GetLastMove :", ex)
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

    Public Function ResetJobStatusForThisTeamMove(ByVal oLog As roLog) As Boolean
        '
        ' Marca como no procesado los dias del DailySchedule de todos los empleados a
        '  los que afecta este movimiento de producción de equipo.
        '
        Dim bolRet As Boolean = False
        Dim xINDateTime As DateTime
        Dim xOUTDateTime As DateTime
        Dim sSQL As String

        Try

            ' Si el movimiento no esta completo, no hace nada
            If Not Me.DateTimeIN.HasValue And Not Me.DateTimeOUT.HasValue Then Exit Function

            If Not Me.DateTimeIN.HasValue Then
                xINDateTime = Me.DateTimeOUT.Value
            Else
                xINDateTime = Me.DateTimeIN.Value
            End If
            If Not Me.DateTimeOUT.HasValue Then
                xOUTDateTime = Me.DateTimeIN.Value
            Else
                xOUTDateTime = Me.DateTimeOUT.Value
            End If

            ' Como versión preliminar, marcamos como no calculados desde el dia antes hasta el dia
            '  despues, asi nos curamos en salud.
            xINDateTime = xINDateTime.AddDays(-1)
            xOUTDateTime = xOUTDateTime.AddDays(1)

            Dim xDate As DateTime
            Dim xEndDate As DateTime = xOUTDateTime
            Dim cmd As DbCommand
            Dim da As DbDataAdapter
            Dim oRow As DataRow

            ' Obtenemos todos los empleados que están en el equipo durante este movimiento
            sSQL = "@SELECT# IDEmployee,BeginDate,IsNull(FinishDate," & Any2Time(Now).SQLDateTime & ") AS MyFinishDate FROM EmployeeTeams WHERE " _
                    & "IDTeam=" & Me.IDTeam & " AND " _
                    & "BeginDate<" & Any2Time(xOUTDateTime).SQLDateTime &
                    " AND IsNull(FinishDate," & Any2Time(Now).SQLDateTime & ")>" & Any2Time(xINDateTime).SQLDateTime & " ORDER BY BeginDate"
            Dim tb As DataTable = CreateDataTable(sSQL, )
            For Each oEmpRow As DataRow In tb.Rows

                xDate = xINDateTime

                While xDate <= xEndDate
                    Dim tbSchedule As New DataTable("DailySchedule")
                    sSQL = "@SELECT# * FROM DailySchedule " &
                           "WHERE IDEmployee = " & oEmpRow("IDEmployee") & " AND " &
                                 "[Date] = " & Any2Time(xDate.Date).SQLSmallDateTime
                    cmd = CreateCommand(sSQL)
                    da = CreateDataAdapter(cmd, True)
                    da.Fill(tbSchedule)
                    If tbSchedule.Rows.Count = 0 Then
                        oRow = tbSchedule.NewRow
                        oRow("IDEmployee") = oEmpRow("IDEmployee")
                        oRow("Date") = xDate.Date
                    Else
                        oRow = tbSchedule.Rows(0)
                    End If
                    oRow("JobStatus") = 0
                    If tbSchedule.Rows.Count = 0 Then tbSchedule.Rows.Add(oRow)
                    da.Update(tbSchedule)
                    xDate = xDate.AddDays(1)
                End While

            Next

            bolRet = True
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJobTeamMove::ResetJobStatusForThisTeamMove :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJobTeamMove::ResetJobStatusForThisTeamMove :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Private Function MoveInfo() As String

        Dim strPunch As String = "IDTeam=" & Me.intIDTeam.ToString & ";"
        If Me.xDateTimeIN.HasValue Then _
            strPunch &= "InDateTime= " & Me.xDateTimeIN.Value.ToShortDateString & " " & Me.xDateTimeIN.Value.ToShortTimeString & ";"
        If Me.xDateTimeOUT.HasValue Then _
            strPunch &= "OutDateTime= " & Me.xDateTimeOUT.Value.ToShortDateString & " " & Me.xDateTimeOUT.Value.ToShortTimeString & ";"
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

        Return strPunch

    End Function

#End Region

End Class