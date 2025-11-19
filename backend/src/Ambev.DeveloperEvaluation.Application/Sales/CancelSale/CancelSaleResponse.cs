using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleResponse
{
    public Guid Id { get; set; }
    public SaleStatus Status { get; set; }
}