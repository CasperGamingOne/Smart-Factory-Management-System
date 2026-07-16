namespace Smart_Factory_Management_System;

public static class Login
{
    public const string Title = "FACTORY ACCESS GATEWAY";
    public const string LoginGatewayTitle = "SMART FACTORY SYSTEM - LOGIN GATEWAY";
    public const string UsernamePrompt = "[white]Enter your Username (or type 'exit' to quit):[/]";
    public const string PasswordPrompt = "[white]Enter your Password:[/]";
    public const string AccessGranted = "[green]✔ Access Granted successfully![/]";
    public const string InvalidCredentials = "[red]❌ Error: Invalid username or password.[/]";
    public const string PressKeyToTryAgain = "[grey]Press any key to try again...[/]";
    public const string AppShuttingDown = "[red]Application shutting down...[/]";
    public const string WelcomeBack = "[green]Welcome back, {0} ({1})![/]";
    public const string BootingEnvironment = "Booting production environment...";
    public const string LoggedOut = "[yellow]Logging out of current profile...[/]";
    public const string ChangePasswordTitle = "FIRST-TIME PASSWORD SETUP";
    public const string FirstTimeMessage = "[green]Hello {0}, you must set a new password for your account.[/]";
    public const string NewPasswordPrompt = "[white]Enter your new password:[/]";
    public const string ConfirmPasswordPrompt = "[white]Confirm your new password:[/]";
    public const string PasswordsDoNotMatch = "[red]❌ Passwords do not match. Please try again.[/]";
    public const string PasswordUpdated = "[green]✔ Password updated successfully![/]";
    public const string UserNotFound = "[red]Error: Could not find user with ID {0} in database.[/]";
}