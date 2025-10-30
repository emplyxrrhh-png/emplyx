Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security
Imports Robotics.Security.Base

Public Class SecurityMethods

    Public Shared Function GetCurrentLoggedUsers(ByVal oState As roWsState) As UserList
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New UserList
        Dim newGState As New roWsState

        genericResponse.Users = SessionHelper.GetCurrentLoggedUsers(bState)
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

    Public Shared Function GetConcurrencyInfo(ByVal oState As roWsState) As ConcurrencyInfoList

        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim genericResponse As New ConcurrencyInfoList
        Dim newGState As New roWsState

        genericResponse.ConcurrencyInfoValues = SessionHelper.GetConcurrencyInfo(bState)
        roWsStateManager.CopyTo(bState, newGState)
        genericResponse.Status = newGState

        Return genericResponse
    End Function

End Class