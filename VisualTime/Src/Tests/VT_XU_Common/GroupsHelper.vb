Imports Robotics.Base
Imports Robotics.DataLayer
Imports VT_XU_Base

Public Class GroupsHelper

    Public Property DeleteCommuniqueeGroupCalled As Boolean


    Public Sub Initialize(datalayerHelper As DatalayerHelper, workingMode As Integer)

        Fakes.ShimAccessHelper.CreateDataTableStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable

                                                                                 Dim tableName As String = datalayerHelper.GetTableNameFromQuery(strQuery)
                                                                                 If workingMode = 0 Then
                                                                                     If tableName = "Groups" Then
                                                                                         Return New DataTable()
                                                                                     ElseIf tableName = "employees" Then
                                                                                         Return New DataTable()
                                                                                     ElseIf tableName = "EmployeeGroups" Then
                                                                                         Return New DataTable()
                                                                                     End If
                                                                                 Else
                                                                                     If tableName = "Groups" Then
                                                                                         Return New DataTable()
                                                                                     ElseIf datalayerHelper.QueryContains(strQuery, "employees e JOIN EmployeeGroups eg ON EG.IDEmployee = E.ID JOIN Communiques") Then
                                                                                         Return datalayerHelper.CreateDataTableMock({"Id", "Name"}, New Object()() {New Object() {1, "Employee 1"}})
                                                                                     ElseIf datalayerHelper.QueryContains(strQuery, "employees e JOIN Surveys") Then
                                                                                         Return New DataTable()
                                                                                     ElseIf tableName = "EmployeeGroups" Then
                                                                                         Return New DataTable()
                                                                                     End If
                                                                                 End If


                                                                             End Function


        Fakes.ShimAccessHelper.ExecuteSqlStringroBaseConnection = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection) As Integer
                                                                      If strQuery.Contains("DELETE") AndAlso strQuery.Contains("CommuniqueGroups") Then
                                                                          DeleteCommuniqueeGroupCalled = True
                                                                      End If
                                                                      Return True
                                                                  End Function
    End Sub


    Public Sub GetGroupStub(idgroup As Integer)
        VTBusiness.Group.Fakes.ShimroGroup.GetGroupByNameStringroGroupStateStringString = Function(name As String, state As VTBusiness.Group.roGroupState, company As String, b As String)
                                                                                              Dim oGroup = New VTBusiness.Group.roGroup()
                                                                                              oGroup.ID = idgroup
                                                                                              Return oGroup
                                                                                          End Function
    End Sub
End Class