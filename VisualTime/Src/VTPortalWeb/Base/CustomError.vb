Imports System.Net
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Dispatcher
Imports System.ServiceModel.Web

Public Class CustomErrorHandler
    Implements IErrorHandler

    Public Function HandleError(ByVal [error] As Exception) As Boolean Implements IErrorHandler.HandleError
        Return True
    End Function

    'Public Sub ProvideFault(ByVal [error] As Exception, ByVal version As MessageVersion, ByRef fault As Message) Implements IErrorHandler.ProvideFault
    '    Dim faultException = New FaultException("Ocurrió un error en el servicio: " & [error].Message)
    '    Dim messageFault As MessageFault = faultException.CreateMessageFault()
    '    fault = Message.CreateMessage(version, messageFault, faultException.Action)


    'End Sub

    Public Sub ProvideFault(ByVal [error] As Exception, ByVal version As MessageVersion, ByRef fault As Message) Implements IErrorHandler.ProvideFault
        Dim httpResponse = WebOperationContext.Current.OutgoingResponse
        httpResponse.StatusCode = HttpStatusCode.OK
        Dim faultException = New FaultException([error].Message)
        Dim messageFault As MessageFault = faultException.CreateMessageFault()
        fault = Message.CreateMessage(version, messageFault, faultException.Action)
    End Sub
End Class
