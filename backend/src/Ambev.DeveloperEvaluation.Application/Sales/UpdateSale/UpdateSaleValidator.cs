
using FluentValidation;

namespace Ambev.DeveloperEvaluation.Application.Sales.UpdateSale;

public class UpdateSaleValidator : AbstractValidator<UpdateSaleCommand>
{
    public UpdateSaleValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("Sale ID is required");
        RuleFor(x => x.CustomerId).NotEmpty().WithMessage("Customer ID is required");
        RuleFor(x => x.CustomerName).NotEmpty().WithMessage("Customer Name is required");
        RuleFor(x => x.Branch).NotEmpty().WithMessage("Branch is required");

        RuleFor(x => x.Items).NotEmpty().WithMessage("Sale must contain at least one item");
        RuleForEach(x => x.Items).SetValidator(new UpdateSaleItemValidator());
    }
}

public class UpdateSaleItemValidator : AbstractValidator<UpdateSaleItemCommand>
{
    public UpdateSaleItemValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty().WithMessage("Product ID is required");
        RuleFor(x => x.ProductDescription).NotEmpty().WithMessage("Product Description is required");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero");
        RuleFor(x => x.UnitPrice).GreaterThan(0).WithMessage("Unit Price must be greater than zero");
    }
}