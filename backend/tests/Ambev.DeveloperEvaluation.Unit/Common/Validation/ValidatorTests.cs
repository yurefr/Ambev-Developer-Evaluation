using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Validation;

public class ValidatorTests
{
    public class TestEntity
    {
        public string Name { get; set; } = string.Empty;
    }

    public class TestEntityValidator : AbstractValidator<TestEntity>
    {
        public TestEntityValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required");
        }
    }

    public class EntityWithoutValidator { }

    [Fact(DisplayName = "ValidateAsync should return errors for invalid entity")]
    public async Task ValidateAsync_InvalidEntity_ReturnsErrors()
    {
        // Arrange
        var entity = new TestEntity { Name = "" };

        // Act
        var errors = await Validator.ValidateAsync(entity);

        // Assert
        errors.Should().NotBeEmpty();
        errors.First().Detail.Should().Be("Name is required");
    }

    [Fact(DisplayName = "ValidateAsync should return empty for valid entity")]
    public async Task ValidateAsync_ValidEntity_ReturnsEmpty()
    {
        // Arrange
        var entity = new TestEntity { Name = "Valid Name" };

        // Act
        var errors = await Validator.ValidateAsync(entity);

        // Assert
        errors.Should().BeEmpty();
    }

    [Fact(DisplayName = "ValidateAsync should throw InvalidOperationException when no validator is found")]
    public async Task ValidateAsync_NoValidatorFound_ThrowsException()
    {
        // Arrange
        var entity = new EntityWithoutValidator();

        // Act
        Func<Task> act = async () => await Validator.ValidateAsync(entity);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"No validator found for type {nameof(EntityWithoutValidator)}");
    }
}