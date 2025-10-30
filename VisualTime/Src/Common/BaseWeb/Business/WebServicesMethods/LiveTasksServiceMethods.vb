Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTUserFields.UserFields
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace API

    Public NotInheritable Class LiveTasksServiceMethods

        Public Shared Function GetLiveTaskStatus(ByVal oPage As System.Web.UI.Page, ByVal IDLiveTask As Integer) As roLiveTask

            Dim oRet As roLiveTask = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roLiveTask) = VTLiveApi.LiveTasksMethods.GetLiveTaskStatus(IDLiveTask, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-374")
            End Try

            Return oRet

        End Function

        Public Shared Function RemoveCompletedTask(ByVal oPage As System.Web.UI.Page, ByVal IDLiveTask As Integer) As Boolean

            Dim oRet As Boolean = True

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LiveTasksMethods.RemoveCompletedTask(IDLiveTask, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = False
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-374")
            End Try

            Return oRet

        End Function

        Public Shared Function GetReportsTaksStatus(ByVal oPage As System.Web.UI.Page, ByVal oTaskStatus As DTOs.roLiveTaskStatus, ByVal onlyUser As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.LiveTasksMethods.GetReportsTaksStatus(oTaskStatus, onlyUser, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result = LiveTasksResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.LiveTaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-375")
            End Try

            Return oRet
        End Function

        Public Shared Function ExecuteTaskWithoutParameters(ByVal oPage As System.Web.UI.Page, ByVal oTaskType As DTOs.roLiveTaskTypes) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateTaskWithoutParameters(oTaskType, oState, False)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-376")
            End Try

            Return oRet

        End Function

        Public Shared Function ExecuteAIPlanner(ByVal oPage As System.Web.UI.Page, ByVal bUpdateSchedule As Boolean, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                           ByVal idOrgChartNode As Integer, ByVal pUnitFilter As String, ByVal bAudit As Boolean) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.ExecuteAIPlanner(bUpdateSchedule, iIdPassport, xBeginDate, xEndDate, idOrgChartNode, pUnitFilter, oState, False)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-377")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateAnalyticTask(ByVal oPage As System.Web.UI.Page, ByVal analyticType As DTOs.roLiveAnalyticType, ByVal idView As Integer, ByVal idLayout As Integer, ByVal iIdPassport As Integer, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                           ByVal strEmployees As String, ByVal strConcepts As String, ByVal strUserFields As String, ByVal strCostCenters As String, ByVal bIncludeZeroValues As Boolean,
                                           ByVal bIncludeUndefinedCostCenters As Boolean, ByVal bAudit As Boolean, ByVal strRequestTypes As String, ByVal strCauses As String) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateAnalyticTask(analyticType, idView, idLayout, iIdPassport, xBeginDate, xEndDate, strEmployees, strConcepts, strUserFields, strCostCenters, bIncludeZeroValues, bIncludeUndefinedCostCenters, oState, False, strRequestTypes, strCauses)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-378")
            End Try

            Return oRet

        End Function

        Public Shared Function CreateReportTaskDX(ByVal isPlannedTask As Boolean, ByVal reportId As Integer, ByVal reportParameters As String, ByVal viewFields As String, ByVal bAudit As Boolean, ByVal oPage As System.Web.UI.Page) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateReportTaskDX(isPlannedTask, reportId, reportParameters, viewFields, WLHelperWeb.CurrentPassportID, oState, False)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If Not (oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError) Then
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-378") 'MIRAR SI HAY QUE CAMBIAR EL ERROR
            End Try

            Return oRet

        End Function

        Public Shared Function CopyEmployeesBackground(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal strDestinationIDFilters As String, ByVal ckCopyAssignments As Boolean, ByVal ckCopyUserFields As Boolean,
                                          ByVal userFieldHistory As List(Of String), ByVal userFieldNoHistory As List(Of String), ByVal ckLabAgree As Boolean, ByVal oAssignments As List(Of String), ByVal copySchedule As Boolean,
                                          ByVal xStart As Date, ByVal xEnd As Date, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean,
                                          ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal ckCopyCenters As Boolean, ByVal oCenters As List(Of String), ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CopyEmployeesBackground(intSourceIDEmployee, strDestinationIDFilters, ckCopyUserFields, userFieldHistory, userFieldNoHistory, ckLabAgree, ckCopyAssignments, oAssignments,
                                                                  copySchedule, xStart, xEnd, bolCopyMainShifts, bolCopyAlternativeShifts, bolCopyHolidays, bolKeepBloquedDays, bolKeepHolidays, ckCopyCenters, oCenters, bAudit, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-379")
            End Try

            Return oRet

        End Function

        Public Shared Function CompleteTasksAndProjectsBackground(ByVal oPage As System.Web.UI.Page, ByVal strTaskIDs As String, ByVal strProjectNames As String, ByVal bAudit As Boolean) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CompleteTasksAndProjectsBackground(strTaskIDs, strProjectNames, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-380")
            End Try

            Return oRet

        End Function

        Public Shared Function MassProgrammedAbsenceBackground(ByVal oPage As System.Web.UI.Page, ByVal strDestinationEmployees As String, ByVal strFilters As String, ByVal strUserFieldsFilter As String, ByVal IDCause As Integer,
                                               ByVal xBeginDateTime As Date, ByVal xEndDateTime As Date, ByVal strDescription As String, ByVal MinDuration As Double, ByVal MaxDuration As Double,
                                                ByVal MaxDays As Integer, ByVal BeginHour As DateTime, ByVal EndHour As DateTime, massType As eRequestType, ByVal bAudit As Boolean) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.MassProgrammedAbsenceBackground(strDestinationEmployees, strFilters, strUserFieldsFilter, IDCause, xBeginDateTime, xEndDateTime, strDescription, MinDuration, MaxDuration, MaxDays, BeginHour, EndHour, massType, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-822")
            End Try

            Return oRet
        End Function

        Public Shared Function MassPunchBackground(ByVal oPage As System.Web.UI.Page, ByVal strDestinationEmployees As String, ByVal strFilters As String, ByVal strUserFieldsFilter As String, ByVal IDTerminal As Integer,
                                                ByVal IDCause As Integer, ByVal xDateTime As Date, ByVal strDirection As String, ByVal bAudit As Boolean) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.MassPunchBackground(strDestinationEmployees, strFilters, strUserFieldsFilter, IDTerminal, IDCause, xDateTime, strDirection, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-381")
            End Try

            Return oRet

        End Function

        Public Shared Function AssignCauseInBackground(ByVal oPage As System.Web.UI.Page, ByVal strDestinationEmployeesFilter As String, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal intIDCause As Integer, ByVal bCompletedDay As Boolean, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.AssignCauseInBackground(strDestinationEmployeesFilter, xBeginPeriod, xEndPeriod, intIDCause, bCompletedDay, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-382")
            End Try

            Return oRet

        End Function

        Public Shared Function CreateMessageForEmployees(ByVal oPage As System.Web.UI.Page, ByVal strDestinationEmployeesFilter As String, ByVal strMessage As String, ByVal strSchedule As String, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateMessageForEmployees(strDestinationEmployeesFilter, strMessage, strSchedule, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-383")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateSetLockDateForEmployees(ByVal oPage As System.Web.UI.Page, ByVal strDestinationEmployeesFilter As String, ByVal xLockDate As DateTime, ByVal bApplyGlobalLockDate As Boolean, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateSetLockDateForEmployees(strDestinationEmployeesFilter, xLockDate, bApplyGlobalLockDate, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-383")
            End Try

            Return oRet
        End Function

        Public Shared Function JustifiedIncidencesInBackground(ByVal oPage As System.Web.UI.Page, ByVal tbIncidences As DataTable, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.JustifiedIncidencesInBackground(tbIncidences, xBeginPeriod, xEndPeriod, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-384")
            End Try

            Return oRet

        End Function

        Public Shared Function AssignCentersInBackground(ByVal oPage As System.Web.UI.Page, ByVal tbIncidences As DataTable, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.AssignCentersInBackground(tbIncidences, xBeginPeriod, xEndPeriod, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-385")
            End Try

            Return oRet

        End Function

        Public Shared Function SecurityActionsInBackground(ByVal oPage As System.Web.UI.Page, ByVal iAction As Integer, strEmployeeFilter As String, strfeautre As String, strFilters As String, strUserFieldFilters As String, bLockAccess As Boolean, iSourceEmployee As Integer, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.SecurityActionsInBackground(iAction, strEmployeeFilter, strfeautre, strFilters, strUserFieldFilters, bLockAccess, iSourceEmployee, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-385")
            End Try

            Return oRet

        End Function

        Public Shared Function CopyShiftsInBackground(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal strDestinationEmployeesFilter As String,
                                          ByVal xBegin As Date, ByVal xEnd As Date, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean _
                                          , ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CopyShiftsInBackground(intSourceIDEmployee, strDestinationEmployeesFilter, xBegin, xEnd, bolCopyMainShifts, bolCopyAlternativeShifts, bolCopyHolidays,
                                                           bolKeepBloquedDays, bolKeepHolidays, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-386")
            End Try

            Return oRet

        End Function

        Public Shared Function CopyAdvShiftsEmployeesInBackground(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As ArrayList, ByVal lstDestinationIDEmployees As ArrayList,
                                          ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal strShiftsSelected As String, ByVal bolCopyMainShifts As Boolean, ByVal bolCopyAlternativeShifts As Boolean, ByVal bolCopyHolidays As Boolean _
                                          , ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal bAudit As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CopyAdvShiftsEmployeesInBackground(intSourceIDEmployee, lstDestinationIDEmployees, xBeginDate, xEndDate, strShiftsSelected, bolCopyMainShifts, bolCopyAlternativeShifts, bolCopyHolidays,
                                                           bolKeepBloquedDays, bolKeepHolidays, oState, bAudit)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-387")
            End Try

            Return oRet

        End Function

        Public Shared Function AssignWeekShiftsInBackground(ByVal oPage As System.Web.UI.Page, ByVal strDestinationEmployeesFilter As String, ByVal lstWeekShifts As ArrayList,
                                                ByVal lstWeekStartShifts As Generic.List(Of DateTime), ByVal lstWeekAssignments As Generic.List(Of Integer),
                                                ByVal xBegin As Date, ByVal xEnd As Date, ByVal bolKeepBloquedDays As Boolean, ByVal bolKeepHolidaysDays As Boolean) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.AssignWeekShiftsInBackground(strDestinationEmployeesFilter, lstWeekShifts, lstWeekStartShifts, lstWeekAssignments, xBegin, xEnd,
                                                           bolKeepBloquedDays, bolKeepHolidaysDays, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-388")
            End Try

            Return oRet

        End Function

        Public Shared Function AssignTemplateInBackground(ByVal oPage As System.Web.UI.Page,
                                              ByVal intIDTemplate As Integer, ByVal intIDGroup As Integer,
                                              ByVal oUserFieldConditions As List(Of roUserFieldCondition),
                                              ByVal intYear As Integer, ByVal intIDShift As Integer, ByVal xStartShift As DateTime,
                                              ByVal bolLockDays As Boolean, ByVal bolIncludeChildGroups As Boolean,
                                              ByVal bolKeepLockedDays As Boolean, ByVal bolKeepHolidays As Boolean) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.AssignScheduleTemplateInBackground(intIDTemplate, intIDGroup, oUserFieldConditions,
                                                                  intYear, intIDShift, xStartShift, bolLockDays, bolIncludeChildGroups, bolKeepLockedDays, bolKeepHolidays, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-389")
            End Try

            Return oRet

        End Function

        Public Shared Function AssignTemplatev2InBackground(ByVal oPage As System.Web.UI.Page,
                                              ByVal intIDTemplate As Integer, ByVal lstDestinationIDEmployees As ArrayList,
                                              ByVal intIDShift As Integer, ByVal xStartShift As DateTime,
                                              ByVal bolLockDays As Boolean,
                                              ByVal bolKeepLockedDays As Boolean, ByVal bolKeepHolidays As Boolean, ByVal bolFeastDays As Boolean, ByVal employeeFilter As String,
                                                            ByVal idUserField As String, ByVal txtUserField As String) As Integer

            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.AssignScheduleTemplatev2InBackground(intIDTemplate, lstDestinationIDEmployees,
                                                                   intIDShift, xStartShift, bolLockDays, bolKeepLockedDays, bolKeepHolidays, oState, bolFeastDays, employeeFilter, idUserField, txtUserField)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-390")
            End Try

            Return oRet

        End Function

        Public Shared Function ExecuteExportInBackground(ByVal oPage As System.Web.UI.Page, ByVal IDExport As Integer, ByVal IDTemplate As Integer, ByVal FileType As String, ByVal EmployeesSelected As String, ByVal inExcel2007Format As Boolean,
                                                         ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date, ByVal isExcelInstalled As Boolean, ByVal ExcelTemplateFile As String, ByVal ConceptGroup As String,
                                                         ByVal Separator As String, ByVal ProfileType As Integer, ByVal bApplyLockDate As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateExportBackground(IDExport, IDTemplate, FileType, EmployeesSelected, inExcel2007Format, ScheduleBeginDate, ScheduleEndDate, inExcel2007Format, ExcelTemplateFile, ConceptGroup, Separator, ProfileType, bApplyLockDate, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-391")
            End Try

            Return oRet
        End Function

        Public Shared Function ExecuteImportInBackground(ByVal oPage As System.Web.UI.Page, ByVal IDImport As Integer, ByVal oFileOrig() As Byte, ByVal oFileSchema() As Byte, ByVal EmployeesSelected As String, ByVal ScheduleBeginDate As Date, ByVal ScheduleEndDate As Date,
                                                         ByVal ExcelIsTemplate As Integer, ByVal CopyMainShifts As Boolean, ByVal CopyHolidays As Boolean, ByVal KeepHolidays As Boolean, ByVal KeepLockedDays As Boolean, ByVal separator As String, ByVal fileType As Integer, Optional ByVal idImportTemplate As Integer = -1) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateImportBackground(IDImport, oFileOrig, oFileSchema, EmployeesSelected, ScheduleBeginDate, ScheduleEndDate, oState, ExcelIsTemplate, CopyMainShifts, CopyHolidays, KeepHolidays, KeepLockedDays, separator, fileType, idImportTemplate)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-392")
            End Try

            Return oRet
        End Function

        Public Shared Function ExecuteCalendarSpecialPaste(ByVal oPage As System.Web.UI.Page, ByVal _copyStartDate As DateTime, ByVal _copyEndDate As DateTime, ByVal _DestStartDate As DateTime, ByVal iIDEmployeeSource As Integer, ByVal iIDEmployeeDestination As Generic.List(Of Integer),
                                                   ByVal iRepeatMode As Integer, ByVal strRepeatModeValue As String, ByVal iRepeatStartMode As Integer, ByVal strRepeatStartModeValue As String,
                                                   ByVal iRepeatSkipMode As Integer, ByVal iRepeatSkiptimes As Integer, ByVal strRepeatSkipModeValue As String, ByVal iBloquedMode As Integer, ByVal iHolidaysMode As Integer,
                                                   ByVal iCopyMainShifts As Integer, ByVal iCopyHolidays As Integer, ByVal iLockDestDays As Integer, ByVal iCopyAssignment As Integer, ByVal iTelecommuteCopyOption As Integer) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateCalendarSpecialPaste(_copyStartDate, _copyEndDate, _DestStartDate, iIDEmployeeSource, iIDEmployeeDestination, iRepeatMode, strRepeatModeValue, iRepeatStartMode, strRepeatStartModeValue,
                                                                                          iRepeatSkipMode, iRepeatSkiptimes, strRepeatSkipModeValue, iBloquedMode, iHolidaysMode, iCopyMainShifts, iCopyHolidays, iLockDestDays, iCopyAssignment, iTelecommuteCopyOption, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-393")
            End Try

            Return oRet
        End Function

        Public Shared Function ExecuteBudgetSpecialPaste(ByVal oPage As System.Web.UI.Page, ByVal _copyStartDate As DateTime, ByVal _copyEndDate As DateTime, ByVal _DestStartDate As DateTime, ByVal iIDNodeSource As Integer, ByVal iIDProductiveUnitSource As Integer,
                                                   ByVal iRepeatMode As Integer, ByVal iHolidaysMode As Integer, ByVal strRepeatModeValue As String, ByVal bolCopyEmployees As Boolean) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateBudgetSpecialPaste(_copyStartDate, _copyEndDate, _DestStartDate, iIDNodeSource, iIDProductiveUnitSource, iRepeatMode, iHolidaysMode, strRepeatModeValue, bolCopyEmployees, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-394")
            End Try

            Return oRet
        End Function

        Public Shared Function CheckCloseDate(ByVal oPage As System.Web.UI.Page) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.LiveTasksMethods.CreateTaskWithoutParameters(DTOs.roLiveTaskTypes.CheckCloseDate, oState, False)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = -1
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-395")
            End Try

            Return oRet
        End Function

        Public Shared Function GetCommonTemplateFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Byte()
            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Byte())
                wsRet = VTLiveApi.LiveTasksMethods.DownloadCommonTemplateAzure(fileName, roLiveQueueTypes.datalink, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-396")
            End Try

            Return oRet
        End Function

        Public Shared Function GetExportedFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Byte()
            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Byte())
                wsRet = VTLiveApi.LiveTasksMethods.DownloadFileAzure(fileName, roLiveQueueTypes.datalink, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-396")
            End Try

            Return oRet
        End Function

        Public Shared Function GetExportedFileZipped(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Byte()
            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Byte())
                wsRet = VTLiveApi.LiveTasksMethods.DownloadFileZippedFromAzure(fileName, roLiveQueueTypes.datalink, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = {}
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                oRet = {}
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-397")
            End Try

            Return oRet
        End Function

        Public Shared Function DownloadFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Byte()
            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Byte()) = VTLiveApi.LiveTasksMethods.DownloadFile(fileName, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-398")
            End Try
            Return oRet
        End Function

        Public Shared Function DownloadFileAzure(ByVal oPage As System.Web.UI.Page, ByVal fileName As String, ByVal Container As roLiveQueueTypes) As Byte()
            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Byte()) = VTLiveApi.LiveTasksMethods.DownloadFileAzure(fileName, Container, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-398")
            End Try
            Return oRet
        End Function

        Public Shared Function DownloadFileAzureToFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String, ByVal destFileName As String, ByVal Container As roLiveQueueTypes) As Byte()
            Dim oRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Byte()) = VTLiveApi.LiveTasksMethods.DownloadFileAzureToFile(fileName, destFileName, Container, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    oRet = Nothing
                Else
                    oRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-398")
            End Try
            Return oRet
        End Function

        Public Shared Function UploadFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String, ByVal filecontent As Byte()) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LiveTasksMethods.UploadFile(fileName, filecontent, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.LiveTaskState, "9-PG03-003")
                    bolRet = Nothing
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-399")
            End Try

            Return bolRet
        End Function

        Public Shared Function DuplicateFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LiveTasksMethods.DuplicateFile(fileName, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.LiveTaskState, "9-PG03-002")
                    bolRet = Nothing
                    HelperWeb.ShowError(oPage, oSession.States.LiveTaskState, "9-PG03-002")
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-400")
            End Try

            Return bolRet
        End Function

        Public Shared Function RemoveFile(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LiveTasksMethods.RemoveFile(fileName, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    bolRet = Nothing
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-401")
            End Try

            Return bolRet
        End Function

        Public Shared Function FileExists(ByVal oPage As System.Web.UI.Page, ByVal fileName As String) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LiveTasksMethods.FileExists(fileName, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.LiveTaskState.Result <> LiveTasksResultEnum.NoError Then
                    bolRet = Nothing
                Else
                    bolRet = wsRet.Value
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-402")
            End Try

            Return bolRet
        End Function

        Public Shared Function DeleteLiveTask(ByVal oPage As System.Web.UI.Page, ByVal IDTask As Integer, ByVal Action As String) As Boolean
            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.LiveTaskState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.LiveTasksMethods.DeleteLiveTask(IDTask, Action, oState)

                oSession.States.LiveTaskState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value
                If Not oSession.States.LiveTaskState.Result = LiveTasksResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.LiveTaskState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-403")
            End Try

            Return oRet
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.LiveTaskState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace