using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

/// <summary>
/// Contains unit tests for GetSaleHandler.
/// </summary>
public class GetSaleHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly GetSaleHandler _handler;

    public GetSaleHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new GetSaleHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid ID When handling Then returns sale result")]
    public async Task Handle_ValidId_ReturnsSaleResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        var query = new GetSaleQuery(sale.Id);
        var result = new GetSaleResult { Id = sale.Id };

        _saleRepository.GetByIdAsync(sale.Id, Arg.Any<CancellationToken>()).Returns(sale);
        _mapper.Map<GetSaleResult>(sale).Returns(result);

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Id.Should().Be(sale.Id);
        await _saleRepository.Received(1).GetByIdAsync(sale.Id, Arg.Any<CancellationToken>());
    }

    [Fact(DisplayName = "Given non-existing ID When handling Then throws KeyNotFoundException")]
    public async Task Handle_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var query = new GetSaleQuery(Guid.NewGuid());
        _saleRepository.GetByIdAsync(query.Id, Arg.Any<CancellationToken>()).Returns((Sale?)null);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact(DisplayName = "Given empty ID When handling Then throws ValidationException")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var query = new GetSaleQuery(Guid.Empty);

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}