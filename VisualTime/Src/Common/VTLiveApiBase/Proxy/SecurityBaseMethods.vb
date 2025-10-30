Imports Robotics.Base.DTOs
Imports Robotics.Base.VTBusiness.Common
Imports Robotics.Security
Imports Robotics.Security.Base
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Public Class SecurityBaseMethods
    ''' <summary>
    ''' Authenticates a user by validating it's credential and password for a specified login method.
    ''' </summary>
    ''' <param name="method">The authentication method to use.</param>
    ''' <param name="credential">Credentials: username, windows account, biometrical data, etc.</param>
    ''' <param name="password">Hashed password to validate credential.</param>
    ''' <param name="hashPassword">Wether to hash password.</param>
    ''' <returns>A passport if authentication succeeds, otherwise, nothing.</returns>

    Public Shared Function Authenticate(ByVal oPassportTicket As roPassportTicket, ByVal method As AuthenticationMethod, ByVal credential As String, ByVal password As String, ByVal hashPassword As Boolean, ByVal oState As roWsState,
                                 ByVal strAPP As String, ByVal strVersionAPP As String, ByVal strVersionServer As String, ByVal strMail As String, ByVal isSSOLogin As Boolean, ByVal strAuthToken As String,
                                 ByVal strSecurityToken As String, ByVal bolAudit As Boolean) As roGenericVtResponse(Of (roPassportTicket, String))

        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of (roPassportTicket, String))
        Dim tmpPassport As roPassportTicket = AuthHelper.Authenticate(oPassportTicket, method, credential, password, hashPassword, bState, strAPP.Trim.ToUpper = "1", strVersionAPP, strVersionAPP, strMail, isSSOLogin, strAuthToken, strSecurityToken, bolAudit)

        oResult.Value = (tmpPassport, strSecurityToken)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    Public Shared Function GetPassportBelongsToAdminGroup(ByVal intIdPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            oResult.Value = roPassportManager.GetPassportBelongsToAdminGroup(intIdPassport)
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetPassportBelongsToAdminGroup")
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function GetPermissionOverFeature(ByVal featureAlias As String, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Permission)
        Try
            If oState.IDPassport > 0 Then
                oResult.Value = WLHelper.GetPermissionOverFeature(oState.IDPassport, featureAlias, featureType)
            Else
                oResult.Value = Permission.None
            End If
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetPermissionOverFeature")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetPermissionOverFeature")
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function SetLastNotificationSended(ByVal iPassportID As Integer, ByVal oDate As Nullable(Of DateTime), ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = roPassportManager.SetLastNotificationSended(iPassportID, oDate)
        If Not oResult.Value Then
            oState.Result = SecurityResultEnum.Exception
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function UpdateLastAccessTimeMVC(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        oResult.Value = SessionHelper.UpdateLastAccessTime(oState.SessionID, oState.IDPassport, bState)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function UpdateLastAccessTime(ByVal sessionID As String, ByVal iPassportID As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        If iPassportID > 0 AndAlso oState.SessionID <> String.Empty Then
            oResult.Value = SessionHelper.UpdateLastAccessTime(sessionID, iPassportID, bState)
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function GetPermissionOverEmployeeAppAlias(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Permission)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.GetPermissionOverEmployee(oState.IDPassport, idEmployee, applicationAlias, featureType)
        Else
            oResult.Value = Permission.None
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The ID of the application in which to check permissions.</param>
    ''' <param name="dDate">The date in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function GetPermissionOverEmployeeOnDateAppAlias(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal dDate As DateTime, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Permission)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.GetPermissionOverEmployeeOnDate(oState.IDPassport, idEmployee, applicationAlias, dDate, featureType)
        Else
            oResult.Value = Permission.None
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function HasPermissionOverEmployeeOnDateEx(ByVal strEmployees() As String, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of String())
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String())
        Dim bolret As String() = Nothing

        Dim tmpbolRet As New Generic.List(Of String)
        Try
            If oState.IDPassport > 0 Then
                For Each strSQL As String In strEmployees
                    If WLHelper.HasPermissionOverEmployeeOnDate(oState.IDPassport, strSQL.Split("@")(0), applicationAlias, perm, strSQL.Split("@")(1), featureType) Then
                        tmpbolRet.Add(strSQL)
                    End If

                Next

            End If

            If Not tmpbolRet Is Nothing Then
                bolret = tmpbolRet.ToArray
            End If
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::HasPermissionOverEmployeeOnDateEx")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::HasPermissionOverEmployeeOnDateEx")
        End Try

        oResult.Value = bolret

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function HasPermissionOverGroupAppAliasEx(ByVal strGroups() As String, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of String())
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String())
        Dim bolret As String() = Nothing

        Dim tmpbolRet As New Generic.List(Of String)
        Try
            If oState.IDPassport > 0 Then
                For Each strSQL As String In strGroups
                    If WLHelper.HasPermissionOverGroup(oState.IDPassport, strSQL, applicationAlias, perm, featureType) Then
                        tmpbolRet.Add(strSQL)
                    End If
                Next
            End If

            If Not tmpbolRet Is Nothing Then
                bolret = tmpbolRet.ToArray
            End If
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::HasPermissionOverGroupAppAliasEx")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::HasPermissionOverGroupAppAliasEx")
        End Try

        oResult.Value = bolret

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function GetPermissionOverEmployeeAppId(ByVal idEmployee As Integer, ByVal idApplication As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Permission)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Permission)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.GetPermissionOverEmployee(oState.IDPassport, idEmployee, idApplication)
        Else
            oResult.Value = Permission.None
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function GetPermissionOverGroupAppAlias(ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal oState As roWsState) As roGenericVtResponse(Of Permission)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Permission)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.GetPermissionOverGroup(oState.IDPassport, idGroup, applicationAlias, featureType)
        Else
            oResult.Value = Permission.None
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns the permission current passport have over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function GetPermissionOverGroupAppId(ByVal idGroup As Integer, ByVal idApplication As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Permission)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Permission)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.GetPermissionOverGroup(oState.IDPassport, idGroup, idApplication)
        Else
            oResult.Value = Permission.None
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission over
    ''' specified feature.
    ''' </summary>
    ''' <param name="featureAlias">The alias of the feature to check permissions for.</param>
    ''' <param name="featureType">The type of feature: 'E' for Employee or 'U' for User.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function HasPermissionOverFeature(ByVal featureAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.HasPermissionOverFeature(oState.IDPassport, featureAlias, featureType, perm)
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult

    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission
    ''' over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function HasPermissionOverEmployee(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.HasPermissionOverEmployee(oState.IDPassport, idEmployee, applicationAlias, perm, featureType)
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission
    ''' over specified employee.
    ''' </summary>
    ''' <param name="idEmployee">The ID of the employee for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function HasPermissionOverEmployeeOnDate(ByVal idEmployee As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal dDate As DateTime, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.HasPermissionOverEmployeeOnDate(oState.IDPassport, idEmployee, applicationAlias, perm, dDate, featureType)
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="applicationAlias">The alias of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function HasPermissionOverGroupAppAlias(ByVal idGroup As Integer, ByVal applicationAlias As String, ByVal featureType As String, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.HasPermissionOverGroup(oState.IDPassport, idGroup, applicationAlias, perm, featureType)
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns whether current passport have specified permission over specified group.
    ''' </summary>
    ''' <param name="idGroup">The ID of the group for which to get permissions.</param>
    ''' <param name="idApplication">The ID of the application in which to check permissions.</param>
    ''' <param name="perm">The required permission.</param>
    ''' <param name="oState">The result error information</param>

    Public Shared Function HasPermissionOverGroupAppId(ByVal idGroup As Integer, ByVal idApplication As Integer, ByVal perm As Permission, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        If bState.IDPassport > 0 Then
            oResult.Value = WLHelper.HasPermissionOverGroup(oState.IDPassport, idGroup, idApplication, perm)
        Else
            oResult.Value = False
        End If

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns a ticket containing passport information for caching.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport to return ticket for.</param>

    Public Shared Function GetPassportTicket(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of roPassportTicket)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPassportTicket)
        Dim oRet As roPassportTicket = Nothing
        Try
            oRet = roPassportManager.GetPassportTicket(idPassport)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetPassportTicket")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetPassportTicket")
        End Try
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns if reset validation code is correct.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport </param>

    Public Shared Function ResetValidationCodeRobotics(ByVal idPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oRet As Boolean = False
        Try
            oRet = AuthHelper.ResetValidationCodeRobotics(idPassport)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::ResetValidationCode")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::ResetValidationCode")
        End Try
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns if reset validation code is correct.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport </param>

    Public Shared Function ResetValidationCode(ByVal idPassport As Integer, ByVal strClientLocation As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oRet As Boolean = False
        Try
            oRet = AuthHelper.ResetValidationCode(idPassport, strClientLocation)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::ResetValidationCode")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::ResetValidationCode")
        End Try
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns if valid code validation.
    ''' </summary>
    ''' <param name="idPassport">The ID of the passport </param>
    ''' <param name="strCode">Vaidation code </param>

    Public Shared Function IsValidCode(ByVal idPassport As Integer, ByVal strCode As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim oRet As Boolean = False
        Try
            oRet = AuthHelper.IsValidCode(idPassport, strCode)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::IsValidCode")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::IsValidCode")
        End Try
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    ''' <summary>
    ''' Returns a ticket containing passport information for caching.
    ''' </summary>
    ''' <param name="SessionID">The ID of the session to return ticket for.</param>

    Public Shared Function GetPassportTicketBySessionID(ByVal PassportID As String, ByVal oMethod As AuthenticationMethod, ByVal oState As roWsState) As roGenericVtResponse(Of roPassportTicket)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of roPassportTicket)
        Dim oRet As roPassportTicket = Nothing
        Try
            oRet = SessionHelper.GetPassportTicketBySessionID(PassportID, oMethod, bState)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetPassportTicketFromSessionID")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetPassportTicketFromSessionID")
        End Try
        oResult.Value = oRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetContext(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of CContext)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of CContext)
        Dim oContext As CContext = Nothing
        Try
            oContext = WLHelper.GetContext(intIDPassport)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetContext")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetContext")
        Finally
            If oContext Is Nothing Then oContext = New CContext
        End Try
        oResult.Value = oContext

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function SetContext(ByVal intIDPassport As Integer, ByVal oContext As CContext, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Try
            WLHelper.SetContext(intIDPassport, oContext)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::SetContext")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::SetContext")
        End Try
        oResult.Value = (oState.Result = SecurityResultEnum.NoError)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function SignOut(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False
        Try
            Dim oPassport As roPassportTicket = roPassportManager.GetPassportTicket(intIDPassport)
            If oPassport IsNot Nothing Then
                If oState.SessionID <> "" AndAlso SessionHelper.SessionRemove(oState.SessionID, bState) Then
                    ' Auditamos desconexión
                    oState.IDPassport = oPassport.ID
                    bState.Audit(Audit.Action.aDisconnect, Audit.ObjectType.tConnection, oPassport.Name, Nothing, -1)
                    ' Guardamos historial de conexion del usuario
                    SessionHelper.UpdateConnectionHistory(bState, oPassport.Name, Audit.Action.aDisconnect)

                    bolRet = True
                Else
                    oState.Result = SecurityResultEnum.DeleteSessionError
                    bolRet = False
                End If
            End If
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::SignOut")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::SignOut")
        End Try
        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

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

        Try
            Dim oLangManager As New roLanguageManager
            oResult.Value = oLangManager.LoadLanguages()
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetLanguages")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetLanguages")
        End Try

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function UpdateHelpVersion(ByVal intIDPassport As Integer, ByVal intHelpVersion As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False

        Try

            bolRet = WLHelper.UpdateHelpVersion(intIDPassport, intHelpVersion, oState)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::UpdateHelpVersion")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::UpdateHelpVersion")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetHelpVersion(ByVal intIDPassport As Integer, ByVal oState As roWsState) As roGenericVtResponse(Of Integer)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Integer)
        Dim bolRet As Integer = 0

        Try
            bolRet = WLHelper.GetHelpVersion(intIDPassport, oState)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetHelpVersion")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetHelpVersion")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function SetLanguage(ByVal intIDPassport As Integer, ByVal strLanguageKey As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False

        Try
            Dim oPassportManager As New roPassportManager(bState)
            Dim oPassport As roPassportTicket = oPassportManager.LoadPassportTicket(intIDPassport, LoadType.Passport)
            If oPassport IsNot Nothing Then

                Dim oLangManager As New roLanguageManager
                Dim oLanguage As roPassportLanguage = oLangManager.LoadByKey(strLanguageKey)
                oPassportManager.UpdatePassportNameAndLanguage(intIDPassport, oPassport.Name, oLanguage.ID)

                bolRet = True

            End If
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::SetLanguage")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::SetLanguage")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

#Region "Informes de Emergencia anonimos"

    Public Shared Function GetEmergencyReportKey(ByVal oState As roWsState) As roGenericVtResponse(Of String)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of String)
        Dim strRet As String = String.Empty

        Try

            Dim oParameters As roParameters = New roParameters("OPTIONS")
            Dim oParams As New roCollection(oParameters.ParametersXML)
            Dim obj As Object = oParams.Item(oParameters.ParametersNames(Parameters.EmergencyReportKey))
            If Not obj Is Nothing Then
                strRet = roTypes.Any2String(obj)
                strRet = CryptographyHelper.Encrypt(strRet)
            End If
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetEmergencyReports")
        End Try

        oResult.Value = strRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function IsEmergencyReportActive(ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False

        Try

            Dim oParameters As roParameters = New roParameters("OPTIONS")
            Dim oParams As New roCollection(oParameters.ParametersXML)
            Dim obj As Object = oParams.Item(oParameters.ParametersNames(Parameters.EmergencyReportActive))
            If Not obj Is Nothing AndAlso roTypes.Any2String(obj) <> "" Then
                bolRet = True
            End If
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::IsEmergencyReportActive")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function IsValidPwd(ByVal strPwd As String, ByVal loggedInPassport As roPassportTicket, ByVal idPassport As Integer, ByVal ValidateHistory As Boolean, ByVal ActualPwd As String, ByVal oState As roWsState) As roGenericVtResponse(Of PasswordLevelError)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of PasswordLevelError)

        oResult.Value = roSecurityOptions.IsValidPwd(strPwd, loggedInPassport, idPassport, bState, ValidateHistory, ActualPwd)

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function ResetCache(ByVal strCompanyName As String, ByVal oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim newGState As New roWsState

        Try
            oResult.Value = Robotics.DataLayer.roCacheManager.GetInstance.RebootCompanyCache(strCompanyName)

            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.AnalyticsTask)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.ReportTaskDX)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.Import)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.BroadcasterTask)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.KeepAlive)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.SendEmail)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.SendNotifications)
            Robotics.Azure.RoAzureSupport.SendTaskToQueue(-999, strCompanyName, Robotics.Base.DTOs.roLiveTaskTypes.CacheControl)

            If (oResult.Value) Then
                roLog.GetInstance().logMessage(roLog.EventType.roInfo, "ResetCache started for company::" & strCompanyName)
            End If

            newGState.Result = 0
        Catch ex As Exception
            oResult.Value = True
            newGState.Result = 0
        Finally
        End Try

        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function GetVersionNotificationShown(intIDPassport As Integer, oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Integer = False

        Try
            bolRet = WLHelper.GetVersionNotificationShown(intIDPassport, oState)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::GetVersionNotificationShown")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::GetVersionNotificationShown")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

    Public Shared Function UpdateVersionNotification(intIDPassport As Integer, intNotificationVersion As Integer, oState As roWsState) As roGenericVtResponse(Of Boolean)
        Dim bState = New roSecurityState(-1)
        roWsStateManager.CopyTo(oState, bState)
        bState.UpdateStateInfo()

        Dim oResult As New roGenericVtResponse(Of Boolean)
        Dim bolRet As Boolean = False

        Try

            bolRet = WLHelper.UpdateNotificationVersion(intIDPassport, intNotificationVersion, oState)
        Catch ex As System.Data.Common.DbException
            bState.UpdateStateInfo(ex, "wscSecurity::UpdateVersionNotification")
        Catch ex As Exception
            bState.UpdateStateInfo(ex, "wscSecurity::UpdateVersionNotification")
        End Try

        oResult.Value = bolRet

        Dim newGState As New roWsState
        roWsStateManager.CopyTo(bState, newGState)
        oResult.Status = newGState

        Return oResult
    End Function

#End Region

End Class