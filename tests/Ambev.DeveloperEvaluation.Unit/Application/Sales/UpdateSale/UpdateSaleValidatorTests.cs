using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

/// <summary>
/// Contains unit tests for UpdateSaleValidator.
/// Tests cover validation rules for IDs and required fields.
/// </summary>
public class UpdateSaleValidatorTests
{
    private readonly UpdateSaleValidator _validator;

    public UpdateSaleValidatorTests()
    {
        _validator = new UpdateSaleValidator();
    }

    [Fact(DisplayName = "Valid update command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty Sale ID should fail validation")]
    public void Given_EmptySaleId_When_Validated_Then_ShouldHaveError()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.Id = Guid.Empty;

        // Act
        var result = _validator.TestValidate(command);

        // Assert
        result.ShouldHaveValidationErrorFor(c => c.Id);
    }
}