Imports Robotics.Base.DTOs
Imports Robotics.Base.VTDataLink.DataLink

Namespace API

    Public NotInheritable Class DataLinkServiceMethods

        ''' <summary>
        ''' Recuperació de las plantillas de Excel
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTemplatesExcel(ByVal oPage As System.Web.UI.Page) As String()
            Dim myList As String() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsret As roGenericVtResponse(Of List(Of String)) = VTLiveApi.DataLinkBaseMethods.GetTemplatesExcel(oState)

                myList = wsret.Value.ToArray
                oSession.States.DataLinkBaseState = wsret.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-139")
            End Try

            Return myList

        End Function

        ''' <summary>
        ''' Recuperació de les guies d'importació
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetImports(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.DataLinkBaseMethods.GetImports(oState)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DataLinkBaseState.Result = DataLinkResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-141")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Recuperació de les guies d'exportació
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetExports(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.DataLinkBaseMethods.GetExports(oState)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DataLinkBaseState.Result = DataLinkResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-142")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Recuperació de les guies d'exportació
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTemplatesByProfileMask(ByVal oPage As System.Web.UI.Page, ProfileMask As String) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataTable) = VTLiveApi.DataLinkBaseMethods.GetTemplatesByProfileMask(ProfileMask, oState)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DataLinkBaseState.Result = DataLinkResultEnum.NoError Then
                    If wsRet.Value.Rows.Count > 0 Then
                        oRet = wsRet.Value
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                'HelperWeb.ShowError(oPage, oTmpState, "9-BW01-142")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene guia de importacion
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="_ID">identificador de la guia de importacion</param>
        ''' <returns>Devuelve TRUE si se puede grabar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function GetImportGuide(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal _Audit As Boolean) As roImportGuide
            Dim oRet As roImportGuide = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roImportGuide) = VTLiveApi.DataLinkBaseMethods.GetImportGuide(_ID, _Audit, oState)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-143")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guardar guia de importacion
        ''' </summary>
        ''' <param name="oPage">Página web donde mostrar si hay errores</param>
        ''' <param name="oImportGuide">Guia de importacion a guardar</param>
        ''' <returns>Devuelve TRUE si se puede grabar correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function SaveImportGuide(ByVal oPage As System.Web.UI.Page, ByRef oImportGuide As roImportGuide, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DataLinkBaseMethods.SaveImportGuide(oImportGuide, oState, bAudit)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-159")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Retorna fichero rar con las plantillas de exportacion para que las guarde el usuario en su pc
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetExportTemplates(ByVal oPage As System.Web.UI.Page) As Byte()
            Dim byteRet As Byte() = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try

                ' oSession.AccessApi.WebServices.AccessGroupService.Timeout = System.Threading.Timeout.Infinite

                Dim wsRet As roGenericVtResponse(Of Byte()) = VTLiveApi.DataLinkBaseMethods.GetExportTemplates(oState)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                byteRet = wsRet.Value

                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-164")

            End Try

            Return byteRet

        End Function

        Public Shared Function GetExportGuide(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal _Audit As Boolean) As roExportGuide
            Dim oRet As roExportGuide = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roExportGuide) = VTLiveApi.DataLinkBaseMethods.GetExportGuide(_ID, _Audit, oState)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-175")
            End Try

            Return oRet

        End Function

        Public Shared Function GetNextExportGuideId(ByVal oPage As System.Web.UI.Page, ByVal iMinRange As Integer, ByVal iMaxRange As Integer) As Integer
            Dim oRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Integer) = VTLiveApi.DataLinkBaseMethods.GetNextExportGuideId(iMinRange, iMaxRange, oState)

                oRet = wsRet.Value

                oSession.States.AccessGroupState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-823")

            End Try

            Return oRet

        End Function

        Public Shared Function SaveExportGuide(ByVal oPage As System.Web.UI.Page, ByRef oExportGuide As roExportGuide, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DataLinkBaseMethods.SaveExportGuide(oExportGuide, oState, bAudit)

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-176")
            End Try

            Return oRet

        End Function

        Public Shared Function ExistsExportPeriod(ByVal oPage As System.Web.UI.Page, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.DataLinkBaseState

            WebServiceHelper.SetState(oState)

            Try

                ' oSession.AccessApi.WebServices.AccessGroupService.Timeout = System.Threading.Timeout.Infinite

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.DataLinkBaseMethods.ExistsExportPeriod(xBeginPeriod, xEndPeriod, oState)

                bolRet = wsRet.Value

                oSession.States.DataLinkBaseState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.DataLinkBaseState.Result <> DataLinkResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.DataLinkBaseState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-824")

            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.DataLinkBaseState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace