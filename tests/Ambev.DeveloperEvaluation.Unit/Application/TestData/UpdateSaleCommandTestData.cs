using Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Factory for generating test data for UpdateSaleCommand.
/// </summary>
public static class UpdateSaleCommandTestData
{
    private static readonly Faker<UpdateSaleItemCommand> updateSaleItemFaker = new Faker<UpdateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 500));

    private static readonly Faker<UpdateSaleCommand> updateSaleHandlerFaker = new Faker<UpdateSaleCommand>()
        .RuleFor(c => c.Id, f => f.Random.Guid())
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.Branch, f => f.Company.CompanyName())
        .RuleFor(c => c.Items, f => updateSaleItemFaker.Generate(f.Random.Int(1, 5)));

    /// <summary>
    /// Generates a valid UpdateSaleCommand with random data.
    /// </summary>
    public static UpdateSaleCommand GenerateValidCommand()
    {
        return updateSaleHandlerFaker.Generate();
    }
}