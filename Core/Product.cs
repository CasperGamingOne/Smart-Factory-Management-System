namespace Smart_Factory_Management_System;

public abstract class Product
{
    private readonly double _productionCost;
    private int _quantity;
    private double _sellingPrice;

    protected Product(string name, double cost, double price, int quantity)
    {
        Name = name;
        ProductionCost = cost;
        SellingPrice = price;
        Quantity = quantity;
    }

    public string? Name { get; protected set; }

    public double ProductionCost
    {
        get => _productionCost;
        private init => _productionCost = value >= 0 ? value : 0;
    }

    public double SellingPrice
    {
        get => _sellingPrice;
        set => _sellingPrice = value >= 0 ? value : 0;
    }

    public int Quantity
    {
        get => _quantity;
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity cannot be negative.");
            }

            _quantity = value;
        }
    }

    public abstract string GetTechnicalSpecifications();
}

public class Microprocessor(string name, double cost, double price, int quantity, int cores, double clockSpeed)
    : Product(name, cost, price, quantity)
{
    public string? Architecture { get; set; }
    public int? Cores { get; protected set; } = cores;
    public double ClockSpeed { get; protected set; } = clockSpeed;

    public override string GetTechnicalSpecifications()
    {
        // Return a formatted string that the UI can simply pass to a Panel
        return $"[bold cyan]Model:[/] {Name}\n" +
               // $"[bold cyan]Serial:[/] {SerialNumber}\n" +
               $"[bold yellow]Cores:[/] {Cores}\n" +
               $"[bold yellow]Clock Speed:[/] {ClockSpeed} GHz";
    }
}

public enum BoardState
{
    BlankBoard,
    SolderPrinted,
    ComponentsPlaced,
    BakedAndSoldered
}

public class Motherboard(string name, double cost, double price, int quantity, string socket, string type)
    : Product(name, cost, price, quantity)
{
    public BoardState CurrentState { get; private set; } = BoardState.BlankBoard;

    public string? SocketStandard { get; set; } = socket;
    public string? PhysicalForm { get; set; } = type;

    // Only the machines will call this
    public void TransitionTo(BoardState nextState)
    {
        CurrentState = nextState;
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