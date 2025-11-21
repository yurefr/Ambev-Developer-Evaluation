
namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record struct Quantity
{
    public const string InvalidValueErrorMessage = "Quantity must be greater than zero.";

    public int Value { get; }

    public Quantity(int value)
    {
        if (value <= 0)
            throw new DomainException(InvalidValueErrorMessage);

        Value = value;
    }

    public static implicit operator Quantity(int value) => new(value);
    public static implicit operator int(Quantity quantity) => quantity.Value;
}