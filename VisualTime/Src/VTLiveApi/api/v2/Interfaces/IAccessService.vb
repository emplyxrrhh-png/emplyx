Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IAccessService_v2

    <SwaggerWcfPath("Get the terminal configuration", "Gets the list of employees authorized in the terminal and the list of period access")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetTerminalConfiguration?Token={Token}&TerminalID={TerminalID}&CompanyCode={CompanyCode}")>
    <OperationContract>
    Function GetTerminalConfiguration(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Identifier of the terminal from which the configuration will be recovered.", ParameterType:=GetType(String))>
                        TerminalID As String,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roTerminalConfiguration)

    <SwaggerWcfPath("Get the terminal datetime", "Gets the terminal datetime about timezone configuration.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetTerminalDateTime?Token={Token}&TerminalID={TerminalID}&CompanyCode={CompanyCode}")>
    <OperationContract>
    Function GetTerminalDateTime(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Identifier of the terminal from which the configuration will be recovered.", ParameterType:=GetType(String))>
                        TerminalID As String,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roWCFDate)

End Interface