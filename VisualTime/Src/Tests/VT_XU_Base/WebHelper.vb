Imports System.Collections.Specialized

Public Class WebHelper

    Public Sub HttpContextStub(Optional dSessionParameters As Dictionary(Of String, String) = Nothing, Optional requestHeaders As Dictionary(Of String, String) = Nothing, Optional requestParams As Dictionary(Of String, String) = Nothing, Optional requestUserHostAddress As String = "8.8.8.8", Optional requestUrl As String = "http://localhost:8080")
        System.Web.Fakes.ShimHttpContext.CurrentGet = Function()
                                                          Dim context As New System.Web.Fakes.ShimHttpContext
                                                          context.SessionGet = Function()
                                                                                   Dim session As New System.Web.SessionState.Fakes.ShimHttpSessionState
                                                                                   session.ItemGetString = Function(key)
                                                                                                               If dSessionParameters Is Nothing Then Return String.Empty
                                                                                                               If dSessionParameters.ContainsKey(key) Then Return dSessionParameters(key)
                                                                                                               Return String.Empty
                                                                                                           End Function
                                                                                   session.ItemSetStringObject = Sub(key, value)
                                                                                                                     If dSessionParameters IsNot Nothing Then dSessionParameters(key) = value
                                                                                                                 End Sub
                                                                                   Return session
                                                                               End Function
                                                          context.ApplicationInstanceGet = Function()
                                                                                               Return New System.Web.HttpApplication
                                                                                           End Function
                                                          context.RequestGet = Function()
                                                                                   Dim instance As New System.Web.Fakes.ShimHttpRequest
                                                                                   instance.UrlGet = Function()
                                                                                                         Return New Uri(requestUrl)
                                                                                                     End Function
                                                                                   instance.UserHostAddressGet = Function()
                                                                                                                     Return requestUserHostAddress
                                                                                                                 End Function
                                                                                   instance.HeadersGet = Function()
                                                                                                             Dim nameValueCollection As New NameValueCollection()
                                                                                                             If requestHeaders Is Nothing Then Return nameValueCollection
                                                                                                             For Each kvp As KeyValuePair(Of String, String) In requestHeaders
                                                                                                                 nameValueCollection.Add(kvp.Key, kvp.Value)
                                                                                                             Next
                                                                                                             Return nameValueCollection
                                                                                                         End Function
                                                                                   instance.ParamsGet = Function()
                                                                                                            Dim nameValueCollection As New NameValueCollection()
                                                                                                            If requestParams Is Nothing Then Return nameValueCollection
                                                                                                            For Each kvp As KeyValuePair(Of String, String) In requestParams
                                                                                                                nameValueCollection.Add(kvp.Key, kvp.Value)
                                                                                                            Next
                                                                                                            Return nameValueCollection
                                                                                                        End Function
                                                                                   Return New System.Web.Fakes.ShimHttpRequest(instance)
                                                                               End Function
                                                          Return context
                                                      End Function

    End Sub

End Class