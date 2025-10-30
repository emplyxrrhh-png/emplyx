Imports System.ComponentModel
Imports Robotics.DataLayer
Imports Robotics.ExternalSystems
Imports Robotics.ExternalSystems.DataLink
Imports Robotics.ExternalSystems.DataLink.RoboticsExternAccess
Imports VT_XU_Base
Imports VT_XU_Common
Imports Xunit

Namespace Unit.Test

    <Collection("Access")>
    <CollectionDefinition("Access", DisableParallelization:=True)>
    <Category("Access")>
    Public Class AccessTest

        Private ReadOnly helperDatalayer As DatalayerHelper
        Private ReadOnly helperAccess As AccessTestHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            helperDatalayer = New DatalayerHelper
            helperAccess = New AccessTestHelper
        End Sub

        <Fact(DisplayName:="Should Return Error If Terminal Not Exists")>
        Sub ShouldReturnErrorIfTerminalNotExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange                
                helperAccess.Initialize(helperDatalayer, 0)

                'Act
                Dim oDataExport As New roApiAccess
                Dim oTerminalConfiguration As roTerminalConfiguration
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetTerminalConfigurationById(oTerminalConfiguration, "1", strErrorMsg, returnCode)

                'Assert
                Assert.True(returnCode = returnCode._TerminalNotExist)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Error If Terminal Is Not Compatible")>
        Sub ShouldReturnErrorIfTerminalIsNotCompatible()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange 
                helperAccess.Initialize(helperDatalayer, 1)

                'Act
                Dim oDataExport As New roApiAccess
                Dim oTerminalConfiguration As roTerminalConfiguration
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetTerminalConfigurationById(oTerminalConfiguration, "1", strErrorMsg, returnCode)

                'Assert
                Assert.True(returnCode = returnCode._NotCompatibleTerminal)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Terminal Configuration If Terminal Exists And Is Compatible")>
        Sub ShouldReturnTerminalConfigurationIfTerminalExistsAndIsCompatible()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange  
                helperAccess.Initialize(helperDatalayer, 2)

                Fakes.ShimAccessHelper.CreateDataTableWithoutTimeoutsStringroBaseConnectionString = Function(ByVal strQuery As String, ByVal oConnection As roBaseConnection, ByVal strTableName As String) As DataTable
                                                                                                        Dim tableName As String = helperDatalayer.GetTableNameFromQuery(strQuery)
                                                                                                        If tableName = "Terminals" Then
                                                                                                            Return helperDatalayer.CreateDataTableMock({"Partners"}, New Object()() {New Object() {True}})
                                                                                                        End If
                                                                                                    End Function

                'Act
                Dim oDataExport As New roApiAccess
                Dim oTerminalConfiguration As roTerminalConfiguration
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetTerminalConfigurationById(oTerminalConfiguration, "1", strErrorMsg, returnCode)

                'Assert
                Assert.True(returnCode = returnCode._OK)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Authorized Users If Exist")>
        Sub ShouldReturnAuthorizedUsersIfExist()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange   
                helperAccess.Initialize(helperDatalayer, 3)

                'Act
                Dim oDataExport As New roApiAccess
                Dim oTerminalConfiguration As roTerminalConfiguration = New roTerminalConfiguration()
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetTerminalConfigurationById(oTerminalConfiguration, "1", strErrorMsg, returnCode)

                'Assert
                Assert.True(oTerminalConfiguration.AuthorizedEmployees.Count() = 1)

            End Using

        End Sub

        <Fact(DisplayName:="Should Return Access Periods If Exist")>
        Sub ShouldReturnAccessPeriodsIfExist()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange       
                helperAccess.Initialize(helperDatalayer, 4)

                'Act
                Dim oDataExport As New roApiAccess
                Dim oTerminalConfiguration As roTerminalConfiguration = New roTerminalConfiguration()
                Dim strErrorMsg As String
                Dim returnCode As DataLink.RoboticsExternAccess.Core.DTOs.ReturnCode
                oDataExport.GetTerminalConfigurationById(oTerminalConfiguration, "1", strErrorMsg, returnCode)

                'Assert
                Assert.True(oTerminalConfiguration.AccessPeriods.Count() = 1)

            End Using

        End Sub

    End Class

End Namespace