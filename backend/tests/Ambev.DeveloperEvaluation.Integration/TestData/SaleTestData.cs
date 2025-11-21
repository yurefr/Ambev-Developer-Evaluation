using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData;

/// <summary>
/// Factory for generating Sale entities for Integration Tests.
/// These entities are designed to be persisted directly to the PostgreSQL database.
/// </summary>
public static class SaleTestData
{
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Guid(),
            f.Person.FullName,
            f.Company.CompanyName()
        ));

    /// <summary>
    /// Generates a valid Sale entity with a specified number of items.
    /// </summary>
    /// <param name="itemCount">The number of items to add to the sale. Defaults to 1.</param>
    /// <returns>A Sale entity ready for persistence.</returns>
    public static Sale GenerateValidSale(int itemCount = 1)
    {
        var sale = SaleFaker.Generate();
        var faker = new Faker();

        for (int i = 0; i < itemCount; i++)
        {
            sale.AddItem(
                faker.Random.Guid(),
                faker.Commerce.ProductName(),
                faker.Random.Int(1, 5),
                faker.Random.Decimal(10, 100)
            );
        }

        return sale;
    }
}