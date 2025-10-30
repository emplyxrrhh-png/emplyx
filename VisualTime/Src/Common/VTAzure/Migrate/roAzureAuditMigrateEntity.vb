Imports Azure
Imports Azure.Data.Tables

Public Class roAzureAuditMigrateEntity
    Implements ITableEntity

    Public Property CompanyId As String
    Public Property [Date] As DateTime
    Public Property Content As String
    Public Property Migrated As Boolean

    Public Property PartitionKey As String Implements ITableEntity.PartitionKey
    Public Property RowKey As String Implements ITableEntity.RowKey
    Public Property Timestamp As DateTimeOffset? Implements ITableEntity.Timestamp
    Public Property ETag As ETag Implements ITableEntity.ETag

    Public Sub New()

    End Sub

    Public Sub New(_CompanyId As String, _date As DateTime, _content As String, _migrated As Boolean)
        Me.CompanyId = _CompanyId
        Me.Content = _content
        Me.Date = _date.ToUniversalTime()
        Me.Migrated = _migrated

        Me.PartitionKey = _CompanyId
        Me.RowKey = Guid.NewGuid.ToString
    End Sub

    'Public Sub ReadEntity(properties As IDictionary(Of String, EntityProperty), operationContext As OperationContext) Implements ITableEntity.ReadEntity
    '    Me.ReadEntityHelper(Me, properties, operationContext)
    'End Sub

    'Public Function WriteEntity(operationContext As OperationContext) As IDictionary(Of String, EntityProperty) Implements ITableEntity.WriteEntity
    '    Return Me.WriteEntityHelper(Me, operationContext)
    'End Function

End Class