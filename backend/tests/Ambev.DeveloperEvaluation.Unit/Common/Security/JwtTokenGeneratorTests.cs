using Ambev.DeveloperEvaluation.Common.Security;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Security;

public class JwtTokenGeneratorTests
{
    private readonly IConfiguration _configuration;
    private readonly JwtTokenGenerator _generator;

    public JwtTokenGeneratorTests()
    {
        _configuration = Substitute.For<IConfiguration>();
        _configuration["Jwt:SecretKey"].Returns("SegredoSuperSecretoParaTestesUnitarios123!");

        _generator = new JwtTokenGenerator(_configuration);
    }

    [Fact(DisplayName = "GenerateToken should return valid JWT token")]
    public void GenerateToken_ValidUser_ReturnsToken()
    {
        // Arrange
        var user = Substitute.For<IUser>();
        user.Id.Returns(Guid.NewGuid().ToString());
        user.Username.Returns("testuser");
        user.Role.Returns("Admin");

        // Act
        var token = _generator.GenerateToken(user);

        // Assert
        token.Should().NotBeNullOrEmpty();

        var handler = new JwtSecurityTokenHandler();
        var jwtToken = handler.ReadJwtToken(token);

        var idClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier || c.Type == "nameid");
        idClaim.Should().NotBeNull();
        idClaim!.Value.Should().Be(user.Id);

        var nameClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name || c.Type == "unique_name");
        nameClaim.Should().NotBeNull();
        nameClaim!.Value.Should().Be(user.Username);

        var roleClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role || c.Type == "role");
        roleClaim.Should().NotBeNull();
        roleClaim!.Value.Should().Be(user.Role);
    }
}