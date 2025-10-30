Imports System.Security.Cryptography.X509Certificates
Imports System.Web
Imports Robotics.Base.DTOs
Imports Robotics.VTBase

Public Class WebServiceHelper

    Private Shared oStateLock As New Object

    Public Shared Sub SetState(ByRef oState As Object, Optional ByVal _IDPassport As Integer = -1)
        ' Establecer el objecto de estado para las llamadas a los ws

        Dim statePassport As Integer
        If _IDPassport <= 0 Then
            statePassport = If(WLHelperWeb.CurrentPassportID > 0, WLHelperWeb.CurrentPassportID, -1)
        Else
            statePassport = _IDPassport
        End If


        roBaseState.SetSessionSmall(oState, statePassport, roConstants.GetDefaultSourceForType(roConstants.GetCurrentAppType()), HttpContext.Current.Session.SessionID)

        Try
            oState.Result = 0
        Catch ex As Exception

        End Try
    End Sub

    Public Shared Sub SetStateSmall(ByRef oState As Object, Optional ByVal _IDPassport As Integer = -1)
        SyncLock oStateLock
            roBaseState.SetSessionSmall(oState, _IDPassport, roConstants.GetDefaultSourceForType(roConstants.GetCurrentAppType()), HttpContext.Current.Session.SessionID)
        End SyncLock

    End Sub

    Public Shared Sub SetSSOVTPortalState(ByRef oState As Object, Optional ByVal _IDPassport As Integer = -1)
        SyncLock oStateLock
            roBaseState.SetSessionSmall(oState, _IDPassport, roAppSource.VTPortal, "")
        End SyncLock
    End Sub

End Class

Public NotInheritable Class SSLValidator

    Private Shared Function OnValidateCertificate(ByVal sender As Object, ByVal certificate As X509Certificate, ByVal chain As X509Chain, ByVal sslPolicyErrors As Net.Security.SslPolicyErrors) As Boolean
        Return True
    End Function

    Public Shared Sub OverrideValidation()
        Net.ServicePointManager.ServerCertificateValidationCallback = AddressOf OnValidateCertificate
        Net.ServicePointManager.Expect100Continue = True
    End Sub

End Class