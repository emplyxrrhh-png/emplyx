Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roMove

    Public Enum MovementStatus
        Indet_
        In_
        Out_
    End Enum

#Region "Declarations - Constructor"

    Private intIDEmployee As Integer
    Private xDateTimeIN As Nullable(Of DateTime)
    Private xDateTimeOUT As Nullable(Of DateTime)

    Private intIDTerminalIN As Integer
    Private intIDTerminalOUT As Integer

    Private intIDCauseIN As Integer
    Private intIDCauseOUT As Integer

    Private xShiftDate As Nullable(Of Date)
    Private lngIDMove As Long

    Private intIDZoneIN As Integer
    Private intIDZoneOUT As Integer

    Private intReaderTypeIN As Integer
    Private intReaderTypeOUT As Integer

    Private oCaptureIN As Image
    Private oCaptureOUT As Image

    Private bolIsNotReliableIN As Boolean
    Private bolIsNotReliableOUT As Boolean

    Public Sub New(ByVal _IDEmployee As Integer, ByVal _IDMove As Long)

        Me.intIDEmployee = _IDEmployee

        Me.IDMove = _IDMove

    End Sub

#End Region

#Region "Properties"

    Public Property IDEmployee() As Integer
        Get
            Return Me.intIDEmployee
        End Get
        Set(ByVal value As Integer)
            Me.intIDEmployee = value
        End Set
    End Property

    Public Property DateTimeIN() As Nullable(Of DateTime)
        Get
            Return Me.xDateTimeIN
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            Me.xDateTimeIN = value
        End Set
    End Property
    Public Property DateTimeOUT() As Nullable(Of DateTime)
        Get
            Return Me.xDateTimeOUT
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            Me.xDateTimeOUT = value
        End Set
    End Property

    Public Property IDTerminalIN() As Integer
        Get
            Return Me.intIDTerminalIN
        End Get
        Set(ByVal value As Integer)
            Me.intIDTerminalIN = value
        End Set
    End Property
    Public Property IDTerminalOUT() As Integer
        Get
            Return Me.intIDTerminalOUT
        End Get
        Set(ByVal value As Integer)
            Me.intIDTerminalOUT = value
        End Set
    End Property

    Public Property IDCauseIN() As Integer
        Get
            Return Me.intIDCauseIN
        End Get
        Set(ByVal value As Integer)
            Me.intIDCauseIN = value
        End Set
    End Property
    Public Property IDCauseOUT() As Integer
        Get
            Return Me.intIDCauseOUT
        End Get
        Set(ByVal value As Integer)
            Me.intIDCauseOUT = value
        End Set
    End Property

    Public Property ShiftDate() As Nullable(Of DateTime)
        Get
            Return Me.xShiftDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            Me.xShiftDate = value
        End Set
    End Property

    Public Property IDMove() As Long
        Get
            Return Me.lngIDMove
        End Get
        Set(ByVal value As Long)
            Me.lngIDMove = value
            Me.Load()
        End Set
    End Property

    Public Property IDZoneIN() As Integer
        Get
            Return Me.intIDZoneIN
        End Get
        Set(ByVal value As Integer)
            Me.intIDZoneIN = value
        End Set
    End Property
    Public Property IDZoneOUT() As Integer
        Get
            Return Me.intIDZoneOUT
        End Get
        Set(ByVal value As Integer)
            Me.intIDZoneOUT = value
        End Set
    End Property

    Public Property ReaderTypeIN() As Integer
        Get
            Return Me.intReaderTypeIN
        End Get
        Set(ByVal value As Integer)
            Me.intReaderTypeIN = value
        End Set
    End Property
    Public Property ReaderTypeOUT() As Integer
        Get
            Return Me.intReaderTypeOUT
        End Get
        Set(ByVal value As Integer)
            Me.intReaderTypeOUT = value
        End Set
    End Property

    Public Property CaptureIN() As Image
        Get
            Return Me.oCaptureIN
        End Get
        Set(ByVal value As Image)
            Me.oCaptureIN = value
        End Set
    End Property

    Public Property CaptureOUT() As Image
        Get
            Return Me.oCaptureOUT
        End Get
        Set(ByVal value As Image)
            Me.oCaptureOUT = value
        End Set
    End Property

    Public Property IsNotReliableIN() As Nullable(Of Boolean)
        Get
            Return Me.bolIsNotReliableIN
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            If value.HasValue Then
                Me.bolIsNotReliableIN = value.Value
            Else
                Me.bolIsNotReliableIN = False
            End If
        End Set
    End Property

    Public Property IsNotReliableOUT() As Nullable(Of Boolean)
        Get
            Return Me.bolIsNotReliableOUT
        End Get
        Set(ByVal value As Nullable(Of Boolean))
            If value.HasValue Then
                Me.bolIsNotReliableOUT = value.Value
            Else
                Me.bolIsNotReliableOUT = False
            End If
        End Set
    End Property

#End Region

#Region "Events"

    Public Event OnLoadNewIN(ByVal oSender As roMove)

    Public Event OnLoadNewOUT(ByVal oSender As roMove)

    Public Event OnLoadIN(ByVal oSender As roMove)

    Public Event OnLoadOUT(ByVal oSender As roMove)

#End Region

#Region "Methods"

    Public Sub LoadNewIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, Optional ByVal _IDMove As Long = -1, Optional ByVal _IDCauseIN As Integer = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False)

        Me.IDMove = _IDMove

        Me.xDateTimeIN = _DateTimeIN
        Me.xShiftDate = _DateTimeIN
        Me.intIDTerminalIN = _IDTerminalIN
        Me.intIDCauseIN = _IDCauseIN
        Me.intIDZoneIN = _IDZoneIN
        Me.intReaderTypeIN = _ReaderTypeIN
        Me.oCaptureIN = _CaptureIN
        Me.bolIsNotReliableIN = _IsNotReliableIN

        RaiseEvent OnLoadNewIN(Me)

    End Sub

    Public Sub LoadNewOUT(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _IDMove As Long = -1, Optional ByVal _IDCauseOUT As Integer = -1, Optional ByVal _IDZoneOUT As Integer = -1, Optional ByVal _ReaderTypeOUT As Integer = -1, Optional ByVal _CaptureOUT As Image = Nothing, Optional ByVal _IsNotReliableOUT As Boolean = False)

        Me.IDMove = _IDMove

        Me.xDateTimeOUT = _DateTimeOUT
        If Not Me.xShiftDate.HasValue Then Me.xShiftDate = _DateTimeOUT
        Me.intIDTerminalOUT = _IDTerminalOUT
        Me.intIDCauseOUT = _IDCauseOUT
        Me.intIDZoneOUT = _IDZoneOUT
        Me.intReaderTypeOUT = _ReaderTypeOUT
        Me.oCaptureOUT = _CaptureOUT
        Me.bolIsNotReliableOUT = _IsNotReliableOUT

        RaiseEvent OnLoadNewOUT(Me)

    End Sub

    Public Sub LoadIN(ByVal _DateTimeIN As Nullable(Of DateTime), ByVal _IDTerminalIN As Integer, Optional ByVal _IDCauseIN As Integer = -1, Optional ByVal _IDZoneIN As Integer = -1, Optional ByVal _ReaderTypeIN As Integer = -1, Optional ByVal _CaptureIN As Image = Nothing, Optional ByVal _IsNotReliableIN As Boolean = False)

        Me.xDateTimeIN = _DateTimeIN
        Me.xShiftDate = _DateTimeIN
        Me.intIDTerminalIN = _IDTerminalIN
        Me.intIDCauseIN = _IDCauseIN
        Me.intIDZoneIN = _IDZoneIN
        Me.intReaderTypeIN = _ReaderTypeIN
        Me.oCaptureIN = _CaptureIN
        Me.bolIsNotReliableIN = _IsNotReliableIN

        RaiseEvent OnLoadIN(Me)

    End Sub

    Public Sub LoadOUT(ByVal _DateTimeOUT As Nullable(Of DateTime), ByVal _IDTerminalOUT As Integer, Optional ByVal _IDCauseOUT As Integer = -1, Optional ByVal _IDZoneOUT As Integer = -1, Optional ByVal _ReaderTypeOUT As Integer = -1, Optional ByVal _CaptureOUT As Image = Nothing, Optional ByVal _IsNotReliableOUT As Boolean = False)

        Me.xDateTimeOUT = _DateTimeOUT
        If Not Me.xShiftDate.HasValue Then Me.xShiftDate = _DateTimeOUT
        Me.intIDTerminalOUT = _IDTerminalOUT
        Me.intIDCauseOUT = _IDCauseOUT
        Me.intIDZoneOUT = _IDZoneOUT
        Me.intReaderTypeOUT = _ReaderTypeOUT
        Me.oCaptureOUT = _CaptureOUT
        Me.bolIsNotReliableOUT = _IsNotReliableOUT

        RaiseEvent OnLoadOUT(Me)

    End Sub

    Private Sub Load()

        If Me.lngIDMove <= 0 Then

            Me.xDateTimeIN = Nothing
            Me.xDateTimeOUT = Nothing
            Me.xShiftDate = Nothing

            Me.intIDTerminalIN = -1
            Me.intIDTerminalOUT = -1

            Me.intIDCauseIN = -1
            Me.intIDCauseOUT = -1

            Me.intIDZoneIN = -1
            Me.intIDZoneOUT = -1

            Me.intReaderTypeIN = -1
            Me.intReaderTypeOUT = -1

            Me.oCaptureIN = Nothing
            Me.oCaptureOUT = Nothing

            Me.bolIsNotReliableIN = False
            Me.bolIsNotReliableOUT = False
        Else

            Dim rd As DbDataReader = Nothing
            Try

                rd = CreateDataReader("@SELECT# * FROM Moves WHERE [ID] = " & Me.IDMove.ToString)
                If rd.Read Then

                    Me.intIDEmployee = rd("IDEmployee")
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

                    If Not IsDBNull(rd("InIDCause")) Then
                        Me.intIDCauseIN = rd("InIDCause")
                    Else
                        Me.intIDCauseIN = -1
                    End If
                    If Not IsDBNull(rd("OutIDCause")) Then
                        Me.intIDCauseOUT = rd("OutIDCause")
                    Else
                        Me.intIDCauseOUT = -1
                    End If

                    If Not IsDBNull(rd("ShiftDate")) Then
                        Me.xShiftDate = rd("ShiftDate")
                    Else
                        Me.xShiftDate = Nothing
                    End If

                    If Not IsDBNull(rd("InIDZone")) Then
                        Me.intIDZoneIN = rd("InIDZone")
                    Else
                        Me.intIDZoneIN = -1
                    End If
                    If Not IsDBNull(rd("OutIDZone")) Then
                        Me.intIDZoneOUT = rd("OutIDZone")
                    Else
                        Me.intIDZoneOUT = -1
                    End If

                    If Not IsDBNull(rd("InIDReaderType")) Then
                        Me.intReaderTypeIN = rd("InIDReaderType")
                    Else
                        Me.intReaderTypeIN = -1
                    End If
                    If Not IsDBNull(rd("OutIDReaderType")) Then
                        Me.intReaderTypeOUT = rd("OutIDReaderType")
                    Else
                        Me.intReaderTypeOUT = -1
                    End If

                    If Not IsDBNull(rd("IsNotReliableIN")) Then
                        Me.bolIsNotReliableIN = rd("IsNotReliableIN")
                    Else
                        Me.bolIsNotReliableIN = False
                    End If
                    If Not IsDBNull(rd("IsNotReliableOUT")) Then
                        Me.bolIsNotReliableOUT = rd("IsNotReliableOUT")
                    Else
                        Me.bolIsNotReliableOUT = False
                    End If

                End If
                rd.Close()

                Me.oCaptureIN = Nothing
                Me.oCaptureOUT = Nothing

                rd = CreateDataReader("@SELECT# * FROM MovesCaptures WHERE IDMove = " & Me.IDMove.ToString)
                If rd.Read Then

                    If Not IsDBNull(rd("InCapture")) Then
                        Dim bImage As Byte() = CType(rd("InCapture"), Byte())
                        Dim ms As MemoryStream = New MemoryStream(bImage)
                        Me.oCaptureIN = CType(Image.FromStream(ms), Bitmap)
                    End If
                    If Not IsDBNull(rd("OutCapture")) Then
                        Dim bImage As Byte() = CType(rd("OutCapture"), Byte())
                        Dim ms As MemoryStream = New MemoryStream(bImage)
                        Me.oCaptureOUT = CType(Image.FromStream(ms), Bitmap)
                    End If

                End If
                rd.Close()
            Catch ex As DbException
            Catch ex As Exception
            Finally
                If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

            End Try

        End If

    End Sub

    Public Function Save(ByVal oLog As roLog, Optional ByVal bolAutomaticBeginJobCheck As Boolean = True, Optional ByVal bolAutomaticFinishJobCheck As Boolean = True) As Boolean
        Dim bolRet As Boolean = False
        oLog.logMessage(roLog.EventType.roDebug, "roMove::Save::Saving punch data (" & Me.MoveInfo() & ")")

        Try
            Dim tbMove As New DataTable("Moves")
            Dim strSQL As String = "@SELECT# * FROM Moves WHERE [ID] = " & Me.lngIDMove.ToString
            Dim cmd As DbCommand = CreateCommand(strSQL)
            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tbMove)

            Dim oRow As DataRow = Nothing
            If Me.lngIDMove <= 0 Then
                oRow = tbMove.NewRow
            ElseIf tbMove.Rows.Count = 1 Then
                oRow = tbMove.Rows(0)
            End If

            oRow("IDEmployee") = Me.intIDEmployee
            If Me.xDateTimeIN.HasValue Then
                xDateTimeIN = CType(xDateTimeIN, Date).Subtract(New TimeSpan(0, 0, CType(xDateTimeIN, Date).Second))
                oRow("InDateTime") = Me.xDateTimeIN.Value
            Else
                oRow("InDateTime") = DBNull.Value
            End If
            If Me.xDateTimeOUT.HasValue Then
                xDateTimeOUT = CType(xDateTimeOUT, Date).Subtract(New TimeSpan(0, 0, CType(xDateTimeOUT, Date).Second))
                oRow("OutDateTime") = Me.xDateTimeOUT.Value
            Else
                oRow("OutDateTime") = DBNull.Value
            End If

            oRow("InIDReader") = IIf(Me.intIDTerminalIN <> -1, Me.intIDTerminalIN, DBNull.Value)
            oRow("OutIDReader") = IIf(Me.intIDTerminalOUT <> -1, Me.intIDTerminalOUT, DBNull.Value)

            oRow("InIDCause") = IIf(Me.intIDCauseIN <> -1, Me.intIDCauseIN, DBNull.Value)
            oRow("OutIDCause") = IIf(Me.intIDCauseOUT <> -1, Me.intIDCauseOUT, DBNull.Value)

            If Me.xShiftDate.HasValue Then
                oRow("ShiftDate") = roTypes.DateTime2Date(Me.xShiftDate.Value)
            Else
                oRow("ShiftDate") = DBNull.Value
            End If

            oRow("InIDZone") = IIf(Me.intIDZoneIN <> -1, Me.intIDZoneIN, DBNull.Value)
            oRow("OutIDZone") = IIf(Me.intIDZoneOUT <> -1, Me.intIDZoneOUT, DBNull.Value)

            oRow("InIDReaderType") = IIf(Me.intReaderTypeIN <> -1, Me.intReaderTypeIN, DBNull.Value)
            oRow("OutIDReaderType") = IIf(Me.intReaderTypeOUT <> -1, Me.intReaderTypeOUT, DBNull.Value)

            oRow("IsNotReliableIN") = IIf(Me.bolIsNotReliableIN, 1, DBNull.Value)
            oRow("IsNotReliableOUT") = IIf(Me.bolIsNotReliableOUT, 1, DBNull.Value)

            If Me.lngIDMove <= 0 Then
                tbMove.Rows.Add(oRow)
            End If

            da.Update(tbMove)

            If Me.lngIDMove <= 0 Then
                Dim rd As DbDataReader = CreateDataReader("@SELECT# TOP 1 [ID] FROM Moves WHERE IDEmployee = " & Me.intIDEmployee.ToString & " " &
                                                          "ORDER BY [ID] DESC")
                If rd.Read Then
                    Me.lngIDMove = rd("ID")
                End If
                rd.Close()
            End If

            ' Modificar DailySchedule.Status i JobStatus
            Dim xDailyDate As DateTime
            If Me.xShiftDate.HasValue Then
                xDailyDate = Me.xShiftDate.Value
            ElseIf Me.xDateTimeOUT.HasValue Then
                xDailyDate = Me.xDateTimeOUT
            Else
                xDailyDate = Me.xDateTimeIN
            End If

            Dim tbSchedule As New DataTable("DailySchedule")
            strSQL = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                       "[Date] = " & Any2Time(xDailyDate.Date).SQLSmallDateTime
            cmd = CreateCommand(strSQL)
            da = CreateDataAdapter(cmd, True)
            da.Fill(tbSchedule)

            If tbSchedule.Rows.Count = 0 Then
                oRow = tbSchedule.NewRow
                oRow("IDEmployee") = Me.intIDEmployee
                oRow("Date") = xDailyDate.Date
            Else
                oRow = tbSchedule.Rows(0)
            End If
            oRow("Status") = 0
            oRow("JobStatus") = 0

            If tbSchedule.Rows.Count = 0 Then tbSchedule.Rows.Add(oRow)
            da.Update(tbSchedule)

            'bolRet = ExecuteSql("@UPDATE# DailySchedule " & _
            '                    "SET Status = 0, JobStatus = 0 " & _
            '                    "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " & _
            '                          "[Date] = CONVERT(smalldatetime, '" & Format(xDailyDate, "dd/MM/yyyy") & "', 103) ")

            ' Grabar imagen
            bolRet = Me.SaveCapture(oLog)

            ' Iniciar automáticamente último trabajo
            If bolAutomaticBeginJobCheck Then
                If Me.xDateTimeIN.HasValue And Not Me.xDateTimeOUT.HasValue Then ' Es una entrada
                    Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                    If oSettings.GetVTSetting(eKeys.AutomaticBeginJob) Then ' Está configurada la opción inicios automáticos de producción
                        Dim oEmployee As New roEmployee
                        oEmployee.ID = Me.intIDEmployee
                        If oEmployee.Load(oLog) Then
                            If oEmployee.EmployeeType = "J" Then ' El empleado es de producción
                                Dim oJobMove As New roJobMove(Me.intIDEmployee, -1, oLog)

                                ' Obtener el último movimiento de producción
                                Dim oLastJobMoveType As roJobMove.MovementStatus
                                Dim xLastJobMoveDate As Date
                                Dim lngLastMoveID As Long
                                oJobMove.GetLastMove(oLastJobMoveType, xLastJobMoveDate, lngLastMoveID)

                                If oLastJobMoveType <> roJobMove.MovementStatus.Indet_ Then
                                    Dim oLastJobMove As New roJobMove(Me.intIDEmployee, lngLastMoveID, oLog)
                                    Dim bolBegin As Boolean = False
                                    If oLastJobMoveType = roJobMove.MovementStatus.Begin_ Then
                                        bolBegin = (oLastJobMove.DateTimeIN.Value < Me.xDateTimeIN.Value)
                                    Else
                                        bolBegin = (oLastJobMove.DateTimeOUT.Value < Me.xDateTimeIN.Value)
                                    End If
                                    If bolBegin Then
                                        If oLastJobMove.Job.EndDate Is Nothing OrElse IsDBNull(oLastJobMove.Job.EndDate) OrElse Not IsDate(oLastJobMove.Job.EndDate) Then
                                            'Solo creo nuevo inicio de Job si el último no estaba cerrado
                                            oJobMove.LoadNewBEGIN(Me.xDateTimeIN.Value, Me.intIDTerminalIN, oLastJobMove.Job, , oLastJobMove.IDMachine, oLastJobMove.IDIncidence)
                                            oJobMove.Save(oLog)
                                        Else
                                            oLog.logMessage(roLog.EventType.roDebug, "roMove::Save: No automatic job init applied. Last job is closed !!!")
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If

            ' Finalizar automáticamente el trabajo activo
            If bolAutomaticFinishJobCheck Then
                If Me.xDateTimeOUT.HasValue Then ' Es una salida
                    Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                    If oSettings.GetVTSetting(eKeys.AutomaticFinishJob) Then ' Está configurada la opción finales automáticos de producción
                        Dim oEmployee As New roEmployee
                        oEmployee.ID = Me.intIDEmployee
                        If oEmployee.Load(oLog) Then
                            If oEmployee.EmployeeType = "J" Then ' El empleado es de producción
                                Dim oJobMove As New roJobMove(Me.intIDEmployee, -1, oLog)

                                ' Obtener el último movimiento de producción
                                Dim oLastJobMoveType As roJobMove.MovementStatus
                                Dim xLastJobMoveDate As Date
                                Dim lngLastMoveID As Long
                                oJobMove.GetLastMove(oLastJobMoveType, xLastJobMoveDate, lngLastMoveID)

                                ' Si el último movimiento es un inicio
                                If oLastJobMoveType = roJobMove.MovementStatus.Begin_ Then
                                    Dim oLastJobMove As New roJobMove(Me.intIDEmployee, lngLastMoveID, oLog)
                                    If (oLastJobMove.DateTimeIN.Value < Me.xDateTimeOUT.Value) Then
                                        oJobMove = New roJobMove(Me.intIDEmployee, lngLastMoveID, oLog)
                                        oJobMove.LoadNewEND(Me.xDateTimeOUT.Value, Me.intIDTerminalOUT, lngLastMoveID)
                                        oJobMove.Save(oLog)
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            End If
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roMove::Save :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roMove::Save :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Private Function SaveCapture(ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False

        oLog.logMessage(roLog.EventType.roDebug, "roMove::Save::Saving punch capture data (" & Me.MoveCaptureInfo() & ")")

        Try

            ' Eliminar imágenes antiguas
            Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
            Dim intDays As Integer = oSettings.GetVTSetting(eKeys.AuditDays)
            Dim xDate As DateTime = Now
            xDate = xDate.AddDays(-intDays)
            Dim strSQL As String
            ''strSQL  = _
            ''    "@DELETE# FROM MovesCaptures " & _
            ''    "WHERE IDMove IN (@SELECT# [ID] FROM Moves " & _
            ''                     "WHERE ISNULL(InDateTime, " & Any2Time(xDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(xDate).SQLDateTime & " AND " & _
            ''                           "ISNULL(OutDateTime, " & Any2Time(xDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(xDate).SQLDateTime & ")"
            ''ExecuteSql(strSQL)

            Dim tbCaptures As New DataTable("MovesCaptures")
            strSQL = "@SELECT# * FROM MovesCaptures WHERE IDMove = " & Me.lngIDMove.ToString
            Dim cmd As DbCommand = CreateCommand(strSQL)
            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tbCaptures)

            Dim oRow As DataRow = Nothing
            Dim bolNewRow As Boolean = False
            If tbCaptures.Rows.Count = 0 Then
                oRow = tbCaptures.NewRow
                bolNewRow = True
                oRow("IDMove") = Me.lngIDMove
            Else
                oRow = tbCaptures.Rows(0)
            End If

            If Me.oCaptureIN IsNot Nothing Then
                oRow("InCapture") = roTypes.Image2Bytes(Me.oCaptureIN)
            Else
                oRow("InCapture") = DBNull.Value
            End If
            If Me.oCaptureOUT IsNot Nothing Then
                oRow("OutCapture") = roTypes.Image2Bytes(Me.oCaptureOUT)
            Else
                oRow("OutCapture") = DBNull.Value
            End If

            If bolNewRow Then
                tbCaptures.Rows.Add(oRow)
            End If

            da.Update(tbCaptures)

            ''Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
            ''Dim strPath As String = Path.Combine(oSettings.GetVTSetting(roSettings.eKeys.Readings), "Terminal" + Me.intIDTerminalIN.ToString)
            ''If Me.oCaptureIN IsNot Nothing And Me.xDateTimeIN.HasValue Then
            ''    Dim b As New Bitmap(Me.oCaptureIN)
            ''    b.Save(strPath & "\CaptureIN_" & Format(Me.xDateTimeIN.Value, "yyyyMMddHHmm") & ".jpg", Imaging.ImageFormat.Jpeg)
            ''End If
            ''If Me.oCaptureOUT IsNot Nothing And Me.xDateTimeOUT.HasValue Then
            ''    Dim b As New Bitmap(Me.oCaptureOUT)
            ''    b.Save(strPath & "\CaptureOUT_" & Format(Me.xDateTimeOUT.Value, "yyyyMMddHHmm") & ".jpg", Imaging.ImageFormat.Jpeg)
            ''End If

            If Me._HistoryThread Is Nothing Then
                Me._HistoryDate = xDate
                Me._HistoryLog = oLog
                Me._HistoryThread = New Threading.Thread(AddressOf DeleteMovesCapturesHistory)
                Me._HistoryThread.Start()
            End If

            bolRet = True
        Catch Ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roMove::SaveCapture :", Ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roMove::SaveCapture :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Public Function Delete(ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim sSql As String = "@DELETE# FROM MovesCaptures WHERE IDMove = " & Me.IDMove.ToString
            bolRet = ExecuteSql(sSql)

            If bolRet Then

                sSql = "@DELETE# FROM Moves WHERE [ID] = " & Me.IDMove.ToString
                bolRet = ExecuteSql(sSql)
                If bolRet Then
                    Me.IDMove = 0
                End If

            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roMove::Delete :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roMove::Delete :", ex)
        Finally

        End Try

        Return bolRet

    End Function

    Public Sub GetLastMove(ByRef oLastMoveType As MovementStatus, ByRef xLastMoveDateTime As DateTime, ByRef lngLastMoveID As Long, ByVal oLog As roLog)

        Dim rd As DbDataReader = Nothing

        Try

            Dim strSQL As String = "@SELECT# TOP 1 ID, InDateTime FROM Moves " &
                                   "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND OutDateTime IS NULL AND InDateTime IS NOT NULL " &
                                   "ORDER BY InDateTime DESC"
            rd = CreateDataReader(strSQL)

            If rd.Read Then
                oLastMoveType = MovementStatus.In_
                xLastMoveDateTime = CDate(rd("InDateTime"))
                lngLastMoveID = rd("ID")
            Else
                oLastMoveType = MovementStatus.Indet_
                lngLastMoveID = -1
            End If
            rd.Close()

            strSQL = "@SELECT# TOP 1 ID, OutDateTime FROM Moves " &
                     "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND OutDateTime IS NOT NULL " &
                     "ORDER BY OutDateTime DESC"
            rd = CreateDataReader(strSQL)
            If rd.Read Then
                If CDate(rd("OutDateTime")) > xLastMoveDateTime Or oLastMoveType = MovementStatus.Indet_ Then
                    oLastMoveType = MovementStatus.Out_
                    xLastMoveDateTime = CDate(rd("OutDateTime"))
                    lngLastMoveID = rd("ID")
                End If
            End If
            rd.Close()
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roMove::GetLastMove :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roMove::GetLastMove :", ex)
        Finally
            If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()

        End Try

    End Sub

    Private Function MoveInfo() As String

        Dim strPunch As String = "IDEmployee=" & Me.intIDEmployee.ToString & ";"
        If Me.xDateTimeIN.HasValue Then _
            strPunch &= "InDateTime= " & Me.xDateTimeIN.Value.ToShortDateString & " " & Me.xDateTimeIN.Value.ToShortTimeString & ";"
        If Me.xDateTimeOUT.HasValue Then _
            strPunch &= "OutDateTime= " & Me.xDateTimeOUT.Value.ToShortDateString & " " & Me.xDateTimeOUT.Value.ToShortTimeString & ";"
        If Me.intIDTerminalIN <> -1 Then _
            strPunch &= "InIDReader=" & Me.intIDTerminalIN.ToString & ";"
        If Me.intIDTerminalOUT <> -1 Then _
            strPunch &= "OutIDReader=" & Me.intIDTerminalOUT.ToString & ";"
        If Me.intIDCauseIN <> -1 Then _
            strPunch &= "InIDCause=" & Me.intIDCauseIN.ToString & ";"
        If Me.intIDCauseOUT <> -1 Then _
            strPunch &= "OutIDCause=" & Me.intIDCauseOUT.ToString & ";"
        If Me.xShiftDate.HasValue Then _
            strPunch &= "ShiftDate=" & Me.xShiftDate.Value.ToShortDateString & ";"
        If Me.intIDZoneIN <> -1 Then _
            strPunch &= "InIDZone=" & Me.intIDZoneIN.ToString & ";"
        If Me.intIDZoneOUT <> -1 Then _
            strPunch &= "OutIDZone=" & Me.intIDZoneOUT.ToString & ";"
        If Me.intReaderTypeIN <> -1 Then _
            strPunch &= "InIDReaderType=" & Me.intReaderTypeIN.ToString & ";"
        If Me.intReaderTypeOUT <> -1 Then _
            strPunch &= "OutIDReaderType=" & Me.intReaderTypeOUT.ToString & ";"
        If Me.bolIsNotReliableIN Then _
            strPunch &= "IsNotReliableIN=1;"
        If Me.bolIsNotReliableOUT Then _
            strPunch &= "IsNotReliableOUT=1;"

        Return strPunch

    End Function

    Private Function MoveCaptureInfo() As String
        Dim strRet As String = ""
        If Me.oCaptureIN IsNot Nothing Then strRet &= "CaptureIN.Size=" & Me.oCaptureIN.Height & "x" & Me.oCaptureIN.Width & ";"
        If Me.oCaptureOUT IsNot Nothing Then strRet &= "CaptureOUT.Size=" & Me.oCaptureOUT.Height & "x" & Me.oCaptureOUT.Width & ";"
        Return strRet
    End Function

    Private _HistoryThread As Threading.Thread = Nothing
    Private _HistoryDate As Date = Nothing
    Private _HistoryLog As roLog = Nothing

    Private Sub DeleteMovesCapturesHistory()

        Try

            Dim strSQL As String
            ''strSQL = _
            ''    "@DELETE# FROM MovesCaptures " & _
            ''    "WHERE IDMove IN (@SELECT# [ID] FROM Moves " & _
            ''                     "WHERE ISNULL(InDateTime, " & Any2Time(_HistoryDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(_HistoryDate).SQLDateTime & " AND " & _
            ''                           "ISNULL(OutDateTime, " & Any2Time(_HistoryDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(_HistoryDate).SQLDateTime & ")"

            strSQL = "@SELECT# DISTINCT [ID] FROM Moves " &
                     "WHERE ISNULL(InDateTime, " & Any2Time(_HistoryDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(_HistoryDate).SQLDateTime & " AND " &
                           "ISNULL(OutDateTime, " & Any2Time(_HistoryDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(_HistoryDate).SQLDateTime & " AND " &
                           "Moves.ID IN (@SELECT# MovesCaptures.IDMove FROM MovesCaptures)"
            Dim tbMoves As DataTable = CreateDataTable(strSQL, )
            If tbMoves IsNot Nothing Then
                For Each oRow As DataRow In tbMoves.Rows
                    strSQL = "@DELETE# FROM MovesCaptures WHERE IDMove = " & oRow("ID")
                    ExecuteSql(strSQL)
                Next
            End If
        Catch Ex As DbException
            _HistoryLog.logMessage(roLog.EventType.roError, "roMove::DeleteMovesCapturesHistory :", Ex)
        Catch ex As Exception
            _HistoryLog.logMessage(roLog.EventType.roError, "roMove::DeleteMovesCapturesHistory :", ex)
        Finally

        End Try

        Me._HistoryThread = Nothing

    End Sub

#End Region

End Class