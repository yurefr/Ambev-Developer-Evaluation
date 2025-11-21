using Ambev.DeveloperEvaluation.Domain.Common;
using FluentAssertions;
using FluentValidation;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Common;

public class BaseEntityTests
{
    public class TestEntity : BaseEntity
    {
        public void AddEvent(object @event) => AddDomainEvent(@event);
        public void ClearEvents() => ClearDomainEvents();
    }

    public class TestEntityValidator : AbstractValidator<TestEntity>
    {
        public TestEntityValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }

    [Fact(DisplayName = "Id should be initialized automatically")]
    public void Id_Initialized_Automatically()
    {
        // Arrange & Act
        var entity = new TestEntity();

        // Assert
        entity.Id.Should().NotBeEmpty();
    }

    [Fact(DisplayName = "CompareTo should return 1 when other is null")]
    public void CompareTo_Null_ReturnsOne()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        var result = entity.CompareTo(null);

        // Assert
        result.Should().Be(1);
    }

    [Fact(DisplayName = "CompareTo should compare based on Id")]
    public void CompareTo_OtherEntity_ComparesIds()
    {
        // Arrange
        var entity1 = new TestEntity { Id = Guid.Parse("00000000-0000-0000-0000-000000000001") };
        var entity2 = new TestEntity { Id = Guid.Parse("00000000-0000-0000-0000-000000000002") };

        // Act
        var result1 = entity1.CompareTo(entity2);
        var result2 = entity2.CompareTo(entity1);
        var resultEqual = entity1.CompareTo(entity1);

        // Assert
        result1.Should().BeNegative();
        result2.Should().BePositive();
        resultEqual.Should().Be(0);
    }

    [Fact(DisplayName = "DomainEvents should be managed correctly")]
    public void DomainEvents_Management_Works()
    {
        // Arrange
        var entity = new TestEntity();
        var @event = new { Name = "TestEvent" };

        // Act
        entity.AddEvent(@event);
        entity.ClearEvents();

        // Assert
        entity.DomainEvents.Should().BeEmpty();
    }

    [Fact(DisplayName = "ValidateAsync should execute validation using the found validator")]
    public async Task ValidateAsync_ExecutesValidator()
    {
        // Arrange
        var entity = new TestEntity();

        // Act
        var result = await entity.ValidateAsync();

        // Assert
        result.Should().BeEmpty();
    }
}
