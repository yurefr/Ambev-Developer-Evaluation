
using AutoMapper;
using MediatR;
using FluentValidation;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using Rebus.Bus;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleHandler : IRequestHandler<UpdateSaleCommand, UpdateSaleResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;
    private readonly IBus _bus;

    public UpdateSaleHandler(
        ISaleRepository saleRepository,
        IMapper mapper,
        IBus bus)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
        _bus = bus;
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

        var itemsToUpdate = command.Items.Select(i =>
           (i.ProductId, i.ProductDescription, i.Quantity, i.UnitPrice));

        sale.UpdateItems(itemsToUpdate);

        await _saleRepository.UpdateAsync(sale, cancellationToken);

        foreach (var domainEvent in sale.DomainEvents)
        {
            await _bus.Publish(domainEvent);
        }

        return _mapper.Map<UpdateSaleResult>(sale);
    }
}