using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class SaleItem : BaseEntity
{
    public Guid ProductId { get; private set; }
    public string ProductDescription { get; private set; }
    public Quantity Quantity { get; private set; }
    public Money UnitPrice { get; private set; }
    public Percentage Discount { get; private set; }
    public Money TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

    protected SaleItem() { }

    public SaleItem(Guid productId, string productDescription, Quantity quantity, Money unitPrice)
    {
        ProductId = productId;
        ProductDescription = productDescription;
        Quantity = quantity;
        UnitPrice = unitPrice;
        Discount = 0m;
        IsCancelled = false;

        CalculateTotal();
    }

    /// <summary>
    /// Sets the item discount and recalculates the total amount.
    /// </summary>
    public void SetDiscount(Percentage discount)
    {
        Discount = discount;
        CalculateTotal();
    }

    public void Cancel()
    {
        IsCancelled = true;
    }

    /// <summary>
    /// Updates the details of an existing sale item.
    /// </summary>
    public void UpdateDetails(Quantity newQuantity, Money newUnitPrice, string newProductDescription)
    {
        Quantity = newQuantity;
        UnitPrice = newUnitPrice;
        ProductDescription = newProductDescription;

        CalculateTotal();
    }

    private void CalculateTotal()
    {
        decimal total = (decimal)Quantity * (decimal)UnitPrice;
        decimal discountAmount = total * (decimal)Discount;
        TotalAmount = new Money(total - discountAmount);
    }
}