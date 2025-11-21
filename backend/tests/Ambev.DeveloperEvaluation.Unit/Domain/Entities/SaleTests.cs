using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using System.Reflection;
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

        sale.Status.Should().Be(SaleStatus.Active);

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
        sale.Status.Should().Be(SaleStatus.Cancelled);

        sale.SaleItems.All(i => i.IsCancelled).Should().BeTrue();

        sale.DomainEvents.Should().ContainItemsAssignableTo<SaleCancelledEvent>();
    }

    [Fact(DisplayName = "UpdateSaleInfo should update customer and branch details")]
    public void UpdateSaleInfo_UpdatesDetails()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var newCustomerId = Guid.NewGuid();
        var newCustomerName = "New Name";
        var newBranch = "New Branch";

        // Act
        sale.UpdateSaleInfo(newCustomerId, newCustomerName, newBranch);

        // Assert
        sale.CustomerId.Should().Be(newCustomerId);
        sale.CustomerName.Should().Be(newCustomerName);
        sale.Branch.Should().Be(newBranch);
        sale.UpdatedAt.Should().NotBeNull();
    }

    [Fact(DisplayName = "UpdateSaleInfo should throw exception if sale is cancelled")]
    public void UpdateSaleInfo_CancelledSale_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act
        Action act = () => sale.UpdateSaleInfo(Guid.NewGuid(), "A", "B");

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Cannot modify a cancelled sale.");
    }

    [Fact(DisplayName = "UpdateItems should update existing item quantity")]
    public void UpdateItems_ExistingItem_UpdatesQuantity()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();

        sale.AddItem(productId, desc, 1, unitPrice);

        // Act
        var updatedItemsList = new List<(Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice)>
        {(productId, desc, 5, unitPrice)};

        sale.UpdateItems(updatedItemsList);

        // Assert
        var item = sale.SaleItems.First(i => i.ProductId == productId);

        item.Quantity.Value.Should().Be(5);
        item.Discount.Value.Should().Be(0.10m);
    }

    [Fact(DisplayName = "CancelItem should cancel specific item and recalculate total")]
    public void CancelItem_ValidItem_CancelsAndRecalculates()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "P1", 1, 100m);
        sale.AddItem(Guid.NewGuid(), "P2", 1, 100m);

        var itemToCancel = sale.SaleItems.First();

        // Act
        sale.CancelItem(itemToCancel.Id);

        // Assert
        itemToCancel.IsCancelled.Should().BeTrue();
        sale.TotalAmount.Value.Should().Be(100m);
    }

    [Fact(DisplayName = "CancelItem should throw exception if item not found")]
    public void CancelItem_ItemNotFound_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        Action act = () => sale.CancelItem(Guid.NewGuid());

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*not found*");
    }

    [Fact(DisplayName = "Protected constructor should initialize instance")]
    public void ProtectedConstructor_InitializesInstance()
    {
        // Arrange & Act
        var sale = (Sale)Activator.CreateInstance(typeof(Sale), true)!;

        // Assert
        sale.Should().NotBeNull();
        sale.SaleItems.Should().NotBeNull();
    }

    [Fact(DisplayName = "AddItem should merge quantity if item already exists")]
    public void AddItem_ExistingItem_MergesQuantity()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var productId = Guid.NewGuid();
        sale.AddItem(productId, "Product A", 2, 10m);

        // Act
        sale.AddItem(productId, "Product A", 3, 10m);

        // Assert
        sale.SaleItems.Should().HaveCount(1);
        var item = sale.SaleItems.First();
        item.Quantity.Value.Should().Be(5);
        item.Discount.Value.Should().Be(0.10m);
    }

    [Fact(DisplayName = "UpdateItems should throw exception if sale is cancelled")]
    public void UpdateItems_CancelledSale_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        var itemsToUpdate = new List<(Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice)>
        { (Guid.NewGuid(), "Desc", 1, 10m)};

        // Act
        Action act = () => sale.UpdateItems(itemsToUpdate);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("Cannot modify a cancelled sale.");
    }

    [Fact(DisplayName = "UpdateItems should throw exception if quantity exceeds 20")]
    public void UpdateItems_QuantityExceedsLimit_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        var itemsToUpdate = new List<(Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice)>
        {(Guid.NewGuid(), "Desc", 21, 10m)};

        // Act
        Action act = () => sale.UpdateItems(itemsToUpdate);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage("*Cannot sell more than 20*");
    }

    [Fact(DisplayName = "UpdateItems should remove items that are not present in the updated list")]
    public void UpdateItems_MissingItemInList_RemovesItemFromSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var itemToKeep = SaleTestData.GenerateItemParams();
        var itemToRemove = SaleTestData.GenerateItemParams();

        sale.AddItem(itemToKeep.ProductId, itemToKeep.ProductDescription, 1, 10m);
        sale.AddItem(itemToRemove.ProductId, itemToRemove.ProductDescription, 1, 10m);
        sale.SaleItems.Should().HaveCount(2);

        var updatedList = new List<(Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice)>
        {
            (itemToKeep.ProductId, itemToKeep.ProductDescription, 5, 10m)
        };

        // Act
        sale.UpdateItems(updatedList);

        // Assert
        sale.SaleItems.Should().HaveCount(1);
        sale.SaleItems.First().ProductId.Should().Be(itemToKeep.ProductId);
        sale.SaleItems.Any(i => i.ProductId == itemToRemove.ProductId).Should().BeFalse();
    }

    [Fact(DisplayName = "UpdateItems should throw exception when updating an EXISTING item with quantity > 20")]
    public void UpdateItems_ExistingItemWithExcessiveQuantity_ThrowsDomainException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();

        sale.AddItem(productId, desc, 1, unitPrice);

        var updatedList = new List<(Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice)>
        {
            (productId, desc, 21, unitPrice)
        };

        // Act
        Action act = () => sale.UpdateItems(updatedList);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage($"Cannot sell more than 20 identical items. Product: {desc}");
    }

    [Fact(DisplayName = "CancelItem should throw exception if item is already cancelled")]
    public void CancelItem_AlreadyCancelled_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Prod", 1, 10m);
        var item = sale.SaleItems.First();
        sale.CancelItem(item.Id);

        // Act
        Action act = () => sale.CancelItem(item.Id);

        // Assert
        act.Should().Throw<DomainException>()
           .WithMessage($"Item {item.Id} is already cancelled.");
    }

    [Fact(DisplayName = "AddItem should throw exception if sale is cancelled")]
    public void AddItem_CancelledSale_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act
        Action act = () => sale.AddItem(Guid.NewGuid(), "Product", 1, 10m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot add items to a cancelled sale.");
    }

    [Fact(DisplayName = "CancelItem should throw exception if sale is cancelled")]
    public void CancelItem_CancelledSale_ThrowsException()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Product", 1, 10m);
        var itemId = sale.SaleItems.First().Id;

        sale.Cancel();

        // Act
        Action act = () => sale.CancelItem(itemId);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage("Cannot modify a cancelled sale.");
    }

}