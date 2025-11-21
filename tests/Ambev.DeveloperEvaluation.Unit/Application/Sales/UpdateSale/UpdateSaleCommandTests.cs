using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleCommandTests
{
    [Fact(DisplayName = "Validate should return valid result for valid command")]
    public void Validate_ValidCommand_ReturnsValidResult()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();

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
        var command = new UpdateSaleCommand();

        // Act
        var result = command.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        var firstError = result.Errors.First();
        firstError.Error.Should().NotBeNullOrEmpty();
        firstError.Detail.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "UpdateSaleItemCommand properties should be accessible")]
    public void UpdateSaleItemCommand_ShouldSetAndGetProperties()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var description = "Updated Product";
        var quantity = 5;
        var price = 150.00m;

        // Act
        var itemCommand = new UpdateSaleItemCommand
        {
            ProductId = productId,
            ProductDescription = description,
            Quantity = quantity,
            UnitPrice = price
        };

        itemCommand.ProductId.Should().Be(productId);
        itemCommand.ProductDescription.Should().Be(description);
        itemCommand.Quantity.Should().Be(quantity);
        itemCommand.UnitPrice.Should().Be(price);
    }
}