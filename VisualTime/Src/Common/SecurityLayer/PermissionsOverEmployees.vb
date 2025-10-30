Imports Robotics.Base.DTOs
Imports Robotics.Security.DataAccess

''' <summary>
''' Reads and modifies permissions over employees for a given passport.
''' </summary>
<Serializable()>
Public Class PermissionsOverEmployees
    Private _passport As roPassport

    ''' <summary>
    ''' Initializes a new instance of the PermissionsOverEmployees class.
    ''' </summary>
    ''' <param name="passport">The passport for which to read or modify permissions.</param>
    Public Sub New(ByVal passport As roPassport)
        _passport = passport
    End Sub

    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    Public Function [Get](ByVal idEmployee As Integer, ByVal idApplication As Integer) As Permission
        Return CType(PermissionsAccess.PermissionsOverEmployees_Get(_passport.ID, idEmployee, idApplication, PermissionCheckMode.Normal, True), Permission)
    End Function


End Class