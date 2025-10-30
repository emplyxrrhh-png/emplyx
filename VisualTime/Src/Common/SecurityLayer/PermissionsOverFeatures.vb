Imports Robotics.Base.DTOs
Imports Robotics.Security.DataAccess

''' <summary>
''' Reads and modifies permissions over features for a given passport.
''' </summary>
<Serializable()>
Public Class PermissionsOverFeatures
    Private _passport As roPassport

    ''' <summary>
    ''' Initializes a new instance of the PermissionsOverFeatures class.
    ''' </summary>
    ''' <param name="passport">The passport for which to read or modify permissions.</param>
    Public Sub New(ByVal passport As roPassport)
        _passport = passport
    End Sub

    ''' <summary>
    ''' Returns the permission current passport have over specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    Public Function [GetSys](ByVal featureAlias As String, ByVal featureType As String) As Permission
        Return CType(PermissionsAccess.PermissionsOverFeatures_Get_Sys(_passport.ID, featureAlias, featureType, PermissionCheckMode.Normal), Permission)
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="mode">Wether to return normal, direct or inherited permissions.</param>
    Public Function [GetSys](ByVal featureAlias As String, ByVal featureType As String, ByVal mode As PermissionCheckMode) As Permission
        Return CType(PermissionsAccess.PermissionsOverFeatures_Get_Sys(_passport.ID, featureAlias, featureType, mode), Permission)
    End Function

    ''' <summary>
    ''' Removes permissions set on specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to remove permissions on.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    Public Sub Remove(ByVal featureAlias As String, ByVal featureType As String, Optional ByVal bolInitTask As Boolean = True)
        PermissionsAccess.PermissionsOverFeatures_Remove(_passport.ID, featureAlias, featureType, bolInitTask)
    End Sub

    ''' <summary>
    ''' Sets permissions on specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature for which to set permissions.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="permission">The permission to set.</param>
    ''' <param name="trans">The transaction under which to perform data access.</param>
    Public Sub [Set](ByVal featureAlias As String, ByVal featureType As String, ByVal permission As Permission, Optional ByVal bolInitTask As Boolean = True)
        PermissionsAccess.PermissionsOverFeatures_Set(_passport.ID, featureAlias, featureType, permission, bolInitTask)
    End Sub

    ''' <summary>
    ''' Returns the highest value that could be set over a feature for specified passport.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <remarks>The return value is the inherited permission,
    ''' or Admin if there is no parent.</remarks>
    Public Function MaxConfigurable(ByVal featureAlias As String, ByVal featureType As String) As Permission
        Return CType(PermissionsAccess.PermissionsOverFeatures_MaxConfigurable(_passport.ID, featureAlias, featureType), Permission)
    End Function

End Class