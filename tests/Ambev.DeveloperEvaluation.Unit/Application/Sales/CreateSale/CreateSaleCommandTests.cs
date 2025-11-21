using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleCommandTests
{
    [Fact(DisplayName = "Validate should return valid result for valid command")]
    public void Validate_ValidCommand_ReturnsValidResult()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();

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
        var command = new CreateSaleCommand();

        // Act
        var result = command.Validate();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().NotBeEmpty();

        var error = result.Errors.First();
        error.Error.Should().NotBeNullOrEmpty();
        error.Detail.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "CreateSaleItemCommand properties should be accessible")]
    public void CreateSaleItemCommand_ShouldSetAndGetProperties()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var description = "Test Product";
        var quantity = 10;
        var price = 99.99m;

        // Act
        var itemCommand = new CreateSaleItemCommand
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