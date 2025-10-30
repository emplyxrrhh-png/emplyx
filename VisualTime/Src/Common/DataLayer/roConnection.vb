Imports System.Data.Common

'Imports System.IO

''' <summary>
''' Provides methods for common sql data access tasks.
''' </summary>
Public Class roConnection
    Inherits roBaseConnection

    Friend Sub New(ByVal bFromReadSource As Boolean)
        MyBase.New(Nothing, Nothing, False, bFromReadSource)
    End Sub

    Friend Sub New(ByVal oActiveConnection As DbConnection, ByVal bFromReadSource As Boolean)
        MyBase.New(oActiveConnection, Nothing, False, bFromReadSource)
    End Sub

    Public Overrides ReadOnly Property Connection As DbConnection
        Get
            If Not Me.bInit OrElse Me.oConnection Is Nothing Then
                Throw New ConnectionStringException("roConnection::Connection not initialized")
            End If

            Return oConnection
        End Get
    End Property

    Public Overrides ReadOnly Property Transaction As DbTransaction
        Get
            Return Nothing
        End Get
    End Property

    Public Function CloseConnection() As Boolean
        Try
            Return Me.CloseIfNeeded(True)
        Catch ex As Exception
            Return False
        End Try
    End Function

End Class