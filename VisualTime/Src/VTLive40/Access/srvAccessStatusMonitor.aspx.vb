Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base

Partial Class srvAccessStatusMonitor
    Inherits PageBase

#Region "Events"

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        If Me.HasFeaturePermission("Access.Zones.Supervision", Permission.Read) Then
            Select Case Request("action")
                Case "getAccessStatusMonitor" 'Retorna un Export (Contenidors)
                    LoadAccessStatusData()
            End Select
        Else
            ' Si el passport actual no tiene permisos, devuelve un msgbox y redirecciona a la página principal al aceptar el mensaje.
            Me.Controls.Clear()
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)
        End If
    End Sub

#End Region

#Region "Methods"

    Private Sub LoadAccessStatusData()
        Dim strCols() As String
        Dim sizeCols() As String
        Dim cssCols() As String

        Dim dSetControl As DataSet
        Dim dtblCurrent As DataTable
        Dim dtblIncorrect As DataTable

        Dim ListIdZones As New Generic.List(Of Integer)

        If Request("IdZones") <> "" Then
            Dim arrZones() As String = Request("IdZones").Split(",")
            For Each it As String In arrZones
                If it <> "" Then
                    ListIdZones.Add(it)
                End If
            Next
        End If

        Dim ListFields As New Generic.List(Of String)
        ListFields.Add("Empresa")
        ListFields.Add("Servicio")

        '## dSetControl = API.AccessMoveServiceMethods.GetAccessMovesMonitor(Me, ListIdZones, ListFields, False)
        dSetControl = API.PunchServiceMethods.GetAccessPunchesMonitor(Me, ListIdZones, ListFields, False)

        'Carrega Grid Usuaris Actuals -------------------------------------
        dtblCurrent = dSetControl.Tables(0)
        dtblIncorrect = dSetControl.Tables(1)

        If dtblCurrent.Rows.Count > 0 Then
            strCols = New String() {Me.Language.Translate("CurrentZone.ImageEmployee", Me.DefaultScope), Me.Language.Translate("CurrentZone.ColumnEmployee", Me.DefaultScope),
                                    Me.Language.Translate("CurrentZone.DateArrival", Me.DefaultScope), Me.Language.Translate("CurrentZone.ZoneArrival", Me.DefaultScope)}

            sizeCols = New String() {"115px", "110px", "70px", "120px"}

            cssCols = New String() {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend",
                                    "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader"}

            Dim htmlHGridCurrent As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
            Me.divHeaderEmpInZone.Controls.Add(htmlHGridCurrent)

            Dim htmlTGridCurrent As HtmlTable = creaGridListsIN(dtblCurrent, sizeCols, False)
            Me.divGridEmpInZone.Controls.Add(htmlTGridCurrent)
        Else
            Me.divHeaderEmpInZone.InnerText = Me.Language.Translate("NoEmployeesInZone", Me.DefaultScope) '"No hay empleados actualmente"
        End If

        'Carrega Grid Accesos incorrectes -------------------------------------
        If dtblIncorrect.Rows.Count > 0 Then
            strCols = New String() {Me.Language.Translate("IncAccess.ImageEmployee", DefaultScope), Me.Language.Translate("IncAccess.ColumnEmployee", Me.DefaultScope),
                                    Me.Language.Translate("IncAccess.DateArrival", Me.DefaultScope), Me.Language.Translate("IncAccess.ZoneArrival", Me.DefaultScope),
                                    Me.Language.Translate("IncAccess.InvalidDetail", Me.DefaultScope)}

            sizeCols = New String() {"115px", "100px", "70px", "120px", "100px"}

            cssCols = New String() {"GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader GridStyle-cellheader-noend",
                                    "GridStyle-cellheader GridStyle-cellheader-noend", "GridStyle-cellheader",
                                    "GridStyle-cellheader"}

            Dim htmlHGridOld As HtmlTable = creaHeaderLists(strCols, sizeCols, cssCols)
            Me.divHeaderIncorrectAccess.Controls.Add(htmlHGridOld)

            Dim htmlTGridOld As HtmlTable = creaGridListsInvalid(dtblIncorrect, sizeCols)
            Me.divGridIncorrectAccess.Controls.Add(htmlTGridOld)
        Else
            divHeaderIncorrectAccess.InnerText = Me.Language.Translate("NoIncorrectAccess", Me.DefaultScope) '"No hay marcajes incorrectos en esta zona"
        End If

    End Sub

    Private Function creaHeaderLists(ByVal nomCols() As String, ByVal sizeCols() As String, ByVal cssCols() As String) As HtmlTable
        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("border-bottom") = "0"
            hTable.Style("margin-bottom") = "0"

            hTRow = New HtmlTableRow

            For n As Integer = 0 To nomCols.Length - 1
                hTCell = New HtmlTableCell
                hTCell.Attributes("class") = cssCols(n)
                hTCell.InnerText = nomCols(n)
                If nomCols(n) = "" Then hTCell.InnerText = " "
                hTCell.Width = sizeCols(n)
                hTRow.Cells.Add(hTCell)
            Next

            hTable.Rows.Add(hTRow)

            Return hTable
        Catch ex As Exception
            Return Nothing
        End Try
    End Function

    Private Function creaGridListsInvalid(ByVal dTable As DataTable, ByVal sizeCols() As String) As HtmlTable
        Try
            Dim strAux As String
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("margin-bottom") = "0"

            'registos de accesos de empleados
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                '1 Celda foto
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(0)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerHtml = "<img src=""../Base/Images/userStart.png"" width=""115px"" />"
                Dim foto As String = roCachePhotoManager.GetPhoto(Me.Page, dTable.Rows(n)(0), "Monitor", 115)
                If foto <> String.Empty Then
                    hTCell.InnerHtml = "<img src=""" & Me.ResolveUrl(foto) & """ width=""115px"" />"
                End If
                hTRow.Cells.Add(hTCell)

                '2 Celda NombreEmpleado +  posibles campos de la ficha
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(1)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerText = dTable.Rows(n)(1).ToString
                hTRow.Cells.Add(hTCell)
                'mirar si hay que añadir campos de la ficha
                If dTable.Columns.Count > 7 Then
                    strAux = ""
                    For NumField = 7 To dTable.Columns.Count - 1
                        If roTypes.Any2String(dTable.Rows(n)(NumField)) <> "" Then
                            strAux = strAux & dTable.Rows(n)(NumField).ToString & "/"
                        End If
                    Next
                    If strAux <> "" Then
                        strAux = " (" & strAux.Substring(0, Len(strAux) - 1) & ")"
                    End If
                    hTCell.InnerText = hTCell.InnerText & strAux
                End If
                If hTCell.InnerText = "" Then hTCell.InnerText = " "

                '3 Celda Fecha
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(2)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerText = dTable.Rows(n)(2).ToString
                If hTCell.InnerText = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                '4 Celda NombreZona de entrada
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(3)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerText = dTable.Rows(n)(4).ToString
                If hTCell.InnerText = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)
                hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
                hTRow.Cells.Add(hTCell)

                '5 Celda Motivo de no acceso
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(4)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                '## If dTable.Rows(n)(5) = AccessMoveService.InvalidTypes.NFLD_ Then
                If dTable.Rows(n)(5) = InvalidTypeEnum.NFLD_ Then
                    hTCell.InnerText = dTable.Rows(n)(6).ToString 'El motivo se encuentra en el campo Detail
                Else
                    hTCell.InnerText = Me.Language.Translate("IncorrectAccess.Type." & dTable.Rows(n)(5).ToString, Me.DefaultScope)
                End If
                If hTCell.InnerText = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)
                hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
                hTRow.Cells.Add(hTCell)

                hTable.Rows.Add(hTRow)

            Next

            Return hTable
        Catch ex As Exception
            Dim htmlTableErr As New HtmlTable
            Dim htmlTableErrCell As New HtmlTableCell
            Dim htmlTableErrRow As New HtmlTableRow
            htmlTableErrCell.InnerText = ex.Message.ToString & " " & ex.StackTrace.ToString
            htmlTableErrRow.Cells.Add(htmlTableErrCell)
            htmlTableErr.Rows.Add(htmlTableErrRow)
            Return htmlTableErr
        End Try

    End Function

    Private Function creaGridListsIN(ByVal dTable As DataTable, ByVal sizeCols() As String, ByVal bColImage As Boolean) As HtmlTable
        Try
            Dim strAux As String
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridGrupos"
            hTable.Style("margin-bottom") = "0"

            'registos de accesos de empleados
            For n As Integer = 0 To dTable.Rows.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                '1 Celda foto
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(0)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerHtml = "<img src=""../Base/Images/userStart.png"" width=""115px"" />"
                Dim foto As String = roCachePhotoManager.GetPhoto(Me.Page, dTable.Rows(n)(0), "Monitor", 115)
                If foto <> String.Empty Then
                    hTCell.InnerHtml = "<img src=""" & Me.ResolveUrl(foto) & """ width=""115px"" />"
                End If
                hTRow.Cells.Add(hTCell)

                '2 Celda NombreEmpleado +  posibles campos de la ficha
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(1)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerText = dTable.Rows(n)(1).ToString
                hTRow.Cells.Add(hTCell)
                'mirar si hay que añadir campos de la ficha
                If bColImage Then
                    If dTable.Columns.Count > 6 Then
                        strAux = ""
                        For NumField = 6 To dTable.Columns.Count - 1
                            If roTypes.Any2String(dTable.Rows(n)(NumField)) <> "" Then
                                strAux = strAux & dTable.Rows(n)(NumField).ToString & "/"
                            End If
                        Next
                        If strAux <> "" Then
                            strAux = " (" & strAux.Substring(0, Len(strAux) - 1) & ")"
                        End If
                        hTCell.InnerText = hTCell.InnerText & strAux
                    End If
                Else
                    If dTable.Columns.Count > 5 Then
                        strAux = ""
                        For NumField = 5 To dTable.Columns.Count - 1
                            If roTypes.Any2String(dTable.Rows(n)(NumField)) <> "" Then
                                strAux = strAux & dTable.Rows(n)(NumField).ToString & "/"
                            End If
                        Next
                        If strAux <> "" Then
                            strAux = " (" & strAux.Substring(0, Len(strAux) - 1) & ")"
                        End If
                        hTCell.InnerText = hTCell.InnerText & strAux
                    End If
                End If
                If hTCell.InnerText = "" Then hTCell.InnerText = " "

                '3 Celda Fecha
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(2)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerText = dTable.Rows(n)(2).ToString
                If hTCell.InnerText = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)

                '4 Celda NombreZona de entrada
                hTCell = New HtmlTableCell
                hTCell.Width = sizeCols(3)
                hTCell.Attributes("class") = "GridStyle-cell" & altRow
                hTCell.Style("display") = "table-cell"
                hTCell.InnerText = dTable.Rows(n)(4).ToString
                If hTCell.InnerText = "" Then hTCell.InnerText = " "
                hTRow.Cells.Add(hTCell)
                hTCell.Attributes("class") = hTCell.Attributes("class") & "  GridStyle-endcell" & altRow
                hTRow.Cells.Add(hTCell)

                hTable.Rows.Add(hTRow)

            Next

            Return hTable
        Catch ex As Exception
            Dim htmlTableErr As New HtmlTable
            Dim htmlTableErrCell As New HtmlTableCell
            Dim htmlTableErrRow As New HtmlTableRow
            htmlTableErrCell.InnerText = ex.Message.ToString & " " & ex.StackTrace.ToString
            htmlTableErrRow.Cells.Add(htmlTableErrCell)
            htmlTableErr.Rows.Add(htmlTableErrRow)
            Return htmlTableErr
        End Try

    End Function

#End Region

End Class