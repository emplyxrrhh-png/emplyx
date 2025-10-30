Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Task

Namespace API

    Public NotInheritable Class TaskTemplatesServiceMethods

#Region "TaskTemplates"

        ''' <summary>
        ''' Obtiene la lista de indicadores definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskTemplates(ByVal _IDProject As Integer, ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As Generic.List(Of roTaskTemplate)
            Dim oRet As Generic.List(Of roTaskTemplate) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try
                Dim strOrder As String = _Order
                If strOrder = String.Empty Then
                    strOrder = "Name ASC"
                End If
                Dim wsRet As roGenericVtResponse(Of List(Of roTaskTemplate)) = VTLiveApi.TaskTemplateMethods.GetTaskTemplates(_IDProject, "", strOrder, _Audit, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result = TaskResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-708")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de indicadores definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskTemplatesDataTable(ByVal _IDProject As Integer, ByVal _IdsFilter As String, ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try
                Dim strOrder As String = _Order
                If strOrder = String.Empty Then
                    strOrder = "Name ASC"
                End If

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskTemplateMethods.GetTaskTemplatesDataTable(_IDProject, _IdsFilter, strOrder, _Audit, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-709")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTaskTemplate(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal _Audit As Boolean) As roTaskTemplate
            Dim oRet As roTaskTemplate = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roTaskTemplate) = VTLiveApi.TaskTemplateMethods.GetTaskTemplate(_ID, _Audit, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-710")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la configuración de un indicador de rendimiento
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_TaskTemplate"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTaskTemplate(ByVal oPage As System.Web.UI.Page, ByRef _TaskTemplate As roTaskTemplate, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roTaskTemplate) = VTLiveApi.TaskTemplateMethods.SaveTaskTemplate(_TaskTemplate, oState, _Audit)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    _TaskTemplate = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-711")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un indicador de rendimiento
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDTaskTemplate"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteTaskTemplate(ByVal oPage As System.Web.UI.Page, ByVal _IDTaskTemplate As Integer, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskTemplateMethods.DeleteTaskTemplate(_IDTaskTemplate, oState, _Audit)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-712")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve el nº de empleados asignados a una tarea o los empleados seleccionados a partir de los id's de grupo y empleado
        ''' </summary>
        ''' <param name="IDTaskTemplate">ID de la tarea</param>
        ''' <param name="sEmployees">ID's de los empleados</param>
        ''' <param name="sGroups">ID's de los grupos</param>
        '''<returns>Nº de empleados asignados a la tarea o seleccionados</returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeesFromTask(ByVal oPage As System.Web.UI.Page, ByVal IDTaskTemplate As Integer, ByVal sEmployees As String, ByVal sGroups As String) As Double
            Dim bolRet As Double = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Double) = VTLiveApi.TaskTemplateMethods.GetEmployeesFromTask(IDTaskTemplate, sEmployees, sGroups)

                oSession.States.TaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-713")
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
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.TaskTemplateMethods.GetTaskName(intIDTask, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-714")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Project Templates"

        ''' <summary>
        ''' Obtiene la lista de indicadores definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetProjectTemplatesDataTable(ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim strOrder As String = _Order
                If strOrder = String.Empty Then
                    strOrder = "Project ASC"
                End If
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.TaskTemplateMethods.GetProjectsDataTable(strOrder, _Audit, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result = TaskResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-715")
            End Try

            Return oRet

        End Function

        Public Shared Function GetProjectTemplate(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal _Audit As Boolean) As roProjectTemplates

            Dim oRet As roProjectTemplates = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roProjectTemplates) = VTLiveApi.TaskTemplateMethods.GetProjectTemplate(_ID, _Audit, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                oRet = wsRet.Value

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-716")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la configuración de un indicador de rendimiento
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_ProjectTemplate"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveProjectTemplate(ByVal oPage As System.Web.UI.Page, ByRef _ProjectTemplate As roProjectTemplates, ByVal _Audit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roProjectTemplates) = VTLiveApi.TaskTemplateMethods.SaveProjectTemplate(_ProjectTemplate, oState, _Audit)
                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    _ProjectTemplate = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-717")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un indicador de rendimiento
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDProjectTemplate"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteProjectTemplate(ByVal oPage As System.Web.UI.Page, ByVal _IDProjectTemplate As Integer, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskTemplateMethods.DeleteProjectTemplate(_IDProjectTemplate, oState, _Audit)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-718")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helper methods"

        Public Shared Function SaveTasksFromTemplates(ByVal _IDPassport As Integer, ByVal oPage As System.Web.UI.Page, ByVal tbTaskTemplates As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.TaskTemplateState

            WebServiceHelper.SetState(oState)

            Try
                Dim dsTaskTemplates As DataSet
                If tbTaskTemplates.DataSet IsNot Nothing Then
                    dsTaskTemplates = tbTaskTemplates.DataSet
                Else
                    dsTaskTemplates = New DataSet
                    dsTaskTemplates.Tables.Add(tbTaskTemplates)
                End If

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.TaskTemplateMethods.SaveTasksFromTemplates(_IDPassport, dsTaskTemplates, oState)

                oSession.States.TaskTemplateState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.TaskTemplateState.Result <> TaskResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.TaskTemplateState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-719")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.TaskTemplateState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace