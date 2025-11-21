using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using FluentValidation;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.ListSales;

/// <summary>
/// Contains unit tests for ListSalesHandler.
/// </summary>
public class ListSalesHandlerTests
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ListSalesHandler _handler;

    public ListSalesHandlerTests()
    {
        _saleRepository = Substitute.For<ISaleRepository>();
        _mapper = Substitute.For<IMapper>();
        _handler = new ListSalesHandler(_saleRepository, _mapper);
    }

    [Fact(DisplayName = "Given valid request When handling Then returns paged sales result")]
    public async Task Handle_ValidRequest_ReturnsPagedResult()
    {
        // Arrange
        var query = new ListSalesQuery { Page = 1, Size = 10 };
        var sales = new List<Sale> { SaleTestData.GenerateValidSale() };
        var totalCount = 1;

        _saleRepository.GetAllAsync(query.Page, query.Size, query.Order, Arg.Any<CancellationToken>())
            .Returns((sales, totalCount));

        _mapper.Map<List<ListSalesItemDto>>(sales).Returns(sales.Select(s => new ListSalesItemDto { Id = s.Id }).ToList());

        // Act
        var response = await _handler.Handle(query, CancellationToken.None);

        // Assert
        response.Should().NotBeNull();
        response.Data.Should().HaveCount(1);
        response.TotalItems.Should().Be(totalCount);
        response.CurrentPage.Should().Be(query.Page);
    }

    [Fact(DisplayName = "Given invalid page size When handling Then throws ValidationException")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Arrange
        var query = new ListSalesQuery { Page = 1, Size = 0 };

        // Act
        var act = () => _handler.Handle(query, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
    }
}