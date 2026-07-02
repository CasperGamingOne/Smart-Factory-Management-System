using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Factory_Management_System
{
    internal class ProductionOrder
    {
        public string OrderId { get; } = Guid.NewGuid().ToString().Substring(0, 8);
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public int CompletedCount { get; set; } = 0;
        public int AssignedTechnicianId { get; set; }
        public bool IsComplete => CompletedCount >= Quantity;

        public ProductionOrder(string productName, int quantity, int technicianId)
        {
            ProductName = productName;
            Quantity = quantity;
            AssignedTechnicianId = technicianId;
        }
    }
}
