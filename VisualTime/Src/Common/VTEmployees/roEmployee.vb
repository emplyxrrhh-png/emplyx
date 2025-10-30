Imports System.Data.Common
Imports System.Runtime.Serialization
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBots
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Employee

    <DataContract>
    <Serializable>
    Public Class roEmployee

        Public Const roCardEmpty As Integer = -999999

        Private intID As Integer
        Private strName As String
        Private strGroupName As String
        Private strType As String
        Private intIDAccessGroup As Nullable(Of Integer)
        Private strEmployeeAlias As String
        Private oImage As Byte()
        Private intBiometricID As Nullable(Of Integer)
        Private bolAttControlled As Boolean
        Private bolAccControlled As Boolean
        Private bolJobControlled As Boolean
        Private bolExtControlled As Boolean
        Private bolRiskControlled As Boolean
        Private oEmployeeStatus As EmployeeStatusEnum
        Private bolAllowCardPlusBio As Boolean = False
        Private bolAllowBioPriority As Boolean = False
        Private bolAllowBiometric As Boolean = False
        Private bolAllowCards As Boolean = False
        Private intHighlightColor As Integer = 0

        Private strWebLogin As String = String.Empty
        Private strWebPassword As String = String.Empty
        Private bolActiveDirectory As Boolean = False
        Private bolHasForgottenRight As Boolean = False

#Region "Properties"

        <DataMember>
        Public Property ID() As Integer
            Get
                Return intID
            End Get
            Set(ByVal value As Integer)
                intID = value
            End Set
        End Property

        <DataMember>
        Public Property Name() As String
            Get
                Return strName
            End Get
            Set(ByVal value As String)
                strName = value
            End Set
        End Property

        <DataMember>
        Public Property GroupName() As String
            Get
                Return strGroupName
            End Get
            Set(ByVal value As String)
                strGroupName = value
            End Set
        End Property

        <DataMember>
        Public Property Type() As String
            Get
                Return strType
            End Get
            Set(ByVal value As String)
                strType = value
            End Set
        End Property

        <DataMember>
        Public Property Image() As Byte()
            Get
                Return oImage
            End Get
            Set(ByVal value As Byte())
                oImage = value
            End Set
        End Property

        <DataMember>
        Public Property IDAccessGroup() As Nullable(Of Integer)
            Get
                Return intIDAccessGroup
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intIDAccessGroup = value
                Me.bolAccControlled = (intIDAccessGroup.HasValue AndAlso intIDAccessGroup.Value > 0)
            End Set
        End Property

        <DataMember>
        Public Property BiometricID() As Nullable(Of Integer)
            Get
                Return intBiometricID
            End Get
            Set(ByVal value As Nullable(Of Integer))
                intBiometricID = value
            End Set
        End Property

        <DataMember>
        Public Property EmployeeAlias() As String
            Get
                Return strEmployeeAlias
            End Get
            Set(ByVal value As String)
                strEmployeeAlias = value
            End Set
        End Property

        <DataMember>
        Public Property AttControlled() As Boolean
            Get
                Return bolAttControlled
            End Get
            Set(ByVal value As Boolean)
                bolAttControlled = value
            End Set
        End Property

        <DataMember>
        Public Property AccControlled() As Boolean
            Get
                ' Si tiene asignado grupo de acceso se marca automàticamente el control de accesos
                Return (bolAccControlled OrElse (Me.IDAccessGroup.HasValue AndAlso Me.IDAccessGroup.Value > 0))
            End Get
            Set(ByVal value As Boolean)
                bolAccControlled = value
            End Set
        End Property

        <DataMember>
        Public Property JobControlled() As Boolean
            Get
                Return bolJobControlled
            End Get
            Set(ByVal value As Boolean)
                bolJobControlled = value
            End Set
        End Property

        <DataMember>
        Public Property ExtControlled() As Boolean
            Get
                Return bolExtControlled
            End Get
            Set(ByVal value As Boolean)
                bolExtControlled = False
            End Set
        End Property

        <DataMember>
        Public Property RiskControlled() As Boolean
            Get
                Return bolRiskControlled
            End Get
            Set(ByVal value As Boolean)
                bolRiskControlled = value
            End Set
        End Property

        <DataMember>
        Public Property EmployeeStatus() As EmployeeStatusEnum
            Get
                Return Me.oEmployeeStatus
            End Get
            Set(ByVal value As EmployeeStatusEnum)
                Me.oEmployeeStatus = value
            End Set
        End Property

        <DataMember>
        Public Property AllowCardPlusBio() As Boolean
            Get
                Return Me.bolAllowCardPlusBio
            End Get
            Set(ByVal value As Boolean)
                Me.bolAllowCardPlusBio = value
            End Set
        End Property

        <DataMember>
        Public Property AllowBioPriority() As Boolean
            Get
                Return Me.bolAllowBioPriority
            End Get
            Set(ByVal value As Boolean)
                Me.bolAllowBioPriority = value
            End Set
        End Property

        <DataMember>
        Public Property AllowBiometric() As Boolean
            Get
                Return Me.bolAllowBiometric
            End Get
            Set(ByVal value As Boolean)
                Me.bolAllowBiometric = value
            End Set
        End Property

        <DataMember>
        Public Property AllowCards() As Boolean
            Get
                Return Me.bolAllowCards
            End Get
            Set(ByVal value As Boolean)
                Me.bolAllowCards = value
            End Set
        End Property

        <DataMember>
        Public Property HighlightColor() As Integer
            Get
                Return Me.intHighlightColor
            End Get
            Set(ByVal value As Integer)
                Me.intHighlightColor = value
            End Set
        End Property

        <DataMember>
        Public Property WebLogin() As String
            Get
                Return strWebLogin
            End Get
            Set(ByVal value As String)
                strWebLogin = value
            End Set
        End Property

        <DataMember>
        Public Property WebPassword() As String
            Get
                Return strWebPassword
            End Get
            Set(ByVal value As String)
                strWebPassword = value
            End Set
        End Property

        <DataMember>
        Public Property ActiveDirectory() As Boolean
            Get
                Return bolActiveDirectory
            End Get
            Set(ByVal value As Boolean)
                bolActiveDirectory = value
            End Set
        End Property

        <DataMember>
        Public Property HasForgottenRight() As Boolean
            Get
                Return bolHasForgottenRight
            End Get
            Set(ByVal value As Boolean)
                bolHasForgottenRight = value
            End Set
        End Property

#End Region

#Region "Helper methods"

        Private Shared Function GetEmployeeData(ByVal oDatareader As System.Data.Common.DbDataReader) As roEmployee
            ' Función genérica que, pasado un datareader, copia todos los datos de un empleado en una
            ' clase WSCEmployee
            Dim oEmployee As New roEmployee
            oEmployee.ID = oDatareader("ID")
            oEmployee.Name = oDatareader("Name")
            oEmployee.GroupName = oDatareader("GroupName")
            oEmployee.Type = oDatareader("Type")
            If Not IsDBNull(oDatareader("Alias")) Then oEmployee.EmployeeAlias = oDatareader("Alias")
            If Not IsDBNull(oDatareader("IDAccessGroup")) Then oEmployee.IDAccessGroup = CInt(oDatareader("IDAccessGroup"))
            If Not IsDBNull(oDatareader("BiometricID")) Then oEmployee.BiometricID = oDatareader("BiometricID")
            If Not IsDBNull(oDatareader("image")) Then
                Dim bits As Byte() = CType(oDatareader("Image"), Byte())

                oEmployee.Image = bits
            Else
                oEmployee.Image = Nothing
            End If
            If Not IsDBNull(oDatareader("AttControlled")) Then
                oEmployee.AttControlled = oDatareader("AttControlled")
            Else
                oEmployee.AttControlled = False
            End If

            If Not IsDBNull(oDatareader("AccControlled")) Then
                oEmployee.AccControlled = oDatareader("AccControlled")
            Else
                oEmployee.AccControlled = False
            End If

            If Not IsDBNull(oDatareader("JobControlled")) Then
                oEmployee.JobControlled = oDatareader("JobControlled")
            Else
                oEmployee.JobControlled = False
            End If

            If Not IsDBNull(oDatareader("ExtControlled")) Then
                oEmployee.ExtControlled = oDatareader("ExtControlled")
            Else
                oEmployee.ExtControlled = False
            End If

            If Not IsDBNull(oDatareader("RiskControlled")) Then
                oEmployee.RiskControlled = False 'oDatareader("RiskControlled")
            Else
                oEmployee.RiskControlled = False
            End If

            If oDatareader("CurrentEmployee") = 0 And oDatareader("Begindate") >= Now Then
                ' El empleado es una futura incorporación
                oEmployee.EmployeeStatus = EmployeeStatusEnum.Future
            ElseIf oDatareader("CurrentEmployee") = 0 And oDatareader("Begindate") < Now Then
                ' El empleado es una baja
                oEmployee.EmployeeStatus = EmployeeStatusEnum.Old
            ElseIf oDatareader("CurrentEmployee") = 1 And oDatareader("Enddate") <> CDate("01/01/2079") Then
                ' Empleado con movilidad
                oEmployee.EmployeeStatus = EmployeeStatusEnum.Movility
            Else
                ' Empleado normal
                oEmployee.EmployeeStatus = EmployeeStatusEnum.Current
            End If

            If Not IsDBNull(oDatareader("AllowCardPlusBio")) Then oEmployee.AllowCardPlusBio = oDatareader("AllowCardPlusBio")
            If Not IsDBNull(oDatareader("AllowBioPriority")) Then oEmployee.AllowBioPriority = oDatareader("AllowBioPriority")
            If Not IsDBNull(oDatareader("AllowBiometric")) Then oEmployee.AllowBiometric = oDatareader("AllowBiometric")
            If Not IsDBNull(oDatareader("AllowCards")) Then oEmployee.AllowCards = oDatareader("AllowCards")

            oEmployee.HighlightColor = oDatareader("HighlightColor")

            oEmployee.WebLogin = roTypes.Any2String(oDatareader("WebLogin"))
            oEmployee.WebPassword = roTypes.Any2String(oDatareader("WebPassword"))
            oEmployee.ActiveDirectory = roTypes.Any2Boolean(oDatareader("ActiveDirectory"))
            oEmployee.HasForgottenRight = roTypes.Any2Boolean(oDatareader("HasForgottenRight"))

            Return oEmployee

        End Function

        Public Shared Function GetEmployee(ByVal IdEmployee As Integer, ByRef oState As roEmployeeState, Optional ByVal bolAudit As Boolean = False) As roEmployee
            ' Devuelve los datos del empleado con el código pasado por parámetro
            Dim strQuery As String
            Dim oDatareader As System.Data.Common.DbDataReader
            Dim oEmployee As roEmployee = Nothing

            Try
                Dim ds As New DataSet
                strQuery = "@SELECT# Employees.*, " &
                                  " sysrovwAllEmployeeGroups.CurrentEmployee, sysrovwAllEmployeeGroups.BeginDate, sysrovwAllEmployeeGroups.EndDate, sysrovwAllEmployeeGroups.GroupName " &
                           "FROM Employees WITH (NOLOCK) INNER JOIN sysrovwAllEmployeeGroups WITH (NOLOCK) " &
                                "ON Employees.[ID] = sysrovwAllEmployeeGroups.IDEmployee WHERE ID = @ID "
                Dim cmd As DbCommand = CreateCommand(strQuery)
                AddParameter(cmd, "@ID", DbType.Int32)
                cmd.Parameters("@ID").Value = IdEmployee

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(ds)

                oDatareader = ds.CreateDataReader ' Genero el DataReader para coger los datos

                If oDatareader IsNot Nothing Then
                    oDatareader.Read()
                    If oDatareader.HasRows Then
                        oEmployee = GetEmployeeData(oDatareader) ' Llamo a la función que rellena los datos del empleado en la clase WSCEmploye
                    End If
                    oDatareader.Close()
                End If

                If oEmployee IsNot Nothing AndAlso bolAudit Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oEmployee.Name, "", 1)
                    oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tEmployee, oEmployee.Name, tbParameters, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployee")
            End Try

            Return oEmployee

        End Function

        Public Shared Function GetEmployeeByContract(ByVal IdContract As String, ByRef oState As roEmployeeState) As roEmployee
            ' Devuelve los datos del empleado con el contrato pasado por parámetro
            Dim strQuery As String
            Dim oDataTable As System.Data.DataTable
            Dim oDatareader As System.Data.Common.DbDataReader
            Dim oEmployee As roEmployee = Nothing

            Try

                strQuery = "@SELECT# Employees.*, " &
                                  "sysrovwAllEmployeeGroups.CurrentEmployee, sysrovwAllEmployeeGroups.BeginDate, sysrovwAllEmployeeGroups.EndDate " &
                           "FROM Employees WITH (NOLOCK) INNER JOIN EmployeeContracts " &
                                    "ON EmployeeContracts.IDEmployee = Employees.ID " &
                                    "INNER JOIN sysrovwAllEmployeeGroups " &
                                    "ON Employees.[ID] = sysrovwAllEmployeeGroups.IDEmployee " &
                           "WHERE EmployeeContracts.IDContract = '" & IdContract & "'"

                oDataTable = CreateDataTable(strQuery, ) ' Ejecuto la select

                If oDataTable IsNot Nothing Then

                    oDatareader = oDataTable.CreateDataReader ' Genero el DataReader para coger los datos

                    If oDatareader IsNot Nothing Then
                        oDatareader.Read()
                        If oDatareader.HasRows Then
                            oEmployee = GetEmployeeData(oDatareader) ' Llamo a la función que rellena los datos del empleado en la clase WSCEmploye
                        End If
                        oDatareader.Close()
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeByContract")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeByContract")
            Finally

            End Try

            Return oEmployee

        End Function

        Public Shared Function GetIdEmployeeByContract(ByVal IdContract As String, ByRef oState As roEmployeeState) As Integer
            ' Recupera el ID del empleado relacionado al contrato pasado por parámetro
            Dim strQuery As String
            Dim oDataTable As System.Data.DataTable
            Dim oDatareader As System.Data.Common.DbDataReader
            Dim intIDEmployee As Integer = -1

            Try

                strQuery = " @SELECT# Employees.[ID] From Employees "
                strQuery = strQuery & " Inner Join EmployeeContracts ON "
                strQuery = strQuery & " IDContract = '" & IdContract & "' "
                strQuery = strQuery & " And IDEmployee = Employees.ID"

                oDataTable = CreateDataTable(strQuery, ) ' Ejecuto la select

                If oDataTable IsNot Nothing Then
                    oDatareader = oDataTable.CreateDataReader ' Genero el DataReader para coger los datos
                    If oDatareader IsNot Nothing Then
                        oDatareader.Read()
                        If oDatareader.HasRows Then
                            intIDEmployee = oDatareader("ID") ' Guado el ID del empleado
                        End If
                        oDatareader.Close()
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIdEmployeeByContract")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIdEmployeeByContract")
            End Try

            Return intIDEmployee

        End Function

        Public Shared Function GetIdEmployeeByNIF(ByVal strNIF As String, ByRef oState As roEmployeeState) As Integer
            ' Recupera el ID del empleado relacionado al NIF pasado por parámetro
            Dim strQuery As String
            Dim oDataTable As System.Data.DataTable
            Dim oDatareader As System.Data.Common.DbDataReader
            Dim intIDEmployee As Integer = -1

            Try

                strQuery = " @SELECT# [ID] From Employees WHERE USR_NIF like '" & strNIF & "'"

                oDataTable = CreateDataTable(strQuery, ) ' Ejecuto la select

                If oDataTable IsNot Nothing Then
                    oDatareader = oDataTable.CreateDataReader ' Genero el DataReader para coger los datos
                    If oDatareader IsNot Nothing Then
                        oDatareader.Read()
                        If oDatareader.HasRows Then
                            intIDEmployee = oDatareader("ID") ' Guado el ID del empleado
                        End If
                        oDatareader.Close()
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIdEmployeeByNIF")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIdEmployeeByNIF")
            End Try

            Return intIDEmployee

        End Function

        Public Shared Function GetEmployeeByName(ByVal strName As String, ByRef oState As roEmployeeState) As roEmployee
            ' Devuelve los datos del empleado con el nombre pasado por parámetro
            Dim strQuery As String
            Dim oDatareader As System.Data.Common.DbDataReader
            Dim oEmployee As roEmployee = Nothing

            Try

                Dim ds As New DataSet
                strQuery = "@SELECT# Employees.*, " &
                                   "sysrovwAllEmployeeGroups.CurrentEmployee, sysrovwAllEmployeeGroups.BeginDate, sysrovwAllEmployeeGroups.EndDate " &
                           "FROM Employees INNER JOIN sysrovwAllEmployeeGroups " &
                                    "ON Employees.[ID] = sysrovwAllEmployeeGroups.IDEmployee " &
                           "WHERE Employees.[Name] = @EmployeeName "
                Dim cmd As DbCommand = CreateCommand(strQuery)
                AddParameter(cmd, "@EmployeeName", DbType.String, 50)
                cmd.Parameters("@EmployeeName").Value = strName

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(ds)

                'oDataSet = CreateDataSet(strQuery) ' Ejecuto la select
                oDatareader = ds.CreateDataReader ' Genero el DataReader para coger los datos

                If oDatareader IsNot Nothing Then
                    oDatareader.Read()
                    If oDatareader.HasRows Then
                        oEmployee = GetEmployeeData(oDatareader) ' Llamo a la función que rellena los datos del empleado en la clase WSCEmploye
                    End If
                    oDatareader.Close()
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeByName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeByName")
            End Try

            Return oEmployee

        End Function

        Public Shared Function GetEmployeeSelectionPath(ByVal idEmployee As String, ByRef oState As roEmployeeState) As roEmployeeSelectionPath
            ' Devuelve los datos del empleado con el nombre pasado por parámetro
            Dim employeeInfo As roEmployeeSelectionPath = Nothing

            Try


                Dim strQuery = $"@SELECT# sysrovwCurrentEmployeeGroups.Path, sysrovwCurrentEmployeeGroups.IDEmployee, sysrovwCurrentEmployeeGroups.IDGroup, sysrovwCurrentEmployeeGroups.IDGroup 
                                    FROM sysrovwCurrentEmployeeGroups
                                    WHERE IDEmployee={idEmployee}"
                Dim dt As DataTable = Robotics.DataLayer.AccessHelper.CreateDataTable(strQuery)

                If dt IsNot Nothing AndAlso dt.Rows.Count > 0 Then
                    employeeInfo = New roEmployeeSelectionPath With {
                        .EmployeePath = $"B{roTypes.Any2Integer(dt.Rows(0)("IDEmployee"))}",
                        .IDEmployee = $"{idEmployee}",
                        .IDGroup = $"{roTypes.Any2Integer(dt.Rows(0)("IDGroup"))}",
                        .GroupSelectionPath = $"{roTypes.Any2String(dt.Rows(0)("Path"))}"
                        }


                    strQuery = $"@SELECT# pg.IDGroup, g.Path 
	                                    FROM sysroPassports_Groups pg 
	                                    INNER JOIN Groups g on pg.IDGroup = g.ID
                                    WHERE IDPassport = {oState.IDPassport}"

                    dt = DataLayer.AccessHelper.CreateDataTable(strQuery)

                    Dim assignedGroupsPaths As String() = {}
                    If dt IsNot Nothing Then
                        assignedGroupsPaths = dt.AsEnumerable().Select(Function(row) roTypes.Any2String(row("Path"))).ToArray()
                    End If

                    If assignedGroupsPaths.Length > 0 Then
                        For Each assignedPath As String In assignedGroupsPaths
                            If employeeInfo.GroupSelectionPath.StartsWith(assignedPath) Then

                                Dim sGroupsPath As String() = assignedPath.Split("\")
                                Dim sAssignedId As String = sGroupsPath(sGroupsPath.Length - 1)


                                employeeInfo.GroupSelectionPath = roTypes.ReplaceFirst(employeeInfo.GroupSelectionPath, assignedPath, sAssignedId)
                                employeeInfo.GroupSelectionPath = $"/source/A{employeeInfo.GroupSelectionPath.Replace("\", "/A")}/{employeeInfo.EmployeePath}"
                                Return employeeInfo
                            End If
                        Next
                    End If

                    'Si llegamos aquí es que solo tengo el empleado como excepción
                    employeeInfo.GroupSelectionPath = $"/source/B{employeeInfo.IDEmployee}"
                    Return employeeInfo

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeSelectionPath")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeSelectionPath")
            End Try

            Return employeeInfo

        End Function

        Public Shared Function GetIdEmployeeByName(ByVal strName As String, ByRef oState As roEmployeeState) As Integer
            ' Recupera el ID del empleado relacionado al nombre pasado por parámetro
            Dim intIDEmployee As Integer = -1

            Try

                Dim tb As New DataTable
                Dim strQuery As String

                strQuery = " @SELECT# Employees.* From Employees Where "
                strQuery = strQuery & " [Name] = @EmployeeName "

                Dim cmd As DbCommand = CreateCommand(strQuery)
                AddParameter(cmd, "@EmployeeName", DbType.String, 50)
                cmd.Parameters("@EmployeeName").Value = strName

                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                If tb IsNot Nothing Then
                    If tb.Rows.Count > 0 Then
                        intIDEmployee = tb.Rows(0).Item("ID")
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetIdEmployeeByName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetIdEmployeeByName")
            Finally

            End Try

            Return intIDEmployee
        End Function

        Public Shared Function ValidateEmployee(ByVal Employee As roEmployee, ByRef oState As roEmployeeState) As Boolean
            ' Comprueba los datos del empleado
            Dim bolRet As Boolean = True
            Dim strQuery As String

            oState.UpdateStateInfo()

            Try
                ' Miro si el ID existe
                If Employee.ID > 0 Then
                    Dim oEmp As roEmployee = roEmployee.GetEmployee(Employee.ID, oState, False)
                    If oEmp Is Nothing Then
                        oState.Result = EmployeeResultEnum.EmployeeNotExist
                    Else
                        ''Comprobar si se ha cambiado el tipo de empleado de J a A y comprobar si este empleado tiene acumulados de Task
                        If Employee.Type = "A" And oEmp.Type = "J" Then
                            If roBusinessSupport.HaveDailyTaskAccruals(oEmp.ID) Then
                                oState.Result = EmployeeResultEnum.EmployeeHaveDailyJobAccruals
                            End If
                        End If
                    End If
                End If

                If oState.Result = EmployeeResultEnum.NoError Then
                    ' El nombre no puede estar en blanco
                    If Employee.Name = "" Then
                        oState.Result = EmployeeResultEnum.InvalidEmployeeName
                    End If
                End If

                If oState.Result = EmployeeResultEnum.NoError Then
                    ' El Tipo tiene que ser A o J
                    If Employee.Type <> "A" And Employee.Type <> "J" Then
                        oState.Result = EmployeeResultEnum.InvalidEmployeeType
                    End If
                End If

                If oState.Result = EmployeeResultEnum.NoError Then
                    ' El IDAccessGroup tiene que existir
                    If Employee.IDAccessGroup.HasValue Then

                        Dim advParam As New AdvancedParameter.roAdvancedParameter("AdvancedAccessMode", New AdvancedParameter.roAdvancedParameterState)

                        If advParam IsNot Nothing AndAlso advParam.Value <> String.Empty AndAlso roTypes.Any2Integer(advParam.Value) = 1 Then

                            Dim strSQLAccess As String = "@SELECT# IDEmployee,IDAuthorization,GroupPath  FROM sysrovwAccessAuthorizations WHERE IDEmployee = " & Employee.ID
                            strSQLAccess = strSQLAccess & " UNION @SELECT# " & Employee.ID & ", IDAccessGroup, '0' AS GroupPath FROM Employees WHERE ID = " & Employee.ID & " AND IDAccessGroup IS NOT NULL"

                            Dim oAccessGroupDT As DataTable = CreateDataTable(strSQLAccess)

                            If oAccessGroupDT IsNot Nothing AndAlso oAccessGroupDT.Rows.Count > 0 Then
                                For Each oAccessGroupRow As DataRow In oAccessGroupDT.Rows
                                    strQuery = " @SELECT# Id From AccessGroups "
                                    strQuery = strQuery & " Where Id = " & oAccessGroupRow("IDAuthorization")
                                    Dim tb As DataTable = CreateDataTable(strQuery, )

                                    If tb IsNot Nothing Then
                                        If Not tb.CreateDataReader.HasRows Then
                                            oState.Result = EmployeeResultEnum.InvalidAccessGroup
                                            Exit For
                                        End If
                                    End If
                                Next
                            End If
                        Else
                            strQuery = " @SELECT# Id From AccessGroups "
                            strQuery = strQuery & " Where Id = " & Employee.IDAccessGroup.Value
                            Dim tb As DataTable = CreateDataTable(strQuery, )

                            If tb IsNot Nothing Then
                                If Not tb.CreateDataReader.HasRows Then
                                    oState.Result = EmployeeResultEnum.InvalidAccessGroup
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::ValidateEmployee")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::ValidateEmployee")
            Finally

            End Try

            bolRet = (oState.Result = EmployeeResultEnum.NoError)

            Return bolRet

        End Function

        Public Shared Function GetEmployeesFiltered(ByVal idPassport As Integer, ByVal employeesList As List(Of Integer)) As List(Of Integer)

            Dim oRet As List(Of Integer) = New List(Of Integer)

            Try

                If employeesList.Count > 0 Then
                    Dim strSQL As String
                    strSQL = "@SELECT# poe.IDEmployee FROM sysrovwSecurity_PermissionOverEmployees poe " &
                        " WHERE poe.IDPassport = " & idPassport & " " &
                        " AND CONVERT(DATE,GETDATE()) between poe.BeginDate and poe.EndDate AND poe.IDEmployee IN (" & String.Join(",", employeesList.ToArray) & ")"

                    Dim tb As DataTable = CreateDataTable(strSQL)

                    If tb IsNot Nothing Then
                        For Each Row In tb.Rows
                            oRet.Add(CInt(Row.Item("IDEmployee")))
                        Next
                    End If
                End If
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, $"roEmployee::GetEmployeesFiltered::Unknown error", ex)
            End Try

            Return oRet

        End Function

        Public Shared Function SaveEmployee(ByRef Employee As roEmployee, ByRef oState As roEmployeeState, Optional ByVal bolUpdatePassport As Boolean = True, Optional ByVal bolCallBroadcaster As Boolean = True) As Boolean
            ' Comprueba los datos del empleado
            Dim bolRet As Boolean
            Dim strQuery As String
            Dim oSqlCommand As DbCommand = Nothing

            Dim strQueryRow As String = ""
            Dim oEmployeeOld As DataRow = Nothing
            Dim oEmployeeNew As DataRow = Nothing

            Dim oEmployeeOldName As String = String.Empty

            oState.UpdateStateInfo()

            If Not DataLayer.roSupport.IsXSSSafe(Employee) Then
                oState.Result = EmployeeResultEnum.XSSvalidationError
                Return False
            End If


            If ValidateEmployee(Employee, oState) Then

                Try
                    strQueryRow = "@SELECT# ID, Name, Type, IdAccessGroup, Alias, BiometricID, Image, " &
                                         "AllowCardPlusBio, AllowBioPriority, AllowBiometric, AllowCards, HighlightColor, WebLogin, WebPassword, ActiveDirectory, HasForgottenRight " &
                                  "FROM Employees WHERE [ID] = " & Employee.ID
                    Dim tbAuditOld As DataTable = CreateDataTable(strQueryRow, "Employees")
                    If tbAuditOld.Rows.Count = 1 Then
                        oEmployeeOld = tbAuditOld.Rows(0)
                        oEmployeeOldName = oEmployeeOld("Name")
                    End If

                    If Employee.ID = -1 Then
                        ' Si el codigo del empleado es -1 quiere decir que no existe por lo que hago un insert
                        strQuery = "@INSERT# INTO Employees "
                        strQuery = strQuery & " ( ID, Name, Type, "
                        strQuery = strQuery & " IdAccessGroup, Alias, "
                        strQuery = strQuery & " Image, AllowCardPlusBio, AllowBioPriority, AllowBiometric, AllowCards, "
                        strQuery = strQuery & " AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled, HighlightColor, WebLogin, WebPassword, ActiveDirectory, HasForgottenRight) "
                        strQuery = strQuery & " Values "
                        strQuery = strQuery & " ( @ID, @Name, @Type, "
                        strQuery = strQuery & " @IDAccessGroup, @Alias, "
                        strQuery = strQuery & " @Image, @AllowCardPlusBio, @AllowBioPriority, @AllowBiometric, @AllowCards, "
                        strQuery = strQuery & " @AttControlled, @AccControlled, @JobControlled, @ExtControlled, @RiskControlled, @HighlightColor, @WebLogin, @WebPassword, @ActiveDirectory,@HasForgottenRight) "
                        'strQuery = strQuery & " @BiometricID, @Image) "
                    Else
                        ' Si el codigo del empleado NO es -1 quiere decir que existe por lo que hago un update
                        strQuery = "@UPDATE# Employees "
                        strQuery = strQuery & " Set Name = @Name "
                        strQuery = strQuery & " , Type = @Type "
                        strQuery = strQuery & " , IDAccessGroup = @IDAccessGroup"
                        strQuery = strQuery & " , Alias = @Alias "
                        'strQuery = strQuery & " , BiometricID = @BiometricID"
                        strQuery = strQuery & " , Image = @Image "
                        strQuery = strQuery & " , AllowCardPlusBio = @AllowCardPlusBio"
                        strQuery = strQuery & " , AllowBioPriority = @AllowBioPriority"
                        strQuery = strQuery & " , AllowBiometric = @AllowBiometric"
                        strQuery = strQuery & " , AllowCards = @AllowCards"
                        strQuery = strQuery & " , AttControlled = @AttControlled"
                        strQuery = strQuery & " , AccControlled = @AccControlled"
                        strQuery = strQuery & " , JobControlled = @JobControlled"
                        strQuery = strQuery & " , ExtControlled = @ExtControlled"
                        strQuery = strQuery & " , RiskControlled = @RiskControlled"
                        strQuery = strQuery & " , HighlightColor = @HighlightColor"
                        strQuery = strQuery & " , WebLogin = @WebLogin"
                        strQuery = strQuery & " , WebPassword = @WebPassword"
                        strQuery = strQuery & " , ActiveDirectory = @ActiveDirectory"
                        strQuery = strQuery & " , HasForgottenRight = @HasForgottenRight"
                        strQuery = strQuery & " Where ID = " & Employee.ID
                    End If

                    oSqlCommand = CreateCommand(strQuery)

                    AddParameter(oSqlCommand, "@Name", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@Type", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@IDAccessGroup", System.Data.SqlDbType.SmallInt)
                    AddParameter(oSqlCommand, "@Alias", System.Data.SqlDbType.VarChar)
                    'AddParameter(oSqlCommand, "@BiometricID", System.Data.SqlDbType.Int)
                    AddParameter(oSqlCommand, "@Image", System.Data.SqlDbType.Binary)
                    AddParameter(oSqlCommand, "@AllowCardPlusBio", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AllowBioPriority", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AllowBiometric", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AllowCards", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AttControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AccControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@JobControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@ExtControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@RiskControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@HighlightColor", System.Data.SqlDbType.Int)
                    AddParameter(oSqlCommand, "@WebLogin", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@WebPassword", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@ActiveDirectory", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@HasForgottenRight", System.Data.SqlDbType.Bit)

                    If Employee.ID = -1 Then ' Sólo añado el parámetro si es un insert
                        AddParameter(oSqlCommand, "@ID", System.Data.SqlDbType.Int)
                    End If

                    oSqlCommand.Parameters("@Name").Value = Employee.Name
                    oSqlCommand.Parameters("@Type").Value = Employee.Type
                    oSqlCommand.Parameters("@HighlightColor").Value = Employee.HighlightColor

                    oSqlCommand.Parameters("@WebLogin").Value = Employee.WebLogin
                    oSqlCommand.Parameters("@WebPassword").Value = Employee.WebPassword
                    oSqlCommand.Parameters("@ActiveDirectory").Value = Employee.ActiveDirectory
                    oSqlCommand.Parameters("@HasForgottenRight").Value = Employee.HasForgottenRight

                    If Employee.IDAccessGroup.HasValue Then
                        oSqlCommand.Parameters("@IdAccessGroup").Value = Employee.IDAccessGroup.Value
                    Else
                        oSqlCommand.Parameters("@IdAccessGroup").Value = DBNull.Value
                    End If
                    If Employee.EmployeeAlias Is Nothing OrElse Employee.EmployeeAlias = "" Then
                        oSqlCommand.Parameters("@Alias").Value = DBNull.Value
                    Else
                        oSqlCommand.Parameters("@Alias").Value = Employee.EmployeeAlias
                    End If

                    If Employee.Image IsNot Nothing Then
                        oSqlCommand.Parameters("@Image").Value = Employee.Image
                    Else
                        oSqlCommand.Parameters("@Image").Value = DBNull.Value
                    End If

                    oSqlCommand.Parameters("@AllowCardPlusBio").Value = Employee.AllowCardPlusBio
                    oSqlCommand.Parameters("@AllowBioPriority").Value = Employee.AllowBioPriority
                    oSqlCommand.Parameters("@AllowBiometric").Value = Employee.AllowBiometric
                    oSqlCommand.Parameters("@AllowCards").Value = Employee.AllowCards

                    oSqlCommand.Parameters("@AttControlled").Value = Employee.AttControlled
                    oSqlCommand.Parameters("@AccControlled").Value = Employee.AccControlled
                    oSqlCommand.Parameters("@JobControlled").Value = Employee.JobControlled
                    oSqlCommand.Parameters("@ExtControlled").Value = Employee.ExtControlled
                    oSqlCommand.Parameters("@RiskControlled").Value = False
                Catch ex As DbException
                    oState.UpdateStateInfo(ex, "roEmployees::SaveEmployee")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roEmployees::SaveEmployee")
                End Try

                If oState.Result = EmployeeResultEnum.NoError Then

                    Do
                        Dim bolNew As Boolean = False
                        Try
                            If Employee.ID = -1 Then ' Sólo añado el parámetro si es un insert
                                bolNew = True
                                Employee.ID = GetNextIDEmployee()
                                oSqlCommand.Parameters("@ID").Value = Employee.ID
                            End If

                            oSqlCommand.ExecuteNonQuery()

                            If bolUpdatePassport Then
                                ' Actualizamos el passport asociado al empleado
                                Dim oPassportManager As New roPassportManager()
                                Dim oPassport As roPassport = oPassportManager.LoadPassport(Employee.ID, LoadType.Employee, )
                                If oPassport Is Nothing Then
                                    oPassport = New roPassport()
                                    oPassport.IDEmployee = Employee.ID
                                    oPassport.GroupType = "E"
                                    Dim oSettings As New roSettings()

                                    Dim oLanguageManager As New roLanguageManager()
                                    oPassport.Language = oLanguageManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))

                                    oPassport.State = 1
                                End If
                                oPassport.Name = Employee.Name
                                oPassportManager.Save(oPassport, False)

                            End If

                            If bolNew Then
                                'insert id usuario de forma automática
                                Dim sIdSQL As String = "@INSERT# INTO [dbo].[EmployeeUserFieldValues] (IDEmployee,Fieldname,date,value) " &
                                                        "VALUES (" & Employee.ID & ",(@SELECT# FieldName FROM [dbo].[sysroUserFields] WHERE [Alias] = 'sysroVisualtimeID'),convert(smalldatetime,'1900-01-01',120), '" & Employee.ID & "')"

                                ExecuteSql(sIdSQL)
                            End If

                            strQueryRow = "@SELECT# ID, Name, Type, IdAccessGroup, Alias, BiometricID, " &
                                                 "AllowCardPlusBio, AllowBioPriority, AllowBiometric, AllowCards " &
                                          "FROM Employees WHERE [ID] = " & Employee.ID
                            Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "Employees")
                            If tbAuditNew.Rows.Count = 1 Then oEmployeeNew = tbAuditNew.Rows(0)

                            ' Timestamp en caso de modificar el nombre
                            If bolNew OrElse oEmployeeOldName <> Employee.Name Then
                                VTBase.Extensions.roTimeStamps.UpdateEmployeeTimestamp(Employee.ID)
                            End If

                            ' Insertar registro auditoria
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            If bolNew Then
                                oState.AddAuditFieldsValues(tbParameters, oEmployeeNew)
                                oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tEmployee, oEmployeeNew("Name"), tbParameters, -1)
                            Else
                                oState.AddAuditFieldsValues(tbParameters, oEmployeeNew, oEmployeeOld)
                                oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tEmployee,
                                             IIf(oEmployeeOld("Name") <> oEmployeeNew("Name"), oEmployeeOld("Name") & " -> " & oEmployeeNew("Name"), oEmployeeNew("Name")),
                                             tbParameters, -1)
                            End If

                            If bolCallBroadcaster Then
                                ' Si se ha modificado el nombre o la imagen del empleado, disparo el proceso BROADCASTER
                                If oEmployeeOld IsNot Nothing Then
                                    Dim intImageOld As Integer = 0
                                    Dim intImageNew As Integer = 0
                                    If Not IsDBNull(oEmployeeOld("Image")) Then
                                        Dim bImageOld As Byte() = oEmployeeOld("Image")
                                        intImageOld = bImageOld.Length
                                    End If
                                    If Employee.Image IsNot Nothing Then
                                        intImageNew = Employee.Image.Length
                                    End If

                                    If oEmployeeOld("Name") <> Employee.Name Or intImageOld <> intImageNew Then
                                        roConnector.InitTask(TasksType.BROADCASTER)
                                    End If
                                Else
                                    If bolNew Then
                                        roConnector.InitTask(TasksType.BROADCASTER)
                                    End If
                                End If
                            End If

                            oState.Result = EmployeeResultEnum.NoError
                            oState.ErrorText = ""
                        Catch ex As DbException
                            oState.UpdateStateInfo(ex, "roEmployees::SaveEmployee")
                            If bolNew Then Employee.ID = -1
                        Catch ex As Exception
                            oState.UpdateStateInfo(ex, "roEmployees::SaveEmployee")
                            If bolNew Then Employee.ID = -1
                        End Try
                    Loop Until InStr(oState.ErrorText, "PRIMARY KEY") = 0
                    ' Esto controla que no se produzca un error de duplicate PRIMARY KEY

                End If

            End If

            bolRet = (oState.Result = EmployeeResultEnum.NoError)

            Return bolRet

        End Function

        ''' <summary>
        ''' Creación de un empleado
        ''' </summary>
        ''' <param name="Employee">roEmployee</param>
        ''' <param name="oState">Control de Errores</param>
        ''' <param name="oTrans">Transacción</param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateEmployee(ByRef Employee As roEmployee, ByRef oState As roEmployeeState, Optional ByVal bolUpdatePassport As Boolean = True) As Boolean
            ' Comprueba los datos del empleado
            Dim bolRet As Boolean
            Dim strQuery As String
            Dim oSqlCommand As System.Data.Common.DbCommand = Nothing

            Dim strQueryRow As String = ""
            Dim oEmployeeOld As DataRow = Nothing
            Dim oEmployeeNew As DataRow = Nothing

            oState.UpdateStateInfo()

            ValidateEmployee(Employee, oState) 'Comprobación del Estado del empleado
            If oState.Result = EmployeeResultEnum.EmployeeNotExist Then 'Si no existe, creamos

                Try

                    strQuery = "@INSERT# INTO Employees "
                    strQuery = strQuery & " ( ID, Name, Type, "
                    strQuery = strQuery & " IdAccessGroup, Alias, "
                    strQuery = strQuery & " Image, AllowCardPlusBio, AllowBioPriority, AllowBiometric, AllowCards, "
                    strQuery = strQuery & " AttControlled, AccControlled, JobControlled, ExtControlled, RiskControlled, HighlightColor, WebLogin, WebPassword, ActiveDirectory, HasForgottenRight) "
                    strQuery = strQuery & " Values "
                    strQuery = strQuery & " ( @ID, @Name, @Type, "
                    strQuery = strQuery & " @IDAccessGroup, @Alias, "
                    strQuery = strQuery & " @Image, @AllowCardPlusBio, @AllowBioPriority, @AllowBiometric, @AllowCards, "
                    strQuery = strQuery & " @AttControlled, @AccControlled, @JobControlled, @ExtControlled, @RiskControlled, @HighlightColor, @WebLogin, @WebPassword, @ActiveDirectory @HasForgottenRight) "
                    'strQuery = strQuery & " @BiometricID, @Image) "

                    oSqlCommand = CreateCommand(strQuery)

                    AddParameter(oSqlCommand, "@Name", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@Type", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@IDAccessGroup", System.Data.SqlDbType.SmallInt)
                    AddParameter(oSqlCommand, "@Alias", System.Data.SqlDbType.VarChar)
                    'AddParameter(oSqlCommand, "@BiometricID", System.Data.SqlDbType.Int)
                    AddParameter(oSqlCommand, "@Image", System.Data.SqlDbType.Binary)
                    AddParameter(oSqlCommand, "@AllowCardPlusBio", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AllowBioPriority", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AllowBiometric", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AllowCards", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AttControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@AccControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@JobControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@ExtControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@RiskControlled", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@HighlightColor", System.Data.SqlDbType.Int)
                    AddParameter(oSqlCommand, "@WebLogin", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@WebPassword", System.Data.SqlDbType.VarChar)
                    AddParameter(oSqlCommand, "@ActiveDirectory", System.Data.SqlDbType.Bit)
                    AddParameter(oSqlCommand, "@HasForgottenRight", System.Data.SqlDbType.Bit)

                    AddParameter(oSqlCommand, "@ID", System.Data.SqlDbType.Int)

                    oSqlCommand.Parameters("@Name").Value = Employee.Name
                    oSqlCommand.Parameters("@Type").Value = Employee.Type
                    oSqlCommand.Parameters("@HighlightColor").Value = Employee.HighlightColor

                    oSqlCommand.Parameters("@WebLogin").Value = Employee.WebLogin
                    oSqlCommand.Parameters("@WebPassword").Value = Employee.WebPassword
                    oSqlCommand.Parameters("@ActiveDirectory").Value = Employee.ActiveDirectory

                    If Employee.IDAccessGroup.HasValue Then
                        oSqlCommand.Parameters("@IdAccessGroup").Value = Employee.IDAccessGroup.Value
                    Else
                        oSqlCommand.Parameters("@IdAccessGroup").Value = DBNull.Value
                    End If
                    If Employee.EmployeeAlias Is Nothing OrElse Employee.EmployeeAlias = "" Then
                        oSqlCommand.Parameters("@Alias").Value = DBNull.Value
                    Else
                        oSqlCommand.Parameters("@Alias").Value = Employee.EmployeeAlias
                    End If
                    'If Employee.BiometricID.HasValue Then
                    '    oSqlCommand.Parameters("@BiometricID").Value = Employee.BiometricID
                    'Else
                    '    oSqlCommand.Parameters("@BiometricID").Value = DBNull.Value
                    'End If
                    If Employee.Image IsNot Nothing Then
                        oSqlCommand.Parameters("@Image").Value = Employee.Image
                    Else
                        oSqlCommand.Parameters("@Image").Value = DBNull.Value
                    End If

                    oSqlCommand.Parameters("@AllowCardPlusBio").Value = Employee.AllowCardPlusBio
                    oSqlCommand.Parameters("@AllowBioPriority").Value = Employee.AllowBioPriority
                    oSqlCommand.Parameters("@AllowBiometric").Value = Employee.AllowBiometric
                    oSqlCommand.Parameters("@AllowCards").Value = Employee.AllowCards

                    oSqlCommand.Parameters("@AttControlled").Value = Employee.AttControlled
                    oSqlCommand.Parameters("@AccControlled").Value = Employee.AccControlled
                    oSqlCommand.Parameters("@JobControlled").Value = Employee.JobControlled
                    oSqlCommand.Parameters("@ExtControlled").Value = Employee.ExtControlled
                    oSqlCommand.Parameters("@RiskControlled").Value = False ' Employee.RiskControlled
                Catch ex As DbException
                    oState.UpdateStateInfo(ex, "roEmployees::CreateEmployee")
                Catch ex As Exception
                    oState.UpdateStateInfo(ex, "roEmployees::CreateEmployee")
                End Try

                If oState.Result = EmployeeResultEnum.EmployeeNotExist Then

                    Do
                        Dim bolNew As Boolean = False
                        Try
                            bolNew = True
                            oSqlCommand.Parameters("@ID").Value = Employee.ID

                            oSqlCommand.ExecuteNonQuery()

                            If bolUpdatePassport Then
                                ' Actualizamos el passport asociado al empleado
                                Dim oPassportManager As New roPassportManager()
                                Dim oPassport As roPassport = oPassportManager.LoadPassport(Employee.ID, LoadType.Employee, )
                                If oPassport Is Nothing Then
                                    oPassport = New roPassport()
                                    oPassport.IDEmployee = Employee.ID
                                    oPassport.GroupType = "E"

                                    Dim oSettings As New roSettings()
                                    Dim oLanguageManager As New roLanguageManager()
                                    oPassport.Language = oLanguageManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))

                                    oPassport.State = 1
                                End If
                                oPassport.Name = Employee.Name
                                oPassportManager.Save(oPassport, False)
                            End If

                            strQueryRow = "@SELECT# ID, Name, Type, IdAccessGroup, Alias, BiometricID, Image, " &
                                                 "AllowCardPlusBio, AllowBioPriority, AllowBiometric, AllowCards " &
                                          "FROM Employees WHERE [ID] = " & Employee.ID
                            Dim tbAuditNew As DataTable = CreateDataTable(strQueryRow, "Employees")
                            If tbAuditNew.Rows.Count = 1 Then oEmployeeNew = tbAuditNew.Rows(0)

                            ' Insertar registro auditoria
                            Dim tbParameters As DataTable = oState.CreateAuditParameters()
                            If bolNew Then
                                oState.AddAuditFieldsValues(tbParameters, oEmployeeNew)
                                oState.Audit(Audit.Action.aInsert, Audit.ObjectType.tEmployee, oEmployeeNew("Name"), tbParameters, -1)
                            End If

                            ' Si se ha modificado el nombre del empleado, disparo el proceso BROADCASTER
                            If oEmployeeOld IsNot Nothing AndAlso oEmployeeOld("Name") <> Employee.Name Then
                                ' Hacemos dos peticiones a broadcaster, para controlar los terminales tipo rx y los tipo mx6
                                roConnector.InitTask(TasksType.BROADCASTER)
                                Dim oParams As New roCollection()
                                oParams.Add("Employee.ID", Employee.ID)
                                roConnector.InitTask(TasksType.BROADCASTER, oParams)
                            End If

                            oState.Result = EmployeeResultEnum.NoError
                            oState.ErrorText = ""
                        Catch ex As DbException
                            oState.UpdateStateInfo(ex, "roEmployees::CreateEmployee")
                            If bolNew Then Employee.ID = -1
                        Catch ex As Exception
                            oState.UpdateStateInfo(ex, "roEmployees::CreateEmployee")
                            If bolNew Then Employee.ID = -1
                        End Try
                    Loop Until InStr(oState.ErrorText, "PRIMARY KEY") = 0
                    ' Esto controla que no se produzca un error de duplicate PRIMARY KEY

                End If

            End If

            bolRet = (oState.Result = EmployeeResultEnum.NoError)

            Return bolRet

        End Function

        Public Shared Function DeleteEmployee(ByVal IDEmployee As Integer, ByRef oState As roEmployeeState, Optional ByVal bCallBroadcaster As Boolean = True) As Boolean
            ' Borra los datos de un empleado
            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False
            Dim oLicense As New roServerLicense
            Dim oSqlArray As New Generic.List(Of String)
            Dim strSQL As String
            Dim lPunchesWithPhotoToDelete As List(Of Integer) = New List(Of Integer)

            oSqlArray.Add("AbsenceTracking")
            oSqlArray.Add("Documents")
            oSqlArray.Add("AlertsTask")
            oSqlArray.Add("AccessMoves")
            oSqlArray.Add("DailyAccruals")
            oSqlArray.Add("DailyCauses")
            oSqlArray.Add("@DELETE# FROM sysroRemarks WHERE ID IN (@SELECT# Remarks FROM DailySchedule WHERE Remarks IS NOT NULL AND IDEmployee =" & IDEmployee & ")")
            oSqlArray.Add("DailySchedule")
            oSqlArray.Add("DailyIncidences")
            oSqlArray.Add("DailyTaskAccruals")
            oSqlArray.Add("EmployeeAccessAuthorization")
            oSqlArray.Add("EmployeeAccrualsRules")
            oSqlArray.Add("EmployeeAssignments")
            oSqlArray.Add("EmployeeBiometricData")
            oSqlArray.Add("EmployeeBiometricDataRX")
            oSqlArray.Add("EmployeeBiometricDataSX")
            oSqlArray.Add("EmployeeBiometricDataZK")
            oSqlArray.Add("EmployeeBiometricFaceDataZK")
            oSqlArray.Add("EmployeeConceptAnnualLimits")
            oSqlArray.Add("EmployeeConceptCarryOvers")
            oSqlArray.Add("EmployeeContracts")
            oSqlArray.Add("EmployeeCostCenters")
            oSqlArray.Add("EmployeeExpectedShifts")
            oSqlArray.Add("EmployeeGroups")
            oSqlArray.Add("EmployeeStatus")
            oSqlArray.Add("EmployeeTasks")
            oSqlArray.Add("EmployeeTaskTemplates")
            oSqlArray.Add("EmployeeTeams")
            oSqlArray.Add("EmployeeTerminalMessages")
            oSqlArray.Add("EmployeeUserFieldValues")
            oSqlArray.Add("InvalidAccessMoves")
            oSqlArray.Add("ProgrammedAbsences")
            oSqlArray.Add("ProgrammedCauses")
            oSqlArray.Add("ProgrammedHolidays")
            oSqlArray.Add("ProgrammedOvertimes")
            oSqlArray.Add("CommuniqueEmployees")
            oSqlArray.Add("CommuniqueEmployeeStatus")
            oSqlArray.Add("@DELETE# FROM MovesCaptures Where IDMove in( @SELECT# ID From Moves Where IDEmployee = " & IDEmployee & ") ")
            oSqlArray.Add("Moves")
            oSqlArray.Add("@DELETE# FROM PunchesCaptures Where IDPunch in( @SELECT# ID From Punches Where IDEmployee = " & IDEmployee & ") ")
            oSqlArray.Add("Punches")
            oSqlArray.Add("@DELETE# FROM sysroRequestDays WHERE IDRequest IN (@SELECT# ID FROM Requests WHERE IDEmployee =" & IDEmployee & ")")
            oSqlArray.Add("@DELETE# FROM RequestsApprovals WHERE IDRequest IN (@SELECT# ID FROM Requests WHERE IDEmployee =" & IDEmployee & ")")
            oSqlArray.Add("Requests")
            oSqlArray.Add("@DELETE# FROM VisitMoves Where VisitPlanID in( @SELECT# ID From VisitPlan Where EmpVisitedID = " & IDEmployee & ") ")
            oSqlArray.Add("@DELETE# FROM VisitMoves Where VisitPlanID in( @SELECT# ID From VisitPlan Where PlannedByID = " & IDEmployee & ") ")
            oSqlArray.Add("@DELETE# FROM VisitUserFieldsValues Where VisitPlanID in( @SELECT# ID From VisitPlan Where EmpVisitedID = " & IDEmployee & ")")
            oSqlArray.Add("@DELETE# FROM VisitUserFieldsValues Where VisitPlanID in( @SELECT# ID From VisitPlan Where PlannedByID = " & IDEmployee & ")")
            oSqlArray.Add("@DELETE# FROM VisitPlan Where EmpVisitedID = " & IDEmployee & " ")
            oSqlArray.Add("@DELETE# FROM VisitPlan Where PlannedByID = " & IDEmployee & " ")
            oSqlArray.Add("sysroSecurityNode_Passports_PermissionsOverEmployeesExceptions")
            oSqlArray.Add("@DELETE# FROM ToDoListTasks WHERE IdList IN (@SELECT# Id FROM ToDoLists WHERE IdEmployee = " & IDEmployee & ") ")
            oSqlArray.Add("ToDoLists")
            oSqlArray.Add("IF (EXISTS (@SELECT# * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'EmployeeAccessGroups')) BEGIN @DELETE# FROM EmployeeAccessGroups WHERE IDEmployee = " & IDEmployee & " END")
            oSqlArray.Add("DeletedProgrammedAbsences")
            oSqlArray.Add("DeletedProgrammedCauses")
            oSqlArray.Add("DeletedProgrammedHolidays")
            oSqlArray.Add("EmployeeCenters")
            oSqlArray.Add("EmployeeJobMoves")
            oSqlArray.Add("EmployeeJobs")
            oSqlArray.Add("EmployeeMachines")
            oSqlArray.Add("EmployeeProfiles")
            oSqlArray.Add("SurveyEmployeeResponses")
            oSqlArray.Add("SurveyEmployees")
            oSqlArray.Add("sysroPendingCarryOvers")
            oSqlArray.Add("sysroPunchesTransactions")
            oSqlArray.Add("TerminalReaderEmployees")
            oSqlArray.Add("TerminalsSyncEmployeeAccessLevelData")
            oSqlArray.Add("TerminalsSyncPushEmployeeTimeZonesData")
            oSqlArray.Add("TerminalsSyncTasks")
            oSqlArray.Add("TerminalsTasksSX")
            oSqlArray.Add("Visit")
            oSqlArray.Add("WebValidatorMonitoredEmployees")
            oSqlArray.Add("WtRequest")
            oSqlArray.Add("ChannelEmployees")
            oSqlArray.Add("sysroPassports_Employees")
            oSqlArray.Add("sysroPassports_PermissionsOverEmployees")
            oSqlArray.Add("@DELETE# FROM ChannelConversationMessages WHERE IdConversation IN (@SELECT# IDConversation FROM ChannelConversationMessages WHERE IDEmployee = " & IDEmployee & ")")
            oSqlArray.Add("@DELETE# FROM ChannelConversations WHERE CreatedBy =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM sysroDeletedPunchesSync WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM sysroPermissionsOverEmployeesExceptions WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM TerminalsSyncBiometricData WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM TerminalsSyncCardsData WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM TerminalsSyncDocumentsData WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM TerminalsSyncEmployeesData WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM TerminalsSyncGroupsData WHERE EmployeeID =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM RequestsApprovals WHERE IDRequest IN (@SELECT# ID FROM Requests WHERE IDEmployeeExchange =" & IDEmployee & ")")
            oSqlArray.Add("@DELETE# FROM Requests WHERE IDEmployeeExchange =" & IDEmployee)
            oSqlArray.Add("@DELETE# FROM BotRules WHERE Type =" & BotRuleTypeEnum.CopyEmployeePermissions & " AND IDTemplate=" & IDEmployee)

            oState.UpdateStateInfo()

            ' Guardo id de los fichajes que voy a borrar si tienen fotos en azure, para borrarlas después de confirmar la transacción
            Dim strSQLPunchesWithPhoto As String = String.Empty
            strSQLPunchesWithPhoto = "@SELECT# ID FROM Punches WHERE PhotoOnAzure = 1 AND IDEmployee = " & IDEmployee
            Dim tbPunchWithPhoto As DataTable = CreateDataTable(strSQLPunchesWithPhoto)
            If tbPunchWithPhoto IsNot Nothing AndAlso tbPunchWithPhoto.Rows.Count > 0 Then
                For Each oPunchRow As DataRow In tbPunchWithPhoto.Rows
                    lPunchesWithPhotoToDelete.Add(roTypes.Any2Integer("ID"))
                Next
            End If

            Try
                ' Valido algunos datos del empleado
                If roBusinessSupport.HaveJobMoves(IDEmployee) Then
                    oState.Result = EmployeeResultEnum.EmployeeHaveJobMoves
                Else
                    If roBusinessSupport.HaveDailyJobAccruals(IDEmployee) Then
                        oState.Result = EmployeeResultEnum.EmployeeHaveDailyJobAccruals
                    Else
                        If roBusinessSupport.HaveTeamJobMoves(IDEmployee) Then
                            oState.Result = EmployeeResultEnum.EmployeeHaveTeamJobMoves
                        End If
                    End If
                End If

                bolRet = (oState.Result = EmployeeResultEnum.NoError)

                If bolRet Then
                    bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction(IsolationLevel.ReadUncommitted)

                    ' Primero borro todas las tablas relacionadas
                    For Each strSQL In oSqlArray
                        If Not (strSQL.ToUpper.Contains("DELETE")) Or strSQL.ToUpper.Contains("DELETEDPROGRAMMED") Then
                            strSQL = $"@DELETE# {strSQL} Where IDEmployee = {IDEmployee}"
                        End If

                        bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                        If Not bolRet Then
                            Exit For
                        End If
                    Next
                End If

                If bolRet Then
                    ' Borramos el passport asociado al empleado, si existe.
                    Dim oSecurityState As New roSecurityState()
                    roBusinessState.CopyTo(oState, oSecurityState)
                    Dim oPassportManager As New roPassportManager(oSecurityState)
                    Dim oPassport As roPassport = oPassportManager.LoadPassport(IDEmployee, LoadType.Employee)
                    If oPassport IsNot Nothing Then

                        Dim _intIDPassport As Integer = oPassport.ID
                        Dim intIDParentPassport As Integer = 0
                        If oPassport.IDParentPassport.HasValue Then
                            intIDParentPassport = oPassport.IDParentPassport
                        End If

                        oPassportManager.Delete(oPassport, bCallBroadcaster)
                        ExecuteSql("@DELETE# FROM sysroPassports_Sessions WHERE IDPassport=" & _intIDPassport.ToString)
                        ExecuteSql("@DELETE# FROM sysropassports_data WHERE IDPassport=" & _intIDPassport.ToString)

                        If intIDParentPassport > 0 And intIDParentPassport <> 3 Then
                            ' hay que borrar el grupo de usuario asociado
                            ' a menos que sea el grupo administrador

                            ' Eliminamos el grupo padre
                            Dim oParentPassport As roPassport = oPassportManager.LoadPassport(intIDParentPassport, LoadType.Passport)
                            If oParentPassport IsNot Nothing Then
                                oPassportManager.Delete(oParentPassport, bCallBroadcaster)
                            End If

                            ' Eliminamos las relaciones con el Organigrama
                            Dim DeleteQuerys() As String = {"@DELETE# FROM sysroSecurityNode_Passports WHERE IDPassport=" & _intIDPassport.ToString,
                                                            "@DELETE# FROM sysroSecurityNode_Passports_PermissionsOverEmployeesExceptions WHERE IDPassport=" & _intIDPassport.ToString}

                            For Each strSQLDelete As String In DeleteQuerys
                                bolRet = ExecuteSql(strSQLDelete)
                                If Not bolRet Then Exit For
                            Next

                        End If
                    End If
                End If

                If bolRet Then
                    ' Borro el Empleado de la tabla Employees
                    Dim tbEmployee As New DataTable("Employees")
                    strSQL = "@SELECT# * FROM Employees WHERE [ID] = " & IDEmployee.ToString
                    Dim cmd As DbCommand = CreateCommand(strSQL)
                    Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                    da.Fill(tbEmployee)

                    If tbEmployee.Rows.Count > 0 Then

                        Dim strName As String = tbEmployee.Rows(0).Item("Name")

                        tbEmployee.Rows(0).Delete()
                        da.Update(tbEmployee)

                        ' Auditar borrado
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, strName, Nothing, -1)
                    Else
                        oState.Result = EmployeeResultEnum.EmployeeNotExist
                    End If

                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::DeleteEmployee")

                oState.ErrorNumber = -1
                oState.ErrorText = ""
                oState.ErrorDetail = ""
                oState.Result = EmployeeResultEnum.Exception
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::DeleteEmployee")

                oState.ErrorNumber = -1
                oState.ErrorText = ""
                oState.ErrorDetail = ""
                oState.Result = EmployeeResultEnum.Exception
                bolRet = False
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                ' Borrado de fotos de Azure
                If lPunchesWithPhotoToDelete IsNot Nothing AndAlso lPunchesWithPhotoToDelete.Count > 0 Then
                    For Each idPunch As Integer In lPunchesWithPhotoToDelete
                        Azure.RoAzureSupport.DeletePunchPhotoFile(roTypes.Any2Integer(idPunch))
                    Next
                End If
            End Try

            Return (oState.Result = EmployeeResultEnum.NoError)

        End Function

        Private Shared Function GetNextIDEmployee() As Integer
            ' Recupera el siguiente codigo de employee a usar

            Dim intNextID As Integer = -1

            Dim strQuery As String = " @SELECT# Max(ID) as Contador From Employees "
            Dim tb As DataTable = CreateDataTable(strQuery, )
            Dim rd As DbDataReader = tb.CreateDataReader

            If rd IsNot Nothing Then
                rd.Read()
                If rd.HasRows Then
                    If Not IsDBNull(rd("Contador")) Then
                        intNextID = rd("Contador") + 1
                    Else
                        intNextID = 1
                    End If
                End If
            End If
            rd.Close()

            ' Evitamos el 9999 porque es un usuario reservado para administración de terminales ...
            If intNextID = 9999 Then intNextID = 10000

            Return intNextID

        End Function

        ''' <summary>
        ''' Graba varios empleados.
        ''' </summary>
        ''' <param name="tbEmployeesData">DataTable con el nombre (EmployeeName - String), código tarjeta (IDCard - Long), código contrato (IDContract - String), fecha inicio (BeginDate - DateTime), tipo empleado ('A', 'J') (EmployeeType - String), código grupo (IDGroup - Integer), ficha con trajeta (CardMethod - Boolean), ficha con huella (BiometricMethod - Boolean), combinación métodos (MergeMethod - Integer)   </param>
        ''' <param name="_State"></param>
        ''' <param name="oActiveTransaction"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function CreateMultiEmployees(ByVal tbEmployeesData As DataTable, ByRef _State As roEmployeeState, ByRef lstEmployeeNameError As List(Of String)) As Boolean

            Dim bolRet As Boolean = False
            Dim intPassportGroup As Integer = 0

            Try

                lstEmployeeNameError = New List(Of String)

                Dim oNewEmployee As roEmployee
                Dim oMobility As roMobility
                Dim oContract As Contract.roContract

                Dim oSettings As New roSettings()
                Dim strLanguageKey = roTypes.Any2String(oSettings.GetVTSetting(eKeys.DefaultLanguage))
                For Each oRow As DataRow In tbEmployeesData.Rows

                    Dim bHaveToClose As Boolean = False
                    Try
                        bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                        bolRet = False

                        ' Crear nuevo empleado
                        oNewEmployee = New roEmployee
                        With oNewEmployee
                            .ID = -1
                            .Name = oRow("EmployeeName").ToString.Trim
                            .Type = oRow("EmployeeType")
                            If oRow.Table.Columns.Contains("AccControlled") Then
                                .AccControlled = roTypes.Any2Boolean(oRow("AccControlled"))
                            End If
                            If oRow.Table.Columns.Contains("RiskControlled") Then
                                .RiskControlled = False 'Any2Boolean(oRow("RiskControlled"))
                            End If
                            .EmployeeAlias = ""
                        End With
                        'Me.MediosAcceso.SetIdentifyMethodsType(oEmployee)

                        If roEmployee.SaveEmployee(oNewEmployee, _State, False) Then

                            'Insertamos al empleado en la tabla de Empleados y grupos
                            oMobility = New roMobility
                            oMobility.BeginDate = CDate(oRow("BeginDate"))
                            oMobility.EndDate = New DateTime(2079, 1, 1)
                            oMobility.IdGroup = oRow("IDGroup")
                            oMobility.IsTransfer = True

                            If roMobility.SaveMobility(oNewEmployee.ID, oMobility, _State, True, True) Then

                                ' Audit
                                ' ...

                                'Insertamos el empleado en la tabla de contratos
                                oContract = New Contract.roContract
                                With oContract
                                    .IDEmployee = oNewEmployee.ID
                                    .IDContract = oRow("IDContract")
                                    .BeginDate = CDate(oRow("BeginDate"))
                                    .EndDate = New DateTime(2079, 1, 1)
                                    .IDCard = Nothing
                                    If Not IsDBNull(oRow("IDLabAgree")) Then
                                        Dim oLabAgreeState As New LabAgree.roLabAgreeState()
                                        roBusinessState.CopyTo(_State, oLabAgreeState)
                                        Dim oLabAgree As New LabAgree.roLabAgree(oRow("IDLabAgree"), oLabAgreeState)
                                        If oLabAgree IsNot Nothing Then .LabAgree = oLabAgree
                                    End If

                                End With

                                If oContract.Save() Then
                                    Dim oLanguageManager As New roLanguageManager()
                                    Dim oLng As roPassportLanguage = oLanguageManager.LoadByKey(oSettings.GetVTSetting(eKeys.DefaultLanguage))


                                    Dim oSecurityState As New roSecurityState(_State.IDPassport, _State.Context)
                                    Dim oPassportManager As New roPassportManager(oSecurityState)
                                    ' Actualizamos el passport asociado al empleado
                                    Dim oPassport As roPassport = oPassportManager.LoadPassport(oNewEmployee.ID, LoadType.Employee)
                                    If oPassport Is Nothing Then
                                        oPassport = New roPassport()
                                        oPassport.IDEmployee = oNewEmployee.ID
                                        oPassport.GroupType = "E"
                                        oPassport.Language = oLng
                                        oPassport.State = 1
                                    End If
                                    oPassport.Name = oNewEmployee.Name

                                    Dim oMethod As roPassportAuthenticationMethodsRow
                                    If roTypes.Any2Boolean(oRow("CardMethod")) Then
                                        oMethod = New roPassportAuthenticationMethodsRow() 'oPassport.AuthenticationMethods.CardRows.Table.NewsysroPassports_AuthenticationMethodsRow
                                        With oMethod
                                            .RowState = RowState.NewRow
                                            .IDPassport = oPassport.ID
                                            .Method = AuthenticationMethod.Card
                                            .Version = ""
                                            .BiometricID = 0
                                            .Credential = oRow("IDCard")
                                            .Password = ""
                                            .Enabled = True
                                        End With
                                        oPassport.AuthenticationMethods().CardRows = {oMethod}
                                    End If
                                    If roTypes.Any2Boolean(oRow("BiometricMethod")) Then
                                        oMethod = New roPassportAuthenticationMethodsRow()
                                        With oMethod
                                            .RowState = RowState.NewRow
                                            .IDPassport = oPassport.ID
                                            .Method = AuthenticationMethod.Biometry
                                            .Version = "RXA200"
                                            .BiometricID = 0
                                            .Credential = ""
                                            .Password = ""
                                            .BiometricData = {}
                                            .Enabled = True
                                        End With
                                        oPassport.AuthenticationMethods().BiometricRows = {oMethod}
                                    End If

                                    'If Not IsDBNull(oRow("MergeMethod")) Then
                                    '    Dim _AuthenticationMerge(0) As AuthenticationMethodMerge
                                    '    _AuthenticationMerge(0) = oRow("MergeMethod")
                                    '    oPassport.AuthenticationMerge = _AuthenticationMerge
                                    'End If

                                    oPassportManager.Save(oPassport, False)
                                    intPassportGroup = roTypes.Any2Integer(oRow("PassportGroup"))
                                    If intPassportGroup > 0 Then
                                        ' EN el caso que nos pasen grupo de usuario y sea seguridad v2, significa
                                        ' que ese empleado tambien es supervisor, por lo que hay que crearle un grupo de usuario propio

                                        Dim oParentPassport As New roPassport()
                                        oParentPassport.Name = "Group " & oPassport.Name
                                        oParentPassport.GroupType = "U"
                                        oParentPassport.State = 1
                                        oParentPassport.EnabledVTDesktop = True
                                        oParentPassport.EnabledVTPortal = True
                                        oParentPassport.EnabledVTPortalApp = True
                                        oParentPassport.EnabledVTVisits = True
                                        oParentPassport.EnabledVTVisitsApp = True
                                        oParentPassport.LocationRequiered = True
                                        oParentPassport.PhotoRequiered = True

                                        oPassportManager.Save(oParentPassport, False)

                                        ' Marcamos el passport como supervisor
                                        ExecuteSql("@UPDATE# sysroPassports SET IsSupervisor=1 WHERE ID = " & oPassport.ID)
                                    End If

                                    ' Ejecutamos la regla de bot del tipo copiar permisos y funcionalidades en caso que este activa
                                    If oSecurityState.Result = SecurityResultEnum.NoError Then
                                        Dim oLicense As New roServerLicense
                                        Dim bolIsInstalledBots As Boolean = oLicense.FeatureIsInstalled("Feature\BotsPremium")

                                        If bolIsInstalledBots Then
                                            Try
                                                Dim oBotState As New roBotState(-1)
                                                Dim oBotManager As New roBotManager(oBotState)
                                                Dim _oParameters As New Dictionary(Of BotRuleParameterEnum, String)
                                                _oParameters.Add(BotRuleParameterEnum.DestinationEmployee, oNewEmployee.ID.ToString)
                                                oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopyEmployeePermissions, _oParameters)
                                                oBotManager.ExecuteRulesByType(BotRuleTypeEnum.CopyEmployeeUserFields, _oParameters)
                                            Catch ex As Exception
                                                roLog.GetInstance().logMessage(roLog.EventType.roDebug, "Creating Employee " & oNewEmployee.Name & ": Error executing bot", ex)
                                            End Try
                                        End If
                                    End If

                                    bolRet = (oSecurityState.Result = SecurityResultEnum.NoError)
                                    If Not bolRet Then
                                        _State.ErrorText = oSecurityState.ErrorText
                                    End If

                                    'Si el empleado es de prodicción, insertamos al empleado en la tabla de
                                    'grupos. Lo asignamos al grupo por defecto (1)
                                    'Insertamos al empleado en la tabla de Empleados y grupos

                                    ' ...

                                End If

                            End If

                        End If
                    Catch ex As DbException
                        _State.UpdateStateInfo(ex, "roEmployees::CreateMultiEmployees EmployeeName='" & oRow("EmployeeName") & "'")
                    Catch ex As Exception
                        _State.UpdateStateInfo(ex, "roEmployees::CreateMultiEmployees EmployeeName='" & oRow("EmployeeName") & "'")
                    Finally
                        ' Validamos la transacción por cada empleado
                        Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
                    End Try

                    If Not bolRet Then
                        lstEmployeeNameError.Add(oRow("EmployeeName"))
                    End If

                Next
            Catch ex As DbException
                _State.UpdateStateInfo(ex, "roEmployees::CreateMultiEmployees")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roEmployees::CreateMultiEmployees")
            End Try

            Return (lstEmployeeNameError.Count = 0)

        End Function

        Public Shared Function DeleteEmployeeRelatedData(ByVal IDEmployee As Integer, ByVal bRemoveEmployeePhoto As Boolean, ByVal bRemoveBiometricData As Boolean, ByVal bRemoveEmployeePunchImages As Boolean, ByRef oState As roEmployeeState) As Boolean

            Dim bolRet As Boolean = True

            Try
                If bRemoveBiometricData Then
                    Dim oPassportManager As New roPassportManager()
                    Dim oPassport As roPassport = oPassportManager.LoadPassport(IDEmployee, LoadType.Employee)
                    For Each oBiometricRow As roPassportAuthenticationMethodsRow In oPassport.AuthenticationMethods.BiometricRows
                        oBiometricRow.RowState = RowState.DeleteRow
                    Next

                    bolRet = oPassportManager.Save(oPassport, False)
                    If bolRet Then
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Eliminar datos biométricos", Nothing, -1)
                    End If
                End If

                If bolRet AndAlso bRemoveEmployeePhoto Then

                    Dim strSQL As String = "@UPDATE# employees Set [IMAGE] = NULL WHERE id = " & IDEmployee

                    bolRet = ExecuteSql(strSQL)

                    If bolRet Then
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Eliminar foto de empleado", Nothing, -1)
                    End If
                End If

                If bolRet AndAlso bRemoveEmployeePunchImages Then

                    Dim strDeleteSQL As String = "@DELETE# FROM PunchesCaptures where IDPunch in ( " &
                                       "@SELECT# top 100 IDPunch from PunchesCaptures inner join Punches on PunchesCaptures.IDPunch = Punches.ID and IDEmployee = " & IDEmployee & ")"

                    Dim strCountSQL As String = "@SELECT# count(*) from PunchesCaptures inner join Punches on PunchesCaptures.IDPunch = Punches.ID and IDEmployee = " & IDEmployee

                    While (roTypes.Any2Integer(ExecuteScalar(strCountSQL)) > 0 AndAlso bolRet)
                        bolRet = ExecuteSql(strDeleteSQL)
                    End While

                    ' Fotos en Azure
                    Dim strSQL As String = String.Empty
                    strSQL = "@SELECT# ID FROM Punches WHERE PhotoOnAzure = 1 AND IDEmployee = " & IDEmployee
                    Dim tb As DataTable = CreateDataTable(strSQL)
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oPunchRow As DataRow In tb.Rows
                            Azure.RoAzureSupport.DeletePunchPhotoFile(roTypes.Any2Integer(oPunchRow("ID")))
                        Next
                    End If

                    If bolRet Then
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Eliminar fotos de fichajes del empleado", Nothing, -1)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::DeleteBiometricData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::DeleteBiometricData")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteBiometricData(ByVal IDEmployee As Integer, ByRef oState As roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oPassportManager As New roPassportManager()
                Dim oPassport As roPassport = oPassportManager.LoadPassport(IDEmployee, LoadType.Employee)
                oPassport.AuthenticationMethods.BiometricRows = {}

                bolRet = oPassportManager.Save(oPassport,)
                If bolRet Then
                    'Auditar eliminacion de datos
                    oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Eliminar datos biométricos", Nothing, -1)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::DeleteBiometricData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::DeleteBiometricData")
            End Try

            Return bolRet

        End Function

        Public Shared Function DeleteBiometricDataForAllEmployees(ByRef oState As roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@DELETE# from sysroPassports_AuthenticationMethods where Method = 4 and version <> 'RXA200'"

                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                If bolRet Then
                    'Auditar eliminacion de datos
                    oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Eliminación de datos biométricos de todos los empleados", Nothing, -1)
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::DeleteBiometricDataForAllEmployees")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::DeleteBiometricDataForAllEmployees")
            End Try

            Return bolRet

        End Function

        Public Shared Function EnableOrDisableBiometricData(ByVal idEmployee As Integer, ByVal disabled As Boolean, ByRef oState As roEmployeeState, Optional bRunBroadCaster As Boolean = True) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oPassportManager As New roPassportManager()
                Dim oPassport As roPassport = oPassportManager.LoadPassport(idEmployee, LoadType.Employee)

                Dim strSQL As String = "@UPDATE# sysroPassports_AuthenticationMethods SET Enabled = " & Convert.ToInt32(Not disabled) & " where IDPassport = " & oPassport.ID & " and Method = 4"

                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                If bolRet Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{EmployeeName}", oPassport.Name, "", 1)
                    oState.AddAuditParameter(tbParameters, "{Status}", roTypes.Any2Integer(Not disabled), "", 1)
                    oState.Audit(Audit.Action.aUpdate, Audit.ObjectType.tEmployee, oPassport.Name, tbParameters, -1)
                    If bRunBroadCaster Then
                        roConnector.InitTask(TasksType.BROADCASTER)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::EnableOrDisableBiometricData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::EnableOrDisableBiometricData")
            End Try

            Return bolRet

        End Function

        Public Shared Function DisableBiometricDataForAllEmployees(ByVal disabled As Boolean, ByRef oState As roEmployeeState) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = "@UPDATE# sysroPassports_AuthenticationMethods SET Enabled = " & roTypes.Any2Integer(Not disabled) & " where Method = 4"

                bolRet = ExecuteSqlWithoutTimeOut(strSQL)
                If bolRet Then
                    'Auditar eliminacion de datos
                    If disabled Then
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Se deshabilita el fichaje biométrico para todos los empleados", Nothing, -1)
                    Else
                        oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tEmployee, "Se habilita el fichaje biométrico para todos los empleados", Nothing, -1)
                    End If
                    roConnector.InitTask(TasksType.BROADCASTER)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::EnableOrDisableBiometricDataForAllEmployees")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::EnableOrDisableBiometricDataForAllEmployees")
            End Try

            Return bolRet

        End Function

        Public Shared Function GetEmployeeWorkcenterAtDate(idEmployee As Integer, dDate As Date, ByRef oState As roEmployeeState)
            Dim sRet As String = String.Empty
            Dim sSQL As String = String.Empty

            Try

                sSQL = "@SELECT# WorkCenter FROM DailySchedule WHERE IdEmployee = " & idEmployee.ToString & " AND Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime
                sRet = roTypes.Any2String(ExecuteScalar(sSQL)).Trim

                If sRet.Length = 0 Then
                    Dim oCurrentContract As Contract.roContract
                    oCurrentContract = Contract.roContract.GetContractInDate(idEmployee, dDate, New Contract.roContractState(-1), False)
                    If Not oCurrentContract Is Nothing Then
                        sRet = oCurrentContract.Enterprise
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeWorkcenterAtDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeWorkcenterAtDate")
            End Try

            Return sRet
        End Function

        Public Shared Function SetEmployeeWorkcenterAtDate(idEmployee As Integer, dDate As Date, sWorkCenter As String, ByRef oState As roEmployeeState)
            Dim bRet As Boolean = False
            Dim sSQL As String = String.Empty

            Try

                sSQL = "@UPDATE# DailySchedule WITH (ROWLOCK) SET WorkCenter = '" & sWorkCenter & "' WHERE IdEmployee = " & idEmployee.ToString & " AND Date = " & roTypes.Any2Time(dDate).SQLSmallDateTime
                bRet = ExecuteSql(sSQL)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::SetEmployeeWorkcenterAtDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::SetEmployeeWorkcenterAtDate")
            End Try

            Return bRet
        End Function

        Public Shared Sub GetEmployeeTelecommutingDataOnDate(ByVal dDate As Date,
                                                             ByVal idEmployee As Integer,
                                                             ByRef oState As roEmployeeState,
                                                             Optional ByRef bEmployeeShouldTelecommute As Boolean = False,
                                                             Optional ByRef sEmployeeTelecommuteMandatoryDays As String = "",
                                                             Optional ByRef sEmployeeTelecommuteOptionalDays As String = "",
                                                             Optional ByRef iEmployeeTelecommuteMaxDays As Integer = -1,
                                                             Optional ByRef iEmployeeTelecommuteMaxPercentage As Integer = -1,
                                                             Optional ByRef iEmployeeTelecommutePeriodType As Integer = -1,
                                                             Optional ByRef sEmployeePresenceMandatoryDays As String = Nothing,
                                                             Optional ByRef bHasTelecommuteAgreementOnDate As Boolean = False)

            Try

                Dim strSQL As String
                strSQL = "@SELECT# * FROM sysrovwTelecommutingAgreement WHERE IdEmployee = " & idEmployee.ToString &
                     " AND " & roTypes.Any2Time(dDate).SQLSmallDateTime & " BETWEEN TelecommutingAgreementStart AND TelecommutingAgreementEnd " &
                     " AND " & roTypes.Any2Time(dDate).SQLSmallDateTime & " BETWEEN ContractStart AND ContractEnd "
                Dim tbAux As DataTable = CreateDataTable(strSQL)
                If Not tbAux Is Nothing AndAlso tbAux.Rows.Count > 0 Then
                    bEmployeeShouldTelecommute = roTypes.Any2Boolean(tbAux.Rows(0)("Telecommuting"))
                    sEmployeeTelecommuteMandatoryDays = roTypes.Any2String(tbAux.Rows(0)("TelecommutingMandatoryDays"))
                    If Not sEmployeePresenceMandatoryDays Is Nothing Then
                        sEmployeePresenceMandatoryDays = roTypes.Any2String(tbAux.Rows(0)("PresenceMandatoryDays"))
                    End If
                    sEmployeeTelecommuteOptionalDays = roTypes.Any2String(tbAux.Rows(0)("TelecommutingOptionalDays"))
                    iEmployeeTelecommuteMaxDays = roTypes.Any2Integer(tbAux.Rows(0)("TelecommutingMaxDays"))
                    iEmployeeTelecommuteMaxPercentage = roTypes.Any2Integer(tbAux.Rows(0)("TelecommutingMaxPercentage"))
                    iEmployeeTelecommutePeriodType = roTypes.Any2Integer(tbAux.Rows(0)("PeriodType"))
                    bHasTelecommuteAgreementOnDate = True
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeTelecommutingDataOnDate")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roEmployees::GetEmployeeTelecommutingDataOnDate")
            End Try
        End Sub

#End Region

    End Class

End Namespace