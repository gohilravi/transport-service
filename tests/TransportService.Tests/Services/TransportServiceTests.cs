using AutoMapper;
using Moq;
using FluentAssertions;
using Xunit;
using TransportService.Core.DTOs;
using TransportService.Core.Entities;
using TransportService.Core.Interfaces;
using TransportService.Infrastructure.Services;

namespace TransportService.Tests.Services;

public class TransportServiceTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<ITransportRepository> _mockTransportRepository;
    private readonly Mock<IPublishEndpoint> _mockPublishEndpoint;
    private readonly TransportService.Infrastructure.Services.TransportService _service;

    public TransportServiceTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockMapper = new Mock<IMapper>();
        _mockTransportRepository = new Mock<ITransportRepository>();
        _mockPublishEndpoint = new Mock<IPublishEndpoint>();
        
        _mockUnitOfWork.Setup(u => u.Transports).Returns(_mockTransportRepository.Object);
        _service = new TransportService.Infrastructure.Services.TransportService(_mockUnitOfWork.Object, _mockMapper.Object, _mockPublishEndpoint.Object);
    }

    [Fact]
    public async Task CreateTransportAsync_WithValidRequest_ReturnsTransportResponse()
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

        var transport = new Transport { Id = 1, CarrierId = 1, PurchaseId = 1 };

        _mockMapper.Setup(m => m.Map<Transport>(request)).Returns(transport);
        _mockTransportRepository.Setup(r => r.CreateAsync(It.IsAny<Transport>())).ReturnsAsync(transport);
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        // Act
        var result = await _service.CreateTransportAsync(request, "elastic-123");

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task CreateTransportAsync_WithException_RollsBackTransaction()
    {
        // Arrange
        var request = new CreateTransportRequest();
        
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<Transport>(request)).Throws(new Exception("Mapping error"));
        _mockUnitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        await _service.Invoking(s => s.CreateTransportAsync(request, "elastic-123"))
            .Should().ThrowAsync<Exception>()
            .WithMessage("Mapping error");

        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTransportStatusAsync_WithValidId_ReturnsUpdatedTransport()
    {
        // Arrange
        var transport = new Transport { Id = 1, Status = "Assigned" };
        var updatedTransport = new Transport { Id = 1, Status = "InTransit" };

        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mockTransportRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(transport);
        _mockTransportRepository.Setup(r => r.UpdateAsync(It.IsAny<Transport>())).ReturnsAsync(updatedTransport);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _mockUnitOfWork.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);

        // Act
        await _service.UpdateTransportStatusAsync(1, "InTransit", "elastic-456");

        // Assert
        _mockUnitOfWork.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.SaveChangesAsync(), Times.Once);
        _mockUnitOfWork.Verify(u => u.CommitTransactionAsync(), Times.Once);
    }

    [Fact]
    public async Task UpdateTransportStatusAsync_WithInvalidId_ThrowsArgumentException()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _mockTransportRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Transport?)null);
        _mockUnitOfWork.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);

        // Act & Assert
        await _service.Invoking(s => s.UpdateTransportStatusAsync(999, "InTransit", "elastic-456"))
            .Should().ThrowAsync<ArgumentException>()
            .WithMessage("Transport with ID 999 not found.");

        _mockUnitOfWork.Verify(u => u.RollbackTransactionAsync(), Times.Once);
    }
}