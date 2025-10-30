Imports System.Web.Http

Public Module WebApiConfig

    Public Sub Register(ByVal config As HttpConfiguration)
        ' Configuración y servicios de API web

        ' Rutas de API web
        config.MapHttpAttributeRoutes()

        config.Routes.MapHttpRoute(
            name:="DefaultApi",
            routeTemplate:="{controller}",
            defaults:=New With {.id = RouteParameter.Optional}
        )
    End Sub

End Module