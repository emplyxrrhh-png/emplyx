Imports Robotics.Web.Base
Imports Robotics.WebControls

Partial Class Scheduler_Controls_LocalizationControl
    Inherits Robotics.WebControls.SchedulerCtrlBase
    Implements ISchedulerControls

    Public Sub New()

    End Sub

#Region "Methods"

    Public Sub RefreshControl(Optional ByVal reload As Boolean = False) Implements ISchedulerControls.RefreshControl
        LoadData(True)
    End Sub

    Public Sub Hide() Implements ISchedulerControls.Hide
    End Sub

    ''' <summary>
    ''' Guarda les dades del control
    ''' </summary>
    ''' <returns>No guarda cap dada. Retorna false</returns>
    ''' <remarks>Funcio inclosa per compatibilitat amb ISchedulerControls</remarks>
    Public Function SaveData() As Boolean Implements ISchedulerControls.SaveData
        Return False
    End Function

    Public Sub LoadData(Optional ByVal reload As Boolean = False)
        LocalizationMapControl1.Data = getCoordsData(reload)
        LocalizationMapControl1.LoadMap()
    End Sub

    Private Function getCoordsData(ByVal reload As Boolean) As String
        Dim sb As StringBuilder = New StringBuilder()
        Dim dv As DataView = Me.PunchesDataView(reload)
        Dim isFirstRow As Boolean = True
        For Each dr As DataRowView In dv
            Dim point As String = dr("Location").ToString().Replace(" ", "")
            point = point.Replace(",", ";")
            If isFirstRow Then
                isFirstRow = False
            Else
                sb.Append("|")
            End If
            If Not String.IsNullOrEmpty(point) AndAlso point <> "0;0" Then
                sb.Append(point)
            End If
        Next
        Return sb.ToString()
    End Function

    Public Sub UpdateMap(ByVal dvPunches As DataView)
        ViewState("Moves_PunchesData") = dvPunches.Table
        ViewState("Moves_MovesLastIDEmployee") = Me.IDEmployee
        ViewState("Moves_MovesLastDate") = Me.DateMoves
        LoadData()
    End Sub

#End Region

#Region "Properties"

    Private ReadOnly Property PunchesDataView(Optional ByVal bolReload As Boolean = False) As DataView
        Get
            ''MyLog.WriteLog(Me, "Init GET PunchesDataView")
            Dim dv As DataView = New DataView(Me.PunchesData(bolReload))
            dv.Sort = "DateTime, Id"
            ''MyLog.WriteLog(Me, "End GET PunchesDataView")
            Return dv
        End Get
    End Property

    Private Property PunchesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Get
            ' Lee todos los movimientos en la tabla
            Dim tb As DataTable = ViewState("Moves_PunchesData")
            Dim intLastIDEmployee As Nullable(Of Integer) = ViewState("Moves_MovesLastIDEmployee")
            Dim xLastDate As Nullable(Of Date) = ViewState("Moves_MovesLastDate")

            If bolReload OrElse tb Is Nothing OrElse Not intLastIDEmployee.HasValue OrElse intLastIDEmployee.Value <> Me.IDEmployee OrElse
                                Not xLastDate.HasValue OrElse xLastDate.Value <> Me.DateMoves Then
                Dim startDate As DateTime = New DateTime(Me.DateMoves.Year, Me.DateMoves.Month, Me.DateMoves.Day, 0, 0, 0)
                Dim endDate As DateTime = New DateTime(Me.DateMoves.Year, Me.DateMoves.Month, Me.DateMoves.Day, 23, 59, 59)
                tb = API.PunchServiceMethods.GetPunchesDataTable(Me.Page, startDate, endDate, Nothing, Nothing,
                    Me.IDEmployee, , , , , , "1,2,3,5,7", , , , )
                If tb IsNot Nothing AndAlso tb.Rows.Count = 0 Then
                    tb.Rows.Add(tb.NewRow)
                End If
                ViewState("Moves_PunchesData") = tb
                ViewState("Moves_MovesLastIDEmployee") = Me.IDEmployee
                ViewState("Moves_MovesLastDate") = Me.DateMoves
            End If
            Return tb

        End Get
        Set(ByVal value As DataTable)
            ViewState("Moves_PunchesData") = value
        End Set
    End Property

#End Region

End Class