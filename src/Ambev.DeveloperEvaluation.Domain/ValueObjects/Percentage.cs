
namespace Ambev.DeveloperEvaluation.Domain.ValueObjects;

public record struct Percentage
{
    public const string InvalidRangeErrorMessage = "Percentage must be between 0 and 1 (e.g., 0.1 for 10%).";

    public decimal Value { get; }

    public Percentage(decimal value)
    {
        if (value < 0 || value > 1)
            throw new DomainException(InvalidRangeErrorMessage);

        Value = value;
    }

    public static implicit operator Percentage(decimal value) => new(value);
    public static implicit operator decimal(Percentage percentage) => percentage.Value;
}