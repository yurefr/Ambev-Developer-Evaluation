using Ambev.DeveloperEvaluation.Domain.Events;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Events;

public class UserRegisteredEventTests
{
    [Fact(DisplayName = "Constructor should set User property")]
    public void Constructor_SetsUser()
    {
        var user = UserTestData.GenerateValidUser();
        var @event = new UserRegisteredEvent(user);

        @event.User.Should().Be(user);
    }
}