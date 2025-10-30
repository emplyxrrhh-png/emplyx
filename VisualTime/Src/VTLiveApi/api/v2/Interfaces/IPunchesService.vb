Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IPunchesService_v2

    <OperationContract>
    <SwaggerWcfPath("Gets recorded or modified signings from a date and time ", " Get the list of recorded or modified signings from a date and time")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetPunches?CompanyCode={CompanyCode}&Token={Token}&Timestamp={Timestamp}&EmployeeID={EmployeeID}")>
    Function GetPunches(
                        <SwaggerWcfParameter(Required:=True, Description:="Token security", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Date and time from which the signings are obtained", ParameterType:=GetType(DateTime))>
                        Timestamp As DateTime,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user who wants to recover the punches", ParameterType:=GetType(String))>
                        Optional EmployeeID As String = ""
                        ) As roWSResponse(Of roPunch())

    <OperationContract>
    <SwaggerWcfPath("Gets the signings performed in a period ", " obtains the signings made in a period, regardless of when registered or modified in the system")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetPunchesBetweenDates?CompanyCode={CompanyCode}&Token={Token}&StartDate={StartDate}&EndDate={EndDate}&EmployeeID={EmployeeID}")>
    Function GetPunchesBetweenDates(
                        <SwaggerWcfParameter(Required:=True, Description:="Token security", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Date and time", ParameterType:=GetType(Date))>
                        StartDate As Date,
                         <SwaggerWcfParameter(Required:=True, Description:="Date and time", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user who wants to recover the punches", ParameterType:=GetType(String))>
                        Optional EmployeeID As String = ""
                        ) As roWSResponse(Of roPunch())

    <OperationContract>
    <SwaggerWcfPath("Record a list of users ", " incorporate signings from one or more users. The details of each signing from the list are detailed in an Opunch object")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="AddPunches?Token={Token}&CompanyCode={CompanyCode}")>
    Function AddPunches(
                               <SwaggerWcfParameter(Required:=True, Description:="Token security", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Object that models the sign of signings to be incorporated", ParameterType:=GetType(roPunch()))>
                               oPunches As roPunch(),
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roPunchesResponse())

    <OperationContract>
    <SwaggerWcfPath("Update punches ", " update one or more punches. The punch to modify is indicated with the PunchID. Only the date, type and actualType can be updated on presence punches.")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="UpdatePunches?Token={Token}&CompanyCode={CompanyCode}")>
    Function UpdatePunches(
                               <SwaggerWcfParameter(Required:=True, Description:="Token security", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Array of Objects that will be updated.", ParameterType:=GetType(roPunchCriteria()))>
                               oPunches As roPunchCriteria(),
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of List(Of roPunchesResponse))

    <OperationContract>
    <SwaggerWcfPath("Delete punches ", " delete one or more punches.")>
    <WebInvoke(Method:="DELETE", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="DeletePunches?Token={Token}&CompanyCode={CompanyCode}")>
    Function DeletePunches(
                               <SwaggerWcfParameter(Required:=True, Description:="Token security", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Array of Objects that will be deleted. Only punches with type 1,2 or 3 are available to delete.", ParameterType:=GetType(roPunchToDeleteCriteria()))>
                               oPunches As roPunchToDeleteCriteria(),
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of List(Of roPunchesResponse))

End Interface