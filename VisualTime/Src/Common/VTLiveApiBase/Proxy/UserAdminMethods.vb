Imports System.Data
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.DataLayer
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.UsersAdmin
Imports Robotics.UsersAdmin.Business
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class UserAdminMethods

    ''' <summary>
    ''' obtiene una cadena de conexión contra la base de datos.
    ''' </summary>
    Public Shared Function GetConnectionString(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of String)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)

        Dim strConnection As String = String.Empty
        Dim Conn As System.Data.SqlClient.SqlConnection

        'Try to connect using SQL authentication
        Try
            strConnection = Robotics.DataLayer.AccessHelper.GetConectionString()
            Conn = New System.Data.SqlClient.SqlConnection(strConnection)
            Conn.Open()
        Catch
            strConnection = String.Empty
            Conn = Nothing
        End Try

        oResult.Value = strConnection

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetUserAdmins(ByVal oState As roWsState) As roGenericVtResponse(Of wscUserAdminList)
        Dim bState = New wscState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of wscUserAdminList)
        oResult.Value = New wscUserAdminList(Nothing)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult.Value.StateInfo, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetUserAdmin(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of wscUserAdmin)
        Dim bState = New wscState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of wscUserAdmin)
        oResult.Value = New wscUserAdmin(intIDPassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oResult.Value.StateInfo, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function NewUserAdmin(ByVal oState As roWsState) As roGenericVtResponse(Of wscUserAdmin)
        Dim bState = New wscState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of wscUserAdmin)
        oResult.Value = New wscUserAdmin()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function UpdateUserAdmin(ByVal oUserAdmin As wscUserAdmin, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New wscState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oUserAdmin.Update()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function UpdatePassportNameAndLanguage(ByVal idPassport As Integer, ByVal sNewName As String, ByVal idLang As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roPassportState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oPassportmanager As New roPassportManager(bState)

        oResult.Value = oPassportmanager.UpdatePassportNameAndLanguage(idPassport, sNewName, idLang)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPassportmanager.State, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function DeleteUserAdmin(ByVal oUserAdmin As wscUserAdmin, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New wscState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = oUserAdmin.Delete()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el passporte a partir del código de pasaporte, de empleado o de usuario.
    ''' </summary>
    ''' <param name="intID">Código utilizado para recuperar el passport</param>
    ''' <param name="loadType">Uno de los valores de <b>Passport.LoadType</b> que especifica el tipo de código pasado (código de passport, código de empleado o código de usuario</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución</param>
    ''' <returns>Objeto pasaporte o nothing si no existe</returns>
    ''' <remarks></remarks>

    Public Shared Function GetPassport(ByVal intID As Integer, ByVal loadType As LoadType, ByVal DecryptPwds As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of roPassport)
        Dim oResult As New roGenericVtResponse(Of roPassport)
        Dim bstate As New roPassportState(-1)

        Dim oPassportmanager As New roPassportManager(bstate)
        Dim tmpPAssport As roPassport = oPassportmanager.LoadPassport(intID, loadType)

        If tmpPAssport IsNot Nothing Then
            oResult.Value = tmpPAssport

            Dim newGState As New roWsState
            roWsStateManager.CopyTo(oPassportmanager.State, newGState)
            oResult.Status = newGState
        Else
            oResult.Status = New roWsState With {
                .ReturnCode = Robotics.Base.DTOs.SecurityResultEnum.PassportDoesNotExists,
                .Result = SecurityResultEnum.PassportDoesNotExists
            }
            oResult.Value = Nothing
        End If

        Return oResult
    End Function
    Public Shared Function IsRoboticsUserOrConsultant(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bstate As New roPassportState(-1)

        Dim isConsultant As Boolean = roPassportManager.IsRoboticsUserOrConsultant(intID)

        oResult.Value = isConsultant
        oResult.Status = oState

        Return oResult
    End Function

    Public Shared Function IsConsultant(ByVal intID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bstate As New roPassportState(-1)

        Dim _isConsultant As Boolean = roPassportManager.IsRoboticsUserOrConsultant(intID)

        oResult.Value = _isConsultant
        oResult.Status = oState

        Return oResult
    End Function

    Public Shared Function GetPassportsByParent(ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of roPassport))
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of roPassport))
        oResult.Value = New Generic.List(Of roPassport)

        Dim oManager As New roPassportManager(bState)

        For Each idPassport As Integer In oManager.GetPassportsByParent(idParentPassport, groupType, bState)
            oResult.Value.Add(oManager.LoadPassport(idPassport, LoadType.Passport))
        Next

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetPassportsByParentLite(ByVal idParentPassport As Nullable(Of Integer), ByVal groupType As String, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()


        Dim oManager As New roPassportManager(bState)
        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oRet As DataSet = Nothing
        Dim tb As DataTable = oManager.GetPassportsByParentLite(idParentPassport, groupType, bState)
        If tb IsNot Nothing Then
            oRet = New DataSet
            oRet.Tables.Add(tb)
        End If
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetLitePassports(ByVal loadType As LoadType, ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oRet As DataSet = Nothing
        Dim oManager As New roPassportManager(bState)
        Dim tb As DataTable = oManager.GetLitePassports(loadType, bState)
        If tb IsNot Nothing Then
            oRet = New DataSet
            oRet.Tables.Add(tb)
        End If
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetAuditPassports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oRet As DataSet = Nothing
        Dim oManager As New roPassportManager(bState)
        Dim tb As DataTable = oManager.GetLitePassports(LoadType.Passport, bState)
        If tb IsNot Nothing Then
            oRet = New DataSet
            oRet.Tables.Add(tb)
        End If
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetSupervisorPassports(ByVal oState As roWsState) As roGenericVtResponse(Of DataSet)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of DataSet)
        Dim oRet As DataSet = Nothing
        Dim oManager As New roPassportManager(bState)
        Dim tb As DataTable = oManager.GetSupervisorPassports(bState)
        If tb IsNot Nothing Then
            oRet = New DataSet
            oRet.Tables.Add(tb)
        End If
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Graba el pasaporte indicado.
    ''' </summary>
    ''' <param name="oPassport">Pasaporte a grabar.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns><b>True</b> si se ha podido grabar; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>

    Public Shared Function SavePassport(ByVal oDTOPassport As roPassport, ByVal EncryptPwds As Boolean, ByVal oState As roWsState, Optional ByVal audit As Boolean = False) As roGenericVtResponse(Of (roPassport, Boolean))

        Dim bState As roPassportState = New roPassportState(-1)
        Dim oResult As New roGenericVtResponse(Of (roPassport, Boolean))

        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oPassportmanager As New roPassportManager(bState)
        Dim bSaved As Boolean = oPassportmanager.Save(oDTOPassport, audit)
        oResult.Value = (oDTOPassport, bSaved)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPassportmanager.State, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function IsSupervisorPassport(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roTypes.Any2Boolean(AccessHelper.ExecuteScalar("@SELECT# isnull(IsSupervisor,0) from sysroPassports where ID=" & intIDPassport.ToString))

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Elimina el passaporte indicado
    ''' </summary>
    ''' <param name="oPassport"></param>
    ''' <param name="updateDriversTasks"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DeletePassport(ByVal oDTOPassport As roPassport, ByVal updateDriversTasks As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False
        Dim oPassportmanager As New roPassportManager(bState)

        Try
            bolRet = oPassportmanager.Delete(oDTOPassport, updateDriversTasks)

        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wsUserAdmin::DeletePassport")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wsUserAdmin::DeletePassport")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPassportmanager.State, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Elimina el passaporte con el ID indicado
    ''' </summary>
    ''' <param name="IDPassport"></param>
    ''' <param name="updateDriversTasks"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DeletePassportByID(ByVal IDPassport As Integer, ByVal updateDriversTasks As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oPassportmanager As New roPassportManager(bState)
        oResult.Value = oPassportmanager.Delete(oPassportmanager.LoadPassport(IDPassport, LoadType.Passport), updateDriversTasks)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(oPassportmanager.State, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Desasigna el empleado del passport indicado
    ''' </summary>
    ''' <param name="oPassport"></param>
    ''' <param name="updateDriversTasks"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function DeleteEmployeeFromPassport(ByVal oDTOPassport As roPassport, ByVal updateDriversTasks As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim oPassportmanager As New roPassportManager(bState)
        oResult.Value = oPassportmanager.DeteleteEmployeeFromPassport(oDTOPassport, updateDriversTasks, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
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
    Public Shared Function CredentialExists(ByVal credential As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Nullable(Of Integer), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPassportManager.CredentialExists(credential, method, version, idpassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
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

    Public Shared Function PasswordExists(ByVal password As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Nullable(Of Integer), ByVal hashPassword As Boolean, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)

        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPassportManager.PasswordExists(password, method, version, idpassport, hashPassword)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
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

    Public Shared Function PlateExists(ByVal Plate As String, ByVal method As AuthenticationMethod, ByVal version As String, ByVal idpassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPassportManager.PlateExists(Plate, method, version, idpassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene el máximo valor de la credencial (en formato numérico) del método especificado. Si no se informa versión, obtiene el máximo de todas las versiones del mismo método.
    ''' </summary>
    ''' <param name="method">Uno de los valores de <b>AuthenticationMethod</b> que especifica el tipo del método de autentificación.</param>
    ''' <param name="version">Versión del método de autentificación.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns>El valor máximo del campo 'credential' del método indicado.</returns>
    ''' <remarks></remarks>
    Public Shared Function MaxCredentialValue(ByVal method As AuthenticationMethod, ByVal version As String, ByVal oState As roWsState) As roGenericVtResponse(Of Long)

        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Long)
        oResult.Value = roPassportManager.MaxCredentialValue(method, version)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
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
    Public Shared Function GetFeaturesList(ByVal type As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Feature))
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Feature))

        Dim oFeatures As New Generic.List(Of Feature)
        Dim _business As New FeaturesBusiness(-1, type)

        Dim oFeature As Feature
        For Each row As FeaturesDataSet.FeaturesRow In _business.Table.Select("Type='" & type & "'", "ID")
            If row.MaxConfigurable > Permission.None Then
                oFeature = New Feature(row, bState)
                oFeatures.Add(oFeature)
            End If
        Next

        oResult.Value = oFeatures

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
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

    Public Shared Function GetFeaturesFromPassport(ByVal idPassport As Integer, ByVal idFeature As Nullable(Of Integer), ByVal type As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Feature))

        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Feature))
        oResult.Value = FeaturesBusiness.GetFeaturesFromPassport(idPassport, idFeature, type, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de todas las funcionalidades de un passport
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFeaturesFromPassportAll(ByVal idPassport As Integer, ByVal type As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Feature))

        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Feature))
        oResult.Value = FeaturesBusiness.GetFeaturesFromPassportAll(idPassport, type, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Obtiene la lista de permisos posibles a asignar a una funcionalidad para un passport en concreto.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetFeaturePermissions(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Generic.List(Of Permission))
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Generic.List(Of Permission))
        oResult.Value = FeaturesBusiness.GetFeaturePermissions(idPassport, idFeature, FeatureType, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
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
    Public Shared Function SetFeaturePermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal _permission As Permission, ByVal FeaturesChanged As Generic.List(Of Feature), ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Generic.List(Of Feature)))
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (Boolean, Generic.List(Of Feature)))

        Dim bolRet As Boolean = FeaturesBusiness.SetFeaturePermission(idPassport, idFeature, FeatureType, _permission, FeaturesChanged, bState)
        oResult.Value = (bolRet, FeaturesChanged)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Reestablece el permiso por defecto a una cierta funcionalidad.
    ''' </summary>
    ''' <param name="idPassport"></param>
    ''' <param name="idFeature"></param>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function SetDefaultFeaturePermission(ByVal idPassport As Integer, ByVal idFeature As Integer, ByVal FeatureType As String, ByVal FeaturesChanged As Generic.List(Of Feature), ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, Generic.List(Of Feature)))
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (Boolean, Generic.List(Of Feature)))

        Dim bolRet As Boolean = FeaturesBusiness.SetDefaultFeaturePermission(idPassport, idFeature, FeatureType, FeaturesChanged, bState)
        oResult.Value = (bolRet, FeaturesChanged)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

#End Region

    ''' <summary>
    ''' Obtiene la lista de idiomas definidos en la bd
    ''' </summary>
    ''' <param name="oState"></param>
    ''' <returns></returns>
    ''' <remarks></remarks>

    Public Shared Function GetLanguages(ByVal oState As roWsState) As roGenericVtResponse(Of roPassportLanguage())
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPassportLanguage())
        Dim oLangManager As New roLanguageManager()
        oResult.Value = oLangManager.LoadLanguages()

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function GetLanguageByKey(key As String, ByVal oState As roWsState) As roGenericVtResponse(Of roPassportLanguage)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPassportLanguage)
        Dim oLangManager As New roLanguageManager()
        oResult.Value = oLangManager.LoadByKey(key)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    ''' <summary>
    ''' Crea user nuevo en el pasaporte indicado.
    ''' </summary>
    ''' <param name="oPassport">Pasaporte a grabar.</param>
    ''' <param name="oState">Objeto con el estado del resultado de la ejecución.</param>
    ''' <returns><b>True</b> si se ha podido crear; en caso contrario, <b>False</b>.</returns>
    ''' <remarks></remarks>
    Public Shared Function CreateUserOfPassport(ByVal oDTOPassport As roPassport, ByVal oState As roWsState) As roGenericVtResponse(Of (Boolean, roPassport))
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (Boolean, roPassport))

        oResult.Value = (True, oDTOPassport)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function




    ''' <summary>
    ''' retorna bool diciendo si el business group está en uso
    ''' </summary>
    Public Shared Function BusinessGroupListInUse(ByVal strBusinessGroup As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = wscUserAdmin.BusinessGroupListInUse(strBusinessGroup, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function SaveParameterInContext(ByVal strParameterName As String, ByVal strParameterValue As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)

        Dim bolRet As Boolean = True
        Try
            Dim oContext As CContext = Nothing
            Try
                oContext = WLHelper.GetContext(oState.IDPassport)
            Catch
                oContext = Nothing
            End Try

            Select Case strParameterName
                Case "DatalinkMode"
                    oContext.DatalinkMode = strParameterValue.Trim
            End Select

            WLHelper.SetContext(oState.IDPassport, oContext)
        Catch ex As Data.Common.DbException
            bState.UpdateStateInfo(ex, "roCalendarManager::GetCalendarPassportConfig")
            bolRet = False
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "roCalendarManager::GetCalendarPassportConfig")
            bolRet = False
        Finally
            '
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

End Class