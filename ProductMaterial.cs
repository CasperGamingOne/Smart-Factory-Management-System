using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Factory_Management_System
{
    public class ProductMaterial
    {
        public string MaterialName { get; set; }
        public double QuantityRequired { get; set; } // Cantitatea necesara (ex: grame, bucati)
        public string Unit { get; set; }             // Unitatea de masura (ex: "g", "buc", "ml")

        public ProductMaterial(string name, double qty, string unit)
        {
            MaterialName = name;
            QuantityRequired = qty;
            Unit = unit;
        }

        public void AfiseazaDetaliiMaterial()
        {
            Console.WriteLine($"- {MaterialName}: {QuantityRequired} {Unit}");
        }
    }
}
