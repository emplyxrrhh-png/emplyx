Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment
Imports Robotics.Base.VTBusiness.Scheduler

Namespace API

    Public NotInheritable Class SchedulerServiceMethods

        ''' <summary>
        ''' Obtiene la configuración de realtes del calendario para el pasaporte indicado.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDPassport"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSchedulerRemarksConfig(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer) As roSchedulerRemarks

            Dim oRet As roSchedulerRemarks = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                'oRet = VTLiveApi.SchedulerMethods.GetSchedulerRemarksConfig(_IDPassport, oState)

                'oSession.States.SchedulerState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                '    ' Mostrar el error
                '    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                'End If

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim response As roGenericVtResponse(Of roSchedulerRemarks) = VTLiveApi.SchedulerMethods.GetSchedulerRemarksConfig(_IDPassport, oState)

                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-535")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la configuración de realtes del calendario para el pasaporte indicado.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDPassport"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetSchedulerRemarksConfigDataTable(ByVal oPage As System.Web.UI.Page, ByVal _IDPassport As Integer) As DataTable

            Dim oRet As DataTable = Nothing

            oRet = New DataTable
            oRet.Columns.Add(New DataColumn("ID", GetType(Integer)))
            oRet.Columns.Add(New DataColumn("IDCause", GetType(Integer)))
            oRet.Columns.Add(New DataColumn("Compare", GetType(Integer)))
            oRet.Columns.Add(New DataColumn("Value", GetType(String)))
            oRet.Columns.Add(New DataColumn("Color", GetType(Integer)))
            oRet.Columns.Add(New DataColumn("Order", GetType(Integer)))
            oRet.PrimaryKey = New DataColumn() {oRet.Columns("ID")}

            Dim oRemarks As roSchedulerRemarks = GetSchedulerRemarksConfig(oPage, _IDPassport)

            Dim oRow As DataRow
            Dim iId As Integer = 1
            For Each oRemark As VTBusiness.Scheduler.roCalendarRemark In oRemarks.Remarks
                oRow = oRet.NewRow
                oRow("ID") = iId
                oRow("IDCause") = oRemark.IDCause
                oRow("Compare") = oRemark.Compare
                oRow("Value") = oRemark.Value
                oRow("Color") = oRemark.Color
                oRow("Order") = iId
                oRet.Rows.Add(oRow)
                iId = iId + 1
            Next

            oRet.AcceptChanges()

            Return oRet

        End Function

        ''' <summary>
        ''' Graba la configuración de resaltes del calendario en el contexto del pasaporte actual
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="oRemarks">Configuración de realtes a grabar.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveSchedulerRemarksConfig(ByVal oPage As System.Web.UI.Page, ByVal oRemarks As roSchedulerRemarks) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                'bolRet = VTLiveApi.SchedulerMethods.SaveSchedulerRemarksConfig(oRemarks, oState)

                'oSession.States.SchedulerState = oState
                'roWsUserManagement.SessionObject = oSession

                'If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                '    ' Mostrar el error
                '    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                'End If

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SchedulerMethods.SaveSchedulerRemarksConfig(oRemarks, oState)

                oSession.States.SchedulerState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-536")
            End Try

            Return bolRet

        End Function



        ''' <summary>
        ''' Obtiene lista de usuarios (entradas y salidas) en base a una lista de ids
        ''' </summary>
        ''' <param name="oPage">Página a recibir el contenido</param>
        ''' <param name="IDEmployees">Lista de IDs de Empleados a obtener</param>
        ''' <param name="bAudit"></param>
        ''' <returns>Una tabla</returns>
        ''' <remarks></remarks>
        Public Shared Function GetScheduledEmployeesFromList(ByVal oPage As System.Web.UI.Page,
                                            ByVal IDEmployees As String,
                                            Optional ByVal bAudit As Boolean = False) As DataTable
            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                'VTLiveApi.SchedulerMethods.Timeout = System.Threading.Timeout.Infinite

                'Dim ds As DataSet = VTLiveApi.SchedulerMethods.GetScheduledEmployeesFromList(IDEmployees, oState)

                'oSession.States.SchedulerState = oState
                'roWsUserManagement.SessionObject = oSession

                'If ds IsNot Nothing And oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
                '    oRet = ds.Tables(0)
                'Else
                '    ' Mostrar el error
                '    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                'End If
                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.SchedulerMethods.GetScheduledEmployeesFromList(IDEmployees, oState)

                oRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-538")
            End Try

            Return oRetTable

        End Function


        Public Shared Function GetScheduleTemplates(ByVal oPage As System.Web.UI.Page) As Generic.List(Of roScheduleTemplate)

            Dim oRet As Generic.List(Of roScheduleTemplate) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim arr As roGenericVtResponse(Of List(Of roScheduleTemplate)) = VTLiveApi.SchedulerMethods.GetScheduleTemplates(oState)
                If oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
                    oRet = arr.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-543")
            End Try

            Return oRet

        End Function

        Public Shared Function GetScheduleTemplatesv2(ByVal oPage As System.Web.UI.Page) As Generic.List(Of roScheduleTemplate)

            Dim oRet As Generic.List(Of roScheduleTemplate) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim arr As roGenericVtResponse(Of List(Of roScheduleTemplate)) = VTLiveApi.SchedulerMethods.GetScheduleTemplatesv2(oState)
                If oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
                    oRet = arr.Value.ToList()
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-544")
            End Try

            Return oRet

        End Function

        Public Shared Function GetScheduleTemplate(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal bAudit As Boolean) As roScheduleTemplate

            Dim oRet As roScheduleTemplate = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roScheduleTemplate) = VTLiveApi.SchedulerMethods.GetScheduleTemplate(_ID, oState, bAudit)

                oRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-545")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveScheduleTemplate(ByVal oPage As System.Web.UI.Page, ByVal oTemplate As roScheduleTemplate, ByVal bAudit As Boolean, ByVal idPassport As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SchedulerMethods.SaveScheduleTemplate(oTemplate, oState, bAudit, idPassport)

                bolRet = wsRet.Value
                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-546")
            End Try
            Return bolRet

        End Function

        Public Shared Function DeleteScheduleTemplate(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SchedulerMethods.DeleteScheduleTemplate(_ID, oState, bAudit)

                bolRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-547")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Obtiene la definición de la dotación para un grupo y fecha.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDGroup"></param>
        ''' <param name="_Date"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDailyCoverage(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _Audit As Boolean) As roDailyCoverage

            Dim oRet As roDailyCoverage = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)
            Try

                Dim wsRet As roGenericVtResponse(Of roDailyCoverage) = VTLiveApi.SchedulerMethods.GetDailyCoverage(_IDGroup, _Date, _Audit, oState)

                oRet = wsRet.Value
                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-550")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la definición de la dotación para un grupo y fecha.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDGroup"></param>
        ''' <param name="_Date"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDailyCoverageDataTable(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _Audit As Boolean) As DataTable

            'Dim oRet As DataTable = Nothing

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roWsState = oSession.States.SchedulerState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    Dim ds As DataSet = VTLiveApi.SchedulerMethods.GetDailyCoverageDataTable(_IDGroup, _Date, _Audit, oState)

            '    oSession.States.SchedulerState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If ds IsNot Nothing And oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
            '        oRet = ds.Tables(0)
            '    Else
            '        ' Mostrar el error
            '        HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
            '    End If
            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.SchedulerMethods.GetDailyCoverageDataTable(_IDGroup, _Date, _Audit, oState)

                oRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-551")
            End Try

            Return oRetTable

        End Function

        ''' <summary>
        ''' Obtiene una lista con las definiciones de las dotaciones teóricas de un grupo para un periodo de fechas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDGroup"></param>
        ''' <param name="_BeginDate"></param>
        ''' <param name="_EndDate"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDailyCoverages(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _Audit As Boolean) As Generic.List(Of roDailyCoverage)

            Dim oRet As Generic.List(Of roDailyCoverage) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim arrRet As roGenericVtResponse(Of List(Of roDailyCoverage)) = VTLiveApi.SchedulerMethods.GetDailyCoverages(_IDGroup, _BeginDate, _EndDate, _Audit, oState)

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError And arrRet IsNot Nothing Then
                    oRet = arrRet.Value.ToList
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-552")
            End Try

            Return oRet

        End Function

        Public Shared Function CopyTeoricDailyCoverage(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _SourceBeginDate As Date, ByVal _SourceEndDate As Date, ByVal _DestinationBeginDate As Date, ByVal _DestinationEndDate As Date) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SchedulerMethods.CopyTeoricDailyCoverage(_IDGroup, _SourceBeginDate, _SourceEndDate, _DestinationBeginDate, _DestinationEndDate, oState)

                bolRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-553")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Guarda la definición de la dotación para un grupo y fecha.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_DailyCoverage"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveTeoricDailyCoverage(ByVal oPage As System.Web.UI.Page, ByVal _DailyCoverage As roDailyCoverage) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SchedulerMethods.SaveTeoricDailyCoverage(_DailyCoverage, oState)

                bolRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-554")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina toda la dotación de un grupo para una fecha.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDGroup"></param>
        ''' <param name="_Date"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteDailyCoverage(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _Date As Date) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.SchedulerMethods.DeleteDailyCoverage(_IDGroup, _Date, oState)

                bolRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-555")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de puestos que puede cubrir un empleado por un horario. Los ordena según la cobertura del puesto en el horario.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee"></param>
        ''' <param name="_IDShift"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetEmployeAndShiftAssignments(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _IDShift As Integer) As Generic.List(Of roAssignment)

            Dim oRet As Generic.List(Of roAssignment) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim _Assignments As roGenericVtResponse(Of List(Of roAssignment)) = VTLiveApi.SchedulerMethods.GetEmployeAndShiftAssignments(_IDEmployee, _IDShift, oState)

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If _Assignments IsNot Nothing Then
                    oRet = _Assignments.Value
                End If
                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-556")
            End Try

            Return oRet

        End Function

        Public Shared Function GetDailyCoverageAssignmentDetailDataTable(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _Date As Date, ByVal _IDassignment As Integer) As DataTable

            'Dim oRet As DataTable = Nothing

            'Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            'Dim oState As roWsState = oSession.States.SchedulerState

            'WebServiceHelper.SetState(oState, oPage)

            'Try

            '    Dim ds As DataSet = VTLiveApi.SchedulerMethods.GetDailyCoverageAssignmentDetailDataTable(_IDGroup, _Date, _IDassignment, oState)

            '    oSession.States.SchedulerState = oState
            '    roWsUserManagement.SessionObject = oSession

            '    If ds IsNot Nothing And oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
            '        oRet = ds.Tables(0)
            '    Else
            '        ' Mostrar el error
            '        HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
            '    End If
            Dim oRet As DataSet = Nothing
            Dim oRetTable As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                oState.IDPassport = WLHelperWeb.CurrentPassport.ID
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.SchedulerMethods.GetDailyCoverageAssignmentDetailDataTable(_IDGroup, _Date, _IDassignment, oState)

                oRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result = SchedulerResultEnum.NoError Then
                    If oRet IsNot Nothing AndAlso oRet.Tables.Count > 0 Then
                        oRetTable = oRet.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-557")
            End Try

            Return oRetTable

        End Function

        Public Shared Function GetDailyCoverageAssignmentFromEmployeeDate(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date) As roDailyCoverageAssignment

            Dim oRet As roDailyCoverageAssignment = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.SchedulerState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roDailyCoverageAssignment) = VTLiveApi.SchedulerMethods.GetDailyCoverageAssignmentFromEmployeeDate(_IDEmployee, _CoverageDate, oState)

                oRet = wsRet.Value

                oSession.States.SchedulerState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.SchedulerState.Result <> SchedulerResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.SchedulerState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-558")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.SchedulerState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace