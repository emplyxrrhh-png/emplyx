Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Concept

Namespace API

    Public NotInheritable Class ConceptsServiceMethods

        Public Shared Function GetConcepts(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetConcepts(oState)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-097")
            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidaysConcepts(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetHolidaysConcepts(oState)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-098")
            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptList(ByVal oPage As System.Web.UI.Page, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetConceptList(oState, filterBusinessGroups)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-099")
            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptsDataTable(ByVal oPage As System.Web.UI.Page, Optional ByVal SQLFilter As String = "") As DataTable

            Dim dtRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetConceptDataset(oState)

                Dim dsAux As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If dsAux IsNot Nothing AndAlso dsAux.Tables.Count > 0 Then
                        dtRet = dsAux.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-100")
            End Try

            Return dtRet

        End Function

        Public Shared Function GetConceptsGroups(ByVal oPage As System.Web.UI.Page, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetConceptGroups(filterBusinessGroups, oState)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-101")
            End Try

            Return oRet

        End Function

        Public Shared Function GetReportGroups(ByVal oPage As System.Web.UI.Page, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetReportGroups(filterBusinessGroups, oState)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-102")
            End Try

            Return oRet

        End Function

        Public Shared Function GetReportGroupConcepts(ByVal oPage As System.Web.UI.Page, ByVal intIDReportGroup As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetReportGroupsConcepts(intIDReportGroup, oState)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-103")
            End Try

            Return oRet

        End Function

        Public Shared Function GetBusinessGroupFromConceptGroups(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim tbRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetBusinessGroupFromConceptGroups(oState)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                        tbRet = ds.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-104")
            End Try

            Return tbRet

        End Function

        Public Shared Function GetConceptByID(ByVal oPage As System.Web.UI.Page, ByVal intIDConcept As Integer, ByVal bAudit As Boolean) As roConcept

            Dim oRet As roConcept = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roConcept) = VTLiveApi.ConceptsMethods.GetConceptByID(intIDConcept, oState, bAudit)

                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-105")
            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptByShortName(ByVal oPage As System.Web.UI.Page, ByVal strShortName As String, ByVal bAudit As Boolean) As roConcept

            Dim oRet As roConcept = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roConcept) = VTLiveApi.ConceptsMethods.GetConceptByShortName(strShortName, oState, bAudit)

                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-106")
            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptGroupByID(ByVal oPage As System.Web.UI.Page, ByVal intIDConceptGroup As Integer, ByVal bAudit As Boolean) As roConceptGroup

            Dim oRet As roConceptGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roConceptGroup) = VTLiveApi.ConceptsMethods.GetConceptGroupByID(intIDConceptGroup, oState, bAudit)

                oRet = response.Value

                oSession.States.ConceptsState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-107")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda el acumulado
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oConcept"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveConcept(ByVal oPage As System.Web.UI.Page, ByRef oConcept As roConcept, ByVal ClosingDate As Nullable(Of Date), ByVal DefinitionHasChanged As Boolean, ByVal bAudit As Boolean) As Boolean
            Dim oRet As (Boolean, roConcept) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                'oRet = VTLiveApi.ConceptsMethods.SaveConcept(oConcept, ClosingDate, DefinitionHasChanged, oState, bAudit).Value
                'oConcept = oRet.Item2

                'oSession.States.ConceptsState = oState
                'roWsUserManagement.SessionObject = oSession

                Dim response As roGenericVtResponse(Of System.ValueTuple(Of Boolean, roConcept)) = VTLiveApi.ConceptsMethods.SaveConcept(oConcept, ClosingDate, DefinitionHasChanged, oState, bAudit)
                oRet.Item1 = response.Value.Item1
                oConcept = response.Value.Item2

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-108")
            End Try

            Return oRet.Item1
        End Function

        ''' <summary>
        ''' Guarda el acumulado
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oConceptGroup"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveConceptGroup(ByVal oPage As System.Web.UI.Page, ByRef oConceptGroup As roConceptGroup, ByVal bAudit As Boolean) As Boolean
            Dim oRet As (Boolean, roConceptGroup) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of System.ValueTuple(Of Boolean, roConceptGroup)) = VTLiveApi.ConceptsMethods.SaveConceptGroup(oConceptGroup, oState, bAudit)
                oRet.Item1 = response.Value.Item1
                oConceptGroup = response.Value.Item2

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-109")
            End Try

            Return oRet.Item1
        End Function

        ''' <summary>
        ''' Elimina el acumulado
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="ID">ID del acumulado a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteConcept(ByVal oPage As System.Web.UI.Page, ByVal ID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.ConceptsMethods.DeleteConcept(ID, oState, bAudit)

                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-110")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Elimina el grupo de acumulados
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="IDGroup">ID del grupo de acumulados a eliminar</param>
        ''' <returns>Devuelve TRUE si se ha podido eliminar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteConceptGroup(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer, ByVal bAudit As Boolean) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.ConceptsMethods.DeleteConceptGroup(IDGroup, oState, bAudit)

                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-111")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Comprueba si los datos del acumulado són validos
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oConcept">Acumulado a validar</param>
        ''' <returns>Devuelve TRUE si los datos són correctos</returns>
        ''' <remarks></remarks>
        Public Shared Function ValidateConcept(ByVal oPage As System.Web.UI.Page, ByVal oConcept As roConcept) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.ConceptsMethods.ValidateConcept(oConcept, oState)

                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-112")
            End Try

            Return oRet

        End Function

        Public Shared Function GetConceptOldestDate(ByVal oPage As System.Web.UI.Page, ByVal _IDConcept As Integer) As Nullable(Of Date)

            Dim oRet As Nullable(Of Date)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of System.Nullable(Of Date)) = VTLiveApi.ConceptsMethods.GetConceptOldestDate(_IDConcept, oState)

                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-113")
            End Try

            Return oRet

        End Function

        Public Shared Function SetReportGroupConceptPosition(ByVal oPage As System.Web.UI.Page, ByVal IDReportGroup As Integer, ByVal IDConcept As Integer, ByVal bolUp As Boolean) As Boolean

            Dim oRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.ConceptsMethods.SetReportGroupConceptPosition(IDReportGroup, IDConcept, bolUp, oState)
                oRet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result <> ConceptResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-114")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ConceptsState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function GetHolidaysConceptsSummaryByEmployee(ByVal oPage As System.Web.UI.Page, ByVal idEmployee As Integer, ByVal idShift As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetHolidaysConceptsSummaryByEmployee(oState, idEmployee, idShift)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-098")
            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidaysConceptsDetailByEmployee(ByVal oPage As System.Web.UI.Page, ByVal idEmployee As Integer, ByVal iSelectedShiftID As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ConceptsState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.ConceptsMethods.GetHolidaysConceptsDetailByEmployee(oState, iSelectedShiftID, idEmployee)

                Dim ds As DataSet = response.Value

                oSession.States.ConceptsState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ConceptsState.Result = ConceptResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ConceptsState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-098")
            End Try

            Return oRet

        End Function
    End Class

End Namespace