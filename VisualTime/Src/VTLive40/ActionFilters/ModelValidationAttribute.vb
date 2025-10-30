Imports System.Net
Imports System.Net.Http
Imports System.Web.Http.Controllers
Imports System.Web.Http.Filters

''' <summary>
'''
''' </summary>
Public Class ModelValidationAttribute
    Inherits ActionFilterAttribute

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="actionContext"></param>
    Public Overrides Sub OnActionExecuting(ByVal actionContext As HttpActionContext)

        If (Not actionContext.ModelState.IsValid) Then
            actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.BadRequest, actionContext.ModelState)
        End If

    End Sub

End Class