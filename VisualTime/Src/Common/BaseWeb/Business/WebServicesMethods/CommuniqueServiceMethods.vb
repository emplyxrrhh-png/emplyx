Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class CommuniqueServiceMethods

        Public Function CreateOrUpdateCommunique(ByVal oPage As PageBase, ByVal oCommunique As roCommunique, Optional ByVal bAudit As Boolean = False) As roCommuniqueResponse
            Dim wsRet As roCommuniqueResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                wsRet = VTLiveApi.CommuniqueMethods.CreateOrUpdateCommunique(oCommunique, oState, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result <> CommuniqueResultEnum.NoError Then
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return wsRet
        End Function

        Public Function DeleteCommunique(ByVal oPage As PageBase, ByVal idCommunique As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueStandarResponse = VTLiveApi.CommuniqueMethods.DeleteCommunique(idCommunique, oState, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Result
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function GetCommuniquesByCreator(ByVal oPage As PageBase, ByVal creatorId As Integer) As roCommunique()
            Dim oRet As roCommunique() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueListResponse = VTLiveApi.CommuniqueMethods.GetCommuniquesByCreator(oState, creatorId)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Communiques
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Function GetAllCommuniques(ByVal oPage As PageBase, Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As roCommunique()
            Dim oRet As roCommunique() = {}
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueListResponse = VTLiveApi.CommuniqueMethods.GetAllCommuniques(oState, idEmployee, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Communiques
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Function GetCommuniqueStatus(ByVal oPage As PageBase, ByVal idCommunique As Integer, Optional idEmployee As Integer = 0, Optional ByVal bAudit As Boolean = False) As roCommuniqueWithStatistics
            Dim oRet As roCommuniqueWithStatistics = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueStatusResponse = VTLiveApi.CommuniqueMethods.GetCommuniqueWithStatistics(idCommunique, oState, idEmployee, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Status
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Function GetAllEmployeeCommuniquesWithStatus(ByVal oPage As PageBase, ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As roCommuniqueWithStatistics()
            Dim oRet() As roCommuniqueWithStatistics = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roEmployeeCommuniquesStatusResponse = VTLiveApi.CommuniqueMethods.GetAllEmployeeCommuniquesWithStatus(idEmployee, oState, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Status
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Function GetCommuniqueForEmployee(ByVal oPage As PageBase, ByVal idCommunique As Integer, ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As roCommuniqueWithStatistics
            Dim oRet As roCommuniqueWithStatistics = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueStatusResponse = VTLiveApi.CommuniqueMethods.GetCommuniqueWithStatistics(idCommunique, oState, idEmployee, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Status
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Function SetCommuniqueRead(ByVal oPage As PageBase, ByVal idCommunique As Integer, ByVal idEmployee As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueStandarResponse = VTLiveApi.CommuniqueMethods.SetCommuniqueRead(idCommunique, idEmployee, oState, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Result
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Function AnswerCommunique(ByVal oPage As PageBase, ByVal idCommunique As Integer, ByVal idEmployee As Integer, ByVal sAnswer As String, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CommuniqueState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roCommuniqueStandarResponse = VTLiveApi.CommuniqueMethods.AnswerCommunique(idCommunique, idEmployee, sAnswer, oState, bAudit)

                oSession.States.CommuniqueState = wsRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CommuniqueState.Result = CommuniqueResultEnum.NoError Then
                    oRet = wsRet.Result
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CommuniqueState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-063")
            End Try

            Return oRet
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CommuniqueState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastErrorCode() As CommuniqueResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CommuniqueState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace