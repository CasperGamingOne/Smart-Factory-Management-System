namespace Smart_Factory_Management_System;

public class ProductionOrder(string productName, int quantity, int technicianId)
{
    public string OrderId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ProductName { get; init; } = productName;
    public int Quantity { get; init; } = quantity;
    public int CompletedCount { get; private set; }
    public int AssignedTechnicianId { get; set; } = technicianId;
    public bool IsComplete => CompletedCount >= Quantity;

    public string CustomProductName { get; set; } = string.Empty;
    public int? Cores { get; set; }
    public double? ClockSpeed { get; set; }
    public string? SocketStandard { get; set; }
    public string? PhysicalForm { get; set; }

    public string PlacedBy { get; set; } = string.Empty;
    public bool IsNotifiedComplete { get; set; } = false;

    public DateTime CreatedAt { get; } = DateTime.UtcNow;

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