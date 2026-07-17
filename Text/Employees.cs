namespace Smart_Factory_Management_System;

public static class Employees
{
    public const string Title = "EMPLOYEE MANAGEMENT MODULE";
    public const string SelectActionPrompt = "[yellow]Select an administrative action:[/]";
    public const string IdColumn = "[yellow]ID[/]";
    public const string NameColumn = "[yellow]Name[/]";
    public const string RoleColumn = "[yellow]Assigned Role[/]";
    public const string ActivityColumn = "[yellow]Activity[/]";
    public const string EnterFullName = "Enter Employee Full Name:";
    public const string EnterUsername = "Enter Employee Username:";
    public const string EnterPassword = "Enter Employee Password:";
    public const string SelectJobTitle = "Select Job Title:";
    public const string InvalidRole = "[red]❌ Invalid role selection. Operation aborted.[/]";
    public const string RegisteredSuccessfully = "[green]✔ Employee '{0}' registered successfully![/]";

    public const string RemoveTitle = "Remove Employee";
    public const string NoEmployeesToRemove = "[yellow]No other employees registered in the system to remove.[/]";
    public const string SelectEmployeeToRemove = "Select employee to remove:";
    public const string RemoveConfirmPrompt = "Are you sure you want to permanently remove employee [red]{0}[/] ({1})?";
    public const string RemoveSuccess = "[green]✔ Employee {0} has been successfully removed from the system.[/]";
    public const string RemoveCancelled = "[yellow]Removal cancelled.[/]";
}