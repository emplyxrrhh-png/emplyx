Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Shift

Namespace API

    Public NotInheritable Class ShiftServiceMethods

        Public Shared Function ShiftsAreAllowed(ByVal oPage As System.Web.UI.Page, ByVal intIDShift1 As Integer, ByVal intIDShift2 As Integer,
                                                ByVal intIDShift3 As Integer, ByVal intIDShift4 As Integer) As Boolean

            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ShiftMethods.ShiftsAreAllowed(oState, intIDShift1, intIDShift2, intIDShift3, intIDShift4)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-628")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShift(ByVal oPage As System.Web.UI.Page, ByVal intIDShift As Integer, ByVal bAudit As Boolean) As roShift
            Dim oRet As roShift = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roShift) = VTLiveApi.ShiftMethods.GetShift(intIDShift, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-629")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftGroup(ByVal oPage As System.Web.UI.Page, ByVal intIDShiftGroup As Integer, ByVal bAudit As Boolean) As roShiftGroup
            Dim oRet As roShiftGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roShiftGroup) = VTLiveApi.ShiftMethods.GetShiftGroup(intIDShiftGroup, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-630")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShifts(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDShiftsGroup As Integer = -1, Optional ByVal ListObsoletes As Boolean = False, Optional ByVal _IDAssignment As Integer = -1, Optional ByVal addIsRigidInfo As Boolean = False, Optional ByVal addNotifiyAtInfo As Boolean = False) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = Nothing
                If _IDAssignment <= 0 Then
                    wsRet = VTLiveApi.ShiftMethods.GetShifts(_IDShiftsGroup, oState, ListObsoletes, addIsRigidInfo, addNotifiyAtInfo)
                Else
                    wsRet = VTLiveApi.ShiftMethods.GetShiftsAssignment(_IDShiftsGroup, _IDAssignment, oState, ListObsoletes)
                End If

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-631")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftsPlanification(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDShiftsGroup As Integer = -1, Optional ByVal ListObsoletes As Boolean = False, Optional ByVal _IDAssignment As Integer = -1) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = Nothing
                If _IDAssignment <= 0 Then
                    wsRet = VTLiveApi.ShiftMethods.GetShiftsPlanification(_IDShiftsGroup, oState, ListObsoletes)
                Else
                    wsRet = VTLiveApi.ShiftMethods.GetShiftsAssignmentPlanification(_IDShiftsGroup, _IDAssignment, oState, ListObsoletes)
                End If

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-632")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftsPortal(ByVal oPage As System.Web.UI.Page, Optional ByVal _IDShiftsGroup As Integer = -1, Optional ByVal ListObsoletes As Boolean = False) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftsPortal(_IDShiftsGroup, oState, ListObsoletes)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-633")
            End Try

            Return oRet
        End Function

        Public Shared Function GetSchemas(ByVal oPage As System.Web.UI.Page, Optional ByVal ListObsoletes As Boolean = False) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetSchemas(ListObsoletes, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-634")
            End Try

            Return oRet

        End Function

        Public Shared Function GetSchemasPlanification(ByVal oPage As System.Web.UI.Page, Optional ByVal ListObsoletes As Boolean = False) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetSchemasPlanification(ListObsoletes, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-635")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftGroups(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftGroups(oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-636")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftGroupsPlanification(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftGroupsPlanification(oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-637")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftsFromGroup(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftsFromGroup(IDGroup, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-638")
            End Try

            Return oRet

        End Function

        Public Shared Function ShiftIsUsed(ByVal oPage As System.Web.UI.Page, ByVal intIDShift As Integer) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ShiftMethods.ShiftIsUsed(intIDShift, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-639")
            End Try

            Return oRet

        End Function

        Public Shared Function DeleteShift(ByVal oPage As System.Web.UI.Page, ByVal intIDShift As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ShiftMethods.DeleteShift(intIDShift, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-640")
            End Try

            Return oRet

        End Function

        Public Shared Function DeleteShiftGroup(ByVal oPage As System.Web.UI.Page, ByVal intIDShiftGroup As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ShiftMethods.DeleteShiftGroup(intIDShiftGroup, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-641")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveTimeZone(ByVal oPage As System.Web.UI.Page, ByRef oTimeZone As roTimeZone, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roTimeZone) = VTLiveApi.ShiftMethods.SaveTimeZone(oTimeZone, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oTimeZone = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-642")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteTimeZone(ByVal oPage As System.Web.UI.Page, ByRef oID As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ShiftMethods.DeleteTimeZoneByID(oID, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                oRet = wsRet.Value

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-643")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveShift(ByVal oPage As System.Web.UI.Page, ByRef oShift As roShift, ByVal bolCheckVacationsEmpty As Boolean, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roShift) = VTLiveApi.ShiftMethods.SaveShift(oShift, bolCheckVacationsEmpty, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oShift = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-644")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia un horario de otro
        ''' </summary>
        ''' <param name="oPage">Página desde donde se ejecuta el proceso (retorno de errores,etc.)</param>
        ''' <param name="IDShiftSource">ID del horario origen</param>
        ''' <param name="NewName">Nombre del nuevo horario (opcional)</param>
        ''' <returns>Devuelve el nuevo horario copiado</returns>
        ''' <remarks></remarks>
        Public Shared Function CopyShift(ByVal oPage As System.Web.UI.Page, ByVal IDShiftSource As Integer, Optional ByVal NewName As String = "", Optional ByVal bAudit As Boolean = True) As roShift
            Dim oRet As roShift = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roShift) = VTLiveApi.ShiftMethods.CopyShift(IDShiftSource, NewName, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-645")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveShiftGroup(ByVal oPage As System.Web.UI.Page, ByRef oShiftGroup As roShiftGroup, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roShiftGroup) = VTLiveApi.ShiftMethods.SaveShiftGroup(oShiftGroup, oState, bAudit)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    oShiftGroup = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-646")
            End Try

            Return bolRet

        End Function

        Public Shared Function CreateEmptyLayer(ByVal oPage As System.Web.UI.Page) As roShiftLayer
            Dim oRet As roShiftLayer = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roShiftLayer) = VTLiveApi.ShiftMethods.CreateEmptyLayer()

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-647")
            End Try

            Return oRet

        End Function

        Public Shared Function GetChildLayerTypes(ByVal oPage As System.Web.UI.Page) As String
            Dim oRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.ShiftMethods.GetChildLayerTypes()

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-648")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftOldestDate(ByVal oPage As System.Web.UI.Page, ByVal intIDShift As Integer) As Date

            Dim oRet As Date = New Date(1900, 1, 1)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of Date) = VTLiveApi.ShiftMethods.GetShiftOldestDate(intIDShift, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-649")
            End Try

            Return oRet

        End Function

        Public Shared Function GetTimeZones(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetTimeZones(oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-650")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftRuleDescription(ByVal oPage As System.Web.UI.Page, ByVal oShiftRule As roShiftRule) As String
            Dim oRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.ShiftMethods.GetShiftRulesDescriptions(oShiftRule, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-651")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Devuelve el nombre del horario incluyendo la hora de inici del flotante especificada.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="intIDShift"></param>
        ''' <param name="xStartShift"></param>
        ''' <returns></returns>
        ''' <remarks>Si el horario no es de flotante, devuelve el nombre del horario.</remarks>
        Public Shared Function FloatingShiftName(ByVal oPage As System.Web.UI.Page, ByVal intIDShift As Integer, ByVal xStartShift As DateTime) As String
            Dim oRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of String) = VTLiveApi.ShiftMethods.FloatingShiftName(intIDShift, xStartShift, oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-655")
            End Try

            Return oRet
        End Function

        Public Shared Function GetBusinessGroupFromShiftGroups(ByVal oPage As System.Web.UI.Page) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetBusinessGroupFromShiftGroups(oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-656")
            End Try

            Return oRet

        End Function

        Public Shared Function BusinessGroupListInUse(ByVal oPage As System.Web.UI.Page, ByVal strBusinessGroup As String, ByVal IdGroup As Integer) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.ShiftMethods.BusinessGroupListInUse(oState, strBusinessGroup, IdGroup)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                bolRet = wsRet.Value

                If oSession.States.ShiftState.Result <> ShiftResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-657")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetShiftsTotalsByEmployee(ByVal oPage As System.Web.UI.Page, ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftsTotalsByEmployee(oState, Ejercicio, IDGroup, IDEmployee)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-658")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftsTotalsByEmployeeAndContract(ByVal oPage As System.Web.UI.Page, ByVal Ejercicio As Integer, ByVal IDGroup As Integer, ByVal IDEmployee As Integer, ByVal IdContract As String) As Data.DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftsTotalsByEmployeeAndContract(oState, Ejercicio, IDGroup, IDEmployee, IdContract)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-659")
            End Try

            Return oRet

        End Function

        Public Shared Function GetShiftsByEmployeeVisibilityPermissions(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, Optional ByVal intIDGroup As Integer = -1,
                                                                        Optional ByVal IncludeObsoletes As Boolean = False, Optional ByVal IDAssignment As Integer = -1,
                                                                        Optional ByVal isPortal As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetShiftsByEmployeeVisibilityPermissions(intIDEmployee, oState, intIDGroup, IncludeObsoletes, IDAssignment, isPortal)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-660")
            End Try

            Return oRet

        End Function

        Public Shared Function GetHolidayShifts(ByVal oPage As System.Web.UI.Page, Optional ByVal intIDGroup As Integer = -1, Optional ByVal IncludeObsoletes As Boolean = False, Optional ByVal OrderByGroup As Boolean = True, Optional bOnlyControlledByContractAnnualizedConcept As Boolean = False) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetHolidaysShifts(oState, intIDGroup, IncludeObsoletes, OrderByGroup, bOnlyControlledByContractAnnualizedConcept)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-661")
            End Try

            Return oRet

        End Function


        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ShiftState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function GetExistingShortNamesAndExport(ByVal oPage As System.Web.UI.Page) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ShiftState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.ShiftMethods.GetExistingShortNamesAndExport(oState)

                oSession.States.ShiftState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ShiftState.Result = ShiftResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.ShiftState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-661")
            End Try

            Return oRet

        End Function

    End Class

End Namespace