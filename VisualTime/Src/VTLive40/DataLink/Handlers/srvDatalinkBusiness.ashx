<%@ WebHandler Language="VB" Class="srvDatalinkBusiness" %>

Imports System
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports Robotics.Base.DTOs
Imports Robotics.Web.Base.API
Imports Robotics.Base.VTDataLink.DataLink

Imports System.Data

Public Class srvDatalinkBusiness
    Inherits handlerBase

    Private oCurrentExport As roGuide
    Private wSepChar As String = Chr(94) & Chr(124) & Chr(94)

    Public Overrides Sub ProcessRoboticsRequest(ByVal context As HttpContext)

        Me.scope = "DataLink"

        If Me.HasFeaturePermission("Employees.DataLink", Permission.Read) OrElse
           Me.HasFeaturePermission("Calendar.DataLink", Permission.Read) OrElse
           Me.HasFeaturePermission("Tasks.DataLink", Permission.Read) OrElse
           Me.HasFeaturePermission("BusinessCenters.DataLink", Permission.Read) Then

            Select Case Request("action")
                Case "getDatalinkTab" ' Retorna la capcelera de la plana
                    LoadDatalinkTab()
                Case "uploadImportFile"
                    UploadFileForImport()
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

    Private Sub UploadFileForImport()

        Dim rError As New roJSON.JSONError(False, Me.Language.Translate("CheckPermission.Denied.Description", DefaultScope))
        Dim oFile As HttpPostedFile = Nothing

        If Request.Files("importFile") IsNot Nothing Then oFile = Request.Files("importFile")

        If oFile IsNot Nothing Then
            Dim file As IO.Stream = oFile.InputStream()
            Dim fileData As Byte() = New Byte(file.Length - 1) {}
            file.Read(fileData, 0, file.Length)

            Session("ImportFileOrig") = fileData
        Else
            rError.Error = True
        End If
        Response.Write(roJSONHelper.SerializeNewtonSoft(rError))
    End Sub

    Private Sub LoadDatalinkTab()
        Try

            Dim oGuide As roDatalinkGuide = Nothing
            Dim strConcept As String = roTypes.Any2String(Request("Concept"))
            If strConcept <> String.Empty Then
                Dim eConcept As roDatalinkConcept = DirectCast([Enum].Parse(GetType(roDatalinkConcept), strConcept), roDatalinkConcept)
                oGuide = API.DataLinkGuideServiceMethods.GetDatalinkGuide(Nothing, eConcept, False)
            End If

            If oGuide IsNot Nothing Then
                Dim strActiveTab As String = roTypes.Any2String(Request("aTab"))

                Dim oMainDiv As New HtmlGenericControl("div")

                Dim oImageDiv As New HtmlGenericControl("div")
                oImageDiv.Attributes("class") = "blackRibbonIcon"

                Dim img As HtmlImage = New HtmlImage
                img.Src = "Images/importExport80.png"
                oImageDiv.Controls.Add(img)

                Dim oTextDiv As New HtmlGenericControl("div")
                oTextDiv.Attributes("class") = "blackRibbonDescription"
                oTextDiv.InnerHtml = "<div id=""NameReadOnly""><a href=""javascript: void(0);"" onclick="""" style=""display: block;cursor: text;"" title=""""><span id=""readOnlyNameImport"" class=""NameText"">" & oGuide.Name & "</span></a></div>" &
                                    "<div id=""NameChange"" style=""display: none;""><table><tr><td><input type=""text"" id=""txtNameImport"" style=""width: 350px;"" value="""" class=""inputNameText"" ConvertControl=""TextField"" CCallowBlank=""false"" onblur=""EditNameImport('false');"" ></td>" &
                                    "</tr></table>" &
                                    "</div>"

                Dim oButtonsDiv As New HtmlGenericControl("div")
                oButtonsDiv.Attributes("class") = "blackRibbonButtons"
                oButtonsDiv.Controls.Add(Me.CreateTabs(oGuide, strActiveTab))

                oMainDiv.Controls.Add(oImageDiv)
                oMainDiv.Controls.Add(oTextDiv)
                oMainDiv.Controls.Add(oButtonsDiv)

                Dim sw As New IO.StringWriter
                Dim htw As New HtmlTextWriter(sw)
                oMainDiv.RenderControl(htw)

                Response.Write(sw.ToString)

            End If

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

    Private Function CreateTabs(ByVal oGuide As roDatalinkGuide, ByRef intActiveTab As String) As HtmlTable
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

        Dim iNextTabIndex As Integer = 0
        Dim iSelectedTabIndex As Integer = 0

        If oGuide.Import IsNot Nothing Then
            ReDim Preserve oTabButtons(iNextTabIndex)

            oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_00", Me.Language.Translate("tabImportName", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
            oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs('00');")
            If intActiveTab = "00" Then iSelectedTabIndex = iNextTabIndex
            iNextTabIndex = iNextTabIndex + 1
        Else
            If intActiveTab = "00" Then intActiveTab = "01"
        End If

        If oGuide.Export IsNot Nothing Then
            ReDim Preserve oTabButtons(iNextTabIndex)
            oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_01", Me.Language.Translate("tabExportName", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
            oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs('01');")
            If intActiveTab = "01" Then iSelectedTabIndex = iNextTabIndex
            iNextTabIndex = iNextTabIndex + 1
        Else
            If intActiveTab = "01" Then intActiveTab = "00"
        End If

        Dim bInclude As Boolean = False
        If oGuide.Export IsNot Nothing AndAlso Me.HasFeaturePermission("Administration.ExportGuidesTemplates", Permission.Admin) AndAlso oGuide.Export.Templates IsNot Nothing AndAlso oGuide.Export.Templates.Count > 0 Then
            ' Solo incluimos la pestaña de planillas si la version no es ONE
            Dim oLicSupport As New Robotics.VTBase.Extensions.roLicenseSupport()
            Dim oLicInfo As Robotics.VTBase.Extensions.roVTLicense = oLicSupport.GetVTLicenseInfo()
            If oLicInfo.Edition <> Robotics.VTBase.Extensions.roServerLicense.roVisualTimeEdition.Starter Then
                For Each oTemplate In oGuide.Export.Templates
                    If oTemplate.TemplateFile <> String.Empty Then bInclude = True
                Next
            End If
        End If

        If bInclude Then
            ReDim Preserve oTabButtons(iNextTabIndex)
            oTabButtons(iNextTabIndex) = CreateNewHtmlAnchor("TABBUTTON_02", Me.Language.Translate("tabTemplateName", Me.DefaultScope), "bTab")
            hTableCellButtons.Controls.Add(oTabButtons(iNextTabIndex))
            hTableRowButtons.Cells.Add(hTableCellButtons)
            hTableButtons.Rows.Add(hTableRowButtons)
            hTableCellButtons = New HtmlTableCell
            hTableRowButtons = New HtmlTableRow
            oTabButtons(iNextTabIndex).Attributes.Add("OnClick", "javascript: changeTabs('02');")
            If intActiveTab = "02" Then iSelectedTabIndex = iNextTabIndex
            iNextTabIndex = iNextTabIndex + 1

        Else
            If intActiveTab = "02" Then
                If oGuide.Export IsNot Nothing Then
                    intActiveTab = "01"
                Else
                    intActiveTab = "00"
                End If
            End If
        End If

        'Afegim a la taula principal
        hCellGen.Controls.Add(hTableButtons)
        hRowGen.Cells.Add(hCellGen)
        hTableGen.Rows.Add(hRowGen)

        oTabButtons(iSelectedTabIndex).Attributes("class") = "bTab-active"

        Return hTableGen ' Retorna el HTMLTable

    End Function

    Private Function CreateNewHtmlAnchor(ByVal Name As String, ByVal Text As String, ByVal CssClassPrefix As String) As HtmlAnchor
        Dim obutton As New HtmlAnchor
        obutton.ID = Name
        obutton.HRef = "javascript: void(0);"
        obutton.Attributes("class") = CssClassPrefix
        obutton.InnerHtml = Text
        Return obutton
    End Function

End Class