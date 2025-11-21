using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using FluentValidation.TestHelper;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.DeleteUser;

public class DeleteUserValidatorTests
{
    private readonly DeleteUserValidator _validator;

    public DeleteUserValidatorTests()
    {
        _validator = new DeleteUserValidator();
    }

    [Fact(DisplayName = "Valid command should pass validation")]
    public void Given_ValidCommand_When_Validated_Then_ShouldNotHaveErrors()
    {
        var command = new DeleteUserCommand(Guid.NewGuid());
        var result = _validator.TestValidate(command);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact(DisplayName = "Empty ID should fail validation")]
    public void Given_EmptyId_When_Validated_Then_ShouldHaveError()
    {
        var command = new DeleteUserCommand(Guid.Empty);
        var result = _validator.TestValidate(command);
        result.ShouldHaveValidationErrorFor(x => x.Id);
    }
}