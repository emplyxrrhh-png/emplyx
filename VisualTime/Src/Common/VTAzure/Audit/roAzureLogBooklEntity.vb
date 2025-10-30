Imports Azure
Imports Azure.Data.Tables

Public Class roAzureLogBooklEntity
    Implements ITableEntity

    Public Property LogInfo As String

    Public Property PartitionKey As String Implements ITableEntity.PartitionKey
    Public Property RowKey As String Implements ITableEntity.RowKey
    Public Property Timestamp As DateTimeOffset? Implements ITableEntity.Timestamp
    Public Property ETag As ETag Implements ITableEntity.ETag

    Public Sub New()

    End Sub

    Public Sub New(_complaintId As String, _logInfo As String)
        Me.LogInfo = _logInfo
        Me.PartitionKey = _complaintId
        Me.RowKey = Guid.NewGuid.ToString
    End Sub

End Class