using Ambev.DeveloperEvaluation.Common.Security;
using Ambev.DeveloperEvaluation.Domain.Entities;
using Ambev.DeveloperEvaluation.Domain.Enums;
using Ambev.DeveloperEvaluation.Functional.TestData;
using Ambev.DeveloperEvaluation.WebApi.Common;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.CreateSale;
using Ambev.DeveloperEvaluation.WebApi.Features.Sales.GetSale;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace Ambev.DeveloperEvaluation.Functional.Features.Sales;

/// <summary>
/// Contains End-to-End (E2E) functional tests for the Sales API endpoints.
/// Tests the full stack including Routing, Controllers, Middleware, and Database persistence.
/// </summary>
public class SalesControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Initializes the test class with a WebApplicationFactory to simulate the API environment.
    /// </summary>
    public SalesControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    private string GenerateAuthToken()
    {
        using var scope = _factory.Services.CreateScope();
        var tokenGenerator = scope.ServiceProvider.GetRequiredService<IJwtTokenGenerator>();

        var user = new User
        {
            Id = Guid.NewGuid(),
            Username = "testuser",
            Role = UserRole.Admin,
            Email = "test@test.com"
        };

        return tokenGenerator.GenerateToken(user);
    }

    [Fact(DisplayName = "CreateSale should return 201 Created and valid data")]
    public async Task CreateSale_ValidRequest_ReturnsCreated()
    {
        // Arrange
        var token = GenerateAuthToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var request = SaleRequestTestData.GenerateValidRequest();

        // Act
        var response = await _client.PostAsJsonAsync("/api/sales", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>(_jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().NotBeEmpty();
        apiResponse.Data.TotalAmount.Should().BeGreaterThan(0);
    }

    [Fact(DisplayName = "GetSale should return 200 OK when sale exists")]
    public async Task GetSale_ExistingId_ReturnsOk()
    {
        // Arrange
        var token = GenerateAuthToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var createRequest = SaleRequestTestData.GenerateValidRequest();
        var createResponse = await _client.PostAsJsonAsync("/api/sales", createRequest);
        var createResult = await createResponse.Content.ReadFromJsonAsync<ApiResponseWithData<CreateSaleResponse>>(_jsonOptions);
        var saleId = createResult!.Data!.Id;

        // Act
        var response = await _client.GetAsync($"/api/sales/{saleId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponseWithData<GetSaleResponse>>(_jsonOptions);

        apiResponse.Should().NotBeNull();
        apiResponse!.Data.Should().NotBeNull();
        apiResponse.Data!.Id.Should().Be(saleId);
        apiResponse.Data.CustomerName.Should().Be(createRequest.CustomerName);
    }

    [Fact(DisplayName = "GetSale should return 404 NotFound when ID does not exist")]
    public async Task GetSale_NonExistingId_ReturnsNotFound()
    {
        // Arrange
        var token = GenerateAuthToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var nonExistingId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/sales/{nonExistingId}");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}