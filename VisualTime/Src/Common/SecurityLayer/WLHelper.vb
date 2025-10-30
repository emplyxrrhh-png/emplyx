Imports System.Data.Common
Imports System.Xml.Serialization
Imports Robotics.Azure
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Security.Base
Imports Robotics.Security.DataAccess
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.roConstants

''' <summary>
''' Serves as the main entry point to access WebLogin objects.
''' </summary>
Public NotInheritable Class WLHelper

    Private Sub New()
    End Sub

    Public Shared Function GetSystemEmailUserFieldName() As String
        Dim sFieldName As String = String.Empty
        Try
            Dim strSQL As String = "@SELECT# FieldName from sysroUserFields where Alias = 'sysroEmail'"
            sFieldName = roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL))
        Catch ex As Exception
            roLog.GetInstance.logMessage(roLog.EventType.roError, "WLHelper::GetSystemEmailUserFieldName", ex)
        End Try

        Return sFieldName
    End Function

    Public Shared Function RecoverEmployeePassword(ByVal strUserName As String, ByVal strEmail As String, ByVal appType As roAppType) As Boolean
        Dim bRet As Boolean = False

        Dim oPassport As roPassport = roPassportManager.GetPassportByCredential(strUserName, AuthenticationMethod.Password, "")

        If oPassport IsNot Nothing Then
            Dim email As String = String.Empty

            If oPassport.Email.Trim <> String.Empty Then
                email = oPassport.Email
            Else
                Dim strSQL As String = "@SELECT# FieldName from sysroUserFields where Alias = 'sysroEmail'"
                Dim strEmailUsrField As String = roTypes.Any2String(AccessHelper.ExecuteScalar(strSQL))

                If strEmailUsrField.Length > 0 AndAlso oPassport.IDEmployee IsNot Nothing AndAlso oPassport.IDEmployee > 0 Then
                    ' Miramos el valor del campo de la ficha del empleado
                    strSQL = "@DECLARE# @Date smalldatetime " &
                                    "SET @Date = " & roTypes.Any2Time(Now.Date).SQLSmallDateTime & " " &
                                    "@SELECT# * FROM GetEmployeeUserFieldValue(" & roTypes.Any2String(oPassport.IDEmployee) & ",'" & strEmailUsrField & "', @Date)"
                    Dim tbs As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
                    If tbs IsNot Nothing AndAlso tbs.Rows.Count > 0 Then
                        email = tbs.Rows(0).Item("Value").ToString
                    End If

                End If
            End If

            If email.ToLower = strEmail.ToLower Then
                'Creamos la key de recuperación de contraseña y enviamos el email al empleado
                If oPassport.IDEmployee IsNot Nothing AndAlso oPassport.IDEmployee > 0 Then
                    bRet = ConnectionAccess.CreateRecoverPasswordKey(oPassport.ID, oPassport.IDEmployee, appType)
                Else
                    bRet = ConnectionAccess.CreateRecoverPasswordKey(oPassport.ID, 0, appType)
                End If

            End If
        End If

        Return bRet
    End Function

    Public Shared Function GetLastLogin(ByVal intIDPassport As Integer) As Date
        Dim bRet As DateTime = Nothing
        Try

            Dim strSQL = "@select# LastLoginDate from sysroPassports WHERE ID =" & intIDPassport

            Dim tb As DataTable = DataLayer.AccessHelper.CreateDataTable(strSQL)
            If tb IsNot Nothing And tb.Rows.Count > 0 Then
                For Each oRow As DataRow In tb.Rows
                    If Not tb.Rows(0).Item("LastLoginDate").Equals(DBNull.Value) Then
                        bRet = roTypes.Any2DateTime(tb.Rows(0).Item("LastLoginDate"))
                    Else
                        bRet = New Date(2079, 1, 1)
                    End If

                Next
            End If
        Catch ex As System.Data.Common.DbException
            bRet = New Date(2079, 1, 1)
        Catch ex As Exception
            bRet = New Date(2079, 1, 1)
        Finally
        End Try
        Return bRet
    End Function

    Public Shared Function UpdateLastLogin(ByVal intIDPassport As Integer) As Boolean
        Dim bRet As Boolean = False
        Try

            Dim strSQL = "@UPDATE# sysroPassports SET LastLoginDate=CurrentLastLoginDate, CurrentLastLoginDate =" & roTypes.Any2Time(DateTime.Now).SQLDateTime & " WHERE ID =" & intIDPassport
            bRet = roTypes.Any2String(DataLayer.AccessHelper.ExecuteSql(strSQL))
        Catch ex As System.Data.Common.DbException
        Catch ex As Exception
        Finally
        End Try
        Return bRet
    End Function

    Public Shared Function DeleteFirebaseToken(ByVal intIDPassport As Integer, ByVal uuid As String) As Boolean
        Dim bRet As Boolean = False

        Try
            Dim strSQL = "@delete# from sysroPassports_DeviceTokens where idPassport =" & intIDPassport & " and uuid= '" & uuid & "'"
            bRet = DataLayer.AccessHelper.ExecuteSql(strSQL)
        Catch ex As System.Data.Common.DbException
        Catch ex As Exception
        Finally
        End Try
        Return bRet
    End Function

    Public Shared Function RegisterFirebaseToken(ByVal token As String, ByVal uuid As String, ByVal intIDPassport As Integer) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim strSQL = "@select# count(idpassport) from sysroPassports_DeviceTokens where [token]=@token and [idpassport]=@idpassport and [uuid]=@uuid"
            Dim parameters As New List(Of CommandParameter) From {
                    New CommandParameter("@token", CommandParameter.ParameterType.tString, token),
                    New CommandParameter("@uuid", CommandParameter.ParameterType.tString, uuid),
                    New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, intIDPassport)
                }

            Dim iCount As Integer = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL, parameters))

            If iCount > 0 Then
                Dim strSQL2 = "@update# sysroPassports_DeviceTokens set [registrationdate]=getDate() where [idpassport]=@idpassport and [uuid]=@uuid and [token]=@token"
                bRet = roTypes.Any2String(DataLayer.AccessHelper.ExecuteSql(strSQL2, parameters))
            Else
                Dim sDelteTokens As String = "@delete# from sysroPassports_DeviceTokens where [idpassport] <> @idpassport and [token]=@token"
                Dim delParam As New List(Of CommandParameter) From {
                    New CommandParameter("@token", CommandParameter.ParameterType.tString, token),
                    New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, intIDPassport)
                }
                DataLayer.AccessHelper.ExecuteSql(sDelteTokens, delParam)

                sDelteTokens = "@delete# from sysroPassports_DeviceTokens where [idpassport] = @idpassport and [uuid]=@uuid"
                delParam = New List(Of CommandParameter) From {
                    New CommandParameter("@uuid", CommandParameter.ParameterType.tString, uuid),
                    New CommandParameter("@idpassport", CommandParameter.ParameterType.tInt, intIDPassport)
                }
                DataLayer.AccessHelper.ExecuteSql(sDelteTokens, delParam)

                Dim strSQL2 = "@insert# sysroPassports_DeviceTokens ([idpassport],[token],[uuid],[registrationdate]) values (@idpassport,@token,@uuid,getDate())"
                bRet = roTypes.Any2String(DataLayer.AccessHelper.ExecuteSql(strSQL2, parameters))
            End If
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "WLHelper::RegisterFirebaseToken::Error setting push token", ex)
        End Try
        Return bRet
    End Function

    Public Shared Function UpdateHelpVersion(ByVal intIDPassport As Integer, ByVal intHelpVersion As Integer, ByVal oState As roWsState) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim strSQL = "@update# sysroPassports set ShowHelp = " & intHelpVersion & " where id=" & intIDPassport
            bRet = roTypes.Any2String(DataLayer.AccessHelper.ExecuteSql(strSQL))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "WLHelper::UpdateHelpVersion", ex)
        End Try
        Return bRet
    End Function

    Public Shared Function GetHelpVersion(ByVal intIDPassport As String, ByVal oState As roWsState) As Integer
        Dim bRet As Integer = 0
        Try
            Dim strSQL = "@select# ShowHelp from sysroPassports where id=" & intIDPassport
            bRet = roTypes.Any2Integer(DataLayer.AccessHelper.ExecuteScalar(strSQL))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "WLHelper::GetHelpVersion", ex)
        End Try
        Return bRet
    End Function

    Public Shared Function ValidateRecoverPasswordRequestKey(ByVal strRequestKey As String, ByVal idPassport As Integer, ByVal appType As roAppType) As Boolean
        Dim bRet As Boolean = False

        'Creamos la key de recuperación de contraseña y enviamos el email al empleado
        bRet = ConnectionAccess.ValidateRecoverPasswordRequestKey(idPassport, strRequestKey, appType)

        Return bRet
    End Function

    ''' <summary>
    ''' IsProductivEmployee
    ''' </summary>
    Public Shared Function IsProductiVEmployee(ByVal idpassport As Integer) As Boolean
        Dim Result As Boolean = False
        Result = ConnectionAccess.IsProductiVEmployee(idpassport)
        Return Result
    End Function


    ''' <summary>
    ''' Returns the permission specified passport have over specified feature.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The id of the passport</param>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    Public Shared Function GetPermissionOverFeature(ByVal _CurrentPassportID As Integer, ByVal featureAlias As String, ByVal featureType As String) As Permission
        Return CType(PermissionsAccess.PermissionsOverFeatures_Get(_CurrentPassportID, featureAlias, featureType, PermissionCheckMode.Normal), Permission)

    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified feature.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The id of the current passport</param>
    ''' <param name="strFeatureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="strFeatureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFeaturePermission(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, Optional ByVal strFeatureType As String = "U") As Permission
        Return WLHelper.GetPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType)
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
    ''' <param name="featureType">The type of features: 'E' for Employee or 'U' for User.</param>
    Public Shared Function GetPermissionOverEmployee(ByVal _CurrentPassportID As Integer, ByVal idEmployee As Integer, ByVal applicationAlias As String, Optional ByVal featureType As String = "") As Permission
        Dim IDApp As Nullable(Of Integer) = FeaturesAccess.GetFeatureIdByAlias(applicationAlias, featureType)
        If IDApp.HasValue Then
            Return CType(PermissionsAccess.PermissionsOverEmployees_Get(_CurrentPassportID, idEmployee, IDApp.Value, PermissionCheckMode.Normal, True), Permission)
        Else
            Return Permission.None
        End If
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
    ''' <param name="dDate">The date on we are looking for the permisions</param>
    ''' <param name="featureType">The type of features: 'E' for Employee or 'U' for User.</param>
    Public Shared Function GetPermissionOverEmployeeOnDate(ByVal _CurrentPassportID As Integer, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal dDate As DateTime, Optional ByVal featureType As String = "") As Permission
        Dim IDApp As Nullable(Of Integer) = FeaturesAccess.GetFeatureIdByAlias(applicationAlias, featureType)
        If IDApp.HasValue Then
            Return CType(PermissionsAccess.PermissionsOverEmployees_Get_Extended(_CurrentPassportID, idEmployee, IDApp.Value, PermissionCheckMode.Normal, True, dDate), Permission)
        Else
            Return Permission.None
        End If
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified feature and employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the current passport.</param>
    ''' <param name="strFeatureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="intIDEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="strFeatureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFeaturePermissionByEmployee(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oEmployeePermission As Permission = WLHelper.GetPermissionOverEmployee(_CurrentPassportID, intIDEmployee, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = WLHelper.GetPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType)
            If oEmployeePermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oEmployeePermission
            End If
        Else
            Return WLHelper.GetPermissionOverEmployee(_CurrentPassportID, intIDEmployee, strFeatureAlias, strFeatureType)
        End If
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    Public Shared Function GetPermissionOverEmployee(ByVal _CurrentPassportID As Integer, ByVal idEmployee As Integer, ByVal idApplication As Integer) As Permission
        Return CType(PermissionsAccess.PermissionsOverEmployees_Get(_CurrentPassportID, idEmployee, idApplication, PermissionCheckMode.Normal, True), Permission)
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified group.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="featureType">The type of features: 'E' for Employee or 'U' for User.</param>
    Public Shared Function GetPermissionOverGroup(ByVal _CurrentPassportID As Integer, ByVal idGroup As Integer, ByVal applicationAlias As String, Optional ByVal featureType As String = "") As Permission
        Dim IDApp As Nullable(Of Integer) = FeaturesAccess.GetFeatureIdByAlias(applicationAlias, featureType)
        If IDApp.HasValue Then
            Return CType(PermissionsAccess.PermissionsOverGroups_Get(_CurrentPassportID, idGroup, IDApp.Value, PermissionCheckMode.Normal), Permission)
        Else
            Return Permission.None
        End If
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified group.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    Public Shared Function GetPermissionOverGroup(ByVal _CurrentPassportID As Integer, ByVal idGroup As Integer, ByVal idApplication As Integer) As Permission
        Return CType(PermissionsAccess.PermissionsOverGroups_Get(_CurrentPassportID, idGroup, idApplication, PermissionCheckMode.Normal), Permission)
    End Function

    ''' <summary>
    ''' Returns the permission specified passport have over specified feature and group.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the current passport.</param>
    ''' <param name="strFeatureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="intIDGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="strFeatureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function GetFeaturePermissionByGroup(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Permission
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim oGroupPermission As Permission = WLHelper.GetPermissionOverGroup(_CurrentPassportID, intIDGroup, strFeatureAlias.Split(".")(0), strFeatureType)
            Dim oFeaturePermission As Permission = WLHelper.GetPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType)
            If oGroupPermission > oFeaturePermission Then
                Return oFeaturePermission
            Else
                Return oGroupPermission
            End If
        Else
            Return WLHelper.GetPermissionOverGroup(_CurrentPassportID, intIDGroup, strFeatureAlias, strFeatureType)
        End If
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission over
    ''' specified feature.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="perm">The required permission.</param>
    Public Shared Function HasPermissionOverFeature(ByVal _CurrentPassportID As Integer, ByVal featureAlias As String, ByVal featureType As String, ByVal perm As Permission) As Boolean

        Return PermissionsAccess.PermissionsOverFeatures_Get(_CurrentPassportID, featureAlias, featureType, PermissionCheckMode.Normal) >= perm

    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission over
    ''' specified feature.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the current passport.</param>
    ''' <param name="strFeatureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="oPermission">The required permission.</param>
    ''' <param name="strFeatureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function HasFeaturePermission(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, Optional ByVal strFeatureType As String = "U") As Boolean
        Return WLHelper.HasPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType, oPermission)
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission
    ''' over specified employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="featureType">The type of features: 'E' for Employee or 'U' for User.</param>
    Public Shared Function HasPermissionOverEmployee(ByVal _CurrentPassportID As Integer, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal perm As Permission, Optional ByVal featureType As String = "") As Boolean
        Dim IDApp As Nullable(Of Integer) = FeaturesAccess.GetFeatureIdByAlias(applicationAlias, featureType)
        If IDApp.HasValue Then
            Return PermissionsAccess.PermissionsOverEmployees_Get(_CurrentPassportID, idEmployee, IDApp.Value, PermissionCheckMode.Normal, True) >= perm
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission
    ''' over specified employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="dDate">The date for which to get permissions.</param>
    ''' <param name="featureType">The type of features: 'E' for Employee or 'U' for User.</param>
    Public Shared Function HasPermissionOverEmployeeOnDate(ByVal _CurrentPassportID As Integer, ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal perm As Permission, ByVal dDate As DateTime, Optional ByVal featureType As String = "") As Boolean
        Dim IDApp As Nullable(Of Integer) = FeaturesAccess.GetFeatureIdByAlias(applicationAlias, featureType)
        If IDApp.HasValue Then
            Return PermissionsAccess.PermissionsOverEmployees_Get_Extended(_CurrentPassportID, idEmployee, IDApp.Value, PermissionCheckMode.Normal, True, dDate) >= perm
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission
    ''' over specified feature annd employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the current passport.</param>
    ''' <param name="strFeatureAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="oPermission">The required permission.</param>
    ''' <param name="intIDEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="strFeatureType">The type of features: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function HasFeaturePermissionByEmployee(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = WLHelper.HasPermissionOverEmployee(_CurrentPassportID, intIDEmployee, strFeatureAlias.Split(".")(0), oPermission, strFeatureType)
            Dim bolFeatureHasPermission As Boolean = WLHelper.HasPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return WLHelper.HasPermissionOverEmployee(_CurrentPassportID, intIDEmployee, strFeatureAlias, oPermission, strFeatureType)
        End If
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission
    ''' over specified feature annd employee.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the current passport.</param>
    ''' <param name="strFeatureAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="oPermission">The required permission.</param>
    ''' <param name="intIDEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="dDate">The date for which to get permissions.</param>
    ''' <param name="strFeatureType">The type of features: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function HasFeaturePermissionByEmployeeOnDate(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDEmployee As Integer, ByVal dDate As DateTime, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolEmployeeHasPermission As Boolean = WLHelper.HasPermissionOverEmployeeOnDate(_CurrentPassportID, intIDEmployee, strFeatureAlias.Split(".")(0), oPermission, dDate, strFeatureType)
            Dim bolFeatureHasPermission As Boolean = WLHelper.HasPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType, oPermission)
            Return (bolEmployeeHasPermission And bolFeatureHasPermission)
        Else
            Return WLHelper.HasPermissionOverEmployeeOnDate(_CurrentPassportID, intIDEmployee, strFeatureAlias, oPermission, dDate, strFeatureType)
        End If
    End Function


    ''' <summary>
    ''' Returns whether specified passport have specified permission over specified group.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="featureType">The type of features: 'E' for Employee or 'U' for User.</param>
    Public Shared Function HasPermissionOverGroup(ByVal _CurrentPassportID As Integer, ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal perm As Permission, Optional ByVal featureType As String = "") As Boolean
        Dim IDApp As Nullable(Of Integer) = FeaturesAccess.GetFeatureIdByAlias(applicationAlias, featureType)
        If IDApp.HasValue Then
            Return PermissionsAccess.PermissionsOverGroups_Get(_CurrentPassportID, idGroup, IDApp.Value, PermissionCheckMode.Normal) >= perm
        Else
            Return False
        End If
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission over specified group.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the passport</param>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    Public Shared Function HasPermissionOverGroup(ByVal _CurrentPassportID As Integer, ByVal idGroup As Integer, ByVal idApplication As Integer, ByVal perm As Permission) As Boolean
        Return PermissionsAccess.PermissionsOverGroups_Get(_CurrentPassportID, idGroup, idApplication, PermissionCheckMode.Normal) >= perm
    End Function

    ''' <summary>
    ''' Returns whether specified passport have specified permission
    ''' over specified feature and group.
    ''' </summary>
    ''' <param name="_CurrentPassportID">The ID of the current passport.</param>
    ''' <param name="strFeatureAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="oPermission">The required permission.</param>
    ''' <param name="intIDGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="strFeatureType">The type of features: 'E' for Employee or 'U' for User.</param>
    ''' <returns></returns>
    ''' <remarks></remarks>
    Public Shared Function HasFeaturePermissionByGroup(ByVal _CurrentPassportID As Integer, ByVal strFeatureAlias As String, ByVal oPermission As Permission, ByVal intIDGroup As Integer, Optional ByVal strFeatureType As String = "U") As Boolean
        If strFeatureAlias.Split(".").Length > 1 Then
            ' No es una funcionalidad raiz, miramos los permisos del empleado sobre la funcionalidad raiz (ej: Employees.Type -> Employees)
            Dim bolGroupHasPermission As Boolean = WLHelper.HasPermissionOverGroup(_CurrentPassportID, intIDGroup, strFeatureAlias.Split(".")(0), oPermission, strFeatureType)
            Dim bolFeatureHasPermission As Boolean = WLHelper.HasPermissionOverFeature(_CurrentPassportID, strFeatureAlias, strFeatureType, oPermission)
            Return (bolGroupHasPermission And bolFeatureHasPermission)
        Else
            Return WLHelper.HasPermissionOverGroup(_CurrentPassportID, intIDGroup, strFeatureAlias, oPermission, strFeatureType)
        End If
    End Function

    Public Shared Function GetContext(ByVal intIDPassport As Integer) As CContext

        Dim oContext As CContext = Nothing

        Try
            Dim oManager As New roPassportManager
            Dim sContext As String = oManager.GetPassportContext(intIDPassport)
            If sContext <> "" Then
                oContext = ContextFromXml(sContext)
                oContext.CalculateDatesPeriod()
            End If
        Catch ex As DbException
        End Try

        Return oContext
    End Function

    Public Shared Sub SetContext(ByVal intIDPassport As Integer, ByVal oContext As CContext)
        Dim oManager As New roPassportManager
        Dim xml As String = XmlSerializer(oContext, oContext.GetType())

        oManager.UpdatePassportContext(intIDPassport, xml)
    End Sub

    Private Shared Function XmlSerializer(ByVal oObject As Object, ByVal oType As System.Type) As String

        Dim oSer As New Xml.Serialization.XmlSerializer(oType)
        Dim strm As New System.IO.MemoryStream
        Dim bMessage As Byte()
        Dim strMessage As String = ""
        oSer.Serialize(strm, oObject)
        If strm.CanRead Then
            ReDim bMessage(strm.Length - 1)
            strm.Position = 0
            strm.Read(bMessage, 0, strm.Length)
            strm.Close()
            strMessage = System.Text.Encoding.UTF8.GetString(bMessage, 0, bMessage.Length)
        End If
        strm.Close()

        Return strMessage

    End Function

    Private Shared Function ContextFromXml(ByVal strXml As String) As CContext

        Dim oRet As CContext = Nothing
        Dim oSer As XmlSerializer
        Dim strm As System.IO.MemoryStream = Nothing

        Try

            strm = New System.IO.MemoryStream()

            Dim bData As Byte()
            ReDim bData(System.Text.Encoding.UTF8.GetByteCount(strXml) - 1)
            bData = System.Text.Encoding.UTF8.GetBytes(strXml)

            strm.Write(bData, 0, bData.Length)
            strm.Position = 0

            oSer = New XmlSerializer(GetType(CContext))
            oRet = CType(oSer.Deserialize(strm), CContext)
        Catch Ex As Xml.XmlException
            ''Stop
        Catch Ex As Exception
            ''Stop
        Finally
            If strm IsNot Nothing Then strm.Close()
        End Try

        Return oRet

    End Function

    Public Shared Function GetVersionNotificationShown(intIDPassport As Integer, oState As roWsState) As Boolean
        Dim bRet As Integer = 0
        Try
            Dim strSQL = $"@SELECT# ShowUpdatePopup from sysroPassports where ID = {intIDPassport}"
            bRet = roTypes.Any2Boolean(DataLayer.AccessHelper.ExecuteScalar(strSQL))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "WLHelper::GetVersionNotificationShown", ex)
        End Try
        Return bRet
    End Function

    Public Shared Function UpdateNotificationVersion(intIDPassport As Integer, intNotificationVersion As Integer, oState As roWsState) As Boolean
        Dim bRet As Boolean = False
        Try
            Dim strSQL = $"@UPDATE# sysroPassports SET ShowUpdatePopup = {intNotificationVersion} WHERE ID = {intIDPassport}"
            bRet = roTypes.Any2String(DataLayer.AccessHelper.ExecuteSql(strSQL))
        Catch ex As Exception
            roLog.GetInstance().logMessage(roLog.EventType.roError, "WLHelper::UpdateNotificationVersion", ex)
        End Try
        Return bRet
    End Function
End Class