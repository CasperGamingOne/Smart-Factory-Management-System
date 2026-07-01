using Spectre.Console;

namespace Smart_Factory_Management_System
{
    internal static class EmployeeMenuHandler
    {
        public static void Run(Factory factory, Employee loggedInUser)
        {
            bool inRoom = true;
            while (inRoom)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[green]EMPLOYEE MANAGEMENT MODULE[/]").Centered());

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an administrative action:[/]")
                        .AddChoices(new[] {
                            "1. View All Registered Staff",
                            "2. Add New Employee",
                            "3. Return to Main Menu"
                        }));

                switch (choice)
                {
                    case "1. View All Registered Staff":
                        DisplayStaffTable(factory);
                        break;

                    case "2. Add New Employee":
                        // Restrict execution based on roles if your design calls for it
                        if (loggedInUser.Role.ToString().ToLower() != "director")
                        {
                            AnsiConsole.MarkupLine("[red]❌ Access Denied: Only Directors can add new staff.[/]");
                        }
                        else
                        {
                            AddNewEmployeeFlow(factory);
                        }
                        break;

                    case "3. Return to Main Menu":
                        inRoom = false; // Collapses this room's context frame naturally
                        break;
                }

                if (inRoom)
                {
                    AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                    Console.ReadKey(true);
                }
            }
        }

        private static void DisplayStaffTable(Factory factory)
        {
            var table = new Table().Border(TableBorder.Rounded);
            table.AddColumn("[yellow]ID[/]");
            table.AddColumn("[yellow]Name[/]");
            table.AddColumn("[yellow]Assigned Role[/]");

            // Safely loop up to employeeCount to prevent NullReferenceExceptions
            for (int i = 0; i < factory.EmployeeCount; i++)
            {
                if (factory.Employees[i] != null)
                {
                    table.AddRow(factory.Employees[i].Id, factory.Employees[i].Name, factory.Employees[i].Role.ToString());
                }
            }

            AnsiConsole.Write(table);
        }

        private static void AddNewEmployeeFlow(Factory factory)
        {
            string id = AnsiConsole.Ask<string>("Enter new Employee ID:");
            string name = AnsiConsole.Ask<string>("Enter Employee Full Name:");

            // Assuming standard system roles
            var role = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select Job Title:")
                    .AddChoices(new[] { "Technician", "Sales Agent", "Accountant" }));

            switch(role)
            {
                case "Technician":
                    factory.AddEmployee(new Technician(id, name));
                    break;
                case "Sales Agent":
                    factory.AddEmployee(new SalesAgent(id, name));
                    break;
                case "Accountant":
                    factory.AddEmployee(new Accountant(id, name));
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]❌ Invalid role selection. Operation aborted.[/]");
                    return;
            }
            // Add back into your underlying factory system safely
            
            AnsiConsole.MarkupLine($"[green]✔ Employee '{name}' registered successfully![/]");
        }
    }
}
