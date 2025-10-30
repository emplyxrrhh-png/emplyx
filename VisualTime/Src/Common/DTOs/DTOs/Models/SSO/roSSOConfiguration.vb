Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    <DataContract>
    Public Enum SSOType
        <EnumMember> None = 0
        <EnumMember> cegidId = 1
        <EnumMember> Adfs = 2
        <EnumMember> AAD = 3
        <EnumMember> Okta = 4
        <EnumMember> SAML = 5
    End Enum

    <DataContract>
    Public Class roSSOConfiguration

        Public Sub New()
            Me.SSOType = SSOType.None
            Me.Active = False
            Me.VTLiveMixAuthEnabled = True
            Me.VTPortalMixAuthEnabled = False
            Me.AAD = New AADConf With {.ClientID = "", .TenantID = ""}
            Me.Adfs = New ADfsConf With {.FedartionURL = "", .FederationRealm = ""}
            Me.Okta = New OktaConf With {.ClientId = "", .ClientSecret = "", .Authority = ""}
            Me.SAML = New SAMLConf With {.MetadataURL = "", .IdentityProviderID = "", .SigningBehaviour = ""}
        End Sub

        <DataMember>
        Public Property SSOType As SSOType
        <DataMember>
        Public Property Active As Boolean
        <DataMember>
        Public Property VTLiveMixAuthEnabled As Boolean
        <DataMember>
        Public Property VTPortalMixAuthEnabled As Boolean

        <DataMember>
        Public Property AAD As AADConf

        <DataMember>
        Public Property Adfs As ADfsConf

        <DataMember>
        Public Property Okta As OktaConf
        <DataMember>
        Public Property SAML As SAMLConf
    End Class

    <DataContract>
    Public Class AADConf
        <DataMember>
        Public Property ClientID As String
        <DataMember>
        Public Property TenantID As String
    End Class

    <DataContract>
    Public Class ADfsConf
        <DataMember>
        Public Property FedartionURL As String
        <DataMember>
        Public Property FederationRealm As String
    End Class

    <DataContract>
    Public Class OktaConf
        <DataMember>
        Public Property ClientId As String
        <DataMember>
        Public Property ClientSecret As String

        <DataMember>
        Public Property Authority As String
    End Class

    <DataContract>
    Public Class SAMLConf
        <DataMember>
        Public Property MetadataURL As String
        <DataMember>
        Public Property IdentityProviderID As String
        <DataMember>
        Public Property SigningBehaviour As String
    End Class

End Namespace