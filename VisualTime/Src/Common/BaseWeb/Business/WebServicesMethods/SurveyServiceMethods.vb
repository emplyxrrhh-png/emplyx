Imports Robotics.Base.DTOs
Imports Robotics.Base.VTSurveys

Namespace API

    Public NotInheritable Class SurveyServiceMethods

        Public Shared Function GetAllSurveys(ByVal oPage As PageBase, Optional idEmployee As Integer = 0) As roSurvey()
            Dim oRet As roSurvey() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                Dim oSurveys As Generic.List(Of roSurvey) = oSurveyManager.GetAllSurveys(idEmployee)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    oRet = oSurveys.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function GetSurvey(ByVal idSurvey As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roSurvey
            Dim oRet As roSurvey = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.GetSurvey(idSurvey,, bAudit)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function CreateOrUpdateSurvey(ByVal oPage As PageBase, ByRef oSurvey As roSurvey, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.CreateOrUpdateSurvey(oSurvey, True)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function UpdateSurveyContent(ByVal oPage As PageBase, ByVal oSurvey As roSurvey, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.UpdateSurveyContent(oSurvey, True)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function DeleteSurvey(oSurvey As roSurvey, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.DeleteSurvey(oSurvey, bAudit)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Function SaveSurveyResponse(oSurveyResponse As roSurveyResponse, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.SaveSurveyResponse(oSurveyResponse, bAudit)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function GetSurveyTemplates(ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roSurveyTemplates
            Dim oRet As roSurveyTemplates = New roSurveyTemplates
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.GetSurveyTemplates(bAudit)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function GetSurveyResponses(ByVal idSurvey As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roSurveyResponses
            Dim oRet As roSurveyResponses = New roSurveyResponses
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.GetSurveyResponses(idSurvey, bAudit)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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

        Public Shared Function GetSurveyResponsesByIdEmployee(ByVal idSurvey As Integer, ByVal idEmployee As Integer(), ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roSurveyResponses
            Dim oRet As roSurveyResponses = New roSurveyResponses
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roSurveyState = oSession.States.SurveyState

            WebServiceHelper.SetState(oState)

            Try
                Dim oSurveyManager As New roSurveyManager(oState)
                oRet = oSurveyManager.GetSurveyResponsesByIdEmployee(idSurvey, idEmployee, bAudit)

                oSession.States.SurveyState = oSurveyManager.State
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.SurveyState.Result = SurveyResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.SurveyState)
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
                strRet = roWsUserManagement.SessionObject().States.SurveyState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As SurveyResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SurveyState.Result
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace