using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class PasswordChangeHandler
{
    public static void Run(Employee user, IAuthRepository<Employee> repository)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule("[yellow]FIRST-TIME PASSWORD SETUP[/]").Centered());
        AnsiConsole.MarkupLine($"[green]Hello {user.Name}, you must set a new password for your account.[/]");

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

        user.PasswordHash = SecurityHelper.HashPassword(newPassword);
        user.IsFirstTimeLogin = false;

        // Need to save the changes
        // Persist changes
        var users = repository.LoadUsers();
        var userToUpdate = users.FirstOrDefault(u => u.Id == user.Id);

        if (userToUpdate != null)
        {
            userToUpdate.PasswordHash = SecurityHelper.HashPassword(newPassword);
            userToUpdate.IsFirstTimeLogin = false;

            try
            {
                repository.SaveUsers(users);
                AnsiConsole.MarkupLine("[green]✅ Password updated successfully.[/]");
            }
            catch (IOException)
            {
                // Log the actual error internally if needed
                AnsiConsole.MarkupLine("[red]❌ Critical error: Could not write to the database file.[/]");
            }
        }
        else
        {
            AnsiConsole.MarkupLine($"[red]Error: Could not find user with ID {user.Id} in database.[/]");
        }

        AnsiConsole.MarkupLine("[green]✔ Password updated successfully![/]");
        Thread.Sleep(1000);
    }
}