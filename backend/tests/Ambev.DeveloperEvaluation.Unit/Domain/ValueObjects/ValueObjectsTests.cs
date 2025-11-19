using Ambev.DeveloperEvaluation.Domain.ValueObjects;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.ValueObjects;

public class ValueObjectsTests
{
    [Fact(DisplayName = "Money should throw exception when value is negative")]
    public void Money_NegativeValue_ThrowsException()
    {
        // Arrange & Act
        Action act = () => _ = new Money(-10m);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage(Money.NegativeValueErrorMessage);
    }

    [Fact(DisplayName = "Quantity should throw exception when value is zero or negative")]
    public void Quantity_ZeroOrNegative_ThrowsException()
    {
        // Arrange & Act
        Action actZero = () => _ = new Quantity(0);
        Action actNegative = () => _ = new Quantity(-1);

        // Assert
        actZero.Should().Throw<DomainException>()
            .WithMessage(Quantity.InvalidValueErrorMessage);

        actNegative.Should().Throw<DomainException>()
            .WithMessage(Quantity.InvalidValueErrorMessage);
    }

    [Theory(DisplayName = "Percentage should throw exception when value is out of range")]
    [InlineData(-0.1)]
    [InlineData(1.1)]
    public void Percentage_OutOfRange_ThrowsException(decimal value)
    {
        // Arrange & Act
        Action act = () => _ = new Percentage(value);

        // Assert
        act.Should().Throw<DomainException>()
            .WithMessage(Percentage.InvalidRangeErrorMessage);
    }
}