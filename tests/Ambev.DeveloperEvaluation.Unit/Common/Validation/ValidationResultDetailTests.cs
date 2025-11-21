using Ambev.DeveloperEvaluation.Common.Validation;
using FluentAssertions;
using FluentValidation.Results;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Common.Validation;

public class ValidationResultDetailTests
{
    [Fact(DisplayName = "Should map ValidationResult to ValidationResultDetail correctly")]
    public void Should_Map_ValidationResult_To_Detail()
    {
        // Arrange
        var failures = new List<ValidationFailure>
        {
            new("Field1", "Error 1") { ErrorCode = "Code1" },
            new("Field2", "Error 2") { ErrorCode = "Code2" }
        };
        var validationResult = new ValidationResult(failures);

        // Act
        var detail = new ValidationResultDetail(validationResult);

        // Assert
        detail.IsValid.Should().BeFalse();
        detail.Errors.Should().HaveCount(2);

        var firstError = detail.Errors.First();
        firstError.Error.Should().Be("Code1");
        firstError.Detail.Should().Be("Error 1");
    }

    [Fact(DisplayName = "Should create empty detail")]
    public void Should_Create_Empty_Detail()
    {
        var detail = new ValidationResultDetail();
        detail.IsValid.Should().BeFalse();
        detail.Errors.Should().BeEmpty();
    }
}