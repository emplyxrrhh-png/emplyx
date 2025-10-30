Imports System.ComponentModel
Imports System.Web.Mvc
Imports VT_XU_Base
Imports Xunit

Namespace Unit.Test

    <Collection("EmployeeController")>
    <CollectionDefinition("EmployeeController", DisableParallelization:=True)>
    <Category("EmployeeController")>
    Public Class EmployeeControllerTest

        Private ReadOnly permissionsHelper As PermissionsHelper
        Private ReadOnly employeeHelper As EmployeesHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            permissionsHelper = New PermissionsHelper
            employeeHelper = New EmployeesHelper

        End Sub

        <Fact(DisplayName:="Should get employee photo if has permissions over employee and feature")>
        Public Sub ShouldGetEmployeePhotoIfHasPermissionsOverEmployeeAndFeature()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3", "Employees.NameFoto#U#3"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeePhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.True(employeeHelper.EmployeeLoaded)
            End Using

        End Sub

        <Fact(DisplayName:="Should get default employee photo if has permissions over employee and not over feature")>
        Public Sub ShouldGetDefaultEmployeePhotoIfHasPermissionsOverEmployeeAndNotOverFeature()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3", "Employees.NameFoto#U#0"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeePhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using

        End Sub

        <Fact(DisplayName:="Should not get employee photo if permission over employee not valid")>
        Public Sub ShouldNotGetEmployeePhotoIfPermissionOverEmployeeNotValid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.InitAvailablePassports({"1#1", "2#2"})
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#1"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeePhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(HttpNotFoundResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get employee photo if has not permissions over employee")>
        Public Sub ShouldNotGetEmployeePhotoIfHasNotPermissionsOverEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeePhoto(2)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(HttpNotFoundResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should get supervisor photo if has permissions over related employee and feature")>
        Public Sub ShouldGetSupervisorPhotoIfHasPermissionsOverRelatedEmployeeAndFeature()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.InitAvailablePassports({"1#1"})
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3", "Employees.NameFoto#U#6"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetSupervisorPhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.True(employeeHelper.EmployeeLoaded)
            End Using

        End Sub

        <Fact(DisplayName:="Should get supervisor photo if requester is the same supervisor")>
        Public Sub ShouldGetSupervisorPhotoIfRequesterIsTheSameSupervisor()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.InitAvailablePassports({"1#1"})
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#1", "Employees.NameFoto#U#1"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetSupervisorPhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.True(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should get supervisor default photo if has permissions over related employee and has not feature permission")>
        Public Sub ShouldGetSupervisorDefaultPhotoIfHasPermissionsOverRelatedEmployeeAndHasNotFeaturePermission()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.InitAvailablePassports({"1#1", "2#2"})
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3", "Employees.NameFoto#U#1"}, {"1#1", "2#2"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetSupervisorPhoto(2)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get supervisor photo if permissions over related employee are not valid")>
        Public Sub ShouldNotGetSupervisorPhotoIfPermissionsOverRelatedEmployeeAreNotValid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.InitAvailablePassports({"1#1", "2#2"})
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#1"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetSupervisorPhoto(2)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(HttpNotFoundResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get supervisor photo if has not permissions over related employee")>
        Public Sub ShouldNotGetSupervisorPhotoIfHasNotPermissionsOverRelatedEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.InitAvailablePassports({"1#1"})
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetSupervisorPhoto(2)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(HttpNotFoundResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should get employee large photo if has permissions over employee and feature")>
        Public Sub ShouldGetEmployeeLargePhotoIfHasPermissionsOverEmployeeAndFeature()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3", "Employees.NameFoto#U#3"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeeLargePhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.True(employeeHelper.EmployeeLoaded)
            End Using

        End Sub

        <Fact(DisplayName:="Should get default employee large photo if has permissions over employee and not over feature")>
        Public Sub ShouldGetDefaultEmployeeLargePhotoIfHasPermissionsOverEmployeeAndNotOverFeature()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3", "Employees.NameFoto#U#1"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeeLargePhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(FileContentResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using

        End Sub

        <Fact(DisplayName:="Should not get employee large photo if permission over employee not valid")>
        Public Sub ShouldNotGetEmployeeLargePhotoIfPermissionOverEmployeeNotValid()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#1"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeeLargePhoto(1)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(HttpNotFoundResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

        <Fact(DisplayName:="Should not get employee large photo if has not permissions over employee")>
        Public Sub ShouldNotGetEmployeeLargePhotoIfHasNotPermissionsOverEmployee()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                employeeHelper.InitializeEmployeeServiceMethods()

                Dim employeeController As New VTLive40.EmployeeController()

                'Act
                Dim result As ActionResult = employeeController.GetEmployeeLargePhoto(2)

                'Assert
                Assert.NotNull(result)
                Assert.IsType(GetType(HttpNotFoundResult), result)
                Assert.False(employeeHelper.EmployeeLoaded)
            End Using
        End Sub

    End Class

End Namespace