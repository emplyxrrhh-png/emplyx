Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions

Namespace Base

    Public Class roGroupFeaturePermissionsOverFeatureManager
        Private oState As roGroupFeaturePermissionOverFeatureState = Nothing

        Public Sub New()
            Me.oState = New roGroupFeaturePermissionOverFeatureState()
        End Sub

        Public Sub New(ByVal _State As roGroupFeaturePermissionOverFeatureState)
            Me.oState = _State
        End Sub

#Region "Methods"

        Public Function Load(ByVal _IDGroupFeature As Integer, ByVal _IDFeature As Integer, Optional ByVal bAudit As Boolean = False) As roGroupFeaturePermissionsOverFeature

            Dim bolRet As roGroupFeaturePermissionsOverFeature = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM sysroGroupFeatures_PermissionsOverFeatures " &
                                       "WHERE [IDGroupFeature] = " & _IDGroupFeature.ToString & " AND [IDFeature] = " & _IDFeature
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    bolRet = New roGroupFeaturePermissionsOverFeature()

                    Dim oRow As DataRow = tb.Rows(0)

                    If Not IsDBNull(oRow("IDGroupFeature")) Then bolRet.IDGroupFeature = oRow("IDGroupFeature")
                    If Not IsDBNull(oRow("IDFeature")) Then bolRet.IDFeature = oRow("IDFeature")
                    If Not IsDBNull(oRow("Permision")) Then bolRet.Permision = oRow("Permision")
                End If

                ' Auditar lectura
                If bAudit AndAlso bolRet IsNot Nothing Then
                    Dim tbParameters As DataTable = oState.CreateAuditParameters()
                    oState.AddAuditParameter(tbParameters, "{Name}", bolRet.IDGroupFeature & "-" & bolRet.IDFeature & "-" & bolRet.Permision, "", 1)
                    Me.oState.Audit(Audit.Action.aSelect, Audit.ObjectType.tGroupFeaturePermissionOverFeature, bolRet.IDGroupFeature & "-" & bolRet.IDFeature & "-" & bolRet.Permision, tbParameters, -1)
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Load")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Load")
            Finally

            End Try

            Return bolRet

        End Function

        Public Function Save(ByRef oGroupFeaturePermissionsOverFeature As roGroupFeaturePermissionsOverFeature, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = False

            Try
                Dim oAuditDataOld As DataRow = Nothing
                Dim oAuditDataNew As DataRow = Nothing

                Dim bolIsNew As Boolean = False

                Dim tb As New DataTable("sysroGroupFeatures_PermissionsOverFeatures")
                Dim strSQL As String = "@SELECT# * FROM sysroGroupFeatures_PermissionsOverFeatures " &
                                       "WHERE [IDGroupFeature] = " & oGroupFeaturePermissionsOverFeature.IDGroupFeature.ToString & " AND [IDFeature] = " & oGroupFeaturePermissionsOverFeature.IDFeature
                Dim cmd As DbCommand = CreateCommand(strSQL)
                Dim da As DbDataAdapter = CreateDataAdapter(cmd, True)
                da.Fill(tb)

                Dim oRow As DataRow
                If tb.Rows.Count = 0 Then
                    oRow = tb.NewRow
                    oRow("IDGroupFeature") = oGroupFeaturePermissionsOverFeature.IDGroupFeature
                    oRow("IDFeature") = oGroupFeaturePermissionsOverFeature.IDFeature

                    bolIsNew = True
                Else
                    oRow = tb.Rows(0)
                    oAuditDataOld = roAudit.CloneRow(oRow)
                End If

                If Me.Validate(oGroupFeaturePermissionsOverFeature, bolIsNew) Then
                    oRow("Permision") = oGroupFeaturePermissionsOverFeature.Permision

                    If tb.Rows.Count = 0 Then
                        tb.Rows.Add(oRow)
                    End If
                    da.Update(tb)

                    bolRet = True

                    If bolRet And bAudit Then
                        oAuditDataNew = oRow

                        bolRet = False
                        ' Auditamos
                        Dim tbAuditParameters As DataTable = roAudit.CreateParametersTable()
                        roAudit.AddFieldsValues(tbAuditParameters, oAuditDataNew, oAuditDataOld)
                        Dim oAuditAction As Audit.Action = IIf(oAuditDataOld Is Nothing, Audit.Action.aInsert, Audit.Action.aUpdate)
                        Dim strObjectName As String
                        If oAuditAction = Audit.Action.aInsert Then
                            strObjectName = oGroupFeaturePermissionsOverFeature.IDGroupFeature & "-" & oGroupFeaturePermissionsOverFeature.IDFeature & "-" & oGroupFeaturePermissionsOverFeature.Permision
                        Else
                            strObjectName = oGroupFeaturePermissionsOverFeature.IDGroupFeature & "-" & oGroupFeaturePermissionsOverFeature.IDFeature & "-" & oGroupFeaturePermissionsOverFeature.Permision
                        End If
                        bolRet = Me.oState.Audit(oAuditAction, Audit.ObjectType.tGroupFeaturePermissionOverFeature, strObjectName, tbAuditParameters, -1)
                    End If

                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Save")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Save")
            End Try

            Return bolRet

        End Function

        Public Function Delete(ByVal oGroupFeaturePermissionsOverFeature As roGroupFeaturePermissionsOverFeature, Optional ByVal bAudit As Boolean = False) As Boolean

            Dim bolRet As Boolean = True
            Try
                If bolRet Then
                    Dim DeleteQuerys() As String = {"@DELETE# FROM sysroGroupFeatures_PermissionsOverFeatures WHERE [IDGroupFeature] = " & oGroupFeaturePermissionsOverFeature.IDGroupFeature.ToString & " AND [IDFeature] = " & oGroupFeaturePermissionsOverFeature.IDFeature}
                    For Each strSQL As String In DeleteQuerys
                        bolRet = ExecuteSql(strSQL)
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet And bAudit Then
                    ' Auditamos
                    bolRet = Me.oState.Audit(Audit.Action.aDelete, Audit.ObjectType.tGroupFeaturePermissionOverFeature, oGroupFeaturePermissionsOverFeature.IDGroupFeature & "-" & oGroupFeaturePermissionsOverFeature.IDFeature & "-" & oGroupFeaturePermissionsOverFeature.Permision, Nothing, -1)
                End If
            Catch ex As DbException
                Me.oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Delete")
            Catch ex As Exception
                Me.oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Delete")
            End Try

            Return bolRet

        End Function

        Public Function Validate(ByVal oGroupFeaturePermissionsOverFeature As roGroupFeaturePermissionsOverFeature, ByVal bolIsNew As Boolean) As Boolean

            Dim bolRet As Boolean = True

            Try
                bolRet = True
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Validate")
                bolRet = False
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roGroupFeaturePermissionsOverFeatureManager::Validate")
                bolRet = False
            End Try

            Return bolRet

        End Function

#End Region

#Region "Helpers"

        Public Shared Function LoadByGroupFeature(ByVal iIDGroupFeature As Integer, ByRef oState As roGroupFeatureState) As roGroupFeaturePermissionsOverFeature()
            Dim bRet As roGroupFeaturePermissionsOverFeature() = Nothing

            Try

                Dim strSQL As String = "@SELECT# sysroGroupFeatures_PermissionsOverFeatures.* FROM sysroGroupFeatures_PermissionsOverFeatures, sysroFeatures " &
                                       "WHERE sysroGroupFeatures_PermissionsOverFeatures.IDFeature = sysroFeatures.ID AND IDGroupFeature = " & iIDGroupFeature.ToString &
                                       " AND sysroFeatures.Type Like 'U' Order by IsNull(sysroFeatures.IDParent,sysroFeatures.ID)"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                Dim tmpList As New Generic.List(Of roGroupFeaturePermissionsOverFeature)
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oLoader = New roGroupFeaturePermissionsOverFeatureManager()
                    For Each oRow As DataRow In tb.Rows
                        tmpList.Add(oLoader.Load(oRow("IDGroupFeature"), oRow("IDFeature"), False))
                    Next
                End If

                ' Obtenemos todas las funcionalidades que no tiene asignadas directamente un permiso y le ponemos por defecto sin permiso
                strSQL = "@SELECT# sysroFeatures.*  FROM sysroFeatures " &
                       "WHERE ID NOT IN (@SELECT# IDFeature FROM sysroGroupFeatures_PermissionsOverFeatures WHERE IDGroupFeature= " & iIDGroupFeature.ToString & " ) " &
                       " AND sysroFeatures.Type Like 'U' Order by IsNull(sysroFeatures.IDParent,sysroFeatures.ID)"
                tb = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    For Each oRow As DataRow In tb.Rows
                        Dim oLoader = New roGroupFeaturePermissionsOverFeature
                        oLoader.IDFeature = oRow("ID")
                        oLoader.IDGroupFeature = iIDGroupFeature
                        oLoader.Permision = 0

                        tmpList.Add(oLoader)
                    Next
                End If

                bRet = tmpList.ToArray
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeGroupManager::LoadByGroupFeature")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeGroupManager::LoadByGroupFeature")
            Finally

            End Try

            Return bRet
        End Function

        Public Shared Function GetAllFeatures(ByVal iIDGroupFeature As Integer, ByRef oState As roGroupFeatureState) As roGroupFeaturePermissionsOverFeature()
            Dim bRet As roGroupFeaturePermissionsOverFeature() = Nothing

            Try

                ' Obtenemos todas las funcionalidades de supervisor y le asignamos por defecto sin permiso
                Dim strSQL As String = "@SELECT# * FROM sysroFeatures " &
                                       "WHERE [Type] Like 'U' Order by IsNull(IDParent,ID)"
                Dim tb As DataTable = CreateDataTable(strSQL, )
                If tb IsNot Nothing AndAlso tb.Rows.Count > 0 Then
                    Dim oLoader = New roGroupFeaturePermissionsOverFeatureManager()
                    Dim tmpList As New Generic.List(Of roGroupFeaturePermissionsOverFeature)
                    For Each oRow As DataRow In tb.Rows
                        Dim oGroupFeaturePermissionsOverFeature As New roGroupFeaturePermissionsOverFeature
                        oGroupFeaturePermissionsOverFeature.IDGroupFeature = iIDGroupFeature
                        oGroupFeaturePermissionsOverFeature.IDFeature = oRow("ID")
                        oGroupFeaturePermissionsOverFeature.Permision = 0

                        tmpList.Add(oGroupFeaturePermissionsOverFeature)
                    Next
                    bRet = tmpList.ToArray
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeGroupManager::GetAllFeatures")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeGroupManager::GetAllFeatures")
            Finally

            End Try

            Return bRet
        End Function

        Public Shared Function DeleteByGroupFeature(ByVal iIDGroupFeature As Integer, ByRef oState As roGroupFeatureState) As Boolean
            Dim bRet As Boolean = True

            Try

                If bRet Then
                    Dim DeleteQuerys() As String = {"@DELETE# FROM sysroGroupFeatures_PermissionsOverFeatures WHERE [IDGroupFeature] = " & iIDGroupFeature.ToString}

                    For Each strSQL As String In DeleteQuerys
                        bRet = ExecuteSql(strSQL)
                        If Not bRet Then Exit For
                    Next
                End If
            Catch ex As Data.Common.DbException
                oState.UpdateStateInfo(ex, "roSecurityNodeGroupManager::DeleteByGroupFeature")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roSecurityNodeGroupManager::DeleteByGroupFeature")
            End Try

            Return bRet
        End Function

#End Region

    End Class

End Namespace