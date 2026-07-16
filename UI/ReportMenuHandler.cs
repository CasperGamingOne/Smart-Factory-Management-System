using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class ReportMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[magenta]{Reports.Title}[/]").Centered());

            var choices = MenuOptions.ReportMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            if (loggedInUser is Director)
                choices.Insert(choices.Count - 1, $"{choices.Count}. {Reports.RequestPrintableReport}");

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Reports.SelectReport)
                    .AddChoices(choices));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "Production Summary":
                    ShowProductionSummary(factory, loggedInUser);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.ProductionSummaryReportGenerated,
                        loggedInUser.Username);
                    break;
                case "Employee Report":
                    ShowEmployeeReport(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.EmployeeReportGenerated, loggedInUser.Username);
                    break;
                case "Batch Revenue Summary":
                    ShowBatchRevenueSummary(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.BatchRevenueReportGenerated, loggedInUser.Username);
                    break;
                case "Order Backlog Summary":
                    ShowOrderBacklogSummary(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.OrderBacklogReportGenerated, loggedInUser.Username);
                    break;
                case "Request Printable Report":
                    RequestPrintableReport(factory, loggedInUser, loggerService);
                    break;
                case "Return to Main Menu":
                    return;
            }
        }
    }

    private static void ShowEmployeeReport(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Reports.StaffingReportTitle}[/]").Centered());
        EmployeeMenuHandler.DisplayStaffTable(factory);
        Pause();
    }

    private static void RequestPrintableReport(Factory factory, Employee director, ILoggerService loggerService)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Reports.RequestPrintableReport}[/]").Centered());

        var reportType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Reports.SelectReportToRequest)
                .AddChoices("Production Summary", "Employee Report", "Batch Revenue Summary", "Order Backlog Summary"));

        factory.AddReportRequest(new ReportRequest(reportType, director.Name));

        AnsiConsole.MarkupLine(string.Format(Reports.RequestSubmitted, reportType));
        loggerService.LogInfo(LogOrigin.USER, LogEvent.PrintableReportRequested,
            $"Report '{reportType}' requested by {director.Username}");
        Pause();
    }

    private static void ShowProductionSummary(Factory factory, Employee loggedInUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[magenta]Production Summary[/]").Centered());

        double inventoryValue = 0;
        foreach (var product in factory.Inventory.Where(p => !p.IsSold))
        {
            inventoryValue += product.SellingPrice * product.Quantity;
        }

        var grid = new Grid().AddColumns(2);
        grid.AddRow(Reports.GeneratedBy, Markup.Escape(loggedInUser.Name));
        grid.AddRow(Reports.Timestamp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        grid.AddRow(Reports.InventoryItemTypes, factory.Inventory.Count(p => !p.IsSold).ToString());
        grid.AddRow(Reports.InventoryUnits, GetTotalInventoryUnits(factory).ToString());
        grid.AddRow(Reports.EstInventoryValue, $"${inventoryValue:F2}");
        grid.AddRow(Reports.Employees, factory.Employees.Count.ToString());
        grid.AddRow(Reports.Machines, factory.Machines.Count.ToString());

        AnsiConsole.Write(new Panel(grid).Header($"[bold]{Reports.SnapshotPanelHeader}[/]").Border(BoxBorder.Rounded));
        Pause();
    }

    private static void ShowBatchRevenueSummary(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[magenta]Batch Revenue Summary[/]").Centered());

        if (factory.BatchCount == 0)
        {
            AnsiConsole.MarkupLine(Reports.NoBatches);
            Pause();
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("BatchId");
        table.AddColumn("Product");
        table.AddColumn("Qty");
        table.AddColumn("Cost");
        table.AddColumn("Sell Price");
        table.AddColumn("Status");

        double totalPotentialRevenue = 0;
        double totalCost = 0;

        for (var i = 0; i < factory.BatchCount; i++)
        {
            var batch = factory.Batches[i];

            totalCost += batch.TotalCost;
            if (batch.UnitSellPrice.HasValue) totalPotentialRevenue += batch.UnitSellPrice.Value * batch.Quantity;

            table.AddRow(
                batch.BatchId,
                batch.ProductName,
                batch.Quantity.ToString(),
                $"${batch.UnitProductionCost:F2}",
                batch.UnitSellPrice?.ToString("F2") ?? "-",
                batch.IsSold ? "Sold" : batch.IsPriced ? "Priced" : "Open"
            );
        }

        AnsiConsole.Write(table);
        AnsiConsole.Write(new Panel(new Markup(
            $"{Reports.TotalProductionCost} ${totalCost:F2}\n" +
            $"{Reports.PotentialRevenue} ${totalPotentialRevenue:F2}")
        ).Border(BoxBorder.Rounded).Header($"[bold]{Reports.BatchSummaryPanelHeader}[/]"));
        Pause();
    }

    private static void ShowOrderBacklogSummary(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[magenta]Order Backlog Summary[/]").Centered());

        if (factory.OrderCount == 0)
        {
            AnsiConsole.MarkupLine(Reports.NoPendingOrders);
            Pause();
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("OrderId");
        table.AddColumn("Product");
        table.AddColumn("Qty");
        table.AddColumn("Completed");
        table.AddColumn("Assigned Tech");
        table.AddColumn("Status");

        foreach (var order in factory.PendingOrders)
        {
            table.AddRow(
                order.OrderId,
                order.ProductName,
                order.Quantity.ToString(),
                order.CompletedCount.ToString(),
                order.AssignedTechnicianId.ToString(),
                order.IsComplete ? "Complete" : "In Progress"
            );
        }

        AnsiConsole.Write(table);
        Pause();
    }

    private static int GetTotalInventoryUnits(Factory factory)
    {
        var totalUnits = 0;

        foreach (var product in factory.Inventory.Where(p => !p.IsSold))
        {
            totalUnits += product.Quantity;
        }

        return totalUnits;
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine(Common.PressKeyToReturn);
        Console.ReadKey(true);
    }
}