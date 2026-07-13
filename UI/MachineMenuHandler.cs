using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class MachineMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser)
    {
        var inRoom = true;
        while (inRoom)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[cyan]MACHINE MONITORING & MAINTENANCE[/]").Centered());

            var menuOptions = MenuOptions.MachineMenu.Select((item, index) => $"{index + 1}. {item}").ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select a diagnostic option:[/]")
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "Overall Fleet Status Overview":
                    DisplayFleetOverview(factory);
                    break;

                case "Run Deep Component Inspection":
                    if (loggedInUser is not Technician)
                        AnsiConsole.MarkupLine(
                            $"[red]❌ Access Denied: {loggedInUser.Role} cannot perform this action.[/]");
                    else
                        RunInspection(factory);
                    break;

                case "Fulfill Pending Orders":
                    ProductionMenuHandler.Run(factory, loggedInUser);
                    break;
                case "Return to Main Menu":
                    inRoom = false;
                    break;
            }

            if (inRoom)
            {
                AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                Console.ReadKey(true);
            }
        }
    }

    private static void DisplayFleetOverview(Factory factory)
    {
        var table = new Table().Border(TableBorder.Square);
        table.AddColumn("[cyan]Machine Asset[/]");
        table.AddColumn("[cyan]Manufacturer[/]");
        table.AddColumn("[cyan]Operational Status[/]");
        table.AddColumn("[cyan]Structural Condition[/]");

        for (var i = 0; i < factory.MachineCount; i++)
        {
            var mach = factory.Machines[i];
            table.AddRow(mach.Name ?? "-", mach.Manufacturer ?? "-", mach.Status.ToString(),
                mach.Condition.ToString());
        }

        AnsiConsole.Write(table);
    }

    private static void RunInspection(Factory factory)
    {
        if (factory.MachineCount == 0)
        {
            AnsiConsole.MarkupLine("[red]No machines are currently provisioned in the asset index.[/]");
            return;
        }

        var selector = new SelectionPrompt<Machine>()
            .Title("Select a machine to manage:")
            .PageSize(10)
            .UseConverter(m =>
            {
                var statusColor = m.Condition == MachineCondition.Critical ? "red" : "green";
                return $"[{statusColor}]{m.Name}[/] - [dim]Status: {m.Status}[/]";
            });
        for (var i = 0; i < factory.MachineCount; i++) selector.AddChoice(factory.Machines[i]);

        var chosenMachine = AnsiConsole.Prompt(selector);

        AnsiConsole.MarkupLine($"\n[bold underline]Auditing Component Stack for: {chosenMachine.Name}[/]");

        chosenMachine.InspectMachine();

        if (chosenMachine.NeedsRepair())
        {
            AnsiConsole.MarkupLine("[yellow]This machine has parts that are not in excellent condition.[/]");

            if (AnsiConsole.Confirm("Repair this machine now?")) chosenMachine.RepairMachine();
        }
        else
        {
            AnsiConsole.MarkupLine("[green]This machine does not currently need repairs.[/]");
        }
    }
}