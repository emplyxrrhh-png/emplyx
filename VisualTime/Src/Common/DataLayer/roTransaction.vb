Imports System.Data.Common

'Imports System.IO

''' <summary>
''' Provides methods for common sql data access tasks.
''' </summary>
Public Class roTransaction
    Inherits roBaseConnection

    Friend Sub New(ByVal bFromReadSource As Boolean, Optional ByVal isolationLevel As IsolationLevel = IsolationLevel.ReadCommitted)
        MyBase.New(Nothing, Nothing, True, bFromReadSource, isolationLevel)
    End Sub

    Friend Sub New(ByVal oActiveTransaction As DbTransaction, ByVal bFromReadSource As Boolean, Optional ByVal isolationLevel As IsolationLevel = IsolationLevel.ReadCommitted)
        MyBase.New(Nothing, oActiveTransaction, bFromReadSource)
    End Sub

    Friend Sub New(ByVal oActiveConnection As DbConnection, ByVal bFromReadSource As Boolean, Optional ByVal isolationLevel As IsolationLevel = IsolationLevel.ReadCommitted)
        MyBase.New(Nothing, oActiveConnection.BeginTransaction(isolationLevel), False, bFromReadSource)
    End Sub

    Public Overrides ReadOnly Property Connection As DbConnection
        Get
            If Not Me.bInit OrElse Me.oTransaction Is Nothing Then
                Throw New ConnectionStringException("roTransaction::Connection not initialized")
            End If

            Return oTransaction.Connection
        End Get
    End Property

    Public Overrides ReadOnly Property Transaction As DbTransaction
        Get
            If Not Me.bInit OrElse Me.oTransaction Is Nothing Then
                Throw New ConnectionStringException("roTransaction::Connection not initialized")
            End If

            Return oTransaction
        End Get
    End Property

    Public Function CommitAndClose() As Boolean
        Try
            Return Me.CloseIfNeeded(True)
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Function RollbackAndClose() As Boolean
        Try
            Return Me.CloseIfNeeded(False)
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class