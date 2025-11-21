using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.ListSales;

public class ListSalesProfileTests
{
    private readonly IMapper _mapper;

    public ListSalesProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<ListSalesProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "ListSalesProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Sale to ListSalesItemDto correctly")]
    public void Given_Sale_When_Mapped_Then_ReturnsDto()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var result = _mapper.Map<ListSalesItemDto>(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleDate.Should().Be(sale.SaleDate);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.Branch.Should().Be(sale.Branch);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        result.Status.Should().Be(sale.Status);
    }
}