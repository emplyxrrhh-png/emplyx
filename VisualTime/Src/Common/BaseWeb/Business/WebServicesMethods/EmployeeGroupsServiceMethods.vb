Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Base.VTBusiness.GroupIndicator
Imports Robotics.Base.VTUserFields.UserFields

Namespace API

    Public NotInheritable Class EmployeeGroupsServiceMethods

        Public Enum eEmployeesFromGroup
            Current
            Future
            Old
        End Enum


        Public Shared Function GetEmployeesFromGroup(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer, ByVal EmployeeType As eEmployeesFromGroup, ByVal strFeature As String, Optional ByVal strFeatureType As String = "U", Optional ByVal includeExcludeWithoutContract As Boolean = False) As DataSet

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Dim oDataset As DataSet = Nothing

            Try

                Select Case EmployeeType
                    Case eEmployeesFromGroup.Current
                        oDataset = VTLiveApi.EmployeeGroupMethods.GetEmployeesFromGroup(IDGroup, strFeature, strFeatureType, includeExcludeWithoutContract, oState).Value
                    Case eEmployeesFromGroup.Future
                        oDataset = VTLiveApi.EmployeeGroupMethods.GetEmployeesInTransitToTheGroup(IDGroup, strFeature, strFeatureType, oState).Value
                    Case eEmployeesFromGroup.Old
                        oDataset = VTLiveApi.EmployeeGroupMethods.GetOldEmployeesFromGroup(IDGroup, strFeature, strFeatureType, includeExcludeWithoutContract, oState).Value
                End Select

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-206")
            End Try

            Return oDataset

        End Function

        Public Shared Function GetEmployeesFromGroupWithType(ByVal oPage As System.Web.UI.Page, ByVal intIDGroup As Integer, ByVal strFeature As String, Optional ByVal FieldWhere As String = "", Optional ByVal strFeatureType As String = "U") As DataTable

            Dim oRet As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim ds As DataSet = VTLiveApi.EmployeeGroupMethods.GetEmployeesFromGroupWithType(intIDGroup, strFeature, strFeatureType, FieldWhere, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        oRet = ds.Tables(0)
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-208")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeesFromGroupWithTypeArrayList(ByVal oPage As System.Web.UI.Page, ByVal intIDGroup As Integer, ByVal strFeature As String, Optional ByVal FieldWhere As String = "", Optional ByVal strFeatureType As String = "U") As ArrayList

            Dim lstRet As ArrayList = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim ds As DataSet = VTLiveApi.EmployeeGroupMethods.GetEmployeesFromGroupWithType(intIDGroup, strFeature, strFeatureType, FieldWhere, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
                    If ds.Tables.Count > 0 Then
                        lstRet = New ArrayList
                        For Each oRow As DataRow In ds.Tables(0).Rows
                            lstRet.Add(oRow("IDEmployee"))
                        Next
                    End If
                Else
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-209")
            End Try

            Return lstRet

        End Function

        Public Shared Function GetGroup(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer, ByVal bAudit As Boolean) As roGroup

            Dim oGroup As roGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                oGroup = VTLiveApi.EmployeeGroupMethods.GetGroup(IDGroup, oState, bAudit).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-210")
            End Try

            Return oGroup

        End Function

        Public Shared Function GetEmployeeListFromGroupRecursive(ByVal oPage As System.Web.UI.Page, ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                 ByVal strFilters As String, ByVal strFilterUserFields As String) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim oSelection() As Integer = VTLiveApi.EmployeeGroupMethods.GetEmployeeListFromGroupsRecursive(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState).Value.ToArray
                If oSelection IsNot Nothing Then oRet = oSelection.ToList()

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-212")
            End Try
            Return oRet

        End Function

        Public Shared Function GetEmployeeListFromGroupNORecursive(ByVal oPage As System.Web.UI.Page, ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                         ByVal strFilters As String, ByVal strFilterUserFields As String) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim oSelection() As Integer = VTLiveApi.EmployeeGroupMethods.GetEmployeeListFromGroupsNORecursive(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState).Value.ToArray
                If oSelection IsNot Nothing Then oRet = oSelection.ToList()

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-213")
            End Try
            Return oRet

        End Function

        Public Shared Function GetEmployeeListFromGroupRecursiveInDates(ByVal oPage As System.Web.UI.Page, ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                        ByVal strFilters As String, ByVal strFilterUserFields As String,
                                                                        ByVal DateInf As DateTime, ByVal DateSup As DateTime) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim oSelection() As Integer = VTLiveApi.EmployeeGroupMethods.GetEmployeeListFromGroupsRecursiveInDates(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState, DateInf, DateSup).Value.ToArray
                If oSelection IsNot Nothing Then oRet = oSelection.ToList()

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-214")
            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeListFromGroupNORecursiveInDates(ByVal oPage As System.Web.UI.Page, ByVal arrGroups() As Integer, ByVal Feature As String, ByVal Type As String,
                                                                          ByVal strFilters As String, ByVal strFilterUserFields As String,
                                                                          ByVal DateInf As DateTime, ByVal DateSup As DateTime) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim oSelection() As Integer = VTLiveApi.EmployeeGroupMethods.GetEmployeeListFromGroupsNORecursiveInDates(arrGroups, Feature, Type, strFilters, strFilterUserFields, oState, DateInf, DateSup).Value.ToArray
                If oSelection IsNot Nothing Then oRet = oSelection.ToList()

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-215")
            End Try

            Return oRet

        End Function

        Public Shared Function GetGroupByName(ByVal oPage As System.Web.UI.Page, ByVal strName As String) As roGroup

            Dim oGroup As roGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                oGroup = VTLiveApi.EmployeeGroupMethods.GetGroupByName(strName, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-216")
            End Try

            Return oGroup

        End Function

        Public Shared Function SaveGroup(ByVal oPage As System.Web.UI.Page, ByVal oGroup As roGroup, ByVal bAudit As Boolean) As Integer

            Dim intRet As Integer = -1

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try
                HelperSession.DeleteEmployeeGroupsFromApplication()
                Dim response As roGenericVtResponse(Of Integer) = VTLiveApi.EmployeeGroupMethods.SaveGroup(oGroup, oState, bAudit)
                intRet = response.Value

                oSession.States.EmployeeGroupState = response.Status
                roWsUserManagement.SessionObject = oSession

                If intRet <= 0 Then
                    If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                        HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-217")
            End Try

            Return intRet

        End Function

        Public Shared Function DeleteGroup(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                HelperSession.DeleteEmployeeGroupsFromApplication()
                Dim oResult = VTLiveApi.EmployeeGroupMethods.DeleteGroup(IDGroup, oState, bAudit)
                bolRet = oResult.Value

                oSession.States.EmployeeGroupState = oResult.Status
                roWsUserManagement.SessionObject = oSession

                If Not bolRet Then
                    If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                        HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                    End If
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-218")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetGroups(ByVal oPage As System.Web.UI.Page, ByVal strFeature As String, Optional ByVal strFeatureType As String = "U") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim ds As DataSet = VTLiveApi.EmployeeGroupMethods.GetGroups(strFeature, strFeatureType, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
                    If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                        tb = ds.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-219")
            End Try

            Return tb
        End Function

        ''' <summary>
        ''' Devuelve un sub-grupo de un grupo
        ''' </summary>
        ''' <param name="oPage">Pagina de llamada</param>
        ''' <param name="IDGroup">ID del grupo a recuperar sub-grupos</param>
        ''' <param name="strFeature">Permisos</param>
        ''' <param name="strFeatureType">U=Usuario, E=Empleado</param>
        ''' <returns>Tabla con las columnas: Groups.ID, Groups.Name, Groups.Path </returns>
        ''' <remarks></remarks>
        Public Shared Function GetChildGroups(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer, ByVal strFeature As String, Optional ByVal strFeatureType As String = "U") As DataTable

            Dim tb As DataTable = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim ds As DataSet = VTLiveApi.EmployeeGroupMethods.GetChildGroups(IDGroup, strFeature, strFeatureType, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result = GroupResultEnum.NoError Then
                    If ds IsNot Nothing AndAlso ds.Tables.Count > 0 Then
                        tb = ds.Tables(0)
                    End If
                Else
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-221")
            End Try

            Return tb

        End Function

        Public Shared Function GetGroupSelectionPath(ByVal oPage As System.Web.UI.Page, ByVal lstGroupsSelection As Generic.List(Of Integer), ByVal lstEmployeesSelection As Generic.List(Of Integer)) As Generic.List(Of Integer)

            Dim oRet As New Generic.List(Of Integer)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim oSelection() As Integer = VTLiveApi.EmployeeGroupMethods.GetGroupSelectionPath(lstGroupsSelection, lstEmployeesSelection, oState).Value.ToArray
                If oSelection IsNot Nothing Then oRet = oSelection.ToList()

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-222")
            End Try

            Return oRet

        End Function

        ''' <summary>
        ''' Obtiene una lista con los nodos de grupos y empleados para mostrar el árbol
        ''' </summary>
        ''' <param name="oPage"></param>
        ''' <param name="strFilterUserFields"></param>
        ''' <param name="strFeatureAlias"></param>
        ''' <param name="strFeatureType"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetTree(ByVal oPage As System.Web.UI.Page, ByVal strFilterUserFields As String, ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Generic.List(Of roGroupTree)

            Dim oRet As New Generic.List(Of roGroupTree)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            'VTLiveApi.EmployeeGroupMethods.Timeout = System.Threading.Timeout.Infinite
            'VTLiveApi.EmployeeGroupMethods.RequireMtom = True

            Try

                Dim oList() As roGroupTree = VTLiveApi.EmployeeGroupMethods.GetTree(strFilterUserFields, strFeatureAlias, strFeatureType, oState).Value.ToArray
                If oList IsNot Nothing Then oRet = oList.ToList()

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-223")
            End Try

            Return oRet

        End Function

        Public Shared Function GetGroupByName(ByVal oPage As System.Web.UI.Page, ByVal strName As String, ByVal intIDCompany As Integer) As roGroup

            Dim oGroup As roGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                oGroup = VTLiveApi.EmployeeGroupMethods.GetGroupByNameWithCompany(strName, intIDCompany, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-224")
            End Try

            Return oGroup

        End Function

        Public Shared Function GetCompanyByName(ByVal oPage As System.Web.UI.Page, ByVal strName As String) As roGroup

            Dim oGroup As roGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                oGroup = VTLiveApi.EmployeeGroupMethods.GetCompanyByName(strName, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-225")
            End Try

            Return oGroup

        End Function

        Public Shared Function GetGroupByNameInLevel(ByVal oPage As System.Web.UI.Page, ByVal strName As String, ByVal strPath As String) As roGroup

            Dim oGroup As roGroup = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                oGroup = VTLiveApi.EmployeeGroupMethods.GetGroupByNameInLevel(strName, strPath, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-226")
            End Try

            Return oGroup

        End Function

        Public Shared Function GetGroupZones(ByVal oPage As System.Web.UI.Page, ByVal IDZone As Integer, ByRef IDZoneWoringTime As Integer, ByRef IDZoneNonWoringTime As Integer) As Boolean

            Dim oRet As Boolean = False
            Dim resp As (Integer, Integer, Boolean)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                resp = VTLiveApi.EmployeeGroupMethods.GetGroupZones(IDZone, IDZoneWoringTime, IDZoneNonWoringTime, oState).Value

                IDZoneWoringTime = resp.Item1
                IDZoneNonWoringTime = resp.Item2
                oRet = resp.Item3

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-227")
            End Try

            Return oRet

        End Function

        Public Shared Function GetGroupCenters(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer, ByRef IDCenter As Integer) As Boolean

            Dim oRet As Boolean = False
            Dim resp As (Integer, Boolean)

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                resp = VTLiveApi.EmployeeGroupMethods.GetGroupCenters(IDGroup, IDCenter, oState).Value

                IDCenter = resp.Item1
                oRet = resp.Item2

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-228")
            End Try

            Return oRet

        End Function

#Region "UserFields"

        Public Shared Function GetUserFieldsDataset(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer) As DataSet

            Dim oDataset As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                oDataset = VTLiveApi.EmployeeGroupMethods.GetUserFieldsDataset(IDGroup, oState).Value

                oSession.States.UserFieldState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-229")
            End Try

            Return oDataset

        End Function

        Public Shared Function GetUserFields(ByVal oPage As PageBase, ByVal intIDGroup As Integer) As Generic.List(Of roGroupUserField)

            Dim oList As Generic.List(Of roGroupUserField) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                Dim arr As roGroupUserField() = VTLiveApi.EmployeeGroupMethods.GetUserFields(intIDGroup, oState).Value.ToArray

                oSession.States.UserFieldState = oState
                roWsUserManagement.SessionObject = oSession

                If arr IsNot Nothing Then
                    oList = arr.ToList()
                End If
                If oSession.States.UserFieldState.Result <> GroupResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-230")
            End Try

            Return oList

        End Function

        Public Shared Function SaveUserField(ByVal oPage As PageBase, ByVal IDGroup As Integer, ByVal strFieldName As String, ByVal strFieldValue As Object, ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                bolRet = VTLiveApi.EmployeeGroupMethods.SaveUserField(IDGroup, strFieldName, strFieldValue, oState, bAudit).Value

                oSession.States.UserFieldState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-231")
            End Try

            Return bolRet

        End Function

        Public Shared Function SaveUserFields(ByVal oPage As PageBase, ByVal IDGroup As Integer, ByVal _UserFields As Generic.List(Of roGroupUserField)) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.UserFieldState

            WebServiceHelper.SetState(oState)

            Try

                bolRet = VTLiveApi.EmployeeGroupMethods.SaveUserFields(IDGroup, _UserFields, oState).Value

                oSession.States.UserFieldState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.UserFieldState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.UserFieldState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-232")
            End Try

            Return bolRet

        End Function

#End Region

#Region "Indicators"

        Public Shared Function GetIndicatorsDataset(ByVal oPage As System.Web.UI.Page, ByVal IDGroup As Integer) As DataSet

            Dim oDataset As DataSet = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                oDataset = VTLiveApi.EmployeeGroupMethods.GetGroupIndicatorsDataset(IDGroup, oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-233")
            End Try

            Return oDataset

        End Function

        Public Shared Function GetIndicators(ByVal oPage As PageBase, ByVal intIDGroup As Integer) As Generic.List(Of roGroupIndicator)

            Dim oList As Generic.List(Of roGroupIndicator) = Nothing

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                Dim arr As roGroupIndicator() = VTLiveApi.EmployeeGroupMethods.GetGroupIndicators(intIDGroup, oState).Value.ToArray

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If arr IsNot Nothing Then
                    oList = arr.ToList()
                End If
                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    ' Mostrar el error
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-234")
            End Try

            Return oList

        End Function

        Public Shared Function SaveIndicators(ByVal oPage As PageBase, ByVal IDGroup As Integer, ByVal _IDsIndicators As Generic.List(Of Integer), ByVal bAudit As Boolean) As Boolean

            Dim bolRet As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                bolRet = VTLiveApi.EmployeeGroupMethods.SaveGroupIndicators(IDGroup, _IDsIndicators, oState, bAudit).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-235")
            End Try

            Return bolRet

        End Function

#End Region

#Region "SaaS Administration"

        Public Shared Function ActivateService(ByVal oPage As PageBase) As Boolean

            Dim bResult As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                bResult = VTLiveApi.EmployeeGroupMethods.ActivateService(oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-236")
            End Try

            Return bResult

        End Function

        Public Shared Function CancelService(ByVal oPage As PageBase) As Boolean

            Dim bResult As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                bResult = VTLiveApi.EmployeeGroupMethods.CancelService(oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-237")
            End Try
            Return bResult

        End Function

        Public Shared Function RegenerateAllPasswords(ByVal oPage As PageBase) As Boolean

            Dim bResult As Boolean

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                bResult = VTLiveApi.EmployeeGroupMethods.RegenerateAllPasswords(oState).Value

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-238")
            End Try

            Return bResult

        End Function

        Public Shared Function GetServiceStatus(ByRef serviceStatus As Boolean, ByVal oPage As PageBase) As Boolean

            Dim bResult As (Boolean, Boolean)
            Dim res As Boolean = False

            Dim oSession As roWsUserObject = roWsUserManagement.SessionObject()
            Dim oState As roWsState = oSession.States.EmployeeGroupState

            WebServiceHelper.SetState(oState)

            Try

                bResult = VTLiveApi.EmployeeGroupMethods.GetServiceStatus(serviceStatus, oState).Value

                serviceStatus = bResult.Item1
                res = bResult.Item2

                oSession.States.EmployeeGroupState = oState
                roWsUserManagement.SessionObject = oSession

                If oSession.States.EmployeeGroupState.Result <> GroupResultEnum.NoError Then
                    HelperWeb.ShowError(oPage, oSession.States.EmployeeGroupState)
                End If
            Catch ex As Exception
                Dim oTmpState As New Robotics.Base.DTOs.roWsState
                oTmpState.Result = 1
                Dim oLanguage As New roLanguageWeb
                oTmpState.ErrorText = oLanguage.TranslateFromWS("LivePortal", "Errors", "Webservices.NotAvailable") + System.Reflection.MethodInfo.GetCurrentMethod().Name.ToString()
                HelperWeb.ShowError(oPage, oTmpState, "9-BW01-239")
            End Try

            Return res

        End Function

#End Region

        Public Shared Function LastErrorText() As String
            Dim strRet As String = ""
            If roWsUserManagement.SessionObject() IsNot Nothing Then
                strRet = roWsUserManagement.SessionObject().States.EmployeeGroupState.ErrorText
            End If
            Return strRet
        End Function

    End Class

End Namespace