Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IDocumentsService

    <OperationContract>
    <SwaggerWcfPath("Adjunta un documento de usuario al Gestor Documental", "Adjunta un documento de usuario al Gestor Documental de VisualTime. Los datos del documento se indican en el objecto oDocument. El documento debe ser de tipo ""Documentación general de usuario"" y debe existir una carpeta creada en VisualTime")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="AddDocument?UserName={UserName}&UserPwd={UserPwd}&CompanyCode={CompanyCode}")>
    Function AddDocument(
                               <SwaggerWcfParameter(Required:=True, Description:="Nombre de usuario utilizado para validarnos en el servicio", ParameterType:=GetType(String))>
                               UserName As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Contraseña utilizada para validarnos en el servicio (MD5)", ParameterType:=GetType(String))>
                               UserPwd As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Objeto que modela el documento. Sólo se aceptan documentos de tipo ""Documentación general de usuario""", ParameterType:=GetType(roDocument))>
                               oDocument As roDocument,
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roDocument)

End Interface