using AutoMapper;
using Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Sales.CancelSaleItem;

public class CancelSaleItemProfile : Profile
{
    public CancelSaleItemProfile()
    {
        CreateMap<CancelSaleItemRequest, CancelSaleItemCommand>();

        CreateMap<Application.Sales.CancelSaleItem.CancelSaleItemResponse, CancelSaleItemResponse>();
        CreateMap<Application.Sales.CancelSaleItem.CancelSaleItemResult, CancelSaleItemResultResponse>();
    }
}