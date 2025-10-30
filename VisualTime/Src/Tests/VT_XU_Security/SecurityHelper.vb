Imports Robotics.Base.DTOs
Imports Robotics.Security
Imports Robotics.Security.Base

Public Class SecurityHelper

    Public Property CredentialUsed As String = ""
    Public Property AuthenticateCount As Integer = 0
    Public Property AuthenticationADCount As Integer = 0

    Public Sub AuthenticateSpy()

        Robotics.Security.Base.Fakes.ShimAuthHelper.AuthenticateAuthenticationMethodStringStringBooleanString =
                    Function(method As AuthenticationMethod, ByVal username As String, ByVal password As String, ByVal hash As Boolean, ByVal version As String)
                        Dim oPassport As roPassportTicket = Nothing
                        CredentialUsed = username
                        AuthenticateCount += 1

                        If method = AuthenticationMethod.Password AndAlso username = "username" AndAlso password = "password" Then
                            oPassport = New roPassportTicket
                        ElseIf method = AuthenticationMethod.Biometry AndAlso username = "finger1" AndAlso password = "fingerbiometry" Then
                            oPassport = New roPassportTicket
                        End If

                        Return oPassport
                    End Function
    End Sub

    Public Sub AuthenticateADStub()

        Robotics.Security.Base.Fakes.ShimAuthenticateAD.AllInstances.AuthenticateByActiveDirectoryroSecurityStateRefStringStringString =
                    Function(oAuth As AuthenticateAD, ByRef oState As roSecurityState, Domain As String, ByVal UserName As String, ByVal Password As String)
                        Dim loggedIn As Boolean

                        AuthenticationADCount += 1

                        Return True
                    End Function
    End Sub

    Public Sub GetSupervisorPermissionOverFeature(idPassport As Integer, ByVal strFeature As String, idFeature As Integer, permission As Integer)
        Robotics.Security.Fakes.ShimWLHelper.GetPermissionOverFeatureInt32StringString =
                        Function(passport As Integer, feature As String, featureType As String)

                            If idPassport = passport AndAlso strFeature = feature Then
                                Return permission
                            Else
                                Return 0
                            End If
                        End Function

    End Sub

    Public Sub GetEmployeeList()
        Robotics.Security.Fakes.ShimroSelector.GetEmployeeListInt32StringStringNullableOfPermissionStringStringStringBooleanNullableOfDateTimeNullableOfDateTime =
                    Function(idPassport As Integer, strFeature As String, strFeatureType As String, perm As Permission, selector As String, strPermission As String, strPermissionType As String, bolIncludeInactive As Boolean, dtStart As Date?, dtEnd As Date?)
                        Dim lstEmployees As New List(Of Integer)
                        lstEmployees.Add(1)
                        Return lstEmployees
                    End Function
    End Sub

    Public Sub GetEmployeePermissions(ByVal requestType As Integer)
        Robotics.Base.VTPortal.VTPortal.Fakes.ShimSecurityHelper.GetEmployeePermissionsroPassportTicketroTerminalroRequestState =
                    Function(oPassport, oTerminal, oReqState)
                        Dim oPermList As New PermissionList
                        oPermList.CanCreateRequests = True
                        Dim requestsPermissions As ReqPermission = New ReqPermission()
                        requestsPermissions.RequestType = requestType
                        requestsPermissions.Permission = 1
                        oPermList.Requests = {requestsPermissions}
                        Return oPermList
                    End Function
    End Sub

End Class