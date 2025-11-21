using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Auth;

public class AuthenticateUserValidatorTests
{
    private readonly AuthenticateUserValidator _validator;

    public AuthenticateUserValidatorTests()
    {
        _validator = new AuthenticateUserValidator();
    }

    [Fact(DisplayName = "Valid command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = new AuthenticateUserCommand
        {
            Email = "test@developer.com",
            Password = "password123"
        };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory(DisplayName = "Invalid email formats should fail validation")]
    [InlineData("")]
    [InlineData("invalid-email")]
    public void Given_InvalidEmail_When_Validated_Then_ShouldHaveError(string email)
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = email, Password = "password123" };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Email);
    }

    [Theory(DisplayName = "Invalid password formats should fail validation")]
    [InlineData("")]
    [InlineData("12345")]
    public void Given_InvalidPassword_When_Validated_Then_ShouldHaveError(string password)
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "test@developer.com", Password = password };

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Password);
    }
}