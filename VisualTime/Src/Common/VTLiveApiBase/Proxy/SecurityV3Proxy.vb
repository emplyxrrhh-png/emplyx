Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTNotifications.Notifications
Imports Robotics.Security.Base

Public Class SecurityV3Methods

    Public Shared Function GetRequestCategories(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roSecurityCategory))

        Dim bState = New roSecurityCategoryState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of List(Of roSecurityCategory))
        genericResponse.Value = roSecurityCategoryManager.LoadSecurityCategories(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function GetRequestTypesWithCategories(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roRequestType))

        Dim bState = New roSecurityCategoryState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of List(Of roRequestType))
        genericResponse.Value = roSecurityCategoryManager.LoadRequestTypesWithCategories(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse

    End Function

    Public Shared Function GetNotificationTypesWithCategories(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roNotificationType))

        Dim bState = New roSecurityCategoryState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of List(Of roNotificationType))
        genericResponse.Value = roNotification.LoadNotificationTypesWithCategories(bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse

    End Function

    Public Shared Function GetPassportLevelOfAuthority(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal xRequestType As eRequestType, ByVal idCause As Integer, ByVal idRequest As Integer) As roGenericVtResponse(Of Integer)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of Integer)
        genericResponse.Value = roPassportManager.GetRequestPassportLevelOfAuthority(bState, idPassport, xRequestType, idCause, idRequest)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse

    End Function

    Public Shared Function SaveNotificationTypesWithCategories(ByVal oNotificationTypeList As List(Of roNotificationType), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roSecurityCategoryState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bSaved As Boolean = roSecurityCategoryManager.SaveNotificationTypesWithCategories(bState, oNotificationTypeList)
        oResult.Value = bSaved

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function GetAllAvailableSupervisorsList(ByVal oState As roWsState, ByVal bLoadUserSystem As Boolean) As roGenericVtResponse(Of roPassport())

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPassport())

        oResult.Value = roPassportManager.GetAllAvailableSupervisorsList(bState, bLoadUserSystem)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetPassportsByEmail(ByVal oState As roWsState, ByVal sEmail As String) As roGenericVtResponse(Of roPassportTicket())

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPassportTicket())

        oResult.Value = roPassportManager.GetPassportsByEmail(bState, sEmail)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function CopySupervisorProperties(ByVal iPassportID As Integer, ByVal iDestinationPassportIDs As Integer(), ByVal copyRestrictions As Boolean, ByVal copyCostCenters As Boolean, ByVal copyBusinessGroups As Boolean, ByVal copyCategories As Boolean, ByVal copyGroups As Boolean, ByVal copyRoles As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        oResult.Value = roPassportManager.CopySupervisorProperties(iPassportID, iDestinationPassportIDs, copyRestrictions, copyCostCenters, copyBusinessGroups, copyCategories, copyGroups, copyRoles, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function


    Public Shared Function GetTreeState(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal idTreeState As String) As roGenericVtResponse(Of String)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of String)
        Dim oManager As New roPassportCacheDataManager(bState)
        genericResponse.Value = oManager.Load(idPassport, idTreeState, SeletorType.TreeState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveTreeState(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal idTreeState As String, ByVal value As String) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of Boolean)
        Dim oManager As New roPassportCacheDataManager(bState)
        genericResponse.Value = oManager.Save(idPassport, idTreeState, value, SeletorType.TreeState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function DeleteTreeState(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal idTreeState As String) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of Boolean)
        Dim oManager As New roPassportCacheDataManager(bState)
        genericResponse.Value = oManager.Delete(idPassport, idTreeState, SeletorType.TreeState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function GetUniversalSelector(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal idUniversalSelector As String) As roGenericVtResponse(Of String)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of String)
        Dim oManager As New roPassportCacheDataManager(bState)
        genericResponse.Value = oManager.Load(idPassport, idUniversalSelector, SeletorType.Universal)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function SaveUniversalSelector(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal idUniversalSelector As String, ByVal value As String) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of Boolean)
        Dim oManager As New roPassportCacheDataManager(bState)
        genericResponse.Value = oManager.Save(idPassport, idUniversalSelector, value, SeletorType.Universal)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function DeleteUniversalSelector(ByVal oState As roWsState, ByVal idPassport As Integer, ByVal idUniversalSelector As String) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New roGenericVtResponse(Of Boolean)
        Dim oManager As New roPassportCacheDataManager(bState)
        genericResponse.Value = oManager.Delete(idPassport, idUniversalSelector, SeletorType.Universal)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

End Class