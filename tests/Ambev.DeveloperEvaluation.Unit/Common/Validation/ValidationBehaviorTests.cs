using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Validation;

public class ValidationBehaviorTests
{
    public class TestRequest : IRequest<TestResponse> { }
    public class TestResponse { }

    [Fact(DisplayName = "Handle should call next when validation succeeds")]
    public async Task Handle_ValidationSuccess_CallsNext()
    {
        // Arrange
        var validators = new List<IValidator<TestRequest>>();
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        next.Invoke().Returns(new TestResponse());

        // Act
        await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await next.Received(1).Invoke();
    }

    [Fact(DisplayName = "Handle should throw ValidationException when validation fails")]
    public async Task Handle_ValidationFailure_ThrowsException()
    {
        // Arrange
        var validator = Substitute.For<IValidator<TestRequest>>();
        var validationResult = new ValidationResult(new[] { new ValidationFailure("Prop", "Error") });
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(validationResult);

        var validators = new List<IValidator<TestRequest>> { validator };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();

        // Act
        Func<Task> act = async () => await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<ValidationException>();
        await next.DidNotReceive().Invoke();
    }

    [Fact(DisplayName = "Handle should validate and call next when validators exist and pass")]
    public async Task Handle_ValidatorsExistAndPass_CallsNext()
    {
        // Arrange
        var validator = Substitute.For<IValidator<TestRequest>>();
        validator.ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>())
            .Returns(new ValidationResult());

        var validators = new List<IValidator<TestRequest>> { validator };
        var behavior = new ValidationBehavior<TestRequest, TestResponse>(validators);
        var request = new TestRequest();
        var next = Substitute.For<RequestHandlerDelegate<TestResponse>>();
        next.Invoke().Returns(new TestResponse());

        // Act
        await behavior.Handle(request, next, CancellationToken.None);

        // Assert
        await validator.Received(1).ValidateAsync(Arg.Any<ValidationContext<TestRequest>>(), Arg.Any<CancellationToken>());
        await next.Received(1).Invoke();
    }
}