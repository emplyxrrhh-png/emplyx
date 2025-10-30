Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IPunchesService

    <OperationContract>
    <SwaggerWcfPath("Obtiene los fichajes registrados o modificados desde una fecha y hora", "Obtiene la lista de fichajes registrados o modificados desde una fecha y hora")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetPunches?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}&Timestamp={Timestamp}")>
    Function GetPunches(
                         <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Fecha y hora a partir del cual se obtienen los fichajes", ParameterType:=GetType(DateTime))>
                        Timestamp As DateTime
                        ) As roWSResponse(Of roPunch())

    <OperationContract>
    <SwaggerWcfPath("Obtiene los fichajes realizados en un periodo", "Obtiene los fichajes realizados en un periodo, independientemente de cuándo se registrasen o modificasen en el sistema")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetPunchesBetweenDates?CompanyCode={CompanyCode}&UserName={UserName}&UserPwd={UserPwd}&StartDate={StartDate}&EndDate={EndDate}")>
    Function GetPunchesBetweenDates(
                         <SwaggerWcfParameter(Required:=True, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        CompanyCode As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Fecha y hora inicial", ParameterType:=GetType(Date))>
                        StartDate As Date,
                         <SwaggerWcfParameter(Required:=True, Description:="Fecha y hora final", ParameterType:=GetType(Date))>
                        EndDate As Date
                        ) As roWSResponse(Of roPunch())

    <OperationContract>
    <SwaggerWcfPath("Registra una lista fichajes de usuarios", "Incorpora fichajes de uno o varios usuarios. Los detalles de cada fichaje de la lista se detallan en un objeto oPunch")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="AddPunches?UserName={UserName}&UserPwd={UserPwd}&CompanyCode={CompanyCode}")>
    Function AddPunches(
                               <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                               UserName As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                               UserPwd As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela el array de fichajes a incorporar", ParameterType:=GetType(roPunch()))>
                               oPunches As roPunch(),
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roPunchesResponse())

End Interface