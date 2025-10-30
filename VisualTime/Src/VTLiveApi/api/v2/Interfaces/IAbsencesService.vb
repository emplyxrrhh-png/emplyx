Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IAbsencesService_v2

    <SwaggerWcfPath("Get the list of absences from one or all of the users in a period", "Gets the list of absences from one or all the users in a period")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetAbsences?Token={Token}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&Criteria={Criteria}&CompanyCode={CompanyCode}")>
    <OperationContract>
    Function GetAbsences(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date of the period", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="End date of the period", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from which the absences will be recovered. If you are not informed, the absences of all users are recovered", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Criterion with which the absences are recovered. By default, those whose dates are included in the indicated period are recovered. If we inform "" Timestamp "", the absences that have been modified in the indicated period are recovered, regardless of the dates of the absence", ParameterType:=GetType(String))>
                        Optional Criteria As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roAbsence())

    <OperationContract>
    <SwaggerWcfPath("Create or update an absence of a user ", " creates or updates an absence of a user. The parameters of the absence are indicated in the OABSENCE object")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateAbsence?Token={Token}&CompanyCode={CompanyCode}")>
    Function CreateOrUpdateAbsence(
                               <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Object that models absence", ParameterType:=GetType(roAbsence))>
                               oAbsence As roAbsence,
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roAbsence())

End Interface