namespace Smart_Factory_Management_System;

public static class UndoText
{
    public const string Title = "UNDO / REVERSE OPERATIONS";
    public const string SelectPrompt = "Select an operation to [red]UNDO[/] (Newest first):";
    public const string UndoSuccess = "[green]✔ Operation successfully undone/reversed![/]";
    public const string UndoError = "[red]❌ Failed to undo: {0}[/]";
    public const string NoOperations = "[yellow]No undoable operations found.[/]";
    public const string CancelAndReturn = "Cancel and Return";

    public const string PasswordWarningHeader = "Undo System Notice";

    public const string PasswordWarningMessage =
        "[bold red]⚠ WARNING:[/] Once changed, password actions cannot be undone/reversed!";

    public const string PasswordPrompt = "Enter your password to confirm change:";
    public const string IncorrectPassword = "[red]❌ Incorrect password. Action aborted.[/]";

    public const string LastActionInfo = "Last Action: [[{0:yyyy-MM-dd HH:mm:ss}]] [cyan]By:[/] {1} - [green]{2}[/]";
    public const string ConfirmUndoPrompt = "Do you want to undo this action?";
    public const string ConfirmYes = "Yes, Undo Last Action";
    public const string LogMessageFormat = "Operation undone: {0}";
}