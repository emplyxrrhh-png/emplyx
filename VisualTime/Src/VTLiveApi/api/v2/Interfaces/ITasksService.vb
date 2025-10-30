Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface ITasksService_v2

    <OperationContract>
    <SwaggerWcfPath("Get the balances of one or all the users in a period in each task", "gets the daily value of the balances for the user or users in the selected period in each task.  The maximum perido is 366 days")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetTaskAccrualsBetweenDates?Token={Token}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&Project={Project}&Task={Task}&CompanyCode={CompanyCode}")>
    Function GetTaskAccrualsBetweenDates(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from which the balances will be recovered. If you are not informed, the balances of all users recover", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the project from which the balances will be recovered. If you are not informed, the balances of all projects recover", ParameterType:=GetType(String))>
                        Optional Project As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the task from which the balances will be recovered. If you are not informed, the balances of all tasks recover", ParameterType:=GetType(String))>
                        Optional Task As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roTaskAccrual())

End Interface