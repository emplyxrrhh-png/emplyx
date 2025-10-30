Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class MoveServiceMethods

        Public Shared Function GetProductionMoves(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal _EndDate As Date) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.MoveState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.MoveMethods.GetProductionMovesInPeriod(_Date, _EndDate, _IDEmployee, oState)

                oSession.States.MoveState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.MoveState.Result = MoveResultEnum.NoError AndAlso wsRet.Value IsNot Nothing Then
                    If wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.MoveState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-404")
            End Try

            Return oRet

        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CaptureState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace