Imports System.Web
Imports Robotics.VTBase

Public Class CorsSupport

    Private Sub New()
    End Sub

    Public Shared Sub HandlePreflightRequest(context As HttpContext)
        Dim req As HttpRequest = context.Request
        Dim res As HttpResponse = context.Response
        Dim origin As String = roTypes.Any2String(req.Headers("Origin"))
        Dim bIsApp As Boolean = roTypes.Any2Boolean(req.Headers("roSrc"))

#If DEBUG Then
        bIsApp = True
#End If

        Dim corsStandar As Boolean = Not [String].IsNullOrEmpty(origin) AndAlso bIsApp
        Dim iphoneCors As Boolean = Not [String].IsNullOrEmpty(origin) AndAlso origin.ToLower().StartsWith("app://")

        If corsStandar OrElse iphoneCors Then
            res.AddHeader("Access-Control-Allow-Origin", origin)
            res.AddHeader("Access-Control-Allow-Credentials", "true")

            Dim methods = req.Headers("Access-Control-Request-Method")
            Dim headers = req.Headers("Access-Control-Request-Headers")

            If Not [String].IsNullOrEmpty(methods) Then
                res.AddHeader("Access-Control-Allow-Methods", methods)
            End If

            If Not [String].IsNullOrEmpty(headers) Then
                res.AddHeader("Access-Control-Allow-Headers", headers)
            End If

            If req.HttpMethod = "OPTIONS" Then
                res.StatusCode = 204
                res.[End]()
            End If
        End If
    End Sub

End Class