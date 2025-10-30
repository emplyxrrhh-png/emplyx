Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security

Public Class SecurityProxy
    Implements ISecuritySvc

    Public Function KeepAlive() As Boolean Implements ISecuritySvc.KeepAlive
        Return True
    End Function

    Public Function LoadActualConsentByPassport(ByVal iID As Integer, ByVal PassportType As LoadType, ByVal ConsentType As ConsentTypeEnum, ByVal oState As roWsState) As roPassportConsentResponse Implements ISecuritySvc.LoadActualConsentByPassport
        Return SecurityMethods.LoadActualConsentByPassport(iID, PassportType, ConsentType, oState)
    End Function

    Public Function GetConsentsByPassport(ByVal iID As Integer, ByVal PassportType As LoadType, ByVal oState As roWsState) As roPassportConsentsResponse Implements ISecuritySvc.GetConsentsByPassport
        Return SecurityMethods.GetConsentsByPassport(iID, PassportType, oState)
    End Function

    Public Function SaveConsent(ByVal oConsent As roPassportConsent, ByVal bAudit As Boolean, ByVal oState As roWsState) As roPassportConsentResponse Implements ISecuritySvc.SaveConsent
        Return SecurityMethods.SaveConsent(oConsent, bAudit, oState)
    End Function

    Public Function MarkConsentAsDelivered(ByVal iIDPassport As Integer, ByVal iIDSupervisor As Integer, ByVal ConsentType As ConsentTypeEnum, ByVal ConsentText As String, ByVal ApprovalDate As Date, ByVal oState As roWsState) As roStandarResponse Implements ISecuritySvc.MarkConsentAsDelivered
        Return SecurityMethods.MarkConsentAsDelivered(iIDPassport, iIDSupervisor, ConsentType, ConsentText, ApprovalDate, oState)
    End Function


    Public Function MarkConsentAsUnDelivered(ByVal iIDPassport As Integer, ByVal iIDSupervisor As Integer, ByVal ConsentType As ConsentTypeEnum, ByVal oState As roWsState) As roStandarResponse Implements ISecuritySvc.MarkConsentAsUnDelivered
        Return SecurityMethods.MarkConsentAsUnDelivered(iIDPassport, iIDSupervisor, ConsentType, oState)
    End Function


    Public Function GetCurrentLoggedUsers(ByVal oState As roWsState) As UserList Implements ISecuritySvc.GetCurrentLoggedUsers
        Return SecurityMethods.GetCurrentLoggedUsers(oState)
    End Function

    Public Function GetConcurrencyInfo(ByVal oState As roWsState) As ConcurrencyInfoList Implements ISecuritySvc.GetConcurrencyInfo
        Return SecurityMethods.GetConcurrencyInfo(oState)
    End Function

End Class
