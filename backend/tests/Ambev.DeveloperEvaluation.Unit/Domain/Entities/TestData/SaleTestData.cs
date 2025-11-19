using Ambev.DeveloperEvaluation.Domain.Entities;
using Bogus;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;

public static class SaleTestData
{
    private static readonly Faker<Sale> SaleFaker = new Faker<Sale>()
        .CustomInstantiator(f => new Sale(
            f.Random.Guid(),
            f.Person.FullName,
            f.Company.CompanyName()
        ));

    /// <summary>
    /// Gera uma venda válida inicializada.
    /// </summary>
    public static Sale GenerateValidSale()
    {
        return SaleFaker.Generate();
    }

    /// <summary>
    /// Gera parâmetros válidos para criar um item de venda.
    /// Retorna uma tupla (ProductId, ProductDescription, Quantity, UnitPrice).
    /// </summary>
    public static (Guid ProductId, string ProductDescription, int Quantity, decimal UnitPrice) GenerateItemParams(int quantity = 1)
    {
        var faker = new Faker();
        return (
            faker.Random.Guid(),
            faker.Commerce.ProductName(),
            quantity,
            faker.Random.Decimal(10, 500)
        );
    }
}