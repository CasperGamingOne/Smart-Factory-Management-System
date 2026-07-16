using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class LoginMenuHandler
{
    public static Employee? ShowLoginScreen(IJsonRepository<Employee> repository, ILoggerService loggerService)
    {
        while (true)
        {
            AnsiConsole.Clear();

            // Elegant Header Panel
            AnsiConsole.Write(
                new Panel(new Text(Login.Title, new Style(Color.Yellow, Color.Black)).Centered())
                    .Border(BoxBorder.Double)
                    .BorderColor(Color.Yellow)
                    .Expand()
            );

            // Ask for Username
            var username = AnsiConsole.Prompt(
                new TextPrompt<string>(Login.UsernamePrompt)
                    .PromptStyle("cyan")
            );

            // Graceful application exit sequence
            if (username.Trim().ToLower() == "exit") return null;

            // Ask for Password
            var password = AnsiConsole.Prompt(
                new TextPrompt<string>(Login.PasswordPrompt)
                    .PromptStyle("cyan")
                    .Secret('*')
            );

            // Authentication call
            var matchedEmployee = Authentication.Authenticate(repository, username, password);

            // Authentication logic
            if (matchedEmployee != null)
            {
                loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.LoginSuccess, matchedEmployee.Username);
                AnsiConsole.MarkupLine(Login.AccessGranted);
                Thread.Sleep(600); // Visual feedback pause
                return matchedEmployee;
            }

            // Error boundary feedback
            loggerService.LogInfo(LogOrigin.SYSTEM, LogEvent.LoginFailed, username);
            AnsiConsole.MarkupLine(Login.InvalidCredentials);
            AnsiConsole.MarkupLine(Login.PressKeyToTryAgain);


            Console.ReadKey(true);
        }
    }
}