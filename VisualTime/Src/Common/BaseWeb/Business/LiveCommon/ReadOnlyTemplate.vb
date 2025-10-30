Imports System.Web
Imports System.Web.UI
Imports DevExpress.Web

Public Class ReadOnlyTemplate
    Implements ITemplate

    Public Sub InstantiateIn1(ByVal container As System.Web.UI.Control) Implements System.Web.UI.ITemplate.InstantiateIn
        Dim Auxcontainer As GridViewEditItemTemplateContainer = CType(container, GridViewEditItemTemplateContainer)
        Dim lbl As New ASPxLabel()
        lbl.ID = "lbl"
        Auxcontainer.Controls.Add(lbl)
        lbl.Text = IIf(Auxcontainer.Text = "&nbsp;", "", HttpContext.Current.Server.HtmlDecode(Auxcontainer.Text))
        lbl.Width = 10
        lbl.Font.Size = 8
    End Sub

End Class