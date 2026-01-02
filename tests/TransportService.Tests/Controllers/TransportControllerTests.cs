using Microsoft.AspNetCore.Mvc;
using Moq;
using FluentAssertions;
using FluentValidation;
using FluentValidation.Results;
using Xunit;
using TransportService.API.Controllers;
using TransportService.Core.DTOs;
using TransportService.Core.Interfaces;

namespace TransportService.Tests.Controllers;

public class TransportControllerTests
{
    private readonly Mock<ITransportService> _mockTransportService;
    private readonly Mock<IValidator<CreateTransportRequest>> _mockCreateValidator;
    private readonly Mock<IValidator<UpdateTransportStatusRequest>> _mockUpdateValidator;
    private readonly TransportController _controller;

    public TransportControllerTests()
    {
        _mockTransportService = new Mock<ITransportService>();
        _mockCreateValidator = new Mock<IValidator<CreateTransportRequest>>();
        _mockUpdateValidator = new Mock<IValidator<UpdateTransportStatusRequest>>();
        _controller = new TransportController(_mockTransportService.Object, _mockCreateValidator.Object, _mockUpdateValidator.Object);
    }

    [Fact]
    public async Task CreateTransport_WithValidRequest_ReturnsCreatedWithId()
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

        var transportResponse = new CreateTransportResponse { Id = 123 };
        
        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateTransportRequest>(), default))
            .ReturnsAsync(new ValidationResult());
        
        _mockTransportService.Setup(s => s.CreateTransportAsync(It.IsAny<CreateTransportRequest>()))
            .ReturnsAsync(transportResponse);

        // Act
        var result = await _controller.CreateTransport(request);

        // Assert
        result.Should().BeOfType<ActionResult<CreateTransportResponse>>();
        var createdResult = result.Result.Should().BeOfType<CreatedAtRouteResult>().Subject;
        createdResult.StatusCode.Should().Be(201);
        
        var response = createdResult.Value.Should().BeOfType<CreateTransportResponse>().Subject;
        response.Id.Should().Be(123);
    }

    [Fact]
    public async Task CreateTransport_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new CreateTransportRequest();
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("OfferId", "OfferId must be greater than 0")
        };
        var validationResult = new ValidationResult(validationFailures);
        
        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateTransportRequest>(), default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.CreateTransport(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task CreateTransport_WithServiceException_ReturnsInternalServerError()
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

        _mockCreateValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateTransportRequest>(), default))
            .ReturnsAsync(new ValidationResult());
        
        _mockTransportService.Setup(s => s.CreateTransportAsync(It.IsAny<CreateTransportRequest>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.CreateTransport(request);

        // Assert
        var statusCodeResult = result.Result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task UpdateTransportStatus_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = "InTransit" };
        
        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateTransportStatusRequest>(), default))
            .ReturnsAsync(new ValidationResult());
        
        _mockTransportService.Setup(s => s.UpdateTransportStatusAsync(1, "InTransit"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _controller.UpdateTransportStatus(1, request);

        // Assert
        result.Should().BeOfType<OkResult>();
    }

    [Fact]
    public async Task UpdateTransportStatus_WithInvalidRequest_ReturnsBadRequest()
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = "" };
        var validationFailures = new List<ValidationFailure>
        {
            new ValidationFailure("Status", "Status is required")
        };
        var validationResult = new ValidationResult(validationFailures);
        
        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateTransportStatusRequest>(), default))
            .ReturnsAsync(validationResult);

        // Act
        var result = await _controller.UpdateTransportStatus(1, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task UpdateTransportStatus_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = "InTransit" };
        
        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateTransportStatusRequest>(), default))
            .ReturnsAsync(new ValidationResult());
        
        _mockTransportService.Setup(s => s.UpdateTransportStatusAsync(999, "InTransit"))
            .ThrowsAsync(new ArgumentException("Transport with ID 999 not found."));

        // Act
        var result = await _controller.UpdateTransportStatus(999, request);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task UpdateTransportStatus_WithServiceException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = "InTransit" };
        
        _mockUpdateValidator.Setup(v => v.ValidateAsync(It.IsAny<UpdateTransportStatusRequest>(), default))
            .ReturnsAsync(new ValidationResult());
        
        _mockTransportService.Setup(s => s.UpdateTransportStatusAsync(1, "InTransit"))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.UpdateTransportStatus(1, request);

        // Assert
        var statusCodeResult = result.Should().BeOfType<ObjectResult>().Subject;
        statusCodeResult.StatusCode.Should().Be(500);
    }
}