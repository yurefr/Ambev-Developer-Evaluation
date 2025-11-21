using Ambev.DeveloperEvaluation.Domain.Entities;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities;

public class SaleItemTests
{
    [Fact(DisplayName = "Constructor should calculate total correctly without discount")]
    public void Constructor_CalculatesTotal_NoDiscount()
    {
        var quantity = 2;
        var price = 10m;
        var item = new SaleItem(Guid.NewGuid(), "Product", quantity, price);

        item.Discount.Value.Should().Be(0);
        item.TotalAmount.Value.Should().Be(20m);
        item.IsCancelled.Should().BeFalse();
    }

    [Fact(DisplayName = "SetDiscount should recalculate total amount")]
    public void SetDiscount_RecalculatesTotal()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", 10, 10m);

        item.SetDiscount(0.10m);

        item.Discount.Value.Should().Be(0.10m);
        item.TotalAmount.Value.Should().Be(90m);
    }

    [Fact(DisplayName = "Cancel should set IsCancelled to true")]
    public void Cancel_SetsIsCancelled()
    {
        var item = new SaleItem(Guid.NewGuid(), "Product", 1, 10m);

        item.Cancel();

        item.IsCancelled.Should().BeTrue();
    }

    [Fact(DisplayName = "Protected constructor should initialize instance")]
    public void ProtectedConstructor_InitializesInstance()
    {
        var item = (SaleItem)Activator.CreateInstance(typeof(SaleItem), true)!;
        item.Should().NotBeNull();
    }
}