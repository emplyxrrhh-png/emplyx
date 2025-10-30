''' <summary>
''' Represents ways of checking permissions.
''' </summary>
Public Enum PermissionCheckMode
    ''' <summary>
    ''' Checks permissions directly defined on passport or inherited.
    ''' </summary>
    Normal = 0
    ''' <summary>
    ''' Only checks permissions directly defined on passport.
    ''' </summary>
    DirectOnly = 1
    ''' <summary>
    ''' Only checks inherited permissions.
    ''' </summary>
    InheritedOnly = 2
End Enum