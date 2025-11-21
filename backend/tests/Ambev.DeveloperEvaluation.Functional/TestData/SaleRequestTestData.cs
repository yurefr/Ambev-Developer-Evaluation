using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Functional.TestData;

/// <summary>
/// Factory for generating API Request DTOs for Functional Tests.
/// Used to simulate HTTP request bodies.
/// </summary>
public static class SaleRequestTestData
{
    private static readonly Faker<CreateSaleItemRequest> ItemFaker = new Faker<CreateSaleItemRequest>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 10))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 100));

    private static readonly Faker<CreateSaleRequest> SaleFaker = new Faker<CreateSaleRequest>()
        .RuleFor(s => s.CustomerId, f => f.Random.Guid())
        .RuleFor(s => s.CustomerName, f => f.Person.FullName)
        .RuleFor(s => s.Branch, f => f.Company.CompanyName())
        .RuleFor(s => s.Items, f => ItemFaker.Generate(3));

    /// <summary>
    /// Generates a valid CreateSaleRequest with 3 random items.
    /// </summary>
    /// <returns>A populated CreateSaleRequest object.</returns>
    public static CreateSaleRequest GenerateValidRequest() => SaleFaker.Generate();
}