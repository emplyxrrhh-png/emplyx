Imports System.ComponentModel
Imports System.IO
Imports System.Text
Imports System.Web.UI

<Serializable()>
<ToolboxData("<{0}:roTreeView runat=server></{0}:roTreeView>"), DesignTimeVisible(True)>
Public Class roTreeView
    Inherits System.Web.UI.WebControls.TreeView
    Implements IPostBackEventHandler
    Implements INamingContainer
    Implements System.Web.UI.ICallbackEventHandler
    Implements System.Web.UI.IAttributeAccessor

    Public Event CheckClick()

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
        Dim strBuilder As New StringBuilder
        Dim strWriter As New StringWriter(strBuilder)
        Dim htmlWriter As New HtmlTextWriter(strWriter)

        MyBase.Render(htmlWriter)

        Dim strFind As String
        Dim strReplace As String

        strFind = "<input type=""checkbox"" "
        strReplace = "<input type=""checkbox"" onClick=""" + getPostBack() + """ "
        writer.Write(strBuilder.ToString.Replace(strFind, strReplace))

    End Sub

    Protected Function getPostBack() As String
        Return Me.Page.ClientScript.GetPostBackEventReference(Me, "@CheckPostBack")
        'Return Me.Page.GetPostBackEventReference(Me, "@CheckPostBack")
    End Function

    Protected Sub OnCheckClick(ByVal e As EventArgs)
        RaiseEvent CheckClick()
    End Sub

    Protected Overrides Sub RaiseCallbackEvent(ByVal eventArgument As String)
        OnCheckClick(New EventArgs())
    End Sub

End Class