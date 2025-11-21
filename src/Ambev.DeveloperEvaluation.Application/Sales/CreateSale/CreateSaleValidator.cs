
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.CreateSale;

public class CreateSaleCommandValidator : AbstractValidator<CreateSaleCommand>
{
    public CreateSaleCommandValidator()
    {
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name is required");
        RuleFor(x => x.Branch).NotEmpty().WithMessage("Branch is required");

        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Sale must contain at least one item");

        RuleForEach(x => x.Items).SetValidator(new CreateSaleItemValidator());
    }
}

public class CreateSaleItemValidator : AbstractValidator<CreateSaleItemCommand>
{
    public CreateSaleItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required");
        RuleFor(x => x.ProductDescription).NotEmpty().WithMessage("Product Description is required");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit Price must be greater than zero");
    }
}