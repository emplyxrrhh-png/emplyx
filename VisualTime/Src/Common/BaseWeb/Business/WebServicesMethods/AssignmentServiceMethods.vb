Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Assignment

Namespace API

    Public NotInheritable Class AssignmentServiceMethods

        ''' <summary>
        ''' Obtiene la lista de puestos definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAssignments(ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As Generic.List(Of roAssignment)

            Dim oRet As Generic.List(Of roAssignment) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AssignmentState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of List(Of roAssignment)) = VTLiveApi.AssignmentMethods.GetAssignments(_Order, _Audit, oState)

                oSession.States.AssignmentState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AssignmentState.Result = AssignmentResultEnum.NoError Then
                    oRet = wsRet.Value
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.AssignmentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-057")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene la lista de puestos definidos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Order"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAssignmentsDataTable(ByVal oPage As System.Web.UI.Page, ByVal _Order As String, ByVal _Audit As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AssignmentState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AssignmentMethods.GetAssignmentsDataTable(_Order, _Audit, oState)

                oSession.States.AssignmentState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AssignmentState.Result = AssignmentResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AssignmentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-058")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAssignment(ByVal oPage As System.Web.UI.Page, ByVal _ID As Integer, ByVal _Audit As Boolean) As roAssignment

            Dim oRet As roAssignment = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AssignmentState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roAssignment) = VTLiveApi.AssignmentMethods.GetAssignment(_ID, _Audit, oState)

                oSession.States.AssignmentState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AssignmentState.Result <> AssignmentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.AssignmentState)
                End If

                oRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-059")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Guarda la configuración de un puesto de trabajo
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_Assignment"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function SaveAssignment(ByVal oPage As System.Web.UI.Page, ByRef _Assignment As roAssignment, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AssignmentState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of roAssignment) = VTLiveApi.AssignmentMethods.SaveAssignment(_Assignment, oState, _Audit)

                oSession.States.AssignmentState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AssignmentState.Result <> AssignmentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.AssignmentState)
                End If

                If wsRet.Value IsNot Nothing Then
                    bolRet = True
                    _Assignment = wsRet.Value
                Else
                    bolRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-060")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina un puesto de trabajo
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDAssignment"></param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function DeleteAssignment(ByVal oPage As System.Web.UI.Page, ByVal _IDAssignment As Integer, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AssignmentState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AssignmentMethods.DeleteAssignment(_IDAssignment, oState, _Audit)

                oSession.States.AssignmentState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AssignmentState.Result <> AssignmentResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.AssignmentState)
                End If

                bolRet = wsRet.Value
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-061")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AssignmentState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace