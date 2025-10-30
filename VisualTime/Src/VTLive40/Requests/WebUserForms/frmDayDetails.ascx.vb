Imports Robotics.Base.DTOs
Imports Robotics.Base.VTRequests.Requests
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

Partial Class frmDayDetails
    Inherits UserControlBase

    Public Sub createRequestDaysResume(ByVal oReqType As eRequestType, ByVal oReqDays As List(Of roRequestDay))

        Try
            Dim hTable As New HtmlTable
            Dim hTRow As New HtmlTableRow
            Dim hTCell As HtmlTableCell
            Dim altRow As String = "2"
            Dim noRegs As Boolean = False
            Dim bIsWin32 As Boolean = False

            Dim strClickEdit As String = ""         'Href onclick Mode edicio
            Dim strClickRemove As String = ""       'Href onclick Mode eliminacio

            hTable.Border = 0
            hTable.CellPadding = 0
            hTable.CellSpacing = 0

            hTable.Attributes("class") = "GridStyle GridEmpleados"

            'TODO: Creo la capcelera (de moment manual... aixo es pot fer GENERIC)
            hTRow = New HtmlTableRow

            hTCell = New HtmlTableCell
            hTCell.Attributes("class") = "GridStyle-cellheader"

            Select Case oReqType
                Case eRequestType.CancelHolidays
                    hTCell.InnerHtml = Me.Language.Translate("tabDayDetail.CancelHolidays", Me.DefaultScope)
                Case eRequestType.PlannedHolidays
                    hTCell.InnerHtml = Me.Language.Translate("tabDayDetail.PlannedHoliday", Me.DefaultScope)
                Case eRequestType.VacationsOrPermissions
                    hTCell.InnerHtml = Me.Language.Translate("tabDayDetail.VacationsOrPermissions", Me.DefaultScope)
                Case eRequestType.ExternalWorkWeekResume
                    hTCell.InnerHtml = Me.Language.Translate("tabDayDetail.ExternalWorkWeekResume", Me.DefaultScope)
            End Select

            hTCell.ColSpan = 2
            hTRow.Cells.Add(hTCell)

            hTable.Rows.Add(hTRow)

            'Bucle als registres
            For n As Integer = 0 To oReqDays.Count - 1
                hTRow = New HtmlTableRow
                altRow = IIf(altRow = "1", "2", "1")

                Dim oActualReqDay As roRequestDay = oReqDays(n)

                hTCell = New HtmlTableCell

                'Cambia el alternateRow
                hTCell.Attributes("class") = "GridStyle-cell" & altRow & " GridStyle-endcell" & altRow

                Dim oParams As New Generic.List(Of String)

                If oReqType = eRequestType.ExternalWorkWeekResume Then
                    oParams.Add(If(oActualReqDay.ActualType.Value = PunchTypeEnum._IN, Me.Language.Translate("DayResume.Input", Me.DefaultScope, oParams), Me.Language.Translate("DayResume.Output", Me.DefaultScope, oParams)))
                    oParams.Add(Format(oActualReqDay.RequestDate, HelperWeb.GetShortDateFormat))
                    oParams.Add(Format(oActualReqDay.RequestDate, HelperWeb.GetShortTimeFormat))

                    oParams.Add(If(oActualReqDay.IDCause.HasValue, CausesServiceMethods.GetCauseByID(Me.Page, oActualReqDay.IDCause, False).Name, ""))

                    oParams.Add(oActualReqDay.Comments)
                Else
                    oParams.Add(oActualReqDay.RequestDate.ToShortDateString())

                    If oActualReqDay.Duration.HasValue Then
                        oParams.Add(CDate(Robotics.VTBase.roTypes.Any2Time(oActualReqDay.Duration.Value).Value).ToShortTimeString)
                    Else
                        oParams.Add(CDate(Robotics.VTBase.roTypes.Any2Time(0).Value).ToShortTimeString)
                    End If

                    If oActualReqDay.FromTime.HasValue Then
                        oParams.Add(oActualReqDay.FromTime.Value.ToShortTimeString())
                    Else
                        oParams.Add(Date.Now.Date.ToShortDateString())
                    End If

                    If oActualReqDay.ToTime.HasValue Then
                        oParams.Add(oActualReqDay.ToTime.Value.ToShortTimeString())
                    Else
                        oParams.Add(Date.Now.Date.ToShortDateString())
                    End If
                End If

                Select Case oReqType
                    Case eRequestType.CancelHolidays
                        hTCell.InnerText = Me.Language.Translate("DayResume.CancelHolidays", Me.DefaultScope, oParams)
                    Case eRequestType.PlannedHolidays
                        If oActualReqDay.AllDay.HasValue AndAlso oActualReqDay.AllDay.Value = True Then
                            hTCell.InnerText = Me.Language.Translate("DayResume.AllDayDetail", Me.DefaultScope, oParams)
                        Else
                            hTCell.InnerText = Me.Language.Translate("DayResume.PeriodDetail", Me.DefaultScope, oParams)
                        End If
                    Case eRequestType.VacationsOrPermissions
                        hTCell.InnerText = Me.Language.Translate("DayResume.VacationsOrPermissions", Me.DefaultScope, oParams)
                    Case eRequestType.ExternalWorkWeekResume
                        hTCell.InnerText = Me.Language.Translate("DayResume.ExternalWorkWeekResume", Me.DefaultScope, oParams)
                End Select

                If hTCell.InnerText = "" Then hTCell.InnerHtml = "&nbsp;"

                'Carrega la celda al row
                hTRow.Cells.Add(hTCell)

                hTable.Rows.Add(hTRow)
            Next

            Me.grdDayDetailsResume.Controls.Add(hTable)
        Catch ex As Exception

        End Try

    End Sub

End Class