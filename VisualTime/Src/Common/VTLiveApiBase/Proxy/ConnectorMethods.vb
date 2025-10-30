Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class ConnectorMethods

    Public Shared Function InitTaskWithParams(ByVal Task As TasksType, ByVal strParamsXML As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oParams As New roCollection(strParamsXML)
        roConnector.InitTask(Task, oParams)
        oResult.Value = True

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetSetting(ByVal oKey As eKeys, ByVal oState As roWsState) As roGenericVtResponse(Of Object)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Object)

        If oKey = eKeys.Running Then
            oResult.Value = "True"
        Else
            Dim oSettings As New roSettings()
            oResult.Value = oSettings.GetVTSetting(oKey)
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetParameter(ByVal oParameter As Parameters, ByVal oState As roWsState) As roGenericVtResponse(Of Object)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Object)
        Dim oParameters As New roParameters("OPTIONS")
        oResult.Value = oParameters.Parameter(oParameter)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function GetParameters(ByVal oState As roWsState) As roGenericVtResponse(Of roParameters)
        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roParameters)
        oResult.Value = New roParameters("OPTIONS")

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

    Public Shared Function SaveParameters(ByVal oParameters As roParameters, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean)

        'cambio mi state genérico a un estado especifico
        Dim bState = New roAccessGroupState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oParameters.Save(True, bAudit, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState
        Return oResult
    End Function

End Class