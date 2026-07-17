using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class ReportMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<ReportRequest> reportRequestsRepo)
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
                    loggerService.LogInfo(LogOrigin.User, LogEvent.ProductionSummaryReportGenerated,
                        loggedInUser.Username);
                    break;
                case "Employee Report":
                    ShowEmployeeReport(factory);
                    loggerService.LogInfo(LogOrigin.User, LogEvent.EmployeeReportGenerated, loggedInUser.Username);
                    break;
                case "Order Backlog Summary":
                    ShowOrderBacklogSummary(factory);
                    loggerService.LogInfo(LogOrigin.User, LogEvent.OrderBacklogReportGenerated, loggedInUser.Username);
                    break;
                case "Request Printable Report":
                    RequestPrintableReport(factory, loggedInUser, loggerService, reportRequestsRepo);
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

    private static void RequestPrintableReport(Factory factory, Employee director, ILoggerService loggerService,
        IJsonRepository<ReportRequest> reportRequestsRepo)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Reports.RequestPrintableReport}[/]").Centered());

        var reportType = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Reports.SelectReportToRequest)
                .AddChoices("Production Summary", "Employee Report", "Batch Revenue Summary", "Order Backlog Summary"));

        var destination = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Reports.ChooseExportFolderPrompt)
                .AddChoices(Reports.ExportDestDesktop, Reports.ExportDestReportsFolder));

        var request = new ReportRequest(reportType, director.Name)
        {
            ExportDestination = destination == Reports.ExportDestDesktop ? "Desktop" : "ReportsFolder"
        };

        factory.AddReportRequest(request);
        reportRequestsRepo.Save(factory.PendingReportRequests);

        AnsiConsole.MarkupLine(string.Format(Reports.RequestSubmitted, reportType));
        AnsiConsole.MarkupLine(string.Format(Reports.ExportDestSelected, destination));
        loggerService.LogInfo(LogOrigin.User, LogEvent.PrintableReportRequested,
            $"Report '{reportType}' (Target: {destination}) requested by {director.Username}");
        Pause();
    }

    internal static void ShowProductionSummary(Factory factory, Employee loggedInUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Reports.ProductionSummaryHeader}[/]").Centered());

        var inventoryValue = factory.Inventory.Where(p => !p.IsSold).Sum(p => p.SellingPrice * p.Quantity);
        var totalUnsoldUnits = GetTotalInventoryUnits(factory);

        var grid = new Grid().AddColumns(2);
        grid.AddRow(Reports.GeneratedBy, Markup.Escape(loggedInUser.Name));
        grid.AddRow(Reports.Timestamp, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        grid.AddRow(Reports.InventoryItemTypes, factory.Inventory.Count(p => !p.IsSold).ToString());
        grid.AddRow(Reports.InventoryUnits, totalUnsoldUnits.ToString());
        grid.AddRow(Reports.EstInventoryValue, $"${inventoryValue:F2}");
        grid.AddRow(Reports.Employees, factory.Employees.Count.ToString());
        grid.AddRow(Reports.Machines, factory.Machines.Count.ToString());

        AnsiConsole.Write(new Panel(grid).Header($"[bold]{Reports.SnapshotPanelHeader}[/]").Border(BoxBorder.Rounded));
        AnsiConsole.WriteLine();

        // 1. Batches Table (Sold and Unsold)
        AnsiConsole.MarkupLine(Reports.BatchesDetailsHeader);
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(Reports.ColumnBatchId);
        table.AddColumn(Reports.ColumnProduct);
        table.AddColumn(Reports.ColumnQty);
        table.AddColumn(Reports.ColumnProductionCostUnit);
        table.AddColumn(Reports.ColumnSellPriceUnit);
        table.AddColumn(Reports.ColumnStatus);

        double totalExpenses = 0;
        double totalIncome = 0;

        foreach (var batch in factory.Batches)
        {
            totalExpenses += batch.TotalCost;
            if (batch.IsSold && batch.UnitSellPrice.HasValue) totalIncome += batch.UnitSellPrice.Value * batch.Quantity;

            table.AddRow(
                batch.BatchId,
                batch.ProductName,
                batch.Quantity.ToString(),
                $"${batch.UnitProductionCost:F2}",
                batch.IsSold ? $"${batch.UnitSellPrice ?? 0.0:F2}" : "$0.00",
                batch.IsSold ? Reports.StatusSold : Reports.StatusProduced
            );
        }

        if (factory.BatchCount > 0)
            AnsiConsole.Write(table);
        else
            AnsiConsole.MarkupLine(Reports.NoBatchesProduced);

        AnsiConsole.WriteLine();

        // 2. Financial Summary
        var totalProfit = totalIncome - totalExpenses;
        var profitColor = totalProfit >= 0 ? "green" : "red";

        var financeGrid = new Grid().AddColumns(2);
        financeGrid.AddRow(Reports.FinanceExpensesLabel, $"[red]${totalExpenses:F2}[/]");
        financeGrid.AddRow(Reports.FinanceIncomeLabel, $"[green]${totalIncome:F2}[/]");
        financeGrid.AddRow(Reports.FinanceProfitLabel, $"[{profitColor}]${totalProfit:F2}[/]");

        AnsiConsole.Write(new Panel(financeGrid)
            .Header($"[bold yellow]{Reports.FinanceHeader}[/]")
            .Border(BoxBorder.Rounded));
        AnsiConsole.WriteLine();

        Pause();
    }

    private static void ShowOrderBacklogSummary(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[magenta]{Reports.OrderBacklogSummaryHeader}[/]").Centered());

        if (factory.OrderCount == 0)
        {
            AnsiConsole.MarkupLine(Reports.NoPendingOrders);
            Pause();
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(Reports.ColumnOrderId);
        table.AddColumn(Reports.ColumnProduct);
        table.AddColumn(Reports.ColumnQty);
        table.AddColumn(Reports.ColumnCompleted);
        table.AddColumn(Reports.ColumnAssignedTech);
        table.AddColumn(Reports.ColumnStatus);

        foreach (var order in factory.PendingOrders)
        {
            table.AddRow(
                order.OrderId,
                order.ProductName,
                order.Quantity.ToString(),
                order.CompletedCount.ToString(),
                order.AssignedTechnicianId.ToString(),
                order.IsComplete ? Reports.StatusComplete : Reports.StatusInProgress
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