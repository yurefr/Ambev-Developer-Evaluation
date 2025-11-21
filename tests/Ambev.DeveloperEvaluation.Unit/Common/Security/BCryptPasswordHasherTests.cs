using Ambev.DeveloperEvaluation.Common.Security;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Security;

public class BCryptPasswordHasherTests
{
    private readonly BCryptPasswordHasher _hasher;

    public BCryptPasswordHasherTests()
    {
        _hasher = new BCryptPasswordHasher();
    }

    [Fact(DisplayName = "HashPassword should return a hashed string")]
    public void HashPassword_ReturnsHash()
    {
        // Arrange
        var password = "password123";

        // Act
        var hash = _hasher.HashPassword(password);

        // Assert
        hash.Should().NotBeNullOrEmpty();
        hash.Should().NotBe(password);
    }

    [Fact(DisplayName = "VerifyPassword should return true for correct password")]
    public void VerifyPassword_CorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "password123";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword(password, hash);

        // Assert
        result.Should().BeTrue();
    }

    [Fact(DisplayName = "VerifyPassword should return false for incorrect password")]
    public void VerifyPassword_IncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "password123";
        var hash = _hasher.HashPassword(password);

        // Act
        var result = _hasher.VerifyPassword("wrongpassword", hash);

        // Assert
        result.Should().BeFalse();
    }
}