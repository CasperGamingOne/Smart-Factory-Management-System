using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class ProductionMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<Machine> machineRepo, IJsonRepository<Product> productRepo)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(Align.Left(new Rule($"[yellow]{Production.Title}[/]")));
        AnsiConsole.WriteLine();

        if (factory.Machines.Count == 0)
        {
            AnsiConsole.Write(new Markup(Production.NoMachinerySeeded + "\n"));
            AnsiConsole.Write(new Markup(Production.PressKeyToGoBack));
            Console.ReadKey(true);
            return;
        }

        var pendingOrders = GetPendingOrders(factory);
        if (pendingOrders.Count == 0)
        {
            AnsiConsole.MarkupLine(Reports.NoPendingOrders);
            AnsiConsole.WriteLine(Common.PressKeyToReturn);
            Console.ReadKey(true);
            return;
        }

        var orderSelector = new SelectionPrompt<ProductionOrder>().Title(Production.SelectOrderToProcess)
            .UseConverter(o =>
                $"{o.OrderId} - {o.ProductName} x{o.Quantity} ({o.CompletedCount}/{o.Quantity}) assigned to #{o.AssignedTechnicianId}");
        foreach (var o in pendingOrders) orderSelector.AddChoice(o);

        var order = AnsiConsole.Prompt(orderSelector);

        if (loggedInUser is not Director &&
            (loggedInUser is not Technician || order.AssignedTechnicianId != loggedInUser.Id))
        {
            AnsiConsole.MarkupLine(Production.OnlyAssignedTech);
            AnsiConsole.WriteLine(Common.PressKeyToReturn);
            Console.ReadKey(true);
            return;
        }

        if (order.ProductName == "Motherboard")
        {
            RunMotherboardWorkflow(factory, order, loggerService);
            machineRepo.Save(factory.Machines);
            productRepo.Save(factory.Inventory);
            AnsiConsole.WriteLine();
            AnsiConsole.Write(new Markup(Common.PressKeyToReturnProduction));
            Console.ReadKey(true);
            return;
        }

        Machine? selectedMachine = null;
        foreach (var mach in factory.Machines)
        {
            if (mach.SupportedProductType.Name.Contains(order.ProductName, StringComparison.OrdinalIgnoreCase))
            {
                selectedMachine = mach;
                break;
            }
        }

        if (selectedMachine == null)
        {
            var machineSelector = new SelectionPrompt<Machine>().Title(Production.SelectMachineToUse);
            foreach (var m in factory.Machines)
                machineSelector.AddChoice(m);

            selectedMachine = AnsiConsole.Prompt(machineSelector);
        }

        if (selectedMachine.Status != MachineStatus.Running)
        {
            var bootChoice = AnsiConsole.Confirm(string.Format(Production.MachineNotRunningBoot, selectedMachine.Name));
            if (!bootChoice) return;
            if (!selectedMachine.StartMachine()) return;
        }

        double unitCost = 0;
        foreach (var p in factory.Inventory)
        {
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
            AnsiConsole.MarkupLine(Production.UnknownProductType);
            return;
        }

        var batch = StartProductionBatch(factory, order, unitCost);

        AnsiConsole.Write(new Markup(
            string.Format(Production.StartProduction, order.OrderId, order.ProductName, order.Quantity)));

        while (!order.IsComplete && selectedMachine.Status == MachineStatus.Running)
        {
            var produced = CreateProducedProduct(template);

            selectedMachine.StartOrder(order);
            selectedMachine.Produce(produced);
            loggerService.LogInfo(LogOrigin.USER, LogEvent.ProductionStarted,
                $"${order.ProductName} * {order.Quantity}");

            if (selectedMachine.Status == MachineStatus.Running)
            {
                var completedCount = CommitProducedUnit(factory, order, batch, produced);
                AnsiConsole.MarkupLine(
                    string.Format(Production.ProducedOneUnit, produced.Name, completedCount, order.Quantity));
            }
            else
            {
                AnsiConsole.MarkupLine(Production.MachineTripped);
                break;
            }
        }

        if (order.IsComplete)
        {
            loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.ProductionCompleted,
                $"{batch.ProductName} * {batch.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.BatchCompleted, batch.BatchId, batch.InventoryIndexes.Count));
        }
        else
        {
            loggerService.LogWarning(LogOrigin.SYSTEM, LogEvent.ProductionInterrupted,
                $"{order.ProductName} * {order.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.OrderIncomplete, order.CompletedCount, order.Quantity));
        }

        machineRepo.Save(factory.Machines);
        productRepo.Save(factory.Inventory);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup(Common.PressKeyToReturnProduction));
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

    private static void RunMotherboardWorkflow(Factory factory, ProductionOrder order, ILoggerService loggerService)
    {
        var batch = StartProductionBatch(factory, order, 20);

        var solderPrinter = FindMachine<SmtMachine>(factory);
        var pickAndPlace = FindMachine<PaPMachine>(factory);
        var reflowOven = FindMachine<ReflowOven>(factory);

        if (solderPrinter == null || pickAndPlace == null || reflowOven == null)
        {
            AnsiConsole.MarkupLine(Production.MotherboardWorkflowNoMachines);
            return;
        }

        AnsiConsole.MarkupLine(
            string.Format(Production.StartMotherboardWorkflow, order.OrderId, order.ProductName, order.Quantity));
        loggerService.LogInfo(LogOrigin.USER, LogEvent.ProductionStarted,
            $"{order.ProductName} * {order.Quantity}");

        var motherboardTemplate = new Motherboard("Batch Motherboard", 20, 0, 1, "AM4", "ATX");

        while (!order.IsComplete)
        {
            var board = (Motherboard)CreateProducedProduct(motherboardTemplate);

            if (!RunMotherboardStage(
                    solderPrinter,
                    order,
                    board,
                    Production.SolderPastePrinting,
                    Production.SolderPastePrintingSpinner,
                    Production.SolderPastePrintingFail))
                break;

            if (!RunMotherboardStage(
                    pickAndPlace,
                    order,
                    board,
                    Production.PickAndPlace,
                    Production.PickAndPlaceSpinner,
                    Production.PickAndPlaceFail))
                break;

            if (!RunMotherboardStage(
                    reflowOven,
                    order,
                    board,
                    Production.ReflowBaking,
                    Production.ReflowBakingSpinner,
                    Production.ReflowBakingFail))
                break;

            var completedCount = CommitProducedUnit(factory, order, batch, board);
            AnsiConsole.MarkupLine(string.Format(Production.MotherboardCompleted, completedCount, order.Quantity));
        }

        if (order.IsComplete)
        {
            loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.ProductionCompleted,
                $"{batch.ProductName} * {batch.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.BatchCompleted, batch.BatchId, batch.InventoryIndexes.Count));
        }
        else
        {
            loggerService.LogWarning(LogOrigin.SYSTEM, LogEvent.ProductionInterrupted,
                $"{order.ProductName} * {order.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.OrderIncomplete, order.CompletedCount, order.Quantity));
        }
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

        foreach (var order in factory.PendingOrders)
        {
            if (!order.IsComplete)
            {
                pendingOrders.Add(order);
            }
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
        order.IncrementCompletedCount();
        factory.AddProduct(produced, batch.BatchId);
        return order.CompletedCount;
    }

    private static T? FindMachine<T>(Factory factory) where T : Machine
    {
        foreach (var m in factory.Machines)
            if (m is T typedMachine)
                return typedMachine;

        return null;
    }
}