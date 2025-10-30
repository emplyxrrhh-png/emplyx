Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IEmployeesService

    <OperationContract>
    <SwaggerWcfPath("Obtiene información de uno, varios o todos los usuarios", "Obtiene información de uno o todos los usuarios. La información para cada usuario puede contener la lista de contratos y movilidades de cada usuario. Dicha información se entrega mediante una lista de objetos oEmployee")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetEmployees?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}&OnlyWithActiveContract={OnlyWithActiveContract}&IncludeOldData={IncludeOldData}&FieldName={FieldName}&FieldValue={FieldValue}&EmployeeID={EmployeeID}")>
    Function GetEmployees(
                        <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Obtener solo datos de usuarios con contrato activo", ParameterType:=GetType(Boolean))>
                        OnlyWithActiveContract As Boolean,
                        <SwaggerWcfParameter(Required:=True, Description:="Obtener los datos históricos del usuario referentes a movilidades y contratos", ParameterType:=GetType(Boolean))>
                        IncludeOldData As Boolean,
                         <SwaggerWcfParameter(Required:=False, Description:="Nombre del campo de la ficha por el cual se quieren filtrar los usuarios", ParameterType:=GetType(String))>
                        Optional FieldName As String = "",
                         <SwaggerWcfParameter(Required:=False, Description:="Valor del campo de la ficha por el cual se quieren filtrar los usuarios", ParameterType:=GetType(String))>
                        Optional FieldValue As String = "",
                         <SwaggerWcfParameter(Required:=False, Description:="Identificador del usuario, en el caso que se quiera obtener información de un único usuario. Si no se informa, se obtiene la información de todos los usuarios, o los que cumplan la condición sobre campos de la ficha, si se especificó", ParameterType:=GetType(String))>
                        Optional EmployeeID As String = ""
                        ) As roWSResponse(Of roEmployee())

    <OperationContract>
    <SwaggerWcfPath("Crea o actualiza un usuario", "Crea o actualiza un usuario. Los datos del usuario se indican en el objecto oEmployee")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateEmployee?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}")>
    Function CreateOrUpdateEmployee(
                                   <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                                   CompanyCode As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                                   UserName As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                                   UserPwd As String,
                                   <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela el usuario que va a ser insertado o actualizado", ParameterType:=GetType(roDatalinkStandarEmployee))>
                                   oEmployee As roDatalinkStandarEmployee) As roDatalinkStandarEmployeeResponse

End Interface