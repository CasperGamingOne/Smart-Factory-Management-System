using System.Text.Json.Serialization;

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

    [JsonInclude] public string OrderId { get; init; } = Guid.NewGuid().ToString().Substring(0, 8);

    [JsonInclude] public string ProductName { get; init; } = string.Empty;

    [JsonInclude] public int Quantity { get; init; }

    [JsonInclude] public int CompletedCount { get; private set; }

    public int AssignedTechnicianId { get; set; }
    public bool IsComplete => CompletedCount >= Quantity;

    [JsonInclude] public string CustomProductName { get; init; } = string.Empty;

    [JsonInclude] public int? Cores { get; init; }

    [JsonInclude] public double? ClockSpeed { get; init; }

    [JsonInclude] public string? SocketStandard { get; init; }

    [JsonInclude] public string? PhysicalForm { get; init; }

    [JsonInclude] public string PlacedBy { get; init; } = string.Empty;

    public bool IsNotifiedComplete { get; set; }
    public string? BatchId { get; set; }

    [JsonInclude] public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

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