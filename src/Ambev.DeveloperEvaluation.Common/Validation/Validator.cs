using FluentValidation;

namespace Ambev.DeveloperEvaluation.Common.Validation;

public static class Validator
{
    public static async Task<IEnumerable<ValidationErrorDetail>> ValidateAsync<T>(T instance)
    {
        var validatorType = typeof(T).Assembly.GetTypes()
            .FirstOrDefault(t => t.IsClass && !t.IsAbstract && typeof(IValidator<T>).IsAssignableFrom(t));

        if (validatorType is null)
        {
            throw new InvalidOperationException($"No validator found for type {typeof(T).Name}");
        }

        var validator = (IValidator)Activator.CreateInstance(validatorType)!;

        var result = await validator.ValidateAsync(new ValidationContext<T>(instance));

        if (!result.IsValid)
        {
            return result.Errors.Select(o => (ValidationErrorDetail)o);
        }

        return [];
    }
}