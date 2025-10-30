Public Class indexv2
    Inherits System.Web.UI.Page

    Private _tick As String = ""
    Private _masterVersion As String = ""

    Public ReadOnly Property Tick() As String
        Get
            Return _tick
        End Get
    End Property

    Public ReadOnly Property MasterVersion() As String
        Get
            Return _masterVersion
        End Get
    End Property

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        _tick = "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
        _masterVersion = "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
    End Sub

End Class