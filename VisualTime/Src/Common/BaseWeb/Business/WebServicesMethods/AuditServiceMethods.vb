Imports Robotics.Base.DTOs
Imports Robotics.VTBase.Audit

Namespace API

    Public NotInheritable Class AuditServiceMethods

        Public Shared Function GetAudit(ByVal oPage As PageBase, ByVal xBeginDate As DateTime, ByVal xEndDate As DateTime,
                                        ByVal intPageNumber As Integer, ByVal intPageRows As Integer, ByVal strOrder As String, ByVal bolOrderAsc As Boolean, ByRef intPagesCount As Integer,
                                        Optional ByVal strUserName As String = "", Optional ByVal ActionID As Action = Action.aNone,
                                        Optional ByVal ObjectType As ObjectType = ObjectType.tNone,
                                        Optional ByVal strClientLocation As String = "") As DataTable

            Dim oRet As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AuditState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AuditMethods.GetAudit(xBeginDate, xEndDate, strUserName, ActionID, ObjectType, strClientLocation,
                                                                       intPageNumber, intPageRows, strOrder, bolOrderAsc, intPagesCount, oState)

                oSession.States.AuditState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result = AuditResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AuditState)
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

        Public Shared Function GetAuditActions(ByVal oPage As PageBase) As DataTable
            Dim oRet As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AuditState

            WebServiceHelper.SetState(oState)

            Try
                oRet = New DataTable("AuditActions")
                oRet.Columns.Add(New DataColumn("ActionID", GetType(Integer)))
                oRet.Columns.Add(New DataColumn("ActionDesc", GetType(String)))

                Dim oRow As DataRow
                For Each oAction As Action In System.Enum.GetValues(GetType(Action))
                    oRow = oRet.NewRow
                    oRow("ActionID") = oAction
                    oRow("ActionDesc") = oPage.Language.Translate("Action." & System.Enum.GetName(GetType(Action), oAction), "Audit")
                    oRet.Rows.Add(oRow)
                Next

                oSession.States.AuditState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result = AuditResultEnum.NoError Then
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AuditState)
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

        Public Shared Function GetAuditObjectTypes(ByVal oPage As PageBase) As DataTable
            Dim oRet As DataTable = Nothing
            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AuditState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.AuditMethods.GetAuditObjectTypes(oState)

                oSession.States.AuditState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.AuditState.Result = AuditResultEnum.NoError Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-064")
            End Try

            Return oRet

        End Function

        Public Shared Function Audit(ByVal _Action As Action, ByVal _ObjectType As ObjectType, ByVal _ObjectName As String, ByVal _ParametersName As List(Of String), ByVal _ParametersValue As List(Of String), ByVal oPage As PageBase) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.AuditState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of Boolean) = VTLiveApi.AuditMethods.Audit(_Action, _ObjectType, _ObjectName, _ParametersName, _ParametersValue, oState)

                oSession.States.AuditState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                bolRet = wsRet.Value

                If oSession.States.AuditState.Result <> AuditResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.AuditState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-065")
            End Try

            Return bolRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AuditState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastErrorCode() As AuditResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.AuditState.Result
            End If
            Return strRet
        End Function

    End Class

End Namespace