using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class ProductionMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<Machine> machineRepo, IJsonRepository<Product> productRepo,
        IJsonRepository<ProductionOrder> ordersRepo)
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

        if (factory.Batches.Count(b => !b.IsSold) >= factory.MaxBatches)
        {
            AnsiConsole.MarkupLine(
                $"[red]Error: Warehouse is at maximum batch capacity ({factory.MaxBatches} active batches).[/]");
            AnsiConsole.MarkupLine(
                "[red]Please record sales to clear warehouse space before fulfilling new orders.[/]");
            AnsiConsole.WriteLine(Common.PressKeyToReturn);
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
            RunMotherboardWorkflow(factory, order, loggerService, ordersRepo);
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

        var template = CreateProductTemplate(order, unitCost);

        if (template == null)
        {
            AnsiConsole.MarkupLine(Production.UnknownProductType);
            return;
        }

        var batch = StartProductionBatch(factory, order, unitCost);

        Machine.SilentMode = true;

        var liveGrid = new Markup("");
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule(string.Format(Production.ProductionLineTitle, selectedMachine.Name)).Centered());
        AnsiConsole.WriteLine();

        AnsiConsole.Live(liveGrid)
            .Start(ctx =>
            {
                while (!order.IsComplete && selectedMachine.Status == MachineStatus.Running)
                {
                    var produced = CreateProducedProduct(template);

                    selectedMachine.StartOrder(order);

                    // Simulate longer active machinery delay
                    Thread.Sleep(1200);

                    var success = selectedMachine.Produce(produced);
                    loggerService.LogInfo(LogOrigin.User, LogEvent.ProductionStarted,
                        $"${order.ProductName} * {order.Quantity}");

                    if (success) CommitProducedUnit(factory, order, batch, produced);

                    // Render updated static state
                    var progressPercentage = (double)order.CompletedCount / order.Quantity * 100;
                    var progressBar = GetProgressBar(order.CompletedCount, order.Quantity);

                    var table = new Table().Border(TableBorder.Rounded).Expand();
                    table.AddColumn("Asset");
                    table.AddColumn("Operation Progress");
                    table.AddColumn("Status");

                    var statusString = selectedMachine.Status == MachineStatus.Running
                        ? "[green]RUNNING[/]"
                        : "[red]TRIPPED[/]";

                    table.AddRow(
                        selectedMachine.Name ?? "Machine",
                        $"{progressBar}  {order.CompletedCount} / {order.Quantity} ({progressPercentage:F0}%)",
                        statusString
                    );

                    var outerGrid = new Grid().AddColumn();
                    outerGrid.AddRow(table);

                    if (selectedMachine.Status != MachineStatus.Running)
                        outerGrid.AddRow(
                            new Markup($"\n{string.Format(Production.MachineTrippedAlert, selectedMachine.Name)}"));

                    outerGrid.AddRow(new Align(
                        new Markup(string.Format(Production.CompletedUnitsStatus, order.CompletedCount,
                            order.Quantity)),
                        HorizontalAlignment.Right
                    ));

                    ctx.UpdateTarget(outerGrid);

                    if (selectedMachine.Status != MachineStatus.Running) break;
                }

                // Final draw update
                var finalProgressPercentage = (double)order.CompletedCount / order.Quantity * 100;
                var finalProgressBar = GetProgressBar(order.CompletedCount, order.Quantity);

                var finalTable = new Table().Border(TableBorder.Rounded).Expand();
                finalTable.AddColumn("Asset");
                finalTable.AddColumn("Operation Progress");
                finalTable.AddColumn("Status");

                var finalStatusString = selectedMachine.Status == MachineStatus.Running
                    ? "[green]RUNNING[/]"
                    : "[red]TRIPPED[/]";

                finalTable.AddRow(
                    selectedMachine.Name ?? "Machine",
                    $"{finalProgressBar}  {order.CompletedCount} / {order.Quantity} ({finalProgressPercentage:F0}%)",
                    finalStatusString
                );

                var finalOuterGrid = new Grid().AddColumn();
                finalOuterGrid.AddRow(finalTable);

                if (selectedMachine.Status != MachineStatus.Running)
                    finalOuterGrid.AddRow(
                        new Markup($"\n{string.Format(Production.MachineTrippedAlert, selectedMachine.Name)}"));

                finalOuterGrid.AddRow(new Align(
                    new Markup(string.Format(Production.CompletedUnitsStatus, order.CompletedCount, order.Quantity)),
                    HorizontalAlignment.Right
                ));

                ctx.UpdateTarget(finalOuterGrid);
            });

        Machine.SilentMode = false;

        if (order.CompletedCount > 0)
        {
            batch.Quantity = order.CompletedCount;

            if (order.IsComplete)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine(Production.CostAdvice);
                var actualCost = AnsiConsole.Prompt(
                    new TextPrompt<double>(Production.CostPrompt)
                        .ValidationErrorMessage(Production.CostValidationInvalidNumber)
                        .Validate(c =>
                            c >= 0
                                ? ValidationResult.Success()
                                : ValidationResult.Error(Production.CostValidationNegative)));

                batch.UnitProductionCost = actualCost;

                foreach (var product in factory.Inventory)
                    if (product.BatchId == batch.BatchId)
                        product.ProductionCost = actualCost;
            }
        }

        if (order.IsComplete)
        {
            loggerService.LogInfo(LogOrigin.System, LogEvent.ProductionCompleted,
                $"{batch.ProductName} * {batch.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.BatchCompleted, batch.BatchId, batch.InventoryIndexes.Count));
        }
        else
        {
            loggerService.LogWarning(LogOrigin.System, LogEvent.ProductionInterrupted,
                $"{order.ProductName} * {order.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.OrderIncomplete, order.CompletedCount, order.Quantity));
        }

        machineRepo.Save(factory.Machines);
        productRepo.Save(factory.Inventory);
        ordersRepo.Save(factory.PendingOrders);

        AnsiConsole.WriteLine();
        AnsiConsole.Write(new Markup(Common.PressKeyToReturnProduction));
        Console.ReadKey(true);
    }

    private static Product? CreateProductTemplate(ProductionOrder order, double unitCost)
    {
        return order.ProductName switch
        {
            "Microprocessor" => new Microprocessor(order.Name, unitCost, 0, 1, order.Cores ?? 4,
                order.ClockSpeed ?? 2.5),
            "Motherboard" => new Motherboard(order.Name, unitCost, 0, 1, order.SocketStandard ?? "AM4",
                order.PhysicalForm ?? "ATX"),
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

    private static void RunMotherboardWorkflow(Factory factory, ProductionOrder order, ILoggerService loggerService,
        IJsonRepository<ProductionOrder> ordersRepo)
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

        Machine.SilentMode = true;

        var liveGrid = new Markup("");
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule(Production.MotherboardLineTitle).Centered());
        AnsiConsole.WriteLine();

        var motherboardTemplate = new Motherboard(order.Name, 20, 0, 1, order.SocketStandard ?? "AM4",
            order.PhysicalForm ?? "ATX");

        AnsiConsole.Live(liveGrid)
            .Start(ctx =>
            {
                while (!order.IsComplete)
                {
                    // Refresh view
                    var progressPercentage = (double)order.CompletedCount / order.Quantity * 100;
                    var progressBar = GetProgressBar(order.CompletedCount, order.Quantity);

                    var table = new Table().Border(TableBorder.Rounded).Expand();
                    table.AddColumn("Production Step (Machine)");
                    table.AddColumn("Status");

                    table.AddRow(Production.StepSmt,
                        solderPrinter.Status == MachineStatus.Running
                            ? "[green]RUNNING[/]"
                            : solderPrinter.Condition == MachineCondition.Critical
                                ? "[red]TRIPPED[/]"
                                : "[yellow]IDLE[/]");
                    table.AddRow(Production.StepPap,
                        pickAndPlace.Status == MachineStatus.Running
                            ? "[green]RUNNING[/]"
                            : pickAndPlace.Condition == MachineCondition.Critical
                                ? "[red]TRIPPED[/]"
                                : "[yellow]IDLE[/]");
                    table.AddRow(Production.StepOven,
                        reflowOven.Status == MachineStatus.Running
                            ? "[green]RUNNING[/]"
                            : reflowOven.Condition == MachineCondition.Critical
                                ? "[red]TRIPPED[/]"
                                : "[yellow]IDLE[/]");

                    var outerGrid = new Grid().AddColumn();
                    outerGrid.AddRow(new Markup(
                        $"[bold cyan]Progress:[/] {progressBar} {order.CompletedCount}/{order.Quantity} ({progressPercentage:F0}%)\n"));
                    outerGrid.AddRow(table);

                    if (solderPrinter.Condition == MachineCondition.Critical ||
                        pickAndPlace.Condition == MachineCondition.Critical ||
                        reflowOven.Condition == MachineCondition.Critical)
                        outerGrid.AddRow(new Markup($"\n{Production.WorkflowTrippedAlert}"));

                    outerGrid.AddRow(new Align(
                        new Markup(string.Format(Production.CompletedUnitsStatus, order.CompletedCount,
                            order.Quantity)),
                        HorizontalAlignment.Right
                    ));

                    ctx.UpdateTarget(outerGrid);

                    if (solderPrinter.Condition == MachineCondition.Critical ||
                        pickAndPlace.Condition == MachineCondition.Critical ||
                        reflowOven.Condition == MachineCondition.Critical)
                        break;

                    var board = factory.Inventory.OfType<Motherboard>()
                        .FirstOrDefault(b =>
                            b.Name == order.Name && b.CurrentState != BoardState.BakedAndSoldered &&
                            !b.IsSold);

                    var isNewBoard = false;
                    if (board == null)
                    {
                        board = (Motherboard)CreateProducedProduct(motherboardTemplate);
                        isNewBoard = true;
                    }

                    board.BatchId = batch.BatchId;
                    if (isNewBoard) factory.AddProduct(board, batch.BatchId);

                    // Run Solder Printer stage
                    if (board.CurrentState == BoardState.BlankBoard)
                        if (!RunMotherboardStage(solderPrinter, order, board, Production.SolderPastePrinting, "", ""))
                            break;

                    // Run Pick & Place stage
                    if (board.CurrentState == BoardState.SolderPrinted)
                        if (!RunMotherboardStage(pickAndPlace, order, board, Production.PickAndPlace, "", ""))
                            break;

                    // Run Reflow Oven stage
                    if (board.CurrentState == BoardState.ComponentsPlaced)
                        if (!RunMotherboardStage(reflowOven, order, board, Production.ReflowBaking, "", ""))
                            break;

                    if (board.CurrentState == BoardState.BakedAndSoldered)
                    {
                        order.IncrementCompletedCount();

                        var completedBoard = factory.Inventory.OfType<Motherboard>()
                            .FirstOrDefault(b =>
                                b.BatchId == batch.BatchId && b != board && b is
                                    { CurrentState: BoardState.BakedAndSoldered, IsSold: false });

                        if (completedBoard != null)
                        {
                            completedBoard.AddQuantity(board.Quantity);
                            factory.RemoveProduct(board);
                        }
                    }
                }

                // Final draw update
                var finalProgressPercentage = (double)order.CompletedCount / order.Quantity * 100;
                var finalProgressBar = GetProgressBar(order.CompletedCount, order.Quantity);

                var finalTable = new Table().Border(TableBorder.Rounded).Expand();
                finalTable.AddColumn("Production Step (Machine)");
                finalTable.AddColumn("Status");

                finalTable.AddRow(Production.StepSmt,
                    solderPrinter.Status == MachineStatus.Running
                        ? "[green]RUNNING[/]"
                        : solderPrinter.Condition == MachineCondition.Critical
                            ? "[red]TRIPPED[/]"
                            : "[yellow]IDLE[/]");
                finalTable.AddRow(Production.StepPap,
                    pickAndPlace.Status == MachineStatus.Running
                        ? "[green]RUNNING[/]"
                        : pickAndPlace.Condition == MachineCondition.Critical
                            ? "[red]TRIPPED[/]"
                            : "[yellow]IDLE[/]");
                finalTable.AddRow(Production.StepOven,
                    reflowOven.Status == MachineStatus.Running
                        ? "[green]RUNNING[/]"
                        : reflowOven.Condition == MachineCondition.Critical
                            ? "[red]TRIPPED[/]"
                            : "[yellow]IDLE[/]");

                var finalOuterGrid = new Grid().AddColumn();
                finalOuterGrid.AddRow(new Markup(
                    $"[bold cyan]Progress:[/] {finalProgressBar} {order.CompletedCount}/{order.Quantity} ({finalProgressPercentage:F0}%)\n"));
                finalOuterGrid.AddRow(finalTable);

                if (solderPrinter.Condition == MachineCondition.Critical ||
                    pickAndPlace.Condition == MachineCondition.Critical ||
                    reflowOven.Condition == MachineCondition.Critical)
                    finalOuterGrid.AddRow(new Markup($"\n{Production.WorkflowTrippedAlert}"));

                finalOuterGrid.AddRow(new Align(
                    new Markup(string.Format(Production.CompletedUnitsStatus, order.CompletedCount, order.Quantity)),
                    HorizontalAlignment.Right
                ));

                ctx.UpdateTarget(finalOuterGrid);
            });

        Machine.SilentMode = false;

        if (order.CompletedCount > 0)
        {
            batch.Quantity = order.CompletedCount;

            if (order.IsComplete)
            {
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine(Production.CostAdvice);
                var actualCost = AnsiConsole.Prompt(
                    new TextPrompt<double>(Production.CostPrompt)
                        .ValidationErrorMessage(Production.CostValidationInvalidNumber)
                        .Validate(c =>
                            c >= 0
                                ? ValidationResult.Success()
                                : ValidationResult.Error(Production.CostValidationNegative)));

                batch.UnitProductionCost = actualCost;

                foreach (var product in factory.Inventory)
                    if (product.BatchId == batch.BatchId)
                        product.ProductionCost = actualCost;
            }
        }

        ordersRepo.Save(factory.PendingOrders);

        if (order.IsComplete)
        {
            loggerService.LogInfo(LogOrigin.System, LogEvent.ProductionCompleted,
                $"{batch.ProductName} * {batch.Quantity}");
            AnsiConsole.MarkupLine(
                string.Format(Production.BatchCompleted, batch.BatchId, batch.InventoryIndexes.Count));
        }
        else
        {
            loggerService.LogWarning(LogOrigin.System, LogEvent.ProductionInterrupted,
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
            if (!Machine.SilentMode) AnsiConsole.MarkupLine($"[red]{failureMessage}[/]");
            return false;
        }

        if (!Machine.SilentMode)
        {
            AnsiConsole.Write(new Rule($"[cyan]{stageName}[/]").Centered());
            AnsiConsole.Status()
                .Spinner(Spinner.Known.BouncingBar)
                .SpinnerStyle(Style.Parse("cyan bold"))
                .Start(spinnerText, _ => { Thread.Sleep(700); });
        }
        else
        {
            // Simulate the active machine delay!
            Thread.Sleep(500);
        }

        if (!machine.Produce(board))
        {
            if (!Machine.SilentMode) AnsiConsole.MarkupLine($"[red]{failureMessage}[/]");
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
        if (!string.IsNullOrEmpty(order.BatchId))
        {
            var existing = factory.Batches.FirstOrDefault(b => b.BatchId == order.BatchId);
            if (existing != null)
            {
                existing.Quantity = order.Quantity;
                return existing;
            }
        }

        var batch = new ProductionBatch(order.ProductName, order.Quantity, unitCost);
        order.BatchId = batch.BatchId;
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

    private static string GetProgressBar(int completed, int total)
    {
        var width = 20;
        var completedWidth = total > 0 ? (int)Math.Round((double)completed / total * width) : 0;
        if (completedWidth > width) completedWidth = width;
        var filled = new string('█', completedWidth);
        var empty = new string('░', width - completedWidth);
        return $"[green]{filled}[/][grey]{empty}[/]";
    }
}