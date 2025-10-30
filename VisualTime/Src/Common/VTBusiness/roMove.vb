Imports System.Data.Common
Imports System.Drawing
Imports System.IO
Imports System.Runtime.Serialization
Imports System.Xml.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes
Imports Robotics.Security.Base

Namespace Move

    <DataContract>
    Public Class roMove

#Region "Declarations - Constructor"

        Private oState As roMoveState

        Private intIDEmployee As Integer
        Private xDateTimeIN As Nullable(Of DateTime)
        Private xDateTimeOUT As Nullable(Of DateTime)

        Private intIDTerminalIN As Integer
        Private intIDTerminalOUT As Integer

        Private strLocalizationIN As String
        Private strLocalizationOUT As Integer

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

        Public Sub New()

            Me.oState = New roMoveState()

            Me.IDMove = 0
            Me.Load()

        End Sub

        Public Sub New(ByVal _State As roMoveState)
            Me.oState = _State
            Me.IDMove = 0
            Me.Load()
        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _IDMove As Long, ByVal _State As roMoveState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee

            Me.IDMove = _IDMove
            Me.Load()

        End Sub

#End Region

#Region "Properties"

        <DataMember>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property

        <DataMember>
        Public Property DateTimeIN() As Nullable(Of DateTime)
            Get
                Return Me.xDateTimeIN
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xDateTimeIN = value
            End Set
        End Property

        <DataMember>
        Public Property DateTimeOUT() As Nullable(Of DateTime)
            Get
                Return Me.xDateTimeOUT
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xDateTimeOUT = value
            End Set
        End Property

        <DataMember>
        Public Property IDTerminalIN() As Integer
            Get
                Return Me.intIDTerminalIN
            End Get
            Set(ByVal value As Integer)
                Me.intIDTerminalIN = value
            End Set
        End Property

        <DataMember>
        Public Property IDTerminalOUT() As Integer
            Get
                Return Me.intIDTerminalOUT
            End Get
            Set(ByVal value As Integer)
                Me.intIDTerminalOUT = value
            End Set
        End Property

        <DataMember>
        Public Property LocalizationIN() As String
            Get
                Return Me.strLocalizationIN
            End Get
            Set(ByVal value As String)
                Me.strLocalizationIN = value
            End Set
        End Property

        <DataMember>
        Public Property LocalizationOUT() As String
            Get
                Return Me.strLocalizationOUT
            End Get
            Set(ByVal value As String)
                Me.strLocalizationOUT = value
            End Set
        End Property

        <DataMember>
        Public Property IDCauseIN() As Integer
            Get
                Return Me.intIDCauseIN
            End Get
            Set(ByVal value As Integer)
                Me.intIDCauseIN = value
            End Set
        End Property

        <DataMember>
        Public Property IDCauseOUT() As Integer
            Get
                Return Me.intIDCauseOUT
            End Get
            Set(ByVal value As Integer)
                Me.intIDCauseOUT = value
            End Set
        End Property

        <DataMember>
        Public Property ShiftDate() As Nullable(Of DateTime)
            Get
                Return Me.xShiftDate
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xShiftDate = value
            End Set
        End Property

        <DataMember>
        Public Property IDMove() As Long
            Get
                Return Me.lngIDMove
            End Get
            Set(ByVal value As Long)
                Me.lngIDMove = value
            End Set
        End Property

        <DataMember>
        Public Property IDZoneIN() As Integer
            Get
                Return Me.intIDZoneIN
            End Get
            Set(ByVal value As Integer)
                Me.intIDZoneIN = value
            End Set
        End Property

        <DataMember>
        Public Property IDZoneOUT() As Integer
            Get
                Return Me.intIDZoneOUT
            End Get
            Set(ByVal value As Integer)
                Me.intIDZoneOUT = value
            End Set
        End Property

        <DataMember>
        Public Property ReaderTypeIN() As Integer
            Get
                Return Me.intReaderTypeIN
            End Get
            Set(ByVal value As Integer)
                Me.intReaderTypeIN = value
            End Set
        End Property

        <DataMember>
        Public Property ReaderTypeOUT() As Integer
            Get
                Return Me.intReaderTypeOUT
            End Get
            Set(ByVal value As Integer)
                Me.intReaderTypeOUT = value
            End Set
        End Property

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property CaptureIN() As Image
            Get
                Return Me.oCaptureIN
            End Get
            Set(ByVal value As Image)
                Me.oCaptureIN = value
            End Set
        End Property

        <DataMember>
        Public Property CaptureINBytes() As Byte()
            Get
                Return Image2Bytes(Me.oCaptureIN)
            End Get
            Set(ByVal value As Byte())
                Me.oCaptureIN = Bytes2Image(value)
            End Set
        End Property

        <XmlIgnore()>
        <IgnoreDataMember>
        Public Property CaptureOUT() As Image
            Get
                Return Me.oCaptureOUT
            End Get
            Set(ByVal value As Image)
                Me.oCaptureOUT = value
            End Set
        End Property

        <DataMember>
        Public Property CaptureOUTBytes() As Byte()
            Get
                Return Image2Bytes(Me.oCaptureOUT)
            End Get
            Set(ByVal value As Byte())
                Me.oCaptureOUT = Bytes2Image(value)
            End Set
        End Property

        <DataMember>
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

        <DataMember>
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
            Me.Load()

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
            Me.Load()

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

        Public Sub Load()

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

                Try

                    Dim tb As DataTable = CreateDataTable("@SELECT# * FROM Moves WHERE [ID] = " & Me.IDMove.ToString, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        Dim oRow As DataRow = tb.Rows(0)

                        Me.intIDEmployee = oRow("IDEmployee")
                        If Not IsDBNull(oRow("InDateTime")) Then
                            Me.xDateTimeIN = oRow("InDateTime")
                        Else
                            Me.xDateTimeIN = Nothing
                        End If
                        If Not IsDBNull(oRow("OutDateTime")) Then
                            Me.xDateTimeOUT = oRow("OutDateTime")
                        Else
                            Me.xDateTimeOUT = Nothing
                        End If

                        If Not IsDBNull(oRow("InIDReader")) Then
                            Me.intIDTerminalIN = oRow("InIDReader")
                        Else
                            Me.intIDTerminalIN = -1
                        End If
                        If Not IsDBNull(oRow("OutIDReader")) Then
                            Me.intIDTerminalOUT = oRow("OutIDReader")
                        Else
                            Me.intIDTerminalOUT = -1
                        End If

                        If Not IsDBNull(oRow("InIDCause")) Then
                            Me.intIDCauseIN = oRow("InIDCause")
                        Else
                            Me.intIDCauseIN = -1
                        End If
                        If Not IsDBNull(oRow("OutIDCause")) Then
                            Me.intIDCauseOUT = oRow("OutIDCause")
                        Else
                            Me.intIDCauseOUT = -1
                        End If

                        If Not IsDBNull(oRow("ShiftDate")) Then
                            Me.xShiftDate = oRow("ShiftDate")
                        Else
                            Me.xShiftDate = Nothing
                        End If

                        If Not IsDBNull(oRow("InIDZone")) Then
                            Me.intIDZoneIN = oRow("InIDZone")
                        Else
                            Me.intIDZoneIN = -1
                        End If
                        If Not IsDBNull(oRow("OutIDZone")) Then
                            Me.intIDZoneOUT = oRow("OutIDZone")
                        Else
                            Me.intIDZoneOUT = -1
                        End If

                        If Not IsDBNull(oRow("InIDReaderType")) Then
                            Me.intReaderTypeIN = oRow("InIDReaderType")
                        Else
                            Me.intReaderTypeIN = -1
                        End If
                        If Not IsDBNull(oRow("OutIDReaderType")) Then
                            Me.intReaderTypeOUT = oRow("OutIDReaderType")
                        Else
                            Me.intReaderTypeOUT = -1
                        End If

                        If Not IsDBNull(oRow("IsNotReliableIN")) Then
                            Me.bolIsNotReliableIN = oRow("IsNotReliableIN")
                        Else
                            Me.bolIsNotReliableIN = False
                        End If
                        If Not IsDBNull(oRow("IsNotReliableOUT")) Then
                            Me.bolIsNotReliableOUT = oRow("IsNotReliableOUT")
                        Else
                            Me.bolIsNotReliableOUT = False
                        End If

                    End If

                    Me.oCaptureIN = Nothing
                    Me.oCaptureOUT = Nothing

                    tb = CreateDataTable("@SELECT# * FROM MovesCaptures WHERE IDMove = " & Me.IDMove.ToString, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        Dim oRow As DataRow = tb.Rows(0)

                        If Not IsDBNull(oRow("InCapture")) Then
                            Dim bImage As Byte() = CType(oRow("InCapture"), Byte())
                            Dim ms As MemoryStream = New MemoryStream(bImage)
                            Me.oCaptureIN = CType(Image.FromStream(ms), Bitmap)
                        End If
                        If Not IsDBNull(oRow("OutCapture")) Then
                            Dim bImage As Byte() = CType(oRow("OutCapture"), Byte())
                            Dim ms As MemoryStream = New MemoryStream(bImage)
                            Me.oCaptureOUT = CType(Image.FromStream(ms), Bitmap)
                        End If

                    End If
                Catch ex As DbException
                    Me.oState.UpdateStateInfo(ex, "roMove::Load")
                Catch ex As Exception
                    Me.oState.UpdateStateInfo(ex, "roMove::Load")
                Finally

                End Try

            End If

        End Sub

        Public Function Save(Optional ByVal bolAutomaticBeginJobCheck As Boolean = True) As Boolean
            Dim bolRet As Boolean = False
            roLog.GetInstance().logMessage(roLog.EventType.roDebug, "roMove::Save::Saving punch data (" & Me.MoveInfo() & ")")

            Try
                ' Para el control de la auditoría
                Dim oMoveOld As DataRow = Nothing
                Dim oMoveNew As DataRow = Nothing
                Dim strQueryRow As String

                strQueryRow = "@SELECT# * " &
                              "FROM Moves WHERE [ID] = " & Me.lngIDMove.ToString
                Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "Moves")
                If tbAuditOld.Rows.Count = 1 Then oMoveOld = tbAuditOld.Rows(0)

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

                oRow("IsNotReliableIN") = IIf(Me.bolIsNotReliableIN, 1, 0)
                oRow("IsNotReliableOUT") = IIf(Me.bolIsNotReliableOUT, 1, 0)

                If Me.lngIDMove <= 0 Then
                    tbMove.Rows.Add(oRow)
                End If

                da.Update(tbMove)

                If Me.lngIDMove <= 0 Then
                    Dim tb As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM Moves WHERE IDEmployee = " & Me.intIDEmployee.ToString & " " &
                                                          "ORDER BY [ID] DESC", )
                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then
                        Me.lngIDMove = tb.Rows(0)("ID")
                    End If
                End If

                Dim oEmployeeState As New Employee.roEmployeeState(Me.oState.IDPassport)
                Dim oEmployee As Employee.roEmployee = Nothing

                ' Auditamos modificación movimiento
                strQueryRow = "@SELECT# * " &
                              "FROM Moves WHERE [ID] = " & Me.lngIDMove.ToString
                Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "Moves")
                If tbAuditNew.Rows.Count = 1 Then oMoveNew = tbAuditNew.Rows(0)

                Dim tbParameters As DataTable = oState.CreateAuditParameters()
                oState.AddAuditFieldsValues(tbParameters, oMoveNew, oMoveOld)
                Dim oAuditAction As Audit.Action = IIf(oMoveOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                Dim strObjectName As String = ""
                If oAuditAction = Audit.Action.aInsert Then
                    strObjectName = "IN: " & Any2String(oMoveNew("InDateTime")) & ", OUT: " & Any2String(oMoveNew("OutDateTime"))
                Else
                    If Any2String(oMoveOld("InDateTime")) <> Any2String(oMoveNew("InDateTime")) Then
                        strObjectName = "IN: " & oMoveOld("InDateTime") & " -> " & oMoveNew("InDateTime")
                    End If
                    If Any2String(oMoveOld("OutDateTime")) <> Any2String(oMoveNew("OutDateTime")) Then
                        strObjectName &= " OUT: " & oMoveOld("OutDateTime") & " -> " & oMoveNew("OutDateTime")
                    End If
                End If
                oEmployee = Employee.roEmployee.GetEmployee(oRow("IDEmployee"), oEmployeeState, False)
                If oEmployee IsNot Nothing Then
                    strObjectName &= " (" & oEmployee.Name & ")"
                End If
                bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tMove, strObjectName, tbParameters, -1)

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

                ' Notificamos al servidor
                roConnector.InitTask(TasksType.MOVES)

                ' Grabar imagen
                bolRet = Me.SaveCapture()

                ' Iniciar automáticamente último trabajo
                If bolAutomaticBeginJobCheck Then
                    If Me.xDateTimeIN.HasValue And Not Me.xDateTimeOUT.HasValue Then ' Es una entrada
                        Dim oSettings As New roSettings()
                        If oSettings.GetVTSetting(eKeys.AutomaticBeginJob) Then ' Está configurada la opción inicios automáticos de producción
                            oEmployee = Employee.roEmployee.GetEmployee(Me.intIDEmployee, oEmployeeState)
                            If oEmployeeState.Result = EmployeeResultEnum.NoError AndAlso oEmployee IsNot Nothing Then
                                If oEmployee.Type = "J" Then ' El empleado es de producción
                                    ''Dim oJobMove As New roJobMove(Me.intIDEmployee, -1, oLog)

                                    ' '' Obtener el último movimiento de producción
                                    ''Dim oLastJobMoveType As roJobMove.MovementStatus
                                    ''Dim xLastJobMoveDate As Date
                                    ''Dim lngLastMoveID As Long
                                    ''oJobMove.GetLastMove(oLastJobMoveType, xLastJobMoveDate, lngLastMoveID)

                                    ''If oLastJobMoveType <> roJobMove.MovementStatus.Indet_ Then
                                    ''    Dim oLastJobMove As New roJobMove(Me.intIDEmployee, lngLastMoveID, oLog)
                                    ''    Dim bolBegin As Boolean = False
                                    ''    If oLastJobMoveType = roJobMove.MovementStatus.Begin_ Then
                                    ''        bolBegin = (oLastJobMove.DateTimeIN.Value < Me.xDateTimeIN.Value)
                                    ''    Else
                                    ''        bolBegin = (oLastJobMove.DateTimeOUT.Value < Me.xDateTimeIN.Value)
                                    ''    End If
                                    ''    If bolBegin Then
                                    ''        oJobMove.LoadNewBEGIN(Me.xDateTimeIN.Value, Me.intIDTerminalIN, oLastJobMove.Job, , oLastJobMove.IDMachine, oLastJobMove.IDIncidence)
                                    ''        oJobMove.Save(oLog)
                                    ''    End If
                                    ''End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roMove::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roMove::Save")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function SaveCapture() As Boolean

            Dim bolRet As Boolean = False

            Try
                ' Eliminar imágenes antiguas
                Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
                Dim intDays As Integer = oSettings.GetVTSetting(eKeys.AuditDays)
                Dim xDate As DateTime = Now
                xDate = xDate.AddDays(-intDays)
                Dim strSQL As String =
                    "@DELETE# FROM MovesCaptures " &
                    "WHERE IDMove IN (@SELECT# [ID] FROM Moves " &
                                     "WHERE ISNULL(InDateTime, " & Any2Time(xDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(xDate).SQLDateTime & " AND " &
                                           "ISNULL(OutDateTime, " & Any2Time(xDate.AddDays(-1)).SQLDateTime & ") < " & Any2Time(xDate).SQLDateTime & ")"
                ExecuteSql(strSQL)

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

                bolRet = True
            Catch Ex As DbException
                Me.oState.UpdateStateInfo(Ex, "roMove::SaveCapture")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roMove::SaveCapture")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Delete() As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim sSql As String = "@DELETE# FROM MovesCaptures WHERE IDMove = " & Me.IDMove.ToString
                bolRet = ExecuteSql(sSql)

                If bolRet Then

                    sSql = "@DELETE# FROM Moves WHERE [ID] = " & Me.IDMove.ToString
                    bolRet = ExecuteSql(sSql)
                    If bolRet Then

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
                        sSql = "@SELECT# * FROM DailySchedule WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND " &
                                                                 "[Date] = " & Any2Time(xDailyDate.Date).SQLSmallDateTime
                        Dim cmd As DbCommand = CreateCommand(sSql)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                        da.Fill(tbSchedule)

                        Dim oRow As DataRow
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

                        ' Notificamos al servidor
                        roConnector.InitTask(TasksType.MOVES)

                        ' Auditamos borrado movimiento
                        Dim tbParameters As DataTable = oState.CreateAuditParameters()
                        Dim strObjectName As String = ""
                        If Me.xDateTimeIN.HasValue Then strObjectName &= "IN: " & Me.xDateTimeIN.Value.ToString
                        If Me.xDateTimeOUT.HasValue Then strObjectName &= " OUT: " & Me.xDateTimeOUT.Value.ToString
                        Dim oEmployeeState As New Employee.roEmployeeState(Me.oState.IDPassport)
                        Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(Me.intIDEmployee, oEmployeeState, False)
                        If oEmployee IsNot Nothing Then
                            strObjectName &= " (" & oEmployee.Name & ")"
                        End If
                        oState.AddAuditParameter(tbParameters, "{MoveName}", strObjectName, "", 1)
                        bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tMove, strObjectName, tbParameters, -1)

                        Me.IDMove = 0
                        Me.Load()

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roMove::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roMove::Delete")
            Finally

            End Try

            Return bolRet

        End Function

        Public Sub GetLastMove(ByRef oLastMoveType As MovementStatus, ByRef xLastMoveDateTime As DateTime, ByRef lngLastMoveID As Long)

            Dim tb As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# TOP 1 ID, InDateTime FROM Moves " &
                                       "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND OutDateTime IS NULL " &
                                       "ORDER BY InDateTime DESC"

                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    oLastMoveType = MovementStatus.In_
                    xLastMoveDateTime = CDate(tb.Rows(0)("InDateTime"))
                    lngLastMoveID = tb.Rows(0)("ID")
                Else
                    oLastMoveType = MovementStatus.Indet_
                    lngLastMoveID = -1
                End If

                strSQL = "@SELECT# TOP 1 ID, OutDateTime FROM Moves " &
                         "WHERE IDEmployee = " & Me.intIDEmployee.ToString & " AND OutDateTime IS NOT NULL " &
                         "ORDER BY OutDateTime DESC"

                tb = CreateDataTable(strSQL, )

                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If CDate(tb.Rows(0)("OutDateTime")) > xLastMoveDateTime Or oLastMoveType = MovementStatus.Indet_ Then
                        oLastMoveType = MovementStatus.Out_
                        xLastMoveDateTime = CDate(tb.Rows(0)("OutDateTime"))
                        lngLastMoveID = tb.Rows(0)("ID")
                    End If
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roMove:GetLastMove")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roMove:GetLastMove")
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
            If Me.intIDCauseIN <> -1 Then _
                strPunch &= "InIDCause=" & Me.intIDCauseIN.ToString & ";"
            If Me.intIDCauseOUT <> -1 Then _
                strPunch &= "OutIDCause=" & Me.intIDCauseOUT.ToString & ";"
            If Me.xShiftDate.HasValue Then _
                strPunch &= "ShiftDate=" & Me.xShiftDate.Value.ToString(oState.Language.GetShortDateFormat) & ";"
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

#Region "Helper methods"

        ''' <summary>
        ''' Retorna l'ultim registre de la taula moves segons el filtrat
        ''' </summary>
        ''' <param name="_State">roState</param>
        ''' <param name="iIDEmployee">ID Usuari</param>
        ''' <param name="iIDReader">ID Terminal</param>
        ''' <param name="iIDCause">ID Justificació</param>
        ''' <param name="iIDZone">ID Zona</param>
        ''' <param name="iReaderType">ID Lector???</param>
        ''' <param name="iIsNotReliable">Es Fiable?</param>
        ''' <param name="dBegin">Data Inici</param>
        ''' <param name="dEnd">Data Final</param>
        ''' <param name="oActiveConnection">Connexio activa (si cal)</param>
        ''' <param name="oActiveTransaction">Transacció (si cal)</param>
        ''' <returns>DataTable amb un sol registre</returns>
        ''' <remarks></remarks>
        Public Shared Function GetLastMoveDataTable(ByVal _State As roMoveState, Optional ByVal iIDEmployee As Integer = -1,
                                                     Optional ByVal iIDReader As Integer = -1,
                                                     Optional ByVal iIDCause As Integer = -1,
                                                     Optional ByVal iIDZone As Integer = -1,
                                                     Optional ByVal iReaderType As Integer = -1,
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal dBegin As DateTime = Nothing,
                                                     Optional ByVal dEnd As DateTime = Nothing) As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL1 As String = "@SELECT# TOP 1 InDateTime as xDate,idemployee, InIDReader as IDReader, 'IN' as MoveType, InIDCause as IDCause, InIDZone as IDZone, InIDReaderType as IDReaderType, IsNotReliableIN as IsNotReliable, ShiftDate From Moves "
                Dim strWhere1 As String = "Where InDateTime IS NOT NULL "

                Dim strSQL2 As String = "@SELECT# TOP 1 OutDateTime,idemployee, OutIdReader, 'OUT', OutIDCause, OutIDZone, OutIDReaderType, IsNotReliableOut, ShiftDate From Moves "
                Dim strWhere2 As String = "Where OutDateTime IS NOT NULL "

                'Filtres
                If iIDEmployee <> -1 Then
                    strWhere1 &= " And IDEmployee = " & iIDEmployee
                    strWhere2 &= " And IDEmployee = " & iIDEmployee
                End If

                If iIDReader <> -1 Then
                    strWhere1 &= " And InIDReader = " & iIDReader
                    strWhere2 &= " And OutIDReader = " & iIDReader
                End If

                If iIDCause <> -1 Then
                    strWhere1 &= " And InIDCause = " & iIDCause
                    strWhere2 &= " And OutIDCause = " & iIDCause
                End If

                If iIDZone <> -1 Then
                    strWhere1 &= " And InIDZone = " & iIDZone
                    strWhere2 &= " And OutIDZone = " & iIDZone
                End If

                If iReaderType <> -1 Then
                    strWhere1 &= " And InIDReaderType = " & iReaderType
                    strWhere2 &= " And OutIDReaderType = " & iReaderType
                End If

                If iIsNotReliable <> -1 Then
                    strWhere1 &= " And IsNotReliableIN = " & iIsNotReliable
                    strWhere2 &= " And IsNotReliableOUT = " & iIsNotReliable
                End If

                If dBegin = Nothing And dEnd = Nothing Then
                Else
                    strWhere1 &= " And InDateTime Between " & Any2Time(dBegin).SQLDateTime & " And " & Any2Time(dEnd).SQLDateTime
                    strWhere2 &= " And OutDateTime Between " & Any2Time(dBegin).SQLDateTime & " And " & Any2Time(dEnd).SQLDateTime
                End If

                Dim strSQL As String = strSQL1 & strWhere1 &
                                        " UNION " &
                                        strSQL2 & strWhere2 &
                                        " Order By xDate DESC, MoveType DESC;"

                'Select complert
                '"@SELECT# TOP 1 InDateTime as xDate,idemployee, InIDReader as IDReader, 'IN' as MoveType, InIDCause as IDCause, InIDZone as IDZone, InIDReaderType as IDReaderType, IsNotReliableIN as IsNotReliable, ShiftDate" & _
                '"from moves Where IdEmployee = 653 And InIDReader = 61 And InIDCause IS NULL And InIDZone IS NULL And InIDReaderType IS NULL And IsNotReliableIN IS NULL And InDateTime IS NOT NULL and InDateTime Between '02/03/2009 00:00:00' And '02/03/2009 23:59:00' " & _
                '"UNION" & _
                '"@SELECT# TOP 1 OutDateTime,idemployee, OutIdReader, 'OUT', OutIDCause, OutIDZone, OutIDReaderType, IsNotReliableOut, ShiftDate" & _
                '"from moves Where IdEmployee = 653 And OutIDReader = 61 And OutIDCause IS NULL And OutIDZone IS NULL And OutIDReaderType IS NULL And IsNotReliableOUT IS NULL And OutDateTime IS NOT NULL and  OutDateTime Between '02/03/2009 00:00:00' And '02/03/2009 23:59:00' " & _
                '"Order By xDate DESC;"

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roMove:GetLastMoveDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roMove:GetLastMoveDataTable")
            Finally

            End Try

            Return dRet

        End Function

        ''' <summary>
        ''' Retorna els registres de la taula moves segons el filtrat
        ''' </summary>
        ''' <param name="_State">roState</param>
        ''' <param name="iIDEmployee">ID Usuari</param>
        ''' <param name="iIDReader">ID Terminal</param>
        ''' <param name="iIDCause">ID Justificació</param>
        ''' <param name="iIDZone">ID Zona</param>
        ''' <param name="iReaderType">ID Lector???</param>
        ''' <param name="iIsNotReliable">Es Fiable?</param>
        ''' <param name="dBegin">Data Inici</param>
        ''' <param name="dEnd">Data Final</param>
        ''' <param name="oActiveConnection">Connexio activa (si cal)</param>
        ''' <param name="oActiveTransaction">Transacció (si cal)</param>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Shared Function GetMovesDataTable(ByVal _State As roMoveState,
                                                     ByVal dBegin As DateTime,
                                                     ByVal dEnd As DateTime,
                                                     Optional ByVal iIDEmployee As Integer = -1,
                                                     Optional ByVal iIDReader As Integer = -1,
                                                     Optional ByVal iIDCause As Integer = -1,
                                                     Optional ByVal iIDZone As Integer = -1,
                                                     Optional ByVal iReaderType As Integer = -1,
                                                     Optional ByVal iIsNotReliable As Integer = -1) As DataTable
            Dim dRet As DataTable = Nothing

            Try

                Dim strSQL1 As String = "@SELECT# * from Moves "
                Dim strWhere1 As String = " Where InDateTime Between " & Any2Time(dBegin).SQLDateTime & " And " & Any2Time(dEnd).SQLDateTime

                Dim strSQL2 As String = "@SELECT# * from Moves "
                Dim strWhere2 As String = " Where OutDateTime Between " & Any2Time(dBegin).SQLDateTime & " And " & Any2Time(dEnd).SQLDateTime

                'Filtres
                If iIDEmployee <> -1 Then
                    strWhere1 &= " And IDEmployee = " & iIDEmployee
                    strWhere2 &= " And IDEmployee = " & iIDEmployee
                End If

                If iIDReader <> -1 Then
                    strWhere1 &= " And InIDReader = " & iIDReader
                    strWhere2 &= " And OutIDReader = " & iIDReader
                End If

                If iIDCause <> -1 Then
                    strWhere1 &= " And InIDCause = " & iIDCause
                    strWhere2 &= " And OutIDCause = " & iIDCause
                End If

                If iIDZone <> -1 Then
                    strWhere1 &= " And InIDZone = " & iIDZone
                    strWhere2 &= " And OutIDZone = " & iIDZone
                End If

                If iReaderType <> -1 Then
                    strWhere1 &= " And InIDReaderType = " & iReaderType
                    strWhere2 &= " And OutIDReaderType = " & iReaderType
                End If

                If iIsNotReliable <> -1 Then
                    strWhere1 &= " And IsNotReliableIN = " & iIsNotReliable
                    strWhere2 &= " And IsNotReliableOUT = " & iIsNotReliable
                End If

                Dim strSQL As String = strSQL1 & strWhere1 &
                                        " UNION " &
                                        strSQL2 & strWhere2 &
                                        " Order By InDateTime Desc, OutDateTime DESC"

                'Select complert
                '"@SELECT# TOP 1 InDateTime as xDate,idemployee, InIDReader as IDReader, 'IN' as MoveType, InIDCause as IDCause, InIDZone as IDZone, InIDReaderType as IDReaderType, IsNotReliableIN as IsNotReliable, ShiftDate" & _
                '"from moves Where IdEmployee = 653 And InIDReader = 61 And InIDCause IS NULL And InIDZone IS NULL And InIDReaderType IS NULL And IsNotReliableIN IS NULL And InDateTime IS NOT NULL and InDateTime Between '02/03/2009 00:00:00' And '02/03/2009 23:59:00' " & _
                '"UNION" & _
                '"@SELECT# TOP 1 OutDateTime,idemployee, OutIdReader, 'OUT', OutIDCause, OutIDZone, OutIDReaderType, IsNotReliableOut, ShiftDate" & _
                '"from moves Where IdEmployee = 653 And OutIDReader = 61 And OutIDCause IS NULL And OutIDZone IS NULL And OutIDReaderType IS NULL And IsNotReliableOUT IS NULL And OutDateTime IS NOT NULL and  OutDateTime Between '02/03/2009 00:00:00' And '02/03/2009 23:59:00' " & _
                '"Order By xDate DESC;"

                dRet = CreateDataTable(strSQL)
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roMove:GetMovesDataTable")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roMove:GetMovesDataTable")
            Finally

            End Try

            Return dRet

        End Function

        ''' <summary>
        ''' Añade un movimiento de presencia para un empleado. El tipo de movimiento generado se calcula en función del estado actual del empleado.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
        ''' <param name="_IDReader">Número del lector por el que se realiza el movimiento</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Move">Devuelve el movimiento generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de movimiento generado</param>
        ''' <param name="_SeqStatus">Devuelve el estado de la secuencia resultado de la acción</param>
        ''' <param name="_SaveData">Indica si se tiene que guardar el movimiento generado o no.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoSequenceMove(ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Move As roMove, ByRef _InputType As MovementStatus, ByRef _SeqStatus As MovementSeqStatus, ByVal _SaveData As Boolean, ByVal _State As roMoveState) As Boolean

            Dim bolRet As Boolean = False

            Try

                _SeqStatus = MovementSeqStatus.OK

                ' Obtener información del último marcaje de presencia del empleado
                Dim oLastMoveType As MovementStatus = MovementStatus.Indet_
                Dim xLastMoveDateTime As DateTime
                Dim lngLastMoveID As Long = -1
                _Move = New roMove(_IDEmployee, -1, _State)
                _Move.GetLastMove(oLastMoveType, xLastMoveDateTime, lngLastMoveID)
                _Move.IDMove = lngLastMoveID
                _Move.Load()

                If oLastMoveType <> MovementStatus.Indet_ Then

                    ' Verificar tiempo entre marcajes (MaxMovementHours)
                    Dim oParameters As New roParameters("OPTIONS", True)
                    Dim intMaxMovement As Integer = 0
                    Dim oTime As roTime = roTypes.Any2Time(oParameters.Parameter(Parameters.MovMaxHours))
                    If oTime.IsValid Then intMaxMovement = oTime.Minutes

                    If intMaxMovement > 0 AndAlso
                       DateDiff(DateInterval.Minute, xLastMoveDateTime, _InputDateTime) > intMaxMovement Then

                        ' El tiempo entre el marcaje actual y el anterior és superior al máximo (MaxMovementHours)
                        If oLastMoveType = MovementStatus.Out_ Then
                            _SeqStatus = MovementSeqStatus.Max_SeqOK
                        Else
                            _SeqStatus = MovementSeqStatus.Max_SeqERR
                        End If
                        _Move.LoadNewIN(_InputDateTime, _IDTerminal, , _IDCause, , , _InputCapture)
                        _InputType = MovementStatus.In_
                    Else

                        Dim intPunchPeriodRTIn As Integer = 0
                        Dim intPunchPeriodRTOut As Integer = 0

                        ' Obtengo configuración tiempo mínimo entre una entrada-salida i una salida-entrada
                        Dim oTerminalState As New Terminal.roTerminalState : roBusinessState.CopyTo(_State, oTerminalState)
                        Dim oReader As New Terminal.roTerminal.roTerminalReader(_IDTerminal, _IDReader, oTerminalState)
                        If oReader IsNot Nothing AndAlso oTerminalState.Result <> TerminalBaseResultEnum.NoError AndAlso oReader.InteractiveConfig IsNot Nothing Then
                            intPunchPeriodRTIn = oReader.InteractiveConfig.PunchPeriodRTIn
                            intPunchPeriodRTOut = oReader.InteractiveConfig.PunchPeriodRTOut
                        Else
                            intPunchPeriodRTIn = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTIn))
                            intPunchPeriodRTOut = Any2Integer(oParameters.Parameter(Parameters.PunchPeriodRTOut))
                        End If

                        ' Detectar marcaje repetido (PunchPeriodRTIn, PunchPeriodRTOut)
                        If oLastMoveType = MovementStatus.In_ Then
                            If DateDiff(DateInterval.Minute, xLastMoveDateTime, _InputDateTime) < intPunchPeriodRTIn Then
                                _SeqStatus = MovementSeqStatus.Repited_IN
                                _InputType = MovementStatus.Indet_
                            Else
                                _InputType = MovementStatus.Out_
                                _Move.LoadNewOUT(_InputDateTime, _IDTerminal, _Move.IDMove, _IDCause, , , _InputCapture)
                            End If
                        ElseIf oLastMoveType = MovementStatus.Out_ Then
                            If DateDiff(DateInterval.Minute, xLastMoveDateTime, _InputDateTime) < intPunchPeriodRTOut Then
                                _SeqStatus = MovementSeqStatus.Repited_OUT
                                _InputType = MovementStatus.Indet_
                            Else
                                _InputType = MovementStatus.In_
                                _Move.LoadNewIN(_InputDateTime, _IDTerminal, -1, _IDCause, , , _InputCapture)
                            End If
                        End If

                    End If
                Else

                    _InputType = MovementStatus.In_
                    _Move.LoadNewIN(_InputDateTime, _IDTerminal, -1, _IDCause, , , _InputCapture)
                    _SeqStatus = MovementSeqStatus.OK

                End If

                bolRet = True

                If _SaveData Then
                    If _SeqStatus <> MovementSeqStatus.Repited_IN And _SeqStatus <> MovementSeqStatus.Repited_OUT Then
                        ' Guardamos el movimiento
                        bolRet = _Move.Save()
                    End If
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::DoSequenceMove")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::DoSequenceMove")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Cambia el estado de presencia actual del empleado generando un fichaje a la hora indicada.
        ''' </summary>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_NowDateTime">Fecha y hora actual</param>
        ''' <param name="_InputDateTime">Hora en la que se generará el fichaje olvidado para el cambio de estado</param>
        ''' <param name="_IDTerminal">Código de terminal por el que se realiza la operación</param>
        ''' <param name="_IDReader">Número de lector por el que se realiza la operación</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Move">Devuelve el movimiento generado</param>
        ''' <param name="_SaveData">Indica si se tiene que guardar el movimiento generado o no.</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ChangeState(ByVal _IDEmployee As Integer, ByVal _NowDateTime As DateTime, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Move As roMove, ByVal _SaveData As Boolean, ByVal _State As roMoveState) As Boolean

            Dim bolRet As Boolean = False

            Try

                ' Obtener información del último marcaje de presencia del empleado
                Dim oLastMoveType As MovementStatus = MovementStatus.Indet_
                Dim xLastMoveDateTime As DateTime
                Dim lngLastMoveID As Long = -1
                _Move = New roMove(_IDEmployee, -1, _State)
                _Move.GetLastMove(oLastMoveType, xLastMoveDateTime, lngLastMoveID)
                _Move.IDMove = lngLastMoveID
                _Move.Load()

                Dim bolValidInputDateTime As Boolean = True

                ' Verificar que la fecha y hora indicada sea válida en función del último fichaje
                If oLastMoveType <> MovementStatus.Indet_ Then

                    Dim xMinValue As Date
                    Dim xMaxValue As Date
                    If _NowDateTime.DayOfYear = xLastMoveDateTime.DayOfYear And _NowDateTime.Year = xLastMoveDateTime.Year Then
                        ' Último marcaje en el mismo día
                        xMinValue = xLastMoveDateTime.AddMinutes(1)
                        xMaxValue = _NowDateTime.AddMinutes(-1)
                    Else
                        ' Último marcaje día o días anterior
                        xMinValue = New Date(_NowDateTime.Year, _NowDateTime.Month, _NowDateTime.Day, 0, 0, 0)
                        xMaxValue = _NowDateTime.AddMinutes(-1)
                    End If

                    If _InputDateTime.Hour = xMinValue.Hour Then
                        If _InputDateTime.Minute >= xMinValue.Minute Then
                            bolValidInputDateTime = True
                        End If
                    ElseIf _InputDateTime.Hour > xMinValue.Hour Then
                        bolValidInputDateTime = True
                    End If

                    If bolValidInputDateTime Then
                        bolValidInputDateTime = False
                        If _InputDateTime.Hour = xMaxValue.Hour Then
                            If _InputDateTime.Minute <= xMaxValue.Minute Then
                                bolValidInputDateTime = True
                            End If
                        ElseIf _InputDateTime.Hour < xMaxValue.Hour Then
                            bolValidInputDateTime = True
                        End If
                    End If

                End If

                If bolValidInputDateTime Then

                    Dim xMarca As DateTime
                    xMarca = New DateTime(_NowDateTime.Year, _NowDateTime.Month, _NowDateTime.Day,
                                           _InputDateTime.Hour, _InputDateTime.Minute, _InputDateTime.Second)
                    Dim intNowMin As Integer = (_NowDateTime.Hour * 60) + _NowDateTime.Minute
                    Dim intInputMin As Integer = (_InputDateTime.Hour * 60) + _InputDateTime.Minute
                    If intNowMin <= intInputMin Then
                        xMarca = xMarca.AddDays(-1)
                    End If

                    ' Generar marcaje olvidado
                    If oLastMoveType = MovementStatus.Out_ Or oLastMoveType = MovementStatus.Indet_ Then
                        ' Informamos marcaje de entrada olvidado
                        _Move.LoadNewIN(xMarca, _IDTerminal, , , , , , True)
                    Else
                        If xMarca.DayOfYear = _NowDateTime.DayOfYear Then
                            ' Informamos marcaje de salida olvidado
                            _Move.LoadOUT(xMarca, _IDTerminal, , , , _InputCapture, True)
                        Else
                            ' Error.

                        End If
                    End If

                    bolRet = True

                    If _SaveData Then
                        ' Guardamos el movimiento
                        bolRet = _Move.Save()
                    End If
                Else
                    _State.Result = MoveResultEnum.InvalidChangeStateTime
                End If
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::ChangeState")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::ChangeState")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve los fichajes en función del filtro indicado.
        ''' </summary>
        ''' <param name="strFilter">Filtro a aplicar</param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns>DataTable con los registros de la tabla 'Moves' coincidentes con el filtro.</returns>
        ''' <remarks></remarks>
        Public Shared Function GetMoves(ByVal strFilter As String, ByVal _State As roMoveState) As DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String

                strSQL = "@SELECT# * FROM Moves "
                If strFilter <> "" Then
                    strSQL &= "WHERE " & strFilter
                End If
                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::GetMoves")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::GetMoves")
            Finally

            End Try

            Return tbRet

        End Function

#End Region

#End Region

    End Class

    Public Class roMoveList

#Region "Declarations - Constructors"

        Private oState As roMoveState

        Private MoveItems As ArrayList

        Public Sub New()

            Me.oState = New roMoveState
            Me.MoveItems = New ArrayList

        End Sub

        Public Sub New(ByVal _State As roMoveState)

            Me.oState = _State
            Me.MoveItems = New ArrayList

        End Sub

#End Region

#Region "Properties"

        <XmlArray("Moves"), XmlArrayItem("roMove", GetType(roMove))>
        Public Property Moves() As ArrayList
            Get
                Return Me.MoveItems
            End Get
            Set(ByVal value As ArrayList)
                Me.MoveItems = value
            End Set
        End Property

        Public ReadOnly Property State() As roMoveState
            Get
                Return Me.oState
            End Get
        End Property

#End Region

#Region "Methods"

        Public Function Save(Optional ByVal bolAutomaticBeginJobCheck As Boolean = True) As Boolean

            Dim bolRet As Boolean = True

            Try
                For Each oMove As roMove In Me.Moves
                    bolRet = oMove.Save(bolAutomaticBeginJobCheck)
                    If Not bolRet Then
                        Exit For
                    End If
                Next
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roMoveList::Save")
            End Try

            Return bolRet

        End Function

        Public Function Save(ByVal tbMoves As DataTable, Optional ByVal bolAutomaticBeginJobCheck As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try

                Dim oMove As roMove
                Dim lstEmployeeDay As New ArrayList
                Dim bolSaved As Boolean = True

                If tbMoves.Rows.Count > 0 Then

                    For Each oRow As DataRow In tbMoves.Rows

                        bolSaved = True

                        Select Case oRow.RowState
                            Case DataRowState.Added, DataRowState.Modified

                                oMove = New roMove(oRow("IDEmployee"), oRow("ID"), Me.oState)
                                With oMove
                                    .DateTimeIN = IIf(Not IsDBNull(oRow("InDateTime")), oRow("InDateTime"), Nothing)
                                    .DateTimeOUT = IIf(Not IsDBNull(oRow("OutDateTime")), oRow("OutDateTime"), Nothing)
                                    .IDTerminalIN = IIf(Not IsDBNull(oRow("InIDReader")), oRow("InIDReader"), -1)
                                    .IDTerminalOUT = IIf(Not IsDBNull(oRow("OutIDReader")), oRow("OutIDReader"), -1)
                                    .IDCauseIN = IIf(Not IsDBNull(oRow("InIDCause")), oRow("InIDCause"), -1)
                                    .IDCauseOUT = IIf(Not IsDBNull(oRow("OutIDCause")), oRow("OutIDCause"), -1)
                                    .ShiftDate = oRow("ShiftDate")
                                    .IDZoneIN = IIf(Not IsDBNull(oRow("InIDZone")), oRow("InIDZone"), -1)
                                    .IDZoneOUT = IIf(Not IsDBNull(oRow("OutIDZone")), oRow("OutIDZone"), -1)
                                    .ReaderTypeIN = IIf(Not IsDBNull(oRow("InIDReaderType")), oRow("InIDReaderType"), -1)
                                    .ReaderTypeOUT = IIf(Not IsDBNull(oRow("OutIDReaderType")), oRow("OutIDReaderType"), -1)
                                    .IsNotReliableIN = IIf(Not IsDBNull(oRow("IsNotReliableIN")), oRow("IsNotReliableIN"), False)
                                    .IsNotReliableOUT = IIf(Not IsDBNull(oRow("IsNotReliableOUT")), oRow("IsNotReliableOUT"), False)
                                    If tbMoves.Columns.Contains("InCapture") Then
                                        .CaptureIN = IIf(Not IsDBNull(oRow("InCapture")), oRow("InCapture"), Nothing)
                                    End If
                                    If tbMoves.Columns.Contains("OutCapture") Then
                                        .CaptureOUT = IIf(Not IsDBNull(oRow("OutCapture")), oRow("OutCapture"), Nothing)
                                    End If
                                End With
                                bolRet = oMove.Save(bolAutomaticBeginJobCheck)

                            Case DataRowState.Deleted
                                oRow.RejectChanges() ' Cmabiar el estado de la fila para poder leer sus datos
                                oMove = New roMove(oRow("IDEmployee"), oRow("ID"), Me.oState)
                                bolRet = oMove.Delete

                            Case Else
                                bolRet = True
                                bolSaved = False

                        End Select

                        If Not bolRet Then

                            Exit For
                        End If

                    Next
                Else

                    bolRet = True

                End If
            Catch ex As Exception
                bolRet = False
                Me.oState.UpdateStateInfo(ex, "roMoveList::Save")
            End Try

            Return bolRet

        End Function

        Public Function LoadData(ByVal ds As DataSet, ByRef oState As roMoveState) As Boolean

            Dim bolRet As Boolean = False

            Try

                If ds.Tables.Contains("Moves") Then

                    Dim tbMoves As DataTable = ds.Tables("Moves")
                    Dim oMove As roMove

                    For Each oRow As DataRow In tbMoves.Rows
                        oMove = New roMove(oState)
                        With oMove
                            .IDEmployee = oRow("IDEmployee")
                            .IDMove = oRow("ID")
                            .Load()
                            ' ...
                        End With
                        Me.Moves.Add(oMove)
                    Next

                    If ds.Tables.Contains("MovesCaptures") Then

                    End If

                    bolRet = True

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roMoveList:LoadData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roMoveList:LoadData")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace