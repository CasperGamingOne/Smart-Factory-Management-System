namespace Smart_Factory_Management_System;

public class ProductionOrder(string productName, int quantity, int technicianId)
{
    public string OrderId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ProductName { get; set; } = productName;
    public int Quantity { get; set; } = quantity;
    public int CompletedCount { get; set; }
    public int AssignedTechnicianId { get; set; } = technicianId;
    public bool IsComplete => CompletedCount >= Quantity;
}