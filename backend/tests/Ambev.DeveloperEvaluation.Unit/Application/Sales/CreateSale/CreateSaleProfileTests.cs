using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.CreateSale;

public class CreateSaleProfileTests
{
    private readonly IMapper _mapper;

    public CreateSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CreateSaleProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "CreateSaleProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Sale to CreateSaleResult correctly")]
    public void Given_Sale_When_Mapped_Then_ReturnsResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Test Product", 2, 50m);

        // Act
        var result = _mapper.Map<CreateSaleResult>(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
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