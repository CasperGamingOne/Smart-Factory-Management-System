using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class ProductionMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule("[yellow]🏭 Active Production Control Deck[/]")));
        AnsiConsole.WriteLine();

        if (factory.MachineCount == 0)
        {
            AnsiConsole.Write(new Markup(
                "[yellow]⚠ No production machinery has been seeded in the factory layout yet.[/]\n"));
            AnsiConsole.Write(new Markup("[grey]Press any key to go back...[/]"));
            Console.ReadKey(true);
            return;
        }

        var pendingOrders = GetPendingOrders(factory);
        if (pendingOrders.Count == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No production orders are currently pending.[/]");
            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
            return;
        }

        var orderSelector = new SelectionPrompt<ProductionOrder>().Title("Select a pending order to process:")
            .UseConverter(o =>
                $"{o.OrderId} - {o.ProductName} x{o.Quantity} ({o.CompletedCount}/{o.Quantity}) assigned to #{o.AssignedTechnicianId}");
        foreach (var o in pendingOrders) orderSelector.AddChoice(o);

        var order = AnsiConsole.Prompt(orderSelector);

        if (loggedInUser is not Director &&
            (loggedInUser is not Technician || order.AssignedTechnicianId != loggedInUser.Id))
        {
            AnsiConsole.MarkupLine("[red]Only the assigned technician can start this order.[/]");
            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
            return;
        }

        if (order.ProductName == "Motherboard")
        {
            RunMotherboardWorkflow(factory, order);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Markup("[grey]Press any key to return to production deck...[/]"));
            Console.ReadKey(true);
            return;
        }

        Machine? selectedMachine = null;
        for (var i = 0; i < factory.MachineCount; i++)
        {
            var mach = factory.Machines[i];
            if (mach.SupportedProductType.Name.Contains(order.ProductName, StringComparison.OrdinalIgnoreCase))
            {
                selectedMachine = mach;
                break;
            }
        }

        if (selectedMachine == null)
        {
            var machineSelector = new SelectionPrompt<Machine>().Title("Select machine to use:");
            for (var i = 0; i < factory.MachineCount; i++) machineSelector.AddChoice(factory.Machines[i]);
            selectedMachine = AnsiConsole.Prompt(machineSelector);
        }

        if (selectedMachine.Status != MachineStatus.Running)
        {
            var bootChoice = AnsiConsole.Confirm($"Machine {selectedMachine.Name} is not running. Boot it?");
            if (!bootChoice) return;
            if (!selectedMachine.StartMachine()) return;
        }

        double unitCost = 0;
        for (var i = 0; i < factory.ProductCount; i++)
        {
            var p = factory.Inventory[i];
            if (p.Name != null && p.Name.Contains(order.ProductName, StringComparison.OrdinalIgnoreCase))
            {
                unitCost = p.ProductionCost;
                break;
            }
        }

        if (unitCost <= 0)
            unitCost = order.ProductName == "Microprocessor" ? 50 : 20;

        var template = CreateProductTemplate(order.ProductName, unitCost);

        if (template == null)
        {
            AnsiConsole.MarkupLine("[red]Unknown product type for this order.[/]");
            return;
        }

        var batch = StartProductionBatch(factory, order, unitCost);

        AnsiConsole.Write(new Markup(
            $"[cyan]Starting production for order {order.OrderId} - {order.ProductName} x{order.Quantity}[/]"));

        while (!order.IsComplete && selectedMachine.Status == MachineStatus.Running)
        {
            var produced = CreateProducedProduct(template);

            selectedMachine.StartOrder(order);
            selectedMachine.Produce(produced);

            if (selectedMachine.Status == MachineStatus.Running)
            {
                var completedCount = CommitProducedUnit(factory, order, batch, produced);
                AnsiConsole.MarkupLine(
                    $"[green]Produced 1 unit ({produced.Name}). Completed {completedCount}/{order.Quantity}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Machine tripped. Production paused.[/]");
                break;
            }
        }

        AnsiConsole.MarkupLine(
            order.IsComplete
                ? $"[green]Batch complete. Created batch {batch.BatchId} with {batch.InventoryIndexes.Count} items.[/]"
                : $"[yellow]Order incomplete. Produced {order.CompletedCount}/{order.Quantity} so far.[/]");

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup("[grey]Press any key to return to production deck...[/]"));
        Console.ReadKey(true);
    }

    private static Product? CreateProductTemplate(string productName, double unitCost)
    {
        return productName switch
        {
            "Microprocessor" => new Microprocessor("Batch Microprocessor", unitCost, 0, 1, 4, 2.5),
            "Motherboard" => new Motherboard("Batch Motherboard", unitCost, 0, 1, "AM4", "ATX"),
            _ => null
        };
    }

    private static Product CreateProducedProduct(Product template)
    {
        return template switch
        {
            Microprocessor microprocessor => new Microprocessor(microprocessor.Name ?? "Batch Microprocessor",
                microprocessor.ProductionCost, 0, 1, microprocessor.Cores ?? 1, microprocessor.ClockSpeed),
            Motherboard motherboard => new Motherboard(motherboard.Name ?? "Batch Motherboard",
                motherboard.ProductionCost, 0, 1, motherboard.SocketStandard ?? "-",
                motherboard.PhysicalForm ?? "-"),
            _ => template
        };
    }

    private static void RunMotherboardWorkflow(Factory factory, ProductionOrder order)
    {
        var batch = StartProductionBatch(factory, order, 20);

        var solderPrinter = FindMachine<SmtMachine>(factory);
        var pickAndPlace = FindMachine<PaPMachine>(factory);
        var reflowOven = FindMachine<ReflowOven>(factory);

        if (solderPrinter == null || pickAndPlace == null || reflowOven == null)
        {
            AnsiConsole.MarkupLine(
                "[red]Motherboard workflow requires SMT, Pick-and-Place, and Reflow machines to be registered.[/]");
            return;
        }

        AnsiConsole.MarkupLine(
            $"[cyan]Starting motherboard workflow for order {order.OrderId} - {order.ProductName} x{order.Quantity}[/]");

        var motherboardTemplate = new Motherboard("Batch Motherboard", 20, 0, 1, "AM4", "ATX");

        while (!order.IsComplete)
        {
            var board = (Motherboard)CreateProducedProduct(motherboardTemplate);

            if (!RunMotherboardStage(
                    solderPrinter,
                    order,
                    board,
                    "Solder Paste Printing",
                    "Exposing board to solder paste mesh...",
                    "Motherboard workflow stopped during solder paste printing."))
                break;

            if (!RunMotherboardStage(
                    pickAndPlace,
                    order,
                    board,
                    "Pick and Place Assembly",
                    "Aligning and mounting components...",
                    "Motherboard workflow stopped during pick-and-place assembly."))
                break;

            if (!RunMotherboardStage(
                    reflowOven,
                    order,
                    board,
                    "Reflow Baking",
                    "Heating solder joints to fuse the board...",
                    "Motherboard workflow stopped during reflow baking."))
                break;

            var completedCount = CommitProducedUnit(factory, order, batch, board);
            AnsiConsole.MarkupLine($"[green]Motherboard completed {completedCount}/{order.Quantity}.[/]");
        }

        if (order.IsComplete)
            AnsiConsole.MarkupLine(
                $"[green]Batch complete. Created batch {batch.BatchId} with {batch.InventoryIndexes.Count} items.[/]");
    }

    private static bool RunMotherboardStage(
        Machine machine,
        ProductionOrder order,
        Motherboard board,
        string stageName,
        string spinnerText,
        string failureMessage)
    {
        machine.StartOrder(order);
        EnsureMachineReady(machine);

        if (machine.Status != MachineStatus.Running)
        {
            AnsiConsole.MarkupLine($"[red]{failureMessage}[/]");
            return false;
        }

        AnsiConsole.Write(new Rule($"[cyan]{stageName}[/]").Centered());
        AnsiConsole.Status()
            .Spinner(Spinner.Known.BouncingBar)
            .SpinnerStyle(Style.Parse("cyan bold"))
            .Start(spinnerText, _ => { Thread.Sleep(700); });

        if (!machine.Produce(board))
        {
            AnsiConsole.MarkupLine($"[red]{failureMessage}[/]");
            return false;
        }

        return true;
    }

    private static void EnsureMachineReady(Machine machine)
    {
        if (machine.Status != MachineStatus.Running) machine.StartMachine();
    }

    private static List<ProductionOrder> GetPendingOrders(Factory factory)
    {
        var pendingOrders = new List<ProductionOrder>();

        for (var i = 0; i < factory.OrderCount; i++)
        {
            var order = factory.PendingOrders[i];
            if (!order.IsComplete) pendingOrders.Add(order);
        }

        return pendingOrders;
    }

    private static ProductionBatch StartProductionBatch(Factory factory, ProductionOrder order, double unitCost)
    {
        var batch = new ProductionBatch(order.ProductName, order.Quantity, unitCost);
        factory.AddBatch(batch);
        return batch;
    }

    private static int CommitProducedUnit(Factory factory, ProductionOrder order, ProductionBatch batch,
        Product produced)
    {
        order.CompletedCount++;
        factory.AddProduct(produced, batch.BatchId);
        return order.CompletedCount;
    }

    private static T? FindMachine<T>(Factory factory) where T : Machine
    {
        for (var i = 0; i < factory.MachineCount; i++)
            if (factory.Machines[i] is T typedMachine)
                return typedMachine;

        return null;
    }
}