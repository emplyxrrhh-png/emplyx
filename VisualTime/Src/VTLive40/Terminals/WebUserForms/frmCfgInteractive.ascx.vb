Imports Robotics.Web.Base

Partial Class frmCfgInteractive
    Inherits UserControlBase
    Public mIDReader As String

    Public Property IDReader() As String
        Get
            Return mIDReader
        End Get
        Set(ByVal value As String)
            mIDReader = value
        End Set
    End Property

    Protected Sub Page_Load1(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        Me.btnOk.OnClientClick = "frmCfgInteractive_Save('" & Me.IDReader & "'); return false;"
        Me.btnCancel.OnClientClick = "frmCfgInteractive_Close('" & Me.IDReader & "'); return false;"
    End Sub

End Class