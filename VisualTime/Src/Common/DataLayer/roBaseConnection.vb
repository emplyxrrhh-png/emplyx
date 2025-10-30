Imports System.Data.Common

'Imports System.IO

''' <summary>
''' Provides methods for common sql data access tasks.
''' </summary>
Public MustInherit Class roBaseConnection
    Implements IDisposable

    Private disposedValue As Boolean = False

    Protected oConnection As DbConnection = Nothing
    Protected oTransaction As DbTransaction = Nothing

    Protected bInit As Boolean = False
    Protected bClose As Boolean = False
    Protected bReadSource As Boolean = False

    Public MustOverride ReadOnly Property Connection As DbConnection

    Public MustOverride ReadOnly Property Transaction As DbTransaction
    Public Property Close As Boolean
        Get
            Return bClose
        End Get
        Set(value As Boolean)
            bClose = value
        End Set
    End Property

    Public Property IsReadSource As Boolean
        Get
            Return bReadSource
        End Get
        Set(value As Boolean)
            bReadSource = value
        End Set
    End Property

    Public ReadOnly Property IsInitialized As Boolean
        Get
            Return Me.bInit
        End Get
    End Property

    Protected Sub New(ByVal bFromReadSource As Boolean)
        bInit = False
        bReadSource = bFromReadSource
    End Sub

    Protected Sub New(ByVal oActiveConnection As DbConnection, ByVal oActiveTransaction As DbTransaction, Optional ByVal bInitTransactionByDefault As Boolean = False, Optional ByVal bFromReadSource As Boolean = False, Optional ByVal isolationLevel As IsolationLevel = IsolationLevel.ReadCommitted)
        Try
            bReadSource = bFromReadSource

            If oActiveTransaction IsNot Nothing Then
                Me.oTransaction = oActiveTransaction
                Me.oConnection = Me.oTransaction.Connection
            Else
                If oActiveConnection IsNot Nothing Then
                    Me.oTransaction = Nothing
                    Me.oConnection = oActiveConnection
                Else
                    If Not bInitTransactionByDefault Then
                        Me.oConnection = AccessHelper.CreateBaseConnection(bFromReadSource)
                    Else
                        Me.oTransaction = AccessHelper.CreateBaseTransaction(bFromReadSource, isolationLevel)
                        Me.oConnection = Me.oTransaction.Connection
                    End If

                End If
            End If

            Me.bInit = True
        Catch ex As ConnectionStringException
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roDebug, "roBaseConnection::ConnectionString::" & ex.Message, ex)
            Me.bInit = False
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::New::", ex)
            Me.bInit = False
        End Try
    End Sub

    Public Function IsOpen() As Boolean
        Try
            If Me.oTransaction IsNot Nothing AndAlso Me.oTransaction.Connection IsNot Nothing Then
                Return Me.oTransaction.Connection.State = ConnectionState.Open
            Else
                If Me.oConnection IsNot Nothing Then
                    Return Me.oConnection.State = ConnectionState.Open
                Else
                    Return False
                End If
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::IsOpen::", ex)
        End Try
    End Function

    Public Sub ForceClose()
        Try
            If Me.oTransaction IsNot Nothing Then
                If Me.oTransaction.Connection IsNot Nothing AndAlso Me.oTransaction.Connection.State = ConnectionState.Open Then
                    Me.oTransaction.Commit()
                End If
                Me.oTransaction.Dispose()
                Me.oTransaction = Nothing
            Else
                If Me.oConnection IsNot Nothing Then
                    If Me.oConnection.State = ConnectionState.Open Then
                        Me.oConnection.Close()
                        Me.oConnection.Dispose()
                    End If
                    Me.oConnection = Nothing
                End If
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::ForceClose::", ex)
        End Try
    End Sub

    Public Function CloseIfNeeded(Optional ByVal bCommitTransaction As Boolean = True) As Boolean

        Try
            If Me.Close Then
                If Me.oTransaction IsNot Nothing Then
                    If bCommitTransaction Then
                        Me.oTransaction.Commit()
                    Else
                        Me.oTransaction.Rollback()
                    End If

                    Me.oTransaction.Dispose()
                Else
                    If Me.oConnection IsNot Nothing Then
                        Me.oConnection.Close()
                        Me.oConnection.Dispose()
                    End If
                End If

            End If

            Return True
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::CloseIfNeeded::", ex)
            Return False
        End Try

    End Function

    Public Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
        GC.SuppressFinalize(Me)
    End Sub

    Protected Overridable Sub Dispose(disposing As Boolean)
        If Not Me.disposedValue Then
            If disposing Then
                ForceClose()
            End If
        End If
        Me.disposedValue = True
    End Sub

    Friend Shared Function InitConnectionForCompany(ByVal oActiveConnection As DbConnection, Optional bCreateTransactionByDefault As Boolean = False, Optional bFromReadSource As Boolean = False) As roBaseConnection

        Dim oBaseConnection As DataLayer.roBaseConnection = Nothing
        Try
            If Not bCreateTransactionByDefault Then
                oBaseConnection = New DataLayer.roConnection(oActiveConnection, bFromReadSource)
                oBaseConnection.Close = True
            Else
                oBaseConnection = New DataLayer.roTransaction(oActiveConnection, bFromReadSource)
                oBaseConnection.Close = True
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::Init from roBaseConnection::", ex)
            oBaseConnection = Nothing
        End Try

        Return oBaseConnection
    End Function

    Friend Shared Function Init(ByVal oActiveConnection As roBaseConnection, Optional bCreateTransactionByDefault As Boolean = False, Optional bFromReadSource As Boolean = False, Optional ByVal isolationLevel As System.Data.IsolationLevel = System.Data.IsolationLevel.Unspecified, Optional ByVal bForceInit As Boolean = True) As roBaseConnection

        Dim oBaseConnection As DataLayer.roBaseConnection = Nothing
        Try
            Dim tmpConnection = roCacheManager.GetInstance().GetConnection(bForceInit)

            If tmpConnection IsNot Nothing AndAlso tmpConnection.IsOpen Then
                oBaseConnection = tmpConnection
            Else
                If Not bCreateTransactionByDefault Then
                    oBaseConnection = New DataLayer.roConnection(bFromReadSource)
                    oBaseConnection.Close = True
                Else
                    oBaseConnection = New DataLayer.roTransaction(bFromReadSource, isolationLevel)
                    oBaseConnection.Close = True
                End If

                roCacheManager.GetInstance.UpdateConnection(oBaseConnection)
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::Init from roBaseConnection::", ex)
            oBaseConnection = Nothing
        End Try

        Return oBaseConnection
    End Function

    Public Shared Function ForceNewConnection(ByVal oActiveConnection As roBaseConnection, Optional bCreateTransactionByDefault As Boolean = False, Optional bFromReadSource As Boolean = False) As roBaseConnection
        Dim oBaseConnection As DataLayer.roBaseConnection = Nothing
        Try
            If Not bCreateTransactionByDefault Then
                oBaseConnection = New DataLayer.roConnection(bFromReadSource)
                oBaseConnection.Close = True
            Else
                oBaseConnection = New DataLayer.roTransaction(bFromReadSource)
                oBaseConnection.Close = True
            End If
        Catch ex As Exception
            Robotics.VTBase.roLog.GetInstance().logMessage(VTBase.roLog.EventType.roError, "roBaseConnection::Init from roBaseConnection::", ex)
            oBaseConnection = Nothing
        End Try

        Return oBaseConnection
    End Function

End Class