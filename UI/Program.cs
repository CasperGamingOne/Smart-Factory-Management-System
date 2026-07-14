using System.Text;
using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class Program
{
    private static void Main()
    {
        Console.Title = "Smart Factory Management System";
        Console.OutputEncoding = Encoding.UTF8;

        // Initialize core factory and seed data
        var fileSystem = new FileSystemService();
        var factory = new Factory();
        var loggerService = new LoggerService(fileSystem);
        var authRepository = new JsonAuthRepository(fileSystem);
        var employees = authRepository.LoadUsers();
        foreach (var employee in employees) factory.AddEmployee(employee);

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]SMART FACTORY SYSTEM - LOGIN GATEWAY[/]").Centered());

            var loggedInUser = LoginMenuHandler.ShowLoginScreen(authRepository, loggerService);
            if (loggedInUser == null)
            {
                loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.ClosingApplication, loggedInUser?.Username);
                AnsiConsole.MarkupLine("[red]Application shutting down...[/]");
                break;
            }

            if (loggedInUser.IsFirstTimeLogin) PasswordChangeHandler.Run(loggedInUser, authRepository);

            AnsiConsole.MarkupLine($"[green]Welcome back, {loggedInUser.Name} ({loggedInUser.Role})![/]");
            AnsiConsole.Status().Start("Booting production environment...", _ => { Thread.Sleep(800); });

            var sessionActive = true;
            while (sessionActive)
            {
                // Render header and session info
                TuiHelper.RenderSessionHeader(loggedInUser, factory);

                // Build role-filtered main menu
                var available = loggedInUser.GetAvailableMenuOptions();
                var indexedAvailable = available.Select((item, index) => $"{index + 1}. {item}").ToList();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an operational module:[/]")
                        .PageSize(10)
                        .AddChoices(indexedAvailable));

                var option = choice.Split(". ", 2)[1];

                if (option == loggedInUser.QuickActionName)
                {
                    ExecuteQuickAction(loggedInUser, factory, authRepository, loggerService);
                }
                else
                {
                    switch (option)
                    {
                        case "Employee Management":
                            EmployeeMenuHandler.Run(factory, loggedInUser, authRepository);
                            break;
                        case "Machine Management":
                            MachineMenuHandler.Run(factory, loggedInUser, loggerService);
                            break;
                        case "Product Management":
                            ProductMenuHandler.Run(factory, loggedInUser, loggerService);
                            break;
                        case "Accounting":
                            AccountingMenuHandler.Run(factory, loggedInUser, loggerService);
                            break;
                        case "Reports":
                            ReportMenuHandler.Run(factory, loggedInUser, loggerService);
                            break;
                        case "Factory Information":
                            FactoryReportMenuHandler.Run(factory, loggedInUser, loggerService);
                            break;
                        case "Log Out / Exit Session":
                            loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.Logout, loggedInUser.Username);
                            AnsiConsole.MarkupLine("[yellow]Logging out of current profile...[/]");
                            Thread.Sleep(600);
                            sessionActive = false;
                            break;
                    }
                }
            }
        }
    }

    private static void ExecuteQuickAction(Employee user, Factory factory, IAuthRepository<Employee> authRepository,
        ILoggerService loggerService)
    {
        if (user is Director) EmployeeMenuHandler.Run(factory, user, authRepository);
        else if (user is Technician) MachineMenuHandler.Run(factory, user, loggerService);
        else if (user is SalesAgent) SalesMenuHandler.Run(factory, user, loggerService);
        else if (user is Accountant) AccountingMenuHandler.Run(factory, user, loggerService);
    }
}