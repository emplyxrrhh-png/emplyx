Imports System.Runtime.Serialization

Namespace Robotics.Base.DTOs

    Public Enum AdvancedParameterType
        Customization
        CustomizationLogEnabled
        ReportsPersistOnSystem
        SSOEnabled
        ADFSEnabled
        PortalBackgroundImage
        VTPortalApiVersion
        VTPortalShowLogoutHome
        AnalyticsPersistOnSystem
        AnalyticsBIPersistOnSystem
        ExternAccessIPs
        ExternAccessUserName
        ExternAccessPassword
        ExternAccessToken1
        ExternAccessToken2
        BISaSLink
        DISaSToken
        ZoneRestrictedByIP
    End Enum

    <DataContract>
    <Serializable>
    Public Class AdvancedParameterCache

        <DataMember()>
        Public Property Name() As String

        <DataMember()>
        Public Property Value() As String

    End Class

End Namespace