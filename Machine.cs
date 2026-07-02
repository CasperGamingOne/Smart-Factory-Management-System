using Spectre.Console;

namespace Smart_Factory_Management_System
{
    public enum MachineStatus { Running, Stopped, Maintenance }

    public enum MachineCondition { Excellent, Good, Critical }

    internal abstract class Machine
    {
        private static int idCounter = 0;

        public int Id { get; private protected set; }

        public string? Name { get; private protected set; }

        public string? Manufacturer { get; private protected set; }

        public string? SerialNumber { get; private protected set; }

        public DateTime InstallationDate { get; private protected set; }

        public MachinePart[]? Parts { get; private protected set; }

        public MachineStatus Status { get; private protected set; } = MachineStatus.Stopped;

        public MachineCondition Condition { get; private protected set; }

        public Type SupportedProductType { get; private protected set; }

        public Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
        {
            idCounter++;
            Id = idCounter;
            Name = machine_name;
            Manufacturer = machine_manufacturer;
            SerialNumber = machine_serial;
            InstallationDate = DateTime.Now.AddYears(-7);
            Parts = parts;
            Condition = condition;
            SupportedProductType = typeof(Product); // Default to base Product type; override in derived classes as needed
        }

        private static readonly Random _random = new Random();

        public TimeSpan GetMachineAge()
        {
            return TimeSpan.FromDays((DateTime.Now - InstallationDate).TotalDays);
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
            foreach (var part in Parts)
            {
                if (part != null && part.Condition == PartCondition.Critical)
                {
                    Status = MachineStatus.Stopped;
                    Condition = MachineCondition.Critical;

                    var errorPanel = new Panel(
                        new Markup($"[red]❌ [bold]CRITICAL INITIALIZATION ERROR:[/] Component [underline]{part.Name}[/] has suffered a complete breakdown!\n" +
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
                new Markup($"[green]✔ [bold]ONLINE:[/] {Name} is fully calibrated and processing manufacturing lines.[/]")
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
                    new Markup($"[yellow]⚠ [bold]SYSTEM IDLE:[/] [underline]{Name}[/] is already stopped and sitting securely in standby mode.[/]")
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
            foreach (var part in Parts)
            {
                if (part != null) activePartsCount++;
            }

            if (activePartsCount == 0) return;

            // 2. Select a single random active part from the array
            int randomIndex = _random.Next(0, activePartsCount);
            int currentStep = 0;
            MachinePart? selectedPart = null;

            foreach (var part in Parts)
            {
                if (part != null)
                {
                    if (currentStep == randomIndex)
                    {
                        selectedPart = part;
                        break;
                    }
                    currentStep++;
                }
            }

            if (selectedPart == null) return;

            // 3. Roll a completely random chance for damage (e.g., 20% chance to accumulate wear per product cycle)
            if (_random.Next(0, 100) < 20)
            {
                var oldCondition = selectedPart.Condition;
                selectedPart.DegradeStep();

                if (selectedPart.Condition != oldCondition)
                {
                    AnsiConsole.WriteLine();
                    if (selectedPart.Condition == PartCondition.Critical)
                    {
                        AnsiConsole.Write(new Markup($"[red bold]⚡ALERT:[/] [underline]{selectedPart.Name}[/] has suffered a total breakdown! Machine safety override has been tripped!\n"));
                        Status = MachineStatus.Stopped;
                        Condition = MachineCondition.Critical;
                    }
                    else
                    {
                        AnsiConsole.Write(new Markup($"[yellow]⚠ SYSTEM NOTICE:[/] [underline]{selectedPart.Name}[/] showing performance degradation (Moved to {selectedPart.Condition} condition).\n"));
                        Condition = MachineCondition.Good;
                    }
                }
            }
        }

        public void RepairMachine()
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(Align.Left(new Rule($"[orange3]Engineering Maintenance Menu: {Name}[/]")));
            AnsiConsole.WriteLine();

            Status = MachineStatus.Maintenance;

            AnsiConsole.Status()
                .Spinner(Spinner.Known.Aesthetic)
                .SpinnerStyle(Style.Parse("orange3 bold"))
                .Start("Accessing core system layout frameworks...", ctx =>
                {
                    Thread.Sleep(600);

                    foreach (var part in Parts)
                    {
                        if (part == null) continue;

                        // Only maintain degraded or broken elements
                        if (part.Condition == PartCondition.Critical || part.Condition == PartCondition.Good)
                        {
                            ctx.Status($"Overhauling assembly unit: [underline]{part.Name}[/]...");
                            Thread.Sleep(700);

                            // Completely random chance calculation: 50% Good, 50% Excellent
                            PartCondition restoredOutcome = _random.Next(0, 2) == 0
                                ? PartCondition.Good
                                : PartCondition.Excellent;

                            part.Repair(restoredOutcome);
                        }
                    }

                    ctx.Status("Conducting structural integrity test loops...");
                    Thread.Sleep(500);
                });

            // Reset master indicators safely back to standby defaults
            Condition = MachineCondition.Excellent;
            Status = MachineStatus.Stopped;

            var repairPanel = new Panel(
                new Markup("[green]✔ [bold]REPAIR ORDER COMPLETE:[/] Engineering maintenance script completed successfully!\n" +
                           "[grey]All internal mechanical alerts cleared. Machine is standing by.[/]")
            )
            {
                Border = BoxBorder.Rounded,
                Padding = new Padding(1, 1, 1, 1),
                Header = new PanelHeader("[green bold] MACHINE RESTORED [/]")
            };

            AnsiConsole.Write(repairPanel);
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
            profileTable.AddRow("Manufacturer Identity", Manufacturer.ToString());
            profileTable.AddRow("Factory Serial Reference", SerialNumber.ToString());
            profileTable.AddRow("Asset Total Life Age", $"{GetMachineAge()} Years Old");

            string statusColor = Status == MachineStatus.Running ? "green" : (Status == MachineStatus.Stopped ? "yellow" : "orange3");
            profileTable.AddRow("Current Asset State", $"[{statusColor} bold]{Status.ToString().ToUpper()}[/]");

            AnsiConsole.Write(new Panel(profileTable) { Header = new PanelHeader("[bold cyan] Asset Identity Profile [/]"), Border = BoxBorder.Rounded });
            AnsiConsole.WriteLine();

            // 2. Component Health Table
            var componentTable = new Table().Border(TableBorder.Rounded);
            componentTable.AddColumn("[bold]Tracked Component Item[/]");
            componentTable.AddColumn(new TableColumn("[bold]Health Status[/]").Centered());
            componentTable.AddColumn("[bold]Technical Specifications & Diagnostics[/]");

            foreach (var part in Parts)
            {
                if (part == null) continue;

                string partColor = part.Condition switch
                {
                    PartCondition.Excellent => "green",
                    PartCondition.Good => "yellow",
                    PartCondition.Critical => "red bold blink",
                    _ => "white"
                };

                // All three arguments here are guaranteed to be strings, resolving the error completely
                componentTable.AddRow(
                    part.Name.ToString(),
                    $"[{partColor}]{part.Condition.ToString().ToUpper()}[/]",
                    part.PrintPartInfo()
                );
            }

            AnsiConsole.Write(componentTable);
            AnsiConsole.WriteLine();
        }

        public abstract void Produce(Product product);       
    }

    internal class Litography_Machine : Machine
    {
        public Litography_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
            SupportedProductType = typeof(Microprocessor);
        }

        public override void Produce(Product blueprint)
        {
            if (blueprint.GetType() != SupportedProductType)
            {
                throw new InvalidOperationException($"This machine only produces {SupportedProductType.Name}!");
            }
            // Block production if machine is stopped or broken
            if (Status != MachineStatus.Running)
            {
                AnsiConsole.Write(new Markup($"[red]❌ Cannot produce {blueprint.Name}. Machine is offline. Please boot or repair it first.[/]\n"));
                return;
            }

            AnsiConsole.Write(new Markup($"[cyan]🏭 Starting processing sequence for: [underline]{blueprint.Name}[/][/]\n"));

            // Output progress bar or loading spinner
            AnsiConsole.Status()
                .Spinner(Spinner.Known.BouncingBar)
                .SpinnerStyle(Style.Parse("cyan bold"))
                .Start("Exposing wafer structure using optical masks...", ctx =>
                {
                    Thread.Sleep(800);
                });

            AnsiConsole.Write(new Markup($"[green]✔ Successfully manufactured: {blueprint.Name}[/]\n"));

            // Triggers completely random simulation tracking chance for part failure
            ApplyProductionWearAndTear();
        }
    }

    internal class SMT_Machine : Machine  // Solder Paste Printer
    {
        public SMT_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
            SupportedProductType = typeof(Motherboard);
        }

        public override void Produce(Product product)
        {
            if (product is Motherboard board)
            {
                // 'board' is safely available here and guaranteed to be assigned.
                if (board.CurrentState == BoardState.BlankBoard)
                {
                    board.TransitionTo(BoardState.SolderPrinted);
                    Console.WriteLine($"[green]Successfully processed {board.Name}[/]");
                }
                else
                {
                    Console.WriteLine($"[yellow]Warning: {board.Name} is not in a processable state.[/]");
                }
            }
            else
            {
                // This handles the case where the product passed is NOT a Motherboard
                Console.WriteLine($"[red]Error: This machine cannot process {product.GetType().Name}[/]");
            }
        }
    }

    internal class PaP_Machine : Machine  // Pick and Place
    {
        public PaP_Machine(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            if (product is Motherboard board)
            {
                // 'board' is safely available here and guaranteed to be assigned.
                if (board.CurrentState == BoardState.BlankBoard)
                {
                    board.TransitionTo(BoardState.SolderPrinted);
                    Console.WriteLine($"[green]Successfully processed {board.Name}[/]");
                }
                else
                {
                    Console.WriteLine($"[yellow]Warning: {board.Name} is not in a processable state.[/]");
                }
            }
            else
            {
                // This handles the case where the product passed is NOT a Motherboard
                Console.WriteLine($"[red]Error: This machine cannot process {product.GetType().Name}[/]");
            }
        }
    }

    internal class Reflow_Oven : Machine
    {
        public Reflow_Oven(string machine_name, string machine_manufacturer, string machine_serial, MachinePart[] parts, MachineCondition condition)
            : base(machine_name, machine_manufacturer, machine_serial, parts, condition)
        {
        }
        public override void Produce(Product product)
        {
            if (product is Motherboard board)
            {
                // Now 'board' is safely available here and guaranteed to be assigned.
                if (board.CurrentState == BoardState.BlankBoard)
                {
                    board.TransitionTo(BoardState.SolderPrinted);
                    Console.WriteLine($"[green]Successfully processed {board.Name}[/]");
                }
                else
                {
                    Console.WriteLine($"[yellow]Warning: {board.Name} is not in a processable state.[/]");
                }
            }
            else
            {
                // This handles the case where the product passed is NOT a Motherboard
                Console.WriteLine($"[red]Error: This machine cannot process {product.GetType().Name}[/]");
            }
        }
    }
}
