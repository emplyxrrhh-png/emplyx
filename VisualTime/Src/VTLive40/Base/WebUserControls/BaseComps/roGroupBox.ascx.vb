Partial Class base_WebUserControls_roGroupBox
    Inherits System.Web.UI.UserControl

    Private cssCheckBox As String = "OptionPanelCheckBoxStyle"
    Private BorderStyle As Boolean = True

    Private strBorderClass As String = "optionPanelRoboticsV2"
    Public tblCSSPrefix As String = "op"

#Region "Propietats"

    <PersistenceMode(PersistenceMode.InnerProperty)> Public ReadOnly Property Content() As PlaceHolder
        Get
            Return externalContent
        End Get
    End Property

    Public Property BorderClass() As String
        Get
            If ViewState("BorderClass") IsNot Nothing Then
                Return ViewState("BorderClass")
            Else
                Return strBorderClass
            End If
        End Get
        Set(ByVal value As String)
            strBorderClass = value
            ViewState("BorderClass") = value
        End Set
    End Property

#End Region

End Class