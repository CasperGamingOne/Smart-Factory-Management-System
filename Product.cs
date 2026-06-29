using System;
using System.Collections.Generic;
using System.Text;

namespace Smart_Factory_Management_System
{
    public abstract class Product
    {
        //proprietati
        public string? Name { get; set; }
        public string? Category { get; protected set; }
        public DateTime ProductionDate { get; private set; }

        // 
        private double productionCost;
        private double sellingPrice;
        private int quantity;

        // proprietatea pt costul de productie (cu validare) 
        public double ProductionCost { get { return productionCost; }
            set
            {
                if (value >= 0)
                {
                    productionCost = value;
                }
                else
                {
                    productionCost = 0; // Previne costul negativ
                    Console.WriteLine("!!! Costul de productie nu poate fi negativ! S-a setat automat pe 0.");
                }
            }
        }

        // proprietatea pentru Pretul de vanzare (cu validare)
        public double SellingPrice { get { return sellingPrice; }
            set
            {
                if (value >= 0)
                {
                    sellingPrice = value;
                }
                else
                {
                    sellingPrice = 0;
                    Console.WriteLine("!!! Pretul de vanzare nu poate fi negativ! S-a setat automat pe 0.");
                }
            }
        }

        // Proprietatea pentru Cantitate (cu validare)
        public int Quantity { get { return quantity; }
            set
            {
                if (value >= 0)
                {
                    quantity = value;
                }
                else
                {
                    quantity = 0; // Previne cantitatea negativa
                    Console.WriteLine("!!! Cantitatea in stoc nu poate fi negativa! S-a setat automat pe 0.");
                }

            }
        }

        // Constructorul pt intitializarea unui produs
        public Product(string name, double cost, double price, int quantity)
        {
            Name = name;
            ProductionCost = cost;
            SellingPrice = price;
            Quantity = quantity;
            ProductionDate = DateTime.Now;
        }
    }
    //clasele derivate pentru diferite tipuri de produse
    public class Microcontroller : Product
    {
        public string Architecture { get; set; }
        public Microcontroller(string name, double cost, double price, int quantity, string arch)
            : base(name, cost, price, quantity)
        {
            Category = "Semiconductori";
            Architecture = arch;
        }
    }
    public class SensorModule : Product
    {
        public string SensorType { get; set; }
        public SensorModule(string name, double cost, double price, int quantity, string type)
            : base(name, cost, price, quantity)
        {
            Category = "Senzori";
            SensorType = type;
        }
    }

}
