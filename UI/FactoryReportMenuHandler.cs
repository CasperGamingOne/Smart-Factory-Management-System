using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class FactoryReportMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[cyan]FACTORY REPORT MENU[/]").Centered());

            var menuOptions = MenuOptions.FactoryReportMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose a factory report:")
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "Factory Overview":
                    ShowOverview(factory, loggedInUser);
                    break;
                case "Staffing Report":
                    ShowStaffingReport(factory);
                    break;
                case "Machine Fleet Report":
                    ShowMachineFleetReport(factory);
                    break;
                case "Inventory Report":
                    ShowInventoryReport(factory);
                    break;
                case "View Operation History":
                    ShowOperationHistory();
                    break;
                case "Return to Main Menu":
                    return;

            }
        }
    }

    //*****
    private static void ShowOperationHistory()
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[cyan]Operation History Log[/]").Centered());

        string logFilePath = "operations.txt";

        if (!File.Exists(logFilePath))
        {
            AnsiConsole.MarkupLine("[yellow]No operation history found yet.[/]");
        }
        else
        {
            // Citim toate liniile din fișier
            var lines = File.ReadAllLines(logFilePath);

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("Timestamp");
            table.AddColumn("User");
            table.AddColumn("Action");

            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 3)
                {
                    table.AddRow(parts[0].Trim(), parts[1].Trim(), parts[2].Trim());
                }
            }
            AnsiConsole.Write(table);
        }

        Pause();
    }

    */
    private static void ShowOverview(Factory factory, Employee loggedInUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[cyan]Factory Overview[/]").Centered());

        var grid = new Grid().AddColumns(2);
        grid.AddRow("[bold white]Requested By[/]", Markup.Escape(loggedInUser.Name));
        grid.AddRow("[bold white]Employees[/]", factory.EmployeeCount.ToString());
        grid.AddRow("[bold white]Machines[/]", factory.MachineCount.ToString());
        grid.AddRow("[bold white]Inventory Item Types[/]", factory.ProductCount.ToString());
        grid.AddRow("[bold white]Pending Orders[/]", factory.OrderCount.ToString());
        grid.AddRow("[bold white]Batches[/]", factory.BatchCount.ToString());

        AnsiConsole.Write(new Panel(grid).Border(BoxBorder.Rounded).Header("[bold]Factory Snapshot[/]"));
        Pause();
    }

    private static void ShowStaffingReport(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[cyan]Staffing Report[/]").Centered());

        if (factory.EmployeeCount == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No staff registered.[/]");
            Pause();
            return;
        }

        EmployeeMenuHandler.DisplayStaffTable(factory);
        Pause();
    }

    private static void ShowMachineFleetReport(Factory factory)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[cyan]Machine Fleet Report[/]").Centered());

        if (factory.MachineCount == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No machines registered.[/]");
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

        for (var i = 0; i < factory.MachineCount; i++)
        {
            var machine = factory.Machines[i];
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
        AnsiConsole.Write(new Rule("[cyan]Inventory Report[/]").Centered());

        if (factory.ProductCount == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No inventory items registered.[/]");
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

        for (var i = 0; i < factory.ProductCount; i++)
        {
            var product = factory.Inventory[i];
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
            $"[bold white]Total Units:[/] {totalQuantity}\n" +
            $"[bold white]Estimated Inventory Value:[/] ${totalValue:F2}")
        ).Border(BoxBorder.Rounded).Header("[bold]Inventory Totals[/]"));
        Pause();
    }

    private static void Pause()
    {
        AnsiConsole.MarkupLine("\n[grey]Press any key to return...[/]");
        Console.ReadKey(true);
    }

}