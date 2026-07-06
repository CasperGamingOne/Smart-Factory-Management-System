using Spectre.Console;

namespace Smart_Factory_Management_System
{
    internal static class SalesMenuHandler
    {
        public static void Run(Factory factory, Employee loggedInUser)
        {
            if (loggedInUser is not SalesAgent)
            {
                AnsiConsole.MarkupLine($"[red]❌ Access Denied: {loggedInUser.Role} cannot access Sales.[/]");
                AnsiConsole.WriteLine("\nPress any key to return...");
                Console.ReadKey(true);
                return;
            }

            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[green]SALES - Create Production Orders[/]").Centered());

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Choose action:")
                        .AddChoices(MenuOptions.SalesMenu));

                switch (choice)
                {
                    case "1. Place Production Order":
                        PlaceOrder(factory);
                        break;
                    case "2. View Pending Orders":
                        ShowPendingOrders(factory);
                        break;
                    case "3. Record Sale for Batch":
                        RecordSale(factory);
                        break;
                    case "4. Return to Main Menu":
                        return;
                }

                AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }

        private static void PlaceOrder(Factory factory)
        {
            AnsiConsole.WriteLine();
            var productChoice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select product to order:")
                    .AddChoices(MenuOptions.ProductTypes));

            int quantity = AnsiConsole.Ask<int>("Enter quantity for the batch:");

            // Choose technician
            var techSelector = new SelectionPrompt<string>()
                .Title("Assign to technician:")
                .AddChoices(MenuOptions.TechAssignChoices);

            var techChoice = AnsiConsole.Prompt(techSelector);
            int technicianId = -1;
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
                    AnsiConsole.MarkupLine("[red]No technicians registered. Ask an admin to add one.[/]");
                    return;
                }

                var techList = new SelectionPrompt<Employee>().Title("Select technician:");
                foreach (var technician in technicians) techList.AddChoice(technician);
                var chosen = AnsiConsole.Prompt(techList);
                technicianId = chosen.Id;
            }

            var order = new ProductionOrder(productChoice, quantity, technicianId);
            factory.AddOrder(order);

            AnsiConsole.MarkupLine(
                $"[green]✔ Order placed: {order.OrderId} - {order.ProductName} x{order.Quantity} assigned to tech #{order.AssignedTechnicianId}[/]");
        }

        private static List<Employee> GetTechnicians(Factory factory)
        {
            var technicians = new List<Employee>();

            for (int i = 0; i < factory.EmployeeCount; i++)
            {
                if (factory.Employees[i] is Technician)
                    technicians.Add(factory.Employees[i]);
            }

            return technicians;
        }

        public static void ShowPendingOrders(Factory factory)
        {
            AnsiConsole.WriteLine();
            if (factory.OrderCount == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No pending orders.[/]");
                return;
            }

            var table = new Table().AddColumn("Order").AddColumn("Product").AddColumn("Qty").AddColumn("Done")
                .AddColumn("TechId");
            for (int i = 0; i < factory.OrderCount; i++)
            {
                var o = factory.PendingOrders[i];
                table.AddRow(o.OrderId, o.ProductName, o.Quantity.ToString(), o.CompletedCount.ToString(),
                    o.AssignedTechnicianId.ToString());
            }

            AnsiConsole.Write(table);
        }

        private static void RecordSale(Factory factory)
        {
            // Offer selling either from completed batches or from existing inventory
            var options = new List<string>();
            if (factory.BatchCount > 0) options.Add("Sell from Batch");
            if (factory.ProductCount > 0) options.Add("Sell from Inventory");
            if (options.Count == 0)
            {
                AnsiConsole.MarkupLine("[yellow]No available inventory or batches to sell.[/]");
                return;
            }

            var pickContext =
                AnsiConsole.Prompt(new SelectionPrompt<string>().Title("Choose sale source:").AddChoices(options));

            if (pickContext == "Sell from Batch")
            {
                var selector = new SelectionPrompt<ProductionBatch>().Title("Select batch to record sale for:");
                for (int i = 0; i < factory.BatchCount; i++) selector.AddChoice(factory.Batches[i]);

                var chosen = AnsiConsole.Prompt(selector);
                if (chosen.IsSold)
                {
                    AnsiConsole.MarkupLine("[yellow]This batch is already marked as sold.[/]");
                    return;
                }

                double soldPrice = AnsiConsole.Ask<double>("Enter unit sold price ($):");
                chosen.IsSold = true;

                AnsiConsole.MarkupLine(
                    $"[green]✔ Recorded sale for batch {chosen.BatchId} at ${soldPrice:F2} per unit.[/]");
            }
            else if (pickContext == "Sell from Inventory")
            {
                var products = new List<Product>();
                for (int i = 0; i < factory.ProductCount; i++)
                {
                    var p = factory.Inventory[i];
                    if (p.Quantity > 0)
                        products.Add(p);
                }

                if (products.Count == 0)
                {
                    AnsiConsole.MarkupLine("[yellow]No stocked items available to sell.[/]");
                    return;
                }

                var prodSelector = new SelectionPrompt<Product>().Title("Select product to sell:")
                    .UseConverter(p => $"{p.Name} (stock: {p.Quantity})");
                foreach (var pr in products) prodSelector.AddChoice(pr);

                var chosen = AnsiConsole.Prompt(prodSelector);
                int qty = AnsiConsole.Ask<int>($"Enter quantity to sell (available: {chosen.Quantity}):");
                if (qty <= 0 || qty > chosen.Quantity)
                {
                    AnsiConsole.MarkupLine("[red]Invalid quantity specified.[/]");
                    return;
                }

                double soldPrice = AnsiConsole.Ask<double>("Enter unit sold price ($):");

                // Decrease stock and create a record batch for accounting
                chosen.Quantity -= qty;
                var batch = new ProductionBatch(chosen.Name ?? "Inventory Sale", qty, chosen.ProductionCost);
                batch.IsSold = true;
                factory.AddBatch(batch);

                AnsiConsole.MarkupLine(
                    $"[green]✔ Sold {qty} units of {chosen.Name} at ${soldPrice:F2} per unit. Batch {batch.BatchId} recorded.[/]");
            }
        }
    }
}