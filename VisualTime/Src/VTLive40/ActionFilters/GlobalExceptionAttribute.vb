Imports System.Web.Mvc

''' <summary>
''' Exception Attribute:  handle all requests exceptions
''' </summary>
Public Class GlobalExceptionAttribute
    Inherits HandleErrorAttribute

    ''' <summary>
    ''' Executed method when exception is produced
    ''' </summary>
    ''' <param name="filterContext"></param>
    Public Overrides Sub OnException(filterContext As ExceptionContext)
        MyBase.OnException(filterContext)
    End Sub

End Class