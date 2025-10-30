Imports System.ComponentModel
Imports System.Web.Mvc
Imports System.Web.WebPages
Imports DevExtreme.AspNet.Mvc
Imports Robotics.Base.DTOs
Imports Robotics.DataLayer
Imports Robotics.Web.Base
Imports VT_XU_Base
Imports Xunit


Namespace Unit.Test

    <Collection("GeniusController")>
    <CollectionDefinition("GeniusController", DisableParallelization:=True)>
    <Category("GeniusController")>
    Public Class GeniusControllerTest

        Private ReadOnly geniusHelper As GeniusHelper
        Private ReadOnly permissionsHelper As PermissionsHelper
        Private ReadOnly baseHelper As BaseHelper
        Private ReadOnly dataLayerHelper As DatalayerHelper
        Private ReadOnly azureHelper As AzureHelper
        Private ReadOnly webHelper As WebHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            geniusHelper = New GeniusHelper
            permissionsHelper = New PermissionsHelper
            baseHelper = New BaseHelper
            dataLayerHelper = New DatalayerHelper
            azureHelper = New AzureHelper
            webHelper = New WebHelper
        End Sub

        <Fact(DisplayName:="Should get a genius view when passport is owner")>
        Public Sub ShouldGetAGeniusViewWhenPassportIsOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.NotNull(geniusController.GetGeniusView(3).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should get a genius view when view is common")>
        Public Sub ShouldGetAGeniusViewWhenViewIsCommon()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.NotNull(geniusController.GetGeniusView(4).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not get a genius view when passport is not owner")>
        Public Sub ShouldNotGetAGeniusViewWhenPassportIsNotOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange  )
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.Null(geniusController.GetGeniusView(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should get a planning list when is view owner")>
        Public Sub ShouldGetAAPlanningListWhenIsViewCommon()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLoadOptions As New DataSourceLoadOptions
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetPlanningListForGeniusViews(1, {1, 3}, {1, 2}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.NotNull(geniusController.GetPlanningList(oLoadOptions, 1))
            End Using
        End Sub

        <Fact(DisplayName:="Should not get a planning list when is not view owner")>
        Public Sub ShouldNotGetAPlanningListWhenIsNotViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLoadOptions As New DataSourceLoadOptions
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetPlanningListForGeniusViews(1, {1, 3}, {1, 2}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.Null(geniusController.GetPlanningList(oLoadOptions, 2))

            End Using
        End Sub

        <Fact(DisplayName:="Should not get a planning list when view is common")>
        Public Sub ShouldNotGetAPlanningListWhenViewIsCommon()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oLoadOptions As New DataSourceLoadOptions
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetPlanningListForGeniusViews(1, {1, 3}, {1, 2}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.Null(geniusController.GetPlanningList(oLoadOptions, 4))

            End Using
        End Sub

        <Fact(DisplayName:="Should delete a planning if is view owner")>
        Public Sub ShouldDeleteAPlanningIfIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetPlanningListForGeniusViews(1, {1, 3}, {1, 2}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.True(CType(geniusController.DeletePlanning(1, "", ""), JsonResult).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete a planning not is owner")>
        Public Sub ShouldNotDeleteAPlanningNotIsOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetPlanningListForGeniusViews(1, {1, 3}, {1, 2}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(CType(geniusController.DeletePlanning(4, "", ""), JsonResult).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should execute a view where is owner")>
        Public Sub ShouldExecuteAViewWhereIsOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim executeView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.NotNull(geniusController.runGeniusView(executeView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not execute a view where is not owner")>
        Public Sub ShouldNotExecuteAViewWhereIsNotOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim executeView As New roGeniusView() With {.Id = 2, .IdPassport = 2}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.Null(geniusController.runGeniusView(executeView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should share a view where is owner")>
        Public Sub ShouldShareAViewWhereIsOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.True(CType(geniusController.ShareGeniusView(1, {2, 3}).Data, Boolean))

            End Using
        End Sub

        <Fact(DisplayName:="Should not share a view where is not owner")>
        Public Sub ShouldNotShareAViewWhereIsNotOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim shareView As New roGeniusView() With {.Id = 2, .IdPassport = 2}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(CType(geniusController.ShareGeniusView(2, {2, 3}).Data, Boolean))

            End Using
        End Sub

        <Fact(DisplayName:="Should get execution info if is view owner")>
        Public Sub ShouldGetExecutionInfoIfIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetViewExecutions(1, 1, True)

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.NotNull(geniusController.GetAzureFileInfo(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not get execution info if is not view owner")>
        Public Sub ShouldNotGetExecutionInfoIfIsNotViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetViewExecutions(2, 2, True)

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.GetAzureFileInfo(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should query execution status if is view owner")>
        Public Sub ShouldQueryExecutionStatusIfIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetViewExecutions(1, 1, True)

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.True(geniusController.getGeniusViewStatus(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should query execution status if is view owner and not finished and error exists")>
        Public Sub ShouldQueryExecutionStatusIfIsViewOwnerAndNotFinishedAndErrorExists()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetViewExecutions(1, 1, False)

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.getGeniusViewStatus(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not query execution status if is not view owner")>
        Public Sub ShouldNotQueryExecutionStatusIfIsNotViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                geniusHelper.SetViewExecutions(2, 2, True)

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.getGeniusViewStatus(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should delete genius view is view owner")>
        Public Sub ShouldDeleteGeniusViewIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oGeniusView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.True(geniusController.DeleteGeniusView(oGeniusView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not delete genius view not is view owner")>
        Public Sub ShouldNotDeleteGeniusViewNotIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oGeniusView As New roGeniusView() With {.Id = 2, .IdPassport = 2}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.DeleteGeniusView(oGeniusView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should update genius view is view owner")>
        Public Sub ShouldUpdateGeniusViewIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oGeniusView As New roGeniusExecution() With {.Id = 1, .IdGeniusView = 1}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.True(geniusController.updateGeniusView(oGeniusView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not update genius view not is view owner")>
        Public Sub ShouldNotUpdateGeniusViewNotIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oGeniusView As New roGeniusExecution() With {.Id = 2, .IdGeniusView = 2}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.updateGeniusView(oGeniusView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should save genius view is view owner")>
        Public Sub ShouldSaveGeniusViewIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oGeniusView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.True(geniusController.saveGeniusView(oGeniusView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not save genius view not is view owner")>
        Public Sub ShouldNotSaveGeniusViewNotIsViewOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                Dim oGeniusView As New roGeniusView() With {.Id = 2, .IdPassport = 2}
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.saveGeniusView(oGeniusView).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should get a genius view when task belongs to owner")>
        Public Sub ShouldGetAGeniusViewWhenTaskBelongsToOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.NotNull(geniusController.GetGeniusByTask(1).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should not get a genius view when task does not belongs toowner")>
        Public Sub ShouldNotGetAGeniusViewWhenTaskDoesNotBelongsToOwner()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange  )
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})

                'Act
                Dim geniusController As New VTLive40.GeniusController()

                'Assert
                Assert.False(geniusController.GetGeniusByTask(2).Data)

            End Using
        End Sub

        <Fact(DisplayName:="Should save genius view when execute view without employees")>
        Public Sub ShouldSaveGeniusViewWhenExecuteViewWithoutEmployees()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                webHelper.HttpContextStub()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim executeView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusViewByIdPageBaseInt32Boolean = Function() New roGeniusView() With {.Id = 1, .IdPassport = 1, .Employees = Nothing}
                geniusHelper.SaveGeniusViewSpy()

                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.runGeniusView(executeView)

                ''Assert
                Assert.True(geniusHelper.ViewSaved)

            End Using
        End Sub

        <Fact(DisplayName:="Should not save genius view when execute view with employees")>
        Public Sub ShouldNotSaveGeniusViewWhenExecuteViewWithEmployees()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                webHelper.HttpContextStub()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim executeView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusViewByIdPageBaseInt32Boolean = Function() New roGeniusView() With {.Id = 1, .IdPassport = 1, .Employees = "employees"}
                geniusHelper.SaveGeniusViewSpy()

                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.runGeniusView(executeView)

                ''Assert
                Assert.False(geniusHelper.ViewSaved)

            End Using
        End Sub

        <Fact(DisplayName:="Should save genius view when create custom view")>
        Public Sub ShouldSaveGeniusViewWhenCreateCustomView()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim customView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusViewByCheckBoxesPageBaseInt32ArrayBoolean = Function() New roGeniusView() With {.Id = 1, .IdPassport = 1}
                geniusHelper.SaveGeniusViewSpy()

                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.saveCustomView(customView)

                ''Assert
                Assert.True(geniusHelper.ViewSaved)

            End Using
        End Sub

        <Fact(DisplayName:="Should save genius view when create genius view from system")>
        Public Sub ShouldSaveGeniusViewWhenCreateGeniusViewFromSystem()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim customView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                geniusHelper.SaveGeniusViewSpy()

                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.createGeniusView(customView)

                ''Assert
                Assert.True(geniusHelper.ViewSaved)

            End Using
        End Sub

        <Fact(DisplayName:="Should save genius view when save existing genius view")>
        Public Sub ShouldSaveGeniusViewWhenSaveExistingGeniusView()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim customView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                geniusHelper.SaveGeniusViewSpy()

                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.saveGeniusView(customView)

                ''Assert
                Assert.True(geniusHelper.ViewSaved)

            End Using
        End Sub

        <Fact(DisplayName:="Should translate measures when create custom view from checkboxes")>
        Public Sub ShouldTranslateMeasuresWhenCreateCustomViewFromCheckboxes()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim customView As New roGeniusView() With {.Id = 1, .IdPassport = 1}
                Robotics.Web.Base.API.Fakes.ShimGeniusServiceMethods.GetGeniusViewByCheckBoxesPageBaseInt32ArrayBoolean = Function() New roGeniusView() With {.Id = 1, .IdPassport = 1, .CubeLayout = "{""slice"":{""reportFilters"":[{""uniqueName"":""IDContract""},{""uniqueName"":""CurrentEmployee""},{""uniqueName"":""UserField1""},{""uniqueName"":""UserField2""},{""uniqueName"":""UserField3""},{""uniqueName"":""UserField4""},{""uniqueName"":""UserField5""},{""uniqueName"":""UserField6""},{""uniqueName"":""UserField7""},{""uniqueName"":""UserField8""},{""uniqueName"":""UserField9""},{""uniqueName"":""UserField10""}],""rows"":[{""uniqueName"":""GroupName""},{""uniqueName"":""FullGroupName""},{""uniqueName"":""EmployeeName""},{""uniqueName"":""Date_ToDateString""},{""uniqueName"":""Time""},{""uniqueName"":""TerminalName""},{""uniqueName"":""PDDesc""}],""columns"":[{""uniqueName"":""[Measures]""}],""measures"":[{""uniqueName"":""Value"",""aggregation"":""sum""}],""expands"":{""expandAll"":true}}, ""options"":{""grid"":{""type"":""classic"",""showTotals"":""off""}}, ""creationDate"":""2021-04-19T15:46:46.872Z""}"}

                Robotics.Web.Base.Fakes.ShimroLanguageWeb.Constructor = Function(a As Robotics.Web.Base.roLanguageWeb)
                                                                            Return a
                                                                        End Function
                Robotics.Web.Base.Fakes.ShimroLanguageWeb.AllInstances.InitVariables = Sub(instance)

                                                                                       End Sub
                Robotics.Web.Base.Fakes.ShimroLanguageWeb.AllInstances.TranslateStringStringListOfStringBooleanString = Function(a As Robotics.Web.Base.roLanguageWeb, key As String, value As String, b As List(Of String), c As Boolean, d As String) "translated"

                geniusHelper.SaveGeniusViewSpy()
                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.saveCustomView(customView)

                ''Assert
                Assert.True(customView.CubeLayout = "{""slice"":{""reportFilters"":[{""uniqueName"":""IDContract""},{""uniqueName"":""CurrentEmployee""},{""uniqueName"":""UserField1""},{""uniqueName"":""UserField2""},{""uniqueName"":""UserField3""},{""uniqueName"":""UserField4""},{""uniqueName"":""UserField5""},{""uniqueName"":""UserField6""},{""uniqueName"":""UserField7""},{""uniqueName"":""UserField8""},{""uniqueName"":""UserField9""},{""uniqueName"":""UserField10""}],""rows"":[{""uniqueName"":""GroupName""},{""uniqueName"":""FullGroupName""},{""uniqueName"":""EmployeeName""},{""uniqueName"":""Date_ToDateString""},{""uniqueName"":""Time""},{""uniqueName"":""TerminalName""},{""uniqueName"":""PDDesc""}],""columns"":[{""uniqueName"":""[Measures]""}],""measures"":[{""uniqueName"":""Value"",""aggregation"":""sum"",""caption"":""translated""}],""expands"":{""expandAll"":true}},""options"":{""grid"":{""type"":""classic"",""showTotals"":""off""}},""creationDate"":""2021-04-19T15:46:46.872Z""}")

            End Using
        End Sub

        <Fact(DisplayName:="Should translate measures when create custom view from system view")>
        Public Sub ShouldTranslateMeasuresWhenCreateCustomViewFromSystemView()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim customView As New roGeniusView() With {.Id = 1, .IdPassport = 1, .CubeLayout = "{""slice"":{""reportFilters"":[{""uniqueName"":""IDContract""},{""uniqueName"":""CurrentEmployee""},{""uniqueName"":""UserField1""},{""uniqueName"":""UserField2""},{""uniqueName"":""UserField3""},{""uniqueName"":""UserField4""},{""uniqueName"":""UserField5""},{""uniqueName"":""UserField6""},{""uniqueName"":""UserField7""},{""uniqueName"":""UserField8""},{""uniqueName"":""UserField9""},{""uniqueName"":""UserField10""}],""rows"":[{""uniqueName"":""GroupName""},{""uniqueName"":""FullGroupName""},{""uniqueName"":""EmployeeName""},{""uniqueName"":""Date_ToDateString""},{""uniqueName"":""Time""},{""uniqueName"":""TerminalName""},{""uniqueName"":""PDDesc""}],""columns"":[{""uniqueName"":""[Measures]""}],""measures"":[{""uniqueName"":""Value"",""aggregation"":""sum""}],""expands"":{""expandAll"":true}}, ""options"":{""grid"":{""type"":""classic"",""showTotals"":""off""}}, ""creationDate"":""2021-04-19T15:46:46.872Z""}"}

                Robotics.Web.Base.Fakes.ShimroLanguageWeb.Constructor = Function(a As Robotics.Web.Base.roLanguageWeb)
                                                                            Return a
                                                                        End Function
                Robotics.Web.Base.Fakes.ShimroLanguageWeb.AllInstances.InitVariables = Sub(instance)

                                                                                       End Sub
                Robotics.Web.Base.Fakes.ShimroLanguageWeb.AllInstances.TranslateStringStringListOfStringBooleanString = Function(a As Robotics.Web.Base.roLanguageWeb, key As String, value As String, b As List(Of String), c As Boolean, d As String) "translated"

                geniusHelper.SaveGeniusViewSpy()
                'Act
                Dim geniusController As New VTLive40.GeniusController()
                geniusController.createGeniusView(customView)

                ''Assert
                Assert.True(customView.CubeLayout = "{""slice"":{""reportFilters"":[{""uniqueName"":""IDContract""},{""uniqueName"":""CurrentEmployee""},{""uniqueName"":""UserField1""},{""uniqueName"":""UserField2""},{""uniqueName"":""UserField3""},{""uniqueName"":""UserField4""},{""uniqueName"":""UserField5""},{""uniqueName"":""UserField6""},{""uniqueName"":""UserField7""},{""uniqueName"":""UserField8""},{""uniqueName"":""UserField9""},{""uniqueName"":""UserField10""}],""rows"":[{""uniqueName"":""GroupName""},{""uniqueName"":""FullGroupName""},{""uniqueName"":""EmployeeName""},{""uniqueName"":""Date_ToDateString""},{""uniqueName"":""Time""},{""uniqueName"":""TerminalName""},{""uniqueName"":""PDDesc""}],""columns"":[{""uniqueName"":""[Measures]""}],""measures"":[{""uniqueName"":""Value"",""aggregation"":""sum"",""caption"":""translated""}],""expands"":{""expandAll"":true}},""options"":{""grid"":{""type"":""classic"",""showTotals"":""off""}},""creationDate"":""2021-04-19T15:46:46.872Z""}")

            End Using
        End Sub

        <Fact(DisplayName:="Should get execution language if is language key is setted")>
        Public Sub ShouldGetExecutionLanguageIfIsLanguageKeyIsSetted()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                dataLayerHelper.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"geniusexecutions", {New Dictionary(Of String, Object) From {{"ID", 1}, {"IdGeniusView", 1}, {"FileLink", "link"}, {"CubeLayout", "layout"}, {"ExecutionDate", Now}, {"IdTask", 1}, {"Name", "name"}, {"SASLink", "SASLink"}, {"culture", "en-US"}}}}
                                              })
                Robotics.Web.Base.Fakes.ShimWebServiceHelper.SetStateObjectRefInt32 = Sub(ByRef a As Object, b As Integer)

                                                                                      End Sub
                azureHelper.GetFileSaSTokenWithURI()
                azureHelper.GetFileSizeFromAzure()
                'Act
                Dim geniusController As New VTLive40.GeniusController()
                Dim result = geniusController.GetAzureFileInfo(1)

                'Assert                
                Assert.Equal("EN", result.Data.executionlanguage.toupper())

            End Using
        End Sub

        <Fact(DisplayName:="Should get execution language if is language key is unexpected")>
        Public Sub ShouldGetExecutionLanguageIfIsLanguageKeyIsUnexpected()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                dataLayerHelper.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"geniusexecutions", {New Dictionary(Of String, Object) From {{"ID", 1}, {"IdGeniusView", 1}, {"FileLink", "link"}, {"CubeLayout", "layout"}, {"ExecutionDate", Now}, {"IdTask", 1}, {"Name", "name"}, {"SASLink", "SASLink"}, {"culture", "en"}}}}
                                              })
                Robotics.Web.Base.Fakes.ShimWebServiceHelper.SetStateObjectRefInt32 = Sub(ByRef a As Object, b As Integer)

                                                                                      End Sub
                azureHelper.GetFileSaSTokenWithURI()
                azureHelper.GetFileSizeFromAzure()
                'Act
                Dim geniusController As New VTLive40.GeniusController()
                Dim result = geniusController.GetAzureFileInfo(1)

                'Assert                
                Assert.Equal("EN", result.Data.executionlanguage.toupper())

            End Using
        End Sub

        <Fact(DisplayName:="Should get execution language if is language key is empty")>
        Public Sub ShouldGetExecutionLanguageIfIsLanguageKeyIsEmpty()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                dataLayerHelper.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"geniusexecutions", {New Dictionary(Of String, Object) From {{"ID", 1}, {"IdGeniusView", 1}, {"FileLink", "link"}, {"CubeLayout", "layout"}, {"ExecutionDate", Now}, {"IdTask", 1}, {"Name", "name"}, {"SASLink", "SASLink"}, {"culture", ""}}}}
                                              })
                Robotics.Web.Base.Fakes.ShimWebServiceHelper.SetStateObjectRefInt32 = Sub(ByRef a As Object, b As Integer)

                                                                                      End Sub
                azureHelper.GetFileSaSTokenWithURI()
                azureHelper.GetFileSizeFromAzure()
                'Act
                Dim geniusController As New VTLive40.GeniusController()
                Dim result = geniusController.GetAzureFileInfo(1)

                'Assert                
                Assert.Equal("", result.Data.executionlanguage.toupper())

            End Using
        End Sub


        <Fact(DisplayName:="Should get execution language if is language key is null")>
        Public Sub ShouldGetExecutionLanguageIfIsLanguageKeyIsNull()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                geniusHelper.SetAvailableGeniusViews(1, {1, 3}, {2}, {4, 5})
                dataLayerHelper.CreateDataTableStub(New Dictionary(Of String, Dictionary(Of String, Object)()) From {
                                                  {"geniusexecutions", {New Dictionary(Of String, Object) From {{"ID", 1}, {"IdGeniusView", 1}, {"FileLink", "link"}, {"CubeLayout", "layout"}, {"ExecutionDate", Now}, {"IdTask", 1}, {"Name", "name"}, {"SASLink", "SASLink"}, {"culture", DBNull.Value}}}}
                                              })
                Robotics.Web.Base.Fakes.ShimWebServiceHelper.SetStateObjectRefInt32 = Sub(ByRef a As Object, b As Integer)

                                                                                      End Sub
                azureHelper.GetFileSaSTokenWithURI()
                azureHelper.GetFileSizeFromAzure()
                'Act
                Dim geniusController As New VTLive40.GeniusController()
                Dim result = geniusController.GetAzureFileInfo(1)

                'Assert                
                Assert.Equal("", result.Data.executionlanguage.toupper())

            End Using
        End Sub

    End Class

End Namespace