Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Web.UI
Imports System.Web.UI.WebControls

'<Designer("System.Web.UI.Design.ReadWriteControlDesigner, System.Design")> _
<Designer(GetType(roDetailFrameDesigner))>
<ToolboxData("<{0}:roDetailFrame runat=server></{0}:roDetailFrame>"), DesignTimeVisible(True)>
<ParseChildren(False)>
<Bindable(True)>
<PersistChildren(True)>
Public Class roSummaryFrame
    Inherits WebControl
    Implements INamingContainer

    Dim strContextMenuScript As String
    Dim strCssPrefix As String
    Dim strBackGroundImage As String
    Dim strSummaryImage As String
    Dim strDropDownImage As String
    Dim strToolsImage As String
    Dim intBorder As Integer

    Dim oHeaderContent As ITemplate
    Dim oImageDropDown As Image
    Dim oPopUpExtender As AjaxControlToolkit.PopupControlExtender
    Dim oTabButtons As New ArrayList

    Dim oToolImage As Image
    Dim oPopUpToolsExtender As AjaxControlToolkit.PopupControlExtender
    Dim oToolsButtons As New ArrayList

    Dim oBackgroundImage As Image

#Region "Métodos"

    Private Function NormalizePath(ByVal Path As String) As String
        Dim strResultText As String

        If Not DesignMode Then
            strResultText = Replace(Path, "../", "")
            strResultText = Replace(strResultText, "~/", "")
        Else
            strResultText = Replace(Path, "~/", "../")
        End If

        Return strResultText
    End Function

    Private Function CreateImage(ByVal Name As String) As Image
        Dim oImage As New Image

        oImage.ID = Name

        Return oImage
    End Function

    Private Function CreatePopupExtender(ByVal ExtenderName As String, ByVal PopupFrame As String, ByVal Target As String) As AjaxControlToolkit.PopupControlExtender
        Dim oPopupExtender As New AjaxControlToolkit.PopupControlExtender

        oPopupExtender.ID = ExtenderName
        oPopupExtender.Page = Me.Page
        oPopupExtender.PopupControlID = PopupFrame
        oPopupExtender.Position = AjaxControlToolkit.PopupControlPopupPosition.Bottom
        oPopupExtender.TargetControlID = Target

        Return oPopupExtender
    End Function

    Private Function CreateUpdatePanel() As System.Web.UI.UpdatePanel
        Dim oUpdatePanel As New UpdatePanel

        oUpdatePanel.ID = "MyUpdatePanel"
        oUpdatePanel.Page = Me.Page

        Return oUpdatePanel
    End Function

    Private Function CreatePopUpPanel() As Panel
        Dim oPanel As New Panel

        oPanel.ID = "MyPopUpFrame"
        oPanel.Visible = False

        Return oPanel
    End Function

    Private Sub CreateTabButtonArray()
        Dim oControl As Control
        Dim strControlName As String

        For Each oControl In Controls
            If oControl.ID IsNot Nothing Then
                strControlName = oControl.ID
                If strControlName.ToUpper.Contains("TABBUTTON") = True Then
                    oTabButtons.Add(oControl)
                End If
            End If
        Next

    End Sub

    Private Sub CreateToolsButtonArray()
        Dim oControl As Control
        Dim strControlName As String

        For Each oControl In Controls
            If oControl.ID IsNot Nothing Then
                strControlName = oControl.ID
                If strControlName.ToUpper.Contains("TOOLBUTTON") = True Then
                    oToolsButtons.Add(oControl)
                End If
            End If
        Next

    End Sub

    Private Function GetJavaScriptCode() As String
        Dim strCode As String

        strCode = "<script type='text/javascript'> "
        strCode = strCode & " function ResizeBackgroundDiv(){ "
        strCode = strCode & "var dw = 0; "
        strCode = strCode & "document.getElementById('DivBackground').style.width = '1px';"
        strCode = strCode & "document.getElementById('BackgroundImage').style.width = '1px';"
        strCode = strCode & "dw = document.getElementById('BackGroundCell').clientWidth;"
        strCode = strCode & "document.getElementById('DivBackground').style.width = dw + 'px';"
        strCode = strCode & "document.getElementById('BackgroundImage').style.width = dw + 'px';"
        strCode = strCode & "ResizeDiv();"
        'strCode = strCode & "document.getElementById('txtWidth').value = dw + 'px';"
        strCode = strCode & "}"

        strCode = strCode & " function ResizeDiv(){ "
        strCode = strCode & "var dw = 0; "
        strCode = strCode & "dw = document.getElementById('BackGroundCell').clientWidth;"
        strCode = strCode & "document.getElementById('DivContent').style.width = dw + 'px';"
        strCode = strCode & "document.getElementById('DivContent').style.top = document.getElementById('DivBackground').offsetTop + 'px';"
        strCode = strCode & "document.getElementById('DivContent').style.left = document.getElementById('DivBackground').offsetLeft + 'px';"
        strCode = strCode & "}"

        strCode = strCode & "</script>"

        Return strCode
    End Function

#End Region

#Region "Properties"

    Public Property ContextMenuScript() As String
        Get
            Return strContextMenuScript
        End Get
        Set(ByVal value As String)
            strContextMenuScript = value
        End Set
    End Property

    Public Property Border() As Integer
        Get
            Return intBorder
        End Get
        Set(ByVal value As Integer)
            intBorder = value
        End Set
    End Property
    Public Property CssPrefix() As String
        Get
            If strCssPrefix = "" Then strCssPrefix = "Summary"
            Return strCssPrefix
        End Get
        Set(ByVal value As String)
            strCssPrefix = value
        End Set
    End Property

    <Browsable(False), Description("The statistics template.")>
    <PersistenceMode(PersistenceMode.InnerProperty)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Public Property HeaderContent() As ITemplate
        Get
            EnsureChildControls()
            Return oHeaderContent
        End Get
        Set(ByVal value As ITemplate)
            oHeaderContent = value
        End Set
    End Property

    Public Overrides ReadOnly Property Controls() As System.Web.UI.ControlCollection
        Get
            EnsureChildControls()
            Return MyBase.Controls
        End Get
    End Property
    <Bindable(True), Category("Image"), UrlProperty()>
    <Editor(GetType(System.Web.UI.Design.UrlEditor), GetType(System.Drawing.Design.UITypeEditor))>
    Public Property BackGroundImage() As String
        Get
            Return NormalizePath(strBackGroundImage)
        End Get
        Set(ByVal value As String)
            strBackGroundImage = value
        End Set
    End Property

    <Bindable(True), Category("Image"), DefaultValue(""), UrlProperty()>
    <Editor(GetType(System.Web.UI.Design.UrlEditor), GetType(System.Drawing.Design.UITypeEditor))>
    Public Property SummaryImage() As String
        Get
            Return NormalizePath(strSummaryImage)
        End Get
        Set(ByVal value As String)
            strSummaryImage = value
        End Set
    End Property

    <Bindable(True), Category("Image"), DefaultValue(""), UrlProperty()>
    <Editor(GetType(System.Web.UI.Design.UrlEditor), GetType(System.Drawing.Design.UITypeEditor))>
    Public Property DropDownImage() As String
        Get
            Return NormalizePath(strDropDownImage)
        End Get
        Set(ByVal value As String)
            strDropDownImage = value
        End Set
    End Property

    <Bindable(True), Category("Image"), DefaultValue(""), UrlProperty()>
    <Editor(GetType(System.Web.UI.Design.UrlEditor), GetType(System.Drawing.Design.UITypeEditor))>
    Public Property ToolsImage() As String
        Get
            Return NormalizePath(strToolsImage)
        End Get
        Set(ByVal value As String)
            strToolsImage = value
        End Set
    End Property

#End Region

#Region "Overrides"

    Protected Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim oControl As System.Web.UI.Control
        Dim intCounter As Integer

        EnsureChildControls()

        CreateTabButtonArray()
        CreateToolsButtonArray()

        oImageDropDown.CssClass = strCssPrefix & "_DropDownImage"
        oImageDropDown.ImageUrl = Me.Page.ResolveUrl("~/Base/Images/Transparencia.Gif")
        oToolImage.CssClass = strCssPrefix & "_ToolImage"
        oToolImage.ImageUrl = Me.Page.ResolveUrl("~/Base/Images/Transparencia.Gif")
        'oToolImage.Width = "24"
        'oToolImage.Height = "24"

        'oBackgroundImage.CssClass = strCssPrefix & "_SummaryImage"
        oBackgroundImage.ImageUrl = Me.Page.ResolveUrl("~/Base/Images/Main/" & strCssPrefix & "_BackGround5.Gif")
        oBackgroundImage.Width = "100"
        oBackgroundImage.Height = "120"

        ' Border Table
        writer.Write(GetJavaScriptCode)
        'writer.Write("<input id='txtWidth' type='text' value='' />")
        writer.Write("<table id='SummaryMainTable' border='" & intBorder & "' cellpadding='0' cellspacing='0' style='width:100%; border-color:transparent; ' onresize='ResizeBackgroundDiv();' >")
        'writer.Write("<tr style='width:100%; height:10px;'>")
        'writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='border-color:White; width:10px; height:10px;' > </td>")
        'writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='border-color:White; height:10px;'> </td>")
        'writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='border-color:White; width:10px; height:10px;'> </td>")
        'writer.Write("</tr>")
        writer.Write("<tr  style='width:100%; height:120px;'>")
        writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='border-color:White; width:1px; height:100px;'></td>")
        writer.Write("<td id='BackGroundCell' style='border-color:White; height:100px; background-color:White;'>")
        writer.Write("<div id='DivBackground' onload='ResizeBackgroundDiv();' runat='server' style='position:relative; left:0px; top:0px; height:120px; background-color:Transparent'>")
        oBackgroundImage.Attributes.Add("OnLoad", "ResizeBackgroundDiv();")
        oBackgroundImage.RenderControl(writer)
        writer.Write("</div>")
        writer.Write("<div id='DivContent' onload='ResizeDiv();' style='position:absolute; left:0px; top:0px; height:100px; background-color:Transparent; z-index:0;'>")
        '' Summary Table
        writer.Write("<table border='" & intBorder & "' style='border-color:White; width:100%; height:100%; vertical-align:top' cellpadding='0' cellspacing='0'>")
        writer.Write("<tr>")
        writer.Write("<td style='padding-left:10px; padding-top:10px; border-color:White; width:77%; height:100%; background-color:Transparent'>")
        writer.Write("<div id='DivContent_InformationFrame' oncontextmenu=""" & strContextMenuScript & """ >")
        ' ************************************************************************
        ' Controles dentro del summary
        ' ************************************************************************
        For Each oControl In Controls
            If oControl.ID IsNot Nothing Then
                If oControl.ID.ToUpper.Contains("TOOLBUTTON") = False And
                    oControl.ID.ToUpper.Contains("TABBUTTON") = False And
                    oControl.ID.Contains("MyPopupFrameImage") = False And
                    oControl.ID.Contains("MyPopupExtenderTab") = False And
                    oControl.ID.Contains("MyPopupFrameImageTool") = False And
                    oControl.ID.Contains("MyPopupExtenderTool") = False Then
                    oControl.RenderControl(writer)
                End If
            End If
        Next
        ' ************************************************************************
        ' END Controles dentro del summary
        ' ************************************************************************
        writer.Write("</div></td>")
        writer.Write("<td style='border-color:White; width:24px; height:100%; background-color:Transparent; vertical-align:bottom' align='right'>")
        ' ToolButton
        If oToolsButtons.Count > 0 Then
            oToolImage.RenderControl(writer)
            oPopUpToolsExtender.RenderControl(writer)
        End If
        writer.Write("</td>")
        writer.Write("<td style='border-color:White; width:100%; height:100%; background-color:Transparent; padding-right:10px' align='right'>")
        ' Tabs Table con DropDown
        writer.Write("<table border='" & intBorder & "' style='border-color:White; width:100%; vertical-align:top' cellpadding='0' cellspacing='0' align='right'>")
        writer.Write("<tr style='width:100%; height:24px; background-color:Transparent'><td style='border-color:White' align='right'>")
        ' Boton 1
        If oTabButtons.Count >= 1 Then
            oTabButtons(0).RenderControl(writer)
        End If
        If oTabButtons.Count >= 8 Then
            oTabButtons(1).RenderControl(writer)
        End If
        writer.Write("</td></tr>")
        writer.Write("<tr style='width:100%; height:24px; background-color:Transparent'><td style='border-color:White;' align='right'>")
        ' Boton 2
        If oTabButtons.Count >= 8 Then
            If oTabButtons.Count >= 3 Then
                oTabButtons(2).RenderControl(writer)
            End If
            If oTabButtons.Count >= 4 Then
                oTabButtons(3).RenderControl(writer)
            End If
        Else
            If oTabButtons.Count = 7 Then
                oTabButtons(6).RenderControl(writer)
            End If
            If oTabButtons.Count >= 2 Then
                oTabButtons(1).RenderControl(writer)
            End If
        End If
        writer.Write("</td></tr>")
        writer.Write("<tr style='width:100%; height:24px; background-color:Transparent'><td style='border-color:White;' align='right'>")
        ' Boton 3
        If oTabButtons.Count >= 8 Then
            If oTabButtons.Count >= 5 Then
                oTabButtons(4).RenderControl(writer)
            End If
            If oTabButtons.Count >= 6 Then
                oTabButtons(5).RenderControl(writer)
            End If
        Else
            If oTabButtons.Count = 6 Then
                oTabButtons(5).RenderControl(writer)
            End If
            If oTabButtons.Count >= 3 Then
                oTabButtons(2).RenderControl(writer)
            End If
        End If
        writer.Write("</td></tr>")
        writer.Write("<tr style='width:100%; height:24px; background-color:Transparent'><td style='border-color:White;' align='right'>")
        ' Boton 4 o Desplegable
        If oTabButtons.Count >= 8 Then
            'If oTabButtons.Count >= 7 Then
            'oTabButtons(6).RenderControl(writer)
            'End If
            oImageDropDown.RenderControl(writer)
            oPopUpExtender.RenderControl(writer)
        Else
            If oTabButtons.Count = 5 Then
                oTabButtons(4).RenderControl(writer)
            End If
            If oTabButtons.Count >= 4 Then
                oTabButtons(3).RenderControl(writer)
            End If
        End If

        'If oTabButtons.Count > 8 Then

        '    If oTabButtons.Count >= 4 Then
        '        oTabButtons(3).RenderControl(writer)
        '    End If
        'Else
        '    If oTabButtons.Count >= 8 Then
        '        oTabButtons(7).RenderControl(writer)
        '    End If

        '    If oTabButtons.Count >= 4 Then
        '        oTabButtons(3).RenderControl(writer)
        '    End If
        'End If

        writer.Write("</td></tr>")
        writer.Write("</table>")
        ' End Tabs Table
        writer.Write("</td>")
        writer.Write("</tr>")
        writer.Write("</table>")
        ' End Summary Table
        writer.Write("</div>")
        writer.Write("</td>")
        writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='width:1px; height:100px; '> </td>")
        writer.Write("</tr>")
        'writer.Write("<tr  style='width:100%; height:10px;' >")
        'writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='width:10px; height:10px; '> </td>")
        'writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='height:10px; '> </td>")
        'writer.Write("<td class='" & strCssPrefix & "_BackGroundImage" & "' style='width:10px; height:10px; '> </td>")
        'writer.Write("</tr>")
        writer.Write("</table>")

        ' ************************************************************************
        ' Popup panel y Dropdowns buttons TABS
        ' ************************************************************************
        Dim oUpdatePanel As UpdatePanel
        Dim oPanel As New Literal
        oUpdatePanel = CreateUpdatePanel()

        oPanel.Text = "<div ID='MyPopupFrameTab' style='display:none'>"
        oPanel.Text = oPanel.Text & "<Table Style='background-color:Transparent; vertical-align:top'>"
        For intCounter = 6 To oTabButtons.Count - 1
            oPanel.Text = oPanel.Text & "<tr><td>"

            Dim oStringBuilder As New StringBuilder()
            Dim oStringWriter As New StringWriter(oStringBuilder)
            Dim oAuxWriter As New HtmlTextWriter(oStringWriter)
            oTabButtons(intCounter).RenderControl(oAuxWriter)
            oPanel.Text = oPanel.Text & oStringBuilder.ToString

            oPanel.Text = oPanel.Text & "</td></tr>"
        Next
        oPanel.Text = oPanel.Text & "</table>"
        oPanel.Text = oPanel.Text & "</div>"

        oUpdatePanel.ContentTemplateContainer.Controls.Add(oPanel)
        oUpdatePanel.RenderControl(writer)
        ' ************************************************************************
        ' END Popup panel y Dropdowns buttons TABS
        ' ************************************************************************

        ' ************************************************************************
        ' Popup panel TOOLS
        ' ************************************************************************
        oPanel = New Literal
        oUpdatePanel = CreateUpdatePanel()

        oPanel.Text = "<div ID='MyPopupFrameTool' style='display:none'>"
        oPanel.Text = oPanel.Text & "<Table Style='background-color:Transparent; vertical-align:top'>"
        For intCounter = 0 To oToolsButtons.Count - 1
            oPanel.Text = oPanel.Text & "<tr><td>"

            Dim oStringBuilder As New StringBuilder()
            Dim oStringWriter As New StringWriter(oStringBuilder)
            Dim oAuxWriter As New HtmlTextWriter(oStringWriter)
            oToolsButtons(intCounter).RenderControl(oAuxWriter)
            oPanel.Text = oPanel.Text & oStringBuilder.ToString

            oPanel.Text = oPanel.Text & "</td></tr>"
        Next
        oPanel.Text = oPanel.Text & "</table>"
        oPanel.Text = oPanel.Text & "</div>"

        oUpdatePanel.ContentTemplateContainer.Controls.Add(oPanel)
        oUpdatePanel.RenderControl(writer)
        ' ************************************************************************
        ' END Popup panel TOOLS
        ' ************************************************************************

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        MyBase.Render(writer)
    End Sub

#End Region

    Public Sub New()
        strCssPrefix = "Summary"
        oImageDropDown = CreateImage("MyPopupFrameImage")
        Controls.Add(oImageDropDown)
        oPopUpExtender = CreatePopupExtender("MyPopupExtenderTab", "MyPopupFrameTab", "MyPopupFrameImage")
        Controls.Add(oPopUpExtender)

        oToolImage = CreateImage("MyPopupFrameImageTool")
        Controls.Add(oToolImage)
        oPopUpToolsExtender = CreatePopupExtender("MyPopupExtenderTool", "MyPopupFrameTool", "MyPopupFrameImageTool")
        Controls.Add(oPopUpToolsExtender)

        oBackgroundImage = CreateImage("BackgroundImage")
    End Sub

End Class

<ToolboxItem(False)>
Public Class roContentTemplate
    Inherits WebControl
    Implements INamingContainer

    Private oParent As roSummaryFrame

    Public Sub New(ByVal Parent As roSummaryFrame)
        Me.oParent = Parent
    End Sub

End Class

Public Class roDetailFrameDesigner
    Inherits System.Web.UI.Design.ContainerControlDesigner

    Private localDetailFrame As roSummaryFrame

    Public Overrides Function GetDesignTimeHtml() As String

        localDetailFrame = CType(Component, roSummaryFrame)

        Dim sw As New System.IO.StringWriter
        Dim tw As New HtmlTextWriter(sw)

        localDetailFrame.RenderBeginTag(tw)
        localDetailFrame.RenderControl(tw)
        localDetailFrame.RenderEndTag(tw)

        Return sw.ToString
    End Function

End Class