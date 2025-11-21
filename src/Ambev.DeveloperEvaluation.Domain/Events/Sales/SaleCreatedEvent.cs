namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

public record SaleCreatedEvent(Guid SaleId, Guid CustomerId, decimal TotalAmount);