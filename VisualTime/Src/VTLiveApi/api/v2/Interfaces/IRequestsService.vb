Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IRequestsService_v2

    <OperationContract>
    <SwaggerWcfPath("Get the requests of one or all the users in a period", "Get the requests of one or all the users in a period, it can be filtered by request type. The maximum perido is 366 days")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetRequestsBetweenDates?Token={Token}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&Type={Type}&CompanyCode={CompanyCode}")>
    Function GetRequestsBetweenDates(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from which the requests will be recovered. If you are not informed, the requests of all users recover", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the type from which the requests will be recovered. If you are not informed, the requests of all types recover", ParameterType:=GetType(String))>
                        Optional Type As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roRequest())

End Interface