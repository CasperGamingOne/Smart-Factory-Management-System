using Spectre.Console;

namespace Smart_Factory_Management_System
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

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
                AnsiConsole.Status().Start("Booting production environment...", ctx => { System.Threading.Thread.Sleep(800); });

                bool sessionActive = true;
                while (sessionActive)
                {
                    // Render header and session info
                    UIHelpers.RenderSessionHeader(loggedInUser, factory);

                    // Build role-filtered main menu
                    var available = new System.Collections.Generic.List<string>();
                    if (loggedInUser is Director)
                        available.AddRange(MenuOptions.MainMenu);
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
                        // Fallback to limited menu
                        available.Add("4. Product Management");
                        available.Add("6. Reports");
                        available.Add("7. Factory Information");
                        available.Add("8. Log Out / Exit Session");
                    }

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
                            AnsiConsole.MarkupLine($"[bold]Factory Capacity Metrics:[/]");
                            AnsiConsole.MarkupLine($"Machines Configured: [cyan]{factory.MachineCount}[/]");
                            AnsiConsole.MarkupLine($"Active Staff: [cyan]{factory.EmployeeCount}[/]");
                            AnsiConsole.MarkupLine("\nPress any key to return...");
                            Console.ReadKey();
                            break;
                        case "8. Log Out / Exit Session":
                            AnsiConsole.MarkupLine("[yellow]Logging out of current profile...[/]");
                            System.Threading.Thread.Sleep(600);
                            sessionActive = false;
                            break;
                        default:
                            // Safety fallback — should not happen with SelectionPrompt choices
                            AnsiConsole.MarkupLine("[red]Unknown selection — returning to main menu[/]");
                            System.Threading.Thread.Sleep(500);
                            break;
                    }
                }
            }
        }
    }
}