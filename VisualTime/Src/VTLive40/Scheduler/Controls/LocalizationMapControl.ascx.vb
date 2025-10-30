Partial Class Scheduler_Controls_LocalizationMapControl
    Inherits System.Web.UI.UserControl

    Public Property Data() As String
        Get
            Return ViewState("Coords")
        End Get
        Set(ByVal value As String)
            ViewState("Coords") = value
        End Set
    End Property

    Public Sub LoadMap()
        Dim sufix As String = ""
        If Not String.IsNullOrEmpty(Data) Then
            sufix = "?points=" + Data
        End If
        Me.MapFrame.Attributes.Add("src", "LocalizationMapPage.aspx" + sufix)
    End Sub

    Public Sub LoadMap(src As String)
        Dim sufix As String = ""
        If Not String.IsNullOrEmpty(Data) Then
            sufix = "?points=" + Data
        End If

        Me.MapFrame.Attributes.Add("src", VirtualPathUtility.ToAbsolute(src) + sufix)
    End Sub

End Class