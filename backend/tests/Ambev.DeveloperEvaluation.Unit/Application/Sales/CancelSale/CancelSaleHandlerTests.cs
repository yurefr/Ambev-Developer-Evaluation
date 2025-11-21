using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSale;

/// <summary>
/// Contains unit tests for CancelSaleHandler.
/// </summary>
public class CancelSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly CancelSaleHandler _handler;

    public CancelSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _bus = Substitute.For<IBus>();
        _handler = new CancelSaleHandler(_saleRepository, _mapper, _bus);
    }

    [Fact(DisplayName = "Given valid command When handling Then cancels sale and publishes event")]
    public async Task Handle_ValidCommand_CancelsSaleAndReturnsResponse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var command = new CancelSaleCommand(sale.Id);
        var response = new CancelSaleResponse { Id = sale.Id, Status = SaleStatus.Cancelled };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelSaleResponse>(sale).Returns(response);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(SaleStatus.Cancelled);

        sale.Status.Should().Be(SaleStatus.Cancelled);

        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _bus.Received(1).Publish(Arg.Any<SaleCancelledEvent>(), Arg.Any<IDictionary<string, string>>());
    }

    [Fact(DisplayName = "Given non-existing ID When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var command = new CancelSaleCommand(Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new CancelSaleCommand(Guid.Empty);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}