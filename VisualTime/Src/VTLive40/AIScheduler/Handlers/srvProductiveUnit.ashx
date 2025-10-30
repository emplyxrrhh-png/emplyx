<%@ WebHandler Language="VB" Class="srvProductiveUnit" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports System.Net
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API

Public Class srvProductiveUnit
    Inherits handlerBase

    <Runtime.Serialization.DataContract()>
    Public Class ProductiveUnitSummaryData
        <Runtime.Serialization.DataMember(Name:="modeNames")>
        Public ModeNames As String()

        <Runtime.Serialization.DataMember(Name:="modeValues")>
        Public ModeValues As Double()

        <Runtime.Serialization.DataMember(Name:="modeSummaryName")>
        Public ModeSummaryName As String

    End Class

    Private oPermission As Permission

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)
        Me.scope = "AIScheduler"

        Me.oPermission = Me.GetFeaturePermission("ProductiveUnit.Definition")

        If oPermission > Permission.None Then
            Select Case Request("action")
                Case "getProductiveUnitTab"
                    LoadProductiveUnitDataTab()
                Case "deleteProductiveUnit"
                    DeleteProductiveUnit(Request("ID"))
                Case "getBarButtons"
                    GetBarButtons(Request("ID"))
                Case "GetSummary"
                    CreateSummaryData(roTypes.Any2Integer(Request("ID")), Request("Range"))
                Case "DrawSummary"
                    GenerateChartData(roTypes.Any2Integer(Request("ID")), Request("Range"))
            End Select
        Else
            Dim strResponse As String = "MESSAGE" &
                          "TitleKey=CheckPermission.Denied.Title&" +
                          "DescriptionKey=CheckPermission.Denied.Description&" +
                          "Option1TextKey=CheckPermission.Denied.Option1Text&" +
                          "Option1DescriptionKey=CheckPermission.Denied.Option1Description&" +
                          "Option1OnClickScript=HideMsgBoxForm(); window.location = '" & WLHelperWeb.DefaultRedirectUrl & "' return false;&" +
                          "IconUrl=" & HelperWeb.MsgBoxIconsUrl(HelperWeb.MsgBoxIcons.AlertIcon)
            Dim rError As New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
            Response.Write(rError.toJSON)
        End If

    End Sub

    Private Sub LoadProductiveUnitDataTab()
        Try

            Dim oCurrentProductiveUnit As roProductiveUnit

            If Request("ID") = "-1" Then
                oCurrentProductiveUnit = New roProductiveUnit
            Else
                oCurrentProductiveUnit = AISchedulingServiceMethods.GetProductiveUnitById(Nothing, Request("ID"), False)
            End If

            If oCurrentProductiveUnit Is Nothing Then Exit Sub

            Dim intActiveTab As Integer = roTypes.Any2Integer(Request("aTab"))
            If intActiveTab > 1 Then intActiveTab = 0

            Dim oMainDiv As New HtmlGenericControl("div")

            Dim oImageDiv As New HtmlGenericControl("div")
            oImageDiv.Attributes("class") = "blackRibbonIcon"

            Dim img As HtmlImage = New HtmlImage
            img.Src = "Images/ProductiveUnit80.png"
            oImageDiv.Controls.Add(img)

            Dim oTextDiv As New HtmlGenericControl("div")
            oTextDiv.Attributes("class") = "blackRibbonDescription"
            oTextDiv.InnerHtml = "<div id=""NameReadOnly""><span id=""readOnlyNameProductiveUnit"" class=""NameText"">" & oCurrentProductiveUnit.Name & " </span></div>"

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
            Response.Write("MESSAGE" &
                           "TitleKey=SaveName.Error.Text&" &
                           "DescriptionText=" & ex.ToString & "&" &
                           "Option1TextKey=SaveName.Error.Option1Text&" &
                           "Option1DescriptionKey=SaveName.Error.Option1Description&" &
                           "Option1OnClickScript=HideMsgBoxForm(); return false;&" &
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

        Dim oTabButtons() As HtmlAnchor = {Nothing, Nothing}

        oTabButtons(0) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabGeneral", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(0))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        oTabButtons(1) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabResume", Me.DefaultScope), "bTab")
        hTableCellButtons.Controls.Add(oTabButtons(1))
        hTableRowButtons.Cells.Add(hTableCellButtons)
        hTableButtons.Rows.Add(hTableRowButtons)
        hTableCellButtons = New HtmlTableCell
        hTableRowButtons = New HtmlTableRow

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        For n As Integer = 0 To oTabButtons.Length - 1
            If n = 0 Then
                oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
            Else
                oTabButtons(n).Attributes.Add("OnClick", "javascript: changeTabs(" & n.ToString & ");")
            End If
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
            Dim guiActions As Generic.List(Of roGuiAction) = PortalServiceMethods.GetGuiActions(Nothing, "Portal\AIScheduling\ProductiveUnit\management", WLHelperWeb.CurrentPassportID)
            Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "PrductiveUnit")
            Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
        Catch ex As Exception
            Response.Write(ex.StackTrace.ToString)
        End Try
    End Sub

    Private Sub DeleteProductiveUnit(ByVal oID As Integer)
        Try
            Dim rError As roJSON.JSONError

            'Check Permissions
            If Me.oPermission < Permission.Admin Then
                rError = New roJSON.JSONError(True, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
                Response.Write(rError.toJSON)
                Exit Sub
            End If

            Dim oCurrentProductiveUnit As roProductiveUnit = AISchedulingServiceMethods.GetProductiveUnitById(Nothing, Request("ID"), False)
            If oCurrentProductiveUnit.ID = -1 Then Exit Sub

            If AISchedulingServiceMethods.DeleteProductiveUnit(Nothing, oCurrentProductiveUnit, True) = False Then
                rError = New roJSON.JSONError(True, roWsUserManagement.SessionObject.States.ProductiveUnitState.ErrorText)
            Else
                rError = New roJSON.JSONError(False, "OK")
            End If
            Response.Write(rError.toJSON)

        Catch ex As Exception
            Dim rError As New roJSON.JSONError(True, ex.Message.ToString)
            Response.Write(rError.toJSON)
        End Try
    End Sub

    Private Sub GenerateChartData(idProductiveUnit As Integer, range As String)
        Dim summaryType As New ProductiveUnitSummaryType
        Select Case range.ToString.ToUpper
            Case ProductiveUnitSummaryType.Daily.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Daily
            Case ProductiveUnitSummaryType.Weekly.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Weekly
            Case ProductiveUnitSummaryType.Monthly.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Monthly
            Case ProductiveUnitSummaryType.Anual.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Anual


        End Select

        Dim oSummary = AISchedulingServiceMethods.GetProductiveUnitSummary(Nothing, AISchedulingServiceMethods.GetProductiveUnitById(Nothing, idProductiveUnit, False), summaryType)

        Dim summaryJson As New ProductiveUnitSummaryData

        If (oSummary.ProductiveUnitSummary_ModeDetail IsNot Nothing AndAlso oSummary.ProductiveUnitSummary_ModeDetail.Count > 0) Then
            Dim arrayValue = If(oSummary.ProductiveUnitSummary_ModeDetail.Count > 8, 8, oSummary.ProductiveUnitSummary_ModeDetail.Count - 1)
            Dim taskNames(arrayValue) As String
            Dim taskValues(arrayValue) As Double
            For value As Integer = 0 To arrayValue
                taskNames(value) = oSummary.ProductiveUnitSummary_ModeDetail(value).ModeName.Replace("\", "\\")
                taskValues(value) = oSummary.ProductiveUnitSummary_ModeDetail(value).Quantity
            Next
            summaryJson.ModeNames = taskNames
            summaryJson.ModeValues = taskValues
            summaryJson.ModeSummaryName = Language.Translate("Summary.PUSummaryName", DefaultScope) & " " & taskValues.Sum()
        End If
        Response.Write(roJSONHelper.SerializeNewtonSoft(summaryJson))

    End Sub


    Public Sub CreateSummaryData(idProductiveUnit As Integer, range As String)
        Dim summaryType As New ProductiveUnitSummaryType
        Select Case range.ToString.ToUpper
            Case ProductiveUnitSummaryType.Daily.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Daily
            Case ProductiveUnitSummaryType.Weekly.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Weekly
            Case ProductiveUnitSummaryType.Monthly.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Monthly
            Case ProductiveUnitSummaryType.Anual.ToString.ToUpper
                summaryType = ProductiveUnitSummaryType.Anual


        End Select

        Dim oSummary = AISchedulingServiceMethods.GetProductiveUnitSummary(Nothing, AISchedulingServiceMethods.GetProductiveUnitById(Nothing, idProductiveUnit, False), summaryType)

        If (oSummary.ProductiveUnitSummary_ModeDetail IsNot Nothing AndAlso oSummary.ProductiveUnitSummary_ModeDetail.Count > 0) Then
            Dim divAccualDraw As New HtmlGenericControl("div")
            For Each oModeDetail As roProductiveUnitSummary_ModeDetail In oSummary.ProductiveUnitSummary_ModeDetail.OrderBy(Function(ac) ac.ModeName).ToList()


                Dim divAccrual As New HtmlGenericControl("div")
                divAccrual.Attributes("class") = "accrual"

                Dim divAccrualName As New HtmlGenericControl("div")
                divAccrualName.InnerHtml = WebUtility.HtmlEncode(If(oModeDetail.ModeName.Length > 32, oModeDetail.ModeName.Substring(0, 32).Replace("\", "_").PadRight(33, " ") & "...", oModeDetail.ModeName.Replace("\", "_").PadRight(33, " ")))
                divAccrualName.Attributes("title") = oModeDetail.ModeName
                divAccrual.Controls.Add(divAccrualName)

                Dim divAccrualImage As New HtmlGenericControl("div")
                divAccrualImage.Attributes("class") = "accrualSummaryBox"

                Dim divColorOne As New HtmlGenericControl("div")
                divColorOne.Attributes("class") = "color1"
                divColorOne.Attributes("id") = "divColor1"
                Dim spanTotal As New HtmlGenericControl("span")
                spanTotal.InnerText = oModeDetail.Quantity
                divColorOne.Controls.Add(spanTotal)

                If (Not oModeDetail.Quantity.Equals(0)) Then
                    Dim divColorTwo As New HtmlGenericControl("div")
                    divColorTwo.Attributes("class") = "accrualWarningColor"
                    If (oModeDetail.Quantity < 0) Then
                        divColorTwo.Style("float") = "left"
                    End If
                    Dim colorDivWidht = (100 - CType(((oModeDetail.Quantity * 100) / oModeDetail.Quantity), Integer))
                    divColorTwo.Style("width") = If(colorDivWidht >= 100, "75%", colorDivWidht.ToString & "%")
                    Dim spanTotal2 As New HtmlGenericControl("span")
                    spanTotal2.Style("color") = "black"

                    spanTotal2.InnerText = oModeDetail.Quantity

                    divColorTwo.Controls.Add(spanTotal2)
                    divColorOne.Controls.Add(divColorTwo)
                End If

                divAccrualImage.Controls.Add(divColorOne)
                divAccrual.Controls.Add(divAccrualImage)
                divAccualDraw.Controls.Add(divAccrual)
            Next

            Dim sw As New IO.StringWriter
            Dim htw As New HtmlTextWriter(sw)
            divAccualDraw.RenderControl(htw)

            Response.Write(sw.ToString)
        End If


    End Sub

End Class