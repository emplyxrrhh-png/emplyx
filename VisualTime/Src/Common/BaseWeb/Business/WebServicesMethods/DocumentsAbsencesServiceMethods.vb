Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.DocumentAbsence

Namespace API

    Public NotInheritable Class DocumentsAbsencesServiceMethods

#Region "Documents"

        Public Shared Function GetDocumentsAbsences(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.DocumentAbsenceMethods.GetDocumentsDataSet(oState)

                oSession.States.DocumentAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentAbsenceState.Result = DocumentAbsenceResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.DocumentAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-182")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDocumentAbsenceByID(ByVal oPage As System.Web.UI.Page, ByVal intIDDocument As Integer, ByVal bAudit As Boolean) As roDocumentAbsence

            Dim oRet As roDocumentAbsence = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentAbsenceState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roDocumentAbsence) = VTLiveApi.DocumentAbsenceMethods.GetDocumentByID(intIDDocument, oState, bAudit)

                oSession.States.DocumentAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentAbsenceState.Result <> DocumentAbsenceResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DocumentAbsenceState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-183")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDocumentAbsenceAdviceByID(ByVal oPage As System.Web.UI.Page, ByVal IDAdvice As Integer) As roDocumentAbsenceAdvice
            Dim oRet As roDocumentAbsenceAdvice = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentAbsenceState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roDocumentAbsenceAdvice) = VTLiveApi.DocumentAbsenceMethods.GetDocumentAbsenceAdviceByID(IDAdvice, oState)

                oSession.States.DocumentAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentAbsenceState.Result <> DocumentAbsenceResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DocumentAbsenceState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-184")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda el convenio
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oDocument"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveDocumentAbsence(ByVal oPage As System.Web.UI.Page, ByRef oDocument As roDocumentAbsence, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roDocumentAbsence) = VTLiveApi.DocumentAbsenceMethods.SaveDocument(oDocument, oState, bAudit)

                oSession.States.DocumentAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.DocumentAbsenceState.Result <> DocumentAbsenceResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DocumentAbsenceState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oDocument = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-185")
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Elimina el convenio
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="ID">ID del convenio a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteDocumentAbsence(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DocumentAbsenceMethods.DeleteDocumentByID(ID, oState, bAudit)

                oSession.States.DocumentAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.DocumentAbsenceState.Result <> DocumentAbsenceResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DocumentAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-186")
            End Try

            Return bolRet

        End Function

        Public Shared Function ExitsDocumentAbsence(ByVal oPage As System.Web.UI.Page, ByVal IDDocumentAbsence As Integer) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentAbsenceState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DocumentAbsenceMethods.ExitsDocument(IDDocumentAbsence, oState)

                oSession.States.DocumentAbsenceState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.DocumentAbsenceState.Result <> DocumentAbsenceResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DocumentAbsenceState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-187")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.DocumentAbsenceState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace