using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class EmployeeMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser)
    {
        var inRoom = true;
        while (inRoom)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green]EMPLOYEE MANAGEMENT MODULE[/]").Centered());

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an administrative action:[/]")
                    .AddChoices(MenuOptions.EmployeeManagementMenu));

            switch (choice)
            {
                case "1. View All Registered Staff":
                    DisplayStaffTable(factory);
                    break;

                case "2. Add New Employee":
                    if (loggedInUser is not Director)
                        AnsiConsole.MarkupLine(
                            $"[red]❌ Access Denied: {loggedInUser.Role} cannot perform this action.[/]");
                    else
                        AddNewEmployeeFlow(factory);

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

    internal static void DisplayStaffTable(Factory factory)
    {
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn("[yellow]ID[/]");
        table.AddColumn("[yellow]Name[/]");
        table.AddColumn("[yellow]Assigned Role[/]");
        table.AddColumn("[yellow]Activity[/]");

        for (var i = 0; i < factory.EmployeeCount; i++)
            table.AddRow(
                factory.Employees[i].Id.ToString(),
                factory.Employees[i].Name,
                factory.Employees[i].Role,
                factory.Employees[i].AfiseazaActivitate()
            );

        AnsiConsole.Write(table);
    }

    private static void AddNewEmployeeFlow(Factory factory)
    {
        var name = AnsiConsole.Ask<string>("Enter Employee Full Name:");

        var role = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Job Title:")
                .AddChoices(MenuOptions.EmployeeRoles));

        switch (role)
        {
            case "Technician":
                factory.AddEmployee(new Technician(name));
                break;
            case "Sales Agent":
                factory.AddEmployee(new SalesAgent(name));
                break;
            case "Accountant":
                factory.AddEmployee(new Accountant(name));
                break;
            default:
                AnsiConsole.MarkupLine("[red]❌ Invalid role selection. Operation aborted.[/]");
                return;
        }

        AnsiConsole.MarkupLine($"[green]✔ Employee '{name}' registered successfully![/]");
    }
}