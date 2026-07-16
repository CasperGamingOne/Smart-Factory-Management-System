using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class PasswordChangeHandler
{
    public static void Run(Employee user, IJsonRepository<Employee> repository, ILoggerService loggerService)
    {
        AnsiConsole.Clear();
        AnsiConsole.Write(new Rule($"[yellow]{Login.ChangePasswordTitle}[/]").Centered());
        AnsiConsole.MarkupLine(string.Format(Login.FirstTimeMessage, user.Name));

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

        var hashedPassword = SecurityHelper.HashPassword(newPassword);
        user.ChangePassword(hashedPassword);

        // Need to save the changes
        // Persist changes
        var users = repository.Load();
        var userToUpdate = users.FirstOrDefault(u => u.Id == user.Id);

        if (userToUpdate != null)
        {
            userToUpdate.ChangePassword(hashedPassword);

            try
            {
                repository.Save(users);
            }
            catch (IOException)
            {
                // Log the actual error internally if needed
                AnsiConsole.MarkupLine(Common.CriticalErrorDb);
            }
        }
        else
        {
            AnsiConsole.MarkupLine(string.Format(Login.UserNotFound, user.Id));
        }

        AnsiConsole.MarkupLine(Login.PasswordUpdated);
        loggerService.LogInfo(LogOrigin.USER, LogEvent.PasswordChangedFirstLogin, user.Username);
        Thread.Sleep(1000);
    }
}