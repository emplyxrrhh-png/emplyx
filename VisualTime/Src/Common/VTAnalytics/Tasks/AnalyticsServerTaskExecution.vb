Imports System.Text.RegularExpressions
Imports Robotics.Base.DTOs
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace VTAnalyticsManager

    Public Class AnalyticsServerTaskExecution

#Region "Declarations - Constructor"

        Public Sub New()
        End Sub

#End Region

#Region "Methods"

        Public Function ExecuteTask(ByVal oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As New BaseTaskResult With {.Result = True, .Description = String.Empty}

            Try
                Dim sType As String = oTask.Action.ToUpper

                Select Case sType
                    Case roLiveTaskTypes.GenerateAnalyticsTasks.ToString.ToUpper
                        Dim oAnalyticManager As New VTAnalyticsManager.roAnalyticsManager
                        bolRet = oAnalyticManager.GenerateScheduledTasks()
                    Case roLiveTaskTypes.AnalyticsTask.ToString.ToUpper
                        bolRet = RunAnalyticsTask(oTask)
                End Select

            Catch Ex As Exception
                'Stop
                roLog.GetInstance.logMessage(roLog.EventType.roError, "CAnalyticsServerNetQC::ExecuteTask :", Ex)
            End Try

            Return bolRet
        End Function

        Private Function RunAnalyticsTask(oTask As roLiveTask) As BaseTaskResult
            Dim bolRet As Boolean = True
            Dim taskdescription As String = String.Empty
            Try
                Dim strAnalyticFileName As String = String.Empty

                Dim iAnalyticsVersion As Integer = roTypes.Any2Integer(oTask.Parameters("APIVersion"))

                Dim analyticType As roLiveAnalyticType = roTypes.Any2Integer(oTask.Parameters("AnalyticType"))
                Dim analyticView As Integer = roTypes.Any2Integer(oTask.Parameters("IdView"))

                Dim iIdPasssport As Integer = oTask.IDPassport
                If iIdPasssport = 0 AndAlso iAnalyticsVersion = roConstants.GeniusAnalytic Then
                    Dim strSQL As String = "@SELECT# Id from sysropassports where Description = '@@ROBOTICS@@Consultor'"
                    iIdPasssport = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                End If

                Dim beginPeriod As Date = DateTime.Parse(oTask.Parameters("ScheduleBeginDate"))
                Dim endPeriod As Date = DateTime.Parse(oTask.Parameters("ScheduleEndDate"))
                Dim beginTime As Date = Nothing
                Dim endTime As Date = Nothing
                If oTask.Parameters("ScheduleBeginTime") <> Nothing Then
                    beginTime = DateTime.Parse(oTask.Parameters("ScheduleBeginTime"))
                End If
                If oTask.Parameters("ScheduleEndTime") <> Nothing Then
                    endTime = DateTime.Parse(oTask.Parameters("ScheduleEndTime"))
                End If
                Dim bolIncludeZeroValues As Boolean = roTypes.Any2Boolean(oTask.Parameters("IncludeZeroValues"))
                Dim bolIncludeZeroCauseValues As Boolean = roTypes.Any2Boolean(oTask.Parameters("IncludeZeroCauseValues"))
                Dim bolIncludeUndefinedCenter As Boolean = roTypes.Any2Boolean(oTask.Parameters("IncludeEntriesWithoutBusinessCenter"))

                Dim strEmployees As String = roTypes.Any2String(oTask.Parameters("Employees"))
                Dim strConcepts As String = roTypes.Any2String(oTask.Parameters("Concepts"))
                Dim strUserFields As String = roTypes.Any2String(oTask.Parameters("UserFields"))
                Dim strCostCenters As String = roTypes.Any2String(oTask.Parameters("BusinessCenters"))
                Dim strRequestTypes As String = roTypes.Any2String(oTask.Parameters("RequestTypes"))
                Dim strCauses As String = roTypes.Any2String(oTask.Parameters("Causes"))

                Dim strFeature As String = roTypes.Any2String(oTask.Parameters("Feature"))
                Dim strDBProcedure As String = roTypes.Any2String(oTask.Parameters("DSFunction"))

                Dim bSendMail As Boolean = roTypes.Any2Boolean(oTask.Parameters("SendEmail"))
                Dim bOverwriteResults As Boolean = roTypes.Any2Boolean(oTask.Parameters("OverwriteResults"))
                Dim bDownloadBI As Boolean = roTypes.Any2Boolean(oTask.Parameters("DownloadBI"))
                Dim BIExecutionName As String = Nothing
                If bDownloadBI Then
                    If bOverwriteResults Then
                        Dim invalidChars As String = System.Text.RegularExpressions.Regex.Escape(New String(System.IO.Path.GetInvalidFileNameChars()))
                        Dim invalidRegStr As String = String.Format("([{0}]*\.+$)|([{0}]+)", invalidChars)
                        BIExecutionName = oTask.Parameters("GeniusViewName")
                        BIExecutionName = System.Text.RegularExpressions.Regex.Replace(BIExecutionName, invalidRegStr, "_")
                        BIExecutionName = BIExecutionName & "/" & BIExecutionName
                    Else
                        BIExecutionName = oTask.Parameters("GeniusViewName")
                        Dim iSufix As Integer = 1
                        Dim lCurrentExecutions As New List(Of String)
                        lCurrentExecutions = Azure.RoAzureSupport.ListFiles(BIExecutionName & "_*", "json", roLiveQueueTypes.analyticsbi, BIExecutionName, True, Azure.RoAzureSupport.GetCompanyName())
                        If lCurrentExecutions IsNot Nothing AndAlso lCurrentExecutions.Count > 0 Then
                            Dim regex As New Regex(Regex.Escape(BIExecutionName) & "_(\d+).json$")
                            Dim executionNumbersStr = lCurrentExecutions.Where(Function(s) regex.IsMatch(s))
                            Dim executionNumbers = executionNumbersStr.Select(Function(s) Integer.Parse(regex.Match(s).Groups(1).Value))
                            iSufix = executionNumbers.Max() + 1
                        End If
                        Dim invalidChars As String = System.Text.RegularExpressions.Regex.Escape(New String(System.IO.Path.GetInvalidFileNameChars()))
                        Dim invalidRegStr As String = String.Format("([{0}]*\.+$)|([{0}]+)", invalidChars)
                        BIExecutionName = System.Text.RegularExpressions.Regex.Replace(BIExecutionName, invalidRegStr, "_")
                        BIExecutionName = BIExecutionName & "/" & BIExecutionName & "_" & iSufix.ToString()
                    End If
                End If
                Dim bIsScheduled As Boolean = roTypes.Any2Boolean(oTask.Parameters("IsScheduled"))

                Dim geniusExecution As roGeniusExecution = Nothing
                If iAnalyticsVersion = roConstants.GeniusAnalytic Then
                    roTrace.GetInstance().AddTraceEvent("Recovering execution with IDTask " & oTask.ID)
                    Dim strSQL As String = "@SELECT# ID from GeniusExecutions WHERE IDTask=" & oTask.ID
                    Dim executionID As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                    roTrace.GetInstance().AddTraceEvent("Get execution with ID " & executionID)
                    Dim oGeniusManager As New Analytics.Manager.roGeniusViewManager(New Analytics.Manager.roAnalyticState(iIdPasssport))
                    roTrace.GetInstance().AddTraceEvent("Initializing Genius Manager")
                    geniusExecution = oGeniusManager.GetGeniusExecutionById(executionID)
                    roTrace.GetInstance().AddTraceEvent("Load execution " & geniusExecution.Id)
                End If

                Dim oLng = New roLanguage()
                oLng.SetLanguageReference("LiveGenius", roTypes.Any2String(oTask.Parameters("DSLanguage")))

                Dim oAnalyticManager As New roAnalyticsManager

                If strDBProcedure = String.Empty Then
                    Select Case analyticType
                        Case roLiveAnalyticType.Audit
                            strAnalyticFileName = oAnalyticManager.GenerateAuditCube(1, iIdPasssport, beginPeriod, endPeriod, roLog.GetInstance())
                        Case Else
                            strAnalyticFileName = String.Empty
                    End Select
                Else
                    If iAnalyticsVersion = roConstants.GeniusAnalytic Then
                        strAnalyticFileName = oAnalyticManager.GenerateDBCube(strDBProcedure, analyticView, iIdPasssport, beginPeriod, endPeriod, strEmployees, strConcepts, strCostCenters, strUserFields, strRequestTypes, strCauses,
                                                                              bolIncludeZeroValues, bolIncludeZeroCauseValues, bolIncludeUndefinedCenter, strFeature, oLng, beginTime, endTime, geniusExecution, BIExecutionName, bOverwriteResults)
                    End If
                End If

                If strAnalyticFileName <> String.Empty AndAlso oTask.State.Result = LiveTasksResultEnum.NoError Then
                    taskdescription = Azure.RoAzureSupport.GetCompanyName() & "/" & strAnalyticFileName

                    If iAnalyticsVersion = roConstants.GeniusAnalytic AndAlso geniusExecution IsNot Nothing Then
                        geniusExecution.FileLink = strAnalyticFileName

                        If bDownloadBI Then
                            geniusExecution.SASLink = Azure.RoAzureSupport.GetFileSaSTokenWithURI(strAnalyticFileName, roLiveQueueTypes.analyticsbi, True, Azure.RoAzureSupport.GetCompanyName())
                        End If

                        Dim oGeniusManager As New Analytics.Manager.roGeniusViewManager(New Analytics.Manager.roAnalyticState(iIdPasssport))
                        oGeniusManager.UpdateGeniusExecution(geniusExecution)

                        'Enviar email con resultado exportación.
                        If bSendMail AndAlso bIsScheduled Then
                            oGeniusManager.SendEmail(geniusExecution, oTask.ID)
                        End If
                    End If

                    bolRet = True
                Else
                    taskdescription = oTask.State.ErrorText
                    bolRet = False
                End If
            Catch ex As Exception
                bolRet = False
                roLog.GetInstance.logMessage(roLog.EventType.roError, "AnalyticsTaskExecution::RunAnalyticsTask::", ex)
            End Try

            Return New BaseTaskResult With {.Result = True, .Description = taskdescription}
        End Function

#End Region


    End Class

End Namespace