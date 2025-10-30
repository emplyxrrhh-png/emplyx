Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IGroupsService

    <OperationContract>
    <SwaggerWcfPath("Obtiene la lista de grupos", "Obtiene la lista de grupos")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetGroups?CompanyCode={CompanyCode}&Root={Root}&IncludeEmployees={IncludeEmployees}&UserName={UserName}&UserPwd={UserPwd}")>
    Function GetGroups(
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Indica si se deben incluir o no los usuarios en la estructura de grupos", ParameterType:=GetType(Boolean))>
                        IncludeEmployees As Boolean,
                        <SwaggerWcfParameter(Required:=False, Description:="Identificador interno del grupo raíz que se quiere recuperar, cuando la estructura de grupos tiene varias raíces. Si no se informa, se obtiene la estructura completa", ParameterType:=GetType(String))>
                        Optional Root As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roGroup())

End Interface