Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTSecurity
Imports Robotics.Base

Public Class SecurityChartProxy
    Implements ISecurityChartSvc

    Public Function KeepAlive() As Boolean Implements ISecurityChartSvc.KeepAlive
        Return True
    End Function


    Public Function GenerateSecurityMode(ByVal oState As roWsState, ByVal intSecurityMode As Integer, ByVal intPassport As Integer) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.GenerateSecurityMode
        Return SecurityChartMethods.GenerateSecurityMode(oState, intSecurityMode, intPassport)
    End Function



    Public Function GenerateSecurityNodesFromGroups(ByVal oState As roWsState, ByVal intLevel As Integer) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.GenerateSecurityNodesFromGroups
        Return SecurityChartMethods.GenerateSecurityNodesFromGroups(oState, intLevel)
    End Function


    Public Function GetSecurityNodeFromGroupOrEmployee(ByVal oState As roWsState, ByVal intID As Integer, ByVal IsGroup As Boolean) As roGenericVtResponse(Of Integer) Implements ISecurityChartSvc.GetSecurityNodeFromGroupOrEmployee
        Return SecurityChartMethods.GetSecurityNodeFromGroupOrEmployee(oState, intID, IsGroup)
    End Function



    Public Function GetSecurityChartForRequest(ByVal oState As roWsState, ByVal intIDRequest As Integer) As roGenericVtResponse(Of roSecurityNode) Implements ISecurityChartSvc.GetSecurityChartForRequest
        Return SecurityChartMethods.GetSecurityChartForRequest(oState, intIDRequest)
    End Function


    Public Function GetSecurityChartForSelector(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityNode) Implements ISecurityChartSvc.GetSecurityChartForSelector
        Return SecurityChartMethods.GetSecurityChartForSelector(idPassport, oState)
    End Function


    Public Function GetSecurityChartNodeFromPassport(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityNode) Implements ISecurityChartSvc.GetSecurityChartNodeFromPassport
        Return SecurityChartMethods.GetSecurityChartNodeFromPassport(idPassport, oState)
    End Function


    Public Function AssignPassportToSecurityNode(ByVal oSecurityNode As roSecurityNode, ByVal oSecurityNodePassport As roSecurityNodePassport, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.AssignPassportToSecurityNode
        Return SecurityChartMethods.AssignPassportToSecurityNode(oSecurityNode, oSecurityNodePassport, oState)
    End Function


    Public Function AssignGroupToSecurityNode(ByVal oSecurityNode As roSecurityNode, ByVal oSecurityNodeGroup As roSecurityNodeGroup, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.AssignGroupToSecurityNode
        Return SecurityChartMethods.AssignGroupToSecurityNode(oSecurityNode, oSecurityNodeGroup, oState)
    End Function

    Public Function GetSecurityChart(ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityNode) Implements ISecurityChartSvc.GetSecurityChart
        Return SecurityChartMethods.GetSecurityChart(oState)
    End Function


    Public Function SaveSecurityChart(ByVal oSecurityChart As roSecurityNode, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityNode) Implements ISecurityChartSvc.SaveSecurityChart
        Return SecurityChartMethods.SaveSecurityChart(oSecurityChart, oState)
    End Function

    Public Function SaveIntermediateChart(ByVal oSecurityChart As roSecurityNode, ByVal direction As Integer, ByVal idDestinationNode As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of (roSecurityNode, Integer, Integer)) Implements ISecurityChartSvc.SaveIntermediateChart
        Return SecurityChartMethods.SaveIntermediateChart(oSecurityChart, direction, idDestinationNode, oState)
    End Function


    Public Function GetSecurityChartNodeSingle(ByVal iIDSecurityNode As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roSecurityNode) Implements ISecurityChartSvc.GetSecurityChartNodeSingle
        Return SecurityChartMethods.GetSecurityChartNodeSingle(iIDSecurityNode, oState)
    End Function


    Public Function GetGroupFeatures(ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature()) Implements ISecurityChartSvc.GetGroupFeatures
        Return SecurityChartMethods.GetGroupFeatures(oState)
    End Function


    Public Function GetAllAvailableSupervisorsList(ByVal oState As roWsState, ByVal bLoadUserSystem As Boolean) As roGenericVtResponse(Of roSecurityChartPassport()) Implements ISecurityChartSvc.GetAllAvailableSupervisorsList
        Return SecurityChartMethods.GetAllAvailableSupervisorsList(oState, bLoadUserSystem)
    End Function


    Public Function GetGroupsChart(ByVal oState As roWsState) As roGenericVtResponse(Of roGroupChartItem()) Implements ISecurityChartSvc.GetGroupsChart
        Return SecurityChartMethods.GetGroupsChart(oState)
    End Function


    Public Function IsPassportAlreadyAssignedToSecurityNode(ByVal idPassport As Integer, ByVal idSecurityNode As Integer, ByVal strSecurityNodeError As String, ByVal oState As roWsState) As roGenericVtResponse(Of (String, Boolean)) Implements ISecurityChartSvc.IsPassportAlreadyAssignedToSecurityNode
        Return SecurityChartMethods.IsPassportAlreadyAssignedToSecurityNode(idPassport, idSecurityNode, strSecurityNodeError, oState)
    End Function



    Public Function GetGroupFeaturesById(ByVal iIdGroupFeature As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature) Implements ISecurityChartSvc.GetGroupFeaturesById
        Return SecurityChartMethods.GetGroupFeaturesById(iIdGroupFeature, oState)
    End Function


    Public Function DeleteGroupFeature(ByVal oGroupFeature As roGroupFeature, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.DeleteGroupFeature
        Return SecurityChartMethods.DeleteGroupFeature(oGroupFeature, oState)
    End Function


    Public Function SaveGroupFeatures(ByVal oGroupFeature As roGroupFeature, ByVal oState As roWsState) As roGenericVtResponse(Of roGroupFeature) Implements ISecurityChartSvc.SaveGroupFeatures
        Return SecurityChartMethods.SaveGroupFeatures(oGroupFeature, oState)
    End Function


    Public Function SetGroupFeaturePermission(ByVal iGroupFeatureID As Integer, ByVal iFeatureID As Integer, ByVal iPermission As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.SetGroupFeaturePermission
        Return SecurityChartMethods.SetGroupFeaturePermission(iGroupFeatureID, iFeatureID, iPermission, oState)
    End Function


    Public Function CopyGroupFeature(ByVal iGroupFeatureID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.CopyGroupFeature
        Return SecurityChartMethods.CopyGroupFeature(iGroupFeatureID, oState)
    End Function


    Public Function CopySupervisorProperties(ByVal iPassportID As Integer, ByVal iDestinationPassportIDs As Integer(), ByVal copyRestrictions As Boolean, ByVal copyCostCenters As Boolean, ByVal copyBusinessGroups As Boolean, ByVal copyCategories As Boolean, ByVal copyGroups As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.CopySupervisorProperties
        Return SecurityChartMethods.CopySupervisorProperties(iPassportID, iDestinationPassportIDs, copyRestrictions, copyCostCenters, copyBusinessGroups, copyCategories, copyGroups, oState)
    End Function


    Public Function GetSecurityChartLoad(ByVal oState As roWsState) As roGenericVtResponse(Of Integer) Implements ISecurityChartSvc.GetSecurityChartLoad

        Return SecurityChartMethods.GetSecurityChartLoad(oState)
    End Function


    Public Function AddExceptionstoPassport(ByVal oState As roWsState, ByVal passport As Integer, ByVal node As roSecurityNodePassport) As roGenericVtResponse(Of Boolean) Implements ISecurityChartSvc.AddExceptionstoPassport

        Return SecurityChartMethods.AddExceptionstoPassport(oState, passport, node)
    End Function

    Public Function GetExceptions(ByVal oState As roWsState, ByVal passport As Integer) As roGenericVtResponse(Of DataSet) Implements ISecurityChartSvc.GetExceptions

        Return SecurityChartMethods.GetExceptions(oState, passport)
    End Function


End Class
