Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IAccrualsService

    <OperationContract>
    <SwaggerWcfPath("Obtiene los saldos de uno o todos los usuarios en un periodo", "Obtiene el valor diario de los saldos para el usuario o usuarios en el periodo seleccionado. Se consideran únicamente los saldos configurados en VisualTime como exportables. El perido máximo es de 366 días")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetAccruals?UserName={UserName}&UserPwd={UserPwd}&StartDate={StartDate}&EndDate={EndDate}&Employee={Employee}&CompanyCode={CompanyCode}")>
    Function GetAccruals(
                        <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                        UserName As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                        UserPwd As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Initial date", ParameterType:=GetType(Date))>
                        StartDate As Date,
                        <SwaggerWcfParameter(Required:=True, Description:="Final date", ParameterType:=GetType(Date))>
                        EndDate As Date,
                        <SwaggerWcfParameter(Required:=False, Description:="Identificador del usuario del que se recuperarán los saldos. Si no se informa, se recuperan los saldos de todos los usuarios", ParameterType:=GetType(String))>
                        Optional Employee As String = "",
                        <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                        Optional CompanyCode As String = "") As roWSResponse(Of roAccrual())

End Interface