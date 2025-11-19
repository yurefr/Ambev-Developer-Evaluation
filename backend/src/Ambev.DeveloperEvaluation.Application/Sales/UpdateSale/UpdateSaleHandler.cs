
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<UpdateSaleHandler> _logger;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<UpdateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<UpdateSaleResult> Handle(UpdateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new UpdateSaleValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = await _saleRepository.GetByIdAsync(command.Id, cancellationToken);
        if (sale == null)
            throw new KeyNotFoundException($"Sale with ID {command.Id} not found");

        sale.UpdateSaleInfo(command.CustomerId, command.CustomerName, command.Branch);

        foreach (var item in command.Items)
        {
            sale.UpdateItem(item.ProductId, item.ProductDescription, item.Quantity, item.UnitPrice);
        }

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        foreach (var domainEvent in sale.DomainEvents)
        {
            _logger.LogInformation("Domain Event Published: {EventName} - Data: {@EventData}",
               domainEvent.GetType().Name,
               domainEvent);
        }

        return _mapper.Map<UpdateSaleResult>(sale);
    }
}