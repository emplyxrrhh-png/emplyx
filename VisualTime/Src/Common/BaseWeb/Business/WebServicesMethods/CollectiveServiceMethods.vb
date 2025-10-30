Imports Robotics.Base.DTOs
Imports Robotics.Base.VTCollectives
Imports Robotics.Base.VTSelectorManager
Imports Robotics.Base.VTUserFields.UserFields

Namespace API

    Public NotInheritable Class CollectiveServiceMethods

        Public Shared Function GetAllCollectives(ByVal oPage As PageBase, Optional loadDefinitions As Boolean = False, Optional audit As Boolean = False) As roCollective()
            Dim oRet As roCollective() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                Dim collectives As Generic.List(Of roCollective) = collectivesManager.GetAllCollectives(loadDefinitions, audit)

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result = CollectiveResult.NoError Then
                    oRet = collectives.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function GetCollective(ByVal idCollective As Integer, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As roCollective
            Dim oRet As roCollective = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                oRet = collectivesManager.GetCollectiveById(idCollective, True, bAudit)

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result <> CollectiveResult.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function GetCollectiveEmployees(collectiveFilterExpression As String, referenceDate As Date, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As List(Of roSelectedEmployee)
            Dim oRet As New List(Of roSelectedEmployee)
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                Dim havingClause As String = collectivesManager.GetHavingClause(collectiveFilterExpression)
                If havingClause = String.Empty Then
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, CollectiveResult.ErorrGeneratingHavingClause)
                    Return oRet
                End If
                oRet = collectivesManager.GetCollectiveEmployees(havingClause, referenceDate, bAudit)

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result <> CollectiveResult.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function CreateOrUpdateCollective(ByVal oPage As PageBase, ByRef collective As roCollective, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                oRet = collectivesManager.CreateOrUpdateCollective(collective, bAudit)

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result <> CollectiveResult.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

        Public Shared Function ValidateCollectiveDefinition(ByVal oPage As PageBase, ByVal collectiveDefinition As roCollectiveDefinition) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                oRet = collectivesManager.ValidateCollectiveDefinition(collectiveDefinition)

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result <> CollectiveResult.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function


        Public Shared Function DeleteCollective(collective As roCollective, ByVal oPage As PageBase, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                oRet = collectivesManager.DeleteCollective(collective, bAudit)

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result = CollectiveResult.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-091")
            End Try

            Return oRet
        End Function

        Public Shared Function GetEmployeeUserFields(ByVal oPage As PageBase) As roUserField()
            Dim oRet As roUserField() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roCollectiveState = oSession.States.CollectiveState

            WebServiceHelper.SetState(oState)

            Try
                Dim collectivesManager As New roCollectivesManager(oState)
                Dim userfields As Generic.List(Of roUserField) = collectivesManager.GetEmployeeUserfields()

                oSession.States.CollectiveState = collectivesManager.State
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CollectiveState.Result = CollectiveResult.NoError Then
                    oRet = userfields.ToArray
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CollectiveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New roWsState With {.Result = 1}
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") & System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet
        End Function

#Region "Last errors"

        Public Shared Function LastCollectiveErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CollectiveState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastCollectiveResult() As CollectiveResult
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CollectiveState.Result
            End If
            Return strRet
        End Function

        Public Shared Function LastMessageErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.MessageState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastMessageResult() As MessageResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.MessageState.Result
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace