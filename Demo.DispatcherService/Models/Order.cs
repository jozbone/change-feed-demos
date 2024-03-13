namespace Demo.DispatcherService.Models;

public class Order
{
    public string Id { get; set; }
    public Guid OrderId { get; set; }
    public int Quantity { get; set; }
    public int AccountNumber { get; set; }
    public string? ProcessingRegion { get; set; }
}