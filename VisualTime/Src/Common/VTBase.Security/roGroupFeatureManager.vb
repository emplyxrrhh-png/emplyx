Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace Base

    Public Class roGroupFeatureManager
        Private oState As roGroupFeatureState = Nothing

        Public Sub New()
            Me.oState = New roGroupFeatureState()
        End Sub

        Public Sub New(ByVal _State As roGroupFeatureState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal _ID As Integer, Optional ByVal bLoadRelated As Boolean = False, Optional ByVal bAudit As Boolean = False) As roGroupFeature

            Dim bolRet As roGroupFeature = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroGroupFeatures " &
                                       "WHERE [ID] = " & _ID.ToString
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    bolRet = New roGroupFeature()

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("Name")) Then bolRet.Name = oRow("Name")
                    If Not IsDBNull(oRow("ID")) Then bolRet.ID = oRow("ID")
                    If Not IsDBNull(oRow("Description")) Then bolRet.Description = oRow("Description")
                    If Not IsDBNull(oRow("BusinessGroupList")) AndAlso roTypes.Any2String(oRow("BusinessGroupList")).Trim() <> String.Empty Then bolRet.BusinessGroupList = roTypes.Any2String(oRow("BusinessGroupList")).Split(";")
                    If Not IsDBNull(oRow("ExternalId")) Then bolRet.ExternalId = oRow("ExternalId")

                    If bLoadRelated Then
                        bolRet.Features = roGroupFeaturePermissionsOverFeatureManager.LoadByGroupFeature(bolRet.ID, oState)
                        If bolRet.Features Is Nothing Then
                            bolRet.Features = roGroupFeaturePermissionsOverFeatureManager.GetAllFeatures(bolRet.ID, oState)
                        End If
                    End If

                End If

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", bolRet.Name, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGroupFeature, bolRet.Name, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroupFeatureManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupFeatureManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(ByRef oGroupFeature As roGroupFeature, Optional ByVal bSaveRelated As Boolean = False, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False
            Dim bHaveToClose As Boolean = False

            Try
                If Not DataLayer.roSupport.IsXSSSafe(oGroupFeature) Then
                    Me.oState.Result = GroupFeatureResultEnum.XSSvalidationError
                    Return False
                End If

                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("sysroGroupFeatures")
                Dim strSQL As String = "@SELECT# * FROM sysroGroupFeatures WHERE ID = " & oGroupFeature.ID.ToString
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oGroupFeature.ID = GetNextID(Me.oState)
                    oRow = tb.NewRow
                    oRow("ID") = oGroupFeature.ID

                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = roAudit.CloneRow(oRow)
                End If

                If Me.Validate(oGroupFeature, bolIsNew) Then
                    oRow("Name") = oGroupFeature.Name
                    oRow("Description") = oGroupFeature.Description
                    oRow("BusinessGroupList") = String.Join(";", oGroupFeature.BusinessGroupList)
                    oRow("ExternalId") = oGroupFeature.ExternalId

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)
                    bolRet = True

                    If bolRet AndAlso bSaveRelated Then
                        'Borramos las funcionalidades existentes
                        bolRet = roGroupFeaturePermissionsOverFeatureManager.DeleteByGroupFeature(oGroupFeature.ID, oState)

                        'Asignamos las nuevas funcionalidades a la función
                        If bolRet AndAlso oGroupFeature.Features IsNot Nothing Then
                            Dim oGroupFeaturePermissionsOverFeatureState As New roGroupFeaturePermissionOverFeatureState(oState.IDPassport)
                            Dim oGroupFeaturePermissionsOverFeatureManager As New roGroupFeaturePermissionsOverFeatureManager(oGroupFeaturePermissionsOverFeatureState)

                            For Each oFeature As roGroupFeaturePermissionsOverFeature In oGroupFeature.Features

                                If bolIsNew Then
                                    oFeature.IDGroupFeature = oGroupFeature.ID
                                End If

                                bolRet = oGroupFeaturePermissionsOverFeatureManager.Save(oFeature, bAudit)
                                If Not bolRet Then Exit For
                            Next
                        End If
                    End If

                    If bolRet And bAudit Then
                        oAuditDataNew = oRow

                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = roAudit.CreateParametersTable()
                        roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oAuditDataNew("Name")
                        Else
                            strObjectName = oAuditDataOld("Name") & " -> " & oAuditDataNew("Name")
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tGroupFeature, strObjectName, tbAuditParameters, -1)
                    End If

                    If bolRet And Not bolIsNew Then
                        ' Lanzamos tarea de mapeo de permisos
                        Dim oStateTask As New roLiveTaskState()

                        Dim oParameters As New roCollection
                        oParameters.Add("Context", 2)
                        oParameters.Add("IDContext", oGroupFeature.ID)
                        oParameters.Add("Action", -1)

                        roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oParameters, oStateTask)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroupFeatureManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroupFeatureManager::Save")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal oGroupFeature As roGroupFeature, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                If oGroupFeature.ID = 0 Then
                    Me.oState.Result = GroupFeatureResultEnum.ManagerGroupNotDelete
                    bolRet = False
                End If

                If bolRet Then
                    ' Validamos que no este asignado a ningun Supervisor en ningun nodo
                    If roTypes.Any2Double(ExecuteScalar("@SELECT# COUNT(*) FROM sysroPassports WHERE IsSupervisor = 1 AND IDGroupFeature =" & oGroupFeature.ID.ToString)) > 0 Then
                        bolRet = False
                        Me.oState.Result = GroupFeatureResultEnum.SupervisorAssigned
                    End If
                End If

                If bolRet Then
                    Dim DeleteQuerys() As String = {"@DELETE# FROM sysroGroupFeatures WHERE ID = " & oGroupFeature.ID,
                                                    "@DELETE# FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature = " & oGroupFeature.ID,
                                                    "@DELETE# FROM sysroGroupFeatures_PermissionsOverFeatures WHERE IDGroupFeature = " & oGroupFeature.ID.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tGroupFeature, oGroupFeature.Name, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroupFeatureManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroupFeatureManager::Delete")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oGroupFeature As roGroupFeature, ByVal bolIsNew As Boolean) As Boolean

            Dim bolRet As Boolean = True

            Try
                If oGroupFeature.Name.Trim = String.Empty Then
                    Me.oState.Result = GroupFeatureResultEnum.EmptyName
                    bolRet = False
                End If

                If bolRet Then
                    If roTypes.Any2Double(ExecuteScalar("@SELECT# count(*) from sysroGroupFeatures WHERE ID <> " & oGroupFeature.ID.ToString & " and Name Like '" & oGroupFeature.Name.Replace("'", "''") & "'")) > 0 Then
                        Me.oState.Result = GroupFeatureResultEnum.ExistSameName
                        bolRet = False
                    End If
                End If

                If oGroupFeature.ExternalId.Trim = String.Empty Then
                    Me.oState.Result = GroupFeatureResultEnum.EmptyExternalId
                    bolRet = False
                End If

                If bolRet Then
                    If roTypes.Any2Double(ExecuteScalar("@SELECT# count(*) from sysroGroupFeatures WHERE ID <> " & oGroupFeature.ID.ToString & " AND ExternalId Like '" & oGroupFeature.ExternalId.Replace("'", "''") & "'")) > 0 Then
                        Me.oState.Result = GroupFeatureResultEnum.ExistSameExternalId
                        bolRet = False
                    End If
                End If

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroupFeatureManager::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupFeatureManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

        Public Shared Function GetNextID(ByVal _State As roGroupFeatureState) As Integer

            Dim intRet As Integer = 0

            Try

                Dim strSQL As String = "@SELECT# Max(ID) AS Contador FROM sysroGroupFeatures "
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    If Not IsDBNull(tb.Rows(0).Item(0)) Then
                        intRet = tb.Rows(0).Item(0)
                    End If
                End If

                intRet += 1
            Catch ex As Data.Common.DbException
                _State.UpdateStateInfo(ex, "roGroupFeatureManager::GetNextID")
            Catch ex As Exception
                _State.UpdateStateInfo(ex, "roGroupFeatureManager::GetNextID")
            Finally

            End Try

            Return intRet

        End Function

#End Region

#Region "Helpers"

        Public Shared Function GetGroupFeaturesList(ByRef oState As roGroupFeatureState) As roGroupFeature()
            Dim bRet As roGroupFeature() = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID FROM sysroGroupFeatures WHERE Name not like '%@@ROBOTICS@@%' Order by Name"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oLoader = New roGroupFeatureManager()
                    Dim tmpList As New Generic.List(Of roGroupFeature)
                    For Each oRow As DataRow In tb.Rows
                        tmpList.Add(oLoader.Load(oRow("ID"), False, False))
                    Next
                    bRet = tmpList.ToArray
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::LoadBySecurityNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::LoadBySecurityNode")
            End Try

            Return bRet
        End Function

        Public Shared Function GetRoboticsGroupFeaturesList(ByRef oState As roGroupFeatureState) As roGroupFeature()
            Dim bRet As roGroupFeature() = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID FROM sysroGroupFeatures WHERE Name like '%@@ROBOTICS@@%' Order by ID"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oLoader = New roGroupFeatureManager()
                    Dim tmpList As New Generic.List(Of roGroupFeature)
                    For Each oRow As DataRow In tb.Rows
                        tmpList.Add(oLoader.Load(oRow("ID"), False, False))
                    Next
                    bRet = tmpList.ToArray
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::GetRoboticsGroupFeaturesList")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::GetRoboticsGroupFeaturesList")
            End Try

            Return bRet
        End Function

        Public Shared Function GetConsultantGroupFeature(ByRef oState As roGroupFeatureState) As roGroupFeature
            Dim bRet As roGroupFeature = Nothing

            Try

                Dim strSQL As String = "@SELECT# ID FROM sysroGroupFeatures WHERE Name like '@@ROBOTICS@@Consultores' Order by ID"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oLoader = New roGroupFeatureManager()
                    Dim tmpList As New Generic.List(Of roGroupFeature)
                    bRet = oLoader.Load(tb.Rows(0)("ID"), False, False)

                    If bRet IsNot Nothing Then bRet.Name = oState.Language.Translate("Consultant.Name", "GroupFeature")
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::GetConsultantGroupFeature")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::GetConsultantGroupFeature")
            End Try

            Return bRet
        End Function

        Public Shared Function SetGroupFeaturePermission(ByVal iGroupFeatureID As Integer, ByVal iFeatureID As Integer, ByVal iPermission As Integer, ByRef oState As roGroupFeatureState, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True
            Dim bHaveToClose As Boolean = False

            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()
                Dim sGroupFeatureName As String = roTypes.Any2String(ExecuteScalar("@SELECT# Name from sysroGroupFeatures where ID = " & iGroupFeatureID))

                Dim strSQL As String = "@SELECT# ID,Alias,PermissionTypes from sysroFeatures where Type = 'U' AND Alias like (@SELECT# isNULL(Alias,'') + '.%' from sysroFeatures where ID =" & iFeatureID & ")"

                'Creamos los permisos para todos los hijos.
                Dim dtFeatures As DataTable = CreateDataTable(strSQL)
                If dtFeatures IsNot Nothing AndAlso dtFeatures.Rows.Count > 0 Then
                    For Each oRow As DataRow In dtFeatures.Rows
                        Dim iChildIdFeature As Integer = roTypes.Any2Integer(oRow("ID"))
                        Dim strPermissionType As String = roTypes.Any2String(oRow("PermissionTypes"))

                        Dim maxConfigurable As Integer = iPermission

                        If Not strPermissionType.Contains("A") AndAlso maxConfigurable > 6 Then
                            maxConfigurable = 6
                        End If

                        If Not strPermissionType.Contains("A") AndAlso Not strPermissionType.Contains("W") AndAlso maxConfigurable > 3 Then
                            maxConfigurable = 3
                        End If

                        If Not strPermissionType.Contains("A") AndAlso Not strPermissionType.Contains("W") AndAlso Not strPermissionType.Contains("R") AndAlso maxConfigurable > 0 Then
                            maxConfigurable = 0
                        End If

                        Dim actualPermission As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# Permision from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature))
                        Dim featureSQL As String = "if exists (@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & ") " &
                                                        " @UPDATE# sysroGroupFeatures_PermissionsOverFeatures set Permision = " & maxConfigurable & " where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & " " &
                                                    " else" &
                                                        " @INSERT# INTO sysroGroupFeatures_PermissionsOverFeatures(Permision,IDGroupFeature,IDFeature) values(" & maxConfigurable & "," & iGroupFeatureID & "," & iChildIdFeature & ") "

                        bolRet = ExecuteSql(featureSQL)

                        If bolRet AndAlso actualPermission <> maxConfigurable Then
                            roGroupFeatureManager.AuditPermissionChange(sGroupFeatureName, oRow("Alias"), maxConfigurable, oState)
                        End If

                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    'Tenemos que modificar los permisos para todos los nodos padre
                    strSQL = "@SELECT# Alias from sysroFeatures where Type = 'U' AND ID =" & iFeatureID
                    Dim strAlias As String = roTypes.Any2String(ExecuteScalar(strSQL))

                    While strAlias.IndexOf(".") > 0

                        strSQL = "@SELECT# ID,PermissionTypes from sysroFeatures where Type = 'U' AND Alias ='" & strAlias & "'"

                        dtFeatures = CreateDataTable(strSQL)
                        If dtFeatures IsNot Nothing AndAlso dtFeatures.Rows.Count > 0 Then
                            For Each oRow As DataRow In dtFeatures.Rows
                                Dim iChildIdFeature As Integer = roTypes.Any2Integer(oRow("ID"))
                                Dim strPermissionType As String = roTypes.Any2String(oRow("PermissionTypes"))

                                Dim maxConfigurable As Integer = iPermission

                                If Not strPermissionType.Contains("A") AndAlso maxConfigurable > 6 Then
                                    maxConfigurable = 6
                                End If

                                If Not strPermissionType.Contains("A") AndAlso Not strPermissionType.Contains("W") AndAlso maxConfigurable > 3 Then
                                    maxConfigurable = 3
                                End If

                                If Not strPermissionType.Contains("A") AndAlso Not strPermissionType.Contains("W") AndAlso Not strPermissionType.Contains("R") AndAlso maxConfigurable > 0 Then
                                    maxConfigurable = 0
                                End If

                                Dim featureSQL As String = ""
                                If iFeatureID <> iChildIdFeature Then
                                    ' Si es su padre
                                    featureSQL = "if exists (@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & ") " &
                                                                " @UPDATE# sysroGroupFeatures_PermissionsOverFeatures set Permision = " & maxConfigurable & " where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & " AND Permision < " & maxConfigurable &
                                                                " else " &
                                                                " @INSERT# INTO sysroGroupFeatures_PermissionsOverFeatures(Permision,IDGroupFeature,IDFeature) values(" & maxConfigurable & "," & iGroupFeatureID & "," & iChildIdFeature & ") "
                                Else
                                    ' Si es el mismo
                                    featureSQL = "if exists (@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & ") " &
                                                " @UPDATE# sysroGroupFeatures_PermissionsOverFeatures set Permision = " & maxConfigurable & " where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature &
                                                " else " &
                                                    " @INSERT# INTO sysroGroupFeatures_PermissionsOverFeatures(Permision,IDGroupFeature,IDFeature) values(" & maxConfigurable & "," & iGroupFeatureID & "," & iChildIdFeature & ") "
                                End If

                                If featureSQL <> String.Empty Then
                                    Dim actualPermission As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature))

                                    bolRet = ExecuteSql(featureSQL)

                                    If bolRet AndAlso actualPermission <> maxConfigurable Then
                                        roGroupFeatureManager.AuditPermissionChange(sGroupFeatureName, strAlias, maxConfigurable, oState)
                                    End If
                                End If



                                If Not bolRet Then Exit For
                            Next
                        End If

                        strAlias = strAlias.Substring(0, strAlias.LastIndexOf("."))

                    End While

                    If bolRet Then
                        strSQL = "@SELECT# ID,PermissionTypes from sysroFeatures where Type = 'U' AND Alias ='" & strAlias & "'"

                        dtFeatures = CreateDataTable(strSQL)
                        If dtFeatures IsNot Nothing AndAlso dtFeatures.Rows.Count > 0 Then
                            For Each oRow As DataRow In dtFeatures.Rows
                                Dim iChildIdFeature As Integer = roTypes.Any2Integer(oRow("ID"))
                                Dim strPermissionType As String = roTypes.Any2String(oRow("PermissionTypes"))

                                Dim maxConfigurable As Integer = iPermission

                                If Not strPermissionType.Contains("A") AndAlso maxConfigurable > 6 Then
                                    maxConfigurable = 6
                                End If

                                If Not strPermissionType.Contains("A") AndAlso Not strPermissionType.Contains("W") AndAlso maxConfigurable > 3 Then
                                    maxConfigurable = 3
                                End If

                                If Not strPermissionType.Contains("A") AndAlso Not strPermissionType.Contains("W") AndAlso Not strPermissionType.Contains("R") AndAlso maxConfigurable > 0 Then
                                    maxConfigurable = 0
                                End If

                                Dim featureSQL As String = ""
                                If iFeatureID <> iChildIdFeature Then
                                    ' Si es un padre
                                    featureSQL = "if exists (@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & ") " &
                                                    " @UPDATE# sysroGroupFeatures_PermissionsOverFeatures set Permision = " & maxConfigurable & " where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & " AND Permision < " & maxConfigurable & " " &
                                                    " else " &
                                                    " @INSERT# INTO sysroGroupFeatures_PermissionsOverFeatures(Permision,IDGroupFeature,IDFeature) values(" & maxConfigurable & "," & iGroupFeatureID & "," & iChildIdFeature & ") "
                                Else
                                    ' Si es el mismo
                                    featureSQL = "if exists (@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & ") " &
                                                " @UPDATE# sysroGroupFeatures_PermissionsOverFeatures set Permision = " & maxConfigurable & " where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature & " " &
                                                " else " &
                                                " @INSERT# INTO sysroGroupFeatures_PermissionsOverFeatures(Permision,IDGroupFeature,IDFeature) values(" & maxConfigurable & "," & iGroupFeatureID & "," & iChildIdFeature & ") "
                                End If

                                If featureSQL <> String.Empty Then
                                    Dim actualPermission As Integer = roTypes.Any2Integer(ExecuteScalar("@SELECT# * from sysroGroupFeatures_PermissionsOverFeatures where IDGroupFeature = " & iGroupFeatureID & " and IDFeature = " & iChildIdFeature))

                                    bolRet = ExecuteSql(featureSQL)
                                    If bolRet AndAlso actualPermission <> maxConfigurable Then
                                        roGroupFeatureManager.AuditPermissionChange(sGroupFeatureName, strAlias, maxConfigurable, oState)
                                    End If
                                End If

                                If Not bolRet Then Exit For
                            Next
                        End If
                    End If

                End If

                If bolRet Then
                    ' Lanzamos tarea de mapeo de permisos
                    Dim oStateTask As New roLiveTaskState(-1)

                    Dim oParameters As New roCollection
                    oParameters.Add("Context", 2)
                    oParameters.Add("IDContext", iGroupFeatureID)
                    oParameters.Add("Action", -1)

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oParameters, oStateTask)
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupFeatureManager::SetGroupFeaturePermission")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupFeatureManager::SetGroupFeaturePermission")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

        Public Shared Function CopyGroupFeature(ByVal iGroupFeatureID As Integer, ByRef oState As roGroupFeatureState) As Boolean
            Dim bRet As Boolean = True

            Try

                Dim oManager As New roGroupFeatureManager(oState)

                Dim oFeature As roGroupFeature = oManager.Load(iGroupFeatureID, True)

                oFeature.Name = "* " & oFeature.Name
                oFeature.ID = -1

                bRet = oManager.Save(oFeature, True, True)

                If bRet Then
                    Dim strSQL As String = "@INSERT# INTO sysroSecurityGroupFeature_Centers(IDGroupFeature,IDCenter) @SELECT# " & oFeature.ID.ToString & ", IDCenter FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature=" & iGroupFeatureID.ToString
                    bRet = ExecuteSqlWithoutTimeOut(strSQL)
                End If

            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::LoadBySecurityNode")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeManager::LoadBySecurityNode")
            End Try

            Return bRet
        End Function

#End Region

        Private Shared Sub AuditPermissionChange(ByVal sGroupFeature As String, ByVal strFeature As String, ByVal iPermission As Permission, ByVal oState As roGroupFeatureState)
            Try
                ' Auditoría de consulta de datos

                Dim lstAuditParameterNames As New List(Of String)
                Dim lstAuditParameterValues As New List(Of String)

                lstAuditParameterNames.Add("{sFeature}")
                lstAuditParameterValues.Add(strFeature.ToString)

                lstAuditParameterNames.Add("{sPermission}")
                lstAuditParameterValues.Add(iPermission.ToString)

                If oState IsNot Nothing Then

                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    If lstAuditParameterNames.Count > 0 Then
                        For i As Integer = 0 To lstAuditParameterNames.Count - 1
                            oState.AddAuditParameter(tbParameters, lstAuditParameterNames(i), lstAuditParameterValues(i), "", 1)
                        Next
                    End If

                    oState.Audit(Robotics.VTBase.Audit.Action.aExecuted, Robotics.VTBase.Audit.ObjectType.tGroupFeaturePermissionOverFeature, sGroupFeature, tbParameters, -1)
                End If
            Catch
            End Try

        End Sub

        ''' <summary>
        ''' Devuelve la lista de ids externas
        ''' </summary>
        ''' <param name="oState"></param>
        ''' <returns></returns>
        ''' <remarks></remarks>
        Public Shared Function GetAllExternalIds(ByVal oState As roGroupFeatureState) As DataTable

            Dim oRet As DataTable = Nothing

            Try

                Dim strSQL As String = $"@SELECT# [ExternalId] FROM sysroGroupFeatures"

                oRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roGroupFeature::GetAllExternalIds")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupFeature::GetAllExternalIds")
            End Try

            Return oRet

        End Function
    End Class

End Namespace