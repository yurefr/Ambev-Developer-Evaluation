using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.UpdateSale;

public class UpdateSaleProfileTests
{
    private readonly IMapper _mapper;

    public UpdateSaleProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<UpdateSaleProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "UpdateSaleProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Sale to UpdateSaleResult correctly including items")]
    public void Given_Sale_When_Mapped_Then_ReturnsCompleteResult()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        sale.AddItem(Guid.NewGuid(), "Updated Product", 3, 15.50m);

        // Act
        var result = _mapper.Map<UpdateSaleResult>(sale);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(sale.Id);
        result.CustomerId.Should().Be(sale.CustomerId);
        result.CustomerName.Should().Be(sale.CustomerName);
        result.Branch.Should().Be(sale.Branch);
        result.TotalAmount.Should().Be(sale.TotalAmount);
        result.IsCancelled.Should().Be(sale.Status == SaleStatus.Cancelled);

        // Assert Items
        result.Items.Should().HaveCount(sale.SaleItems.Count);

        var itemResult = result.Items.First(i => i.ProductDescription == "Updated Product");
        var itemEntity = sale.SaleItems.First(i => i.ProductDescription == "Updated Product");

        itemResult.ProductId.Should().Be(itemEntity.ProductId);
        itemResult.ProductDescription.Should().Be(itemEntity.ProductDescription);
        itemResult.Quantity.Should().Be(itemEntity.Quantity);
        itemResult.UnitPrice.Should().Be(itemEntity.UnitPrice);
        itemResult.Discount.Should().Be(itemEntity.Discount);
        itemResult.TotalAmount.Should().Be(itemEntity.TotalAmount);
        itemResult.IsCancelled.Should().Be(itemEntity.IsCancelled);
    }
}