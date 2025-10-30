Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess.roDatalinkDailyCause
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IDailyCausesService_v2
    <OperationContract>
    <SwaggerWcfPath("Creates or updates a DailyCause", "Create or update a DailyCause. The caused data is indicated in the oDailyCause object")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateDailyCause?CompanyCode={CompanyCode}&Token={Token}")>
    Function CreateOrUpdateDailyCause(
                                   <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                   Token As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Object that models the user to be inserted or updated", ParameterType:=GetType(roDailyCause))>
                                   oDailyCause As roDailyCause,
                                   <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   Optional CompanyCode As String = "") As roDatalinkStandarDailyCauseResponse

    <OperationContract>
    <SwaggerWcfPath("Get the daily causes of one or all users in a period.", "Retrieve the daily value of causes for the selected user or users in the chosen period.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetDailyCauses?Token={Token}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&CompanyCode={CompanyCode}&AddRelatedIncidence={AddRelatedIncidence}&Criteria={Criteria}")>
    Function GetDailyCauses(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from whom the causes will be recovered. If not informed, the causes of all users are recovered. Can also indicate multiple user identifiers separated by ;", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Get data from related daily incidence", ParameterType:=GetType(Boolean))>
                        Optional AddRelatedIncidence As Boolean = False,
                       <SwaggerWcfParameter(Required:=False, Description:="Criterion with which the Daily Causes are recovered. By default, those whose dates are included in the indicated period are recovered. If we inform ""Timestamp"", the daily causes that have been modified in the indicated period are recovered, regardless of the dates of the daily cause", ParameterType:=GetType(String))>
                       Optional Criteria As String = "") As roWSResponse(Of roDailyCause())

    <OperationContract>
    <SwaggerWcfPath("Get the daily causes that were modified after a timestamp of one or all users.", "Retrieve the daily value of causes for the selected user or users in the chosen period that were modified after the timestamp.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetDailyCausesByTimestamp?Token={Token}&Timestamp={Timestamp}&Employee={Employee}&CompanyCode={CompanyCode}&AddRelatedIncidence={AddRelatedIncidence}")>
    Function GetDailyCausesByTimestamp(
                       <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                       Token As String,
                       <SwaggerWcfParameter(Required:=True, Description:="Date and time after which causes are obtained", ParameterType:=GetType(DateTime))>
                       Timestamp As DateTime,
                       <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from whom the causes will be recovered. If not informed, the causes of all users are recovered. Can also indicate multiple user identifiers separated by ;", ParameterType:=GetType(String))>
                       Optional Employee As String = "",
                       <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                       Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Get data from related daily incidence", ParameterType:=GetType(Boolean))>
                        Optional AddRelatedIncidence As Boolean = False) As roWSResponse(Of roDailyCause())
End Interface