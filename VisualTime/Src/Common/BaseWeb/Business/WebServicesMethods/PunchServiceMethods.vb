Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTBusiness.Task
Imports Robotics.VTBase

Namespace API

    Public NotInheritable Class PunchServiceMethods

        Public Shared Function GetPunchesCostCenterPeriod(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _Date As Date = Nothing, Optional ByVal _EndDate As Date = Nothing) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                If _Date = Nothing Then _Date = System.DateTime.Now

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetPunchesPresFromEmployeeInPeriod(_IDEmployee, _Date, _EndDate, False, oState, "ActualType IN(13)")

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-462")
            End Try

            Return tb

        End Function

        Public Shared Function GetPunchesPresPeriod(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _Date As Date = Nothing, Optional ByVal _EndDate As Date = Nothing) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                If _Date = Nothing Then _Date = System.DateTime.Now

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetPunchesPresFromEmployeeInPeriod(_IDEmployee, _Date, _EndDate, False, oState, "ActualType IN(1,2)")

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-463")
            End Try

            Return tb

        End Function

        Public Shared Function GetPunchesPres(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _Date As Date = Nothing) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                If _Date = Nothing Then _Date = System.DateTime.Now

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetPunchesPresFromEmployee(_IDEmployee, _Date, False, oState, "ActualType IN(1,2)")

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-464")
            End Try

            Return tb

        End Function

        Public Shared Function GetPunchesTasks(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDEmployee As Integer = -1, Optional ByVal _Date As Date = Nothing, Optional ByVal _EndDate As Date = Nothing) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                If _Date = Nothing Then _Date = System.DateTime.Now

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetPunchesPresFromEmployeeInPeriod(_IDEmployee, _Date, _EndDate, False, oState, "ActualType IN(4)")

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-465")
            End Try

            Return tb

        End Function

        Public Shared Function GetCountPunches(ByVal oPage As System.Web.UI.Page,
                                                    Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal iIDTerminal As String = "",
                                                     Optional ByVal iIDReader As String = "",
                                                     Optional ByVal iIDCause As String = "",
                                                     Optional ByVal iIDZone As String = "",
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal strTypes As String = "",
                                                     Optional ByVal strActualTypes As String = "",
                                                     Optional ByVal strInvalidTypes As String = "",
                                                     Optional ByVal strActions As String = "",
                                                     Optional ByVal strOrderBy As String = "") As Integer

            Dim intRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.PunchMethods.GetPunchesCountDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, strTypes, strActualTypes, strInvalidTypes, strActions, strOrderBy, oState)
                intRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-466")
            End Try

            Return intRet

        End Function

        ''' <summary>
        ''' Devuelve los reistros de la tabla de fichajes segun criterio
        ''' </summary>
        ''' <param name="oPage">Pagina a retornar errors, ...</param>
        ''' <param name="dShiftDateBegin">Para filtrar según la fecha asignada inicial.</param>
        ''' <param name="dShiftDateEnd">Para filtrar según la fecha asignada final.</param>
        ''' <param name="dPeriodBegin">Para filtrar según la fecha/hora inicial.</param>
        ''' <param name="dPeriodEnd">Para filtrar según la fecha/hora  final.</param>
        ''' <param name="iIDEmployee">Para filtrar según el código de empleado</param>
        ''' <param name="iIDTerminal">Para filtrar según el código de terminal.</param>
        ''' <param name="iIDReader">Para filtrar según el código del lector.</param>
        ''' <param name="iIDCause">Para filtrar según el código de justificación.</param>
        ''' <param name="iIDZone">Para filtrar según el código de zona.</param>
        ''' <param name="iIsNotReliable">Para filtrar según si es un movimiento fiable o no.</param>
        ''' <param name="strTypes">Para filtrar por tipos de fichaje real</param>
        ''' <param name="strActualTypes">Para filtrar por tipos de fichaje actual.</param>
        ''' <param name="strInvalidTypes">Para filtrar por tipos de fichaje invalido de accesos.</param>
        ''' <param name="strActions">Para filtrar por tipos de acciones.</param>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Shared Function GetPunchesDataTable(ByVal oPage As System.Web.UI.Page,
                                                    Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal iIDTerminal As String = "",
                                                     Optional ByVal iIDReader As String = "",
                                                     Optional ByVal iIDCause As String = "",
                                                     Optional ByVal iIDZone As String = "",
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal strTypes As String = "",
                                                     Optional ByVal strActualTypes As String = "",
                                                     Optional ByVal strInvalidTypes As String = "",
                                                     Optional ByVal strActions As String = "",
                                                     Optional ByVal strOrderBy As String = "") As DataTable
            Dim dRet As DataTable = Nothing
            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetPunchesDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, strTypes, strActualTypes, strInvalidTypes, strActions, strOrderBy, oState)

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-467")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve el último registr ode la tabla de fichajes según criterio
        ''' </summary>
        ''' <param name="oPage">Pagina a devolver errores, ...</param>
        ''' <param name="dShiftDateBegin">Para filtrar según la fecha asignada inicial.</param>
        ''' <param name="dShiftDateEnd">Para filtrar según la fecha asignada final.</param>
        ''' <param name="dPeriodBegin">Para filtrar según la fecha/hora inicial.</param>
        ''' <param name="dPeriodEnd">Para filtrar según la fecha/hora  final.</param>
        ''' <param name="iIDEmployee">Para filtrar según el código de empleado</param>
        ''' <param name="iIDTerminal">Para filtrar según el código de terminal.</param>
        ''' <param name="iIDReader">Para filtrar según el código del lector.</param>
        ''' <param name="iIDCause">Para filtrar según el código de justificación.</param>
        ''' <param name="iIDZone">Para filtrar según el código de zona.</param>
        ''' <param name="iIsNotReliable">Para filtrar según si es un movimiento fiable o no.</param>
        ''' <param name="strTypes">Para filtrar por tipos de fichaje real</param>
        ''' <param name="strActualTypes">Para filtrar por tipos de fichaje actual.</param>
        ''' <param name="strInvalidTypes">Para filtrar por tipos de fichaje invalido de accesos.</param>
        ''' <param name="strActions">Para filtrar por tipos de acciones.</param>
        ''' <returns>DataTable</returns>
        ''' <remarks></remarks>
        Public Shared Function GetLastPunchDataTable(ByVal oPage As System.Web.UI.Page,
                                                    Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal iIDTerminal As String = "",
                                                     Optional ByVal iIDReader As String = "",
                                                     Optional ByVal iIDCause As String = "",
                                                     Optional ByVal iIDZone As String = "",
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal strTypes As String = "",
                                                     Optional ByVal strActualTypes As String = "",
                                                     Optional ByVal strInvalidTypes As String = "",
                                                     Optional ByVal strActions As String = "") As DataTable
            Dim dRet As DataTable = Nothing
            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetLastPunchDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, strTypes, strActualTypes, strInvalidTypes, strActions, oState)

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-468")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve el num. de registros de la tabla de fichajes segun criterio
        ''' </summary>
        ''' <param name="oPage">Pagina a devolver errores, ...</param>
        ''' <param name="dShiftDateBegin">Para filtrar según la fecha asignada inicial.</param>
        ''' <param name="dShiftDateEnd">Para filtrar según la fecha asignada final.</param>
        ''' <param name="dPeriodBegin">Para filtrar según la fecha/hora inicial.</param>
        ''' <param name="dPeriodEnd">Para filtrar según la fecha/hora  final.</param>
        ''' <param name="iIDEmployee">Para filtrar según el código de empleado</param>
        ''' <param name="iIDTerminal">Para filtrar según el código de terminal.</param>
        ''' <param name="iIDReader">Para filtrar según el código del lector.</param>
        ''' <param name="iIDCause">Para filtrar según el código de justificación.</param>
        ''' <param name="iIDZone">Para filtrar según el código de zona.</param>
        ''' <param name="iIsNotReliable">Para filtrar según si es un movimiento fiable o no.</param>
        ''' <param name="strTypes">Para filtrar por tipos de fichaje real</param>
        ''' <param name="strActualTypes">Para filtrar por tipos de fichaje actual.</param>
        ''' <param name="strInvalidTypes">Para filtrar por tipos de fichaje invalido de accesos.</param>
        ''' <param name="strActions">Para filtrar por tipos de acciones.</param>
        ''' <returns>Num. de registros</returns>
        ''' <remarks></remarks>
        '''
        Public Shared Function GetPunchesCountDataTable(ByVal oPage As System.Web.UI.Page,
                                                    Optional ByVal dShiftDateBegin As Date = Nothing,
                                                     Optional ByVal dShiftDateEnd As Date = Nothing,
                                                     Optional ByVal dPeriodBegin As DateTime = Nothing,
                                                     Optional ByVal dPeriodEnd As DateTime = Nothing,
                                                     Optional ByVal iIDEmployee As String = "",
                                                     Optional ByVal iIDTerminal As String = "",
                                                     Optional ByVal iIDReader As String = "",
                                                     Optional ByVal iIDCause As String = "",
                                                     Optional ByVal iIDZone As String = "",
                                                     Optional ByVal iIsNotReliable As Integer = -1,
                                                     Optional ByVal strTypes As String = "",
                                                     Optional ByVal strActualTypes As String = "",
                                                     Optional ByVal strInvalidTypes As String = "",
                                                     Optional ByVal strActions As String = "",
                                                     Optional ByVal strOrderBy As String = "") As Integer
            Dim intRet As Integer = 0
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.PunchMethods.GetPunchesCountDataTable(dShiftDateBegin, dShiftDateEnd, dPeriodBegin, dPeriodEnd, iIDEmployee, iIDTerminal, iIDReader, iIDCause, iIDZone, iIsNotReliable, strTypes, strActualTypes, strInvalidTypes, strActions, strOrderBy, oState)
                intRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-469")
            End Try

            Return intRet

        End Function

        Public Shared Function GetPunch(ByVal oPage As System.Web.UI.Page, ByVal intIDPunch As Long, ByVal bLoadPhoto As Boolean) As roPunch

            Dim oRet As roPunch = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roPunch) = VTLiveApi.PunchMethods.GetPunch(intIDPunch, bLoadPhoto, oState)
                oRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-470")
            End Try

            Return oRet

        End Function

        Public Shared Function SavePunch(ByVal oPage As System.Web.UI.Page, ByVal oMove As roPunch, ByVal bolAutomaticBeginJobCheck As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.SavePunch(oMove, bolAutomaticBeginJobCheck, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-471")
            End Try

            Return bolRet

        End Function

        Public Shared Function SavePunches(ByVal oPage As System.Web.UI.Page, ByVal tbPunches As DataTable, ByVal bolAutomaticBeginJobCheck As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                ' Introducir datatable 'tbPunches' a una dataset 'dsPunches' para poderlo pasar al ws
                Dim dsPunches As DataSet
                If tbPunches.DataSet IsNot Nothing Then
                    dsPunches = tbPunches.DataSet
                Else
                    dsPunches = New DataSet
                    dsPunches.Tables.Add(tbPunches)
                End If

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.SavePunches(dsPunches, bolAutomaticBeginJobCheck, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-472")
            End Try

            Return bolRet

        End Function

        Public Shared Function SavePunches(ByVal oPage As System.Web.UI.Page, ByVal tbPunches As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                ' Introducir datatable 'tbPunches' a una dataset 'dsPunches' para poderlo pasar al ws
                Dim dsPunches As DataSet
                If tbPunches.DataSet IsNot Nothing Then
                    dsPunches = tbPunches.DataSet
                Else
                    dsPunches = New DataSet
                    dsPunches.Tables.Add(tbPunches)
                End If

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.SavePunches(dsPunches, False, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Or Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-473")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveInvalidPunches(ByVal oPage As System.Web.UI.Page, ByVal tbPunches As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim dsPunches As DataSet
                If tbPunches.DataSet IsNot Nothing Then
                    dsPunches = tbPunches.DataSet
                Else
                    dsPunches = New DataSet
                    dsPunches.Tables.Add(tbPunches)
                End If

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.SaveInvalidPunches(dsPunches, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Or Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-474")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeletePunch(ByVal oPage As System.Web.UI.Page, ByVal oPunch As roPunch) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.DeletePunch(oPunch, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-475")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeletePunchByID(ByVal oPage As System.Web.UI.Page, ByVal lngIDPunch As Long) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.DeletePunchByID(lngIDPunch, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-476")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Añade un movimiento de presencia para un empleado. El tipo de movimiento generado se calcula en función del estado actual del empleado.
        ''' </summary>
        ''' <param name="oPage">Pagina a la que se devuelven los errores</param>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
        ''' <param name="_IDReader">Número del lector por el que se realiza el movimiento</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de movimiento generado</param>
        ''' <param name="_SeqStatus">Devuelve el estado de la secuencia resultado de la acción</param>
        ''' <param name="_SaveData">Indica si se tiene que guardar el movimiento generado o no.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoSequencePunch(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch, ByRef _InputType As PunchStatus, ByRef _SeqStatus As PunchSeqStatus, ByVal _SaveData As Boolean, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1, Optional ByVal LocationZone As String = "", Optional ByVal fullAddress As String = "", Optional ByVal timeZone As String = "") As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim oInputCaptureBytes() As Byte = Nothing
                If _InputCapture IsNot Nothing Then
                    oInputCaptureBytes = roTypes.Image2Bytes(_InputCapture)
                End If

                Dim response As roGenericVtResponse(Of roDoSequencePunch) = VTLiveApi.PunchMethods.DoSequencePunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDReader, _IDCause, oInputCaptureBytes, _Lat, _Lon, LocationZone, fullAddress, timeZone, _SaveData, oState)

                bolRet = response.Value.Saved
                _Punch = response.Value.Punch
                _InputType = response.Value.Status
                _SeqStatus = response.Value.SeqStatus

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-477")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Añade un fichaje de tarea para un empleado.
        ''' </summary>
        ''' <param name="oPage">Pagina a la que se devuelven los errores</param>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
        ''' <param name="_IDTask">Código de la tarea a fichar</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de movimiento generado</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoTaskPunch(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDTask As Integer,
                                           ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch, ByRef _InputType As PunchStatus, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1,
                                           Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal timeZone As String = "", Optional ByVal Field1 As String = "", Optional ByVal Field2 As String = "", Optional ByVal Field3 As String = "",
                                           Optional ByVal Field4 As Double = -1, Optional ByVal Field5 As Double = -1, Optional ByVal Field6 As Double = -1) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim oInputCaptureBytes() As Byte = Nothing
                If _InputCapture IsNot Nothing Then
                    oInputCaptureBytes = roTypes.Image2Bytes(_InputCapture)
                End If

                Dim response As roGenericVtResponse(Of roDoSequencePunch) = VTLiveApi.PunchMethods.DoTaskPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDTask, oInputCaptureBytes, _Lat, _Lon, LocationZone, FullAddress, timeZone, Field1, Field2, Field3, Field4, Field5, Field6, oState)

                bolRet = response.Value.Saved
                _Punch = response.Value.Punch
                _InputType = response.Value.Status

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-478")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Añade un fichaje de tarea para un empleado.
        ''' </summary>
        ''' <param name="oPage">Pagina a la que se devuelven los errores</param>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_InputDateTime">Fecha y hora del movimiento a generar</param>
        ''' <param name="_IDTerminal">Código del terminal por el que se realiza el movimiento</param>
        ''' <param name="_IDCostCenter">Código del centro de negocios</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado.</param>
        ''' <param name="_InputType">Devuelve el tipo de movimiento generado</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DoCostCenterPunch(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDCostCenter As Integer,
                                           ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch, ByRef _InputType As PunchStatus, Optional ByVal _Lat As Double = -1, Optional ByVal _Lon As Double = -1,
                                           Optional ByVal LocationZone As String = "", Optional ByVal FullAddress As String = "", Optional ByVal timeZone As String = "") As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim oInputCaptureBytes() As Byte = Nothing
                If _InputCapture IsNot Nothing Then
                    oInputCaptureBytes = roTypes.Image2Bytes(_InputCapture)
                End If

                Dim response As roGenericVtResponse(Of roDoSequencePunch) = VTLiveApi.PunchMethods.DoCostCenterPunch(_IDEmployee, _InputDateTime, _IDTerminal, _IDCostCenter, oInputCaptureBytes, _Lat, _Lon, LocationZone, FullAddress, timeZone, oState)

                bolRet = response.Value.Saved
                _Punch = response.Value.Punch
                _InputType = response.Value.Status

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-479")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Cambia el estado de presencia actual del empleado generando un fichaje a la hora indicada.
        ''' </summary>
        ''' <param name="oPage">Pagina a la que se devuelven los errores</param>
        ''' <param name="_IDEmployee">Código del empleado</param>
        ''' <param name="_NowDateTime">Fecha y hora actual</param>
        ''' <param name="_InputDateTime">Hora en la que se generará el fichaje olvidado para el cambio de estado</param>
        ''' <param name="_IDTerminal">Código de terminal por el que se realiza la operación</param>
        ''' <param name="_IDReader">Número de lector por el que se realiza la operación</param>
        ''' <param name="_IDCause">Código de la justificación. Si no hay, se tiene que informar como '-1'.</param>
        ''' <param name="_InputCapture">Imagen correspondiente a la captura del fichaje. Si no hay se tiene que informar 'Nothing'</param>
        ''' <param name="_Punch">Devuelve el fichaje generado</param>
        ''' <param name="_SaveData">Indica si se tiene que guardar el movimiento generado o no.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function ChangeState(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _NowDateTime As DateTime, ByVal _InputDateTime As DateTime, ByVal _IDTerminal As Integer, ByVal _IDReader As Integer, ByVal _IDCause As Integer, ByVal _InputCapture As System.Drawing.Image, ByRef _Punch As roPunch, ByVal _SaveData As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim oInputCaptureBytes() As Byte = Nothing
                If _InputCapture IsNot Nothing Then
                    oInputCaptureBytes = roTypes.Image2Bytes(_InputCapture)
                End If

                Dim response As roGenericVtResponse(Of roPunch) = VTLiveApi.PunchMethods.ChangeState(_IDEmployee, _NowDateTime, _InputDateTime, _IDTerminal, _IDReader, _IDCause, oInputCaptureBytes, _Punch, _SaveData, oState)

                If response.Value Is Nothing Then
                    bolRet = False
                    _Punch = response.Value
                Else
                    bolRet = True
                End If

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-480")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Reorganiza los fichajes por Empleado / Dia (actualizacion)
        ''' </summary>
        ''' <param name="oPage">PageBase</param>
        ''' <param name="_IDEmployee">ID de Empleado</param>
        ''' <param name="_Date">Fecha a reorganizar</param>
        ''' <returns>DataTable con el Resultado a reorganizar</returns>
        ''' <remarks></remarks>
        Public Shared Function ReorderPunches(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As Boolean
            Dim lRet As Boolean = True

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.ReorderPunches(_IDEmployee, _Date, False, oState)

                If response.Value = True Then
                    lRet = True
                Else
                    lRet = False
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-481")
                lRet = False
            End Try
            Return lRet
        End Function

        Public Shared Function GetAccessPunchesStatus(ByVal oPage As System.Web.UI.Page, ByVal bColImage As Boolean, Optional ByVal Feature As String = "", Optional ByVal Type As String = "U",
                                    Optional ByVal IDAccessGroups As Generic.List(Of Integer) = Nothing,
                                    Optional ByVal IDZones As Generic.List(Of Integer) = Nothing, Optional ByVal onlyTotals As Boolean = False
                                    ) As DataSet
            Dim dsRet As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of DataSet) = Nothing

                If IDAccessGroups IsNot Nothing AndAlso IDZones IsNot Nothing Then
                    response = VTLiveApi.PunchMethods.GetAccessPunchesStatus(Feature, Type, IDAccessGroups, IDZones, oState, bColImage, onlyTotals)
                ElseIf IDAccessGroups IsNot Nothing AndAlso IDZones Is Nothing Then
                    response = VTLiveApi.PunchMethods.GetAccessPunchesStatus(Feature, Type, IDAccessGroups, Nothing, oState, bColImage, onlyTotals)
                ElseIf IDAccessGroups Is Nothing AndAlso IDZones IsNot Nothing Then
                    response = VTLiveApi.PunchMethods.GetAccessPunchesStatus(Feature, Type, Nothing, IDZones, oState, bColImage, onlyTotals)
                Else
                    response = VTLiveApi.PunchMethods.GetAccessPunchesStatus(Feature, Type, Nothing, Nothing, oState, bColImage, onlyTotals)
                End If

                dsRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError Then
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-483")
            End Try

            Return dsRet
        End Function

        ''' <summary>
        ''' Devuelve los fichajes no asignados a ningún empleado
        ''' </summary>
        ''' <param name="oPage">PageBase</param>
        ''' <returns>DataTable con los registros</returns>
        ''' <remarks></remarks>
        Public Shared Function GetInvalidPunches(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetInvalidPunches(oState)

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-484")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los fichajes no asignados a ningún empleado
        ''' </summary>
        ''' <param name="oPage">PageBase</param>
        ''' <returns>DataTable con los registros</returns>
        ''' <remarks></remarks>
        Public Shared Function GetInvalidEntries(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetInvalidEntries(oState)

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-485")
            End Try
            Return tb

        End Function

        ''' <summary>
        ''' Reprocesa los fichajes que no se pudieron importar
        ''' </summary>
        ''' <param name="oPage">PageBase</param>
        ''' <returns>DataTable con los registros</returns>
        ''' <remarks></remarks>
        Public Shared Function ReprocessInvalidEntries(ByVal oPage As System.Web.UI.Page, ByVal oInvalidEntries As DTOs.roPunchInvalidEntry()) As Boolean
            Dim bolRet As Boolean = False
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.PunchMethods.ReprocessInvalidEntries(oInvalidEntries, oState)
                bolRet = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Or Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-485")
            End Try
            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve los fichajes no asignados a ningún empleado y con una tarjeta concreta
        ''' </summary>
        ''' <param name="oPage">PageBase</param>
        ''' <returns>DataTable con los registros</returns>
        ''' <remarks></remarks>
        Public Shared Function GetInvalidPunchesByIDCard(ByVal oPage As System.Web.UI.Page, ByVal lngIDCard As Long) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetInvalidPunchesByIDCard(oState, lngIDCard)

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-486")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Retorna dataset con los ultimos fichajes realizados de accesos validos e invalidos
        ''' </summary>
        ''' <param name="ListIdZones">Lista de zonas sobre las que obtener datos</param>
        ''' <returns>Dataset</returns>
        Public Shared Function GetAccessPunchesMonitor(ByVal oPage As System.Web.UI.Page, ByVal ListIdZones As Generic.List(Of Integer),
                                                       ByVal ListIdFields As Generic.List(Of String), ByVal bColImage As Boolean) As DataSet
            Dim ds As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.PunchMethods.GetAccessPunchesMonitor(oState, ListIdZones, ListIdFields, bColImage)

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result = PunchResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    ds = response.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-487")
            End Try

            Return ds

        End Function

        Public Shared Function GetAllowedTasksByEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, Optional ByVal checkActualTask As Boolean = False, Optional ByVal actualTaskId As Integer = -1) As Object 'roTask()
            Dim result As roTask()

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roTask()) = VTLiveApi.PunchMethods.GetAllowedTasksByEmployee(_IDEmployee, oState, checkActualTask, actualTaskId)
                result = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    result = (New Generic.List(Of roTask)).ToArray()
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                result = {}
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-490")
            End Try

            Return result
        End Function

        Public Shared Function GetAllowedTasksByEmployeeFiltered(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, Optional ByVal checkActualTask As Boolean = False, Optional ByVal actualTaskId As Integer = -1, Optional ByVal taskFilterName As String = "") As Object ' roTask()
            Dim result As roTask()

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roTask()) = VTLiveApi.PunchMethods.GetAllowedTasksByEmployeeFiltered(_IDEmployee, oState, checkActualTask, actualTaskId, taskFilterName)
                result = response.Value

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    result = (New Generic.List(Of roTask)).ToArray()
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                result = {}
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-491")
            End Try
            Return result
        End Function

        Public Shared Function GetLastPunchTaskInfoByEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByRef oPunchTypeEnum As PunchTypeEnum, ByRef oDate As Date) As Integer
            Dim result As Integer
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.PunchState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roLastTaskInfoByEmployee) = VTLiveApi.PunchMethods.GetLastTaskInfoByEmployee(_IDEmployee, oState)
                result = response.Value.LastPunchID
                oPunchTypeEnum = response.Value.LastPunchType
                oDate = response.Value.LastPunchDateTime

                oSession.States.PunchState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.PunchState.Result <> PunchResultEnum.NoError Then
                    result = -1
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.PunchState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-492")
            End Try

            Return result

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.PunchState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace