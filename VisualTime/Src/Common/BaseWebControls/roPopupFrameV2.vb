Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls

'<Designer("System.Web.UI.Design.ReadWriteControlDesigner, System.Design")> _
<Designer(GetType(roPopupFrameDesignerV2))>
<ToolboxData("<{0}:roPopupFrame runat=server></{0}:roPopupFrame>"), DesignTimeVisible(True)>
<ParseChildren(False)>
<Bindable(True)>
<PersistChildren(True)>
Public Class roPopupFrameV2
    Inherits WebControl
    Implements INamingContainer

    Const ControlBoxPrefix = "CBox"
    Const TitlePrefix = "Title"

    Dim oHideButton As System.Web.UI.WebControls.Button
    Dim oPopupExtender As AjaxControlToolkit.ModalPopupExtender

    Dim bolShowTitleBar As Boolean
    Dim strTitle As String
    Dim strBehaviorID As String

    Dim strScriptSrc As String
    Dim oContentTemplate As ITemplate
    Dim strCssPrefix As String
    ''Dim strTopLeftImage As String
    ''Dim strTopRightImage As String
    ''Dim strTopMidImage As String
    ''Dim strMarginLeftImage As String
    ''Dim strMarginRightImage As String
    ''Dim strBottomLeftImage As String
    ''Dim strBottomRightImage As String
    ''Dim strBottomMidImage As String

#Region "Métodos"

    Private Sub CreatePopupExtender()
        oPopupExtender = New AjaxControlToolkit.ModalPopupExtender

        oPopupExtender.ID = "MyModalPopupExtender"
        oPopupExtender.Page = Me.Page
        oPopupExtender.TargetControlID = oHideButton.ID
        oPopupExtender.DropShadow = False

        Controls.Add(oPopupExtender)
    End Sub

    Private Sub CreateHideButton()
        oHideButton = New System.Web.UI.WebControls.Button

        oHideButton.ID = "MyHideButton"
        oHideButton.Page = Me.Page
        oHideButton.Style("display") = "none"

        Controls.Add(oHideButton)

    End Sub

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

    Public Sub Show()
        Me.oPopupExtender.Show()
    End Sub

#End Region

#Region "Properties"

    <Browsable(False), Description("The statistics template.")>
    <TemplateContainer(GetType(roPopupFrameTemplateV2))>
    <PersistenceMode(PersistenceMode.InnerProperty)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Public Property FrameContentTemplate() As ITemplate
        Get
            Return oContentTemplate
        End Get
        Set(ByVal value As ITemplate)
            oContentTemplate = value
        End Set
    End Property
    Public Overrides ReadOnly Property Controls() As System.Web.UI.ControlCollection
        Get
            EnsureChildControls()
            Return MyBase.Controls
        End Get
    End Property

    Public Property BehaviorID() As String
        Get
            Return strBehaviorID
        End Get
        Set(ByVal value As String)
            strBehaviorID = value
        End Set
    End Property
    Public Property CssPrefix() As String
        Get
            If strCssPrefix = "" Then strCssPrefix = "PopupFrame"
            Return strCssPrefix
        End Get
        Set(ByVal value As String)
            strCssPrefix = value
        End Set
    End Property

    Public Property ShowTitleBar() As Boolean
        Get
            Return bolShowTitleBar
        End Get
        Set(ByVal value As Boolean)
            bolShowTitleBar = value
        End Set
    End Property

    Public Property Title() As String
        Get
            Return strTitle
        End Get
        Set(ByVal value As String)
            strTitle = value
        End Set
    End Property

    Public ReadOnly Property PopupExtender() As AjaxControlToolkit.ModalPopupExtender
        Get
            Return Me.oPopupExtender
        End Get
    End Property

    <Browsable(True), DefaultValue("modalBackground")>
    Public Property CssClassPopupExtenderBackground() As String
        Get
            Dim val As Object = Me.ViewState("CssClassPopupExtenderBackground")
            If val Is Nothing Then
                Return "modalBackground"
            Else
                Return CStr(val)
            End If
        End Get
        Set(ByVal value As String)
            Me.ViewState("CssClassPopupExtenderBackground") = value
        End Set
    End Property

    Public Property DragEnabled() As Boolean
        Get
            If ViewState("DragEnabled") Is Nothing Then
                Return True
            Else
                Return ViewState("DragEnabled")
            End If

        End Get
        Set(ByVal value As Boolean)
            ViewState("DragEnabled") = value
        End Set
    End Property

#End Region

#Region "Overrides"

    Public Sub New()
        'strCssPrefix = "PopupFrameGray"
        strCssPrefix = ""

        CreateHideButton()
        CreatePopupExtender()

    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        'Dim strScriptBlock As String

        MyBase.OnInit(e)

    End Sub

    Protected Overrides Sub CreateChildControls()
        Dim oControl As New WebControl

        If oContentTemplate IsNot Nothing Then
            oControl = New roPopupFrameTemplateV2(Me)
            oContentTemplate.InstantiateIn(oControl)

            Controls.Add(oControl)
        End If
    End Sub

    Protected Overrides Sub RenderContents(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim oControl As System.Web.UI.Control
        Dim strImageSufix As String

        EnsureChildControls()

        If ShowTitleBar Then
            strImageSufix = "_Title"
        Else
            strImageSufix = ""
        End If

        oHideButton.RenderControl(writer)

        If Not strBehaviorID Is Nothing Then
            If Me.DragEnabled Then
                oPopupExtender.PopupDragHandleControlID = Me.ID & "MyPopupFrameDrag"
            Else
                oPopupExtender.PopupDragHandleControlID = ""
            End If
            oPopupExtender.PopupControlID = Me.ID & "MyPopupFrame_DIV"
            oPopupExtender.BehaviorID = strBehaviorID
            oPopupExtender.BackgroundCssClass = Me.CssClassPopupExtenderBackground
            oPopupExtender.RenderControl(writer)
        End If

        For Each oControl In Me.Controls
            If oControl.ID IsNot Nothing Then
                If oControl.ID.ToUpper.StartsWith(TitlePrefix.ToUpper) Then
                    CType(oControl, Object).CssClass = strCssPrefix & "_Title"
                End If
            End If
        Next

        If strBehaviorID <> Nothing Then
            writer.Write("<div runat='server' id='" & Me.ID & "MyPopupFrame_DIV' class='bodyPopupExtended " & CssPrefix & "' style='display:none;'>") ' width:" & Me.Width.ToString & "; Height:" & Me.Height.ToString & ";'>")
        Else
            writer.Write("<div runat='server' id='" & Me.ID & "MyPopupFrame_DIV' class='bodyPopupExtended " & CssPrefix & "' >") ' Style='width:" & Me.Width.ToString & "; Height:" & Me.Height.ToString & ";'>")
        End If

        If ShowTitleBar Then
            For Each oControl In Me.Controls
                If oControl.ID IsNot Nothing Then
                    If oControl.ID.ToUpper.StartsWith(TitlePrefix.ToUpper) Then
                        oControl.RenderControl(writer)
                    End If
                End If
            Next
            For Each oControl In Me.Controls
                If oControl.ID IsNot Nothing Then
                    If oControl.ID.ToUpper.StartsWith(ControlBoxPrefix.ToUpper) Then
                        oControl.RenderControl(writer)
                    End If
                End If
            Next
        End If

        For Each oControl In Me.Controls
            If oControl.ID IsNot Nothing Then
                If Not oControl.ID.ToUpper.StartsWith(ControlBoxPrefix.ToUpper) And Not oControl.ID.ToUpper.StartsWith(TitlePrefix.ToUpper) _
                And oControl.ID <> oPopupExtender.ID And oControl.ID <> oHideButton.ID Then
                    oControl.RenderControl(writer)
                End If
            Else
                oControl.RenderControl(writer)
            End If
        Next

        writer.Write("</div>")

    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)

        MyBase.Render(writer)

    End Sub

#End Region

End Class

<ToolboxItem(False)>
Public Class roPopupFrameTemplateV2
    Inherits WebControl
    Implements INamingContainer

    Private oParent As roPopupFrameV2

    Public Sub New(ByVal Parent As roPopupFrameV2)
        Me.oParent = Parent
    End Sub

End Class

Public Class roPopupFrameDesignerV2
    Inherits System.Web.UI.Design.ContainerControlDesigner

    Private localPopupFrame As roPopupFrameV2

    Public Overrides Function GetDesignTimeHtml() As String

        localPopupFrame = CType(Component, roPopupFrameV2)

        Dim sw As New System.IO.StringWriter
        Dim tw As New HtmlTextWriter(sw)

        localPopupFrame.RenderBeginTag(tw)
        localPopupFrame.RenderControl(tw)
        localPopupFrame.RenderEndTag(tw)

        Return sw.ToString
    End Function

End Class