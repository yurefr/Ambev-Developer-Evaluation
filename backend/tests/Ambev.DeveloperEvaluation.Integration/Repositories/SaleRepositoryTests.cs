using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

/// <summary>
/// Contains integration tests for SaleRepository.
/// </summary>
public class SaleRepositoryTests
{
    private readonly DefaultContext _context;
    private readonly ISaleRepository _saleRepository;

    public SaleRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(connectionString);

        _context = new DefaultContext(builder.Options);
        _saleRepository = new SaleRepository(_context);

        _context.Database.EnsureCreated();
    }

    [Fact(DisplayName = "CreateAsync should persist sale and items in database")]
    public async Task CreateAsync_ValidSale_ShouldPersistInDatabase()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(itemCount: 3);

        // Act
        await _saleRepository.CreateAsync(sale, CancellationToken.None);

        // Assert
        var persistedSale = await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == sale.Id);

        persistedSale.Should().NotBeNull();
        persistedSale!.Id.Should().Be(sale.Id);
        persistedSale.CustomerId.Should().Be(sale.CustomerId);
        persistedSale.TotalAmount.Value.Should().Be(sale.TotalAmount.Value);

        persistedSale.SaleItems.Should().HaveCount(3);
        persistedSale.SaleItems.First().ProductDescription.Should().NotBeNullOrEmpty();
    }

    [Fact(DisplayName = "GetByIdAsync should retrieve sale with items")]
    public async Task GetByIdAsync_ExistingId_ShouldReturnSaleWithItems()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(itemCount: 2);
        await _saleRepository.CreateAsync(sale, CancellationToken.None);

        // Act
        var retrievedSale = await _saleRepository.GetByIdAsync(sale.Id, CancellationToken.None);

        // Assert
        retrievedSale.Should().NotBeNull();
        retrievedSale!.Id.Should().Be(sale.Id);
        retrievedSale.SaleItems.Should().HaveCount(2);
    }

    [Fact(DisplayName = "UpdateAsync should modify existing sale")]
    public async Task UpdateAsync_ExistingSale_ShouldUpdateDatabase()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale(itemCount: 1);
        await _saleRepository.CreateAsync(sale, CancellationToken.None);

        // Act
        sale.Cancel();
        await _saleRepository.UpdateAsync(sale, CancellationToken.None);

        // Assert
        var updatedSale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);
        updatedSale.Should().NotBeNull();
        updatedSale!.Status.Should().Be(Domain.Enums.SaleStatus.Cancelled);
    }

    [Fact(DisplayName = "DeleteAsync should remove sale from database")]
    public async Task DeleteAsync_ExistingId_ShouldRemoveSale()
    {
        // Arrange
        var sale = SaleTestData.GenerateValidSale();
        await _saleRepository.CreateAsync(sale, CancellationToken.None);

        // Act
        var result = await _saleRepository.DeleteAsync(sale.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
        var deletedSale = await _context.Sales.FirstOrDefaultAsync(s => s.Id == sale.Id);
        deletedSale.Should().BeNull();
    }
}