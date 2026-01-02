using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Json;
using System.Net;
using FluentAssertions;
using Xunit;
using TransportService.Core.DTOs;
using TransportService.Infrastructure.Data;
using TransportService.API;

namespace TransportService.Tests.Integration;

public class TransportApiIntegrationTests : IClassFixture<WebApplicationFactory<TransportService.API.Program>>
{
    private readonly HttpClient _client;
    private readonly WebApplicationFactory<TransportService.API.Program> _factory;

    public TransportApiIntegrationTests(WebApplicationFactory<TransportService.API.Program> factory)
    {
        _factory = factory.WithWebHostBuilder(builder =>
        {
            builder.ConfigureServices(services =>
            {
                // Remove the existing DbContext registration
                var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));
                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                // Add in-memory database for testing
                services.AddDbContext<ApplicationDbContext>(options =>
                {
                    options.UseInMemoryDatabase("TestDb");
                });
            });
        });
        
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task CreateTransport_WithValidData_ReturnsCreatedWithId()
    {
        // Arrange
        var request = new CreateTransportRequest
        {
            OfferId = 1,
            PurchaseId = 1,
            SellerId = 1,
            BuyerId = 1,
            CarrierId = 1,
            SellerZipCode = "12345",
            BuyerZipCode = "67890",
            ScheduleWindow = new ScheduleWindow
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                ScheduledDate = DateTime.UtcNow.AddDays(1).AddHours(10)
            }
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transport", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        
        var result = await response.Content.ReadFromJsonAsync<CreateTransportResponse>();
        result.Should().NotBeNull();
        result!.Id.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task CreateTransport_WithInvalidData_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTransportRequest
        {
            // Missing required fields
            OfferId = 0,
            SellerZipCode = "invalid",
            BuyerZipCode = "invalid"
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/transport", request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task UpdateTransportStatus_WithValidData_ReturnsOk()
    {
        // Arrange - First create a transport
        var createRequest = new CreateTransportRequest
        {
            OfferId = 1,
            PurchaseId = 1,
            SellerId = 1,
            BuyerId = 1,
            CarrierId = 1,
            SellerZipCode = "12345",
            BuyerZipCode = "67890",
            ScheduleWindow = new ScheduleWindow
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2),
                ScheduledDate = DateTime.UtcNow.AddDays(1).AddHours(14)
            }
        };

        var createResponse = await _client.PostAsJsonAsync("/api/transport", createRequest);
        var createdTransport = await createResponse.Content.ReadFromJsonAsync<CreateTransportResponse>();

        var updateRequest = new UpdateTransportStatusRequest
        {
            Status = "InTransit"
        };

        // Act
        var response = await _client.PatchAsJsonAsync($"/api/transport/{createdTransport!.Id}/status", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdateTransportStatus_WithInvalidId_ReturnsNotFound()
    {
        // Arrange
        var updateRequest = new UpdateTransportStatusRequest
        {
            Status = "InTransit"
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/transport/999/status", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task UpdateTransportStatus_WithEmptyStatus_ReturnsBadRequest()
    {
        // Arrange
        var updateRequest = new UpdateTransportStatusRequest
        {
            Status = ""
        };

        // Act
        var response = await _client.PatchAsJsonAsync("/api/transport/1/status", updateRequest);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}