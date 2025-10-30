Imports System.IO
Imports System.Net
Imports System.Web.Hosting
Imports System.Web.Mvc
Imports DevExtreme.AspNet.Data
Imports DevExtreme.AspNet.Mvc
Imports Newtonsoft.Json
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.Web.Base

<PermissionsAtrribute(FeatureAlias:="Employees", Permission:=Robotics.Base.DTOs.Permission.Read)>
<LoggedInAtrribute(Requiered:=True)>
Public Class OnBoardingController
    Inherits System.Web.Mvc.Controller
    Private oLanguage As roLanguageWeb

    Function Index() As ActionResult
        If WLHelperWeb.CurrentPassport IsNot Nothing Then
            ViewBag.RootUrl = ConfigurationManager.AppSettings("RootUrl")
            ViewBag.PermissionOverEmployees = Convert.ToInt32(API.SecurityServiceMethods.GetPermissionOverFeature(Nothing, "Employees", "U"))
            Try
                LoadInitialData()
            Catch ex As Exception
            End Try
            Return View("OnBoarding")
        Else
            Return View("NoSession")
        End If
    End Function

    Public Function LoadInitialData() As Boolean

        '''''''''''''''''''''''''''''''''''''''''''''
        ''Recuperar empleados para el autocompletar''
        '''''''''''''''''''''''''''''''''''''''''''''
        Try
            Dim availableEmployees As New List(Of OnBoardingInfo)
            Dim alreadyUsedEmployees As New List(Of OnBoardingInfo)
            ' Dim lastCreatedOnboardings As New List(Of OnBoardingInfo)

            Dim oEmpState As New Employee.roEmployeeState(WLHelperWeb.CurrentPassport.ID)
            Dim employeeList = GetSupervisedEmployees(oEmpState)
            Dim oToDoLists = API.ToDoListServiceMethods.GetAllToDoListsByType(ToDoListType.OnBoarding, Nothing)
            ' Dim oToDoLastLists = API.ToDoListServiceMethods.GetLastDoListsByType(ToDoListType.OnBoarding, Nothing)

            Dim empAvailable As OnBoardingInfo
            ' Dim copyAvailable As OnBoardingInfo
            If employeeList.Employees.Length > 0 Then

                Dim idAll As New List(Of Integer)
                idAll = employeeList.Employees.ToList.Select(Function(x) x.EmployeeId).ToList()

                Dim idAlreadyIn As New List(Of Integer)
                idAlreadyIn = oToDoLists.ToList.Select(Function(x) x.IdEmployee).ToList()

                'Dim idLastUsed As New List(Of Integer)
                'idLastUsed = oToDoLastLists.ToList.Select(Function(x) x.IdEmployee).ToList()

                Dim totalAvailable As New List(Of Integer)
                totalAvailable = idAll.Except(idAlreadyIn).ToList()

                Dim finalAvailable As EmployeeInfo()
                finalAvailable = employeeList.Employees.ToList.FindAll(Function(y) totalAvailable.ToArray.Contains(y.EmployeeId)).ToArray()
                finalAvailable = finalAvailable.GroupBy(Function(obj) obj.EmployeeId).Select(Function(grupo) grupo.First()).ToArray()

                Dim finalUsed As EmployeeInfo()
                finalUsed = employeeList.Employees.ToList.FindAll(Function(y) idAlreadyIn.ToArray.Contains(y.EmployeeId)).ToArray()
                finalUsed = finalUsed.GroupBy(Function(obj) obj.EmployeeId).Select(Function(grupo) grupo.First()).ToArray()

                'Dim finalLastUSed As EmployeeInfo()
                'finalLastUSed = employeeList.Employees.ToList.FindAll(Function(y) idLastUsed.ToArray.Contains(y.EmployeeId)).ToArray()
                'For Each emp In finalLastUSed
                '    copyAvailable = New OnBoardingInfo
                '    copyAvailable.IdEmployee = emp.EmployeeId
                '    copyAvailable.EmployeeName = emp.Name
                '    lastCreatedOnboardings.Add(copyAvailable)
                'Next

                For Each emp In finalAvailable
                    empAvailable = New OnBoardingInfo
                    empAvailable.IdEmployee = emp.EmployeeId
                    empAvailable.EmployeeName = emp.Name
                    empAvailable.BeginContractDate = emp.BeginDate.ToString("yyyy-MM-dd")
                    availableEmployees.Add(empAvailable)
                Next

                For Each emp In finalUsed
                    empAvailable = New OnBoardingInfo
                    empAvailable.IdEmployee = emp.EmployeeId
                    empAvailable.EmployeeName = emp.Name & " (" & oToDoLists.ToList.FirstOrDefault(Function(x) x.IdEmployee = emp.EmployeeId).CreatedOn.Date.ToShortDateString & ")"
                    alreadyUsedEmployees.Add(empAvailable)
                Next

            End If

            HttpContext.Session("ListOfAvailableEmployees") = availableEmployees
            HttpContext.Session("ListOfUsedEmployees") = alreadyUsedEmployees

            ViewBag.AvailableEmployees = availableEmployees
            ViewBag.AlreadyUsedEmployees = alreadyUsedEmployees
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    <HttpPost>
    Public Function GetUsedEmployees() As JsonResult

        Dim alreadyUsedEmployees = HttpContext.Session("ListOfUsedEmployees")

        Return Json(alreadyUsedEmployees)

    End Function

    <HttpPost>
    Public Function GetAvailableEmployees() As JsonResult

        Dim availableEmployees = HttpContext.Session("ListOfAvailableEmployees")

        Return Json(availableEmployees)

    End Function

    <HttpGet>
    Public Function GetOnBoardings(ByVal loadOptions As DataSourceLoadOptions, ByVal hasData As String, ByVal hasData2 As String) As ActionResult

        Dim onBoardingList As New List(Of OnBoardingInfo)
        Dim oToDoLists() As roToDoList
        Dim oEmpState As New Employee.roEmployeeState(-1)
        Dim fileName As String = Path.Combine(HostingEnvironment.ApplicationPhysicalPath, "Base/Images/USER_48.PNG")
        Dim fileStream As New FileStream(fileName, FileMode.Open, FileAccess.Read)

        Dim ImageData As Byte()
        ImageData = New Byte(fileStream.Length - 1) {}
        fileStream.Read(ImageData, 0, System.Convert.ToInt32(fileStream.Length))
        fileStream.Close()

        oToDoLists = API.ToDoListServiceMethods.GetAllToDoListsByType(ToDoListType.OnBoarding, Nothing)

        For Each list As roToDoList In oToDoLists

            Dim employee = API.EmployeeServiceMethods.GetEmployee(Nothing, list.IdEmployee, False)

            Dim OnBoardingListObject As New OnBoardingInfo
            OnBoardingListObject.IdEmployee = list.IdEmployee
            OnBoardingListObject.StartDate = list.StartDate
            OnBoardingListObject.Status = list.Status
            OnBoardingListObject.EmployeeName = API.EmployeeServiceMethods.GetEmployeeName(Nothing, list.IdEmployee.ToString)
            OnBoardingListObject.Group = API.EmployeeServiceMethods.GetCurrentGroupName(Nothing, list.IdEmployee)
            OnBoardingListObject.Comments = list.Comments
            OnBoardingListObject.Image = "Employee/GetEmployeePhoto/" & list.IdEmployee.ToString
            OnBoardingListObject.IdList = list.Id

            onBoardingList.Add(OnBoardingListObject)
        Next

        Dim result = DataSourceLoader.Load(onBoardingList.ToArray, loadOptions)
        Dim resultJson = JsonConvert.SerializeObject(result)
        'HttpContext.Session("OnBoarding_EmployeeList") = resultJson

        Return Content(resultJson, "application/json")

    End Function

    <HttpPost>
    Public Function InsertTask(ByVal values As NewTaskData) As ActionResult
        Dim ListTaskToCreate As New List(Of roToDoTask)
        Dim newTask As New roToDoTask()
        newTask.IdList = values.List
        newTask.TaskName = values.Task
        newTask.SupervisorName = ""
        newTask.LastChangeDate = Date.Now
        ListTaskToCreate.Add(newTask)

        Dim toDoList As roToDoList = API.ToDoListServiceMethods.GetToDoList(newTask.IdList, Nothing)

        If toDoList IsNot Nothing AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, toDoList.IdEmployee, "Employees", "U", Permission.Write) Then
            API.ToDoListServiceMethods.CreateOrUpdateToDoTask(Nothing, ListTaskToCreate.ToArray, True)

            If API.ToDoListServiceMethods.LastResult = ToDoListResultEnum.NoError Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpPost>
    Public Function InsertOnBoarding(ByVal values As NewOnBoardingData) As ActionResult
        Dim recharged As Boolean
        Dim newOnBoarding As New roToDoList()
        newOnBoarding.IdEmployee = values.Employee
        newOnBoarding.LastModifiedBy = WLHelperWeb.CurrentPassport.Name
        newOnBoarding.StartDate = DateTime.ParseExact(values.StartDate, "yyyy-MM-dd", Nothing)
        newOnBoarding.Type = ToDoListType.OnBoarding
        newOnBoarding.Comments = ""

        If API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, newOnBoarding.IdEmployee, "Employees", "U", Permission.Write) Then
            If values.CopyEmp IsNot Nothing Then
                API.ToDoListServiceMethods.CloneOnboarding(newOnBoarding, values.CopyEmp, Nothing, True)
            Else
                API.ToDoListServiceMethods.CreateOrUpdateToDoList(Nothing, newOnBoarding, True)
            End If

            If API.ToDoListServiceMethods.LastResult = ToDoListResultEnum.NoError Then
                recharged = LoadInitialData()
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            If recharged = True Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpPut>
    Public Function UpdateOnBoarding(ByVal key As Integer, ByVal values As String) As ActionResult

        Dim List As New roToDoList
        List = API.ToDoListServiceMethods.GetToDoList(key, Nothing)
        JsonConvert.PopulateObject(values, List)
        List.LastModifiedBy = WLHelperWeb.CurrentPassport.Name

        If List IsNot Nothing AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, List.IdEmployee, "Employees", "U", Permission.Write) Then
            API.ToDoListServiceMethods.CreateOrUpdateToDoList(Nothing, List, True)
            If API.ToDoListServiceMethods.LastResult = ToDoListResultEnum.NoError Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
            Return New HttpStatusCodeResult(HttpStatusCode.OK)
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)

        End If

    End Function

    <HttpDelete>
    Public Function DeleteOnBoarding(ByVal key As Integer, ByVal values As String) As ActionResult

        Dim List As New roToDoList()
        List.Id = key

        Dim toDoList As roToDoList = API.ToDoListServiceMethods.GetToDoList(key, Nothing)
        If toDoList IsNot Nothing AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, toDoList.IdEmployee, "Employees", "U", Permission.Write) Then
            API.ToDoListServiceMethods.DeleteToDoList(List, Nothing)
            If API.ToDoListServiceMethods.LastResult = ToDoListResultEnum.NoError Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpPut>
    Public Function UpdateTask(ByVal key As Integer, ByVal values As String) As ActionResult

        Dim ListTaskToCreate As New List(Of roToDoTask)
        Dim Task As New roToDoTask()
        Task = API.ToDoListServiceMethods.GetToDoTask(key, Nothing)
        JsonConvert.PopulateObject(values, Task)
        Task.SupervisorName = WLHelperWeb.CurrentPassport.Name
        Task.LastChangeDate = Date.Now
        ListTaskToCreate.Add(Task)

        Dim toDoList As roToDoList = API.ToDoListServiceMethods.GetToDoList(Task.IdList, Nothing)
        If toDoList IsNot Nothing AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, toDoList.IdEmployee, "Employees", "U", Permission.Write) Then
            API.ToDoListServiceMethods.CreateOrUpdateToDoTask(Nothing, ListTaskToCreate.ToArray, True)
            If API.ToDoListServiceMethods.LastResult = ToDoListResultEnum.NoError Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpDelete>
    Public Function DeleteTask(ByVal key As Integer) As ActionResult
        Dim ListTaskToDelete As New List(Of roToDoTask)
        Dim Task As New roToDoTask()
        Task = API.ToDoListServiceMethods.GetToDoTask(key, Nothing)

        Dim toDoList As roToDoList = API.ToDoListServiceMethods.GetToDoList(Task.IdList, Nothing)
        If toDoList IsNot Nothing AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, toDoList.IdEmployee, "Employees", "U", Permission.Write) Then
            ListTaskToDelete.Add(Task)
            If API.ToDoListServiceMethods.DeleteToDoTasks(ListTaskToDelete.ToArray, Nothing, True) Then
                Return New HttpStatusCodeResult(HttpStatusCode.OK)
            Else
                Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
            End If
        Else
            Return New HttpStatusCodeResult(HttpStatusCode.BadRequest)
        End If

    End Function

    <HttpGet>
    Public Function GetTasksResume(ByVal loadOptions As DataSourceLoadOptions, ByVal idList As String) As ActionResult
        Dim taskInfoList As New List(Of TaskInfo)

        Dim toDoList As roToDoList = API.ToDoListServiceMethods.GetToDoList(idList, Nothing)
        If toDoList IsNot Nothing AndAlso API.SecurityServiceMethods.HasPermissionOverEmployee(Nothing, toDoList.IdEmployee, "Employees", "U", Permission.Write) Then
            Dim oToDoLists = API.ToDoListServiceMethods.GetToDoListTasks(idList, Nothing)

            If oToDoLists IsNot Nothing Then
                For Each task In oToDoLists
                    Dim taskInfo = New TaskInfo
                    If task.SupervisorName = "" Then
                        taskInfo.LastChangeDate = ""
                        taskInfo.SupervisorName = ""
                    Else
                        taskInfo.LastChangeDate = task.LastChangeDate.ToString("dd/MM/yyyy")
                        taskInfo.SupervisorName = task.SupervisorName
                    End If
                    taskInfo.Done = task.Done
                    taskInfo.TaskName = task.TaskName
                    taskInfo.Id = task.Id
                    taskInfo.IdList = task.IdList

                    taskInfoList.Add(taskInfo)
                Next
            End If

            Dim result = DataSourceLoader.Load(taskInfoList, loadOptions)
            Dim resultJson = JsonConvert.SerializeObject(result)
            Return Content(resultJson, "application/json")
        Else
            Return Nothing
        End If

    End Function

    Private Function GetSupervisedEmployees(ByVal oEmpState As Employee.roEmployeeState) As EmployeeList
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

            Dim dtOnBoardings As DataTable = roBusinessSupport.GetAllEmployees("", "Employees", "U", oEmpState, False)

            If dtOnBoardings IsNot Nothing AndAlso dtOnBoardings.Rows.Count > 0 Then
                For Each oRow As DataRow In dtOnBoardings.Rows
                    empsList.Add(New EmployeeInfo With {
                              .EmployeeId = roTypes.Any2Integer(oRow("IDEmployee")),
                              .Name = roTypes.Any2String(oRow("EmployeeName")),
                              .BeginDate = roTypes.Any2DateTime(oRow("BeginDate"))
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

    Public Function GetServerLanguage() As roLanguageWeb
        If Me.oLanguage Is Nothing Then
            Me.oLanguage = New roLanguageWeb
            WLHelperWeb.SetLanguage(Me.oLanguage, "LiveGUI")
        End If
        Return Me.oLanguage
    End Function

End Class