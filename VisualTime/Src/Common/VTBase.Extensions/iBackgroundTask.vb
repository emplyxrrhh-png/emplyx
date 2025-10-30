Imports System.ServiceModel

Namespace VTLiveTasks

    <ServiceContract>
    Public Interface IBackgroundMessage

        <OperationContract>
        Function ExecuteTask(ByVal oContext As String) As Boolean

        <OperationContract>
        Function TaskFinished(ByVal idTask As Integer) As Boolean

        <OperationContract>
        Function DeleteTask(ByVal oContext As String) As Boolean

        <OperationContract>
        Function NumberOfThreads() As Integer

    End Interface

End Namespace