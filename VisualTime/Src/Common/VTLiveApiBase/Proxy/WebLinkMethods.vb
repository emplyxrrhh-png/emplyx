Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTWebLinks

Public Class WebLinkMethods


    Public Shared Function GetAllWebLinks(ByVal oState As roWsState) As roGenericVtResponse(Of List(Of roWebLink))

        Dim bState As New roWebLinksManagerState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of List(Of roWebLink))
        Dim webLinkState As New roWebLinksManagerState(oState.IDPassport)
        Dim webLinksList As List(Of roWebLink) = roWebLinksManager.GetAllWebLinks(webLinkState)

        oResult.Value = webLinksList

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function CreateOrUpdateWebLink(webLink As roWebLink, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        Dim bState As New roWebLinksManagerState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim webLinkState As New roWebLinksManagerState(oState.IDPassport)
        Dim result As Integer = roWebLinksManager.CreateOrUpdateWebLink(webLink, webLinkState)

        oResult.Value = result

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function DeleteWebLink(webLink As roWebLink, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState As New roWebLinksManagerState(oState.IDPassport)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim webLinkState As New roWebLinksManagerState(oState.IDPassport)
        Dim result As Boolean = roWebLinksManager.DeleteWebLink(webLink, webLinkState)

        oResult.Value = result

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function
End Class