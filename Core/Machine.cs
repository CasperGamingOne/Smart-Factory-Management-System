using System.Text.Json.Serialization;
using Spectre.Console;

namespace Smart_Factory_Management_System;

public enum MachineStatus
{
    Running,
    Stopped,
    Maintenance
}

public enum MachineCondition
{
    Excellent,
    Good,
    Critical
}

[JsonDerivedType(typeof(LithographyMachine), "litographyMachine")]
[JsonDerivedType(typeof(SmtMachine), "smtMachine")]
[JsonDerivedType(typeof(PaPMachine), "papMachine")]
[JsonDerivedType(typeof(ReflowOven), "reflowOven")]
public abstract class Machine
{
    private static int _idCounter;

    private static readonly Random Random = new();

    protected Machine(string name, string manufacturer, string serialNumber, List<MachinePart> parts,
        MachineCondition condition)
    {
        _idCounter++;
        Id = _idCounter;
        Name = name;
        Manufacturer = manufacturer;
        SerialNumber = serialNumber;
        InstallationDate = DateTime.Now.AddYears(-7);
        Parts = parts;
        Condition = condition;
        SupportedProductType = typeof(Product);
    }

    public int Id { get; set; }

    public string? Name { get; init; }

    public string? Manufacturer { get; init; }

    public string? SerialNumber { get; }

    public DateTime InstallationDate { get; }

    public List<MachinePart>? Parts { get; }

    public MachineStatus Status { get; private protected set; } = MachineStatus.Stopped;

    public MachineCondition Condition { get; private set; }

    [JsonIgnore] public Type SupportedProductType { get; private protected init; }

    protected ProductionOrder? ActiveOrder { get; private set; }
    //*****
    public int TotalProcessedCount { get; private set; }
    public int TotalFailuresCount { get; private set; }

    public void IncrementSuccess() => TotalProcessedCount++;
    public void IncrementFailure() => TotalFailuresCount++;


    //*******
    public static void InitializeIdCounter(int maxId)
    {
        _idCounter = maxId;
    }

    public TimeSpan GetMachineAge()
    {
        return TimeSpan.FromDays((DateTime.Now - InstallationDate).TotalDays);
    }

    private double GetMachineAgeInYears()
    {
        return (DateTime.Now - InstallationDate).TotalDays / 365.25;
    }

    public void StartOrder(ProductionOrder order)
    {
        ActiveOrder = order;
        Status = MachineStatus.Running;
    }

    public bool StartMachine()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule($"[yellow]System Boot Sequences: {Name}[/]")));
        AnsiConsole.WriteLine();

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start("Analyzing diagnostic circuit registers...", ctx =>
            {
                Thread.Sleep(600);
                ctx.Status("Checking component hardware array counters...");
                Thread.Sleep(500);
            });

        foreach (var part in Parts ?? new List<MachinePart>())
            if (part.Condition == PartCondition.Critical)
            {
                Status = MachineStatus.Stopped;
                Condition = MachineCondition.Critical;

                var errorPanel = new Panel(
                    new Markup(
                        $"[red]❌ [bold]CRITICAL INITIALIZATION ERROR:[/] Component [underline]{part.Name}[/] has suffered a complete breakdown!\n" +
                        $"[grey]Action Required:[/] Dispatch an authorized engineer to run maintenance protocols.[/]")
                )
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1),
                    Header = new PanelHeader("[red bold] BOOT FAILURE [/]")
                };

                AnsiConsole.Write(errorPanel);
                return false;
            }

        Condition = GetMachineCondition();
        Status = MachineStatus.Running;

        var successPanel = new Panel(
            new Markup(
                $"[green]✔ [bold]ONLINE:[/] {Name} is fully calibrated and processing manufacturing lines.[/]")
        )
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };

        AnsiConsole.Write(successPanel);

        //****
        return true;
    }

    public void StopMachine()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule($"[red]System Shutdown Sequence: {Name}[/]")));
        AnsiConsole.WriteLine();

        if (Status == MachineStatus.Stopped)
        {
            var alreadyStoppedPanel = new Panel(
                new Markup(
                    $"[yellow]⚠ [bold]SYSTEM IDLE:[/] [underline]{Name}[/] is already stopped and sitting securely in standby mode.[/]")
            )
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1)
            };
            AnsiConsole.Write(alreadyStoppedPanel);
            return;
        }

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("red bold"))
            .Start("Initiating safe assembly line deceleration...", ctx =>
            {
                Thread.Sleep(500);
                ctx.Status("Spooling down dynamic mechanical sub-structures...");
                Thread.Sleep(600);
                ctx.Status("Isolating secondary high-voltage power relays...");
                Thread.Sleep(500);
            });

        Status = MachineStatus.Stopped;
        //******
        var stopPanel = new Panel(
            new Markup($"[red]🛑 [bold]SHUTDOWN COMPLETE:[/] {Name} has been safely isolated and powered down.\n" +
                       $"[grey]Operational State updated to:[/] [yellow bold]STOPPED (STANDBY)[/]")
        )
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader("[red bold] SYSTEM OFFLINE [/]")
        };

        AnsiConsole.Write(stopPanel);
    }

    protected void ApplyProductionWearAndTear()
    {
        if (Parts == null || Parts.Count == 0) return;

        var randomIndex = Random.Next(0, Parts.Count);
        var selectedPart = Parts[randomIndex];

        if (Random.Next(0, 100) < 80)
        {
            var oldCondition = selectedPart.Condition;
            selectedPart.DegradeStep();

            if (selectedPart.Condition != oldCondition)
            {
                AnsiConsole.WriteLine();
                if (selectedPart.Condition == PartCondition.Critical)
                {
                    AnsiConsole.Write(new Markup(
                        $"[red bold]⚡ALERT:[/] [underline]{selectedPart.Name}[/] has suffered a total breakdown! Machine safety override has been tripped!\n"));
                    Status = MachineStatus.Stopped;
                    Condition = MachineCondition.Critical;
                }
                else
                {
                    AnsiConsole.Write(new Markup(
                        $"[yellow]⚠ SYSTEM NOTICE:[/] [underline]{selectedPart.Name}[/] showing performance degradation (Moved to {selectedPart.Condition} condition).\n"));
                    int daysRemaining = GetEstimatedDaysUntilMaintenance();
                    AnsiConsole.MarkupLine($"[grey]Info: Mentenanță estimată necesară în aproximativ {daysRemaining} zile.[/]");
                    Condition = MachineCondition.Good;
                }
            }
        }
    }
    //*****
    public double CalculateEfficiency()
    {
        // 1. Calculăm healthScore fără ??
        double healthScore;

        if (Parts != null && Parts.Count > 0)
        {
            healthScore = Parts.Average(p => p.Condition switch
            {
                PartCondition.Excellent => 1.0,
                PartCondition.Good => 0.5,
                _ => 0.0
            });
        }
        else
        {
            healthScore = 1.0;
        }

        // 2. Procent bazat pe rata de succes (nu necesită ??)
        double successRate = (TotalProcessedCount + TotalFailuresCount) > 0
            ? (double)TotalProcessedCount / (TotalProcessedCount + TotalFailuresCount)
            : 1.0;

        // 3. Eficiența finală
        return (healthScore * 0.7 + successRate * 0.3) * 100;
    }
    /// ***********
    public int GetEstimatedDaysUntilMaintenance()
    {
        if (Parts == null || Parts.Count == 0)
        {
            return 30; 
        }

        // Calculăm scorul folosind LINQ
        var scores = Parts.Select(p => p.Condition switch
        {
            PartCondition.Excellent => 3,
            PartCondition.Good => 2,
            _ => 1
        });

        double averageScore = scores.Average();

        // Formula de calcul
        return (int)(averageScore * 10);
    }



    public void InspectMachine()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule($"[cyan]Diagnostics Hub: {Name}[/]")));
        AnsiConsole.WriteLine();

        var profileTable = new Table();
        profileTable.Border(TableBorder.Minimal);

        profileTable.AddColumn("[grey]Hardware Property[/]");
        profileTable.AddColumn("[grey]Assigned Value[/]");

        profileTable.AddRow("Manufacturer Identity", Manufacturer ?? "-");
        profileTable.AddRow("Factory Serial Reference", SerialNumber ?? "-");
        profileTable.AddRow("Asset Total Life Age",
            $"{GetMachineAgeInYears():F1} Years / {(int)GetMachineAge().TotalDays} Days");

        var statusColor = Status == MachineStatus.Running
            ? "green"
            : Status == MachineStatus.Stopped
                ? "yellow"
                : "orange3";
        profileTable.AddRow("Current Asset State", $"[{statusColor} bold]{Status.ToString().ToUpper()}[/]");

        AnsiConsole.Write(new Panel(profileTable)
            { Header = new PanelHeader("[bold cyan] Asset Identity Profile [/]"), Border = BoxBorder.Rounded });
        AnsiConsole.WriteLine();

        var componentTable = new Table().Border(TableBorder.Rounded);
        componentTable.AddColumn("[bold]Tracked Component Item[/]");
        componentTable.AddColumn(new TableColumn("[bold]Health Status[/]").Centered());
        componentTable.AddColumn("[bold]Technical Specifications & Diagnostics[/]");

        foreach (var part in Parts ?? new List<MachinePart>())
        {
            var p = part;

            var condVal = p.Condition.GetValueOrDefault();
            var partColor = condVal switch
            {
                PartCondition.Excellent => "green",
                PartCondition.Good => "yellow",
                PartCondition.Critical => "red bold blink",
                _ => "white"
            };

            componentTable.AddRow(
                p.Name ?? "-",
                $"[{partColor}]{p.Condition?.ToString().ToUpper() ?? "UNKNOWN"}[/]",
                p.PrintPartInfo()
            );
        }

        AnsiConsole.Write(componentTable);
        AnsiConsole.WriteLine();
        //******
        var daysLeft = GetEstimatedDaysUntilMaintenance();

        // Alegem culoarea în funcție de urgență
        var alertColor = daysLeft <= 10 ? "red" : (daysLeft <= 20 ? "yellow" : "green");

        var maintenancePanel = new Panel(new Markup(
            $"[bold]Predictive Maintenance Estimate:[/] Mașina [bold]{Name}[/] " +
            $"necesită mentenanță în aproximativ [{alertColor}]{daysLeft} zile[/]."))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("[bold blue] Info: Mentenanță Predictivă [/]")
        };

        AnsiConsole.Write(maintenancePanel);
        AnsiConsole.WriteLine();

        //****
        double efficiency = CalculateEfficiency();
        var effColor = efficiency > 80 ? "green" : (efficiency > 50 ? "yellow" : "red");

        AnsiConsole.Write(new Panel(new Markup(
            $"Eficiență Operațională: [{effColor} bold]{efficiency:F1}%[/]"))
        {
            Header = new PanelHeader("[bold]Production Efficiency Dashboard[/]")
        });
        //****

    }

    public bool NeedsRepair()
    {
        if (Condition == MachineCondition.Critical) return true;

        foreach (var part in Parts ?? new List<MachinePart>())
            if (part.Condition != PartCondition.Excellent)
                return true;

        return false;
    }

    public bool RepairMachine()
    {
        if (!NeedsRepair())
        {
            AnsiConsole.MarkupLine($"[green]{Name} does not need repairs right now.[/]");
            return false;
        }

        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule($"[yellow]Maintenance Bay: {Name}[/]")));
        AnsiConsole.WriteLine();

        var repairedParts = 0;
        foreach (var part in Parts ?? new List<MachinePart>())
            if (part.Condition != PartCondition.Excellent)
            {
                part.Repair(PartCondition.Excellent);
                repairedParts++;
            }

        Condition = GetMachineCondition();
        Status = MachineStatus.Stopped;

        var panel = new Panel(new Markup(
            $"[green]✔ Repair complete for [bold]{Name}[/].[/]\n" +
            $"[grey]Parts restored:[/] {repairedParts}\n" +
            $"[grey]Machine state:[/] {Condition} / {Status}"))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("[bold green] REPAIR COMPLETE [/]")
        };

        AnsiConsole.Write(panel);
        return true;
    }

    private MachineCondition GetMachineCondition()
    {
        foreach (var part in Parts ?? new List<MachinePart>())
        {
            if (part.Condition == PartCondition.Good) return MachineCondition.Good;

            if (part.Condition == PartCondition.Critical) return MachineCondition.Critical;
        }

        return MachineCondition.Excellent;
    }

    protected static bool TryProcessMotherboard(Product product, BoardState requiredInputState,
        BoardState nextState, string stageName)
    {
        if (product is not Motherboard board)
        {
            AnsiConsole.MarkupLine($"[red]Error: This machine cannot process {product.GetType().Name}[/]");
            return false;
        }

        if (board.CurrentState != requiredInputState)
        {
            AnsiConsole.MarkupLine(
                $"[yellow]Warning: {board.Name} is not in the expected {requiredInputState} state for {stageName}.[/]");
            return false;
        }

        board.TransitionTo(nextState);
        AnsiConsole.MarkupLine(
            $"[green]Successfully completed {stageName} for {board.Name}. New state: {nextState}[/]");
        return true;
    }

    public abstract bool Produce(Product product);
}

public class LithographyMachine : Machine
{
    public LithographyMachine(string name, string manufacturer, string serialNumber,
        List<MachinePart> parts, MachineCondition condition)
        : base(name, manufacturer, serialNumber, parts, condition)
    {
        SupportedProductType = typeof(Microprocessor);
    }

    public override bool Produce(Product blueprint)
    {
        //Factory.ShowInventoryAlerts();

        if (ActiveOrder == null || ActiveOrder.IsComplete)
        {
            Status = MachineStatus.Stopped;
            AnsiConsole.Write(new Markup($"[green]✔ Successfully manufactured: {blueprint.Name}[/]\n"));
            return true;
        }

        if (blueprint.GetType() != SupportedProductType)
            throw new InvalidOperationException($"This machine only produces {SupportedProductType.Name}!");

        if (Status != MachineStatus.Running)
        {
            AnsiConsole.Write(new Markup(
                $"[red]❌ Cannot produce {blueprint.Name}. Machine is offline. Please boot or repair it first.[/]\n"));
            return false;
        }

        AnsiConsole.Write(
            new Markup($"[cyan]🏭 Starting processing sequence for: [underline]{blueprint.Name}[/][/]\n"));

        AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .SpinnerStyle(Style.Parse("cyan bold"))
            .Start("Exposing wafer structure using optical masks...", _ => { Thread.Sleep(800); });
        ApplyProductionWearAndTear();
        return true;
    }
}

public class SmtMachine : Machine // Solder Paste Printer
{
    public SmtMachine(string name, string manufacturer, string serialNumber, List<MachinePart> parts,
        MachineCondition condition)
        : base(name, manufacturer, serialNumber, parts, condition)
    {
        SupportedProductType = typeof(Motherboard);
    }

    public override bool Produce(Product product)
    {
        return TryProcessMotherboard(product, BoardState.BlankBoard, BoardState.SolderPrinted,
            "Solder Paste Printing");
    }
}

public class PaPMachine(
    string name,
    string manufacturer,
    string serialNumber,
    List<MachinePart> parts,
    MachineCondition condition)
    : Machine(name, manufacturer, serialNumber, parts, condition) // Pick and Place
{
    public override bool Produce(Product product)
    {
        return TryProcessMotherboard(product, BoardState.SolderPrinted, BoardState.ComponentsPlaced,
            "Pick and Place Assembly");
    }
}

public class ReflowOven(
    string name,
    string manufacturer,
    string serialNumber,
    List<MachinePart> parts,
    MachineCondition condition)
    : Machine(name, manufacturer, serialNumber, parts, condition)
{
    public override bool Produce(Product product)
    {
        return TryProcessMotherboard(product, BoardState.ComponentsPlaced, BoardState.BakedAndSoldered,
            "Reflow Baking");
    }


    //***


}