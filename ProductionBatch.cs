namespace Smart_Factory_Management_System
{
    internal class ProductionBatch(string productName, int quantity, double unitCost)
    {
        public string BatchId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
        public string ProductName { get; set; } = productName;
        public int Quantity { get; set; } = quantity;
        public double UnitProductionCost { get; set; } = unitCost;
        public double? UnitSellPrice { get; set; }
        public bool IsPriced => UnitSellPrice is > 0;
        public bool IsSold { get; set; }

        public List<int> InventoryIndexes { get; } = new List<int>();

        public double TotalCost => UnitProductionCost * Quantity;
    }
}