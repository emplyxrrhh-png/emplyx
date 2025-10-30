Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes

Public Class roJob

#Region "Declarations - Constructor"

    'Variables locales
    Private mvarID As Double
    Private mvarJobName As String
    Private mvarPath As String
    Private mvarResource As Double
    Private mvarIDTeam As Double
    Private mvarAmount As Double
    Private mvarUnitPieces As Double
    Private mvarAllowOnlyGrantedTeam As Boolean
    Private mvarPunchID As String
    Private mvarStartDate As Object
    Private mvarEndDate As Object
    Private mvarPreparationTime As Double
    Private mvarPieceTime As Double
    Private mvarAllowPieces As Boolean
    Private mvarPieces() As Boolean
    Private mvarPiecesName() As String
    Private mvarJobPerformance As New roPerformance
    Private mvarIDMachine As Double
    Private mvarAskMachine As Integer
    Private mvarIsQualityJob As Boolean
    Private mvarQualityFields As roJobQualityFields

    Public Sub New()
        Me.mvarID = -1
    End Sub

#End Region

#Region "Properties"

    Public ReadOnly Property IDMachine() As Double
        Get
            Return mvarIDMachine
        End Get
    End Property

    Public ReadOnly Property JobPerformance() As roPerformance
        Get
            Return mvarJobPerformance
        End Get
    End Property

    Public ReadOnly Property AllowPieces() As Boolean
        Get
            Return mvarAllowPieces
        End Get
    End Property

    Public ReadOnly Property Pieces() As Boolean()
        Get
            Return Me.mvarPieces
        End Get
    End Property

    Public ReadOnly Property PiecesName() As String()
        Get
            Return Me.mvarPiecesName
        End Get
    End Property

    Public ReadOnly Property PieceTime() As Double
        Get
            Return mvarPieceTime
        End Get
    End Property

    Public ReadOnly Property PreparationTime() As Double
        Get
            Return mvarPreparationTime
        End Get
    End Property

    Public ReadOnly Property EndDate() As Object
        Get
            Return mvarEndDate
        End Get
    End Property

    Public ReadOnly Property StartDate() As Object
        Get
            Return mvarStartDate
        End Get
    End Property

    Public Property PunchID() As String
        Get
            Return mvarPunchID
        End Get
        Set(ByVal value As String)
            Me.mvarPunchID = value
        End Set
    End Property

    Public ReadOnly Property AllowOnlyGrantedTeam() As Boolean
        Get
            Return mvarAllowOnlyGrantedTeam
        End Get
    End Property

    Public ReadOnly Property UnitPieces() As Double
        Get
            Return mvarUnitPieces
        End Get
    End Property

    Public ReadOnly Property Amount() As Double
        Get
            Return mvarAmount
        End Get
    End Property

    Public ReadOnly Property IDTeam() As Double
        Get
            Return mvarIDTeam
        End Get
    End Property

    Public ReadOnly Property JobName() As String
        Get
            Return mvarJobName
        End Get
    End Property

    Public Property ID() As Double
        Get
            Return mvarID
        End Get
        Set(ByVal value As Double)
            mvarID = value
        End Set
    End Property

    Public ReadOnly Property AskMachine() As Integer
        Get
            Return Me.mvarAskMachine
        End Get
    End Property

    Public ReadOnly Property Resource() As Integer
        Get
            Return Me.mvarResource
        End Get
    End Property

    Public ReadOnly Property IsQualityJob() As Boolean
        Get
            Return Me.mvarIsQualityJob
        End Get
    End Property

    Public ReadOnly Property QualityFields() As roJobQualityFields
        Get
            Return Me.mvarQualityFields
        End Get
    End Property

#End Region

#Region "Methods"

    Public Function Load(ByVal LogHandle As roLog, Optional ByVal GetPerformance As String = "", Optional ByVal bolFromPunchID As Boolean = False) As Boolean
        '
        ' Carga datos de la Fase
        '
        '
        Dim bolRet As Boolean = False
        Dim dsTeam As New DataSet
        Dim dsJob As New DataSet
        Dim sSQL As String
        Dim rdPositioned As DataTable
        Try

            If (mvarID = 0 And Not bolFromPunchID) Or
               (mvarPunchID = 0 And bolFromPunchID) Then
                Return False
                Exit Function
            End If

            sSQL = "@SELECT# * FROM Jobs "
            If Not bolFromPunchID Then
                sSQL &= "WHERE ID = " & mvarID.ToString
            Else
                sSQL &= "WHERE PunchID = " & mvarPunchID.ToString
            End If
            rdPositioned = CreateDataTable(sSQL, )

            If rdPositioned.Rows.Count = 1 Then

                mvarID = Any2Double(rdPositioned.Rows(0).Item("ID"))
                mvarJobName = Any2String(rdPositioned.Rows(0).Item("Name"))
                mvarIDTeam = Any2Double(rdPositioned.Rows(0).Item("IDTeam"))
                mvarUnitPieces = Any2Double(rdPositioned.Rows(0).Item("UnitPieces"))
                mvarAllowOnlyGrantedTeam = IIf(rdPositioned.Rows(0).Item("AllowOnlyGrantedTeam") = 0, False, True)
                mvarPunchID = Any2String(rdPositioned.Rows(0).Item("PunchID"))
                mvarIDMachine = Any2Double(rdPositioned.Rows(0).Item("IDMachine"))
                mvarAskMachine = Any2Integer(rdPositioned.Rows(0).Item("AskMachine"))
                mvarStartDate = rdPositioned.Rows(0).Item("StartDate")
                mvarEndDate = rdPositioned.Rows(0).Item("EndDate")
                mvarPreparationTime = Any2Double(rdPositioned.Rows(0).Item("PreparationTime"))
                mvarPieceTime = Any2Double(rdPositioned.Rows(0).Item("PieceTime"))
                mvarAllowPieces = IIf(rdPositioned.Rows(0).Item("AllowPieces") = 0, False, True)
                mvarPieces = Me.GetPieces(LogHandle)
                mvarPath = Any2String(rdPositioned.Rows(0).Item("Path"))
                mvarResource = Any2Double(rdPositioned.Rows(0).Item("FirstResource"))

                'Carga datos del Rendimiento Actual
                mvarJobPerformance.IDJob = mvarID
                mvarJobPerformance.TypePerformance = roPerformance.roCriteria.ByJob
                mvarJobPerformance.FirstResource = mvarResource
                If GetPerformance <> "NO" Then mvarJobPerformance.CalculatedPerformance(LogHandle)

                ' Cargar información campos calidad (QualityFields)
                Me.mvarQualityFields = Nothing
                Try
                    mvarIsQualityJob = rdPositioned.Rows(0).Item("IsQualityJob")
                Catch
                    mvarIsQualityJob = False
                End Try
                If mvarIsQualityJob Then
                    Me.mvarQualityFields = New roJobQualityFields(LogHandle)
                    If Me.mvarQualityFields.Fields.Length = 0 Then
                        Me.mvarIsQualityJob = False
                    End If
                End If

                bolRet = True
            Else

                bolRet = False

            End If
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roJob::Load :", ex)
            mvarID = 0
            bolRet = False
        End Try

        Return bolRet

    End Function

    Public Sub GetTeoricAmount_Time(ByRef TeoricTime As Double, ByRef TeoricPieces As Double, ByVal LogHandle As roLog)
        '
        'Tiempo concecido y piezas a realizar de una fase
        '
        Dim Amount As Double

        Try

            TeoricTime = 0 : TeoricPieces = 0

            Amount = 1

            FindAmountOrder(LogHandle, Amount)

            TeoricTime = (mvarUnitPieces * mvarPieceTime * Amount) + mvarPreparationTime
            TeoricPieces = mvarUnitPieces * Amount
            mvarAmount = Amount
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roJob::GetTeoricAmount_Time :", ex)
        End Try

    End Sub

    Private Sub FindAmountOrder(ByRef LogHandle As roLog, ByRef Amount As Double)
        '
        'Buscamos las piezas que hay que realizar a nivel de orden
        '

        Dim sSQL As String
        Dim ads As DataTable
        Dim ID As String
        Dim i As Integer
        Dim Values As Integer
        Dim SelValues As Integer
        Dim itsok As Boolean

        Try

            Amount = 1

            'Si es un fase se tienen que mirar las Ordenes a las
            'que pertenece --> Si la fase es es 1\2\1
            '--> 1
            '--> 1\2
            Values = StringItemsCount(mvarPath, "\")
            ID = ""
            For i = 0 To Values - 2
                If i = 0 Then
                    ID = String2Item(mvarPath, i, "\")
                Else
                    ID = ID & "\" & String2Item(mvarPath, i, "\")
                End If
            Next i
            sSQL = "@SELECT# ID,Amount FROM Orders WHERE LEN(id) <=" & Len(ID) & " AND ID Like '" & String2Item(mvarPath, 0, "\") & "%' AND LEN(id) >=" & Len(String2Item(mvarPath, 0, "\"))
            ads = CreateDataTable(sSQL)
            If ads IsNot Nothing Then
                For Each oRow As DataRow In ads.Rows
                    SelValues = StringItemsCount(oRow("id"), "\")
                    itsok = True
                    For i = 0 To SelValues - 1
                        If Not (String2Item(ID, i, "\") = String2Item(oRow("id"), i, "\")) Then
                            itsok = False
                        End If
                    Next i
                    If itsok Then Amount = Amount * Any2Double(oRow("amount"))
                Next
            End If
            ads.Dispose()
            ads = Nothing
        Catch ex As Exception
            LogHandle.logMessage(roLog.EventType.roError, "roJob::FindAmountOrder :", ex)
        End Try
    End Sub

    Private Function GetPieces(ByVal oLog As roLog) As Boolean()

        Dim bolRet() As Boolean

        ReDim bolRet(-1)

        Try

            Dim tb As DataTable = CreateDataTable("@SELECT# IsUsed,Name FROM sysroPieceTypes WHERE [ID] > 0 ORDER BY [ID]", )
            ReDim bolRet(tb.Rows.Count - 1)
            ReDim mvarPiecesName(tb.Rows.Count - 1)

            For n As Integer = 0 To tb.Rows.Count - 1
                bolRet(n) = tb.Rows(n).Item("IsUsed")
                mvarPiecesName(n) = tb.Rows(n).Item("Name")
            Next
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJob::GetPieces :", ex)
        End Try

        Return bolRet

    End Function

    Public Function UpdateStartDate(ByVal xStartDate As Date, ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False
        Dim strSQL As String

        Try

            strSQL = "@UPDATE# Jobs " &
                     "Set StartDate = " & Any2Time(xStartDate).SQLDateTime & " " &
                     "WHERE ID = " & Me.mvarID.ToString
            If ExecuteSql(strSQL) Then

                Dim cmd As DbCommand
                Dim da As DbDataAdapter
                Dim tbOrder As DataTable

                For n As Integer = 0 To StringItemsCount(Me.mvarPath, "\") - 2

                    strSQL = "@SELECT# ID, BeginDate FROM Orders " &
                             "WHERE ID = '" & Trim(String2Item(Me.mvarPath, n, "\")) & "'"
                    cmd = CreateCommand(strSQL)
                    da = CreateDataAdapter(cmd, True)
                    tbOrder = New DataTable
                    da.Fill(tbOrder)

                    If tbOrder.Rows.Count = 1 Then
                        If IsDBNull(tbOrder.Rows(0).Item("BeginDate")) Then
                            tbOrder.Rows(0).Item("BeginDate") = xStartDate
                            da.Update(tbOrder)
                        End If
                    End If

                Next

                Me.mvarStartDate = xStartDate.Date

                bolRet = True

            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJob::UpdateStartDate :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJob::UpdateStartDate :", ex)
        End Try

        Return bolRet

    End Function

    Public Function UpdateMachine(ByVal intIDMachine As Integer, ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False

        If Me.mvarIDMachine = 0 AndAlso Me.mvarAskMachine = 1 Then

            Try

                Dim strSQL As String
                strSQL = "@UPDATE# Jobs " &
                         "SET IDMachine = " & intIDMachine.ToString & ", " &
                             "AskMachine = 0 " &
                         "WHERE [ID] = " & Me.mvarID.ToString

                bolRet = ExecuteSql(strSQL)

                If bolRet Then
                    Me.mvarIDMachine = intIDMachine
                    Me.mvarAskMachine = 0
                End If
            Catch ex As DbException
                oLog.logMessage(roLog.EventType.roError, "roJob::UpdateMachine :", ex)
            Catch ex As Exception
                oLog.logMessage(roLog.EventType.roError, "roJob::UpdateMachine :", ex)
            End Try
        Else
            bolRet = True
        End If

        Return bolRet

    End Function

    Public Function HasMoves(ByVal oLog As roLog) As Boolean

        Dim bolRet As Boolean = False

        Try

            Dim strSQL As String
            strSQL = "@SELECT# COUNT(*) FROM EmployeeJobMoves " &
                     "WHERE IDJob = " & Me.mvarID.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, "")
            If tb.Rows.Count > 0 Then
                bolRet = (tb.Rows(0).Item(0) > 0)
            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJob::HasMoves :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJob::HasMoves :", ex)
        End Try

        Return bolRet

    End Function

    Public Function AutomaticPreparationIncidence(ByVal oLog As roLog) As Integer

        Dim intRet As Integer = 0

        Dim rd As DbDataReader = Nothing

        ' Si es la primera vez que se inicia esta fase y tiene tiempo de preparación
        If Not Me.HasMoves(oLog) AndAlso Me.PreparationTime > 0 Then

            Dim oSettings As New roSettings("HKEY_LOCAL_MACHINE\Software\Robotics\VisualTime")
            If oSettings.GetVTSetting(eKeys.AutomaticPreparation) Then ' Está configurada la opción de incidencia de preparación automàtica
                ' Si la operación tiene tiempo de preparación, iniciar el trabajo con esa incidencia de forma automática

                Try

                    Dim strSQL As String = "@SELECT# [ID] FROM JobIncidences " &
                                           "WHERE LOWER(CONVERT(varchar, Description)) LIKE '%preparationtime%'"
                    rd = CreateDataReader(strSQL)

                    If rd.Read Then
                        intRet = rd("ID")
                    End If
                    rd.Close()
                Catch ex As DbException
                    oLog.logMessage(roLog.EventType.roError, "roJob::AutomaticPreparationIncidence :", ex)
                Catch ex As Exception
                    oLog.logMessage(roLog.EventType.roError, "roJob::AutomaticPreparationIncidence :", ex)
                Finally
                    If rd IsNot Nothing AndAlso Not rd.IsClosed Then rd.Close()
                End Try

            End If

        End If

        Return intRet

    End Function

    Public Function GetTotalPieces(ByVal oLog As roLog) As Double

        Dim dblRet As Double = 0
        Try

            Dim strSQL As String
            Dim strFields As String = ""

            strSQL = "@SELECT# [ID] FROM sysroPieceTypes WHERE [ID] > 0 AND IsConsideredValid = 1"
            Dim tbTypes As DataTable = CreateDataTable(strSQL, )
            For Each oRow As DataRow In tbTypes.Rows
                If strFields <> "" Then strFields &= " + "
                strFields &= "ISNULL(Pieces" & oRow("ID") & ",0)"
            Next

            If strFields <> "" Then

                strSQL = "@SELECT# SUM(" & strFields & ") FROM EmployeeJobMoves " &
                         "WHERE IDJob = " & Me.ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb.Rows.Count = 1 AndAlso Not IsDBNull(tb.Rows(0).Item(0)) Then
                    dblRet = tb.Rows(0).Item(0)
                End If

            End If
        Catch ex As DbException
            oLog.logMessage(roLog.EventType.roError, "roJob::GetTotalPieces :", ex)
        Catch ex As Exception
            oLog.logMessage(roLog.EventType.roError, "roJob::GetTotalPieces :", ex)
        End Try

        Return dblRet

    End Function

    Public Function GetJobOrder(ByRef strIDOrder As String, ByRef strOrderName As String, ByVal _Log As roLog) As Boolean

        Dim bolRet As Boolean = False
        Try

            Dim strSQL As String = "@SELECT# Orders.ID, Orders.Name FROM Orders, Jobs " &
                                   "WHERE substring(Jobs.path,1,len(Jobs.path) - CHARINDEX( '\',reverse(Jobs.path))) = Orders.ID AND " &
                                         "Jobs.ID = " & Me.ID.ToString
            Dim tb As DataTable = CreateDataTable(strSQL, )
            If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                strIDOrder = tb.Rows(0).Item("ID")
                strOrderName = tb.Rows(0).Item("Name")

                bolRet = True

            End If
        Catch ex As DbException
            _Log.logMessage(roLog.EventType.roError, "roJob::GetJobOrder :", ex)
        Catch ex As Exception
            _Log.logMessage(roLog.EventType.roError, "roJob::GetJobOrder :", ex)
        End Try

        Return bolRet

    End Function

#Region "Helper methods"

    Public Shared Function GetActiveOrders(ByVal _Log As roLog) As DataTable

        Dim tbRet As DataTable = Nothing
        Try
            Dim strSQL As String = "@SELECT# * FROM Orders WHERE EndDate IS NULL AND ISNUMERIC(ID) = 1 ORDER BY Name"
            tbRet = CreateDataTable(strSQL, )
        Catch ex As DbException
            _Log.logMessage(roLog.EventType.roError, "roJob::GetActiveOrders :", ex)
        Catch ex As Exception
            _Log.logMessage(roLog.EventType.roError, "roJob::GetActiveOrders :", ex)
        End Try

        Return tbRet

    End Function

    Public Shared Function GetJobsFromOrder(ByVal strIDOrder As String, ByVal _Log As roLog) As DataTable

        Dim tbRet As DataTable = Nothing

        Try

            Dim strSQL As String = "@SELECT# * FROM Jobs " &
                                   "WHERE substring(path,1,len(path) - CHARINDEX( '\',reverse(path))) = @OrderID AND " &
                                         "EndDate IS NULL " &
                                   "ORDER BY Name"

            tbRet = New DataTable("Jobs")
            Dim cmd As DbCommand = CreateCommand(strSQL)
            AddParameter(cmd, "@OrderID", DbType.String, 30)
            cmd.Parameters("@OrderID").Value = strIDOrder

            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
            da.Fill(tbRet)
        Catch ex As DbException
            _Log.logMessage(roLog.EventType.roError, "roJob::GetJobsFromOrder :", ex)
        Catch ex As Exception
            _Log.logMessage(roLog.EventType.roError, "roJob::GetJobsFromOrder :", ex)
        End Try

        Return tbRet

    End Function

#End Region

#End Region

End Class