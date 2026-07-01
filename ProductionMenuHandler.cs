using Spectre.Console;

namespace Smart_Factory_Management_System
{
    internal static class ProductionMenuHandler
    {
        public static void Run(Factory factory, Employee loggedInUser)
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(Align.Left(new Rule("[yellow]🏭 Active Production Control Deck[/]")));
                AnsiConsole.WriteLine();

                // 1. Safeguard: Ensure there are machines registered in the factory database
                if (factory.MachineCount == 0)
                {
                    AnsiConsole.Write(new Markup("[yellow]⚠ No production machinery has been seeded in the factory layout yet.[/]\n"));
                    AnsiConsole.Write(new Markup("[grey]Press any key to go back...[/]"));
                    Console.ReadKey(true);
                    return;
                }

                // 2. Build a beautiful Spectre choice list from the Factory's active tracking array
                var machinePrompt = new SelectionPrompt<string>()
                    .Title("Select a target manufacturing asset to engage:")
                    .PageSize(10)
                    .MoreChoicesText("[grey](Move up and down to browse active hardware catalog)[/]");

                // Safely add instantiated machines based on the factory loop tracking variable
                for (int i = 0; i < factory.MachineCount; i++)
                {
                    var mach = factory.Machines[i];
                    string statusBadge = mach.Status switch
                    {
                        MachineStatus.Running => "[green]RUNNING[/]",
                        MachineStatus.Stopped => "[yellow]STOPPED[/]",
                        MachineStatus.Maintenance => "[orange3]MAINTENANCE[/]",
                        _ => "UNKNOWN"
                    };

                    // Format option string cleanly so it is easily parsable later
                    machinePrompt.AddChoice($"{mach.Name} ({mach.SerialNumber}) - Status: {statusBadge}");
                }
                machinePrompt.AddChoice("[red]« Return to Main Control Menu[/]");

                var selectedOption = AnsiConsole.Prompt(machinePrompt);

                if (selectedOption == "[red]« Return to Main Control Menu[/]")
                {
                    return;
                }

                // 3. Extract and match the selected machine from the factory array
                Machine selectedMachine = null;
                for (int i = 0; i < factory.MachineCount; i++)
                {
                    if (selectedOption.Contains(factory.Machines[i].SerialNumber))
                    {
                        selectedMachine = factory.Machines[i];
                        break;
                    }
                }

                if (selectedMachine == null) continue;

                // 4. State Validation Guard: Boot the machine if it's currently turned off
                if (selectedMachine.Status == MachineStatus.Stopped)
                {
                    AnsiConsole.Write(new Markup($"\n[yellow]⚠ Machine [underline]{selectedMachine.Name}[/] is currently offline (STOPPED).[/]\n"));
                    bool bootChoice = AnsiConsole.Confirm("Would you like to initiate system boot diagnostics and start the asset?");

                    if (bootChoice)
                    {
                        // StartMachine checks component health; returns false if any part is Critical
                        bool bootSuccess = selectedMachine.StartMachine();
                        AnsiConsole.WriteLine();
                        AnsiConsole.Write(new Markup("[grey]Press any key to continue...[/]"));
                        Console.ReadKey(true);

                        if (!bootSuccess) continue; // Loop back to selection deck if boot sequence failed
                    }
                    else
                    {
                        continue;
                    }
                }
                else if (selectedMachine.Status == MachineStatus.Maintenance)
                {
                    AnsiConsole.Write(new Markup($"\n[orange3]❌ ACCESS DENIED:[/] [underline]{selectedMachine.Name}[/] is locked in Engineering Maintenance mode. Operations suspended.\n"));
                    AnsiConsole.Write(new Markup("[grey]Press any key to continue...[/]"));
                    Console.ReadKey(true);
                    continue;
                }

                // 5. Select the Product to Manufacture (Electronics Factory Core Inventory Profile)
                AnsiConsole.WriteLine();
                var productPrompt = new SelectionPrompt<string>()
                    .Title($"[cyan]Choose a production target blueprint for {selectedMachine.Name}:[/]")
                    .AddChoices(new[] {
                        "Microprocessor (Silicon Wafer Layer)",
                        "High-Density Motherboard Assembly",
                        "4K Optical Camera Sensor Module",
                        "[red]« Cancel Job Request[/]"
                    });

                var selectedProductType = AnsiConsole.Prompt(productPrompt);
                if (selectedProductType == "[red]« Cancel Job Request[/]") continue;

                // 6. Instantiate the chosen product asset matching your business requirements
                Product productToProduce = selectedProductType switch
                {
                    "Microprocessor (Silicon Wafer Layer)" => new Microcontroller("NextGen Microprocessor", 375, 500, 10, "CPU-5nm"),
                    "High-Density Motherboard Assembly" => new SensorModule("Flir Lepton Micro-Thermal", 150, 200, 20, "ThermalInfrared"),
                    _ => null
                };

                if (productToProduce == null) continue;

                // 7. Execute the polymorphic workflow pipeline
                AnsiConsole.Clear();
                AnsiConsole.Write(Align.Left(new Rule($"[cyan]Executing Production Job Loop: {productToProduce.Name}[/]")));
                AnsiConsole.WriteLine();

                // Runs internal timers, simulation updates, and executes ApplyProductionWearAndTear()
                selectedMachine.Produce(productToProduce);

                // 8. Log the completed product output directly into factory inventory storage if system didn't trip
                if (selectedMachine.Status == MachineStatus.Running)
                {
                    factory.AddProduct(productToProduce);
                    AnsiConsole.Write(new Markup($"\n[green]✔ Production inventory receipts accepted! Inventory updated (+1 {productToProduce.Name}).[/]\n"));
                }
                else
                {
                    AnsiConsole.Write(new Markup($"\n[red]❌ Production run aborted mid-cycle. Safety sensors isolated line status.[/]\n"));
                }

                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Markup("[grey]Press any key to return to production deck...[/]"));
                Console.ReadKey(true);
            }
        }
    }
}
