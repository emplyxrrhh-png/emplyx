Imports Robotics.Base.VTBusiness.Cause
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class ReorderMoves
    Inherits PageBase

#Region "Declarations"

    Private _Action_Edit As Integer = 0
    Private _Action_Remove As Integer = 1
    Private _Action_Accept As Integer = 2
    Private _Action_Cancel As Integer = 3

    Private _Moves_EditClickIndex As Integer = 0
    Private _Moves_RemoveClickIndex As Integer = 1
    Private _Moves_selectCellIndex As Integer = 2
    Private _Moves_ActionButtons() As String = {"imgEdit", "imgRemove", "imgEditAccept", "imgEditCancel"}
    Private _Moves_EditCellsIndex() As Integer = {3, 4, 5, 6}
    Private _Moves_EditControls() As String = {"InDateTime_TextBox", "InIDCause_DropDownList", "OutDateTime_TextBox", "OutIDCause_DropDownList"}
    Private _Moves_CaptionControls() As String = {"InDateTime_Label", "InCause_TextBox", "OutDateTime_Label", "OutCause_TextBox"}
    Private _Moves_EditFields() As String = {"InDateTime", "InIDCause", "OutDateTime", "OutIDCause"}

    Private _Incidences_EditClickIndex As Integer = 0
    Private _Incidences_RemoveClickIndex As Integer = 1
    Private _Incidences_selectCellIndex As Integer = 2
    Private _Incidences_ActionButtons() As String = {"imgEditIncidences", "imgRemoveIncidences", "imgEditAcceptIncidences", "imgEditCancelIncidences"}
    Private _Incidences_EditCellsIndex() As Integer = {8, 9}
    Private _Incidences_EditControls() As String = {"IDCause_DropDownList", "Value_TextBox"}
    Private _Incidences_CaptionControls() As String = {"Cause_TextBox", "Value_Label"}
    Private _Incidences_EditFields() As String = {"IDCause", "Value"}

    'Private intIDGroup As Integer
    Private intIDEmployee As Integer

    Private xDate As Date

    Private bolAsyncCall As Boolean ' Indica si la página se ha llamado a través de 'GetCallbackEventReference' desde el cliente.

    Private intSelectorType As Integer

    Private xFreezingDate As Date

#End Region

#Region "Properties"

    Private Property IDEmployee() As Integer
        Get
            Return ViewState("Moves_IDEmployee")
        End Get
        Set(ByVal value As Integer)

            If value <> ViewState("Moves_IDEmployee") Then

            End If

            ViewState("Moves_IDEmployee") = value
            Me.hdnEmployeeID.Value = value
            If value > 0 Then
                Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Me, value, False)
                If oEmployee IsNot Nothing Then
                End If
            End If

        End Set
    End Property

    Private Property DateMoves() As Date
        Get
            Return CDate(ViewState("Moves_Date"))
        End Get
        Set(ByVal value As Date)

            ''If Me.oContext.BeginDate.Date > value Or Me.oContext.EndDate.Date < value.Date Then
            ''    value = Me.oContext.BeginDate.Date
            ''ElseIf Me.oContext.EndDate.Date < value.Date Then
            ''    value = Me.oContext.EndDate.Date
            ''End If

            If value <> CDate(ViewState("Moves_Date")) Then
            End If

            ViewState("Moves_Date") = value
            Me.hdnDate.Value = value 'Format(value, "dd/MM/yyyy")

        End Set
    End Property

#End Region

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        Me.InsertExtraJavascript("Ajax", "~/Base/Scripts/Ajax.js")
    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        ' Obtenemos la fecha de congelación actual
        Me.xFreezingDate = API.ConnectorServiceMethods.GetFirstDate(Me)

        If Not Me.IsPostBack Then
            Dim EmployeeID As String = Request.Params("ID")
            If EmployeeID IsNot Nothing AndAlso EmployeeID.Length > 0 Then
                Me.IDEmployee = CInt(EmployeeID)
            End If

            Dim strDate As String = Request.Params("DateMoves") ' En formato 'dd/MM/yyyy'
            If strDate IsNot Nothing AndAlso strDate.Length > 0 AndAlso IsDate(strDate) Then
                'Me.DateMoves = New Date(strDate.Substring(6, 4), strDate.Substring(3, 2), strDate.Substring(0, 2))
                Me.DateMoves = CDate(strDate)
            End If

            Me.LoadMoves(True)

        End If

    End Sub

    Private Function MovesData(Optional ByVal bolReload As Boolean = False) As DataTable
        Dim tb As DataTable = New DataTable

        'tb = MoveService.MoveServiceMethods.GetMoves(Me, Me.IDEmployee, Me.DateMoves)
        'tb = MoveService.MoveServiceMethods.ReorderMovesPreview(Me, Me.IDEmployee, Me.DateMoves)
        tb = Nothing
        If tb IsNot Nothing Then
            If tb.Rows.Count = 0 Then
                tb.Columns.Add("InIDCauseName", GetType(String))
                tb.Columns.Add("OutIDCauseName", GetType(String))
                tb.Rows.Add(tb.NewRow)
            Else
                tb.Columns.Add("InIDCauseName", GetType(String))
                tb.Columns.Add("OutIDCauseName", GetType(String))
                For Each dRow As DataRow In tb.Rows
                    Dim oCause As roCause
                    If Not dRow("InIDCause") Is DBNull.Value Then
                        oCause = CausesServiceMethods.GetCauseByID(Me.Page, dRow("InIDCause"), False)
                        If oCause IsNot Nothing Then dRow("InIDCauseName") = oCause.Name
                    End If
                    If Not dRow("OutIDCause") Is DBNull.Value Then
                        oCause = CausesServiceMethods.GetCauseByID(Me.Page, dRow("OutIDCause"), False)
                        If oCause IsNot Nothing Then dRow("OutIDCauseName") = oCause.Name
                    End If
                Next
            End If
        End If

        Return tb

    End Function

    Private Sub LoadMoves(Optional ByVal bolReload As Boolean = False)
        With Me.grdMoves
            .DataSourceID = ""
            .DataSource = Me.MovesData(bolReload)
            .DataBind()
        End With
        HelperWeb.EmptyGridFix(Me.grdMoves)
    End Sub

#Region "grdMoves"

    Private Enum MOVES_COL
        IN_TIME = 3
        IN_CAUSE = 4
        OUT_TIME = 5
        OUT_CAUSE = 6
        SHIFTDATE = 7
    End Enum

    Protected Sub grdMoves_RowCreated(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdMoves.RowCreated

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                If e.Row.RowIndex = _gridView.SelectedIndex Then
                    Dim oSelect As Image = e.Row.Cells(Me._Moves_selectCellIndex).FindControl("Select_Image")
                    If oSelect IsNot Nothing Then oSelect.ImageUrl = "~/Base/Images/Grid/Select.gif"
                End If

            Case DataControlRowType.Footer

        End Select

    End Sub

    Protected Sub grdMoves_RowDataBound(ByVal sender As Object, ByVal e As System.Web.UI.WebControls.GridViewRowEventArgs) Handles grdMoves.RowDataBound

        Dim _gridView As GridView = sender

        Select Case e.Row.RowType
            Case DataControlRowType.Header

            Case DataControlRowType.DataRow

                Dim oMoveData As DataRowView = e.Row.DataItem ' Me.MovesData.Rows(e.Row.RowIndex)

                'e.Row.Visible = (oMoveData.Row.RowState <> DataRowState.Deleted)
                'If e.Row.Visible Then

                If oMoveData.Row.RowState = DataRowState.Added And e.Row.RowIndex = Me.MovesData.Rows.Count - 1 Then
                    Dim oSelect As Image = e.Row.Cells(Me._Moves_selectCellIndex).FindControl("Select_Image")
                    If oSelect IsNot Nothing Then oSelect.ImageUrl = "~/Base/Images/Grid/New.gif"
                End If

                Dim lblInDateTime As Label = e.Row.Cells(1).FindControl("InDateTime_Label")
                Dim lblInIDCause As Label = e.Row.Cells(2).FindControl("InIDCause_Label")
                Dim lblInIDCauseName As Label = e.Row.Cells(2).FindControl("InIDCause_LabelName")

                Dim lblOutDateTime As Label = e.Row.Cells(3).FindControl("OutDateTime_Label")
                Dim lblOutIDCause As Label = e.Row.Cells(4).FindControl("OutIDCause_Label")
                Dim lblOutIDCauseName As Label = e.Row.Cells(4).FindControl("OutIDCause_LabelName")

                Dim oColor As Drawing.Color = Drawing.Color.Empty

                ' IN_TIME
                If Not IsDBNull(oMoveData("InDateTime")) Then
                    lblInDateTime.Text = Format(oMoveData("InDateTime"), "HH:mm")
                End If

                ' OUT_TIME
                If Not IsDBNull(oMoveData("OutDateTime")) Then
                    lblOutDateTime.Text = Format(oMoveData("OutDateTime"), "HH:mm")
                End If

                ' OUT_CAUSE

                Dim bolEdit As Boolean = False
                Dim bolContextMenu As Boolean = False

            Case DataControlRowType.Footer

        End Select

    End Sub

#End Region

    Protected Sub btAccept_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles btAccept.Click
        'Robotics.Web.Base.MoveService.MoveServiceMethods.ReorderMoves(Me, Me.IDEmployee, DateMoves)
        Me.MustRefresh = "1"
        Me.CanClose = True
    End Sub

End Class