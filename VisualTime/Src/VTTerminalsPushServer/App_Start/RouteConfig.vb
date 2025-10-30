Public Module RouteConfig

    Public Sub RegisterRoutes(ByVal routes As RouteCollection)
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}")

        routes.MapRoute(
            name:="Default",
            url:="{controller}/{action}/{id}",
            defaults:=New With {.controller = "IClock", .action = "CData", .id = UrlParameter.Optional}
        )
    End Sub

End Module