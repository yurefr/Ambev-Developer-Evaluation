namespace Ambev.DeveloperEvaluation.Domain.Events.Sales;

public record SaleModifiedEvent(Guid SaleId, decimal NewTotalAmount);