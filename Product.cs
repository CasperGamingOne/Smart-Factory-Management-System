namespace Smart_Factory_Management_System
{
    public abstract class Product
    {
        public string? Name { get; set; }
        public string? Category { get; protected set; }
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

        /*
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
        */

        // Constructorul pt intitializarea unui produs
        public Product(string name, double cost, double price, int quantity)
        {
            Name = name;
            ProductionCost = cost;
            SellingPrice = price;
            // Quantity = quantity;
            ProductionDate = DateTime.Now;
        }
    }
    //clasele derivate pentru diferite tipuri de produse
    public class Microprocessor : Product
    {
        public string Architecture { get; set; }
        public Microprocessor(string name, double cost, double price, int quantity, string arch)
            : base(name, cost, price, quantity)
        {
            Category = "Semiconductori";
            Architecture = arch;
        }
    }

    public enum BoardState { BlankBoard, SolderPrinted, ComponentsPlaced, BakedAndSoldered }

    public class Motherboard : Product
    {
        public BoardState CurrentState { get; private set; } = BoardState.BlankBoard;

        // Only the machines will call this
        public void TransitionTo(BoardState nextState) => CurrentState = nextState;

        public string SensorType { get; set; }
        public Motherboard(string name, double cost, double price, int quantity, string type)
            : base(name, cost, price, quantity)
        {
            Category = "Senzori";
            SensorType = type;
        }
    }

}
