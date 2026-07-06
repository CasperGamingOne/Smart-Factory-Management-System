using Spectre.Console;

namespace Smart_Factory_Management_System
{
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

    internal abstract class Machine
    {
        private static int _idCounter;

        private static readonly Random Random = new();

        protected Machine(string machineName, string machineManufacturer, string machineSerial, MachinePart[] parts,
            MachineCondition condition)
        {
            _idCounter++;
            Id = _idCounter;
            Name = machineName;
            Manufacturer = machineManufacturer;
            SerialNumber = machineSerial;
            InstallationDate = DateTime.Now.AddYears(-7);
            Parts = parts;
            Condition = condition;
            SupportedProductType =
                typeof(Product); // Default to base Product type; override in derived classes as needed
        }

        public int Id { get; private protected set; }

        public string? Name { get; private protected set; }

        public string? Manufacturer { get; private protected set; }

        private string? SerialNumber { get; }

        private DateTime InstallationDate { get; }

        private MachinePart[]? Parts { get; }

        public MachineStatus Status { get; private protected set; } = MachineStatus.Stopped;

        public MachineCondition Condition { get; private set; }

        public Type SupportedProductType { get; private protected init; }

        protected ProductionOrder? ActiveOrder { get; private set; }

        public TimeSpan GetMachineAge()
        {
            return TimeSpan.FromDays((DateTime.Now - InstallationDate).TotalDays);
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

            // Null-checked safety verification over the array loop
            foreach (var part in Parts ?? Array.Empty<MachinePart>())
            {
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
            }

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
            return true;
        }

        public void StopMachine()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(Align.Left(new Rule($"[red]System Shutdown Sequence: {Name}[/]")));
            AnsiConsole.WriteLine();

            // Safe State Guard: If it's already stopped, exit cleanly without blinking animations
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

            // High-precision simulated spin down sequence
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

            // Commit State Change
            Status = MachineStatus.Stopped;

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
            // 1. Filter instantiated parts into a clean tracking bucket
            int activePartsCount = 0;
            foreach (var unused in Parts ?? Array.Empty<MachinePart>())
            {
                activePartsCount++;
            }

            if (activePartsCount == 0) return;

            // 2. Select a single random active part from the array
            var randomIndex = Random.Next(0, activePartsCount);
            int currentStep = 0;
            MachinePart? selectedPart = null;

            foreach (var part in Parts ?? Array.Empty<MachinePart>())
            {
                if (currentStep == randomIndex)
                {
                    selectedPart = part;
                    break;
                }

                currentStep++;
            }

            if (selectedPart == null) return;

            // 3. Roll a completely random chance for damage (e.g., 20% chance to accumulate wear per product cycle)
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
                            $"[red bold]⚡ALERT:[/] [underline]{selectedPart.Name}[/] has suffered a total breakdown! Machine safety override has been tripped!\n"));
                        Status = MachineStatus.Stopped;
                        Condition = MachineCondition.Critical;
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup(
                            $"[yellow]⚠ SYSTEM NOTICE:[/] [underline]{selectedPart.Name}[/] showing performance degradation (Moved to {selectedPart.Condition} condition).\n"));
                        Condition = MachineCondition.Good;
                    }
                }
            }
        }

        public void InspectMachine()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(Align.Left(new Rule($"[cyan]Diagnostics Hub: {Name}[/]")));
            AnsiConsole.WriteLine();

            // 1. Identity Profile Table
            var profileTable = new Table();
            profileTable.Border(TableBorder.Minimal);

            profileTable.AddColumn("[grey]Hardware Property[/]");
            profileTable.AddColumn("[grey]Assigned Value[/]");

            // Ensure all secondary parameters evaluate explicitly to strings
            profileTable.AddRow("Manufacturer Identity", Manufacturer ?? "-");
            profileTable.AddRow("Factory Serial Reference", SerialNumber ?? "-");
            profileTable.AddRow("Asset Total Life Age", $"{GetMachineAge()} Years Old");

            var statusColor = Status == MachineStatus.Running
                ? "green"
                : Status == MachineStatus.Stopped
                    ? "yellow"
                    : "orange3";
            profileTable.AddRow("Current Asset State", $"[{statusColor} bold]{Status.ToString().ToUpper()}[/]");

            AnsiConsole.Write(new Panel(profileTable)
                { Header = new PanelHeader("[bold cyan] Asset Identity Profile [/]"), Border = BoxBorder.Rounded });
            AnsiConsole.WriteLine();

            // 2. Component Health Table
            var componentTable = new Table().Border(TableBorder.Rounded);
            componentTable.AddColumn("[bold]Tracked Component Item[/]");
            componentTable.AddColumn(new TableColumn("[bold]Health Status[/]").Centered());
            componentTable.AddColumn("[bold]Technical Specifications & Diagnostics[/]");

            foreach (var part in Parts ?? Array.Empty<MachinePart>())
            {
                var p = part;

                var condVal = p.Condition.GetValueOrDefault();
                string partColor = condVal switch
                {
                    PartCondition.Excellent => "green",
                    PartCondition.Good => "yellow",
                    PartCondition.Critical => "red bold blink",
                    _ => "white"
                };

                // All three arguments here are guaranteed to be strings, resolving the error completely
                componentTable.AddRow(
                    p.Name ?? "-",
                    $"[{partColor}]{(p.Condition?.ToString().ToUpper() ?? "UNKNOWN")}[/]",
                    p.PrintPartInfo()
                );
            }

            AnsiConsole.Write(componentTable);
            AnsiConsole.WriteLine();
        }

        protected static bool TryProcessMotherboard(Product product, BoardState requiredInputState,
            BoardState nextState, string stageName)
        {
            if (product is not Motherboard board)
            {
                Console.WriteLine($"[red]Error: This machine cannot process {product.GetType().Name}[/]");
                return false;
            }

            if (board.CurrentState != requiredInputState)
            {
                Console.WriteLine(
                    $"[yellow]Warning: {board.Name} is not in the expected {requiredInputState} state for {stageName}.[/]");
                return false;
            }

            board.TransitionTo(nextState);
            Console.WriteLine($"[green]Successfully completed {stageName} for {board.Name}. New state: {nextState}[/]");
            return true;
        }

        public abstract bool Produce(Product product);
    }

    internal class LitographyMachine : Machine
    {
        public LitographyMachine(string machineName, string machineManufacturer, string machineSerial,
            MachinePart[] parts, MachineCondition condition)
            : base(machineName, machineManufacturer, machineSerial, parts, condition)
        {
            SupportedProductType = typeof(Microprocessor);
        }

        public override bool Produce(Product blueprint)
        {
            if (ActiveOrder == null || ActiveOrder.IsComplete)
            {
                Status = MachineStatus.Stopped;
                AnsiConsole.Write(new Markup($"[green]✔ Successfully manufactured: {blueprint.Name}[/]\n"));
                return true;
            }

            if (blueprint.GetType() != SupportedProductType)
            {
                throw new InvalidOperationException($"This machine only produces {SupportedProductType.Name}!");
            }

            // Block production if machine is stopped or broken
            if (Status != MachineStatus.Running)
            {
                AnsiConsole.Write(new Markup(
                    $"[red]❌ Cannot produce {blueprint.Name}. Machine is offline. Please boot or repair it first.[/]\n"));
                return false;
            }

            AnsiConsole.Write(
                new Markup($"[cyan]🏭 Starting processing sequence for: [underline]{blueprint.Name}[/][/]\n"));

            // Output progress bar or loading spinner
            AnsiConsole.Status()
                .Spinner(Spinner.Known.BouncingBar)
                .SpinnerStyle(Style.Parse("cyan bold"))
                .Start("Exposing wafer structure using optical masks...", _ => { Thread.Sleep(800); });
            // Triggers completely random simulation tracking chance for part failure
            ApplyProductionWearAndTear();
            return true;
        }
    }

    internal class SmtMachine : Machine // Solder Paste Printer
    {
        public SmtMachine(string machineName, string machineManufacturer, string machineSerial, MachinePart[] parts,
            MachineCondition condition)
            : base(machineName, machineManufacturer, machineSerial, parts, condition)
        {
            SupportedProductType = typeof(Motherboard);
        }

        public override bool Produce(Product product)
        {
            return TryProcessMotherboard(product, BoardState.BlankBoard, BoardState.SolderPrinted,
                "Solder Paste Printing");
        }
    }

    internal class PaPMachine : Machine // Pick and Place
    {
        public PaPMachine(string machineName, string machineManufacturer, string machineSerial, MachinePart[] parts,
            MachineCondition condition)
            : base(machineName, machineManufacturer, machineSerial, parts, condition)
        {
        }

        public override bool Produce(Product product)
        {
            return TryProcessMotherboard(product, BoardState.SolderPrinted, BoardState.ComponentsPlaced,
                "Pick and Place Assembly");
        }
    }

    internal class ReflowOven : Machine
    {
        public ReflowOven(string machineName, string machineManufacturer, string machineSerial, MachinePart[] parts,
            MachineCondition condition)
            : base(machineName, machineManufacturer, machineSerial, parts, condition)
        {
        }

        public override bool Produce(Product product)
        {
            return TryProcessMotherboard(product, BoardState.ComponentsPlaced, BoardState.BakedAndSoldered,
                "Reflow Baking");
        }
    }
}