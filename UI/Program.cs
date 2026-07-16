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
        var employeeRepo = new JsonRepository<Employee>(fileSystem, "employees.json");
        var machinesRepo = new JsonRepository<Machine>(fileSystem, "machines.json");
        var productsRepo = new JsonRepository<Product>(fileSystem, "products.json");
        var accountService = new AccountService(employeeRepo);

        var dataSeeder = new DataSeeder(employeeRepo, machinesRepo, productsRepo, fileSystem);
        dataSeeder.Seed();

        factory.LoadFromRepository(
            employeeRepo.Load(),
            machinesRepo.Load(),
            productsRepo.Load()
        );

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[yellow]{Login.LoginGatewayTitle}[/]").Centered());

            var loggedInUser = LoginMenuHandler.ShowLoginScreen(employeeRepo, loggerService);
            if (loggedInUser == null)
            {
                loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.ClosingApplication, loggedInUser?.Username);
                AnsiConsole.MarkupLine(Login.AppShuttingDown);
                break;
            }

            if (loggedInUser.IsFirstTimeLogin) PasswordChangeHandler.Run(loggedInUser, employeeRepo, loggerService);

            AnsiConsole.MarkupLine(string.Format(Login.WelcomeBack, loggedInUser.Name, loggedInUser.Role));
            AnsiConsole.Status().Start(Login.BootingEnvironment, _ => { Thread.Sleep(800); });

            var sessionActive = true;
            while (sessionActive)
            {
                // Render header and session info
                TuiHelper.RenderSessionHeader(loggedInUser, factory);

                // Build role-filtered main menu
                var available = loggedInUser.GetAvailableMenuOptions().ToList();
                var logoutIndex = available.IndexOf("Log Out / Exit Session");
                if (logoutIndex >= 0)
                    available.Insert(logoutIndex, "Account Settings");
                else
                    available.Add("Account Settings");

                var indexedAvailable = available.Select((item, index) => $"{index + 1}. {item}").ToList();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("[yellow]Select an operational module:[/]")
                        .PageSize(10)
                        .AddChoices(indexedAvailable));

                var option = choice.Split(". ", 2)[1];

                if (option == loggedInUser.QuickActionName)
                {
                    ExecuteQuickAction(loggedInUser, factory, employeeRepo, loggerService, machinesRepo, productsRepo);
                }
                else
                {
                    switch (option)
                    {
                        case "Employee Management":
                            EmployeeMenuHandler.Run(factory, loggedInUser, loggerService, employeeRepo);
                            break;
                        case "Machine Management":
                            MachineMenuHandler.Run(factory, loggedInUser, loggerService, machinesRepo, productsRepo);
                            break;
                        case "Product Management":
                            ProductMenuHandler.Run(factory, loggedInUser, loggerService, productsRepo);
                            break;
                        case "View Operation History":
                            loggerService.ShowOperationHistory();
                            AnsiConsole.MarkupLine("\n[grey]Press any key to continue...[/]");
                            Console.ReadKey(true);
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
                        case "Account Settings":
                            AccountSettingsMenuHandler.Run(loggedInUser, accountService, loggerService);
                            break;
                        case "Log Out / Exit Session":
                            loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.Logout, loggedInUser.Username);
                            AnsiConsole.MarkupLine(Login.LoggedOut);
                            Thread.Sleep(600);
                            sessionActive = false;
                            break;
                    }
                }
            }
        }
    }

    private static void ExecuteQuickAction(Employee user, Factory factory, IJsonRepository<Employee> authRepository,
        ILoggerService loggerService, IJsonRepository<Machine> machinesRepo, IJsonRepository<Product> productsRepo)
    {
        if (user is Director) EmployeeMenuHandler.Run(factory, user, loggerService, authRepository);
        else if (user is Technician) MachineMenuHandler.Run(factory, user, loggerService, machinesRepo, productsRepo);
        else if (user is SalesAgent) SalesMenuHandler.Run(factory, user, loggerService, productsRepo);
        else if (user is Accountant) AccountingMenuHandler.Run(factory, user, loggerService);
    }
}