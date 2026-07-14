using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class EmployeeMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, IAuthRepository<Employee> repository)
    {
        var inRoom = true;
        while (inRoom)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[green]EMPLOYEE MANAGEMENT MODULE[/]").Centered());

            var menuOptions = MenuOptions.EmployeeManagementMenu.Select((item, index) => $"{index + 1}. {item}")
                .ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[yellow]Select an administrative action:[/]")
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "View All Registered Staff":
                    DisplayStaffTable(factory);
                    break;

                case "Add New Employee":
                    if (loggedInUser is not Director)
                        AnsiConsole.MarkupLine(
                            $"[red]❌ Access Denied: {loggedInUser.Role} cannot perform this action.[/]");
                    else
                        AddNewEmployeeFlow(factory, repository);

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
                factory.Employees[i].ShowActivity()
            );

        AnsiConsole.Write(table);
    }

    private static void AddNewEmployeeFlow(Factory factory, IAuthRepository<Employee> repository)
    {
        var name = AnsiConsole.Ask<string>("Enter Employee Full Name:");
        var username = AnsiConsole.Ask<string>("Enter Employee Username:");
        var password = AnsiConsole.Ask<string>("Enter Employee Password:");
        var hashedPassword = SecurityHelper.HashPassword(password);

        var role = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Select Job Title:")
                .AddChoices(MenuOptions.EmployeeRoles));

        Employee? newEmployee;
        switch (role)
        {
            case "Technician":
                newEmployee = new Technician(name, username, hashedPassword);
                break;
            case "Sales Agent":
                newEmployee = new SalesAgent(name, username, hashedPassword);
                break;
            case "Accountant":
                newEmployee = new Accountant(name, username, hashedPassword);
                break;
            default:
                AnsiConsole.MarkupLine("[red]❌ Invalid role selection. Operation aborted.[/]");
                return;
        }

        newEmployee.IsFirstTimeLogin = true;
        factory.AddEmployee(newEmployee);

        var users = repository.LoadUsers();
        users.Add(newEmployee);
        repository.SaveUsers(users);

        AnsiConsole.MarkupLine($"[green]✔ Employee '{name}' registered successfully![/]");
    }
}