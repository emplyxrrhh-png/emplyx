Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.AccessGroup
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class ConnectorProxy
    Implements IConnectorSvc

    Public Function KeepAlive() As Boolean Implements IConnectorSvc.KeepAlive
        Return True
    End Function


    Public Function InitTaskWithParams(ByVal Task As TasksType, ByVal strParamsXML As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IConnectorSvc.InitTaskWithParams
        Return ConnectorMethods.InitTaskWithParams(Task, strParamsXML, oState)
    End Function

    Public Function GetSetting(ByVal oKey As eKeys, ByVal oState As roWsState) As roGenericVtResponse(Of Object) Implements IConnectorSvc.GetSetting
        Return ConnectorMethods.GetSetting(oKey, oState)
    End Function

    Public Function GetParameter(ByVal oParameter As Parameters, ByVal oState As roWsState) As roGenericVtResponse(Of Object) Implements IConnectorSvc.GetParameter
        Return ConnectorMethods.GetParameter(oParameter, oState)
    End Function

    Public Function GetParameters(ByVal oState As roWsState) As roGenericVtResponse(Of roParameters) Implements IConnectorSvc.GetParameters

        Return ConnectorMethods.GetParameters(oState)
    End Function

    Public Function SaveParameters(ByVal oParameters As roParameters, ByVal oState As roWsState, ByVal bAudit As Boolean) As roGenericVtResponse(Of Boolean) Implements IConnectorSvc.SaveParameters
        Return ConnectorMethods.SaveParameters(oParameters, oState, bAudit)
    End Function


End Class
