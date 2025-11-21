using Ambev.DeveloperEvaluation.Application.Sales.ListSales;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Sales.ListSales;

public class ListSalesQueryTests
{
    [Fact(DisplayName = "ListSalesQuery should set and get properties correctly")]
    public void Given_Properties_When_Set_Then_ShouldReturnCorrectValues()
    {
        // Arrange
        var page = 2;
        var size = 20;
        var order = "CustomerName asc";
        var filters = new Dictionary<string, string> { { "Branch", "Main" } };

        // Act
        var query = new ListSalesQuery
        {
            Page = page,
            Size = size,
            Order = order,
            Filters = filters
        };

        // Assert
        query.Page.Should().Be(page);
        query.Size.Should().Be(size);
        query.Order.Should().Be(order);
        query.Filters.Should().BeEquivalentTo(filters);
    }

    [Fact(DisplayName = "ListSalesQuery should have default values")]
    public void Given_NewQuery_When_Created_Then_ShouldHaveDefaults()
    {
        // Act
        var query = new ListSalesQuery();

        // Assert
        query.Page.Should().Be(1);
        query.Size.Should().Be(10);
        query.Order.Should().BeNull();
        query.Filters.Should().BeNull();
    }
}