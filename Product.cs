using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace Smart_Factory_Management_System
{
    public abstract class Product
    {
        public string? Name { get; set; }
        public int? Cores { get; protected set; }
        public double ClockSpeed { get; protected set; }
        public DateTime ProductionDate { get; private set; }

        private double productionCost;
        private double sellingPrice;
        private int quantity;

        public double ProductionCost
        {
            get { return productionCost; }
            set { productionCost = (value >= 0) ? value : 0; }
        }

        public double SellingPrice
        {
            get { return sellingPrice; }
            set { sellingPrice = (value >= 0) ? value : 0; }
        }
 
        public double CalculeazaMarjaProfit()
        {
            return SellingPrice - ProductionCost;
        }

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

        public abstract string GetTechnicalSpecifications();
    }
    //clasele derivate pentru diferite tipuri de produse
    public class Microprocessor : Product
    {
        public string Architecture { get; set; }
        public Microprocessor(string name, double cost, double price, int quantity, int cores, double clockSpeed)
            : base(name, cost, price, quantity)
        {
            Cores = cores;
            ClockSpeed = clockSpeed;
        }

        public override string GetTechnicalSpecifications()
        {
            // Return a formatted string that the UI can simply pass to a Panel
            return $"[bold cyan]Model:[/] {Name}\n" +
                   // $"[bold cyan]Serial:[/] {SerialNumber}\n" +
                   $"[bold yellow]Cores:[/] {Cores}\n" +
                   $"[bold yellow]Clock Speed:[/] {ClockSpeed} GHz";
        }
    }

    public enum BoardState { BlankBoard, SolderPrinted, ComponentsPlaced, BakedAndSoldered }

    public class Motherboard : Product
    {
        public BoardState CurrentState { get; private set; } = BoardState.BlankBoard;

        // Only the machines will call this
        public void TransitionTo(BoardState nextState) => CurrentState = nextState;

        public string SocketStandard { get; set; }
        public string PhysicalForm { get; set; }
        public Motherboard(string name, double cost, double price, int quantity, string socket, string type)
            : base(name, cost, price, quantity)
        {
            SocketStandard = socket;
            PhysicalForm = type;
        }

        public override string GetTechnicalSpecifications()
        {
            // Return a formatted string that the UI can simply pass to a Panel
            return $"[bold cyan]Model:[/] {Name}\n" +
                   // $"[bold cyan]Serial:[/] {SerialNumber}\n" +
                   $"[bold yellow]Socket:[/] {SocketStandard}\n" +
                   $"[bold yellow]Form Factor:[/] {PhysicalForm}";
        }
    }

}
