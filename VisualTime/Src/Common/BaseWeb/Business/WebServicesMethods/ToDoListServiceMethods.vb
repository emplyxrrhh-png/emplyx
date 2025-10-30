Imports Robotics.Base.DTOs
Imports Robotics.Base.VTToDoLists

Namespace API

    Public NotInheritable Class ToDoListServiceMethods

        Public Shared Function GetAllToDoListsByType(ByVal listType As ToDoListType, ByVal oPage As PageBase, Optional bGetEmployeeDetail As Boolean = False) As roToDoList()
            Dim oRet As roToDoList() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                Dim oToDolists As Generic.List(Of roToDoList) = oToDoListManager.GetAllToDoListsByType(listType, bGetEmployeeDetail, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    oRet = oToDolists.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetLastDoListsByType(ByVal listType As ToDoListType, ByVal oPage As PageBase, Optional iTotal As Integer = 3, Optional bGetEmployeeDetail As Boolean = False) As roToDoList()
            Dim oRet As roToDoList() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                Dim oToDolists As Generic.List(Of roToDoList) = oToDoListManager.GetLastToDoListsByType(listType, iTotal, bGetEmployeeDetail, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    oRet = oToDolists.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetOnBoardingByEmployee(ByVal idEmployee As Integer, ByVal oPage As PageBase) As roToDoList
            Dim oRet As roToDoList = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                Dim oToDolist As roToDoList = oToDoListManager.GetOnBoarding(idEmployee, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    oRet = oToDolist
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetToDoList(ByVal idTodoList As Integer, ByVal oPage As PageBase, Optional ByVal listType As ToDoListType = ToDoListType.OnBoarding, Optional ByVal bAudit As Boolean = False) As roToDoList
            Dim oRet As roToDoList = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.GetToDoList(idTodoList, listType, bAudit)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetToDoListTasksByEmployee(ByVal idEmployee As Integer, ByVal oPage As PageBase) As roToDoTask()
            Dim oRet As roToDoTask() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                Dim oToDolists As Generic.List(Of roToDoTask) = oToDoListManager.GetToDoListTasksByIdEmployee(idEmployee, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    oRet = oToDolists.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetToDoListTasks(ByVal idList As Integer, ByVal oPage As PageBase) As roToDoTask()
            Dim oRet As roToDoTask() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                Dim oToDolists As Generic.List(Of roToDoTask) = oToDoListManager.GetToDoListTasks(idList, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    oRet = oToDolists.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetToDoTask(ByVal idTask As Integer, ByVal oPage As PageBase) As roToDoTask
            Dim oRet As roToDoTask = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.GetToDoTask(idTask, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateOrUpdateToDoList(ByVal oPage As PageBase, ByVal oToDoList As roToDoList, Optional ByVal bAudit As Boolean = False) As roToDoList
            Dim oRet As New roToDoList
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.CreateOrUpdateToDoList(oToDoList, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateOrUpdateToDoTask(ByVal oPage As PageBase, ByVal oToDoTask() As roToDoTask, Optional ByVal bAudit As Boolean = False) As roToDoTask()
            Dim oRet As roToDoTask() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.CreateOrUpdateToDoTasks(oToDoTask.ToList, True).ToArray

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteToDoTasks(lTodoTasks As roToDoTask(), ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.DeleteToDoTasks(lTodoTasks.ToList, True)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteToDoList(oTodoList As roToDoList, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.DeleteToDoList(oTodoList, bAudit)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function CloneOnboarding(oTargetToDoList As roToDoList, idSourceEmployee As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roToDoList
            Dim oRet As roToDoList = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roToDoListState = oSession.States.ToDoListState

            WebServiceHelper.SetState(oState)

            Try
                Dim oToDoListManager As New roToDoListManager(oState)
                oRet = oToDoListManager.CloneOnBoarding(oTargetToDoList, idSourceEmployee, bAudit)

                oSession.States.ToDoListState = oToDoListManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.ToDoListState.Result = ToDoListResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ToDoListState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

#Region "Last errors"

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ToDoListState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As ToDoListResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ToDoListState.Result
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace