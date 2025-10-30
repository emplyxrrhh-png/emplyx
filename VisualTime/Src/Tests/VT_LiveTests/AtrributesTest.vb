Imports System.ComponentModel
Imports System.Web.Mvc
Imports Robotics.Base.DTOs
Imports VT_XU_Base
Imports VTLive40.Controllers.Base
Imports Xunit

Namespace Unit.Test

    <Collection("Atrributes")>
    <CollectionDefinition("Atrributes", DisableParallelization:=True)>
    <Category("Atrributes")>
    Public Class AtrributesTest

        Private ReadOnly permissionsHelper As PermissionsHelper
        Private ReadOnly envHelper As EnvironmentHelper

        ''' <summary>
        ''' Test Setup
        ''' </summary>
        Sub New()
            permissionsHelper = New PermissionsHelper
            envHelper = New EnvironmentHelper
        End Sub

        <Fact(DisplayName:="Should allow access when loggedin attribute is requiered and has passport")>
        Public Sub ShouldAllowAccessWhenLoggedinAttributeIsRequieredAndHasPassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim loggedAttribute As New VTLive40.LoggedInAtrribute() With {.Requiered = True}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                loggedAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnActionExecuted)
            End Using

        End Sub

        <Fact(DisplayName:="Should deny access when loggedin attribute is requiered and has no passport")>
        Public Sub ShouldDenyAccessWhenLoggedinAttributeIsRequieredAndHasNoPassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.StubSetIdentity(0, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim loggedAttribute As New VTLive40.LoggedInAtrribute() With {.Requiered = True}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                loggedAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnAccessDenied)
            End Using

        End Sub

        <Fact(DisplayName:="Should allow access when loggedin attribute is not requiered and has passport")>
        Public Sub ShouldAllowAccessWhenLoggedinAttributeIsNotRequieredAndHasPassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim loggedAttribute As New VTLive40.LoggedInAtrribute() With {.Requiered = False}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                loggedAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnActionExecuted)
            End Using

        End Sub

        <Fact(DisplayName:="Should allow access when loggedin attribute is not requiered and has no passport")>
        Public Sub ShouldAllowAccessWhenLoggedinAttributeIsNotRequieredAndHasNoPassport()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.StubSetIdentity(0, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim loggedAttribute As New VTLive40.LoggedInAtrribute() With {.Requiered = False}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                loggedAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnActionExecuted)
            End Using

        End Sub

        <Fact(DisplayName:="Should allow access when passport has permissions requiered on permissions attribute")>
        Public Sub ShouldAllowAccessWhenPassportHasPermissionsrequieredOnPermissionsAttribute()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1", "2#2"}, {1})
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim permissionAttribute As New VTLive40.PermissionsAtrribute() With {.FeatureAlias = "Employees", .FeatureType = "U", .Permission = Permission.Write}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                permissionAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnActionExecuted)
            End Using

        End Sub

        <Fact(DisplayName:="Should allow access when passport has only one of permissions requiered on permissions attribute")>
        Public Sub ShouldAllowAccessWhenPassportHasOnlyOneOfPermissionsRequieredOnPermissionsAttribute()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#6"}, {"1#1", "2#2"}, {1})
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim permissionAttribute As New VTLive40.PermissionsAtrribute() With {.FeatureAlias = "Employees,Calendar", .FeatureType = "U", .Permission = Permission.Write}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                permissionAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnActionExecuted)
            End Using

        End Sub

        <Fact(DisplayName:="Should deny access when passport has no permissions requiered on permissions attribute")>
        Public Sub ShouldDenyAccessWhenPassportHasNoPermissionsRequieredOnPermissionsAttribute()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.SetFeatureAndUsersPermissions(1, {"Employees#U#3"}, {"1#1", "2#2"}, {1})
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim permissionAttribute As New VTLive40.PermissionsAtrribute() With {.FeatureAlias = "Employees", .FeatureType = "U", .Permission = Permission.Write}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                permissionAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnAccessDenied)
            End Using

        End Sub

        <Fact(DisplayName:="Should deny access when user not logged in")>
        Public Sub ShouldDenyAccessWhenUserNotLoggedIn()
            Using s = Microsoft.QualityTools.Testing.Fakes.ShimsContext.Create()
                CommonHelper.InitHAEnvironmentFakes()
                'Arrange
                envHelper.InitializeEnvirontment()
                permissionsHelper.SetFeatureAndUsersPermissions(0, {"Employees#U#3"}, {"1#1", "2#2"}, {1})
                permissionsHelper.StubSetIdentity(1, Guid.NewGuid.ToString(), Guid.NewGuid.ToString())
                Dim permissionAttribute As New VTLive40.PermissionsAtrribute() With {.FeatureAlias = "Employees", .FeatureType = "U", .Permission = Permission.Write}
                Dim actionDescriptor As New ReflectedActionDescriptor(GetType(BaseController).GetMethod("CardView"), "CardView", New ReflectedControllerDescriptor(GetType(BaseController)))
                Dim filterContext As New ActionExecutingContext() With {.ActionDescriptor = actionDescriptor}

                'Act
                permissionAttribute.OnActionExecuting(filterContext)

                'Assert
                Assert.True(envHelper.OnAccessDenied)
            End Using

        End Sub

    End Class

End Namespace