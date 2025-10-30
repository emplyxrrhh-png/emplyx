Imports Robotics.Base.DTOs
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class Requests_RequestApprovals
    Inherits PageBase

    Protected Sub Page_Init1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Init
        'Scripts del robotics
        Me.InsertExtraJavascript("BrowserDetect", "~/Base/Scripts/BrowserDetect.js", , True)
        Me.InsertExtraJavascript("Cookies", "~/Base/Scripts/Cookies.js", , True)
        Me.InsertExtraJavascript("Generic", "~/Base/Scripts/Generic.js", , True)

        Me.InsertExtraJavascript("jsDate", "~/Base/Scripts/jsDate.js")

        Me.InsertExtraJavascript("jsGrid", "~/Base/Scripts/jsGrid.js")
        Me.InsertExtraJavascript("jsDatePicker", "~/Base/Scripts/jsDatePicker.js")

    End Sub

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.InsertCssIncludes(Me.Page)

        Me.Img1.Src = Me.ResolveUrl("~/Requests/Images/ApprovalsHistory.png")

        'ISM: Quitamos los permisos ya que aquí solo podemos llegar para solicitudes en las que ya tenemos permisos.
        'If Me.HasFeaturePermission("Employees.UserFields.Requests", Permission.Read) And _
        '   Me.HasFeaturePermission("Calendar.Requests", Permission.Read) And _
        '   Me.HasFeaturePermission("Calendar.Punches", Permission.Read) Then
        'Else
        '    WLHelperWeb.RedirectAccessDenied(True)
        '    Exit Sub
        'End If

        Dim oRequest As roRequest = API.RequestServiceMethods.GetRequestByID(Me, roTypes.Any2Integer(Request("IDRequest")), False)
        If oRequest IsNot Nothing Then

            Dim tbApprovals As New DataTable
            tbApprovals.Columns.Add(New DataColumn("Passport", GetType(String)))
            tbApprovals.Columns.Add(New DataColumn("DateTime", GetType(DateTime)))
            tbApprovals.Columns.Add(New DataColumn("Status", GetType(Integer)))
            tbApprovals.Columns.Add(New DataColumn("StatusLevel", GetType(Integer)))
            tbApprovals.Columns.Add(New DataColumn("Comments", GetType(String)))

            If oRequest.RequestApprovals IsNot Nothing Then
                Dim oRow As DataRow = Nothing
                Dim oPassportTicket As roPassportTicket = Nothing
                For Each oRequestApproval As roRequestApproval In oRequest.RequestApprovals
                    oRow = tbApprovals.NewRow
                    oPassportTicket = API.SecurityServiceMethods.GetPassportTicket(Me, oRequestApproval.IDPassport)
                    If Not oPassportTicket Is Nothing Then
                        oRow("Passport") = oPassportTicket.Name
                    Else
                        oRow("Passport") = " -Desconocido- "
                    End If
                    oRow("DateTime") = oRequestApproval.ApprovalDateTime
                    oRow("Status") = oRequestApproval.Status
                    oRow("StatusLevel") = oRequestApproval.StatusLevel
                    oRow("Comments") = oRequestApproval.Comments
                    tbApprovals.Rows.Add(oRow)
                Next
            End If

            Me.divRequestApprovals.Controls.Add(Me.creaGridRequestApprovals(tbApprovals))

        End If

    End Sub

    Private Function creaGridRequestApprovals(ByVal dTable As DataTable) As HtmlTable
        ''Try
        Dim hTable As New HtmlTable
        Dim hTRow As New HtmlTableRow
        Dim hTCell As HtmlTableCell
        Dim altRow As String = "2"
        hTable.Border = 0
        hTable.CellPadding = 0
        hTable.CellSpacing = 0

        hTable.Attributes("class") = "GridStyle GridEmpleados"

        'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
        hTRow = New HtmlTableRow

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        'hTCell.Width = "250"
        hTCell.Attributes("style") = "border-right: 0px; "
        hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Passport", Me.DefaultScope)
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.Attributes("style") = "border-right: 0px; "
        hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.DateTime", Me.DefaultScope)
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.Attributes("style") = "border-right: 0px; "
        hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Status", Me.DefaultScope)
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.Attributes("style") = "border-right: 0px; "
        hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.StatusLevel", Me.DefaultScope)
        hTRow.Cells.Add(hTCell)

        hTCell = New HtmlTableCell
        hTCell.Attributes("class") = "GridStyle-cellheader"
        hTCell.InnerHtml = Me.Language.Translate("RequestApprovalsGrid.Columns.Comments", Me.DefaultScope)
        hTRow.Cells.Add(hTCell)

        hTable.Rows.Add(hTRow)

        Dim Rows() As DataRow

        Dim intCountHigh As Integer = 0

        Rows = dTable.Select("", "DateTime")

        If Rows.Length > 0 Then

            ' Obtenemos información del estado de la solicitud
            Dim tbRequestStates As DataTable = API.RequestServiceMethods.GetRequestStates(Me)

            ' Bucle por los campos de la categoría actual
            For Each oRow As DataRow In Rows

                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                ' Pinta columna nombre passport
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.Attributes("style") = "border-right: 0px; "
                hTCell.InnerText = oRow("Passport")
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                ' Pinta columna fecha/hora
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.Attributes("style") = "border-right: 0px; "
                hTCell.InnerText = Format(CDate(oRow("DateTime")), HelperWeb.GetShortDateFormat) & " " & Format(CDate(oRow("DateTime")), HelperWeb.GetShortTimeFormat & ":ss")
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                ' Pinta columna estado
                Dim oStateRow As DataRow = Nothing
                Dim oRows() As DataRow = tbRequestStates.Select("ElementID=" & oRow("Status"))
                If oRows.Length > 0 Then oStateRow = oRows(0)

                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.Attributes("style") = "border-right: 0; "
                hTCell.InnerText = oStateRow("ElementDesc")
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                ' Pinta columna nivel de aprobación
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.InnerText = oRow("StatusLevel")
                hTCell.Attributes("style") = "border-right: 0; "
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                ' Pinta columna comentarios
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Attributes.Add("nowrap", "nowrap")
                hTCell.Style("padding") = "3px"
                hTCell.InnerHtml = roTypes.Any2String(oRow("Comments")).Replace(vbLf, "</br>")
                If hTCell.InnerHtml = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                ' Cerramos la fila
                ''hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
                ''hTCell.ColSpan = 1
                ''hTRow.Cells.Add(hTCell)

                hTable.Rows.Add(hTRow)

            Next

        End If

        Return hTable

    End Function

End Class