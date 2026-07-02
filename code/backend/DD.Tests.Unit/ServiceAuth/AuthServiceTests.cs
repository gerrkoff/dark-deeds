using DD.ServiceAuth.Domain.Entities;
using DD.ServiceAuth.Domain.OAuth;
using DD.ServiceAuth.Domain.Services;
using DD.Shared.Details.Abstractions.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

namespace DD.Tests.Unit.ServiceAuth;

// CreateAccessTokenAsync is the OAuth/MCP access-token entry point: it must mint the token with
// the dedicated dd-oauth-access audience (scoping it to /mcp) and return null when the user does
// not exist. Asserting the exact audience argument forwarded to ITokenService.Serialize guards
// against a regression that would drop the scoping and make MCP tokens valid app-wide.
public class AuthServiceTests
{
    private const string UserId = "user-42";
    private const int LifetimeMinutes = 60;

    private readonly Mock<UserManager<UserEntity>> _userManager = CreateUserManagerMock();
    private readonly Mock<ITokenService> _tokenService = new();
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _authService = new AuthService(_userManager.Object, _tokenService.Object);
    }

    [Fact]
    public async Task CreateAccessTokenAsync_ExistingUser_SerializesWithMcpAudienceAndLifetime()
    {
        // Arrange
        var user = new UserEntity { UserName = "test-user", DisplayName = "Test User" };
        _userManager.Setup(m => m.FindByIdAsync(UserId)).ReturnsAsync(user);
        _tokenService
            .Setup(s => s.Serialize(It.IsAny<AuthTokenBuildInfo>(), It.IsAny<int?>(), It.IsAny<string?>()))
            .Returns("minted-token");

        // Act
        var token = await _authService.CreateAccessTokenAsync(UserId, LifetimeMinutes);

        // Assert
        Assert.Equal("minted-token", token);
        _tokenService.Verify(
            s => s.Serialize(
                It.Is<AuthTokenBuildInfo>(b => b.UserId == user.Id.ToString() && b.Username == "test-user"),
                LifetimeMinutes,
                OAuthConstants.AccessTokenAudience),
            Times.Once);
    }

    [Fact]
    public async Task CreateAccessTokenAsync_UnknownUser_ReturnsNullAndDoesNotSerialize()
    {
        // Arrange
        _userManager.Setup(m => m.FindByIdAsync(UserId)).ReturnsAsync((UserEntity?)null);

        // Act
        var token = await _authService.CreateAccessTokenAsync(UserId, LifetimeMinutes);

        // Assert
        Assert.Null(token);
        _tokenService.Verify(
            s => s.Serialize(It.IsAny<AuthTokenBuildInfo>(), It.IsAny<int?>(), It.IsAny<string?>()),
            Times.Never);
    }

    private static Mock<UserManager<UserEntity>> CreateUserManagerMock()
    {
        // UserManager exposes no injectable interface, so the documented test seam is to mock the
        // class itself: only the required IUserStore is supplied and the rest is left null, since
        // the virtual FindByIdAsync is overridden per-test and the other collaborators are unused.
        var store = new Mock<IUserStore<UserEntity>>();
        return new Mock<UserManager<UserEntity>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }
}
