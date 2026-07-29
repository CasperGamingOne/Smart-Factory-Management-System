using System.Text.Json.Serialization;
using Spectre.Console;

namespace Smart_Factory_Management_System;

public enum MachineStatus
{
    Running,
    Stopped
}

public enum MachineCondition
{
    Excellent,
    Good,
    Critical
}

[JsonDerivedType(typeof(LitographyMachine), "litographyMachine")]
[JsonDerivedType(typeof(SmtMachine), "smtMachine")]
[JsonDerivedType(typeof(PaPMachine), "papMachine")]
[JsonDerivedType(typeof(ReflowOven), "reflowOven")]
public abstract class Machine
{
    private static int _idCounter;

    private static readonly Random Random = new();

    protected Machine(string name, string manufacturer, string serialNumber, List<MachinePart> parts,
        MachineCondition condition, int id = 0)
    {
        if (id > 0)
        {
            Id = id;
        }
        else
        {
            _idCounter++;
            Id = _idCounter;
        }

        Name = name;
        Manufacturer = manufacturer;
        SerialNumber = serialNumber;
        InstallationDate = DateTime.Now.AddYears(-7);
        Parts = parts;
        Condition = condition;
        SupportedProductType = typeof(Product);
    }

    public int Id { get; }

    public string? Name { get; }

    public string? Manufacturer { get; }

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

    public static bool SilentMode { get; set; }

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
        AnsiConsole.Write(Align.Left(new Rule(string.Format($"[yellow]{Machines.BootSequence}[/]", Name))));
        AnsiConsole.WriteLine();

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start(Machines.BootSpinnerAnalyzing, ctx =>
            {
                Thread.Sleep(600);
                ctx.Status(Machines.BootSpinnerChecking);
                Thread.Sleep(500);
            });

        foreach (var part in Parts ?? new List<MachinePart>())
            if (part.Condition == PartCondition.Critical)
            {
                Status = MachineStatus.Stopped;
                Condition = MachineCondition.Critical;

                var errorPanel = new Panel(
                    new Markup(
                        string.Format(Machines.CriticalInitError, part.Name))
                )
                {
                    Border = BoxBorder.Rounded,
                    Padding = new Padding(1, 1, 1, 1),
                    Header = new PanelHeader($"[red bold]{Machines.BootFailureHeader}[/]")
                };

                AnsiConsole.Write(errorPanel);
                return false;
            }

        Condition = GetMachineCondition();
        Status = MachineStatus.Running;

        var successPanel = new Panel(
            new Markup(
                string.Format(Machines.OnlineMessage, Name))
        )
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1)
        };

        AnsiConsole.Write(successPanel);

        //****
        return true;
    }

    protected void ApplyProductionWearAndTear()
    {
        if (Parts == null || Parts.Count == 0) return;

        var randomIndex = Random.Next(0, Parts.Count);
        var selectedPart = Parts[randomIndex];

        // 15% chance overall for wear-and-tear per production cycle:
        //   - If part is Excellent, it degrades one step to Good.
        //   - If part is Good, it degrades one step to Critical.
        //   - Critical parts stay Critical.
        if (Random.Shared.Next(0, 100) < 15)
        {
            var oldCondition = selectedPart.Condition;

            if (selectedPart.Condition == PartCondition.Excellent)
                selectedPart.DegradeStep(); // Excellent -> Good
            else if (selectedPart.Condition == PartCondition.Good)
                selectedPart.DegradeStep(); // Good -> Critical
            // If already Critical, nothing changes

            if (selectedPart.Condition != oldCondition)
            {
                if (selectedPart.Condition == PartCondition.Critical)
                {
                    if (!SilentMode)
                    {
                        AnsiConsole.WriteLine();
                        AnsiConsole.Write(new Markup(
                            string.Format(Machines.AlertCriticalPart, selectedPart.Name)));
                    }

                    Status = MachineStatus.Stopped;
                    Condition = MachineCondition.Critical;
                }
                else
                {
                    if (!SilentMode)
                    {
                        AnsiConsole.WriteLine();
                        AnsiConsole.Write(new Markup(
                            string.Format(Machines.WarningDegradation, selectedPart.Name, selectedPart.Condition)));
                    }

                    Condition = MachineCondition.Good;
                }
            }
        }
    }

    public double CalculateEfficiency()
    {
        if (Parts == null || Parts.Count == 0) return 0;

        // Mathematical formula: the average of part condition scores multiplied by the product of those scores.
        // Excellent = 1.0, Good = 0.85 (drops by 15%), Critical = 0.0.
        // If any part reaches Critical, the product terms will drive the entire efficiency score to 0.0 naturally.
        var partFactors = Parts.Select(p => p.Condition switch
        {
            PartCondition.Excellent => 1.0,
            PartCondition.Good => 0.85,
            _ => 0.0
        }).ToList();

        var averageFactor = partFactors.Average();
        var productFactor = partFactors.Aggregate(1.0, (acc, val) => acc * val);

        return averageFactor * productFactor * 100.0;
    }

    public int GetEstimatedDaysUntilMaintenance()
    {
        if (Parts == null || Parts.Count == 0) return 30;

        // Formula derived from ApplyProductionWearAndTear randomness:
        // - Random selection of 1 of N parts (1/N chance).
        // - 15% degradation probability (0.15).
        // - Degradation is strictly one-step: Excellent -> Good -> Critical.
        // - Expected production cycles for a part to degrade by 1 step is (N / 0.15).
        // - Excellent parts have 2 steps to critical, Good parts have 1 step, Critical parts have 0 steps.
        // - Machine fails (and requires repair) as soon as the first part goes Critical.
        // - Remaining Cycles Estimate = min(StepsRemaining) * (N / 0.15).
        var n = Parts.Count;
        var remainingSteps = Parts.Select(p => p.Condition switch
        {
            PartCondition.Excellent => 2,
            PartCondition.Good => 1,
            _ => 0
        });

        var minSteps = remainingSteps.Min();
        if (minSteps == 0) return 0;

        var expectedCycles = minSteps * n / 0.15;
        return (int)Math.Round(expectedCycles);
    }

    public void InspectMachine()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule(string.Format($"[cyan]{Machines.DiagnosticHub}[/]", Name))));
        AnsiConsole.WriteLine();

        var profileTable = new Table();
        profileTable.Border(TableBorder.Minimal);

        profileTable.AddColumn(Machines.HardwarePropertyColumn);
        profileTable.AddColumn(Machines.AssignedValueColumn);

        profileTable.AddRow(Machines.ManufacturerIdentity, Manufacturer ?? "-");
        profileTable.AddRow(Machines.FactorySerialRef, SerialNumber ?? "-");
        profileTable.AddRow(Machines.AssetLifeAge,
            string.Format(Machines.AssetLifeAgeValue, GetMachineAgeInYears(), (int)GetMachineAge().TotalDays));

        var statusColor = Status == MachineStatus.Running
            ? "green"
            : Status == MachineStatus.Stopped
                ? "yellow"
                : "orange3";
        profileTable.AddRow(Machines.CurrentAssetState, $"[{statusColor} bold]{Status.ToString().ToUpper()}[/]");

        AnsiConsole.Write(new Panel(profileTable)
        {
            Header = new PanelHeader($"[bold cyan]{Machines.AssetIdentityProfileHeader}[/]"), Border = BoxBorder.Rounded
        });
        AnsiConsole.WriteLine();

        var componentTable = new Table().Border(TableBorder.Rounded);
        componentTable.AddColumn(Machines.TrackedComponentHeader);
        componentTable.AddColumn(new TableColumn(Machines.HealthStatusHeader).Centered());
        componentTable.AddColumn(Machines.TechSpecsHeader);

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

        var daysLeft = GetEstimatedDaysUntilMaintenance();
        var alertColor = daysLeft <= 5 ? "red" : daysLeft <= 10 ? "yellow" : "green";

        var maintenancePanel = new Panel(new Markup(
            $"[bold]Predictive Maintenance Estimate:[/] Machine [bold]{Name}[/] " +
            $"requires maintenance in approximately [{alertColor}]{daysLeft} active production cycles (days)[/]."))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader("[bold blue] Info: Predictive Maintenance [/]")
        };

        AnsiConsole.Write(maintenancePanel);
        AnsiConsole.WriteLine();

        double efficiency = CalculateEfficiency();
        var effColor = efficiency > 80 ? "green" : (efficiency > 50 ? "yellow" : "red");

        AnsiConsole.Write(new Panel(new Markup(
            $"Operational Efficiency: [{effColor} bold]{efficiency:F1}%[/]"))
        {
            Header = new PanelHeader("[bold]Production Efficiency Dashboard[/]")
        });
        AnsiConsole.WriteLine();
    }

    public bool NeedsRepair()
    {
        if (Condition == MachineCondition.Critical) return true;

        foreach (var part in Parts ?? new List<MachinePart>())
            if (part.Condition != PartCondition.Excellent)
                return true;

        return false;
    }

    public void RepairMachine()
    {
        if (!NeedsRepair())
        {
            AnsiConsole.MarkupLine(string.Format(Machines.DoesNotNeedRepairs, Name));
            return;
        }

        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule(string.Format($"[yellow]{Machines.MaintenanceBay}[/]", Name))));
        AnsiConsole.WriteLine();

        var repairedParts = 0;

        AnsiConsole.Status()
            .Spinner(Spinner.Known.Dots)
            .SpinnerStyle(Style.Parse("yellow bold"))
            .Start(string.Format(Machines.RepairSpinnerOpeningPanels, Name), ctx =>
            {
                Thread.Sleep(800);
                foreach (var part in Parts ?? new List<MachinePart>())
                    if (part.Condition != PartCondition.Excellent)
                    {
                        ctx.Status(string.Format(Machines.RepairSpinnerServicingPart, part.Name));
                        Thread.Sleep(1000);
                        part.Repair(PartCondition.Excellent);
                        repairedParts++;
                    }

                ctx.Status(Machines.RepairSpinnerDiagnostics);
                Thread.Sleep(800);
            });

        Condition = GetMachineCondition();
        Status = MachineStatus.Stopped;

        var panel = new Panel(new Markup(
            string.Format(Machines.RepairCompleteMsg, Name, repairedParts, Condition, Status)))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader($"[bold green]{Machines.RepairCompleteHeader}[/]")
        };

        AnsiConsole.Write(panel);
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

    public abstract bool Produce(Product product);
}

public class LitographyMachine : Machine
{
    public LitographyMachine(string name, string manufacturer, string serialNumber,
        List<MachinePart> parts, MachineCondition condition, int id = 0)
        : base(name, manufacturer, serialNumber, parts, condition, id)
    {
        SupportedProductType = typeof(Microprocessor);
    }

    public override bool Produce(Product blueprint)
    {
        //Factory.ShowInventoryAlerts();

        if (ActiveOrder == null || ActiveOrder.IsComplete)
        {
            Status = MachineStatus.Stopped;
            if (!SilentMode) AnsiConsole.Write(new Markup(string.Format(Machines.LitographySuccess, blueprint.Name)));
            return true;
        }

        if (blueprint.GetType() != SupportedProductType)
            throw new InvalidOperationException($"This machine only produces {SupportedProductType.Name}!");

        if (!SilentMode)
        {
            AnsiConsole.Write(
                new Markup(string.Format(Machines.LitographyStart, blueprint.Name)));

            AnsiConsole.Status()
                .Spinner(Spinner.Known.BouncingBar)
                .SpinnerStyle(Style.Parse("cyan bold"))
                .Start(Machines.LitographySpinner, _ => { Thread.Sleep(800); });
        }

        ApplyProductionWearAndTear();
        return true;
    }
}

public class SmtMachine : Machine // Solder Paste Printer
{
    public SmtMachine(string name, string manufacturer, string serialNumber, List<MachinePart> parts,
        MachineCondition condition, int id = 0)
        : base(name, manufacturer, serialNumber, parts, condition, id)
    {
        SupportedProductType = typeof(Motherboard);
    }

    public override bool Produce(Product product)
    {
        if (product is not Motherboard board)
        {
            if (!SilentMode)
                AnsiConsole.MarkupLine(string.Format(Machines.ProcessErrorUnsupported, product.GetType().Name));
            return false;
        }

        if (board.CurrentState != BoardState.BlankBoard)
        {
            if (!SilentMode)
                AnsiConsole.MarkupLine(
                    string.Format(Machines.ProcessWarningUnexpectedState, board.Name, BoardState.BlankBoard,
                        "Solder Paste Printing"));
            return false;
        }

        board.TransitionTo(BoardState.SolderPrinted);
        if (!SilentMode)
            AnsiConsole.MarkupLine(
                string.Format(Machines.ProcessSuccessState, board.Name, BoardState.SolderPrinted,
                    "Solder Paste Printing"));
        ApplyProductionWearAndTear();
        return true;
    }
}

public class PaPMachine(
    string name,
    string manufacturer,
    string serialNumber,
    List<MachinePart> parts,
    MachineCondition condition,
    int id = 0)
    : Machine(name, manufacturer, serialNumber, parts, condition, id) // Pick and Place
{
    public override bool Produce(Product product)
    {
        if (product is not Motherboard board)
        {
            if (!SilentMode)
                AnsiConsole.MarkupLine(string.Format(Machines.ProcessErrorUnsupported, product.GetType().Name));
            return false;
        }

        if (board.CurrentState != BoardState.SolderPrinted)
        {
            if (!SilentMode)
                AnsiConsole.MarkupLine(
                    string.Format(Machines.ProcessWarningUnexpectedState, board.Name, BoardState.SolderPrinted,
                        "Pick and Place Assembly"));
            return false;
        }

        board.TransitionTo(BoardState.ComponentsPlaced);
        if (!SilentMode)
            AnsiConsole.MarkupLine(
                string.Format(Machines.ProcessSuccessState, board.Name, BoardState.ComponentsPlaced,
                    "Pick and Place Assembly"));
        ApplyProductionWearAndTear();
        return true;
    }
}

public class ReflowOven(
    string name,
    string manufacturer,
    string serialNumber,
    List<MachinePart> parts,
    MachineCondition condition,
    int id = 0)
    : Machine(name, manufacturer, serialNumber, parts, condition, id)
{
    public override bool Produce(Product product)
    {
        if (product is not Motherboard board)
        {
            if (!SilentMode)
                AnsiConsole.MarkupLine(string.Format(Machines.ProcessErrorUnsupported, product.GetType().Name));
            return false;
        }

        if (board.CurrentState != BoardState.ComponentsPlaced)
        {
            if (!SilentMode)
                AnsiConsole.MarkupLine(
                    string.Format(Machines.ProcessWarningUnexpectedState, board.Name, BoardState.ComponentsPlaced,
                        "Reflow Baking"));
            return false;
        }

        board.TransitionTo(BoardState.BakedAndSoldered);
        if (!SilentMode)
            AnsiConsole.MarkupLine(
                string.Format(Machines.ProcessSuccessState, board.Name, BoardState.BakedAndSoldered, "Reflow Baking"));
        ApplyProductionWearAndTear();
        return true;
    }


    //***
}