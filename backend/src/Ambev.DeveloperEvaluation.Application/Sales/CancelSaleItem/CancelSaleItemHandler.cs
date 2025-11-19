using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSaleItem;

public class CancelSaleItemHandler : IRequestHandler<CancelSaleItemCommand, CancelSaleItemResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleItemHandler> _logger;

    public CancelSaleItemHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CancelSaleItemHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CancelSaleItemResponse> Handle(CancelSaleItemCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleItemValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.SaleId, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.SaleId} not found");

        sale.CancelItem(request.ItemId);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        foreach (var domainEvent in sale.DomainEvents)
        {
            _logger.LogInformation("Domain Event Published: {EventName} - Data: {@EventData}",
               domainEvent.GetType().Name,
               domainEvent);
        }

        return _mapper.Map<CancelSaleItemResponse>(sale);
    }
}