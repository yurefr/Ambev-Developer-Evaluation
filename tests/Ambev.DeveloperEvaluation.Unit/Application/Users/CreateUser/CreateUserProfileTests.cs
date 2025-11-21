using Ambev.DeveloperEvaluation.Application.Users.CreateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Unit.Domain;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.CreateUser;

public class CreateUserProfileTests
{
    private readonly IMapper _mapper;

    public CreateUserProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<CreateUserProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "CreateUserProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map CreateUserCommand to User correctly")]
    public void Given_Command_When_Mapped_Then_ReturnsUser()
    {
        // Arrange
        var command = CreateUserHandlerTestData.GenerateValidCommand();

        // Act
        var user = _mapper.Map<User>(command);

        // Assert
        user.Username.Should().Be(command.Username);
        user.Email.Should().Be(command.Email);
        user.Phone.Should().Be(command.Phone);
        user.Status.Should().Be(command.Status);
        user.Role.Should().Be(command.Role);
    }

    [Fact(DisplayName = "Should map User to CreateUserResult correctly")]
    public void Given_User_When_Mapped_Then_ReturnsResult()
    {
        // Arrange
        var user = new User { Id = Guid.NewGuid(), Username = "test", Email = "test@test.com" };

        // Act
        var result = _mapper.Map<CreateUserResult>(user);

        // Assert
        result.Id.Should().Be(user.Id);
    }
}