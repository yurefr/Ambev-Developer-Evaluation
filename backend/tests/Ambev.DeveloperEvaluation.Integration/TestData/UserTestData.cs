using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Bogus;

namespace Ambev.DeveloperEvaluation.Integration.TestData;

public static class UserTestData
{
    private static readonly Faker<User> UserFaker = new Faker<User>()
        .CustomInstantiator(f => new User())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Status, f => UserStatus.Active)
        .RuleFor(u => u.Role, f => UserRole.Customer);

    public static User GenerateValidUser()
    {
        return UserFaker.Generate();
    }
}