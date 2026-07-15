namespace Smart_Factory_Management_System;

public class ProductionOrder(string productName, int quantity, int technicianId)
{
    public string OrderId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ProductName { get; init; } = productName;
    public int Quantity { get; init; } = quantity;
    public int CompletedCount { get; private set; }
    public int AssignedTechnicianId { get; init; } = technicianId;
    public bool IsComplete => CompletedCount >= Quantity;

    public void IncrementCompletedCount()
    {
        if (IsComplete)
            throw new InvalidOperationException("Order is already complete.");
        CompletedCount++;
    }
}