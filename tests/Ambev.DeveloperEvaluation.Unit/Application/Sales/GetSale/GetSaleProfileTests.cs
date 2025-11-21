using Ambev.DeveloperEvaluation.Application.Sales.GetSale;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.GetSale;

public class GetSaleProfileTests
{
    private readonly IMapper _mapper;

    public GetSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<GetSaleProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "GetSaleProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Sale to GetSaleResult correctly including items")]
    public void Given_Sale_When_Mapped_Then_ReturnsCompleteResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Test Product", 5, 100m);

        // Act
        var result = _mapper.Map<GetSaleResult>(sale);

        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.SaleDate.Should().Be(sale.SaleDate);
        result.CustomerId.Should().Be(sale.CustomerId);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.Branch.Should().Be(sale.Branch);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        result.Status.Should().Be(sale.Status);

        result.Items.Should().HaveCount(sale.SaleItems.Count);

        var itemResult = result.Items.First(i => i.ProductDescription == "Test Product");
        var itemEntity = sale.SaleItems.First(i => i.ProductDescription == "Test Product");

        itemResult.ProductId.Should().Be(itemEntity.ProductId);
        itemResult.ProductDescription.Should().Be(itemEntity.ProductDescription);
        itemResult.Quantity.Should().Be(itemEntity.Quantity);
        itemResult.UnitPrice.Should().Be(itemEntity.UnitPrice);
        itemResult.Discount.Should().Be(itemEntity.Discount);
        itemResult.TotalAmount.Should().Be(itemEntity.TotalAmount);
        itemResult.IsCancelled.Should().Be(itemEntity.IsCancelled);
    }
}