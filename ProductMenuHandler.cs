using Spectre.Console;

namespace Smart_Factory_Management_System
{ 
    internal class ProductMenuHandler
    {
        public static void Show(Factory factory, Employee currentUser)
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
                        .AddChoices(new[] {
                            "1. View Finished Goods Stock (Polymorphic Table)",
                            "2. View Inventory Financial & Capacity Analytics",
                            "3. Manually Register/Seed Asset (Manager Override)",
                            "4. Return to Main Menu"
                        }));

                switch (choice)
                {
                    case "1. View Finished Goods Stock (Polymorphic Table)":
                        DisplayInventoryTable(factory);
                        break;
                    case "2. View Inventory Financial & Capacity Analytics":
                        DisplayInventoryAnalytics(factory);
                        break;
                    case "3. Manually Register/Seed Asset (Manager Override)":
                        HandleManualProductRegistration(factory, currentUser);
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
            table.AddColumn("[bold green]Unique SKU / Serial[/]");
            table.AddColumn("[bold yellow]Technical Specifications (Polymorphic)[/]");
            table.AddColumn("[bold magenta]Value ($)[/]");

            // Loop strictly to .ProductCount tracking bound to guarantee null safety
            for (int i = 0; i < factory.ProductCount; i++)
            {
                var product = factory.Inventory[i];
                if (product != null)
                {
                    string dynamicSpecs = product.GetTechnicalSpecifications();

                    table.AddRow(
                        (i + 1).ToString(),
                        product.Name,
                        product.ProductionCost.ToString(),
                        dynamicSpecs,
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
            int opticsCount = 0;

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
            statsGrid.AddRow("[bold white]Total Volume Level:[/]", $"[green]{factory.ProductCount} items[/] (📊 CPUs: {cpuCount} | ⚙️ PCBs: {pcbCount} | 📷 Lenses: {opticsCount})");
            statsGrid.AddRow("[bold white]Asset Portfolio Valuation:[/]", $"[yellow]${cumulativeValue:F2} USD[/]");
            statsGrid.AddRow("[bold white]Warehouse Occupancy Rate:[/]", $"[cyan]{storageUtilization:F1}% utilized[/]");

            AnsiConsole.Write(new Panel(statsGrid).Header("[bold magenta] Business Operations Analysis [/]").Border(BoxBorder.Double));
            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }

        private static void HandleManualProductRegistration(Factory factory, Employee currentUser)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[red]⚙️ Manual Stock Override System[/]").Centered());
            AnsiConsole.WriteLine();

            // Authorization check
            if (currentUser.Role.ToString().ToLower() != "manager")
            {
                AnsiConsole.Write(new Panel("[red]❌ ACCESS DENIED: Only personnel with structural 'Manager' roles can manually override warehouse tracking logs.[/]").Border(BoxBorder.Rounded));
                AnsiConsole.WriteLine("\nPress any key to return...");
                Console.ReadKey(true);
                return;
            }

            AnsiConsole.MarkupLine("[bold white]Select specific product concrete class to initialize:[/]");
            var categorySelection = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .AddChoices(new[] { "Microprocessor", "Motherboard", "Camera Module" }));

            string baselineModelName = AnsiConsole.Ask<string>($"Enter common designation name for this [cyan]{categorySelection}[/]:");
            double financialValue = AnsiConsole.Ask<double>("Set baseline unit manufacturing cost value ($):");

            // Build unique SKU tracking token
            string preCode = categorySelection.Substring(0, 3).ToUpper();
            string generatedSku = $"SKU-{DateTime.Now.Ticks.ToString().Substring(11)}-{preCode}";

            // Polymorphic Reference Variable initialization
            Product specializedProduct = null;

            // Conditional block branches based on target subclass parameters
            switch (categorySelection)
            {
                case "Microprocessor":
                    int processingCores = AnsiConsole.Prompt(
                        new TextPrompt<int>("Specify processing core array allocation count:")
                            .ValidationErrorMessage("[red]Please provide a positive integer allocation setup.[/]")
                            .Validate(c => c > 0));

                    double coreClockSpeed = AnsiConsole.Ask<double>("Set processor core computing frequency target (GHz):");

                    // Specific Concrete Instantiation
                    specializedProduct = new Microprocessor(baselineModelName, generatedSku, financialValue, DateTime.Now, processingCores, coreClockSpeed);
                    break;

                case "Motherboard":
                    string socketStandard = AnsiConsole.Ask<string>("Enter target layout architectural socket standard type (e.g., AM5, LGA1700):");
                    string physicalForm = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("Select board layout form standard dimension factor:")
                            .AddChoices(new[] { "ATX", "Micro-ATX", "Mini-ITX", "E-ATX" }));

                    // Specific Concrete Instantiation
                    specializedProduct = new Motherboard(baselineModelName, generatedSku, financialValue, DateTime.Now, socketStandard, physicalForm);
                    break;
            }

            if (specializedProduct != null)
            {
                factory.AddProduct(specializedProduct);
                AnsiConsole.WriteLine();
                AnsiConsole.Write(new Panel($"[green]✅ Polymorphic Derived Asset Registered!\nLogged Serial: [yellow]{generatedSku}[/]\nSpecs: {specializedProduct.GetTechnicalSpecifications()}[/]").Border(BoxBorder.Rounded));
            }

            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
        }
    }
}