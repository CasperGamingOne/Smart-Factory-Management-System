using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class EmployeeMenuHandler
{
    public static void Run(Factory factory, Employee loggedInUser, ILoggerService loggerService,
        IJsonRepository<Employee> repository)
    {
        var inRoom = true;
        while (inRoom)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[green]{Employees.Title}[/]").Centered());

            var menuOptions = MenuOptions.EmployeeManagementMenu.Select((item, index) => $"{index + 1}. {item}")
                .ToList();
            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(Employees.SelectActionPrompt)
                    .AddChoices(menuOptions));

            var option = choice.Split(". ", 2)[1];
            switch (option)
            {
                case "View Registered Staff":
                case "View All Registered Staff":
                    DisplayStaffTable(factory);
                    loggerService.LogInfo(LogOrigin.User, LogEvent.StaffViewed, loggedInUser.Username);
                    break;

                case "Add New Employee":
                    if (loggedInUser is not Director)
                        AnsiConsole.MarkupLine(
                            string.Format(Common.AccessDenied, loggedInUser.Role));
                    else
                        AddNewEmployeeFlow(factory, repository, loggerService, loggedInUser);

                    break;

                case "Remove Employee":
                    if (loggedInUser is not Director)
                        AnsiConsole.MarkupLine(
                            string.Format(Common.AccessDenied, loggedInUser.Role));
                    else
                        RemoveEmployeeFlow(factory, repository, loggerService, loggedInUser);

                    break;

                case "Return to Main Menu":
                    inRoom = false;
                    break;
            }

            if (inRoom)
            {
                AnsiConsole.MarkupLine(Common.PressKeyToContinue);
                Console.ReadKey(true);
            }
        }
    }

    internal static void DisplayStaffTable(Factory factory)
    {
        var table = new Table().Border(TableBorder.Rounded);
        table.AddColumn(Employees.IdColumn);
        table.AddColumn(Employees.NameColumn);
        table.AddColumn(Employees.RoleColumn);
        table.AddColumn(Employees.ActivityColumn);

        foreach (var e in factory.Employees)
            table.AddRow(
                e.Id.ToString(),
                e.Name,
                e.Role,
                e.ShowActivity()
            );

        AnsiConsole.Write(table);
    }

    private static void AddNewEmployeeFlow(Factory factory, IJsonRepository<Employee> repository,
        ILoggerService loggerService, Employee loggedInUser)
    {
        var name = AnsiConsole.Ask<string>(Employees.EnterFullName);
        var username = AnsiConsole.Ask<string>(Employees.EnterUsername);

        if (string.IsNullOrWhiteSpace(username))
        {
            AnsiConsole.MarkupLine("[red]❌ Username cannot be empty.[/]");
            return;
        }

        if (factory.Employees.Any(e => e.Username.Equals(username, StringComparison.OrdinalIgnoreCase)))
        {
            AnsiConsole.MarkupLine("[red]❌ Username is already taken by another employee. Operation aborted.[/]");
            return;
        }

        var hashedPassword = SecurityHelper.HashPassword("password");

        var role = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title(Employees.SelectJobTitle)
                .AddChoices(MenuOptions.EmployeeRoles));

        Employee newEmployee;

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
                AnsiConsole.MarkupLine(Employees.InvalidRole);
                return;
        }

        factory.AddEmployee(newEmployee);

        var users = repository.Load();
        users.Add(newEmployee);
        repository.Save(users);

        AnsiConsole.MarkupLine(string.Format(Employees.RegisteredSuccessfully, name));
        loggerService.LogInfo(LogOrigin.User, LogEvent.EmployeeAdded,
            $"New user '{username}' ({role}) registered by '{loggedInUser.Username}'");
    }

    private static void RemoveEmployeeFlow(Factory factory, IJsonRepository<Employee> repository,
        ILoggerService loggerService, Employee loggedInUser)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[red]{Employees.RemoveTitle}[/]").Centered());

        var candidates = factory.Employees.Where(e => e.Id != loggedInUser.Id).ToList();

        if (candidates.Count == 0)
        {
            AnsiConsole.MarkupLine(Employees.NoEmployeesToRemove);
            return;
        }

        var selector = new SelectionPrompt<Employee>()
            .Title(Employees.SelectEmployeeToRemove)
            .UseConverter(e => $"{e.Id} - {e.Name} ({e.Role})")
            .AddChoices(candidates);

        var chosen = AnsiConsole.Prompt(selector);

        if (AnsiConsole.Confirm(string.Format(Employees.RemoveConfirmPrompt, chosen.Name, chosen.Role)))
        {
            factory.RemoveEmployee(chosen);

            var users = repository.Load();
            var matchedUser = users.FirstOrDefault(u => u.Id == chosen.Id);
            if (matchedUser != null)
            {
                users.Remove(matchedUser);
                repository.Save(users);
            }

            UndoService.Instance.RegisterCommand(new RemoveEmployeeCommand(chosen, factory, repository,
                loggedInUser.Username));

            AnsiConsole.MarkupLine(string.Format(Employees.RemoveSuccess, chosen.Name));
            loggerService.LogInfo(LogOrigin.User, LogEvent.EmployeeRemoved,
                $"Employee '{chosen.Username}' ({chosen.Role}) removed by '{loggedInUser.Username}'");
        }
        else
        {
            AnsiConsole.MarkupLine(Employees.RemoveCancelled);
        }
    }
}