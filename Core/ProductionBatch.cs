namespace Smart_Factory_Management_System;

public class ProductionBatch(string productName, int quantity, double unitCost)
{
    private readonly List<int> _inventoryIndexes = new();
    public string BatchId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
    public string ProductName { get; init; } = productName;
    public int Quantity { get; init; } = quantity;
    public double UnitProductionCost { get; init; } = unitCost;
    public double? UnitSellPrice { get; private set; }
    public bool IsPriced => UnitSellPrice is > 0;
    public bool IsSold { get; private set; }
    public IReadOnlyList<int> InventoryIndexes => _inventoryIndexes;

    public double TotalCost => UnitProductionCost * Quantity;

    public void SetUnitSellPrice(double? price)
    {
        if (price < 0)
            throw new ArgumentOutOfRangeException(nameof(price), "Price cannot be negative.");
        UnitSellPrice = price;
    }

    public void MarkAsSold()
    {
        IsSold = true;
    }

    public void MarkAsUnsold()
    {
        IsSold = false;
    }

    public void AddInventoryIndex(int index)
    {
        _inventoryIndexes.Add(index);
    }
}