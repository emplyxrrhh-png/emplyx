<%@ WebHandler Language="VB" Class="srvAccessPeriods" %>

Imports System
Imports System.Web
Imports Robotics.Web.Base
Imports Robotics.VTBase
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTBusiness.AccessPeriod

Public Class srvAccessPeriods
    Inherits handlerBase

    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)

        Me.oPermission = Me.GetFeaturePermission("Access.Periods.Definition")

        If oPermission > Permission.None Then
            Select Case Request("action")
                Case "getAccessPeriodTab"
                    LoadAccessPeriodsDataTab()
                Case "deleteAccessPeriod" ' Elimina un AccessPeriod (JSON)
                    DeleteAccessPeriod(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "getAccessPeriodDescription" ' Recupera la descripcio
                    GetAccessPeriodDescription()
            End Select

        Else

            Dim strResponse As String = "MESSAGE" & _
                          "TitleKey=CheckPermission.Denied.Title&" + _
                          "DescriptionKey=CheckPermission.Denied.Description&" + _
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" + _
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" + _
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" + _
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)

        End If

    End Sub

    Private Sub LoadAccessPeriodsDataTab()
        Try

            Dim oCurrentPeriod As roAccessPeriod

            If Request("ID") = "-1" Then
                oCurrentPeriod = New roAccessPeriod
            Else
                oCurrentPeriod = AccessPeriodServiceMethods.GetAccessPeriodByID(Nothing, Request("ID"))
            End If

            If oCurrentPeriod Is Nothing Then Exit Sub

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))
            If intActiveTab > 1 Then intActiveTab = 0


            Dim oMainDiv As New HtmlGenericControl("div")
            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/AccessPeriods80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameAccessPeriod"" class=""NameText"">" & oCurrentPeriod.Name & " </span></div>"

            Dim oButtonsDiv As New HtmlGenericControl("div")
            oButtonsDiv.Attributes("class") = "blackRibbonButtons"
            oButtonsDiv.Controls.Add(Me.CreateTabs(intActiveTab))

            oMainDiv.Controls.Add(oImageDiv)
            oMainDiv.Controls.Add(oTextDiv)
            oMainDiv.Controls.Add(oButtonsDiv)

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            oMainDiv.RenderControl(htw)

            Response.Write(sw.ToString)

        Catch ex As Exception
            Response.Write("MESSAGE" & _
                           "TitleKey=SaveName.Error.Text&" & _
                           "DescriptionText=" & ex.ToString & "&" & _
                           "Option1TextKey=SaveName.Error.Option1Text&" & _
                           "Option1DescriptionKey=SaveName.Error.Option1Description&" & _
                           "Option1OnClickScript=HideMsgBoxForm(); return false;&" & _
                           "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.ErrorIcon))
        End Try
    End Sub

    Private Function CreateTabs(ByRef intActiveTab As Integer) As HtmlTable
        Dim hTableGen As New HtmlTable
        Dim hRowGen As New HtmlTableRow
        Dim hCellGen As New HtmlTableCell

        Dim hTableButtons As New HtmlTable
        Dim hTableRowButtons As New HtmlTableRow
        Dim hTableCellButtons As New HtmlTableCell

        hTableGen.Border = 0
        hTableGen.CellSpacing = 0
        hTableGen.CellPadding = 0

        hTableButtons.Border = 0
        hTableButtons.CellSpacing = 0
        hTableButtons.CellPadding = 0

        Dim oTabButtons() As HtmlAnchor = {Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
        Next

        oTabButtons(intActiveTab).Attributes("class") = "bTab-active"

        Return hTableGen ' Retorna el HTMLTable

    End Function

    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerText = Text
        Return obutton
    End Function

    Private Sub GetBarButtons(ByVal sID As String)
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\AccessManagement\AccessPeriods\management", WLHelperWeb.CurrentPassportID)

            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "AccessPeriods")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub DeleteAccessPeriod(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            If oID = -1 Then Exit Sub

            If AccessPeriodServiceMethods.DeleteAccessPeriod(Nothing, oID, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.AccessPeriodState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)
        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub GetAccessPeriodDescription()
        Try
            Dim IDAccessPeriod As Integer = Request("IDAccessPeriod")
            Dim Description As String = ""
            Dim DayofWeek As String = Request("DayofWeek")
            Dim BeginTime As String = Request("BeginTime")
            Dim EndTime As String = Request("EndTime")
            Dim Day As String = Request("Day")
            Dim Month As String = Request("Month")

            If DayofWeek <> "" Then DayofWeek = CInt(DayofWeek)

            Dim oAccessPeriodDaily As New roAccessPeriodDaily()
            Dim oAccessPeriodHolidays As New roAccessPeriodHolidays()

            If Day = "" And Month = "" Then 'Es normal
                oAccessPeriodDaily.IDAccessPeriod = IDAccessPeriod
                oAccessPeriodDaily.DayofWeek = DayofWeek
                oAccessPeriodDaily.BeginTime = New Date(1899, 12, 30, BeginTime.Split(":")(0), BeginTime.Split(":")(1), 0)
                oAccessPeriodDaily.EndTime = New Date(1899, 12, 30, EndTime.Split(":")(0), EndTime.Split(":")(1), 0)
                Description = AccessPeriodServiceMethods.GetAccessPeriodDailyDescription(Nothing, oAccessPeriodDaily)
            Else ' Es especifico (festivo, etc.)
                oAccessPeriodHolidays.IDAccessPeriod = IDAccessPeriod
                oAccessPeriodHolidays.Day = CInt(Day)
                oAccessPeriodHolidays.Month = CInt(Month)
                If BeginTime <> "" Then
                    oAccessPeriodHolidays.BeginTime = New Date(1899, 12, 30, BeginTime.Split(":")(0), BeginTime.Split(":")(1), 0)
                Else
                    oAccessPeriodHolidays.BeginTime = New Date(1899, 12, 30, 0, 0, 0)
                End If

                If EndTime <> "" Then
                    oAccessPeriodHolidays.EndTime = New Date(1899, 12, 30, EndTime.Split(":")(0), EndTime.Split(":")(1), 0)
                Else
                    oAccessPeriodHolidays.EndTime = New Date(1899, 12, 30, 0, 0, 0)
                End If

                Description = AccessPeriodServiceMethods.GetAccessPeriodHolidaysDescription(Nothing, oAccessPeriodHolidays)
            End If

            Dim oJF As New JSONFieldItem("DESCRIPTION", Description, New String() {}, JSONFieldItem.JSONType.None_JSON, Nothing, False)
            Response.Write(roJSONHelper.Serialize(oJF))

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString & " " & ex.StackTrace)
            Response.Write(rError.toJSON)
        End Try
    End Sub


End Class