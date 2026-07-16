using System.Text.Json.Serialization;

namespace Smart_Factory_Management_System;

[JsonDerivedType(typeof(Microprocessor), "microprocessor")]
[JsonDerivedType(typeof(Motherboard), "motherboard")]
public abstract class Product
{
    private readonly double _productionCost;
    private int _quantity;
    private double _sellingPrice;

    protected Product(string name, double productionCost, double sellingPrice, int quantity)
    {
        Name = name;
        ProductionCost = productionCost;
        SellingPrice = sellingPrice;
        Quantity = quantity;
    }

    public string? Name { get; init; }

    public string? BatchId { get; set; }
    public bool IsSold { get; set; }

    public double ProductionCost
    {
        get => _productionCost;
        private init => _productionCost = value >= 0 ? value : 0;
    }

    public double SellingPrice
    {
        get => _sellingPrice;
        private set => _sellingPrice = value >= 0 ? value : 0;
    }

    public int Quantity
    {
        get => _quantity;
        private set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Quantity cannot be negative.");
            }

            _quantity = value;
        }
    }
    //**
    public int MinStockThreshold { get; set; } = 5; // Prag implicit
    

    public bool IsLowStock()
    {
        return Quantity <= MinStockThreshold;
    }
    //***
    public void MarkAsSold()
    {
        IsSold = true;
    }

    public void UpdateSellingPrice(double price)
    {
        SellingPrice = price;
    }

    public void AddQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount to add must be positive.", nameof(amount));
        Quantity += amount;
    }

    public void DeductQuantity(int amount)
    {
        if (amount <= 0)
            throw new ArgumentException("Amount to deduct must be positive.", nameof(amount));
        if (Quantity < amount)
            throw new InvalidOperationException("Insufficient stock to deduct.");
        Quantity -= amount;
    }

    public abstract string GetTechnicalSpecifications();
}

public class Microprocessor(
    string name,
    double productionCost,
    double sellingPrice,
    int quantity,
    int? cores,
    double clockSpeed)
    : Product(name, productionCost, sellingPrice, quantity)
{
    public string? Architecture { get; init; }
    public int? Cores { get; init; } = cores;
    public double ClockSpeed { get; init; } = clockSpeed;

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

public class Motherboard(
    string name,
    double productionCost,
    double sellingPrice,
    int quantity,
    string socketStandard,
    string physicalForm)
    : Product(name, productionCost, sellingPrice, quantity)
{
    public BoardState CurrentState { get; private set; } = BoardState.BlankBoard;

    public string? SocketStandard { get; init; } = socketStandard;
    public string? PhysicalForm { get; init; } = physicalForm;

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