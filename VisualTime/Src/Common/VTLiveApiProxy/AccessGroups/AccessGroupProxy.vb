Imports System.ServiceModel.Activation
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common

Public Class AccessGroupProxy
    Implements IAccessGroupSvc

    Public Function KeepAlive() As Boolean Implements IAccessGroupSvc.KeepAlive
        Return True
    End Function


    Public Function GetAccessGroups(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAccessGroup)) Implements IAccessGroupSvc.GetAccessGroups
        Return AccessGroupMethods.GetAccessGroups(oState, bolAudit)
    End Function

    Public Function GetAccessGroupsDataSet(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAccessGroupSvc.GetAccessGroupsDataSet
        Return AccessGroupMethods.GetAccessGroupsDataSet(oState)
    End Function

    Public Function GetAccessGroupsByPassport(ByVal oState As roWsState, ByVal intIDPassport As Integer) As roGenericVtResponse(Of Integer()) Implements IAccessGroupSvc.GetAccessGroupsByPassport
        Return AccessGroupMethods.GetAccessGroupsByPassport(oState, intIDPassport)
    End Function

    Public Function GetAccessGroupsByPassportDataSet(ByVal oState As roWsState, ByVal intIDPassport As Integer, ByVal validateParent As Boolean) As roGenericVtResponse(Of DataSet) Implements IAccessGroupSvc.GetAccessGroupsByPassportDataSet
        Return AccessGroupMethods.GetAccessGroupsByPassportDataSet(oState, intIDPassport, validateParent)
    End Function

    Public Function GetAccessGroupByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean, ByVal idPassport As Integer) As roGenericVtResponse(Of roAccessGroup) Implements IAccessGroupSvc.GetAccessGroupByID
        Return AccessGroupMethods.GetAccessGroupByID(intID, oState, bolAudit, idPassport)
    End Function

    Public Function SaveAccessGroupByPassport(ByVal intIDPassport As Integer, ByVal intAccessGroups() As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessGroupSvc.SaveAccessGroupByPassport
        Return AccessGroupMethods.SaveAccessGroupByPassport(intIDPassport, intAccessGroups, oState, bolAudit)
    End Function

    Public Function SaveAccessGroup(ByVal oAccessGroup As roAccessGroup, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of roAccessGroup) Implements IAccessGroupSvc.SaveAccessGroup
        Return AccessGroupMethods.SaveAccessGroup(oAccessGroup, oState, bolAudit)
    End Function

    Public Function DeleteAccessGroup(ByVal oAccessGroup As roAccessGroup, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessGroupSvc.DeleteAccessGroup
        Return AccessGroupMethods.DeleteAccessGroup(oAccessGroup, oState, bolAudit)
    End Function

    Public Function DeleteAccessGroupByID(ByVal intID As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessGroupSvc.DeleteAccessGroupByID
        Return AccessGroupMethods.DeleteAccessGroupByID(intID, oState, bolAudit)
    End Function

    Public Function GetEmployeeAuthorizations(ByVal IDEmployee As Integer, ByVal IDGroup As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IAccessGroupSvc.GetEmployeeAuthorizations
        Return AccessGroupMethods.GetEmployeeAuthorizations(IDEmployee, IDGroup, oState)
    End Function

    Public Function CopyAccess(ByVal _IDSourceAccess As Integer, ByVal _NewName As String, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of roAccessGroup) Implements IAccessGroupSvc.CopyAccess
        Return AccessGroupMethods.CopyAccess(_IDSourceAccess, _NewName, oState, bAudit)
    End Function

    Public Function EmptyAccessGroupEmployees(ByVal _IDSourceAccess As Integer, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessGroupSvc.EmptyAccessGroupEmployees
        Return AccessGroupMethods.EmptyAccessGroupEmployees(_IDSourceAccess, oState, bAudit)
    End Function

    Public Function GetAuthorizationsByZone(ByVal IdZone As Integer, ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Generic.List(Of roAccessGroup)) Implements IAccessGroupSvc.GetAuthorizationsByZone
        Return AccessGroupMethods.GetAuthorizationsByZone(IdZone, oState, bolAudit)
    End Function

    Public Function UpgradeAccessMode(ByVal oState As roWsState, ByVal bolAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IAccessGroupSvc.UpgradeAccessMode
        Return AccessGroupMethods.UpgradeAccessMode(oState, bolAudit)
    End Function
End Class
