using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

/// <summary>
/// Contains unit tests for CreateSaleCommandValidator.
/// Tests cover validation rules for customer, branch, and items.
/// </summary>
public class CreateSaleValidatorTests
{
    private readonly CreateSaleCommandValidator _validator;

    public CreateSaleValidatorTests()
    {
        _validator = new CreateSaleCommandValidator();
    }

    [Fact(DisplayName = "Valid command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty items list should fail validation")]
    public void Given_EmptyItemsList_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateInvalidCommand_NoItems();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Items);
    }

    [Fact(DisplayName = "Empty customer ID should fail validation")]
    public void Given_EmptyCustomerId_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        command.CustomerId = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.CustomerId);
    }
}