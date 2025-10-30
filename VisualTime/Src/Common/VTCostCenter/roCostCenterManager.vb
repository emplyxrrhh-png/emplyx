Imports System.Data.Common
Imports Robotics.Base.DTOs
Imports Robotics.Base.VTCostCenter.CostCenter
Imports Robotics.DataLayer.AccessHelper
Imports Robotics.VTBase
Imports Robotics.VTBase.Extensions.VTLiveTasks

Namespace CostCenter

    Public Class roCostCenterManager
        Private oState As roCostCenterManagerState = Nothing

        Public ReadOnly Property State As roCostCenterManagerState
            Get
                Return oState
            End Get
        End Property

        Public Sub New()
            Me.oState = New roCostCenterManagerState()
        End Sub

        Public Sub New(ByVal _State As roCostCenterManagerState)
            Me.oState = _State
        End Sub

#Region "Methods"
        Public Shared Function GetBusinessCenterBySecurityGroupDataTable(ByVal oState As roCostCenterManagerState, ByVal intIdSecurityGroup As Integer, ByVal bolCheckStatus As Boolean) As System.Data.DataTable

            Dim tbRet As DataTable = Nothing

            Try

                Dim strSQL As String = "@SELECT# * FROM BusinessCenters WHERE ID IN (@SELECT# DISTINCT IDCenter FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature =" & intIdSecurityGroup.ToString & ") "
                If bolCheckStatus Then
                    strSQL = strSQL & " AND Status=1"
                End If
                strSQL = strSQL & " Order by Name"
                tbRet = CreateDataTable(strSQL, )
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCostCenterManager::GetBusinessCenterBySecurityGroupDataTable")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCostCenterManager::GetBusinessCenterBySecurityGroupDataTable")
            Finally

            End Try

            Return tbRet

        End Function

        Public Shared Function SaveBusinessCenterBySecurityGroup(ByVal oState As roCostCenterManagerState, ByVal intIDSecurityGroup As Integer, ByVal intBusinessCenter() As Integer, Optional ByVal bAudit As Boolean = False) As Boolean
            Dim bolRet As Boolean = False

            Dim bHaveToClose As Boolean = False
            Try
                bHaveToClose = Robotics.DataLayer.AccessHelper.StartTransaction()

                'Borramos empleados y grupos asignados
                Dim DeleteQuerys() As String = {"@DELETE# FROM sysroSecurityGroupFeature_Centers WHERE IDGroupFeature = " & intIDSecurityGroup.ToString}
                For Each strSQLDelete As String In DeleteQuerys
                    bolRet = ExecuteSql(strSQLDelete)
                    If Not bolRet Then Exit For
                Next

                'Asignamos los nuevos centros de coste
                If bolRet Then
                    'Asignamos los nuevos grupos
                    Dim i As Integer = 0
                    For i = 0 To intBusinessCenter.Length - 1
                        bolRet = ExecuteSql("@INSERT# INTO sysroSecurityGroupFeature_Centers (IDGroupFeature, IDCenter) VALUES(" & intIDSecurityGroup.ToString & "," & intBusinessCenter(i).ToString & ")")
                        If Not bolRet Then Exit For
                    Next
                End If

                If bolRet Then
                    ' Lanzamos tarea de mapeo de permisos
                    Dim oStateTask As New roLiveTaskState(-1)

                    Dim oParameters As New roCollection
                    oParameters.Add("Context", 2)
                    oParameters.Add("IDContext", intIDSecurityGroup)
                    oParameters.Add("Action", -1)

                    roLiveTask.CreateLiveTask(roLiveTaskTypes.SecurityPermissions, oParameters, oStateTask)
                End If

                If bolRet And bAudit Then
                End If
            Catch ex As DbException
                oState.UpdateStateInfo(ex, "roCostCenterManager::SaveBusinessCenterBySecurityGroup")
            Catch ex As Exception
                oState.UpdateStateInfo(ex, "roCostCenterManager::SaveBusinessCenterBySecurityGroup")
            Finally
                Robotics.DataLayer.AccessHelper.EndCurrentTransaction(bHaveToClose, bolRet)
            End Try

            Return bolRet

        End Function

#End Region

    End Class

End Namespace