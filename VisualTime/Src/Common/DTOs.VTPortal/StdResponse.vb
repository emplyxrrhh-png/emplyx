Imports System.Runtime.Serialization

Namespace DTOs

    ''' <summary>
    ''' Representa el resultado de una acció genérica del Portal
    ''' </summary>
    <DataContract>
    Public Class StdResponse

        Public Sub New()
            Result = False
            Status = 0
            CustomErrorText = String.Empty
        End Sub

        ''' <summary>
        ''' Resultado de la acció. True=Se realizó la acción, False=No se realizó la acción
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Result As Boolean
        ''' <summary>
        ''' Texto especifico para indicar errores customizados
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CustomErrorText As String

        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As Long
    End Class

    ''' <summary>
    ''' Representa el resultado de una acció genérica del Portal
    ''' </summary>
    <DataContract>
    Public Class DaysCount

        Public Sub New()
            Days = 0
            Status = 0
        End Sub

        ''' <summary>
        ''' Resultado de la acció. True=Se realizó la acción, False=No se realizó la acción
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Days As Integer

        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As Long
    End Class

    <DataContract>
    Public Class StdServerVersion

        Public Sub New()
            Result = True
            Status = 0
            ApiVersion = 0
            DefaultVersion = "default"
            SSOVersion = 0
            SSOType = ""
            SSOUserName = ""
            RefreshConfiguration = ""
        End Sub

        ''' <summary>
        ''' Resultado de la acció. True=Se realizó la acción, False=No se realizó la acción
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Result As Boolean
        ''' <summary>
        ''' Esta habilitat el login mixte en el SSO
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SSOmixedModeEnabled As Boolean
        ''' <summary>
        ''' Tipo de single sign on que aplica en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SSOServerEnabled As Boolean
        ''' <summary>
        ''' Tipo de single sign on que aplica en el sistema
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SSOType As String
        ''' <summary>
        ''' Nombre del usuario logeado en el windows si SSOType = "SSO"
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SSOUserName As String
        ''' <summary>
        ''' Estado de autenticacion del usuario logeado en el windows si SSOType = "SSO"
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property SSOUserLoggedIn As Boolean
        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As Long

        ''' <summary>
        ''' Cadena de texto con un mensaje de error personalizado si atañe
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ApiVersion As Integer

        <DataMember>
        Public Property DefaultVersion As String

        <DataMember>
        Public Property SSOVersion As Integer


        <DataMember>
        Public Property RefreshConfiguration As String
    End Class

    <DataContract>
    Public Class StdRequestResponse

        Public Sub New()
            Result = True
            Status = 0
            StatusErrorMsg = String.Empty
            RequestId = -1
            PunchWithoutRequest = False
        End Sub

        ''' <summary>
        ''' Resultado de la acción. True=Se realizó la acción, False=No se realizó la acción
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Result As Boolean

        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As Long

        ''' <summary>
        ''' Cadena de texto con un mensaje de error personalizado si atañe
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property StatusErrorMsg As String

        ''' <summary>
        ''' Cadena de texto con un mensaje de error personalizado si atañe
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property StatusInfoMsg As String

        ''' <summary>
        ''' Id de la solicitud
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property RequestId As Integer

        <DataMember>
        Public Property PunchWithoutRequest As Boolean
    End Class

    <DataContract>
    Public Class StdCheckTelecommuteResponse

        Public Sub New()
            Result = False
            Status = 0
            CustomErrorText = String.Empty
            NeedRequest = False
        End Sub

        ''' <summary>
        ''' Resultado de la acció. True=Se realizó la acción, False=No se realizó la acción
        ''' </summary>
        ''' <returns></returns>
        <DataMember()>
        Public Property Result As Boolean
        ''' <summary>
        ''' Texto especifico para indicar errores customizados
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property CustomErrorText As String

        ''' <summary>
        ''' 0 indica que los datos són correctos. Cualquier otro valor indica un error que se puede obtener de los posibles valores de ErrorCodes
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Status As Long

        ''' <summary>
        ''' Indica si se debe realizar solicitud para establecer teletrabajo
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property NeedRequest As Boolean
    End Class

    <DataContract>
    Public Class TimegateResponse

        Public Sub New()
            Status = 0
            LoginInfo = Nothing
            PunchInfo = Nothing
            TimeGateMode = ScopeMode.UNDEFINED
        End Sub

        <DataMember()>
        Public TimeGateMode As ScopeMode

        <DataMember()>
        Public LoginInfo As LoginResult

        <DataMember()>
        Public PunchInfo As roTerminalInteractivePunch

        <DataMember>
        Public Property Status As Long
    End Class

End Namespace