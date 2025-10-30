Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IGroupsService_v2

    <OperationContract>
    <SwaggerWcfPath("Get the list of groups", "Get the list of groups")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetGroups?CompanyCode={CompanyCode}&Root={Root}&IncludeEmployees={IncludeEmployees}&Token={Token}&GroupID={GroupID}")>
    Function GetGroups(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Indicates whether or not users should be included in the group structure", ParameterType:=GetType(Boolean))>
                        IncludeEmployees As Boolean,
                        <SwaggerWcfParameter(Required:=False, Description:="Internal identifier of the root group to be retrieved, when the group structure has several roots. If not reported, the full structure is obtained", ParameterType:=GetType(String))>
                        Optional Root As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the group who wants to recover structure", ParameterType:=GetType(String))>
                        Optional GroupID As String = "") As roWSResponse(Of roGroup())

End Interface