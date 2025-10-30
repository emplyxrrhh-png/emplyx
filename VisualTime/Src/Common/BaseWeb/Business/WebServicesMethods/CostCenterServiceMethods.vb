Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.BusinessCenter

Namespace API

    Public NotInheritable Class CostCenterServiceMethods

        Public Shared Function GetCostCenters(ByVal oPage As PageBase) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CostCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CostCenterMethods.GetCostCenters(oState)

                oSession.States.CostCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CostCenterState.Result = CostCenterResultEnum.NoError Then
                    If wsRet.Value IsNot Nothing AndAlso wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CostCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-136")
            End Try

            Return oRet

        End Function

        Public Shared Function GetAvailableCostCentersByEmployee(ByVal oPage As PageBase, ByVal IDEmployee As Integer, ByVal xDate As Date, ByVal bolCheckPassportPermission As Boolean) As DataTable

            'Return oRet
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CostCenterState

            WebServiceHelper.SetState(oState)

            Try

                Dim wsRet As roGenericVtResponse(Of DataSet) = VTLiveApi.CostCenterMethods.GetAvailableCostCentersDataTable(IDEmployee, xDate, bolCheckPassportPermission, oState)

                oSession.States.CostCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.CostCenterState.Result = CostCenterResultEnum.NoError Then
                    If wsRet.Value IsNot Nothing AndAlso wsRet.Value.Tables.Count > 0 Then
                        oRet = wsRet.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    If oPage IsNot Nothing Then HelperWeb.ShowError(oPage, oSession.States.CostCenterState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-137")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeWorkingCostCenter(ByVal oPage As PageBase, ByVal IDEmployee As Integer, ByRef IdCostCenter As Integer) As String

            Dim oRet As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.CostCenterState

            WebServiceHelper.SetState(oState)

            Try
                Dim wsRet As roGenericVtResponse(Of roBusinessCenter) = VTLiveApi.CostCenterMethods.GetEmployeeWorkingCostCenter(IDEmployee, IdCostCenter, oState)

                oSession.States.CostCenterState = wsRet.Status
                roWsUserManagement.SessionObject = oSession
                If oSession.States.CostCenterState.Result <> CostCenterResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.CostCenterState)
                End If

                If wsRet.Value IsNot Nothing AndAlso wsRet.Value.ID > 0 Then
                    oRet = wsRet.Value.Name
                    IdCostCenter = wsRet.Value.ID
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-138")
            End Try

            Return oRet
        End Function

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.CostCenterState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace