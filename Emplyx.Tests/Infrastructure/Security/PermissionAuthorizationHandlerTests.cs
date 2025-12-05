using System.Security.Claims;
using Emplyx.Domain.Repositories;
using Emplyx.Infrastructure.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Moq;

namespace Emplyx.Tests.Infrastructure.Security;

public class PermissionAuthorizationHandlerTests
{
    private readonly Mock<IHttpContextAccessor> _httpContextAccessorMock;
    private readonly Mock<IUsuarioRepository> _usuarioRepositoryMock;
    private readonly PermissionAuthorizationHandler _handler;

    public PermissionAuthorizationHandlerTests()
    {
        _httpContextAccessorMock = new Mock<IHttpContextAccessor>();
        _usuarioRepositoryMock = new Mock<IUsuarioRepository>();
        _handler = new PermissionAuthorizationHandler(_httpContextAccessorMock.Object, _usuarioRepositoryMock.Object);
    }

    [Fact]
    public async Task HandleAsync_ShouldSucceed_WhenUserHasPermission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var permission = "TestPermission";
        var requirement = new PermissionRequirement(permission);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        _usuarioRepositoryMock.Setup(x => x.HasPermissionAsync(userId, permission, null, default))
            .ReturnsAsync(true);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
    }

    [Fact]
    public async Task HandleAsync_ShouldFail_WhenUserDoesNotHavePermission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var permission = "TestPermission";
        var requirement = new PermissionRequirement(permission);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        _usuarioRepositoryMock.Setup(x => x.HasPermissionAsync(userId, permission, null, default))
            .ReturnsAsync(false);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeFalse();
    }

    [Fact]
    public async Task HandleAsync_ShouldUseContextId_WhenHeaderIsPresent()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var contextId = Guid.NewGuid();
        var permission = "TestPermission";
        var requirement = new PermissionRequirement(permission);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString())
        }));
        var context = new AuthorizationHandlerContext(new[] { requirement }, user, null);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Headers["X-Context-Id"] = contextId.ToString();
        _httpContextAccessorMock.Setup(x => x.HttpContext).Returns(httpContext);

        _usuarioRepositoryMock.Setup(x => x.HasPermissionAsync(userId, permission, contextId, default))
            .ReturnsAsync(true);

        // Act
        await _handler.HandleAsync(context);

        // Assert
        context.HasSucceeded.Should().BeTrue();
        _usuarioRepositoryMock.Verify(x => x.HasPermissionAsync(userId, permission, contextId, default), Times.Once);
    }
}
