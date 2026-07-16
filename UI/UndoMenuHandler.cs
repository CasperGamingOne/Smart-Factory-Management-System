using Spectre.Console;

namespace Smart_Factory_Management_System;

public static class UndoMenuHandler
{
    public static void Run(Employee user, ILoggerService loggerService)
    {
        while (true)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(new Rule($"[yellow]{UndoText.Title}[/]").Centered());
            AnsiConsole.WriteLine();

            var commands = UndoService.Instance.GetUndoableCommands(user);

            if (commands.Count == 0)
            {
                AnsiConsole.MarkupLine(UndoText.NoOperations);
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine("[grey]Press any key to return...[/]");
                Console.ReadKey(true);
                return;
            }

            // Get the last action (newest command in the filtered list)
            var lastCommand = commands[commands.Count - 1];

            AnsiConsole.MarkupLine(string.Format(UndoText.LastActionInfo, lastCommand.ExecutedAt,
                Markup.Escape(lastCommand.ExecutedBy), Markup.Escape(lastCommand.Description)));
            AnsiConsole.WriteLine();

            var choice = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title(UndoText.ConfirmUndoPrompt)
                    .AddChoices(UndoText.ConfirmYes, UndoText.CancelAndReturn)
            );

            if (choice == UndoText.CancelAndReturn) return;

            if (choice == UndoText.ConfirmYes)
            {
                try
                {
                    UndoService.Instance.UndoCommand(lastCommand);
                    loggerService.LogInfo(LogOrigin.UNDO, LogEvent.OperationUndone,
                        $"{lastCommand.Description} (Executed by {lastCommand.ExecutedBy})");
                    AnsiConsole.MarkupLine(UndoText.UndoSuccess);
                }
                catch (Exception ex)
                {
                    AnsiConsole.MarkupLine(string.Format(UndoText.UndoError, ex.Message));
                }

                Thread.Sleep(1500);
            }
        }
    }
}