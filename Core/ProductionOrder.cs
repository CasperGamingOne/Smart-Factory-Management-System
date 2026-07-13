namespace Smart_Factory_Management_System;

public class ProductionOrder
{
    public ProductionOrder(string productName, int quantity, int technicianId)
    {
        ProductName = productName;
        Quantity = quantity;
        AssignedTechnicianId = technicianId;
    }

    public string OrderId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ProductName { get; set; }
    public int Quantity { get; set; }
    public int CompletedCount { get; set; }
    public int AssignedTechnicianId { get; set; }
    public bool IsComplete => CompletedCount >= Quantity;
}