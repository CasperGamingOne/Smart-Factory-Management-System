using Spectre.Console;
using System;

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

                // 2. Offer two main actions: choose pending order or return
                var action = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .Title("Choose production action:")
                    .AddChoices(MenuOptions.ProductionActions));

                if (action != null && (action.StartsWith("2") || action.Contains("Return", StringComparison.OrdinalIgnoreCase)))
                    return;

                // Show pending orders
                if (factory.OrderCount == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No pending production orders to fulfill.[/]");
                    AnsiConsole.WriteLine("\nPress any key to return...");
                    Console.ReadKey(true);
                    return;
                }

                var orderSelector = new SelectionPrompt<ProductionOrder>().Title("Select a pending order to process:")
                    .UseConverter(o => $"{o.OrderId} - {o.ProductName} x{o.Quantity} ({o.CompletedCount}/{o.Quantity}) assigned to #{o.AssignedTechnicianId}");
                for (int i = 0; i < factory.OrderCount; i++)
                {
                    var o = factory.PendingOrders[i];
                    if (o != null && !o.IsComplete)
                        orderSelector.AddChoice(o);
                }

                var order = AnsiConsole.Prompt(orderSelector);

                // Ensure the logged-in technician is assigned
                if (loggedInUser is Technician == false && order.AssignedTechnicianId != loggedInUser.Id)
                {
                    AnsiConsole.MarkupLine("[red]Only the assigned technician can start this order.[/]");
                    AnsiConsole.WriteLine("\nPress any key to return...");
                    Console.ReadKey(true);
                    return;
                }

                // Find a machine that supports the product type
                Machine? selectedMachine = null;
                for (int i = 0; i < factory.MachineCount; i++)
                {
                    var mach = factory.Machines[i];
                    if (mach != null && mach.SupportedProductType.Name.Contains(order.ProductName, StringComparison.OrdinalIgnoreCase))
                    {
                        selectedMachine = mach;
                        break;
                    }
                }

                // If not found, allow user to pick any machine
                if (selectedMachine == null)
                {
                    var machineSelector = new SelectionPrompt<Machine>().Title("Select machine to use:");
                    for (int i = 0; i < factory.MachineCount; i++) machineSelector.AddChoice(factory.Machines[i]);
                    selectedMachine = AnsiConsole.Prompt(machineSelector);
                }

                // Boot machine if needed
                if (selectedMachine.Status != MachineStatus.Running)
                {
                    bool bootChoice = AnsiConsole.Confirm($"Machine {selectedMachine.Name} is not running. Boot it?");
                    if (!bootChoice) continue;
                    if (!selectedMachine.StartMachine()) continue;
                }

                // Determine a realistic unit production cost by checking seeded inventory
                double unitCost = 0;
                for (int i = 0; i < factory.ProductCount; i++)
                {
                    var p = factory.Inventory[i];
                    if (p != null && p.Name != null && p.Name.Contains(order.ProductName, StringComparison.OrdinalIgnoreCase))
                    {
                        unitCost = p.ProductionCost;
                        break;
                    }
                }

                // Fallback to small defaults if not found
                if (unitCost <= 0)
                    unitCost = order.ProductName == "Microprocessor" ? 50 : 20;

                // Prepare a product template for instantiation per unit
                Product? template = order.ProductName switch
                {
                    "Microprocessor" => new Microprocessor("Batch Microprocessor", unitCost, 0, 1, 4, 2.5),
                    "Motherboard" => new Motherboard("Batch Motherboard", unitCost, 0, 1, "AM4", "ATX"),
                    _ => null
                };

                if (template == null)
                {
                    AnsiConsole.MarkupLine("[red]Unknown product type for this order.[/]");
                    continue;
                }

                // Non-null alias for template to satisfy nullability analysis
                var temp = template!;

                // Create a production batch record
                var batch = new ProductionBatch(order.ProductName, order.Quantity, unitCost);
                factory.AddBatch(batch);

                AnsiConsole.Write(new Markup($"[cyan]Starting production for order {order.OrderId} - {order.ProductName} x{order.Quantity}[/]"));

                // Iterate until order complete or machine trips
                while (!order.IsComplete && selectedMachine.Status == MachineStatus.Running)
                {
                    // Instantiate a fresh product for each unit
                    Product produced = order.ProductName switch
                    {
                        "Microprocessor" => new Microprocessor(temp.Name ?? "Batch Microprocessor", temp.ProductionCost, 0, 1, temp.Cores ?? 1, temp.ClockSpeed),
                        "Motherboard" => new Motherboard(temp.Name ?? "Batch Motherboard", temp.ProductionCost, 0, 1, ((Motherboard)temp).SocketStandard ?? "-", ((Motherboard)temp).PhysicalForm ?? "-"),
                        _ => template
                    };

                    // Request the machine to produce a unit
                    selectedMachine.StartOrder(order);
                    selectedMachine.Produce(produced);

                    // Only add to inventory if machine still running after produce
                    if (selectedMachine.Status == MachineStatus.Running)
                    {
                        // Increment order counter centrally
                        order.CompletedCount++;
                        factory.AddProduct(produced, batch.BatchId);
                        AnsiConsole.MarkupLine($"[green]Produced 1 unit ({produced.Name}). Completed {order.CompletedCount}/{order.Quantity}[/]");
                    }
                    else
                    {
                        AnsiConsole.MarkupLine("[red]Machine tripped. Production paused.[/]");
                        break;
                    }
                }

                if (order.IsComplete)
                {
                    AnsiConsole.MarkupLine($"[green]Batch complete. Created batch {batch.BatchId} with {batch.InventoryIndexes.Count} items.[/]");
                }
                else
                {
                    AnsiConsole.MarkupLine($"[yellow]Order incomplete. Produced {order.CompletedCount}/{order.Quantity} so far.[/]");
                }

                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Markup("[grey]Press any key to return to production deck...[/]"));
                Console.ReadKey(true);
            }
        }
    }
}
