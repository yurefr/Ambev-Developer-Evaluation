using Ambev.DeveloperEvaluation.Application.Users.GetUser;
using Ambev.DeveloperEvaluation.Unit.Domain.Entities.TestData;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.GetUser;

public class GetUserProfileTests
{
    private readonly IMapper _mapper;

    public GetUserProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<GetUserProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "GetUserProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map User to GetUserResult correctly")]
    public void Given_User_When_Mapped_Then_ReturnsCorrectResult()
    {
        // Arrange
        var user = UserTestData.GenerateValidUser();

        // Act
        var result = _mapper.Map<GetUserResult>(user);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(user.Id);
        result.Name.Should().Be(user.Username);
        result.Email.Should().Be(user.Email);
        result.Phone.Should().Be(user.Phone);
        result.Role.Should().Be(user.Role);
        result.Status.Should().Be(user.Status);
    }
}