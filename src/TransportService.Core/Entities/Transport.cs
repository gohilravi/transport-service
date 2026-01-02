namespace TransportService.Core.Entities;

public class Transport
{
    public int Id { get; set; }
    public int CarrierId { get; set; }
    public int PurchaseId { get; set; }
    public string PickupLocation { get; set; } = string.Empty;
    public string DeliveryLocation { get; set; } = string.Empty;
    public DateTime? ScheduleDate { get; set; }
    public string? VehicleDetails { get; set; }
    public string Status { get; set; } = "Scheduled";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties (if needed)
    // public Carrier? Carrier { get; set; }
    // public Purchase? Purchase { get; set; }
}