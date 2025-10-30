Imports System.Data.Common
Imports System.Drawing
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roTypes

Namespace AccessMove

    <DataContract()>
    Public Enum eSortType
        <EnumMember> Ascendend
        <EnumMember> Descended
    End Enum

    <DataContract()>
    Public Enum eTypeAccessMoves
        <EnumMember> AccessMoves
        <EnumMember> InvalidAccessMoves
        <EnumMember> AllAccessMoves
    End Enum

    <DataContract()>
    Public Class roAccessMove

        Public Enum InvalidTypes
            <EnumMember> NOHP_  ' Acceso denegado por documentación incorrecta
            <EnumMember> NTIME_ ' Acceso denegado por fuera de hora
            <EnumMember> NRDR_  ' Acceso denegado por lector invàlido
            <EnumMember> NERR_  ' Error interno de la aplicación
            <EnumMember> NCON_  ' Acceso denegado: sin contrato
            <EnumMember> NFLD_  ' Acceso denegado: campos de la ficha del empleado
        End Enum

#Region "Declarations - Constructors"

        Private oState As roAccessMoveState

        Private lngID As Long
        Private intIDEmployee As Integer
        Private xDateTime As Nullable(Of DateTime)
        Private intIDTerminal As Nullable(Of Integer)
        Private intIDReader As Nullable(Of Integer)
        Private strType As Nullable(Of Char)
        Private intIDZone As Nullable(Of Integer)
        Private oCapture As Image
        Private lngIDCapture As Long
        Private bolInvalid As Boolean
        Private oInvalidType As InvalidTypes
        Private strDetail As String
        Private intIdTurn As Integer

        Public Sub New()

            Me.oState = New roAccessMoveState()

            Me.ID = 0

        End Sub

        Public Sub New(ByVal _IDEmployee As Integer, ByVal _ID As Long, ByVal _Invalid As Boolean, ByVal _State As roAccessMoveState)

            Me.oState = _State

            Me.intIDEmployee = _IDEmployee
            Me.bolInvalid = _Invalid
            Me.lngID = _ID

            Me.Load()

        End Sub

#End Region

#Region "Properties"

        <IgnoreDataMember()>
        Public Property State() As roAccessMoveState
            Get
                Return Me.oState
            End Get
            Set(ByVal value As roAccessMoveState)
                Me.oState = value
            End Set
        End Property
        <DataMember()>
        Public Property ID() As Long
            Get
                Return Me.lngID
            End Get
            Set(ByVal value As Long)
                Me.lngID = value
            End Set
        End Property
        <DataMember()>
        Public Property IDEmployee() As Integer
            Get
                Return Me.intIDEmployee
            End Get
            Set(ByVal value As Integer)
                Me.intIDEmployee = value
            End Set
        End Property
        <DataMember()>
        Public Property DateTime() As Nullable(Of DateTime)
            Get
                Return Me.xDateTime
            End Get
            Set(ByVal value As Nullable(Of DateTime))
                Me.xDateTime = value
            End Set
        End Property
        <DataMember()>
        Public Property IDTerminal() As Nullable(Of Integer)
            Get
                Return Me.intIDTerminal
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDTerminal = value
            End Set
        End Property
        <DataMember()>
        Public Property IDReader() As Nullable(Of Integer)
            Get
                Return Me.intIDReader
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDReader = value
            End Set
        End Property
        <DataMember()>
        Public Property Type() As Nullable(Of Char)
            Get
                Return Me.strType
            End Get
            Set(ByVal value As Nullable(Of Char))
                Me.strType = value
            End Set
        End Property
        <DataMember()>
        Public Property IDZone() As Nullable(Of Integer)
            Get
                Return Me.intIDZone
            End Get
            Set(ByVal value As Nullable(Of Integer))
                Me.intIDZone = value
            End Set
        End Property
        <DataMember()>
        Public Property Capture() As Image
            Get
                Return Me.oCapture
            End Get
            Set(ByVal value As Image)
                Me.oCapture = value
            End Set
        End Property
        <DataMember()>
        Public Property Invalid() As Boolean
            Get
                Return Me.bolInvalid
            End Get
            Set(ByVal value As Boolean)
                Me.bolInvalid = value
            End Set
        End Property
        <DataMember()>
        Public Property InvalidType() As InvalidTypes
            Get
                Return Me.oInvalidType
            End Get
            Set(ByVal value As InvalidTypes)
                Me.oInvalidType = value
            End Set
        End Property
        <DataMember()>
        Public Property Detail() As String
            Get
                Return Me.strDetail
            End Get
            Set(ByVal value As String)
                Me.strDetail = value
            End Set
        End Property
        <DataMember()>
        Public Property IdTurn() As Integer
            Get
                Return Me.intIdTurn
            End Get
            Set(ByVal value As Integer)
                Me.intIdTurn = value
            End Set
        End Property

#End Region

#Region "Methods"

        Public Sub Load()

            Try

                Me.xDateTime = Nothing
                Me.intIDReader = Nothing
                Me.strType = Nothing
                Me.intIDZone = Nothing
                Me.oCapture = Nothing
                Me.lngIDCapture = -1
                Me.IdTurn = 0

                Dim strSQL As String

                If Not Me.bolInvalid Then

                    strSQL = "@SELECT# * FROM AccessMoves WHERE [ID] = " & Me.lngID.ToString

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                        Me.intIDEmployee = tb.Rows(0).Item("IDEmployee")
                        If Not IsDBNull(tb.Rows(0).Item("DateTime")) Then
                            Me.xDateTime = tb.Rows(0).Item("DateTime")
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDReader")) Then
                            Me.intIDReader = tb.Rows(0).Item("IDReader")
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("Type")) Then
                            Me.strType = tb.Rows(0).Item("Type")
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDZone")) Then
                            Me.intIDZone = tb.Rows(0).Item("IDZone")
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDCapture")) Then
                            Me.lngIDCapture = tb.Rows(0).Item("IDCapture")
                            Dim oCaptureState As New Capture.roCaptureState(Me.oState.IDPassport, Me.oState.Context, Me.oState.ClientAddress)
                            Dim oCaptureObj As New Capture.roCapture(tb.Rows(0).Item("IDCapture"), oCaptureState)
                            Me.oCapture = oCaptureObj.CaptureImage
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDTurn")) Then
                            Me.intIdTurn = tb.Rows(0).Item("IDTurn")
                        End If

                    End If
                Else

                    strSQL = "@SELECT# * FROM InvalidAccessMoves WHERE [ID] = " & Me.lngID.ToString

                    Dim tb As DataTable = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count = 1 Then

                        Me.intIDEmployee = tb.Rows(0).Item("IDEmployee")
                        If Not IsDBNull(tb.Rows(0).Item("DateTime")) Then
                            Me.xDateTime = tb.Rows(0).Item("DateTime")
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDTerminal")) Then
                            Me.intIDTerminal = tb.Rows(0).Item("IDTerminal")
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDReader")) Then
                            Me.intIDReader = tb.Rows(0).Item("IDReader")
                        End If
                        Me.strType = "A"
                        If Not IsDBNull(tb.Rows(0).Item("IDZone")) Then
                            Me.intIDZone = tb.Rows(0).Item("IDZone")
                        End If
                        Me.oInvalidType = Any2Integer(tb.Rows(0).Item("Type"))
                        Me.strDetail = Any2String(tb.Rows(0).Item("Detail"))

                        If Not IsDBNull(tb.Rows(0).Item("IDCapture")) Then
                            Me.lngIDCapture = tb.Rows(0).Item("IDCapture")
                            Dim oCaptureState As New Capture.roCaptureState(Me.oState.IDPassport, Me.oState.Context, Me.oState.ClientAddress)
                            Dim oCaptureObj As New Capture.roCapture(tb.Rows(0).Item("IDCapture"), oCaptureState)
                            Me.oCapture = oCaptureObj.CaptureImage
                        End If
                        If Not IsDBNull(tb.Rows(0).Item("IDTurn")) Then
                            Me.intIdTurn = tb.Rows(0).Item("IDTurn")
                        End If

                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessMove::Load")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessMove::Load")
            Finally

            End Try

        End Sub

        Public Function Save() As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try

                ' Informamos en log
                Dim strType As String = Me.strType
                If Me.bolInvalid Then
                    strType = Me.oInvalidType.ToString
                End If
                Dim strPunch As String = "IDEmployee=" & Me.IDEmployee.ToString & ";" &
                                         "DateTime=" & Me.DateTime.Value.ToString & ";" &
                                         "IDTerminal=" & Any2String(Me.IDTerminal) & ";" &
                                         "IDReader=" & Any2String(Me.IDReader) & ";" &
                                         "Type=" & strType & ";" &
                                         "IDZone=" & Any2String(Me.IDZone) & ";" &
                                         "Detail=" & Me.strDetail & ";" &
                                         "Capture=" & IIf(Me.oCapture IsNot Nothing, "True", "False") & ";" &
                                         "IdTurn=" & Me.IdTurn

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.DeleteHistory()

                ' Guardar captura imagen
                If Me.oCapture IsNot Nothing Then
                    Dim oCaptureState As New Capture.roCaptureState(Me.oState.IDPassport, Me.oState.Context, Me.oState.ClientAddress)
                    Dim oCaptureObj As New Capture.roCapture(Me.lngIDCapture, oCaptureState)
                    oCaptureObj.DateTime = Me.xDateTime
                    oCaptureObj.CaptureImage = Me.oCapture
                    If oCaptureObj.Save() Then
                        Me.lngIDCapture = oCaptureObj.ID
                    End If
                End If

                Dim strTableName As String = "AccessMoves"
                If Me.bolInvalid Then
                    strTableName = "InvalidAccessMoves"
                End If

                Dim strSQL As String
                strSQL = "@SELECT# * FROM " & strTableName & " WHERE [ID] = " & Me.lngID.ToString

                Dim tbMove As New DataTable(strTableName)
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tbMove)

                Dim oRow As DataRow = Nothing
                If tbMove.Rows.Count = 0 Then
                    oRow = tbMove.NewRow
                    Me.lngID = -1
                Else
                    oRow = tbMove.Rows(0)
                End If

                oRow("IDEmployee") = Me.intIDEmployee
                If Me.xDateTime.HasValue Then
                    oRow("DateTime") = Me.xDateTime.Value
                Else
                    oRow("DateTime") = DBNull.Value
                End If
                If Me.bolInvalid Then
                    If Me.intIDTerminal.HasValue Then
                        oRow("IDTerminal") = Me.intIDTerminal.Value
                    Else
                        oRow("IDTerminal") = DBNull.Value
                    End If
                    If Me.intIDReader.HasValue Then
                        oRow("IDReader") = Me.intIDReader.Value
                    Else
                        oRow("IDReader") = DBNull.Value
                    End If
                Else
                    'En la tabla AccessMoves no hay IDTerminal, solo IDReader que corresponde al Terminal
                    If Me.intIDTerminal.HasValue Then
                        oRow("IDReader") = Me.intIDTerminal.Value
                    Else
                        oRow("IDReader") = DBNull.Value
                    End If
                End If
                If Not Me.bolInvalid Then
                    If Me.strType.HasValue Then
                        oRow("Type") = Me.strType.Value
                    Else
                        oRow("Type") = DBNull.Value
                    End If
                Else
                    oRow("Type") = Me.oInvalidType
                End If

                If Me.intIDZone.HasValue Then
                    oRow("IDZone") = Me.intIDZone.Value
                Else
                    oRow("IDZone") = DBNull.Value
                End If

                If Me.bolInvalid Then
                    oRow("Detail") = Me.strDetail
                End If

                If Me.lngIDCapture > 0 Then
                    oRow("IDCapture") = Me.lngIDCapture
                Else
                    oRow("IDCapture") = DBNull.Value
                End If

                If UCase(strTableName) = "ACCESSMOVES" Then
                    oRow("IDTurn") = Me.intIdTurn
                End If

                If Me.lngID = -1 Then
                    tbMove.Rows.Add(oRow)
                End If

                da.Update(tbMove)

                If Me.lngID <= 0 Then
                    Dim tb As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM " & strTableName & " WHERE IDEmployee = " & Me.intIDEmployee.ToString & " " &
                                                              "ORDER BY [ID] DESC")
                    If tb.Rows.Count > 0 Then
                        Me.lngID = tb.Rows(0).Item("ID")
                    End If
                End If

                bolRet = True
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessMove::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessMove::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina el històrico de accesos
        ''' </summary>
        ''' <param name="oActiveTransaction"></param>
        ''' <remarks></remarks>
        Private Sub DeleteHistory()

            Try

                Dim strSQL As String

                Dim oParam As New roParameters("OPTIONS", True)
                Dim intNumMonthsAccess As Integer = oParam.Parameter(Parameters.NumMonthsAccess)

                Dim xLastAccessDate As Nullable(Of DateTime) = Nothing
                Dim xLastGoodAccess As DateTime

                If intNumMonthsAccess <> 0 Then
                    ' Obtenemos la útima fecha que ha habido accesos

                    Dim strTableName As String = "AccessMoves"
                    If Me.bolInvalid Then strTableName = "InvalidAccessMoves"

                    strSQL = "@SELECT# MAX(DateTime) FROM " & strTableName
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        If Not IsDBNull(tb.Rows(0).Item(0)) Then
                            xLastAccessDate = tb.Rows(0).Item(0)
                        End If
                    End If

                    ' Comprobamos que no haya mas de 2 meses entre la ultima vez que
                    ' se hizo un acceso y la fecha de hoy para preever posibles
                    ' errores en la fecha del PC
                    If xLastAccessDate.HasValue AndAlso DateDiff(DateInterval.Month, Now, xLastAccessDate.Value) > 2 Then Exit Sub

                    ' Primera fecha de acceso que hay que guardar
                    xLastGoodAccess = Now.Date.AddMonths(intNumMonthsAccess * -1)

                    ' Borramos todos los accesos que sean anteriores a la primera fecha
                    ' que hay que gaurdar accesos historicos

                    strSQL = "@DELETE# FROM " & strTableName & " WHERE DateTime < " & Any2Time(xLastGoodAccess).SQLDateTime
                    ExecuteSql(strSQL)

                    ' Guardamos la fecha actual
                    ''oParam.Parameter(Parameters.LastDateAccess) = Any2String(Any2Time(Now).DateOnly)

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roAccessMove::DeleteHistory")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roAccessMove::DeleteHistory")
            End Try

        End Sub

#End Region

#Region "Funciones Helper Cubo"

        '=========================================
        '============= CUBO ======================
        '=========================================
        Public Shared Function GetAccessPlatesViewsDataSet(ByVal IdPassport As Integer, ByVal oState As roAccessMoveState) As DataSet

            Dim dsRet As DataSet = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID, IdView, NameView, Description, DateView FROM sysroSchedulerViews WHERE TypeView = 'A' AND IdPassport = " & IdPassport & " ORDER BY ID "

                dsRet = CreateDataSet(strSQL, "AccessPlatesViews")

                If Not dsRet Is Nothing AndAlso dsRet.Tables.Count > 0 Then
                    Dim tb As DataTable = dsRet.Tables(0)
                    tb.PrimaryKey = New DataColumn() {tb.Columns("ID")}
                    tb.AcceptChanges()
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTask::GetAccessPlatesViewsDataSet")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::GetAccessPlatesViewsDataSet")
            Finally

            End Try

            Return dsRet
        End Function

        Public Shared Function GetAccessPlatesViewbyID(ByVal ID As Integer, ByVal IdPassport As Integer, ByVal oState As roAccessMoveState) As DataSet

            Dim dsRet As DataSet = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroSchedulerViews WHERE TypeView = 'A' AND ID = " & ID & " AND IdPassport = " & IdPassport & " ORDER BY ID "
                dsRet = CreateDataSet(strSQL, "SchedulerViews")
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTask::GetAccessPlatesViewbyID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::GetAccessPlatesViewbyID")
            Finally

            End Try

            Return dsRet
        End Function

        Public Shared Function NewAccessPlatesView(ByVal IdView As Integer, ByVal IdPassport As Integer, ByVal NameView As String, ByVal Description As String, ByVal DateView As DateTime,
                                                 ByVal Employees As String, ByVal DateInf As DateTime, ByVal DateSup As DateTime, ByVal CubeLayout As String, ByVal FilterData As String,
                                                 ByVal oState As roAccessMoveState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("SchedulerViews")
                Dim strSQL As String = "@SELECT# * FROM sysroSchedulerViews WHERE 1=2"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("ID") = GetNextAccessPlatesViewID(oState)
                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                End If

                oRow("IdView") = IdView
                oRow("IdPassport") = IdPassport
                oRow("NameView") = NameView
                oRow("Description") = Description
                oRow("DateView") = DateView
                oRow("Employees") = Employees
                oRow("DateInf") = DateInf
                oRow("DateSup") = DateSup
                oRow("CubeLayout") = CubeLayout
                oRow("TypeView") = "A"
                oRow("FilterData") = FilterData

                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTask::NewAccessPlatesView")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::NewAccessPlatesView")
            End Try

            Return bolRet
        End Function

        Private Shared Function GetNextAccessPlatesViewID(ByVal oState As roAccessMoveState) As Integer

            Dim intRet As Integer = 0

            Try
                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM sysroSchedulerViews "
                Dim tb As DataTable = CreateDataTable(strSQL)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roTask::GetNextAccessPlatesViewID")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::GetNextAccessPlatesViewID")
            End Try

            Return intRet

        End Function

        Public Shared Function DeleteAccessPlatesView(ByVal ID As Integer, ByVal oState As roAccessMoveState) As Boolean

            Dim bolRet As Boolean = True

            Try

                Dim strSQL As String = "@DELETE# FROM sysroSchedulerViews WHERE ID = " & ID
                bolRet = ExecuteSql(strSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roTask::DeleteAccessPlatesView")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roTask::DeleteAccessPlatesView")
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace