Imports System.Threading.Tasks
Imports Microsoft.Owin

Public Class OwinSessionMiddleware
    Inherits OwinMiddleware

    Public Sub New(ByVal [next] As OwinMiddleware)
        MyBase.New([next])
    End Sub

    Public Overrides Function Invoke(ByVal context As IOwinContext) As Task
        Dim httpContext = context.[Get](Of HttpContextBase)(GetType(HttpContextBase).FullName)
        httpContext.SetSessionStateBehavior(SessionStateBehavior.Required)

        Return Me.Next.Invoke(context)
    End Function

End Class