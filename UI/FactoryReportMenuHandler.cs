using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class FactoryReportMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[cyan]{Reports.OverviewTitle}[/]").Centered());

            var menuOptions = MenuOptions.FactoryReportMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Reports.ChooseFactoryReport)
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "Factory Overview":
                    ShowOverview(factory, loggedInUser);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.FactoryOverviewReportGenerated,
                        loggedInUser.Username);
                    break;
                case "Staffing Report":
                    ShowStaffingReport(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.StaffingReportGenerated, loggedInUser.Username);
                    break;
                case "Machine Fleet Report":
                    ShowMachineFleetReport(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.MachineFleetReportGenerated, loggedInUser.Username);
                    break;
                case "Inventory Report":
                    ShowInventoryReport(factory);
                    loggerService.LogInfo(LogOrigin.USER, LogEvent.InventoryReportGenerated, loggedInUser.Username);
                    break;
                case "Return to Main Menu":
                    return;
            }
        }
    }


    private static void ShowOverview(Factory factory, Employee loggedInUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[cyan]{Reports.FactoryOverviewTitle}[/]").Centered());

        var grid = new Grid().AddColumns(2);
        grid.AddRow(Reports.RequestedBy, Markup.Escape(loggedInUser.Name));
        grid.AddRow(Reports.Employees, factory.Employees.Count.ToString());
        grid.AddRow(Reports.Machines, factory.Machines.Count.ToString());
        grid.AddRow(Reports.InventoryItemTypes, factory.Inventory.Count.ToString());
        grid.AddRow(Reports.PendingOrders, factory.OrderCount.ToString());
        grid.AddRow(Reports.Batches, factory.BatchCount.ToString());

        AnsiConsole.Write(new Panel(grid).Border(BoxBorder.Rounded)
            .Header($"[bold]{Reports.FactorySnapshotHeader}[/]"));
        Pause();
    }

    private static void ShowStaffingReport(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[cyan]{Reports.StaffingReportTitle}[/]").Centered());

        if (factory.Employees.Count == 0)
        {
            AnsiConsole.MarkupLine(Reports.NoStaff);
            Pause();
            return;
        }

        EmployeeMenuHandler.DisplayStaffTable(factory);
        Pause();
    }

    private static void ShowMachineFleetReport(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[cyan]{Reports.MachineFleetReportTitle}[/]").Centered());

        if (factory.Machines.Count == 0)
        {
            AnsiConsole.MarkupLine(Reports.NoMachines);
            Pause();
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("ID");
        table.AddColumn("Name");
        table.AddColumn("Manufacturer");
        table.AddColumn("Condition");
        table.AddColumn("Status");
        table.AddColumn("Age (days)");

        foreach (var machine in factory.Machines)
        {
            table.AddRow(
                machine.Id.ToString(),
                machine.Name ?? "-",
                machine.Manufacturer ?? "-",
                machine.Condition.ToString(),
                machine.Status.ToString(),
                ((int)machine.GetMachineAge().TotalDays).ToString()
            );
        }

        AnsiConsole.Write(table);
        Pause();
    }

    private static void ShowInventoryReport(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[cyan]{Reports.InventoryReportTitle}[/]").Centered());

        if (!factory.Inventory.Any(p => !p.IsSold))
        {
            AnsiConsole.MarkupLine(Reports.NoInventory);
            Pause();
            return;
        }

        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("Item");
        table.AddColumn("Qty");
        table.AddColumn("Cost");
        table.AddColumn("Price");
        table.AddColumn("Inventory Value");

        double totalValue = 0;
        var totalQuantity = 0;

        foreach (var product in factory.Inventory.Where(p => !p.IsSold))
        {
            {
                var itemValue = product.SellingPrice * product.Quantity;
                totalValue += itemValue;
                totalQuantity += product.Quantity;

                table.AddRow(
                    product.Name ?? "-",
                    product.Quantity.ToString(),
                    $"${product.ProductionCost:F2}",
                    $"${product.SellingPrice:F2}",
                    $"${itemValue:F2}"
                );
            }
        }

        AnsiConsole.Write(table);
        AnsiConsole.Write(new Panel(new Markup(
            $"{Reports.TotalUnits} {totalQuantity}\n" +
            $"{Reports.EstInventoryValueTotal} ${totalValue:F2}")
        ).Border(BoxBorder.Rounded).Header($"[bold]{Reports.InventoryTotalsHeader}[/]"));
        Pause();
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine(Common.PressKeyToReturn);
        Console.ReadKey(true);
    }
}