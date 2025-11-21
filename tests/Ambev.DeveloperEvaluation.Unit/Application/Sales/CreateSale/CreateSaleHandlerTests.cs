using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Events.Sales;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Application.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Rebus.Bus;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

/// <summary>
/// Contains unit tests for CreateSaleHandler.
/// </summary>
public class CreateSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;
    private readonly CreateSaleHandler _handler;

    public CreateSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _bus = Substitute.For<IBus>();
        _handler = new CreateSaleHandler(_saleRepository, _mapper, _bus);
    }

    [Fact(DisplayName = "Given valid command When handling Then creates sale, publishes event and returns result")]
    public async Task Handle_ValidCommand_ReturnsSuccessResult()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateValidCommand();
        var result = new CreateSaleResult { Id = Guid.NewGuid() };

        _saleRepository.CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<Sale>());

        _mapper.Map<CreateSaleResult>(Arg.Any<Sale>()).Returns(result);

        // Act
        var response = await _handler.Handle(command, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(result.Id);

        await _saleRepository.Received(1).CreateAsync(Arg.Is<Sale>(s =>
            s.CustomerId == command.CustomerId &&
            s.Branch == command.Branch &&
            s.SaleItems.Count == command.Items.Count),
            Arg.Any<CancellationToken>());

        await _bus.Received(1).Publish(Arg.Any<SaleCreatedEvent>(), Arg.Any<IDictionary<string, string>>());
    }

    [Fact(DisplayName = "Given invalid command When handling Then throws ValidationException")]
    public async Task Handle_InvalidCommand_ThrowsValidationException()
    {
        // Arrange
        var command = CreateSaleCommandTestData.GenerateInvalidCommand_NoItems();

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        await _saleRepository.DidNotReceive().CreateAsync(Arg.Any<Sale>(), Arg.Any<CancellationToken>());
    }
}