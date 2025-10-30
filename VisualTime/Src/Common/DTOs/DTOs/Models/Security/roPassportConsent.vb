Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum ConsentTypeEnum
        <EnumMember> _NOTDEFINDED = 0 ' No definido
        <EnumMember> _Portal = 1 ' Portal
        <EnumMember> _Desktop = 2    ' Escritorio
        <EnumMember> _Terminal = 3   ' Terminal
        <EnumMember> _Visits = 4   ' Visitas
    End Enum

    <DataContract()>
    Public Class roPassportConsentResponse

        <DataMember()>
        Public Property PassportConsent As roPassportConsent

        <DataMember()>
        Public Property State As roWsState

    End Class

    <DataContract()>
    Public Class roPassportConsentPortals

        <DataMember()>
        Public Property PassportConsent As roPassportConsent

        <DataMember()>
        Public Property Status As Integer

    End Class

    <DataContract()>
    Public Class roPassportConsentsResponse

        <DataMember()>
        Public Property PassportConsents As roPassportConsent()

        <DataMember()>
        Public Property State As roWsState

    End Class

    <DataContract()>
    Public Class roPassportConsent

        Public Sub New()
            Me.ID = -1
            Me.IDPassport = 0
            Me.Message = String.Empty
            Me.IsValid = True
            Me.IDPassportAction = 0
            Me.NamePassportAction = String.Empty
            Me.Type = ConsentTypeEnum._NOTDEFINDED
            Me.ApprovalDate = New Date(1900, 1, 1, 0, 0, 0)
        End Sub

        ''' <summary>
        ''' Identificador único del consentimiento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ID As Long

        ''' <summary>
        ''' Identificador del passport
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDPassport As Integer

        ''' <summary>
        ''' Tipo de consentimiento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Type As ConsentTypeEnum

        ''' <summary>
        ''' texto del consentimiento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property Message As String

        ''' <summary>
        ''' Fecha aprobacion consentimiento
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property ApprovalDate As Date

        ''' <summary>
        ''' Indica si el consentimiento es valido
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IsValid As Boolean

        ''' <summary>
        ''' Indica el pasaporte que ha aprovado el consentimiento de forma indirecta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property IDPassportAction As Integer

        ''' <summary>
        ''' Nombre del pasaporte que ha aprovado el consentimiento de forma indirecta
        ''' </summary>
        ''' <returns></returns>
        <DataMember>
        Public Property NamePassportAction As String

    End Class

End Namespace