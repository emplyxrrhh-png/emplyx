Imports Robotics.Web.Base

Partial Public Class login
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

        Robotics.Web.Base.HelperWeb.EraseCookie("IsMT")
        Robotics.Web.Base.HelperWeb.EraseCookie("MTLiveApiUrl")

        If Not Me.IsPostBack Then
            'Dim backgroundImage As String = "img-form_C.png"
            Dim backgroundImage As String = "Q" & HelperWeb.getSeason(Date.Now) & "-" & HelperWeb.RandomGenerator.Next(1, 6) & ".jpg"

            Me.backgroundcolumn.Style("background-image") = "url(img/" & backgroundImage & ");"
        End If
    End Sub

End Class