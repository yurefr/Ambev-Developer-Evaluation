
using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleHandler : IRequestHandler<CreateSaleCommand, CreateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;

    public CreateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IBus bus)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _bus = bus;
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
            await _bus.Publish(domainEvent);
        }

        return _mapper.Map<CreateSaleResult>(sale);
    }
}