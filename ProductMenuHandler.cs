using Spectre.Console;

namespace Smart_Factory_Management_System
{ 
    internal class ProductMenuHandler
    {
        public static void Run(Factory factory, Employee currentUser)
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[yellow]⚡ Product & Inventory Management System ⚡[/]").Centered());
                AnsiConsole.WriteLine();

                var summaryGrid = new Grid();
                summaryGrid.AddColumn();
                summaryGrid.AddRow(new Markup($"[grey]Operator Session:[/] [cyan]{currentUser.Name}[/] ([yellow]{currentUser.Role}[/])"));
                summaryGrid.AddRow(new Markup($"[grey]Warehouse Storage Stock:[/] [green]{factory.ProductCount} / {factory.Inventory.Length} units[/]"));

                AnsiConsole.Write(
                    new Panel(summaryGrid)
                        .Header("[bold blue] Storage Inventory Card [/]")
                        .Border(BoxBorder.Rounded)
                        .BorderStyle(new Style(Color.Blue))
                );
                AnsiConsole.WriteLine();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[bold white]Navigate to an inventory operation:[/]")
                        .PageSize(10)
                        .AddChoices(MenuOptions.ProductMenu));

                switch (choice)
                {
                    case "1. View Finished Goods Stock":
                        DisplayInventoryTable(factory);
                        break;
                    case "2. View Inventory Financial & Capacity Analytics":
                        DisplayInventoryAnalytics(factory);
                        break;
                    case "3. Sales & Orders":
                        // Reuse Sales menu view; if user is SalesAgent, open full Sales UI
                        if (currentUser is SalesAgent)
                            SalesMenuHandler.Run(factory, currentUser);
                        else
                            SalesMenuHandler.ShowPendingOrders(factory);
                        break;
                    case "4. Return to Main Menu":
                        return;
                }
            }
        }

        private static void DisplayInventoryTable(Factory factory)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green]📦 Finished Electronics Inventory Stock[/]").Centered());
            AnsiConsole.WriteLine();

            if (factory.ProductCount == 0)
            {
                AnsiConsole.Write(new Panel("[yellow]⚠️ Warehouse stock is currently empty. Fire up your Production Lines to manufacture goods![/]").Border(BoxBorder.Square));
                AnsiConsole.WriteLine("\nPress any key to return...");
                Console.ReadKey(true);
                return;
            }

            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[bold blue]Slot[/]");
            table.AddColumn("[bold cyan]Component Name[/]");
            table.AddColumn("[bold green]Quantity[/]");
            table.AddColumn("[bold green]Production Cost ($)[/]");
            table.AddColumn("[bold yellow]Technical Specifications[/]");
            table.AddColumn("[bold magenta]Value ($)[/]");

            // Populate table rows with inventory data
            for (int i = 0; i < factory.ProductCount; i++)
            {
                var product = factory.Inventory[i];
                if (product != null)
                {
                    string dynamicSpecs = product.GetTechnicalSpecifications();

                    table.AddRow(
                        (i + 1).ToString(),
                        product.Name ?? "-",
                        product.Quantity.ToString(),
                        product.ProductionCost.ToString("F2"),
                        dynamicSpecs ?? "-",
                        $"${product.SellingPrice:F2}"
                    );
                }
            }

            AnsiConsole.Write(table);
            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        private static void DisplayInventoryAnalytics(Factory factory)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[magenta]📈 Inventory Portfolio & Capacity Metrics[/]").Centered());
            AnsiConsole.WriteLine();

            double cumulativeValue = 0;
            int cpuCount = 0;
            int pcbCount = 0;

            for (int i = 0; i < factory.ProductCount; i++)
            {
                var product = factory.Inventory[i];
                if (product != null)
                {
                    cumulativeValue += product.SellingPrice;

                    // Type checking subclasses safely for metrics grouping
                    if (product is Microprocessor) cpuCount++;
                    else if (product is Motherboard) pcbCount++;
                }
            }

            double storageUtilization = ((double)factory.ProductCount / factory.Inventory.Length) * 100;

            var statsGrid = new Grid().AddColumns(2);
            statsGrid.AddRow("[bold white]Total Volume Level:[/]", $"[green]{factory.ProductCount} items[/] (📊 CPUs: {cpuCount} | ⚙️ PCBs: {pcbCount} )");
            statsGrid.AddRow("[bold white]Asset Portfolio Valuation:[/]", $"[yellow]${cumulativeValue:F2} USD[/]");
            statsGrid.AddRow("[bold white]Warehouse Occupancy Rate:[/]", $"[cyan]{storageUtilization:F1}% utilized[/]");

            AnsiConsole.Write(new Panel(statsGrid).Header("[bold magenta] Business Operations Analysis [/]").Border(BoxBorder.Double));
            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

    }
}