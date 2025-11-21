using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;
using Rebus.Handlers;

namespace Ambev.DeveloperEvaluation.Application.Events;

/// <summary>
/// Handler responsible for catching domain events related to Sales and logging them.
/// This simulates a subscriber in an Event-Driven Architecture.
/// </summary>
public class SaleEventHandler :
    IHandleMessages<SaleCreatedEvent>,
    IHandleMessages<SaleModifiedEvent>,
    IHandleMessages<SaleCancelledEvent>,
    IHandleMessages<SaleItemCancelledEvent>
{
    private readonly ILogger<SaleEventHandler> _logger;

    public SaleEventHandler(ILogger<SaleEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(SaleCreatedEvent message)
    {
        _logger.LogInformation("Event Received: SaleCreated | ID: {SaleId} | Customer: {CustomerId} | Amount: {TotalAmount}",
            message.SaleId, message.CustomerId, message.TotalAmount);
        return Task.CompletedTask;
    }

    public Task Handle(SaleModifiedEvent message)
    {
        _logger.LogInformation("Event Received: SaleModified | ID: {SaleId} | New Amount: {NewTotalAmount}",
            message.SaleId, message.NewTotalAmount);
        return Task.CompletedTask;
    }

    public Task Handle(SaleCancelledEvent message)
    {
        _logger.LogInformation("Event Received: SaleCancelled | ID: {SaleId}", message.SaleId);
        return Task.CompletedTask;
    }

    public Task Handle(SaleItemCancelledEvent message)
    {
        _logger.LogInformation("Event Received: SaleItemCancelled | SaleID: {SaleId} | ProductID: {ProductId}",
            message.SaleId, message.ProductId);
        return Task.CompletedTask;
    }
}