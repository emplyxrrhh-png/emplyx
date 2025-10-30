Imports System.ComponentModel
Imports System.Web.Mvc
Imports VT_XU_Base
Imports Xunit

Namespace Unit.Test

    <Collection("DocumentsController")>
    <CollectionDefinition("DocumentsController", DisableParallelization:=True)>
    <Category("DocumentsController")>
    Public Class DocumentsControllerTest

        Private ReadOnly documentsHelper As DocumentsHelper
        Private ReadOnly permissionsHelper As PermissionsHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            documentsHelper = New DocumentsHelper
            permissionsHelper = New PermissionsHelper

        End Sub

        <Fact(DisplayName:="Should load document if has permission over employee and template")>
        Public Sub ShouldLoadDocumentIfHasPermissionOverEmployeeAndTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(1)

                'Assert
                Assert.True(fileResult.FileDownloadName <> String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if permission over employee minimun not reached and has template permission")>
        Public Sub ShouldNotLoadDocumentIfPermissionOverEmployeeMinimunNotReachedAndHasTemplatePermission()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#0"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(1)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if has permission over employee and not on template")>
        Public Sub ShouldNotLoadDocumentIfHasPermissionOverEmployeeAndNotOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(2)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if has not permission over employee and has permission on template")>
        Public Sub ShouldNotLoadDocumentIfHasNotPermissionOverEmployeeAndHasPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(3)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if has not permission over employee and has not permission on template")>
        Public Sub ShouldNotLoadDocumentIfHasNotPermissionOverEmployeeAndHasNotPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(4)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should load document if has permission over group and template")>
        Public Sub ShouldLoadDocumentIfHasPermissionOverGroupAndTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(5)

                'Assert
                Assert.True(fileResult.FileDownloadName <> String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if permission over group minimun not requiered and has permission template")>
        Public Sub ShouldNotLoadDocumentIfPermissionOverGroupMinimunNotRequieredAndHasPermissionTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#0"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(5)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if has permission over group and not on template")>
        Public Sub ShouldNotLoadDocumentIfHasPermissionOverGroupAndNotOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(6)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if has not permission over group and has permission on template")>
        Public Sub ShouldNotLoadDocumentIfHasNotPermissionOverGroupAndHasPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(7)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should not load document if has not permission over group and has not permission on template")>
        Public Sub ShouldNotLoadDocumentIfHasNotPermissionOverGroupAndHasNotPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim fileResult As FileResult = documentsController.GetDocument(8)

                'Assert
                Assert.True(fileResult.FileDownloadName = String.Empty)

            End Using
        End Sub

        <Fact(DisplayName:="Should remove document if has permission over employee and template")>
        Public Sub ShouldRemoveDocumentIfHasPermissionOverEmployeeAndTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(1)

                'Assert
                Assert.True(actionResult.StatusCode = 200)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if permission over employee minimum not requiered and has template permission")>
        Public Sub ShouldNotRemoveDocumentIfPermissionOverEmployeeMinimumNotRequieredAndHasTemplatePermission()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#0"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(1)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if has permission over employee and not on template")>
        Public Sub ShouldNotRemoveDocumentIfHasPermissionOverEmployeeAndNotOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(2)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if has not permission over employee and has permission on template")>
        Public Sub ShouldNotRemoveDocumentIfHasNotPermissionOverEmployeeAndHasPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(3)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if has not permission over employee and has not permission on template")>
        Public Sub ShouldNotRemoveDocumentIfHasNotPermissionOverEmployeeAndHasNotPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(4)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should remove document if has permission over group and template")>
        Public Sub ShouldRemoveDocumentIfHasPermissionOverGroupAndTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(5)

                'Assert
                Assert.True(actionResult.StatusCode = 200)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if permission over group minimum not requiered and has template permission")>
        Public Sub ShouldNotRemoveDocumentIfPermissionOverGroupMinimumNotRequieredAndHasTemplatePermission()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#0"}, {"1#1", "2#2"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(5)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if has permission over group and not on template")>
        Public Sub ShouldNotRemoveDocumentIfHasPermissionOverGroupAndNotOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(6)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if has not permission over group and has permission on template")>
        Public Sub ShouldNotRemoveDocumentIfHasNotPermissionOverGroupAndHasPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(7)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

        <Fact(DisplayName:="Should not remove document if has not permission over group and has not permission on template")>
        Public Sub ShouldNotRemoveDocumentIfHasNotPermissionOverGroupAndHasNotPermissionOnTemplate()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1"}, {1})
                documentsHelper.SetAvailableDocuments(1, {"1#1#1#0", "2#2#1#0", "3#1#2#0", "4#2#2#0", "5#1#0#1", "6#2#0#1", "7#1#0#2", "8#2#0#2"}, {"1#1,2,3", "2#2"})

                'Act
                Dim documentsController As New VTLive40.DocumentaryManagementController()
                Dim actionResult As HttpStatusCodeResult = documentsController.RemoveDocument(8)

                'Assert
                Assert.True(actionResult.StatusCode = 500)

            End Using
        End Sub

    End Class

End Namespace