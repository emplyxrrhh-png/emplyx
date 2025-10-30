Imports System.Web
Imports Robotics.VTBase

Public Class roWsUserManagement
    Public Const EXPIRATION As Integer = 30 * 60
    Private Const GUID_SESSION_IDENTIFIER As String = "WLPASSPORT_GUID"
    ' 30 * 60
    Private Shared ht As New Hashtable()

    Private Shared ReadOnly Property CurrentPassportGUID() As String
        Get
            Dim oPassportGUID As String = String.Empty
            Try
                oPassportGUID = roTypes.Any2String(HttpContext.Current.Session(GUID_SESSION_IDENTIFIER))
            Catch ex As Exception
                oPassportGUID = String.Empty
            End Try

            If oPassportGUID = String.Empty Then
                oPassportGUID = Guid.NewGuid.ToString()
                HttpContext.Current.Session(GUID_SESSION_IDENTIFIER) = oPassportGUID
            End If

            Return oPassportGUID
        End Get
    End Property

    Public Shared Property SessionObject As roWsUserObject
        Get
            Dim rObject As roWsUserObject = CheckSession()
            If rObject Is Nothing Then rObject = CreateSession()

            Return rObject
        End Get
        Set(value As roWsUserObject)
            UpdateSession(value)
        End Set
    End Property

    Public Shared Function ResetSessionObject() As roWsUserObject
        Dim rObject As roWsUserObject = CheckSession()
        If rObject Is Nothing Then
            rObject = CreateSession()
        Else
            rObject.States = Nothing
            rObject.AccessApi = Nothing
            rObject.AccessApi = New roBackend(rObject.PassportGUID)
            rObject.States = New roWebServicesStates()

            UpdateSession(rObject)
        End If

        Return rObject
    End Function

    Public Shared Function RemoveCurrentsession() As Boolean
        SyncLock GetType(roWsUserManagement)
            If Not ht.Contains(roWsUserManagement.CurrentPassportGUID) Then
                Return False
            Else
                DeleteSession()
                Return True
            End If
        End SyncLock
    End Function

    Private Shared Function CreateSession() As roWsUserObject
        SyncLock GetType(roWsUserManagement)
            Dim oSession As New roWsUserObject(DateTime.Now, roWsUserManagement.CurrentPassportGUID)
            ht.Add(roWsUserManagement.CurrentPassportGUID, oSession)
            Return oSession
        End SyncLock
    End Function

    Private Shared Function CheckSession(Optional ByVal updateTimeStamp As Boolean = True) As roWsUserObject
        SyncLock GetType(roWsUserManagement)
            ' Primero purgamos sesiones caducadas
            PurgeOldSessions()

            ' Si no está el guid, ha caducado
            If Not ht.Contains(roWsUserManagement.CurrentPassportGUID) Then
                Return Nothing
            End If
            ' Actualizamos el timestamp
            If updateTimeStamp Then
                DirectCast(ht(roWsUserManagement.CurrentPassportGUID), roWsUserObject).LastAccess = DateTime.Now
            End If

            Dim sObject As roWsUserObject = DirectCast(ht(roWsUserManagement.CurrentPassportGUID), roWsUserObject)
            Return sObject
        End SyncLock
    End Function

    Private Shared Function UpdateSession(ByVal session As roWsUserObject) As Boolean
        SyncLock GetType(roWsUserManagement)
            ' Primero purgamos sesiones caducadas
            PurgeOldSessions()
            ' Si no está el guid, ha caducado
            If Not ht.Contains(roWsUserManagement.CurrentPassportGUID) Then
                Return False
            End If

            ht(roWsUserManagement.CurrentPassportGUID) = session
            Return True
        End SyncLock
    End Function

    Private Shared Sub DeleteSession()
        SyncLock GetType(roWsUserManagement)
            ht.Remove(roWsUserManagement.CurrentPassportGUID)
        End SyncLock
    End Sub

    Private Shared Sub PurgeOldSessions()
        SyncLock GetType(roWsUserManagement)
            Dim [rem] As New ArrayList()
            Dim now As DateTime = DateTime.Now
            Dim iter As IDictionaryEnumerator = ht.GetEnumerator()

            While iter.MoveNext()
                Dim sObj As roWsUserObject = DirectCast(iter.Value, roWsUserObject)
                If (now - sObj.LastAccess).TotalSeconds > EXPIRATION Then
                    [rem].Add(iter.Key)
                End If
            End While

            For i As Integer = 0 To [rem].Count - 1
                ht.Remove([rem](i))
            Next
        End SyncLock
    End Sub

End Class