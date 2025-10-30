Imports Robotics.Base.VTBusiness
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports VT_XU_Base

Public Class AccessTestHelper
    Public Sub Initialize(datalayerHelper As DatalayerHelper, workingMode As Integer)

        Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                Dim tableName As String = datalayerHelper.GetTableNameFromQuery(strQuery)

                                                                                                If workingMode = 0 Then
                                                                                                    If tableName = "Terminals" Then
                                                                                                        Return datalayerHelper.CreateDataTableMock({"Partners"}, {})
                                                                                                    End If
                                                                                                ElseIf workingMode = 1 Then
                                                                                                    If tableName = "Terminals" Then
                                                                                                        Return datalayerHelper.CreateDataTableMock({"Partners"}, New Object()() {New Object() {False}})
                                                                                                    End If
                                                                                                ElseIf workingMode = 2 Then
                                                                                                    If tableName = "Terminals" Then
                                                                                                        Return datalayerHelper.CreateDataTableMock({"Partners"}, New Object()() {New Object() {True}})
                                                                                                    End If
                                                                                                ElseIf workingMode = 3 Then
                                                                                                    If tableName = "Terminals" Then
                                                                                                        Return datalayerHelper.CreateDataTableMock({"Partners"}, New Object()() {New Object() {True}})
                                                                                                    Else
                                                                                                        If tableName = "TerminalsSyncEmployeesData" Then
                                                                                                            Return datalayerHelper.CreateDataTableMock({"Value"}, New Object()() {New Object() {1}})
                                                                                                        End If
                                                                                                    End If
                                                                                                ElseIf workingMode = 4 Then
                                                                                                    If tableName = "Terminals" Then
                                                                                                        Return datalayerHelper.CreateDataTableMock({"Partners"}, New Object()() {New Object() {True}})
                                                                                                    Else
                                                                                                        If tableName = "TerminalsSyncTimeZonesData" Then
                                                                                                            Return datalayerHelper.CreateDataTableMock({"BeginTime", "EndTime", "DayOf"}, New Object()() {New Object() {"00:00", "23:59", 1}})
                                                                                                        End If
                                                                                                    End If
                                                                                                End If

                                                                                            End Function
    End Sub


    Public Sub InitZones()

        Zone.Fakes.ShimroZone.AllInstances.SupervisorGet = Function(instance) 1
        Zone.Fakes.ShimroZone.AllInstances.CapacityGet = Function(instance) 1
    End Sub
End Class