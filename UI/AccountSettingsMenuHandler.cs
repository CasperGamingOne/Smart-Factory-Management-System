using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class AccountSettingsMenuHandler
{
    public static void Run(Employee user, IAccountService accountService, ILoggerService loggerService)
    {
        var inSettings = true;
        while (inSettings)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule("[yellow]⚙️ ACCOUNT SETTINGS ⚙️[/]").Centered());
            AnsiConsole.WriteLine();

            // Display current details
            var infoTable = new Table().Border(TableBorder.Rounded);
            infoTable.AddColumn("[cyan]Field[/]");
            infoTable.AddColumn("[cyan]Current Value[/]");
            infoTable.AddRow("Full Name", user.Name);
            infoTable.AddRow("Username", user.Username);
            infoTable.AddRow("Role", user.Role);
            AnsiConsole.Write(infoTable);
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[white]Select settings action:[/]")
                    .AddChoices("1. Change Full Name", "2. Change Username", "3. Change Password",
                        "4. Return to Main Menu")
            );

            switch (choice)
            {
                case "1. Change Full Name":
                    ChangeFullName(user, accountService, loggerService);
                    break;
                case "2. Change Username":
                    ChangeUsername(user, accountService, loggerService);
                    break;
                case "3. Change Password":
                    ChangePassword(user, accountService, loggerService);
                    break;
                case "4. Return to Main Menu":
                    inSettings = false;
                    break;
            }
        }
    }

    private static void ChangeFullName(Employee user, IAccountService accountService, ILoggerService loggerService)
    {
        AnsiConsole.WriteLine();
        var newName = AnsiConsole.Ask<string>("Enter your new Full Name:");

        try
        {
            if (accountService.UpdateFullName(user, newName))
            {
                AnsiConsole.MarkupLine("[green]✅ Full Name updated successfully![/]");
                loggerService.LogInfo(LogOrigin.USER, LogEvent.FullNameUpdated, user.Username);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ Critical error: Could not write to the database file.[/]");
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            AnsiConsole.MarkupLine($"[red]❌ {ex.Message}[/]");
        }

        Thread.Sleep(1000);
    }

    private static void ChangeUsername(Employee user, IAccountService accountService, ILoggerService loggerService)
    {
        AnsiConsole.WriteLine();
        var newUsername = AnsiConsole.Ask<string>("Enter your new Username:");

        try
        {
            if (accountService.UpdateUsername(user, newUsername))
            {
                AnsiConsole.MarkupLine("[green]✅ Username updated successfully![/]");
                loggerService.LogInfo(LogOrigin.USER, LogEvent.UsernameUpdated, user.Username);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ Critical error: Could not write to the database file.[/]");
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            AnsiConsole.MarkupLine($"[red]❌ {ex.Message}[/]");
            Thread.Sleep(1500);
            return;
        }

        Thread.Sleep(1000);
    }

    private static void ChangePassword(Employee user, IAccountService accountService, ILoggerService loggerService)
    {
        AnsiConsole.WriteLine();
        string newPassword;
        while (true)
        {
            newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Enter your new password:[/]")
                    .PromptStyle("cyan")
                    .Secret('*')
            );

            var confirmPassword = AnsiConsole.Prompt(
                new TextPrompt<string>("[white]Confirm your new password:[/]")
                    .PromptStyle("cyan")
                    .Secret('*')
            );

            if (newPassword == confirmPassword) break;

            AnsiConsole.MarkupLine("[red]❌ Passwords do not match. Please try again.[/]");
        }

        try
        {
            if (accountService.UpdatePassword(user, newPassword))
            {
                AnsiConsole.MarkupLine("[green]✅ Password updated successfully![/]");
                loggerService.LogInfo(LogOrigin.USER, LogEvent.PasswordUpdated, user.Username);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]❌ Critical error: Could not write to the database file.[/]");
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            AnsiConsole.MarkupLine($"[red]❌ {ex.Message}[/]");
        }

        Thread.Sleep(1000);
    }
}