Imports System.Data.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roPerformance

    Const RESOURCE_EMPLOYEE = 1
    Const RESOURCE_MACHINE = 2
    Const RESOURCE_EMPLOYEE_MACHINE = 3
    Const RESOURCE_NONE = 0

    Public Enum roCriteria
        ByEmployee = 1
        ByTeam = 2
        ByJob = 3
        ByOrder = 4
        ByMachine = 5
    End Enum

#Region "Declarations - Constructor"

    Private mvarIDMachine As Integer
    Private mvarIDEmployee As Integer
    Private mvarIDJob As Double
    Private mvarIDTeam As Integer
    Private mvarBeginDate As Nullable(Of DateTime)
    Private mvarFinishDate As Nullable(Of DateTime)
    Private mvarPerformance As Double
    Private mvarTotalTime As Double
    Private mvarTypePerformance As roCriteria
    Private mvarTotalPieces As Double
    Private mvarTotalPieces1 As Double
    Private mvarTotalPieces2 As Double
    Private mvarTotalPieces3 As Double
    Private mvarGoodPerformance As Double
    Private mvarIDOrder As String
    Private mISOrder As Boolean
    Private mvarFirstResource As Double

#End Region

#Region "Properties"

    Public Property FirstResource() As Double
        Get
            Return Me.mvarFirstResource
        End Get
        Set(ByVal value As Double)
            mvarFirstResource = Any2Double(value)
        End Set
    End Property

    Public Property IDOrder() As String
        Get
            Return Me.mvarIDOrder
        End Get
        Set(ByVal value As String)
            Me.mvarIDOrder = value
        End Set
    End Property

    Public ReadOnly Property GoodPerformance() As Double
        Get
            Return Me.mvarGoodPerformance
        End Get
    End Property

    Public ReadOnly Property TotalPieces1() As Double
        Get
            Return Me.mvarTotalPieces1
        End Get
    End Property

    Public ReadOnly Property TotalPieces2() As Double
        Get
            Return Me.mvarTotalPieces2
        End Get
    End Property

    Public ReadOnly Property TotalPieces3() As Double
        Get
            Return Me.mvarTotalPieces3
        End Get
    End Property

    Public ReadOnly Property TotalPieces() As Double
        Get
            Return Me.mvarTotalPieces
        End Get
    End Property

    Public Property TypePerformance() As roCriteria
        Get
            Return Me.mvarTypePerformance
        End Get
        Set(ByVal value As roCriteria)
            Me.mvarTypePerformance = value
        End Set
    End Property

    Public ReadOnly Property TotalTime() As Double
        Get
            Return Me.mvarTotalTime
        End Get
    End Property

    Public ReadOnly Property Performance() As Double
        Get
            Return Me.mvarPerformance
        End Get
    End Property

    Public Property BeginDate() As Nullable(Of DateTime)
        Get
            Return Me.mvarBeginDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            Me.mvarBeginDate = value
        End Set
    End Property

    Public Property FinishDate() As Nullable(Of DateTime)
        Get
            Return Me.mvarFinishDate
        End Get
        Set(ByVal value As Nullable(Of DateTime))
            Me.mvarFinishDate = value
        End Set
    End Property

    Public Property IDTeam() As Integer
        Get
            Return Me.mvarIDTeam
        End Get
        Set(ByVal value As Integer)
            Me.mvarIDTeam = value
        End Set
    End Property

    Public Property IDJob() As Double
        Get
            Return Me.mvarIDJob
        End Get
        Set(ByVal value As Double)
            Me.mvarIDJob = value
        End Set
    End Property

    Public Property IDEmployee() As Integer
        Get
            Return Me.mvarIDEmployee
        End Get
        Set(ByVal value As Integer)
            Me.mvarIDEmployee = value
        End Set
    End Property

    Public Property IDMachine() As Integer
        Get
            Return Me.mvarIDMachine
        End Get
        Set(ByVal value As Integer)
            Me.mvarIDMachine = value
        End Set
    End Property

#End Region

#Region "Methods"

    Public Function CalculatedPerformance(ByVal oLog As roLog) As Boolean
        'Calculamos el Rendimiento de una fase o de la orden
        'dependiento de los criterios de selección
        '--> (Rendimiento Actual o entre fechas)
        '--> (Rendimiento Piezas buenas Actual o entre fechas)
        '-->(Piezas totales y por tipo)
        '-->(Tiempo Invertido)

        Dim bolRet As Boolean

        Dim tb As DataTable
        Dim sSQL As String
        Dim CountReg As Integer
        Dim TimeReg As Double
        Dim CountPiece As Boolean
        Dim ToPerformance As Boolean, ToPerformanceType As Boolean
        Dim i As Integer
        Dim PiecesReg As Double
        Dim RendReg As Double
        Dim GoodType As Boolean
        Dim UnitPieces As Double
        Dim RepeatLoop As Integer
        Dim x As Integer

        Try

            bolRet = True

            'Inicializamos Variables
            InitValues()

            If mvarTypePerformance = roCriteria.ByOrder Then
                RepeatLoop = 2
            Else
                RepeatLoop = 1
            End If

            For x = 1 To RepeatLoop
                'Creamos la sentencia SQL dependiendo de los criterios seleccionados
                sSQL = CreateSQL(x)

                tb = CreateDataTable(sSQL, )
                For Each oRow As DataRow In tb.Rows
                    'Tiempo del registro
                    TimeReg = Any2Double(oRow("TTime"))

                    ' Si calcumaos los datos de una fase
                    If Not mISOrder Then
                        For i = 1 To 3
                            'Caracteristicas del tipo de pieza
                            FindTypePiece(i, CountPiece, ToPerformanceType)

                            If ToPerformanceType Then ToPerformance = True

                            If CountPiece Then
                                'Acumulamos las piezas que cuentan como realizadas
                                GetPiecesAccruals(i, oRow, GoodType, mvarTotalPieces)
                            End If
                        Next i

                        'Acumulamos los totales por los diferentes tipos de piezas
                        GetAccrualsByPieceType(oRow)
                    End If

                    ToPerformance = False
                    If Any2Double(oRow("IDIncidence")) = 0 And Not mISOrder Then
                        ToPerformance = True
                    End If

                    'Solo se calcula el rendimiento si no hay incidencia
                    'y si hay piezas con rendimiento y es una fase
                    If ToPerformance Then
                        UnitPieces = Any2Double(oRow("TPieceTime"))
                        CountReg = Any2Double(oRow("TCount"))
                        PiecesReg = Any2Double(oRow("TTotalPieces"))
                        If UnitPieces <> 0 And CountReg <> 0 And TimeReg <> 0 And PiecesReg <> 0 Then
                            RendReg = (UnitPieces / CountReg) / (TimeReg / PiecesReg) * 100
                        Else
                            RendReg = 0
                        End If

                        'Rendimient Actual
                        mvarPerformance = RendReg
                    End If

                    'Mirams si el tiempo de la incidencia cuenta como tiempo de produccion
                    If Any2Double(oRow("IDIncidence")) = 0 Then
                        mvarTotalTime = mvarTotalTime + TimeReg
                    ElseIf Any2Boolean(ExecuteScalar("@SELECT# CountOnPerformance FROM JobIncidences WHERE ID= " & Any2Double(oRow("IDIncidence")))) Then
                        mvarTotalTime = mvarTotalTime + TimeReg
                    End If

                Next

                'Calculamos los resultados finales
                If mISOrder Then
                    'Orden
                    mvarPerformance = 0
                    mvarGoodPerformance = 0 : mvarTotalPieces = 0
                    mvarTotalPieces1 = 0 : mvarTotalPieces2 = 0 : mvarTotalPieces3 = 0
                End If

            Next x
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roPerformance::CalculatedPerformance :", ex)
            bolRet = False
            InitValues()
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roPerformance::CalculatedPerformance :", ex)
            bolRet = False
            InitValues()
        Finally

        End Try

        Return bolRet

    End Function

    Public Function CalculatedPerformanceEx(ByRef oLog As roLog) As Boolean
        'Calculamos el Rendimiento de una fase o de la orden
        'dependiento de los criterios de selección
        '--> (Rendimiento Actual o entre fechas)
        '--> (Rendimiento Piezas buenas Actual o entre fechas)
        '-->(Piezas totales y por tipo)
        '-->(Tiempo Invertido)

        Dim bolRet As Boolean

        Dim tb As DataTable
        Dim sSQL As String
        Dim CountReg As Integer
        Dim CountGoodReg As Integer
        Dim TimeReg As Double
        Dim CountPiece As Boolean
        Dim ToPerformance As Boolean, ToPerformanceType As Boolean
        Dim i As Integer
        Dim PiecesReg As Double
        Dim RendReg As Double
        Dim GoodType As Boolean
        Dim PieceTimeJob As Double, PreparationTime As Double, UnitPieces As Double
        Dim Path As String
        Dim Amount As Double

        Try

            bolRet = True

            'Creamos la sentencia SQL dependiendo de los criterios seleccionados
            sSQL = CreateSQL(1)

            'Inicializamos Variables
            InitValues()

            tb = CreateDataTable(sSQL, )
            For Each oRow As DataRow In tb.Rows

                'Tiempo del registro
                TimeReg = Any2Double(oRow("TTime"))

                'Calculamos piezas a realizar
                Path = Any2String(ExecuteScalar("@SELECT# Path FROM Jobs WHERE ID=" & oRow("IDJob")))

                FindAmountOrder(Amount, Path)

                'Acumulamos el total por tipo de pieza
                GetAccrualsByPieceType(oRow)

                ToPerformance = False : GoodType = False
                PiecesReg = 0
                For i = 1 To 3
                    'Caracteristicas del tipo de pieza
                    FindTypePiece(i, CountPiece, ToPerformanceType)

                    If ToPerformanceType Then ToPerformance = True

                    If CountPiece Then
                        'Acumulamos el total de piezas
                        GetPiecesAccruals(i, oRow, GoodType, PiecesReg)
                    End If
                Next i
                mvarTotalPieces = mvarTotalPieces + PiecesReg

                'Solo se calcula el rendimiento si no hay incidencia
                'y si hay piezas con rendimiento
                If Any2Double(oRow("IDIncidence")) = 0 And ToPerformance Then

                    'Busca datos generales del lanzamiento de la fase
                    FindJobDetails(Any2Double(oRow("IDJob")), PieceTimeJob, PreparationTime, UnitPieces)

                    If Not mISOrder Then
                        If UnitPieces <> 0 And PiecesReg <> 0 And TimeReg <> 0 Then
                            RendReg = ((PieceTimeJob * UnitPieces * Amount) + PreparationTime) / (UnitPieces * Amount)
                            RendReg = (RendReg / (TimeReg / PiecesReg)) * 100
                        Else
                            RendReg = 0
                        End If
                    Else
                        If UnitPieces <> 0 Then
                            RendReg = ((PieceTimeJob * UnitPieces * Amount) + PreparationTime) / (UnitPieces * Amount)
                            RendReg = (RendReg * PiecesReg)
                        Else
                            RendReg = 0
                        End If
                    End If

                    'Rendimient Actual
                    mvarPerformance = mvarPerformance + RendReg

                    'Rendimiento piezas buenas
                    If GoodType And ToPerformance Then
                        mvarGoodPerformance = mvarGoodPerformance + RendReg
                        CountGoodReg = CountGoodReg + 1
                    End If

                    'Contador
                    CountReg = CountReg + 1
                End If

                'Mirams si el tiempo de la incidencia cuenta como tiempo de produccion
                If Any2Boolean(ExecuteScalar("@SELECT# CountOnPerformance FROM JobIncidences WHERE ID= " & Any2Double(oRow("IDIncidence")))) Then
                    mvarTotalTime = mvarTotalTime + TimeReg
                End If

            Next

            'Calculamos los resultados finales
            If Not mISOrder Then
                'Fase
                'Rendimiento Actual
                If CountReg <> 0 Then
                    mvarPerformance = mvarPerformance / CountReg
                Else
                    mvarPerformance = 0
                End If

                'Rendimiento Piezas Buenas
                If CountGoodReg <> 0 Then
                    mvarGoodPerformance = mvarGoodPerformance / CountGoodReg
                Else
                    mvarGoodPerformance = 0
                End If
            Else
                'Orden
                'Rendimiento Actual
                If mvarTotalTime <> 0 Then
                    mvarPerformance = (mvarPerformance / mvarTotalTime) * 100
                Else
                    mvarPerformance = 0
                End If
                mvarGoodPerformance = 0 : mvarTotalPieces = 0
                mvarTotalPieces1 = 0 : mvarTotalPieces2 = 0 : mvarTotalPieces3 = 0
            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roPerformance::CalculatedPerformanceEx :", ex)
            bolRet = False
            InitValues()
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roPerformance::CalculatedPerformanceEx :", ex)
            bolRet = False
            InitValues()
        Finally

        End Try

        Return bolRet

    End Function

    Private Function CreateSQL(ByVal RepeatCount As Integer) As String
        '
        'Generamos la sentencia SQL a partir de los
        'criterios de seleccion

        Dim sSQL As String
        Dim sWhere As String

        CreateSQL = ""

        sSQL = "@SELECT# sum(PieceTime) as TPieceTime,count(*) as TCount, SUM(Pieces1) as TPieces1,SUM(Pieces2) as TPieces2"
        sSQL = sSQL & ",SUM(Pieces3) as TPieces3,SUM(Value) as TTime, sum(TotalPieces) as TTotalPieces"
        sSQL = sSQL & ",IDJob,IDIncidence "
        If mvarTypePerformance <> roCriteria.ByOrder Then
            If (mvarTypePerformance <> roCriteria.ByMachine And mvarFirstResource = RESOURCE_EMPLOYEE) Or mvarTypePerformance = roCriteria.ByEmployee Then
                sSQL = sSQL & " FROM DailyJobAccruals WHERE"
            Else
                sSQL = sSQL & " FROM DailyMachineAccruals WHERE"
            End If
        Else
            If RepeatCount = 1 Then
                sSQL = sSQL & " FROM DailyJobAccruals WHERE"
            Else
                sSQL = sSQL & " FROM DailyMachineAccruals WHERE"
            End If
        End If

        sWhere = ""

        Select Case mvarTypePerformance
            Case roCriteria.ByEmployee
                sWhere = sWhere & " IDEmployee=" & mvarIDEmployee
            Case roCriteria.ByMachine
                sWhere = sWhere & " IDMachine=" & mvarIDMachine
            Case roCriteria.ByTeam
                sWhere = sWhere & " IDTeam=" & mvarIDTeam
            Case roCriteria.ByOrder
                If RepeatCount = 1 Then
                    sWhere = sWhere & " IDJob IN(@SELECT# ID From Jobs WHERE Path like '" & mvarIDOrder & "\%' and FirstResource = " & RESOURCE_EMPLOYEE & ")"
                Else
                    sWhere = sWhere & " IDJob IN(@SELECT# ID From Jobs WHERE Path like '" & mvarIDOrder & "\%' and FirstResource = " & RESOURCE_MACHINE & ")"
                End If
        End Select

        If mvarBeginDate.HasValue Then
            sWhere = sWhere & IIf(Len(sWhere) = 0, "", " AND ")
            sWhere = sWhere & " Date >=" & SQLSmallDateTime(mvarBeginDate.Value)
        End If

        If mvarFinishDate.HasValue Then
            sWhere = sWhere & IIf(Len(sWhere) = 0, "", " AND ")
            sWhere = sWhere & " Date <=" & SQLSmallDateTime(mvarFinishDate.Value)
        End If

        If InStr(sWhere, "Path like") = 0 Then
            'Consultamos datos de la fase
            sWhere = sWhere & IIf(Len(sWhere) = 0, "", " AND ")
            sWhere = sWhere & " IDJob=" & mvarIDJob
            mISOrder = False
        Else
            mISOrder = True
        End If

        sSQL = sSQL & sWhere
        sSQL = sSQL & " GROUP By IDJob,IDIncidence"
        sSQL = sSQL & " ORDER By IDJob,IDIncidence"

        Return sSQL

    End Function

    Private Function CreateSQLEx() As String
        '
        'Generamos la sentencia SQL a partir de los
        'criterios de seleccion

        Dim sSQL As String
        Dim sWhere As String

        CreateSQLEx = ""

        sSQL = "@SELECT# SUM(Pieces1) as TPieces1,SUM(Pieces2) as TPieces2"
        sSQL = sSQL & ",SUM(Pieces3) as TPieces3,SUM(Value) as TTime"
        sSQL = sSQL & ",IDJob,IDIncidence "
        If mvarTypePerformance <> roCriteria.ByMachine Then
            sSQL = sSQL & " FROM DailyJobAccruals WHERE"
        Else
            sSQL = sSQL & " FROM DailyMachineAccruals WHERE"
        End If

        sWhere = ""

        Select Case mvarTypePerformance
            Case roCriteria.ByEmployee
                sWhere = sWhere & " IDEmployee=" & mvarIDEmployee
            Case roCriteria.ByMachine
                sWhere = sWhere & " IDMachine=" & mvarIDMachine
            Case roCriteria.ByTeam
                sWhere = sWhere & " IDTeam=" & mvarIDTeam
            Case roCriteria.ByOrder
                sWhere = sWhere & " IDJob IN(@SELECT# ID From Jobs WHERE Path like '" & mvarIDOrder & "\%')"

        End Select

        If mvarBeginDate.HasValue Then
            sWhere = sWhere & IIf(Len(sWhere) = 0, "", " AND ")
            sWhere = sWhere & " Date >=" & SQLSmallDateTime(mvarBeginDate.Value)
        End If

        If mvarFinishDate.HasValue Then
            sWhere = sWhere & IIf(Len(sWhere) = 0, "", " AND ")
            sWhere = sWhere & " Date <=" & SQLSmallDateTime(mvarFinishDate.Value)
        End If

        If InStr(sWhere, "Path like") = 0 Then
            'Consultamos datos de la fase
            sWhere = sWhere & IIf(Len(sWhere) = 0, "", " AND ")
            sWhere = sWhere & " IDJob=" & mvarIDJob
            mISOrder = False
        Else
            mISOrder = True
        End If

        sSQL = sSQL & sWhere
        sSQL = sSQL & " GROUP By IDJob,IDIncidence"
        sSQL = sSQL & " ORDER By IDJob,IDIncidence"

        Return sSQL

    End Function

    Private Sub FindTypePiece(ByVal TypePiece As Integer, ByRef CountPiece As Boolean, ByRef ToPerformanceType As Boolean)
        '
        'Buscamos las caracteristicas del tipo de pieza
        '
        Dim sSQL As String
        Dim tb As DataTable

        Try

            sSQL = "@SELECT# Name,CountOnPerformance,IsConsideredValid From SysroPiecetypes WHERE "
            sSQL = sSQL & " ID = " & TypePiece

            CountPiece = False : ToPerformanceType = False
            'Seleccionamos los datos
            tb = CreateDataTable(sSQL)
            If tb.Rows.Count > 0 Then
                CountPiece = tb.Rows(0).Item("IsConsideredValid")
                ToPerformanceType = tb.Rows(0).Item("CountOnPerformance")
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub FindJobDetails(ByVal IDJob As Double, ByRef PieceTimeJob As Double, ByRef PreparationTime As Double, ByRef UnitPieces As Double)
        '
        'Busca los datos de lanzamiento de la fase
        '
        Dim sSQL As String
        Dim tb As DataTable

        Try

            PieceTimeJob = 0
            PreparationTime = 0
            UnitPieces = 0

            sSQL = "@SELECT# PieceTime,PreparationTime, UnitPieces FROM Jobs "
            sSQL = sSQL & " WHERE ID =" & IDJob
            tb = CreateDataTable(sSQL)
            If tb.Rows.Count > 0 Then
                PieceTimeJob = Any2Double(tb.Rows(0).Item("PieceTime"))
                PreparationTime = Any2Double(tb.Rows(0).Item("PreparationTime"))
                UnitPieces = Any2Double(tb.Rows(0).Item("UnitPieces"))
            End If
        Catch ex As Exception

        End Try

    End Sub

    Private Sub InitValues()
        '
        'Inicializamos las variables
        '
        mvarPerformance = 0 : mvarTotalTime = 0 : mvarTotalPieces = 0
        mvarTotalPieces1 = 0 : mvarTotalPieces2 = 0 : mvarTotalPieces3 = 0
        mvarGoodPerformance = 0

        'CountReg = 0 : CountGoodReg = 0

    End Sub

    Private Sub GetAccrualsByPieceType(ByVal oRow As DataRow)
        '
        'Acumulamos los totales por tipo de pieza
        '
        mvarTotalPieces1 = mvarTotalPieces1 + Any2Double(oRow("TPieces1"))
        mvarTotalPieces2 = mvarTotalPieces2 + Any2Double(oRow("TPieces2"))
        mvarTotalPieces3 = mvarTotalPieces3 + Any2Double(oRow("TPieces3"))

    End Sub

    Private Sub GetPiecesAccruals(ByVal PieceType As Integer, ByRef oRow As DataRow, ByRef GoodType As Boolean, ByRef PiecesReg As Double)
        '
        'Acumulamos el total de piezas realizadas
        '

        Select Case PieceType
            Case 1
                PiecesReg = PiecesReg + Any2Double(oRow("TPieces1"))
                GoodType = True
            Case 2
                PiecesReg = PiecesReg + Any2Double(oRow("TPieces2"))
            Case 3
                PiecesReg = PiecesReg + Any2Double(oRow("TPieces3"))
        End Select

    End Sub

    Private Sub FindAmountOrder(ByRef Amount As Double, ByVal Path As String)
        '
        'Buscamos las piezas que hay que realizar a nivel de orden
        '

        Dim sSQL As String
        Dim tb As DataTable
        Dim ID As String
        Dim i As Integer
        Dim Values As Integer
        Dim SelValues As Integer
        Dim itsok As Boolean

        Amount = 1

        'Si es un fase se tienen que mirar las Ordenes a las
        'que pertenece --> Si la fase es es 1\2\1
        '--> 1
        '--> 1\2
        Values = StringItemsCount(Path, "\")
        ID = ""
        For i = 0 To Values - 2
            If i = 0 Then
                ID = String2Item(Path, i, "\")
            Else
                ID = ID & "\" & String2Item(Path, i, "\")
            End If
        Next i
        sSQL = "@SELECT# ID,Amount FROM Orders WHERE LEN(id) <=" & Len(ID) & " AND ID Like '" & String2Item(Path, 0, "\") & "%' AND LEN(id) >=" & Len(String2Item(Path, 0, "\"))
        tb = CreateDataTable(sSQL)
        For Each oRow As DataRow In tb.Rows
            SelValues = StringItemsCount(oRow("id"), "\")
            itsok = True
            For i = 0 To SelValues - 1
                If Not (String2Item(ID, i, "\") = String2Item(oRow("id"), i, "\")) Then
                    itsok = False
                End If
            Next i
            If itsok Then Amount = Amount * Any2Double(oRow("amount"))
        Next

    End Sub

#End Region

End Class