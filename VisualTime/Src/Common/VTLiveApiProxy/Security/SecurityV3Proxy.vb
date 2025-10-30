Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security

Public Class SecurityV3Proxy
    Implements ISecurityV3Svc

    Public Function KeepAlive() As Boolean Implements ISecurityV3Svc.KeepAlive
        Return True
    End Function

    Public Function GetRequestCategories(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roSecurityCategory)) Implements ISecurityV3Svc.GetRequestCategories

        Return SecurityV3Methods.GetRequestCategories(oState)

    End Function

    Public Function GetRequestTypesWithCategories(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roRequestType)) Implements ISecurityV3Svc.GetRequestTypesWithCategories

        Return SecurityV3Methods.GetRequestTypesWithCategories(oState)

    End Function


    Public Function GetNotificationTypesWithCategories(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roNotificationType)) Implements ISecurityV3Svc.GetNotificationTypesWithCategories

        Return SecurityV3Methods.GetNotificationTypesWithCategories(oState)

    End Function

    Public Function GetPassportLevelOfAuthority(ByVal oState As roWsState, ByVal IDPassport As Integer, ByVal xRequestType As eRequestType, ByVal IDCause As Integer) As roGenericVtResponse(Of Integer) Implements ISecurityV3Svc.GetPassportLevelOfAuthority

        Return SecurityV3Methods.GetPassportLevelOfAuthority(oState, IDPassport, xRequestType, IDCause)

    End Function

    Public Function SaveRequestTypesWithCategories(ByVal oRequestTypeList As List(Of roRequestType), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityV3Svc.SaveRequestTypesWithCategories
        Return SecurityV3Methods.SaveRequestTypesWithCategories(oRequestTypeList, oState)
    End Function


    Public Function SaveNotificationTypesWithCategories(ByVal oNotificationTypeList As List(Of roNotificationType), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityV3Svc.SaveNotificationTypesWithCategories
        Return SecurityV3Methods.SaveNotificationTypesWithCategories(oNotificationTypeList, oState)
    End Function

End Class
