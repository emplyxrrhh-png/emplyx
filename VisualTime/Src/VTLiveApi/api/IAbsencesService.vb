Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IAbsencesService

    <SwaggerWcfPath("Obtiene la lista de ausencias de uno o todos lo usuarios en un periodo", "Obtiene la lista de ausencias de uno o todos lo usuarios en un periodo")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetAbsences?UserName={UserName}&UserPwd={UserPwd}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&Criteria={Criteria}&CompanyCode={CompanyCode}")>
    <OperationContract>
    Function GetAbsences(
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Fecha inicial del periodo", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Fecha final del periodo", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identificador del usuario del que se recuperarán las ausencias. Si no se informa, se recuperan las ausencias de todos los usuarios", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Criterio con el que se recuperan las ausencias. Por defecto, se recuperan aquellas cuyas fechas estén comprendidas en el periodo indicado. Si informamos ""timestamp"", se recuperan las ausencias que hayan sido modificadas en el periodo indicado, independientemente de las fechas de la ausencia", ParameterType:=GetType(String))>
                        Optional Criteria As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roAbsence())

    <OperationContract>
    <SwaggerWcfPath("Crea o actualiza una ausencia de un usuario", "Crea o actualiza una ausencia de un usuario. Los parámetros de la ausencia se indican en el objeto oAbsence")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="CreateOrUpdateAbsence?UserName={UserName}&UserPwd={UserPwd}&CompanyCode={CompanyCode}")>
    Function CreateOrUpdateAbsence(
                               <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                               UserName As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                               UserPwd As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela la ausencia", ParameterType:=GetType(roAbsence))>
                               oAbsence As roAbsence,
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roAbsence())

End Interface