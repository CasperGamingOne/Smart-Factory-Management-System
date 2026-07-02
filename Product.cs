using Smart_Factory_Management_System;

namespace Smart_Factory_Management_System
{
    public abstract class Product
    {
        //proprietati
        public string? Name { get; set; }
        public string? Category { get; protected set; }
        public DateTime ProductionDate { get; private set; }
        public ProductMaterial[] Materiale { get; private set; }
        private int nrMateriale = 0;

        // 
        private double productionCost;
        private double sellingPrice;
        private int quantity;

        // proprietatea pt costul de productie (cu validare) 
        public double ProductionCost
        {
            get { return productionCost; }
            set { productionCost = (value >= 0) ? value : 0; }
        }

        // proprietatea pentru Pretul de vanzare (cu validare)
        public double SellingPrice
        {
            get { return sellingPrice; }
            set { sellingPrice = (value >= 0) ? value : 0; }
        }
        //calcul marja de profit 
        public double CalculeazaMarjaProfit()
        {
            return SellingPrice - ProductionCost;
        }

        // Proprietatea pentru Cantitate (cu validare)
        public int Quantity
        {
            get { return quantity; }
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
            Materiale = new ProductMaterial[10];

        }
        // Metoda pentru adaugarea unui material
        public void AdaugaMaterial(string nume, double cantitate, string unitate)
        {
            if (cantitate < 0)
            {
                Console.WriteLine("Eroare: Cantitatea materialului nu poate fi negativa!");
                return; // Opreste executia metodei aici
            }
            if (nrMateriale < Materiale.Length)
            {
                Materiale[nrMateriale] = new ProductMaterial(nume, cantitate, unitate);
                nrMateriale++;
            }
            else
            {
                Console.WriteLine("Eroare: Nu mai poti adauga materiale (limita atinsa).");
            }
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
    public class PowerModule : Product   //produs nou: Modul de Alimentare
    {
        public double OutputVoltage { get; set; } // Tensiunea de iesire (5V,12V )

        public PowerModule(string name, double cost, double price, int quantity, double voltage)
            : base(name, cost, price, quantity)
        {
            Category = "Alimentare";
            OutputVoltage = voltage;
        }
    }
    // Produs nou: Modul de Comunicare
    public class CommunicationModule : Product
    {
        public string Protocol { get; set; } // Tipul protocolului (ex: Wi-Fi, Bluetooth, LoRa)

        public CommunicationModule(string name, double cost, double price, int quantity, string protocol)
            : base(name, cost, price, quantity)
        {
            Category = "Comunicatii";
            Protocol = protocol;
        }
    }


}