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
            AnsiConsole.Write(new Rule($"[yellow]{AccountSettings.Title}[/]").Centered());
            AnsiConsole.WriteLine();

            // Display current details
            var infoTable = new Table().Border(TableBorder.Rounded);
            infoTable.AddColumn(AccountSettings.FieldColumn);
            infoTable.AddColumn(AccountSettings.ValueColumn);
            infoTable.AddRow(AccountSettings.FullNameRow, user.Name);
            infoTable.AddRow(AccountSettings.UsernameRow, user.Username);
            infoTable.AddRow(AccountSettings.RoleRow, user.Role);
            AnsiConsole.Write(infoTable);
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(AccountSettings.SelectActionPrompt)
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
        var newName = AnsiConsole.Ask<string>(AccountSettings.EnterNewFullName);

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>(UndoText.PasswordPrompt)
                .PromptStyle("cyan")
                .Secret('*')
        );
        if (!SecurityHelper.VerifyPassword(password, user.PasswordHash))
        {
            AnsiConsole.MarkupLine(UndoText.IncorrectPassword);
            Thread.Sleep(1500);
            return;
        }

        try
        {
            var oldName = user.Name;
            if (accountService.UpdateFullName(user, newName))
            {
                AnsiConsole.MarkupLine(AccountSettings.FullNameUpdated);
                loggerService.LogInfo(LogOrigin.USER, LogEvent.FullNameUpdated, user.Username);
                UndoService.Instance.RegisterCommand(new ChangeFullNameCommand(user, oldName, newName, accountService,
                    user.Username));
            }
            else
            {
                AnsiConsole.MarkupLine(Common.CriticalErrorDb);
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
        var newUsername = AnsiConsole.Ask<string>(AccountSettings.EnterNewUsername);

        var password = AnsiConsole.Prompt(
            new TextPrompt<string>(UndoText.PasswordPrompt)
                .PromptStyle("cyan")
                .Secret('*')
        );
        if (!SecurityHelper.VerifyPassword(password, user.PasswordHash))
        {
            AnsiConsole.MarkupLine(UndoText.IncorrectPassword);
            Thread.Sleep(1500);
            return;
        }

        try
        {
            var oldUsername = user.Username;
            if (accountService.UpdateUsername(user, newUsername))
            {
                AnsiConsole.MarkupLine(AccountSettings.UsernameUpdated);
                loggerService.LogInfo(LogOrigin.USER, LogEvent.UsernameUpdated, user.Username);
                UndoService.Instance.RegisterCommand(new ChangeUsernameCommand(user, oldUsername, newUsername,
                    accountService, user.Username));
            }
            else
            {
                AnsiConsole.MarkupLine(Common.CriticalErrorDb);
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
        var warningPanel = new Panel(UndoText.PasswordWarningMessage)
        {
            Border = BoxBorder.Double,
            Padding = new Padding(1, 1, 1, 1),
            Header = new PanelHeader($"[yellow]{UndoText.PasswordWarningHeader}[/]")
        };
        AnsiConsole.Write(warningPanel);
        AnsiConsole.WriteLine();

        string newPassword;
        while (true)
        {
            newPassword = AnsiConsole.Prompt(
                new TextPrompt<string>(Login.NewPasswordPrompt)
                    .PromptStyle("cyan")
                    .Secret('*')
            );

            var confirmPassword = AnsiConsole.Prompt(
                new TextPrompt<string>(Login.ConfirmPasswordPrompt)
                    .PromptStyle("cyan")
                    .Secret('*')
            );

            if (newPassword == confirmPassword) break;

            AnsiConsole.MarkupLine(Login.PasswordsDoNotMatch);
        }

        try
        {
            if (accountService.UpdatePassword(user, newPassword))
            {
                AnsiConsole.MarkupLine(AccountSettings.PasswordUpdated);
                loggerService.LogInfo(LogOrigin.USER, LogEvent.PasswordUpdated, user.Username);
            }
            else
            {
                AnsiConsole.MarkupLine(Common.CriticalErrorDb);
            }
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            AnsiConsole.MarkupLine($"[red]❌ {ex.Message}[/]");
        }

        Thread.Sleep(1000);
    }
}