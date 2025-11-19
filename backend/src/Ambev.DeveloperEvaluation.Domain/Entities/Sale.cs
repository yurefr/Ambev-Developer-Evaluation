using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public DateTime SaleDate { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public string Branch { get; private set; }
    public Money TotalAmount { get; private set; }
    public SaleStatus Status { get; private set; }

    private readonly List<SaleItem> _saleItems = new();
    public IReadOnlyCollection<SaleItem> SaleItems => _saleItems.AsReadOnly();

    protected Sale() { }

    public Sale(Guid customerId, string customerName, string branch)
    {
        Id = Guid.NewGuid();
        SaleDate = DateTime.UtcNow;
        CustomerId = customerId;
        CustomerName = customerName;
        Branch = branch;
        Status = SaleStatus.Active;
        TotalAmount = 0m;

        AddDomainEvent(new SaleCreatedEvent(Id, CustomerId, TotalAmount));
    }

    /// <summary>
    /// Adds an item to the sale, applying quantity limits and discount rules.
    /// </summary>
    public void AddItem(Guid productId, string productDescription, int quantity, decimal unitPrice)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot add items to a cancelled sale.");

        var existingItem = _saleItems.FirstOrDefault(i => i.ProductId == productId);
        var currentQuantity = existingItem != null ? (int)existingItem.Quantity : 0;
        var newQuantity = currentQuantity + quantity;

        if (newQuantity > 20)
            throw new DomainException($"Cannot sell more than 20 identical items. Product: {productDescription}");

        var discount = CalculateDiscount(newQuantity);

        if (existingItem != null)
        {
            _saleItems.Remove(existingItem);
        }

        var newItem = new SaleItem(productId, productDescription, newQuantity, unitPrice);
        newItem.SetDiscount(discount);

        _saleItems.Add(newItem);

        UpdateTotal();

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
    }

    /// <summary>
    /// Updates an existing item or adds it if it doesn't exist.
    /// Replaces the quantity instead of accumulating.
    /// </summary>
    public void UpdateItem(Guid productId, string productDescription, int quantity, decimal unitPrice)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot modify a cancelled sale.");

        if (quantity > 20)
            throw new DomainException($"Cannot sell more than 20 identical items. Product: {productDescription}");

        var existingItem = _saleItems.FirstOrDefault(i => i.ProductId == productId);

        if (existingItem != null)
        {
            _saleItems.Remove(existingItem);
        }

        var discount = CalculateDiscount(quantity);
        var newItem = new SaleItem(productId, productDescription, new Quantity(quantity), new Money(unitPrice));
        newItem.SetDiscount(discount);

        _saleItems.Add(newItem);

        UpdateTotal();

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
    }

    public void UpdateSaleInfo(Guid customerId, string customerName, string branch)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot modify a cancelled sale.");

        CustomerId = customerId;
        CustomerName = customerName;
        Branch = branch;

        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        if (Status == SaleStatus.Cancelled) return;

        Status = SaleStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;

        foreach (var item in _saleItems)
        {
            item.Cancel();
        }

        AddDomainEvent(new SaleCancelledEvent(Id));
    }

    /// <summary>
    /// Cancels a specific item in the sale and recalculates the total amount.
    /// </summary>
    /// <param name="itemId">The unique identifier of the sale item to cancel.</param>
    public void CancelItem(Guid itemId)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot modify a cancelled sale.");

        var item = _saleItems.FirstOrDefault(i => i.Id == itemId);
        if (item == null)
            throw new DomainException($"Sale item with ID {itemId} not found in sale {Id}.");

        if (item.IsCancelled)
            throw new DomainException($"Item {itemId} is already cancelled.");

        item.Cancel();
        UpdateTotal();

        UpdatedAt = DateTime.UtcNow;

        AddDomainEvent(new SaleItemCancelledEvent(Id, item.ProductId));
    }

    private void UpdateTotal()
    {
        decimal total = 0;
        foreach (var item in _saleItems)
        {
            total += (decimal)item.TotalAmount;
        }
        TotalAmount = new Money(total);
    }

    private Percentage CalculateDiscount(int quantity)
    {
        if (quantity < 4)
            return 0m;

        if (quantity >= 4 && quantity < 10)
            return 0.10m;

        if (quantity >= 10 && quantity <= 20)
            return 0.20m;

        return 0m;
    }
}