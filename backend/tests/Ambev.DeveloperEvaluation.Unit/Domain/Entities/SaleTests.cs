using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleTests
{
    [Fact(DisplayName = "New sale should start with zero total and active status")]
    public void Given_NewSale_When_Created_Then_ShouldHaveDefaultValues()
    {
        // Arrange & Act
        var sale = SaleTestData.GenerateValidSale();

        // Assert
        sale.TotalAmount.Value.Should().Be(0);
        sale.IsCancelled.Should().BeFalse();
        sale.DomainEvents.Should().ContainItemsAssignableTo<SaleCreatedEvent>();
    }

    [Fact(DisplayName = "Adding items below 4 quantity should apply NO discount")]
    public void Given_Sale_When_AddingItemBelow4_Then_NoDiscountApplied()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();
        var quantity = 3;

        // Act
        sale.AddItem(productId, desc, quantity, unitPrice);

        // Assert
        var item = sale.SaleItems.First();
        item.Discount.Value.Should().Be(0m);
        item.TotalAmount.Value.Should().Be(quantity * unitPrice);
        sale.TotalAmount.Value.Should().Be(quantity * unitPrice);
    }

    [Fact(DisplayName = "Adding 4 to 9 items should apply 10% discount")]
    public void Given_Sale_When_AddingBetween4And9Items_Then_Apply10PercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();
        var quantity = 5;

        // Act
        sale.AddItem(productId, desc, quantity, unitPrice);

        // Assert
        var item = sale.SaleItems.First();
        item.Discount.Value.Should().Be(0.10m);

        var expectedTotal = (quantity * unitPrice) * 0.90m;
        item.TotalAmount.Value.Should().Be(expectedTotal);
        sale.TotalAmount.Value.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "Adding 10 to 20 items should apply 20% discount")]
    public void Given_Sale_When_AddingBetween10And20Items_Then_Apply20PercentDiscount()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();
        var quantity = 15;

        // Act
        sale.AddItem(productId, desc, quantity, unitPrice);

        // Assert
        var item = sale.SaleItems.First();
        item.Discount.Value.Should().Be(0.20m);

        var expectedTotal = (quantity * unitPrice) * 0.80m;
        item.TotalAmount.Value.Should().Be(expectedTotal);
        sale.TotalAmount.Value.Should().Be(expectedTotal);
    }

    [Fact(DisplayName = "Adding more than 20 items should throw DomainException")]
    public void Given_Sale_When_AddingMoreThan20Items_Then_ThrowException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();

        // Act & Assert
        var action = () => sale.AddItem(productId, desc, 21, unitPrice);

        action.Should().Throw<DomainException>()
            .WithMessage("*Cannot sell more than 20 identical items*");
    }

    [Fact(DisplayName = "Cancelling sale should mark as cancelled and emit event")]
    public void Given_ActiveSale_When_Cancelled_Then_IsCancelledTrueAndEventEmitted()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();
        sale.AddItem(productId, desc, 1, unitPrice);

        // Act
        sale.Cancel();

        // Assert
        sale.IsCancelled.Should().BeTrue();
        sale.SaleItems.All(i => i.IsCancelled).Should().BeTrue();
        sale.DomainEvents.Should().ContainItemsAssignableTo<SaleCancelledEvent>();
    }
}