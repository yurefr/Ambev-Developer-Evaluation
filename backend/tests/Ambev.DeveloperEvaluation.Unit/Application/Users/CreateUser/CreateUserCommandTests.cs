using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Unit.Domain;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.CreateUser;

public class CreateUserCommandTests
{
    [Fact(DisplayName = "Validate should return valid result for valid command")]
    public void Validate_ValidCommand_ReturnsValidResult()
    {
        // Arrange
        var command = CreateUserHandlerTestData.GenerateValidCommand();

        // Act
        var result = command.Validate();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "Validate should return invalid result for invalid command")]
    public void Validate_InvalidCommand_ReturnsInvalidResult()
    {
        // Arrange
        var command = new CreateUserCommand();
        // Act
        var result = command.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        var error = result.Errors.First();
        error.Error.Should().NotBeNull();
    }
}