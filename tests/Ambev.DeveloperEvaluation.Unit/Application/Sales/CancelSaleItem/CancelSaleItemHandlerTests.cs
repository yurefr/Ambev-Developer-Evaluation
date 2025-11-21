using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSaleItem;

/// <summary>
/// Contains unit tests for CancelSaleItemHandler.
/// </summary>
public class CancelSaleItemHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly CancelSaleItemHandler _handler;

    public CancelSaleItemHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _bus = Substitute.For<IBus>();
        _handler = new CancelSaleItemHandler(_saleRepository, _mapper, _bus);
    }

    [Fact(DisplayName = "Given valid command When handling Then cancels item and publishes event")]
    public async Task Handle_ValidCommand_CancelsItemAndReturnsResponse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var (productId, desc, _, unitPrice) = SaleTestData.GenerateItemParams();
        sale.AddItem(productId, desc, 1, unitPrice);
        var item = sale.SaleItems.First();

        var command = new CancelSaleItemCommand(sale.Id, item.Id);
        var response = new CancelSaleItemResponse { SaleId = sale.Id };

        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<CancelSaleItemResponse>(sale).Returns(response);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();

        item.IsCancelled.Should().BeTrue();

        await _saleRepository.Received(1).UpdateAsync(sale, Arg.Any<CancellationToken>());
        await _bus.Received(1).Publish(Arg.Any<SaleItemCancelledEvent>(), Arg.Any<IDictionary<string, string>>());
    }

    [Fact(DisplayName = "Given non-existing sale ID When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingSaleId_ThrowsNotFoundException()
    {
        // Arrange
        var command = new CancelSaleItemCommand(Guid.NewGuid(), Guid.NewGuid());
        _saleRepository.GetByIdAsync(command.SaleId, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = new CancelSaleItemCommand(Guid.Empty, Guid.NewGuid());

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}