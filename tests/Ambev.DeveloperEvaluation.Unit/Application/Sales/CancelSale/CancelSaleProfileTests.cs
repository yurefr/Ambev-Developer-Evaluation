using Ambev.DeveloperEvaluation.Application.Sales.CancelSale;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSale;

public class CancelSaleProfileTests
{
    private readonly IMapper _mapper;

    public CancelSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CancelSaleProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "CancelSaleProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Sale to CancelSaleResponse correctly")]
    public void Given_Sale_When_Mapped_Then_ReturnsCancelSaleResponse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.Cancel();

        // Act
        var result = _mapper.Map<CancelSaleResponse>(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.Status.Should().Be(SaleStatus.Cancelled);
    }
}