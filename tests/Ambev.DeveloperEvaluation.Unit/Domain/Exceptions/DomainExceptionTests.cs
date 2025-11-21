using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Domain.Exceptions;

public class DomainExceptionTests
{
    [Fact(DisplayName = "Constructor with message should set Message property")]
    public void Constructor_Message_SetsProperty()
    {
        var message = "Error occurred";
        var exception = new DomainException(message);
        exception.Message.Should().Be(message);
    }

    [Fact(DisplayName = "Constructor with message and inner exception should set properties")]
    public void Constructor_MessageAndInner_SetsProperties()
    {
        var message = "Error occurred";
        var inner = new Exception("Inner error");

        var exception = new DomainException(message, inner);

        exception.Message.Should().Be(message);
        exception.InnerException.Should().Be(inner);
    }
}