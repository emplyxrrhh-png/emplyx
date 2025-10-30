Imports System.Collections.ObjectModel
Imports System.ServiceModel
Imports System.ServiceModel.Channels
Imports System.ServiceModel.Description
Imports System.ServiceModel.Dispatcher

Public Class CustomErrorBehaviorAttribute
    Inherits Attribute
    Implements IServiceBehavior

    Public Sub AddBindingParameters(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase, ByVal endpoints As Collection(Of ServiceEndpoint), ByVal bindingParameters As BindingParameterCollection) Implements IServiceBehavior.AddBindingParameters
        'No implementado
    End Sub

    Public Sub ApplyDispatchBehavior(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase) Implements IServiceBehavior.ApplyDispatchBehavior
        For Each dispatcher As ChannelDispatcher In serviceHostBase.ChannelDispatchers
            dispatcher.ErrorHandlers.Add(New CustomErrorHandler())
        Next
    End Sub

    Public Sub Validate(ByVal serviceDescription As ServiceDescription, ByVal serviceHostBase As ServiceHostBase) Implements IServiceBehavior.Validate
        'No implementado
    End Sub


End Class
