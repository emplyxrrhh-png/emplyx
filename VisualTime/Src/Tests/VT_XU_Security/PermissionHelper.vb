Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Security

Public Class PermissionHelper

    Public Property DeleteCommuniqueeGroupCalled As Boolean


    Public Sub HasPermissionFake(hasPermission As Boolean)

        Robotics.Security.Fakes.ShimWLHelper.HasPermissionOverFeatureInt32StringStringPermission = Function(ByVal passportId As Integer, ByVal featureName As String, ByVal featureValue As String, ByVal permissionType As Robotics.Base.DTOs.Permission) As Boolean
                                                                                                       Return hasPermission
                                                                                                   End Function

        Robotics.Security.Fakes.ShimWLHelper.GetPermissionOverFeatureInt32StringString = Function(ByVal passportId As Integer, ByVal featureAlias As String, ByVal featureType As String) As Permission
                                                                                             If hasPermission Then
                                                                                                 Return Permission.Admin
                                                                                             End If
                                                                                             Return Permission.None
                                                                                         End Function


    End Sub

End Class