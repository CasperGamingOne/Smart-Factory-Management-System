using Spectre.Console;

namespace Smart_Factory_Management_System;

internal static class TuiHelper
{
    public static void RenderSessionHeader(Employee user)
    {
        AnsiConsole.Clear();
        var header = new Rule($"[blue]FACTORY CONTROL PANEL - Session: {user.Name}[/]").Centered();
        var activity = user.ShowActivity();

        var info = new Grid().AddColumn().AddRow($"[bold]ID:[/] {user.Id}")
            .AddRow($"[bold]Name:[/] {user.Name}")
            .AddRow($"[bold]Role:[/] {user.Role}")
            .AddRow($"[bold]Activity:[/] {Markup.Escape(activity)}");

        var panel = new Panel(info).Header("[grey]Current User[/]").Border(BoxBorder.Rounded);

        // Write the session header and user panel.
        AnsiConsole.Write(header);
        AnsiConsole.Write(panel);
        AnsiConsole.WriteLine();
    }
}