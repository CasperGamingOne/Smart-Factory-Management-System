using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class SalesMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<Product> productRepo)
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
                    PlaceOrder(factory, loggerService, loggedInUser);
                    break;
                case "View Pending Orders":
                    ShowPendingOrders(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.PendingOrdersViewed, loggedInUser.Username);
                    break;
                case "Record Sale for Batch":
                    RecordSale(factory, loggerService, loggedInUser);
                    productRepo.Save(factory.Inventory);
                    break;
                case "Return to Main Menu":
                    return;
            }

            AnsiConsole.MarkupLine(Common.PressKeyToContinue);
            Console.ReadKey(true);
        }
    }

    private static void PlaceOrder(Factory factory, ILoggerService loggerService, Employee loggedInUser)
    {
        AnsiConsole.WriteLine();
        var productChoice = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Sales.SelectProduct)
                .AddChoices(MenuOptions.ProductTypes));

        var quantity = AnsiConsole.Ask<int>(Sales.EnterQuantity);

        // Choose technician
        var techSelector = new SelectionPrompt<string>()
            .Title(Sales.AssignToTechnician)
            .AddChoices(MenuOptions.TechAssignChoices);

        var techChoice = AnsiConsole.Prompt(techSelector);
        var technicianId = -1;
        var technicians = GetTechnicians(factory);

        if (techChoice == "Auto-assign (first available)")
        {
            if (technicians.Count > 0)
                technicianId = technicians[0].Id;
        }
        else
        {
            if (technicians.Count == 0)
            {
                AnsiConsole.MarkupLine(Sales.NoTechnicians);
                return;
            }

            var techList = new SelectionPrompt<Employee>().Title(Sales.SelectTechnician);
            foreach (var technician in technicians) techList.AddChoice(technician);
            var chosen = AnsiConsole.Prompt(techList);
            technicianId = chosen.Id;
        }

        var order = new ProductionOrder(productChoice, quantity, technicianId);
        factory.AddOrder(order);

        AnsiConsole.MarkupLine(
            string.Format(Sales.OrderPlaced, order.OrderId, order.ProductName, order.Quantity,
                order.AssignedTechnicianId));
        loggerService.LogInfo(LogOrigin.USER, LogEvent.OrderPlaced,
            $"Order {order.OrderId}: {order.ProductName} x{order.Quantity} (Assigned Tech: {order.AssignedTechnicianId}) by {loggedInUser.Username}");
    }

    private static List<Employee> GetTechnicians(Factory factory)
    {
        var technicians = new List<Employee>();

        foreach (var t in factory.Employees)
            if (t is Technician)
                technicians.Add(t);

        return technicians;
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

    private static void RecordSale(Factory factory, ILoggerService loggerService, Employee loggedInUser)
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
            var selector = new SelectionPrompt<ProductionBatch>().Title(Sales.SelectBatchToSell);
            for (var i = 0; i < factory.BatchCount; i++) selector.AddChoice(factory.Batches[i]);

            var chosen = AnsiConsole.Prompt(selector);
            if (chosen.IsSold)
            {
                AnsiConsole.MarkupLine(Sales.BatchAlreadySold);
                return;
            }

            var soldPrice = AnsiConsole.Ask<double>(Sales.EnterUnitSoldPrice);
            chosen.MarkAsSold();
            chosen.SetUnitSellPrice(soldPrice);

            // Apply price to linked inventory items and mark them as sold
            foreach (var idx in chosen.InventoryIndexes)
                if (idx >= 0 && idx < factory.Inventory.Count)
                {
                    factory.Inventory[idx].UpdateSellingPrice(soldPrice);
                    factory.Inventory[idx].MarkAsSold();
                }

            AnsiConsole.MarkupLine(
                string.Format(Sales.SaleRecordedBatch, chosen.BatchId, soldPrice));
            loggerService.LogInfo(LogOrigin.USER, LogEvent.SaleRecorded,
                $"Batch {chosen.BatchId} sold at ${soldPrice:F2}/unit by {loggedInUser.Username}");
        }
        else if (pickContext == "Sell from Inventory")
        {
            var products = new List<Product>();
            foreach (var p in factory.Inventory)
            {
                if (p.Quantity > 0 && !p.IsSold)
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

            var batch = new ProductionBatch(chosen.Name ?? "Inventory Sale", qty, chosen.ProductionCost);
            batch.MarkAsSold();
            batch.SetUnitSellPrice(soldPrice);
            factory.AddBatch(batch);

            AnsiConsole.MarkupLine(
                string.Format(Sales.SaleRecordedInventory, qty, chosen.Name, soldPrice, batch.BatchId));
            loggerService.LogInfo(LogOrigin.USER, LogEvent.SaleRecorded,
                $"{qty} units of {chosen.Name} sold at ${soldPrice:F2}/unit by {loggedInUser.Username}");
        }
    }
}