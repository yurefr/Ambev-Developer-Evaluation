
using AutoMapper;
using MediatR;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<CreateSaleHandler> _logger;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        ILogger<CreateSaleHandler> logger)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<CreateSaleResult> Handle(CreateSaleCommand command, CancellationToken cancellationToken)
    {
        var validator = new CreateSaleCommandValidator();
        var validationResult = await validator.ValidateAsync(command, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var sale = new Sale(command.CustomerId, command.CustomerName, command.Branch);

        foreach (var item in command.Items)
        {
            sale.AddItem(item.ProductId, item.ProductDescription, item.Quantity, item.UnitPrice);
        }

        await _saleRepository.CreateAsync(sale, cancellationToken);

        foreach (var domainEvent in sale.DomainEvents)
        {
            _logger.LogInformation("Domain Event Published: {EventName} - Data: {@EventData}",
                domainEvent.GetType().Name,
                domainEvent);
        }

        return _mapper.Map<CreateSaleResult>(sale);
    }
}