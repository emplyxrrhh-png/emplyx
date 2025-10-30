Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface ISupervisorsService_v2


    <OperationContract>
    <SwaggerWcfPath("Create or update a user", "Create or update a user. The user data is indicated in the oSupervisor object")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateSupervisor?CompanyCode={CompanyCode}&Token={Token}")>
    Function CreateOrUpdateSupervisor(
                               <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Supervisor data", ParameterType:=GetType(roSupervisor))>
                               supervisor As roSupervisor,
                               Optional ByVal CompanyCode As String = ""
                               ) As roWSResponse(Of roSupervisor)

    <OperationContract>
    <SwaggerWcfPath("Get all supervisors", "Retrieve the list of all supervisors")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetAllSupervisors?CompanyCode={CompanyCode}&Token={Token}")>
    Function GetAllSupervisors(
                                <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                Token As String,
                                Optional ByVal CompanyCode As String = ""
                                ) As roWSResponse(Of List(Of roSupervisor))

    <OperationContract>
    <SwaggerWcfPath("Get roles", "Retrieve the list of roles")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetRoles?CompanyCode={CompanyCode}&Token={Token}")>
    Function GetRoles(
                                <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                Token As String,
                                Optional ByVal CompanyCode As String = ""
                                ) As roWSResponse(Of List(Of roRole))


    <OperationContract>
    <SwaggerWcfPath("Get roles", "Retrieve the list of categories")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetCategories?CompanyCode={CompanyCode}&Token={Token}")>
    Function GetCategories(
                                <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                Token As String,
                                Optional ByVal CompanyCode As String = ""
                                ) As roWSResponse(Of List(Of roCategory))

End Interface