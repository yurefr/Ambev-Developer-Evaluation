using Ambev.DeveloperEvaluation.Domain.Repositories;
using Ambev.DeveloperEvaluation.Integration.TestData;
using Ambev.DeveloperEvaluation.ORM;
using Ambev.DeveloperEvaluation.ORM.Repositories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Ambev.DeveloperEvaluation.Integration.Repositories;

public class UserRepositoryTests
{
    private readonly DefaultContext _context;
    private readonly IUserRepository _userRepository;

    public UserRepositoryTests()
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<DefaultContext>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        builder.UseNpgsql(connectionString);

        _context = new DefaultContext(builder.Options);
        _userRepository = new UserRepository(_context);

        _context.Database.EnsureCreated();
    }

    [Fact(DisplayName = "CreateAsync should persist user in database")]
    public async Task CreateAsync_ValidUser_ShouldPersist()
    {
        var user = UserTestData.GenerateValidUser();

        await _userRepository.CreateAsync(user, CancellationToken.None);

        var persistedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        persistedUser.Should().NotBeNull();
        persistedUser!.Email.Should().Be(user.Email);
    }

    [Fact(DisplayName = "GetByEmailAsync should return user when email exists")]
    public async Task GetByEmailAsync_ExistingEmail_ShouldReturnUser()
    {
        var user = UserTestData.GenerateValidUser();
        await _userRepository.CreateAsync(user, CancellationToken.None);

        var retrievedUser = await _userRepository.GetByEmailAsync(user.Email, CancellationToken.None);

        retrievedUser.Should().NotBeNull();
        retrievedUser!.Id.Should().Be(user.Id);
    }

    [Fact(DisplayName = "DeleteAsync should remove user from database")]
    public async Task DeleteAsync_ExistingUser_ShouldRemove()
    {
        var user = UserTestData.GenerateValidUser();
        await _userRepository.CreateAsync(user, CancellationToken.None);

        var result = await _userRepository.DeleteAsync(user.Id, CancellationToken.None);

        result.Should().BeTrue();
        var deletedUser = await _context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
        deletedUser.Should().BeNull();
    }
}