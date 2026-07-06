using System.Text;
using Spectre.Console;

namespace Smart_Factory_Management_System
{
    internal class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            // Initialize core factory and seed data
            Factory factory = new Factory();

            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new Rule("[yellow]SMART FACTORY SYSTEM - LOGIN GATEWAY[/]").Centered());

                Employee? loggedInUser = LoginHandler.ShowLoginScreen(factory);
                if (loggedInUser == null)
                {
                    AnsiConsole.MarkupLine("[red]Application shutting down...[/]");
                    break;
                }

                AnsiConsole.MarkupLine($"[green]Welcome back, {loggedInUser.Name} ({loggedInUser.Role})![/]");
                AnsiConsole.Status().Start("Booting production environment...", _ => { Thread.Sleep(800); });

                bool sessionActive = true;
                while (sessionActive)
                {
                    // Render header and session info
                    TUIHelper.RenderSessionHeader(loggedInUser, factory);

                    // Build role-filtered main menu
                    var available = GetAvailableMainMenu(loggedInUser);

                    var choice = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                            .Title("[yellow]Select an operational module:[/]")
                            .PageSize(10)
                            .AddChoices(available));

                    switch (choice)
                    {
                        case "1. Quick Actions":
                            // Delegate to the employee-specific role menu
                            loggedInUser.OpenRoleMenu(factory);
                            break;
                        case "2. Employee Management":
                            EmployeeMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "3. Machine Management":
                            MachineMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "4. Product Management":
                            ProductMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "5. Accounting":
                            AccountingMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "6. Reports":
                            ReportMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "7. Factory Information":
                            FactoryReportMenuHandler.Run(factory, loggedInUser);
                            break;
                        case "8. Log Out / Exit Session":
                            AnsiConsole.MarkupLine("[yellow]Logging out of current profile...[/]");
                            Thread.Sleep(600);
                            sessionActive = false;
                            break;
                    }
                }
            }
        }

        private static List<string> GetAvailableMainMenu(Employee loggedInUser)
        {
            var available = new List<string>();

            if (loggedInUser is Director)
            {
                available.AddRange(MenuOptions.MainMenu);
            }
            else if (loggedInUser is Technician)
            {
                available.Add("1. Quick Actions");
                available.Add("3. Machine Management");
                available.Add("4. Product Management");
                available.Add("6. Reports");
                available.Add("7. Factory Information");
                available.Add("8. Log Out / Exit Session");
            }
            else if (loggedInUser is SalesAgent)
            {
                available.Add("1. Quick Actions");
                available.Add("4. Product Management");
                available.Add("5. Accounting");
                available.Add("6. Reports");
                available.Add("7. Factory Information");
                available.Add("8. Log Out / Exit Session");
            }
            else if (loggedInUser is Accountant)
            {
                available.Add("4. Product Management");
                available.Add("5. Accounting");
                available.Add("6. Reports");
                available.Add("7. Factory Information");
                available.Add("8. Log Out / Exit Session");
            }
            else
            {
                available.Add("4. Product Management");
                available.Add("6. Reports");
                available.Add("7. Factory Information");
                available.Add("8. Log Out / Exit Session");
            }

            return available;
        }
    }
}