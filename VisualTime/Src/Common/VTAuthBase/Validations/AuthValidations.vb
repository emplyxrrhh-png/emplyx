Imports System.Web
Imports Robotics.VTBase

Public Class AuthValidations

    Public Shared Function IsAlreadyLoggedOnAnotherCompany() As Boolean
        Dim currentCompanyName As String = roConstants.GetGlobalEnvironmentParameter(Robotics.Base.DTOs.GlobalAsaxParameter.CompanyId).ToString().Trim().ToLower()

        'Si no tengo nombre actual de empresa no estoy logeado
        If currentCompanyName = String.Empty Then Return False

        'Si no tengo el contexto de la aplicación no estoy logeado
        If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing OrElse HttpContext.Current.Request Is Nothing Then Return False

        'Si no tengo ningun pasaporte en la sesión no estoy logeado
        If WLHelperWeb.CurrentPassport(True) Is Nothing Then Return False


        Dim requestPaths As String() = HttpContext.Current.Request.Url.LocalPath.Split("/")
        Dim refererCompany As String = String.Empty

        If requestPaths(1).ToUpper = "AUTH" AndAlso requestPaths.Length = 3 Then
            'Si la llamada es al path de autenticación de SSO con el nombre de empresa, leo la empresa del path
            refererCompany = requestPaths(2).Trim()
        ElseIf roTypes.Any2String(HttpContext.Current.Request.Params("referer")) <> String.Empty Then
            'Si la llamada es al path de autenticación de SSO con el nombre de empresa, leo la empresa del path
            refererCompany = roTypes.Any2String(HttpContext.Current.Request.Params("referer")).Trim()
        End If

        If refererCompany <> String.Empty Then
            If currentCompanyName <> refererCompany Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function


    Public Shared Function IsAlreadyLoggedOnAnotherCompany(ByVal refererCompany As String) As Boolean
        Dim currentCompanyName As String = roConstants.GetGlobalEnvironmentParameter(Robotics.Base.DTOs.GlobalAsaxParameter.CompanyId).ToString().Trim().ToLower()

        'Si no tengo nombre actual de empresa no estoy logeado
        If currentCompanyName = String.Empty Then Return False

        'Si no tengo el contexto de la aplicación no estoy logeado
        If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing OrElse HttpContext.Current.Request Is Nothing Then Return False

        'Si no tengo ningun pasaporte en la sesión no estoy logeado
        If WLHelperWeb.CurrentPassport(True) Is Nothing Then Return False

        If refererCompany <> String.Empty Then
            If currentCompanyName <> refererCompany Then
                Return True
            Else
                Return False
            End If
        Else
            Return False
        End If

    End Function


    Public Shared Function GetCompanyWithSessionInitiated() As String
        Dim currentCompanyName As String = roConstants.GetGlobalEnvironmentParameter(Robotics.Base.DTOs.GlobalAsaxParameter.CompanyId).ToString().Trim().ToLower()

        'Si no tengo nombre actual de empresa no estoy logeado
        If currentCompanyName = String.Empty Then Return String.Empty

        'Si no tengo el contexto de la aplicación no estoy logeado
        If HttpContext.Current Is Nothing OrElse HttpContext.Current.Session Is Nothing OrElse HttpContext.Current.Request Is Nothing Then Return String.Empty

        'Si no tengo ningun pasaporte en la sesión no estoy logeado
        If WLHelperWeb.CurrentPassport(True) Is Nothing Then Return String.Empty


        Return currentCompanyName

    End Function

End Class

