using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSale;

public class CancelSaleResponse
{
    public Guid Id { get; set; }
    public SaleStatus Status { get; set; }
}