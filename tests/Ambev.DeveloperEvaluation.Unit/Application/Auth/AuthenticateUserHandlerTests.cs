using Ambev.DeveloperEvaluation.Application.Auth.AuthenticateUser;
using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Domain.Repositories;
using FluentAssertions;
using NSubstitute;
using Xunit;

namespace Ambev.DeveloperEvaluation.Unit.Application.Auth;

public class AuthenticateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly AuthenticateUserHandler _handler;

    public AuthenticateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _passwordHasher = Substitute.For<IPasswordHasher>();
        _jwtTokenGenerator = Substitute.For<IJwtTokenGenerator>();
        _handler = new AuthenticateUserHandler(_userRepository, _passwordHasher, _jwtTokenGenerator);
    }

    [Fact(DisplayName = "Should authenticate successfully when credentials are valid and user is active")]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "valid@test.com", Password = "password" };
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            Password = "hashed_password",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            Username = "Test User"
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);
        _jwtTokenGenerator.GenerateToken(user).Returns("valid_token");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().Be("valid_token");
        result.Email.Should().Be(user.Email);
        result.Role.Should().Be(user.Role.ToString());
    }

    [Fact(DisplayName = "Should throw UnauthorizedAccessException when user does not exist")]
    public async Task Handle_UserNotFound_ThrowsUnauthorized()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "wrong@test.com", Password = "password" };
        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns((User?)null);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact(DisplayName = "Should throw UnauthorizedAccessException when password is wrong")]
    public async Task Handle_WrongPassword_ThrowsUnauthorized()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "valid@test.com", Password = "wrong_password" };
        var user = new User { Email = command.Email, Password = "hashed_password" };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(false);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("Invalid credentials");
    }

    [Fact(DisplayName = "Should throw UnauthorizedAccessException when user is inactive")]
    public async Task Handle_InactiveUser_ThrowsUnauthorized()
    {
        // Arrange
        var command = new AuthenticateUserCommand { Email = "inactive@test.com", Password = "password" };
        var user = new User
        {
            Email = command.Email,
            Password = "hashed_password",
            Status = UserStatus.Inactive
        };

        _userRepository.GetByEmailAsync(command.Email, Arg.Any<CancellationToken>()).Returns(user);
        _passwordHasher.VerifyPassword(command.Password, user.Password).Returns(true);

        // Act
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Assert
        await act.Should().ThrowAsync<UnauthorizedAccessException>()
            .WithMessage("User is not active");
    }
}