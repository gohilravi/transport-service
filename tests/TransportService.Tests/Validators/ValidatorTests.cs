using FluentAssertions;
using Xunit;
using TransportService.Core.DTOs;
using TransportService.Core.Validators;

namespace TransportService.Tests.Validators;

public class CreateTransportRequestValidatorTests
{
    private readonly CreateTransportRequestValidator _validator;

    public CreateTransportRequestValidatorTests()
    {
        _validator = new CreateTransportRequestValidator();
    }

    [Fact]
    public void Validate_WithValidRequest_ReturnsValid()
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
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData(0, "OfferId must be greater than 0")]
    [InlineData(-1, "OfferId must be greater than 0")]
    public void Validate_WithInvalidOfferId_ReturnsInvalid(int offerId, string expectedError)
    {
        // Arrange
        var request = new CreateTransportRequest
        {
            OfferId = offerId,
            PurchaseId = 1,
            SellerId = 1,
            BuyerId = 1,
            CarrierId = 1,
            SellerZipCode = "12345",
            BuyerZipCode = "67890",
            ScheduleWindow = new ScheduleWindow
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(1),
                ScheduledDate = DateTime.UtcNow.AddDays(1).AddHours(10)
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedError);
    }

    [Theory]
    [InlineData("", "SellerZipCode is required")]
    [InlineData("123", "SellerZipCode must be a valid ZIP code format")]
    [InlineData("12345-123", "SellerZipCode must be a valid ZIP code format")]
    public void Validate_WithInvalidSellerZipCode_ReturnsInvalid(string zipCode, string expectedError)
    {
        // Arrange
        var request = new CreateTransportRequest
        {
            OfferId = 1,
            PurchaseId = 1,
            SellerId = 1,
            BuyerId = 1,
            CarrierId = 1,
            SellerZipCode = zipCode,
            BuyerZipCode = "67890",
            ScheduleWindow = new ScheduleWindow
            {
                StartDate = DateTime.UtcNow.AddDays(1),
                EndDate = DateTime.UtcNow.AddDays(2)
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedError);
    }

    [Fact]
    public void Validate_WithPastStartDate_ReturnsInvalid()
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
                StartDate = DateTime.UtcNow.AddDays(-1), // Past date
                EndDate = DateTime.UtcNow.AddDays(1)
            }
        };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "ScheduleWindow StartDate must be in the future");
    }
}

public class UpdateTransportStatusRequestValidatorTests
{
    private readonly UpdateTransportStatusRequestValidator _validator;

    public UpdateTransportStatusRequestValidatorTests()
    {
        _validator = new UpdateTransportStatusRequestValidator();
    }

    [Fact]
    public void Validate_WithValidStatus_ReturnsValid()
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = "InTransit" };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "Status is required")]
    [InlineData(null, "Status is required")]
    public void Validate_WithEmptyStatus_ReturnsInvalid(string status, string expectedError)
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = status };

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == expectedError);
    }

    [Fact]
    public void Validate_WithTooLongStatus_ReturnsInvalid()
    {
        // Arrange
        var request = new UpdateTransportStatusRequest { Status = new string('A', 31) }; // 31 characters

        // Act
        var result = _validator.Validate(request);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.ErrorMessage == "Status must not exceed 30 characters");
    }
}