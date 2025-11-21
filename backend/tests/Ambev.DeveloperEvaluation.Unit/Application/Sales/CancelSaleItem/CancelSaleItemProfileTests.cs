using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CancelSaleItem;

public class CancelSaleItemProfileTests
{
    private readonly IMapper _mapper;

    public CancelSaleItemProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CancelSaleItemProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "CancelSaleItemProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Sale to CancelSaleItemResponse correctly")]
    public void Given_Sale_When_Mapped_Then_ReturnsResponse()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();

        // Act
        var response = _mapper.Map<CancelSaleItemResponse>(sale);

        // Assert
        response.Should().NotBeNull();
        response.SaleId.Should().Be(sale.Id);
        response.NewTotalAmount.Should().Be(sale.TotalAmount);
        response.Status.Should().Be(sale.Status);
        response.Items.Should().HaveCount(sale.SaleItems.Count);
    }

    [Fact(DisplayName = "Should map SaleItem to CancelSaleItemResult correctly")]
    public void Given_SaleItem_When_Mapped_Then_ReturnsResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Test Product", 1, 10m);
        var item = sale.SaleItems.First();

        // Act
        var result = _mapper.Map<CancelSaleItemResult>(item);

        // Assert
        result.Should().NotBeNull();
        result.ProductId.Should().Be(item.ProductId);
        result.ProductDescription.Should().Be(item.ProductDescription);
        result.Quantity.Should().Be(item.Quantity);
        result.UnitPrice.Should().Be(item.UnitPrice);
        result.TotalAmount.Should().Be(item.TotalAmount);
        result.IsCancelled.Should().Be(item.IsCancelled);
    }
}