Imports System.Runtime.Serialization
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum PasswordSecurityLevelEnum
        <EnumMember> _LOW = 0         ' Bajo
        <EnumMember> _MEDIUM = 1      ' Medio
        <EnumMember> _HIGH = 2        '  Alto
    End Enum

    <DataContract>
    Public Enum PasswordLevelError
        <EnumMember> No_Error = 0
        <EnumMember> Low_Error = 1
        <EnumMember> Medium_Error = 2
        <EnumMember> High_Error = 3
        <EnumMember> No_Passport_Error = 4
    End Enum

    <DataContract>
    Public Enum ContextPeriodTypes
        <EnumMember> Today
        <EnumMember> Yesterday
        <EnumMember> CurrentWeek
        <EnumMember> LastWeek
        <EnumMember> CurrentMonth
        <EnumMember> LastMonth
        <EnumMember> CurrentYear
        <EnumMember> Other
    End Enum

    <DataContract()>
    Public Enum LoadType
        <EnumMember> Passport
        <EnumMember> Employee
        <EnumMember> User
    End Enum

    <DataContract()>
    Public Class roPassport

        Public Sub New()
            EnabledVTDesktop = True
            EnabledVTPortal = True
            EnabledVTPortalApp = True
            EnabledVTVisits = True
            PhotoRequiered = True
            LocationRequiered = True
            LoginWithoutContract = False
            AuthenticationMethods = New roPassportAuthenticationMethods
            IDUser = Nothing
            IDEmployee = Nothing
            IDParentPassport = Nothing
            StartDate = Nothing
            ExpirationDate = Nothing
            State = Nothing
            Description = String.Empty
            Name = String.Empty
            Email = String.Empty
            GroupType = String.Empty
            ConfData = String.Empty
        End Sub

        <DataMember()>
        Public Property ID As Integer

        <DataMember()>
        Public Property IDParentPassport As Nullable(Of Integer)

        <DataMember()>
        Public Property GroupType As String

        <DataMember()>
        Public Property Name As String

        <DataMember()>
        Public Property Description As String

        <DataMember()>
        Public Property Email As String

        <DataMember()>
        Public Property IDUser As Nullable(Of Integer)

        <DataMember()>
        Public Property IDEmployee As Nullable(Of Integer)

        <DataMember()>
        Public Property Language As roPassportLanguage

        <DataMember()>
        Public Property LevelOfAuthority As Nullable(Of Byte)

        <IgnoreDataMember()>
        Public Property ConfData As String

        <DataMember()>
        Public Property EnabledVTDesktop As Boolean

        <DataMember()>
        Public Property EnabledVTPortal As Boolean

        <DataMember()>
        Public Property EnabledVTPortalApp As Boolean

        <DataMember()>
        Public Property EnabledVTVisits As Boolean

        <DataMember()>
        Public Property EnabledVTVisitsApp As Boolean

        <DataMember()>
        Public Property PhotoRequiered As Boolean

        <DataMember()>
        Public Property LoginWithoutContract As Boolean

        <DataMember()>
        Public Property LocationRequiered As Boolean

        <DataMember()>
        Public Property LicenseAccepted As Boolean

        <DataMember()>
        Public Property IsSupervisor As Boolean

        <DataMember()>
        Public ReadOnly Property IsPrivateUser As Boolean
            Get
                Return Me.Description.Contains("@@ROBOTICS@@")
            End Get
        End Property

        <DataMember>
        Public Property StartDate As Nullable(Of Date)

        <DataMember>
        Public Property ExpirationDate As Nullable(Of Date)

        ''' <summary>
        ''' Gets or sets the state of passport
        ''' </summary>
        ''' <remarks>1- Valid, 0- Invalid </remarks>
        <DataMember>
        Public Property State As Nullable(Of Integer)

        ''' <summary>
        ''' Returns wether this passport is a user.
        ''' </summary>
        <DataMember>
        Public Property IsUser As Boolean

        ''' <summary>
        ''' Returns wether this passport is an employee.
        ''' </summary>
        <DataMember>
        Public Property IsEmployee As Boolean

        ''' <summary>
        ''' Returns wether this passport is a users group.
        ''' </summary>
        <DataMember>
        Public Property IsUsersGroup As Boolean

        ''' <summary>
        ''' Returns wether this passport is an employee group.
        ''' </summary>
        <DataMember>
        Public Property IsEmployeesGroup As Boolean

        ''' <summary>
        ''' Returns wether this passport is active.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember>
        Public Property IsActivePassport As Boolean

        ''' <summary>
        ''' Returns wether this passport is BloquedAccessApp.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember>
        Public Property IsBloquedAccessApp As Boolean

        ''' <summary>
        ''' Returns wether this passport is IsTemporayBloqued.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember>
        Public Property IsTemporayBloqued As Boolean

        ''' <summary>
        ''' Returns wether this passport is IsExpiredPwd.
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        <DataMember>
        Public Property IsExpiredPwd As Boolean

        ''' <summary>
        ''' Gets or sets an object containing passport authentication methods
        ''' </summary>
        ''' <value></value>
        ''' <returns></returns>
        ''' <remarks></remarks>
        '''
        <DataMember>
        Public Property AuthenticationMethods As roPassportAuthenticationMethods

        <DataMember()>
        Public Property CanApproveOwnRequests As Boolean

        <DataMember()>
        Public Property IDGroupFeature As Integer

        <DataMember>
        Public Property Categories As roPassportCategories

        <DataMember>
        Public Property Groups As roPassportGroups

        <DataMember>
        Public Property Exceptions As roPassportExceptions

    End Class

    <DataContract>
    Public Class roPassportAuthenticationMethods

        Public Sub New()
            PasswordRow = Nothing
            IntegratedSecurityRow = Nothing
            PinRow = Nothing
            CardRows = {}
            BiometricRows = {}
            PlateRows = {}
            NFCRow = Nothing
        End Sub

        '<DataMember>
        'Public Property idPassport As Integer

        <DataMember>
        Public Property PasswordRow As roPassportAuthenticationMethodsRow

        <DataMember>
        Public Property IntegratedSecurityRow As roPassportAuthenticationMethodsRow

        <DataMember>
        Public Property CardRows As roPassportAuthenticationMethodsRow()

        <DataMember>
        Public Property BiometricRows As roPassportAuthenticationMethodsRow()

        <DataMember>
        Public Property PinRow As roPassportAuthenticationMethodsRow

        <DataMember>
        Public Property PlateRows As roPassportAuthenticationMethodsRow()

        <DataMember>
        Public Property NFCRow As roPassportAuthenticationMethodsRow


    End Class

    <DataContract>
    Public Class roPassportAuthenticationMethodsRow

        <DataMember>
        Public Property IDPassport As Integer

        <DataMember>
        Public Property Method As AuthenticationMethod

        <DataMember>
        Public Property Version As String

        <DataMember>
        Public Property Credential As String

        <DataMember>
        Public Property Password() As String

        <DataMember>
        Public Property StartDate() As Date

        <DataMember>
        Public Property ExpirationDate() As Date

        <DataMember>
        Public Property BiometricID() As Integer

        <DataMember>
        Public Property BiometricData() As Byte()

        <DataMember>
        Public Property BiometricAlgorithm() As String

        <DataMember>
        Public Property BiometricTerminalId() As Integer

        <DataMember>
        Public Property TimeStamp() As Date

        <DataMember>
        Public Property Enabled() As Boolean

        <DataMember>
        Public Property LastUpdatePassword() As Date

        <DataMember>
        Public Property BloquedAccessApp() As Boolean

        <DataMember>
        Public Property InvalidAccessAttemps() As Integer

        <DataMember>
        Public Property LastDateInvalidAccessAttempted() As Date

        <DataMember>
        Public Property LastAppActionDate() As Date

        <DataMember>
        Public Property BlockedAccessByInactivity() As Boolean

        <DataMember>
        Public Property RowState() As RowState

    End Class

    <DataContract>
    Public Class roPassportLanguage

        <DataMember>
        Public Property ID As Integer

        <DataMember>
        Public Property Key As String

        <DataMember>
        Public Property Culture As String

        <DataMember>
        Public Property ParametersXml As String

        <DataMember>
        Public Property Installed As Boolean

    End Class

    <DataContract>
    Public Class roPassportWithPhoto

        <DataMember>
        Public Property IdPassport As Integer

        <DataMember>
        Public Property IdEmployee As Integer

        <DataMember>
        Public Property EmployeeName As String

        <DataMember>
        Public Property PassportName As String

        <DataMember>
        Public Property EmployeePhoto As String

        Public Sub New()
            IdPassport = -1
            IdEmployee = -1
            EmployeeName = ""
            PassportName = ""
            EmployeePhoto = ""

        End Sub

    End Class

    <DataContract>
    Public Class roPassportCategories

        <DataMember>
        Public Property idPassport As Integer

        <DataMember>
        Public Property CategoryRows As roPassportCategoryRow()

    End Class

    <DataContract>
    Public Class roPassportCategoryRow

        <DataMember>
        Public Property IDPassport As Integer

        <DataMember>
        Public Property IDCategory As CategoryType

        <DataMember>
        Public Property LevelOfAuthority As Byte

        <DataMember>
        Public Property ShowFromLevel As Byte

        <DataMember>
        Public Property RowState() As RowState

    End Class

    Public Enum RowState
        <EnumMember()> NoChangeRow
        <EnumMember()> NewRow
        <EnumMember()> UpdateRow
        <EnumMember()> DeleteRow
    End Enum

    <DataContract>
    Public Class roPassportGroups

        <DataMember>
        Public Property idPassport As Integer

        <DataMember>
        Public Property GroupRows As roPassportGroupRow()

    End Class

    <DataContract>
    Public Class roPassportGroupRow

        <DataMember>
        Public Property IDPassport As Integer

        <DataMember>
        Public Property IDGroup As Integer

        <DataMember>
        Public Property RowState() As RowState

    End Class

    <DataContract>
    Public Class roPassportExceptions

        <DataMember>
        Public Property idPassport As Integer

        <DataMember>
        Public Property Exceptions As roPassportExceptionRow()

    End Class

    <DataContract>
    Public Class roPassportExceptionRow

        <DataMember>
        Public Property IDPassport As Integer

        <DataMember>
        Public Property IDEmployee As Integer

        <DataMember>
        Public Property Name As String

        <DataMember>
        Public Property Available As Boolean

    End Class

End Namespace