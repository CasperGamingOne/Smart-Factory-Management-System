using System.Text;
using Spectre.Console;

namespace Smart_Factory_Management_System;

internal class Program
{
    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;

        // Initialize core factory and seed data
        var factory = new Factory();

        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]SMART FACTORY SYSTEM - LOGIN GATEWAY[/]").Centered());

            var loggedInUser = LoginHandler.ShowLoginScreen(factory);
            if (loggedInUser == null)
            {
                AnsiConsole.MarkupLine("[red]Application shutting down...[/]");
                break;
            }

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
                    ExecuteQuickAction(loggedInUser, factory);
                }
                else
                {
                    switch (option)
                    {
                        case "Employee Management":
                            EmployeeMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "Machine Management":
                            MachineMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "Product Management":
                            ProductMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "Accounting":
                            AccountingMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "Reports":
                            ReportMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "Factory Information":
                            FactoryReportMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "Log Out / Exit Session":
                            AnsiConsole.MarkupLine("[yellow]Logging out of current profile...[/]");
                            Thread.Sleep(600);
                            sessionActive = false;
                            break;
                    }
                }
            }
        }
    }

    private static void ExecuteQuickAction(Employee user, Factory factory)
    {
        if (user is Director) EmployeeMenuHandler.Run(factory, user);
        else if (user is Technician) MachineMenuHandler.Run(factory, user);
        else if (user is SalesAgent) SalesMenuHandler.Run(factory, user);
        else if (user is Accountant) AccountingMenuHandler.Run(factory, user);
    }
}