Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class DocumentsServiceMethods

#Region "documentos v2"

        Public Shared Function GetTemplateDocumentsList(ByVal bFilter As Boolean, ByVal docType As DocumentScope, oPage As Page, bAudit As Boolean) As List(Of roDocumentTemplate)
            Dim oRet As roDocumentTemplateListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetTemplateDocuments(bFilter, docType, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el errorl
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateListResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-188")
            End Try

            Return oRet.DocumentTemplates.ToList
        End Function

        Public Shared Function GetCauseAvailableDocumentTemplateByEmployee(ByVal idEmployee As Integer, ByVal idCause As Integer, ByVal bIsStarting As Boolean, oPage As Page, bAudit As Boolean) As List(Of roDocumentTemplate)
            Dim oRet As roDocumentTemplateListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetCauseAvailableDocumentTemplateByEmployee(idEmployee, idCause, bIsStarting, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateListResponse()
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-189")
            End Try

            Return oRet.DocumentTemplates.ToList
        End Function

        Public Shared Function GetTemplateDocumentsAvailableByAbsence(ByVal idAbsence As Integer, ByVal idRelatedObject As Integer, ByVal isStarting As Boolean, ByVal eforecast As ForecastType, oPage As Page, bAudit As Boolean) As List(Of roDocumentTemplate)
            Dim oRet As roDocumentTemplateListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetTemplateDocumentsAvailableByAbsence(idAbsence, idRelatedObject, isStarting, eforecast, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-190")
            End Try

            Return oRet.DocumentTemplates.ToList
        End Function

        Public Shared Function GetAvailableDocumentTemplateList(ByVal oDoctype As DocumentType, ByVal idRelatedObject As Integer, ByVal minPermission As Permission, ByVal oPage As Page, bAudit As Boolean) As List(Of roDocumentTemplate)
            Dim oRet As roDocumentTemplateListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetAvailableDocumentTemplateByType(oDoctype, idRelatedObject, minPermission, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-191")
            End Try

            Return oRet.DocumentTemplates.ToList
        End Function

        Public Shared Function GetDocumentTemplateListbyType(ByVal oDoctype As DocumentType, oPage As Page, bAudit As Boolean) As List(Of roDocumentTemplate)
            Dim oRet As roDocumentTemplateListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetTemplateDocumentsByType(oDoctype, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-192")
            End Try

            Return oRet.DocumentTemplates.ToList
        End Function

        Public Shared Function GetAccessAuthorizationsTemplates(oPage As Page, ByVal bAudit As Boolean) As List(Of roDocumentTemplate)
            Dim oRet As roDocumentTemplateListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetAccessAuthorizationsTemplates(bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-193")
            End Try

            Return oRet.DocumentTemplates.ToList
        End Function

        Public Shared Function GetDocumentTemplateById(ByVal oPage As Page, ByVal idDocument As Integer, ByVal bAudit As Boolean) As roDocumentTemplate
            Dim oRet As roDocumentTemplateResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentTemplateById(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-194")
            End Try
            Return oRet.DocumentTemplate
        End Function

        Public Shared Function SaveDocumentTemplate(oDocument As roDocumentTemplate, ByVal oPage As Page, ByVal bAudit As Boolean) As roDocumentTemplate
            Dim oRet As roDocumentTemplateResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.DocumentsMethods.SaveDocumentTemplate(oDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentTemplateResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-195")
            End Try
            Return oRet.DocumentTemplate
        End Function

        Public Shared Function DeleteDocumentTemplate(oDocument As roDocumentTemplate, oPage As Page, bAudit As Boolean) As Boolean
            Dim oRet As roDocumentStandarResponse = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)
            Try
                oRet = VTLiveApi.DocumentsMethods.DeleteDocumentTemplate(oDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession
                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentStandarResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-196")
            End Try
            Return oRet.Result
        End Function

        Public Shared Function CanEditDocument(idDocument As Integer, ByVal oPage As Page, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roDocumentPermission = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.CanEditDocument(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentPermission
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-197")
            End Try

            Return oRet.CanEditDocument
        End Function

        Public Shared Function RegisterA3Payroll(ByVal oPage As Page, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roDocumentStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.RegisterA3Payroll(bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentStandarResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-197")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function UnRegisterA3Payroll(ByVal oPage As Page, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roDocumentStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.UnRegisterA3Payroll(bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentStandarResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-197")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function GetDocumentsbyRequest(idRequest As Integer, ByVal oPage As Page, ByVal bAudit As Boolean) As Generic.List(Of Integer)
            Dim oRet As New List(Of Integer)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentsbyRequest(idRequest, bAudit, oState)
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-197")
            End Try

            Return oRet
        End Function

        Public Shared Function CanAccessRequestDocumentation(idDocument As Integer, ByVal oPage As Page, ByVal bAudit As Boolean) As Integer
            Dim oRet As roRequestDocument = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.CanAccessRequestDocumentation(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roRequestDocument
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-197")
            End Try

            Return oRet.DocumentId
        End Function

        Public Shared Function SaveDocument(oDocument As roDocument, ByVal oPage As Page, ByVal bAudit As Boolean) As roDocument
            Dim oRet As roDocumentResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.SaveDocument(oDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-198")
            End Try

            Return oRet.Document
        End Function

        Public Shared Function GetDocumentEmployeesByGroup(idGroup As Integer, oPage As Page, bAudit As Boolean) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentEmployeesByGroup(idGroup, "", "", bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-199")
            End Try

            Return oRet.Documents.ToList
        End Function

        Public Shared Function GetAllDocumentEmployeesBySelection(employees As String, filter As String, filterUser As String, oPage As Page, bAudit As Boolean) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetAllDocumentEmployeesBySelection(employees, filter, filterUser, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-199")
            End Try

            Return oRet.Documents.ToList
        End Function

        Public Shared Function GetDocumentsByTemplateId(templateId As Integer, oPage As Page, bAudit As Boolean, Optional searchTerm As String = "", Optional showAll As Boolean = False) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentsByTemplateId(templateId, bAudit, oState, searchTerm, showAll)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-199")
            End Try

            Return oRet.Documents.ToList
        End Function

        Public Shared Function GetSystemDocumentList(ByVal scope As DocumentScope, oPage As Page, bAudit As Boolean) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetSystemDocumentList(scope, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-199")
            End Try

            Return oRet.Documents.ToList
        End Function

        Public Shared Function GetDocumentsByFilterName(filterExpression As String, oPage As Page, bAudit As Boolean) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentsByFilterName(filterExpression, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-199")
            End Try

            Return oRet.Documents.ToList
        End Function

        Public Shared Function GetDocumentsByType(idRelatedDocument As Integer, type As DocumentType, oPage As Page, bAudit As Boolean) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentsByType(type, idRelatedDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentListResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-199")
            End Try

            Return oRet.Documents.ToList
        End Function

        Public Shared Function ChangeDeliveredDocumentState(ByVal idDocument As Integer, ByVal oNewDocStatus As DocumentStatus, ByVal strSupervisorRemark As String, ByVal updateStatusDate As DateTime, oPage As Page, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roDocumentStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.UpdateDocumentStatus(idDocument, oNewDocStatus, strSupervisorRemark, updateStatusDate, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentStandarResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-200")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function GetDocumentById(ByVal oPage As Page, ByVal idDocument As Integer, ByVal bAudit As Boolean) As roDocument
            Dim oRet As roDocumentResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentById(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-201")
            End Try

            Return oRet.Document

        End Function


        Public Shared Function GetBioCertificateBytes(ByVal oPage As Page, ByVal certificateName As String, ByVal bAudit As Boolean) As roDocumentFile
            Dim oRet As roDocumentFileRespone = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetBioCertificateBytes(certificateName, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentFileRespone

                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-202")
            End Try
            Return oRet.DocumentFile

        End Function

        Public Shared Function GetDocumentFile(ByVal oPage As Page, ByVal idDocument As Integer, ByVal bAudit As Boolean) As roDocumentFile
            Dim oRet As roDocumentFileRespone = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentFileById(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentFileRespone

                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-202")
            End Try
            Return oRet.DocumentFile

        End Function

        Public Shared Function GetSignReportDocumentBytesById(ByVal oPage As Page, ByVal idDocument As Integer, ByVal bAudit As Boolean) As roDocumentFile
            Dim oRet As roDocumentFileRespone = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetSignReportDocumentBytesById(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentFileRespone

                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-205")
            End Try
            Return oRet.DocumentFile

        End Function

        Public Shared Function DeleteDocument(ByVal oPage As Page, ByVal idDocument As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roDocumentStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.DeleteDocument(idDocument, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentStandarResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-203")
            End Try
            Return oRet.Result
        End Function

        Public Shared Function CheckIfEmployeeDocumentExists(ByVal oPage As Page, ByVal document As roDocument, ByVal bAudit As Boolean) As roDocument
            Dim oRet As roDocumentResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.CheckIfEmployeeDocumentExists(document, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-203")
            End Try
            Return oRet.Document
        End Function

        Public Shared Function GetDocumentationFaults(oPage As Page, documentType As DocumentType, Optional idRelatedObject As Integer = -1, Optional idForecastObject As Integer = 0, Optional ByVal oFilterForecast As ForecastType = ForecastType.Any, Optional checkStatusLevel As Boolean = False) As DocumentAlerts
            Dim oRet As DocumentAlertsRespone = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.GetDocumentationFaultAlerts(idRelatedObject, documentType, oFilterForecast, idForecastObject, oState, checkStatusLevel)

                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New DocumentAlertsRespone
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-204")
            End Try

            If (oRet IsNot Nothing) Then Return oRet.DocumentAlerts
            Return Nothing
        End Function

        Public Shared Function SignStatusDocumentInProgress(ByVal oPage As Page, ByVal idDocument As Integer, ByVal GUIDDoc As String, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roDocumentStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.SignStatusDocumentInProgress(idDocument, GUIDDoc, bAudit, oState)
                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                oRet = New roDocumentStandarResponse
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-207")
            End Try
            Return oRet.Result
        End Function

        Public Shared Function SplitA3PayrollDocument(ByVal oPage As Page, ByVal document As roDocument, ByVal bAudit As Boolean) As List(Of roDocument)
            Dim oRet As roDocumentListResponse = New roDocumentListResponse

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DocumentState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.DocumentsMethods.SplitA3PayrollDocument(document, bAudit, oState)

                oSession.States.DocumentState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DocumentState.Result <> DocumentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-201")
            End Try

            Return oRet.Documents.ToList

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.DocumentState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastResult() As DocumentResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.DocumentState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace