Imports System.ServiceModel
Imports System.ServiceModel.Web
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports SwaggerWcf.Attributes

<ServiceContract>
Public Interface IDocumentsService_v2

    <OperationContract>
    <SwaggerWcfPath("Attaches a user document to the document manager ", " Attach a user document to the VisualTime Document Manager. The document data is indicated in the Object Obocument. The document should be of type "" General User Documentation "" and there must be a folder created in VisualTime")>
    <WebInvoke(Method:="POST", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="AddDocument?Token={Token}&CompanyCode={CompanyCode}")>
    Function AddDocument(
                               <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="Object that models the document. Only documents of type "" General User Documentation""", ParameterType:=GetType(roDocument))>
                               oDocument As roDocument,
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roDocument)

    <OperationContract>
    <SwaggerWcfPath("Get the documents of one or all users in a period.", "Retrieve the documents for the selected user or users. The request is limited to a maximum of 10 items in the response.")>
    <WebInvoke(Method:="GET", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="GetDocuments?Token={Token}&Type={Type}&Title={Title}&Employee={Employee}&Company={Company}&Timestamp={Timestamp}&UpdateType={UpdateType}&Extension={Extension}&Template={Template}&CompanyCode={CompanyCode}")>
    Function GetDocuments(
                        <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                        Token As String,
                        <SwaggerWcfParameter(Required:=True, Description:="Type of documents to recover. Type can be employee or company.", ParameterType:=GetType(String))>
Type As String,
<SwaggerWcfParameter(Required:=False, Description:="Word/s contained in the title of the documents that you want to recover.", ParameterType:=GetType(String))>
Optional Title As String = "",
<SwaggerWcfParameter(Required:=False, Description:="Identifier of the user from which the documents will be recovered. If you are not informed, the documents of all users recover. Can also indicate multiple user identifiers separated by ;", ParameterType:=GetType(String))>
Optional Employee As String = "",
<SwaggerWcfParameter(Required:=False, Description:="Company from which you want to obtain the documents", ParameterType:=GetType(String))>
Optional Company As String = "",
<SwaggerWcfParameter(Required:=False, Description:="Timestamp from which the document was created/modified/signed", ParameterType:=GetType(Date))>
Optional Timestamp As DateTime = Nothing,
<SwaggerWcfParameter(Required:=False, Description:="Update type: StatusChanged, Signed, Delivered. If the timestamp is indicated, it will be applied to the type of update indicated.", ParameterType:=GetType(String))>
Optional UpdateType As String = "",
<SwaggerWcfParameter(Required:=False, Description:="Extension of the files you want to recover", ParameterType:=GetType(String))>
Optional Extension As String = "",
<SwaggerWcfParameter(Required:=False, Description:="Document template name", ParameterType:=GetType(String))>
Optional Template As String = "",
<SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
Optional CompanyCode As String = ""
) As roWSResponse(Of roDocument())

    <OperationContract>
    <SwaggerWcfPath("Deletes a document from Document Manager ", "Deletes a document from document manager. Document is identified by its externalId. Only documents added through API can be deleted")>
    <WebInvoke(Method:="DELETE", RequestFormat:=WebMessageFormat.Json, ResponseFormat:=WebMessageFormat.Json, BodyStyle:=WebMessageBodyStyle.Bare, UriTemplate:="DeleteDocument?Token={Token}&ExternalId={ExternalId}&CompanyCode={CompanyCode}")>
    Function DeleteDocument(
                               <SwaggerWcfParameter(Required:=True, Description:="Security token", ParameterType:=GetType(String))>
                               Token As String,
                               <SwaggerWcfParameter(Required:=True, Description:="ExternalId of document to be deleted", ParameterType:=GetType(String))>
                               ExternalId As String,
                               <SwaggerWcfParameter(Required:=False, Description:="Company identifier. Only for multitenant environments", ParameterType:=GetType(String))>
                               Optional CompanyCode As String = "") As roWSResponse(Of roDocumentResponse)


End Interface