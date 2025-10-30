Imports System.Runtime.Serialization

Namespace DTOs

    <DataContract>
    Public Class LoggedInUserInfo
        ''' <summary>
        ''' Token de seguridad proporcionado si el inicio de sesión es correcto
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Token As [String]

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property UserId As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeId As Integer

        ''' <summary>
        ''' Indica si el empleado ha leido y aceptado la licencia de uso del programa
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LicenseAccepted As Boolean

        ''' <summary>
        ''' Consentimiento aceptado por el empleado para acceder al portal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Consent As roPassportConsent

        ''' <summary>
        ''' Indica si el servidor posee licendia de Anywhere para aplicar fotografia y geolocalización en los fichajes.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AnywhereLicense As Boolean

        ''' <summary>
        ''' Indica si el servidor requiere que la geolocalización sea fina o no.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequiereFineLocation As Boolean

        ''' <summary>
        ''' Indica si el empleado tiene acceso al módulo de declaración de jornada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DailyRecordEnabled As Boolean

        <DataMember()>
        Public Property DailyRecordPatternEnabled As Boolean

        ''' <summary>
        ''' Nivel de seguridad requerido para la complejidad de la contraseña en caso de requerirse el cambio
        '''         Normal = 0
        '''         Alto = 1
        '''         Muy alto = 2
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SecurityLevel As Integer

        ''' <summary>
        ''' Versión de la API de comunicaciones del portal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ApiVersion As Integer

        ''' <summary>
        ''' Credencial del usuario
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property UserName As String

        ''' <summary>
        ''' Idioma del usuario logeado
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property Language As String

        ''' <summary>
        ''' Estado de la petición de login en el VisualTime
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property Status As Long

        ''' <summary>
        ''' Indica si el empleado tiene acceso al portal del supervisor.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SupervisorPortalEnabled As Boolean

        ''' <summary>
        ''' Indica si se deben mostrar las secciones en las que no disponemos de permisos
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShowForbiddenSections As Boolean

        ''' <summary>
        ''' Indica si se debe mostrar el boton para cerrar sesión en la pantalla inicial
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShowLogoutHome As Boolean

        ''' <summary>
        ''' Indica si se se está conectando por AD
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IsAD As String
        <DataMember()>
        Public Property DefaultVersion As String
        <DataMember>
        Public Property ShowPermissionsIcon As Boolean
        ''' <summary>
        ''' Token de seguridad proporcionado si el inicio de sesión es correcto
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ServerTimezone As [String]

        <DataMember()>
        Public Property SSOServerEnabled As Boolean
        <DataMember()>
        Public Property IsSaas As Boolean
        <DataMember()>
        Public Property SSOUserLoggedIn As Boolean
        <DataMember()>
        Public Property SSOUserName As String
        <DataMember()>
        Public Property HeaderMD5 As String
        <DataMember()>
        Public Property IsCegidHub As Boolean
        <DataMember()>
        Public Property IsCommuniquesEnabled As Boolean
        <DataMember()>
        Public Property IsChannelsEnabled As Boolean
        ''' <summary>
        ''' Indica si el supervisor tiene permisos para impersonar
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IsImpersonateEnabled As Boolean
        <DataMember()>
        Public Property IsLatamMex As Boolean
    End Class

    <DataContract>
    Public Class roTokenResponse

        Public Sub New()
            Result = False
            Status = False
            oState = New roWsState()
        End Sub

        <DataMember>
        Public Property Result As Boolean
        <DataMember>
        Public Property Status As Boolean
        <DataMember>
        Public Property oState As roWsState
    End Class

    ''' <summary>
    ''' Resultado de la petición de inicio de sesión en VisualTime
    ''' </summary>
    <DataContract>
    Public Class LoginResult
        Public Sub New()
            Me.LastLogin = New Date(1970, 1, 1, 0, 0, 0, DateTimeKind.Local)
            Me.SSOServerEnabled = False
            Me.SSOUserLoggedIn = False
            Me.SSOUserName = ""
        End Sub

        ''' <summary>
        ''' Token de seguridad proporcionado si el inicio de sesión es correcto
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Token As [String]

        ''' <summary>
        ''' Identificador de passaporte
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property UserId As Integer

        ''' <summary>
        ''' Identificador de empleado
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property EmployeeId As Integer

        ''' <summary>
        ''' Indica si el servidor posee licendia de Anywhere para aplicar fotografia y geolocalización en los fichajes.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property AnywhereLicense As Boolean

        ''' <summary>
        ''' Indica si el empleado tiene acceso al portal del supervisor.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SupervisorPortalEnabled As Boolean

        ''' <summary>
        ''' Indica si el empleado tiene acceso al módulo de declaración de jornada
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property DailyRecordEnabled As Boolean

        <DataMember()>
        Public Property DailyRecordPatternEnabled As Boolean

        <DataMember()>
        Public Property ShowPermissionsIcon As Boolean
        ''' <summary>
        ''' Indica si el servidor requiere que la geolocalización sea fina o no.
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property RequiereFineLocation As Boolean

        ''' <summary>
        ''' Nivel de seguridad requerido para la complejidad de la contraseña en caso de requerirse el cambio
        '''         Normal = 0
        '''         Alto = 1
        '''         Muy alto = 2
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SecurityLevel As Integer

        ''' <summary>
        ''' Versión de la API de comunicaciones del portal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ApiVersion As Integer

        ''' <summary>
        ''' Indica si el empleado ha leido y aceptado la licencia de uso del programa
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property LicenseAccepted As Boolean

        ''' <summary>
        ''' Consentimiento aceptado por el empleado para acceder al portal
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Consent As roPassportConsent

        ''' <summary>
        ''' Indica si se deben mostrar las secciones en las que no disponemos de permisos
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShowForbiddenSections As Boolean

        ''' <summary>
        ''' Indica si se debe mostrar el boton para cerrar sesión en la pantalla inicial
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property ShowLogoutHome As Boolean
        ''' <summary>
        ''' Token de seguridad proporcionado si el inicio de sesión es correcto
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ServerTimezone As [String]

        ''' <summary>
        ''' Indica si se se está conectando por AD
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property IsAD As String
        ''' <summary>
        ''' Estado de la petición de login en el VisualTime
        ''' </summary>
        ''' <returns> 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes</returns>
        <DataMember>
        Public Property Status As Long
        <DataMember>
        Public Property DefaultLanguage As String

        <DataMember>
        Public Property DefaultVersion As String

        <DataMember()>
        Public Property SSOServerEnabled As Boolean
        <DataMember()>
        Public Property IsSaas As Boolean
        <DataMember()>
        Public Property LastLogin As DateTime
        <DataMember()>
        Public Property SSOUserLoggedIn As Boolean
        <DataMember()>
        Public Property SSOUserName As String

        <DataMember()>
        Public Property Telecommuting As Boolean
        <DataMember()>
        Public Property TelecommutingDays As String
        <DataMember()>
        Public Property HeaderMD5 As String
        <DataMember()>
        Public Property ShowLegalText As Boolean
        <DataMember()>
        Public Property IsCegidHub As Boolean
        <DataMember()>
        Public Property IsCommuniquesEnabled As Boolean
        <DataMember()>
        Public Property IsChannelsEnabled As Boolean
        <DataMember()>
        Public Property IsImpersonateEnabled As Boolean
        <DataMember()>
        Public Property IsLatamMex As Boolean
    End Class

End Namespace