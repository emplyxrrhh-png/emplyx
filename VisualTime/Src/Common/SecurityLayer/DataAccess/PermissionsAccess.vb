Imports System.Data.Common
Imports System.Security
Imports System.Web.UI.HtmlControls
Imports System.Windows.Forms
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace DataAccess

    Friend NotInheritable Class PermissionsAccess

        Public Shared Function PermissionsOverFeatures_Get(ByVal idPassport As Integer, ByVal featureAlias As String, ByVal featureType As String, ByVal mode As Integer) As Integer
            Dim iPermission As Integer = 0
            Try
                Dim sqlCommand As String = String.Empty
                sqlCommand = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures " &
                                 " WHERE IdPassport = @idPassport AND FeatureAlias = @featureAlias AND FeatureType = @featureType "
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@featureAlias", CommandParameter.ParameterType.tString, featureAlias))
                parameters.Add(New CommandParameter("@featureType", CommandParameter.ParameterType.tString, featureType))

                iPermission = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))

                Return iPermission
            Finally
            End Try
        End Function

        Public Shared Function PermissionsOverFeatures_Get_Sys(ByVal idPassport As Integer, ByVal featureAlias As String, ByVal featureType As String, ByVal mode As Integer) As Integer
            Dim iPermission As Integer
            Try
                Select Case mode
                    Case 2 'Permisos heredados
                        iPermission = 0
                    Case Else
                        Dim sqlCommand As String = String.Empty
                        sqlCommand = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures " &
                                         " WHERE IdPassport = @idPassport AND FeatureAlias = @featureAlias AND FeatureType = @featureType "
                        Dim parameters As New List(Of CommandParameter)
                        parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                        parameters.Add(New CommandParameter("@featureAlias", CommandParameter.ParameterType.tString, featureAlias))
                        parameters.Add(New CommandParameter("@featureType", CommandParameter.ParameterType.tString, featureType))

                        iPermission = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                End Select
            Finally
            End Try
            Return iPermission
        End Function

        Public Shared Sub PermissionsOverFeatures_Remove(ByVal idPassport As Integer, ByVal featureAlias As String, ByVal featureType As String, Optional ByVal bolInitTask As Boolean = True)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_PermissionsOverFeatures_Remove")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32).Value = idPassport
                AccessHelper.AddParameter(Command, "@featureAlias", DbType.String, 100).Value = featureAlias
                AccessHelper.AddParameter(Command, "@featureType", DbType.AnsiString, 1).Value = featureType
                Command.ExecuteNonQuery()

                If bolInitTask Then
                    Dim oStateTask As New roLiveTaskState(-1)
                    Dim oParameters As New roCollection
                    oParameters.Add("Mode", "")
                    roLiveTask.CreateLiveTask(roLiveTaskTypes.ChangeRequestPermissions, oParameters, oStateTask)
                End If
            Finally

            End Try
        End Sub

        Public Shared Sub PermissionsOverFeatures_Set(ByVal idPassport As Integer, ByVal featureAlias As String, ByVal featureType As String, ByVal permission As Integer, Optional ByVal bolInitTask As Boolean = True)

            Try

                Dim Command As DbCommand = AccessHelper.CreateCommand("WebLogin_PermissionsOverFeatures_Set")
                Command.CommandType = CommandType.StoredProcedure
                AccessHelper.AddParameter(Command, "@idPassport", DbType.Int32).Value = idPassport
                AccessHelper.AddParameter(Command, "@featureAlias", DbType.String, 100).Value = featureAlias
                AccessHelper.AddParameter(Command, "@featureType", DbType.AnsiString, 1).Value = featureType
                AccessHelper.AddParameter(Command, "@permission", DbType.Byte).Value = permission
                Command.ExecuteNonQuery()

                If bolInitTask Then
                    Dim oStateTask As New roLiveTaskState(-1)
                    Dim oParameters As New roCollection
                    oParameters.Add("Mode", "")
                    roLiveTask.CreateLiveTask(roLiveTaskTypes.ChangeRequestPermissions, oParameters, oStateTask)
                End If
            Finally

            End Try
        End Sub

        Public Shared Function PermissionsOverFeatures_MaxConfigurable(ByVal idPassport As Integer, ByVal featureAlias As String, ByVal featureType As String) As Integer
            Dim iPermission As Integer
            Try
                Dim sqlCommand As String = String.Empty
                If featureType = "E" Then
                    iPermission = 9
                Else
                    sqlCommand = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverFeatures " &
                                     " WHERE IdPassport = @idPassport AND FeatureAlias = @featureAlias AND FeatureType = @featureType "
                    Dim parameters As New List(Of CommandParameter)
                    parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                    parameters.Add(New CommandParameter("@featureAlias", CommandParameter.ParameterType.tString, featureAlias))
                    parameters.Add(New CommandParameter("@featureType", CommandParameter.ParameterType.tString, featureType))

                    iPermission = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                End If

            Finally
            End Try
            Return iPermission
        End Function

        Public Shared Function PermissionsOverEmployees_Get(ByVal idPassport As Integer, ByVal idEmployee As Integer, ByVal idFeature As Integer, ByVal mode As Integer, ByVal includeGroups As Boolean) As Integer
            Dim iPermission As Integer
            Try
                Dim sqlCommand As String = String.Empty
                sqlCommand = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverEmployees " &
                                 " INNER JOIN sysrovwSecurity_PermissionOverFeatures ON sysrovwSecurity_PermissionOverFeatures.IdPassport = sysrovwSecurity_PermissionOverEmployees.IdPassport " &
                                 " WHERE sysrovwSecurity_PermissionOverEmployees.IdPassport = @idPassport AND sysrovwSecurity_PermissionOverEmployees.IdEmployee = @idEmployee AND  CONVERT(date,GETDATE()) BETWEEN sysrovwSecurity_PermissionOverEmployees.BeginDate AND sysrovwSecurity_PermissionOverEmployees.EndDate " &
                                 " AND sysrovwSecurity_PermissionOverFeatures.IdFeature = @idFeature"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@idEmployee", CommandParameter.ParameterType.tInt, idEmployee))
                parameters.Add(New CommandParameter("@idFeature", CommandParameter.ParameterType.tInt, idFeature))

                iPermission = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                'Security. Antes esta función como mucho devolvía 6, porque no se podía tener administración sobre departamentos
                If IPermission = 9 Then IPermission = 6
            Finally
            End Try
            Return iPermission
        End Function

        Public Shared Function PermissionsOverEmployees_Get_Extended(ByVal idPassport As Integer, ByVal idEmployee As Integer, ByVal idFeature As Integer, ByVal mode As Integer, ByVal includeGroups As Boolean, ByVal dDate As DateTime) As Integer
            Dim iPermission As Integer
            Try

                Dim sqlCommand As String = String.Empty
                sqlCommand = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverEmployees " &
                                 " INNER JOIN sysrovwSecurity_PermissionOverFeatures ON sysrovwSecurity_PermissionOverFeatures.IdPassport = sysrovwSecurity_PermissionOverEmployees.IdPassport " &
                                 " WHERE sysrovwSecurity_PermissionOverEmployees.IdPassport = @idPassport AND sysrovwSecurity_PermissionOverEmployees.IdEmployee = @idEmployee AND CONVERT(DATE,@date) BETWEEN sysrovwSecurity_PermissionOverEmployees.BeginDate AND sysrovwSecurity_PermissionOverEmployees.EndDate " &
                                 " AND sysrovwSecurity_PermissionOverFeatures.IdFeature = @idFeature"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@idEmployee", CommandParameter.ParameterType.tInt, idEmployee))
                parameters.Add(New CommandParameter("@idFeature", CommandParameter.ParameterType.tInt, idFeature))
                parameters.Add(New CommandParameter("@date", CommandParameter.ParameterType.tDateTime, dDate))

                iPermission = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                'TODO: Security. Antes esta función como mucho devolvía 6, porque no se podía tener administración sobre departamentos
                If iPermission = 9 Then iPermission = 6
            Finally
            End Try
            Return iPermission
        End Function

        Public Shared Function PermissionsOverGroups_Get(ByVal idPassport As Integer, ByVal idGroup As Integer, ByVal idFeature As Integer, ByVal mode As Integer) As Integer
            Dim iPermission As Integer
            Try
                Dim sqlCommand As String = String.Empty
                sqlCommand = "@SELECT# Permission FROM sysrovwSecurity_PermissionOverGroups " &
                                 " INNER JOIN sysrovwSecurity_PermissionOverFeatures ON sysrovwSecurity_PermissionOverFeatures.IdPassport = sysrovwSecurity_PermissionOverGroups.IdPassport " &
                                 " WHERE sysrovwSecurity_PermissionOverGroups.IdPassport = @idPassport AND IdGroup = @idGroup AND GETDATE() BETWEEN sysrovwSecurity_PermissionOverGroups.BeginDate AND sysrovwSecurity_PermissionOverGroups.EndDate " &
                                 " AND sysrovwSecurity_PermissionOverFeatures.IdFeature = @idFeature"
                Dim parameters As New List(Of CommandParameter)
                parameters.Add(New CommandParameter("@idPassport", CommandParameter.ParameterType.tInt, idPassport))
                parameters.Add(New CommandParameter("@idGroup", CommandParameter.ParameterType.tInt, idGroup))
                parameters.Add(New CommandParameter("@idFeature", CommandParameter.ParameterType.tInt, idFeature))

                iPermission = roTypes.Any2Integer(AccessHelper.ExecuteScalar(sqlCommand, parameters))
                'TODO: Security. Antes esta función como mucho devolvía 6, porque no se podía tener administración sobre departamentos
                If iPermission = 9 Then iPermission = 6
            Finally
            End Try

            Return iPermission
        End Function

    End Class

End Namespace