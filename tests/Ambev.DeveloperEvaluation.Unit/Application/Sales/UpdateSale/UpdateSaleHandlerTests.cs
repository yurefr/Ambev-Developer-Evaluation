using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

/// <summary>
/// Contains unit tests for UpdateSaleHandler.
/// </summary>
public class UpdateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly UpdateSaleHandler _handler;

    public UpdateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _bus = Substitute.For<IBus>();
        _handler = new UpdateSaleHandler(_saleRepository, _mapper, _bus);
    }

    [Fact(DisplayName = "Given valid command When handling Then updates sale, publishes event and returns result")]
    public async Task Handle_ValidCommand_UpdatesSaleAndReturnsResult()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        var existingSale = SaleTestData.GenerateValidSale();

        command.Id = existingSale.Id;
        var result = new UpdateSaleResult { Id = command.Id };

        _saleRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>()).Returns(existingSale);
        _saleRepository.UpdateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>()).Returns(existingSale);
        _mapper.Map<UpdateSaleResult>(existingSale).Returns(result);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(command.Id);

        await _saleRepository.Received(1).UpdateAsync(Arg.Is<Sale>(s => s.Id == command.Id), Arg.Any<CancellationToken>());

        await _bus.Received(1).Publish(Arg.Any<SaleModifiedEvent>(), Arg.Any<IDictionary<string, string>>());
    }

    [Fact(DisplayName = "Given non-existing ID When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
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
        var command = UpdateSaleCommandTestData.GenerateValidCommand();
        command.Id = Guid.Empty;

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}