using Ambev.DeveloperEvaluation.Domain.Common;
using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;

namespace Ambev.DeveloperEvaluation.Domain.Entities;

public class Sale : BaseEntity
{
    public DateTime SaleDate { get; private set; }
    public Guid CustomerId { get; private set; }
    public string CustomerName { get; private set; }
    public string Branch { get; private set; }
    public Money TotalAmount { get; private set; }
    public bool IsCancelled { get; private set; }

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
        IsCancelled = false;
        TotalAmount = 0m;

        AddDomainEvent(new SaleCreatedEvent(Id, CustomerId, TotalAmount));
    }

    /// <summary>
    /// Adds an item to the sale, applying quantity limits and discount rules.
    /// </summary>
    public void AddItem(Guid productId, string productDescription, int quantity, decimal unitPrice)
    {
        if (IsCancelled)
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

        AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
    }

    public void Cancel()
    {
        if (IsCancelled) return;

        IsCancelled = true;
        foreach (var item in _saleItems)
        {
            item.Cancel();
        }

        AddDomainEvent(new SaleCancelledEvent(Id));
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