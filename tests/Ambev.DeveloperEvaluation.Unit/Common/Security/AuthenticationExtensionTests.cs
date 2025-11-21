using Ambev.DeveloperEvaluation.Common.Security;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Security;

public class AuthenticationExtensionTests
{
    [Fact(DisplayName = "AddJwtAuthentication should throw exception when secret key is missing")]
    public void AddJwtAuthentication_MissingSecretKey_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Substitute.For<IConfiguration>();
        configuration["Jwt:SecretKey"].Returns((string?)null);

        // Act
        Action act = () => services.AddJwtAuthentication(configuration);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact(DisplayName = "AddJwtAuthentication should register services when configuration is valid")]
    public void AddJwtAuthentication_ValidConfig_RegistersServices()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Substitute.For<IConfiguration>();
        configuration["Jwt:SecretKey"].Returns("ChaveSecretaValidaParaTesteUnitario123");

        // Act
        services.AddJwtAuthentication(configuration);

        // Assert
        services.Should().Contain(s => s.ServiceType == typeof(IJwtTokenGenerator));
    }

    [Fact(DisplayName = "AddJwtAuthentication should configure JwtBearerOptions correctly")]
    public void AddJwtAuthentication_ConfiguresOptionsCorrectly()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Substitute.For<IConfiguration>();
        var secretKey = "ChaveSecretaMuitoSeguraParaTestes123";
        configuration["Jwt:SecretKey"].Returns(secretKey);

        // Act
        services.AddJwtAuthentication(configuration);
        var provider = services.BuildServiceProvider();

        var optionsMonitor = provider.GetRequiredService<IOptionsMonitor<JwtBearerOptions>>();
        var options = optionsMonitor.Get(JwtBearerDefaults.AuthenticationScheme);

        // Assert
        options.SaveToken.Should().BeTrue();
        options.RequireHttpsMetadata.Should().BeFalse();
        options.TokenValidationParameters.Should().NotBeNull();
        options.TokenValidationParameters.ValidateIssuerSigningKey.Should().BeTrue();
        options.TokenValidationParameters.ValidateIssuer.Should().BeFalse();
        options.TokenValidationParameters.ValidateAudience.Should().BeFalse();
        options.TokenValidationParameters.IssuerSigningKey.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddJwtAuthentication should configure AuthenticationOptions correctly")]
    public void AddJwtAuthentication_ConfiguresAuthenticationOptions()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = Substitute.For<IConfiguration>();
        configuration["Jwt:SecretKey"].Returns("ChaveSecretaParaTeste123");

        // Act
        services.AddJwtAuthentication(configuration);
        var provider = services.BuildServiceProvider();

        var options = provider.GetRequiredService<IOptions<AuthenticationOptions>>().Value;

        // Assert
        options.DefaultAuthenticateScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        options.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
    }
}