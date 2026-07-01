using Spectre.Console;

namespace Smart_Factory_Management_System
{
        internal static class LoginHandler
        {
            public static Employee? ShowLoginScreen(Factory factory)
            {
                while (true)
                {
                    AnsiConsole.Clear();

                    // Elegant Header Panel
                    AnsiConsole.Write(
                        new Panel(new Text("FACTORY ACCESS GATEWAY", new Style(Color.Yellow, Color.Black)).Centered())
                            .Border(BoxBorder.Double)
                            .BorderColor(Color.Yellow)
                            .Expand()
                    );

                    // Ask only for the Employee ID
                    string empId = AnsiConsole.Prompt(
                        new TextPrompt<string>("[white]Enter your Employee ID (or type 'exit' to quit):[/]")
                            .PromptStyle("cyan")
                    );

                    // Graceful application exit sequence
                    if (empId.Trim().ToLower() == "exit")
                    {
                        return null; // Signals Program.cs to close down
                    }

                    // Null-safe check inside the internal Factory collection up to EmployeeCount
                    Employee? matchedEmployee = null;
                    for (int i = 0; i < factory.EmployeeCount; i++)
                    {
                        if (factory.Employees[i] != null && factory.Employees[i].Id.Equals(empId, StringComparison.OrdinalIgnoreCase))
                        {
                            matchedEmployee = factory.Employees[i];
                            break;
                        }
                    }

                    // Authentication purely by ID validation
                    if (matchedEmployee != null)
                    {
                        AnsiConsole.MarkupLine("[green]✔ Access Granted successfully![/]");
                        System.Threading.Thread.Sleep(600); // Visual feedback pause
                        return matchedEmployee; // Immediately hands control and user context back to Program.cs
                    }
                    else
                    {
                        // Error boundary feedback if the ID doesn't match seeded array values
                        AnsiConsole.MarkupLine("[red]❌ Error: Employee ID not found in system registers.[/]");
                        AnsiConsole.MarkupLine("[grey]Press any key to try again...[/]");
                        Console.ReadKey(true);
                    }
                }
            }
        }
}