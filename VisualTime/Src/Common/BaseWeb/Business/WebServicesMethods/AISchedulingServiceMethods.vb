Imports System.Web.UI
Imports Robotics.Base.DTOs

Namespace API

    Public NotInheritable Class AISchedulingServiceMethods

#Region "Productive Units"

        Public Shared Function GetProductiveUnits(ByVal oPage As Page) As Generic.List(Of roProductiveUnit)
            Dim oRet As roProductiveUnitListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetProductiveUnits(oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProductiveUnitState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-039")
            End Try
            Return oRet.ProductiveUnits.ToList

        End Function

        Public Shared Function SaveProductiveUnit(ByVal oPage As Page, ByVal oProductiveUnit As roProductiveUnit, ByVal bAudit As Boolean) As roProductiveUnit
            Dim oRet As roProductiveUnitResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.SaveProductiveUnit(oProductiveUnit, bAudit, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProductiveUnitState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-040")
            End Try

            Return oRet.ProductiveUnit
        End Function

        Public Shared Function GetProductiveUnitById(ByVal oPage As Page, ByVal idProductiveUnit As Integer, ByVal bAudit As Boolean) As roProductiveUnit
            Dim oRet As roProductiveUnitResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetProductiveUnitById(idProductiveUnit, bAudit, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-041")
            End Try

            Return oRet.ProductiveUnit
        End Function

        Public Shared Function GetProductiveUnitSummary(ByVal oPage As Page, ByVal oProductiveUnit As roProductiveUnit, ByVal _ProductiveUnitSummaryType As ProductiveUnitSummaryType) As roProductiveUnitSummary
            Dim oRet As roProductiveUnitSummaryResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetProductiveUnitSummary(oProductiveUnit, _ProductiveUnitSummaryType, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProductiveUnitState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-042")
            End Try

            Return oRet.ProductiveUnitSummary
        End Function

        Public Shared Function DeleteProductiveUnit(ByVal oPage As Page, ByVal oProductiveUnit As roProductiveUnit, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roProductiveUnitStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.DeleteProductiveUnit(oProductiveUnit, bAudit, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    'Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProductiveUnitState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-043")
            End Try

            Return oRet.Result
        End Function

#End Region

#Region "Budgets"

        Public Shared Function GetBudget(ByVal oPage As Page, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal strNodeFilter As String, ByVal TypeView As BudgetView, ByVal DetailLevel As BudgetDetailLevel, ByVal bAudit As Boolean, ByVal _ProductiveUnit As Integer, ByVal strProductiveUnitFilter As String, ByVal loadIndictments As Boolean) As roBudget
            Dim oRet As roBudgetResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetBudget(xFirstDay, xLastDay, strNodeFilter, TypeView, DetailLevel, bAudit, oState, _ProductiveUnit, strProductiveUnitFilter, loadIndictments)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-044")
            End Try

            Return oRet.Budget

        End Function

        Public Shared Function GetBudgetHourPeriodDefinition(ByVal oPage As Page, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal iIDProductiveUnit As Integer, ByVal iIDSecurityNode As Integer, ByVal bAudit As Boolean, ByVal loadIndictments As Boolean) As roBudget
            Dim oRet As roBudgetResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetBudgetPeriodHourDefinition(xFirstDay, xLastDay, iIDProductiveUnit, iIDSecurityNode, bAudit, oState, loadIndictments)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BudgetState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-045")
            End Try

            Return oRet.Budget

        End Function

        Public Shared Function GetProductiveUnitsFromNode(ByVal oPage As Page, ByVal iIDSecurityNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime) As Generic.List(Of roProductiveUnit)
            Dim oRet As roProductiveUnitListResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetProductiveUnitsFromNode(iIDSecurityNode, xFirstDay, xLastDay, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProductiveUnitState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-046")
            End Try

            Return oRet.ProductiveUnits.ToList

        End Function

        Public Shared Function GetNewBudgetRow(ByVal oPage As Page, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime, ByVal iIDProductiveUnit As Integer, ByVal iIDSecurityNode As Integer, ByVal bAudit As Boolean) As roBudget
            Dim oRet As roBudgetResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetNewBudgetRow(xFirstDay, xLastDay, iIDProductiveUnit, iIDSecurityNode, bAudit, oState)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BudgetState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-047")
            End Try

            Return oRet.Budget

        End Function

        Public Shared Function RemoveNewBudgetRow(ByVal oPage As Page, ByVal iIDProductiveUnit As Integer, ByVal iIDSecurityNode As Integer, ByVal bAudit As Boolean) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.RemoveBudgetRowData(iIDSecurityNode, iIDProductiveUnit, bAudit, oState)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BudgetState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-048")
            End Try

            Return oRet.Result

        End Function

        Public Shared Function SaveBudget(ByVal oPage As Page, ByVal oBudget As roBudget, ByVal bAudit As Boolean) As roBudgetResult
            Dim oRet As roBudgetResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.SaveBudget(oBudget, bAudit, oState)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BudgetState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-049")
            End Try

            Return oRet.BudgetResult

        End Function

        Public Shared Function GetBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse(ByVal oPage As Page, ByVal _IDNode As Integer, ByVal _Day As DateTime, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition) As roBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse
            Dim oRet As roBudgetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummaryResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetEmployeeAvailableForPositionAndCurrentStatusEmployeesSummary(_IDNode, _Day, oProductiveUnitModePosition, oState)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BudgetState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-050")
            End Try

            Return oRet
        End Function

        Public Shared Function AddEmployeePlanOnPosition(ByVal oPage As Page, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.AddEmployeePlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.ProductiveUnitState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-051")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function AddEmployeesPlanOnPosition(ByVal oPage As Page, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData()) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.AddEmployeesPlanOnPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-052")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function RemoveEmployeeFromPosition(ByVal oPage As Page, ByVal oProductiveUnitModePosition As roProductiveUnitModePosition, ByVal xPlanDate As Date, ByVal oProductiveUnitModePositionEmployeeData As roProductiveUnitModePositionEmployeeData) As Boolean
            Dim oRet As roStandarResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.ProductiveUnitState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.RemoveEmployeeFromPosition(oProductiveUnitModePosition, xPlanDate, oProductiveUnitModePositionEmployeeData, oState)
                oSession.States.ProductiveUnitState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.ProductiveUnitState.Result <> ProductiveUnitResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.DocumentState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-053")
            End Try

            Return oRet.Result
        End Function

        Public Shared Function GetEmployeeAvailableForNode(ByVal oPage As Page, ByVal _IDNode As Integer, ByVal xFirstDay As DateTime, ByVal xLastDay As DateTime) As roEmployeeAvailableForNode()
            Dim oRet As roEmployeeAvailableForNodeResponse = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.BudgetState
            WebServiceHelper.SetState(oState)

            Try
                oRet = VTLiveApi.AISchedulingMethods.GetEmployeeAvailableForNode(_IDNode, xFirstDay, xLastDay, oState)
                oSession.States.BudgetState = oRet.oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.BudgetState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-054")
            End Try

            Return oRet.AvailableEmployees
        End Function

#End Region

#Region "AiScheuduler"

        'Public Shared Function RunAISchedulerForBudget(ByVal oPage As Page, ByVal oBudget As roBudget, ByVal bAudit As Boolean) As roCalendarResponse
        '    Dim oRet As roCalendarResponse = Nothing

        '    Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '    Dim oState As roWsState = oSession.States.BudgetState
        '    WebServiceHelper.SetState(oState)

        '        oRet = VTLiveApi.AISchedulingMethods.RunAISchedulerForBudget(oBudget, bAudit, oState)
        '        oSession.States.BudgetState = oRet.oState
        '        roWsUserManagement.SessionObject = oSession

        '        If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
        '            ' Mostrar el error
        '            HelperWeb.ShowError(oPage, oSession.States.BudgetState)
        '        End If

        '    Return oRet

        'End Function

        'Public Shared Function RemoveAIScheduledForBudget(ByVal oPage As Page, ByVal oBudget As roBudget, ByVal bAudit As Boolean) As roCalendarResponse
        '    Dim oRet As roCalendarResponse = Nothing

        '    Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '    Dim oState As roWsState = oSession.States.BudgetState
        '    WebServiceHelper.SetState(oState)

        '        oRet = VTLiveApi.AISchedulingMethods.RemoveAIScheduledForBudget(oBudget, bAudit, oState)
        '        oSession.States.BudgetState = oRet.oState
        '        roWsUserManagement.SessionObject = oSession

        '        If oSession.States.BudgetState.Result <> BudgetResultEnum.NoError Then
        '            ' Mostrar el error
        '            HelperWeb.ShowError(oPage, oSession.States.BudgetState)
        '        End If

        '    Return oRet

        'End Function

#End Region

#Region "Last errors"

        Public Shared Function LastProductiveErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProductiveUnitState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastProductiveResult() As ProductiveUnitResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.ProductiveUnitState.Result
            End If
            Return strRet
        End Function

        Public Shared Function LastBudgetStateErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastBudgetStateResult() As BudgetResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BudgetState.Result
            End If
            Return strRet
        End Function

        Public Shared Function LastBudgetRowStateErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BudgetState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastBudgetRowStateResult() As BudgetRowResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BudgetRowState.Result
            End If
            Return strRet
        End Function

        Public Shared Function LastBudgetRowPeriodStateErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BudgetRowPeriodState.ErrorText
            End If
            Return strRet
        End Function

        Public Shared Function LastBudgetRowPeriodStateResult() As BudgetRowPeriodDataResultEnum
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.BudgetRowPeriodState.Result
            End If
            Return strRet
        End Function

#End Region

    End Class

End Namespace