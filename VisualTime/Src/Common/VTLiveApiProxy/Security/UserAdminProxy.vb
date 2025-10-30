Imports Robotics.Base
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.UsersAdmin
Imports Robotics.UsersAdmin.Business
Imports Robotics.VTBase

Public Class UserAdminProxy
    Implements IUserAdminSvc

    Public Function KeepAlive() As Boolean Implements IUserAdminSvc.KeepAlive
        Return True
    End Function

    ''' <summary>
    ''' obtiene una cadena de conexión contra la base de datos.
    ''' </summary>
    Public Function GetConnectionString(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IUserAdminSvc.GetConnectionString

        Return UserAdminMethods.GetConnectionString(idPassport, oState)
    End Function


    Public Function GetUserAdmins(ByVal oState As roWsState) As roGenericVtResponse(Of wscUserAdminList) Implements IUserAdminSvc.GetUserAdmins

        Return UserAdminMethods.GetUserAdmins(oState)
    End Function

    Public Function GetUserAdmin(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of wscUserAdmin) Implements IUserAdminSvc.GetUserAdmin

        Return UserAdminMethods.GetUserAdmin(intIDPassport, oState)
    End Function

    Public Function NewUserAdmin(ByVal oState As roWsState) As roGenericVtResponse(Of wscUserAdmin) Implements IUserAdminSvc.NewUserAdmin

        Return UserAdminMethods.NewUserAdmin(oState)
    End Function

    Public Function UpdateUserAdmin(ByVal oUserAdmin As wscUserAdmin, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.UpdateUserAdmin

        Return UserAdminMethods.UpdateUserAdmin(oUserAdmin, oState)

    End Function

    Public Function DeleteUserAdmin(ByVal oUserAdmin As wscUserAdmin, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.DeleteUserAdmin

        Return UserAdminMethods.DeleteUserAdmin(oUserAdmin, oState)
    End Function

    ''' <summary>
    ''' Obtiene el passporte a partir del código de pasaporte, de empleado o de usuario.
    ''' </summary>
    ''' <param name="intID">Código utilizado para recuperar el passport</param>
    ''' <param name="loadType">Uno de los valores de <b>Passport.LoadType</b> que especifica el tipo de código pasado (código de passport, código de empleado o código de usuario</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución</param>
    ''' <returns>Objeto pasaporte o nothing si no existe</returns>
    ''' <remarks></remarks>

    Public Function GetPassport(ByVal intID As Integer, ByVal loadType As LoadType, ByVal DecryptPwds As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roPassport) Implements IUserAdminSvc.GetPassport

        Return UserAdminMethods.GetPassport(intID, loadType, DecryptPwds, oState)
    End Function

    Public Function GetPassportsByParent(ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roPassport)) Implements IUserAdminSvc.GetPassportsByParent

        Return UserAdminMethods.GetPassportsByParent(idParentPassport, groupType, oState)
    End Function

    Public Function GetPassportsByParentLite(ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserAdminSvc.GetPassportsByParentLite

        Return UserAdminMethods.GetPassportsByParentLite(idParentPassport, groupType, oState)
    End Function

    Public Function GetPassports(ByVal loadType As LoadType, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roPassport)) Implements IUserAdminSvc.GetPassports

        Return UserAdminMethods.GetPassports(loadType, oState)
    End Function

    Public Function GetLitePassports(ByVal loadType As LoadType, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserAdminSvc.GetLitePassports

        Return UserAdminMethods.GetLitePassports(loadType, oState)
    End Function

    Public Function GetAuditPassports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserAdminSvc.GetAuditPassports

        Return UserAdminMethods.GetAuditPassports(oState)
    End Function

    Public Function GetSupervisorPassports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet) Implements IUserAdminSvc.GetSupervisorPassports

        Return UserAdminMethods.GetSupervisorPassports(oState)
    End Function

    ''' <summary>
    ''' Graba el pasaporte indicado.
    ''' </summary>
    ''' <param name="oPassport">Pasaporte a grabar.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns><b>True</b> si se ha podido grabar; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>

    Public Function SavePassport(ByVal oDTOPassport As roPassport, ByVal EncryptPwds As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of (roPassport, Boolean)) Implements IUserAdminSvc.SavePassport
        Return UserAdminMethods.SavePassport(oDTOPassport, EncryptPwds, oState)
    End Function

    Public Function IsSupervisorPassport(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.IsSupervisorPassport
        Return UserAdminMethods.IsSupervisorPassport(intIDPassport, oState)
    End Function

    ''' <summary>
    ''' Elimina el passaporte indicado
    ''' </summary>
    ''' <param name="oPassport"></param>
    ''' <param name="updateDriversTasks"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeletePassport(ByVal oDTOPassport As roPassport, ByVal updateDriversTasks As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.DeletePassport
        Return UserAdminMethods.DeletePassport(oDTOPassport, updateDriversTasks, oState)
    End Function


    ''' <summary>
    ''' Elimina el passaporte con el ID indicado
    ''' </summary>
    ''' <param name="IDPassport"></param>
    ''' <param name="updateDriversTasks"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeletePassportByID(ByVal IDPassport As Integer, ByVal updateDriversTasks As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.DeletePassportByID
        Return UserAdminMethods.DeletePassportByID(IDPassport, updateDriversTasks, oState)
    End Function


    ''' <summary>
    ''' Desasigna el empleado del passport indicado
    ''' </summary>
    ''' <param name="oPassport"></param>
    ''' <param name="updateDriversTasks"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function DeleteEmployeeFromPassport(ByVal oDTOPassport As roPassport, ByVal updateDriversTasks As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.DeleteEmployeeFromPassport
        Return UserAdminMethods.DeleteEmployeeFromPassport(oDTOPassport, updateDriversTasks, oState)
    End Function



    ''' <summary>
    ''' Verifica si ya existe una credencial de una método de autentificación especificado.
    ''' </summary>
    ''' <param name="credential">Valor de la credencial del método de autentificación.</param>
    ''' <param name="method">Uno de los valores de <b>AuthenticationMethod</b> que especifica el tipo del método de autentificación.</param>
    ''' <param name="version">Versión del método de autentificación.</param>
    ''' <param name="idpassport">Código para informar del pasaporte a excluir de la verificación. Si se especifica nothing se verifican todos los pasaportes.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución</param>
    ''' <returns><b>True</b> si existe la credencial; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>
    Public Function CredentialExists(ByVal credential As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Nullable(Of Integer), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.CredentialExists
        Return UserAdminMethods.CredentialExists(credential, method, version, idpassport, oState)
    End Function


    ''' <summary>
    ''' Verifica si ya existe la contraseña especificada en los métodos de autentificación.
    ''' </summary>
    ''' <param name="password">Contraseña a buscar en los métodos de autentificación.</param>
    ''' <param name="method">Uno de los valores de <b>AuthenticationMethod</b> que especifica el tipo del método de autentificación.</param>
    ''' <param name="version">Versión del método de autentificación.</param>
    ''' <param name="idpassport">Código para informar del pasaporte a excluir de la verificación. Si se especifica nothing se verifican todos los pasaportes.</param>
    ''' <param name="hashPassword">Indica si se tiene que aplicar encriptación a la contraseña.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns><b>True</b> si existe la contraseña; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>

    Public Function PasswordExists(ByVal password As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Nullable(Of Integer), ByVal hashPassword As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.PasswordExists
        Return UserAdminMethods.PasswordExists(password, method, version, idpassport, hashPassword, oState)
    End Function


    ''' <summary>
    ''' Verifica si ya existe la matrícula especificada en los métodos de autentificación.
    ''' </summary>
    ''' <param name="plate">Matrícula a buscar en los métodos de autentificación.</param>
    ''' <param name="method">Uno de los valores de <b>AuthenticationMethod</b> que especifica el tipo del método de autentificación.</param>
    ''' <param name="version">Versión del método de autentificación.</param>
    ''' <param name="idpassport">Código para informar del pasaporte a excluir de la verificación. Si se especifica nothing se verifican todos los pasaportes.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns><b>True</b> si existe la contraseña; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>

    Public Function PlateExists(ByVal Plate As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.PlateExists

        Return UserAdminMethods.PlateExists(Plate, method, version, idpassport, oState)
    End Function

    ''' <summary>
    ''' Obtiene el máximo valor de la credencial (en formato numérico) del método especificado. Si no se informa versión, obtiene el máximo de todas las versiones del mismo método.
    ''' </summary>        
    ''' <param name="method">Uno de los valores de <b>AuthenticationMethod</b> que especifica el tipo del método de autentificación.</param>
    ''' <param name="version">Versión del método de autentificación.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns>El valor máximo del campo 'credential' del método indicado.</returns>
    ''' <remarks></remarks>
    Public Function MaxCredentialValue(ByVal method As AuthenticationMethod, ByVal version As String, ByVal oState As roWsState) As roGenericVtResponse(Of Long) Implements IUserAdminSvc.MaxCredentialValue
        Return UserAdminMethods.MaxCredentialValue(method, version, oState)
    End Function

#Region "Features"

    ''' <summary>
    ''' Obtiene la lista de funcionalidades de un passport. Si idFeature es nothing, se devuelven las funcionalidades de primer nivel, 
    ''' en caso contrario, se devuelven las funcionalidades asociadas (solo primer nivel)
    ''' </summary>
    ''' <param name="type"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFeaturesList(ByVal type As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Feature)) Implements IUserAdminSvc.GetFeaturesList
        Return UserAdminMethods.GetFeaturesList(type, oState)
    End Function


    ''' <summary>
    ''' Obtiene la lista de funcionalidades de un passport. Si idFeature es nothing, se devuelven las funcionalidades de primer nivel, 
    ''' en caso contrario, se devuelven las funcionalidades asociadas (solo primer nivel)
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetFeaturesFromPassport(ByVal idPassport As Integer, ByVal idFeature As Nullable(Of Integer), ByVal type As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Feature)) Implements IUserAdminSvc.GetFeaturesFromPassport
        Return UserAdminMethods.GetFeaturesFromPassport(idPassport, idFeature, type, oState)
    End Function

    ''' <summary>
    ''' Obtiene la lista de todas las funcionalidades de un passport
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetFeaturesFromPassportAll(ByVal idPassport As Integer, ByVal type As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Feature)) Implements IUserAdminSvc.GetFeaturesFromPassportAll
        Return UserAdminMethods.GetFeaturesFromPassportAll(idPassport, type, oState)
    End Function


    ''' <summary>
    ''' Obtiene la lista de permisos posibles a asignar a una funcionalidad para un passport en concreto.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetFeaturePermissions(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Permission)) Implements IUserAdminSvc.GetFeaturePermissions
        Return UserAdminMethods.GetFeaturePermissions(idPassport, idFeature, FeatureType, oState)
    End Function


    ''' <summary>
    ''' Establece el permiso indicado a una funcionalidad.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="_permission"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetFeaturePermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal _permission As Permission, ByVal FeaturesChanged As Generic.List(Of Feature), ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Generic.List(Of Feature))) Implements IUserAdminSvc.SetFeaturePermission

        Return UserAdminMethods.SetFeaturePermission(idPassport, idFeature, FeatureType, _permission, FeaturesChanged, oState)
    End Function


    ''' <summary>
    ''' Reestablece el permiso por defecto a una cierta funcionalidad.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SetDefaultFeaturePermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal FeaturesChanged As Generic.List(Of Feature), ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Generic.List(Of Feature))) Implements IUserAdminSvc.SetDefaultFeaturePermission

        Return UserAdminMethods.SetDefaultFeaturePermission(idPassport, idFeature, FeatureType, FeaturesChanged, oState)
    End Function

#End Region

#Region "EmployeeGroup"

    ''' <summary>
    ''' Obtiene la lista de grupos de empleados de un passport y una funcionalidad. Si idGroup es nothing, se devuelven los grupos de empleados de primer nivel, 
    ''' en caso contrario, se devuelven los grupos de empleados asociados (solo primer nivel)
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function GetEmployeeGroupsFromPassportFeature(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal idGroup As Nullable(Of Integer), ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of EmployeeGroup)) Implements IUserAdminSvc.GetEmployeeGroupsFromPassportFeature

        Return UserAdminMethods.GetEmployeeGroupsFromPassportFeature(idPassport, idFeature, idGroup, oState)
    End Function


    ''' <summary>
    ''' Obtiene la lista de todos los grupos de empleados de un passport y funcionalidad
    ''' </summary>
    ''' <param name="idPassport"></param>        
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetEmployeeGroupsFromPassportFeatureAll(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of EmployeeGroup)) Implements IUserAdminSvc.GetEmployeeGroupsFromPassportFeatureAll

        Return UserAdminMethods.GetEmployeeGroupsFromPassportFeatureAll(idPassport, idFeature, oState)
    End Function


    ''' <summary>
    ''' Establece el permiso indicado a un grupo de empleados.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="_permission"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetEmployeeGroupPermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal idGroup As Integer, ByVal _permission As Permission, ByVal Changed As Generic.List(Of EmployeeGroup), ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Generic.List(Of EmployeeGroup))) Implements IUserAdminSvc.SetEmployeeGroupPermission

        Return UserAdminMethods.SetEmployeeGroupPermission(idPassport, idFeature, idGroup, _permission, Changed, oState)
    End Function


    ''' <summary>
    ''' Reestablece el permiso por defecto a un grupo de empleados.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function SetDefaultEmployeeGroupPermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal idGroup As Integer, ByVal Changed As Generic.List(Of EmployeeGroup), ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Generic.List(Of EmployeeGroup))) Implements IUserAdminSvc.SetDefaultEmployeeGroupPermission

        Return UserAdminMethods.SetDefaultEmployeeGroupPermission(idPassport, idFeature, idGroup, Changed, oState)
    End Function




#End Region

#Region "Employee"

    ''' <summary>
    ''' Obtiene la lista de todos los empleados de un passport y funcionalidad
    ''' </summary>
    ''' <param name="idPassport"></param>        
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetEmployeesFromPassportFeatureAll(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Employee)) Implements IUserAdminSvc.GetEmployeesFromPassportFeatureAll

        Return UserAdminMethods.GetEmployeesFromPassportFeatureAll(idPassport, idFeature, oState)
    End Function




    ''' <summary>
    ''' Establece el permiso indicado a un empleado.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="idEmployee"></param>
    ''' <param name="_permission"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function SetEmployeePermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal idGroup As Integer, ByVal _permission As Permission, ByVal Changed As Employee, ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Employee)) Implements IUserAdminSvc.SetEmployeePermission

        Return UserAdminMethods.SetEmployeePermission(idPassport, idFeature, idGroup, _permission, Changed, oState)
    End Function



    ''' <summary>
    ''' Reestablece el permiso por defecto a un cierto empleado.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function RemoveEmployeePermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal idEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.RemoveEmployeePermission

        Return UserAdminMethods.RemoveEmployeePermission(idPassport, idFeature, idEmployee, oState)
    End Function

    ''' <summary>
    ''' Añade permiso sobre un empleado
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Function AddEmployee(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal idEmployee As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.AddEmployee

        Return UserAdminMethods.AddEmployee(idPassport, idFeature, idEmployee, oState)
    End Function

#End Region

    ''' <summary>
    ''' Obtiene la lista de idiomas definidos en la bd
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Function GetLanguages(ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Language)) Implements IUserAdminSvc.GetLanguages

        Return UserAdminMethods.GetLanguages(oState)
    End Function


    ''' <summary>
    ''' Crea user nuevo en el pasaporte indicado.
    ''' </summary>
    ''' <param name="oPassport">Pasaporte a grabar.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns><b>True</b> si se ha podido crear; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>
    Public Function CreateUserOfPassport(ByVal oDTOPassport As roPassport, ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, roPassport)) Implements IUserAdminSvc.CreateUserOfPassport

        Return UserAdminMethods.CreateUserOfPassport(oDTOPassport, oState)
    End Function


    ''' <summary>
    ''' obtiene una cadena con los business group a los que el idpassport tiene permiso.
    ''' </summary>
    Public Function GetBusinessGroupListByIdPassport(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String) Implements IUserAdminSvc.GetBusinessGroupListByIdPassport

        Return UserAdminMethods.GetBusinessGroupListByIdPassport(idPassport, oState)
    End Function

    ''' <summary>
    ''' guarda la cadena con los business group a los que el idpassport tiene permiso.
    ''' </summary>
    Public Function SetBusinessGroupListByIdPassport(ByVal idPassport As Integer, ByVal strValue As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.SetBusinessGroupListByIdPassport

        Return UserAdminMethods.SetBusinessGroupListByIdPassport(idPassport, strValue, oState)
    End Function

    ''' <summary>
    ''' retorna bool diciendo si el business group está en uso
    ''' </summary>
    Public Function BusinessGroupListInUse(ByVal strBusinessGroup As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.BusinessGroupListInUse

        Return UserAdminMethods.BusinessGroupListInUse(strBusinessGroup, oState)
    End Function


    Public Function SaveParameterInContext(ByVal strParameterName As String, ByVal strParameterValue As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean) Implements IUserAdminSvc.SaveParameterInContext
        Return UserAdminMethods.SaveParameterInContext(strParameterName, strParameterValue, oState)
    End Function

End Class
