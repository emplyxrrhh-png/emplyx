Imports Robotics.DataLayer
Imports Robotics.Base.DTOs
Imports Robotics.Base.DTOs.UserFieldsTypes
Imports Robotics.Base.VTBusiness
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Base.VTEmployees
Imports Robotics.Base.VTUserFields
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.roTypes
Imports Robotics.Base.VTBusiness.Group
Imports Robotics.Security.Base
Imports Robotics.Base.VTSelectorManager

Namespace DataLink



    Public Class roApiEmployees
        Inherits roDataLinkApi

        Private _IncludeEmployees As Boolean = False

        Protected ReadOnly Property ImportEngine As roEmployeeImport
            Get
                Return CType(Me.oDataImport, roEmployeeImport)
            End Get
        End Property


        Public Sub New(Optional ByVal state As roDataLinkState = Nothing)
            MyBase.New(state)

            If Me.oDataImport Is Nothing Then
                Me.oDataImport = New roEmployeeImport(DataLink.eImportType.IsCustom, "", Me.State)
            End If
        End Sub

        Public Function GetEmployees(ByRef oEmployees As Generic.List(Of RoboticsExternAccess.roEmployee), ByVal onlyWithActiveContract As Boolean, ByVal IncludeOldData As Boolean, ByVal employeeID As String, ByVal FieldName As String, ByVal FieldValue As String, ByRef strErrorMsg As String, ByRef iReturnCode As RoboticsExternAccess.Core.DTOs.ReturnCode, Optional ByVal Timestamp As DateTime? = Nothing) As Boolean
            Dim bolRet As Boolean = False

            Try
                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError

                ' Obtenemos el campo identificador del empleado
                Dim strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"

                oEmployees = New Generic.List(Of RoboticsExternAccess.roEmployee)

                Dim strDateEnd As String = Format$(Now.Date, "yyyyMMdd")
                Dim strSQL As String = "@SELECT# ID, Name," & " isnull((@SELECT# VALUE from GetAllEmployeeUserFieldValue('" & strImportPrimaryKeyUserField & "','" & strDateEnd & "') WHERE idEmployee=Employees.ID ),'') as EmployeeKey "
                strSQL += ", (@SELECT# count(*) FROM EmployeeContracts WHERE (BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND (EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) and EmployeeContracts.IDEmployee = employees.id) ActiveContract  FROM Employees "

                ' Filtrado por campo de la ficha
                Dim strFilterUser As String = String.Empty
                Try
                    If FieldName.Trim.Length > 0 And FieldValue.Trim.Length > 0 Then
                        strFilterUser = "Usr_" & FieldName.Trim & "|(0)~=~(" & FieldValue.Trim & ")~" & ChrW(127) & ChrW(127) & ChrW(127) & ChrW(127) & ChrW(127)
                        strFilterUser = roSelectorManager.BuildUserFieldsWhere(strFilterUser)
                    End If
                Catch ex As Exception
                    strFilterUser = ""
                End Try
                If strFilterUser.Length > 0 Then strSQL += " WHERE " & strFilterUser

                ' Filtrado por timestamp
                Try
                    If Timestamp IsNot Nothing Then
                        If strFilterUser.Length > 0 Then
                            strSQL += " AND TimeStamp >=" & Any2Time(Timestamp).SQLDateTime
                        Else
                            strSQL += " WHERE TimeStamp >=" & Any2Time(Timestamp).SQLDateTime
                        End If
                    End If
                Catch ex As Exception
                    strFilterUser = ""
                End Try

                ' Cargamos lista de empleados
                Dim oEmployeesDT As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                If oEmployeesDT IsNot Nothing AndAlso oEmployeesDT.Rows.Count > 0 Then
                    ' Si se filtra por un empleado concreto
                    If employeeID.Length > 0 Then
                        Dim tbaux = oEmployeesDT.AsEnumerable().Where(Function(x) x("EmployeeKey") = employeeID)
                        If tbaux IsNot Nothing AndAlso tbaux.Count > 0 Then
                            oEmployeesDT = tbaux.CopyToDataTable()
                        Else
                            oEmployeesDT = New DataTable
                        End If
                    End If

                    ' Solo empleados activos
                    If onlyWithActiveContract Then
                        Dim tbaux = oEmployeesDT.AsEnumerable().Where(Function(x) x("ActiveContract") > 0)
                        If tbaux IsNot Nothing AndAlso tbaux.Count > 0 Then
                            oEmployeesDT = tbaux.CopyToDataTable()
                        Else
                            oEmployeesDT = New DataTable
                        End If
                    End If

                    For Each oEmployeesRow As DataRow In oEmployeesDT.Rows
                        Dim oEmployee As New RoboticsExternAccess.roEmployee
                        oEmployee.Name = oEmployeesRow("Name")
                        oEmployee.ID = oEmployeesRow("EmployeeKey")

                        ' Lista de contratos
                        oEmployee.Contracts = {}
                        Dim oContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(oEmployeesRow("ID"), New Contract.roContractState(-1))
                        If oContracts IsNot Nothing Then
                            oEmployee.Contracts = oContracts.Select().ToList.ConvertAll(AddressOf XContractConverter).ToArray
                            If Not IncludeOldData Then
                                ' Sin historico
                                Dim tbaux = New Generic.List(Of RoboticsExternAccess.roContract)
                                tbaux.Add(oEmployee.Contracts.Last)
                                oEmployee.Contracts = tbaux.ToArray
                            End If
                        End If

                        ' Lista de movilidades
                        oEmployee.Mobilities = {}
                        Dim oMobilities As DataTable = Employee.roMobility.GetMobilities(oEmployeesRow("ID"), New Employee.roEmployeeState(-1))
                        If oMobilities IsNot Nothing Then
                            oEmployee.Mobilities = oMobilities.Select().ToList.ConvertAll(AddressOf XMobilityConverter).ToArray
                            If Not IncludeOldData Then
                                ' Sin historico
                                Dim tbaux = New Generic.List(Of RoboticsExternAccess.roMobility)
                                tbaux.Add(oEmployee.Mobilities.Last)
                                oEmployee.Mobilities = tbaux.ToArray
                            End If
                        End If

                        ' Ficha
                        oEmployee.Fields = {}
                        Dim oFields As DataTable = Nothing
                        If Not IncludeOldData Then
                            ' Sin historico
                            oFields = CreateDataTable("@SELECT#  FieldName, Date, Value from dbo.sysrovwEmployeeeUserFieldCurrentValues where IDEmployee= " & oEmployeesRow("ID").ToString & "  ORDER BY FieldName")
                        Else
                            oFields = CreateDataTable("@SELECT#  FieldName, Date, Value FROM EmployeeUserFieldValues WHERE IDEmployee = " & oEmployeesRow("ID").ToString & "  ORDER BY FieldName, Date")
                        End If

                        If oFields IsNot Nothing Then
                            oEmployee.Fields = oFields.Select().ToList.ConvertAll(AddressOf XEmployeeFieldConverter).ToArray
                        End If

                        ' Metodos de autentificaciçon
                        oEmployee.AuthenticationMethods = {}
                        Dim oPassport As roPassport = roPassportManager.GetPassport(oEmployeesRow("ID"), LoadType.Employee, New roSecurityState(-1))
                        If oPassport IsNot Nothing AndAlso oPassport.AuthenticationMethods IsNot Nothing Then
                            Dim oAuthentications As New Generic.List(Of RoboticsExternAccess.roAuthentication)
                            If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Length > 0 AndAlso
                                    oPassport.AuthenticationMethods.CardRows(0).Enabled Then
                                Dim _Card As String = oPassport.AuthenticationMethods.CardRows(0).Credential.ToString
                                If _Card <> "0" AndAlso _Card.Length > 0 Then
                                    Dim oAuthentication As New RoboticsExternAccess.roAuthentication
                                    oAuthentication.Method = AuthenticationMethod.Card
                                    oAuthentication.Credential = _Card
                                    oAuthentications.Add(oAuthentication)
                                End If
                            End If
                            If oPassport.AuthenticationMethods.PasswordRow IsNot Nothing AndAlso oPassport.AuthenticationMethods.PasswordRow.Credential.Length > 0 Then
                                Dim oAuthentication As New RoboticsExternAccess.roAuthentication
                                oAuthentication.Method = AuthenticationMethod.Password
                                oAuthentication.Credential = oPassport.AuthenticationMethods.PasswordRow.Credential
                                oAuthentications.Add(oAuthentication)
                            End If
                            If oPassport.AuthenticationMethods.PlateRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.PlateRows.Length > 0 Then
                                For i As Integer = 0 To oPassport.AuthenticationMethods.PlateRows.Length - 1
                                    Dim oAuthentication As New RoboticsExternAccess.roAuthentication
                                    oAuthentication.Method = AuthenticationMethod.Plate
                                    oAuthentication.Credential = oPassport.AuthenticationMethods.PlateRows(i).Credential
                                    oAuthentications.Add(oAuthentication)
                                Next
                            End If
                            If oAuthentications.Count > 0 Then oEmployee.AuthenticationMethods = oAuthentications.ToArray

                        End If

                        oEmployees.Add(oEmployee)
                    Next
                End If

                iReturnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK
                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetEmployees")
                bolRet = False
            End Try

            Return bolRet
        End Function

        Public Function GetEmployeeById(ByVal employeeID As String, ByRef strErrorMsg As String) As RoboticsExternAccess.roEmployee
            Dim oEmployee As RoboticsExternAccess.roEmployee = Nothing

            Try
                ' Obtenemos el campo identificador del empleado
                Dim strImportPrimaryKeyUserField = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"

                Dim strDateEnd As String = Format$(Now.Date, "yyyyMMdd")
                Dim strSQL As String = "@SELECT# ID, Name," & " isnull((@SELECT# VALUE from GetAllEmployeeUserFieldValue('" & strImportPrimaryKeyUserField & "','" & strDateEnd & "') WHERE idEmployee=Employees.ID ),'') as EmployeeKey "
                strSQL += $", (@SELECT# count(*) FROM EmployeeContracts WHERE (BeginDate <= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) AND (EndDate >= CONVERT(smalldatetime, CONVERT(nvarchar, GETDATE(), 102), 102)) and EmployeeContracts.IDEmployee = employees.id) ActiveContract  FROM Employees {DataLayer.SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetEmployeeInfo)}"

                ' Cargamos lista de empleados
                Dim oEmployeesDT As DataTable = CreateDataTableWithoutTimeouts(strSQL)

                If oEmployeesDT IsNot Nothing AndAlso oEmployeesDT.Rows.Count > 0 Then
                    ' Si se filtra por un empleado concreto
                    If employeeID.Length > 0 Then
                        Dim tbaux = oEmployeesDT.AsEnumerable().Where(Function(x) x("ID") = employeeID)
                        If tbaux IsNot Nothing AndAlso tbaux.Any() Then
                            oEmployeesDT = tbaux.CopyToDataTable()
                        Else
                            oEmployeesDT = New DataTable
                        End If
                    End If

                    For Each oEmployeesRow As DataRow In oEmployeesDT.Rows
                        oEmployee = New RoboticsExternAccess.roEmployee
                        oEmployee.Name = oEmployeesRow("Name")
                        oEmployee.ID = oEmployeesRow("EmployeeKey")

                        ' Lista de contratos
                        oEmployee.Contracts = {}
                        Dim oContracts As DataTable = Contract.roContract.GetContractsByIDEmployee(oEmployeesRow("ID"), New Contract.roContractState(-1))
                        If oContracts IsNot Nothing Then
                            oEmployee.Contracts = oContracts.Select().ToList.ConvertAll(AddressOf XContractConverter).ToArray
                        End If

                        ' Lista de movilidades
                        oEmployee.Mobilities = {}
                        Dim oMobilities As DataTable = Employee.roMobility.GetMobilities(oEmployeesRow("ID"), New Employee.roEmployeeState(-1))
                        If oMobilities IsNot Nothing Then
                            oEmployee.Mobilities = oMobilities.Select().ToList.ConvertAll(AddressOf XMobilityConverter).ToArray
                        End If

                        ' Ficha
                        oEmployee.Fields = {}
                        Dim oFields As DataTable = Nothing
                        oFields = CreateDataTable("@SELECT#  FieldName, Date, Value FROM EmployeeUserFieldValues WHERE IDEmployee = " & oEmployeesRow("ID").ToString & "  ORDER BY FieldName, Date")

                        If oFields IsNot Nothing Then
                            oEmployee.Fields = oFields.Select().ToList.ConvertAll(AddressOf XEmployeeFieldConverter).ToArray
                        End If

                        ' Metodos de autentificaciçon
                        oEmployee.AuthenticationMethods = {}
                        Dim oPassport As roPassport = roPassportManager.GetPassport(oEmployeesRow("ID"), LoadType.Employee, New roSecurityState(-1))
                        If oPassport IsNot Nothing AndAlso oPassport.AuthenticationMethods IsNot Nothing Then
                            Dim oAuthentications As New Generic.List(Of RoboticsExternAccess.roAuthentication)
                            If oPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows.Length > 0 AndAlso oPassport.AuthenticationMethods.CardRows(0).Enabled Then
                                Dim _Card As String = oPassport.AuthenticationMethods.CardRows(0).Credential.ToString
                                If _Card <> "0" AndAlso _Card.Length > 0 Then oAuthentications.Add(New RoboticsExternAccess.roAuthentication With {.Method = AuthenticationMethod.Card, .Credential = _Card})
                            End If

                            If oPassport.AuthenticationMethods.PasswordRow IsNot Nothing AndAlso oPassport.AuthenticationMethods.PasswordRow.Credential.Length > 0 Then oAuthentications.Add(New roAuthentication() With {.Method = AuthenticationMethod.Password, .Credential = oPassport.AuthenticationMethods.PasswordRow.Credential})

                            If oAuthentications.Count > 0 Then oEmployee.AuthenticationMethods = oAuthentications.ToArray

                        End If
                    Next
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetEmployees")
                oEmployee = Nothing
            End Try

            Return oEmployee
        End Function

        Public Function CreateOrUpdateEmployee(ByVal oEmployeeData As RoboticsExternAccess.IDatalinkEmployee, ByRef strErrorMsg As String, ByRef iNewEmployeeId As Integer,
                                       Optional ByVal bCallBroadcaster As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim ColumnsVal As String() = {}
                Dim ColumnsUsrName As String() = {}
                Dim ColumnsPos As Integer() = {}

                Dim oEmployee As roDatalinkStandarEmployee = CType(oEmployeeData, roDatalinkStandarEmployee)
                Dim bProtectDataOnContractUpdate As Boolean = False
                If oEmployee.Protection IsNot Nothing Then
                    bProtectDataOnContractUpdate = oEmployee.Protection.ContractData
                End If

                bolRet = oEmployeeData.GetEmployeeColumnsDefinition(ColumnsVal, ColumnsUsrName, ColumnsPos)

                If bolRet Then
                    Dim oEmployeeState As New Employee.roEmployeeState
                    roBusinessState.CopyTo(Me.State, oEmployeeState)

                    Dim oUserFieldState As New UserFields.roUserFieldState()
                    roBusinessState.CopyTo(Me.State, oUserFieldState)

                    Dim tbUserFieldList As DataTable = UserFields.roUserField.GetUserFields(Types.EmployeeField, oUserFieldState)
                    Me.ImportEngine.createMandatoryFields(oEmployeeState, oUserFieldState, False)
                    Me.ImportEngine.createExcelUserFields(ColumnsVal, ColumnsUsrName, oEmployeeState, oUserFieldState, False)

                    Dim newCompanyIds As New Generic.List(Of Integer)

                    ' Grupos de usuario
                    Dim dtUserGroups As DataTable = Me.ImportEngine.GetUserGroups()

                    ' Convenios
                    Dim dtLabAgrees As DataTable = Me.ImportEngine.GetLabAgrees()

                    Dim intIDEmployee As Integer = Me.ImportEngine.isEmployeeNew(ColumnsVal(ColumnsPos(RoboticsExternAccess.EmployeeColumns.ImportPrimaryKey)), ColumnsVal(ColumnsPos(RoboticsExternAccess.EmployeeColumns.DNI)), oUserFieldState)

                    If intIDEmployee < 0 Then
                        bolRet = Me.ImportEngine.NewEmployee(ColumnsVal, ColumnsUsrName, tbUserFieldList, newCompanyIds, dtUserGroups, dtLabAgrees, strErrorMsg, bCallBroadcaster, False)
                        iNewEmployeeId = roTypes.Any2Integer(ExecuteScalar("@SELECT# top 1 ID from Employees order by ID desc"))
                    Else
                        bolRet = Me.ImportEngine.UpdateEmployee(intIDEmployee, ColumnsVal, ColumnsUsrName, ColumnsPos, tbUserFieldList, newCompanyIds, dtUserGroups, dtLabAgrees, strErrorMsg, bCallBroadcaster, False, If(oEmployee.Protection Is Nothing, False, oEmployee.Protection.ContractData))
                        iNewEmployeeId = intIDEmployee
                    End If
                    Me.State.Result = Me.ImportEngine.State.Result
                Else
                    Me.State.Result = DataLinkResultEnum.FormatColumnIsWrong
                    strErrorMsg = "Invalid employee object"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateEmployee")
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function CreateOrUpdateEmployeePhoto(ByVal oEmployeePhoto As RoboticsExternAccess.roDatalinkStandardPhoto, ByVal UserName As String, ByRef strErrorMsg As String) As Boolean
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Dim oEmployeeState As New Employee.roEmployeeState(Me.State.IDPassport, Me.State.Context, Me.State.ClientAddress)

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Me.State.Result = DataLinkResultEnum.Exception

                ' El empleado destino debe existir
                Dim oUserFieldState As New UserFields.roUserFieldState()
                Dim intIDEmployee As Integer = Me.ImportEngine.isEmployeeNew(oEmployeePhoto.UniqueEmployeeID, oEmployeePhoto.NifEmpleado, oUserFieldState)

                If intIDEmployee > 0 Then
                    ' La foto debe contener datos
                    If oEmployeePhoto.PhotoData.Length > 0 Then

                        Dim bValidFile As Boolean = True
                        Try
                            Dim employeePhoto As Byte() = Convert.FromBase64String(oEmployeePhoto.PhotoData)
                            Dim oEmployee As Employee.roEmployee = Employee.roEmployee.GetEmployee(intIDEmployee, oEmployeeState, False)
                            oEmployee.Image = roImagesHelper.ResizeImageIfNeeded(employeePhoto, 200, 200)
                            bolRet = Employee.roEmployee.SaveEmployee(oEmployee, oEmployeeState, False, True)
                        Catch ex As Exception
                            bValidFile = False
                        End Try
                        If Not bValidFile Then
                            Me.State.Result = DataLinkResultEnum.InvalidPhotoData
                            strErrorMsg = "Photo format not supported"
                        End If
                    Else
                        Me.State.Result = DataLinkResultEnum.InvalidPhotoData
                        strErrorMsg = "Empty photo"
                    End If
                Else
                    Me.State.Result = DataLinkResultEnum.InvalidEmployee
                    strErrorMsg = "No such employee"
                End If
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkImport::CreateOrUpdateEmployeePhoto")
                Me.State.Result = DataLinkResultEnum.Exception
                strErrorMsg = "Exception: " + ex.Message
            Finally

                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet
        End Function

        Public Function ImportEmployeesSAGE200c_ResetContractsAndFixMobilitiesIfNeeded(ByVal sEmployeeNIF As String, ByVal sUniqueIDEmployee As String, lContracts As List(Of RoboticsExternAccess.roDatalinkEmployeeContract)) As Boolean
            Return Me.ImportEngine.ImportEmployeesSAGE200c_ResetContractsAndFixMobilitiesIfNeeded(sEmployeeNIF, sUniqueIDEmployee, lContracts)
        End Function



        ''' <summary>
        ''' Función para obtener la estructura de departamentos desde Servicios REST
        ''' </summary>
        ''' <param name="oEmployees"></param>
        ''' <param name="strErrorMsg"></param>
        ''' <param name="iReturnCode"></param>
        ''' <param name="oCn"></param>
        ''' <returns></returns>
        Public Function GetGroups(ByRef oGroups As List(Of RoboticsExternAccess.roGroup), ByVal bIncludeEmployees As Boolean, ByVal sRootCode As String, ByRef strErrorMsg As String, ByRef returnCode As RoboticsExternAccess.Core.DTOs.ReturnCode, Optional ByVal GroupID As String = "") As Boolean
            Dim bolRet As Boolean = False

            Try
                _IncludeEmployees = bIncludeEmployees

                Dim oGroupTree As New List(Of roGroupTree)
                oGroupTree = roGroupTreeManager.GetTree("", "", "U", New Group.roGroupState(Me.State.IDPassport), sRootCode, GroupID)

                oGroups = oGroupTree.ConvertAll(AddressOf XGroupConverter)

                returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._OK

                bolRet = True
            Catch ex As Exception
                Me.State.UpdateStateInfo(ex, "roDataLinkExport::GetGroups")
                returnCode = RoboticsExternAccess.Core.DTOs.ReturnCode._UnknownError
                bolRet = False
            End Try

            Return bolRet
        End Function

        ''' <summary>
        ''' Convierte un objeto de tipo roGroupTree a roGroup usado en api de servicios REST
        ''' </summary>
        ''' <param name="oGroupTree"></param>
        ''' <returns></returns>
        Private Function XGroupConverter(oGroupTree As roGroupTree) As RoboticsExternAccess.roGroup
            Dim oRet As RoboticsExternAccess.roGroup
            Try
                oRet = New RoboticsExternAccess.roGroup
                oRet.Id = oGroupTree.ID.ToString
                oRet.FullPath = oGroupTree.Path
                oRet.Name = oGroupTree.Name
                oRet.Childs = oGroupTree.ChildrenGroups.ConvertAll(AddressOf XGroupConverter).ToArray
                oRet.Employees = If(Not _IncludeEmployees, {}, oGroupTree.Employees.ConvertAll(AddressOf XEmployeeGroupConverter).ToArray)
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Convierte un objeto de tipo roEmployeeTree al tipo roEmployee usado en la api de servicios REST
        ''' </summary>
        ''' <param name="oEmployee"></param>
        ''' <returns></returns>
        Private Function XEmployeeGroupConverter(oEmployee As roEmployeeTree) As RoboticsExternAccess.roEmployeeOnGroup
            Dim oRet As RoboticsExternAccess.roEmployeeOnGroup
            Try
                oRet = New RoboticsExternAccess.roEmployeeOnGroup
                oRet.Name = oEmployee.Name

                ' Recuperamos identificador único
                Dim strImportPrimaryKeyUserField As String = New AdvancedParameter.roAdvancedParameter("ImportPrimaryKeyUserField", New AdvancedParameter.roAdvancedParameterState).Value
                If strImportPrimaryKeyUserField = String.Empty Then strImportPrimaryKeyUserField = "NIF"
                Dim oEmployeeUserField As UserFields.roEmployeeUserField = New UserFields.roEmployeeUserField
                oEmployeeUserField = UserFields.roEmployeeUserField.GetEmployeeUserFieldValueAtDate(oEmployee.ID, strImportPrimaryKeyUserField, DateTime.Now, New UserFields.roUserFieldState(Me.State.IDPassport))
                oRet.Id = If(oEmployeeUserField.FieldValue Is Nothing, String.Empty, oEmployeeUserField.FieldValue)

                oRet.InContract = (Not Contract.roContract.GetContractInDate(oEmployee.ID, DateTime.Now, New Contract.roContractState(Me.State.IDPassport)) Is Nothing)
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Convierte un objeto de tipo DataRow de contratos a roIDContract usado en api de servicios REST
        ''' </summary>
        ''' <param name="oContract"></param>
        ''' <returns></returns>
        Private Function XContractConverter(oContract As DataRow) As RoboticsExternAccess.roContract
            Dim oRet As RoboticsExternAccess.roContract
            Try
                oRet = New RoboticsExternAccess.roContract
                oRet.BeginDate = New roWCFDate(CDate(oContract("BeginDate")))
                oRet.EndDate = New roWCFDate(CDate(oContract("EndDate")))
                oRet.IDContract = oContract("IDContract")
                oRet.LabAgree = IIf(IsDBNull(oContract("LabAgreeName")), "", oContract("LabAgreeName"))
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Convierte un objeto de tipo DataRow de movilidades a roIDMobility usado en api de servicios REST
        ''' </summary>
        ''' <param name="oContract"></param>
        ''' <returns></returns>
        Private Function XMobilityConverter(oContract As DataRow) As RoboticsExternAccess.roMobility
            Dim oRet As RoboticsExternAccess.roMobility
            Try
                oRet = New RoboticsExternAccess.roMobility
                oRet.BeginDate = New roWCFDate(CDate(oContract("BeginDate")))
                oRet.EndDate = New roWCFDate(CDate(oContract("EndDate")))
                oRet.IDGroup = oContract("IDGroup")
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

        ''' <summary>
        ''' Convierte un objeto de tipo DataRow de campos de la ficha a roIEmployeeField usado en api de servicios REST
        ''' </summary>
        ''' <param name="oContract"></param>
        ''' <returns></returns>
        Private Function XEmployeeFieldConverter(oEmployeeField As DataRow) As RoboticsExternAccess.roEmployeeField
            Dim oRet As RoboticsExternAccess.roEmployeeField
            Try
                oRet = New RoboticsExternAccess.roEmployeeField
                oRet.FieldName = oEmployeeField("FieldName")
                oRet.FieldValueDate = New roWCFDate(CDate(oEmployeeField("Date")))
                oRet.FieldValue = oEmployeeField("Value")
            Catch ex As Exception
                oRet = Nothing
            End Try

            Return oRet
        End Function

    End Class

End Namespace