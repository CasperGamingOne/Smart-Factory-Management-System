namespace Smart_Factory_Management_System;

public class ProductionOrder
{
    public ProductionOrder()
    {
    }

    public ProductionOrder(string productName, int quantity, int technicianId)
    {
        ProductName = productName;
        Quantity = quantity;
        AssignedTechnicianId = technicianId;
    }

    public string OrderId { get; set; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public int CompletedCount { get; set; }
    public int AssignedTechnicianId { get; set; }
    public bool IsComplete => CompletedCount >= Quantity;

    public string CustomProductName { get; set; } = string.Empty;
    public int? Cores { get; set; }
    public double? ClockSpeed { get; set; }
    public string? SocketStandard { get; set; }
    public string? PhysicalForm { get; set; }

    public string PlacedBy { get; set; } = string.Empty;
    public bool IsNotifiedComplete { get; set; }
    public string? BatchId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public double GetPriorityScore()
    {
        var minutesElapsed = (DateTime.UtcNow - CreatedAt).TotalMinutes;
        return minutesElapsed - Quantity;
    }

    public void IncrementCompletedCount()
    {
        if (IsComplete)
            throw new InvalidOperationException("Order is already complete.");
        CompletedCount++;
    }
}