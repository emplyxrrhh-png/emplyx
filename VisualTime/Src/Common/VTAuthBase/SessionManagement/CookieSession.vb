Imports System.Web

Public Class CookieSession

    Private Sub New()
    End Sub

    Public Shared Function GetAuthenticationCookie(sAppName As String) As String
        Dim sCookieName As String = "ro_" & sAppName & "TokenID"
        If HttpContext.Current.Request.Cookies(sCookieName) IsNot Nothing Then
            Return HttpContext.Current.Server.UrlDecode(HttpContext.Current.Request.Cookies(sCookieName).Value)
        Else
            Return String.Empty
        End If

    End Function

    Public Shared Sub CreateAuthenticationCookie(bAuthenticated As Boolean, sContent As String, sAppName As String)
        Dim sCookieName As String = "ro_" & sAppName & "TokenID"
        If bAuthenticated Then
            If HttpContext.Current.Request.Cookies(sCookieName) IsNot Nothing Then
                Dim oCookie As New System.Web.HttpCookie(sCookieName, HttpContext.Current.Server.UrlEncode(sContent))
                oCookie.Expires = Date.Now.AddDays(30).ToUniversalTime()
                oCookie.HttpOnly = True

                oCookie.SameSite = SameSiteMode.Strict
                If HttpContext.Current.Request.IsSecureConnection Then
                    oCookie.Secure = True
                End If

                HttpContext.Current.Response.Cookies.Add(oCookie)
            Else
                Dim oCookie As New System.Web.HttpCookie(sCookieName, HttpContext.Current.Server.UrlEncode(sContent))
                oCookie.Expires = Date.Now.AddDays(30).ToUniversalTime()
                oCookie.HttpOnly = True

                oCookie.SameSite = SameSiteMode.Strict
                If HttpContext.Current.Request.IsSecureConnection Then
                    oCookie.Secure = True
                End If

                HttpContext.Current.Response.Cookies.Add(oCookie)
            End If
        Else
            If HttpContext.Current.Request.Cookies(sCookieName) IsNot Nothing Then
                Dim oCookie As New System.Web.HttpCookie(sCookieName, "")
                oCookie.Expires = Date.Now.AddDays(30).ToUniversalTime()
                oCookie.HttpOnly = True

                oCookie.SameSite = SameSiteMode.Strict
                If HttpContext.Current.Request.IsSecureConnection Then
                    oCookie.Secure = True
                End If

                HttpContext.Current.Response.Cookies.Add(oCookie)
            Else
                Dim oCookie As New System.Web.HttpCookie(sCookieName, "")
                oCookie.Expires = Date.Now.AddDays(30).ToUniversalTime()
                oCookie.HttpOnly = True

                oCookie.SameSite = SameSiteMode.Strict
                If HttpContext.Current.Request.IsSecureConnection Then
                    oCookie.Secure = True
                End If

                HttpContext.Current.Response.Cookies.Add(oCookie)
            End If
        End If
    End Sub

    Public Shared Sub ClearAuthenticationCookie(sAppName As String)
        Dim sCookieName As String = "ro_" & sAppName & "TokenID"
        If HttpContext.Current.Request.Cookies(sCookieName) IsNot Nothing Then
            Dim oCookie As New System.Web.HttpCookie(sCookieName, "")
            oCookie.Expires = Date.Now.AddDays(30).ToUniversalTime()
            oCookie.HttpOnly = True

            oCookie.SameSite = SameSiteMode.Strict
            If HttpContext.Current.Request.IsSecureConnection Then
                oCookie.Secure = True
            End If

            HttpContext.Current.Response.Cookies.Add(oCookie)
        End If
    End Sub

End Class