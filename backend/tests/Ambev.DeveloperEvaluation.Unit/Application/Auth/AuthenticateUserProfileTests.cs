using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Auth;

public class AuthenticateUserProfileTests
{
    private readonly IMapper _mapper;

    public AuthenticateUserProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<AuthenticateUserProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "Mapper configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        // Assert
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map User to AuthenticateUserResult correctly")]
    public void Given_User_When_Mapped_Then_ReturnsCorrectResult()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Email = "test@test.com",
            Phone = "+5511999999999",
            Role = UserRole.Customer,
            Status = UserStatus.Active
        };

        // Act
        var result = _mapper.Map<AuthenticateUserResult>(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        result.Phone.Should().Be(user.Phone);
        result.Role.Should().Be(user.Role.ToString());
    }
}