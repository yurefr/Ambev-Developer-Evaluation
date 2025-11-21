
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleProfile : Profile
{
    public UpdateSaleProfile()
    {
        CreateMap<Sale, UpdateSaleResult>()
            .ForMember(dest => dest.Items, opt => opt.MapFrom(src => src.SaleItems))
            .ForMember(dest => dest.IsCancelled, opt => opt.MapFrom(src => src.Status == SaleStatus.Cancelled));

        CreateMap<SaleItem, UpdateSaleItemResult>();
    }
}