using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Ambev.DeveloperEvaluation.ORM.Repositories;

/// <summary>
/// Implementation of ISaleRepository using Entity Framework Core
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
            .Include(s => s.SaleItems)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        _context.Sales.Remove(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<(IEnumerable<Sale> Sales, int TotalCount)> GetAllAsync(int page, int size, string? order, CancellationToken cancellationToken = default)
    {
        var query = _context.Sales.AsNoTracking();
        var totalCount = await query.CountAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(order))
        {
            query = query.OrderByDescending(s => s.SaleDate);
        }
        else
        {
            var parts = order.Trim().Split(' ');
            var property = parts[0].ToLower();
            var isDesc = parts.Length > 1 && parts[1].ToLower() == "desc";

            query = property switch
            {
                "saledate" => isDesc ? query.OrderByDescending(s => s.SaleDate) : query.OrderBy(s => s.SaleDate),
                "customername" => isDesc ? query.OrderByDescending(s => s.CustomerName) : query.OrderBy(s => s.CustomerName),
                "totalamount" => isDesc ? query.OrderByDescending(s => s.TotalAmount) : query.OrderBy(s => s.TotalAmount),
                "status" => isDesc ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
                _ => query.OrderByDescending(s => s.SaleDate)
            };
        }

        var sales = await query
            .Skip((page - 1) * size)
            .Take(size)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }
}