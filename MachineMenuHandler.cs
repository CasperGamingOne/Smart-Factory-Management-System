using Spectre.Console;
using System.Security.Principal;

namespace Smart_Factory_Management_System
{
    internal static class MachineMenuHandler
    {
        public static void Run(Factory factory, Employee loggedInUser)
        {
            bool inRoom = true;
            while (inRoom)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[cyan]MACHINE MONITORING & MAINTENANCE[/]").Centered());

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select a diagnostic option:[/]")
                        .AddChoices(new[] {
                            "1. Overall Fleet Status Overview",
                            "2. Run Deep Component Inspection",
                            "3. Return to Main Menu"
                        }));

                switch (choice)
                {
                    case "1. Overall Fleet Status Overview":
                        DisplayFleetOverview(factory);
                        break;

                    case "2. Run Deep Component Inspection":
                        // Restrict execution based on roles if your design calls for it
                        if (loggedInUser is not Technician)  /// better handling for role check
                        {
                            AnsiConsole.MarkupLine($"[red]❌ Access Denied: {loggedInUser.Role} cannot perform this action.[/]");
                        }
                        else
                        {
                            RunInspection(factory);
                        }
                        break;

                    case "3. Return to Main Menu":
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

            for (int i = 0; i < factory.MachineCount; i++)
            {
                var mach = factory.Machines[i];
                if (mach != null)
                {
                    table.AddRow(mach.Name, mach.Manufacturer, mach.Status.ToString(), mach.Condition.ToString());
                }
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

            // Let the user choose exactly which machine they wish to audit
            var selector = new SelectionPrompt<Machine>()
                .Title("Select a machine to manage:")
                .PageSize(10)
                // This converts the machine object into the display string
                .UseConverter(m =>
                {
                    string statusColor = m.Condition == MachineCondition.Critical ? "red" : "green";
                    return $"[{statusColor}]{m.Name}[/] - [dim]Status: {m.Status}[/]";
                });
            for (int i = 0; i < factory.MachineCount; i++)
            {
                selector.AddChoice(factory.Machines[i]);
            }

            var chosenMachine = AnsiConsole.Prompt(selector);

            AnsiConsole.MarkupLine($"\n[bold underline]Auditing Component Stack for: {chosenMachine.Name}[/]");

            // Polymorphically prints component logs cleanly via composition arrays
            // based on our electronic factory design specs
            chosenMachine.InspectMachine();
        }
    }
}
