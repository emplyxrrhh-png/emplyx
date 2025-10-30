Imports System.ComponentModel
Imports System.Web.UI
Imports System.Web.UI.WebControls

<DefaultProperty("Text"), ToolboxData("<{0}:roTitleFrame runat=server></{0}:roTitleFrame>")>
Public Class roTitleFrame
    Inherits WebControl
    Implements INamingContainer

#Region "Declarations - constructor"

    Dim oContent As ITemplate

#End Region

#Region "Properties"

    <Browsable(False), Description("Contenido del frame")>
    <PersistenceMode(PersistenceMode.InnerProperty)>
    <DesignerSerializationVisibility(DesignerSerializationVisibility.Content)>
    Public Property Content() As ITemplate
        Get
            EnsureChildControls()
            Return Me.oContent
        End Get
        Set(ByVal value As ITemplate)
            Me.oContent = value
        End Set
    End Property

    <Bindable(True), Category("Appearance"), DefaultValue(""), Localizable(True)> Property Text() As String
        Get
            Dim s As String = CStr(ViewState("Text"))
            If s Is Nothing Then
                Return String.Empty
            Else
                Return s
            End If
        End Get

        Set(ByVal Value As String)
            ViewState("Text") = Value
        End Set
    End Property

#End Region

#Region "Methods"

    Protected Overrides Sub RenderContents(ByVal writer As HtmlTextWriter)
        writer.Write(Text)
    End Sub

#End Region

End Class