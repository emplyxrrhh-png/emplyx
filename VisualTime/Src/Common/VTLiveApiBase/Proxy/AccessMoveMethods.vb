Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessMove
Imports Robotics.Base.VTBusiness.Common

Public Class AccessMoveMethods

    '=========================================
    '============= CUBO ======================
    '=========================================
    Public Shared Function GetAccessPlatesViewsDataSet(ByVal IdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessMoveState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        oResult.Value = roAccessMove.GetAccessPlatesViewsDataSet(IdPassport, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetAccessPlatesViewbyID(ByVal ID As Integer, ByVal IdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessMoveState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        oResult.Value = roAccessMove.GetAccessPlatesViewbyID(ID, IdPassport, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function DeleteAccessPlatesView(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessMoveState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roAccessMove.DeleteAccessPlatesView(intID, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult

    End Function

    Public Shared Function NewAccessPlatesView(ByVal IdView As Integer, ByVal IdPassport As Integer, ByVal NameView As String, ByVal Description As String, ByVal DateView As DateTime,
                                      ByVal Employees As String, ByVal DateInf As DateTime, ByVal DateSup As DateTime, ByVal CubeLayout As String, ByVal FilterData As String,
                                      ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessMoveState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roAccessMove.NewAccessPlatesView(IdView, IdPassport, NameView, Description, DateView, Employees, DateInf, DateSup, CubeLayout, FilterData, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class