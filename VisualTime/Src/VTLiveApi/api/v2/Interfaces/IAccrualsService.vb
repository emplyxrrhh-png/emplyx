Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IAccrualsService_v2

    <OperationContract>
    <SwaggerWcfPath("Get the balances of one or all users in a period.", "Retrieve the daily value of balances for the selected user or users in the chosen period. Only the balances configured in VisualTime as exportable are considered. The maximum period is 366 days.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetAccruals?Token={Token}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&CompanyCode={CompanyCode}")>
    Function GetAccruals(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from which the balances will be recovered. If you are not informed, the balances of all users recover. Can also indicate multiple user identifiers separated by ;", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roAccrual())

    <OperationContract>
    <SwaggerWcfPath("Get the balances of one or all the users on a specific date", "Get the balances of one or all the users on a specific date. Only the balances configured in VisualTime as exportable are considered.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetAccrualsAtDate?Token={Token}&AtDate={AtDate}&Employee={Employee}&CompanyCode={CompanyCode}")>
    Function GetAccrualsAtDate(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="A specific date", ParameterType:=GetType(Date))>
                        AtDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from whom the balances will be retrieved. If not specified, balances for all users will be retrieved. Can also indicate multiple user identifiers separated by ;", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roAccrual())

End Interface