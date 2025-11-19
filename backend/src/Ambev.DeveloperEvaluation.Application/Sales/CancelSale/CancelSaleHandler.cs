using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.CancelSale;

public class CancelSaleHandler : IRequestHandler<CancelSaleCommand, CancelSaleResponse>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CancelSaleHandler> _logger;

    public CancelSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CancelSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CancelSaleResponse> Handle(CancelSaleCommand request, CancellationToken cancellationToken)
    {
        var validator = new CancelSaleValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(request.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {request.Id} not found");

        sale.Cancel();

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        foreach (var domainEvent in sale.DomainEvents)
        {
            _logger.LogInformation("Domain Event Published: {EventName} - Data: {@EventData}",
               domainEvent.GetType().Name,
               domainEvent);
        }

        return _mapper.Map<CancelSaleResponse>(sale);
    }
}