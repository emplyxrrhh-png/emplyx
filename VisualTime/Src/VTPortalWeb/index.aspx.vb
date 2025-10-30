Public Class index
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

        Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalApi")
        Robotics.Web.Base.HelperWeb.EraseCookie("IsMT")
        Robotics.Web.Base.HelperWeb.EraseCookie("MTLiveApiUrl")

        If (Robotics.VTBase.roTypes.Any2String(Request.Params("userName"))) <> String.Empty Then
            Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalAdfsUserName")
            Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalAdfsUserName", Server.UrlDecode(Request.Params("userName")), False)
        End If

        If (Robotics.VTBase.roTypes.Any2String(Request.Params("token"))) <> String.Empty Then
            Robotics.Web.Base.HelperWeb.EraseCookie("VTPortalToken")
            Robotics.Web.Base.HelperWeb.CreateCookie("VTPortalToken", Server.UrlDecode(Request.Params("token")), False)
        End If

    End Sub

End Class