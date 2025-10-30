Imports Robotics.Base.Analytics.Manager
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTAnalyticsManager
Imports Robotics.VTBase.Extensions

Namespace API

    Public NotInheritable Class GeniusServiceMethods

        Public Shared Function GetGeniusViewById(ByVal oPage As PageBase, ByVal id As Integer, ByVal bAudit As Boolean) As roGeniusView

            Dim oLst As New roGeniusView

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.Load(id, bAudit)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusViewSharedListById(ByVal oPage As PageBase, ByVal id As Integer, ByVal bAudit As Boolean) As List(Of String)

            Dim oLst As List(Of String) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.GetSharedList(id)

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusPlanificationById(ByVal oPage As PageBase, ByVal id As Integer, ByVal bAudit As Boolean) As roGeniusScheduler

            Dim oLst As New roGeniusScheduler

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusSchedulerManager(oState)
                oLst = oManager.Load(id, bAudit)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusViewByCheckBoxes(ByVal oPage As PageBase, ByVal checkedCheckBoxes As Integer(), ByVal bAudit As Boolean) As roGeniusView

            Dim oLst As New roGeniusView

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.LoadByCheckBoxes(checkedCheckBoxes, bAudit)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusViewByTask(ByVal oPage As PageBase, ByVal id As Integer, ByVal bAudit As Boolean) As roGeniusView

            Dim oLst As New roGeniusView

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.LoadByTask(id, bAudit)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusViewsTemplates(ByVal oPage As PageBase) As List(Of roGeniusView)

            Dim oLst As New List(Of roGeniusView)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.GetGeniusViewsTemplates()
                oLst = oLst.FindAll(Function(v) v.RequieredLicense Is String.Empty Or HelperSession.GetFeatureIsInstalledFromApplication(v.RequieredLicense))

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusCheckboxes(ByVal oPage As PageBase) As List(Of roGeniusCheckbox)

            Dim oLst As New List(Of roGeniusCheckbox)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusCheckboxManager(oState)
                oLst = oManager.GetGeniusCheckboxes()

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetUserGeniusViews(ByVal oPage As PageBase) As List(Of roGeniusView)

            Dim oLst As New List(Of roGeniusView)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.GetGeniusViews()
                oLst = oLst.FindAll(Function(v) v.RequieredLicense Is String.Empty Or HelperSession.GetFeatureIsInstalledFromApplication(v.RequieredLicense))

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetUserGeniusPlanifications(ByVal oPage As PageBase, ByRef id As Integer, ByRef idPassport As Integer) As List(Of roGeniusScheduler)

            Dim oLst As New List(Of roGeniusScheduler)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusSchedulerManager(oState)
                oLst = oManager.GetReportSchedulers("IdGeniusView = " + id.ToString() + " AND IdPassport = " + idPassport.ToString())

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function ExecuteGeniusView(ByVal oPage As PageBase, ByVal oView As roGeniusView) As roGeniusExecution

            Dim oLst As New roGeniusExecution

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.Execute(oView, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function ShareGeniusView(ByRef oPage As PageBase, ByVal oView As roGeniusView, Optional ByVal idPassport As Integer = Nothing, Optional ByVal users As Array = Nothing) As Boolean

            Dim bRet As Boolean = True

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                If idPassport > 0 Then
                    Dim idParentShared As Integer = oView.Id
                    If users IsNot Nothing AndAlso users.Length > 0 Then
                        For Each idUser As Object In users
                            If (oManager.Share(oView, idUser, True, idParentShared).Equals(False)) Then
                                bRet = False
                            End If
                        Next

                    End If
                Else
                    bRet = oManager.Share(oView, idPassport, True)
                End If

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return bRet
        End Function

        Public Shared Function SaveGeniusView(ByRef oPage As PageBase, ByVal oView As roGeniusView) As Boolean

            Dim oLst As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.Save(oView, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function SaveGeniusPlanification(ByRef oPage As PageBase, ByVal oSchedule As roGeniusScheduler) As Boolean

            Dim oLst As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusSchedulerManager(oState)
                oLst = oManager.Save(oSchedule, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function DeleteGeniusView(ByVal oPage As PageBase, ByVal oView As roGeniusView) As Boolean

            Dim oLst As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.Delete(oView, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function DeleteGeniusPlanification(ByVal oPage As PageBase, ByVal oSchedule As roGeniusScheduler) As Boolean

            Dim oLst As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusSchedulerManager(oState)
                oLst = oManager.Delete(oSchedule, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusExecutionById(ByVal oPage As PageBase, ByVal intIdExecution As Integer) As roGeniusExecution

            Dim oLst As roGeniusExecution = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try
                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.GetGeniusExecutionById(intIdExecution, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function GetGeniusExecutionWithSasKeyById(ByVal oPage As PageBase, ByVal intIdExecution As Integer) As roGeniusExecution

            Dim oLst As roGeniusExecution = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try
                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.GetGeniusExecutionByIdWithSasKey(intIdExecution, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function DeleteGeniusBIExecution(ByVal oPage As PageBase, ByVal iGeniusExecutionID As Integer, ByVal sFileName As String) As Boolean

            Dim oLst As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try

                Dim oManager As New roGeniusViewManager(oState)
                Dim oLicense As New roServerLicense
                If oLicense.FeatureIsInstalled("Feature\BIIntegration") Then
                    oLst = oManager.DeleteExecution(iGeniusExecutionID, sFileName, True, True) 'Por ahora, cuando pase por aquí siempre será BIExecution (último parámetro)
                End If

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function UpdateGeniusViewLayout(ByVal oPage As PageBase, ByVal geniusExecutions As roGeniusExecution) As Boolean

            Dim oLst As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roAnalyticState = oSession.States.AnalyticsState

            WebServiceHelper.SetState(oState)

            Try
                Dim oManager As New roGeniusViewManager(oState)
                oLst = oManager.UpdateGeniusViewLayout(geniusExecutions, True)

                oSession.States.AnalyticsState = oManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return oLst
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AnalyticsState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastErrorCode() As AuditResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AnalyticsState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace