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
    /// Adds an item to the sale.
    /// </summary>
    public void AddItem(Guid productId, string productDescription, int quantity, decimal unitPrice)
    {
        AddItemInternal(productId, productDescription, quantity, unitPrice);

        UpdatedAt = DateTime.UtcNow;
        AddDomainEvent(new SaleModifiedEvent(Id, TotalAmount));
    }

    private void AddItemInternal(Guid productId, string productDescription, int quantity, decimal unitPrice)
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
    }

    /// <summary>
    /// Updates the sale's items based on a new collection.
    /// </summary>
    public void UpdateItems(IEnumerable<(Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice)> updatedItems)
    {
        if (Status == SaleStatus.Cancelled)
            throw new DomainException("Cannot modify a cancelled sale.");

        var updatedItemsList = updatedItems.ToList();
        var productIdsInUpdate = updatedItemsList.Select(i => i.ProductId).ToHashSet();

        var itemsToRemove = _saleItems.Where(i => !productIdsInUpdate.Contains(i.ProductId)).ToList();
        foreach (var item in itemsToRemove)
        {
            _saleItems.Remove(item);
        }

        foreach (var itemDto in updatedItemsList)
        {
            var existingItem = _saleItems.FirstOrDefault(i => i.ProductId == itemDto.ProductId);

            if (existingItem != null)
            {
                if (itemDto.Quantity > 20)
                    throw new DomainException($"Cannot sell more than 20 identical items. Product: {itemDto.ProductDescription}");

                existingItem.UpdateDetails(itemDto.Quantity, itemDto.UnitPrice, itemDto.ProductDescription);
                var discount = CalculateDiscount(itemDto.Quantity);
                existingItem.SetDiscount(discount);
            }
            else
            {
                AddItemInternal(itemDto.ProductId, itemDto.ProductDescription, itemDto.Quantity, itemDto.UnitPrice);
            }
        }

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
            if (!item.IsCancelled)
            {
                total += (decimal)item.TotalAmount;
            }
        }
        TotalAmount = new Money(total);
    }

    private Percentage CalculateDiscount(int quantity)
    {
        if (quantity >= 10)
            return 0.20m;

        if (quantity >= 4)
            return 0.10m;

        return 0m;
    }
}