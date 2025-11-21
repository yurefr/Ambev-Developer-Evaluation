
namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record struct Money
{
    public const string NegativeValueErrorMessage = "Money value cannot be negative.";

    public decimal Value { get; }

    public Money(decimal value)
    {
        if (value < 0)
            throw new DomainException(NegativeValueErrorMessage);

        Value = value;
    }

    public static implicit operator Money(decimal value) => new(value);

    public static implicit operator decimal(Money money) => money.Value;
}