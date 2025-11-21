using Ambev.DeveloperEvaluation.Application.Users.DeleteUser;
using Ambev.DeveloperEvaluation.WebApi.Features.Users.DeleteUser;
using AutoMapper;
using FluentAssertions;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Users.DeleteUser;

public class DeleteUserProfileTests
{
    private readonly IMapper _mapper;

    public DeleteUserProfileTests()
    {
        var configuration = new MapperConfiguration(cfg => cfg.AddProfile<DeleteUserProfile>());
        _mapper = configuration.CreateMapper();
    }

    [Fact(DisplayName = "DeleteUserProfile configuration should be valid")]
    public void AssertConfigurationIsValid()
    {
        _mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }

    [Fact(DisplayName = "Should map Guid to DeleteUserCommand correctly")]
    public void Given_Guid_When_Mapped_Then_ReturnsCommand()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var command = _mapper.Map<DeleteUserCommand>(id);

        // Assert
        command.Id.Should().Be(id);
    }
}