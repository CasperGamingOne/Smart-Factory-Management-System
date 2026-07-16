using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class AccountingMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService)
    {
        if (loggedInUser is not Accountant)
        {
            AnsiConsole.MarkupLine(string.Format(Common.AccessDeniedSection, loggedInUser.Role, "Accounting"));
            AnsiConsole.WriteLine(Common.PressKeyToReturn);
            Console.ReadKey(true);
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[magenta]{Accounting.Title}[/]").Centered());

            var menuOptions = MenuOptions.AccountingMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Common.ChooseAction)
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "View Batches":
                    ShowBatches(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.BatchesViewed, loggedInUser.Username);
                    break;
                case "Set Unit Sell Price for Batch":
                    SetBatchPrice(factory, loggerService, loggedInUser);
                    break;
                case "Process Report Requests":
                    ProcessReportRequests(factory, loggedInUser, loggerService);
                    break;
                case "Return to Main Menu":
                    return;
            }

            AnsiConsole.MarkupLine(Common.PressKeyToContinue);
            Console.ReadKey(true);
        }
    }

    private static void ShowBatches(Factory factory)
    {
        AnsiConsole.WriteLine();
        if (factory.BatchCount == 0)
        {
            AnsiConsole.MarkupLine(Accounting.NoBatches);
            return;
        }

        var table = new Table().AddColumn("BatchId").AddColumn("Product").AddColumn("Qty").AddColumn("UnitCost")
            .AddColumn("UnitSell").AddColumn("Sold");
        for (var i = 0; i < factory.BatchCount; i++)
        {
            var b = factory.Batches[i];
            table.AddRow(b.BatchId, b.ProductName, b.Quantity.ToString(), b.UnitProductionCost.ToString("F2"),
                b.UnitSellPrice?.ToString("F2") ?? "-", b.IsSold ? "Yes" : "No");
        }

        AnsiConsole.Write(table);
    }

    private static void SetBatchPrice(Factory factory, ILoggerService loggerService, Employee loggedInUser)
    {
        if (factory.BatchCount == 0)
        {
            AnsiConsole.MarkupLine(Accounting.NoBatchesToPrice);
            return;
        }

        var selector = new SelectionPrompt<ProductionBatch>().Title(Accounting.SelectBatchToPrice);
        for (var i = 0; i < factory.BatchCount; i++) selector.AddChoice(factory.Batches[i]);

        var chosen = AnsiConsole.Prompt(selector);
        var price = AnsiConsole.Ask<double>(Accounting.SetUnitPricePrompt);
        chosen.SetUnitSellPrice(price);

        // Apply price to linked inventory items
        foreach (var idx in chosen.InventoryIndexes)
            if (idx >= 0 && idx < factory.Inventory.Count)
                factory.Inventory[idx].UpdateSellingPrice(price);

        AnsiConsole.MarkupLine(string.Format(Accounting.BatchPriced, chosen.BatchId, price));
        loggerService.LogInfo(LogOrigin.USER, LogEvent.BatchPriceSet,
            $"Batch {chosen.BatchId} priced at ${price:F2} by {loggedInUser.Username}");
    }

    private static void ProcessReportRequests(Factory factory, Employee accountant, ILoggerService loggerService)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Accounting.ProcessReportRequestsTitle}[/]").Centered());

        var pendingRequests = factory.PendingReportRequests.Take(factory.ReportRequestCount)
            .Where(r => r.Status == ReportStatus.Pending).ToList();

        if (!pendingRequests.Any())
        {
            AnsiConsole.MarkupLine(Accounting.NoPendingRequests);
            return;
        }

        var selector = new SelectionPrompt<ReportRequest>()
            .Title(Accounting.SelectRequestToFulfill)
            .AddChoices(pendingRequests);

        var chosen = AnsiConsole.Prompt(selector);

        chosen.Fulfill();

        AnsiConsole.MarkupLine(
            string.Format(Accounting.RequestFulfilled, chosen.ReportType, chosen.RequestId, accountant.Name));
        loggerService.LogInfo(LogOrigin.USER, LogEvent.ReportRequestsProcessed,
            $"Report '{chosen.ReportType}' (Req ID: {chosen.RequestId}) fulfilled by {accountant.Username}");
    }
}