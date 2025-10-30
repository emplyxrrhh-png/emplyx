Imports System.IO
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports Newtonsoft.Json.Linq
Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTDocuments
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.Web.Base
Imports Robotics.Web.Base.API

<PermissionsAtrribute(FeatureAlias:="Employees.Communiques", Permission:=Robotics.Base.DTOs.Permission.Write)>
<LoggedInAtrribute(Requiered:=True)>
Public Class CommuniqueController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    Public Function ScriptsVersion() As String
        Return "?v=" & Robotics.VTBase.roTypes.Any2String(Reflection.Assembly.GetExecutingAssembly().GetCustomAttributes(GetType(Reflection.AssemblyDescriptionAttribute), False).FirstOrDefault().Description)
    End Function

    Function Index() As ActionResult
        Try
            LoadInitialData()
            ViewBag.RequieredFeature = ""

            Dim oLicSupport As New roLicenseSupport()
            Dim oLicInfo As roVTLicense = oLicSupport.GetVTLicenseInfo()

            If oLicInfo.Edition = roServerLicense.roVisualTimeEdition.NotSet Then
                ViewBag.AdvancedCommuniques = True
            Else
                If (oLicInfo.Edition = roServerLicense.roVisualTimeEdition.Advanced Or oLicInfo.Edition = roServerLicense.roVisualTimeEdition.Premium) Then
                    ViewBag.AdvancedCommuniques = HelperSession.GetFeatureIsInstalledFromApplication("Feature\AdvancedCommuniques")
                Else
                    ViewBag.AdvancedCommuniques = False
                End If

            End If

            'EDITIONS2022 ViewBag.AdvancedCommuniques = HelperSession.GetFeatureIsInstalledFromApplication("Feature\AdvancedCommuniques")
        Catch ex As Exception
        End Try
        Return View("index", GetCommuniqueDataView())
    End Function

    Function CardView() As PartialViewResult
        Return PartialView("cardView", GetCommuniqueDataView())
    End Function

    Function GetCommuniqueDataView() As roCommunique()
        Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()
        Dim data As roCommunique() = CommuniqueSvc.GetAllCommuniques(Nothing)

        Return data
    End Function

    <HttpPost>
    Function GetViewTemplate(templateName As String) As ActionResult
        If templateName = "selectorEmployees" Then
            Return PartialView(templateName, Nothing)
        Else
            Return PartialView(templateName, {}) ' New ReportsController().GetReportsDataView())
        End If
    End Function

    <HttpPost>
    Function GetCommunique(Id As Integer) As String
        Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()
        Dim comm As roCommuniqueWithStatistics = CommuniqueSvc.GetCommuniqueStatus(Nothing, Id)
        If comm IsNot Nothing AndAlso comm.Communique.CreatedBy.IdPassport = WLHelperWeb.CurrentPassportID Then
            If HttpContext IsNot Nothing AndAlso HttpContext.Session IsNot Nothing Then
                If comm.Communique.Documents.Length > 0 Then
                    HttpContext.Session("Current_Document_Save") = comm.Communique.Documents
                Else
                    HttpContext.Session("Current_Document_Save") = Nothing
                End If
            End If
        Else
            comm = New roCommuniqueWithStatistics() With {.Communique = New roCommunique, .EmployeeCommuniqueStatus = {}}
        End If
        Return roJSONHelper.SerializeNewtonSoft(comm)
    End Function

    <HttpPost>
    Function GetAllCommuniques() As String
        Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()
        Dim data As roCommunique() = CommuniqueSvc.GetAllCommuniques(Nothing)

        Return roJSONHelper.SerializeNewtonSoft(data)
    End Function

    <HttpPost>
    Function GetPermisionOverCommuniques() As String
        Dim Permission = SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Employees.Communiques", "U")
        Return roJSONHelper.SerializeNewtonSoft(Permission)
    End Function

    <HttpPost>
    Function GetPermissionToAttachDocuments() As String
        Dim oDocState As New VTDocuments.roDocumentState()
        Dim oDocumentManager As New VTDocuments.roDocumentManager(oDocState)
        Dim allowedAccessLevels = oDocumentManager.GetAllowedDocumentLOPDAccessLevels(WLHelperWeb.CurrentPassportID)
        Dim allowedDocumentAreas = oDocumentManager.GetAllowedDocumentAreas(WLHelperWeb.CurrentPassportID)

        Dim hasAttachmentPermission = allowedAccessLevels.Any(Function(level) level >= 1) AndAlso allowedDocumentAreas.Any(Function(area) area >= 1)

        Return roJSONHelper.SerializeNewtonSoft(hasAttachmentPermission)
    End Function

    <HttpPost>
    Function SaveAttachedFile(file As HttpPostedFileBase) As String
        Dim httpRequest = HttpContext.Request
        Dim oRes As roCommuniqueResponse = Nothing
        Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()

        If file IsNot Nothing AndAlso file.ContentLength > 0 Then
            Try
                Dim commId = Robotics.VTBase.roTypes.Any2Integer(httpRequest.Path.Substring(httpRequest.Path.LastIndexOf("/") + 1))
                Dim comm As roCommuniqueWithStatistics = CommuniqueSvc.GetCommuniqueStatus(Nothing, commId)

                If Not (commId = -1 OrElse (comm IsNot Nothing AndAlso comm.Communique.CreatedBy.IdPassport = WLHelperWeb.CurrentPassportID)) Then
                    oRes = New roCommuniqueResponse()
                Else
                    Dim tmpFiles As roDocument() = HttpContext.Session("Current_Document_Save")
                    If tmpFiles Is Nothing Then tmpFiles = {}
                    Dim target As MemoryStream = New MemoryStream()
                    file.InputStream.CopyTo(target)

                    Dim docTemplateList As List(Of roDocumentTemplate) = DocumentsServiceMethods.GetTemplateDocumentsList(True, DocumentScope.Communique, Nothing, False)
                    Dim docTemplate As roDocumentTemplate = docTemplateList.Find(Function(x) x.IsSystem = True AndAlso x.Scope = DocumentScope.Communique)
                    Dim newDoc As roDocument = New roDocument With {
                                                    .IdCommunique = commId,
                                                    .Document = target.ToArray(),
                                                    .Title = file.FileName.Replace(file.FileName.Substring(file.FileName.LastIndexOf(".")), ""),
                                                    .DocumentType = file.FileName.Substring(file.FileName.LastIndexOf(".")),
                                                    .DocumentTemplate = docTemplate,
                                                    .DeliveryChannel = "VisualTimeLive",
                                                    .DeliveredDate = DateTime.Now,
                                                    .BeginDate = DateSerial(1970, 1, 1),
                                                    .EndDate = DateSerial(2079, 1, 1),
                                                    .LastStatusChange = DateTime.Now,
                                                    .DeliveredBy = WLHelperWeb.CurrentPassport.Name,
                                                    .Id = -1
                                                }

                    Dim DocsList = New List(Of roDocument)
                    DocsList.AddRange(tmpFiles)
                    DocsList.Add(newDoc)

                    While DocsList.Count > 2
                        DocsList.RemoveAt(0)
                    End While

                    'comm.Documents = DocsList.ToArray

                    HttpContext.Session("Current_Document_Save") = DocsList.ToArray

                End If
            Catch ex As Exception
                Return roJSONHelper.SerializeNewtonSoft(oRes)
            End Try

            Return roJSONHelper.SerializeNewtonSoft(oRes)
        End If
        Return "False"
    End Function

    <HttpPost>
    Function RemoveAttachedFile(documentTitle As String) As Boolean
        Try
            Dim tmpFiles As roDocument() = HttpContext.Session("Current_Document_Save")
            Dim filesList As List(Of roDocument) = tmpFiles.ToList

            Dim index As Integer = filesList.ToList.FindIndex(Function(f) f.Title.Equals(documentTitle.Substring(0, documentTitle.LastIndexOf("."))))

            If index >= 0 Then filesList.RemoveAt(index)

            HttpContext.Session("Current_Document_Save") = filesList.ToArray
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    <HttpPost>
    Function CleanAttachedFilesFromSession() As Boolean
        HttpContext.Session("Current_Document_Save") = Nothing
        If HttpContext.Session("Current_Document_Save") Is Nothing Then
            Return True
        End If
        Return False
    End Function

    <HttpPost>
    Function GetEmployeesFromSelectorOutput(employees As String)
        Dim employeesArr() As String = employees.Split("@")
        Dim sentence = GetSentenceFromSelectedEmployees(employeesArr(0), employeesArr(1), employeesArr(2))
        Return sentence
    End Function

    <HttpPost>
    Function GetCurrentEmployeePassport() As String
        Dim Passport = API.UserAdminServiceMethods.GetPassport(Nothing, WLHelperWeb.CurrentPassportID, LoadType.Passport)
        Return roJSONHelper.SerializeNewtonSoft(Passport)
    End Function

    Function CountCommuniques() As Object
        Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()
        Dim data As roCommunique() = CommuniqueSvc.GetAllCommuniques(Nothing)

        Dim ArchivedComuniques = data.Where(Function(comm) comm.Archived)
        Dim ActiveComuniques = data.Where(Function(comm) Not comm.Archived)
        Dim ActiveCommDocumentsWeight As Integer = 0
        Dim ArchivedCommDocumentsWeight As Integer = 0

        For Each comm In ActiveComuniques
            For Each doc In comm.Documents
                ActiveCommDocumentsWeight = ActiveCommDocumentsWeight + doc.Weight
            Next
        Next

        For Each comm In ArchivedComuniques
            For Each doc In comm.Documents
                ArchivedCommDocumentsWeight = ArchivedCommDocumentsWeight + doc.Weight
            Next
        Next

        Dim res = New Dictionary(Of String, Double) From {
            {"ArchivedAmount", ArchivedComuniques.Count},
            {"ActiveAmount", ActiveComuniques.Count},
            {"ActiveWeight", Math.Round(ActiveCommDocumentsWeight / 1024 / 1024, 2, MidpointRounding.AwayFromZero)},
            {"ArchivedWeight", Math.Round(ArchivedCommDocumentsWeight / 1024 / 1024, 2, MidpointRounding.AwayFromZero)}
        }

        Return res

    End Function

    Public Function GetServerLanguage() As roLanguageWeb
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveCommunique")
        End If
        Return Me.oLanguage
    End Function

    <HttpPost>
    <ValidateInput(False)>
    Function SaveCommunique(communique As String) As String
        Dim httpRequest = HttpContext
        Dim oRes As Object
        Try
            Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()
            Dim beginOfTimes As DateTime = New DateTime(1970, 1, 1, 0, 0, 0, 0)
            Dim comm As JToken = JObject.Parse(communique)
            Dim Passport As roPassport = API.UserAdminServiceMethods.GetPassport(Nothing, WLHelperWeb.CurrentPassportID, LoadType.Passport)
            Dim PassportWithPhoto As roPassportWithPhoto = New roPassportWithPhoto() With {.EmployeeName = Passport.Name, .IdEmployee = Passport.ID, .IdPassport = Passport.ID, .PassportName = Passport.Name, .EmployeePhoto = ""}

            Dim FormatedCommunique As roCommunique = roJSONHelper.DeserializeNewtonSoft(communique, GetType(roCommunique))

            If FormatedCommunique.Documents.Length > 0 Then
                FormatedCommunique.Documents = HttpContext.Session("Current_Document_Save")
            Else
                FormatedCommunique.Documents = {}
            End If

            HttpContext.Session("Current_Document_Save") = Nothing

            oRes = CommuniqueSvc.CreateOrUpdateCommunique(Nothing, FormatedCommunique)
        Catch ex As Exception
            oRes = False
        End Try

        Return roJSONHelper.SerializeNewtonSoft(oRes)
    End Function

    <HttpPost>
    Function DeleteCommunique(IdCommunique As Integer) As Boolean
        Dim oRes As Boolean

        Try
            Dim CommuniqueSvc As CommuniqueServiceMethods = New CommuniqueServiceMethods()
            Dim comm As roCommuniqueWithStatistics = CommuniqueSvc.GetCommuniqueStatus(Nothing, IdCommunique)
            If comm.Communique.CreatedBy.IdPassport = WLHelperWeb.CurrentPassportID Then
                oRes = CommuniqueSvc.DeleteCommunique(Nothing, IdCommunique, False)
            End If
        Catch ex As Exception
            Return False
        End Try

        Return oRes
    End Function

    Function GetBarButtons(ByVal sID As String) As String
        Try

            Dim guiActions As Generic.List(Of roGuiAction) = API.PortalServiceMethods.GetGuiActions(Nothing, "Portal\ShiftManagement\Causes\Management", WLHelperWeb.CurrentPassportID)

            ' Dim oResponse As roTools.PageCustomActions = roTools.BuildPageCustomActions(guiActions, sID, Me.Language, Me.DefaultScope, "Causes")
            ' Response.Write(roJSONHelper.SerializeNewtonSoft(oResponse))
            Return guiActions.ToString
        Catch ex As Exception
            ' Response.Write(ex.StackTrace.ToString)
        End Try
        Return String.Empty
    End Function

#Region "SELECTOR DE EMPLEADOS"

    Public Function GetSupervisedEmployees(ByVal oEmpState As Employee.roEmployeeState) As EmployeeList
        Dim lrret As New EmployeeList

        Try
            lrret.Status = ErrorCodes.OK
            Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
            Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

            Dim ImageData As Byte()
            ImageData = New Byte(fileStream.Length - 1) {}
            fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
            fileStream.Close()

            lrret.DefaultImage = "url(data:image/png;base64," & Convert.ToBase64String(ImageData) & ") no-repeat center center"

            Dim empsList As New Generic.List(Of EmployeeInfo)

            Dim dtCommuniques As DataTable = roBusinessSupport.GetAllEmployees("", "Employees", "U", oEmpState, False)

            If dtCommuniques IsNot Nothing AndAlso dtCommuniques.Rows.Count > 0 Then
                For Each oRow As DataRow In dtCommuniques.Rows
                    empsList.Add(New EmployeeInfo With {
                              .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                              .Name = roTypes.Any2String(oRow("EmployeeName")),
                              .Image = "Employee/GetEmployeePhoto/" & roTypes.Any2String(oRow("IDEmployee"))
                            })

                Next
            End If

            lrret.Employees = empsList.ToArray()
        Catch ex As Exception
            lrret.Status = ErrorCodes.GENERAL_ERROR
            lrret.Employees = {}

            Dim oLogState As New roBusinessState("Common.BaseState", "")
            oLogState.UpdateStateInfo(ex, "VTPortal::OnBoardingsHelper::GetSupervisedOnBoardings")
        End Try

        Return lrret
    End Function

    Private Function LoadInitialData() As Boolean
        Try
            Dim availableEmployees As New List(Of EmployeeSelector)
            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)
            Dim employeeList = GetSupervisedEmployees(oEmpState)

            Dim oGroupState As New roGroupState(WLHelperWeb.CurrentPassport.ID, oEmpState.Context, oEmpState.ClientAddress)
            Dim dsGroups As DataSet = roGroup.GetGroups("Calendar", "U", oGroupState)

            Dim groupList = dsGroups.Tables(0).AsEnumerable().[Select](Function(dataRow) New GroupSelector(dataRow.Field(Of Integer)("ID"), dataRow.Field(Of String)("FullGroupName"))).ToList()

            If employeeList.Employees.Length > 0 Then
                Dim empAvailable As EmployeeSelector

                Dim totalAvailable As New List(Of Integer)
                totalAvailable = employeeList.Employees.ToList.Select(Function(x) x.EmployeeId).ToList()

                Dim finalAvailable As EmployeeInfo()
                finalAvailable = employeeList.Employees.ToList.FindAll(Function(y) totalAvailable.ToArray.Contains(y.EmployeeId)).ToArray()
                finalAvailable = finalAvailable.GroupBy(Function(obj) obj.EmployeeId).Select(Function(grupo) grupo.First()).ToArray()

                For Each emp In finalAvailable
                    empAvailable = New EmployeeSelector
                    empAvailable.IdEmployee = emp.EmployeeId
                    empAvailable.EmployeeName = emp.Name
                    empAvailable.Image = emp.Image
                    availableEmployees.Add(empAvailable)
                Next

            End If

            HttpContext.Session("ListOfAvailableEmployees") = availableEmployees

            ViewBag.AvailableEmployees = availableEmployees
            ViewBag.AvailableGroups = groupList
        Catch ex As Exception
            Return False
        End Try
        Return True
    End Function

    Public Function GetSentenceFromSelectedEmployees(hdnEmployees As String, hdnFilter As String, hdnFilterUser As String) As String

        Dim strRet As String = String.Empty

        Try
            If Not String.IsNullOrEmpty(hdnEmployees) Then

                Dim DateInf As DateTime = DateTime.Now
                Dim DateSup As DateTime = DateTime.Now

                If DateInf > DateSup Then
                    Dim aux As DateTime = DateSup
                    DateSup = DateInf
                    DateInf = aux
                End If

                Dim Selection() As String = hdnEmployees.Trim.Split(",")

                If Selection.Length = 1 Then

                    If Selection(0).Substring(0, 1) = "A" Then 'Grupo
                        'obtener el nombre del grupo
                        Dim oGroup As roGroup = API.EmployeeGroupsServiceMethods.GetGroup(Nothing, Selection(0).Substring(1), False)
                        If Not oGroup Is Nothing Then
                            'Dim NumEmployees As Integer = GetNumOfEmployees()
                            Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(hdnEmployees, hdnFilter, hdnFilterUser, "Access", DateInf, DateSup)
                            strRet = NumEmployees & ";" & oGroup.Name
                        End If

                    ElseIf Selection(0).Substring(0, 1) = "B" Then 'Empleado
                        'obtener el nombre del empleado
                        Dim oEmployee As roEmployee = API.EmployeeServiceMethods.GetEmployee(Nothing, Selection(0).Substring(1), False)
                        If Not oEmployee Is Nothing Then
                            strRet = "1;" & oEmployee.Name
                        End If
                    End If
                Else
                    'Dim NumEmployees As Integer = GetNumOfEmployees()
                    Dim NumEmployees As Integer = HelperWeb.GetNumOfEmployeesAnalitics(hdnEmployees, hdnFilter, hdnFilterUser, "Access", DateInf, DateSup)
                    strRet = NumEmployees & ";" & NumEmployees.ToString & " " & New roLanguageWeb().Translate("Pivot.EmployeesSelected", "CommunicationController")
                End If
            End If
        Catch
            strRet = "0; "
        End Try

        Return strRet

    End Function

#End Region

End Class