using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.GetUser;

public class GetUserValidatorTests
{
    private readonly GetUserValidator _validator;

    public GetUserValidatorTests()
    {
        _validator = new GetUserValidator();
    }

    [Fact(DisplayName = "Valid command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = new GetUserCommand(Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty ID should fail validation")]
    public void Given_EmptyId_When_Validated_Then_ShouldHaveError()
    {
        var command = new GetUserCommand(Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}