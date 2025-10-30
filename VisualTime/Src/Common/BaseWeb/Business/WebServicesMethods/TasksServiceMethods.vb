Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.Base.VTUserFields.UserFields

Namespace API

    Public NotInheritable Class TasksServiceMethods

        ''' <summary>
        ''' Recupera una Tarea por ID
        ''' </summary>
        ''' <param name="oPage">Página donde mostrar los errores</param>
        ''' <param name="intIDTask">ID de tarea a recuperar</param>
        ''' <returns>Devuelve un roTask</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskByID(ByVal oPage As System.Web.UI.Page, ByVal intIDTask As Integer, ByVal bAudit As Boolean) As roTask

            Dim oRet As roTask = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roTask) = VTLiveApi.TaskMethods.GetTask(intIDTask, oState, bAudit)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-662")
            End Try

            Return oRet

        End Function

        Public Shared Function GetLastTaskByEmployee(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer) As roTask
            Dim oRet As roTask = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roTask) = VTLiveApi.TaskMethods.GetLastTaskByEmployee(intIDEmployee, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-663")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el nº de empleados asignados a una tarea o los empleados seleccionados a partir de los id's de grupo y empleado
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        ''' <param name="sEmployees">ID's de los empleados</param>
        ''' <param name="sGroups">ID's de los grupos</param>
        '''<returns>Nº de empleados asignados a la tarea o seleccionados</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesFromTask(ByVal oPage As System.Web.UI.Page, ByVal IDTask As Integer, ByVal sEmployees As String, ByVal sGroups As String) As Double
            Dim oRet As Double = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Double) = VTLiveApi.TaskMethods.GetEmployeesFromTask(IDTask, sEmployees, sGroups)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-664")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el nº de empleados trabajando en la tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Nº de empleados trabajando en la tarea seleccionada</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkingInTask(ByVal oPage As System.Web.UI.Page, ByVal IDTask As Integer) As Double
            Dim oRet As Double = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Double) = VTLiveApi.TaskMethods.GetEmployeesWorkingInTask(IDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-665")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el listado de empleados trabajando en la tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Listado de empleados trabajando en la tarea seleccionada</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkingInTaskList(ByVal oPage As System.Web.UI.Page, ByVal IDTask As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetEmployeesWorkingInTaskList(IDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-666")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el listado de empleados trabajando en la tarea
        ''' </summary>
        ''' <param name="IDTask">ID de la tarea</param>
        '''<returns>Listado de empleados trabajando en la tarea seleccionada</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesWorkedInTaskList(ByVal oPage As System.Web.UI.Page, ByVal IDTask As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetEmployeesWorkedInTaskList(IDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-667")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve las últimas tareas que ha realizado un empleado
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>

        Public Shared Function GetLastTasksByEmployee(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetLastTasksByEmployee(intIDEmployee, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-669")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve tareas que contengan un valor
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTasksByName(ByVal oPage As System.Web.UI.Page, ByVal strLikeName As String) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetTasksByName(strLikeName, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-670")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve tareas que contengan un valor
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTasksByProjectName(ByVal oPage As System.Web.UI.Page, ByVal strLikeProject As String) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetTasksByProjectName(strLikeProject, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-671")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve listado de proyectos que contienen el patro de busqueda
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectsByName(ByVal oPage As System.Web.UI.Page, ByVal strLikeProject As String) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetProjectsByName(strLikeProject, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-672")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve estadísticas de la tarea
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskStatistics(ByVal oPage As System.Web.UI.Page, ByVal intIDTask As Integer, ByVal ViewType As TaskStatisticsViewEnum, ByVal TaskStatisticsGroupBy As TaskStatisticsGroupByEnum, ByVal xBeginDate As Date, ByVal xEnd As Date) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetStatistics(intIDTask, ViewType, TaskStatisticsGroupBy, xBeginDate, xEnd, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-673")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la configuración de una Tarea
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Task"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTask(ByVal oPage As System.Web.UI.Page, ByRef _Task As roTask, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roTask) = VTLiveApi.TaskMethods.SaveTask(_Task, oState, _Audit)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    _Task = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-674")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina una tarea
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Task"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTask(ByVal oPage As System.Web.UI.Page, ByVal _Task As roTask, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.DeleteTask(_Task, oState, _Audit)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-675")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina una tarea a partir del ID
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDTask"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTaskByID(ByVal oPage As System.Web.UI.Page, ByVal _IDTask As Integer, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.DeleteTaskByID(_IDTask, oState, _Audit)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-676")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia una tarea para crear otra
        ''' </summary>
        ''' <param name="oPage">Página desde donde se ejecuta el proceso (retorno de errores,etc.)</param>
        ''' <param name="IDTaskSource">ID de la tarea origen</param>
        ''' <param name="NewName">Nombre de la nueva tarea (opcional)</param>
        ''' <returns>Devuelve el ID de la nueva tarea</returns>
        ''' <remarks></remarks>
        Public Shared Function CopyTask(ByVal oPage As System.Web.UI.Page, ByVal IDTaskSource As Integer, Optional ByVal NewName As String = "", Optional ByVal bAudit As Boolean = True) As Integer
            Dim bolRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.TaskMethods.CopyTask(IDTaskSource, NewName, oState, bAudit)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-677")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Recupera un nombre de Tarea por ID
        ''' </summary>
        ''' <param name="oPage">Página donde mostrar los errores</param>
        ''' <param name="intIDTask">ID de tarea a recuperar</param>
        ''' <returns>Devuelve un nombre de tarea</returns>
        ''' <remarks></remarks>
        Public Shared Function GetNameTask(ByVal oPage As System.Web.UI.Page, ByVal intIDTask As Integer) As String
            Dim bolRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.TaskMethods.GetTaskName(intIDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-678")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda un BusinessCenter
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oBusinessCenter"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveBusinessCenter(ByVal oPage As System.Web.UI.Page, ByRef oBusinessCenter As roBusinessCenter, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roBusinessCenter) = VTLiveApi.TaskMethods.SaveBusinessCenter(oBusinessCenter, oState, _Audit)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oBusinessCenter = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-679")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un BusinessCenter
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oBusinessCenter"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteBusinessCenter(ByVal oPage As System.Web.UI.Page, ByVal oBusinessCenter As roBusinessCenter, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.DeleteBusinessCenter(oBusinessCenter, oState, _Audit)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-680")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteBusinessCenterByID(ByVal oPage As System.Web.UI.Page, ByVal _IDBusinessCenter As Integer, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.DeleteBusinessCenterByID(_IDBusinessCenter, oState, _Audit)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-681")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetBusinessCenterByID(ByVal oPage As System.Web.UI.Page, ByVal intIDBusinessCenter As Integer, ByVal bAudit As Boolean) As roBusinessCenter
            Dim oRet As roBusinessCenter = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roBusinessCenter) = VTLiveApi.TaskMethods.GetBusinessCenterByID(intIDBusinessCenter, oState, bAudit)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-682")
            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessCenterZones(ByVal oPage As System.Web.UI.Page, ByVal intIDCenter As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetBusinessCenterZones(intIDCenter, oState)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result = BusinessCenterResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-683")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los BusinessCenter
        ''' </summary>
        Public Shared Function GetBusinessCenters(ByVal oPage As System.Web.UI.Page, ByVal CheckStatus As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetBusinessCenters(oState, CheckStatus)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result = BusinessCenterResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-684")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los BusinessCenter
        ''' </summary>
        Public Shared Function GetBusinessCentersByFilter(ByVal oPage As System.Web.UI.Page, strFilter As String) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetBusinessCentersByFilter(oState, strFilter)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result = BusinessCenterResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-685")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los BusinessCenter
        ''' </summary>
        Public Shared Function GetBusinessCenterByPassportDataTable(ByVal oPage As System.Web.UI.Page, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetBusinessCenterByPassportDataTable(oState, intIDPassport, bolCheckStatus)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result = BusinessCenterResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-686")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene los BusinessCenter
        ''' </summary>
        Public Shared Function GetBusinessCenterBySecurityGroupDataTable(ByVal oPage As System.Web.UI.Page, ByVal intIDSecurityGroup As Integer, ByVal bolCheckStatus As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetBusinessCenterBySecurityGroupDataTable(oState, intIDSecurityGroup, bolCheckStatus)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result = BusinessCenterResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-687")
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Guarda los BusinessCenter por empleado
        ''' </summary>

        Public Shared Function SaveEmployeeBusinessCenters(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal tbCenters As DataTable) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim dsCenters As DataSet
                If tbCenters.DataSet IsNot Nothing Then
                    dsCenters = tbCenters.DataSet
                Else
                    dsCenters = New DataSet
                    dsCenters.Tables.Add(tbCenters)
                End If

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SavEmployeeBusinessCenters(_IDEmployee, dsCenters, oState)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-688")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene los BusinessCenter por empleado
        ''' </summary>
        Public Shared Function GetEmployeeBusinessCentersDataTable(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, Optional ByVal bolDefaultCenter As Boolean = True, Optional ByVal bolOnlyActiveCenters As Boolean = False) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetEmployeeBusinessCentersDataTable(oState, intIDEmployee, bolDefaultCenter, bolOnlyActiveCenters)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result = BusinessCenterResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-689")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene el BusinessCenter por defecto del empleado para un dia
        ''' </summary>
        Public Shared Function GetEmployeeDefaultBusinessCenter(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, ByVal xDate As Date) As Integer
            Dim bolRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.TaskMethods.GetEmployeeDefaultBusinessCenter(oState, intIDEmployee, xDate)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-690")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene los BusinessCenter por IDPassport
        ''' </summary>
        Public Shared Function GetBusinessCenterByPassport(ByVal oPage As System.Web.UI.Page, ByVal intIDPassport As Integer, ByVal bolCheckStatus As Boolean) As Integer()
            Dim bolRet As Integer() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer()) = VTLiveApi.TaskMethods.GetBusinessCenterByPassport(oState, intIDPassport, bolCheckStatus, False)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-691")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveBusinessCenterByPassport(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer, ByVal _IDBusinessCenters() As Integer, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveBusinessCenterByPassport(oState, _IDPassport, _IDBusinessCenters, _Audit)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-692")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveBusinessCenterBySecurityGroup(ByVal oPage As System.Web.UI.Page, ByVal _IdSecurityGroup As Integer, ByVal _IDBusinessCenters() As Integer, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BusinessCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveBusinessCenterBySecurityGroup(oState, _IdSecurityGroup, _IDBusinessCenters, _Audit)

                oSession.States.BusinessCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BusinessCenterState.Result <> BusinessCenterResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BusinessCenterState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-693")
            End Try

            Return bolRet
        End Function

        Public Shared Function GetTaskFieldsDataTable(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetTaskFieldsDataTable(_TaskId, oState)

                oSession.States.TaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskFieldState.Result = TaskFieldResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-694")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskAlertsDataTable(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetTaskAlertsDataTable(_TaskId, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-695")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskFieldsList(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer) As Generic.List(Of roTaskField)
            Dim oRet As Generic.List(Of roTaskField) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roTaskField)) = VTLiveApi.TaskMethods.GetTaskFieldsList(_TaskId, oState)

                oSession.States.TaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskFieldState.Result = TaskFieldResultEnum.NoError Then
                    oRet = wsRet.Value.ToList
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-697")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveTaskFields(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer, ByVal _TaskFields As Generic.List(Of roTaskField)) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveTaskFields(_TaskId, _TaskFields, oState)

                oSession.States.TaskFieldState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskFieldState.Result <> TaskFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskFieldState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-698")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskAlerts(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer, ByVal _TaskAlerts As Generic.List(Of roTaskAlert)) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveTaskAlerts(_TaskId, _TaskAlerts, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-699")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskAlertsFromPunch(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal _Comment As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveTaskAlertsFromPunch(_TaskId, _IDEmployee, _Date, _Comment, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-700")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveTaskFieldsFromPunch(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer, ByVal Field1 As String, ByVal Field2 As String, ByVal Field3 As String, ByVal Field4 As Double, ByVal Field5 As Double, ByVal Field6 As Double) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveTaskFieldsFromPunch(_TaskId, Field1, Field2, Field3, Field4, Field5, Field6, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-700")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetAntTaskPunchDate(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime) As DateTime
            Dim bolRet As DateTime

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DateTime) = VTLiveApi.TaskMethods.GetAntTaskPunchDate(_TaskId, _IDEmployee, _Date, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-702")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetNextTaskPunchDate(ByVal oPage As System.Web.UI.Page, ByVal _TaskId As Integer, ByVal _IDEmployee As Integer, ByVal _Date As DateTime) As DateTime
            Dim bolRet As DateTime

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DateTime) = VTLiveApi.TaskMethods.GetNextTaskPunchDate(_TaskId, _IDEmployee, _Date, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-703")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda el layout del cubo de datos
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskProgress(ByVal oPage As System.Web.UI.Page, ByVal _IDTask As Integer) As Double
            Dim bolRet As Double = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Double) = VTLiveApi.TaskMethods.GetStatusTask(_IDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-704")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda el layout del cubo de datos
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskWorkedTime(ByVal oPage As System.Web.UI.Page, ByVal _IDTask As Integer) As Double
            Dim bolRet As Double = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Double) = VTLiveApi.TaskMethods.GetTaskWorkedTime(_IDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-705")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetAssignments(ByVal oPage As System.Web.UI.Page, ByVal _IDTask As Integer, ByVal bAudit As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskMethods.GetTaskAssignmentsDatatable(_IDTask, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-706")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveTaskAssignments(ByVal oPage As System.Web.UI.Page, ByVal intIDTask As Integer, ByVal lstTaskAssignments As Generic.List(Of roTaskAssignment)) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskMethods.SaveTaskAssignments(intIDTask, lstTaskAssignments, oState)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-707")
            End Try

            Return bolRet

        End Function

        '*** FIN VISTAS DE ANALYTICS DE TAREAS

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.TaskState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastErrorTextBusinessCenter() As String

            Dim strRet As String = ""

            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BusinessCenterState.ErrorText
            End If

            Return strRet

        End Function

    End Class

End Namespace