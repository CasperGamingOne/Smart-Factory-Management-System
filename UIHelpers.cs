using Spectre.Console;

namespace Smart_Factory_Management_System
{
    internal static class UIHelpers
    {
        public static void RenderSessionHeader(Employee user, Factory factory)
        {
            AnsiConsole.Clear();
            var header = new Rule($"[blue]FACTORY CONTROL PANEL - Session: {user.Name}[/]").Centered();

            var info = new Grid().AddColumn().AddRow($"[bold]ID:[/] {user.Id}")
                                   .AddRow($"[bold]Name:[/] {user.Name}")
                                   .AddRow($"[bold]Role:[/] {user.Role}");

            var panel = new Panel(info).Header("[grey]Current User[/]").Border(BoxBorder.Rounded);

            // Write header first, then right-align the session panel (no filler panel).
            AnsiConsole.Write(header);
            AnsiConsole.Write(panel);
            AnsiConsole.WriteLine();
        }
    }
}
