using Ambev.DeveloperEvaluation.Application.Sales.CreateSale;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Application.TestData;

/// <summary>
/// Factory for generating test data for CreateSaleCommand.
/// Uses Bogus to create realistic and varied data scenarios.
/// </summary>
public static class CreateSaleCommandTestData
{
    private static readonly Faker<CreateSaleItemCommand> createSaleItemFaker = new Faker<CreateSaleItemCommand>()
        .RuleFor(i => i.ProductId, f => f.Random.Guid())
        .RuleFor(i => i.ProductDescription, f => f.Commerce.ProductName())
        .RuleFor(i => i.Quantity, f => f.Random.Int(1, 20))
        .RuleFor(i => i.UnitPrice, f => f.Random.Decimal(10, 500));

    private static readonly Faker<CreateSaleCommand> createSaleHandlerFaker = new Faker<CreateSaleCommand>()
        .RuleFor(c => c.CustomerId, f => f.Random.Guid())
        .RuleFor(c => c.CustomerName, f => f.Person.FullName)
        .RuleFor(c => c.Branch, f => f.Company.CompanyName())
        .RuleFor(c => c.Items, f => createSaleItemFaker.Generate(f.Random.Int(1, 5)));

    /// <summary>
    /// Generates a valid CreateSaleCommand with random data.
    /// The command will contain between 1 and 5 items.
    /// </summary>
    public static CreateSaleCommand GenerateValidCommand()
    {
        return createSaleHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates an invalid CreateSaleCommand with no items.
    /// Useful for testing validation rules.
    /// </summary>
    public static CreateSaleCommand GenerateInvalidCommand_NoItems()
    {
        var command = createSaleHandlerFaker.Generate();
        command.Items.Clear();
        return command;
    }
}