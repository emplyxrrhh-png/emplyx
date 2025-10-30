Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Move
Imports Robotics.Base.VTBusiness.Punch
Imports Robotics.Base.VTEmployees.Employee
Imports Robotics.Base.VTUserFields.UserFields

Namespace API

    Public NotInheritable Class EmployeeServiceMethods

#Region "Employee"

        Public Shared Function GetAssignments(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal bAudit As Boolean) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeeAssignmentsDatatable(_IDEmployee, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-244")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal bAudit As Boolean) As roEmployee

            Dim oRet As roEmployee = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployee) = VTLiveApi.EmployeeMethods.GetEmployee(_IDEmployee, oState, bAudit)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-245")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeName(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As String

            Dim strName As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.EmployeeMethods.GetEmployeeName(IDEmployee, oState)
                strName = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-246")
            End Try
            Return strName

        End Function

        Public Shared Function SaveEmployeeAssignments(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, ByVal lstEmployeeAssignments As Generic.List(Of roEmployeeAssignment)) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveEmployeeAssignments(intIDEmployee, lstEmployeeAssignments, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-247")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveEmployee(ByVal oPage As System.Web.UI.Page, ByRef oEmployee As roEmployee) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployee) = VTLiveApi.EmployeeMethods.SaveEmployee(oEmployee, oState)
                If response.Value Is Nothing Then
                    bolRet = False
                Else
                    bolRet = True
                End If

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-248")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeFromList(ByVal oPage As System.Web.UI.Page, ByVal IDList As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeeFromList(IDList, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-249")
            End Try
            Return oRet

        End Function


        Public Shared Function CheckIfEmployeeHasData(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.CheckIfEmployeeHasData(IDEmployee, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If Not bolRet Then
                    If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                        HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-251")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteEmployee(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DeleteEmployee(IDEmployee, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If Not bolRet Then
                    If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                        HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-251")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeSelectionPath(ByVal oPage As System.Web.UI.Page, ByVal idEmployee As Integer) As roEmployeeSelectionPath

            Dim employeeInfo As roEmployeeSelectionPath = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployeeSelectionPath) = VTLiveApi.EmployeeMethods.GetEmployeeSelectionPath(idEmployee, oState)
                employeeInfo = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-252")
            End Try

            Return employeeInfo

        End Function

        Public Shared Function GetIdEmployeeByName(ByVal oPage As System.Web.UI.Page, ByVal strName As String) As Integer

            Dim intRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.EmployeeMethods.GetIdEmployeeByName(strName, oState)
                intRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-252")
            End Try

            Return intRet

        End Function

        Public Shared Function GetActiveEmployeesCount(ByVal oPage As System.Web.UI.Page, ByVal _Date As Date) As Integer

            Dim intRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.EmployeeMethods.GetActiveEmployeesCount(_Date, oState)
                intRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-253")
            End Try

            Return intRet

        End Function

        Public Shared Function GetActiveEmployeesTaskCount(ByVal oPage As System.Web.UI.Page, ByVal _Date As Date) As Integer

            Dim intRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.EmployeeMethods.GetActiveEmployeesTaskCount(_Date, oState)
                intRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-254")
            End Try

            Return intRet

        End Function

        Public Shared Function GetAllEmployees(ByVal oPage As System.Web.UI.Page, ByVal strWhere As String, ByVal strFeature As String, Optional ByVal strFeatureType As String = "U") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetAllEmployees(strWhere, strFeature, strFeatureType, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                ' oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "WebService.Error") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-255")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployees(ByVal oPage As System.Web.UI.Page, ByVal strWhere As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployees(strWhere, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                Dim oLanguage As New roLanguageWeb
                oTmpState.Result = 1
                ' oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "WebService.Error") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-255")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesByName(ByVal oPage As System.Web.UI.Page, ByVal strLikeName As String, ByVal strWhere As String, ByVal strFeature As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesByName(strLikeName, strWhere, strFeature, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-257")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesByIDContract(ByVal oPage As System.Web.UI.Page, ByVal strLikeIDContract As String, ByVal strWhere As String, ByVal strFeature As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesByIDContract(strLikeIDContract, strWhere, strFeature, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-258")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesByIDCard(ByVal oPage As System.Web.UI.Page, ByVal strLikeIDCard As String, ByVal strWhere As String, ByVal strFeature As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesByIDCard(strLikeIDCard, strWhere, strFeature, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-259")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesByPlate(ByVal oPage As System.Web.UI.Page, ByVal strLikePlate As String, ByVal strWhere As String, ByVal strFeature As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesByPlate(strLikePlate, strWhere, strFeature, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-260")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesById(ByVal oPage As System.Web.UI.Page, ByVal IdEmployee As Integer, ByVal strFeature As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesById(IdEmployee, strFeature, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-261")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesByAdvancedFilter(ByVal oPage As System.Web.UI.Page, ByVal strAdvancedFilter As String, ByVal strWhere As String, ByVal strFeature As String) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesByAdvancedFilter(strAdvancedFilter, strWhere, strFeature, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-262")
            End Try

            Return oRet

        End Function

        Public Shared Function CreateMultiEmployees(ByVal oPage As System.Web.UI.Page, ByVal tbEmployeesData As DataTable, ByRef lstEmployeeNameError As List(Of String), Optional activities As List(Of String) = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim ds As DataSet
                If tbEmployeesData.DataSet Is Nothing Then
                    ds = New DataSet
                    ds.Tables.Add(tbEmployeesData)
                Else
                    ds = tbEmployeesData.DataSet
                End If

                Dim response As roGenericVtResponse(Of List(Of String)) = VTLiveApi.EmployeeMethods.CreateMultiEmployees(ds, oState)
                bolRet = (response.Value.Count = 0)
                lstEmployeeNameError = response.Value.ToList

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-263")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetDocumentToView(ByVal oPage As System.Web.UI.Page, ByVal strFileName As String) As Byte()

            Dim arrByte() As Byte = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Byte()) = VTLiveApi.EmployeeMethods.GetDocumentToView(strFileName, oState)
                arrByte = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-264")
            End Try
            Return arrByte

        End Function

        Public Shared Function GetEmployeesSummaryById(ByVal oPage As System.Web.UI.Page, ByVal IdEmployee As Integer, onDate As Date, accrualSummaryType As SummaryType, causesSummaryType As SummaryType, tasksSummaryType As SummaryType, centersSummaryType As SummaryType, requestType As SummaryRequestType) As roEmployeeSummary

            Dim oRet As roEmployeeSummary = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployeeSummary) = VTLiveApi.EmployeeMethods.GetEmployeeSummary(IdEmployee, onDate, accrualSummaryType, causesSummaryType, tasksSummaryType, centersSummaryType, requestType, oState)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-266")
            End Try

            Return oRet

        End Function

#End Region

#Region "UserFields"

        Public Shared Function GetUserFieldsDataset(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, Optional ByVal xDate As Date = Nothing) As DataSet

            Dim oDataset As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try

                If xDate = Nothing Then xDate = Now.Date

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetUserFieldsDataset(IDEmployee, xDate, oState)
                oDataset = response.Value

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-267")
            End Try

            Return oDataset

        End Function

        Public Shared Function GetUserFields(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, Optional ByVal xDate As Date = Nothing) As Generic.List(Of roEmployeeUserField)

            Dim oList As Generic.List(Of roEmployeeUserField) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try

                If xDate = Nothing Then xDate = Now.Date

                Dim response As roGenericVtResponse(Of List(Of roEmployeeUserField)) = VTLiveApi.EmployeeMethods.GetUserFields(intIDEmployee, xDate, oState)

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If response.Value IsNot Nothing Then
                    oList = response.Value
                End If
                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-268")
            End Try
            Return oList

        End Function

        Public Shared Function SaveUserField(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal strFieldName As String, ByVal strFieldValue As Object, Optional ByVal xDate As Date = Nothing, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try

                If xDate = Nothing Then xDate = Now.Date
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveUserField(IDEmployee, strFieldName, xDate, strFieldValue, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-269")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveUserFields(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal _UserFields As Generic.List(Of roEmployeeUserField), ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveUserFields(IDEmployee, _UserFields, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-270")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetUserFieldHistoryDatatable(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, ByVal strFieldName As String) As DataTable

            Dim tbRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetUserFieldHistoryDataset(intIDEmployee, strFieldName, oState)

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                    tbRet = response.Value.Tables(0)
                End If
                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-271")
            End Try

            Return tbRet

        End Function

        Public Shared Function SaveUserFieldHistory(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal tbHistory As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveUserFieldHistory(IDEmployee, tbHistory, oState)
                bolRet = response.Value

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-272")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteUserFieldHistory(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal _FieldName As String, ByVal _FromDate As Date) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DeleteUserFieldHistory(IDEmployee, _FieldName, _FromDate, oState)
                bolRet = response.Value

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-273")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeUserFieldValueAtDate(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _FieldName As String, ByVal _Date As Date) As roEmployeeUserField

            Dim oRet As roEmployeeUserField = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeUserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployeeUserField) = VTLiveApi.EmployeeMethods.GetEmployeeUserFieldValueAtDate(_IDEmployee, _FieldName, _Date, oState)
                oRet = response.Value

                oSession.States.EmployeeUserFieldState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeUserFieldState.Result <> UserFieldResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeUserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-274")
            End Try

            Return oRet

        End Function

#End Region

#Region "Authorizations Management"

        Public Shared Function AddAccessGroupToEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _IDAuthorization As Integer, Optional ByVal bolAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = True

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.AddAccessGroupToEmployee(_IDEmployee, _IDAuthorization, bolAudit, oState)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                    oRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-275")
            End Try
            Return oRet
        End Function

        Public Shared Function RemoveAccessGroupToEmployee(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _IDAuthorization As Integer, Optional ByVal bolAudit As Boolean = False) As Boolean
            Dim oRet As Boolean = True

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.RemoveAccessGroupToEmployee(_IDEmployee, _IDAuthorization, bolAudit, oState)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                    oRet = False
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-276")
            End Try

            Return oRet
        End Function

        Public Shared Function SaveAccessAuthorizations(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _IDGroup As Integer, ByVal tbAuthorizations As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim dsAuthorizations As DataSet
                If tbAuthorizations.DataSet IsNot Nothing Then
                    dsAuthorizations = tbAuthorizations.DataSet
                Else
                    dsAuthorizations = New DataSet
                    dsAuthorizations.Tables.Add(tbAuthorizations)
                End If

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveAccessAuthorizations(_IDEmployee, _IDGroup, dsAuthorizations, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-277")
            End Try

            Return bolRet

        End Function

#End Region

        Public Shared Function LaboralDaysInPeriod(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal xBeginPeriod As Date, ByVal xEndPeriod As Date) As Integer

            Dim oRet As Integer = 0

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.EmployeeMethods.LaboralDaysInPeriod(_IDEmployee, xBeginPeriod, xEndPeriod, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-278")
            End Try

            Return oRet
        End Function

        Public Shared Function GetTelecommuteStatsAtDate(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal xDate As Date) As roEmployeeTelecommuteAgreementStats

            Dim oRet As roEmployeeTelecommuteAgreementStats = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)
            Try
                Dim response As roGenericVtResponse(Of roEmployeeTelecommuteAgreementStats) = VTLiveApi.EmployeeMethods.GetTelecommuteStatsAtDate(_IDEmployee, xDate, oState)

                oRet = response.Value
                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-278")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteBiometricData(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DeleteBiometricData(_IDEmployee, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-280")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteBiometricDataForAllEmployees(ByVal oPage As System.Web.UI.Page) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DeleteBiometricDataForAllEmployees(oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-280")
            End Try

            Return bolRet

        End Function

        Public Shared Function DisableBiometricDataForAllEmployees(ByVal oPage As System.Web.UI.Page, ByVal disabled As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DisableBiometricDataForAllEmployees(disabled, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-280")
            End Try

            Return bolRet

        End Function

        Public Shared Function RemoveEmployeeData(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal bRemoveEmployeePhoto As Boolean, ByVal bRemoveBiometricData As Boolean, ByVal bRemoveEmployeePunchImages As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DeleteEmployeeRelatedData(_IDEmployee, bRemoveEmployeePhoto, bRemoveBiometricData, bRemoveEmployeePunchImages, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-280")
            End Try

            Return bolRet

        End Function

#Region "Mobility"

        Public Shared Function GetCurrentGroupName(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As String
            Dim strFullGroupName As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.EmployeeMethods.GetCurrentGroupName(IDEmployee, oState)
                strFullGroupName = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-281")
            End Try

            Return strFullGroupName

        End Function

        Public Shared Function GetCurrentFullGroupName(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As String
            Dim strFullGroupName As String = String.Empty

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.EmployeeMethods.GetCurrentFullGroupName(IDEmployee, oState)
                strFullGroupName = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-281")
            End Try

            Return strFullGroupName

        End Function

        Public Shared Function GetCurrentMobility(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As roMobility

            Dim oMobility As roMobility = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roMobility) = VTLiveApi.EmployeeMethods.GetCurrentMobility(IDEmployee, oState)
                oMobility = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-282")
            End Try

            Return oMobility

        End Function

        Public Shared Function SaveMobility(ByVal oPage As System.Web.UI.Page, ByVal intIDEmployee As Integer, ByVal oMobility As roMobility, ByVal CallBroadCaster As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveMobility(intIDEmployee, oMobility, oState, CallBroadCaster)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-283")
            End Try

            Return bolRet

        End Function

        Public Shared Function UpdateEmployeeGroup(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _IDGroup As Integer, ByVal _FromDate As Date,
                                                   ByVal bolCopyPlan As Boolean, ByVal _SourceIDEmployee As Integer, ByVal _
                                                   _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                                   ByRef xDateLocked As Date, Optional ByVal ShiftType As ActionShiftType = ActionShiftType.AllShift, Optional ByVal copyHolidays As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.UpdateEmployeeGroup(_IDEmployee, _IDGroup, _FromDate, bolCopyPlan, _SourceIDEmployee, ShiftType,
                                                              _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-284")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveEmployeeLockDate(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _LockDate As Date, ByVal _EmployeeLockDateType As Boolean, ByVal bAudit As Boolean) As Boolean
            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveEmployeeLockDate(_IDEmployee, _LockDate, _EmployeeLockDateType, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-284")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeLockDatetoApply(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByRef EmployeeLockDateType As Boolean, ByVal bAudit As Boolean) As Date

            Dim oRet As Date = New Date(1900, 1, 1)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployeeLockDateInfo) = VTLiveApi.EmployeeMethods.GetEmployeeLockDatetoApply(IDEmployee, EmployeeLockDateType, oState, bAudit)
                oRet = response.Value.EmployeeLockDate
                EmployeeLockDateType = response.Value.EmployeeLockDateType

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-282")
            End Try

            Return oRet

        End Function

        Public Shared Function SaveLockDate(ByVal oPage As System.Web.UI.Page, ByVal xLockDate As Date,
                                              ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveLockDate(xLockDate, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-125")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeesOnLockDate(ByVal oPage As System.Web.UI.Page, ByVal _strEmployeeFilter As String, xDate As Date) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeesOnLockDate(_strEmployeeFilter, xDate, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-279")
            End Try

            Return oRet
        End Function

        Public Shared Function GetMobilities(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetMobilities(_IDEmployee, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        tb = response.Value.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-285")
            End Try

            Return tb

        End Function

        Public Shared Function ValidateMobilities(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByRef tbMobilities As DataTable, ByRef intInvalidRow As Integer) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim dsMobilities As DataSet
                If tbMobilities.DataSet IsNot Nothing Then
                    dsMobilities = tbMobilities.DataSet
                Else
                    dsMobilities = New DataSet
                    dsMobilities.Tables.Add(tbMobilities)
                End If

                Dim response As roGenericVtResponse(Of roMobilityValidation) = VTLiveApi.EmployeeMethods.ValidateMobilities(_IDEmployee, dsMobilities, oState)

                tbMobilities = response.Value.Mobilities.Tables(0)
                intInvalidRow = response.Value.InvalidRow
                bolRet = response.Value.Valid

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-286")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveMobilities(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal tbMobilities As DataTable) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim dsMobilities As DataSet
                If tbMobilities.DataSet IsNot Nothing Then
                    dsMobilities = tbMobilities.DataSet
                Else
                    dsMobilities = New DataSet
                    dsMobilities.Tables.Add(tbMobilities)
                End If

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveMobilities(_IDEmployee, dsMobilities, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-287")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Schedule"

        ''' <summary>
        ''' Copia la planificación de horarios de un empleado a otro, indicando una fecha inicio a partir de la que se copiará.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
        ''' <param name="xBegin">Fecha inicial de la planificación.</param>
        ''' <param name="ShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        '''  </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyPlan(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer,
                                        ByVal xBegin As Date, ByVal ShiftType As ActionShiftType,
                                        ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                        ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyPlan(intSourceIDEmployee, intDestinationIDEmployee, xBegin, ShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction,
                                                   xDateLocked, copyHolidays, oState, bAudit)

                bolRet = response.Value.CopyPlanResult
                xDateLocked = response.Value.EmployeeLockDate

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-288")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal lstDestinationIDEmployees As ArrayList,
                                          ByVal xBegin As Date, ByVal xEnd As Date, ByVal ShiftType As ActionShiftType,
                                          ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                          ByRef intIDEmployeeLocked As Integer, ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean
            'ByVal xBeginIni As Date

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                If lstDestinationIDEmployees.Count > 0 Then

                    If intIDEmployeeLocked > 0 Then
                        Dim bolValidID As Boolean = False
                        For Each intID As Integer In lstDestinationIDEmployees
                            If intID = intIDEmployeeLocked Then
                                bolValidID = True
                                Exit For
                            End If
                        Next
                        If Not bolValidID Then intIDEmployeeLocked = 0
                    End If

                    If xDateLocked <> Nothing AndAlso (xDateLocked < xBegin OrElse xDateLocked > xEnd) Then
                        xDateLocked = Nothing
                    End If

                    For Each intIDEmployee As Integer In lstDestinationIDEmployees

                        If intIDEmployeeLocked <= 0 OrElse (intIDEmployeeLocked > 0 And intIDEmployeeLocked = intIDEmployee) Then

                            intIDEmployeeLocked = 0

                            Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShifts(intSourceIDEmployee, intIDEmployee, xBegin, xEnd, ShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, oState, copyHolidays, bAudit)
                            bolRet = response.Value.CopyPlanResult
                            xDateLocked = response.Value.EmployeeLockDate

                            oSession.States.EmployeeState = response.Status
                            roWsUserManagement.SessionObject = oSession

                            If Not response.Value.CopyPlanResult Then
                                If oSession.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                                    Exit For
                                ElseIf oSession.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Or
                                       oSession.States.EmployeeState.Result = EmployeeResultEnum.ShiftWithoutPermission Or
                                       oSession.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleCoverageDay Then
                                    intIDEmployeeLocked = intIDEmployee
                                    Exit For
                                End If
                            End If

                        End If

                    Next
                    bolRet = (oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Or
                              oSession.States.EmployeeState.Result = EmployeeResultEnum.InPeriodOfFreezing Or
                              oSession.States.EmployeeState.Result = EmployeeResultEnum.EmployeeNoActiveContract Or
                              oSession.States.EmployeeState.Result = EmployeeResultEnum.AccessDenied)
                Else
                    bolRet = True
                End If
                If Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-289")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de horarios de un empleado a otro, indicando un periodo origen y un fecha destino.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
        ''' <param name="xSourceBegin">Fecha inicio del periodo a copiar.</param>
        ''' <param name="xSourceEnd">Fecha fin del periodo a copiar.</param>
        ''' <param name="xDestinationBegin">Fecha inicio del periodo al que se copiará.</param>
        ''' <param name="ShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        '''  </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer,
                                          ByVal xSourceBegin As Date, ByVal xSourceEnd As Date, ByVal xDestinationBegin As Date, ByVal ShiftType As ActionShiftType,
                                          ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                          ByRef xDateLocked As Date, ByVal copyHolidays As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsPeriod(intSourceIDEmployee, intDestinationIDEmployee, xSourceBegin, xSourceEnd, xDestinationBegin, ShiftType,
                                                  _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState)

                bolRet = response.Value.CopyPlanResult
                xDateLocked = response.Value.EmployeeLockDate

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-290")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de horarios de un empleado a otro, indicando un periodo origen y un periodo destino.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fecha modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="intSourceIDEmployee">Código del empleado del que se obtendrá la planificación.</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado al que se le copiará la planificación.</param>
        ''' <param name="xSourceBegin">Fecha inicio del periodo a copiar.</param>
        ''' <param name="xSourceEnd">Fecha fin del periodo a copiar.</param>
        ''' <param name="xDestinationBegin">Fecha inicio del periodo al que se copiará.</param>
        ''' <param name="xDestinationEnd">Fecha fin del periodo al que se copiará.</param>
        ''' <param name="ShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        '''  </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer,
                                          ByVal xSourceBegin As Date, ByVal xSourceEnd As Date, ByVal xDestinationBegin As Date, ByVal xDestinationEnd As Date, ByVal ShiftType As ActionShiftType,
                                          ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                          ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsPeriodWithEnd(intSourceIDEmployee, intDestinationIDEmployee, xSourceBegin, xSourceEnd, xDestinationBegin, xDestinationEnd, ShiftType,
                                                         _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState, bAudit)

                bolRet = response.Value.CopyPlanResult
                xDateLocked = response.Value.EmployeeLockDate

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-291")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de horarios de unos empleados a otros. Indicando una fecha origen y otra destino.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="SourceIDEmployees">Lista de códigos de los empleados origen.</param>
        ''' <param name="DestinationIDEmployees">Lista de códigos de los empleados destino.</param>
        ''' <param name="xSourceDate">Fecha origen.</param>
        ''' <param name="xDestinationDate">Fecha destino.</param>
        ''' <param name="ShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLoked).<br></br>Si se informa un empleado (distinta de 0 y dentro del grupo de empleados) se utiliza como empleado inicio del grupo de copia.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date,
                                          ByVal xDestinationDate As Date, ByVal ShiftType As ActionShiftType, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction,
                                          ByVal _ShiftPermissionAction As ShiftPermissionAction, ByRef intIDEmployeeLocked As Integer, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsEmployees(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, ShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, oState, copyHolidays, bAudit)

                bolRet = response.Value.CopyPlanResult
                intIDEmployeeLocked = response.Value.IDEmployeeLocked

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-292")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Copia la planificación de horarios de unos empleados a otros, indicando una fecha origen y un periodo destino. Los horarios de la fecha origen se repetiran en el periodo destino.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="SourceIDEmployees">Lista de códigos de los empleados origen.</param>
        ''' <param name="DestinationIDEmployees">Lista de códigos de los empleados destino.</param>
        ''' <param name="xSourceDate">Fecha origen.</param>
        ''' <param name="xDestinationDate">Fecha inicio destino.</param>
        ''' <param name="xDestinationEndDate">Fecha fin destino.</param>
        ''' <param name="ShiftType">Para indicar que tipo de horarios copiar: 0- Copia solo los horarios principales, 1- Copia solo los horarios alternativos, 2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar para el empleado bloqueado (intIDEmployeeLocked).</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer,
                                          ByVal xSourceDate As Date, ByVal xDestinationDate As Date, ByVal xDestinationEndDate As Date, ByVal ShiftType As ActionShiftType,
                                          ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction, ByRef intIDEmployeeLocked As Integer, ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsEmployeesWithEnd(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, xDestinationEndDate, ShiftType,
                                                            _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, xDateLocked, oState, copyHolidays, bAudit)

                bolRet = response.Value.CopyPlanResult
                intIDEmployeeLocked = response.Value.IDEmployeeLocked
                xDateLocked = response.Value.EmployeeLockDate

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-293")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetRemarkText(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As String

            Dim strRet As String = ""

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of String) = VTLiveApi.EmployeeMethods.GetRemarkText(_IDEmployee, _Date, oState)
                strRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-294")
            End Try

            Return strRet

        End Function

        Public Shared Function SaveRemarkText(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal strText As String) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SetRemarkText(_IDEmployee, _Date, strText, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Or Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-295")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetTaskPlan(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetTaskPlan(_IDEmployee, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    oRet = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-296")
            End Try

            Return oRet

        End Function

        Public Shared Function GetScheduleStatus(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetScheduleStatus(_IDEmployee, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    oRet = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-297")
            End Try

            Return oRet

        End Function

        Public Shared Function GetPlan(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetPlan(_IDEmployee, _Date, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    oRet = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-298")
            End Try

            Return oRet

        End Function

        Public Shared Function GetPlan(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _BeginDate As Date, ByVal _EndDate As Date) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetPlan(_IDEmployee, _BeginDate, _EndDate, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    oRet = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-299")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Planifica un horario (y sus alternativos si es necesario) a un empleado para una fecha.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee">Código del empleado.</param>
        ''' <param name="_Date">Fecha a planificar.</param>
        ''' <param name="_IDShift1">Código del horario principal.</param>
        ''' <param name="_IDShift2">Código del primer horario alternativo. Si no existe horario alternativo informar zero.</param>
        ''' <param name="_IDShift3">Código del segundo horario alternativo. Si no existe horario alternativo informar zero.</param>
        ''' <param name="_IDShift4">Código del tercer horario alternativo. Si no existe horario alternativo informar zero.</param>
        ''' <param name="_StartShift1">Hora de inicio si el horario 1 es flotante. Si no se trata de un horario flotante se debe indicar Nothing.</param>
        ''' <param name="_StartShift2">Hora de inicio si el horario alternativo 2 es flotante. Si no se trata de un horario flotante se debe indicar Nothing.</param>
        ''' <param name="_StartShift3">Hora de inicio si el horario alternativo 3 es flotante. Si no se trata de un horario flotante se debe indicar Nothing.</param>
        ''' <param name="_StartShift4">Hora de inicio si el horario alternativo 4 es flotante. Si no se trata de un horario flotante se debe indicar Nothing.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function AssignShift(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal _IDShift1 As Integer, ByVal _IDShift2 As Integer, ByVal _IDShift3 As Integer, ByVal _IDShift4 As Integer,
                                           ByVal _StartShift1 As DateTime, ByVal _StartShift2 As DateTime, ByVal _StartShift3 As DateTime, ByVal _StartShift4 As DateTime,
                                           ByVal _IDAssignment As Integer, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal bAudit As Boolean,
                                           Optional ByVal _ShiftPermissionAction As ShiftPermissionAction = ShiftPermissionAction.ContinueAll) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.AssignShift(_IDEmployee, _Date, _IDShift1, _IDShift2, _IDShift3, _IDShift4, _StartShift1, _StartShift2, _StartShift3, _StartShift4, _IDAssignment,
                                                      _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-300")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Asigna el horario como el primer alternativo no informado del empleado y fecha. <br/>
        ''' Si no hay horario principal informado o todos los alternativos ya están informados devuelve 'false'.<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
        ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee">Código del empleado.</param>
        ''' <param name="_Date">Fecha a planificar.</param>
        ''' <param name="_IDAlterShift">Código del horario alternativo.</param>
        ''' <param name="_AlterStartShift">Hora de inicio del horario flotante alternativo. Si no se trata de un horario flotante debe especificar Nothing.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function AssignAlterShift(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal _IDAlterShift As Integer, ByVal _AlterStartShift As DateTime, ByVal _LockedDayAction As LockedDayAction) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.AssignAlterShift(_IDEmployee, _Date, _IDAlterShift, _AlterStartShift, _LockedDayAction, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-301")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina la planificación de un empleado para una fecha, indicando el horario a eliminar (-1 se eliminan todos: el principal y los alternativos).<br/>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que el empleado tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee">Código del empleado.</param>
        ''' <param name="_ShiftPosition">Posicición del horario a eliminar: -1. Todos, 1. Horario Principal, 2. Primer horario alternativo, 3. Segundo horario alternativo, 4. Tercer horario alternativo.</param>
        ''' <param name="_Date">Fecha en la que se modificará la planificación.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <returns>True o False</returns>
        ''' <remarks>En caso de retorno a false: a través del parámetro de retorno 'oState' se indica el motivo.</remarks>
        Public Shared Function RemoveShift(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal _ShiftPosition As Integer, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.RemoveShift(_IDEmployee, _ShiftPosition, _Date, _LockedDayAction, _CoverageDayAction, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-302")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Planifica la lista de horarios a un empleado entre fechas.<br></br>
        ''' Si es necesario, la lista de horarios se asigna de forma cíclica.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.<br></br>
        ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="lstShifts">Lista de horarios (IDShift1*IDShift2*IDShift3*IDShift4).</param>
        ''' <param name="intDestinationIDEmployee">Código del empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha de inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha final de planificación.</param>
        ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyListShifts(ByVal oPage As System.Web.UI.Page, ByVal lstShifts As Generic.List(Of String), ByVal intDestinationIDEmployee As Integer,
                                              ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal intShiftType As ActionShiftType,
                                              ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction,
                                              ByRef xDateLocked As Date, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyListShifts(lstShifts, intDestinationIDEmployee, xBeginDate, xEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, copyHolidays, oState, bAudit)

                bolRet = response.Value.CopyPlanResult
                xDateLocked = response.Value.EmployeeLockDate

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-303")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Planifica la lista de horarios a varios empleado entre fechas.<br></br>
        ''' La lista de horarios corresponde a los horarios a asignar para una misma fecha para los distintos empleados. Si es necesario, la lista de horarios se asigna de forma cíclica para una misma fecha. Si hay varios días a planificar, se repite la lista para cada día.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre los empleados seleccionados.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' Notifica el cambio al servidor para que recalcule las fechas modificadas.<br></br>
        ''' Verifica el estado de bloqueo de los días a planificar, y planifica en función del parámetro '_LockedDayAction'.<br></br>
        ''' La forma de asignar los horarios es la siguiente: por cada fecha del periodo se recorre todos los empleados a planificar y les asigna el/los horarios correspondientes.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="lstShifts">Lista de horarios (IDShift1*IDShift2*IDShift3*IDShift4).</param>
        ''' <param name="lstDestionationIDEmployees">Lista de códigos de empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha fin de planificación.</param>
        ''' <param name="intShiftType">Tipo de horarios a copiar:<br></br>0- Copia sólo los horarios principales<br></br>1- Copia sólo los horarios alternativos<br></br>2- Copia todos los horarios.</param>
        ''' <param name="_LockedDayAction">Acción a realizar con los días bloqueados.<br></br>
        ''' None- Devuelve estado de error 'DailyScheduleLockedDay' y interrumpe el proceso de planificación. Los días anteriores a la fecha bloqueada quedan planificados.<br></br>
        ''' ReplaceFirst- Planifica sólo el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' ReplaceAll- Planifica todos los días bloqueados que se encuentra en el periodo.<br></br>
        ''' NoReplaceFirst- No planifica el primer día bloqueado que se encuentra en el periodo.<br></br>
        ''' NoReplaceAll- No planifica ningún día bloqueado en el periodo.
        ''' </param>
        ''' <param name="_CoverageDayAction">Acción a realizar con los días con covertura.</param>
        ''' <param name="xDateLocked">Devuelve la primera fecha bloqueada que no se ha podido planificar.<br></br>Si se informa una fecha (distinta de Nothing y dentro del periodo) se utiliza como inicio del periodo de copia.</param>
        ''' <param name="intIDEmployeeLocked">Devuelve el primer empleado que no se ha podido planificar para la fecha bloqueada (xDateLoked).<br></br>Si se informa un empleado (distinta de 0 y dentro del grupo de empleados) se utiliza como empleado inicio del grupo de copia.</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CopyListShiftsEmployees(ByVal oPage As System.Web.UI.Page, ByVal lstShifts As Generic.List(Of String), ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date,
                                                       ByVal xEndDate As Date, ByVal intShiftType As ActionShiftType, ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction,
                                                       ByVal _ShiftPermissionAction As ShiftPermissionAction, ByRef xDateLocked As Date,
                                                       ByRef intIDEmployeeLocked As Integer, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyListShiftsEmployees(lstShifts, lstDestionationIDEmployees, xBeginDate, xEndDate, intShiftType, _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, xDateLocked, intIDEmployeeLocked, copyHolidays, oState, bAudit)

                bolRet = response.Value.CopyPlanResult
                intIDEmployeeLocked = response.Value.IDEmployeeLocked
                xDateLocked = response.Value.EmployeeLockDate

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-304")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Bloquea/desbloquea los días informados a un empleado entre fechas.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="intDestinationIDEmployee">Código del empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha de inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha final de planificación.</param>
        ''' <param name="xLocked">Bloquear/desbloquear</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LockDaysList(ByVal oPage As System.Web.UI.Page, ByVal intDestinationIDEmployee As Integer, ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.LockDaysList(intDestinationIDEmployee, xBeginDate, xEndDate, xLocked, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-305")
            End Try
            Return bolRet

        End Function

        ''' <summary>
        ''' Bloquea/desbloquea los días informados a un empleado entre fechas.<br></br>
        ''' Verifica que el passport actual tenga permisos para planificar (Calendar.Scheduler) sobre el empleado seleccionado.<br/>
        ''' Verifica que el periodo no este dentro de congelación y que los empleados destino tenga contrato activo.<br/>
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="lstDestionationIDEmployees">Lista de códigos de empleado a planificar.</param>
        ''' <param name="xBeginDate">Fecha inicio de planificación.</param>
        ''' <param name="xEndDate">Fecha fin de planificación.</param>
        ''' <param name="xLocked">Bloquear/desbloquear el día</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function LockDaysListEmployees(ByVal oPage As System.Web.UI.Page, ByVal lstDestionationIDEmployees As Generic.List(Of Integer), ByVal xBeginDate As Date, ByVal xEndDate As Date, ByVal xLocked As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.LockDaysListEmployees(lstDestionationIDEmployees, xBeginDate, xEndDate, xLocked, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-306")
            End Try

            Return bolRet

        End Function

        '' ***************************** s'ha de borrar

        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal lstDestinationIDEmployees As ArrayList,
                                          ByVal xBegin As Date, ByVal xEnd As Date, ByVal ShiftType As ActionShiftType, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                If lstDestinationIDEmployees.Count > 0 Then
                    For Each intIDEmployee As Integer In lstDestinationIDEmployees

                        Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShifts(intSourceIDEmployee, intIDEmployee, xBegin, xEnd, ShiftType, LockedDayAction.None, LockedDayAction.None, ShiftPermissionAction.None, Nothing, Nothing, copyHolidays, bAudit)
                        oState = response.Status

                        If Not response.Value.CopyPlanResult Then
                            If oSession.States.EmployeeState.Result = EmployeeResultEnum.Exception Then
                                Exit For
                            ElseIf oSession.States.EmployeeState.Result = EmployeeResultEnum.DailyScheduleLockedDay Then
                                'intIDEmployeeLocked = intIDEmployee
                                Exit For
                            End If
                        End If
                    Next
                    bolRet = (oSession.States.EmployeeState.Result <> EmployeeResultEnum.Exception)

                    oSession.States.EmployeeState = oState
                    roWsUserManagement.SessionObject = oSession
                Else
                    bolRet = True
                End If
                If Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-307")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBegin As Date,
                                          ByVal xSourceEnd As Date, ByVal xDestinationBegin As Date, ByVal ShiftType As ActionShiftType, ByVal copyHolidays As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim xDateLocked As Date

                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsPeriod(intSourceIDEmployee, intDestinationIDEmployee, xSourceBegin, xSourceEnd, xDestinationBegin, ShiftType,
                                                  LockedDayAction.None, LockedDayAction.None, ShiftPermissionAction.None, xDateLocked, copyHolidays, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-308")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal intSourceIDEmployee As Integer, ByVal intDestinationIDEmployee As Integer, ByVal xSourceBegin As Date,
                                          ByVal xSourceEnd As Date, ByVal xDestinationBegin As Date, ByVal xDestinationEnd As Date, ByVal ShiftType As ActionShiftType, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim xDateLocked As Date

                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsPeriodWithEnd(intSourceIDEmployee, intDestinationIDEmployee, xSourceBegin, xSourceEnd, xDestinationBegin, xDestinationEnd, ShiftType,
                                                         LockedDayAction.None, LockedDayAction.None, ShiftPermissionAction.None, xDateLocked, copyHolidays, oState, bAudit)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-309")
            End Try
            Return bolRet

        End Function

        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer,
                                          ByVal xSourceDate As Date, ByVal xDestinationDate As Date, ByVal ShiftType As ActionShiftType, ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim intIDEmployeeLocked As Integer

                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsEmployees(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, ShiftType,
                                                     LockedDayAction.None, LockedDayAction.None, ShiftPermissionAction.None, intIDEmployeeLocked, oState, copyHolidays, bAudit)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-310")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyShifts(ByVal oPage As System.Web.UI.Page, ByVal SourceIDEmployees() As Integer, ByVal DestinationIDEmployees() As Integer, ByVal xSourceDate As Date, ByVal xDestinationDate As Date, ByVal xDestinationEndDate As Date, ByVal ShiftType As ActionShiftType,
                                          ByVal _LockedDayAction As LockedDayAction, ByVal _CoverageDayAction As LockedDayAction, ByVal _ShiftPermissionAction As ShiftPermissionAction, ByRef xDateLocked As Date, ByRef intIDEmployeeLocked As Integer,
                                          ByVal copyHolidays As Boolean, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of roEmployeeCopyPlanResult) = VTLiveApi.EmployeeMethods.CopyShiftsEmployeesWithEnd(SourceIDEmployees, DestinationIDEmployees, xSourceDate, xDestinationDate, xDestinationEndDate, ShiftType,
                                                            _LockedDayAction, _CoverageDayAction, _ShiftPermissionAction, intIDEmployeeLocked, xDateLocked, oState, copyHolidays, bAudit)

                bolRet = response.Value.CopyPlanResult
                xDateLocked = response.Value.EmployeeLockDate
                intIDEmployeeLocked = response.Value.IDEmployeeLocked

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                Else
                    bolRet = True
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-311")
            End Try

            Return bolRet

        End Function

        Public Shared Function AssignShift(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As DateTime, ByVal _IDShift1 As Integer, ByVal _IDShift2 As Integer, ByVal _IDShift3 As Integer, ByVal _IDShift4 As Integer,
                                           ByVal _StartShift1 As DateTime, ByVal _StartShift2 As DateTime, ByVal _StartShift3 As DateTime, ByVal _StartShift4 As DateTime,
                                           ByVal _IDAssignment As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.AssignShift(_IDEmployee, _Date, _IDShift1, _IDShift2, _IDShift3, _IDShift4, _StartShift1, _StartShift2, _StartShift3, _StartShift4,
                                                      _IDAssignment, LockedDayAction.None, LockedDayAction.None, ShiftPermissionAction.None, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> ShiftResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-312")
            End Try

            Return bolRet

        End Function

        ' *********************************************

#End Region

#Region "Presence Query Methods"



        '''' <summary>
        '''' Obtiene el estado de presencia de un empleado en una fecha/hora indicada.
        '''' </summary>
        '''' <param name="oPage">Pagina a la que se devuelven los errores</param>
        '''' <param name="_IDEmployee">Código del empleado</param>
        '''' <param name="_InputDateTime">Fecha y hora en la que se obtiene el estado</param>
        '''' <param name="_LastMoveType">Devuelve el tipo del último movimiento de presencia del empleado</param>
        '''' <param name="_LastMoveDateTime">Devuelve la fecha y hora del último movimiento del empleado</param>
        '''' <param name="_LastMove">Devuelve el último movimiento de presencia del empleado</param>
        '''' <returns></returns>
        '''' <remarks></remarks>
        'Public Shared Function GetPresenceStatus(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByRef _LastMoveType As MovementStatus, ByRef _LastMoveDateTime As DateTime, ByRef _LastMove As roMove, ByRef _PresenceMinutes As Integer) As PresenceStatus

        '    Dim oRet As PresenceStatus = PresenceStatus.Outside

        '    Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
        '    Dim oState As roWsState = oSession.States.EmployeeState

        '    WebServiceHelper.SetState(oState)

        '    Try

        '        Dim response As roGenericVtResponse(Of roMovePresenceStatus) = VTLiveApi.EmployeeMethods.GetPresenceStatus(_IDEmployee, _InputDateTime, _LastMoveType, _LastMoveDateTime, _LastMove, _PresenceMinutes, oState)

        '        _LastMoveType = response.Value.LastMoveType
        '        _LastMoveDateTime = response.Value.LastMoveDateTime
        '        _LastMove = VTLiveApi.EmployeeMethods.getmov(_IDEmployee, _InputDateTime, _LastMoveType, _LastMoveDateTime, _LastMove, _PresenceMinutes, oState)

        '        oSession.States.EmployeeState = response.Status
        '        roWsUserManagement.SessionObject = oSession

        '        If oState.Result <> EmployeeResultEnum.NoError Then
        '            ' Mostrar el error
        '            HelperWeb.ShowError(oPage, oState)
        '        End If

        '    Catch ex As Exception
        '        Dim oTmpState As New Robotics.Base.DTOs.roWsState
        '        oTmpState.Result = 1
        '        Dim oLanguage As New roLanguageWeb
        '        oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
        '        HelperWeb.ShowError(oPage, oTmpState, "9-BW01-315")
        '    End Try

        '    Return oRet

        'End Function

        Public Shared Function GetPresenceStatusEx(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _InputDateTime As DateTime, ByRef _LastPunchType As MovementStatus, ByRef _LastPunchDateTime As DateTime, ByRef _LastPunch As roPunch, ByRef _PresenceMinutes As Integer) As PresenceStatus

            Dim oRet As PresenceStatus = PresenceStatus.Outside

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roPunchPresenceStatus) = VTLiveApi.EmployeeMethods.GetPresenceStatusEx(_IDEmployee, _InputDateTime, _LastPunchType, _LastPunchDateTime, _LastPunch, _PresenceMinutes, oState)

                oRet = response.Value.PresenceStatus
                _LastPunchType = response.Value.LastMoveType
                _LastPunchDateTime = response.Value.LastMoveDateTime

                'TODO Recover Last punch
                _LastPunch = New roPunch()
                _LastPunch.ID = response.Value.LastPunchId
                _PresenceMinutes = response.Value.PresenceStatus

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-316")
            End Try

            Return oRet

        End Function

#End Region

#Region "Moves"

        Public Shared Function GetLastMove(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As roMove

            Dim oRet As roMove = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roMove) = VTLiveApi.EmployeeMethods.GetLastMove(IDEmployee, oState)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-317")
            End Try

            Return oRet

        End Function

        Public Shared Function GetLastPunch(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As roPunch

            Dim oRet As roPunch = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roPunch) = VTLiveApi.EmployeeMethods.GetLastPunchPres(IDEmployee, oState)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-318")
            End Try

            Return oRet

        End Function

        Public Shared Function GetIncompleteMoves(ByVal oPage As System.Web.UI.Page, ByVal _Begin As Date, ByVal _End As Date, Optional ByVal _IDGroup As Integer = -1, Optional ByVal _IDEmployees As String = "") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetIncompleteMoves(_Begin, _End, _IDGroup, _IDEmployees, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-319")
            End Try

            Return tb

        End Function

        Public Shared Function GetIncompleteDays(ByVal oPage As System.Web.UI.Page, ByVal _Begin As Date, ByVal _End As Date, Optional ByVal _IDGroup As Integer = -1, Optional ByVal _IDEmployees As String = "") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetIncompleteDays(_Begin, _End, _IDGroup, _IDEmployees, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-320")
            End Try

            Return tb

        End Function

        Public Shared Function GetSuspiciousMoves(ByVal oPage As System.Web.UI.Page, ByVal _Begin As Date, ByVal _End As Date, Optional ByVal _IDGroup As Integer = -1, Optional ByVal _IDEmployees As String = "") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetSuspiciousMoves(_Begin, _End, _IDGroup, _IDEmployees, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-321")
            End Try

            Return tb

        End Function

        Public Shared Function GetSuspiciousPunches(ByVal oPage As System.Web.UI.Page, ByVal _Begin As Date, ByVal _End As Date, Optional ByVal _IDGroup As Integer = -1, Optional ByVal _IDEmployees As String = "") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetSuspiciousPunches(_Begin, _End, _IDGroup, _IDEmployees, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-322")
            End Try

            Return tb

        End Function

#End Region

#Region "Incidences"

        Public Shared Function GetIncidences(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetIncidences(_IDEmployee, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-323")
            End Try

            Return tb

        End Function

        Public Shared Function GetMassIncidences(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployees As String, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal _IncidencesType As String, ByVal OnlyNotJustified As Boolean, ByVal _CentersFilter As String, ByVal _CausesFilter As String, ByVal _CausesValueFilter As String) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetMassIncidences(_IDEmployees, _BeginDate, _EndDate, _IncidencesType, OnlyNotJustified, _CentersFilter, _CausesFilter, _CausesValueFilter, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-324")
            End Try

            Return tb

        End Function

        Public Shared Function GetMassCenters(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployees As String, ByVal _BeginDate As Date, ByVal _EndDate As Date, ByVal strCausesWhere As String, strBusinessCenters As String, Optional directEmployeeIds As String = "-1", Optional directGroupIds As String = "-1") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetMassCenters(_IDEmployees, _BeginDate, _EndDate, strCausesWhere, oState, strBusinessCenters, directEmployeeIds, directGroupIds)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-325")
            End Try

            Return tb

        End Function

        Public Shared Function GetIncompleteIncidences(ByVal oPage As System.Web.UI.Page, ByVal _Begin As Date, ByVal _End As Date, Optional ByVal _IDGroup As Integer = -1, Optional ByVal _IDEmployees As String = "") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetIncompleteIncidences(_Begin, _End, _IDGroup, _IDEmployees, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-326")
            End Try

            Return tb

        End Function

        Public Shared Function SaveDailyCauses(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal tbIncidences As DataTable, ByVal bolUpdateStatus As Boolean,
                                               ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim ds As DataSet
                If tbIncidences.DataSet IsNot Nothing Then
                    ds = tbIncidences.DataSet
                Else
                    ds = New DataSet
                    ds.Tables.Add(tbIncidences)
                End If

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.SaveDailyCauses(_IDEmployee, _Date, ds, bolUpdateStatus, oState, bAudit)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Or Not bolRet Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-327")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Accruals"

        Public Shared Function GetDailyAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetDailyAccruals(_IDEmployee, _Date, filterBusinessGroups, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-328")
            End Try

            Return tb

        End Function

        Public Shared Function GetDailyTaskAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetDailyTaskAccruals(_IDEmployee, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-329")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados anuales de un empleado y una fecha.
        ''' Columnas: IDConcept, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_Date">Fecha actual</param>
        ''' <returns>Columnas: IDConcept, Name, Total, IDType, TotalFormat</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAnualAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetAnualAccruals(_IDEmployee, _Date, filterBusinessGroups, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-330")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados de tareas de un empleado y una fecha.
        ''' Columnas: IDTask, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_Date">Fecha actual</param>
        ''' <returns>Columnas: IDTask, Name, Total, IDType, TotalFormat</returns>
        ''' <remarks></remarks>
        Public Shared Function GetTaskAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetTaskAccruals(_IDEmployee, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-331")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados mensuales de un empleado y una fecha.
        ''' Columnas: IDConcept, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_Date">Fecha actual</param>
        ''' <returns>Columnas: IDConcept, Name, Total, IDType, TotalFormat</returns>
        ''' <remarks></remarks>
        Public Shared Function GetMonthAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetMonthAccruals(_IDEmployee, _Date, oState, filterBusinessGroups)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-332")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados semanales de un empleado y una fecha.
        ''' Columnas: IDConcept, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_Date">Fecha actual</param>
        ''' <returns>Columnas: IDConcept, Name, Total, IDType, TotalFormat</returns>
        ''' <remarks></remarks>
        Public Shared Function GetWeekAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetWeekAccruals(_IDEmployee, _Date, oState, filterBusinessGroups)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-333")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados por contrato de un empleado y una fecha.
        ''' Columnas: IDConcept, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_Date">Fecha actual</param>
        ''' <returns>Columnas: IDConcept, Name, Total, IDType, TotalFormat</returns>
        ''' <remarks></remarks>
        Public Shared Function GetContractAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetContractAccruals(_IDEmployee, _Date, oState, filterBusinessGroups)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-334")
            End Try

            Return tb

        End Function

        Public Shared Function GetAnnualWorkAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date, ByVal filterBusinessGroups As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetAnnualWorkAccruals(_IDEmployee, _Date, oState, filterBusinessGroups)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-349")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados mensuales de un empleado y una fecha.
        ''' Columnas: IDTask, Name, Total,  TotalFormat
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_Date">Fecha actual</param>
        ''' <returns>Columnas: IDTask, Name, Total, TotalFormat</returns>
        ''' <remarks></remarks>
        Public Shared Function GetMonthTaskAccruals(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _Date As Date) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetMonthTaskAccruals(_IDEmployee, _Date, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-335")
            End Try

            Return tb

        End Function

        ''' <summary>
        ''' Devuelve los acumulados mensuales y anuales de un empleado y una fecha.
        ''' Columnas: IDConcept, Name, Total, IDType, TotalFormat(valor total del acumulado formateado en función del tipo de acumulado), DefaultQuery (tipo de acumulado, Y:anual, M:Mensual)
        ''' </summary>
        ''' <param name="_IDEmployee">Código empleado</param>
        ''' <param name="_CurrentDate">Fecha actual</param>
        ''' <param name="_OnlyViewInTerminals">Mostrar sólo los acumulados que se tienen que mostrar en los terminales</param>
        ''' <returns>Columnas: IDConcept, Name, Total, IDType, TotalFormat, DefaultQuery</returns>
        ''' <remarks></remarks>
        Public Shared Function GetAccrualsQuery(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _CurrentDate As Date, ByVal _OnlyViewInTerminals As Boolean) As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetAccrualsQuery(_IDEmployee, _CurrentDate, _OnlyViewInTerminals, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError AndAlso response.Value IsNot Nothing Then
                    tb = response.Value.Tables(0)
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-336")
            End Try

            Return tb

        End Function

        Public Shared Function CopyAccrualRules(ByVal oPage As System.Web.UI.Page, ByVal intIDSourceEmployee As Integer, ByVal intIDDestinationEmployee As Integer, ByVal bolDescartarReglasActuales As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try
                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.CopyAccrualRules(intIDSourceEmployee, intIDDestinationEmployee, bolDescartarReglasActuales, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-337")
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyConceptAnnualLimits(ByVal oPage As System.Web.UI.Page, ByVal intIDSourceEmployee As Integer, ByVal intIDDestinationEmployee As Integer, ByVal bolDiscardAnnualLimits As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.CopyConceptAnnualLimits(intIDSourceEmployee, intIDDestinationEmployee, bolDiscardAnnualLimits, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-338")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Messges"

        Public Shared Function GetTerminalMessages(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetTerminalMessages(IDEmployee, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-339")
            End Try

            Return oRet
        End Function

        Public Shared Function DeleteTerminalMessages(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal Message As String, ByVal ID As Integer) As Boolean
            Dim oRet As Boolean = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.DeleteTerminalMessages(IDEmployee, Message, ID, oState)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If Not oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-340")
            End Try

            Return oRet
        End Function

#End Region

#Region "Querys"

        ''' <summary>
        ''' Resumen de vacaciones
        ''' </summary>
        ''' <param name="IDEmployee">ID de empleado a consultar</param>
        ''' <param name="xCurrentDate">Fecha actual a consultar</param>
        ''' <param name="intDone">Devuelve núm. de días ya disfrutados</param>
        ''' <param name="intPending">Devuelve número de días solicitados pendientes de procesa</param>
        ''' <param name="intLasting">Devuelve el número de días aprobados pendientes de disfrutar</param>
        ''' <param name="intDisponible">Devuelve los días disponibles de vacaciones</param>
        ''' <param name="xBeginPeriod">Opcional. Devuelve fecha inicial del periodo</param>
        ''' <param name="xEndPeriod">Opcional. Devuelve fecha final del periodo</param>
        ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function VacationsResumeQuery(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal IDShift As Integer, ByVal xCurrentDate As DateTime,
                                                    ByVal xVacationsDate As DateTime, ByRef intDone As Double, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double, ByRef intExpiredDays As Double, ByRef intDaysWithoutEnjoyment As Double,
                                                    Optional ByRef xBeginPeriod As DateTime = Nothing, Optional ByRef xEndPeriod As DateTime = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roHolidayResumeInfo) = VTLiveApi.EmployeeMethods.VacationsResumeQuery(IDEmployee, IDShift, xCurrentDate, xBeginPeriod, xEndPeriod, xVacationsDate, intDone, intPending, intLasting, intDisponible, intExpiredDays, intDaysWithoutEnjoyment, oState)

                intDone = response.Value.Done
                intPending = response.Value.Pending
                intLasting = response.Value.Lasting
                intDisponible = response.Value.Disponible
                xBeginPeriod = response.Value.BeginPeriod
                xEndPeriod = response.Value.EndPeriod
                intExpiredDays = response.Value.ExpiredDays
                intDaysWithoutEnjoyment = response.Value.DaysWithoutEnjoyment

                bolRet = (response.Status.Result = EmployeeResultEnum.NoError)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-341")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Resumen de vacaciones/permisos por horas
        ''' </summary>
        ''' <param name="IDEmployee">ID de empleado a consultar</param>
        ''' <param name="xCurrentDate">Fecha actual a consultar</param>
        ''' <param name="intPending">Devuelve número de días solicitados pendientes de procesa</param>
        ''' <param name="intLasting">Devuelve el número de días aprobados pendientes de disfrutar</param>
        ''' <param name="intDisponible">Devuelve los días disponibles de vacaciones</param>
        ''' <param name="xBeginPeriod">Opcional. Devuelve fecha inicial del periodo</param>
        ''' <param name="xEndPeriod">Opcional. Devuelve fecha final del periodo</param>
        ''' <returns>Devuelve TRUE si se realiza la función correctamente</returns>
        ''' <remarks></remarks>
        Public Shared Function ProgrammedHolidaysResumeQuery(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal IDCause As Integer, ByVal xCurrentDate As DateTime,
                                                    ByVal xProgrammedHolidaysDate As DateTime, ByRef intPending As Double, ByRef intLasting As Double, ByRef intDisponible As Double,
                                                    Optional ByRef xBeginPeriod As DateTime = Nothing, Optional ByRef xEndPeriod As DateTime = Nothing) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roHolidayResumeInfo) = VTLiveApi.EmployeeMethods.ProgrammedHolidaysResumeQuery(IDEmployee, IDCause, xCurrentDate, xBeginPeriod, xEndPeriod, xProgrammedHolidaysDate, intPending, intLasting, intDisponible, oState)

                intPending = response.Value.Pending
                intLasting = response.Value.Lasting
                intDisponible = response.Value.Disponible
                xBeginPeriod = response.Value.BeginPeriod
                xEndPeriod = response.Value.EndPeriod

                bolRet = (response.Status.Result = EmployeeResultEnum.NoError)
                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-342")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Employee status"

        Public Shared Function GetEmployeeStatus(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal bAudit As Boolean) As roEmployeeStatus

            Dim oRet As roEmployeeStatus = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of roEmployeeStatus) = VTLiveApi.EmployeeMethods.GetEmployeeStatus(_IDEmployee, oState, bAudit)
                oRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-343")
            End Try

            Return oRet

        End Function

#End Region

#Region "Coverages"

        ''' <summary>
        ''' Devuelve la lista de posibles empleados que pueden realizar una cobertura del empleado para una fecha en concreto.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee"></param>
        ''' <param name="_CoverageDate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetCoverageEmployees(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetCoverageEmployees(_IDEmployee, _CoverageDate, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-344")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Define una cobertura para un empleado y fecha en concreto.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployeeCoverage">Código del empleado que realizará la cobertura.</param>
        ''' <param name="_IDEmployeeCovered">Código del empleado a cubrir.</param>
        ''' <param name="_CoverageDate">Fecha de la cobertura</param>
        ''' <param name="_IDShift">Código del horario a utilizar para el empleado que realizará la cobertura. Si se quiere utilizar el que tiene actualmente planificado, indicar -1.</param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function AddEmployeeCoverage(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployeeCoverage As Integer, ByVal _IDEmployeeCovered As Integer, ByVal _CoverageDate As Date, ByRef _LockedDayActionEmployeeCovered As LockedDayAction, ByRef _LockedDayActionEmployeeCoverage As LockedDayAction, ByRef _IDEmployeeLocked As Integer, ByVal _IDShift As Integer, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.AddEmployeeCoverage(_IDEmployeeCoverage, _IDEmployeeCovered, _CoverageDate, _IDShift, _LockedDayActionEmployeeCovered, _LockedDayActionEmployeeCoverage, _IDEmployeeLocked, _Audit, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-345")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Elimina una cobertura para un empleado y fecha.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDEmployee">Código del empleado al que se le quiere eliminar la cobertura (puede ser tanto el empleado que realiza la cobertura cómo el empleado que se está cubriendo).</param>
        ''' <param name="_CoverageDate">Fecha de la cobertura</param>
        ''' <param name="_Audit"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function RemoveEmployeeCoverage(ByVal oPage As System.Web.UI.Page, ByVal _IDEmployee As Integer, ByVal _CoverageDate As Date, ByRef _LockedDayActionEmployeeCovered As LockedDayAction, ByRef _LockedDayActionEmployeeCoverage As LockedDayAction, ByRef _EmployeeType As Integer, ByRef _IDEmployeeLocked As Integer, ByVal _Audit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of Boolean) = VTLiveApi.EmployeeMethods.RemoveEmployeeCoverage(_IDEmployee, _CoverageDate, _LockedDayActionEmployeeCovered, _LockedDayActionEmployeeCoverage, _EmployeeType, _IDEmployeeLocked, _Audit, oState)
                bolRet = response.Value

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result <> EmployeeResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-346")
            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Devuelve la lista de posibles empleados que pueden realizar un puesto para poder planificar una dotación para un grupo, puesto y fecha concretos.
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="_IDGroup"></param>
        ''' <param name="_IDAssignment"></param>
        ''' <param name="_CoverageDate"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetDailyCoverageEmployees(ByVal oPage As System.Web.UI.Page, ByVal _IDGroup As Integer, ByVal _IDAssignment As Integer, ByVal _CoverageDate As Date) As DataTable
            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetDailyCoverageEmployees(_IDGroup, _IDAssignment, _CoverageDate, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-347")
            End Try

            Return oRet

        End Function

#End Region

#Region "Forecasts"

        Public Shared Function GetEmployeeForecastsInPeriod(ByVal oPage As System.Web.UI.Page, ByVal IDEmployee As Integer, ByVal _BeginDate As Date) As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeState

            WebServiceHelper.SetState(oState)

            Try

                Dim response As roGenericVtResponse(Of DataSet) = VTLiveApi.EmployeeMethods.GetEmployeeForecastsInPeriod(IDEmployee, _BeginDate, oState)

                oSession.States.EmployeeState = response.Status
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeState.Result = EmployeeResultEnum.NoError Then
                    If response.Value IsNot Nothing AndAlso response.Value.Tables.Count > 0 Then
                        oRet = response.Value.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-445")
            End Try

            Return oRet

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.EmployeeState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace