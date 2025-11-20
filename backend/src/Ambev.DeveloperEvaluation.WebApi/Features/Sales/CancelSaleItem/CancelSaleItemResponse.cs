using Ambev.DeveloperEvaluation.Domain.Enums;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

public class CancelSaleItemResponse
{
    public Guid SaleId { get; set; }
    public decimal NewTotalAmount { get; set; }
    public SaleStatus Status { get; set; }
    public List<CancelSaleItemResultResponse> Items { get; set; } = new();
}

public class CancelSaleItemResultResponse
{
    public Guid ProductId { get; set; }
    public string ProductDescription { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalAmount { get; set; }
    public bool IsCancelled { get; set; }
}