using Ambev.DeveloperEvaluation.Application.Events;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Events;

public class SaleEventHandlerTests
{
    private readonly ILogger<SaleEventHandler> _logger;
    private readonly SaleEventHandler _handler;

    public SaleEventHandlerTests()
    {
        _logger = Substitute.For<ILogger<SaleEventHandler>>();
        _handler = new SaleEventHandler(_logger);
    }

    [Fact(DisplayName = "Should handle SaleCreatedEvent successfully")]
    public async Task Handle_SaleCreatedEvent_LogsInformation()
    {
        var @event = new SaleCreatedEvent(Guid.NewGuid(), Guid.NewGuid(), 100m);
        await _handler.Handle(@event);
    }

    [Fact(DisplayName = "Should handle SaleModifiedEvent successfully")]
    public async Task Handle_SaleModifiedEvent_LogsInformation()
    {
        var @event = new SaleModifiedEvent(Guid.NewGuid(), 200m);
        await _handler.Handle(@event);
    }

    [Fact(DisplayName = "Should handle SaleCancelledEvent successfully")]
    public async Task Handle_SaleCancelledEvent_LogsInformation()
    {
        var @event = new SaleCancelledEvent(Guid.NewGuid());
        await _handler.Handle(@event);
    }

    [Fact(DisplayName = "Should handle SaleItemCancelledEvent successfully")]
    public async Task Handle_SaleItemCancelledEvent_LogsInformation()
    {
        var @event = new SaleItemCancelledEvent(Guid.NewGuid(), Guid.NewGuid());
        await _handler.Handle(@event);
    }
}