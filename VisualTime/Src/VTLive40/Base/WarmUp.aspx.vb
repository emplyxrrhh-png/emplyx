Partial Class WarmUp
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load

        'If Robotics.VTBase.roConstants.GetRuntimeEnvironment() = Robotics.VTBase.roConstants.roRuntimeLocation.MT OrElse Robotics.VTBase.roTypes.Any2Boolean(HelperSession.AdvancedParametersCache("VTLiveApi.Warmup")) Then
        '    Robotics.Web.Base.KeepAliveProcess.preloadWarmup()
        'End If

    End Sub

End Class