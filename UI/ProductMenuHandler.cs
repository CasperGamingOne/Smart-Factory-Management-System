using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class ProductMenuHandler
{
    public static void Run(Factory factory, Employee currentUser, ILoggerService loggerService,
        IJsonRepository<Product> productRepo)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[yellow]{Products.Title}[/]").Centered());
            AnsiConsole.WriteLine();

            var summaryGrid = new Grid();
            summaryGrid.AddColumn();
            summaryGrid.AddRow(new Markup(
                string.Format(Products.OperatorSession, currentUser.Name, currentUser.Role)));
            summaryGrid.AddRow(new Markup(
                string.Format(Products.WarehouseStock, factory.Inventory.Count, factory.InventoryCapacity)));

            AnsiConsole.Write(
                new Panel(summaryGrid)
                    .Header(Products.PanelHeader)
                    .Border(BoxBorder.Rounded)
                    .BorderStyle(new Style(Color.Blue))
            );
            AnsiConsole.WriteLine();

            var menuOptions = MenuOptions.ProductMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Products.NavigatePrompt)
                    .PageSize(10)
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "View Finished Goods Stock":
                    DisplayInventoryTable(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.InventoryViewed, currentUser.Username);
                    break;
                case "View Inventory Financial & Capacity Analytics":
                    DisplayInventoryAnalytics(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.InventoryAnalyticsViewed, currentUser.Username);
                    break;
                case "Sales & Orders":
                    // Reuse Sales menu view; if user is SalesAgent, open full Sales UI
                    if (currentUser is SalesAgent || currentUser is Director)
                        SalesMenuHandler.Run(factory, currentUser, loggerService, productRepo);
                    else
                        SalesMenuHandler.ShowPendingOrders(factory);
                    break;
                case "Return to Main Menu":
                    return;
            }
        }
    }

    private static void DisplayInventoryTable(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[green]{Products.ViewFinishedStockTitle}[/]").Centered());
        AnsiConsole.WriteLine();

        var unsoldProducts = factory.Inventory.Where(p => !p.IsSold).ToList();

        if (unsoldProducts.Count == 0)
        {
            AnsiConsole.Write(
                new Panel(Products.WarehouseEmpty)
                    .Border(BoxBorder.Square));
            AnsiConsole.WriteLine(Common.PressKeyToReturn);
            Console.ReadKey(true);
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(Products.SlotColumn);
        table.AddColumn(Products.ComponentNameColumn);
        table.AddColumn(Products.QuantityColumn);
        table.AddColumn(Products.ProductionCostColumn);
        table.AddColumn(Products.SpecsColumn);
        table.AddColumn(Products.ValueColumn);

        // Populate table rows with inventory data
        for (var i = 0; i < unsoldProducts.Count; i++)
        {
            var product = unsoldProducts[i];
            {
                var dynamicSpecs = product.GetTechnicalSpecifications();

                table.AddRow(
                    (i + 1).ToString(),
                    product.Name ?? "-",
                    product.Quantity.ToString(),
                    product.ProductionCost.ToString("F2"),
                    dynamicSpecs,
                    $"${product.SellingPrice:F2}"
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.WriteLine(Common.PressKeyToReturn);
        Console.ReadKey(true);
    }

    private static void DisplayInventoryAnalytics(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Products.PortfolioTitle}[/]").Centered());
        AnsiConsole.WriteLine();

        var unsoldProducts = factory.Inventory.Where(p => !p.IsSold).ToList();

        double cumulativeValue = 0;
        var cpuCount = 0;
        var pcbCount = 0;

        foreach (var product in unsoldProducts)
        {
            cumulativeValue += product.SellingPrice;

            // Type checking subclasses safely for metrics grouping
            if (product is Microprocessor) cpuCount++;
            else if (product is Motherboard) pcbCount++;
        }

        var storageUtilization = unsoldProducts.Count == 0
            ? 0.0
            : (double)unsoldProducts.Count / factory.InventoryCapacity * 100;

        var statsGrid = new Grid().AddColumns(2);
        statsGrid.AddRow(Products.StatsGridTotalVolume,
            string.Format(Products.StatsGridTotalVolumeValue, unsoldProducts.Count, cpuCount, pcbCount));
        statsGrid.AddRow(Products.StatsGridValuation, string.Format(Products.StatsGridValuationValue, cumulativeValue));
        statsGrid.AddRow(Products.StatsGridOccupancy,
            string.Format(Products.StatsGridOccupancyValue, storageUtilization));

        AnsiConsole.Write(new Panel(statsGrid).Header(Products.AnalyticsPanelHeader)
            .Border(BoxBorder.Double));
        AnsiConsole.WriteLine(Common.PressKeyToReturn);
        Console.ReadKey(true);
    }
}