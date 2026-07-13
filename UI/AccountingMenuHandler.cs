using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class AccountingMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser)
    {
        if (loggedInUser is not Accountant)
        {
            AnsiConsole.MarkupLine($"[red]❌ Access Denied: {loggedInUser.Role} cannot access Accounting.[/]");
            AnsiConsole.WriteLine("\nPress any key to return...");
            Console.ReadKey(true);
            return;
        }

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[magenta]ACCOUNTING - Price Produced Batches[/]").Centered());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose action:")
                    .AddChoices(MenuOptions.AccountingMenu));

            switch (choice)
            {
                case "1. View Batches":
                    ShowBatches(factory);
                    break;
                case "2. Set Unit Sell Price for Batch":
                    SetBatchPrice(factory);
                    break;
                case "3. Return to Main Menu":
                    return;
            }

            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
            Console.ReadKey(true);
        }
    }

    private static void ShowBatches(Factory factory)
    {
        AnsiConsole.WriteLine();
        if (factory.BatchCount == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No completed production batches yet.[/]");
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

    private static void SetBatchPrice(Factory factory)
    {
        if (factory.BatchCount == 0)
        {
            AnsiConsole.MarkupLine("[yellow]No batches available to price.[/]");
            return;
        }

        var selector = new SelectionPrompt<ProductionBatch>().Title("Select batch to price:");
        for (var i = 0; i < factory.BatchCount; i++) selector.AddChoice(factory.Batches[i]);

        var chosen = AnsiConsole.Prompt(selector);
        var price = AnsiConsole.Ask<double>("Set unit selling price for this batch ($):");
        chosen.UnitSellPrice = price;

        // Apply price to linked inventory items
        foreach (var idx in chosen.InventoryIndexes)
            if (idx >= 0 && idx < factory.ProductCount)
                factory.Inventory[idx].SellingPrice = price;

        AnsiConsole.MarkupLine($"[green]✔ Batch {chosen.BatchId} priced at ${price:F2} per unit.[/]");
    }
}