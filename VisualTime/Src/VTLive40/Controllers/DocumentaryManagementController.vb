Imports System.IO
Imports System.Security.Cryptography
Imports System.Web.Mvc
Imports DevExpress.DataProcessing
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.Web.Base
Imports VTLive40.Controllers.Base

<PermissionsAtrribute(FeatureAlias:="Documents", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class DocumentaryManagementController
    Inherits BaseController

#Region "Private Variables"

    Private oLanguage As roLanguageWeb

#End Region

    Public Sub New()
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveDocumentaryManagement")
        End If
    End Sub

    ''' <summary>
    '''
    ''' </summary>
    ''' <returns></returns>
    Function Index() As ActionResult

        Me.InitializeBase(CardTreeTypes.DocumentaryManagement, TabBarButtonTypes.DocumentaryManagement, "DocumentaryManagement", {"Title", "TreeTitle"}, "LiveDocumentaryManagement") _
                          .SetBarButton(BarButtonTypes.DocumentaryManagement) _
                          .SetViewInfo("LiveDocumentaryManagement", "Index", "Title", "TreeTitle", "Base/Images/StartMenuIcos/Documents.png")

        WLHelperWeb.CardListSearchFiltering = Nothing

        Return View("index")
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="templateId"></param>
    ''' <param name="searchTerm"></param>
    ''' <returns></returns>
    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Documents.DocumentsDefinition", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function GetDocuments(templateId As Integer, searchTerm As String, Optional showAll As Boolean = False) As ContentResult

        Dim response As New List(Of FileManagerDocument)

        If (templateId <> -1 Or Not String.IsNullOrEmpty(searchTerm)) Then
            Dim documents As New List(Of roDocument)
            If (templateId <> -1) Then
                documents = API.DocumentsServiceMethods.GetDocumentsByTemplateId(templateId, Nothing, False, searchTerm, showAll)
            Else
                documents = API.DocumentsServiceMethods.GetDocumentsByFilterName(searchTerm, Nothing, False)
            End If

            HttpContext.Session("LimitReached") = Web.HttpContext.Current.Session("LimitReached")

            response = documents.Where(Function(ro) String.IsNullOrWhiteSpace(searchTerm) OrElse
                                        (ro.Title.ToUpper() + ro.DocumentType.ToUpper()).Contains(searchTerm.ToUpper()) OrElse
                                        (ro.EmployeeName.ToUpper().Contains(searchTerm.ToUpper())) OrElse
                                        (String.IsNullOrEmpty(ro.EmployeeName))) _
                                .Select(Function(ro) New FileManagerDocument With {
                                                .keyExpr = $"{ro.Id}##{ro.Title}{ro.DocumentType}",
                                                .id = ro.Id,
                                                .name = ro.Title + ro.DocumentType,
                                                .size = ro.Weight,
                                                .dateModified = ro.LastStatusChange,
                                                .hasSubDirectories = False,
                                                .isDirectory = False
                                            }).ToList()

        End If

        Return New ContentResult With {.Content = roJSONHelper.SerializeNewtonSoft(response), .ContentType = "application/json"}

    End Function

    Function GetLimitReached() As Boolean
        Return HttpContext.Session("LimitReached")
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="searchTerm"></param>
    ''' <returns></returns>
    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Documents.DocumentsDefinition", Permission:=Robotics.Base.DTOs.Permission.Read)>
    Function FindAllDocuments(searchTerm As String) As JsonResult

        Dim response As New List(Of FileManagerDocument)
        If (Not String.IsNullOrEmpty(searchTerm)) Then

            Dim documents = API.DocumentsServiceMethods.GetDocumentsByFilterName(searchTerm, Nothing, False)
            WLHelperWeb.CardListSearchFiltering = documents.Select(Function(oDoc) oDoc.DocumentTemplate.Id).ToList()
        Else
            WLHelperWeb.CardListSearchFiltering = Nothing

        End If

        Return Json(response)

    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="documentId"></param>
    ''' <returns></returns>
    <HttpPost>
    Function GetDocument(<Http.FromBody()> documentId As Integer) As FileResult

        Dim oDoc As roDocument = API.DocumentsServiceMethods.GetDocumentById(Nothing, documentId, False)

        If oDoc IsNot Nothing Then
            Dim oTemplate As roDocumentTemplate = API.DocumentsServiceMethods.GetDocumentTemplateById(Nothing, oDoc.DocumentTemplate.Id, False)

            If oTemplate IsNot Nothing Then
                If oDoc.IdEmployee > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, oDoc.IdEmployee, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                    Dim document = API.DocumentsServiceMethods.GetDocumentFile(Nothing, documentId, False)
                    Return File(document.DocumentContent, "application/force-download", document.DocumentName)
                End If

                If oDoc.IdCompany > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Nothing, oDoc.IdCompany, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                    Dim document = API.DocumentsServiceMethods.GetDocumentFile(Nothing, documentId, False)
                    Return File(document.DocumentContent, "application/force-download", document.DocumentName)
                End If
            End If

        End If

        Return File({}, "application/force-download", "")

    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="templateId"></param>
    ''' <returns></returns>
    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Documents.DocumentsDefinition", Permission:=Robotics.Base.DTOs.Permission.Write)>
    Function AddDocument(templateId As String) As JsonResult

        If (Request.Files.Count > 0) Then

            Dim fi As HttpPostedFileBase = Request.Files.Get(0)

            Dim fileBytes As Byte() = New Byte(fi.ContentLength - 1) {}
            Using stream As IO.Stream = fi.InputStream
                stream.Read(fileBytes, 0, fi.ContentLength)
            End Using

            Dim maxFileSize As Integer = roTypes.Any2Integer(HelperSession.AdvancedParametersCache("VTLive.MaxAllowedFileSize"))
            If maxFileSize = 0 Then maxFileSize = 256
            Dim documentTemplate As roDocumentTemplate = API.DocumentsServiceMethods.GetTemplateDocumentsList(False, Nothing, Nothing, True).Where(Function(template) template.Id = templateId).FirstOrDefault()
            Dim isA3PayrollDocument As Boolean = (documentTemplate.Scope = DocumentScope.EmployeeContract AndAlso documentTemplate.ShortName = "A3_PR")

            If fileBytes.Length <= (maxFileSize * 1024) OrElse isA3PayrollDocument Then

                Dim document As New roDocument()
                document.Id = -1
                document.Document = fileBytes
                document.Title = IO.Path.GetFileNameWithoutExtension(fi.FileName)
                document.Weight = fileBytes.Length
                document.DeliveredBy = WLHelperWeb.CurrentPassport.Name

                document.DeliveryChannel = "VisualTimeLive"
                document.DeliveredDate = DateTime.Now
                document.EndDate = DateTime.Now
                document.DocumentType = IO.Path.GetExtension(fi.FileName)
                document.DocumentTemplate = documentTemplate

                If (Not Helpers.Constants.ExcludedFileExtensions.Contains(document.DocumentType)) Then

                    If (document.DocumentTemplate IsNot Nothing) Then
                        If Not (document.DocumentTemplate.Scope = DocumentScope.LeaveOrPermission OrElse document.DocumentTemplate.Scope = DocumentScope.CauseNote) Then

                            Dim listDocuments As List(Of roDocument) = New List(Of roDocument)

                            ' Si es una nómina de A3, debo partir el documento, y guardar cada uno de los documentos por separado
                            If isA3PayrollDocument Then
                                listDocuments = API.DocumentsServiceMethods.SplitA3PayrollDocument(Nothing, document, False)
                                Select Case roWsUserManagement.SessionObject().States.DocumentState.Result
                                    Case DocumentResultEnum.InvalidA3PayrollFormat
                                        Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.BadRequest, Me.oLanguage.Translate("Index.InvalidA3PayrollFormat", String.Empty)))
                                End Select
                            Else
                                listDocuments.Add(document)
                            End If

                            Dim errorText As String = String.Empty
                            Dim statusText As String = String.Empty
                            Dim statusResume As New Dictionary(Of DocumentResultEnum, List(Of Integer))
                            Dim totalOk As Integer = 0
                            Dim totalKo As Integer = 0
                            Dim position As Integer = 0
                            For Each singledocument In listDocuments
                                position += 1
                                singledocument.BeginDate = singledocument.DocumentTemplate.BeginValidity
                                singledocument.EndDate = singledocument.DocumentTemplate.EndValidity
                                singledocument.LastStatusChange = DateTime.Now

                                Dim savedDocument = API.DocumentsServiceMethods.SaveDocument(singledocument, Nothing, True)

                                ' Para A3, si existía el documento, lo borro ahora (si el guardado fue correcto)
                                If isA3PayrollDocument AndAlso savedDocument IsNot Nothing AndAlso savedDocument.Id > 0 Then
                                    Dim existingDocument = API.DocumentsServiceMethods.CheckIfEmployeeDocumentExists(Nothing, savedDocument, False)
                                    If existingDocument IsNot Nothing Then
                                        API.DocumentsServiceMethods.DeleteDocument(Nothing, existingDocument.Id, False)
                                    End If
                                End If

                                Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
                                Dim oState As roWsState = oSession.States.DocumentState

                                If (oState.Result <> DocumentResultEnum.NoError) Then
                                    Select Case oState.Result
                                        Case DocumentResultEnum.EmployeeRequired
                                            If isA3PayrollDocument Then
                                                If statusResume.ContainsKey(DocumentResultEnum.EmployeeRequired) Then
                                                    statusResume(DocumentResultEnum.EmployeeRequired).Add(position)
                                                Else
                                                    statusResume.Add(DocumentResultEnum.EmployeeRequired, New List(Of Integer) From {position})
                                                End If
                                            Else
                                                errorText = Me.oLanguage.Translate("Index.EmployeeRequired", String.Empty)
                                            End If
                                        Case DocumentResultEnum.EmployeeRequiredPatternAlt1
                                            errorText = Me.oLanguage.Translate("Index.EmployeeRequiredPatternAlt1", String.Empty)
                                        Case DocumentResultEnum.ContractRequired
                                            If isA3PayrollDocument Then
                                                If statusResume.ContainsKey(DocumentResultEnum.ContractRequired) Then
                                                    statusResume(DocumentResultEnum.ContractRequired).Add(position)
                                                Else
                                                    statusResume.Add(DocumentResultEnum.ContractRequired, New List(Of Integer) From {position})
                                                End If
                                            Else
                                                errorText = Me.oLanguage.Translate("Index.ContractRequired", String.Empty)
                                            End If
                                        Case DocumentResultEnum.CompanyRequired
                                            errorText = Me.oLanguage.Translate("Index.CompanyRequired", String.Empty)
                                        Case DocumentResultEnum.NoPermissionOverEmployee
                                            If isA3PayrollDocument Then
                                                If statusResume.ContainsKey(DocumentResultEnum.NoPermissionOverEmployee) Then
                                                    statusResume(DocumentResultEnum.NoPermissionOverEmployee).Add(position)
                                                Else
                                                    statusResume.Add(DocumentResultEnum.NoPermissionOverEmployee, New List(Of Integer) From {position})
                                                End If
                                            Else
                                                errorText = Me.oLanguage.Translate("Index.NoPermissionOverEmployee", String.Empty)
                                            End If
                                        Case DocumentResultEnum.NoPermissionOverGroup
                                            errorText = Me.oLanguage.Translate("Index.NoPermissionOverGroup", String.Empty)
                                        Case DocumentResultEnum.PDFDocumentRequired
                                            errorText = Me.oLanguage.Translate("Index.PDFDocumentRequired", String.Empty)
                                        Case DocumentResultEnum.NumberOfSignedDocumentsExceeded
                                            errorText = Me.oLanguage.Translate("Index.NumberOfSignedDocumentsExceeded", String.Empty)
                                        Case Else
                                            errorText = "Error no controlado."
                                    End Select
                                    totalKo += 1
                                Else
                                    totalOk += 1
                                End If
                            Next

                            If isA3PayrollDocument Then
                                ' Nómina de A3. Si se superó la validación de formato, siempre retorno estado OK, con resumen de lo ocurrido con los distintos documentos de nómina
                                ' Nóminas correctas
                                Dim oparams As New List(Of String)
                                Select Case totalOk
                                    Case 0
                                        statusText = Me.oLanguage.Translate("Index.A3PayrollCorrect.noone", "", oparams)
                                    Case 1
                                        oparams.Add(totalOk)
                                        statusText = Me.oLanguage.Translate("Index.A3PayrollCorrect.one", "", oparams)
                                    Case Else
                                        oparams.Add(totalOk)
                                        statusText = Me.oLanguage.Translate("Index.A3PayrollCorrect.many", "", oparams)
                                End Select

                                ' Nóminas no importadas por falta de empleado
                                If statusResume.ContainsKey(DocumentResultEnum.EmployeeRequired) AndAlso statusResume(DocumentResultEnum.EmployeeRequired).Any Then
                                    oparams = New List(Of String)
                                    oparams.Add(statusResume(DocumentResultEnum.EmployeeRequired).Count)

                                    Dim pagesDetail As String = String.Join("<br>", statusResume(DocumentResultEnum.EmployeeRequired).
                                      Select(Function(x, i) New With {.Valor = x, .Index = i}).
                                      GroupBy(Function(item) item.Index \ 50).
                                      Select(Function(grupo) String.Join(",", grupo.Select(Function(item) item.Valor))))

                                    oparams.Add("<br>" & pagesDetail)
                                    oparams.Add(document.Title)
                                    statusText = statusText & "<br>" & Me.oLanguage.Translate("Index.a3payrollunknownemployee", "", oparams)

                                    ' Auditoría
                                    Dim lstFileParameterNames As New List(Of String)
                                    Dim lstFileParameterValues As New List(Of String)
                                    lstFileParameterNames.Add("{message}")
                                    lstFileParameterValues.Add(statusText.Replace("<br>", vbCrLf))
                                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tA3PayrollImport, document.DocumentTemplate.Name, lstFileParameterNames, lstFileParameterValues, Nothing)
                                End If

                                ' Nóminas no importadas por falta de permisos
                                If statusResume.ContainsKey(DocumentResultEnum.NoPermissionOverEmployee) AndAlso statusResume(DocumentResultEnum.NoPermissionOverEmployee).Any Then
                                    oparams = New List(Of String)
                                    oparams.Add(statusResume(DocumentResultEnum.NoPermissionOverEmployee).Count)

                                    Dim pagesDetail As String = String.Join("<br>", statusResume(DocumentResultEnum.NoPermissionOverEmployee).
                                      Select(Function(x, i) New With {.Valor = x, .Index = i}).
                                      GroupBy(Function(item) item.Index \ 50).
                                      Select(Function(grupo) String.Join(",", grupo.Select(Function(item) item.Valor))))

                                    oparams.Add("<br>" & pagesDetail)
                                    oparams.Add(document.Title)
                                    statusText = statusText & "<br>" & Me.oLanguage.Translate("Index.a3payrollunauthorizedemployee", "", oparams)

                                    ' Auditoría
                                    Dim lstFileParameterNames As New List(Of String)
                                    Dim lstFileParameterValues As New List(Of String)
                                    lstFileParameterNames.Add("{message}")
                                    lstFileParameterValues.Add(statusText.Replace("<br>", vbCrLf))
                                    API.AuditServiceMethods.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tA3PayrollImport, document.DocumentTemplate.Name, lstFileParameterNames, lstFileParameterValues, Nothing)
                                End If

                                ' Nóminas no importadas por falta de contrato
                                If statusResume.ContainsKey(DocumentResultEnum.ContractRequired) AndAlso statusResume(DocumentResultEnum.ContractRequired).Any Then
                                    oparams = New List(Of String)
                                    oparams.Add(statusResume(DocumentResultEnum.ContractRequired).Count)
                                    oparams.Add(String.Join(",", statusResume(DocumentResultEnum.ContractRequired)))
                                    statusText = statusText & "<br>" & Me.oLanguage.Translate("Index.a3payrollnocontractemployee", "", oparams)
                                End If

                                Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.OK, statusText))
                            Else
                                ' Cualquier documento que no sea nomina de A3
                                If errorText = String.Empty Then
                                    Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.OK, ""))
                                Else
                                    Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.Conflict, errorText))
                                End If
                            End If
                        Else
                            Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.BadRequest, Me.oLanguage.Translate("Index.CauseNoteOrLeaveOrPermission", String.Empty)))
                        End If
                    Else
                        Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.BadRequest, Me.oLanguage.Translate("Index.NoDocumentTemplate", String.Empty)))
                    End If
                Else
                    Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.BadRequest, Me.oLanguage.Translate("Index.ExcludedFileExtensions", String.Empty)))
                End If
            Else
                Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.BadRequest, String.Concat(Me.oLanguage.Translate("Index.DocumentMaxSizeFile", String.Empty), " Max: ", (maxFileSize / 1024), "MB")))
            End If
        Else
            Return Json(New HttpStatusCodeResult(Net.HttpStatusCode.BadRequest, "No File Data"))
        End If
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="documentId"></param>
    ''' <returns></returns>
    <HttpPost>
    Function RemoveDocument(documentId As Integer) As ActionResult

        Dim oDoc As roDocument = API.DocumentsServiceMethods.GetDocumentById(Nothing, documentId, False)

        If oDoc IsNot Nothing Then
            Dim oTemplate As roDocumentTemplate = API.DocumentsServiceMethods.GetDocumentTemplateById(Nothing, oDoc.DocumentTemplate.Id, False)

            If oTemplate IsNot Nothing Then
                If oDoc.IdEmployee > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, oDoc.IdEmployee, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                    If API.DocumentsServiceMethods.DeleteDocument(Nothing, documentId, True) Then Return New HttpStatusCodeResult(Net.HttpStatusCode.OK)
                End If

                If oDoc.IdCompany > 0 AndAlso API.SecurityServiceMethods.HasPermissionOverGroupAppAlias(Nothing, oDoc.IdCompany, "Employees", "U", Robotics.Base.DTOs.Permission.Read) Then
                    If API.DocumentsServiceMethods.DeleteDocument(Nothing, documentId, True) Then Return New HttpStatusCodeResult(Net.HttpStatusCode.OK)
                End If
            End If

        End If

        Return New HttpStatusCodeResult(Net.HttpStatusCode.InternalServerError)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    ''' <param name="documentId"></param>
    ''' <returns></returns>
    <HttpPost>
    <PermissionsAtrribute(FeatureAlias:="Documents.DocumentsDefinition", Permission:=Robotics.Base.DTOs.Permission.Write)>
    Function RemoveDocumentTemplate(templateId As Integer) As ActionResult

        Dim errorMsg As String = String.Empty

        Dim oDocumentTemplate = API.DocumentsServiceMethods.GetDocumentTemplateById(Nothing, templateId, False)
        Dim resultOk = API.DocumentsServiceMethods.DeleteDocumentTemplate(oDocumentTemplate, Nothing, True)

        Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        Dim oState As roWsState = oSession.States.DocumentState

        If (oState.Result <> DocumentResultEnum.NoError) Then
            Select Case oState.Result
                Case DocumentResultEnum.TemplateInUse
                    errorMsg = Me.oLanguage.Translate("Index.TemplateInUse", String.Empty)
            End Select
        End If

        If resultOk Then
            Return New HttpStatusCodeResult(Net.HttpStatusCode.OK)
        Else
            Return New HttpStatusCodeResult(Net.HttpStatusCode.InternalServerError, errorMsg)
        End If
    End Function

    <HttpGet>
    Function GetAvailableBioCertificates() As JsonResult
        Dim documents As List(Of roDocument) = API.DocumentsServiceMethods.GetSystemDocumentList(DocumentScope.BioCertificate, Nothing, False)
        documents = documents.OrderByDescending(Function(x) x.DeliveredDate).ToList()

        Return Json(documents.ToArray, JsonRequestBehavior.AllowGet)
    End Function

    ''' <summary>
    '''
    ''' </summary>
    Public Class FileManagerDocument
        Public keyExpr As String
        Public id As Integer
        Public name As String
        Public size As Integer
        Public dateModified As DateTime
        Public thumbnail As String
        Public isDirectory As Boolean
        Public hasSubDirectories As Boolean
    End Class

End Class