Imports System.Data.Common
Imports System.Data.SqlClient
Imports System.Data.SqlTypes
Imports System.Net
Imports System.Security.Cryptography
Imports System.Text
Imports System.Web.UI.WebControls
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks
Imports Robotics.VTBase.roTypes
Imports Robotics.VTBase.Scheduler

Namespace Base

    Public Class roPassportManager
        Private oState As roPassportState = Nothing

        Public ReadOnly Property State As roPassportState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roPassportState()
        End Sub

        Public Sub New(ByVal IdPassport As Integer)
            Me.oState = New roPassportState(IdPassport)
        End Sub

        Public Sub New(ByVal _State As roPassportState)
            If _State Is Nothing Then _State = New roPassportState()

            Me.oState = _State
        End Sub

        Public Sub New(ByVal _State As roSecurityState)
            If _State Is Nothing Then _State = New roSecurityState()

            Dim oPassportState As New roPassportState()
            roBusinessState.CopyTo(_State, oPassportState)

            Me.oState = oPassportState
        End Sub

#Region "Methods"

        Public Function LoadPassport(ByVal _ID As Integer, ByVal loadType As LoadType, Optional ByVal bAudit As Boolean = False) As roPassport

            Dim oRet As roPassport = Nothing

            Try

                Dim strSQL As String = "@SELECT# Id,IDGroupFeature From sysroPassports WHERE"

                ' Cargamos el passport en función del identificador
                Select Case loadType
                    Case LoadType.Passport
                        strSQL += " ID=" & _ID.ToString
                    Case LoadType.Employee
                        strSQL += " IDEmployee=" & _ID.ToString
                    Case LoadType.User
                        strSQL += " IDUser=" & _ID.ToString
                End Select

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Cargamos datos generales
                    oRet = New roPassport

                    oRet = LoadMainData(roTypes.Any2Integer(dTbl.Rows(0)("Id")))
                    If oRet IsNot Nothing Then
                        ' Cargamos los metodos de autentificacion
                        oRet.AuthenticationMethods = LoadAuthenticationMethods(oRet)
                        ' Rol asignado
                        oRet.IDGroupFeature = If(IsDBNull(dTbl.Rows(0)("IDGroupFeature")), 0, dTbl.Rows(0)("IDGroupFeature"))

                        If oRet.IsSupervisor Then
                            oRet.Categories = LoadCategories(oRet)
                            oRet.Groups = LoadGroups(oRet)
                            oRet.Exceptions = LoadExceptions(oRet)
                        Else
                            oRet.Categories = New roPassportCategories With {.idPassport = oRet.ID, .CategoryRows = Nothing}
                            oRet.Groups = New roPassportGroups With {.idPassport = oRet.ID, .GroupRows = Nothing}
                            oRet.Exceptions = New roPassportExceptions With {.idPassport = oRet.ID, .Exceptions = {}}
                        End If
                        oRet = SetComputedProperties(oRet)
                    End If
                End If

                ' Auditar lectura
                If bAudit AndAlso oRet IsNot Nothing Then
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tPassport, oRet.Name, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::Load")
            End Try

            Return oRet

        End Function

        Private Shared Function SetComputedProperties(oRet As roPassport) As roPassport

            'Seteamos la propiedad global de passport activo
            If ((oRet.StartDate.HasValue AndAlso oRet.StartDate.Value.Date <= Now.Date) OrElse Not oRet.StartDate.HasValue) AndAlso
               ((oRet.ExpirationDate.HasValue AndAlso oRet.ExpirationDate.Value.Date >= Now.Date) OrElse Not oRet.ExpirationDate.HasValue) AndAlso
               (oRet.State.HasValue AndAlso oRet.State.Value = 1) Then
                oRet.IsActivePassport = True
            Else
                oRet.IsActivePassport = False
            End If

            'Seteamos la propiedad global de acceso bloqueado.
            oRet.IsBloquedAccessApp = False
            If oRet.AuthenticationMethods.PasswordRow IsNot Nothing Then
                oRet.IsBloquedAccessApp = oRet.AuthenticationMethods.PasswordRow.BloquedAccessApp
            End If

            'Seteamos si la contraseña ha caducado.
            oRet.IsExpiredPwd = False
            If oRet.AuthenticationMethods.PasswordRow IsNot Nothing Then
                If oRet.AuthenticationMethods.PasswordRow.Credential.Contains("\") Then
                    oRet.IsExpiredPwd = False
                Else
                    oRet.IsExpiredPwd = False
                    Dim intDaysPasswordExpired = AuthHelper.DaysPasswordExpired(0)

                    If intDaysPasswordExpired > 0 Then
                        Dim oLastUpdatePassword As DateTime = Now

                        If oRet.AuthenticationMethods.PasswordRow.LastUpdatePassword <> Date.MinValue Then
                            oLastUpdatePassword = oRet.AuthenticationMethods.PasswordRow.LastUpdatePassword
                        End If

                        ' Si han pasado mas dias , la contraseña ha caducado
                        If DateDiff(DateInterval.Day, oLastUpdatePassword, Now) >= intDaysPasswordExpired Then
                            oRet.IsExpiredPwd = True
                        End If
                    Else
                        ' Miramos si se ha regenerado la contraseña
                        Dim oLastUpdatePassword As DateTime = Now

                        If oRet.AuthenticationMethods.PasswordRow.LastUpdatePassword <> Date.MinValue Then
                            oLastUpdatePassword = oRet.AuthenticationMethods.PasswordRow.LastUpdatePassword
                            Dim oExpiredDate As New DateTime(1900, 1, 1)
                            If oLastUpdatePassword = oExpiredDate Then
                                oRet.IsExpiredPwd = True
                            End If
                        End If
                    End If
                End If
            End If


            'Seteamos la propiedad global de temporalmente bloqueado
            oRet.IsTemporayBloqued = False
            If oRet.AuthenticationMethods.PasswordRow IsNot Nothing Then
                Dim iInvalidAttemps As Integer = AuthHelper.AccessAttempsTemporaryBlock(0)
                If oRet.AuthenticationMethods.PasswordRow.InvalidAccessAttemps >= iInvalidAttemps Then
                    Dim oDate As DateTime = Nothing

                    If oRet.AuthenticationMethods.PasswordRow.LastDateInvalidAccessAttempted <> Date.MinValue Then
                        oDate = oRet.AuthenticationMethods.PasswordRow.LastDateInvalidAccessAttempted
                    End If

                    ' Si han pasado menos de 10 minutos esta temporalmente bloqueada
                    If oDate <> Nothing AndAlso DateDiff(DateInterval.Minute, oDate, Now) < 10 Then
                        oRet.IsTemporayBloqued = True
                    End If
                End If
            End If

            'Seteamos la propiedad global de pasaporte de grupo de empleado
            oRet.IsEmployeesGroup = False
            If oRet.GroupType = "E" AndAlso Not oRet.IDEmployee.HasValue Then
                oRet.IsEmployeesGroup = True
            End If

            Return oRet
        End Function

        Public Function LoadSupervisor(ByVal _ID As Integer, ByVal loadType As LoadType, Optional ByVal bAudit As Boolean = False) As roSupervisor
            Dim oRet As roSupervisor = Nothing

            Try
                Dim strSQL As String = "@SELECT# Id,IDGroupFeature From sysroPassports WHERE"

                ' Cargamos el passport en función del identificador
                Select Case loadType
                    Case LoadType.Passport
                        strSQL += " ID=" & _ID.ToString
                    Case LoadType.Employee
                        strSQL += " IDEmployee=" & _ID.ToString
                    Case LoadType.User
                        strSQL += " IDUser=" & _ID.ToString
                End Select

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Cargamos datos generales
                    oRet = New roSupervisor()

                    ' Get the passport ID
                    Dim passportId As Integer = roTypes.Any2Integer(dTbl.Rows(0)("Id"))

                    ' Load the main passport data
                    LoadMainDataForSupervisor(passportId, oRet)

                    ' Load categories, groups, and exceptions
                    LoadCategoriesForSupervisor(passportId, oRet)
                    LoadGroupsForSupervisor(passportId, oRet)
                    LoadExceptionsForSupervisor(passportId, oRet)

                    ' Load authentication methods to get username
                    LoadAuthenticationMethodsForSupervisor(passportId, oRet)
                End If

                ' Auditar lectura
                If bAudit AndAlso oRet IsNot Nothing Then
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tPassport, oRet.Name, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadSupervisor")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadSupervisor")
            End Try

            Return oRet
        End Function

        Private Sub LoadMainDataForSupervisor(ByVal _IDPassport As Integer, ByRef oRet As roSupervisor)
            Try
                Dim strSQL As String = "@SELECT# * From sysroPassports WHERE ID=" & _IDPassport.ToString

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Basic properties
                    oRet.Name = Any2String(dTbl.Rows(0)("Name"))
                    oRet.Description = Any2String(dTbl.Rows(0)("Description"))
                    oRet.RoleID = If(IsDBNull(dTbl.Rows(0)("IDGroupFeature")) OrElse Any2Integer(dTbl.Rows(0)("IDGroupFeature")) = 0,
                             Nothing,
                             Any2Integer(dTbl.Rows(0)("IDGroupFeature")).ToString())
                    oRet.Email = Any2String(dTbl.Rows(0)("Email"))

                    ' Get language
                    Dim oLanguageManager As New roLanguageManager(Me.State)
                    Dim language = oLanguageManager.LoadById(Any2Integer(dTbl.Rows(0)("IDLanguage")))
                    oRet.Language = If(language Is Nothing, "es", language.Key)

                    ' Date fields 
                    If Not IsDBNull(dTbl.Rows(0)("StartDate")) Then
                        Dim startDate As DateTime = CType(dTbl.Rows(0)("StartDate"), DateTime)
                        oRet.ValidityStartDate = New Robotics.VTBase.roWCFDate(startDate)
                    End If

                    If Not IsDBNull(dTbl.Rows(0)("ExpirationDate")) Then
                        Dim endDate As DateTime = CType(dTbl.Rows(0)("ExpirationDate"), DateTime)
                        oRet.ValidityEndDate = New Robotics.VTBase.roWCFDate(endDate)
                    End If

                    ' Active status
                    Dim isActive As Boolean = False
                    If ((Not IsDBNull(dTbl.Rows(0)("StartDate")) AndAlso dTbl.Rows(0)("StartDate") <= Now.Date) OrElse IsDBNull(dTbl.Rows(0)("StartDate"))) AndAlso
               ((Not IsDBNull(dTbl.Rows(0)("ExpirationDate")) AndAlso dTbl.Rows(0)("ExpirationDate") >= Now.Date) OrElse IsDBNull(dTbl.Rows(0)("ExpirationDate"))) AndAlso
               (Not IsDBNull(dTbl.Rows(0)("State")) AndAlso Any2Integer(dTbl.Rows(0)("State")) = 1) Then
                        isActive = True
                    End If
                    oRet.IsActive = isActive
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::LoadMainDataForSupervisor")
            End Try
        End Sub


        Private Sub LoadAuthenticationMethodsForSupervisor(ByVal _IDPassport As Integer, ByRef oRet As roSupervisor)
            Try
                ' Get username from Password authentication method
                Dim strSQL As String = "@SELECT# Credential From sysroPassports_AuthenticationMethods " &
                              "WHERE IDPassport=" & _IDPassport.ToString & " AND Method = " & AuthenticationMethod.Password

                Dim result As Object = AccessHelper.ExecuteScalar(strSQL)
                If result IsNot Nothing AndAlso Not IsDBNull(result) Then
                    oRet.UserName = result.ToString()
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::LoadAuthenticationMethodsForSupervisor")
            End Try
        End Sub

        Private Sub LoadCategoriesForSupervisor(ByVal _IDPassport As Integer, ByRef oRet As roSupervisor)
            Try
                Dim strSQL As String = "@SELECT# IDCategory, LevelOfAuthority, ShowFromLevel From sysroPassports_Categories " &
                              "WHERE IDPassport=" & _IDPassport.ToString

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    Dim categoryList As New List(Of roSupervisorCategory)

                    For Each row As DataRow In dTbl.Rows
                        Dim category As New roSupervisorCategory With {
                    .CategoryID = CType(row("IDCategory"), Integer).ToString(),
                    .AuthorizationLevel = row("LevelOfAuthority").ToString(),
                    .MinimumSupervisionLevel = row("ShowFromLevel").ToString()
                }
                        categoryList.Add(category)
                    Next

                    oRet.Categories = categoryList.ToArray()
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::LoadCategoriesForSupervisor")
            End Try
        End Sub

        Private Sub LoadGroupsForSupervisor(ByVal _IDPassport As Integer, ByRef oRet As roSupervisor)
            Try
                Dim strSQL As String = "@SELECT# IDGroup From sysroPassports_Groups " &
                              "WHERE IDPassport=" & _IDPassport.ToString

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    Dim groupList As New List(Of String)

                    For Each row As DataRow In dTbl.Rows
                        groupList.Add(row("IDGroup").ToString())
                    Next

                    oRet.SupervisedGroupIDs = groupList.ToArray()
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::LoadGroupsForSupervisor")
            End Try
        End Sub

        Private Sub LoadExceptionsForSupervisor(ByVal _IDPassport As Integer, ByRef oRet As roSupervisor)
            Try
                Dim strSQL As String = "@SELECT# IDEmployee, Permission From sysroPassports_Employees " &
                              "WHERE IDPassport=" & _IDPassport.ToString & " AND Permission=1"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count > 0 Then
                    Dim employeeList As New List(Of String)

                    For Each row As DataRow In dTbl.Rows
                        employeeList.Add(row("IDEmployee").ToString())
                    Next

                    oRet.SupervisedEmployeeIDs = employeeList.ToArray()
                End If
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::LoadExceptionsForSupervisor")
            End Try
        End Sub

        ''' <summary>
        ''' Returns the supervisor with specified ID.
        ''' </summary>
        ''' <param name="id">The ID of the supervisor to load, or the employee or user id.</param>
        Public Shared Function GetSupervisor(ByVal id As Integer, Optional ByRef _State As roSecurityState = Nothing) As roSupervisor
            Try
                Return roPassportManager.GetSupervisor(id, LoadType.Passport, _State)
            Catch ex As Exception
                If _State IsNot Nothing Then
                    _State.Result = SecurityResultEnum.PassportDoesNotExists
                End If
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Returns the supervisor with specified ID.
        ''' </summary>
        ''' <param name="id">The ID of the supervisor to load, or the employee or user id.</param>
        ''' <param name="loadType">The type of ID: passport, employee or user.</param>
        Public Shared Function GetSupervisor(ByVal id As Integer, ByVal loadType As LoadType, Optional ByRef _State As roSecurityState = Nothing) As roSupervisor
            If _State Is Nothing Then _State = New roSecurityState
            Try
                Dim oManager As New roPassportManager(_State)

                Dim oSupervisor As roSupervisor = oManager.LoadSupervisor(id, loadType)
                roBusinessState.CopyTo(oManager.State, _State)
                Return oSupervisor
            Catch ex As Exception
                _State.Result = SecurityResultEnum.PassportDoesNotExists
                Return Nothing
            End Try
        End Function


        Public Function LoadPassportTicket(ByVal _ID As Integer, ByVal loadType As LoadType, Optional ByVal bAudit As Boolean = False) As roPassportTicket

            Dim oRet As roPassportTicket = Nothing

            Try

                Dim strSQL As String = "@SELECT# Id From sysroPassports WHERE"

                ' Cargamos el passport en función del identificador
                Select Case loadType
                    Case LoadType.Passport
                        strSQL += " ID=" & _ID.ToString
                    Case LoadType.Employee
                        strSQL += " IDEmployee=" & _ID.ToString
                    Case LoadType.User
                        strSQL += " IDUser=" & _ID.ToString
                End Select

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Cargamos datos generales
                    oRet = New roPassportTicket

                    oRet = LoadMainDataLite(roTypes.Any2Integer(dTbl.Rows(0)("Id")))

                End If

                ' Auditar lectura
                If bAudit AndAlso oRet IsNot Nothing Then
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tPassport, oRet.Name, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadPassportTicket")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadPassportTicket")
            Finally

            End Try

            Return oRet

        End Function

        Public Function LoadMainData(ByVal _IDPassport As Integer, Optional ByVal bAudit As Boolean = False) As roPassport

            Dim oRet As roPassport = Nothing

            Try

                Dim strSQL As String = "@SELECT# * From sysroPassports WHERE "
                strSQL += " ID=" & _IDPassport.ToString

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Cargamos datos generales
                    oRet = New roPassport

                    oRet.ID = roTypes.Any2Integer(dTbl.Rows(0)("Id"))
                    oRet.IDParentPassport = IIf(IsDBNull(dTbl.Rows(0)("IDParentPassport")), Nothing, Any2Integer(dTbl.Rows(0)("IDParentPassport")))
                    oRet.GroupType = Any2String(dTbl.Rows(0)("GroupType"))
                    oRet.Name = Any2String(dTbl.Rows(0)("Name"))
                    oRet.Description = Any2String(dTbl.Rows(0)("Description"))
                    oRet.Email = Any2String(dTbl.Rows(0)("Email"))
                    oRet.IDUser = IIf(IsDBNull(dTbl.Rows(0)("IDUser")), Nothing, Any2Integer(dTbl.Rows(0)("IDUser")))
                    oRet.IDEmployee = IIf(IsDBNull(dTbl.Rows(0)("IDEmployee")), Nothing, Any2Integer(dTbl.Rows(0)("IDEmployee")))
                    Dim oLanguageManager As New roLanguageManager(Me.State)
                    oRet.Language = oLanguageManager.LoadById(Any2Integer(dTbl.Rows(0)("IDLanguage")))
                    oRet.ConfData = Any2String(dTbl.Rows(0)("ConfData"))
                    oRet.StartDate = IIf(IsDBNull(dTbl.Rows(0)("StartDate")), Nothing, dTbl.Rows(0)("StartDate"))
                    oRet.ExpirationDate = IIf(IsDBNull(dTbl.Rows(0)("ExpirationDate")), Nothing, dTbl.Rows(0)("ExpirationDate"))
                    oRet.State = Any2Integer(dTbl.Rows(0)("State"))
                    oRet.EnabledVTDesktop = Any2Boolean(dTbl.Rows(0)("EnabledVTDesktop"))
                    oRet.EnabledVTPortal = Any2Boolean(dTbl.Rows(0)("EnabledVTPortal"))
                    oRet.EnabledVTPortalApp = Any2Boolean(dTbl.Rows(0)("EnabledVTPortalApp"))
                    oRet.EnabledVTVisits = Any2Boolean(dTbl.Rows(0)("EnabledVTVisits"))
                    oRet.EnabledVTVisitsApp = Any2Boolean(dTbl.Rows(0)("EnabledVTVisitsApp"))
                    oRet.IsSupervisor = Any2Boolean(dTbl.Rows(0)("IsSupervisor"))
                    oRet.LicenseAccepted = Any2Boolean(dTbl.Rows(0)("LicenseAccepted"))
                    oRet.PhotoRequiered = Any2Boolean(dTbl.Rows(0)("PhotoRequiered"))
                    oRet.LocationRequiered = Any2Boolean(dTbl.Rows(0)("LocationRequiered"))
                    oRet.CanApproveOwnRequests = Any2Boolean(dTbl.Rows(0)("CanApproveOwnRequests"))
                    oRet.LoginWithoutContract = Any2Boolean(dTbl.Rows(0)("LoginWithoutContract"))

                End If

                ' Auditar lectura
                If bAudit AndAlso oRet IsNot Nothing Then
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tPassport, oRet.Name, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadMainData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadMainData")
            End Try

            Return oRet

        End Function

        Public Function LoadMainDataLite(ByVal idPassport As Integer, Optional ByVal bAudit As Boolean = False) As roPassportTicket

            Dim oRet As roPassportTicket = Nothing

            Try

                Dim strSQL As String = $"@SELECT# sp.Id, sp.IDParentPassport, sp.Name, sp.Description, sp.Email, sp.IDEmployee, sp.IDUser, sp.State, sp.StartDate, sp.ExpirationDate, sp.IDGroupFeature,
                                            sp.IDLanguage, sp.EnabledVTDesktop, sp.EnabledVTPortal, sp.EnabledVTPortalApp, sp.EnabledVTVisits, sp.EnabledVTVisitsApp, sp.IsSupervisor,
	                                        dbo.GetEmployeeUserFieldValueMin(sp.IDEmployee,ISNULL((@select# FieldName from sysroUserFields where Alias = 'sysroEmail'), 'Correo electrónico'),getdate()) as EmpEmail,
	                                        ISNULL(auth.BloquedAccessApp,1) AS BloquedAccessApp,
	                                        ISNULL(auth.Credential,'') As AuthCredential,
	                                        ISNULL(cegidID.Credential,'') As CegidIDCredential,
	                                        auth.InvalidAccessAttemps,auth.LastDateInvalidAccessAttempted, auth.LastUpdatePassword,
	                                        secpar.AccessAttempsTemporaryBlock,secpar.DaysPasswordExpired
	                                    From sysroSecurityParameters secpar, sysroPassports sp
	                                        LEFT JOIN sysroPassports_AuthenticationMethods auth ON auth.IDPassport = sp.ID and auth.Method = 1
	                                        LEFT JOIN sysroPassports_AuthenticationMethods cegidID ON cegidID.IDPassport = sp.ID and cegidID.Method = 2
                                        WHERE secpar.IDPassport = 0 and sp.ID={idPassport.ToString}"

                Dim dTbl As System.Data.DataTable = CreateDataTable(strSQL)
                If dTbl IsNot Nothing AndAlso dTbl.Rows.Count = 1 Then
                    ' Cargamos datos generales
                    oRet = New roPassportTicket

                    oRet.ID = roTypes.Any2Integer(dTbl.Rows(0)("Id"))
                    oRet.IDParentPassport = IIf(IsDBNull(dTbl.Rows(0)("IDParentPassport")), Nothing, Any2Integer(dTbl.Rows(0)("IDParentPassport")))
                    oRet.Name = Any2String(dTbl.Rows(0)("Name"))
                    oRet.Description = Any2String(dTbl.Rows(0)("Description"))
                    oRet.Email = IIf(Any2String(dTbl.Rows(0)("Email")).Length > 0, Any2String(dTbl.Rows(0)("Email")), Any2String(dTbl.Rows(0)("EmpEmail")))
                    oRet.IDUser = IIf(IsDBNull(dTbl.Rows(0)("IDUser")), Nothing, Any2Integer(dTbl.Rows(0)("IDUser")))
                    oRet.IDEmployee = IIf(IsDBNull(dTbl.Rows(0)("IDEmployee")), Nothing, Any2Integer(dTbl.Rows(0)("IDEmployee")))
                    oRet.EnabledVTDesktop = Any2Boolean(dTbl.Rows(0)("EnabledVTDesktop"))
                    oRet.EnabledVTPortal = Any2Boolean(dTbl.Rows(0)("EnabledVTPortal"))
                    oRet.EnabledVTPortalApp = Any2Boolean(dTbl.Rows(0)("EnabledVTPortalApp"))
                    oRet.EnabledVTVisits = Any2Boolean(dTbl.Rows(0)("EnabledVTVisits"))
                    oRet.EnabledVTVisitsApp = Any2Boolean(dTbl.Rows(0)("EnabledVTVisitsApp"))
                    oRet.IDGroupFeature = If(IsDBNull(dTbl.Rows(0)("IDGroupFeature")), 0, dTbl.Rows(0)("IDGroupFeature"))
                    oRet.IsSupervisor = Any2Boolean(dTbl.Rows(0)("IsSupervisor"))
                    oRet.IsBloquedAccessApp = Any2Boolean(dTbl.Rows(0)("BloquedAccessApp"))
                    oRet.IsSSO = Any2String(dTbl.Rows(0)("AuthCredential")).IndexOf("\") >= 0
                    oRet.AuthCredential = Any2String(dTbl.Rows(0)("AuthCredential"))
                    oRet.CegidIdCredential = Any2String(dTbl.Rows(0)("CegidIDCredential"))
                    Dim oLanguageManager As New roLanguageManager(Me.State)
                    oRet.Language = oLanguageManager.LoadById(Any2Integer(dTbl.Rows(0)("IDLanguage")))

                    oRet.IsExpiredPwd = CheckIfPasswordIsExpired(oRet.IsSSO, dTbl)
                    oRet.IsTemporayBloqued = CheckIfPasswordIsTemporaryBloqued(dTbl)
                    oRet.IsActivePassport = CheckIfPassportIsActive(dTbl)

                End If

                ' Auditar lectura
                If bAudit AndAlso oRet IsNot Nothing Then
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tPassport, oRet.Name, Nothing, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadMainDataLite")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadMainDataLite")
            End Try

            Return oRet

        End Function

        Private Shared Function CheckIfPassportIsActive(dTbl As DataTable) As Boolean
            If ((Not IsDBNull(dTbl.Rows(0)("StartDate")) AndAlso dTbl.Rows(0)("StartDate") <= Now.Date) OrElse IsDBNull(dTbl.Rows(0)("StartDate"))) AndAlso
                                   ((Not IsDBNull(dTbl.Rows(0)("ExpirationDate")) AndAlso dTbl.Rows(0)("ExpirationDate") >= Now.Date) OrElse IsDBNull(dTbl.Rows(0)("ExpirationDate"))) AndAlso
                                   (Not IsDBNull(dTbl.Rows(0)("State")) AndAlso Any2Integer(dTbl.Rows(0)("State")) = 1) Then
                Return True
            End If

            Return False
        End Function

        Private Shared Function CheckIfPasswordIsTemporaryBloqued(dTbl As DataTable) As Boolean
            Dim iInvalidAttemps As Integer = Any2Integer(dTbl.Rows(0)("AccessAttempsTemporaryBlock"))
            If Not IsDBNull(dTbl.Rows(0)("InvalidAccessAttemps")) AndAlso roTypes.Any2Integer(dTbl.Rows(0)("InvalidAccessAttemps")) >= iInvalidAttemps Then
                Dim oDate As DateTime = Nothing

                If Not IsDBNull(dTbl.Rows(0)("LastDateInvalidAccessAttempted")) AndAlso roTypes.Any2DateTime(dTbl.Rows(0)("LastDateInvalidAccessAttempted")) <> Date.MinValue Then
                    oDate = roTypes.Any2DateTime(dTbl.Rows(0)("LastDateInvalidAccessAttempted"))
                End If

                ' Si han pasado menos de 10 minutos esta temporalmente bloqueada
                If oDate <> Nothing AndAlso DateDiff(DateInterval.Minute, oDate, Now) < 10 Then
                    Return True
                End If
            End If

            Return False
        End Function

        Private Shared Function CheckIfPasswordIsExpired(isSSO As Boolean, dTbl As DataTable) As Boolean
            If Not isSSO Then
                Dim intDaysPasswordExpired = Any2Integer(dTbl.Rows(0)("DaysPasswordExpired"))

                If intDaysPasswordExpired > 0 Then
                    Dim oLastUpdatePassword As DateTime = Now

                    If Not IsDBNull(dTbl.Rows(0)("LastUpdatePassword")) AndAlso roTypes.Any2DateTime(dTbl.Rows(0)("LastUpdatePassword")) <> Date.MinValue Then
                        oLastUpdatePassword = roTypes.Any2DateTime(dTbl.Rows(0)("LastUpdatePassword"))
                    End If

                    ' Si han pasado mas dias , la contraseña ha caducado
                    If DateDiff(DateInterval.Day, oLastUpdatePassword, Now) >= intDaysPasswordExpired Then
                        Return True
                    End If
                Else
                    If Not IsDBNull(dTbl.Rows(0)("LastUpdatePassword")) AndAlso roTypes.Any2DateTime(dTbl.Rows(0)("LastUpdatePassword")) = roTypes.CreateDateTime(1900, 1, 1) Then
                        Return True
                    End If
                End If
            End If

            Return False
        End Function

        Public Function GetPassportContext(ByVal idPassport As Integer) As String
            Dim sContext As String = String.Empty
            Try
                Dim strSQL As String = "@SELECT# ConfData FROM sysropassports where Id = @idpassport"

                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, idPassport))

                sContext = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL, parameters))

            Catch ex As Exception

            End Try

            Return sContext
        End Function

        Public Function UpdatePassportContext(ByVal idPassport As Integer, ByVal strContext As String) As Boolean
            Dim bSaved As Boolean = False
            Try
                Dim strSQL As String = "@UPDATE# sysropassports set ConfData = @strContext where Id = @idpassport"

                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@strContext", CommandParameter.ParameterType.tString, strContext))

                bSaved = DataLayer.AccessHelper.ExecuteSql(strSQL, parameters)

            Catch ex As Exception

            End Try

            Return bSaved
        End Function

        Public Function Save(ByRef oPassport As roPassport, Optional ByVal bAudit As Boolean = False, Optional ByVal bolLaunchSecurityTask As Boolean = True) As Boolean
            Dim bolRet As Boolean = False
            Dim bLaunchBroadcaster As Boolean = False
            Dim bolNew As Boolean = False
            Dim oDelPassParent As roPassport = Nothing
            Dim bolAddPassParent As Boolean = False
            Dim bolIsSupervisor As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(oPassport) Then
                    Me.State.Result = SecurityResultEnum.XSSvalidationError
                    Return False
                End If


                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                ' Miramos si el passport es nuevo
                Dim oPass As roPassport = LoadPassport(oPassport.ID, LoadType.Passport)
                If oPass Is Nothing Then bolNew = True

                If oPassport.GroupType = "E" AndAlso oPassport.IDUser.HasValue AndAlso oPassport.IDUser > 0 AndAlso oPassport.IDEmployee.HasValue AndAlso oPassport.IDEmployee > 0 AndAlso oPassport.Email <> String.Empty Then
                    'Quitamos el email ya que prevalece el de empleado
                    oPassport.Email = String.Empty
                End If

                ' Verificamos si el passport es de un Supervisor
                If oPassport.GroupType = "" OrElse oPassport.IsSupervisor Then bolIsSupervisor = True

                If oPassport.GroupType = "" AndAlso oPassport.IDEmployee.HasValue AndAlso oPassport.IDEmployee > 0 Then
                    ' En el caso que nos venga de tipo supervisor per contenga id empleado
                    ' quiere decir que es un hibrido, y debemos forzar el tipo (esto solo puede pasar si se viene de la pantalla de crear empleado)
                    oPassport.GroupType = "E"
                    'Quitamos el email ya que prevalece el de empleado
                    oPassport.Email = String.Empty
                End If

                bolRet = True

                ' Si es un supervisor nuevo hay que crear el grupo padre
                If bolNew AndAlso bolIsSupervisor Then bolAddPassParent = True

                If oPassport.GroupType = "E" Then
                    ' Si es supervisor y es empleado, debemos crear su grupo de usuario en caso que no exista
                    If bolIsSupervisor Then
                        Dim oPassParent As roPassport = Nothing
                        If oPassport.IDParentPassport.HasValue AndAlso oPassport.IDParentPassport > 0 Then
                            oPassParent = LoadPassport(oPassport.IDParentPassport, LoadType.Passport)
                        End If
                        If oPassParent Is Nothing Then bolAddPassParent = True
                    Else
                        ' Si es un empleado y no es supervisor, ha que eliminar al grupo padre si existe
                        If oPassport.IDParentPassport.HasValue AndAlso oPassport.IDParentPassport > 0 Then
                            oDelPassParent = LoadPassport(oPassport.IDParentPassport, LoadType.Passport)
                            If oDelPassParent IsNot Nothing Then
                                oPassport.IDParentPassport = Nothing
                                bolRet = Delete(oDelPassParent)
                            End If
                        End If
                    End If
                End If

                ' Creamos el grupo padre del supervisor, en caso necesario
                If bolRet AndAlso bolAddPassParent Then bolRet = AddParentGroup(oPassport, bAudit)

                ' Guardamos los datos del passport
                If bolRet Then bolRet = SavePassportData(oPassport, bLaunchBroadcaster)

                ' Guardo excepciones sobre el empleado si aplica y si es supervisor

                If oPassport.IsSupervisor AndAlso (oPassport.IDEmployee IsNot Nothing AndAlso oPassport.IDEmployee > 0 AndAlso (oPassport.Exceptions Is Nothing OrElse (oPassport.Exceptions.Exceptions.Length = 0 AndAlso Not oPassport.CanApproveOwnRequests))) Then
                    bolRet = SetEmployeePassportException(oPassport.ID, oPassport.IDEmployee, oPassport.CanApproveOwnRequests)
                End If

                ' Auditamos
                If bolRet AndAlso bAudit Then
                    Me.oState.Audit(If(bolNew, Audit.Action.aInsert, Audit.Action.aUpdate), Audit.ObjectType.tPassport, oPassport.Name, Nothing, -1)
                End If

                If bolRet AndAlso bolNew Then
                    bolRet = AuthHelper.SetPassportKeyValidated(oPassport.ID, False, "", "")
                End If

                If bolRet AndAlso bolLaunchSecurityTask Then
                    Try
                        ' Lanzamos tarea de mapeo de permisos
                        Dim oStateTask As New roLiveTaskState(-1)
                        Dim oParameters As New roCollection
                        oParameters.Add("Context", 3)
                        oParameters.Add("IDContext", oPassport.ID)
                        oParameters.Add("Action", 1)

                        roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oParameters, oStateTask)
                    Catch ex As Exception
                        oState.UpdateStateInfo(ex, "roPassportManager::Save:SecurityPermissions")
                    End Try
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPassportManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Try
                'Lanzo Broadcaster en caso necesario
                If bolRet AndAlso bLaunchBroadcaster Then roConnector.InitTask(TasksType.BROADCASTER)
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::Save::LaunchBroadcaster")
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByRef oPassport As roPassport, Optional ByVal _LaunchBroadcaster As Boolean = False, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False
            Dim oSqlArray As New Generic.List(Of String)
            Dim bHaveToClose As Boolean = False

            Dim strSQL As String = ""

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                oSqlArray.Add("@UPDATE# sysroPassports SET IDParentPassport=null WHERE IDParentPassport=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM sysroPermissionsOverEmployeesExceptions WHERE PassportID=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM sysroPermissionsOverFeatures WHERE PassportID=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM sysroPermissionsOverGroups WHERE PassportID=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM sysroPermissionsOverRequests WHERE IDParentPassport=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM geniusExecutions WHERE IDGeniusView IN (@SELECT# ID from GeniusViews where IdPassport=" & oPassport.ID.ToString & ")")
                oSqlArray.Add("@DELETE# FROM geniusViews WHERE IdPassport=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM sysroPassports_DeviceTokens WHERE IdPassport=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM ReportExecutionsLastParameters WHERE PassportId=" & oPassport.ID.ToString)
                oSqlArray.Add("@DELETE# FROM ReportExecutions WHERE PassportId=" & oPassport.ID.ToString) 'Informes solicitados
                oSqlArray.Add("@DELETE# FROM ReportLayoutPermission WHERE PassportId=" & oPassport.ID.ToString) '??
                oSqlArray.Add("ReportsScheduler") 'Informes CR programados
                oSqlArray.Add("sysroSecurityParameters")
                oSqlArray.Add("sysroPassports_PasswordHistory")
                oSqlArray.Add("sysroPassports_Data")
                oSqlArray.Add("sysroPassports_AuthorizedAdress")
                oSqlArray.Add("sysroPassports_Groups")
                oSqlArray.Add("sysroPassports_AccessGroup")
                oSqlArray.Add("sysroPassports_AuthenticationMethods")
                oSqlArray.Add("sysroPassports_Centers")
                oSqlArray.Add("sysroPassports_PermissionsOverEmployees")
                oSqlArray.Add("sysroPassports_PermissionsOverFeatures")
                oSqlArray.Add("sysroPassports_PermissionsOverGroups")
                oSqlArray.Add("sysroPassports_SaaS_Status")
                oSqlArray.Add("sysroPassports_Sessions")
                oSqlArray.Add("@DELETE# FROM BotRules WHERE Type =" & BotRuleTypeEnum.CopySupervisorPermissions & " AND IDTemplate=" & oPassport.ID.ToString)

                '---- Lo siguiente DELETES no los ejecutamos, porque desaparecerían objetos que podrían estar en uso por otros usuarios.
                '---- De existir, el empleado no se podrá borrar.
                '---- oSqlArray.Add("@DELETE# FROM ReportPlannedExecutions WHERE PassportId=" & oPassport.ID.ToString) 'Informes programados
                '---- oSqlArray.Add("@DELETE# FROM Communiques WHERE IdCreatedBy=" & oPassport.ID.ToString) 'Comunicados
                '---- oSqlArray.Add("@DELETE# FROM Channels WHERE IdCreatedBy=" & oPassport.ID.ToString) 'Canales
                '---- oSqlArray.Add("@DELETE# FROM Surveys WHERE IdCreatedBy=" & oPassport.ID.ToString) 'Encuestas
                '---- oSqlArray.Add("ReportLayouts") 'Informes
                'TODO: Solventar el problema, por ejemplo asignando el objeto a otro supervisor con mismos permisos? ...

                'Finalmente borramos el passport
                oSqlArray.Add("@DELETE# FROM sysroPassports WHERE ID=" & oPassport.ID.ToString)

                ' Borramos todas las tablas relacionadas
                For Each strSQL In oSqlArray
                    bolRet = False
                    If Not strSQL.ToUpper.Contains("DELETE") AndAlso Not strSQL.ToUpper.Contains("UPDATE") Then
                        strSQL = $"@DELETE# {strSQL} WHERE IDPassport = {oPassport.ID}"
                    End If

                    bolRet = ExecuteSql(strSQL)
                    If Not bolRet Then
                        Exit For
                    End If
                Next

                If bolRet Then
                    Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tPassport, oPassport.Name, Nothing, -1)

                    If _LaunchBroadcaster Then roConnector.InitTask(TasksType.BROADCASTER)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPassportManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try
            Return bolRet

        End Function

        Public Function DeteleteEmployeeFromPassport(oPassport As roPassport, updateDriversTasks As Boolean, bState As roSecurityState) As Boolean

            Dim bolRet As Boolean = True
            Try
                If oPassport.GroupType = "E" Then
                    oPassport.GroupType = ""
                    bolRet = Me.Save(oPassport)
                End If
            Catch ex As System.Data.Common.DbException
                bState.UpdateStateInfo(ex, "roPassportManager::DeleteEmployeeFromPassport")
            Catch ex As Exception
                bState.UpdateStateInfo(ex, "roPassportManager::DeleteEmployeeFromPassport")
            End Try

            Return bolRet
        End Function

        Private Function SavePassportData(ByRef oPassport As roPassport, ByRef bLaunchBroadcaster As Boolean) As Boolean
            Dim bolRet As Boolean = False
            Dim bolIsNew As Boolean = True

            Try

                Dim tb As New DataTable("sysropassports")
                Dim strSQL As String = "@SELECT# * FROM sysroPassports WHERE Id = " & oPassport.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If (tb.Rows.Count.Equals(0)) Then
                    oRow = tb.NewRow
                Else
                    oRow = tb.Rows(0)
                    bolIsNew = False
                End If

                bolRet = True

                ' Creamos usuario en caso que sea un nuevo passport
                If bolIsNew Then bolRet = CreateUser(oPassport)

                If bolRet Then
                    'oRow("ID") = oPassport.ID
                    oRow("IDParentPassport") = If(IsNothing(oPassport.IDParentPassport) OrElse oPassport.IDParentPassport = 0, DBNull.Value, oPassport.IDParentPassport)
                    oRow("GroupType") = roTypes.Any2String(oPassport.GroupType)
                    oRow("Name") = roTypes.Any2String(oPassport.Name)
                    oRow("Description") = roTypes.Any2String(oPassport.Description)
                    oRow("Email") = roTypes.Any2String(oPassport.Email)
                    oRow("IDUser") = If((IsNothing(oPassport.IDUser) OrElse oPassport.IDUser = 0), DBNull.Value, oPassport.IDUser)
                    oRow("IDEmployee") = If(IsNothing(oPassport.IDEmployee) OrElse oPassport.IDEmployee = 0, DBNull.Value, oPassport.IDEmployee)
                    oRow("IDLanguage") = If(IsNothing(oPassport.Language), 0, roTypes.Any2Integer(oPassport.Language.ID))
                    oRow("ConfData") = roTypes.Any2String(oPassport.ConfData)
                    oRow("StartDate") = If(IsNothing(oPassport.StartDate), DBNull.Value, oPassport.StartDate)
                    oRow("ExpirationDate") = If(IsNothing(oPassport.ExpirationDate), DBNull.Value, oPassport.ExpirationDate)
                    oRow("State") = If(IsNothing(oPassport.State), 1, oPassport.State)
                    oRow("EnabledVTDesktop") = roTypes.Any2Boolean(oPassport.EnabledVTDesktop)
                    oRow("EnabledVTPortal") = roTypes.Any2Boolean(oPassport.EnabledVTPortal)
                    oRow("EnabledVTPortalApp") = roTypes.Any2Boolean(oPassport.EnabledVTPortalApp)
                    oRow("EnabledVTVisits") = roTypes.Any2Boolean(oPassport.EnabledVTVisits)
                    oRow("EnabledVTVisitsApp") = roTypes.Any2Boolean(oPassport.EnabledVTVisitsApp)
                    oRow("IsSupervisor") = roTypes.Any2Boolean(oPassport.IsSupervisor)
                    oRow("LicenseAccepted") = roTypes.Any2Boolean(oPassport.LicenseAccepted)
                    oRow("PhotoRequiered") = roTypes.Any2Boolean(oPassport.PhotoRequiered)
                    oRow("LocationRequiered") = roTypes.Any2Boolean(oPassport.LocationRequiered)
                    oRow("CanApproveOwnRequests") = roTypes.Any2Boolean(oPassport.CanApproveOwnRequests)
                    oRow("IDGroupFeature") = roTypes.Any2Integer(oPassport.IDGroupFeature)
                    oRow("LoginWithoutContract") = roTypes.Any2Boolean(oPassport.LoginWithoutContract)

                    If tb.Rows.Count = 0 Then tb.Rows.Add(oRow)
                    da.Update(tb)
                End If

                'Obtenemos el ID del passport creado
                If bolIsNew AndAlso bolRet Then
                    Dim tbNew As DataTable = CreateDataTable("@SELECT# TOP 1 [ID] FROM sysroPassports WHERE Name like '" & oPassport.Name.Replace("'", "''") & "' ORDER BY [ID] DESC")
                    If tbNew IsNot Nothing AndAlso tbNew.Rows.Count = 1 Then
                        oPassport.ID = tbNew.Rows(0)("ID")
                    End If
                End If

                If bolIsNew AndAlso oPassport.IDEmployee.HasValue AndAlso oPassport.IDEmployee > 0 Then
                    bolRet = SetDefaultEmployeePerissions(oPassport)
                End If

                ' Guardamos los metodos de autentificacion
                bolRet = SaveAuthenticationMethods(oPassport, bolIsNew, bLaunchBroadcaster)

                ' Categorias que puede gestionar
                If bolRet Then bolRet = SaveCategories(oPassport)

                ' Grupos de empleados que puede gestionar
                If bolRet Then bolRet = SaveGroups(oPassport)

                ' Excepciones sobre empleados
                If bolRet Then bolRet = SaveExceptions(oPassport)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager:: SavePassportData")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::SavePassportData")
            End Try

            Return bolRet
        End Function

        Public Function IsValidAuthenticationMethods(ByVal oPassport As roPassport, Optional ByVal IgnoreEmployeeContract As Boolean = False) As Boolean
            Dim bolRet As Boolean = True

            Dim InvalidMethod As AuthenticationMethod

            Try

                If bolRet Then

                    ' Solo se verifican las credenciales si el pasaporte no es de tipo empleado o el empleado tiene contrato activo
                    Dim bolCheck As Boolean = False
                    If IgnoreEmployeeContract Then
                        bolCheck = True
                    ElseIf oPassport IsNot Nothing Then
                        bolCheck = (Not oPassport.IDEmployee.HasValue OrElse roPassportManager.GetEmployeeActiveContract(oPassport).Rows.Count > 0 OrElse oPassport.IDEmployee = 0)
                    End If
                    If bolCheck Then
                        If oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.PlateRows IsNot Nothing Then
                            For Each oPlate As roPassportAuthenticationMethodsRow In oPassport.AuthenticationMethods.PlateRows
                                If oPlate.RowState <> RowState.DeleteRow Then
                                    bolRet = Not PlateExists(oPlate.Credential, oPlate.Method, oPlate.Version, oPlate.IDPassport)
                                    If Not bolRet Then
                                        InvalidMethod = oPlate.Method
                                        Exit For
                                    End If
                                End If

                            Next
                        End If
                        If bolRet AndAlso oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.PasswordRow IsNot Nothing AndAlso oPassport.AuthenticationMethods.PasswordRow.RowState <> RowState.DeleteRow Then
                            bolRet = Not CredentialExists(oPassport.AuthenticationMethods.PasswordRow.Credential, oPassport.AuthenticationMethods.PasswordRow.Method, oPassport.AuthenticationMethods.PasswordRow.Version, oPassport.AuthenticationMethods.PasswordRow.IDPassport)
                            If Not bolRet Then
                                InvalidMethod = AuthenticationMethod.Password
                            End If

                        End If

                        If bolRet AndAlso oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.IntegratedSecurityRow IsNot Nothing AndAlso oPassport.AuthenticationMethods.IntegratedSecurityRow.RowState <> RowState.DeleteRow Then
                            bolRet = Not CredentialExists(oPassport.AuthenticationMethods.IntegratedSecurityRow.Credential, oPassport.AuthenticationMethods.IntegratedSecurityRow.Method, oPassport.AuthenticationMethods.IntegratedSecurityRow.Version, oPassport.AuthenticationMethods.IntegratedSecurityRow.IDPassport)
                            If Not bolRet Then
                                InvalidMethod = AuthenticationMethod.IntegratedSecurity
                            End If
                        End If

                        If bolRet AndAlso oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.CardRows IsNot Nothing Then
                            For Each oCard As roPassportAuthenticationMethodsRow In oPassport.AuthenticationMethods.CardRows
                                If oCard.RowState <> RowState.DeleteRow AndAlso oCard.Enabled Then
                                    bolRet = Not CredentialExists(oCard.Credential, oCard.Method, oCard.Version, oCard.IDPassport)
                                    If Not bolRet Then
                                        InvalidMethod = oCard.Method
                                        Exit For
                                    End If
                                End If
                            Next

                        End If

                        If bolRet AndAlso oPassport.AuthenticationMethods IsNot Nothing AndAlso oPassport.AuthenticationMethods.NFCRow IsNot Nothing AndAlso oPassport.AuthenticationMethods.NFCRow.RowState <> RowState.DeleteRow Then
                            bolRet = Not CredentialExists(oPassport.AuthenticationMethods.NFCRow.Credential, oPassport.AuthenticationMethods.NFCRow.Method, oPassport.AuthenticationMethods.NFCRow.Version, oPassport.AuthenticationMethods.NFCRow.IDPassport)
                            If Not bolRet Then
                                InvalidMethod = AuthenticationMethod.NFC
                            End If

                        End If

                    End If
                End If

                If Not bolRet Then
                    Select Case InvalidMethod
                        Case AuthenticationMethod.Biometry
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsBiometryIncorrect
                        Case AuthenticationMethod.Card
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsCardIncorrect
                        Case AuthenticationMethod.IntegratedSecurity
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsIntegratedSecurityIncorrect
                        Case AuthenticationMethod.Password
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodPasswordIncorrect
                        Case AuthenticationMethod.Pin
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsPinIncorrect
                        Case AuthenticationMethod.Plate
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsPlateIncorrect
                        Case AuthenticationMethod.NFC
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsNFCIncorrect

                        Case Else
                            Me.oState.Result = SecurityResultEnum.AuthenticationMethodsIncorrect
                    End Select
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roPassportManager::IsValidAuthenticationMethods")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roPassportManager::IsValidAuthenticationMethods")
            End Try
            Return bolRet

        End Function

        Public Function UpdatePassportNameAndLanguage(ByVal idPassport As Integer, ByVal sNewName As String, ByVal idLang As Integer) As Boolean
            ' Nivel de mando del empleado en funcion del tipo de solicitud
            Dim bolRet As Boolean = True
            Dim strQuery As String

            Try
                strQuery = "@UPDATE# [dbo].[sysropassports] SET Name = '" & sNewName.Replace("'", "''") & "'"

                If idLang >= 0 Then
                    strQuery = strQuery & " , IDLanguage = " & idLang
                End If

                strQuery = strQuery & " WHERE ID = " & idPassport

                bolRet = ExecuteSql(strQuery)
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::UpdatePassportName")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::UpdatePassportName")
            End Try

            Return bolRet

        End Function

        Public Function SetEmployeePassportException(idPassport As Integer, idEmployee As Integer, Permission As Boolean) As Boolean
            Dim bRet As Boolean = False
            Try
                Dim sqlCommand As String
                sqlCommand = "IF EXISTS(@SELECT# IdPassport FROM sysroPassports_Employees WHERE IDPassport = @idPassport AND IDEmployee = @idEmployee) " &
                             "@UPDATE# sysroPassports_Employees SET Permission = @permission WHERE IDPassport = @idPassport AND IDEmployee = @idEmployee " &
                             "ELSE " &
                             "@INSERT# INTO sysroPassports_Employees (IDEmployee, IDPassport, Permission) VALUES (@idEmployee, @idPassport, @permission)"

                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@idEmployee", CommandParameter.ParameterType.tInt, idEmployee))
                parameters.Add(New CommandParameter("@permission", CommandParameter.ParameterType.tBoolean, Permission))
                AccessHelper.ExecuteSql(sqlCommand, parameters)

                bRet = True
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::SetEmployeePassportException")
            End Try

            Return bRet
        End Function

        ''' <summary>
        ''' Return de list of passports prepared for audit
        ''' </summary>
        Public Function GetLitePassports(ByVal loadType As LoadType, Optional ByVal _State As roSecurityState = Nothing) As DataTable
            Dim oRet As New DataTable
            Try
                Dim strSQL As String = "@SELECT# ID AS UserID, Name AS UserName, Description From sysroPassports "

                ' Cargamos el passport en función del identificador
                Select Case loadType
                    Case LoadType.Employee
                        strSQL &= " WHERE IDEmployee IS NOT NULL"
                    Case LoadType.User
                        strSQL &= " WHERE IDUSer IS NOT NULL"
                End Select

                oRet = CreateDataTable(strSQL)

                Return oRet
            Catch
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Return de list of passports that has the supervisor role
        ''' </summary>
        Public Function GetSupervisorPassports(Optional ByVal _State As roSecurityState = Nothing) As DataTable
            Try
                Dim tb As DataTable = Nothing

                Dim strSQL As String = "@SELECT# ID,Name from sysroPassports " &
                    " WHERE (sysroPassports.EnabledVTDesktop=1 ) " & ' Desktop Activo
                    " AND IsSupervisor=1 " &
                    " AND GroupType <> 'U' " &
                    " AND Description not like '%@@ROBOTICS@@%' " &
                    " AND sysroPassports.State =1 " ' Passport Activo

                tb = AccessHelper.CreateDataTable(strSQL)

                Return tb
            Catch
                Return Nothing
            End Try
        End Function

        Public Function GetParents(ByVal idPassport As Integer) As List(Of Integer)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("@SELECT# * FROM dbo.GetPassportParents(@idPassport)")
                Command.CommandType = CommandType.Text
                AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32).Value = idPassport
                Dim Result As New List(Of Integer)
                Dim Reader As DbDataReader = Command.ExecuteReader()
                While Reader.Read()
                    Result.Add(Reader.GetInt32(0))
                End While
                Reader.Close()
                Return Result
            Finally

            End Try
        End Function

        Public Function GetChilds(ByVal idPassport As Integer) As List(Of Integer)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("@SELECT# * FROM dbo.GetPassportChilds(@idPassport)")
                Command.CommandType = CommandType.Text
                AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32).Value = idPassport
                Dim Result As New List(Of Integer)
                Dim Reader As DbDataReader = Command.ExecuteReader()
                While Reader.Read()
                    Result.Add(Reader.GetInt32(0))
                End While
                Reader.Close()
                Return Result
            Finally

            End Try
        End Function

        ''' <summary>
        ''' Returns the ids of passports having the specified parent.
        ''' </summary>
        ''' <param name="idParentPassport">The parent of the passports to list, or Nothing to return root passports.</param>
        Public Function GetPassportsByParent(ByVal idParentPassport As Nullable(Of Integer)) As List(Of Integer)
            Return GetPassportsByParent(idParentPassport, Nothing)
        End Function

        ''' <summary>
        ''' Returns the ids of passports having the specified parent and of specified type.
        ''' </summary>
        ''' <param name="idParentPassport">The parent of the passports to list, or Nothing to return root passports.</param>
        ''' <param name="groupType">The type of passports to return, or Nothing to return all types.</param>
        Public Function GetPassportsByParent(ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String, Optional ByVal _State As roSecurityState = Nothing) As List(Of Integer)

            Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Passports_SelectByParent")
            Command.CommandType = CommandType.StoredProcedure
            AccessHelper.AddParameter(Command, "@idParentPassport", DbType.Int32)
            If idParentPassport.HasValue Then
                Command.Parameters("@idParentPassport").Value = idParentPassport.Value
            End If
            AccessHelper.AddParameter(Command, "@groupType", DbType.AnsiString, 1)
            If groupType IsNot Nothing Then
                Command.Parameters("@groupType").Value = groupType
            End If
            Dim Result As New List(Of Integer)
            Dim Reader As DbDataReader = Command.ExecuteReader()
            While Reader.Read()
                Result.Add(Reader.GetInt32(0))
            End While
            Reader.Close()
            Return Result

        End Function

        ''' <summary>
        ''' Returns the ids of passports having the specified parent.
        ''' </summary>
        ''' <param name="idParentPassport">The parent of the passports to list, or Nothing to return root passports.</param>
        Public Function GetPassportsByParentLite(ByVal idParentPassport As Nullable(Of Integer)) As DataTable
            Return GetPassportsByParentLite(idParentPassport, Nothing)
        End Function

        ''' <summary>
        ''' Returns the ids of passports having the specified parent and of specified type.
        ''' </summary>
        ''' <param name="idParentPassport">The parent of the passports to list, or Nothing to return root passports.</param>
        ''' <param name="groupType">The type of passports to return, or Nothing to return all types.</param>
        Public Function GetPassportsByParentLite(ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String, Optional ByVal _State As roSecurityState = Nothing) As DataTable
            Dim Result As New DataTable
            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_Passports_SelectDetailByParent")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@idParentPassport", DbType.Int32)
                If idParentPassport.HasValue Then
                    Command.Parameters("@idParentPassport").Value = idParentPassport.Value
                End If
                AccessHelper.AddParameter(Command, "@groupType", DbType.AnsiString, 1)
                If groupType IsNot Nothing Then
                    Command.Parameters("@groupType").Value = groupType
                End If
                Result.Columns.Add("ID", GetType(Integer))
                Result.Columns.Add("Name", GetType(String))
                Result.Columns.Add("Description", GetType(String))
                Result.Columns.Add("GroupType", GetType(String))

                Dim Adapter As DbDataAdapter = AccessHelper.CreateDataAdapter(Command)
                Adapter.Fill(Result)
            Catch ex As DbException
                If _State IsNot Nothing Then
                    _State.UpdateStateInfo(ex)
                Else
                    Throw ex
                End If
            End Try

            Return Result
        End Function

#End Region

#Region "Get Passport / Ticket"

        ''' <summary>
        ''' Returns the passport with specified ID.
        ''' </summary>
        ''' <param name="id">The ID of the passport to load, or the employee or user id.</param>
        Public Shared Function GetPassport(ByVal id As Integer, Optional ByRef _State As roSecurityState = Nothing) As roPassport
            Try
                Return roPassportManager.GetPassport(id, LoadType.Passport, _State)
            Catch ex As Exception
                _State.Result = SecurityResultEnum.PassportDoesNotExists
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Returns the passport with specified ID.
        ''' </summary>
        ''' <param name="id">The ID of the passport to load, or the employee or user id.</param>
        ''' <param name="loadType">The type of ID: passport, employee or user.</param>
        Public Shared Function GetPassport(ByVal id As Integer, ByVal loadType As LoadType, Optional ByRef _State As roSecurityState = Nothing) As roPassport
            If _State Is Nothing Then _State = New roSecurityState
            Try
                Dim oManager As New roPassportManager(_State)

                Dim oPassport As roPassport = oManager.LoadPassport(id, loadType)
                roBusinessState.CopyTo(oManager.State, _State)
                Return oPassport
            Catch
                _State.Result = SecurityResultEnum.PassportDoesNotExists
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Returns the passport with specified ID.
        ''' </summary>
        ''' <param name="id">The ID of the passport to load, or the employee or user id.</param>
        Public Shared Function GetPassportTicket(ByVal id As Integer, Optional ByRef _State As roSecurityState = Nothing) As roPassportTicket
            Try
                Return roPassportManager.GetPassportTicket(id, LoadType.Passport, _State)
            Catch ex As Exception
                _State.Result = SecurityResultEnum.PassportDoesNotExists
                Return Nothing
            End Try
        End Function

        ''' <summary>
        ''' Returns the passport with specified ID.
        ''' </summary>
        ''' <param name="id">The ID of the passport to load, or the employee or user id.</param>
        ''' <param name="loadType">The type of ID: passport, employee or user.</param>
        Public Shared Function GetPassportTicket(ByVal id As Integer, ByVal loadType As LoadType, Optional ByRef _State As roSecurityState = Nothing) As roPassportTicket
            If _State Is Nothing Then _State = New roSecurityState

            Try
                Dim oManager As New roPassportManager(_State)

                Dim oPassport As roPassportTicket = oManager.LoadPassportTicket(id, loadType)

                roBusinessState.CopyTo(oManager.State, _State)
                Return oPassport
            Catch
                _State.Result = SecurityResultEnum.PassportDoesNotExists
                Return Nothing
            End Try
        End Function

        Public Shared Function GetPassportWithPhoto(ByRef iPassportID As Integer) As roPassportWithPhoto
            Dim bRet As New roPassportWithPhoto

            Dim strSQL As String = "@SELECT# e.Id as IdEmployee, sp.Id as IdPassport, e.Name as EmployeeName, sp.Name as PassportName, e.Image as EmployeePhoto  from sysropassports sp left join employees e on sp.IDEmployee = e.ID WHERE sp.ID= " & iPassportID.ToString
            Dim dt As DataTable = AccessHelper.CreateDataTable(strSQL)

            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                bRet.IdEmployee = VTBase.roTypes.Any2Integer(dt.Rows(0)("IdEmployee"))
                bRet.IdPassport = VTBase.roTypes.Any2Integer(dt.Rows(0)("IdPassport"))
                bRet.EmployeeName = VTBase.roTypes.Any2String(dt.Rows(0)("EmployeeName"))
                bRet.PassportName = VTBase.roTypes.Any2String(dt.Rows(0)("PassportName"))
                If Not IsDBNull(dt.Rows(0)("EmployeePhoto")) Then
                    bRet.EmployeePhoto = Convert.ToBase64String(dt.Rows(0)("EmployeePhoto"))
                Else
                    bRet.EmployeePhoto = String.Empty
                End If
            End If

            Return bRet
        End Function

#End Region

#Region "Private Methods"

        Private Function LoadAuthenticationMethods(ByVal oPassport As roPassport) As roPassportAuthenticationMethods
            '
            ' Cargamos los metodos de autentificación del passport
            '

            Dim oRet As roPassportAuthenticationMethods = New roPassportAuthenticationMethods()
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.Password
                Dim dTblAuthenticationMethods As System.Data.DataTable = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    oRet.PasswordRow = New roPassportAuthenticationMethodsRow With {
                                  .IDPassport = oPassport.ID,
                                  .BiometricData = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BiometricData")), Nothing, dTblAuthenticationMethods.Rows(0)("BiometricData")),
                                  .BiometricID = dTblAuthenticationMethods.Rows(0)("BiometricID"),
                                  .BiometricAlgorithm = roTypes.Any2String(dTblAuthenticationMethods.Rows(0)("BiometricAlgorithm")),
                                  .BiometricTerminalId = roTypes.Any2Integer(dTblAuthenticationMethods.Rows(0)("BiometricTerminalId")),
                                  .BloquedAccessApp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")), False, dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")),
                                  .Credential = dTblAuthenticationMethods.Rows(0)("Credential"),
                                  .Enabled = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("Enabled")), False, dTblAuthenticationMethods.Rows(0)("Enabled")),
                                  .ExpirationDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("ExpirationDate")), Nothing, dTblAuthenticationMethods.Rows(0)("ExpirationDate")),
                                  .InvalidAccessAttemps = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")), 0, dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")),
                                  .LastDateInvalidAccessAttempted = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")), Nothing, dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")),
                                  .LastUpdatePassword = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")), Nothing, dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")),
                                  .Method = dTblAuthenticationMethods.Rows(0)("Method"),
                                  .Password = dTblAuthenticationMethods.Rows(0)("Password"),
                                  .StartDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("StartDate")), Nothing, dTblAuthenticationMethods.Rows(0)("StartDate")),
                                  .TimeStamp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("TimeStamp")), Nothing, dTblAuthenticationMethods.Rows(0)("TimeStamp")),
                                  .Version = dTblAuthenticationMethods.Rows(0)("Version"),
                                  .RowState = RowState.NoChangeRow,
                                  .BlockedAccessByInactivity = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BlockedAccessByInactivity")), False, dTblAuthenticationMethods.Rows(0)("BlockedAccessByInactivity")),
                                  .LastAppActionDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastAppActionDate")), Nothing, dTblAuthenticationMethods.Rows(0)("LastAppActionDate"))
                                  }
                End If

                strSQL = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.IntegratedSecurity
                dTblAuthenticationMethods = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    oRet.IntegratedSecurityRow = New roPassportAuthenticationMethodsRow With {
                                  .IDPassport = oPassport.ID,
                                  .BiometricData = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BiometricData")), Nothing, dTblAuthenticationMethods.Rows(0)("BiometricData")),
                                  .BiometricID = dTblAuthenticationMethods.Rows(0)("BiometricID"),
                                  .BiometricAlgorithm = roTypes.Any2String(dTblAuthenticationMethods.Rows(0)("BiometricAlgorithm")),
                                  .BiometricTerminalId = roTypes.Any2Integer(dTblAuthenticationMethods.Rows(0)("BiometricTerminalId")),
                                  .BloquedAccessApp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")), False, dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")),
                                  .Credential = dTblAuthenticationMethods.Rows(0)("Credential"),
                                  .Enabled = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("Enabled")), False, dTblAuthenticationMethods.Rows(0)("Enabled")),
                                  .ExpirationDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("ExpirationDate")), Nothing, dTblAuthenticationMethods.Rows(0)("ExpirationDate")),
                                  .InvalidAccessAttemps = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")), 0, dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")),
                                  .LastDateInvalidAccessAttempted = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")), Nothing, dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")),
                                  .LastUpdatePassword = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")), Nothing, dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")),
                                  .Method = dTblAuthenticationMethods.Rows(0)("Method"),
                                  .Password = dTblAuthenticationMethods.Rows(0)("Password"),
                                  .StartDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("StartDate")), Nothing, dTblAuthenticationMethods.Rows(0)("StartDate")),
                                  .TimeStamp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("TimeStamp")), Nothing, dTblAuthenticationMethods.Rows(0)("TimeStamp")),
                                  .Version = dTblAuthenticationMethods.Rows(0)("Version"),
                                  .RowState = RowState.NoChangeRow
                                      }
                End If

                strSQL = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.Pin
                dTblAuthenticationMethods = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    oRet.PinRow = New roPassportAuthenticationMethodsRow With {
                                     .IDPassport = oPassport.ID,
                                  .BiometricData = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BiometricData")), Nothing, dTblAuthenticationMethods.Rows(0)("BiometricData")),
                                  .BiometricID = dTblAuthenticationMethods.Rows(0)("BiometricID"),
                                  .BiometricAlgorithm = roTypes.Any2String(dTblAuthenticationMethods.Rows(0)("BiometricAlgorithm")),
                                  .BiometricTerminalId = roTypes.Any2Integer(dTblAuthenticationMethods.Rows(0)("BiometricTerminalId")),
                                  .BloquedAccessApp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")), False, dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")),
                                  .Credential = dTblAuthenticationMethods.Rows(0)("Credential"),
                                  .Enabled = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("Enabled")), False, dTblAuthenticationMethods.Rows(0)("Enabled")),
                                  .ExpirationDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("ExpirationDate")), Nothing, dTblAuthenticationMethods.Rows(0)("ExpirationDate")),
                                  .InvalidAccessAttemps = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")), 0, dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")),
                                  .LastDateInvalidAccessAttempted = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")), Nothing, dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")),
                                  .LastUpdatePassword = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")), Nothing, dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")),
                                  .Method = dTblAuthenticationMethods.Rows(0)("Method"),
                                  .Password = dTblAuthenticationMethods.Rows(0)("Password"),
                                  .StartDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("StartDate")), Nothing, dTblAuthenticationMethods.Rows(0)("StartDate")),
                                  .TimeStamp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("TimeStamp")), Nothing, dTblAuthenticationMethods.Rows(0)("TimeStamp")),
                                  .Version = dTblAuthenticationMethods.Rows(0)("Version"),
                                  .RowState = RowState.NoChangeRow
                                      }
                End If

                strSQL = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.Card
                dTblAuthenticationMethods = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    Dim tmpAuthentication As New Generic.List(Of roPassportAuthenticationMethodsRow)

                    For Each oRow As DataRow In dTblAuthenticationMethods.Rows
                        tmpAuthentication.Add(New roPassportAuthenticationMethodsRow With {
                                      .IDPassport = oPassport.ID,
                                      .BiometricData = If(IsDBNull(oRow("BiometricData")), Nothing, oRow("BiometricData")),
                                      .BiometricID = oRow("BiometricID"),
                                      .BiometricAlgorithm = roTypes.Any2String(oRow("BiometricAlgorithm")),
                                      .BiometricTerminalId = roTypes.Any2Integer(oRow("BiometricTerminalId")),
                                      .BloquedAccessApp = If(IsDBNull(oRow("BloquedAccessApp")), False, oRow("BloquedAccessApp")),
                                      .Credential = oRow("Credential"),
                                      .Enabled = If(IsDBNull(oRow("Enabled")), False, oRow("Enabled")),
                                      .ExpirationDate = If(IsDBNull(oRow("ExpirationDate")), Nothing, oRow("ExpirationDate")),
                                      .InvalidAccessAttemps = If(IsDBNull(oRow("InvalidAccessAttemps")), 0, oRow("InvalidAccessAttemps")),
                                      .LastDateInvalidAccessAttempted = If(IsDBNull(oRow("LastDateInvalidAccessAttempted")), Nothing, oRow("LastDateInvalidAccessAttempted")),
                                      .LastUpdatePassword = If(IsDBNull(oRow("LastUpdatePassword")), Nothing, oRow("LastUpdatePassword")),
                                      .Method = oRow("Method"),
                                      .Password = oRow("Password"),
                                      .StartDate = If(IsDBNull(oRow("StartDate")), Nothing, oRow("StartDate")),
                                      .TimeStamp = If(IsDBNull(oRow("TimeStamp")), Nothing, oRow("TimeStamp")),
                                      .Version = oRow("Version"),
                                      .RowState = RowState.NoChangeRow
                                          })
                    Next
                    oRet.CardRows = tmpAuthentication.ToArray

                End If

                strSQL = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.Biometry
                dTblAuthenticationMethods = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    Dim tmpAuthentication As New Generic.List(Of roPassportAuthenticationMethodsRow)

                    For Each oRow As DataRow In dTblAuthenticationMethods.Rows
                        tmpAuthentication.Add(New roPassportAuthenticationMethodsRow With {
                                      .IDPassport = oPassport.ID,
                                      .BiometricData = If(IsDBNull(oRow("BiometricData")), Nothing, oRow("BiometricData")),
                                      .BiometricID = oRow("BiometricID"),
                                      .BiometricAlgorithm = roTypes.Any2String(oRow("BiometricAlgorithm")),
                                      .BiometricTerminalId = roTypes.Any2Integer(oRow("BiometricTerminalId")),
                                      .BloquedAccessApp = If(IsDBNull(oRow("BloquedAccessApp")), False, oRow("BloquedAccessApp")),
                                      .Credential = oRow("Credential"),
                                      .Enabled = If(IsDBNull(oRow("Enabled")), False, oRow("Enabled")),
                                      .ExpirationDate = If(IsDBNull(oRow("ExpirationDate")), Nothing, oRow("ExpirationDate")),
                                      .InvalidAccessAttemps = If(IsDBNull(oRow("InvalidAccessAttemps")), 0, oRow("InvalidAccessAttemps")),
                                      .LastDateInvalidAccessAttempted = If(IsDBNull(oRow("LastDateInvalidAccessAttempted")), Nothing, oRow("LastDateInvalidAccessAttempted")),
                                      .LastUpdatePassword = If(IsDBNull(oRow("LastUpdatePassword")), Nothing, oRow("LastUpdatePassword")),
                                      .Method = oRow("Method"),
                                      .Password = oRow("Password"),
                                      .StartDate = If(IsDBNull(oRow("StartDate")), Nothing, oRow("StartDate")),
                                      .TimeStamp = If(IsDBNull(oRow("TimeStamp")), Nothing, oRow("TimeStamp")),
                                      .Version = oRow("Version"),
                                      .RowState = RowState.NoChangeRow
                                          })
                    Next
                    oRet.BiometricRows = tmpAuthentication.ToArray

                End If

                strSQL = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.Plate
                dTblAuthenticationMethods = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    Dim tmpAuthentication As New Generic.List(Of roPassportAuthenticationMethodsRow)

                    For Each oRow As DataRow In dTblAuthenticationMethods.Rows
                        tmpAuthentication.Add(New roPassportAuthenticationMethodsRow With {
                                      .IDPassport = oPassport.ID,
                                      .BiometricData = If(IsDBNull(oRow("BiometricData")), Nothing, oRow("BiometricData")),
                                      .BiometricID = oRow("BiometricID"),
                                      .BiometricAlgorithm = roTypes.Any2String(oRow("BiometricAlgorithm")),
                                      .BiometricTerminalId = roTypes.Any2Integer(oRow("BiometricTerminalId")),
                                      .BloquedAccessApp = If(IsDBNull(oRow("BloquedAccessApp")), False, oRow("BloquedAccessApp")),
                                      .Credential = oRow("Credential"),
                                      .Enabled = If(IsDBNull(oRow("Enabled")), False, oRow("Enabled")),
                                      .ExpirationDate = If(IsDBNull(oRow("ExpirationDate")), Nothing, oRow("ExpirationDate")),
                                      .InvalidAccessAttemps = If(IsDBNull(oRow("InvalidAccessAttemps")), 0, oRow("InvalidAccessAttemps")),
                                      .LastDateInvalidAccessAttempted = If(IsDBNull(oRow("LastDateInvalidAccessAttempted")), Nothing, oRow("LastDateInvalidAccessAttempted")),
                                      .LastUpdatePassword = If(IsDBNull(oRow("LastUpdatePassword")), Nothing, oRow("LastUpdatePassword")),
                                      .Method = oRow("Method"),
                                      .Password = oRow("Password"),
                                      .StartDate = If(IsDBNull(oRow("StartDate")), Nothing, oRow("StartDate")),
                                      .TimeStamp = If(IsDBNull(oRow("TimeStamp")), Nothing, oRow("TimeStamp")),
                                      .Version = oRow("Version"),
                                      .RowState = RowState.NoChangeRow
                                          })
                    Next
                    oRet.PlateRows = tmpAuthentication.ToArray

                End If

                strSQL = "@SELECT# * From sysroPassports_AuthenticationMethods WHERE IDPassport=" & oPassport.ID.ToString & " AND Method = " & AuthenticationMethod.NFC
                dTblAuthenticationMethods = CreateDataTable(strSQL)
                If dTblAuthenticationMethods IsNot Nothing AndAlso dTblAuthenticationMethods.Rows.Count > 0 Then
                    oRet.NFCRow = New roPassportAuthenticationMethodsRow With {
                                     .IDPassport = oPassport.ID,
                                  .BiometricData = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BiometricData")), Nothing, dTblAuthenticationMethods.Rows(0)("BiometricData")),
                                  .BiometricID = dTblAuthenticationMethods.Rows(0)("BiometricID"),
                                  .BiometricAlgorithm = roTypes.Any2String(dTblAuthenticationMethods.Rows(0)("BiometricAlgorithm")),
                                  .BiometricTerminalId = roTypes.Any2Integer(dTblAuthenticationMethods.Rows(0)("BiometricTerminalId")),
                                  .BloquedAccessApp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")), False, dTblAuthenticationMethods.Rows(0)("BloquedAccessApp")),
                                  .Credential = dTblAuthenticationMethods.Rows(0)("Credential"),
                                  .Enabled = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("Enabled")), False, dTblAuthenticationMethods.Rows(0)("Enabled")),
                                  .ExpirationDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("ExpirationDate")), Nothing, dTblAuthenticationMethods.Rows(0)("ExpirationDate")),
                                  .InvalidAccessAttemps = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")), 0, dTblAuthenticationMethods.Rows(0)("InvalidAccessAttemps")),
                                  .LastDateInvalidAccessAttempted = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")), Nothing, dTblAuthenticationMethods.Rows(0)("LastDateInvalidAccessAttempted")),
                                  .LastUpdatePassword = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")), Nothing, dTblAuthenticationMethods.Rows(0)("LastUpdatePassword")),
                                  .Method = dTblAuthenticationMethods.Rows(0)("Method"),
                                  .Password = dTblAuthenticationMethods.Rows(0)("Password"),
                                  .StartDate = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("StartDate")), Nothing, dTblAuthenticationMethods.Rows(0)("StartDate")),
                                  .TimeStamp = If(IsDBNull(dTblAuthenticationMethods.Rows(0)("TimeStamp")), Nothing, dTblAuthenticationMethods.Rows(0)("TimeStamp")),
                                  .Version = dTblAuthenticationMethods.Rows(0)("Version"),
                                  .RowState = RowState.NoChangeRow
                                      }
                End If


                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadAuthenticationMethods")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadAuthenticationMethods")
            End Try

            Return oRet

        End Function

        Private Function LoadCategories(ByVal oPassport As roPassport) As roPassportCategories
            '
            ' Cargamos las categorias que puede gestionar el passport y su configuración
            '

            Dim oRet As roPassportCategories = Nothing
            Dim bolRet As Boolean = False

            Try

                oRet = New roPassportCategories With {.idPassport = oPassport.ID, .CategoryRows = Nothing}

                Dim strSQL As String = "@SELECT# * From sysroPassports_Categories WHERE IDPassport=" & oPassport.ID.ToString
                Dim dTblCategories As DataTable = CreateDataTable(strSQL)
                If dTblCategories IsNot Nothing AndAlso dTblCategories.Rows.Count > 0 Then
                    Dim tmpCategory As New Generic.List(Of roPassportCategoryRow)

                    For Each oRow As DataRow In dTblCategories.Rows
                        tmpCategory.Add(New roPassportCategoryRow With {
                                      .IDPassport = oPassport.ID,
                                      .IDCategory = oRow("IDCategory"),
                                      .LevelOfAuthority = oRow("LevelOfAuthority"),
                                      .ShowFromLevel = oRow("ShowFromLevel"),
                                      .RowState = RowState.NoChangeRow
                                          })
                    Next
                    oRet.CategoryRows = tmpCategory.ToArray

                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadCategories")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadCategories")
            End Try

            Return oRet

        End Function

        Private Function LoadExceptions(ByVal oPassport As roPassport) As roPassportExceptions
            '
            ' Cargamos las categorias que puede gestionar el passport
            '

            Dim oRet As roPassportExceptions = Nothing
            Dim bolRet As Boolean = False

            Try

                oRet = New roPassportExceptions With {.idPassport = oPassport.ID, .Exceptions = Nothing}

                Dim strSQL As String = $"@SELECT# distinct sysroPassports_Employees.*,ceg.EmployeeName, ceg.FullGroupName From sysroPassports_Employees
                                            INNER JOIN sysrovwAllEmployeeGroups ceg ON sysroPassports_Employees.IDEmployee = ceg.IDEmployee
                                            WHERE IDPassport={oPassport.ID.ToString}"
                Dim dTblGroups As DataTable = CreateDataTable(strSQL)
                If dTblGroups IsNot Nothing AndAlso dTblGroups.Rows.Count > 0 Then
                    Dim tmpGroup As New Generic.List(Of roPassportExceptionRow)

                    For Each oRow As DataRow In dTblGroups.Rows
                        tmpGroup.Add(New roPassportExceptionRow With {
                                      .IDPassport = oPassport.ID,
                                      .IDEmployee = oRow("IDEmployee"),
                                      .Name = $"{oRow("EmployeeName")} ({oRow("FullGroupName")})",
                                      .Available = roTypes.Any2Boolean(oRow("Permission"))
                                    })
                    Next
                    oRet.Exceptions = tmpGroup.ToArray
                Else
                    oRet.Exceptions = {}
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadExceptions")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadExceptions")
            Finally

            End Try

            Return oRet

        End Function

        Private Function LoadGroups(ByVal oPassport As roPassport) As roPassportGroups
            '
            ' Cargamos las categorias que puede gestionar el passport
            '

            Dim oRet As roPassportGroups = Nothing
            Dim bolRet As Boolean = False

            Try

                oRet = New roPassportGroups With {.idPassport = oPassport.ID, .GroupRows = Nothing}

                Dim strSQL As String = "@SELECT# * From sysroPassports_Groups WHERE IDPassport=" & oPassport.ID.ToString
                Dim dTblGroups As DataTable = CreateDataTable(strSQL)
                If dTblGroups IsNot Nothing AndAlso dTblGroups.Rows.Count > 0 Then
                    Dim tmpGroup As New Generic.List(Of roPassportGroupRow)

                    For Each oRow As DataRow In dTblGroups.Rows
                        tmpGroup.Add(New roPassportGroupRow With {
                                      .IDPassport = oPassport.ID,
                                      .IDGroup = oRow("IDGroup")
                                          })
                    Next
                    oRet.GroupRows = tmpGroup.ToArray

                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::LoadCategories")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::LoadCategories")
            Finally

            End Try

            Return oRet

        End Function

        Public Shared Function GetEmployeeActiveContract(ByVal oPassport As roPassport) As DataTable

            Dim oRet As DataTable = Nothing
            Dim bolRet As Boolean = False

            Try

                Dim strSQL As String = "@SELECT# * FROM EmployeeContracts INNER Join sysroPassports On EmployeeContracts.IDEmployee = sysroPassports.IDEmployee Where sysroPassports.ID = " & oPassport.ID.ToString & " And EmployeeContracts.BeginDate <= GETDATE() And EmployeeContracts.EndDate >= GETDATE()"

                oRet = CreateDataTable(strSQL)

                bolRet = True
            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPassportManager::GetEmployeeActiveContract::Unknown error", ex)
            End Try

            Return oRet

        End Function

        Private Function SetDefaultEmployeePerissions(ByVal oPassport As roPassport) As Boolean
            Dim bolRet As Boolean

            Try

                bolRet = DataLayer.AccessHelper.ExecuteSql($"@DELETE# sysroPassports_PermissionsOverFeatures WHERE IDPassport={oPassport.ID}")
                bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20,6)")
                bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20000,3)")
                bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20001,6)")
                bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20100,6)")
                bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20110,6)")

                If DataLayer.roCacheManager.GetInstance().GetAdvParametersCache(RoAzureSupport.GetCompanyName(), "Customization") = "CRUZROJA" Then
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20120,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20140,0)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20130,0)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},20150,0)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21200,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21000,3)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21100,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21140,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21150,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21160,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21170,0)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21110,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21130,0)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},21120,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},26,6)")
                    bolRet = bolRet AndAlso DataLayer.AccessHelper.ExecuteSql($"@INSERT# INTO sysroPassports_PermissionsOverFeatures (IDPassport, IdFeature, Permission) values ({oPassport.ID},26001,3)")
                End If

            Catch ex As Exception
                roLog.GetInstance().logMessage(roLog.EventType.roError, "roPassportManager::SetDefaultEmployeePerissions::Unknown error", ex)
            End Try

            Return bolRet
        End Function

        Private Function SaveAuthenticationMethods(ByRef oPassport As roPassport, ByVal bolIsNewPassport As Boolean, ByRef bLaunchBroadcaster As Boolean) As Boolean
            '
            ' Guardamos los metodos de autentificación del passport
            '

            Dim bolRet As Boolean = False
            Dim bolNewPassword As Boolean = False

            Try
                bLaunchBroadcaster = False
                Dim strSQL As String = ""
                Dim strCode As StringBuilder = New StringBuilder()

                Dim oOldPassport As roPassport = Me.LoadPassport(oPassport.ID, LoadType.Passport)

                Dim oParameters As roParameters = New roParameters("OPTIONS")
                Dim bBiometricIdentificationDisabled As Boolean = roTypes.Any2Boolean(oParameters.Parameter(Parameters.DisableBiometricData))

                If oPassport.AuthenticationMethods IsNot Nothing Then
                    If IsValidAuthenticationMethods(oPassport) Then
                        bolRet = True
                        For iNthMethod = 1 To 7
                            Dim bolRedim As Boolean = False
                            Dim oPAM As roPassportAuthenticationMethodsRow() = Nothing

                            Select Case iNthMethod
                                Case 1
                                    oPAM = oPassport.AuthenticationMethods.CardRows
                                Case 2
                                    oPAM = oPassport.AuthenticationMethods.PlateRows
                                Case 3
                                    oPAM = oPassport.AuthenticationMethods.BiometricRows
                                Case 4, 5, 6, 7
                                    ReDim oPAM(0)
                                    bolRedim = True
                                    If iNthMethod = 4 Then
                                        oPAM(0) = oPassport.AuthenticationMethods.PasswordRow
                                    ElseIf iNthMethod = 5 Then
                                        oPAM(0) = oPassport.AuthenticationMethods.IntegratedSecurityRow
                                    ElseIf iNthMethod = 6 Then
                                        oPAM(0) = oPassport.AuthenticationMethods.PinRow
                                    ElseIf iNthMethod = 7 Then
                                        oPAM(0) = oPassport.AuthenticationMethods.NFCRow
                                    End If
                            End Select

                            If bolRedim AndAlso oPAM(0) Is Nothing Then oPAM = Nothing

                            If oPAM IsNot Nothing Then
                                For Each oAM As roPassportAuthenticationMethodsRow In oPAM.OrderByDescending((Function(f) f.RowState))
                                    If oAM IsNot Nothing Then
                                        oAM.IDPassport = oPassport.ID
                                        If oAM.RowState = RowState.DeleteRow Then
                                            ' Borramos el registro de la BBDD
                                            strSQL = "@DELETE#  From sysroPassports_AuthenticationMethods WHERE IDPassport = " & oPassport.ID.ToString & " AND Method=" & oAM.Method & " AND Version='" & oAM.Version & "'" & " AND BiometricID=" & oAM.BiometricID.ToString
                                            ExecuteSql(strSQL)

                                            If oAM.Method <> AuthenticationMethod.NFC Then bLaunchBroadcaster = True
                                        End If
                                        If oAM.RowState = RowState.NewRow OrElse oAM.RowState = RowState.UpdateRow Then
                                            If oAM.Method = AuthenticationMethod.Password Then
                                                'Si la autenticacion real se hace bajo AD, en VTLive se hará con el pass igual al nombre del usuario porque el pass de la BBDD es irrelevante
                                                'solo se usa a efectos de configuracion de Web services
                                                If oPassport.AuthenticationMethods.PasswordRow.Credential.Contains("\") Then
                                                    oPassport.AuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(oPassport.AuthenticationMethods.PasswordRow.Credential.ToLower())
                                                ElseIf oPassport.AuthenticationMethods.PasswordRow.Password.Length = 0 Then
                                                    ' Si no tiene contraseña
                                                    ' tenemos que generar una nueva y enviarla al usuario

                                                    Dim r As New Random()
                                                    For k As Integer = 1 To 10
                                                        strCode.Append(CStr(r.Next(0, 9)))
                                                    Next

                                                    oPassport.AuthenticationMethods.PasswordRow.Password = CryptographyHelper.EncryptWithMD5(strCode.ToString)
                                                    Dim xDate As Date = roTypes.CreateDateTime(1900, 1, 1)
                                                    oPassport.AuthenticationMethods.PasswordRow.LastUpdatePassword = xDate
                                                    oPassport.AuthenticationMethods.PasswordRow.InvalidAccessAttemps = 0
                                                    oPassport.AuthenticationMethods.PasswordRow.LastDateInvalidAccessAttempted = Nothing

                                                    bolNewPassword = True
                                                End If

                                                If Not bolIsNewPassport Then
                                                    Dim oPass As roPassport = LoadPassport(oPassport.ID, LoadType.Passport)
                                                    If oPass IsNot Nothing AndAlso oPass.AuthenticationMethods IsNot Nothing AndAlso oPass.AuthenticationMethods.PasswordRow IsNot Nothing Then
                                                        If oPass.AuthenticationMethods.PasswordRow.Password <> oPassport.AuthenticationMethods.PasswordRow.Password Then
                                                            SavePasswordHistory(oPassport.ID, oPass.AuthenticationMethods.PasswordRow().Password, Now)
                                                            oPassport.AuthenticationMethods.PasswordRow.LastUpdatePassword = Now
                                                            oPassport.AuthenticationMethods.PasswordRow.InvalidAccessAttemps = 0
                                                            oPassport.AuthenticationMethods.PasswordRow.LastDateInvalidAccessAttempted = Nothing
                                                        End If
                                                    End If
                                                End If

                                                'Integracion con Visits
                                                If oPassport.IDEmployee.HasValue OrElse oPassport.IDEmployee > 0 Then
                                                    Dim sPass As String = oAM.Password
                                                    Dim sSQL As String

                                                    If oPassport.AuthenticationMethods.PasswordRow.Enabled Then
                                                        If oPassport.AuthenticationMethods.PasswordRow.Credential.Contains("\") Then
                                                            sSQL = "@UPDATE# Employees SET WebLogin='" & oPassport.AuthenticationMethods.PasswordRow.Credential & "', " &
                                                            "WebPassword='" & sPass & "', " &
                                                            "ActiveDirectory = 1 " &
                                                            "WHERE ID = " & oPassport.IDEmployee.Value
                                                        Else
                                                            sSQL = "@UPDATE# Employees SET WebLogin='" & oPassport.AuthenticationMethods.PasswordRow.Credential & "', " &
                                                            "WebPassword='" & sPass & "', " &
                                                            "ActiveDirectory = 0 " &
                                                            "WHERE ID = " & oPassport.IDEmployee.Value
                                                        End If
                                                    Else
                                                        sSQL = "@UPDATE# Employees SET WebLogin='', WebPassword='', ActiveDirectory= 0  WHERE ID = " & oPassport.IDEmployee.Value
                                                    End If

                                                    ExecuteSql(sSQL)

                                                End If

                                                ' Si estaba bloqueado por inactividad, y me lo han desbloqueado, reseteo fecha de última acción para que no se vuelva a bloquear de nuevo
                                                If Not oAM.BloquedAccessApp Then
                                                    strSQL = "@UPDATE# sysroPassports_AuthenticationMethods SET LastAppActionDate = " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & ", BlockedAccessByInactivity = 0 " &
                                                             " WHERE BlockedAccessByInactivity = 1 AND Method = 1 AND IDPassport = " & oPassport.ID.ToString
                                                    ExecuteSql(strSQL)
                                                End If
                                            End If

                                            ' Creamos o actualizamos registro
                                            Dim tb As New DataTable("sysroPassports_AuthenticationMethods")
                                            strSQL = "@SELECT# * FROM sysroPassports_AuthenticationMethods WHERE IDPassport = " & oPassport.ID.ToString & " AND Method=" & oAM.Method & " AND Version='" & oAM.Version & "'" & " AND BiometricID=" & oAM.BiometricID.ToString
                                            Dim cmd As DbCommand = CreateCommand(strSQL)
                                            Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                                            da.Fill(tb)

                                            Dim oRow As DataRow
                                            If tb.Rows.Count = 0 Then
                                                oRow = tb.NewRow
                                            Else
                                                oRow = tb.Rows(0)
                                            End If
                                            oRow("IDPassport") = oPassport.ID
                                            oRow("BiometricData") = If(IsNothing(oAM.BiometricData), DBNull.Value, oAM.BiometricData)
                                            oRow("BiometricID") = oAM.BiometricID
                                            oRow("BloquedAccessApp") = oAM.BloquedAccessApp
                                            oRow("Credential") = oAM.Credential

                                            ' Si es un método de biometría, y está globalmente deshabilitada, la guardo deshabilitada
                                            If oAM.Method = AuthenticationMethod.Biometry AndAlso bBiometricIdentificationDisabled Then
                                                oAM.Enabled = False
                                            End If

                                            oRow("Enabled") = oAM.Enabled
                                            oRow("ExpirationDate") = If(oAM.ExpirationDate = Nothing, DBNull.Value, oAM.ExpirationDate)
                                            oRow("InvalidAccessAttemps") = oAM.InvalidAccessAttemps
                                            oRow("LastDateInvalidAccessAttempted") = If(oAM.LastDateInvalidAccessAttempted = Nothing, DBNull.Value, oAM.LastDateInvalidAccessAttempted)
                                            oRow("LastUpdatePassword") = If(oAM.LastUpdatePassword = Nothing, DBNull.Value, oAM.LastUpdatePassword)
                                            oRow("Method") = oAM.Method
                                            oRow("Password") = oAM.Password
                                            oRow("StartDate") = If(oAM.StartDate = Nothing, DBNull.Value, oAM.StartDate)
                                            oRow("TimeStamp") = If(oAM.TimeStamp = Nothing, DBNull.Value, oAM.TimeStamp)
                                            oRow("Version") = oAM.Version
                                            oRow("BiometricTerminalId") = If(oAM.BiometricTerminalId = 0, DBNull.Value, roTypes.Any2Integer(oAM.BiometricTerminalId))
                                            oRow("BiometricAlgorithm") = roTypes.Any2String(oAM.BiometricAlgorithm)

                                            If oPassport.IDEmployee.HasValue AndAlso oAM.Method = AuthenticationMethod.Card AndAlso oOldPassport.AuthenticationMethods IsNot Nothing AndAlso
                                                oOldPassport.AuthenticationMethods.CardRows IsNot Nothing AndAlso oOldPassport.AuthenticationMethods.CardRows.Any() AndAlso oOldPassport.AuthenticationMethods.CardRows(0).Credential <> oAM.Credential Then
                                                roTimeStamps.UpdateEmployeeTimestamp(oPassport.IDEmployee.Value)
                                            End If

                                            If oPassport.IDEmployee.HasValue AndAlso oAM.Method = AuthenticationMethod.Password AndAlso oOldPassport.AuthenticationMethods IsNot Nothing AndAlso
                                                oOldPassport.AuthenticationMethods.PasswordRow IsNot Nothing AndAlso oOldPassport.AuthenticationMethods.PasswordRow.Credential <> oAM.Credential Then
                                                roTimeStamps.UpdateEmployeeTimestamp(oPassport.IDEmployee.Value)
                                            End If

                                            If oPassport.IDEmployee.HasValue AndAlso oAM.Method = AuthenticationMethod.NFC AndAlso oOldPassport.AuthenticationMethods IsNot Nothing AndAlso
                                                oOldPassport.AuthenticationMethods.NFCRow IsNot Nothing AndAlso oOldPassport.AuthenticationMethods.NFCRow.Credential <> oAM.Credential Then
                                                roTimeStamps.UpdateEmployeeTimestamp(oPassport.IDEmployee.Value)
                                            End If


                                            If tb.Rows.Count = 0 Then
                                                tb.Rows.Add(oRow)
                                            End If
                                            da.Update(tb)

                                            If oAM.Method <> AuthenticationMethod.NFC Then bLaunchBroadcaster = True

                                        End If
                                    End If
                                Next
                            End If
                        Next
                    Else
                        bolRet = False
                    End If
                Else
                    bolRet = True
                End If

                If bolRet AndAlso bolNewPassword Then
                    ' Creamos tarea para enviar el password y marcamos como caducado solo en el caso que
                    ' no este configurado como active directory

                    Dim intIDEmployee As Integer = 0

                    If oPassport.IDEmployee.HasValue Then
                        intIDEmployee = oPassport.IDEmployee
                    End If

                    'Solo enviaremos el mail si el servicio esta activo
                    Dim sActive As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar("@SELECT# Data FROM sysroParameters WHERE ID = 'ACTIVE'"))
                    If roPassportManager.IsRoboticsUserOrConsultant(oPassport.ID) OrElse sActive = 1 Then
                        strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES (1903, " & intIDEmployee & ", " & oPassport.ID & ",'" & CryptographyHelper.Encrypt(strCode.ToString) & "')"
                        AccessHelper.ExecuteSql(strSQL)
                        strSQL = "@INSERT# INTO sysroNotificationTasks (IDNotification, Key1Numeric, Key2Numeric, Parameters ) VALUES (1905, " & intIDEmployee & ", " & oPassport.ID & ",'" & oPassport.AuthenticationMethods.PasswordRow.Credential & "')"
                        AccessHelper.ExecuteSql(strSQL)
                    End If
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::SaveAuthenticationMethods")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::SaveAuthenticationMethods")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Function SaveCategories(ByVal oPassport As roPassport) As Boolean
            '
            ' Guardamos las categorias asignadas al supervisor
            '

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = ""

                ' Borramos todas las categorias
                strSQL = "@DELETE#  From sysroPassports_Categories WHERE IDPassport = " & oPassport.ID.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet AndAlso oPassport.Categories IsNot Nothing AndAlso oPassport.Categories.CategoryRows IsNot Nothing Then
                    For Each oCat As roPassportCategoryRow In oPassport.Categories.CategoryRows.OrderByDescending((Function(f) f.RowState))
                        oCat.IDPassport = oPassport.ID
                        ' Creamos o actualizamos registro
                        Dim tb As New DataTable("sysroPassports_Categories")
                        strSQL = "@SELECT# * FROM sysroPassports_Categories WHERE IDPassport = " & oPassport.ID.ToString & " AND IDCategory = " & oCat.IDCategory
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                        Else
                            oRow = tb.Rows(0)
                        End If
                        oRow("IDPassport") = oPassport.ID
                        oRow("IDCategory") = oCat.IDCategory
                        oRow("LevelOfAuthority") = oCat.LevelOfAuthority
                        oRow("ShowFromLevel") = oCat.ShowFromLevel
                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::SaveCategories")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::SaveCategories")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Function SaveGroups(ByVal oPassport As roPassport) As Boolean
            '
            ' Guardamos los grupos que puede gestionar el supervisor
            '

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = ""

                ' Borramos todos los grupos
                strSQL = "@DELETE#  From sysroPassports_Groups WHERE IDPassport = " & oPassport.ID.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet AndAlso oPassport.Groups IsNot Nothing AndAlso oPassport.Groups.GroupRows IsNot Nothing Then
                    For Each oGro As roPassportGroupRow In oPassport.Groups.GroupRows.OrderByDescending((Function(f) f.RowState))
                        oGro.IDPassport = oPassport.ID
                        ' Creamos o actualizamos registro
                        Dim tb As New DataTable("sysroPassports_Groups")
                        strSQL = "@SELECT# * FROM sysroPassports_Groups WHERE IDPassport = " & oPassport.ID.ToString & " AND IDGroup = " & oGro.IDGroup.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                        Else
                            oRow = tb.Rows(0)
                        End If
                        oRow("IDPassport") = oPassport.ID
                        oRow("IDGroup") = oGro.IDGroup
                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::SaveGroups")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::SaveGroups")
                bolRet = False
            Finally

            End Try

            Return bolRet

        End Function

        Private Function SaveExceptions(ByVal oPassport As roPassport) As Boolean
            '
            ' Guardamos los grupos que puede gestionar el supervisor
            '

            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = ""

                ' Borramos todos los grupos
                strSQL = "@DELETE#  From sysroPassports_Employees WHERE IDPassport = " & oPassport.ID.ToString
                bolRet = ExecuteSql(strSQL)

                If bolRet AndAlso oPassport.Exceptions IsNot Nothing AndAlso oPassport.Exceptions.Exceptions IsNot Nothing Then
                    For Each oException As roPassportExceptionRow In oPassport.Exceptions.Exceptions
                        oException.IDPassport = oPassport.ID
                        ' Creamos o actualizamos registro
                        Dim tb As New DataTable("sysroPassports_Groups")
                        strSQL = "@SELECT# * FROM sysroPassports_Employees WHERE IDPassport = " & oPassport.ID.ToString & " AND IDEmployee = " & oException.IDEmployee.ToString
                        Dim cmd As DbCommand = CreateCommand(strSQL)
                        Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                        da.Fill(tb)

                        Dim oRow As DataRow
                        If tb.Rows.Count = 0 Then
                            oRow = tb.NewRow
                        Else
                            oRow = tb.Rows(0)
                        End If
                        oRow("IDPassport") = oPassport.ID
                        oRow("IDEmployee") = oException.IDEmployee
                        oRow("Permission") = If(oException.Available, 1, 0)
                        If tb.Rows.Count = 0 Then
                            tb.Rows.Add(oRow)
                        End If
                        da.Update(tb)
                    Next
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::SaveExceptions")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::SaveExceptions")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Private Function CreateUser(ByRef oPassport As roPassport) As Boolean
            '
            ' Creamos usuario relacionado con el passport
            '
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = ""

                ' Creamos o actualizamos registro
                Dim tb As New DataTable("sysroUsers")
                strSQL = "@SELECT# * FROM sysroUsers WHERE Login  like '" & "´WebLogin´ : " & oPassport.ID.ToString & "'"
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)

                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                Else
                    oRow = tb.Rows(0)
                End If
                oRow("Login") = "´WebLogin´ : " & oPassport.ID.ToString
                oRow("Password") = DBNull.Value
                oRow("IDSecurityGroup") = ""
                If tb.Rows.Count = 0 Then
                    tb.Rows.Add(oRow)
                End If
                da.Update(tb)

                Dim tbNew As DataTable = CreateDataTable("@SELECT# TOP 1 [IDUser] FROM sysroUsers WHERE Login like '" & oRow("Login") & "'" & " ORDER BY [IDUser] DESC")
                If tbNew IsNot Nothing AndAlso tbNew.Rows.Count = 1 Then
                    oPassport.IDUser = tbNew.Rows(0)("IDUser")
                End If

                bolRet = True
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::CreateUser")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::CreateUser")
            Finally

            End Try

            Return bolRet

        End Function

        Private Function AddParentGroup(ByRef oPassport As roPassport, Optional ByVal bolLaunchSecurityTask As Boolean = True, Optional ByVal audit As Boolean = False) As Boolean
            '
            ' Creamos Grupo padre del supervisor
            '
            Dim bolRet As Boolean = False

            Try
                Dim strSQL As String = ""

                Dim oParentPassport As New roPassport
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
                oParentPassport.Language = oPassport.Language

                bolRet = Save(oParentPassport, audit, bolLaunchSecurityTask)

                ' Asignamos al supervisor el grupo creado
                oPassport.IDParentPassport = oParentPassport.ID
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::AddParentGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::AddParentGroup")
            Finally

            End Try

            Return bolRet

        End Function

        ''' <summary>
        ''' Returns the ids of passports having the specified role
        ''' </summary>
        ''' <param name="idRole">The role of the passports to list.</param>
        ''' <param name="groupType">The type of passports to return, or Nothing to return all types.</param>
        Public Function GetPassportsByRole(ByVal idRole As Nullable(Of Integer), Optional ByVal _State As roSecurityState = Nothing) As List(Of Integer)
            Dim strSQL = "@SELECT# ID " &
                            " FROM sysropassports" &
                            " WHERE IdGroupFeature = " & idRole & " ORDER BY GroupType, Name"

            Dim tb = CreateDataTable(strSQL, )
            Dim Result As New List(Of Integer)
            For Each row As DataRow In tb.Rows
                Result.Add(row("ID"))
            Next
            Return Result

        End Function

#End Region

#Region "Public Helpers"

        Public Shared Function SetLastNotificationSended(ByVal idPassport As Integer, ByVal oDate As Nullable(Of DateTime)) As Boolean
            Dim bRet As Boolean = False

            Dim strDate As String = "NULL"

            If oDate.HasValue Then
                strDate = VTBase.roTypes.Any2Time(oDate.Value).SQLDateTime
            End If

            Dim strSQL As String = "IF EXISTS(@SELECT# 1 from sysroPassports_Data where IDPassport= " & idPassport & " and AppCode='" & roAppType.VTLive.ToString() & "')" &
                                    " BEGIN" &
                                    " @UPDATE# sysroPassports_Data SET LastRequestNotification = " & strDate & " WHERE IDPassport = " & idPassport & " and AppCode='" & roAppType.VTLive.ToString() & "'" &
                                    " END"

            bRet = AccessHelper.ExecuteSql(strSQL)

            Return bRet
        End Function

        Public Shared Function IsRoboticsUserOrConsultant(ByVal idPassport As Integer) As Boolean
            Dim isSupervisor As Boolean
            Try
                Dim strSQL As String = "@SELECT# 1 FROM sysroPassports sp inner join sysroGroupFeatures sgp on sp.IDGroupFeature = sgp.ID and sgp.Name like '%@@ROBOTICS@@%' WHERE sp.IsSupervisor = 1 AND sp.ID = " & idPassport
                isSupervisor = roTypes.Any2Boolean(AccessHelper.ExecuteScalar(strSQL))
            Catch ex As Exception
                isSupervisor = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "IsRoboticsUserOrConsultant::", ex)
            End Try


            Return isSupervisor
        End Function

        Public Shared Function IsConsultant(ByVal idPassport As Integer) As Boolean
            Dim isSupervisor As Boolean
            Try
                Dim strSQL As String = "@SELECT# 1 FROM sysroPassports sp inner join sysroGroupFeatures sgp on sp.IDGroupFeature = sgp.ID and sgp.Name like '%@@ROBOTICS@@Consultores%' WHERE sp.IsSupervisor = 1 AND sp.ID = " & idPassport
                isSupervisor = roTypes.Any2Boolean(AccessHelper.ExecuteScalar(strSQL))
            Catch ex As Exception
                isSupervisor = False
                roLog.GetInstance().logMessage(roLog.EventType.roError, "IsRoboticsUserOrConsultant::", ex)
            End Try


            Return isSupervisor
        End Function

        Public Shared Function GetLastNotificationSended(ByVal idPassport As Integer) As Nullable(Of DateTime)
            Dim bRet As Nullable(Of DateTime) = Nothing

            Dim strSQL As String = "@SELECT# LastRequestNotification FROM sysroPassports_Data WHERE IDPassport = " & idPassport & " AND AppCode='" & roAppType.VTLive.ToString() & "'"
            Dim objLastRequestNotification As Object = AccessHelper.ExecuteScalar(strSQL)

            If objLastRequestNotification IsNot Nothing AndAlso Not IsDBNull(objLastRequestNotification) Then
                bRet = VTBase.roTypes.Any2DateTime(objLastRequestNotification)
            End If

            Return bRet
        End Function



        Public Shared Function SetTimeZoneData(ByVal appSource As roAppType, ByVal idPassport As Integer, ByVal timeZone As String, ByRef oState As roSecurityState) As Boolean
            Dim bRet As Boolean = False

            Dim sessionContext As String = String.Empty
            If oState IsNot Nothing Then sessionContext = oState.SessionID & "%" & oState.ClientAddress

            Dim strSQL As String = "IF EXISTS(@SELECT# 1 from sysroPassports_Data where IDPassport = " & idPassport & " AND AppCode='" & appSource.ToString() & "')" &
                                    " BEGIN" &
                                    " @UPDATE# sysroPassports_Data SET TimeZone = '" & timeZone & "' WHERE IDPassport = " & idPassport & " AND AppCode='" & appSource.ToString() & "'" &
                                    " END"

            bRet = AccessHelper.ExecuteSql(strSQL)

            Return bRet
        End Function

        Public Shared Function SetLicenseAgreementValidation(ByVal idPassport As Integer, ByVal bAcceptLicense As Boolean, ByRef oState As roSecurityState) As Boolean
            Dim bRet As Boolean = False

            Dim sessionContext As String = String.Empty

            Dim strSQL As String = " @UPDATE# sysroPassports SET LicenseAccepted = " & If(bAcceptLicense, 1, 0) & " WHERE ID = " & idPassport

            bRet = AccessHelper.ExecuteSql(strSQL)

            Return bRet
        End Function

        Public Shared Function GetPassportBelongsToAdminGroup(ByVal intIdPassport As Integer) As Boolean
            Dim belongsToAdmin As Boolean = False

            Dim strSQL As String = "@SELECT# COUNT(*) as BelongsToAdmin FROM (@SELECT# ID as GROUPID from GetPassportParents(" & intIdPassport & ") WHERE ID = 3) gpp"
            Dim belongs As Integer = roTypes.Any2Integer(AccessHelper.ExecuteScalar(strSQL))

            If belongs = 1 Then
                belongsToAdmin = True
            End If
            Return belongsToAdmin
        End Function

        Public Shared Function GetRequestPassportLevelOfAuthority(ByRef oState As roPassportState, ByVal IDPassport As Integer, ByVal xRequestType As eRequestType, ByVal IDCause As Integer, ByVal IdRequest As Integer) As Integer
            ' Nivel de mando del empleado en funcion del tipo de solicitud
            Dim iLevel As Integer = 0

            Try
                Dim sqlCommand As String = String.Empty
                sqlCommand = $"@SELECT# SupervisorLevelOfAuthority FROM sysrovwSecurity_PermissionOverRequests WHERE IdPassport = @idPassport AND IdRequest = @idrequest {DataLayer.SQLServerHint.GetSQLHint(SQLServerHint.SelectHinted.GetRequestPassportLevelOfAuthority)}"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, IDPassport))
                parameters.Add(New CommandParameter("@idrequest", CommandParameter.ParameterType.tInt, IdRequest))
                iLevel = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))

            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportLevelOfAuthority")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportLevelOfAuthority")
            End Try

            Return iLevel

        End Function

        Public Shared Function GetAllAvailableSupervisorsList(ByRef oState As roPassportState, Optional ByVal bLoadUserSystem As Boolean = False) As roPassport()
            Dim bRet As roPassport() = Nothing

            Dim tmpList As New Generic.List(Of roPassport)
            Dim strSQL As String
            Dim tb As DataTable

            Try

                strSQL = "@SELECT# srp.ID, srp.IDParentPassport, srp.IDEmployee, srp.Name, srp.GroupType, srp.IDGroupFeature as Role " &
                            " FROM sysropassports srp " &
                            " WHERE ((srp.GroupType = '') OR (srp.IsSupervisor = 1)) AND srp.Description not like '%@@ROBOTICS@@%' and GroupType <> 'U' Order by srp.Name"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                    For Each oRow As DataRow In tb.Rows
                        Dim tmpSecUser As New roPassport() With {
                            .ID = roTypes.Any2Integer(oRow("ID")),
                            .IDParentPassport = roTypes.Any2Integer(oRow("IDParentPassport")),
                            .GroupType = roTypes.Any2String(oRow("GroupType")),
                            .Name = roTypes.Any2String(oRow("Name")),
                            .IDGroupFeature = roTypes.Any2Integer(oRow("Role"))
                        }
                        tmpList.Add(tmpSecUser)
                    Next
                End If

                If bLoadUserSystem Then

                    strSQL = "@SELECT# srp.ID, srp.IDParentPassport, srp.IDEmployee, srp.Name, srp.GroupType, srp.IDGroupFeature as Role  " &
                                " FROM sysropassports srp " &
                                " WHERE ((srp.GroupType = '') OR (srp.IsSupervisor = 1)) AND srp.Description like '%@@ROBOTICS@@System%'  and GroupType <> 'U' Order by srp.Name"

                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then

                        For Each oRow As DataRow In tb.Rows
                            Dim tmpSecUser As New roPassport() With {
                                .ID = roTypes.Any2Integer(oRow("ID")),
                                .IDParentPassport = roTypes.Any2Integer(oRow("IDParentPassport")),
                                .GroupType = roTypes.Any2String(oRow("GroupType")),
                                .Name = roTypes.Any2String(oRow("Name")),
                                .IDGroupFeature = roTypes.Any2Integer(oRow("Role"))
                            }
                            tmpList.Add(tmpSecUser)
                        Next
                    End If
                End If

                bRet = tmpList.ToArray
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportsSecurityNodeList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportsSecurityNodeList")
            End Try

            Return bRet
        End Function

        Public Shared Function GetAllSupervisors(ByRef oState As roPassportState, Optional ByVal bLoadUserSystem As Boolean = False) As roSupervisor()
            Dim bRet As roSupervisor() = Nothing

            Dim tmpList As New Generic.List(Of roSupervisor)
            Dim strSQL As String
            Dim tb As DataTable

            Try

                strSQL = "@SELECT# srp.ID " &
                            " FROM sysropassports srp " &
                            " WHERE ((srp.GroupType = '') OR (srp.IsSupervisor = 1)) AND srp.Description not like '%@@ROBOTICS@@%' and GroupType <> 'U' Order by srp.Name"

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        tmpList.Add(GetSupervisor(oRow("ID")))
                    Next
                End If

                If bLoadUserSystem Then

                    strSQL = "@SELECT# srp.ID " &
                                " FROM sysropassports srp " &
                                " WHERE ((srp.GroupType = '') OR (srp.IsSupervisor = 1)) AND srp.Description like '%@@ROBOTICS@@System%'  and GroupType <> 'U' Order by srp.Name"

                    tb = CreateDataTable(strSQL, )
                    If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                        For Each oRow As DataRow In tb.Rows
                            tmpList.Add(GetSupervisor(oRow("ID")))
                        Next
                    End If
                End If

                bRet = tmpList.ToArray
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportsSecurityNodeList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportsSecurityNodeList")
            End Try

            Return bRet
        End Function

        Public Shared Function GetPassportsByEmail(ByRef oState As roPassportState, ByVal sEmail As String) As roPassportTicket()
            Dim bRet As roPassportTicket() = Nothing

            Dim tmpList As New Generic.List(Of roPassportTicket)
            Dim strSQL As String
            Dim tb As DataTable

            Try

                strSQL = "@SELECT# FieldName from sysroUserFields where Alias = 'sysroEmail'"
                Dim strFieldName As String = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL))

                strSQL = $"@SELECT# srp.ID 
                            FROM sysropassports srp 
                            WHERE IDEmployee is null AND Email = '{sEmail.Trim}'
                           UNION
                           @SELECT# ID from sysroPassports where IDEmployee in (@SELECT# distinct IDEmployee from EmployeeUserFieldValues where FieldName = '{strFieldName}' and convert(nvarchar(max),Value) = '{sEmail}')
                          "

                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Dim tmpSecUser As roPassportTicket = roPassportManager.GetPassportTicket(oRow("ID"))
                        If tmpSecUser IsNot Nothing Then tmpList.Add(tmpSecUser)
                    Next
                End If

                bRet = tmpList.ToArray
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportsByEmail")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roPassportManager::GetPassportsByEmail")
            End Try

            Return bRet
        End Function

        Public Shared Function CopySupervisorProperties(ByVal iPassportID As Integer, ByVal iDestinationPassportIDs As Integer(), ByVal copyRestrictions As Boolean, ByVal copyCostCenters As Boolean, ByVal copyBusinessGroups As Boolean, ByVal copyCategories As Boolean, ByVal copyGroups As Boolean, ByVal copyRoles As Boolean, ByRef oState As roPassportState) As Boolean
            Dim bRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim strSQL As String = String.Empty

                For Each oDestintationPassportID As Integer In iDestinationPassportIDs
                    If bRet AndAlso copyRestrictions Then
                        strSQL = "@DELETE# sysroSecurityParameters WHERE IDPassport = (@SELECT# IDParentPassport FROM sysroPassports WHERE ID = " & oDestintationPassportID & ")"

                        bRet = ExecuteSql(strSQL)
                        If bRet Then
                            strSQL = "@INSERT# INTO sysroSecurityParameters ([IDPassport],[PasswordSecurityLevel],[PreviousPasswordsStored],[DaysPasswordExpired],[AccessAttempsTemporaryBlock],[AccessAttempsPermanentBlock],[OnlyAllowedAdress]" &
                                        " ,[OnlySameVersionApp],[RequestValidationCode],[SaveAuthorizedPointDays],[DifferentAccessPointExceeded],[BlockAccessVTPortal],[BlockAccessVTDesktop],[EnabledAccessSupportRobotics],[BlockAccessVTVisits],[CalendarLock])" &
                                 " @SELECT# (@SELECT# IDParentPassport FROM sysroPassports WHERE ID = " & oDestintationPassportID & "),[PasswordSecurityLevel],[PreviousPasswordsStored],[DaysPasswordExpired],[AccessAttempsTemporaryBlock],[AccessAttempsPermanentBlock],[OnlyAllowedAdress], " &
                                 " [OnlySameVersionApp],[RequestValidationCode],[SaveAuthorizedPointDays],[DifferentAccessPointExceeded],[BlockAccessVTPortal],[BlockAccessVTDesktop],[EnabledAccessSupportRobotics],[BlockAccessVTVisits],[CalendarLock] " &
                                 " FROM sysroSecurityParameters WHERE IDPassport = (@SELECT# IDParentPassport FROM sysroPassports WHERE ID = " & iPassportID & ")"
                            bRet = ExecuteSql(strSQL)
                        End If
                    End If

                    'Arbol de grupos
                    If bRet AndAlso copyGroups Then
                        strSQL = "@DELETE# sysroPassports_Groups WHERE IDPassport = " & oDestintationPassportID

                        bRet = ExecuteSql(strSQL)
                        If bRet Then
                            strSQL = "@INSERT# INTO sysroPassports_Groups ([IDPassport],[IDGroup])" &
                                 " @SELECT# " & oDestintationPassportID & ", [IDGroup] " &
                                 " FROM sysroPassports_Groups WHERE IDPassport = " & iPassportID
                            bRet = ExecuteSql(strSQL)
                        End If

                        If bRet Then
                            strSQL = "@DELETE# sysroPassports_Employees WHERE IDPassport = " & oDestintationPassportID

                            bRet = ExecuteSql(strSQL)
                            If bRet Then
                                strSQL = "@INSERT# INTO sysroPassports_Employees ([IDPassport],[IDEmployee],[Permission])" &
                                     " @SELECT# " & oDestintationPassportID & ", [IDEmployee], [Permission] " &
                                     " FROM sysroPassports_Employees WHERE IDPassport = " & iPassportID
                                bRet = ExecuteSql(strSQL)
                            End If
                        End If
                    End If

                    'Categorias de solicitud
                    If bRet AndAlso copyCategories Then
                        strSQL = "@DELETE# sysroPassports_Categories WHERE IDPassport = " & oDestintationPassportID

                        bRet = ExecuteSql(strSQL)
                        If bRet Then
                            strSQL = "@INSERT# INTO sysroPassports_Categories ([IDPassport],[IDCategory],[LevelOfAuthority],[ShowFromLevel])" &
                                 " @SELECT# " & oDestintationPassportID & ",[IDCategory],[LevelOfAuthority],[ShowFromLevel] " &
                                 " FROM sysroPassports_Categories WHERE IDPassport = " & iPassportID
                            bRet = ExecuteSql(strSQL)
                        End If
                    End If

                    'Roles
                    If bRet AndAlso copyRoles Then

                        strSQL = "@UPDATE# sysroPassports SET IDGroupFeature = (" &
                                 " @SELECT# IDGroupFeature FROM sysroPassports WHERE ID = " & iPassportID & ")" &
                                 " WHERE ID = " & oDestintationPassportID
                        bRet = ExecuteSql(strSQL)
                    End If

                    If Not bRet Then Exit For
                Next
            Catch ex As Data.Common.DbException
                bRet = False
                oState.UpdateStateInfo(ex, "roPassportManager::CopySupervisorProperties")
            Catch ex As Exception
                bRet = False
                oState.UpdateStateInfo(ex, "roPassportManager::CopySupervisorProperties")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bRet)
            End Try

            Return bRet
        End Function

        Public Shared Function GetPassportIDBySecurityTokens(ByRef iPassportID As Integer, ByVal authToken As String, ByVal securityToken As String, ByRef securityContext As String, ByRef timeZone As String) As roPassportTicket
            Dim bRet As roPassportTicket = Nothing

            Dim strSQL As String = "@SELECT# * FROM sysroPassports_Data WITH (NOLOCK) WHERE AuthToken = '" & authToken & "'" & " and securityToken = '" & securityToken & "'"

            Dim dt As DataTable = AccessHelper.CreateDataTable(strSQL)

            If dt IsNot Nothing AndAlso dt.Rows.Count = 1 Then
                iPassportID = VTBase.roTypes.Any2Integer(dt.Rows(0)("IDPassport"))
                timeZone = VTBase.roTypes.Any2String(dt.Rows(0)("TimeZone"))

                If roTypes.Any2String(securityToken) = roTypes.Any2String(dt.Rows(0)("SecurityToken")) Then
                    securityContext = VTBase.roTypes.Any2String(dt.Rows(0)("SessionContext"))

                    bRet = roPassportManager.GetPassportTicket(iPassportID, LoadType.Passport)

                    strSQL = "@UPDATE# sysroPassports_Sessions Set InvalidationCode = 0, UpdateDate = GETDATE() WHERE DataId = '" & dt.Rows(0)("ID") & "'"
                    AccessHelper.ExecuteSql(strSQL)
                End If
            End If

            Return bRet
        End Function

        Public Shared Function GetPassportByCredential(ByVal credential As String, ByVal method As AuthenticationMethod, ByVal version As String) As roPassport
            Dim oret As roPassport = Nothing

            Dim strSQL As String = "@SELECT# ID " &
                                    " FROM sysroPassports p " &
                                 " LEFT JOIN sysroPassports_AuthenticationMethods a ON p.ID = a.IDPassport  " &
                                " WHERE a.Method = @method AND " &
                                " a.version = @version AND " &
                                     " a.Credential = @credential AND " &
                                        " a.Enabled = 1 AND " &
                                            " (p.IDEmployee IS NULL OR " &
                                                " ISNULL((@SELECT# COUNT(*) FROM EmployeeContracts  " &
                                      " WHERE EmployeeContracts.IDEmployee = p.IDEmployee AND " &
                                    " EmployeeContracts.BeginDate <= GETDATE() AND EmployeeContracts.EndDate >= GETDATE()), 0) > 0) "
            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@credential", DbType.String, 255).Value = credential
            AccessHelper.AddParameter(cmd, "@method", DbType.Int32).Value = method
            AccessHelper.AddParameter(cmd, "@version", DbType.String, 50).Value = version
            Dim ret As Object = cmd.ExecuteScalar()
            If Not IsDBNull(ret) AndAlso ret IsNot Nothing Then
                oret = roPassportManager.GetPassport(ret, LoadType.Passport)
                Return oret
            Else
                Return oret
            End If
        End Function

        Public Shared Function ValidateCredentials(ByVal method As AuthenticationMethod, ByVal credential As String, ByRef password As String, ByVal hashPassword As Boolean, ByVal version As String, ByVal isSSOLogin As Boolean, ByRef oState As roSecurityState) As roPassportTicket

            Dim oPassport As roPassportTicket = Nothing

            Try

                ' Miramos si debo autenticar contra dominio
                Dim defaultDomain As String = String.Empty
                Dim strSQL As String
                If method = AuthenticationMethod.Password OrElse method = AuthenticationMethod.IntegratedSecurity Then
                    If credential.StartsWith(".\") Then
                        ' Si se informó ".\", entro sin dominio aunque haya por defecto
                        credential = credential.Replace(".\", "")
                    ElseIf Not credential.Contains("\") Then
                        ' Si no se informó dominio, y hay uno por defecto, aplico el por defecto
                        strSQL = "@SELECT# isnull(Value,'') from sysroLiveAdvancedParameters where ParameterName = 'VTLive.AD.DefaultDomain'"
                        defaultDomain = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQL))
                        If defaultDomain.Length > 0 Then
                            credential = defaultDomain & "\" & credential
                        End If
                    End If
                End If

                If method = AuthenticationMethod.Password AndAlso credential.Contains("\") Then
                    Dim bRet As Boolean
                    Dim strDomain As String = credential.Substring(0, credential.IndexOf("\")).Trim()
                    Dim strUser As String = credential.Substring(credential.IndexOf("\")).Trim()
                    If strUser.Length > 0 Then strUser = strUser.Substring(1)

                    If Not isSSOLogin Then
                        Dim oAuthenticateAD As New AuthenticateAD()
                        bRet = oAuthenticateAD.AuthenticateByActiveDirectory(oState, strDomain, strUser, password)
                    Else
                        bRet = True
                    End If

                    If bRet Then
                        'Si la autenticacion real se ha heho bajo AD, en VTLive se hace con el pass igual al nombre del usuario porque el pass de la BBDD es irrelevante
                        'solo se usa a efectos de configuracion de Web services
                        password = credential.ToLower()
                        hashPassword = True

                        oPassport = AuthHelper.Authenticate(method, credential, password, hashPassword, version)
                        If oPassport Is Nothing Then
                            oState.Result = SecurityResultEnum.PassportAuthenticationIncorrect
                        End If
                    Else
                        oState.Result = SecurityResultEnum.PassportAuthenticationIncorrect
                    End If
                Else
                    oPassport = AuthHelper.Authenticate(method, credential, password, hashPassword, version)
                    If oPassport Is Nothing Then
                        oState.Result = SecurityResultEnum.PassportAuthenticationIncorrect
                    End If
                End If
            Catch ex As System.Data.Common.DbException
                oState.UpdateStateInfo(ex, "wscSecurity::ValidateCredentials")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wscSecurity::ValidateCredentials")
            End Try

            Return oPassport
        End Function

        Public Shared Function CheckIfPassportHasPermissionsOverVTPortal(ByVal oPassportTicket As roPassportTicket, ByVal bisAccessingFromApp As Boolean, ByRef oState As roSecurityState) As Boolean
            Dim bRet As Boolean = True

            Dim bAccessAsEmployee As Boolean = (oState.AppType = roAppType.VTPortal)
            Try
                Dim strSQL As String = "@SELECT# isnull(Value,'0') from sysroLiveAdvancedParameters where ParameterName = 'VTLive.IPRestriction.OnlyPunches'"
                Dim accessRestrictionEnabled As Boolean = (Not roTypes.Any2Boolean(DataLayer.AccessHelper.ExecuteScalar(strSQL)))

                Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(oPassportTicket.ID)

                '0.Comprobar si la cuenta esta bloqueada
                If oPassportTicket.IsBloquedAccessApp Then
                    oState.Result = SecurityResultEnum.BloquedAccessApp
                    bRet = False
                Else
                    ' 1.Bloqueo temporal. Comprobar si se han excedido el nº de errores y han pasado menos de 10 minutos desde el ultimo acceso erroneo
                    If oPassportTicket.IsTemporayBloqued Then
                        oState.Result = SecurityResultEnum.TemporayBloqued
                        bRet = False
                    Else

                        ' 3. Validar aplicaciones permitidas
                        If Not roPassportManager.IsValidApp(oPassportTicket, oState, bisAccessingFromApp) Then
                            oState.Result = SecurityResultEnum.InvalidApp
                            bRet = False
                        Else
                            ' 4. Validar bloqueos de acceso generales y del grupo
                            If AuthHelper.IsGeneralBlockAccess(oPassportTicket.ID, oPassportTicket.IsSupervisor, oPassportTicket.IDGroupFeature, oState) Then
                                oState.Result = SecurityResultEnum.GeneralBlockAccess
                                bRet = False
                            Else
                                Dim bCheckSupervisorIsActive As Boolean = True

                                'Si accedo al portal del empleado  en seguridad > = V2 y soy empleado no compruebo el estado del supervisor
                                If bAccessAsEmployee AndAlso oPassportTicket.IDEmployee.HasValue AndAlso oPassportTicket.IDEmployee > 0 Then
                                    bCheckSupervisorIsActive = False
                                End If

                                'Si accedo a live en seguridad >=V2 compruebo si soy supervisor
                                If Not bAccessAsEmployee Then
                                    bCheckSupervisorIsActive = oPassportTicket.IsSupervisor
                                End If

                                '5. Validamos si el passport esta activo
                                If bCheckSupervisorIsActive AndAlso Not oPassportTicket.IsActivePassport Then
                                    oState.Result = SecurityResultEnum.PassportInactive
                                    bRet = False
                                Else
                                    If (accessRestrictionEnabled) Then
                                        ' 7. Validacion IP del cliente si no es APP
                                        Dim strClientLocation As String = oState.ClientAddress.Split("#")(0)
                                        If Not bisAccessingFromApp AndAlso Not AuthHelper.IsValidClientLocation(oPassportTicket.ID, strClientLocation) Then
                                            oState.Result = SecurityResultEnum.InvalidClientLocation
                                            bRet = False
                                        End If
                                    End If
                                End If
                            End If
                        End If
                    End If
                End If
            Catch ex As System.Data.Common.DbException
                oState.UpdateStateInfo(ex, "wscSecurity::CheckIfPassportHasPermissionsOverVTPortal")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "wscSecurity::CheckIfPassportHasPermissionsOverVTPortal")
            End Try
            Return bRet
        End Function

        Public Shared Function IsValidApp(ByVal oPassport As roPassportTicket, ByVal oState As roBaseState, Optional ByVal bisAccessingFromApp As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            If Not bisAccessingFromApp Then
                Select Case oState.AppType
                    Case roAppType.VTLive
                        If oPassport.EnabledVTDesktop AndAlso oPassport.IsSupervisor Then bolRet = True
                    Case roAppType.VTPortal
                        If oPassport.EnabledVTPortal Then bolRet = True
                    Case roAppType.VTVisits
                        If oPassport.EnabledVTVisits Then bolRet = True
                    Case Else
                        bolRet = True
                End Select
            Else
                Select Case oState.AppType
                    Case roAppType.VTPortal
                        If oPassport.EnabledVTPortalApp Then bolRet = True
                    Case roAppType.VTVisits
                        If oPassport.EnabledVTVisitsApp Then bolRet = True
                    Case Else
                        bolRet = True
                End Select
            End If

            Return bolRet
        End Function

        ''' <summary>
        ''' Returns whether specified credentials already exists with authentication method
        ''' for other passports than the one specified.
        ''' </summary>
        ''' <param name="credential">The credential to look for.</param>
        ''' <param name="method">The authentication method for which to look at credentials.</param>
        ''' <param name="version">The version for wich to look at credentials.</param>
        ''' <param name="idPassport">Credentials of this passport will be excluded from the search.</param>
        Public Shared Function CredentialExists(ByVal credential As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idPassport As Nullable(Of Integer)) As Boolean
            Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_CredentialExists")
            Command.CommandType = CommandType.StoredProcedure
            AccessHelper.AddParameter(Command, "@credential", DbType.String, 255).Value = credential
            AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = method
            AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
            AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32)
            If idPassport.HasValue Then
                Command.Parameters("@idPassport").Value = idPassport.Value
            End If
            Dim Result As Object = Command.ExecuteScalar()
            If Result Is DBNull.Value OrElse CInt(Result) = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        Public Shared Function HasReadModeEnabled(ByVal idEmployee As Integer) As Boolean
            Dim bRet As Boolean = False

            If idEmployee > 0 Then

                Dim strSQL = "@SELECT# HasFutureContract, sp.LoginWithoutContract, case when svw.CurrentEmployee is null then 0 else svw.CurrentEmployee end as CurrentEmployee FROM " &
                                                                  "(@SELECT# CASE WHEN max(enddate) > getdate() THEN 1 ELSE 0 END AS HasFutureContract FROM  " &
                                                                  "employeecontracts ec WHERE ec.IDEmployee=" & idEmployee & "  ) a INNER JOIN sysropassports sp ON sp.IDEmployee =" & idEmployee & "  " &
                                                                  " left join sysrovwCurrentEmployeeGroups svw on svw.IDEmployee = " & idEmployee & " "

                Dim resultTable As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                If resultTable IsNot Nothing AndAlso resultTable.Rows.Count > 0 Then

                    For Each oRow As DataRow In resultTable.Rows
                        If roTypes.Any2Integer(oRow("CurrentEmployee")) = 0 Then
                            'ESTANDAR
                            If roTypes.Any2Integer(oRow("HasFutureContract")) = 1 AndAlso roTypes.Any2Integer(oRow("LoginWithoutContract")) = 1 Then
                                bRet = True
                            Else 'PERSONALIZACION VORAMAR
                                Dim strSQLField = "@select# value from EmployeeUserFieldValues where fieldname='Acceso Portal' and IDEmployee= '" & idEmployee & "'"
                                Dim field = roTypes.Any2String(DataLayer.AccessHelper.ExecuteScalar(strSQLField))

                                If field.ToString.ToLower = "siempre" Then
                                    bRet = True
                                End If
                            End If
                        End If
                    Next
                End If

            End If

            Return bRet

        End Function

#End Region

#Region "Private helpers"

        ''' <summary>
        ''' Returns whether specified password already exists with authentication method
        ''' for other passports than the one specified.
        ''' </summary>
        ''' <param name="Plate">The Plate to look for.</param>
        ''' <param name="method">The authentication method for which to look at credentials.</param>
        ''' <param name="version">The version for wich to look at credentials.</param>
        ''' <param name="idPassport">Credentials of this passport will be excluded from the search.</param>
        Public Shared Function PlateExists(ByVal Plate As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idPassport As Integer) As Boolean
            Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_PlateExists")
            Command.CommandType = CommandType.StoredProcedure
            AccessHelper.AddParameter(Command, "@credential", DbType.String, 255).Value = Plate
            AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = method
            AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
            AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32).Value = idPassport
            Dim Result As Object = Command.ExecuteScalar()
            If Result Is DBNull.Value OrElse CInt(Result) = 0 Then
                Return False
            Else
                Return True
            End If
        End Function

        ''' <summary>
        ''' Returns whether specified password already exists with authentication method
        ''' for other passports than the one specified.
        ''' </summary>
        ''' <param name="password">The password to look for.</param>
        ''' <param name="method">The authentication method for which to look at credentials.</param>
        ''' <param name="version">The version for wich to look at credentials.</param>
        ''' <param name="idPassport">Credentials of this passport will be excluded from the search.</param>
        Public Shared Function PasswordExists(ByVal password As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idPassport As Nullable(Of Integer), Optional ByVal hashPassword As Boolean = False) As Boolean

            ' Hash password.
            Dim Pass As String = password
            'If hashPassword Then
            Pass = CryptographyHelper.EncryptWithMD5(password)
            'End If

            Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_PasswordExists")
            Command.CommandType = CommandType.StoredProcedure
            AccessHelper.AddParameter(Command, "@password", DbType.String, 1000).Value = Pass
            AccessHelper.AddParameter(Command, "@method", DbType.Int32).Value = method
            AccessHelper.AddParameter(Command, "@version", DbType.String, 50).Value = version
            AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32)
            If idPassport.HasValue Then
                Command.Parameters("@idPassport").Value = idPassport.Value
            End If
            Dim Result As Object = Command.ExecuteScalar()
            If Result Is DBNull.Value OrElse CInt(Result) = 0 Then
                Return False
            Else
                Return True
            End If

        End Function

        ''' <summary>
        ''' Save specified password history
        ''' for other passports than the one specified.
        ''' </summary>
        ''' <param name="idPassport">Credentials of this passport will be excluded from the search.</param>
        ''' <param name="Password">Credentials of this passport will be excluded from the search.</param>
        ''' <param name="LastUpdate">Credentials of this passport will be excluded from the search.</param>
        Public Shared Function SavePasswordHistory(ByVal idPassport As Integer, ByVal Password As String, ByVal LastUpdate As DateTime) As Boolean
            Dim bolRet As Boolean = False
            Dim PreviousPasswordsStored As Integer = 0

            Dim tb1 As New DataTable("sysroSecurityParameters")
            Dim strSQL As String = "@SELECT# PreviousPasswordsStored FROM sysroSecurityParameters " &
                                   "WHERE IDPassport = 0 "
            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)

            Dim ret As Object = cmd.ExecuteScalar()
            If Not IsDBNull(ret) Then
                PreviousPasswordsStored = Val(ret)
            End If

            ' Guardamos la contraseña anterior
            Dim tb As New DataTable("sysroPassports_PasswordHistory")
            strSQL = "@SELECT# * FROM sysroPassports_PasswordHistory " &
                     "WHERE IDPassport = @IDPassport "
            cmd = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@IDPassport", DbType.String)

            cmd.Parameters("@IDPassport").Value = idPassport

            Dim ad As DbDataAdapter = AccessHelper.CreateDataAdapter(cmd, True)
            ad.Fill(tb)

            Dim oRow As DataRow
            oRow = tb.NewRow
            'oRow("ID") = ID
            oRow("IDPassport") = idPassport
            oRow("Password") = Password
            oRow("TimeStamp") = LastUpdate

            tb.Rows.Add(oRow)

            ad.Update(tb)

            ' Borramos las contraseñas antiguas
            strSQL = "@DELETE# FROM sysroPassports_PasswordHistory WHERE " &
                        "ID IN ( @SELECT# ID FROM (@SELECT# row_number() OVER(order by x.TimeStamp desc) AS IDRow  , x.* " &
                            " FROM (@SELECT# ID, Timestamp  FROM sysroPassports_PasswordHistory WHERE idpassport = @idpassport ) x) AS y " &
                                    "WHERE IDRow > @PreviousPasswordsStored )"

            cmd = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@PreviousPasswordsStored", DbType.Int32)
            AccessHelper.AddParameter(cmd, "@idpassport", DbType.Int32)
            cmd.Parameters("@PreviousPasswordsStored").Value = PreviousPasswordsStored
            cmd.Parameters("@idpassport").Value = idPassport
            cmd.ExecuteNonQuery()

            bolRet = True

            Return bolRet
        End Function

        Public Shared Function MaxCredentialValue(ByVal method As Integer, ByVal version As String) As Long

            Dim strSQL As String = "@SELECT# MAX(CONVERT(float, Credential)) FROM sysroPassports_AuthenticationMethods " &
                                   "WHERE Method = @Method AND " &
                                         "ISNUMERIC(Credential) = 1"
            If version <> Nothing Then
                strSQL &= " AND Version = @Version"
            End If
            Dim cmd As DbCommand = AccessHelper.CreateCommand(strSQL)
            AccessHelper.AddParameter(cmd, "@method", DbType.Int32)
            If version <> Nothing Then AccessHelper.AddParameter(cmd, "@Version", DbType.String, 50)

            cmd.Parameters("@Method").Value = method
            If version <> Nothing Then cmd.Parameters("@Version").Value = version

            Dim ret As Object = cmd.ExecuteScalar()
            If Not IsDBNull(ret) Then
                Return Val(ret)
            Else
                Return 0
            End If

        End Function

#End Region

    End Class

End Namespace