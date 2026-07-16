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

    public void StopMachine()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule(string.Format($"[red]{Machines.ShutdownSequence}[/]", Name))));
        AnsiConsole.WriteLine();

        if (Status == MachineStatus.Stopped)
        {
            var alreadyStoppedPanel = new Panel(
                new Markup(
                    string.Format(Machines.AlreadyStopped, Name))
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
            .Start(Machines.SafeDecelerationSpinner, ctx =>
            {
                Thread.Sleep(500);
                ctx.Status(Machines.SpoolDownSpinner);
                Thread.Sleep(600);
                ctx.Status(Machines.IsolateRelaysSpinner);
                Thread.Sleep(500);
            });

        Status = MachineStatus.Stopped;
        //******
        var stopPanel = new Panel(
            new Markup(string.Format(Machines.ShutdownComplete, Name))
        )
        {
            Border = BoxBorder.Rounded,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader($"[red bold]{Machines.SystemOfflineHeader}[/]")
        };

        AnsiConsole.Write(stopPanel);
    }

    protected void ApplyProductionWearAndTear()
    {
        if (Parts == null || Parts.Count == 0) return;

        var randomIndex = Random.Next(0, Parts.Count);
        var selectedPart = Parts[randomIndex];

        if (Random.Next(0, 100) < 20)
        {
            var oldCondition = selectedPart.Condition;
            selectedPart.DegradeStep();

            if (selectedPart.Condition != oldCondition)
            {
                AnsiConsole.WriteLine();
                if (selectedPart.Condition == PartCondition.Critical)
                {
                    AnsiConsole.Write(new Markup(
                        string.Format(Machines.AlertCriticalPart, selectedPart.Name)));
                    Status = MachineStatus.Stopped;
                    Condition = MachineCondition.Critical;
                }
                else
                {
                    AnsiConsole.Write(new Markup(
                        string.Format(Machines.WarningDegradation, selectedPart.Name, selectedPart.Condition)));
                    Condition = MachineCondition.Good;
                }
            }
        }
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
            AnsiConsole.MarkupLine(string.Format(Machines.DoesNotNeedRepairs, Name));
            return false;
        }

        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule(string.Format($"[yellow]{Machines.MaintenanceBay}[/]", Name))));
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
            string.Format(Machines.RepairCompleteMsg, Name, repairedParts, Condition, Status)))
        {
            Border = BoxBorder.Rounded,
            Header = new PanelHeader($"[bold green]{Machines.RepairCompleteHeader}[/]")
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
            AnsiConsole.MarkupLine(string.Format(Machines.ProcessErrorUnsupported, product.GetType().Name));
            return false;
        }

        if (board.CurrentState != requiredInputState)
        {
            AnsiConsole.MarkupLine(
                string.Format(Machines.ProcessWarningUnexpectedState, board.Name, requiredInputState, stageName));
            return false;
        }

        board.TransitionTo(nextState);
        AnsiConsole.MarkupLine(
            string.Format(Machines.ProcessSuccessState, board.Name, nextState, stageName));
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
        if (ActiveOrder == null || ActiveOrder.IsComplete)
        {
            Status = MachineStatus.Stopped;
            AnsiConsole.Write(new Markup(string.Format(Machines.LithographySuccess, blueprint.Name)));
            return true;
        }

        if (blueprint.GetType() != SupportedProductType)
            throw new InvalidOperationException($"This machine only produces {SupportedProductType.Name}!");

        if (Status != MachineStatus.Running)
        {
            AnsiConsole.Write(new Markup(
                string.Format(Machines.LithographyOffline, blueprint.Name)));
            return false;
        }

        AnsiConsole.Write(
            new Markup(string.Format(Machines.LithographyStart, blueprint.Name)));

        AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .SpinnerStyle(Style.Parse("cyan bold"))
            .Start(Machines.LithographySpinner, _ => { Thread.Sleep(800); });
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
}