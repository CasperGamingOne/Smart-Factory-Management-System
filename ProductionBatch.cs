using System;
using System.Collections.Generic;

namespace Smart_Factory_Management_System
{
    internal class ProductionBatch
    {
        public string BatchId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double UnitProductionCost { get; set; }
        public double? UnitSellPrice { get; set; }
        public double? SoldUnitPrice { get; set; }
        public bool IsPriced => UnitSellPrice.HasValue && UnitSellPrice.Value > 0;
        public bool IsSold { get; set; } = false;

        public List<int> InventoryIndexes { get; } = new List<int>();

        public ProductionBatch(string productName, int quantity, double unitCost)
        {
            ProductName = productName;
            Quantity = quantity;
            UnitProductionCost = unitCost;
        }

        public double TotalCost => UnitProductionCost * Quantity;
    }
}
