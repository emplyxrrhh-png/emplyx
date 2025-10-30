Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTRequests
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase

Namespace VTPortal

    Public Class UserFieldHelper

        Public Shared Function GetEmployeeUserFields(ByVal oPassportTicket As roPassportTicket, ByVal oEmpState As Employee.roEmployeeState) As UserFields
            Dim lrret As New UserFields
            Try
                Dim oReqState As New Requests.roRequestState(oPassportTicket.ID)
                Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState(oPassportTicket.ID)
                roBusinessState.CopyTo(oEmpState, oUserFieldState)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oPassportTicket, Nothing, oReqState)
                If oPermList.UserFieldQuery Then
                    Dim oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None} ' Permiso configurado sobre la información de la ficha para los distintos niveles de acceso del empleado actual ('Employees.UserFields.Information.Low', 'Employees.UserFields.Information.Medium', 'Employees.UserFields.Information.High')
                    Dim oQueryPermission As Permission = VTPortal.SecurityHelper.GetFeaturePermission(oPassportTicket.ID, "UserFields.Query", "E")
                    Dim oRequestsPermission As Permission = Permission.None
                    Dim oRequestTypeSecurity As New Requests.roRequestTypeSecurity(eRequestType.UserFieldsChange, New Requests.roRequestState(oPassportTicket.ID))
                    oRequestsPermission = VTPortal.SecurityHelper.GetFeaturePermission(oPassportTicket.ID, oRequestTypeSecurity.EmployeeFeatureName, "E")
                    If oQueryPermission >= Permission.Read OrElse oRequestsPermission >= Permission.Write Then
                        oUserFieldsAccessPermission(0) = oQueryPermission
                        oUserFieldsAccessPermission(1) = oQueryPermission
                        oUserFieldsAccessPermission(2) = oQueryPermission
                        If oQueryPermission >= Permission.Read Then
                            Dim dTbl As DataTable = VTUserFields.UserFields.roEmployeeUserField.GetUserFieldsDataTable(oPassportTicket.IDEmployee, Date.Now, oUserFieldState)
                            Dim Columns() As String = {"FieldCaption", "Value"}
                            lrret.Categories = VTPortal.UserFieldHelper.CreateUserFieldsAccordion(oPassportTicket.ID, dTbl, Columns, oUserFieldsAccessPermission, oRequestsPermission, oPassportTicket.Language.Key, oEmpState).ToArray()
                        End If
                        lrret.CanCreateRequest = (oRequestsPermission >= Permission.Write)
                        lrret.Status = ErrorCodes.OK
                    Else
                        lrret.Status = ErrorCodes.USER_FIELDS_ACCESS_DENIED
                    End If
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::UserFieldHelper::GetEmployeeUserFields")
            End Try

            Return lrret
        End Function

        Public Shared Function GetEmployeeUserFieldsAsSupervisor(ByVal oEmpPassport As roPassportTicket, ByVal oSupPassport As roPassportTicket, ByVal oEmpState As Employee.roEmployeeState) As UserFields
            Dim lrret As New UserFields
            Try
                Dim oReqState As New Requests.roRequestState(oEmpPassport.ID)
                Dim oUserFieldState As New VTUserFields.UserFields.roUserFieldState(oEmpPassport.ID)
                roBusinessState.CopyTo(oEmpState, oUserFieldState)
                Dim oPermList As PermissionList = SecurityHelper.GetEmployeePermissions(oEmpPassport, Nothing, oReqState)
                If oPermList.UserFieldQuery Then
                    Dim oUserFieldsAccessPermission() As Permission = {Permission.None, Permission.None, Permission.None} ' Permiso configurado sobre la información de la ficha para los distintos niveles de acceso del empleado actual ('Employees.UserFields.Information.Low', 'Employees.UserFields.Information.Medium', 'Employees.UserFields.Information.High')
                    Dim oRequestsPermission As Permission = Permission.None
                    Dim oRequestTypeSecurity As New Requests.roRequestTypeSecurity(eRequestType.UserFieldsChange, New Requests.roRequestState(oEmpPassport.ID))
                    oRequestsPermission = VTPortal.SecurityHelper.GetFeaturePermission(oEmpPassport.ID, oRequestTypeSecurity.EmployeeFeatureName, "E")

                    Dim oQueryPermission As Permission = VTPortal.SecurityHelper.GetFeaturePermission(oEmpPassport.ID, "UserFields.Query", "E")
                    oRequestsPermission = VTPortal.SecurityHelper.GetFeaturePermission(oEmpPassport.ID, oRequestTypeSecurity.EmployeeFeatureName, "E")
                    If oQueryPermission >= Permission.Read OrElse oRequestsPermission >= Permission.Write Then

                        oUserFieldsAccessPermission(0) = VTPortal.SecurityHelper.GetFeaturePermission(oSupPassport.ID, "Employees.UserFields.Information.Low", "U")
                        oUserFieldsAccessPermission(1) = VTPortal.SecurityHelper.GetFeaturePermission(oSupPassport.ID, "Employees.UserFields.Information.Medium", "U")
                        oUserFieldsAccessPermission(2) = VTPortal.SecurityHelper.GetFeaturePermission(oSupPassport.ID, "Employees.UserFields.Information.High", "U")

                        Dim dTbl As DataTable = VTUserFields.UserFields.roEmployeeUserField.GetUserFieldsDataTable(oEmpPassport.IDEmployee, Date.Now, oUserFieldState)
                        Dim Columns() As String = {"FieldCaption", "Value"}
                        lrret.Categories = VTPortal.UserFieldHelper.CreateUserFieldsAccordion(oEmpPassport.ID, dTbl, Columns, oUserFieldsAccessPermission, oRequestsPermission, oEmpPassport.Language.Key, oEmpState).ToArray()

                        lrret.CanCreateRequest = (oRequestsPermission >= Permission.Write)
                        lrret.Status = ErrorCodes.OK
                    Else
                        lrret.Status = ErrorCodes.USER_FIELDS_ACCESS_DENIED
                    End If
                Else
                    lrret.Status = ErrorCodes.GENERAL_ERROR_NoPermissions
                End If
            Catch ex As Exception
                lrret.Status = ErrorCodes.GENERAL_ERROR

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::UserFieldHelper::GetEmployeeUserFields")
            End Try

            Return lrret
        End Function

        Public Shared Function CreateUserFieldsAccordion(ByVal idPassport As Integer, ByVal dTable As DataTable, ByVal ColumnNames() As String, ByVal oUserFieldsAccessPermission() As Permission, ByVal oRequestsPermission As Permission, ByVal oLangKey As String, ByVal oEmpState As Employee.roEmployeeState) As Generic.List(Of Category)
            Dim jsCategories As New Generic.List(Of Category)

            Try
                Dim Categories() As String = VTUserFields.UserFields.roUserField.GetCategories(True, New VTUserFields.UserFields.roUserFieldState(idPassport)).ToArray
                If Categories IsNot Nothing Then

                    Dim oLang As New roLanguage()
                    oLang.SetLanguageReference("LiveOne", oLangKey)

                    ReDim Preserve Categories(Categories.Length)
                    Categories(Categories.Length - 1) = ""

                    Dim Rows() As DataRow
                    Dim bolEditable As Boolean = False

                    Dim intCountHigh As Integer = 0

                    For Each strCategory As String In Categories
                        Dim curCategory As New Category
                        ' Obtenemos los campos correspondientes a la categoría
                        Rows = dTable.Select("Category = '" & strCategory.Replace("'", "''") & "'" & IIf(strCategory = "", " OR Category IS NULL", ""), "FieldCaption")

                        If Rows.Length > 0 Then

                            Dim catUserFields As New Generic.List(Of EmployeeUserField)

                            If strCategory <> "" Then
                                curCategory.key = strCategory
                            Else
                                curCategory.key = oLang.Translate("UserFields.UserField.Category.None", "UserFields")
                            End If

                            ' Bucle por los campos de la categoría actual
                            For Each oRow As DataRow In Rows
                                Dim curUserField As New EmployeeUserField

                                curUserField.Name = oRow("FieldName").ToString
                                curUserField.Caption = oRow("FieldCaption").ToString
                                curUserField.Description = roTypes.Any2String(oRow("Description"))
                                curUserField.Value = oRow("Value").ToString
                                curUserField.ValueDate = CDate(oRow("Date").ToString)
                                curUserField.Type = oRow("Type")

                                Dim strBegin As String = ""
                                Dim strEnd As String = ""

                                curUserField.CanDeliverDocument = True

                                Select Case oRow("Type")
                                    Case FieldTypes.tDate
                                        If oRow("ValueDateTime").ToString <> "" Then
                                            curUserField.Value = Format(CDate(oRow("ValueDateTime").ToString), System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)
                                            curUserField.TypeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern
                                        End If
                                    Case FieldTypes.tTime
                                        If oRow("ValueDateTime").ToString <> "" Then
                                            curUserField.Value = Format(CDate(oRow("ValueDateTime").ToString), "HH:mm")
                                            curUserField.TypeFormat = "HH:mm"
                                        End If
                                    Case FieldTypes.tDatePeriod
                                        If curUserField.Value.Split("*")(0) <> "" AndAlso curUserField.Value.Split("*")(0).Length = 10 Then
                                            Dim xDate As New Date(curUserField.Value.Split("*")(0).Substring(0, 4), curUserField.Value.Split("*")(0).Substring(5, 2), curUserField.Value.Split("*")(0).Substring(8, 2))
                                            strBegin = Format(xDate, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)
                                        End If

                                        If curUserField.Value.Split("*").Length > 1 AndAlso curUserField.Value.Split("*")(1).Length = 10 Then
                                            Dim xDate As New Date(curUserField.Value.Split("*")(1).Substring(0, 4), curUserField.Value.Split("*")(1).Substring(5, 2), curUserField.Value.Split("*")(1).Substring(8, 2))
                                            strEnd = Format(xDate, System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern)
                                        End If
                                        curUserField.TypeFormat = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern

                                        Dim strLanguageKey As String
                                        If strBegin <> "" Or strEnd <> "" Then
                                            oLang.ClearUserTokens()

                                            If strBegin <> "" Then oLang.AddUserToken(strBegin)
                                            If strEnd <> "" Then oLang.AddUserToken(strEnd)
                                            strLanguageKey = "UserFields.DatePeriod.FieldCaption"
                                            If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                            If strEnd = "" Then strLanguageKey &= ".BeginOnly"

                                            curUserField.Value = oLang.Translate(strLanguageKey, "UserFields")
                                        Else
                                            curUserField.Value = ""
                                        End If

                                    Case FieldTypes.tTimePeriod
                                        If curUserField.Value.Split("*")(0) <> "" AndAlso curUserField.Value.Split("*")(0).Length = 5 Then
                                            Dim xDate As New Date(1900, 1, 1, curUserField.Value.Split("*")(0).Substring(0, 2), curUserField.Value.Split("*")(0).Substring(3, 2), 0)
                                            strBegin = Format(xDate, "HH:mm")
                                        End If
                                        If curUserField.Value.Split("*").Length > 1 AndAlso curUserField.Value.Split("*")(1).Length = 5 Then
                                            Dim xDate As New Date(1900, 1, 1, curUserField.Value.Split("*")(1).Substring(0, 2), curUserField.Value.Split("*")(1).Substring(3, 2), 0)
                                            strEnd = Format(xDate, "HH:mm")
                                        End If
                                        curUserField.TypeFormat = "HH:mm"

                                        Dim strLanguageKey As String
                                        If strBegin <> "" Or strEnd <> "" Then
                                            oLang.ClearUserTokens()
                                            Dim oParams As New Generic.List(Of String) 'ArrayList
                                            If strBegin <> "" Then oParams.Add(strBegin)
                                            If strEnd <> "" Then oParams.Add(strEnd)
                                            strLanguageKey = "UserFields.TimePeriod.FieldCaption"
                                            If strBegin = "" Then strLanguageKey &= ".EndOnly"
                                            If strEnd = "" Then strLanguageKey &= ".BeginOnly"
                                            curUserField.Value = oLang.Translate(strLanguageKey, "UserFields")
                                        Else
                                            curUserField.Value = ""
                                        End If
                                    Case FieldTypes.tDocument
                                        Dim oDocState As New VTDocuments.roDocumentState(-1)
                                        roBusinessState.CopyTo(oEmpState, oDocState)
                                        Dim oManager As New VTDocuments.roDocumentManager(oDocState)

                                        Dim oTmpManager As New VTDocuments.roDocumentManager(New VTDocuments.roDocumentState(-1))
                                        Dim oDocTemplate As roDocumentTemplate = oTmpManager.GetTemplateDocuments(True, DocumentScope.EmployeeField).Find(Function(x) x.Name = oRow("FieldName").ToString)

                                        If (oDocTemplate Is Nothing OrElse (oDocTemplate IsNot Nothing AndAlso Not oDocTemplate.EmployeeDeliverAllowed)) Then
                                            curUserField.CanDeliverDocument = False
                                        End If

                                        Dim oDocument As roDocument = oManager.LoadDocument(roTypes.Any2Integer(curUserField.Value))

                                        If oDocument.Id <> -1 Then
                                            curUserField.Value = oDocument.Title & oDocument.DocumentType
                                        Else
                                            curUserField.Value = String.Empty
                                        End If
                                    Case Else
                                End Select

                                curUserField.AccessLevel = oRow("AccessLevel")
                                curUserField.HasHistory = roTypes.Any2Boolean(oRow("History"))

                                Dim bAdd As Boolean = False
                                If curUserField.AccessLevel = 0 AndAlso oUserFieldsAccessPermission(0) > Permission.None Then
                                    bAdd = True
                                ElseIf curUserField.AccessLevel = 1 AndAlso oUserFieldsAccessPermission(1) > Permission.None Then
                                    bAdd = True
                                ElseIf curUserField.AccessLevel = 2 AndAlso oUserFieldsAccessPermission(2) > Permission.None Then
                                    bAdd = True
                                End If

                                If bAdd Then catUserFields.Add(curUserField)
                            Next

                            curCategory.items = catUserFields.ToArray()
                            If curCategory.items.Length > 0 Then jsCategories.Add(curCategory)
                        End If

                    Next

                End If
            Catch ex As Exception
                jsCategories = New Generic.List(Of Category)

                Dim oLogState As New roBusinessState("Common.BaseState", "")
                oLogState.UpdateStateInfo(ex, "VTPortal::UserFieldHelper::CreateUserFieldsAccordion")
            End Try

            Return jsCategories

        End Function

    End Class

End Namespace