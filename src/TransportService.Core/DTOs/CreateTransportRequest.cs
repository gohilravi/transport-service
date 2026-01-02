namespace TransportService.Core.DTOs;

public class CreateTransportRequest
{
    public int OfferId { get; set; }
    public int PurchaseId { get; set; }
    public int SellerId { get; set; }
    public int BuyerId { get; set; }
    public int CarrierId { get; set; }
    public string SellerZipCode { get; set; } = string.Empty;
    public string BuyerZipCode { get; set; } = string.Empty;
    public ScheduleWindow ScheduleWindow { get; set; } = new();
}