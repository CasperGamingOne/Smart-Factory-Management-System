using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class SalesMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<Product> productRepo, IJsonRepository<ProductionOrder> ordersRepo)
    {
        if (loggedInUser is not SalesAgent && loggedInUser is not Director)
        {
            AnsiConsole.MarkupLine(string.Format(Common.AccessDeniedSection, loggedInUser.Role, "Sales"));
            AnsiConsole.WriteLine(Common.PressKeyToReturn);
            Console.ReadKey(true);
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[green]{Sales.Title}[/]").Centered());

            var menuOptions = MenuOptions.SalesMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Common.ChooseAction)
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "Place Production Order":
                    PlaceOrder(factory, loggerService, loggedInUser, ordersRepo);
                    break;
                case "View Pending Orders":
                    ShowPendingOrders(factory);
                    loggerService.LogInfo(LogOrigin.User, LogEvent.PendingOrdersViewed, loggedInUser.Username);
                    break;
                case "Record Sale for Batch":
                    RecordSale(factory, loggerService, loggedInUser, productRepo);
                    productRepo.Save(factory.Inventory);
                    break;
                case "Return to Main Menu":
                    return;
            }

            AnsiConsole.MarkupLine(Common.PressKeyToContinue);
            Console.ReadKey(true);
        }
    }

    private static void PlaceOrder(Factory factory, ILoggerService loggerService, Employee loggedInUser,
        IJsonRepository<ProductionOrder> ordersRepo)
    {
        AnsiConsole.WriteLine();
        var productChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Sales.SelectProduct)
                .AddChoices(MenuOptions.ProductTypes));

        var customName = AnsiConsole.Ask<string>(Sales.EnterCustomProductName);
        var quantity = AnsiConsole.Prompt(
            new TextPrompt<int>(Sales.EnterQuantity)
                .ValidationErrorMessage("[red]Quantity must be between 1 and 100 units per batch.[/]")
                .Validate(q => q switch
                {
                    <= 0 => ValidationResult.Error("[red]Quantity must be positive.[/]"),
                    > 100 => ValidationResult.Error("[red]Maximum quantity per batch is 100 units.[/]"),
                    _ => ValidationResult.Success()
                }));

        int? cores = null;
        double? clockSpeed = null;
        string? socketStandard = null;
        string? physicalForm = null;

        if (productChoice == "Microprocessor")
        {
            cores = AnsiConsole.Ask<int>(Sales.EnterCpuCores);
            clockSpeed = AnsiConsole.Ask<double>(Sales.EnterClockSpeed);
        }
        else if (productChoice == "Motherboard")
        {
            socketStandard = AnsiConsole.Ask<string>(Sales.EnterSocketStandard);
            physicalForm = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Sales.SelectFormFactor)
                    .AddChoices(MenuOptions.FormFactors));
        }

        var order = new ProductionOrder(productChoice, quantity, -1)
        {
            CustomProductName = customName,
            Cores = cores,
            ClockSpeed = clockSpeed.GetValueOrDefault(),
            SocketStandard = socketStandard,
            PhysicalForm = physicalForm,
            PlacedBy = loggedInUser.Username
        };
        factory.AddOrder(order);
        ordersRepo.Save(factory.PendingOrders);

        AnsiConsole.MarkupLine(
            string.Format(Sales.OrderPlacedOnHold, order.OrderId, order.CustomProductName, order.Quantity));
        loggerService.LogInfo(LogOrigin.User, LogEvent.OrderPlaced,
            $"Order {order.OrderId}: {order.CustomProductName} x{order.Quantity} (Status: On Hold / Unassigned) by {loggedInUser.Username}");
    }

    public static void ShowPendingOrders(Factory factory)
    {
        AnsiConsole.WriteLine();
        if (factory.OrderCount == 0)
        {
            AnsiConsole.MarkupLine(Sales.NoPendingOrders);
            return;
        }

        var table = new Table().AddColumn("Order").AddColumn("Product").AddColumn("Qty").AddColumn("Done")
            .AddColumn("TechId");
        foreach (var o in factory.PendingOrders)
        {
            table.AddRow(
                o.OrderId,
                o.ProductName,
                o.Quantity.ToString(),
                o.CompletedCount.ToString(),
                o.AssignedTechnicianId.ToString()
            );
        }

        AnsiConsole.Write(table);
    }

    private static void RecordSale(Factory factory, ILoggerService loggerService, Employee loggedInUser,
        IJsonRepository<Product> productRepo)
    {
        // Offer selling either from completed batches or from existing inventory
        var options = new List<string>();
        if (factory.BatchCount > 0) options.Add("Sell from Batch");
        if (factory.Inventory.Any(p => !p.IsSold)) options.Add("Sell from Inventory");
        if (options.Count == 0)
        {
            AnsiConsole.MarkupLine(Sales.RecordSaleNoStock);
            return;
        }

        var pickContext =
            AnsiConsole.Prompt(new SelectionPrompt<string>().Title(Sales.ChooseSaleSource).AddChoices(options));

        if (pickContext == "Sell from Batch")
        {
            var unsoldBatches = factory.Batches.Where(b => !b.IsSold).ToList();
            if (!unsoldBatches.Any())
            {
                AnsiConsole.MarkupLine(Sales.NoUnsoldBatchesToSell);
                return;
            }

            var selector = new SelectionPrompt<ProductionBatch>()
                .Title(Sales.SelectBatchToSell)
                .UseConverter(b =>
                    $"{b.BatchId} - {b.ProductName} (Qty: {b.Quantity}, Cost: ${b.UnitProductionCost:F2})")
                .AddChoices(unsoldBatches);

            var chosen = AnsiConsole.Prompt(selector);

            var soldPrice = AnsiConsole.Ask<double>(Sales.EnterUnitSoldPrice);
            chosen.MarkAsSold();
            chosen.SetUnitSellPrice(soldPrice);

            // Apply price to linked inventory items and mark them as sold
            var inventoryIndexes = new List<int>();
            foreach (var idx in chosen.InventoryIndexes)
                if (idx >= 0 && idx < factory.Inventory.Count)
                {
                    factory.Inventory[idx].UpdateSellingPrice(soldPrice);
                    factory.Inventory[idx].MarkAsSold();
                    inventoryIndexes.Add(idx);
                }

            AnsiConsole.MarkupLine(
                string.Format(Sales.SaleRecordedBatch, chosen.BatchId, soldPrice));
            loggerService.LogInfo(LogOrigin.User, LogEvent.SaleRecorded,
                $"Batch {chosen.BatchId} sold at ${soldPrice:F2}/unit by {loggedInUser.Username}");

            UndoService.Instance.RegisterCommand(new SellFromBatchCommand(chosen, factory, soldPrice, inventoryIndexes,
                productRepo, loggedInUser.Username));
        }
        else if (pickContext == "Sell from Inventory")
        {
            var products = new List<Product>();
            foreach (var p in factory.Inventory)
            {
                if (p is { Quantity: > 0, IsSold: false })
                    products.Add(p);
            }

            if (products.Count == 0)
            {
                AnsiConsole.MarkupLine(Sales.NoStockedItems);
                return;
            }

            var prodSelector = new SelectionPrompt<Product>().Title(Sales.SelectProductToSell)
                .UseConverter(p => $"{p.Name} (stock: {p.Quantity})");
            foreach (var pr in products) prodSelector.AddChoice(pr);

            var chosen = AnsiConsole.Prompt(prodSelector);
            var qty = AnsiConsole.Ask<int>(string.Format(Sales.SellQuantity, chosen.Quantity));
            if (qty <= 0)
            {
                AnsiConsole.MarkupLine(Sales.QtyMustBePositive);
                return;
            }

            if (qty > chosen.Quantity)
            {
                AnsiConsole.MarkupLine(
                    string.Format(Sales.InsufficientInventory, qty, chosen.Quantity));
                return;
            }

            var soldPrice = AnsiConsole.Ask<double>(Sales.EnterUnitSoldPrice);

            // Decrease stock from available pool
            chosen.DeductQuantity(qty);

            // Create a duplicate copy marked as sold for history
            Product soldProduct;
            if (chosen is Microprocessor cpu)
                soldProduct = new Microprocessor(cpu.Name ?? "Microprocessor", cpu.ProductionCost, soldPrice, qty,
                    cpu.Cores, cpu.ClockSpeed)
                {
                    BatchId = cpu.BatchId,
                    IsSold = true
                };
            else if (chosen is Motherboard board)
                soldProduct = new Motherboard(board.Name ?? "Motherboard", board.ProductionCost, soldPrice, qty,
                    board.SocketStandard ?? "-", board.PhysicalForm ?? "-")
                {
                    BatchId = board.BatchId,
                    IsSold = true
                };
            else
                soldProduct = chosen;

            factory.AddProduct(soldProduct, soldProduct.BatchId);

            // Find and deduct quantity from the original production batch
            var originalBatch = factory.Batches.FirstOrDefault(b => b.BatchId == chosen.BatchId);
            if (originalBatch != null)
            {
                originalBatch.Quantity -= qty;
                if (originalBatch.Quantity <= 0) originalBatch.MarkAsSold();
            }

            var batch = new ProductionBatch(chosen.Name ?? "Inventory Sale", qty, chosen.ProductionCost);
            batch.MarkAsSold();
            batch.SetUnitSellPrice(soldPrice);
            factory.AddBatch(batch);

            AnsiConsole.MarkupLine(
                string.Format(Sales.SaleRecordedInventory, qty, chosen.Name, soldPrice, batch.BatchId));
            loggerService.LogInfo(LogOrigin.User, LogEvent.SaleRecorded,
                $"{qty} units of {chosen.Name} sold at ${soldPrice:F2}/unit by {loggedInUser.Username}");

            UndoService.Instance.RegisterCommand(new SellFromInventoryCommand(chosen, qty, soldPrice, soldProduct,
                batch, factory, productRepo, loggedInUser.Username));
        }
    }
}