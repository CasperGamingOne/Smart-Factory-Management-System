using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class AccountSettingsHandler
{
    public static void Run(Employee user, IJsonRepository<Employee> repository)
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
                    ChangeFullName(user, repository);
                    break;
                case "2. Change Username":
                    ChangeUsername(user, repository);
                    break;
                case "3. Change Password":
                    ChangePassword(user, repository);
                    break;
                case "4. Return to Main Menu":
                    inSettings = false;
                    break;
            }
        }
    }

    private static void ChangeFullName(Employee user, IJsonRepository<Employee> repository)
    {
        AnsiConsole.WriteLine();
        var newName = AnsiConsole.Ask<string>("Enter your new Full Name:");
        if (string.IsNullOrWhiteSpace(newName))
        {
            AnsiConsole.MarkupLine("[red]❌ Full Name cannot be empty.[/]");
            Thread.Sleep(1000);
            return;
        }

        user.UpdateName(newName);
        PersistChanges(user, repository);
        AnsiConsole.MarkupLine("[green]✅ Full Name updated successfully![/]");
        Thread.Sleep(1000);
    }

    private static void ChangeUsername(Employee user, IJsonRepository<Employee> repository)
    {
        AnsiConsole.WriteLine();
        var newUsername = AnsiConsole.Ask<string>("Enter your new Username:");
        if (string.IsNullOrWhiteSpace(newUsername))
        {
            AnsiConsole.MarkupLine("[red]❌ Username cannot be empty.[/]");
            Thread.Sleep(1000);
            return;
        }

        // Verify uniqueness
        var users = repository.Load();
        if (users.Any(u => u.Id != user.Id && u.Username.Equals(newUsername, StringComparison.OrdinalIgnoreCase)))
        {
            AnsiConsole.MarkupLine("[red]❌ This username is already taken by another account.[/]");
            Thread.Sleep(1500);
            return;
        }

        user.UpdateUsername(newUsername);
        PersistChanges(user, repository);
        AnsiConsole.MarkupLine("[green]✅ Username updated successfully![/]");
        Thread.Sleep(1000);
    }

    private static void ChangePassword(Employee user, IJsonRepository<Employee> repository)
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

        user.ChangePassword(SecurityHelper.HashPassword(newPassword));
        PersistChanges(user, repository);
        AnsiConsole.MarkupLine("[green]✅ Password updated successfully![/]");
        Thread.Sleep(1000);
    }

    private static void PersistChanges(Employee user, IJsonRepository<Employee> repository)
    {
        var users = repository.Load();
        var dbUser = users.FirstOrDefault(u => u.Id == user.Id);
        if (dbUser != null)
        {
            dbUser.UpdateName(user.Name);
            dbUser.UpdateUsername(user.Username);
            dbUser.ChangePassword(user.PasswordHash);

            try
            {
                repository.Save(users);
            }
            catch (IOException)
            {
                AnsiConsole.MarkupLine("[red]❌ Critical error: Could not write to the database file.[/]");
            }
        }
    }
}