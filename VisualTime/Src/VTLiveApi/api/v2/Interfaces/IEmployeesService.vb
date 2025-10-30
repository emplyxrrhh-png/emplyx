Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IEmployeesService_v2

    <OperationContract>
    <SwaggerWcfPath("Get information from one, several or all users", "Get information from one or all users. The information for each user may contain the list of contracts and mobilities of each user. Such information is delivered via a list of oEmployee objects.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetEmployees?CompanyCode={CompanyCode}&Token={Token}&OnlyWithActiveContract={OnlyWithActiveContract}&IncludeOldData={IncludeOldData}&FieldName={FieldName}&FieldValue={FieldValue}&EmployeeID={EmployeeID}")>
    Function GetEmployees(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Get only data from users with an active contract", ParameterType:=GetType(Boolean))>
                        OnlyWithActiveContract As Boolean,
                        <SwaggerWcfParameter(Required:=True, Description:="Obtain the user's historical data regarding mobility and contracts", ParameterType:=GetType(Boolean))>
                        IncludeOldData As Boolean,
                         <SwaggerWcfParameter(Required:=False, Description:="Name of the record field by which you want to filter the users", ParameterType:=GetType(String))>
                        Optional FieldName As String = "",
                         <SwaggerWcfParameter(Required:=False, Description:="Value of the record field by which you want to filter the users", ParameterType:=GetType(String))>
                        Optional FieldValue As String = "",
                         <SwaggerWcfParameter(Required:=False, Description:="User identifier, in case you want to obtain information from a single user. If it is not reported, the information of all users is obtained, or those that meet the condition on fields of the record, if specified", ParameterType:=GetType(String))>
                        Optional EmployeeID As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional ByVal CompanyCode As String = ""
                        ) As roWSResponse(Of roEmployee())

    <OperationContract>
    <SwaggerWcfPath("Get information from one, several or all users who were modified after a date", "Get information from one or all users who were modified after a date. The information for each user may contain the list of contracts and mobilities of each user. Such information is delivered via a list of oEmployee objects.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetEmployeesByTimestamp?CompanyCode={CompanyCode}&Token={Token}&OnlyWithActiveContract={OnlyWithActiveContract}&IncludeOldData={IncludeOldData}&Timestamp={Timestamp}&FieldName={FieldName}&FieldValue={FieldValue}&EmployeeID={EmployeeID}")>
    Function GetEmployeesByTimestamp(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Get only data from users with an active contract", ParameterType:=GetType(Boolean))>
                        OnlyWithActiveContract As Boolean,
                        <SwaggerWcfParameter(Required:=True, Description:="Obtain the user's historical data regarding mobility and contracts", ParameterType:=GetType(Boolean))>
                        IncludeOldData As Boolean,
                        <SwaggerWcfParameter(Required:=True, Description:="Date and time from which users are obtained", ParameterType:=GetType(DateTime))>
                        Timestamp As DateTime,
                         <SwaggerWcfParameter(Required:=False, Description:="Name of the record field by which you want to filter the users", ParameterType:=GetType(String))>
                        Optional FieldName As String = "",
                         <SwaggerWcfParameter(Required:=False, Description:="Value of the record field by which you want to filter the users", ParameterType:=GetType(String))>
                        Optional FieldValue As String = "",
                         <SwaggerWcfParameter(Required:=False, Description:="User identifier, in case you want to obtain information from a single user. If it is not reported, the information of all users is obtained, or those that meet the condition on fields of the record, if specified", ParameterType:=GetType(String))>
                        Optional EmployeeID As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional ByVal CompanyCode As String = ""
                        ) As roWSResponse(Of roEmployee())

    <OperationContract>
    <SwaggerWcfPath("Create or update a user", "Create or update a user. The user data is indicated in the oEmployee object")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateEmployee?CompanyCode={CompanyCode}&Token={Token}")>
    Function CreateOrUpdateEmployee(
                                   <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                                   Token As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Object that models the user to be inserted or updated", ParameterType:=GetType(roDatalinkStandarEmployee))>
                                   oEmployee As roDatalinkStandarEmployee,
                                   <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   Optional ByVal CompanyCode As String = ""
                                   ) As roDatalinkStandarEmployeeResponse

    <OperationContract>
    <SwaggerWcfPath("Upload employee's photo ", " Upload employee's photo. The photo data must be on base64 format. ")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="UploadEmployeePhoto?Token={Token}&CompanyCode={CompanyCode}")>
    Function UploadEmployeePhoto(
                               <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Object that models the user photo. """, ParameterType:=GetType(roEmployeePhoto))>
                               oEmployeePhoto As roEmployeePhoto,
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roEmployeePhoto)

End Interface