using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class LoginMenuHandler
{
    public static Employee? ShowLoginScreen(IAuthRepository<Employee> repository, ILoggerService loggerService)
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

            // Ask for Username
            var username = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Enter your Username (or type 'exit' to quit):[/]")
                    .PromptStyle("cyan")
            );

            // Graceful application exit sequence
            if (username.Trim().ToLower() == "exit") return null;

            // Ask for Password
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Enter your Password:[/]")
                    .PromptStyle("cyan")
                    .Secret('*')
            );

            // Authentication call
            var matchedEmployee = Authentication.Authenticate(repository, username, password);

            // Authentication logic
            if (matchedEmployee != null)
            {
                loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.LoginSuccess, matchedEmployee.Username);
                AnsiConsole.MarkupLine("[green]✔ Access Granted successfully![/]");
                Thread.Sleep(600); // Visual feedback pause
                return matchedEmployee;
            }

            // Error boundary feedback
            loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.LoginFailed, username);
            AnsiConsole.MarkupLine("[red]❌ Error: Invalid username or password.[/]");
            AnsiConsole.MarkupLine("[grey]Press any key to try again...[/]");


            Console.ReadKey(true);
        }
    }
}